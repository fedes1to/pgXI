using System;
using System.Collections.Generic;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class BackSystem : MonoBehaviour
	{
		private sealed class Subscription : IDisposable
		{
			private Action _callback;

			private readonly string _context;

			private LinkedListNode<Subscription> _node;

			public string Context
			{
				get
				{
					return _context;
				}
			}

			public bool Disposed
			{
				get
				{
					return _node == null;
				}
			}

			internal Subscription(Action callback, string context, LinkedList<Subscription> list)
			{
				_callback = callback;
				_context = context ?? string.Empty;
				if (list != null)
				{
					_node = list.AddLast(this);
				}
			}

			public void Dispose()
			{
				if (!Disposed)
				{
					_callback = null;
					LinkedList<Subscription> list = _node.List;
					if (list != null)
					{
						list.Remove(_node);
					}
					_node = null;
				}
			}

			public void Invoke()
			{
				if (Disposed)
				{
					Debug.LogWarning("Attempt to invoke disposed handler.");
				}
				else if (_callback != null)
				{
					_callback();
				}
			}
		}

		private static readonly Lazy<BackSystem> _instance = new Lazy<BackSystem>(InitializeInstance);

		private readonly LinkedList<Subscription> _subscriptions = new LinkedList<Subscription>();

		public static BackSystem Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		public static bool Active
		{
			get
			{
				return true;
			}
		}

		public IDisposable Register(Action callback, string context = null)
		{
			Subscription result = new Subscription(callback, context, _subscriptions);
			if (Application.isEditor)
			{
				Debug.Log(string.Format("<color=lightblue>Back stack after registration: {0}</color>", this));
			}
			return result;
		}

		public override string ToString()
		{
			return Json.Serialize(ToJson());
		}

		private void Update()
		{
			if (EscapePressed())
			{
				Input.ResetInputAxes();
				string arg = ((!Application.isEditor) ? string.Empty : ToString());
				CollectGarbage();
				LinkedListNode<Subscription> last = _subscriptions.Last;
				if (last != null)
				{
					last.Value.Invoke();
					if (Application.isEditor)
					{
						Debug.Log(string.Format("<color=#db7093ff>Back stack on invoke: {0} -> {1}</color>", arg, this));
					}
				}
			}
			else if (Input.GetKeyUp(KeyCode.Backspace) && Application.isEditor)
			{
				Debug.Log(string.Format("<color=#db7093ff>Current back stack: {0}</color>", this));
			}
		}

		private static bool EscapePressed()
		{
			if (!Active)
			{
				return false;
			}
			return Input.GetKeyUp(KeyCode.Escape);
		}

		private object ToJson()
		{
			List<string> list = new List<string>(_subscriptions.Count);
			foreach (Subscription subscription in _subscriptions)
			{
				if (subscription.Disposed)
				{
					list.Add(subscription.Context + " (Disposed)");
				}
				else
				{
					list.Add(subscription.Context);
				}
			}
			return list;
		}

		private void CollectGarbage()
		{
			LinkedListNode<Subscription> last = _subscriptions.Last;
			while (last != null && last.Value.Disposed)
			{
				_subscriptions.RemoveLast();
				last = _subscriptions.Last;
			}
		}

		private static BackSystem InitializeInstance()
		{
			BackSystem backSystem = UnityEngine.Object.FindObjectOfType<BackSystem>();
			if (backSystem != null)
			{
				UnityEngine.Object.DontDestroyOnLoad(backSystem.gameObject);
				return backSystem;
			}
			GameObject gameObject = new GameObject("Rilisoft.BackSystem");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<BackSystem>();
		}
	}
}

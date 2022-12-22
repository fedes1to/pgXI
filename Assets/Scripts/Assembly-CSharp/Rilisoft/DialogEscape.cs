using System;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class DialogEscape : MonoBehaviour
	{
		private readonly Lazy<ButtonHandler> _buttonHandler;

		private IDisposable _escapeSubscription;

		public string Context { get; set; }

		public DialogEscape()
		{
			_buttonHandler = new Lazy<ButtonHandler>(base.GetComponent<ButtonHandler>);
		}

		private void OnEnable()
		{
			if (_escapeSubscription != null)
			{
				_escapeSubscription.Dispose();
			}
			_escapeSubscription = BackSystem.Instance.Register(HandleEscape, Context ?? "Dialog");
		}

		private void OnDisable()
		{
			if (_escapeSubscription != null)
			{
				_escapeSubscription.Dispose();
				_escapeSubscription = null;
			}
		}

		private void HandleEscape()
		{
			if (_buttonHandler.Value != null)
			{
				_buttonHandler.Value.DoClick();
			}
		}
	}
}

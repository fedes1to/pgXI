using UnityEngine;

namespace Rilisoft
{
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private const string MSG_DUPLICATE = "[Singleton] Something went really wrong - there should never be more than 1 singleton!";

		private const string MSG_PREFAB_NOT_SETTED = "[Singleton] prefab not setted";

		private const string MSG_NOT_FOUND_IN_PREFAB = "[Singleton] can not find singleton class in prefab";

		private static T _instance;

		protected static bool IsSetted
		{
			get
			{
				return (Object)_instance != (Object)null;
			}
		}

		public static T Instance
		{
			get
			{
				if ((Object)_instance != (Object)null)
				{
					return _instance;
				}
				_instance = Object.FindObjectOfType<T>();
				if ((Object)_instance != (Object)null)
				{
					_instance.SendMessage("OnInstanceCreated", SendMessageOptions.DontRequireReceiver);
					return _instance;
				}
				GameObject gameObject = null;
				ISingletonFromPrefab singletonFromPrefab = _instance as ISingletonFromPrefab;
				if (singletonFromPrefab != null)
				{
					if (singletonFromPrefab.SingletonPrefab != null)
					{
						GameObject gameObject2 = Object.Instantiate(singletonFromPrefab.SingletonPrefab);
						T component = gameObject2.GetComponent<T>();
						if ((Object)component != (Object)null)
						{
							_instance = component;
							gameObject = gameObject2;
						}
						else
						{
							Debug.LogError("[Singleton] can not find singleton class in prefab");
						}
					}
					else
					{
						Debug.LogError("[Singleton] prefab not setted");
					}
				}
				else
				{
					gameObject = new GameObject(typeof(T).Name);
					_instance = gameObject.AddComponent<T>();
				}
				gameObject.name += " [Singleton]";
				Object.DontDestroyOnLoad(gameObject);
				_instance.SendMessage("OnInstanceCreated", SendMessageOptions.DontRequireReceiver);
				if (Object.FindObjectsOfType<T>().Length > 1)
				{
					Debug.LogError("[Singleton] Something went really wrong - there should never be more than 1 singleton!");
				}
				return _instance;
			}
			protected set
			{
				_instance = value;
				if (Object.FindObjectsOfType<T>().Length > 1)
				{
					Debug.LogError("[Singleton] Something went really wrong - there should never be more than 1 singleton!");
				}
				Object.DontDestroyOnLoad(_instance.gameObject);
				_instance.SendMessage("OnInstanceCreated", SendMessageOptions.DontRequireReceiver);
			}
		}

		protected virtual void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}

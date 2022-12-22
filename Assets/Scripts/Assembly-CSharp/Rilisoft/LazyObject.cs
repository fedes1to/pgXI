using UnityEngine;

namespace Rilisoft
{
	public class LazyObject<T> where T : MonoBehaviour
	{
		private readonly string _resourcePath;

		private GameObject _prefabVal;

		private T _value;

		private readonly GameObject _attachTo;

		private GameObject _prefab
		{
			get
			{
				if (_prefabVal == null && !_resourcePath.IsNullOrEmpty())
				{
					_prefabVal = Resources.Load<GameObject>(_resourcePath);
				}
				return _prefabVal;
			}
			set
			{
				_prefabVal = value;
			}
		}

		public T Value
		{
			get
			{
				if ((Object)_value == (Object)null)
				{
					GameObject gameObject = Object.Instantiate(_prefab);
					_value = gameObject.GetComponent<T>();
					if (_attachTo != null)
					{
						gameObject.transform.SetParent(_attachTo.transform);
						gameObject.transform.localPosition = _attachTo.transform.localPosition;
						gameObject.transform.localScale = Vector3.one;
					}
				}
				return _value;
			}
		}

		public bool HasValue
		{
			get
			{
				return (Object)_value != (Object)null;
			}
		}

		public bool ObjectIsLoaded
		{
			get
			{
				return (Object)_value != (Object)null;
			}
		}

		public bool ObjectIsActive
		{
			get
			{
				return ObjectIsLoaded && _value.gameObject.activeInHierarchy;
			}
		}

		public LazyObject(GameObject prefab, GameObject attachTo)
		{
			_prefab = prefab;
			_attachTo = attachTo;
		}

		public LazyObject(string resourcePath, GameObject attachTo)
		{
			_resourcePath = resourcePath;
			_attachTo = attachTo;
		}

		public void DestroyValue()
		{
			if ((Object)_value != (Object)null)
			{
				Object.DestroyImmediate(_value.gameObject);
			}
		}
	}
}

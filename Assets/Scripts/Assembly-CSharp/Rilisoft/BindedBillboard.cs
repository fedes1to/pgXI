using System;
using UnityEngine;

namespace Rilisoft
{
	public class BindedBillboard : MonoBehaviour
	{
		[SerializeField]
		private Transform _pointInWorld;

		[SerializeField]
		private Collider _touchCollider;

		private Func<Transform> _pointGetter;

		public Collider Collider
		{
			get
			{
				if (_touchCollider == null)
				{
					_touchCollider = base.gameObject.GetComponent<Collider>();
					if (_touchCollider == null)
					{
						_touchCollider = GetComponentInChildren<Collider>();
					}
				}
				return _touchCollider;
			}
		}

		public void BindTo(Func<Transform> point)
		{
			_pointInWorld = null;
			_pointGetter = point;
		}

		private void Awake()
		{
			if (_pointGetter == null && _pointInWorld != null)
			{
				_pointGetter = () => _pointInWorld;
			}
		}

		private void Update()
		{
			if (_pointInWorld != null)
			{
				_pointGetter = () => _pointInWorld;
			}
			if (_pointGetter != null && !(_pointGetter() == null) && !(NickLabelController.currentCamera == null))
			{
				base.transform.OverlayPosition(_pointGetter());
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, 0f);
				if (Collider != null)
				{
					Collider.enabled = _pointGetter().gameObject.activeSelf;
				}
			}
		}
	}
}

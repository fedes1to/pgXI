using UnityEngine;

namespace Rilisoft
{
	public class NestEggClickHandler : MonoBehaviour
	{
		[SerializeField]
		private GameObject _eggObject;

		[SerializeField]
		private NestBanner _banner;

		[SerializeField]
		private float _sensitivity = 0.1f;

		[SerializeField]
		private float _thresholdPercent = 1f;

		private Quaternion _eggBaseRotation;

		private bool _isClicking;

		private bool _isDragging;

		private Vector3 _mouseDownPos;

		private Vector3 _prevMousePos;

		private float Threshhold
		{
			get
			{
				return (float)Screen.width * _thresholdPercent * 0.01f;
			}
		}

		private void Awake()
		{
			_eggBaseRotation = _eggObject.transform.localRotation;
		}

		private void Update()
		{
			if (_banner.IsVisible)
			{
				if (Input.GetMouseButtonDown(0))
				{
					MouseDown();
				}
				if (Input.GetMouseButtonUp(0))
				{
					MouseUp();
				}
				if (_isClicking && !_isDragging && Mathf.Abs(_mouseDownPos.x - Input.mousePosition.x) >= Threshhold)
				{
					_isDragging = true;
				}
				if (_isDragging)
				{
					Vector3 vector = _prevMousePos - Input.mousePosition;
					Vector3 euler = new Vector3(_eggObject.transform.localEulerAngles.x, _eggObject.transform.localEulerAngles.y + vector.x * _sensitivity, _eggObject.transform.localEulerAngles.z);
					_eggObject.transform.localRotation = Quaternion.Euler(euler);
				}
				_prevMousePos = Input.mousePosition;
			}
		}

		private void MouseDown()
		{
			if (_banner.IsVisible)
			{
				_isClicking = true;
				_mouseDownPos = Input.mousePosition;
				_prevMousePos = _mouseDownPos;
			}
		}

		private void MouseUp()
		{
			if (_banner.IsVisible)
			{
				if (!_isDragging && Mathf.Abs(_mouseDownPos.magnitude - Input.mousePosition.magnitude) < Threshhold)
				{
					_banner.Hide();
				}
				_isClicking = false;
				_isDragging = false;
				_mouseDownPos = Vector3.zero;
				_prevMousePos = _mouseDownPos;
			}
		}
	}
}

using System.Collections;
using UnityEngine;

namespace Rilisoft
{
	[RequireComponent(typeof(UIWidget))]
	public class DirectionPointer : MonoBehaviour
	{
		[SerializeField]
		private DirectionViewTargetType _forPointerType;

		[Range(0f, 3f)]
		[SerializeField]
		private float _hideTime = 0.3f;

		public bool OutOfRange;

		private UIWidget _widgetVal;

		public DirectionViewTargetType ForPointerType
		{
			get
			{
				return _forPointerType;
			}
		}

		public DirectionViewerTarget Target { get; private set; }

		public bool IsInited
		{
			get
			{
				return Target != null;
			}
		}

		private UIWidget _widget
		{
			get
			{
				if (_widgetVal == null)
				{
					_widgetVal = GetComponent<UIWidget>();
				}
				return _widgetVal;
			}
		}

		public void TurnOn(DirectionViewerTarget pointer)
		{
			Target = pointer;
			base.gameObject.SetActive(true);
			_widget.alpha = 1f;
			_widget.gameObject.transform.localScale = Vector3.one;
		}

		public void Hide()
		{
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(TurnOffCoroutine());
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}

		public void TurnOff()
		{
			Target = null;
			OutOfRange = false;
			Hide();
		}

		private IEnumerator TurnOffCoroutine()
		{
			float elapsed = 0f;
			while (elapsed <= _hideTime && Target == null)
			{
				elapsed += Time.deltaTime;
				_widget.alpha = Mathf.Lerp(1f, 0.1f, elapsed / _hideTime);
				float scalMltp = Mathf.Lerp(1f, 2f, elapsed / _hideTime);
				_widget.gameObject.transform.localScale = Vector3.one * scalMltp;
				yield return null;
			}
			base.gameObject.SetActive(false);
		}
	}
}

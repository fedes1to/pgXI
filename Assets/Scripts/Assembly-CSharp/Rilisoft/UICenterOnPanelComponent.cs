using UnityEngine;
using UnityEngine.Events;

namespace Rilisoft
{
	public class UICenterOnPanelComponent : MonoBehaviour
	{
		[SerializeField]
		[ReadOnly]
		protected UIPanel _panel;

		[SerializeField]
		public Direction Direction = Direction.Horizontal;

		[SerializeField]
		public float Slack = 0.1f;

		[SerializeField]
		public UnityEvent OnCentered;

		[SerializeField]
		public UnityEvent OnCenteredLoss;

		[ReadOnly]
		[SerializeField]
		protected bool _centered;

		protected Vector2 Offset;

		public Vector3 Center
		{
			get
			{
				Vector3[] worldCorners = _panel.worldCorners;
				return (worldCorners[2] + worldCorners[0]) * 0.5f;
			}
		}

		public CenterDirection CenterDirection
		{
			get
			{
				return (!(Center.x - base.transform.position.x > 0f)) ? CenterDirection.OnRight : CenterDirection.OnLeft;
			}
		}

		private void Awake()
		{
			_panel = NGUITools.FindInParents<UIPanel>(base.gameObject);
			OnCentered = OnCentered ?? new UnityEvent();
			OnCenteredLoss = OnCenteredLoss ?? new UnityEvent();
		}

		private void OnEnable()
		{
			_centered = false;
		}

		protected virtual void Update()
		{
			if (_panel == null)
			{
				_panel = NGUITools.FindInParents<UIPanel>(base.gameObject);
			}
			if (_panel == null)
			{
				return;
			}
			Offset = new Vector2(Mathf.Abs(Center.x - base.transform.position.x), Mathf.Abs(Center.y - base.transform.position.y));
			float num = ((Direction != Direction.Horizontal) ? Offset.y : Offset.x);
			if (num <= Slack)
			{
				if (!_centered)
				{
					_centered = true;
					if (OnCentered != null)
					{
						OnCentered.Invoke();
					}
				}
			}
			else if (_centered)
			{
				_centered = false;
				if (OnCenteredLoss != null)
				{
					OnCenteredLoss.Invoke();
				}
			}
		}
	}
}

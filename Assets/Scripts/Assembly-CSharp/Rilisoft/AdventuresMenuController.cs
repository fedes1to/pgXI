using UnityEngine;

namespace Rilisoft
{
	internal sealed class AdventuresMenuController : MonoBehaviour
	{
		[SerializeField]
		private UIButton sandboxButton;

		[SerializeField]
		private float period = 334f;

		private float _distance;

		private void Awake()
		{
		}

		private void OnEnable()
		{
			Refresh();
		}

		private void Refresh()
		{
			sandboxButton.gameObject.SetActive(IsSandboxEnabled());
			Transform parent = sandboxButton.transform.parent;
			float x = ((!IsSandboxEnabled()) ? (0.5f * period) : 0f);
			parent.localPosition = new Vector3(x, parent.localPosition.y, parent.localPosition.z);
		}

		private bool IsSandboxEnabled()
		{
			return true;
		}
	}
}

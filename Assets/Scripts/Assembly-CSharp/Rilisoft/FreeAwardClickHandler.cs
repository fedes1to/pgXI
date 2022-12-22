using UnityEngine;

namespace Rilisoft
{
	public class FreeAwardClickHandler : MonoBehaviour
	{
		private void OnClick()
		{
			if (FreeAwardShowHandler.Instance != null)
			{
				FreeAwardShowHandler.Instance.OnClick();
			}
		}
	}
}

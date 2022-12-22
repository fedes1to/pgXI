using UnityEngine;

namespace Rilisoft
{
	public class InAppBonusLobbyClickHandler : MonoBehaviour
	{
		private void OnClick()
		{
			if (InAppBonusLobbyController.Instance != null)
			{
				InAppBonusLobbyController.Instance.Click();
			}
		}
	}
}

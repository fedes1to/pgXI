using UnityEngine;

namespace Rilisoft
{
	public class LeprechauntClickHandler : MonoBehaviour
	{
		private void OnClick()
		{
			if (Singleton<LeprechauntManager>.Instance != null)
			{
				LeprechauntLobbyView.Instance.Tap();
			}
		}
	}
}

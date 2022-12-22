using UnityEngine;

internal sealed class ShowHideRanksPanel : MonoBehaviour
{
	private void OnEnable()
	{
		if (ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.isMenu = false;
			ExperienceController.sharedController.isConnectScene = false;
			ExperienceController.sharedController.isShowRanks = false;
		}
		ActivityIndicator.IsActiveIndicator = true;
	}

	private void OnDisable()
	{
		if (ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.posRanks = new Vector2(21f * Defs.Coef, 21f * Defs.Coef);
			ExperienceController.sharedController.isMenu = true;
			ExperienceController.sharedController.isConnectScene = true;
			ExperienceController.sharedController.isShowRanks = true;
		}
		ActivityIndicator.IsActiveIndicator = false;
	}
}

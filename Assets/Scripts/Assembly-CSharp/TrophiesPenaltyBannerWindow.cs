using UnityEngine;

public class TrophiesPenaltyBannerWindow : BannerWindow
{
	public const string KEY_LEAVE_FROM_DUEL = "leave_from_duel_penalty";

	[SerializeField]
	private UILabel _penalty;

	public override void Show()
	{
		base.Show();
		int @int = PlayerPrefs.GetInt("leave_from_duel_penalty");
		_penalty.text = @int.ToString();
	}

	public void HideButtonAction()
	{
		PlayerPrefs.SetInt("leave_from_duel_penalty", 0);
		if (BannerWindowController.SharedController != null)
		{
			BannerWindowController.SharedController.ResetStateBannerShowed(BannerWindowType.TrophiesPenalty);
			BannerWindowController.SharedController.HideBannerWindow();
		}
	}
}

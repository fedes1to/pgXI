using Rilisoft;

public class TournamentAvailableBannerWindow : BannerWindow
{
	private static readonly PrefsBoolCachedProperty _canShow = new PrefsBoolCachedProperty("TournamentAwailableBannerWindow_needShow");

	public static bool CanShow
	{
		get
		{
			return _canShow.Value;
		}
		set
		{
			_canShow.Value = value;
		}
	}

	public void HideButtonAction()
	{
		CanShow = false;
		if (BannerWindowController.SharedController != null)
		{
			BannerWindowController.SharedController.ResetStateBannerShowed(BannerWindowType.TournamentAvailable);
			BannerWindowController.SharedController.HideBannerWindow();
		}
		if (MainMenuController.sharedController != null)
		{
			MainMenuController.sharedController.ShowLeaderboards(LeaderboardsView.State.Tournament);
		}
	}
}

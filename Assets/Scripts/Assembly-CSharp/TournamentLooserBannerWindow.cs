using Rilisoft;

public class TournamentLooserBannerWindow : BannerWindow
{
	private static readonly PrefsBoolCachedProperty _canShow = new PrefsBoolCachedProperty("TournamentLooserBannerWindow_needShow");

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
			BannerWindowController.SharedController.ResetStateBannerShowed(BannerWindowType.TournamentLooser);
			BannerWindowController.SharedController.HideBannerWindow();
		}
	}
}

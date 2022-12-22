public class BtnBannerPereliv : ButtonBannerBase
{
	public override bool BannerIsActive()
	{
		return MainMenuController.trafficForwardActive;
	}

	public override void OnClickButton()
	{
		MainMenuController.sharedController.HandleTrafficForwardingClicked();
	}

	public override void OnHide()
	{
	}

	public override void OnShow()
	{
	}

	public override void OnChangeLocalize()
	{
	}
}

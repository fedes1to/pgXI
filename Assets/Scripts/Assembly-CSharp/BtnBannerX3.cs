public class BtnBannerX3 : ButtonBannerBase
{
	public override bool BannerIsActive()
	{
		return PromoActionsManager.sharedManager.IsEventX3Active;
	}

	public override void OnClickButton()
	{
		MainMenuController.sharedController.ShowBankWindow();
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

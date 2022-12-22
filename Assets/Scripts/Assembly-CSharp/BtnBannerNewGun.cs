using UnityEngine;

public class BtnBannerNewGun : ButtonBannerBase
{
	public string tagForClick = string.Empty;

	public UILabel lbSale;

	public UILabel lbPrice;

	public UITexture txGun;

	public UISprite sprCoinImg;

	public override bool BannerIsActive()
	{
		if (PromoActionsManager.sharedManager != null)
		{
			if (PromoActionsManager.sharedManager.news.Count > 0)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public override void OnClickButton()
	{
		if (!string.IsNullOrEmpty(tagForClick))
		{
			MainMenuController.sharedController.HandlePromoActionClicked(tagForClick);
		}
	}

	public override void OnHide()
	{
	}

	public override void OnShow()
	{
		if (string.IsNullOrEmpty(tagForClick))
		{
			OnUpdateParameter();
		}
	}

	public override void OnUpdateParameter()
	{
		if (PromoActionsManager.sharedManager != null && PromoActionsManager.sharedManager.news.Count > 0)
		{
			int index = Random.Range(0, PromoActionsManager.sharedManager.news.Count);
			tagForClick = PromoActionsManager.sharedManager.news[index];
		}
		if (!string.IsNullOrEmpty(tagForClick))
		{
			string empty = string.Empty;
			int itemCategory = ItemDb.GetItemCategory(tagForClick);
			Texture mainTexture = Resources.Load<Texture>(empty);
			txGun.mainTexture = mainTexture;
			ItemPrice priceByShopId = ItemDb.GetPriceByShopId(tagForClick, (ShopNGUIController.CategoryNames)(-1));
			lbSale.text = string.Format("{0}\n{1}%", LocalizationStore.Key_0419, PromoActionsManager.sharedManager.discounts[tagForClick][0].Value);
			lbPrice.text = PromoActionsManager.sharedManager.discounts[tagForClick][1].Value.ToString();
			lbPrice.color = ((!priceByShopId.Currency.Equals("Coins")) ? new Color(0.3176f, 0.8117f, 1f) : new Color(1f, 0.8627f, 0f));
			sprCoinImg.spriteName = ((!priceByShopId.Currency.Equals("Coins")) ? "gem_znachek" : "ingame_coin");
		}
	}

	public override void OnChangeLocalize()
	{
	}
}

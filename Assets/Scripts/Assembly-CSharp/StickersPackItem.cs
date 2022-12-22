using UnityEngine;

public class StickersPackItem : MonoBehaviour
{
	public TypePackSticker typePack;

	public UILabel priceLabel;

	public GameObject btnForBuyPack;

	public GameObject btnAvaliablePack;

	private BuySmileBannerController buyPackController;

	public string KeyForBuy
	{
		get
		{
			return StickersController.KeyForBuyPack(typePack);
		}
	}

	private void Start()
	{
		if ((bool)priceLabel)
		{
			priceLabel.text = StickersController.GetPricePack(typePack).Price.ToString();
		}
		buyPackController = GetComponentInParent<BuySmileBannerController>();
	}

	private void OnEnable()
	{
		CheckStateBtn();
	}

	public void CheckStateBtn()
	{
		if (StickersController.IsBuyPack(typePack))
		{
			if ((bool)btnForBuyPack)
			{
				btnForBuyPack.SetActive(false);
			}
			if ((bool)btnAvaliablePack)
			{
				btnAvaliablePack.SetActive(true);
			}
		}
		else
		{
			if ((bool)btnForBuyPack)
			{
				btnForBuyPack.SetActive(true);
			}
			if ((bool)btnAvaliablePack)
			{
				btnAvaliablePack.SetActive(false);
			}
		}
	}

	public void TryBuyPack()
	{
		if (buyPackController != null)
		{
			ButtonClickSound.Instance.PlayClick();
			buyPackController.BuyStickersPack(this);
		}
	}

	public void OnBuy()
	{
		CheckStateBtn();
		StickersController.EventPackBuy();
	}
}

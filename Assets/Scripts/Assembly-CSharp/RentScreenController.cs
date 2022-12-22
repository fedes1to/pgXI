using System;
using UnityEngine;

public sealed class RentScreenController : PropertyInfoScreenController
{
	public GameObject viewButtonPanel;

	public GameObject rentButtonsPanel;

	public UIButton viewButton;

	public GameObject window;

	public UILabel[] header;

	public UILabel[] rentFor;

	public UILabel[] prices;

	public UILabel[] pricesCoins;

	public UIButton[] buttons;

	public UITexture itemImage;

	public Action<string> onPurchaseCustomAction;

	public Action onEnterCoinsShopAdditionalAction;

	public Action onExitCoinsShopAdditionalAction;

	public Action<string> customEquipWearAction;

	private string _itemTag;

	private ShopNGUIController.CategoryNames category;

	private Func<int, int> priceFormula
	{
		get
		{
			return delegate(int ind)
			{
				int result = 10;
				if (_itemTag != null)
				{
					ItemRecord byTag = ItemDb.GetByTag(_itemTag);
					ItemPrice priceByShopId = ItemDb.GetPriceByShopId((byTag == null || byTag.ShopId == null) ? _itemTag : byTag.ShopId, (ShopNGUIController.CategoryNames)(-1));
					if (priceByShopId != null)
					{
						int num = -1;
						if (num == -1)
						{
							result = Mathf.RoundToInt((float)priceByShopId.Price * TempItemsController.PriceCoefs[_itemTag][ind]);
						}
					}
				}
				return result;
			};
		}
	}

	public string Header
	{
		set
		{
			UILabel[] array = header;
			foreach (UILabel uILabel in array)
			{
				if (uILabel != null && value != null)
				{
					uILabel.text = value;
				}
			}
		}
	}

	public string RentFor
	{
		set
		{
			UILabel[] array = rentFor;
			foreach (UILabel uILabel in array)
			{
				if (uILabel != null && value != null && _itemTag != null)
				{
					uILabel.text = string.Format(value, ItemDb.GetItemNameByTag(_itemTag));
				}
			}
		}
	}

	public string ItemTag
	{
		set
		{
			_itemTag = value;
			if (_itemTag == null)
			{
				return;
			}
			int itemCategory = ItemDb.GetItemCategory(_itemTag);
			category = (ShopNGUIController.CategoryNames)itemCategory;
			ItemRecord byTag = ItemDb.GetByTag(_itemTag);
			ItemPrice priceByShopId = ItemDb.GetPriceByShopId((byTag == null || byTag.ShopId == null) ? _itemTag : byTag.ShopId, (ShopNGUIController.CategoryNames)(-1));
			bool flag = priceByShopId != null && priceByShopId.Currency != null && priceByShopId.Currency.Equals("Coins");
			UILabel[] array = ((!flag) ? prices : pricesCoins);
			foreach (UILabel uILabel in array)
			{
				if (uILabel != null)
				{
					uILabel.gameObject.SetActive(true);
					uILabel.text = priceFormula(Array.IndexOf((!flag) ? prices : pricesCoins, uILabel)).ToString();
				}
			}
			UILabel[] array2 = ((!flag) ? pricesCoins : prices);
			foreach (UILabel uILabel2 in array2)
			{
				if (uILabel2 != null)
				{
					uILabel2.gameObject.SetActive(false);
				}
			}
		}
	}

	public override void Hide()
	{
		base.transform.parent = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void HandleRentButton(UIButton b)
	{
		if (Defs.isSoundFX)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		int arg = Array.IndexOf(buttons, b);
		ItemRecord byTag = ItemDb.GetByTag(_itemTag);
		ItemPrice priceByShopId = ItemDb.GetPriceByShopId((byTag == null || byTag.ShopId == null) ? _itemTag : byTag.ShopId, (ShopNGUIController.CategoryNames)(-1));
		ItemPrice price = new ItemPrice(priceFormula(arg), (priceByShopId == null) ? "GemsCurrency" : priceByShopId.Currency);
		bool flag = TempItemsController.sharedController != null && TempItemsController.sharedController.ContainsItem(_itemTag);
		ShopNGUIController.TryToBuy(window, price, delegate
		{
			bool flag2 = !Wear.armorNumTemp.ContainsKey(_itemTag ?? string.Empty) && _itemTag != null && (_itemTag.Equals(WeaponTags.DragonGunRent_Tag) || _itemTag.Equals(WeaponTags.PumpkinGunRent_Tag) || _itemTag.Equals(WeaponTags.RayMinigunRent_Tag) || _itemTag.Equals(WeaponTags.Red_StoneRent_Tag) || _itemTag.Equals(WeaponTags.TwoBoltersRent_Tag));
			Action<string> action3 = onPurchaseCustomAction;
			if (action3 != null)
			{
				action3(_itemTag);
			}
			if (TempItemsController.sharedController != null)
			{
				TempItemsController.sharedController.ExpiredItems.Remove(_itemTag);
			}
			Hide();
		}, null, null, null, delegate
		{
			Action action2 = onEnterCoinsShopAdditionalAction;
			if (action2 != null)
			{
				action2();
			}
		}, delegate
		{
			Action action = onExitCoinsShopAdditionalAction;
			if (action != null)
			{
				action();
			}
		});
	}

	public void HandleViewButton()
	{
		Hide();
		if (_itemTag == null || !TempItemsController.GunsMappingFromTempToConst.ContainsKey(_itemTag))
		{
			return;
		}
		string text = WeaponManager.FirstUnboughtOrForOurTier(TempItemsController.GunsMappingFromTempToConst[_itemTag]);
		if (text != null)
		{
			int itemCategory = ItemDb.GetItemCategory(text);
			if (itemCategory == -1)
			{
			}
		}
	}

	private void Awake()
	{
		rentButtonsPanel.SetActive(false);
		viewButtonPanel.SetActive(true);
	}

	public static void SetDepthForExpGUI(int newDepth)
	{
		ExpController instance = ExpController.Instance;
		if (instance != null)
		{
			instance.experienceView.experienceCamera.depth = newDepth;
		}
	}

	private void Start()
	{
		SetDepthForExpGUI(89);
	}

	private void OnDestroy()
	{
		SetDepthForExpGUI(99);
	}
}

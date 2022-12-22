using System;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public class TryGunScreenController : MonoBehaviour
{
	public GameObject buyPanel;

	public GameObject equipPanel;

	public GameObject backButton;

	public GameObject gemsPrice;

	public GameObject gemsPriceOld;

	public GameObject coinsPrice;

	public GameObject coinsPriceOld;

	public UITexture itemImage;

	public List<UILabel> itemNameLabels;

	public GameObject headSpecialOffer;

	public GameObject headExpired;

	public List<UILabel> numberOfMatchesLabels;

	public List<UILabel> discountLabels;

	public Action<string> onPurchaseCustomAction;

	public Action onEnterCoinsShopAdditionalAction;

	public Action onExitCoinsShopAdditionalAction;

	public Action<string> customEquipWearAction;

	private string _itemTag;

	private ShopNGUIController.CategoryNames category;

	private ItemPrice price;

	private ItemPrice priceWithoutPromo;

	private bool _expiredTryGun;

	private IDisposable _escapeSubscription;

	public bool ExpiredTryGun
	{
		get
		{
			return _expiredTryGun;
		}
		set
		{
			try
			{
				_expiredTryGun = value;
				backButton.SetActive(value);
				buyPanel.SetActive(value);
				equipPanel.SetActive(!value);
				gemsPrice.SetActive(value && price.Currency == "GemsCurrency");
				gemsPriceOld.SetActive(value && price.Currency == "GemsCurrency");
				coinsPrice.SetActive(value && price.Currency == "Coins");
				coinsPriceOld.SetActive(value && price.Currency == "Coins");
				headSpecialOffer.SetActive(!value);
				headExpired.SetActive(value);
				if (value)
				{
					if (price.Currency == "GemsCurrency")
					{
						gemsPrice.GetComponent<UILabel>().text = price.Price.ToString();
						gemsPriceOld.GetComponent<UILabel>().text = priceWithoutPromo.Price.ToString();
					}
					if (price.Currency == "Coins")
					{
						coinsPrice.GetComponent<UILabel>().text = price.Price.ToString();
						coinsPriceOld.GetComponent<UILabel>().text = priceWithoutPromo.Price.ToString();
					}
					try
					{
						foreach (UILabel discountLabel in discountLabels)
						{
							bool onlyServerDiscount;
							discountLabel.text = string.Format(LocalizationStore.Get("Key_1996"), Mathf.RoundToInt(WeaponManager.TryGunPromoDuration() / 60f), ShopNGUIController.DiscountFor(ItemTag, out onlyServerDiscount));
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("Exception in setting up discount in try gun screen: " + ex);
					}
					AnalyticsStuff.LogWEaponsSpecialOffers_Conversion(true);
					return;
				}
				int num = ((!ABTestController.useBuffSystem) ? KillRateCheck.instance.GetRoundsForGun() : BuffSystem.instance.GetRoundsForGun());
				foreach (UILabel numberOfMatchesLabel in numberOfMatchesLabels)
				{
					numberOfMatchesLabel.text = string.Format(LocalizationStore.Get("Key_1995"), num);
				}
			}
			catch (Exception ex2)
			{
				Debug.LogError("Exception in ExpiredTryGun: " + ex2);
			}
		}
	}

	public string ItemTag
	{
		get
		{
			return _itemTag;
		}
		set
		{
			try
			{
				_itemTag = value;
				category = (ShopNGUIController.CategoryNames)ItemDb.GetItemCategory(_itemTag);
				price = ShopNGUIController.GetItemPrice(_itemTag, category);
				priceWithoutPromo = ShopNGUIController.GetItemPrice(_itemTag, category, false, false);
				Texture itemIcon = ItemDb.GetItemIcon(_itemTag, category);
				if (itemIcon != null && itemImage != null)
				{
					itemImage.mainTexture = itemIcon;
				}
				string itemNameByTag = ItemDb.GetItemNameByTag(_itemTag);
				foreach (UILabel itemNameLabel in itemNameLabels)
				{
					itemNameLabel.text = itemNameByTag;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in ItemTag: " + ex);
			}
		}
	}

	public void HandleEquip()
	{
		try
		{
			WeaponManager.sharedManager.AddTryGun(ItemTag);
			if (ABTestController.useBuffSystem)
			{
				BuffSystem.instance.SetGetTryGun(ItemDb.GetByTag(ItemTag).PrefabName);
			}
			else
			{
				KillRateCheck.instance.SetGetWeapon();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("TryGunScreenController HandleEquip exception: " + ex);
		}
	}

	public void HandleClose()
	{
		DestroyScreen();
	}

	private void DestroyScreen()
	{
		base.transform.parent = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void HandleBuy()
	{
		int priceAmount = price.Price;
		string priceCurrency = price.Currency;
		ShopNGUIController.TryToBuy(base.gameObject, price, delegate
		{
			if (Defs.isSoundFX)
			{
			}
			ShopNGUIController.FireWeaponOrArmorBought();
			ShopNGUIController.ProvideItem(category, ItemTag, 1, false, 0, delegate(string item)
			{
				if (ShopNGUIController.sharedShop != null)
				{
					ShopNGUIController.sharedShop.FireBuyAction(item);
				}
			}, null, true, true, false);
			try
			{
				string empty = string.Empty;
				string itemNameNonLocalized = ItemDb.GetItemNameNonLocalized(WeaponManager.LastBoughtTag(ItemTag) ?? WeaponManager.FirstUnboughtTag(ItemTag), empty, category);
				bool isDaterWeapon = false;
				if (ShopNGUIController.IsWeaponCategory(category))
				{
					WeaponSounds weaponInfo = ItemDb.GetWeaponInfo(ItemTag);
					isDaterWeapon = weaponInfo != null && weaponInfo.IsAvalibleFromFilter(3);
				}
				string categoryParameterName = AnalyticsConstants.GetSalesName(category) ?? category.ToString();
				AnalyticsStuff.LogSales(itemNameNonLocalized, categoryParameterName, isDaterWeapon);
				AnalyticsFacade.InAppPurchase(itemNameNonLocalized, AnalyticsStuff.AnalyticsReadableCategoryNameFromOldCategoryName(categoryParameterName), 1, priceAmount, priceCurrency);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in loggin in Try Gun Screen Controller: " + ex);
			}
			DestroyScreen();
		}, null, null, null, delegate
		{
		}, delegate
		{
		});
	}

	private void Start()
	{
		_escapeSubscription = BackSystem.Instance.Register(delegate
		{
			if (!ExpiredTryGun)
			{
				HandleEquip();
			}
			HandleClose();
		}, "Try Gun Screen");
	}

	private void OnDestroy()
	{
		_escapeSubscription.Dispose();
	}
}

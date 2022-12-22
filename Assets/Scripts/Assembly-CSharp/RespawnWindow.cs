using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public sealed class RespawnWindow : MonoBehaviour
{
	public UILabel killerLevelNicknameLabel;

	public UITexture killerRank;

	public UILabel killerClanNameLabel;

	public UITexture killerClanLogo;

	public UILabel autoRespawnTitleLabel;

	public UILabel autoRespawnTimerLabel;

	public GameObject characterViewHolder;

	public Camera characterViewCamera;

	public UITexture characterViewTexture;

	public CharacterView characterView;

	public RespawnWindowItemToBuy killerWeapon;

	public RespawnWindowItemToBuy recommendedWeapon;

	public RespawnWindowItemToBuy recommendedArmor;

	public GameObject coinsShopButton;

	public GameObject armorObj;

	public GameObject healthIcon;

	public GameObject mechIcon;

	public GameObject healthBackground;

	public GameObject healtharmorBackground;

	public UITable healthTable;

	public RespawnWindowEquipmentItem hatItem;

	public RespawnWindowEquipmentItem maskItem;

	public RespawnWindowEquipmentItem armorItem;

	public RespawnWindowEquipmentItem capeItem;

	public RespawnWindowEquipmentItem bootsItem;

	public RespawnWindowEquipmentItem petItem;

	public RespawnWindowEquipmentItem gadgetSupportItem;

	public RespawnWindowEquipmentItem gadgetTrowingItem;

	public RespawnWindowEquipmentItem gadgetToolsItem;

	public UILabel armorCountLabel;

	public UILabel healthCountLabel;

	public GameObject characterDrag;

	public GameObject cameraDrag;

	private static RespawnWindow _instance;

	private float _originalTimeout;

	private float _remained;

	[NonSerialized]
	public RenderTexture respawnWindowRT;

	public static RespawnWindow Instance
	{
		get
		{
			return _instance;
		}
	}

	public bool isShown
	{
		get
		{
			return base.gameObject.activeSelf;
		}
	}

	private void Start()
	{
		if (coinsShopButton != null)
		{
			ButtonHandler component = coinsShopButton.GetComponent<ButtonHandler>();
			if (component != null)
			{
				component.Clicked += OnToBankClicked;
			}
		}
	}

	public void Show(KillerInfo inKillerInfo)
	{
		KillerInfo killerInfo = new KillerInfo();
		inKillerInfo.CopyTo(killerInfo);
		killerLevelNicknameLabel.text = killerInfo.nickname;
		killerRank.mainTexture = killerInfo.rankTex;
		killerClanLogo.mainTexture = killerInfo.clanLogoTex;
		killerClanNameLabel.text = killerInfo.clanName;
		FillItemsToBuy(killerInfo);
		FillEquipments(killerInfo);
		FillStats(killerInfo);
		Defs.inRespawnWindow = true;
		base.gameObject.SetActive(true);
		_instance = this;
		characterViewHolder.SetActive(true);
		SetKillerNameVisible(false);
		_originalTimeout = 15f;
	}

	public void ShowCharacter(KillerInfo killerInfo)
	{
		characterView.ShowCharacterType(CharacterView.CharacterType.Player);
		characterView.characterInterface.usePetFromStorager = false;
		characterView.SetWeaponAndSkin(killerInfo.weapon, killerInfo.skinTex, true);
		if (!string.IsNullOrEmpty(killerInfo.hat))
		{
			characterView.UpdateHat(killerInfo.hat);
		}
		else
		{
			characterView.RemoveHat();
		}
		if (!string.IsNullOrEmpty(killerInfo.cape))
		{
			characterView.UpdateCape(killerInfo.cape, killerInfo.capeTex);
		}
		else
		{
			characterView.RemoveCape();
		}
		if (!string.IsNullOrEmpty(killerInfo.mask))
		{
			characterView.UpdateMask(killerInfo.mask);
		}
		else
		{
			characterView.RemoveMask();
		}
		if (!string.IsNullOrEmpty(killerInfo.boots))
		{
			characterView.UpdateBoots(killerInfo.boots);
		}
		else
		{
			characterView.RemoveBoots();
		}
		if (!string.IsNullOrEmpty(killerInfo.armor))
		{
			characterView.UpdateArmor(killerInfo.armor);
		}
		else
		{
			characterView.RemoveArmor();
		}
		characterViewHolder.SetActive(true);
		characterViewCamera.gameObject.SetActive(true);
		characterView.gameObject.SetActive(true);
		SetKillerNameVisible(true);
	}

	public void CloseRespawnWindow()
	{
		RespawnPlayer();
		Hide();
	}

	public void SetCurrentTime(float sec)
	{
		autoRespawnTimerLabel.text = Mathf.CeilToInt(Mathf.Max(0f, sec)).ToString();
	}

	private void RespawnPlayer()
	{
		Player_move_c myPlayerMoveC = WeaponManager.sharedManager.myPlayerMoveC;
		if (myPlayerMoveC != null)
		{
			myPlayerMoveC.RespawnPlayer();
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		characterViewHolder.SetActive(false);
		characterView.gameObject.SetActive(false);
		Reset();
		Defs.inRespawnWindow = false;
		_instance = null;
	}

	public void OnBtnGoBattleClick()
	{
		RespawnPlayer();
		Hide();
	}

	private void FillEquipments(KillerInfo killerInfo)
	{
		hatItem.SetItemTag(killerInfo.hat, 6);
		maskItem.SetItemTag(killerInfo.mask, 12);
		armorItem.SetItemTag(killerInfo.armor, 7);
		capeItem.SetItemTag(killerInfo.cape, 9);
		bootsItem.SetItemTag(killerInfo.boots, 10);
		petItem.SetItemTag(killerInfo.pet, 25000);
		gadgetSupportItem.SetItemTag(killerInfo.gadgetSupport, 13500);
		gadgetTrowingItem.SetItemTag(killerInfo.gadgetTrowing, 12500);
		gadgetToolsItem.SetItemTag(killerInfo.gadgetTools, 13000);
	}

	private void FillItemsToBuy(KillerInfo killerInfo)
	{
		try
		{
			List<string> list = ((BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64) ? GetWeaponsForBuy() : new List<string>());
			string itemTag = ((list.Count <= 0) ? null : list[0]);
			string text = ((BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64) ? GetArmorForBuy() : null);
			if (string.IsNullOrEmpty(text))
			{
				text = ((list.Count <= 1) ? null : list[1]);
			}
			if (killerInfo != null && killerInfo.weapon != null)
			{
				string weapon = killerInfo.weapon;
				int? upgradeNum = null;
				if (GearManager.IsItemGear(weapon))
				{
					if (weapon == GearManager.Turret)
					{
						upgradeNum = 1 + killerInfo.turretUpgrade;
					}
					else if (weapon == GearManager.Mech)
					{
						upgradeNum = 1 + killerInfo.mechUpgrade;
					}
				}
				killerWeapon.SetWeaponTag(weapon, upgradeNum);
			}
			else
			{
				killerWeapon.SetWeaponTag(string.Empty, 0);
			}
			recommendedWeapon.SetWeaponTag(itemTag);
			recommendedArmor.SetWeaponTag(text);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		finally
		{
		}
	}

	public void OnItemToBuyClick(RespawnWindowItemToBuy itemToBuy)
	{
		if (itemToBuy.itemTag == null || itemToBuy.itemPrice == null)
		{
			return;
		}
		int priceAmount = itemToBuy.itemPrice.Price;
		string priceCurrency = itemToBuy.itemPrice.Currency;
		ShopNGUIController.TryToBuy(base.gameObject, itemToBuy.itemPrice, delegate
		{
			if (Defs.isSoundFX)
			{
				UIPlaySound component = itemToBuy.btnBuy.GetComponent<UIPlaySound>();
				if (component != null)
				{
					component.Play();
				}
			}
			ShopNGUIController.CategoryNames itemCategory = (ShopNGUIController.CategoryNames)itemToBuy.itemCategory;
			if (itemCategory == ShopNGUIController.CategoryNames.ArmorCategory || ShopNGUIController.IsWeaponCategory(itemCategory))
			{
				ShopNGUIController.FireWeaponOrArmorBought();
			}
			int num = 1;
			if (GearManager.IsItemGear(itemToBuy.itemTag))
			{
				num = GearManager.ItemsInPackForGear(itemToBuy.itemTag);
				if (itemToBuy.itemTag == GearManager.Grenade)
				{
					int b = Defs2.MaxGrenadeCount - Storager.getInt(itemToBuy.itemTag, false);
					num = Mathf.Min(num, b);
				}
			}
			ShopNGUIController.ProvideItem(itemCategory, (!ShopNGUIController.IsWeaponCategory(itemCategory)) ? itemToBuy.itemTag : WeaponManager.FirstUnboughtOrForOurTier(itemToBuy.itemTag), num, false, 0, delegate(string item)
			{
				if (ShopNGUIController.sharedShop != null)
				{
					ShopNGUIController.sharedShop.FireBuyAction(item);
				}
			}, null, true, true, false);
			killerWeapon.SetWeaponTag(killerWeapon.itemTag);
			recommendedWeapon.SetWeaponTag(recommendedWeapon.itemTag);
			recommendedArmor.SetWeaponTag(recommendedArmor.itemTag);
			try
			{
				string empty = string.Empty;
				string itemNameNonLocalized = ItemDb.GetItemNameNonLocalized(WeaponManager.LastBoughtTag(itemToBuy.itemTag) ?? WeaponManager.FirstUnboughtTag(itemToBuy.itemTag), empty, itemCategory);
				bool isDaterWeapon = false;
				if (ShopNGUIController.IsWeaponCategory(itemCategory))
				{
					WeaponSounds weaponInfo = ItemDb.GetWeaponInfo(itemToBuy.itemTag);
					isDaterWeapon = weaponInfo != null && weaponInfo.IsAvalibleFromFilter(3);
				}
				string categoryParameterName = AnalyticsConstants.GetSalesName(itemCategory) ?? itemCategory.ToString();
				AnalyticsStuff.LogSales(itemNameNonLocalized, categoryParameterName, isDaterWeapon);
				AnalyticsFacade.InAppPurchase(itemNameNonLocalized, AnalyticsStuff.AnalyticsReadableCategoryNameFromOldCategoryName(categoryParameterName), 1, priceAmount, priceCurrency);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in loggin in Respawn Window: " + ex);
			}
		}, null, null, null, delegate
		{
			SetPaused(true);
		}, delegate
		{
			SetPaused(false);
		});
	}

	private void FillStats(KillerInfo killerInfo)
	{
		int armorCountFor = Wear.GetArmorCountFor(killerInfo.armor, killerInfo.hat);
		if (armorCountFor > 0)
		{
			armorObj.SetActive(true);
			healthBackground.SetActive(false);
			healtharmorBackground.SetActive(true);
			armorCountLabel.text = string.Format("{0}/{1}", Mathf.Min(armorCountFor, killerInfo.armorValue), armorCountFor);
			healthTable.repositionNow = true;
		}
		else
		{
			armorObj.SetActive(false);
			healthBackground.SetActive(true);
			healtharmorBackground.SetActive(false);
			healthTable.repositionNow = true;
		}
		healthIcon.SetActive(true);
		healthCountLabel.text = string.Format("{0}/{1}", killerInfo.healthValue, ExperienceController.HealthByLevel[killerInfo.rank]);
	}

	private void OnBackFromBankClicked(object sender, EventArgs e)
	{
		BankController.Instance.BackRequested -= OnBackFromBankClicked;
		BankController.Instance.InterfaceEnabled = false;
		if (this != null)
		{
			base.gameObject.SetActive(true);
		}
		SetPaused(false);
	}

	private void OnToBankClicked(object sender, EventArgs e)
	{
		ShowBankWindow();
	}

	private void ShowBankWindow()
	{
		ButtonClickSound.Instance.PlayClick();
		BankController.Instance.BackRequested += OnBackFromBankClicked;
		BankController.Instance.InterfaceEnabled = true;
		SetPaused(true);
		if (this != null)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void SetPaused(bool paused)
	{
	}

	private List<string> GetWeaponsForBuy()
	{
		List<string> weaponsForBuy = WeaponManager.GetWeaponsForBuy();
		SortWeaponsByDps(weaponsForBuy);
		return weaponsForBuy;
	}

	private string GetArmorForBuy()
	{
		List<string> list = new List<string>();
		list.AddRange(Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0]);
		bool filterNextTierUpgrades = true;
		List<string> list2 = PromoActionsGUIController.FilterPurchases(list, filterNextTierUpgrades);
		foreach (string item in list)
		{
			if (TempItemsController.PriceCoefs.ContainsKey(item) && !list2.Contains(item))
			{
				list2.Add(item);
			}
		}
		foreach (string item2 in list2)
		{
			list.Remove(item2);
		}
		return list.FirstOrDefault();
	}

	private void SortWeaponsByDps(List<string> weaponTags)
	{
		weaponTags.Sort(delegate(string weaponTag1, string weaponTag2)
		{
			WeaponSounds weaponInfo = ItemDb.GetWeaponInfo(weaponTag1);
			if (weaponInfo == null)
			{
				return 1;
			}
			WeaponSounds weaponInfo2 = ItemDb.GetWeaponInfo(weaponTag2);
			return (weaponInfo2 == null) ? (-1) : weaponInfo2.DPS.CompareTo(weaponInfo.DPS);
		});
	}

	private void SetKillerNameVisible(bool visible)
	{
		killerLevelNicknameLabel.gameObject.SetActive(visible);
		killerRank.gameObject.SetActive(visible);
		killerClanNameLabel.gameObject.SetActive(visible);
		killerClanLogo.gameObject.SetActive(visible);
	}

	private void Reset()
	{
		killerWeapon.Reset();
		recommendedWeapon.Reset();
		recommendedArmor.Reset();
		hatItem.ResetImage();
		maskItem.ResetImage();
		armorItem.ResetImage();
		capeItem.ResetImage();
		bootsItem.ResetImage();
		petItem.ResetImage();
		gadgetSupportItem.ResetImage();
		gadgetTrowingItem.ResetImage();
		gadgetToolsItem.ResetImage();
	}

	private void OnEnable()
	{
		if (!Defs.inRespawnWindow)
		{
			base.gameObject.SetActive(false);
		}
	}
}

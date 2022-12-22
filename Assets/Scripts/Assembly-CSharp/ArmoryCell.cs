using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public class ArmoryCell : MonoBehaviour
{
	public GameObject unlocked;

	public UISprite bottomBorderIndicatorSprite;

	public UISprite topBorderIndicatorSprite;

	public GameObject darkForeground;

	public GameObject lockSprite;

	public GameObject championEggImage;

	public List<UILabel> leagueRating;

	public GameObject selectionIndicator;

	public List<UILabel> petName;

	public List<UILabel> petLevel;

	public List<UILabel> eggCondition;

	public UIToggle toggle;

	public GameObject newLabel;

	public GameObject topSeller;

	public MeshRenderer capeRenderer;

	public GameObject newSkinLabel;

	public GameObject rented;

	public GameObject isPriceForUpgrade;

	public GameObject equipped;

	public GameObject modelForSkin;

	public GameObject upgradesContainer;

	public List<UISprite> upgrades;

	public UILabel priceLabel;

	public GameObject gemSprite;

	public GameObject coinSprite;

	public UILabel discountLabel;

	public UISprite discountSprite;

	public UISprite coloredBorder;

	public UISprite boughtIndicator;

	public UITexture icon;

	public bool isEmpty;

	private string firstUnboughtOrForOurTier = string.Empty;

	private string lastBoughtTag = string.Empty;

	private List<string> upgradesChain;

	private string firstTagForOurTier = string.Empty;

	private int lastBoughtUpdateCounter;

	public string ItemId { get; private set; }

	public ShopNGUIController.CategoryNames Category { get; private set; }

	public bool IsFullyVisible
	{
		get
		{
			return bottomBorderIndicatorSprite.isVisible && topBorderIndicatorSprite.isVisible;
		}
	}

	public bool IsBest { get; private set; }

	public static event Action<ArmoryCell> ToggleValueChanged;

	public static event Action<ArmoryCell> Clicked;

	public void MakeCellEmpty()
	{
		IsBest = false;
		isEmpty = true;
		StopAllCoroutines();
		selectionIndicator.SetActiveSafeSelf(false);
		newLabel.SetActiveSafeSelf(false);
		topSeller.SetActiveSafeSelf(false);
		newSkinLabel.SetActiveSafeSelf(false);
		rented.SetActiveSafeSelf(false);
		isPriceForUpgrade.SetActiveSafeSelf(false);
		equipped.SetActiveSafeSelf(false);
		modelForSkin.SetActiveSafeSelf(false);
		upgradesContainer.SetActiveSafeSelf(false);
		priceLabel.gameObject.SetActiveSafeSelf(false);
		gemSprite.SetActiveSafeSelf(false);
		coinSprite.SetActiveSafeSelf(false);
		discountSprite.gameObject.SetActiveSafeSelf(false);
		boughtIndicator.alpha = 0f;
		icon.gameObject.SetActiveSafeSelf(false);
		toggle.enabled = false;
		petName.ForEach(delegate(UILabel lab)
		{
			lab.gameObject.SetActiveSafeSelf(false);
		});
		petLevel.ForEach(delegate(UILabel lab)
		{
			lab.gameObject.SetActiveSafeSelf(false);
		});
		eggCondition.ForEach(delegate(UILabel lab)
		{
			lab.gameObject.SetActiveSafeSelf(false);
		});
		leagueRating.ForEach(delegate(UILabel lab)
		{
			lab.gameObject.SetActiveSafeSelf(false);
		});
		lockSprite.SetActiveSafeSelf(false);
		darkForeground.SetActiveSafeSelf(false);
		unlocked.SetActiveSafeSelf(false);
		UnsubscribeEquipEvents();
	}

	public void SetupEmptyCellCategory(ShopNGUIController.CategoryNames category)
	{
		Category = category;
		switch (Category)
		{
		case ShopNGUIController.CategoryNames.PrimaryCategory:
		case ShopNGUIController.CategoryNames.BackupCategory:
		case ShopNGUIController.CategoryNames.MeleeCategory:
		case ShopNGUIController.CategoryNames.SpecilCategory:
		case ShopNGUIController.CategoryNames.SniperCategory:
		case ShopNGUIController.CategoryNames.PremiumCategory:
		case ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory:
		case ShopNGUIController.CategoryNames.BestWeapons:
			icon.gameObject.SetActiveSafeSelf(true);
			icon.mainTexture = Resources.Load<Texture>("OfferIcons/Empty_Weapon_Icon");
			break;
		case ShopNGUIController.CategoryNames.ThrowingCategory:
		case ShopNGUIController.CategoryNames.ToolsCategoty:
		case ShopNGUIController.CategoryNames.SupportCategory:
		case ShopNGUIController.CategoryNames.BestGadgets:
			icon.gameObject.SetActiveSafeSelf(true);
			icon.mainTexture = Resources.Load<Texture>("OfferIcons/Empty_Gadget_Icon");
			break;
		case ShopNGUIController.CategoryNames.SkinsCategory:
		case ShopNGUIController.CategoryNames.LeagueSkinsCategory:
			icon.gameObject.SetActiveSafeSelf(true);
			icon.mainTexture = Resources.Load<Texture>("OfferIcons/Empty_Skin_Icon");
			break;
		case ShopNGUIController.CategoryNames.EggsCategory:
			icon.gameObject.SetActiveSafeSelf(true);
			icon.mainTexture = Resources.Load<Texture>("OfferIcons/Empty_Egg_Icon");
			break;
		case ShopNGUIController.CategoryNames.HatsCategory:
		case ShopNGUIController.CategoryNames.CapesCategory:
		case ShopNGUIController.CategoryNames.BootsCategory:
		case ShopNGUIController.CategoryNames.MaskCategory:
		case ShopNGUIController.CategoryNames.LeagueHatsCategory:
		case ShopNGUIController.CategoryNames.BestWear:
			icon.gameObject.SetActiveSafeSelf(true);
			icon.mainTexture = Resources.Load<Texture>("OfferIcons/Empty_Wear_Icon");
			break;
		}
	}

	public void ReSubscribeToEquipEvents()
	{
		UnsubscribeEquipEvents();
		SubscribeToEquipEvents();
	}

	internal void Setup(ShopNGUIController.ShopItem item, bool isBest)
	{
		IsBest = isBest;
		isEmpty = false;
		ItemId = item.Id;
		Category = item.Category;
		lastBoughtTag = string.Empty;
		firstTagForOurTier = string.Empty;
		UpdateFirstUnbought();
		if (ShopNGUIController.IsWeaponCategory(Category) || ShopNGUIController.IsWearCategory(Category))
		{
			lastBoughtTag = WeaponManager.LastBoughtTag(ItemId);
		}
		else if (ShopNGUIController.IsGadgetsCategory(Category))
		{
			lastBoughtTag = GadgetsInfo.LastBoughtFor(ItemId);
		}
		else if (Category == ShopNGUIController.CategoryNames.PetsCategory)
		{
			lastBoughtTag = ((!PetsManager.IsEmptySlotId(ItemId)) ? ItemId : string.Empty);
		}
		if (ShopNGUIController.IsWeaponCategory(Category))
		{
			upgradesChain = WeaponUpgrades.ChainForTag(ItemId);
			if (upgradesChain != null)
			{
				firstTagForOurTier = WeaponManager.FirstTagForOurTier(ItemId);
			}
		}
		else if (ShopNGUIController.IsGadgetsCategory(Category))
		{
			upgradesChain = GadgetsInfo.UpgradesChainForGadget(ItemId);
			if (upgradesChain != null)
			{
				firstTagForOurTier = GadgetsInfo.FirstForOurTier(ItemId);
			}
		}
		lastBoughtUpdateCounter = 0;
	}

	public void UpdateAllAndStartUpdateCoroutine()
	{
		if (isEmpty)
		{
			return;
		}
		toggle.enabled = Category != ShopNGUIController.CategoryNames.PetsCategory || !PetsManager.IsEmptySlotId(ItemId);
		selectionIndicator.SetActiveSafeSelf(Category != ShopNGUIController.CategoryNames.EggsCategory && (Category != ShopNGUIController.CategoryNames.PetsCategory || !PetsManager.IsEmptySlotId(ItemId)));
		bool state = ItemId == "cape_Custom" && Storager.getInt("cape_Custom", true) > 0;
		capeRenderer.gameObject.SetActiveSafeSelf(state);
		bool state2 = ItemId == "CustomSkinID" && !IsUnboughtSkinsEditor();
		newSkinLabel.SetActiveSafeSelf(state2);
		petName.ForEach(delegate(UILabel lab)
		{
			lab.gameObject.SetActiveSafeSelf(Category == ShopNGUIController.CategoryNames.EggsCategory);
		});
		petLevel.ForEach(delegate(UILabel lab)
		{
			lab.gameObject.SetActiveSafeSelf(Category == ShopNGUIController.CategoryNames.PetsCategory && !PetsManager.IsEmptySlotId(ItemId));
		});
		if (Category == ShopNGUIController.CategoryNames.EggsCategory)
		{
			Egg egg2 = Singleton<EggsManager>.Instance.GetPlayerEggs().FirstOrDefault((Egg e) => e.Id.ToString() == ItemId);
			eggCondition.ForEach(delegate(UILabel lab)
			{
				lab.gameObject.SetActiveSafeSelf(egg2 != null && egg2.Data.Rare != EggRarity.Champion);
			});
			if (egg2 == null)
			{
				Debug.LogErrorFormat("UpdateAllAndStartUpdateCoroutine: egg == null, ItemId = {0}", ItemId ?? "null");
			}
		}
		else
		{
			eggCondition.ForEach(delegate(UILabel lab)
			{
				lab.gameObject.SetActiveSafeSelf(false);
			});
		}
		if (Category == ShopNGUIController.CategoryNames.EggsCategory)
		{
			Egg egg = Singleton<EggsManager>.Instance.GetPlayerEggs().FirstOrDefault((Egg e) => e.Id.ToString() == ItemId);
			leagueRating.ForEach(delegate(UILabel lab)
			{
				lab.gameObject.SetActiveSafeSelf(egg != null && egg.Data.Rare == EggRarity.Champion);
			});
			if (egg == null)
			{
				Debug.LogErrorFormat("UpdateAllAndStartUpdateCoroutine: egg == null, ItemId = {0}", ItemId ?? "null");
			}
		}
		else
		{
			leagueRating.ForEach(delegate(UILabel lab)
			{
				lab.gameObject.SetActiveSafeSelf((Category == ShopNGUIController.CategoryNames.SkinsCategory && !SkinsController.IsLeagueSkinAvailableByLeague(ItemId) && !SkinsController.IsSkinBought(ItemId)) || (Category == ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory && !WeaponSkinsManager.IsAvailableByLeague(ItemId) && !WeaponSkinsManager.IsBoughtSkin(ItemId)) || (Category == ShopNGUIController.CategoryNames.HatsCategory && Wear.LeagueForWear(ItemId, ShopNGUIController.CategoryNames.HatsCategory) > (int)RatingSystem.instance.currentLeague && Storager.getInt(ItemId, true) == 0));
			});
		}
		bool state3 = Category == ShopNGUIController.CategoryNames.SkinsCategory && !IsUnboughtSkinsEditor();
		modelForSkin.SetActiveSafeSelf(state3);
		bool state4 = (ItemId != "cape_Custom" || Storager.getInt("cape_Custom", true) == 0) && (Category != ShopNGUIController.CategoryNames.SkinsCategory || IsUnboughtSkinsEditor());
		icon.gameObject.SetActiveSafeSelf(state4);
		if (Category == ShopNGUIController.CategoryNames.SkinsCategory)
		{
			if (IsUnboughtSkinsEditor())
			{
				SetIcon();
			}
			else
			{
				try
				{
					modelForSkin.GetComponent<MeshRenderer>().material.mainTexture = ((!(ItemId == "CustomSkinID")) ? SkinsController.skinsForPers[ItemId] : Resources.Load<Texture>("skins_maker_skin"));
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in ArmoryCell SetTextureRecursivelyFrom: " + ex);
				}
			}
		}
		else if (ItemId == "cape_Custom" && Storager.getInt("cape_Custom", true) > 0)
		{
			try
			{
				if (SkinsController.capeUserTexture != null)
				{
					capeRenderer.material.mainTexture = SkinsController.capeUserTexture;
				}
			}
			catch (Exception ex2)
			{
				Debug.LogError("Exception in ArmoryCell capeRenderer.material.mainTexture = : " + ex2);
			}
		}
		else
		{
			SetIcon();
		}
		if (ShopNGUIController.IsWeaponCategory(Category))
		{
			WeaponSounds weaponInfo = ItemDb.GetWeaponInfo(ItemId);
			if (weaponInfo != null)
			{
				Color color = ColorForRarity(weaponInfo.rarity);
				coloredBorder.color = color;
				string prefabName = ItemDb.GetByTag(ItemId).PrefabName;
				SetEquipped(WeaponManager.sharedManager.playerWeapons.OfType<Weapon>().Any((Weapon w) => w.weaponPrefab.nameNoClone() == prefabName));
			}
		}
		else
		{
			coloredBorder.color = ColorForRarity(ItemDb.ItemRarity.Common);
		}
		if (Category == ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory)
		{
			try
			{
				WeaponSkin skin = WeaponSkinsManager.GetSkin(ItemId);
				if (skin != null)
				{
					ItemRecord byPrefabName = ItemDb.GetByPrefabName(skin.ToWeapons[0]);
					if (byPrefabName != null)
					{
						HandleWeaponSkinsManager_EquippedSkinForWeapon(byPrefabName, WeaponSkinsManager.GetSettedSkinId(byPrefabName.PrefabName));
					}
					else
					{
						SetEquipped(false);
					}
				}
				else
				{
					SetEquipped(false);
				}
			}
			catch (Exception ex3)
			{
				Debug.LogError("Exception in initial setting equipped weapon skin in ArmoryCell: " + ex3);
			}
		}
		if (ShopNGUIController.IsWearCategory(Category))
		{
			SetEquipped(Storager.getString(ShopNGUIController.SnForWearCategory(Category), false) == ItemId);
		}
		if (Category == ShopNGUIController.CategoryNames.SkinsCategory)
		{
			HandleSkinsController_EquippedSkin(SkinsController.currentSkinNameForPers ?? string.Empty, string.Empty);
			upgradesContainer.SetActiveSafeSelf(false);
		}
		if (Category == ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory || Category == ShopNGUIController.CategoryNames.EggsCategory)
		{
			upgradesContainer.SetActiveSafeSelf(false);
		}
		if (Category == ShopNGUIController.CategoryNames.PetsCategory)
		{
			upgradesContainer.SetActiveSafeSelf(false);
			SetEquipped(Singleton<PetsManager>.Instance.GetEqipedPetId() == ItemId);
		}
		if (ShopNGUIController.IsGadgetsCategory(Category))
		{
			SetEquipped(ItemId == GadgetsInfo.EquippedForCategory((GadgetInfo.GadgetCategory)Category));
		}
		if (Category == ShopNGUIController.CategoryNames.EggsCategory)
		{
			SetEquipped(false);
		}
		StartUpdateInfo();
		UpdateRented();
	}

	public void ToggleClicked()
	{
		bool value = toggle.value;
		UpdatePriceAndDiscount();
		UpdateUpgrades();
		if (value)
		{
			Action<ArmoryCell> toggleValueChanged = ArmoryCell.ToggleValueChanged;
			if (toggleValueChanged != null)
			{
				toggleValueChanged(this);
			}
		}
	}

	public void UpdateDiscountVisibility()
	{
		discountSprite.alpha = ((!toggle.value) ? 0f : 1f);
	}

	private void HandleWeaponSkinsManager_EquippedSkinForWeapon(ItemRecord weaponRecord, string skinId)
	{
		if (weaponRecord == null || skinId == null || Category != ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory)
		{
			if (weaponRecord == null || skinId == null)
			{
				SetEquipped(false);
			}
			return;
		}
		try
		{
			WeaponSkin skin = WeaponSkinsManager.GetSkin(ItemId);
			if (skin != null && skin.IsForWeapon(weaponRecord.PrefabName))
			{
				SetEquipped(skinId == ItemId);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in HandleWeaponSkinsManager_EquippedSkinForWeapon: " + ex);
		}
	}

	private void HandleSkinsController_EquippedSkin(string newSkin, string oldSkin)
	{
		if (ItemId == "CustomSkinID" && Category == ShopNGUIController.CategoryNames.SkinsCategory)
		{
			SetEquipped(false);
		}
		else if (Category == ShopNGUIController.CategoryNames.SkinsCategory)
		{
			SetEquipped(newSkin == ItemId);
		}
	}

	private void StartUpdateInfo()
	{
		if (ItemId != null)
		{
			StartCoroutine(UpdateInfo());
		}
	}

	private void SetIcon()
	{
		string text = ItemId;
		if (Category == ShopNGUIController.CategoryNames.EggsCategory)
		{
			Egg egg = Singleton<EggsManager>.Instance.GetPlayerEggs().FirstOrDefault((Egg e) => e.Id.ToString() == ItemId);
			if (egg != null)
			{
				text = egg.Data.Id;
			}
			else
			{
				Debug.LogErrorFormat("ArmoryCell: SetIcon, egg = null, ItemId = {0}", ItemId);
			}
		}
		Texture itemIcon = ItemDb.GetItemIcon(text, Category);
		if (itemIcon != null)
		{
			icon.mainTexture = itemIcon;
		}
	}

	private void OnEnable()
	{
		if (!isEmpty)
		{
			ReSubscribeToEquipEvents();
			UpdateAllAndStartUpdateCoroutine();
		}
	}

	private void OnDisable()
	{
		UnsubscribeEquipEvents();
	}

	private void SubscribeToEquipEvents()
	{
		WeaponManager.WeaponEquipped_AllCases += HandleWeaponManager_WeaponEquipped_AllCases;
		ShopNGUIController.EquippedWearInCategory += HandleEquippedWearInCategory;
		ShopNGUIController.UnequippedWearInCategory += Handle_UnequippedWearInCategory;
		SkinsController.EquippedSkin += HandleSkinsController_EquippedSkin;
		WeaponSkinsManager.EquippedSkinForWeapon += HandleWeaponSkinsManager_EquippedSkinForWeapon;
		ShopNGUIController.EquippedPet += ShopNGUIController_EquippedPet;
		ShopNGUIController.UnequippedPet += ShopNGUIController_UnequippedPet;
		ShopNGUIController.EquippedGadget += ShopNGUIController_EquippedGadget;
	}

	private void UnsubscribeEquipEvents()
	{
		WeaponManager.WeaponEquipped_AllCases -= HandleWeaponManager_WeaponEquipped_AllCases;
		ShopNGUIController.EquippedWearInCategory -= HandleEquippedWearInCategory;
		ShopNGUIController.UnequippedWearInCategory -= Handle_UnequippedWearInCategory;
		SkinsController.EquippedSkin -= HandleSkinsController_EquippedSkin;
		WeaponSkinsManager.EquippedSkinForWeapon -= HandleWeaponSkinsManager_EquippedSkinForWeapon;
		ShopNGUIController.EquippedPet -= ShopNGUIController_EquippedPet;
		ShopNGUIController.UnequippedPet -= ShopNGUIController_UnequippedPet;
		ShopNGUIController.EquippedGadget -= ShopNGUIController_EquippedGadget;
	}

	private void ShopNGUIController_UnequippedPet(string obj)
	{
		if (Category == ShopNGUIController.CategoryNames.PetsCategory)
		{
			SetEquipped(false);
		}
	}

	private void ShopNGUIController_EquippedGadget(string newGadget, string oldGadget, GadgetInfo.GadgetCategory gadgetCategory)
	{
		if (ShopNGUIController.IsGadgetsCategory((ShopNGUIController.CategoryNames)gadgetCategory) && gadgetCategory == (GadgetInfo.GadgetCategory)Category)
		{
			SetEquipped(GadgetsInfo.EquippedForCategory(gadgetCategory) == ItemId);
		}
	}

	private void ShopNGUIController_EquippedPet(string newPet, string oldPet)
	{
		if (Category == ShopNGUIController.CategoryNames.PetsCategory)
		{
			SetEquipped(ItemId == newPet);
		}
	}

	private void UpdateSkins()
	{
		if (Category != ShopNGUIController.CategoryNames.SkinsCategory)
		{
			return;
		}
		try
		{
			bool flag = !IsUnboughtSkinsEditor() && !SkinsController.IsLeagueSkinAvailableByLeague(ItemId) && !SkinsController.IsSkinBought(ItemId);
			lockSprite.SetActiveSafeSelf(flag);
			darkForeground.SetActiveSafeSelf(flag);
			if (flag)
			{
				SkinItem skinItem = SkinsController.sharedController.skinItemsDict[ItemId];
				leagueRating.ForEach(delegate(UILabel lab)
				{
					lab.text = RatingSystem.instance.RatingNeededForLeague(skinItem.currentLeague).ToString();
				});
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in ArmoryCell.UpdateSkins: {0}", ex);
		}
	}

	private void UpdateWeaponSkins()
	{
		if (Category != ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory)
		{
			return;
		}
		try
		{
			bool flag = !WeaponSkinsManager.IsAvailableByLeague(ItemId) && !WeaponSkinsManager.IsBoughtSkin(ItemId);
			lockSprite.SetActiveSafeSelf(flag);
			darkForeground.SetActiveSafeSelf(flag);
			if (flag)
			{
				WeaponSkin skinItem = WeaponSkinsManager.GetSkin(ItemId);
				leagueRating.ForEach(delegate(UILabel lab)
				{
					lab.text = RatingSystem.instance.RatingNeededForLeague(skinItem.ForLeague).ToString();
				});
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in ArmoryCell.UpdateWeaponSkins: {0}", ex);
		}
	}

	private void UpdateLeagueHats()
	{
		if (Category != ShopNGUIController.CategoryNames.HatsCategory)
		{
			return;
		}
		try
		{
			RatingSystem.RatingLeague leagueOfCurrentItem = (RatingSystem.RatingLeague)Wear.LeagueForWear(ItemId, ShopNGUIController.CategoryNames.HatsCategory);
			bool flag = leagueOfCurrentItem > RatingSystem.instance.currentLeague && Storager.getInt(ItemId, true) == 0;
			lockSprite.SetActiveSafeSelf(flag);
			darkForeground.SetActiveSafeSelf(flag);
			if (flag)
			{
				leagueRating.ForEach(delegate(UILabel lab)
				{
					lab.text = RatingSystem.instance.RatingNeededForLeague(leagueOfCurrentItem).ToString();
				});
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in ArmoryCell.UpdateLeagueHats: {0}", ex);
		}
	}

	private void UpdatePetsAndEggs()
	{
		if (Category == ShopNGUIController.CategoryNames.PetsCategory)
		{
			try
			{
				if (!PetsManager.IsEmptySlotId(ItemId))
				{
					petLevel.ForEach(delegate(UILabel lab)
					{
						lab.text = string.Format(LocalizationStore.Get("Key_2496"), Singleton<PetsManager>.Instance.GetInfo(lastBoughtTag).Up + 1);
					});
				}
				return;
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in setting pet cell gui: {0}", ex);
				return;
			}
		}
		if (Category != ShopNGUIController.CategoryNames.EggsCategory)
		{
			return;
		}
		try
		{
			Egg ourEgg = Singleton<EggsManager>.Instance.GetPlayerEggs().FirstOrDefault((Egg egg) => egg.Id.ToString() == ItemId);
			eggCondition.ForEach(delegate(UILabel lab)
			{
				lab.text = EggHatchingConditionFormatter.TextForConditionOfEgg(ourEgg);
			});
			leagueRating.ForEach(delegate(UILabel lab)
			{
				lab.text = EggHatchingConditionFormatter.TextForConditionOfEgg(ourEgg);
			});
			petName.ForEach(delegate(UILabel lab)
			{
				lab.text = LocalizationStore.Get(EggData.LkeyForRarity(ourEgg.Data.Rare)).ToUpperInvariant();
			});
		}
		catch (Exception ex2)
		{
			Debug.LogErrorFormat("Exception in setting egg cell gui: {0}", ex2);
		}
	}

	private IEnumerator UpdateInfo()
	{
		while (true)
		{
			try
			{
				RiliExtensions.SetActiveSafeSelf(state: PromoActionsManager.sharedManager != null && ItemId != null && Category != ShopNGUIController.CategoryNames.EggsCategory && PromoActionsManager.sharedManager.UnlockedItems.Union(PromoActionsManager.sharedManager.ItemsToRemoveFromUnlocked).Contains(ItemId) && lastBoughtTag.IsNullOrEmpty() && !IsBest, go: unlocked);
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				Debug.LogErrorFormat("Exception in UpdateInfo, unlocked: {0}", ex);
			}
			lockSprite.SetActiveSafeSelf(false);
			darkForeground.SetActiveSafeSelf(false);
			UpdatePriceAndDiscount();
			UpdateUpgrades();
			UpdateNewAndTopSeller();
			UpdatePetsAndEggs();
			UpdateSkins();
			UpdateWeaponSkins();
			UpdateLeagueHats();
			yield return CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.WaitForSeconds(0.5f));
			if (this == null || base.gameObject == null)
			{
				break;
			}
			try
			{
				lastBoughtUpdateCounter++;
				if (lastBoughtUpdateCounter % 10 != 0)
				{
					continue;
				}
				if (ShopNGUIController.IsWeaponCategory(Category) || ShopNGUIController.IsWearCategory(Category))
				{
					string newLastBoughtTag = WeaponManager.LastBoughtTag(ItemId);
					if (newLastBoughtTag != lastBoughtTag)
					{
						lastBoughtTag = newLastBoughtTag;
						UpdateFirstUnbought();
					}
				}
				else if (ShopNGUIController.IsGadgetsCategory(Category))
				{
					string newLastBought = GadgetsInfo.LastBoughtFor(ItemId);
					if (newLastBought != lastBoughtTag)
					{
						lastBoughtTag = newLastBought;
						UpdateFirstUnbought();
					}
				}
				else if (Category == ShopNGUIController.CategoryNames.PetsCategory)
				{
					string petId = lastBoughtTag;
					if (petId.IsNullOrEmpty())
					{
						petId = PetsManager.PetIdWithoutSuffixes(ItemId);
					}
					PlayerPet newLastBoughtPetInfo = Singleton<PetsManager>.Instance.GetPlayerPet(petId);
					string newLastBought2 = ((newLastBoughtPetInfo == null) ? lastBoughtTag : newLastBoughtPetInfo.InfoId);
					if (newLastBought2 != lastBoughtTag)
					{
						lastBoughtTag = newLastBought2;
						UpdateFirstUnbought();
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat("Exception in UpdateInfo: {0}", e);
			}
		}
	}

	private void HandleEquippedWearInCategory(string newEquipped, ShopNGUIController.CategoryNames unused, string equippedBefore)
	{
		if (ShopNGUIController.IsWearCategory(Category))
		{
			if (equippedBefore == ItemId)
			{
				SetEquipped(false);
			}
			if (newEquipped == ItemId)
			{
				SetEquipped(true);
			}
		}
	}

	private void Handle_UnequippedWearInCategory(ShopNGUIController.CategoryNames unused, string equippedBefore)
	{
		if (ShopNGUIController.IsWearCategory(Category) && equippedBefore == ItemId)
		{
			SetEquipped(false);
		}
	}

	private void HandleWeaponManager_WeaponEquipped_AllCases(WeaponSounds ws)
	{
		if (Category == (ShopNGUIController.CategoryNames)(ws.categoryNabor - 1))
		{
			SetEquipped(ws.nameNoClone() == ItemDb.GetByTag(ItemId).PrefabName);
		}
	}

	public void UpdateNewAndTopSeller()
	{
		bool flag = ShopNGUIController.IsWeaponCategory(Category) || ShopNGUIController.IsWearCategory(Category) || ShopNGUIController.IsGadgetsCategory(Category) || Category == ShopNGUIController.CategoryNames.SkinsCategory;
		bool flag2 = !equipped.activeSelf && lastBoughtTag.IsNullOrEmpty() && (Category != ShopNGUIController.CategoryNames.SkinsCategory || !SkinsController.IsSkinBought(ItemId));
		bool flag3 = !unlocked.activeSelf && Category != ShopNGUIController.CategoryNames.EggsCategory && flag2 && PromoActionsManager.sharedManager != null && PromoActionsManager.sharedManager.news.Contains(ItemId);
		bool flag4 = !unlocked.activeSelf && Category != ShopNGUIController.CategoryNames.EggsCategory && !flag3 && flag2 && PromoActionsManager.sharedManager != null && PromoActionsManager.sharedManager.topSellers.Contains(ItemId);
		newLabel.SetActiveSafeSelf(flag3 && flag);
		topSeller.SetActiveSafeSelf(flag4 && flag);
	}

	private void UpdateBoughtIndicator()
	{
		bool flag = Category == ShopNGUIController.CategoryNames.SkinsCategory && !IsUnboughtSkinsEditor() && !SkinsController.IsSkinBought(ItemId) && SkinsController.leagueSkinsIds.Contains(ItemId);
		bool flag2 = Category == ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory && !WeaponSkinsManager.IsBoughtSkin(ItemId);
		bool flag3 = false;
		if (Category == ShopNGUIController.CategoryNames.HatsCategory)
		{
			IEnumerable<string> source = Wear.UnboughtLeagueItemsByLeagues().SelectMany((KeyValuePair<RatingSystem.RatingLeague, List<string>> kvp) => kvp.Value);
			flag3 = source.Contains(ItemId);
		}
		boughtIndicator.alpha = ((Category == ShopNGUIController.CategoryNames.EggsCategory || (lastBoughtTag.IsNullOrEmpty() && HasPrice()) || (Category == ShopNGUIController.CategoryNames.PetsCategory && PetsManager.IsEmptySlotId(ItemId)) || (ItemId == "super_socialman" && Storager.getInt(Defs.IsFacebookLoginRewardaGained, true) == 0) || Category == ShopNGUIController.CategoryNames.EggsCategory || flag || flag2 || flag3) ? 0f : ((!toggle.value) ? 1f : 0f));
	}

	public void UpdateUpgrades()
	{
		UpdateBoughtIndicator();
		if (ShopNGUIController.IsWeaponCategory(Category))
		{
			if ((lastBoughtTag.IsNullOrEmpty() && !toggle.value) || (WeaponManager.sharedManager != null && WeaponManager.sharedManager.IsAvailableTryGun(ItemId)))
			{
				upgradesContainer.SetActiveSafeSelf(false);
				return;
			}
			upgradesContainer.SetActiveSafeSelf(true);
			List<string> actualUpgrades2 = null;
			if (upgradesChain != null)
			{
				int num = upgradesChain.IndexOf(firstTagForOurTier);
				actualUpgrades2 = upgradesChain.GetRange(num, upgradesChain.Count - num);
			}
			else
			{
				actualUpgrades2 = new List<string> { ItemId };
			}
			upgrades.ForEach(delegate(UISprite sprite)
			{
				int num5 = upgrades.IndexOf(sprite);
				bool state2 = num5 < actualUpgrades2.Count;
				sprite.gameObject.SetActiveSafeSelf(state2);
			});
			for (int i = 0; i < actualUpgrades2.Count; i++)
			{
				ItemRecord byTag = ItemDb.GetByTag(actualUpgrades2[i]);
				bool haveUpgrade = byTag.StorageId == null || Storager.getInt(byTag.StorageId, true) > 0;
				upgrades[i].spriteName = SpriteNameForUpgradeState(haveUpgrade);
			}
		}
		else if (ShopNGUIController.IsWearCategory(Category))
		{
			bool maxUpgrade;
			int totalNumberOfUpgrades;
			int num2 = ShopNGUIController.CurrentNumberOfUpgradesForWear(ItemId, out maxUpgrade, Category, out totalNumberOfUpgrades);
			bool flag = num2 > 0 || (totalNumberOfUpgrades > 1 && toggle.value);
			upgradesContainer.SetActiveSafeSelf(flag);
			if (flag)
			{
				for (int j = 0; j < upgrades.Count; j++)
				{
					upgrades[j].gameObject.SetActiveSafeSelf(j < totalNumberOfUpgrades);
					bool haveUpgrade2 = j < num2;
					upgrades[j].spriteName = SpriteNameForUpgradeState(haveUpgrade2);
				}
			}
		}
		else if (ShopNGUIController.IsGadgetsCategory(Category))
		{
			if (lastBoughtTag.IsNullOrEmpty() && !toggle.value)
			{
				upgradesContainer.SetActiveSafeSelf(false);
				return;
			}
			upgradesContainer.SetActiveSafeSelf(true);
			List<string> actualUpgrades = null;
			if (upgradesChain != null)
			{
				int num3 = upgradesChain.IndexOf(firstTagForOurTier);
				actualUpgrades = upgradesChain.GetRange(num3, upgradesChain.Count - num3);
			}
			else
			{
				actualUpgrades = new List<string> { ItemId };
			}
			upgrades.ForEach(delegate(UISprite sprite)
			{
				int num4 = upgrades.IndexOf(sprite);
				bool state = num4 < actualUpgrades.Count;
				sprite.gameObject.SetActiveSafeSelf(state);
			});
			for (int k = 0; k < actualUpgrades.Count; k++)
			{
				bool haveUpgrade3 = GadgetsInfo.IsBought(actualUpgrades[k]);
				upgrades[k].spriteName = SpriteNameForUpgradeState(haveUpgrade3);
			}
		}
		else if (Category != ShopNGUIController.CategoryNames.PetsCategory)
		{
		}
	}

	private void UpdateRented()
	{
		bool state = ShopNGUIController.IsWeaponCategory(Category) && WeaponManager.sharedManager != null && WeaponManager.sharedManager.IsAvailableTryGun(ItemId);
		rented.SetActiveSafeSelf(state);
	}

	private static string SpriteNameForUpgradeState(bool haveUpgrade)
	{
		return (!haveUpgrade) ? "Lev_comp_gray_star" : "Lev_comp_gold_star";
	}

	private bool HasPrice()
	{
		bool result = false;
		if (ShopNGUIController.IsWeaponCategory(Category) || ShopNGUIController.IsGadgetsCategory(Category))
		{
			result = (!ShopNGUIController.IsWeaponCategory(Category) || ItemDb.IsCanBuy(ItemId)) && firstUnboughtOrForOurTier != lastBoughtTag;
		}
		else if (ShopNGUIController.IsWearCategory(Category))
		{
			Dictionary<Wear.LeagueItemState, List<string>> dictionary = Wear.LeagueItems();
			bool flag = dictionary[Wear.LeagueItemState.Closed].Contains(ItemId) && !dictionary[Wear.LeagueItemState.Purchased].Contains(ItemId);
			result = firstUnboughtOrForOurTier != lastBoughtTag && !flag;
		}
		else if (Category == ShopNGUIController.CategoryNames.SkinsCategory)
		{
			if (IsUnboughtSkinsEditor())
			{
				result = true;
			}
			else if (!SkinsController.IsCustomSkinId(ItemId))
			{
				try
				{
					bool isForMoneySkin;
					bool flag2 = SkinsController.IsSkinBought(ItemId, out isForMoneySkin);
					result = ItemId != "super_socialman" && isForMoneySkin && !flag2 && SkinsController.IsLeagueSkinAvailableByLeague(ItemId);
					return result;
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in hasPrice for skin: " + ex);
					return result;
				}
			}
		}
		else
		{
			if (Category == ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory)
			{
				try
				{
					result = WeaponSkinsManager.AvailableForBuy(WeaponSkinsManager.GetSkin(ItemId));
					return result;
				}
				catch (Exception ex2)
				{
					Debug.LogErrorFormat("Exception in determining has price for league weapon skin(ArmoryCell): skinId = {0}\n, {1}", ItemId ?? "null", ex2);
					return result;
				}
			}
			if (Category == ShopNGUIController.CategoryNames.PetsCategory)
			{
				try
				{
					result = !PetsManager.IsEmptySlotId(ItemId) && Singleton<PetsManager>.Instance.GetNextUp(ItemId) != null;
					return result;
				}
				catch (Exception ex3)
				{
					Debug.LogErrorFormat("Exception in getting hasPrice for pets: {0}", ex3);
					return result;
				}
			}
		}
		return result;
	}

	private void UpdatePriceAndDiscount()
	{
		bool flag = HasPrice();
		if (flag)
		{
			ItemPrice itemPrice = ShopNGUIController.GetItemPrice((Category != ShopNGUIController.CategoryNames.SkinsCategory && Category != ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory) ? firstUnboughtOrForOurTier : ItemId, Category, false, true, true);
			priceLabel.text = itemPrice.Price.ToString();
			GameObject go = ((!(itemPrice.Currency == "GemsCurrency")) ? coinSprite : gemSprite);
			go.SetActiveSafeSelf(true);
			GameObject go2 = ((!(itemPrice.Currency == "Coins")) ? coinSprite : gemSprite);
			go2.SetActiveSafeSelf(false);
			bool state = Category != ShopNGUIController.CategoryNames.SkinsCategory && Category != ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory && !lastBoughtTag.IsNullOrEmpty();
			isPriceForUpgrade.SetActiveSafeSelf(state);
		}
		priceLabel.gameObject.SetActiveSafeSelf(flag);
		if ((Category == ShopNGUIController.CategoryNames.SkinsCategory && ItemId == "CustomSkinID" && !IsUnboughtSkinsEditor()) || Category == ShopNGUIController.CategoryNames.EggsCategory || (Category == ShopNGUIController.CategoryNames.SkinsCategory && !SkinsController.IsLeagueSkinAvailableByLeague(ItemId)) || (Category == ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory && !WeaponSkinsManager.IsAvailableByLeague(ItemId)) || (Category == ShopNGUIController.CategoryNames.PetsCategory && PetsManager.IsEmptySlotId(ItemId)))
		{
			discountSprite.gameObject.SetActiveSafeSelf(false);
			return;
		}
		bool onlyServerDiscount;
		int num = ShopNGUIController.DiscountFor((Category != ShopNGUIController.CategoryNames.SkinsCategory) ? firstUnboughtOrForOurTier : ItemId, out onlyServerDiscount);
		bool flag2 = num > 0 && flag;
		discountSprite.gameObject.SetActiveSafeSelf(flag2);
		if (flag2)
		{
			discountLabel.text = "-" + num + "%";
			UpdateDiscountVisibility();
		}
	}

	private bool IsUnboughtSkinsEditor()
	{
		return ItemId == "CustomSkinID" && Storager.getInt(Defs.SkinsMakerInProfileBought, true) == 0;
	}

	private void SetEquipped(bool e)
	{
		equipped.SetActiveSafeSelf(e);
		UpdateNewAndTopSeller();
	}

	private Color ColorForRarity(ItemDb.ItemRarity rarity)
	{
		switch (rarity)
		{
		case ItemDb.ItemRarity.Common:
			return new Color(0.8902f, 0.8902f, 0.8902f);
		case ItemDb.ItemRarity.Uncommon:
			return new Color(0.55294f, 1f, 0.01176f);
		case ItemDb.ItemRarity.Rare:
			return new Color(0.01176f, 0.80392f, 1f);
		case ItemDb.ItemRarity.Epic:
			return new Color(1f, 0.86667f, 0.01176f);
		case ItemDb.ItemRarity.Legendary:
			return new Color(1f, 0.36471f, 0.36471f);
		default:
			return new Color(0.77647f, 0.00784f, 0.80392f);
		}
	}

	private void OnClick()
	{
		if (toggle.value)
		{
			Action<ArmoryCell> clicked = ArmoryCell.Clicked;
			if (clicked != null)
			{
				clicked(this);
			}
		}
	}

	private void UpdateFirstUnbought()
	{
		try
		{
			if (ShopNGUIController.IsWeaponCategory(Category))
			{
				firstUnboughtOrForOurTier = WeaponManager.FirstUnboughtOrForOurTier(ItemId);
			}
			else if (ShopNGUIController.IsWearCategory(Category))
			{
				firstUnboughtOrForOurTier = WeaponManager.FirstUnboughtTag(ItemId);
			}
			else if (ShopNGUIController.IsGadgetsCategory(Category))
			{
				firstUnboughtOrForOurTier = GadgetsInfo.FirstUnboughtOrForOurTier(ItemId);
			}
			else if (Category == ShopNGUIController.CategoryNames.PetsCategory)
			{
				if (!PetsManager.IsEmptySlotId(ItemId))
				{
					PetInfo firstUnboughtPet = Singleton<PetsManager>.Instance.GetFirstUnboughtPet(ItemId);
					firstUnboughtOrForOurTier = ((firstUnboughtPet == null) ? firstUnboughtOrForOurTier : firstUnboughtPet.Id);
				}
			}
			else
			{
				firstUnboughtOrForOurTier = ItemId;
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in ArmoryCell.UpdateFirstUnbought: {0}", ex);
		}
	}
}

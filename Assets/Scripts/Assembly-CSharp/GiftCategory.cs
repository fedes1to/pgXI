using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

[Serializable]
public class GiftCategory
{
	public GiftCategoryType Type;

	public int ScrollPosition;

	public string KeyTranslateInfoCommon = string.Empty;

	private readonly List<GiftInfo> _rootGifts = new List<GiftInfo>();

	private List<GiftInfo> _ag;

	private List<string> _availableRandomProducts;

	private List<GiftInfo> _allGifts
	{
		get
		{
			return _ag ?? (_ag = GetAvailableGifts());
		}
		set
		{
			_ag = value;
		}
	}

	public bool AnyGifts
	{
		get
		{
			return _allGifts.Any();
		}
	}

	public float PercentChance
	{
		get
		{
			if (Type == GiftCategoryType.Guns_gray || Type == GiftCategoryType.Masks || Type == GiftCategoryType.Boots || Type == GiftCategoryType.Capes || Type == GiftCategoryType.Hats_random || Type == GiftCategoryType.ArmorAndHat || Type == GiftCategoryType.WeaponSkin || Type == GiftCategoryType.Gadgets)
			{
				return _allGifts[0].PercentAddInSlot;
			}
			return _allGifts.Sum((GiftInfo g) => g.PercentAddInSlot);
		}
	}

	private List<GiftInfo> _availableGifts
	{
		get
		{
			return _allGifts.Where((GiftInfo g) => AvailableGift(g.Id, Type)).ToList();
		}
	}

	public int AvaliableGiftsCount
	{
		get
		{
			return _availableGifts.Count;
		}
	}

	private float _availableGiftsPercentSum
	{
		get
		{
			return _availableGifts.Sum((GiftInfo g) => g.PercentAddInSlot);
		}
	}

	public void AddGift(GiftInfo info)
	{
		_rootGifts.Add(info);
	}

	public void CheckGifts()
	{
		_allGifts = GetAvailableGifts();
		foreach (GiftInfo allGift in _allGifts)
		{
			if (Type == GiftCategoryType.ArmorAndHat)
			{
				allGift.Id = Wear.ArmorOrArmorHatAvailableForBuy(ShopNGUIController.CategoryNames.ArmorCategory);
			}
			if (Type == GiftCategoryType.Skins)
			{
				allGift.IsRandomSkin = true;
				allGift.Id = SkinsController.RandomUnboughtSkinId();
			}
		}
	}

	public bool AvailableGift(string idGift, GiftCategoryType curType)
	{
		if (string.IsNullOrEmpty(idGift))
		{
			return false;
		}
		switch (curType)
		{
		case GiftCategoryType.Coins:
		case GiftCategoryType.Gems:
		case GiftCategoryType.Gear:
		case GiftCategoryType.Event_content:
		case GiftCategoryType.Freespins:
			return true;
		case GiftCategoryType.Guns_gray:
		{
			if (idGift.IsNullOrEmpty())
			{
				return false;
			}
			ItemRecord itemRecord = GiftController.GrayCategoryWeapons[ExpController.OurTierForAnyPlace()].FirstOrDefault((ItemRecord rec) => rec.Tag == idGift);
			return itemRecord != null && Storager.getInt(itemRecord.StorageId, true) == 0;
		}
		case GiftCategoryType.Gun1:
		case GiftCategoryType.Gun2:
		case GiftCategoryType.Gun3:
		case GiftCategoryType.Gun4:
			return Storager.getInt(idGift, true) == 0;
		case GiftCategoryType.Wear:
			return !ItemDb.IsItemInInventory(idGift);
		case GiftCategoryType.ArmorAndHat:
		{
			string text = Wear.ArmorOrArmorHatAvailableForBuy(ShopNGUIController.CategoryNames.ArmorCategory);
			return idGift == text;
		}
		case GiftCategoryType.Skins:
		{
			bool isForMoneySkin = false;
			return !SkinsController.IsSkinBought(idGift, out isForMoneySkin);
		}
		case GiftCategoryType.Editor:
			if (idGift.IsNullOrEmpty() || (idGift != "editor_Cape" && idGift != "editor_Skin"))
			{
				return false;
			}
			if (idGift == "editor_Skin" && Storager.getInt(Defs.SkinsMakerInProfileBought, false) > 0)
			{
				return false;
			}
			if (idGift == "editor_Cape" && Storager.getInt("cape_Custom", false) > 0)
			{
				return false;
			}
			return true;
		case GiftCategoryType.Masks:
		case GiftCategoryType.Boots:
		case GiftCategoryType.Hats_random:
			return !idGift.IsNullOrEmpty() && Storager.getInt(idGift, true) == 0;
		case GiftCategoryType.Capes:
			return !idGift.IsNullOrEmpty() && Storager.getInt(idGift, true) == 0;
		case GiftCategoryType.Stickers:
		{
			TypePackSticker? typePackSticker = idGift.ToEnum<TypePackSticker>();
			return typePackSticker.HasValue && !StickersController.IsBuyPack(typePackSticker.Value);
		}
		case GiftCategoryType.WeaponSkin:
			return !WeaponSkinsManager.IsBoughtSkin(idGift);
		case GiftCategoryType.Gadgets:
			return !GadgetsInfo.IsBought(idGift);
		default:
			return false;
		}
	}

	private static List<string> GetAvailableProducts(ShopNGUIController.CategoryNames category, int maxTier = -1, string[] withoutIds = null)
	{
		if (maxTier < 0)
		{
			maxTier = ExpController.OurTierForAnyPlace();
		}
		List<string> list = Wear.AllWears(category, maxTier, true, true);
		List<string> list2 = PromoActionsGUIController.FilterPurchases(list, true, false);
		if (withoutIds != null)
		{
			list2.AddRange(withoutIds);
		}
		return list.Except(list2).ToList();
	}

	public GiftInfo GetRandomGift()
	{
		if (_availableGifts == null || _availableGifts.Count == 0)
		{
			return null;
		}
		if (_availableGiftsPercentSum < 0f)
		{
			return null;
		}
		float num = UnityEngine.Random.Range(0f, _availableGiftsPercentSum);
		float num2 = 0f;
		GiftInfo result = null;
		for (int i = 0; i < _availableGifts.Count; i++)
		{
			GiftInfo giftInfo = _availableGifts[i];
			num2 += giftInfo.PercentAddInSlot;
			if (num2 > num)
			{
				result = giftInfo;
				break;
			}
		}
		return result;
	}

	private List<GiftInfo> GetAvailableGifts()
	{
		List<GiftInfo> res = new List<GiftInfo>();
		foreach (GiftInfo root in _rootGifts)
		{
			if (root.Id == "guns_gray")
			{
				List<string> availableGrayWeaponsTags = GiftController.GetAvailableGrayWeaponsTags();
				availableGrayWeaponsTags.ForEach(delegate(string w)
				{
					res.Add(GiftInfo.CreateInfo(root, w));
				});
			}
			else if (root.Id == "equip_Mask")
			{
				List<string> availableProducts = GetAvailableProducts(ShopNGUIController.CategoryNames.MaskCategory);
				availableProducts.ForEach(delegate(string p)
				{
					res.Add(GiftInfo.CreateInfo(root, p));
				});
			}
			else if (root.Id == "equip_Cape")
			{
				List<string> availableProducts2 = GetAvailableProducts(ShopNGUIController.CategoryNames.CapesCategory, -1, new string[1] { "cape_Custom" });
				availableProducts2.ForEach(delegate(string p)
				{
					res.Add(GiftInfo.CreateInfo(root, p));
				});
			}
			else if (root.Id == "equip_Boots")
			{
				List<string> availableProducts3 = GetAvailableProducts(ShopNGUIController.CategoryNames.BootsCategory);
				availableProducts3.ForEach(delegate(string p)
				{
					res.Add(GiftInfo.CreateInfo(root, p));
				});
			}
			else if (root.Id == "equip_Hat")
			{
				List<string> availableProducts4 = GetAvailableProducts(ShopNGUIController.CategoryNames.HatsCategory);
				availableProducts4.ForEach(delegate(string p)
				{
					res.Add(GiftInfo.CreateInfo(root, p));
				});
			}
			else if (root.Id == "random")
			{
				List<WeaponSkin> availableForBuySkins = WeaponSkinsManager.GetAvailableForBuySkins();
				availableForBuySkins.ForEach(delegate(WeaponSkin s)
				{
					res.Add(GiftInfo.CreateInfo(root, s.Id));
				});
			}
			else if (root.Id == "gadget_random")
			{
				IEnumerable<string> enumeration = GadgetsInfo.AvailableForBuyGadgets(ExpController.OurTierForAnyPlace());
				enumeration.ForEach(delegate(string g)
				{
					res.Add(GiftInfo.CreateInfo(root, g));
				});
			}
			else
			{
				res.Add(root);
			}
		}
		return res;
	}
}

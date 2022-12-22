using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public static class ItemDb
{
	public enum ItemRarity
	{
		Common,
		Uncommon,
		Rare,
		Epic,
		Legendary,
		Mythic
	}

	public const int CrystalCrossbowCoinsPrice = 150;

	private static string[] ItemRarityLocalizeKeys;

	private static string[] ItemRarityBBCodes;

	private static List<ItemRecord> _records;

	private static Dictionary<int, ItemRecord> _recordsById;

	private static Dictionary<string, ItemRecord> _recordsByTag;

	private static Dictionary<string, ItemRecord> _recordsByStorageId;

	private static Dictionary<string, ItemRecord> _recordsByShopId;

	private static Dictionary<string, ItemRecord> _recordsByPrefabName;

	public static Dictionary<string, ItemRecord> RecordsByTag
	{
		get
		{
			return _recordsByTag;
		}
	}

	public static List<ItemRecord> allRecords
	{
		get
		{
			return _records;
		}
	}

	public static Dictionary<string, ItemRecord> allRecordsWithStorageIds
	{
		get
		{
			return _recordsByStorageId;
		}
	}

	static ItemDb()
	{
		ItemRarityLocalizeKeys = new string[6] { "Key_2394", "Key_2395", "Key_2396", "Key_2397", "Key_2398", "Key_2399" };
		ItemRarityBBCodes = new string[6] { "[E3E3E3]", "[8DFF03]", "[03CDFF]", "[FFDD03]", "[FF5D5D]", "[C602CD]" };
		_records = ItemDbRecords.GetRecords();
		_recordsById = new Dictionary<int, ItemRecord>(_records.Count);
		_recordsByTag = new Dictionary<string, ItemRecord>(_records.Count);
		_recordsByStorageId = new Dictionary<string, ItemRecord>(_records.Count);
		_recordsByShopId = new Dictionary<string, ItemRecord>(_records.Count);
		_recordsByPrefabName = new Dictionary<string, ItemRecord>(_records.Count);
		for (int i = 0; i < _records.Count; i++)
		{
			ItemRecord itemRecord = _records[i];
			_recordsById[itemRecord.Id] = itemRecord;
			if (!string.IsNullOrEmpty(itemRecord.Tag))
			{
				_recordsByTag[itemRecord.Tag] = itemRecord;
			}
			if (!string.IsNullOrEmpty(itemRecord.StorageId))
			{
				_recordsByStorageId[itemRecord.StorageId] = itemRecord;
			}
			if (!string.IsNullOrEmpty(itemRecord.ShopId))
			{
				_recordsByShopId[itemRecord.ShopId] = itemRecord;
			}
			if (!string.IsNullOrEmpty(itemRecord.PrefabName))
			{
				_recordsByPrefabName[itemRecord.PrefabName] = itemRecord;
			}
		}
	}

	public static string GetItemRarityLocalizeName(ItemRarity _itemRarity)
	{
		return ItemRarityBBCodes[(int)_itemRarity] + LocalizationStore.Get(ItemRarityLocalizeKeys[(int)_itemRarity]);
	}

	public static ItemRecord GetById(int id)
	{
		if (_recordsById.ContainsKey(id))
		{
			return _recordsById[id];
		}
		return null;
	}

	public static ItemRecord GetByTag(string tag)
	{
		if (tag == null)
		{
			return null;
		}
		ItemRecord value;
		if (_recordsByTag.TryGetValue(tag, out value))
		{
			return value;
		}
		return null;
	}

	public static ItemRecord GetByPrefabName(string prefabName)
	{
		if (prefabName == null)
		{
			return null;
		}
		ItemRecord value;
		if (_recordsByPrefabName.TryGetValue(prefabName, out value))
		{
			return value;
		}
		return null;
	}

	public static ItemRecord GetByShopId(string shopId)
	{
		if (shopId == null)
		{
			return null;
		}
		ItemRecord value;
		if (_recordsByShopId.TryGetValue(shopId, out value))
		{
			return value;
		}
		return null;
	}

	public static ItemPrice GetPriceByShopId(string shopId, ShopNGUIController.CategoryNames category)
	{
		if (shopId == null)
		{
			return null;
		}
		if (BalanceController.pricesFromServer.ContainsKey(shopId))
		{
			return BalanceController.pricesFromServer[shopId];
		}
		if (category == ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory)
		{
			WeaponSkin skin = WeaponSkinsManager.GetSkin(shopId);
			if (skin != null)
			{
				return new ItemPrice(skin.Price, (skin.Currency != VirtualCurrencyBonusType.Gem) ? "Coins" : "GemsCurrency");
			}
			return new ItemPrice(0, "Coins");
		}
		ItemRecord value;
		if (_recordsByShopId.TryGetValue(shopId, out value))
		{
			return value.Price;
		}
		return VirtualCurrencyHelper.Price(shopId);
	}

	public static bool IsCanBuy(string tag)
	{
		ItemRecord byTag = GetByTag(tag);
		if (byTag != null)
		{
			return byTag.CanBuy;
		}
		return false;
	}

	public static string GetShopIdByTag(string tag)
	{
		ItemRecord byTag = GetByTag(tag);
		return (byTag == null) ? null : byTag.ShopId;
	}

	public static string GetTagByShopId(string shopId)
	{
		ItemRecord byShopId = GetByShopId(shopId);
		return (byShopId == null) ? null : byShopId.Tag;
	}

	public static string GetStorageIdByTag(string tag)
	{
		ItemRecord byTag = GetByTag(tag);
		return (byTag == null) ? null : byTag.StorageId;
	}

	public static string GetStorageIdByShopId(string shopId)
	{
		ItemRecord byShopId = GetByShopId(shopId);
		return (byShopId == null) ? null : byShopId.StorageId;
	}

	public static IEnumerable<ItemRecord> GetCanBuyWeapon(bool includeDeactivated = false)
	{
		if (includeDeactivated)
		{
			return _records.Where((ItemRecord item) => item.CanBuy);
		}
		return _records.Where((ItemRecord item) => item.CanBuy && !item.Deactivated);
	}

	public static string[] GetCanBuyWeaponTags(bool includeDeactivated = false)
	{
		return (from item in GetCanBuyWeapon(includeDeactivated)
			select item.Tag).ToArray();
	}

	public static List<string> GetCanBuyWeaponStorageIds(bool includeDeactivated = false)
	{
		return (from item in GetCanBuyWeapon(includeDeactivated)
			where item.StorageId != null
			select item.StorageId).ToList();
	}

	public static void Fill_tagToStoreIDMapping(Dictionary<string, string> tagToStoreIDMapping)
	{
		tagToStoreIDMapping.Clear();
		foreach (KeyValuePair<string, ItemRecord> item in _recordsByTag)
		{
			if (!string.IsNullOrEmpty(item.Value.ShopId))
			{
				tagToStoreIDMapping[item.Key] = item.Value.ShopId;
			}
		}
	}

	public static void Fill_storeIDtoDefsSNMapping(Dictionary<string, string> storeIDtoDefsSNMapping)
	{
		storeIDtoDefsSNMapping.Clear();
		foreach (KeyValuePair<string, ItemRecord> item in _recordsByShopId)
		{
			if (!string.IsNullOrEmpty(item.Value.StorageId))
			{
				storeIDtoDefsSNMapping[item.Key] = item.Value.StorageId;
			}
		}
	}

	public static bool IsTemporaryGun(string tg)
	{
		if (tg == null)
		{
			return false;
		}
		ItemRecord byTag = GetByTag(tg);
		return byTag != null && byTag.TemporaryGun;
	}

	public static bool IsWeaponCanDrop(string tag)
	{
		if (tag == "Knife")
		{
			return false;
		}
		ItemRecord byTag = GetByTag(tag);
		if (byTag == null)
		{
			return false;
		}
		return !byTag.CanBuy;
	}

	public static void InitStorageForTag(string tag)
	{
		ItemRecord byTag = GetByTag(tag);
		if (byTag == null)
		{
			Debug.LogWarning("item didn't found for tag: " + tag);
		}
		else if (string.IsNullOrEmpty(byTag.StorageId))
		{
			Debug.LogWarning("StoragId is null or empty for tag: " + tag);
		}
		else
		{
			Storager.setInt(byTag.StorageId, 0, false);
		}
	}

	public static int GetItemCategory(string tag)
	{
		int num = -1;
		ItemRecord byTag = GetByTag(tag);
		if (byTag != null)
		{
			WeaponSounds weaponSounds = Resources.Load<WeaponSounds>(string.Format("Weapons/{0}", byTag.PrefabName));
			return (!(weaponSounds != null)) ? (-1) : (weaponSounds.categoryNabor - 1);
		}
		if (num == -1)
		{
			GadgetInfo value = null;
			if (GadgetsInfo.info.TryGetValue(tag, out value) && value != null)
			{
				num = (int)value.Category;
			}
		}
		if (num == -1)
		{
			ShopNGUIController.CategoryNames categoryNames = Wear.wear.Keys.FirstOrDefault((ShopNGUIController.CategoryNames wearCategory) => Wear.wear[wearCategory].FirstOrDefault((List<string> l) => l.Contains(tag)) != null);
			if (ShopNGUIController.IsWearCategory(categoryNames))
			{
				num = (int)categoryNames;
			}
		}
		if (num == -1 && (SkinsController.skinsNamesForPers.ContainsKey(tag) || tag.Equals("CustomSkinID")))
		{
			num = 8;
		}
		if (num == -1 && WeaponSkinsManager.SkinIds.Contains(tag))
		{
			num = 2000;
		}
		if (num == -1)
		{
			try
			{
				if ((from eggData in Singleton<EggsManager>.Instance.GetAllEggs()
					select eggData.Id).Contains(tag))
				{
					num = 30000;
				}
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in checking is eggs category: {0}", ex);
			}
		}
		if (num == -1)
		{
			try
			{
				if (PetsInfo.info.ContainsKey(tag))
				{
					num = 25000;
				}
			}
			catch (Exception ex2)
			{
				Debug.LogErrorFormat("Exception in checking is pets category: {0}", ex2);
			}
		}
		if (num == -1 && GearManager.IsItemGear(tag))
		{
			num = 11;
		}
		return num;
	}

	public static int[] GetItemFilterMap(string tag)
	{
		ItemRecord byTag = GetByTag(tag);
		if (byTag != null)
		{
			WeaponSounds weaponSounds = Resources.Load<WeaponSounds>("Weapons/" + byTag.PrefabName);
			return (!(weaponSounds != null)) ? new int[0] : ((weaponSounds.filterMap == null) ? new int[0] : weaponSounds.filterMap);
		}
		return new int[0];
	}

	public static ShopPositionParams GetInfoForNonWeaponItem(string name, ShopNGUIController.CategoryNames category)
	{
		ShopPositionParams result = null;
		if (category == ShopNGUIController.CategoryNames.ArmorCategory)
		{
			result = Resources.Load<ShopPositionParams>("Armor_Info/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.HatsCategory)
		{
			result = Resources.Load<ShopPositionParams>("Hats_Info/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.CapesCategory)
		{
			result = Resources.Load<ShopPositionParams>("Capes_Info/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.BootsCategory)
		{
			result = Resources.Load<ShopPositionParams>("Shop_Boots_Info/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.GearCategory)
		{
			result = Resources.Load<ShopPositionParams>("Shop_Gear/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.MaskCategory)
		{
			result = Resources.Load<ShopPositionParams>("Masks_Info/" + name);
		}
		return result;
	}

	public static GameObject GetWearFromResources(string name, ShopNGUIController.CategoryNames category)
	{
		GameObject result = null;
		if (category == ShopNGUIController.CategoryNames.ArmorCategory)
		{
			result = Resources.Load<GameObject>("Armor/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.HatsCategory)
		{
			result = Resources.Load<GameObject>("Hats/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.CapesCategory)
		{
			result = Resources.Load<GameObject>("Capes/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.BootsCategory)
		{
			result = Resources.Load<GameObject>("Shop_Boots/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.GearCategory)
		{
			result = Resources.Load<GameObject>("Shop_Gear/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.MaskCategory)
		{
			result = Resources.Load<GameObject>("Masks/" + name);
		}
		return result;
	}

	public static ResourceRequest GetWearFromResourcesAsync(string name, ShopNGUIController.CategoryNames category)
	{
		ResourceRequest result = null;
		if (category == ShopNGUIController.CategoryNames.ArmorCategory)
		{
			result = Resources.LoadAsync<GameObject>("Armor/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.HatsCategory)
		{
			result = Resources.LoadAsync<GameObject>("Hats/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.CapesCategory)
		{
			result = Resources.LoadAsync<GameObject>("Capes/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.BootsCategory)
		{
			result = Resources.LoadAsync<GameObject>("Shop_Boots/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.GearCategory)
		{
			result = Resources.LoadAsync<GameObject>("Shop_Gear/" + name);
		}
		if (category == ShopNGUIController.CategoryNames.MaskCategory)
		{
			result = Resources.LoadAsync<GameObject>("Masks/" + name);
		}
		return result;
	}

	public static string GetItemName(string tag, ShopNGUIController.CategoryNames category)
	{
		//Discarded unreachable code: IL_00a9
		if (string.IsNullOrEmpty(tag))
		{
			return string.Empty;
		}
		if (category == ShopNGUIController.CategoryNames.PetsCategory)
		{
			PlayerPet playerPet = Singleton<PetsManager>.Instance.GetPlayerPet(tag);
			if (playerPet != null)
			{
				return playerPet.PetName;
			}
			return string.Empty;
		}
		if (ShopNGUIController.IsGadgetsCategory(category))
		{
			List<string> list = GadgetsInfo.Upgrades[tag];
			GadgetInfo value;
			if (list != null && GadgetsInfo.info.TryGetValue(list[0], out value))
			{
				return LocalizationStore.Get(value.Lkey);
			}
			return string.Empty;
		}
		if (category == ShopNGUIController.CategoryNames.EggsCategory)
		{
			try
			{
				return LocalizationStore.Get(Singleton<EggsManager>.Instance.GetEggData(tag).LKey);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in getting egg localized name: {0}", ex);
				return tag;
			}
		}
		ShopPositionParams shopPositionParams = null;
		ItemRecord byTag = GetByTag(tag);
		if (byTag != null)
		{
			WeaponSounds weaponSounds = Resources.Load<WeaponSounds>("Weapons/" + byTag.PrefabName);
			string empty = string.Empty;
			if (weaponSounds != null)
			{
				try
				{
					ItemRecord byPrefabName = GetByPrefabName(weaponSounds.name.Replace("(Clone)", string.Empty));
					string tag2 = WeaponUpgrades.TagOfFirstUpgrade(byPrefabName.Tag);
					ItemRecord byTag2 = GetByTag(tag2);
					WeaponSounds weaponSounds2 = Resources.Load<WeaponSounds>(string.Format("Weapons/{0}", byTag2.PrefabName));
					return weaponSounds2.shopName;
				}
				catch (Exception ex2)
				{
					Debug.LogError("Error in getting shop name of first upgrade: " + ex2);
					return weaponSounds.shopName;
				}
			}
			return empty;
		}
		shopPositionParams = GetInfoForNonWeaponItem(tag, category);
		if (shopPositionParams != null)
		{
			return shopPositionParams.shopName;
		}
		return string.Empty;
	}

	public static string GetItemNameNonLocalized(string itemId, string shopId, ShopNGUIController.CategoryNames category, string defaultDesc = null)
	{
		//Discarded unreachable code: IL_013e
		ItemRecord byTag = GetByTag(itemId);
		if (byTag != null)
		{
			WeaponSounds weaponSounds = Resources.Load<WeaponSounds>("Weapons/" + byTag.PrefabName);
			return (!(weaponSounds != null)) ? string.Empty : weaponSounds.shopNameNonLocalized;
		}
		switch (category)
		{
		case ShopNGUIController.CategoryNames.ArmorCategory:
			return Resources.Load<ShopPositionParams>("Armor_Info/" + itemId).shopNameNonLocalized;
		case ShopNGUIController.CategoryNames.HatsCategory:
		{
			ShopPositionParams shopPositionParams = Resources.Load<ShopPositionParams>("Hats_Info/" + itemId);
			return (!(shopPositionParams != null)) ? string.Empty : shopPositionParams.shopNameNonLocalized;
		}
		case ShopNGUIController.CategoryNames.SkinsCategory:
			return (!SkinsController.shopKeyFromNameSkin.ContainsKey(shopId)) ? shopId : SkinsController.shopKeyFromNameSkin[shopId];
		case ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory:
		{
			WeaponSkin skin = WeaponSkinsManager.GetSkin(itemId);
			return (skin == null || skin.Lkey == null) ? itemId : LocalizationStore.GetByDefault(skin.Lkey).ToUpper();
		}
		case ShopNGUIController.CategoryNames.EggsCategory:
			try
			{
				EggData eggData = Singleton<EggsManager>.Instance.GetEggData(itemId);
				return (eggData == null || eggData.LKey == null) ? itemId : LocalizationStore.GetByDefault(eggData.LKey).ToUpper();
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in getting egg non localized name: {0}", ex);
			}
			break;
		}
		if (category == ShopNGUIController.CategoryNames.PetsCategory)
		{
			return itemId;
		}
		if (ShopNGUIController.IsGadgetsCategory(category))
		{
			return itemId;
		}
		if (InAppData.inappReadableNames.ContainsKey(shopId))
		{
			return InAppData.inappReadableNames[shopId];
		}
		return defaultDesc ?? shopId;
	}

	public static Texture GetItemIcon(string tag, ShopNGUIController.CategoryNames category, int? upgradeNumForWeapons = null, bool useWeaponSkins = true)
	{
		if (category == (ShopNGUIController.CategoryNames)(-1))
		{
			return null;
		}
		string text = null;
		string text2 = tag;
		if (ShopNGUIController.IsWeaponCategory(category))
		{
			foreach (List<string> upgrade in WeaponUpgrades.upgrades)
			{
				int num = upgrade.IndexOf(tag);
				if (num != -1)
				{
					text2 = upgrade[0];
					if (!upgradeNumForWeapons.HasValue)
					{
						upgradeNumForWeapons = 1 + num;
					}
					break;
				}
			}
		}
		int num2 = 1;
		if (ShopNGUIController.IsWeaponCategory(category))
		{
			ItemRecord byTag = GetByTag(tag);
			if (!byTag.UseImagesFromFirstUpgrade && !byTag.TemporaryGun)
			{
				num2 = ((!upgradeNumForWeapons.HasValue || !upgradeNumForWeapons.HasValue) ? 1 : upgradeNumForWeapons.Value);
			}
		}
		try
		{
			if (useWeaponSkins && ShopNGUIController.IsWeaponCategory(category))
			{
				string settedSkinIdByWeaponTag = WeaponSkinsManager.GetSettedSkinIdByWeaponTag(text2);
				if (!string.IsNullOrEmpty(settedSkinIdByWeaponTag))
				{
					text2 = settedSkinIdByWeaponTag;
					num2 = 1;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in getting weapon skin id in GetItemIcon: " + ex);
		}
		if (ShopNGUIController.IsGadgetsCategory(category))
		{
			text2 = GadgetsInfo.BaseName(tag);
		}
		if (category == ShopNGUIController.CategoryNames.PetsCategory)
		{
			text2 = (PetsManager.IsEmptySlotId(tag) ? tag : Singleton<PetsManager>.Instance.GetFirstUpgrade(tag).Id);
		}
		text = text2 + "_icon" + num2 + "_big";
		if (!string.IsNullOrEmpty(text))
		{
			return Resources.Load<Texture>("OfferIcons/" + text);
		}
		return null;
	}

	public static Texture GetTextureItemByTag(string tag, int? upgradeNum = null)
	{
		int itemCategory = GetItemCategory(tag);
		return GetItemIcon(tag, (ShopNGUIController.CategoryNames)itemCategory, upgradeNum);
	}

	public static bool IsItemInInventory(string tag)
	{
		string key = tag;
		string storageIdByTag = GetStorageIdByTag(tag);
		if (!string.IsNullOrEmpty(storageIdByTag))
		{
			key = storageIdByTag;
		}
		if (!Storager.hasKey(key))
		{
			return false;
		}
		return Storager.getInt(key, true) == 1;
	}

	public static bool HasWeaponNeedUpgradesForBuyNext(string tag)
	{
		foreach (List<string> upgrade in WeaponUpgrades.upgrades)
		{
			int num = upgrade.IndexOf(tag);
			if (num != -1)
			{
				bool flag = true;
				for (int i = 0; i < num; i++)
				{
					flag = flag && IsItemInInventory(upgrade[i]);
				}
				return flag;
			}
		}
		return true;
	}

	public static string GetItemNameByTag(string tag)
	{
		int itemCategory = GetItemCategory(tag);
		return GetItemName(tag, (ShopNGUIController.CategoryNames)itemCategory);
	}

	public static WeaponSounds GetWeaponInfoByPrefabName(string prefabName)
	{
		if (prefabName != null)
		{
			WeaponSounds weaponSounds = Resources.Load<WeaponSounds>("Weapons/" + prefabName);
			return (!(weaponSounds != null)) ? null : weaponSounds;
		}
		return null;
	}

	public static WeaponSounds GetWeaponInfo(string weaponTag)
	{
		ItemRecord byTag = GetByTag(weaponTag);
		if (byTag == null)
		{
			return null;
		}
		return GetWeaponInfoByPrefabName(byTag.PrefabName);
	}

	public static Texture GetTextureForShopItem(string itemTag)
	{
		int value = 0;
		string text = itemTag;
		bool flag = GearManager.IsItemGear(itemTag);
		if (flag)
		{
			text = GearManager.HolderQuantityForID(itemTag);
			value = GearManager.CurrentNumberOfUphradesForGear(text) + 1;
		}
		if (flag && (text == GearManager.Turret || text == GearManager.Mech))
		{
			int? upgradeNum = value;
			if (upgradeNum.HasValue && upgradeNum.Value > 0)
			{
				return GetTextureItemByTag(text, upgradeNum);
			}
		}
		return GetTextureItemByTag(text);
	}
}

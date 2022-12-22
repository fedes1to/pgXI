using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public sealed class VirtualCurrencyHelper
{
	private static int[] _coinInappsQuantityDefault;

	public static int[] _coinInappsQuantityStaticBank;

	public static string[] coinInappsLocalizationKeys;

	private static int[] _coinInappsQuantity;

	private static int[] _gemsInappsQuantityDefault;

	public static int[] _gemsInappsQuantityStaticBank;

	public static string[] gemsInappsLocalizationKeys;

	private static int[] _gemsInappsQuantity;

	public static readonly int[] coinPriceIds;

	public static readonly int[] gemsPriceIds;

	public static readonly int[] starterPackFakePrice;

	private static Dictionary<string, SaltedInt> prices;

	private static System.Random _prng;

	private static WeakReference _referencePricesInUsd;

	private static Dictionary<string, ItemPrice> _armorPricesDefault;

	public static int[] coinInappsQuantity
	{
		get
		{
			return _coinInappsQuantity;
		}
	}

	public static int[] gemsInappsQuantity
	{
		get
		{
			return _gemsInappsQuantity;
		}
	}

	internal static Dictionary<string, decimal> ReferencePricesInUsd
	{
		get
		{
			if (_referencePricesInUsd != null && _referencePricesInUsd.IsAlive)
			{
				return (Dictionary<string, decimal>)_referencePricesInUsd.Target;
			}
			Dictionary<string, decimal> dictionary = InitializeReferencePrices();
			_referencePricesInUsd = new WeakReference(dictionary, false);
			return dictionary;
		}
	}

	static VirtualCurrencyHelper()
	{
		_coinInappsQuantityDefault = new int[7] { 15, 45, 80, 165, 335, 865, 2000 };
		_coinInappsQuantityStaticBank = new int[7] { 15, 50, 90, 185, 390, 1050, 2250 };
		coinInappsLocalizationKeys = new string[7] { "Key_2106", "Key_2107", "Key_2108", "Key_2109", "Key_2110", "Key_2111", "Key_2112" };
		_coinInappsQuantity = InitCoinInappsQuantity(_coinInappsQuantityDefault);
		_gemsInappsQuantityDefault = new int[7] { 9, 27, 48, 100, 200, 517, 1200 };
		_gemsInappsQuantityStaticBank = new int[7] { 9, 30, 60, 120, 260, 700, 1500 };
		gemsInappsLocalizationKeys = new string[7] { "Key_2113", "Key_2114", "Key_2115", "Key_2116", "Key_2117", "Key_2118", "Key_2119" };
		_gemsInappsQuantity = InitGemsInappsQuantity(_gemsInappsQuantityDefault);
		coinPriceIds = new int[7] { 1, 3, 5, 10, 20, 50, 100 };
		gemsPriceIds = new int[7] { 1, 3, 5, 10, 20, 50, 100 };
		starterPackFakePrice = new int[8] { 6, 5, 4, 3, 2, 1, 1, 1 };
		prices = new Dictionary<string, SaltedInt>();
		_prng = new System.Random(4103);
		_armorPricesDefault = new Dictionary<string, ItemPrice>();
		AddPrice(PremiumAccountController.AccountType.OneDay.ToString(), 5);
		AddPrice(PremiumAccountController.AccountType.ThreeDay.ToString(), 10);
		AddPrice(PremiumAccountController.AccountType.SevenDays.ToString(), 20);
		AddPrice(PremiumAccountController.AccountType.Month.ToString(), 60);
		AddPrice("crystalsword", 185);
		AddPrice("Fullhealth", 15);
		AddPrice("bigammopack", 15);
		AddPrice("MinerWeapon", 35);
		AddPrice(StoreKitEventListener.elixirID, 15);
		AddPrice(StoreKitEventListener.armor, 10);
		AddPrice(StoreKitEventListener.armor2, 15);
		AddPrice(StoreKitEventListener.armor3, 20);
		AddPrice("CustomSkinID", Defs.skinsMakerPrice);
		AddPrice("cape_Archimage", 60);
		AddPrice("cape_BloodyDemon", 60);
		AddPrice("cape_RoyalKnight", 60);
		AddPrice("cape_SkeletonLord", 60);
		AddPrice("cape_EliteCrafter", 60);
		AddPrice("cape_Custom", 75);
		AddPrice("HitmanCape_Up1", 30);
		AddPrice("BerserkCape_Up1", 30);
		AddPrice("DemolitionCape_Up1", 30);
		AddPrice("SniperCape_Up1", 30);
		AddPrice("StormTrooperCape_Up1", 30);
		AddPrice("HitmanCape_Up2", 45);
		AddPrice("BerserkCape_Up2", 45);
		AddPrice("DemolitionCape_Up2", 45);
		AddPrice("SniperCape_Up2", 45);
		AddPrice("StormTrooperCape_Up2", 45);
		AddPrice("cape_Engineer", 60);
		AddPrice("cape_Engineer_Up1", 30);
		AddPrice("cape_Engineer_Up2", 45);
		AddPrice("hat_DiamondHelmet", 65);
		AddPrice("hat_Adamant_3", 3);
		AddPrice("hat_Headphones", 50);
		AddPrice("hat_ManiacMask", 65);
		AddPrice("hat_KingsCrown", 150);
		AddPrice("hat_SeriousManHat", 50);
		AddPrice("hat_Samurai", 95);
		AddPrice("league1_hat_hitman", 75);
		AddPrice("league2_hat_cowboyhat", 100);
		AddPrice("league3_hat_afro", 150);
		AddPrice("league4_hat_mushroom", 200);
		AddPrice("league5_hat_brain", 250);
		AddPrice("league6_hat_tiara", 300);
		AddPrice("hat_AlmazHelmet", 95);
		AddPrice("hat_ArmyHelmet", 95);
		AddPrice("hat_SteelHelmet", 95);
		AddPrice("hat_GoldHelmet", 95);
		AddPrice("hat_Army_1", 70);
		AddPrice("hat_Army_2", 70);
		AddPrice("hat_Army_3", 70);
		AddPrice("hat_Army_4", 70);
		AddPrice("hat_Steel_1", 85);
		AddPrice("hat_Steel_2", 85);
		AddPrice("hat_Steel_3", 85);
		AddPrice("hat_Steel_4", 85);
		AddPrice("hat_Royal_1", 100);
		AddPrice("hat_Royal_2", 100);
		AddPrice("hat_Royal_3", 100);
		AddPrice("hat_Royal_4", 100);
		AddPrice("hat_Almaz_1", 120);
		AddPrice("hat_Almaz_2", 120);
		AddPrice("hat_Almaz_3", 120);
		AddPrice("hat_Almaz_4", 120);
		AddPrice(PotionsController.HastePotion, 2);
		AddPrice(PotionsController.MightPotion, 2);
		AddPrice(PotionsController.RegenerationPotion, 5);
		AddPrice(GearManager.UpgradeIDForGear("InvisibilityPotion", 1), 1);
		AddPrice("InvisibilityPotion" + GearManager.UpgradeSuffix + 2, 1);
		AddPrice("InvisibilityPotion" + GearManager.UpgradeSuffix + 3, 1);
		AddPrice("InvisibilityPotion" + GearManager.UpgradeSuffix + 4, 1);
		AddPrice("InvisibilityPotion" + GearManager.UpgradeSuffix + 5, 1);
		AddPrice("GrenadeID" + GearManager.UpgradeSuffix + 1, 1);
		AddPrice("GrenadeID" + GearManager.UpgradeSuffix + 2, 1);
		AddPrice("GrenadeID" + GearManager.UpgradeSuffix + 3, 1);
		AddPrice("GrenadeID" + GearManager.UpgradeSuffix + 4, 1);
		AddPrice("GrenadeID" + GearManager.UpgradeSuffix + 5, 1);
		AddPrice(GearManager.Turret + GearManager.UpgradeSuffix + 1, 1);
		AddPrice(GearManager.Turret + GearManager.UpgradeSuffix + 2, 1);
		AddPrice(GearManager.Turret + GearManager.UpgradeSuffix + 3, 1);
		AddPrice(GearManager.Turret + GearManager.UpgradeSuffix + 4, 1);
		AddPrice(GearManager.Turret + GearManager.UpgradeSuffix + 5, 1);
		AddPrice(GearManager.Mech + GearManager.UpgradeSuffix + 1, 1);
		AddPrice(GearManager.Mech + GearManager.UpgradeSuffix + 2, 1);
		AddPrice(GearManager.Mech + GearManager.UpgradeSuffix + 3, 1);
		AddPrice(GearManager.Mech + GearManager.UpgradeSuffix + 4, 1);
		AddPrice(GearManager.Mech + GearManager.UpgradeSuffix + 5, 1);
		AddPrice(GearManager.Jetpack + GearManager.UpgradeSuffix + 1, 1);
		AddPrice(GearManager.Jetpack + GearManager.UpgradeSuffix + 2, 1);
		AddPrice(GearManager.Jetpack + GearManager.UpgradeSuffix + 3, 1);
		AddPrice(GearManager.Jetpack + GearManager.UpgradeSuffix + 4, 1);
		AddPrice(GearManager.Jetpack + GearManager.UpgradeSuffix + 5, 1);
		AddPrice(GearManager.Wings + GearManager.OneItemSuffix + 0, 3);
		AddPrice(GearManager.Bear + GearManager.OneItemSuffix + 0, 2);
		AddPrice(GearManager.BigHeadPotion + GearManager.OneItemSuffix + 0, 1);
		AddPrice(GearManager.MusicBox + GearManager.OneItemSuffix + 0, 5);
		AddPrice(GearManager.Like + GearManager.OneItemSuffix + 0, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 0, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 1, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 2, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 3, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 4, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 5, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 0, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 1, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 2, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 3, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 4, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 5, 3);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 0, 5);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 1, 5);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 2, 5);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 3, 5);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 4, 5);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 5, 5);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 0, 7);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 1, 7);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 2, 7);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 3, 7);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 4, 7);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 5, 7);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 0, 4);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 1, 4);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 2, 4);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 3, 4);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 4, 4);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 5, 4);
		AddPrice("boots_red", 50);
		AddPrice("boots_gray", 50);
		AddPrice("boots_blue", 50);
		AddPrice("boots_green", 50);
		AddPrice("boots_black", 50);
		AddPrice("boots_tabi", 120);
		AddPrice("HitmanBoots_Up1", 25);
		AddPrice("StormTrooperBoots_Up1", 25);
		AddPrice("SniperBoots_Up1", 25);
		AddPrice("DemolitionBoots_Up1", 25);
		AddPrice("BerserkBoots_Up1", 25);
		AddPrice("HitmanBoots_Up2", 40);
		AddPrice("StormTrooperBoots_Up2", 40);
		AddPrice("SniperBoots_Up2", 40);
		AddPrice("DemolitionBoots_Up2", 40);
		AddPrice("BerserkBoots_Up2", 40);
		AddPrice("mask_sniper", 40);
		AddPrice("mask_sniper_up1", 15);
		AddPrice("mask_sniper_up2", 30);
		AddPrice("mask_demolition", 40);
		AddPrice("mask_demolition_up1", 15);
		AddPrice("mask_demolition_up2", 30);
		AddPrice("mask_hitman_1", 40);
		AddPrice("mask_hitman_1_up1", 15);
		AddPrice("mask_hitman_1_up2", 30);
		AddPrice("mask_berserk", 40);
		AddPrice("mask_berserk_up1", 15);
		AddPrice("mask_berserk_up2", 30);
		AddPrice("mask_trooper", 40);
		AddPrice("mask_trooper_up1", 15);
		AddPrice("mask_trooper_up2", 30);
		AddPrice("mask_engineer", 40);
		AddPrice("mask_engineer_up1", 15);
		AddPrice("mask_engineer_up2", 30);
		AddPrice("EngineerBoots", 50);
		AddPrice("EngineerBoots_Up1", 25);
		AddPrice("EngineerBoots_Up2", 40);
		AddPriceForArmor("Armor_Army_1", 70);
		AddPriceForArmor("Armor_Army_2", 70);
		AddPriceForArmor("Armor_Army_3", 70);
		AddPriceForArmor("Armor_Steel_1", 85);
		AddPriceForArmor("Armor_Steel_2", 85);
		AddPriceForArmor("Armor_Steel_3", 85);
		AddPriceForArmor("Armor_Royal_1", 100);
		AddPriceForArmor("Armor_Royal_2", 100);
		AddPriceForArmor("Armor_Royal_3", 100);
		AddPriceForArmor("Armor_Almaz_1", 120);
		AddPriceForArmor("Armor_Almaz_2", 120);
		AddPriceForArmor("Armor_Almaz_3", 120);
		AddPriceForArmor("Armor_Novice", 10);
		AddPriceForArmor("Armor_Rubin_1", 135);
		AddPriceForArmor("Armor_Rubin_2", 135);
		AddPriceForArmor("Armor_Rubin_3", 135);
		AddPriceForArmor("Armor_Adamant_Const_1", 170);
		AddPriceForArmor("Armor_Adamant_Const_2", 170);
		AddPriceForArmor("Armor_Adamant_Const_3", 170);
		AddPriceForArmor("Armor_Army_4", 120);
		AddPriceForArmor("Armor_Steel_4", 120);
		AddPriceForArmor("Armor_Royal_4", 135);
		AddPriceForArmor("Armor_Almaz_4", 120);
		AddPriceForArmor("Armor_Adamant_3", 3);
		AddPrice("hat_Rubin_1", 135);
		AddPrice("hat_Rubin_2", 135);
		AddPrice("hat_Rubin_3", 135);
		AddPrice("hat_Adamant_Const_1", 170);
		AddPrice("hat_Adamant_Const_2", 170);
		AddPrice("hat_Adamant_Const_3", 170);
		AddPrice(StickersController.KeyForBuyPack(TypePackSticker.classic), 20);
		AddPrice(StickersController.KeyForBuyPack(TypePackSticker.christmas), 30);
		AddPrice(StickersController.KeyForBuyPack(TypePackSticker.easter), 40);
		AddPrice(Defs.BuyGiftKey, 5);
	}

	public static int[] InitCoinInappsQuantity(int[] _mass)
	{
		int[] array = new int[_mass.Length];
		Array.Copy(_mass, array, _mass.Length);
		return array;
	}

	public static int[] InitGemsInappsQuantity(int[] _mass)
	{
		int[] array = new int[_mass.Length];
		Array.Copy(_mass, array, _mass.Length);
		return array;
	}

	public static void ResetInappsQuantityOnDefault()
	{
		_gemsInappsQuantity = InitGemsInappsQuantity(_gemsInappsQuantityDefault);
		_coinInappsQuantity = InitCoinInappsQuantity(_coinInappsQuantityDefault);
	}

	public static void RewriteInappsQuantity(int _priceId, int _coinQuantity, int _gemsQuantity, int _bonusCoins, int _bonusGems)
	{
		for (int i = 0; i < coinPriceIds.Length; i++)
		{
			if (coinPriceIds[i] == _priceId)
			{
				_coinInappsQuantity[i] = _coinQuantity;
				_gemsInappsQuantity[i] = _gemsQuantity;
				AbstractBankView.discountsCoins[i] = _bonusCoins;
				AbstractBankView.discountsGems[i] = _bonusGems;
				break;
			}
		}
	}

	public static ItemPrice Price(string key)
	{
		if (key == null)
		{
			return null;
		}
		if (key == "Eggs.SuperIncubatorId")
		{
			return new ItemPrice(200, "Coins");
		}
		if (PetsInfo.info.Keys.Contains(key))
		{
			return new ItemPrice(200, "Coins");
		}
		if (Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory].SelectMany((List<string> list) => list).ToList().Contains(key))
		{
			if (Defs2.ArmorPricesFromServer != null && Defs2.ArmorPricesFromServer.ContainsKey(key))
			{
				ItemPrice itemPrice = Defs2.ArmorPricesFromServer[key];
				if (itemPrice != null)
				{
					return itemPrice;
				}
				Debug.LogError("armorPriceFromServer == null, armor = " + key);
			}
			return _armorPricesDefault[key];
		}
		if (GadgetsInfo.info.Keys.Contains(key))
		{
			List<ItemPrice> value;
			if (BalanceController.GadgetPricesFromServer != null && BalanceController.GadgetPricesFromServer.TryGetValue(key, out value) && value != null)
			{
				string text = GadgetsInfo.FirstForOurTier(key);
				int num = ((!(text == key)) ? 1 : 0);
				if (value.Count > num && value[num] != null)
				{
					return value[num];
				}
				Debug.LogError("listServerPrices.Count > index && listServerPrices [index] != null: key = " + key);
			}
			return new ItemPrice(200, "Coins");
		}
		if (!prices.ContainsKey(key))
		{
			return null;
		}
		int value2 = prices[key].Value;
		string currency = "Coins";
		string text2 = GearManager.HolderQuantityForID(key);
		bool flag = false;
		flag = text2 != null && (GearManager.Gear.Contains(text2) || GearManager.DaterGear.Contains(text2)) && !key.Contains(GearManager.UpgradeSuffix) && !text2.Equals(GearManager.Grenade);
		switch (key)
		{
		case "cape_Archimage":
		case "cape_BloodyDemon":
		case "cape_RoyalKnight":
		case "cape_SkeletonLord":
		case "cape_EliteCrafter":
		case "HitmanCape_Up1":
		case "BerserkCape_Up1":
		case "DemolitionCape_Up1":
		case "SniperCape_Up1":
		case "StormTrooperCape_Up1":
		case "HitmanCape_Up2":
		case "BerserkCape_Up2":
		case "DemolitionCape_Up2":
		case "SniperCape_Up2":
		case "StormTrooperCape_Up2":
		case "cape_Engineer":
		case "cape_Engineer_Up1":
		case "cape_Engineer_Up2":
			flag = true;
			break;
		}
		switch (key)
		{
		case "boots_red":
		case "boots_gray":
		case "boots_blue":
		case "boots_green":
		case "boots_black":
		case "HitmanBoots_Up1":
		case "StormTrooperBoots_Up1":
		case "SniperBoots_Up1":
		case "DemolitionBoots_Up1":
		case "BerserkBoots_Up1":
		case "HitmanBoots_Up2":
		case "StormTrooperBoots_Up2":
		case "SniperBoots_Up2":
		case "DemolitionBoots_Up2":
		case "BerserkBoots_Up2":
		case "EngineerBoots":
		case "EngineerBoots_Up1":
		case "EngineerBoots_Up2":
			flag = true;
			break;
		}
		IEnumerable<string> source = Wear.wear[ShopNGUIController.CategoryNames.MaskCategory].SelectMany((List<string> l) => l);
		if (key != "hat_ManiacMask" && source.Contains(key))
		{
			flag = true;
		}
		if (key == StickersController.KeyForBuyPack(TypePackSticker.classic))
		{
			flag = true;
		}
		if (key == StickersController.KeyForBuyPack(TypePackSticker.christmas))
		{
			flag = true;
		}
		if (key == StickersController.KeyForBuyPack(TypePackSticker.easter))
		{
			flag = true;
		}
		if (key == Defs.BuyGiftKey)
		{
			flag = true;
		}
		if (TempItemsController.PriceCoefs.ContainsKey(key))
		{
			flag = true;
		}
		if (key != null && (key.Equals(PremiumAccountController.AccountType.OneDay.ToString()) || key.Equals(PremiumAccountController.AccountType.ThreeDay.ToString()) || key.Equals(PremiumAccountController.AccountType.SevenDays.ToString()) || key.Equals(PremiumAccountController.AccountType.Month.ToString())))
		{
			flag = true;
		}
		if (flag)
		{
			currency = "GemsCurrency";
		}
		return new ItemPrice(value2, currency);
	}

	private static int CoefInappsQuantity()
	{
		if (PromoActionsManager.sharedManager != null && PromoActionsManager.sharedManager.IsEventX3Active)
		{
			return 3;
		}
		return 1;
	}

	public static int GetCoinInappsQuantity(int inappIndex)
	{
		if (PromoActionsManager.sharedManager == null)
		{
			Debug.LogError("GetCoinInappsQuantity: PromoActionsManager.sharedManager == null when calculating");
		}
		return CoefInappsQuantity() * coinInappsQuantity[inappIndex];
	}

	public static int GetGemsInappsQuantity(int inappIndex)
	{
		if (PromoActionsManager.sharedManager == null)
		{
			Debug.LogError("GetGemsInappsQuantity: PromoActionsManager.sharedManager == null when calculating");
		}
		return CoefInappsQuantity() * gemsInappsQuantity[inappIndex];
	}

	public static void AddPrice(string key, int value)
	{
		prices.Add(key, new SaltedInt(_prng.Next(), value));
	}

	private static void AddPriceForArmor(string armor, int amount)
	{
		if (armor == null)
		{
			Debug.LogError("AddPriceForArmor armor == null");
		}
		else
		{
			_armorPricesDefault.Add(armor, new ItemPrice(amount, (!(armor == "Armor_Adamant_3")) ? "Coins" : "GemsCurrency"));
		}
	}

	private static Dictionary<string, decimal> InitializeReferencePrices()
	{
		Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
		dictionary.Add("coin1", 0.99m);
		dictionary.Add("coin7", 2.99m);
		dictionary.Add("coin2", 4.99m);
		dictionary.Add("coin4", 19.99m);
		dictionary.Add("coin5", 49.99m);
		dictionary.Add("coin8", 99.99m);
		dictionary.Add("gem1", 0.99m);
		dictionary.Add("gem2", 2.99m);
		dictionary.Add("gem3", 4.99m);
		dictionary.Add("gem4", 9.99m);
		dictionary.Add("gem5", 19.99m);
		dictionary.Add("gem6", 49.99m);
		dictionary.Add("gem7", 99.99m);
		dictionary.Add("starterpack8", 0.99m);
		dictionary.Add("starterpack7", 0.99m);
		dictionary.Add("starterpack5", 1.99m);
		dictionary.Add("starterpack3", 3.99m);
		dictionary.Add("starterpack1", 5.99m);
		Dictionary<string, decimal> dictionary2 = dictionary;
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
		{
			dictionary2.Add("coin3.", 9.99m);
			dictionary2.Add("starterpack6", 0.99m);
			dictionary2.Add("starterpack4", 2.99m);
			dictionary2.Add("starterpack2", 4.99m);
		}
		else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			dictionary2.Add("coin3", 9.99m);
			dictionary2.Add("starterpack10", 2.99m);
			dictionary2.Add("starterpack9", 4.99m);
		}
		return dictionary2;
	}
}

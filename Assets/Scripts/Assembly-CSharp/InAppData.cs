using System.Collections.Generic;
using System.Linq;

public static class InAppData
{
	public static Dictionary<string, string> inappReadableNames;

	static InAppData()
	{
		inappReadableNames = new Dictionary<string, string>();
		inappReadableNames.Add("bigammopack", "Big Pack of Ammo");
		inappReadableNames.Add("Fullhealth", "Full Health");
		inappReadableNames.Add(StoreKitEventListener.elixirID, "Elixir of Resurrection");
		inappReadableNames.Add(StoreKitEventListener.armor, "Armor");
		inappReadableNames.Add(StoreKitEventListener.armor2, "Armor2");
		inappReadableNames.Add(StoreKitEventListener.armor3, "Armor3");
		inappReadableNames.Add("cape_Archimage", "Archimage Cape");
		inappReadableNames.Add("cape_BloodyDemon", "Bloody Demon Cape");
		inappReadableNames.Add("cape_RoyalKnight", "Royal Knight Cape");
		inappReadableNames.Add("cape_SkeletonLord", "Skeleton Lord Cape");
		inappReadableNames.Add("cape_EliteCrafter", "Elite Crafter Cape");
		inappReadableNames.Add("cape_Custom", "Custom Cape");
		inappReadableNames.Add("HitmanCape_Up1", "HitmanCape_Up1");
		inappReadableNames.Add("BerserkCape_Up1", "BerserkCape_Up1");
		inappReadableNames.Add("DemolitionCape_Up1", "DemolitionCape_Up1");
		inappReadableNames.Add("cape_Engineer", "EngineerCape");
		inappReadableNames.Add("cape_Engineer_Up1", "EngineerCape_Up1");
		inappReadableNames.Add("cape_Engineer_Up2", "EngineerCape_Up2");
		inappReadableNames.Add("SniperCape_Up1", "SniperCape_Up1");
		inappReadableNames.Add("StormTrooperCape_Up1", "StormTrooperCape_Up1");
		inappReadableNames.Add("HitmanCape_Up2", "HitmanCape_Up2");
		inappReadableNames.Add("BerserkCape_Up2", "BerserkCape_Up2");
		inappReadableNames.Add("DemolitionCape_Up2", "DemolitionCape_Up2");
		inappReadableNames.Add("SniperCape_Up2", "SniperCape_Up2");
		inappReadableNames.Add("StormTrooperCape_Up2", "StormTrooperCape_Up2");
		inappReadableNames.Add("hat_Adamant_3", "hat_Adamant_3");
		inappReadableNames.Add("hat_DiamondHelmet", "Diamond Helmet");
		inappReadableNames.Add("hat_Headphones", "Headphones");
		inappReadableNames.Add("hat_KingsCrown", "King's Crown");
		inappReadableNames.Add("hat_SeriousManHat", "Leprechaun's Hat");
		inappReadableNames.Add("hat_Samurai", "Samurais Helm");
		inappReadableNames.Add("league1_hat_hitman", "league1_hat_hitman");
		inappReadableNames.Add("league2_hat_cowboyhat", "league2_hat_cowboyhat");
		inappReadableNames.Add("league3_hat_afro", "league3_hat_afro");
		inappReadableNames.Add("league4_hat_mushroom", "league4_hat_mushroom");
		inappReadableNames.Add("league5_hat_brain", "league5_hat_brain");
		inappReadableNames.Add("league6_hat_tiara", "league6_hat_tiara");
		inappReadableNames.Add("hat_AlmazHelmet", "hat_AlmazHelmet");
		inappReadableNames.Add("hat_ArmyHelmet", "hat_ArmyHelmet");
		inappReadableNames.Add("hat_SteelHelmet", "hat_SteelHelmet");
		inappReadableNames.Add("hat_GoldHelmet", "hat_GoldHelmet");
		inappReadableNames.Add("hat_Army_1", "hat_Army_1");
		inappReadableNames.Add("hat_Almaz_1", "hat_Almaz_1");
		inappReadableNames.Add("hat_Steel_1", "hat_Steel_1");
		inappReadableNames.Add("hat_Royal_1", "hat_Royal_1");
		inappReadableNames.Add("hat_Army_2", "hat_Army_2");
		inappReadableNames.Add("hat_Almaz_2", "hat_Almaz_2");
		inappReadableNames.Add("hat_Steel_2", "hat_Steel_2");
		inappReadableNames.Add("hat_Royal_2", "hat_Royal_2");
		inappReadableNames.Add("hat_Army_3", "hat_Army_3");
		inappReadableNames.Add("hat_Almaz_3", "hat_Almaz_3");
		inappReadableNames.Add("hat_Steel_3", "hat_Steel_3");
		inappReadableNames.Add("hat_Royal_3", "hat_Royal_3");
		inappReadableNames.Add("hat_Army_4", "hat_Army_4");
		inappReadableNames.Add("hat_Almaz_4", "hat_Almaz_4");
		inappReadableNames.Add("hat_Steel_4", "hat_Steel_4");
		inappReadableNames.Add("hat_Royal_4", "hat_Royal_4");
		inappReadableNames.Add("hat_Rubin_1", "hat_Rubin_1");
		inappReadableNames.Add("hat_Rubin_2", "hat_Rubin_2");
		inappReadableNames.Add("hat_Rubin_3", "hat_Rubin_3");
		inappReadableNames.Add("Armor_Steel_1", "Armor_Steel_1");
		inappReadableNames.Add("Armor_Steel_2", "Armor_Steel_2");
		inappReadableNames.Add("Armor_Steel_3", "Armor_Steel_3");
		inappReadableNames.Add("Armor_Steel_4", "Armor_Steel_4");
		inappReadableNames.Add("Armor_Royal_1", "Armor_Royal_1");
		inappReadableNames.Add("Armor_Royal_2", "Armor_Royal_2");
		inappReadableNames.Add("Armor_Royal_3", "Armor_Royal_3");
		inappReadableNames.Add("Armor_Royal_4", "Armor_Royal_4");
		inappReadableNames.Add("Armor_Almaz_1", "Armor_Almaz_1");
		inappReadableNames.Add("Armor_Almaz_2", "Armor_Almaz_2");
		inappReadableNames.Add("Armor_Almaz_3", "Armor_Almaz_3");
		inappReadableNames.Add("Armor_Almaz_4", "Armor_Almaz_4");
		inappReadableNames.Add("Armor_Army_1", "Armor_Army_1");
		inappReadableNames.Add("Armor_Army_2", "Armor_Army_2");
		inappReadableNames.Add("Armor_Army_3", "Armor_Army_3");
		inappReadableNames.Add("Armor_Army_4", "Armor_Army_4");
		inappReadableNames.Add("Armor_Novice", "Armor_Novice");
		inappReadableNames.Add("Armor_Rubin_1", "Armor_Rubin_1");
		inappReadableNames.Add("Armor_Rubin_2", "Armor_Rubin_2");
		inappReadableNames.Add("Armor_Rubin_3", "Armor_Rubin_3");
		inappReadableNames.Add("Armor_Adamant_Const_1", "Armor_Adamant_Const_1");
		inappReadableNames.Add("Armor_Adamant_Const_2", "Armor_Adamant_Const_2");
		inappReadableNames.Add("Armor_Adamant_Const_3", "Armor_Adamant_Const_3");
		inappReadableNames.Add("hat_Adamant_Const_1", "hat_Adamant_Const_1");
		inappReadableNames.Add("hat_Adamant_Const_2", "hat_Adamant_Const_2");
		inappReadableNames.Add("hat_Adamant_Const_3", "hat_Adamant_Const_3");
		inappReadableNames.Add("Armor_Adamant_3", "Armor_Adamant_3");
		string[] potions = PotionsController.potions;
		foreach (string text in potions)
		{
			inappReadableNames.Add(text, text);
		}
		inappReadableNames.Add("boots_red", "boots_red");
		inappReadableNames.Add("boots_gray", "boots_gray");
		inappReadableNames.Add("boots_blue", "boots_blue");
		inappReadableNames.Add("boots_green", "boots_green");
		inappReadableNames.Add("boots_black", "boots_black");
		inappReadableNames.Add("boots_tabi", "boots ninja");
		inappReadableNames.Add("HitmanBoots_Up1", "HitmanBoots_Up1");
		inappReadableNames.Add("StormTrooperBoots_Up1", "StormTrooperBoots_Up1");
		inappReadableNames.Add("SniperBoots_Up1", "SniperBoots_Up1");
		inappReadableNames.Add("DemolitionBoots_Up1", "DemolitionBoots_Up1");
		inappReadableNames.Add("BerserkBoots_Up1", "BerserkBoots_Up1");
		inappReadableNames.Add("HitmanBoots_Up2", "HitmanBoots_Up2");
		inappReadableNames.Add("StormTrooperBoots_Up2", "StormTrooperBoots_Up2");
		inappReadableNames.Add("SniperBoots_Up2", "SniperBoots_Up2");
		inappReadableNames.Add("DemolitionBoots_Up2", "DemolitionBoots_Up2");
		inappReadableNames.Add("BerserkBoots_Up2", "BerserkBoots_Up2");
		inappReadableNames.Add("EngineerBoots", "EngineerBoots");
		inappReadableNames.Add("EngineerBoots_Up1", "EngineerBoots_Up1");
		inappReadableNames.Add("EngineerBoots_Up2", "EngineerBoots_Up2");
		foreach (string item in Wear.wear[ShopNGUIController.CategoryNames.MaskCategory].SelectMany((List<string> list) => list))
		{
			if (!(item == "hat_ManiacMask"))
			{
				inappReadableNames.Add(item, item);
			}
		}
		inappReadableNames.Add("hat_ManiacMask", "Maniac Mask");
		inappReadableNames.Add("InvisibilityPotion" + GearManager.UpgradeSuffix + 1, "InvisibilityPotion" + GearManager.UpgradeSuffix + 1);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.UpgradeSuffix + 2, "InvisibilityPotion" + GearManager.UpgradeSuffix + 2);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.UpgradeSuffix + 3, "InvisibilityPotion" + GearManager.UpgradeSuffix + 3);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.UpgradeSuffix + 4, "InvisibilityPotion" + GearManager.UpgradeSuffix + 4);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.UpgradeSuffix + 5, "InvisibilityPotion" + GearManager.UpgradeSuffix + 5);
		inappReadableNames.Add("GrenadeID" + GearManager.UpgradeSuffix + 1, "GrenadeID" + GearManager.UpgradeSuffix + 1);
		inappReadableNames.Add("GrenadeID" + GearManager.UpgradeSuffix + 2, "GrenadeID" + GearManager.UpgradeSuffix + 2);
		inappReadableNames.Add("GrenadeID" + GearManager.UpgradeSuffix + 3, "GrenadeID" + GearManager.UpgradeSuffix + 3);
		inappReadableNames.Add("GrenadeID" + GearManager.UpgradeSuffix + 4, "GrenadeID" + GearManager.UpgradeSuffix + 4);
		inappReadableNames.Add("GrenadeID" + GearManager.UpgradeSuffix + 5, "GrenadeID" + GearManager.UpgradeSuffix + 5);
		inappReadableNames.Add(GearManager.Turret + GearManager.UpgradeSuffix + 1, GearManager.Turret + GearManager.UpgradeSuffix + 1);
		inappReadableNames.Add(GearManager.Turret + GearManager.UpgradeSuffix + 2, GearManager.Turret + GearManager.UpgradeSuffix + 2);
		inappReadableNames.Add(GearManager.Turret + GearManager.UpgradeSuffix + 3, GearManager.Turret + GearManager.UpgradeSuffix + 3);
		inappReadableNames.Add(GearManager.Turret + GearManager.UpgradeSuffix + 4, GearManager.Turret + GearManager.UpgradeSuffix + 4);
		inappReadableNames.Add(GearManager.Turret + GearManager.UpgradeSuffix + 5, GearManager.Turret + GearManager.UpgradeSuffix + 5);
		inappReadableNames.Add(GearManager.Mech + GearManager.UpgradeSuffix + 1, GearManager.Mech + GearManager.UpgradeSuffix + 1);
		inappReadableNames.Add(GearManager.Mech + GearManager.UpgradeSuffix + 2, GearManager.Mech + GearManager.UpgradeSuffix + 2);
		inappReadableNames.Add(GearManager.Mech + GearManager.UpgradeSuffix + 3, GearManager.Mech + GearManager.UpgradeSuffix + 3);
		inappReadableNames.Add(GearManager.Mech + GearManager.UpgradeSuffix + 4, GearManager.Mech + GearManager.UpgradeSuffix + 4);
		inappReadableNames.Add(GearManager.Mech + GearManager.UpgradeSuffix + 5, GearManager.Mech + GearManager.UpgradeSuffix + 5);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.UpgradeSuffix + 1, GearManager.Jetpack + GearManager.UpgradeSuffix + 1);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.UpgradeSuffix + 2, GearManager.Jetpack + GearManager.UpgradeSuffix + 2);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.UpgradeSuffix + 3, GearManager.Jetpack + GearManager.UpgradeSuffix + 3);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.UpgradeSuffix + 4, GearManager.Jetpack + GearManager.UpgradeSuffix + 4);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.UpgradeSuffix + 5, GearManager.Jetpack + GearManager.UpgradeSuffix + 5);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 0, "InvisibilityPotion" + GearManager.OneItemSuffix + 0);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 1, "InvisibilityPotion" + GearManager.OneItemSuffix + 1);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 2, "InvisibilityPotion" + GearManager.OneItemSuffix + 2);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 3, "InvisibilityPotion" + GearManager.OneItemSuffix + 3);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 4, "InvisibilityPotion" + GearManager.OneItemSuffix + 4);
		inappReadableNames.Add("InvisibilityPotion" + GearManager.OneItemSuffix + 5, "InvisibilityPotion" + GearManager.OneItemSuffix + 5);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 0, "GrenadeID" + GearManager.OneItemSuffix + 0);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 1, "GrenadeID" + GearManager.OneItemSuffix + 1);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 2, "GrenadeID" + GearManager.OneItemSuffix + 2);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 3, "GrenadeID" + GearManager.OneItemSuffix + 3);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 4, "GrenadeID" + GearManager.OneItemSuffix + 4);
		inappReadableNames.Add("GrenadeID" + GearManager.OneItemSuffix + 5, "GrenadeID" + GearManager.OneItemSuffix + 5);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 0, GearManager.Turret + GearManager.OneItemSuffix + 0);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 1, GearManager.Turret + GearManager.OneItemSuffix + 1);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 2, GearManager.Turret + GearManager.OneItemSuffix + 2);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 3, GearManager.Turret + GearManager.OneItemSuffix + 3);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 4, GearManager.Turret + GearManager.OneItemSuffix + 4);
		inappReadableNames.Add(GearManager.Turret + GearManager.OneItemSuffix + 5, GearManager.Turret + GearManager.OneItemSuffix + 5);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 0, GearManager.Mech + GearManager.OneItemSuffix + 0);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 1, GearManager.Mech + GearManager.OneItemSuffix + 1);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 2, GearManager.Mech + GearManager.OneItemSuffix + 2);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 3, GearManager.Mech + GearManager.OneItemSuffix + 3);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 4, GearManager.Mech + GearManager.OneItemSuffix + 4);
		inappReadableNames.Add(GearManager.Mech + GearManager.OneItemSuffix + 5, GearManager.Mech + GearManager.OneItemSuffix + 5);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 0, GearManager.Jetpack + GearManager.OneItemSuffix + 0);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 1, GearManager.Jetpack + GearManager.OneItemSuffix + 1);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 2, GearManager.Jetpack + GearManager.OneItemSuffix + 2);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 3, GearManager.Jetpack + GearManager.OneItemSuffix + 3);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 4, GearManager.Jetpack + GearManager.OneItemSuffix + 4);
		inappReadableNames.Add(GearManager.Jetpack + GearManager.OneItemSuffix + 5, GearManager.Jetpack + GearManager.OneItemSuffix + 5);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public static class Wear
{
	public enum LeagueItemState
	{
		Open,
		Closed,
		Purchased
	}

	public const int IndexOfArmorHatsList = 0;

	public const int NumberOfArmorsPerTier = 3;

	public const string BerserkCape = "cape_BloodyDemon";

	public const string DemolitionCape = "cape_RoyalKnight";

	public const string SniperCape = "cape_SkeletonLord";

	public const string HitmanCape = "cape_Archimage";

	public const string StormTrooperCape = "cape_EliteCrafter";

	public const string cape_Custom = "cape_Custom";

	public const string hat_Headphones = "hat_Headphones";

	public const string hat_ManiacMask = "hat_ManiacMask";

	public const string hat_KingsCrown = "hat_KingsCrown";

	public const string hat_Samurai = "hat_Samurai";

	public const string hat_DiamondHelmet = "hat_DiamondHelmet";

	public const string hat_SeriousManHat = "hat_SeriousManHat";

	public const string hat_AlmazHelmet = "hat_AlmazHelmet";

	public const string hat_ArmyHelmet = "hat_ArmyHelmet";

	public const string hat_GoldHelmet = "hat_GoldHelmet";

	public const string hat_SteelHelmet = "hat_SteelHelmet";

	public const string league1_hat_hitman = "league1_hat_hitman";

	public const string league2_hat_cowboyhat = "league2_hat_cowboyhat";

	public const string league3_hat_afro = "league3_hat_afro";

	public const string league4_hat_mushroom = "league4_hat_mushroom";

	public const string league5_hat_brain = "league5_hat_brain";

	public const string league6_hat_tiara = "league6_hat_tiara";

	public const string hat_Army_1 = "hat_Army_1";

	public const string hat_Royal_1 = "hat_Royal_1";

	public const string hat_Almaz_1 = "hat_Almaz_1";

	public const string hat_Steel_1 = "hat_Steel_1";

	public const string hat_Army_2 = "hat_Army_2";

	public const string hat_Royal_2 = "hat_Royal_2";

	public const string hat_Almaz_2 = "hat_Almaz_2";

	public const string hat_Steel_2 = "hat_Steel_2";

	public const string hat_Army_3 = "hat_Army_3";

	public const string hat_Royal_3 = "hat_Royal_3";

	public const string hat_Almaz_3 = "hat_Almaz_3";

	public const string hat_Steel_3 = "hat_Steel_3";

	public const string hat_Army_4 = "hat_Army_4";

	public const string hat_Royal_4 = "hat_Royal_4";

	public const string hat_Almaz_4 = "hat_Almaz_4";

	public const string hat_Steel_4 = "hat_Steel_4";

	public const string hat_Rubin_1 = "hat_Rubin_1";

	public const string hat_Rubin_2 = "hat_Rubin_2";

	public const string hat_Rubin_3 = "hat_Rubin_3";

	public const string BerserkBoots = "boots_black";

	public const string SniperBoots = "boots_blue";

	public const string StormTrooperBoots = "boots_gray";

	public const string DemolitionBoots = "boots_green";

	public const string HitmanBoots = "boots_red";

	public const string boots_tabi = "boots_tabi";

	public const string Armor_Steel = "Armor_Steel_1";

	public const string Armor_Steel_2 = "Armor_Steel_2";

	public const string Armor_Steel_3 = "Armor_Steel_3";

	public const string Armor_Steel_4 = "Armor_Steel_4";

	public const string Armor_Royal_1 = "Armor_Royal_1";

	public const string Armor_Royal_2 = "Armor_Royal_2";

	public const string Armor_Royal_3 = "Armor_Royal_3";

	public const string Armor_Royal_4 = "Armor_Royal_4";

	public const string Armor_Almaz_1 = "Armor_Almaz_1";

	public const string Armor_Almaz_2 = "Armor_Almaz_2";

	public const string Armor_Almaz_3 = "Armor_Almaz_3";

	public const string Armor_Almaz_4 = "Armor_Almaz_4";

	public const string Armor_Army_1 = "Armor_Army_1";

	public const string Armor_Army_2 = "Armor_Army_2";

	public const string Armor_Army_3 = "Armor_Army_3";

	public const string Armor_Army_4 = "Armor_Army_4";

	public const string Armor_Rubin_1 = "Armor_Rubin_1";

	public const string Armor_Rubin_2 = "Armor_Rubin_2";

	public const string Armor_Rubin_3 = "Armor_Rubin_3";

	public const string StormTrooperCape_Up1 = "StormTrooperCape_Up1";

	public const string StormTrooperCape_Up2 = "StormTrooperCape_Up2";

	public const string HitmanCape_Up1 = "HitmanCape_Up1";

	public const string HitmanCape_Up2 = "HitmanCape_Up2";

	public const string BerserkCape_Up1 = "BerserkCape_Up1";

	public const string BerserkCape_Up2 = "BerserkCape_Up2";

	public const string SniperCape_Up1 = "SniperCape_Up1";

	public const string SniperCape_Up2 = "SniperCape_Up2";

	public const string EngineerCape = "cape_Engineer";

	public const string EngineerCape_Up1 = "cape_Engineer_Up1";

	public const string EngineerCape_Up2 = "cape_Engineer_Up2";

	public const string DemolitionCape_Up1 = "DemolitionCape_Up1";

	public const string DemolitionCape_Up2 = "DemolitionCape_Up2";

	public const string hat_Headphones_Up1 = "hat_Headphones_Up1";

	public const string hat_Headphones_Up2 = "hat_Headphones_Up2";

	public const string hat_ManiacMask_Up1 = "hat_ManiacMask_Up1";

	public const string hat_ManiacMask_Up2 = "hat_ManiacMask_Up2";

	public const string hat_KingsCrown_Up1 = "hat_KingsCrown_Up1";

	public const string hat_KingsCrown_Up2 = "hat_KingsCrown_Up2";

	public const string hat_Samurai_Up1 = "hat_Samurai_Up1";

	public const string hat_Samurai_Up2 = "hat_Samurai_Up2";

	public const string hat_DiamondHelmet_Up1 = "hat_DiamondHelmet_Up1";

	public const string hat_DiamondHelmet_Up2 = "hat_DiamondHelmet_Up2";

	public const string hat_SeriousManHat_Up1 = "hat_SeriousManHat_Up1";

	public const string hat_SeriousManHat_Up2 = "hat_SeriousManHat_Up2";

	public const string StormTrooperBoots_Up1 = "StormTrooperBoots_Up1";

	public const string StormTrooperBoots_Up2 = "StormTrooperBoots_Up2";

	public const string HitmanBoots_Up1 = "HitmanBoots_Up1";

	public const string HitmanBoots_Up2 = "HitmanBoots_Up2";

	public const string BerserkBoots_Up1 = "BerserkBoots_Up1";

	public const string BerserkBoots_Up2 = "BerserkBoots_Up2";

	public const string SniperBoots_Up1 = "SniperBoots_Up1";

	public const string SniperBoots_Up2 = "SniperBoots_Up2";

	public const string DemolitionBoots_Up1 = "DemolitionBoots_Up1";

	public const string DemolitionBoots_Up2 = "DemolitionBoots_Up2";

	public const string EngineerBoots = "EngineerBoots";

	public const string EngineerBoots_Up1 = "EngineerBoots_Up1";

	public const string EngineerBoots_Up2 = "EngineerBoots_Up2";

	public const string Armor_Novice = "Armor_Novice";

	public const string Armor_Adamant_3 = "Armor_Adamant_3";

	public const string hat_Adamant_3 = "hat_Adamant_3";

	public const string Armor_Adamant_Const_1 = "Armor_Adamant_Const_1";

	public const string Armor_Adamant_Const_2 = "Armor_Adamant_Const_2";

	public const string Armor_Adamant_Const_3 = "Armor_Adamant_Const_3";

	public const string hat_Adamant_Const_1 = "hat_Adamant_Const_1";

	public const string hat_Adamant_Const_2 = "hat_Adamant_Const_2";

	public const string hat_Adamant_Const_3 = "hat_Adamant_Const_3";

	public const string SniperMask = "mask_sniper";

	public const string SniperMask_Up1 = "mask_sniper_up1";

	public const string SniperMask_Up2 = "mask_sniper_up2";

	public const string DemolitionMask = "mask_demolition";

	public const string DemolitionMask_Up1 = "mask_demolition_up1";

	public const string DemolitionMask_Up2 = "mask_demolition_up2";

	public const string BerserkMask = "mask_berserk";

	public const string BerserkMask_Up1 = "mask_berserk_up1";

	public const string BerserkMask_Up2 = "mask_berserk_up2";

	public const string StormTrooperMask = "mask_trooper";

	public const string StormTrooperMask_Up1 = "mask_trooper_up1";

	public const string StormTrooperMask_Up2 = "mask_trooper_up2";

	public const string HitmanMask = "mask_hitman_1";

	public const string HitmanMask_Up1 = "mask_hitman_1_up1";

	public const string HitmanMask_Up2 = "mask_hitman_1_up2";

	public const string EngineerMask = "mask_engineer";

	public const string EngineerMask_Up1 = "mask_engineer_up1";

	public const string EngineerMask_Up2 = "mask_engineer_up2";

	public static Dictionary<string, string> descriptionLocalizationKeys;

	public static readonly Dictionary<ShopNGUIController.CategoryNames, List<List<string>>> wear;

	public static Dictionary<string, float> armorNum;

	public static Dictionary<string, List<float>> armorNumTemp;

	public static Dictionary<string, KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>> bootsMethods;

	public static Dictionary<string, KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>> capesMethods;

	public static Dictionary<string, KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>> hatsMethods;

	public static Dictionary<string, KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>> armorMethods;

	public static Dictionary<string, SaltedFloat> curArmor;

	static Wear()
	{
		descriptionLocalizationKeys = new Dictionary<string, string>
		{
			{ "boots_tabi", "Key_1816" },
			{ "boots_blue", "Key_1164" },
			{ "SniperBoots_Up1", "Key_1165" },
			{ "SniperBoots_Up2", "Key_1166" },
			{ "boots_green", "Key_1167" },
			{ "DemolitionBoots_Up1", "Key_1168" },
			{ "DemolitionBoots_Up2", "Key_1169" },
			{ "boots_black", "Key_1170" },
			{ "BerserkBoots_Up1", "Key_1171" },
			{ "BerserkBoots_Up2", "Key_1172" },
			{ "boots_gray", "Key_1173" },
			{ "StormTrooperBoots_Up1", "Key_1174" },
			{ "StormTrooperBoots_Up2", "Key_1175" },
			{ "boots_red", "Key_1176" },
			{ "HitmanBoots_Up1", "Key_1177" },
			{ "HitmanBoots_Up2", "Key_1178" },
			{ "EngineerBoots", "Key_1686" },
			{ "EngineerBoots_Up1", "Key_1687" },
			{ "EngineerBoots_Up2", "Key_1688" },
			{ "cape_Custom", "Key_1817" },
			{ "cape_SkeletonLord", "Key_1179" },
			{ "SniperCape_Up1", "Key_1180" },
			{ "SniperCape_Up2", "Key_1181" },
			{ "cape_RoyalKnight", "Key_1182" },
			{ "DemolitionCape_Up1", "Key_1183" },
			{ "DemolitionCape_Up2", "Key_1184" },
			{ "cape_BloodyDemon", "Key_1185" },
			{ "BerserkCape_Up1", "Key_1186" },
			{ "BerserkCape_Up2", "Key_1187" },
			{ "cape_EliteCrafter", "Key_1188" },
			{ "StormTrooperCape_Up1", "Key_1189" },
			{ "StormTrooperCape_Up2", "Key_1190" },
			{ "cape_Archimage", "Key_1191" },
			{ "HitmanCape_Up1", "Key_1192" },
			{ "HitmanCape_Up2", "Key_1193" },
			{ "cape_Engineer", "Key_1683" },
			{ "cape_Engineer_Up1", "Key_1684" },
			{ "cape_Engineer_Up2", "Key_1685" },
			{ "hat_DiamondHelmet", "Key_1822" },
			{ "hat_ManiacMask", "Key_1819" },
			{ "hat_KingsCrown", "Key_1820" },
			{ "hat_Samurai", "Key_1821" },
			{ "hat_SeriousManHat", "Key_1823" },
			{ "hat_Headphones", "Key_1818" },
			{ "league1_hat_hitman", "Key_2462" },
			{ "league2_hat_cowboyhat", "Key_2174" },
			{ "league3_hat_afro", "Key_2175" },
			{ "league4_hat_mushroom", "Key_2176" },
			{ "league5_hat_brain", "Key_2177" },
			{ "league6_hat_tiara", "Key_2178" },
			{ "mask_sniper", "Key_1845" },
			{ "mask_sniper_up1", "Key_1896" },
			{ "mask_sniper_up2", "Key_1897" },
			{ "mask_demolition", "Key_1846" },
			{ "mask_demolition_up1", "Key_1898" },
			{ "mask_demolition_up2", "Key_1899" },
			{ "mask_berserk", "Key_1847" },
			{ "mask_berserk_up1", "Key_1900" },
			{ "mask_berserk_up2", "Key_1901" },
			{ "mask_trooper", "Key_1848" },
			{ "mask_trooper_up1", "Key_1902" },
			{ "mask_trooper_up2", "Key_1903" },
			{ "mask_hitman_1", "Key_1849" },
			{ "mask_hitman_1_up1", "Key_1904" },
			{ "mask_hitman_1_up2", "Key_1905" },
			{ "mask_engineer", "Key_1850" },
			{ "mask_engineer_up1", "Key_1906" },
			{ "mask_engineer_up2", "Key_1907" }
		};
		wear = new Dictionary<ShopNGUIController.CategoryNames, List<List<string>>>(5, ShopNGUIController.CategoryNameComparer.Instance)
		{
			{
				ShopNGUIController.CategoryNames.CapesCategory,
				new List<List<string>>
				{
					new List<string> { "cape_Custom" },
					new List<string> { "cape_EliteCrafter", "StormTrooperCape_Up1", "StormTrooperCape_Up2" },
					new List<string> { "cape_Archimage", "HitmanCape_Up1", "HitmanCape_Up2" },
					new List<string> { "cape_BloodyDemon", "BerserkCape_Up1", "BerserkCape_Up2" },
					new List<string> { "cape_Engineer", "cape_Engineer_Up1", "cape_Engineer_Up2" },
					new List<string> { "cape_SkeletonLord", "SniperCape_Up1", "SniperCape_Up2" },
					new List<string> { "cape_RoyalKnight", "DemolitionCape_Up1", "DemolitionCape_Up2" }
				}
			},
			{
				ShopNGUIController.CategoryNames.HatsCategory,
				new List<List<string>>
				{
					new List<string>
					{
						"hat_Army_1", "hat_Army_2", "hat_Army_3", "hat_Steel_1", "hat_Steel_2", "hat_Steel_3", "hat_Royal_1", "hat_Royal_2", "hat_Royal_3", "hat_Almaz_1",
						"hat_Almaz_2", "hat_Almaz_3", "hat_Rubin_1", "hat_Rubin_2", "hat_Rubin_3", "hat_Adamant_Const_1", "hat_Adamant_Const_2", "hat_Adamant_Const_3"
					},
					new List<string> { "league1_hat_hitman" },
					new List<string> { "league2_hat_cowboyhat" },
					new List<string> { "league3_hat_afro" },
					new List<string> { "league4_hat_mushroom" },
					new List<string> { "league5_hat_brain" },
					new List<string> { "league6_hat_tiara" },
					new List<string> { "hat_Adamant_3" },
					new List<string> { "hat_Headphones" },
					new List<string> { "hat_KingsCrown" },
					new List<string> { "hat_Samurai" },
					new List<string> { "hat_DiamondHelmet" },
					new List<string> { "hat_SeriousManHat" }
				}
			},
			{
				ShopNGUIController.CategoryNames.BootsCategory,
				new List<List<string>>
				{
					new List<string> { "boots_gray", "StormTrooperBoots_Up1", "StormTrooperBoots_Up2" },
					new List<string> { "boots_red", "HitmanBoots_Up1", "HitmanBoots_Up2" },
					new List<string> { "boots_black", "BerserkBoots_Up1", "BerserkBoots_Up2" },
					new List<string> { "EngineerBoots", "EngineerBoots_Up1", "EngineerBoots_Up2" },
					new List<string> { "boots_blue", "SniperBoots_Up1", "SniperBoots_Up2" },
					new List<string> { "boots_green", "DemolitionBoots_Up1", "DemolitionBoots_Up2" },
					new List<string> { "boots_tabi" }
				}
			},
			{
				ShopNGUIController.CategoryNames.ArmorCategory,
				new List<List<string>>
				{
					new List<string>
					{
						"Armor_Army_1", "Armor_Army_2", "Armor_Army_3", "Armor_Steel_1", "Armor_Steel_2", "Armor_Steel_3", "Armor_Royal_1", "Armor_Royal_2", "Armor_Royal_3", "Armor_Almaz_1",
						"Armor_Almaz_2", "Armor_Almaz_3", "Armor_Rubin_1", "Armor_Rubin_2", "Armor_Rubin_3", "Armor_Adamant_Const_1", "Armor_Adamant_Const_2", "Armor_Adamant_Const_3"
					},
					new List<string> { "Armor_Novice" },
					new List<string> { "Armor_Adamant_3" }
				}
			},
			{
				ShopNGUIController.CategoryNames.MaskCategory,
				new List<List<string>>
				{
					new List<string> { "mask_trooper", "mask_trooper_up1", "mask_trooper_up2" },
					new List<string> { "mask_hitman_1", "mask_hitman_1_up1", "mask_hitman_1_up2" },
					new List<string> { "mask_berserk", "mask_berserk_up1", "mask_berserk_up2" },
					new List<string> { "mask_engineer", "mask_engineer_up1", "mask_engineer_up2" },
					new List<string> { "mask_sniper", "mask_sniper_up1", "mask_sniper_up2" },
					new List<string> { "mask_demolition", "mask_demolition_up1", "mask_demolition_up2" },
					new List<string> { "hat_ManiacMask" }
				}
			}
		};
		armorNum = new Dictionary<string, float>();
		armorNumTemp = new Dictionary<string, List<float>>
		{
			{
				"Armor_Adamant_3",
				new List<float> { 5f, 10f, 16f, 20f, 25f }
			},
			{
				"hat_Adamant_3",
				new List<float> { 5f, 10f, 16f, 20f, 25f }
			}
		};
		bootsMethods = new Dictionary<string, KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>>();
		capesMethods = new Dictionary<string, KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>>();
		hatsMethods = new Dictionary<string, KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>>();
		armorMethods = new Dictionary<string, KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>>();
		curArmor = new Dictionary<string, SaltedFloat>();
		bootsMethods.Add("boots_red", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(ActivateBoots_red, deActivateBoots_red));
		bootsMethods.Add("boots_gray", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(ActivateBoots_grey, deActivateBoots_grey));
		bootsMethods.Add("boots_blue", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(ActivateBoots_blue, deActivateBoots_blue));
		bootsMethods.Add("boots_green", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(ActivateBoots_green, deActivateBoots_green));
		bootsMethods.Add("boots_black", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(ActivateBoots_black, deActivateBoots_black));
		capesMethods.Add("cape_BloodyDemon", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_cape_BloodyDemon, deActivate_cape_BloodyDemon));
		capesMethods.Add("cape_RoyalKnight", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_cape_RoyalKnight, deActivate_cape_RoyalKnight));
		capesMethods.Add("cape_SkeletonLord", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_cape_SkeletonLord, deActivate_cape_SkeletonLord));
		capesMethods.Add("cape_Archimage", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_cape_Archimage, deActivate_cape_Archimage));
		capesMethods.Add("cape_EliteCrafter", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_cape_EliteCrafter, deActivate_cape_EliteCrafter));
		capesMethods.Add("cape_Custom", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_cape_Custom, deActivate_cape_Custom));
		hatsMethods.Add("hat_Adamant_3", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_EMPTY, deActivate_hat_EMPTY));
		hatsMethods.Add("hat_Headphones", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_EMPTY, deActivate_hat_EMPTY));
		hatsMethods.Add("hat_ManiacMask", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_ManiacMask, deActivate_hat_ManiacMask));
		hatsMethods.Add("hat_KingsCrown", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_KingsCrown, deActivate_hat_KingsCrown));
		hatsMethods.Add("hat_Samurai", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_Samurai, deActivate_hat_Samurai));
		hatsMethods.Add("hat_DiamondHelmet", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_DiamondHelmet, deActivate_hat_DiamondHelmet));
		hatsMethods.Add("hat_SeriousManHat", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_SeriousManHat, deActivate_hat_SeriousManHat));
		hatsMethods.Add("league1_hat_hitman", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_EMPTY, deActivate_Armor_EMPTY));
		hatsMethods.Add("league2_hat_cowboyhat", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_EMPTY, deActivate_Armor_EMPTY));
		hatsMethods.Add("league3_hat_afro", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_EMPTY, deActivate_Armor_EMPTY));
		hatsMethods.Add("league4_hat_mushroom", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_EMPTY, deActivate_Armor_EMPTY));
		hatsMethods.Add("league5_hat_brain", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_EMPTY, deActivate_Armor_EMPTY));
		hatsMethods.Add("league6_hat_tiara", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_EMPTY, deActivate_Armor_EMPTY));
		hatsMethods.Add("hat_AlmazHelmet", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_AlmazHelmet, deActivate_hat_AlmazHelmet));
		hatsMethods.Add("hat_ArmyHelmet", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_ArmyHelmet, deActivate_hat_ArmyHelmet));
		hatsMethods.Add("hat_GoldHelmet", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_GoldHelmet, deActivate_hat_GoldHelmet));
		hatsMethods.Add("hat_SteelHelmet", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_SteelHelmet, deActivate_hat_SteelHelmet));
		hatsMethods.Add("hat_Steel_1", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_Steel_1, deActivate_hat_Steel_1));
		hatsMethods.Add("hat_Royal_1", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_Royal_1, deActivate_hat_Royal_1));
		hatsMethods.Add("hat_Almaz_1", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_hat_Almaz_1, deActivate_hat_Almaz_1));
		bootsMethods.Add("boots_tabi", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_boots_tabi, deActivate_boots_tabi));
		armorMethods.Add("Armor_Adamant_3", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_EMPTY, deActivate_Armor_EMPTY));
		armorMethods.Add("Armor_Steel_1", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_EMPTY, deActivate_Armor_EMPTY));
		armorMethods.Add("Armor_Steel_2", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Steel_2, deActivate_Armor_Steel_2));
		armorMethods.Add("Armor_Steel_3", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Steel_3, deActivate_Armor_Steel_3));
		armorMethods.Add("Armor_Steel_4", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Steel_4, deActivate_Armor_Steel_4));
		armorMethods.Add("Armor_Royal_1", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Royal_1, deActivate_Armor_Royal_1));
		armorMethods.Add("Armor_Royal_2", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Royal_2, deActivate_Armor_Royal_2));
		armorMethods.Add("Armor_Royal_3", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Royal_3, deActivate_Armor_Royal_3));
		armorMethods.Add("Armor_Royal_4", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Royal_4, deActivate_Armor_Royal_4));
		armorMethods.Add("Armor_Almaz_1", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Almaz_1, deActivate_Armor_Almaz_1));
		armorMethods.Add("Armor_Almaz_2", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Almaz_2, deActivate_Armor_Almaz_2));
		armorMethods.Add("Armor_Almaz_3", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Almaz_3, deActivate_Armor_Almaz_3));
		armorMethods.Add("Armor_Almaz_4", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Almaz_4, deActivate_Armor_Almaz_4));
		armorMethods.Add("Armor_Novice", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Almaz_2, deActivate_Armor_Almaz_2));
		armorMethods.Add("Armor_Army_1", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_1, deActivate_Armor_Army_1));
		armorMethods.Add("Armor_Army_2", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_2, deActivate_Armor_Army_2));
		armorMethods.Add("Armor_Army_3", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_3, deActivate_Armor_Army_3));
		armorMethods.Add("Armor_Army_4", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_4, deActivate_Armor_Army_4));
		armorMethods.Add("Armor_Rubin_1", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_1, deActivate_Armor_Army_1));
		armorMethods.Add("Armor_Rubin_2", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_2, deActivate_Armor_Army_2));
		armorMethods.Add("Armor_Rubin_3", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_3, deActivate_Armor_Army_3));
		armorMethods.Add("Armor_Adamant_Const_1", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_1, deActivate_Armor_Army_1));
		armorMethods.Add("Armor_Adamant_Const_2", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_2, deActivate_Armor_Army_2));
		armorMethods.Add("Armor_Adamant_Const_3", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_3, deActivate_Armor_Army_3));
		armorMethods.Add("hat_Rubin_1", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_1, deActivate_Armor_Army_1));
		armorMethods.Add("hat_Rubin_2", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_2, deActivate_Armor_Army_2));
		armorMethods.Add("hat_Rubin_3", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_3, deActivate_Armor_Army_3));
		armorMethods.Add("hat_Adamant_Const_1", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_1, deActivate_Armor_Army_1));
		armorMethods.Add("hat_Adamant_Const_2", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_2, deActivate_Armor_Army_2));
		armorMethods.Add("hat_Adamant_Const_3", new KeyValuePair<Action<Player_move_c, Dictionary<string, object>>, Action<Player_move_c, Dictionary<string, object>>>(Activate_Armor_Army_3, deActivate_Armor_Army_3));
		armorNum.Add("Armor_Novice", 0f);
		armorNum.Add("Armor_Army_1", 1f);
		armorNum.Add("Armor_Army_2", 2f);
		armorNum.Add("Armor_Army_3", 3f);
		armorNum.Add("Armor_Army_4", 9f);
		armorNum.Add("Armor_Steel_1", 4f);
		armorNum.Add("Armor_Steel_2", 5f);
		armorNum.Add("Armor_Steel_3", 8f);
		armorNum.Add("Armor_Steel_4", 27f);
		armorNum.Add("Armor_Royal_1", 9f);
		armorNum.Add("Armor_Royal_2", 10f);
		armorNum.Add("Armor_Royal_3", 14f);
		armorNum.Add("Armor_Royal_4", 63f);
		armorNum.Add("Armor_Almaz_1", 15f);
		armorNum.Add("Armor_Almaz_2", 16f);
		armorNum.Add("Armor_Almaz_3", 18f);
		armorNum.Add("Armor_Almaz_4", 133f);
		armorNum.Add("Armor_Rubin_1", 19f);
		armorNum.Add("Armor_Rubin_2", 20f);
		armorNum.Add("Armor_Rubin_3", 22f);
		armorNum.Add("Armor_Adamant_Const_1", 24f);
		armorNum.Add("Armor_Adamant_Const_2", 26f);
		armorNum.Add("Armor_Adamant_Const_3", 28f);
		armorNum.Add("hat_Army_1", 1f);
		armorNum.Add("hat_Steel_1", 4f);
		armorNum.Add("hat_Royal_1", 9f);
		armorNum.Add("hat_Almaz_1", 15f);
		armorNum.Add("hat_Army_2", 2f);
		armorNum.Add("hat_Steel_2", 5f);
		armorNum.Add("hat_Royal_2", 10f);
		armorNum.Add("hat_Almaz_2", 16f);
		armorNum.Add("hat_Army_3", 3f);
		armorNum.Add("hat_Steel_3", 8f);
		armorNum.Add("hat_Royal_3", 14f);
		armorNum.Add("hat_Almaz_3", 18f);
		armorNum.Add("hat_Army_4", 1f);
		armorNum.Add("hat_Steel_4", 1f);
		armorNum.Add("hat_Royal_4", 2f);
		armorNum.Add("hat_Almaz_4", 3f);
		armorNum.Add("hat_Rubin_1", 19f);
		armorNum.Add("hat_Rubin_2", 20f);
		armorNum.Add("hat_Rubin_3", 22f);
		armorNum.Add("hat_Adamant_Const_1", 24f);
		armorNum.Add("hat_Adamant_Const_2", 26f);
		armorNum.Add("hat_Adamant_Const_3", 28f);
	}

	public static void RemoveTemporaryWear(string item)
	{
	}

	public static string ArmorOrArmorHatAvailableForBuy(ShopNGUIController.CategoryNames category)
	{
		//Discarded unreachable code: IL_00ba, IL_00db
		if (category != ShopNGUIController.CategoryNames.ArmorCategory && category != ShopNGUIController.CategoryNames.HatsCategory)
		{
			Debug.LogError("ArmorOrArmorHatAvailableForBuy incorrect category " + category);
			return string.Empty;
		}
		if (category == ShopNGUIController.CategoryNames.ArmorCategory && ShopNGUIController.NoviceArmorAvailable)
		{
			return string.Empty;
		}
		try
		{
			string text = WeaponManager.LastBoughtTag(wear[category][0][0]);
			string text2 = WeaponManager.FirstUnboughtTag(wear[category][0][0]);
			if (text != null && text == text2)
			{
				return string.Empty;
			}
			if (TierForWear(text2) <= ExpController.OurTierForAnyPlace())
			{
				return text2;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Debug.LogError("ArmorOrArmorHatAvailableForBuy Exception: " + ex);
			return string.Empty;
		}
	}

	public static bool NonArmorHat(string showHatTag)
	{
		return showHatTag != null && wear[ShopNGUIController.CategoryNames.HatsCategory].SelectMany((List<string> list) => list).Contains(showHatTag) && showHatTag != "hat_Adamant_3" && !wear[ShopNGUIController.CategoryNames.HatsCategory][0].Contains(showHatTag);
	}

	public static float MaxArmorForItem(string armorName, int roomTier)
	{
		float value = 0f;
		if (armorName != null && TempItemsController.PriceCoefs.ContainsKey(armorName) && ExpController.Instance != null)
		{
			if (armorNumTemp.ContainsKey(armorName) && armorNumTemp[armorName].Count > ExpController.Instance.OurTier)
			{
				value = armorNumTemp[armorName][Math.Min(roomTier, ExpController.Instance.OurTier)];
			}
		}
		else
		{
			int num = Math.Min((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier, roomTier);
			bool flag = false;
			List<string> list = null;
			foreach (List<List<string>> value2 in wear.Values)
			{
				foreach (List<string> item in value2)
				{
					if (item.Contains(armorName ?? string.Empty))
					{
						flag = true;
						list = item;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (list != null)
			{
				int num2 = list.IndexOf(armorName ?? string.Empty);
				if (num2 > 3 * (num + 1) - 1)
				{
					armorName = list[3 * (num + 1) - 1];
				}
			}
			armorNum.TryGetValue(armorName ?? string.Empty, out value);
		}
		return value * EffectsController.IcnreaseEquippedArmorPercentage;
	}

	public static int GetArmorCountFor(string armorTag, string hatTag)
	{
		return (int)(MaxArmorForItem(armorTag, ExpController.LevelsForTiers.Length - 1) + MaxArmorForItem(hatTag, ExpController.LevelsForTiers.Length - 1));
	}

	public static List<string> AllWears(ShopNGUIController.CategoryNames category, bool onlyNonLeagueItems = true)
	{
		List<string> list = new List<string>();
		list = (from l in wear[category]
			from i in l
			select i).ToList();
		if (onlyNonLeagueItems)
		{
			try
			{
				list = list.Where((string item) => LeagueForWear(item, category) == 0 && item != "league1_hat_hitman").ToList();
				return list;
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in AllWears filtering onlyNonLeagueItems: " + ex);
				return list;
			}
		}
		return list;
	}

	public static List<string> AllWears(ShopNGUIController.CategoryNames category, int tier, bool includePreviousTiers_UNUSED = false, bool withoutUpgrades = false)
	{
		List<int> list = new List<int>();
		for (int i = 0; i <= tier; i++)
		{
			list.Add(i);
		}
		List<string> list2 = new List<string>();
		foreach (string item3 in AllWears(category))
		{
			int item2 = TierForWear(item3);
			if (list.Contains(item2))
			{
				list2.Add(item3);
			}
		}
		if (withoutUpgrades)
		{
			List<List<string>> source = wear[category];
			for (int num = list2.Count; num > 0; num--)
			{
				string item = list2[num - 1];
				if (source.All((List<string> l) => l[0] != item))
				{
					list2.Remove(item);
				}
			}
		}
		return list2;
	}

	public static int LeagueForWear(string name, ShopNGUIController.CategoryNames category)
	{
		if (name == null)
		{
			Debug.LogError("LeagueForWear: name == null");
			return 0;
		}
		ShopPositionParams shopPositionParams = null;
		try
		{
			shopPositionParams = ItemDb.GetInfoForNonWeaponItem(name, category);
		}
		catch (Exception ex)
		{
			Debug.LogError("LeagueForWear: Exception: " + ex);
		}
		return (shopPositionParams != null) ? shopPositionParams.League : 0;
	}

	public static Dictionary<RatingSystem.RatingLeague, List<string>> UnboughtLeagueItemsByLeagues()
	{
		Dictionary<RatingSystem.RatingLeague, List<string>> dictionary = LeagueItemsByLeagues();
		try
		{
			dictionary = dictionary.ToDictionary((KeyValuePair<RatingSystem.RatingLeague, List<string>> kvp) => kvp.Key, (KeyValuePair<RatingSystem.RatingLeague, List<string>> kvp) => kvp.Value.Where((string item) => Storager.getInt(item, true) == 0).ToList(), RatingSystem.RatingLeagueComparer.Instance);
			return dictionary;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in UnboughtLeagueItemsByLeagues: " + ex);
			return dictionary;
		}
	}

	public static Dictionary<RatingSystem.RatingLeague, List<string>> LeagueItemsByLeagues()
	{
		Dictionary<RatingSystem.RatingLeague, List<string>> dictionary = new Dictionary<RatingSystem.RatingLeague, List<string>>(RatingSystem.RatingLeagueComparer.Instance);
		try
		{
			dictionary = (from item in wear[ShopNGUIController.CategoryNames.HatsCategory].SelectMany((List<string> list) => list)
				group item by LeagueForWear(item, ShopNGUIController.CategoryNames.HatsCategory) into grouping
				where grouping.Key > 0
				select grouping).ToDictionary((IGrouping<int, string> grouping) => (RatingSystem.RatingLeague)grouping.Key, (IGrouping<int, string> grouping) => grouping.ToList(), RatingSystem.RatingLeagueComparer.Instance);
			dictionary[RatingSystem.RatingLeague.Wood] = new List<string> { "league1_hat_hitman" };
			return dictionary;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in LeagueItemsByLeagues: " + ex);
			return dictionary;
		}
	}

	public static Dictionary<LeagueItemState, List<string>> LeagueItems()
	{
		Dictionary<LeagueItemState, List<string>> dictionary = new Dictionary<LeagueItemState, List<string>>();
		try
		{
			IEnumerable<string> enumerable = from item in wear[ShopNGUIController.CategoryNames.HatsCategory].SelectMany((List<string> list) => list)
				where LeagueForWear(item, ShopNGUIController.CategoryNames.HatsCategory) > 0 || item == "league1_hat_hitman"
				select item;
			dictionary[LeagueItemState.Open] = (from item in enumerable
				where LeagueForWear(item, ShopNGUIController.CategoryNames.HatsCategory) <= (int)RatingSystem.instance.currentLeague
				orderby LeagueForWear(item, ShopNGUIController.CategoryNames.HatsCategory)
				select item).ToList();
			dictionary[LeagueItemState.Closed] = (from item in enumerable.Except(dictionary[LeagueItemState.Open])
				orderby LeagueForWear(item, ShopNGUIController.CategoryNames.HatsCategory)
				select item).ToList();
			dictionary[LeagueItemState.Purchased] = (from item in enumerable
				where Storager.getInt(item, true) > 0
				orderby LeagueForWear(item, ShopNGUIController.CategoryNames.HatsCategory)
				select item).ToList();
			return dictionary;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in LeagueItems: " + ex);
			return dictionary;
		}
	}

	public static int TierForWear(string w)
	{
		if (w == null)
		{
			return 0;
		}
		if (wear[ShopNGUIController.CategoryNames.HatsCategory][0].Contains(w))
		{
			return wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(w) / 3;
		}
		if (wear[ShopNGUIController.CategoryNames.ArmorCategory][0].Contains(w))
		{
			return wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(w) / 3;
		}
		foreach (KeyValuePair<ShopNGUIController.CategoryNames, List<List<string>>> item in wear)
		{
			foreach (List<string> item2 in item.Value)
			{
				if (item2.Contains(w))
				{
					return (item.Key != ShopNGUIController.CategoryNames.MaskCategory) ? item2.IndexOf(w) : (item2.IndexOf(w) * 2);
				}
			}
		}
		return 0;
	}

	public static void ActivateBoots_red(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivateBoots_red(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_boots_tabi(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_boots_tabi(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void ActivateBoots_grey(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivateBoots_grey(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void ActivateBoots_blue(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivateBoots_blue(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void ActivateBoots_green(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivateBoots_green(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void ActivateBoots_black(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivateBoots_black(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_cape_BloodyDemon(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_cape_BloodyDemon(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_cape_RoyalKnight(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_cape_RoyalKnight(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_cape_SkeletonLord(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_cape_SkeletonLord(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_cape_Archimage(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_cape_Archimage(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_cape_EliteCrafter(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_cape_EliteCrafter(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_cape_Custom(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null) && !Defs.isHunger)
		{
			move.koofDamageWeaponFromPotoins += 0.05f;
		}
	}

	public static void deActivate_cape_Custom(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null) && !Defs.isHunger)
		{
			move.koofDamageWeaponFromPotoins -= 0.05f;
		}
	}

	public static void Activate_hat_EMPTY(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_hat_EMPTY(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_hat_ManiacMask(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_hat_ManiacMask(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_hat_KingsCrown(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null) && !Defs.isHunger)
		{
			move.koofDamageWeaponFromPotoins += 0.05f;
		}
	}

	public static void deActivate_hat_KingsCrown(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null) && !Defs.isHunger)
		{
			move.koofDamageWeaponFromPotoins -= 0.05f;
		}
	}

	public static void Activate_hat_Samurai(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null) && !Defs.isHunger)
		{
			move.koofDamageWeaponFromPotoins += 0.05f;
		}
	}

	public static void deActivate_hat_Samurai(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null) && !Defs.isHunger)
		{
			move.koofDamageWeaponFromPotoins -= 0.05f;
		}
	}

	public static void Activate_hat_DiamondHelmet(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_hat_DiamondHelmet(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_hat_SeriousManHat(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_hat_SeriousManHat(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_EMPTY(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_EMPTY(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Steel_2(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Steel_2(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Steel_3(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Steel_3(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Steel_4(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Steel_4(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Royal_4(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Royal_4(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Royal_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Royal_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Royal_2(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Royal_2(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Royal_3(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Royal_3(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Almaz_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Almaz_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Almaz_2(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Almaz_2(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Almaz_3(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Almaz_3(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Almaz_4(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Almaz_4(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Army_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Army_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Army_2(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Army_2(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Army_3(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Army_3(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_Armor_Army_4(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_Armor_Army_4(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_hat_SteelHelmet(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_hat_SteelHelmet(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_hat_GoldHelmet(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_hat_GoldHelmet(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_hat_ArmyHelmet(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_hat_ArmyHelmet(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_hat_AlmazHelmet(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_hat_AlmazHelmet(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_hat_Steel_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_hat_Steel_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_hat_Royal_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_hat_Royal_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void Activate_hat_Almaz_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void deActivate_hat_Almaz_1(Player_move_c move, Dictionary<string, object> p)
	{
		if (!(move == null))
		{
		}
	}

	public static void RenewCurArmor(int roomTier)
	{
		curArmor.Clear();
		foreach (string key in armorNum.Keys)
		{
			curArmor.Add(key, new SaltedFloat((!Defs.isHunger) ? MaxArmorForItem(key, roomTier) : 0f));
		}
		foreach (string key2 in armorNumTemp.Keys)
		{
			curArmor.Add(key2, new SaltedFloat((!Defs.isHunger) ? MaxArmorForItem(key2, roomTier) : 0f));
		}
	}
}

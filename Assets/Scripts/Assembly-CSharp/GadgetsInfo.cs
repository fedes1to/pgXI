using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public class GadgetsInfo
{
	private const string FirstForOurTierKey = "GadgetsInfo.FirstForOurTier";

	private static string up1_suffix;

	private static string up2_suffix;

	private static Dictionary<string, string> firstGadgetsForOurTier;

	private static bool _firstsForTiersInitialized;

	private static Dictionary<string, List<string>> _upgrades;

	private static IEnumerable<List<string>> _upgradeChains;

	private static bool _upgradesInitialized;

	private static GadgetInfo.GadgetCategory _defaultGadget;

	private static Dictionary<string, GadgetInfo> _info;

	private static Dictionary<GadgetInfo.GadgetCategory, Dictionary<string, GadgetInfo>> _infosByCategories;

	public static Dictionary<string, GadgetInfo> info
	{
		get
		{
			return _info;
		}
	}

	public static GadgetInfo.GadgetCategory DefaultGadget
	{
		get
		{
			return _defaultGadget;
		}
		set
		{
			_defaultGadget = value;
		}
	}

	public static Dictionary<string, List<string>> Upgrades
	{
		get
		{
			InitializeUpgrades();
			return _upgrades;
		}
	}

	public static IEnumerable<List<string>> UpgradeChains
	{
		get
		{
			InitializeUpgrades();
			return _upgradeChains;
		}
	}

	public static event Action<string> OnGetGadget;

	static GadgetsInfo()
	{
		up1_suffix = "_up1";
		up2_suffix = "_up2";
		firstGadgetsForOurTier = new Dictionary<string, string>();
		_firstsForTiersInitialized = false;
		_upgrades = null;
		_upgradeChains = null;
		_upgradesInitialized = false;
		_defaultGadget = GadgetInfo.GadgetCategory.Throwing;
		_info = new Dictionary<string, GadgetInfo>();
		_infosByCategories = null;
		GadgetsInfo.OnGetGadget = null;
		_info = new Dictionary<string, GadgetInfo>
		{
			{
				"gadget_black_label",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_black_label", "Key_2515", 2, null, "gadget_black_label_up1", "Key_2583", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime
				}, new List<WeaponSounds.Effects>(), PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_black_label_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_black_label_up1", "Key_2659", 4, "gadget_black_label", "gadget_black_label_up2", "Key_2583", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime
				}, new List<WeaponSounds.Effects>(), PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_black_label_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_black_label_up2", "Key_2660", 6, "gadget_black_label_up1", null, "Key_2583", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime
				}, new List<WeaponSounds.Effects>(), PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_Blizzard_generator",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_Blizzard_generator", "Key_2512", 2, null, "gadget_Blizzard_generator_up1", "Key_2581", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.SlowTheTarget
				}, PlayerEventScoreController.ScoreEvent.coldShower)
			},
			{
				"gadget_Blizzard_generator_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_Blizzard_generator_up1", "Key_2657", 4, "gadget_Blizzard_generator", "gadget_Blizzard_generator_up2", "Key_2581", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.SlowTheTarget
				}, PlayerEventScoreController.ScoreEvent.coldShower)
			},
			{
				"gadget_Blizzard_generator_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_Blizzard_generator_up2", "Key_2658", 6, "gadget_Blizzard_generator_up1", null, "Key_2581", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.SlowTheTarget
				}, PlayerEventScoreController.ScoreEvent.coldShower)
			},
			{
				"gadget_christmastreeturret",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_christmastreeturret", "Key_2747", 2, null, "gadget_christmastreeturret_up1", "Key_2738", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Durability,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Automatic,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_christmastreeturret_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_christmastreeturret_up1", "Key_2773", 4, "gadget_christmastreeturret", "gadget_christmastreeturret_up2", "Key_2738", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Durability,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Automatic,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_christmastreeturret_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_christmastreeturret_up2", "Key_2774", 6, "gadget_christmastreeturret_up1", null, "Key_2738", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Durability,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Automatic,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_demon_stone",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_demon_stone", "Key_2524", 2, null, "gadget_demon_stone_up1", "Key_2580", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Durability,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Flying,
					WeaponSounds.Effects.Melee
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_demon_stone_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_demon_stone_up1", "Key_2661", 4, "gadget_demon_stone", "gadget_demon_stone_up2", "Key_2580", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Durability,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Flying,
					WeaponSounds.Effects.Melee
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_demon_stone_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_demon_stone_up2", "Key_2662", 6, "gadget_demon_stone_up1", null, "Key_2580", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Durability,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Flying,
					WeaponSounds.Effects.Melee
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_disabler",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_disabler", "Key_2526", 6, null, null, "Key_2582", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.GadgetBlocker,
					WeaponSounds.Effects.AreaOfEffects
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_dragonwhistle",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_dragonwhistle", "Key_2525", 4, null, "gadget_dragonwhistle_up1", "Key_2585", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.Burning
				}, PlayerEventScoreController.ScoreEvent.dragonSpirit)
			},
			{
				"gadget_dragonwhistle_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_dragonwhistle_up1", "Key_2668", 6, "gadget_dragonwhistle", null, "Key_2585", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.Burning
				}, PlayerEventScoreController.ScoreEvent.dragonSpirit)
			},
			{
				"gadget_fakebonus",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_fakebonus", "Key_2527", 3, null, "gadget_fakebonus_up1", "Key_2567", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Damage
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Detonation,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.joker)
			},
			{
				"gadget_fakebonus_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_fakebonus_up1", "Key_2665", 5, "gadget_fakebonus", null, "Key_2567", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Damage
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Detonation,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.joker)
			},
			{
				"gadget_firemushroom",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_firemushroom", "Key_2514", 2, null, "gadget_firemushroom_up1", "Key_2586", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.Burning
				}, PlayerEventScoreController.ScoreEvent.mushroomer)
			},
			{
				"gadget_firemushroom_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_firemushroom_up1", "Key_2655", 4, "gadget_firemushroom", "gadget_firemushroom_up2", "Key_2586", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.Burning
				}, PlayerEventScoreController.ScoreEvent.mushroomer)
			},
			{
				"gadget_firemushroom_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_firemushroom_up2", "Key_2656", 6, "gadget_firemushroom_up1", null, "Key_2586", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.Burning
				}, PlayerEventScoreController.ScoreEvent.mushroomer)
			},
			{
				"gadget_firework",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_firework", "Key_2743", 1, null, "gadget_firework_up1", "Key_2736", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Detonation,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_firework_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_firework_up1", "Key_2767", 3, "gadget_firework", "gadget_firework_up2", "Key_2736", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Detonation,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_firework_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_firework_up2", "Key_2768", 5, "gadget_firework_up1", null, "Key_2736", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Detonation,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_fraggrenade",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_fraggrenade", "Key_2480", 1, null, "gadget_fraggrenade_up1", "Key_2538", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.AreaDamage }, PlayerEventScoreController.ScoreEvent.deadGrenade)
			},
			{
				"gadget_fraggrenade_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_fraggrenade_up1", "Key_2641", 3, "gadget_fraggrenade", "gadget_fraggrenade_up2", "Key_2538", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.AreaDamage }, PlayerEventScoreController.ScoreEvent.deadGrenade)
			},
			{
				"gadget_fraggrenade_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_fraggrenade_up2", "Key_2642", 5, "gadget_fraggrenade_up1", null, "Key_2538", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.AreaDamage }, PlayerEventScoreController.ScoreEvent.deadGrenade)
			},
			{
				"gadget_jetpack",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_jetpack", "Key_0772", 1, null, "gadget_jetpack_up1", "Key_2572", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_jetpack_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_jetpack_up1", "Key_2645", 3, "gadget_jetpack", "gadget_jetpack_up2", "Key_2572", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_jetpack_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_jetpack_up2", "Key_2646", 5, "gadget_jetpack_up1", null, "Key_2572", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_leaderdrum",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_leaderdrum", "Key_2744", 1, null, "gadget_leaderdrum_up1", "Key_2737", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.DamageBoost,
					WeaponSounds.Effects.AreaOfEffects
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_leaderdrum_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_leaderdrum_up1", "Key_2769", 3, "gadget_leaderdrum", "gadget_leaderdrum_up2", "Key_2737", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.DamageBoost,
					WeaponSounds.Effects.AreaOfEffects
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_leaderdrum_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_leaderdrum_up2", "Key_2770", 5, "gadget_leaderdrum_up1", null, "Key_2737", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.DamageBoost,
					WeaponSounds.Effects.AreaOfEffects
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_mech",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_mech", "Key_0774", 4, null, "gadget_mech_up1", "Key_2587", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Durability,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Automatic }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_mech_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_mech_up1", "Key_2669", 6, "gadget_mech", null, "Key_2587", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Durability,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Automatic }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_medicalstation",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_medicalstation", "Key_2513", 3, null, "gadget_medicalstation_up1", "Key_2540", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Durability
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Healing,
					WeaponSounds.Effects.AreaOfEffects
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_medicalstation_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_medicalstation_up1", "Key_2666", 5, "gadget_medicalstation", null, "Key_2540", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Durability
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Healing,
					WeaponSounds.Effects.AreaOfEffects
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_medkit",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_medkit", "Key_2475", 1, null, "gadget_medkit_up1", "Key_2576", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Healing
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Healing }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_medkit_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_medkit_up1", "Key_2643", 3, "gadget_medkit", "gadget_medkit_up2", "Key_2576", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Healing
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Healing }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_medkit_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_medkit_up2", "Key_2644", 5, "gadget_medkit_up1", null, "Key_2576", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Healing
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Healing }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_mine",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_mine", "Key_2485", 2, null, "gadget_mine_up1", "Key_2568", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Damage
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Detonation,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.illusionist)
			},
			{
				"gadget_mine_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_mine_up1", "Key_2653", 4, "gadget_mine", "gadget_mine_up2", "Key_2568", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Damage
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Detonation,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.illusionist)
			},
			{
				"gadget_mine_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_mine_up2", "Key_2654", 6, "gadget_mine_up1", null, "Key_2568", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Damage
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.Detonation,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.illusionist)
			},
			{
				"gadget_molotov",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_molotov", "Key_2484", 1, null, "gadget_molotov_up1", "Key_2569", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.Burning
				}, PlayerEventScoreController.ScoreEvent.renegade)
			},
			{
				"gadget_molotov_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_molotov_up1", "Key_2647", 3, "gadget_molotov", "gadget_molotov_up2", "Key_2569", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.Burning
				}, PlayerEventScoreController.ScoreEvent.renegade)
			},
			{
				"gadget_molotov_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_molotov_up2", "Key_2648", 5, "gadget_molotov_up1", null, "Key_2569", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.Burning
				}, PlayerEventScoreController.ScoreEvent.renegade)
			},
			{
				"gadget_ninjashurickens",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_ninjashurickens", "Key_2878", 3, null, "gadget_ninjashurickens_up1", "Key_2895", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.ThroughEnemies,
					WeaponSounds.Effects.Bleeding
				}, PlayerEventScoreController.ScoreEvent.renegade)
			},
			{
				"gadget_ninjashurickens_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_ninjashurickens_up1", "Key_2878", 5, "gadget_ninjashurickens", null, "Key_2895", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.ThroughEnemies,
					WeaponSounds.Effects.Bleeding
				}, PlayerEventScoreController.ScoreEvent.renegade)
			},
			{
				"gadget_nucleargrenade",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_nucleargrenade", "Key_2481", 6, null, null, "Key_2570", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Damage
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AreaDamage,
					WeaponSounds.Effects.Radiation
				}, PlayerEventScoreController.ScoreEvent.nuker)
			},
			{
				"gadget_nutcracker",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_nutcracker", "Key_2746", 4, null, "gadget_nutcracker_up1", "Key_2739", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AutoHoming,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.illusionist)
			},
			{
				"gadget_nutcracker_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_nutcracker_up1", "Key_2777", 6, "gadget_nutcracker", null, "Key_2739", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.AutoHoming,
					WeaponSounds.Effects.AreaDamage
				}, PlayerEventScoreController.ScoreEvent.illusionist)
			},
			{
				"gadget_pandorabox",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_pandorabox", "Key_2563", 3, null, "gadget_pandorabox_up1", "Key_2584", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Damage
				}, new List<WeaponSounds.Effects>(), PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_pandorabox_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_pandorabox_up1", "Key_2667", 5, "gadget_pandorabox", null, "Key_2584", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Damage
				}, new List<WeaponSounds.Effects>(), PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_petbooster",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_petbooster", "Key_2745", 2, null, "gadget_petbooster_up1", "Key_2741", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.DamageBoost }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_petbooster_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_petbooster_up1", "Key_2772", 4, "gadget_petbooster", "gadget_petbooster_up2", "Key_2741", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.DamageBoost }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_petbooster_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_petbooster_up2", "Key_2773", 6, "gadget_petbooster_up1", null, "Key_2741", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.DamageBoost }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_reflector",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_reflector", "Key_2482", 5, null, null, "Key_2577", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.DamageReflection }, PlayerEventScoreController.ScoreEvent.none)
			},
			//{
			//	"gadget_resurrection",
			//	new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_resurrection", "Key_2477", 6, null, null, "Key_2578", new List<GadgetInfo.Parameter> { GadgetInfo.Parameter.Cooldown }, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Resurrection }, PlayerEventScoreController.ScoreEvent.none)
			//},
			{
				"gadget_shield",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_shield", "Key_2478", 1, null, "gadget_shield_up1", "Key_2573", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Durability
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.DamageAbsorbtion }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_shield_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_shield_up1", "Key_2649", 3, "gadget_shield", "gadget_shield_up2", "Key_2573", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Durability
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.DamageAbsorbtion }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_shield_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Support, "gadget_shield_up2", "Key_2650", 5, "gadget_shield_up1", null, "Key_2573", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Durability
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.DamageAbsorbtion }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_singularity",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_singularity", "Key_2476", 5, null, null, "Key_2571", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.GravitationForce,
					WeaponSounds.Effects.AreaOfEffects
				}, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_snowman",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_snowman", "Key_2749", 3, null, "gadget_snowman_up1", "Key_2740", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.DamageTransfer }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_snowman_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_snowman_up1", "Key_2778", 5, "gadget_snowman", null, "Key_2740", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.DamageTransfer }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_stealth",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_stealth", "Key_2483", 2, null, "gadget_stealth_up1", "Key_2574", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Invisibility }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_stealth_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_stealth_up1", "Key_2663", 4, "gadget_stealth", "gadget_stealth_up2", "Key_2574", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Invisibility }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_stealth_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_stealth_up2", "Key_2664", 6, "gadget_stealth_up1", null, "Key_2574", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Cooldown,
					GadgetInfo.Parameter.Lifetime
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Invisibility }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_stickycandy",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_stickycandy", "Key_2748", 2, null, "gadget_stickycandy_up1", "Key_2742", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.SlowTheTarget,
					WeaponSounds.Effects.DisableJump
				}, PlayerEventScoreController.ScoreEvent.nuker)
			},
			{
				"gadget_stickycandy_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_stickycandy_up1", "Key_2775", 4, "gadget_stickycandy", "gadget_stickycandy_up2", "Key_2742", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.SlowTheTarget,
					WeaponSounds.Effects.DisableJump
				}, PlayerEventScoreController.ScoreEvent.nuker)
			},
			{
				"gadget_stickycandy_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Throwing, "gadget_stickycandy_up2", "Key_2776", 6, "gadget_stickycandy_up1", null, "Key_2742", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects>
				{
					WeaponSounds.Effects.SlowTheTarget,
					WeaponSounds.Effects.DisableJump
				}, PlayerEventScoreController.ScoreEvent.nuker)
			},
			{
				"gadget_timewatch",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_timewatch", "Key_2479", 5, null, null, "Key_2575", new List<GadgetInfo.Parameter> { GadgetInfo.Parameter.Cooldown }, new List<WeaponSounds.Effects> { WeaponSounds.Effects.TimeShift }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_turret",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_turret", "Key_0773", 1, null, "gadget_turret_up1", "Key_2539", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Durability,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Automatic }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_turret_up1",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_turret_up1", "Key_2651", 3, "gadget_turret", "gadget_turret_up2", "Key_2539", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Durability,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Automatic }, PlayerEventScoreController.ScoreEvent.none)
			},
			{
				"gadget_turret_up2",
				new GadgetInfo(GadgetInfo.GadgetCategory.Tools, "gadget_turret_up2", "Key_2652", 5, "gadget_turret_up1", null, "Key_2539", new List<GadgetInfo.Parameter>
				{
					GadgetInfo.Parameter.Damage,
					GadgetInfo.Parameter.Durability,
					GadgetInfo.Parameter.Lifetime,
					GadgetInfo.Parameter.Cooldown
				}, new List<WeaponSounds.Effects> { WeaponSounds.Effects.Automatic }, PlayerEventScoreController.ScoreEvent.none)
			}
		};
	}

	public static string BaseName(string id)
	{
		return id.Replace(up1_suffix, string.Empty).Replace(up2_suffix, string.Empty);
	}

	public static GameObject GetArmoryInfoPrefabFromName(string id)
	{
		string text = BaseName(id);
		GameObject gameObject = Resources.Load<GameObject>("GadgetsContent/GadgetsArmoryInfoPreview/" + text);
		if (gameObject == null)
		{
			gameObject = Resources.Load<GameObject>("GadgetsContent/GadgetsArmoryInfoPreview/empty");
		}
		return gameObject;
	}

	public static Dictionary<GadgetInfo.GadgetCategory, List<string>> GetGadgetsByCategoriesFromItems(List<string> items)
	{
		//Discarded unreachable code: IL_00a2
		try
		{
			IEnumerable<string> source = items.Intersect(info.Keys);
			IEnumerable<GadgetInfo> source2 = source.Select((string gadgetId) => info[gadgetId]);
			IEnumerable<IGrouping<GadgetInfo.GadgetCategory, GadgetInfo>> source3 = from gadgetInfo in source2
				group gadgetInfo by gadgetInfo.Category;
			return source3.ToDictionary((IGrouping<GadgetInfo.GadgetCategory, GadgetInfo> grouping) => grouping.Key, (IGrouping<GadgetInfo.GadgetCategory, GadgetInfo> grouping) => grouping.Select((GadgetInfo gadgetInfo) => gadgetInfo.Id).ToList());
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in GetGadgetsFromItems: {0}", ex);
		}
		return new Dictionary<GadgetInfo.GadgetCategory, List<string>>();
	}

	public static List<string> GetNewGadgetsForTier(int tier)
	{
		//Discarded unreachable code: IL_0077
		try
		{
			IEnumerable<GadgetInfo> source = UpgradeChains.Select((List<string> chain) => info[chain.First()]);
			return (from gadgetInfo in source
				where gadgetInfo.Tier == tier
				select gadgetInfo.Id).ToList();
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in GetNewGadgetsForTier: {0}", ex);
		}
		return new List<string>();
	}

	public static Dictionary<string, GadgetInfo> GadgetsOfCategory(GadgetInfo.GadgetCategory category)
	{
		if (_infosByCategories == null)
		{
			_infosByCategories = new Dictionary<GadgetInfo.GadgetCategory, Dictionary<string, GadgetInfo>>(GadgetCategoryComparer.Instance);
			GadgetInfo.GadgetCategory gadgetCategory;
			foreach (GadgetInfo.GadgetCategory item in Enum.GetValues(typeof(GadgetInfo.GadgetCategory)).OfType<GadgetInfo.GadgetCategory>())
			{
				gadgetCategory = item;
				_infosByCategories[gadgetCategory] = info.Where((KeyValuePair<string, GadgetInfo> kvp) => kvp.Value.Category == gadgetCategory).ToDictionary((KeyValuePair<string, GadgetInfo> kvp) => kvp.Key, (KeyValuePair<string, GadgetInfo> kvp) => kvp.Value);
			}
		}
		Dictionary<string, GadgetInfo> value = null;
		if (_infosByCategories.TryGetValue(category, out value))
		{
			return value;
		}
		return null;
	}

	public static IEnumerable<string> AvailableForBuyGadgets(int maximumTier)
	{
		//Discarded unreachable code: IL_006d
		try
		{
			return from chain in UpgradeChains
				where !IsBought(chain.Last())
				select FirstUnboughtOrForOurTier(chain.Last()) into firstUnbought
				where info[firstUnbought].Tier <= maximumTier
				select firstUnbought;
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in AvailableForBuyGadgets: {0}", ex);
		}
		return new List<string>();
	}

	public static void ActualizeEquippedGadgets()
	{
		try
		{
			GadgetInfo.GadgetCategory[] array = new GadgetInfo.GadgetCategory[3]
			{
				GadgetInfo.GadgetCategory.Throwing,
				GadgetInfo.GadgetCategory.Tools,
				GadgetInfo.GadgetCategory.Support
			};
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				GadgetInfo.GadgetCategory category = array[i];
				string text = EquippedForCategory(category);
				if (!text.IsNullOrEmpty())
				{
					string text2 = LastBoughtFor(text);
					if (!string.IsNullOrEmpty(text2) && text2 != text)
					{
						ShopNGUIController.EquipGadget(text2, category);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in ActualizeEquippedGadgets: {0}", ex);
		}
	}

	public static void ProvideGadget(string gadgetId)
	{
		if (gadgetId == null)
		{
			Debug.LogError("ProvideGadget gadgetId == null");
			return;
		}
		if (!info.ContainsKey(gadgetId))
		{
			Debug.LogError("ProvideGadget !info.ContainsKey(gadgetId)");
			return;
		}
		List<string> list = Upgrades[gadgetId];
		if (list == null)
		{
			Debug.LogError("ProvideGadget chain == null");
			return;
		}
		int index = 0;
		do
		{
			if (Storager.getInt(list[index], true) == 0)
			{
				Storager.setInt(list[index], 1, true);
				Action<string> onGetGadget = GadgetsInfo.OnGetGadget;
				if (onGetGadget != null)
				{
					onGetGadget(list[index]);
				}
			}
		}
		while (list[index++] != gadgetId);
	}

	public static string EquippedForCategory(GadgetInfo.GadgetCategory category)
	{
		return Storager.getString(SNForCategory(category), false);
	}

	public static string SNForCategory(GadgetInfo.GadgetCategory category)
	{
		return "Equipped_" + category.ToString() + "_SN";
	}

	public static string LastBoughtFor(string gadgetId)
	{
		if (gadgetId == null)
		{
			Debug.LogError("LastBoughtFor gadgetId == null");
			return null;
		}
		List<string> list = UpgradesChainForGadget(gadgetId);
		if (list == null)
		{
			Debug.LogError("LastBoughtFor chain == null , gadgetId = " + gadgetId);
			return null;
		}
		string result = null;
		for (int i = 0; i < list.Count && IsBought(list[i]); i++)
		{
			result = list[i];
		}
		return result;
	}

	public static List<string> UpgradesChainForGadget(string gadgetId)
	{
		if (gadgetId == null)
		{
			Debug.LogError("UpgradesChainForGadget gadgetId = null");
			return null;
		}
		List<string> value = null;
		Upgrades.TryGetValue(gadgetId, out value);
		if (value == null)
		{
			Debug.LogError("UpgradesChainForGadget chain = null, gadget = " + gadgetId);
		}
		return value;
	}

	public static string FirstForOurTier(string gadgetId)
	{
		if (gadgetId == null)
		{
			Debug.LogError("FirstForOurTier gadgetId == null");
			return null;
		}
		if (!_firstsForTiersInitialized)
		{
			InitFirstForOurTierData();
			_firstsForTiersInitialized = true;
		}
		List<string> list = UpgradesChainForGadget(gadgetId);
		if (list == null)
		{
			Debug.LogError("FirstForOurTier chain == null , gadgetId = " + gadgetId);
			return null;
		}
		if (list.Count > 0)
		{
			string value = null;
			firstGadgetsForOurTier.TryGetValue(list[0], out value);
			if (value == null)
			{
				Debug.LogError("FirstForOurTier first == null , gadgetId = " + gadgetId);
			}
			return value;
		}
		Debug.LogError("FirstForOurTier chain.Count = 0 gadgetId = " + gadgetId);
		return null;
	}

	public static string FirstUnboughtOrForOurTier(string gadgetId)
	{
		if (gadgetId == null)
		{
			Debug.LogError("FirstUnboughtOrForOurTier gadgetId == null");
			return null;
		}
		List<string> list = UpgradesChainForGadget(gadgetId);
		if (list == null)
		{
			Debug.LogError("FirstUnboughtOrForOurTier chain == null , gadgetId = " + gadgetId);
			return null;
		}
		string text = FirstUnbought(gadgetId);
		if (text == null)
		{
			Debug.LogError("FirstUnboughtOrForOurTier firstUnobught == null , gadgetId = " + gadgetId);
			return null;
		}
		string text2 = FirstForOurTier(gadgetId);
		if (text2 == null)
		{
			Debug.LogError("FirstUnboughtOrForOurTier forOurTier == null , gadgetId = " + gadgetId);
			return null;
		}
		return (list.IndexOf(text2) <= list.IndexOf(text)) ? text : text2;
	}

	public static string FirstUnbought(string gadgetId)
	{
		if (gadgetId == null)
		{
			Debug.LogError("FirstUnbought gadgetId == null");
			return null;
		}
		List<string> list = UpgradesChainForGadget(gadgetId);
		if (list == null)
		{
			Debug.LogError("FirstUnbought chain == null , gadgetId = " + gadgetId);
			return null;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (!IsBought(list[i]))
			{
				return list[i];
			}
		}
		return list[list.Count - 1];
	}

	public static bool IsBought(string gadgetId)
	{
		if (gadgetId == null)
		{
			Debug.LogError("IsBought gadgetId == null");
			return false;
		}
		return Storager.getInt(gadgetId, true) > 0;
	}

	public static void FixFirstsForOurTier()
	{
		try
		{
			bool flag = false;
			foreach (List<string> upgradeChain in UpgradeChains)
			{
				if (upgradeChain != null && upgradeChain.Count == 0)
				{
					continue;
				}
				string text = upgradeChain[0];
				string text2 = LastBoughtFor(text);
				if (text2 != null)
				{
					string item = FirstForOurTier(text);
					int num = upgradeChain.IndexOf(text2);
					int num2 = upgradeChain.IndexOf(item);
					if (num < num2)
					{
						flag = true;
						firstGadgetsForOurTier[text] = text2;
					}
				}
			}
			if (flag)
			{
				SaveFirstsToDisc();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in gadgets FixFirstsForOurTier: " + ex);
		}
	}

	private static void InitFirstForOurTierData()
	{
		//Discarded unreachable code: IL_0098
		if (!Storager.hasKey("GadgetsInfo.FirstForOurTier"))
		{
			Storager.setString("GadgetsInfo.FirstForOurTier", "{}", false);
		}
		string @string = Storager.getString("GadgetsInfo.FirstForOurTier", false);
		try
		{
			Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				firstGadgetsForOurTier.Add(item.Key, (string)item.Value);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return;
		}
		int ourTier = ExpController.GetOurTier();
		bool flag = false;
		foreach (List<string> upgradeChain in UpgradeChains)
		{
			if (upgradeChain.Count == 0)
			{
				Debug.LogError("InitFirstTagsData upgrades.Count == 0");
			}
			else
			{
				if (firstGadgetsForOurTier.ContainsKey(upgradeChain[0]))
				{
					continue;
				}
				flag = true;
				List<GadgetInfo> list = upgradeChain.Select((string gadgetId) => info[gadgetId]).ToList();
				bool flag2 = false;
				for (int i = 0; i < upgradeChain.Count; i++)
				{
					if (list[i] != null && list[i].Tier > ourTier)
					{
						firstGadgetsForOurTier.Add(upgradeChain[0], upgradeChain[Math.Max(0, i - 1)]);
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					firstGadgetsForOurTier.Add(upgradeChain[0], upgradeChain[upgradeChain.Count - 1]);
				}
			}
		}
		if (flag)
		{
			SaveFirstsToDisc();
		}
	}

	private static void SaveFirstsToDisc()
	{
		Storager.setString("GadgetsInfo.FirstForOurTier", Json.Serialize(firstGadgetsForOurTier), false);
	}

	private static void InitializeUpgrades()
	{
		if (_upgradesInitialized)
		{
			return;
		}
		_upgradesInitialized = true;
		IEnumerable<IGrouping<string, KeyValuePair<string, GadgetInfo>>> source = from kvp in info
			group kvp by BaseOfUpgardesChainFor(kvp.Value).Id;
		IEnumerable<List<string>> source2 = source.Select((IGrouping<string, KeyValuePair<string, GadgetInfo>> grouping) => (from kvp in grouping.OrderBy(delegate(KeyValuePair<string, GadgetInfo> kvp)
			{
				int num = 0;
				if (kvp.Value.PreviousUpgradeId != null)
				{
					num++;
				}
				if (kvp.Value.NextUpgradeId == null)
				{
					num += 10;
				}
				return num;
			})
			select kvp.Key).ToList());
		Dictionary<string, List<string>> upgrades = source2.SelectMany((List<string> upgradesChain) => upgradesChain.Select((string upgrade) => new KeyValuePair<string, List<string>>(upgrade, upgradesChain))).ToDictionary((KeyValuePair<string, List<string>> kvp) => kvp.Key, (KeyValuePair<string, List<string>> kvp) => kvp.Value);
		_upgrades = upgrades;
		_upgradeChains = _upgrades.Values.Distinct();
	}

	private static GadgetInfo BaseOfUpgardesChainFor(GadgetInfo gadgetInfo)
	{
		if (gadgetInfo == null)
		{
			Debug.LogError("BaseOfUpgardesChainFor gadgetInfo == null");
			return null;
		}
		return (gadgetInfo.PreviousUpgradeId == null) ? gadgetInfo : BaseOfUpgardesChainFor(info[gadgetInfo.PreviousUpgradeId]);
	}
}

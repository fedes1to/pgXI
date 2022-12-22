using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GearManager
{
	public const int MaxGrenadeCount = 10;

	public const int MaxGearCount = 1000000;

	public const int NumberOfGearInPack = 3;

	public static readonly string InvisibilityPotion = "InvisibilityPotion";

	public static readonly string Grenade = "GrenadeID";

	public static readonly string Like = "LikeID";

	public static readonly string Jetpack = "Jetpack";

	public static readonly string Turret = "Turret";

	public static readonly string Mech = "Mech";

	public static readonly string BigHeadPotion = "BigHeadPotion";

	public static readonly string Wings = "Wings";

	public static readonly string Bear = "Bear";

	public static readonly string MusicBox = "MusicBox";

	public static readonly string[] Gear = new string[5] { Grenade, InvisibilityPotion, Jetpack, Turret, Mech };

	public static readonly string[] DaterGear = new string[4] { BigHeadPotion, Wings, MusicBox, Bear };

	public static List<string> AllGear
	{
		get
		{
			return Gear.Concat(DaterGear).ToList();
		}
	}

	public static int NumOfGearUpgrades
	{
		get
		{
			return ExpController.LevelsForTiers.Length - 1;
		}
	}

	public static string OneItemSuffix
	{
		get
		{
			return "_OneItem_";
		}
	}

	public static string UpgradeSuffix
	{
		get
		{
			return "_Up_";
		}
	}

	public static string AnalyticsIDForOneItemOfGear(string itemName, bool changeGrenade = false)
	{
		if (itemName == null)
		{
			return string.Empty;
		}
		string text = HolderQuantityForID(itemName);
		if (text == null || !AllGear.Contains(text))
		{
			return string.Empty;
		}
		int num = CurrentNumberOfUphradesForGear(text);
		string text2 = text;
		if (changeGrenade && text.Equals(Grenade) && num == 0)
		{
			text2 = "Grenade";
		}
		if (num > 0)
		{
			text2 = text2 + "_" + num;
		}
		return text2;
	}

	public static int CurrentNumberOfUphradesForGear(string id)
	{
		if (Defs.isDaterRegim)
		{
			return 0;
		}
		int a = 0;
		if (ExpController.Instance != null)
		{
			a = ExpController.Instance.OurTier;
		}
		return Mathf.Min(a, NumOfGearUpgrades);
	}

	public static string OneItemIDForGear(string id, int i)
	{
		if (id == null)
		{
			return null;
		}
		return id + OneItemSuffix + i;
	}

	public static string UpgradeIDForGear(string id, int i)
	{
		if (id == null)
		{
			return null;
		}
		return id + UpgradeSuffix + i;
	}

	public static string HolderQuantityForID(string id)
	{
		if (id == null)
		{
			return string.Empty;
		}
		foreach (string item in AllGear)
		{
			if (ItemIsGear(id, item))
			{
				return item;
			}
		}
		return id;
	}

	public static int ItemsInPackForGear(string id)
	{
		return (id == null || !id.Equals(Grenade)) ? 3 : 5;
	}

	public static int MaxCountForGear(string id)
	{
		return (id == null || !id.Equals(Grenade)) ? 1000000 : 10;
	}

	public static string NameForUpgrade(string item, int num)
	{
		return item + UpgradeSuffix + num;
	}

	private static bool ItemIsGear(string item, string gear)
	{
		if (gear == null || item == null)
		{
			return false;
		}
		string text = gear + UpgradeSuffix;
		string text2 = gear + OneItemSuffix;
		int result;
		return item == gear || (item.StartsWith(text) && item.Length > text.Length && int.TryParse(string.Empty + item[text.Length], out result)) || (item.StartsWith(text2) && item.Length > text2.Length && int.TryParse(string.Empty + item[text2.Length], out result));
	}

	public static bool IsItemGear(string tag)
	{
		if (tag == null)
		{
			return false;
		}
		for (int i = 0; i < AllGear.Count; i++)
		{
			if (ItemIsGear(tag, AllGear[i]))
			{
				return true;
			}
		}
		return false;
	}
}

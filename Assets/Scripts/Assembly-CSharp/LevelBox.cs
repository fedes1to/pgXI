using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelBox
{
	public static readonly List<LevelBox> campaignBoxes;

	public readonly List<CampaignLevel> levels = new List<CampaignLevel>(6);

	public int starsToOpen;

	public string name;

	public string mapName;

	public string PreviewNAme = string.Empty;

	public int gems;

	public int coins;

	public static Dictionary<string, string> weaponsFromBosses;

	public int CompletionExperienceAward { get; set; }

	static LevelBox()
	{
		campaignBoxes = new List<LevelBox>(4);
		weaponsFromBosses = new Dictionary<string, string>(20);
		InitializeWeaponsFromBosses(weaponsFromBosses);
		LevelBox item = new LevelBox
		{
			starsToOpen = int.MaxValue,
			PreviewNAme = "Box_coming_soon",
			name = "coming soon",
			CompletionExperienceAward = 0
		};
		LevelBox levelBox = new LevelBox
		{
			starsToOpen = 30,
			name = "Crossed",
			mapName = string.Empty,
			PreviewNAme = "Box_3",
			CompletionExperienceAward = 100,
			gems = 15,
			coins = 20
		};
		levelBox.levels.Add(new CampaignLevel("Swamp_campaign3", "Key_0471"));
		levelBox.levels.Add(new CampaignLevel("Castle_campaign3", "Key_1317"));
		levelBox.levels.Add(new CampaignLevel("Space_campaign3", "Key_0519"));
		levelBox.levels.Add(new CampaignLevel("Parkour", "Key_1318"));
		levelBox.levels.Add(new CampaignLevel("Code_campaign3", "Key_0845"));
		LevelBox levelBox2 = new LevelBox
		{
			starsToOpen = 20,
			name = "minecraft",
			mapName = string.Empty,
			PreviewNAme = "Box_2",
			CompletionExperienceAward = 70,
			gems = 10,
			coins = 15
		};
		CampaignLevel item2 = new CampaignLevel
		{
			sceneName = "Utopia",
			localizeKeyForLevelMap = "Key_0841",
			predlog = "in"
		};
		CampaignLevel item3 = new CampaignLevel
		{
			sceneName = "Maze",
			localizeKeyForLevelMap = "Key_0842",
			predlog = "in"
		};
		CampaignLevel item4 = new CampaignLevel
		{
			sceneName = "Sky_islands",
			localizeKeyForLevelMap = "Key_0843",
			predlog = "on"
		};
		CampaignLevel item5 = new CampaignLevel
		{
			sceneName = "Winter",
			localizeKeyForLevelMap = "Key_0844",
			predlog = "on"
		};
		CampaignLevel item6 = new CampaignLevel
		{
			sceneName = "Castle",
			localizeKeyForLevelMap = "Key_0845",
			predlog = "in"
		};
		CampaignLevel item7 = new CampaignLevel
		{
			sceneName = "Gluk_2",
			localizeKeyForLevelMap = "Key_0846",
			predlog = "in"
		};
		levelBox2.levels.Add(item2);
		levelBox2.levels.Add(item3);
		levelBox2.levels.Add(item4);
		levelBox2.levels.Add(item5);
		levelBox2.levels.Add(item6);
		levelBox2.levels.Add(item7);
		LevelBox levelBox3 = new LevelBox
		{
			starsToOpen = 0,
			name = "Real",
			mapName = string.Empty,
			PreviewNAme = "Box_1",
			CompletionExperienceAward = 50,
			gems = 5,
			coins = 15
		};
		CampaignLevel item8 = new CampaignLevel
		{
			sceneName = "Farm",
			localizeKeyForLevelMap = "Key_0832",
			predlog = "at"
		};
		CampaignLevel item9 = new CampaignLevel
		{
			sceneName = "Cementery",
			localizeKeyForLevelMap = "Key_0833",
			predlog = "in"
		};
		CampaignLevel item10 = new CampaignLevel
		{
			sceneName = "City",
			localizeKeyForLevelMap = "Key_0834",
			predlog = "in"
		};
		CampaignLevel item11 = new CampaignLevel
		{
			sceneName = "Hospital",
			localizeKeyForLevelMap = "Key_0835",
			predlog = "in"
		};
		CampaignLevel item12 = new CampaignLevel
		{
			sceneName = "Bridge",
			localizeKeyForLevelMap = "Key_0836",
			predlog = "on"
		};
		CampaignLevel item13 = new CampaignLevel
		{
			sceneName = "Jail",
			localizeKeyForLevelMap = "Key_0837",
			predlog = "at"
		};
		CampaignLevel item14 = new CampaignLevel
		{
			sceneName = "Slender",
			localizeKeyForLevelMap = "Key_0838",
			predlog = "in"
		};
		CampaignLevel item15 = new CampaignLevel
		{
			sceneName = "Area52",
			localizeKeyForLevelMap = "Key_0839",
			predlog = "at"
		};
		CampaignLevel item16 = new CampaignLevel
		{
			sceneName = "School",
			localizeKeyForLevelMap = "Key_0840",
			predlog = "in"
		};
		levelBox3.levels.Add(item8);
		levelBox3.levels.Add(item9);
		levelBox3.levels.Add(item10);
		levelBox3.levels.Add(item11);
		levelBox3.levels.Add(item12);
		levelBox3.levels.Add(item13);
		levelBox3.levels.Add(item14);
		levelBox3.levels.Add(item15);
		levelBox3.levels.Add(item16);
		campaignBoxes.Add(levelBox3);
		campaignBoxes.Add(levelBox2);
		campaignBoxes.Add(levelBox);
		campaignBoxes.Add(item);
	}

	public static CampaignLevel GetLevelBySceneName(string sceneName)
	{
		//Discarded unreachable code: IL_004b, IL_0073
		try
		{
			return campaignBoxes.SelectMany((LevelBox levelBox) => levelBox.levels).FirstOrDefault((CampaignLevel level) => level.sceneName == sceneName);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return campaignBoxes[0].levels[0];
		}
	}

	private static void InitializeWeaponsFromBosses(Dictionary<string, string> weaponsFromBosses)
	{
		weaponsFromBosses.Add("Farm", WeaponManager.UZI_WN);
		weaponsFromBosses.Add("Cementery", WeaponManager.MP5WN);
		weaponsFromBosses.Add("City", "Weapon4");
		weaponsFromBosses.Add("Hospital", "Weapon8");
		weaponsFromBosses.Add("Jail", "Weapon5");
		weaponsFromBosses.Add("Slender", "Weapon51");
		weaponsFromBosses.Add("Area52", "Weapon52");
		weaponsFromBosses.Add("Bridge", WeaponManager.M16_2WN);
		weaponsFromBosses.Add("Utopia", WeaponManager.CampaignRifle_WN);
		weaponsFromBosses.Add("Maze", WeaponManager.SimpleFlamethrower_WN);
		weaponsFromBosses.Add("Sky_islands", WeaponManager.Rocketnitza_WN);
		weaponsFromBosses.Add("Code_campaign3", WeaponManager.BugGunWN);
	}
}

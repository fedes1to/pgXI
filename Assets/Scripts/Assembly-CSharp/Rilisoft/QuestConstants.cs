using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft.NullExtensions;
using UnityEngine;

namespace Rilisoft
{
	internal static class QuestConstants
	{
		public const string AddFriend = "addFriend";

		public const string GetGotcha = "getGotcha";

		public const string BreakSeries = "breakSeries";

		public const string CaptureFlags = "captureFlags";

		public const string CapturePoints = "capturePoints";

		public const string JoinClan = "joinClan";

		public const string KillFlagCarriers = "killFlagCarriers";

		public const string KillInCampaign = "killInCampaign";

		public const string KillInMode = "killInMode";

		public const string KillNpcWithWeapon = "killNpcWithWeapon";

		public const string KillViaHeadshot = "killViaHeadshot";

		public const string KillWithGrenade = "killWithGrenade";

		public const string KillWithWeapon = "killWithWeapon";

		public const string LikeFacebook = "likeFacebook";

		public const string LoginFacebook = "loginFacebook";

		public const string LoginTwitter = "loginTwitter";

		public const string MakeSeries = "makeSeries";

		public const string Revenge = "revenge";

		public const string SurviveWavesInArena = "surviveWavesInArena";

		public const string WinInMap = "winInMap";

		public const string WinInMode = "winInMode";

		public const string AnalyticsEventName = "Daily Quests";

		private static readonly HashSet<string> _supportedQuests = new HashSet<string>(new string[14]
		{
			"breakSeries", "killFlagCarriers", "killInCampaign", "killInMode", "killNpcWithWeapon", "killViaHeadshot", "killWithWeapon", "makeSeries", "revenge", "surviveWavesInArena",
			"winInMap", "winInMode", "captureFlags", "capturePoints"
		});

		private static readonly Dictionary<string, ShopNGUIController.CategoryNames> _weaponSlots = new Dictionary<string, ShopNGUIController.CategoryNames>
		{
			{
				"Backup",
				ShopNGUIController.CategoryNames.BackupCategory
			},
			{
				"Melee",
				ShopNGUIController.CategoryNames.MeleeCategory
			},
			{
				"Premium",
				ShopNGUIController.CategoryNames.PremiumCategory
			},
			{
				"Primary",
				ShopNGUIController.CategoryNames.PrimaryCategory
			},
			{
				"Sniper",
				ShopNGUIController.CategoryNames.SniperCategory
			},
			{
				"Special",
				ShopNGUIController.CategoryNames.SpecilCategory
			}
		};

		private static readonly Dictionary<string, string> localizationQuests = new Dictionary<string, string>
		{
			{ "addFriend", "Key_1894" },
			{ "getGotcha", "Key_2429" },
			{ "breakSeries", "Key_1709" },
			{ "captureFlags", "Key_1704" },
			{ "capturePoints", "Key_1703" },
			{ "joinClan", "Key_1895" },
			{ "killFlagCarriers", "Key_1702" },
			{ "killInCampaign", "Key_1712" },
			{ "killInMode", "Key_1701" },
			{ "killNpcWithWeapon", "Key_1713" },
			{ "killViaHeadshot", "Key_1706" },
			{ "killWithGrenade", "Key_1707" },
			{ "killWithWeapon", "Key_1705" },
			{ "likeFacebook", "Key_1892" },
			{ "loginFacebook", "Key_1891" },
			{ "loginTwitter", "Key_1893" },
			{ "makeSeries", "Key_1710" },
			{ "revenge", "Key_1708" },
			{ "surviveWavesInArena", "Key_1711" },
			{ "winInMap", "Key_1700" },
			{ "winInMode", "Key_1699" }
		};

		internal static string GetDifficultyKey(Difficulty difficulty)
		{
			return difficulty.ToString().ToLowerInvariant();
		}

		internal static bool IsSupported(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			return _supportedQuests.Contains(id);
		}

		internal static string[] GetSupportedQuests()
		{
			return _supportedQuests.ToArray();
		}

		internal static ShopNGUIController.CategoryNames? ParseWeaponSlot(string weaponSlot)
		{
			//Discarded unreachable code: IL_0050, IL_0065
			if (string.IsNullOrEmpty(weaponSlot))
			{
				return null;
			}
			ShopNGUIController.CategoryNames value;
			if (_weaponSlots.TryGetValue(weaponSlot, out value))
			{
				return value;
			}
			try
			{
				return (ShopNGUIController.CategoryNames)(int)Enum.Parse(typeof(ShopNGUIController.CategoryNames), weaponSlot);
			}
			catch
			{
				return null;
			}
		}

		internal static ConnectSceneNGUIController.RegimGame? ParseMode(string mode)
		{
			//Discarded unreachable code: IL_0037, IL_004c
			if (string.IsNullOrEmpty(mode))
			{
				return null;
			}
			try
			{
				return (ConnectSceneNGUIController.RegimGame)(int)Enum.Parse(typeof(ConnectSceneNGUIController.RegimGame), mode);
			}
			catch
			{
				return null;
			}
		}

		public static string GetAccumulativeQuestDescriptionByType(AccumulativeQuestBase quest)
		{
			//Discarded unreachable code: IL_0203, IL_0211
			string value;
			localizationQuests.TryGetValue(quest.Id, out value);
			string text = value.Map(LocalizationStore.Get, "{0}");
			ModeAccumulativeQuest modeAccumulativeQuest = quest as ModeAccumulativeQuest;
			if (modeAccumulativeQuest != null)
			{
				string value2;
				if (!ConnectSceneNGUIController.gameModesLocalizeKey.TryGetValue(Convert.ToInt32(modeAccumulativeQuest.Mode).ToString(), out value2))
				{
					value2 = modeAccumulativeQuest.Mode.ToString();
					Debug.LogError("Couldnot find mode name for " + modeAccumulativeQuest.Mode);
				}
				return string.Format(text, string.Format("[fff600]{0}[-]", modeAccumulativeQuest.RequiredCount), string.Format("[ff9600]{0}[-]", LocalizationStore.Get(value2)));
			}
			MapAccumulativeQuest mapAccumulativeQuest = quest as MapAccumulativeQuest;
			if (mapAccumulativeQuest != null)
			{
				SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(mapAccumulativeQuest.Map);
				string empty = string.Empty;
				if (infoScene == null)
				{
					empty = mapAccumulativeQuest.Map;
					Debug.LogError("Couldnot find map name for " + mapAccumulativeQuest.Map);
				}
				else
				{
					empty = infoScene.TranslateName;
				}
				return string.Format(text, string.Format("[fff600]{0}[-]", mapAccumulativeQuest.RequiredCount), string.Format("[ff9600]{0}[-]", empty));
			}
			WeaponSlotAccumulativeQuest weaponSlotAccumulativeQuest = quest as WeaponSlotAccumulativeQuest;
			if (weaponSlotAccumulativeQuest != null)
			{
				string value3;
				if (!ShopNGUIController.weaponCategoryLocKeys.TryGetValue(weaponSlotAccumulativeQuest.WeaponSlot.ToString(), out value3))
				{
					value3 = weaponSlotAccumulativeQuest.WeaponSlot.ToString().Replace("Category", string.Empty);
					Debug.LogError("Couldnot find weapon name for " + weaponSlotAccumulativeQuest.WeaponSlot);
				}
				return string.Format(text, string.Format("[fff600]{0}[-]", weaponSlotAccumulativeQuest.RequiredCount), string.Format("[ff9600]{0}[-]", LocalizationStore.Get(value3)));
			}
			try
			{
				return string.Format(text, string.Format("[fff600]{0}[-]", quest.RequiredCount));
			}
			catch (FormatException)
			{
				return text;
			}
		}
	}
}

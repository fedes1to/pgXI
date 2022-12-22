using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	internal struct QuestImage
	{
		private HashSet<string> _arenaQuests;

		private HashSet<string> _campaignQuests;

		private HashSet<string> _modeQuests;

		private HashSet<string> _weaponQuests;

		private Dictionary<ConnectSceneNGUIController.RegimGame, string> _mapModeToSpriteName;

		private Dictionary<ShopNGUIController.CategoryNames, string> _mapWeaponToSpriteName;

		private static readonly Color s_defaultColor = new Color32(0, 253, 53, byte.MaxValue);

		private static readonly QuestImage s_instance = default(QuestImage);

		public static QuestImage Instance
		{
			get
			{
				return s_instance;
			}
		}

		private HashSet<string> CampaignQuests
		{
			get
			{
				if (_campaignQuests == null)
				{
					_campaignQuests = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					_campaignQuests.Add("killInCampaign");
					_campaignQuests.Add("killNpcWithWeapon");
				}
				return _campaignQuests;
			}
		}

		private HashSet<string> ArenaQuests
		{
			get
			{
				if (_arenaQuests == null)
				{
					_arenaQuests = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					_arenaQuests.Add("surviveWavesInArena");
				}
				return _arenaQuests;
			}
		}

		private Dictionary<ConnectSceneNGUIController.RegimGame, string> MapModeToSpriteName
		{
			get
			{
				if (_mapModeToSpriteName == null)
				{
					_mapModeToSpriteName = new Dictionary<ConnectSceneNGUIController.RegimGame, string>(new GameRegimeComparer())
					{
						{
							ConnectSceneNGUIController.RegimGame.Deathmatch,
							"mode_death_znachek"
						},
						{
							ConnectSceneNGUIController.RegimGame.TimeBattle,
							"mode_time_znachek"
						},
						{
							ConnectSceneNGUIController.RegimGame.TeamFight,
							"mode_team_znachek"
						},
						{
							ConnectSceneNGUIController.RegimGame.DeadlyGames,
							"mode_deadly_games_znachek"
						},
						{
							ConnectSceneNGUIController.RegimGame.FlagCapture,
							"mode_flag_znachek"
						},
						{
							ConnectSceneNGUIController.RegimGame.CapturePoints,
							"mode_capture_point"
						}
					};
				}
				return _mapModeToSpriteName;
			}
		}

		private Dictionary<ShopNGUIController.CategoryNames, string> MapWeaponToSpriteName
		{
			get
			{
				if (_mapWeaponToSpriteName == null)
				{
					_mapWeaponToSpriteName = new Dictionary<ShopNGUIController.CategoryNames, string>(6, ShopNGUIController.CategoryNameComparer.Instance)
					{
						{
							ShopNGUIController.CategoryNames.BackupCategory,
							"shop_icons_backup"
						},
						{
							ShopNGUIController.CategoryNames.MeleeCategory,
							"shop_icons_melee"
						},
						{
							ShopNGUIController.CategoryNames.PremiumCategory,
							"shop_icons_premium"
						},
						{
							ShopNGUIController.CategoryNames.PrimaryCategory,
							"shop_icons_primary"
						},
						{
							ShopNGUIController.CategoryNames.SniperCategory,
							"shop_icons_sniper"
						},
						{
							ShopNGUIController.CategoryNames.SpecilCategory,
							"shop_icons_special"
						}
					};
				}
				return _mapWeaponToSpriteName;
			}
		}

		public Color GetColor(QuestBase quest)
		{
			if (quest == null)
			{
				return s_defaultColor;
			}
			if (CampaignQuests.Contains(quest.Id))
			{
				return new Color32(byte.MaxValue, 184, 0, byte.MaxValue);
			}
			if (ArenaQuests.Contains(quest.Id))
			{
				return new Color32(byte.MaxValue, 121, 0, byte.MaxValue);
			}
			return s_defaultColor;
		}

		public string GetSpriteName(QuestBase quest)
		{
			if (quest == null)
			{
				return GetSpriteNameForMultiplayer();
			}
			ModeAccumulativeQuest modeAccumulativeQuest = quest as ModeAccumulativeQuest;
			if (modeAccumulativeQuest != null)
			{
				return GetSpriteNameForMultiplayer(modeAccumulativeQuest.Mode);
			}
			WeaponSlotAccumulativeQuest weaponSlotAccumulativeQuest = quest as WeaponSlotAccumulativeQuest;
			if (weaponSlotAccumulativeQuest != null)
			{
				if (CampaignQuests.Contains(quest.Id))
				{
					return GetSpriteNameForCampaign(weaponSlotAccumulativeQuest.WeaponSlot);
				}
				return GetSpriteNameForMultiplayer(weaponSlotAccumulativeQuest.WeaponSlot);
			}
			if (ArenaQuests.Contains(quest.Id))
			{
				return GetSpriteNameForArena();
			}
			if (CampaignQuests.Contains(quest.Id))
			{
				return GetSpriteNameForCampaign();
			}
			return GetSpriteNameForMultiplayer();
		}

		private string GetSpriteNameForMultiplayer()
		{
			return "battle_now_znachek";
		}

		private string GetSpriteNameForMultiplayer(ConnectSceneNGUIController.RegimGame mode)
		{
			string value;
			if (MapModeToSpriteName.TryGetValue(mode, out value))
			{
				return value;
			}
			return GetSpriteNameForMultiplayer();
		}

		private string GetSpriteNameForMultiplayer(ShopNGUIController.CategoryNames weapon)
		{
			string value;
			if (MapWeaponToSpriteName.TryGetValue(weapon, out value))
			{
				return value;
			}
			return GetSpriteNameForMultiplayer();
		}

		private string GetSpriteNameForCampaign()
		{
			return "star";
		}

		private string GetSpriteNameForCampaign(ShopNGUIController.CategoryNames weapon)
		{
			string value;
			if (MapWeaponToSpriteName.TryGetValue(weapon, out value))
			{
				return value;
			}
			return GetSpriteNameForCampaign();
		}

		private string GetSpriteNameForArena()
		{
			return "mode_arena";
		}
	}
}

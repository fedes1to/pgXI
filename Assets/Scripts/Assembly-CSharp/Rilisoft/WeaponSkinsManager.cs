using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class WeaponSkinsManager
	{
		private static List<WeaponSkin> _allSkins;

		public static List<string> SkinIds = new List<string>
		{
			"weaponskin_acid_canon", "weaponskin_antihero_rifle", "weaponskin_dragon_breath", "weaponskin_ghost_lantern", "weaponskin_loud_piggy", "weaponskin_peacemaker", "weaponskin_prototype", "weaponskin_secret_forces_rifle", "weaponskin_shotgun_pistol", "weaponskin_steam_revolver",
			"weaponskin_storm_hammer", "weaponskin_toy_bomber"
		};

		public static List<WeaponSkin> AllSkins
		{
			get
			{
				if (_allSkins == null)
				{
					_allSkins = LoadSkins();
					_allSkins.Sort(delegate(WeaponSkin ws1, WeaponSkin ws2)
					{
						int num = ws1.ForLeague.CompareTo(ws2.ForLeague);
						return (num == 0) ? ws1.Price.CompareTo(ws2.Price) : num;
					});
				}
				return _allSkins;
			}
		}

		public static event Action<ItemRecord, string> EquippedSkinForWeapon;

		static WeaponSkinsManager()
		{
			WeaponSkinsManager.EquippedSkinForWeapon = null;
		}

		public static WeaponSkin GetSkin(string skinId)
		{
			return AllSkins.FirstOrDefault((WeaponSkin s) => s.Id == skinId);
		}

		public static List<WeaponSkin> SkinsForWeapon(string weaponName)
		{
			ItemRecord byPrefabName = ItemDb.GetByPrefabName(weaponName);
			if (byPrefabName != null)
			{
				List<string> list = WeaponUpgrades.ChainForTag(byPrefabName.Tag);
				if (list == null)
				{
					return AllSkins.Where((WeaponSkin s) => s.ToWeapons[0] == weaponName).ToList();
				}
				ItemRecord recOfFirstUpgrade = ItemDb.GetByTag(list[0]);
				return AllSkins.Where((WeaponSkin s) => s.ToWeapons[0] == recOfFirstUpgrade.PrefabName).ToList();
			}
			return AllSkins.Where((WeaponSkin s) => s.ToWeapons[0] == weaponName).ToList();
		}

		public static bool UnequipSkin(string skinId)
		{
			WeaponSkin skin = GetSkin(skinId);
			if (skin == null)
			{
				Debug.LogError("WeaponSkinsManager UnequipSkin weaponSkinInfo == null, skinId = " + skinId);
				return false;
			}
			ItemRecord byPrefabName = ItemDb.GetByPrefabName(skin.ToWeapons[0]);
			if (byPrefabName == null)
			{
				Debug.LogError("WeaponSkinsManager UnequipSkin itemRecord == null, skinId = " + skinId);
				return false;
			}
			Storager.setString(StoragerNameForEquippedSkinForWeapon(byPrefabName.Tag), string.Empty, false);
			Action<ItemRecord, string> equippedSkinForWeapon = WeaponSkinsManager.EquippedSkinForWeapon;
			if (equippedSkinForWeapon != null)
			{
				equippedSkinForWeapon(byPrefabName, string.Empty);
			}
			UpdateWeaponSkins(byPrefabName.Tag);
			return true;
		}

		public static bool SetSkinToCurrentWeapon(string skinId)
		{
			WeaponSkin skin = GetSkin(skinId);
			if (skin == null)
			{
				return false;
			}
			if (Storager.getInt(skin.Id, true) < 1)
			{
				return false;
			}
			if (WeaponManager.sharedManager == null || WeaponManager.sharedManager.currentWeaponSounds == null)
			{
				return false;
			}
			WeaponSounds currentWeaponSounds = WeaponManager.sharedManager.currentWeaponSounds;
			ItemRecord byPrefabName = ItemDb.GetByPrefabName(currentWeaponSounds.nameNoClone());
			if (byPrefabName == null)
			{
				return false;
			}
			List<string> list = WeaponUpgrades.ChainForTag(byPrefabName.Tag);
			ItemRecord itemRecord = byPrefabName;
			if (list != null)
			{
				itemRecord = ItemDb.GetByTag(list[0]);
			}
			if (skin.ToWeapons[0] != itemRecord.PrefabName)
			{
				return false;
			}
			bool flag = skin.SetTo(WeaponManager.sharedManager.currentWeaponSounds.gameObject);
			if (flag)
			{
				Storager.setString(StoragerNameForEquippedSkinForWeapon(itemRecord.Tag), skin.Id, false);
				UpdateWeaponSkins(byPrefabName.Tag);
			}
			return flag;
		}

		public static bool SetSkinToWeapon(string skinId, string weaponName)
		{
			if (!IsBoughtSkin(skinId))
			{
				return false;
			}
			ItemRecord byPrefabName = ItemDb.GetByPrefabName(weaponName);
			if (byPrefabName == null)
			{
				return false;
			}
			string text = byPrefabName.Tag;
			List<string> list = WeaponUpgrades.ChainForTag(text);
			if (list != null)
			{
				text = list[0];
			}
			Storager.setString(StoragerNameForEquippedSkinForWeapon(text), skinId, false);
			Action<ItemRecord, string> equippedSkinForWeapon = WeaponSkinsManager.EquippedSkinForWeapon;
			if (equippedSkinForWeapon != null)
			{
				equippedSkinForWeapon(ItemDb.GetByTag(text), skinId);
			}
			UpdateWeaponSkins(text);
			return true;
		}

		private static void UpdateWeaponSkins(string weaponTag = null)
		{
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
			{
				WeaponManager.sharedManager.myPlayerMoveC.ChangeWeapon(WeaponManager.sharedManager.CurrentWeaponIndex, false);
			}
			if (weaponTag != null && ShopNGUIController.sharedShop != null)
			{
				ShopNGUIController.sharedShop.SetWeapon(weaponTag, null);
			}
			if (PersConfigurator.currentConfigurator != null)
			{
				PersConfigurator.currentConfigurator.ResetWeapon();
			}
			if (InGameGUI.sharedInGameGUI != null)
			{
				InGameGUI.sharedInGameGUI.UpdateWeaponIconsForWrap();
			}
		}

		public static WeaponSkin GetSkinForWeapon(string weaponName)
		{
			string settedSkinId = GetSettedSkinId(weaponName);
			if (settedSkinId.IsNullOrEmpty())
			{
				return null;
			}
			return GetSkin(settedSkinId);
		}

		public static string StoragerNameForEquippedSkinForWeapon(string weaponTag)
		{
			return weaponTag + "_skin";
		}

		public static string GetSettedSkinId(string weaponName)
		{
			ItemRecord byPrefabName = ItemDb.GetByPrefabName(weaponName);
			if (byPrefabName == null)
			{
				return null;
			}
			string tag = byPrefabName.Tag;
			return GetSettedSkinIdByWeaponTag(tag);
		}

		public static string GetSettedSkinIdByWeaponTag(string weaponTag)
		{
			List<string> list = WeaponUpgrades.ChainForTag(weaponTag);
			if (list != null)
			{
				weaponTag = list[0];
			}
			return Storager.getString(StoragerNameForEquippedSkinForWeapon(weaponTag), false);
		}

		public static List<WeaponSkin> BoughtSkins()
		{
			return AllSkins.Where((WeaponSkin s) => IsBoughtSkin(s.Id)).ToList();
		}

		public static bool IsBoughtSkin(string skinId)
		{
			return Storager.getInt(skinId, true) > 0;
		}

		public static bool ProvideSkin(string skinId)
		{
			if (!AllSkins.Any((WeaponSkin s) => s.Id == skinId))
			{
				return false;
			}
			Storager.setInt(skinId, 1, true);
			return true;
		}

		public static List<WeaponSkin> GetAvailableForBuySkins()
		{
			return AllSkins.Where((WeaponSkin s) => AvailableForBuy(s)).ToList();
		}

		public static bool AvailableForBuy(WeaponSkin skin)
		{
			return skin != null && !IsBoughtSkin(skin.Id) && skin.ForLeague <= RatingSystem.instance.currentLeague;
		}

		public static bool IsAvailableByLeague(string skinId)
		{
			if (skinId.IsNullOrEmpty())
			{
				Debug.LogErrorFormat("IsAvailableByLeague: skinId.isNullOrEmpty()");
				return true;
			}
			WeaponSkin skin = GetSkin(skinId);
			if (skin == null)
			{
				Debug.LogErrorFormat("IsAvailableByLeague: skin == null , skinId = {0}", skinId);
				return true;
			}
			return IsAvailableByLeague(skin);
		}

		public static bool IsAvailableByLeague(WeaponSkin skin)
		{
			return skin != null && skin.ForLeague <= RatingSystem.instance.currentLeague;
		}

		public static List<WeaponSkin> SkinsForLeague(RatingSystem.RatingLeague league)
		{
			return AllSkins.Where((WeaponSkin s) => s.ForLeague == league).ToList();
		}

		private static List<WeaponSkin> LoadSkins()
		{
			WeaponSkinsData weaponSkinsData = Resources.Load<WeaponSkinsData>("WeaponSkins/weapon_skins_data");
			if (weaponSkinsData == null)
			{
				Debug.LogError("[WEAPON_SKINS] skins data not found");
				return null;
			}
			return weaponSkinsData.Data;
		}
	}
}

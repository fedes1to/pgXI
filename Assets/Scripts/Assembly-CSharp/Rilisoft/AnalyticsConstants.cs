using System.Collections.Generic;

namespace Rilisoft
{
	public sealed class AnalyticsConstants
	{
		public enum AccrualType
		{
			Earned,
			Purchased
		}

		public enum TutorialState
		{
			Started = 1,
			Controls_Overview = 2,
			Controls_Move = 3,
			Controls_Jump = 4,
			Kill_Enemy = 5,
			Portal = 6,
			Rewards = 7,
			Open_Shop = 8,
			Category_Sniper = 9,
			Equip_Sniper = 10,
			Category_Armor = 11,
			Equip_Armor = 12,
			Back_Shop = 13,
			Connect_Scene = 14,
			Table_Battle = 15,
			Battle_Start = 16,
			Battle_End = 17,
			Finished = 18,
			Get_Progress = 100
		}

		public const string AppsFlyerInitiatedCheckout = "af_initiated_checkout";

		public const string LevelUp = "LevelUp";

		public const string SocialEvent = "Social";

		public const string ViralityEvent = "Virality";

		private static readonly Dictionary<ShopNGUIController.CategoryNames, string> _shopCategoryToLogSalesNamesMapping = new Dictionary<ShopNGUIController.CategoryNames, string>(21, ShopNGUIController.CategoryNameComparer.Instance)
		{
			{
				ShopNGUIController.CategoryNames.PrimaryCategory,
				"Primary"
			},
			{
				ShopNGUIController.CategoryNames.BackupCategory,
				"Back Up"
			},
			{
				ShopNGUIController.CategoryNames.MeleeCategory,
				"Melee"
			},
			{
				ShopNGUIController.CategoryNames.SpecilCategory,
				"Special"
			},
			{
				ShopNGUIController.CategoryNames.SniperCategory,
				"Sniper"
			},
			{
				ShopNGUIController.CategoryNames.PremiumCategory,
				"Premium"
			},
			{
				ShopNGUIController.CategoryNames.SkinsCategory,
				"Skins"
			},
			{
				ShopNGUIController.CategoryNames.ArmorCategory,
				"Armor"
			},
			{
				ShopNGUIController.CategoryNames.BootsCategory,
				"Boots"
			},
			{
				ShopNGUIController.CategoryNames.CapesCategory,
				"Capes"
			},
			{
				ShopNGUIController.CategoryNames.HatsCategory,
				"Hats"
			},
			{
				ShopNGUIController.CategoryNames.GearCategory,
				"Gear"
			},
			{
				ShopNGUIController.CategoryNames.MaskCategory,
				"Masks"
			},
			{
				ShopNGUIController.CategoryNames.SkinsCategoryEditor,
				"Skins Category Editor"
			},
			{
				ShopNGUIController.CategoryNames.SkinsCategoryFemale,
				"SkinsCategoryFamele"
			},
			{
				ShopNGUIController.CategoryNames.SkinsCategoryMale,
				"SkinsCategoryMale"
			},
			{
				ShopNGUIController.CategoryNames.SkinsCategoryPremium,
				"SkinsCategoryPremium"
			},
			{
				ShopNGUIController.CategoryNames.SkinsCategorySpecial,
				"SkinsCategorySpecial"
			},
			{
				ShopNGUIController.CategoryNames.LeagueHatsCategory,
				"League Hats"
			},
			{
				ShopNGUIController.CategoryNames.LeagueSkinsCategory,
				"League Skins"
			},
			{
				ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory,
				"League Weapon Skins"
			},
			{
				ShopNGUIController.CategoryNames.ThrowingCategory,
				"Throwing Gadgets"
			},
			{
				ShopNGUIController.CategoryNames.ToolsCategoty,
				"Tool Gadgets"
			},
			{
				ShopNGUIController.CategoryNames.SupportCategory,
				"Support Gadgets"
			},
			{
				ShopNGUIController.CategoryNames.PetsCategory,
				"Pets"
			},
			{
				ShopNGUIController.CategoryNames.EggsCategory,
				"Eggs"
			},
			{
				ShopNGUIController.CategoryNames.BestWeapons,
				"Best Weapons"
			},
			{
				ShopNGUIController.CategoryNames.BestWear,
				"Best Wear"
			},
			{
				ShopNGUIController.CategoryNames.BestGadgets,
				"Best Gadgets"
			}
		};

		internal static string GetSalesName(ShopNGUIController.CategoryNames category)
		{
			string value;
			if (_shopCategoryToLogSalesNamesMapping.TryGetValue(category, out value))
			{
				return value;
			}
			return null;
		}

		internal static IEnumerable<string> GetSalesNames()
		{
			return _shopCategoryToLogSalesNamesMapping.Values;
		}
	}
}

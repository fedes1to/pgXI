using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class AchievementShooting : Achievement
	{
		public AchievementShooting(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (base._data.WeaponCategories == null && !base._data.WeaponCategories.Any())
			{
				Debug.LogErrorFormat("achievement '{0}' without value", base._data.Id);
			}
			else
			{
				Player_move_c.OnMyShootingStateSchanged += Player_move_c_OnMyShootingStateSchanged;
			}
		}

		private void Player_move_c_OnMyShootingStateSchanged(bool obj)
		{
			if (obj && WeaponManager.sharedManager != null && WeaponManager.sharedManager.currentWeaponSounds != null)
			{
				ShopNGUIController.CategoryNames value = (ShopNGUIController.CategoryNames)(WeaponManager.sharedManager.currentWeaponSounds.categoryNabor - 1);
				if (base._data.WeaponCategories.Contains(value))
				{
					Gain(1);
				}
			}
		}

		public override void Dispose()
		{
			Player_move_c.OnMyShootingStateSchanged -= Player_move_c_OnMyShootingStateSchanged;
		}
	}
}

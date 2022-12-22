using UnityEngine;

namespace Rilisoft
{
	public class AchievementKillPlayerThroughWeaponCategory : Achievement
	{
		public AchievementKillPlayerThroughWeaponCategory(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (!base._data.WeaponCategory.HasValue)
			{
				Debug.LogErrorFormat("achievement '{0}' without value", base._data.Id);
				return;
			}
			QuestMediator.Events.KillOtherPlayer += delegate(object sender, KillOtherPlayerEventArgs e)
			{
				if (e.WeaponSlot == base._data.WeaponCategory.Value)
				{
					Gain(1);
				}
			};
		}
	}
}

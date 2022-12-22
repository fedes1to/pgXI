using UnityEngine;

namespace Rilisoft
{
	public class AchievementKillPlayerWhenHpEqualsOne : Achievement
	{
		public AchievementKillPlayerWhenHpEqualsOne(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (!base.IsCompleted)
			{
				QuestMediator.Events.KillOtherPlayer += QuestMediator_Events_KillOtherPlayer;
			}
		}

		private void QuestMediator_Events_KillOtherPlayer(object sender, KillOtherPlayerEventArgs e)
		{
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null && Mathf.RoundToInt(WeaponManager.sharedManager.myPlayerMoveC.CurHealth) == 1)
			{
				Gain(1);
			}
		}

		public override void Dispose()
		{
			QuestMediator.Events.KillOtherPlayer -= QuestMediator_Events_KillOtherPlayer;
		}
	}
}

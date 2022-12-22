using System;

namespace Rilisoft
{
	public class AchievementTurretKill : Achievement
	{
		public AchievementTurretKill(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.TurretKill += QuestMediator_Events_TurretKill;
		}

		private void QuestMediator_Events_TurretKill(object sender, EventArgs e)
		{
			Gain(1);
		}

		public override void Dispose()
		{
			QuestMediator.Events.TurretKill -= QuestMediator_Events_TurretKill;
		}
	}
}

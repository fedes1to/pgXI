namespace Rilisoft
{
	public class AchievementKillAtFly : Achievement
	{
		public AchievementKillAtFly(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.KillOtherPlayerOnFly += QuestMediator_Events_KillOtherPlayerOnFly;
		}

		private void QuestMediator_Events_KillOtherPlayerOnFly(object sender, KillOtherPlayerOnFlyEventArgs e)
		{
			if (e.IamFly && e.KilledPlayerFly)
			{
				Gain(1);
			}
		}

		public override void Dispose()
		{
			QuestMediator.Events.KillOtherPlayerOnFly -= QuestMediator_Events_KillOtherPlayerOnFly;
		}
	}
}

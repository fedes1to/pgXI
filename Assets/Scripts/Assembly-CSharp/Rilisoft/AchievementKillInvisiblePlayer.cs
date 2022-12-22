namespace Rilisoft
{
	public class AchievementKillInvisiblePlayer : Achievement
	{
		public AchievementKillInvisiblePlayer(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.KillOtherPlayer += QuestMediator_Events_KillOtherPlayer;
		}

		private void QuestMediator_Events_KillOtherPlayer(object sender, KillOtherPlayerEventArgs e)
		{
			if (e.IsInvisible)
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

namespace Rilisoft
{
	public class AchievementKillPlayers : Achievement
	{
		public AchievementKillPlayers(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.KillOtherPlayer += delegate
			{
				Gain(1);
			};
		}
	}
}

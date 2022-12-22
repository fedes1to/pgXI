namespace Rilisoft
{
	public class AchievementKillMobs : Achievement
	{
		public AchievementKillMobs(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.KillMonster += delegate
			{
				Gain(1);
			};
		}
	}
}

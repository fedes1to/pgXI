namespace Rilisoft
{
	public class AchievementWin : Achievement
	{
		public AchievementWin(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.Win += delegate
			{
				Gain(1);
			};
		}
	}
}

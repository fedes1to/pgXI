namespace Rilisoft
{
	public class AchievementGacha : Achievement
	{
		public AchievementGacha(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.GetGotcha += delegate
			{
				Gain(1);
			};
		}
	}
}

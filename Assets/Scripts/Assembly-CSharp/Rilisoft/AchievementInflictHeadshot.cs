namespace Rilisoft
{
	public class AchievementInflictHeadshot : Achievement
	{
		public AchievementInflictHeadshot(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.KillOtherPlayer += delegate(object sender, KillOtherPlayerEventArgs e)
			{
				if (e.Headshot)
				{
					Gain(1);
				}
			};
		}
	}
}

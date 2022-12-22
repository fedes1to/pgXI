namespace Rilisoft
{
	public class AchievementReturnAfterDays : Achievement
	{
		private const string LAST_VISIT_KEY = "lv";

		public AchievementReturnAfterDays(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (!base.IsCompleted)
			{
				FriendsController_ServerTimeUpdated();
				FriendsController.ServerTimeUpdated += FriendsController_ServerTimeUpdated;
			}
		}

		private void FriendsController_ServerTimeUpdated()
		{
			long serverTime = FriendsController.ServerTime;
			if (serverTime < 1)
			{
				return;
			}
			object obj = base.Progress.CustomDataGet("lv");
			if (obj == null)
			{
				obj = serverTime;
				base.Progress.CustomDataSet("lv", obj);
				return;
			}
			base.Progress.CustomDataSet("lv", serverTime);
			long num = serverTime - (long)obj;
			if (num >= 1)
			{
				int num2 = (int)(num / 86400);
				if (base.ToNextStagePoints > 0 && num2 >= base.ToNextStagePoints)
				{
					SetProgress(base.ToNextStagePoints);
				}
			}
		}

		public override void Dispose()
		{
			FriendsController.ServerTimeUpdated -= FriendsController_ServerTimeUpdated;
		}
	}
}

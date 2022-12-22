namespace Rilisoft
{
	public class AchievementJoinToClan : Achievement
	{
		public AchievementJoinToClan(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			FriendsController.OnClanIdSetted += FriendsController_OnClanIdSetted;
		}

		private void FriendsController_OnClanIdSetted(string obj)
		{
			if (base.Progress.Points < base.PointsLeft && !obj.IsNullOrEmpty())
			{
				Gain(1);
			}
		}

		public override void Dispose()
		{
			FriendsController.OnClanIdSetted -= FriendsController_OnClanIdSetted;
		}
	}
}

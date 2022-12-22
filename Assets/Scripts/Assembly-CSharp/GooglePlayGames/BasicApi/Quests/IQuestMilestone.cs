namespace GooglePlayGames.BasicApi.Quests
{
	public interface IQuestMilestone
	{
		string Id { get; }

		string EventId { get; }

		string QuestId { get; }

		ulong CurrentCount { get; }

		ulong TargetCount { get; }

		byte[] CompletionRewardData { get; }

		MilestoneState State { get; }
	}
}

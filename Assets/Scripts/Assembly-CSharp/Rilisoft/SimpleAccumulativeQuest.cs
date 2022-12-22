namespace Rilisoft
{
	public sealed class SimpleAccumulativeQuest : AccumulativeQuestBase
	{
		public SimpleAccumulativeQuest(string id, long day, int slot, Difficulty difficulty, Reward reward, bool active, bool rewarded, int requiredCount, int initialCount = 0)
			: base(id, day, slot, difficulty, reward, active, rewarded, requiredCount, initialCount)
		{
		}
	}
}

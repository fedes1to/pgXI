using System.Collections;

namespace Rilisoft
{
	public class AchievementQuestsComplited : Achievement
	{
		public AchievementQuestsComplited(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			AchievementsManager.Awaiter.Register(WaitQuestSystem());
		}

		private IEnumerator WaitQuestSystem()
		{
			while (QuestSystem.Instance == null)
			{
				yield return null;
			}
			QuestSystem.Instance.QuestCompleted += QuestSystem_Instance_QuestCompleted;
		}

		private void QuestSystem_Instance_QuestCompleted(object sender, QuestCompletedEventArgs e)
		{
			Gain(1);
		}

		public override void Dispose()
		{
			AchievementsManager.Awaiter.Remove(WaitQuestSystem());
			if (QuestSystem.Instance != null)
			{
				QuestSystem.Instance.QuestCompleted -= QuestSystem_Instance_QuestCompleted;
			}
		}
	}
}

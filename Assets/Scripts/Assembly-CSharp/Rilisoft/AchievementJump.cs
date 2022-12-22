using System;

namespace Rilisoft
{
	public class AchievementJump : Achievement
	{
		public AchievementJump(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.Jump += QuestMediator_Events_Jump;
		}

		private void QuestMediator_Events_Jump(object sender, EventArgs e)
		{
			Gain(1);
		}

		public override void Dispose()
		{
			QuestMediator.Events.Jump -= QuestMediator_Events_Jump;
		}
	}
}

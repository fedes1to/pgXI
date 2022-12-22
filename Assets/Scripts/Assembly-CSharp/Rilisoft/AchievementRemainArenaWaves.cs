using System.Linq;

namespace Rilisoft
{
	public class AchievementRemainArenaWaves : Achievement
	{
		public AchievementRemainArenaWaves(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.SurviveWaveInArena += QuestMediator_Events_SurviveWaveInArena;
		}

		private void QuestMediator_Events_SurviveWaveInArena(object sender, SurviveWaveInArenaEventArgs e)
		{
			if (base._data.Thresholds.Contains(e.WaveNumber))
			{
				int stageIdx = base._data.Thresholds.ToList().IndexOf(e.WaveNumber);
				int num = PointsToStage(stageIdx);
				if (num > 0)
				{
					Gain(num);
				}
			}
		}

		public override void Dispose()
		{
			QuestMediator.Events.SurviveWaveInArena -= QuestMediator_Events_SurviveWaveInArena;
		}
	}
}

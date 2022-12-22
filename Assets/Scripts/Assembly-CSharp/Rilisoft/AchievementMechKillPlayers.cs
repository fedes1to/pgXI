using System.Collections;

namespace Rilisoft
{
	public class AchievementMechKillPlayers : Achievement
	{
		private bool _meshStateListen;

		private int _killedCounter;

		public AchievementMechKillPlayers(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			QuestMediator.Events.KillOtherPlayer += QuestMediator_Events_KillOtherPlayer;
		}

		private void QuestMediator_Events_KillOtherPlayer(object sender, KillOtherPlayerEventArgs e)
		{
			if (WeaponManager.sharedManager.myPlayerMoveC.isMechActive)
			{
				_killedCounter++;
				if (!_meshStateListen)
				{
					_meshStateListen = true;
					AchievementsManager.Awaiter.Register(WaitMechOff());
				}
			}
		}

		private IEnumerator WaitMechOff()
		{
			while (WeaponManager.sharedManager != null && (bool)WeaponManager.sharedManager.myPlayerMoveC && WeaponManager.sharedManager.myPlayerMoveC.isMechActive)
			{
				yield return null;
			}
			_meshStateListen = false;
			if (_killedCounter >= base.ToNextStagePointsLeft)
			{
				int stageIdx = MaxStageForPoints(_killedCounter);
				if (stageIdx > -1)
				{
					SetProgress(PointsToStage(stageIdx));
				}
			}
			_killedCounter = 0;
		}

		public override void Dispose()
		{
			QuestMediator.Events.KillOtherPlayer -= QuestMediator_Events_KillOtherPlayer;
		}
	}
}

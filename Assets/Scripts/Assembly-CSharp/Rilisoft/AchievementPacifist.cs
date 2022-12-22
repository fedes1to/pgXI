using System.Collections;
using UnityEngine;

namespace Rilisoft
{
	public class AchievementPacifist : Achievement
	{
		private bool isShooting;

		public AchievementPacifist(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (!base.IsCompleted)
			{
				Player_move_c.OnMyPlayerMoveCCreated += Player_move_c_OnMyPlayerMoveCCreated;
				Player_move_c.OnMyPlayerMoveCDestroyed += Player_move_c_OnMyPlayerMoveCDestroyed;
				Player_move_c.OnMyShootingStateSchanged += Player_move_c_OnMyShootingStateSchanged;
			}
		}

		private void Player_move_c_OnMyPlayerMoveCCreated()
		{
			if (!Defs.isHunger && !Defs.isDaterRegim && Defs.isMulti)
			{
				AchievementsManager.Awaiter.Register(Update());
			}
		}

		private IEnumerator Update()
		{
			if (base.IsCompleted)
			{
				yield break;
			}
			Player_move_c playerMoveC = WeaponManager.sharedManager.myPlayerMoveC;
			while (!isShooting && !(playerMoveC == null))
			{
				if (playerMoveC.liveTime >= (float)base.ToNextStagePointsLeft)
				{
					int stageIdx = MaxStageForPoints(Mathf.RoundToInt(playerMoveC.liveTime));
					if (stageIdx > -1)
					{
						SetProgress(PointsToStage(stageIdx));
					}
					if (base.IsCompleted)
					{
						break;
					}
				}
				yield return null;
			}
		}

		private void Player_move_c_OnMyPlayerMoveCDestroyed(float liveTime)
		{
			isShooting = false;
		}

		private void Player_move_c_OnMyShootingStateSchanged(bool obj)
		{
			AchievementsManager.Awaiter.Remove(Update());
			isShooting = obj;
		}

		public override void Dispose()
		{
			Player_move_c.OnMyPlayerMoveCCreated -= Player_move_c_OnMyPlayerMoveCCreated;
			Player_move_c.OnMyPlayerMoveCDestroyed -= Player_move_c_OnMyPlayerMoveCDestroyed;
			Player_move_c.OnMyShootingStateSchanged -= Player_move_c_OnMyShootingStateSchanged;
		}
	}
}

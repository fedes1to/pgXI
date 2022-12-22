using System.Collections;

namespace Rilisoft
{
	public abstract class AchievementBindedToMyPlayer : Achievement
	{
		private int _lastHash;

		protected Player_move_c MyPlayer
		{
			get
			{
				return (!(WeaponManager.sharedManager != null)) ? null : WeaponManager.sharedManager.myPlayerMoveC;
			}
		}

		public AchievementBindedToMyPlayer(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			AchievementsManager.Awaiter.Register(Tick());
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				if (MyPlayer == null)
				{
					_lastHash = 0;
				}
				if (_lastHash == 0 && MyPlayer != null)
				{
					_lastHash = MyPlayer.GetHashCode();
					OnPlayerInstanceSetted();
				}
				yield return null;
			}
		}

		protected abstract void OnPlayerInstanceSetted();

		public override void Dispose()
		{
			AchievementsManager.Awaiter.Remove(Tick());
		}
	}
}

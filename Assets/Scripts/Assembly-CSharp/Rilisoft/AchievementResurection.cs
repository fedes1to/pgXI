namespace Rilisoft
{
	public class AchievementResurection : AchievementBindedToMyPlayer
	{
		private int _lastHash;

		private new Player_move_c MyPlayer
		{
			get
			{
				return (!(WeaponManager.sharedManager != null)) ? null : WeaponManager.sharedManager.myPlayerMoveC;
			}
		}

		public AchievementResurection(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
		}

		protected override void OnPlayerInstanceSetted()
		{
			if (MyPlayer != null)
			{
				MyPlayer.OnMyPlayerResurected += Player_move_c_OnMyPlayerResurected;
			}
		}

		private void Player_move_c_OnMyPlayerResurected()
		{
			Gain(1);
		}

		public override void Dispose()
		{
			base.Dispose();
			if (MyPlayer != null)
			{
				MyPlayer.OnMyPlayerResurected -= Player_move_c_OnMyPlayerResurected;
			}
		}
	}
}

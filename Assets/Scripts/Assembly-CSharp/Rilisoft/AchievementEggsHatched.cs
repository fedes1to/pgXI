namespace Rilisoft
{
	public class AchievementEggsHatched : Achievement
	{
		public AchievementEggsHatched(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			EggsManager.OnEggHatched += EggsManager_OnEggHatched;
		}

		private void EggsManager_OnEggHatched(Egg egg, PetInfo petInfo)
		{
			Gain(1);
		}

		public override void Dispose()
		{
			EggsManager.OnEggHatched -= EggsManager_OnEggHatched;
		}
	}
}

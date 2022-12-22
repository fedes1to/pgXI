using System.Collections;
using System.Linq;

namespace Rilisoft
{
	public class AchievementCollectPets : Achievement
	{
		public AchievementCollectPets(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			AchievementsManager.Awaiter.Register(WaitPetsManager());
		}

		private IEnumerator WaitPetsManager()
		{
			while (Singleton<PetsManager>.Instance == null)
			{
				yield return null;
			}
			UpdateProgress();
			Singleton<PetsManager>.Instance.OnPlayerPetAdded += PetsManager_Instance_OnPlayerPetAdded;
		}

		private void PetsManager_Instance_OnPlayerPetAdded(string petId)
		{
			UpdateProgress();
		}

		private void UpdateProgress()
		{
			if (!(Singleton<PetsManager>.Instance == null))
			{
				int num = Singleton<PetsManager>.Instance.PlayerPets.Count();
				if (base.Points < num)
				{
					SetProgress(num);
				}
			}
		}

		public override void Dispose()
		{
			if (Singleton<PetsManager>.Instance != null)
			{
				Singleton<PetsManager>.Instance.OnPlayerPetAdded -= PetsManager_Instance_OnPlayerPetAdded;
			}
		}
	}
}

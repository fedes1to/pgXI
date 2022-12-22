namespace Rilisoft
{
	public class PetUpdateInfo
	{
		public PlayerPet PetOld;

		public PlayerPet PetNew;

		public bool PetAdded
		{
			get
			{
				return PetOld == null;
			}
		}

		public bool Upgraded
		{
			get
			{
				return PetOld != null && PetNew != null && PetOld.InfoId != PetNew.InfoId;
			}
		}

		public bool PetPointsAdded
		{
			get
			{
				return PetOld != null && PetNew != null && PetOld.Points != PetNew.Points;
			}
		}

		public PetUpdateInfo()
		{
		}

		public PetUpdateInfo(PlayerPet petOld, PlayerPet petNew)
		{
			PetOld = petOld;
			PetNew = petNew;
		}
	}
}

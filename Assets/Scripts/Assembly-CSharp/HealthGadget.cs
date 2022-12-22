public class HealthGadget : ImmediateGadget
{
	public HealthGadget(GadgetInfo _info)
		: base(_info)
	{
	}

	public override void PreUse()
	{
	}

	public override void Use()
	{
		if (WeaponManager.sharedManager.myPlayerMoveC.ApplyMedkit(Info))
		{
			StartCooldown();
		}
	}

	public override void PostUse()
	{
	}
}

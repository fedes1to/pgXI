public class DisablerGadget : ImmediateGadget
{
	public DisablerGadget(GadgetInfo _info)
		: base(_info)
	{
	}

	public override void PreUse()
	{
	}

	public override void Use()
	{
		StartCooldown();
		WeaponManager.sharedManager.myPlayerMoveC.DisablerGadget(Info);
	}
}

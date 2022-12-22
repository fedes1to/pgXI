public class DragonWhistleGadget : ImmediateGadget
{
	public DragonWhistleGadget(GadgetInfo _info)
		: base(_info)
	{
	}

	public override void PreUse()
	{
	}

	public override void Use()
	{
		WeaponManager.sharedManager.myPlayerMoveC.UseDragonWhistle(Info);
		StartCooldown();
	}
}

public class TimeWatchGadget : ImmediateGadget
{
	public TimeWatchGadget(GadgetInfo _info)
		: base(_info)
	{
	}

	public override void PreUse()
	{
	}

	public override void Use()
	{
		StartCooldown();
		WeaponManager.sharedManager.myPlayerMoveC.ApplyTimeWatch();
	}

	public override void PostUse()
	{
	}
}

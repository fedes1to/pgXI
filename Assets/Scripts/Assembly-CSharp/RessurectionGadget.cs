public class RessurectionGadget : PassiveGadget
{
	public RessurectionGadget(GadgetInfo _info)
		: base(_info)
	{
	}

	public override void PreUse()
	{
	}

	public override void Use()
	{
	}

	public override void PostUse()
	{
	}

	public override void OnKill(bool inDeathCollider)
	{
		StartCooldown();
		WeaponManager.sharedManager.myPlayerMoveC.ApplyResurrection(inDeathCollider);
	}
}

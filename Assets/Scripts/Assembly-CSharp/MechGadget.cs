public class MechGadget : BaseEffectGadget
{
	public MechGadget(GadgetInfo _info, Player_move_c.GadgetEffect effect)
		: base(_info, effect, true)
	{
	}

	public override void Use()
	{
		WeaponManager.sharedManager.myPlayerMoveC.GadgetsOnMechKill = base.ResetUseCounter;
		base.Use();
	}

	public override void OnTimeExpire()
	{
		WeaponManager.sharedManager.myPlayerMoveC.GadgetsOnMechKill = null;
		base.OnTimeExpire();
	}
}

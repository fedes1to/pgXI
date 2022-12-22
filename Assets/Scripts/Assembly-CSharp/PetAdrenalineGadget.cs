public class PetAdrenalineGadget : BaseEffectGadget
{
	public PetAdrenalineGadget(GadgetInfo _info)
		: base(_info, Player_move_c.GadgetEffect.petAdrenaline)
	{
	}

	public override void Use()
	{
		WeaponManager.sharedManager.myPlayerMoveC.GadgetsOnPetKill = base.ResetUseCounter;
		base.Use();
	}

	public override void OnTimeExpire()
	{
		WeaponManager.sharedManager.myPlayerMoveC.GadgetsOnPetKill = null;
		base.OnTimeExpire();
	}
}

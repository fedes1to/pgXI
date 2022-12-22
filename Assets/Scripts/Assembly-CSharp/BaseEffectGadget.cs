public class BaseEffectGadget : ImmediateGadget
{
	public Player_move_c.GadgetEffect effect;

	public bool sendInfoID;

	public BaseEffectGadget(GadgetInfo _info, Player_move_c.GadgetEffect effect, bool sendInfoID = false)
		: base(_info)
	{
		this.effect = effect;
		this.sendInfoID = sendInfoID;
	}

	public override void PreUse()
	{
	}

	public override void Use()
	{
		StartUseTimer();
		if (sendInfoID)
		{
			WeaponManager.sharedManager.myPlayerMoveC.SetGadgetEffectActivation(effect, true, Info.Id);
		}
		else
		{
			WeaponManager.sharedManager.myPlayerMoveC.SetGadgetEffectActivation(effect, true, string.Empty);
		}
	}

	public override void OnTimeExpire()
	{
		StartCooldown();
		WeaponManager.sharedManager.myPlayerMoveC.SetGadgetEffectActivation(effect, false, string.Empty);
	}
}

public abstract class ThrowGadget : Gadget
{
	private Rocket currentRocket;

	public abstract string GrenadeGadgetId { get; }

	public ThrowGadget(GadgetInfo _info)
		: base(_info)
	{
	}

	public void SetCurrentRocket(Rocket rocket)
	{
		currentRocket = rocket;
		rocket.OnExplode = ClearCurrentRocket;
	}

	protected void KillCurrentRocket()
	{
		if (!(currentRocket == null))
		{
			currentRocket.KillRocket();
		}
	}

	private void ClearCurrentRocket()
	{
		currentRocket = null;
	}

	public override void OnMatchEnd()
	{
		KillCurrentRocket();
	}

	public virtual void ShowThrowingEffect(float time)
	{
	}

	public abstract void CreateRocket(WeaponSounds weapon);

	public abstract void ThrowGrenade();
}

public class TurretGadget : ImmediateGadget
{
	private TurretController currentTurret;

	public override bool CanUse
	{
		get
		{
			return _cooldownTime.value == 0f;
		}
	}

	public TurretGadget(GadgetInfo _info)
		: base(_info)
	{
	}

	public override void Use()
	{
		if (currentTurret != null)
		{
			currentTurret.SendImKilledRPC();
			currentTurret = null;
		}
	}

	public override void PreUse()
	{
	}

	public override void OnTimeExpire()
	{
		if (currentTurret != null)
		{
			currentTurret.SendImKilledRPC();
			currentTurret = null;
		}
	}

	public void StartedCurrentTurret(TurretController _curTurret)
	{
		_curTurret.GadgetOnKill = base.ResetUseCounter;
		StartUseTimer();
		StartCooldown();
		currentTurret = _curTurret;
	}
}

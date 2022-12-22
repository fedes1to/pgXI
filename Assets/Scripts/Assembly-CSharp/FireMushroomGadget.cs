using UnityEngine;

public class FireMushroomGadget : ImmediateGadget
{
	private float nextHitTime;

	public FireMushroomGadget(GadgetInfo _info)
		: base(_info)
	{
	}

	public override void PreUse()
	{
	}

	public override void Update()
	{
		if (nextHitTime < Time.time && _durationTime.value > 0f)
		{
			nextHitTime = Time.time + 2f;
			WeaponManager.sharedManager.myPlayerMoveC.FireMushroomShot(Info);
		}
	}

	public override void Use()
	{
		StartUseTimer();
		nextHitTime = Time.time + 2f;
		WeaponManager.sharedManager.myPlayerMoveC.ActivateFireMushroom();
	}

	public override void OnTimeExpire()
	{
		StartCooldown();
	}
}

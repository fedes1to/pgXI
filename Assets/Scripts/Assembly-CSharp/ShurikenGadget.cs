using UnityEngine;

public class ShurikenGadget : ThrowGadget
{
	public GameObject[] currentGrenades;

	public override string GrenadeGadgetId
	{
		get
		{
			return GadgetsInfo.BaseName(Info.Id);
		}
	}

	public ShurikenGadget(GadgetInfo _info)
		: base(_info)
	{
	}

	public override void PreUse()
	{
		WeaponManager.sharedManager.myPlayerMoveC.GrenadePress(this);
	}

	public override void Use()
	{
		StartCooldown();
		WeaponManager.sharedManager.myPlayerMoveC.GrenadeFire();
	}

	public override void CreateRocket(WeaponSounds weapon)
	{
		currentGrenades = new GameObject[3];
		for (int i = 0; i < currentGrenades.Length; i++)
		{
			Rocket rocket = Player_move_c.CreateRocket(weapon, WeaponManager.sharedManager.myPlayerMoveC.myCurrentWeaponSounds.grenatePoint.position, WeaponManager.sharedManager.myPlayerMoveC.myCurrentWeaponSounds.grenatePoint.rotation, 1f);
			if (Defs.isMulti && !Defs.isInet)
			{
				rocket.SendNetworkViewMyPlayer(WeaponManager.sharedManager.myPlayerMoveC.myPlayerTransform.GetComponent<NetworkView>().viewID);
			}
			currentGrenades[i] = rocket.gameObject;
			rocket.multiplayerDamage = Info.Damage;
			rocket.damage = Info.SurvivalDamage;
		}
	}

	public override void ThrowGrenade()
	{
		if (currentGrenades != null && currentGrenades.Length != 0 && !(currentGrenades[0] == null))
		{
			currentGrenades[0].transform.parent.gameObject.SetActive(true);
			for (int i = 0; i < currentGrenades.Length; i++)
			{
				Rocket component = currentGrenades[i].GetComponent<Rocket>();
				float num = ((!(component.currentRocketSettings != null)) ? 150f : component.currentRocketSettings.startForce);
				component.GetComponent<Rigidbody>().isKinematic = false;
				component.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(0f, -15f + (float)(i * 15), 0f) * (num * WeaponManager.sharedManager.myPlayerMoveC.myTransform.forward));
				component.GetComponent<Rigidbody>().useGravity = false;
				component.RunGrenade();
			}
		}
	}
}

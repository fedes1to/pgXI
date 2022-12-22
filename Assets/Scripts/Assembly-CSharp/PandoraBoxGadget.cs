using UnityEngine;

public class PandoraBoxGadget : ThrowGadget
{
	private bool isSuccess;

	public override string GrenadeGadgetId
	{
		get
		{
			return GadgetsInfo.BaseName(Info.Id);
		}
	}

	public PandoraBoxGadget(GadgetInfo _info)
		: base(_info)
	{
	}

	public override void PreUse()
	{
		StartCooldown();
		WeaponManager.sharedManager.myPlayerMoveC.GrenadePress(this);
		WeaponManager.sharedManager.myPlayerMoveC.GrenadeFire();
	}

	public override void Use()
	{
	}

	public override void ShowThrowingEffect(float time)
	{
		isSuccess = Random.Range(0, 2) == 1;
		if (InGameGUI.sharedInGameGUI != null)
		{
			if (isSuccess)
			{
				InGameGUI.sharedInGameGUI.pandoraSuccessEffect.Play(time * 2f);
			}
			else
			{
				InGameGUI.sharedInGameGUI.pandoraFailEffect.Play(time * 2f);
			}
		}
	}

	public override void CreateRocket(WeaponSounds weapon)
	{
	}

	public override void ThrowGrenade()
	{
		WeaponManager.sharedManager.myPlayerMoveC.UsePandoraBox(Info, isSuccess);
	}
}

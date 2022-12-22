using UnityEngine;

public class TurretController_MedStation : TurretController
{
	public float healRadius = 4f;

	public float timeHeal = 1f;

	private float nextHealTime;

	private float healValue = 1f;

	protected override void UpdateTurret()
	{
		base.UpdateTurret();
		if (isRun && !isKilled && (!isEnemyTurret || Defs.isCOOP) && WeaponManager.sharedManager.myPlayerMoveC != null && nextHealTime < Time.time && (base.transform.position - WeaponManager.sharedManager.myPlayerMoveC.myPlayerTransform.position).sqrMagnitude < healRadius * healRadius)
		{
			if (WeaponManager.sharedManager.myPlayerMoveC.AddHealth(healValue) && InGameGUI.sharedInGameGUI != null)
			{
				InGameGUI.sharedInGameGUI.medStationEffect.Play();
			}
			if (WeaponManager.sharedManager.myPlayerMoveC.myPetEngine != null && WeaponManager.sharedManager.myPlayerMoveC.myPetEngine.IsAlive)
			{
				WeaponManager.sharedManager.myPlayerMoveC.myPetEngine.UpdateCurrentHealth(WeaponManager.sharedManager.myPlayerMoveC.myPetEngine.CurrentHealth += healValue);
			}
			nextHealTime = Time.time + timeHeal;
		}
	}

	protected override void SetParametersFromGadgets(GadgetInfo info)
	{
		base.SetParametersFromGadgets(info);
		healValue = info.HPS;
	}
}

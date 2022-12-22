using UnityEngine;

public class TurretController_ChristmasTree : TurretController
{
	[Header("ChristmasTree settings")]
	public Transform tower;

	public float damageRadius = 7f;

	public float damageHeight = 2f;

	public float towerRotationSpeed = 900f;

	public float hitTime = 0.1f;

	public float hitChance = 10f;

	public GameObject gunFlashes;

	public GameObject workSound;

	public AudioClip shotClip;

	private float nextHitTime;

	protected override void UpdateTurret()
	{
		base.UpdateTurret();
		gunFlashes.SetActive(isReady);
		if (workSound != null)
		{
			workSound.SetActive(isReady && Defs.isSoundFX);
		}
		if (isReady)
		{
			float num = Time.deltaTime * towerRotationSpeed;
			tower.localRotation = Quaternion.Euler(new Vector3(0f, 0f, tower.localRotation.eulerAngles.z - num));
			if (isRun && !isKilled && (!Defs.isMulti || isMine) && nextHitTime < Time.time)
			{
				ShotTargets();
				nextHitTime = Time.time + hitTime;
			}
		}
	}

	private void ShotTargets()
	{
		Initializer.TargetsList targetsList = new Initializer.TargetsList(myPlayerMoveC);
		foreach (Transform item in targetsList)
		{
			if (!(hitChance > (float)Random.Range(0, 100)))
			{
				Vector3 vector = item.transform.position - base.transform.position;
				if (!(vector.y < 0f - damageHeight) && !(vector.y > damageHeight) && !(vector.x * vector.x + vector.z * vector.z > damageRadius * damageRadius))
				{
					HitIDestructible(item.gameObject);
				}
			}
		}
		if (!Defs.isMulti)
		{
			ShotRPC();
		}
		else if (!Defs.isInet)
		{
			_networkView.RPC("ShotRPC", RPCMode.All);
		}
		else
		{
			photonView.RPC("ShotRPC", PhotonTargets.All);
		}
	}

	protected override void Shot()
	{
		if (Defs.isSoundFX)
		{
			GetComponent<AudioSource>().PlayOneShot(shotClip);
		}
	}
}

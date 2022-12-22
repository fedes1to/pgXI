using System.Reflection;
using Rilisoft;
using UnityEngine;
using ZeichenKraftwerk;

public sealed class TurretController_Minigun : TurretController_Tower
{
	[Header("Minigun settings")]
	public ParticleSystem gunFlash;

	public Rotator rotator;

	public AudioClip shotClip;

	public Transform shotPoint;

	public Transform shotPoint2;

	private float maxSpeedRotator = -1000f;

	private float downSpeedRotator = 500f;

	private float dissipation = 0.015f;

	protected override void UpdateTurret()
	{
		base.UpdateTurret();
		if (rotator != null && rotator.eulersPerSecond.z < -200f)
		{
			rotator.eulersPerSecond = new Vector3(0f, rotator.eulersPerSecond.z + downSpeedRotator * Time.deltaTime, 0f);
		}
	}

	[Obfuscation(Exclude = true)]
	private void StopGunFlash()
	{
		gunFlash.enableEmission = false;
	}

	protected override void Shot()
	{
		if (shotPoint2 == null || shotPoint == null)
		{
			return;
		}
		if (rotator != null)
		{
			rotator.eulersPerSecond = new Vector3(0f, maxSpeedRotator, 0f);
		}
		if (Defs.isSoundFX)
		{
			GetComponent<AudioSource>().PlayOneShot(shotClip);
		}
		if (gunFlash != null)
		{
			gunFlash.enableEmission = true;
			gunFlash.Play();
		}
		CancelInvoke("StopGunFlash");
		Invoke("StopGunFlash", maxTimerShot * 1.1f);
		if (Defs.isMulti && !isMine)
		{
			return;
		}
		Vector3 vector = new Vector3(shotPoint2.position.x - shotPoint.position.x + Random.Range(0f - dissipation, dissipation), shotPoint2.position.y - shotPoint.position.y + Random.Range(0f - dissipation, dissipation), shotPoint2.position.z - shotPoint.position.z + Random.Range(0f - dissipation, dissipation));
		Ray ray = new Ray(shotPoint.position, vector);
		Debug.DrawRay(shotPoint.position, vector * 100f, Color.green, 1f);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, 100f, Tools.AllWithoutDamageCollidersMask) || (Defs.isMulti && !(WeaponManager.sharedManager.myPlayer != null)))
		{
			return;
		}
		bool flag = hitInfo.collider.transform.root != null && hitInfo.collider.transform.root.gameObject.Equals(WeaponManager.sharedManager.myPlayer);
		bool flag2 = false;
		if (flag || HitIDestructible(hitInfo.collider.gameObject))
		{
			flag2 = true;
		}
		if (Defs.isMulti)
		{
			if (!Defs.isInet)
			{
				WeaponManager.sharedManager.myPlayerMoveC.GetComponent<NetworkView>().RPC("HoleRPC", RPCMode.All, flag2, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
			}
			else
			{
				WeaponManager.sharedManager.myPlayerMoveC.photonView.RPC("HoleRPC", PhotonTargets.All, flag2, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
			}
		}
	}
}

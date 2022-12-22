using System;
using System.Collections;
using Rilisoft;
using UnityEngine;

public class TurretController_Tower : TurretController
{
	[Header("Tower settings")]
	public Transform tower;

	public Transform gun;

	private float idleAlphaY;

	private float idleRotateSpeedY = 20f;

	private float maxDeltaRotateY = 60f;

	private float maxRotateX = 75f;

	private float minRotateX = -60f;

	private float speedRotateY = 220f;

	private float speedRotateX = 30f;

	private float timerShot;

	protected override void SearchTarget()
	{
		base.SearchTarget();
		if (Mathf.Abs(idleAlphaY) < 0.5f)
		{
			idleAlphaY = UnityEngine.Random.Range(-1f * maxDeltaRotateY / 2f, maxDeltaRotateY / 2f);
		}
		else
		{
			float num = Time.deltaTime * idleRotateSpeedY * Mathf.Abs(idleAlphaY) / idleAlphaY;
			idleAlphaY -= num;
			tower.localRotation = Quaternion.Euler(new Vector3(0f, 0f, tower.localRotation.eulerAngles.z + num));
		}
		if (Mathf.Abs(gun.localRotation.eulerAngles.x) > 1f)
		{
			gun.Rotate((float)((!(gun.localRotation.eulerAngles.x < 180f)) ? 1 : (-1)) * speedRotateX * Time.deltaTime, 0f, 0f);
		}
	}

	protected override IEnumerator ScanTarget()
	{
		inScaning = true;
		GameObject closestTargetObj = null;
		float closestTarget = float.MaxValue;
		Initializer.TargetsList targets = new Initializer.TargetsList(WeaponManager.sharedManager.myPlayerMoveC, false, false);
		foreach (Transform enemy in targets)
		{
			Vector3 enemyDelta = enemy.position - base.transform.position;
			Vector3 enemyForward = new Vector3(enemyDelta.x, 0f, enemyDelta.z);
			float targetDistance = enemyDelta.sqrMagnitude;
			if (targetDistance < closestTarget && targetDistance < maxRadiusScanTargetSQR && Vector3.Angle(enemyForward, enemyDelta) < maxRotateX)
			{
				Vector3 popravochka = Vector3.zero;
				BoxCollider _collider = enemy.GetComponent<BoxCollider>();
				if (_collider == null && enemy.CompareTag("Enemy"))
				{
					for (int i = 0; i < enemy.childCount; i++)
					{
						BoxCollider boxcollider = enemy.GetChild(i).GetComponent<BoxCollider>();
						if (boxcollider != null)
						{
							_collider = boxcollider;
							break;
						}
					}
				}
				if (_collider != null)
				{
					popravochka = _collider.transform.rotation * _collider.center;
				}
				Ray ray = new Ray(tower.position, enemy.transform.position + popravochka - tower.position);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, maxRadiusScanTarget, Tools.AllWithoutDamageCollidersMask) && hit.collider.gameObject.transform.root.Equals(enemy.root))
				{
					closestTarget = targetDistance;
					closestTargetObj = enemy.gameObject;
				}
			}
			yield return null;
		}
		if (closestTargetObj != null)
		{
			target = closestTargetObj.transform;
		}
		else
		{
			target = null;
		}
		inScaning = false;
	}

	protected override void TargetUpdate()
	{
		base.TargetUpdate();
		bool flag = false;
		Vector2 to = new Vector2(target.position.x, target.position.z) - new Vector2(tower.position.x, tower.position.z);
		float deltaAngles = GetDeltaAngles(tower.rotation.eulerAngles.y, Mathf.Abs(to.x) / to.x * Vector2.Angle(Vector2.up, to));
		float num = (0f - speedRotateY) * Time.deltaTime * Mathf.Abs(deltaAngles) / deltaAngles;
		if (Mathf.Abs(deltaAngles) < 10f)
		{
			flag = true;
		}
		if (Mathf.Abs(num) > Mathf.Abs(deltaAngles))
		{
			num = 0f - deltaAngles;
		}
		if (Mathf.Abs(num) > 0.001f)
		{
			tower.Rotate(0f, 0f, num);
		}
		Vector3 vector = Vector3.zero;
		BoxCollider boxCollider = target.GetComponent<BoxCollider>();
		if (boxCollider == null && target.CompareTag("Enemy"))
		{
			for (int i = 0; i < target.childCount; i++)
			{
				BoxCollider component = target.GetChild(i).GetComponent<BoxCollider>();
				if (component != null)
				{
					boxCollider = component;
					break;
				}
			}
		}
		if (boxCollider != null)
		{
			vector = boxCollider.transform.rotation * boxCollider.center;
		}
		float angle = -180f * Mathf.Atan((target.position.y + vector.y - tower.position.y) / Vector3.Distance(target.position + vector, base.transform.position)) / (float)Math.PI;
		float deltaAngles2 = GetDeltaAngles(gun.localRotation.eulerAngles.x, angle);
		num = (0f - speedRotateX) * Time.deltaTime * Mathf.Abs(deltaAngles2) / deltaAngles2;
		if (Mathf.Abs(num) > Mathf.Abs(deltaAngles2))
		{
			num = 0f - deltaAngles2;
		}
		if (Mathf.Abs(num) > 0.001f)
		{
			gun.Rotate(num, 0f, 0f);
		}
		if (!flag)
		{
			return;
		}
		timerShot -= Time.deltaTime;
		if (timerShot < 0f)
		{
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
			timerShot = maxTimerShot;
		}
	}

	private float GetDeltaAngles(float angle1, float angle2)
	{
		if (angle1 < 0f)
		{
			angle1 += 360f;
		}
		if (angle2 < 0f)
		{
			angle2 += 360f;
		}
		float num = angle1 - angle2;
		if (Mathf.Abs(num) > 180f)
		{
			num = ((!(angle1 > angle2)) ? (num + 360f) : (num - 360f));
		}
		return num;
	}

	protected override void OnKill()
	{
		if (gun.rotation.x > minRotateX)
		{
			gun.Rotate(speedRotateX * Time.deltaTime, 0f, 0f);
		}
	}

	protected override void UpdateTurret()
	{
		base.UpdateTurret();
	}
}

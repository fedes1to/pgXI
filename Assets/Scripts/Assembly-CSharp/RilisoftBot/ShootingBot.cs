using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

namespace RilisoftBot
{
	public class ShootingBot : BaseShootingBot
	{
		private const float offsetPointDamagePlayer = 0.5f;

		[Header("Explosion damage settings")]
		public bool isProjectileExplosion;

		public float damagePerHitMin;

		public GameObject effectExplosion;

		public float radiusExplosion;

		public float speedBullet = 10f;

		[Header("Automatic bullet speed settings")]
		public bool isCalculateSpeedBullet;

		[Header("Shooting sound settings")]
		public AudioClip shootingSound;

		private float _normalBulletSpeed;

		private List<Transform> damagedTargets = new List<Transform>();

		[Header("Physics shot settings")]
		public bool isMoveByPhysics;

		public float force = 14f;

		public float angle = -10f;

		protected override void Initialize()
		{
			base.Initialize();
			float length = animations[animationsName.Attack].length;
			_normalBulletSpeed = (attackDistance + rangeShootingDistance) / length;
		}

		protected override void InitializeShotsPool(int sizePool)
		{
			base.InitializeShotsPool(sizePool);
			float lifeTime = (attackDistance + rangeShootingDistance) / GetBulletSpeed();
			for (int i = 0; i < bulletsEffectPool.Length; i++)
			{
				BulletForBot component = bulletsEffectPool[i].GetComponent<BulletForBot>();
				if (component != null)
				{
					component.lifeTime = lifeTime;
					component.OnBulletDamage += MakeDamageTarget;
					component.needDestroyByStop = false;
				}
			}
		}

		private BulletForBot GetShotFromPool()
		{
			GameObject shotEffectFromPool = GetShotEffectFromPool();
			return shotEffectFromPool.GetComponent<BulletForBot>();
		}

		private float GetBulletSpeed()
		{
			if (isCalculateSpeedBullet)
			{
				speedBullet = _normalBulletSpeed * speedAnimationAttack;
			}
			return speedBullet;
		}

		private void MakeDamageTarget(GameObject target, Vector3 positionDamage)
		{
			if (isProjectileExplosion)
			{
				Object.Instantiate(effectExplosion, positionDamage, Quaternion.identity);
				Collider[] array = Physics.OverlapSphere(positionDamage, radiusExplosion, Tools.AllAvailabelBotRaycastMask);
				if (array.Length == 0)
				{
					return;
				}
				float num = radiusExplosion * radiusExplosion;
				damagedTargets.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].gameObject == null)
					{
						continue;
					}
					Transform root = array[i].transform.root;
					if (!(root.gameObject == null) && !(base.transform.gameObject == null) && !root.Equals(base.transform))
					{
						float sqrMagnitude = (root.position - positionDamage).sqrMagnitude;
						if (!(sqrMagnitude > num) && (isFriendlyFire || !root.CompareTag("Enemy")) && !damagedTargets.Contains(array[i].transform.root.transform))
						{
							float num2 = damagePerHitMin + (damagePerHit - damagePerHitMin) * ((num - sqrMagnitude) / num);
							MakeDamage(array[i].transform.root.transform, (int)num2);
							damagedTargets.Add(array[i].transform.root.transform);
						}
					}
				}
			}
			else if (target != null)
			{
				MakeDamage(target.transform);
			}
		}

		protected override void Fire(Transform pointFire, GameObject target)
		{
			Vector3 position = target.transform.position;
			position.y += 0.5f;
			FireBullet(pointFire.position, position, true);
			if (Defs.isCOOP)
			{
				FireByRPC(pointFire.position, position);
			}
		}

		private void FireBullet(Vector3 pointFire, Vector3 positionToFire, bool doDamage)
		{
			BulletForBot shotFromPool = GetShotFromPool();
			if (isCalculateSpeedBullet)
			{
				animations[animationsName.Attack].speed = speedAnimationAttack;
			}
			if (isMoveByPhysics)
			{
				Quaternion quaternion = Quaternion.AngleAxis(angle, base.transform.right);
				Vector3 forceVector = quaternion * base.transform.forward;
				shotFromPool.ApplyForceFroBullet(pointFire, positionToFire, isFriendlyFire, force, forceVector, doDamage);
			}
			else
			{
				shotFromPool.StartBullet(pointFire, positionToFire, GetBulletSpeed(), isFriendlyFire, doDamage);
			}
			TryPlayAudioClip(shootingSound);
		}

		[RPC]
		[PunRPC]
		private void FireBulletRPC(Vector3 pointFire, Vector3 positionToFire)
		{
			FireBullet(pointFire, positionToFire, false);
		}

		protected override void OnBotDestroyEvent()
		{
			for (int i = 0; i < bulletsEffectPool.Length; i++)
			{
				if (bulletsEffectPool[i].gameObject == null)
				{
					continue;
				}
				BulletForBot component = bulletsEffectPool[i].GetComponent<BulletForBot>();
				if (component != null)
				{
					component.OnBulletDamage -= MakeDamageTarget;
					if (component.IsUse)
					{
						component.needDestroyByStop = true;
					}
					else
					{
						Object.Destroy(bulletsEffectPool[i]);
					}
				}
			}
		}
	}
}

using UnityEngine;

namespace RilisoftBot
{
	public class MeleeShootBot : ShootingBot
	{
		public enum AttackType
		{
			MeleeAndShoot,
			MeleeAndShootAtTime
		}

		[Header("Melee damage settings")]
		public float meleeAttackDetect = 6f;

		public float meleeAttackDistance = 3f;

		public float meleeDamagePerHit = 5f;

		[Header("Attack settings")]
		public AttackType attackType;

		public float minTimeToShoot = 30f;

		public float maxTimeToShoot = 40f;

		private bool isEnemyInMeleeZone;

		private bool isEnemyEnterInAttackZone;

		private string animationNameMelee;

		private bool wasMeleeShot;

		private float nextShootTime;

		private string GameNameMeleeAnimation()
		{
			if (modelCollider == null)
			{
				return string.Empty;
			}
			string arg = modelCollider.gameObject.name;
			return string.Format("{0}_shooting", arg);
		}

		protected override void Initialize()
		{
			base.Initialize();
			animationNameMelee = GameNameMeleeAnimation();
			nextShootTime = Time.time + Random.Range(minTimeToShoot, maxTimeToShoot);
		}

		public override void DelayShootAfterEvent(float seconds)
		{
			if (nextShootTime < Time.time + seconds)
			{
				nextShootTime = Time.time + seconds;
			}
		}

		public override bool CheckEnemyInAttackZone(float distanceToEnemy)
		{
			float squareAttackDistance = GetSquareAttackDistance();
			isEnemyInMeleeZone = false;
			if (distanceToEnemy < squareAttackDistance)
			{
				if (distanceToEnemy < meleeAttackDetect * meleeAttackDetect)
				{
					isEnemyInMeleeZone = true;
					if (distanceToEnemy < meleeAttackDistance * meleeAttackDistance)
					{
						return true;
					}
					return false;
				}
				if (attackType == AttackType.MeleeAndShootAtTime && nextShootTime < Time.time)
				{
					return true;
				}
				if (attackType != AttackType.MeleeAndShootAtTime)
				{
					isEnemyEnterInAttackZone = true;
					return true;
				}
			}
			if (isEnemyEnterInAttackZone)
			{
				squareAttackDistance += rangeShootingDistance * rangeShootingDistance;
				if (distanceToEnemy < squareAttackDistance)
				{
					return true;
				}
			}
			isEnemyEnterInAttackZone = false;
			return false;
		}

		protected override void MakeShot(GameObject target)
		{
			if (wasMeleeShot)
			{
				Melee(target);
				return;
			}
			if (attackType == AttackType.MeleeAndShootAtTime)
			{
				nextShootTime = Time.time + Random.Range(minTimeToShoot, maxTimeToShoot);
			}
			base.MakeShot(target);
		}

		protected override void PlayAnimationZombieAttackOrStop()
		{
			if (isEnemyInMeleeZone)
			{
				wasMeleeShot = true;
				if ((bool)animations[animationNameMelee])
				{
					animations.CrossFade(animationNameMelee);
				}
				else if ((bool)animations[animationsName.Stop])
				{
					animations.CrossFade(animationsName.Stop);
				}
			}
			else
			{
				wasMeleeShot = false;
				if ((bool)animations[animationsName.Attack])
				{
					animations.CrossFade(animationsName.Attack);
				}
				else if ((bool)animations[animationsName.Stop])
				{
					animations.CrossFade(animationsName.Stop);
				}
			}
		}

		private void Melee(GameObject target)
		{
			float num = Vector3.SqrMagnitude(target.transform.position - base.transform.position);
			if (num < meleeAttackDistance * meleeAttackDistance)
			{
				MakeDamage(target.transform, meleeDamagePerHit);
			}
		}
	}
}

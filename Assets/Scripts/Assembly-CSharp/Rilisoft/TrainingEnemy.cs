using System.Collections;
using System.Linq;
using Unity.Linq;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class TrainingEnemy : MonoBehaviour, IDamageable
	{
		private enum State
		{
			None,
			Awakened,
			Dead
		}

		public AudioClip wakeUpAudioClip;

		public AudioClip dieAudioClip;

		public GameObject aimTarget;

		public int hitPoints = 3;

		private SkinnedMeshRenderer meshRender;

		public Texture hitTexture;

		private Texture skinTexture;

		[ReadOnly]
		public int baseHitPoints;

		public float heightFlyOutHitEffect = 1.75f;

		private Collider _headCol;

		private AudioSource _audioSource;

		private Animation _animation;

		private State _currentState;

		private Collider HeadCollider
		{
			get
			{
				if (_headCol != null)
				{
					return _headCol;
				}
				GameObject gameObject = base.gameObject.Descendants("HeadCollider").FirstOrDefault();
				if (gameObject != null)
				{
					_headCol = gameObject.GetComponent<Collider>();
				}
				return _headCol;
			}
		}

		public bool isLivingTarget
		{
			get
			{
				return false;
			}
		}

		public void WakeUp(float delaySeconds = 0f)
		{
			StartCoroutine(AwakeCoroutine(delaySeconds));
		}

		public void ApplyDamage(float damage, IDamageable damageFrom, Player_move_c.TypeKills typeKill)
		{
			ApplyDamage(damage, damageFrom, typeKill, WeaponSounds.TypeDead.angel, string.Empty);
		}

		public void ApplyDamage(float damage, IDamageable damageFrom, Player_move_c.TypeKills typeKill, WeaponSounds.TypeDead typeDead, string weaponName, int killerId = 0)
		{
			ApplyDamage(damage, typeKill == Player_move_c.TypeKills.headshot);
		}

		public bool IsEnemyTo(Player_move_c player)
		{
			return true;
		}

		public bool IsDead()
		{
			return _currentState == State.Dead;
		}

		public void ApplyDamage(float damage, bool isHeadShot)
		{
			if (_currentState != State.Awakened)
			{
				return;
			}
			StartCoroutine(HighlightHitCoroutine());
			if (isHeadShot)
			{
				ShowHeadShotEffect();
			}
			else
			{
				ShowHitEffect();
			}
			hitPoints--;
			if (_animation != null)
			{
				_animation.Play("Dummy_Damage", PlayMode.StopSameLayer);
			}
			if (hitPoints <= 0)
			{
				_currentState = State.Dead;
				if (aimTarget != null)
				{
					Object.Destroy(aimTarget);
					aimTarget = null;
				}
				StartCoroutine(DieCoroutine());
			}
		}

		private void Awake()
		{
			baseHitPoints = hitPoints;
			if (aimTarget != null)
			{
				aimTarget.SetActive(false);
			}
		}

		private void Start()
		{
			_audioSource = GetComponent<AudioSource>();
			_animation = GetComponent<Animation>();
			if (_animation != null)
			{
				_animation.Play("Dummy_Idle", PlayMode.StopSameLayer);
			}
			meshRender = GetComponentInChildren<SkinnedMeshRenderer>();
			if ((bool)meshRender)
			{
				meshRender.sharedMaterial = new Material(meshRender.sharedMaterial);
				skinTexture = meshRender.sharedMaterial.mainTexture;
			}
		}

		private IEnumerator HighlightHitCoroutine()
		{
			SetTexture(hitTexture);
			yield return new WaitForSeconds(0.125f);
			SetTexture(skinTexture);
		}

		public void SetTexture(Texture needTx)
		{
			if (meshRender != null)
			{
				meshRender.sharedMaterial.mainTexture = needTx;
			}
		}

		private IEnumerator AwakeCoroutine(float delaySeconds = 0f)
		{
			if (delaySeconds > 0f)
			{
				yield return new WaitForSeconds(delaySeconds);
			}
			if (_animation != null)
			{
				if (_audioSource != null && wakeUpAudioClip != null)
				{
					_audioSource.PlayOneShot(wakeUpAudioClip);
				}
				_animation.Play("Dummy_Up", PlayMode.StopSameLayer);
				while (_animation.isPlaying)
				{
					yield return null;
				}
				if (aimTarget != null)
				{
					aimTarget.SetActive(true);
				}
			}
			_currentState = State.Awakened;
		}

		private IEnumerator DieCoroutine()
		{
			if (_animation != null)
			{
				while (_animation.IsPlaying("Dummy_Damage"))
				{
					yield return null;
				}
				if (_audioSource != null && dieAudioClip != null)
				{
					_audioSource.PlayOneShot(dieAudioClip);
				}
				_animation.Play("Dead", PlayMode.StopSameLayer);
				while (_animation.isPlaying)
				{
					yield return null;
				}
			}
			if (ZombieCreator.sharedCreator != null)
			{
				ZombieCreator.sharedCreator.NumOfDeadZombies++;
			}
		}

		private void ShowHitEffect()
		{
			if (!Device.isPixelGunLow)
			{
				HitParticle currentParticle = ParticleStacks.instance.hitStack.GetCurrentParticle(false);
				if (currentParticle != null)
				{
					currentParticle.StartShowParticle(base.transform.position, base.transform.rotation, false, base.transform.position + Vector3.up * heightFlyOutHitEffect);
				}
			}
		}

		private void ShowHeadShotEffect()
		{
			if (!Device.isPixelGunLow)
			{
				HitParticle currentParticle = HeadShotStackController.sharedController.GetCurrentParticle(false);
				if (currentParticle != null && HeadCollider != null)
				{
					Vector3 position = ((!(HeadCollider is BoxCollider)) ? ((SphereCollider)HeadCollider).center : ((BoxCollider)HeadCollider).center);
					currentParticle.StartShowParticle(base.transform.position, base.transform.rotation, false, HeadCollider.transform.TransformPoint(position));
				}
			}
		}
	}
}

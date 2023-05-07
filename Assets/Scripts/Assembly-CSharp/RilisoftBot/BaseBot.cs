using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Rilisoft;
using UnityEngine;

namespace RilisoftBot
{
	public class BaseBot : MonoBehaviour, IDamageable
	{
		protected class BotAnimationName
		{
			public string Walk = "Norm_Walk";

			public string Run = "Zombie_Walk";

			public string Stop = "Zombie_Off";

			public string Death = "Zombie_Dead";

			public string Attack = "Zombie_Attack";

			public string Idle = "Idle";
		}

		private enum RunNetworkAnimationType
		{
			ZombieWalk,
			ZombieAttackOrStop,
			None
		}

		public const string BaseNameBotGuard = "BossGuard";

		private const int ScoreForDamage = 5;

		private const float HeadShotDamageModif = 2f;

		private const int MultiplySmoothMove = 5;

		public string nameBot;

		[Header("Sound settings")]
		public AudioClip damageSound;

		public AudioClip voiceMobSoud;

		public AudioClip takeDamageSound;

		public AudioClip deathSound;

		public AudioClip stepSound;

		public AudioClip runStepSound;

		[Header("Common damage settings")]
		public float notAttackingSpeed = 1f;

		public float attackingSpeed = 1f;

		public float health = 100f;

		public float attackDistance = 3f;

		public float detectRadius = 17f;

		public float damagePerHit = 1f;

		public int scorePerKill = 50;

		public float[] attackingSpeedRandomRange = new float[2] { -0.5f, 0.5f };

		[Header("Effects settings")]
		public Texture flashDeadthTexture;

		public float heightFlyOutHitEffect = 1.75f;

		[NonSerialized]
		public int indexMobPrefabForCoop;

		protected BotAiController botAiController;

		protected Transform mobModel;

		protected Animation animations;

		protected bool isMobChampion;

		protected BoxCollider modelCollider;

		protected SphereCollider headCollider;

		protected BotAnimationName animationsName;

		protected AudioSource audioSource;

		private bool _isFlashing;

		private PhotonView _photonView;

		private bool _isMultiplayerMode;

		private BotChangeDamageMaterial[] _botMaterials;

		private IEnemyEffectsManager _effectsManager;

		private AdvancedEffects advancedEffects;

		private float[] coefHealthFromTier = new float[6] { 1f, 1.25f, 1.65f, 2f, 2.25f, 2.65f };

		private bool _isPlayingDamageSound;

		private bool _isDeathAudioPlaying;

		[Header("Automatic animation speed settings")]
		public bool isAutomaticAnimationEnable;

		[Range(0.1f, 2f)]
		public float speedAnimationWalk = 1f;

		[Range(0.1f, 2f)]
		public float speedAnimationRun = 1f;

		[Range(0.1f, 2f)]
		public float speedAnimationAttack = 1f;

		[Header("Flying settings")]
		public bool isFlyingSpeedLimit;

		public float maxFlyingSpeed;

		[Header("Guard settings")]
		public GameObject[] guards;

		private bool _isWeaponCreated;

		private bool _killed;

		private float _modMoveSpeedByDebuff = 1f;

		private List<BotDebuff> _botDebufs = new List<BotDebuff>();

		private Vector3 _botPosition;

		private Quaternion _botRotation;

		private RunNetworkAnimationType _currentRunNetworkAnimation = RunNetworkAnimationType.None;

		public bool IsDeath { get; private set; }

		public bool IsFalling { get; private set; }

		public bool needDestroyByMasterClient { get; private set; }

		public float baseHealth { get; private set; }

		public bool isLivingTarget
		{
			get
			{
				return true;
			}
		}

		private void Awake()
		{
			advancedEffects = base.gameObject.AddComponent<AdvancedEffects>();
			_effectsManager = GetComponent<IEnemyEffectsManager>();
			audioSource = GetComponent<AudioSource>();
			isMobChampion = false;
			Initialize();
		}

		protected virtual void Initialize()
		{
			animationsName = new BotAnimationName();
			AntiHackForCreateMobInInvalidGameMode();
			_photonView = GetComponent<PhotonView>();
			_isMultiplayerMode = _photonView != null && Defs.isCOOP;
			animations = GetComponentInChildren<Animation>();
			animations.Stop();
			botAiController = GetComponent<BotAiController>();
			modelCollider = GetComponentInChildren<BoxCollider>();
			headCollider = GetComponentInChildren<SphereCollider>();
			UnityEngine.Random.seed = (int)DateTime.Now.Ticks & 0xFFFF;
			InitializeRandomAttackSpeed();
			ModifyParametrsForLocalMode();
			needDestroyByMasterClient = false;
			int num = ((PhotonNetwork.room != null) ? Convert.ToInt32(PhotonNetwork.room.customProperties["tier"]) : ((ExpController.Instance != null) ? ExpController.Instance.OurTier : 0));
			health *= coefHealthFromTier[num];
			baseHealth = health;
		}

		private void Start()
		{
			if (_isMultiplayerMode && _photonView.isMine)
			{
				_photonView.RPC("SetBotHealthRPC", PhotonTargets.All, health);
			}
			if (!_isMultiplayerMode)
			{
				ZombieCreator.sharedCreator.NumOfLiveZombies++;
			}
			mobModel = modelCollider.transform.GetChild(0);
			_botMaterials = GetComponentsInChildren<BotChangeDamageMaterial>();
			InitNetworkStateData();
			Initializer.enemiesObj.Add(base.gameObject);
			if (_effectsManager == null)
			{
				_effectsManager = base.gameObject.AddComponent<PortalEnemyEffectsManager>();
			}
			_effectsManager.ShowSpawnEffect();
			PlayVoiceSound();
		}

		public virtual void DelayShootAfterEvent(float seconds)
		{
		}

		private Texture GetBotSkin()
		{
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
			if (componentsInChildren.Length == 0)
			{
				return null;
			}
			return componentsInChildren[componentsInChildren.Length - 1].material.mainTexture;
		}

		private void AntiHackForCreateMobInInvalidGameMode()
		{
			if (Defs.isMulti && !Defs.isCOOP)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void OrientToTarget(Vector3 targetPos)
		{
			base.transform.LookAt(targetPos);
		}

		[Obfuscation(Exclude = true)]
		public void PlayAnimationIdle()
		{
			animations.Stop();
			if ((bool)animations[animationsName.Idle])
			{
				animations.CrossFade(animationsName.Idle);
			}
			StopSteps();
		}

		public void PlayAnimationWalk()
		{
			animations.Stop();
			if ((bool)animations[animationsName.Walk])
			{
				animations.CrossFade(animationsName.Walk);
			}
			else
			{
				animations.CrossFade(animationsName.Run);
			}
			PlayWalkStepSound();
		}

		private void PlayAnimationZombieWalk()
		{
			if ((bool)animations[animationsName.Run])
			{
				animations.CrossFade(animationsName.Run);
			}
			PlayRunStepSound();
		}

		protected virtual void PlayAnimationZombieAttackOrStop()
		{
			if ((bool)animations[animationsName.Attack])
			{
				animations.CrossFade(animationsName.Attack);
			}
			else if ((bool)animations[animationsName.Stop])
			{
				animations.CrossFade(animationsName.Stop);
			}
			StopSteps();
		}

		private void InitializeRandomAttackSpeed()
		{
			if (isAutomaticAnimationEnable)
			{
				float min = (attackingSpeed - attackingSpeedRandomRange[0]) / attackingSpeed;
				float max = (attackingSpeed + attackingSpeedRandomRange[1]) / attackingSpeed;
				speedAnimationRun = UnityEngine.Random.Range(min, max);
			}
			else
			{
				attackingSpeed += UnityEngine.Random.Range(0f - attackingSpeedRandomRange[0], attackingSpeedRandomRange[1]);
			}
		}

		private void SetRangeParametrs()
		{
			if (!isMobChampion && !IsBotGuard())
			{
				ZombieCreator.LastEnemy += IncreaseRange;
				if (ZombieCreator.sharedCreator.IsLasTMonsRemains)
				{
					IncreaseRange();
				}
			}
		}

		private void ModifyParametrsForLocalMode()
		{
			float num = 0f;
			float num2 = 0f;
			if (isAutomaticAnimationEnable)
			{
				num = speedAnimationWalk;
				num2 = speedAnimationRun;
			}
			else
			{
				num = notAttackingSpeed;
				num2 = attackingSpeed;
			}
			if (!_isMultiplayerMode)
			{
				if (!Defs.IsSurvival)
				{
					num2 *= Defs.DiffModif;
					health *= Defs.DiffModif;
					num *= Defs.DiffModif;
				}
				else if (Defs.IsSurvival && TrainingController.TrainingCompleted)
				{
					int currentWave = ZombieCreator.sharedCreator.currentWave;
					if (currentWave == 0)
					{
						num *= 0.75f;
						num2 *= 0.75f;
						health *= 0.7f;
					}
					else if (currentWave == 1)
					{
						num *= 0.85f;
						num2 *= 0.85f;
						health *= 0.8f;
					}
					else if (currentWave == 2)
					{
						num *= 0.9f;
						num2 *= 0.9f;
						health *= 0.9f;
					}
					else if (currentWave >= 7)
					{
						num *= 1.25f;
						num2 *= 1.25f;
					}
					else if (currentWave >= 9)
					{
						health *= 1.25f;
					}
				}
			}
			if (_isMultiplayerMode || Defs.IsSurvival)
			{
				num *= 0.9f + (float)ExpController.OurTierForAnyPlace() * 0.1f;
				health *= 0.9f + (float)ExpController.OurTierForAnyPlace() * 0.1f;
			}
			if (isAutomaticAnimationEnable)
			{
				speedAnimationWalk = num;
				speedAnimationRun = num2;
			}
			else
			{
				notAttackingSpeed = num;
				attackingSpeed = num2;
			}
			if (!Defs.IsSurvival && !_isMultiplayerMode)
			{
				SetRangeParametrs();
			}
		}

		private void IncreaseRange()
		{
			if (isAutomaticAnimationEnable)
			{
				speedAnimationRun = Mathf.Max(speedAnimationRun, 1.5f);
			}
			else
			{
				attackingSpeed = Mathf.Max(attackingSpeed, 3f);
			}
			detectRadius = 150f;
		}

		public float GetSquareAttackDistance()
		{
			return attackDistance * attackDistance;
		}

		public float GetSquareDetectRadius()
		{
			return detectRadius * detectRadius;
		}

		public void SetPositionForFallState()
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 7f * Time.deltaTime, base.transform.position.z);
		}

		public void TryPlayAudioClip(AudioClip audioClip)
		{
			if (Defs.isSoundFX && !(audioSource == null) && !(audioClip == null))
			{
				audioSource.PlayOneShot(audioClip);
			}
		}

		public void PlayVoiceSound()
		{
			TryPlayAudioClip(voiceMobSoud);
		}

		public void PlayWalkStepSound()
		{
			if (Defs.isSoundFX && !(stepSound == null) && audioSource.clip != stepSound)
			{
				audioSource.loop = true;
				audioSource.clip = stepSound;
				audioSource.Play();
			}
		}

		public void PlayRunStepSound()
		{
			if (Defs.isSoundFX && !(runStepSound == null) && audioSource.clip != runStepSound)
			{
				audioSource.loop = true;
				audioSource.clip = runStepSound;
				audioSource.Play();
			}
		}

		public void StopSteps()
		{
			if (!(stepSound == null) && (audioSource.clip == stepSound || audioSource.clip == runStepSound))
			{
				audioSource.Pause();
				audioSource.clip = null;
			}
		}

		public void TryPlayDeathSound(float delay)
		{
			if (Defs.isSoundFX && !(audioSource == null) && IsCanPlayDeathSound(delay))
			{
				audioSource.PlayOneShot(deathSound);
			}
		}

		public void TryPlayDamageSound(float delay)
		{
			if (Defs.isSoundFX && !(audioSource == null) && !_isPlayingDamageSound)
			{
				StartCoroutine(CheckCanPlayDamageAudio(delay));
				audioSource.PlayOneShot(damageSound);
			}
		}

		private IEnumerator CheckCanPlayDamageAudio(float timeOut)
		{
			_isPlayingDamageSound = true;
			yield return new WaitForSeconds(timeOut);
			_isPlayingDamageSound = false;
		}

		private IEnumerator ResetDeathAudio(float timeOut)
		{
			_isDeathAudioPlaying = true;
			yield return new WaitForSeconds(timeOut);
			_isDeathAudioPlaying = false;
		}

		private bool IsCanPlayDeathSound(float timeOut)
		{
			if (_isDeathAudioPlaying)
			{
				return false;
			}
			StartCoroutine(ResetDeathAudio(timeOut));
			return true;
		}

		[Obfuscation(Exclude = true)]
		public void PrepareDeath(bool isOwnerDamage = true)
		{
			if (!_isMultiplayerMode)
			{
				ZombieCreator.LastEnemy -= IncreaseRange;
			}
			botAiController.isDetectPlayer = false;
			botAiController.IsCanMove = false;
			IsDeath = true;
			float num = deathSound.length;
			TryPlayDeathSound(num);
			animations.Stop();
			if ((bool)animations[animationsName.Death])
			{
				animations.Play(animationsName.Death);
				num = Mathf.Max(num, animations[animationsName.Death].length);
				StartCoroutine(DelayedSetFallState(animations[animationsName.Death].length * 1.25f));
			}
			else
			{
				IsFalling = true;
			}
			StartCoroutine(DelayedDestroySelf(num));
			modelCollider.enabled = false;
			if (headCollider != null)
			{
				headCollider.enabled = false;
			}
			if (isOwnerDamage)
			{
				GlobalGameController.Score += scorePerKill;
			}
			CheckForceKillGuards();
		}

		private IEnumerator DelayedSetFallState(float delay)
		{
			yield return new WaitForSeconds(delay);
			IsFalling = true;
		}

		private IEnumerator DelayedDestroySelf(float delay)
		{
			yield return new WaitForSeconds(delay);
			if (!_isMultiplayerMode && !IsBotGuard())
			{
				ZombieCreator.sharedCreator.NumOfDeadZombies++;
			}
			DestroyByNetworkType();
		}

		private void CheckForceKillGuards()
		{
			if (guards.Length == 0)
			{
				return;
			}
			ZombieCreator sharedCreator = ZombieCreator.sharedCreator;
			if (sharedCreator == null)
			{
				return;
			}
			for (int i = 0; i < sharedCreator.bossGuads.Length; i++)
			{
				GameObject gameObject = sharedCreator.bossGuads[i];
				if (!(gameObject.gameObject == null))
				{
					BaseBot botScriptForObject = GetBotScriptForObject(gameObject.transform);
					if (!botScriptForObject.IsDeath)
					{
						botScriptForObject.GetDamage(-2.1474836E+09f, null, false);
					}
				}
			}
		}

		protected virtual void OnBotDestroyEvent()
		{
		}

		private void OnDestroy()
		{
			OnBotDestroyEvent();
			if (!_isMultiplayerMode)
			{
				ZombieCreator.LastEnemy -= IncreaseRange;
			}
			Initializer.enemiesObj.Remove(base.gameObject);
		}

		public static BaseBot GetBotScriptForObject(Transform obj)
		{
			return obj.GetComponent<BaseBot>();
		}

		public virtual bool CheckEnemyInAttackZone(float distanceToEnemy)
		{
			return false;
		}

		public virtual Vector3 GetHeadPoint()
		{
			Vector3 position = base.transform.position;
			position.y += ((!(headCollider != null)) ? (modelCollider.size.y * 0.75f) : headCollider.center.y);
			return position;
		}

		public virtual float GetMaxAttackDistance()
		{
			return GetMaxAttackDistance();
		}

		public static void LogDebugData(string message)
		{
		}

		private float CheckAnimationSpeedWalkMoveForBot(float modSpeed)
		{
			float num = speedAnimationWalk * modSpeed;
			animations[animationsName.Walk].speed = num;
			return num * attackingSpeed;
		}

		private float CheckAnimationSpeedRunMoveForBot(float modSpeed)
		{
			float num = speedAnimationRun * modSpeed;
			animations[animationsName.Run].speed = num;
			return num * notAttackingSpeed;
		}

		public float GetWalkSpeed()
		{
			if (isAutomaticAnimationEnable)
			{
				return CheckAnimationSpeedWalkMoveForBot(_modMoveSpeedByDebuff);
			}
			return notAttackingSpeed * _modMoveSpeedByDebuff;
		}

		public float GetAttackSpeedByCompleteLevel()
		{
			if (isAutomaticAnimationEnable)
			{
				return CheckAnimationSpeedRunMoveForBot(_modMoveSpeedByDebuff);
			}
			return attackingSpeed * _modMoveSpeedByDebuff;
		}

		public static Vector3 GetPositionSpawnGuard(Vector3 bossPosition)
		{
			float num = UnityEngine.Random.Range(0.5f, 1f);
			return bossPosition + new Vector3(num, num, num);
		}

		private bool IsBotGuard()
		{
			return base.gameObject.name.Contains("BossGuard");
		}

		private void ShowDamageTexture(bool isEnable, bool isPoison = false)
		{
			if (_botMaterials == null || _botMaterials.Length == 0)
			{
				return;
			}
			for (int i = 0; i < _botMaterials.Length; i++)
			{
				if (isEnable)
				{
					_botMaterials[i].ShowDamageEffect(isPoison);
				}
				else
				{
					_botMaterials[i].ResetMainMaterial();
				}
			}
		}

		private IEnumerator ShowDamageEffect(bool poison)
		{
			_isFlashing = true;
			ShowDamageTexture(true, poison);
			yield return new WaitForSeconds(0.125f);
			ShowDamageTexture(false);
			_isFlashing = false;
		}

		private void TakeBonusForKill()
		{
			if (isMobChampion && !_isWeaponCreated && LevelBox.weaponsFromBosses.ContainsKey(Application.loadedLevelName))
			{
				string weaponName = LevelBox.weaponsFromBosses[Application.loadedLevelName];
				Vector3 pos = base.gameObject.transform.position + new Vector3(0f, 0.25f, 0f);
				if (Application.loadedLevelName == "Sky_islands")
				{
					pos -= new Vector3(0f, 1.5f, 0f);
				}
				GameObject weaponBonus = ZombieCreator.sharedCreator.weaponBonus;
				GameObject gameObject = ((!(weaponBonus != null)) ? BonusCreator._CreateBonus(weaponName, pos) : BonusCreator._CreateBonusFromPrefab(weaponBonus, pos));
				gameObject.AddComponent<GotToNextLevel>();
				ZombieCreator.sharedCreator.weaponBonus = null;
				_isWeaponCreated = true;
			}
		}

		public void MakeDamage(Transform target, float damageValue)
		{
			IDamageable component = target.gameObject.GetComponent<IDamageable>();
			if (component != null)
			{
				component.ApplyDamage(damageValue, this, Player_move_c.TypeKills.mob);
			}
			TryPlayAudioClip(takeDamageSound);
		}

		public void MakeDamage(Transform target)
		{
			MakeDamage(target, damagePerHit);
		}

		private void ShowHeadShotEffect()
		{
			if (!Device.isPixelGunLow)
			{
				HitParticle currentParticle = HeadShotStackController.sharedController.GetCurrentParticle(false);
				if (currentParticle != null)
				{
					currentParticle.StartShowParticle(base.transform.position, base.transform.rotation, false, headCollider.transform.TransformPoint(headCollider.center));
				}
			}
		}

		private void ShowPoisonHitEffect()
		{
			if (Device.isPixelGunLow)
			{
				return;
			}
			HitParticle currentParticle = ParticleStacks.instance.poisonHitStack.GetCurrentParticle(false);
			if (currentParticle != null)
			{
				if (headCollider != null)
				{
					currentParticle.StartShowParticle(base.transform.position, base.transform.rotation, false, headCollider.transform.TransformPoint(headCollider.center));
				}
				else
				{
					currentParticle.StartShowParticle(base.transform.position, base.transform.rotation, false, base.transform.position + Vector3.up * heightFlyOutHitEffect);
				}
			}
		}

		private void ShowCriticalHitEffect()
		{
			if (Device.isPixelGunLow)
			{
				return;
			}
			HitParticle currentParticle = ParticleStacks.instance.criticalHitStack.GetCurrentParticle(false);
			if (currentParticle != null)
			{
				if (headCollider != null)
				{
					currentParticle.StartShowParticle(base.transform.position, base.transform.rotation, false, headCollider.transform.TransformPoint(headCollider.center));
				}
				else
				{
					currentParticle.StartShowParticle(base.transform.position, base.transform.rotation, false, base.transform.position + Vector3.up * heightFlyOutHitEffect);
				}
			}
		}

		private void ShowBleedingHitEffect()
		{
			if (Device.isPixelGunLow)
			{
				return;
			}
			HitParticle currentParticle = ParticleStacks.instance.bleedHitStack.GetCurrentParticle(false);
			if (currentParticle != null)
			{
				if (headCollider != null)
				{
					currentParticle.StartShowParticle(base.transform.position, base.transform.rotation, false, headCollider.transform.TransformPoint(headCollider.center));
				}
				else
				{
					currentParticle.StartShowParticle(base.transform.position, base.transform.rotation, false, base.transform.position + Vector3.up * heightFlyOutHitEffect);
				}
			}
		}

		private void ShowHitEffect()
		{
			if (Device.isPixelGunLow)
			{
				return;
			}
			HitParticle currentParticle = ParticleStacks.instance.hitStack.GetCurrentParticle(false);
			if (currentParticle != null)
			{
				if (headCollider != null)
				{
					currentParticle.StartShowParticle(base.transform.position, base.transform.rotation, false, headCollider.transform.TransformPoint(headCollider.center));
				}
				else
				{
					currentParticle.StartShowParticle(base.transform.position, base.transform.rotation, false, base.transform.position + Vector3.up * heightFlyOutHitEffect);
				}
			}
		}

		public void ApplyDamage(float damage, IDamageable damageFrom, Player_move_c.TypeKills typeKill)
		{
			ApplyDamage(damage, damageFrom, typeKill, WeaponSounds.TypeDead.angel, string.Empty);
		}

		public void ApplyDamage(float damage, IDamageable damageFrom, Player_move_c.TypeKills typeKill, WeaponSounds.TypeDead typeDead, string weaponName, int killerId = 0)
		{
			if (Defs.isMulti)
			{
				if (!IsDeath)
				{
					GetDamageForMultiplayer(0f - damage, weaponName, typeKill);
					WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().score = GlobalGameController.Score;
					WeaponManager.sharedManager.myNetworkStartTable.SynhScore();
				}
			}
			else
			{
				Transform transform = null;
				if (damageFrom != null)
				{
					transform = (damageFrom as MonoBehaviour).transform;
				}
				GetDamage(0f - damage, transform, weaponName, transform, typeKill);
			}
		}

		public bool IsEnemyTo(Player_move_c player)
		{
			return true;
		}

		public bool IsDead()
		{
			return IsDeath;
		}

		public void GetDamage(float damage, Transform instigator, string weaponName, bool isOwnerDamage = true, Player_move_c.TypeKills typeKills = Player_move_c.TypeKills.none)
		{
			GetDamage(damage, instigator, isOwnerDamage, typeKills);
			if (_killed)
			{
				if (Application.isEditor)
				{
					Debug.LogWarning("Bot is receiving damage after death.");
				}
			}
			else
			{
				if (health != 0f || !isOwnerDamage)
				{
					return;
				}
				_killed = true;
				if (TrainingController.TrainingCompleted && (!Defs.isMulti || !NetworkStartTable.LocalOrPasswordRoom()) && !string.IsNullOrEmpty(weaponName))
				{
					ShopNGUIController.CategoryNames weaponSlot = (ShopNGUIController.CategoryNames)(-1);
					string prefabName = weaponName.Replace("(Clone)", string.Empty);
					ItemRecord byPrefabName = ItemDb.GetByPrefabName(prefabName);
					if (byPrefabName != null)
					{
						int itemCategory = ItemDb.GetItemCategory(byPrefabName.Tag);
						weaponSlot = (ShopNGUIController.CategoryNames)itemCategory;
					}
					bool campaign = !Defs.isMulti && !Defs.IsSurvival;
					QuestMediator.NotifyKillMonster(weaponSlot, campaign);
				}
			}
		}

		public void GetDamage(float damage, Transform instigator, bool isOwnerDamage = true, Player_move_c.TypeKills typeKills = Player_move_c.TypeKills.none)
		{
			if (IsDeath)
			{
				return;
			}
			if (damage < 0f && !_isFlashing)
			{
				StartCoroutine(ShowDamageEffect(typeKills == Player_move_c.TypeKills.poison));
			}
			switch (typeKills)
			{
			case Player_move_c.TypeKills.headshot:
				ShowHeadShotEffect();
				damage *= 2f;
				break;
			case Player_move_c.TypeKills.poison:
				ShowPoisonHitEffect();
				break;
			case Player_move_c.TypeKills.critical:
				ShowCriticalHitEffect();
				break;
			case Player_move_c.TypeKills.bleeding:
				ShowBleedingHitEffect();
				break;
			default:
				ShowHitEffect();
				break;
			}
			health += damage;
			if (health < 0f)
			{
				health = 0f;
			}
			if (health == 0f)
			{
				PrepareDeath(isOwnerDamage);
				if (isOwnerDamage)
				{
					TakeBonusForKill();
				}
			}
			else if (isOwnerDamage)
			{
				GlobalGameController.Score += 5;
			}
			TryPlayDamageSound(damageSound.length);
			if (instigator != null && health > 0f)
			{
				botAiController.SetTargetForced(instigator);
			}
		}

		private BotDebuff GetDebuffByType(BotDebuffType type)
		{
			for (int i = 0; i < _botDebufs.Count; i++)
			{
				if (_botDebufs[i].type == type)
				{
					return _botDebufs[i];
				}
			}
			return null;
		}

		private void RunDebuff(BotDebuff debuff)
		{
			if (debuff.type == BotDebuffType.DecreaserSpeed)
			{
				float floatParametr = debuff.GetFloatParametr();
				_modMoveSpeedByDebuff = floatParametr;
			}
		}

		private void StopDebuff(BotDebuff debuff)
		{
			if (debuff.type == BotDebuffType.DecreaserSpeed)
			{
				_modMoveSpeedByDebuff = 1f;
			}
		}

		public void ApplyDebuffByMode(BotDebuffType type, float timeLife, object parametrs)
		{
			if (!_isMultiplayerMode)
			{
				ApplyDebuff(type, timeLife, parametrs);
			}
			else
			{
				ApplyDebufForMultiplayer(type, timeLife, parametrs);
			}
		}

		private void ReplaceDebuff(BotDebuff oldDebuff, float newTimeLife, object newParametrs)
		{
			if (oldDebuff.type == BotDebuffType.DecreaserSpeed)
			{
				oldDebuff.ReplaceValues(newTimeLife, newParametrs);
				RunDebuff(oldDebuff);
			}
		}

		public void ApplyDebuff(BotDebuffType type, float timeLife, object parametrs)
		{
			BotDebuff debuffByType = GetDebuffByType(type);
			if (debuffByType == null)
			{
				BotDebuff botDebuff = new BotDebuff(type, timeLife, parametrs);
				botDebuff.OnRun += RunDebuff;
				botDebuff.OnStop += StopDebuff;
				_botDebufs.Add(botDebuff);
			}
			else
			{
				ReplaceDebuff(debuffByType, timeLife, parametrs);
			}
		}

		public void UpdateDebuffState()
		{
			if (_botDebufs.Count == 0)
			{
				return;
			}
			for (int i = 0; i < _botDebufs.Count; i++)
			{
				if (!_botDebufs[i].isRun)
				{
					_botDebufs[i].Run();
					continue;
				}
				_botDebufs[i].timeLife -= Time.deltaTime;
				if (_botDebufs[i].timeLife <= 0f)
				{
					_botDebufs[i].Stop();
					_botDebufs.Remove(_botDebufs[i]);
				}
			}
		}

		private void InitNetworkStateData()
		{
			_botPosition = base.transform.position;
			_botRotation = base.transform.rotation;
		}

		private void DisableMobForDeleteMasterClient()
		{
			modelCollider.gameObject.SetActive(false);
			if (headCollider != null)
			{
				headCollider.gameObject.SetActive(false);
			}
			MonoBehaviour[] components = GetComponents<MonoBehaviour>();
			for (int i = 0; i < components.Length; i++)
			{
				bool flag = components[i] as PhotonView != null;
				bool flag2 = components[i] as BaseBot != null;
				if (!flag && !flag2)
				{
					components[i].enabled = false;
				}
			}
		}

		public void DestroyByNetworkType()
		{
			if (!_isMultiplayerMode)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
			needDestroyByMasterClient = true;
			DisableMobForDeleteMasterClient();
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (_isMultiplayerMode)
			{
				if (stream.isWriting)
				{
					stream.SendNext(base.transform.position);
					stream.SendNext(base.transform.rotation);
				}
				else
				{
					_botPosition = (Vector3)stream.ReceiveNext();
					_botRotation = (Quaternion)stream.ReceiveNext();
				}
			}
		}

		private void Update()
		{
			UpdateDebuffState();
			if (GetComponent<AudioSource>().enabled == (Time.timeScale == 0f))
			{
				GetComponent<AudioSource>().enabled = !GetComponent<AudioSource>().enabled;
				if (GetComponent<AudioSource>().enabled)
				{
					GetComponent<AudioSource>().Play();
				}
			}
			if (_isMultiplayerMode)
			{
				if (PhotonNetwork.isMasterClient && needDestroyByMasterClient)
				{
					PhotonNetwork.Destroy(base.gameObject);
				}
				if (!_photonView.isMine && !needDestroyByMasterClient)
				{
					base.transform.position = Vector3.Lerp(base.transform.position, _botPosition, Time.deltaTime * 5f);
					base.transform.rotation = Quaternion.Lerp(base.transform.rotation, _botRotation, Time.deltaTime * 5f);
				}
			}
		}

		public void FireByRPC(Vector3 pointFire, Vector3 positionToFire)
		{
			_photonView.RPC("FireBulletRPC", PhotonTargets.Others, pointFire, positionToFire);
		}

		[PunRPC]
		[RPC]
		public void SetBotHealthRPC(float botHealth)
		{
			health = botHealth;
		}

		[PunRPC]
		[RPC]
		public void PlayZombieRunRPC()
		{
			PlayAnimationZombieWalk();
			_currentRunNetworkAnimation = RunNetworkAnimationType.ZombieWalk;
		}

		[RPC]
		[PunRPC]
		public void PlayZombieAttackRPC()
		{
			PlayAnimationZombieAttackOrStop();
			_currentRunNetworkAnimation = RunNetworkAnimationType.ZombieAttackOrStop;
		}

		[RPC]
		[PunRPC]
		public void GetDamageRPC(float damage, int _typeKills)
		{
			GetDamage(damage, null, false, (Player_move_c.TypeKills)_typeKills);
		}

		public void GetDamageForMultiplayer(float damage, string weaponName, Player_move_c.TypeKills typeKills)
		{
			GetDamage(damage, null, weaponName, true, typeKills);
			_photonView.RPC("GetDamageRPC", PhotonTargets.Others, damage, (int)typeKills);
		}

		[RPC]
		[PunRPC]
		public void ApplyDebuffRPC(int typeDebuff, float timeLife, float parametr)
		{
			ApplyDebuff((BotDebuffType)typeDebuff, timeLife, parametr);
		}

		public void ApplyDebufForMultiplayer(BotDebuffType type, float timeLife, object parametrs)
		{
			ApplyDebuff(type, timeLife, parametrs);
			if (type == BotDebuffType.DecreaserSpeed)
			{
				_photonView.RPC("ApplyDebuffRPC", _photonView.owner, (int)type, timeLife, (float)parametrs);
			}
		}

		public void PlayAnimZombieWalkByMode()
		{
			if (!_isMultiplayerMode)
			{
				PlayAnimationZombieWalk();
				return;
			}
			if (_currentRunNetworkAnimation != 0)
			{
				PlayAnimationZombieWalk();
				_photonView.RPC("PlayZombieRunRPC", PhotonTargets.Others);
			}
			_currentRunNetworkAnimation = RunNetworkAnimationType.ZombieWalk;
		}

		public void PlayAnimZombieAttackOrStopByMode()
		{
			if (!_isMultiplayerMode)
			{
				PlayAnimationZombieAttackOrStop();
				return;
			}
			if (_currentRunNetworkAnimation != RunNetworkAnimationType.ZombieAttackOrStop)
			{
				PlayAnimationZombieAttackOrStop();
				_photonView.RPC("PlayZombieAttackRPC", PhotonTargets.Others);
			}
			_currentRunNetworkAnimation = RunNetworkAnimationType.ZombieAttackOrStop;
		}
	}
}

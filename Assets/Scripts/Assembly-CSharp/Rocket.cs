using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Rilisoft;
using RilisoftBot;
using UnityEngine;

public sealed class Rocket : MonoBehaviour
{
	private readonly float[] lightningCoefs = new float[5] { 1f, 0.5f, 0.25f, 0.125f, 0.0625f };

	[NonSerialized]
	public RocketSettings currentRocketSettings;

	[NonSerialized]
	public bool isRun;

	[NonSerialized]
	public string explosionName;

	[NonSerialized]
	public string weaponPrefabName;

	[NonSerialized]
	public float damage;

	[NonSerialized]
	public Vector2 damageRange;

	[NonSerialized]
	public float multiplayerDamage;

	[NonSerialized]
	public float radiusDamage;

	[NonSerialized]
	public float radiusDamageSelf;

	[NonSerialized]
	public float radiusImpulse;

	[NonSerialized]
	public float impulseForce;

	[NonSerialized]
	public float impulseForceSelf;

	[NonSerialized]
	public bool isSlowdown;

	[NonSerialized]
	public float slowdownTime;

	[NonSerialized]
	public float slowdownCoeff;

	[NonSerialized]
	public float chargePower = 1f;

	[NonSerialized]
	public bool isMine;

	private WeaponSounds currentWeaponSounds;

	private Dictionary<string, RocketSettings> rocketsDict = new Dictionary<string, RocketSettings>();

	private PhotonView photonView;

	private bool isKilled;

	private Player_move_c myPlayerMoveC;

	private Vector3 correctPos = Vector3.down * 1000f;

	private Transform thisTransform;

	private bool isFirstPos = true;

	private float lastToxicHit;

	private int counterJumpLightning;

	private int lightningHitCount;

	private float timerFromJumpLightning = 1f;

	private Transform targetLightning;

	private Transform targetDamageLightning;

	private float maxTimerFromJumpLightning = 1f;

	private float progressCaptureTargetLightning;

	private bool isDetectFirstTargetLightning;

	private Transform targetAutoHoming;

	private Transform megabulletLastTarget;

	private Transform stickedObject;

	private Vector3 stickedObjectPos;

	private Rigidbody myRigidbody;

	private bool isActivated;

	private int _rocketNum;

	public Action OnExplode;

	private Vector3 dronePoint;

	private bool isStickedToPlayer;

	public int rocketNum
	{
		get
		{
			return _rocketNum;
		}
		set
		{
			_rocketNum = value;
			StartCoroutine(SetCurrentRocket(_rocketNum));
		}
	}

	private bool IsHitInDamageRadius(Vector3 targetPos, Vector3 selfPos, float radius)
	{
		return (targetPos - selfPos).sqrMagnitude < radius * radius;
	}

	private float GetCoefDamageAtPoint(Vector3 pos)
	{
		float num = Vector3.SqrMagnitude(thisTransform.position - pos);
		return 1f - 0.3f * num / (radiusDamage * radiusDamage);
	}

	public IEnumerator Hit(Collider hitCollider)
	{
		if (Defs.isMulti && (WeaponManager.sharedManager.myPlayer == null || !isMine))
		{
			yield break;
		}
		while (currentRocketSettings == null)
		{
			yield return null;
		}
		GameObject hitObject = null;
		GameObject hitObjectRoot2 = null;
		Transform hitObjectRootTransform = null;
		if (hitCollider != null)
		{
			hitObject = hitCollider.gameObject;
			hitObjectRootTransform = hitCollider.gameObject.transform.root;
			hitObjectRoot2 = hitObjectRootTransform.gameObject;
		}
		if (currentRocketSettings.typeDead == WeaponSounds.TypeDead.like)
		{
			LikeHit(hitObject.transform);
			yield break;
		}
		if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.BlackMark)
		{
			BlackMarkHit(hitObject.transform);
			yield break;
		}
		if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Lightning)
		{
			SendShowExplosion();
		}
		Vector3 point = thisTransform.position;
		Initializer.TargetsList targets = new Initializer.TargetsList(WeaponManager.sharedManager.myPlayerMoveC, Defs.isMulti);
		foreach (Transform target in targets)
		{
			bool _isHit = false;
			bool _isHeadShot = false;
			if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Lightning)
			{
				_isHit = target.Equals(targetDamageLightning);
			}
			else if (IsDamageByRadius())
			{
				if (IsHitInDamageRadius(target.position, point, radiusDamage))
				{
					_isHit = true;
				}
			}
			else if (target.Equals(hitObjectRootTransform))
			{
				_isHit = true;
				if (currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.ToxicBomb)
				{
					_isHeadShot = hitObject.name == "HeadCollider" || hitCollider is SphereCollider;
				}
			}
			if (_isHit)
			{
				HitIDestructible(target, _isHeadShot);
			}
		}
	}

	private void HitIDestructible(Transform _obj, bool _isHeadShot)
	{
		IDamageable component = _obj.GetComponent<IDamageable>();
		if (component == null)
		{
			return;
		}
		if (Defs.isDaterRegim)
		{
			if (currentWeaponSounds.isDaterWeapon && component is PlayerDamageable)
			{
				Player_move_c myPlayer = (component as PlayerDamageable).myPlayer;
				if (!myPlayer.Equals(WeaponManager.sharedManager.myPlayerMoveC) && !myPlayer.isMechActive)
				{
					myPlayer.SendDaterChat(WeaponManager.sharedManager.myPlayerMoveC.mySkinName.NickName, WeaponManager.sharedManager.currentWeaponSounds.daterMessage, myPlayer.mySkinName.NickName);
				}
			}
			return;
		}
		float num = 1f;
		if (_isHeadShot)
		{
			num = 2f + EffectsController.AddingForHeadshot(currentWeaponSounds.categoryNabor - 1);
		}
		if (IsDamageByRadius())
		{
			num *= GetCoefDamageAtPoint(_obj.position);
		}
		if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Lightning)
		{
			num *= lightningCoefs[lightningHitCount];
			lightningHitCount++;
		}
		if (_obj.gameObject.Equals(WeaponManager.sharedManager.myPlayer))
		{
			num *= EffectsController.SelfExplosionDamageDecreaseCoef;
		}
		num *= WeaponManager.sharedManager.myPlayerMoveC.DamageMultiplierByGadgets();
		float num2 = multiplayerDamage;
		num2 *= num;
		WeaponSounds.TypeDead typeDead = currentRocketSettings.typeDead;
		Player_move_c.TypeKills typeKill = ((!_isHeadShot) ? currentRocketSettings.typeKilsIconChat : Player_move_c.TypeKills.headshot);
		string weaponName = weaponPrefabName;
		if (IsGrenadeWeaponName(weaponPrefabName))
		{
			weaponName = GadgetsInfo.BaseName(weaponPrefabName);
		}
		component.ApplyDamage(num2, myPlayerMoveC.myDamageable, typeKill, typeDead, weaponName, WeaponManager.sharedManager.myPlayer.GetComponent<PixelView>().viewID);
		if (currentWeaponSounds.isPoisoning)
		{
			WeaponManager.sharedManager.myPlayerMoveC.PoisonShotWithEffect(_obj.gameObject, new Player_move_c.PoisonParameters(multiplayerDamage, currentWeaponSounds));
		}
		if (currentWeaponSounds.isDamageHeal && component.isLivingTarget)
		{
			WeaponManager.sharedManager.myPlayerMoveC.AddHealth(multiplayerDamage * currentWeaponSounds.damageHealMultiplier);
		}
		if (isSlowdown)
		{
			WeaponManager.sharedManager.myPlayerMoveC.SlowdownTarget(component, slowdownTime, slowdownCoeff);
		}
	}

	private void LikeHit(Transform go)
	{
		if (WeaponManager.sharedManager.myPlayerMoveC == null)
		{
			return;
		}
		Player_move_c player_move_c = null;
		if (go != null && go.CompareTag("Player"))
		{
			player_move_c = go.GetComponent<SkinName>().playerMoveC;
		}
		else
		{
			float num = float.MaxValue;
			for (int i = 0; i < Initializer.players.Count; i++)
			{
				Player_move_c player_move_c2 = Initializer.players[i];
				if (!player_move_c2.Equals(WeaponManager.sharedManager.myPlayerMoveC))
				{
					float num2 = Vector3.SqrMagnitude(Initializer.players[i].myPlayerTransform.position - thisTransform.position);
					if (num2 < radiusDamage * radiusDamage && num2 < num)
					{
						num = num2;
						player_move_c = player_move_c2;
					}
				}
			}
		}
		if (player_move_c != null)
		{
			WeaponManager.sharedManager.myPlayerMoveC.SendLike(player_move_c);
		}
	}

	private void BlackMarkHit(Transform go)
	{
		if (WeaponManager.sharedManager.myPlayerMoveC == null)
		{
			return;
		}
		Player_move_c player_move_c = null;
		if (go != null && go.CompareTag("Player") && Initializer.IsEnemyTarget(go))
		{
			player_move_c = go.GetComponent<SkinName>().playerMoveC;
		}
		else
		{
			float num = float.MaxValue;
			for (int i = 0; i < Initializer.players.Count; i++)
			{
				if (!Initializer.IsEnemyTarget(Initializer.players[i].myPlayerTransform))
				{
					continue;
				}
				Player_move_c player_move_c2 = Initializer.players[i];
				if (!player_move_c2.Equals(WeaponManager.sharedManager.myPlayerMoveC))
				{
					float num2 = Vector3.SqrMagnitude(Initializer.players[i].myPlayerTransform.position - thisTransform.position);
					if (num2 < radiusDamage * radiusDamage && num2 < num)
					{
						num = num2;
						player_move_c = player_move_c2;
					}
				}
			}
		}
		if (player_move_c != null)
		{
			WeaponManager.sharedManager.myPlayerMoveC.BlackMarkPlayer(player_move_c, GadgetsInfo.info[weaponPrefabName]);
		}
	}

	private void Update()
	{
		if (myPlayerMoveC == null)
		{
			SetMyPlayerMoveC();
		}
		if (Defs.isMulti && !isMine && isRun)
		{
			InterpolatePos();
			if (currentRocketSettings != null)
			{
				RocketSettings.TypeFlyRocket typeFly = currentRocketSettings.typeFly;
				if (typeFly == RocketSettings.TypeFlyRocket.Drone)
				{
					UpdateTargetDroneForOthers();
				}
			}
		}
		else if (!(currentRocketSettings == null) && (!Defs.isMulti || isMine))
		{
			switch (currentRocketSettings.typeFly)
			{
			case RocketSettings.TypeFlyRocket.Autoaim:
			case RocketSettings.TypeFlyRocket.AutoaimBullet:
				UpdateForceForAutoaim();
				break;
			case RocketSettings.TypeFlyRocket.AutoTarget:
			case RocketSettings.TypeFlyRocket.HomingGrenade:
				UpdateForceForAutoHoming();
				break;
			case RocketSettings.TypeFlyRocket.Lightning:
				UpdateTargetForLightning();
				break;
			case RocketSettings.TypeFlyRocket.StickyBomb:
			case RocketSettings.TypeFlyRocket.StickyMine:
				UpdateTargetStickedBomb();
				break;
			case RocketSettings.TypeFlyRocket.ToxicBomb:
				UpdateTargetToxicBomb();
				break;
			case RocketSettings.TypeFlyRocket.Drone:
				UpdateTargetDrone();
				break;
			case RocketSettings.TypeFlyRocket.FakeBonus:
				UpdateFakeBonus();
				break;
			}
		}
	}

	private void UpdateFakeBonus()
	{
		if (!isRun)
		{
			return;
		}
		if (isActivated)
		{
			Initializer.TargetsList targetsList = new Initializer.TargetsList();
			{
				foreach (Transform item in targetsList)
				{
					if ((item.position - thisTransform.position).sqrMagnitude < currentRocketSettings.raduisDetectTarget * currentRocketSettings.raduisDetectTarget)
					{
						KillRocket();
					}
				}
				return;
			}
		}
		base.transform.rotation = Quaternion.identity;
		myRigidbody.angularVelocity = Vector3.zero;
	}

	private void UpdateForceForAutoaim()
	{
		if (isRun && WeaponManager.sharedManager.myPlayerMoveC != null && !WeaponManager.sharedManager.myPlayerMoveC.isKilled)
		{
			Vector3 pointAutoAim = WeaponManager.sharedManager.myPlayerMoveC.GetPointAutoAim(thisTransform.position);
			Vector3 normalized = (pointAutoAim - thisTransform.position).normalized;
			myRigidbody.AddForce(normalized * 27f);
			myRigidbody.velocity = myRigidbody.velocity.normalized * currentRocketSettings.autoRocketForce;
			thisTransform.rotation = Quaternion.LookRotation(myRigidbody.velocity);
		}
	}

	private void UpdateForceForAutoHoming()
	{
		if (!isRun)
		{
			return;
		}
		if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.HomingGrenade)
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f), Time.deltaTime * 2f);
			myRigidbody.velocity = Vector3.Lerp(myRigidbody.velocity, new Vector3(myRigidbody.velocity.x, 0f, myRigidbody.velocity.z), Time.deltaTime * 2f);
		}
		if (targetAutoHoming == null || IsKilledTarget(targetAutoHoming) || (targetAutoHoming.position - thisTransform.position).sqrMagnitude > (currentRocketSettings.raduisDetectTarget + 1f) * (currentRocketSettings.raduisDetectTarget + 1f))
		{
			targetAutoHoming = FindNearestTarget(45f);
		}
		if (targetAutoHoming != null)
		{
			Vector3 vector = Vector3.zero;
			if (targetAutoHoming.childCount > 0 && targetAutoHoming.GetChild(0).GetComponent<BoxCollider>() != null)
			{
				vector = targetAutoHoming.GetChild(0).GetComponent<BoxCollider>().center;
			}
			Vector3 normalized = (targetAutoHoming.position + vector - thisTransform.position).normalized;
			myRigidbody.AddForce(normalized * 9f);
			myRigidbody.velocity = myRigidbody.velocity.normalized * currentRocketSettings.autoRocketForce;
			thisTransform.rotation = Quaternion.LookRotation(myRigidbody.velocity);
		}
	}

	private void UpdateTargetForLightning()
	{
		if (targetDamageLightning != null)
		{
			myRigidbody.isKinematic = true;
			targetLightning = FindLightningTarget();
			if (targetLightning == null)
			{
				thisTransform.position = targetDamageLightning.position;
				timerFromJumpLightning -= Time.deltaTime;
				if (timerFromJumpLightning <= 0f)
				{
					counterJumpLightning++;
					if (counterJumpLightning > currentRocketSettings.countJumpLightning)
					{
						KillRocket();
					}
					else
					{
						StartCoroutine(Hit(null));
					}
					timerFromJumpLightning = maxTimerFromJumpLightning;
				}
			}
			else
			{
				targetDamageLightning = null;
				progressCaptureTargetLightning = 0f;
			}
		}
		if (targetLightning != null)
		{
			if (!IsKilledTarget(targetLightning))
			{
				thisTransform.position = Vector3.Lerp(thisTransform.position, targetLightning.position, progressCaptureTargetLightning + 5f * Time.deltaTime);
			}
			else
			{
				KillRocket();
			}
		}
		else if (isDetectFirstTargetLightning && (targetDamageLightning == null || IsKilledTarget(targetDamageLightning)))
		{
			KillRocket();
		}
	}

	private void UpdateTargetStickedBomb()
	{
		if (!isRun || !myRigidbody.isKinematic)
		{
			return;
		}
		if (stickedObject != null && stickedObjectPos != stickedObject.position)
		{
			KillRocket();
			return;
		}
		Initializer.TargetsList targetsList = new Initializer.TargetsList();
		foreach (Transform item in targetsList)
		{
			if ((item.position - thisTransform.position).sqrMagnitude < currentRocketSettings.raduisDetectTarget * currentRocketSettings.raduisDetectTarget)
			{
				KillRocket();
			}
		}
	}

	private void UpdateTargetDrone()
	{
		if (!isRun || !isActivated)
		{
			return;
		}
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f), Time.deltaTime * 2f);
		myRigidbody.angularVelocity = Vector3.MoveTowards(myRigidbody.angularVelocity, Vector3.zero, Time.time * 10f);
		Vector3 normalized = (dronePoint - base.transform.position).normalized;
		float sqrMagnitude = (dronePoint - base.transform.position).sqrMagnitude;
		myRigidbody.AddForce(normalized * Mathf.Min(8f, sqrMagnitude));
		myRigidbody.velocity = myRigidbody.velocity.normalized * Mathf.Clamp(sqrMagnitude, 0f, currentRocketSettings.autoRocketForce);
		if (lastToxicHit > Time.time)
		{
			return;
		}
		Initializer.TargetsList targetsList = new Initializer.TargetsList();
		foreach (Transform item in targetsList)
		{
			if ((item.position - thisTransform.position).sqrMagnitude < currentRocketSettings.raduisDetectTarget * currentRocketSettings.raduisDetectTarget && WeaponManager.sharedManager.myPlayerMoveC != null)
			{
				lastToxicHit = Time.time + currentRocketSettings.toxicHitTime;
				WeaponManager.sharedManager.myPlayerMoveC.DamageTarget(item.gameObject, multiplayerDamage, weaponPrefabName, currentRocketSettings.typeDead, currentRocketSettings.typeKilsIconChat);
			}
		}
	}

	private void UpdateTargetDroneForOthers()
	{
		if (isRun && isActivated && !(WeaponManager.sharedManager.myPlayerMoveC == null) && !(myPlayerMoveC == null) && !Defs.isCOOP && (!ConnectSceneNGUIController.isTeamRegim || myPlayerMoveC.myCommand != WeaponManager.sharedManager.myPlayerMoveC.myCommand) && (WeaponManager.sharedManager.myPlayerMoveC.transform.position - thisTransform.position).sqrMagnitude < currentRocketSettings.raduisDetectTarget * currentRocketSettings.raduisDetectTarget)
		{
			lastToxicHit = Time.time + currentRocketSettings.toxicHitTime;
			WeaponManager.sharedManager.myPlayerMoveC.SlowdownRPC(0.5f, currentRocketSettings.toxicHitTime);
		}
	}

	private void UpdateTargetToxicBomb()
	{
		if (!isRun || !myRigidbody.isKinematic)
		{
			return;
		}
		if (stickedObject != null && stickedObjectPos != stickedObject.position)
		{
			KillRocket();
		}
		else
		{
			if (lastToxicHit > Time.time)
			{
				return;
			}
			Initializer.TargetsList targetsList = new Initializer.TargetsList();
			foreach (Transform item in targetsList)
			{
				if ((item.position - thisTransform.position).sqrMagnitude < currentRocketSettings.raduisDetectTarget * currentRocketSettings.raduisDetectTarget && WeaponManager.sharedManager.myPlayerMoveC != null)
				{
					lastToxicHit = Time.time + currentRocketSettings.toxicHitTime;
					WeaponManager.sharedManager.myPlayerMoveC.DamageTarget(item.gameObject, multiplayerDamage * currentRocketSettings.toxicDamageMultiplier, weaponPrefabName, currentRocketSettings.typeDead, Player_move_c.TypeKills.poison);
				}
			}
		}
	}

	private bool IsKilledTarget(Transform _target)
	{
		if (_target == null)
		{
			return true;
		}
		if (_target.GetComponent<SkinName>() != null)
		{
			return _target.GetComponent<SkinName>().playerMoveC.isKilled;
		}
		if (_target.GetComponent<TurretController>() != null)
		{
			return _target.GetComponent<TurretController>().isKilled;
		}
		if (_target.GetComponent<BaseBot>() != null)
		{
			return _target.GetComponent<BaseBot>().IsDeath;
		}
		if (_target.GetComponent<BaseDummy>() != null)
		{
			return _target.GetComponent<BaseDummy>().isDead;
		}
		return true;
	}

	private Transform FindLightningTarget()
	{
		Transform result = null;
		float num = float.MaxValue;
		Initializer.TargetsList targetsList = new Initializer.TargetsList();
		foreach (Transform item in targetsList)
		{
			if (!item.Equals(targetDamageLightning))
			{
				float num2 = Vector3.SqrMagnitude(thisTransform.position - item.position);
				RaycastHit hitInfo;
				if (num2 < currentRocketSettings.raduisDetectTargetLightning * currentRocketSettings.raduisDetectTargetLightning && num2 < num && Physics.Raycast(thisTransform.position, item.position - thisTransform.position, out hitInfo, currentRocketSettings.raduisDetectTargetLightning, Player_move_c._ShootRaycastLayerMask) && hitInfo.collider.gameObject != null && hitInfo.collider.gameObject.transform.root.Equals(item))
				{
					result = item;
					num = num2;
				}
			}
		}
		return result;
	}

	private Transform FindNearestTarget(float searchAngle)
	{
		Transform result = null;
		float num = float.MaxValue;
		Initializer.TargetsList targetsList = new Initializer.TargetsList();
		foreach (Transform item in targetsList)
		{
			if (item.Equals(targetAutoHoming))
			{
				continue;
			}
			Vector3 a = item.position - thisTransform.position;
			if (!(Vector3.Angle(myRigidbody.velocity.normalized, a.normalized) > searchAngle))
			{
				float num2 = Vector3.SqrMagnitude(a);
				RaycastHit hitInfo;
				if (num2 < currentRocketSettings.raduisDetectTarget * currentRocketSettings.raduisDetectTarget && num2 < num && Physics.Raycast(thisTransform.position, item.position - thisTransform.position, out hitInfo, currentRocketSettings.raduisDetectTarget, Player_move_c._ShootRaycastLayerMask) && hitInfo.collider.gameObject != null && hitInfo.collider.gameObject.transform.root.Equals(item))
				{
					result = item;
					num = num2;
				}
			}
		}
		return result;
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			if (isRun)
			{
				stream.SendNext(thisTransform.position);
				stream.SendNext(thisTransform.rotation);
			}
		}
		else
		{
			correctPos = (Vector3)stream.ReceiveNext();
			thisTransform.rotation = (Quaternion)stream.ReceiveNext();
		}
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			if (isRun)
			{
				Vector3 value = thisTransform.position;
				Quaternion value2 = thisTransform.rotation;
				stream.Serialize(ref value);
				stream.Serialize(ref value2);
			}
			return;
		}
		Vector3 value3 = Vector3.zero;
		Quaternion value4 = Quaternion.identity;
		stream.Serialize(ref value3);
		stream.Serialize(ref value4);
		correctPos = value3;
		thisTransform.rotation = value4;
		if (isFirstPos)
		{
			isFirstPos = false;
			thisTransform.position = value3;
			thisTransform.rotation = value4;
		}
	}

	private void InterpolatePos()
	{
		if (Vector3.SqrMagnitude(thisTransform.position - correctPos) > 3000f)
		{
			thisTransform.position = correctPos;
		}
		else
		{
			thisTransform.position = Vector3.Lerp(thisTransform.position, correctPos, Time.deltaTime * 5f);
		}
	}

	private bool IsGrenadeWeaponName(string _weaponName)
	{
		return !_weaponName.Contains("Weapon");
	}

	private bool IsGravityRocket(RocketSettings _rs)
	{
		return _rs.typeFly == RocketSettings.TypeFlyRocket.ToxicBomb || _rs.typeFly == RocketSettings.TypeFlyRocket.StickyBomb || _rs.typeFly == RocketSettings.TypeFlyRocket.StickyMine || _rs.typeFly == RocketSettings.TypeFlyRocket.Firework || _rs.typeFly == RocketSettings.TypeFlyRocket.FakeBonus || _rs.typeFly == RocketSettings.TypeFlyRocket.Bomb || _rs.typeFly == RocketSettings.TypeFlyRocket.BlackMark || _rs.typeFly == RocketSettings.TypeFlyRocket.GravityRocket;
	}

	private bool IsDamageByRadius()
	{
		return currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Grenade || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.GrenadeBouncing || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Bomb || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Rocket || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.ChargeRocket || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.GravityRocket || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.BlackMark || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Autoaim || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Ball || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.AutoTarget || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.StickyBomb || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.FakeBonus || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Firework || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.StickyMine || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.NuclearGrenade || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.HomingGrenade;
	}

	private void Awake()
	{
		photonView = GetComponent<PhotonView>();
		myRigidbody = GetComponent<Rigidbody>();
		thisTransform = base.transform;
		if (Defs.isMulti)
		{
			if (!Defs.isInet)
			{
				isMine = GetComponent<NetworkView>().isMine;
				return;
			}
			isMine = photonView.isMine;
			if (isMine)
			{
				PhotonObjectCacher.AddObject(base.gameObject);
			}
		}
		else
		{
			isMine = true;
		}
	}

	private void Start()
	{
		SetMyPlayerMoveC();
	}

	private void SetMyPlayerMoveC()
	{
		if (Defs.isMulti && !isMine)
		{
			myRigidbody.isKinematic = true;
			GetComponent<BoxCollider>().enabled = false;
			if (!Defs.isInet)
			{
				return;
			}
			for (int i = 0; i < Initializer.players.Count; i++)
			{
				if (Initializer.players[i].mySkinName.photonView != null && photonView.ownerId == Initializer.players[i].mySkinName.photonView.ownerId)
				{
					myPlayerMoveC = Initializer.players[i];
					break;
				}
			}
		}
		else
		{
			myPlayerMoveC = WeaponManager.sharedManager.myPlayerMoveC;
		}
	}

	public void SendNetworkViewMyPlayer(NetworkViewID myId)
	{
		GetComponent<NetworkView>().RPC("SendNetworkViewMyPlayerRPC", RPCMode.AllBuffered, myId);
	}

	[RPC]
	[PunRPC]
	public void SendNetworkViewMyPlayerRPC(NetworkViewID myId)
	{
		for (int i = 0; i < Initializer.players.Count; i++)
		{
			if (myId.Equals(Initializer.players[i].mySkinName.GetComponent<NetworkView>().viewID))
			{
				myPlayerMoveC = Initializer.players[i];
				break;
			}
		}
	}

	public void SendSetRocketActive(string _weaponName, float _radiusImpulse, float _chargePower)
	{
		weaponPrefabName = _weaponName;
		radiusImpulse = _radiusImpulse;
		chargePower = _chargePower;
		if (Defs.isMulti && isMine)
		{
			if (!Defs.isInet)
			{
				if (chargePower == 1f)
				{
					GetComponent<NetworkView>().RPC("SetRocketActive", RPCMode.All, weaponPrefabName, radiusImpulse, base.transform.position);
				}
				else
				{
					GetComponent<NetworkView>().RPC("SetRocketActiveWithCharge", RPCMode.All, weaponPrefabName, radiusImpulse, base.transform.position, chargePower);
				}
			}
			else if (chargePower == 1f)
			{
				photonView.RPC("SetRocketActive", PhotonTargets.All, weaponPrefabName, radiusImpulse, base.transform.position);
			}
			else
			{
				photonView.RPC("SetRocketActiveWithCharge", PhotonTargets.All, weaponPrefabName, radiusImpulse, base.transform.position, chargePower);
			}
		}
		else if (!Defs.isMulti)
		{
			SetRocketActiveWithCharge(weaponPrefabName, radiusImpulse, base.transform.position, chargePower);
		}
	}

	[PunRPC]
	[RPC]
	public void SetRocketActive(string weapon, float _radiusImpulse, Vector3 pos)
	{
		SetRocketActiveWithCharge(weapon, _radiusImpulse, pos, 1f);
	}

	[RPC]
	[PunRPC]
	public void SetRocketActiveWithCharge(string _weaponName, float _radiusImpulse, Vector3 pos, float _chargePower)
	{
		if (Defs.IsDeveloperBuild && _weaponName == "WeaponGrenade")
		{
			_weaponName = "gadget_fraggrenade";
		}
		if (Defs.IsDeveloperBuild && _weaponName == "WeaponLike")
		{
			_weaponName = "Like";
		}
		bool flag = IsGrenadeWeaponName(_weaponName);
		string text = ((!flag) ? "Weapons" : "GadgetsContent");
		if (Application.isEditor)
		{
			Debug.Log(_weaponName);
		}
		currentWeaponSounds = (Resources.Load(text + "/" + _weaponName) as GameObject).GetComponent<WeaponSounds>();
		explosionName = currentWeaponSounds.bazookaExplosionName;
		impulseForce = currentWeaponSounds.impulseForce;
		rocketNum = currentWeaponSounds.rocketNum;
		myRigidbody.isKinematic = IsGrenadeWeaponName(_weaponName);
		if (isMine)
		{
			float num = 0f;
			if (!flag)
			{
				num = ((!(myPlayerMoveC != null)) ? 0f : myPlayerMoveC.koofDamageWeaponFromPotoins);
				int num2 = ((!(ExpController.Instance != null)) ? (ExpController.LevelsForTiers.Length - 1) : ExpController.Instance.OurTier);
				int num3 = ((!(myPlayerMoveC != null)) ? num2 : myPlayerMoveC.TierOrRoomTier(num2));
				damage = (float)currentWeaponSounds.survivalDamage * (1f + num + EffectsController.DamageModifsByCats(currentWeaponSounds.categoryNabor - 1));
				multiplayerDamage = ((ExpController.Instance != null && ExpController.Instance.OurTier < currentWeaponSounds.DamageByTier.Length) ? currentWeaponSounds.DamageByTier[num3] : ((currentWeaponSounds.DamageByTier.Length <= 0) ? 0f : currentWeaponSounds.DamageByTier[0]));
			}
			damageRange = currentWeaponSounds.damageRange * (1f + num + EffectsController.DamageModifsByCats(currentWeaponSounds.categoryNabor - 1));
			radiusDamage = currentWeaponSounds.bazookaExplosionRadius;
			radiusDamageSelf = currentWeaponSounds.bazookaExplosionRadiusSelf;
			isSlowdown = currentWeaponSounds.isSlowdown;
			slowdownCoeff = currentWeaponSounds.slowdownCoeff;
			slowdownTime = currentWeaponSounds.slowdownTime;
			impulseForce = currentWeaponSounds.impulseForce;
			impulseForceSelf = currentWeaponSounds.impulseForceSelf;
			if (currentWeaponSounds.isCharging)
			{
				damage *= chargePower;
				multiplayerDamage *= chargePower;
				damageRange *= chargePower;
			}
			myRigidbody.useGravity = currentWeaponSounds.grenadeLauncher;
		}
		else
		{
			base.transform.position = pos;
			chargePower = _chargePower;
			weaponPrefabName = _weaponName;
			radiusImpulse = _radiusImpulse;
		}
		StartCoroutine(StartRocketCoroutine());
	}

	private IEnumerator StartRocketCoroutine()
	{
		while (currentRocketSettings == null)
		{
			yield return null;
		}
		isKilled = false;
		if (isMine)
		{
			timerFromJumpLightning = maxTimerFromJumpLightning;
			counterJumpLightning = 0;
			lightningHitCount = 0;
			isDetectFirstTargetLightning = false;
			targetDamageLightning = null;
			targetAutoHoming = null;
			targetLightning = null;
			megabulletLastTarget = null;
			if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.FakeBonus)
			{
				currentRocketSettings.GetComponent<BoxCollider>().enabled = true;
			}
			GetComponent<BoxCollider>().size = currentRocketSettings.sizeBoxCollider;
			GetComponent<BoxCollider>().center = currentRocketSettings.centerBoxCollider;
			if (IsGravityRocket(currentRocketSettings))
			{
				myRigidbody.useGravity = true;
			}
		}
		if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.ChargeRocket)
		{
			currentRocketSettings.transform.localScale = Vector3.one * Mathf.Lerp(currentRocketSettings.chargeScaleMin, currentRocketSettings.chargeScaleMax, chargePower);
			GetComponent<BoxCollider>().size *= Mathf.Lerp(currentRocketSettings.chargeScaleMin, currentRocketSettings.chargeScaleMax, chargePower);
		}
		if (!IsGrenadeWeaponName(weaponPrefabName))
		{
			StartRocketRPC();
			yield break;
		}
		if (!Defs.isMulti || isMine)
		{
			Player_move_c.SetLayerRecursively(base.gameObject, 9);
		}
		while (myPlayerMoveC == null || myPlayerMoveC.myPlayerTransform.childCount == 0 || myPlayerMoveC.myCurrentWeaponSounds == null || myPlayerMoveC.myCurrentWeaponSounds.grenatePoint == null)
		{
			yield return null;
		}
		thisTransform.SetParent(myPlayerMoveC.myCurrentWeaponSounds.grenatePoint);
		thisTransform.localPosition = Vector3.zero;
		thisTransform.localRotation = Quaternion.identity;
		if (currentRocketSettings != null)
		{
			currentRocketSettings.transform.localPosition = Vector3.zero;
		}
	}

	[PunRPC]
	[RPC]
	public void StartRocketRPC()
	{
		if (IsGrenadeWeaponName(weaponPrefabName) && myPlayerMoveC != null && myPlayerMoveC.myCurrentWeaponSounds != null && myPlayerMoveC.myCurrentWeaponSounds.fakeGrenade != null && base.transform.parent != null)
		{
			base.transform.parent.gameObject.SetActive(true);
			myPlayerMoveC.myCurrentWeaponSounds.fakeGrenade.SetActive(false);
		}
		base.transform.parent = null;
		isRun = true;
		StartCoroutine(StartRocketRPCCoroutine());
	}

	private IEnumerator StartRocketRPCCoroutine()
	{
		while (currentRocketSettings == null)
		{
			yield return null;
		}
		if (currentRocketSettings.trail != null)
		{
			currentRocketSettings.trail.enabled = true;
		}
		if (currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.Firework && currentRocketSettings.flyParticle != null)
		{
			currentRocketSettings.flyParticle.SetActive(true);
		}
		if (!Defs.isMulti || isMine)
		{
			Tools.SetLayerRecursively(base.gameObject, LayerMask.NameToLayer("Rocket"));
			float _lifeTime = ((!Defs.isDaterRegim || (rocketNum != 36 && rocketNum != 18)) ? currentRocketSettings.lifeTime : 1f);
			Invoke("KillRocket", _lifeTime);
			if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Firework)
			{
				Invoke("FlyFirework", Mathf.Max(0f, _lifeTime - 2f));
			}
		}
		if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Drone)
		{
			Invoke("ActivateDrone", 1f);
			if (currentRocketSettings.GetComponent<Animation>() != null)
			{
				currentRocketSettings.GetComponent<Animation>().Play("throw");
			}
		}
		if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.FakeBonus)
		{
			Invoke("SetFakeBonusRun", 2.5f);
		}
		if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.HomingGrenade)
		{
			if (currentRocketSettings.GetComponent<Animation>() != null)
			{
				currentRocketSettings.GetComponent<Animation>().Play("Fly");
			}
			myRigidbody.useGravity = false;
		}
	}

	private void ActivateDrone()
	{
		if (currentRocketSettings != null && currentRocketSettings.droneRotator != null)
		{
			currentRocketSettings.droneRotator.enabled = true;
		}
		if (currentRocketSettings != null && currentRocketSettings.droneParticle != null)
		{
			currentRocketSettings.droneParticle.SetActive(true);
		}
		isActivated = true;
		if (!Defs.isMulti || isMine)
		{
			myRigidbody.useGravity = false;
			myRigidbody.velocity += Vector3.up;
			dronePoint = base.transform.position;
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position, Vector3.down, out hitInfo, 10000f, Player_move_c._ShootRaycastLayerMask) && hitInfo.distance < 4.5f)
			{
				dronePoint = hitInfo.point + Vector3.up * 2.5f;
			}
		}
	}

	public IEnumerator SetCurrentRocket(int _rocketNum)
	{
		if (rocketsDict.ContainsKey(_rocketNum.ToString()))
		{
			currentRocketSettings = rocketsDict[_rocketNum.ToString()];
			currentRocketSettings.gameObject.SetActive(true);
			yield break;
		}
		ResourceRequest request = Resources.LoadAsync<GameObject>("Rockets/Rocket_" + _rocketNum);
		yield return request;
		if (!request.isDone)
		{
			yield break;
		}
		if (rocketsDict.ContainsKey(_rocketNum.ToString()))
		{
			currentRocketSettings = rocketsDict[_rocketNum.ToString()];
			currentRocketSettings.gameObject.SetActive(true);
			yield break;
		}
		GameObject _currentRocketPrefab = request.asset as GameObject;
		GameObject _currentRocket = UnityEngine.Object.Instantiate(_currentRocketPrefab);
		_currentRocket.transform.SetParent(base.transform);
		_currentRocket.transform.localPosition = Vector3.zero;
		_currentRocket.transform.localRotation = Quaternion.Euler(Vector3.zero);
		_currentRocket.transform.localScale = new Vector3(1f, 1f, 1f);
		currentRocketSettings = _currentRocket.GetComponent<RocketSettings>();
		rocketsDict.Add(_rocketNum.ToString(), currentRocketSettings);
		if (!isMine && _currentRocket.GetComponent<BoxCollider>() != null)
		{
			_currentRocket.GetComponent<BoxCollider>().enabled = false;
		}
	}

	public void RunGrenade()
	{
		if (Defs.isMulti && isMine)
		{
			if (!Defs.isInet)
			{
				GetComponent<NetworkView>().RPC("StartRocketRPC", RPCMode.All);
			}
			else
			{
				photonView.RPC("StartRocketRPC", PhotonTargets.All);
			}
		}
		else if (!Defs.isMulti)
		{
			StartRocketRPC();
		}
		myRigidbody.isKinematic = false;
	}

	private void FlyFirework()
	{
		if (currentRocketSettings.flyParticle != null)
		{
			currentRocketSettings.flyParticle.SetActive(true);
		}
		myRigidbody.isKinematic = false;
		myRigidbody.AddForce(Vector3.up * 200f);
	}

	public void SendSetRocketSticked()
	{
		if (Defs.isMulti && isMine)
		{
			if (!Defs.isInet)
			{
				GetComponent<NetworkView>().RPC("SetRocketStickedRPC", RPCMode.Others, thisTransform.position);
			}
			else
			{
				photonView.RPC("SetRocketStickedRPC", PhotonTargets.Others, thisTransform.position);
			}
		}
	}

	[RPC]
	[PunRPC]
	public void SetRocketStickedRPC(Vector3 position)
	{
		base.transform.position = position;
		SetRocketSticked();
	}

	public void SetRocketStickedToPlayer(Player_move_c player)
	{
		CancelInvoke("KillRocket");
		CancelInvoke("FlyFirework");
		Invoke("KillRocket", 2f);
		if (currentRocketSettings.flyParticle != null)
		{
			currentRocketSettings.flyParticle.SetActive(true);
		}
		myRigidbody.isKinematic = true;
		isStickedToPlayer = true;
		base.transform.parent = player.myPlayerTransform;
		SetRocketSticked();
		if (Defs.isMulti && isMine)
		{
			if (!Defs.isInet)
			{
				GetComponent<NetworkView>().RPC("SetRocketStickedToPlayerRPC", RPCMode.Others, player.skinNamePixelView.viewID, thisTransform.localPosition);
			}
			else
			{
				photonView.RPC("SetRocketStickedToPlayerRPC", PhotonTargets.Others, player.skinNamePixelView.viewID, thisTransform.localPosition);
			}
		}
	}

	[PunRPC]
	[RPC]
	public void SetRocketStickedToPlayerRPC(int pixelID, Vector3 relativePosition)
	{
		Player_move_c player_move_c = null;
		isRun = false;
		GetComponent<BoxCollider>().enabled = false;
		for (int i = 0; i < Initializer.players.Count; i++)
		{
			if (Initializer.players[i].skinNamePixelView.viewID == pixelID)
			{
				player_move_c = Initializer.players[i];
				break;
			}
		}
		if (!(player_move_c == null))
		{
			if (player_move_c.Equals(WeaponManager.sharedManager.myPlayerMoveC))
			{
				WeaponManager.sharedManager.myPlayerMoveC.PlayerEffectRPC(12, 1f);
			}
			base.transform.parent = player_move_c.myPlayerTransform;
			base.transform.localPosition = relativePosition;
			SetRocketSticked();
		}
	}

	private void SetRocketSticked()
	{
		if (!(currentRocketSettings == null))
		{
			if (currentRocketSettings.flyParticle != null)
			{
				currentRocketSettings.flyParticle.SetActive(false);
			}
			if (currentRocketSettings.stickedParticle != null)
			{
				currentRocketSettings.stickedParticle.SetActive(true);
			}
			if (currentRocketSettings.GetComponent<Animation>() != null)
			{
				currentRocketSettings.GetComponent<Animation>().Play("stick");
			}
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (IsSkipCollision(other.gameObject))
		{
			return;
		}
		if ((currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Firework || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.StickyMine || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.StickyBomb || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.ToxicBomb) && !IsDamageableObject(other.transform.root.gameObject))
		{
			if ((currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.Firework || !(Vector3.Angle(other.contacts[0].normal, Vector3.up) > 30f)) && !isActivated)
			{
				isActivated = true;
				base.transform.position = other.contacts[0].point + other.contacts[0].normal * 0.035f;
				if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.StickyMine)
				{
					base.transform.up = other.contacts[0].normal;
				}
				if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Firework)
				{
					base.transform.up = Vector3.back;
				}
				myRigidbody.isKinematic = true;
				stickedObject = other.transform;
				stickedObjectPos = stickedObject.position;
				SetRocketSticked();
				SendSetRocketSticked();
			}
		}
		else if (currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.FakeBonus)
		{
			CollisionWithCollide(other.collider);
		}
	}

	private void SetFakeBonusRun()
	{
		if (!isRun)
		{
			return;
		}
		if (!Defs.isMulti || isMine)
		{
			myRigidbody.isKinematic = true;
			if (currentRocketSettings != null)
			{
				currentRocketSettings.GetComponent<BoxCollider>().enabled = false;
			}
		}
		isActivated = true;
		if (currentRocketSettings != null && currentRocketSettings.droneRotator != null)
		{
			currentRocketSettings.droneRotator.enabled = true;
		}
	}

	public void OnMegabulletTriggerEnter(Collider other)
	{
		if (!other.gameObject.transform.root.Equals(megabulletLastTarget) && !IsSkipCollision(other.gameObject) && (!(other.gameObject.transform.parent != null) || !other.gameObject.transform.parent.gameObject.CompareTag("Untagged")) && (!(other.gameObject.transform.parent == null) || !other.gameObject.CompareTag("Untagged")) && IsDamageableObject(other.gameObject.transform.root.gameObject))
		{
			megabulletLastTarget = other.gameObject.transform.root;
			StartCoroutine(Hit(other));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (IsSkipCollision(other.gameObject) || ((currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Firework || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.FakeBonus || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.StickyMine || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.StickyBomb || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.ToxicBomb) && !IsDamageableObject(other.gameObject.transform.root.gameObject)))
		{
			return;
		}
		if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Firework && !isStickedToPlayer)
		{
			IDamageable component = other.transform.root.gameObject.GetComponent<IDamageable>();
			if (component != null && component.IsEnemyTo(WeaponManager.sharedManager.myPlayerMoveC) && component is PlayerDamageable)
			{
				SetRocketStickedToPlayer((component as PlayerDamageable).myPlayer);
			}
		}
		if (!isStickedToPlayer)
		{
			CollisionWithCollide(other);
		}
	}

	private bool IsDamageableObject(GameObject go)
	{
		return go.GetComponent<IDamageable>() != null;
	}

	private bool IsSkipCollision(GameObject go)
	{
		if (!isRun)
		{
			return true;
		}
		if (Defs.isMulti && !isMine)
		{
			return true;
		}
		if (go.name.Equals("DamageCollider") || go.name.Equals("StopCollider"))
		{
			return true;
		}
		if (go.CompareTag("Area") || go.CompareTag("CapturePoint"))
		{
			return true;
		}
		Transform root = go.transform.root;
		if (WeaponManager.sharedManager.myPlayer != null && (root.Equals(WeaponManager.sharedManager.myPlayer.transform) || (WeaponManager.sharedManager.myPlayerMoveC.myPetEngine != null && root.Equals(WeaponManager.sharedManager.myPlayerMoveC.myPetEngine.ThisTransform))))
		{
			return true;
		}
		if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Drone || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.GrenadeBouncing || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.SingularityGrenade || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.NuclearGrenade || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.SlowdownGrenade)
		{
			return true;
		}
		if ((currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Bomb || currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Ball) && !IsDamageableObject(root.gameObject))
		{
			return true;
		}
		return false;
	}

	private void CollisionWithCollide(Collider _collide)
	{
		GameObject gameObject = _collide.gameObject;
		if (currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.Ghost && ((currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.MegaBullet && currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.Lightning) || gameObject.transform.root.CompareTag("Untagged")))
		{
			if ((currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.StickyBomb && currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.StickyMine && currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.ToxicBomb && currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.FakeBonus) || !myRigidbody.isKinematic)
			{
				StartCoroutine(KillRocket(_collide));
			}
		}
		else if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Lightning)
		{
			if (Initializer.IsEnemyTarget(gameObject.transform.root))
			{
				targetDamageLightning = gameObject.transform.root;
				timerFromJumpLightning = maxTimerFromJumpLightning;
				counterJumpLightning++;
				if (gameObject.CompareTag("Turret") && gameObject.GetComponent<TurretController>().turretType == TurretController.TurretType.ShieldWall)
				{
					counterJumpLightning = currentRocketSettings.countJumpLightning + 1;
				}
				if (counterJumpLightning > currentRocketSettings.countJumpLightning)
				{
					KillRocket();
				}
				else
				{
					StartCoroutine(Hit(null));
				}
			}
		}
		else if (currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.MegaBullet)
		{
			StartCoroutine(Hit(_collide));
		}
	}

	[Obfuscation(Exclude = true)]
	public void KillRocket()
	{
		StartCoroutine(KillRocket(null));
	}

	public IEnumerator KillRocket(Collider _hitCollision)
	{
		if (isKilled)
		{
			yield break;
		}
		isKilled = true;
		if (OnExplode != null)
		{
			OnExplode();
			OnExplode = null;
		}
		yield return StartCoroutine(Hit(_hitCollision));
		if (Defs.isMulti)
		{
			if (!Defs.isInet)
			{
				GetComponent<NetworkView>().RPC("Collide", RPCMode.Others, explosionName, thisTransform.position);
			}
			else
			{
				photonView.RPC("Collide", PhotonTargets.Others, explosionName, thisTransform.position);
			}
		}
		Collide(explosionName, thisTransform.position);
	}

	[RPC]
	[PunRPC]
	private void Collide(string _explosionName, Vector3 _pos)
	{
		base.transform.parent = null;
		thisTransform.position = _pos;
		if (Defs.inComingMessagesCounter <= 5 && currentRocketSettings != null && currentRocketSettings.typeFly != RocketSettings.TypeFlyRocket.Lightning)
		{
			BazookaExplosion(_explosionName);
		}
		DestroyRocket();
	}

	public void BazookaExplosion(string explosionName)
	{
		ShowExplosion(explosionName);
		GameObject myPlayer = WeaponManager.sharedManager.myPlayer;
		if (myPlayer == null || (WeaponManager.sharedManager.myPlayerMoveC.isImmortality && !isMine))
		{
			return;
		}
		Vector3 position = thisTransform.position;
		Vector3 dir = myPlayer.transform.position - position;
		float sqrMagnitude = dir.sqrMagnitude;
		float num = radiusImpulse * radiusImpulse;
		if (sqrMagnitude < num)
		{
			ImpactReceiver impactReceiver = myPlayer.GetComponent<ImpactReceiver>();
			if (impactReceiver == null)
			{
				impactReceiver = myPlayer.AddComponent<ImpactReceiver>();
			}
			float num2 = 100f;
			if (radiusImpulse != 0f)
			{
				num2 = Mathf.Sqrt(sqrMagnitude / num);
			}
			float num3 = Mathf.Max(0f, 1f - num2);
			float num4 = ((Defs.isMulti && !isMine) ? impulseForce : impulseForceSelf);
			num4 *= num3;
			impactReceiver.AddImpact(dir, num4);
			if ((!Defs.isMulti || isMine) && num3 > 0.01f)
			{
				WeaponManager.sharedManager.myPlayerMoveC.isRocketJump = true;
			}
		}
	}

	private void SendShowExplosion()
	{
		if (Defs.isMulti && isMine)
		{
			if (!Defs.isInet)
			{
				GetComponent<NetworkView>().RPC("ShowExplosion", RPCMode.All, explosionName);
			}
			else
			{
				photonView.RPC("ShowExplosion", PhotonTargets.All, explosionName);
			}
		}
		else if (!Defs.isMulti)
		{
			ShowExplosion(explosionName);
		}
	}

	[PunRPC]
	[RPC]
	private void ShowExplosion(string explosionName)
	{
		if (currentRocketSettings == null)
		{
			return;
		}
		if (Defs.IsDeveloperBuild && explosionName == "WeaponLike")
		{
			explosionName = "Like";
		}
		Vector3 position = thisTransform.position;
		string text = ResPath.Combine("Explosions", explosionName);
		GameObject objectFromName = RayAndExplosionsStackController.sharedController.GetObjectFromName(text);
		if (!(objectFromName != null))
		{
			return;
		}
		objectFromName.transform.position = position;
		bool flag = !Defs.isMulti || ((!Defs.isInet) ? GetComponent<NetworkView>().isMine : photonView.isMine);
		if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.SingularityGrenade)
		{
			objectFromName.GetComponent<SingularityHole>().owner = myPlayerMoveC;
		}
		if (flag)
		{
			if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.NuclearGrenade)
			{
				objectFromName.GetComponent<DamageInRadiusEffect>().ActivateEffect(damage * currentRocketSettings.toxicDamageMultiplier, multiplayerDamage * currentRocketSettings.toxicDamageMultiplier, currentRocketSettings.raduisDetectTarget, currentRocketSettings.toxicHitTime, weaponPrefabName, currentRocketSettings.typeDead, currentRocketSettings.typeKilsIconChat);
			}
			if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Molotov)
			{
				objectFromName.GetComponent<DamageInRadiusEffect>().ActivatePoisonEffect(damage, multiplayerDamage, currentRocketSettings.raduisDetectTarget, currentRocketSettings.toxicHitTime, currentWeaponSounds, Player_move_c.PoisonType.Burn);
			}
			if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.SlowdownGrenade)
			{
				objectFromName.GetComponent<DamageInRadiusEffect>().ActivateSlowdonEffect(currentWeaponSounds);
			}
		}
	}

	[Obfuscation(Exclude = true)]
	private void DestroyRocket()
	{
		if (!Defs.isMulti || isMine)
		{
			CancelInvoke("KillRocket");
			if (currentRocketSettings != null && currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.Firework)
			{
				CancelInvoke("FlyFirework");
			}
			RocketStack.sharedController.ReturnRocket(base.gameObject);
		}
		SetRocketDeactive();
	}

	[RPC]
	[PunRPC]
	public void SetRocketDeactive()
	{
		isRun = false;
		isKilled = false;
		isActivated = false;
		isStickedToPlayer = false;
		thisTransform.position = Vector3.down * 10000f;
		if (currentRocketSettings != null)
		{
			currentRocketSettings.gameObject.SetActive(false);
			if (currentRocketSettings.trail != null)
			{
				currentRocketSettings.trail.enabled = false;
			}
			if (currentRocketSettings.flyParticle != null)
			{
				currentRocketSettings.flyParticle.SetActive(false);
			}
			if (currentRocketSettings.stickedParticle != null)
			{
				currentRocketSettings.stickedParticle.SetActive(false);
			}
			if (currentRocketSettings.droneRotator != null)
			{
				currentRocketSettings.droneRotator.enabled = false;
				if (currentRocketSettings.typeFly == RocketSettings.TypeFlyRocket.FakeBonus)
				{
					currentRocketSettings.droneRotator.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
				}
			}
			if (currentRocketSettings.droneParticle != null)
			{
				currentRocketSettings.droneParticle.SetActive(false);
			}
		}
		currentRocketSettings = null;
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (isMine && !isKilled)
		{
			if (chargePower == 1f)
			{
				photonView.RPC("SetRocketActive", player, weaponPrefabName, radiusImpulse, base.transform.position);
			}
			else
			{
				photonView.RPC("SetRocketActiveWithCharge", player, weaponPrefabName, radiusImpulse, base.transform.position, chargePower);
			}
			if (isRun && IsGrenadeWeaponName(weaponPrefabName))
			{
				photonView.RPC("StartRocketRPC", player);
			}
		}
	}

	private void OnPlayerConnected(NetworkPlayer player)
	{
		if (isMine && !isKilled)
		{
			if (chargePower == 1f)
			{
				GetComponent<NetworkView>().RPC("SetRocketActive", player, weaponPrefabName, radiusImpulse, base.transform.position);
			}
			else
			{
				GetComponent<NetworkView>().RPC("SetRocketActiveWithCharge", player, weaponPrefabName, radiusImpulse, base.transform.position, chargePower);
			}
			if (isRun && IsGrenadeWeaponName(weaponPrefabName))
			{
				GetComponent<NetworkView>().RPC("StartRocketRPC", player);
			}
		}
	}
}

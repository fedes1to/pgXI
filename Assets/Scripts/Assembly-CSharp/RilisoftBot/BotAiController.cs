using System;
using System.Collections;
using Rilisoft;
using UnityEngine;
using UnityEngine.AI;

namespace RilisoftBot
{
	public class BotAiController : MonoBehaviour
	{
		private enum AiState
		{
			Patrol,
			MoveToTarget,
			Damage,
			Waiting,
			Teleportation,
			None
		}

		private enum TypeBot
		{
			Melee,
			Shooting,
			ShootAndMelee,
			None
		}

		private enum TargetType
		{
			Player,
			Turret,
			Bot,
			None,
			Pet
		}

		private const int MaxAttempTeleportation = 5;

		private const float TimeDelayedTeleport = 0.2f;

		private const float TimeOutUpdateMultiplayerTargets = 3f;

		private const float TimeOutUpdateLocalTargets = 1f;

		private BaseBot _botController;

		private TypeBot _typeBot;

		private AiState _currentState;

		private bool _isMultiplayerCoopMode;

		private PhotonView _photonView;

		private bool _isDeaded;

		[Header("Patrol module settings")]
		public float minLenghtMove = 9f;

		private bool _isCanMove;

		private float _lastTimeMoving;

		private Vector3 _targetPoint;

		private NavMeshAgent _naveMeshAgent;

		[Header("Movement module settings")]
		public bool isStationary;

		public bool isTeleportationMove;

		public static bool deathAudioPlaying;

		private float _timeToTakeDamage;

		private bool _isFalling;

		private BoxCollider _modelCollider;

		private float _timeToCheckAvailabelShot;

		private bool _isTargetAvalabelShot;

		[Header("Teleport movement setting")]
		public float timeToNextTeleport = 2f;

		public float[] DeltaTeleportAttackDistance = new float[2] { 1f, 2f };

		public GameObject effectTeleport;

		public float angleByPlayerLook = 30f;

		public AudioClip teleportStart;

		public AudioClip teleportEnd;

		private float _timeLastTeleport;

		private GameObject _effectObject;

		private bool _isWaiting;

		[NonSerialized]
		public bool isDetectPlayer = true;

		private bool _isEntered;

		private float _timeToUpdateMultiplayerTargets = 3f;

		private float _timeToUpdateLocalTargets = 1f;

		private bool _isTargetCaptureForce;

		public bool IsCanMove
		{
			get
			{
				return _isCanMove;
			}
			set
			{
				if (_isCanMove != value)
				{
					_isCanMove = value;
					if (_isCanMove)
					{
						_lastTimeMoving = -1f;
						_botController.PlayAnimZombieWalkByMode();
					}
				}
			}
		}

		public Transform currentTarget { get; private set; }

		private bool IsWaitingState
		{
			get
			{
				return _isWaiting;
			}
			set
			{
				if (_isWaiting != value)
				{
					_isWaiting = value;
					if (_isWaiting)
					{
						_botController.PlayAnimationIdle();
					}
				}
			}
		}

		private void Start()
		{
			_photonView = GetComponent<PhotonView>();
			_isMultiplayerCoopMode = Defs.isCOOP && _photonView != null;
			_currentState = AiState.None;
			_botController = GetComponent<BaseBot>();
			_typeBot = GetCurrentTypeBot();
			_naveMeshAgent = GetComponent<NavMeshAgent>();
			_modelCollider = GetComponentInChildren<BoxCollider>();
			InitializePatrolModule();
			if (_typeBot == TypeBot.Melee)
			{
				_timeToTakeDamage = GetTimeToTakeDamageMeleeBot();
			}
			_timeLastTeleport = timeToNextTeleport;
			InitTeleportData();
			_naveMeshAgent.Warp(base.transform.position + _naveMeshAgent.baseOffset * Vector3.up);
		}

		private TypeBot GetCurrentTypeBot()
		{
			if (_botController == null)
			{
				return TypeBot.None;
			}
			if (_botController as MeleeBot != null || _botController as MeleeBossBot != null)
			{
				return TypeBot.Melee;
			}
			if (_botController as MeleeShootBot != null)
			{
				return TypeBot.ShootAndMelee;
			}
			return TypeBot.Shooting;
		}

		private void UpdateCurrentAiState()
		{
			if (IsCanMove)
			{
				_currentState = AiState.Patrol;
			}
			else if (!_botController.IsDeath && currentTarget != null)
			{
				_currentState = CheckActiveAttackState();
			}
			else
			{
				_currentState = AiState.None;
			}
		}

		private bool CheckApplyMultiplayerLogic()
		{
			if (!_isMultiplayerCoopMode)
			{
				return false;
			}
			if (ZombiManager.sharedManager == null)
			{
				return true;
			}
			if (!ZombiManager.sharedManager.startGame)
			{
				if (PhotonNetwork.isMasterClient)
				{
					_botController.DestroyByNetworkType();
				}
				return true;
			}
			if (!_photonView.isMine)
			{
				return true;
			}
			return false;
		}

		private void Update()
		{
			if (Defs.isMulti && _naveMeshAgent.enabled != PhotonNetwork.isMasterClient)
			{
				_naveMeshAgent.enabled = PhotonNetwork.isMasterClient;
			}
			if (CheckApplyMultiplayerLogic())
			{
				return;
			}
			UpdateTargetsForBot();
			UpdateCurrentAiState();
			if (_currentState == AiState.Patrol)
			{
				UpdatePatrolState();
			}
			else if (_currentState == AiState.MoveToTarget)
			{
				UpdateMoveToTargetState();
			}
			else if (_currentState == AiState.Damage)
			{
				UpdateDamagedTargetState();
			}
			else if (_currentState == AiState.Waiting)
			{
				IsWaitingState = true;
			}
			else if (_currentState == AiState.Teleportation)
			{
				StartCoroutine(TeleportFromRandomPoint());
			}
			if (_botController.IsDeath)
			{
				if (_botController.IsFalling)
				{
					_botController.SetPositionForFallState();
				}
				if (!_isDeaded)
				{
					_naveMeshAgent.enabled = false;
					_isDeaded = true;
				}
			}
			for (int i = 0; i < Initializer.singularities.Count; i++)
			{
				SingularityHole singularityHole = Initializer.singularities[i];
				Vector3 vector = singularityHole.transform.position - base.transform.position;
				float force = singularityHole.GetForce(vector.sqrMagnitude);
				if (force != 0f)
				{
					base.transform.position += vector.normalized * force * Time.deltaTime;
				}
			}
		}

		private void InitializePatrolModule()
		{
			_lastTimeMoving = -1f;
			if (_isMultiplayerCoopMode && !_photonView.isMine)
			{
				_botController.PlayAnimZombieWalkByMode();
				IsCanMove = false;
			}
			else
			{
				IsCanMove = !isStationary;
			}
		}

		private void UpdatePatrolState()
		{
			if (IsCanMove && _lastTimeMoving <= Time.time)
			{
				ResetNavigationPathIfNeed();
				Vector3 position = base.transform.position;
				_targetPoint = new Vector3(position.x + UnityEngine.Random.Range(0f - minLenghtMove, minLenghtMove), position.y, position.z + UnityEngine.Random.Range(0f - minLenghtMove, minLenghtMove));
				_lastTimeMoving = Time.time + Vector3.Distance(base.transform.position, _targetPoint) / _botController.GetWalkSpeed();
				if (!_naveMeshAgent.SetDestination(_targetPoint))
				{
					_lastTimeMoving = 0f;
					return;
				}
				_botController.OrientToTarget(_targetPoint);
				_naveMeshAgent.speed = _botController.GetWalkSpeed();
			}
		}

		private float GetTimeToTakeDamageMeleeBot()
		{
			if (_typeBot != 0)
			{
				return 0f;
			}
			MeleeBot meleeBot = _botController as MeleeBot;
			return meleeBot.CheckTimeToTakeDamage();
		}

		private void CheckTargetAvailabelForShot()
		{
			_timeToCheckAvailabelShot -= Time.deltaTime;
			if (!(_timeToCheckAvailabelShot > 0f))
			{
				_timeToCheckAvailabelShot = 1f;
				_isTargetAvalabelShot = IsTargetAvailabelForShot();
			}
		}

		private string GetTargetTagAndPointToShot(out Vector3 pointToShot)
		{
			switch (GetTargetType(currentTarget))
			{
			case TargetType.Player:
			{
				SkinName component = currentTarget.GetComponent<SkinName>();
				if (component == null)
				{
					pointToShot = Vector3.zero;
					return null;
				}
				Transform transform = component.headObj.transform;
				pointToShot = transform.position;
				return "Player";
			}
			case TargetType.Turret:
				pointToShot = currentTarget.GetComponent<TurretController>().GetHeadPoint();
				return "Turret";
			case TargetType.Bot:
				pointToShot = currentTarget.GetComponent<BaseBot>().GetHeadPoint();
				return "Enemy";
			case TargetType.Pet:
				pointToShot = currentTarget.position;
				return "Pet";
			default:
				pointToShot = Vector3.zero;
				return null;
			}
		}

		private bool IsTargetAvailabelForShot()
		{
			Vector3 headPoint = _botController.GetHeadPoint();
			Vector3 pointToShot;
			string targetTagAndPointToShot = GetTargetTagAndPointToShot(out pointToShot);
			float maxAttackDistance = _botController.GetMaxAttackDistance();
			RaycastHit hitInfo;
			if (Physics.Raycast(headPoint, pointToShot - headPoint, out hitInfo, maxAttackDistance, Tools.AllAvailabelBotRaycastMask))
			{
				Transform transform = hitInfo.collider.transform;
				return transform.root.CompareTag(targetTagAndPointToShot);
			}
			return false;
		}

		private void UpdateMoveToTargetState()
		{
			IsWaitingState = false;
			if (_botController.isFlyingSpeedLimit && _naveMeshAgent.isOnOffMeshLink)
			{
				_naveMeshAgent.speed = _botController.maxFlyingSpeed;
			}
			else
			{
				_naveMeshAgent.speed = _botController.GetAttackSpeedByCompleteLevel();
			}
			_naveMeshAgent.SetDestination(currentTarget.position);
			if (_typeBot == TypeBot.Melee)
			{
				_timeToTakeDamage = GetTimeToTakeDamageMeleeBot();
			}
			_botController.PlayAnimZombieWalkByMode();
		}

		private AiState CheckActiveAttackState()
		{
			if (_botController.IsDeath || currentTarget == null)
			{
				return AiState.None;
			}
			if (CheckMoveFromTeleport())
			{
				return AiState.Teleportation;
			}
			float distanceToEnemy = Vector3.SqrMagnitude(currentTarget.position - (base.transform.position + Vector3.up));
			if (_botController.CheckEnemyInAttackZone(distanceToEnemy))
			{
				if (_typeBot == TypeBot.Shooting || _typeBot == TypeBot.ShootAndMelee)
				{
					CheckTargetAvailabelForShot();
					AiState result = ((!isStationary) ? AiState.MoveToTarget : AiState.Waiting);
					if (_isTargetAvalabelShot)
					{
						return AiState.Damage;
					}
					return result;
				}
				return AiState.Damage;
			}
			return (!isStationary) ? AiState.MoveToTarget : AiState.Waiting;
		}

		private void InitTeleportData()
		{
			if (isTeleportationMove)
			{
				_effectObject = UnityEngine.Object.Instantiate(effectTeleport);
				_effectObject.transform.parent = base.transform;
				_effectObject.transform.localPosition = Vector3.zero;
				_effectObject.transform.rotation = Quaternion.identity;
				_effectObject.SetActive(false);
			}
		}

		private IEnumerator ShowEffectTeleport(float seconds)
		{
			_effectObject.SetActive(true);
			yield return new WaitForSeconds(seconds);
			_effectObject.SetActive(false);
		}

		private IEnumerator TeleportFromRandomPoint()
		{
			bool isWarpComplete = false;
			Vector3 positionFromTeleport2 = Vector3.zero;
			isStationary = true;
			StartCoroutine(ShowEffectTeleport(0.2f));
			_botController.TryPlayAudioClip(teleportStart);
			yield return new WaitForSeconds(0.2f);
			for (int i = 0; i < 5; i++)
			{
				positionFromTeleport2 = GetPositionFromTeleport();
				isWarpComplete = _naveMeshAgent.Warp(positionFromTeleport2);
				if (isWarpComplete)
				{
					break;
				}
			}
			if (!isWarpComplete)
			{
				_naveMeshAgent.Warp(Vector3.zero);
			}
			_botController.DelayShootAfterEvent(4f);
			StartCoroutine(ShowEffectTeleport(0.2f));
			_botController.TryPlayAudioClip(teleportEnd);
			yield return new WaitForSeconds(0.2f);
			isStationary = false;
		}

		private Vector3 GetPositionFromTeleport()
		{
			Vector3 zero = Vector3.zero;
			float min = _botController.attackDistance + DeltaTeleportAttackDistance[0];
			float max = _botController.attackDistance + DeltaTeleportAttackDistance[1];
			float num = UnityEngine.Random.Range(min, max);
			float value = UnityEngine.Random.value;
			if (value >= 0f && value < 0.4f)
			{
				Quaternion quaternion = Quaternion.Euler(0f, angleByPlayerLook, 0f);
				return currentTarget.position + quaternion * (currentTarget.forward * num);
			}
			if (value >= 0.4f && value < 0.5f)
			{
				Quaternion quaternion2 = Quaternion.Euler(0f, angleByPlayerLook, 0f);
				Vector3 forward = currentTarget.forward;
				forward.z = 0f - forward.z;
				return currentTarget.position + quaternion2 * (forward * num);
			}
			if (value >= 0.5f && value < 0.6f)
			{
				Vector3 forward2 = currentTarget.forward;
				forward2.z = 0f - forward2.z;
				Quaternion quaternion3 = Quaternion.Euler(0f, 0f - angleByPlayerLook, 0f);
				return currentTarget.position + quaternion3 * (forward2 * num);
			}
			Quaternion quaternion4 = Quaternion.Euler(0f, 0f - angleByPlayerLook, 0f);
			return currentTarget.position + quaternion4 * (currentTarget.forward * num);
		}

		private bool CheckMoveFromTeleport()
		{
			if (!isTeleportationMove)
			{
				return false;
			}
			if (_timeLastTeleport > 0f)
			{
				_timeLastTeleport -= Time.deltaTime;
				return false;
			}
			_timeLastTeleport = timeToNextTeleport;
			return true;
		}

		public void SetTargetToMove(Transform target)
		{
			if (isDetectPlayer)
			{
				if (target != null && currentTarget != target)
				{
					ResetNavigationPathIfNeed();
					_botController.PlayAnimZombieWalkByMode();
				}
				else if (target == null && currentTarget != target)
				{
					ResetNavigationPathIfNeed();
					_botController.PlayAnimationWalk();
				}
				currentTarget = target;
				IsCanMove = target == null && !isStationary;
			}
		}

		private void ResetNavigationPathIfNeed()
		{
			if (_naveMeshAgent.path != null && !_naveMeshAgent.isOnOffMeshLink)
			{
				_naveMeshAgent.ResetPath();
			}
		}

		private void UpdateDamagedTargetState()
		{
			IsWaitingState = false;
			ResetNavigationPathIfNeed();
			Vector3 position = currentTarget.position;
			position.y = base.transform.position.y;
			_botController.OrientToTarget(position);
			_botController.PlayAnimZombieAttackOrStopByMode();
			if (_typeBot == TypeBot.Melee)
			{
				_timeToTakeDamage -= Time.deltaTime;
				if (_timeToTakeDamage <= 0f)
				{
					_botController.MakeDamage(currentTarget);
					_timeToTakeDamage = GetTimeToTakeDamageMeleeBot();
				}
			}
		}

		private TargetType GetTargetType(Transform target)
		{
			if (target.CompareTag("Player"))
			{
				return TargetType.Player;
			}
			if (target.CompareTag("Turret"))
			{
				return TargetType.Turret;
			}
			if (target.CompareTag("Enemy"))
			{
				return TargetType.Bot;
			}
			if (target.CompareTag("Pet"))
			{
				return TargetType.Pet;
			}
			return TargetType.None;
		}

		private void UpdateTargetsForBot()
		{
			if (!isDetectPlayer)
			{
				return;
			}
			if (_isMultiplayerCoopMode)
			{
				_timeToUpdateMultiplayerTargets -= Time.deltaTime;
				if (_timeToUpdateMultiplayerTargets <= 0f)
				{
					_timeToUpdateMultiplayerTargets = 3f;
					CheckTargetForMultiplayerMode();
				}
			}
			else
			{
				_timeToUpdateLocalTargets -= Time.deltaTime;
				if (_timeToUpdateLocalTargets <= 0f)
				{
					CheckTargetForLocalMode();
				}
			}
		}

		private bool CheckForcedTarget()
		{
			if (!_isTargetCaptureForce)
			{
				return false;
			}
			if (IsCurrentTargetLost())
			{
				SetTargetToMove(null);
				_isTargetCaptureForce = false;
			}
			return true;
		}

		private void CheckTargetForMultiplayerMode()
		{
			if (CheckForcedTarget())
			{
				return;
			}
			float num = -1f;
			bool isTargetPlayer;
			GameObject nearestTargetForMultiplayer = GetNearestTargetForMultiplayer(out isTargetPlayer);
			if (nearestTargetForMultiplayer == null)
			{
				SetTargetToMove(null);
				return;
			}
			num = ((!isTargetPlayer) ? GetDistanceToTurret(nearestTargetForMultiplayer) : GetDistanceToPlayer(nearestTargetForMultiplayer));
			if (num != -1f && _botController.detectRadius >= num && !IsTargetLost(nearestTargetForMultiplayer.transform))
			{
				SetTargetToMove(nearestTargetForMultiplayer.transform);
			}
			else
			{
				SetTargetToMove(null);
			}
		}

		private void CheckTargetForLocalMode()
		{
			if (CheckForcedTarget())
			{
				return;
			}
			if (!_isEntered)
			{
				GameObject gameObject = ((Initializer.turretsObj != null && Initializer.turretsObj.Count != 0) ? Initializer.turretsObj[0] : null);
				float distanceToTurret = GetDistanceToTurret(gameObject);
				bool flag = distanceToTurret != -1f && _botController.detectRadius >= distanceToTurret;
				GameObject gameObject2 = GameObject.FindGameObjectWithTag("Player");
				float distanceToPlayer = GetDistanceToPlayer(gameObject2);
				bool flag2 = distanceToPlayer != -1f && _botController.detectRadius >= distanceToPlayer;
				GameObject gameObject3 = GameObject.FindGameObjectWithTag("Pet");
				float distanceToTurret2 = GetDistanceToTurret(gameObject3);
				bool flag3 = distanceToTurret2 != -1f && _botController.detectRadius >= distanceToTurret2;
				Transform transform = null;
				if (flag2 && flag)
				{
					transform = ((!(distanceToPlayer < distanceToTurret)) ? gameObject.transform : gameObject2.transform);
				}
				else if (flag2)
				{
					transform = gameObject2.transform;
				}
				else if (flag)
				{
					transform = gameObject.transform;
				}
				else if (flag3)
				{
					transform = gameObject3.transform;
				}
				if (transform != null)
				{
					SetTargetToMove(transform);
					_isEntered = true;
				}
			}
			else if (IsCurrentTargetLost())
			{
				SetTargetToMove(null);
				_isEntered = false;
			}
		}

		private float GetDistanceToPlayer(GameObject playerObj)
		{
			if (playerObj == null)
			{
				return -1f;
			}
			if (IsTargetNotAvailabel(playerObj.transform, TargetType.Player))
			{
				return -1f;
			}
			return Vector3.Distance(base.transform.position, playerObj.transform.position);
		}

		private float GetDistanceToTurret(GameObject turretObj)
		{
			if (turretObj == null)
			{
				return -1f;
			}
			if (IsTargetNotAvailabel(turretObj.transform, TargetType.Turret) && IsTargetNotAvailabel(turretObj.transform, TargetType.Pet))
			{
				return -1f;
			}
			return Vector3.Distance(base.transform.position, turretObj.transform.position);
		}

		private GameObject GetNearestTargetForMultiplayer(out bool isTargetPlayer)
		{
			isTargetPlayer = false;
			GameObject result = null;
			if (Initializer.players.Count > 0)
			{
				float num = float.MaxValue;
				for (int i = 0; i < Initializer.players.Count; i++)
				{
					if (!IsTargetNotAvailabel(Initializer.players[i]))
					{
						float num2 = Vector3.SqrMagnitude(base.transform.position - Initializer.players[i].myPlayerTransform.position);
						if (num2 < num)
						{
							num = num2;
							result = Initializer.players[i].mySkinName.gameObject;
							isTargetPlayer = true;
						}
					}
				}
				for (int j = 0; j < Initializer.turretsObj.Count; j++)
				{
					if (!IsTargetNotAvailabel(Initializer.turretsObj[j].transform, TargetType.Turret))
					{
						float num3 = Vector3.SqrMagnitude(base.transform.position - Initializer.turretsObj[j].transform.position);
						if (num3 < num)
						{
							num = num3;
							result = Initializer.turretsObj[j];
							isTargetPlayer = false;
						}
					}
				}
				for (int k = 0; k < Initializer.petsObj.Count; k++)
				{
					if (!IsTargetNotAvailabel(Initializer.petsObj[k].transform, TargetType.Pet))
					{
						float num4 = Vector3.SqrMagnitude(base.transform.position - Initializer.petsObj[k].transform.position);
						if (num4 < num)
						{
							num = num4;
							result = Initializer.petsObj[k];
							isTargetPlayer = false;
						}
					}
				}
			}
			return result;
		}

		private bool IsCurrentTargetLost()
		{
			return IsTargetLost(currentTarget);
		}

		private bool IsTargetLost(Transform target)
		{
			if (target == null)
			{
				return true;
			}
			if (_isTargetCaptureForce && target.gameObject == null)
			{
				return true;
			}
			TargetType targetType = GetTargetType(target);
			if (targetType == TargetType.Player)
			{
				if (IsTargetNotAvailabel(target, TargetType.Player))
				{
					return true;
				}
				if (!_isTargetCaptureForce && Vector3.SqrMagnitude(base.transform.position - target.transform.position) > _botController.GetSquareDetectRadius())
				{
					return true;
				}
			}
			if (targetType == TargetType.Turret && IsTargetNotAvailabel(target, TargetType.Turret))
			{
				return true;
			}
			if (targetType == TargetType.Pet && IsTargetNotAvailabel(target, TargetType.Pet))
			{
				return true;
			}
			return false;
		}

		private bool IsTargetNotAvailabel(Transform target, TargetType targetType)
		{
			if (targetType == TargetType.Player)
			{
				SkinName component = target.GetComponent<SkinName>();
				if (component != null && component.playerMoveC.isInvisible)
				{
					return true;
				}
			}
			if (targetType == TargetType.Turret)
			{
				TurretController component2 = target.GetComponent<TurretController>();
				if (component2 == null || component2.isKilled || !component2.isRun)
				{
					return true;
				}
			}
			if (targetType == TargetType.Pet)
			{
				PetEngine component3 = target.GetComponent<PetEngine>();
				if (component3 == null || !component3.IsAlive)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsTargetNotAvailabel(Player_move_c target)
		{
			if (target != null && target.isInvisible)
			{
				return true;
			}
			return false;
		}

		public void SetTargetForced(Transform target)
		{
			SetTargetToMove(target);
			_isTargetCaptureForce = true;
		}
	}
}

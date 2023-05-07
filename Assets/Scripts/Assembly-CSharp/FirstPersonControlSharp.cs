using System.Collections;
using Rilisoft;
using UnityEngine;

public sealed class FirstPersonControlSharp : MonoBehaviour
{
	private const string newbieJumperAchievement = "NewbieJumperAchievement";

	private const int maxJumpCount = 10;

	private const string keyNinja = "NinjaJumpsCount";

	public Transform cameraPivot;

	public float forwardSpeed = 4f;

	public float backwardSpeed = 1f;

	public float sidestepSpeed = 1f;

	public float jumpSpeed = 4.5f;

	public float inAirMultiplier = 0.25f;

	public Vector2 rotationSpeed = new Vector2(2f, 1f);

	public float tiltPositiveYAxis = 0.6f;

	public float tiltNegativeYAxis = 0.4f;

	public float tiltXAxisMinimum = 0.1f;

	public string myIp;

	public GameObject playerGameObject;

	public int typeAnim;

	private Transform thisTransform;

	public GameObject camPlayer;

	public CharacterController character;

	private Vector3 cameraVelocity;

	private Vector3 velocity;

	private bool canJump = true;

	public bool isMine;

	private Rect fireZone;

	private Rect jumpZone;

	private float timeUpdateAnim;

	public AudioClip jumpClip;

	private Player_move_c _moveC;

	public float gravityMultiplier = 1f;

	private Vector3 mousePosOld = Vector3.zero;

	private bool _invert;

	public bool ninjaJumpUsed = true;

	private HungerGameController hungerGameController;

	private bool isHunger;

	private bool isInet;

	private bool isMulti;

	private SkinName mySkinName;

	private int oldJumpCount;

	private int oldNinjaJumpsCount;

	private Vector3 _movement;

	private Vector2 _cameraMouseDelta;

	private bool jumpBy3dTouch;

	private Vector3 rinkMovement;

	private bool steptRink;

	private bool secondJumpEnabled = true;

	private bool jump
	{
		get
		{
			return Defs.isJump;
		}
		set
		{
			Defs.isJump = value;
		}
	}

	private void Awake()
	{
		isHunger = Defs.isHunger;
		isInet = Defs.isInet;
		isMulti = Defs.isMulti;
	}

	private void Start()
	{
		Screen.lockCursor = true;
		mySkinName = GetComponent<SkinName>();
		if (!isInet)
		{
			isMine = GetComponent<NetworkView>().isMine;
		}
		else
		{
			isMine = PhotonView.Get(this).isMine;
		}
		if (isHunger)
		{
			hungerGameController = HungerGameController.Instance;
		}
		if (!isMulti || isMine)
		{
			HandleInvertCamUpdated();
			PauseNGUIController.InvertCamUpdated += HandleInvertCamUpdated;
			oldJumpCount = PlayerPrefs.GetInt("NewbieJumperAchievement", 0);
			oldNinjaJumpsCount = (Storager.hasKey("NinjaJumpsCount") ? Storager.getInt("NinjaJumpsCount", false) : 0);
		}
		thisTransform = GetComponent<Transform>();
		character = GetComponent<CharacterController>();
		_moveC = playerGameObject.GetComponent<Player_move_c>();
	}

	private void HandleInvertCamUpdated()
	{
		_invert = PlayerPrefs.GetInt(Defs.InvertCamSN, 0) == 1;
	}

	private void OnEndGame()
	{
		if (!isMulti || isMine)
		{
			if ((bool)JoystickController.leftJoystick)
			{
				JoystickController.leftJoystick.transform.parent.gameObject.SetActive(false);
			}
			if ((bool)JoystickController.rightJoystick)
			{
				JoystickController.rightJoystick.gameObject.SetActive(false);
			}
		}
		base.enabled = false;
	}

	[RPC]
	[PunRPC]
	private void setIp(string _ip)
	{
		myIp = _ip;
	}

	private Vector2 updateKeyboardControls()
	{
		int num = 0;
		int num2 = 0;
		if (Input.GetKey("w"))
		{
			num = 1;
		}
		if (Input.GetKey("s"))
		{
			num = -1;
		}
		if (Input.GetKey("a"))
		{
			num2 = -1;
		}
		if (Input.GetKey("d"))
		{
			num2 = 1;
		}
		return new Vector2(num2, num);
	}

	private void Jump()
	{
		if (!TrainingController.TrainingCompleted)
		{
			TrainingController.timeShowJump = 1000f;
			HintController.instance.HideHintByName("press_jump");
		}
		jump = true;
		canJump = false;
		if (!Defs.isJetpackEnabled && (!WeaponManager.sharedManager.myPlayerMoveC.isMechActive || !WeaponManager.sharedManager.myPlayerMoveC.IsGadgetEffectActive(Player_move_c.GadgetEffect.demon)))
		{
			QuestMediator.NotifyJump();
			mySkinName.sendAnimJump();
		}
		if (!TrainingController.TrainingCompleted || (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android && BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer) || !Social.localUser.authenticated)
		{
			return;
		}
		int num = oldJumpCount + 1;
		if (oldJumpCount >= 10)
		{
			return;
		}
		oldJumpCount = num;
		if (num != 10)
		{
			return;
		}
		PlayerPrefs.SetInt("NewbieJumperAchievement", num);
		float newProgress = 100f;
		string text = ((BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer && Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "CgkIr8rGkPIJEAIQAQ" : "Jumper_id");
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AGSAchievementsClient.UpdateAchievementProgress(text, newProgress);
			return;
		}
		Social.ReportProgress(text, newProgress, delegate(bool success)
		{
			string text2 = string.Format("Newbie Jumper achievement progress {0:0.0}%: {1}", newProgress, success);
		});
	}

	private void Update()
	{
		if ((isMulti && !isMine) || mySkinName.playerMoveC.isKilled || JoystickController.leftJoystick == null || JoystickController.rightJoystick == null)
		{
			return;
		}
		if (mySkinName.playerMoveC.isRocketJump && character.isGrounded)
		{
			mySkinName.playerMoveC.isRocketJump = false;
		}
		if (Screen.lockCursor)
		{
			MoveCamera(new Vector2(Input.GetAxis("Mouse X") * 10, Input.GetAxis("Mouse Y") * 10));
			JoystickController.leftJoystick.value = updateKeyboardControls();
		}
		_movement = thisTransform.TransformDirection(new Vector3(JoystickController.leftJoystick.value.x, 0f, JoystickController.leftJoystick.value.y));
		if ((!isHunger || !hungerGameController.isGo) && isHunger)
		{
			_movement = Vector3.zero;
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None && TrainingController.stepTraining < TrainingState.TapToMove)
		{
			_movement = Vector3.zero;
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None && TrainingController.stepTraining == TrainingState.TapToMove && _movement != Vector3.zero)
		{
			TrainingController.isNextStep = TrainingState.TapToMove;
		}
		_movement.y = 0f;
		_movement.Normalize();
		JoystickController.leftJoystick.value = updateKeyboardControls();
		Vector2 vector = new Vector2(Mathf.Abs(JoystickController.leftJoystick.value.x), Mathf.Abs(JoystickController.leftJoystick.value.y));
		if (Input.GetKeyDown(KeyCode.Space))
		{
			JoystickController.rightJoystick.jumpPressed = true;
		}
		if (Input.GetKeyUp(KeyCode.Space))
		{
			JoystickController.rightJoystick.jumpPressed = false;
		}
		if (Input.GetKey("r") && !_moveC.isReloading)
		{
			_moveC.ReloadPressed();
		}
		if (JoystickController.leftTouchPad.isShooting && JoystickController.leftTouchPad.isActiveFireButton && !Application.isMobilePlatform)
		{
			vector = new Vector2(0f, 0f);
		}
		if (Defs.isMulti && Input.GetKeyDown("t") && !_moveC.showChat)
		{
			_moveC.ShowChat();
		}
		if (Defs.isMulti && Input.GetKeyDown("tab") && !_moveC.showRanks)
		{
			_moveC.RanksPressed();
		}
		if (Defs.isMulti && Input.GetKeyUp("tab") && _moveC.showRanks)
		{
			NetworkStartTableNGUIController.sharedController.BackPressFromRanksTable(true);
		}
		if (Input.GetKey("enter") && RespawnWindow.Instance != null && RespawnWindow.Instance.isShown || Input.GetKeyDown(KeyCode.Space) && RespawnWindow.Instance != null && RespawnWindow.Instance.isShown)
		{
			RespawnWindow.Instance.OnBtnGoBattleClick();
		}
		if (Input.GetKeyDown("v"))
		{
			CameraSceneController sharedController = CameraSceneController.sharedController;
			if (sharedController != null && sharedController.killCamController != null)
			{
				sharedController.killCamController.lastDistance = 1f;
				sharedController.SetTargetKillCam(base.transform);
			}
		}
		if (Input.GetMouseButton(0) && ((Screen.lockCursor)) && !_moveC.isKilled && !_moveC.gadgeted)
		{
			_moveC.ShotPressed();
		}
		if (Input.GetMouseButtonDown(1) && Screen.lockCursor)
		{
			if ((!isMulti || isMine) && (bool)_moveC && WeaponManager.sharedManager != null && WeaponManager.sharedManager.currentWeaponSounds != null && WeaponManager.sharedManager.currentWeaponSounds.isZooming)
			{
				_moveC.ZoomPress();
			}
		}
		mousePosOld = Input.mousePosition;
		if (vector.y > vector.x)
		{
			if (JoystickController.leftJoystick.value.y > 0f)
			{
				_movement *= forwardSpeed * EffectsController.SpeedModifier(WeaponManager.sharedManager.currentWeaponSounds.categoryNabor - 1) * vector.y;
			}
			else
			{
				_movement *= backwardSpeed * EffectsController.SpeedModifier(WeaponManager.sharedManager.currentWeaponSounds.categoryNabor - 1) * vector.y;
			}
		}
		else
		{
			_movement *= sidestepSpeed * EffectsController.SpeedModifier(WeaponManager.sharedManager.currentWeaponSounds.categoryNabor - 1) * vector.x * (float)((!character.isGrounded) ? 1 : 1);
		}
		bool flag = Defs.isJetpackEnabled || (WeaponManager.sharedManager.myPlayerMoveC.isMechActive && WeaponManager.sharedManager.myPlayerMoveC.IsGadgetEffectActive(Player_move_c.GadgetEffect.demon));
		if (character.isGrounded)
		{
			if (EffectsController.NinjaJumpEnabled)
			{
				ninjaJumpUsed = false;
			}
			canJump = true;
			jump = false;
			TouchPadController rightJoystick = JoystickController.rightJoystick;
			if (canJump && (rightJoystick.jumpPressed || JoystickController.leftTouchPad.isJumpPressed))
			{
				if (!flag)
				{
					rightJoystick.jumpPressed = false;
				}
				Jump();
			}
			if (jump)
			{
				secondJumpEnabled = false;
				if (!JoystickController.leftTouchPad.isJumpPressed)
				{
					StartCoroutine(EnableSecondJump());
				}
				else
				{
					jumpBy3dTouch = true;
				}
				velocity = Vector3.zero;
				velocity.y = jumpSpeed * EffectsController.JumpModifier;
			}
		}
		else
		{
			if (!JoystickController.leftTouchPad.isJumpPressed && jumpBy3dTouch)
			{
				secondJumpEnabled = true;
				jumpBy3dTouch = false;
			}
			if (jump && mySkinName.interpolateScript.myAnim == 0 && !flag)
			{
				mySkinName.sendAnimJump();
			}
			TouchPadController rightJoystick2 = JoystickController.rightJoystick;
			TouchPadInJoystick leftTouchPad = JoystickController.leftTouchPad;
			if ((rightJoystick2.jumpPressed || leftTouchPad.isJumpPressed) && ((EffectsController.NinjaJumpEnabled && !ninjaJumpUsed && secondJumpEnabled) || flag))
			{
				if (!flag)
				{
					QuestMediator.NotifyJump();
					RegisterNinjAchievment();
				}
				ninjaJumpUsed = true;
				canJump = false;
				if (!flag)
				{
					mySkinName.sendAnimJump();
				}
				velocity.y = 1.1f * (jumpSpeed * EffectsController.JumpModifier);
			}
			if (!flag)
			{
				rightJoystick2.jumpPressed = false;
			}
			velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
		}
		if (WeaponManager.sharedManager.myPlayerMoveC.IsPlayerEffectActive(Player_move_c.PlayerEffect.rocketFly))
		{
			velocity.y = 25f;
		}
		_movement += velocity;
		_movement += Physics.gravity * gravityMultiplier;
		if (Defs.isMulti && !Defs.isCOOP && !WeaponManager.sharedManager.myPlayerMoveC.isImmortality)
		{
			Vector3 zero = Vector3.zero;
			bool flag2 = false;
			for (int i = 0; i < Initializer.singularities.Count; i++)
			{
				if (!Initializer.singularities[i].owner.Equals(WeaponManager.sharedManager.myPlayerMoveC) && (!ConnectSceneNGUIController.isTeamRegim || WeaponManager.sharedManager.myPlayerMoveC.myCommand != Initializer.singularities[i].owner.myCommand))
				{
					SingularityHole singularityHole = Initializer.singularities[i];
					Vector3 vector2 = singularityHole.transform.position - base.transform.position;
					float force = singularityHole.GetForce(vector2.sqrMagnitude);
					if (force > 0f)
					{
						zero += vector2.normalized * force;
					}
					if (force < 0f)
					{
						flag2 = true;
					}
				}
			}
			if (zero.sqrMagnitude >= 0f)
			{
				if (zero.y > 0f || flag2)
				{
					_movement.y = 0f;
					velocity.y = 0f;
				}
				_movement += zero;
			}
			for (int j = 0; j < Initializer.players.Count; j++)
			{
				if (Initializer.players[j].IsPlayerEffectActive(Player_move_c.PlayerEffect.attrackPlayer))
				{
					Vector3 normalized = (Initializer.players[j].myPlayerTransform.position + Initializer.players[j].myPlayerTransform.forward * 1.2f - base.transform.position).normalized;
					_movement += normalized * 5f;
				}
			}
		}
		_movement *= Time.deltaTime;
		timeUpdateAnim -= Time.deltaTime;
		if (timeUpdateAnim < 0f)
		{
			if (character.isGrounded)
			{
				timeUpdateAnim = 0.5f;
				if (new Vector2(_movement.x, _movement.z).sqrMagnitude > 0f)
				{
					_moveC.WalkAnimation();
				}
				else
				{
					_moveC.IdleAnimation();
				}
			}
			else
			{
				_moveC.WalkAnimation();
			}
		}
		Update2();
	}

	private float sensitivity = 10f;

	private void Update2()
	{
		if (!character.enabled)
		{
			return;
		}
		if (!mySkinName.onRink)
		{
			if (mySkinName.onConveyor)
			{
				_movement += mySkinName.conveyorDirection * Time.deltaTime;
			}
			Vector2 delta = GrabCameraInputDelta();
			if (Device.isPixelGunLow && Defs.isTouchControlSmoothDump)
			{
				MoveCamera(delta);
			}
			character.Move(_movement);
			_movement = Vector3.zero;
			steptRink = false;
		}
		else
		{
			if (!steptRink)
			{
				rinkMovement = _movement;
				steptRink = true;
			}
			rinkMovement = Vector3.MoveTowards(rinkMovement, _movement, 0.068f * Time.deltaTime);
			rinkMovement.y = _movement.y;
			character.Move(rinkMovement);
		}
		if (character.isGrounded)
		{
			velocity = Vector3.zero;
		}
		else
		{
			if (mySkinName.onRink)
			{
				rinkMovement = _movement;
			}
			mySkinName.onConveyor = false;
		}
		if (Defs.isMulti && CameraSceneController.sharedController.killCamController.enabled)
		{
			CameraSceneController.sharedController.killCamController.UpdateMouseX();
		}
	}

	public void MoveCamera(Vector2 delta)
	{
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None && TrainingController.stepTraining == TrainingState.SwipeToRotate && delta != Vector2.zero)
		{
			TrainingController.isNextStep = TrainingState.SwipeToRotate;
		}
		float sensitivity = Defs.Sensitivity;
		float num = 1f;
		if (_moveC != null)
		{
			num *= ((!_moveC.isZooming) ? 1f : 0.2f);
		}
		if (JoystickController.rightJoystick != null)
		{
			JoystickController.rightJoystick.ApplyDeltaTo(delta, thisTransform, cameraPivot.transform, sensitivity * num, _invert);
		}
	}

	private Vector2 GrabCameraInputDelta()
	{
		Vector2 result = Vector2.zero;
		TouchPadController rightJoystick = JoystickController.rightJoystick;
		if (rightJoystick != null)
		{
			result = rightJoystick.GrabDeltaPosition();
		}
		return result;
	}

	private void RegisterNinjAchievment()
	{
		if (!Social.localUser.authenticated)
		{
			return;
		}
		int num = oldNinjaJumpsCount + 1;
		if (oldNinjaJumpsCount < 50)
		{
			Storager.setInt("NinjaJumpsCount", num, false);
		}
		oldNinjaJumpsCount = num;
		if (Storager.hasKey("ParkourNinjaAchievementCompleted") || num < 50)
		{
			return;
		}
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
		{
			GpgFacade.Instance.IncrementAchievement("CgkIr8rGkPIJEAIQAw", 1, delegate(bool success)
			{
				Debug.Log("Achievement Parkour Ninja incremented: " + success);
			});
		}
		Storager.setInt("ParkourNinjaAchievementCompleted", 1, false);
	}

	private IEnumerator EnableSecondJump()
	{
		yield return new WaitForSeconds(0.25f);
		secondJumpEnabled = true;
	}

	private void OnDestroy()
	{
		if (!isMulti || isMine)
		{
			PauseNGUIController.InvertCamUpdated -= HandleInvertCamUpdated;
		}
	}
}

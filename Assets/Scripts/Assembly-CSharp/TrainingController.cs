using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Rilisoft;
using RilisoftBot;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public sealed class TrainingController : MonoBehaviour
{
	public enum NewTrainingCompletedStage
	{
		None,
		ShootingRangeCompleted,
		ShopCompleted,
		FirstMatchCompleted
	}

	public const string NewTrainingStageHolderKey = "TrainingController.NewTrainingStageHolderKey";

	public static TrainingController sharedController;

	private static bool _trainingCompletedInitialized;

	private static bool cachedTrainingComplete;

	private static bool _comletedTrainingStageInitialized;

	private static NewTrainingCompletedStage _completedTrainingStage;

	public GameObject swipeToRotateOverlay;

	public GameObject dragToMoveOverlay;

	public GameObject pickupGunOverlay;

	public GameObject touch3dPressGun;

	public GameObject touch3dPressFire;

	public GameObject wellDoneOverlay;

	public GameObject getCoinOverlay;

	public GameObject enterShopOverlay;

	public GameObject shopArrowOverlay;

	public GameObject swipeToChangeWeaponOverlay;

	public GameObject tapToChangeWeaponOverlay;

	public GameObject shootReloadOverlay;

	public GameObject selectGrenadeOverlay;

	public GameObject buyGrenadeArrowOverlay;

	public GameObject throwGrenadeOverlay;

	public GameObject throwGrenadeArrowOverlay;

	public GameObject killZombiesOverlay;

	public GameObject overlaysRoot;

	public GameObject joystickFingerOverlay;

	public GameObject joystickShadowOverlay;

	public GameObject touchpadOverlay;

	public GameObject touchpadFingerOverlay;

	public GameObject swipeWeaponFingerOverlay;

	public GameObject tapWeaponArrowOverlay;

	public GameObject enemyPrototype;

	public GameObject[] enemies;

	public Transform teleportTransform;

	public Transform weaponTransform;

	public Transform playerTransform;

	private static readonly Vector3 _playerDefaultPosition;

	private GameObject[] _overlays = new GameObject[0];

	internal static TrainingState stepTraining;

	internal static Dictionary<string, TrainingState> stepTrainingList;

	internal static TrainingState isNextStep;

	public static bool isPressSkip;

	public static bool isPause;

	private Rect animTextureRect;

	private static bool nextStepAfterSkipTraining;

	private GameObject coinsPrefab;

	private Texture2D[] animTextures;

	private static int stepAnim;

	private static int maxStepAnim;

	private static bool isCanceled;

	private float speedAnim;

	private static TrainingState setNextStepInd;

	private Texture2D shop;

	private Texture2D shop_n;

	private bool isAnimShop;

	private static TrainingState oldStepTraning;

	private UIButton shopButton;

	private static bool? _trainingCompleted;

	private ActionDisposable _weaponChangedSubscription = new ActionDisposable(delegate
	{
	});

	private readonly List<TrainingEnemy> _enemies = new List<TrainingEnemy>(3);

	private UIButton _pauseButton;

	private int _weaponChangingCount;

	public static float timeShowJump;

	public static float timeShow3dTouchJump;

	private bool isShow3dTouchJump;

	public static float timeShowFire;

	public static float timeShow3dTouchFire;

	private bool isShow3dTouchFire;

	private GameObject weapon;

	private readonly Lazy<PlayerArrowToPortalController> _directionArrow;

	public static bool TrainingCompleted
	{
		get
		{
			if (cachedTrainingComplete)
			{
				return true;
			}
			if (!_trainingCompletedInitialized)
			{
				if (Storager.getInt(Defs.TrainingCompleted_4_4_Sett, false) == 0 && PlayerPrefs.GetInt(Defs.TrainingCompleted_4_4_Sett, 0) == 1)
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.Log("Trying to set TrainingCompleted flag...");
					}
					OnGetProgress();
				}
				if (TrainingController.onChangeTraining != null)
				{
					TrainingController.onChangeTraining();
				}
				_trainingCompletedInitialized = true;
			}
			cachedTrainingComplete = Storager.getInt(Defs.TrainingCompleted_4_4_Sett, false) > 0 || CompletedTrainingStage == NewTrainingCompletedStage.FirstMatchCompleted;
			return cachedTrainingComplete;
		}
	}

	public static NewTrainingCompletedStage CompletedTrainingStage
	{
		get
		{
			if (!_comletedTrainingStageInitialized)
			{
				_comletedTrainingStageInitialized = true;
				_completedTrainingStage = (NewTrainingCompletedStage)Storager.getInt("TrainingController.NewTrainingStageHolderKey", false);
			}
			return _completedTrainingStage;
		}
		set
		{
			_comletedTrainingStageInitialized = true;
			if (_completedTrainingStage == value)
			{
				return;
			}
			_completedTrainingStage = value;
			Storager.setInt("TrainingController.NewTrainingStageHolderKey", (int)_completedTrainingStage, false);
			if (_completedTrainingStage == NewTrainingCompletedStage.FirstMatchCompleted)
			{
				Action action = TrainingController.onChangeTraining;
				if (action != null)
				{
					action();
				}
			}
		}
	}

	public Vector3 PlayerDesiredPosition
	{
		get
		{
			return (!(playerTransform != null)) ? _playerDefaultPosition : playerTransform.position;
		}
	}

	public static Vector3 PlayerDefaultPosition
	{
		get
		{
			return _playerDefaultPosition;
		}
	}

	public static bool? TrainingCompletedFlagForLogging
	{
		get
		{
			return _trainingCompleted;
		}
		set
		{
			_trainingCompleted = value;
		}
	}

	public static bool FireButtonEnabled
	{
		get
		{
			return stepTraining >= TrainingState.KillZombie;
		}
	}

	public static event Action onChangeTraining;

	public TrainingController()
	{
		_directionArrow = new Lazy<PlayerArrowToPortalController>(delegate
		{
			GameObject gameObject = GameObject.FindWithTag("Player");
			return (gameObject == null) ? null : gameObject.GetComponent<PlayerArrowToPortalController>();
		});
	}

	static TrainingController()
	{
		sharedController = null;
		_trainingCompletedInitialized = false;
		cachedTrainingComplete = false;
		_comletedTrainingStageInitialized = false;
		_completedTrainingStage = NewTrainingCompletedStage.None;
		_playerDefaultPosition = new Vector3(-0.72f, 1.75f, -13.23f);
		stepTraining = (TrainingState)(-1);
		stepTrainingList = new Dictionary<string, TrainingState>(10);
		isNextStep = TrainingState.None;
		isPause = false;
		nextStepAfterSkipTraining = false;
		setNextStepInd = TrainingState.None;
		timeShowJump = 0f;
		timeShow3dTouchJump = 0f;
		timeShowFire = 0f;
		timeShow3dTouchFire = 0f;
		stepTrainingList.Add("TapToMove", TrainingState.TapToMove);
		stepTrainingList.Add("GetTheGun", TrainingState.GetTheGun);
		stepTrainingList.Add("WellDone", TrainingState.WellDone);
		stepTrainingList.Add("Shop", TrainingState.Shop);
		stepTrainingList.Add("TapToSelectWeapon", TrainingState.TapToSelectWeapon);
		stepTrainingList.Add("TapToShoot", TrainingState.TapToShoot);
		stepTrainingList.Add("TapToThrowGrenade", TrainingState.TapToThrowGrenade);
		stepTrainingList.Add("KillZombi", TrainingState.KillZombie);
		stepTrainingList.Add("GoToPortal", TrainingState.GoToPortal);
	}

	public static void OnGetProgress()
	{
		PlayerPrefs.SetString(LeaderboardScript.LeaderboardsResponseCacheTimestamp, DateTime.UtcNow.Subtract(TimeSpan.FromHours(2.0)).ToString("s", CultureInfo.InvariantCulture));
		if (ShopNGUIController.NoviceArmorAvailable)
		{
			ShopNGUIController.UnequipCurrentWearInCategory(ShopNGUIController.CategoryNames.ArmorCategory, false);
			ShopNGUIController.ProvideItem(ShopNGUIController.CategoryNames.ArmorCategory, "Armor_Army_1", 1, false, 0, null, null, true, false, false);
		}
		Storager.setInt("Training.ShouldRemoveNoviceArmorInShopKey", 0, false);
		AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Get_Progress);
		Storager.setInt(Defs.TrainingCompleted_4_4_Sett, 1, false);
		if (!Defs.isABTestBalansNoneSkip)
		{
			FriendsController.ResetABTestsBalans();
		}
		FriendsController.ResetABTestAdvert();
		foreach (ABTestBase currentABTest in ABTestController.currentABTests)
		{
			currentABTest.ResetABTest();
		}
		if (ABTestController.useBuffSystem)
		{
			BuffSystem.instance.OnGetProgress();
		}
		else
		{
			KillRateCheck.instance.OnGetProgress();
		}
	}

	public static void SkipTraining()
	{
		oldStepTraning = stepTraining;
		stepTraining = TrainingState.None;
		isPressSkip = true;
		isCanceled = true;
		_trainingCompleted = false;
		AnalyticsFacade.SendCustomEventToAppsFlyer("Training complete", new Dictionary<string, string>());
	}

	public static void CancelSkipTraining()
	{
		isCanceled = false;
		isPressSkip = false;
		stepTraining = oldStepTraning;
		TrainingController component = GameObject.FindGameObjectWithTag("TrainingController").GetComponent<TrainingController>();
		if (nextStepAfterSkipTraining)
		{
			nextStepAfterSkipTraining = false;
			component.StartNextStepTraning();
		}
		if (stepAnim == 0)
		{
			component.FirstStep();
		}
		else
		{
			component.NextStepAnim();
		}
	}

	private void AdjustShootReloadLabel()
	{
		bool flag = PlayerPrefs.GetInt(Defs.SwitchingWeaponsSwipeRegimSN, 0) == 1;
		if (shootReloadOverlay != null && flag)
		{
			shootReloadOverlay.transform.localPosition = shootReloadOverlay.transform.localPosition - new Vector3(120f, 0f, 0f);
		}
	}

	private void AdjustJoystickAreaAndFinger()
	{
		float num = (GlobalGameController.LeftHanded ? 1 : (-1));
		Vector3 vector = new Vector3((float)Defs.JoyStickX * num, Defs.JoyStickY, 0f);
		if (dragToMoveOverlay != null)
		{
			dragToMoveOverlay.transform.localPosition = vector + new Vector3(30f, 120f, 0f);
		}
		Vector3[] array = Load.LoadVector3Array(ControlsSettingsBase.JoystickSett);
		if (array != null && array.Length > 4)
		{
			vector = array[4];
		}
		if (joystickShadowOverlay != null)
		{
			joystickShadowOverlay.GetComponent<RectTransform>().anchoredPosition = vector;
		}
		TrainingFinger trainingFinger = ((!(joystickFingerOverlay == null)) ? joystickFingerOverlay.GetComponent<TrainingFinger>() : null);
		if (trainingFinger != null)
		{
			trainingFinger.GetComponent<RectTransform>().anchoredPosition = vector + new Vector3(20f, 20f, 0f);
		}
	}

	private void AdjustGrenadeLabelAndArrow()
	{
		Vector3 zero = Vector3.zero;
		Vector3[] array = Load.LoadVector3Array(ControlsSettingsBase.JoystickSett);
		if (array == null || array.Length < 6)
		{
			float num = (GlobalGameController.LeftHanded ? 1 : (-1));
			zero = new Vector3((float)Defs.GrenadeX * num, Defs.GrenadeY, 0f);
		}
		else
		{
			zero = array[5];
		}
		TrainingArrow trainingArrow = ((!(buyGrenadeArrowOverlay == null)) ? buyGrenadeArrowOverlay.GetComponent<TrainingArrow>() : null);
		if (trainingArrow != null)
		{
			trainingArrow.SetAnchoredPosition(zero - new Vector3(64f, 0f, 0f));
		}
		TrainingArrow trainingArrow2 = ((!(throwGrenadeArrowOverlay == null)) ? throwGrenadeArrowOverlay.GetComponent<TrainingArrow>() : null);
		if (trainingArrow2 != null)
		{
			trainingArrow2.SetAnchoredPosition(zero - new Vector3(90f, -60f, 0f));
		}
		if (selectGrenadeOverlay != null)
		{
			selectGrenadeOverlay.transform.localPosition = zero - new Vector3(120f, 0f, 0f);
		}
		if (throwGrenadeOverlay != null)
		{
			throwGrenadeOverlay.transform.localPosition = zero - new Vector3(400f, -120f, 0f);
		}
	}

	private IEnumerator Start()
	{
		sharedController = this;
		PlayerArrowToPortalController.PopulateArrowPoolIfEmpty();
		_overlays = new GameObject[10] { swipeToRotateOverlay, dragToMoveOverlay, pickupGunOverlay, wellDoneOverlay, getCoinOverlay, enterShopOverlay, shootReloadOverlay, selectGrenadeOverlay, throwGrenadeOverlay, killZombiesOverlay };
		isPause = false;
		animTextures = new Texture2D[3];
		stepTraining = TrainingState.None;
		isNextStep = TrainingState.None;
		setNextStepInd = TrainingState.None;
		StartNextStepTraning();
		coinsPrefab = GameObject.FindGameObjectWithTag("CoinBonus");
		if (coinsPrefab != null)
		{
			coinsPrefab.SetActive(false);
		}
		PlayerPrefs.SetInt("LogCountMatch", 1);
		while (GameObject.FindGameObjectWithTag("InGameGUI") == null)
		{
			yield return null;
		}
		shopButton = GameObject.FindGameObjectWithTag("InGameGUI").GetComponent<InGameGUI>().shopButton.GetComponent<UIButton>();
		InGameGUI.sharedInGameGUI.SetSwipeWeaponPanelVisibility(false);
		_pauseButton = InGameGUI.sharedInGameGUI.pauseButton;
		if (_pauseButton != null)
		{
			_pauseButton.isEnabled = false;
		}
		if (!(InGameGUI.sharedInGameGUI != null))
		{
			yield break;
		}
		if (InGameGUI.sharedInGameGUI.jumpButton != null && pickupGunOverlay != null)
		{
			List<UISprite> sprites2 = new List<UISprite>();
			InGameGUI.sharedInGameGUI.jumpButton.GetComponentsInChildren(true, sprites2);
			TrainingBlinking tb2 = pickupGunOverlay.AddComponent<TrainingBlinking>();
			tb2.SetSprites(sprites2);
		}
		if (InGameGUI.sharedInGameGUI.fireButton != null)
		{
			List<UISprite> sprites = new List<UISprite>();
			InGameGUI.sharedInGameGUI.fireButton.GetComponentsInChildren(true, sprites);
			if (killZombiesOverlay != null)
			{
				TrainingBlinking tb = killZombiesOverlay.AddComponent<TrainingBlinking>();
				tb.SetSprites(sprites);
			}
		}
	}

	private void OnDestroy()
	{
		sharedController = null;
		if (_pauseButton != null)
		{
			_pauseButton.isEnabled = true;
		}
		_weaponChangedSubscription.Dispose();
	}

	private void HandleWeaponChanged(object sender, EventArgs e)
	{
		if (_weaponChangingCount > 0 && stepTraining == TrainingState.TapToSelectWeapon)
		{
			isNextStep = TrainingState.TapToSelectWeapon;
		}
		_weaponChangingCount++;
	}

	[Obfuscation(Exclude = true)]
	public void StartNextStepTraning()
	{
		if (isPressSkip)
		{
			nextStepAfterSkipTraining = true;
			return;
		}
		stepTraining++;
		Vector2 vector = Vector2.zero;
		if (stepTraining == TrainingState.SwipeToRotate)
		{
			AdjustJoystickAreaAndFinger();
			isCanceled = true;
			maxStepAnim = 13;
			speedAnim = 0.5f;
			stepAnim = 0;
			if (enemies != null && enemies.Length > 0)
			{
				GameObject[] array = enemies;
				foreach (GameObject gameObject in array)
				{
					TrainingEnemy item = gameObject.GetComponent<TrainingEnemy>() ?? gameObject.AddComponent<TrainingEnemy>();
					_enemies.Add(item);
				}
			}
			else if (enemyPrototype != null)
			{
				Behaviour[] array2 = new Behaviour[3]
				{
					enemyPrototype.GetComponent<BotAiController>(),
					enemyPrototype.GetComponent<MeleeBot>(),
					enemyPrototype.GetComponent<NavMeshAgent>()
				};
				Behaviour[] array3 = array2;
				foreach (Behaviour behaviour in array3)
				{
					if (behaviour != null)
					{
						behaviour.enabled = false;
						UnityEngine.Object.Destroy(behaviour);
					}
				}
				GameObject gameObject2 = new GameObject("DynamicEnemies");
				gameObject2.transform.localPosition = new Vector3(-2f, 0f, 15f);
				int enemiesToKill = GlobalGameController.EnemiesToKill;
				for (int k = 0; k < enemiesToKill; k++)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(enemyPrototype);
					gameObject3.transform.parent = gameObject2.transform;
					Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
					gameObject3.transform.localPosition = new Vector3((float)(3 * k) + insideUnitCircle.x, 0f, insideUnitCircle.y);
					gameObject3.transform.localRotation = Quaternion.AngleAxis(180f + UnityEngine.Random.Range(-60f, 60f), Vector3.up);
					TrainingEnemy item2 = gameObject3.GetComponent<TrainingEnemy>() ?? gameObject3.AddComponent<TrainingEnemy>();
					_enemies.Add(item2);
				}
			}
		}
		if (stepTraining == TrainingState.TapToMove)
		{
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Controls_Overview);
			isCanceled = true;
			maxStepAnim = 19;
			speedAnim = 0.5f;
			stepAnim = 0;
			for (int l = 0; l != animTextures.Length; l++)
			{
				animTextures[l] = null;
			}
			if (animTextures[0] != null)
			{
				vector = new Vector2(-10f * Defs.Coef, (float)Screen.height - ((float)animTextures[0].height - 51f) * Defs.Coef);
			}
		}
		if (stepTraining == TrainingState.GetTheGun)
		{
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Controls_Move);
			HintController.instance.ShowHintByName("press_jump", 0f);
			isCanceled = true;
			maxStepAnim = 2;
			speedAnim = 0.2f;
			stepAnim = 0;
			Vector3 vector2 = ((!(weaponTransform != null)) ? new Vector3(-1.6f, 1.75f, -2.6f) : weaponTransform.position);
			if (weaponTransform != null)
			{
				UnityEngine.Object.Destroy(weaponTransform.gameObject);
			}
			if (weapon == null)
			{
				weapon = BonusCreator._CreateBonus(WeaponManager.MP5WN, vector2);
			}
			else
			{
				weapon.transform.position = vector2;
			}
			if (_directionArrow.Value != null)
			{
				_directionArrow.Value.RemovePointOfInterest();
				_directionArrow.Value.SetPointOfInterest(weapon.transform);
			}
		}
		if (stepTraining == TrainingState.WellDone || stepTraining == TrainingState.WellDoneCoin)
		{
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Controls_Jump);
			HintController.instance.HideHintByName("press_jump");
			HintController.instance.ShowHintByName("press_fire", 0f);
			isCanceled = true;
			maxStepAnim = 1;
			speedAnim = 1f;
			stepAnim = 0;
			if (_directionArrow.Value != null)
			{
				_directionArrow.Value.RemovePointOfInterest();
			}
		}
		if (stepTraining == TrainingState.GetTheCoin)
		{
			if (coinsPrefab != null)
			{
				coinsPrefab.SetActive(true);
				coinsPrefab.GetComponent<CoinBonus>().SetPlayer();
			}
			isCanceled = true;
			maxStepAnim = 2;
			speedAnim = 3f;
			stepAnim = 0;
		}
		if (stepTraining == TrainingState.EnterTheShop)
		{
			isAnimShop = false;
			AnimShop();
			isCanceled = true;
			maxStepAnim = 13;
			speedAnim = 0.3f;
			stepAnim = 0;
			if (Application.isEditor)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
		if (stepTraining == TrainingState.TapToSelectWeapon)
		{
			InGameGUI.sharedInGameGUI.SetSwipeWeaponPanelVisibility(PlayerPrefs.GetInt(Defs.SwitchingWeaponsSwipeRegimSN, 0) == 1);
			Player_move_c playerMove = GameObject.FindGameObjectWithTag("PlayerGun").GetComponent<Player_move_c>();
			if (playerMove != null)
			{
				playerMove.WeaponChanged += HandleWeaponChanged;
				_weaponChangedSubscription = new ActionDisposable(delegate
				{
					playerMove.WeaponChanged -= HandleWeaponChanged;
					_weaponChangingCount = 0;
				});
			}
		}
		else
		{
			_weaponChangedSubscription.Dispose();
		}
		if (stepTraining == TrainingState.TapToShoot)
		{
			AdjustShootReloadLabel();
			isCanceled = true;
			maxStepAnim = 2;
			speedAnim = 3f;
			stepAnim = 0;
			if (Application.isEditor)
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
		TrainingState value;
		if (stepTrainingList.TryGetValue("SwipeWeapon", out value) && value == stepTraining)
		{
			isCanceled = true;
			maxStepAnim = 13;
			speedAnim = 0.3f;
			stepAnim = 0;
			for (int m = 0; m != animTextures.Length; m++)
			{
				animTextures[m] = null;
			}
			if (animTextures[0] != null)
			{
				vector = new Vector2((float)Screen.width - (float)animTextures[0].width * Defs.Coef, 0f);
			}
		}
		if (stepTraining == TrainingState.KillZombie)
		{
			if (_enemies.Count > 0)
			{
				foreach (TrainingEnemy enemy in _enemies)
				{
					enemy.WakeUp(UnityEngine.Random.value);
				}
			}
			else
			{
				GameObject.FindGameObjectWithTag("GameController").transform.GetComponent<ZombieCreator>().BeganCreateEnemies();
			}
			InGameGUI.sharedInGameGUI.centerAnhor.SetActive(true);
			isCanceled = true;
			maxStepAnim = 2;
			speedAnim = 3f;
			stepAnim = 0;
			if (Application.isEditor)
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
		if (stepTraining == TrainingState.GoToPortal)
		{
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Kill_Enemy);
			if (_directionArrow.Value != null)
			{
				_directionArrow.Value.RemovePointOfInterest();
				_directionArrow.Value.SetPointOfInterest(teleportTransform);
			}
			PlayerPrefs.SetInt("PendingGooglePlayGamesSync", 1);
		}
		if (stepTraining == TrainingState.TapToSelectWeapon)
		{
			isCanceled = true;
			maxStepAnim = 19;
			speedAnim = 0.5f;
			stepAnim = 0;
			animTextures[0] = Resources.Load<Texture2D>("Training/ob_change_0");
			animTextures[1] = Resources.Load<Texture2D>("Training/ob_change_1");
			if (animTextures[0] != null)
			{
				vector = new Vector2((float)Screen.width * 0.5f - 164f * Defs.Coef - (float)animTextures[0].width * 0.5f * Defs.Coef, (float)Screen.height - (112f + (float)animTextures[0].height) * Defs.Coef);
			}
		}
		if (stepTraining == TrainingState.TapToThrowGrenade)
		{
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
			{
				WeaponManager.sharedManager.myPlayerMoveC.GrenadeCount = 10;
			}
			AdjustGrenadeLabelAndArrow();
			isCanceled = true;
			maxStepAnim = 19;
			speedAnim = 0.5f;
			stepAnim = 0;
			for (int n = 0; n != animTextures.Length; n++)
			{
				animTextures[n] = null;
			}
			Defs.InitCoordsIphone();
			if (animTextures[0] != null)
			{
				vector = new Vector2((float)Screen.width - ((float)(-Defs.GrenadeX + animTextures[0].width) + 80f) * Defs.Coef, (float)Screen.height - ((float)(Defs.GrenadeY + animTextures[0].height) - 80f) * Defs.Coef);
			}
		}
		if (animTextures[0] != null)
		{
			animTextureRect = new Rect(vector.x, vector.y, (float)animTextures[0].width * Defs.Coef, (float)animTextures[0].height * Defs.Coef);
		}
		Invoke("FirstStep", 1f);
	}

	[Obfuscation(Exclude = true)]
	private void AnimShop()
	{
		isAnimShop = !isAnimShop;
		bool flag = stepTraining == TrainingState.EnterTheShop;
		string normalSprite = shopButton.normalSprite;
		string pressedSprite = shopButton.pressedSprite;
		shopButton.pressedSprite = normalSprite;
		shopButton.normalSprite = pressedSprite;
		if (flag)
		{
			Invoke("AnimShop", 0.3f);
		}
	}

	[Obfuscation(Exclude = true)]
	private void FirstStep()
	{
		isCanceled = false;
		stepAnim = 0;
		NextStepAnim();
	}

	[Obfuscation(Exclude = true)]
	private void NextStepAnim()
	{
		CancelInvoke("NextStepAnim");
		if (!isCanceled)
		{
			stepAnim++;
			if (stepTraining == TrainingState.WellDone && stepAnim >= maxStepAnim)
			{
				isNextStep = TrainingState.WellDone;
			}
			else if (stepTraining == TrainingState.WellDoneCoin && stepAnim >= maxStepAnim)
			{
				isNextStep = TrainingState.WellDoneCoin;
			}
			else
			{
				Invoke("NextStepAnim", speedAnim);
			}
		}
	}

	private void Update()
	{
		if (coinsPrefab == null && stepTraining < TrainingState.GetTheCoin)
		{
			coinsPrefab = GameObject.FindGameObjectWithTag("CoinBonus");
			if (coinsPrefab != null)
			{
				coinsPrefab.SetActive(false);
			}
		}
		if (isNextStep > setNextStepInd)
		{
			setNextStepInd = isNextStep;
			if (stepTraining == TrainingState.SwipeToRotate || stepTraining == TrainingState.TapToMove)
			{
				Invoke("StartNextStepTraning", 1.5f);
			}
			else if (stepTraining == TrainingState.TapToShoot)
			{
				Invoke("StartNextStepTraning", 3f);
			}
			else
			{
				StartNextStepTraning();
			}
		}
		if (ShopNGUIController.GuiActive || isPause)
		{
			if (shopArrowOverlay != null)
			{
				shopArrowOverlay.SetActive(false);
			}
			if (buyGrenadeArrowOverlay != null)
			{
				buyGrenadeArrowOverlay.SetActive(false);
			}
			if (throwGrenadeArrowOverlay != null)
			{
				throwGrenadeArrowOverlay.SetActive(false);
			}
			if (joystickFingerOverlay != null)
			{
				joystickFingerOverlay.SetActive(false);
			}
			if (joystickShadowOverlay != null)
			{
				joystickShadowOverlay.SetActive(false);
			}
			if (touchpadOverlay != null)
			{
				touchpadOverlay.SetActive(false);
			}
			if (touchpadFingerOverlay != null)
			{
				touchpadFingerOverlay.SetActive(false);
			}
			if (swipeWeaponFingerOverlay != null)
			{
				swipeWeaponFingerOverlay.SetActive(false);
			}
			if (tapWeaponArrowOverlay != null)
			{
				tapWeaponArrowOverlay.SetActive(false);
			}
		}
	}

	private void LateUpdate()
	{
		RefreshOverlays();
	}

	public void Hide3dTouchJump()
	{
		if (touch3dPressGun.activeSelf)
		{
			touch3dPressGun.SetActive(false);
		}
	}

	public void Hide3dTouchFire()
	{
		if (touch3dPressFire.activeSelf)
		{
			touch3dPressFire.SetActive(false);
		}
	}

	private void RefreshOverlays()
	{
		if (isPause)
		{
			return;
		}
		GameObject objA = null;
		if (stepTraining == TrainingState.SwipeToRotate)
		{
			objA = swipeToRotateOverlay;
		}
		else if (stepTraining == TrainingState.TapToMove)
		{
			objA = dragToMoveOverlay;
		}
		else if (stepTraining == TrainingState.GetTheGun)
		{
			if (Defs.touchPressureSupported || Application.isEditor)
			{
				timeShowJump += Time.deltaTime;
				if (touch3dPressGun.activeSelf)
				{
					timeShow3dTouchJump += Time.deltaTime;
					if (timeShow3dTouchJump > 5f)
					{
						Hide3dTouchJump();
					}
				}
				if (!isShow3dTouchJump && timeShowJump > 3f)
				{
					isShow3dTouchJump = true;
					HintController.instance.HideHintByName("press_jump");
					touch3dPressGun.SetActive(true);
				}
			}
			objA = pickupGunOverlay;
		}
		else if (stepTraining == TrainingState.WellDone || stepTraining == TrainingState.WellDoneCoin)
		{
			objA = wellDoneOverlay;
		}
		else if (stepTraining == TrainingState.GetTheCoin)
		{
			objA = getCoinOverlay;
		}
		else if (stepTraining == TrainingState.EnterTheShop)
		{
			objA = enterShopOverlay;
		}
		else if (stepTraining == TrainingState.TapToShoot)
		{
			objA = shootReloadOverlay;
		}
		else if (stepTraining == TrainingState.TapToThrowGrenade)
		{
			objA = throwGrenadeOverlay;
		}
		else if (stepTraining == TrainingState.KillZombie)
		{
			if (Defs.touchPressureSupported || Application.isEditor)
			{
				timeShowFire += Time.deltaTime;
				if (touch3dPressFire.activeSelf)
				{
					timeShow3dTouchFire += Time.deltaTime;
					if (timeShow3dTouchFire > 5f)
					{
						Hide3dTouchFire();
					}
				}
				if (!isShow3dTouchFire && timeShowFire > 3f)
				{
					isShow3dTouchFire = true;
					HintController.instance.HideHintByName("press_fire");
					touch3dPressFire.SetActive(true);
				}
			}
			objA = killZombiesOverlay;
		}
		foreach (GameObject item in _overlays.Where((GameObject o) => null != o))
		{
			item.SetActive(object.ReferenceEquals(objA, item));
		}
		bool flag = PlayerPrefs.GetInt(Defs.SwitchingWeaponsSwipeRegimSN, 0) == 1;
		if (swipeToChangeWeaponOverlay != null)
		{
			swipeToChangeWeaponOverlay.SetActive(stepTraining == TrainingState.TapToSelectWeapon && flag);
		}
		if (tapToChangeWeaponOverlay != null)
		{
			tapToChangeWeaponOverlay.SetActive(stepTraining == TrainingState.TapToSelectWeapon && !flag);
		}
		if (shopArrowOverlay != null)
		{
			shopArrowOverlay.SetActive(stepTraining == TrainingState.EnterTheShop);
		}
		if (throwGrenadeArrowOverlay != null)
		{
			throwGrenadeArrowOverlay.SetActive(stepTraining == stepTrainingList["TapToThrowGrenade"]);
		}
		if (joystickFingerOverlay != null)
		{
			joystickFingerOverlay.SetActive(stepTraining == stepTrainingList["TapToMove"]);
		}
		if (joystickShadowOverlay != null)
		{
			joystickShadowOverlay.SetActive(stepTraining == stepTrainingList["TapToMove"]);
		}
		if (touchpadOverlay != null)
		{
			touchpadOverlay.SetActive(stepTraining == TrainingState.SwipeToRotate);
		}
		if (touchpadFingerOverlay != null)
		{
			touchpadFingerOverlay.SetActive(stepTraining == TrainingState.SwipeToRotate);
		}
		if (swipeWeaponFingerOverlay != null)
		{
			swipeWeaponFingerOverlay.SetActive(stepTraining == TrainingState.TapToSelectWeapon && flag);
		}
		if (tapWeaponArrowOverlay != null)
		{
			tapWeaponArrowOverlay.SetActive(stepTraining == TrainingState.TapToSelectWeapon && !flag);
		}
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.CampaignContainer.SetActive(stepTraining == TrainingState.KillZombie);
			InGameGUI.sharedInGameGUI.leftAnchor.SetActive(false);
			InGameGUI.sharedInGameGUI.rightAnchor.SetActive(false);
		}
	}

	private void OnLevelWasLoaded(int unused)
	{
		if (SceneManager.GetActiveScene().name == Defs.TrainingSceneName)
		{
			GC.Collect();
			weapon = BonusCreator._CreateBonus(WeaponManager.MP5WN, new Vector3(0f, -10000f, 0f));
		}
	}
}

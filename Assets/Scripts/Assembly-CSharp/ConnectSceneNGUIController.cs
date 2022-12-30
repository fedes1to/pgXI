using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using FyberPlugin;
using Rilisoft;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectSceneNGUIController : MonoBehaviour
{
	public enum PlatformConnect
	{
		ios = 1,
		android,
		custom
	}

	public enum RegimGame
	{
		Deathmatch,
		TimeBattle,
		TeamFight,
		DeadlyGames,
		FlagCapture,
		CapturePoints,
		InFriendWindow,
		InClanWindow,
		Duel
	}

	public struct infoServer
	{
		public string ipAddress;

		public int port;

		public string name;

		public string map;

		public int playerLimit;

		public int connectedPlayers;

		public string coments;
	}

	public struct infoClient
	{
		public string ipAddress;

		public string name;

		public string coments;
	}

	public enum ABTestParams
	{
		Old = 1,
		Rating,
		Buff
	}

	public const string PendingInterstitialKey = "PendingInterstitial";

	public static PlatformConnect myPlatformConnect = PlatformConnect.ios;

	private string rulesDeadmatch;

	private string rulesDater;

	private string rulesTeamFight;

	private string rulesTimeBattle;

	private string rulesDeadlyGames;

	private string rulesFlagCapture;

	private string rulesCapturePoint;

	private string rulesDuel;

	public GameObject armoryButton;

	public int myLevelGame;

	public UILabel rulesLabel;

	public static int gameTier = 1;

	public static readonly IDictionary<string, string> gameModesLocalizeKey = new Dictionary<string, string>
	{
		{
			0.ToString(),
			"Key_0104"
		},
		{
			1.ToString(),
			"Key_0135"
		},
		{
			2.ToString(),
			"Key_0130"
		},
		{
			3.ToString(),
			"Key_0121"
		},
		{
			4.ToString(),
			"Key_0113"
		},
		{
			5.ToString(),
			"Key_1263"
		},
		{
			6.ToString(),
			"Key_1465"
		},
		{
			7.ToString(),
			"Key_1466"
		},
		{
			8.ToString(),
			"Key_2428"
		}
	};

	public UITable customButtonsGrid;

	public List<infoServer> servers = new List<infoServer>();

	private float posNumberOffPlayersX = -139f;

	private string goMapName;

	public SetHeadLabelText headCustomPanel;

	public static TypeModeGame curSelectMode;

	private Dictionary<string, Texture> mapPreview = new Dictionary<string, Texture>();

	public UILabel priceRegimLabel;

	public UILabel priceMapLabel;

	public UILabel priceMapLabelInCreate;

	public GameObject mapPreviewTexture;

	public GameObject grid;

	public MyCenterOnChild centerScript;

	public Transform ScrollTransform;

	public Transform selectMapPanelTransform;

	public MapPreviewController selectMap;

	public float widthCell;

	public int countMap;

	public UIButton createRoomUIBtn;

	public UISprite fonMapPreview;

	public UIPanel mapPreviewPanel;

	public GameObject mainPanel;

	public GameObject localBtn;

	public GameObject customBtn;

	public GameObject randomBtn;

	public GameObject goBtn;

	public GameObject backBtn;

	public GameObject unlockBtn;

	public GameObject unlockMapBtnInCreate;

	public GameObject unlockMapBtn;

	public GameObject cancelFromConnectToPhotonBtn;

	public GameObject connectToPhotonPanel;

	public GameObject failInternetLabel;

	public GameObject customPanel;

	public GameObject gameInfoItemPrefab;

	public GameObject loadingMapPanel;

	public GameObject searchPanel;

	public GameObject clearBtn;

	public GameObject searchBtn;

	public GameObject showSearchPanelBtn;

	public GameObject selectMapPanel;

	public GameObject createPanel;

	public GameObject goToCreateRoomBtn;

	public GameObject createRoomBtn;

	public GameObject clearInSetPasswordBtn;

	public GameObject okInsetPasswordBtn;

	public GameObject setPasswordPanel;

	public GameObject enterPasswordPanel;

	public GameObject joinRoomFromEnterPasswordBtn;

	public GameObject connectToWiFIInCreateLabel;

	public GameObject connectToWiFIInCustomLabel;

	public Transform scrollViewSelectMapTransform;

	public PlusMinusController numberOfPlayer;

	public PlusMinusController killToWin;

	public TeamNumberOfPlayer teamCountPlayer;

	public UIGrid gridGames;

	public UISprite fonGames;

	public UIInput searchInput;

	public UIInput nameServerInput;

	public UIInput setPasswordInput;

	public UIInput enterPasswordInput;

	public Transform gridGamesTransform;

	public UITexture loadingToDraw;

	public UILabel conditionLabel;

	private static RegimGame _regim = RegimGame.Deathmatch;

	public static bool isReturnFromGame;

	public int nRegim;

	private bool isSetUseMap;

	public string gameNameFilter;

	public List<GameObject> gamesInfo = new List<GameObject>();

	public DisableObjectFromTimer gameIsfullLabel;

	public DisableObjectFromTimer incorrectPasswordLabel;

	public DisableObjectFromTimer serverIsNotAvalible;

	public DisableObjectFromTimer accountBlockedLabel;

	public DisableObjectFromTimer nameAlreadyUsedLabel;

	private float timerShowBan = -1f;

	private bool isConnectingToPhoton;

	private bool isCancelConnectingToPhoton;

	private int pressButton;

	private List<RoomInfo> filteredRoomList = new List<RoomInfo>();

	private int countNoteCaptureDeadmatch = 5;

	private int countNoteCaptureCOOP = 5;

	private int countNoteCaptureHunger = 5;

	private int countNoteCaptureFlag = 5;

	private int countNoteCaptureCompany = 5;

	public static ConnectSceneNGUIController sharedController;

	private string password = string.Empty;

	public LANBroadcastService lanScan;

	private RoomInfo joinRoomInfoFromCustom;

	private bool firstConnectToPhoton;

	private bool isGoInPhotonGame;

	private bool isMainPanelActiv = true;

	public GameObject ChooseMapLabelSmall;

	private AdvertisementController _advertisementController;

	public CategoryButtonsController categoryButtonsController;

	public BtnCategory deathmatchToggle;

	public BtnCategory teamFightToogle;

	public BtnCategory timeBattleToogle;

	public BtnCategory deadlyGamesToogle;

	public BtnCategory flagCaptureToogle;

	public BtnCategory capturePointsToogle;

	public BtnCategory duelToggle;

	public bool isStartShowAdvert;

	private Action actAfterConnectToPhoton;

	private GameInfo[] roomFields;

	public UIWrapContent wrapGames;

	public UIScrollView scrollGames;

	public static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>> mapStatistics = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>();

	public static string selectedMap = string.Empty;

	public static bool directedFromQuests = false;

	public GameObject modeAnimObj;

	public GameObject finger;

	public BtnCategory[] modeButtonByLevel;

	public UILabel[] modeUnlockLabelByLevel;

	public BtnCategory modeButtonDuel;

	public UILabel modeDuelRatingNeedLabel;

	public GameObject modeDuelRatingNeed;

	private bool fingerStopped;

	private bool animationStarted;

	private bool _stopFingerAnimation;

	private bool loadReplaceAdmobPerelivRunning;

	private bool loadAdmobRunning;

	private int _countOfLoopsRequestAdThisTime;

	private float _lastTimeInterstitialShown;

	public static bool NeedShowReviewInConnectScene = false;

	public static readonly string mapProperty = "C0";

	public static readonly string passwordProperty = "C1";

	public static readonly string platformProperty = "C2";

	public static readonly string endingProperty = "C3";

	public static readonly string maxKillProperty = "C4";

	public static readonly string ABTestProperty = "C5";

	public static readonly string ABTestEnum = "C6";

	public static readonly string roomStatusProperty = "Closed";

	private bool abTestConnect;

	private int joinNewRoundTries;

	private int tryJoinRoundMap;

	private Vector3 startPosNameServerNameInput = Vector3.zero;

	private IDisposable _someWindowSubscription;

	private int _tempMinValue = 3;

	private int _tempMaxValue = 7;

	private int _tempStep = 2;

	private int daterStep = 5;

	private int daterMinValue = 5;

	private int daterMaxValue = 10;

	private IDisposable _backSubscription;

	private int countNote = 1;

	private bool isFirstUpdateLocalServerList;

	private string _logCache = string.Empty;

	private float startPosX;

	private int maxcount = 1;

	private bool isFirstGamesReposition;

	private int countColumn = 3;

	private float _widthCell = 282f;

	private float _heightCell = 1f;

	private float _scale = 1f;

	private float borderWidth = 10f;

	private LoadingNGUIController _loadingNGUIController;

	private LANBroadcastService.ReceivedMessage[] _copy;

	private Vector3 posSelectMapPanelInMainMenu = Vector3.up * 10000f;

	public static RegimGame regim
	{
		get
		{
			return _regim;
		}
		set
		{
			_regim = value;
			UpdateUseMasMaps();
		}
	}

	public static bool isTeamRegim
	{
		get
		{
			return Defs.isMulti && (regim == RegimGame.TeamFight || regim == RegimGame.FlagCapture || regim == RegimGame.CapturePoints);
		}
	}

	internal static bool InterstitialRequest { get; set; }

	internal static bool ReplaceAdmobWithPerelivRequest { get; set; }

	public static string MainLoadingTexture()
	{
		return (!Device.isRetinaAndStrong) ? "main_loading" : "main_loading_Hi";
	}

	public static void GoToClans()
	{
		LoadConnectScene.textureToShow = null;
		LoadConnectScene.sceneToLoad = "Clans";
		LoadConnectScene.noteToShow = null;
		SceneManager.LoadScene(Defs.PromSceneName);
	}

	public static void GoToFriends()
	{
		FriendsController friendsController = FriendsController.sharedController;
		if (friendsController != null)
		{
			friendsController.GetFriendsData(false);
		}
		MainMenuController.friendsOnStart = true;
		LoadConnectScene.textureToShow = null;
		LoadConnectScene.sceneToLoad = Defs.MainMenuScene;
		LoadConnectScene.noteToShow = null;
		SceneManager.LoadScene(Defs.PromSceneName);
		Defs.isDaterRegim = false;
	}

	public static void Local()
	{
		PhotonNetwork.isMessageQueueRunning = true;
		PhotonNetwork.Disconnect();
		if (Defs.isGameFromFriends)
		{
			GoToFriends();
			return;
		}
		if (Defs.isGameFromClans)
		{
			GoToClans();
			return;
		}
		LoadConnectScene.textureToShow = null;
		if (!Defs.isDaterRegim)
		{
			LoadConnectScene.sceneToLoad = "ConnectScene";
		}
		else
		{
			LoadConnectScene.sceneToLoad = "ConnectSceneSandbox";
		}
		LoadConnectScene.noteToShow = null;
		SceneManager.LoadScene(Defs.PromSceneName);
	}

	public static void GoToProfile()
	{
		PlayerPrefs.SetInt(Defs.SkinEditorMode, 1);
		GlobalGameController.EditingLogo = 0;
		GlobalGameController.EditingCape = 0;
		SceneManager.LoadScene("SkinEditor");
	}

	public void StopFingerAnim()
	{
		if (finger != null && finger.activeSelf)
		{
			fingerStopped = true;
			finger.SetActive(false);
			UIScrollView component = scrollViewSelectMapTransform.GetComponent<UIScrollView>();
			component.onDragStarted = (UIScrollView.OnDragNotification)Delegate.Remove(component.onDragStarted, new UIScrollView.OnDragNotification(StopFingerAnim));
		}
	}

	private void OnEnableWhenAnimate()
	{
		if (animationStarted)
		{
			StopFingerAnim();
			modeAnimObj.SetActive(false);
			fingerStopped = false;
			StartCoroutine(AnimateModeOpen());
		}
	}

	private IEnumerator AnimateModeOpen()
	{
		modeAnimObj.GetComponent<AudioSource>().enabled = Defs.isSoundFX;
		animationStarted = true;
		if (!TrainingController.TrainingCompleted)
		{
			localBtn.GetComponent<UIButton>().isEnabled = false;
			randomBtn.GetComponent<UIButton>().isEnabled = false;
			customBtn.GetComponent<UIButton>().isEnabled = false;
			goBtn.GetComponent<UIButton>().isEnabled = false;
		}
		int storagedStageDuel = Storager.getInt("ModeUnlockDuel", false);
		int currentStateDuel = 1; //((RatingSystem.instance.currentRating >= 1200) ? storagedStageDuel : 0);
		modeDuelRatingNeedLabel.text = 1200.ToString();
		modeDuelRatingNeed.SetActive(false); // RatingSystem.instance.currentRating < 1200
		modeButtonDuel.isEnable = currentStateDuel == 1;
		int storagedStage = Storager.getInt("ModeUnlockStage", false);
		if (Storager.getInt("TrainingCompleted_4_4_Sett", false) == 1)
		{
			storagedStage = modeButtonByLevel.Length;
		}
		int currentStage = Mathf.Clamp(storagedStage, 0, modeButtonByLevel.Length);
		for (int j = 0; j < modeButtonByLevel.Length; j++)
		{
			modeButtonByLevel[j].isEnable = j < currentStage;
		}
		int currentLevel = ((!(ExperienceController.sharedController != null)) ? 31 : ExperienceController.sharedController.currentLevel);
		if (currentLevel >= 4)
		{
			currentLevel = modeButtonByLevel.Length;
		}
		if (modeUnlockLabelByLevel != null)
		{
			for (int i = 0; i < modeUnlockLabelByLevel.Length; i++)
			{
				modeUnlockLabelByLevel[i].gameObject.SetActive(i > Mathf.Max(currentStage, currentLevel) - 2);
				modeUnlockLabelByLevel[i].text = string.Format(LocalizationStore.Get("Key_1923"), Mathf.Min(i + 2, 4));
			}
		}
		if (currentStage < Mathf.Min(currentLevel, modeButtonByLevel.Length))
		{
			BannerWindowController.SharedController.AddBannersTimeout(20.1f);
		}
		if (currentStage == 0 && !TrainingController.TrainingCompleted)
		{
			UIScrollView component = scrollViewSelectMapTransform.GetComponent<UIScrollView>();
			component.onDragStarted = (UIScrollView.OnDragNotification)Delegate.Combine(component.onDragStarted, new UIScrollView.OnDragNotification(StopFingerAnim));
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Connect_Scene);
		}
		yield return new WaitForSeconds(0.5f);
		while (currentStage < Mathf.Min(currentLevel, modeButtonByLevel.Length))
		{
			BannerWindowController.SharedController.AddBannersTimeout(20.1f);
			if (currentStage == 1)
			{
				BannerWindowController.firstScreen = true;
				BannerWindowController.SharedController.ClearBannerStates();
			}
			BtnCategory currentMode = modeButtonByLevel[currentStage];
			if (currentStage != 0)
			{
				modeAnimObj.transform.SetParent(categoryButtonsController.transform.parent);
				modeAnimObj.transform.position = currentMode.transform.position;
				modeAnimObj.transform.localScale = currentMode.transform.localScale;
				modeAnimObj.SetActive(true);
				yield return new WaitForSeconds(0.1f);
				modeButtonByLevel[currentStage].isEnable = true;
				yield return new WaitForSeconds(1.4f);
				modeAnimObj.SetActive(false);
			}
			if (currentStage == 0 && !TrainingController.TrainingCompleted)
			{
				yield return FingerAnimationCoroutine();
			}
			if (currentStage == 1)
			{
				HintController.instance.ShowHintByName("deathmatch", 0f);
				HintController.instance.ShowHintByName("gobattletimeout", 0f);
			}
			currentStage++;
			Storager.setInt("ModeUnlockStage", currentStage, false);
		}
		if (storagedStage != currentStage)
		{
			Storager.setInt("ModeUnlockStage", currentStage, false);
		}
		if (currentStateDuel == 0) // && RatingSystem.instance.currentRating >= 1200)
		{
			modeAnimObj.transform.SetParent(categoryButtonsController.transform.parent);
			modeAnimObj.transform.position = modeButtonDuel.transform.position;
			modeAnimObj.SetActive(true);
			yield return new WaitForSeconds(0.1f);
			modeButtonDuel.isEnable = true;
			yield return new WaitForSeconds(1.4f);
			modeAnimObj.SetActive(false);
			currentStateDuel = 1;
		}
		if (storagedStageDuel != currentStateDuel)
		{
			Storager.setInt("ModeUnlockDuel", currentStateDuel, false);
		}
		if (!TrainingController.TrainingCompleted)
		{
			goBtn.GetComponent<UIButton>().isEnabled = true;
			HintController.instance.ShowHintByName("gobattle", 0f);
		}
		animationStarted = false;
	}

	public void StopFingerAnimation()
	{
		finger.SetActive(false);
		_stopFingerAnimation = true;
	}

	private IEnumerator FingerAnimationCoroutine()
	{
		finger.SetActive(true);
		string fromName = grid.transform.GetChild(1).GetComponent<MapPreviewController>().sceneMapName;
		string toName = grid.transform.GetChild(2).GetComponent<MapPreviewController>().sceneMapName;
		yield return finger.MoveOverTime(finger.transform.position, grid.transform.GetChild(1).transform.position, 1f);
		if (_stopFingerAnimation)
		{
			yield break;
		}
		Animator fingerAnimator = finger.GetComponentInChildren<Animator>(true);
		fingerAnimator.SetTrigger("touch");
		yield return new WaitForSeconds(0.8f);
		if (_stopFingerAnimation)
		{
			yield break;
		}
		SelectMap(fromName);
		yield return new WaitForSeconds(1f);
		if (!_stopFingerAnimation)
		{
			yield return finger.MoveOverTime(grid.transform.GetChild(1).transform.position, grid.transform.GetChild(2).transform.position, 1f);
			fingerAnimator.SetTrigger("touch");
			yield return new WaitForSeconds(0.8f);
			if (!_stopFingerAnimation)
			{
				SelectMap(toName);
				yield return new WaitForSeconds(1f);
				finger.gameObject.SetActive(false);
			}
		}
	}

	private void Start()
	{
		if (FriendsController.sharedController != null)
		{
			FriendsController.sharedController.SendOurDataInConnectScene();
		}
		Defs.isGameFromFriends = false;
		gameInfoItemPrefab.SetActive(false);
		mapPreviewTexture.SetActive(false);
		startPosNameServerNameInput = nameServerInput.transform.localPosition;
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted && WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.SaveWeaponAsLastUsed(0);
		}
		if (FriendsController.sharedController != null)
		{
			FriendsController.sharedController.profileInfo.Clear();
		}
		Defs.isDaterRegim = SceneLoader.ActiveSceneName.Equals("ConnectSceneSandbox");
		GlobalGameController.CountKills = 0;
		GlobalGameController.Score = 0;
		WeaponManager.RefreshExpControllers();
		rulesDeadmatch = LocalizationStore.Key_0550;
		rulesTeamFight = LocalizationStore.Key_0551;
		rulesTimeBattle = LocalizationStore.Key_0552;
		rulesDeadlyGames = LocalizationStore.Key_0553;
		rulesFlagCapture = LocalizationStore.Key_0554;
		rulesCapturePoint = LocalizationStore.Get("Key_1368");
		rulesDater = LocalizationStore.Get("Key_1538");
		rulesDuel = LocalizationStore.Get("Key_2406");
		sharedController = this;
		myLevelGame = ((!(ExperienceController.sharedController != null) || ExperienceController.sharedController.currentLevel > 2) ? ((ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel <= 5) ? 1 : 2) : 0);
		mainPanel.SetActive(false);
		selectMapPanel.SetActive(false);
		createPanel.SetActive(false);
		customPanel.SetActive(false);
		searchPanel.SetActive(false);
		setPasswordPanel.SetActive(false);
		enterPasswordPanel.SetActive(false);
		StartSearchLocalServers();
		PlayerPrefs.SetString("TypeGame", "client");
		gameIsfullLabel.gameObject.SetActive(false);
		accountBlockedLabel.gameObject.SetActive(false);
		serverIsNotAvalible.gameObject.SetActive(false);
		nameAlreadyUsedLabel.gameObject.SetActive(false);
		incorrectPasswordLabel.gameObject.SetActive(false);
		unlockMapBtn.SetActive(false);
		unlockMapBtnInCreate.SetActive(false);
		unlockBtn.SetActive(false);
		string path = MainLoadingTexture();
		loadingToDraw.mainTexture = Resources.Load<Texture>(path);
		loadingMapPanel.SetActive(true);
		connectToPhotonPanel.SetActive(false);
		if (PhotonNetwork.connectionState == ConnectionState.Connected)
		{
			firstConnectToPhoton = true;
		}
		SetPosSelectMapPanelInMainMenu();
		regim = ((!TrainingController.TrainingCompleted) ? RegimGame.TeamFight : ((!Defs.isDaterRegim) ? ((RegimGame)PlayerPrefs.GetInt("RegimMulty", 2)) : RegimGame.Deathmatch));
		directedFromQuests = false;
		if (false) //regim == RegimGame.Duel && RatingSystem.instance.currentRating < 1200)
		{
			regim = RegimGame.TeamFight;
		}
		SceneInfo sceneInfo = ((!(SceneInfoController.instance != null)) ? null : SceneInfoController.instance.GetInfoScene(selectedMap));
		if (sceneInfo != null)
		{
			if (sceneInfo.IsAvaliableForMode(TypeModeGame.TeamFight))
			{
				regim = RegimGame.TeamFight;
			}
			else if (sceneInfo.IsAvaliableForMode(TypeModeGame.Deathmatch))
			{
				regim = RegimGame.Deathmatch;
			}
			else if (sceneInfo.IsAvaliableForMode(TypeModeGame.FlagCapture))
			{
				regim = RegimGame.FlagCapture;
			}
			else if (sceneInfo.IsAvaliableForMode(TypeModeGame.CapturePoints))
			{
				regim = RegimGame.CapturePoints;
			}
			else if (sceneInfo.IsAvaliableForMode(TypeModeGame.DeadlyGames))
			{
				regim = RegimGame.DeadlyGames;
			}
			else if (sceneInfo.IsAvaliableForMode(TypeModeGame.TimeBattle))
			{
				regim = RegimGame.TimeBattle;
			}
		}
		if (!Defs.isDaterRegim)
		{
			teamFightToogle.wasPressed = false;
			if (regim == RegimGame.Deathmatch)
			{
				categoryButtonsController.BtnClicked(deathmatchToggle.btnName);
			}
			if (regim == RegimGame.TimeBattle)
			{
				categoryButtonsController.BtnClicked(timeBattleToogle.btnName);
			}
			if (regim == RegimGame.DeadlyGames)
			{
				categoryButtonsController.BtnClicked(deadlyGamesToogle.btnName);
			}
			if (regim == RegimGame.FlagCapture)
			{
				categoryButtonsController.BtnClicked(flagCaptureToogle.btnName);
			}
			if (regim == RegimGame.Duel)
			{
				categoryButtonsController.BtnClicked(duelToggle.btnName);
			}
			if (regim == RegimGame.TeamFight)
			{
				categoryButtonsController.BtnClicked(teamFightToogle.btnName);
			}
			if (regim == RegimGame.CapturePoints)
			{
				categoryButtonsController.BtnClicked(capturePointsToogle.btnName);
			}
			deathmatchToggle.Clicked += SetRegimDeathmatch;
			timeBattleToogle.Clicked += SetRegimTimeBattle;
			teamFightToogle.Clicked += SetRegimTeamFight;
			deadlyGamesToogle.Clicked += SetRegimDeadleGames;
			flagCaptureToogle.Clicked += SetRegimFlagCapture;
			capturePointsToogle.Clicked += SetRegimCapturePoints;
			duelToggle.Clicked += SetRegimDuel;
		}
		StartCoroutine(LoadMapPreview());
		if (localBtn != null)
		{
			ButtonHandler component = localBtn.GetComponent<ButtonHandler>();
			if (component != null)
			{
				component.Clicked += HandleLocalBtnClicked;
			}
		}
		if (customBtn != null)
		{
			ButtonHandler component2 = customBtn.GetComponent<ButtonHandler>();
			if (component2 != null)
			{
				component2.Clicked += HandleCustomBtnClicked;
			}
		}
		if (randomBtn != null)
		{
			ButtonHandler component3 = randomBtn.GetComponent<ButtonHandler>();
			if (component3 != null)
			{
				component3.Clicked += HandleRandomBtnClicked;
			}
		}
		if (goBtn != null)
		{
			ButtonHandler component4 = goBtn.GetComponent<ButtonHandler>();
			if (component4 != null)
			{
				component4.Clicked += HandleGoBtnClicked;
			}
		}
		if (backBtn != null)
		{
			ButtonHandler component5 = backBtn.GetComponent<ButtonHandler>();
			if (component5 != null)
			{
				component5.Clicked += HandleBackBtnClicked;
			}
		}
		if (unlockBtn != null)
		{
			ButtonHandler component6 = unlockBtn.GetComponent<ButtonHandler>();
			if (component6 != null)
			{
				component6.Clicked += HandleUnlockBtnClicked;
			}
		}
		if (unlockMapBtn != null)
		{
			ButtonHandler component7 = unlockMapBtn.GetComponent<ButtonHandler>();
			if (component7 != null)
			{
				component7.Clicked += HandleUnlockMapBtnClicked;
			}
		}
		if (unlockMapBtnInCreate != null)
		{
			ButtonHandler component8 = unlockMapBtnInCreate.GetComponent<ButtonHandler>();
			if (component8 != null)
			{
				component8.Clicked += HandleUnlockMapBtnClicked;
			}
		}
		if (cancelFromConnectToPhotonBtn != null)
		{
			ButtonHandler component9 = cancelFromConnectToPhotonBtn.GetComponent<ButtonHandler>();
			if (component9 != null)
			{
				component9.Clicked += HandleCancelFromConnectToPhotonBtnClicked;
			}
		}
		if (clearBtn != null)
		{
			ButtonHandler component10 = clearBtn.GetComponent<ButtonHandler>();
			if (component10 != null)
			{
				component10.Clicked += HandleClearBtnClicked;
			}
		}
		if (searchBtn != null)
		{
			ButtonHandler component11 = searchBtn.GetComponent<ButtonHandler>();
			if (component11 != null)
			{
				component11.Clicked += HandleSearchBtnClicked;
			}
		}
		if (showSearchPanelBtn != null)
		{
			ButtonHandler component12 = showSearchPanelBtn.GetComponent<ButtonHandler>();
			if (component12 != null)
			{
				component12.Clicked += HandleShowSearchPanelBtnClicked;
			}
		}
		if (goToCreateRoomBtn != null)
		{
			ButtonHandler component13 = goToCreateRoomBtn.GetComponent<ButtonHandler>();
			if (component13 != null)
			{
				component13.Clicked += HandleGoToCreateRoomBtnClicked;
			}
		}
		if (createRoomBtn != null)
		{
			createRoomUIBtn = createRoomBtn.GetComponent<UIButton>();
			ButtonHandler component14 = createRoomBtn.GetComponent<ButtonHandler>();
			if (component14 != null)
			{
				component14.Clicked += HandleCreateRoomBtnClicked;
			}
		}
		if (clearInSetPasswordBtn != null)
		{
			ButtonHandler component15 = clearInSetPasswordBtn.GetComponent<ButtonHandler>();
			if (component15 != null)
			{
				component15.Clicked += HandleClearInSetPasswordBtnClicked;
			}
		}
		if (okInsetPasswordBtn != null)
		{
			ButtonHandler component16 = okInsetPasswordBtn.GetComponent<ButtonHandler>();
			if (component16 != null)
			{
				component16.Clicked += delegate
				{
					OnPaswordSelected();
				};
			}
		}
		if (joinRoomFromEnterPasswordBtn != null)
		{
			ButtonHandler component17 = joinRoomFromEnterPasswordBtn.GetComponent<ButtonHandler>();
			if (component17 != null)
			{
				component17.Clicked += HandleJoinRoomFromEnterPasswordBtnClicked;
			}
		}
		InitializeBannerWindow();
		InterstitialManager.Instance.ResetAdProvider();
		string text = string.Empty;
		if (NeedShowReviewInConnectScene)
		{
			text = "NeedShowReviewInConnectScene";
		}
		else if (BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64)
		{
			string text2 = ((!ReplaceAdmobWithPerelivRequest) ? "Fake interstitial request not performed." : GetReasonToDismissFakeInterstitial());
			if (string.IsNullOrEmpty(text2))
			{
				ReplaceAdmobWithPerelivRequest = false;
				StartCoroutine(WaitLoadingAndShowReplaceAdmobPereliv("Connect Scene", false));
				text = "ReplaceAdmobWithPereliv";
			}
			else
			{
				string format = ((!Application.isEditor) ? "Dismissing fake interstitial. {0}" : "<color=magenta>Dismissing fake interstitial. {0}</color>");
				Debug.LogFormat(format, text2);
				if (!InterstitialRequest)
				{
					text = "InterstitialRequest == false";
				}
				else
				{
					text = GetReasonToDismissInterstitialConnectScene();
					if (string.IsNullOrEmpty(text))
					{
						isStartShowAdvert = true;
						StartCoroutine(WaitLoadingAndShowInterstitialCoroutine("Connect Scene", false));
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			string format2 = ((!Application.isEditor) ? "Dismissing interstitial. {0}" : "<color=magenta>Dismissing interstitial. {0}</color>");
			Debug.LogFormat(format2, text);
		}
		enterPasswordInput.onSubmit.Add(new EventDelegate(EnterPassInputSubmit));
	}

	private IEnumerator OnApplicationPause(bool pausing)
	{
		if (pausing)
		{
			lanScan.StopBroadCasting();
			yield break;
		}
		yield return new WaitForSeconds(1f);
		StartSearchLocalServers();
		InterstitialManager.Instance.ResetAdProvider();
		if (MobileAdManager.Instance.SuppressShowOnReturnFromPause)
		{
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = false;
			yield break;
		}
		string reasonToDismissFakeInterstitial = GetReasonToDismissFakeInterstitial();
		if (string.IsNullOrEmpty(reasonToDismissFakeInterstitial))
		{
			ReplaceAdmobPerelivController.IncreaseTimesCounter();
			if (!loadAdmobRunning)
			{
				StartCoroutine(WaitLoadingAndShowReplaceAdmobPereliv("On return from pause to Connect Scene"));
			}
		}
		else
		{
			string format = ((!Application.isEditor) ? "Dismissing fake interstitial. {0}" : "<color=magenta>Dismissing fake interstitial. {0}</color>");
			Debug.LogFormat(format, reasonToDismissFakeInterstitial);
		}
	}

	private IEnumerator WaitLoadingAndShowReplaceAdmobPereliv(string context, bool loadData = true)
	{
		if (loadReplaceAdmobPerelivRunning)
		{
			yield break;
		}
		try
		{
			loadReplaceAdmobPerelivRunning = true;
			if (loadData && !ReplaceAdmobPerelivController.sharedController.DataLoading && !ReplaceAdmobPerelivController.sharedController.DataLoaded)
			{
				ReplaceAdmobPerelivController.sharedController.LoadPerelivData();
			}
			while (ReplaceAdmobPerelivController.sharedController == null || !ReplaceAdmobPerelivController.sharedController.DataLoaded)
			{
				if (!ReplaceAdmobPerelivController.sharedController.DataLoading)
				{
					loadReplaceAdmobPerelivRunning = false;
					yield break;
				}
				yield return null;
			}
			if (mainPanel != null)
			{
				while (!mainPanel.activeInHierarchy)
				{
					yield return null;
				}
				yield return new WaitForSeconds(0.5f);
			}
			ReplaceAdmobPerelivController.TryShowPereliv(context);
			ReplaceAdmobPerelivController.sharedController.DestroyImage();
		}
		finally
		{
			loadReplaceAdmobPerelivRunning = false;
		}
	}

	private IEnumerator WaitLoadingAndShowInterstitialCoroutine(string context, bool loadData = true)
	{
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("Starting WaitLoadingAndShowInterstitialCoroutine()    " + InterstitialManager.Instance.Provider);
		}
		if (loadAdmobRunning)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Quitting WaitLoadingAndShowInterstitialCoroutine() because loadAdmobRunning==true");
			}
			yield break;
		}
		loadAdmobRunning = true;
		try
		{
			float loadAttemptTime = Time.realtimeSinceStartup;
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("FyberFacade.Instance.Requests.Count: " + FyberFacade.Instance.Requests.Count);
			}
			if (FyberFacade.Instance.Requests.Count == 0)
			{
				Task<Ad> r2 = FyberFacade.Instance.RequestImageInterstitial("WaitLoadingAndShowInterstitialCoroutine(), requests count: 0");
				FyberFacade.Instance.Requests.AddLast(r2);
			}
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Waiting either at least one loading request completed successfully, or all failed...");
			}
			while (true)
			{
				if (FyberFacade.Instance.Requests.Any((Task<Ad> r) => r.IsCompleted && !r.IsFaulted))
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("Found successfully completed request among {0}", FyberFacade.Instance.Requests.Count);
					}
					break;
				}
				if (FyberFacade.Instance.Requests.All((Task<Ad> r) => r.IsCompleted))
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.Log("All requests are completed.");
					}
					break;
				}
				if (Time.realtimeSinceStartup - loadAttemptTime > 5.2f)
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.Log("Loading timed out.");
					}
					break;
				}
				yield return null;
			}
			List<Task<Ad>> completedRequests = FyberFacade.Instance.Requests.Where((Task<Ad> r) => r.IsCompleted).ToList();
			List<Task<Ad>> noOffersRequests = completedRequests.Where((Task<Ad> r) => r.IsFaulted && r.Exception.InnerException is AdNotAwailableException).ToList();
			if (noOffersRequests.Count > 0)
			{
				if (Defs.IsDeveloperBuild)
				{
					Debug.Log("Removing not filled requests: " + noOffersRequests.Count);
				}
				foreach (Task<Ad> noOffersRequest in noOffersRequests)
				{
					FyberFacade.Instance.Requests.Remove(noOffersRequest);
					completedRequests = null;
				}
			}
			if (completedRequests == null)
			{
				completedRequests = FyberFacade.Instance.Requests.Where((Task<Ad> r) => r.IsCompleted).ToList();
			}
			List<Task<Ad>> errorRequests = completedRequests.Where((Task<Ad> r) => r.IsFaulted && r.Exception.InnerException is AdRequestException).ToList();
			if (errorRequests.Count > 0)
			{
				if (Defs.IsDeveloperBuild)
				{
					Debug.Log("Removing failed requests: " + errorRequests.Count);
				}
				foreach (Task<Ad> errorRequest in errorRequests)
				{
					FyberFacade.Instance.Requests.Remove(errorRequest);
					completedRequests = null;
				}
			}
			if (mainPanel != null)
			{
				while (!mainPanel.activeInHierarchy)
				{
					yield return null;
				}
				yield return new WaitForSeconds(0.5f);
			}
			if (!PhotonNetwork.inRoom)
			{
				Dictionary<string, string> attributes = new Dictionary<string, string>
				{
					{ "af_content_type", "Interstitial" },
					{ "af_content_id", "Interstitial (ConnectScene)" }
				};
				AnalyticsFacade.SendCustomEventToAppsFlyer("af_content_view", attributes);
				MenuBackgroundMusic.sharedMusic.Stop();
				Task<AdResult> showTask = FyberFacade.Instance.ShowInterstitial(new Dictionary<string, string> { { "Context", "Connect Scene" } }, "WaitLoadingAndShowInterstitialCoroutine()");
				InterstitialCounter.Instance.IncrementRealInterstitialCount();
				Storager.setInt("PendingInterstitial", 8, false);
				showTask.ContinueWith(delegate(Task<AdResult> t)
				{
					MenuBackgroundMusic.sharedMusic.Start();
					Storager.setInt("PendingInterstitial", 0, false);
					isStartShowAdvert = false;
					if (t.IsFaulted)
					{
						Debug.LogWarningFormat("[Rilisoft] Showing interstitial failed: {0}", t.Exception.InnerException.Message);
					}
				});
			}
			_lastTimeInterstitialShown = Time.realtimeSinceStartup;
		}
		finally
		{
			loadAdmobRunning = false;
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Finishing WaitLoadingAndShowInterstitialCoroutine()    " + InterstitialManager.Instance.Provider);
			}
		}
	}

	private void InitializeBannerWindow()
	{
		_advertisementController = base.gameObject.GetComponent<AdvertisementController>();
		if (_advertisementController == null)
		{
			_advertisementController = base.gameObject.AddComponent<AdvertisementController>();
		}
		BannerWindowController.SharedController.advertiseController = _advertisementController;
	}

	private void SetUnLockedButton(UIToggle butToogle)
	{
		UIButton component = butToogle.gameObject.GetComponent<UIButton>();
		component.normalSprite = "yell_btn";
		component.hoverSprite = "yell_btn";
		component.pressedSprite = "green_btn_n";
		butToogle.transform.FindChild("LockedSprite").gameObject.SetActive(false);
		butToogle.transform.FindChild("Checkmark").GetComponent<UISprite>().spriteName = "green_btn";
	}

	private void SetRegimDeathmatch(object sender, EventArgs e)
	{
		HintController.instance.HideHintByName("deathmatch");
		if (regim != 0)
		{
			SetRegim(RegimGame.Deathmatch);
		}
	}

	private void SetRegimTeamFight(object sender, EventArgs e)
	{
		if (regim != RegimGame.TeamFight)
		{
			SetRegim(RegimGame.TeamFight);
		}
	}

	private void SetRegimTimeBattle(object sender, EventArgs e)
	{
		if (regim != RegimGame.TimeBattle)
		{
			SetRegim(RegimGame.TimeBattle);
		}
	}

	private void SetRegimDeadleGames(object sender, EventArgs e)
	{
		if (regim != RegimGame.DeadlyGames)
		{
			SetRegim(RegimGame.DeadlyGames);
		}
	}

	private void SetRegimFlagCapture(object sender, EventArgs e)
	{
		if (regim != RegimGame.FlagCapture)
		{
			SetRegim(RegimGame.FlagCapture);
		}
	}

	private void SetRegimCapturePoints(object sender, EventArgs e)
	{
		if (regim != RegimGame.CapturePoints)
		{
			SetRegim(RegimGame.CapturePoints);
		}
	}

	private void SetRegimDuel(object sender, EventArgs e)
	{
		if (regim != RegimGame.Duel)
		{
			SetRegim(RegimGame.Duel);
		}
	}

	private void HandleJoinRoomFromEnterPasswordBtnClicked(object sender, EventArgs e)
	{
		if (enterPasswordInput.value.Equals(joinRoomInfoFromCustom.customProperties[passwordProperty].ToString()))
		{
			JoinToRoomPhotonAfterCheck();
			return;
		}
		enterPasswordPanel.SetActive(false);
		if (ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.isShowRanks = true;
		}
		customPanel.SetActive(true);
		Invoke("UpdateFilteredRoomListInvoke", 0.03f);
	}

	private void HandleSetPasswordBtnClicked(object sender, EventArgs e)
	{
		createPanel.SetActive(false);
		selectMapPanel.SetActive(false);
		setPasswordInput.value = password;
		setPasswordPanel.SetActive(true);
	}

	private void HandleClearInSetPasswordBtnClicked(object sender, EventArgs e)
	{
		setPasswordInput.value = string.Empty;
	}

	private void OnPaswordSelected()
	{
		password = setPasswordInput.value;
		BackFromSetPasswordPanel();
	}

	private void BackFromSetPasswordPanel()
	{
		createPanel.SetActive(true);
		selectMapPanel.SetActive(true);
		setPasswordPanel.SetActive(false);
	}

	public static void CreateGameRoom(string roomName, int playerLimit, int mapIndex, int MaxKill, string _password, RegimGame gameMode)
	{
		int num = 11;
		string[] array = new string[num];
		array[0] = mapProperty;
		array[1] = passwordProperty;
		array[2] = platformProperty;
		array[3] = endingProperty;
		array[4] = maxKillProperty;
		array[5] = "TimeMatchEnd";
		array[6] = "tier";
		array[7] = ABTestProperty;
		array[8] = ABTestEnum;
		array[9] = "SpecialBonus";
		array[10] = roomStatusProperty;
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[mapProperty] = mapIndex;
		hashtable[passwordProperty] = _password;
		hashtable[platformProperty] = (int)((!string.IsNullOrEmpty(_password)) ? PlatformConnect.custom : myPlatformConnect);
		hashtable[endingProperty] = 0;
		hashtable[maxKillProperty] = MaxKill;
		hashtable["TimeMatchEnd"] = PhotonNetwork.time;
		hashtable["tier"] = ((ExpController.Instance != null) ? ExpController.Instance.OurTier : 0);
		if (ExpController.Instance.OurTier == 0)
		{
			hashtable[ABTestProperty] = (Defs.isABTestBalansCohortActual ? 1 : 0);
		}
		if (Defs.isActivABTestBuffSystem)
		{
			hashtable[ABTestEnum] = ((!ABTestController.useBuffSystem) ? 1 : 3);
		}
		hashtable["SpecialBonus"] = 0;
		hashtable[roomStatusProperty] = 0;
		PhotonCreateRoom(roomName, true, true, (playerLimit <= 10) ? playerLimit : 10, hashtable, array);
	}

	public static void PhotonCreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers, ExitGames.Client.Photon.Hashtable roomProps, string[] roomPropsInLobby)
	{
		PlayerPrefs.SetString("TypeGame", "server");
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.customRoomProperties = roomProps;
		roomOptions.customRoomPropertiesForLobby = roomPropsInLobby;
		RoomOptions roomOptions2 = roomOptions;
		roomOptions2.maxPlayers = (byte)maxPlayers;
		roomOptions2.isOpen = isOpen;
		roomOptions2.isVisible = isVisible;
		if (!Defs.useSqlLobby)
		{
			PhotonNetwork.CreateRoom(roomName, roomOptions2, TypedLobby.Default);
			return;
		}
		TypedLobby typedLobby = new TypedLobby("PixelGun3D", LobbyType.SqlLobby);
		PhotonNetwork.CreateRoom(roomName, roomOptions2, typedLobby);
	}

	public static void JoinRandomGameRoom(int mapIndex, RegimGame gameMode, int joinToNewRound, bool abTestSeparate = false)
	{
		string text = string.Empty;
		if (Defs.useSqlLobby)
		{
			if (mapIndex == -1)
			{
				TypeModeGame needMode = ((!Defs.isDaterRegim) ? ((TypeModeGame)(int)Enum.Parse(typeof(TypeModeGame), gameMode.ToString())) : TypeModeGame.Dater);
				int[] array = SceneInfoController.instance.GetListScenesForMode(needMode).avaliableScenes.Select((SceneInfo m) => m.indexMap).ToArray();
				text += "( ";
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = text;
					text = text2 + mapProperty + " = " + array[i];
					if (i + 1 < array.Length)
					{
						text += " OR ";
					}
				}
				text += " )";
			}
			else
			{
				text = mapProperty + " = " + mapIndex;
			}
			text = text + " AND " + passwordProperty + " = \"\"";
			if (!Defs.isDaterRegim)
			{
				string text2 = text;
				string[] obj = new string[5] { text2, " AND ", platformProperty, " = ", null };
				int num = (int)myPlatformConnect;
				obj[4] = num.ToString();
				text = string.Concat(obj);
			}
			switch (joinToNewRound)
			{
			case 0:
				text = text + " AND " + endingProperty + " = 0";
				break;
			case 1:
				text = text + " AND " + endingProperty + " = 2";
				break;
			}
			if (ExpController.Instance != null && ExpController.Instance.OurTier == 0)
			{
				text = ((!Defs.isABTestBalansCohortActual) ? (text + " AND " + ABTestProperty + " = 0") : (text + " AND " + ABTestProperty + " = 1"));
			}
			if (Defs.isActivABTestBuffSystem)
			{
				text = text + " AND " + ABTestEnum + " = ";
				if (abTestSeparate)
				{
					if (Defs.isActivABTestBuffSystem)
					{
						text += ((!ABTestController.useBuffSystem) ? 1 : 3);
					}
				}
				else if (Defs.isActivABTestBuffSystem)
				{
					text += (ABTestController.useBuffSystem ? 1 : 3);
				}
			}
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[passwordProperty] = string.Empty;
		if (!Defs.useSqlLobby)
		{
			hashtable[mapProperty] = mapIndex;
		}
		if (!Defs.isDaterRegim && regim != RegimGame.DeadlyGames && regim != RegimGame.TimeBattle)
		{
			hashtable[maxKillProperty] = 3;
		}
		if (joinToNewRound == 0)
		{
			hashtable[endingProperty] = 0;
		}
		if (!Defs.isDaterRegim)
		{
			hashtable[platformProperty] = (int)myPlatformConnect;
		}
		if (ExpController.Instance != null && ExpController.Instance.OurTier == 0)
		{
			if (Defs.isABTestBalansCohortActual)
			{
				hashtable[ABTestProperty] = 1;
			}
			else
			{
				hashtable[ABTestProperty] = 0;
			}
		}
		PlayerPrefs.SetString("TypeGame", "client");
		if (Defs.useSqlLobby)
		{
			TypedLobby typedLobby = new TypedLobby("PixelGun3D", LobbyType.SqlLobby);
			Debug.Log(text);
			PhotonNetwork.JoinRandomRoom(hashtable, 0, MatchmakingMode.FillRoom, typedLobby, text);
		}
		else
		{
			PhotonNetwork.JoinRandomRoom(hashtable, 0);
		}
	}

	private void JoinRandomRoom(int mapIndex, RegimGame gameMode)
	{
		joinNewRoundTries = 0;
		tryJoinRoundMap = mapIndex;
		if (mapIndex != -1)
		{
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(mapIndex);
			if (infoScene == null)
			{
				Debug.LogError("scInfo == null");
				return;
			}
			goMapName = infoScene.NameScene;
		}
		else if (!Defs.useSqlLobby)
		{
			mapIndex = GetRandomMapIndex();
			if (mapIndex == -1)
			{
				return;
			}
			SceneInfo infoScene2 = SceneInfoController.instance.GetInfoScene(mapIndex);
			if (infoScene2 == null)
			{
				Debug.LogError("scInfo == null");
				return;
			}
			goMapName = infoScene2.NameScene;
		}
		else
		{
			goMapName = string.Empty;
		}
		if (!string.IsNullOrEmpty(goMapName))
		{
			if (WeaponManager.sharedManager != null)
			{
				WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(goMapName) ? Defs.filterMaps[goMapName] : 0);
			}
			StartCoroutine(SetFonLoadingWaitForReset(goMapName));
			loadingMapPanel.SetActive(true);
			ActivityIndicator.IsActiveIndicator = true;
		}
		JoinRandomGameRoom(mapIndex, gameMode, joinNewRoundTries, abTestConnect);
	}

	private void HandleCreateRoomBtnClicked(object sender, EventArgs e)
	{
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(selectMap.mapID);
		if (infoScene == null)
		{
			return;
		}
		string text = infoScene.gameObject.name;
		if (infoScene.isPremium && Storager.getInt(text + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(text))
		{
			PhotonNetwork.isMessageQueueRunning = true;
			PhotonNetwork.Disconnect();
			return;
		}
		string text2 = FilterBadWorld.FilterString(nameServerInput.value);
		bool flag = false;
		if (Defs.isInet)
		{
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			for (int i = 0; i < roomList.Length; i++)
			{
				if (roomList[i].name.Equals(text2))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			nameAlreadyUsedLabel.timer = 3f;
			nameAlreadyUsedLabel.gameObject.SetActive(true);
			return;
		}
		goMapName = text;
		PlayerPrefs.SetString("MapName", goMapName);
		if (killToWin.value.Value > killToWin.maxValue.Value)
		{
			killToWin.value = killToWin.maxValue;
		}
		if (killToWin.value.Value < killToWin.minValue.Value)
		{
			killToWin.value = killToWin.minValue;
		}
		PlayerPrefs.SetString("MaxKill", "4");
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(text) ? Defs.filterMaps[text] : 0);
		}
		StartCoroutine(SetFonLoadingWaitForReset(goMapName));
		loadingMapPanel.SetActive(true);
		int num = ((regim == RegimGame.Deathmatch || regim == RegimGame.TimeBattle || regim == RegimGame.DeadlyGames) ? numberOfPlayer.value.Value : ((regim != RegimGame.Duel) ? teamCountPlayer.value : 2));
		int maxKill = (Defs.isDaterRegim ? killToWin.value.Value : ((regim == RegimGame.DeadlyGames) ? 10 : ((!Defs.isDaterRegim) ? 4 : 5)));
		if (Defs.isInet)
		{
			loadingMapPanel.SetActive(true);
			ActivityIndicator.IsActiveIndicator = true;
			CreateGameRoom(text2, num, infoScene.indexMap, maxKill, setPasswordInput.value, regim);
		}
		else
		{
			bool useNat = Network.HavePublicAddress();
			Network.InitializeServer(num - 1, 25002, useNat);
			PlayerPrefs.SetString("ServerName", text2);
			PlayerPrefs.SetString("PlayersLimits", num.ToString());
			Singleton<SceneLoader>.Instance.LoadSceneAsync("PromScene");
		}
	}

	private void ShowKillToWinPanel(bool show)
	{
		if (show)
		{
			numberOfPlayer.transform.localPosition = new Vector3(posNumberOffPlayersX, numberOfPlayer.transform.localPosition.y, numberOfPlayer.transform.localPosition.z);
			teamCountPlayer.transform.localPosition = new Vector3(posNumberOffPlayersX, teamCountPlayer.transform.localPosition.y, teamCountPlayer.transform.localPosition.z);
			killToWin.headLabel.text = LocalizationStore.Get("Key_0953");
			killToWin.gameObject.SetActive(true);
		}
		else
		{
			numberOfPlayer.transform.localPosition = new Vector3(0f, numberOfPlayer.transform.localPosition.y, numberOfPlayer.transform.localPosition.z);
			teamCountPlayer.transform.localPosition = new Vector3(0f, teamCountPlayer.transform.localPosition.y, teamCountPlayer.transform.localPosition.z);
			killToWin.gameObject.SetActive(false);
		}
	}

	private void HandleGoToCreateRoomBtnClicked(object sender, EventArgs e)
	{
		PlayerPrefs.SetString("TypeGame", "server");
		password = string.Empty;
		SetPosSelectMapPanelInCreatePanel();
		createPanel.SetActive(true);
		setPasswordInput.gameObject.SetActive(Defs.isInet);
		nameServerInput.transform.localPosition = ((!Defs.isInet) ? new Vector3(0f, startPosNameServerNameInput.y, startPosNameServerNameInput.z) : startPosNameServerNameInput);
		selectMapPanel.SetActive(true);
		StartCoroutine(SetUseMasMap(false));
		customPanel.SetActive(false);
		nameAlreadyUsedLabel.timer = -1f;
		nameAlreadyUsedLabel.gameObject.SetActive(false);
		if (regim == RegimGame.Deathmatch)
		{
			teamCountPlayer.gameObject.SetActive(false);
			numberOfPlayer.gameObject.SetActive(false);
			numberOfPlayer.minValue.Value = 2;
			numberOfPlayer.maxValue.Value = 10;
			numberOfPlayer.value.Value = 10;
			if (Defs.isDaterRegim)
			{
				ShowKillToWinPanel(true);
				killToWin.minValue.Value = daterMinValue;
				killToWin.maxValue.Value = daterMaxValue;
				killToWin.value.Value = daterMinValue;
				killToWin.stepValue = daterStep;
			}
			else
			{
				ShowKillToWinPanel(false);
				if (ExperienceController.sharedController != null)
				{
					if (ExperienceController.sharedController.currentLevel <= 2)
					{
						killToWin.minValue.Value = 3;
						killToWin.maxValue.Value = 7;
						killToWin.value.Value = 3;
						killToWin.stepValue = 2;
					}
					else
					{
						killToWin.minValue.Value = 3;
						killToWin.maxValue.Value = 7;
						killToWin.value.Value = 3;
						killToWin.stepValue = 2;
					}
				}
			}
		}
		if (regim == RegimGame.TimeBattle)
		{
			teamCountPlayer.gameObject.SetActive(false);
			numberOfPlayer.gameObject.SetActive(false);
			numberOfPlayer.minValue.Value = 2;
			numberOfPlayer.maxValue.Value = 4;
			numberOfPlayer.value.Value = 4;
			ShowKillToWinPanel(false);
		}
		if (regim == RegimGame.TeamFight)
		{
			teamCountPlayer.gameObject.SetActive(false);
			teamCountPlayer.SetValue(10);
			numberOfPlayer.gameObject.SetActive(false);
			ShowKillToWinPanel(false);
			killToWin.stepValue = 2;
			if (ExperienceController.sharedController != null)
			{
				if (ExperienceController.sharedController.currentLevel <= 2)
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
				else
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
			}
		}
		if (regim == RegimGame.FlagCapture)
		{
			teamCountPlayer.gameObject.SetActive(false);
			teamCountPlayer.SetValue(10);
			numberOfPlayer.gameObject.SetActive(false);
			ShowKillToWinPanel(false);
			killToWin.stepValue = 2;
			if (ExperienceController.sharedController != null)
			{
				if (ExperienceController.sharedController.currentLevel <= 2)
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
				else
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
			}
		}
		if (regim == RegimGame.CapturePoints)
		{
			teamCountPlayer.gameObject.SetActive(false);
			teamCountPlayer.SetValue(10);
			numberOfPlayer.gameObject.SetActive(false);
			ShowKillToWinPanel(false);
			killToWin.stepValue = 2;
			if (ExperienceController.sharedController != null)
			{
				if (ExperienceController.sharedController.currentLevel <= 2)
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
				else
				{
					killToWin.minValue.Value = 3;
					killToWin.maxValue.Value = 7;
					killToWin.value.Value = 3;
				}
			}
		}
		if (regim == RegimGame.DeadlyGames)
		{
			teamCountPlayer.gameObject.SetActive(false);
			numberOfPlayer.gameObject.SetActive(false);
			numberOfPlayer.minValue.Value = 3;
			numberOfPlayer.maxValue.Value = 8;
			numberOfPlayer.value.Value = 6;
			ShowKillToWinPanel(false);
			killToWin.stepValue = 5;
			if (ExperienceController.sharedController != null)
			{
				if (ExperienceController.sharedController.currentLevel <= 2)
				{
					killToWin.minValue.Value = 5;
					killToWin.maxValue.Value = 10;
					killToWin.value.Value = 10;
				}
				else
				{
					killToWin.minValue.Value = 5;
					killToWin.maxValue.Value = 10;
					killToWin.value.Value = 10;
				}
			}
		}
		if (regim == RegimGame.Duel)
		{
			teamCountPlayer.gameObject.SetActive(false);
			numberOfPlayer.gameObject.SetActive(false);
			numberOfPlayer.minValue.Value = 2;
			numberOfPlayer.maxValue.Value = 2;
			numberOfPlayer.value.Value = 2;
			ShowKillToWinPanel(false);
			killToWin.minValue.Value = 3;
			killToWin.maxValue.Value = 7;
			killToWin.value.Value = 3;
			killToWin.stepValue = 2;
		}
	}

	private void HandleShowSearchPanelBtnClicked(object sender, EventArgs e)
	{
		customPanel.SetActive(false);
		if (searchInput != null)
		{
			searchInput.value = gameNameFilter;
		}
		searchPanel.SetActive(true);
	}

	private void HandleClearBtnClicked(object sender, EventArgs e)
	{
		if (searchInput != null)
		{
			searchInput.value = string.Empty;
		}
	}

	private void HandleSearchBtnClicked(object sender, EventArgs e)
	{
		customPanel.SetActive(true);
		if (searchInput != null)
		{
			gameNameFilter = searchInput.value;
		}
		updateFilteredRoomList(gameNameFilter);
		scrollGames.ResetPosition();
		searchPanel.SetActive(false);
	}

	private void HandleCancelFromConnectToPhotonBtnClicked()
	{
		if (_someWindowSubscription != null)
		{
			_someWindowSubscription.Dispose();
		}
		if (failInternetLabel != null)
		{
			failInternetLabel.SetActive(false);
		}
		if (connectToPhotonPanel != null)
		{
			connectToPhotonPanel.SetActive(false);
		}
		if (actAfterConnectToPhoton != null)
		{
			actAfterConnectToPhoton = null;
			return;
		}
		PhotonNetwork.isMessageQueueRunning = true;
		PhotonNetwork.Disconnect();
	}

	private void HandleCancelFromConnectToPhotonBtnClicked(object sender, EventArgs e)
	{
		HandleCancelFromConnectToPhotonBtnClicked();
	}

	private void LogBuyMap(string context)
	{
		try
		{
			AnalyticsStuff.LogSales(context, "Premium Maps");
		}
		catch (Exception ex)
		{
			Debug.LogError("LogBuyMap exception: " + ex);
		}
	}

	private void HandleUnlockMapBtnClicked(object sender, EventArgs e)
	{
		SceneInfo scInfo = SceneInfoController.instance.GetInfoScene(selectMap.mapID);
		if (scInfo == null)
		{
			return;
		}
		int mapPrice = Defs.PremiumMaps[scInfo.NameScene];
		Action action = null;
		action = delegate
		{
			coinsShop.thisScript.notEnoughCurrency = null;
			coinsShop.thisScript.onReturnAction = null;
			int @int = Storager.getInt("Coins", false);
			int num = @int - mapPrice;
			string nameScene = scInfo.NameScene;
			if (num >= 0)
			{
				LogBuyMap(nameScene);
				AnalyticsFacade.InAppPurchase(nameScene, "Premium Maps", 1, mapPrice, "Coins");
				Storager.setInt(nameScene + "Key", 1, true);
				selectMap.mapPreviewTexture.mainTexture = mapPreview[nameScene];
				Storager.setInt("Coins", num, false);
				ShopNGUIController.SpendBoughtCurrency("Coins", mapPrice);
				if (Application.platform != RuntimePlatform.IPhonePlayer)
				{
					PlayerPrefs.Save();
				}
				ShopNGUIController.SynchronizeAndroidPurchases("Map unlocked from connect scene: " + nameScene);
				if (coinsPlashka.thisScript != null)
				{
					coinsPlashka.thisScript.enabled = false;
				}
			}
			else
			{
				StoreKitEventListener.State.PurchaseKey = "In map selection";
				if (BankController.Instance != null)
				{
					EventHandler handleBackFromBank = null;
					handleBackFromBank = delegate
					{
						BankController.Instance.BackRequested -= handleBackFromBank;
						mainPanel.transform.root.gameObject.SetActive(true);
						coinsShop.thisScript.notEnoughCurrency = null;
						BankController.Instance.InterfaceEnabled = false;
					};
					BankController.Instance.BackRequested += handleBackFromBank;
					mainPanel.transform.root.gameObject.SetActive(false);
					coinsShop.thisScript.notEnoughCurrency = "Coins";
					BankController.Instance.InterfaceEnabled = true;
				}
				else
				{
					Debug.LogWarning("BankController.Instance == null");
				}
			}
		};
		action();
	}

	public void ShowBankWindow()
	{
		if (BankController.Instance != null)
		{
			EventHandler backFromBankHandler = null;
			backFromBankHandler = delegate
			{
				BankController.Instance.BackRequested -= backFromBankHandler;
				mainPanel.transform.root.gameObject.SetActive(true);
				BankController.Instance.InterfaceEnabled = false;
			};
			BankController.Instance.BackRequested += backFromBankHandler;
			mainPanel.transform.root.gameObject.SetActive(false);
			BankController.Instance.InterfaceEnabled = true;
		}
		else
		{
			Debug.LogWarning("BankController.Instance == null");
		}
	}

	private void HandleCoinsShopClicked(object sender, EventArgs e)
	{
		ShowBankWindow();
	}

	private void HandleLocalBtnClicked(object sender, EventArgs e)
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		Defs.isInet = false;
		CustomBtnAct();
	}

	private void ShowConnectToPhotonPanel()
	{
		_someWindowSubscription = BackSystem.Instance.Register(HandleCancelFromConnectToPhotonBtnClicked, "Connect to Photon panel");
		if (FriendsController.sharedController != null && FriendsController.sharedController.Banned == 1)
		{
			accountBlockedLabel.timer = 3f;
			accountBlockedLabel.gameObject.SetActive(true);
		}
		else
		{
			ConnectToPhoton();
			connectToPhotonPanel.SetActive(true);
		}
	}

	private void HandleCustomBtnClicked(object sender, EventArgs e)
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		actAfterConnectToPhoton = CustomBtnAct;
		PhotonNetwork.autoJoinLobby = true;
		ShowConnectToPhotonPanel();
	}

	private void CustomBtnAct()
	{
		gameNameFilter = string.Empty;
		if (Defs.isInet)
		{
			Invoke("UpdateFilteredRoomListInvoke", 0.03f);
		}
		showSearchPanelBtn.SetActive(Defs.isInet);
		mainPanel.SetActive(false);
		selectMapPanel.SetActive(false);
		customPanel.SetActive(true);
		if (!Defs.isDaterRegim)
		{
			headCustomPanel.SetText(LocalizationStore.Get(gameModesLocalizeKey[((int)regim).ToString()]));
		}
		else
		{
			headCustomPanel.gameObject.SetActive(false);
		}
		password = string.Empty;
		incorrectPasswordLabel.timer = -1f;
		incorrectPasswordLabel.gameObject.SetActive(false);
		gameIsfullLabel.timer = -1f;
		gameIsfullLabel.gameObject.SetActive(false);
	}

	[Obfuscation(Exclude = true)]
	private void UpdateFilteredRoomListInvoke()
	{
		updateFilteredRoomList(gameNameFilter);
	}

	private void HandleRandomBtnClicked(object sender, EventArgs e)
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		actAfterConnectToPhoton = RandomBtnAct;
		PhotonNetwork.autoJoinLobby = false;
		ShowConnectToPhotonPanel();
	}

	private void RandomBtnAct()
	{
		JoinRandomRoom(-1, regim);
	}

	public static int GetRandomMapIndex()
	{
		bool flag = true;
		AllScenesForMode listScenesForMode = SceneInfoController.instance.GetListScenesForMode(curSelectMode);
		if (listScenesForMode == null)
		{
			return -1;
		}
		int count = listScenesForMode.avaliableScenes.Count;
		int num = UnityEngine.Random.Range(0, count);
		int num2 = 0;
		SceneInfo sceneInfo;
		do
		{
			if (num2 > count)
			{
				return -1;
			}
			sceneInfo = listScenesForMode.avaliableScenes[num];
			if (!(sceneInfo == null))
			{
				num++;
				num2++;
				if (num >= count)
				{
					num = 0;
				}
				flag = sceneInfo.isPremium && Storager.getInt(sceneInfo.NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(sceneInfo.NameScene);
			}
		}
		while (flag);
		return sceneInfo.indexMap;
	}

	public void HandleGoBtnClicked(object sender, EventArgs e)
	{
		if (selectMap.mapID == -1)
		{
			HandleRandomBtnClicked(sender, e);
			return;
		}
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		actAfterConnectToPhoton = GoBtnAct;
		PhotonNetwork.autoJoinLobby = false;
		ShowConnectToPhotonPanel();
	}

	private void GoBtnAct()
	{
		if (selectMap.mapID == -1)
		{
			RandomBtnAct();
			return;
		}
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(selectMap.mapID);
		if (!(infoScene == null))
		{
			bool isPremium = infoScene.isPremium;
			if (!isPremium || (isPremium && (Storager.getInt(infoScene.NameScene + "Key", true) == 1 || PremiumAccountController.MapAvailableDueToPremiumAccount(infoScene.NameScene))))
			{
				JoinRandomRoom(infoScene.indexMap, regim);
				return;
			}
			PhotonNetwork.isMessageQueueRunning = true;
			PhotonNetwork.Disconnect();
		}
	}

	private void HandleBackBtnClicked(object sender, EventArgs e)
	{
		if (mainPanel != null && mainPanel.activeSelf)
		{
			if (FriendsController.sharedController != null)
			{
				FriendsController.sharedController.GetFriendsData(false);
			}
			mapPreview.Clear();
			MenuBackgroundMusic.keepPlaying = true;
			LoadConnectScene.textureToShow = null;
			LoadConnectScene.sceneToLoad = Defs.MainMenuScene;
			LoadConnectScene.noteToShow = null;
			Application.LoadLevel(Defs.PromSceneName);
			isGoInPhotonGame = false;
		}
		if (customPanel != null && customPanel.activeSelf)
		{
			connectToWiFIInCreateLabel.SetActive(false);
			connectToWiFIInCustomLabel.SetActive(false);
			createRoomUIBtn.isEnabled = true;
			Defs.isInet = true;
			customPanel.SetActive(false);
			while (gridGamesTransform.childCount > 0)
			{
				Transform child = gridGamesTransform.GetChild(0);
				child.parent = null;
				gamesInfo.Remove(child.gameObject);
				UnityEngine.Object.Destroy(child.gameObject);
			}
			mainPanel.SetActive(true);
			selectMapPanel.SetActive(true);
			PhotonNetwork.isMessageQueueRunning = true;
			PhotonNetwork.Disconnect();
		}
		if (searchPanel != null && searchPanel.activeSelf)
		{
			searchInput.value = gameNameFilter;
			searchPanel.SetActive(false);
			customPanel.SetActive(true);
		}
		if (createPanel != null && createPanel.activeSelf)
		{
			PlayerPrefs.SetString("TypeGame", "client");
			createPanel.SetActive(false);
			StartCoroutine(SetUseMasMap(true, true));
			customPanel.SetActive(true);
		}
		if (setPasswordPanel != null && setPasswordPanel.activeSelf)
		{
			BackFromSetPasswordPanel();
		}
		if (enterPasswordPanel != null && enterPasswordPanel.activeSelf)
		{
			enterPasswordPanel.SetActive(false);
			customPanel.SetActive(true);
			if (ExperienceController.sharedController != null)
			{
				ExperienceController.sharedController.isShowRanks = true;
			}
		}
	}

	private void HandleUnlockBtnClicked(object sender, EventArgs e)
	{
		int _price = 0;
		string _storagerPurchasedKey = string.Empty;
		if (regim == RegimGame.FlagCapture)
		{
			_price = Defs.CaptureFlagPrice;
			_storagerPurchasedKey = Defs.CaptureFlagPurchasedKey;
		}
		if (regim == RegimGame.DeadlyGames)
		{
			_price = Defs.HungerGamesPrice;
			_storagerPurchasedKey = Defs.hungerGamesPurchasedKey;
		}
		Action action = null;
		action = delegate
		{
			coinsShop.thisScript.notEnoughCurrency = null;
			coinsShop.thisScript.onReturnAction = null;
			int @int = Storager.getInt("Coins", false);
			int num = @int - _price;
			if (num >= 0)
			{
				Storager.setInt(_storagerPurchasedKey, 1, true);
				Storager.setInt("Coins", num, false);
				ShopNGUIController.SpendBoughtCurrency("Coins", _price);
				if (Application.platform != RuntimePlatform.IPhonePlayer)
				{
					PlayerPrefs.Save();
				}
				ShopNGUIController.SynchronizeAndroidPurchases("Mode enabled");
				if (coinsPlashka.thisScript != null)
				{
					coinsPlashka.thisScript.enabled = false;
				}
				unlockBtn.SetActive(false);
				customBtn.SetActive(true);
				randomBtn.SetActive(true);
				conditionLabel.gameObject.SetActive(false);
				goBtn.SetActive(true);
			}
			else
			{
				StoreKitEventListener.State.PurchaseKey = "Mode opened";
				if (BankController.Instance != null)
				{
					EventHandler handleBackFromBank = null;
					handleBackFromBank = delegate
					{
						BankController.Instance.BackRequested -= handleBackFromBank;
						mainPanel.transform.root.gameObject.SetActive(true);
						coinsShop.thisScript.notEnoughCurrency = null;
						BankController.Instance.InterfaceEnabled = false;
					};
					BankController.Instance.BackRequested += handleBackFromBank;
					mainPanel.transform.root.gameObject.SetActive(false);
					coinsShop.thisScript.notEnoughCurrency = "Coins";
					BankController.Instance.InterfaceEnabled = true;
				}
				else
				{
					Debug.LogWarning("BankController.Instance == null");
				}
			}
		};
		action();
	}

	private static void SetFlagsForDeathmatchRegim()
	{
		Defs.isMulti = true;
		Defs.isInet = true;
		Defs.isCOOP = false;
		Defs.isCompany = false;
		Defs.isHunger = false;
		Defs.isFlag = false;
		Defs.isCapturePoints = false;
		Defs.IsSurvival = false;
		Defs.isDuel = false;
		StoreKitEventListener.State.Mode = "Deathmatch Wordwide";
		StoreKitEventListener.State.Parameters.Clear();
	}

	private void SetRegim(RegimGame _regim)
	{
		PlayerPrefs.SetInt("RegimMulty", (int)_regim);
		regim = _regim;
		if (!Defs.isDaterRegim)
		{
			unlockMapBtn.SetActive(false);
			unlockMapBtnInCreate.SetActive(false);
		}
		createRoomBtn.SetActive(true);
		Defs.isMulti = true;
		Defs.isInet = true;
		Defs.isCOOP = regim == RegimGame.TimeBattle;
		Defs.isCompany = regim == RegimGame.TeamFight;
		Defs.isHunger = regim == RegimGame.DeadlyGames;
		Defs.isFlag = regim == RegimGame.FlagCapture;
		Defs.isCapturePoints = regim == RegimGame.CapturePoints;
		Defs.isDuel = regim == RegimGame.Duel;
		Defs.IsSurvival = false;
		localBtn.SetActive(BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64 && (regim == RegimGame.Deathmatch || regim == RegimGame.TeamFight));
		customButtonsGrid.Reposition();
		if (conditionLabel != null)
		{
			conditionLabel.gameObject.SetActive(false);
		}
		if (unlockBtn != null)
		{
			unlockBtn.SetActive(false);
		}
		if (randomBtn != null)
		{
			randomBtn.SetActive(true);
		}
		if (customBtn != null)
		{
			customBtn.SetActive(true);
		}
		if (goBtn != null)
		{
			goBtn.SetActive(true);
		}
		switch (regim)
		{
		case RegimGame.Deathmatch:
			StoreKitEventListener.State.Mode = "Deathmatch Wordwide";
			StoreKitEventListener.State.Parameters.Clear();
			rulesLabel.text = ((!Defs.isDaterRegim) ? rulesDeadmatch : rulesDater);
			break;
		case RegimGame.TimeBattle:
			StoreKitEventListener.State.Mode = "Time Survival";
			StoreKitEventListener.State.Parameters.Clear();
			rulesLabel.text = rulesTimeBattle;
			break;
		case RegimGame.TeamFight:
			StoreKitEventListener.State.Mode = "Team Battle";
			StoreKitEventListener.State.Parameters.Clear();
			rulesLabel.text = rulesTeamFight;
			break;
		case RegimGame.FlagCapture:
			StoreKitEventListener.State.Mode = "Flag Capture";
			StoreKitEventListener.State.Parameters.Clear();
			rulesLabel.text = rulesFlagCapture;
			break;
		case RegimGame.CapturePoints:
			StoreKitEventListener.State.Mode = "Capture points";
			StoreKitEventListener.State.Parameters.Clear();
			rulesLabel.text = rulesCapturePoint;
			break;
		case RegimGame.DeadlyGames:
			StoreKitEventListener.State.Mode = "Deadly Games";
			StoreKitEventListener.State.Parameters.Clear();
			rulesLabel.text = rulesDeadlyGames;
			break;
		case RegimGame.Duel:
			StoreKitEventListener.State.Mode = "Duel";
			StoreKitEventListener.State.Parameters.Clear();
			rulesLabel.text = rulesDuel;
			break;
		}
		StartCoroutine(SetUseMasMap(true, false));
	}

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(delegate
		{
			if (BannerWindowController.SharedController != null && BannerWindowController.SharedController.IsAnyBannerShown)
			{
				BannerWindowController.SharedController.HideBannerWindow();
			}
			else
			{
				HandleBackBtnClicked(null, EventArgs.Empty);
			}
		}, "Connect Scene");
		OnEnableWhenAnimate();
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void Update()
	{
		if (Defs.IsDeveloperBuild)
		{
			Camera[] array = UnityEngine.Object.FindObjectsOfType<Camera>();
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text = text + array[i].transform.root.gameObject.name + " " + array[i].gameObject.name + " " + LayerMask.LayerToName(array[i].transform.root.gameObject.layer) + " - ";
			}
			if (_logCache != text)
			{
				_logCache = text;
				Debug.Log("Cameras:" + text);
			}
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			countNote++;
		}
		bool flag = deathmatchToggle != null && deathmatchToggle.isEnable;
		if (armoryButton != null && armoryButton.activeSelf != flag)
		{
			armoryButton.SetActive(flag);
		}
		if (customPanel.activeSelf && !Defs.isInet)
		{
			if (isFirstUpdateLocalServerList)
			{
				UpdateLocalServersList();
			}
			else
			{
				isFirstUpdateLocalServerList = true;
			}
		}
		if (!Defs.isInet)
		{
			connectToWiFIInCreateLabel.SetActive(!CheckLocalAvailability());
			connectToWiFIInCustomLabel.SetActive(!CheckLocalAvailability());
			if (createRoomUIBtn.isEnabled != CheckLocalAvailability())
			{
				createRoomUIBtn.isEnabled = CheckLocalAvailability();
			}
		}
		else
		{
			if (connectToWiFIInCreateLabel.activeSelf)
			{
				connectToWiFIInCreateLabel.SetActive(false);
			}
			if (connectToWiFIInCreateLabel.activeSelf)
			{
				connectToWiFIInCustomLabel.SetActive(false);
			}
		}
		if (!unlockBtn.activeSelf && (mainPanel.activeSelf || createPanel.activeSelf) && selectMap != null)
		{
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(selectMap.mapID);
			if (infoScene == null)
			{
				return;
			}
			if (!isSetUseMap && infoScene.isPremium && Storager.getInt(infoScene.NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(infoScene.NameScene))
			{
				if (!unlockMapBtn.activeSelf)
				{
					priceMapLabel.text = Defs.PremiumMaps[infoScene.NameScene].ToString();
					unlockMapBtn.SetActive(true);
					goBtn.SetActive(false);
					priceMapLabelInCreate.text = Defs.PremiumMaps[infoScene.NameScene].ToString();
					unlockMapBtnInCreate.SetActive(true);
					createRoomBtn.SetActive(false);
				}
			}
			else if (unlockMapBtn.activeSelf)
			{
				unlockMapBtn.SetActive(false);
				goBtn.SetActive(true);
				unlockMapBtnInCreate.SetActive(false);
				createRoomBtn.SetActive(true);
			}
		}
		if ((!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled) && (!(BannerWindowController.SharedController != null) || !BannerWindowController.SharedController.IsAnyBannerShown) && (!(loadingToDraw != null) || !loadingToDraw.gameObject.activeInHierarchy) && (!(_loadingNGUIController != null) || !_loadingNGUIController.gameObject.activeInHierarchy) && SkinEditorController.sharedController == null && ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.isShowRanks = true;
		}
	}

	private bool IsUseMap(int indMap)
	{
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(curSelectMode, indMap);
		if (infoScene != null)
		{
			bool flag = infoScene.isPremium && Storager.getInt(infoScene.NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(infoScene.NameScene);
			return !flag;
		}
		return false;
	}

	private static void ResetWeaponManagerForDeathmatch()
	{
		SetFlagsForDeathmatchRegim();
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(0);
		}
	}

	private IEnumerator LoadMapPreview()
	{
		List<SceneInfo> listAllNeedMap = new List<SceneInfo>();
		if (Defs.isDaterRegim)
		{
			AllScenesForMode scMode2 = SceneInfoController.instance.GetListScenesForMode(TypeModeGame.Dater);
			if (scMode2 != null)
			{
				listAllNeedMap.AddRange(scMode2.avaliableScenes);
			}
		}
		else
		{
			TypeModeGame[] arrNeedMode = new TypeModeGame[7]
			{
				TypeModeGame.Deathmatch,
				TypeModeGame.TeamFight,
				TypeModeGame.TimeBattle,
				TypeModeGame.FlagCapture,
				TypeModeGame.DeadlyGames,
				TypeModeGame.CapturePoints,
				TypeModeGame.Duel
			};
			TypeModeGame[] array = arrNeedMode;
			foreach (TypeModeGame curMode in array)
			{
				AllScenesForMode scMode = ((!(SceneInfoController.instance != null)) ? null : SceneInfoController.instance.GetListScenesForMode(curMode));
				if (scMode != null)
				{
					listAllNeedMap.AddRange(scMode.avaliableScenes);
				}
			}
		}
		string allScene = string.Empty;
		for (int scI = 0; scI < listAllNeedMap.Count; scI++)
		{
			if (!mapPreview.ContainsKey(listAllNeedMap[scI].NameScene))
			{
				allScene = allScene + listAllNeedMap[scI].NameScene + "\n";
				mapPreview.Add(listAllNeedMap[scI].NameScene, Resources.Load("LevelLoadingsPreview" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/Loading_" + listAllNeedMap[scI].NameScene) as Texture);
				if (listAllNeedMap[scI].isPremium && Storager.getInt(listAllNeedMap[scI].NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(listAllNeedMap[scI].NameScene))
				{
					mapPreview.Add(listAllNeedMap[scI].NameScene + "_off", Resources.Load<Texture>("LevelLoadingsPreview" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/Loading_" + listAllNeedMap[scI].NameScene + "_off"));
				}
				yield return null;
			}
		}
		mapPreview.Add("Random", Resources.Load("LevelLoadingsPreview" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/Random_Map") as Texture);
		if (Application.isEditor)
		{
			Debug.Log(allScene);
		}
		yield return null;
		mainPanel.SetActive(true);
		selectMapPanel.SetActive(true);
		ResetWeaponManagerForDeathmatch();
		SetRegim(regim);
		yield return null;
		string dismissReason = GetReasonToDismissInterstitialConnectScene();
		if (string.IsNullOrEmpty(dismissReason))
		{
			string format = ((!Application.isEditor) ? "{0}.LoadMapPreview(), InterstitialRequest: {1}" : "<color=magenta>{0}.LoadMapPreview(), InterstitialRequest: {1}</color>");
			Debug.LogFormat(format, GetType().Name, InterstitialRequest);
			if (InterstitialRequest)
			{
				AdsConfigMemento adsConfig = AdsConfigManager.Instance.LastLoadedConfig;
				string category = AdsConfigManager.GetPlayerCategory(adsConfig);
				ReturnInConnectSceneAdPointMemento pointConfig = adsConfig.AdPointsConfig.ReturnInConnectScene;
				double delayInSeconds = pointConfig.GetFinalDelayInSeconds(category);
				float startWaitingTime = Time.realtimeSinceStartup;
				while ((double)(Time.realtimeSinceStartup - startWaitingTime) < delayInSeconds)
				{
					yield return null;
				}
			}
		}
		else
		{
			string format2 = ((!Application.isEditor) ? "Dismissing wait for interstitial. {0}" : "<color=magenta>Dismissing wait for interstitial. {0}</color>");
			Debug.LogFormat(format2, dismissReason);
		}
		InterstitialRequest = false;
		ActivityIndicator.IsActiveIndicator = false;
		if (!Defs.isDaterRegim)
		{
			StartCoroutine(AnimateModeOpen());
		}
		yield return null;
		loadingMapPanel.SetActive(false);
		if (NeedShowReviewInConnectScene)
		{
			BannerWindowController.firstScreen = true;
		}
		yield return new WaitForSeconds(1f);
		if (NeedShowReviewInConnectScene)
		{
			NeedShowReviewInConnectScene = false;
			ReviewHUDWindow.Instance.ShowWindowRating();
		}
	}

	internal static string GetReasonToDismissFakeInterstitial()
	{
		//Discarded unreachable code: IL_00b5, IL_00ca
		try
		{
			AdsConfigMemento lastLoadedConfig = AdsConfigManager.Instance.LastLoadedConfig;
			if (lastLoadedConfig == null)
			{
				return "Ads config is `null`.";
			}
			if (lastLoadedConfig.Exception != null)
			{
				return lastLoadedConfig.Exception.Message;
			}
			string playerCategory = AdsConfigManager.GetPlayerCategory(lastLoadedConfig);
			InterstitialConfigMemento interstitialConfig = lastLoadedConfig.InterstitialConfig;
			bool realInterstitialsEnabled = interstitialConfig.GetEnabled(playerCategory);
			FakeInterstitialConfigMemento fakeInterstitialConfig = lastLoadedConfig.FakeInterstitialConfig;
			double timeSpanSinceLastShowInMinutes = AdsConfigManager.GetTimeSpanSinceLastShowInMinutes();
			double timeoutBetweenShowInMinutes = interstitialConfig.GetTimeoutBetweenShowInMinutes(playerCategory);
			if (timeSpanSinceLastShowInMinutes < timeoutBetweenShowInMinutes)
			{
				return "TimeoutBetweenShowInMinutes";
			}
			return fakeInterstitialConfig.GetDisabledReason(playerCategory, ExperienceController.GetCurrentLevel(), InterstitialCounter.Instance.FakeInterstitialCount, InterstitialCounter.Instance.FakeInterstitialCount + InterstitialCounter.Instance.RealInterstitialCount, realInterstitialsEnabled);
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
	}

	internal static int GetReasonCodeToDismissInterstitialConnectScene()
	{
		AdsConfigMemento lastLoadedConfig = AdsConfigManager.Instance.LastLoadedConfig;
		if (lastLoadedConfig == null)
		{
			return 100;
		}
		if (lastLoadedConfig.Exception != null)
		{
			return 200;
		}
		int interstitialDisabledReasonCode = AdsConfigManager.GetInterstitialDisabledReasonCode(lastLoadedConfig);
		if (interstitialDisabledReasonCode != 0)
		{
			return 300 + interstitialDisabledReasonCode;
		}
		ReturnInConnectSceneAdPointMemento returnInConnectScene = lastLoadedConfig.AdPointsConfig.ReturnInConnectScene;
		if (returnInConnectScene == null)
		{
			return 400;
		}
		string playerCategory = AdsConfigManager.GetPlayerCategory(lastLoadedConfig);
		int disabledReasonCode = returnInConnectScene.GetDisabledReasonCode(playerCategory);
		if (disabledReasonCode != 0)
		{
			return 500 + disabledReasonCode;
		}
		int currentDailyInterstitialCount = FyberFacade.Instance.GetCurrentDailyInterstitialCount();
		int finalImpressionMaxCountPerDay = returnInConnectScene.GetFinalImpressionMaxCountPerDay(playerCategory);
		if (currentDailyInterstitialCount >= finalImpressionMaxCountPerDay)
		{
			return 600;
		}
		double totalMinutes = InGameTimeKeeper.Instance.CurrentInGameTime.TotalMinutes;
		double finalMinInGameTimePerDayInMinutes = returnInConnectScene.GetFinalMinInGameTimePerDayInMinutes(playerCategory);
		if (totalMinutes < finalMinInGameTimePerDayInMinutes)
		{
			return 700;
		}
		return 0;
	}

	internal static string GetReasonToDismissInterstitialConnectScene()
	{
		AdsConfigMemento lastLoadedConfig = AdsConfigManager.Instance.LastLoadedConfig;
		if (lastLoadedConfig == null)
		{
			return "Ads config is `null`.";
		}
		if (lastLoadedConfig.Exception != null)
		{
			return lastLoadedConfig.Exception.Message;
		}
		string interstitialDisabledReason = AdsConfigManager.GetInterstitialDisabledReason(lastLoadedConfig);
		if (!string.IsNullOrEmpty(interstitialDisabledReason))
		{
			return interstitialDisabledReason;
		}
		ReturnInConnectSceneAdPointMemento returnInConnectScene = lastLoadedConfig.AdPointsConfig.ReturnInConnectScene;
		if (returnInConnectScene == null)
		{
			return string.Format("`{0}` config is `null`", returnInConnectScene.Id);
		}
		string playerCategory = AdsConfigManager.GetPlayerCategory(lastLoadedConfig);
		string disabledReason = returnInConnectScene.GetDisabledReason(playerCategory);
		if (!string.IsNullOrEmpty(disabledReason))
		{
			return disabledReason;
		}
		int currentDailyInterstitialCount = FyberFacade.Instance.GetCurrentDailyInterstitialCount();
		int finalImpressionMaxCountPerDay = returnInConnectScene.GetFinalImpressionMaxCountPerDay(playerCategory);
		if (currentDailyInterstitialCount >= finalImpressionMaxCountPerDay)
		{
			return string.Format(CultureInfo.InvariantCulture, "`interstitialCount: {0}` >= `maxInterstitialCount: {1}` for `{2}`", currentDailyInterstitialCount, finalImpressionMaxCountPerDay, playerCategory);
		}
		double totalMinutes = InGameTimeKeeper.Instance.CurrentInGameTime.TotalMinutes;
		double finalMinInGameTimePerDayInMinutes = returnInConnectScene.GetFinalMinInGameTimePerDayInMinutes(playerCategory);
		if (totalMinutes < finalMinInGameTimePerDayInMinutes)
		{
			return string.Format(CultureInfo.InvariantCulture, "`inGameTimeMinutes: {0:f2}` < `minInGameTimePerDayInMinutes: {1:f2}` for `{2}`", totalMinutes, finalMinInGameTimePerDayInMinutes, playerCategory);
		}
		return string.Empty;
	}

	public static void UpdateUseMasMaps()
	{
		if (Defs.isDaterRegim)
		{
			curSelectMode = TypeModeGame.Dater;
			return;
		}
		switch (regim)
		{
		case RegimGame.TimeBattle:
			curSelectMode = TypeModeGame.TimeBattle;
			break;
		case RegimGame.TeamFight:
			curSelectMode = TypeModeGame.TeamFight;
			break;
		case RegimGame.DeadlyGames:
			curSelectMode = TypeModeGame.DeadlyGames;
			break;
		case RegimGame.FlagCapture:
			curSelectMode = TypeModeGame.FlagCapture;
			break;
		case RegimGame.CapturePoints:
			curSelectMode = TypeModeGame.CapturePoints;
			break;
		case RegimGame.Duel:
			curSelectMode = TypeModeGame.Duel;
			break;
		default:
			curSelectMode = TypeModeGame.Deathmatch;
			break;
		}
	}

	private IEnumerator SetUseMasMap(bool isUseRandom = true, bool isOffSelectMapPanel = false)
	{
		if (isOffSelectMapPanel)
		{
			scrollViewSelectMapTransform.GetComponent<UIPanel>().alpha = 0.001f;
		}
		isSetUseMap = true;
		SpringPanel _spr = ScrollTransform.GetComponent<SpringPanel>();
		if (_spr != null)
		{
			UnityEngine.Object.Destroy(_spr);
		}
		ScrollTransform.GetComponent<UIPanel>().clipOffset = new Vector2(0f, 0f);
		if (isUseRandom && !isOffSelectMapPanel)
		{
			SetPosSelectMapPanelInMainMenu();
		}
		int maxCountMaps = SceneInfoController.instance.GetMaxCountMapsInRegims();
		maxCountMaps++;
		AllScenesForMode modeInfo = ((!(SceneInfoController.instance != null)) ? null : SceneInfoController.instance.GetListScenesForMode(curSelectMode));
		if (modeInfo == null)
		{
			Debug.LogError("modeInfo == null");
			yield break;
		}
		yield return null;
		if (grid.transform.childCount < maxCountMaps)
		{
			float widthBorder = 15f;
			int countColumn = ((!((double)((float)Screen.width / (float)Screen.height) < 1.5)) ? 4 : 3);
			float _widthCell = (fonMapPreview.localSize.x - (float)countColumn * widthBorder - widthBorder) / (float)countColumn;
			float _heightCell = 1f;
			float _scale = 1f;
			mapPreviewTexture.SetActive(true);
			int startCountMapsPreview = grid.transform.childCount;
			for (int j = startCountMapsPreview; j < maxCountMaps; j++)
			{
				GameObject newTexture3 = UnityEngine.Object.Instantiate(mapPreviewTexture);
				newTexture3.transform.SetParent(grid.transform, false);
				MapPreviewController currentMapPreviewController2 = newTexture3.GetComponent<MapPreviewController>();
				_scale = _widthCell / currentMapPreviewController2.mapPreviewTexture.localSize.x;
				_heightCell = currentMapPreviewController2.mapPreviewTexture.localSize.y * _scale;
				newTexture3.transform.GetChild(0).localScale = new Vector3(_scale, _scale, 1f);
				newTexture3.name = "Map_" + j;
			}
			mapPreviewTexture.SetActive(false);
			grid.GetComponent<UIGrid>().cellWidth = _widthCell + widthBorder;
			grid.GetComponent<UIGrid>().cellHeight = _heightCell + widthBorder;
			grid.GetComponent<UIGrid>().maxPerLine = countColumn;
			grid.GetComponent<UIGrid>().Reposition();
		}
		List<SceneInfo> mapsLst = ((ExperienceController.sharedController.currentLevel >= 2) ? modeInfo.avaliableScenes : modeInfo.avaliableScenes.Shuffle().ToList());
		if (isUseRandom)
		{
			GameObject randomCell = grid.transform.GetChild(0).gameObject;
			MapPreviewController randomCellPreviewController = randomCell.GetComponent<MapPreviewController>();
			randomCellPreviewController.mapPreviewTexture.mainTexture = ((!mapPreview.ContainsKey("Random")) ? null : mapPreview["Random"]);
			randomCellPreviewController.NameMapLbl.GetComponent<SetHeadLabelText>().SetText(LocalizationStore.Get("Key_2463"));
			randomCellPreviewController.bottomPanel.SetActive(false);
			randomCellPreviewController.mapID = -1;
			randomCellPreviewController.sceneMapName = "Random";
		}
		for (int k = 0; k < mapsLst.Count; k++)
		{
			SceneInfo scInfo = mapsLst[k];
			GameObject newTexture = grid.transform.GetChild(k + (isUseRandom ? 1 : 0)).gameObject;
			if (!newTexture.activeSelf)
			{
				newTexture.SetActive(true);
			}
			MapPreviewController currentMapPreviewController = newTexture.GetComponent<MapPreviewController>();
			bool _isClose = scInfo.isPremium && Storager.getInt(scInfo.NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(scInfo.NameScene);
			if (!currentMapPreviewController.bottomPanel.activeSelf)
			{
				currentMapPreviewController.bottomPanel.SetActive(true);
			}
			currentMapPreviewController.mapPreviewTexture.mainTexture = ((!mapPreview.ContainsKey(scInfo.NameScene)) ? null : mapPreview[scInfo.NameScene]);
			currentMapPreviewController.NameMapLbl.GetComponent<SetHeadLabelText>().SetText(scInfo.TranslatePreviewName.ToUpper());
			currentMapPreviewController.SizeMapNameLbl[0].SetActive(scInfo.sizeMap == InfoSizeMap.small);
			currentMapPreviewController.SizeMapNameLbl[1].SetActive(scInfo.sizeMap == InfoSizeMap.normal);
			currentMapPreviewController.SizeMapNameLbl[2].SetActive(scInfo.sizeMap == InfoSizeMap.big || scInfo.sizeMap == InfoSizeMap.veryBig);
			currentMapPreviewController.mapID = scInfo.indexMap;
			currentMapPreviewController.sceneMapName = scInfo.NameScene;
			if (scInfo.AvaliableWeapon == ModeWeapon.knifes)
			{
				currentMapPreviewController.milee.SetActive(true);
				currentMapPreviewController.milee.GetComponent<UILabel>().text = LocalizationStore.Get("Key_0096");
			}
			else if (scInfo.AvaliableWeapon == ModeWeapon.sniper)
			{
				currentMapPreviewController.milee.SetActive(true);
				currentMapPreviewController.milee.GetComponent<UILabel>().text = LocalizationStore.Get("Key_0949");
			}
			else if (Defs.isDaterRegim)
			{
				currentMapPreviewController.milee.SetActive(true);
				currentMapPreviewController.milee.GetComponent<UILabel>().text = LocalizationStore.Get("Key_1421");
			}
			else
			{
				currentMapPreviewController.milee.SetActive(false);
			}
			currentMapPreviewController.UpdatePopularity();
		}
		scrollViewSelectMapTransform.GetComponent<UIScrollView>().ResetPosition();
		grid.transform.localPosition = new Vector3(ScrollTransform.GetComponent<UIPanel>().baseClipRegion.x, grid.transform.localPosition.y, grid.transform.localPosition.z);
		for (int i = mapsLst.Count + (isUseRandom ? 1 : 0); i < grid.transform.childCount; i++)
		{
			GameObject newTexture2 = grid.transform.GetChild(i).gameObject;
			if (newTexture2.activeSelf)
			{
				newTexture2.SetActive(false);
			}
		}
		SelectMap(selectedMap);
		yield return null;
		scrollViewSelectMapTransform.GetComponent<UIPanel>().SetDirty();
		scrollViewSelectMapTransform.GetComponent<UIPanel>().Refresh();
		if (isOffSelectMapPanel)
		{
			selectMapPanel.SetActive(false);
			scrollViewSelectMapTransform.GetComponent<UIPanel>().alpha = 1f;
			SetPosSelectMapPanelInMainMenu();
		}
		isSetUseMap = false;
	}

	private void SelectMap(string map)
	{
		selectedMap = map;
		float num = scrollViewSelectMapTransform.GetComponent<UIScrollView>().bounds.extents.y * 2f;
		float y = scrollViewSelectMapTransform.GetComponent<UIPanel>().GetViewSize().y;
		if (!string.IsNullOrEmpty(selectedMap))
		{
			Transform child = grid.transform.GetChild(0);
			foreach (Transform item in grid.transform)
			{
				string sceneMapName = item.GetComponent<MapPreviewController>().sceneMapName;
				if (!selectedMap.Equals(sceneMapName))
				{
					continue;
				}
				if (num > y)
				{
					float num2 = -1f * (item.localPosition.y + _heightCell * 0.5f);
					if (num2 < 0f)
					{
						num2 = 0f;
					}
					float y2 = scrollViewSelectMapTransform.GetComponent<UIPanel>().clipSoftness.y;
					float num3 = num - y + y2;
					if (num2 > num3)
					{
						num2 = num3;
					}
					scrollViewSelectMapTransform.localPosition = new Vector3(scrollViewSelectMapTransform.localPosition.x, num2, scrollViewSelectMapTransform.localPosition.z);
				}
				item.GetComponent<UIToggle>().value = true;
				selectMap = item.GetComponent<MapPreviewController>();
				break;
			}
			selectedMap = string.Empty;
		}
		else
		{
			grid.transform.GetChild(0).GetComponent<UIToggle>().value = true;
			selectMap = grid.transform.GetChild(0).GetComponent<MapPreviewController>();
		}
	}

	public void OnReceivedRoomListUpdate()
	{
		if (customPanel.activeSelf && Defs.isInet && Defs.isInet)
		{
			Invoke("UpdateFilteredRoomListInvoke", 0.03f);
		}
	}

	private void SetRoomInfo(GameInfo _gameInfo, int index)
	{
		_gameInfo.index = index;
		if (filteredRoomList.Count > index)
		{
			_gameInfo.gameObject.SetActive(true);
			RoomInfo roomInfo = filteredRoomList[index];
			string text = roomInfo.name;
			if (text.Length == 36 && text.IndexOf("-") == 8 && text.LastIndexOf("-") == 23)
			{
				text = LocalizationStore.Get("Key_0088");
			}
			_gameInfo.serverNameLabel.text = text;
			_gameInfo.countPlayersLabel.text = roomInfo.playerCount + "/" + roomInfo.maxPlayers;
			bool flag = string.IsNullOrEmpty(roomInfo.customProperties[passwordProperty].ToString());
			_gameInfo.openSprite.SetActive(flag);
			_gameInfo.closeSprite.SetActive(!flag);
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(roomInfo.customProperties[mapProperty].ToString()));
			string text2 = infoScene.TranslatePreviewName.ToUpper();
			_gameInfo.mapNameLabel.SetText(text2);
			_gameInfo.roomInfo = roomInfo;
			_gameInfo.mapTexture.mainTexture = mapPreview[infoScene.NameScene];
			_gameInfo.SizeMapNameLbl[0].SetActive(infoScene.sizeMap == InfoSizeMap.small);
			_gameInfo.SizeMapNameLbl[1].SetActive(infoScene.sizeMap == InfoSizeMap.normal);
			_gameInfo.SizeMapNameLbl[2].SetActive(infoScene.sizeMap == InfoSizeMap.big || infoScene.sizeMap == InfoSizeMap.veryBig);
		}
		else
		{
			_gameInfo.gameObject.SetActive(false);
		}
	}

	public void updateFilteredRoomList(string gFilter)
	{
		filteredRoomList.Clear();
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		bool flag = !string.IsNullOrEmpty(gFilter);
		for (int i = 0; i < roomList.Length; i++)
		{
			if (!flag && roomList[i].playerCount == roomList[i].maxPlayers)
			{
				continue;
			}
			if (!Defs.isDaterRegim && roomList[i].customProperties[platformProperty] != null)
			{
				string text = roomList[i].customProperties[platformProperty].ToString();
				int num = (int)myPlatformConnect;
				if (!text.Equals(num.ToString()) && !roomList[i].customProperties[platformProperty].ToString().Equals(3.ToString()))
				{
					continue;
				}
			}
			if ((!Defs.isABTestBalansCohortActual || !(ExpController.Instance != null) || ExpController.Instance.OurTier != 0 || (roomList[i].customProperties[ABTestProperty] != null && (int)roomList[i].customProperties[ABTestProperty] == 1)) && (Defs.isABTestBalansCohortActual || !(ExpController.Instance != null) || ExpController.Instance.OurTier != 0 || roomList[i].customProperties[ABTestProperty] == null || (int)roomList[i].customProperties[ABTestProperty] != 1))
			{
				bool flag2 = true;
				if (flag)
				{
					flag2 = roomList[i].name.StartsWith(gFilter, true, null) && (roomList[i].name.Length != 36 || roomList[i].name.IndexOf("-") != 8 || roomList[i].name.LastIndexOf("-") != 23);
				}
				if (flag2 && IsUseMap((int)roomList[i].customProperties[mapProperty]))
				{
					filteredRoomList.Add(roomList[i]);
				}
			}
		}
		if (countNote > 10)
		{
			countNote = 1;
		}
		countNote = 50;
		if (filteredRoomList.Count < countNote)
		{
			countNote = filteredRoomList.Count;
		}
		while (countNote < gamesInfo.Count)
		{
			UnityEngine.Object.Destroy(gamesInfo[gamesInfo.Count - 1]);
			gamesInfo.RemoveAt(gamesInfo.Count - 1);
		}
		if (countNote > gamesInfo.Count)
		{
			countColumn = ((!((double)((float)Screen.width / (float)Screen.height) < 1.5)) ? 4 : 3);
			_widthCell = (fonGames.localSize.x - (float)(countColumn * 10)) / (float)countColumn;
			if (countNote > gamesInfo.Count)
			{
				gameInfoItemPrefab.SetActive(true);
			}
			while (countNote > gamesInfo.Count)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(gameInfoItemPrefab);
				gameObject.name = "GameInfo_" + gamesInfo.Count;
				gameObject.transform.SetParent(gridGamesTransform, false);
				_scale = _widthCell / gameObject.GetComponent<GameInfo>().mapTexture.localSize.x;
				_heightCell = gameObject.GetComponent<GameInfo>().mapTexture.localSize.y * _scale;
				gameObject.transform.GetChild(0).transform.localScale = new Vector3(_scale, _scale, _scale);
				gamesInfo.Add(gameObject);
				gameInfoItemPrefab.SetActive(false);
			}
			if (gameInfoItemPrefab.activeSelf)
			{
				gameInfoItemPrefab.SetActive(false);
			}
			gridGames.GetComponent<UIGrid>().cellWidth = _widthCell + borderWidth;
			gridGames.GetComponent<UIGrid>().cellHeight = _heightCell + borderWidth;
			gridGames.GetComponent<UIGrid>().maxPerLine = countColumn;
		}
		float num2 = scrollGames.bounds.extents.y * 2f;
		float y = scrollGames.GetComponent<UIPanel>().GetViewSize().y;
		gridGames.Reposition();
		if (!isFirstGamesReposition || num2 < y)
		{
			gridGames.transform.localPosition = new Vector3((0f - (_widthCell + borderWidth)) * ((float)countColumn * 0.5f - 0.5f), gridGames.transform.localPosition.y, gridGames.transform.localPosition.z);
			scrollGames.ResetPosition();
			isFirstGamesReposition = true;
		}
		for (int j = 0; j < countNote; j++)
		{
			SetRoomInfo(gamesInfo[j].GetComponent<GameInfo>(), j);
		}
	}

	private void OnPhotonRandomJoinFailed()
	{
		Debug.Log("OnPhotonJoinRoomFailed");
		if (string.IsNullOrEmpty(goMapName))
		{
			int randomMapIndex = GetRandomMapIndex();
			if (randomMapIndex == -1)
			{
				return;
			}
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(randomMapIndex);
			if (infoScene == null)
			{
				return;
			}
			goMapName = infoScene.name;
		}
		SceneInfo infoScene2 = SceneInfoController.instance.GetInfoScene(goMapName);
		if (infoScene2 == null)
		{
			return;
		}
		if (joinNewRoundTries >= 2 && abTestConnect)
		{
			abTestConnect = false;
			joinNewRoundTries = 0;
		}
		if (joinNewRoundTries < 2)
		{
			Debug.Log("No rooms with new round: " + joinNewRoundTries + ((!abTestConnect) ? string.Empty : " <color=yellow>AbTestSeparate</color>"));
			joinNewRoundTries++;
			JoinRandomGameRoom(tryJoinRoundMap, regim, joinNewRoundTries, abTestConnect);
			return;
		}
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(goMapName) ? Defs.filterMaps[goMapName] : 0);
		}
		StartCoroutine(SetFonLoadingWaitForReset(goMapName));
		int maxKill = ((regim == RegimGame.DeadlyGames) ? 10 : ((!Defs.isDaterRegim) ? 4 : 5));
		int playerLimit = (Defs.isCOOP ? 4 : (Defs.isCompany ? 10 : (Defs.isHunger ? 6 : ((!Defs.isDuel) ? 10 : 2))));
		CreateGameRoom(null, playerLimit, infoScene2.indexMap, maxKill, string.Empty, regim);
	}

	private void OnPhotonJoinRoomFailed()
	{
		ActivityIndicator.IsActiveIndicator = false;
		loadingMapPanel.SetActive(false);
		gameIsfullLabel.timer = 3f;
		gameIsfullLabel.gameObject.SetActive(true);
		incorrectPasswordLabel.timer = -1f;
		incorrectPasswordLabel.gameObject.SetActive(false);
		Debug.Log("OnPhotonJoinRoomFailed");
	}

	private void OnJoinedRoom()
	{
		AnalyticsStuff.LogMultiplayer();
		Debug.Log("OnJoinedRoom " + PhotonNetwork.room.customProperties[mapProperty].ToString());
		PhotonNetwork.isMessageQueueRunning = false;
		NotificationController.ResetPaused();
		GlobalGameController.healthMyPlayer = 0f;
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(PhotonNetwork.room.customProperties[mapProperty].ToString()));
		goMapName = infoScene.NameScene;
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(goMapName) ? Defs.filterMaps[goMapName] : 0);
		}
		StartCoroutine(MoveToGameScene(infoScene.NameScene));
	}

	private void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
	}

	private void OnPhotonCreateRoomFailed()
	{
		Debug.Log("OnPhotonCreateRoomFailed");
		nameAlreadyUsedLabel.timer = 3f;
		nameAlreadyUsedLabel.gameObject.SetActive(true);
		loadingMapPanel.SetActive(false);
		ActivityIndicator.IsActiveIndicator = false;
	}

	private void OnDisconnectedFromPhoton()
	{
		Debug.Log("OnDisconnectedFromPhoton");
		if ((!mainPanel.activeSelf || loadingMapPanel.activeSelf) && firstConnectToPhoton && Defs.isInet)
		{
			mainPanel.SetActive(true);
			selectMapPanel.SetActive(true);
			createPanel.SetActive(false);
			StartCoroutine(SetUseMasMap(true, false));
			customPanel.SetActive(false);
			while (gridGamesTransform.childCount > 0)
			{
				Transform child = gridGamesTransform.GetChild(0);
				child.parent = null;
				gamesInfo.Remove(child.gameObject);
				UnityEngine.Object.Destroy(child.gameObject);
			}
			searchPanel.SetActive(false);
			setPasswordPanel.SetActive(false);
			enterPasswordPanel.SetActive(false);
			if (ExperienceController.sharedController != null)
			{
				ExperienceController.sharedController.isShowRanks = true;
			}
			loadingMapPanel.SetActive(false);
			SetPosSelectMapPanelInMainMenu();
			serverIsNotAvalible.timer = 3f;
			serverIsNotAvalible.gameObject.SetActive(true);
			UICamera.selectedObject = null;
			RegimGame regimGame = regim;
			ResetWeaponManagerForDeathmatch();
			SetRegim(regimGame);
		}
		if (actAfterConnectToPhoton != null)
		{
			Invoke("ConnectToPhoton", 0.5f);
		}
		if (connectToPhotonPanel.activeSelf)
		{
			failInternetLabel.SetActive(true);
		}
	}

	private void OnFailedToConnectToPhoton(object parameters)
	{
		Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters);
		if (connectToPhotonPanel.activeSelf)
		{
			failInternetLabel.SetActive(true);
		}
		if (!isCancelConnectingToPhoton)
		{
			Invoke("ConnectToPhoton", 1f);
		}
	}

	public void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster");
		firstConnectToPhoton = true;
		PhotonNetwork.playerName = ProfileController.GetPlayerNameOrDefault();
		if (connectToPhotonPanel.activeSelf && actAfterConnectToPhoton != new Action(RandomBtnAct))
		{
			connectToPhotonPanel.SetActive(false);
		}
		if (actAfterConnectToPhoton != null)
		{
			actAfterConnectToPhoton();
			actAfterConnectToPhoton = null;
		}
		else
		{
			PhotonNetwork.isMessageQueueRunning = true;
			PhotonNetwork.Disconnect();
		}
	}

	public void OnConnectedToPhoton()
	{
		Debug.Log("OnConnectedToPhoton");
	}

	public void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby: " + PhotonNetwork.lobby.Name);
		OnConnectedToMaster();
	}

	private IEnumerator SetFonLoadingWaitForReset(string _mapName = "", bool isAddCountRun = false)
	{
		GetMapName(_mapName, isAddCountRun);
		if (_loadingNGUIController != null)
		{
			UnityEngine.Object.Destroy(_loadingNGUIController.gameObject);
			_loadingNGUIController = null;
		}
		while (WeaponManager.sharedManager == null)
		{
			yield return null;
		}
		while (WeaponManager.sharedManager.LockGetWeaponPrefabs > 0)
		{
			yield return null;
		}
		ShowLoadingGUI(_mapName);
	}

	private void SetFonLoading(string _mapName = "", bool isAddCountRun = false)
	{
		GetMapName(_mapName, isAddCountRun);
		if (_loadingNGUIController != null)
		{
			UnityEngine.Object.Destroy(_loadingNGUIController.gameObject);
			_loadingNGUIController = null;
		}
		ShowLoadingGUI(_mapName);
	}

	private void ShowLoadingGUI(string _mapName)
	{
		BannerWindowController.SharedController.HideBannerWindowNoShowNext();
		_loadingNGUIController = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("LoadingGUI")).GetComponent<LoadingNGUIController>();
		_loadingNGUIController.SceneToLoad = _mapName;
		_loadingNGUIController.loadingNGUITexture.mainTexture = LoadConnectScene.textureToShow;
		_loadingNGUIController.transform.parent = loadingMapPanel.transform;
		_loadingNGUIController.transform.localPosition = Vector3.zero;
		_loadingNGUIController.Init();
	}

	private void GetMapName(string _mapName, bool isAddCountRun)
	{
		Debug.Log("setFonLoading " + _mapName);
		Texture texture = null;
		if (Defs.isCOOP)
		{
			int @int = PlayerPrefs.GetInt("CountRunCoop", 0);
			bool flag = @int < 5;
			if (isAddCountRun)
			{
				PlayerPrefs.SetInt("CountRunCoop", PlayerPrefs.GetInt("CountRunCoop", 0) + 1);
			}
			Texture texture2 = Resources.Load("NoteLoadings/note_Time_Survival_" + @int % countNoteCaptureCOOP) as Texture;
		}
		else if (Defs.isCompany)
		{
			int int2 = PlayerPrefs.GetInt("CountRunCompany", 0);
			bool flag = int2 < 5;
			Texture texture2 = Resources.Load("NoteLoadings/note_Team_Battle_" + int2 % countNoteCaptureCompany) as Texture;
			if (isAddCountRun)
			{
				PlayerPrefs.SetInt("CountRunCompany", PlayerPrefs.GetInt("CountRunCompany", 0) + 1);
			}
		}
		else if (Defs.isHunger)
		{
			int int3 = PlayerPrefs.GetInt("CountRunHunger", 0);
			bool flag = int3 < 5;
			Texture texture2 = Resources.Load("NoteLoadings/note_Deadly_Games_" + int3 % countNoteCaptureHunger) as Texture;
			if (isAddCountRun)
			{
				PlayerPrefs.SetInt("CountRunHunger", PlayerPrefs.GetInt("CountRunHunger", 0) + 1);
			}
		}
		else if (Defs.isFlag)
		{
			int int4 = PlayerPrefs.GetInt("CountRunFlag", 0);
			bool flag = int4 < 5;
			Texture texture2 = Resources.Load("NoteLoadings/note_Flag_Capture_" + int4 % countNoteCaptureFlag) as Texture;
			if (isAddCountRun)
			{
				PlayerPrefs.SetInt("CountRunFlag", PlayerPrefs.GetInt("CountRunFlag", 0) + 1);
			}
		}
		else
		{
			int int5 = PlayerPrefs.GetInt("CountRunDeadmath", 0);
			bool flag = int5 < 5;
			Texture texture2 = Resources.Load("NoteLoadings/note_Deathmatch_" + int5 % countNoteCaptureDeadmatch) as Texture;
			if (isAddCountRun)
			{
				PlayerPrefs.SetInt("CountRunDeadmath", PlayerPrefs.GetInt("CountRunDeadmath", 0) + 1);
			}
		}
		LoadConnectScene.textureToShow = Resources.Load("LevelLoadings" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/Loading_" + _mapName) as Texture2D;
		LoadingInAfterGame.loadingTexture = LoadConnectScene.textureToShow;
		LoadConnectScene.sceneToLoad = _mapName;
		LoadConnectScene.noteToShow = null;
		loadingToDraw.gameObject.SetActive(false);
		loadingToDraw.mainTexture = null;
	}

	private IEnumerator MoveToGameScene(string _goMapName)
	{
		Debug.Log("MoveToGameScene=" + _goMapName);
		Defs.isGameFromFriends = false;
		Defs.isGameFromClans = false;
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(_goMapName) ? Defs.filterMaps[_goMapName] : 0);
		}
		GlobalGameController.countKillsBlue = 0;
		GlobalGameController.countKillsRed = 0;
		while (PhotonNetwork.room == null)
		{
			yield return 0;
		}
		PlayerPrefs.SetString("RoomName", PhotonNetwork.room.name);
		PlayerPrefs.SetInt("CustomGame", 0);
		PhotonNetwork.isMessageQueueRunning = false;
		mapPreview.Clear();
		yield return null;
		yield return Resources.UnloadUnusedAssets();
		yield return StartCoroutine(SetFonLoadingWaitForReset(_goMapName, true));
		loadingMapPanel.SetActive(true);
		isGoInPhotonGame = true;
		AsyncOperation async = Singleton<SceneLoader>.Instance.LoadSceneAsync("PromScene");
		if (FriendsController.sharedController != null)
		{
			FriendsController.sharedController.GetFriendsData(false);
		}
		yield return async;
		for (int i = 0; i < grid.transform.childCount; i++)
		{
			UnityEngine.Object.Destroy(grid.transform.GetChild(i).gameObject);
		}
		mapPreview.Clear();
	}

	[Obfuscation(Exclude = true)]
	private void ConnectToPhoton()
	{
		if (FriendsController.sharedController != null && FriendsController.sharedController.Banned == 1)
		{
			return;
		}
		if (PhotonNetwork.connectionState == ConnectionState.Connecting || PhotonNetwork.connectionState == ConnectionState.Connected)
		{
			Debug.Log("ConnectToPhoton return");
			return;
		}
		Debug.Log("ConnectToPhoton");
		if (FriendsController.sharedController != null && FriendsController.sharedController.Banned == 1)
		{
			timerShowBan = 3f;
			return;
		}
		isConnectingToPhoton = true;
		isCancelConnectingToPhoton = false;
		gameTier = ((!(ExpController.Instance != null)) ? 1 : ExpController.Instance.OurTier);
		if (Defs.useSqlLobby)
		{
			PhotonNetwork.lobby = new TypedLobby("PixelGun3D", LobbyType.SqlLobby);
		}
		PhotonNetwork.ConnectUsingSettings(Initializer.Separator + regim.ToString() + (Defs.isDaterRegim ? "Dater" : ((!Defs.isHunger) ? gameTier.ToString() : "0")) + "v" + GlobalGameController.MultiplayerProtocolVersion);
	}

	private void StartSearchLocalServers()
	{
		lanScan.StartSearchBroadCasting(SeachServer);
	}

	private void SeachServer(string ipServerSeaches)
	{
		bool flag = false;
		if (servers.Count > 0)
		{
			foreach (infoServer server in servers)
			{
				if (server.ipAddress.Equals(ipServerSeaches))
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			infoServer item = default(infoServer);
			item.ipAddress = ipServerSeaches;
			servers.Add(item);
		}
	}

	private int LocalServerComparison(LANBroadcastService.ReceivedMessage msg1, LANBroadcastService.ReceivedMessage msg2)
	{
		return msg1.ipAddress.CompareTo(msg2.ipAddress);
	}

	private void SetLocalRoomInfo(GameInfo _gameInfo, LANBroadcastService.ReceivedMessage _roomInfo)
	{
		string text = _roomInfo.name;
		if (string.IsNullOrEmpty(text))
		{
			text = LocalizationStore.Get("Key_0948");
		}
		_gameInfo.serverNameLabel.text = text;
		_gameInfo.countPlayersLabel.text = _roomInfo.connectedPlayers + "/" + _roomInfo.playerLimit;
		_gameInfo.openSprite.SetActive(true);
		_gameInfo.closeSprite.SetActive(false);
		string map = _roomInfo.map;
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(_roomInfo.map);
		string text2 = infoScene.TranslatePreviewName.ToUpper();
		_gameInfo.mapNameLabel.SetText(text2);
		_gameInfo.roomInfoLocal = _roomInfo;
		_gameInfo.mapTexture.mainTexture = mapPreview[infoScene.NameScene];
		_gameInfo.SizeMapNameLbl[0].SetActive(infoScene.sizeMap == InfoSizeMap.small);
		_gameInfo.SizeMapNameLbl[1].SetActive(infoScene.sizeMap == InfoSizeMap.normal);
		_gameInfo.SizeMapNameLbl[2].SetActive(infoScene.sizeMap == InfoSizeMap.big || infoScene.sizeMap == InfoSizeMap.veryBig);
	}

	private void UpdateLocalServersList()
	{
		List<LANBroadcastService.ReceivedMessage> list = new List<LANBroadcastService.ReceivedMessage>();
		for (int i = 0; i < lanScan.lstReceivedMessages.Count; i++)
		{
			bool flag = Defs.filterMaps.ContainsKey(lanScan.lstReceivedMessages[i].map) && Defs.filterMaps[lanScan.lstReceivedMessages[i].map] == 3;
			if (((Defs.isDaterRegim && flag) || (!Defs.isDaterRegim && !flag)) && lanScan.lstReceivedMessages[i].regim == (int)regim)
			{
				list.Add(lanScan.lstReceivedMessages[i]);
			}
		}
		countNote = 50;
		if (list.Count < countNote)
		{
			countNote = list.Count;
		}
		while (countNote < gamesInfo.Count)
		{
			UnityEngine.Object.Destroy(gamesInfo[gamesInfo.Count - 1]);
			gamesInfo.RemoveAt(gamesInfo.Count - 1);
		}
		if (countNote > gamesInfo.Count)
		{
			countColumn = ((!((double)((float)Screen.width / (float)Screen.height) < 1.5)) ? 4 : 3);
			_widthCell = (fonGames.localSize.x - (float)(countColumn * 10)) / (float)countColumn;
			if (countNote > gamesInfo.Count)
			{
				gameInfoItemPrefab.SetActive(true);
			}
			while (countNote > gamesInfo.Count)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(gameInfoItemPrefab);
				gameObject.name = "GameInfo_" + gamesInfo.Count;
				gameObject.transform.SetParent(gridGamesTransform, false);
				_scale = _widthCell / gameObject.GetComponent<GameInfo>().mapTexture.localSize.x;
				_heightCell = gameObject.GetComponent<GameInfo>().mapTexture.localSize.y * _scale;
				gameObject.transform.GetChild(0).localScale = new Vector3(_scale, _scale, _scale);
				gamesInfo.Add(gameObject);
				gameInfoItemPrefab.SetActive(false);
			}
			if (gameInfoItemPrefab.activeSelf)
			{
				gameInfoItemPrefab.SetActive(false);
			}
			gridGames.GetComponent<UIGrid>().cellWidth = _widthCell + borderWidth;
			gridGames.GetComponent<UIGrid>().cellHeight = _heightCell + borderWidth;
			gridGames.GetComponent<UIGrid>().maxPerLine = countColumn;
		}
		float num = scrollGames.bounds.extents.y * 2f;
		float y = scrollGames.GetComponent<UIPanel>().GetViewSize().y;
		gridGames.Reposition();
		if (!isFirstGamesReposition || num < y)
		{
			gridGames.transform.localPosition = new Vector3((0f - (_widthCell + borderWidth)) * ((float)countColumn * 0.5f - 0.5f), gridGames.transform.localPosition.y, gridGames.transform.localPosition.z);
			scrollGames.ResetPosition();
			isFirstGamesReposition = true;
		}
		for (int j = 0; j < countNote; j++)
		{
			SetLocalRoomInfo(gamesInfo[j].GetComponent<GameInfo>(), list[j]);
		}
	}

	public void JoinToLocalRoom(LANBroadcastService.ReceivedMessage _roomInfo)
	{
		if (_roomInfo.connectedPlayers == _roomInfo.playerLimit)
		{
			gameIsfullLabel.timer = 3f;
			gameIsfullLabel.gameObject.SetActive(true);
			incorrectPasswordLabel.timer = -1f;
			incorrectPasswordLabel.gameObject.SetActive(false);
			return;
		}
		GlobalGameController.countKillsBlue = 0;
		GlobalGameController.countKillsRed = 0;
		Defs.ServerIp = _roomInfo.ipAddress;
		PlayerPrefs.SetString("MaxKill", _roomInfo.comment);
		PlayerPrefs.SetString("MapName", _roomInfo.map);
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(_roomInfo.map) ? Defs.filterMaps[_roomInfo.map] : 0);
		}
		StartCoroutine(SetFonLoadingWaitForReset(_roomInfo.map));
		Invoke("goGame", 0.1f);
	}

	private bool CheckLocalAvailability()
	{
		if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			return true;
		}
		return false;
	}

	public void JoinToRoomPhoton(RoomInfo _roomInfo)
	{
		if (_roomInfo.playerCount == _roomInfo.maxPlayers)
		{
			gameIsfullLabel.timer = 3f;
			gameIsfullLabel.gameObject.SetActive(true);
			incorrectPasswordLabel.timer = -1f;
			incorrectPasswordLabel.gameObject.SetActive(false);
			return;
		}
		joinRoomInfoFromCustom = _roomInfo;
		if (string.IsNullOrEmpty(_roomInfo.customProperties[passwordProperty].ToString()))
		{
			JoinToRoomPhotonAfterCheck();
			return;
		}
		gameIsfullLabel.timer = -1f;
		gameIsfullLabel.gameObject.SetActive(false);
		incorrectPasswordLabel.timer = 3f;
		incorrectPasswordLabel.gameObject.SetActive(true);
		enterPasswordInput.value = string.Empty;
		enterPasswordPanel.SetActive(true);
		enterPasswordInput.isSelected = false;
		enterPasswordInput.isSelected = true;
		if (ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.isShowRanks = false;
		}
		customPanel.SetActive(false);
	}

	private void EnterPassInputSubmit()
	{
		enterPasswordInput.RemoveFocus();
		enterPasswordInput.isSelected = false;
		Invoke("EnterPassInput", 0.1f);
	}

	private void EnterPassInput()
	{
		HandleJoinRoomFromEnterPasswordBtnClicked(null, null);
	}

	public void JoinToRoomPhotonAfterCheck()
	{
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(joinRoomInfoFromCustom.customProperties[mapProperty].ToString()));
		StartCoroutine(SetFonLoadingWaitForReset(infoScene.NameScene));
		loadingMapPanel.SetActive(true);
		PhotonNetwork.JoinRoom(joinRoomInfoFromCustom.name);
		ActivityIndicator.IsActiveIndicator = true;
	}

	private void SetPosSelectMapPanelInMainMenu()
	{
		if (!Defs.isDaterRegim)
		{
			if (posSelectMapPanelInMainMenu.y < 9000f)
			{
				selectMapPanelTransform.localPosition = posSelectMapPanelInMainMenu;
			}
		}
		else
		{
			selectMapPanelTransform.localPosition = Vector3.zero;
		}
	}

	private void SetPosSelectMapPanelInCreatePanel()
	{
		posSelectMapPanelInMainMenu = selectMapPanelTransform.localPosition;
		selectMapPanelTransform.localPosition = Vector3.zero;
		if (Defs.isDaterRegim)
		{
			selectMapPanelTransform.localPosition = new Vector3(0f, -90f, 0f);
		}
	}

	[Obfuscation(Exclude = true)]
	private void goGame()
	{
		WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(PlayerPrefs.GetString("MapName")) ? Defs.filterMaps[PlayerPrefs.GetString("MapName")] : 0);
		Singleton<SceneLoader>.Instance.LoadScene(PlayerPrefs.GetString("MapName"));
	}

	private void Awake()
	{
		abTestConnect = Defs.isActivABTestBuffSystem;
		if (isReturnFromGame)
		{
			Defs.countReturnInConnectScene++;
		}
		PhotonObjectCacher.AddObject(base.gameObject);
		setPasswordInput.onSubmit.Add(new EventDelegate(delegate
		{
			OnPaswordSelected();
		}));
		SceneInfoController.instance.UpdateListAvaliableMap();
	}

	private void OnDestroy()
	{
		Debug.Log("OnDestroy ConnectSceneController");
		if (!Defs.isInet || (!isGoInPhotonGame && PhotonNetwork.connectionState == ConnectionState.Connected) || PhotonNetwork.connectionState == ConnectionState.Connecting)
		{
			PhotonNetwork.isMessageQueueRunning = true;
			PhotonNetwork.Disconnect();
			Debug.Log("PhotonNetwork.Disconnect()");
		}
		if (ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.isShowRanks = false;
			ExperienceController.sharedController.isMenu = false;
			ExperienceController.sharedController.isConnectScene = false;
		}
		lanScan.StopBroadCasting();
		sharedController = null;
		PhotonObjectCacher.RemoveObject(base.gameObject);
	}

	public void HandleShopClicked()
	{
		if (!ShopNGUIController.GuiActive && !MainMenuController.IsLevelUpOrBannerShown() && (!(connectToPhotonPanel != null) || !connectToPhotonPanel.activeInHierarchy))
		{
			ShopNGUIController.sharedShop.SetInGame(false);
			ShopNGUIController.GuiActive = true;
			ShopNGUIController.sharedShop.resumeAction = HandleResumeFromShop;
		}
	}

	public void HandleResumeFromShop()
	{
		ShopNGUIController.GuiActive = false;
		ShopNGUIController.sharedShop.resumeAction = delegate
		{
		};
		StartCoroutine(MainMenuController.ShowRanks());
	}
}

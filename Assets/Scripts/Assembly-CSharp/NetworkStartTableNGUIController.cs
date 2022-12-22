using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Facebook.Unity;
using Rilisoft;
using UnityEngine;

public sealed class NetworkStartTableNGUIController : MonoBehaviour
{
	public static NetworkStartTableNGUIController sharedController;

	public GameObject facebookButton;

	public GameObject twitterButton;

	public Transform rentScreenPoint;

	public GameObject ranksInterface;

	public RanksTable ranksTable;

	public GameObject shopAnchor;

	public GameObject finishedInterface;

	public UILabel[] finishedInterfaceLabels;

	public GameObject startInterfacePanel;

	public GameObject winnerPanelCom1;

	public GameObject winnerPanelCom2;

	public GameObject endInterfacePanel;

	public Animator interfaceAnimator;

	public GameObject allInterfacePanel;

	public GameObject randomBtn;

	public GameObject socialPnl;

	public GameObject spectratorModePnl;

	public GameObject spectatorModeBtnPnl;

	public GameObject spectatorModeOnBtn;

	public GameObject spectatorModeOffBtn;

	public GameObject MapSelectPanel;

	public string winner;

	public int winnerCommand;

	public UILabel HungerStartLabel;

	private int addCoins;

	private int addExperience;

	private bool isCancelHideAvardPanel;

	private bool updateRealTableAfterActionPanel = true;

	public GameObject SexualButton;

	public GameObject InAppropriateActButton;

	public GameObject OtherButton;

	public GameObject ReasonsPanel;

	public GameObject ActionPanel;

	public GameObject AddButton;

	public GameObject ReportButton;

	public GameObject questsButton;

	public GameObject hideOldRanksButton;

	public GameObject rewardButton;

	public GameObject shopButton;

	public GameObject labelNewItems;

	public UILabel[] actionPanelNicklabel;

	public GameObject trophiAddIcon;

	public GameObject trophiMinusIcon;

	public string pixelbookID;

	public string nick;

	public GoMapInEndGame[] goMapInEndGameButtons = new GoMapInEndGame[3];

	public GoMapInEndGame[] goMapInEndGameButtonsDuel = new GoMapInEndGame[2];

	public int CountAddFriens;

	public UILabel[] totalBlue;

	public UILabel[] totalRed;

	private GameObject cameraObj;

	public GameObject changeMapLabel;

	public GameObject rewardPanel;

	public GameObject listOfPlayers;

	public GameObject backButtonInHunger;

	public GameObject goBattleLabel;

	public GameObject daterButtonLabel;

	public UITexture rewardCoinsObject;

	public UITexture rewardExpObject;

	public UISprite rewardTrophysObject;

	public UITexture[] trophyItems;

	public UISprite currentCup;

	public UISprite NewCup;

	public GameObject trophyPanel;

	public GameObject trophyShine;

	public UISprite currentBar;

	public UISprite nextBar;

	public UILabel trophyPoints;

	public Transform rewardCoinsAnimPoint;

	public Transform rewardExpAnimPoint;

	public UILabel[] rewardCoins;

	public UILabel[] rewardExperience;

	public UILabel[] gameModeLabel;

	public UILabel[] rewardTrophy;

	public GameObject[] finishWin;

	public GameObject[] finishDefeat;

	public GameObject[] finishDraw;

	public UILabel teamOneLabel;

	public UILabel teamTwoLabel;

	private Vector3 defaultTeamOneState;

	private Vector3 defaultTeamTwoState;

	public UIToggle shareToggle;

	public UILabel[] textLeagueUp;

	public UILabel[] textLeagueDown;

	public GameObject duelPanel;

	[HideInInspector]
	public DuelUIController duelUI;

	public FrameResizer rewardFrame;

	public bool isRewardShow;

	public RuntimeAnimatorController standartAnimController;

	public RuntimeAnimatorController DuelAnimController;

	private readonly Lazy<string> _versionString = new Lazy<string>(() => typeof(NetworkStartTableNGUIController).Assembly.GetName().Version.ToString());

	private IDisposable _backSubscription;

	private bool waitForAnimationDone;

	private bool leagueUp;

	private int expRewardValue;

	private int coinsRewardValue;

	private int trophyRewardValue;

	private float currentBarFillAmount;

	private float nextBarFillAmount;

	private bool isUsed;

	private bool waitForTrophyAnimationDone;

	private bool oldRanksIsActive;

	private FacebookController.StoryPriority _facebookPriority;

	private FacebookController.StoryPriority _twiiterPriority;

	public Action shareAction;

	public Action customHide;

	public RewardWindowBase rewardWindow { get; set; }

	public FacebookController.StoryPriority facebookPriority
	{
		get
		{
			return _facebookPriority;
		}
		set
		{
			_facebookPriority = value;
		}
	}

	public FacebookController.StoryPriority twitterPriority
	{
		get
		{
			return _twiiterPriority;
		}
		set
		{
			_twiiterPriority = value;
		}
	}

	public FacebookController.StoryPriority faceBookPriority
	{
		set
		{
			facebookPriority = value;
			twitterPriority = value;
		}
	}

	public string EventTitle { get; set; }

	public Func<string> twitterStatus { get; set; }

	private void Awake()
	{
		interfaceAnimator.runtimeAnimatorController = ((!Defs.isDuel) ? standartAnimController : DuelAnimController);
		sharedController = this;
	}

	private void OnDestroy()
	{
		sharedController = null;
	}

	private void Start()
	{
		if (BuffSystem.instance != null && !BuffSystem.instance.haveAllInteractons && Storager.getInt("Training.ShouldRemoveNoviceArmorInShopKey", false) == 1 && HintController.instance != null)
		{
			HintController.instance.ShowHintByName("shop_remove_novice_armor", 0f);
		}
		cameraObj = base.transform.GetChild(0).gameObject;
		if (SexualButton != null)
		{
			ButtonHandler component = SexualButton.GetComponent<ButtonHandler>();
			if (component != null)
			{
				component.Clicked += SexualButtonHandler;
			}
		}
		if (InAppropriateActButton != null)
		{
			ButtonHandler component2 = InAppropriateActButton.GetComponent<ButtonHandler>();
			if (component2 != null)
			{
				component2.Clicked += InAppropriateActButtonHandler;
			}
		}
		if (OtherButton != null)
		{
			ButtonHandler component3 = OtherButton.GetComponent<ButtonHandler>();
			if (component3 != null)
			{
				component3.Clicked += OtherButtonHandler;
			}
		}
		if (ReportButton != null)
		{
			ButtonHandler component4 = ReportButton.GetComponent<ButtonHandler>();
			if (component4 != null)
			{
				component4.Clicked += ShowReasonPanel;
			}
		}
		if (AddButton != null)
		{
			ButtonHandler component5 = AddButton.GetComponent<ButtonHandler>();
			if (component5 != null)
			{
				component5.Clicked += AddButtonHandler;
			}
		}
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints)
		{
			listOfPlayers.transform.localPosition -= 50f * Vector3.up;
			if (NetworkStartTable.LocalOrPasswordRoom())
			{
				MapSelectPanel.transform.localPosition += 80f * Vector3.up;
			}
		}
		if (duelPanel != null)
		{
			duelUI = duelPanel.GetComponent<DuelUIController>();
		}
	}

	private void Update()
	{
		if (ExpController.Instance != null && ExpController.Instance.experienceView != null)
		{
			bool levelUpPanelOpened = ExpController.Instance.experienceView.LevelUpPanelOpened;
			if (cameraObj.activeSelf == levelUpPanelOpened)
			{
				cameraObj.SetActive(!levelUpPanelOpened);
			}
		}
		if ((Defs.isHunger || Defs.isRegimVidosDebug) && spectatorModeBtnPnl.activeSelf && Initializer.players.Count == 0)
		{
			spectatorModeBtnPnl.SetActive(false);
			spectratorModePnl.SetActive(false);
			ShowTable(false);
		}
		facebookButton.SetActive(FacebookController.FacebookSupported && FacebookController.FacebookPost_Old_Supported && FB.IsLoggedIn);
		twitterButton.SetActive(TwitterController.TwitterSupported && TwitterController.TwitterSupported_OldPosts && TwitterController.IsLoggedIn);
		bool flag = facebookButton.activeSelf || twitterButton.activeSelf;
		if (socialPnl.activeSelf != flag)
		{
			socialPnl.SetActive(flag);
		}
	}

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(HandleEscape, "Network Start Table GUI");
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	public void HandleEscape()
	{
		if (ReasonsPanel != null && ReasonsPanel.activeInHierarchy)
		{
			BackFromReasonPanel();
		}
		else if (ActionPanel != null && ActionPanel.activeInHierarchy)
		{
			CancelFromActionPanel();
		}
		else if (ShopNGUIController.GuiActive)
		{
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myTable != null)
			{
				WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().HandleResumeFromShop();
			}
		}
		else if (hideOldRanksButton.activeInHierarchy)
		{
			List<EventDelegate> onClick = hideOldRanksButton.GetComponent<UIButton>().onClick;
			EventDelegate.Execute(onClick);
		}
	}

	public void ShowActionPanel(string _pixelbookID, string _nick)
	{
		pixelbookID = _pixelbookID;
		nick = _nick;
		HideTable();
		for (int i = 0; i < actionPanelNicklabel.Length; i++)
		{
			actionPanelNicklabel[i].text = nick;
		}
		ActionPanel.SetActive(true);
		spectatorModeBtnPnl.SetActive(false);
		if (FriendsController.sharedController.IsShowAdd(pixelbookID) && CountAddFriens < 3)
		{
			AddButton.GetComponent<UIButton>().isEnabled = true;
		}
		else
		{
			AddButton.GetComponent<UIButton>().isEnabled = false;
		}
	}

	public void HideActionPanel()
	{
		ActionPanel.SetActive(false);
		ShowTable(updateRealTableAfterActionPanel);
		if ((Defs.isHunger || Defs.isRegimVidosDebug) && Initializer.players.Count > 0)
		{
			spectatorModeBtnPnl.SetActive(Initializer.players.Count != 0);
		}
	}

	public void ShowReasonPanel(object sender, EventArgs e)
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			Debug.Log("ShowReasonPanel");
			ReasonsPanel.SetActive(true);
			ActionPanel.SetActive(false);
		}
	}

	public void HideReasonPanel()
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			ReasonsPanel.SetActive(false);
			ActionPanel.SetActive(true);
		}
	}

	public bool CheckHideInternalPanel()
	{
		if (ActionPanel.activeInHierarchy)
		{
			CancelFromActionPanel();
			return true;
		}
		if (ReasonsPanel.activeInHierarchy)
		{
			BackFromReasonPanel();
			return true;
		}
		return false;
	}

	public void AddButtonHandler(object sender, EventArgs e)
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			Debug.Log("[Add] " + pixelbookID);
			CountAddFriens++;
			string value = ((!Defs.isDaterRegim) ? "Multiplayer Battle" : "Sandbox (Dating)");
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Added Friends", value);
			dictionary.Add("Deleted Friends", "Add");
			Dictionary<string, object> socialEventParameters = dictionary;
			FriendsController.sharedController.SendInvitation(pixelbookID, socialEventParameters);
			if (!FriendsController.sharedController.notShowAddIds.Contains(pixelbookID))
			{
				FriendsController.sharedController.notShowAddIds.Add(pixelbookID);
			}
			AddButton.GetComponent<UIButton>().isEnabled = false;
		}
	}

	public void CancelFromActionPanel()
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			HideActionPanel();
		}
	}

	public void BackFromReasonPanel()
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			HideReasonPanel();
		}
	}

	public void InAppropriateActButtonHandler(object sender, EventArgs e)
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			Action handler = delegate
			{
				string value = _versionString.Value;
				string text = string.Concat("mailto:", Defs.SupportMail, "?subject=INAPPROPRIATE ACT ", nick, "(", pixelbookID, ")&body=%0D%0A%0D%0A%0D%0A%0D%0A%0D%0A------------%20DO NOT DELETE%20------------%0D%0AUTC%20Time:%20", DateTime.Now.ToString(), "%0D%0AGame:%20PixelGun3D%0D%0AVersion:%20", value, "%0D%0APlayerID:%20", FriendsController.sharedController.id, "%0D%0ACategory:%20INAPPROPRIATE ACT ", nick, "(", pixelbookID, ")%0D%0ADevice%20Type:%20", SystemInfo.deviceType, "%20", SystemInfo.deviceModel, "%0D%0AOS%20Version:%20", SystemInfo.operatingSystem, "%0D%0A------------------------");
				text = text.Replace(" ", "%20");
				Application.OpenURL(text);
			};
			FeedbackMenuController.ShowDialogWithCompletion(handler);
		}
	}

	public void SexualButtonHandler(object sender, EventArgs e)
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			Action handler = delegate
			{
				string value = _versionString.Value;
				string text = string.Concat("mailto:", Defs.SupportMail, "?subject=CHEATING ", nick, "(", pixelbookID, ")&body=%0D%0A%0D%0A%0D%0A%0D%0A%0D%0A------------%20DO NOT DELETE%20------------%0D%0AUTC%20Time:%20", DateTime.Now.ToString(), "%0D%0AGame:%20PixelGun3D%0D%0AVersion:%20", value, "%0D%0APlayerID:%20", FriendsController.sharedController.id, "%0D%0ACategory:%20CHEATING ", nick, "(", pixelbookID, ")%0D%0ADevice%20Type:%20", SystemInfo.deviceType, "%20", SystemInfo.deviceModel, "%0D%0AOS%20Version:%20", SystemInfo.operatingSystem, "%0D%0A------------------------");
				text = text.Replace(" ", "%20");
				Application.OpenURL(text);
			};
			FeedbackMenuController.ShowDialogWithCompletion(handler);
		}
	}

	public void OtherButtonHandler(object sender, EventArgs e)
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			Action handler = delegate
			{
				string value = _versionString.Value;
				string text = string.Concat("mailto:", Defs.SupportMail, "?subject=Report ", nick, "(", pixelbookID, ")&body=%0D%0A%0D%0A%0D%0A%0D%0A%0D%0A------------%20DO NOT DELETE%20------------%0D%0AUTC%20Time:%20", DateTime.Now.ToString(), "%0D%0AGame:%20PixelGun3D%0D%0AVersion:%20", value, "%0D%0APlayerID:%20", FriendsController.sharedController.id, "%0D%0ACategory:%20Report ", nick, "(", pixelbookID, ")%0D%0ADevice%20Type:%20", SystemInfo.deviceType, "%20", SystemInfo.deviceModel, "%0D%0AOS%20Version:%20", SystemInfo.operatingSystem, "%0D%0A------------------------");
				text = text.Replace(" ", "%20");
				Application.OpenURL(text);
			};
			FeedbackMenuController.ShowDialogWithCompletion(handler);
		}
	}

	public void StartSpectatorMode()
	{
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.aimPanel.SetActive(true);
		}
		spectatorModeOnBtn.SetActive(true);
		spectatorModeOffBtn.SetActive(false);
		spectratorModePnl.SetActive(true);
		socialPnl.SetActive(false);
		MapSelectPanel.SetActive(false);
		HideTable();
		if (WeaponManager.sharedManager.myTable != null)
		{
			WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().isRegimVidos = true;
		}
	}

	public void EndSpectatorMode()
	{
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.aimPanel.SetActive(false);
		}
		spectatorModeOnBtn.SetActive(false);
		spectatorModeOffBtn.SetActive(true);
		spectratorModePnl.SetActive(false);
		MapSelectPanel.SetActive(true);
		if (WeaponManager.sharedManager.myTable != null)
		{
			if (WeaponManager.sharedManager.myNetworkStartTable.currentGameObjectPlayer != null)
			{
				Player_move_c.SetLayerRecursively(WeaponManager.sharedManager.myNetworkStartTable.currentGameObjectPlayer.transform.GetChild(0).gameObject, 0);
			}
			WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().isRegimVidos = false;
		}
		ShowTable(true);
	}

	[Obfuscation(Exclude = true)]
	public void HideAvardPanel()
	{
		if (!isCancelHideAvardPanel)
		{
			rewardWindow = null;
			ShowEndInterface(winner, winnerCommand);
			if (WeaponManager.sharedManager.myTable != null)
			{
				WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().isShowAvard = false;
			}
			else
			{
				UnityEngine.Object.Destroy(sharedController.gameObject);
			}
			isCancelHideAvardPanel = true;
		}
	}

	public static RewardWindowBase ShowRewardWindow(bool win, Transform par)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("NguiWindows/WinWindowNGUI"));
		RewardWindowBase component = gameObject.GetComponent<RewardWindowBase>();
		FacebookController.StoryPriority priority = FacebookController.StoryPriority.Red;
		component.priority = priority;
		component.twitterPriority = FacebookController.StoryPriority.MultyWinLimit;
		component.shareAction = delegate
		{
			FacebookController.PostOpenGraphStory("win", "battle", priority, new Dictionary<string, string> { 
			{
				"battle",
				ConnectSceneNGUIController.regim.ToString().ToLower()
			} });
		};
		component.HasReward = true;
		component.CollectOnlyNoShare = !win;
		component.twitterStatus = () => "I've won a battle in @PixelGun3D! Join the fight now! #pixelgun3d #pixelgun #3d #pg3d #mobile #fps #shooter http://goo.gl/8fzL9u";
		component.EventTitle = "Won Batlle";
		gameObject.transform.parent = par;
		Player_move_c.SetLayerRecursively(gameObject, LayerMask.NameToLayer("NGUITable"));
		gameObject.transform.localPosition = new Vector3(0f, 0f, -130f);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		return component;
	}

	public void ShowFinishedInterface(bool isWinner, bool deadHeat)
	{
		bool flag = ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints;
		finishedInterface.SetActive(true);
		string text = LocalizationStore.Get(Defs.isDaterRegim ? "Key_1987" : (deadHeat ? ((!Defs.isDuel || Initializer.networkTables.Count >= 2) ? "Key_0571" : "Key_2436") : (isWinner ? "Key_1115" : ((!flag) ? "Key_1976" : "Key_1116"))));
		GameObject[] array = finishDraw;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive((deadHeat || (!isWinner && !flag)) && !Defs.isDaterRegim);
		}
		GameObject[] array2 = finishWin;
		foreach (GameObject gameObject2 in array2)
		{
			gameObject2.SetActive(isWinner && !deadHeat && !Defs.isDaterRegim);
		}
		GameObject[] array3 = finishDefeat;
		foreach (GameObject gameObject3 in array3)
		{
			gameObject3.SetActive(flag && !isWinner && !deadHeat && !Defs.isDaterRegim);
		}
		for (int l = 0; l < finishedInterfaceLabels.Length; l++)
		{
			finishedInterfaceLabels[l].text = text;
		}
		finishedInterfaceLabels[0].gameObject.SetActive(true);
	}

	[Obsolete]
	public void showAvardPanel(string _winner, int _addCoin, int _addExpierence, bool _isCustom, bool firstPlace, int _winnerCommand)
	{
		isCancelHideAvardPanel = false;
		if (_isCustom)
		{
			addCoins = 0;
			addExperience = 0;
		}
		else
		{
			addCoins = _addCoin;
			addExperience = _addExpierence;
		}
		string text = string.Format("+ {0} {1}", _addCoin, LocalizationStore.Key_0275);
		string text2 = string.Format("+ {0} {1}", _addExpierence, LocalizationStore.Key_0204);
		ConnectSceneNGUIController.RegimGame regim = ConnectSceneNGUIController.regim;
		PremiumAccountController instance = PremiumAccountController.Instance;
		bool flag = regim == ConnectSceneNGUIController.RegimGame.Deathmatch || regim == ConnectSceneNGUIController.RegimGame.FlagCapture || regim == ConnectSceneNGUIController.RegimGame.TeamFight || regim == ConnectSceneNGUIController.RegimGame.CapturePoints;
		bool flag2 = PromoActionsManager.sharedManager.IsDayOfValorEventActive && flag;
		bool flag3 = instance.IsActiveOrWasActiveBeforeStartMatch();
		int num = 1;
		int num2 = 1;
		if (flag3 || flag2)
		{
			num = ((!Defs.isCOOP && !Defs.isHunger) ? AdminSettingsController.GetMultiplyerRewardWithBoostEvent(false) : PremiumAccountController.Instance.RewardCoeff);
			num2 = ((!Defs.isCOOP && !Defs.isHunger) ? AdminSettingsController.GetMultiplyerRewardWithBoostEvent(true) : PremiumAccountController.Instance.RewardCoeff);
		}
		rewardWindow = ShowRewardWindow(firstPlace, sharedController.allInterfacePanel.transform.parent);
		rewardWindow.customHide = delegate
		{
			rewardWindow.CancelInvoke("Hide");
			HideAvardPanel();
		};
		RewardWindowAfterMatch component = rewardWindow.GetComponent<RewardWindowAfterMatch>();
		component.victory.SetActive(true);
		component.lose.SetActive(false);
		if (flag3 && flag2)
		{
			component.daysAndPremiumBack.SetActive(true);
			component.premiumBackground.SetActive(false);
			component.daysOfValorBackground.SetActive(false);
			component.normlaBeckground.SetActive(false);
		}
		else if (flag3)
		{
			component.daysAndPremiumBack.SetActive(false);
			component.premiumBackground.SetActive(true);
			component.daysOfValorBackground.SetActive(false);
			component.normlaBeckground.SetActive(false);
		}
		else if (flag2)
		{
			component.daysAndPremiumBack.SetActive(false);
			component.premiumBackground.SetActive(false);
			component.daysOfValorBackground.SetActive(true);
			component.normlaBeckground.SetActive(false);
		}
		else
		{
			component.daysAndPremiumBack.SetActive(false);
			component.premiumBackground.SetActive(false);
			component.daysOfValorBackground.SetActive(false);
			component.normlaBeckground.SetActive(true);
		}
		component.coinsMultiplierContainer.SetActive(num2 > 1 && _addCoin > 0);
		component.coinsMultiplier.text = "x" + num2;
		component.expMultiplierContainer.SetActive(num > 1);
		component.expMilyiplier.text = "x" + num;
		foreach (UILabel coin in component.coins)
		{
			coin.text = text;
		}
		foreach (UILabel item in component.exp)
		{
			item.text = text2;
		}
		if (_addCoin == 0)
		{
			component.coinsContainer.SetActive(false);
			component.expContainer.transform.localPosition = new Vector3(0f, component.expContainer.transform.localPosition.y, component.expContainer.transform.localPosition.z);
		}
		endInterfacePanel.SetActive(true);
		finishedInterface.SetActive(false);
		MapSelectPanel.SetActive(false);
		socialPnl.SetActive(BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64);
		winnerCommand = _winnerCommand;
		winner = _winner;
		if (Defs.isDaterRegim)
		{
		}
		if (addExperience > 0)
		{
			ExperienceController.sharedController.addExperience(addExperience);
		}
		if (addCoins > 0)
		{
			int @int = Storager.getInt("Coins", false);
			Storager.setInt("Coins", @int + addCoins, false);
			AnalyticsFacade.CurrencyAccrual(addCoins, "Coins");
		}
	}

	public void ShowFinishedDuelInterface(RatingSystem.RatingChange ratingChange, bool showAward, int _addCoin, int _addExpierence, bool isWinner, bool deadheat)
	{
	}

	public void ShowStartDuelInterface()
	{
		duelUI = duelPanel.GetComponent<DuelUIController>();
		startInterfacePanel.SetActive(false);
		allInterfacePanel.SetActive(false);
		HideTable();
		duelPanel.SetActive(true);
		duelUI.ShowStartInterface();
	}

	public void ShowStartInterface()
	{
		if (Defs.isDuel)
		{
			ShowStartDuelInterface();
			return;
		}
		string value;
		if (Defs.isDaterRegim)
		{
			UILabel[] array = gameModeLabel;
			foreach (UILabel uILabel in array)
			{
				uILabel.text = LocalizationStore.Get("Key_1567");
			}
		}
		else if (ConnectSceneNGUIController.gameModesLocalizeKey.TryGetValue(Convert.ToInt32(ConnectSceneNGUIController.regim).ToString(), out value))
		{
			UILabel[] array2 = gameModeLabel;
			foreach (UILabel uILabel2 in array2)
			{
				uILabel2.text = LocalizationStore.Get(value);
			}
		}
		questsButton.SetActive(TrainingController.TrainingCompleted && !Defs.isHunger);
		MapSelectPanel.SetActive(false);
		goBattleLabel.SetActive(!Defs.isDaterRegim);
		daterButtonLabel.SetActive(Defs.isDaterRegim);
		allInterfacePanel.SetActive(true);
		startInterfacePanel.SetActive(true);
		rewardPanel.SetActive(false);
		isRewardShow = false;
		ShowTable(true);
		StartCoroutine("TryToShowExpiredBanner");
	}

	public void ShowNewMatchInterface()
	{
		isRewardShow = false;
		rewardPanel.SetActive(false);
		allInterfacePanel.SetActive(true);
		startInterfacePanel.SetActive(true);
		ShowTable(true);
	}

	public void HideStartInterface()
	{
		isRewardShow = false;
		rewardPanel.SetActive(false);
		Debug.Log("HideStartInterface");
		finishedInterface.SetActive(false);
		allInterfacePanel.SetActive(false);
		startInterfacePanel.SetActive(false);
		ReasonsPanel.SetActive(false);
		ActionPanel.SetActive(false);
		updateRealTableAfterActionPanel = true;
		HideTable();
		StopCoroutine("TryToShowExpiredBanner");
	}

	public void ShowEndInterfaceDeadInHunger(string _winner, RatingSystem.RatingChange ratingChange)
	{
		StartCoroutine(MatchFinishedInterface(_winner, ratingChange, false, 0, 0, false, false, false, 0, 0, 0, true));
	}

	public void MathFinishedDeadInHunger()
	{
		if (spectratorModePnl.activeSelf)
		{
			EndSpectatorMode();
			return;
		}
		spectatorModeOnBtn.SetActive(false);
		spectatorModeOffBtn.SetActive(true);
		spectratorModePnl.SetActive(false);
	}

	public void FreezePlayer()
	{
		if (WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			WeaponManager.sharedManager.myPlayerMoveC.BlockPlayerInEnd();
			InGameGUI.sharedInGameGUI.gameObject.SetActive(false);
			if (ChatViewrController.sharedController != null)
			{
				UnityEngine.Object.Destroy(ChatViewrController.sharedController.gameObject);
			}
		}
	}

	public IEnumerator MatchFinishedInterface(string _winner, RatingSystem.RatingChange ratingChange, bool showAward, int _addCoin, int _addExpierence, bool _isCustom, bool firstPlace, bool iAmWinnerInTeam, int _winnerCommand, int blueTotal, int redTotal, bool deadInHunger = false)
	{
		for (int i = 0; i < totalBlue.Length; i++)
		{
			totalBlue[i].text = blueTotal.ToString();
		}
		for (int j = 0; j < totalRed.Length; j++)
		{
			totalRed[j].text = redTotal.ToString();
		}
		ranksTable.totalBlue = blueTotal;
		ranksTable.totalRed = redTotal;
		bool isTeamMode = ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints;
		interfaceAnimator.ResetTrigger("RewardTaken");
		interfaceAnimator.ResetTrigger("GetReward");
		interfaceAnimator.SetBool("IsTwoTeams", isTeamMode);
		interfaceAnimator.SetBool("isRewarded", showAward || ratingChange.addRating != 0);
		interfaceAnimator.SetBool("isExpOnly", _addCoin == 0 || (ExperienceController.sharedController.currentLevel == 31 && _addCoin > 0));
		interfaceAnimator.SetBool("isHunger", Defs.isHunger);
		interfaceAnimator.SetBool("isDater", Defs.isDaterRegim);
		interfaceAnimator.SetBool("IsTrophyUp", ratingChange.isUp);
		interfaceAnimator.SetBool("IsTrophyDown", ratingChange.isDown);
		interfaceAnimator.SetBool("IsTrophyAdd", ratingChange.addRating != 0 && !ratingChange.isUp && !ratingChange.isDown);
		interfaceAnimator.SetBool("NoRaiting", ratingChange.addRating == 0);
		interfaceAnimator.SetBool("isTrophyOnly", ratingChange.addRating != 0 && _addCoin == 0 && _addExpierence == 0);
		interfaceAnimator.SetBool("isTrophyAdded", false);
		interfaceAnimator.SetBool("isRatingSystem", true);
		interfaceAnimator.SetBool("interfaceAnimationDone", false);
		isUsed = false;
		trophiAddIcon.SetActive(ratingChange.addRating > 0);
		trophiMinusIcon.SetActive(ratingChange.addRating < 0);
		trophyPanel.SetActive(ratingChange.addRating != 0);
		currentCup.spriteName = ratingChange.oldLeague.ToString() + " " + (3 - ratingChange.oldDivision);
		NewCup.spriteName = ratingChange.newLeague.ToString() + " " + (3 - ratingChange.newDivision);
		if (ratingChange.addRating > 0)
		{
			currentBarFillAmount = ratingChange.oldRatingAmount;
			nextBarFillAmount = ratingChange.newRatingAmount;
			currentBar.fillAmount = currentBarFillAmount;
			nextBar.fillAmount = currentBarFillAmount;
			nextBar.color = Color.yellow;
		}
		else
		{
			currentBarFillAmount = ratingChange.oldRatingAmount;
			nextBarFillAmount = ratingChange.newRatingAmount;
			currentBar.fillAmount = nextBarFillAmount;
			nextBar.fillAmount = nextBarFillAmount;
			nextBar.color = Color.red;
		}
		leagueUp = ratingChange.newLeague > ratingChange.oldLeague;
		if (ratingChange.maxRating == int.MaxValue)
		{
			trophyPoints.text = ratingChange.newRating.ToString();
		}
		else
		{
			trophyPoints.text = ratingChange.newRating + "/" + ratingChange.maxRating;
		}
		trophyShine.SetActive(ratingChange.isUp);
		trophyRewardValue = ratingChange.addRating;
		string trophyAwardText = string.Format((ratingChange.addRating <= 0) ? "{0}" : "+{0}", trophyRewardValue);
		UILabel[] array = rewardTrophy;
		foreach (UILabel label in array)
		{
			label.gameObject.SetActive(ratingChange.addRating != 0);
			label.text = trophyAwardText;
		}
		string leagueChangeText = string.Format(LocalizationStore.Get(RatingSystem.leagueChangeLocalizations[(int)ratingChange.newLeague]), RatingSystem.divisionByIndex[ratingChange.newDivision]);
		UILabel[] array2 = textLeagueUp;
		foreach (UILabel label2 in array2)
		{
			label2.text = leagueChangeText;
		}
		UILabel[] array3 = textLeagueDown;
		foreach (UILabel label3 in array3)
		{
			label3.text = leagueChangeText;
		}
		shareToggle.value = firstPlace && showAward && (FB.IsLoggedIn || TwitterController.IsLoggedIn);
		shareToggle.gameObject.SetActive(shareToggle.value);
		if (defaultTeamOneState == Vector3.zero)
		{
			defaultTeamOneState = teamOneLabel.transform.localPosition;
		}
		if (defaultTeamTwoState == Vector3.zero)
		{
			defaultTeamTwoState = teamTwoLabel.transform.localPosition;
		}
		if (!isTeamMode || _winnerCommand == 0)
		{
			teamOneLabel.transform.localPosition = defaultTeamOneState;
			teamTwoLabel.transform.localPosition = defaultTeamTwoState;
		}
		else
		{
			teamOneLabel.transform.localPosition = defaultTeamOneState + Vector3.right * 55f;
			teamTwoLabel.transform.localPosition = defaultTeamTwoState + Vector3.left * 55f;
		}
		if (Defs.isHunger)
		{
			EndSpectatorMode();
			HideTable();
		}
		if (WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			FreezePlayer();
			interfaceAnimator.SetTrigger("MatchEnd");
			shopButton.GetComponent<Collider>().enabled = Defs.isDaterRegim;
			ShowFinishedInterface(iAmWinnerInTeam, (Defs.isCompany || Defs.isFlag || Defs.isCapturePoints) && _winnerCommand == 0);
			waitForAnimationDone = true;
			while (waitForAnimationDone)
			{
				yield return null;
			}
		}
		rewardCoinsObject.gameObject.SetActive(false);
		rewardExpObject.gameObject.SetActive(false);
		if (showAward || ratingChange.addRating != 0)
		{
			interfaceAnimator.SetTrigger("Reward");
			if (ExperienceController.sharedController.currentLevel == 31 && _addCoin > 0)
			{
				expRewardValue = 0;
			}
			expRewardValue = _addExpierence;
			coinsRewardValue = _addCoin;
		}
		else
		{
			expRewardValue = 0;
			coinsRewardValue = 0;
		}
		isRewardShow = showAward || ratingChange.addRating != 0;
		if (Defs.isDaterRegim)
		{
			for (int k = 0; k < finishedInterfaceLabels.Length; k++)
			{
				finishedInterfaceLabels[k].text = _winner;
			}
		}
		ExperienceController.sharedController.isShowRanks = true;
		WeaponManager.sharedManager.myNetworkStartTable.DestroyPlayer();
		if (showAward)
		{
			ShowAwardEndInterface(_winner, _addCoin, _addExpierence, _isCustom, firstPlace, _winnerCommand);
		}
		else
		{
			ShowEndInterface(_winner, _winnerCommand, deadInHunger);
		}
		if (Defs.isHunger)
		{
			backButtonInHunger.SetActive(false);
			randomBtn.SetActive(false);
			MapSelectPanel.SetActive(false);
			questsButton.SetActive(false);
			spectatorModeBtnPnl.SetActive(false);
		}
		if (leagueUp)
		{
			rewardButton.SetActive(true);
			string newLeagueEggID = GiveLeagueEggAndReturnId();
			List<Texture> items = GetLeagueItems(newLeagueEggID);
			for (int l = 0; l < trophyItems.Length; l++)
			{
				if (l >= items.Count)
				{
					trophyItems[l].gameObject.SetActive(false);
					continue;
				}
				trophyItems[l].gameObject.SetActive(true);
				trophyItems[l].mainTexture = items[l];
			}
			rewardPanel.SetActive(trophyItems[0].gameObject.activeSelf);
			labelNewItems.SetActive(trophyItems[0].gameObject.activeSelf);
			rewardFrame.ResizeFrame();
			Invoke("OnTrophyOkButtonPress", 60f);
		}
		else
		{
			rewardButton.SetActive(false);
			rewardPanel.SetActive(false);
			labelNewItems.SetActive(false);
		}
		waitForTrophyAnimationDone = ratingChange.addRating != 0;
		while (waitForTrophyAnimationDone)
		{
			yield return null;
		}
		if (Defs.isHunger)
		{
			backButtonInHunger.SetActive(true);
			randomBtn.SetActive(true);
			spectatorModeBtnPnl.SetActive(true);
		}
		for (int m = 0; m < trophyItems.Length; m++)
		{
			trophyItems[m].gameObject.SetActive(false);
		}
		if (showAward)
		{
			rewardExpObject.color = Color.white;
			rewardCoinsObject.color = Color.white;
			rewardCoinsObject.gameObject.SetActive(_addCoin > 0);
			rewardExpObject.gameObject.SetActive(ExperienceController.sharedController.currentLevel < 31);
			rewardPanel.SetActive(rewardCoinsObject.gameObject.activeSelf || rewardExpObject.gameObject.activeSelf);
			rewardFrame.ResizeFrame();
		}
	}

	private string GiveLeagueEggAndReturnId()
	{
		string result = string.Empty;
		try
		{
			RatingSystem.RatingLeague nextLeague = RatingSystem.instance.currentLeague + 1;
			string key = EggGivenForLeagueKey(nextLeague);
			if (Storager.getInt(key, false) == 0)
			{
				EggData eggData2 = Singleton<EggsManager>.Instance.GetAllEggs().FirstOrDefault((EggData eggData) => eggData.League == nextLeague);
				if (eggData2 != null)
				{
					result = eggData2.Id;
					Singleton<EggsManager>.Instance.AddEgg(eggData2);
				}
				else
				{
					Debug.LogErrorFormat("Giving league egg: No egg found for league {0}", nextLeague.ToString());
				}
				Storager.setInt(key, 1, false);
				return result;
			}
			return result;
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in giving league egg: {0}", ex);
			return result;
		}
	}

	private string EggGivenForLeagueKey(RatingSystem.RatingLeague league)
	{
		return string.Format("{0}_egg_given", league.ToString());
	}

	private List<Texture> GetLeagueItems(string newLeagueEggID)
	{
		RatingSystem.RatingLeague league = RatingSystem.instance.currentLeague;
		List<Texture> list = new List<Texture>();
		List<WeaponSkin> list2 = WeaponSkinsManager.SkinsForLeague(league);
		foreach (WeaponSkin item in list2)
		{
			list.Add(ItemDb.GetItemIcon(item.Id, ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory));
		}
		List<string> list3 = Wear.UnboughtLeagueItemsByLeagues()[league];
		for (int i = 0; i < list3.Count; i++)
		{
			list.Add(ItemDb.GetTextureForShopItem(list3[i]));
		}
		List<SkinItem> list4 = (from kvp in SkinsController.sharedController.skinItemsDict
			where kvp.Value.currentLeague == league
			select kvp.Value).ToList();
		foreach (SkinItem item2 in list4)
		{
			list.Add(Resources.Load<Texture>(string.Format("LeagueSkinsProfileImages/league{0}_skin_profile", (int)(league + 1))));
		}
		try
		{
			if (!newLeagueEggID.IsNullOrEmpty())
			{
				list.Add(ItemDb.GetItemIcon(newLeagueEggID, ShopNGUIController.CategoryNames.EggsCategory));
				return list;
			}
			return list;
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in GetLEaguueItems, adding egg texture: {0}", ex);
			return list;
		}
	}

	public void OnTrophyAnimationDone()
	{
		if (!isUsed)
		{
			if (!leagueUp)
			{
				waitForTrophyAnimationDone = false;
				interfaceAnimator.SetBool("isTrophyAdded", true);
			}
			isUsed = true;
		}
	}

	public void OnMatchEndAnimationDone()
	{
		interfaceAnimator.SetBool("interfaceAnimationDone", true);
	}

	public IEnumerator MatchFinishedInDuelInterface(RatingSystem.RatingChange ratingChange, bool showAward, int _addCoin, int _addExpierence, bool _isCustom, bool firstPlace, bool deadheat)
	{
		interfaceAnimator.ResetTrigger("RewardTaken");
		interfaceAnimator.ResetTrigger("GetReward");
		interfaceAnimator.ResetTrigger("AnimationEnds");
		interfaceAnimator.SetBool("isRewarded", showAward || ratingChange.addRating != 0);
		interfaceAnimator.SetBool("isExpOnly", _addCoin == 0 || (ExperienceController.sharedController.currentLevel == 31 && _addCoin > 0));
		interfaceAnimator.SetBool("IsTrophyUp", ratingChange.isUp);
		interfaceAnimator.SetBool("IsTrophyDown", ratingChange.isDown);
		interfaceAnimator.SetBool("IsTrophyAdd", ratingChange.addRating != 0 && !ratingChange.isUp && !ratingChange.isDown);
		interfaceAnimator.SetBool("NoRaiting", ratingChange.addRating == 0);
		interfaceAnimator.SetBool("isTrophyOnly", ratingChange.addRating != 0 && _addCoin == 0 && _addExpierence == 0);
		interfaceAnimator.SetBool("isTrophyAdded", false);
		interfaceAnimator.SetBool("interfaceAnimationDone", false);
		isUsed = false;
		trophiAddIcon.SetActive(ratingChange.addRating > 0);
		trophiMinusIcon.SetActive(ratingChange.addRating < 0);
		trophyPanel.SetActive(ratingChange.addRating != 0);
		currentCup.spriteName = ratingChange.oldLeague.ToString() + " " + (3 - ratingChange.oldDivision);
		NewCup.spriteName = ratingChange.newLeague.ToString() + " " + (3 - ratingChange.newDivision);
		if (ratingChange.addRating > 0)
		{
			currentBarFillAmount = ratingChange.oldRatingAmount;
			nextBarFillAmount = ratingChange.newRatingAmount;
			currentBar.fillAmount = currentBarFillAmount;
			nextBar.fillAmount = currentBarFillAmount;
			nextBar.color = Color.yellow;
		}
		else
		{
			currentBarFillAmount = ratingChange.oldRatingAmount;
			nextBarFillAmount = ratingChange.newRatingAmount;
			currentBar.fillAmount = nextBarFillAmount;
			nextBar.fillAmount = nextBarFillAmount;
			nextBar.color = Color.red;
		}
		leagueUp = ratingChange.newLeague > ratingChange.oldLeague;
		if (ratingChange.maxRating == int.MaxValue)
		{
			trophyPoints.text = ratingChange.newRating.ToString();
		}
		else
		{
			trophyPoints.text = ratingChange.newRating + "/" + ratingChange.maxRating;
		}
		trophyShine.SetActive(ratingChange.isUp);
		trophyRewardValue = ratingChange.addRating;
		string trophyAwardText = string.Format((ratingChange.addRating <= 0) ? "{0}" : "+{0}", trophyRewardValue);
		UILabel[] array = rewardTrophy;
		foreach (UILabel label in array)
		{
			label.gameObject.SetActive(ratingChange.addRating != 0);
			label.text = trophyAwardText;
		}
		string leagueChangeText = string.Format(LocalizationStore.Get(RatingSystem.leagueChangeLocalizations[(int)ratingChange.newLeague]), RatingSystem.divisionByIndex[ratingChange.newDivision]);
		UILabel[] array2 = textLeagueUp;
		foreach (UILabel label2 in array2)
		{
			label2.text = leagueChangeText;
		}
		UILabel[] array3 = textLeagueDown;
		foreach (UILabel label3 in array3)
		{
			label3.text = leagueChangeText;
		}
		shareToggle.value = false;
		if (WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			FreezePlayer();
			interfaceAnimator.SetTrigger("MatchEnd");
			shopButton.GetComponent<Collider>().enabled = false;
			ShowFinishedInterface(firstPlace, deadheat);
			waitForAnimationDone = true;
			while (waitForAnimationDone)
			{
				yield return null;
			}
		}
		rewardCoinsObject.gameObject.SetActive(false);
		rewardExpObject.gameObject.SetActive(false);
		if (showAward || ratingChange.addRating != 0)
		{
			interfaceAnimator.SetTrigger("Reward");
			if (ExperienceController.sharedController.currentLevel == 31 && _addCoin > 0)
			{
				expRewardValue = 0;
			}
			expRewardValue = _addExpierence;
			coinsRewardValue = _addCoin;
		}
		else
		{
			expRewardValue = 0;
			coinsRewardValue = 0;
		}
		isRewardShow = showAward || ratingChange.addRating != 0;
		ExperienceController.sharedController.isShowRanks = true;
		WeaponManager.sharedManager.myNetworkStartTable.DestroyPlayer();
		if (showAward)
		{
			ShowAwardEndInterface(string.Empty, _addCoin, _addExpierence, _isCustom, firstPlace, 0);
		}
		else
		{
			ShowEndInterface(string.Empty, 0);
		}
		duelUI.ShowFinishedInterface(ratingChange, showAward, _addCoin, _addExpierence, firstPlace, deadheat);
		if (leagueUp)
		{
			rewardButton.SetActive(true);
			string newLeagueEggID = GiveLeagueEggAndReturnId();
			List<Texture> items = GetLeagueItems(newLeagueEggID);
			for (int i = 0; i < trophyItems.Length; i++)
			{
				if (i >= items.Count)
				{
					trophyItems[i].gameObject.SetActive(false);
					continue;
				}
				trophyItems[i].gameObject.SetActive(true);
				trophyItems[i].mainTexture = items[i];
			}
			rewardPanel.SetActive(trophyItems[0].gameObject.activeSelf);
			labelNewItems.SetActive(trophyItems[0].gameObject.activeSelf);
			rewardFrame.ResizeFrame();
			Invoke("OnTrophyOkButtonPress", 60f);
		}
		else
		{
			rewardButton.SetActive(false);
			rewardPanel.SetActive(false);
			labelNewItems.SetActive(false);
		}
		waitForTrophyAnimationDone = ratingChange.addRating != 0;
		while (waitForTrophyAnimationDone)
		{
			yield return null;
		}
		for (int j = 0; j < trophyItems.Length; j++)
		{
			trophyItems[j].gameObject.SetActive(false);
		}
		if (showAward)
		{
			rewardExpObject.color = Color.white;
			rewardCoinsObject.color = Color.white;
			rewardCoinsObject.gameObject.SetActive(_addCoin > 0);
			rewardExpObject.gameObject.SetActive(ExperienceController.sharedController.currentLevel < 31);
			rewardPanel.SetActive(rewardCoinsObject.gameObject.activeSelf || rewardExpObject.gameObject.activeSelf);
			rewardFrame.ResizeFrame();
		}
	}

	public void OnTablesShow()
	{
		waitForAnimationDone = false;
	}

	public void OnRewardShow()
	{
		StartCoroutine(StartRewardAnimation());
	}

	public IEnumerator StartRewardAnimation()
	{
		rewardFrame.ResizeFrame();
		float animTime3 = 0f;
		while (ShopNGUIController.GuiActive || (BankController.Instance != null && BankController.Instance.InterfaceEnabled))
		{
			yield return null;
		}
		if (expRewardValue > 0)
		{
			Vector3 expStart = rewardExpObject.transform.localPosition;
			while (animTime3 < 1f)
			{
				rewardExpObject.transform.localPosition = Vector3.Lerp(expStart, rewardExpAnimPoint.localPosition, Mathf.Min(animTime3, 1f));
				rewardExpObject.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), Mathf.Min(animTime3, 1f));
				animTime3 += Time.deltaTime / 0.4f;
				yield return null;
			}
			ExperienceController.sharedController.addExperience(expRewardValue);
			if (WeaponManager.sharedManager.myNetworkStartTable != null)
			{
				WeaponManager.sharedManager.myNetworkStartTable.UpdateRanks();
			}
			expRewardValue = 0;
		}
		rewardExpObject.gameObject.SetActive(false);
		animTime3 = 0f;
		if (coinsRewardValue > 0)
		{
			Vector3 coinsStart = rewardCoinsObject.transform.localPosition;
			while (animTime3 < 1f)
			{
				if (!Defs.isHunger)
				{
					rewardCoinsObject.transform.localPosition = Vector3.Lerp(coinsStart, rewardCoinsAnimPoint.localPosition, Mathf.Min(animTime3, 1f));
				}
				rewardCoinsObject.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), Mathf.Min(animTime3, 1f));
				animTime3 += Time.deltaTime / 0.4f;
				yield return null;
			}
			BankController.AddCoins(coinsRewardValue);
			coinsRewardValue = 0;
		}
		rewardCoinsObject.gameObject.SetActive(false);
		animTime3 = 0f;
	}

	public void StartTrophyAnim()
	{
		StartCoroutine(TrophyFillAnimation());
	}

	private IEnumerator TrophyFillAnimation()
	{
		float animTime = 0f;
		if (trophyRewardValue != 0)
		{
			while (animTime < 1f)
			{
				nextBar.fillAmount = Mathf.Lerp(currentBarFillAmount, nextBarFillAmount, Mathf.Min(animTime, 1f));
				animTime += Time.deltaTime;
				yield return null;
			}
		}
	}

	public void ShowAwardEndInterface(string _winner, int _addCoin, int _addExpierence, bool _isCustom, bool firstPlace, int _winnerCommand)
	{
		if (_isCustom)
		{
			addCoins = 0;
			addExperience = 0;
		}
		else
		{
			addCoins = _addCoin;
			addExperience = _addExpierence;
		}
		string text = string.Format("+{0} {1}", _addCoin, LocalizationStore.Key_0275);
		string text2 = string.Format("+{0} {1}", _addExpierence, LocalizationStore.Key_0204);
		ConnectSceneNGUIController.RegimGame regim = ConnectSceneNGUIController.regim;
		PremiumAccountController instance = PremiumAccountController.Instance;
		bool flag = regim == ConnectSceneNGUIController.RegimGame.Deathmatch || regim == ConnectSceneNGUIController.RegimGame.FlagCapture || regim == ConnectSceneNGUIController.RegimGame.TeamFight || regim == ConnectSceneNGUIController.RegimGame.CapturePoints;
		bool flag2 = PromoActionsManager.sharedManager.IsDayOfValorEventActive && flag;
		bool flag3 = instance.IsActiveOrWasActiveBeforeStartMatch();
		int num = 1;
		int num2 = 1;
		if (flag3 || flag2)
		{
			num = ((!Defs.isCOOP && !Defs.isHunger) ? AdminSettingsController.GetMultiplyerRewardWithBoostEvent(false) : PremiumAccountController.Instance.RewardCoeff);
			num2 = ((!Defs.isCOOP && !Defs.isHunger) ? AdminSettingsController.GetMultiplyerRewardWithBoostEvent(true) : PremiumAccountController.Instance.RewardCoeff);
		}
		if ((flag3 && flag2) || flag3 || flag2)
		{
		}
		UILabel[] array = rewardCoins;
		foreach (UILabel uILabel in array)
		{
			uILabel.text = text;
		}
		UILabel[] array2 = rewardExperience;
		foreach (UILabel uILabel2 in array2)
		{
			uILabel2.text = text2;
		}
		ShowEndInterface(_winner, _winnerCommand);
	}

	public void ShowEndInterface(string _winner, int _winnerCommand, bool deadInHunger = false)
	{
		if (!ShopNGUIController.NoviceArmorAvailable)
		{
			GetComponent<HintController>().HideHintByName("shop_remove_novice_armor");
		}
		NotificationController.instance.SaveTimeValues();
		if (ABTestController.useBuffSystem)
		{
			BuffSystem.instance.EndRound();
		}
		else
		{
			KillRateCheck.instance.CheckKillRate();
		}
		WeaponManager.sharedManager.myNetworkStartTable.ClearKillrate();
		endInterfacePanel.SetActive(!Defs.isDaterRegim);
		backButtonInHunger.SetActive(Defs.isHunger);
		if (Defs.isDuel)
		{
			return;
		}
		if (Defs.isCompany || Defs.isFlag || Defs.isCapturePoints)
		{
			winnerPanelCom1.SetActive(_winnerCommand == 1);
			winnerPanelCom2.SetActive(_winnerCommand == 2);
		}
		startInterfacePanel.SetActive(Defs.isDaterRegim);
		goBattleLabel.SetActive(!Defs.isDaterRegim);
		daterButtonLabel.SetActive(Defs.isDaterRegim);
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.aimPanel.SetActive(false);
		}
		socialPnl.SetActive(BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64);
		winner = _winner;
		allInterfacePanel.SetActive(true);
		ranksTable.UpdateRanksFromOldSpisok();
		if (Defs.isHunger || Defs.isRegimVidosDebug)
		{
			if (Defs.isHunger)
			{
				randomBtn.SetActive(true);
				questsButton.SetActive(true);
			}
			spectatorModeBtnPnl.SetActive(true);
			updateRealTableAfterActionPanel = deadInHunger;
			if (!ActionPanel.activeSelf && !ReasonsPanel.activeSelf)
			{
				ShowTable(deadInHunger);
			}
		}
		else
		{
			updateRealTableAfterActionPanel = false;
			ShowTable(false);
			MapSelectPanel.SetActive(false);
			questsButton.SetActive(false);
		}
		StartCoroutine("TryToShowExpiredBanner");
	}

	private void ShareResults()
	{
		faceBookPriority = FacebookController.StoryPriority.Red;
		twitterPriority = FacebookController.StoryPriority.MultyWinLimit;
		twitterStatus = () => "I've won a battle in @PixelGun3D! Join the fight now! #pixelgun3d #pixelgun #3d #pg3d #mobile #fps #shooter http://goo.gl/8fzL9u";
		EventTitle = "Won Batlle";
		if (TwitterController.TwitterSupported && TwitterController.IsLoggedIn && TwitterController.Instance.CanPostStatusUpdateWithPriority(twitterPriority))
		{
			TwitterController.Instance.PostStatusUpdate(twitterStatus(), twitterPriority);
			AnalyticsFacade.SendCustomEvent("Virality", new Dictionary<string, object>
			{
				{ "Post Twitter", EventTitle },
				{ "Total Twitter", "Posts" }
			});
		}
		if (FacebookController.FacebookSupported && FB.IsLoggedIn && FacebookController.sharedController.CanPostStoryWithPriority(facebookPriority))
		{
			FacebookController.PostOpenGraphStory("win", "battle", facebookPriority, new Dictionary<string, string> { 
			{
				"battle",
				ConnectSceneNGUIController.regim.ToString().ToLower()
			} });
			AnalyticsFacade.SendCustomEvent("Virality", new Dictionary<string, object>
			{
				{ "Post Facebook", EventTitle },
				{ "Total Facebook", "Posts" }
			});
		}
	}

	public void OnTablesShown()
	{
		if (Defs.isDaterRegim)
		{
			return;
		}
		if (!Defs.isHunger || expRewardValue > 0 || coinsRewardValue > 0)
		{
			if (Defs.isDuel)
			{
				oldRanksIsActive = true;
				HideOldRanks();
			}
			else
			{
				Invoke("HideOldRanks", 60f);
				oldRanksIsActive = true;
			}
			hideOldRanksButton.SetActive(!Defs.isDuel);
			if (Defs.isHunger)
			{
				backButtonInHunger.SetActive(false);
				randomBtn.SetActive(false);
				MapSelectPanel.SetActive(false);
				questsButton.SetActive(false);
				spectatorModeBtnPnl.SetActive(false);
			}
		}
		else
		{
			hideOldRanksButton.SetActive(false);
			MapSelectPanel.SetActive(true);
			questsButton.SetActive(true);
		}
	}

	public void HideOldRanks()
	{
		if (oldRanksIsActive && (hideOldRanksButton.activeSelf || Defs.isDuel))
		{
			CancelInvoke("HideOldRanks");
			interfaceAnimator.SetTrigger("OkPressed");
			if (expRewardValue > 0 || coinsRewardValue > 0 || trophyRewardValue != 0)
			{
				interfaceAnimator.SetTrigger("GetReward");
			}
			hideOldRanksButton.SetActive(false);
		}
	}

	public void HandleHideOldRanksClick()
	{
		if (oldRanksIsActive)
		{
			if (shareToggle.value && (expRewardValue > 0 || coinsRewardValue > 0))
			{
				ShareResults();
			}
			HideOldRanks();
		}
	}

	public void FinishHideOldRanks()
	{
		shopButton.GetComponent<UIButton>().isEnabled = true;
		shopButton.GetComponent<Collider>().enabled = true;
		interfaceAnimator.SetTrigger("RewardTaken");
		if (oldRanksIsActive || Defs.isHunger)
		{
			trophyRewardValue = 0;
			oldRanksIsActive = false;
			questsButton.SetActive(TrainingController.TrainingCompleted);
			isRewardShow = false;
			if (Defs.isMulti)
			{
				MapSelectPanel.SetActive(true);
			}
			if (Defs.isDuel)
			{
				duelUI.ShowEndInterface();
			}
			else if (!Defs.isHunger)
			{
				finishedInterface.SetActive(false);
				HideEndInterface();
				ShowNewMatchInterface();
				WeaponManager.sharedManager.myNetworkStartTable.ResetOldScore();
			}
			else
			{
				backButtonInHunger.SetActive(true);
				randomBtn.SetActive(true);
				spectatorModeBtnPnl.SetActive(true);
			}
			if (WeaponManager.sharedManager.myTable != null)
			{
				WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().isShowAvard = false;
			}
			else
			{
				StartCoroutine(WaitAndRemoveInterfaceOnReconnect());
			}
		}
	}

	private IEnumerator WaitAndRemoveInterfaceOnReconnect()
	{
		yield return null;
		UnityEngine.Object.Destroy(sharedController.gameObject);
	}

	private IEnumerator TryToShowExpiredBanner()
	{
		while (FriendsController.sharedController == null || TempItemsController.sharedController == null)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(FriendsController.sharedController.MyWaitForSeconds(1f));
			try
			{
				if (ShopNGUIController.GuiActive || (BankController.Instance != null && BankController.Instance.InterfaceEnabled) || (ExpController.Instance != null && ExpController.Instance.WaitingForLevelUpView) || (ExpController.Instance != null && ExpController.Instance.IsLevelUpShown) || waitForAnimationDone || waitForTrophyAnimationDone || rentScreenPoint.childCount != 0)
				{
					continue;
				}
				if (BuffSystem.instance != null && BuffSystem.instance.haveAllInteractons && Storager.getInt("Training.ShouldRemoveNoviceArmorInShopKey", false) == 1)
				{
					GameObject window = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("NguiWindows/WeRemoveNoviceArmorBanner"));
					window.transform.parent = rentScreenPoint;
					Player_move_c.SetLayerRecursively(window, LayerMask.NameToLayer("NGUITable"));
					window.transform.localPosition = new Vector3(0f, 0f, -130f);
					window.transform.localRotation = Quaternion.identity;
					window.transform.localScale = new Vector3(1f, 1f, 1f);
					GetComponent<HintController>().HideHintByName("shop_remove_novice_armor");
					try
					{
						ShopNGUIController.EquipWearInCategoryIfNotEquiped("Armor_Army_1", ShopNGUIController.CategoryNames.ArmorCategory, false);
					}
					catch (Exception e2)
					{
						Debug.LogError("Exception in NetworkStartTableNguiController: ShopNGUIController.EquipWearInCategoryIfNotEquiped: " + e2);
					}
				}
				else
				{
					ShopNGUIController.ShowTryGunIfPossible(startInterfacePanel.activeSelf, rentScreenPoint, "NGUITable");
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning("exception in NetworkTableNGUI  TryToShowExpiredBanner: " + e);
			}
		}
	}

	public static bool IsStartInterfaceShown()
	{
		return sharedController != null && sharedController.startInterfacePanel != null && sharedController.startInterfacePanel.activeSelf;
	}

	public static bool IsEndInterfaceShown()
	{
		return sharedController != null && sharedController.endInterfacePanel != null && sharedController.endInterfacePanel.activeSelf;
	}

	public void HideEndInterface()
	{
		Debug.Log("HideEndInterface");
		socialPnl.SetActive(false);
		allInterfacePanel.SetActive(false);
		endInterfacePanel.SetActive(false);
		winnerPanelCom1.SetActive(false);
		winnerPanelCom2.SetActive(false);
		if (defaultTeamOneState == Vector3.zero)
		{
			defaultTeamOneState = teamOneLabel.transform.localPosition;
		}
		if (defaultTeamTwoState == Vector3.zero)
		{
			defaultTeamTwoState = teamTwoLabel.transform.localPosition;
		}
		teamOneLabel.transform.localPosition = defaultTeamOneState;
		teamTwoLabel.transform.localPosition = defaultTeamTwoState;
		HideTable();
		ReasonsPanel.SetActive(false);
		ActionPanel.SetActive(false);
		updateRealTableAfterActionPanel = true;
		StopCoroutine("TryToShowExpiredBanner");
	}

	private void ShowTable(bool _isRealUpdate = true)
	{
		ranksTable.isShowRanks = _isRealUpdate;
		ranksTable.tekPanel.SetActive(true);
	}

	public void HideTable()
	{
		ranksTable.isShowRanks = false;
		ranksTable.tekPanel.SetActive(false);
	}

	public void ShowRanksTable()
	{
		ShowTable(true);
		ranksInterface.SetActive(true);
	}

	public void HideRanksTable(bool isHideTable = true)
	{
		if (isHideTable)
		{
			HideTable();
		}
		ranksInterface.SetActive(false);
	}

	public void BackPressFromRanksTable(bool isHideTable = true)
	{
		if (!CheckHideInternalPanel())
		{
			HideRanksTable(isHideTable);
			ReasonsPanel.SetActive(false);
			ActionPanel.SetActive(false);
			if (WeaponManager.sharedManager.myPlayerMoveC != null)
			{
				WeaponManager.sharedManager.myPlayerMoveC.BackRanksPressed();
			}
		}
	}

	public void UpdateGoMapButtonsInDuel(bool show = true)
	{
		bool flag = !show || ConnectSceneNGUIController.gameTier != ExpController.Instance.OurTier;
		for (int i = 0; i < goMapInEndGameButtonsDuel.Length; i++)
		{
			goMapInEndGameButtonsDuel[i].gameObject.SetActive(!flag);
		}
		changeMapLabel.SetActive(!flag);
		if (flag)
		{
			return;
		}
		AllScenesForMode listScenesForMode = SceneInfoController.instance.GetListScenesForMode(ConnectSceneNGUIController.curSelectMode);
		SceneInfo[] array = new SceneInfo[goMapInEndGameButtonsDuel.Length];
		goMapInEndGameButtonsDuel[0].SetMap(null);
		for (int j = 1; j < array.Length; j++)
		{
			int num = 0;
			bool flag2 = true;
			int num2 = UnityEngine.Random.Range(0, listScenesForMode.avaliableScenes.Count);
			while (flag2)
			{
				flag2 = false;
				SceneInfo sceneInfo = listScenesForMode.avaliableScenes[num2];
				for (int k = 0; k < j; k++)
				{
					if (array[k] == sceneInfo)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2 && (sceneInfo.NameScene.Equals(Application.loadedLevelName) || sceneInfo.AvaliableWeapon == ModeWeapon.dater || (sceneInfo.isPremium && Storager.getInt(sceneInfo.NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(sceneInfo.NameScene))))
				{
					flag2 = true;
				}
				if (!flag2)
				{
					array[j] = sceneInfo;
				}
				else
				{
					num2++;
					num++;
					if (num2 > listScenesForMode.avaliableScenes.Count - 1)
					{
						num2 = 0;
					}
				}
				if (num > listScenesForMode.avaliableScenes.Count)
				{
					Debug.LogWarning("no map");
					break;
				}
			}
			if (array[j] != null)
			{
				goMapInEndGameButtonsDuel[j].SetMap(array[j]);
			}
			else
			{
				goMapInEndGameButtonsDuel[j].gameObject.SetActive(false);
			}
		}
	}

	public void UpdateGoMapButtons(bool show = true)
	{
		if (Defs.isDuel)
		{
			UpdateGoMapButtonsInDuel(show);
			return;
		}
		bool flag = !show || ConnectSceneNGUIController.gameTier != ExpController.Instance.OurTier;
		for (int i = 0; i < goMapInEndGameButtons.Length; i++)
		{
			goMapInEndGameButtons[i].gameObject.SetActive(!flag);
		}
		changeMapLabel.SetActive(!flag);
		if (flag)
		{
			return;
		}
		AllScenesForMode listScenesForMode = SceneInfoController.instance.GetListScenesForMode(ConnectSceneNGUIController.curSelectMode);
		SceneInfo[] array = new SceneInfo[goMapInEndGameButtons.Length];
		goMapInEndGameButtons[0].SetMap(null);
		for (int j = 1; j < array.Length; j++)
		{
			int num = 0;
			bool flag2 = true;
			int num2 = UnityEngine.Random.Range(0, listScenesForMode.avaliableScenes.Count);
			while (flag2)
			{
				flag2 = false;
				SceneInfo sceneInfo = listScenesForMode.avaliableScenes[num2];
				for (int k = 0; k < j; k++)
				{
					if (array[k] == sceneInfo)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2 && (sceneInfo.NameScene.Equals(Application.loadedLevelName) || sceneInfo.AvaliableWeapon == ModeWeapon.dater || (sceneInfo.isPremium && Storager.getInt(sceneInfo.NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(sceneInfo.NameScene))))
				{
					flag2 = true;
				}
				if (!flag2)
				{
					array[j] = sceneInfo;
				}
				else
				{
					num2++;
					num++;
					if (num2 > listScenesForMode.avaliableScenes.Count - 1)
					{
						num2 = 0;
					}
				}
				if (num > listScenesForMode.avaliableScenes.Count)
				{
					Debug.LogWarning("no map");
					break;
				}
			}
			goMapInEndGameButtons[j].SetMap(array[j]);
		}
	}

	public void OnRewardAnimationEnds()
	{
		interfaceAnimator.SetTrigger("AnimationEnds");
	}

	public void OnTrophyOkButtonPress()
	{
		CancelInvoke("OnTrophyOkButtonPress");
		waitForTrophyAnimationDone = false;
		interfaceAnimator.SetBool("isTrophyAdded", true);
	}
}

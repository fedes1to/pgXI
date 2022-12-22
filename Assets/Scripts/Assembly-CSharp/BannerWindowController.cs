using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using Unity.Linq;
using UnityEngine;

internal sealed class BannerWindowController : MonoBehaviour
{
	private const float StartBannerShowDelay = 3f;

	public BannerWindow[] bannerWindows;

	public static bool firstScreen = true;

	[NonSerialized]
	public AdvertisementController advertiseController;

	private readonly int BannerWindowCount = Enum.GetNames(typeof(BannerWindowType)).Length;

	private Queue<BannerWindow> _bannerQueue;

	private BannerWindow _currentBanner;

	private bool[] _bannerShowed;

	private bool[] _needShowBanner;

	private bool _someBannerShown;

	private float _lastCheckTime;

	private float _whenStart;

	private bool _isBlockShowForNewPlayer;

	private bool _isConnectScene;

	private int _viewedBannersCount;

	public static BannerWindowController SharedController { get; private set; }

	internal bool IsAnyBannerShown
	{
		get
		{
			return _currentBanner != null;
		}
	}

	private BannerWindowController()
	{
		_bannerShowed = new bool[BannerWindowCount];
		_needShowBanner = new bool[BannerWindowCount];
	}

	private void Awake()
	{
		SharedController = this;
	}

	private void Start()
	{
		_currentBanner = null;
		_bannerQueue = new Queue<BannerWindow>();
		_someBannerShown = false;
		_whenStart = Time.realtimeSinceStartup + 3f;
		if (StarterPackController.Get != null)
		{
			StarterPackController.Get.CheckShowStarterPack();
			StarterPackController.Get.UpdateCountShownWindowByTimeCondition();
		}
		PromoActionsManager.UpdateDaysOfValorShownCondition();
		_isBlockShowForNewPlayer = !IsBannersCanShowAfterNewInstall();
		_isConnectScene = SceneLoader.ActiveSceneName.Equals("ConnectScene");
	}

	public void AddBannersTimeout(float seconds)
	{
		_lastCheckTime = Time.realtimeSinceStartup + seconds;
	}

	private void OnDestroy()
	{
		SharedController = null;
		_bannerQueue = null;
		advertiseController = null;
		firstScreen = false;
	}

	private IEnumerator OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			yield return null;
			yield return null;
			yield return null;
			if (StarterPackController.Get != null)
			{
				StarterPackController.Get.UpdateCountShownWindowByTimeCondition();
			}
			PromoActionsManager.UpdateDaysOfValorShownCondition();
			_isBlockShowForNewPlayer = !IsBannersCanShowAfterNewInstall();
		}
	}

	public void RegisterWindow(BannerWindow window, BannerWindowType windowType)
	{
		if (bannerWindows.Length < (int)(windowType + 1))
		{
			List<BannerWindow> list = bannerWindows.ToList();
			while (list.Count() < (int)(windowType + 1))
			{
				list.Add(null);
			}
			bannerWindows = list.ToArray();
		}
		bannerWindows[(int)windowType] = window;
		int layer = LayerMask.NameToLayer("Banners");
		window.gameObject.Descendants().ForEach(delegate(GameObject go)
		{
			go.layer = layer;
		});
	}

	private BannerWindow ShowBannerWindow(BannerWindowType windowType)
	{
		if (_isConnectScene && _viewedBannersCount > 0)
		{
			return null;
		}
		if (bannerWindows.Length < 0 || (int)windowType > bannerWindows.Length - 1)
		{
			return null;
		}
		if (bannerWindows[(int)windowType] == null)
		{
			return null;
		}
		if (bannerWindows[(int)windowType].gameObject.activeSelf)
		{
			return null;
		}
		BannerWindow bannerWindow = bannerWindows[(int)windowType];
		if (_currentBanner == null)
		{
			_currentBanner = bannerWindow;
			_currentBanner.type = windowType;
			if (_isConnectScene)
			{
				_viewedBannersCount++;
			}
			bannerWindow.Show();
		}
		else
		{
			_bannerQueue.Enqueue(bannerWindow);
		}
		return bannerWindow;
	}

	public void HideBannerWindowNoShowNext()
	{
		if (_currentBanner != null)
		{
			_currentBanner.Hide();
			_currentBanner = null;
		}
	}

	public void ClearBannerStates()
	{
		_bannerShowed = new bool[BannerWindowCount];
		_needShowBanner = new bool[BannerWindowCount];
	}

	public void HideBannerWindow()
	{
		BuySmileBannerController.openedFromPromoActions = false;
		HideBannerWindowNoShowNext();
		if ((!_isConnectScene || _viewedBannersCount <= 0) && _bannerQueue.Count > 0)
		{
			(_currentBanner = _bannerQueue.Dequeue()).Show();
		}
	}

	private void ShowAdmobBanner()
	{
		if (!(AdmobPerelivWindow.admobTexture == null) && !string.IsNullOrEmpty(AdmobPerelivWindow.admobUrl))
		{
			ShowBannerWindow(BannerWindowType.Admob);
		}
	}

	public void AdmobBannerExitClick()
	{
		ButtonClickSound.Instance.PlayClick();
		HideBannerWindow();
		_bannerShowed[2] = false;
		_needShowBanner[2] = false;
		ResetStateBannerShowed(BannerWindowType.Admob);
	}

	public void AdmobBannerApplyClick()
	{
		Application.OpenURL(AdmobPerelivWindow.admobUrl);
	}

	private void ShowAdvertisementBanner(AdvertisementController advertisementController)
	{
		if (!(advertisementController.AdvertisementTexture == null))
		{
			advertiseController = advertisementController;
			BannerWindow bannerWindow = ShowBannerWindow(BannerWindowType.Advertisement);
			if (!(bannerWindow == null) && AdsConfigManager.Instance.LastLoadedConfig != null)
			{
				string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
				PerelivSettings perelivSettings = PerelivConfigManager.Instance.GetPerelivSettings(playerCategory);
				bannerWindow.SetEnableExitButton(perelivSettings.ButtonClose);
				bannerWindow.SetBackgroundImage(advertisementController.AdvertisementTexture);
			}
		}
	}

	public void AdvertBannerExitClick()
	{
		ButtonClickSound.Instance.PlayClick();
		advertiseController.Close();
		UpdateAdvertShownCount();
		HideBannerWindow();
	}

	public void NewVersionBannerExitClick()
	{
		ButtonClickSound.Instance.PlayClick();
		UpdateNewVersionShownCount();
		HideBannerWindow();
	}

	private static void UpdateNewVersionShownCount()
	{
		PlayerPrefs.SetInt(Defs.UpdateAvailableShownTimesSN, PlayerPrefs.GetInt(Defs.UpdateAvailableShownTimesSN, 3) - 1);
		PlayerPrefs.SetString(Defs.LastTimeUpdateAvailableShownSN, DateTimeOffset.Now.ToString("s"));
		PlayerPrefs.Save();
	}

	private static void ClearNewVersionShownCount()
	{
		PlayerPrefs.SetInt(Defs.UpdateAvailableShownTimesSN, 0);
		PlayerPrefs.SetString(Defs.LastTimeUpdateAvailableShownSN, DateTimeOffset.Now.ToString("s"));
		PlayerPrefs.Save();
	}

	public void AdvertBannerApplyClick()
	{
		ButtonClickSound.Instance.PlayClick();
		advertiseController.Close();
		UpdateAdvertShownCount();
		if (AdsConfigManager.Instance.LastLoadedConfig != null)
		{
			string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
			PerelivSettings perelivSettings = PerelivConfigManager.Instance.GetPerelivSettings(playerCategory);
			Application.OpenURL(perelivSettings.RedirectUrl);
		}
		HideBannerWindow();
	}

	public void NewVersionBannerApplyClick()
	{
		ButtonClickSound.Instance.PlayClick();
		ClearNewVersionShownCount();
		Application.OpenURL(MainMenu.RateUsURL);
		HideBannerWindow();
	}

	public void EverydayRewardApplyClick()
	{
		ButtonClickSound.TryPlayClick();
		TakeEverydayRewardForPlayer();
		HideBannerWindow();
	}

	private void TakeEverydayRewardForPlayer()
	{
		NotificationController.isGetEveryDayMoney = false;
		if (MainMenu.sharedMenu != null)
		{
			MainMenu.sharedMenu.isShowAvard = false;
		}
		int @int = Storager.getInt("Coins", false);
		Storager.setInt("Coins", @int + 3, false);
		AnalyticsFacade.CurrencyAccrual(3, "Coins");
		CoinsMessage.FireCoinsAddedEvent(false, 2);
		AudioClip audioClip = Resources.Load<AudioClip>("coin_get");
		if (audioClip != null && Defs.isSoundFX)
		{
			NGUITools.PlaySound(audioClip);
		}
	}

	public void SorryBannerExitButtonClick()
	{
		MainMenuController.sharedController.stubLoading.SetActive(false);
		HideBannerWindow();
	}

	public void EventX3ExitClick()
	{
		ButtonClickSound.TryPlayClick();
		UpdateEventX3ShownCount();
		HideBannerWindow();
	}

	public void EventX3ApplyClick()
	{
		EventX3ExitClick();
		if (MainMenuController.sharedController != null)
		{
			MainMenuController.sharedController.ShowBankWindow();
		}
		else if (ConnectSceneNGUIController.sharedController != null)
		{
			ConnectSceneNGUIController.sharedController.ShowBankWindow();
		}
	}

	private void UpdateEventX3ShownCount()
	{
		PlayerPrefs.SetInt(Defs.EventX3WindowShownCount, PlayerPrefs.GetInt(Defs.EventX3WindowShownCount, 1) - 1);
		PlayerPrefs.Save();
	}

	private void UpdateAdvertShownCount()
	{
		if (AdsConfigManager.Instance.LastLoadedConfig != null)
		{
			string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
			PerelivSettings perelivSettings = PerelivConfigManager.Instance.GetPerelivSettings(playerCategory);
			if (!perelivSettings.ShowAlways)
			{
				PlayerPrefs.SetInt(Defs.AdvertWindowShownCount, PlayerPrefs.GetInt(Defs.AdvertWindowShownCount, 3) - 1);
				PlayerPrefs.SetString(Defs.AdvertWindowShownLastTime, PromoActionsManager.CurrentUnixTime.ToString());
				PlayerPrefs.Save();
			}
		}
	}

	private bool IsBannersCanShowAfterNewInstall()
	{
		if (string.IsNullOrEmpty(Defs.StartTimeShowBannersString))
		{
			return true;
		}
		DateTime result;
		if (!DateTime.TryParse(Defs.StartTimeShowBannersString, out result))
		{
			return true;
		}
		return (DateTime.UtcNow - result).TotalMinutes >= 1440.0 || Defs.countReturnInConnectScene >= 4;
	}

	private void Update()
	{
		if ((_isConnectScene && _viewedBannersCount > 0) || Time.realtimeSinceStartup < _whenStart || !(Time.realtimeSinceStartup - _lastCheckTime >= 1f))
		{
			return;
		}
		CheckBannersShowConditions();
		for (int i = 0; i < _needShowBanner.Length; i++)
		{
			if ((_someBannerShown && i != 2) || !_needShowBanner[i] || ActivityIndicator.IsActiveIndicator)
			{
				continue;
			}
			if (MainMenuController.IsShowRentExpiredPoint() || (MainMenuController.sharedController != null && (MainMenuController.sharedController.FreePanelIsActive || MainMenuController.sharedController.singleModePanel.activeSelf)) || (BankController.Instance != null && BankController.Instance.InterfaceEnabled) || !FreeAwardController.FreeAwardChestIsInIdleState || MainMenuController.SavedShwonLobbyLevelIsLessThanActual() || (ExpController.Instance != null && ExpController.Instance.IsLevelUpShown))
			{
				break;
			}
			if ((i != 6 || !SceneLoader.ActiveSceneName.Equals(Defs.MainMenuScene) || Storager.getInt(Defs.ShownLobbyLevelSN, false) >= 3) && (i != 1 || SceneLoader.ActiveSceneName.Equals(Defs.MainMenuScene)) && (i != 0 || SceneLoader.ActiveSceneName.Equals(Defs.MainMenuScene)) && (!_isConnectScene || !_needShowBanner[10] || i == 10))
			{
				_needShowBanner[i] = false;
				switch (i)
				{
				case 2:
					ShowAdmobBanner();
					break;
				case 5:
					ShowAdvertisementBanner(advertiseController);
					break;
				default:
					ShowBannerWindow((BannerWindowType)i);
					break;
				}
				_someBannerShown = true;
				break;
			}
		}
		_lastCheckTime = Time.realtimeSinceStartup;
	}

	private void CheckDownloadAdvertisement()
	{
		if (!(ExperienceController.sharedController == null) && AdsConfigManager.Instance.LastLoadedConfig != null)
		{
			string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
			PerelivSettings perelivSettings = PerelivConfigManager.Instance.GetPerelivSettings(playerCategory);
			int currentLevel = ExperienceController.sharedController.currentLevel;
			bool flag = perelivSettings.MinLevel == -1 || currentLevel >= perelivSettings.MinLevel;
			bool flag2 = perelivSettings.MaxLevel == -1 || currentLevel <= perelivSettings.MaxLevel;
			bool flag3 = perelivSettings.ShowAlways || PlayerPrefs.GetInt(Defs.AdvertWindowShownCount, 3) > 0;
			if (perelivSettings.Enabled && advertiseController.CurrentState == AdvertisementController.State.Idle && flag && flag2 && flag3)
			{
				advertiseController.Run();
			}
		}
	}

	private bool IsAdvertisementDownloading()
	{
		if (advertiseController == null)
		{
			return false;
		}
		AdvertisementController.State currentState = advertiseController.CurrentState;
		return currentState != 0 && currentState != AdvertisementController.State.Complete && currentState != AdvertisementController.State.Error;
	}

	private void CheckBannersShowConditions()
	{
		if (PromoActionsManager.sharedManager == null)
		{
			return;
		}
		if (AdmobPerelivWindow.admobTexture != null && !string.IsNullOrEmpty(AdmobPerelivWindow.admobUrl))
		{
			if (Nest.Instance != null && Nest.Instance.BannerIsVisible)
			{
				if (Application.isEditor)
				{
					Debug.Log("Skipping fake interstitial while Nest Banner is active.");
				}
			}
			else if (!_bannerShowed[2])
			{
				_bannerShowed[2] = true;
				_needShowBanner[2] = true;
			}
		}
		CheckDownloadAdvertisement();
		if (IsAdvertisementDownloading() || AdsConfigManager.Instance.LastLoadedConfig == null)
		{
			return;
		}
		string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
		PerelivSettings perelivSettings = PerelivConfigManager.Instance.GetPerelivSettings(playerCategory);
		if (perelivSettings.Enabled && advertiseController.CurrentState == AdvertisementController.State.Complete)
		{
			if (ConnectSceneNGUIController.sharedController != null)
			{
				if (Application.isEditor)
				{
					Debug.Log("Skipping pereliv in Connect Scene.");
				}
			}
			else if (Nest.Instance != null && Nest.Instance.BannerIsVisible)
			{
				if (Application.isEditor)
				{
					Debug.Log("Skipping pereliv while Nest Banner is active.");
				}
			}
			else if (!_bannerShowed[5])
			{
				_bannerShowed[5] = true;
				_needShowBanner[5] = true;
			}
		}
		if (PlayerPrefs.GetInt("leave_from_duel_penalty") != 0 && (Nest.Instance == null || !Nest.Instance.BannerIsVisible) && !_bannerShowed[10])
		{
			_bannerShowed[10] = true;
			_needShowBanner[10] = true;
		}
		if (Nest.Instance == null || !Nest.Instance.BannerIsVisible)
		{
			if (TournamentAvailableBannerWindow.CanShow && !_bannerShowed[16])
			{
				_bannerShowed[16] = true;
				_needShowBanner[16] = true;
			}
			if (TournamentLooserBannerWindow.CanShow && !_bannerShowed[18])
			{
				_bannerShowed[18] = true;
				_needShowBanner[18] = true;
			}
			if (TournamentWinnerBannerWindow.CanShow && !_bannerShowed[17])
			{
				_bannerShowed[17] = true;
				_needShowBanner[17] = true;
			}
		}
		if (!_isBlockShowForNewPlayer)
		{
			if (!firstScreen && PromoActionsManager.sharedManager.IsDayOfValorEventActive && TrainingController.TrainingCompleted && PlayerPrefs.GetInt("DaysOfValorShownCount", 1) > 0 && !_bannerShowed[6])
			{
				_bannerShowed[6] = true;
				_needShowBanner[6] = true;
			}
			if (ConnectSceneNGUIController.sharedController != null && ConnectSceneNGUIController.isReturnFromGame && !ReviewController.IsNeedActive && !ReviewHUDWindow.isShow && PromoActionsManager.sharedManager.IsEventX3Active && TrainingController.TrainingCompleted && PlayerPrefs.GetInt(Defs.EventX3WindowShownCount, 1) > 0 && !_bannerShowed[7])
			{
				_bannerShowed[7] = true;
				_needShowBanner[7] = true;
			}
			if (GlobalGameController.NewVersionAvailable && PlayerPrefs.GetInt(Defs.UpdateAvailableShownTimesSN, 3) > 0 && !_bannerShowed[9])
			{
				_bannerShowed[9] = true;
				_needShowBanner[9] = true;
			}
			if (!firstScreen && StarterPackController.Get.IsNeedShowEventWindow() && !_bannerShowed[11])
			{
				_bannerShowed[11] = true;
				_needShowBanner[11] = true;
			}
		}
	}

	public void ResetStateBannerShowed(BannerWindowType windowType)
	{
		if (bannerWindows.Length >= 0 && (int)windowType <= bannerWindows.Length - 1)
		{
			_bannerShowed[(int)windowType] = false;
			_someBannerShown = false;
		}
	}

	public bool IsBannerShow(BannerWindowType bannerType)
	{
		if (_currentBanner == null)
		{
			return false;
		}
		return _currentBanner.type == bannerType;
	}

	public void ForceShowBanner(BannerWindowType windowType)
	{
		if (_currentBanner == null)
		{
			ShowBannerWindow(windowType);
		}
		else if (_currentBanner.type != windowType)
		{
			HideBannerWindow();
			ShowBannerWindow(windowType);
		}
	}

	internal void SubmitCurrentBanner()
	{
		if (!(_currentBanner == null))
		{
			_currentBanner.Submit();
		}
	}
}

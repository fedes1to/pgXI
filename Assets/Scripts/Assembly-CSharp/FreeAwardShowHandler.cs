using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

[DisallowMultipleComponent]
internal sealed class FreeAwardShowHandler : MonoBehaviour
{
	private enum SkipReason
	{
		None,
		CameraTouchOverGui,
		FriendsInterfaceEnabled,
		BankInterfaceEnabled,
		ShopInterfaceEnabled,
		RewardedVideoInterfaceEnabled,
		BannerEnabled,
		MainMenuComponentEnabled,
		LeaderboardEnabled,
		ProfileEnabled,
		NewsEnabled,
		LevelUpShown,
		AskNameWindow,
		TrainingMotCompleted,
		AdvertCountLessThanLimit,
		TimeTamperingDetected,
		ServerTime
	}

	public GameObject chestModelCoins;

	public GameObject chestModelGems;

	public GameObject freeAwardGuiPrefab;

	private NickLabelController _nickLabelValue;

	private SkipReason _lastSkipReasonLogged;

	private bool _cachedCheckIfVideoEnabled;

	private Collider _collider;

	public static FreeAwardShowHandler Instance { get; private set; }

	private NickLabelController NickLabel
	{
		get
		{
			if (_nickLabelValue == null)
			{
				if (NickLabelStack.sharedStack != null)
				{
					_nickLabelValue = NickLabelStack.sharedStack.GetNextCurrentLabel();
				}
				if (_nickLabelValue != null)
				{
					_nickLabelValue.StartShow(NickLabelController.TypeNickLabel.FreeCoins, base.gameObject.transform);
				}
			}
			return _nickLabelValue;
		}
	}

	public bool IsInteractable
	{
		get
		{
			if (HitCollider == null)
			{
				return false;
			}
			return HitCollider.enabled;
		}
		set
		{
			if (!(HitCollider == null))
			{
				HitCollider.enabled = value;
			}
		}
	}

	private Collider HitCollider
	{
		get
		{
			return _collider;
		}
	}

	private FreeAwardShowHandler()
	{
	}

	private void OnLocalizationChanged()
	{
	}

	private SkipReason GetReasonToDisableRewardedVideoInterface()
	{
		if (FriendsWindowGUI.Instance != null && FriendsWindowGUI.Instance.InterfaceEnabled)
		{
			return SkipReason.FriendsInterfaceEnabled;
		}
		if (BankController.Instance != null && BankController.Instance.InterfaceEnabled)
		{
			return SkipReason.BankInterfaceEnabled;
		}
		if (ShopNGUIController.sharedShop != null && ShopNGUIController.GuiActive)
		{
			return SkipReason.ShopInterfaceEnabled;
		}
		if (FreeAwardController.Instance != null)
		{
			if (!FreeAwardController.Instance.IsInState<FreeAwardController.IdleState>())
			{
				return SkipReason.RewardedVideoInterfaceEnabled;
			}
			if (!FreeAwardController.Instance.AdvertCountLessThanLimit())
			{
				return SkipReason.AdvertCountLessThanLimit;
			}
			if (FreeAwardController.Instance.TimeTamperingDetected())
			{
				return SkipReason.TimeTamperingDetected;
			}
		}
		if (BannerWindowController.SharedController != null && BannerWindowController.SharedController.IsAnyBannerShown)
		{
			return SkipReason.BannerEnabled;
		}
		if (AskNameManager.instance != null && !AskNameManager.isComplete)
		{
			return SkipReason.AskNameWindow;
		}
		MainMenuController sharedController = MainMenuController.sharedController;
		if (sharedController != null)
		{
			if (sharedController.stubLoading.activeInHierarchy)
			{
				return SkipReason.MainMenuComponentEnabled;
			}
			if (sharedController.FreePanelIsActive)
			{
				return SkipReason.MainMenuComponentEnabled;
			}
			if (sharedController.SettingsJoysticksPanel.activeInHierarchy || sharedController.settingsPanel.activeInHierarchy)
			{
				return SkipReason.MainMenuComponentEnabled;
			}
			if (sharedController.singleModePanel.gameObject.activeInHierarchy)
			{
				return SkipReason.MainMenuComponentEnabled;
			}
			if (sharedController.RentExpiredPoint.Map((Transform r) => r.childCount > 0))
			{
				return SkipReason.MainMenuComponentEnabled;
			}
			if (FeedbackMenuController.Instance != null && FeedbackMenuController.Instance.gameObject.activeInHierarchy)
			{
				return SkipReason.MainMenuComponentEnabled;
			}
		}
		if (LeaderboardScript.Instance != null && LeaderboardScript.Instance.UIEnabled)
		{
			return SkipReason.LeaderboardEnabled;
		}
		if (FriendsController.sharedController.Map((FriendsController c) => c.ProfileInterfaceActive))
		{
			return SkipReason.ProfileEnabled;
		}
		if (FriendsController.ServerTime == -1)
		{
			return SkipReason.ServerTime;
		}
		if (NewsLobbyController.sharedController != null && NewsLobbyController.sharedController.gameObject.activeInHierarchy)
		{
			return SkipReason.NewsEnabled;
		}
		if (ExpController.Instance != null && ExpController.Instance.IsLevelUpShown)
		{
			return SkipReason.LevelUpShown;
		}
		if (!TrainingController.TrainingCompleted)
		{
			return SkipReason.TrainingMotCompleted;
		}
		return SkipReason.None;
	}

	private void Awake()
	{
		Instance = this;
		if (FreeAwardController.Instance == null && freeAwardGuiPrefab != null)
		{
			UnityEngine.Object.Instantiate(freeAwardGuiPrefab, Vector3.zero, Quaternion.identity);
		}
		LocalizationManager.OnLocalizeEvent += OnLocalizationChanged;
	}

	private void OnEnable()
	{
		if (_collider == null)
		{
			_collider = base.gameObject.GetComponent<Collider>();
		}
		SkipReason reasonToDisableRewardedVideoInterface = GetReasonToDisableRewardedVideoInterface();
		if (reasonToDisableRewardedVideoInterface != 0)
		{
			Debug.LogFormat("Reason to disable rewarded video interface in lobby: {0}", reasonToDisableRewardedVideoInterface);
			SetInterfaceActive(false);
			return;
		}
		string reasonToDismissVideoChestInLobby = MobileAdManager.GetReasonToDismissVideoChestInLobby();
		if (!string.IsNullOrEmpty(reasonToDismissVideoChestInLobby))
		{
			Debug.LogFormat("Reason to dismiss rewarded video in lobby: {0}", reasonToDismissVideoChestInLobby);
			SetInterfaceActive(false);
		}
		else
		{
			SetInterfaceActive(true);
			StartCoroutine(SetupBillboard());
		}
	}

	private void Update()
	{
		SkipReason reasonToDisableRewardedVideoInterface = GetReasonToDisableRewardedVideoInterface();
		if (reasonToDisableRewardedVideoInterface != 0)
		{
			SetInterfaceActive(false);
			if (Application.isEditor && reasonToDisableRewardedVideoInterface != _lastSkipReasonLogged)
			{
				Debug.LogFormat("Reason to disable rewarded video interface: {0}", reasonToDisableRewardedVideoInterface);
				_lastSkipReasonLogged = reasonToDisableRewardedVideoInterface;
			}
		}
		else
		{
			if (Time.frameCount % 60 == 0)
			{
				string reasonToDismissVideoChestInLobby = MobileAdManager.GetReasonToDismissVideoChestInLobby();
				_cachedCheckIfVideoEnabled = string.IsNullOrEmpty(reasonToDismissVideoChestInLobby);
			}
			SetInterfaceActive(_cachedCheckIfVideoEnabled);
		}
	}

	private void OnDestroy()
	{
		LocalizationManager.OnLocalizeEvent -= OnLocalizationChanged;
		Instance = null;
	}

	private IEnumerator SetupBillboard()
	{
		while (MainMenuController.sharedController == null)
		{
			yield return null;
		}
		MainMenuController.sharedController.FreeAwardBindedBillboard.BindTo(() => base.gameObject.transform);
		_collider = MainMenuController.sharedController.FreeAwardBindedBillboard.Collider;
	}

	private void SetInterfaceActive(bool active)
	{
		if (HitCollider == null)
		{
			chestModelCoins.SetActiveSafe(false);
			chestModelGems.SetActiveSafe(false);
			NickLabel.gameObject.SetActiveSafe(false);
			return;
		}
		if (!active)
		{
			HitCollider.enabled = false;
			chestModelCoins.SetActiveSafe(false);
			chestModelGems.SetActiveSafe(false);
			NickLabel.gameObject.SetActiveSafe(false);
			return;
		}
		if (FreeAwardController.Instance == null)
		{
			HitCollider.enabled = false;
			chestModelCoins.SetActiveSafe(false);
			chestModelGems.SetActiveSafe(false);
			NickLabel.gameObject.SetActiveSafe(false);
			return;
		}
		HitCollider.enabled = true;
		chestModelCoins.SetActiveSafe(FreeAwardController.Instance.CurrencyForAward == "Coins");
		chestModelGems.SetActiveSafe(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency");
		if (!NickLabel.gameObject.activeInHierarchy)
		{
			NickLabel.gameObject.SetActive(true);
			if (FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")
			{
				NickLabel.freeAwardGemsLabel.SetActive(true);
				NickLabel.freeAwardCoinsLabel.SetActive(false);
			}
			else if (FreeAwardController.Instance.CurrencyForAward == "Coins")
			{
				NickLabel.freeAwardGemsLabel.SetActive(false);
				NickLabel.freeAwardCoinsLabel.SetActive(true);
			}
			else
			{
				NickLabel.freeAwardGemsLabel.SetActive(false);
				NickLabel.freeAwardCoinsLabel.SetActive(false);
			}
		}
	}

	public void OnClick()
	{
		SkipReason reasonToDisableRewardedVideoInterface = GetReasonToDisableRewardedVideoInterface();
		if (reasonToDisableRewardedVideoInterface != 0)
		{
			Debug.LogFormat("Reason to disabled rewarded video interface: {0}", reasonToDisableRewardedVideoInterface);
			return;
		}
		AdsConfigMemento lastLoadedConfig = AdsConfigManager.Instance.LastLoadedConfig;
		if (lastLoadedConfig == null)
		{
			Debug.LogWarning("config == null");
			return;
		}
		if (lastLoadedConfig.Exception != null)
		{
			Debug.LogWarning(lastLoadedConfig.Exception.Message);
			return;
		}
		string videoDisabledReason = AdsConfigManager.GetVideoDisabledReason(lastLoadedConfig);
		if (!string.IsNullOrEmpty(videoDisabledReason))
		{
			Debug.LogWarning(videoDisabledReason);
			return;
		}
		ChestInLobbyPointMemento chestInLobby = lastLoadedConfig.AdPointsConfig.ChestInLobby;
		if (chestInLobby == null)
		{
			Debug.LogWarning("pointConfig == null");
			return;
		}
		string playerCategory = AdsConfigManager.GetPlayerCategory(lastLoadedConfig);
		List<double> finalRewardedVideoDelayMinutes = chestInLobby.GetFinalRewardedVideoDelayMinutes(playerCategory);
		if (finalRewardedVideoDelayMinutes.Count == 0)
		{
			return;
		}
		DateTime date = DateTime.UtcNow.Date;
		KeyValuePair<int, DateTime> keyValuePair = FreeAwardController.Instance.LastAdvertShow(date);
		int num = Math.Max(0, keyValuePair.Key + 1);
		if (num <= finalRewardedVideoDelayMinutes.Count)
		{
			DateTime dateTime = ((!(keyValuePair.Value < date)) ? keyValuePair.Value : date);
			TimeSpan timeSpan = TimeSpan.FromMinutes(finalRewardedVideoDelayMinutes[num]);
			DateTime watchState = dateTime + timeSpan;
			FreeAwardController.Instance.SetWatchState(watchState);
			if (ButtonClickSound.Instance != null)
			{
				ButtonClickSound.Instance.PlayClick();
			}
		}
	}

	private void PlayeAnimationTitle(bool isReverse, GameObject titleLabel)
	{
		UIPlayTween component = titleLabel.GetComponent<UIPlayTween>();
		component.resetOnPlay = true;
		component.tweenGroup = (isReverse ? 1 : 0);
		component.Play(true);
	}

	private void PlayAnimationShowChest(bool isReserse)
	{
		GameObject gameObject = ((!(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")) ? chestModelCoins : chestModelGems);
		Animation component = gameObject.GetComponent<Animation>();
		if (!(component == null))
		{
			if (isReserse)
			{
				component["Animate"].speed = -1f;
				component["Animate"].time = component["Animate"].length;
			}
			else
			{
				component["Animate"].speed = 1f;
				component["Animate"].time = 0f;
			}
			component.Play();
		}
	}

	public static void CheckShowChest(bool interfaceEnabled)
	{
		if (interfaceEnabled && !(Instance == null))
		{
			Collider hitCollider = Instance.HitCollider;
			if (!(hitCollider == null) && hitCollider.enabled)
			{
				Instance.SetInterfaceActive(false);
			}
		}
	}
}

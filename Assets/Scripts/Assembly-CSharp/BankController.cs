using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class BankController : MonoBehaviour
{
	public const int InitialIosGems = 0;

	public const int InitialIosCoins = 0;

	public AbstractBankView bankViewCommon;

	public GameObject uiRoot;

	public ChestBonusView bonusDetailView;

	public static bool canShowIndication = true;

	private bool firsEnterToBankOccured;

	private float tmOfFirstEnterTheBank;

	private bool _lockInterfaceEnabledCoroutine;

	private int _counterEn;

	private IDisposable _backSubscription;

	private string m_lastPrintedDismissReason = string.Empty;

	private AbstractBankView m_currentBankView;

	private string _debugMessage = string.Empty;

	private bool _escapePressed;

	private static float _lastTimePurchaseButtonPressed;

	private float m_timeWhenReutrnedFromPause = float.MinValue;

	private static BankController _instance;

	private readonly Lazy<bool> _timeTamperingDetected = new Lazy<bool>(delegate
	{
		bool flag = FreeAwardController.Instance.TimeTamperingDetected();
		if (flag && Defs.IsDeveloperBuild)
		{
			Debug.LogWarning("FreeAwardController: time tampering detected in Bank.");
		}
		return flag;
	});

	private AbstractBankView ActualBankView
	{
		get
		{
			return bankViewCommon;
		}
	}

	public AbstractBankView CurrentBankView
	{
		get
		{
			return m_currentBankView;
		}
		private set
		{
			m_currentBankView = value;
		}
	}

	public bool InterfaceEnabled
	{
		get
		{
			return CurrentBankView != null && CurrentBankView.gameObject.activeInHierarchy;
		}
		set
		{
			SetInterfaceEnabledWithDesiredCurrency(value, null);
		}
	}

	public bool InterfaceEnabledCoroutineLocked
	{
		get
		{
			return _lockInterfaceEnabledCoroutine;
		}
	}

	public static BankController Instance
	{
		get
		{
			return _instance;
		}
	}

	private List<Dictionary<string, object>> CurrentInappBonuses { get; set; }

	private float TimeWhenReturnedFromPause
	{
		get
		{
			return m_timeWhenReutrnedFromPause;
		}
		set
		{
			m_timeWhenReutrnedFromPause = value;
		}
	}

	public static event Action onUpdateMoney;

	public event EventHandler BackRequested
	{
		add
		{
			if (bankViewCommon != null)
			{
				bankViewCommon.BackButtonPressed += value;
			}
			this.EscapePressed = (EventHandler)Delegate.Combine(this.EscapePressed, value);
		}
		remove
		{
			if (bankViewCommon != null)
			{
				bankViewCommon.BackButtonPressed -= value;
			}
			this.EscapePressed = (EventHandler)Delegate.Remove(this.EscapePressed, value);
		}
	}

	private event EventHandler EscapePressed;

	public static void UpdateAllIndicatorsMoney()
	{
		if (BankController.onUpdateMoney != null)
		{
			BankController.onUpdateMoney();
		}
	}

	public static void GiveInitialNumOfCoins()
	{
		if (!Storager.hasKey("Coins"))
		{
			Storager.setInt("Coins", 0, false);
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				PlayerPrefs.Save();
			}
		}
		if (!Storager.hasKey("GemsCurrency"))
		{
			switch (BuildSettings.BuildTargetPlatform)
			{
			case RuntimePlatform.IPhonePlayer:
				Storager.setInt("GemsCurrency", 0, false);
				break;
			case RuntimePlatform.Android:
				Storager.setInt("GemsCurrency", 0, false);
				break;
			}
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				PlayerPrefs.Save();
			}
		}
	}

	public void SetInterfaceEnabledWithDesiredCurrency(bool enabled, string desiredCurrency)
	{
		SetInterfaceEnabledCore(enabled, desiredCurrency);
	}

	private void SetInterfaceEnabledCore(bool value, string desiredCurrency)
	{
		if (!value && _backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
		_lockInterfaceEnabledCoroutine = true;
		int num = _counterEn++;
		Debug.Log("InterfaceEnabledCoroutine " + num + " start: " + value);
		try
		{
			if (value && !firsEnterToBankOccured)
			{
				firsEnterToBankOccured = true;
				tmOfFirstEnterTheBank = Time.realtimeSinceStartup;
			}
			if (value)
			{
				UpdateCurrenntInappBonus(null);
			}
			if (ActualBankView != CurrentBankView && CurrentBankView != null)
			{
				CurrentBankView.gameObject.SetActive(false);
				CurrentBankView = null;
			}
			if (ActualBankView != null)
			{
				if (value)
				{
					ActualBankView.DesiredCurrency = desiredCurrency;
				}
				ActualBankView.gameObject.SetActive(value);
				CurrentBankView = ((!value) ? null : ActualBankView);
			}
			uiRoot.SetActive(value);
			if (!value)
			{
				ActivityIndicator.IsActiveIndicator = false;
			}
			FreeAwardShowHandler.CheckShowChest(value);
			if (value)
			{
				coinsShop.thisScript.RefreshProductsIfNeed(false);
				OnEventX3AmazonBonusUpdated();
			}
		}
		finally
		{
			if (value)
			{
				if (_backSubscription != null)
				{
					_backSubscription.Dispose();
				}
				_backSubscription = BackSystem.Instance.Register(HandleEscape, "Bank");
			}
			if (Device.IsLoweMemoryDevice)
			{
				ActivityIndicator.ClearMemory();
			}
			_lockInterfaceEnabledCoroutine = false;
			Debug.Log("InterfaceEnabledCoroutine " + num + " stop: " + value);
		}
	}

	private void HandleEscape()
	{
		if (FreeAwardController.Instance != null && !FreeAwardController.Instance.IsInState<FreeAwardController.IdleState>())
		{
			FreeAwardController.Instance.HandleClose();
			_escapePressed = false;
		}
		else
		{
			_escapePressed = true;
		}
	}

	private void Awake()
	{
		BalanceController.UpdatedBankView += BalanceController_UpdatedBankView;
		InappBonuessController.OnGiveInappBonus += InappBonuessController_OnGiveInappBonus;
	}

	private void InappBonuessController_OnGiveInappBonus(InappRememberedBonus bonus)
	{
		TimeWhenReturnedFromPause = float.MinValue;
	}

	private void Start()
	{
		_instance = this;
		PromoActionsManager.EventX3Updated += OnEventX3Updated;
		if (bankViewCommon != null)
		{
			bankViewCommon.PurchaseButtonPressed += HandlePurchaseButtonPressed;
		}
		PromoActionsManager.EventAmazonX3Updated += OnEventX3AmazonBonusUpdated;
		bankViewCommon.freeAwardButton.gameObject.SetActiveSafeSelf(false);
	}

	private void BalanceController_UpdatedBankView()
	{
		try
		{
			CurrentInappBonuses = BalanceController.GetCurrentInnapBonus();
			if (CurrentBankView != null && InterfaceEnabled)
			{
				CurrentBankView.UpdateUi();
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in BalanceController_UpdatedBankView: {0}", ex);
		}
	}

	private void OnEventX3Updated()
	{
		try
		{
			if (CurrentBankView != null)
			{
				CurrentBankView.UpdateUi();
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in OnEventX3Updated: {0}", ex);
		}
	}

	private void OnEventX3AmazonBonusUpdated()
	{
		if (CurrentBankView == null || CurrentBankView.eventX3AmazonBonusWidget == null)
		{
			return;
		}
		try
		{
			GameObject gameObject = CurrentBankView.eventX3AmazonBonusWidget.gameObject;
			gameObject.SetActive(PromoActionsManager.sharedManager.IsAmazonEventX3Active);
			UILabel[] componentsInChildren = gameObject.GetComponentsInChildren<UILabel>();
			UILabel uILabel = CurrentBankView.Map((AbstractBankView b) => b.amazonEventCaptionLabel) ?? componentsInChildren.FirstOrDefault((UILabel l) => "CaptionLabel".Equals(l.name, StringComparison.OrdinalIgnoreCase));
			PromoActionsManager.AmazonEventInfo o = PromoActionsManager.sharedManager.Map((PromoActionsManager p) => p.AmazonEvent);
			if (uILabel != null)
			{
				uILabel.text = o.Map((PromoActionsManager.AmazonEventInfo e) => e.Caption) ?? string.Empty;
			}
			UILabel o2 = CurrentBankView.Map((AbstractBankView b) => b.amazonEventTitleLabel) ?? componentsInChildren.FirstOrDefault((UILabel l) => "TitleLabel".Equals(l.name, StringComparison.OrdinalIgnoreCase));
			UILabel[] array = o2.Map((UILabel t) => t.GetComponentsInChildren<UILabel>()) ?? new UILabel[0];
			float num = o.Map((PromoActionsManager.AmazonEventInfo e) => e.Percentage);
			string text = LocalizationStore.Get("Key_1672");
			UILabel[] array2 = array;
			foreach (UILabel uILabel2 in array2)
			{
				uILabel2.text = ("Key_1672".Equals(text, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(text, num));
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in OnEventX3AmazonBonusUpdated: {0}", ex);
		}
	}

	private void UpdateCurrenntInappBonus(Action onUpdate)
	{
		List<Dictionary<string, object>> currentInnapBonus = BalanceController.GetCurrentInnapBonus();
		if (!InappBonuessController.AreInappBonusesEquals(currentInnapBonus, CurrentInappBonuses))
		{
			if (onUpdate != null)
			{
				onUpdate();
			}
			CurrentInappBonuses = currentInnapBonus;
		}
	}

	private void CheckInappBonusActionChanged()
	{
		try
		{
			Action onUpdate = delegate
			{
				if (InterfaceEnabled && CurrentBankView != null)
				{
					CurrentBankView.UpdateUi();
				}
			};
			UpdateCurrenntInappBonus(onUpdate);
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in BankController.Update: {0}", ex);
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			TimeWhenReturnedFromPause = Time.realtimeSinceStartup;
		}
	}

	private void Update()
	{
		if (!InterfaceEnabled)
		{
			_escapePressed = false;
			return;
		}
		if (FriendsController.ServerTime != -1 || Time.realtimeSinceStartup - TimeWhenReturnedFromPause > 10f)
		{
			CheckInappBonusActionChanged();
		}
		string text = string.Empty;
		if (FreeAwardController.Instance == null)
		{
			CurrentBankView.freeAwardButton.gameObject.SetActiveSafeSelf(false);
		}
		else if (Defs.MainMenuScene != SceneManagerHelper.ActiveSceneName)
		{
			CurrentBankView.freeAwardButton.gameObject.SetActiveSafeSelf(false);
		}
		else if (!FreeAwardController.Instance.IsInState<FreeAwardController.IdleState>())
		{
			CurrentBankView.freeAwardButton.gameObject.SetActiveSafeSelf(false);
		}
		else if (FriendsController.ServerTime == -1)
		{
			CurrentBankView.freeAwardButton.gameObject.SetActiveSafeSelf(false);
		}
		else if (!TrainingController.TrainingCompleted)
		{
			CurrentBankView.freeAwardButton.gameObject.SetActiveSafeSelf(false);
		}
		else
		{
			text = MobileAdManager.GetReasonToDismissVideoChestInLobby();
			if (string.IsNullOrEmpty(text))
			{
				if (_timeTamperingDetected.Value)
				{
					text = "Time tampering detected.";
				}
				else if (!FreeAwardController.Instance.AdvertCountLessThanLimit())
				{
					text = "AdvertCountLessThanLimit == false";
					CurrentBankView.freeAwardButton.gameObject.SetActiveSafeSelf(false);
				}
				else
				{
					CurrentBankView.freeAwardButton.gameObject.SetActiveSafeSelf(true);
				}
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			CurrentBankView.freeAwardButton.gameObject.SetActiveSafeSelf(false);
			if (text != m_lastPrintedDismissReason)
			{
				string format = ((!Application.isEditor) ? "Dismissing interstitial. {0}" : "<color=magenta>Dismissing interstitial. {0}</color>");
				Debug.LogFormat(format, text);
				m_lastPrintedDismissReason = text;
			}
		}
		UpdateMiscUiOnView(bankViewCommon);
		EventHandler escapePressed = this.EscapePressed;
		if (_escapePressed && escapePressed != null)
		{
			escapePressed(this, EventArgs.Empty);
			_escapePressed = false;
		}
	}

	private void LateUpdate()
	{
		if (InterfaceEnabled && ExperienceController.sharedController != null && !_lockInterfaceEnabledCoroutine)
		{
			ExperienceController.sharedController.isShowRanks = false;
		}
	}

	private void UpdateMiscUiOnView(AbstractBankView bankView)
	{
		if (bankView == null || !bankView.gameObject.activeSelf)
		{
			return;
		}
		if (coinsShop.IsWideLayoutAvailable)
		{
			bankView.ConnectionProblemLabelEnabled = false;
			bankView.CrackersWarningLabelEnabled = true;
			bankView.NotEnoughCoinsLabelEnabled = false;
			bankView.NotEnoughGemsLabelEnabled = false;
			bankView.AreBankContentsEnabled = false;
			bankView.PurchaseSuccessfulLabelEnabled = false;
		}
		else
		{
			if (!(coinsShop.thisScript != null))
			{
				return;
			}
			bankView.NotEnoughCoinsLabelEnabled = coinsShop.thisScript.notEnoughCurrency == "Coins";
			bankView.NotEnoughGemsLabelEnabled = coinsShop.thisScript.notEnoughCurrency == "GemsCurrency";
			ActivityIndicator.IsActiveIndicator = StoreKitEventListener.purchaseInProcess;
			if (coinsShop.IsNoConnection)
			{
				if (Time.realtimeSinceStartup - tmOfFirstEnterTheBank > 3f)
				{
					bankView.ConnectionProblemLabelEnabled = true;
				}
				bankView.NotEnoughCoinsLabelEnabled = false;
				bankView.NotEnoughGemsLabelEnabled = false;
				bankView.AreBankContentsEnabled = false;
				bankView.PurchaseSuccessfulLabelEnabled = false;
			}
			else
			{
				bankView.ConnectionProblemLabelEnabled = false;
				bankView.AreBankContentsEnabled = true;
			}
			bankView.PurchaseSuccessfulLabelEnabled = coinsShop.thisScript.ProductPurchasedRecently;
		}
	}

	private void OnDestroy()
	{
		PromoActionsManager.EventX3Updated -= OnEventX3Updated;
		PromoActionsManager.EventAmazonX3Updated -= OnEventX3AmazonBonusUpdated;
		if (bankViewCommon != null)
		{
			bankViewCommon.PurchaseButtonPressed -= HandlePurchaseButtonPressed;
		}
		BalanceController.UpdatedBankView -= BalanceController_UpdatedBankView;
		InappBonuessController.OnGiveInappBonus -= InappBonuessController_OnGiveInappBonus;
	}

	private void HandlePurchaseButtonPressed(AbstractBankViewItem item)
	{
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64 && Time.realtimeSinceStartup - _lastTimePurchaseButtonPressed < 1f)
		{
			Debug.Log("Bank button pressed but ignored");
			return;
		}
		_lastTimePurchaseButtonPressed = Time.realtimeSinceStartup;
		if (StoreKitEventListener.purchaseInProcess)
		{
			Debug.Log("Cannot perform request while purchase is in progress.");
		}
		if (coinsShop.thisScript != null)
		{
			coinsShop.thisScript.HandlePurchaseButton(item.purchaseInfo.Index, item.purchaseInfo.Currency, item);
		}
		else
		{
			Debug.LogErrorFormat("HandlePurchaseButtonPressed coinsShop.thisScript == null");
		}
	}

	public static void AddCoins(int count, bool needIndication = true, AnalyticsConstants.AccrualType accrualType = AnalyticsConstants.AccrualType.Earned)
	{
		int @int = Storager.getInt("Coins", false);
		Storager.setInt("Coins", @int + count, false);
		AnalyticsFacade.CurrencyAccrual(count, "Coins", accrualType);
		if (needIndication)
		{
			CoinsMessage.FireCoinsAddedEvent(false, 2);
		}
	}

	public static void AddGems(int count, bool needIndication = true, AnalyticsConstants.AccrualType accrualType = AnalyticsConstants.AccrualType.Earned)
	{
		int @int = Storager.getInt("GemsCurrency", false);
		Storager.setInt("GemsCurrency", @int + count, false);
		AnalyticsFacade.CurrencyAccrual(count, "GemsCurrency", accrualType);
		if (needIndication)
		{
			CoinsMessage.FireCoinsAddedEvent(true);
		}
	}

	public static IEnumerator WaitForIndicationGems(bool isGems)
	{
		while (!canShowIndication)
		{
			yield return null;
		}
		CoinsMessage.FireCoinsAddedEvent(isGems);
	}

	public void FreeAwardButtonClick()
	{
		ButtonClickSound.TryPlayClick();
		if (FreeAwardController.Instance == null || !FreeAwardController.Instance.AdvertCountLessThanLimit() || AdsConfigManager.Instance.LastLoadedConfig == null)
		{
			return;
		}
		ChestInLobbyPointMemento chestInLobby = AdsConfigManager.Instance.LastLoadedConfig.AdPointsConfig.ChestInLobby;
		if (chestInLobby == null)
		{
			return;
		}
		string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
		List<double> finalRewardedVideoDelayMinutes = chestInLobby.GetFinalRewardedVideoDelayMinutes(playerCategory);
		if (finalRewardedVideoDelayMinutes.Count != 0)
		{
			DateTime date = DateTime.UtcNow.Date;
			KeyValuePair<int, DateTime> keyValuePair = FreeAwardController.Instance.LastAdvertShow(date);
			int num = Math.Max(0, keyValuePair.Key + 1);
			if (num <= finalRewardedVideoDelayMinutes.Count)
			{
				DateTime dateTime = ((!(keyValuePair.Value < date)) ? keyValuePair.Value : date);
				FreeAwardController.Instance.SetWatchState(dateTime + TimeSpan.FromMinutes(finalRewardedVideoDelayMinutes[num]));
			}
		}
	}
}

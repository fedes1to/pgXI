using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FyberPlugin;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

internal sealed class FreeAwardController : MonoBehaviour
{
	public class StateEventArgs : EventArgs
	{
		public State State { get; set; }

		public State OldState { get; set; }
	}

	public abstract class State
	{
	}

	public sealed class IdleState : State
	{
		private static readonly IdleState _instance = new IdleState();

		internal static IdleState Instance
		{
			get
			{
				return _instance;
			}
		}

		private IdleState()
		{
		}
	}

	public sealed class WatchState : State
	{
		private readonly DateTime _nextTimeEnabled;

		public WatchState(DateTime nextTimeEnabled)
		{
			DateTime utcNow = DateTime.UtcNow;
			if (Defs.IsDeveloperBuild && utcNow < nextTimeEnabled)
			{
				UnityEngine.Debug.Log("Watching state inactive: need to wait till UTC " + nextTimeEnabled.ToString("T", CultureInfo.InvariantCulture));
			}
			_nextTimeEnabled = nextTimeEnabled;
		}

		public TimeSpan GetEstimatedTimeSpan()
		{
			return _nextTimeEnabled - DateTime.UtcNow;
		}
	}

	public sealed class WaitingState : State
	{
		private readonly float _startTime;

		public float StartTime
		{
			get
			{
				return _startTime;
			}
		}

		public WaitingState(float startTime)
		{
			_startTime = startTime;
		}

		public WaitingState()
			: this(Time.realtimeSinceStartup)
		{
		}
	}

	public sealed class WatchingState : State
	{
		private readonly float _startTime;

		private readonly Promise<string> _adClosed = new Promise<string>();

		public Future<string> AdClosed
		{
			get
			{
				return _adClosed.Future;
			}
		}

		public float StartTime
		{
			get
			{
				return _startTime;
			}
		}

		public WatchingState()
		{
			_startTime = Time.realtimeSinceStartup;
			string arg = DetermineContext();
			Storager.setInt("PendingFreeAward", (int)Instance.Provider, false);
			if (Instance.Provider == AdProvider.GoogleMobileAds)
			{
				UnityEngine.Debug.Log("[Rilisoft] GoogleMobileAds are not supported directly.");
			}
			else if (Instance.Provider == AdProvider.UnityAds)
			{
				UnityEngine.Debug.Log("[Rilisoft] UnityAds are not supported directly.");
			}
			else if (Instance.Provider == AdProvider.Vungle)
			{
				UnityEngine.Debug.Log("[Rilisoft] Vungle is not supported directly.");
			}
			else
			{
				if (Instance.Provider != AdProvider.Fyber)
				{
					return;
				}
				AdvertisementInfo advertisementInfo = new AdvertisementInfo(0, 0);
				if (!FyberVideoLoaded.IsCompleted)
				{
					UnityEngine.Debug.LogWarning("FyberVideoLoaded.IsCompleted: False");
					return;
				}
				Ad ad = FyberVideoLoaded.Result as Ad;
				if (ad == null)
				{
					UnityEngine.Debug.LogWarningFormat("FyberVideoLoaded.Result: {0}", FyberVideoLoaded.Result);
					return;
				}
				Action<AdResult> adFinished = null;
				adFinished = delegate(AdResult adResult)
				{
					FyberCallback.AdFinished -= adFinished;
					LogImpressionDetails(advertisementInfo);
					if (adResult.Message == "CLOSE_FINISHED")
					{
						AnalyticsFacade.SendCustomEventToFacebook("rewarded_ads_watched_count", null);
					}
					_adClosed.SetResult(adResult.Message);
				};
				FyberCallback.AdFinished += adFinished;
				ad.Start();
				FyberVideoLoaded = null;
				Dictionary<string, string> eventParams = new Dictionary<string, string>
				{
					{ "af_content_type", "Rewarded video" },
					{
						"af_content_id",
						string.Format("Rewarded video ({0})", arg)
					}
				};
				AnalyticsFacade.SendCustomEventToAppsFlyer("af_content_view", eventParams);
			}
		}

		private static void LogImpressionDetails(AdvertisementInfo advertisementInfo)
		{
			if (advertisementInfo == null)
			{
				advertisementInfo = AdvertisementInfo.Default;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Round {0}", advertisementInfo.Round + 1);
			stringBuilder.AppendFormat(", Slot {0} ({1})", advertisementInfo.Slot + 1, AnalyticsHelper.GetAdProviderName(Instance.GetProviderByIndex(advertisementInfo.Slot)));
			if (InterstitialManager.Instance.Provider == AdProvider.GoogleMobileAds)
			{
				stringBuilder.AppendFormat(", Unit {0}", advertisementInfo.Unit + 1);
			}
			if (string.IsNullOrEmpty(advertisementInfo.Details))
			{
				stringBuilder.Append(" - Impression");
			}
			else
			{
				stringBuilder.AppendFormat(" - Impression: {0}", advertisementInfo.Details);
			}
		}

		internal void SimulateCallbackInEditor(string result)
		{
			if (Application.isEditor)
			{
				_adClosed.SetResult(result ?? string.Empty);
			}
		}

		private static string DetermineContext()
		{
			if (BankController.Instance != null && BankController.Instance.InterfaceEnabled)
			{
				if (Defs.isMulti)
				{
					return "Bank (Multiplayer)";
				}
				if (Defs.isCompany)
				{
					return "Bank (Campaign)";
				}
				if (Defs.IsSurvival)
				{
					return "Bank (Survival)";
				}
				return "Bank";
			}
			return "At Lobby";
		}
	}

	public sealed class ConnectionState : State
	{
		private readonly float _startTime;

		public float StartTime
		{
			get
			{
				return _startTime;
			}
		}

		public ConnectionState()
		{
			_startTime = Time.realtimeSinceStartup;
		}
	}

	public sealed class AwardState : State
	{
		private static readonly AwardState _instance = new AwardState();

		internal static AwardState Instance
		{
			get
			{
				return _instance;
			}
		}

		private AwardState()
		{
		}
	}

	public sealed class CloseState : State
	{
		private static readonly CloseState _instance = new CloseState();

		internal static CloseState Instance
		{
			get
			{
				return _instance;
			}
		}

		private CloseState()
		{
		}
	}

	[CompilerGenerated]
	private sealed class _003CStart_003Ec__Iterator165 : IDisposable, IEnumerator, IEnumerator<object>
	{
		internal DateTime _003CcurrentTime_003E__0;

		internal int _0024PC;

		internal object _0024current;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		public bool MoveNext()
		{
			//Discarded unreachable code: IL_010a
			uint num = (uint)_0024PC;
			_0024PC = -1;
			switch (num)
			{
			case 0u:
			case 1u:
				if (FriendsController.sharedController == null)
				{
					_0024current = null;
					_0024PC = 1;
					break;
				}
				goto case 2u;
			case 2u:
				if (string.IsNullOrEmpty(FriendsController.sharedController.id))
				{
					_0024current = null;
					_0024PC = 2;
					break;
				}
				CoroutineRunner.Instance.StartCoroutine(InitFyber());
				goto case 3u;
			case 3u:
				if (!_initializedOnce)
				{
					_0024current = null;
					_0024PC = 3;
					break;
				}
				goto case 4u;
			case 4u:
				if (FriendsController.ServerTime < 0)
				{
					_0024current = null;
					_0024PC = 4;
					break;
				}
				try
				{
					_003CcurrentTime_003E__0 = StarterPackModel.GetCurrentTimeByUnixTime((int)FriendsController.ServerTime);
					AddEmptyEntryForAdvertTime(_003CcurrentTime_003E__0);
				}
				finally
				{
					_003C_003E__Finally0();
				}
				_0024PC = -1;
				goto default;
			default:
				return false;
			}
			return true;
		}

		[DebuggerHidden]
		public void Dispose()
		{
			_0024PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		private void _003C_003E__Finally0()
		{
			RemoveOldEntriesForAdvertTimes();
		}
	}

	private const string AdvertTimeDuringCurrentPeriodKey = "AdvertTimeDuringCurrentPeriod";

	public const string PendingFreeAwardKey = "PendingFreeAward";

	public Camera renderCamera;

	public FreeAwardView view;

	private static FreeAwardController _instance;

	private int _adProviderIndex;

	private State _currentState = IdleState.Instance;

	private IDisposable _backSubscription;

	private static bool _initializedOnce;

	public static string appId = "32897";

	public static string securityToken = "cf77aeadd83faf98e0cad61a1f1403c8";

	private KeyValuePair<string, int> _advertCountCache = new KeyValuePair<string, int>(string.Empty, 0);

	private bool? _simplifiedInterfaceCache;

	private DateTime _currentTime;

	public static bool FreeAwardChestIsInIdleState
	{
		get
		{
			return Instance == null || Instance.IsInState<IdleState>();
		}
	}

	public static FreeAwardController Instance
	{
		get
		{
			return _instance;
		}
	}

	public AdProvider Provider
	{
		get
		{
			return GetProviderByIndex(_adProviderIndex);
		}
	}

	private State CurrentState
	{
		get
		{
			return _currentState;
		}
		set
		{
			if (value != null)
			{
				SetCacheDirty();
				if (_backSubscription != null)
				{
					_backSubscription.Dispose();
					_backSubscription = null;
				}
				if (!(value is IdleState))
				{
					_backSubscription = BackSystem.Instance.Register(HandleClose, "Rewarded Video");
				}
				if (view != null)
				{
					view.CurrentState = value;
				}
				State currentState = _currentState;
				_currentState = value;
				EventHandler<StateEventArgs> stateChanged = this.StateChanged;
				if (stateChanged != null)
				{
					stateChanged(this, new StateEventArgs
					{
						State = value,
						OldState = currentState
					});
				}
			}
		}
	}

	internal static Future<object> FyberVideoLoaded { get; set; }

	private DateTime CurrentTime
	{
		get
		{
			return DateTime.UtcNow;
		}
	}

	public static int CountMoneyForAward
	{
		get
		{
			if (AdsConfigManager.Instance.LastLoadedConfig == null)
			{
				return ChestInLobbyPointMemento.DefaultAward;
			}
			ChestInLobbyPointMemento chestInLobby = AdsConfigManager.Instance.LastLoadedConfig.AdPointsConfig.ChestInLobby;
			if (chestInLobby == null)
			{
				return ChestInLobbyPointMemento.DefaultAward;
			}
			string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
			return chestInLobby.GetFinalAward(playerCategory);
		}
	}

	public string CurrencyForAward
	{
		get
		{
			if (AdsConfigManager.Instance.LastLoadedConfig == null)
			{
				return ChestInLobbyPointMemento.DefaultCurrency;
			}
			ChestInLobbyPointMemento chestInLobby = AdsConfigManager.Instance.LastLoadedConfig.AdPointsConfig.ChestInLobby;
			if (chestInLobby == null)
			{
				return ChestInLobbyPointMemento.DefaultCurrency;
			}
			string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
			return chestInLobby.GetFinalAwardCurrency(playerCategory);
		}
	}

	internal bool SimplifiedInterface
	{
		get
		{
			if (!_simplifiedInterfaceCache.HasValue)
			{
				if (AdsConfigManager.Instance.LastLoadedConfig == null)
				{
					return ChestInLobbyPointMemento.DefaultSimplifiedInterface;
				}
				ChestInLobbyPointMemento chestInLobby = AdsConfigManager.Instance.LastLoadedConfig.AdPointsConfig.ChestInLobby;
				if (chestInLobby == null)
				{
					return ChestInLobbyPointMemento.DefaultSimplifiedInterface;
				}
				string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
				bool finalSimplifiedInterface = chestInLobby.GetFinalSimplifiedInterface(playerCategory);
				_simplifiedInterfaceCache = finalSimplifiedInterface;
			}
			return _simplifiedInterfaceCache.Value;
		}
	}

	public event EventHandler<StateEventArgs> StateChanged;

	public AdProvider GetProviderByIndex(int index)
	{
		return AdProvider.Fyber;
	}

	internal int SwitchAdProvider()
	{
		int adProviderIndex = _adProviderIndex;
		AdProvider provider = Provider;
		_adProviderIndex++;
		if (provider == AdProvider.GoogleMobileAds)
		{
			MobileAdManager.Instance.DestroyVideoInterstitial();
		}
		if (Provider == AdProvider.GoogleMobileAds)
		{
			MobileAdManager.Instance.SwitchVideoIdGroup();
		}
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("Switching provider from {0} ({1}) to {2} ({3})", adProviderIndex, provider, _adProviderIndex, Provider);
			UnityEngine.Debug.Log(message);
		}
		return _adProviderIndex;
	}

	private void ResetAdProvider()
	{
		int adProviderIndex = _adProviderIndex;
		AdProvider provider = Provider;
		_adProviderIndex = 0;
		AdProvider provider2 = Provider;
		if (provider == AdProvider.GoogleMobileAds && provider != provider2)
		{
			MobileAdManager.Instance.DestroyVideoInterstitial();
		}
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("Resetting AdProvider from {0} ({1}) to {2} ({3})", adProviderIndex, provider, _adProviderIndex, Provider);
			UnityEngine.Debug.Log(message);
		}
		MobileAdManager.Instance.ResetVideoAdUnitId();
	}

	public T TryGetState<T>() where T : State
	{
		return CurrentState as T;
	}

	public bool IsInState<T>() where T : State
	{
		return CurrentState is T;
	}

	internal void SetWatchState(DateTime nextTimeEnabled)
	{
		ResetAdProvider();
		WatchState watchState = (WatchState)(CurrentState = new WatchState(nextTimeEnabled));
		if (SimplifiedInterface)
		{
			TimeSpan estimatedTimeSpan = watchState.GetEstimatedTimeSpan();
			if (estimatedTimeSpan <= TimeSpan.FromMinutes(0.0))
			{
				HandleWatch();
			}
		}
	}

	private void LoadVideo(string callerName = null)
	{
		if (callerName == null)
		{
			callerName = string.Empty;
		}
		if (Instance.Provider == AdProvider.Fyber)
		{
			FyberVideoLoaded = LoadFyberVideo(callerName);
		}
	}

	public void HandleClose()
	{
		ButtonClickSound.TryPlayClick();
		if (IsInState<CloseState>())
		{
			HideButtonsShowAward();
		}
		if (!IsInState<AwardState>())
		{
			if (Provider == AdProvider.GoogleMobileAds)
			{
				MobileAdManager.Instance.DestroyVideoInterstitial();
			}
			CurrentState = IdleState.Instance;
		}
		else
		{
			HandleGetAward();
		}
	}

	public void HandleWatch()
	{
		LoadVideo("HandleWatch");
		CurrentState = new WaitingState();
	}

	public void HandleDeveloperSkip()
	{
		CurrentState = new WatchingState();
	}

	public int GiveAwardAndIncrementCount()
	{
		int result = AddAdvertTime(DateTime.UtcNow);
		if (CurrencyForAward == "GemsCurrency")
		{
			BankController.AddGems(CountMoneyForAward);
		}
		else
		{
			BankController.AddCoins(CountMoneyForAward);
		}
		Storager.setInt("PendingFreeAward", 0, false);
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
			PlayerPrefs.Save();
		}
		return result;
	}

	public void HandleGetAward()
	{
		int num = GiveAwardAndIncrementCount();
		AnalyticsStuff.LogDailyVideoRewarded(num);
		if (AdsConfigManager.Instance.LastLoadedConfig == null)
		{
			CurrentState = CloseState.Instance;
			return;
		}
		ChestInLobbyPointMemento chestInLobby = AdsConfigManager.Instance.LastLoadedConfig.AdPointsConfig.ChestInLobby;
		if (chestInLobby == null)
		{
			CurrentState = CloseState.Instance;
			return;
		}
		string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
		List<double> finalRewardedVideoDelayMinutes = chestInLobby.GetFinalRewardedVideoDelayMinutes(playerCategory);
		if (num < finalRewardedVideoDelayMinutes.Count)
		{
			ResetAdProvider();
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = TimeSpan.FromMinutes(finalRewardedVideoDelayMinutes[num]);
			DateTime dateTime = utcNow + timeSpan;
			bool flag = utcNow.Date < dateTime.Date;
			CurrentState = ((!flag) ? ((State)new WatchState(dateTime)) : ((State)CloseState.Instance));
			if (Defs.IsDeveloperBuild)
			{
				string text = Json.Serialize(finalRewardedVideoDelayMinutes);
				UnityEngine.Debug.LogFormat("HandleGetAward(): `utcNow`: {0:s}, `delay`: {1:f2}, `nextTimeEnabled`: {2:s}, `CurrentState`: {3}, `delays`: {4}, `newCount`: {5}", utcNow, timeSpan.TotalMinutes, dateTime, CurrentState, text, num);
			}
		}
		else
		{
			CurrentState = CloseState.Instance;
		}
	}

	internal static Future<object> LoadFyberVideo(string callerName = null)
	{
		if (callerName == null)
		{
			callerName = string.Empty;
		}
		Promise<object> promise = new Promise<object>();
		Action<Ad> onAdAvailable = null;
		Action<AdFormat> onAdNotAvailable = null;
		Action<RequestError> onRequestFail = null;
		onAdAvailable = delegate(Ad ad)
		{
			if (Defs.IsDeveloperBuild)
			{
				UnityEngine.Debug.LogFormat("LoadFyberVideo > AdAvailable: {{ format: {0}, placementId: '{1}' }}", ad.AdFormat, ad.PlacementId);
			}
			promise.SetResult(ad);
			FyberCallback.AdAvailable -= onAdAvailable;
			FyberCallback.AdNotAvailable -= onAdNotAvailable;
			FyberCallback.RequestFail -= onRequestFail;
		};
		onAdNotAvailable = delegate(AdFormat adFormat)
		{
			if (Defs.IsDeveloperBuild)
			{
				UnityEngine.Debug.LogFormat("LoadFyberVideo > AdNotAvailable: {{ format: {0} }}", adFormat);
			}
			promise.SetResult(adFormat);
			FyberCallback.AdAvailable -= onAdAvailable;
			FyberCallback.AdNotAvailable -= onAdNotAvailable;
			FyberCallback.RequestFail -= onRequestFail;
		};
		onRequestFail = delegate(RequestError requestError)
		{
			if (Defs.IsDeveloperBuild)
			{
				UnityEngine.Debug.LogFormat("LoadFyberVideo > RequestFail: {{ requestError: {0} }}", requestError.Description);
			}
			promise.SetResult(requestError);
			FyberCallback.AdAvailable -= onAdAvailable;
			FyberCallback.AdNotAvailable -= onAdNotAvailable;
			FyberCallback.RequestFail -= onRequestFail;
		};
		FyberCallback.AdAvailable += onAdAvailable;
		FyberCallback.AdNotAvailable += onAdNotAvailable;
		FyberCallback.RequestFail += onRequestFail;
		RequestFyberRewardedVideo(0);
		return promise.Future;
	}

	private static void RequestFyberRewardedVideo(int roundIndex)
	{
		RewardedVideoRequester.Create().NotifyUserOnCompletion(false).Request();
	}

	private void HideButtonsShowAward()
	{
		BankController instance = BankController.Instance;
		if (instance != null && instance.InterfaceEnabled)
		{
			instance.CurrentBankView.freeAwardButton.gameObject.SetActiveSafeSelf(false);
		}
	}

	internal bool AdvertCountLessThanLimit()
	{
		if (AdsConfigManager.Instance.LastLoadedConfig == null)
		{
			return false;
		}
		ChestInLobbyPointMemento chestInLobby = AdsConfigManager.Instance.LastLoadedConfig.AdPointsConfig.ChestInLobby;
		if (chestInLobby == null)
		{
			return false;
		}
		string playerCategory = AdsConfigManager.GetPlayerCategory(AdsConfigManager.Instance.LastLoadedConfig);
		List<double> finalRewardedVideoDelayMinutes = chestInLobby.GetFinalRewardedVideoDelayMinutes(playerCategory);
		int count = finalRewardedVideoDelayMinutes.Count;
		ProfilerSample profilerSample = new ProfilerSample("AdvertCountLessThanLimit()->GetAdvertCountDuringCurrentPeriod()");
		int advertCountDuringCurrentPeriod;
		try
		{
			advertCountDuringCurrentPeriod = GetAdvertCountDuringCurrentPeriod();
		}
		finally
		{
			profilerSample.Dispose();
		}
		if (advertCountDuringCurrentPeriod >= count)
		{
			return false;
		}
		DateTime currentTime = CurrentTime;
		TimeSpan timeSpan = TimeSpan.FromMinutes(finalRewardedVideoDelayMinutes[advertCountDuringCurrentPeriod]);
		bool flag = (currentTime + timeSpan).Date > currentTime.Date;
		return !flag;
	}

	internal bool TimeTamperingDetected()
	{
		if (!Storager.hasKey("AdvertTimeDuringCurrentPeriod"))
		{
			return false;
		}
		string @string = Storager.getString("AdvertTimeDuringCurrentPeriod", false);
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary == null)
		{
			return false;
		}
		string strB = dictionary.Keys.Min();
		string text = CurrentTime.ToString("yyyy-MM-dd");
		return text.CompareTo(strB) < 0;
	}

	private static void RemoveOldEntriesForAdvertTimes()
	{
		if (!Storager.hasKey("AdvertTimeDuringCurrentPeriod"))
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(Storager.getString("AdvertTimeDuringCurrentPeriod", false)) as Dictionary<string, object>;
		if (dictionary != null && dictionary.Keys.Count >= 2)
		{
			string maxKey = dictionary.Keys.Max();
			string[] array = dictionary.Keys.Where((string k) => !k.Equals(maxKey, StringComparison.Ordinal)).ToArray();
			string[] array2 = array;
			foreach (string key in array2)
			{
				dictionary.Remove(key);
			}
			string val = Json.Serialize(dictionary);
			Storager.setString("AdvertTimeDuringCurrentPeriod", val, false);
		}
	}

	private static void AddEmptyEntryForAdvertTime(DateTime date)
	{
		string dateKey = date.ToString("yyyy-MM-dd");
		Action action = delegate
		{
			Dictionary<string, object> obj = new Dictionary<string, object> { 
			{
				dateKey,
				new string[0]
			} };
			string val2 = Json.Serialize(obj);
			Storager.setString("AdvertTimeDuringCurrentPeriod", val2, false);
		};
		if (!Storager.hasKey("AdvertTimeDuringCurrentPeriod"))
		{
			action();
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(Storager.getString("AdvertTimeDuringCurrentPeriod", false)) as Dictionary<string, object>;
		if (dictionary == null)
		{
			action();
		}
		else if (!dictionary.ContainsKey(dateKey))
		{
			dictionary.Add(dateKey, new string[0]);
			string val = Json.Serialize(dictionary);
			Storager.setString("AdvertTimeDuringCurrentPeriod", val, false);
		}
	}

	private void Awake()
	{
		_currentTime = DateTime.UtcNow;
		if (_instance == null)
		{
			_instance = this;
		}
		CurrentState = IdleState.Instance;
	}

	private void OnDestroy()
	{
		_instance = null;
	}

	public static IEnumerator InitFyber()
	{
		while (!FriendsController.isReadABTestAdvertConfig)
		{
			yield return null;
		}
		yield return null;
		if (_initializedOnce)
		{
			yield break;
		}
		SetCookieAcceptPolicy();
		FyberLogger.EnableLogging(true);
		string userId = FriendsController.sharedController.id;
		if (!Application.isEditor)
		{
			AppsFlyer.setCustomerUserID(userId);
		}
		if (!TrainingController.TrainingCompleted || Initializer.Instance != null)
		{
			string messageFormat = ((!Application.isEditor) ? "{0}" : "<color=olive>{0}</color>");
			UnityEngine.Debug.LogFormat(messageFormat, "FreeAwardController: Postponing Fyber initialization till training is completed...");
			while (!TrainingController.TrainingCompleted || Initializer.Instance != null)
			{
				yield return null;
			}
		}
		string idTail = ((userId.Length <= 4) ? userId : userId.Substring(userId.Length - 4, 4));
		string payingBin = Storager.getInt("PayingUser", true).ToString(CultureInfo.InvariantCulture);
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"pub0",
				SystemInfo.deviceModel
			},
			{ "pub1", idTail },
			{ "pub2", userId },
			{ "pub3", payingBin }
		};
		if (Defs.IsDeveloperBuild)
		{
			UnityEngine.Debug.LogFormat("{0}", "FreeAwardController: Initializing Fyber...");
		}
		Fyber fyber = Fyber.With(appId).WithSecurityToken(securityToken).WithUserId(userId)
			.WithParameters(parameters);
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer)
		{
			fyber = fyber.WithManualPrecaching();
		}
		Settings settings = fyber.Start();
		if (Defs.IsDeveloperBuild)
		{
			UnityEngine.Debug.Log("Start Fyber with parameters: appId=" + appId + " securityToken=" + securityToken);
		}
		User.SetAppVersion(GlobalGameController.AppVersion);
		User.SetDevice(SystemInfo.deviceModel);
		User.PutCustomValue("pg3d_paying", payingBin);
		User.PutCustomValue("RandomKey", userId);
		_initializedOnce = true;
	}

	private IEnumerator Start()
	{
		while (FriendsController.sharedController == null)
		{
			yield return null;
		}
		while (string.IsNullOrEmpty(FriendsController.sharedController.id))
		{
			yield return null;
		}
		CoroutineRunner.Instance.StartCoroutine(InitFyber());
		while (!_initializedOnce)
		{
			yield return null;
		}
		while (FriendsController.ServerTime < 0)
		{
			yield return null;
		}
		try
		{
			DateTime currentTime = StarterPackModel.GetCurrentTimeByUnixTime((int)FriendsController.ServerTime);
			AddEmptyEntryForAdvertTime(currentTime);
		}
		finally
		{
//			((_003CStart_003Ec__Iterator165)(object)this)._003C_003E__Finally0();
		}
	}

	private void Update()
	{
		double num = 3.0;
		if (AdsConfigManager.Instance.LastLoadedConfig != null && AdsConfigManager.Instance.LastLoadedConfig.VideoConfig != null)
		{
			num = AdsConfigManager.Instance.LastLoadedConfig.VideoConfig.TimeoutWaitInSeconds;
		}
		WaitingState waitingState = TryGetState<WaitingState>();
		if (waitingState != null)
		{
			if (Application.isEditor || Tools.RuntimePlatform == RuntimePlatform.MetroPlayerX64)
			{
				if (!((double)(Time.realtimeSinceStartup - waitingState.StartTime) > num))
				{
					return;
				}
				if (Provider == AdProvider.GoogleMobileAds)
				{
					if (MobileAdManager.Instance.SwitchVideoAdUnitId())
					{
						SwitchAdProvider();
					}
				}
				else
				{
					SwitchAdProvider();
				}
				CurrentState = new ConnectionState();
			}
			else if (Provider == AdProvider.GoogleMobileAds)
			{
				if (MobileAdManager.Instance.VideoInterstitialState == MobileAdManager.State.Loaded)
				{
					CurrentState = new WatchingState();
				}
				else if (!string.IsNullOrEmpty(MobileAdManager.Instance.VideoAdFailedToLoadMessage))
				{
					if (Defs.IsDeveloperBuild)
					{
						string message = string.Format("Admob loading failed after {0:F3}s of {1}. Keep waiting.", Time.realtimeSinceStartup - waitingState.StartTime, num);
						UnityEngine.Debug.Log(message);
					}
					if (MobileAdManager.Instance.SwitchVideoAdUnitId())
					{
						int num2 = SwitchAdProvider();
						if (PromoActionsManager.MobileAdvert.AdProviders.Count > 0 && num2 >= PromoActionsManager.MobileAdvert.CountRoundReplaceProviders * PromoActionsManager.MobileAdvert.AdProviders.Count)
						{
							string message2 = string.Format("Reporting connection issues after {0} switches.  Providers count {1}, rounds count {2}", num2, PromoActionsManager.MobileAdvert.AdProviders.Count, PromoActionsManager.MobileAdvert.CountRoundReplaceProviders);
							UnityEngine.Debug.Log(message2);
							CurrentState = new ConnectionState();
							return;
						}
					}
					LoadVideo("Update");
					CurrentState = new WaitingState(waitingState.StartTime);
				}
				else if ((double)(Time.realtimeSinceStartup - waitingState.StartTime) > num)
				{
					if (MobileAdManager.Instance.SwitchVideoAdUnitId())
					{
						SwitchAdProvider();
					}
					CurrentState = new ConnectionState();
				}
			}
			else if (Provider == AdProvider.Fyber)
			{
				if (FyberVideoLoaded != null && FyberVideoLoaded.IsCompleted)
				{
					Ad ad = FyberVideoLoaded.Result as Ad;
					if (ad != null)
					{
						CurrentState = new WatchingState();
						return;
					}
					RequestError requestError = FyberVideoLoaded.Result as RequestError;
					if (Defs.IsDeveloperBuild)
					{
						UnityEngine.Debug.LogFormat("Fyber loading failed: {0}. Keep waiting.", (requestError != null) ? requestError.Description : ((!(FyberVideoLoaded.Result is AdFormat)) ? "?" : "Not available"));
					}
					int num3 = SwitchAdProvider();
					if (PromoActionsManager.MobileAdvert.AdProviders.Count > 0 && num3 >= PromoActionsManager.MobileAdvert.CountRoundReplaceProviders * PromoActionsManager.MobileAdvert.AdProviders.Count)
					{
						CurrentState = new ConnectionState();
						return;
					}
					LoadVideo("Update");
					CurrentState = new WaitingState(waitingState.StartTime);
				}
				else if ((double)(Time.realtimeSinceStartup - waitingState.StartTime) > num)
				{
					SwitchAdProvider();
					CurrentState = new ConnectionState();
				}
			}
			else if ((double)(Time.realtimeSinceStartup - waitingState.StartTime) > num)
			{
				CurrentState = new ConnectionState();
			}
			return;
		}
		WatchingState watchingState = TryGetState<WatchingState>();
		if (watchingState != null)
		{
			if (Application.isEditor && Time.realtimeSinceStartup - watchingState.StartTime > 1f)
			{
				watchingState.SimulateCallbackInEditor("CLOSE_FINISHED");
			}
			if (watchingState.AdClosed.IsCompleted)
			{
				if (Defs.IsDeveloperBuild)
				{
					UnityEngine.Debug.LogFormat("[Rilisoft] Watching rewarded video completed: '{0}'", watchingState.AdClosed.Result);
				}
				Storager.setInt("PendingFreeAward", 0, false);
				if (watchingState.AdClosed.Result.Equals("CLOSE_FINISHED", StringComparison.Ordinal))
				{
					CurrentState = AwardState.Instance;
				}
				else if (watchingState.AdClosed.Result.Equals("ERROR", StringComparison.Ordinal))
				{
					ResetAdProvider();
					CurrentState = new WatchState(DateTime.MinValue);
				}
				else if (watchingState.AdClosed.Result.Equals("CLOSE_ABORTED", StringComparison.Ordinal))
				{
					CurrentState = new WatchState(DateTime.MinValue);
				}
				else
				{
					string message3 = string.Format("[Rilisoft] Unsupported result for rewarded video: “{0}”", watchingState.AdClosed.Result);
					UnityEngine.Debug.LogWarning(message3);
					CurrentState = new WatchState(DateTime.MinValue);
				}
			}
		}
		else
		{
			ConnectionState connectionState = TryGetState<ConnectionState>();
			if (connectionState != null && Time.realtimeSinceStartup - connectionState.StartTime > 3f)
			{
				CurrentState = IdleState.Instance;
			}
		}
	}

	public int GetAdvertCountDuringCurrentPeriod()
	{
		if (!Storager.hasKey("AdvertTimeDuringCurrentPeriod"))
		{
			return 0;
		}
		string @string = Storager.getString("AdvertTimeDuringCurrentPeriod", false);
		if (_advertCountCache.Key == @string)
		{
			return _advertCountCache.Value;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary == null)
		{
			if (Application.isEditor || Defs.IsDeveloperBuild)
			{
				UnityEngine.Debug.LogWarningFormat("Cannot parse '{0}' to dictionary: {1}", "AdvertTimeDuringCurrentPeriod", @string);
			}
			int num = 0;
			_advertCountCache = new KeyValuePair<string, int>(@string, num);
			return num;
		}
		string text = CurrentTime.ToString("yyyy-MM-dd");
		string text2 = dictionary.Keys.Max();
		if (text.CompareTo(text2) < 0)
		{
			int result = int.MaxValue;
			int.TryParse(text2.Replace("-", string.Empty), out result);
			result = Math.Max(10000000, result);
			_advertCountCache = new KeyValuePair<string, int>(@string, result);
			return result;
		}
		object value;
		if (dictionary.TryGetValue(text, out value))
		{
			List<object> source = (value as List<object>) ?? new List<object>();
			int num2 = source.OfType<string>().Count();
			_advertCountCache = new KeyValuePair<string, int>(@string, num2);
			return num2;
		}
		int num3 = 0;
		_advertCountCache = new KeyValuePair<string, int>(@string, num3);
		return num3;
	}

	public int AddAdvertTime(DateTime time)
	{
		string key = time.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
		string item = time.ToString("T", CultureInfo.InvariantCulture);
		bool flag = Storager.hasKey("AdvertTimeDuringCurrentPeriod");
		if (!flag)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(key, new List<string>(1) { item });
			Dictionary<string, object> advertTime = dictionary;
			SetAdvertTime(advertTime);
			return 1;
		}
		string json = ((!flag) ? "{}" : Storager.getString("AdvertTimeDuringCurrentPeriod", false));
		Dictionary<string, object> dictionary2 = (Json.Deserialize(json) as Dictionary<string, object>) ?? new Dictionary<string, object>();
		object value;
		if (dictionary2.TryGetValue(key, out value))
		{
			List<object> source = (value as List<object>) ?? new List<object>();
			List<string> list = source.OfType<string>().ToList();
			list.Add(item);
			dictionary2[key] = list.ToList();
			SetAdvertTime(dictionary2);
			return list.Count;
		}
		dictionary2[key] = new List<string>(1) { item };
		SetAdvertTime(dictionary2);
		return 1;
	}

	public KeyValuePair<int, DateTime> LastAdvertShow(DateTime date)
	{
		KeyValuePair<int, DateTime> result = new KeyValuePair<int, DateTime>(int.MinValue, DateTime.MinValue);
		string key = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
		bool flag = Storager.hasKey("AdvertTimeDuringCurrentPeriod");
		if (!flag)
		{
			return result;
		}
		string json = ((!flag) ? "{}" : Storager.getString("AdvertTimeDuringCurrentPeriod", false));
		Dictionary<string, object> dictionary = (Json.Deserialize(json) as Dictionary<string, object>) ?? new Dictionary<string, object>();
		object value;
		if (dictionary.TryGetValue(key, out value))
		{
			List<object> list = (value as List<object>) ?? new List<object>();
			if (list.Count == 0)
			{
				return result;
			}
			List<string> list2 = list.OfType<string>().ToList();
			if (list2.Count == 0)
			{
				return result;
			}
			string text = list2.Max();
			DateTime result2;
			if (DateTime.TryParseExact(text, "T", CultureInfo.InvariantCulture, DateTimeStyles.None, out result2))
			{
				return new KeyValuePair<int, DateTime>(value: new DateTime(date.Year, date.Month, date.Day, result2.Hour, result2.Minute, result2.Second, DateTimeKind.Utc), key: list2.Count - 1);
			}
			UnityEngine.Debug.LogWarning("Couldnot parse last time advert shown: " + text);
			return result;
		}
		return result;
	}

	private void SetAdvertTime(Dictionary<string, object> d)
	{
		if (d == null)
		{
			d = new Dictionary<string, object>();
		}
		string val = Json.Serialize(d) ?? "{}";
		Storager.setString("AdvertTimeDuringCurrentPeriod", val, false);
	}

	private static void SetCookieAcceptPolicy()
	{
		if (Defs.IsDeveloperBuild)
		{
			UnityEngine.Debug.Log(string.Format("Setting cookie accept policy is dumb on {0}.", Application.platform));
		}
	}

	private void SetCacheDirty()
	{
		_simplifiedInterfaceCache = null;
	}
}

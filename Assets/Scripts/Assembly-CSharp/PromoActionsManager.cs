using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class PromoActionsManager : MonoBehaviour
{
	public sealed class AdvertInfo
	{
		public bool enabled;

		public string imageUrl;

		public string adUrl;

		public string message;

		public bool showAlways;

		public bool btnClose;

		public int minLevel;

		public int maxLevel;
	}

	public class MobileAdvertInfo
	{
		private List<string> _admobImageAdUnitIds = new List<string>();

		private List<string> _admobVideoAdUnitIds = new List<string>();

		private List<int> _adProviders = new List<int>();

		private double _daysOfBeingPayingUser = double.MaxValue;

		private List<int> _interstitialProviders = new List<int>();

		private List<List<string>> _admobImageIdGroups = new List<List<string>>();

		private List<List<string>> _admobVideoIdGroups = new List<List<string>>();

		[Obsolete]
		public bool ImageEnabled { get; set; }

		public bool VideoEnabled { get; set; }

		public bool VideoShowPaying { get; set; }

		public bool VideoShowNonpaying { get; set; }

		public int TimeoutBetweenShowInterstitial { get; set; }

		public int CountSessionNewPlayer { get; set; }

		public int CountRoundReplaceProviders { get; set; }

		public int TimeoutSkipVideoPaying { get; set; }

		public int TimeoutSkipVideoNonpaying { get; set; }

		public double ConnectSceneDelaySeconds { get; set; }

		public double DaysOfBeingPayingUser
		{
			get
			{
				return _daysOfBeingPayingUser;
			}
			set
			{
				_daysOfBeingPayingUser = Math.Max(0.0, value);
			}
		}

		public string AdmobVideoAdUnitId { get; set; }

		public List<string> AdmobImageAdUnitIds
		{
			get
			{
				return _admobImageAdUnitIds;
			}
			set
			{
				_admobImageAdUnitIds = value ?? new List<string>();
			}
		}

		public List<string> AdmobVideoAdUnitIds
		{
			get
			{
				return _admobVideoAdUnitIds;
			}
			set
			{
				_admobVideoAdUnitIds = value ?? new List<string>();
			}
		}

		public List<List<string>> AdmobImageIdGroups
		{
			get
			{
				return _admobImageIdGroups;
			}
			set
			{
				_admobImageIdGroups = value ?? new List<List<string>>();
			}
		}

		public List<List<string>> AdmobVideoIdGroups
		{
			get
			{
				return _admobVideoIdGroups;
			}
			set
			{
				_admobVideoIdGroups = value ?? new List<List<string>>();
			}
		}

		public int AdProvider { get; set; }

		public List<int> AdProviders
		{
			get
			{
				return _adProviders;
			}
			set
			{
				_adProviders = value ?? new List<int>();
			}
		}

		public List<int> InterstitialProviders
		{
			get
			{
				return _interstitialProviders;
			}
			set
			{
				_interstitialProviders = value ?? new List<int>();
			}
		}

		public float TimeoutWaitingInterstitialAfterMatchSeconds { get; set; }

		public double MinMatchDurationMinutes { get; set; }
	}

	internal sealed class ReplaceAdmobPerelivInfo
	{
		public bool enabled;

		public readonly List<string> imageUrls = new List<string>();

		public readonly List<string> adUrls = new List<string>();

		public int ShowEveryTimes { get; set; }

		public int ShowTimesTotal { get; set; }

		public bool ShowToPaying { get; set; }

		public bool ShowToNew { get; set; }

		public int MinLevel { get; set; }

		public int MaxLevel { get; set; }
	}

	internal sealed class AmazonEventInfo
	{
		public DateTime StartTime { get; set; }

		public DateTime EndTime
		{
			get
			{
				return StartTime + TimeSpan.FromSeconds(DurationSeconds);
			}
		}

		public float DurationSeconds { get; set; }

		public List<int> Timezones { get; set; }

		public float Percentage { get; set; }

		public string Caption { get; set; }

		public AmazonEventInfo()
		{
			StartTime = DateTime.MaxValue;
			Timezones = new List<int>();
			Caption = string.Empty;
		}
	}

	public delegate void OnDayOfValorEnableDelegate(bool enable);

	private const float OffersUpdateTimeout = 900f;

	private const float EventX3UpdateTimeout = 930f;

	private const float AdvertInfoTimeout = 960f;

	public const long NewbieEventX3Duration = 259200L;

	public const long NewbieEventX3Timeout = 259200L;

	private const float BestBuyInfoTimeout = 1020f;

	public const int ShownCountDaysOfValor = 1;

	private const float DayOfValorInfoTimeout = 1050f;

	private const string UNLOCKED_ITEMS_MANAGEMENT_STORAGER_KEY = "PromoActionsManager.UNLOCKED_ITEMS_MANAGEMENT_STORAGER_KEY";

	private const string UNLOCKED_ITEMS_KEY = "UnlockedItems";

	public static PromoActionsManager sharedManager;

	public static bool ActionsAvailable = true;

	public Dictionary<string, List<SaltedInt>> discounts = new Dictionary<string, List<SaltedInt>>();

	public List<string> topSellers = new List<string>();

	private float startTime;

	private string promoActionAddress = URLs.PromoActionsTest;

	private float _eventX3GetInfoStartTime;

	private float _eventX3LastCheckTime;

	private long _newbieEventX3StartTime;

	private long _newbieEventX3StartTimeAdditional;

	private long _eventX3StartTime;

	private long _eventX3Duration;

	private bool _eventX3Active;

	private long _eventX3AmazonEventStartTime;

	private long _eventX3AmazonEventEndTime;

	private readonly List<long> _eventX3AmazonEventValidTimeZone = new List<long>();

	private bool _eventX3AmazonEventActive;

	private float _advertGetInfoStartTime;

	private static readonly AdvertInfo _paidAdvert = new AdvertInfo();

	private static readonly AdvertInfo _freeAdvert = new AdvertInfo();

	private static readonly ReplaceAdmobPerelivInfo _replaceAdmobPereliv = new ReplaceAdmobPerelivInfo();

	private static readonly MobileAdvertInfo _mobileAdvert = new MobileAdvertInfo();

	public static float startupTime = 0f;

	private bool _isGetEventX3InfoRunning;

	private AmazonEventInfo _amazonEventInfo;

	public static bool x3InfoDownloadaedOnceDuringCurrentRun = false;

	private bool _isGetAdvertInfoRunning;

	private string _previousResponseText;

	private List<string> _bestBuyIds = new List<string>();

	private bool _isGetBestBuyInfoRunning;

	private float _bestBuyGetInfoStartTime;

	private long _dayOfValorStartTime;

	private long _dayOfValorEndTime;

	private long _dayOfValorMultiplyerForExp;

	private long _dayOfValorMultiplyerForMoney;

	private bool _isGetDayOfValorInfoRunning;

	private float _dayOfValorGetInfoStartTime;

	private static TimeSpan TimeToShowDaysOfValor = TimeSpan.FromHours(12.0);

	private TimeSpan _timeToEndDayOfValor;

	private List<string> m_itemsToRemoveFromUnlocked = new List<string>();

	private List<string> m_unlockedItems = new List<string>();

	private List<string> m_news = new List<string>
	{
		"mercenary", "mercenary_up1", "mercenary_up2", "photon_sniper_rifle", "photon_sniper_rifle_up1", "photon_sniper_rifle_up2", "subzero", "subzero_up1", "subzero_up2", "gadget_ninjashurickens",
		"gadget_ninjashurickens_up1"
	};

	public List<string> news
	{
		get
		{
			return m_news;
		}
	}

	public bool IsEventX3Active
	{
		get
		{
			return _eventX3Active;
		}
	}

	public List<string> UnlockedItems
	{
		get
		{
			return m_unlockedItems;
		}
		private set
		{
			m_unlockedItems = value;
		}
	}

	public List<string> ItemsToRemoveFromUnlocked
	{
		get
		{
			return m_itemsToRemoveFromUnlocked;
		}
		private set
		{
			m_itemsToRemoveFromUnlocked = value;
		}
	}

	public bool IsAmazonEventX3Active
	{
		get
		{
			if (_amazonEventInfo == null)
			{
				return false;
			}
			if (_amazonEventInfo.DurationSeconds <= float.Epsilon)
			{
				return false;
			}
			if (!CheckTimezone(_amazonEventInfo.Timezones))
			{
				return false;
			}
			DateTime utcNow = DateTime.UtcNow;
			return _amazonEventInfo.StartTime <= utcNow && utcNow <= _amazonEventInfo.EndTime;
		}
	}

	public long EventX3RemainedTime
	{
		get
		{
			if (IsEventX3Active)
			{
				return _eventX3StartTime + _eventX3Duration - CurrentUnixTime;
			}
			return 0L;
		}
	}

	public static AdvertInfo Advert
	{
		get
		{
			return (!StoreKitEventListener.IsPayingUser()) ? _freeAdvert : _paidAdvert;
		}
	}

	internal static ReplaceAdmobPerelivInfo ReplaceAdmobPereliv
	{
		get
		{
			return _replaceAdmobPereliv;
		}
	}

	public static MobileAdvertInfo MobileAdvert
	{
		get
		{
			return _mobileAdvert;
		}
	}

	public static bool MobileAdvertIsReady { get; private set; }

	internal AmazonEventInfo AmazonEvent
	{
		get
		{
			return _amazonEventInfo;
		}
	}

	public bool IsNewbieEventX3Active
	{
		get
		{
			if (_newbieEventX3StartTime == 0L)
			{
				return false;
			}
			long currentUnixTime = CurrentUnixTime;
			long num = _newbieEventX3StartTime + 259200 + 259200;
			if (currentUnixTime >= num)
			{
				ResetNewbieX3StartTime();
				return false;
			}
			return _newbieEventX3StartTime < currentUnixTime && currentUnixTime < _newbieEventX3StartTime + 259200;
		}
	}

	private bool IsX3StartTimeAfterNewbieX3TimeoutEndTime
	{
		get
		{
			if (_newbieEventX3StartTimeAdditional == 0L)
			{
				return true;
			}
			long num = _newbieEventX3StartTimeAdditional + 259200 + 259200;
			return _eventX3StartTime >= num;
		}
	}

	public static long CurrentUnixTime
	{
		get
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return (long)(DateTime.UtcNow - dateTime).TotalSeconds;
		}
	}

	public bool IsDayOfValorEventActive { get; private set; }

	public int DayOfValorMultiplyerForExp
	{
		get
		{
			if (!IsDayOfValorEventActive)
			{
				return 1;
			}
			return (int)_dayOfValorMultiplyerForExp;
		}
	}

	public int DayOfValorMultiplyerForMoney
	{
		get
		{
			if (!IsDayOfValorEventActive)
			{
				return 1;
			}
			return (int)_dayOfValorMultiplyerForMoney;
		}
	}

	public static event Action OnLockedItemsUpdated;

	public static event Action ActionsUUpdated;

	public static event Action EventX3Updated;

	public static event Action EventAmazonX3Updated;

	public static event Action BestBuyStateUpdate;

	public static event OnDayOfValorEnableDelegate OnDayOfValorEnable;

	public static void FireUnlockedItemsUpdated()
	{
		Action onLockedItemsUpdated = PromoActionsManager.OnLockedItemsUpdated;
		if (onLockedItemsUpdated != null)
		{
			onLockedItemsUpdated();
		}
	}

	public void RemoveItemFromUnlocked(string item)
	{
		try
		{
			UnlockedItems.Remove(item);
			ItemsToRemoveFromUnlocked.Remove(item);
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in RemoveItemFromUnlocked: {0}", ex);
		}
	}

	public void ReplaceUnlockedItemsWith(List<string> itemsViewed)
	{
		try
		{
			UnlockedItems.Clear();
			UnlockedItems.AddRange(itemsViewed);
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in ReplaceUnlockedItemsWith: {0}", ex);
		}
	}

	public void RemoveViewedUnlockedItems()
	{
		ItemsToRemoveFromUnlocked.Clear();
	}

	public int ItemsViewed(List<string> itemsViewed)
	{
		//Discarded unreachable code: IL_0073, IL_0095
		try
		{
			List<string> itemsToRemoveFromUnlocked = UnlockedItems.Intersect(itemsViewed).ToList();
			int num = itemsToRemoveFromUnlocked.Count();
			if (num > 0)
			{
				UnlockedItems.RemoveAll((string item) => itemsToRemoveFromUnlocked.Contains(item));
				List<string> second = itemsToRemoveFromUnlocked.ToList();
				ItemsToRemoveFromUnlocked = ItemsToRemoveFromUnlocked.Union(second).ToList();
			}
			return num;
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in ItemsViewed: {0}", ex);
			return 0;
		}
	}

	private void Awake()
	{
		LoadUnlockedItems();
		startupTime = Time.realtimeSinceStartup;
		promoActionAddress = URLs.PromoActions;
	}

	private void Start()
	{
		sharedManager = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Task futureToWait = PersistentCacheManager.Instance.StartDownloadSignaturesLoop();
		StartCoroutine(GetActionsLoop(futureToWait));
		StartCoroutine(GetEventX3InfoLoop());
		StartCoroutine(GetAdvertInfoLoop(futureToWait));
		StartCoroutine(AdsConfigManager.Instance.GetAdvertInfoLoop(futureToWait));
		StartCoroutine(PerelivConfigManager.Instance.GetPerelivConfigLoop(futureToWait));
		StartCoroutine(GetBestBuyInfoLoop(futureToWait));
		StartCoroutine(GetDayOfValorInfoLoop());
	}

	private void Update()
	{
		if (Time.realtimeSinceStartup - _eventX3LastCheckTime >= 1f)
		{
			CheckEventX3Active();
			if (Time.frameCount % 120 == 0)
			{
				RefreshAmazonEvent();
			}
			CheckDayOfValorActive();
			_eventX3LastCheckTime = Time.realtimeSinceStartup;
		}
	}

	private IEnumerator OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			yield return null;
			yield return null;
			yield return null;
			StartCoroutine(GetActions());
			StartCoroutine(GetEventX3Info());
			StartCoroutine(GetAmazonEventCoroutine());
			StartCoroutine(GetAdvertInfo());
			StartCoroutine(DownloadBestBuyInfo());
			StartCoroutine(DownloadDayOfValorInfo());
		}
		else
		{
			SaveUnlockedItems();
		}
	}

	private IEnumerator GetActionsLoop(Task futureToWait)
	{
		yield return new WaitUntil(() => futureToWait.IsCompleted);
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage < TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
		{
			yield return null;
		}
		while (true)
		{
			StartCoroutine(GetActions());
			while (Time.realtimeSinceStartup - startTime < 900f)
			{
				yield return null;
			}
		}
	}

	private IEnumerator GetEventX3InfoLoop()
	{
		UpdateNewbieEventX3Info();
		while (true)
		{
			yield return StartCoroutine(GetEventX3Info());
			yield return StartCoroutine(GetAmazonEventCoroutine());
			while (Time.realtimeSinceStartup - _eventX3GetInfoStartTime < 930f)
			{
				yield return null;
			}
		}
	}

	private IEnumerator GetAdvertInfoLoop(Task futureToWait)
	{
		yield return new WaitUntil(() => futureToWait.IsCompleted);
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage <= TrainingController.NewTrainingCompletedStage.None)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(GetAdvertInfo());
			while (Time.realtimeSinceStartup - _advertGetInfoStartTime < 960f)
			{
				yield return null;
			}
		}
	}

	private IEnumerator GetAmazonEventCoroutine()
	{
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android || Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
		{
			yield break;
		}
		WWW response = Tools.CreateWwwIfNotConnected(URLs.AmazonEvent);
		if (response == null)
		{
			yield break;
		}
		yield return response;
		string responseText = URLs.Sanitize(response);
		if (!string.IsNullOrEmpty(response.error))
		{
			Debug.LogWarning("Amazon event response error: " + response.error);
			yield break;
		}
		if (string.IsNullOrEmpty(responseText))
		{
			Debug.LogWarning("Amazon event response is empty");
			yield break;
		}
		Dictionary<string, object> amazonEvent = Json.Deserialize(responseText) as Dictionary<string, object>;
		if (amazonEvent == null)
		{
			Debug.LogWarning("Amazon event bad response: " + responseText);
		}
		else
		{
			if (!IsNeedCheckAmazonEventX3())
			{
				yield break;
			}
			_amazonEventInfo = new AmazonEventInfo();
			object startTimeObj;
			if (amazonEvent.TryGetValue("startTime", out startTimeObj))
			{
				try
				{
					_amazonEventInfo.StartTime = Convert.ToDateTime(startTimeObj, CultureInfo.InvariantCulture);
				}
				catch (Exception ex6)
				{
					Exception ex3 = ex6;
					Debug.LogException(ex3);
				}
			}
			object durationSecondsObj;
			if (amazonEvent.TryGetValue("durationSeconds", out durationSecondsObj))
			{
				try
				{
					_amazonEventInfo.DurationSeconds = Convert.ToSingle(durationSecondsObj);
				}
				catch (Exception ex7)
				{
					Exception ex5 = ex7;
					Debug.LogException(ex5);
				}
			}
			object timezonesObj;
			if (amazonEvent.TryGetValue("timezones", out timezonesObj))
			{
				List<object> timezonesRaw = (timezonesObj as List<object>) ?? new List<object>();
				try
				{
					_amazonEventInfo.Timezones = timezonesRaw.ConvertAll(Convert.ToInt32);
				}
				catch (Exception ex4)
				{
					Debug.LogException(ex4);
				}
			}
			object percentageObj;
			if (amazonEvent.TryGetValue("percentage", out percentageObj))
			{
				try
				{
					_amazonEventInfo.Percentage = Convert.ToSingle(percentageObj);
				}
				catch (Exception ex8)
				{
					Exception ex2 = ex8;
					Debug.LogException(ex2);
				}
			}
			object captionObj;
			if (amazonEvent.TryGetValue("caption", out captionObj))
			{
				try
				{
					_amazonEventInfo.Caption = Convert.ToString(captionObj, CultureInfo.InvariantCulture) ?? string.Empty;
				}
				catch (Exception ex9)
				{
					Exception ex = ex9;
					Debug.LogException(ex);
				}
			}
			RefreshAmazonEvent();
		}
	}

	private IEnumerator GetEventX3Info()
	{
		if (_isGetEventX3InfoRunning)
		{
			yield break;
		}
		_eventX3GetInfoStartTime = Time.realtimeSinceStartup;
		_isGetEventX3InfoRunning = true;
		if (string.IsNullOrEmpty(URLs.EventX3))
		{
			_isGetEventX3InfoRunning = false;
			yield break;
		}
		WWW response = Tools.CreateWwwIfNotConnected(URLs.EventX3);
		yield return response;
		string responseText = URLs.Sanitize(response);
		if (response == null || !string.IsNullOrEmpty(response.error))
		{
			if (Application.isEditor)
			{
				Debug.LogWarningFormat("EventX3 response error: {0}", (response == null) ? "null" : response.error);
			}
			_isGetEventX3InfoRunning = false;
			_eventX3GetInfoStartTime = Time.realtimeSinceStartup - 930f + 15f;
			yield break;
		}
		if (string.IsNullOrEmpty(responseText))
		{
			Debug.LogWarning("EventX3 response is empty");
			_isGetEventX3InfoRunning = false;
			yield break;
		}
		object eventX3InfoObj = Json.Deserialize(responseText);
		Dictionary<string, object> eventX3Info = eventX3InfoObj as Dictionary<string, object>;
		if (eventX3Info == null || !eventX3Info.ContainsKey("start") || !eventX3Info.ContainsKey("duration"))
		{
			Debug.LogWarning("EventX3 response is bad");
			_isGetEventX3InfoRunning = false;
			yield break;
		}
		long startTime = (long)eventX3Info["start"];
		long duration = (long)eventX3Info["duration"];
		_eventX3StartTime = startTime;
		_eventX3Duration = duration;
		CheckEventX3Active();
		x3InfoDownloadaedOnceDuringCurrentRun = true;
		_isGetEventX3InfoRunning = false;
	}

	private bool IsNeedCheckAmazonEventX3()
	{
		if (Defs.IsDeveloperBuild)
		{
			return true;
		}
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
		{
			return false;
		}
		if (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
		{
			return false;
		}
		return true;
	}

	private static bool CheckTimezone(List<int> timezones)
	{
		if (timezones == null)
		{
			return false;
		}
		return timezones.Any(DateTimeOffset.Now.Offset.Hours.Equals);
	}

	private bool CheckAvailabelTimeZoneForAmazonEvent()
	{
		if (!_eventX3Active)
		{
			return false;
		}
		if (_eventX3AmazonEventValidTimeZone == null || _eventX3AmazonEventValidTimeZone.Count == 0)
		{
			return false;
		}
		TimeSpan offset = DateTimeOffset.Now.Offset;
		for (int i = 0; i < _eventX3AmazonEventValidTimeZone.Count; i++)
		{
			long num = _eventX3AmazonEventValidTimeZone[i];
			if (num == offset.Hours)
			{
				return true;
			}
		}
		return false;
	}

	private void ParseAmazonEventData(Dictionary<string, object> jsonData)
	{
		if (jsonData.ContainsKey("startAmazonEventTime"))
		{
			_eventX3AmazonEventStartTime = (long)jsonData["startAmazonEventTime"];
		}
		if (jsonData.ContainsKey("endAmazonEventTime"))
		{
			_eventX3AmazonEventEndTime = (long)jsonData["endAmazonEventTime"];
		}
		if (jsonData.ContainsKey("timeZonesForEventAmazon"))
		{
			List<object> list = jsonData["timeZonesForEventAmazon"] as List<object>;
			for (int i = 0; i < list.Count; i++)
			{
				_eventX3AmazonEventValidTimeZone.Add((long)list[i]);
			}
		}
	}

	private void RefreshAmazonEvent()
	{
		if (PromoActionsManager.EventAmazonX3Updated != null)
		{
			PromoActionsManager.EventAmazonX3Updated();
		}
	}

	[Obsolete]
	private void CheckAmazonEventX3Active()
	{
		if (!_eventX3Active || !CheckAvailabelTimeZoneForAmazonEvent())
		{
			_eventX3AmazonEventActive = false;
			return;
		}
		bool eventX3AmazonEventActive = _eventX3AmazonEventActive;
		if (_eventX3AmazonEventStartTime != 0L && _eventX3AmazonEventEndTime != 0L)
		{
			long currentUnixTime = CurrentUnixTime;
			_eventX3AmazonEventActive = _eventX3StartTime < currentUnixTime && currentUnixTime < _eventX3AmazonEventEndTime;
		}
		else
		{
			_eventX3AmazonEventStartTime = 0L;
			_eventX3AmazonEventEndTime = 0L;
			_eventX3AmazonEventActive = false;
		}
		if (_eventX3AmazonEventActive != eventX3AmazonEventActive && PromoActionsManager.EventAmazonX3Updated != null)
		{
			PromoActionsManager.EventAmazonX3Updated();
		}
	}

	public void ForceCheckEventX3Active()
	{
		CheckEventX3Active();
	}

	private void CheckEventX3Active()
	{
		bool eventX3Active = _eventX3Active;
		if (IsNewbieEventX3Active)
		{
			_eventX3StartTime = _newbieEventX3StartTime;
			_eventX3Duration = 259200L;
			_eventX3Active = true;
		}
		else if (_eventX3StartTime != 0L && _eventX3Duration != 0L && IsX3StartTimeAfterNewbieX3TimeoutEndTime)
		{
			long currentUnixTime = CurrentUnixTime;
			_eventX3Active = _eventX3StartTime < currentUnixTime && currentUnixTime < _eventX3StartTime + _eventX3Duration;
		}
		else
		{
			_eventX3StartTime = 0L;
			_eventX3Duration = 0L;
			_eventX3Active = false;
		}
		if (_eventX3Active != eventX3Active)
		{
			if (_eventX3Active)
			{
				PlayerPrefs.SetInt(Defs.EventX3WindowShownCount, 1);
				PlayerPrefs.Save();
			}
			if (PromoActionsManager.EventX3Updated != null)
			{
				PromoActionsManager.EventX3Updated();
			}
		}
	}

	private void ResetNewbieX3StartTime()
	{
		if (_newbieEventX3StartTime != 0L)
		{
			Storager.setString(Defs.NewbieEventX3StartTime, 0L.ToString(), false);
			_newbieEventX3StartTime = 0L;
		}
	}

	public static long GetUnixTimeFromStorage(string storageId)
	{
		long result = 0L;
		if (Storager.hasKey(storageId))
		{
			string @string = Storager.getString(storageId, false);
			long.TryParse(@string, out result);
		}
		return result;
	}

	public void UpdateNewbieEventX3Info()
	{
		_newbieEventX3StartTime = GetUnixTimeFromStorage(Defs.NewbieEventX3StartTime);
		_newbieEventX3StartTimeAdditional = GetUnixTimeFromStorage(Defs.NewbieEventX3StartTimeAdditional);
	}

	private long GetNewbieEventX3LastLoggedTime()
	{
		if (_newbieEventX3StartTime != 0L)
		{
			return GetUnixTimeFromStorage(Defs.NewbieEventX3LastLoggedTime);
		}
		return 0L;
	}

	private IEnumerator GetAdvertInfo()
	{
		if (_isGetAdvertInfoRunning)
		{
			yield break;
		}
		_advertGetInfoStartTime = Time.realtimeSinceStartup;
		_isGetAdvertInfoRunning = true;
		_paidAdvert.enabled = false;
		_freeAdvert.enabled = false;
		string url = URLs.Advert;
		if (string.IsNullOrEmpty(url))
		{
			_isGetAdvertInfoRunning = false;
			yield break;
		}
		string cachedResponse = PersistentCacheManager.Instance.GetValue(url);
		string responseText;
		if (!string.IsNullOrEmpty(cachedResponse))
		{
			responseText = cachedResponse;
		}
		else
		{
			WWW response = Tools.CreateWwwIfNotConnected(url);
			yield return response;
			if (response == null || !string.IsNullOrEmpty(response.error))
			{
				Debug.LogWarningFormat("Advert response error: {0}", (response == null) ? "null" : response.error);
				_isGetAdvertInfoRunning = false;
				yield break;
			}
			responseText = URLs.Sanitize(response);
			if (string.IsNullOrEmpty(responseText))
			{
				Debug.LogWarning("Advert response is empty");
				_isGetAdvertInfoRunning = false;
				yield break;
			}
			PersistentCacheManager.Instance.SetValue(response.url, responseText);
		}
		object advertInfoObj = Json.Deserialize(responseText);
		Dictionary<string, object> advertInfo = advertInfoObj as Dictionary<string, object>;
		if (advertInfoObj == null)
		{
			_isGetAdvertInfoRunning = false;
			yield break;
		}
		if (advertInfo.ContainsKey("paid"))
		{
			ParseAdvertInfo(advertInfo["paid"], _paidAdvert);
		}
		if (advertInfo.ContainsKey("free"))
		{
			ParseAdvertInfo(advertInfo["free"], _freeAdvert);
		}
		if (advertInfo.ContainsKey("replace_admob_pereliv_10_2_0"))
		{
			Dictionary<string, object> replaceAdmob = advertInfo["replace_admob_pereliv_10_2_0"] as Dictionary<string, object>;
			ParseReplaceAdmobPereliv(replaceAdmob, _replaceAdmobPereliv);
		}
		else
		{
			Debug.Log("Advert response doesn't contain “replace_admob_pereliv_10_2_0” property.");
		}
		_isGetAdvertInfoRunning = false;
		MobileAdvertIsReady = true;
	}

	private static void ParseReplaceAdmobPereliv(Dictionary<string, object> replaceAdmob, ReplaceAdmobPerelivInfo replaceAdmobObj)
	{
		if (replaceAdmob != null)
		{
			try
			{
				List<string> list = (replaceAdmob["imageUrls"] as List<object>).OfType<string>().ToList();
				foreach (string item in list)
				{
					replaceAdmobObj.imageUrls.Add(item);
				}
				List<string> list2 = (replaceAdmob["adUrls"] as List<object>).OfType<string>().ToList();
				foreach (string item2 in list2)
				{
					replaceAdmobObj.adUrls.Add(item2);
				}
				replaceAdmobObj.enabled = Convert.ToBoolean(replaceAdmob["enabled"]);
				replaceAdmobObj.ShowEveryTimes = Mathf.Max(Convert.ToInt32(replaceAdmob["showEveryTimes"]), 1);
				replaceAdmobObj.ShowTimesTotal = Mathf.Max(Convert.ToInt32(replaceAdmob["showTimesTotal"]), 0);
				replaceAdmobObj.ShowToPaying = Convert.ToBoolean(replaceAdmob["showToPaying"]);
				replaceAdmobObj.ShowToNew = Convert.ToBoolean(replaceAdmob["showToNew"]);
				try
				{
					replaceAdmobObj.MinLevel = Convert.ToInt32(replaceAdmob["minLevel"]);
				}
				catch
				{
					replaceAdmobObj.MinLevel = -1;
				}
				try
				{
					replaceAdmobObj.MaxLevel = Convert.ToInt32(replaceAdmob["maxLevel"]);
					return;
				}
				catch
				{
					replaceAdmobObj.MaxLevel = -1;
					return;
				}
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
				return;
			}
		}
		Debug.LogWarning("replaceAdmob == null");
	}

	private void ParseAdvertInfo(object advertInfoObj, AdvertInfo advertInfo)
	{
		Dictionary<string, object> dictionary = advertInfoObj as Dictionary<string, object>;
		if (dictionary != null)
		{
			advertInfo.imageUrl = Convert.ToString(dictionary["imageUrl"]);
			advertInfo.adUrl = Convert.ToString(dictionary["adUrl"]);
			advertInfo.message = Convert.ToString(dictionary["text"]);
			advertInfo.showAlways = Convert.ToBoolean(dictionary["showAlways"]);
			advertInfo.btnClose = Convert.ToBoolean(dictionary["btnClose"]);
			advertInfo.minLevel = Convert.ToInt32(dictionary["minLevel"]);
			advertInfo.maxLevel = Convert.ToInt32(dictionary["maxLevel"]);
			advertInfo.enabled = Convert.ToBoolean(dictionary["enabled"]);
		}
	}

	private void ClearAll()
	{
		discounts.Clear();
		topSellers.Clear();
	}

	public static List<string> AllIdsForPromosExceptArmor()
	{
		IEnumerable<string> second = from kvp in WeaponManager.tagToStoreIDMapping
			where kvp.Value != null && WeaponManager.storeIDtoDefsSNMapping.ContainsKey(kvp.Value)
			select kvp.Key;
		return Wear.wear.SelectMany((KeyValuePair<ShopNGUIController.CategoryNames, List<List<string>>> kvp) => (kvp.Key == ShopNGUIController.CategoryNames.ArmorCategory) ? new List<List<string>>() : kvp.Value).SelectMany((List<string> list) => list).Except(new List<string> { "hat_Adamant_3" })
			.Except(Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0])
			.Concat(SkinsController.shopKeyFromNameSkin.Values)
			.Concat(second)
			.Concat(WeaponSkinsManager.AllSkins.Select((WeaponSkin info) => info.Id))
			.Concat(GadgetsInfo.info.Keys)
			.Distinct()
			.ToList();
	}

	public IEnumerator GetActions()
	{
		startTime = Time.realtimeSinceStartup;
		string cachedResponse = PersistentCacheManager.Instance.GetValue(promoActionAddress);
		string responseText;
		if (!string.IsNullOrEmpty(cachedResponse))
		{
			responseText = cachedResponse;
		}
		else
		{
			WWW download = Tools.CreateWwwIfNotConnected(promoActionAddress);
			if (download == null)
			{
				yield break;
			}
			yield return download;
			string response = URLs.Sanitize(download);
			if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
			{
				Debug.Log("GetActions response:    " + response);
			}
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogWarning("GetActions error:    " + download.error);
				}
				ClearAll();
				ActionsAvailable = false;
				yield break;
			}
			responseText = response;
			PersistentCacheManager.Instance.SetValue(download.url, responseText);
		}
		if (_previousResponseText != null && responseText != null && responseText == _previousResponseText)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("GetActions:    responseText == _previousResponseText");
			}
			yield break;
		}
		_previousResponseText = responseText;
		ActionsAvailable = true;
		ClearAll();
		Dictionary<string, object> actions = Json.Deserialize(responseText) as Dictionary<string, object>;
		if (actions == null)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning(" GetActions actions = null");
			}
			yield break;
		}
		object discountsObj;
		if (actions.TryGetValue("discounts_up", out discountsObj))
		{
			List<object> discountsObjList = discountsObj as List<object>;
			if (discountsObjList != null)
			{
				try
				{
					var discountsAsObjects = (from list in discountsObjList.OfType<List<object>>()
						where list.Count > 1
						select new
						{
							ItemID = ((list[0] as string) ?? string.Empty),
							Discount = Convert.ToInt32((long)list[1])
						}).ToList();
					var shmotEntry = discountsAsObjects.FirstOrDefault(entry => entry.ItemID == "shmot");
					var armorEntry = discountsAsObjects.FirstOrDefault(entry => entry.ItemID == "armor");
					if (shmotEntry != null)
					{
						IEnumerable<string> idsNotInDiscounts = AllIdsForPromosExceptArmor().Except(discountsAsObjects.Select(item => item.ItemID));
						foreach (string item2 in idsNotInDiscounts)
						{
							discountsAsObjects.Add(new
							{
								ItemID = item2,
								Discount = shmotEntry.Discount
							});
						}
						discountsAsObjects.RemoveAll(item => item.ItemID == "shmot");
					}
					if (armorEntry != null)
					{
						IEnumerable<string> armorIdsNotInDiscounts = Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].Except(discountsAsObjects.Select(item => item.ItemID));
						foreach (string armorItem in armorIdsNotInDiscounts)
						{
							discountsAsObjects.Add(new
							{
								ItemID = armorItem,
								Discount = armorEntry.Discount
							});
						}
						discountsAsObjects.RemoveAll(item => item.ItemID == "armor");
					}
					foreach (var discount in discountsAsObjects)
					{
						List<SaltedInt> vals = new List<SaltedInt>
						{
							new SaltedInt(11259645, discount.Discount)
						};
						discounts.Add(discount.ItemID, vals);
					}
				}
				catch (Exception ex)
				{
					Exception e = ex;
					Debug.LogError("Exception in processing discounts from server: " + e);
				}
			}
		}
		object topSellersObj;
		if (actions.TryGetValue("topSellers_up", out topSellersObj))
		{
			List<object> topSellersObjList = topSellersObj as List<object>;
			if (topSellersObjList != null)
			{
				foreach (string tg in topSellersObjList)
				{
					topSellers.Add(tg);
				}
			}
		}
		if (PromoActionsManager.ActionsUUpdated != null)
		{
			PromoActionsManager.ActionsUUpdated();
		}
	}

	private IEnumerator DownloadBestBuyInfo()
	{
		if (_isGetBestBuyInfoRunning)
		{
			yield break;
		}
		_bestBuyGetInfoStartTime = Time.realtimeSinceStartup;
		_isGetBestBuyInfoRunning = true;
		string url = URLs.BestBuy;
		if (string.IsNullOrEmpty(url))
		{
			_isGetBestBuyInfoRunning = false;
			yield break;
		}
		string cachedResponse = PersistentCacheManager.Instance.GetValue(url);
		string responseText;
		if (!string.IsNullOrEmpty(cachedResponse))
		{
			responseText = cachedResponse;
		}
		else
		{
			WWW response = Tools.CreateWwwIfNotConnected(URLs.BestBuy);
			yield return response;
			if (response == null || !string.IsNullOrEmpty(response.error))
			{
				Debug.LogWarningFormat("Best buy response error: {0}", (response == null) ? "null" : response.error);
				_bestBuyIds.Clear();
				_isGetBestBuyInfoRunning = false;
				yield break;
			}
			responseText = URLs.Sanitize(response);
			if (string.IsNullOrEmpty(responseText))
			{
				Debug.LogWarning("Best buy response is empty");
				_bestBuyIds.Clear();
				_isGetBestBuyInfoRunning = false;
				yield break;
			}
			PersistentCacheManager.Instance.SetValue(url, responseText);
		}
		object bestBuyInfoObj = Json.Deserialize(responseText);
		Dictionary<string, object> bestBuyInfo = bestBuyInfoObj as Dictionary<string, object>;
		if (bestBuyInfo == null || !bestBuyInfo.ContainsKey("bestBuy"))
		{
			Debug.LogWarning("Best buy response is bad");
			_bestBuyIds.Clear();
			_isGetBestBuyInfoRunning = false;
			yield break;
		}
		List<object> bestBuyItemObjects = bestBuyInfo["bestBuy"] as List<object>;
		if (bestBuyItemObjects != null)
		{
			_bestBuyIds.Clear();
			for (int i = 0; i < bestBuyItemObjects.Count; i++)
			{
				string bestBuyId = (string)bestBuyItemObjects[i];
				_bestBuyIds.Add(bestBuyId);
			}
		}
		if (PromoActionsManager.BestBuyStateUpdate != null)
		{
			PromoActionsManager.BestBuyStateUpdate();
		}
		_isGetBestBuyInfoRunning = false;
	}

	public bool IsBankItemBestBuy(PurchaseEventArgs purchaseInfo)
	{
		if (_bestBuyIds.Count == 0 || purchaseInfo == null)
		{
			return false;
		}
		string arg = ((!(purchaseInfo.Currency == "GemsCurrency")) ? "coins" : "gems");
		string item = string.Format("{0}_{1}", arg, purchaseInfo.Index + 1);
		return _bestBuyIds.Contains(item);
	}

	private IEnumerator GetBestBuyInfoLoop(Task futureToWait)
	{
		Task futureToWait2 = default(Task);
		yield return new WaitUntil(() => futureToWait.IsCompleted);
		while (true)
		{
			yield return StartCoroutine(DownloadBestBuyInfo());
			while (Time.realtimeSinceStartup - _bestBuyGetInfoStartTime < 1020f)
			{
				yield return null;
			}
		}
	}

	private void ClearDataDayOfValor()
	{
		_dayOfValorStartTime = 0L;
		_dayOfValorEndTime = 0L;
		_dayOfValorMultiplyerForExp = 1L;
		_dayOfValorMultiplyerForMoney = 1L;
	}

	private IEnumerator DownloadDayOfValorInfo()
	{
		if (_isGetDayOfValorInfoRunning)
		{
			yield break;
		}
		_dayOfValorGetInfoStartTime = Time.realtimeSinceStartup;
		_isGetDayOfValorInfoRunning = true;
		if (string.IsNullOrEmpty(URLs.DayOfValor))
		{
			_isGetDayOfValorInfoRunning = false;
			yield break;
		}
		WWW response = Tools.CreateWwwIfNotConnected(URLs.DayOfValor);
		yield return response;
		string responseText = URLs.Sanitize(response);
		if (response == null || !string.IsNullOrEmpty(response.error))
		{
			Debug.LogWarningFormat("Day of valor response error: {0}", (response == null) ? "null" : response.error);
			_isGetDayOfValorInfoRunning = false;
			ClearDataDayOfValor();
			yield break;
		}
		if (string.IsNullOrEmpty(responseText))
		{
			Debug.LogWarning("Best buy response is empty");
			_isGetDayOfValorInfoRunning = false;
			ClearDataDayOfValor();
			yield break;
		}
		object dayOfValorInfoObj = Json.Deserialize(responseText);
		Dictionary<string, object> dayOfValorInfo = dayOfValorInfoObj as Dictionary<string, object>;
		if (dayOfValorInfo == null)
		{
			Debug.LogWarning("Day of valor response is bad");
			_isGetDayOfValorInfoRunning = false;
			ClearDataDayOfValor();
			yield break;
		}
		ClearDataDayOfValor();
		if (dayOfValorInfo.ContainsKey("startTime"))
		{
			_dayOfValorStartTime = (long)dayOfValorInfo["startTime"];
		}
		if (dayOfValorInfo.ContainsKey("endTime"))
		{
			_dayOfValorEndTime = (long)dayOfValorInfo["endTime"];
		}
		if (dayOfValorInfo.ContainsKey("multiplyerForExp"))
		{
			_dayOfValorMultiplyerForExp = (long)dayOfValorInfo["multiplyerForExp"];
		}
		if (dayOfValorInfo.ContainsKey("multiplyerForMoney"))
		{
			_dayOfValorMultiplyerForMoney = (long)dayOfValorInfo["multiplyerForMoney"];
		}
		_isGetDayOfValorInfoRunning = false;
	}

	private IEnumerator GetDayOfValorInfoLoop()
	{
		while (!TrainingController.TrainingCompleted)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(DownloadDayOfValorInfo());
			while (Time.realtimeSinceStartup - _dayOfValorGetInfoStartTime < 1050f)
			{
				yield return null;
			}
		}
	}

	private void CheckDayOfValorActive()
	{
		bool isDayOfValorEventActive = IsDayOfValorEventActive;
		if (_dayOfValorStartTime != 0L && _dayOfValorEndTime != 0L && ExpController.LobbyLevel >= 1)
		{
			long currentUnixTime = CurrentUnixTime;
			IsDayOfValorEventActive = _dayOfValorStartTime < currentUnixTime && currentUnixTime < _dayOfValorEndTime;
			_timeToEndDayOfValor = TimeSpan.FromSeconds(_dayOfValorEndTime - currentUnixTime);
		}
		else
		{
			ClearDataDayOfValor();
			IsDayOfValorEventActive = false;
		}
		if (IsDayOfValorEventActive != isDayOfValorEventActive && PromoActionsManager.OnDayOfValorEnable != null)
		{
			PromoActionsManager.OnDayOfValorEnable(IsDayOfValorEventActive);
		}
	}

	public static void UpdateDaysOfValorShownCondition()
	{
		string @string = PlayerPrefs.GetString("LastTimeShowDaysOfValor", string.Empty);
		if (!string.IsNullOrEmpty(@string))
		{
			DateTime result = default(DateTime);
			if (DateTime.TryParse(@string, out result) && DateTime.UtcNow - result >= TimeToShowDaysOfValor)
			{
				PlayerPrefs.SetInt("DaysOfValorShownCount", 1);
			}
		}
	}

	public string GetTimeToEndDaysOfValor()
	{
		if (!IsDayOfValorEventActive)
		{
			return string.Empty;
		}
		if (_timeToEndDayOfValor.Days > 0)
		{
			return string.Format("{0} days {1:00}:{2:00}:{3:00}", _timeToEndDayOfValor.Days, _timeToEndDayOfValor.Hours, _timeToEndDayOfValor.Minutes, _timeToEndDayOfValor.Seconds);
		}
		return string.Format("{0:00}:{1:00}:{2:00}", _timeToEndDayOfValor.Hours, _timeToEndDayOfValor.Minutes, _timeToEndDayOfValor.Seconds);
	}

	internal void SaveUnlockedItems()
	{
		try
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			dictionary.Add("UnlockedItems", UnlockedItems);
			Dictionary<string, List<string>> obj = dictionary;
			Storager.setString("PromoActionsManager.UNLOCKED_ITEMS_MANAGEMENT_STORAGER_KEY", Json.Serialize(obj), false);
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in SaveUnlockedItems: {0}", ex);
		}
	}

	private void LoadUnlockedItems()
	{
		try
		{
			if (!Storager.hasKey("PromoActionsManager.UNLOCKED_ITEMS_MANAGEMENT_STORAGER_KEY"))
			{
				Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
				dictionary.Add("UnlockedItems", new List<string>());
				Dictionary<string, List<string>> obj = dictionary;
				Storager.setString("PromoActionsManager.UNLOCKED_ITEMS_MANAGEMENT_STORAGER_KEY", Json.Serialize(obj), false);
			}
			string @string = Storager.getString("PromoActionsManager.UNLOCKED_ITEMS_MANAGEMENT_STORAGER_KEY", false);
			Dictionary<string, object> dictionary2 = Json.Deserialize(@string) as Dictionary<string, object>;
			UnlockedItems = (dictionary2["UnlockedItems"] as List<object>).OfType<string>().ToList();
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in LoadUnlockedItems: {0}", ex);
			m_unlockedItems = new List<string>();
		}
	}
}

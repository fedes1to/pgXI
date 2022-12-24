using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Rilisoft;
using Rilisoft.MiniJson;
using Rilisoft.NullExtensions;
using SimpleJSON;
using UnityEngine;

public sealed class FriendsController : MonoBehaviour
{
	public enum PossiblleOrigin
	{
		None,
		Local,
		Facebook,
		RandomPlayer
	}

	public enum NotConnectCondition
	{
		level,
		platform,
		map,
		clientVersion,
		InChat,
		None
	}

	public class ResultParseOnlineData
	{
		public string mapIndex;

		public bool isPlayerInChat;

		public NotConnectCondition notConnectCondition;

		private string _gameRegim;

		private string _gameMode;

		public string gameMode
		{
			get
			{
				return _gameMode;
			}
			set
			{
				_gameMode = value;
				_gameRegim = _gameMode.Substring(_gameMode.Length - 1);
			}
		}

		public bool IsCanConnect
		{
			get
			{
				return notConnectCondition == NotConnectCondition.None;
			}
		}

		public OnlineState GetOnlineStatus()
		{
			switch (Convert.ToInt32(_gameRegim))
			{
			case 6:
				return OnlineState.inFriends;
			case 7:
				return OnlineState.inClans;
			default:
				return OnlineState.playing;
			}
		}

		public string GetGameModeName()
		{
			IDictionary<string, string> gameModesLocalizeKey = ConnectSceneNGUIController.gameModesLocalizeKey;
			if (!gameModesLocalizeKey.ContainsKey(_gameRegim))
			{
				return string.Empty;
			}
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(mapIndex));
			if (infoScene != null && infoScene.IsAvaliableForMode(TypeModeGame.Dater))
			{
				return LocalizationStore.Get("Key_1567");
			}
			return LocalizationStore.Get(gameModesLocalizeKey[_gameRegim]);
		}

		public string GetMapName()
		{
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(mapIndex));
			if (infoScene == null)
			{
				return string.Empty;
			}
			return infoScene.TranslateName;
		}

		public string GetNotConnectConditionString()
		{
			if (IsCanConnect)
			{
				return string.Empty;
			}
			string result = string.Empty;
			switch (notConnectCondition)
			{
			case NotConnectCondition.clientVersion:
				result = LocalizationStore.Get("Key_1418");
				break;
			case NotConnectCondition.level:
				result = LocalizationStore.Get("Key_1420");
				break;
			case NotConnectCondition.map:
				result = LocalizationStore.Get("Key_1419");
				break;
			case NotConnectCondition.platform:
				result = LocalizationStore.Get("Key_1414");
				break;
			}
			return result;
		}

		public string GetNotConnectConditionShortString()
		{
			if (IsCanConnect)
			{
				return string.Empty;
			}
			string result = string.Empty;
			switch (notConnectCondition)
			{
			case NotConnectCondition.clientVersion:
				result = LocalizationStore.Get("Key_1573");
				break;
			case NotConnectCondition.level:
				result = LocalizationStore.Get("Key_1574");
				break;
			case NotConnectCondition.map:
				result = LocalizationStore.Get("Key_1575");
				break;
			case NotConnectCondition.platform:
				result = LocalizationStore.Get("Key_1576");
				break;
			case NotConnectCondition.InChat:
				result = LocalizationStore.Get("Key_1577");
				break;
			}
			return result;
		}
	}

	public enum TypeTrafficForwardingLog
	{
		newView,
		view,
		click
	}

	public delegate void OnChangeClanName(string newName);

	private const string FriendsKey = "FriendsKey";

	private const string ToUsKey = "ToUsKey";

	private const string PlayerInfoKey = "PlayerInfoKey";

	private const string FriendsInfoKey = "FriendsInfoKey";

	private const string ClanFriendsInfoKey = "ClanFriendsInfoKey";

	private const string ClanInvitesKey = "ClanInvitesKey";

	private const string PixelbookSettingsKey = "PixelbookSettingsKey";

	public const string LobbyNewsKey = "LobbyNewsKey";

	public const string LobbyIsAnyNewsKey = "LobbyIsAnyNewsKey";

	public const string PixelFilterWordsKey = "PixelFilterWordsKey";

	public const string PixelFilterSymbolsKey = "PixelFilterSymbolsKey";

	public const float TimeUpdateFriendAndClanData = 20f;

	public static bool isDebugLogWWW = true;

	public int Banned = -1;

	public static float onlineDelta = 60f;

	public static Dictionary<string, Dictionary<string, string>> mapPopularityDictionary = new Dictionary<string, Dictionary<string, string>>();

	public static bool readyToOperate = false;

	public static FriendsController sharedController = null;

	private string currentCompetitionKey = "currentCompetitionKey";

	private int _currentCompetition = -1;

	private float _expirationTimeCompetition = -1f;

	private static bool _expirationTimeCompetitionInit = false;

	private static string expirationTimeCompetitionKey = "expirationTimeCompetitionKey";

	private static bool _advertEnabled = false;

	private bool friendsReceivedOnce;

	[SerializeField]
	[ReadOnly]
	private string _clanId;

	public string clanLeaderID;

	public string clanLogo;

	public string clanName;

	public int NumberOfFriendsRequests;

	public int NumberOffFullInfoRequests;

	public int NumberOfBestPlayersRequests;

	public int NumberOfClanInfoRequests;

	public int NumberOfCreateClanRequests;

	private float lastTouchTm;

	public bool idle;

	private List<int> ids = new List<int>();

	public List<string> friendsDeletedLocal = new List<string>();

	public string JoinClanSent;

	private string AccountCreated = "AccountCreated";

	private string _id;

	internal List<string> friends = new List<string>();

	internal readonly List<Dictionary<string, string>> clanMembers = new List<Dictionary<string, string>>();

	internal List<string> invitesFromUs = new List<string>();

	internal List<string> invitesToUs = new List<string>();

	internal List<Dictionary<string, string>> ClanInvites = new List<Dictionary<string, string>>();

	internal readonly List<string> ClanSentInvites = new List<string>();

	internal readonly List<string> clanSentInvitesLocal = new List<string>();

	internal readonly List<string> clanCancelledInvitesLocal = new List<string>();

	internal readonly List<string> clanDeletedLocal = new List<string>();

	internal readonly Dictionary<string, Dictionary<string, object>> playersInfo = new Dictionary<string, Dictionary<string, object>>();

	internal readonly Dictionary<string, Dictionary<string, object>> friendsInfo = new Dictionary<string, Dictionary<string, object>>();

	internal readonly Dictionary<string, Dictionary<string, object>> clanFriendsInfo = new Dictionary<string, Dictionary<string, object>>();

	internal readonly Dictionary<string, Dictionary<string, object>> profileInfo = new Dictionary<string, Dictionary<string, object>>();

	internal readonly Dictionary<string, Dictionary<string, string>> onlineInfo = new Dictionary<string, Dictionary<string, string>>();

	internal readonly List<string> notShowAddIds = new List<string>();

	internal readonly Dictionary<string, Dictionary<string, object>> facebookFriendsInfo = new Dictionary<string, Dictionary<string, object>>();

	public string alphaIvory;

	private static HMAC _hmac;

	public string nick;

	public string skin;

	public int rank;

	public int coopScore;

	public int survivalScore;

	internal SaltedInt wins = new SaltedInt(641227346);

	public Dictionary<string, object> ourInfo;

	public string id_fb;

	private float timerUpdatePixelbookSetting = 900f;

	private static long localServerTime;

	private static float tickForServerTime = 0f;

	private static bool isUpdateServerTimeAfterRun;

	private bool isGetServerTimeFromMainUrl = true;

	public static bool isInitPixelbookSettingsFromServer = false;

	private string FacebookIDKey = "FacebookIDKey";

	public OnChangeClanName onChangeClanName;

	private string _prevClanName;

	public bool dataSent;

	private bool infoLoaded;

	public static float timeOutSendUpdatePlayerFromConnectScene = ((!Defs.IsDeveloperBuild) ? 360f : 36f);

	public string tempClanID;

	public string tempClanLogo;

	public string tempClanName;

	public string tempClanCreatorID;

	private bool _shouldStopOnline;

	private bool _shouldStopOnlineWithClanInfo;

	private bool _shouldStopRefrClanOnline;

	public Action GetFacebookFriendsCallback;

	private string _inputToken;

	private KeyValuePair<string, int>? _winCountTimestamp;

	private bool ReceivedLastOnline;

	private bool getCohortInfo;

	private float timeSendUpdatePlayer;

	public float timerUpdateFriend = 20f;

	public static Action OnShowBoxProcessFriendsData;

	public static Action OnHideBoxProcessFriendsData;

	private bool _shouldStopRefreshingInfo;

	private float deltaTimeInGame;

	private float sendingTime;

	private bool firstUpdateAfterApplicationPause;

	public Dictionary<string, PossiblleOrigin> getPossibleFriendsResult = new Dictionary<string, PossiblleOrigin>();

	private bool isUpdateInfoAboutAllFriends;

	public static Action UpdateFriendsInfoAction;

	public Dictionary<string, string> clicksJoinByFriends = new Dictionary<string, string>();

	private static FriendProfileController _friendProfileController;

	private static DateTime timeSendTrafficForwarding = new DateTime(2000, 1, 1, 12, 0, 0);

	private static bool _isConfigNameAdvertInit;

	private static string _configNameABTestAdvert = "none";

	public static bool isReadABTestAdvertConfig = false;

	public int currentCompetition
	{
		get
		{
			if (_currentCompetition < 0)
			{
				_currentCompetition = Storager.getInt(currentCompetitionKey, false);
			}
			return _currentCompetition;
		}
		internal set
		{
			_currentCompetition = value;
			Storager.setInt(currentCompetitionKey, _currentCompetition, false);
		}
	}

	public float expirationTimeCompetition
	{
		get
		{
			if (!_expirationTimeCompetitionInit)
			{
				int @int = PlayerPrefs.GetInt(expirationTimeCompetitionKey, 0);
			}
			_expirationTimeCompetitionInit = true;
			return _expirationTimeCompetition;
		}
		private set
		{
			_expirationTimeCompetition = value + Time.realtimeSinceStartup;
			PlayerPrefs.SetInt(expirationTimeCompetitionKey, Mathf.RoundToInt(value));
			_expirationTimeCompetitionInit = true;
		}
	}

	public bool ClanLimitReached
	{
		get
		{
			FriendsController friendsController = sharedController;
			return friendsController.clanMembers.Count + friendsController.ClanSentInvites.Count + friendsController.clanSentInvitesLocal.Count >= friendsController.ClanLimit;
		}
	}

	public int ClanLimit
	{
		get
		{
			return Defs.maxMemberClanCount;
		}
	}

	internal static bool PolygonEnabled
	{
		get
		{
			return Defs.IsDeveloperBuild;
		}
	}

	internal static bool AdvertEnabled
	{
		get
		{
			return _advertEnabled;
		}
		set
		{
			_advertEnabled = value;
		}
	}

	public static bool ClanDataSettted { get; private set; }

	public static int CurrentPlatform
	{
		get
		{
			return ProtocolListGetter.CurrentPlatform;
		}
	}

	public string ClanID
	{
		get
		{
			return _clanId;
		}
		set
		{
			_clanId = value;
			if (FriendsController.OnClanIdSetted != null)
			{
				FriendsController.OnClanIdSetted(_clanId);
			}
		}
	}

	public static long ServerTime
	{
		get
		{
			if (isUpdateServerTimeAfterRun)
			{
				return localServerTime;
			}
			return -1L;
		}
	}

	public static string actionAddress
	{
		get
		{
			return URLs.Friends;
		}
	}

	public string id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public KeyValuePair<string, int>? WinCountTimestamp
	{
		get
		{
			return _winCountTimestamp;
		}
	}

	public Dictionary<string, object> getInfoPlayerResult { get; private set; }

	public List<string> findPlayersByParamResult { get; private set; }

	public static bool HasFriends
	{
		get
		{
			string @string = PlayerPrefs.GetString("FriendsKey", "[]");
			return !string.IsNullOrEmpty(@string) && @string != "[]";
		}
	}

	public bool ProfileInterfaceActive
	{
		get
		{
			if (_friendProfileController == null)
			{
				return false;
			}
			return _friendProfileController.FriendProfileGo.Map((GameObject g) => g.activeInHierarchy);
		}
	}

	public static string configNameABTestAdvert
	{
		get
		{
			if (!_isConfigNameAdvertInit)
			{
				_configNameABTestAdvert = PlayerPrefs.GetString("CNAdvert", "none");
				_isConfigNameAdvertInit = true;
			}
			return _configNameABTestAdvert;
		}
		set
		{
			_isConfigNameAdvertInit = true;
			_configNameABTestAdvert = value;
			PlayerPrefs.SetString("CNAdvert", _configNameABTestAdvert);
		}
	}

	public static event Action FriendsUpdated;

	public static event Action ClanUpdated;

	public static event Action FullInfoUpdated;

	public static event Action ServerTimeUpdated;

	public event Action FailedSendNewClan;

	public event Action<int> ReturnNewIDClan;

	public static event Action<string> OnClanIdSetted;

	public static event Action<int, int> NewFacebookLimitsAvailable;

	public static event Action<int, int, int, int> NewTwitterLimitsAvailable;

	public static event Action<int, int, int, int> NewCheaterDetectParametersAvailable;

	public static event Action OurInfoUpdated;

	static FriendsController()
	{
		FriendsController.FriendsUpdated = null;
		FriendsController.FullInfoUpdated = null;
		FriendsController.ServerTimeUpdated = null;
	}

	public void FastGetPixelbookSettings()
	{
		timerUpdatePixelbookSetting = -1f;
	}

	private IEnumerator GetPixelbookSettingsLoop(Task futureToWait)
	{
		yield return new WaitUntil(() => futureToWait.IsCompleted);
		timerUpdatePixelbookSetting = Defs.timeUpdatePixelbookInfo;
		while (true)
		{
			yield return StartCoroutine(GetPixelbookSettings());
			while (timerUpdatePixelbookSetting > 0f)
			{
				timerUpdatePixelbookSetting -= Time.unscaledDeltaTime;
				yield return null;
			}
			timerUpdatePixelbookSetting = Defs.timeUpdatePixelbookInfo;
		}
	}

	private IEnumerator GetNewsLoop(Task futureToWait)
	{
		yield return new WaitUntil(() => futureToWait.IsCompleted);
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage <= TrainingController.NewTrainingCompletedStage.ShopCompleted)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(GetLobbyNews(false));
			yield return new WaitForSeconds(Defs.timeUpdateNews);
		}
	}

	private IEnumerator GetFiltersSettings()
	{
		WWW response = Tools.CreateWwwIfNotConnected(URLs.FilterBadWord);
		if (response == null)
		{
			yield break;
		}
		yield return response;
		if (!string.IsNullOrEmpty(response.error))
		{
			Debug.LogWarning("FilterBadWord response error: " + response.error);
			yield break;
		}
		string responseText = URLs.Sanitize(response);
		if (string.IsNullOrEmpty(responseText))
		{
			Debug.LogWarning("FilterBadWord response is empty");
			yield break;
		}
		Dictionary<string, object> filterDict = Json.Deserialize(responseText) as Dictionary<string, object>;
		string wordsList = Json.Serialize(filterDict["Words"]);
		string symbolsList = Json.Serialize(filterDict["Symbols"]);
		PlayerPrefs.SetString("PixelFilterWordsKey", wordsList);
		PlayerPrefs.SetString("PixelFilterSymbolsKey", symbolsList);
		PlayerPrefs.Save();
		FilterBadWorld.InitBadLists();
	}

	private IEnumerator GetBuffSettings(Task futureToWait)
	{
		yield return new WaitUntil(() => futureToWait.IsCompleted);
		string url = ((!ABTestController.useBuffSystem) ? URLs.BuffSettings1031 : URLs.BuffSettings1050);
		string cachedResponse = PersistentCacheManager.Instance.GetValue(url);
		string responseText;
		if (!string.IsNullOrEmpty(cachedResponse))
		{
			responseText = cachedResponse;
		}
		else
		{
			WWW response;
			while (true)
			{
				response = Tools.CreateWwwIfNotConnected(url);
				if (response == null)
				{
					yield return new WaitForSeconds(20f);
					continue;
				}
				yield return response;
				if (!string.IsNullOrEmpty(response.error))
				{
					Debug.LogWarning("GetBuffSettings response error: " + response.error);
					yield return new WaitForSeconds(20f);
					continue;
				}
				responseText = URLs.Sanitize(response);
				if (!string.IsNullOrEmpty(responseText))
				{
					break;
				}
				Debug.LogWarning("GetBuffSettings response is empty");
				yield return new WaitForSeconds(20f);
			}
			PersistentCacheManager.Instance.SetValue(response.url, responseText);
		}
		Storager.setString("BuffsParam", responseText, false);
		if (ABTestController.useBuffSystem)
		{
			BuffSystem.instance.TryLoadConfig();
		}
	}

	private IEnumerator GetRatingSystemConfig()
	{
		while (true)
		{
			WWWForm form = new WWWForm();
			WWW download = Tools.CreateWwwIfNotConnected(URLs.RatingSystemConfigURL);
			if (download == null)
			{
				yield return StartCoroutine(MyWaitForSeconds(60f));
				continue;
			}
			yield return download;
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild || Application.isEditor)
				{
					Debug.LogWarning("GetRatingSystemConfig error: " + download.error);
				}
				yield return StartCoroutine(MyWaitForSeconds(600f));
				continue;
			}
			string responseText = URLs.Sanitize(download);
			if (!string.IsNullOrEmpty(responseText))
			{
				Storager.setString("rSCKeyV2", responseText, false);
				RatingSystem.instance.ParseConfig();
			}
			yield return StartCoroutine(MyWaitForSeconds(1800f));
		}
	}

	private IEnumerator GetLobbyNews(bool fromPause)
	{
		string url = URLs.LobbyNews;
		string cachedResponse = PersistentCacheManager.Instance.GetValue(url);
		string responseText;
		if (!string.IsNullOrEmpty(cachedResponse))
		{
			responseText = cachedResponse;
		}
		else
		{
			WWW response = Tools.CreateWwwIfNotConnected(url);
			if (response == null)
			{
				yield break;
			}
			yield return response;
			if (!string.IsNullOrEmpty(response.error))
			{
				Debug.LogWarning("GetLobbyNews response error: " + response.error);
				yield break;
			}
			responseText = URLs.Sanitize(response);
			PersistentCacheManager.Instance.SetValue(response.url, responseText);
		}
		if (string.IsNullOrEmpty(responseText))
		{
			Debug.LogWarning("GetLobbyNews response is empty");
			yield break;
		}
		string oldNews = PlayerPrefs.GetString("LobbyNewsKey", "[]");
		bool isAnyNews = false;
		List<object> oldNewsAsList = Json.Deserialize(oldNews) as List<object>;
		List<Dictionary<string, object>> oldNewsAll = ((oldNewsAsList == null) ? new List<Dictionary<string, object>>() : oldNewsAsList.OfType<Dictionary<string, object>>().ToList());
		List<object> responseTextAsList = Json.Deserialize(responseText) as List<object>;
		List<Dictionary<string, object>> newsAll = ((responseTextAsList == null) ? new List<Dictionary<string, object>>() : responseTextAsList.OfType<Dictionary<string, object>>().ToList());
		if (newsAll.Count == 0)
		{
			isAnyNews = false;
		}
		else
		{
			for (int i = 0; i < newsAll.Count; i++)
			{
				newsAll[i]["readed"] = 0;
				bool isOld = false;
				for (int o = 0; o < oldNewsAll.Count; o++)
				{
					if (Convert.ToInt32(oldNewsAll[o]["date"]) == Convert.ToInt32(newsAll[i]["date"]))
					{
						isOld = true;
						if (oldNewsAll[o].ContainsKey("readed"))
						{
							newsAll[i]["readed"] = oldNewsAll[o]["readed"];
						}
						break;
					}
				}
				try
				{
					if (!isOld)
					{
						AnalyticsFacade.SendCustomEvent("News", new Dictionary<string, object>
						{
							{ "CTR", "Show" },
							{ "Conversion Total", "Show" }
						});
					}
				}
				catch (Exception e)
				{
					Debug.LogError("Exception in log News (CTR = Show, Conversion Total = Show): " + e);
				}
				if (Convert.ToInt32(newsAll[i]["readed"]) == 0)
				{
					isAnyNews = true;
				}
			}
		}
		PlayerPrefs.SetString("LobbyNewsKey", Json.Serialize(newsAll));
		PlayerPrefs.SetInt("LobbyIsAnyNewsKey", isAnyNews ? 1 : 0);
		PlayerPrefs.Save();
		if (isAnyNews && MainMenuController.sharedController != null)
		{
			MainMenuController.sharedController.newsIndicator.SetActive(true);
		}
	}

	private IEnumerator GetTimeFromServerLoop()
	{
		isUpdateServerTimeAfterRun = false;
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage < TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
		{
			yield return null;
		}
		while (string.IsNullOrEmpty(id))
		{
			yield return null;
		}
		while (!isUpdateServerTimeAfterRun)
		{
			yield return StartCoroutine(GetTimeFromServer());
			float timerUpdate = Defs.timeUpdateServerTime;
			while (!isUpdateServerTimeAfterRun && timerUpdate > 0f)
			{
				timerUpdate -= Time.unscaledDeltaTime;
				yield return null;
			}
		}
		if (FriendsController.ServerTimeUpdated != null)
		{
			FriendsController.ServerTimeUpdated();
		}
	}

	private IEnumerator GetTimeFromServer()
	{
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("action", "get_time");
		wwwForm.AddField("app_version", string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion));
		wwwForm.AddField("uniq_id", sharedController.id);
		wwwForm.AddField("auth", Hash("get_time"));
		WWW download = Tools.CreateWww((!isGetServerTimeFromMainUrl) ? URLs.TimeOnSecure : URLs.Friends, wwwForm, string.Empty);
		if (download != null)
		{
			yield return download;
			string response = URLs.Sanitize(download);
			long currentServerTime;
			if (!string.IsNullOrEmpty(download.error))
			{
				Debug.LogWarning("get_time error:    " + download.error);
				isGetServerTimeFromMainUrl = !isGetServerTimeFromMainUrl;
			}
			else if (long.TryParse(response, out currentServerTime))
			{
				localServerTime = currentServerTime;
				tickForServerTime = 0f;
				isUpdateServerTimeAfterRun = true;
			}
			else
			{
				Debug.LogError("Could not parse response: " + response);
				isGetServerTimeFromMainUrl = !isGetServerTimeFromMainUrl;
			}
		}
	}

	private IEnumerator GetPixelbookSettings()
	{
		string url = URLs.PixelbookSettings;
		string cachedResponse = PersistentCacheManager.Instance.GetValue(url);
		string responseText;
		if (!string.IsNullOrEmpty(cachedResponse))
		{
			responseText = cachedResponse;
		}
		else
		{
			WWW response = Tools.CreateWwwIfNotConnected(url);
			if (response == null)
			{
				yield break;
			}
			yield return response;
			if (!string.IsNullOrEmpty(response.error))
			{
				Debug.LogWarning("PixelbookSettings response error: " + response.error);
				yield break;
			}
			responseText = URLs.Sanitize(response);
			if (string.IsNullOrEmpty(responseText))
			{
				Debug.LogWarning("PixelbookSettings response is empty");
				yield break;
			}
			PersistentCacheManager.Instance.SetValue(url, responseText);
		}
		PlayerPrefs.SetString("PixelbookSettingsKey", responseText);
		PlayerPrefs.Save();
		UpdatePixelbookSettingsFromPrefs();
		isInitPixelbookSettingsFromServer = true;
	}

	public static void UpdatePixelbookSettingsFromPrefs()
	{
		string @string = PlayerPrefs.GetString("PixelbookSettingsKey", "{}");
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary == null || !dictionary.ContainsKey("MaxFriendCount"))
		{
			return;
		}
		if (dictionary.ContainsKey("FriendsUrl"))
		{
			URLs.Friends = Convert.ToString(dictionary["FriendsUrl"]);
		}
		if (dictionary.ContainsKey("MaxFriendCount"))
		{
			Defs.maxCountFriend = Convert.ToInt32(dictionary["MaxFriendCount"]);
		}
		if (dictionary.ContainsKey("MaxMemberClanCount"))
		{
			Defs.maxMemberClanCount = Convert.ToInt32(dictionary["MaxMemberClanCount"]);
		}
		if (dictionary.ContainsKey("TimeUpdateFriendInfo"))
		{
			Defs.timeUpdateFriendInfo = Convert.ToInt32(dictionary["TimeUpdateFriendInfo"]);
		}
		if (dictionary.ContainsKey("TimeUpdateOnlineInGame"))
		{
			Defs.timeUpdateOnlineInGame = Convert.ToInt32(dictionary["TimeUpdateOnlineInGame"]);
		}
		if (dictionary.ContainsKey("TimeUpdateInfoInProfile"))
		{
			Defs.timeUpdateInfoInProfile = Convert.ToInt32(dictionary["TimeUpdateInfoInProfile"]);
		}
		if (dictionary.ContainsKey("TimeUpdateLeaderboardIfNullResponce"))
		{
			Defs.timeUpdateLeaderboardIfNullResponce = Convert.ToInt32(dictionary["TimeUpdateLeaderboardIfNullResponce"]);
		}
		if (dictionary.ContainsKey("TimeBlockRefreshFriendDate"))
		{
			Defs.timeBlockRefreshFriendDate = Convert.ToInt32(dictionary["TimeBlockRefreshFriendDate"]);
		}
		if (dictionary.ContainsKey("TimeWaitLoadPossibleFriends"))
		{
			Defs.timeWaitLoadPossibleFriends = Convert.ToInt32(dictionary["TimeWaitLoadPossibleFriends"]);
		}
		if (dictionary.ContainsKey("PauseUpdateLeaderboard"))
		{
			Defs.pauseUpdateLeaderboard = Convert.ToInt32(dictionary["PauseUpdateLeaderboard"]);
		}
		if (dictionary.ContainsKey("TimeUpdatePixelbookInfo"))
		{
			Defs.timeUpdatePixelbookInfo = Convert.ToInt32(dictionary["TimeUpdatePixelbookInfo"]);
		}
		if (dictionary.ContainsKey("HistoryPrivateMessageLength"))
		{
			Defs.historyPrivateMessageLength = Convert.ToInt32(dictionary["HistoryPrivateMessageLength"]);
		}
		if (dictionary.ContainsKey("OutgoingInviteTimeoutMinutes"))
		{
			BattleInviteListener.Instance.OutgoingInviteTimeout = TimeSpan.FromMinutes(Convert.ToSingle(dictionary["OutgoingInviteTimeoutMinutes"]));
		}
		if (dictionary.ContainsKey("TimerIntervalDelaysFirstsEggs"))
		{
			List<object> list = dictionary["TimerIntervalDelaysFirstsEggs"] as List<object>;
			Nest.timerIntervalDelays.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				Nest.timerIntervalDelays.Add(Convert.ToInt64(list[i]));
			}
		}
		if (dictionary.ContainsKey("TimeUpdateStartCheckIfNullResponce"))
		{
			Defs.timeUpdateStartCheckIfNullResponce = Convert.ToInt32(dictionary["TimeUpdateStartCheckIfNullResponce"]);
		}
		if (dictionary.ContainsKey("TimeoutSendUpdatePlayerFromConnectScene"))
		{
			timeOutSendUpdatePlayerFromConnectScene = Convert.ToInt32(dictionary["TimeoutSendUpdatePlayerFromConnectScene"]);
		}
		if (dictionary.ContainsKey("EnableLogForIDs") && sharedController != null && !string.IsNullOrEmpty(sharedController.id))
		{
			List<object> list2 = dictionary["EnableLogForIDs"] as List<object>;
			foreach (object item in list2)
			{
				if (sharedController.id == item.ToString())
				{
					LogsManager.EnableLogsFromServer();
					break;
				}
			}
		}
		if (dictionary.ContainsKey("FacebookLimits"))
		{
			try
			{
				object obj = dictionary["FacebookLimits"];
				Dictionary<string, object> dictionary2 = obj as Dictionary<string, object>;
				int arg = (int)(long)dictionary2["GreenLimit"];
				int arg2 = (int)(long)dictionary2["RedLimit"];
				Action<int, int> newFacebookLimitsAvailable = FriendsController.NewFacebookLimitsAvailable;
				if (newFacebookLimitsAvailable != null)
				{
					newFacebookLimitsAvailable(arg, arg2);
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		if (dictionary.ContainsKey("TwitterLimits"))
		{
			try
			{
				object obj2 = dictionary["TwitterLimits"];
				Dictionary<string, object> dictionary3 = obj2 as Dictionary<string, object>;
				int arg3 = (int)(long)dictionary3["GreenLimit"];
				int arg4 = (int)(long)dictionary3["RedLimit"];
				int arg5 = (int)(long)dictionary3["MultyWinLimit"];
				int arg6 = (int)(long)dictionary3["ArenaLimit"];
				Action<int, int, int, int> newTwitterLimitsAvailable = FriendsController.NewTwitterLimitsAvailable;
				if (newTwitterLimitsAvailable != null)
				{
					newTwitterLimitsAvailable(arg3, arg4, arg5, arg6);
				}
			}
			catch (Exception exception2)
			{
				Debug.LogException(exception2);
			}
		}
		if (dictionary.ContainsKey("CheaterDetectParameters"))
		{
			try
			{
				object obj3 = dictionary["CheaterDetectParameters"];
				Dictionary<string, object> dictionary4 = obj3 as Dictionary<string, object>;
				Dictionary<string, object> dictionary5 = dictionary4["Paying"] as Dictionary<string, object>;
				int arg7 = (int)(long)dictionary5["Coins"];
				int arg8 = (int)(long)dictionary5["GemsCurrency"];
				Dictionary<string, object> dictionary6 = dictionary4["NonPaying"] as Dictionary<string, object>;
				int arg9 = (int)(long)dictionary6["Coins"];
				int arg10 = (int)(long)dictionary6["GemsCurrency"];
				Action<int, int, int, int> newCheaterDetectParametersAvailable = FriendsController.NewCheaterDetectParametersAvailable;
				if (newCheaterDetectParametersAvailable != null)
				{
					newCheaterDetectParametersAvailable(arg7, arg8, arg9, arg10);
				}
			}
			catch (Exception exception3)
			{
				Debug.LogException(exception3);
			}
		}
		if (dictionary.ContainsKey("UseSqlLobby1031"))
		{
			Defs.useSqlLobby = Convert.ToInt32(dictionary["UseSqlLobby1031"]) == 1;
		}
	}

	private static void FillListDictionary(string key, List<Dictionary<string, string>> list)
	{
		string @string = PlayerPrefs.GetString(key, "[]");
		List<object> list2 = Json.Deserialize(@string) as List<object>;
		if (list2 == null || list2.Count <= 0)
		{
			return;
		}
		foreach (Dictionary<string, object> item in list2.OfType<Dictionary<string, object>>())
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> item2 in item)
			{
				string text = item2.Value as string;
				if (text != null)
				{
					dictionary.Add(item2.Key, text);
				}
			}
			list.Add(dictionary);
		}
	}

	private static List<string> FillList(string key)
	{
		List<string> list = new List<string>();
		string @string = PlayerPrefs.GetString(key, "[]");
		List<object> list2 = Json.Deserialize(@string) as List<object>;
		if (list2 != null && list2.Count > 0)
		{
			foreach (string item in list2.OfType<string>())
			{
				list.Add(item);
			}
			return list;
		}
		return list;
	}

	private static void FillDictionary(string key, Dictionary<string, Dictionary<string, object>> dictionary)
	{
		string text = string.Empty;
		using (new StopwatchLogger("Storager extracting " + key))
		{
			text = PlayerPrefs.GetString(key, "{}");
		}
		Debug.Log(key + " (length): " + text.Length);
		Dictionary<string, object> dictionary2 = null;
		using (new StopwatchLogger("Json decoding " + key))
		{
			dictionary2 = Json.Deserialize(text) as Dictionary<string, object>;
		}
		if (dictionary2 == null || dictionary2.Count <= 0)
		{
			return;
		}
		Debug.Log(key + " (count): " + dictionary2.Count);
		using (new StopwatchLogger("Dictionary copying " + key))
		{
			foreach (KeyValuePair<string, object> item in dictionary2)
			{
				Dictionary<string, object> dictionary3 = item.Value as Dictionary<string, object>;
				if (dictionary3 != null)
				{
					dictionary.Add(item.Key, dictionary3);
				}
			}
		}
	}

	private void Awake()
	{
		if (!Storager.hasKey(FacebookIDKey))
		{
			Storager.setString(FacebookIDKey, string.Empty, false);
		}
		id_fb = Storager.getString(FacebookIDKey, false);
		sharedController = this;
	}

	public IEnumerable<float> InitController()
	{
		string secret = alphaIvory ?? string.Empty;
		if (string.IsNullOrEmpty(secret))
		{
			Debug.LogError("Secret is empty!");
		}
		_hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secret), true);
		StartCoroutine("GetABTestAdvertConfig");
		StartCoroutine("GetCurrentcompetition");
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage < TrainingController.NewTrainingCompletedStage.ShopCompleted && (!Storager.hasKey("abTestBalansConfig2Key") || string.IsNullOrEmpty(Storager.getString("abTestBalansConfig2Key", false))))
		{
			Storager.setString("abTestBalansConfig2Key", string.Empty, false);
			StartCoroutine(GetABTestBalansConfigName());
		}
		else
		{
			getCohortInfo = true;
			if (Defs.isABTestBalansCohortActual)
			{
				StartCoroutine(GetABTestBalansCohortNameActual());
			}
		}
		Task futureToWait = PersistentCacheManager.Instance.FirstResponse;
		StopCoroutine("GetBanList");
		StartCoroutine("GetBanList");
		StartCoroutine("UpdatePopularityMapsLoop");
		StartCoroutine(GetTimeFromServerLoop());
		StartCoroutine(SendGameTimeLoop());
		UpdatePixelbookSettingsFromPrefs();
		StartCoroutine(GetPixelbookSettingsLoop(futureToWait));
		StartCoroutine(GetNewsLoop(futureToWait));
		ProfileController.LoadStatisticFromKeychain();
		TrafficForwardingScript trafficForwardingScript = GetComponent<TrafficForwardingScript>() ?? base.gameObject.AddComponent<TrafficForwardingScript>();
		StartCoroutine(trafficForwardingScript.GetTrafficForwardingConfigLoopCoroutine());
		StartCoroutine(GetFiltersSettings());
		StartCoroutine(GetBuffSettings(futureToWait));
		StartCoroutine(GetRatingSystemConfig());
		if (FacebookController.FacebookSupported)
		{
			FacebookController.ReceivedSelfID += HandleReceivedSelfID;
		}
		lastTouchTm = Time.realtimeSinceStartup + 15f;
		friends = FillList("FriendsKey");
		StartSendReview();
		yield return 0f;
		invitesToUs = FillList("ToUsKey");
		yield return 0f;
		FillDictionary("PlayerInfoKey", playersInfo);
		FillDictionary("FriendsInfoKey", friendsInfo);
		FillDictionary("ClanFriendsInfoKey", clanFriendsInfo);
		yield return 0f;
		FillListDictionary("ClanInvitesKey", ClanInvites);
		yield return 0f;
		FillClickJoinFriendsListByCachedValue();
		yield return 0f;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!Storager.hasKey(AccountCreated))
		{
			Storager.setString(AccountCreated, string.Empty, false);
		}
		id = Storager.getString(AccountCreated, false);
		if (string.IsNullOrEmpty(id))
		{
			Debug.Log("Account id: null or empty    Calling CreatePlayer()...");
			StartCoroutine(CreatePlayer());
		}
		else
		{
			Debug.LogFormat("Account id: {0}    Calling CheckOurIdExists()...", id);
			StartCoroutine(CheckOurIDExists());
		}
		SyncClickJoinFriendsListWithListFriends();
	}

	private void HandleReceivedSelfID(string idfb)
	{
		if (idfb != null && (string.IsNullOrEmpty(id_fb) || !idfb.Equals(id_fb)))
		{
			id_fb = idfb;
			if (!Storager.hasKey(FacebookIDKey))
			{
				Storager.setString(FacebookIDKey, string.Empty, false);
			}
			Storager.setString(FacebookIDKey, id_fb, false);
			SendOurData(false);
		}
	}

	public void UnbanUs(Action onSuccess)
	{
	}

	public void ChangeClanLogo()
	{
		if (readyToOperate)
		{
			StartCoroutine(_ChangeClanLogo());
		}
	}

	public void GetOurWins()
	{
		if (readyToOperate)
		{
			StartCoroutine(_GetOurWins());
		}
	}

	public void SendRoundWon()
	{
		if (readyToOperate)
		{
			int num = -1;
			if (PhotonNetwork.room != null)
			{
				num = (int)ConnectSceneNGUIController.regim;
			}
			if (num != -1)
			{
				StartCoroutine(_SendRoundWon(num));
			}
		}
	}

	public static string Hash(string action, string token = null)
	{
		if (action == null)
		{
			Debug.LogWarning("Hash: action is null");
			return string.Empty;
		}
		string text = token ?? ((!(sharedController != null)) ? null : sharedController.id);
		if (text == null)
		{
			Debug.LogWarning("Hash: Token is null");
			return string.Empty;
		}
		string text2 = ((!action.Equals("get_player_online")) ? (ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion) : "*:*.*.*");
		string s = text2 + text + action;
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		byte[] array = _hmac.ComputeHash(bytes);
		string text3 = BitConverter.ToString(array);
		return text3.Replace("-", string.Empty).ToLower();
	}

	public static string HashForPush(byte[] responceData)
	{
		if (responceData == null)
		{
			Debug.LogWarning("HashForPush: responceData is null");
			return string.Empty;
		}
		if (_hmac == null)
		{
			throw new InvalidOperationException("Hmac is not initialized yet.");
		}
		byte[] array = _hmac.ComputeHash(responceData);
		string text = BitConverter.ToString(array);
		return text.Replace("-", string.Empty).ToLower();
	}

	public bool IsShowAdd(string _pixelBookID)
	{
		bool flag = true;
		if (friends.Count >= Defs.maxCountFriend || _pixelBookID.Equals("-1") || _pixelBookID.Equals(sharedController.id))
		{
			return false;
		}
		if (sharedController.friends.Contains(_pixelBookID))
		{
			return false;
		}
		return !sharedController.notShowAddIds.Contains(_pixelBookID);
	}

	private IEnumerator _GetOurWins()
	{
		while (string.IsNullOrEmpty(sharedController.id) || !TrainingController.TrainingCompleted)
		{
			yield return null;
		}
		string appVersionString = ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion;
		WWWForm form = new WWWForm();
		form.AddField("action", "get_info_by_id");
		form.AddField("app_version", appVersionString);
		form.AddField("id", id);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("get_info_by_id"));
		string response;
		while (true)
		{
			WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
			if (download == null)
			{
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			yield return download;
			response = URLs.Sanitize(download);
			if (!string.IsNullOrEmpty(download.error) || !string.IsNullOrEmpty(response))
			{
			}
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogWarning("_GetOurWins error: " + download.error);
				}
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			if (string.IsNullOrEmpty(response) || !response.Equals("fail"))
			{
				break;
			}
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning("_GetOurWins fail.");
			}
			yield return StartCoroutine(MyWaitForSeconds(10f));
		}
		Dictionary<string, object> __newInfo = ParseInfo(response);
		if (__newInfo == null)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning(" _GetOurWins newInfo = null");
			}
			yield break;
		}
		ourInfo = __newInfo;
		SaveProfileData();
		if (FriendsController.OurInfoUpdated != null)
		{
			FriendsController.OurInfoUpdated();
		}
	}

	private void SaveProfileData()
	{
		if (ourInfo != null && ourInfo.ContainsKey("wincount"))
		{
			int num = 0;
			Dictionary<string, object> dict = ourInfo["wincount"] as Dictionary<string, object>;
			num = 0;
			dict.TryGetValue<int>("0", out num);
			Storager.setInt(Defs.RatingDeathmatch, num, false);
			num = 0;
			dict.TryGetValue<int>("2", out num);
			Storager.setInt(Defs.RatingTeamBattle, num, false);
			num = 0;
			dict.TryGetValue<int>("3", out num);
			Storager.setInt(Defs.RatingHunger, num, false);
			num = 0;
			dict.TryGetValue<int>("4", out num);
			Storager.setInt(Defs.RatingCapturePoint, num, false);
		}
	}

	private IEnumerator _SendRoundWon(int mode)
	{
		string appVersionField = ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion;
		while (true)
		{
			WWWForm form = new WWWForm();
			form.AddField("action", "round_won");
			form.AddField("app_version", appVersionField);
			form.AddField("uniq_id", id);
			form.AddField("mode", mode);
			form.AddField("auth", Hash("round_won"));
			WWW roundWonRequest = Tools.CreateWww(actionAddress, form, string.Empty);
			yield return roundWonRequest;
			string response = URLs.Sanitize(roundWonRequest);
			if (string.IsNullOrEmpty(roundWonRequest.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
			{
				Debug.Log("_SendRoundWon: " + response);
			}
			if (!string.IsNullOrEmpty(roundWonRequest.error))
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogWarning("_SendRoundWon error: " + roundWonRequest.error);
				}
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			if (string.IsNullOrEmpty(response) || !response.Equals("fail"))
			{
				break;
			}
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_SendRoundWon fail.");
			}
			yield return StartCoroutine(MyWaitForSeconds(10f));
		}
		PlayerPrefs.SetInt("TotalWinsForLeaderboards", PlayerPrefs.GetInt("TotalWinsForLeaderboards", 0) + 1);
	}

	private IEnumerator _ChangeClanLogo()
	{
		string appVersionField = ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion;
		while (true)
		{
			WWWForm form = new WWWForm();
			form.AddField("action", "change_logo");
			form.AddField("app_version", appVersionField);
			form.AddField("id_clan", ClanID);
			form.AddField("logo", clanLogo);
			form.AddField("id", id);
			form.AddField("uniq_id", id);
			form.AddField("auth", Hash("change_logo"));
			WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
			if (download == null)
			{
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			yield return download;
			string response = URLs.Sanitize(download);
			if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
			{
				Debug.Log("_ChangeClanLogo: " + response);
			}
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogWarning("_ChangeClanLogo error: " + download.error);
				}
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			if (!string.IsNullOrEmpty(response) && response.Equals("fail"))
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogWarning("_ChangeClanLogo fail.");
				}
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			break;
		}
	}

	public void ChangeClanName(string newNm, Action onSuccess, Action<string> onFailure)
	{
		if (readyToOperate)
		{
			StartCoroutine(_ChangeClanName(newNm, onSuccess, onFailure));
		}
	}

	private IEnumerator _ChangeClanName(string newNm, Action onSuccess, Action<string> onFailure)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "change_clan_name");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id_clan", ClanID);
		form.AddField("id", id);
		string filteredNick2 = newNm;
		filteredNick2 = FilterBadWorld.FilterString(newNm);
		form.AddField("name", filteredNick2);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("change_clan_name"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			if (onFailure != null)
			{
				onFailure("Request skipped.");
			}
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("_ChangeClanName: " + response);
		}
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_ChangeClanName error: " + download.error);
			}
			if (onFailure != null)
			{
				onFailure(download.error);
			}
		}
		else if (!string.IsNullOrEmpty(response) && response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_ChangeClanName fail.");
			}
			if (onFailure != null)
			{
				onFailure(response);
			}
		}
		else if (onSuccess != null)
		{
			onSuccess();
		}
	}

	private IEnumerator UpdatePopularityMapsLoop()
	{
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage < TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
		{
			yield return null;
		}
		while (true)
		{
			UpdatePopularityMaps();
			yield return StartCoroutine(MyWaitForSeconds(1800f));
		}
	}

	public void UpdatePopularityMaps()
	{
		StopCoroutine("GetPopularityMap");
		StartCoroutine("GetPopularityMap");
	}

	private IEnumerator GetPopularityMap()
	{
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage <= TrainingController.NewTrainingCompletedStage.None)
		{
			yield return null;
		}
		Dictionary<string, object> dict;
		while (true)
		{
			WWW download = Tools.CreateWwwIfNotConnected(URLs.PopularityMapUrl);
			if (download == null)
			{
				yield break;
			}
			yield return download;
			string response = URLs.Sanitize(download);
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild || Application.isEditor)
				{
					Debug.Log("CheckMapPopularity error: " + download.error);
				}
				yield return StartCoroutine(MyWaitForSeconds(18f));
				continue;
			}
			if (!string.IsNullOrEmpty(response) && response.Equals("fail"))
			{
				if (Debug.isDebugBuild || Application.isEditor)
				{
					Debug.LogWarning("CheckMapPopularity fail.");
				}
				yield return StartCoroutine(MyWaitForSeconds(18f));
				continue;
			}
			object o = Json.Deserialize(response);
			dict = o as Dictionary<string, object>;
			if (dict != null)
			{
				break;
			}
			if (Application.isEditor || Debug.isDebugBuild)
			{
				Debug.LogWarning(" GetPopularityMap dict = null");
			}
			yield return StartCoroutine(MyWaitForSeconds(20f));
		}
		foreach (KeyValuePair<string, object> kvp in dict)
		{
			Dictionary<string, string> _mapPopularityInRegim = new Dictionary<string, string>();
			Dictionary<string, object> dict2 = kvp.Value as Dictionary<string, object>;
			if (dict2 == null)
			{
				continue;
			}
			foreach (KeyValuePair<string, object> kvp2 in dict2)
			{
				_mapPopularityInRegim.Add(kvp2.Key, kvp2.Value.ToString());
			}
			if (_mapPopularityInRegim.Count > 0 && !mapPopularityDictionary.ContainsKey(kvp.Key))
			{
				mapPopularityDictionary.Add(kvp.Key, _mapPopularityInRegim);
			}
		}
	}

	private IEnumerator GetABTestBalansConfigName()
	{
		string response2;
		while (true)
		{
			WWWForm form = new WWWForm();
			form.AddField("action", "get_cohort_name");
			form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
			form.AddField("device", SystemInfo.deviceUniqueIdentifier);
			WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
			if (download == null)
			{
				yield return StartCoroutine(MyWaitForSeconds(1f));
				continue;
			}
			yield return download;
			response2 = URLs.Sanitize(download);
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log("get_cohort_name error: " + download.error);
				}
				yield return StartCoroutine(MyWaitForSeconds(1f));
				continue;
			}
			if (!"fail".Equals(response2))
			{
				break;
			}
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("get_cohort_name fail.");
			}
			yield return StartCoroutine(MyWaitForSeconds(1f));
		}
		if ("skip".Equals(response2))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("get_cohort_name skip");
			}
			getCohortInfo = true;
		}
		else
		{
			response2 = response2.Replace("/", "-");
			StartCoroutine(GetABTestBalansConfig(response2, true));
		}
	}

	private IEnumerator GetABTestBalansConfig(string nameConfig, bool isFirst)
	{
		WWW download;
		while (true)
		{
			WWWForm form = new WWWForm();
			download = Tools.CreateWwwIfNotConnected((!nameConfig.Equals("none")) ? (URLs.ABTestBalansFolderURL + nameConfig + ".json") : URLs.ABTestBalansURL);
			if (download == null)
			{
				yield return StartCoroutine(MyWaitForSeconds(1f));
				continue;
			}
			yield return download;
			if (nameConfig.Equals("none") && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted))
			{
				getCohortInfo = true;
				yield break;
			}
			if (string.IsNullOrEmpty(download.error))
			{
				break;
			}
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning("GetABTestBalansConfig error: " + download.error);
			}
			yield return StartCoroutine(MyWaitForSeconds(1f));
		}
		string responseText = URLs.Sanitize(download);
		if (!string.IsNullOrEmpty(responseText) && responseText != Storager.getString("abTestBalansConfig2Key", false))
		{
			Storager.setString("abTestBalansConfig2Key", responseText, false);
			getCohortInfo = true;
			if (!nameConfig.Equals("none"))
			{
				Defs.isABTestBalansNoneSkip = true;
			}
			BalanceController.sharedController.ParseConfig(isFirst);
			if (isFirst && Defs.abTestBalansCohort == Defs.ABTestCohortsType.B)
			{
				StartCoroutine(GetABTestBalansCohortNameActual());
			}
			if (Debug.isDebugBuild)
			{
				Debug.Log("GetConfigABtestBalans");
			}
		}
	}

	private IEnumerator GetABTestBalansCohortNameActual()
	{
		while (true)
		{
			WWWForm form = new WWWForm();
			WWW download = Tools.CreateWwwIfNotConnected(URLs.ABTestBalansActualCohortNameURL);
			if (download == null)
			{
				yield return StartCoroutine(MyWaitForSeconds(60f));
				continue;
			}
			yield return download;
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Application.isEditor)
				{
					Debug.LogWarning("GetABTestBalansCohortNameActual error: " + download.error);
				}
				yield return StartCoroutine(MyWaitForSeconds(60f));
				continue;
			}
			string responseText = URLs.Sanitize(download);
			if (!string.IsNullOrEmpty(responseText))
			{
				Dictionary<string, object> _nameCohortDict = Json.Deserialize(responseText) as Dictionary<string, object>;
				if (_nameCohortDict == null)
				{
					if (Application.isEditor)
					{
						Debug.LogWarning("GetABTestBalansCohortNameActual parse error");
					}
					yield return StartCoroutine(MyWaitForSeconds(60f));
					continue;
				}
				if (Debug.isDebugBuild)
				{
					Debug.Log("GetConfigABtestBalans");
				}
				if (!Convert.ToString(_nameCohortDict["ActualCohortNameB"]).Equals(Defs.abTestBalansCohortName))
				{
					break;
				}
				StartCoroutine(GetABTestBalansConfig(Defs.abTestBalansCohortName, false));
				yield return StartCoroutine(MyWaitForSeconds(900f));
			}
			else
			{
				yield return StartCoroutine(MyWaitForSeconds(60f));
			}
		}
		Defs.isABTestBalansCohortActual = false;
		ResetABTestsBalans();
	}

	public static void ResetABTestsBalans()
	{
		Storager.setString("abTestBalansConfig2Key", string.Empty, false);
		if (BalanceController.sharedController != null)
		{
			BalanceController.sharedController.ParseConfig(false);
		}
		Defs.abTestBalansCohort = Defs.ABTestCohortsType.NONE;
		Defs.abTestBalansCohortName = string.Empty;
		Defs.isABTestBalansCohortActual = false;
	}

	private IEnumerator GetBanList()
	{
		string appVersionField = ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion;
		int ban;
		while (true)
		{
			if (string.IsNullOrEmpty(id))
			{
				yield return null;
				continue;
			}
			WWWForm form = new WWWForm();
			form.AddField("app_version", appVersionField);
			form.AddField("id", id);
			WWW download = Tools.CreateWwwIfNotConnected(URLs.BanURL, form, string.Empty);
			if (download == null)
			{
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			yield return download;
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild || Application.isEditor)
				{
					Debug.LogWarning("GetBanList error: " + download.error);
				}
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			string responseText = URLs.Sanitize(download);
			if (int.TryParse(responseText, out ban))
			{
				break;
			}
			Debug.LogWarning("GetBanList cannot parse ban!");
			yield return StartCoroutine(MyWaitForSeconds(10f));
		}
		Banned = ban;
		if (Debug.isDebugBuild)
		{
			Debug.Log("GetBanList Banned: " + Banned);
		}
	}

	private IEnumerator CheckOurIDExists()
	{
		string appVersionField = ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion;
		string response;
		while (true)
		{
			WWWForm form = new WWWForm();
			form.AddField("action", "start_check");
			form.AddField("app_version", appVersionField);
			form.AddField("platform", ProtocolListGetter.CurrentPlatform.ToString());
			form.AddField("uniq_id", sharedController.id);
			form.AddField("device_model", SystemInfo.deviceModel);
			form.AddField("type_device", Device.isPixelGunLow ? 1 : 2);
			form.AddField("auth", Hash("start_check"));
			form.AddField("abuse_method", Storager.getInt("AbuseMethod", false));
			if (Launcher.PackageInfo.HasValue)
			{
			}
			WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
			if (download == null)
			{
				yield return StartCoroutine(MyWaitForSeconds(Defs.timeUpdateStartCheckIfNullResponce));
				continue;
			}
			yield return download;
			response = URLs.Sanitize(download);
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogWarning("CheckOurIDExists error: " + download.error);
				}
				yield return StartCoroutine(MyWaitForSeconds(Defs.timeUpdateStartCheckIfNullResponce));
				continue;
			}
			if (!"fail".Equals(response))
			{
				break;
			}
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("CheckOurIDExists fail.");
			}
			yield return StartCoroutine(MyWaitForSeconds(Defs.timeUpdateStartCheckIfNullResponce));
		}
		int newId;
		if (!int.TryParse(response, out newId))
		{
			Dictionary<string, object> clanInfo = Json.Deserialize(response) as Dictionary<string, object>;
			if (clanInfo == null)
			{
				Debug.LogWarning("CheckOurIDExists cannot parse clan info!");
			}
			else
			{
				object clanIDObj;
				if (clanInfo.TryGetValue("id", out clanIDObj) && clanIDObj != null && !clanIDObj.Equals("null"))
				{
					ClanID = Convert.ToString(clanIDObj);
				}
				object clanCreatorObj;
				if (clanInfo.TryGetValue("creator_id", out clanCreatorObj) && clanCreatorObj != null && !clanCreatorObj.Equals("null"))
				{
					clanLeaderID = clanCreatorObj as string;
				}
				object clanNameObj;
				if (clanInfo.TryGetValue("name", out clanNameObj) && clanNameObj != null && !clanNameObj.Equals("null"))
				{
					_prevClanName = clanName;
					clanName = clanNameObj as string;
					if (!_prevClanName.Equals(clanName) && onChangeClanName != null)
					{
						onChangeClanName(clanName);
					}
				}
				object clanLogoObj;
				if (clanInfo.TryGetValue("logo", out clanLogoObj) && clanLogoObj != null && !clanLogoObj.Equals("null"))
				{
					clanLogo = clanLogoObj as string;
				}
			}
		}
		else
		{
			Storager.setString(AccountCreated, response, false);
			id = response;
			onlineInfo.Clear();
			friends.Clear();
			invitesFromUs.Clear();
			playersInfo.Clear();
			invitesToUs.Clear();
			ClanInvites.Clear();
			notShowAddIds.Clear();
			SaveCurrentState();
			PlayerPrefs.Save();
		}
		readyToOperate = true;
		StartCoroutine(GetFriendsDataLoop());
		StartCoroutine(GetClanDataLoop());
		GetOurLAstOnline();
		StartCoroutine(RequestWinCountTimestampCoroutine());
		GetOurWins();
	}

	public void InitOurInfo()
	{
		nick = ProfileController.GetPlayerNameOrDefault();
		byte[] inArray = SkinsController.currentSkinForPers.EncodeToPNG();
		skin = Convert.ToBase64String(inArray);
		rank = ExperienceController.sharedController.currentLevel;
		wins = Storager.getInt("Rating", false);
		survivalScore = PlayerPrefs.GetInt(Defs.SurvivalScoreSett, 0);
		coopScore = PlayerPrefs.GetInt(Defs.COOPScore, 0);
		infoLoaded = true;
	}

	public IEnumerator WaitForReadyToOperateAndUpdatePlayer()
	{
		while (!readyToOperate)
		{
			yield return null;
		}
		StartCoroutine(UpdatePlayer(true));
	}

	public void SendOurData(bool SendSkin = false)
	{
		if (readyToOperate)
		{
			StartCoroutine(UpdatePlayer(SendSkin));
		}
	}

	public void SendOurDataInConnectScene()
	{
		if (!(Time.realtimeSinceStartup - timeSendUpdatePlayer < timeOutSendUpdatePlayerFromConnectScene) && readyToOperate)
		{
			StartCoroutine(UpdatePlayer(false));
		}
	}

	private void SaveCurrentState()
	{
		if (friends != null)
		{
			string text = Json.Serialize(friends);
			PlayerPrefs.SetString("FriendsKey", text ?? "[]");
		}
		if (invitesToUs != null)
		{
			string text2 = Json.Serialize(invitesToUs);
			PlayerPrefs.SetString("ToUsKey", text2 ?? "[]");
		}
		if (playersInfo != null)
		{
			string text3 = Json.Serialize(playersInfo);
			PlayerPrefs.SetString("PlayerInfoKey", text3 ?? "{}");
		}
		if (friendsInfo != null)
		{
			string text4 = Json.Serialize(friendsInfo);
			PlayerPrefs.SetString("FriendsInfoKey", text4 ?? "{}");
		}
		if (clanFriendsInfo != null)
		{
			string text5 = Json.Serialize(clanFriendsInfo);
			PlayerPrefs.SetString("ClanFriendsInfoKey", text5 ?? "{}");
		}
		if (ClanInvites != null)
		{
			string text6 = Json.Serialize(ClanInvites);
			PlayerPrefs.SetString("ClanInvitesKey", text6 ?? "[]");
		}
		UpdateCachedClickJoinListValue();
	}

	private void DumpCurrentState()
	{
	}

	private IEnumerator OnApplicationPause(bool pause)
	{
		if (pause)
		{
			DumpCurrentState();
			AnalyticsStuff.SaveTrainingStep();
			yield break;
		}
		isUpdateServerTimeAfterRun = false;
		firstUpdateAfterApplicationPause = true;
		yield return null;
		yield return null;
		yield return null;
		StartSendReview();
		if (GiftBannerWindow.instance != null)
		{
			GiftBannerWindow.instance.ForceCloseAll();
		}
		StopCoroutine("GetBanList");
		StartCoroutine("GetBanList");
		if (Defs.isABTestBalansCohortActual)
		{
			StopCoroutine(GetABTestBalansCohortNameActual());
			StartCoroutine(GetABTestBalansCohortNameActual());
		}
		StopCoroutine("GetABTestAdvertConfig");
		StartCoroutine("GetABTestAdvertConfig");
		StartCoroutine("GetCurrentcompetition");
		StartCoroutine("GetCurrentcompetition");
		UpdatePopularityMaps();
		StartCoroutine(GetTimeFromServerLoop());
		StartCoroutine(GetFiltersSettings());
		StartCoroutine(GetLobbyNews(true));
		Task futureToWait = PersistentCacheManager.Instance.FirstResponse;
		StartCoroutine(GetBuffSettings(futureToWait));
		StartCoroutine(GetRatingSystemConfig());
		GetFriendsData(true);
		FastGetPixelbookSettings();
		if (SceneLoader.ActiveSceneName.Equals("Friends") && FriendsGUIController.ShowProfile && FriendProfileController.currentFriendId != null && readyToOperate)
		{
			StartCoroutine(UpdatePlayerInfoById(FriendProfileController.currentFriendId));
		}
		if (SceneLoader.ActiveSceneName.Equals("Clans"))
		{
			if (!string.IsNullOrEmpty(ClanID))
			{
				StartCoroutine(GetClanDataOnce());
			}
			if (ClansGUIController.AtAddPanel)
			{
				StartCoroutine(GetAllPlayersOnline());
			}
			else
			{
				StartCoroutine(GetClanPlayersOnline());
			}
			if (ClansGUIController.ShowProfile && FriendProfileController.currentFriendId != null && readyToOperate)
			{
				StartCoroutine(UpdatePlayerInfoById(FriendProfileController.currentFriendId));
			}
		}
	}

	private void OnDestroy()
	{
		DumpCurrentState();
	}

	public void SendInvitation(string personId, Dictionary<string, object> socialEventParameters)
	{
		if (!string.IsNullOrEmpty(personId) && readyToOperate)
		{
			if (socialEventParameters == null)
			{
				throw new ArgumentNullException("socialEventParameters");
			}
			StartCoroutine(FriendRequest(personId, socialEventParameters));
		}
	}

	public void SendCreateClan(string personId, string nameClan, string skinClan, Action<string> ErrorHandler)
	{
		if (!string.IsNullOrEmpty(personId) && !string.IsNullOrEmpty(nameClan) && !string.IsNullOrEmpty(skinClan) && readyToOperate)
		{
			StartCoroutine(_SendCreateClan(personId, nameClan, skinClan, ErrorHandler));
		}
		else if (ErrorHandler != null)
		{
			ErrorHandler("Error: FALSE:  ! string.IsNullOrEmpty (personId) && ! string.IsNullOrEmpty (nameClan) && ! string.IsNullOrEmpty (skinClan) && readyToOperate");
		}
	}

	public static void SendPlayerInviteToClan(string personId, Action<bool, bool> callbackResult = null)
	{
		if (!(sharedController == null) && !string.IsNullOrEmpty(personId) && readyToOperate)
		{
			sharedController.StartCoroutine(sharedController.SendClanInvitation(personId, callbackResult));
		}
	}

	public void AcceptInvite(string accepteeId, Action<bool> action = null)
	{
		if (!string.IsNullOrEmpty(accepteeId) && readyToOperate)
		{
			StartCoroutine(AcceptFriend(accepteeId, action));
		}
	}

	public void AcceptClanInvite(string recordId)
	{
		if (!string.IsNullOrEmpty(recordId) && readyToOperate)
		{
			StartCoroutine(_AcceptClanInvite(recordId));
		}
	}

	private IEnumerator _AcceptClanInvite(string recordId)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "accept_invite");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id_player", id);
		form.AddField("id_clan", recordId);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("accept_invite"));
		WWW acceptInviteRequest = Tools.CreateWww(actionAddress, form, string.Empty);
		JoinClanSent = recordId;
		yield return acceptInviteRequest;
		string response = URLs.Sanitize(acceptInviteRequest);
		if (string.IsNullOrEmpty(acceptInviteRequest.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("Accept clan invite: " + response);
		}
		if (!string.IsNullOrEmpty(acceptInviteRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_AcceptClanInvite error: " + acceptInviteRequest.error);
			}
			JoinClanSent = null;
		}
		else if (!string.IsNullOrEmpty(response) && response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_AcceptClanInvite fail.");
			}
			JoinClanSent = null;
		}
		else
		{
			clanLogo = tempClanLogo;
			ClanID = tempClanID;
			clanName = tempClanName;
			clanLeaderID = tempClanCreatorID;
		}
	}

	public void StartRefreshingOnline()
	{
		if (readyToOperate)
		{
			_shouldStopOnline = false;
			StartCoroutine(RefreshOnlinePlayer());
		}
	}

	public void StopRefreshingOnline()
	{
		if (readyToOperate)
		{
			_shouldStopOnline = true;
		}
	}

	public void StartRefreshingOnlineWithClanInfo()
	{
		if (readyToOperate)
		{
			_shouldStopOnlineWithClanInfo = false;
			StartCoroutine(RefreshOnlinePlayerWithClanInfo());
		}
	}

	public void StopRefreshingOnlineWithClanInfo()
	{
		if (readyToOperate)
		{
			_shouldStopOnlineWithClanInfo = true;
		}
	}

	private IEnumerator RefreshOnlinePlayerWithClanInfo()
	{
		while (true)
		{
			if (idle)
			{
				yield return null;
				continue;
			}
			StartCoroutine(GetAllPlayersOnlineWithClanInfo());
			float startTime = Time.realtimeSinceStartup;
			do
			{
				yield return null;
			}
			while (Time.realtimeSinceStartup - startTime < 20f && !_shouldStopOnlineWithClanInfo);
			if (_shouldStopOnlineWithClanInfo)
			{
				break;
			}
		}
		_shouldStopOnlineWithClanInfo = false;
	}

	private IEnumerator RefreshOnlinePlayer()
	{
		while (true)
		{
			if (idle)
			{
				yield return null;
				continue;
			}
			StartCoroutine(GetAllPlayersOnline());
			float startTime = Time.realtimeSinceStartup;
			do
			{
				yield return null;
			}
			while (Time.realtimeSinceStartup - startTime < 20f && !_shouldStopOnline);
			if (_shouldStopOnline)
			{
				break;
			}
		}
		_shouldStopOnline = false;
	}

	public void StartRefreshingClanOnline()
	{
		if (readyToOperate)
		{
			_shouldStopRefrClanOnline = false;
			StartCoroutine(RefreshClanOnline());
		}
	}

	public void StopRefreshingClanOnline()
	{
		if (readyToOperate)
		{
			_shouldStopRefrClanOnline = true;
		}
	}

	private IEnumerator RefreshClanOnline()
	{
		while (true)
		{
			if (idle)
			{
				yield return null;
				continue;
			}
			StartCoroutine(GetClanPlayersOnline());
			float startTime = Time.realtimeSinceStartup;
			do
			{
				yield return null;
			}
			while (Time.realtimeSinceStartup - startTime < 20f && !_shouldStopRefrClanOnline);
			if (_shouldStopRefrClanOnline)
			{
				break;
			}
		}
		_shouldStopRefrClanOnline = false;
	}

	private IEnumerator GetClanPlayersOnline()
	{
		if (!readyToOperate)
		{
			yield break;
		}
		List<string> ids = new List<string>();
		foreach (Dictionary<string, string> fr in clanMembers)
		{
			string firIdStr;
			if (fr.TryGetValue("id", out firIdStr))
			{
				ids.Add(firIdStr);
			}
		}
		yield return StartCoroutine(_GetOnlineForPlayerIDs(ids));
	}

	private IEnumerator GetAllPlayersOnline()
	{
		if (readyToOperate)
		{
			yield return StartCoroutine(_GetOnlineForPlayerIDs(friends));
		}
	}

	private IEnumerator GetAllPlayersOnlineWithClanInfo()
	{
		if (readyToOperate)
		{
			yield return StartCoroutine(_GetOnlineWithClanInfoForPlayerIDs(friends));
		}
	}

	private IEnumerator _GetOnlineForPlayerIDs(List<string> ids)
	{
		if (ids.Count == 0)
		{
			yield break;
		}
		string json = Json.Serialize(ids);
		if (json == null)
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "get_all_players_online");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id", id);
		form.AddField("ids", json);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("get_all_players_online"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && (Debug.isDebugBuild || Application.isEditor))
		{
			Debug.Log(response);
		}
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning("GetAllPlayersOnline error: " + download.error);
			}
			yield break;
		}
		Dictionary<string, object> __list = Json.Deserialize(response) as Dictionary<string, object>;
		if (__list == null)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning(" GetAllPlayersOnline info = null");
			}
			yield break;
		}
		Dictionary<string, Dictionary<string, string>> list = new Dictionary<string, Dictionary<string, string>>();
		foreach (string key2 in __list.Keys)
		{
			Dictionary<string, object> d2 = __list[key2] as Dictionary<string, object>;
			Dictionary<string, string> newd = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> kvp in d2)
			{
				newd.Add(kvp.Key, kvp.Value as string);
			}
			list.Add(key2, newd);
		}
		onlineInfo.Clear();
		foreach (string key in list.Keys)
		{
			Dictionary<string, string> d = list[key];
			int _game_mode = int.Parse(d["game_mode"]);
			int _regim = _game_mode - _game_mode / 10 * 10;
			if (_regim != 3 && _regim != 8)
			{
				if (!onlineInfo.ContainsKey(key))
				{
					onlineInfo.Add(key, d);
				}
				else
				{
					onlineInfo[key] = d;
				}
			}
		}
	}

	private IEnumerator _GetOnlineWithClanInfoForPlayerIDs(List<string> ids)
	{
		if (ids.Count == 0)
		{
			yield break;
		}
		string json = Json.Serialize(ids);
		if (json == null)
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "get_all_players_online_with_clan_info");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id", id);
		form.AddField("ids", json);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("get_all_players_online_with_clan_info"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log(response);
		}
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_GetOnlineWithClanInfoForPlayerIDs error: " + download.error);
			}
			yield break;
		}
		Dictionary<string, object> allDict = Json.Deserialize(response) as Dictionary<string, object>;
		if (allDict == null)
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning(" _GetOnlineWithClanInfoForPlayerIDs allDict = null");
			}
			yield break;
		}
		Dictionary<string, object> __list = allDict["online"] as Dictionary<string, object>;
		if (__list == null)
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning(" _GetOnlineWithClanInfoForPlayerIDs __list = null");
			}
			yield break;
		}
		Dictionary<string, Dictionary<string, string>> list = new Dictionary<string, Dictionary<string, string>>();
		foreach (string key3 in __list.Keys)
		{
			Dictionary<string, object> d3 = __list[key3] as Dictionary<string, object>;
			Dictionary<string, string> newd2 = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> kvp2 in d3)
			{
				newd2.Add(kvp2.Key, kvp2.Value as string);
			}
			list.Add(key3, newd2);
		}
		onlineInfo.Clear();
		foreach (string key2 in list.Keys)
		{
			Dictionary<string, string> d2 = list[key2];
			int _game_mode = int.Parse(d2["game_mode"]);
			int _regim = _game_mode - _game_mode / 10 * 10;
			if (_regim != 3 && _regim != 8)
			{
				if (!onlineInfo.ContainsKey(key2))
				{
					onlineInfo.Add(key2, d2);
				}
				else
				{
					onlineInfo[key2] = d2;
				}
			}
		}
		Dictionary<string, object> clanInfo = allDict["clan_info"] as Dictionary<string, object>;
		if (clanInfo == null)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning(" _GetOnlineWithClanInfoForPlayerIDs clanInfo = null");
			}
			yield break;
		}
		Dictionary<string, Dictionary<string, string>> convertedClanInfo = new Dictionary<string, Dictionary<string, string>>();
		foreach (string key in clanInfo.Keys)
		{
			Dictionary<string, object> d = clanInfo[key] as Dictionary<string, object>;
			Dictionary<string, string> newd = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> kvp in d)
			{
				newd.Add(kvp.Key, Convert.ToString(kvp.Value));
			}
			convertedClanInfo.Add(key, newd);
		}
		foreach (string playerID in convertedClanInfo.Keys)
		{
			Dictionary<string, string> playerClanInfo = convertedClanInfo[playerID];
			if (!sharedController.playersInfo.ContainsKey(playerID))
			{
				continue;
			}
			Dictionary<string, object> pl = sharedController.playersInfo[playerID];
			if (pl.ContainsKey("player"))
			{
				Dictionary<string, object> plpl = pl["player"] as Dictionary<string, object>;
				if (plpl.ContainsKey("clan_creator_id"))
				{
					plpl["clan_creator_id"] = playerClanInfo["clan_creator_id"];
				}
				else
				{
					plpl.Add("clan_creator_id", playerClanInfo["clan_creator_id"]);
				}
				if (plpl.ContainsKey("clan_name"))
				{
					plpl["clan_name"] = playerClanInfo["clan_name"];
				}
				else
				{
					plpl.Add("clan_name", playerClanInfo["clan_name"]);
				}
				if (plpl.ContainsKey("clan_logo"))
				{
					plpl["clan_logo"] = playerClanInfo["clan_logo"];
				}
				else
				{
					plpl.Add("clan_logo", playerClanInfo["clan_logo"]);
				}
			}
		}
	}

	public void GetFacebookFriendsInfo(Action callb)
	{
		if (readyToOperate)
		{
			StartCoroutine(_GetFacebookFriendsInfo(callb));
		}
	}

	private IEnumerator _GetFacebookFriendsInfo(Action callb)
	{
		if (!FacebookController.FacebookSupported || FacebookController.sharedController.friendsList == null)
		{
			yield break;
		}
		GetFacebookFriendsCallback = callb;
		List<string> ids = new List<string>();
		foreach (Friend f in FacebookController.sharedController.friendsList)
		{
			ids.Add(f.id);
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "get_info_by_facebook_ids");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("platform", ProtocolListGetter.CurrentPlatform.ToString());
		form.AddField("id", id);
		string json = Json.Serialize(ids);
		form.AddField("ids", json);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("get_info_by_facebook_ids"));
		Debug.LogFormat("Facebook json: {0}", json);
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			GetFacebookFriendsCallback = null;
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && (Debug.isDebugBuild || Application.isEditor))
		{
			Debug.Log(response);
		}
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning("_GetFacebookFriendsInfo error: " + download.error);
			}
			GetFacebookFriendsCallback = null;
			yield break;
		}
		List<object> __info = Json.Deserialize(response) as List<object>;
		if (__info == null)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning(" _GetFacebookFriendsInfo info = null");
			}
			GetFacebookFriendsCallback = null;
			yield break;
		}
		foreach (Dictionary<string, object> d in __info)
		{
			Dictionary<string, object> ff = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> i in d)
			{
				ff.Add(i.Key, i.Value);
			}
			object ffid;
			if (ff.TryGetValue("id", out ffid))
			{
				facebookFriendsInfo.Add(ffid as string, ff);
			}
		}
		if (GetFacebookFriendsCallback != null)
		{
			GetFacebookFriendsCallback();
		}
		GetFacebookFriendsCallback = null;
	}

	private IEnumerator UpdatePlayerOnlineLoop()
	{
		while (true)
		{
			if (idle)
			{
				yield return null;
				continue;
			}
			int gameMode = -1;
			int platform = (int)ConnectSceneNGUIController.myPlatformConnect;
			if (PhotonNetwork.room != null)
			{
				gameMode = (int)ConnectSceneNGUIController.regim;
				if (!string.IsNullOrEmpty(PhotonNetwork.room.customProperties[ConnectSceneNGUIController.passwordProperty].ToString()))
				{
					platform = 3;
				}
			}
			if (gameMode != -1)
			{
				StartCoroutine(UpdatePlayerOnline(platform * 100 + ConnectSceneNGUIController.gameTier * 10 + gameMode));
			}
			yield return StartCoroutine(MyWaitForSeconds(Defs.timeUpdateOnlineInGame));
		}
	}

	public void SendAddPurchaseEvent(string purchaseId, string transactionId, float parsedPrice, string currencyCode, string countryCode)
	{
		int inapp = 0;
		int num = Array.IndexOf(StoreKitEventListener.coinIds, purchaseId);
		bool flag = false;
		if (num != -1)
		{
			inapp = VirtualCurrencyHelper.coinPriceIds[num];
		}
		else
		{
			num = Array.IndexOf(StoreKitEventListener.gemsIds, purchaseId);
			if (num != -1)
			{
				inapp = VirtualCurrencyHelper.gemsPriceIds[num];
			}
		}
		StartCoroutine(AddPurchaseEvent(inapp, purchaseId, transactionId, parsedPrice, currencyCode, countryCode));
	}

	private IEnumerator AddPurchaseEvent(int inapp, string purchaseId, string transactionId, float parsedPrice, string currencyCode, string countryCode)
	{
		WaitForSeconds awaiter = new WaitForSeconds(5f);
		while (true)
		{
			WWWForm form = new WWWForm();
			form.AddField("action", "add_purchase");
			form.AddField("auth", Hash("add_purchase"));
			form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
			form.AddField("uniq_id", id);
			form.AddField("platform", ProtocolListGetter.CurrentPlatform.ToString());
			form.AddField("type_device", Device.isPixelGunLow ? 1 : 2);
			int playerLevel = ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
			form.AddField("rank", playerLevel);
			form.AddField("inapp", inapp);
			form.AddField("transactionId", transactionId);
			form.AddField("parsedPrice", Mathf.RoundToInt(parsedPrice * 1000f).ToString());
			form.AddField("currencyCode", currencyCode);
			form.AddField("countryCode", countryCode);
			form.AddField("tier", ExpController.OurTierForAnyPlace());
			if (Defs.abTestBalansCohort != 0 && Defs.abTestBalansCohort != Defs.ABTestCohortsType.SKIP)
			{
				form.AddField("cohortName", Defs.abTestBalansCohortName);
			}
			if (Defs.cohortABTestAdvert == Defs.ABTestCohortsType.A || Defs.cohortABTestAdvert == Defs.ABTestCohortsType.B)
			{
				string _currentConfigAdvertNameForEvent = ((Defs.cohortABTestAdvert != Defs.ABTestCohortsType.A) ? "AdvertB_" : "AdvertA_") + configNameABTestAdvert;
				form.AddField("cohort_ad", _currentConfigAdvertNameForEvent);
			}
			foreach (ABTestBase abtest in ABTestController.currentABTests)
			{
				if (abtest.cohort == ABTestController.ABTestCohortsType.A || abtest.cohort == ABTestController.ABTestCohortsType.B)
				{
					form.AddField(abtest.currentFolder, abtest.cohortName);
				}
			}
			form.AddField("purchaseId", purchaseId);
			float savedPlayTime = 0f;
			float savedPlayTimeInMatch = 0f;
			if (Storager.hasKey("PlayTime") && float.TryParse(Storager.getString("PlayTime", false), out savedPlayTime))
			{
				form.AddField("playTime", Mathf.RoundToInt(savedPlayTime));
			}
			if (Storager.hasKey("PlayTimeInMatch") && float.TryParse(Storager.getString("PlayTimeInMatch", false), out savedPlayTimeInMatch))
			{
				form.AddField("playTimeInMatch", Mathf.RoundToInt(savedPlayTimeInMatch));
			}
			WWW addPurchaseEventRequest = Tools.CreateWww(actionAddress, form, string.Empty);
			yield return addPurchaseEventRequest;
			string response = URLs.Sanitize(addPurchaseEventRequest);
			if (!string.IsNullOrEmpty(addPurchaseEventRequest.error))
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogWarning("AddPurchaseEvent error: " + addPurchaseEventRequest.error);
				}
				yield return awaiter;
				continue;
			}
			if (!string.IsNullOrEmpty(response) && response.Equals("fail"))
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogWarning("AddPurchaseEvent fail.");
				}
				yield return awaiter;
				continue;
			}
			break;
		}
	}

	public static void SendToturialEvent(int _event, string _progress)
	{
		CoroutineRunner.Instance.StartCoroutine(AddToturialEvent(_event, _progress));
	}

	private static IEnumerator AddToturialEvent(int _event, string _progress)
	{
		WWWForm form = new WWWForm();
		form.AddField("event_id", _event);
		form.AddField("progress", _progress);
		form.AddField("device_id", SystemInfo.deviceUniqueIdentifier);
		form.AddField("device_model", SystemInfo.deviceModel);
		form.AddField("type_device", Device.isPixelGunLow ? 1 : 2);
		form.AddField("platform", ProtocolListGetter.CurrentPlatform.ToString());
		form.AddField("version", GlobalGameController.AppVersion);
		form.AddField("release", (!Defs.IsDeveloperBuild) ? 1 : 0);
		WWW addToturialEventRequest = Tools.CreateWww("https://acct.pixelgunserver.com/events/add_event.php", form, string.Empty);
		yield return addToturialEventRequest;
		string response = URLs.Sanitize(addToturialEventRequest);
		if (!string.IsNullOrEmpty(addToturialEventRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("Toturial Event error: " + addToturialEventRequest.error);
			}
		}
		else if (!string.IsNullOrEmpty(response) && response.Equals("fail") && Debug.isDebugBuild)
		{
			Debug.LogWarning("Toturial Event fail.");
		}
	}

	public void SendRequestGetCurrentcompetition()
	{
		StartCoroutine("GetCurrentcompetition");
	}

	public IEnumerator GetCurrentcompetition()
	{
		string response;
		while (true)
		{
			if (string.IsNullOrEmpty(id))
			{
				yield return null;
				continue;
			}
			WWWForm form = new WWWForm();
			form.AddField("action", "get_current_competition");
			form.AddField("auth", Hash("get_current_competition"));
			form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
			form.AddField("uniq_id", id);
			WWW getCurrentSeasonRequest = Tools.CreateWww(actionAddress, form, string.Empty);
			yield return getCurrentSeasonRequest;
			if (!string.IsNullOrEmpty(getCurrentSeasonRequest.error))
			{
				if (Debug.isDebugBuild)
				{
					Debug.LogWarning("GetCurrentcompetitionRequest error: " + getCurrentSeasonRequest.error);
				}
				yield return new WaitForSeconds(20f);
				continue;
			}
			response = URLs.Sanitize(getCurrentSeasonRequest);
			if (string.IsNullOrEmpty(response) || !response.Equals("fail"))
			{
				break;
			}
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetCurrentcompetitionnRequest fail.");
			}
			yield return new WaitForSeconds(20f);
		}
		ParseResponseCurrenCompetion(response);
	}

	public IEnumerator SynchRating(int rating)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "synch_rating_tiers");
		form.AddField("auth", Hash("synch_rating_tiers"));
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("uniq_id", id);
		form.AddField("rating", rating);
		form.AddField("abuse_method", Storager.getInt("AbuseMethod", false));
		form.AddField("tier", ExpController.OurTierForAnyPlace());
		form.AddField("competition_id", currentCompetition);
		WWW synchRatingRequest = Tools.CreateWww(actionAddress, form, string.Empty);
		yield return synchRatingRequest;
		string response = URLs.Sanitize(synchRatingRequest);
		if (!string.IsNullOrEmpty(synchRatingRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("synchRatingRequest error: " + synchRatingRequest.error);
			}
		}
		else if (string.IsNullOrEmpty(response) || (!response.Equals("fail") && !response.Equals("ok")))
		{
			ParseResponseCurrenCompetion(response);
		}
	}

	private void ParseResponseCurrenCompetion(string _response)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(_response) as Dictionary<string, object>;
		bool flag = false;
		int result;
		if (dictionary.ContainsKey("competition_id") && int.TryParse(dictionary["competition_id"].ToString(), out result) && currentCompetition != result)
		{
			StartNewCompetion();
			flag = true;
			currentCompetition = result;
		}
		if (dictionary.ContainsKey("competition_time"))
		{
			expirationTimeCompetition = Convert.ToSingle(dictionary["competition_time"]);
		}
		bool flag2 = false;
		if (dictionary.ContainsKey("reward"))
		{
			Dictionary<string, object> dictionary2 = dictionary["reward"] as Dictionary<string, object>;
			if (dictionary2 != null && dictionary2.ContainsKey("place"))
			{
				int num = Convert.ToInt32(dictionary2["place"]);
				if (num <= BalanceController.countPlaceAwardInCompetion)
				{
					flag2 = true;
				}
			}
		}
		if (flag)
		{
			if (flag2)
			{
				TournamentWinnerBannerWindow.CanShow = true;
			}
			else if (RatingSystem.instance.currentLeague == RatingSystem.RatingLeague.Adamant)
			{
				TournamentLooserBannerWindow.CanShow = true;
			}
			int trophiesSeasonThreshold = RatingSystem.instance.TrophiesSeasonThreshold;
			if (RatingSystem.instance.currentRating > trophiesSeasonThreshold)
			{
				int num2 = RatingSystem.instance.currentRating - trophiesSeasonThreshold;
				RatingSystem.instance.negativeRating += num2;
				RatingSystem.instance.UpdateLeagueEvent(null, null);
			}
		}
	}

	private void StartNewCompetion()
	{
		if (!string.IsNullOrEmpty(id))
		{
			LeaderboardScript.RequestLeaderboards(id);
		}
	}

	private IEnumerator UpdatePlayerOnline(int gameMode)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "update_player_online");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id", id);
		form.AddField("game_mode", gameMode.ToString("D3"));
		form.AddField("room_name", (PhotonNetwork.room == null || PhotonNetwork.room.name == null) ? string.Empty : PhotonNetwork.room.name);
		form.AddField("map", (PhotonNetwork.room == null || PhotonNetwork.room.customProperties == null) ? string.Empty : PhotonNetwork.room.customProperties[ConnectSceneNGUIController.mapProperty].ToString());
		form.AddField("protocol", GlobalGameController.MultiplayerProtocolVersion);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("update_player_online"));
		form.AddField("platform", ProtocolListGetter.CurrentPlatform.ToString());
		form.AddField("type_device", Device.isPixelGunLow ? 1 : 2);
		form.AddField("game_time", Mathf.RoundToInt(deltaTimeInGame));
		sendingTime = Mathf.RoundToInt(deltaTimeInGame);
		form.AddField("tier", ExpController.OurTierForAnyPlace());
		if (Defs.abTestBalansCohort != 0 && Defs.abTestBalansCohort != Defs.ABTestCohortsType.SKIP)
		{
			form.AddField("cohortName", Defs.abTestBalansCohortName);
		}
		if (Defs.cohortABTestAdvert == Defs.ABTestCohortsType.A || Defs.cohortABTestAdvert == Defs.ABTestCohortsType.B)
		{
			string _currentConfigAdvertNameForEvent = ((Defs.cohortABTestAdvert != Defs.ABTestCohortsType.A) ? "AdvertB_" : "AdvertA_") + configNameABTestAdvert;
			form.AddField("cohort_ad", _currentConfigAdvertNameForEvent);
		}
		foreach (ABTestBase abtest in ABTestController.currentABTests)
		{
			if (abtest.cohort == ABTestController.ABTestCohortsType.A || abtest.cohort == ABTestController.ABTestCohortsType.B)
			{
				form.AddField(abtest.currentFolder, abtest.cohortName);
			}
		}
		form.AddField("paying", Storager.getInt("PayingUser", true).ToString());
		int playerLevel = ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
		form.AddField("rank", playerLevel);
		WWW updatePlayerOnlineRequest = Tools.CreateWww(actionAddress, form, string.Empty);
		yield return updatePlayerOnlineRequest;
		string response = URLs.Sanitize(updatePlayerOnlineRequest);
		if (!string.IsNullOrEmpty(updatePlayerOnlineRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("UpdatePlayerOnline error: " + updatePlayerOnlineRequest.error);
			}
			yield break;
		}
		if (!string.IsNullOrEmpty(response) && response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("UpdatePlayerOnline fail.");
			}
			yield break;
		}
		deltaTimeInGame -= sendingTime;
		if (!string.IsNullOrEmpty(response) && !response.Equals("ok"))
		{
			Dictionary<string, object> cacheResponse = Json.Deserialize(response) as Dictionary<string, object>;
			if (cacheResponse != null && cacheResponse.ContainsKey("fight_invites"))
			{
				ParseFightInvite(cacheResponse["fight_invites"] as List<object>);
			}
		}
	}

	private IEnumerator GetToken()
	{
		string appVersionField = ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion;
		WWWForm form = new WWWForm();
		form.AddField("action", "create_player_intent");
		form.AddField("app_version", appVersionField);
		string response;
		while (true)
		{
			WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
			if (download == null)
			{
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			yield return download;
			response = URLs.Sanitize(download);
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild || Application.isEditor)
				{
					Debug.Log("create_player_intent error: " + download.error);
				}
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			if (!string.IsNullOrEmpty(response) && response.Equals("fail"))
			{
				if (Debug.isDebugBuild || Application.isEditor)
				{
					Debug.LogWarning("create_player_intent fail.");
				}
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			if (response != null)
			{
				break;
			}
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning("create_player_intent response == null");
			}
			yield return StartCoroutine(MyWaitForSeconds(10f));
		}
		_inputToken = response;
	}

	private IEnumerator CreatePlayer()
	{
		string appVersionField = ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion;
		string response;
		while (true)
		{
			yield return StartCoroutine(GetToken());
			while (string.IsNullOrEmpty(_inputToken))
			{
				yield return null;
			}
			WWWForm form = new WWWForm();
			form.AddField("action", "create_player");
			form.AddField("platform", ProtocolListGetter.CurrentPlatform.ToString());
			form.AddField("device", SystemInfo.deviceUniqueIdentifier);
			form.AddField("device_model", SystemInfo.deviceModel);
			form.AddField("app_version", appVersionField);
			string hash = Hash("create_player", _inputToken);
			form.AddField("auth", hash);
			form.AddField("token", _inputToken);
			if (Defs.IsDeveloperBuild)
			{
				form.AddField("dev", 1);
			}
			string tokenHashString = string.Format("token:hash = {0}:{1}", _inputToken, hash);
			_inputToken = null;
			bool canPrintSecuritySensitiveInfo = Debug.isDebugBuild || Defs.IsDeveloperBuild;
			if (canPrintSecuritySensitiveInfo)
			{
				Debug.Log("CreatePlayer: Trying to perform request for “" + tokenHashString + "”...");
			}
			WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
			if (download == null)
			{
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			yield return download;
			response = URLs.Sanitize(download);
			if (canPrintSecuritySensitiveInfo)
			{
				Debug.LogFormat("CreatePlayer: Response for '{0}' received:    '{1}'", tokenHashString, response);
			}
			if (!string.IsNullOrEmpty(download.error))
			{
				Debug.LogWarning("CreatePlayer error:    " + download.error);
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			if (!string.IsNullOrEmpty(response) && response.Equals("fail"))
			{
				Debug.LogWarning("CreatePlayer failed.");
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			if (string.IsNullOrEmpty(response))
			{
				Debug.LogWarning("CreatePlayer response is empty.");
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			long resultId;
			if (!long.TryParse(response, out resultId))
			{
				Debug.LogWarning("CreatePlayer parsing error in response:    “" + response + "”");
				yield return StartCoroutine(MyWaitForSeconds(10f));
				continue;
			}
			if (resultId >= 1)
			{
				break;
			}
			Debug.LogWarning("CreatePlayer bad id:    “" + response + "”");
			yield return StartCoroutine(MyWaitForSeconds(10f));
		}
		Debug.Log("CreatePlayer succeeded with response:    “" + response + "”");
		Storager.setString(AccountCreated, response, false);
		id = response;
		readyToOperate = true;
		StartCoroutine(GetFriendsDataLoop());
		StartCoroutine(GetClanDataLoop());
		GetOurLAstOnline();
		GetOurWins();
	}

	private void SetWinCountTimestamp(string timestamp, int winCount)
	{
		_winCountTimestamp = new KeyValuePair<string, int>(timestamp, winCount);
		string text = string.Format("{{ \"{0}\": {1} }}", timestamp, winCount);
		Storager.setString("Win Count Timestamp", text, false);
		if (Application.isEditor)
		{
			Debug.Log("Setting win count timestamp:    " + text);
		}
	}

	public bool TryIncrementWinCountTimestamp()
	{
		if (!_winCountTimestamp.HasValue)
		{
			return false;
		}
		_winCountTimestamp = new KeyValuePair<string, int>(_winCountTimestamp.Value.Key, _winCountTimestamp.Value.Value + 1);
		return true;
	}

	private IEnumerator RequestWinCountTimestampCoroutine()
	{
		yield break;
	}

	private void GetOurLAstOnline()
	{
		StartCoroutine(GetInfoByEverydayDelta());
		ReceivedLastOnline = true;
		StartCoroutine(UpdatePlayerOnlineLoop());
	}

	public void DownloadInfoByEverydayDelta()
	{
		StartCoroutine(GetInfoByEverydayDelta());
	}

	private IEnumerator GetInfoByEverydayDelta()
	{
		bool needTakeMarathonBonus = false;
		WWWForm form = new WWWForm();
		form.AddField("action", "get_player_online");
		form.AddField("id", id);
		form.AddField("app_version", "*:*.*.*");
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("get_player_online"));
		WWW getPlayerOnlineRequest = Tools.CreateWwwIfNotConnected(actionAddress, form, "*:*.*.*");
		if (getPlayerOnlineRequest == null)
		{
			yield return StartCoroutine(MyWaitForSeconds(120f));
			yield break;
		}
		yield return getPlayerOnlineRequest;
		string response = URLs.Sanitize(getPlayerOnlineRequest);
		if (!string.IsNullOrEmpty(getPlayerOnlineRequest.error))
		{
			Debug.LogWarning("GetInfoByEverydayDelta()    Error: " + getPlayerOnlineRequest.error);
			yield return StartCoroutine(MyWaitForSeconds(120f));
			yield break;
		}
		if ("fail".Equals(response))
		{
			Debug.LogWarning("GetInfoByEverydayDelta()    Fail returned.");
			yield return StartCoroutine(MyWaitForSeconds(120f));
			yield break;
		}
		JSONNode data = JSON.Parse(response);
		if (data == null)
		{
			Debug.LogWarning("GetInfoByEverydayDelta()    Cannot deserialize response: " + response);
			yield return StartCoroutine(MyWaitForSeconds(120f));
			yield break;
		}
		string deltaData = data["delta"].Value;
		float deltaValue;
		if (float.TryParse(deltaData, out deltaValue))
		{
			if (deltaValue > 82800f)
			{
				NotificationController.isGetEveryDayMoney = true;
				if (Storager.getInt(Defs.NeedTakeMarathonBonus, false) == 0)
				{
					Storager.setInt(Defs.NeedTakeMarathonBonus, 1, false);
				}
			}
		}
		else
		{
			Debug.LogWarning("GetInfoByEverydayDelta()    Cannot parse delta: " + deltaData);
			yield return StartCoroutine(MyWaitForSeconds(120f));
		}
	}

	private string GetAccesoriesString()
	{
		string @string = Storager.getString(Defs.CapeEquppedSN, false);
		string value;
		if (@string == "cape_Custom")
		{
			string string2 = PlayerPrefs.GetString("NewUserCape");
			value = Tools.DeserializeJson<CapeMemento>(string2).Cape;
			if (string.IsNullOrEmpty(value))
			{
				value = SkinsController.StringFromTexture(Resources.Load<Texture2D>("cape_CustomTexture"));
			}
		}
		else
		{
			value = string.Empty;
		}
		string string3 = Storager.getString(Defs.HatEquppedSN, false);
		string string4 = Storager.getString(Defs.BootsEquppedSN, false);
		string string5 = Storager.getString("MaskEquippedSN", false);
		string string6 = Storager.getString(Defs.ArmorNewEquppedSN, false);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("type", "0");
		dictionary.Add("name", @string);
		dictionary.Add("skin", value);
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		dictionary2.Add("type", "1");
		dictionary2.Add("name", string3);
		dictionary2.Add("skin", string.Empty);
		Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
		dictionary3.Add("type", "2");
		dictionary3.Add("name", string4);
		dictionary3.Add("skin", string.Empty);
		Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
		dictionary4.Add("type", "3");
		dictionary4.Add("name", string6);
		dictionary4.Add("skin", string.Empty);
		Dictionary<string, string> dictionary5 = new Dictionary<string, string>();
		dictionary5.Add("type", "4");
		dictionary5.Add("name", string5);
		dictionary5.Add("skin", string.Empty);
		List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
		list.Add(dictionary);
		list.Add(dictionary2);
		list.Add(dictionary3);
		list.Add(dictionary4);
		list.Add(dictionary5);
		return Json.Serialize(list);
	}

	public void SendAccessories()
	{
		if (readyToOperate)
		{
			StartCoroutine(UpdateAccessories());
		}
	}

	private IEnumerator UpdateAccessories()
	{
		if (string.IsNullOrEmpty(id))
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "update_accessories");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("auth", Hash("update_accessories"));
		form.AddField("uniq_id", id);
		form.AddField("accessories", GetAccesoriesString());
		WWW updateAccessoriesRequest = Tools.CreateWww(actionAddress, form, string.Empty);
		yield return updateAccessoriesRequest;
		string response = URLs.Sanitize(updateAccessoriesRequest);
		if (string.IsNullOrEmpty(updateAccessoriesRequest.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("UpdateAccessories: " + response);
		}
		if (!string.IsNullOrEmpty(updateAccessoriesRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("UpdateAccessories error: " + updateAccessoriesRequest.error);
			}
		}
		else if (!string.IsNullOrEmpty(response) && response.Equals("fail") && (Debug.isDebugBuild || Application.isEditor))
		{
			Debug.LogWarning("UpdateAccessories fail.");
		}
	}

	private IEnumerator UpdatePlayer(bool sendSkin)
	{
		while (!ReceivedLastOnline || !infoLoaded || !getCohortInfo)
		{
			yield return null;
		}
		timeSendUpdatePlayer = Time.realtimeSinceStartup;
		InitOurInfo();
		WWWForm form = new WWWForm();
		form.AddField("action", "update_player");
		form.AddField("id", id);
		string filteredNick = nick;
		filteredNick = FilterBadWorld.FilterString(nick);
		if (filteredNick.Length > 20)
		{
			filteredNick = filteredNick.Substring(0, 20);
		}
		form.AddField("nick", filteredNick);
		form.AddField("skin", (!sendSkin) ? string.Empty : skin);
		form.AddField("rank", rank);
		form.AddField("wins", wins.Value);
		if (Defs.IsDeveloperBuild)
		{
			form.AddField("developer", 1);
		}
		form.AddField("cohortName", Defs.abTestBalansCohortName);
		if (Defs.cohortABTestAdvert == Defs.ABTestCohortsType.A || Defs.cohortABTestAdvert == Defs.ABTestCohortsType.B)
		{
			string _currentConfigAdvertNameForEvent = ((Defs.cohortABTestAdvert != Defs.ABTestCohortsType.A) ? "AdvertB_" : "AdvertA_") + configNameABTestAdvert;
			form.AddField("cohort_ad", _currentConfigAdvertNameForEvent);
		}
		foreach (ABTestBase abtest in ABTestController.currentABTests)
		{
			if (abtest.cohort == ABTestController.ABTestCohortsType.A || abtest.cohort == ABTestController.ABTestCohortsType.B)
			{
				form.AddField(abtest.currentFolder, abtest.cohortName);
			}
		}
		int totalWinCount = PlayerPrefs.GetInt("TotalWinsForLeaderboards", 0);
		form.AddField("total_wins", totalWinCount);
		form.AddField("id_fb", id_fb ?? string.Empty);
		form.AddField("device", SystemInfo.deviceUniqueIdentifier);
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("coins", Storager.getInt("Coins", false).ToString());
		form.AddField("gems", Storager.getInt("GemsCurrency", false).ToString());
		form.AddField("paying", Storager.getInt("PayingUser", true).ToString());
		form.AddField("kill_cnt", ProfileController.countGameTotalKills.Value);
		form.AddField("death_cnt", ProfileController.countGameTotalDeaths.Value);
		float savedPlayTime = 0f;
		float savedPlayTimeInMatch = 0f;
		if (Storager.hasKey("PlayTime") && float.TryParse(Storager.getString("PlayTime", false), out savedPlayTime))
		{
			form.AddField("playTime", Mathf.RoundToInt(savedPlayTime));
		}
		if (Storager.hasKey("PlayTimeInMatch") && float.TryParse(Storager.getString("PlayTimeInMatch", false), out savedPlayTimeInMatch))
		{
			form.AddField("playTimeInMatch", Mathf.RoundToInt(savedPlayTimeInMatch));
		}
		string killRatesString = Storager.getString("LastKillRates", false);
		List<object> killRateList = Json.Deserialize(killRatesString) as List<object>;
		if (killRateList != null && killRateList.Count == 2)
		{
			int[] kills = (killRateList[0] as List<object>).Select((object o) => Convert.ToInt32(o)).ToArray();
			int[] deaths = (killRateList[1] as List<object>).Select((object o) => Convert.ToInt32(o)).ToArray();
			int allKills = 0;
			int allDeath = 0;
			for (int i = 0; i < kills.Length; i++)
			{
				allKills += kills[i];
			}
			for (int j = 0; j < deaths.Length; j++)
			{
				allDeath += deaths[j];
			}
			form.AddField("kill_cnt_month", allKills);
			form.AddField("death_cnt_month", allDeath);
		}
		if (ProfileController.countGameTotalHit.Value != 0)
		{
			int _accuracy = Mathf.RoundToInt(100f * (float)ProfileController.countGameTotalHit.Value / (float)ProfileController.countGameTotalShoot.Value);
			form.AddField("accuracy", _accuracy);
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			string advertisingId = AndroidSystem.Instance.GetAdvertisingId();
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Android advertising id: " + advertisingId);
			}
			form.AddField("ad_id", advertisingId);
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string advertisingId2 = string.Empty;
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("iOS advertising id: " + advertisingId2);
			}
			form.AddField("ad_id", advertisingId2);
		}
		form.AddField("accessories", GetAccesoriesString());
		Dictionary<string, string> scoresdictOne = new Dictionary<string, string>
		{
			{ "game", "0" },
			{
				"max_score",
				survivalScore.ToString()
			}
		};
		Dictionary<string, string> scoresdictTwo = new Dictionary<string, string>
		{
			{ "game", "1" },
			{
				"max_score",
				coopScore.ToString()
			}
		};
		Dictionary<string, string> scoresdictThree = new Dictionary<string, string>
		{
			{ "game", "2" },
			{
				"max_score",
				Storager.getInt("DaterDayLived", false).ToString()
			}
		};
		string serializedScores = Json.Serialize(new List<Dictionary<string, string>> { scoresdictOne, scoresdictTwo, scoresdictThree });
		form.AddField("scores", serializedScores);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("update_player"));
		form.AddField("coins_bought", Storager.getInt(Defs.AllCurrencyBought + "Coins", false).ToString());
		form.AddField("gems_bought", Storager.getInt(Defs.AllCurrencyBought + "GemsCurrency", false).ToString());
		bool killRateStatisticsWasSent = false;
		try
		{
			bool shouldSendKillRate = true;
			string lastSendKillRateString = PlayerPrefs.GetString(Defs.LastSendKillRateTimeKey, string.Empty);
			DateTime lastSendKillRate;
			if (!string.IsNullOrEmpty(lastSendKillRateString) && DateTime.TryParse(lastSendKillRateString, out lastSendKillRate))
			{
				TimeSpan timeout = TimeSpan.FromHours(20.0);
				shouldSendKillRate = DateTime.UtcNow - lastSendKillRate >= timeout;
			}
			if (shouldSendKillRate)
			{
				Dictionary<string, Dictionary<int, int>>.KeyCollection keysIntersection = KillRateStatisticsManager.WeWereKilledOld.Keys;
				Dictionary<string, Dictionary<int, int>> calculatedStatistics = new Dictionary<string, Dictionary<int, int>>();
				foreach (string weapon in keysIntersection)
				{
					Dictionary<int, int> weKill = ((!KillRateStatisticsManager.WeKillOld.ContainsKey(weapon)) ? new Dictionary<int, int>() : KillRateStatisticsManager.WeKillOld[weapon]);
					Dictionary<int, int> weWereKilled = KillRateStatisticsManager.WeWereKilledOld[weapon];
					if (weKill == null)
					{
						Debug.LogError("Exception adding kill_rate to update_player: weKill == null  " + weapon);
						continue;
					}
					if (weWereKilled == null)
					{
						Debug.LogError("Exception adding kill_rate to update_player: weWereKilled == null  " + weapon);
						continue;
					}
					Dictionary<int, int>.KeyCollection tiersIntersecion = weWereKilled.Keys;
					Dictionary<int, int> calculatedInnerDictionary = new Dictionary<int, int>();
					foreach (int tier in tiersIntersecion)
					{
						int weWereKilledTier = weWereKilled[tier];
						if (weWereKilledTier == 0)
						{
							Debug.LogError("Exception adding kill_rate to update_player: weWereKilledTier == 0 " + weapon);
							continue;
						}
						int result = (weKill.ContainsKey(tier) ? weKill[tier] : 0) * 1000 / weWereKilledTier;
						calculatedInnerDictionary.Add(tier, result);
					}
					WeaponSounds weaponInfo = ItemDb.GetWeaponInfoByPrefabName(weapon);
					if (weaponInfo == null)
					{
						Debug.LogError("Exception adding kill_rate to update_player: weaponInfo == null  " + weapon);
						continue;
					}
					string readableWeaponName = weaponInfo.shopNameNonLocalized;
					if (readableWeaponName == null)
					{
						Debug.LogError("Exception adding kill_rate to update_player: readableWeaponName == null  " + weapon);
						continue;
					}
					string weaponInfoTag = null;
					try
					{
						weaponInfoTag = ItemDb.GetByPrefabName(weaponInfo.name.Replace("(Clone)", string.Empty)).Tag;
					}
					catch (Exception ex)
					{
						Exception e2 = ex;
						if (Application.isEditor)
						{
							Debug.LogWarning("Exception  weaponInfoTag = ItemDb.GetByPrefabName(weaponInfo.name.Replace(\"(Clone)\",\"\")).Tag:  " + e2);
						}
					}
					if (weaponInfoTag == null)
					{
						Debug.LogError("Exception adding kill_rate to update_player: weaponInfo.tag == null  " + weapon);
						continue;
					}
					if (weapon == WeaponManager.SocialGunWN || WeaponManager.GotchaGuns.Contains(weaponInfoTag))
					{
						readableWeaponName = readableWeaponName + "__DPS_TIER_" + (Storager.getInt("RememberedTierWhenObtainGun_" + weapon, false) + 1);
					}
					calculatedStatistics.Add(readableWeaponName, calculatedInnerDictionary);
				}
				if (calculatedStatistics.Count > 0)
				{
					Dictionary<string, object> dictToSent = new Dictionary<string, object>
					{
						{
							"version",
							GlobalGameController.AppVersion
						},
						{ "kill_rate", calculatedStatistics }
					};
					string killRateJson = Json.Serialize(dictToSent);
					form.AddField("kill_rate", killRateJson);
					killRateStatisticsWasSent = true;
					if (Debug.isDebugBuild)
					{
						string modifyLog = string.Format("<color=white>kill_rate: {0}</color>", killRateJson);
						Debug.Log(modifyLog);
					}
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Exception adding kill_rate to update_player: " + e);
		}
		WWW updatePlayerRequest = Tools.CreateWww(actionAddress, form, string.Empty);
		yield return updatePlayerRequest;
		string response = URLs.Sanitize(updatePlayerRequest);
		if (string.IsNullOrEmpty(updatePlayerRequest.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("Update: " + response);
		}
		if (!string.IsNullOrEmpty(updatePlayerRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("Update error: " + updatePlayerRequest.error);
			}
		}
		else if (!string.IsNullOrEmpty(response) && response.Equals("fail"))
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning("Update fail.");
			}
		}
		else if (killRateStatisticsWasSent)
		{
			PlayerPrefs.SetString(Defs.LastSendKillRateTimeKey, DateTime.UtcNow.ToString("s"));
		}
	}

	private IEnumerator GetClanDataLoop()
	{
		while (true)
		{
			if (idle || !SceneLoader.ActiveSceneName.Equals("Clans") || string.IsNullOrEmpty(ClanID))
			{
				yield return null;
				continue;
			}
			StartCoroutine(GetClanDataOnce());
			yield return StartCoroutine(MyWaitForSeconds(20f));
		}
	}

	public IEnumerator MyWaitForSeconds(float tm)
	{
		float startTime = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (Time.realtimeSinceStartup - startTime < tm);
	}

	private void TrySendEventShowBoxProcessFriendsData()
	{
		if (OnShowBoxProcessFriendsData != null)
		{
			OnShowBoxProcessFriendsData();
		}
	}

	private void TrySendEventHideBoxProcessFriendsData()
	{
		if (OnHideBoxProcessFriendsData != null)
		{
			OnHideBoxProcessFriendsData();
		}
	}

	private IEnumerator GetClanDataOnce()
	{
		if (!readyToOperate)
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "get_clan_info");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id_player", id);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("get_clan_info"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			yield break;
		}
		NumberOfClanInfoRequests++;
		try
		{
			yield return download;
		}
		finally
		{
			NumberOfClanInfoRequests--;
		}
		string response = URLs.Sanitize(download);
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetClanDataOnce error: " + download.error);
			}
			yield break;
		}
		if (response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetClanDataOnce fail.");
			}
			yield break;
		}
		int code;
		if (string.IsNullOrEmpty(response) || int.TryParse(response, out code))
		{
			ClearClanData();
			yield break;
		}
		_UpdateClanMembers(response);
		if (FriendsController.ClanUpdated != null)
		{
			FriendsController.ClanUpdated();
		}
	}

	public void ClearClanData()
	{
		ClanID = null;
		clanName = string.Empty;
		clanLogo = string.Empty;
		clanLeaderID = string.Empty;
		clanMembers.Clear();
		ClanSentInvites.Clear();
		clanSentInvitesLocal.Clear();
	}

	private void _UpdateClanMembers(string text)
	{
		object obj2 = Json.Deserialize(text);
		Dictionary<string, object> dictionary = obj2 as Dictionary<string, object>;
		if (dictionary == null)
		{
			if (Application.isEditor || Debug.isDebugBuild)
			{
				Debug.LogWarning(" _UpdateClanMembers dict = null");
			}
			return;
		}
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			switch (item.Key)
			{
			case "info":
			{
				Dictionary<string, object> dictionary2 = item.Value as Dictionary<string, object>;
				if (dictionary2 == null)
				{
					break;
				}
				object value;
				if (dictionary2.TryGetValue("name", out value))
				{
					_prevClanName = clanName;
					clanName = value as string;
					if (!_prevClanName.Equals(clanName) && onChangeClanName != null)
					{
						onChangeClanName(clanName);
					}
				}
				object value2;
				if (dictionary2.TryGetValue("logo", out value2))
				{
					clanLogo = value2 as string;
				}
				object value3;
				if (dictionary2.TryGetValue("creator_id", out value3))
				{
					clanLeaderID = value3 as string;
				}
				break;
			}
			case "players":
			{
				List<object> list2 = item.Value as List<object>;
				if (list2 != null)
				{
					clanMembers.Clear();
					foreach (Dictionary<string, object> item2 in list2)
					{
						Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
						foreach (KeyValuePair<string, object> item3 in item2)
						{
							if (item3.Value is string)
							{
								dictionary4.Add(item3.Key, item3.Value as string);
							}
						}
						clanMembers.Add(dictionary4);
					}
				}
				List<string> toRem__ = new List<string>();
				foreach (string item4 in clanDeletedLocal)
				{
					bool flag = false;
					foreach (Dictionary<string, string> clanMember in clanMembers)
					{
						if (clanMember.ContainsKey("id") && clanMember["id"].Equals(item4))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						toRem__.Add(item4);
					}
				}
				clanDeletedLocal.RemoveAll((string obj) => toRem__.Contains(obj));
				break;
			}
			case "invites":
			{
				ClanSentInvites.Clear();
				List<object> list = item.Value as List<object>;
				if (list == null)
				{
					break;
				}
				foreach (string item5 in list)
				{
					int result;
					if (int.TryParse(item5, out result) && !ClanSentInvites.Contains(result.ToString()))
					{
						ClanSentInvites.Add(result.ToString());
						clanSentInvitesLocal.Remove(result.ToString());
					}
				}
				break;
			}
			}
		}
		List<string> toRem = new List<string>();
		foreach (string item6 in clanCancelledInvitesLocal)
		{
			if (!ClanSentInvites.Contains(item6))
			{
				toRem.Add(item6);
			}
		}
		clanCancelledInvitesLocal.RemoveAll((string obj) => toRem.Contains(obj));
		if (FriendsController.ClanUpdated != null)
		{
			FriendsController.ClanUpdated();
		}
		ClanDataSettted = true;
		if (clanMembers.Count > 3 && clanLeaderID == id)
		{
			AnalyticsStuff.TrySendOnceToFacebook("create_clan_3", null, null);
		}
	}

	private void _UpdateFriends(string text, bool requestAllInfo)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		invitesFromUs.Clear();
		invitesToUs.Clear();
		friends.Clear();
		ClanInvites.Clear();
		friendsDeletedLocal.Clear();
		object obj = Json.Deserialize(text);
		Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
		object value;
		if (dictionary == null)
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning(" _UpdateFriends dict = null");
			}
		}
		else if (dictionary.TryGetValue("friends", out value))
		{
			List<object> list = value as List<object>;
			if (list == null)
			{
				if (Application.isEditor || Debug.isDebugBuild)
				{
					Debug.LogWarning(" _UpdateFriends __list = null");
				}
				return;
			}
			_ProcessFriendsList(list, requestAllInfo);
			object value2;
			if (dictionary.TryGetValue("clans_invites", out value2))
			{
				List<object> list2 = value2 as List<object>;
				if (list2 == null)
				{
					if (Application.isEditor || Debug.isDebugBuild)
					{
						Debug.LogWarning(" _UpdateFriends clanInv = null");
					}
				}
				else
				{
					_ProcessClanInvitesList(list2);
				}
			}
			else
			{
				Debug.LogWarning(" _UpdateFriends clanInvObj!");
			}
		}
		else
		{
			Debug.LogWarning(" _UpdateFriends friendsObj!");
		}
	}

	private void _ProcessClanInvitesList(List<object> clanInv)
	{
		List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
		foreach (Dictionary<string, object> item in clanInv)
		{
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> item2 in item)
			{
				dictionary2.Add(item2.Key, item2.Value as string);
			}
			list.Add(dictionary2);
		}
		ClanInvites.Clear();
		ClanInvites = list;
	}

	private void _ProcessFriendsList(List<object> __list, bool requestAllInfo)
	{
		List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
		foreach (Dictionary<string, object> item in __list)
		{
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> item2 in item)
			{
				dictionary2.Add(item2.Key, item2.Value as string);
			}
			list.Add(dictionary2);
		}
		foreach (Dictionary<string, string> item3 in list)
		{
			Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
			if (item3["whom"].Equals(id) && item3["status"].Equals("0"))
			{
				foreach (string key in item3.Keys)
				{
					if (!key.Equals("whom") && !key.Equals("status"))
					{
						try
						{
							dictionary3.Add((!key.Equals("who")) ? key : "friend", item3[key]);
						}
						catch
						{
						}
					}
				}
				invitesToUs.Add(dictionary3["friend"]);
				notShowAddIds.Remove(item3["who"]);
			}
			if (!item3["status"].Equals("1"))
			{
				continue;
			}
			string text = ((!item3["who"].Equals(id)) ? "whom" : "who");
			string text2 = ((!text.Equals("who")) ? "who" : "whom");
			foreach (string key2 in item3.Keys)
			{
				if (!key2.Equals(text) && !key2.Equals("status"))
				{
					dictionary3.Add((!key2.Equals(text2)) ? key2 : "friend", item3[key2]);
				}
			}
			friends.Add(dictionary3["friend"]);
			notShowAddIds.Remove(item3[text2]);
		}
		if (requestAllInfo)
		{
			UpdatePLayersInfo();
		}
		else
		{
			_UpdatePlayersInfo();
		}
	}

	private void _UpdatePlayersInfo()
	{
		List<string> list = new List<string>();
		list.AddRange(friends);
		list.AddRange(invitesToUs);
		if (list.Count > 0)
		{
			StartCoroutine(GetInfoAboutNPlayers(list));
		}
	}

	private IEnumerator GetInfoAboutNPlayers()
	{
		List<string> allFriends = new List<string>();
		allFriends.AddRange(friends);
		allFriends.AddRange(invitesToUs);
		if (allFriends.Count != 0)
		{
			yield return StartCoroutine(GetInfoAboutNPlayers(allFriends));
		}
	}

	public void GetInfoAboutPlayers(List<string> ids)
	{
		StartCoroutine(GetInfoAboutNPlayers(ids));
	}

	public IEnumerator GetInfoAboutNPlayers(List<string> ids)
	{
		string json = Json.Serialize(ids);
		if (json == null)
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "get_all_short_info_by_id");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("ids", json);
		form.AddField("id", id);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("get_all_short_info_by_id"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		TrySendEventHideBoxProcessFriendsData();
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetInfoAboutNPlayers error: " + download.error);
			}
			yield break;
		}
		if (response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetInfoAboutNPlayers fail.");
			}
			yield break;
		}
		Dictionary<string, object> __list = Json.Deserialize(response) as Dictionary<string, object>;
		if (__list == null)
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning(" GetInfoAboutNPlayers info = null");
			}
			yield break;
		}
		Dictionary<string, Dictionary<string, object>> list = new Dictionary<string, Dictionary<string, object>>();
		foreach (string key2 in __list.Keys)
		{
			Dictionary<string, object> d2 = __list[key2] as Dictionary<string, object>;
			Dictionary<string, object> newd = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> kvp in d2)
			{
				newd.Add(kvp.Key, kvp.Value);
			}
			list.Add(key2, newd);
		}
		foreach (string key in list.Keys)
		{
			Dictionary<string, object> d = list[key];
			bool _isAdd = false;
			if (friends.Contains(key))
			{
				_isAdd = true;
				if (friendsInfo.ContainsKey(key))
				{
					friendsInfo[key] = d;
				}
				else
				{
					friendsInfo.Add(key, d);
				}
			}
			if (!_isAdd)
			{
				if (profileInfo.ContainsKey(key))
				{
					profileInfo[key] = d;
				}
				else
				{
					profileInfo.Add(key, d);
				}
				if (!sharedController.id.Equals(key) && FindFriendsFromLocalLAN.lanPlayerInfo.Contains(key) && !getPossibleFriendsResult.ContainsKey(key))
				{
					getPossibleFriendsResult.Add(key, PossiblleOrigin.Local);
				}
			}
			if (playersInfo.ContainsKey(key))
			{
				playersInfo[key] = d;
			}
			else
			{
				playersInfo.Add(key, d);
			}
		}
		isUpdateInfoAboutAllFriends = false;
		if (FriendsController.FriendsUpdated != null)
		{
			FriendsController.FriendsUpdated();
		}
		SaveCurrentState();
	}

	public void UpdatePLayersInfo()
	{
		if (readyToOperate)
		{
			StartCoroutine(GetInfoAboutNPlayers());
		}
	}

	public void StartRefreshingInfo(string playerId)
	{
		if (readyToOperate)
		{
			_shouldStopRefreshingInfo = false;
			StartCoroutine(GetIfnoAboutPlayerLoop(playerId));
		}
	}

	public void StopRefreshingInfo()
	{
		if (readyToOperate)
		{
			_shouldStopRefreshingInfo = true;
		}
	}

	private IEnumerator GetIfnoAboutPlayerLoop(string playerId)
	{
		while (true)
		{
			if (idle)
			{
				yield return null;
				continue;
			}
			StartCoroutine(UpdatePlayerInfoById(playerId));
			float startTime = Time.realtimeSinceStartup;
			do
			{
				yield return null;
			}
			while (Time.realtimeSinceStartup - startTime < 20f && !_shouldStopRefreshingInfo);
			if (!_shouldStopRefreshingInfo)
			{
				continue;
			}
			break;
		}
	}

	public IEnumerator GetInfoByIdCoroutine(string playerId)
	{
		getInfoPlayerResult = null;
		if (string.IsNullOrEmpty(playerId))
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "get_info_by_id");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id", playerId);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("get_info_by_id"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			yield break;
		}
		NumberOffFullInfoRequests++;
		try
		{
			yield return download;
		}
		finally
		{
			NumberOffFullInfoRequests--;
		}
		string response = URLs.Sanitize(download);
		if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("Info for id " + playerId + ": " + response);
		}
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetInfoById error: " + download.error);
			}
			yield break;
		}
		if (response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetInfoById fail.");
			}
			yield break;
		}
		Dictionary<string, object> responseData = ParseInfo(response);
		if (responseData == null)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning(" GetInfoById newInfo = null");
			}
		}
		else
		{
			getInfoPlayerResult = responseData;
		}
	}

	public IEnumerator GetInfoByParamCoroutine(string param)
	{
		findPlayersByParamResult = null;
		if (string.IsNullOrEmpty(param))
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "get_users_info_by_param");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("param", param);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("get_users_info_by_param"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			yield break;
		}
		TrySendEventShowBoxProcessFriendsData();
		yield return download;
		TrySendEventHideBoxProcessFriendsData();
		string response = URLs.Sanitize(download);
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetInfoById error: " + download.error);
			}
			yield break;
		}
		if (response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetInfoById fail.");
			}
			yield break;
		}
		if (!string.IsNullOrEmpty(download.error) || string.IsNullOrEmpty(response) || Debug.isDebugBuild)
		{
		}
		List<object> responseData = Json.Deserialize(response) as List<object>;
		if (responseData == null)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning(" GetInfoByParam newInfo = null");
			}
		}
		else
		{
			if (responseData == null || responseData.Count <= 0)
			{
				yield break;
			}
			findPlayersByParamResult = new List<string>();
			foreach (object _playerInfoObj in responseData)
			{
				Dictionary<string, object> _playerInfoDict = _playerInfoObj as Dictionary<string, object>;
				string _id = Convert.ToString(_playerInfoDict["id"]);
				findPlayersByParamResult.Add(_id);
				if (profileInfo.ContainsKey(_id))
				{
					profileInfo[_id]["player"] = _playerInfoDict;
					continue;
				}
				Dictionary<string, object> _infoPlayer = new Dictionary<string, object> { { "player", _playerInfoDict } };
				profileInfo.Add(_id, _infoPlayer);
			}
		}
	}

	private IEnumerator UpdatePlayerInfoById(string playerId)
	{
		yield return StartCoroutine(GetInfoByIdCoroutine(playerId));
		if (getInfoPlayerResult == null)
		{
			yield break;
		}
		playersInfo[playerId] = getInfoPlayerResult;
		bool _addInfo = false;
		if (friends.Contains(playerId))
		{
			_addInfo = true;
			if (friendsInfo.ContainsKey(playerId))
			{
				friendsInfo[playerId] = getInfoPlayerResult;
			}
			else
			{
				friendsInfo.Add(playerId, getInfoPlayerResult);
			}
		}
		if (clanFriendsInfo.ContainsKey(playerId))
		{
			clanFriendsInfo[playerId] = getInfoPlayerResult;
			_addInfo = true;
		}
		if (!_addInfo)
		{
			if (profileInfo.ContainsKey(playerId))
			{
				profileInfo[playerId] = getInfoPlayerResult;
			}
			else
			{
				profileInfo.Add(playerId, getInfoPlayerResult);
			}
		}
		if (playersInfo.ContainsKey(playerId))
		{
			playersInfo[playerId] = getInfoPlayerResult;
		}
		else
		{
			playersInfo.Add(playerId, getInfoPlayerResult);
		}
		if (FriendsController.FullInfoUpdated != null)
		{
			FriendsController.FullInfoUpdated();
		}
	}

	private Dictionary<string, object> ParseInfo(string info)
	{
		return Json.Deserialize(info) as Dictionary<string, object>;
	}

	public IEnumerator FriendRequest(string personId, Dictionary<string, object> socialEventParameters, Action<bool, bool> callbackAnswer = null)
	{
		if (socialEventParameters == null)
		{
			throw new ArgumentNullException("socialEventParameters");
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "friend_request");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id", id);
		form.AddField("whom", personId);
		form.AddField("type", 0);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("friend_request"));
		WWW friendRequest = Tools.CreateWww(actionAddress, form, string.Empty);
		TrySendEventShowBoxProcessFriendsData();
		yield return friendRequest;
		string response = URLs.Sanitize(friendRequest);
		TrySendEventHideBoxProcessFriendsData();
		if (string.IsNullOrEmpty(friendRequest.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("Friend request: " + response);
		}
		bool isCallbackAnswerRecive = false;
		if (!string.IsNullOrEmpty(friendRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("FriendRequest error: " + friendRequest.error);
			}
			if (callbackAnswer != null)
			{
				callbackAnswer(false, false);
				isCallbackAnswerRecive = true;
			}
		}
		else if (response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("FriendRequest fail.");
			}
			if (callbackAnswer != null)
			{
				callbackAnswer(false, false);
				isCallbackAnswerRecive = true;
			}
		}
		if (response.Equals("ok"))
		{
			TutorialQuestManager.Instance.AddFulfilledQuest("addFriend");
			QuestMediator.NotifySocialInteraction("addFriend");
			if (invitesToUs.Contains(personId))
			{
				invitesToUs.Remove(personId);
				friends.Add(personId);
			}
			else
			{
				invitesFromUs.Add(personId);
			}
			AnalyticsFacade.SendCustomEvent("Social", socialEventParameters);
		}
		if (callbackAnswer != null && !isCallbackAnswerRecive)
		{
			callbackAnswer(true, response.Equals("exist"));
		}
	}

	private IEnumerator _SendCreateClan(string personId, string nameClan, string skinClan, Action<string> ErrorHandler)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "create_clan");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id", personId);
		string filteredNick = nameClan;
		filteredNick = FilterBadWorld.FilterString(nameClan);
		form.AddField("name", filteredNick);
		form.AddField("logo", skinClan);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("create_clan"));
		WWW download2 = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download2 == null)
		{
			if (ErrorHandler != null)
			{
				ErrorHandler("Request skipped.");
			}
			yield break;
		}
		NumberOfCreateClanRequests++;
		float tm = Time.realtimeSinceStartup;
		try
		{
			while (!download2.isDone && string.IsNullOrEmpty(download2.error) && Time.realtimeSinceStartup - tm < 25f)
			{
				yield return null;
			}
		}
		finally
		{
			NumberOfCreateClanRequests--;
		}
		bool timeout = !download2.isDone && string.IsNullOrEmpty(download2.error) && Time.realtimeSinceStartup - tm >= 25f;
		string response = ((!timeout) ? URLs.Sanitize(download2) : string.Empty);
		if (!timeout && string.IsNullOrEmpty(download2.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("_SendCreateClan request: " + response);
		}
		int newClanID;
		if (timeout || !string.IsNullOrEmpty(download2.error))
		{
			string errorMessage = ((!timeout) ? download2.error : "TIMEOUT");
			if (ErrorHandler != null)
			{
				ErrorHandler(errorMessage);
			}
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_SendCreateClan error: " + errorMessage);
			}
			if (timeout)
			{
				download2.Dispose();
				download2 = null;
			}
		}
		else if ("fail".Equals(response))
		{
			if (ErrorHandler != null)
			{
				ErrorHandler("fail");
			}
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_SendCreateClan fail.");
			}
		}
		else if (int.TryParse(response, out newClanID))
		{
			if (newClanID != -1)
			{
				ClanID = newClanID.ToString();
			}
			if (this.ReturnNewIDClan != null)
			{
				this.ReturnNewIDClan(newClanID);
			}
		}
	}

	public void ExitClan(string who = null)
	{
		if (readyToOperate)
		{
			StartCoroutine(_ExitClan(who));
		}
	}

	private IEnumerator _ExitClan(string who)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "exit_clan");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id_player", who ?? id);
		form.AddField("id_clan", ClanID);
		form.AddField("id", id);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("exit_clan"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("_ExitClan: " + response);
		}
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_ExitClan error: " + download.error);
			}
		}
		else if ("fail".Equals(response) && Debug.isDebugBuild)
		{
			Debug.LogWarning("_ExitClan fail.");
		}
	}

	public void DeleteClan()
	{
		if (readyToOperate && ClanID != null)
		{
			StartCoroutine(_DeleteClan());
		}
	}

	private IEnumerator _DeleteClan()
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "delete_clan");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id_clan", ClanID);
		form.AddField("id", id);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("delete_clan"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("_DeleteClan: " + response);
		}
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_DeleteClan error: " + download.error);
			}
		}
		else if ("fail".Equals(response) && Debug.isDebugBuild)
		{
			Debug.LogWarning("_DeleteClan fail.");
		}
	}

	private IEnumerator SendClanInvitation(string personID, Action<bool, bool> callbackResult = null)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "invite_to_clan");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id_player", personID);
		form.AddField("id_clan", ClanID);
		form.AddField("id", id);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("invite_to_clan"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			if (callbackResult != null)
			{
				callbackResult(false, false);
			}
			clanSentInvitesLocal.Remove(personID);
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("SendClanInvitation: " + response);
		}
		bool isCallbackCall = false;
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("SendClanInvitation error: " + download.error);
			}
			if (callbackResult != null)
			{
				isCallbackCall = true;
				callbackResult(false, false);
			}
			clanSentInvitesLocal.Remove(personID);
		}
		else if ("fail".Equals(response))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("SendClanInvitation fail.");
			}
			if (callbackResult != null)
			{
				isCallbackCall = true;
				callbackResult(false, false);
			}
			clanSentInvitesLocal.Remove(personID);
		}
		if (response.Equals("ok") && !ClanSentInvites.Contains(personID))
		{
			ClanSentInvites.Add(personID);
		}
		if (callbackResult != null && !isCallbackCall)
		{
			callbackResult(true, response.Equals("exist"));
		}
	}

	private IEnumerator AcceptFriend(string accepteeId, Action<bool> action = null)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "accept_friend");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("player_id", id);
		form.AddField("acceptee_id", accepteeId);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("accept_friend"));
		WWW acceptFriendRequest = Tools.CreateWww(actionAddress, form, string.Empty);
		TrySendEventShowBoxProcessFriendsData();
		yield return acceptFriendRequest;
		string response = URLs.Sanitize(acceptFriendRequest);
		TrySendEventHideBoxProcessFriendsData();
		if (!string.IsNullOrEmpty(acceptFriendRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("AcceptFriend error: " + acceptFriendRequest.error);
			}
			if (action != null)
			{
				action(false);
			}
		}
		else if ("fail".Equals(response))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("AcceptFriend fail.");
			}
			if (action != null)
			{
				action(false);
			}
		}
		else if (string.IsNullOrEmpty(acceptFriendRequest.error) && !string.IsNullOrEmpty(response))
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Accept friend: " + response);
			}
			if (invitesToUs.Contains(accepteeId))
			{
				invitesToUs.Remove(accepteeId);
				FriendsWindowController.Instance.UpdateCurrentFriendsArrayAndItems();
			}
			if (!friends.Contains(accepteeId))
			{
				friends.Add(accepteeId);
			}
			QuestMediator.NotifySocialInteraction("addFriend");
			if (action != null)
			{
				action(true);
			}
		}
	}

	public static void DeleteFriend(string rejecteeId, Action<bool> action = null)
	{
		if (!(sharedController == null) && readyToOperate)
		{
			sharedController.StartCoroutine(sharedController.DeleteFriendCoroutine(rejecteeId, action));
		}
	}

	private IEnumerator DeleteFriendCoroutine(string rejecteeId, Action<bool> action = null)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "reject_friendship");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("rejectee_id", rejecteeId);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("reject_friendship"));
		WWW rejectFriendshipRequest = Tools.CreateWww(actionAddress, form, string.Empty);
		yield return rejectFriendshipRequest;
		string response = URLs.Sanitize(rejectFriendshipRequest);
		if (!string.IsNullOrEmpty(rejectFriendshipRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("Reject_friendship error: " + rejectFriendshipRequest.error);
			}
			if (action != null)
			{
				action(false);
			}
		}
		else if ("fail".Equals(response))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("Reject_friendship fail.");
			}
			if (action != null)
			{
				action(false);
			}
		}
		else if (string.IsNullOrEmpty(rejectFriendshipRequest.error) && !string.IsNullOrEmpty(response))
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("reject_friendship: " + response);
			}
			if (friends.Contains(rejecteeId))
			{
				friends.Remove(rejecteeId);
				FriendsWindowController.Instance.UpdateCurrentFriendsArrayAndItems();
			}
			if (action != null)
			{
				action(true);
			}
		}
	}

	public void RejectInvite(string rejecteeId, Action<bool> action = null)
	{
		if (readyToOperate)
		{
			StartCoroutine(RejectInviteFriendCoroutine(rejecteeId, action));
		}
	}

	public void RejectClanInvite(string clanID, string playerID = null)
	{
		if (!string.IsNullOrEmpty(clanID) && readyToOperate)
		{
			StartCoroutine(_RejectClanInvite(clanID, playerID));
		}
	}

	private IEnumerator RejectInviteFriendCoroutine(string rejecteeId, Action<bool> action = null)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "reject_invite_friendship");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("rejectee_id", rejecteeId);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("reject_invite_friendship"));
		WWW rejectInviteFriendshipRequest = Tools.CreateWww(actionAddress, form, string.Empty);
		TrySendEventShowBoxProcessFriendsData();
		yield return rejectInviteFriendshipRequest;
		TrySendEventHideBoxProcessFriendsData();
		string response = URLs.Sanitize(rejectInviteFriendshipRequest);
		if (!string.IsNullOrEmpty(rejectInviteFriendshipRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("RejectFriend error: " + rejectInviteFriendshipRequest.error);
			}
			if (action != null)
			{
				action(false);
			}
		}
		else if ("fail".Equals(response))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("RejectFriend fail.");
			}
			if (action != null)
			{
				action(false);
			}
		}
		else if (string.IsNullOrEmpty(rejectInviteFriendshipRequest.error) && !string.IsNullOrEmpty(response))
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Reject friend: " + response);
			}
			if (invitesToUs.Contains(rejecteeId))
			{
				invitesToUs.Remove(rejecteeId);
				FriendsWindowController.Instance.UpdateCurrentFriendsArrayAndItems();
			}
			if (action != null)
			{
				action(true);
			}
		}
	}

	private IEnumerator _RejectClanInvite(string clanID, string playerID)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "reject_invite");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id_player", playerID ?? id);
		form.AddField("id_clan", clanID);
		form.AddField("id", id);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("reject_invite"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			clanCancelledInvitesLocal.Remove(playerID);
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_RejectClanInvite error: " + download.error);
			}
			clanCancelledInvitesLocal.Remove(playerID);
		}
		else if ("fail".Equals(response))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_RejectClanInvite fail.");
			}
			clanCancelledInvitesLocal.Remove(playerID);
		}
		if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("_RejectClanInvite: " + response);
		}
	}

	public void DeleteClanMember(string memebrID)
	{
		if (!string.IsNullOrEmpty(memebrID) && readyToOperate)
		{
			StartCoroutine(_DeleteClanMember(memebrID));
		}
	}

	private IEnumerator _DeleteClanMember(string memberID)
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "delete_clan_member");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id_player", memberID);
		form.AddField("id_clan", ClanID);
		form.AddField("id", id);
		form.AddField("uniq_id", sharedController.id);
		form.AddField("auth", Hash("delete_clan_member"));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			clanDeletedLocal.Remove(memberID);
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_DeleteClanMember error: " + download.error);
			}
			clanDeletedLocal.Remove(memberID);
		}
		else if ("fail".Equals(response))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("_DeleteClanMember fail.");
			}
			clanDeletedLocal.Remove(memberID);
		}
		if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("_DeleteClanMember: " + response);
		}
	}

	private void Update()
	{
		if (isUpdateServerTimeAfterRun)
		{
			tickForServerTime += Time.unscaledDeltaTime;
			if (tickForServerTime >= 1f)
			{
				localServerTime++;
				tickForServerTime -= 1f;
			}
		}
		if (!firstUpdateAfterApplicationPause)
		{
			deltaTimeInGame += Time.unscaledDeltaTime;
		}
		else
		{
			firstUpdateAfterApplicationPause = false;
		}
		if (Banned == 1 && PhotonNetwork.connected)
		{
			PhotonNetwork.isMessageQueueRunning = true;
			PhotonNetwork.Disconnect();
		}
		if (Input.touchCount > 0)
		{
			if (Time.realtimeSinceStartup - lastTouchTm > 30f)
			{
				idle = true;
			}
		}
		else
		{
			lastTouchTm = Time.realtimeSinceStartup;
			idle = false;
		}
	}

	private string GetJsonIdsFacebookFriends()
	{
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("Start GetJsonIdsFacebookFriends");
		}
		FacebookController facebookController = FacebookController.sharedController;
		if (facebookController == null)
		{
			return "[]";
		}
		if (facebookController.friendsList == null || facebookController.friendsList.Count == 0)
		{
			return "[]";
		}
		List<string> list = new List<string>();
		for (int i = 0; i < facebookController.friendsList.Count; i++)
		{
			Friend friend = facebookController.friendsList[i];
			list.Add(friend.id);
		}
		string text = Json.Serialize(list);
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("GetJsonIdsFacebookFriends: " + text);
		}
		return text;
	}

	private IEnumerator GetPossibleFriendsList(int playerLevel, int platformId, string clientVersion)
	{
		WWWForm wwwForm = new WWWForm();
		string requestName = "possible_friends_list";
		string appVersion = string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion);
		wwwForm.AddField("action", requestName);
		wwwForm.AddField("app_version", appVersion);
		wwwForm.AddField("uniq_id", id);
		wwwForm.AddField("auth", Hash(requestName));
		if (FindFriendsFromLocalLAN.lanPlayerInfo.Count > 0)
		{
			wwwForm.AddField("local_ids", Json.Serialize(FindFriendsFromLocalLAN.lanPlayerInfo));
		}
		string facebookFriendsJsonIds = GetJsonIdsFacebookFriends();
		wwwForm.AddField("ids", facebookFriendsJsonIds);
		wwwForm.AddField("rank", playerLevel.ToString());
		wwwForm.AddField("platform_id", platformId.ToString());
		wwwForm.AddField("version", clientVersion);
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, wwwForm, string.Empty);
		if (download == null)
		{
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetPossibleFriendsList error: " + download.error);
			}
			yield break;
		}
		if (response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetPossibleFriendsList fail.");
			}
			yield break;
		}
		Dictionary<string, object> dataList = Json.Deserialize(response) as Dictionary<string, object>;
		if (dataList == null)
		{
			yield break;
		}
		getPossibleFriendsResult.Clear();
		if (dataList.ContainsKey("local_users"))
		{
			List<object> userList2 = dataList["local_users"] as List<object>;
			if (userList2 != null && userList2.Count > 0)
			{
				foreach (Dictionary<string, object> dictListItem3 in userList2.OfType<Dictionary<string, object>>())
				{
					Dictionary<string, object> clone3 = new Dictionary<string, object>();
					foreach (KeyValuePair<string, object> dictItem3 in dictListItem3)
					{
						clone3.Add(dictItem3.Key, dictItem3.Value);
					}
					string userId3 = Convert.ToString(clone3["id"]);
					if (profileInfo.ContainsKey(userId3))
					{
						Dictionary<string, object> _cache6 = profileInfo[userId3]["player"] as Dictionary<string, object>;
						_cache6["nick"] = clone3["nick"];
						_cache6["rank"] = clone3["rank"];
						_cache6["clan_logo"] = clone3["clan_logo"];
						_cache6["clan_name"] = clone3["clan_name"];
						_cache6["skin"] = clone3["skin"];
						profileInfo[userId3]["player"] = _cache6;
					}
					else
					{
						Dictionary<string, object> _cache6 = new Dictionary<string, object>
						{
							{
								"nick",
								clone3["nick"]
							},
							{
								"rank",
								clone3["rank"]
							},
							{
								"clan_logo",
								clone3["clan_logo"]
							},
							{
								"clan_name",
								clone3["clan_name"]
							},
							{
								"skin",
								clone3["skin"]
							}
						};
						Dictionary<string, object> _infoPlayer3 = new Dictionary<string, object> { { "player", _cache6 } };
						profileInfo.Add(userId3, _infoPlayer3);
					}
					if (!getPossibleFriendsResult.ContainsKey(userId3) && !friends.Contains(userId3))
					{
						getPossibleFriendsResult.Add(userId3, PossiblleOrigin.Local);
					}
				}
			}
		}
		if (dataList.ContainsKey("facebook_users"))
		{
			List<object> facebookUsers = dataList["facebook_users"] as List<object>;
			if (facebookUsers != null && facebookUsers.Count > 0)
			{
				foreach (Dictionary<string, object> dictListItem2 in facebookUsers.OfType<Dictionary<string, object>>())
				{
					string userId2 = Convert.ToString(dictListItem2["id"]);
					if (IsPlayerOurFriend(userId2))
					{
						continue;
					}
					Dictionary<string, object> clone2 = new Dictionary<string, object>();
					foreach (KeyValuePair<string, object> dictItem2 in dictListItem2)
					{
						clone2.Add(dictItem2.Key, dictItem2.Value);
					}
					if (profileInfo.ContainsKey(userId2))
					{
						Dictionary<string, object> _cache4 = profileInfo[userId2]["player"] as Dictionary<string, object>;
						_cache4["nick"] = clone2["nick"];
						_cache4["rank"] = clone2["rank"];
						_cache4["clan_logo"] = clone2["clan_logo"];
						_cache4["clan_name"] = clone2["clan_name"];
						_cache4["skin"] = clone2["skin"];
						profileInfo[userId2]["player"] = _cache4;
					}
					else
					{
						Dictionary<string, object> _cache4 = new Dictionary<string, object>
						{
							{
								"nick",
								clone2["nick"]
							},
							{
								"rank",
								clone2["rank"]
							},
							{
								"clan_logo",
								clone2["clan_logo"]
							},
							{
								"clan_name",
								clone2["clan_name"]
							},
							{
								"skin",
								clone2["skin"]
							}
						};
						Dictionary<string, object> _infoPlayer2 = new Dictionary<string, object> { { "player", _cache4 } };
						profileInfo.Add(userId2, _infoPlayer2);
					}
					if (!getPossibleFriendsResult.ContainsKey(userId2) && !friends.Contains(userId2))
					{
						getPossibleFriendsResult.Add(userId2, PossiblleOrigin.Facebook);
					}
				}
			}
		}
		if (dataList.ContainsKey("users"))
		{
			List<object> userList = dataList["users"] as List<object>;
			if (userList != null && userList.Count > 0)
			{
				foreach (Dictionary<string, object> dictListItem in userList.OfType<Dictionary<string, object>>())
				{
					Dictionary<string, object> clone = new Dictionary<string, object>();
					foreach (KeyValuePair<string, object> dictItem in dictListItem)
					{
						clone.Add(dictItem.Key, dictItem.Value);
					}
					string userId = Convert.ToString(clone["id"]);
					if (profileInfo.ContainsKey(userId))
					{
						Dictionary<string, object> _cache2 = profileInfo[userId]["player"] as Dictionary<string, object>;
						_cache2["nick"] = clone["nick"];
						_cache2["rank"] = clone["rank"];
						_cache2["clan_logo"] = clone["clan_logo"];
						_cache2["clan_name"] = clone["clan_name"];
						_cache2["skin"] = clone["skin"];
						profileInfo[userId]["player"] = _cache2;
					}
					else
					{
						Dictionary<string, object> _cache2 = new Dictionary<string, object>
						{
							{
								"nick",
								clone["nick"]
							},
							{
								"rank",
								clone["rank"]
							},
							{
								"clan_logo",
								clone["clan_logo"]
							},
							{
								"clan_name",
								clone["clan_name"]
							},
							{
								"skin",
								clone["skin"]
							}
						};
						Dictionary<string, object> _infoPlayer = new Dictionary<string, object> { { "player", _cache2 } };
						profileInfo.Add(userId, _infoPlayer);
					}
					if (!getPossibleFriendsResult.ContainsKey(userId) && !friends.Contains(userId))
					{
						getPossibleFriendsResult.Add(userId, PossiblleOrigin.RandomPlayer);
					}
				}
			}
		}
		if (FriendsController.FriendsUpdated != null)
		{
			FriendsController.FriendsUpdated();
		}
	}

	public void DownloadDataAboutPossibleFriends()
	{
		int currentLevel = ExperienceController.GetCurrentLevel();
		int myPlatformConnect = (int)ConnectSceneNGUIController.myPlatformConnect;
		string multiplayerProtocolVersion = GlobalGameController.MultiplayerProtocolVersion;
		StartCoroutine(GetPossibleFriendsList(currentLevel, myPlatformConnect, multiplayerProtocolVersion));
	}

	private IEnumerator ClearAllFriendsInvitesCoroutine()
	{
		WWWForm wwwForm = new WWWForm();
		string requestName = "delete_friend_invites";
		string appVersion = string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion);
		wwwForm.AddField("action", requestName);
		wwwForm.AddField("app_version", appVersion);
		wwwForm.AddField("uniq_id", id);
		wwwForm.AddField("auth", Hash(requestName));
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, wwwForm, string.Empty);
		if (download == null)
		{
			yield break;
		}
		TrySendEventShowBoxProcessFriendsData();
		yield return download;
		TrySendEventHideBoxProcessFriendsData();
		if (FriendsWindowController.Instance != null)
		{
			FriendsWindowController.Instance.statusBar.clearAllInviteButton.isEnabled = true;
		}
		string response = URLs.Sanitize(download);
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("ClearAllFriendsInvites error: " + download.error);
			}
		}
		else if (response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("ClearAllFriendsInvites fail.");
			}
		}
		else if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response))
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("ClearAllFriendsInvites: " + response);
			}
			if (invitesToUs != null)
			{
				invitesToUs.Clear();
			}
			if (FriendsController.FriendsUpdated != null)
			{
				FriendsController.FriendsUpdated();
			}
		}
	}

	public void GetFriendsData(bool _isUpdateInfoAboutAllFriends = false)
	{
		timerUpdateFriend = -1f;
		if (_isUpdateInfoAboutAllFriends)
		{
			isUpdateInfoAboutAllFriends = true;
		}
	}

	private IEnumerator SendGameTimeLoop()
	{
		float timerSendTimeGame = 30f;
		while (true)
		{
			if (readyToOperate && !idle && !string.IsNullOrEmpty(sharedController.id))
			{
				while (timerSendTimeGame > 0f)
				{
					timerSendTimeGame = ((PhotonNetwork.room != null) ? 30f : (timerSendTimeGame - Time.unscaledDeltaTime));
					yield return null;
				}
				yield return StartCoroutine(SendGameTime());
				timerSendTimeGame = 30f;
			}
			else
			{
				yield return null;
			}
		}
	}

	private IEnumerator SendGameTime()
	{
		WWWForm form = new WWWForm();
		form.AddField("action", "update_game_time");
		form.AddField("auth", Hash("update_game_time"));
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("uniq_id", sharedController.id);
		sendingTime = Mathf.RoundToInt(deltaTimeInGame);
		form.AddField("game_time", Mathf.RoundToInt(sendingTime));
		form.AddField("tier", ExpController.OurTierForAnyPlace());
		form.AddField("platform", ProtocolListGetter.CurrentPlatform.ToString());
		if (Defs.abTestBalansCohort != 0 && Defs.abTestBalansCohort != Defs.ABTestCohortsType.SKIP)
		{
			form.AddField("cohortName", Defs.abTestBalansCohortName);
		}
		if (Defs.cohortABTestAdvert == Defs.ABTestCohortsType.A || Defs.cohortABTestAdvert == Defs.ABTestCohortsType.B)
		{
			string _currentConfigAdvertNameForEvent = ((Defs.cohortABTestAdvert != Defs.ABTestCohortsType.A) ? "AdvertB_" : "AdvertA_") + configNameABTestAdvert;
			form.AddField("cohort_ad", _currentConfigAdvertNameForEvent);
		}
		foreach (ABTestBase abtest in ABTestController.currentABTests)
		{
			if (abtest.cohort == ABTestController.ABTestCohortsType.A || abtest.cohort == ABTestController.ABTestCohortsType.B)
			{
				form.AddField(abtest.currentFolder, abtest.cohortName);
			}
		}
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("Send delta time: " + sendingTime);
		}
		WWW updateGameTimeRequest = Tools.CreateWww(actionAddress, form, string.Empty);
		yield return updateGameTimeRequest;
		string response = URLs.Sanitize(updateGameTimeRequest);
		if (!string.IsNullOrEmpty(updateGameTimeRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("updateGameTimeRequest error: " + updateGameTimeRequest.error);
			}
			yield break;
		}
		if (!string.IsNullOrEmpty(response) && response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("updateGameTimeRequest fail.");
			}
			yield break;
		}
		deltaTimeInGame -= sendingTime;
		if (!string.IsNullOrEmpty(response) && !response.Equals("ok"))
		{
			Dictionary<string, object> cacheResponse = Json.Deserialize(response) as Dictionary<string, object>;
			if (cacheResponse != null && cacheResponse.ContainsKey("fight_invites"))
			{
				ParseFightInvite(cacheResponse["fight_invites"] as List<object>);
			}
		}
	}

	private IEnumerator GetFriendsDataLoop()
	{
		while (true)
		{
			if (readyToOperate && !idle && !string.IsNullOrEmpty(sharedController.id) && TrainingController.TrainingCompleted)
			{
				yield return StartCoroutine(UpdateFriendsInfo(isUpdateInfoAboutAllFriends));
				while (timerUpdateFriend > 0f)
				{
					if (FriendsWindowGUI.Instance != null && FriendsWindowGUI.Instance.InterfaceEnabled)
					{
						timerUpdateFriend -= Time.unscaledDeltaTime;
					}
					yield return null;
				}
				timerUpdateFriend = Defs.timeUpdateFriendInfo;
			}
			else
			{
				yield return null;
			}
		}
	}

	private IEnumerator UpdateFriendsInfo(bool _isUpdateInfoAboutAllFriends)
	{
		WWWForm wwwForm = new WWWForm();
		string requestName = "update_friends_info";
		string appVersion = string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion);
		wwwForm.AddField("action", requestName);
		wwwForm.AddField("app_version", appVersion);
		wwwForm.AddField("uniq_id", id);
		wwwForm.AddField("auth", Hash(requestName));
		bool isFromFromFriendsScene = FriendsWindowGUI.Instance != null && FriendsWindowGUI.Instance.InterfaceEnabled;
		wwwForm.AddField("from_friends", isFromFromFriendsScene ? 1 : 0);
		if (FriendsWindowController.IsActiveFriendListTab() && friends.Count > 0)
		{
			string json = Json.Serialize(friends);
			if (json != null)
			{
				wwwForm.AddField("get_all_players_online", json);
			}
		}
		wwwForm.AddField("private_messages", ChatController.GetPrivateChatJsonForSend());
		WWW updateFriendsInfoRequest = Tools.CreateWww(actionAddress, wwwForm, "from_friends: " + isFromFromFriendsScene);
		yield return updateFriendsInfoRequest;
		string response = URLs.Sanitize(updateFriendsInfoRequest);
		invitesToUs.Clear();
		if (!string.IsNullOrEmpty(updateFriendsInfoRequest.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("update_frieds_info error: " + updateFriendsInfoRequest.error);
			}
			TrySendEventHideBoxProcessFriendsData();
		}
		else if (response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("update_frieds_info fail.");
			}
			TrySendEventHideBoxProcessFriendsData();
		}
		else if (string.IsNullOrEmpty(updateFriendsInfoRequest.error) && !string.IsNullOrEmpty(response))
		{
			ParseUpdateFriendsInfoResponse(response, _isUpdateInfoAboutAllFriends);
			notShowAddIds.Clear();
			if (UpdateFriendsInfoAction != null)
			{
				UpdateFriendsInfoAction();
			}
		}
	}

	public void SendInviteFightToPlayer(string _idFriend)
	{
		StartCoroutine(SendInviteFightToPlayerCoroutine(_idFriend));
	}

	private IEnumerator SendInviteFightToPlayerCoroutine(string _idFriend)
	{
		string _nick = ProfileController.GetPlayerNameOrDefault();
		_nick = FilterBadWorld.FilterString(_nick);
		WWWForm form = new WWWForm();
		string appVersion = string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion);
		form.AddField("action", "send_fight_invite");
		form.AddField("app_version", appVersion);
		form.AddField("uniq_id", id ?? string.Empty);
		form.AddField("name", _nick ?? string.Empty);
		form.AddField("reciever_id", _idFriend ?? string.Empty);
		form.AddField("auth", Hash("send_fight_invite"));
		if (Application.isEditor)
		{
			Debug.LogFormat("`HandleCallFriend to Action Server()`: `{0}`", Encoding.UTF8.GetString(form.data, 0, form.data.Length));
		}
		WWW request = Tools.CreateWww(actionAddress, form, string.Empty);
		yield return request;
		string response = URLs.Sanitize(request);
		if (!string.IsNullOrEmpty(request.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("send_fight_invite error: " + request.error);
			}
		}
		else if (response.Equals("fail"))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("send_fight_invite fail.");
			}
		}
		else if (Debug.isDebugBuild)
		{
			Debug.Log("send_fight_invite: " + response);
		}
	}

	public void ParseFightInvite(List<object> _invites)
	{
		if (!Defs.isEnableLocalInviteFromFriends)
		{
			return;
		}
		for (int i = 0; i < _invites.Count; i++)
		{
			Dictionary<string, object> dictionary = _invites[i] as Dictionary<string, object>;
			if (dictionary.ContainsKey("id") && dictionary.ContainsKey("name"))
			{
				string friendId = dictionary["id"].ToString();
				string nickname = dictionary["name"].ToString();
				BattleInviteListener.Instance.NotifyBattleIncomingInvite(friendId, nickname);
			}
		}
	}

	private void ParseUpdateFriendsInfoResponse(string response, bool _isUpdateInfoAboutAllFriends)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(response) as Dictionary<string, object>;
		List<string> list = new List<string>();
		HashSet<string> hashSet = new HashSet<string>(friends);
		HashSet<string> hashSet2 = new HashSet<string>(invitesToUs);
		if (dictionary.ContainsKey("friends"))
		{
			friends.Clear();
			List<object> list2 = dictionary["friends"] as List<object>;
			for (int i = 0; i < list2.Count; i++)
			{
				string text = list2[i] as string;
				if (getPossibleFriendsResult.ContainsKey(text))
				{
					getPossibleFriendsResult.Remove(text);
				}
				friends.Add(text);
				if ((!_isUpdateInfoAboutAllFriends && !friendsInfo.ContainsKey(text)) || _isUpdateInfoAboutAllFriends)
				{
					list.Add(text);
				}
			}
		}
		if (dictionary.ContainsKey("invites"))
		{
			invitesToUs.Clear();
			List<object> list3 = dictionary["invites"] as List<object>;
			for (int j = 0; j < list3.Count; j++)
			{
				string text2 = list3[j] as string;
				if (!friends.Contains(text2))
				{
					invitesToUs.Add(text2);
				}
				if ((!_isUpdateInfoAboutAllFriends && !friendsInfo.ContainsKey(text2) && !clanFriendsInfo.ContainsKey(text2) && !profileInfo.ContainsKey(text2)) || _isUpdateInfoAboutAllFriends)
				{
					list.Add(text2);
				}
			}
		}
		if (dictionary.ContainsKey("invites_outcoming"))
		{
			invitesFromUs.Clear();
			List<object> list4 = dictionary["invites_outcoming"] as List<object>;
			for (int k = 0; k < list4.Count; k++)
			{
				string item = list4[k] as string;
				if (!friends.Contains(item))
				{
					invitesFromUs.Add(item);
				}
			}
		}
		if (_isUpdateInfoAboutAllFriends)
		{
			List<string> list5 = new List<string>(friends);
			List<string> list6 = new List<string>();
			list5.AddRange(invitesToUs);
			foreach (KeyValuePair<string, Dictionary<string, object>> item2 in friendsInfo)
			{
				if (!list5.Contains(item2.Key))
				{
					list6.Add(item2.Key);
				}
			}
			if (list6.Count > 0)
			{
				for (int l = 0; l < list6.Count; l++)
				{
					friendsInfo.Remove(list6[l]);
				}
				SaveCurrentState();
			}
		}
		if (dictionary.ContainsKey("onLines"))
		{
			string response2 = Json.Serialize(dictionary["onLines"]);
			ParseOnlinesResponse(response2);
		}
		if (list.Count > 0)
		{
			StartCoroutine(GetInfoAboutNPlayers(list));
		}
		else
		{
			if ((!hashSet.SetEquals(friends) || !hashSet2.SetEquals(invitesToUs)) && FriendsController.FriendsUpdated != null)
			{
				FriendsController.FriendsUpdated();
			}
			TrySendEventHideBoxProcessFriendsData();
		}
		if (dictionary.ContainsKey("chat"))
		{
			string response3 = Json.Serialize(dictionary["chat"]);
			if (ChatController.sharedController != null)
			{
				ChatController.sharedController.ParseUpdateChatMessageResponse(response3);
			}
		}
		if (dictionary.ContainsKey("fight_invites"))
		{
			ParseFightInvite(dictionary["fight_invites"] as List<object>);
		}
	}

	private void ParseOnlinesResponse(string response)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(response) as Dictionary<string, object>;
		if (dictionary == null)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning(" GetAllPlayersOnline info = null");
			}
			return;
		}
		Dictionary<string, Dictionary<string, string>> dictionary2 = new Dictionary<string, Dictionary<string, string>>();
		foreach (string key in dictionary.Keys)
		{
			Dictionary<string, object> dictionary3 = dictionary[key] as Dictionary<string, object>;
			Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> item in dictionary3)
			{
				dictionary4.Add(item.Key, item.Value as string);
			}
			dictionary2.Add(key, dictionary4);
		}
		onlineInfo.Clear();
		foreach (string key2 in dictionary2.Keys)
		{
			Dictionary<string, string> dictionary5 = dictionary2[key2];
			int num = int.Parse(dictionary5["game_mode"]);
			int num2 = num - num / 10 * 10;
			if (num2 != 3 && num2 != 8)
			{
				if (!onlineInfo.ContainsKey(key2))
				{
					onlineInfo.Add(key2, dictionary5);
				}
				else
				{
					onlineInfo[key2] = dictionary5;
				}
			}
		}
	}

	public void ClearAllFriendsInvites()
	{
		StartCoroutine(ClearAllFriendsInvitesCoroutine());
	}

	public void UpdateRecordByFriendsJoinClick(string friendId)
	{
		if (clicksJoinByFriends.ContainsKey(friendId))
		{
			clicksJoinByFriends[friendId] = DateTime.UtcNow.ToString("s");
		}
		else
		{
			clicksJoinByFriends.Add(friendId, DateTime.UtcNow.ToString("s"));
		}
	}

	public DateTime GetDateLastClickJoinFriend(string friendId)
	{
		if (!clicksJoinByFriends.ContainsKey(friendId))
		{
			return DateTime.MaxValue;
		}
		DateTime result;
		return (!DateTime.TryParse(clicksJoinByFriends[friendId], out result)) ? result : DateTime.MaxValue;
	}

	private void ClearListClickJoinFriends()
	{
		clicksJoinByFriends.Clear();
		PlayerPrefs.SetString("CachedFriendsJoinClickList", string.Empty);
	}

	private void UpdateCachedClickJoinListValue()
	{
		if (clicksJoinByFriends.Count != 0)
		{
			string text = Json.Serialize(clicksJoinByFriends);
			PlayerPrefs.SetString("CachedFriendsJoinClickList", text ?? string.Empty);
		}
	}

	private void FillClickJoinFriendsListByCachedValue()
	{
		string @string = PlayerPrefs.GetString("CachedFriendsJoinClickList", string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			clicksJoinByFriends.Add(item.Key, Convert.ToString(item.Value));
		}
	}

	private void SyncClickJoinFriendsListWithListFriends()
	{
		if (clicksJoinByFriends.Count == 0)
		{
			return;
		}
		if (friends.Count == 0)
		{
			ClearListClickJoinFriends();
			return;
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, string> clicksJoinByFriend in clicksJoinByFriends)
		{
			if (!playersInfo.ContainsKey(clicksJoinByFriend.Key))
			{
				list.Add(clicksJoinByFriend.Key);
			}
		}
		if (list.Count != 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				string key = list[i];
				clicksJoinByFriends.Remove(key);
			}
			UpdateCachedClickJoinListValue();
		}
	}

	public static ResultParseOnlineData ParseOnlineData(Dictionary<string, string> onlineData)
	{
		string gameModeString = onlineData["game_mode"];
		string protocolString = onlineData["protocol"];
		string mapIndex = string.Empty;
		if (onlineData.ContainsKey("map"))
		{
			mapIndex = onlineData["map"];
		}
		return ParseOnlineData(gameModeString, protocolString, mapIndex);
	}

	private static ResultParseOnlineData ParseOnlineData(string gameModeString, string protocolString, string mapIndex)
	{
		int num = int.Parse(gameModeString);
		num = ((num <= 99) ? (-1) : (num / 100));
		int result;
		if (!int.TryParse(gameModeString, out result))
		{
			result = -1;
		}
		else
		{
			if (result > 99)
			{
				result -= num * 100;
			}
			result /= 10;
		}
		ResultParseOnlineData resultParseOnlineData = new ResultParseOnlineData();
		bool flag = num == -1 || num != (int)ConnectSceneNGUIController.myPlatformConnect;
		bool flag2 = result == -1 || ExpController.GetOurTier() != result;
		bool flag3 = num == 3;
		int num2 = Convert.ToInt32(gameModeString);
		string multiplayerProtocolVersion = GlobalGameController.MultiplayerProtocolVersion;
		resultParseOnlineData.gameMode = gameModeString;
		resultParseOnlineData.mapIndex = mapIndex;
		bool flag4 = num2 == 6;
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(mapIndex));
		bool flag5 = ((infoScene != null && infoScene.IsAvaliableForMode(TypeModeGame.Dater)) ? true : false);
		if (flag4)
		{
			resultParseOnlineData.notConnectCondition = NotConnectCondition.InChat;
		}
		else if (!flag3 && flag && !flag5)
		{
			resultParseOnlineData.notConnectCondition = NotConnectCondition.platform;
		}
		else if (!flag3 && flag2 && !flag5)
		{
			resultParseOnlineData.notConnectCondition = NotConnectCondition.level;
		}
		else if (multiplayerProtocolVersion != protocolString)
		{
			resultParseOnlineData.notConnectCondition = NotConnectCondition.clientVersion;
		}
		else if (infoScene == null)
		{
			resultParseOnlineData.notConnectCondition = NotConnectCondition.map;
		}
		else
		{
			resultParseOnlineData.notConnectCondition = NotConnectCondition.None;
		}
		resultParseOnlineData.isPlayerInChat = flag4;
		return resultParseOnlineData;
	}

	private static void SendEmailWithMyId()
	{
		MailUrlBuilder mailUrlBuilder = new MailUrlBuilder();
		mailUrlBuilder.to = string.Empty;
		mailUrlBuilder.subject = LocalizationStore.Get("Key_1565");
		string text = ((!(sharedController != null)) ? string.Empty : sharedController.id);
		string format = LocalizationStore.Get("Key_1508");
		mailUrlBuilder.body = string.Format(format, sharedController.id);
		Application.OpenURL(mailUrlBuilder.GetUrl());
	}

	public static void SendMyIdByEmail()
	{
		MainMenuController.DoMemoryConsumingTaskInEmptyScene(delegate
		{
			InfoWindowController.ShowDialogBox(LocalizationStore.Get("Key_1572"), SendEmailWithMyId);
		});
	}

	public static void CopyMyIdToClipboard()
	{
		CopyPlayerIdToClipboard(sharedController.id);
	}

	public static void CopyPlayerIdToClipboard(string playerId)
	{
		UniPasteBoard.SetClipBoardString(playerId);
		InfoWindowController.ShowInfoBox(LocalizationStore.Get("Key_1618"));
	}

	public static void JoinToFriendRoom(string friendId)
	{
		if (!(sharedController == null) && sharedController.onlineInfo.ContainsKey(friendId))
		{
			Dictionary<string, string> dictionary = sharedController.onlineInfo[friendId];
			int result;
			int.TryParse(dictionary["game_mode"], out result);
			string nameRoom = dictionary["room_name"];
			string text = dictionary["map"];
			JoinToFriendRoomController instance = JoinToFriendRoomController.Instance;
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(text));
			if (infoScene != null && instance != null)
			{
				instance.ConnectToRoom(result, nameRoom, text);
				sharedController.UpdateRecordByFriendsJoinClick(friendId);
			}
		}
	}

	public static bool IsPlayerOurFriend(string playerId)
	{
		if (sharedController == null)
		{
			return false;
		}
		return sharedController.friends.Contains(playerId);
	}

	public static bool IsPlayerOurClanMember(string playerId)
	{
		if (sharedController == null)
		{
			return false;
		}
		for (int i = 0; i < sharedController.clanMembers.Count; i++)
		{
			Dictionary<string, string> dictionary = sharedController.clanMembers[i];
			if (dictionary["id"].Equals(playerId))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsSelfClanLeader()
	{
		if (sharedController == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(sharedController.clanLeaderID))
		{
			return false;
		}
		return sharedController.clanLeaderID.Equals(sharedController.id);
	}

	public static void SendFriendshipRequest(string playerId, Dictionary<string, object> socialEventParameters, Action<bool, bool> callbackResult)
	{
		if (!(sharedController == null))
		{
			if (socialEventParameters == null)
			{
				throw new ArgumentNullException("socialEventParameters");
			}
			sharedController.StartCoroutine(sharedController.FriendRequest(playerId, socialEventParameters, callbackResult));
		}
	}

	public static Dictionary<string, object> GetFullPlayerDataById(string playerId)
	{
		if (sharedController == null)
		{
			return null;
		}
		Dictionary<string, object> value;
		if (sharedController.friendsInfo.TryGetValue(playerId, out value))
		{
			return value;
		}
		if (sharedController.clanFriendsInfo.TryGetValue(playerId, out value))
		{
			return value;
		}
		if (sharedController.profileInfo.TryGetValue(playerId, out value))
		{
			return value;
		}
		return null;
	}

	public static bool IsFriendsMax()
	{
		if (sharedController == null)
		{
			return false;
		}
		return sharedController.friends.Count >= Defs.maxCountFriend;
	}

	public static bool IsFriendsDataExist()
	{
		if (sharedController == null)
		{
			return false;
		}
		return sharedController.friends.Count > 0 && sharedController.friendsInfo.Count > 0;
	}

	public static bool IsFriendsOrLocalDataExist()
	{
		if (sharedController == null)
		{
			return false;
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, PossiblleOrigin> item in sharedController.getPossibleFriendsResult)
		{
			if (sharedController.profileInfo.ContainsKey(item.Key) && item.Value == PossiblleOrigin.Local)
			{
				list.Add(item.Key);
			}
		}
		return (sharedController.friends.Count > 0 && sharedController.friendsInfo.Count > 0) || list.Count > 0;
	}

	public static bool IsPossibleFriendsDataExist()
	{
		if (sharedController == null)
		{
			return false;
		}
		return sharedController.getPossibleFriendsResult.Count > 0 && sharedController.profileInfo.Count > 0;
	}

	public static bool IsFriendInvitesDataExist()
	{
		if (sharedController == null)
		{
			return false;
		}
		return sharedController.invitesToUs.Count > 0 && (sharedController.clanFriendsInfo.Count > 0 || sharedController.profileInfo.Count > 0);
	}

	public static bool IsDataAboutFriendDownload(string playerId)
	{
		if (sharedController == null)
		{
			return false;
		}
		if (sharedController.friendsInfo.ContainsKey(playerId))
		{
			return true;
		}
		if (sharedController.clanFriendsInfo.ContainsKey(playerId))
		{
			return true;
		}
		if (sharedController.profileInfo.ContainsKey(playerId))
		{
			return true;
		}
		return false;
	}

	public static void ShowProfile(string id, ProfileWindowType type, Action<bool> onCloseEvent = null)
	{
		if (_friendProfileController == null)
		{
			_friendProfileController = new FriendProfileController(onCloseEvent);
		}
		_friendProfileController.HandleProfileClickedCore(id, type, onCloseEvent);
	}

	public static void DisposeProfile()
	{
		if (_friendProfileController != null)
		{
			_friendProfileController.Dispose();
			_friendProfileController = null;
		}
	}

	public static bool IsMyPlayerId(string playerId)
	{
		if (sharedController == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(sharedController.id))
		{
			return false;
		}
		return sharedController.id.Equals(playerId);
	}

	public static bool IsAlreadySendInvitePlayer(string playerId)
	{
		if (sharedController == null)
		{
			return false;
		}
		return sharedController.invitesFromUs.Contains(playerId);
	}

	public static PossiblleOrigin GetPossibleFriendFindOrigin(string playerId)
	{
		if (sharedController == null)
		{
			return PossiblleOrigin.None;
		}
		if (!sharedController.getPossibleFriendsResult.ContainsKey(playerId))
		{
			return PossiblleOrigin.None;
		}
		return sharedController.getPossibleFriendsResult[playerId];
	}

	public static bool IsAlreadySendClanInvitePlayer(string playerId)
	{
		if (sharedController == null)
		{
			return false;
		}
		return sharedController.ClanSentInvites.Contains(playerId);
	}

	public static bool IsMaxClanMembers()
	{
		if (sharedController == null)
		{
			return false;
		}
		return sharedController.clanMembers.Count >= Defs.maxMemberClanCount;
	}

	public static void StartSendReview()
	{
		if (sharedController != null)
		{
			sharedController.StopCoroutine("WaitReviewAndSend");
			sharedController.StartCoroutine("WaitReviewAndSend");
		}
	}

	private IEnumerator WaitReviewAndSend()
	{
		while (ReviewController.ExistReviewForSend)
		{
			yield return StartCoroutine(SendReviewForPlayerWithID(ReviewController.ReviewRating, ReviewController.ReviewMsg));
			yield return new WaitForSeconds(600f);
		}
	}

	public IEnumerator SendReviewForPlayerWithID(int rating, string msgReview)
	{
		if (ReviewController.isSending)
		{
			yield break;
		}
		ReviewController.isSending = true;
		string playerId = sharedController.id;
		if (string.IsNullOrEmpty(playerId))
		{
			ReviewController.isSending = false;
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "set_feedback");
		form.AddField("text", msgReview);
		form.AddField("rating", rating);
		form.AddField("version", GlobalGameController.AppVersion);
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("platform", ProtocolListGetter.CurrentPlatform);
		form.AddField("device_model", SystemInfo.deviceModel);
		form.AddField("auth", Hash("set_feedback"));
		form.AddField("uniq_id", playerId);
		WWW download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
		if (download == null)
		{
			ReviewController.isSending = false;
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (!string.IsNullOrEmpty(download.error))
		{
			ReviewController.isSending = false;
			Debug.LogFormat("Error send review: {0}", download.error);
			yield break;
		}
		if (Debug.isDebugBuild)
		{
			Debug.Log("Review send for id " + playerId + ": " + response);
		}
		ReviewController.isSending = false;
		ReviewController.ExistReviewForSend = false;
	}

	public static void LogPromoTrafficForwarding(TypeTrafficForwardingLog type)
	{
		if (type != TypeTrafficForwardingLog.view || !((DateTime.Now - timeSendTrafficForwarding).TotalMinutes < 60.0))
		{
			if (type == TypeTrafficForwardingLog.view || type == TypeTrafficForwardingLog.newView)
			{
				timeSendTrafficForwarding = DateTime.Now;
			}
			if (sharedController != null)
			{
				sharedController.StartCoroutine(sharedController.SendPromoTrafficForwarding(type));
			}
		}
	}

	public IEnumerator SendPromoTrafficForwarding(TypeTrafficForwardingLog type)
	{
		WWW download;
		while (true)
		{
			if (string.IsNullOrEmpty(id))
			{
				yield break;
			}
			if (Application.isEditor)
			{
				Debug.Log("SendPromoTrafficForwarding:" + type);
			}
			WWWForm form = new WWWForm();
			form.AddField("action", "promo_pgw_stat_update");
			form.AddField("auth", Hash("promo_pgw_stat_update"));
			form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
			form.AddField("uniq_id", id);
			form.AddField("is_paying", Storager.getInt("PayingUser", true));
			form.AddField("platform", ProtocolListGetter.CurrentPlatform);
			if (type == TypeTrafficForwardingLog.click)
			{
				form.AddField("add_click", 1);
			}
			if (type == TypeTrafficForwardingLog.newView)
			{
				form.AddField("add_new_view", 1);
			}
			if (type == TypeTrafficForwardingLog.newView || type == TypeTrafficForwardingLog.view)
			{
				form.AddField("add_view", 1);
			}
			download = Tools.CreateWwwIfNotConnected(actionAddress, form, string.Empty);
			if (download == null)
			{
				yield break;
			}
			yield return download;
			if (download != null && string.IsNullOrEmpty(download.error))
			{
				break;
			}
			if (Application.isEditor && download != null && !string.IsNullOrEmpty(download.error))
			{
				Debug.LogWarning("Error send log promo_pgw_stat_update: " + download.error);
			}
			yield return new WaitForSeconds(600f);
		}
		if (Application.isEditor)
		{
			Debug.Log(string.Concat(str3: URLs.Sanitize(download), str0: "SendPromoTrafficForwarding(", str1: type.ToString(), str2: "):"));
		}
	}

	private IEnumerator GetABTestAdvertConfig()
	{
		while (!TrainingController.TrainingCompleted)
		{
			yield return null;
		}
		if (Defs.cohortABTestAdvert == Defs.ABTestCohortsType.SKIP)
		{
			yield break;
		}
		string responseText;
		while (true)
		{
			WWW download = Tools.CreateWww(URLs.ABTestAdvertURL);
			if (download == null)
			{
				yield return StartCoroutine(MyWaitForSeconds(30f));
				continue;
			}
			yield return download;
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild || Application.isEditor)
				{
					Debug.LogWarning("GetABTestAdvertConfig error: " + download.error);
				}
				yield return StartCoroutine(MyWaitForSeconds(30f));
			}
			else
			{
				responseText = URLs.Sanitize(download);
				if (!string.IsNullOrEmpty(responseText))
				{
					break;
				}
			}
		}
		Storager.setString("abTestAdvertConfigKey", responseText, false);
		ParseABTestAdvertConfig(false);
	}

	public static void ParseABTestAdvertConfig(bool isFromReset = false)
	{
		if (Defs.cohortABTestAdvert == Defs.ABTestCohortsType.SKIP)
		{
			isReadABTestAdvertConfig = true;
		}
		if (!Storager.hasKey("abTestAdvertConfigKey") || string.IsNullOrEmpty(Storager.getString("abTestAdvertConfigKey", false)))
		{
			return;
		}
		string @string = Storager.getString("abTestAdvertConfigKey", false);
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary != null && dictionary.ContainsKey("enableABTest"))
		{
			int num = Convert.ToInt32(dictionary["enableABTest"]);
			if (num == 1 && Defs.cohortABTestAdvert != Defs.ABTestCohortsType.SKIP)
			{
				configNameABTestAdvert = Convert.ToString(dictionary["configName"]);
				if (Defs.cohortABTestAdvert == Defs.ABTestCohortsType.NONE)
				{
					int cohortABTestAdvert = UnityEngine.Random.Range(1, 3);
					Defs.cohortABTestAdvert = (Defs.ABTestCohortsType)cohortABTestAdvert;
					string nameCohort = ((Defs.cohortABTestAdvert != Defs.ABTestCohortsType.A) ? "AdvertB_" : "AdvertA_") + configNameABTestAdvert;
					AnalyticsStuff.LogABTest("Advert", nameCohort);
					if (sharedController != null)
					{
						sharedController.SendOurData(false);
					}
				}
				if (Defs.cohortABTestAdvert == Defs.ABTestCohortsType.B)
				{
					Dictionary<string, object> dictionary2 = dictionary["settings-b"] as Dictionary<string, object>;
					FreeAwardController.appId = Convert.ToString(dictionary2["appId"]);
					FreeAwardController.securityToken = Convert.ToString(dictionary2["token"]);
					AdsConfigManager.configFromABTestAdvert = Json.Serialize(dictionary["config"]);
				}
				else
				{
					Dictionary<string, object> dictionary3 = dictionary["settings-a"] as Dictionary<string, object>;
					FreeAwardController.appId = Convert.ToString(dictionary3["appId"]);
					FreeAwardController.securityToken = Convert.ToString(dictionary3["token"]);
				}
			}
			else if (!isFromReset)
			{
				ResetABTestAdvert();
			}
		}
		isReadABTestAdvertConfig = true;
	}

	public static void ResetABTestAdvert()
	{
		if (Defs.cohortABTestAdvert != Defs.ABTestCohortsType.SKIP)
		{
			if (Defs.cohortABTestAdvert == Defs.ABTestCohortsType.A || Defs.cohortABTestAdvert == Defs.ABTestCohortsType.B)
			{
				string nameCohort = ((Defs.cohortABTestAdvert != Defs.ABTestCohortsType.A) ? "AdvertB_" : "AdvertA_") + configNameABTestAdvert;
				AnalyticsStuff.LogABTest("Advert", nameCohort, false);
				if (sharedController != null)
				{
					sharedController.SendOurData(false);
				}
			}
			Defs.cohortABTestAdvert = Defs.ABTestCohortsType.SKIP;
			ParseABTestAdvertConfig(true);
			FreeAwardController.appId = "32897";
			FreeAwardController.securityToken = "cf77aeadd83faf98e0cad61a1f1403c8";
			AdsConfigManager.configFromABTestAdvert = string.Empty;
		}
		isReadABTestAdvertConfig = true;
	}
}

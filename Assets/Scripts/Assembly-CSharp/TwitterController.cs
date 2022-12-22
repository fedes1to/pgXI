using System;
using System.Collections.Generic;
using System.Linq;
using Prime31;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

internal sealed class TwitterController : MonoBehaviour
{
	private abstract class TwitterFacadeBase
	{
		public abstract void Init(string consumerKey, string consumerSecret);

		public abstract bool IsLoggedIn();

		public abstract void PostStatusUpdate(string status);

		public abstract void ShowLoginDialog(Action WP8customOnSuccessLogin = null);

		public abstract void Logout();
	}

	private class IosTwitterFacade : TwitterFacadeBase
	{
		public override void Init(string consumerKey, string consumerSecret)
		{
		}

		public override bool IsLoggedIn()
		{
			throw new NotSupportedException();
		}

		public string LoggedInUsername()
		{
			throw new NotSupportedException();
		}

		public override void PostStatusUpdate(string status)
		{
		}

		public override void ShowLoginDialog(Action WP8customOnSuccessLogin = null)
		{
		}

		public override void Logout()
		{
		}
	}

	private class AndroidTwitterFacade : TwitterFacadeBase
	{
		public override void Init(string consumerKey, string consumerSecret)
		{
			TwitterAndroid.init(consumerKey, consumerSecret);
		}

		public override bool IsLoggedIn()
		{
			return TwitterAndroid.isLoggedIn();
		}

		public override void PostStatusUpdate(string status)
		{
			TwitterAndroid.postStatusUpdate(status);
		}

		public override void ShowLoginDialog(Action WP8customOnSuccessLogin = null)
		{
			TwitterAndroid.showLoginDialog(false);
		}

		public override void Logout()
		{
			TwitterAndroid.logout();
		}
	}

	private class DummyTwitterFacade : TwitterFacadeBase
	{
		public override void Init(string consumerKey, string consumerSecret)
		{
		}

		public override bool IsLoggedIn()
		{
			return BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64;
		}

		public override void PostStatusUpdate(string status)
		{
		}

		public override void ShowLoginDialog(Action WP8customOnSuccessLogin = null)
		{
		}

		public override void Logout()
		{
		}
	}

	public const int DefaultGreenPriorityLimit = 7;

	public const int DefaultRedPriorityLimit = 3;

	public const int DefaultMultyWinPriorityLimit = 1;

	public const int DefaultArenaPriorityLimit = 1;

	private const string DefaultCallerContext = "Unknown";

	private const string StoryPostHistoryKey = "TwitterControllerStoryPostHistoryKey";

	private const string PriorityKey = "priority";

	private const string TimeKey = "time";

	public static TwitterController Instance;

	public static readonly Dictionary<FacebookController.StoryPriority, int> StoryPostLimits;

	private string _loginContext = "Unknown";

	private static readonly Lazy<TwitterFacadeBase> _twitterFacade;

	private bool _postInProcess;

	private float _timeSinceLastStoryPostHistoryClean;

	private List<Dictionary<string, object>> storiesPostHistory = new List<Dictionary<string, object>>();

	public static bool IsLoggedIn
	{
		get
		{
			return TwitterFacade.IsLoggedIn();
		}
	}

	public static bool TwitterSupported
	{
		get
		{
			return BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64;
		}
	}

	public static bool TwitterSupported_OldPosts
	{
		get
		{
			return TwitterSupported && BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64 && Instance != null && !Instance.CanPostStatusUpdateWithPriority(FacebookController.StoryPriority.Green);
		}
	}

	private static TwitterFacadeBase TwitterFacade
	{
		get
		{
			return _twitterFacade.Value;
		}
	}

	static TwitterController()
	{
		StoryPostLimits = new Dictionary<FacebookController.StoryPriority, int>(4, new FacebookController.StoryPriorityComparer())
		{
			{
				FacebookController.StoryPriority.Green,
				7
			},
			{
				FacebookController.StoryPriority.Red,
				3
			},
			{
				FacebookController.StoryPriority.MultyWinLimit,
				1
			},
			{
				FacebookController.StoryPriority.ArenaLimit,
				1
			}
		};
		_twitterFacade = new Lazy<TwitterFacadeBase>(InitializeFacade);
	}

	private void Awake()
	{
		Instance = this;
		if (TwitterSupported)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			empty = "13K6E5YAJvXSEaig0GVVFd68K";
			empty2 = "I4DtR7TC0OU26OMYI0hLmP1jhVHfNuPRMskbYDIoOS2xYBBWdS";
			TwitterFacade.Init(empty, empty2);
			TwitterManager.loginSucceededEvent += OnTwitterLogin;
			TwitterManager.loginFailedEvent += OnTwitterLoginFailed;
			TwitterManager.requestDidFinishEvent += OnTwitterPost;
			TwitterManager.requestDidFailEvent += OnTwitterPostFailed;
			FriendsController.NewTwitterLimitsAvailable += HandleNewTwitterLimitsAvailable;
		}
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		InitStoryPostHistoryKey();
		LoadStoryPostHistory();
	}

	public void Login(Action onSuccess = null, Action onFailure = null, string context = "Unknown")
	{
		if (!TwitterSupported)
		{
			return;
		}
		Action<string> onSuccessCallback = null;
		Action<string> onFailCallback = null;
		onSuccessCallback = delegate
		{
			if (onSuccess != null)
			{
				onSuccess();
			}
			TwitterManager.loginSucceededEvent -= onSuccessCallback;
			TwitterManager.loginFailedEvent -= onFailCallback;
		};
		onFailCallback = delegate
		{
			if (onFailure != null)
			{
				onFailure();
			}
			TwitterManager.loginSucceededEvent -= onSuccessCallback;
			TwitterManager.loginFailedEvent -= onFailCallback;
		};
		TwitterManager.loginSucceededEvent += onSuccessCallback;
		TwitterManager.loginFailedEvent += onFailCallback;
		if (Storager.getInt(Defs.IsTwitterLoginRewardaGained, true) == 0)
		{
			Storager.setInt(Defs.TwitterRewardGainStarted, 1, false);
		}
		_loginContext = context;
		TwitterFacade.ShowLoginDialog(null);
	}

	public void Logout()
	{
		if (TwitterSupported)
		{
			TwitterFacade.Logout();
		}
	}

	public void PostStatusUpdate(string status, FacebookController.StoryPriority priority)
	{
		if (TwitterSupported)
		{
			PostStatusUpdate(status, delegate
			{
				RegisterStoryPostedWithPriorityCore(priority);
			});
		}
	}

	public void PostStatusUpdate(string status, Action customOnSuccess = null)
	{
		if (!TwitterSupported)
		{
			return;
		}
		Action<string> postAction = null;
		Action<string> loginFailedAction = null;
		postAction = delegate
		{
			TwitterManager.loginSucceededEvent -= postAction;
			TwitterManager.loginFailedEvent -= loginFailedAction;
			if (!_postInProcess)
			{
				if (customOnSuccess != null)
				{
					Action<object> onSuccessPost = null;
					Action<string> onFailedPost = null;
					onSuccessPost = delegate
					{
						customOnSuccess();
						TwitterManager.requestDidFinishEvent -= onSuccessPost;
						TwitterManager.requestDidFailEvent -= onFailedPost;
					};
					onFailedPost = delegate
					{
						TwitterManager.requestDidFinishEvent -= onSuccessPost;
						TwitterManager.requestDidFailEvent -= onFailedPost;
					};
					TwitterManager.requestDidFinishEvent += onSuccessPost;
					TwitterManager.requestDidFailEvent += onFailedPost;
				}
				_postInProcess = true;
				TwitterFacade.PostStatusUpdate(status);
			}
		};
		loginFailedAction = delegate
		{
			TwitterManager.loginSucceededEvent -= postAction;
			TwitterManager.loginFailedEvent -= loginFailedAction;
		};
		if (TwitterFacade.IsLoggedIn())
		{
			postAction(string.Empty);
			return;
		}
		TwitterManager.loginSucceededEvent += postAction;
		TwitterManager.loginFailedEvent += loginFailedAction;
		TwitterFacade.ShowLoginDialog(delegate
		{
			postAction(string.Empty);
		});
	}

	public bool CanPostStatusUpdateWithPriority(FacebookController.StoryPriority priority)
	{
		//Discarded unreachable code: IL_006f
		try
		{
			if (priority == FacebookController.StoryPriority.Green)
			{
				return storiesPostHistory.Count < StoryPostLimits[priority];
			}
			return storiesPostHistory.Where((Dictionary<string, object> rec) => int.Parse((string)rec["priority"]) == (int)priority).Count() < StoryPostLimits[priority];
		}
		catch (Exception ex)
		{
			Debug.LogError("Exeption in CanPostStoryWithPriority:\n" + ex);
		}
		return false;
	}

	public static void CheckAndGiveTwitterReward(string context)
	{
		if (TwitterSupported && Storager.getInt(Defs.IsTwitterLoginRewardaGained, true) == 0 && Storager.getInt(Defs.TwitterRewardGainStarted, false) == 1 && TwitterFacade.IsLoggedIn())
		{
			Storager.setInt(Defs.TwitterRewardGainStarted, 0, false);
			Storager.setInt(Defs.IsTwitterLoginRewardaGained, 1, true);
			BankController.AddGems(10);
			TutorialQuestManager.Instance.AddFulfilledQuest("loginTwitter");
			QuestMediator.NotifySocialInteraction("loginTwitter");
			AnalyticsFacade.SendCustomEvent("Virality", new Dictionary<string, object> { 
			{
				"Login Twitter",
				context ?? "Unknown"
			} });
			AnalyticsFacade.SendCustomEventToAppsFlyer("Virality", new Dictionary<string, string> { 
			{
				"Login Twitter",
				context ?? "Unknown"
			} });
		}
	}

	private void OnTwitterLogin(string result)
	{
		CheckAndGiveTwitterReward(_loginContext);
		string message = string.Format("TwitterController.OnTwitterLogin(“{0}”)    {1}", result, _loginContext);
		Debug.Log(message);
		_loginContext = "Unknown";
	}

	private void OnTwitterLoginFailed(string error)
	{
		string message = string.Format("TwitterController.OnTwitterLoginFailed(“{0}”)    {1}", error, _loginContext);
		Debug.Log(message);
		_loginContext = "Unknown";
	}

	private void OnTwitterPost(object result)
	{
		_postInProcess = false;
		Debug.Log(("TwitterController: OnTwitterPost: " + result) ?? string.Empty);
	}

	private void OnTwitterPostFailed(string _error)
	{
		_postInProcess = false;
		Debug.Log(("TwitterController: OnTwitterPostFailed: " + _error) ?? string.Empty);
	}

	private void HandleNewTwitterLimitsAvailable(int greenLimit, int redLimit, int multyWinLimit, int arenaLimit)
	{
		StoryPostLimits[FacebookController.StoryPriority.Green] = greenLimit;
		StoryPostLimits[FacebookController.StoryPriority.Red] = redLimit;
		StoryPostLimits[FacebookController.StoryPriority.MultyWinLimit] = multyWinLimit;
		StoryPostLimits[FacebookController.StoryPriority.ArenaLimit] = arenaLimit;
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			SaveStoryPostHistory();
		}
		else
		{
			LoadStoryPostHistory();
		}
	}

	private void Update()
	{
		if (Time.realtimeSinceStartup - _timeSinceLastStoryPostHistoryClean > 10f)
		{
			CleanStoryPostHistory();
		}
	}

	private void OnDestroy()
	{
		SaveStoryPostHistory();
		if (TwitterSupported)
		{
			FriendsController.NewTwitterLimitsAvailable -= HandleNewTwitterLimitsAvailable;
		}
	}

	private void SaveStoryPostHistory()
	{
		Storager.setString("TwitterControllerStoryPostHistoryKey", Rilisoft.MiniJson.Json.Serialize(storiesPostHistory), false);
	}

	private void LoadStoryPostHistory()
	{
		try
		{
			List<object> list = Rilisoft.MiniJson.Json.Deserialize(Storager.getString("TwitterControllerStoryPostHistoryKey", false)) as List<object>;
			storiesPostHistory.Clear();
			foreach (object item2 in list)
			{
				Dictionary<string, object> item = item2 as Dictionary<string, object>;
				storiesPostHistory.Add(item);
			}
			CleanStoryPostHistory();
		}
		catch (Exception)
		{
			storiesPostHistory.Clear();
		}
	}

	private void CleanStoryPostHistory()
	{
		_timeSinceLastStoryPostHistoryClean = Time.realtimeSinceStartup;
		try
		{
			long num = 86400L;
			long minimumValidTime = PromoActionsManager.CurrentUnixTime - num;
			storiesPostHistory.RemoveAll((Dictionary<string, object> entry) => long.Parse((string)entry["time"]) < minimumValidTime);
		}
		catch (Exception ex)
		{
			Debug.LogError("TwitterController Exeption in CleanStoryPostHistory:\n" + ex);
		}
	}

	private void RegisterStoryPostedWithPriorityCore(FacebookController.StoryPriority priority)
	{
		if (TwitterSupported)
		{
			List<Dictionary<string, object>> list = storiesPostHistory;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			int num = (int)priority;
			dictionary.Add("priority", num.ToString());
			dictionary.Add("time", PromoActionsManager.CurrentUnixTime.ToString());
			list.Add(dictionary);
		}
	}

	private void InitStoryPostHistoryKey()
	{
		if (!Storager.hasKey("TwitterControllerStoryPostHistoryKey"))
		{
			Storager.setString("TwitterControllerStoryPostHistoryKey", "[]", false);
		}
	}

	private static TwitterFacadeBase InitializeFacade()
	{
		if (!TwitterSupported)
		{
			return new DummyTwitterFacade();
		}
		switch (BuildSettings.BuildTargetPlatform)
		{
		case RuntimePlatform.IPhonePlayer:
			return new IosTwitterFacade();
		case RuntimePlatform.Android:
			return new AndroidTwitterFacade();
		default:
			return new DummyTwitterFacade();
		}
	}
}

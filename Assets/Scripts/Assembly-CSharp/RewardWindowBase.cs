using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using Rilisoft;
using UnityEngine;

public sealed class RewardWindowBase : MonoBehaviour
{
	private const string DefaultCallerContext = "Reward Window";

	public bool ShowLoginsButton = true;

	public GameObject connectToSocialContainer;

	public List<UILabel> connectToSocial;

	public UIWidget containerWidget;

	public float widgetExpanded;

	public float widgetCollapsed;

	public float widgetNoConnectToSocial;

	public UIButton facebook;

	public UIButton twitter;

	public UIButton continueButton;

	public UIButton hideButton;

	public UIButton facebookNoReward;

	public UIButton twitterNoReward;

	public UIButton continueAndShare;

	public UIButton collect;

	public UIButton collectAndShare;

	public UIGrid innerGrid;

	public ToggleButton share;

	public GameObject shareContainer;

	public bool shouldHideExpGui = true;

	public GameObject soundsController;

	[Header("Not Connected")]
	public Vector3 continue_NotConnected;

	[Header("Not Connected")]
	public Vector3 facebook_NotConnected;

	[Header("Not Connected")]
	public Vector3 twitter_NotConnected;

	[Header("Twitter Connected")]
	public Vector3 continue_TwiiterConnected;

	[Header("Twitter Connected")]
	public Vector3 facebook_TwiiterConnected;

	[Header("Facebook Connected")]
	public Vector3 continue_FacebookConnected;

	[Header("Facebook Connected")]
	public Vector3 twitter_FacebookConnected;

	[Header("All Connected")]
	public Vector3 continue_AllConnected;

	public Action shareAction;

	public Action customHide;

	private bool _collectOnlyNoShare;

	[HideInInspector]
	public Animator animatorLevel;

	private IDisposable _backSubscription;

	private float _timeSinceUpdateConnetToSocialText;

	private FacebookController.StoryPriority _facebookPriority;

	private FacebookController.StoryPriority _twiiterPriority;

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

	public FacebookController.StoryPriority priority
	{
		set
		{
			facebookPriority = value;
			twitterPriority = value;
		}
	}

	public Func<string> twitterStatus { get; set; }

	public bool HasReward { get; set; }

	public bool CollectOnlyNoShare
	{
		get
		{
			return _collectOnlyNoShare;
		}
		set
		{
			_collectOnlyNoShare = value;
		}
	}

	public string EventTitle { get; set; }

	private string CallerContext
	{
		get
		{
			return (!string.IsNullOrEmpty(EventTitle)) ? string.Format("{0}: {1}", "Reward Window", EventTitle) : "Reward Window";
		}
	}

	public static event Action Shown;

	private void Awake()
	{
		if (share != null)
		{
			share.IsChecked = true;
		}
		animatorLevel = GetComponent<Animator>();
		LocalizationStore.AddEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void OnDestroy()
	{
		LocalizationStore.DelEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void HandleLocalizationChanged()
	{
		SetConnectToSocialText();
	}

	private void OnEnable()
	{
		SetConnectToSocialText();
		if ((bool)soundsController)
		{
			soundsController.SetActive(Defs.isSoundFX);
		}
		if (Application.isEditor)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		Action shown = RewardWindowBase.Shown;
		if (shown != null)
		{
			shown();
		}
		if (shouldHideExpGui)
		{
			RentScreenController.SetDepthForExpGUI(0);
		}
		StartCoroutine(SendAnalyticsOnShowCoroutine());
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(delegate
		{
			if (hideButton != null)
			{
				List<EventDelegate> onClick = hideButton.onClick;
				EventDelegate.Execute(onClick);
			}
		}, "Reward Window");
		if (animatorLevel != null)
		{
			if (ExperienceController.sharedController.currentLevel == 2)
			{
				animatorLevel.SetTrigger("Weapons");
			}
			else if (ExperienceController.sharedController.AddHealthOnCurLevel == 0)
			{
				animatorLevel.SetTrigger("is2items");
			}
			else
			{
				animatorLevel.SetTrigger("is3items");
			}
			animatorLevel.SetBool("IsRatingSystem", true);
		}
	}

	private void OnDisable()
	{
		if (shouldHideExpGui)
		{
			RentScreenController.SetDepthForExpGUI(99);
		}
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private IEnumerator SendAnalyticsOnShowCoroutine()
	{
		for (int i = 0; i < 2; i++)
		{
			yield return null;
		}
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "Show Event", EventTitle } };
		if (!CollectOnlyNoShare && FacebookController.FacebookSupported && FacebookController.sharedController.CanPostStoryWithPriority(facebookPriority))
		{
			parameters.Add("Total Facebook", "Shows");
		}
		if (!CollectOnlyNoShare && TwitterController.TwitterSupported && TwitterController.Instance.CanPostStatusUpdateWithPriority(twitterPriority))
		{
			parameters.Add("Total Twitter", "Shows");
		}
		AnalyticsFacade.SendCustomEvent("Virality", parameters);
	}

	private bool ShouldShowFacebookWithRewardButton()
	{
		return !FB.IsLoggedIn && Storager.getInt(Defs.IsFacebookLoginRewardaGained, true) == 0 && !CollectOnlyNoShare && ShowLoginsButton && !Device.isPixelGunLow;
	}

	private bool ShouldShowTwitterWithRewardButton()
	{
		return !TwitterController.IsLoggedIn && Storager.getInt(Defs.IsTwitterLoginRewardaGained, true) == 0 && !CollectOnlyNoShare && ShowLoginsButton && !Device.isPixelGunLow;
	}

	private void SetConnectToSocialText()
	{
		_timeSinceUpdateConnetToSocialText = Time.realtimeSinceStartup;
		foreach (UILabel item in connectToSocial)
		{
			if (item != null)
			{
				item.text = string.Format(LocalizationStore.Get("Key_1460"), 10);
			}
		}
	}

	private void Start()
	{
		SetButtonPositionsAndActive();
	}

	private void SetButtonPositionsAndActive()
	{
		if (!ShowLoginsButton)
		{
			return;
		}
		bool flag = false;
		bool flag2 = ((FacebookController.FacebookSupported && Storager.getInt(Defs.IsFacebookLoginRewardaGained, true) == 0) || (TwitterController.TwitterSupported && Storager.getInt(Defs.IsTwitterLoginRewardaGained, true) == 0)) && !CollectOnlyNoShare && ShowLoginsButton && !Device.isPixelGunLow;
		if (connectToSocialContainer != null && connectToSocialContainer.activeSelf != flag2)
		{
			connectToSocialContainer.SetActive(flag2);
			flag = true;
		}
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = FacebookController.FacebookSupported && ShouldShowFacebookWithRewardButton() && TrainingController.TrainingCompleted;
		if (facebook != null && facebook.gameObject.activeSelf != flag5)
		{
			facebook.gameObject.SetActive(flag5);
			flag3 = true;
		}
		bool flag6 = FacebookController.FacebookSupported && !FB.IsLoggedIn && Storager.getInt(Defs.IsFacebookLoginRewardaGained, true) == 1 && !CollectOnlyNoShare && TrainingController.TrainingCompleted && ShowLoginsButton && !Device.isPixelGunLow;
		if (facebookNoReward != null && facebookNoReward.gameObject.activeSelf != flag6)
		{
			facebookNoReward.gameObject.SetActive(flag6);
			flag3 = true;
		}
		bool flag7 = TwitterController.TwitterSupported && ShouldShowTwitterWithRewardButton() && TrainingController.TrainingCompleted;
		if (twitter != null && twitter.gameObject.activeSelf != flag7)
		{
			twitter.gameObject.SetActive(flag7);
			flag3 = true;
		}
		bool flag8 = TwitterController.TwitterSupported && !TwitterController.IsLoggedIn && Storager.getInt(Defs.IsTwitterLoginRewardaGained, true) == 1 && !CollectOnlyNoShare && TrainingController.TrainingCompleted && ShowLoginsButton && !Device.isPixelGunLow;
		if (twitterNoReward != null && twitterNoReward.gameObject.activeSelf != flag8)
		{
			twitterNoReward.gameObject.SetActive(flag8);
			flag3 = true;
		}
		bool flag9 = (FacebookController.FacebookSupported && (flag5 || flag6)) || (TwitterController.TwitterSupported && (flag7 || flag8));
		if (innerGrid != null && innerGrid.gameObject.activeSelf != flag9)
		{
			innerGrid.gameObject.SetActive(flag9);
			flag4 = true;
		}
		if (innerGrid != null && flag9 && flag3)
		{
			innerGrid.Reposition();
		}
		bool flag10 = TwitterController.TwitterSupported && TwitterController.Instance != null && TwitterController.Instance.CanPostStatusUpdateWithPriority(twitterPriority) && TwitterController.IsLoggedIn;
		bool flag11 = FacebookController.FacebookSupported && FacebookController.sharedController != null && FacebookController.sharedController.CanPostStoryWithPriority(facebookPriority) && FB.IsLoggedIn;
		bool flag12 = !HasReward && !flag10 && !flag11 && !CollectOnlyNoShare;
		if (continueButton != null && continueButton.gameObject.activeSelf != flag12)
		{
			continueButton.gameObject.SetActive(flag12);
		}
		bool flag13 = (HasReward && !flag10 && !flag11) || CollectOnlyNoShare;
		if (collect != null && collect.gameObject.activeSelf != flag13)
		{
			collect.gameObject.SetActive(flag13);
		}
		bool flag14 = !HasReward && (flag10 || flag11) && !CollectOnlyNoShare;
		if (continueAndShare != null && continueAndShare.gameObject.activeSelf != flag14)
		{
			continueAndShare.gameObject.SetActive(flag14);
		}
		bool flag15 = HasReward && (flag10 || flag11) && !CollectOnlyNoShare;
		if (collectAndShare != null && collectAndShare.gameObject.activeSelf != flag15)
		{
			collectAndShare.gameObject.SetActive(flag15);
		}
		if (containerWidget != null && flag)
		{
			if (flag2)
			{
				containerWidget.height = (int)widgetExpanded;
			}
			else if (flag9)
			{
				containerWidget.height = (int)widgetNoConnectToSocial;
			}
			else if (!flag9)
			{
				containerWidget.height = (int)widgetCollapsed;
			}
		}
		if (containerWidget != null && flag4)
		{
			if (flag9)
			{
				if (flag2)
				{
					containerWidget.height = (int)widgetExpanded;
				}
				else
				{
					containerWidget.height = (int)widgetNoConnectToSocial;
				}
			}
			else if (!flag9)
			{
				containerWidget.height = (int)widgetCollapsed;
			}
		}
		if (hideButton != null)
		{
			bool active = flag15 || flag14;
			hideButton.gameObject.SetActive(active);
		}
	}

	public void ShowAuthorizationSucceded()
	{
		ShowAuthorizationResultWindow(true);
	}

	public void ShowAuthorizationFailed()
	{
		ShowAuthorizationResultWindow(false);
	}

	private void ShowAuthorizationResultWindow(bool success)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("NguiWindows/" + ((!success) ? "PanelAuthFailed" : "PanelAuthSucces")));
		gameObject.transform.parent = base.transform;
		Player_move_c.SetLayerRecursively(gameObject, base.gameObject.layer);
		gameObject.transform.localPosition = new Vector3(0f, 0f, -130f);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void HandleFacebookButton()
	{
		ButtonClickSound.TryPlayClick();
		MainMenuController.DoMemoryConsumingTaskInEmptyScene(delegate
		{
			FacebookController.Login(ShowAuthorizationSucceded, ShowAuthorizationFailed, CallerContext);
		}, delegate
		{
			string callerContext = CallerContext;
			FacebookController.Login(null, null, callerContext);
		});
	}

	public void HandleTwitterButton()
	{
		ButtonClickSound.TryPlayClick();
		MainMenuController.DoMemoryConsumingTaskInEmptyScene(delegate
		{
			if (TwitterController.Instance != null)
			{
				TwitterController.Instance.Login(ShowAuthorizationSucceded, ShowAuthorizationFailed, CallerContext);
			}
		}, delegate
		{
			if (TwitterController.Instance != null)
			{
				TwitterController instance = TwitterController.Instance;
				string callerContext = CallerContext;
				instance.Login(null, null, callerContext);
			}
		});
	}

	public void HandleShareToggle(UIToggle toggle)
	{
	}

	private void Update()
	{
		SetButtonPositionsAndActive();
	}

	public void HandleContinueButtonNoHide()
	{
		ButtonClickSound.TryPlayClick();
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
			Action action = shareAction;
			if (action != null)
			{
				action();
				AnalyticsFacade.SendCustomEvent("Virality", new Dictionary<string, object>
				{
					{ "Post Facebook", EventTitle },
					{ "Total Facebook", "Posts" }
				});
			}
		}
	}

	public void HandleContinueButton()
	{
		HandleContinueButtonNoHide();
		Hide();
	}

	public void Hide()
	{
		ButtonClickSound.TryPlayClick();
		Action action = customHide;
		if (action != null)
		{
			action();
			customHide = null;
		}
		base.transform.parent = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void StartGemsStarterAnimation()
	{
		StartCoroutine(base.transform.parent.GetComponent<LevelUpWithOffers>().GemsStarterAnimation());
	}

	public void StartCoinsStarterAnimation()
	{
		StartCoroutine(base.transform.parent.GetComponent<LevelUpWithOffers>().CoinsStarterAnimation());
	}
}

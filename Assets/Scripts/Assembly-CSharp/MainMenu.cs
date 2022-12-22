using System.Collections;
using Prime31;
using Rilisoft;
using UnityEngine;

public sealed class MainMenu : MonoBehaviour
{
	public static MainMenu sharedMenu;

	public GameObject JoysticksUIRoot;

	public static bool BlockInterface;

	public static bool IsAdvertRun;

	private bool isShowDeadMatch;

	private bool isShowCOOP;

	public bool isFirstFrame = true;

	public bool isInappWinOpen;

	private bool musicOld;

	private bool fxOld;

	public Texture inAppFon;

	public GUIStyle puliInApp;

	public GUIStyle healthInApp;

	public GUIStyle pulemetInApp;

	public GUIStyle crystalSwordInapp;

	public GUIStyle elixirInapp;

	private bool showUnlockDialog;

	private bool isPressFullOnMulty;

	private float _timeWhenPurchShown;

	public GameObject skinsManagerPrefab;

	public GameObject weaponManagerPrefab;

	public GUIStyle backBut;

	private ExperienceController expController;

	private AdvertisementController _advertisementController;

	public bool isShowAvard;

	public static readonly string iTunesEnderManID = "811995374";

	private static bool firstEnterLobbyAtThisLaunch = true;

	private bool _skinsMakerQuerySucceeded;

	public static int FontSizeForMessages
	{
		get
		{
			return Mathf.RoundToInt((float)Screen.height * 0.03f);
		}
	}

	public static string RateUsURL
	{
		get
		{
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
				return "https://play.google.com/store/apps/details?id=com.pixel.gun3d&hl=en";
			}
			return Defs2.ApplicationUrl;
		}
	}

	public static float iOSVersion
	{
		get
		{
			float result = -1f;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				string text = SystemInfo.operatingSystem.Replace("iPhone OS ", string.Empty);
				float.TryParse(text.Substring(0, 1), out result);
			}
			return result;
		}
	}

	public static string GetEndermanUrl()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.isEditor)
		{
			return "https://itunes.apple.com/app/apple-store/id811995374?pt=1579002&ct=pgapp&mt=8-";
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://play.google.com/store/apps/details?id=com.slender.android" : "http://www.amazon.com/Pocket-Slenderman-Rising-your-virtual/dp/B00I6IXU5A/ref=sr_1_5?s=mobile-apps&ie=UTF8&qid=1395990920&sr=1-5&keywords=slendy";
		}
		return string.Empty;
	}

	private void completionHandler(string error, object result)
	{
		if (error != null)
		{
			Debug.LogError(error);
		}
		else
		{
			Utils.logObject(result);
		}
	}

	private void Awake()
	{
		Defs.isDaterRegim = false;
		if (firstEnterLobbyAtThisLaunch)
		{
			firstEnterLobbyAtThisLaunch = false;
			GlobalGameController.SetMultiMode();
			return;
		}
		using (new StopwatchLogger("MainMenu.Awake()"))
		{
			GlobalGameController.SetMultiMode();
			if (WeaponManager.sharedManager != null)
			{
				WeaponManager.sharedManager.Reset(0);
			}
			else if (!WeaponManager.sharedManager && (bool)weaponManagerPrefab)
			{
				GameObject gameObject = Object.Instantiate(weaponManagerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.GetComponent<WeaponManager>().Reset(0);
			}
		}
	}

	private IEnumerator WaitForExperienceGuiAndAdd(ExperienceController legacyExperienceController, int addend)
	{
		while (ExpController.Instance == null)
		{
			yield return null;
		}
		legacyExperienceController.addExperience(addend);
	}

	private void Start()
	{
		using (new StopwatchLogger("MainMenu.Start()"))
		{
			sharedMenu = this;
			StoreKitEventListener.State.Mode = "In_main_menu";
			StoreKitEventListener.State.PurchaseKey = "In shop";
			StoreKitEventListener.State.Parameters.Clear();
			if (!FriendsController.sharedController.dataSent)
			{
				FriendsController.sharedController.InitOurInfo();
				FriendsController.sharedController.StartCoroutine(FriendsController.sharedController.WaitForReadyToOperateAndUpdatePlayer());
				FriendsController.sharedController.dataSent = true;
			}
			if (NotificationController.isGetEveryDayMoney)
			{
				isShowAvard = true;
			}
			bool flag = false;
			expController = ExperienceController.sharedController;
			if (expController == null)
			{
				Debug.LogError("MainMenu.Start():    expController == null");
			}
			if (expController != null)
			{
				expController.isMenu = true;
			}
			float coef = Defs.Coef;
			if (expController != null)
			{
				expController.posRanks = new Vector2(21f * Defs.Coef, 21f * Defs.Coef);
			}
			string @string = PlayerPrefs.GetString(Defs.ShouldReoeatActionSett, string.Empty);
			if (@string.Equals(Defs.GoToProfileAction))
			{
				PlayerPrefs.SetString(Defs.ShouldReoeatActionSett, string.Empty);
				PlayerPrefs.Save();
			}
			Storager.setInt(Defs.EarnedCoins, 0, false);
			Invoke("setEnabledGUI", 0.1f);
			ActivityIndicator.IsActiveIndicator = true;
			PlayerPrefs.SetInt("typeConnect__", -1);
			if (!GameObject.FindGameObjectWithTag("SkinsManager") && (bool)skinsManagerPrefab)
			{
				Object.Instantiate(skinsManagerPrefab, Vector3.zero, Quaternion.identity);
			}
			if (!WeaponManager.sharedManager && (bool)weaponManagerPrefab)
			{
				Object.Instantiate(weaponManagerPrefab, Vector3.zero, Quaternion.identity);
			}
			GlobalGameController.ResetParameters();
			GlobalGameController.Score = 0;
			bool flag2 = false;
			if (PlayerPrefs.GetInt(Defs.ShouldEnableShopSN, 0) == 1)
			{
				flag2 = true;
				PlayerPrefs.SetInt(Defs.ShouldEnableShopSN, 0);
				PlayerPrefs.Save();
			}
			if (Tools.RuntimePlatform != RuntimePlatform.MetroPlayerX64 && (Application.platform != RuntimePlatform.Android || Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon) && Defs.EnderManAvailable && !flag2 && !flag && !isShowAvard && PlayerPrefs.GetInt(Defs.ShowEnder_SN, 0) == 1)
			{
				float @float = PlayerPrefs.GetFloat(Defs.TimeFromWhichShowEnder_SN, 0f);
				float num = Switcher.SecondsFrom1970() - @float;
				Debug.Log("diff mainmenu: " + num);
				if (num >= ((!Application.isEditor && !Debug.isDebugBuild) ? 86400f : 0f))
				{
					PlayerPrefs.SetInt(Defs.ShowEnder_SN, 0);
					Object.Instantiate(Resources.Load("Ender") as GameObject);
				}
			}
		}
	}

	private void SetInApp()
	{
		isInappWinOpen = !isInappWinOpen;
		if (expController != null)
		{
			expController.isShowRanks = !isInappWinOpen;
			expController.isMenu = !isInappWinOpen;
		}
		if (isInappWinOpen)
		{
			if (StoreKitEventListener.restoreInProcess)
			{
				ActivityIndicator.IsActiveIndicator = true;
			}
			if (!Defs.isMulti)
			{
				Time.timeScale = 0f;
			}
		}
		else
		{
			ActivityIndicator.IsActiveIndicator = false;
		}
	}

	public static bool SkinsMakerSupproted()
	{
		bool result = BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64;
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
		{
			result = Defs.AndroidEdition != Defs.RuntimeAndroidEdition.GoogleLite;
		}
		return result;
	}

	private void Update()
	{
		float num = ((float)Screen.width - 42f * Defs.Coef - Defs.Coef * (672f + (float)(SkinsMakerSupproted() ? 262 : 0))) / ((!SkinsMakerSupproted()) ? 2f : 3f);
		if (expController != null)
		{
			expController.posRanks = new Vector2(21f * Defs.Coef, 21f * Defs.Coef);
		}
	}

	private void OnDestroy()
	{
		sharedMenu = null;
		if (expController != null)
		{
			expController.isShowRanks = false;
			expController.isMenu = false;
		}
	}
}

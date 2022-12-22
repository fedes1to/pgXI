using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Prime31;
using Rilisoft;
using UnityEngine;

[RequireComponent(typeof(FrameStopwatchScript))]
internal sealed class Switcher : MonoBehaviour
{
	internal const string AbuseMethodKey = "AbuseMethod";

	public static Dictionary<string, int> sceneNameToGameNum;

	public static Dictionary<string, int> counCreateMobsInLevel;

	public static string LoadingInResourcesPath;

	public static string[] loadingNames;

	public GameObject balanceControllerPrefab;

	public GameObject coinsShopPrefab;

	public GameObject nickStackPrefab;

	public GameObject skinsManagerPrefab;

	public GameObject ExperienceControllerPrefab;

	public GameObject weaponManagerPrefab;

	public GameObject experienceGuiPrefab;

	public GameObject bankGuiPrefab;

	public GameObject freeAwardGuiPrefab;

	public GameObject buttonClickSoundPrefab;

	public GameObject faceBookControllerPrefab;

	public GameObject licenseVerificationControllerPrefab;

	public GameObject potionsControllerPrefab;

	public GameObject protocolListGetterPrefab;

	public GameObject updateCheckerPrefab;

	public GameObject promoActionsManagerPrefab;

	public GameObject starterPackManagerPrefab;

	public GameObject tempItemsControllerPrefab;

	public GameObject remotePushNotificationControllerPrefab;

	public GameObject premiumAccountControllerPrefab;

	public GameObject twitterControllerPrefab;

	public GameObject sponsorPayPluginHolderPrefab;

	public GameObject giftControllerPrefab;

	public GameObject disabler;

	public GameObject sceneInfoController;

	private Rect plashkaCoinsRect;

	private Texture fonToDraw;

	private bool _newLaunchingApproach;

	public static Stopwatch timer;

	private static bool _initialAppVersionInitialized;

	private static string _InitialAppVersion;

	public static GameObject comicsSound;

	private float _progress;

	private float oldProgress;

	private static AbuseMetod? _abuseMethod;

	public static string InitialAppVersion
	{
		get
		{
			if (!_initialAppVersionInitialized)
			{
				_InitialAppVersion = PlayerPrefs.GetString(Defs.InitialAppVersionKey);
				_initialAppVersionInitialized = true;
			}
			return _InitialAppVersion;
		}
		private set
		{
			_InitialAppVersion = value;
			_initialAppVersionInitialized = true;
		}
	}

	internal static AbuseMetod AbuseMethod
	{
		get
		{
			if (!_abuseMethod.HasValue)
			{
				_abuseMethod = (AbuseMetod)Storager.getInt("AbuseMethod", false);
			}
			return _abuseMethod.Value;
		}
	}

	static Switcher()
	{
		sceneNameToGameNum = new Dictionary<string, int>();
		counCreateMobsInLevel = new Dictionary<string, int>();
		LoadingInResourcesPath = "LevelLoadings";
		loadingNames = new string[17]
		{
			"Loading_coliseum", "loading_Cementery", "Loading_Maze", "Loading_City", "Loading_Hospital", "Loading_Jail", "Loading_end_world_2", "Loading_Arena", "Loading_Area52", "Loading_Slender",
			"Loading_Hell", "Loading_bloody_farm", "Loading_most", "Loading_school", "Loading_utopia", "Loading_sky", "Loading_winter"
		};
		timer = new Stopwatch();
		_initialAppVersionInitialized = false;
		_InitialAppVersion = string.Empty;
		sceneNameToGameNum.Add("Training", 0);
		sceneNameToGameNum.Add("Cementery", 1);
		sceneNameToGameNum.Add("Maze", 2);
		sceneNameToGameNum.Add("City", 3);
		sceneNameToGameNum.Add("Hospital", 4);
		sceneNameToGameNum.Add("Jail", 5);
		sceneNameToGameNum.Add("Gluk_2", 6);
		sceneNameToGameNum.Add("Arena", 7);
		sceneNameToGameNum.Add("Area52", 8);
		sceneNameToGameNum.Add("Slender", 9);
		sceneNameToGameNum.Add("Castle", 10);
		sceneNameToGameNum.Add("Farm", 11);
		sceneNameToGameNum.Add("Bridge", 12);
		sceneNameToGameNum.Add("School", 13);
		sceneNameToGameNum.Add("Utopia", 14);
		sceneNameToGameNum.Add("Sky_islands", 15);
		sceneNameToGameNum.Add("Winter", 16);
		sceneNameToGameNum.Add("Swamp_campaign3", 17);
		sceneNameToGameNum.Add("Castle_campaign3", 18);
		sceneNameToGameNum.Add("Space_campaign3", 19);
		sceneNameToGameNum.Add("Parkour", 20);
		sceneNameToGameNum.Add("Code_campaign3", 21);
		counCreateMobsInLevel.Add("Farm", 10);
		counCreateMobsInLevel.Add("Cementery", 15);
		counCreateMobsInLevel.Add("City", 20);
		counCreateMobsInLevel.Add("Hospital", 25);
		counCreateMobsInLevel.Add("Bridge", 25);
		counCreateMobsInLevel.Add("Jail", 30);
		counCreateMobsInLevel.Add("Slender", 30);
		counCreateMobsInLevel.Add("Area52", 35);
		counCreateMobsInLevel.Add("School", 35);
		counCreateMobsInLevel.Add("Utopia", 25);
		counCreateMobsInLevel.Add("Maze", 30);
		counCreateMobsInLevel.Add("Sky_islands", 30);
		counCreateMobsInLevel.Add("Winter", 30);
		counCreateMobsInLevel.Add("Castle", 35);
		counCreateMobsInLevel.Add("Gluk_2", 35);
		counCreateMobsInLevel.Add("Swamp_campaign3", 30);
		counCreateMobsInLevel.Add("Castle_campaign3", 35);
		counCreateMobsInLevel.Add("Space_campaign3", 25);
		counCreateMobsInLevel.Add("Parkour", 15);
		counCreateMobsInLevel.Add("Code_campaign3", 35);
	}

	internal static IEnumerable<float> InitializeStorager()
	{
		float progress = 0f;
		if (Application.isEditor)
		{
			if (!PlayerPrefs.HasKey(Defs.initValsInKeychain15))
			{
				Storager.setString(Defs.CapeEquppedSN, Defs.CapeNoneEqupped, false);
				Storager.setString(Defs.HatEquppedSN, Defs.HatNoneEqupped, false);
				yield return progress;
			}
			if (!PlayerPrefs.HasKey(Defs.initValsInKeychain46))
			{
				Storager.setString("MaskEquippedSN", "MaskNoneEquipped", false);
				yield return progress;
			}
		}
		if (!Storager.hasKey(Defs.keysInappBonusGivenkey))
		{
			Storager.setString(Defs.keysInappBonusGivenkey, string.Empty, false);
		}
		if (!Storager.hasKey(Defs.keyInappPresentIDWeaponRedkey))
		{
			Storager.setString(Defs.keyInappBonusStartActionForPresentIDWeaponRedkey, string.Empty, false);
			Storager.setString(Defs.keyInappPresentIDWeaponRedkey, string.Empty, false);
			Storager.setString(Defs.keyInappBonusStartActionForPresentIDPetkey, string.Empty, false);
			Storager.setString(Defs.keyInappPresentIDPetkey, string.Empty, false);
			Storager.setString(Defs.keyInappBonusStartActionForPresentIDGadgetkey, string.Empty, false);
			Storager.setString(Defs.keyInappPresentIDGadgetkey, string.Empty, false);
		}
		if (!Storager.hasKey(Defs.initValsInKeychain15))
		{
			Storager.setInt(Defs.initValsInKeychain15, 0, false);
			Storager.setInt(Defs.LobbyLevelApplied, 1, false);
			Storager.setString(Defs.CapeEquppedSN, Defs.CapeNoneEqupped, false);
			Storager.setString(Defs.HatEquppedSN, Defs.HatNoneEqupped, false);
			Storager.setInt(Defs.IsFirstLaunchFreshInstall, 1, false);
			yield return progress;
		}
		else if (Storager.getInt(Defs.LobbyLevelApplied, false) == 0)
		{
			Storager.setInt(Defs.ShownLobbyLevelSN, 3, false);
		}
		try
		{
			string hat = Storager.getString(Defs.HatEquppedSN, false);
			if (hat != null && (TempItemsController.PriceCoefs.ContainsKey(hat) || Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].Contains(hat)))
			{
				Storager.setString(Defs.HatEquppedSN, Defs.HatNoneEqupped, false);
			}
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogError("Exception in Trying to unequip armor hat or temp armor hat (mistakenly got from gocha as a gift): " + e);
		}
		if (!Storager.hasKey(Defs.IsFirstLaunchFreshInstall))
		{
			Storager.setInt(Defs.IsFirstLaunchFreshInstall, 0, false);
		}
		progress = 0.25f;
		if (Application.isEditor || (Application.platform == RuntimePlatform.IPhonePlayer && UnityEngine.Debug.isDebugBuild) || (Application.platform == RuntimePlatform.IPhonePlayer && !Storager.hasKey(Defs.initValsInKeychain17)))
		{
			Storager.setInt(Defs.initValsInKeychain17, 0, false);
			PlayerPrefs.SetFloat(value: SecondsFrom1970(), key: Defs.TimeFromWhichShowEnder_SN);
		}
		if (Application.isEditor && !PlayerPrefs.HasKey(Defs.initValsInKeychain27))
		{
			Storager.setString(Defs.BootsEquppedSN, Defs.BootsNoneEqupped, false);
		}
		if (!Storager.hasKey(Defs.initValsInKeychain27))
		{
			Storager.setInt(Defs.initValsInKeychain27, 0, false);
			Storager.setString(Defs.BootsEquppedSN, Defs.BootsNoneEqupped, false);
			yield return progress;
		}
		progress = 0.5f;
		yield return progress;
		if (!Storager.hasKey(Defs.initValsInKeychain40))
		{
			Storager.setInt(Defs.initValsInKeychain40, 0, false);
			Storager.setString(Defs.ArmorNewEquppedSN, Defs.ArmorNewNoneEqupped, false);
			Storager.setInt("GrenadeID", 5, false);
			yield return progress;
		}
		if (!Storager.IsInitialized(Defs.initValsInKeychain41))
		{
			Storager.setInt(Defs.initValsInKeychain41, 0, false);
			string hatBought = null;
			string visualHatArmor = null;
			if (Storager.getInt("hat_Almaz_1", false) > 0)
			{
				hatBought = "hat_Army_3";
				Storager.setInt("hat_Almaz_1", 0, false);
				Storager.setInt("hat_Royal_1", 0, false);
				Storager.setInt("hat_Steel_1", 0, false);
				visualHatArmor = "hat_Almaz_1";
				yield return progress;
			}
			else if (Storager.getInt("hat_Royal_1", false) > 0)
			{
				hatBought = "hat_Army_2";
				Storager.setInt("hat_Royal_1", 0, false);
				Storager.setInt("hat_Steel_1", 0, false);
				visualHatArmor = "hat_Royal_1";
				yield return progress;
			}
			else if (Storager.getInt("hat_Steel_1", false) > 0)
			{
				hatBought = "hat_Army_1";
				Storager.setInt("hat_Steel_1", 0, false);
				visualHatArmor = "hat_Steel_1";
				yield return progress;
			}
			if (hatBought != null)
			{
				string hatEquipped = Storager.getString(Defs.HatEquppedSN, false);
				if (hatEquipped.Equals("hat_Almaz_1") || hatEquipped.Equals("hat_Royal_1") || hatEquipped.Equals("hat_Steel_1"))
				{
					Storager.setString(Defs.HatEquppedSN, hatBought, false);
				}
				for (int i = 0; i <= Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(hatBought); i++)
				{
					Storager.setInt(Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0][i], 1, false);
					yield return progress;
				}
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				Storager.setString(Defs.VisualHatArmor, string.Empty, false);
			}
			if (visualHatArmor != null)
			{
				Storager.setString(Defs.VisualHatArmor, visualHatArmor, false);
			}
			if (!Storager.hasKey("LikeID"))
			{
				Storager.setInt("LikeID", 5, false);
			}
			yield return progress;
			string armorBought = null;
			string visualArmor = null;
			if (Storager.getInt("Armor_Almaz_1", false) > 0)
			{
				armorBought = "Armor_Army_3";
				Storager.setInt("Armor_Almaz_1", 0, false);
				Storager.setInt("Armor_Royal_1", 0, false);
				Storager.setInt("Armor_Steel_1", 0, false);
				visualArmor = "Armor_Almaz_1";
				yield return progress;
			}
			else if (Storager.getInt("Armor_Royal_1", false) > 0)
			{
				armorBought = "Armor_Army_2";
				Storager.setInt("Armor_Royal_1", 0, false);
				Storager.setInt("Armor_Steel_1", 0, false);
				visualArmor = "Armor_Royal_1";
				yield return progress;
			}
			else if (Storager.getInt("Armor_Steel_1", false) > 0)
			{
				armorBought = "Armor_Army_1";
				Storager.setInt("Armor_Steel_1", 0, false);
				visualArmor = "Armor_Steel_1";
				yield return progress;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				Storager.setString(Defs.VisualArmor, string.Empty, false);
			}
			if (visualArmor != null)
			{
				Storager.setString(Defs.VisualArmor, visualArmor, false);
			}
			yield return progress;
			if (armorBought != null)
			{
				string armorEquipped = Storager.getString(Defs.ArmorNewEquppedSN, false);
				if (armorEquipped.Equals("Armor_Almaz_1") || armorEquipped.Equals("Armor_Royal_1") || armorEquipped.Equals("Armor_Steel_1"))
				{
					Storager.setString(Defs.ArmorNewEquppedSN, armorBought, false);
					yield return progress;
				}
				for (int j = 0; j <= Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(armorBought); j++)
				{
					Storager.setInt(Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0][j], 1, false);
					yield return progress;
				}
			}
		}
		progress = 0.75f;
		if (!Storager.IsInitialized(Defs.initValsInKeychain43))
		{
			Storager.SetInitialized(Defs.initValsInKeychain43);
			PlayerPrefs.SetString(Defs.StartTimeShowBannersKey, DateTimeOffset.UtcNow.ToString("s"));
			PlayerPrefs.Save();
			yield return progress;
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				Storager.setInt(Defs.NeedTakeMarathonBonus, 0, false);
				Storager.setInt(Defs.NextMarafonBonusIndex, 0, false);
				yield return progress;
			}
		}
		if (!Storager.hasKey(GearManager.MusicBox))
		{
			Storager.setInt(GearManager.MusicBox, 2, false);
			Storager.setInt(GearManager.Wings, 2, false);
			Storager.setInt(GearManager.Bear, 2, false);
			Storager.setInt(GearManager.BigHeadPotion, 2, false);
		}
		Defs.StartTimeShowBannersString = PlayerPrefs.GetString(Defs.StartTimeShowBannersKey, string.Empty);
		UnityEngine.Debug.Log(" StartTimeShowBannersString=" + Defs.StartTimeShowBannersString);
		if (!Storager.IsInitialized(Defs.initValsInKeychain44))
		{
			Storager.SetInitialized(Defs.initValsInKeychain44);
			if (Storager.hasKey(Defs.NextMarafonBonusIndex) && Storager.getInt(Defs.NextMarafonBonusIndex, false) == -1)
			{
				Storager.setInt(Defs.NextMarafonBonusIndex, 0, false);
			}
			yield return progress;
		}
		if (!Storager.IsInitialized(Defs.initValsInKeychain45))
		{
			Storager.SetInitialized(Defs.initValsInKeychain45);
			Storager.setInt(Defs.PremiumEnabledFromServer, 0, false);
			if (Storager.getInt("currentLevel2", true) == 0)
			{
				PlayerPrefs.SetString(Defs.DateOfInstallAppForInAppPurchases041215, DateTime.UtcNow.ToString("s"));
			}
			yield return progress;
		}
		if (!Storager.IsInitialized(Defs.initValsInKeychain46))
		{
			Storager.SetInitialized(Defs.initValsInKeychain46);
			Storager.setString("MaskEquippedSN", "MaskNoneEquipped", false);
			yield return progress;
		}
		if (!Storager.hasKey("Win Count Timestamp"))
		{
			Storager.setString("Win Count Timestamp", "{ \"1970-01-01\": 0 }", false);
		}
		if (!Storager.hasKey("StartTimeShowStarterPack"))
		{
			Storager.setString("StartTimeShowStarterPack", string.Empty, false);
			yield return progress;
		}
		if (!Storager.hasKey("TimeEndStarterPack"))
		{
			Storager.setString("TimeEndStarterPack", string.Empty, false);
			yield return progress;
		}
		if (!Storager.hasKey("NextNumberStarterPack"))
		{
			Storager.setInt("NextNumberStarterPack", 0, false);
			yield return progress;
		}
		if (!Storager.hasKey(Defs.ArmorEquppedSN))
		{
			Storager.setString(Defs.ArmorEquppedSN, Defs.ArmorNoneEqupped, false);
		}
		if (!Storager.hasKey(Defs.ShowSorryWeaponAndArmor))
		{
			Storager.setInt(Defs.ShowSorryWeaponAndArmor, 0, false);
		}
		if (Storager.getInt(Defs.IsFirstLaunchFreshInstall, false) > 0)
		{
			Storager.setInt(Defs.IsFirstLaunchFreshInstall, 0, false);
		}
		if (!Storager.hasKey(Defs.NewbieEventX3StartTime))
		{
			Storager.setString(Defs.NewbieEventX3StartTime, 0L.ToString(), false);
			Storager.setString(Defs.NewbieEventX3StartTimeAdditional, 0L.ToString(), false);
			Storager.setString(Defs.NewbieEventX3LastLoggedTime, 0L.ToString(), false);
			PlayerPrefs.SetInt(Defs.WasNewbieEventX3, 0);
		}
		if (!PlayerPrefs.HasKey(Defs.LastTimeUpdateAvailableShownSN))
		{
			DateTime myDate1 = new DateTime(1970, 1, 9, 0, 0, 0);
			DateTimeOffset _1970 = new DateTimeOffset(myDate1);
			PlayerPrefs.SetString(Defs.LastTimeUpdateAvailableShownSN, _1970.ToString("s"));
			PlayerPrefs.Save();
		}
		string lastTimeUpdateShownString = PlayerPrefs.GetString(Defs.LastTimeUpdateAvailableShownSN);
		DateTimeOffset lastTimeUpdateShown = default(DateTimeOffset);
		if (!DateTimeOffset.TryParse(lastTimeUpdateShownString, out lastTimeUpdateShown) && UnityEngine.Debug.isDebugBuild)
		{
			UnityEngine.Debug.LogWarning("Cannot parse " + lastTimeUpdateShownString);
		}
		if (DateTimeOffset.Now - lastTimeUpdateShown > TimeSpan.FromHours(12.0))
		{
			PlayerPrefs.SetInt(Defs.UpdateAvailableShownTimesSN, 3);
			PlayerPrefs.SetString(Defs.LastTimeUpdateAvailableShownSN, DateTimeOffset.Now.ToString("s"));
			yield return progress;
		}
		float eventX3ShowTimeoutHours = 12f;
		if (!PlayerPrefs.HasKey(Defs.EventX3WindowShownLastTime))
		{
			PlayerPrefs.SetInt(Defs.EventX3WindowShownCount, 1);
			PlayerPrefs.SetString(Defs.EventX3WindowShownLastTime, PromoActionsManager.CurrentUnixTime.ToString());
			yield return progress;
		}
		long eventX3WindowShownLastTime;
		long.TryParse(PlayerPrefs.GetString(Defs.EventX3WindowShownLastTime), out eventX3WindowShownLastTime);
		if (PromoActionsManager.CurrentUnixTime - eventX3WindowShownLastTime > (long)TimeSpan.FromHours(eventX3ShowTimeoutHours).TotalSeconds)
		{
			PlayerPrefs.SetInt(Defs.EventX3WindowShownCount, 1);
			PlayerPrefs.SetString(Defs.EventX3WindowShownLastTime, PromoActionsManager.CurrentUnixTime.ToString());
		}
		PlayerPrefs.Save();
		yield return progress;
		if (!PlayerPrefs.HasKey(Defs.AdvertWindowShownLastTime))
		{
			PlayerPrefs.SetInt(Defs.AdvertWindowShownCount, 3);
			PlayerPrefs.SetString(Defs.AdvertWindowShownLastTime, PromoActionsManager.CurrentUnixTime.ToString());
		}
		long advertWindowShownLastTime;
		long.TryParse(PlayerPrefs.GetString(Defs.AdvertWindowShownLastTime), out advertWindowShownLastTime);
		float advertShowTimeoutHours = ((!Defs.IsDeveloperBuild) ? 12f : (1f / 12f));
		if (PromoActionsManager.CurrentUnixTime - advertWindowShownLastTime > (long)TimeSpan.FromHours(advertShowTimeoutHours).TotalSeconds)
		{
			PlayerPrefs.SetInt(Defs.AdvertWindowShownCount, 3);
			PlayerPrefs.SetString(Defs.AdvertWindowShownLastTime, PromoActionsManager.CurrentUnixTime.ToString());
		}
		yield return progress;
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			if (!Storager.hasKey(Defs.LevelsWhereGetCoinS))
			{
				CoinBonus.SetLevelsWhereGotBonus(string.Empty, VirtualCurrencyBonusType.Coin);
			}
			if (!Storager.hasKey(Defs.LevelsWhereGotGems))
			{
				CoinBonus.SetLevelsWhereGotBonus("[]", VirtualCurrencyBonusType.Gem);
			}
			if (!Storager.hasKey(Defs.RatingFlag))
			{
				Storager.setInt(Defs.RatingDeathmatch, 0, false);
				Storager.setInt(Defs.RatingTeamBattle, 0, false);
				Storager.setInt(Defs.RatingHunger, 0, false);
				Storager.setInt(Defs.RatingFlag, 0, false);
			}
			if (!Storager.hasKey(Defs.RatingCapturePoint))
			{
				Storager.setInt(Defs.RatingCapturePoint, 0, false);
			}
		}
		PlayerPrefs.Save();
		yield return 1f;
	}

	private static double Hypot(double x, double y)
	{
		x = Math.Abs(x);
		y = Math.Abs(y);
		double num = Math.Max(x, y);
		double num2 = Math.Min(x, y) / num;
		return num * Math.Sqrt(1.0 + num2 * num2);
	}

	private IEnumerator ParseConfigsCoroutine()
	{
		float start2 = Time.realtimeSinceStartup;
		ScopeLogger scopeLogger = new ScopeLogger("Switcher.Start()", "Parsing advert config", Defs.IsDeveloperBuild);
		try
		{
			if (Storager.hasKey("abTestAdvertConfigKey"))
			{
				FriendsController.ParseABTestAdvertConfig(false);
			}
			else
			{
				Storager.setString("abTestAdvertConfigKey", string.Empty, false);
			}
		}
		finally
		{
			scopeLogger.Dispose();
		}
		if (Time.realtimeSinceStartup - start2 > 1f / 60f)
		{
			start2 = Time.realtimeSinceStartup;
			yield return null;
		}
	}

	private IEnumerator Start()
	{
		oldProgress = 0f;
		UnityEngine.Debug.LogFormat("> Switcher.Start(): {0:f3}, {1}", Time.realtimeSinceStartup, Time.frameCount);
		yield return StartCoroutine(ParseConfigsCoroutine());
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			PlayComicsSound();
		}
		UnityEngine.Debug.Log("Switcher.Start() > InitializeSwitcher()");
		bool armyArmor1ComesFromCloud = false;
		foreach (float item in InitializeSwitcher(delegate
		{
			armyArmor1ComesFromCloud = true;
		}))
		{
			float step2 = item;
			timer.Reset();
			timer.Start();
			oldProgress = _progress;
			ActivityIndicator.LoadingProgress = _progress;
			yield return step2;
		}
		ScopeLogger scopeLogger = new ScopeLogger("Switcher.Start()", "Loading main menu asynchronously", Defs.IsDeveloperBuild);
		try
		{
			foreach (float item2 in LoadMainMenu(armyArmor1ComesFromCloud))
			{
				float step = item2;
				timer.Reset();
				timer.Start();
				oldProgress = _progress;
				ActivityIndicator.LoadingProgress = _progress;
				yield return step;
			}
		}
		finally
		{
			scopeLogger.Dispose();
		}
		UnityEngine.Debug.LogFormat("< Switcher.Start(): {0:f3}, {1}", Time.realtimeSinceStartup, Time.frameCount);
	}

	public static string LoadingCupTexture(int number)
	{
		return "loading_cups_" + number + ((!Device.isRetinaAndStrong) ? string.Empty : "-hd");
	}

	public IEnumerable<float> InitializeSwitcher(Action setArmorArmy1ComesFromCloud = null)
	{
		UnityEngine.Debug.Log("> InitializeSwitcher()");
		Stopwatch _stopwatch = new Stopwatch();
		ProgressBounds bounds = new ProgressBounds();
		Action logBounds = delegate
		{
		};
		Action<string> log = delegate
		{
		};
		Func<float, long, string> format = delegate(float progress, long ms)
		{
			int num = Mathf.RoundToInt(progress * 100f);
			return string.Format(" << {0}%: {1}", num, ms);
		};
		InGameTimeKeeper.Instance.Initialize();
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			string bgTextureName = LoadingCupTexture(1);
			fonToDraw = Resources.Load<Texture>(bgTextureName);
			string legendLocalization = LocalizationStore.Get("Key_1925");
			string legendText = ((!("Key_1925" == legendLocalization)) ? legendLocalization : "PLEASE REBOOT YOUR DEVICE IF FROZEN.");
			ActivityIndicator.instance.legendLabel.text = legendText;
			ActivityIndicator.instance.legendLabel.gameObject.SetActive(true);
		}
		else
		{
			string bgTextureName2 = ConnectSceneNGUIController.MainLoadingTexture();
			fonToDraw = Resources.Load<Texture>(bgTextureName2);
		}
		ActivityIndicator.SetLoadingFon(fonToDraw);
		ActivityIndicator.IsShowWindowLoading = true;
		ActivityIndicator.instance.panelProgress.SetActive(true);
		bounds.SetBounds(0f, 0.09f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		if (!PlayerPrefs.HasKey("First Launch (Advertisement)"))
		{
			PlayerPrefs.SetString("First Launch (Advertisement)", DateTimeOffset.UtcNow.ToString("s"));
		}
		if (!PlayerPrefs.HasKey(Defs.InitialAppVersionKey))
		{
			if (!PlayerPrefs.HasKey("NamePlayer"))
			{
				PlayerPrefs.SetString(Defs.InitialAppVersionKey, GlobalGameController.AppVersion);
			}
			else
			{
				PlayerPrefs.SetString(Defs.InitialAppVersionKey, "1.0.0");
			}
		}
		InitialAppVersion = PlayerPrefs.GetString(Defs.InitialAppVersionKey);
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
		{
			AbstractManager.initialize(typeof(GoogleIABManager));
		}
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
		{
			try
			{
				if (Defs.IsDeveloperBuild)
				{
					UnityEngine.Debug.Log("Switcher: Trying to initialize Google Play Games...");
				}
				GpgFacade.Instance.Initialize();
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				UnityEngine.Debug.LogException(ex);
			}
		}
		_progress = bounds.Clamp(_progress + 0.005f);
		yield return _progress;
		if (sponsorPayPluginHolderPrefab != null)
		{
			UnityEngine.Object.Instantiate(sponsorPayPluginHolderPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.005f);
		yield return _progress;
		UnityEngine.Object.Instantiate(balanceControllerPrefab);
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		GlobalGameController.LeftHanded = PlayerPrefs.GetInt(Defs.LeftHandedSN, 1) == 1;
		if (!PlayerPrefs.HasKey(Defs.SwitchingWeaponsSwipeRegimSN))
		{
			double diagonalInPixels = Hypot(Screen.width, Screen.height);
			int switchingWeaponMode = 0;
			if (Screen.dpi > 0f)
			{
				double diagonalInInches = diagonalInPixels / (double)Screen.dpi;
				if (UnityEngine.Debug.isDebugBuild)
				{
					UnityEngine.Debug.Log(string.Format("Device dpi: {0},    diagonal: {1} px ({2}\")", Screen.dpi, diagonalInPixels, diagonalInInches));
				}
				switchingWeaponMode = ((!(diagonalInInches < 6.8)) ? 1 : 0);
			}
			else if (UnityEngine.Debug.isDebugBuild)
			{
				UnityEngine.Debug.Log(string.Format("Device dpi: {0},    diagonal: {1} px", Screen.dpi, diagonalInPixels));
			}
			PlayerPrefs.SetInt(Defs.SwitchingWeaponsSwipeRegimSN, switchingWeaponMode);
		}
		GlobalGameController.switchingWeaponSwipe = PlayerPrefs.GetInt(Defs.SwitchingWeaponsSwipeRegimSN, 0) == 1;
		string oldV = Load.LoadString("keyOldVersion");
		string curV = GlobalGameController.AppVersion;
		if (oldV != curV)
		{
			PlayerPrefs.SetInt("countSessionDayOnStartCorrentVersion", PlayerPrefs.GetInt(Defs.SessionDayNumberKey, 1));
			ReviewController.IsSendReview = false;
			ReviewController.ExistReviewForSend = false;
			ReviewController.CheckActiveReview();
			Save.SaveString("keyOldVersion", curV);
		}
		Tools.AddSessionNumber();
		CoroutineRunner.Instance.StartCoroutine(AnalyticsStuff.WaitInitializationThenLogGameDayCountCoroutine());
		if (!Storager.hasKey(Defs.WeaponsGotInCampaign))
		{
			Storager.setString(Defs.WeaponsGotInCampaign, string.Empty, false);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		Screen.sleepTimeout = 180;
		if (SkinsController.sharedController == null && (bool)skinsManagerPrefab)
		{
			UnityEngine.Object.Instantiate(skinsManagerPrefab, Vector3.zero, Quaternion.identity);
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			foreach (float item3 in SkinsController.sharedController.LoadSkinsInTexture())
			{
				float step5 = item3;
				yield return _progress;
			}
		}
		if (PromoActionsManager.sharedManager == null && promoActionsManagerPrefab != null)
		{
			UnityEngine.Object.Instantiate(promoActionsManagerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (nickStackPrefab == null)
		{
			UnityEngine.Debug.LogError("Switcher.InitializeSwitcher():    nickStackPrefab == null");
		}
		else if (NickLabelStack.sharedStack == null)
		{
			UnityEngine.Object nicklabelStack = UnityEngine.Object.Instantiate(nickStackPrefab, Vector3.zero, Quaternion.identity);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (sceneInfoController == null)
		{
			UnityEngine.Debug.LogError("Switcher.InitializeSwitcher():    sceneInfoController == null");
		}
		else
		{
			UnityEngine.Object.Instantiate(sceneInfoController, Vector3.zero, Quaternion.identity);
		}
		if (ExperienceControllerPrefab == null)
		{
			UnityEngine.Debug.LogError("Switcher.InitializeSwitcher():    ExperienceControllerPrefab == null");
		}
		else if (ExperienceController.sharedController == null)
		{
			UnityEngine.Object experienceController = UnityEngine.Object.Instantiate(ExperienceControllerPrefab, Vector3.zero, Quaternion.identity);
			_progress = bounds.Lerp(_progress, 0.6f);
			yield return _progress;
			foreach (float item4 in ExperienceController.sharedController.InitController())
			{
				float step4 = item4;
				_progress = bounds.Clamp(_progress + 0.01f);
				yield return _progress;
			}
		}
		bounds.SetBounds(0.1f, 0.19f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		if (experienceGuiPrefab != null)
		{
			if (ExpController.Instance == null)
			{
				UnityEngine.Object expGui = UnityEngine.Object.Instantiate(experienceGuiPrefab, Vector3.zero, Quaternion.identity);
				UnityEngine.Object.DontDestroyOnLoad(expGui);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("ExperienceGuiPrefab == null");
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (bankGuiPrefab != null)
		{
			if (BankController.Instance == null)
			{
				UnityEngine.Object bankGui = UnityEngine.Object.Instantiate(bankGuiPrefab, Vector3.zero, Quaternion.identity);
				UnityEngine.Object.DontDestroyOnLoad(bankGui);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("BankGuiPrefab == null");
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (freeAwardGuiPrefab != null)
		{
			if (FreeAwardController.Instance == null)
			{
				UnityEngine.Object freeAwardGui = UnityEngine.Object.Instantiate(freeAwardGuiPrefab, Vector3.zero, Quaternion.identity);
				UnityEngine.Object.DontDestroyOnLoad(freeAwardGui);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("freeAwardGuiPrefab == null");
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		AnalyticsFacade.Initialize();
		PersistentCache persistentCache = PersistentCache.Instance;
		if (Defs.IsDeveloperBuild)
		{
			UnityEngine.Debug.LogFormat("Persistent cache: '{0}'", persistentCache.PersistentDataPath);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (RemotePushNotificationController.Instance == null && (bool)remotePushNotificationControllerPrefab)
		{
			UnityEngine.Object.Instantiate(remotePushNotificationControllerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (ShopNGUIController.sharedShop == null)
		{
			ResourceRequest shopTask = Resources.LoadAsync("ShopNGUI");
			while (!shopTask.isDone)
			{
				yield return _progress;
			}
			UnityEngine.Object shopP = shopTask.asset;
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			UnityEngine.Object.Instantiate(shopP, Vector3.zero, Quaternion.identity);
		}
		bounds.SetBounds(0.2f, 0.29f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (Storager.getInt("InitBanerAwardCompition", false) == 0)
		{
			if (RatingSystem.instance.currentLeague == RatingSystem.RatingLeague.Adamant)
			{
				TournamentAvailableBannerWindow.CanShow = true;
				int threshold = RatingSystem.instance.TrophiesSeasonThreshold;
				if (RatingSystem.instance.currentRating > threshold)
				{
					int compensate = RatingSystem.instance.currentRating - threshold;
					RatingSystem.instance.negativeRating += compensate;
					RatingSystem.instance.UpdateLeagueEvent(null, null);
				}
			}
			Storager.setInt("InitBanerAwardCompition", 1, false);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (FriendsController.sharedController == null)
		{
			ResourceRequest friendsControllerTask = Resources.LoadAsync("FriendsController");
			while (!friendsControllerTask.isDone)
			{
				yield return _progress;
			}
			UnityEngine.Object fcp = friendsControllerTask.asset;
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			UnityEngine.Object.Instantiate(fcp, Vector3.zero, Quaternion.identity);
			yield return _progress;
			foreach (float item5 in FriendsController.sharedController.InitController())
			{
				float step3 = item5;
				yield return _progress;
			}
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		string token = null;
		Storager.Initialize(!token.IsNullOrEmpty());
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			fonToDraw = Resources.Load<Texture>(LoadingCupTexture(2));
			foreach (float item6 in ActivityIndicator.instance.ReplaceLoadingFon(fonToDraw, 0.3f))
			{
				float step2 = item6;
				yield return _progress;
			}
			ActivityIndicator.instance.legendLabel.text = LocalizationStore.Get("Key_1926");
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		_stopwatch.Start();
		foreach (float item7 in InitializeStorager())
		{
			float storagerInitProgress = item7;
			if (_stopwatch.ElapsedMilliseconds > 100)
			{
				_stopwatch.Reset();
				_stopwatch.Start();
				yield return _progress;
			}
		}
		_stopwatch.Reset();
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (disabler != null)
		{
			UnityEngine.Object.Instantiate(disabler);
		}
		bounds.SetBounds(0.3f, 0.39f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			List<string> weaponsForWhichSetRememberedTier = new List<string>();
			bool armorArmy1Comes;
			Storager.SynchronizeIosWithCloud(ref weaponsForWhichSetRememberedTier, out armorArmy1Comes);
			if (armorArmy1Comes && setArmorArmy1ComesFromCloud != null)
			{
				setArmorArmy1ComesFromCloud();
			}
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			Storager.SyncWithCloud(Defs.SkinsMakerInProfileBought);
			Storager.SyncWithCloud(Defs.code010110_Key);
			Storager.SyncWithCloud(Defs.smallAsAntKey);
			Storager.SyncWithCloud(Defs.UnderwaterKey);
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			ShopNGUIController.CategoryNames[] firstCats = new ShopNGUIController.CategoryNames[2]
			{
				ShopNGUIController.CategoryNames.HatsCategory,
				ShopNGUIController.CategoryNames.CapesCategory
			};
			ShopNGUIController.CategoryNames[] array = firstCats;
			foreach (ShopNGUIController.CategoryNames cat in array)
			{
				foreach (List<string> ll in Wear.wear[cat])
				{
					foreach (string item in ll)
					{
						Storager.SyncWithCloud(item);
					}
				}
			}
			yield return _progress;
			IEnumerable<ShopNGUIController.CategoryNames> secondCats = Wear.wear.Keys.Except(firstCats);
			foreach (ShopNGUIController.CategoryNames cat2 in secondCats)
			{
				foreach (List<string> ll2 in Wear.wear[cat2])
				{
					foreach (string item2 in ll2)
					{
						Storager.SyncWithCloud(item2);
					}
				}
			}
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			SkinItem[] skinItems = SkinsController.sharedController.skinItems;
			foreach (SkinItem _skinInfo in skinItems)
			{
				Storager.SyncWithCloud(_skinInfo.name);
			}
			yield return _progress;
			foreach (string weaponSkin in WeaponSkinsManager.SkinIds)
			{
				Storager.SyncWithCloud(weaponSkin);
			}
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			int levelBefore = ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
			WeaponManager.RefreshExpControllers();
			ExperienceController.SendAnalyticsForLevelsFromCloud(levelBefore);
			try
			{
				WeaponManager.SetRememberedTiersForWeaponsComesFromCloud(weaponsForWhichSetRememberedTier);
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError("SetRememberedTiersForWeaponsComesFromCloud exception: " + e);
			}
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			Dictionary<string, GadgetInfo>.KeyCollection gadgetIds = GadgetsInfo.info.Keys;
			foreach (string gadgetId in gadgetIds)
			{
				Storager.SyncWithCloud(gadgetId);
			}
			List<string> canBuyWeaponStorageIds = ItemDb.GetCanBuyWeaponStorageIds(true);
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			_stopwatch.Start();
			for (int j = 0; j < canBuyWeaponStorageIds.Count; j++)
			{
				string storageId = canBuyWeaponStorageIds[j];
				if (!string.IsNullOrEmpty(storageId))
				{
					Storager.SyncWithCloud(storageId);
				}
				if (j % 100 == 0)
				{
					_progress = bounds.Clamp(_progress + 0.01f);
					yield return _progress;
					_stopwatch.Reset();
					_stopwatch.Start();
				}
				if (_stopwatch.ElapsedMilliseconds > 100)
				{
					yield return _progress;
					_stopwatch.Reset();
					_stopwatch.Start();
				}
			}
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Started);
			AnalyticsStuff.TrySendOnceToAppsFlyer("first_launch");
			AnalyticsStuff.TrySendOnceToFacebook("app_launch_first", null, null);
		}
		bounds.SetBounds(0.4f, 0.49f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		CountMoneyForRemovedGear();
		_progress = bounds.Clamp(_progress + 0.001f);
		yield return _progress;
		CountMoneyForArmorHats();
		if (Storager.hasKey(Defs.HatEquppedSN) && Storager.getString(Defs.HatEquppedSN, false) == "hat_ManiacMask")
		{
			Storager.setString(Defs.HatEquppedSN, ShopNGUIController.NoneEquippedForWearCategory(ShopNGUIController.CategoryNames.HatsCategory), false);
			Storager.setString("MaskEquippedSN", "hat_ManiacMask", false);
		}
		_progress = bounds.Clamp(_progress + 0.001f);
		yield return _progress;
		WeaponManager.ActualizeWeaponsForCampaignProgress();
		_progress = bounds.Clamp(0.41f);
		yield return _progress;
		if (coinsShop.thisScript == null && (bool)coinsShopPrefab)
		{
			UnityEngine.Object.Instantiate(coinsShopPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (FacebookController.sharedController == null && FacebookController.FacebookSupported && faceBookControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(faceBookControllerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (ButtonClickSound.Instance == null && buttonClickSoundPrefab != null)
		{
			UnityEngine.Object.Instantiate(buttonClickSoundPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.005f);
		yield return _progress;
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		bool needInstantiateLicenseVerification = false;
		if (Defs.IsDeveloperBuild)
		{
			UnityEngine.Debug.LogFormat("Loading {0:P1} > Instantiate License Verification Controller: {1}", _progress, needInstantiateLicenseVerification);
		}
		if (needInstantiateLicenseVerification)
		{
			UnityEngine.Object.Instantiate(licenseVerificationControllerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		bool needInstantiateTempItems = TempItemsController.sharedController == null && tempItemsControllerPrefab != null;
		if (Defs.IsDeveloperBuild)
		{
			UnityEngine.Debug.LogFormat("Loading {0:P1} > Instantiate Temp Items: {1}", _progress, needInstantiateTempItems);
		}
		if (needInstantiateTempItems)
		{
			UnityEngine.Object.Instantiate(tempItemsControllerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (updateCheckerPrefab != null)
		{
			UnityEngine.Object.Instantiate(updateCheckerPrefab);
		}
		bounds.SetBounds(0.5f, 0.52f);
		logBounds();
		_progress = bounds.LowerBound;
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			yield return _progress;
			fonToDraw = Resources.Load<Texture>(LoadingCupTexture(3));
			foreach (float item8 in ActivityIndicator.instance.ReplaceLoadingFon(fonToDraw, 0.3f))
			{
				float step = item8;
				yield return _progress;
			}
			ActivityIndicator.instance.legendLabel.text = LocalizationStore.Get("Key_1927");
		}
		yield return _progress;
		_progress = bounds.Clamp(_progress + 0.01f);
		if (Defs.IsDeveloperBuild)
		{
			UnityEngine.Debug.LogFormat("Loading {0:P1} > Instantiate TwitterController.", _progress);
		}
		if (TwitterController.Instance == null && twitterControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(twitterControllerPrefab);
		}
		yield return _progress;
		_progress = bounds.Clamp(_progress + 0.01f);
		WeaponManager wm = null;
		ScopeLogger scopeLogger = new ScopeLogger("Loading " + _progress.ToString("P1", CultureInfo.InvariantCulture), "Instantiate WeaponManager.", Defs.IsDeveloperBuild);
		try
		{
			GameObject o = (GameObject)UnityEngine.Object.Instantiate(weaponManagerPrefab, Vector3.zero, Quaternion.identity);
			wm = o.GetComponent<WeaponManager>();
		}
		finally
		{
			scopeLogger.Dispose();
		}
		bounds.SetBounds(0.52f, 0.88f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		if (wm != null)
		{
			int i = 0;
			while (!wm.Initialized)
			{
				_progress = bounds.Clamp(_progress + 0.01f);
				yield return _progress;
				if (Launcher.UsingNewLauncher)
				{
					yield return -1f;
				}
				i++;
			}
		}
		yield return _progress;
		bounds.SetBounds(0.89f, 0.99f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		SetUpPhoton(MiscAppsMenu.Instance.misc);
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		CheckHugeUpgrade();
		PerformEssentialInitialization("Coins", AbuseMetod.Coins);
		PerformEssentialInitialization("GemsCurrency", AbuseMetod.Gems);
		PerformExpendablesInitialization();
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		CampaignProgress.OpenNewBoxIfPossible();
		if (StarterPackController.Get == null && starterPackManagerPrefab != null)
		{
			UnityEngine.Object.Instantiate(starterPackManagerPrefab);
		}
		if (PotionsController.sharedController == null && potionsControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(potionsControllerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		QuestSystem.Instance.Initialize();
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (PremiumAccountController.Instance == null && premiumAccountControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(premiumAccountControllerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			Storager.SyncWithCloud("PayingUser");
			Storager.SyncWithCloud(Defs.IsFacebookLoginRewardaGained);
			Storager.SyncWithCloud(Defs.IsTwitterLoginRewardaGained);
			foreach (string gochaGun in WeaponManager.GotchaGuns)
			{
				Storager.SyncWithCloud(gochaGun);
			}
		}
		if (GiftController.Instance == null && giftControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(giftControllerPrefab);
		}
		Screen.sleepTimeout = 180;
		_progress = bounds.Clamp(_progress + 0.01f);
		while (!Singleton<AchievementsManager>.Instance.IsReady)
		{
			yield return _progress;
		}
		_progress = 0.96f;
		yield return _progress;
	}

	private void SetUpPhoton(HiddenSettings settings)
	{
		string text = SelectPhotonAppId(settings);
		if (Defs.IsDeveloperBuild)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("appId", text);
			dictionary.Add("defaultAppId", PhotonNetwork.PhotonServerSettings.AppID);
			Dictionary<string, string> dictionary2 = dictionary;
		}
		//PhotonNetwork.PhotonServerSettings.AppID = text;
	}

	private static string SelectPhotonAppId(HiddenSettings settings)
	{
		byte[] bytes = Convert.FromBase64String(settings.PhotonAppIdSignaturePad);
		byte[] array = Convert.FromBase64String(settings.PhotonAppIdSignatureEncoded);
		byte[] signatureHash = AndroidSystem.Instance.GetSignatureHash();
		byte[] bytes2 = Enumerable.Repeat(signatureHash, int.MaxValue).SelectMany((byte[] bs) => bs).Take(array.Length)
			.ToArray();
		byte[] array2 = new byte[36];
		new BitArray(bytes).Xor(new BitArray(array)).Xor(new BitArray(bytes2)).CopyTo(array2, 0);
		return Encoding.UTF8.GetString(array2, 0, array2.Length);
	}

	public static void PlayComicsSound()
	{
		if (!(comicsSound != null))
		{
			GameObject gameObject = Resources.Load<GameObject>("BackgroundMusic/Background_Comics");
			if (gameObject == null)
			{
				UnityEngine.Debug.LogWarning("ComicsSoundPrefab is null.");
				return;
			}
			comicsSound = UnityEngine.Object.Instantiate(gameObject);
			UnityEngine.Object.DontDestroyOnLoad(comicsSound);
		}
	}

	private static void CheckHugeUpgrade()
	{
		bool flag = Storager.hasKey("Coins");
		bool flag2 = Storager.hasKey(Defs.ArmorNewEquppedSN);
		if (flag && !flag2)
		{
			AppendAbuseMethod(AbuseMetod.UpgradeFromVulnerableVersion);
			UnityEngine.Debug.LogError("Upgrade tampering detected: " + AbuseMethod);
		}
	}

	private static void PerformEssentialInitialization(string currencyKey, AbuseMetod abuseMethod)
	{
		if (!Storager.hasKey(currencyKey))
		{
			return;
		}
		int @int = Storager.getInt(currencyKey, false);
		if (DigestStorager.Instance.ContainsKey(currencyKey))
		{
			if (!DigestStorager.Instance.Verify(currencyKey, @int))
			{
				AppendAbuseMethod(abuseMethod);
				UnityEngine.Debug.LogError("Currency tampering detected: " + AbuseMethod);
			}
		}
		else
		{
			DigestStorager.Instance.Set(currencyKey, @int);
		}
	}

	[Obsolete("Because of issues with CryptoPlayerPrefs")]
	private static void PerformWeaponInitialization()
	{
		IEnumerable<string> source = WeaponManager.storeIDtoDefsSNMapping.Values.Where((string w) => Storager.getInt(w, false) == 1);
		int value = source.Count();
		if (DigestStorager.Instance.ContainsKey("WeaponsCount"))
		{
			if (!DigestStorager.Instance.Verify("WeaponsCount", value))
			{
				AppendAbuseMethod(AbuseMetod.Weapons);
				UnityEngine.Debug.LogError("Weapon tampering detected: " + AbuseMethod);
			}
		}
		else
		{
			DigestStorager.Instance.Set("WeaponsCount", value);
		}
	}

	private static void PerformExpendablesInitialization()
	{
		string[] source = new string[4]
		{
			GearManager.InvisibilityPotion,
			GearManager.Jetpack,
			GearManager.Turret,
			GearManager.Mech
		};
		byte[] value = source.SelectMany((string key) => BitConverter.GetBytes(Storager.getInt(key, false))).ToArray();
		if (DigestStorager.Instance.ContainsKey("ExpendablesCount"))
		{
			if (!DigestStorager.Instance.Verify("ExpendablesCount", value))
			{
				AppendAbuseMethod(AbuseMetod.Expendables);
				UnityEngine.Debug.LogError("Expendables tampering detected: " + AbuseMethod);
			}
		}
		else
		{
			DigestStorager.Instance.Set("ExpendablesCount", value);
		}
	}

	private static void ClearProgress()
	{
	}

	public IEnumerable<float> LoadMainMenu(bool armyArmor1ComesFromCloud = false)
	{
		ScopeLogger scopeLogger = new ScopeLogger("Switcher.LoadMainMenu()", Defs.IsDeveloperBuild);
		try
		{
			if ((ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel > 1) || armyArmor1ComesFromCloud)
			{
				if (!TrainingController.TrainingCompleted)
				{
					TrainingController.OnGetProgress();
				}
				else if (Storager.getInt("Training.ShouldRemoveNoviceArmorInShopKey", false) == 1 && armyArmor1ComesFromCloud)
				{
					if (ShopNGUIController.NoviceArmorAvailable)
					{
						ShopNGUIController.UnequipCurrentWearInCategory(ShopNGUIController.CategoryNames.ArmorCategory, false);
						ShopNGUIController.ProvideItem(ShopNGUIController.CategoryNames.ArmorCategory, "Armor_Army_1", 1, false, 0, null, null, true, false, false);
					}
					Storager.setInt("Training.ShouldRemoveNoviceArmorInShopKey", 0, false);
				}
			}
			if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
			{
				Defs.isFlag = false;
				Defs.isCOOP = false;
				Defs.isMulti = false;
				Defs.isHunger = false;
				Defs.isCompany = false;
				Defs.IsSurvival = false;
				Defs.isCapturePoints = false;
				GlobalGameController.Score = 0;
				WeaponManager.sharedManager.CurrentWeaponIndex = WeaponManager.sharedManager.playerWeapons.OfType<Weapon>().ToList().FindIndex((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == 1);
			}
			string sceneName = ((TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != 0) ? DetermineSceneName() : Defs.TrainingSceneName);
			_progress = 0.96f;
			yield return _progress;
			AsyncOperation loadLevelTask = Singleton<SceneLoader>.Instance.LoadSceneAsync(sceneName);
			CoroutineRunner.Instance.StartCoroutine(WaitLoadSceneAsyncOperation(loadLevelTask, sceneName, 0.96f));
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}

	private IEnumerator WaitLoadSceneAsyncOperation(AsyncOperation loadSceneAsyncOperation, string sceneName, float leftBound)
	{
		ScopeLogger scopeLogger = new ScopeLogger("Switcher.WaitLoadSceneAsyncOperation(): " + sceneName, Defs.IsDeveloperBuild);
		try
		{
			while (!loadSceneAsyncOperation.isDone)
			{
				_progress = leftBound + loadSceneAsyncOperation.progress / 50f;
				yield return _progress;
			}
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}

	private static bool IsWeaponBought(string weaponTag)
	{
		string value;
		string value2;
		return WeaponManager.tagToStoreIDMapping.TryGetValue(weaponTag, out value) && value != null && WeaponManager.storeIDtoDefsSNMapping.TryGetValue(value, out value2) && value2 != null && Storager.hasKey(value2) && Storager.getInt(value2, true) > 0;
	}

	private static void CountMoneyForRemovedGear()
	{
		Storager.hasKey(Defs.GemsGivenRemovedGear);
		if (Storager.getInt(Defs.GemsGivenRemovedGear, false) != 0)
		{
			return;
		}
		int num = 0;
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add(GearManager.Turret, 5);
		dictionary.Add(GearManager.Mech, 7);
		dictionary.Add(GearManager.InvisibilityPotion, 3);
		dictionary.Add(GearManager.Jetpack, 4);
		Dictionary<string, int> dictionary2 = dictionary;
		foreach (string key in dictionary2.Keys)
		{
			num += Storager.getInt(key, false) * dictionary2[key];
		}
		Storager.setInt(Defs.GemsGivenRemovedGear, 1, false);
		foreach (string key2 in dictionary2.Keys)
		{
			Storager.setInt(key2, 0, false);
		}
	}

	private static void CountMoneyForGunsFrom831To901()
	{
		Storager.hasKey(Defs.MoneyGiven831to901);
		Storager.SyncWithCloud(Defs.MoneyGiven831to901);
		Storager.hasKey(Defs.Weapons831to901);
		if (Storager.getInt(Defs.Weapons831to901, false) != 0)
		{
			return;
		}
		bool flag = Storager.getInt(Defs.MoneyGiven831to901, true) == 0;
		int num = 0;
		int num2 = 0;
		if (flag)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add(WeaponTags.CrossbowTag, 120);
			dictionary.Add(WeaponTags.CrystalCrossbowTag, 155);
			dictionary.Add(WeaponTags.SteelCrossbowTag, 120);
			dictionary.Add(WeaponTags.Bow_3_Tag, 185);
			dictionary.Add(WeaponTags.WoodenBowTag, 100);
			dictionary.Add(WeaponTags.Staff2Tag, 200);
			dictionary.Add(WeaponTags.Staff_3_Tag, 235);
			Dictionary<string, int> dictionary2 = dictionary;
			foreach (KeyValuePair<string, int> item in dictionary2)
			{
				string key = item.Key;
				int value = item.Value;
				if (IsWeaponBought(key))
				{
					num += value;
				}
			}
			dictionary = new Dictionary<string, int>();
			dictionary.Add(WeaponTags.AutoShotgun_Tag, 255);
			dictionary.Add(WeaponTags.TwoRevolvers_Tag, 267);
			dictionary.Add(WeaponTags.TwoBolters_Tag, 249);
			dictionary.Add(WeaponTags.SnowballGun_Tag, 281);
			Dictionary<string, int> dictionary3 = dictionary;
			foreach (KeyValuePair<string, int> item2 in dictionary3)
			{
				string key2 = item2.Key;
				int value2 = item2.Value;
				if (IsWeaponBought(key2))
				{
					num2 += value2;
				}
			}
			dictionary = new Dictionary<string, int>();
			dictionary.Add("cape_EliteCrafter", 50);
			dictionary.Add("cape_Archimage", 65);
			dictionary.Add("cape_BloodyDemon", 50);
			dictionary.Add("cape_SkeletonLord", 75);
			dictionary.Add("cape_RoyalKnight", 65);
			Dictionary<string, int> dictionary4 = dictionary;
			foreach (KeyValuePair<string, int> item3 in dictionary4)
			{
				string key3 = item3.Key;
				int value3 = item3.Value;
				if (Storager.hasKey(key3) && Storager.getInt(key3, false) != 0)
				{
					num += value3;
				}
			}
			dictionary = new Dictionary<string, int>();
			dictionary.Add("boots_gray", 50);
			dictionary.Add("boots_red", 50);
			dictionary.Add("boots_black", 100);
			dictionary.Add("boots_blue", 50);
			dictionary.Add("boots_green", 75);
			Dictionary<string, int> dictionary5 = dictionary;
			foreach (KeyValuePair<string, int> item4 in dictionary5)
			{
				string key4 = item4.Key;
				int value4 = item4.Value;
				if (Storager.hasKey(key4) && Storager.getInt(key4, false) != 0)
				{
					num += value4;
				}
			}
		}
		Storager.setInt(Defs.Weapons831to901, 1, false);
		Storager.setInt(Defs.MoneyGiven831to901, 1, true);
	}

	private static void CountMoneyForArmorHats()
	{
		Storager.hasKey("MoneyGivenRemovedArmorHat");
		Storager.SyncWithCloud("MoneyGivenRemovedArmorHat");
		Storager.hasKey("RemovedArmorHatMethodExecuted");
		if (Storager.getInt("RemovedArmorHatMethodExecuted", false) != 0)
		{
			return;
		}
		bool flag = Storager.getInt("MoneyGivenRemovedArmorHat", true) == 0;
		int num = 0;
		if (flag)
		{
			foreach (string item2 in Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0])
			{
				if (Storager.getInt(item2, true) > 0)
				{
					num += VirtualCurrencyHelper.Price(item2).Price;
				}
			}
		}
		Storager.hasKey(Defs.HatEquppedSN);
		string item = Storager.getString(Defs.HatEquppedSN, false) ?? string.Empty;
		if (Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].Contains(item))
		{
			Storager.setString(Defs.HatEquppedSN, ShopNGUIController.NoneEquippedForWearCategory(ShopNGUIController.CategoryNames.HatsCategory), false);
		}
		Storager.setInt("RemovedArmorHatMethodExecuted", 1, false);
		Storager.setInt("MoneyGivenRemovedArmorHat", 1, true);
	}

	public static float SecondsFrom1970()
	{
		DateTime dateTime = new DateTime(1970, 1, 9, 0, 0, 0);
		DateTime now = DateTime.Now;
		return (float)(now - dateTime).TotalSeconds;
	}

	private void OnDestroy()
	{
		ActivityIndicator.IsShowWindowLoading = false;
	}

	private static string DetermineSceneName()
	{
		switch (GlobalGameController.currentLevel)
		{
		case -1:
			return Defs.MainMenuScene;
		case 0:
			return "Cementery";
		case 1:
			return "Maze";
		case 2:
			return "City";
		case 3:
			return "Hospital";
		case 4:
			return "Jail";
		case 5:
			return "Gluk_2";
		case 6:
			return "Arena";
		case 7:
			return "Area52";
		case 101:
			return "Training";
		case 8:
			return "Slender";
		case 9:
			return "Castle";
		default:
			return Defs.MainMenuScene;
		}
	}

	internal static void AppendAbuseMethod(AbuseMetod f)
	{
		_abuseMethod = AbuseMethod | f;
		AbuseMetod? abuseMethod = _abuseMethod;
		Storager.setInt("AbuseMethod", (int)abuseMethod.Value, false);
	}
}

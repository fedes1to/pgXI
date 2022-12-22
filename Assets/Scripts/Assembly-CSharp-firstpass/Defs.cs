using System.Collections.Generic;
using UnityEngine;

public sealed class Defs
{
	public enum GameSecondFireButtonMode
	{
		Sniper,
		On,
		Off
	}

	public enum WeaponIndex
	{
		Grenade = 1000,
		Turret
	}

	public enum DisconectGameType
	{
		Exit,
		Reconnect,
		RandomGameInHunger,
		SelectNewMap,
		RandomGameInDuel
	}

	public enum ABTestCohortsType
	{
		NONE,
		A,
		B,
		SKIP
	}

	public enum RuntimeAndroidEdition
	{
		None,
		Amazon,
		GoogleLite
	}

	public const string ConnectSceneName = "ConnectScene";

	public const string NoviceArmorUsedKey = "Training.NoviceArmorUsedKey";

	public const string ShouldRemoveNoviceArmorInShopKey = "Training.ShouldRemoveNoviceArmorInShopKey";

	public const string GoogleSignInDeniedKey = "GoogleSignInDenied";

	public const string SuperIncubatorId = "Eggs.SuperIncubatorId";

	public const string DefaultGadgetKey = "GadgetsInfo.DefaultGadgetKey";

	public const string EquppedPetSN = "EquppedPetSN";

	public const string CampaignChooseBoxSceneName = "CampaignChooseBox";

	public const string KillRateKey = "KillRateKeyStatistics";

	public const string keyOldVersion = "keyOldVersion";

	public const string countSessionDayOnStartCorrentVersion = "countSessionDayOnStartCorrentVersion";

	public const string MoneyGivenRemovedArmorHat = "MoneyGivenRemovedArmorHat";

	public const string RemovedArmorHatMethodExecuted = "RemovedArmorHatMethodExecuted";

	public const string GrenadeID = "GrenadeID";

	public const string LikeID = "LikeID";

	public const string WeaponLike = "WeaponLike";

	public const string keyGameTotalKills = "keyGameTotalKills";

	public const string keyGameDeath = "keyGameDeath";

	public const string keyGameShoot = "keyGameShoot";

	public const string keyGameHit = "keyGameHit";

	public const string keyCountLikes = "keyCountLikes";

	public const string abTestBalansConfigKey = "abTestBalansConfig2Key";

	public const string abTestAdvertConfigKey = "abTestAdvertConfigKey";

	public const string ratingSystemConfigKey = "rSCKeyV2";

	public const string SniperModeWSSN = "WeaponManager.SniperModeWSSN";

	public const string KnifesModeWSSN = "WeaponManager.KnifesModeWSSN";

	public const string MaskEquippedSN = "MaskEquippedSN";

	public const string MaskNoneEquipped = "MaskNoneEquipped";

	public const string Coins = "Coins";

	public const string Gems = "GemsCurrency";

	public const string payingUserKey = "PayingUser";

	public const string LastPaymentTimeKey = "Last Payment Time";

	public const string LastPaymentTimeAdvertisementKey = "Last Payment Time (Advertisement)";

	public const string WinCountTimestampKey = "Win Count Timestamp";

	public const string WeaponPopularityKey = "Statistics.WeaponPopularity";

	public const string WeaponPopularityForTierKey = "Statistics.WeaponPopularityForTier";

	public const string WeaponPopularityTimestampKey = "Statistics.WeaponPopularityTimestamp";

	public const string WeaponPopularityForTierTimestampKey = "Statistics.WeaponPopularityForTierTimestamp";

	public const string ArmorPopularityKey = "Statistics.ArmorPopularity";

	public const string ArmorPopularityForTierKey = "Statistics.ArmorPopularityForTier";

	public const string ArmorPopularityForLevelKey = "Statistics.ArmorPopularityForLevel";

	public const string ArmorPopularityTimestampKey = "Statistics.ArmorPopularityTimestamp";

	public const string ArmorPopularityForTierTimestampKey = "Statistics.ArmorPopularityForTierTimestamp";

	public const string ArmorPopularityForLevelTimestampKey = "Statistics.ArmorPopularityForLevelTimestamp";

	public const string TimeInModeKeyPrefix = "Statistics.TimeInMode.Level";

	public const string RoundsInModeKeyPrefix = "Statistics.RoundsInMode.Level";

	public const string ExpInModeKeyPrefix = "Statistics.ExpInMode.Level";

	public const string WantToResetKeychain = "WantToResetKeychain";

	public const string StartTimeStarterPack = "StartTimeShowStarterPack";

	public const string EndTimeStarterPack = "TimeEndStarterPack";

	public const string NextNumberStarterPack = "NextNumberStarterPack";

	public const string LastTimeShowStarterPack = "LastTimeShowStarterPack";

	public const string CountShownStarterPack = "CountShownStarterPack";

	public const string CountShownGunForLogin = "FacebookController.CountShownGunForLogin";

	public const string PendingGooglePlayGamesSync = "PendingGooglePlayGamesSync";

	public const int EVENT_X3_SHOW_COUNT = 3;

	public const string FirstLaunchAdvertisementKey = "First Launch (Advertisement)";

	public const string IsLeaderboardsOpened = "Leaderboards.opened";

	public const string DaysOfValorShownCount = "DaysOfValorShownCount";

	public const string LastTimeShowDaysOfValor = "LastTimeShowDaysOfValor";

	public const string StartTimePremiumAccount = "StartTimePremiumAccount";

	public const string EndTimePremiumAccount = "EndTimePremiumAccount";

	public const string BuyHistoryPremiumAccount = "BuyHistoryPremiumAccount";

	public const string LastLoggedTimePremiumAccount = "LastLoggedTimePremiumAccount";

	public const string CachedFriendsJoinClickList = "CachedFriendsJoinClickList";

	public const int ConnectFacebookGemsReward = 10;

	public const int ConnectTwitterGemsReward = 10;

	public const string LockedSkinName = "super_socialman";

	public const float BotHpBarShowTime = 2f;

	public const int ratingCountForDuelMode = 1200;

	public static string LastSendKillRateTimeKey;

	public static string StrongDeviceDev;

	public static string TrafficForwardingShowAnalyticsSent;

	public static string DateOfInstallAppForInAppPurchases041215;

	public static int CoinsForTraining;

	public static int GemsForTraining;

	public static int ExpForTraining;

	public static string GemsGivenRemovedGear;

	public static string LastTimeShowSocialGun;

	public static string ShownRewardWindowForCape;

	public static string ShownRewardWindowForSkin;

	public static string keyInappBonusActionkey;

	public static string keysInappBonusGivenkey;

	public static string keyInappPresentIDWeaponRedkey;

	public static string keyInappBonusStartActionForPresentIDWeaponRedkey;

	public static string keyInappPresentIDPetkey;

	public static string keyInappBonusStartActionForPresentIDPetkey;

	public static string keyInappPresentIDGadgetkey;

	public static string keyInappBonusStartActionForPresentIDGadgetkey;

	public static string DaterWSSN;

	public static string SmileMessageSuffix;

	public static string IsFacebookLoginRewardaGained;

	public static string FacebookRewardGainStarted;

	public static string IsTwitterLoginRewardaGained;

	public static string TwitterRewardGainStarted;

	public static bool ResetTrainingInDevBuild;

	public static bool useSqlLobby;

	public static string keyTestCountGetGift;

	public static string BuyGiftKey;

	public static bool isTouchControlSmoothDump;

	public static bool localTimeInsteadServerTime;

	public static readonly string initValsInKeychain43;

	public static readonly string initValsInKeychain44;

	public static readonly string initValsInKeychain45;

	public static readonly string initValsInKeychain46;

	public static bool isMouseControl;

	public static bool isRegimVidosDebug;

	private static bool _isActivABTestBuffSystem;

	private static bool _isInitActivABTestBuffSystem;

	private static string nonActivABTestBuffSystemKey;

	private static ABTestCohortsType _cohortABTestAdvert;

	private static bool _isInitCohortABTestAdvert;

	private static string cohortABTestAdvertKey;

	public static readonly string MoneyGiven831to901;

	public static string GotCoinsForTraining;

	public static DisconectGameType typeDisconnectGame;

	public static GameSecondFireButtonMode gameSecondFireButtonMode;

	public static int ZoomButtonX;

	public static int ZoomButtonY;

	public static int ReloadButtonX;

	public static int ReloadButtonY;

	public static int JumpButtonX;

	public static int JumpButtonY;

	public static int FireButtonX;

	public static int FireButtonY;

	public static int JoyStickX;

	public static int JoyStickY;

	public static int GrenadeX;

	public static int GrenadeY;

	public static int FireButton2X;

	public static int FireButton2Y;

	public static string VisualHatArmor;

	public static string VisualArmor;

	public static string GadgetContentFolder;

	private static bool _isEnableLocalInviteFromFriends;

	private static bool _isInitEnableLocalInviteFromFriends;

	private static string _enableLocalInviteFromFriendsKey;

	private static bool _isEnableRemoteInviteFromFriends;

	private static bool _isInitEnableRemoteInviteFromFriends;

	private static string _enableRemoteInviteFromFriendsKey;

	public static string RatingDeathmatch;

	public static string RatingTeamBattle;

	public static string RatingHunger;

	public static string RatingFlag;

	public static string RatingCapturePoint;

	public static string RatingDuel;

	public static bool isDaterRegim;

	public static int LogoWidth;

	public static int LogoHeight;

	public static string[] SurvivalMaps;

	public static int CurrentSurvMapIndex;

	public static float FreezerSlowdownTime;

	private static bool _initializedJoystickParams;

	private static bool _isJumpAndShootButtonOn;

	private static bool _isInitJumpAndShootButtonOn;

	public static bool isShowUserAgrement;

	public static int maxCountFriend;

	public static int maxMemberClanCount;

	public static float timeUpdateFriendInfo;

	public static float timeUpdateOnlineInGame;

	public static float timeUpdateInfoInProfile;

	public static float timeBlockRefreshFriendDate;

	public static float timeUpdateLeaderboardIfNullResponce;

	public static float timeUpdateStartCheckIfNullResponce;

	public static float timeWaitLoadPossibleFriends;

	public static float pauseUpdateLeaderboard;

	public static float timeUpdatePixelbookInfo;

	public static float timeUpdateNews;

	public static int historyPrivateMessageLength;

	public static float timeUpdateServerTime;

	public static ABTestCohortsType abTestBalansCohort;

	private static bool? _isAbTestBalansCohortActual;

	public static string abTestBalansCohortName;

	public static string bigPorogString;

	public static string smallPorogString;

	public static string friendsSceneName;

	public static int ammoInGamePanelPrice;

	public static int healthInGamePanelPrice;

	public static int ClansPrice;

	public static int ProfileFromFriends;

	public static string ServerIp;

	public static bool isMulti;

	public static bool isInet;

	public static bool isCOOP;

	public static bool isCompany;

	public static bool isFlag;

	public static bool isHunger;

	public static bool isGameFromFriends;

	public static bool isGameFromClans;

	public static bool isCapturePoints;

	public static bool isDuel;

	public static readonly string PixelGunAppID;

	public static readonly string AppStoreURL;

	public static readonly string SupportMail;

	public static bool EnderManAvailable;

	public static bool isSoundMusic;

	public static bool isSoundFX;

	public static float BottomOffs;

	public static Dictionary<string, int> filterMaps;

	private static readonly Dictionary<string, int> _premiumMaps;

	public static int NumberOfElixirs;

	public static bool isGrenateFireEnable;

	public static bool isZooming;

	public static bool isJetpackEnabled;

	public static bool isJump;

	public static float GoToProfileShopInterval;

	public static readonly string InvertCamSN;

	public static List<GameObject> players;

	public static string PromSceneName;

	public static string _3_shotgun_2;

	public static string _3_shotgun_3;

	public static string flower_2;

	public static string flower_3;

	public static string gravity_2;

	public static string gravity_3;

	public static string grenade_launcher_3;

	public static string revolver_2_2;

	public static string revolver_2_3;

	public static string scythe_3;

	public static string plazma_2;

	public static string plazma_3;

	public static string plazma_pistol_2;

	public static string plazma_pistol_3;

	public static string railgun_2;

	public static string railgun_3;

	public static string Razer_3;

	public static string tesla_3;

	public static string Flamethrower_3;

	public static string FreezeGun_0;

	public static string svd_3;

	public static string barret_3;

	public static string minigun_3;

	public static string LightSword_3;

	public static string Sword_2_3;

	public static string Staff_3;

	public static string DragonGun;

	public static string Bow_3;

	public static string Bazooka_1_3;

	public static string Bazooka_2_1;

	public static string Bazooka_2_3;

	public static string m79_2;

	public static string m79_3;

	public static string m32_1_2;

	public static string Red_Stone_3;

	public static string XM8_1;

	public static string PumpkinGun_1;

	public static string XM8_2;

	public static string XM8_3;

	public static string PumpkinGun_2;

	public static string PumpkinGun_5;

	private static float _touchPressurePower;

	private static int _isUse3DTouch;

	private static int _isUseJump3DTouch;

	private static int _isUseShoot3DTouch;

	public static readonly string Weapons800to801;

	public static readonly string Weapons831to901;

	public static readonly string Update_AddSniperCateogryKey;

	public static readonly string FixWeapons911;

	public static readonly string ReturnAlienGun930;

	public static int diffGame;

	public static bool IsSurvival;

	public static string StartTimeShowBannersString;

	private static int _countReturnInConnectScene;

	private static bool _countReturnInConnectSceneInit;

	public static bool showTableInNetworkStartTable;

	public static bool showNickTableInNetworkStartTable;

	public static bool isTurretWeapon;

	private static float? _sensitivity;

	private static bool? _isChatOn;

	public static int inComingMessagesCounter;

	public static HashSet<string> unimportantRPCList;

	public static bool inRespawnWindow;

	public static readonly string IsFirstLaunchFreshInstall;

	public static readonly string NewbieEventX3StartTime;

	public static readonly string NewbieEventX3StartTimeAdditional;

	public static readonly string NewbieEventX3LastLoggedTime;

	public static readonly string WasNewbieEventX3;

	public static Dictionary<string, string> bootsMaterialDict;

	public static string ShownLobbyLevelSN
	{
		get
		{
			return "ShownLobbyLevelSN";
		}
	}

	public static string PremiumEnabledFromServer
	{
		get
		{
			return "PremiumEnabledFromServer";
		}
	}

	public static string AllCurrencyBought
	{
		get
		{
			return "AllCurrencyBought";
		}
	}

	public static string LobbyLevelApplied
	{
		get
		{
			return "LobbyLevelApplied";
		}
	}

	public static string LastTimeTempItemsSuspended
	{
		get
		{
			return "LastTimeTempItemsSuspended";
		}
	}

	public static string TempItemsDictionaryKey
	{
		get
		{
			return "TempItemsDictionaryKey";
		}
	}

	public static string initValsInKeychain15
	{
		get
		{
			return "_initValsInKeychain15_";
		}
	}

	public static string initValsInKeychain17
	{
		get
		{
			return "initValsInKeychain17";
		}
	}

	public static string initValsInKeychain27
	{
		get
		{
			return "initValsInKeychain27";
		}
	}

	public static string initValsInKeychain40
	{
		get
		{
			return "initValsInKeychain40";
		}
	}

	public static string initValsInKeychain41
	{
		get
		{
			return "initValsInKeychain41";
		}
	}

	public static bool isActivABTestBuffSystem
	{
		get
		{
			if (!_isInitActivABTestBuffSystem)
			{
				_isActivABTestBuffSystem = PlayerPrefs.GetInt(nonActivABTestBuffSystemKey, 1) == 0;
				_isInitActivABTestBuffSystem = true;
			}
			return _isActivABTestBuffSystem;
		}
		set
		{
			_isActivABTestBuffSystem = value;
			_isInitActivABTestBuffSystem = true;
			PlayerPrefs.SetInt(nonActivABTestBuffSystemKey, (!value) ? 1 : 0);
		}
	}

	public static ABTestCohortsType cohortABTestAdvert
	{
		get
		{
			if (!_isInitCohortABTestAdvert)
			{
				_cohortABTestAdvert = (ABTestCohortsType)PlayerPrefs.GetInt(cohortABTestAdvertKey, 0);
				_isInitCohortABTestAdvert = true;
			}
			return _cohortABTestAdvert;
		}
		set
		{
			_cohortABTestAdvert = value;
			_isInitCohortABTestAdvert = true;
			PlayerPrefs.SetInt(cohortABTestAdvertKey, (int)_cohortABTestAdvert);
		}
	}

	public static string TierAfter8_3_0Key
	{
		get
		{
			return "TierAfter8_3_0Key";
		}
	}

	public static string InnerWeaponsFolder
	{
		get
		{
			return "WeaponSystem/Inner";
		}
	}

	public static string InnerWeapons_Suffix
	{
		get
		{
			return "_Inner";
		}
	}

	public static bool touchPressureSupported
	{
		get
		{
			return Input.touchPressureSupported;
		}
	}

	public static bool isEnableInviteFromFriends
	{
		get
		{
			return isEnableLocalInviteFromFriends || _isEnableRemoteInviteFromFriends;
		}
		set
		{
			isEnableLocalInviteFromFriends = value;
			isEnableRemoteInviteFromFriends = value;
		}
	}

	public static bool isEnableLocalInviteFromFriends
	{
		get
		{
			if (!_isInitEnableLocalInviteFromFriends)
			{
				_isEnableLocalInviteFromFriends = PlayerPrefs.GetInt(_enableLocalInviteFromFriendsKey, 1) == 1;
				_isInitEnableLocalInviteFromFriends = true;
			}
			return _isEnableLocalInviteFromFriends;
		}
		set
		{
			_isEnableLocalInviteFromFriends = value;
			_isInitEnableLocalInviteFromFriends = true;
			PlayerPrefs.SetInt(_enableLocalInviteFromFriendsKey, value ? 1 : 0);
		}
	}

	public static bool isEnableRemoteInviteFromFriends
	{
		get
		{
			if (!_isInitEnableRemoteInviteFromFriends)
			{
				_isEnableRemoteInviteFromFriends = PlayerPrefs.GetInt(_enableRemoteInviteFromFriendsKey, 1) == 1;
				_isInitEnableRemoteInviteFromFriends = true;
			}
			return _isEnableRemoteInviteFromFriends;
		}
		set
		{
			_isEnableRemoteInviteFromFriends = value;
			_isInitEnableRemoteInviteFromFriends = true;
			PlayerPrefs.SetInt(_enableRemoteInviteFromFriendsKey, value ? 1 : 0);
		}
	}

	public static bool isJumpAndShootButtonOn
	{
		get
		{
			if (!_isInitJumpAndShootButtonOn)
			{
				_isJumpAndShootButtonOn = !touchPressureSupported || PlayerPrefs.GetInt("isJumpAndShootButtonOn", 1) == 1;
				_isInitJumpAndShootButtonOn = true;
			}
			return _isJumpAndShootButtonOn;
		}
		set
		{
			_isJumpAndShootButtonOn = value;
			_isInitJumpAndShootButtonOn = true;
			PlayerPrefs.SetInt("isJumpAndShootButtonOn", value ? 1 : 0);
		}
	}

	public static float ScreenDiagonal
	{
		get
		{
			return Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height) / Screen.dpi;
		}
	}

	public static bool isABTestBalansNoneSkip
	{
		get
		{
			return PlayerPrefs.GetInt("NoneSkipABTestBalans", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("NoneSkipABTestBalans", value ? 1 : 0);
		}
	}

	public static bool isABTestBalansCohortActual
	{
		get
		{
			bool? isAbTestBalansCohortActual = _isAbTestBalansCohortActual;
			if (isAbTestBalansCohortActual.HasValue)
			{
				return _isAbTestBalansCohortActual == true;
			}
			_isAbTestBalansCohortActual = PlayerPrefs.GetInt("ABTCA", 0) == 1;
			return _isAbTestBalansCohortActual == true;
		}
		set
		{
			_isAbTestBalansCohortActual = value;
			PlayerPrefs.SetInt("ABTCA", (_isAbTestBalansCohortActual == true) ? 1 : 0);
		}
	}

	public static IDictionary<string, int> PremiumMaps
	{
		get
		{
			return _premiumMaps;
		}
	}

	public static float HalfLength
	{
		get
		{
			return 17f;
		}
	}

	public static string ShowEnder_SN
	{
		get
		{
			return "ShowEnder_SN";
		}
	}

	public static string TimeFromWhichShowEnder_SN
	{
		get
		{
			return "TimeFromWhichShowEnder_SN";
		}
	}

	public static string CustomTextureName
	{
		get
		{
			return "cape_CustomTexture";
		}
	}

	public static int CustomCapeTextureWidth
	{
		get
		{
			return 12;
		}
	}

	public static int CustomCapeTextureHeight
	{
		get
		{
			return 16;
		}
	}

	public static string LeftHandedSN
	{
		get
		{
			return "LeftHandedSN";
		}
	}

	public static string SwitchingWeaponsSwipeRegimSN
	{
		get
		{
			return "SwitchingWeaponsSwipeRegimSN";
		}
	}

	public static string CampaignWSSN
	{
		get
		{
			return "CampaignWSSN";
		}
	}

	public static string MultiplayerWSSN
	{
		get
		{
			return "MultiplayerWSSN";
		}
	}

	public static string ReplaceSkins_1_SN
	{
		get
		{
			return "ReplaceSkins_1_SN";
		}
	}

	public static string CapeEquppedSN
	{
		get
		{
			return "CapeEquppedSN";
		}
	}

	public static string HatEquppedSN
	{
		get
		{
			return "HatEquppedSN";
		}
	}

	public static string CapeNoneEqupped
	{
		get
		{
			return "cape_NoneEquipped";
		}
	}

	public static string HatNoneEqupped
	{
		get
		{
			return "hat_NoneEquipped";
		}
	}

	public static string BootsEquppedSN
	{
		get
		{
			return "BootsEquppedSN";
		}
	}

	public static string BootsNoneEqupped
	{
		get
		{
			return "boots_NoneEquipped";
		}
	}

	public static string ArmorEquppedSN
	{
		get
		{
			return "ArmorEquppedSN";
		}
	}

	public static string ArmorNoneEqupped
	{
		get
		{
			return "__no_armor";
		}
	}

	public static string ArmorNewEquppedSN
	{
		get
		{
			return "ArmorNewEquppedSN";
		}
	}

	public static string ArmorNewNoneEqupped
	{
		get
		{
			return "__no_armor_NEW";
		}
	}

	public static string AmmoBoughtSN
	{
		get
		{
			return "AmmoBoughtSN";
		}
	}

	public static string LastTimeUpdateAvailableShownSN
	{
		get
		{
			return "LastTimeUpdateAvailableShownSN";
		}
	}

	public static string UpdateAvailableShownTimesSN
	{
		get
		{
			return "UpdateAvailableShownTimesSN";
		}
	}

	public static string EventX3WindowShownLastTime
	{
		get
		{
			return "EventX3WindowShownLastTime";
		}
	}

	public static string CurrentLanguage
	{
		get
		{
			return "CurrentLanguage";
		}
	}

	public static string EventX3WindowShownCount
	{
		get
		{
			return "EventX3WindowShownCount";
		}
	}

	public static string AdvertWindowShownLastTime
	{
		get
		{
			return "AdvertWindowShownLastTime";
		}
	}

	public static string AdvertWindowShownCount
	{
		get
		{
			return "AdvertWindowShownCount";
		}
	}

	public static string SurvSkinsPath
	{
		get
		{
			return "EnemySkins/Survival";
		}
	}

	public static string TrainingCompleted_4_4_Sett
	{
		get
		{
			return "TrainingCompleted_4_4_Sett";
		}
	}

	public static string BassCannonSN
	{
		get
		{
			return "BassCannonSN";
		}
	}

	public static string ShouldEnableShopSN
	{
		get
		{
			return "ShouldEnableShopSN";
		}
	}

	public static string TrainingSceneName
	{
		get
		{
			return "Training";
		}
	}

	public static string CapesDir
	{
		get
		{
			return "Capes";
		}
	}

	public static string HatsDir
	{
		get
		{
			return "Hats";
		}
	}

	public static string BootsDir
	{
		get
		{
			return "Boots";
		}
	}

	public static string ArtLevsS
	{
		get
		{
			return "ArtLevsS";
		}
	}

	public static string ArtBoxS
	{
		get
		{
			return "ArtBoxS";
		}
	}

	public static string BestScoreSett
	{
		get
		{
			return "BestScoreSett";
		}
	}

	public static string SurvivalScoreSett
	{
		get
		{
			return "SurvivalScoreSett";
		}
	}

	public static string InAppBoughtSett
	{
		get
		{
			return "BigAmmoPackBought";
		}
	}

	public static string CurrentWeaponSett
	{
		get
		{
			return "CurrentWeapon";
		}
	}

	public static string MinerWeaponSett
	{
		get
		{
			return "MinerWeaponSett";
		}
	}

	public static string SwordSett
	{
		get
		{
			return "SwordSett";
		}
	}

	public static int ScoreForSurplusAmmo
	{
		get
		{
			return 50;
		}
	}

	public static string ShownNewWeaponsSN
	{
		get
		{
			return "ShownNewWeaponsSN";
		}
	}

	public static string LevelsWhereGetCoinS
	{
		get
		{
			return "LevelsWhereGetCoinS";
		}
	}

	public static string LevelsWhereGotGems
	{
		get
		{
			return "Bonus.LevelsWhereGotGems";
		}
	}

	public static string NumberOfElixirsSett
	{
		get
		{
			return "NumberOfElixirsSett";
		}
	}

	public static string WeaponsGotInCampaign
	{
		get
		{
			return "WeaponsGotInCampaign";
		}
	}

	public static float Coef
	{
		get
		{
			return (float)Screen.height / 768f;
		}
	}

	public static string SkinEditorMode
	{
		get
		{
			return "SkinEditorMode";
		}
	}

	public static string MultSkinsDirectoryName
	{
		get
		{
			return "Multiplayer Skins";
		}
	}

	public static string katana_SN
	{
		get
		{
			return "katana_SN";
		}
	}

	public static string katana_2_SN
	{
		get
		{
			return "katana_2_SN";
		}
	}

	public static string katana_3_SN
	{
		get
		{
			return "katana_3_SN";
		}
	}

	public static string AK74_SN
	{
		get
		{
			return "AK74_SN";
		}
	}

	public static string AK74_2_SN
	{
		get
		{
			return "AK74_2_SN";
		}
	}

	public static string AK74_3_SN
	{
		get
		{
			return "AK74_3_SN";
		}
	}

	public static string FreezeGun_SN
	{
		get
		{
			return "FreezeGun_SN";
		}
	}

	public static string CombatRifleSett
	{
		get
		{
			return "CombatRifleSett";
		}
	}

	public static string m_16_3_Sett
	{
		get
		{
			return "m_16_3_Sett";
		}
	}

	public static string m_16_4_Sett
	{
		get
		{
			return "m_16_4_Sett";
		}
	}

	public static string GoldenEagleSett
	{
		get
		{
			return "GoldenEagleSett";
		}
	}

	public static string SparklyBlasterSN
	{
		get
		{
			return "SparklyBlasterSN";
		}
	}

	public static string CherryGunSN
	{
		get
		{
			return "CherryGunSN";
		}
	}

	public static string MagicBowSett
	{
		get
		{
			return "MagicBowSett";
		}
	}

	public static string FlowePowerSN
	{
		get
		{
			return "FlowePowerSN";
		}
	}

	public static string BuddySN
	{
		get
		{
			return "BuddySN";
		}
	}

	public static string AUGSN
	{
		get
		{
			return "AUGSett";
		}
	}

	public static string AUG_2SN
	{
		get
		{
			return "AUG_2SN";
		}
	}

	public static string RazerSN
	{
		get
		{
			return "RazerSN";
		}
	}

	public static string Razer_2SN
	{
		get
		{
			return "Razer_2SN";
		}
	}

	public static string SPASSett
	{
		get
		{
			return "SPASSett";
		}
	}

	public static string GoldenAxeSett
	{
		get
		{
			return "GoldenAxeSett";
		}
	}

	public static string ChainsawS
	{
		get
		{
			return "ChainsawS";
		}
	}

	public static string FAMASS
	{
		get
		{
			return "FAMASS";
		}
	}

	public static string GlockSett
	{
		get
		{
			return "GlockSett";
		}
	}

	public static string ScytheSN
	{
		get
		{
			return "ScytheSN";
		}
	}

	public static string Scythe_2_SN
	{
		get
		{
			return "Scythe_2_SN";
		}
	}

	public static string ShovelSN
	{
		get
		{
			return "ShovelSN";
		}
	}

	public static string Sword_2_SN
	{
		get
		{
			return "Sword_2_SN";
		}
	}

	public static string HammerSN
	{
		get
		{
			return "HammerSN";
		}
	}

	public static string StaffSN
	{
		get
		{
			return "StaffSN";
		}
	}

	public static string CrystalSPASSN
	{
		get
		{
			return "CrystalSPASSN";
		}
	}

	public static string CrystalGlockSN
	{
		get
		{
			return "CrystalGlockSN";
		}
	}

	public static string LaserRifleSN
	{
		get
		{
			return "LaserRifleSN";
		}
	}

	public static string LightSwordSN
	{
		get
		{
			return "LightSwordSN";
		}
	}

	public static string BerettaSN
	{
		get
		{
			return "BerettaSN";
		}
	}

	public static string Beretta_2_SN
	{
		get
		{
			return "Beretta_2_SN";
		}
	}

	public static string MaceSN
	{
		get
		{
			return "MaceSN";
		}
	}

	public static string MinigunSN
	{
		get
		{
			return "MinigunSN";
		}
	}

	public static string MauserSN
	{
		get
		{
			return "MauserSN";
		}
	}

	public static string ShmaiserSN
	{
		get
		{
			return "ShmaiserSN";
		}
	}

	public static string ThompsonSN
	{
		get
		{
			return "ThompsonSN";
		}
	}

	public static string Thompson_2SN
	{
		get
		{
			return "Thompson_2SN";
		}
	}

	public static string CrossbowSN
	{
		get
		{
			return "CrossbowSN";
		}
	}

	public static string plazmaSN
	{
		get
		{
			return "plazmaSN";
		}
	}

	public static string plazma_pistol_SN
	{
		get
		{
			return "plazma_pistol_SN";
		}
	}

	public static string Tesla_2SN
	{
		get
		{
			return "Tesla_2SN";
		}
	}

	public static string Bazooka_3SN
	{
		get
		{
			return "Bazooka_3SN";
		}
	}

	public static string GravigunSN
	{
		get
		{
			return "GravigunSN";
		}
	}

	public static string GoldenPickSN
	{
		get
		{
			return "GoldenPickSN";
		}
	}

	public static string TreeSN
	{
		get
		{
			return "TreeSN";
		}
	}

	public static string Tree_2_SN
	{
		get
		{
			return "Tree_2_SN";
		}
	}

	public static string FireAxeSN
	{
		get
		{
			return "FireAxeSN";
		}
	}

	public static string _3PLShotgunSN
	{
		get
		{
			return "_3PLShotgunSN";
		}
	}

	public static string Revolver2SN
	{
		get
		{
			return "Revolver2SN";
		}
	}

	public static string CrystakPickSN
	{
		get
		{
			return "CrystakPickSN";
		}
	}

	public static string IronSwordSN
	{
		get
		{
			return "IronSwordSN";
		}
	}

	public static string GoldenSwordSN
	{
		get
		{
			return "GoldenSwordSN";
		}
	}

	public static string GoldenRed_StoneSN
	{
		get
		{
			return "GoldenRed_StoneSN";
		}
	}

	public static string GoldenSPASSN
	{
		get
		{
			return "GoldenSPASSN";
		}
	}

	public static string GoldenGlockSN
	{
		get
		{
			return "GoldenGlockSN";
		}
	}

	public static string RedMinigunSN
	{
		get
		{
			return "RedMinigunSN";
		}
	}

	public static string CrystalCrossbowSN
	{
		get
		{
			return "CrystalCrossbowSN";
		}
	}

	public static string RedLightSaberSN
	{
		get
		{
			return "RedLightSaberSN";
		}
	}

	public static string SandFamasSN
	{
		get
		{
			return "SandFamasSN";
		}
	}

	public static string WhiteBerettaSN
	{
		get
		{
			return "WhiteBerettaSN";
		}
	}

	public static string BlackEagleSN
	{
		get
		{
			return "BlackEagleSN";
		}
	}

	public static string CrystalAxeSN
	{
		get
		{
			return "CrystalAxeSN";
		}
	}

	public static string SteelAxeSN
	{
		get
		{
			return "SteelAxeSN";
		}
	}

	public static string WoodenBowSN
	{
		get
		{
			return "WoodenBowSN";
		}
	}

	public static string Chainsaw2SN
	{
		get
		{
			return "Chainsaw2SN";
		}
	}

	public static string SteelCrossbowSN
	{
		get
		{
			return "SteelCrossbowSN";
		}
	}

	public static string Hammer2SN
	{
		get
		{
			return "Hammer2SN";
		}
	}

	public static string Mace2SN
	{
		get
		{
			return "Mace2SN";
		}
	}

	public static string Sword_22SN
	{
		get
		{
			return "Sword_22SN";
		}
	}

	public static string Staff2SN
	{
		get
		{
			return "Staff2SN";
		}
	}

	public static string BarrettSN
	{
		get
		{
			return "BarrettSN";
		}
	}

	public static string SVDSN
	{
		get
		{
			return "SVDSN";
		}
	}

	public static string Barrett2SN
	{
		get
		{
			return "Barrett2SN";
		}
	}

	public static string SVD_2SN
	{
		get
		{
			return "SVD_2SN";
		}
	}

	public static string FlameThrowerSN
	{
		get
		{
			return "FlameThrowerSN";
		}
	}

	public static string FlameThrower_2SN
	{
		get
		{
			return "FlameThrower_2SN";
		}
	}

	public static string BazookaSN
	{
		get
		{
			return "BazookaSN";
		}
	}

	public static string Bazooka_2SN
	{
		get
		{
			return "Bazooka_2SN";
		}
	}

	public static string GrenadeLnchSN
	{
		get
		{
			return "GrenadeLnchSN";
		}
	}

	public static string GrenadeLnch_2SN
	{
		get
		{
			return "GrenadeLnch_2SN";
		}
	}

	public static string TeslaSN
	{
		get
		{
			return "TeslaSN";
		}
	}

	public static string RailgunSN
	{
		get
		{
			return "RailgunSN";
		}
	}

	public static string NavyFamasSN
	{
		get
		{
			return "NavyFamasSN";
		}
	}

	public static string Eagle_3SN
	{
		get
		{
			return "Eagle_3SN";
		}
	}

	public static string hungerGamesPurchasedKey
	{
		get
		{
			return "HungerGamesPuchased";
		}
	}

	public static string smallAsAntKey
	{
		get
		{
			return "AntsKey";
		}
	}

	public static string code010110_Key
	{
		get
		{
			return "MatrixKey";
		}
	}

	public static string UnderwaterKey
	{
		get
		{
			return "UnderwaterKey";
		}
	}

	public static string CaptureFlagPurchasedKey
	{
		get
		{
			return "CaptureFlagGamesPuchased";
		}
	}

	public static string FirstLaunch
	{
		get
		{
			return "FirstLaunch";
		}
	}

	public static float touchPressurePower
	{
		get
		{
			if (_touchPressurePower < 0f)
			{
				_touchPressurePower = PlayerPrefs.GetFloat("3dTOUCHPower", 0.8f);
			}
			return _touchPressurePower;
		}
		set
		{
			_touchPressurePower = value;
			PlayerPrefs.SetFloat("3dTOUCHPower", _touchPressurePower);
		}
	}

	public static bool isUse3DTouch
	{
		get
		{
			if (_isUse3DTouch < 0)
			{
				_isUse3DTouch = PlayerPrefs.GetInt("Use3dTOUCH", 1);
			}
			return _isUse3DTouch == 1;
		}
		set
		{
			_isUse3DTouch = (value ? 1 : 0);
			PlayerPrefs.SetInt("Use3dTOUCH", _isUse3DTouch);
		}
	}

	public static bool isUseJump3DTouch
	{
		get
		{
			if (_isUseJump3DTouch < 0)
			{
				_isUseJump3DTouch = PlayerPrefs.GetInt("UseJump3dTOUCH", isUse3DTouch ? 1 : 0);
			}
			return _isUseJump3DTouch == 1;
		}
		set
		{
			_isUseJump3DTouch = (value ? 1 : 0);
			PlayerPrefs.SetInt("UseJump3dTOUCH", _isUseJump3DTouch);
		}
	}

	public static bool isUseShoot3DTouch
	{
		get
		{
			if (_isUseShoot3DTouch < 0)
			{
				_isUseShoot3DTouch = PlayerPrefs.GetInt("UseShoot3dTOUCH", isUse3DTouch ? 1 : 0);
			}
			return _isUseShoot3DTouch == 1;
		}
		set
		{
			_isUseShoot3DTouch = (value ? 1 : 0);
			PlayerPrefs.SetInt("UseShoot3dTOUCH", _isUseShoot3DTouch);
		}
	}

	public static string inappsRestored_3_1
	{
		get
		{
			return "inappsRestored_3_1";
		}
	}

	public static string restoreWindowShownProfile
	{
		get
		{
			return "restoreWindowShownProfile";
		}
	}

	public static string restoreWindowShownSingle
	{
		get
		{
			return "restoreWindowShownSingle";
		}
	}

	public static string restoreWindowShownMult
	{
		get
		{
			return "restoreWindowShownMult";
		}
	}

	public static string ShowSorryWeaponAndArmor
	{
		get
		{
			return "ShowSorryWeaponAndArmor";
		}
	}

	public static string ChangeOldLanguageName
	{
		get
		{
			return "ChangeOldLanguageName";
		}
	}

	public static string InitialAppVersionKey
	{
		get
		{
			return "InitialAppVersion";
		}
	}

	internal static int SaltSeed
	{
		get
		{
			return 2083243184;
		}
	}

	public static string SkinsMakerInProfileBought
	{
		get
		{
			return "SkinsMakerInProfileBought";
		}
	}

	public static string EarnedCoins
	{
		get
		{
			return "EarnedCoins";
		}
	}

	public static string COOPScore
	{
		get
		{
			return "COOPScore";
		}
	}

	public static string SkinsWrittenToGallery
	{
		get
		{
			return "SkinsWrittenToGallery";
		}
	}

	public static float screenRation
	{
		get
		{
			return (float)Screen.width / (float)Screen.height;
		}
	}

	public static string NumOfMultSkinsSett
	{
		get
		{
			return "NumOfMultSkinsSett";
		}
	}

	public static string KilledZombiesSett
	{
		get
		{
			return "KilledZombiesSett";
		}
	}

	public static string KilledZombiesMaxSett
	{
		get
		{
			return "KilledZombiesMaxSett";
		}
	}

	public static string WavesSurvivedMaxS
	{
		get
		{
			return "WavesSurvivedMaxS";
		}
	}

	public static string ProfileEnteredFromMenu
	{
		get
		{
			return "ProfileEnteredFromMenu";
		}
	}

	public static float DiffModif
	{
		get
		{
			float result = 0.6f;
			switch (diffGame)
			{
			case 1:
				result = 0.8f;
				break;
			case 2:
				result = 1f;
				break;
			}
			return result;
		}
	}

	public static string CancelButtonTitle
	{
		get
		{
			return "Cancel";
		}
	}

	public static int skinsMakerPrice
	{
		get
		{
			return 50;
		}
	}

	public static int HungerGamesPrice
	{
		get
		{
			return 75;
		}
	}

	public static int CaptureFlagPrice
	{
		get
		{
			return 100;
		}
	}

	public static int HoursToEndX3ForIndicate
	{
		get
		{
			return 6;
		}
	}

	public static string MainMenuScene
	{
		get
		{
			return "Menu_School";
		}
	}

	public static string ShouldReoeatActionSett
	{
		get
		{
			return "ShouldReoeatActionSett";
		}
	}

	public static string DiffSett
	{
		get
		{
			return "DifficultySett";
		}
	}

	public static string GoToProfileAction
	{
		get
		{
			return "GoToProfileAction";
		}
	}

	public static string GoToSkinsMakerAction
	{
		get
		{
			return "GoToSkinsMakerAction";
		}
	}

	public static string GoToPresetsAction
	{
		get
		{
			return "GoToPresetsAction";
		}
	}

	public static string SkinsMakerInMainMenuPurchased
	{
		get
		{
			return "SkinsMakerInMainMenuPurchased";
		}
	}

	public static string SessionNumberKey
	{
		get
		{
			return "SessionNumber";
		}
	}

	public static string SessionDayNumberKey
	{
		get
		{
			return "SessionDayNumber";
		}
	}

	public static string LastTimeSessionDayKey
	{
		get
		{
			return "LastTimeSessionDay";
		}
	}

	public static string LastTimeShowBanerKey
	{
		get
		{
			return "LastTimeShowBanerKey";
		}
	}

	public static int countReturnInConnectScene
	{
		get
		{
			if (!_countReturnInConnectSceneInit)
			{
				_countReturnInConnectScene = PlayerPrefs.GetInt("countReturnInConnectScene", 0);
				_countReturnInConnectSceneInit = true;
			}
			return _countReturnInConnectScene;
		}
		set
		{
			_countReturnInConnectScene = value;
			PlayerPrefs.SetInt("countReturnInConnectScene", _countReturnInConnectScene);
			_countReturnInConnectSceneInit = true;
		}
	}

	public static string StartTimeShowBannersKey
	{
		get
		{
			return "StartTimeShowBanners";
		}
	}

	public static string RankParameterKey
	{
		get
		{
			return "Rank";
		}
	}

	public static string GameModesEventKey
	{
		get
		{
			return "Game Modes";
		}
	}

	public static string MultiplayerModesKey
	{
		get
		{
			return "Multiplayer Modes";
		}
	}

	public static string NextMarafonBonusIndex
	{
		get
		{
			return "NextMarafonBonusIndex";
		}
	}

	public static string GameGUIOffMode
	{
		get
		{
			return "GameGUIOffMode";
		}
	}

	public static string NeedTakeMarathonBonus
	{
		get
		{
			return "NeedTakeMarathonBonus";
		}
	}

	public static string MarathonTestMode
	{
		get
		{
			return "MarathonTestMode";
		}
	}

	public static float Sensitivity
	{
		get
		{
			if (!_sensitivity.HasValue)
			{
				_sensitivity = PlayerPrefs.GetFloat("SensitivitySett", 12f);
			}
			return _sensitivity.Value;
		}
		set
		{
			_sensitivity = value;
			PlayerPrefs.SetFloat("SensitivitySett", value);
		}
	}

	public static bool IsChatOn
	{
		get
		{
			if (!_isChatOn.HasValue)
			{
				_isChatOn = PlayerPrefs.GetInt("ChatOn", 1) == 1;
			}
			return _isChatOn.Value;
		}
		set
		{
			_isChatOn = value;
			PlayerPrefs.SetInt("ChatOn", value ? 1 : 0);
		}
	}

	public static RuntimeAndroidEdition AndroidEdition
	{
		get
		{
			return RuntimeAndroidEdition.GoogleLite;
		}
	}

	public static bool IsDeveloperBuild
	{
		get
		{
			return false;
		}
	}

	public static string HockeyAppID
	{
		get
		{
			return "2d830f37b5a8daaef2b7ada172fc767d";
		}
	}

	static Defs()
	{
		LastSendKillRateTimeKey = "LastSendKillRateTimeKey";
		StrongDeviceDev = "StrongDeviceDev_DevSetting";
		TrafficForwardingShowAnalyticsSent = "TrafficForwardingShowAnalyticsSent";
		DateOfInstallAppForInAppPurchases041215 = "test_window_frames_margins_set_date";
		CoinsForTraining = 15;
		GemsForTraining = 10;
		ExpForTraining = 10;
		GemsGivenRemovedGear = "GemsGivenRemovedGear_10_3_0";
		LastTimeShowSocialGun = "FacebookControllerLastTimeShowSocialGunKey";
		ShownRewardWindowForCape = "ShownRewardWindowForCape";
		ShownRewardWindowForSkin = "ShownRewardWindowForSkin";
		keyInappBonusActionkey = "keyInappBonusActionkey";
		keysInappBonusGivenkey = "keysInappBonusGivenkey";
		keyInappPresentIDWeaponRedkey = "keyInappPresentIDWeaponRedkey";
		keyInappBonusStartActionForPresentIDWeaponRedkey = "keyInappBonusStartActionForPresentIDWeaponRedkey";
		keyInappPresentIDPetkey = "keyInappPresentIDPetkey";
		keyInappBonusStartActionForPresentIDPetkey = "keyInappBonusStartActionForPresentIDPetkey";
		keyInappPresentIDGadgetkey = "keyInappPresentIDGadgetkey";
		keyInappBonusStartActionForPresentIDGadgetkey = "keyInappBonusStartActionForPresentIDGadgetkey";
		DaterWSSN = "DaterWSSN";
		SmileMessageSuffix = "ýSýMýIýLýEý";
		IsFacebookLoginRewardaGained = "IsFacebookLoginRewardaGained";
		FacebookRewardGainStarted = "FacebookRewardGainStarted";
		IsTwitterLoginRewardaGained = "IsTwitterLoginRewardaGained";
		TwitterRewardGainStarted = "TwitterRewardGainStarted";
		ResetTrainingInDevBuild = false;
		useSqlLobby = true;
		keyTestCountGetGift = "keyTestCountGetGift";
		BuyGiftKey = "BuyGiftKey";
		localTimeInsteadServerTime = false;
		initValsInKeychain43 = "initValsInKeychain43";
		initValsInKeychain44 = "initValsInKeychain44";
		initValsInKeychain45 = "initValsInKeychain45";
		initValsInKeychain46 = "initValsInKeychain46";
		isMouseControl = false;
		isRegimVidosDebug = false;
		nonActivABTestBuffSystemKey = "nonActivABTestBuffSystemKey";
		cohortABTestAdvertKey = "cohortABTestAdvertKey";
		MoneyGiven831to901 = "MoneyGiven831to901";
		GotCoinsForTraining = "GotCoinsForTraining";
		typeDisconnectGame = DisconectGameType.Exit;
		gameSecondFireButtonMode = GameSecondFireButtonMode.Sniper;
		ZoomButtonX = -176;
		ZoomButtonY = 431;
		ReloadButtonX = -72;
		ReloadButtonY = 340;
		JumpButtonX = -95;
		JumpButtonY = 79;
		FireButtonX = -250;
		FireButtonY = 150;
		JoyStickX = 172;
		JoyStickY = 160;
		GrenadeX = -46;
		GrenadeY = 445;
		FireButton2X = 160;
		FireButton2Y = 337;
		VisualHatArmor = "VisualHatArmor";
		VisualArmor = "VisualArmor";
		GadgetContentFolder = "GadgetsContent";
		_enableLocalInviteFromFriendsKey = "enableLocalInviteFromFriendsKey";
		_enableRemoteInviteFromFriendsKey = "enableRemoteInviteFromFriendsKey";
		RatingDeathmatch = "RatingDeathmatch";
		RatingTeamBattle = "RatingTeamBattle";
		RatingHunger = "RatingHunger";
		RatingFlag = "RatingFlag";
		RatingCapturePoint = "RatingCapturePoint";
		RatingDuel = "RatingDuel";
		LogoWidth = 8;
		LogoHeight = 8;
		SurvivalMaps = new string[8] { "Arena_Swamp", "Arena_Underwater", "Coliseum", "Arena_Castle", "Arena_Space", "Arena_Hockey", "Arena_Mine", "Pizza" };
		CurrentSurvMapIndex = -1;
		FreezerSlowdownTime = 5f;
		_initializedJoystickParams = false;
		isShowUserAgrement = false;
		maxCountFriend = 100;
		maxMemberClanCount = 20;
		timeUpdateFriendInfo = 15f;
		timeUpdateOnlineInGame = 12f;
		timeUpdateInfoInProfile = 15f;
		timeBlockRefreshFriendDate = 5f;
		timeUpdateLeaderboardIfNullResponce = 15f;
		timeUpdateStartCheckIfNullResponce = 15f;
		timeWaitLoadPossibleFriends = 5f;
		pauseUpdateLeaderboard = 60f;
		timeUpdatePixelbookInfo = 900f;
		timeUpdateNews = 1800f;
		historyPrivateMessageLength = 100;
		timeUpdateServerTime = 15f;
		abTestBalansCohort = ABTestCohortsType.NONE;
		_isAbTestBalansCohortActual = null;
		abTestBalansCohortName = "NONE";
		bigPorogString = "No space for new friends. Delete friends or requests";
		smallPorogString = "Tap ADD TO FRIENDS to send a friendship request to the player";
		friendsSceneName = "FriendsScene";
		ammoInGamePanelPrice = 3;
		healthInGamePanelPrice = 5;
		ClansPrice = 0;
		ProfileFromFriends = 0;
		isInet = true;
		PixelGunAppID = "640111933";
		AppStoreURL = "https://itunes.apple.com/app/pixel-gun-3d-block-world-pocket/id" + PixelGunAppID + "?mt=8";
		SupportMail = "pixelgun3D.supp0rt@gmail.com";
		EnderManAvailable = true;
		isSoundMusic = false;
		isSoundFX = false;
		BottomOffs = 21f;
		filterMaps = new Dictionary<string, int>();
		_premiumMaps = new Dictionary<string, int>();
		NumberOfElixirs = 1;
		isGrenateFireEnable = true;
		isZooming = false;
		isJetpackEnabled = false;
		isJump = false;
		GoToProfileShopInterval = 1f;
		InvertCamSN = "InvertCamSN";
		players = new List<GameObject>();
		PromSceneName = "PromScene";
		_3_shotgun_2 = "_3_shotgun_2";
		_3_shotgun_3 = "_3_shotgun_3";
		flower_2 = "flower_2";
		flower_3 = "flower_3";
		gravity_2 = "gravity_2";
		gravity_3 = "gravity_3";
		grenade_launcher_3 = "grenade_launcher_3";
		revolver_2_2 = "revolver_2_2";
		revolver_2_3 = "revolver_2_3";
		scythe_3 = "scythe_3";
		plazma_2 = "plazma_2";
		plazma_3 = "plazma_3";
		plazma_pistol_2 = "plazma_pistol_2";
		plazma_pistol_3 = "plazma_pistol_3";
		railgun_2 = "railgun_2";
		railgun_3 = "railgun_3";
		Razer_3 = "Razer_3";
		tesla_3 = "tesla_3";
		Flamethrower_3 = "Flamethrower_3";
		FreezeGun_0 = "FreezeGun_0";
		svd_3 = "svd_3";
		barret_3 = "barret_3";
		minigun_3 = "minigun_3";
		LightSword_3 = "LightSword_3";
		Sword_2_3 = "Sword_2_3";
		Staff_3 = "Staff 3";
		DragonGun = "DragonGun";
		Bow_3 = "Bow_3";
		Bazooka_1_3 = "Bazooka_1_3";
		Bazooka_2_1 = "Bazooka_2_1";
		Bazooka_2_3 = "Bazooka_2_3";
		m79_2 = "m79_2";
		m79_3 = "m79_3";
		m32_1_2 = "m32_1_2";
		Red_Stone_3 = "Red_Stone_3";
		XM8_1 = "XM8_1";
		PumpkinGun_1 = "PumpkinGun_1";
		XM8_2 = "XM8_2";
		XM8_3 = "XM8_3";
		PumpkinGun_2 = "PumpkinGun_2";
		PumpkinGun_5 = "PumpkinGun_5";
		_touchPressurePower = -1f;
		_isUse3DTouch = -1;
		_isUseJump3DTouch = -1;
		_isUseShoot3DTouch = -1;
		Weapons800to801 = "Weapons800to801";
		Weapons831to901 = "Weapons831to901";
		Update_AddSniperCateogryKey = "Update_AddSniperCateogryKey";
		FixWeapons911 = "FixWeapons911";
		ReturnAlienGun930 = "ReturnAlienGun930";
		diffGame = 2;
		StartTimeShowBannersString = string.Empty;
		_countReturnInConnectScene = 0;
		showTableInNetworkStartTable = false;
		showNickTableInNetworkStartTable = false;
		isTurretWeapon = false;
		inComingMessagesCounter = 0;
		unimportantRPCList = new HashSet<string> { "fireFlash", "HoleRPC", "ShowBonuseParticleRPC", "ShowMultyKillRPC", "ReloadGun" };
		inRespawnWindow = false;
		IsFirstLaunchFreshInstall = "IsFirstLaunchFreshInstall";
		NewbieEventX3StartTime = "NewbieEventX3StartTime";
		NewbieEventX3StartTimeAdditional = "NewbieEventX3StartTimeAdditional";
		NewbieEventX3LastLoggedTime = "NewbieEventX3LastLoggedTime";
		WasNewbieEventX3 = "WasNewbieEventX3";
		bootsMaterialDict = new Dictionary<string, string>
		{
			{ "boots_black", "BerserkBoots" },
			{ "BerserkBoots_Up1", "BerserkBoots" },
			{ "BerserkBoots_Up2", "BerserkBoots" },
			{ "boots_blue", "SniperBoots" },
			{ "SniperBoots_Up1", "SniperBoots" },
			{ "SniperBoots_Up2", "SniperBoots" },
			{ "boots_Engineer", "EngineerBoots" },
			{ "boots_Engineer_Up1", "EngineerBoots" },
			{ "boots_Engineer_Up2", "EngineerBoots" },
			{ "EngineerBoots", "EngineerBoots" },
			{ "EngineerBoots_Up1", "EngineerBoots" },
			{ "EngineerBoots_Up2", "EngineerBoots" },
			{ "boots_gray", "StormTrooperBoots" },
			{ "StormTrooperBoots_Up1", "StormTrooperBoots" },
			{ "StormTrooperBoots_Up2", "StormTrooperBoots" },
			{ "boots_green", "DemolitionBoots" },
			{ "DemolitionBoots_Up1", "DemolitionBoots" },
			{ "DemolitionBoots_Up2", "DemolitionBoots" },
			{ "boots_red", "HitmanBoots" },
			{ "HitmanBoots_Up1", "HitmanBoots" },
			{ "HitmanBoots_Up2", "HitmanBoots" },
			{ "boots_tabi", "boots_tabi" }
		};
		_premiumMaps.Add("Ants", 15);
		_premiumMaps.Add("Matrix", 15);
		_premiumMaps.Add("Underwater", 15);
		filterMaps.Add("Knife", 1);
		filterMaps.Add("Sniper", 2);
		filterMaps.Add("LoveIsland", 3);
		filterMaps.Add("WinterIsland", 3);
	}

	public static Color AmbientLightColorForShop()
	{
		return new Color(20f / 51f, 20f / 51f, 20f / 51f, 1f);
	}

	public static int CompareAlphaNumerically(object x, object y)
	{
		string text = x as string;
		if (text == null)
		{
			return 0;
		}
		string text2 = y as string;
		if (text2 == null)
		{
			return 0;
		}
		int length = text.Length;
		int length2 = text2.Length;
		int num = 0;
		int num2 = 0;
		while (num < length && num2 < length2)
		{
			char c = text[num];
			char c2 = text2[num2];
			char[] array = new char[length];
			int num3 = 0;
			char[] array2 = new char[length2];
			int num4 = 0;
			do
			{
				array[num3++] = c;
				num++;
				if (num < length)
				{
					c = text[num];
					continue;
				}
				break;
			}
			while (char.IsDigit(c) == char.IsDigit(array[0]));
			do
			{
				array2[num4++] = c2;
				num2++;
				if (num2 < length2)
				{
					c2 = text2[num2];
					continue;
				}
				break;
			}
			while (char.IsDigit(c2) == char.IsDigit(array2[0]));
			string text3 = new string(array);
			string text4 = new string(array2);
			int num6;
			if (char.IsDigit(array[0]) && char.IsDigit(array2[0]))
			{
				int num5 = int.Parse(text3);
				int value = int.Parse(text4);
				num6 = num5.CompareTo(value);
			}
			else
			{
				num6 = text3.CompareTo(text4);
			}
			if (num6 != 0)
			{
				return num6;
			}
		}
		return length - length2;
	}

	public static void InitCoordsIphone()
	{
		if (!_initializedJoystickParams)
		{
			float screenDiagonal = ScreenDiagonal;
			if (screenDiagonal > 9f)
			{
				ZoomButtonX = -216;
				ZoomButtonY = 369;
				ReloadButtonX = -49;
				ReloadButtonY = 302;
				JumpButtonX = -101;
				JumpButtonY = 90;
				FireButtonX = -258;
				FireButtonY = 179;
				JoyStickX = 172;
				JoyStickY = 160;
				GrenadeX = -110;
				GrenadeY = 381;
				FireButton2X = 173;
				FireButton2Y = 340;
			}
			if (screenDiagonal > 7.5f && screenDiagonal <= 9f)
			{
				ZoomButtonX = -230;
				ZoomButtonY = 397;
				ReloadButtonX = -53;
				ReloadButtonY = 355;
				JumpButtonX = -116;
				JumpButtonY = 99;
				FireButtonX = -284;
				FireButtonY = 175;
				JoyStickX = 172;
				JoyStickY = 160;
				GrenadeX = -130;
				GrenadeY = 419;
				FireButton2X = 173;
				FireButton2Y = 352;
			}
			if (screenDiagonal > 6f && screenDiagonal <= 7.5f)
			{
				ZoomButtonX = -227;
				ZoomButtonY = 404;
				ReloadButtonX = -67;
				ReloadButtonY = 351;
				JumpButtonX = -125;
				JumpButtonY = 104;
				FireButtonX = -291;
				FireButtonY = 189;
				JoyStickX = 170;
				JoyStickY = 167;
				GrenadeX = -131;
				GrenadeY = 441;
				FireButton2X = 173;
				FireButton2Y = 352;
			}
			if (screenDiagonal > 4.8f && screenDiagonal <= 6f)
			{
				ZoomButtonX = -263;
				ZoomButtonY = 409;
				ReloadButtonX = -61;
				ReloadButtonY = 359;
				JumpButtonX = -126;
				JumpButtonY = 105;
				FireButtonX = -319;
				FireButtonY = 181;
				JoyStickX = 170;
				JoyStickY = 167;
				GrenadeX = -155;
				GrenadeY = 424;
				FireButton2X = 176;
				FireButton2Y = 361;
			}
			if (screenDiagonal > 4f && screenDiagonal <= 4.8f)
			{
				ZoomButtonX = -298;
				ZoomButtonY = 402;
				ReloadButtonX = -68;
				ReloadButtonY = 369;
				JumpButtonX = -133;
				JumpButtonY = 99;
				FireButtonX = -331;
				FireButtonY = 179;
				JoyStickX = 170;
				JoyStickY = 167;
				GrenadeX = -175;
				GrenadeY = 428;
				FireButton2X = 189;
				FireButton2Y = 357;
			}
		}
		_initializedJoystickParams = true;
	}

	public static string GetIntendedAndroidPackageName()
	{
		return GetIntendedAndroidPackageName(AndroidEdition);
	}

	public static string GetIntendedAndroidPackageName(RuntimeAndroidEdition androidEdition)
	{
		switch (androidEdition)
		{
		case RuntimeAndroidEdition.GoogleLite:
			return "com.pixel.gun3d";
		case RuntimeAndroidEdition.Amazon:
			return "com.PixelGun.a3D";
		default:
			return string.Empty;
		}
	}
}

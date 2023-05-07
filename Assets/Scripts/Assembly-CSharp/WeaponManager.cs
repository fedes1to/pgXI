using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prime31;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class WeaponManager : MonoBehaviour
{
	public enum WeaponTypeForLow
	{
		AssaultRifle_1,
		AssaultRifle_2,
		Shotgun_1,
		Shotgun_2,
		Machinegun,
		Pistol_1,
		Pistol_2,
		Submachinegun,
		Knife,
		Sword,
		Flamethrower_1,
		Flamethrower_2,
		SniperRifle_1,
		SniperRifle_2,
		Bow,
		RocketLauncher_1,
		RocketLauncher_2,
		RocketLauncher_3,
		GrenadeLauncher,
		Snaryad,
		Snaryad_Otskok,
		Snaryad_Disk,
		Railgun,
		Ray,
		AOE,
		Instant_Area_Damage,
		X3_Snaryad,
		NOT_CHANGE
	}

	internal sealed class WeaponTypeForLowComparer : IEqualityComparer<WeaponTypeForLow>
	{
		public bool Equals(WeaponTypeForLow x, WeaponTypeForLow y)
		{
			return x == y;
		}

		public int GetHashCode(WeaponTypeForLow obj)
		{
			return (int)obj;
		}
	}

	public struct infoClient
	{
		public string ipAddress;

		public string name;

		public string coments;
	}

	public const int DefaultNumberOfMatchesForTryGuns = 3;

	private const string TryGunsTableServerKey = "TryGuns";

	public const string NumberOfMatchesKey = "NumberOfMatchesKey";

	public const string EquippedBeforeKey = "EquippedBeforeKey";

	public const string TryGunsDictionaryKey = "TryGunsDictionaryKey";

	public const string ExpiredTryGunsListKey = "ExpiredTryGunsListKey";

	public const string TryGunsKey = "WeaponManager.TryGunsKey";

	public const string TryGunsDiscountsKey = "WeaponManager.TryGunsDiscountsKey";

	public const string TryGunsDiscountsValuesKey = "WeaponManager.TryGunsDiscountsValuesKey";

	public const int NumOfWeaponCategories = 6;

	private const string FirstTagForOurTierKey = "FirstTagsForOurTier";

	private const string SniperCategoryAddedToWeaponSetsKey = "WeaponManager_SniperCategoryAddedToWeaponSetsKey";

	private const string LastUsedWeaponsKey = "WeaponManager.LastUsedWeaponsKey";

	private static bool _buffsPAramsInitialized;

	private static Dictionary<ShopNGUIController.CategoryNames, List<List<string>>> _defaultTryGunsTable;

	private Dictionary<string, long> tryGunPromos = new Dictionary<string, long>();

	private Dictionary<string, SaltedLong> tryGunDiscounts = new Dictionary<string, SaltedLong>();

	public Dictionary<string, Dictionary<string, object>> TryGuns = new Dictionary<string, Dictionary<string, object>>();

	public List<string> ExpiredTryGuns = new List<string>();

	public static Dictionary<int, FilterMapSettings> WeaponSetSettingNamesForFilterMaps;

	public static List<string> GotchaGuns;

	public static List<KeyValuePair<string, string>> replaceConstWithTemp;

	public GameObject _grenadeWeaponCache;

	public GameObject _turretWeaponCache;

	public GameObject _rocketCache;

	public GameObject _turretCache;

	public static string WeaponPreviewsPath;

	public static string DaterFreeWeaponPrefabName;

	public static List<GameObject> cachedInnerPrefabsForCurrentShopCategory;

	public static Dictionary<string, string> campaignBonusWeapons;

	public static Dictionary<string, string> tagToStoreIDMapping;

	public static Dictionary<string, string> storeIDtoDefsSNMapping;

	private static readonly HashSet<string> _purchasableWeaponSet;

	public static string _3_shotgun_2_WN;

	public static string _3_shotgun_3_WN;

	public static string flower_2_WN;

	public static string flower_3_WN;

	public static string gravity_2_WN;

	public static string gravity_3_WN;

	public static string grenade_launcher_3_WN;

	public static string revolver_2_2_WN;

	public static string revolver_2_3_WN;

	public static string scythe_3_WN;

	public static string plazma_2_WN;

	public static string plazma_3_WN;

	public static string plazma_pistol_2_WN;

	public static string plazma_pistol_3_WN;

	public static string railgun_2_WN;

	public static string railgun_3_WN;

	public static string Razer_3_WN;

	public static string tesla_3_WN;

	public static string Flamethrower_3_WN;

	public static string FreezeGun_0_WN;

	public static string svd_3_WN;

	public static string barret_3_WN;

	public static string minigun_3_WN;

	public static string LightSword_3_WN;

	public static string Sword_2_3_WN;

	public static string Staff_3_WN;

	public static string DragonGun_WN;

	public static string Bow_3_WN;

	public static string Bazooka_1_3_WN;

	public static string Bazooka_2_1_WN;

	public static string Bazooka_2_3_WN;

	public static string m79_2_WN;

	public static string m79_3_WN;

	public static string m32_1_2_WN;

	public static string Red_Stone_3_WN;

	public static string XM8_1_WN;

	public static string PumpkinGun_1_WN;

	public static string XM8_2_WN;

	public static string XM8_3_WN;

	public static string PumpkinGun_2_WN;

	public static string Rocketnitza_WN;

	public static WeaponManager sharedManager;

	public static readonly int LastNotNewWeapon;

	public List<string> shownWeapons = new List<string>();

	public HostData hostDataServer;

	public string ServerIp;

	public GameObject myPlayer;

	public Player_move_c myPlayerMoveC;

	public GameObject myGun;

	public GameObject myTable;

	public NetworkStartTable myNetworkStartTable;

	private UnityEngine.Object[] _weaponsInGame;

	private UnityEngine.Object[] _multiWeapons;

	private UnityEngine.Object[] _hungerWeapons;

	private ArrayList _playerWeapons = new ArrayList();

	private ArrayList _allAvailablePlayerWeapons = new ArrayList();

	private int currentWeaponIndex;

	private Dictionary<string, int> lastUsedWeaponsForFilterMaps = new Dictionary<string, int>
	{
		{ "0", 0 },
		{ "1", 2 },
		{ "2", 4 },
		{ "3", 2 }
	};

	public Camera useCam;

	private WeaponSounds _currentWeaponSounds = new WeaponSounds();

	private Dictionary<string, Action<string, int>> _purchaseActinos = new Dictionary<string, Action<string, int>>(300);

	public List<infoClient> players = new List<infoClient>();

	public List<List<GameObject>> _weaponsByCat = new List<List<GameObject>>();

	public List<List<GameObject>> FilteredShopListsForPromos;

	public List<List<GameObject>> FilteredShopListsNoUpgrades;

	private List<GameObject> _playerWeaponsSetInnerPrefabsCache = new List<GameObject>();

	private int _lockGetWeaponPrefabs;

	private List<WeaponSounds> outerWeaponPrefabs;

	private static List<string> _Removed150615_Guns;

	private static List<string> _Removed150615_GunsPrefabNAmes;

	private static bool firstTagsForTiersInitialized;

	private static Dictionary<string, string> firstTagsWithRespecToOurTier;

	private static string[] oldTags;

	private bool _resetLock;

	private static bool _migrationsCompletedAtThisLaunch;

	public int _currentFilterMap;

	private bool _initialized;

	private static List<string> weaponsMovedToSniperCategory;

	private List<ShopPositionParams> _wearInfoPrefabsToCache = new List<ShopPositionParams>();

	private AnimationClip[] _profileAnimClips;

	private static Comparison<WeaponSounds> dpsComparerWS;

	private Comparison<GameObject> dpsComparer = delegate(GameObject leftw, GameObject rightw)
	{
		if (leftw == null || rightw == null)
		{
			return 0;
		}
		WeaponSounds component = leftw.GetComponent<WeaponSounds>();
		WeaponSounds component2 = rightw.GetComponent<WeaponSounds>();
		return dpsComparerWS(component, component2);
	};

	private static readonly StringBuilder _sharedStringBuilder;

	public Dictionary<string, long> TryGunPromos
	{
		get
		{
			return tryGunPromos;
		}
	}

	public bool AnyDiscountForTryGuns
	{
		get
		{
			return tryGunPromos != null && tryGunPromos.Count > 0;
		}
	}

	public static Dictionary<ShopNGUIController.CategoryNames, List<List<string>>> tryGunsTable
	{
		get
		{
			Dictionary<ShopNGUIController.CategoryNames, List<List<string>>> dictionary = null;
			try
			{
				if (!_buffsPAramsInitialized && !Storager.hasKey("BuffsParam"))
				{
					Storager.setString("BuffsParam", "{}", false);
				}
				_buffsPAramsInitialized = true;
				Dictionary<string, object> dictionary2 = Rilisoft.MiniJson.Json.Deserialize(Storager.getString("BuffsParam", false)) as Dictionary<string, object>;
				if (dictionary2 != null && dictionary2.ContainsKey("TryGuns"))
				{
					Dictionary<string, object> source = dictionary2["TryGuns"] as Dictionary<string, object>;
					dictionary = source.ToDictionary((KeyValuePair<string, object> kvp) => (ShopNGUIController.CategoryNames)(int)Enum.Parse(typeof(ShopNGUIController.CategoryNames), kvp.Key, true), (KeyValuePair<string, object> kvp) => (from listObject in (kvp.Value as List<object>).OfType<List<object>>()
						select listObject.OfType<string>().ToList()).ToList());
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in reading try guns table from storager: " + ex);
			}
			return (dictionary == null) ? _defaultTryGunsTable : dictionary;
		}
	}

	public bool ResetLockSet
	{
		get
		{
			return _resetLock;
		}
	}

	public static string PistolWN
	{
		get
		{
			return "Weapon1";
		}
	}

	public static string ShotgunWN
	{
		get
		{
			return "Weapon2";
		}
	}

	public static string MP5WN
	{
		get
		{
			return "Weapon3";
		}
	}

	public static string RevolverWN
	{
		get
		{
			return "Weapon4";
		}
	}

	public static string MachinegunWN
	{
		get
		{
			return "Weapon5";
		}
	}

	public static string AK47WN
	{
		get
		{
			return "Weapon8";
		}
	}

	public static string KnifeWN
	{
		get
		{
			return "Weapon9";
		}
	}

	public static string ObrezWN
	{
		get
		{
			return "Weapon51";
		}
	}

	public static string AlienGunWN
	{
		get
		{
			return "Weapon52";
		}
	}

	public static string BugGunWN
	{
		get
		{
			return "Weapon250";
		}
	}

	public static string SocialGunWN
	{
		get
		{
			return "Weapon302";
		}
	}

	public static string _initialWeaponName
	{
		get
		{
			return "FirstPistol";
		}
	}

	public static string PickWeaponName
	{
		get
		{
			return "Weapon6";
		}
	}

	public static string MultiplayerMeleeTag
	{
		get
		{
			return "Knife";
		}
	}

	public static string SwordWeaponName
	{
		get
		{
			return "Weapon7";
		}
	}

	public static string CombatRifleWeaponName
	{
		get
		{
			return "Weapon10";
		}
	}

	public static string GoldenEagleWeaponName
	{
		get
		{
			return "Weapon11";
		}
	}

	public static string MagicBowWeaponName
	{
		get
		{
			return "Weapon12";
		}
	}

	public static string SpasWeaponName
	{
		get
		{
			return "Weapon13";
		}
	}

	public static string GoldenAxeWeaponnName
	{
		get
		{
			return "Weapon14";
		}
	}

	public static string ChainsawWN
	{
		get
		{
			return "Weapon15";
		}
	}

	public static string FAMASWN
	{
		get
		{
			return "Weapon16";
		}
	}

	public static string GlockWN
	{
		get
		{
			return "Weapon17";
		}
	}

	public static string ScytheWN
	{
		get
		{
			return "Weapon18";
		}
	}

	public static string Scythe_2_WN
	{
		get
		{
			return "Weapon68";
		}
	}

	public static string ShovelWN
	{
		get
		{
			return "Weapon19";
		}
	}

	public static string HammerWN
	{
		get
		{
			return "Weapon20";
		}
	}

	public static string Sword_2_WN
	{
		get
		{
			return "Weapon21";
		}
	}

	public static string StaffWN
	{
		get
		{
			return "Weapon22";
		}
	}

	public static string LaserRifleWN
	{
		get
		{
			return "Weapon23";
		}
	}

	public static string LightSwordWN
	{
		get
		{
			return "Weapon24";
		}
	}

	public static string BerettaWN
	{
		get
		{
			return "Weapon25";
		}
	}

	public static string Beretta_2_WN
	{
		get
		{
			return "Weapon71";
		}
	}

	public static string MaceWN
	{
		get
		{
			return "Weapon26";
		}
	}

	public static string CrossbowWN
	{
		get
		{
			return "Weapon27";
		}
	}

	public static string MinigunWN
	{
		get
		{
			return "Weapon28";
		}
	}

	public static string GoldenPickWN
	{
		get
		{
			return "Weapon29";
		}
	}

	public static string CrystalPickWN
	{
		get
		{
			return "Weapon30";
		}
	}

	public static string IronSwordWN
	{
		get
		{
			return "Weapon31";
		}
	}

	public static string GoldenSwordWN
	{
		get
		{
			return "Weapon32";
		}
	}

	public static string GoldenRed_StoneWN
	{
		get
		{
			return "Weapon33";
		}
	}

	public static string GoldenSPASWN
	{
		get
		{
			return "Weapon34";
		}
	}

	public static string GoldenGlockWN
	{
		get
		{
			return "Weapon35";
		}
	}

	public static string RedMinigunWN
	{
		get
		{
			return "Weapon36";
		}
	}

	public static string CrystalCrossbowWN
	{
		get
		{
			return "Weapon37";
		}
	}

	public static string RedLightSaberWN
	{
		get
		{
			return "Weapon38";
		}
	}

	public static string SandFamasWN
	{
		get
		{
			return "Weapon39";
		}
	}

	public static string WhiteBerettaWN
	{
		get
		{
			return "Weapon40";
		}
	}

	public static string BlackEagleWN
	{
		get
		{
			return "Weapon41";
		}
	}

	public static string CrystalAxeWN
	{
		get
		{
			return "Weapon42";
		}
	}

	public static string SteelAxeWN
	{
		get
		{
			return "Weapon43";
		}
	}

	public static string WoodenBowWN
	{
		get
		{
			return "Weapon44";
		}
	}

	public static string Chainsaw2WN
	{
		get
		{
			return "Weapon45";
		}
	}

	public static string SteelCrossbowWN
	{
		get
		{
			return "Weapon46";
		}
	}

	public static string Hammer2WN
	{
		get
		{
			return "Weapon47";
		}
	}

	public static string Mace2WN
	{
		get
		{
			return "Weapon48";
		}
	}

	public static string Sword_22WN
	{
		get
		{
			return "Weapon49";
		}
	}

	public static string Staff2WN
	{
		get
		{
			return "Weapon50";
		}
	}

	public static string M16_2WN
	{
		get
		{
			return "Weapon53";
		}
	}

	public static string M16_3WN
	{
		get
		{
			return "Weapon69";
		}
	}

	public static string M16_4WN
	{
		get
		{
			return "Weapon70";
		}
	}

	public static string CrystalGlockWN
	{
		get
		{
			return "Weapon54";
		}
	}

	public static string CrystalSPASWN
	{
		get
		{
			return "Weapon55";
		}
	}

	public static string TreeWN
	{
		get
		{
			return "Weapon56";
		}
	}

	public static string Tree_2_WN
	{
		get
		{
			return "Weapon72";
		}
	}

	public static string FireAxeWN
	{
		get
		{
			return "Weapon57";
		}
	}

	public static string _3pl_shotgunWN
	{
		get
		{
			return "Weapon58";
		}
	}

	public static string Revolver2WN
	{
		get
		{
			return "Weapon59";
		}
	}

	public static string BarrettWN
	{
		get
		{
			return "Weapon60";
		}
	}

	public static string svdWN
	{
		get
		{
			return "Weapon61";
		}
	}

	public static string NavyFamasWN
	{
		get
		{
			return "Weapon62";
		}
	}

	public static string svd_2WN
	{
		get
		{
			return "Weapon63";
		}
	}

	public static string Eagle_3WN
	{
		get
		{
			return "Weapon64";
		}
	}

	public static string Barrett_2WN
	{
		get
		{
			return "Weapon65";
		}
	}

	public static string UZI_WN
	{
		get
		{
			return "Weapon66";
		}
	}

	public static string CampaignRifle_WN
	{
		get
		{
			return "Weapon67";
		}
	}

	public static string SimpleFlamethrower_WN
	{
		get
		{
			return "Weapon333";
		}
	}

	public static string Flamethrower_WN
	{
		get
		{
			return "Weapon73";
		}
	}

	public static string Flamethrower_2_WN
	{
		get
		{
			return "Weapon74";
		}
	}

	public static string Bazooka_WN
	{
		get
		{
			return "Weapon75";
		}
	}

	public static string Bazooka_2_WN
	{
		get
		{
			return "Weapon76";
		}
	}

	public static string Railgun_WN
	{
		get
		{
			return "Weapon77";
		}
	}

	public static string Tesla_WN
	{
		get
		{
			return "Weapon78";
		}
	}

	public static string GrenadeLunacher_WN
	{
		get
		{
			return "Weapon79";
		}
	}

	public static string GrenadeLunacher_2_WN
	{
		get
		{
			return "Weapon80";
		}
	}

	public static string Tesla_2_WN
	{
		get
		{
			return "Weapon81";
		}
	}

	public static string Bazooka_3_WN
	{
		get
		{
			return "Weapon82";
		}
	}

	public static string Gravigun_WN
	{
		get
		{
			return "Weapon83";
		}
	}

	public static string AUG_WN
	{
		get
		{
			return "Weapon84";
		}
	}

	public static string AUG_2_WN
	{
		get
		{
			return "Weapon85";
		}
	}

	public static string Razer_WN
	{
		get
		{
			return "Weapon86";
		}
	}

	public static string Razer_2_WN
	{
		get
		{
			return "Weapon87";
		}
	}

	public static string katana_WN
	{
		get
		{
			return "Weapon88";
		}
	}

	public static string katana_2_WN
	{
		get
		{
			return "Weapon89";
		}
	}

	public static string katana_3_WN
	{
		get
		{
			return "Weapon90";
		}
	}

	public static string plazma_WN
	{
		get
		{
			return "Weapon91";
		}
	}

	public static string plazma_pistol_WN
	{
		get
		{
			return "Weapon92";
		}
	}

	public static string Flower_WN
	{
		get
		{
			return "Weapon93";
		}
	}

	public static string Buddy_WN
	{
		get
		{
			return "Weapon94";
		}
	}

	public static string Mauser_WN
	{
		get
		{
			return "Weapon95";
		}
	}

	public static string Shmaiser_WN
	{
		get
		{
			return "Weapon96";
		}
	}

	public static string Thompson_WN
	{
		get
		{
			return "Weapon97";
		}
	}

	public static string Thompson_2_WN
	{
		get
		{
			return "Weapon98";
		}
	}

	public static string BassCannon_WN
	{
		get
		{
			return "Weapon99";
		}
	}

	public static string SpakrlyBlaster_WN
	{
		get
		{
			return "Weapon100";
		}
	}

	public static string CherryGun_WN
	{
		get
		{
			return "Weapon101";
		}
	}

	public static string AK74_WN
	{
		get
		{
			return "Weapon102";
		}
	}

	public static string AK74_2_WN
	{
		get
		{
			return "Weapon103";
		}
	}

	public static string AK74_3_WN
	{
		get
		{
			return "Weapon104";
		}
	}

	public static string FreezeGun_WN
	{
		get
		{
			return "Weapon105";
		}
	}

	public int CurrentWeaponIndex
	{
		get
		{
			return currentWeaponIndex;
		}
		set
		{
			currentWeaponIndex = value;
		}
	}

	public int ShopListsTierConstraint
	{
		get
		{
			return 10000;
		}
	}

	public UnityEngine.Object[] weaponsInGame
	{
		get
		{
			return _weaponsInGame;
		}
	}

	public ArrayList playerWeapons
	{
		get
		{
			return _playerWeapons;
		}
	}

	public ArrayList allAvailablePlayerWeapons
	{
		get
		{
			return _allAvailablePlayerWeapons;
		}
		private set
		{
			_allAvailablePlayerWeapons = value;
		}
	}

	public WeaponSounds currentWeaponSounds
	{
		get
		{
			return _currentWeaponSounds;
		}
		set
		{
			_currentWeaponSounds = value;
		}
	}

	public int LockGetWeaponPrefabs
	{
		get
		{
			return _lockGetWeaponPrefabs;
		}
	}

	public static List<string> Removed150615_PrefabNames
	{
		get
		{
			if (_Removed150615_Guns == null)
			{
				InitializeRemoved150615Weapons();
			}
			return _Removed150615_GunsPrefabNAmes;
		}
	}

	public static List<string> Removed150615_Guns
	{
		get
		{
			if (_Removed150615_Guns == null)
			{
				InitializeRemoved150615Weapons();
			}
			return _Removed150615_Guns;
		}
	}

	public int CurrentFilterMap
	{
		get
		{
			return _currentFilterMap;
		}
	}

	public bool Initialized
	{
		get
		{
			return _initialized;
		}
	}

	public static event Action TryGunRemoved;

	public static event Action TryGunExpired;

	public static event Action<WeaponSounds> WeaponEquipped;

	public static event Action<WeaponSounds> WeaponEquipped_AllCases;

	static WeaponManager()
	{
		WeaponSetSettingNamesForFilterMaps = new Dictionary<int, FilterMapSettings>
		{
			{
				0,
				new FilterMapSettings
				{
					settingName = Defs.MultiplayerWSSN,
					defaultWeaponSet = _KnifeAndPistolAndMP5AndSniperAndRocketnitzaSet
				}
			},
			{
				1,
				new FilterMapSettings
				{
					settingName = "WeaponManager.KnifesModeWSSN",
					defaultWeaponSet = _KnifeSet
				}
			},
			{
				2,
				new FilterMapSettings
				{
					settingName = "WeaponManager.SniperModeWSSN",
					defaultWeaponSet = _KnifeAndPistolAndSniperSet
				}
			},
			{
				3,
				new FilterMapSettings
				{
					settingName = Defs.DaterWSSN,
					defaultWeaponSet = _InitialDaterSet
				}
			}
		};
		GotchaGuns = new List<string>
		{
			"gift_gun",
			"Candy_Baton",
			"mp5_gold_gift",
			WeaponTags.spark_shark_Tag,
			WeaponTags.power_claw_Tag
		};
		replaceConstWithTemp = new List<KeyValuePair<string, string>>();
		WeaponPreviewsPath = "WeaponPreviews";
		DaterFreeWeaponPrefabName = "Weapon298";
		cachedInnerPrefabsForCurrentShopCategory = new List<GameObject>();
		campaignBonusWeapons = new Dictionary<string, string>();
		tagToStoreIDMapping = new Dictionary<string, string>(200);
		storeIDtoDefsSNMapping = new Dictionary<string, string>(200);
		_purchasableWeaponSet = new HashSet<string>();
		_3_shotgun_2_WN = "Weapon107";
		_3_shotgun_3_WN = "Weapon108";
		flower_2_WN = "Weapon109";
		flower_3_WN = "Weapon110";
		gravity_2_WN = "Weapon111";
		gravity_3_WN = "Weapon112";
		grenade_launcher_3_WN = "Weapon113";
		revolver_2_2_WN = "Weapon114";
		revolver_2_3_WN = "Weapon115";
		scythe_3_WN = "Weapon116";
		plazma_2_WN = "Weapon117";
		plazma_3_WN = "Weapon118";
		plazma_pistol_2_WN = "Weapon119";
		plazma_pistol_3_WN = "Weapon120";
		railgun_2_WN = "Weapon121";
		railgun_3_WN = "Weapon122";
		Razer_3_WN = "Weapon123";
		tesla_3_WN = "Weapon124";
		Flamethrower_3_WN = "Weapon125";
		FreezeGun_0_WN = "Weapon126";
		svd_3_WN = "Weapon128";
		barret_3_WN = "Weapon129";
		minigun_3_WN = "Weapon127";
		LightSword_3_WN = "Weapon130";
		Sword_2_3_WN = "Weapon131";
		Staff_3_WN = "Weapon132";
		DragonGun_WN = "Weapon133";
		Bow_3_WN = "Weapon134";
		Bazooka_1_3_WN = "Weapon135";
		Bazooka_2_1_WN = "Weapon136";
		Bazooka_2_3_WN = "Weapon137";
		m79_2_WN = "Weapon138";
		m79_3_WN = "Weapon139";
		m32_1_2_WN = "Weapon140";
		Red_Stone_3_WN = "Weapon141";
		XM8_1_WN = "Weapon142";
		PumpkinGun_1_WN = "Weapon143";
		XM8_2_WN = "Weapon144";
		XM8_3_WN = "Weapon145";
		PumpkinGun_2_WN = "Weapon147";
		Rocketnitza_WN = "Weapon162";
		sharedManager = null;
		LastNotNewWeapon = 76;
		_Removed150615_Guns = null;
		_Removed150615_GunsPrefabNAmes = null;
		firstTagsForTiersInitialized = false;
		firstTagsWithRespecToOurTier = new Dictionary<string, string>();
		oldTags = new string[53]
		{
			WeaponTags.MinersWeaponTag,
			WeaponTags.Sword_2_3_Tag,
			WeaponTags.RailgunTag,
			WeaponTags.SteelAxeTag,
			WeaponTags.IronSwordTag,
			WeaponTags.Red_Stone_3_Tag,
			WeaponTags.SPASTag,
			WeaponTags.SteelCrossbowTag,
			WeaponTags.minigun_3_Tag,
			WeaponTags.LightSword_3_Tag,
			WeaponTags.FAMASTag,
			WeaponTags.FreezeGunTag,
			WeaponTags.BerettaTag,
			WeaponTags.EagleTag,
			WeaponTags.GlockTag,
			WeaponTags.svdTag,
			WeaponTags.m16Tag,
			WeaponTags.TreeTag,
			WeaponTags.revolver_2_3_Tag,
			WeaponTags.FreezeGun_0_Tag,
			WeaponTags.TeslaTag,
			WeaponTags.Bazooka_3Tag,
			WeaponTags.GrenadeLuancher_2Tag,
			WeaponTags.BazookaTag,
			WeaponTags.AUGTag,
			WeaponTags.AK74Tag,
			WeaponTags.GravigunTag,
			WeaponTags.XM8_1_Tag,
			WeaponTags.PumpkinGun_1_Tag,
			WeaponTags.SnowballMachingun_Tag,
			WeaponTags.SnowballGun_Tag,
			WeaponTags.HeavyShotgun_Tag,
			WeaponTags.TwoBolters_Tag,
			WeaponTags.TwoRevolvers_Tag,
			WeaponTags.AutoShotgun_Tag,
			WeaponTags.Solar_Ray_Tag,
			WeaponTags.Water_Pistol_Tag,
			WeaponTags.Solar_Power_Cannon_Tag,
			WeaponTags.Water_Rifle_Tag,
			WeaponTags.Valentine_Shotgun_Tag,
			WeaponTags.Needle_Throw_Tag,
			WeaponTags.Needle_Throw_Tag,
			WeaponTags.Carrot_Sword_Tag,
			WeaponTags._3_shotgun_3_Tag,
			WeaponTags.plazma_3_Tag,
			WeaponTags.katana_3_Tag,
			WeaponTags.DragonGun_Tag,
			WeaponTags.Bazooka_2_3_Tag,
			WeaponTags.buddy_Tag,
			WeaponTags.barret_3_Tag,
			WeaponTags.Flamethrower_3_Tag,
			WeaponTags.SparklyBlasterTag,
			WeaponTags.Thompson_2_Tag
		};
		_migrationsCompletedAtThisLaunch = false;
		weaponsMovedToSniperCategory = new List<string>
		{
			"Weapon299", "Weapon322", "Weapon323", CampaignRifle_WN, "Weapon44", "Weapon46", "Weapon61", "Weapon256", "Weapon77", "Weapon209",
			"Weapon65", "Weapon27", "Weapon63", "Weapon134", "Weapon37", "Weapon268", "Weapon121", "Weapon210", "Weapon251", "Weapon128",
			"Weapon269", "Weapon122", "Weapon211", "Weapon271", "Weapon221", "Weapon188", "Weapon192", "Weapon129", "Weapon241"
		};
		dpsComparerWS = delegate(WeaponSounds leftWS, WeaponSounds rightWS)
		{
			//Discarded unreachable code: IL_01cf, IL_01dd
			if (ExpController.Instance == null || leftWS == null || rightWS == null)
			{
				return 0;
			}
			float num = leftWS.DPS - rightWS.DPS;
			if (num > 0f)
			{
				return 1;
			}
			if (num < 0f)
			{
				return -1;
			}
			try
			{
				ItemRecord byPrefabName = ItemDb.GetByPrefabName(leftWS.name.Replace("(Clone)", string.Empty));
				ItemPrice itemPrice = ((!byPrefabName.CanBuy) ? new ItemPrice(10, "Coins") : byPrefabName.Price);
				ItemRecord byPrefabName2 = ItemDb.GetByPrefabName(rightWS.name.Replace("(Clone)", string.Empty));
				ItemPrice itemPrice2 = ((!byPrefabName2.CanBuy) ? new ItemPrice(10, "Coins") : byPrefabName2.Price);
				if (itemPrice.Currency == "GemsCurrency" && itemPrice2.Currency == "Coins")
				{
					return 1;
				}
				if (itemPrice.Currency == "Coins" && itemPrice2.Currency == "GemsCurrency")
				{
					return -1;
				}
				if (itemPrice.Price.CompareTo(itemPrice2.Price) != 0)
				{
					return itemPrice.Price.CompareTo(itemPrice2.Price);
				}
				return Array.IndexOf(WeaponComparer.multiplayerWeaponsOrd, ItemDb.GetByPrefabName(leftWS.name.Replace("(Clone)", string.Empty)).Tag).CompareTo(Array.IndexOf(WeaponComparer.multiplayerWeaponsOrd, ItemDb.GetByPrefabName(rightWS.name.Replace("(Clone)", string.Empty)).Tag));
			}
			catch
			{
				return 0;
			}
		};
		_sharedStringBuilder = new StringBuilder();
		WeaponManager.WeaponEquipped = null;
		WeaponManager.WeaponEquipped_AllCases = null;
		_buffsPAramsInitialized = false;
		_defaultTryGunsTable = new Dictionary<ShopNGUIController.CategoryNames, List<List<string>>>
		{
			{
				ShopNGUIController.CategoryNames.PrimaryCategory,
				new List<List<string>>
				{
					new List<string> { "Weapon127", "Weapon142", "Weapon206", "Weapon167" },
					new List<string> { "Weapon163", "Weapon141" },
					new List<string> { "Weapon84" },
					new List<string>(),
					new List<string>(),
					new List<string> { "Weapon220" },
					new List<string> { "Weapon521" }
				}
			},
			{
				ShopNGUIController.CategoryNames.BackupCategory,
				new List<List<string>>
				{
					new List<string> { "Weapon160", "Weapon203" },
					new List<string>(),
					new List<string>(),
					new List<string>(),
					new List<string> { "Weapon308" },
					new List<string> { "Weapon223" }
				}
			},
			{
				ShopNGUIController.CategoryNames.MeleeCategory,
				new List<List<string>>
				{
					new List<string>(),
					new List<string>(),
					new List<string>(),
					new List<string>(),
					new List<string>(),
					new List<string>()
				}
			},
			{
				ShopNGUIController.CategoryNames.SpecilCategory,
				new List<List<string>>
				{
					new List<string> { "Weapon178" },
					new List<string> { "Weapon105" },
					new List<string>(),
					new List<string>(),
					new List<string> { "Weapon306" },
					new List<string>()
				}
			},
			{
				ShopNGUIController.CategoryNames.SniperCategory,
				new List<List<string>>
				{
					new List<string> { "Weapon77", "Weapon209" },
					new List<string> { "Weapon339" },
					new List<string>(),
					new List<string> { "Weapon251" },
					new List<string>(),
					new List<string> { "Weapon221" }
				}
			},
			{
				ShopNGUIController.CategoryNames.PremiumCategory,
				new List<List<string>>
				{
					new List<string> { "Weapon82", "Weapon212" },
					new List<string> { "Weapon180" },
					new List<string> { "Weapon133", "Weapon253", "Weapon99" },
					new List<string>(),
					new List<string> { "Weapon161" },
					new List<string>()
				}
			}
		};
		ItemDb.Fill_tagToStoreIDMapping(tagToStoreIDMapping);
		ItemDb.Fill_storeIDtoDefsSNMapping(storeIDtoDefsSNMapping);
		_purchasableWeaponSet.UnionWith(storeIDtoDefsSNMapping.Values);
	}

	public long DiscountForTryGun(string tg)
	{
		if (tg == null)
		{
			return 0L;
		}
		if (tryGunDiscounts == null || !tryGunDiscounts.ContainsKey(tg))
		{
			return BaseTryGunDiscount();
		}
		return tryGunDiscounts[tg].Value;
	}

	public void AddTryGunPromo(string tg)
	{
		if (tg == null)
		{
			Debug.LogError("AddTryGunPromo tg == null");
			return;
		}
		tryGunPromos.Add(tg, PromoActionsManager.CurrentUnixTime);
		int b = BaseTryGunDiscount();
		try
		{
			ItemRecord byTag = ItemDb.GetByTag(tg);
			string currency = byTag.Price.Currency;
			int @int = Storager.getInt(currency, false);
			int num = ShopNGUIController.PriceIfGunWillBeTryGun(tg);
			bool flag = currency == "GemsCurrency";
			IList<PurchaseEventArgs> list;
			if (flag)
			{
				IList<PurchaseEventArgs> gemsPurchasesInfo = AbstractBankView.gemsPurchasesInfo;
				list = gemsPurchasesInfo;
			}
			else
			{
				list = AbstractBankView.goldPurchasesInfo;
			}
			int index = list[0].Index;
			int num2 = ((!flag) ? VirtualCurrencyHelper.GetCoinInappsQuantity(index) : VirtualCurrencyHelper.GetGemsInappsQuantity(index));
			if (num > @int + num2)
			{
				int num3 = @int + num2 - 1;
				ItemPrice itemPrice = ShopNGUIController.GetItemPrice(tg, (ShopNGUIController.CategoryNames)ItemDb.GetItemCategory(tg), false, false);
				b = (int)((1f - (float)num3 / (float)itemPrice.Price) * 100f) + 1;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in AddTryGunPromo: " + ex);
		}
		b = Mathf.Min(70, b);
		tryGunDiscounts.Add(tg, new SaltedLong(685488L, b));
	}

	public static int BaseTryGunDiscount()
	{
		int num = ((!ABTestController.useBuffSystem) ? 50 : 50);
		try
		{
			num = ((!ABTestController.useBuffSystem) ? KillRateCheck.instance.discountValue : BuffSystem.instance.discountValue);
			num = Math.Max(0, num);
			num = Math.Min(75, num);
			return num;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in getting KillRateCheck.instance.discountValue: " + ex);
			return num;
		}
	}

	public void AddTryGun(string tg)
	{
		try
		{
			int value = 3;
			try
			{
				value = ((!ABTestController.useBuffSystem) ? KillRateCheck.instance.GetRoundsForGun() : BuffSystem.instance.GetRoundsForGun());
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in numOfMatches = KillRateCheck.instance.GetRoundsForGun(): " + ex);
			}
			TryGuns.Add(tg, new Dictionary<string, object> { 
			{
				"NumberOfMatchesKey",
				new SaltedInt(52394, value)
			} });
			Weapon weapon = AddWeaponWithTagToAllAvailable(tg);
			WeaponSounds weaponWS = weapon.weaponPrefab.GetComponent<WeaponSounds>();
			string text = null;
			try
			{
				text = ItemDb.GetByPrefabName(playerWeapons.OfType<Weapon>().FirstOrDefault((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor == weaponWS.categoryNabor).weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag;
			}
			catch (Exception ex2)
			{
				Debug.LogWarning("Exception in try guns get equipped before: " + ex2);
			}
			TryGuns[tg].Add("EquippedBeforeKey", text ?? string.Empty);
			EquipWeapon(weapon);
			sharedManager.SaveWeaponAsLastUsed(sharedManager.CurrentWeaponIndex);
		}
		catch (Exception ex3)
		{
			Debug.LogError("Exception in AddTryGun: " + ex3);
		}
	}

	public void DecreaseTryGunsMatchesCount()
	{
		if (Defs.isHunger)
		{
			return;
		}
		try
		{
			List<string> list = new List<string>();
			KeyValuePair<string, Dictionary<string, object>> tryGunKvp;
			foreach (KeyValuePair<string, Dictionary<string, object>> tryGun in TryGuns)
			{
				tryGunKvp = tryGun;
				if (!(weaponsInGame.FirstOrDefault((UnityEngine.Object w) => ItemDb.GetByPrefabName(w.name).Tag == tryGunKvp.Key) == null))
				{
					int num = Math.Max(0, ((SaltedInt)tryGunKvp.Value["NumberOfMatchesKey"]).Value - 1);
					tryGunKvp.Value["NumberOfMatchesKey"] = new SaltedInt(838318, num);
					if (num == 0)
					{
						list.Add(tryGunKvp.Key);
					}
				}
			}
			foreach (string item in list)
			{
				RemoveTryGun(item);
			}
			if (list.Count > 0)
			{
				Action tryGunRemoved = WeaponManager.TryGunRemoved;
				if (tryGunRemoved != null)
				{
					tryGunRemoved();
				}
				if (ABTestController.useBuffSystem)
				{
					BuffSystem.instance.OnGunTakeOff();
				}
				else
				{
					KillRateCheck.OnGunTakeOff();
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in DecreaseTryGunsMatchesCount: " + ex);
		}
	}

	public bool IsAvailableTryGun(string tryGunTag)
	{
		//Discarded unreachable code: IL_0071, IL_008e
		try
		{
			return tryGunTag != null && TryGuns != null && TryGuns.Keys.Contains(tryGunTag) && TryGuns[tryGunTag].ContainsKey("NumberOfMatchesKey") && ((SaltedInt)TryGuns[tryGunTag]["NumberOfMatchesKey"]).Value > 0;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in IsAvailableTryGun: " + ex);
			return false;
		}
	}

	public void RemoveTryGun(string tryGunTag)
	{
		if (TryGuns == null || !TryGuns.ContainsKey(tryGunTag))
		{
			return;
		}
		try
		{
			Dictionary<string, object> dict = TryGuns[tryGunTag];
			string value;
			if (dict.TryGetValue<string>("EquippedBeforeKey", out value))
			{
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						string lastBoughtTag = LastBoughtTag(value);
						Weapon weapon = allAvailablePlayerWeapons.OfType<Weapon>().FirstOrDefault((Weapon w) => ItemDb.GetByPrefabName(w.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag == lastBoughtTag);
						if (weapon != null)
						{
							EquipWeapon(weapon);
						}
						else
						{
							int cat = ItemDb.GetItemCategory(lastBoughtTag);
							Weapon weapon2 = allAvailablePlayerWeapons.OfType<Weapon>().FirstOrDefault((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == cat && ItemDb.GetByPrefabName(w.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag != tryGunTag && !IsAvailableTryGun(tryGunTag));
							if (weapon2 != null)
							{
								EquipWeapon(weapon2);
							}
							else
							{
								SaveWeaponSet(Defs.CampaignWSSN, string.Empty, cat);
								int num = -1;
								for (int i = 0; i < playerWeapons.Count; i++)
								{
									if (playerWeapons[i] != null && ItemDb.GetByPrefabName(((Weapon)playerWeapons[i]).weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag == tryGunTag)
									{
										num = i;
										break;
									}
								}
								if (num != -1)
								{
									playerWeapons.RemoveAt(num);
								}
								else
								{
									Debug.LogError("RemoveTryGun: error removing weapon from playerWeapons");
								}
								SetWeaponsSet(0);
								if (cat == 4)
								{
									SaveWeaponSet("WeaponManager.SniperModeWSSN", CampaignRifle_WN, cat);
								}
								if (cat == 2)
								{
									SaveWeaponSet("WeaponManager.KnifesModeWSSN", KnifeWN, cat);
								}
								SaveWeaponSet(Defs.MultiplayerWSSN, _KnifeAndPistolAndMP5AndSniperAndRocketnitzaSet().Split("#"[0])[cat], cat);
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("tryGun.TryGetValue(EquippedBeforeKey, out gunBefore) exception: " + ex);
					}
				}
			}
			else
			{
				Debug.LogError("RemoveTryGun: No EquippedBeforeKey for " + tryGunTag);
			}
			allAvailablePlayerWeapons = new ArrayList((from w in allAvailablePlayerWeapons.OfType<Weapon>()
				where ItemDb.GetByPrefabName(w.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag != tryGunTag
				select w).ToList());
			TryGuns.Remove(tryGunTag);
			if (!ExpiredTryGuns.Contains(tryGunTag))
			{
				ExpiredTryGuns.Add(tryGunTag);
			}
			if (ShopNGUIController.sharedShop != null && ShopNGUIController.GuiActive)
			{
				ShopNGUIController.sharedShop.ReloadGridOrCarousel(ShopNGUIController.sharedShop.CurrentItem);
				ShopNGUIController.sharedShop.UpdateIcons(false);
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError("Exception in RemoveTryGun: " + ex2);
		}
	}

	private void SaveTryGunsDiscounts()
	{
		try
		{
			Storager.setString("WeaponManager.TryGunsDiscountsKey", Rilisoft.MiniJson.Json.Serialize(tryGunPromos), false);
			Storager.setString("WeaponManager.TryGunsDiscountsValuesKey", Rilisoft.MiniJson.Json.Serialize(tryGunDiscounts.ToDictionary((KeyValuePair<string, SaltedLong> kvp) => kvp.Key, (KeyValuePair<string, SaltedLong> kvp) => kvp.Value.Value)), false);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in SaveTryGunsDiscounts: " + ex);
		}
	}

	private void SaveTryGunsInfo()
	{
		try
		{
			Dictionary<string, Dictionary<string, object>> value = TryGuns.Select((KeyValuePair<string, Dictionary<string, object>> kvp) => new KeyValuePair<string, Dictionary<string, object>>(kvp.Key, new Dictionary<string, object>
			{
				{
					"NumberOfMatchesKey",
					((SaltedInt)kvp.Value["NumberOfMatchesKey"]).Value
				},
				{
					"EquippedBeforeKey",
					kvp.Value["EquippedBeforeKey"]
				}
			})).ToDictionary((KeyValuePair<string, Dictionary<string, object>> kvp) => kvp.Key, (KeyValuePair<string, Dictionary<string, object>> kvp) => kvp.Value);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("TryGunsDictionaryKey", value);
			dictionary.Add("ExpiredTryGunsListKey", ExpiredTryGuns);
			Dictionary<string, object> obj = dictionary;
			Storager.setString("WeaponManager.TryGunsKey", Rilisoft.MiniJson.Json.Serialize(obj), false);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in SaveTryGunsInfo: " + ex);
		}
	}

	private void LoadTryGunDiscounts()
	{
		try
		{
			if (!Storager.hasKey("WeaponManager.TryGunsDiscountsKey"))
			{
				Storager.setString("WeaponManager.TryGunsDiscountsKey", "{}", false);
			}
			Dictionary<string, object> dictionary = Rilisoft.MiniJson.Json.Deserialize(Storager.getString("WeaponManager.TryGunsDiscountsKey", false)) as Dictionary<string, object>;
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				tryGunPromos.Add(item.Key, (long)item.Value);
			}
			if (!Storager.hasKey("WeaponManager.TryGunsDiscountsValuesKey"))
			{
				Storager.setString("WeaponManager.TryGunsDiscountsValuesKey", "{}", false);
			}
			Dictionary<string, object> dictionary2 = Rilisoft.MiniJson.Json.Deserialize(Storager.getString("WeaponManager.TryGunsDiscountsValuesKey", false)) as Dictionary<string, object>;
			foreach (KeyValuePair<string, object> item2 in dictionary2)
			{
				tryGunDiscounts.Add(item2.Key, new SaltedLong(17425L, (int)(long)item2.Value));
			}
			RemoveExpiredPromosForTryGuns();
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in LoadTryGunDiscounts: " + ex);
		}
	}

	private void LoadTryGunsInfo()
	{
		try
		{
			if (!Storager.hasKey("WeaponManager.TryGunsKey"))
			{
				Storager.setString("WeaponManager.TryGunsKey", "{}", false);
			}
			Dictionary<string, object> dictionary = Rilisoft.MiniJson.Json.Deserialize(Storager.getString("WeaponManager.TryGunsKey", false)) as Dictionary<string, object>;
			object value;
			if (dictionary.TryGetValue("TryGunsDictionaryKey", out value))
			{
				TryGuns = (value as Dictionary<string, object>).Select(delegate(KeyValuePair<string, object> kvp)
				{
					Dictionary<string, object> value2 = new Dictionary<string, object>
					{
						{
							"NumberOfMatchesKey",
							new SaltedInt(52394, (int)(long)(kvp.Value as Dictionary<string, object>)["NumberOfMatchesKey"])
						},
						{
							"EquippedBeforeKey",
							(kvp.Value as Dictionary<string, object>)["EquippedBeforeKey"]
						}
					};
					return new KeyValuePair<string, Dictionary<string, object>>(kvp.Key, value2);
				}).ToDictionary((KeyValuePair<string, Dictionary<string, object>> kvp) => kvp.Key, (KeyValuePair<string, Dictionary<string, object>> kvp) => kvp.Value);
			}
			if (dictionary.ContainsKey("ExpiredTryGunsListKey"))
			{
				ExpiredTryGuns = (dictionary["ExpiredTryGunsListKey"] as List<object>).OfType<string>().ToList();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in LoadTryGunsInfo: " + ex);
		}
	}

	public void RemoveDiscountForTryGun(string tg)
	{
		tryGunPromos.Remove(tg);
		tryGunDiscounts.Remove(tg);
	}

	public bool IsWeaponDiscountedAsTryGun(string tg)
	{
		return tryGunPromos != null && tryGunPromos.ContainsKey(tg);
	}

	public long StartTimeForTryGunDiscount(string tg)
	{
		if (tg != null && tryGunPromos != null && tryGunPromos.ContainsKey(tg))
		{
			return tryGunPromos[tg];
		}
		return 0L;
	}

	public static float TryGunPromoDuration()
	{
		float num = ((!ABTestController.useBuffSystem) ? 3600 : 3600);
		try
		{
			num = ((!ABTestController.useBuffSystem) ? KillRateCheck.instance.timeForDiscount : BuffSystem.instance.timeForDiscount);
			num = Math.Max(60f, num);
			return num;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in duration = KillRateCheck.instance.timeForDiscount: " + ex);
			return num;
		}
	}

	public void RemoveExpiredPromosForTryGuns()
	{
		try
		{
			float duration = TryGunPromoDuration();
			List<KeyValuePair<string, long>> list = tryGunPromos.Where((KeyValuePair<string, long> kvp) => (float)(PromoActionsManager.CurrentUnixTime - kvp.Value) >= duration).ToList();
			foreach (KeyValuePair<string, long> item in list)
			{
				RemoveDiscountForTryGun(item.Key);
			}
			if (list.Count() > 0)
			{
				if (ShopNGUIController.sharedShop != null && ShopNGUIController.GuiActive)
				{
					ShopNGUIController.sharedShop.UpdateButtons();
				}
				Action tryGunExpired = WeaponManager.TryGunExpired;
				if (tryGunExpired != null)
				{
					tryGunExpired();
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in RemoveExpiredPromosForTryGuns: " + ex);
		}
	}

	private IEnumerator Step()
	{
		while (true)
		{
			yield return StartCoroutine(CoroutineRunner.WaitForSeconds(1f));
			RemoveExpiredPromosForTryGuns();
		}
	}

	public static Dictionary<ShopNGUIController.CategoryNames, List<string>> GetWeaponTagsByCategoriesFromItems(List<string> items)
	{
		//Discarded unreachable code: IL_0077
		try
		{
			return (from weaponTag in items.Intersect(ItemDb.RecordsByTag.Keys)
				group weaponTag by (ShopNGUIController.CategoryNames)(ItemDb.GetWeaponInfo(weaponTag).categoryNabor - 1)).ToDictionary((IGrouping<ShopNGUIController.CategoryNames, string> grouping) => grouping.Key, (IGrouping<ShopNGUIController.CategoryNames, string> grouping) => grouping.ToList());
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in GetWeaponTagsFromItems: {0}", ex);
		}
		return new Dictionary<ShopNGUIController.CategoryNames, List<string>>();
	}

	public static List<string> GetNewWeaponsForTier(int tier)
	{
		//Discarded unreachable code: IL_007d
		try
		{
			IEnumerable<ItemRecord> canBuyWeapon = ItemDb.GetCanBuyWeapon(false);
			IEnumerable<ItemRecord> source = canBuyWeapon.Where((ItemRecord record) => ItemDb.GetWeaponInfo(record.Tag).tier == tier);
			return (from record in source.Where(delegate(ItemRecord record)
				{
					List<string> list = WeaponUpgrades.ChainForTag(record.Tag);
					return list == null || (list.Count > 0 && list[0] == record.Tag);
				})
				select record.Tag).ToList();
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in GetNewWeaponsForTier: {0}", ex);
		}
		return new List<string>();
	}

	public void UnloadAll()
	{
		_rocketCache = null;
		_turretCache = null;
		_playerWeaponsSetInnerPrefabsCache.Clear();
		_turretWeaponCache = null;
		_playerWeapons.Clear();
		_allAvailablePlayerWeapons.Clear();
		_weaponsInGame = null;
		Resources.UnloadUnusedAssets();
	}

	public static bool IsExclusiveWeapon(string weaponTag)
	{
		return GotchaGuns.Contains(weaponTag) || weaponTag == SocialGunWN;
	}

	public static void ProvideExclusiveWeaponByTag(string weaponTag)
	{
		if (string.IsNullOrEmpty(weaponTag))
		{
			Debug.LogError("Error in ProvideExclusiveWeaponByTag: string.IsNullOrEmpty(weaponTag)");
			return;
		}
		if (Storager.getInt(weaponTag, true) > 0)
		{
			Debug.LogError("Error in ProvideExclusiveWeaponByTag: Storager.getInt (weaponTag, true) > 0");
			return;
		}
		ItemRecord byTag = ItemDb.GetByTag(weaponTag);
		if (byTag == null)
		{
			Debug.LogError("Error in ProvideExclusiveWeaponByTag: weaponRecord == null");
			return;
		}
		if (byTag.PrefabName == null)
		{
			Debug.LogError("Error in ProvideExclusiveWeaponByTag: weaponRecord.PrefabName == null");
			return;
		}
		Storager.setInt(weaponTag, 1, true);
		AddExclusiveWeaponToWeaponStructures(byTag.PrefabName);
	}

	public static void RefreshExpControllers()
	{
		if (ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.Refresh();
		}
		else
		{
			Debug.LogError("RefreshLevelAndSetRememberedTiersFromCloud ExperienceController.sharedController == null");
		}
		if (ExpController.Instance != null)
		{
			ExpController.Instance.Refresh();
		}
		else
		{
			Debug.LogError("RefreshLevelAndSetRememberedTiersFromCloud ExpController.Instance == null");
		}
	}

	public static void RefreshLevelAndSetRememberedTiersFromCloud(List<string> weaponsForWhichSetRememberedTier)
	{
		try
		{
			SetRememberedTiersForWeaponsComesFromCloud(weaponsForWhichSetRememberedTier);
		}
		catch (Exception ex)
		{
			Debug.LogError("RefreshLevelAndSetRememberedTiersFromCloud exception: " + ex);
		}
	}

	public static void SetRememberedTiersForWeaponsComesFromCloud(List<string> weaponsForWhichSetRememberedTier)
	{
		try
		{
			foreach (string item in weaponsForWhichSetRememberedTier)
			{
				ItemRecord byTag = ItemDb.GetByTag(item);
				if (byTag != null)
				{
					string prefabName = byTag.PrefabName;
					if (prefabName != null)
					{
						SetRememberedTierForWeapon(prefabName);
					}
					else
					{
						Debug.LogError("SetRememberedTiersForWeaponsComesFromCloud prefabName == null");
					}
				}
				else
				{
					Debug.LogWarning("SetRememberedTiersForWeaponsComesFromCloud record == null");
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("SetRememberedTiersForWeaponsComesFromCloud exception: " + ex);
		}
	}

	public static void SetRememberedTierForWeapon(string prefabName)
	{
		Storager.setInt("RememberedTierWhenObtainGun_" + prefabName, ExpController.OurTierForAnyPlace(), false);
	}

	public static void AddExclusiveWeaponToWeaponStructures(string prefabName)
	{
		if (string.IsNullOrEmpty(prefabName))
		{
			Debug.LogError("Error in AddExclusiveWeaponToWeaponStructures: string.IsNullOrEmpty(prefabName)");
			return;
		}
		SetRememberedTierForWeapon(prefabName);
		if (!(sharedManager != null) || !sharedManager.Initialized)
		{
			return;
		}
		GameObject gameObject = null;
		try
		{
			gameObject = sharedManager.weaponsInGame.OfType<GameObject>().FirstOrDefault((GameObject w) => w.name.Equals(prefabName));
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in AddExclusiveWeaponToWeaponStructures: " + ex);
		}
		if (gameObject != null)
		{
			int score;
			sharedManager.AddWeapon(gameObject, out score);
		}
		if (ShopNGUIController.sharedShop != null && ShopNGUIController.GuiActive)
		{
			ShopNGUIController.sharedShop.UpdateIcons(false);
		}
	}

	public static GameObject AddRay(Vector3 pos, Vector3 forw, string nm, float len = 150f)
	{
		GameObject objectFromName = RayAndExplosionsStackController.sharedController.GetObjectFromName(ResPath.Combine("Rays", nm));
		if (objectFromName == null)
		{
			return null;
		}
		Transform transform = objectFromName.transform;
		Transform transform2 = ((transform.childCount <= 0) ? null : transform.GetChild(0));
		if (transform2 != null)
		{
			transform2.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0f, 0f, len));
		}
		objectFromName.transform.position = pos;
		objectFromName.transform.forward = forw;
		return objectFromName;
	}

	public static void SetGunFlashActive(GameObject gunFlash, bool _a)
	{
		if (!(gunFlash == null))
		{
			Transform transform = null;
			if (gunFlash.transform.childCount > 0)
			{
				transform = gunFlash.transform.GetChild(0);
			}
			if (transform != null && transform.gameObject.activeSelf != _a)
			{
				transform.gameObject.SetActive(_a);
			}
		}
	}

	public static void ClearCachedInnerPrefabs()
	{
		cachedInnerPrefabsForCurrentShopCategory.Clear();
	}

	public static GameObject InnerPrefabForWeaponBuffered(GameObject weapon)
	{
		return LoadAsyncTool.Get(Defs.InnerWeaponsFolder + "/" + weapon.name + Defs.InnerWeapons_Suffix, true).asset as GameObject;
	}

	public static string FirstUnboughtOrForOurTier(string tg)
	{
		string text = FirstUnboughtTag(tg);
		if (tagToStoreIDMapping.ContainsKey(tg))
		{
			string text2 = FirstTagForOurTier(tg);
			List<string> list = WeaponUpgrades.ChainForTag(tg);
			if (text2 != null && list != null && list.IndexOf(text2) > list.IndexOf(text))
			{
				text = text2;
			}
		}
		return text;
	}

	public static ResourceRequest InnerPrefabForWeaponAsync(string weapon)
	{
		return Resources.LoadAsync<GameObject>(Defs.InnerWeaponsFolder + "/" + weapon + Defs.InnerWeapons_Suffix);
	}

	public static bool PurchasableWeaponSetContains(string weaponTag)
	{
		return _purchasableWeaponSet.Contains(weaponTag);
	}

	public void SaveWeaponAsLastUsed(int index)
	{
		if (!Defs.isMulti || (Defs.isHunger && !(SceneLoader.ActiveSceneName == "ConnectScene") && !(SceneLoader.ActiveSceneName == "ConnectSceneSandbox")) || playerWeapons == null || playerWeapons.Count <= index || index < 0)
		{
			return;
		}
		try
		{
			int value = (playerWeapons[index] as Weapon).weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1;
			if (lastUsedWeaponsForFilterMaps.ContainsKey(_currentFilterMap.ToString()))
			{
				lastUsedWeaponsForFilterMaps[_currentFilterMap.ToString()] = value;
			}
			else
			{
				lastUsedWeaponsForFilterMaps.Add(_currentFilterMap.ToString(), value);
			}
		}
		catch (Exception)
		{
			Debug.LogError("Exception in SaveWeaponAsLastUsed index = " + index);
		}
	}

	public int CurrentIndexOfLastUsedWeaponInPlayerWeapons()
	{
		if (Defs.isHunger)
		{
			return 0;
		}
		int result = 0;
		try
		{
			if (lastUsedWeaponsForFilterMaps.ContainsKey(_currentFilterMap.ToString()))
			{
				int lastUsedCategory = lastUsedWeaponsForFilterMaps[_currentFilterMap.ToString()];
				int num = playerWeapons.Cast<Weapon>().ToList().FindIndex((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == lastUsedCategory);
				if (num != -1)
				{
					return num;
				}
				return result;
			}
			return result;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in CurrentIndexOfLastUsedWeaponInPlayerWeapons: " + ex);
			return 0;
		}
	}

	private void UpdateFilteredShopLists()
	{
		FilteredShopListsForPromos = new List<List<GameObject>>();
		FilteredShopListsNoUpgrades = new List<List<GameObject>>();
		for (int i = 0; i < _weaponsByCat.Count; i++)
		{
			FilteredShopListsForPromos.Add(new List<GameObject>());
			FilteredShopListsNoUpgrades.Add(new List<GameObject>());
			for (int j = 0; j < _weaponsByCat[i].Count; j++)
			{
				bool flag = true;
				bool flag2 = true;
				try
				{
					ItemRecord byPrefabName = ItemDb.GetByPrefabName(_weaponsByCat[i][j].name.Replace("(Clone)", string.Empty));
					if (byPrefabName.CanBuy)
					{
						List<string> list = WeaponUpgrades.ChainForTag(byPrefabName.Tag);
						string text = ((list == null || list.Count <= 0) ? byPrefabName.Tag : list[0]);
						ItemRecord byTag = ItemDb.GetByTag(text);
						string text2 = LastBoughtTag(byPrefabName.Tag);
						if ((Storager.getInt(byPrefabName.StorageId, true) == 0 && list != null && list.IndexOf(byPrefabName.Tag) > list.IndexOf(FirstTagForOurTier(byPrefabName.Tag))) || (text2 != null && list != null && list.IndexOf(text2) > list.IndexOf(byPrefabName.Tag)))
						{
							flag2 = false;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in UpdateFilteredShopLists: " + ex);
				}
				if (flag)
				{
					FilteredShopListsForPromos[i].Add(_weaponsByCat[i][j]);
				}
				if (flag2)
				{
					FilteredShopListsNoUpgrades[i].Add(_weaponsByCat[i][j]);
				}
			}
		}
	}

	public void SaveWeaponSet(string sn, string wn, int pos)
	{
		string text = LoadWeaponSet(sn);
		string[] array = text.Split('#');
		array[pos] = wn;
		string text2 = string.Join("#", array);
		if (!Application.isEditor)
		{
			if (!Storager.hasKey(sn))
			{
			}
			Storager.setString(sn, text2, false);
		}
		else
		{
			PlayerPrefs.SetString(sn, text2);
		}
	}

	public static string _KnifeSet()
	{
		return "##" + KnifeWN + "###";
	}

	public static string _KnifeAndPistolSet()
	{
		return "#" + PistolWN + "#" + KnifeWN + "###";
	}

	public static string _KnifeAndPistolAndShotgunSet()
	{
		return ShotgunWN + "#" + PistolWN + "#" + KnifeWN + "###";
	}

	public static string _KnifeAndPistolAndSniperSet()
	{
		return "#" + PistolWN + "#" + KnifeWN + "##" + CampaignRifle_WN + "#";
	}

	public static string _KnifeAndPistolAndMP5AndSniperAndRocketnitzaSet()
	{
		return MP5WN + "#" + PistolWN + "#" + KnifeWN + "#" + ((!TrainingController.TrainingCompleted) ? string.Empty : SimpleFlamethrower_WN) + "#" + ((!TrainingController.TrainingCompleted) ? string.Empty : CampaignRifle_WN) + "#" + ((!TrainingController.TrainingCompleted) ? string.Empty : Rocketnitza_WN);
	}

	public static string _InitialDaterSet()
	{
		return "##" + DaterFreeWeaponPrefabName + "###";
	}

	private string DefaultSetForWeaponSetSettingName(string sn)
	{
		string result = _KnifeAndPistolAndShotgunSet();
		if (sn != Defs.CampaignWSSN)
		{
			try
			{
				result = WeaponSetSettingNamesForFilterMaps.Where((KeyValuePair<int, FilterMapSettings> kvp) => kvp.Value.settingName == sn).FirstOrDefault().Value.defaultWeaponSet();
				return result;
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LoadWeaponSet: sn = " + sn + "    exception: " + ex);
				return result;
			}
		}
		return result;
	}

	public string LoadWeaponSet(string sn)
	{
		if (!Application.isEditor)
		{
			if (!Storager.hasKey(sn))
			{
				Storager.setString(sn, DefaultSetForWeaponSetSettingName(sn), false);
			}
			return Storager.getString(sn, false);
		}
		return PlayerPrefs.GetString(sn, DefaultSetForWeaponSetSettingName(sn));
	}

	public void SetWeaponsSet(int filterMap = 0)
	{
		_playerWeapons.Clear();
		bool isMulti = Defs.isMulti;
		bool isHunger = Defs.isHunger;
		string text = null;
		if (!isMulti)
		{
			text = ((!Defs.IsSurvival && TrainingController.TrainingCompleted) ? LoadWeaponSet(Defs.CampaignWSSN) : ((!Defs.IsSurvival || !TrainingController.TrainingCompleted) ? _KnifeAndPistolSet() : LoadWeaponSet(Defs.MultiplayerWSSN)));
		}
		else if (!isHunger)
		{
			if (WeaponSetSettingNamesForFilterMaps.ContainsKey(filterMap))
			{
				text = LoadWeaponSet(WeaponSetSettingNamesForFilterMaps[filterMap].settingName);
			}
			else
			{
				Debug.LogError("WeaponSetSettingNamesForFilterMaps.ContainsKey (filterMap): filterMap = " + filterMap);
			}
		}
		else
		{
			text = _KnifeSet();
		}
		string[] array = text.Split('#');
		string[] array2 = array;
		foreach (string value in array2)
		{
			if (string.IsNullOrEmpty(value))
			{
				continue;
			}
			foreach (Weapon allAvailablePlayerWeapon in allAvailablePlayerWeapons)
			{
				if (allAvailablePlayerWeapon.weaponPrefab.name.Equals(value))
				{
					EquipWeapon(allAvailablePlayerWeapon, false);
					break;
				}
			}
		}
		if (filterMap == 2)
		{
			foreach (Weapon allAvailablePlayerWeapon2 in allAvailablePlayerWeapons)
			{
				if (allAvailablePlayerWeapon2.weaponPrefab.name.Equals(KnifeWN))
				{
					EquipWeapon(allAvailablePlayerWeapon2, false);
					break;
				}
			}
			foreach (Weapon allAvailablePlayerWeapon3 in allAvailablePlayerWeapons)
			{
				if (allAvailablePlayerWeapon3.weaponPrefab.name.Equals(PistolWN))
				{
					EquipWeapon(allAvailablePlayerWeapon3, false);
					break;
				}
			}
		}
		if (filterMap == 2 && playerWeapons.Count == 2)
		{
			foreach (Weapon allAvailablePlayerWeapon4 in allAvailablePlayerWeapons)
			{
				if (allAvailablePlayerWeapon4.weaponPrefab.name.Equals(CampaignRifle_WN))
				{
					EquipWeapon(allAvailablePlayerWeapon4, false);
					break;
				}
			}
		}
		if (filterMap == 3 && playerWeapons.OfType<Weapon>().FirstOrDefault((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == 2) == null)
		{
			foreach (Weapon allAvailablePlayerWeapon5 in allAvailablePlayerWeapons)
			{
				if (allAvailablePlayerWeapon5.weaponPrefab.name.Equals(DaterFreeWeaponPrefabName))
				{
					EquipWeapon(allAvailablePlayerWeapon5, false);
					break;
				}
			}
		}
		if (playerWeapons.Count == 0)
		{
			UpdatePlayersWeaponSetCache();
		}
	}

	public static string LastBoughtTag(string tg, List<string> weaponUpgradesHint = null)
	{
		if (tg == null)
		{
			return null;
		}
		if (tg == "Armor_Novice")
		{
			return (!ShopNGUIController.NoviceArmorAvailable) ? null : "Armor_Novice";
		}
		if (tagToStoreIDMapping.ContainsKey(tg))
		{
			bool flag = weaponUpgradesHint != null;
			List<string> list = weaponUpgradesHint;
			if (list == null)
			{
				foreach (List<string> upgrade in WeaponUpgrades.upgrades)
				{
					if (upgrade.Contains(tg))
					{
						list = upgrade;
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				for (int num = list.Count - 1; num >= 0; num--)
				{
					if (Storager.getInt(storeIDtoDefsSNMapping[tagToStoreIDMapping[list[num]]], true) == 1)
					{
						return list[num];
					}
				}
				return null;
			}
			bool flag2 = ItemDb.IsTemporaryGun(tg);
			if ((!flag2 && Storager.getInt(storeIDtoDefsSNMapping[tagToStoreIDMapping[tg]], true) == 1) || (flag2 && TempItemsController.sharedController.ContainsItem(tg)))
			{
				return tg;
			}
			return null;
		}
		foreach (KeyValuePair<ShopNGUIController.CategoryNames, List<List<string>>> item in Wear.wear)
		{
			foreach (List<string> item2 in item.Value)
			{
				if (!item2.Contains(tg))
				{
					continue;
				}
				if (TempItemsController.PriceCoefs.ContainsKey(tg))
				{
					return (!(TempItemsController.sharedController != null) || !TempItemsController.sharedController.ContainsItem(tg)) ? null : tg;
				}
				if (Storager.getInt(item2[0], true) == 0)
				{
					return null;
				}
				for (int i = 1; i < item2.Count; i++)
				{
					if (Storager.getInt(item2[i], true) == 0)
					{
						return item2[i - 1];
					}
				}
				return item2[item2.Count - 1];
			}
		}
		return tg;
	}

	public static string FirstUnboughtTag(string tg)
	{
		if (tg == null)
		{
			return null;
		}
		if (tg == "Armor_Novice")
		{
			return "Armor_Novice";
		}
		if (tagToStoreIDMapping.ContainsKey(tg))
		{
			bool flag = false;
			List<string> list = null;
			foreach (List<string> upgrade in WeaponUpgrades.upgrades)
			{
				if (upgrade.Contains(tg))
				{
					list = upgrade;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				for (int num = list.Count - 1; num >= 0; num--)
				{
					if (Storager.getInt(storeIDtoDefsSNMapping[tagToStoreIDMapping[list[num]]], true) == 1)
					{
						if (num < list.Count - 1)
						{
							return list[num + 1];
						}
						return list[num];
					}
				}
				return list[0];
			}
			return tg;
		}
		if (TempItemsController.PriceCoefs.ContainsKey(tg))
		{
			return tg;
		}
		foreach (KeyValuePair<ShopNGUIController.CategoryNames, List<List<string>>> item in Wear.wear)
		{
			foreach (List<string> item2 in item.Value)
			{
				if (!item2.Contains(tg))
				{
					continue;
				}
				for (int i = 0; i < item2.Count; i++)
				{
					if (Storager.getInt(item2[i], true) == 0)
					{
						return item2[i];
					}
				}
				return item2[item2.Count - 1];
			}
		}
		return tg;
	}

	private void UpdatePlayersWeaponSetCache()
	{
		if (Device.IsLoweMemoryDevice)
		{
			Resources.UnloadUnusedAssets();
		}
	}

	private void SetWeaponInAppropriateMultyModes(WeaponSounds ws)
	{
		List<int> list = new List<int>();
		list.Add(0);
		List<int> list2 = list.Concat((ws.filterMap == null) ? new int[0] : ws.filterMap).Distinct().ToList();
		foreach (int item in list2)
		{
			if (WeaponSetSettingNamesForFilterMaps.ContainsKey(item))
			{
				SaveWeaponSet(WeaponSetSettingNamesForFilterMaps[item].settingName, ws.gameObject.name, ws.categoryNabor - 1);
			}
			else
			{
				Debug.LogError("WeaponSetSettingNamesForFilterMaps.ContainsKey (mode): " + item);
			}
		}
	}

	public void EquipWeapon(Weapon w, bool shouldSave = true, bool shouldEquipToDaterSetOnly = false)
	{
		if (w == null)
		{
			Debug.LogWarning("Exiting from EquipWeapon(), because weapon is null.");
			return;
		}
		WeaponSounds component = w.weaponPrefab.GetComponent<WeaponSounds>();
		int categoryNabor = component.categoryNabor;
		bool flag = false;
		for (int i = 0; i < playerWeapons.Count; i++)
		{
			if ((playerWeapons[i] as Weapon).weaponPrefab.GetComponent<WeaponSounds>().categoryNabor == categoryNabor)
			{
				flag = true;
				playerWeapons[i] = w;
				UpdatePlayersWeaponSetCache();
				break;
			}
		}
		if (!flag)
		{
			playerWeapons.Add(w);
			UpdatePlayersWeaponSetCache();
		}
		playerWeapons.Sort(new WeaponComparer());
		playerWeapons.Reverse();
		CurrentWeaponIndex = playerWeapons.IndexOf(w);
		Action<WeaponSounds> weaponEquipped_AllCases = WeaponManager.WeaponEquipped_AllCases;
		if (weaponEquipped_AllCases != null)
		{
			weaponEquipped_AllCases(component);
		}
		if (!shouldSave)
		{
			return;
		}
		string[] array = Storager.getString(Defs.WeaponsGotInCampaign, false).Split('#');
		List<string> list = new List<string>();
		string[] array2 = array;
		foreach (string item in array2)
		{
			list.Add(item);
		}
		bool flag2 = (!(w.weaponPrefab.name == Rocketnitza_WN) || list.Contains(Rocketnitza_WN)) && (!w.weaponPrefab.name.Equals(MP5WN) || list.Contains(MP5WN)) && (!w.weaponPrefab.name.Equals(CampaignRifle_WN) || list.Contains(CampaignRifle_WN)) && (!w.weaponPrefab.name.Equals(SimpleFlamethrower_WN) || list.Contains(SimpleFlamethrower_WN));
		if (Defs.isMulti)
		{
			if (Defs.isHunger)
			{
				if (SceneLoader.ActiveSceneName == "ConnectScene" || SceneLoader.ActiveSceneName == "ConnectSceneSandbox")
				{
					SetWeaponInAppropriateMultyModes(component);
					if (flag2)
					{
						SaveWeaponSet(Defs.CampaignWSSN, w.weaponPrefab.name, categoryNabor - 1);
					}
				}
			}
			else
			{
				if (shouldEquipToDaterSetOnly && Defs.isDaterRegim)
				{
					SaveWeaponSet(Defs.DaterWSSN, w.weaponPrefab.name, categoryNabor - 1);
				}
				else
				{
					SetWeaponInAppropriateMultyModes(component);
				}
				if (flag2)
				{
					SaveWeaponSet(Defs.CampaignWSSN, w.weaponPrefab.name, categoryNabor - 1);
				}
			}
		}
		else if (!Defs.IsSurvival && TrainingController.TrainingCompleted)
		{
			if (!component.campaignOnly && !w.weaponPrefab.name.Equals(AlienGunWN))
			{
				SetWeaponInAppropriateMultyModes(component);
			}
			SaveWeaponSet(Defs.CampaignWSSN, w.weaponPrefab.name, categoryNabor - 1);
		}
		else if (Defs.IsSurvival && TrainingController.TrainingCompleted && !component.campaignOnly && !w.weaponPrefab.name.Equals(AlienGunWN))
		{
			SetWeaponInAppropriateMultyModes(component);
			if (flag2)
			{
				SaveWeaponSet(Defs.CampaignWSSN, w.weaponPrefab.name, categoryNabor - 1);
			}
		}
		if (WeaponManager.WeaponEquipped != null)
		{
			WeaponManager.WeaponEquipped(component);
		}
	}

	public void GetWeaponPrefabs(int filterMap = 0)
	{
		IEnumerator weaponPrefabsCoroutine = GetWeaponPrefabsCoroutine(filterMap);
		while (weaponPrefabsCoroutine.MoveNext())
		{
			object current = weaponPrefabsCoroutine.Current;
		}
	}

	private static string GetWeaponPathByName(string weaponName)
	{
		if (string.IsNullOrEmpty(weaponName))
		{
			return "Weapons/";
		}
		_sharedStringBuilder.Length = 0;
		_sharedStringBuilder.Append("Weapons/").Append(weaponName);
		string result = _sharedStringBuilder.ToString();
		_sharedStringBuilder.Length = 0;
		return result;
	}

	private IEnumerator GetWeaponPrefabsCoroutine(int filterMap = 0)
	{
		_lockGetWeaponPrefabs++;
		int counter = 0;
		if (outerWeaponPrefabs == null)
		{
			List<string> weaponNames = (from rec in ItemDb.allRecords
				where !rec.Deactivated
				select rec.PrefabName).ToList();
			int weaponNameCount = weaponNames.Count;
			outerWeaponPrefabs = new List<WeaponSounds>(weaponNameCount);
			for (int weaponIndex = 0; weaponIndex < weaponNameCount; weaponIndex++)
			{
				string wn = weaponNames[weaponIndex];
				if (wn.IsNullOrEmpty())
				{
					continue;
				}
				string weaponPath = GetWeaponPathByName(wn);
				WeaponSounds weaponPrefab = Resources.Load<WeaponSounds>(weaponPath);
				if (weaponPrefab == null)
				{
					Debug.LogError("No weapon " + wn);
					continue;
				}
				outerWeaponPrefabs.Add(weaponPrefab);
				counter++;
				if (counter % 10 == 0)
				{
					yield return null;
				}
			}
		}
		bool isMulti = Defs.isMulti;
		bool isHungry = isMulti && Defs.isHunger;
		List<UnityEngine.Object> wInG = new List<UnityEngine.Object>(outerWeaponPrefabs.Count);
		foreach (WeaponSounds ws in outerWeaponPrefabs)
		{
			if (!ws.IsAvalibleFromFilter(filterMap))
			{
				continue;
			}
			if (isMulti)
			{
				if (!isHungry)
				{
					if (!ws.campaignOnly)
					{
						wInG.Add(ws.gameObject);
					}
					continue;
				}
				int num = int.Parse(ws.gameObject.name.Substring("Weapon".Length));
				if (num == 9 || ChestController.weaponForHungerGames.Contains(num))
				{
					wInG.Add(ws.gameObject);
				}
			}
			else
			{
				wInG.Add(ws.gameObject);
			}
		}
		_weaponsInGame = wInG.ToArray();
		_lockGetWeaponPrefabs--;
	}

	private bool _WeaponAvailable(GameObject prefab, List<string> weaponsGotInCampaign, int filterMap)
	{
		string text = ItemDb.GetByPrefabName(prefab.name.Replace("(Clone)", string.Empty)).Tag;
		bool isMulti = Defs.isMulti;
		bool isHunger = Defs.isHunger;
		bool flag = !Defs.IsSurvival && TrainingController.TrainingCompleted && !isMulti;
		if (isMulti && filterMap == 3 && prefab.name.Equals(DaterFreeWeaponPrefabName))
		{
			return true;
		}
		if (prefab.name.Replace("(Clone)", string.Empty) == KnifeWN)
		{
			return true;
		}
		if (prefab.name.Replace("(Clone)", string.Empty) == PistolWN && !isHunger)
		{
			return true;
		}
		if (prefab.name.Equals(ShotgunWN) && !isHunger)
		{
			return true;
		}
		if (prefab.name.Equals(MP5WN) && (isMulti || Defs.IsSurvival) && !isHunger)
		{
			return true;
		}
		if (prefab.name.Equals(CampaignRifle_WN) && (isMulti || Defs.IsSurvival) && !isHunger)
		{
			return true;
		}
		if (prefab.name.Equals(SimpleFlamethrower_WN) && (isMulti || Defs.IsSurvival) && !isHunger)
		{
			return true;
		}
		if (prefab.name.Equals(Rocketnitza_WN) && (isMulti || Defs.IsSurvival) && !isHunger)
		{
			return true;
		}
		WeaponSounds component = prefab.GetComponent<WeaponSounds>();
		if (!isHunger && text != null && TempItemsController.sharedController.ContainsItem(text) && (filterMap == 0 || (component.filterMap != null && component.filterMap.Contains(filterMap))))
		{
			return true;
		}
		if (flag && LevelBox.weaponsFromBosses.ContainsValue(prefab.name) && weaponsGotInCampaign.Contains(prefab.name))
		{
			return true;
		}
		bool flag2 = prefab.name.Equals(BugGunWN) && weaponsGotInCampaign.Contains(BugGunWN);
		if (Defs.IsSurvival && TrainingController.TrainingCompleted && !isMulti && flag2)
		{
			return true;
		}
		if (!Defs.IsSurvival && TrainingController.TrainingCompleted && isMulti && !isHunger && flag2)
		{
			return true;
		}
		bool flag3 = (prefab.name.Equals(SocialGunWN) && Storager.getInt(Defs.IsFacebookLoginRewardaGained, true) > 0) || (text != null && GotchaGuns.Contains(text) && Storager.getInt(text, true) > 0);
		if (((Defs.IsSurvival && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None) && !isMulti) || (!Defs.IsSurvival && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None) && isMulti && !isHunger) || (flag && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None))) && flag3)
		{
			return true;
		}
		return false;
	}

	public static float ShotgunShotsCountModif()
	{
		return 2f / 3f;
	}

	private void _SortShopLists()
	{
		for (int i = 0; i < 6; i++)
		{
			Dictionary<string, List<GameObject>> dictionary = new Dictionary<string, List<GameObject>>();
			foreach (GameObject item in _weaponsByCat[i])
			{
				string key = WeaponUpgrades.TagOfFirstUpgrade(ItemDb.GetByPrefabName(item.name.Replace("(Clone)", string.Empty)).Tag);
				if (dictionary.ContainsKey(key))
				{
					dictionary[key].Add(item);
					continue;
				}
				dictionary.Add(key, new List<GameObject> { item });
			}
			List<List<GameObject>> list = dictionary.Values.ToList();
			foreach (List<GameObject> item2 in list)
			{
				if (item2.Count > 1)
				{
					item2.Sort(dpsComparer);
				}
			}
			List<List<GameObject>> list2 = new List<List<GameObject>>();
			List<List<GameObject>> list3 = new List<List<GameObject>>();
			foreach (List<GameObject> item3 in list)
			{
				string text = WeaponUpgrades.TagOfFirstUpgrade(ItemDb.GetByPrefabName(item3[0].name.Replace("(Clone)", string.Empty)).Tag);
				((!ItemDb.IsCanBuy(text)) ? list3 : list2).Add(item3);
			}
			Comparison<List<GameObject>> comparison = delegate(List<GameObject> leftList, List<GameObject> rightList)
			{
				if (leftList == null || rightList == null || leftList.Count < 1 || rightList.Count < 1)
				{
					return 0;
				}
				WeaponSounds component = leftList[0].GetComponent<WeaponSounds>();
				WeaponSounds component2 = rightList[0].GetComponent<WeaponSounds>();
				return dpsComparerWS(component, component2);
			};
			list2.Sort(comparison);
			list3.Sort(comparison);
			List<GameObject> list4 = new List<GameObject>();
			foreach (List<GameObject> item4 in list3)
			{
				list4.AddRange(item4);
			}
			foreach (List<GameObject> item5 in list2)
			{
				list4.AddRange(item5);
			}
			_weaponsByCat[i] = list4;
		}
	}

	private static void InitializeRemoved150615Weapons()
	{
		List<string> list = new List<string>();
		list.Add("78tb789to2345f78t2345782345fv789tbf345");
		_Removed150615_GunsPrefabNAmes = list;
		_Removed150615_Guns = new List<string>(_Removed150615_GunsPrefabNAmes.Count);
		foreach (string removed150615_GunsPrefabNAme in _Removed150615_GunsPrefabNAmes)
		{
			ItemRecord byPrefabName = ItemDb.GetByPrefabName(removed150615_GunsPrefabNAme);
			if (byPrefabName != null && byPrefabName.Tag != null)
			{
				_Removed150615_Guns.Add(byPrefabName.Tag);
			}
		}
	}

	private void _AddWeaponToShopListsIfNeeded(GameObject w)
	{
		WeaponSounds component = w.GetComponent<WeaponSounds>();
		bool flag = false;
		bool flag2 = false;
		List<string> list = null;
		string wtag = "Undefined";
		try
		{
			wtag = ItemDb.GetByPrefabName(w.name.Replace("(Clone)", string.Empty)).Tag;
		}
		catch (UnityException exception)
		{
			Debug.LogError("Tag issue encountered.");
			Debug.LogException(exception);
		}
		foreach (List<string> upgrade in WeaponUpgrades.upgrades)
		{
			if (upgrade.Contains(wtag))
			{
				flag2 = true;
				list = upgrade;
				break;
			}
		}
		if (flag2)
		{
			int num = list.IndexOf(wtag);
			if (Storager.getInt(storeIDtoDefsSNMapping[tagToStoreIDMapping[wtag]], true) == 1)
			{
				if (num == list.Count - 1)
				{
					flag = true;
				}
				else if (num < list.Count - 1 && Storager.getInt(storeIDtoDefsSNMapping[tagToStoreIDMapping[list[num + 1]]], true) == 0)
				{
					flag = true;
				}
			}
			else
			{
				string text = FirstTagForOurTier(wtag);
				if (((num > 0 && ((text != null && text.Equals(wtag)) || Storager.getInt(storeIDtoDefsSNMapping[tagToStoreIDMapping[list[num - 1]]], true) == 1) && component.tier < 100) || (num == 0 && text != null && text.Equals(wtag) && ExpController.Instance != null && ExpController.Instance.OurTier >= component.tier)) && (!Removed150615_Guns.Contains(WeaponUpgrades.TagOfFirstUpgrade(wtag)) || LastBoughtTag(wtag) != null))
				{
					flag = true;
				}
			}
		}
		else
		{
			Lazy<string> lazy = new Lazy<string>(delegate
			{
				string value;
				if (!tagToStoreIDMapping.TryGetValue(wtag, out value))
				{
					Debug.LogError("Weapon tag not found in tagToStoreIDMapping: " + wtag);
					return string.Empty;
				}
				string value2;
				if (!storeIDtoDefsSNMapping.TryGetValue(value, out value2))
				{
					Debug.LogError("Weapon name not found in storeIDtoDefsSNMapping: " + value2);
					return string.Empty;
				}
				return value2;
			});
			flag = (TrainingController.TrainingCompleted || (!(wtag == WeaponTags.BASIC_FLAMETHROWER_Tag) && !(wtag == WeaponTags.SignalPistol_Tag))) && ((ExpController.Instance != null && ExpController.Instance.OurTier >= component.tier) || Storager.getInt(lazy.Value, true) == 1) && (!Removed150615_Guns.Contains(WeaponUpgrades.TagOfFirstUpgrade(wtag)) || LastBoughtTag(wtag) != null);
		}
		if (!flag)
		{
			return;
		}
		try
		{
			_weaponsByCat[component.categoryNabor - 1].Add(w);
		}
		catch (Exception ex)
		{
			if (Application.isEditor || Debug.isDebugBuild)
			{
				Debug.LogError("WeaponManager: exception: " + ex);
			}
		}
	}

	private void AddTempGunsToShopCategoryLists(int filterMap, bool isHungry)
	{
		if (isHungry || (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage <= TrainingController.NewTrainingCompletedStage.None))
		{
			return;
		}
		try
		{
			IEnumerable<WeaponSounds> enumerable = from o in weaponsInGame.OfType<GameObject>()
				select o.GetComponent<WeaponSounds>() into ws
				where ItemDb.IsTemporaryGun(ItemDb.GetByPrefabName(ws.name.Replace("(Clone)", string.Empty)).Tag) && TempItemsController.sharedController != null && TempItemsController.sharedController.ContainsItem(ItemDb.GetByPrefabName(ws.name.Replace("(Clone)", string.Empty)).Tag)
				select ws;
			if (filterMap != 0)
			{
				enumerable = enumerable.Where((WeaponSounds ws) => ws.filterMap != null && ws.filterMap.Contains(filterMap));
			}
			foreach (WeaponSounds item in enumerable)
			{
				_weaponsByCat[item.categoryNabor - 1].Add(item.gameObject);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Exception " + ex);
		}
	}

	private void _InitShopCategoryLists(int filterMap = 0)
	{
		bool isMulti = Defs.isMulti;
		bool flag = isMulti && Defs.isHunger;
		bool flag2 = !Defs.IsSurvival && TrainingController.TrainingCompleted && !isMulti;
		string[] array = Storager.getString(Defs.WeaponsGotInCampaign, false).Split('#');
		List<string> list = new List<string>();
		string[] array2 = array;
		foreach (string item in array2)
		{
			list.Add(item);
		}
		foreach (List<GameObject> item2 in _weaponsByCat)
		{
			item2.Clear();
		}
		AddTempGunsToShopCategoryLists(filterMap, flag);
		if ((isMulti && !flag) || (Defs.IsSurvival && TrainingController.TrainingCompleted))
		{
			UnityEngine.Object[] array3 = weaponsInGame;
			for (int j = 0; j < array3.Length; j++)
			{
				GameObject gameObject = (GameObject)array3[j];
				string text = ItemDb.GetByPrefabName(gameObject.name).Tag;
				WeaponSounds component = gameObject.GetComponent<WeaponSounds>();
				if (gameObject.name == DaterFreeWeaponPrefabName)
				{
					if (filterMap == 3)
					{
						_weaponsByCat[component.categoryNabor - 1].Add(gameObject);
					}
				}
				else
				{
					if (component.campaignOnly)
					{
						continue;
					}
					if (gameObject.name.Equals(AlienGunWN))
					{
						if (!list.Contains(AlienGunWN))
						{
						}
					}
					else if (gameObject.name.Equals(BugGunWN))
					{
						if (list.Contains(BugGunWN))
						{
							_weaponsByCat[component.categoryNabor - 1].Add(gameObject);
						}
					}
					else if (gameObject.name.Equals(SocialGunWN))
					{
						if (Storager.getInt(Defs.IsFacebookLoginRewardaGained, true) > 0)
						{
							_weaponsByCat[component.categoryNabor - 1].Add(gameObject);
						}
					}
					else if (text != null && GotchaGuns.Contains(text))
					{
						if (Storager.getInt(text, true) > 0)
						{
							_weaponsByCat[component.categoryNabor - 1].Add(gameObject);
						}
					}
					else if (!ItemDb.IsTemporaryGun(text))
					{
						_AddWeaponToShopListsIfNeeded(gameObject);
					}
				}
			}
			_SortShopLists();
		}
		else if (flag2)
		{
			UnityEngine.Object[] array4 = weaponsInGame;
			for (int k = 0; k < array4.Length; k++)
			{
				GameObject gameObject2 = (GameObject)array4[k];
				string text2 = ItemDb.GetByPrefabName(gameObject2.name).Tag;
				WeaponSounds component2 = gameObject2.GetComponent<WeaponSounds>();
				if (gameObject2.name == DaterFreeWeaponPrefabName)
				{
					continue;
				}
				if (component2.campaignOnly || gameObject2.name.Equals(BugGunWN) || gameObject2.name.Equals(AlienGunWN) || gameObject2.name.Equals(MP5WN) || gameObject2.name.Equals(CampaignRifle_WN) || gameObject2.name.Equals(SimpleFlamethrower_WN) || gameObject2.name.Equals(Rocketnitza_WN))
				{
					if (list.Contains(gameObject2.name))
					{
						_weaponsByCat[component2.categoryNabor - 1].Add(gameObject2);
					}
				}
				else if (gameObject2.name.Equals(SocialGunWN))
				{
					if (Storager.getInt(Defs.IsFacebookLoginRewardaGained, true) > 0)
					{
						_weaponsByCat[component2.categoryNabor - 1].Add(gameObject2);
					}
				}
				else if (text2 != null && GotchaGuns.Contains(text2))
				{
					if (Storager.getInt(text2, true) > 0)
					{
						_weaponsByCat[component2.categoryNabor - 1].Add(gameObject2);
					}
				}
				else if (!ItemDb.IsTemporaryGun(text2))
				{
					_AddWeaponToShopListsIfNeeded(gameObject2);
				}
			}
			_SortShopLists();
		}
		else
		{
			if (!flag)
			{
				return;
			}
			UnityEngine.Object[] array5 = weaponsInGame;
			for (int l = 0; l < array5.Length; l++)
			{
				GameObject gameObject3 = (GameObject)array5[l];
				if (gameObject3.name.Equals(KnifeWN))
				{
					_AddWeaponToShopListsIfNeeded(gameObject3);
					break;
				}
			}
			_SortShopLists();
		}
	}

	private static bool OldChainThatAlwaysShownFromStart(string tg)
	{
		string value = WeaponUpgrades.TagOfFirstUpgrade(tg);
		return oldTags.Contains(value);
	}

	private static void SaveFirstTagsToDisc()
	{
		Storager.setString("FirstTagsForOurTier", Rilisoft.MiniJson.Json.Serialize(firstTagsWithRespecToOurTier), false);
	}

	private void FixFirstTags()
	{
		try
		{
			bool flag = false;
			foreach (List<string> upgrade in WeaponUpgrades.upgrades)
			{
				if (upgrade != null && upgrade.Count == 0)
				{
					continue;
				}
				string text = upgrade.First();
				ItemRecord byTag = ItemDb.GetByTag(text);
				if (byTag == null || byTag.StorageId == null || Storager.getInt(byTag.StorageId, true) == 0)
				{
					continue;
				}
				string text2 = LastBoughtTag(text, upgrade);
				if (text2 != null)
				{
					string item = FirstTagForOurTier(text, upgrade);
					int num = upgrade.IndexOf(text2);
					int num2 = upgrade.IndexOf(item);
					if (num < num2)
					{
						flag = true;
						firstTagsWithRespecToOurTier[text] = text2;
					}
				}
			}
			if (flag)
			{
				SaveFirstTagsToDisc();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in FixFirstTags: " + ex);
		}
	}

	private static void InitFirstTagsData()
	{
		//Discarded unreachable code: IL_0098
		if (!Storager.hasKey("FirstTagsForOurTier"))
		{
			Storager.setString("FirstTagsForOurTier", "{}", false);
		}
		string @string = Storager.getString("FirstTagsForOurTier", false);
		try
		{
			Dictionary<string, object> dictionary = Rilisoft.MiniJson.Json.Deserialize(@string) as Dictionary<string, object>;
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				firstTagsWithRespecToOurTier.Add(item.Key, (string)item.Value);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return;
		}
		bool flag = false;
		int ourTier = ExpController.GetOurTier();
		foreach (List<string> upgrade in WeaponUpgrades.upgrades)
		{
			if (upgrade.Count == 0 || firstTagsWithRespecToOurTier.ContainsKey(upgrade[0]))
			{
				continue;
			}
			flag = true;
			if (OldChainThatAlwaysShownFromStart(upgrade[0]))
			{
				firstTagsWithRespecToOurTier.Add(upgrade[0], upgrade[0]);
				continue;
			}
			List<WeaponSounds> list = upgrade.Select((string tg) => ItemDb.GetWeaponInfo(tg)).ToList();
			bool flag2 = false;
			for (int i = 0; i < upgrade.Count; i++)
			{
				if (list[i] != null && list[i].tier > ourTier)
				{
					firstTagsWithRespecToOurTier.Add(upgrade[0], upgrade[Math.Max(0, i - 1)]);
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				firstTagsWithRespecToOurTier.Add(upgrade[0], upgrade[upgrade.Count - 1]);
			}
		}
		if (flag)
		{
			SaveFirstTagsToDisc();
		}
	}

	public static string FirstTagForOurTier(string tg, List<string> upgradesHint = null)
	{
		if (tg == null)
		{
			return null;
		}
		if (!firstTagsForTiersInitialized)
		{
			InitFirstTagsData();
			firstTagsForTiersInitialized = true;
		}
		List<string> list = upgradesHint ?? WeaponUpgrades.ChainForTag(tg);
		if (list != null && list.Count > 0)
		{
			string value = null;
			firstTagsWithRespecToOurTier.TryGetValue(list[0], out value);
			return value;
		}
		return null;
	}

	private void _UpdateShopCategList(Weapon w)
	{
		WeaponSounds component = w.weaponPrefab.GetComponent<WeaponSounds>();
		string text = ItemDb.GetByPrefabName(w.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag;
		if (tagToStoreIDMapping.ContainsKey(text))
		{
			bool flag = false;
			List<string> list = null;
			foreach (List<string> upgrade in WeaponUpgrades.upgrades)
			{
				if (upgrade.Contains(text))
				{
					list = upgrade;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				int num = list.IndexOf(ItemDb.GetByPrefabName(w.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag);
				int num2 = -1;
				foreach (GameObject item2 in _weaponsByCat[component.categoryNabor - 1])
				{
					if (item2.name.Replace("(Clone)", string.Empty) == w.weaponPrefab.name.Replace("(Clone)", string.Empty))
					{
						num2 = _weaponsByCat[component.categoryNabor - 1].IndexOf(item2);
						break;
					}
				}
				if (num < list.Count - 1)
				{
					GameObject item = null;
					try
					{
						string path = string.Format("Weapons/{0}", ItemDb.GetByTag(list[num + 1]).PrefabName);
						item = Resources.Load<GameObject>(path);
					}
					catch (Exception ex)
					{
						Debug.LogErrorFormat("Exception in setting newW in _UpdateShopCategList: {0}", ex);
					}
					if (num2 != -1)
					{
						string text2 = ItemDb.GetByPrefabName(w.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag;
						string text3 = FirstTagForOurTier(text2);
						if (num > 0 && (text3 == null || !text3.Equals(text2)))
						{
							_weaponsByCat[component.categoryNabor - 1].RemoveAt(num2 - 1);
						}
						_weaponsByCat[component.categoryNabor - 1].Insert(num2, item);
					}
					else
					{
						Debug.LogWarning("_UpdateShopCategList: prevInd = -1   ws.categoryNabor - 1: " + (component.categoryNabor - 1));
					}
				}
				else
				{
					string text4 = ItemDb.GetByPrefabName(w.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag;
					string text5 = FirstTagForOurTier(text4);
					if (text5 == null || !text5.Equals(text4))
					{
						_weaponsByCat[component.categoryNabor - 1].RemoveAt(num2 - 1);
					}
				}
			}
		}
		else
		{
			_weaponsByCat[component.categoryNabor - 1].Add(w.weaponPrefab);
		}
		_SortShopLists();
	}

	public void Reset(int filterMap = 0)
	{
		IEnumerator enumerator = ResetCoroutine(filterMap);
		while (enumerator.MoveNext())
		{
			object current = enumerator.Current;
		}
	}

	private static List<string> AllWeaponSetsSettingNames()
	{
		List<string> list = new List<string>();
		list.Add(Defs.CampaignWSSN);
		return list.Concat(WeaponSetSettingNamesForFilterMaps.Values.Select((FilterMapSettings fms) => fms.settingName)).ToList();
	}

	private void Update()
	{
	}

	public bool ActualizeEquippedWeapons()
	{
		bool result = false;
		List<string> list = AllWeaponSetsSettingNames();
		foreach (string item in list)
		{
			string text = LoadWeaponSet(item);
			string[] array = text.Split('#');
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				if (string.IsNullOrEmpty(text2))
				{
					continue;
				}
				ItemRecord byPrefabName = ItemDb.GetByPrefabName(text2);
				if (byPrefabName == null || byPrefabName.Tag == null || !byPrefabName.CanBuy)
				{
					continue;
				}
				string text3 = LastBoughtTag(byPrefabName.Tag);
				if (text3 != null && !(text3 == byPrefabName.Tag))
				{
					ItemRecord byTag = ItemDb.GetByTag(text3);
					if (byTag != null && byTag.PrefabName != null)
					{
						SaveWeaponSet(item, byTag.PrefabName, i);
						result = true;
					}
				}
			}
		}
		return result;
	}

	private bool ReequipItemsAfterCloudSync()
	{
		bool flag = sharedManager != null && sharedManager.myPlayerMoveC != null;
		List<ShopNGUIController.CategoryNames> list = new List<ShopNGUIController.CategoryNames>();
		ShopNGUIController.CategoryNames[] array = new ShopNGUIController.CategoryNames[5]
		{
			ShopNGUIController.CategoryNames.ArmorCategory,
			ShopNGUIController.CategoryNames.BootsCategory,
			ShopNGUIController.CategoryNames.CapesCategory,
			ShopNGUIController.CategoryNames.HatsCategory,
			ShopNGUIController.CategoryNames.MaskCategory
		};
		foreach (ShopNGUIController.CategoryNames categoryNames in array)
		{
			string text = ShopNGUIController.NoneEquippedForWearCategory(categoryNames);
			string @string = Storager.getString(ShopNGUIController.SnForWearCategory(categoryNames), false);
			if (@string != null && text != null && !@string.Equals(text) && @string != "Armor_Novice")
			{
				string text2 = LastBoughtTag(@string);
				if (!string.IsNullOrEmpty(text2) && text2 != @string)
				{
					ShopNGUIController.EquipWearInCategoryIfNotEquiped(text2, categoryNames, flag);
					list.Add(categoryNames);
				}
			}
		}
		GadgetsInfo.ActualizeEquippedGadgets();
		bool result = ActualizeEquippedWeapons();
		if (flag)
		{
			if (myPlayerMoveC.mySkinName != null)
			{
				myPlayerMoveC.mySkinName.SetWearVisible(null);
				if (list.Contains(ShopNGUIController.CategoryNames.ArmorCategory))
				{
					myPlayerMoveC.mySkinName.SetArmor(null);
				}
				if (list.Contains(ShopNGUIController.CategoryNames.BootsCategory))
				{
					myPlayerMoveC.mySkinName.SetBoots(null);
				}
				if (list.Contains(ShopNGUIController.CategoryNames.CapesCategory))
				{
					myPlayerMoveC.mySkinName.SetCape(null);
				}
				if (list.Contains(ShopNGUIController.CategoryNames.HatsCategory))
				{
					myPlayerMoveC.mySkinName.SetHat(null);
				}
				if (list.Contains(ShopNGUIController.CategoryNames.MaskCategory))
				{
					myPlayerMoveC.mySkinName.SetMask(null);
				}
			}
		}
		else if (PersConfigurator.currentConfigurator != null && list.Count > 0)
		{
			PersConfigurator.currentConfigurator.UpdateWear();
		}
		return result;
	}

	private void ReequipWeaponsForGuiAndRpcAndUpdateIcons()
	{
		if (!(myPlayerMoveC != null) || !(ShopNGUIController.sharedShop != null) || ShopNGUIController.sharedShop.equipAction == null)
		{
			return;
		}
		foreach (Weapon playerWeapon in playerWeapons)
		{
			if (playerWeapon != null && playerWeapon.weaponPrefab != null)
			{
				ShopNGUIController.sharedShop.equipAction(ItemDb.GetByPrefabName(playerWeapon.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag);
			}
			if (ShopNGUIController.GuiActive)
			{
				ShopNGUIController.sharedShop.UpdateIcons(false);
			}
		}
	}

	private Weapon AddWeaponWithTagToAllAvailable(string tagToAdd)
	{
		//Discarded unreachable code: IL_0059, IL_0076
		try
		{
			WeaponSounds weaponSounds = Resources.Load<WeaponSounds>("Weapons/" + ItemDb.GetByTag(tagToAdd).PrefabName);
			Weapon weapon = new Weapon();
			weapon.weaponPrefab = weaponSounds.gameObject;
			weapon.currentAmmoInBackpack = weaponSounds.InitialAmmoWithEffectsApplied;
			weapon.currentAmmoInClip = weaponSounds.ammoInClip;
			allAvailablePlayerWeapons.Add(weapon);
			return weapon;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in AddWeaponWithTagToAllAvailable: " + ex);
			return null;
		}
	}

	private static void GivePreviousUpgradesOfCrystalSword()
	{
		try
		{
			string storageId = ItemDb.GetByTag(WeaponTags.CrystalSwordTag).StorageId;
			if (Storager.getInt(storageId, true) == 1)
			{
				Storager.setInt(ItemDb.GetByTag(WeaponTags.IronSwordTag).StorageId, 1, true);
				Storager.setInt(ItemDb.GetByTag(WeaponTags.GoldenSwordTag).StorageId, 1, true);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in giving previous upgrades of crystal sword in Reset: " + ex);
		}
	}

	private IEnumerator DoMigrationsIfNeeded()
	{
		if (!_migrationsCompletedAtThisLaunch)
		{
			if (!Storager.hasKey(Defs.Weapons800to801))
			{
				yield return StartCoroutine(UpdateWeapons800To801());
			}
			if (!Storager.hasKey(Defs.FixWeapons911))
			{
				FixWeaponsDueToCategoriesMoved911();
				yield return null;
			}
			if (!Storager.hasKey(Defs.ReturnAlienGun930))
			{
				ReturnAlienGunToCampaignBack();
				yield return null;
			}
			_migrationsCompletedAtThisLaunch = true;
		}
	}

	public IEnumerator ResetCoroutine(int filterMap = 0, bool doIosCloudSync = true)
	{
		if (_resetLock)
		{
			Debug.LogWarning("Simultaneous executing of WeaponManagers ResetCoroutines");
		}
		_resetLock = true;
		using (new ActionDisposable(delegate
		{
			_resetLock = false;
		}))
		{
			bool newWeaponsComeFromCloudInWeaponSet = false;
			if (doIosCloudSync)
			{
				List<string> weaponsForWhichSetRememberedTier = new List<string>();
				bool armorArmy1Comes;
				Storager.SynchronizeIosWithCloud(ref weaponsForWhichSetRememberedTier, out armorArmy1Comes);
				GivePreviousUpgradesOfCrystalSword();
				int levelBefore = ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
				RefreshExpControllers();
				ExperienceController.SendAnalyticsForLevelsFromCloud(levelBefore);
				RefreshLevelAndSetRememberedTiersFromCloud(weaponsForWhichSetRememberedTier);
				if ((ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel > 1) || armorArmy1Comes)
				{
					if (!TrainingController.TrainingCompleted)
					{
						TrainingController.OnGetProgress();
					}
					else if (Storager.getInt("Training.ShouldRemoveNoviceArmorInShopKey", false) == 1 && armorArmy1Comes)
					{
						if (ShopNGUIController.NoviceArmorAvailable)
						{
							ShopNGUIController.UnequipCurrentWearInCategory(ShopNGUIController.CategoryNames.ArmorCategory, false);
							ShopNGUIController.ProvideItem(ShopNGUIController.CategoryNames.ArmorCategory, "Armor_Army_1", 1, false, 0, null, null, true, false, false);
						}
						Storager.setInt("Training.ShouldRemoveNoviceArmorInShopKey", 0, false);
					}
				}
				newWeaponsComeFromCloudInWeaponSet = ReequipItemsAfterCloudSync();
			}
			FixFirstTags();
			GadgetsInfo.FixFirstsForOurTier();
			_currentFilterMap = filterMap;
			bool isMulti = Defs.isMulti;
			bool isHungry = Defs.isHunger;
			if (!isHungry)
			{
				if (!_initialized)
				{
					yield return StartCoroutine(GetWeaponPrefabsCoroutine(filterMap));
				}
				else
				{
					GetWeaponPrefabs(filterMap);
				}
				yield return null;
			}
			yield return null;
			yield return StartCoroutine(DoMigrationsIfNeeded());
			allAvailablePlayerWeapons.Clear();
			CurrentWeaponIndex = 0;
			string[] _arr = Storager.getString(Defs.WeaponsGotInCampaign, false).Split('#');
			List<string> weaponsGotInCampaign = new List<string>();
			string[] array = _arr;
			foreach (string s in array)
			{
				weaponsGotInCampaign.Add(s);
			}
			UnityEngine.Object[] array2 = weaponsInGame;
			for (int k = 0; k < array2.Length; k++)
			{
				GameObject prefab = (GameObject)array2[k];
				if (_WeaponAvailable(prefab, weaponsGotInCampaign, filterMap))
				{
					Weapon pistol = new Weapon();
					pistol.weaponPrefab = prefab;
					pistol.currentAmmoInBackpack = pistol.weaponPrefab.GetComponent<WeaponSounds>().InitialAmmoWithEffectsApplied;
					pistol.currentAmmoInClip = pistol.weaponPrefab.GetComponent<WeaponSounds>().ammoInClip;
					allAvailablePlayerWeapons.Add(pistol);
				}
			}
			yield return null;
			if ((isMulti && isHungry) || (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None))
			{
				SetWeaponsSet(filterMap);
				_InitShopCategoryLists(filterMap);
				UpdateFilteredShopLists();
				CurrentWeaponIndex = 0;
				if (newWeaponsComeFromCloudInWeaponSet)
				{
					ReequipWeaponsForGuiAndRpcAndUpdateIcons();
				}
				yield break;
			}
			HashSet<string> addedWeaponTags = new HashSet<string>();
			Func<string, bool> weaponWithTagIsBought = delegate(string tg)
			{
				ItemRecord byTag = ItemDb.GetByTag(tg);
				if (byTag != null)
				{
					if (byTag.TemporaryGun)
					{
						return false;
					}
					if (byTag.StorageId != null)
					{
						return Storager.getInt(byTag.StorageId, true) > 0;
					}
					Debug.LogError("lastBoughtUpgrade: StorageId returns null for tag " + tg);
				}
				else
				{
					Debug.LogError("lastBoughtUpgrade: GetByTag returns null for tag " + tg);
				}
				return false;
			};
			try
			{
				List<List<string>> allUpgrades = WeaponUpgrades.upgrades;
				foreach (List<string> weaponUpgrades in allUpgrades)
				{
					addedWeaponTags.UnionWith(weaponUpgrades);
					string lastBoughtUpgrade = weaponUpgrades.FindLast((string tg) => weaponWithTagIsBought(tg));
					if (lastBoughtUpgrade == null && weaponUpgrades.Count > 0 && IsAvailableTryGun(weaponUpgrades[0]))
					{
						lastBoughtUpgrade = weaponUpgrades[0];
					}
					if (lastBoughtUpgrade != null)
					{
						AddWeaponWithTagToAllAvailable(lastBoughtUpgrade);
					}
				}
			}
			catch (Exception e2)
			{
				Debug.LogError("lastBoughtUpgrade: Exception " + e2);
			}
			yield return null;
			try
			{
				List<string> canBuyWeaponTags = ItemDb.GetCanBuyWeaponTags(false).Except(addedWeaponTags).ToList();
				for (int i = 0; i < canBuyWeaponTags.Count; i++)
				{
					if (weaponWithTagIsBought(canBuyWeaponTags[i]) || IsAvailableTryGun(canBuyWeaponTags[i]))
					{
						AddWeaponWithTagToAllAvailable(canBuyWeaponTags[i]);
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError("lastBoughtUpgrade: Exception " + e);
			}
			yield return null;
			SetWeaponsSet(filterMap);
			_InitShopCategoryLists(filterMap);
			UpdateFilteredShopLists();
			CurrentWeaponIndex = 0;
			if (newWeaponsComeFromCloudInWeaponSet)
			{
				ReequipWeaponsForGuiAndRpcAndUpdateIcons();
			}
		}
	}

	public bool AddWeapon(GameObject weaponPrefab, out int score)
	{
		score = 0;
		WeaponSounds component = weaponPrefab.GetComponent<WeaponSounds>();
		if (component != null && sharedManager != null && !component.IsAvalibleFromFilter(sharedManager.CurrentFilterMap))
		{
			return false;
		}
		int result;
		if (Defs.isHunger && component != null && int.TryParse(component.nameNoClone().Substring("Weapon".Length), out result) && result != 9 && !ChestController.weaponForHungerGames.Contains(result))
		{
			return false;
		}
		bool flag = false;
		foreach (Weapon allAvailablePlayerWeapon in allAvailablePlayerWeapons)
		{
			if (allAvailablePlayerWeapon.weaponPrefab.name.Replace("(Clone)", string.Empty) == weaponPrefab.name.Replace("(Clone)", string.Empty))
			{
				int idx = allAvailablePlayerWeapons.IndexOf(allAvailablePlayerWeapon);
				if (!AddAmmo(idx))
				{
					score += Defs.ScoreForSurplusAmmo;
				}
				if (!ItemDb.IsTemporaryGun(ItemDb.GetByPrefabName(weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag) && !IsAvailableTryGun(ItemDb.GetByPrefabName(weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag))
				{
					return false;
				}
				flag = true;
			}
		}
		Weapon weapon2 = new Weapon();
		weapon2.weaponPrefab = weaponPrefab;
		weapon2.currentAmmoInBackpack = weapon2.weaponPrefab.GetComponent<WeaponSounds>().InitialAmmoWithEffectsApplied;
		weapon2.currentAmmoInClip = weapon2.weaponPrefab.GetComponent<WeaponSounds>().ammoInClip;
		if (!flag)
		{
			allAvailablePlayerWeapons.Add(weapon2);
		}
		else
		{
			int num = -1;
			foreach (Weapon allAvailablePlayerWeapon2 in allAvailablePlayerWeapons)
			{
				if (allAvailablePlayerWeapon2.weaponPrefab.name.Equals(weaponPrefab.name))
				{
					num = allAvailablePlayerWeapons.IndexOf(allAvailablePlayerWeapon2);
					break;
				}
			}
			if (num > -1 && num < allAvailablePlayerWeapons.Count)
			{
				allAvailablePlayerWeapons[num] = weapon2;
			}
		}
		string tg = ItemDb.GetByPrefabName(weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag;
		int num2 = _RemovePrevVersionsOfUpgrade(tg);
		bool flag2 = true;
		List<string> list = new List<string>();
		list.Add(CampaignRifle_WN);
		list.Add(AlienGunWN);
		list.Add(SimpleFlamethrower_WN);
		list.Add(BugGunWN);
		list.Add(Rocketnitza_WN);
		List<string> list2 = list;
		WeaponSounds weaponSettingsOfNewWeapon = weapon2.weaponPrefab.GetComponent<WeaponSounds>();
		if (weaponSettingsOfNewWeapon.campaignOnly || weapon2.weaponPrefab.name.Replace("(Clone)", string.Empty) == MP5WN || list2.Contains(weapon2.weaponPrefab.name.Replace("(Clone)", string.Empty)))
		{
			try
			{
				if (CurrentWeaponIndex >= 0 && CurrentWeaponIndex < playerWeapons.Count)
				{
					Weapon weapon4 = playerWeapons[CurrentWeaponIndex] as Weapon;
					if (weapon4 != null)
					{
						GameObject weaponPrefab2 = weapon4.weaponPrefab;
						ItemRecord byPrefabName = ItemDb.GetByPrefabName(weaponPrefab2.nameNoClone());
						if (byPrefabName != null && (tagToStoreIDMapping.ContainsKey(byPrefabName.Tag) || GotchaGuns.Contains(byPrefabName.Tag) || byPrefabName.Tag == WeaponTags.FriendsUzi_Tag))
						{
							flag2 = false;
						}
					}
				}
				WeaponSounds weaponSounds = (from w in playerWeapons.OfType<Weapon>()
					select w.weaponPrefab.GetComponent<WeaponSounds>()).FirstOrDefault((WeaponSounds ws) => ws.categoryNabor == weaponSettingsOfNewWeapon.categoryNabor);
				if (weaponSounds != null)
				{
					ItemRecord byPrefabName2 = ItemDb.GetByPrefabName(weaponSounds.nameNoClone());
					if (byPrefabName2 != null && (tagToStoreIDMapping.ContainsKey(byPrefabName2.Tag) || GotchaGuns.Contains(byPrefabName2.Tag) || byPrefabName2.Tag == WeaponTags.FriendsUzi_Tag))
					{
						flag2 = false;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in finding weapon of checking notBoughtToCampaign: " + ex);
				flag2 = false;
			}
		}
		if (flag2)
		{
			EquipWeapon(weapon2);
			SaveWeaponAsLastUsed(CurrentWeaponIndex);
		}
		_UpdateShopCategList(weapon2);
		UpdateFilteredShopLists();
		return flag2;
	}

	private int _RemovePrevVersionsOfUpgrade(string tg)
	{
		int num = 0;
		foreach (List<string> upgrade in WeaponUpgrades.upgrades)
		{
			int num2 = upgrade.IndexOf(tg);
			if (num2 == -1)
			{
				continue;
			}
			for (int i = 0; i < num2; i++)
			{
				List<Weapon> list = new List<Weapon>();
				for (int j = 0; j < allAvailablePlayerWeapons.Count; j++)
				{
					Weapon weapon = allAvailablePlayerWeapons[j] as Weapon;
					if (ItemDb.GetByPrefabName(weapon.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag.Equals(upgrade[i]))
					{
						list.Add(weapon);
					}
				}
				for (int k = 0; k < list.Count; k++)
				{
					allAvailablePlayerWeapons.Remove(list[k]);
				}
				num += list.Count;
			}
			return num;
		}
		return num;
	}

	public GameObject GetPrefabByTag(string weaponTag)
	{
		if (weaponTag.IsNullOrEmpty())
		{
			Debug.LogErrorFormat("GetPrefabByTag: weaponTag.IsNullOrEmpty()");
			return null;
		}
		ItemRecord byTag = ItemDb.GetByTag(weaponTag);
		if (byTag == null)
		{
			Debug.LogErrorFormat("GetPrefabByTag: rec == null, weaponTag = {0}", weaponTag);
			return null;
		}
		if (byTag.PrefabName.IsNullOrEmpty())
		{
			Debug.LogErrorFormat("GetPrefabByTag: rec.PrefabName.IsNullOrEmpty(), weaponTag = {0}", weaponTag);
			return null;
		}
		GameObject gameObject = Resources.Load<GameObject>(string.Format("Weapons/{0}", byTag.PrefabName));
		if (gameObject == null)
		{
			Debug.LogErrorFormat("GetPrefabByTag: prefab == null, weaponTag = {0}", weaponTag);
			return null;
		}
		return gameObject;
	}

	public bool AddAmmo(int idx = -1)
	{
		if (idx == -1)
		{
			idx = allAvailablePlayerWeapons.IndexOf(playerWeapons[CurrentWeaponIndex]);
		}
		if (allAvailablePlayerWeapons[idx] == playerWeapons[CurrentWeaponIndex] && currentWeaponSounds.isMelee && !currentWeaponSounds.isShotMelee)
		{
			return false;
		}
		Weapon weapon = (Weapon)allAvailablePlayerWeapons[idx];
		WeaponSounds component = weapon.weaponPrefab.GetComponent<WeaponSounds>();
		if (weapon.currentAmmoInBackpack < component.MaxAmmoWithEffectApplied)
		{
			weapon.currentAmmoInBackpack += ((!(currentWeaponSounds != null) || (!currentWeaponSounds.isShotMelee && !(currentWeaponSounds.name.Replace("(Clone)", string.Empty) == "Weapon335") && !(currentWeaponSounds.name.Replace("(Clone)", string.Empty) == "Weapon353") && !(currentWeaponSounds.name.Replace("(Clone)", string.Empty) == "Weapon354"))) ? component.ammoInClip : component.ammoForBonusShotMelee);
			if (weapon.currentAmmoInBackpack > component.MaxAmmoWithEffectApplied)
			{
				weapon.currentAmmoInBackpack = component.MaxAmmoWithEffectApplied;
			}
			return true;
		}
		return false;
	}

	public bool AddAmmoForAllGuns()
	{
		bool result = false;
		for (int i = 0; i < playerWeapons.Count; i++)
		{
			Weapon weapon = (Weapon)playerWeapons[i];
			WeaponSounds component = weapon.weaponPrefab.GetComponent<WeaponSounds>();
			if ((!component.isMelee || component.isShotMelee) && weapon.currentAmmoInBackpack < component.MaxAmmoWithEffectApplied)
			{
				weapon.currentAmmoInBackpack += ((!(component != null) || (!component.isShotMelee && !(component.name.Replace("(Clone)", string.Empty) == "Weapon335") && !(component.name.Replace("(Clone)", string.Empty) == "Weapon353") && !(component.name.Replace("(Clone)", string.Empty) == "Weapon354"))) ? component.ammoInClip : component.ammoForBonusShotMelee);
				if (weapon.currentAmmoInBackpack > component.MaxAmmoWithEffectApplied)
				{
					weapon.currentAmmoInBackpack = component.MaxAmmoWithEffectApplied;
				}
				result = true;
			}
		}
		return result;
	}

	public void SetMaxAmmoFrAllWeapons()
	{
		foreach (Weapon allAvailablePlayerWeapon in allAvailablePlayerWeapons)
		{
			allAvailablePlayerWeapon.currentAmmoInClip = allAvailablePlayerWeapon.weaponPrefab.GetComponent<WeaponSounds>().ammoInClip;
			allAvailablePlayerWeapon.currentAmmoInBackpack = allAvailablePlayerWeapon.weaponPrefab.GetComponent<WeaponSounds>().MaxAmmoWithEffectApplied;
		}
	}

	private void Awake()
	{
		ScopeLogger scopeLogger = new ScopeLogger("WeaponManager.Awake()", Defs.IsDeveloperBuild);
		try
		{
			if (Storager.getInt("WeaponManager_SniperCategoryAddedToWeaponSetsKey", false) == 0)
			{
				foreach (string wssn in AllWeaponSetsSettingNames())
				{
					try
					{
						if (Storager.hasKey(wssn))
						{
							string weaponSet = Storager.getString(wssn, false);
							if (weaponSet == null)
							{
								Debug.LogError("Adding sniper category to weapon sets error: weaponSet == null  wssn = " + wssn);
							}
							int num = weaponSet.LastIndexOf("#");
							if (num == -1)
							{
								Debug.LogError("Adding sniper category to weapon sets error: lastIndexOfHash == -1  wssn = " + wssn + "  weaponSet = " + weaponSet);
							}
							weaponSet = weaponSet.Insert(num, "#");
							string[] splittedWeaponSet = weaponSet.Split('#');
							if (splittedWeaponSet == null)
							{
								Debug.LogError("Adding sniper category to weapon sets error: splittedWeaponSet == null  wssn = " + wssn + "  weaponSet = " + weaponSet);
							}
							bool flag = true;
							if (splittedWeaponSet.Length > 6)
							{
								Debug.LogError("Adding sniper category to weapon sets error: splittedWeaponSet.Length > NumOfWeaponCategories  wssn = " + wssn + "  weaponSet = " + weaponSet);
								Storager.setString(wssn, DefaultSetForWeaponSetSettingName(wssn), false);
								flag = false;
							}
							if (splittedWeaponSet.Length < 6)
							{
								Debug.LogError("Adding sniper category to weapon sets error: splittedWeaponSet.Length < NumOfWeaponCategories  wssn = " + wssn + "  weaponSet = " + weaponSet);
								Storager.setString(wssn, DefaultSetForWeaponSetSettingName(wssn), false);
								flag = false;
							}
							if (!flag)
							{
								continue;
							}
							for (int i = 0; i < splittedWeaponSet.Length; i++)
							{
								if (splittedWeaponSet[i] == null)
								{
									splittedWeaponSet[i] = string.Empty;
								}
							}
							Dictionary<ShopNGUIController.CategoryNames, string> dictionary = new Dictionary<ShopNGUIController.CategoryNames, string>();
							for (int j = 0; j < splittedWeaponSet.Length; j++)
							{
								if (splittedWeaponSet[j] == null || !weaponsMovedToSniperCategory.Contains(splittedWeaponSet[j]))
								{
									continue;
								}
								dictionary.Add((ShopNGUIController.CategoryNames)j, splittedWeaponSet[j]);
								splittedWeaponSet[j] = string.Empty;
								switch (j)
								{
								case 3:
									if (wssn == Defs.MultiplayerWSSN)
									{
										splittedWeaponSet[j] = SimpleFlamethrower_WN;
									}
									else if (wssn == Defs.CampaignWSSN)
									{
										Dictionary<string, Dictionary<string, int>> boxesLevelsAndStars = CampaignProgress.boxesLevelsAndStars;
										Dictionary<string, int> value = new Dictionary<string, int>();
										bool flag2 = false;
										if (boxesLevelsAndStars.TryGetValue("minecraft", out value) && value.ContainsKey("Maze"))
										{
											splittedWeaponSet[j] = SimpleFlamethrower_WN;
											flag2 = true;
										}
										if (!flag2 && boxesLevelsAndStars.TryGetValue("Real", out value) && value.ContainsKey("Jail"))
										{
											splittedWeaponSet[j] = MachinegunWN;
											flag2 = true;
										}
									}
									break;
								case 0:
									if (wssn == Defs.MultiplayerWSSN)
									{
										splittedWeaponSet[j] = MP5WN;
									}
									else if (wssn == Defs.CampaignWSSN)
									{
										splittedWeaponSet[j] = "Weapon2";
									}
									else if (wssn == Defs.DaterWSSN)
									{
										splittedWeaponSet[j] = string.Empty;
									}
									break;
								}
							}
							int newSniperIndex = 4;
							Action<string> action = delegate(string weaponName)
							{
								if (splittedWeaponSet.Length > newSniperIndex)
								{
									splittedWeaponSet[newSniperIndex] = weaponName;
								}
								else
								{
									Debug.LogError("Adding sniper category to weapon sets error: splittedWeaponSet.Length > newSniperIndex    newSniperIndex: " + newSniperIndex + "   wssn = " + wssn + "   weaponSet = " + weaponSet);
								}
							};
							if (dictionary.Values.Count > 0)
							{
								action(dictionary.Values.FirstOrDefault() ?? string.Empty);
							}
							else if (wssn == Defs.MultiplayerWSSN)
							{
								action(CampaignRifle_WN);
							}
							else if (wssn == Defs.CampaignWSSN)
							{
								Dictionary<string, Dictionary<string, int>> boxesLevelsAndStars2 = CampaignProgress.boxesLevelsAndStars;
								Dictionary<string, int> value2 = new Dictionary<string, int>();
								if (boxesLevelsAndStars2.TryGetValue("minecraft", out value2) && value2.ContainsKey("Utopia"))
								{
									action(CampaignRifle_WN);
								}
							}
							if (splittedWeaponSet[3] == "Weapon317" || splittedWeaponSet[3] == "Weapon318" || splittedWeaponSet[3] == "Weapon319")
							{
								if (wssn == Defs.MultiplayerWSSN)
								{
									splittedWeaponSet[3] = SimpleFlamethrower_WN;
								}
								else if (wssn == Defs.CampaignWSSN)
								{
									Dictionary<string, Dictionary<string, int>> boxesLevelsAndStars3 = CampaignProgress.boxesLevelsAndStars;
									Dictionary<string, int> value3 = new Dictionary<string, int>();
									bool flag3 = false;
									if (boxesLevelsAndStars3.TryGetValue("minecraft", out value3) && value3.ContainsKey("Maze"))
									{
										splittedWeaponSet[3] = SimpleFlamethrower_WN;
										flag3 = true;
									}
									if (!flag3 && boxesLevelsAndStars3.TryGetValue("Real", out value3) && value3.ContainsKey("Jail"))
									{
										splittedWeaponSet[3] = MachinegunWN;
										flag3 = true;
									}
								}
							}
							Storager.setString(wssn, string.Join("#", splittedWeaponSet), false);
							continue;
						}
						Storager.setString(wssn, DefaultSetForWeaponSetSettingName(wssn), false);
					}
					catch (Exception ex)
					{
						Debug.LogError("Exceptio in foreach (var wssn in AllWeaponSetsSettingNames())  wssn = " + wssn + "   exception: " + ex);
						try
						{
							Storager.setString(wssn, DefaultSetForWeaponSetSettingName(wssn), false);
						}
						catch (Exception ex2)
						{
							Debug.LogError("Exceptio in Storager.setString (wssn, DefaultSetForWeaponSetSettingName(wssn),false);  wssn = " + wssn + "   exception: " + ex2);
						}
					}
				}
				Storager.setInt("WeaponManager_SniperCategoryAddedToWeaponSetsKey", 1, false);
			}
			if (!Storager.hasKey("WeaponManager.LastUsedWeaponsKey"))
			{
				SaveLastUsedWeapons();
			}
			else
			{
				try
				{
					Dictionary<string, object> dictionary2 = Rilisoft.MiniJson.Json.Deserialize(Storager.getString("WeaponManager.LastUsedWeaponsKey", false)) as Dictionary<string, object>;
					foreach (string key in dictionary2.Keys)
					{
						lastUsedWeaponsForFilterMaps[key] = (int)(long)dictionary2[key];
					}
				}
				catch (Exception ex3)
				{
					Debug.LogError("Loading last used weapons: " + ex3);
				}
			}
			LoadTryGunsInfo();
			LoadTryGunDiscounts();
		}
		finally
		{
			scopeLogger.Dispose();
		}
		LoadWearInfoPrefabsToCache();
	}

	private void LoadWearInfoPrefabsToCache()
	{
		_wearInfoPrefabsToCache.AddRange(Resources.LoadAll<ShopPositionParams>("Armor_Info"));
		_wearInfoPrefabsToCache.AddRange(Resources.LoadAll<ShopPositionParams>("Capes_Info"));
		_wearInfoPrefabsToCache.AddRange(Resources.LoadAll<ShopPositionParams>("Shop_Boots_Info"));
		_wearInfoPrefabsToCache.AddRange(Resources.LoadAll<ShopPositionParams>("Masks_Info"));
		_wearInfoPrefabsToCache.AddRange(Resources.LoadAll<ShopPositionParams>("Hats_Info"));
	}

	private void SaveLastUsedWeapons()
	{
		Storager.setString("WeaponManager.LastUsedWeaponsKey", Rilisoft.MiniJson.Json.Serialize(lastUsedWeaponsForFilterMaps), false);
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			try
			{
				SaveLastUsedWeapons();
			}
			catch (Exception ex)
			{
				Debug.LogError("Saving last used weapons: " + ex);
			}
			SaveTryGunsInfo();
			SaveTryGunsDiscounts();
		}
		else
		{
			LoadTryGunsInfo();
			LoadTryGunDiscounts();
		}
	}

	private IEnumerator LoadRocketToCache()
	{
		ResourceRequest request = Resources.LoadAsync<GameObject>("Rocket");
		yield return request;
		if (request.isDone)
		{
			_rocketCache = (GameObject)request.asset;
		}
	}

	private IEnumerator LoadTurretToCache()
	{
		ResourceRequest request = Resources.LoadAsync<GameObject>("Turret");
		yield return request;
		if (request.isDone)
		{
			_turretCache = (GameObject)request.asset;
		}
	}

	private IEnumerator Start()
	{
		ScopeLogger scopeLogger = new ScopeLogger("WeaponManager.Start()", Defs.IsDeveloperBuild);
		try
		{
			StartCoroutine(Step());
			yield return null;
			_turretWeaponCache = InnerPrefabForWeaponSync("WeaponTurret");
			StartCoroutine(LoadRocketToCache());
			StartCoroutine(LoadTurretToCache());
			Defs.gameSecondFireButtonMode = (Defs.GameSecondFireButtonMode)PlayerPrefs.GetInt("GameSecondFireButtonMode", 0);
			sharedManager = this;
			for (int j = 0; j < 6; j++)
			{
				_weaponsByCat.Add(new List<GameObject>());
			}
			string[] canBuyWeaponTags = ItemDb.GetCanBuyWeaponTags(true);
			for (int i = 0; i < canBuyWeaponTags.Length; i++)
			{
				string shopId = ItemDb.GetShopIdByTag(canBuyWeaponTags[i]);
				_purchaseActinos.Add(shopId, AddWeaponToInv);
			}
			yield return null;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (!Application.isEditor && Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
			{
				GoogleIABManager.purchaseSucceededEvent += AddWeapon;
			}
			GlobalGameController.SetMultiMode();
			yield return StartCoroutine(ResetCoroutine(0, false));
			_initialized = true;
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}

	public void AddWeaponToInv(string shopId, int timeForRentIndex = 0)
	{
		string tagByShopId = ItemDb.GetTagByShopId(shopId);
		Player_move_c.SaveWeaponInPrefs(tagByShopId, timeForRentIndex);
		GameObject prefabByTag = GetPrefabByTag(tagByShopId);
		if (prefabByTag != null)
		{
			int score;
			AddWeapon(prefabByTag, out score);
		}
	}

	public void AddNewWeapon(string id, int timeForRentIndex = 0)
	{
		if (id == null)
		{
			throw new ArgumentNullException("id");
		}
		if (_purchaseActinos.ContainsKey(id))
		{
			_purchaseActinos[id](id, timeForRentIndex);
		}
	}

	private void AddWeapon(GooglePurchase p)
	{
		try
		{
			AddNewWeapon(p.productId);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	private void OnDestroy()
	{
		if (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
		{
			GoogleIABManager.purchaseSucceededEvent -= AddWeapon;
		}
	}

	public void ReloadWeaponFromSet(int index)
	{
		int num = ((Weapon)playerWeapons[index]).weaponPrefab.GetComponent<WeaponSounds>().ammoInClip - ((Weapon)playerWeapons[index]).currentAmmoInClip;
		if (((Weapon)playerWeapons[index]).currentAmmoInBackpack >= num)
		{
			((Weapon)playerWeapons[index]).currentAmmoInClip += num;
			if (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != 0)
			{
				((Weapon)playerWeapons[index]).currentAmmoInBackpack -= num;
			}
		}
		else
		{
			((Weapon)playerWeapons[index]).currentAmmoInClip += ((Weapon)playerWeapons[index]).currentAmmoInBackpack;
			((Weapon)playerWeapons[index]).currentAmmoInBackpack = 0;
		}
	}

	public void ReloadAmmo()
	{
		ReloadWeaponFromSet(CurrentWeaponIndex);
		if (myPlayerMoveC != null)
		{
			myPlayerMoveC.isReloading = false;
		}
	}

	public void Reload()
	{
		if (!currentWeaponSounds.isShotMelee)
		{
			currentWeaponSounds.animationObject.GetComponent<Animation>().Stop("Empty");
			if (!currentWeaponSounds.isDoubleShot)
			{
				currentWeaponSounds.animationObject.GetComponent<Animation>().CrossFade("Shoot");
			}
			currentWeaponSounds.animationObject.GetComponent<Animation>().CrossFade("Reload");
			currentWeaponSounds.animationObject.GetComponent<Animation>()["Reload"].speed = myPlayerMoveC._currentReloadAnimationSpeed;
		}
	}

	private void ReturnAlienGunToCampaignBack()
	{
		Storager.setInt(Defs.ReturnAlienGun930, 1, false);
		Storager.setString(Defs.MultiplayerWSSN, DefaultSetForWeaponSetSettingName(Defs.MultiplayerWSSN), false);
		Storager.setString(Defs.CampaignWSSN, DefaultSetForWeaponSetSettingName(Defs.CampaignWSSN), false);
	}

	private void FixWeaponsDueToCategoriesMoved911()
	{
		Storager.setInt(Defs.FixWeapons911, 1, false);
		Storager.setString(Defs.MultiplayerWSSN, DefaultSetForWeaponSetSettingName(Defs.MultiplayerWSSN), false);
		Storager.setString(Defs.CampaignWSSN, DefaultSetForWeaponSetSettingName(Defs.CampaignWSSN), false);
	}

	public void RemoveTemporaryItem(string tg)
	{
		if (tg == null)
		{
			return;
		}
		ItemRecord byTag = ItemDb.GetByTag(tg);
		if (byTag == null || byTag.PrefabName == null)
		{
			return;
		}
		string text = LoadWeaponSet(Defs.MultiplayerWSSN);
		string[] array = text.Split('#');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == null)
			{
				array[i] = string.Empty;
			}
		}
		int num = -1;
		foreach (Weapon allAvailablePlayerWeapon in allAvailablePlayerWeapons)
		{
			if (ItemDb.GetByPrefabName(allAvailablePlayerWeapon.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag.Equals(tg))
			{
				num = allAvailablePlayerWeapons.IndexOf(allAvailablePlayerWeapon);
				break;
			}
		}
		if (num != -1)
		{
			allAvailablePlayerWeapons.RemoveAt(num);
		}
		int num2 = Array.IndexOf(array, byTag.PrefabName);
		if (num2 != -1)
		{
			sharedManager.SaveWeaponSet(Defs.MultiplayerWSSN, TopWeaponForCat(num2), num2);
			sharedManager.SaveWeaponSet(Defs.CampaignWSSN, TopWeaponForCat(num2, true), num2);
		}
		SetWeaponsSet(_currentFilterMap);
		_InitShopCategoryLists(_currentFilterMap);
		UpdateFilteredShopLists();
	}

	private string TopWeaponForCat(int ind, bool campaign = false)
	{
		string result = _KnifeAndPistolAndMP5AndSniperAndRocketnitzaSet();
		if (campaign)
		{
			result = _KnifeAndPistolAndShotgunSet();
		}
		List<WeaponSounds> list = new List<WeaponSounds>();
		foreach (Weapon allAvailablePlayerWeapon in allAvailablePlayerWeapons)
		{
			WeaponSounds component = allAvailablePlayerWeapon.weaponPrefab.GetComponent<WeaponSounds>();
			if (component.categoryNabor - 1 == ind)
			{
				ItemRecord byPrefabName = ItemDb.GetByPrefabName(component.name.Replace("(Clone)", string.Empty));
				if (byPrefabName != null && byPrefabName.CanBuy)
				{
					list.Add(component);
				}
			}
		}
		list.Sort(dpsComparerWS);
		if (list.Count > 0)
		{
			result = list[list.Count - 1].gameObject.name;
		}
		return result;
	}

	public static List<string> GetWeaponsForBuy()
	{
		List<string> list = new List<string>();
		string[] canBuyWeaponTags = ItemDb.GetCanBuyWeaponTags(false);
		string[] array = canBuyWeaponTags;
		foreach (string text in array)
		{
			if (tagToStoreIDMapping.ContainsKey(text) && !ItemDb.IsTemporaryGun(text))
			{
				list.Add(text);
			}
		}
		bool filterNextTierUpgrades = true;
		List<string> second = PromoActionsGUIController.FilterPurchases(list, filterNextTierUpgrades, true, false, false);
		return list.Except(second).ToList();
	}

	public static GameObject InnerPrefabForWeaponSync(string weapon)
	{
		return Resources.Load<GameObject>(Defs.InnerWeaponsFolder + "/" + weapon + Defs.InnerWeapons_Suffix);
	}

	public static bool RemoveGunFromAllTryGunRelated(string tg)
	{
		if (tg == null)
		{
			Debug.LogError("RemoveGunFromAllTryGunRelated: tg == null");
			return false;
		}
		bool result = sharedManager.TryGuns.Remove(tg);
		sharedManager.ExpiredTryGuns.RemoveAll((string expiredGunTag) => expiredGunTag == tg);
		sharedManager.RemoveDiscountForTryGun(tg);
		return result;
	}

	public static void ActualizeWeaponsForCampaignProgress()
	{
		try
		{
			string[] array = Storager.getString(Defs.WeaponsGotInCampaign, false).Split('#');
			List<string> list = new List<string>();
			string[] array2 = array;
			foreach (string item in array2)
			{
				list.Add(item);
			}
			foreach (string key in CampaignProgress.boxesLevelsAndStars.Keys)
			{
				foreach (string key2 in CampaignProgress.boxesLevelsAndStars[key].Keys)
				{
					string value;
					if (LevelBox.weaponsFromBosses.TryGetValue(key2, out value) && !list.Contains(value))
					{
						list.Add(value);
					}
				}
			}
			if (list.Contains(ShotgunWN))
			{
				list[list.IndexOf(ShotgunWN)] = UZI_WN;
			}
			string val = string.Join("#", list.ToArray());
			Storager.setString(Defs.WeaponsGotInCampaign, val, false);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ActualizeWeaponsForCampaignProgress: " + ex);
		}
	}

	public static bool AllUpgradesOfWeaponAreBought(string weapnoTag)
	{
		if (weapnoTag == null)
		{
			return false;
		}
		List<string> list = WeaponUpgrades.ChainForTag(weapnoTag);
		if (list == null)
		{
			return LastBoughtTag(weapnoTag) != null;
		}
		return LastBoughtTag(weapnoTag) == list.LastOrDefault();
	}

	private IEnumerator UpdateWeapons800To801()
	{
		Storager.setInt(Defs.Weapons800to801, 1, false);
		Storager.setString(Defs.MultiplayerWSSN, DefaultSetForWeaponSetSettingName(Defs.MultiplayerWSSN), false);
		Storager.setString(Defs.CampaignWSSN, DefaultSetForWeaponSetSettingName(Defs.CampaignWSSN), false);
		if (Storager.getInt(Defs.BarrettSN, true) > 0)
		{
			Storager.setInt(Defs.Barrett2SN, 1, true);
		}
		if (Storager.getInt(Defs.plazma_pistol_SN, true) > 0)
		{
			Storager.setInt(Defs.plazma_pistol_2, 1, true);
		}
		if (Storager.getInt(Defs.StaffSN, true) > 0)
		{
			Storager.setInt(Defs.Staff2SN, 1, true);
		}
		if (Storager.getInt(Defs.MagicBowSett, true) > 0)
		{
			Storager.setInt(Defs.Bow_3, 1, true);
		}
		if (Storager.getInt(Defs.MaceSN, true) > 0)
		{
			Storager.setInt(Defs.Mace2SN, 1, true);
		}
		if (Storager.getInt(Defs.ChainsawS, true) > 0)
		{
			Storager.setInt(Defs.Chainsaw2SN, 1, true);
		}
		if (!_initialized)
		{
			yield return null;
		}
		if (Storager.getInt(Defs.FlowePowerSN, true) > 0)
		{
			Storager.setInt(Defs.flower_3, 1, true);
		}
		if (Storager.getInt(Defs.flower_2, true) > 0)
		{
			Storager.setInt(Defs.flower_3, 1, true);
		}
		if (Storager.getInt(Defs.ScytheSN, true) > 0)
		{
			Storager.setInt(Defs.scythe_3, 1, true);
		}
		if (Storager.getInt(Defs.Scythe_2_SN, true) > 0)
		{
			Storager.setInt(Defs.scythe_3, 1, true);
		}
		if (Storager.getInt(Defs.FlameThrowerSN, true) > 0)
		{
			Storager.setInt(Defs.Flamethrower_3, 1, true);
		}
		if (Storager.getInt(Defs.FlameThrower_2SN, true) > 0)
		{
			Storager.setInt(Defs.Flamethrower_3, 1, true);
		}
		if (!_initialized)
		{
			yield return null;
		}
		if (Storager.getInt(Defs.RazerSN, true) > 0)
		{
			Storager.setInt(Defs.Razer_3, 1, true);
		}
		if (Storager.getInt(Defs.Razer_2SN, true) > 0)
		{
			Storager.setInt(Defs.Razer_3, 1, true);
		}
		if (Storager.getInt(Defs.Revolver2SN, true) > 0)
		{
			Storager.setInt(Defs.revolver_2_3, 1, true);
		}
		if (Storager.getInt(Defs.revolver_2_2, true) > 0)
		{
			Storager.setInt(Defs.revolver_2_3, 1, true);
		}
		if (Storager.getInt(Defs.Sword_2_SN, true) > 0)
		{
			Storager.setInt(Defs.Sword_2_3, 1, true);
		}
		if (Storager.getInt(Defs.Sword_22SN, true) > 0)
		{
			Storager.setInt(Defs.Sword_2_3, 1, true);
		}
		if (!_initialized)
		{
			yield return null;
		}
		if (Storager.getInt(Defs.MinigunSN, true) > 0)
		{
			Storager.setInt(Defs.minigun_3, 1, true);
		}
		if (Storager.getInt(Defs.RedMinigunSN, true) > 0)
		{
			Storager.setInt(Defs.minigun_3, 1, true);
		}
		if (Storager.getInt(Defs.m79_2, true) > 0)
		{
			Storager.setInt(Defs.m79_3, 1, true);
		}
		if (Storager.getInt(Defs.Bazooka_2_1, true) > 0)
		{
			Storager.setInt(Defs.Bazooka_2_3, 1, true);
		}
		if (Storager.getInt(Defs.plazmaSN, true) > 0)
		{
			Storager.setInt(Defs.plazma_3, 1, true);
		}
		if (Storager.getInt(Defs.plazma_2, true) > 0)
		{
			Storager.setInt(Defs.plazma_3, 1, true);
		}
		if (!_initialized)
		{
			yield return null;
		}
		if (Storager.getInt(Defs._3PLShotgunSN, true) > 0)
		{
			Storager.setInt(Defs._3_shotgun_3, 1, true);
		}
		if (Storager.getInt(Defs._3_shotgun_2, true) > 0)
		{
			Storager.setInt(Defs._3_shotgun_3, 1, true);
		}
		if (Storager.getInt(Defs.LaserRifleSN, true) > 0)
		{
			Storager.setInt(Defs.Red_Stone_3, 1, true);
		}
		if (Storager.getInt(Defs.GoldenRed_StoneSN, true) > 0)
		{
			Storager.setInt(Defs.Red_Stone_3, 1, true);
		}
		if (Storager.getInt(Defs.LightSwordSN, true) > 0)
		{
			Storager.setInt(Defs.LightSword_3, 1, true);
		}
		if (Storager.getInt(Defs.RedLightSaberSN, true) > 0)
		{
			Storager.setInt(Defs.LightSword_3, 1, true);
		}
		if (Storager.getInt(Defs.katana_SN, true) > 0)
		{
			Storager.setInt(Defs.katana_3_SN, 1, true);
		}
		if (Storager.getInt(Defs.katana_2_SN, true) > 0)
		{
			Storager.setInt(Defs.katana_3_SN, 1, true);
		}
	}
}

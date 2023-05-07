using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Rilisoft;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopNGUIController : MonoBehaviour
{
	public enum TutorialPhase
	{
		SelectWeaponCategory,
		SelectSniperSection,
		SelectRifle,
		EquipRifle,
		SelectWearCategory,
		EquipArmor,
		SelectArmorSection,
		SelectPetsCategory,
		ShowEggsHint,
		LeaveArmory
	}

	public enum TutorialStageTrigger
	{
		Enter,
		Exit,
		Update
	}

	private class TutorialState
	{
		public TutorialPhase ForStage;

		public Action<TutorialStageTrigger> StageAct;

		public TutorialState(TutorialPhase forStage, Action<TutorialStageTrigger> act)
		{
			ForStage = forStage;
			StageAct = act;
		}
	}

	public enum CategoryNames
	{
		PrimaryCategory = 0,
		BackupCategory = 1,
		MeleeCategory = 2,
		SpecilCategory = 3,
		SniperCategory = 4,
		PremiumCategory = 5,
		HatsCategory = 6,
		ArmorCategory = 7,
		SkinsCategory = 8,
		CapesCategory = 9,
		BootsCategory = 10,
		GearCategory = 11,
		MaskCategory = 12,
		SkinsCategoryEditor = 1000,
		SkinsCategoryMale = 1100,
		SkinsCategoryFemale = 1200,
		SkinsCategorySpecial = 1300,
		SkinsCategoryPremium = 1400,
		LeagueWeaponSkinsCategory = 2000,
		LeagueHatsCategory = 2100,
		LeagueSkinsCategory = 2200,
		ThrowingCategory = 12500,
		ToolsCategoty = 13000,
		SupportCategory = 13500,
		PetsCategory = 25000,
		EggsCategory = 30000,
		BestWeapons = 35000,
		BestWear = 40000,
		BestGadgets = 45000
	}

	internal sealed class CategoryNameComparer : IEqualityComparer<CategoryNames>
	{
		private static readonly CategoryNameComparer s_instance = new CategoryNameComparer();

		public static CategoryNameComparer Instance
		{
			get
			{
				return s_instance;
			}
		}

		private CategoryNameComparer()
		{
		}

		public bool Equals(CategoryNames x, CategoryNames y)
		{
			return x == y;
		}

		public int GetHashCode(CategoryNames obj)
		{
			return (int)obj;
		}
	}

	public enum Supercategory
	{
		Best,
		Weapons,
		Wear,
		League,
		Gadgets,
		Pets
	}

	private sealed class SupercategoryComparer : IEqualityComparer<Supercategory>
	{
		public bool Equals(Supercategory x, Supercategory y)
		{
			return x == y;
		}

		public int GetHashCode(Supercategory obj)
		{
			return (int)obj;
		}
	}

	public class ShopItem : IEquatable<ShopItem>
	{
		public string Id { get; private set; }

		public CategoryNames Category { get; private set; }

		public ShopItem(string id, CategoryNames category)
		{
			Id = id;
			Category = category;
		}

		public bool Equals(ShopItem other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			return Id.Equals(other.Id) && Category.Equals(other.Category);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ShopItem))
			{
				return false;
			}
			ShopItem other = (ShopItem)obj;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			int num = ((Id != null) ? Id.GetHashCode() : 0);
			int hashCode = Category.GetHashCode();
			return hashCode ^ num;
		}
	}

	public delegate void Action7<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

	public const string BoughtCurrencsySettingBase = "BoughtCurrency";

	public const string KEY_TUTORIAL_STATE_PASSED = "shop_tutorial_state_passed_VER_12_1";

	private const string KEY_TUTORIAL_STATE_VIEWED = "shop_tutorial_state_viewed";

	public const string KEY_BTN_TRY_HIGHLIGHTED = "tutorial_button_try_highlighted";

	private const string KEY_TUTORIAL_INFO_HINT_VIEWED = "tutorial_info_hint_viewed";

	private const string KEY_TUTORIAL_EGG_INFO_HINT_VIEWED = "Shop.Tutorial.KEY_TUTORIAL_EGG_INFO_HINT_VIEWED";

	private const string KEY_TUTORIAL_PET_UPGRADE_HINT_VIEWED = "Shop.Tutorial.KEY_TUTORIAL_PET_UPRADE_HINT_VIEWED";

	private const string ShowArmorKey = "ShowArmorKeySetting";

	private const string ShowHatKey = "ShowHatKeySetting";

	private const string ShowWearKey = "ShowWearKeySetting";

	public const string CustomSkinID = "CustomSkinID";

	public const string TrainingShopStageStepKey = "ShopNGUIController.TrainingShopStageStepKey";

	private const float differenceOfWidthOfBackgroundAndGrid = 12f;

	[Header("Shop 2016 - Armor Carousel")]
	public GameObject armorCarousel;

	public UIPanel scrollViewPanel;

	public UIGrid wrapContent;

	public MyCenterOnChild carouselCenter;

	[NonSerialized]
	public int itemIndexInCarousel = -1;

	private GameObject _lastSelectedItem;

	private static List<ShopItem> m_bestItemsToRemoveOnLeave = new List<ShopItem>();

	private static readonly Dictionary<CategoryNames, List<CategoryNames>> m_categoriesOfBestCategories = new Dictionary<CategoryNames, List<CategoryNames>>(3, CategoryNameComparer.Instance)
	{
		{
			CategoryNames.BestWeapons,
			new List<CategoryNames>
			{
				CategoryNames.PrimaryCategory,
				CategoryNames.BackupCategory,
				CategoryNames.SpecilCategory,
				CategoryNames.SniperCategory,
				CategoryNames.PremiumCategory,
				CategoryNames.MeleeCategory
			}
		},
		{
			CategoryNames.BestWear,
			new List<CategoryNames>
			{
				CategoryNames.HatsCategory,
				CategoryNames.MaskCategory,
				CategoryNames.CapesCategory,
				CategoryNames.BootsCategory,
				CategoryNames.SkinsCategory
			}
		},
		{
			CategoryNames.BestGadgets,
			new List<CategoryNames>
			{
				CategoryNames.ThrowingCategory,
				CategoryNames.ToolsCategoty,
				CategoryNames.SupportCategory
			}
		}
	};

	[Header("Training After Removing Novice Armor")]
	public GameObject trainingRemoveNoviceArmorCollider;

	public List<GameObject> trainingTipsRemovedNoviceArmor = new List<GameObject>();

	[Header("DEPRECATED Training")]
	public GameObject trainingColliders;

	public List<GameObject> trainingTips = new List<GameObject>();

	[SerializeField]
	private GameObject _tutorialHintsContainer;

	[SerializeField]
	private GameObject _tutorialHintsExtContainer;

	private List<TutorialState> _tutorialStates = new List<TutorialState>();

	private TutorialState _tutorialCurrentState;

	private List<CancellationTokenSource> _tutorialTokensSources = new List<CancellationTokenSource>();

	private List<CancellationTokenSource> _tutorialPetUpgradeTokensSources = new List<CancellationTokenSource>();

	private List<CancellationTokenSource> _tutorialEggsInfoTokensSources = new List<CancellationTokenSource>();

	private EventDelegate m_tutorialInfoBtnED;

	private EventDelegate m_tutorialInfoBtnED_Eggs;

	private bool m_petUpgradeHideCoroutineStarted;

	private bool m_startedLeaveArmoryStateTimer;

	private bool m_shouldMoveToLeaveState;

	private static bool _showArmorValue = true;

	private static bool _showHatValue = true;

	private static bool? _showWearValue;

	public static ShopNGUIController sharedShop;

	[Header("Shop 2016")]
	[Header("Shop 2016 - Tutorial")]
	public GameObject noOffersAtThisTime;

	public List<UILabel> weaponSupercategoryUnlockedItems;

	public List<UILabel> gadgetsSupercategoryUnlockedItems;

	public List<UILabel> primaryWeaponsUnlockedItems;

	public List<UILabel> backupWeaponsUnlockedItems;

	public List<UILabel> meleeWeaponsUnlockedItems;

	public List<UILabel> specialWeaponsUnlockedItems;

	public List<UILabel> sniperWeaponsUnlockedItems;

	public List<UILabel> premiumWeaponsUnlockedItems;

	public List<UILabel> throwingGadgetsUnlockedItems;

	public List<UILabel> toolsGadgetsUnlockedItems;

	public List<UILabel> supportGadgetsUnlockedItems;

	public UISprite sideFrameNearCategoryButtons;

	public List<UILabel> petUpgradePointsLabels;

	public GameObject petUpgradesInSpecial;

	public UISprite petUpgradeIndicator;

	public GameObject panelProperties;

	public GameObject returnEveryDay;

	public GameObject superIncubatorButton;

	public GameObject noEggs;

	public GameObject noPets;

	public GameObject gridSlider;

	public GameObject eggScreen;

	public List<UILabel> eggTimeLabels;

	public UIPanel armoryRootPanel;

	public List<Transform> buttonContainers;

	public PropertiesHideSHow hideButton;

	public Transform bottomPointForShader;

	public Transform topPointForShader;

	public GameObject propertiesAndButtonsPanel;

	public GameObject equipped;

	public PropertiesArmoryItemContainer propertiesContainer;

	public UIToggle showArmorButton;

	public UIToggle showWearButton;

	public List<UIRect> widgetsToUpdateAnchorsOnStart;

	public UILabel WeaponsRarityLabel;

	public List<UILabel> wearNameLabels;

	public List<UILabel> skinNameLabels;

	public List<UILabel> armorNameLabels;

	public GameObject stub;

	public UILabel fireRate;

	public UILabel fireRateMElee;

	public UILabel mobility;

	public UILabel mobilityMelee;

	public UILabel capacity;

	public UILabel damage;

	public UILabel damageMelee;

	public UIWidget sccrollViewBackground;

	public UIGrid itemsGrid;

	public UIScrollView gridScrollView;

	public CategoryButtonsController superCategoriesButtonController;

	public UIGrid weaponCategoriesGrid;

	public UIGrid wearCategoriesGrid;

	public UIGrid gridCategoriesLeague;

	public UIGrid petsGrid;

	public UIGrid gadgetsGrid;

	public UIGrid bestCategoriesGrid;

	public UIButton facebookLoginLockedSkinButton;

	public UIButton upgradeButton;

	public UIButton buyButton;

	public UIButton equipButton;

	public UIButton unequipButton;

	public UIButton infoButton;

	public UIButton editButton;

	public UIButton deleteButton;

	public UIButton enableButton;

	public UIButton unlockButton;

	public UIButton createButton;

	public GameObject weaponProperties;

	public GameObject meleeProperties;

	public GameObject SpecialParams;

	public List<UISprite> effectsSprites;

	public List<UILabel> effectsLabels;

	public GameObject nonArmorWearProperties;

	public GameObject armorWearProperties;

	public GameObject skinProperties;

	public UILabel nonArmorWearDEscription;

	public UILabel armorWearDescription;

	public UILabel armorCountLabel;

	public GameObject armorLock;

	public Transform rentScreenPoint;

	public GameObject purchaseSuccessful;

	public List<Light> mylights;

	public Transform characterPoint;

	public GameObject mainPanel;

	public UIButton backButton;

	public UIButton coinShopButton;

	public Action resumeAction;

	public Action wearResumeAction;

	public Action<CategoryNames, string> wearUnequipAction;

	public Action<CategoryNames, string, string> wearEquipAction;

	public Action<string> buyAction;

	public Action<string> equipAction;

	public Action<string> activatePotionAction;

	public Action<string> onEquipSkinAction;

	private GameObject weapon;

	private AnimationClip profile;

	public UIButton tryGun;

	public UILabel caption;

	public GameObject gadgetBlocker;

	public AudioClip upgradeBtnSound;

	public AudioClip upgradeBtnPetSound;

	public bool inGame = true;

	public List<Camera> ourCameras;

	public AnimationCoroutineRunner animationCoroutineRunner;

	public AnimationCoroutineRunner petProfileAnimationRunner;

	public GameObject ActiveObject;

	public bool EnableConfigurePos;

	public GameObject tryGunPanel;

	public UILabel tryGunMatchesCount;

	public UILabel tryGunDiscountTime;

	public static readonly Dictionary<string, string> weaponCategoryLocKeys = new Dictionary<string, string>(6)
	{
		{
			CategoryNames.PrimaryCategory.ToString(),
			"Key_0352"
		},
		{
			CategoryNames.BackupCategory.ToString(),
			"Key_0442"
		},
		{
			CategoryNames.MeleeCategory.ToString(),
			"Key_0441"
		},
		{
			CategoryNames.SpecilCategory.ToString(),
			"Key_0440"
		},
		{
			CategoryNames.SniperCategory.ToString(),
			"Key_1669"
		},
		{
			CategoryNames.PremiumCategory.ToString(),
			"Key_0093"
		}
	};

	private GameObject gadgetPreview;

	private GameObject[] _onPersArmorRefs;

	private float m_timeOfPLastStuffUpdate;

	private bool _shouldShowRewardWindowSkin;

	private bool _shouldShowRewardWindowCape;

	private Dictionary<Supercategory, List<UILabel>> m_supercategoriesToUnlockedItemsLabels;

	private Dictionary<CategoryNames, List<UILabel>> m_categoriesToUnlockedItemsLabels;

	private GameObject _refOnLowPolyArmor;

	private Material[] _refsOnLowPolyArmorMaterials;

	public static string[] gearOrder = PotionsController.potions;

	private float lastTime;

	public static float IdleTimeoutPers = 5f;

	private float idleTimerLastTime;

	private float _timePurchaseSuccessfulShown;

	private float _timePurchaseRentSuccessfulShown;

	private bool categoryGridsRepositioned;

	private IDisposable _backSubscription;

	private bool _escapeRequested;

	private float timeOfEnteringShopForProtectFromPressingCoinsButton;

	private Color? _storedAmbientLight;

	private bool? _storedFogEnabled;

	private static List<Camera> disablesCameras = new List<Camera>();

	private bool _isFromPromoActions;

	private string _promoActionsIdClicked;

	private string _assignedWeaponTag = string.Empty;

	private bool InTrainingAfterNoviceArmorRemoved;

	private Rect _touchZoneForRotatingCharacter;

	private float _rotationRateForCharacter;

	private static Comparison<string> skinIdsComparisonForShop = delegate(string kvp1, string kvp2)
	{
		//Discarded unreachable code: IL_0077, IL_0084
		try
		{
			bool flag = SkinsController.standardSkinsIds.Contains(kvp1);
			bool flag2 = SkinsController.standardSkinsIds.Contains(kvp2);
			if (flag == flag2)
			{
				if (flag)
				{
					return SkinsController.standardSkinsIds.IndexOf(kvp1).CompareTo(SkinsController.standardSkinsIds.IndexOf(kvp2));
				}
				return long.Parse(kvp1).CompareTo(long.Parse(kvp2));
			}
			return (!flag) ? 1 : (-1);
		}
		catch
		{
			return 0;
		}
	};

	private readonly Dictionary<Supercategory, CategoryNames> supercategoryLastUsedCategory = new Dictionary<Supercategory, CategoryNames>(6, new SupercategoryComparer())
	{
		{
			Supercategory.Best,
			CategoryNames.BestWeapons
		},
		{
			Supercategory.Weapons,
			CategoryNames.PrimaryCategory
		},
		{
			Supercategory.Wear,
			CategoryNames.HatsCategory
		},
		{
			Supercategory.League,
			CategoryNames.LeagueWeaponSkinsCategory
		},
		{
			Supercategory.Gadgets,
			CategoryNames.ThrowingCategory
		},
		{
			Supercategory.Pets,
			CategoryNames.EggsCategory
		}
	};

	private ShopItem m_itemToSetAfterEnter;

	public CategoryNames CategoryToChoose;

	[Range(-200f, 200f)]
	public float firstOFfset = -50f;

	[Range(-200f, 200f)]
	public float secondOffset = 50f;

	private CharacterInterface characterInterface;

	public float scaleCoefficent = 0.5f;

	private static List<ShopPositionParams> hats = new List<ShopPositionParams>();

	private static List<ShopPositionParams> capes = new List<ShopPositionParams>();

	private static List<ShopPositionParams> boots = new List<ShopPositionParams>();

	private static List<ShopPositionParams> masks = new List<ShopPositionParams>();

	private static List<ShopPositionParams> armor = new List<ShopPositionParams>();

	private Action<List<ShopPositionParams>, CategoryNames> sort = delegate(List<ShopPositionParams> prefabs, CategoryNames c)
	{
		Comparison<ShopPositionParams> comparison = delegate(ShopPositionParams go1, ShopPositionParams go2)
		{
			List<string> list = null;
			List<string> list2 = null;
			foreach (List<string> item in Wear.wear[c])
			{
				if (item.Contains(go1.name))
				{
					list = item;
				}
				if (item.Contains(go2.name))
				{
					list2 = item;
				}
			}
			if (list == null || list2 == null)
			{
				return 0;
			}
			return (list == list2) ? (list.IndexOf(go1.name) - list.IndexOf(go2.name)) : (Wear.wear[c].IndexOf(list) - Wear.wear[c].IndexOf(list2));
		};
		prefabs.Sort(comparison);
	};

	private readonly Dictionary<CategoryNames, bool> propertiesShownInCategory = new Dictionary<CategoryNames, bool>(20, CategoryNameComparer.Instance)
	{
		{
			CategoryNames.PrimaryCategory,
			true
		},
		{
			CategoryNames.BackupCategory,
			true
		},
		{
			CategoryNames.MeleeCategory,
			true
		},
		{
			CategoryNames.SpecilCategory,
			true
		},
		{
			CategoryNames.SniperCategory,
			true
		},
		{
			CategoryNames.PremiumCategory,
			true
		},
		{
			CategoryNames.ArmorCategory,
			true
		},
		{
			CategoryNames.BootsCategory,
			true
		},
		{
			CategoryNames.HatsCategory,
			true
		},
		{
			CategoryNames.CapesCategory,
			true
		},
		{
			CategoryNames.LeagueHatsCategory,
			true
		},
		{
			CategoryNames.MaskCategory,
			true
		},
		{
			CategoryNames.SkinsCategory,
			false
		},
		{
			CategoryNames.SkinsCategoryMale,
			false
		},
		{
			CategoryNames.SkinsCategoryFemale,
			false
		},
		{
			CategoryNames.SkinsCategoryPremium,
			false
		},
		{
			CategoryNames.SkinsCategoryEditor,
			false
		},
		{
			CategoryNames.LeagueSkinsCategory,
			false
		},
		{
			CategoryNames.LeagueWeaponSkinsCategory,
			false
		},
		{
			CategoryNames.ThrowingCategory,
			true
		},
		{
			CategoryNames.ToolsCategoty,
			true
		},
		{
			CategoryNames.SupportCategory,
			true
		},
		{
			CategoryNames.EggsCategory,
			false
		},
		{
			CategoryNames.PetsCategory,
			true
		},
		{
			CategoryNames.GearCategory,
			false
		},
		{
			CategoryNames.BestWeapons,
			true
		},
		{
			CategoryNames.BestWear,
			true
		},
		{
			CategoryNames.BestGadgets,
			true
		}
	};

	private GameObject pixlMan;

	private Transform infoScreen;

	private int needRefreshInLateUpdate;

	private Texture _skinsMakerSkinCache;

	private bool updateScrollViewOnLateUpdateForTryPanel;

	private static bool gridScrollViewPanelUpdatedOnFirstLaunch = false;

	private static bool _gridInitiallyRepositioned = false;

	private int itemScrollBottomAnchor;

	private int itemScrollBottomAnchorRent;

	private string _gunThatWeUsedInPolygon;

	private bool m_firstReportItemsViewedSkipped;

	private float m_timeOfLAstReportVisibleCells = float.MinValue;

	private ShopItem m_currentItem;

	public static bool NoviceArmorAvailable
	{
		get
		{
			return Storager.getInt("Training.NoviceArmorUsedKey", false) == 1 && (!TrainingController.TrainingCompleted || Storager.getInt("Training.ShouldRemoveNoviceArmorInShopKey", false) == 1);
		}
	}

	private static List<ShopItem> BestItemsToRemoveOnLeave
	{
		get
		{
			return m_bestItemsToRemoveOnLeave;
		}
	}

	private static Dictionary<CategoryNames, List<CategoryNames>> СategoriesOfBestCategories
	{
		get
		{
			return m_categoriesOfBestCategories;
		}
	}

	public TutorialPhase TutorialPhasePassed
	{
		get
		{
			return (TutorialPhase)PlayerPrefs.GetInt("shop_tutorial_state_passed_VER_12_1", 0);
		}
		set
		{
			PlayerPrefs.SetInt("shop_tutorial_state_passed_VER_12_1", (int)value);
		}
	}

	private TutorialPhase? TutorialPhaseLastViewed
	{
		get
		{
			int @int = PlayerPrefs.GetInt("shop_tutorial_state_viewed", -1);
			TutorialPhase? result = null;
			if (@int > -1)
			{
				result = (TutorialPhase)@int;
			}
			return result;
		}
		set
		{
			PlayerPrefs.SetInt("shop_tutorial_state_viewed", (int)value.Value);
		}
	}

	public static bool ShowArmor
	{
		get
		{
			return _showArmorValue;
		}
		private set
		{
			if (_showArmorValue != value)
			{
				_showArmorValue = value;
				Action showArmorChanged = ShopNGUIController.ShowArmorChanged;
				if (showArmorChanged != null)
				{
					showArmorChanged();
				}
			}
		}
	}

	public static bool ShowHat
	{
		get
		{
			return _showHatValue;
		}
		private set
		{
			if (_showHatValue != value)
			{
				_showHatValue = value;
				Action showArmorChanged = ShopNGUIController.ShowArmorChanged;
				if (showArmorChanged != null)
				{
					showArmorChanged();
				}
			}
		}
	}

	public static bool ShowWear
	{
		get
		{
			if (!_showWearValue.HasValue)
			{
				_showWearValue = PlayerPrefs.GetInt("ShowWearKeySetting", 0).ToBool();
			}
			return _showWearValue.Value;
		}
		private set
		{
			if (!_showWearValue.HasValue || _showWearValue.Value != value)
			{
				_showWearValue = value;
				PlayerPrefs.SetInt("ShowWearKeySetting", value.ToInt());
				Action showWearChanged = ShopNGUIController.ShowWearChanged;
				if (showWearChanged != null)
				{
					showWearChanged();
				}
			}
		}
	}

	public ShopItem CurrentItem
	{
		get
		{
			return m_currentItem;
		}
		private set
		{
			m_currentItem = value;
		}
	}

	public bool IsExiting { get; private set; }

	public CategoryNames CurrentCategory { get; private set; }

	public Camera Camera3D
	{
		get
		{
			return (ourCameras == null) ? null : ourCameras.FirstOrDefault((Camera c) => c.name.Equals("Camera3D"));
		}
	}

	public string GunThatWeUsedInPolygon
	{
		get
		{
			return _gunThatWeUsedInPolygon;
		}
		set
		{
			_gunThatWeUsedInPolygon = value;
		}
	}

	public bool EggWindowIsOpened
	{
		get
		{
			EggHatchingWindowController[] componentsInChildren = rentScreenPoint.GetComponentsInChildren<EggHatchingWindowController>();
			return componentsInChildren != null && componentsInChildren.Length > 0;
		}
	}

	public List<ArmoryCell> AllArmoryCells
	{
		get
		{
			return (from cell in itemsGrid.GetComponentsInChildren<ArmoryCell>(true)
				orderby cell.name
				select cell).ToList();
		}
	}

	private Supercategory CurrentSupercategory
	{
		get
		{
			return (Supercategory)(int)Enum.Parse(typeof(Supercategory), superCategoriesButtonController.currentBtnName);
		}
	}

	public bool IsFromPromoActions
	{
		get
		{
			return _isFromPromoActions;
		}
	}

	public static bool GuiActive
	{
		get
		{
			return sharedShop != null && sharedShop.ActiveObject.activeInHierarchy;
		}
		set
		{
			bool guiActive = GuiActive;
			if (value)
			{
				if (ArmoryInfoScreenController.sharedController == null)
				{
					if (sharedShop._backSubscription != null)
					{
						sharedShop._backSubscription.Dispose();
					}
					sharedShop._backSubscription = BackSystem.Instance.Register(sharedShop.HandleEscape, "Shop");
				}
			}
			else
			{
				if (sharedShop._tutorialCurrentState != null)
				{
					sharedShop._tutorialCurrentState.StageAct(TutorialStageTrigger.Exit);
				}
				if (sharedShop._backSubscription != null)
				{
					if (ArmoryInfoScreenController.sharedController == null)
					{
						sharedShop._backSubscription.Dispose();
						sharedShop._backSubscription = null;
					}
					Storager.RefreshWeaponDigestIfDirty();
				}
			}
			if (value && sharedShop.tryGun != null)
			{
				sharedShop.tryGun.gameObject.SetActive(false);
			}
			FreeAwardShowHandler.CheckShowChest(value);
			if (sharedShop != null)
			{
				sharedShop.SetOtherCamerasEnabled(!value);
				if (value)
				{
					sharedShop.stub.SetActive(true);
					try
					{
						SwitchAmbientLightAndFogToShop();
						sharedShop.timeOfEnteringShopForProtectFromPressingCoinsButton = Time.realtimeSinceStartup;
						sharedShop.UpdateIcons(false);
						CategoryNames inFactCategory = CategoryNames.PrimaryCategory;
						ShopItem itemToSet;
						if (sharedShop.m_itemToSetAfterEnter != null || sharedShop.CategoryToChoose != 0)
						{
							itemToSet = sharedShop.m_itemToSetAfterEnter;
							inFactCategory = sharedShop.CategoryToChoose;
							sharedShop.m_itemToSetAfterEnter = null;
							sharedShop.CategoryToChoose = CategoryNames.PrimaryCategory;
						}
						else
						{
							CategoryNames categoryNames = CategoryNames.PrimaryCategory;
							if (WeaponManager.sharedManager != null && WeaponManager.sharedManager._currentFilterMap == 1)
							{
								inFactCategory = CategoryNames.MeleeCategory;
								categoryNames = CategoryNames.MeleeCategory;
							}
							else if (WeaponManager.sharedManager != null && WeaponManager.sharedManager._currentFilterMap == 2)
							{
								inFactCategory = CategoryNames.SniperCategory;
								categoryNames = CategoryNames.SniperCategory;
							}
							string text = _CurrentWeaponSetIDs()[(int)categoryNames];
							if (text != null)
							{
								itemToSet = new ShopItem(text, categoryNames);
							}
							else
							{
								string id = HighestDPSGun(categoryNames, out inFactCategory);
								itemToSet = new ShopItem(id, inFactCategory);
							}
						}
						string text2 = null;
						if (itemToSet != null)
						{
							if (IsGadgetsCategory(inFactCategory) || inFactCategory == CategoryNames.BestGadgets)
							{
								text2 = GadgetsInfo.LastBoughtFor(itemToSet.Id);
							}
							else if (inFactCategory == CategoryNames.PetsCategory)
							{
								text2 = itemToSet.Id;
								try
								{
									PlayerPet playerPet = Singleton<PetsManager>.Instance.GetPlayerPet(itemToSet.Id);
									text2 = ((playerPet == null) ? itemToSet.Id : playerPet.InfoId);
								}
								catch (Exception ex)
								{
									Debug.LogErrorFormat("Exception in getting actual upgrade of pet in GuiActive: {0}", ex);
								}
							}
							else
							{
								text2 = WeaponManager.LastBoughtTag(itemToSet.Id);
							}
							if (text2 == null)
							{
								if (IsWearCategory(itemToSet.Category))
								{
									List<string> list2 = Wear.wear.Values.SelectMany((List<List<string>> listOfLists) => listOfLists).FirstOrDefault((List<string> list) => list.Contains(itemToSet.Id));
									if (list2 != null)
									{
										itemToSet = new ShopItem(list2[0], itemToSet.Category);
									}
								}
							}
							else
							{
								itemToSet = new ShopItem(text2, itemToSet.Category);
							}
						}
						sharedShop.AllCategoryButtonTransforms().ForEach(delegate(Transform t)
						{
							t.GetComponent<UIToggle>().SetInstantlyNoHandlers(false);
						});
						BestItemsToRemoveOnLeave.Clear();
						sharedShop.MakeShopActive();
						sharedShop.ChooseCategoryAndSuperCategory(inFactCategory, itemToSet, true);
						sharedShop.UpdateAllWearAndSkinOnPers();
						sharedShop.AdjustCategoryGridCells();
						sharedShop.SetPetsCategoryEnable();
						if (ArmoryInfoScreenController.sharedController != null)
						{
							ArmoryInfoScreenController.sharedController.DestroyWindow();
						}
						try
						{
							if (Defs.isHunger && SceneManager.GetActiveScene().name != "ConnectScene")
							{
								List<CategoryNames> second = (from weapon in WeaponManager.sharedManager.allAvailablePlayerWeapons.OfType<Weapon>()
									select (CategoryNames)(weapon.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1)).Distinct().ToList();
								List<CategoryNames> list3 = new List<CategoryNames>();
								list3.Add(CategoryNames.PrimaryCategory);
								list3.Add(CategoryNames.BackupCategory);
								list3.Add(CategoryNames.MeleeCategory);
								list3.Add(CategoryNames.SpecilCategory);
								list3.Add(CategoryNames.SniperCategory);
								list3.Add(CategoryNames.PremiumCategory);
								List<CategoryNames> first = list3;
								first = first.Except(second).ToList();
								sharedShop.AdjustCategoryButtonsForFilterMap();
								sharedShop.DisableButtonsInIndexes(first);
							}
						}
						catch (Exception ex2)
						{
							Debug.LogError("Exception in disabling buttons for hunger: " + ex2);
						}
					}
					catch (Exception ex3)
					{
						Debug.LogError("Exception in ShopNGUIController.GuiActive: " + ex3);
					}
					sharedShop.StartCoroutine(sharedShop.DisableStub());
					sharedShop.UpdateUnlockedItemsIndicators();
					RemoveViewedUnlockedItems();
				}
				else
				{
					Color? storedAmbientLight = sharedShop._storedAmbientLight;
					RenderSettings.ambientLight = ((!storedAmbientLight.HasValue) ? RenderSettings.ambientLight : storedAmbientLight.Value);
					bool? storedFogEnabled = sharedShop._storedFogEnabled;
					RenderSettings.fog = ((!storedFogEnabled.HasValue) ? RenderSettings.fog : storedFogEnabled.Value);
					MyCenterOnChild myCenterOnChild = sharedShop.carouselCenter;
					myCenterOnChild.onFinished = (SpringPanel.OnFinished)Delegate.Remove(myCenterOnChild.onFinished, new SpringPanel.OnFinished(sharedShop.HandleCarouselCentering));
					PromoActionsManager.ActionsUUpdated -= sharedShop.HandleOffersUpdated;
					sharedShop.SetWeapon(null, null);
					sharedShop.ActiveObject.SetActive(false);
					sharedShop.carouselCenter.enabled = false;
					WeaponManager.ClearCachedInnerPrefabs();
					sharedShop.AllArmoryCells.ForEach(delegate(ArmoryCell cell)
					{
						if (cell.icon != null)
						{
							cell.icon.mainTexture = null;
						}
					});
				}
			}
			if (guiActive != GuiActive)
			{
				if (!GuiActive)
				{
					sharedShop.characterInterface.UpdatePet(string.Empty);
				}
				Action<bool> shopChangedIsActive = ShopNGUIController.ShopChangedIsActive;
				if (shopChangedIsActive != null)
				{
					shopChangedIsActive(GuiActive);
				}
			}
		}
	}

	public CharacterInterface ShopCharacterInterface
	{
		get
		{
			return characterInterface;
		}
	}

	public Transform armorPoint
	{
		get
		{
			return characterInterface.armorPoint;
		}
	}

	public Transform leftBootPoint
	{
		get
		{
			return characterInterface.leftBootPoint;
		}
	}

	public Transform rightBootPoint
	{
		get
		{
			return characterInterface.rightBootPoint;
		}
	}

	public Transform capePoint
	{
		get
		{
			return characterInterface.capePoint;
		}
	}

	public Transform hatPoint
	{
		get
		{
			return characterInterface.hatPoint;
		}
	}

	public Transform maskPoint
	{
		get
		{
			return characterInterface.maskPoint;
		}
	}

	public GameObject body
	{
		get
		{
			return characterInterface.gunPoint.gameObject;
		}
	}

	public Animation animation
	{
		get
		{
			return characterInterface.animation;
		}
	}

	public Transform MainMenu_Pers
	{
		get
		{
			return characterInterface.transform;
		}
	}

	public static event Action<string, CategoryNames, string> EquippedWearInCategory;

	public static event Action<CategoryNames, string> UnequippedWearInCategory;

	public static event Action ShowArmorChanged;

	public static event Action ShowWearChanged;

	public static event Action<bool> ShopChangedIsActive;

	public static event Action GunOrArmorBought;

	public static event Action<string> TryGunBought;

	public static event Action<string, string, GadgetInfo.GadgetCategory> EquippedGadget;

	public static event Action<string, string> EquippedPet;

	public static event Action<string> UnequippedPet;

	public static event Action GunBought;

	internal void ReloadCarousel(ShopItem item)
	{
		ShopCarouselElement[] componentsInChildren = wrapContent.GetComponentsInChildren<ShopCarouselElement>(true);
		ShopCarouselElement[] array = componentsInChildren;
		foreach (ShopCarouselElement shopCarouselElement in array)
		{
			UnityEngine.Object.Destroy(shopCarouselElement.gameObject);
			shopCarouselElement.transform.parent = null;
		}
		wrapContent.Reposition();
		List<GameObject> modelsList = GetModelsList(CurrentCategory);
		if (item == null)
		{
			item = CurrentItem;
		}
		int num = 10000;
		if (item != null)
		{
			num = modelsList.FindIndex((GameObject go) => go.nameNoClone() == item.Id);
		}
		for (int j = 0; j < modelsList.Count; j++)
		{
			GameObject original = Resources.Load<GameObject>("ShopCarouselElement");
			GameObject gameObject = UnityEngine.Object.Instantiate(original);
			gameObject.transform.parent = wrapContent.transform;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.transform.localPosition = Vector3.zero;
			GameObject gameObject2 = modelsList[j];
			gameObject.name = j.ToString("D7");
			ShopCarouselElement sce = gameObject.GetComponent<ShopCarouselElement>();
			string itemID = gameObject2.name;
			sce.itemID = itemID;
			sce.SetupPriceAndDiscount(sce.itemID, CategoryNames.ArmorCategory);
			bool flag = false;
			Action<GameObject, CategoryNames> action = delegate(GameObject loadedOBject, CategoryNames category)
			{
				AddModel(loadedOBject, delegate(GameObject manipulateObject, Vector3 positionShop, Vector3 rotationShop, string readableName, float scaleCoefShop, int tier, int league)
				{
					if (sce == null)
					{
						UnityEngine.Object.Destroy(manipulateObject);
					}
					else
					{
						sce.readableName = readableName ?? string.Empty;
						manipulateObject.transform.parent = sce.transform;
						sce.baseScale = new Vector3(scaleCoefShop, scaleCoefShop, scaleCoefShop);
						sce.model = manipulateObject.transform;
						sce.ourPosition = positionShop;
						sce.SetPos((!EnableConfigurePos) ? 0f : 1f, 0f);
						sce.model.Rotate(rotationShop, Space.World);
						if (ExpController.Instance != null && ExpController.Instance.OurTier < tier && tier < 100 && sce.itemID == WeaponManager.FirstUnboughtTag(sce.itemID) && sce.itemID != "cape_Custom" && sce.itemID != "boots_tabi" && sce.locked != null)
						{
							sce.locked.SetActive(true);
						}
						if (sce.itemID != WeaponManager.FirstUnboughtTag(sce.itemID))
						{
							if (sce.arrow != null)
							{
								sce.arrow.gameObject.SetActive(true);
							}
							Dictionary<CategoryNames, float> dictionary = new Dictionary<CategoryNames, float>(5, CategoryNameComparer.Instance)
							{
								{
									CategoryNames.HatsCategory,
									85f
								},
								{
									CategoryNames.ArmorCategory,
									105f
								},
								{
									CategoryNames.CapesCategory,
									75f
								},
								{
									CategoryNames.BootsCategory,
									81f
								},
								{
									CategoryNames.MaskCategory,
									75f
								}
							};
							if (dictionary.ContainsKey(CategoryNames.ArmorCategory))
							{
								sce.arrnoInitialPos = new Vector3(dictionary[CategoryNames.ArmorCategory], sce.arrnoInitialPos.y, sce.arrnoInitialPos.z);
							}
						}
					}
				}, MapShopCategoryToItemCategory(category));
			};
			action(ItemDb.GetWearFromResources(gameObject2.nameNoClone(), CategoryNames.ArmorCategory), CategoryNames.ArmorCategory);
		}
		wrapContent.Reposition();
		ChooseCarouselItem(item, true);
	}

	public void ChooseCarouselItem(ShopItem itemToSet, bool moveCarousel = false)
	{
		if (itemToSet == null)
		{
			return;
		}
		ShopCarouselElement[] array = wrapContent.GetComponentsInChildren<ShopCarouselElement>(true);
		if (array == null)
		{
			array = new ShopCarouselElement[0];
		}
		ShopCarouselElement[] array2 = array;
		foreach (ShopCarouselElement shopCarouselElement in array2)
		{
			if (!shopCarouselElement.itemID.Equals(itemToSet.Id))
			{
				continue;
			}
			if (moveCarousel)
			{
				SpringPanel component = scrollViewPanel.GetComponent<SpringPanel>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				if (scrollViewPanel.gameObject.activeInHierarchy)
				{
					scrollViewPanel.GetComponent<UIScrollView>().MoveRelative(new Vector3(0f - shopCarouselElement.transform.localPosition.x - scrollViewPanel.transform.localPosition.x, scrollViewPanel.transform.localPosition.y, scrollViewPanel.transform.localPosition.z));
				}
				wrapContent.Reposition();
			}
			CurrentItem = itemToSet;
			UpdatePersWithNewItem(CurrentItem);
			UpdateButtons();
			caption.text = shopCarouselElement.readableName ?? string.Empty;
			{
				foreach (UILabel armorNameLabel in armorNameLabels)
				{
					armorNameLabel.text = ItemDb.GetItemNameByTag(CurrentItem.Id);
				}
				break;
			}
		}
	}

	private void HandleCarouselCentering()
	{
		HandleCarouselCentering(carouselCenter.centeredObject);
	}

	private void HandleCarouselCentering(GameObject centeredObj)
	{
		if (centeredObj != null && centeredObj != _lastSelectedItem)
		{
			_lastSelectedItem = centeredObj;
			ShopCarouselElement component = centeredObj.GetComponent<ShopCarouselElement>();
			ChooseCarouselItem(new ShopItem(component.itemID, CategoryNames.ArmorCategory));
		}
		if (EnableConfigurePos && centeredObj != null)
		{
			centeredObj.GetComponent<ShopCarouselElement>().SetPos(1f, 0f);
		}
	}

	private void CheckCenterItemChanging()
	{
		if (CurrentCategory != CategoryNames.ArmorCategory || !scrollViewPanel.cachedGameObject.activeInHierarchy)
		{
			return;
		}
		Transform cachedTransform = scrollViewPanel.cachedTransform;
		itemIndexInCarousel = -1;
		int num = (int)wrapContent.cellWidth;
		int childCount = wrapContent.transform.childCount;
		if (cachedTransform.localPosition.x > 0f)
		{
			itemIndexInCarousel = 0;
		}
		else if (cachedTransform.localPosition.x < (float)(-1 * num * childCount))
		{
			itemIndexInCarousel = childCount - 1;
		}
		else
		{
			itemIndexInCarousel = -1 * Mathf.RoundToInt((cachedTransform.localPosition.x - (float)(Mathf.CeilToInt(cachedTransform.localPosition.x / (float)num / (float)childCount) * num * childCount)) / (float)num);
		}
		itemIndexInCarousel = Mathf.Clamp(itemIndexInCarousel, 0, childCount - 1);
		if (itemIndexInCarousel >= 0 && itemIndexInCarousel < wrapContent.transform.childCount)
		{
			GameObject centeredObj = wrapContent.transform.GetChild(itemIndexInCarousel).gameObject;
			if (!EnableConfigurePos)
			{
				HandleCarouselCentering(centeredObj);
			}
		}
	}

	private List<GameObject> GetModelsList(CategoryNames category)
	{
		Func<CategoryNames, Comparison<GameObject>> func = (CategoryNames cn) => delegate(GameObject lh, GameObject rh)
		{
			List<string> list3 = Wear.wear[cn].FirstOrDefault((List<string> list) => list.Contains(lh.name));
			List<string> list4 = Wear.wear[cn].FirstOrDefault((List<string> list) => list.Contains(rh.name));
			if (list3 == null || list4 == null)
			{
				return 0;
			}
			return (list3 == list4) ? (list3.IndexOf(lh.name) - list3.IndexOf(rh.name)) : (Wear.wear[cn].IndexOf(list3) - Wear.wear[cn].IndexOf(list4));
		};
		List<GameObject> list2 = new List<GameObject>();
		if (Storager.getInt("Training.NoviceArmorUsedKey", false) == 1 && !TrainingController.TrainingCompleted)
		{
			GameObject gameObject = Resources.Load<GameObject>("Armor_Info/Armor_Novice");
			if (gameObject != null)
			{
				list2.Add(gameObject);
			}
			else
			{
				Debug.LogError("No novice armor when Storager.getInt(Defs.NoviceArmorUsedKey,false) == 1 && !TrainingController.TrainingCompleted");
			}
		}
		else
		{
			foreach (ShopPositionParams item in armor)
			{
				FilterUpgradesArmor(list2, item.gameObject, CategoryNames.ArmorCategory, Defs.VisualArmor);
			}
		}
		list2.Sort(func(CategoryNames.ArmorCategory));
		return list2;
	}

	private void FilterUpgradesArmor(List<GameObject> modelsList, GameObject prefab, CategoryNames category, string visualDefName)
	{
		if (prefab.name.Replace("(Clone)", string.Empty) == "Armor_Novice")
		{
			return;
		}
		if (prefab != null && TempItemsController.PriceCoefs.ContainsKey(prefab.name) && TempItemsController.sharedController != null)
		{
			if (TempItemsController.sharedController.ContainsItem(prefab.name))
			{
				modelsList.Add(prefab);
			}
			return;
		}
		List<string> list = Wear.wear[category].FirstOrDefault((List<string> l) => l.Contains(prefab.name)).ToList();
		if (list == null)
		{
			return;
		}
		int num = list.IndexOf(prefab.name);
		if (Storager.getInt(prefab.name, true) > 0)
		{
			if (num == list.Count - 1)
			{
				modelsList.Add(prefab);
			}
			else if (num >= 0 && num < list.Count - 1 && Storager.getInt(list[num + 1], true) == 0)
			{
				modelsList.Add(prefab);
			}
			return;
		}
		if (num == 0 && Wear.LeagueForWear(prefab.name, category) <= (int)RatingSystem.instance.currentLeague)
		{
			modelsList.Add(prefab);
		}
		if (num > 0 && Storager.getInt(list[num - 1], true) > 0)
		{
			modelsList.Add(prefab);
		}
	}

	private ArmoryCell GetNewCell()
	{
		//Discarded unreachable code: IL_0017
		try
		{
			return UnityEngine.Object.Instantiate(Resources.Load<ArmoryCell>("ArmoryCell"));
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in AddCellToPool: " + ex);
		}
		return null;
	}

	public static void AddBoughtCurrency(string currency, int count)
	{
		if (currency == null)
		{
			Debug.LogWarning("AddBoughtCurrency: currency == null");
			return;
		}
		if (Debug.isDebugBuild)
		{
			Debug.Log(string.Format("<color=#ff00ffff>AddBoughtCurrency {0} {1}</color>", currency, count));
		}
		Storager.setInt("BoughtCurrency" + currency, Storager.getInt("BoughtCurrency" + currency, false) + count, false);
	}

	public static void SpendBoughtCurrency(string currency, int count)
	{
		if (currency == null)
		{
			Debug.LogWarning("SpendBoughtCurrency: currency == null");
		}
		else if (Debug.isDebugBuild)
		{
			Debug.Log(string.Format("<color=#ff00ffff>SpendBoughtCurrency {0} {1}</color>", currency, count));
		}
	}

	public static void TryToBuy(GameObject mainPanel, ItemPrice price, Action onSuccess, Action onFailure = null, Func<bool> successAdditionalCond = null, Action onReturnFromBank = null, Action onEnterCoinsShopAction = null, Action onExitCoinsShopAction = null)
	{
		Debug.Log("Trying to buy from ShopNGUIController...");
		if (BankController.Instance == null)
		{
			Debug.LogWarning("BankController.Instance == null");
			return;
		}
		if (price == null)
		{
			Debug.LogWarning("price == null");
			return;
		}
		EventHandler handleBackFromBank = null;
		handleBackFromBank = delegate
		{
			BankController.Instance.BackRequested -= handleBackFromBank;
			BankController.Instance.InterfaceEnabled = false;
			coinsShop.thisScript.notEnoughCurrency = null;
			if (mainPanel != null)
			{
				mainPanel.SetActive(true);
			}
			if (onReturnFromBank != null)
			{
				onReturnFromBank();
			}
			if (onExitCoinsShopAction != null)
			{
				onExitCoinsShopAction();
			}
		};
		EventHandler act = null;
		act = delegate
		{
			BankController.Instance.BackRequested -= act;
			mainPanel.SetActive(true);
			coinsShop.thisScript.notEnoughCurrency = null;
			coinsShop.thisScript.onReturnAction = null;
			int @int = Storager.getInt(price.Currency, false);
			int num = @int;
			num -= price.Price;
			bool flag = num >= 0;
			bool num2;
			if (successAdditionalCond != null)
			{
				if (successAdditionalCond())
				{
					goto IL_0088;
				}
				num2 = flag;
			}
			else
			{
				num2 = flag;
			}
			if (!num2)
			{
				if (onFailure != null)
				{
					onFailure();
				}
				coinsShop.thisScript.notEnoughCurrency = price.Currency;
				Debug.Log("Trying to display bank interface...");
				BankController.Instance.BackRequested += handleBackFromBank;
				BankController.Instance.InterfaceEnabled = true;
				mainPanel.SetActive(false);
				if (onEnterCoinsShopAction != null)
				{
					onEnterCoinsShopAction();
				}
				return;
			}
			goto IL_0088;
			IL_0088:
			Storager.setInt(price.Currency, num, false);
			SpendBoughtCurrency(price.Currency, price.Price);
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				PlayerPrefs.Save();
			}
			if (ABTestController.useBuffSystem)
			{
				BuffSystem.instance.OnSomethingPurchased();
			}
			if (onSuccess != null)
			{
				onSuccess();
			}
		};
		act(BankController.Instance, EventArgs.Empty);
	}

	private static void FilterWearUpgrades(string item, CategoryNames category, List<string> outputList)
	{
		if (item.Replace("(Clone)", string.Empty) == "Armor_Novice")
		{
			return;
		}
		if (item != null && TempItemsController.PriceCoefs.ContainsKey(item) && TempItemsController.sharedController != null)
		{
			if (TempItemsController.sharedController.ContainsItem(item))
			{
				outputList.Add(item);
			}
			return;
		}
		List<string> list = Wear.wear[category].FirstOrDefault((List<string> l) => l.Contains(item)).ToList();
		if (list == null)
		{
			return;
		}
		int num = list.IndexOf(item);
		if (Storager.getInt(item, true) > 0)
		{
			if (num == list.Count - 1)
			{
				outputList.Add(item);
			}
			else if (num >= 0 && num < list.Count - 1 && Storager.getInt(list[num + 1], true) == 0)
			{
				outputList.Add(item);
			}
		}
		else if (num == 0 && Wear.LeagueForWear(item, category) <= (int)RatingSystem.instance.currentLeague)
		{
			outputList.Add(item);
		}
	}

	public static bool ShowLockedFacebookSkin()
	{
		if (!FacebookController.FacebookSupported)
		{
			return false;
		}
		if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			return Storager.getInt("super_socialman", true) > 0;
		}
		return true;
	}

	public static List<ShopItem> GetItemNamesList(CategoryNames category)
	{
		Func<CategoryNames, Comparison<string>> func = (CategoryNames cn) => delegate(string leftWearItem, string rightWearItem)
		{
			List<string> list10 = Wear.wear[cn].FirstOrDefault((List<string> list) => list.Contains(leftWearItem));
			List<string> list11 = Wear.wear[cn].FirstOrDefault((List<string> list) => list.Contains(rightWearItem));
			if (list10 == null || list11 == null)
			{
				return 0;
			}
			return (list10 == list11) ? (list10.IndexOf(leftWearItem) - list10.IndexOf(rightWearItem)) : (Wear.wear[cn].IndexOf(list10) - Wear.wear[cn].IndexOf(list11));
		};
		List<ShopItem> list2 = new List<ShopItem>();
		if (IsBestCategory(category))
		{
			List<CategoryNames> categoriesOfThisBestCategory = СategoriesOfBestCategories[category];
			Func<ShopItem, bool> isBought = delegate(ShopItem shopItem)
			{
				if (IsWeaponCategory(shopItem.Category))
				{
					ItemRecord byTag = ItemDb.GetByTag(shopItem.Id);
					if (byTag != null && !byTag.StorageId.IsNullOrEmpty())
					{
						return Storager.getInt(byTag.StorageId, true) > 0;
					}
					return false;
				}
				if (IsGadgetsCategory(shopItem.Category))
				{
					return GadgetsInfo.IsBought(shopItem.Id);
				}
				return (shopItem.Category == CategoryNames.SkinsCategory) ? SkinsController.IsSkinBought(shopItem.Id) : (Storager.getInt(shopItem.Id, true) > 0);
			};
			Func<List<string>, List<string>, List<ShopItem>> func2 = delegate(List<string> src, List<string> excludeList)
			{
				//Discarded unreachable code: IL_00b9, IL_00e1
				try
				{
					List<ShopItem> list9 = new List<ShopItem>();
					for (int i = 0; i < categoriesOfThisBestCategory.Count; i++)
					{
						List<ShopItem> shopListOfThisCategory = GetItemNamesList(categoriesOfThisBestCategory[i]);
						IEnumerable<ShopItem> source = shopListOfThisCategory.Where((ShopItem shopItem) => src.Contains(shopItem.Id) && !excludeList.Contains(shopItem.Id) && !isBought(shopItem));
						List<ShopItem> second4 = source.OrderBy((ShopItem shopItem) => shopListOfThisCategory.IndexOf(shopItem)).ToList();
						list9 = list9.Concat(second4).ToList();
					}
					return list9;
				}
				catch (Exception ex3)
				{
					Debug.LogErrorFormat("Exception in filterSource: {0}", ex3);
					return new List<ShopItem>();
				}
			};
			List<ShopItem> list3 = func2(PromoActionsManager.sharedManager.news, new List<string>());
			List<ShopItem> second = func2(PromoActionsManager.sharedManager.topSellers, list3.Select((ShopItem shopItem) => shopItem.Id).ToList());
			List<ShopItem> second2 = BestItemsToRemoveOnLeave ?? new List<ShopItem>();
			return list3.Concat(second).Concat(second2).ToList();
		}
		if (IsWeaponCategory(category))
		{
			list2 = WeaponManager.sharedManager.FilteredShopListsNoUpgrades[(int)category].Select((GameObject p) => new ShopItem(ItemDb.GetByPrefabName(p.nameNoClone()).Tag, category)).ToList();
		}
		else
		{
			Dictionary<CategoryNames, List<string>> dictionary = new Dictionary<CategoryNames, List<string>>(4, CategoryNameComparer.Instance);
			dictionary.Add(CategoryNames.HatsCategory, hats.Select((ShopPositionParams item) => item.nameNoClone()).ToList());
			dictionary.Add(CategoryNames.MaskCategory, masks.Select((ShopPositionParams item) => item.nameNoClone()).ToList());
			dictionary.Add(CategoryNames.BootsCategory, boots.Select((ShopPositionParams item) => item.nameNoClone()).ToList());
			dictionary.Add(CategoryNames.CapesCategory, capes.Select((ShopPositionParams item) => item.nameNoClone()).ToList());
			Dictionary<CategoryNames, List<string>> dictionary2 = dictionary;
			switch (MapShopCategoryToItemCategory(category))
			{
			case CategoryNames.SkinsCategory:
				switch (category)
				{
				case CategoryNames.LeagueSkinsCategory:
					try
					{
						IEnumerable<ShopItem> collection2 = SkinsController.leagueSkinsIds.Select((string skinId) => new ShopItem(skinId, CategoryNames.SkinsCategory));
						list2.AddRange(collection2);
					}
					catch (Exception ex2)
					{
						Debug.LogError("Exception in filling league skins: " + ex2);
					}
					break;
				case CategoryNames.SkinsCategory:
				{
					IEnumerable<string> second3 = from item in SkinsController.sharedController.skinItems
						where item.category != SkinItem.CategoryNames.LeagueSkinsCategory
						orderby item.category
						select item.name;
					List<string> list5 = new List<string>();
					list5.Add("CustomSkinID");
					IEnumerable<ShopItem> collection = from skinId in list5.Concat(CustomSkinsReverse()).Concat(second3)
						select new ShopItem(skinId, CategoryNames.SkinsCategory);
					list2.AddRange(collection);
					break;
				}
				default:
					Debug.LogErrorFormat("Unknown category: {0}", category);
					break;
				}
				if (!ShowLockedFacebookSkin())
				{
					list2.RemoveAll((ShopItem shopItem) => shopItem.Id == "super_socialman");
				}
				break;
			case CategoryNames.HatsCategory:
			case CategoryNames.CapesCategory:
			case CategoryNames.BootsCategory:
			case CategoryNames.MaskCategory:
			{
				IEnumerable<string> enumerable2 = Wear.LeagueItems().SelectMany((KeyValuePair<Wear.LeagueItemState, List<string>> kvp) => kvp.Value).Distinct();
				if (category != MapShopCategoryToItemCategory(category))
				{
					List<string> list7 = enumerable2.ToList();
					list7.Sort(func(MapShopCategoryToItemCategory(category)));
					list2.AddRange(list7.Select((string itemId) => new ShopItem(itemId, MapShopCategoryToItemCategory(category))));
					break;
				}
				IEnumerable<string> enumerable3 = dictionary2[MapShopCategoryToItemCategory(category)].Except(enumerable2);
				List<string> list8 = new List<string>();
				foreach (string item in enumerable3)
				{
					FilterWearUpgrades(item, MapShopCategoryToItemCategory(category), list8);
				}
				list8.Sort(func(MapShopCategoryToItemCategory(category)));
				list2.AddRange(list8.Select((string itemId) => new ShopItem(itemId, MapShopCategoryToItemCategory(category))));
				break;
			}
			case CategoryNames.LeagueWeaponSkinsCategory:
				list2.AddRange(WeaponSkinsManager.AllSkins.Select((WeaponSkin skin) => new ShopItem(skin.Id, CategoryNames.LeagueWeaponSkinsCategory)));
				break;
			case CategoryNames.PetsCategory:
			{
				IEnumerable<ShopItem> collection3 = from petOrEmptySlotId in Singleton<PetsManager>.Instance.PlayerPetIdsAndEmptySlots()
					select new ShopItem(petOrEmptySlotId, CategoryNames.PetsCategory);
				list2.AddRange(collection3);
				break;
			}
			case CategoryNames.EggsCategory:
			{
				List<Egg> list6 = new List<Egg>(Singleton<EggsManager>.Instance.GetPlayerEggsInIncubator());
				list6.Sort(delegate(Egg egg1, Egg egg2)
				{
					if (egg1.Data.Rare == EggRarity.Champion && egg2.Data.Rare == EggRarity.Champion)
					{
						return 0;
					}
					if (egg1.Data.Rare == EggRarity.Champion)
					{
						return -1;
					}
					if (egg2.Data.Rare == EggRarity.Champion)
					{
						return 1;
					}
					List<Egg> playerEggsInIncubator = Singleton<EggsManager>.Instance.GetPlayerEggsInIncubator();
					return playerEggsInIncubator.IndexOf(egg2).CompareTo(playerEggsInIncubator.IndexOf(egg1));
				});
				list2.AddRange(list6.Select((Egg egg) => new ShopItem(egg.Id.ToString(), CategoryNames.EggsCategory)));
				break;
			}
			case CategoryNames.ThrowingCategory:
			case CategoryNames.ToolsCategoty:
			case CategoryNames.SupportCategory:
			{
				IEnumerable<List<string>> enumerable = GadgetsInfo.UpgradeChains.Where((List<string> chain) => GadgetsInfo.info[chain[0]].Category == (GadgetInfo.GadgetCategory)category);
				List<string> list4 = new List<string>();
				foreach (List<string> item2 in enumerable)
				{
					string text = GadgetsInfo.LastBoughtFor(item2[0]);
					if (text != null)
					{
						list4.Add(text);
						continue;
					}
					string text2 = GadgetsInfo.FirstForOurTier(item2[0]);
					if (GadgetsInfo.info[text2].Tier <= ExpController.OurTierForAnyPlace())
					{
						list4.Add(text2);
					}
				}
				try
				{
					Func<string, int> getGadgetsTier = delegate(string gadgetId)
					{
						string key = GadgetsInfo.Upgrades[gadgetId][0];
						return GadgetsInfo.info[key].Tier;
					};
					list4 = list4.OrderBy((string gadgetId) => getGadgetsTier(gadgetId)).ToList();
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Exception in sorting gadgets in shop: {0}", ex);
				}
				list2.AddRange(list4.Select((string gadgetId) => new ShopItem(gadgetId, MapShopCategoryToItemCategory(category))));
				break;
			}
			}
		}
		return list2;
	}

	private static IEnumerable<string> CustomSkinsReverse()
	{
		long result;
		return from x in SkinsController.CustomSkinIds()
			orderby long.TryParse(x, out result) ? result : 0 descending
			select x;
	}

	private static string GetWeaponStatText(int currentValue, int nextValue)
	{
		if (nextValue - currentValue == 0)
		{
			return currentValue.ToString();
		}
		if (nextValue - currentValue > 0)
		{
			return string.Format("{0}[00ff00]+{1}[-]", currentValue, nextValue - currentValue);
		}
		return string.Format("{0}[FACC2E]{1}[-]", currentValue, nextValue - currentValue);
	}

	public void SetCamera()
	{
		Transform child = characterPoint.GetChild(0);
		HOTween.Kill(child);
		Vector3 vector = new Vector3(0f, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, 0f, 0f);
		Vector3 vector3 = new Vector3(1f, 1f, 1f);
		CategoryNames categoryNames = CurrentCategory;
		if (CurrentItem != null)
		{
			categoryNames = CurrentItem.Category;
		}
		switch (categoryNames)
		{
		case CategoryNames.CapesCategory:
			vector = new Vector3(0f, 0f, 0f);
			vector2 = new Vector3(0f, -130.76f, 0f);
			break;
		case CategoryNames.HatsCategory:
			vector = new Vector3(1.06f, -0.54f, 2.19f);
			vector2 = new Vector3(0f, -9.5f, 0f);
			break;
		case CategoryNames.MaskCategory:
			vector = new Vector3(1.06f, -0.54f, 2.19f);
			vector2 = new Vector3(0f, -9.5f, 0f);
			break;
		default:
			vector = new Vector3(0f, 0f, 0f);
			vector2 = new Vector3(0f, 0f, 0f);
			break;
		}
		float p_duration = 0.5f;
		idleTimerLastTime = Time.realtimeSinceStartup;
		HOTween.To(child, p_duration, new TweenParms().Prop("localPosition", vector).Prop("localRotation", new PlugQuaternion(vector2)).UpdateType(UpdateType.TimeScaleIndependentUpdate)
			.Ease(EaseType.Linear)
			.OnComplete((TweenDelegate.TweenCallback)delegate
			{
				idleTimerLastTime = Time.realtimeSinceStartup;
			}));
	}

	public static bool IsModeWithNormalTimeScaleInShop()
	{
		bool flag = !Defs.IsSurvival && TrainingController.TrainingCompleted && !Defs.isMulti;
		return !Defs.IsSurvival && !flag;
	}

	private void PlayPetAnimation()
	{
		try
		{
			if (characterInterface != null && characterInterface.myPet != null && !IsModeWithNormalTimeScaleInShop())
			{
				Animation component = characterInterface.myPet.GetComponent<PetEngine>().Model.GetComponent<Animation>();
				StopPetAnimation();
				if (component.GetClip("Profile") == null)
				{
					Debug.LogErrorFormat("Error: pet {0} has no Profile animation clip", characterInterface.myPet.nameNoClone());
				}
				else if (petProfileAnimationRunner.gameObject.activeInHierarchy)
				{
					petProfileAnimationRunner.StartPlay(component, "Profile", false, null);
				}
				else
				{
					Debug.LogErrorFormat("Coroutine couldn't be started because the the game object 'AnimationCoroutineRunner' is inactive! (Pet)");
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in PlayPetAnimation: {0}", ex);
		}
	}

	public void PlayWeaponAnimation()
	{
		if (profile != null && weapon != null && weapon.GetComponent<WeaponSounds>() != null)
		{
			Animation component = weapon.GetComponent<WeaponSounds>().animationObject.GetComponent<Animation>();
			if (IsModeWithNormalTimeScaleInShop())
			{
				if (component.GetClip("Profile") == null)
				{
					component.AddClip(profile, "Profile");
				}
				component.Play("Profile");
			}
			else
			{
				animationCoroutineRunner.StopAllCoroutines();
				if (component.GetClip("Profile") == null)
				{
					component.AddClip(profile, "Profile");
				}
				if (animationCoroutineRunner.gameObject.activeInHierarchy)
				{
					animationCoroutineRunner.StartPlay(component, "Profile", false, null);
				}
				else
				{
					Debug.LogWarning("Coroutine couldn't be started because the the game object 'AnimationCoroutineRunner' is inactive!");
				}
			}
		}
		MainMenu_Pers.GetComponent<Animation>().Stop();
		MainMenu_Pers.GetComponent<Animation>().Play("Idle");
	}

	public static Texture TextureForEquippedWeaponOrWear(int cc)
	{
		string str = (IsWeaponCategory((CategoryNames)cc) ? _CurrentWeaponSetIDs()[cc] : (IsWearCategory((CategoryNames)cc) ? sharedShop.WearForCat((CategoryNames)cc) : (IsGadgetsCategory((CategoryNames)cc) ? GadgetsInfo.EquippedForCategory((GadgetInfo.GadgetCategory)cc) : ((cc != 25000) ? "potion" : Singleton<PetsManager>.Instance.GetEqipedPetId()))));
		if (str.IsNullOrEmpty())
		{
			return null;
		}
		return ItemDb.GetItemIcon(str, (CategoryNames)cc);
	}

	public void SetIconModelsPositions(Transform t, CategoryNames c)
	{
		switch (MapShopCategoryToItemCategory(c))
		{
		case CategoryNames.ArmorCategory:
		{
			t.transform.localPosition = new Vector3(0f, 0f, 0f);
			t.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			float num5 = 1f;
			t.transform.localScale = new Vector3(num5, num5, num5);
			break;
		}
		case CategoryNames.BootsCategory:
		{
			t.transform.localPosition = new Vector3(-0.4f, -0.1f, 0f);
			t.transform.localRotation = Quaternion.Euler(new Vector3(13f, 149f, 0f));
			float num4 = 75f;
			t.transform.localScale = new Vector3(num4, num4, num4);
			break;
		}
		case CategoryNames.CapesCategory:
		{
			t.transform.localPosition = new Vector3(-0.720093f, 5.35f, 0f);
			t.transform.localRotation = Quaternion.Euler(new Vector3(8f, -25f, 0f));
			float num3 = 60f;
			t.transform.localScale = new Vector3(num3, num3, num3);
			break;
		}
		case CategoryNames.SkinsCategory:
			SkinsController.SetTransformParamtersForSkinModel(t);
			break;
		case CategoryNames.GearCategory:
		{
			t.transform.localPosition = new Vector3(4.648193f, 2.444565f, 0f);
			t.transform.localRotation = Quaternion.Euler(new Vector3(8f, -25f, 0f));
			float num2 = 319.3023f;
			t.transform.localScale = new Vector3(num2, num2, num2);
			break;
		}
		case CategoryNames.HatsCategory:
		{
			t.transform.localPosition = new Vector3(-0.62f, -0.04f, 0f);
			t.transform.localRotation = Quaternion.Euler(new Vector3(-75f, -165f, -90f));
			float num = 82.5f;
			t.transform.localScale = new Vector3(num, num, num);
			break;
		}
		}
	}

	private void DisableGunflashes(GameObject root)
	{
		if (root == null)
		{
			return;
		}
		if (root.name.Equals("GunFlash"))
		{
			root.SetActive(false);
		}
		foreach (Transform item in root.transform)
		{
			if (!(null == item))
			{
				DisableGunflashes(item.gameObject);
			}
		}
	}

	public void UpdateIcons(bool animateModel = false)
	{
		Enum.GetValues(typeof(CategoryNames)).OfType<CategoryNames>().ForEach(delegate(CategoryNames cat)
		{
			UpdateIcon(cat, animateModel);
		});
	}

	public void UpdateIcon(CategoryNames category, bool animateModel = false)
	{
		if (category == CategoryNames.GearCategory || category == CategoryNames.EggsCategory || category == CategoryNames.SkinsCategoryMale || category == CategoryNames.SkinsCategoryFemale || category == CategoryNames.SkinsCategorySpecial || category == CategoryNames.SkinsCategoryPremium || category == CategoryNames.SkinsCategoryEditor || category == CategoryNames.BestWeapons || category == CategoryNames.BestWear || category == CategoryNames.BestGadgets)
		{
			return;
		}
		ShopCategoryButton shopCategoryButton = TransformOfButtonForCategory(category).GetComponent<ShopCategoryButton>();
		Action<Texture> action = delegate(Texture texture)
		{
			shopCategoryButton.icon.mainTexture = texture;
		};
		if (category == CategoryNames.LeagueWeaponSkinsCategory)
		{
			action(shopCategoryButton.icon.mainTexture);
			return;
		}
		string capeName = string.Empty;
		try
		{
			capeName = Storager.getString(Defs.CapeEquppedSN, false);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception getting cape in UpdateIcon: " + ex);
		}
		if (category == CategoryNames.CapesCategory && shopCategoryButton.modelPoint != null)
		{
			shopCategoryButton.modelPoint.DestroyChildren();
		}
		if (category == CategoryNames.SkinsCategory || category == CategoryNames.LeagueSkinsCategory)
		{
			try
			{
				Tools.SetTextureRecursivelyFrom(shopCategoryButton.modelPoint.gameObject, SkinsController.currentSkinForPers, new GameObject[0]);
				HOTween.Kill(shopCategoryButton.modelPoint);
				shopCategoryButton.modelPoint.localScale = Vector3.one;
				return;
			}
			catch (Exception ex2)
			{
				Debug.LogErrorFormat("Exception in UpdateIcon, updating skin icon: {0}", ex2);
				return;
			}
		}
		if (category == CategoryNames.CapesCategory && capeName == "cape_Custom")
		{
			if (shopCategoryButton.icon != null)
			{
				shopCategoryButton.icon.mainTexture = null;
			}
			if (shopCategoryButton.emptyIcon != null)
			{
				shopCategoryButton.emptyIcon.SetActive(false);
			}
			GameObject wearFromResources = ItemDb.GetWearFromResources("cape_Custom", CategoryNames.CapesCategory);
			AddModel(wearFromResources, delegate(GameObject manipulateObject, Vector3 positionShop, Vector3 rotationShop, string readableName, float sc, int _unusedTier, int _unusedLeague)
			{
				manipulateObject.transform.parent = shopCategoryButton.modelPoint;
				float num = 0.5f;
				Transform transform = manipulateObject.transform;
				transform.localPosition = shopCategoryButton.modelPoint.localPosition + positionShop * num;
				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
				transform.Rotate(rotationShop, Space.World);
				transform.localScale = new Vector3(sc * num, sc * num, sc * num);
				if (category == CategoryNames.CapesCategory && capeName == "cape_Custom" && SkinsController.capeUserTexture != null)
				{
					Player_move_c.SetTextureRecursivelyFrom(manipulateObject, SkinsController.capeUserTexture, new GameObject[0]);
				}
				SetIconModelsPositions(transform, category);
			}, MapShopCategoryToItemCategory(category));
		}
		else
		{
			Texture texture2 = TextureForEquippedWeaponOrWear((int)MapShopCategoryToItemCategory(category));
			if (shopCategoryButton.emptyIcon != null)
			{
				shopCategoryButton.emptyIcon.SetActive(texture2 == null);
			}
			if (texture2 != null)
			{
				action(texture2);
			}
			else if (shopCategoryButton.icon != null)
			{
				shopCategoryButton.icon.mainTexture = null;
			}
		}
	}

	public void EquipWear(string tg)
	{
		EquipWearInCategory(tg, CurrentItem.Category, inGame);
	}

	public static void EquipWearInCategoryIfNotEquiped(string tg, CategoryNames cat, bool inGameLocal)
	{
		if (!Storager.hasKey(SnForWearCategory(cat)))
		{
			Storager.setString(SnForWearCategory(cat), NoneEquippedForWearCategory(cat), false);
		}
		if (!Storager.getString(SnForWearCategory(cat), false).Equals(tg))
		{
			EquipWearInCategory(tg, cat, inGameLocal);
		}
	}

	public static void SendEquippedWearInCategory(string equippedItem, CategoryNames cat, string previousItem)
	{
		Action<string, CategoryNames, string> equippedWearInCategory = ShopNGUIController.EquippedWearInCategory;
		if (equippedWearInCategory != null)
		{
			equippedWearInCategory(equippedItem ?? string.Empty, cat, previousItem ?? string.Empty);
		}
	}

	private static void EquipWearInCategory(string tg, CategoryNames cat, bool inGameLocal)
	{
		bool flag = !Defs.isMulti;
		string @string = Storager.getString(SnForWearCategory(cat), false);
		Player_move_c player_move_c = null;
		if (inGameLocal)
		{
			if (flag)
			{
				if (!SceneLoader.ActiveSceneName.Equals("LevelComplete") && !SceneLoader.ActiveSceneName.Equals("ChooseLevel"))
				{
					player_move_c = GameObject.FindGameObjectWithTag("Player").GetComponent<SkinName>().playerMoveC;
				}
			}
			else if (WeaponManager.sharedManager.myPlayer != null)
			{
				player_move_c = WeaponManager.sharedManager.myPlayerMoveC;
			}
		}
		SetAsEquippedAndSendToServer(tg, cat);
		if (sharedShop.wearEquipAction != null)
		{
			sharedShop.wearEquipAction(cat, @string ?? NoneEquippedForWearCategory(cat), sharedShop.WearForCat(cat));
		}
		if (cat == CategoryNames.BootsCategory && inGameLocal && player_move_c != null)
		{
			if (!@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.bootsMethods.ContainsKey(@string))
			{
				Wear.bootsMethods[@string].Value(player_move_c, new Dictionary<string, object>());
			}
			if (Wear.bootsMethods.ContainsKey(tg))
			{
				Wear.bootsMethods[tg].Key(player_move_c, new Dictionary<string, object>());
			}
		}
		if (cat == CategoryNames.CapesCategory && inGameLocal && player_move_c != null)
		{
			if (!@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.capesMethods.ContainsKey(@string))
			{
				Wear.capesMethods[@string].Value(player_move_c, new Dictionary<string, object>());
			}
			if (Wear.capesMethods.ContainsKey(tg))
			{
				Wear.capesMethods[tg].Key(player_move_c, new Dictionary<string, object>());
			}
		}
		if (cat == CategoryNames.HatsCategory && inGameLocal && player_move_c != null)
		{
			if (!@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.hatsMethods.ContainsKey(@string))
			{
				Wear.hatsMethods[@string].Value(player_move_c, new Dictionary<string, object>());
			}
			if (Wear.hatsMethods.ContainsKey(tg))
			{
				Wear.hatsMethods[tg].Key(player_move_c, new Dictionary<string, object>());
			}
		}
		if (cat == CategoryNames.ArmorCategory && inGameLocal && player_move_c != null)
		{
			if (!@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.armorMethods.ContainsKey(@string))
			{
				Wear.armorMethods[@string].Value(player_move_c, new Dictionary<string, object>());
			}
			if (Wear.armorMethods.ContainsKey(tg))
			{
				Wear.armorMethods[tg].Key(player_move_c, new Dictionary<string, object>());
			}
		}
		if (GuiActive)
		{
			sharedShop.UpdateButtons();
			sharedShop.UpdateIcons(true);
		}
		SendEquippedWearInCategory(tg, cat, @string);
	}

	public static void UnequipCurrentWearInCategory(CategoryNames cat, bool inGameLocal)
	{
		bool flag = !Defs.isMulti;
		string @string = Storager.getString(SnForWearCategory(cat), false);
		Player_move_c player_move_c = null;
		if (inGameLocal)
		{
			if (flag)
			{
				if (!SceneLoader.ActiveSceneName.Equals("LevelComplete") && !SceneLoader.ActiveSceneName.Equals("ChooseLevel"))
				{
					player_move_c = GameObject.FindGameObjectWithTag("Player").GetComponent<SkinName>().playerMoveC;
				}
			}
			else if (WeaponManager.sharedManager.myPlayer != null)
			{
				player_move_c = WeaponManager.sharedManager.myPlayerMoveC;
			}
		}
		Storager.setString(SnForWearCategory(cat), NoneEquippedForWearCategory(cat), false);
		FriendsController.sharedController.SendAccessories();
		if (sharedShop.wearEquipAction != null)
		{
			sharedShop.wearEquipAction(cat, @string ?? NoneEquippedForWearCategory(cat), NoneEquippedForWearCategory(cat));
		}
		if (cat == CategoryNames.BootsCategory && inGameLocal && player_move_c != null && !@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.bootsMethods.ContainsKey(@string))
		{
			Wear.bootsMethods[@string].Value(player_move_c, new Dictionary<string, object>());
		}
		if (cat == CategoryNames.CapesCategory && inGameLocal && player_move_c != null && !@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.capesMethods.ContainsKey(@string))
		{
			Wear.capesMethods[@string].Value(player_move_c, new Dictionary<string, object>());
		}
		if (cat == CategoryNames.HatsCategory && inGameLocal && player_move_c != null && !@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.hatsMethods.ContainsKey(@string))
		{
			Wear.hatsMethods[@string].Value(player_move_c, new Dictionary<string, object>());
		}
		if (cat == CategoryNames.ArmorCategory && inGameLocal && player_move_c != null && !@string.Equals(NoneEquippedForWearCategory(cat)) && Wear.armorMethods.ContainsKey(@string))
		{
			Wear.armorMethods[@string].Value(player_move_c, new Dictionary<string, object>());
		}
		if (sharedShop.wearUnequipAction != null)
		{
			sharedShop.wearUnequipAction(cat, @string ?? NoneEquippedForWearCategory(cat));
		}
		if (GuiActive)
		{
			sharedShop.UpdateIcons(false);
		}
		Action<CategoryNames, string> unequippedWearInCategory = ShopNGUIController.UnequippedWearInCategory;
		if (unequippedWearInCategory != null)
		{
			unequippedWearInCategory(cat, @string ?? string.Empty);
		}
	}

	public static void ShowTryGunIfPossible(bool placeForGiveNewTryGun, Transform point, string layer, Action<string> onPurchase = null, Action onEnterCoinsShopAdditional = null, Action onExitoinsShopAdditional = null, Action<string> customEquipWearAction = null)
	{
		if (!Defs.isHunger && !Defs.isDuel && !placeForGiveNewTryGun && WeaponManager.sharedManager != null && WeaponManager.sharedManager.ExpiredTryGuns.Count > 0 && TrainingController.TrainingCompleted)
		{
			{
				string tg;
				foreach (string expiredTryGun in WeaponManager.sharedManager.ExpiredTryGuns)
				{
					tg = expiredTryGun;
					try
					{
						if (WeaponManager.sharedManager.weaponsInGame.FirstOrDefault((UnityEngine.Object w) => ItemDb.GetByPrefabName(w.name).Tag == tg) != null)
						{
							WeaponManager.sharedManager.ExpiredTryGuns.RemoveAll((string t) => t == tg);
							if (WeaponManager.LastBoughtTag(tg) == null)
							{
								ShowAddTryGun(tg, point, layer, onPurchase, onEnterCoinsShopAdditional, onExitoinsShopAdditional, customEquipWearAction, true);
								break;
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("Exception in foreach (var tg in WeaponManager.sharedManager.ExpiredTryGuns): " + ex);
					}
				}
				return;
			}
		}
		if (Defs.isHunger || Defs.isDuel || Defs.isDaterRegim || WeaponManager.sharedManager._currentFilterMap != 0 || !((!ABTestController.useBuffSystem) ? KillRateCheck.instance.giveWeapon : BuffSystem.instance.giveTryGun) || !TrainingController.TrainingCompleted)
		{
			return;
		}
		try
		{
			int maximumCoinBank = UpperCoinsBankBound();
			List<ItemRecord> source = (from prefabName in WeaponManager.tryGunsTable.SelectMany((KeyValuePair<CategoryNames, List<List<string>>> kvp) => kvp.Value[ExpController.OurTierForAnyPlace()])
				select ItemDb.GetByPrefabName(prefabName) into rec
				where rec.StorageId != null && Storager.getInt(rec.StorageId, true) == 0
				where !WeaponManager.sharedManager.IsAvailableTryGun(rec.Tag) && !WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(rec.Tag)
				select rec).ToList();
			List<ItemRecord> source2 = (from rec in source
				where rec.Price.Currency == "Coins"
				where PriceIfGunWillBeTryGun(rec.Tag) > maximumCoinBank
				select rec).Randomize().ToList();
			string text = null;
			if (source2.Any())
			{
				text = source2.First().Tag;
			}
			else
			{
				int maximumGemBank = UpperGemsBankBound();
				List<ItemRecord> source3 = (from rec in source
					where rec.Price.Currency == "GemsCurrency"
					where PriceIfGunWillBeTryGun(rec.Tag) > maximumGemBank
					select rec).Randomize().ToList();
				text = ((!source3.Any()) ? TryGunForCategoryWithMaxUnbought() : source3.First().Tag);
			}
			if (text != null)
			{
				ShowAddTryGun(text, point, layer, onPurchase, onEnterCoinsShopAdditional, onExitoinsShopAdditional, customEquipWearAction);
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError("Exception in giving: " + ex2);
		}
	}

	private static void AnimateScaleForTransform(Transform t)
	{
		Vector3 localScale = t.localScale;
		t.localScale *= 1.25f;
		HOTween.To(t, 0.25f, new TweenParms().Prop("localScale", localScale).UpdateType(UpdateType.TimeScaleIndependentUpdate).Ease(EaseType.Linear));
	}

	private static int UpperCoinsBankBound()
	{
		int value = ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
		value = Mathf.Clamp(value, 0, ExperienceController.addCoinsFromLevels.Length - 1);
		return Storager.getInt("Coins", false) + 30 + ExperienceController.addCoinsFromLevels[value];
	}

	private static int UpperGemsBankBound()
	{
		int value = ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
		value = Mathf.Clamp(value, 0, ExperienceController.addGemsFromLevels.Length - 1);
		return Storager.getInt("GemsCurrency", false) + ExperienceController.addGemsFromLevels[value];
	}

	private static string TryGunForCategoryWithMaxUnbought()
	{
		List<CategoryNames> list = new List<CategoryNames>();
		list.Add(CategoryNames.PrimaryCategory);
		list.Add(CategoryNames.BackupCategory);
		list.Add(CategoryNames.MeleeCategory);
		list.Add(CategoryNames.SpecilCategory);
		list.Add(CategoryNames.SniperCategory);
		list.Add(CategoryNames.PremiumCategory);
		List<CategoryNames> list2 = list.Randomize().OrderBy(delegate(CategoryNames cat)
		{
			List<WeaponSounds> list4 = (from w in WeaponManager.sharedManager.weaponsInGame
				select ((GameObject)w).GetComponent<WeaponSounds>() into ws
				where ws.categoryNabor - 1 == (int)cat && ws.tier == ExpController.OurTierForAnyPlace()
				where WeaponManager.tagToStoreIDMapping.ContainsKey(ItemDb.GetByPrefabName(ws.name).Tag) && WeaponManager.storeIDtoDefsSNMapping.ContainsKey(WeaponManager.tagToStoreIDMapping[ItemDb.GetByPrefabName(ws.name).Tag]) && Storager.getInt(WeaponManager.storeIDtoDefsSNMapping[WeaponManager.tagToStoreIDMapping[ItemDb.GetByPrefabName(ws.name).Tag]], true) == 1
				select ws).ToList();
			return list4.Count;
		}).ToList();
		string result = null;
		for (int i = 0; i < list2.Count; i++)
		{
			CategoryNames cat2 = list2[i];
			List<WeaponSounds> source = (from ws in (from w in WeaponManager.sharedManager.weaponsInGame
					select ((GameObject)w).GetComponent<WeaponSounds>() into ws
					where ws.categoryNabor - 1 == (int)cat2 && ws.tier == ExpController.OurTierForAnyPlace()
					select ws).Where(delegate(WeaponSounds ws)
				{
					List<string> list3 = WeaponUpgrades.ChainForTag(ItemDb.GetByPrefabName(ws.name).Tag);
					return list3 == null || (list3.Count > 0 && list3[0] == ItemDb.GetByPrefabName(ws.name).Tag);
				})
				where WeaponManager.tagToStoreIDMapping.ContainsKey(ItemDb.GetByPrefabName(ws.name).Tag) && WeaponManager.storeIDtoDefsSNMapping.ContainsKey(WeaponManager.tagToStoreIDMapping[ItemDb.GetByPrefabName(ws.name).Tag]) && Storager.getInt(WeaponManager.storeIDtoDefsSNMapping[WeaponManager.tagToStoreIDMapping[ItemDb.GetByPrefabName(ws.name).Tag]], true) == 0
				where WeaponManager.tryGunsTable[cat2][ExpController.OurTierForAnyPlace()].Contains(ItemDb.GetByTag(ItemDb.GetByPrefabName(ws.name).Tag).PrefabName)
				where !WeaponManager.sharedManager.IsAvailableTryGun(ItemDb.GetByPrefabName(ws.name).Tag) && !WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(ItemDb.GetByPrefabName(ws.name).Tag)
				select ws).Randomize().ToList();
			if (source.Count() != 0)
			{
				result = ItemDb.GetByPrefabName(source.First().name).Tag;
				break;
			}
		}
		return result;
	}

	public static bool ShowPremimAccountExpiredIfPossible(Transform point, string layer, string header = "", bool showOnlyIfExpired = true)
	{
		if (showOnlyIfExpired && (!PremiumAccountController.AccountHasExpired || !Defs2.CanShowPremiumAccountExpiredWindow))
		{
			return false;
		}
		if (point != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("PremiumAccount"));
			gameObject.transform.parent = point;
			Player_move_c.SetLayerRecursively(gameObject, LayerMask.NameToLayer(layer ?? "Default"));
			gameObject.transform.localPosition = new Vector3(0f, 0f, -130f);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			PremiumAccountScreenController component = gameObject.GetComponent<PremiumAccountScreenController>();
			component.Header = header;
			PremiumAccountController.AccountHasExpired = false;
			return true;
		}
		return false;
	}

	public static void GiveArmorArmy1OrNoviceArmor()
	{
		ProvideItem(CategoryNames.ArmorCategory, (Storager.getInt("Training.NoviceArmorUsedKey", false) != 1) ? "Armor_Army_1" : "Armor_Novice", 1, false, 0, null, null, true, Storager.getInt("Training.NoviceArmorUsedKey", false) == 1, false);
	}

	public static void SetEggInfoHintViewed()
	{
		Storager.setInt("Shop.Tutorial.KEY_TUTORIAL_EGG_INFO_HINT_VIEWED", 1, false);
	}

	private void OnTrainingCompleted_4_4_Sett_Changed()
	{
		if (Storager.getInt(Defs.TrainingCompleted_4_4_Sett, false) > 0)
		{
			if (_tutorialCurrentState != null)
			{
				_tutorialCurrentState.StageAct(TutorialStageTrigger.Exit);
				_tutorialCurrentState = null;
			}
			_tutorialHintsContainer.gameObject.SetActive(false);
			backButton.isEnabled = true;
			unlockButton.isEnabled = true;
		}
	}

	private void UpdateTutorialState()
	{
		if (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
		{
			UpdateExtTutorial();
			return;
		}
		if (Storager.getInt(Defs.TrainingCompleted_4_4_Sett, false) > 0)
		{
			OnTrainingCompleted_4_4_Sett_Changed();
			return;
		}
		if (!_tutorialStates.Any())
		{
			_tutorialStates.Add(new TutorialState(TutorialPhase.SelectWeaponCategory, TutorialSelectWeaponCategory));
			_tutorialStates.Add(new TutorialState(TutorialPhase.SelectSniperSection, TutorialSelectSniperSection));
			_tutorialStates.Add(new TutorialState(TutorialPhase.SelectRifle, TutorialSelectRifle));
			_tutorialStates.Add(new TutorialState(TutorialPhase.EquipRifle, TutorialEquipRifle));
			_tutorialStates.Add(new TutorialState(TutorialPhase.SelectWearCategory, TutorialSelectWearCategory));
			_tutorialStates.Add(new TutorialState(TutorialPhase.EquipArmor, TutorialEquipArmor));
			_tutorialStates.Add(new TutorialState(TutorialPhase.SelectArmorSection, TutorialSelectArmorSection));
			_tutorialStates.Add(new TutorialState(TutorialPhase.SelectPetsCategory, TutorialSelectPetsCategory));
			_tutorialStates.Add(new TutorialState(TutorialPhase.ShowEggsHint, TutorialShowEggsHint));
			_tutorialStates.Add(new TutorialState(TutorialPhase.LeaveArmory, TutorialLeaveArmory));
		}
		TutorialPhase tutorialPhase = ((_tutorialCurrentState == null) ? TutorialPhasePassed : _tutorialCurrentState.ForStage);
		if (tutorialPhase < TutorialPhase.SelectWearCategory)
		{
			if (WeaponManager.sharedManager == null)
			{
				return;
			}
			for (int i = 0; i < WeaponManager.sharedManager.playerWeapons.Count; i++)
			{
				string text = ItemDb.GetByPrefabName((WeaponManager.sharedManager.playerWeapons[i] as Weapon).weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag;
				if (text == WeaponTags.HunterRifleTag)
				{
					TutorialPhasePassed = TutorialPhase.SelectWearCategory;
					tutorialPhase = TutorialPhase.SelectWearCategory;
					break;
				}
			}
		}
		else if (Storager.getString(Defs.ArmorNewEquppedSN, false) != Defs.ArmorNewNoneEqupped && TutorialPhasePassed < TutorialPhase.SelectPetsCategory)
		{
			TutorialPhasePassed = TutorialPhase.SelectPetsCategory;
			tutorialPhase = TutorialPhase.SelectPetsCategory;
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Equip_Armor);
		}
		TutorialPhase toPhase = tutorialPhase;
		backButton.isEnabled = tutorialPhase >= TutorialPhase.LeaveArmory;
		unlockButton.isEnabled = tutorialPhase >= TutorialPhase.LeaveArmory;
		switch (tutorialPhase)
		{
		case TutorialPhase.SelectWeaponCategory:
			toPhase = ((superCategoriesButtonController.currentBtnName == "Weapons") ? TutorialPhase.SelectSniperSection : TutorialPhase.SelectWeaponCategory);
			break;
		case TutorialPhase.SelectSniperSection:
			toPhase = ((!(superCategoriesButtonController.currentBtnName != "Weapons")) ? ((CurrentCategory != CategoryNames.SniperCategory) ? TutorialPhase.SelectSniperSection : TutorialPhase.SelectRifle) : TutorialPhase.SelectWeaponCategory);
			break;
		case TutorialPhase.SelectRifle:
			toPhase = ((!(superCategoriesButtonController.currentBtnName != "Weapons")) ? ((CurrentCategory != CategoryNames.SniperCategory) ? TutorialPhase.SelectSniperSection : ((!(CurrentItem.Id != WeaponTags.HunterRifleTag)) ? TutorialPhase.EquipRifle : TutorialPhase.SelectRifle)) : TutorialPhase.SelectWeaponCategory);
			break;
		case TutorialPhase.EquipRifle:
		{
			if (superCategoriesButtonController.currentBtnName != "Weapons")
			{
				toPhase = TutorialPhase.SelectWeaponCategory;
				break;
			}
			if (CurrentCategory != CategoryNames.SniperCategory)
			{
				toPhase = TutorialPhase.SelectSniperSection;
				break;
			}
			if (CurrentItem.Id != WeaponTags.HunterRifleTag)
			{
				toPhase = TutorialPhase.SelectRifle;
				break;
			}
			for (int j = 0; j < WeaponManager.sharedManager.playerWeapons.Count; j++)
			{
				string text2 = ItemDb.GetByPrefabName((WeaponManager.sharedManager.playerWeapons[j] as Weapon).weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag;
				if (text2 == WeaponTags.HunterRifleTag)
				{
					if (TutorialPhasePassed < TutorialPhase.SelectWearCategory)
					{
						TutorialPhasePassed = TutorialPhase.SelectWearCategory;
					}
					toPhase = TutorialPhase.SelectWearCategory;
					break;
				}
			}
			break;
		}
		case TutorialPhase.SelectWearCategory:
			toPhase = ((!(superCategoriesButtonController.currentBtnName == "Wear")) ? TutorialPhase.SelectWearCategory : TutorialPhase.EquipArmor);
			break;
		case TutorialPhase.EquipArmor:
			if (superCategoriesButtonController.currentBtnName != "Wear")
			{
				toPhase = TutorialPhase.SelectWearCategory;
				break;
			}
			if (CurrentCategory != CategoryNames.ArmorCategory)
			{
				toPhase = TutorialPhase.SelectArmorSection;
				break;
			}
			if (CurrentItem.Id != "Armor_Army_1")
			{
				ChooseCarouselItem(new ShopItem("Armor_Army_1", CategoryNames.ArmorCategory), true);
			}
			toPhase = TutorialPhase.EquipArmor;
			break;
		case TutorialPhase.SelectArmorSection:
			if (superCategoriesButtonController.currentBtnName != "Wear")
			{
				toPhase = TutorialPhase.SelectWearCategory;
				break;
			}
			if (CurrentCategory != CategoryNames.ArmorCategory)
			{
				toPhase = TutorialPhase.SelectArmorSection;
				break;
			}
			if (CurrentItem.Id != "Armor_Army_1")
			{
				ChooseCarouselItem(new ShopItem("Armor_Army_1", CategoryNames.ArmorCategory), true);
			}
			toPhase = TutorialPhase.EquipArmor;
			break;
		case TutorialPhase.SelectPetsCategory:
			toPhase = ((!(superCategoriesButtonController.currentBtnName != "Pets")) ? TutorialPhase.ShowEggsHint : TutorialPhase.SelectPetsCategory);
			break;
		case TutorialPhase.ShowEggsHint:
			if (m_shouldMoveToLeaveState)
			{
				TutorialPhasePassed = TutorialPhase.LeaveArmory;
				toPhase = TutorialPhase.LeaveArmory;
				backButton.isEnabled = true;
				unlockButton.isEnabled = true;
			}
			else
			{
				toPhase = TutorialPhase.ShowEggsHint;
			}
			break;
		}
		CoroutineRunner.Instance.StartCoroutine(ToTutorialPhaseCoroutine(toPhase));
	}

	private IEnumerator ToTutorialPhaseCoroutine(TutorialPhase toPhase)
	{
		TutorialStopBlinking();
		if (_tutorialCurrentState != null)
		{
			_tutorialCurrentState.StageAct(TutorialStageTrigger.Exit);
			if ((int)_tutorialCurrentState.ForStage < _tutorialHintsContainer.transform.childCount && _tutorialCurrentState.ForStage != TutorialPhase.ShowEggsHint)
			{
				_tutorialHintsContainer.transform.GetChild((int)_tutorialCurrentState.ForStage).gameObject.SetActive(false);
			}
			if (_tutorialCurrentState.ForStage >= TutorialPhase.ShowEggsHint && (superCategoriesButtonController.currentBtnName != Supercategory.Pets.ToString() || backButton.isEnabled))
			{
				_tutorialHintsContainer.transform.GetChild(8).gameObject.SetActive(false);
			}
		}
		TutorialPhase toPhase2 = default(TutorialPhase);
		_tutorialCurrentState = _tutorialStates.FirstOrDefault((TutorialState d) => d.ForStage == toPhase2);
		if (_tutorialCurrentState == null)
		{
			throw new Exception(string.Format("undefined tutorial state: {0}", toPhase));
		}
		_tutorialCurrentState.StageAct(TutorialStageTrigger.Enter);
		AnalyticsConstants.TutorialState? state = null;
		if (!TutorialPhaseLastViewed.HasValue)
		{
			state = AnalyticsConstants.TutorialState.Open_Shop;
			TutorialPhaseLastViewed = TutorialPhase.SelectWeaponCategory;
		}
		else if (TutorialPhaseLastViewed.Value < toPhase)
		{
			TutorialPhaseLastViewed = toPhase;
			switch (toPhase)
			{
			case TutorialPhase.SelectRifle:
				state = AnalyticsConstants.TutorialState.Category_Sniper;
				break;
			case TutorialPhase.SelectWearCategory:
				state = AnalyticsConstants.TutorialState.Equip_Sniper;
				break;
			case TutorialPhase.EquipArmor:
				state = AnalyticsConstants.TutorialState.Category_Armor;
				break;
			case TutorialPhase.LeaveArmory:
				state = AnalyticsConstants.TutorialState.Equip_Armor;
				break;
			}
		}
		if (state.HasValue)
		{
			AnalyticsStuff.Tutorial(state.Value);
		}
		if ((int)_tutorialCurrentState.ForStage < _tutorialHintsContainer.transform.childCount && _tutorialCurrentState.ForStage != TutorialPhase.ShowEggsHint)
		{
			_tutorialHintsContainer.transform.GetChild((int)_tutorialCurrentState.ForStage).gameObject.SetActive(true);
		}
		if (_tutorialCurrentState.ForStage >= TutorialPhase.ShowEggsHint && superCategoriesButtonController.currentBtnName == Supercategory.Pets.ToString() && !backButton.isEnabled)
		{
			_tutorialHintsContainer.transform.GetChild(8).gameObject.SetActive(true);
		}
		yield break;
	}

	private void TutorialSelectWeaponCategory(TutorialStageTrigger trigger)
	{
		switch (trigger)
		{
		case TutorialStageTrigger.Enter:
		{
			BtnCategory btnCategory = superCategoriesButtonController.buttons.First((BtnCategory b) => b.btnName == "Weapons");
			UISprite component = btnCategory.gameObject.GetChildGameObject("Blink", true).GetComponent<UISprite>();
			CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(component, 0.004f, 1f));
			break;
		}
		case TutorialStageTrigger.Exit:
			TutorialStopBlinking();
			break;
		}
	}

	private void TutorialSelectSniperSection(TutorialStageTrigger trigger)
	{
		switch (trigger)
		{
		case TutorialStageTrigger.Enter:
		{
			GameObject go = TransformOfButtonForCategory(CategoryNames.SniperCategory).gameObject;
			UIWidget component = go.GetChildGameObject("Pressed").GetComponent<UIWidget>();
			CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(component, 0.004f, 1f));
			break;
		}
		case TutorialStageTrigger.Exit:
			TutorialStopBlinking();
			break;
		}
	}

	private void TutorialSelectRifle(TutorialStageTrigger trigger)
	{
		switch (trigger)
		{
		case TutorialStageTrigger.Enter:
		{
			GameObject go = TransformOfButtonForCategory(CategoryNames.SniperCategory).gameObject;
			UIWidget component = go.GetChildGameObject("Pressed").GetComponent<UIWidget>();
			CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(component, 1f, 1f));
			ArmoryCell armoryCellByItemId = GetArmoryCellByItemId(WeaponTags.HunterRifleTag);
			UISprite component2 = armoryCellByItemId.gameObject.GetChildGameObject("Blink").GetComponent<UISprite>();
			CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(component2, 0.004f, 1f));
			gridScrollView.SetDragAmount(0f, 0f, false);
			gridScrollView.enabled = false;
			gridScrollView.verticalScrollBar.gameObject.SetActiveSafeSelf(false);
			break;
		}
		case TutorialStageTrigger.Exit:
			gridScrollView.enabled = true;
			gridScrollView.verticalScrollBar.gameObject.SetActiveSafeSelf(true);
			TutorialStopBlinking();
			break;
		}
	}

	private void TutorialEquipRifle(TutorialStageTrigger trigger)
	{
		switch (trigger)
		{
		case TutorialStageTrigger.Enter:
		{
			GameObject go = TransformOfButtonForCategory(CategoryNames.SniperCategory).gameObject;
			UIWidget component = go.GetChildGameObject("Pressed").GetComponent<UIWidget>();
			CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(component, 1f, 1f));
			gridScrollView.enabled = false;
			gridScrollView.verticalScrollBar.gameObject.SetActiveSafeSelf(false);
			UISprite component2 = equipButton.gameObject.GetChildGameObject("Blink").GetComponent<UISprite>();
			CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(component2, 0.004f, 1f));
			break;
		}
		case TutorialStageTrigger.Exit:
			gridScrollView.enabled = true;
			gridScrollView.verticalScrollBar.gameObject.SetActiveSafeSelf(true);
			TutorialStopBlinking();
			break;
		}
	}

	private void TutorialSelectWearCategory(TutorialStageTrigger trigger)
	{
		switch (trigger)
		{
		case TutorialStageTrigger.Enter:
		{
			BtnCategory btnCategory = superCategoriesButtonController.buttons.First((BtnCategory b) => b.btnName == "Wear");
			UISprite component = btnCategory.gameObject.GetChildGameObject("Blink", true).GetComponent<UISprite>();
			CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(component, 0.004f, 1f));
			break;
		}
		case TutorialStageTrigger.Exit:
			TutorialStopBlinking();
			break;
		}
	}

	private void TutorialEquipArmor(TutorialStageTrigger trigger)
	{
		switch (trigger)
		{
		case TutorialStageTrigger.Enter:
		{
			ChooseCarouselItem(new ShopItem("Armor_Army_1", CategoryNames.ArmorCategory), true);
			scrollViewPanel.GetComponent<UIScrollView>().enabled = false;
			gridScrollView.verticalScrollBar.gameObject.SetActiveSafeSelf(false);
			UISprite component = equipButton.gameObject.GetChildGameObject("Blink").GetComponent<UISprite>();
			CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(component, 0.004f, 1f));
			break;
		}
		case TutorialStageTrigger.Exit:
			TutorialStopBlinking();
			scrollViewPanel.GetComponent<UIScrollView>().enabled = true;
			gridScrollView.verticalScrollBar.gameObject.SetActiveSafeSelf(true);
			break;
		}
	}

	private void TutorialSelectArmorSection(TutorialStageTrigger trigger)
	{
		switch (trigger)
		{
		case TutorialStageTrigger.Enter:
		{
			GameObject go = TransformOfButtonForCategory(CategoryNames.ArmorCategory).gameObject;
			UIWidget component = go.GetChildGameObject("Pressed").GetComponent<UIWidget>();
			CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(component, 0.004f, 1f, 1f));
			gridScrollView.verticalScrollBar.gameObject.SetActiveSafeSelf(false);
			break;
		}
		case TutorialStageTrigger.Exit:
			TutorialStopBlinking();
			gridScrollView.verticalScrollBar.gameObject.SetActiveSafeSelf(true);
			break;
		}
	}

	private void TutorialSelectPetsCategory(TutorialStageTrigger trigger)
	{
		switch (trigger)
		{
		case TutorialStageTrigger.Enter:
		{
			BtnCategory btnCategory = superCategoriesButtonController.buttons.First((BtnCategory b) => b.btnName == "Pets");
			UISprite component = btnCategory.gameObject.GetChildGameObject("Blink", true).GetComponent<UISprite>();
			CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(component, 0.004f, 1f));
			gridScrollView.verticalScrollBar.gameObject.SetActiveSafeSelf(CurrentCategory == CategoryNames.ArmorCategory);
			break;
		}
		case TutorialStageTrigger.Exit:
			TutorialStopBlinking();
			break;
		}
	}

	private void TutorialShowEggsHint(TutorialStageTrigger trigger)
	{
		switch (trigger)
		{
		case TutorialStageTrigger.Enter:
			if (!m_startedLeaveArmoryStateTimer)
			{
				StartCoroutine(WaitAndSetShouldMoveToLeaveState());
				m_startedLeaveArmoryStateTimer = true;
			}
			break;
		case TutorialStageTrigger.Exit:
			TutorialStopBlinking();
			scrollViewPanel.GetComponent<UIScrollView>().enabled = true;
			gridScrollView.verticalScrollBar.gameObject.SetActiveSafeSelf(true);
			break;
		}
	}

	private void TutorialLeaveArmory(TutorialStageTrigger trigger)
	{
		switch (trigger)
		{
		case TutorialStageTrigger.Enter:
		{
			UISprite component = backButton.gameObject.GetChildGameObject("Blink").GetComponent<UISprite>();
			CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(component, 0.004f, 1f));
			break;
		}
		case TutorialStageTrigger.Exit:
			TutorialStopBlinking();
			break;
		}
	}

	private IEnumerator WaitAndSetShouldMoveToLeaveState()
	{
		yield return new WaitForRealSeconds(3.5f);
		m_shouldMoveToLeaveState = true;
		UpdateTutorialState();
	}

	private void TutorialStopBlinking()
	{
		CancellationTokenSource[] array = _tutorialTokensSources.ToArray();
		foreach (CancellationTokenSource cancellationTokenSource in array)
		{
			cancellationTokenSource.Cancel();
		}
	}

	private void TutorialStopBlinking_EggIfno()
	{
		CancellationTokenSource[] array = _tutorialEggsInfoTokensSources.ToArray();
		foreach (CancellationTokenSource cancellationTokenSource in array)
		{
			cancellationTokenSource.Cancel();
		}
	}

	private void TutorialStopBlinking_PetUpgrade()
	{
		CancellationTokenSource[] array = _tutorialPetUpgradeTokensSources.ToArray();
		foreach (CancellationTokenSource cancellationTokenSource in array)
		{
			cancellationTokenSource.Cancel();
		}
	}

	private IEnumerator BlinkAlpha(UIWidget widget, float fromAlpha, float toAlpha, float defaultAlpha = 0.004f, float blinkTimeInSeconds = 0.5f)
	{
		float elapsedTime = 0f;
		CancellationTokenSource tokenSource = new CancellationTokenSource();
		_tutorialTokensSources.Add(tokenSource);
		while (!tokenSource.Token.IsCancellationRequested)
		{
			if (elapsedTime >= blinkTimeInSeconds)
			{
				elapsedTime = 0f;
				float tmp = fromAlpha;
				fromAlpha = toAlpha;
				toAlpha = tmp;
			}
			widget.alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / blinkTimeInSeconds);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		_tutorialTokensSources.Remove(tokenSource);
		widget.alpha = defaultAlpha;
	}

	private IEnumerator BlinkAlpha_PetUpgrades(UIWidget widget, float fromAlpha, float toAlpha, float defaultAlpha = 0.004f, float blinkTimeInSeconds = 0.5f)
	{
		float elapsedTime = 0f;
		CancellationTokenSource tokenSource = new CancellationTokenSource();
		_tutorialPetUpgradeTokensSources.Add(tokenSource);
		while (!tokenSource.Token.IsCancellationRequested)
		{
			if (elapsedTime >= blinkTimeInSeconds)
			{
				elapsedTime = 0f;
				float tmp = fromAlpha;
				fromAlpha = toAlpha;
				toAlpha = tmp;
			}
			widget.alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / blinkTimeInSeconds);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		_tutorialPetUpgradeTokensSources.Remove(tokenSource);
		widget.alpha = defaultAlpha;
	}

	private IEnumerator BlinkAlpha_EggInfo(UIWidget widget, float fromAlpha, float toAlpha, float defaultAlpha = 0.004f, float blinkTimeInSeconds = 0.5f)
	{
		float elapsedTime = 0f;
		CancellationTokenSource tokenSource = new CancellationTokenSource();
		_tutorialEggsInfoTokensSources.Add(tokenSource);
		while (!tokenSource.Token.IsCancellationRequested)
		{
			if (elapsedTime >= blinkTimeInSeconds)
			{
				elapsedTime = 0f;
				float tmp = fromAlpha;
				fromAlpha = toAlpha;
				toAlpha = tmp;
			}
			widget.alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / blinkTimeInSeconds);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		_tutorialEggsInfoTokensSources.Remove(tokenSource);
		widget.alpha = defaultAlpha;
	}

	private IEnumerator BlinkColor(Func<Color> getter, Action<Color> setter, Color fromColor, Color toColor, float blinkTimeInSeconds = 0.5f)
	{
		float elapsedTime = 0f;
		Color defaultColor = getter();
		CancellationTokenSource tokenSource = new CancellationTokenSource();
		_tutorialTokensSources.Add(tokenSource);
		while (!tokenSource.Token.IsCancellationRequested)
		{
			if (elapsedTime >= blinkTimeInSeconds)
			{
				elapsedTime = 0f;
				Color tmp = fromColor;
				fromColor = toColor;
				toColor = tmp;
			}
			setter(Color.Lerp(fromColor, toColor, elapsedTime / blinkTimeInSeconds));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		_tutorialTokensSources.Remove(tokenSource);
		setter(defaultColor);
	}

	private void UpdateExtTutorial()
	{
		if (!GuiActive)
		{
			return;
		}
		TutorialStopBlinking();
		TutorialStopBlinking_EggIfno();
		TutorialStopBlinking_PetUpgrade();
		if (PlayerPrefs.GetInt("tutorial_info_hint_viewed", 0) == 0)
		{
			if (superCategoriesButtonController.currentBtnName == "Weapons")
			{
				GameObject childGameObject = infoButton.gameObject.GetChildGameObject("Blink");
				if (childGameObject != null)
				{
					CoroutineRunner.Instance.StartCoroutine(BlinkAlpha(childGameObject.GetComponent<UISprite>(), 0.004f, 1f));
				}
				else
				{
					CoroutineRunner.Instance.StartCoroutine(BlinkColor(() => infoButton.defaultColor, delegate(Color c)
					{
						infoButton.defaultColor = c;
					}, infoButton.defaultColor, Color.green));
				}
				GameObject childGameObject2 = _tutorialHintsExtContainer.GetChildGameObject("OpenInfoHint", true);
				childGameObject2.SetActive(true);
				if (m_tutorialInfoBtnED == null)
				{
					m_tutorialInfoBtnED = new EventDelegate(TutorialOnInfoButtonClicked);
				}
				if (!infoButton.onClick.Contains(m_tutorialInfoBtnED))
				{
					infoButton.onClick.Add(m_tutorialInfoBtnED);
				}
				if (infoScreen != null)
				{
					TutorialOnInfoButtonClicked();
				}
			}
			else
			{
				TutorialStopBlinking();
				GameObject childGameObject3 = _tutorialHintsExtContainer.GetChildGameObject("OpenInfoHint", true);
				childGameObject3.SetActive(false);
				if (infoButton.onClick.Contains(m_tutorialInfoBtnED))
				{
					infoButton.onClick.Remove(m_tutorialInfoBtnED);
				}
			}
		}
		else if (Storager.getInt("tutorial_button_try_highlighted", false) == 0 && ExperienceController.sharedController.currentLevel > 1)
		{
			GameObject childGameObject4 = _tutorialHintsExtContainer.GetChildGameObject("TryHint", true);
			if (tryGun.gameObject.activeInHierarchy && superCategoriesButtonController.currentBtnName == "Weapons" && infoScreen == null)
			{
				childGameObject4.SetActive(true);
				Storager.SubscribeToChanged("tutorial_button_try_highlighted", OnKEY_BTN_TRY_HIGHLIGHTED_Changed);
			}
			else
			{
				childGameObject4.SetActive(false);
			}
		}
		if (ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel > 1 && Storager.getInt("Shop.Tutorial.KEY_TUTORIAL_EGG_INFO_HINT_VIEWED", false) == 0)
		{
			if (CurrentCategory == CategoryNames.EggsCategory)
			{
				GameObject childGameObject5 = infoButton.gameObject.GetChildGameObject("Blink");
				CoroutineRunner.Instance.StartCoroutine(BlinkAlpha_EggInfo(childGameObject5.GetComponent<UISprite>(), 0.004f, 1f));
				GameObject childGameObject6 = _tutorialHintsExtContainer.GetChildGameObject("PetsInfo", true);
				childGameObject6.SetActive(true);
				if (m_tutorialInfoBtnED_Eggs == null)
				{
					m_tutorialInfoBtnED_Eggs = new EventDelegate(TutorialOnInfoEggsButtonClicked);
				}
				if (!infoButton.onClick.Contains(m_tutorialInfoBtnED_Eggs))
				{
					infoButton.onClick.Add(m_tutorialInfoBtnED_Eggs);
				}
				if (rentScreenPoint.FindChild("PetInfoScreen(Clone)") != null)
				{
					TutorialOnInfoEggsButtonClicked();
				}
			}
			else
			{
				TutorialStopBlinking_EggIfno();
				GameObject childGameObject7 = _tutorialHintsExtContainer.GetChildGameObject("PetsInfo", true);
				childGameObject7.SetActive(false);
				if (infoButton.onClick.Contains(m_tutorialInfoBtnED_Eggs))
				{
					infoButton.onClick.Remove(m_tutorialInfoBtnED_Eggs);
				}
			}
		}
		try
		{
			if (Storager.getInt("Shop.Tutorial.KEY_TUTORIAL_PET_UPRADE_HINT_VIEWED", false) != 0)
			{
				return;
			}
			if (CurrentCategory == CategoryNames.PetsCategory && Singleton<PetsManager>.Instance.PlayerPets.Count > 0 && CurrentItem.Id != null && PetsInfo.info.ContainsKey(CurrentItem.Id) && upgradeButton.gameObject.activeInHierarchy && upgradeButton.isEnabled)
			{
				GameObject childGameObject8 = upgradeButton.gameObject.GetChildGameObject("Blink");
				CoroutineRunner.Instance.StartCoroutine(BlinkAlpha_PetUpgrades(childGameObject8.GetComponent<UISprite>(), 0.004f, 1f));
				GameObject childGameObject9 = _tutorialHintsExtContainer.GetChildGameObject("PetsUpgrade", true);
				childGameObject9.SetActive(true);
				if (!m_petUpgradeHideCoroutineStarted)
				{
					StartCoroutine(WaitAndRemovePetUpgradeHint());
				}
			}
			else
			{
				RemovePetUpgradeHint();
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in UpdateExitTutorial pet upgrade hint: {0}", ex);
		}
	}

	private IEnumerator WaitAndRemovePetUpgradeHint()
	{
		m_petUpgradeHideCoroutineStarted = true;
		yield return new WaitForRealSeconds(5f);
		RemovePetUpgradeHint();
		Storager.setInt("Shop.Tutorial.KEY_TUTORIAL_PET_UPRADE_HINT_VIEWED", 1, false);
	}

	private void RemovePetUpgradeHint()
	{
		TutorialStopBlinking_PetUpgrade();
		GameObject childGameObject = _tutorialHintsExtContainer.GetChildGameObject("PetsUpgrade", true);
		childGameObject.SetActive(false);
	}

	private void OnKEY_BTN_TRY_HIGHLIGHTED_Changed()
	{
		if (Storager.getInt("tutorial_button_try_highlighted", false) > 0)
		{
			Storager.UnSubscribeToChanged("tutorial_button_try_highlighted", OnKEY_BTN_TRY_HIGHLIGHTED_Changed);
			GameObject childGameObject = _tutorialHintsExtContainer.GetChildGameObject("TryHint", true);
			childGameObject.SetActive(false);
		}
	}

	private void TutorialOnInfoButtonClicked()
	{
		TutorialStopBlinking();
		infoButton.onClick.Remove(m_tutorialInfoBtnED);
		GameObject childGameObject = _tutorialHintsExtContainer.GetChildGameObject("OpenInfoHint", true);
		childGameObject.SetActive(false);
		PlayerPrefs.SetInt("tutorial_info_hint_viewed", 1);
	}

	private void TutorialOnInfoEggsButtonClicked()
	{
		TutorialStopBlinking_EggIfno();
		infoButton.onClick.Remove(m_tutorialInfoBtnED_Eggs);
		GameObject childGameObject = _tutorialHintsExtContainer.GetChildGameObject("PetsInfo", true);
		childGameObject.SetActive(false);
		SetEggInfoHintViewed();
	}

	private IEnumerator WaitCoroutine(Func<bool> condition, Action afterTrue)
	{
		while (!condition())
		{
			yield return null;
		}
		afterTrue();
	}

	private void TutorialDisableHints()
	{
		for (int i = 0; i < _tutorialHintsContainer.transform.childCount; i++)
		{
			_tutorialHintsContainer.transform.GetChild(i).gameObject.SetActive(false);
		}
		for (int j = 0; j < _tutorialHintsExtContainer.transform.childCount; j++)
		{
			_tutorialHintsContainer.transform.GetChild(j).gameObject.SetActive(false);
		}
	}

	public void SetArmorVisible(bool isVisible)
	{
		ShowArmor = isVisible;
		SetPersArmorVisible(armorPoint);
		PlayerPrefs.SetInt("ShowArmorKeySetting", ShowArmor.ToInt());
	}

	public void SetHatVisible(bool isVisible)
	{
		ShowHat = isVisible;
		SetPersHatVisible(hatPoint);
		PlayerPrefs.SetInt("ShowHatKeySetting", ShowWear.ToInt());
	}

	public void SetWearVisible(bool isVisible)
	{
		if (isVisible != ShowWear)
		{
			ShowWear = isVisible;
			PlayerPrefs.SetInt("ShowWearKeySetting", ShowWear.ToInt());
			SetRenderersVisibleFromPoint(characterInterface.hatPoint, isVisible);
			SetRenderersVisibleFromPoint(characterInterface.maskPoint, isVisible);
			SetRenderersVisibleFromPoint(characterInterface.leftBootPoint, isVisible);
			SetRenderersVisibleFromPoint(characterInterface.rightBootPoint, isVisible);
			SetRenderersVisibleFromPoint(characterInterface.capePoint, isVisible);
		}
	}

	public static void SetPersArmorVisible(Transform armorPoint)
	{
		SetRenderersVisibleFromPoint(armorPoint, ShowArmor);
		if (armorPoint.childCount <= 0)
		{
			return;
		}
		Transform child = armorPoint.GetChild(0);
		ArmorRefs component = child.GetChild(0).GetComponent<ArmorRefs>();
		if (component != null)
		{
			if (component.leftBone != null)
			{
				SetRenderersVisibleFromPoint(component.leftBone, ShowArmor);
			}
			if (component.rightBone != null)
			{
				SetRenderersVisibleFromPoint(component.rightBone, ShowArmor);
			}
		}
	}

	public static void SetPersHatVisible(Transform hatPoint)
	{
	}

	public void HandleShowArmorButton(UIToggle toggle)
	{
		SetArmorVisible(toggle.value);
	}

	public void HandleShowHatButton(UIToggle toggle)
	{
		SetHatVisible(toggle.value);
	}

	public void HandleShowWearButton(UIToggle toggle)
	{
		SetWearVisible(toggle.value);
	}

	public static CategoryNames MapShopCategoryToItemCategory(CategoryNames category)
	{
		if (category == CategoryNames.BestWeapons || category == CategoryNames.BestWear || category == CategoryNames.BestGadgets)
		{
			Debug.LogErrorFormat("MapShopCategoryToItemCategory - best category");
			return category;
		}
		switch (category)
		{
		case CategoryNames.LeagueHatsCategory:
			return CategoryNames.HatsCategory;
		case CategoryNames.SkinsCategoryEditor:
		case CategoryNames.SkinsCategoryMale:
		case CategoryNames.SkinsCategoryFemale:
		case CategoryNames.SkinsCategorySpecial:
		case CategoryNames.SkinsCategoryPremium:
		case CategoryNames.LeagueSkinsCategory:
			return CategoryNames.SkinsCategory;
		default:
			return category;
		}
	}

	public static CategoryNames RealCategoryToPseudoCategory(CategoryNames category, string itemId)
	{
		if (Wear.LeagueForWear(itemId, category) > 0 || itemId == "league1_hat_hitman")
		{
			return CategoryNames.LeagueHatsCategory;
		}
		return category;
	}

	public void HandleBuyButton()
	{
		if (IsExiting)
		{
			Debug.LogErrorFormat("HandleBuyButton: IsExiting");
		}
		else
		{
			BuyOrUpgradeWeapon(false);
		}
	}

	public void HandleUpgradeButton()
	{
		if (IsExiting)
		{
			Debug.LogErrorFormat("HandleUpgradeButton: IsExiting");
		}
		else
		{
			BuyOrUpgradeWeapon(true);
		}
	}

	public void HandleRenamePetButton()
	{
		if (rentScreenPoint.childCount != 0 || CurrentItem == null || CurrentItem.Category != CategoryNames.PetsCategory || CurrentItem.Id == null)
		{
			return;
		}
		try
		{
			Transform transform = UnityEngine.Object.Instantiate(Resources.Load<Transform>("NguiWindows/PetWindows"));
			transform.parent = rentScreenPoint;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			EggHatchingWindowController component = transform.GetComponent<EggHatchingWindowController>();
			component.SetRenameMode();
			component.SetPetId(CurrentItem.Id);
			component.ReplaceEggWithPet(CurrentItem.Id);
			component.SetPetsNameToInput(Singleton<PetsManager>.Instance.GetPlayerPet(CurrentItem.Id).PetName);
			ShopItem itemBefore = CurrentItem;
			component.OnCloseCustomAction = delegate
			{
				if (CurrentCategory == CategoryNames.PetsCategory && itemBefore != null && !itemBefore.Id.IsNullOrEmpty())
				{
					ChooseItem(itemBefore, true);
				}
			};
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in HandleRenamePetButton: {0}", ex);
			rentScreenPoint.DestroyChildren();
		}
	}

	private bool IsGridEmpty()
	{
		Transform child = itemsGrid.GetChild(0);
		if (child != null)
		{
			ArmoryCell component = child.GetComponent<ArmoryCell>();
			if (component != null)
			{
				return component.isEmpty || !component.gameObject.activeSelf;
			}
			Debug.LogErrorFormat("IsGridEmpty: armoryCell == null");
		}
		else
		{
			Debug.LogErrorFormat("IsGridEmpty: firstCel == null");
		}
		return true;
	}

	private bool NeedToShowPropertiesInCategory(CategoryNames category)
	{
		if (IsBestCategory(CurrentCategory) && IsGridEmpty())
		{
			return false;
		}
		if (category == CategoryNames.PetsCategory && Singleton<PetsManager>.Instance.PlayerPets.Count == 0)
		{
			return false;
		}
		bool value;
		if (propertiesShownInCategory.TryGetValue(category, out value))
		{
			return value;
		}
		Debug.LogErrorFormat("NeedToShowPropertiesInCategory: not found value for category {0}", category);
		return false;
	}

	public void HandleSuperIncubatorButton()
	{
		if (rentScreenPoint.childCount == 0 && CurrentCategory == CategoryNames.EggsCategory)
		{
			Transform transform = UnityEngine.Object.Instantiate(Resources.Load<Transform>("NguiWindows/SuperIncubatorWindow"));
			transform.parent = rentScreenPoint;
			transform.localPosition = Vector3.zero;
			transform.localScale = new Vector3(1f, 1f, 1f);
			transform.GetComponent<SuperIncubatorWindowController>().BuyAction = HandleSuperIncubatorBuyButton;
		}
	}

	public void EquipPetAndUpdate(string petId)
	{
		if (GuiActive)
		{
			StopPetAnimation();
		}
		EquipPet(petId);
		if (GuiActive)
		{
			UpdateIcons(false);
		}
	}

	public void HandleEquipButton()
	{
		if (IsWeaponCategory(CurrentItem.Category))
		{
			string text = WeaponManager.LastBoughtTag(CurrentItem.Id);
			if (text == null && WeaponManager.sharedManager != null && WeaponManager.sharedManager.IsAvailableTryGun(CurrentItem.Id))
			{
				text = CurrentItem.Id;
			}
			if (text == null)
			{
				return;
			}
			string prefabName = ItemDb.GetByTag(text).PrefabName;
			Weapon w = WeaponManager.sharedManager.allAvailablePlayerWeapons.OfType<Weapon>().FirstOrDefault((Weapon weapon) => weapon.weaponPrefab.nameNoClone() == prefabName);
			WeaponManager.sharedManager.EquipWeapon(w, true, true);
			WeaponManager.sharedManager.SaveWeaponAsLastUsed(WeaponManager.sharedManager.CurrentWeaponIndex);
			if (equipAction != null)
			{
				equipAction(text);
			}
			UpdateIcons(false);
		}
		else if (IsWearCategory(CurrentItem.Category))
		{
			string text2 = WeaponManager.LastBoughtTag(CurrentItem.Id);
			if (!string.IsNullOrEmpty(text2))
			{
				EquipWear(text2);
			}
			if ((TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted) && InTrainingAfterNoviceArmorRemoved)
			{
				InTrainingAfterNoviceArmorRemoved = false;
				HandleOffersUpdated();
			}
		}
		else if (CurrentItem.Category == CategoryNames.SkinsCategory)
		{
			SetSkinAsCurrent(CurrentItem.Id);
			UpdateIcons(true);
		}
		else if (CurrentItem.Category == CategoryNames.LeagueWeaponSkinsCategory)
		{
			EquipWeaponSkinWrapper(CurrentItem.Id);
			UpdateIcons(false);
		}
		else if (CurrentItem.Category == CategoryNames.PetsCategory)
		{
			EquipPetAndUpdate(CurrentItem.Id);
		}
		else if (IsGadgetsCategory(CurrentItem.Category))
		{
			EquipGadget(CurrentItem.Id, (GadgetInfo.GadgetCategory)CurrentItem.Category);
			UpdateIcons(false);
		}
		UpdateButtons();
		UpdateTutorialState();
	}

	public void HandleUnequipButton()
	{
		if (IsWearCategory(CurrentItem.Category))
		{
			UnequipCurrentWearInCategory(CurrentItem.Category, inGame);
		}
		else if (CurrentItem.Category == CategoryNames.LeagueWeaponSkinsCategory)
		{
			WeaponSkinsManager.UnequipSkin(CurrentItem.Id);
		}
		else if (CurrentItem.Category == CategoryNames.PetsCategory)
		{
			StopPetAnimation();
			UnequipPet(CurrentItem.Id);
			ChooseItem(CurrentItem);
		}
		UpdateButtons();
		UpdateIcons(false);
	}

	public void HandleEnableButton()
	{
		if (IsExiting)
		{
			Debug.LogErrorFormat("HandleEnableButton: IsExiting");
		}
		else if (CurrentItem.Id == "cape_Custom")
		{
			BuyOrUpgradeWeapon(false);
		}
	}

	public void HandleCreateButton()
	{
		if (!inGame)
		{
			GoToSkinsEditor();
		}
	}

	public void HandleSuperIncubatorBuyButton()
	{
		ItemPrice itemPrice = GetItemPrice("Eggs.SuperIncubatorId", CategoryNames.EggsCategory);
		int price = itemPrice.Price;
		string currency = itemPrice.Currency;
		TryToBuy(mainPanel, itemPrice, delegate
		{
			Singleton<EggsManager>.Instance.AddEggsForSuperIncubator();
			AnalyticsStuff.LogSales("Eggs.SuperIncubatorId", "Eggs");
			SuperIncubatorWindowController componentInChildren = rentScreenPoint.GetComponentInChildren<SuperIncubatorWindowController>(true);
			if (componentInChildren != null)
			{
				componentInChildren.HandleClose();
			}
		}, null, null, delegate
		{
			PlayPersAnimations();
		}, delegate
		{
			SetBankCamerasEnabled();
		}, delegate
		{
			ShowGridOrArmorCarousel();
			SetOtherCamerasEnabled(false);
			ChooseCategory(CategoryNames.EggsCategory);
		});
	}

	public void HandleUnlockButton()
	{
		if (IsExiting)
		{
			Debug.LogErrorFormat("HandleUnlockButton: IsExiting");
			return;
		}
		ItemPrice itemPrice = GetItemPrice("CustomSkinID", CategoryNames.SkinsCategory);
		int priceAmount = itemPrice.Price;
		string priceCurrency = itemPrice.Currency;
		TryToBuy(mainPanel, itemPrice, delegate
		{
			if (Defs.isSoundFX)
			{
				UIPlaySound component = unlockButton.GetComponent<UIPlaySound>();
				if (component != null)
				{
					component.Play();
				}
			}
			if (ShopNGUIController.GunBought != null)
			{
				ShopNGUIController.GunBought();
			}
			string salesName = AnalyticsConstants.GetSalesName(CategoryNames.SkinsCategory);
			AnalyticsStuff.LogSales(Defs.SkinsMakerInProfileBought, salesName);
			AnalyticsFacade.InAppPurchase(Defs.SkinsMakerInProfileBought, salesName, 1, priceAmount, priceCurrency);
			Storager.setInt(Defs.SkinsMakerInProfileBought, 1, true);
			ReloadItemGrid(new ShopItem("CustomSkinID", CategoryNames.SkinsCategory));
			if (!inGame)
			{
				SynchronizeAndroidPurchases("Custom skin");
			}
			if (!inGame)
			{
				GoToSkinsEditor();
			}
		}, null, null, delegate
		{
			PlayPersAnimations();
		}, delegate
		{
			ButtonClickSound.Instance.PlayClick();
			SetBankCamerasEnabled();
		}, delegate
		{
			ShowGridOrArmorCarousel();
			SetOtherCamerasEnabled(false);
		});
		ShowLockOrPropertiesAndButtons();
	}

	public void HandleEditButton()
	{
		if (!inGame)
		{
			GoToSkinsEditor();
		}
	}

	public void HandleDeleteButton()
	{
		string skinWhereDelteWasPressed = CurrentItem.Id;
		InfoWindowController.ShowDialogBox(LocalizationStore.Get("Key_1693"), delegate
		{
			ButtonClickSound.Instance.PlayClick();
			string currentSkinNameForPers = SkinsController.currentSkinNameForPers;
			if (skinWhereDelteWasPressed != null)
			{
				SkinsController.DeleteUserSkin(skinWhereDelteWasPressed);
				if (skinWhereDelteWasPressed.Equals(currentSkinNameForPers))
				{
					SetSkinAsCurrent("0");
					UpdateIcons(false);
				}
				UpdatePersSkin(SkinsController.currentSkinNameForPers ?? "0");
			}
			ReloadGridOrCarousel(new ShopItem("CustomSkinID", CategoryNames.SkinsCategory));
		});
	}

	public void HandleInfoButton()
	{
		if (rentScreenPoint.childCount == 0)
		{
			if (CurrentCategory == CategoryNames.EggsCategory)
			{
				Transform transform = UnityEngine.Object.Instantiate(Resources.Load<Transform>("NguiWindows/PetInfoScreen"));
				transform.parent = rentScreenPoint;
				transform.localPosition = Vector3.zero;
				transform.localScale = new Vector3(1f, 1f, 1f);
				UpdateTutorialState();
				SetEggInfoHintViewed();
			}
			else if (IsWeaponCategory(CurrentItem.Category) || IsGadgetsCategory(CurrentItem.Category))
			{
				infoScreen = UnityEngine.Object.Instantiate(Resources.Load<Transform>("ArmoryInfoScreen"));
				infoScreen.parent = rentScreenPoint;
				infoScreen.localPosition = Vector3.zero;
				infoScreen.localScale = new Vector3(1f, 1f, 1f);
				infoScreen.GetComponent<ArmoryInfoScreenController>().SetItem(sharedShop.CurrentItem);
				UpdateTutorialState();
			}
		}
	}

	public void HandleFacebookButton()
	{
		_isFromPromoActions = false;
		MainMenuController.DoMemoryConsumingTaskInEmptyScene(delegate
		{
			FacebookController.Login(delegate
			{
				if (GuiActive)
				{
					sharedShop.UpdateButtons();
				}
			}, null, "Shop");
		}, delegate
		{
			FacebookController.Login(null, null, "Shop");
		});
	}

	public void HandleProfileButton()
	{
		GameObject mainMenu = GameObject.Find("MainMenuNGUI");
		if ((bool)mainMenu)
		{
			mainMenu.SetActive(false);
		}
		GameObject inGameGui = GameObject.FindWithTag("InGameGUI");
		if ((bool)inGameGui)
		{
			inGameGui.SetActive(false);
		}
		GameObject networkTable = GameObject.FindWithTag("NetworkStartTableNGUI");
		if ((bool)networkTable)
		{
			networkTable.SetActive(false);
		}
		GuiActive = false;
		Action action = delegate
		{
		};
		ProfileController.Instance.DesiredWeaponTag = _assignedWeaponTag;
		ProfileController.Instance.ShowInterface(action, delegate
		{
			GuiActive = true;
			if ((bool)mainMenu)
			{
				mainMenu.SetActive(true);
			}
			if ((bool)inGameGui)
			{
				inGameGui.SetActive(true);
			}
			if ((bool)networkTable)
			{
				networkTable.SetActive(true);
			}
		});
	}

	public static bool IsWeaponCategory(CategoryNames c)
	{
		return c < CategoryNames.HatsCategory;
	}

	public static bool IsWearCategory(CategoryNames c)
	{
		return Wear.wear.Keys.Contains(c);
	}

	private static string[] _CurrentWeaponSetIDs()
	{
		string[] array = new string[6];
		WeaponManager sharedManager = WeaponManager.sharedManager;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (num >= sharedManager.playerWeapons.Count)
			{
				array[i] = null;
				continue;
			}
			Weapon weapon = sharedManager.playerWeapons[num] as Weapon;
			if (weapon.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == i)
			{
				num++;
				array[i] = ItemDb.GetByPrefabName(weapon.weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag;
			}
			else
			{
				array[i] = null;
			}
		}
		return array;
	}

	public static void EquipGadget(string gadgetId, GadgetInfo.GadgetCategory category)
	{
		if (gadgetId == null)
		{
			Debug.LogError("EquipGadget gadgetId == null");
			return;
		}
		string arg = GadgetsInfo.EquippedForCategory(category);
		Storager.setString(GadgetsInfo.SNForCategory(category), gadgetId, false);
		Action<string, string, GadgetInfo.GadgetCategory> equippedGadget = ShopNGUIController.EquippedGadget;
		if (equippedGadget != null)
		{
			equippedGadget(gadgetId, arg, category);
		}
	}

	public static void EquipPet(string petId)
	{
		if (petId == null)
		{
			Debug.LogError("EquipPet pet == null");
			return;
		}
		string eqipedPetId = Singleton<PetsManager>.Instance.GetEqipedPetId();
		Singleton<PetsManager>.Instance.SetEquipedPet(petId);
		Action<string, string> equippedPet = ShopNGUIController.EquippedPet;
		if (equippedPet != null)
		{
			equippedPet(petId, eqipedPetId);
		}
	}

	public static void UnequipPet(string petId)
	{
		if (petId == null)
		{
			Debug.LogError("UnequipPet petId == null");
			return;
		}
		Singleton<PetsManager>.Instance.SetEquipedPet(string.Empty);
		Action<string> unequippedPet = ShopNGUIController.UnequippedPet;
		if (unequippedPet != null)
		{
			unequippedPet(petId);
		}
	}

	public static void ShowAddTryGun(string gunTag, Transform point, string lr, Action<string> onPurchase = null, Action onEnterCoinsShopAdditional = null, Action onExitCoinsShopAdditional = null, Action<string> customEquipWearAction = null, bool expiredTryGun = false)
	{
		try
		{
			GameObject original = Resources.Load<GameObject>("TryGunScreen");
			GameObject gameObject = UnityEngine.Object.Instantiate(original);
			TryGunScreenController component = gameObject.GetComponent<TryGunScreenController>();
			Player_move_c.SetLayerRecursively(component.gameObject, LayerMask.NameToLayer(lr));
			component.transform.parent = point;
			component.transform.localPosition = new Vector3(0f, 0f, -130f);
			component.transform.localScale = new Vector3(1f, 1f, 1f);
			if (expiredTryGun)
			{
				WeaponManager.sharedManager.AddTryGunPromo(gunTag);
			}
			component.ItemTag = gunTag;
			component.onPurchaseCustomAction = onPurchase;
			component.onEnterCoinsShopAdditionalAction = onEnterCoinsShopAdditional;
			component.onExitCoinsShopAdditionalAction = onExitCoinsShopAdditional;
			component.customEquipWearAction = customEquipWearAction;
			component.ExpiredTryGun = expiredTryGun;
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShowAddTryGun: " + ex);
		}
	}

	private static int NumberOfColumnsInArmoryGrid()
	{
		return (!((double)((float)Screen.width / (float)Screen.height) < 1.5)) ? 4 : 3;
	}

	private void SetSliceShaderParams(ArmoryCell armoryCell)
	{
		Material material = armoryCell.capeRenderer.material;
		Material material2 = armoryCell.modelForSkin.GetComponent<MeshRenderer>().material;
		float y = topPointForShader.transform.localPosition.y;
		float y2 = bottomPointForShader.transform.localPosition.y + 5f;
		Transform parent = gridScrollView.transform.parent;
		material2.SetFloat("_TopBorder", parent.TransformPoint(new Vector3(0f, y, 0f)).y);
		material2.SetFloat("_BottomBorder", parent.TransformPoint(new Vector3(0f, y2, 0f)).y);
	}

	private void StopPetAnimation()
	{
		petProfileAnimationRunner.StopAllCoroutines();
	}

	private void UpdatePointsForSkinsShader()
	{
		topPointForShader.GetComponent<UIWidget>().ResetAndUpdateAnchors();
		bottomPointForShader.GetComponent<UIWidget>().ResetAndUpdateAnchors();
	}

	internal void ReloadItemGrid(ShopItem itemToSet)
	{
		if (!_gridInitiallyRepositioned)
		{
			itemsGrid.Reposition();
		}
		int num = NumberOfColumnsInArmoryGrid();
		UIWidget component = gridScrollView.transform.parent.GetComponent<UIWidget>();
		component.ResetAndUpdateAnchors();
		UpdatePointsForSkinsShader();
		float cellWidth = ((float)component.width - 12f - (float)num * 10f) / (float)num;
		float cellHeight = 1f;
		float scale = 1f;
		scale = cellWidth / 170f;
		cellHeight = 150f * scale;
		ArmoryCell[] array = itemsGrid.GetComponentsInChildren<ArmoryCell>(true) ?? new ArmoryCell[0];
		if (!_gridInitiallyRepositioned)
		{
			array.ForEach(delegate(ArmoryCell cell)
			{
				Transform transform2 = cell.transform;
				transform2.localScale = Vector3.one;
				transform2.GetChild(0).localScale = new Vector3(scale, scale, scale);
				cell.GetComponent<BoxCollider>().size = new Vector2(cellWidth, cellHeight);
			});
			_gridInitiallyRepositioned = true;
		}
		List<ShopItem> itemNamesList = GetItemNamesList(CurrentCategory);
		array.ForEach(delegate(ArmoryCell child)
		{
			UIToggle component2 = child.GetComponent<UIToggle>();
			if (component2 != null && component2.value)
			{
				List<EventDelegate> onChange = component2.onChange;
				component2.onChange = new List<EventDelegate>();
				bool instantTween = component2.instantTween;
				component2.instantTween = true;
				component2.Set(false);
				component2.onChange = onChange;
				component2.instantTween = instantTween;
			}
		});
		if (itemToSet == null && CurrentItem != null && itemNamesList.Any((ShopItem item) => item.Id == CurrentItem.Id && item.Category == CurrentItem.Category))
		{
			itemToSet = CurrentItem;
		}
		bool flag = false;
		Action<ArmoryCell, int> action = delegate(ArmoryCell cell, int cellNumber)
		{
			Transform transform = cell.transform;
			transform.SetParent(itemsGrid.transform, false);
			transform.localScale = Vector3.one;
			transform.GetChild(0).localScale = new Vector3(scale, scale, scale);
			cell.GetComponent<BoxCollider>().size = new Vector2(cellWidth, cellHeight);
			cell.name = cellNumber.ToString("D4");
		};
		for (int i = 0; i < itemNamesList.Count; i++)
		{
			bool flag2 = i >= array.Length;
			ArmoryCell armoryCell = ((!flag2) ? array[i] : GetNewCell());
			if (flag2)
			{
				flag = true;
				action(armoryCell, i);
			}
			armoryCell.Setup(itemNamesList[i], IsBestCategory(CurrentCategory));
			if (!armoryCell.gameObject.activeSelf)
			{
				armoryCell.gameObject.SetActiveSafeSelf(true);
			}
			else
			{
				armoryCell.StopAllCoroutines();
				armoryCell.UpdateAllAndStartUpdateCoroutine();
			}
			armoryCell.ReSubscribeToEquipEvents();
			try
			{
				SetSliceShaderParams(armoryCell);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in setting SliceToWorldPosShader: " + ex);
			}
		}
		int num2 = ((num != 3) ? 12 : 9);
		int num3 = 0;
		num3 = ((itemNamesList.Count < num2 && itemNamesList.Count > 0) ? (num2 - itemNamesList.Count) : ((itemNamesList.Count % num > 0 && itemNamesList.Count > 0) ? (num - itemNamesList.Count % num) : 0));
		int num4 = itemNamesList.Count + num3;
		for (int j = itemNamesList.Count; j < num4; j++)
		{
			bool flag3 = j >= array.Length;
			ArmoryCell armoryCell2 = ((!flag3) ? array[j] : GetNewCell());
			if (flag3)
			{
				flag = true;
				action(armoryCell2, j);
			}
			armoryCell2.MakeCellEmpty();
			armoryCell2.gameObject.SetActiveSafeSelf(true);
			armoryCell2.SetupEmptyCellCategory(CurrentCategory);
		}
		for (int k = num4; k < array.Length; k++)
		{
			array[k].gameObject.SetActiveSafeSelf(false);
		}
		itemsGrid.cellWidth = cellWidth + 10f;
		itemsGrid.cellHeight = cellHeight + 10f;
		itemsGrid.maxPerLine = num;
		if (flag)
		{
			itemsGrid.Reposition();
		}
		gridScrollView.ResetPosition();
		if (!gridScrollViewPanelUpdatedOnFirstLaunch)
		{
			gridScrollViewPanelUpdatedOnFirstLaunch = true;
			if (gridScrollView.panel != null)
			{
				gridScrollView.panel.ResetAndUpdateAnchors();
				gridScrollView.panel.Refresh();
			}
			AdjustCategoryGridCells();
		}
		if (itemToSet != null)
		{
			if (itemNamesList.Any((ShopItem item) => item.Id == itemToSet.Id))
			{
				ChooseItem(itemToSet, true, true);
			}
			else if (IsWeaponCategory(CurrentCategory))
			{
				string id = WeaponManager.LastBoughtTag(itemToSet.Id) ?? WeaponManager.FirstUnboughtOrForOurTier(CurrentItem.Id);
				ChooseItem(new ShopItem(id, CurrentCategory), true, true);
			}
			else if (CurrentCategory == CategoryNames.SkinsCategory)
			{
				CurrentItem = new ShopItem("CustomSkinID", CategoryNames.SkinsCategory);
			}
			else
			{
				CurrentItem = null;
			}
		}
	}

	public static void AddModel(GameObject pref, Action7<GameObject, Vector3, Vector3, string, float, int, int> act, CategoryNames category, bool isButtonInGameGui = false, WeaponSounds wsForPos = null)
	{
		float arg = 150f;
		Vector3 arg2 = Vector3.zero;
		Vector3 arg3 = Vector3.zero;
		GameObject gameObject = null;
		int arg4 = 0;
		int arg5 = 0;
		string arg6 = null;
		if (IsWeaponCategory(category))
		{
			arg = wsForPos.scaleShop;
			arg2 = wsForPos.positionShop;
			arg3 = wsForPos.rotationShop;
			gameObject = pref.GetComponent<InnerWeaponPars>().bonusPrefab;
			try
			{
				ItemRecord byPrefabName = ItemDb.GetByPrefabName(wsForPos.name.Replace("(Clone)", string.Empty));
				string text = WeaponUpgrades.TagOfFirstUpgrade(byPrefabName.Tag);
				ItemRecord byTag = ItemDb.GetByTag(text);
				WeaponSounds weaponSounds = Resources.Load<WeaponSounds>(string.Format("Weapons/{0}", byTag.PrefabName));
				arg6 = weaponSounds.shopName;
			}
			catch (Exception ex)
			{
				Debug.LogError("Error in getting shop name of first upgrade: " + ex);
				arg6 = wsForPos.shopName;
			}
			arg4 = wsForPos.tier;
		}
		else
		{
			switch (category)
			{
			case CategoryNames.SkinsCategory:
			{
				gameObject = UnityEngine.Object.Instantiate(pref);
				CharacterInterface component = gameObject.GetComponent<CharacterInterface>();
				component.SetSimpleCharacter();
				arg3 = component.rotationShop;
				arg = component.scaleShop;
				arg2 = component.positionShop;
				arg4 = component.shopTier;
				break;
			}
			case CategoryNames.HatsCategory:
			case CategoryNames.ArmorCategory:
			case CategoryNames.CapesCategory:
			case CategoryNames.BootsCategory:
			case CategoryNames.GearCategory:
			case CategoryNames.MaskCategory:
			{
				gameObject = pref.transform.GetChild(0).gameObject;
				ShopPositionParams infoForNonWeaponItem = ItemDb.GetInfoForNonWeaponItem(pref.nameNoClone(), category);
				arg3 = infoForNonWeaponItem.rotationShop;
				arg = infoForNonWeaponItem.scaleShop;
				arg2 = infoForNonWeaponItem.positionShop;
				arg6 = infoForNonWeaponItem.shopName;
				arg4 = infoForNonWeaponItem.tier;
				arg5 = infoForNonWeaponItem.League;
				break;
			}
			case CategoryNames.ThrowingCategory:
			case CategoryNames.ToolsCategoty:
			case CategoryNames.SupportCategory:
				gameObject = pref;
				arg3 = Vector3.zero;
				arg = 1f;
				arg2 = Vector3.zero;
				arg6 = string.Empty;
				arg4 = 1;
				arg5 = 1;
				break;
			}
		}
		Vector3 localPosition = Vector3.zero;
		GameObject gameObject2 = null;
		if (category == CategoryNames.SkinsCategory)
		{
			gameObject2 = gameObject;
			localPosition = new Vector3(0f, -1f, 0f);
		}
		else if (IsGadgetsCategory(category))
		{
			gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		}
		else if (gameObject != null)
		{
			Material[] array = null;
			Mesh mesh = null;
			SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
			if (skinnedMeshRenderer == null)
			{
				SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
				if (componentsInChildren != null && componentsInChildren.Length > 0)
				{
					skinnedMeshRenderer = componentsInChildren[0];
				}
			}
			if (skinnedMeshRenderer != null)
			{
				array = skinnedMeshRenderer.sharedMaterials;
				mesh = skinnedMeshRenderer.sharedMesh;
			}
			else
			{
				MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
				MeshRenderer component3 = gameObject.GetComponent<MeshRenderer>();
				if (component2 != null)
				{
					mesh = component2.sharedMesh;
				}
				if (component3 != null)
				{
					array = component3.sharedMaterials;
				}
			}
			if (array != null && mesh != null)
			{
				gameObject2 = new GameObject();
				gameObject2.AddComponent<MeshFilter>().sharedMesh = mesh;
				MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
				meshRenderer.materials = array;
				localPosition = -meshRenderer.bounds.center;
			}
		}
		try
		{
			DisableLightProbesRecursively(gameObject2);
		}
		catch (Exception ex2)
		{
			Debug.LogError("Exception DisableLightProbesRecursively: " + ex2);
		}
		GameObject gameObject3 = new GameObject();
		gameObject3.name = gameObject2.name;
		gameObject2.transform.localPosition = localPosition;
		gameObject2.transform.parent = gameObject3.transform;
		Player_move_c.SetLayerRecursively(gameObject3, LayerMask.NameToLayer("NGUIShop"));
		if (act != null)
		{
			act(gameObject3, arg2, arg3, arg6, arg, arg4, arg5);
		}
	}

	public void HandlePropertiesHideShow(PropertiesHideSHow propertiesHideShowScript)
	{
		if (IsWeaponCategory(CurrentItem.Category))
		{
			CategoryNames[] array = new CategoryNames[6]
			{
				CategoryNames.PrimaryCategory,
				CategoryNames.BackupCategory,
				CategoryNames.MeleeCategory,
				CategoryNames.SpecilCategory,
				CategoryNames.SniperCategory,
				CategoryNames.PremiumCategory
			};
			foreach (CategoryNames key in array)
			{
				propertiesShownInCategory[key] = !propertiesHideShowScript.isHidden;
			}
		}
		else if (IsGadgetsCategory(CurrentItem.Category))
		{
			CategoryNames[] array2 = new CategoryNames[3]
			{
				CategoryNames.ThrowingCategory,
				CategoryNames.ToolsCategoty,
				CategoryNames.SupportCategory
			};
			foreach (CategoryNames key2 in array2)
			{
				propertiesShownInCategory[key2] = !propertiesHideShowScript.isHidden;
			}
		}
		else
		{
			propertiesShownInCategory[CurrentItem.Category] = !propertiesHideShowScript.isHidden;
		}
		CoroutineRunner.Instance.StartCoroutine(SetPositionAfterPropertiesHideShow(propertiesHideShowScript));
	}

	private void GetArmoryCellAndAdjustShaderParams()
	{
		ArmoryCell componentInChildren = itemsGrid.GetComponentInChildren<ArmoryCell>(true);
		if (componentInChildren != null)
		{
			SetSliceShaderParams(componentInChildren);
		}
	}

	private IEnumerator SetPositionAfterPropertiesHideShow(PropertiesHideSHow propertiesHideShowScript)
	{
		CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.WaitForSecondsActionEveryNFrames(propertiesHideShowScript.animTime * 1.2f, delegate
		{
			gridScrollView.DisableSpring();
			gridScrollView.RestrictWithinBounds(true);
			//if (!gridScrollView.shouldMove)
			//{
				gridScrollView.SetDragAmount(0f, 0f, false);
			//}
			UpdateSkinShaderParams();
		}, 1));
		yield return CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.WaitForSecondsActionEveryNFrames(propertiesHideShowScript.animTime * 1.1f, delegate
		{
			RefreshGridScrollView();
		}, 3));
		if (CurrentItem.Category == CategoryNames.CapesCategory || CurrentItem.Category == CategoryNames.SkinsCategory)
		{
			GetArmoryCellAndAdjustShaderParams();
		}
		RefreshGridScrollView();
		needRefreshInLateUpdate = 2;
	}

	public ArmoryCell GetArmoryCellByItemId(string itemId)
	{
		return AllArmoryCells.FirstOrDefault((ArmoryCell c) => c.ItemId == itemId);
	}

	private void ClearCaption()
	{
		caption.text = string.Empty;
		foreach (UILabel wearNameLabel in wearNameLabels)
		{
			wearNameLabel.text = string.Empty;
		}
		foreach (UILabel skinNameLabel in skinNameLabels)
		{
			skinNameLabel.text = string.Empty;
		}
		foreach (UILabel armorNameLabel in armorNameLabels)
		{
			armorNameLabel.text = string.Empty;
		}
	}

	private void SetCurrentItemCaption(ShopItem item)
	{
		string id = item.Id;
		if (item.Category == CategoryNames.EggsCategory)
		{
			Egg egg = Singleton<EggsManager>.Instance.GetPlayerEggs().FirstOrDefault((Egg e) => e.Id.ToString() == item.Id);
			if (egg != null)
			{
				id = egg.Data.Id;
			}
			else
			{
				Debug.LogErrorFormat("ShopNguiController: idForName, egg = null, item.Id = {0}", item.Id);
			}
		}
		string itemName = ItemDb.GetItemName(id, item.Category);
		caption.text = itemName;
		foreach (UILabel wearNameLabel in wearNameLabels)
		{
			wearNameLabel.text = itemName;
		}
		if (skinNameLabels == null || (CurrentItem.Category != CategoryNames.SkinsCategory && CurrentItem.Category != CategoryNames.LeagueWeaponSkinsCategory))
		{
			return;
		}
		foreach (UILabel skinNameLabel in skinNameLabels)
		{
			if (CurrentItem.Category == CategoryNames.SkinsCategory)
			{
				skinNameLabel.text = ((item.Id == "CustomSkinID") ? LocalizationStore.Get("Key_1090") : ((!SkinsController.skinsNamesForPers.ContainsKey(item.Id)) ? string.Empty : SkinsController.skinsNamesForPers[item.Id]));
				continue;
			}
			try
			{
				WeaponSkin skin = WeaponSkinsManager.GetSkin(item.Id);
				if (skin != null)
				{
					skinNameLabel.text = LocalizationStore.Get(skin.Lkey);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in setting weapon skin caption: " + ex);
			}
		}
	}

	public void ChooseItem(ShopItem item, bool moveScroll = false, bool switchToggleInstantly = false)
	{
		ReturnPersWearAndSkinWhenSwitching();
		if (item.Id == null)
		{
			if (IsWeaponCategory(item.Category))
			{
				UpdatePersWithNewItem(item);
			}
			return;
		}
		List<ArmoryCell> allArmoryCells = AllArmoryCells;
		List<ArmoryCell> list = allArmoryCells.Where((ArmoryCell ac) => ac.gameObject.activeSelf).ToList();
		int num = list.FindIndex((ArmoryCell cell) => cell.ItemId == item.Id);
		ArmoryCell armoryCell = list[num];
		if (moveScroll)
		{
			float num2 = gridScrollView.bounds.extents.y * 2f;
			UIPanel uIPanel = gridScrollView.panel ?? gridScrollView.GetComponent<UIPanel>();
			float y = uIPanel.GetViewSize().y;
			if (num != -1)
			{
				int num3 = NumberOfColumnsInArmoryGrid();
				int num4 = num / num3;
				int num5 = ((list.Count % num3 != 0) ? (list.Count / num3 + 1) : (list.Count / num3));
				float cellHeight = itemsGrid.cellHeight;
				float num6 = (float)(num4 + 1) * cellHeight;
				if (y < num6)
				{
					float num7 = num2 - y;
					if (num7 > 0f)
					{
						float num8 = (num6 - y) / num7;
						num8 += 3f / num7;
						gridScrollView.SetDragAmount(0f, num8, false);
						if (!categoryGridsRepositioned && num8 > 1f)
						{
							float y2 = (num8 - 1f) * num7;
							gridScrollView.MoveRelative(new Vector3(0f, y2));
						}
						if (categoryGridsRepositioned)
						{
							float y3 = 1f;
							gridScrollView.MoveRelative(new Vector3(0f, y3));
						}
					}
				}
			}
		}
		if (armoryCell != null)
		{
			UIToggle component = armoryCell.GetComponent<UIToggle>();
			if (component != null)
			{
				List<EventDelegate> onChange = component.onChange;
				component.onChange = new List<EventDelegate>();
				bool instantTween = component.instantTween;
				if (switchToggleInstantly)
				{
					component.instantTween = true;
				}
				component.Set(true);
				component.onChange = onChange;
				component.instantTween = instantTween;
				armoryCell.UpdateUpgrades();
				armoryCell.UpdateDiscountVisibility();
			}
		}
		CurrentItem = item;
		UpdatePersWithNewItem(item);
		UpdateButtons();
		SetCurrentItemCaption(item);
		UpdateTutorialState();
	}

	private void RefreshGridScrollView()
	{
		if (gridScrollView.panel != null)
		{
			gridScrollView.panel.SetDirty();
			gridScrollView.panel.Refresh();
		}
	}

	private void SetUpUpgradesAndTiers(bool bought, ref bool buyActive, ref bool upgradeActive, ref bool saleActive, ref bool needMoreTrophiesActive)
	{
		bool flag = TempItemsController.PriceCoefs.ContainsKey(CurrentItem.Id);
		bool maxUpgrade = false;
		int totalNumberOfUpgrades;
		int num = ((CurrentItem.Id == null) ? (-1) : CurrentNumberOfUpgradesForWear(CurrentItem.Id, out maxUpgrade, CurrentItem.Category, out totalNumberOfUpgrades));
		bool flag2 = maxUpgrade;
		if ((!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted) || InTrainingAfterNoviceArmorRemoved)
		{
			buyActive = false;
			upgradeActive = false;
		}
		else
		{
			RatingSystem.RatingLeague ratingLeague = (RatingSystem.RatingLeague)Wear.LeagueForWear(CurrentItem.Id, CurrentItem.Category);
			bool flag3 = ratingLeague > RatingSystem.instance.currentLeague;
			buyActive = CurrentItem.Id != null && !flag2 && num == 0 && CurrentItem.Id != "cape_Custom" && !flag && !flag3;
			upgradeActive = CurrentItem.Id != null && !flag2 && num != 0 && !flag;
			needMoreTrophiesActive = num == 0 && flag3;
		}
		if (!flag2)
		{
			int num2 = Wear.TierForWear(WeaponManager.FirstUnboughtTag(CurrentItem.Id));
			upgradeButton.isEnabled = (!upgradeActive || ExpController.OurTierForAnyPlace() >= num2) && !flag;
		}
	}

	private static void UpdateTryGunDiscountTime(PropertiesArmoryItemContainer props, string itemId)
	{
		try
		{
			if (props != null && props.tryGunDiscountTime != null)
			{
				props.tryGunDiscountTime.text = RiliExtensions.GetTimeStringDays(WeaponManager.sharedManager.StartTimeForTryGunDiscount(itemId) + (long)WeaponManager.TryGunPromoDuration() - PromoActionsManager.CurrentUnixTime);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in UpdateTryGunDiscountTime: " + ex);
		}
	}

	public void UpdateButtons()
	{
		string text = null;
		if ((CurrentCategory != CategoryNames.EggsCategory || Singleton<EggsManager>.Instance.GetPlayerEggs().Count != 0) && (CurrentCategory != CategoryNames.PetsCategory || Singleton<PetsManager>.Instance.PlayerPets.Count != 0) && !IsEmptyBestCategory())
		{
			CategoryNames category = CurrentItem.Category;
			if (IsWeaponCategory(category))
			{
				text = WeaponManager.FirstUnboughtTag(CurrentItem.Id) ?? CurrentItem.Id;
				string text2 = WeaponManager.FirstTagForOurTier(CurrentItem.Id);
				List<string> list = WeaponUpgrades.ChainForTag(CurrentItem.Id);
				if (text2 != null && list != null && list.IndexOf(text2) > list.IndexOf(text))
				{
					text = text2;
				}
			}
			if (IsGadgetsCategory(category))
			{
				text = GadgetsInfo.FirstUnboughtOrForOurTier(CurrentItem.Id) ?? CurrentItem.Id;
				string text3 = GadgetsInfo.FirstForOurTier(CurrentItem.Id);
				List<string> list2 = GadgetsInfo.UpgradesChainForGadget(CurrentItem.Id);
				if (text3 != null && list2 != null && list2.IndexOf(text3) > list2.IndexOf(text))
				{
					text = text3;
				}
			}
		}
		GetStateButtons((CurrentItem == null) ? null : CurrentItem.Id, text, propertiesContainer, true);
		ReparentButtons();
		UpdatePropertiesPanels();
		UpdateVisibilityOfPropertiesPanelAndButtons();
		SetCamera();
	}

	public void GetStateButtons(string _viewedId, string _showForId_WEAPONS_AND_GADGET_ONLY, PropertiesArmoryItemContainer _propertiesContainer, bool isFromMainWindow)
	{
		bool flag = false;
		bool state = false;
		bool state2 = false;
		bool state3 = false;
		bool flag2 = false;
		bool flag3 = false;
		bool state4 = false;
		bool buyActive = false;
		bool state5 = false;
		bool state6 = false;
		bool upgradeActive = false;
		bool flag4 = false;
		bool flag5 = false;
		bool saleActive = false;
		string text = string.Empty;
		bool state7 = false;
		bool state8 = false;
		bool state9 = false;
		bool state10 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool needMoreTrophiesActive = false;
		bool flag8 = false;
		bool state11 = false;
		string text2 = string.Empty;
		string text3 = string.Empty;
		bool flag9 = TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted;
		bool flag10 = _viewedId != null && TempItemsController.PriceCoefs.ContainsKey(_viewedId);
		if (_propertiesContainer.upgradeButton != null)
		{
			_propertiesContainer.upgradeButton.isEnabled = true;
		}
		if (_propertiesContainer.trainButton != null)
		{
			_propertiesContainer.trainButton.isEnabled = true;
		}
		if (_propertiesContainer.gadgetProperties != null)
		{
			_propertiesContainer.gadgetProperties.SetActiveSafeSelf(CurrentItem != null && (IsGadgetsCategory(CurrentItem.Category) || CurrentItem.Category == CategoryNames.PetsCategory));
		}
		if (_propertiesContainer.weaponsRarityLabel != null)
		{
			_propertiesContainer.weaponsRarityLabel.gameObject.SetActiveSafe(CurrentItem != null && (IsWeaponCategory(CurrentItem.Category) || CurrentItem.Category == CategoryNames.PetsCategory));
		}
		if (_propertiesContainer.specialParams != null)
		{
			_propertiesContainer.specialParams.SetActiveSafeSelf(CurrentItem != null && (IsWeaponCategory(CurrentItem.Category) || IsGadgetsCategory(CurrentItem.Category) || (CurrentItem.Category == CategoryNames.PetsCategory && Singleton<PetsManager>.Instance.PlayerPets.Count > 0)));
		}
		if (_propertiesContainer.descriptionGadget != null)
		{
			_propertiesContainer.descriptionGadget.gameObject.SetActiveSafeSelf(CurrentItem != null && IsGadgetsCategory(CurrentItem.Category));
		}
		petUpgradesInSpecial.SetActiveSafeSelf(CurrentItem != null && CurrentItem.Category == CategoryNames.PetsCategory);
		if (!IsGridEmpty() && CurrentItem != null && (CurrentCategory != CategoryNames.PetsCategory || Singleton<PetsManager>.Instance.PlayerPets.Count != 0))
		{
			if (IsWeaponCategory(CurrentItem.Category))
			{
				WeaponSounds weaponSounds = null;
				WeaponSounds weaponSounds2 = null;
				weaponSounds = ItemDb.GetWeaponInfo(_viewedId);
				weaponSounds2 = ItemDb.GetWeaponInfo(_showForId_WEAPONS_AND_GADGET_ONLY);
				bool flag11 = false;
				bool haveAllUpgrades = false;
				int num = ((_viewedId == null) ? (-1) : CurrentNumberOfUpgradesForWeapon(_viewedId, out haveAllUpgrades, CurrentItem.Category));
				if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.IsAvailableTryGun(_viewedId))
				{
					num = 0;
				}
				flag11 = haveAllUpgrades && (!(WeaponManager.sharedManager != null) || !WeaponManager.sharedManager.IsAvailableTryGun(_viewedId));
				buyActive = _viewedId != null && !flag11 && num == 0 && !flag10 && flag9 && !InTrainingAfterNoviceArmorRemoved && (isFromMainWindow || _viewedId == _showForId_WEAPONS_AND_GADGET_ONLY);
				upgradeActive = _viewedId != null && !flag11 && num != 0 && weaponSounds2.tier < 100 && !flag10 && flag9 && !InTrainingAfterNoviceArmorRemoved;
				if (WeaponManager.sharedManager != null && _viewedId != null)
				{
					bool flag12 = WeaponManager.sharedManager.IsAvailableTryGun(_viewedId);
					bool flag13 = WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(_viewedId);
					flag = flag12 || flag13;
					if (flag)
					{
						try
						{
							if (_propertiesContainer.tryGunMatchesCount != null)
							{
								_propertiesContainer.tryGunMatchesCount.gameObject.SetActiveSafeSelf(flag12);
							}
							if (_propertiesContainer.tryGunDiscountTime != null)
							{
								_propertiesContainer.tryGunDiscountTime.gameObject.SetActiveSafeSelf(flag13);
							}
							if (flag12 && _propertiesContainer.tryGunMatchesCount != null)
							{
								_propertiesContainer.tryGunMatchesCount.text = ((SaltedInt)WeaponManager.sharedManager.TryGuns[_viewedId]["NumberOfMatchesKey"]).Value.ToString();
							}
							if (flag13)
							{
								UpdateTryGunDiscountTime(_propertiesContainer, _viewedId);
							}
						}
						catch (Exception ex)
						{
							Debug.LogError("Exception in tryGunMatchesCount.text: " + ex);
						}
					}
				}
				List<string> list = WeaponUpgrades.ChainForTag(_viewedId);
				state11 = _showForId_WEAPONS_AND_GADGET_ONLY != WeaponManager.FirstTagForOurTier(_showForId_WEAPONS_AND_GADGET_ONLY) && weaponSounds2 != null && ExpController.OurTierForAnyPlace() >= weaponSounds2.tier && !flag10 && flag9 && !InTrainingAfterNoviceArmorRemoved && list != null && list.Contains(_viewedId) && list.Contains(_showForId_WEAPONS_AND_GADGET_ONLY) && list.IndexOf(_showForId_WEAPONS_AND_GADGET_ONLY) > num;
				if (state11)
				{
					upgradeActive = false;
				}
				flag6 = (!isFromMainWindow || upgradeActive) && weaponSounds2 != null && ExpController.OurTierForAnyPlace() < weaponSounds2.tier && weaponSounds2.tier < 100 && !flag10 && flag9 && !InTrainingAfterNoviceArmorRemoved;
				if (flag6)
				{
					int num2 = ((weaponSounds2.tier < 0 || weaponSounds2.tier >= ExpController.LevelsForTiers.Length) ? ExpController.LevelsForTiers[ExpController.LevelsForTiers.Length - 1] : ExpController.LevelsForTiers[weaponSounds2.tier]);
					text2 = string.Format("{0} {1}", LocalizationStore.Get("Key_1073"), num2);
					upgradeActive = false;
				}
				if (_propertiesContainer.needBuyPrevious != null)
				{
					_propertiesContainer.needBuyPrevious.SetActive(state11);
					_propertiesContainer.needBuyPreviousLabel.text = LocalizationStore.Get("Key_2392");
				}
				string text4 = null;
				if (_viewedId != null)
				{
					text4 = WeaponManager.LastBoughtTag(_viewedId);
				}
				if (text4 == null && _viewedId != null && WeaponManager.sharedManager.IsAvailableTryGun(_viewedId))
				{
					text4 = _viewedId;
				}
				string text5 = _CurrentWeaponSetIDs()[(int)CurrentItem.Category];
				flag3 = text4 != null && _viewedId != null && !(text5 == text4) && _viewedId != null && (num > 0 || WeaponManager.sharedManager.IsAvailableTryGun(_viewedId)) && (flag9 || _viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || _viewedId == WeaponManager.LastBoughtTag("Armor_Army_1")) && (isFromMainWindow || _viewedId == _showForId_WEAPONS_AND_GADGET_ONLY);
				flag2 = !isFromMainWindow && !string.IsNullOrEmpty(_viewedId) && ((text5 != null && text5.Equals(WeaponManager.LastBoughtTag(_viewedId) ?? string.Empty)) || (WeaponManager.sharedManager.IsAvailableTryGun(_viewedId) && text5.Equals(_viewedId))) && (flag9 || _viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || _viewedId == WeaponManager.LastBoughtTag("Armor_Army_1")) && (isFromMainWindow || _viewedId == _showForId_WEAPONS_AND_GADGET_ONLY);
				if ((upgradeActive || flag10 || WeaponManager.sharedManager.IsAvailableTryGun(_viewedId)) && _viewedId != null && _showForId_WEAPONS_AND_GADGET_ONLY != null && (flag10 || !_showForId_WEAPONS_AND_GADGET_ONLY.Equals(_viewedId) || WeaponManager.sharedManager.IsAvailableTryGun(_viewedId)) && flag3)
				{
					flag3 = (flag9 || _viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || _viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
				}
				if ((upgradeActive || flag10 || WeaponManager.sharedManager.IsAvailableTryGun(_viewedId)) && _viewedId != null && _showForId_WEAPONS_AND_GADGET_ONLY != null && (flag10 || !_showForId_WEAPONS_AND_GADGET_ONLY.Equals(_viewedId) || WeaponManager.sharedManager.IsAvailableTryGun(_viewedId)) && flag2)
				{
					flag2 = (flag9 || _viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || _viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
				}
				if (!upgradeActive && !flag10 && !buyActive && flag3)
				{
					flag3 = (flag9 || _viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || _viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
				}
				if (!upgradeActive && !flag10 && !buyActive && flag2)
				{
					flag2 = (flag9 || _viewedId == WeaponTags.HunterRifleTag) && (!InTrainingAfterNoviceArmorRemoved || _viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
				}
				if (!isFromMainWindow && (flag2 || flag3))
				{
					upgradeActive = false;
				}
				bool onlyServerDiscount;
				int num3 = DiscountFor(_showForId_WEAPONS_AND_GADGET_ONLY, out onlyServerDiscount);
				if ((upgradeActive || buyActive) && num3 > 0)
				{
					saleActive = flag9 && !InTrainingAfterNoviceArmorRemoved;
					text = "-" + num3 + "%";
				}
				else
				{
					saleActive = false;
				}
				if (_propertiesContainer.discountPanel != null)
				{
					_propertiesContainer.discountPanel.SetActiveSafeSelf(saleActive);
				}
				if (_propertiesContainer.discountLabel != null)
				{
					_propertiesContainer.discountLabel.text = text;
				}
				if ((upgradeActive || buyActive) && _propertiesContainer.price != null)
				{
					_propertiesContainer.price.gameObject.SetActiveSafeSelf(true);
					_propertiesContainer.price.SetPrice(GetItemPrice(_viewedId, CurrentItem.Category));
				}
				else if (_propertiesContainer.price != null)
				{
					_propertiesContainer.price.gameObject.SetActiveSafeSelf(false);
				}
				if (_showForId_WEAPONS_AND_GADGET_ONLY != null)
				{
					state9 = (!weaponSounds.isMelee || weaponSounds.isShotMelee) && _viewedId != null;
					state10 = weaponSounds.isMelee && !weaponSounds.isShotMelee && _viewedId != null;
					if (weaponSounds2 == null)
					{
						weaponSounds2 = weaponSounds;
					}
					int[] array = null;
					array = ((!weaponSounds.isMelee || weaponSounds.isShotMelee) ? new int[4]
					{
						(!flag10) ? weaponSounds.damageShop : ((int)weaponSounds.DPS),
						weaponSounds.fireRateShop,
						weaponSounds.CapacityShop,
						weaponSounds.mobilityShop
					} : new int[3]
					{
						(!flag10) ? weaponSounds.damageShop : ((int)weaponSounds.DPS),
						weaponSounds.fireRateShop,
						weaponSounds.mobilityShop
					});
					int[] array2 = null;
					array2 = ((!weaponSounds.isMelee || weaponSounds.isShotMelee) ? new int[4]
					{
						(!flag10) ? weaponSounds2.damageShop : ((int)weaponSounds2.DPS),
						weaponSounds2.fireRateShop,
						weaponSounds2.CapacityShop,
						weaponSounds2.mobilityShop
					} : new int[3]
					{
						(!flag10) ? weaponSounds2.damageShop : ((int)weaponSounds2.DPS),
						weaponSounds2.fireRateShop,
						weaponSounds2.mobilityShop
					});
					int[] array3 = array2;
					if (weaponSounds.isMelee && !weaponSounds.isShotMelee)
					{
						_propertiesContainer.damageMelee.text = GetWeaponStatText(array[0], array3[0]);
						_propertiesContainer.fireRateMElee.text = GetWeaponStatText(array[1], array3[1]);
						_propertiesContainer.mobilityMelee.text = GetWeaponStatText(array[2], array3[2]);
					}
					else
					{
						_propertiesContainer.damage.text = GetWeaponStatText(array[0], array3[0]);
						_propertiesContainer.fireRate.text = GetWeaponStatText(array[1], array3[1]);
						_propertiesContainer.capacity.text = GetWeaponStatText(array[2], array3[2]);
						_propertiesContainer.mobility.text = GetWeaponStatText(array[3], array3[3]);
					}
					_propertiesContainer.specialParams.SetActiveSafeSelf(true);
					float num4 = 0.81500006f;
					WeaponSounds weaponSounds3 = weaponSounds2;
					if (weaponSounds3 != null)
					{
						if (_propertiesContainer.weaponsRarityLabel != null)
						{
							_propertiesContainer.weaponsRarityLabel.text = LocalizationStore.Get("Key_2393") + ": " + ItemDb.GetItemRarityLocalizeName(weaponSounds3.rarity);
						}
						for (int i = 0; i < _propertiesContainer.effectsLabels.Count; i++)
						{
							_propertiesContainer.effectsLabels[i].gameObject.SetActiveSafeSelf(i < weaponSounds3.InShopEffects.Count);
							if (i < weaponSounds3.InShopEffects.Count)
							{
								_propertiesContainer.effectsLabels[i].text = ((weaponSounds3.InShopEffects[i] != WeaponSounds.Effects.Zoom) ? string.Empty : (weaponSounds3.zoomShop + "X ")) + LocalizationStore.Get(WeaponSounds.keysAndSpritesForEffects[weaponSounds3.InShopEffects[i]].Value);
								_propertiesContainer.effectsSprites[i].spriteName = WeaponSounds.keysAndSpritesForEffects[weaponSounds3.InShopEffects[i]].Key;
								_propertiesContainer.effectsSprites[i].ResetAndUpdateAnchors();
							}
						}
						if (_propertiesContainer.specialTable != null)
						{
							_propertiesContainer.specialTable.Reposition();
						}
					}
				}
			}
			else
			{
				state9 = false;
				state10 = false;
				switch (CurrentItem.Category)
				{
				case CategoryNames.HatsCategory:
				case CategoryNames.ArmorCategory:
				case CategoryNames.CapesCategory:
				case CategoryNames.BootsCategory:
				case CategoryNames.MaskCategory:
				{
					string text7 = WeaponManager.LastBoughtTag(_viewedId);
					bool flag28 = text7 != null;
					bool flag29 = flag28 && WearForCat(CurrentItem.Category).Equals(text7);
					string text8 = WeaponManager.FirstUnboughtTag(_viewedId);
					SetUpUpgradesAndTiers(flag28, ref buyActive, ref upgradeActive, ref saleActive, ref needMoreTrophiesActive);
					flag3 = _viewedId != "Armor_Novice" && flag28 && !flag29 && text7 != null && text7.Equals(_viewedId) && (TutorialPhasePassed >= TutorialPhase.SelectWearCategory || TrainingController.TrainingCompleted) && (!InTrainingAfterNoviceArmorRemoved || _viewedId == WeaponManager.LastBoughtTag("Armor_Army_1"));
					state4 = _viewedId != "Armor_Novice" && flag28 && flag29 && text7 != null && text7.Equals(_viewedId) && flag9 && !InTrainingAfterNoviceArmorRemoved;
					flag2 = ((!isFromMainWindow || CurrentItem.Category == CategoryNames.ArmorCategory) && _viewedId == "Armor_Novice") || (text7 != null && WearForCat(CurrentItem.Category).Equals(text7) && !flag10);
					if (!flag28 && _viewedId != null && _viewedId.Equals("cape_Custom"))
					{
						state = !inGame && !Defs.isDaterRegim && flag9 && !InTrainingAfterNoviceArmorRemoved;
					}
					if (!inGame && flag28 && _viewedId != null && _viewedId.Equals("cape_Custom"))
					{
						state2 = !Defs.isDaterRegim && flag9 && !InTrainingAfterNoviceArmorRemoved;
					}
					flag3 = ((!upgradeActive && !flag10) ? (flag3 && _viewedId != null && text8 != null && (text7 == null || text7.Equals(text8) || !text8.Equals(_viewedId))) : (flag3 && (flag10 || (_viewedId != null && text8 != null && (text7 == null || text7.Equals(text8) || !text8.Equals(_viewedId))))));
					if (!(_viewedId == "cape_Custom") || !flag28)
					{
						flag2 = flag2 && (flag10 || (_viewedId != null && text8 != null && (text7 == null || text7.Equals(text8) || !text8.Equals(_viewedId))));
					}
					int num8 = Wear.TierForWear(WeaponManager.FirstUnboughtTag(_viewedId));
					flag6 = upgradeActive && ExpController.OurTierForAnyPlace() < num8 && !flag10 && flag9 && !InTrainingAfterNoviceArmorRemoved;
					if (flag6)
					{
						int num9 = ((num8 < 0 || num8 >= ExpController.LevelsForTiers.Length) ? ExpController.LevelsForTiers[ExpController.LevelsForTiers.Length - 1] : ExpController.LevelsForTiers[num8]);
						text2 = string.Format("{0} {1}", LocalizationStore.Get("Key_1073"), num9);
						upgradeActive = false;
					}
					if (_propertiesContainer.needBuyPrevious != null)
					{
						_propertiesContainer.needBuyPrevious.SetActiveSafeSelf(state11);
						_propertiesContainer.needBuyPreviousLabel.text = LocalizationStore.Get("Key_2392");
					}
					try
					{
						if (CurrentItem.Category == CategoryNames.ArmorCategory)
						{
							float f = Wear.armorNum[_viewedId];
							_propertiesContainer.armorCountLabel.text = ((text7 == null || !(_viewedId != text7)) ? f.ToString() : GetWeaponStatText(Mathf.RoundToInt(Wear.armorNum[text7]), Mathf.RoundToInt(f)));
							_propertiesContainer.armorCountLabel.gameObject.SetActiveSafeSelf(_viewedId != "Armor_Novice");
							_propertiesContainer.armorWearDescription.text = LocalizationStore.Get("Key_0354");
						}
						else
						{
							_propertiesContainer.nonArmorWearDEscription.text = LocalizationStore.Get(Wear.descriptionLocalizationKeys[_viewedId]);
						}
					}
					catch (Exception ex2)
					{
						Debug.LogError("Exception in setting desciption for wear: " + ex2);
					}
					break;
				}
				case CategoryNames.SkinsCategory:
				{
					state = false;
					bool flag23 = false;
					bool flag24 = _viewedId == "super_socialman";
					bool flag25 = false;
					if (_viewedId != "CustomSkinID")
					{
						bool isForMoneySkin = false;
						flag25 = SkinsController.IsSkinBought(_viewedId, out isForMoneySkin);
						bool flag26 = !SkinsController.IsLeagueSkinAvailableByLeague(_viewedId);
						buyActive = isForMoneySkin && !flag25 && !flag24 && flag9 && !InTrainingAfterNoviceArmorRemoved && !flag26;
						needMoreTrophiesActive = flag26 && (!isForMoneySkin || !flag25);
						upgradeActive = false;
						flag3 = (!isForMoneySkin || flag25) && _viewedId != SkinsController.currentSkinNameForPers && flag9 && !InTrainingAfterNoviceArmorRemoved;
						state4 = false;
						flag2 = !isFromMainWindow && (!isForMoneySkin || flag25) && _viewedId.Equals(SkinsController.currentSkinNameForPers);
						state5 = flag24 && !flag25 && flag9 && !InTrainingAfterNoviceArmorRemoved;
						bool flag27 = false;
						long result;
						flag27 = long.TryParse(_viewedId, out result) && result >= 1000000;
						state2 = !inGame && flag27 && !Defs.isDaterRegim && flag9 && !InTrainingAfterNoviceArmorRemoved;
						state3 = flag27 && flag9 && !InTrainingAfterNoviceArmorRemoved;
					}
					else
					{
						flag23 = Storager.getInt(Defs.SkinsMakerInProfileBought, true) > 0;
						state7 = !flag23 && flag9;
						state6 = !inGame && flag23 && flag9 && !InTrainingAfterNoviceArmorRemoved;
						state2 = false;
					}
					flag2 = flag2 && flag9 && !InTrainingAfterNoviceArmorRemoved;
					flag6 = false;
					break;
				}
				case CategoryNames.LeagueWeaponSkinsCategory:
				{
					state = false;
					bool flag20 = false;
					flag20 = WeaponSkinsManager.IsBoughtSkin(_viewedId);
					WeaponSkin skin = WeaponSkinsManager.GetSkin(_viewedId);
					bool flag21 = !WeaponSkinsManager.IsAvailableByLeague(_viewedId);
					buyActive = !flag20 && flag9 && !InTrainingAfterNoviceArmorRemoved && !flag21;
					needMoreTrophiesActive = flag21 && !flag20;
					upgradeActive = false;
					bool flag22 = flag20 && WeaponSkinsManager.GetSettedSkinId(skin.ToWeapons[0]) == _viewedId && flag9 && !InTrainingAfterNoviceArmorRemoved;
					flag2 = !isFromMainWindow && flag22;
					flag3 = flag20 && !flag22 && flag9 && !InTrainingAfterNoviceArmorRemoved;
					state4 = flag20 && flag22;
					state2 = false;
					state3 = false;
					flag6 = false;
					break;
				}
				case CategoryNames.PetsCategory:
				{
					state = false;
					state2 = false;
					state3 = false;
					flag6 = false;
					flag7 = false;
					buyActive = false;
					bool flag30 = Singleton<PetsManager>.Instance.PlayerPets.Count > 0;
					bool flag31 = Singleton<PetsManager>.Instance.GetEqipedPetId() == _viewedId && flag9 && !InTrainingAfterNoviceArmorRemoved;
					flag2 = !isFromMainWindow && flag31 && flag30;
					flag3 = !flag31 && flag9 && !InTrainingAfterNoviceArmorRemoved && flag30;
					state4 = flag31 && flag30;
					try
					{
						PetInfo petInfo = ((!flag30) ? null : Singleton<PetsManager>.Instance.GetNextUp(_viewedId));
						flag4 = petInfo != null && TrainingController.TrainingCompleted && flag30;
						flag7 = flag4 && (!Singleton<PetsManager>.Instance.NextUpAvailable(_viewedId) || petInfo.Tier > ExpController.OurTierForAnyPlace()) && flag9 && !InTrainingAfterNoviceArmorRemoved && flag30;
						if (flag7)
						{
							if (!Singleton<PetsManager>.Instance.NextUpAvailable(_viewedId))
							{
								text3 = string.Format("{0}", LocalizationStore.Get("Key_2541"));
							}
							else
							{
								int tier2 = petInfo.Tier;
								int num10 = ((tier2 < 0 || tier2 >= ExpController.LevelsForTiers.Length) ? ExpController.LevelsForTiers[ExpController.LevelsForTiers.Length - 1] : ExpController.LevelsForTiers[tier2]);
								text3 = string.Format("{0} {1}", LocalizationStore.Get("Key_1073"), num10);
							}
							flag4 = false;
						}
						float num11 = 1f;
						string textProgress;
						if (petInfo != null)
						{
							int toUpPoints = PetsManager.Infos[_viewedId].ToUpPoints;
							int points = Singleton<PetsManager>.Instance.GetPlayerPet(_viewedId).Points;
							num11 = (float)points / (float)toUpPoints;
							textProgress = LocalizationStore.Get("Key_2793") + " " + points + "/" + toUpPoints;
						}
						else
						{
							textProgress = LocalizationStore.Get("Key_2547");
						}
						num11 = ((!(num11 > 1f)) ? num11 : 1f);
						petUpgradeIndicator.transform.localScale = new Vector3(num11, 1f, 1f);
						petUpgradePointsLabels.ForEach(delegate(UILabel label)
						{
							label.text = textProgress;
						});
					}
					catch (Exception ex3)
					{
						Debug.LogErrorFormat("Exception in getting needTierPetActive for pets: {0}", ex3);
					}
					if (_propertiesContainer.gadgetsPropertiesList != null)
					{
						string key = ((!flag30) ? null : Singleton<PetsManager>.Instance.GetFirstUnboughtPet(_viewedId).Id);
						for (int l = 0; l < _propertiesContainer.gadgetsPropertiesList.Count; l++)
						{
							GadgetInfo.Parameter parameter2 = (GadgetInfo.Parameter)l;
							bool flag32 = flag30 && PetsInfo.info[_viewedId].Parameters.Contains(parameter2);
							_propertiesContainer.gadgetsPropertiesList[l].gameObject.SetActiveSafe(flag32);
							if (flag32)
							{
								int currentValue2 = 0;
								int nextValue2 = 0;
								switch (parameter2)
								{
								case GadgetInfo.Parameter.Attack:
									currentValue2 = Mathf.RoundToInt(PetsInfo.info[_viewedId].DPS);
									nextValue2 = Mathf.RoundToInt(PetsInfo.info[key].DPS);
									break;
								case GadgetInfo.Parameter.HP:
									currentValue2 = Mathf.RoundToInt(PetsInfo.info[_viewedId].HP);
									nextValue2 = Mathf.RoundToInt(PetsInfo.info[key].HP);
									break;
								case GadgetInfo.Parameter.Speed:
									currentValue2 = Mathf.RoundToInt(PetsInfo.info[_viewedId].SpeedModif);
									nextValue2 = Mathf.RoundToInt(PetsInfo.info[key].SpeedModif);
									break;
								case GadgetInfo.Parameter.Respawn:
									currentValue2 = Mathf.RoundToInt(PetsInfo.info[_viewedId].RespawnTime);
									nextValue2 = Mathf.RoundToInt(PetsInfo.info[key].RespawnTime);
									break;
								}
								_propertiesContainer.gadgetsPropertiesList[l].propertyLabel.text = GetWeaponStatText(currentValue2, nextValue2);
							}
						}
						if ((bool)_propertiesContainer.gadgetPropertyTable)
						{
							_propertiesContainer.gadgetPropertyTable.Reposition();
						}
					}
					if (_propertiesContainer.weaponsRarityLabel != null)
					{
						_propertiesContainer.weaponsRarityLabel.text = ((!flag30) ? string.Empty : (LocalizationStore.Get("Key_2393") + ": " + ItemDb.GetItemRarityLocalizeName(PetsInfo.info[_viewedId].Rarity)));
					}
					for (int m = 0; m < _propertiesContainer.effectsLabels.Count; m++)
					{
						bool flag33 = flag30 && m < PetsInfo.info[(!isFromMainWindow) ? _showForId_WEAPONS_AND_GADGET_ONLY : _viewedId].Effects.Count;
						_propertiesContainer.effectsLabels[m].gameObject.SetActiveSafeSelf(flag33);
						if (flag33)
						{
							_propertiesContainer.effectsLabels[m].text = LocalizationStore.Get(WeaponSounds.keysAndSpritesForEffects[PetsInfo.info[(!isFromMainWindow) ? _showForId_WEAPONS_AND_GADGET_ONLY : _viewedId].Effects[m]].Value);
							_propertiesContainer.effectsSprites[m].spriteName = WeaponSounds.keysAndSpritesForEffects[PetsInfo.info[(!isFromMainWindow) ? _showForId_WEAPONS_AND_GADGET_ONLY : _viewedId].Effects[m]].Key;
							_propertiesContainer.effectsSprites[m].ResetAndUpdateAnchors();
						}
					}
					if (_propertiesContainer.specialTable != null)
					{
						_propertiesContainer.specialTable.Reposition();
					}
					if (!flag30)
					{
						wearNameLabels.ForEach(delegate(UILabel label)
						{
							label.text = string.Empty;
						});
					}
					state8 = flag30;
					break;
				}
				case CategoryNames.EggsCategory:
					state = false;
					state2 = false;
					state3 = false;
					flag6 = false;
					buyActive = false;
					upgradeActive = false;
					flag2 = false;
					flag3 = false;
					state4 = false;
					if (_propertiesContainer.needTier != null && _propertiesContainer.needTierLabel != null)
					{
						_propertiesContainer.needTier.SetActiveSafeSelf(false);
					}
					if (_propertiesContainer.needBuyPrevious != null)
					{
						_propertiesContainer.needBuyPrevious.SetActiveSafeSelf(false);
					}
					flag5 = true;
					break;
				case CategoryNames.ThrowingCategory:
				case CategoryNames.ToolsCategoty:
				case CategoryNames.SupportCategory:
				{
					state = false;
					state2 = false;
					state3 = false;
					state4 = false;
					bool flag14 = false;
					string text6 = GadgetsInfo.LastBoughtFor(_viewedId);
					List<string> list2 = GadgetsInfo.Upgrades[_viewedId];
					int num5 = ((text6 != null) ? (list2.IndexOf(text6) + 1) : 0);
					flag14 = num5 == list2.Count;
					bool flag15 = list2.IndexOf(_showForId_WEAPONS_AND_GADGET_ONLY) < num5;
					int tier = GadgetsInfo.info[_showForId_WEAPONS_AND_GADGET_ONLY].Tier;
					bool flag16 = GadgetsInfo.info[_showForId_WEAPONS_AND_GADGET_ONLY].Tier <= ExpController.OurTierForAnyPlace();
					buyActive = num5 == 0 && flag9 && !InTrainingAfterNoviceArmorRemoved && (isFromMainWindow || _viewedId == _showForId_WEAPONS_AND_GADGET_ONLY);
					state11 = !buyActive && list2.IndexOf(_showForId_WEAPONS_AND_GADGET_ONLY) > num5 && flag9 && !InTrainingAfterNoviceArmorRemoved;
					upgradeActive = num5 > 0 && !flag14 && flag16 && !flag15 && !state11 && flag9 && !InTrainingAfterNoviceArmorRemoved;
					bool flag17 = GadgetsInfo.EquippedForCategory((GadgetInfo.GadgetCategory)CurrentItem.Category) == ((!isFromMainWindow) ? _showForId_WEAPONS_AND_GADGET_ONLY : _viewedId) && flag9 && !InTrainingAfterNoviceArmorRemoved;
					flag2 = !isFromMainWindow && flag17;
					flag3 = !flag17 && text6 != null && text6 == ((!isFromMainWindow) ? _showForId_WEAPONS_AND_GADGET_ONLY : _viewedId) && flag9 && !InTrainingAfterNoviceArmorRemoved;
					flag6 = !state11 && !flag16 && !flag15 && flag9 && !InTrainingAfterNoviceArmorRemoved;
					if (flag6)
					{
						int num6 = ((tier >= ExpController.LevelsForTiers.Length) ? ExpController.LevelsForTiers[ExpController.LevelsForTiers.Length - 1] : ExpController.LevelsForTiers[tier]);
						text2 = string.Format("{0} {1}", LocalizationStore.Get("Key_1073"), num6);
						upgradeActive = false;
					}
					if (_propertiesContainer.needBuyPrevious != null)
					{
						_propertiesContainer.needBuyPrevious.SetActiveSafeSelf(state11);
						_propertiesContainer.needBuyPreviousLabel.text = LocalizationStore.Get("Key_2502");
					}
					bool onlyServerDiscount2;
					int num7 = DiscountFor(_showForId_WEAPONS_AND_GADGET_ONLY, out onlyServerDiscount2);
					if ((upgradeActive || buyActive) && num7 > 0)
					{
						saleActive = flag9 && !InTrainingAfterNoviceArmorRemoved;
						text = "-" + num7 + "%";
					}
					else
					{
						saleActive = false;
					}
					if (_propertiesContainer.discountPanel != null)
					{
						_propertiesContainer.discountPanel.SetActiveSafeSelf(saleActive);
					}
					if (_propertiesContainer.discountLabel != null)
					{
						_propertiesContainer.discountLabel.text = text;
					}
					if ((upgradeActive || buyActive) && _propertiesContainer.price != null)
					{
						_propertiesContainer.price.gameObject.SetActiveSafeSelf(true);
						_propertiesContainer.price.SetPrice(GetItemPrice(_showForId_WEAPONS_AND_GADGET_ONLY, CurrentItem.Category));
					}
					else if (_propertiesContainer.price != null)
					{
						_propertiesContainer.price.gameObject.SetActiveSafeSelf(false);
					}
					if (_propertiesContainer.descriptionGadget != null)
					{
						_propertiesContainer.descriptionGadget.text = LocalizationStore.Get(GadgetsInfo.info[list2[0]].DescriptionLkey);
					}
					if (_propertiesContainer.gadgetsPropertiesList != null)
					{
						for (int j = 0; j < _propertiesContainer.gadgetsPropertiesList.Count; j++)
						{
							GadgetInfo.Parameter parameter = (GadgetInfo.Parameter)j;
							bool flag18 = GadgetsInfo.info[_viewedId].Parameters.Contains(parameter);
							_propertiesContainer.gadgetsPropertiesList[j].gameObject.SetActiveSafe(flag18);
							if (flag18)
							{
								int currentValue = 0;
								int nextValue = 0;
								switch (parameter)
								{
								case GadgetInfo.Parameter.Damage:
									currentValue = Mathf.RoundToInt(GadgetsInfo.info[_viewedId].DPS);
									nextValue = Mathf.RoundToInt(GadgetsInfo.info[_showForId_WEAPONS_AND_GADGET_ONLY].DPS);
									break;
								case GadgetInfo.Parameter.Cooldown:
									currentValue = Mathf.RoundToInt(GadgetsInfo.info[_viewedId].Cooldown);
									nextValue = Mathf.RoundToInt(GadgetsInfo.info[_showForId_WEAPONS_AND_GADGET_ONLY].Cooldown);
									break;
								case GadgetInfo.Parameter.Durability:
									currentValue = Mathf.RoundToInt(GadgetsInfo.info[_viewedId].Durability);
									nextValue = Mathf.RoundToInt(GadgetsInfo.info[_showForId_WEAPONS_AND_GADGET_ONLY].Durability);
									break;
								case GadgetInfo.Parameter.Lifetime:
									currentValue = Mathf.RoundToInt(GadgetsInfo.info[_viewedId].Duration);
									nextValue = Mathf.RoundToInt(GadgetsInfo.info[_showForId_WEAPONS_AND_GADGET_ONLY].Duration);
									break;
								case GadgetInfo.Parameter.Healing:
									currentValue = Mathf.RoundToInt(GadgetsInfo.info[_viewedId].Heal);
									nextValue = Mathf.RoundToInt(GadgetsInfo.info[_showForId_WEAPONS_AND_GADGET_ONLY].Heal);
									break;
								}
								_propertiesContainer.gadgetsPropertiesList[j].propertyLabel.text = GetWeaponStatText(currentValue, nextValue);
							}
						}
					}
					if ((bool)_propertiesContainer.gadgetPropertyTable)
					{
						_propertiesContainer.gadgetPropertyTable.Reposition();
					}
					for (int k = 0; k < _propertiesContainer.effectsLabels.Count; k++)
					{
						bool flag19 = k < GadgetsInfo.info[(!isFromMainWindow) ? _showForId_WEAPONS_AND_GADGET_ONLY : CurrentItem.Id].Effects.Count;
						_propertiesContainer.effectsLabels[k].gameObject.SetActiveSafeSelf(flag19);
						if (flag19)
						{
							_propertiesContainer.effectsLabels[k].text = LocalizationStore.Get(WeaponSounds.keysAndSpritesForEffects[GadgetsInfo.info[(!isFromMainWindow) ? _showForId_WEAPONS_AND_GADGET_ONLY : CurrentItem.Id].Effects[k]].Value);
							_propertiesContainer.effectsSprites[k].spriteName = WeaponSounds.keysAndSpritesForEffects[GadgetsInfo.info[(!isFromMainWindow) ? _showForId_WEAPONS_AND_GADGET_ONLY : CurrentItem.Id].Effects[k]].Key;
							_propertiesContainer.effectsSprites[k].ResetAndUpdateAnchors();
						}
					}
					if (_propertiesContainer.specialTable != null)
					{
						_propertiesContainer.specialTable.Reposition();
					}
					break;
				}
				}
			}
		}
		else if (IsBestCategory(CurrentCategory))
		{
			flag8 = true;
		}
		else if (CurrentCategory == CategoryNames.EggsCategory)
		{
			flag5 = true;
		}
		if (_propertiesContainer.tryGunPanel != null)
		{
			_propertiesContainer.tryGunPanel.SetActiveSafeSelf(flag);
			UIPanel component = gridScrollView.GetComponent<UIPanel>();
			if (flag)
			{
				component.bottomAnchor.Set(component.bottomAnchor.relative, itemScrollBottomAnchorRent);
				itemsGrid.transform.localPosition = new Vector3(itemsGrid.transform.localPosition.x, itemsGrid.transform.localPosition.y + 25f, itemsGrid.transform.localPosition.z);
				updateScrollViewOnLateUpdateForTryPanel = true;
			}
			else if (component.bottomAnchor.absolute != itemScrollBottomAnchor)
			{
				component.bottomAnchor.Set(component.bottomAnchor.relative, itemScrollBottomAnchor);
				updateScrollViewOnLateUpdateForTryPanel = true;
			}
		}
		if (_propertiesContainer.needTier != null && _propertiesContainer.needTierLabel != null)
		{
			_propertiesContainer.needTier.SetActiveSafeSelf(flag6);
			_propertiesContainer.needTierLabel.text = text2;
		}
		if (_propertiesContainer.needTierPet != null && _propertiesContainer.needTierPetLabel != null)
		{
			_propertiesContainer.needTierPet.SetActiveSafeSelf(flag7);
			_propertiesContainer.needTierPetLabel.text = text3;
		}
		if (_propertiesContainer.renamePetButton != null)
		{
			_propertiesContainer.renamePetButton.gameObject.SetActiveSafeSelf(state8);
		}
		if (_propertiesContainer.editButton != null)
		{
			_propertiesContainer.editButton.gameObject.SetActiveSafeSelf(state2);
		}
		if (_propertiesContainer.enableButton != null)
		{
			_propertiesContainer.enableButton.gameObject.SetActiveSafeSelf(state);
		}
		if (_propertiesContainer.deleteButton != null)
		{
			_propertiesContainer.deleteButton.gameObject.SetActiveSafeSelf(state3);
		}
		if (_propertiesContainer.buyButton != null)
		{
			_propertiesContainer.buyButton.gameObject.SetActiveSafeSelf(buyActive);
		}
		if (_propertiesContainer.equipButton != null)
		{
			_propertiesContainer.equipButton.gameObject.SetActiveSafeSelf(flag3);
		}
		if (_propertiesContainer.unequipButton != null)
		{
			_propertiesContainer.unequipButton.gameObject.SetActiveSafeSelf(state4);
		}
		if (_propertiesContainer.upgradeButton != null)
		{
			_propertiesContainer.upgradeButton.gameObject.SetActiveSafeSelf(upgradeActive);
		}
		if (_propertiesContainer.trainButton != null)
		{
			_propertiesContainer.trainButton.gameObject.SetActiveSafeSelf(flag4);
		}
		if (facebookLoginLockedSkinButton != null)
		{
			facebookLoginLockedSkinButton.gameObject.SetActiveSafeSelf(state5);
		}
		if (unlockButton != null)
		{
			unlockButton.gameObject.SetActiveSafeSelf(state7);
		}
		if (_propertiesContainer.needMoreTrophiesPanel != null)
		{
			_propertiesContainer.needMoreTrophiesPanel.SetActiveSafeSelf(needMoreTrophiesActive);
		}
		superIncubatorButton.SetActiveSafeSelf(flag5 && TrainingController.TrainingCompleted);
		if (_propertiesContainer.equipped != null)
		{
			_propertiesContainer.equipped.SetActiveSafeSelf(flag2);
		}
		if (createButton != null)
		{
			createButton.gameObject.SetActiveSafeSelf(state6);
		}
		_propertiesContainer.weaponProperties.SetActiveSafeSelf(state9);
		_propertiesContainer.meleeProperties.SetActiveSafeSelf(state10);
		bool flag34 = false;
		if (_propertiesContainer.tryGun != null)
		{
			_propertiesContainer.tryGun.gameObject.SetActiveSafeSelf(false);
		}
	}

	public static int DiscountFor(string itemTag, out bool onlyServerDiscount)
	{
		//Discarded unreachable code: IL_0182, IL_01a5
		try
		{
			if (itemTag == null)
			{
				Debug.LogError("DiscountFor: itemTag == null");
				onlyServerDiscount = false;
				return 0;
			}
			bool flag = false;
			bool flag2 = false;
			float num = 100f;
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(itemTag))
			{
				long num2 = WeaponManager.sharedManager.DiscountForTryGun(itemTag);
				num -= (float)num2;
				num = Math.Max(1f, num);
				num = Math.Min(100f, num);
				flag2 = true;
			}
			num /= 100f;
			float num3 = 100f;
			if (!flag2 && PromoActionsManager.sharedManager.discounts.ContainsKey(itemTag) && PromoActionsManager.sharedManager.discounts[itemTag].Count > 0)
			{
				num3 -= (float)PromoActionsManager.sharedManager.discounts[itemTag][0].Value;
				num3 = Math.Max(10f, num3);
				num3 = Math.Min(100f, num3);
				flag = true;
			}
			num3 /= 100f;
			onlyServerDiscount = !flag2 && flag;
			if (!flag2 && !flag)
			{
				return 0;
			}
			float value = num * num3;
			value = Mathf.Clamp(value, 0.01f, 1f);
			float f = (1f - value) * 100f;
			int num4 = Mathf.RoundToInt(f);
			if (onlyServerDiscount && num4 % 5 != 0)
			{
				num4 = 5 * (num4 / 5 + 1);
			}
			return Math.Min(num4, 99);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in DiscountFor: " + ex);
			onlyServerDiscount = false;
			return 0;
		}
	}

	public static ItemPrice GetItemPrice(string itemId, CategoryNames category, bool upgradeNotBuyGear = false, bool useDiscounts = true, bool itemIdIsFirstUnbought = false)
	{
		//Discarded unreachable code: IL_0209, IL_0233
		try
		{
			if (itemId == null)
			{
				return new ItemPrice(0, "Coins");
			}
			string text = string.Empty;
			if (itemIdIsFirstUnbought)
			{
				text = itemId;
			}
			else if (IsWeaponCategory(category) || IsWearCategory(category))
			{
				text = WeaponManager.FirstUnboughtOrForOurTier(itemId);
			}
			else if (IsGadgetsCategory(category))
			{
				text = GadgetsInfo.FirstUnboughtOrForOurTier(itemId);
			}
			else if (category == CategoryNames.PetsCategory)
			{
				PetInfo firstUnboughtPet = Singleton<PetsManager>.Instance.GetFirstUnboughtPet(itemId);
				text = ((firstUnboughtPet == null) ? itemId : firstUnboughtPet.Id);
			}
			string text2 = itemId;
			if (itemId != null && WeaponManager.tagToStoreIDMapping.ContainsKey(itemId))
			{
				text2 = WeaponManager.tagToStoreIDMapping[text];
			}
			if (category == CategoryNames.GearCategory)
			{
				text2 = ((!upgradeNotBuyGear) ? GearManager.OneItemIDForGear(GearManager.HolderQuantityForID(text2), GearManager.CurrentNumberOfUphradesForGear(text2)) : GearManager.UpgradeIDForGear(GearManager.HolderQuantityForID(text2), GearManager.CurrentNumberOfUphradesForGear(text2) + 1));
			}
			if (IsWearCategory(category) || IsGadgetsCategory(category) || category == CategoryNames.PetsCategory)
			{
				text2 = text;
			}
			string itemTag = ((!IsWeaponCategory(category) && !IsWearCategory(category) && !IsGadgetsCategory(category) && category != CategoryNames.PetsCategory) ? itemId : text);
			ItemPrice itemPrice = ItemDb.GetPriceByShopId(text2, category) ?? new ItemPrice(10, "Coins");
			int num = itemPrice.Price;
			if (useDiscounts)
			{
				bool onlyServerDiscount;
				int num2 = DiscountFor(itemTag, out onlyServerDiscount);
				if (num2 > 0)
				{
					float num3 = num2;
					num = Math.Max((int)((float)num * 0.01f), Mathf.RoundToInt((float)num * (1f - num3 / 100f)));
					if (onlyServerDiscount)
					{
						num = ((num % 5 >= 3) ? (num + (5 - num % 5)) : (num - num % 5));
					}
				}
			}
			if (category == CategoryNames.GearCategory && !upgradeNotBuyGear)
			{
				num *= GearManager.ItemsInPackForGear(GearManager.HolderQuantityForID(text2));
			}
			return new ItemPrice(num, itemPrice.Currency);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in currentPrice: " + ex);
			return new ItemPrice(0, "Coins");
		}
	}

	public static int PriceIfGunWillBeTryGun(string tg)
	{
		return Mathf.RoundToInt((float)GetItemPrice(tg, (CategoryNames)ItemDb.GetItemCategory(tg), false, false).Price * ((float)WeaponManager.BaseTryGunDiscount() / 100f));
	}

	public static int CurrentNumberOfUpgradesForWear(string id, out bool maxUpgrade, CategoryNames c, out int totalNumberOfUpgrades)
	{
		if (id == "Armor_Novice")
		{
			maxUpgrade = NoviceArmorAvailable;
			totalNumberOfUpgrades = 1;
			return NoviceArmorAvailable ? 1 : 0;
		}
		List<string> list = Wear.wear[c].FirstOrDefault((List<string> l) => l.Contains(id));
		if (list == null)
		{
			maxUpgrade = false;
			totalNumberOfUpgrades = 1;
			return 0;
		}
		totalNumberOfUpgrades = list.Count;
		for (int i = 0; i < list.Count; i++)
		{
			if (Storager.getInt(list[i], true) == 0)
			{
				maxUpgrade = false;
				return i;
			}
		}
		maxUpgrade = true;
		return list.Count;
	}

	private static int CurrentNumberOfUpgradesForWeapon(string weaponId, out bool haveAllUpgrades, CategoryNames c, bool countTryGunsAsUpgrade = true)
	{
		List<string> list = WeaponUpgrades.ChainForTag(weaponId);
		if (list == null)
		{
			List<string> list2 = new List<string>();
			list2.Add(weaponId);
			list = list2;
		}
		List<string> list3 = list;
		int num = list3.Count;
		if (WeaponManager.tagToStoreIDMapping.ContainsKey(weaponId))
		{
			int num2 = list3.Count - 1;
			while (num2 >= 0)
			{
				string defName = weaponId;
				bool flag = ItemDb.IsTemporaryGun(weaponId);
				if (!flag)
				{
					defName = WeaponManager.storeIDtoDefsSNMapping[WeaponManager.tagToStoreIDMapping[list3[num2]]];
				}
				bool flag2 = HasBoughtGood(defName, flag);
				if (!flag2 && countTryGunsAsUpgrade && WeaponManager.sharedManager != null)
				{
					flag2 = WeaponManager.sharedManager.IsAvailableTryGun(list3[num2]);
				}
				if (!flag2)
				{
					num--;
					num2--;
					continue;
				}
				break;
			}
		}
		haveAllUpgrades = num == ((list3.Count <= 0) ? 1 : list3.Count);
		return num;
	}

	private static bool HasBoughtGood(string defName, bool tempGun = false)
	{
		bool flag = ((!tempGun) ? (Storager.getInt(defName, true) == 0) : (!TempItemsController.sharedController.ContainsItem(defName)));
		return !flag;
	}

	public void UpdatePersWithNewItem(ShopItem itemToSet)
	{
		if (IsWeaponCategory(CurrentItem.Category))
		{
			if (itemToSet == null && WeaponManager.sharedManager.playerWeapons.Count > 0)
			{
				string id = ItemDb.GetByPrefabName((WeaponManager.sharedManager.playerWeapons[0] as Weapon).weaponPrefab.name.Replace("(Clone)", string.Empty)).Tag;
				itemToSet = new ShopItem(id, CurrentItem.Category);
			}
			SetWeapon(itemToSet.Id, null);
			return;
		}
		if (IsGadgetsCategory(CurrentItem.Category))
		{
			UpdatePersGadget(GadgetsInfo.BaseName(itemToSet.Id));
			return;
		}
		switch (CurrentItem.Category)
		{
		case CategoryNames.HatsCategory:
			UpdatePersHat(itemToSet.Id);
			break;
		case CategoryNames.SkinsCategory:
			if (itemToSet.Id != "CustomSkinID")
			{
				UpdatePersSkin(itemToSet.Id);
			}
			break;
		case CategoryNames.PetsCategory:
			UpdatePersPet(itemToSet.Id);
			break;
		case CategoryNames.CapesCategory:
			UpdatePersCape(itemToSet.Id);
			break;
		case CategoryNames.MaskCategory:
			UpdatePersMask(itemToSet.Id);
			break;
		case CategoryNames.BootsCategory:
			UpdatePersBoots(itemToSet.Id);
			break;
		case CategoryNames.ArmorCategory:
			UpdatePersArmor(itemToSet.Id);
			break;
		case CategoryNames.LeagueWeaponSkinsCategory:
			if (itemToSet != null)
			{
				WeaponSkin skin = WeaponSkinsManager.GetSkin(itemToSet.Id);
				if (skin != null)
				{
					ItemRecord byPrefabName = ItemDb.GetByPrefabName(skin.ToWeapons[0]);
					if (byPrefabName != null)
					{
						SetWeapon(byPrefabName.Tag, itemToSet.Id);
					}
				}
				break;
			}
			try
			{
				string weaponTag = null;
				if (WeaponManager.sharedManager.playerWeapons.Count > 0)
				{
					weaponTag = ItemDb.GetByPrefabName((WeaponManager.sharedManager.playerWeapons[0] as Weapon).weaponPrefab.nameNoClone()).Tag;
				}
				SetWeapon(weaponTag, null);
				break;
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in defaultWeaponAfterWeaponSkinsCategory: " + ex);
				break;
			}
		}
	}

	public void UpdatePersPet(string pet)
	{
		StopPetAnimation();
		characterInterface.UpdatePet(pet);
		PlayPetAnimation();
	}

	public void UpdatePersHat(string hat)
	{
		characterInterface.UpdateHat(hat, !ShowWear);
		SetPersHatVisible(hatPoint);
	}

	public void UpdatePersArmor(string armor)
	{
		if (armorPoint.childCount > 0)
		{
			Transform child = armorPoint.GetChild(0);
			ArmorRefs component = child.GetChild(0).GetComponent<ArmorRefs>();
			if (component != null)
			{
				if (component.leftBone != null)
				{
					component.leftBone.parent = child.GetChild(0);
				}
				if (component.rightBone != null)
				{
					component.rightBone.parent = child.GetChild(0);
				}
				child.parent = null;
				child.position = new Vector3(0f, -10000f, 0f);
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
		if (armor.Equals(Defs.ArmorNewNoneEqupped))
		{
			return;
		}
		string @string = Storager.getString(Defs.VisualArmor, false);
		if (!string.IsNullOrEmpty(@string) && Wear.wear[CategoryNames.ArmorCategory][0].IndexOf(armor) >= 0 && Wear.wear[CategoryNames.ArmorCategory][0].IndexOf(armor) < Wear.wear[CategoryNames.ArmorCategory][0].IndexOf(@string))
		{
			armor = @string;
		}
		if (!(weapon != null))
		{
			return;
		}
		GameObject gameObject = Resources.Load("Armor_Shop/" + armor) as GameObject;
		if (gameObject == null)
		{
			return;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		DisableLightProbesRecursively(gameObject2);
		ArmorRefs component2 = gameObject2.transform.GetChild(0).GetComponent<ArmorRefs>();
		if (component2 != null)
		{
			WeaponSounds component3 = weapon.GetComponent<WeaponSounds>();
			gameObject2.transform.parent = armorPoint.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
			Player_move_c.SetLayerRecursively(gameObject2, LayerMask.NameToLayer("NGUIShop"));
			if (component2.leftBone != null && component3.LeftArmorHand != null)
			{
				component2.leftBone.parent = component3.LeftArmorHand;
				component2.leftBone.localPosition = Vector3.zero;
				component2.leftBone.localRotation = Quaternion.identity;
				component2.leftBone.localScale = new Vector3(1f, 1f, 1f);
			}
			if (component2.rightBone != null && component3.RightArmorHand != null)
			{
				component2.rightBone.parent = component3.RightArmorHand;
				component2.rightBone.localPosition = Vector3.zero;
				component2.rightBone.localRotation = Quaternion.identity;
				component2.rightBone.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		SetPersArmorVisible(armorPoint);
	}

	public void UpdatePersMask(string mask)
	{
		characterInterface.UpdateMask(mask, !ShowWear);
	}

	public void UpdatePersCape(string cape)
	{
		characterInterface.UpdateCape(cape, null, !ShowWear);
	}

	public void UpdatePersSkin(string skinId)
	{
		if (skinId == null)
		{
			Debug.LogError("Skin id should not be null!");
		}
		else
		{
			SetSkinOnPers(SkinsController.skinsForPers[skinId]);
		}
	}

	public void SetSkinOnPers(Texture skin)
	{
		WeaponSounds weaponSounds = ((body.transform.childCount <= 0) ? null : body.transform.GetChild(0).GetComponent<WeaponSounds>());
		GadgetArmoryItem gadget = ((!(weaponSounds == null) || body.transform.childCount <= 0) ? null : body.transform.GetChild(0).GetComponent<GadgetArmoryItem>());
		characterInterface.SetSkin(skin, weaponSounds, gadget);
	}

	public void UpdatePersBoots(string bs)
	{
		characterInterface.UpdateBoots(bs, !ShowWear);
	}

	public void UpdatePersGadget(string idGadget)
	{
		animationCoroutineRunner.StopAllCoroutines();
		if (WeaponManager.sharedManager == null || idGadget == null)
		{
			return;
		}
		GameObject gameObject = Resources.Load<GameObject>("GadgetsContent/gadget_shop_preview/" + idGadget);
		if (gameObject == null)
		{
			Debug.Log("pref==null");
			return;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		GadgetArmoryItem component = gameObject2.GetComponent<GadgetArmoryItem>();
		if (gadgetPreview != null)
		{
			UnityEngine.Object.Destroy(gadgetPreview);
			gadgetPreview = null;
		}
		if (component.isReplaceOnlyHands)
		{
			characterInterface.skinCharacter.SetActive(true);
			if (armorPoint.childCount > 0)
			{
				ArmorRefs component2 = armorPoint.GetChild(0).GetChild(0).GetComponent<ArmorRefs>();
				if (component2 != null)
				{
					if (component2.leftBone != null)
					{
						Vector3 position = component2.leftBone.position;
						Quaternion rotation = component2.leftBone.rotation;
						component2.leftBone.parent = armorPoint.GetChild(0).GetChild(0);
						component2.leftBone.position = position;
						component2.leftBone.rotation = rotation;
					}
					if (component2.rightBone != null)
					{
						Vector3 position2 = component2.rightBone.position;
						Quaternion rotation2 = component2.rightBone.rotation;
						component2.rightBone.parent = armorPoint.GetChild(0).GetChild(0);
						component2.rightBone.position = position2;
						component2.rightBone.rotation = rotation2;
					}
				}
			}
			List<Transform> list = new List<Transform>();
			foreach (Transform item in body.transform)
			{
				list.Add(item);
			}
			foreach (Transform item2 in list)
			{
				item2.parent = null;
				item2.position = new Vector3(0f, -10000f, 0f);
				UnityEngine.Object.Destroy(item2.gameObject);
			}
			DisableLightProbesRecursively(gameObject2);
			gameObject2.transform.parent = body.transform;
			weapon = gameObject2;
			weapon.transform.localScale = new Vector3(1f, 1f, 1f);
			weapon.transform.position = body.transform.position;
			weapon.transform.localPosition = Vector3.zero;
			weapon.transform.localRotation = Quaternion.identity;
			if (armorPoint.childCount > 0 && component != null)
			{
				ArmorRefs component3 = armorPoint.GetChild(0).GetChild(0).GetComponent<ArmorRefs>();
				if (component3 != null)
				{
					if (component3.leftBone != null && component.LeftArmorHand != null)
					{
						component3.leftBone.parent = component.LeftArmorHand;
						component3.leftBone.localPosition = Vector3.zero;
						component3.leftBone.localRotation = Quaternion.identity;
						component3.leftBone.localScale = new Vector3(1f, 1f, 1f);
					}
					if (component3.rightBone != null && component.RightArmorHand != null)
					{
						component3.rightBone.parent = component.RightArmorHand;
						component3.rightBone.localPosition = Vector3.zero;
						component3.rightBone.localRotation = Quaternion.identity;
						component3.rightBone.localScale = new Vector3(1f, 1f, 1f);
					}
				}
			}
			if (SkinsController.currentSkinForPers != null)
			{
				SetSkinOnPers(SkinsController.currentSkinForPers);
			}
		}
		else
		{
			gadgetPreview = gameObject2;
			characterInterface.skinCharacter.SetActive(false);
			gadgetPreview.transform.SetParent(characterInterface.transform);
			gadgetPreview.transform.localScale = new Vector3(1f, 1f, 1f);
			gadgetPreview.transform.position = body.transform.position;
			gadgetPreview.transform.localPosition = Vector3.zero;
			gadgetPreview.transform.localRotation = Quaternion.identity;
		}
		Player_move_c.SetLayerRecursively(gameObject2, LayerMask.NameToLayer("NGUIShop"));
		DisableLightProbesRecursively(gameObject2.gameObject);
	}

	public void ReloadCategoryTempItemsRemoved(List<string> expired)
	{
	}

	private void ReturnPersWearAndSkinWhenSwitching()
	{
		if (CurrentItem == null)
		{
			return;
		}
		string id = string.Empty;
		switch (CurrentItem.Category)
		{
		case CategoryNames.SkinsCategory:
			if (SkinsController.currentSkinNameForPers != null)
			{
				id = SkinsController.currentSkinNameForPers;
			}
			else if (SkinsController.skinsForPers != null && SkinsController.skinsForPers.Keys.Count > 0)
			{
				id = SkinsController.skinsForPers.Keys.FirstOrDefault();
			}
			break;
		case CategoryNames.HatsCategory:
		case CategoryNames.ArmorCategory:
		case CategoryNames.CapesCategory:
		case CategoryNames.BootsCategory:
		case CategoryNames.MaskCategory:
			id = WearForCat(CurrentItem.Category);
			break;
		case CategoryNames.LeagueWeaponSkinsCategory:
		case CategoryNames.PetsCategory:
			try
			{
				id = Singleton<PetsManager>.Instance.GetEqipedPetId();
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in getting equipped pet id: {0}", ex);
			}
			break;
		}
		if (IsWearCategory(CurrentItem.Category) || CurrentItem.Category == CategoryNames.SkinsCategory || CurrentItem.Category == CategoryNames.LeagueWeaponSkinsCategory || CurrentItem.Category == CategoryNames.PetsCategory)
		{
			UpdatePersWithNewItem(new ShopItem(id, CurrentItem.Category));
		}
	}

	private CategoryNames CategoryForSuperCategory(BtnCategory superCategory)
	{
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted && superCategory.btnName == Supercategory.Wear.ToString())
		{
			try
			{
				CategoryNames[] array = new CategoryNames[5]
				{
					CategoryNames.MaskCategory,
					CategoryNames.HatsCategory,
					CategoryNames.CapesCategory,
					CategoryNames.SkinsCategory,
					CategoryNames.BootsCategory
				};
				foreach (CategoryNames category in array)
				{
					GameObject go = TransformOfButtonForCategory(category).gameObject;
					UIWidget component = go.GetChildGameObject("Pressed").GetComponent<UIWidget>();
					component.alpha = 0f;
				}
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in CategoryForSuperCategory, tutorial: {0}", ex);
			}
			return CategoryNames.ArmorCategory;
		}
		if (superCategory == null || superCategory.btnName == null)
		{
			Debug.LogError("CategoryForSuperCategory superCategory == null || superCategory.btnName == null");
			return CategoryNames.PrimaryCategory;
		}
		return supercategoryLastUsedCategory[(Supercategory)(int)Enum.Parse(typeof(Supercategory), superCategory.btnName)];
	}

	private IEnumerable<Transform> AllCategoryButtonTransforms()
	{
		return weaponCategoriesGrid.GetChildList().Concat(wearCategoriesGrid.GetChildList()).Concat(gridCategoriesLeague.GetChildList())
			.Concat(gadgetsGrid.GetChildList())
			.Concat(petsGrid.GetChildList())
			.Concat(bestCategoriesGrid.GetChildList());
	}

	private Transform TransformOfButtonForCategory(CategoryNames category)
	{
		return AllCategoryButtonTransforms().FirstOrDefault((Transform c) => c.name == category.ToString());
	}

	private void SetToggleForCategory(CategoryNames category)
	{
		Transform transform = TransformOfButtonForCategory(category);
		if (transform != null)
		{
			UIToggle component = transform.GetComponent<UIToggle>();
			if (component != null)
			{
				List<EventDelegate> onChange = component.onChange;
				component.onChange = new List<EventDelegate>();
				bool instantTween = component.instantTween;
				component.instantTween = true;
				component.value = true;
				component.onChange = onChange;
				component.instantTween = instantTween;
			}
			else
			{
				Debug.LogError("ChooseCategoryAndSuperCategory: uIToggle == null: category: " + category);
			}
		}
	}

	public static int CurrentNumberOfUnlockedItems()
	{
		//Discarded unreachable code: IL_0056, IL_0077
		try
		{
			Dictionary<Supercategory, Dictionary<CategoryNames, List<string>>> dictionary = UnlockedItemsByCategoriesForCurrentShop();
			return dictionary.Values.SelectMany((Dictionary<CategoryNames, List<string>> itemsDict) => itemsDict.Values).Sum((List<string> list) => list.Count);
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in CurrentNumberOfUnlockedItems: {0}", ex);
			return 0;
		}
	}

	private static Dictionary<Supercategory, Dictionary<CategoryNames, List<string>>> UnlockedItemsByCategoriesForCurrentShop()
	{
		if (PromoActionsManager.sharedManager == null)
		{
			Debug.LogErrorFormat("UnlockedItemsByCategoriesForCurrentShop: PromoActionsManager.sharedManager == null");
			return null;
		}
		Dictionary<Supercategory, Dictionary<CategoryNames, List<string>>> dictionary = new Dictionary<Supercategory, Dictionary<CategoryNames, List<string>>>();
		dictionary.Add(Supercategory.Weapons, WeaponManager.GetWeaponTagsByCategoriesFromItems(PromoActionsManager.sharedManager.UnlockedItems).ToDictionary((KeyValuePair<CategoryNames, List<string>> kvp) => kvp.Key, (KeyValuePair<CategoryNames, List<string>> kvp) => kvp.Value.Intersect(from item in GetItemNamesList(kvp.Key)
			select item.Id).ToList()));
		dictionary.Add(Supercategory.Gadgets, GadgetsInfo.GetGadgetsByCategoriesFromItems(PromoActionsManager.sharedManager.UnlockedItems).ToDictionary((KeyValuePair<GadgetInfo.GadgetCategory, List<string>> kvp) => (CategoryNames)kvp.Key, (KeyValuePair<GadgetInfo.GadgetCategory, List<string>> kvp) => kvp.Value.Intersect(from item in GetItemNamesList((CategoryNames)kvp.Key)
			select item.Id).ToList()));
		return dictionary;
	}

	private void UpdateUnlockedItemsIndicators()
	{
		if (PromoActionsManager.sharedManager == null)
		{
			Debug.LogErrorFormat("UpdateUnlockedItems: PromoActionsManager.sharedManager == null");
			return;
		}
		try
		{
			Dictionary<Supercategory, Dictionary<CategoryNames, List<string>>> dictionary = UnlockedItemsByCategoriesForCurrentShop();
			IEnumerable<CategoryNames> enumerable = Enum.GetValues(typeof(CategoryNames)).OfType<CategoryNames>();
			Dictionary<CategoryNames, List<string>> dictionary2 = dictionary.Values.SelectMany((Dictionary<CategoryNames, List<string>> itemsDict) => itemsDict).ToDictionary((KeyValuePair<CategoryNames, List<string>> kvp) => kvp.Key, (KeyValuePair<CategoryNames, List<string>> kvp) => kvp.Value);
			foreach (CategoryNames item in enumerable)
			{
				List<UILabel> value;
				if (!m_categoriesToUnlockedItemsLabels.TryGetValue(item, out value))
				{
					continue;
				}
				List<string> value2;
				if (!dictionary2.TryGetValue(item, out value2) || value2 == null)
				{
					value2 = new List<string>();
				}
				int numberToDisplay = value2.Count();
				bool flag = numberToDisplay > 0;
				value[0].transform.parent.gameObject.SetActiveSafeSelf(flag);
				if (flag)
				{
					value.ForEach(delegate(UILabel label)
					{
						label.text = numberToDisplay.ToString();
					});
				}
			}
			foreach (Supercategory key in dictionary.Keys)
			{
				int numOfItemsInSupercategory = dictionary[key].Values.Sum((List<string> items) => items.Count);
				bool flag2 = numOfItemsInSupercategory > 0;
				List<UILabel> list = m_supercategoriesToUnlockedItemsLabels[key];
				list[0].transform.parent.gameObject.SetActiveSafeSelf(flag2);
				if (flag2)
				{
					list.ForEach(delegate(UILabel label)
					{
						label.text = numOfItemsInSupercategory.ToString();
					});
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in UpdateUnlockedItems: {0}", ex);
		}
	}

	private static void RemoveViewedUnlockedItems()
	{
		try
		{
			PromoActionsManager.sharedManager.RemoveViewedUnlockedItems();
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in UpdateUnlockedItems in removing unlocked items and saving: {0}", ex);
		}
	}

	public void SuperCategoryChoosen(BtnCategory superCategory)
	{
		CategoryNames categoryNames = CategoryForSuperCategory(superCategory);
		BestItemsToRemoveOnLeave.Clear();
		RemoveViewedUnlockedItems();
		SetToggleForCategory(categoryNames);
		ChooseCategory(categoryNames);
	}

	public void CategoryChoosen(UIToggle toggle)
	{
		if (toggle.value)
		{
			CategoryNames newCategory = (CategoryNames)(int)Enum.Parse(typeof(CategoryNames), toggle.gameObject.name, true);
			BestItemsToRemoveOnLeave.Clear();
			RemoveViewedUnlockedItems();
			ChooseCategory(newCategory);
		}
	}

	public static bool IsLeagueCategory(CategoryNames category)
	{
		return category == CategoryNames.LeagueHatsCategory || category == CategoryNames.LeagueSkinsCategory || category == CategoryNames.LeagueWeaponSkinsCategory;
	}

	public static bool IsPetsOrEggsCategory(CategoryNames category)
	{
		return category == CategoryNames.PetsCategory || category == CategoryNames.EggsCategory;
	}

	public static bool IsGadgetsCategory(CategoryNames category)
	{
		return category == CategoryNames.ThrowingCategory || category == CategoryNames.SupportCategory || category == CategoryNames.ToolsCategoty;
	}

	public static bool IsBestCategory(CategoryNames category)
	{
		return category == CategoryNames.BestWeapons || category == CategoryNames.BestWear || category == CategoryNames.BestGadgets;
	}

	public void ChooseCategoryAndSuperCategory(CategoryNames category, ShopItem itemToSet, bool initial)
	{
		try
		{
			superCategoriesButtonController.BtnClicked(IsWeaponCategory(category) ? Supercategory.Weapons.ToString() : (IsWearCategory(category) ? Supercategory.Wear.ToString() : (IsLeagueCategory(category) ? Supercategory.League.ToString() : (IsPetsOrEggsCategory(category) ? Supercategory.Pets.ToString() : (IsGadgetsCategory(category) ? Supercategory.Gadgets.ToString() : ((category != CategoryNames.SkinsCategory) ? Supercategory.Best.ToString() : Supercategory.Wear.ToString()))))), true);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in superCategoriesButtonController.BtnClicked: " + ex);
		}
		SetToggleForCategory(category);
		ChooseCategory(category, itemToSet, initial);
	}

	public void ReloadGridOrCarousel(ShopItem item)
	{
		if (CurrentCategory == CategoryNames.ArmorCategory)
		{
			ReloadCarousel(item);
		}
		else
		{
			if (CurrentCategory != CategoryNames.SniperCategory)
			{
				gridScrollView.enabled = true;
			}
			ReloadItemGrid(item);
		}
		UpdateTutorialState();
	}

	private static IEnumerable<string> CurrentWeaponSkinIds()
	{
		List<string> sortedSkinIds = WeaponSkinsManager.AllSkins.Select((WeaponSkin skin) => skin.Id).ToList();
		return from skin in WeaponSkinsManager.BoughtSkins().Union(WeaponSkinsManager.GetAvailableForBuySkins())
			orderby sortedSkinIds.IndexOf(skin.Id)
			select skin.Id;
	}

	private void UpdateEggLabels()
	{
	}

	public void ShowLockOrPropertiesAndButtons()
	{
	}

	private bool SetAppropeiateStateForPropertiesPanel()
	{
		bool state = false;
		if (CurrentCategory == CategoryNames.PetsCategory && Singleton<PetsManager>.Instance.PlayerPets.Count == 0)
		{
			state = false;
		}
		else if (CurrentItem != null)
		{
			state = NeedToShowPropertiesInCategory(CurrentItem.Category);
		}
		bool isHidden = hideButton.isHidden;
		hideButton.SetState(state);
		return hideButton.isHidden != isHidden;
	}

	public void UpdatePetsCategoryIfNeeded()
	{
		if (CurrentCategory == CategoryNames.PetsCategory)
		{
			ChooseCategory(CurrentCategory);
		}
	}

	private void UpdateSkinShaderParams()
	{
		if (CurrentCategory == CategoryNames.SkinsCategory || CurrentCategory == CategoryNames.CapesCategory || CurrentCategory == CategoryNames.BestWear || CurrentCategory == CategoryNames.LeagueSkinsCategory)
		{
			UpdatePointsForSkinsShader();
			SetArmoryCellShaderParams();
		}
	}

	private void UpdateVisibilityOfPropertiesPanelAndButtons()
	{
		bool flag = false;
		bool state = false;
		if (CurrentItem != null)
		{
			CategoryNames category = CurrentItem.Category;
			flag = (category != CategoryNames.ArmorCategory && category != CategoryNames.SkinsCategory && category != CategoryNames.LeagueWeaponSkinsCategory && !IsWearCategory(category) && category != CategoryNames.PetsCategory && (CurrentCategory != CategoryNames.PetsCategory || Singleton<PetsManager>.Instance.PlayerPets.Count != 0)) || CurrentCategory == CategoryNames.EggsCategory;
			if (IsEmptyBestCategory())
			{
				flag = false;
			}
			state = ((flag && CurrentItem.Category != CategoryNames.EggsCategory) || (category == CategoryNames.PetsCategory && Singleton<PetsManager>.Instance.PlayerPets.Count > 0) || (IsWearCategory(CurrentItem.Category) && CurrentItem.Category != CategoryNames.ArmorCategory)) && TrainingController.TrainingCompleted && CurrentCategory != CategoryNames.EggsCategory;
			if (IsEmptyBestCategory())
			{
				state = false;
			}
		}
		else
		{
			flag = CurrentCategory == CategoryNames.EggsCategory;
		}
		if (infoButton != null)
		{
			infoButton.gameObject.SetActiveSafeSelf(flag);
		}
		if (hideButton != null)
		{
			hideButton.gameObject.SetActiveSafeSelf(state);
		}
		bool flag2 = SetAppropeiateStateForPropertiesPanel();
		if (gridScrollView.panel != null)
		{
			gridScrollView.panel.ResetAndUpdateAnchors();
		}
		if (flag2)
		{
			UpdateSkinShaderParams();
		}
	}

	private bool IsEmptyBestCategory()
	{
		return IsBestCategory(CurrentCategory) && IsGridEmpty();
	}

	public void ChooseCategory(CategoryNames newCategory, ShopItem itemToSet = null, bool initial = false)
	{
		sharedShop.armorLock.SetActiveSafeSelf(Defs.isHunger && SceneManager.GetActiveScene().name != "ConnectScene" && newCategory == CategoryNames.ArmorCategory);
		supercategoryLastUsedCategory[CurrentSupercategory] = newCategory;
		weaponCategoriesGrid.gameObject.SetActiveSafeSelf(IsWeaponCategory(newCategory));
		wearCategoriesGrid.gameObject.SetActiveSafeSelf(IsWearCategory(newCategory) || (newCategory == CategoryNames.SkinsCategory && newCategory != CategoryNames.LeagueSkinsCategory));
		armorCarousel.SetActiveSafeSelf(newCategory == CategoryNames.ArmorCategory);
		gridCategoriesLeague.gameObject.SetActiveSafeSelf(newCategory == CategoryNames.LeagueHatsCategory || newCategory == CategoryNames.LeagueSkinsCategory || newCategory == CategoryNames.LeagueWeaponSkinsCategory);
		gadgetsGrid.gameObject.SetActiveSafeSelf(IsGadgetsCategory(newCategory));
		gadgetBlocker.SetActiveSafeSelf((IsGadgetsCategory(newCategory) || newCategory == CategoryNames.BestGadgets) && WeaponManager.sharedManager.myPlayer != null);
		petsGrid.gameObject.SetActiveSafeSelf(IsPetsOrEggsCategory(newCategory));
		bestCategoriesGrid.gameObject.SetActiveSafeSelf(newCategory == CategoryNames.BestWeapons || newCategory == CategoryNames.BestWear || newCategory == CategoryNames.BestGadgets);
		gridScrollView.gameObject.SetActiveSafeSelf(newCategory != CategoryNames.ArmorCategory);
		gridSlider.SetActiveSafeSelf(newCategory != CategoryNames.ArmorCategory);
		try
		{
			int count = Singleton<EggsManager>.Instance.GetPlayerEggsInIncubator().Count;
			noEggs.SetActiveSafeSelf(newCategory == CategoryNames.EggsCategory && count == 0);
			panelProperties.SetActiveSafeSelf(newCategory != CategoryNames.EggsCategory);
			returnEveryDay.SetActiveSafeSelf(newCategory == CategoryNames.EggsCategory);
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in setting activity of noEggs: {0}", ex);
		}
		WeaponManager.ClearCachedInnerPrefabs();
		if (!initial)
		{
			ReturnPersWearAndSkinWhenSwitching();
		}
		CurrentCategory = newCategory;
		if (!IsWeaponCategory(CurrentCategory) && (weapon == null || gadgetPreview != null || (!IsGadgetsCategory(CurrentCategory) && CurrentCategory != CategoryNames.BestGadgets && weapon.GetComponent<WeaponSounds>() == null)) && CurrentCategory != CategoryNames.BestWeapons)
		{
			SetWeapon(_CurrentWeaponSetIDs()[0] ?? WeaponManager._initialWeaponName, null);
		}
		if (IsWeaponCategory(CurrentCategory))
		{
			string text = _CurrentWeaponSetIDs()[(int)CurrentCategory];
			if (itemToSet != null && text != null && itemToSet.Id == text && WeaponManager.sharedManager != null && WeaponManager.sharedManager.FilteredShopListsNoUpgrades.Count > (int)CurrentCategory)
			{
				ItemRecord rec = ItemDb.GetByTag(itemToSet.Id);
				if (rec != null && WeaponManager.sharedManager.FilteredShopListsNoUpgrades[(int)CurrentCategory].FirstOrDefault((GameObject go) => go.name == rec.PrefabName) == null)
				{
					itemToSet = null;
				}
			}
			if (itemToSet == null)
			{
				if (text != null)
				{
					itemToSet = new ShopItem(text, CurrentCategory);
				}
				else
				{
					CategoryNames inFactCategory;
					string id = HighestDPSGun(CurrentCategory, out inFactCategory);
					if (CurrentCategory == inFactCategory)
					{
						itemToSet = new ShopItem(id, CurrentCategory);
					}
				}
			}
		}
		else
		{
			switch (CurrentCategory)
			{
			case CategoryNames.LeagueHatsCategory:
			{
				Dictionary<Wear.LeagueItemState, List<string>> dictionary2 = Wear.LeagueItems();
				string text12 = WearForCat(CategoryNames.HatsCategory);
				string text13 = ((!(text12 != NoneEquippedForWearCategory(CategoryNames.HatsCategory)) || !dictionary2[Wear.LeagueItemState.Purchased].Contains(text12) || WeaponManager.LastBoughtTag(text12) == null || !(WeaponManager.LastBoughtTag(text12) == text12)) ? (from item in dictionary2[Wear.LeagueItemState.Open].Union(dictionary2[Wear.LeagueItemState.Purchased])
					orderby Wear.LeagueForWear(item, MapShopCategoryToItemCategory(CurrentCategory))
					select item).FirstOrDefault() : text12);
				if (!text13.IsNullOrEmpty())
				{
					itemToSet = new ShopItem(text13, CategoryNames.HatsCategory);
				}
				break;
			}
			case CategoryNames.HatsCategory:
				if (itemToSet == null)
				{
					Dictionary<Wear.LeagueItemState, List<string>> dictionary = Wear.LeagueItems();
					string text5 = WearForCat(CategoryNames.HatsCategory);
					string text6 = ((!(text5 != NoneEquippedForWearCategory(CategoryNames.HatsCategory)) || dictionary[Wear.LeagueItemState.Purchased].Contains(text5) || WeaponManager.LastBoughtTag(text5) == null || !(WeaponManager.LastBoughtTag(text5) == text5)) ? hats.FirstOrDefault((ShopPositionParams hat) => hat.name == "hat_Headphones").name : text5);
					if (!text6.IsNullOrEmpty())
					{
						itemToSet = new ShopItem(text6, CategoryNames.HatsCategory);
					}
				}
				break;
			case CategoryNames.ArmorCategory:
				if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
				{
					itemToSet = new ShopItem(WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1", CategoryNames.ArmorCategory);
				}
				if (InTrainingAfterNoviceArmorRemoved)
				{
					itemToSet = new ShopItem(WeaponManager.LastBoughtTag("Armor_Army_1") ?? "Armor_Army_1", CategoryNames.ArmorCategory);
				}
				else if (itemToSet == null)
				{
					string text2 = WearForCat(CategoryNames.ArmorCategory);
					string text3 = ((!(text2 != NoneEquippedForWearCategory(CategoryNames.ArmorCategory)) || WeaponManager.LastBoughtTag(text2) == null || !WeaponManager.LastBoughtTag(text2).Equals(text2)) ? WeaponManager.FirstUnboughtTag(Wear.wear[CategoryNames.ArmorCategory][0][0]) : text2);
					if (!text3.IsNullOrEmpty())
					{
						itemToSet = new ShopItem(text3, CategoryNames.ArmorCategory);
					}
				}
				scrollViewPanel.transform.localPosition = Vector3.zero;
				scrollViewPanel.clipOffset = new Vector2(0f, 0f);
				break;
			case CategoryNames.LeagueSkinsCategory:
				if (itemToSet == null)
				{
					string text14 = null;
					text14 = ((SkinsController.currentSkinNameForPers == null || !SkinsController.leagueSkinsIds.Contains(SkinsController.currentSkinNameForPers)) ? SkinsController.leagueSkinsIds.FirstOrDefault() : SkinsController.currentSkinNameForPers);
					if (!text14.IsNullOrEmpty())
					{
						itemToSet = new ShopItem(text14, CategoryNames.SkinsCategory);
					}
				}
				break;
			case CategoryNames.SkinsCategory:
				if (itemToSet == null)
				{
					itemToSet = new ShopItem("CustomSkinID", CategoryNames.SkinsCategory);
				}
				break;
			case CategoryNames.CapesCategory:
			case CategoryNames.BootsCategory:
			case CategoryNames.MaskCategory:
				if (itemToSet == null)
				{
					List<ShopPositionParams> list = ((CurrentCategory == CategoryNames.CapesCategory) ? capes : ((CurrentCategory != CategoryNames.BootsCategory) ? masks : boots));
					int index = 0;
					string text10 = WearForCat(CurrentCategory);
					string text11 = ((NoneEquippedForWearCategory(CurrentCategory) == null || !(text10 != NoneEquippedForWearCategory(CurrentCategory)) || WeaponManager.LastBoughtTag(text10) == null || !(WeaponManager.LastBoughtTag(text10) == text10) || CurrentCategory == CategoryNames.CapesCategory) ? (WeaponManager.LastBoughtTag(list[index].name) ?? list[index].name) : text10);
					if (!text11.IsNullOrEmpty())
					{
						itemToSet = new ShopItem(text11, CurrentCategory);
					}
				}
				break;
			case CategoryNames.LeagueWeaponSkinsCategory:
				if (itemToSet == null)
				{
					IEnumerable<string> source = CurrentWeaponSkinIds();
					string text4 = source.FirstOrDefault();
					if (!text4.IsNullOrEmpty())
					{
						itemToSet = new ShopItem(text4, CategoryNames.LeagueWeaponSkinsCategory);
					}
				}
				break;
			case CategoryNames.PetsCategory:
			{
				if (itemToSet != null)
				{
					break;
				}
				string text7 = Singleton<PetsManager>.Instance.GetEqipedPetId();
				if (string.IsNullOrEmpty(text7))
				{
					text7 = Singleton<PetsManager>.Instance.PlayerPets.Select((PlayerPet playerPet) => playerPet.InfoId).FirstOrDefault();
				}
				if (!text7.IsNullOrEmpty())
				{
					itemToSet = new ShopItem(text7, CategoryNames.PetsCategory);
				}
				break;
			}
			case CategoryNames.EggsCategory:
				if (itemToSet != null)
				{
					break;
				}
				try
				{
					Egg egg = Singleton<EggsManager>.Instance.GetPlayerEggsInIncubator().FirstOrDefault();
					if (egg != null && egg.Data != null)
					{
						itemToSet = new ShopItem(egg.Id.ToString(), CategoryNames.EggsCategory);
					}
				}
				catch (Exception ex2)
				{
					Debug.LogErrorFormat("Exception in setting idToSet for eggs category: {0}", ex2);
				}
				break;
			case CategoryNames.ThrowingCategory:
			case CategoryNames.ToolsCategoty:
			case CategoryNames.SupportCategory:
				if (itemToSet == null)
				{
					string text8 = GadgetsInfo.EquippedForCategory((GadgetInfo.GadgetCategory)CurrentCategory);
					string text9 = ((!text8.IsNullOrEmpty()) ? text8 : GetItemNamesList(CurrentCategory).FirstOrDefault().Id);
					if (!text9.IsNullOrEmpty())
					{
						itemToSet = new ShopItem(text9, CurrentCategory);
					}
				}
				break;
			case CategoryNames.BestWeapons:
			case CategoryNames.BestWear:
			case CategoryNames.BestGadgets:
			{
				List<ShopItem> itemNamesList = GetItemNamesList(CurrentCategory);
				if (itemToSet == null || !itemNamesList.Contains(itemToSet))
				{
					ShopItem shopItem = itemNamesList.FirstOrDefault();
					if (shopItem != null)
					{
						itemToSet = new ShopItem(shopItem.Id, shopItem.Category);
					}
					else
					{
						SetWeapon(_CurrentWeaponSetIDs()[0] ?? WeaponManager._initialWeaponName, null);
					}
				}
				break;
			}
			}
		}
		ReloadGridOrCarousel(itemToSet);
		ShowLockOrPropertiesAndButtons();
		needRefreshInLateUpdate = 2;
		bool flag = IsEmptyBestCategory();
		if (CurrentCategory == CategoryNames.EggsCategory || CurrentCategory == CategoryNames.PetsCategory || flag)
		{
			UpdateButtons();
		}
		if (flag || (CurrentCategory == CategoryNames.PetsCategory && Singleton<PetsManager>.Instance.PlayerPets.Count == 0))
		{
			ClearCaption();
		}
		if (noOffersAtThisTime != null)
		{
			noOffersAtThisTime.SetActiveSafeSelf(IsEmptyBestCategory());
		}
	}

	private void UpdatePropertiesPanels()
	{
		propertiesContainer.armorWearProperties.SetActiveSafeSelf(CurrentItem != null && CurrentItem.Category == CategoryNames.ArmorCategory);
		propertiesContainer.nonArmorWearProperties.SetActiveSafeSelf(CurrentItem != null && IsWearCategory(CurrentItem.Category) && CurrentItem.Category != CategoryNames.ArmorCategory);
		if (propertiesContainer.skinProperties != null)
		{
			propertiesContainer.skinProperties.SetActiveSafeSelf(CurrentItem != null && (CurrentItem.Category == CategoryNames.SkinsCategory || CurrentItem.Category == CategoryNames.LeagueWeaponSkinsCategory));
		}
	}

	public static void SetRenderersVisibleFromPoint(Transform pt, bool showArmor)
	{
		Player_move_c.PerformActionRecurs(pt.gameObject, delegate(Transform t)
		{
			Renderer component = t.GetComponent<Renderer>();
			if (component != null)
			{
				WearInvisbleParams component2 = t.GetComponent<WearInvisbleParams>();
				if (component2 != null)
				{
					if (!component2.SkipSetInvisible)
					{
						if (component2.HideIsInvisible)
						{
							t.gameObject.SetActive(showArmor);
						}
						else if (showArmor)
						{
							component.material.shader = ((!component2.BaseShader.IsNullOrEmpty()) ? Shader.Find(component2.BaseShader) : Shader.Find("Mobile/Diffuse"));
						}
						else
						{
							component.material.shader = ((!component2.InvisibleShader.IsNullOrEmpty()) ? Shader.Find(component2.InvisibleShader) : Shader.Find("Mobile/Transparent-Shop"));
						}
					}
				}
				else
				{
					component.material.shader = Shader.Find((!showArmor) ? "Mobile/Transparent-Shop" : "Mobile/Diffuse");
				}
			}
		});
	}

	private void PetsManager_Instance_OnPlayerPetAdded(string petId)
	{
		if (!GuiActive)
		{
			return;
		}
		EggHatchingWindowController[] componentsInChildren = rentScreenPoint.GetComponentsInChildren<EggHatchingWindowController>();
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			EggHatchingWindowController eggHatchingWindowController = componentsInChildren.FirstOrDefault();
			if (eggHatchingWindowController != null && eggHatchingWindowController.CurrentWindowMode == EggHatchingWindowController.WindowMode.Hatching)
			{
				return;
			}
		}
		if (CurrentCategory == CategoryNames.PetsCategory)
		{
			ChooseCategory(CurrentCategory);
		}
		else
		{
			SetPetsCategoryEnable();
		}
	}

	private void Awake()
	{
		m_supercategoriesToUnlockedItemsLabels = new Dictionary<Supercategory, List<UILabel>>
		{
			{
				Supercategory.Weapons,
				weaponSupercategoryUnlockedItems
			},
			{
				Supercategory.Gadgets,
				gadgetsSupercategoryUnlockedItems
			}
		};
		m_categoriesToUnlockedItemsLabels = new Dictionary<CategoryNames, List<UILabel>>
		{
			{
				CategoryNames.PrimaryCategory,
				primaryWeaponsUnlockedItems
			},
			{
				CategoryNames.BackupCategory,
				backupWeaponsUnlockedItems
			},
			{
				CategoryNames.MeleeCategory,
				meleeWeaponsUnlockedItems
			},
			{
				CategoryNames.SpecilCategory,
				specialWeaponsUnlockedItems
			},
			{
				CategoryNames.SniperCategory,
				sniperWeaponsUnlockedItems
			},
			{
				CategoryNames.PremiumCategory,
				premiumWeaponsUnlockedItems
			},
			{
				CategoryNames.ThrowingCategory,
				throwingGadgetsUnlockedItems
			},
			{
				CategoryNames.ToolsCategoty,
				toolsGadgetsUnlockedItems
			},
			{
				CategoryNames.SupportCategory,
				supportGadgetsUnlockedItems
			}
		};
		EggsManager.OnReadyToUse += EggsManager_OnReadyToUse;
		Singleton<PetsManager>.Instance.OnPlayerPetAdded += PetsManager_Instance_OnPlayerPetAdded;
		ShopCategoryButton.CategoryButtonClicked += delegate(ShopCategoryButton obj)
		{
			CategoryChoosen(obj.GetComponent<UIToggle>());
		};
		ArmoryCell.ToggleValueChanged += delegate(ArmoryCell cell)
		{
			if (cell.ItemId != CurrentItem.Id)
			{
				ChooseItem(new ShopItem(cell.ItemId, cell.Category));
			}
		};
		ArmoryCell.Clicked += delegate(ArmoryCell cell)
		{
			if (!(cell.ItemId != CurrentItem.Id) && CurrentItem.Category != CategoryNames.EggsCategory && ((!IsGadgetsCategory(CurrentItem.Category) && CurrentCategory != CategoryNames.BestGadgets) || !(WeaponManager.sharedManager.myPlayerMoveC != null)))
			{
				HandleInfoButton();
			}
		};
		superCategoriesButtonController.actions.AddRange(new List<Action<BtnCategory>> { SuperCategoryChoosen, SuperCategoryChoosen, SuperCategoryChoosen, SuperCategoryChoosen, SuperCategoryChoosen, SuperCategoryChoosen, SuperCategoryChoosen });
		CreateCharacterModel();
		_rotationRateForCharacter = RilisoftRotator.RotationRateForCharacterInMenues;
		if (Application.isEditor && BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			_rotationRateForCharacter *= 200f;
		}
		_touchZoneForRotatingCharacter = new Rect(0f, 0.1f * (float)Screen.height, 0.5f * (float)Screen.width, 0.8f * (float)Screen.height);
		_showArmorValue = PlayerPrefs.GetInt("ShowArmorKeySetting", 1) == 1;
		_showWearValue = PlayerPrefs.GetInt("ShowWearKeySetting", 1) == 1;
		HOTween.Init(true, true, true);
		HOTween.EnableOverwriteManager(true);
		m_timeOfPLastStuffUpdate = Time.realtimeSinceStartup;
		if (tryGun != null)
		{
			tryGun.GetComponent<ButtonHandler>().Clicked += delegate
			{
				if (Storager.getInt("tutorial_button_try_highlighted", false) == 0)
				{
					Storager.setInt("tutorial_button_try_highlighted", 1, false);
				}
			};
		}
		List<EventDelegate> onChange = showArmorButton.onChange;
		showArmorButton.onChange = new List<EventDelegate>();
		bool instantTween = showArmorButton.instantTween;
		showArmorButton.instantTween = true;
		showArmorButton.Set(ShowArmor);
		showArmorButton.onChange = onChange;
		showArmorButton.instantTween = instantTween;
		List<EventDelegate> onChange2 = showWearButton.onChange;
		showWearButton.onChange = new List<EventDelegate>();
		bool instantTween2 = showWearButton.instantTween;
		showWearButton.instantTween = true;
		showWearButton.Set(ShowWear);
		showWearButton.onChange = onChange2;
		showWearButton.instantTween = instantTween2;
		sharedShop = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		ActiveObject.SetActive(false);
		if (coinShopButton != null)
		{
			coinShopButton.GetComponent<ButtonHandler>().Clicked += delegate
			{
				if (!(Time.realtimeSinceStartup - timeOfEnteringShopForProtectFromPressingCoinsButton < 0.5f) && BankController.Instance != null)
				{
					if (BankController.Instance.InterfaceEnabledCoroutineLocked)
					{
						Debug.LogWarning("InterfaceEnabledCoroutineLocked");
					}
					else
					{
						EventHandler handleBackFromBank = null;
						handleBackFromBank = delegate
						{
							if (BankController.Instance.InterfaceEnabledCoroutineLocked)
							{
								Debug.LogWarning("InterfaceEnabledCoroutineLocked");
							}
							else
							{
								BankController.Instance.BackRequested -= handleBackFromBank;
								BankController.Instance.InterfaceEnabled = false;
								m_itemToSetAfterEnter = CurrentItem;
								CategoryToChoose = CurrentCategory;
								GuiActive = true;
								if (CurrentItem != null)
								{
									UpdatePersWithNewItem(CurrentItem);
								}
							}
						};
						BankController.Instance.BackRequested += handleBackFromBank;
						BankController.Instance.InterfaceEnabled = true;
						GuiActive = false;
					}
				}
			};
		}
		if (backButton != null)
		{
			backButton.GetComponent<ButtonHandler>().Clicked += delegate
			{
				if (BankController.Instance != null && BankController.Instance.InterfaceEnabled)
				{
					Debug.LogErrorFormat("trying to press back in shop when in bank");
				}
				else
				{
					StartCoroutine(BackAfterDelay());
				}
			};
		}
		hats.AddRange(Resources.LoadAll<ShopPositionParams>("Hats_Info"));
		sort(hats, CategoryNames.HatsCategory);
		armor.AddRange(Resources.LoadAll<ShopPositionParams>("Armor_Info"));
		sort(armor, CategoryNames.ArmorCategory);
		capes.AddRange(Resources.LoadAll<ShopPositionParams>("Capes_Info"));
		sort(capes, CategoryNames.CapesCategory);
		masks.AddRange(Resources.LoadAll<ShopPositionParams>("Masks_Info"));
		sort(masks, CategoryNames.MaskCategory);
		boots.AddRange(Resources.LoadAll<ShopPositionParams>("Shop_Boots_Info"));
		sort(boots, CategoryNames.BootsCategory);
		pixlMan = Resources.Load<GameObject>("Character_model");
		if (!Device.IsLoweMemoryDevice)
		{
			_onPersArmorRefs = Resources.LoadAll<GameObject>("Armor_Shop");
		}
		if (Device.isPixelGunLow)
		{
			_refOnLowPolyArmor = Resources.Load<GameObject>("Armor_Low");
			_refsOnLowPolyArmorMaterials = Resources.LoadAll<Material>("LowPolyArmorMaterials");
		}
		Storager.SubscribeToChanged(Defs.TrainingCompleted_4_4_Sett, OnTrainingCompleted_4_4_Sett_Changed);
	}

	private void EggsManager_OnReadyToUse(Egg egg)
	{
		if (GuiActive && CurrentCategory == CategoryNames.EggsCategory)
		{
			ChooseCategory(CategoryNames.EggsCategory);
		}
	}

	public void GoToSkinsEditor()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("SkinEditorController"));
		SkinEditorController component = gameObject.GetComponent<SkinEditorController>();
		if (!(component != null))
		{
			return;
		}
		Action<string> backHandler = null;
		backHandler = delegate(string newSkin)
		{
			SkinEditorController.ExitFromSkinEditor -= backHandler;
			MenuBackgroundMusic.sharedMusic.StopCustomMusicFrom(SkinEditorController.sharedController.gameObject);
			mainPanel.SetActive(true);
			ShowGridOrArmorCarousel();
			if (CurrentCategory == CategoryNames.CapesCategory || newSkin != null)
			{
				if (CurrentItem.Id == "CustomSkinID")
				{
					SetSkinAsCurrent(newSkin);
				}
				if (CurrentCategory == CategoryNames.SkinsCategory && CurrentItem.Id == SkinsController.currentSkinNameForPers)
				{
					FireOnEquipSkin(newSkin);
				}
				if (CurrentItem.Id == "cape_Custom")
				{
					EquipWear("cape_Custom");
				}
				ReloadAfterEditing(newSkin);
			}
			else
			{
				ReloadAfterEditing(newSkin);
			}
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC == null)
			{
				if (CurrentCategory != CategoryNames.CapesCategory && PlayerPrefs.GetInt(Defs.ShownRewardWindowForSkin, 0) == 0)
				{
					_shouldShowRewardWindowSkin = true;
				}
				if (CurrentCategory == CategoryNames.CapesCategory && PlayerPrefs.GetInt(Defs.ShownRewardWindowForCape, 0) == 0)
				{
					_shouldShowRewardWindowCape = true;
				}
			}
		};
		SkinEditorController.ExitFromSkinEditor += backHandler;
		SkinEditorController.currentSkinName = ((!(CurrentItem.Id == "CustomSkinID")) ? CurrentItem.Id : null);
		SkinEditorController.modeEditor = ((MapShopCategoryToItemCategory(CurrentCategory) != CategoryNames.SkinsCategory) ? SkinEditorController.ModeEditor.Cape : SkinEditorController.ModeEditor.SkinPers);
		mainPanel.SetActive(false);
	}

	public void ReloadAfterEditing(string n)
	{
		string id = n ?? ((CurrentCategory != CategoryNames.SkinsCategory) ? "cape_Custom" : (CurrentItem.Id ?? "CustomSkinID"));
		ReloadGridOrCarousel(new ShopItem(id, CurrentCategory));
		PlayPersAnimations();
		UpdateIcons(false);
	}

	private static List<Camera> BankRelatedCameras()
	{
		List<Camera> list = BankController.Instance.GetComponentsInChildren<Camera>(true).ToList();
		if (FreeAwardController.Instance != null && FreeAwardController.Instance.renderCamera != null)
		{
			list.Add(FreeAwardController.Instance.renderCamera);
		}
		return list;
	}

	public static void SetBankCamerasEnabled()
	{
		List<Camera> list = BankRelatedCameras();
		foreach (Camera item in list)
		{
			if (!(item == null))
			{
				item.enabled = true;
				if (item.GetComponent<UICamera>() != null)
				{
					item.GetComponent<UICamera>().enabled = true;
				}
				if (disablesCameras.Contains(item))
				{
					disablesCameras.Remove(item);
				}
			}
		}
	}

	public void ShowGridOrArmorCarousel()
	{
		if (CurrentCategory == CategoryNames.ArmorCategory)
		{
			if (!armorCarousel.activeSelf)
			{
				armorCarousel.SetActive(true);
			}
		}
		else if (!gridScrollView.gameObject.activeSelf)
		{
			gridScrollView.gameObject.SetActive(true);
		}
	}

	private void BuyOrUpgradeWeapon(bool upgradeNotBuy = false)
	{
		string storeId = CurrentItem.Id;
		string itemId = CurrentItem.Id;
		if (WeaponManager.tagToStoreIDMapping.ContainsKey(itemId))
		{
			storeId = WeaponManager.tagToStoreIDMapping[WeaponManager.FirstUnboughtOrForOurTier(itemId)];
		}
		else if (IsWearCategory(CurrentItem.Category))
		{
			storeId = WeaponManager.FirstUnboughtTag(storeId);
			itemId = storeId;
		}
		else if (IsGadgetsCategory(CurrentItem.Category))
		{
			itemId = (storeId = GadgetsInfo.FirstUnboughtOrForOurTier(CurrentItem.Id));
		}
		if (storeId == null)
		{
			return;
		}
		ItemPrice price = GetItemPrice(CurrentItem.Id, CurrentItem.Category, upgradeNotBuy);
		TryToBuy(mainPanel, price, delegate
		{
			try
			{
				if (upgradeNotBuy && Defs.isSoundFX)
				{
					NGUITools.PlaySound((CurrentItem.Category != CategoryNames.PetsCategory) ? upgradeBtnSound : upgradeBtnPetSound);
				}
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in playing sound on upgrade in shop: {0}", ex);
			}
			if (Defs.isSoundFX)
			{
				UIPlaySound component = ((!upgradeNotBuy) ? buyButton : upgradeButton).GetComponent<UIPlaySound>();
				if (component != null)
				{
					component.Play();
				}
			}
			ActualBuy(storeId, itemId, price);
		}, delegate
		{
		}, null, delegate
		{
			PlayPersAnimations();
		}, delegate
		{
			ButtonClickSound.Instance.PlayClick();
			SetBankCamerasEnabled();
		}, delegate
		{
			ShowGridOrArmorCarousel();
			SetOtherCamerasEnabled(false);
		});
	}

	private static void EquipWeaponSkinWrapper(string itemId)
	{
		try
		{
			WeaponSkin skin = WeaponSkinsManager.GetSkin(itemId);
			if (skin != null)
			{
				if (!WeaponSkinsManager.SetSkinToWeapon(itemId, skin.ToWeapons[0]))
				{
					Debug.LogError("Error in setting weapon skin after giving: itemId = " + itemId);
				}
			}
			else
			{
				Debug.LogError("Error in giving weapon skin: skinInfo != null && skinInfo.ToWeapons != null, itemId = " + itemId);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in giving weapon skin " + itemId + ": " + ex);
		}
	}

	public static void ProvideItem(CategoryNames category, string itemId, int gearCount = 1, bool buyArmorUpToSourceTg = false, int timeForRentIndexForOldTempWeapons = 0, Action<string> contextSpecificAction = null, Action<string> customEquipWearAction = null, bool equipSkin = true, bool equipWear = true, bool doAndroidCloudSync = true)
	{
		if (category == CategoryNames.GearCategory)
		{
			itemId = GearManager.HolderQuantityForID(itemId);
		}
		string text = itemId;
		if (WeaponManager.tagToStoreIDMapping.ContainsKey(itemId))
		{
			text = WeaponManager.tagToStoreIDMapping[itemId];
		}
		if (text == null)
		{
			return;
		}
		ProvideItemCore(category, text, itemId, delegate(string item)
		{
			if (customEquipWearAction != null)
			{
				customEquipWearAction(item);
			}
			else if (equipWear)
			{
				SetAsEquippedAndSendToServer(item, category);
				SendEquippedWearInCategory(item, category, string.Empty);
			}
		}, contextSpecificAction, delegate(string item)
		{
			if (equipSkin)
			{
				SaveSkinAndSendToServer(item);
			}
		}, true, gearCount, buyArmorUpToSourceTg, timeForRentIndexForOldTempWeapons, doAndroidCloudSync);
	}

	private static int AddedNumberOfGearWhenBuyingPack(string id)
	{
		int num = GearManager.ItemsInPackForGear(GearManager.HolderQuantityForID(id));
		if (Storager.getInt(id, false) + num > GearManager.MaxCountForGear(id))
		{
			num = GearManager.MaxCountForGear(id) - Storager.getInt(id, false);
		}
		return num;
	}

	private static void ProvideItemCore(CategoryNames category, string storeId, string itemId, Action<string> onEquipWearAction, Action<string> contextSpecificAction, Action<string> onSkinBoughtAction, bool giveOneItemOfGear = false, int gearCount = 1, bool buyArmorAndHatsUpToTg_UNUSED_NOW = false, int timeForRentIndex = 0, bool doAndroidCloudSync = true)
	{
		if (PromoActionsManager.sharedManager != null)
		{
			PromoActionsManager.sharedManager.RemoveItemFromUnlocked(itemId);
		}
		else
		{
			Debug.LogErrorFormat("SynchronizeIosWithCloud: PromoActionsManager.sharedManager == null");
		}
		if (ShopNGUIController.GunBought != null)
		{
			ShopNGUIController.GunBought();
		}
		if (IsWearCategory(category))
		{
			Storager.setInt(itemId, 1, true);
			if (doAndroidCloudSync && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage >= TrainingController.NewTrainingCompletedStage.ShopCompleted))
			{
				SynchronizeAndroidPurchases("Wear: " + itemId);
			}
			if (onEquipWearAction != null)
			{
				onEquipWearAction(itemId);
			}
		}
		if (IsWeaponCategory(category) && WeaponManager.FirstUnboughtTag(itemId) != itemId)
		{
			List<string> list = WeaponUpgrades.ChainForTag(itemId);
			if (list != null)
			{
				int num = list.IndexOf(itemId) - 1;
				if (num >= 0)
				{
					for (int i = 0; i <= num; i++)
					{
						try
						{
							Storager.setInt(WeaponManager.storeIDtoDefsSNMapping[WeaponManager.tagToStoreIDMapping[list[i]]], 1, true);
						}
						catch
						{
							Debug.LogError("Error filling chain in indexOfWeaponBeforeCurrentTg");
						}
					}
				}
			}
		}
		WeaponManager.sharedManager.AddNewWeapon(storeId, timeForRentIndex);
		if (WeaponManager.sharedManager != null && IsWeaponCategory(category))
		{
			try
			{
				string text = WeaponManager.LastBoughtTag(itemId);
				bool flag = WeaponManager.sharedManager.IsAvailableTryGun(text);
				bool flag2 = WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(text);
				WeaponManager.RemoveGunFromAllTryGunRelated(text);
				if (flag2)
				{
					string empty = string.Empty;
					string itemNameNonLocalized = ItemDb.GetItemNameNonLocalized(text, empty, category);
					AnalyticsStuff.LogWEaponsSpecialOffers_Conversion(false, itemNameNonLocalized);
				}
				if (flag2 || flag)
				{
					Action<string> tryGunBought = ShopNGUIController.TryGunBought;
					if (tryGunBought != null)
					{
						tryGunBought(text);
					}
					if (ABTestController.useBuffSystem)
					{
						BuffSystem.instance.OnTryGunBuyed(ItemDb.GetByTag(itemId).PrefabName);
					}
					else
					{
						KillRateCheck.OnTryGunBuyed();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in removeing TryGun structures: " + ex);
			}
		}
		if (category == CategoryNames.GearCategory)
		{
			if (storeId.Contains(GearManager.UpgradeSuffix))
			{
				string key = GearManager.NameForUpgrade(GearManager.HolderQuantityForID(storeId), GearManager.CurrentNumberOfUphradesForGear(GearManager.HolderQuantityForID(storeId)) + 1);
				Storager.setInt(key, 1, false);
			}
			else
			{
				int num2 = AddedNumberOfGearWhenBuyingPack(storeId);
				Storager.setInt(storeId, Storager.getInt(storeId, false) + ((!giveOneItemOfGear) ? num2 : gearCount), false);
			}
		}
		if (category == CategoryNames.LeagueWeaponSkinsCategory)
		{
			WeaponSkinsManager.ProvideSkin(itemId);
			EquipWeaponSkinWrapper(itemId);
			if (doAndroidCloudSync)
			{
				SynchronizeAndroidPurchases("Weapon skins");
			}
		}
		if (category == CategoryNames.PetsCategory)
		{
			PetUpdateInfo petUpdateInfo = Singleton<PetsManager>.Instance.Upgrade(itemId);
			if (petUpdateInfo != null && petUpdateInfo.PetNew != null && !petUpdateInfo.PetNew.InfoId.IsNullOrEmpty())
			{
				EquipPet(petUpdateInfo.PetNew.InfoId);
			}
			else
			{
				Debug.LogErrorFormat("ProvideItemCore: error equipping pet after upgrading {0}", itemId ?? "null");
			}
		}
		if (IsGadgetsCategory(category))
		{
			GadgetsInfo.ProvideGadget(itemId);
			EquipGadget(itemId, (GadgetInfo.GadgetCategory)category);
			if (doAndroidCloudSync)
			{
				SynchronizeAndroidPurchases("Gadgets");
			}
		}
		if (contextSpecificAction != null)
		{
			contextSpecificAction(storeId);
		}
		if (category != CategoryNames.SkinsCategory)
		{
			return;
		}
		if (storeId != null && SkinsController.shopKeyFromNameSkin.ContainsValue(storeId))
		{
			Storager.setInt(storeId, 1, true);
			if (doAndroidCloudSync)
			{
				SynchronizeAndroidPurchases("Skin: " + storeId);
			}
		}
		if (onSkinBoughtAction != null)
		{
			onSkinBoughtAction(storeId);
		}
	}

	public void FireBuyAction(string item)
	{
		if (buyAction != null)
		{
			buyAction(item);
		}
	}

	private void MarkItemAsToRemoveOnLeave(ShopItem itemBefore)
	{
		BestItemsToRemoveOnLeave.Add(itemBefore);
		List<string> previousUpgrades = new List<string>();
		try
		{
			List<string> value2;
			if (CurrentCategory == CategoryNames.BestWeapons)
			{
				List<string> list2 = WeaponUpgrades.ChainForTag(itemBefore.Id);
				if (list2 != null)
				{
					previousUpgrades = list2.GetRange(0, list2.IndexOf(itemBefore.Id));
				}
			}
			else if (CurrentCategory == CategoryNames.BestWear)
			{
				List<List<string>> value;
				if (Wear.wear.TryGetValue(itemBefore.Category, out value) && value != null)
				{
					List<string> list3 = value.FirstOrDefault((List<string> list) => list.Contains(itemBefore.Id));
					if (list3 != null)
					{
						previousUpgrades = list3.GetRange(0, list3.IndexOf(itemBefore.Id));
					}
				}
			}
			else if (CurrentCategory == CategoryNames.BestGadgets && GadgetsInfo.Upgrades.TryGetValue(itemBefore.Id, out value2) && value2 != null)
			{
				previousUpgrades = value2.GetRange(0, value2.IndexOf(itemBefore.Id));
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in MarkItemAsToRemoveOnLeave: {0}", ex);
		}
		if (previousUpgrades != null && previousUpgrades.Count > 0)
		{
			BestItemsToRemoveOnLeave.RemoveAll((ShopItem shopItem) => previousUpgrades.Contains(shopItem.Id));
		}
	}

	private void ActualBuy(string storeId, string itemId, ItemPrice itemPrice)
	{
		if (CurrentItem.Category == CategoryNames.ArmorCategory || IsWeaponCategory(CurrentItem.Category))
		{
			FireWeaponOrArmorBought();
		}
		CategoryNames category = CurrentItem.Category;
		ProvideItemCore(CurrentItem.Category, storeId, itemId, delegate(string item)
		{
			EquipWear(item);
		}, delegate(string item)
		{
			if (IsWeaponCategory(category) || IsWearCategory(category))
			{
				FireBuyAction(item);
			}
			purchaseSuccessful.SetActive(true);
			_timePurchaseSuccessfulShown = Time.realtimeSinceStartup;
		}, delegate(string item)
		{
			SetSkinAsCurrent(item);
		}, false, 1, false, 0, !inGame);
		if (WeaponManager.tagToStoreIDMapping.ContainsValue(storeId))
		{
			IEnumerable<string> source = from item in WeaponManager.tagToStoreIDMapping
				where item.Value == storeId
				select item into kv
				select kv.Key;
			if (!inGame)
			{
				SynchronizeAndroidPurchases("Weapon: " + (source.FirstOrDefault() ?? "null"));
			}
		}
		try
		{
			string text = storeId;
			try
			{
				if (CurrentItem.Category == CategoryNames.SkinsCategory && text != null && SkinsController.shopKeyFromNameSkin.ContainsKey(text))
				{
					text = SkinsController.shopKeyFromNameSkin[text];
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in setting shopId: " + ex);
			}
			string text2 = ((!IsGadgetsCategory(CurrentItem.Category)) ? (WeaponManager.LastBoughtTag(CurrentItem.Id) ?? WeaponManager.FirstUnboughtTag(CurrentItem.Id)) : (GadgetsInfo.LastBoughtFor(CurrentItem.Id) ?? GadgetsInfo.FirstUnbought(CurrentItem.Id)));
			string itemNameNonLocalized = ItemDb.GetItemNameNonLocalized(text2, text, CurrentItem.Category);
			try
			{
				bool isDaterWeapon = false;
				if (IsWeaponCategory(CurrentItem.Category))
				{
					WeaponSounds weaponInfo = ItemDb.GetWeaponInfo(text2);
					isDaterWeapon = weaponInfo != null && weaponInfo.IsAvalibleFromFilter(3);
				}
				CategoryNames categoryNames = ((CurrentCategory != CategoryNames.LeagueHatsCategory) ? CurrentItem.Category : CategoryNames.LeagueHatsCategory);
				string text3 = AnalyticsConstants.GetSalesName(categoryNames) ?? categoryNames.ToString();
				AnalyticsStuff.LogSales(itemNameNonLocalized, text3, isDaterWeapon);
				AnalyticsFacade.InAppPurchase(itemNameNonLocalized, AnalyticsStuff.AnalyticsReadableCategoryNameFromOldCategoryName(text3), 1, itemPrice.Price, itemPrice.Currency);
				if (IsBestCategory(CurrentCategory))
				{
					AnalyticsStuff.LogBestSales(itemNameNonLocalized, CurrentCategory);
				}
				try
				{
					if (GunThatWeUsedInPolygon != null && GunThatWeUsedInPolygon == WeaponManager.LastBoughtTag(itemId))
					{
						AnalyticsFacade.SendCustomEvent("Polygon", new Dictionary<string, object>
						{
							{ "Conversion", "Buy" },
							{ "Currency Spended", itemNameNonLocalized }
						});
						GunThatWeUsedInPolygon = null;
					}
				}
				catch (Exception ex2)
				{
					Debug.LogError("Exception in sending Polygon analytics: " + ex2);
				}
				if (_isFromPromoActions && _promoActionsIdClicked != null && text2 != null && _promoActionsIdClicked == text2)
				{
					AnalyticsStuff.LogSpecialOffersPanel("Efficiency", "Buy", text3 ?? "Unknown", itemNameNonLocalized);
				}
				_isFromPromoActions = false;
			}
			catch (Exception ex3)
			{
				Debug.LogError("Exception in LogSales block in Shop: " + ex3);
			}
		}
		catch (Exception ex4)
		{
			Debug.LogError("Exception in Shop Logging: " + ex4);
		}
		string id = null;
		if (IsGadgetsCategory(CurrentItem.Category))
		{
			id = GadgetsInfo.LastBoughtFor(CurrentItem.Id);
		}
		else if (CurrentItem.Category == CategoryNames.PetsCategory)
		{
			try
			{
				PlayerPet playerPet = Singleton<PetsManager>.Instance.GetPlayerPet(CurrentItem.Id);
				id = ((playerPet == null) ? CurrentItem.Id : playerPet.InfoId);
			}
			catch (Exception ex5)
			{
				Debug.LogErrorFormat("Exception in getting actual upgrade of pet in ActualBuy: {0}", ex5);
				PlayerPet playerPet2 = Singleton<PetsManager>.Instance.PlayerPets.FirstOrDefault();
				if (playerPet2 != null)
				{
					id = playerPet2.InfoId;
				}
			}
		}
		else
		{
			id = WeaponManager.LastBoughtTag(CurrentItem.Id);
		}
		CurrentItem = new ShopItem(id, CurrentItem.Category);
		UpdateIcons(true);
		if (IsBestCategory(CurrentCategory))
		{
			MarkItemAsToRemoveOnLeave(CurrentItem);
		}
		ReloadGridOrCarousel(CurrentItem);
		Resources.UnloadUnusedAssets();
		try
		{
			AllArmoryCells.ForEach(delegate(ArmoryCell cell)
			{
				cell.GetComponent<UIPanel>().SetDirty();
			});
		}
		catch (Exception ex6)
		{
			Debug.LogError("Exception in AllArmoryCells.ForEach(cell => cell.GetComponent<UIPanel>().SetDirty()): " + ex6);
		}
		if (!inGame && CurrentItem.Id == "cape_Custom")
		{
			GoToSkinsEditor();
		}
		UpdateUnlockedItemsIndicators();
	}

	private static void SaveSkinAndSendToServer(string id)
	{
		SkinsController.SetCurrentSkin(id);
		byte[] array = SkinsController.currentSkinForPers.EncodeToPNG();
		if (array != null)
		{
			string text = Convert.ToBase64String(array);
			if (text != null)
			{
				FriendsController.sharedController.skin = text;
				FriendsController.sharedController.SendOurData(true);
			}
		}
	}

	private void FireOnEquipSkin(string id)
	{
		if (onEquipSkinAction != null)
		{
			onEquipSkinAction(id);
		}
	}

	public void SetSkinAsCurrent(string id)
	{
		SaveSkinAndSendToServer(id);
		FireOnEquipSkin(id);
	}

	public static void SetAsEquippedAndSendToServer(string itemId, CategoryNames category)
	{
		if (category == CategoryNames.BestWeapons || category == CategoryNames.BestGadgets || category == CategoryNames.BestWear)
		{
			Debug.LogError("Tried to pass best category to SetAsEquippedAndSendToServer: tg = " + itemId + "   c = " + category);
			return;
		}
		if (!IsWearCategory(MapShopCategoryToItemCategory(category)))
		{
			Debug.LogError("Tried to pass non-wear category to SetAsEquippedAndSendToServer: tg = " + itemId + "   c = " + category);
			return;
		}
		if (string.IsNullOrEmpty(itemId))
		{
			Debug.LogError("string.IsNullOrEmpty(tg) in SetAsEquippedAndSendToServer: tg = " + itemId + "   c = " + category);
			itemId = NoneEquippedForWearCategory(MapShopCategoryToItemCategory(category));
		}
		Storager.setString(SnForWearCategory(category), itemId, false);
		if (FriendsController.sharedController == null)
		{
			Debug.LogError("FriendsController.sharedController == null");
		}
		else
		{
			FriendsController.sharedController.SendAccessories();
		}
	}

	public IEnumerator BackAfterDelay()
	{
		IsExiting = true;
		using (new ActionDisposable(delegate
		{
			IsExiting = false;
		}))
		{
			_isFromPromoActions = false;
			yield return null;
			if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted && TutorialPhasePassed == TutorialPhase.LeaveArmory)
			{
				TrainingController.CompletedTrainingStage = TrainingController.NewTrainingCompletedStage.ShopCompleted;
				HintController.instance.StartShow();
				AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Back_Shop);
				TutorialDisableHints();
			}
			if (resumeAction != null)
			{
				resumeAction();
			}
			else
			{
				GuiActive = false;
			}
			if (wearResumeAction != null)
			{
				wearResumeAction();
			}
			if (InTrainingAfterNoviceArmorRemoved)
			{
				trainingColliders.SetActive(false);
				trainingRemoveNoviceArmorCollider.SetActive(false);
			}
			InTrainingAfterNoviceArmorRemoved = false;
			if (ExperienceController.sharedController != null)
			{
				ExperienceController.sharedController.isShowRanks = false;
			}
			GunThatWeUsedInPolygon = null;
		}
	}

	public static string SnForWearCategory(CategoryNames c)
	{
		object result;
		switch (c)
		{
		case CategoryNames.CapesCategory:
			result = Defs.CapeEquppedSN;
			break;
		case CategoryNames.BootsCategory:
			result = Defs.BootsEquppedSN;
			break;
		case CategoryNames.ArmorCategory:
			result = Defs.ArmorNewEquppedSN;
			break;
		case CategoryNames.MaskCategory:
			result = "MaskEquippedSN";
			break;
		default:
			result = Defs.HatEquppedSN;
			break;
		}
		return (string)result;
	}

	public static string NoneEquippedForWearCategory(CategoryNames c)
	{
		object result;
		switch (c)
		{
		case CategoryNames.CapesCategory:
			result = Defs.CapeNoneEqupped;
			break;
		case CategoryNames.BootsCategory:
			result = Defs.BootsNoneEqupped;
			break;
		case CategoryNames.ArmorCategory:
			result = Defs.ArmorNewNoneEqupped;
			break;
		case CategoryNames.MaskCategory:
			result = "MaskNoneEquipped";
			break;
		default:
			result = Defs.HatNoneEqupped;
			break;
		}
		return (string)result;
	}

	public string WearForCat(CategoryNames c)
	{
		//Discarded unreachable code: IL_0023
		if (!IsWearCategory(c))
		{
			return string.Empty;
		}
		try
		{
			return Storager.getString(SnForWearCategory(c), false);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in WearForCat: " + ex);
		}
		return string.Empty;
	}

	private void HandleOffersUpdated()
	{
		UpdateButtons();
	}

	private void OnDestroy()
	{
		EggsManager.OnReadyToUse -= EggsManager_OnReadyToUse;
		Singleton<PetsManager>.Instance.OnPlayerPetAdded -= PetsManager_Instance_OnPlayerPetAdded;
		if (profile != null)
		{
			Resources.UnloadAsset(profile);
			profile = null;
		}
		Storager.UnSubscribeToChanged(Defs.TrainingCompleted_4_4_Sett, OnTrainingCompleted_4_4_Sett_Changed);
		PromoActionsManager.OnLockedItemsUpdated -= PromoActionsManager_OnLockedItemsUpdated;
	}

	private void Start()
	{
		for (int i = 0; i < 100; i++)
		{
			Transform transform = UnityEngine.Object.Instantiate(Resources.Load<Transform>("ArmoryCell"));
			transform.SetParent(itemsGrid.transform, false);
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			transform.name = i.ToString("D4");
			transform.localRotation = Quaternion.identity;
		}
		itemScrollBottomAnchor = gridScrollView.GetComponent<UIPanel>().bottomAnchor.absolute;
		itemScrollBottomAnchorRent = gridScrollView.GetComponent<UIPanel>().bottomAnchor.absolute + 25;
		_skinsMakerSkinCache = Resources.Load<Texture>("skins_maker_skin");
		StartCoroutine(TryToShowExpiredBanner());
		PromoActionsManager.OnLockedItemsUpdated += PromoActionsManager_OnLockedItemsUpdated;
	}

	private void PromoActionsManager_OnLockedItemsUpdated()
	{
		if (GuiActive)
		{
			UpdateUnlockedItemsIndicators();
		}
	}

	private void OnEnable()
	{
		UpdateTutorialState();
	}

	private void OnDisable()
	{
		if (_tutorialCurrentState != null)
		{
			_tutorialCurrentState.StageAct(TutorialStageTrigger.Exit);
		}
	}

	private IEnumerator TryToShowExpiredBanner()
	{
		while (FriendsController.sharedController == null || TempItemsController.sharedController == null)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(FriendsController.sharedController.MyWaitForSeconds(1f));
			try
			{
				if (!GuiActive || (superCategoriesButtonController.currentBtnName == Supercategory.Best.ToString() && CurrentCategory == CategoryNames.BestWear) || (superCategoriesButtonController.currentBtnName != Supercategory.Best.ToString() && MapShopCategoryToItemCategory(CurrentCategory) == CategoryNames.SkinsCategory) || !(superCategoriesButtonController.currentBtnName != Supercategory.Wear.ToString()) || rentScreenPoint.childCount != 0)
				{
					continue;
				}
				int readyEggsCount = 0;
				try
				{
					readyEggsCount = Singleton<EggsManager>.Instance.ReadyEggs().Count;
				}
				catch (Exception ex)
				{
					Exception e2 = ex;
					Debug.LogErrorFormat("Exception in getting number of ready eggs: {0}", e2);
				}
				if (readyEggsCount > 0)
				{
					Transform eggHatchingWindow = UnityEngine.Object.Instantiate(Resources.Load<Transform>("NguiWindows/PetWindows"));
					eggHatchingWindow.parent = rentScreenPoint;
					eggHatchingWindow.localPosition = Vector3.zero;
					eggHatchingWindow.localScale = new Vector3(1f, 1f, 1f);
					eggHatchingWindow.GetComponent<EggHatchingWindowController>().EggForHatching = Singleton<EggsManager>.Instance.ReadyEggs().First();
				}
				else if (Storager.getInt("Training.ShouldRemoveNoviceArmorInShopKey", false) == 1)
				{
					GameObject window = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("NguiWindows/WeRemoveNoviceArmorBanner"));
					window.transform.parent = rentScreenPoint;
					Player_move_c.SetLayerRecursively(window, LayerMask.NameToLayer("NGUIShop"));
					window.transform.localPosition = new Vector3(0f, 0f, -130f);
					window.transform.localRotation = Quaternion.identity;
					window.transform.localScale = new Vector3(1f, 1f, 1f);
					UpdatePersArmor(Defs.ArmorNewNoneEqupped);
					sharedShop.trainingColliders.SetActive(true);
					sharedShop.trainingRemoveNoviceArmorCollider.SetActive(true);
					InTrainingAfterNoviceArmorRemoved = true;
					if (HintController.instance != null)
					{
						HintController.instance.HideHintByName("shop_remove_novice_armor");
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning("exception in Shop  TryToShowExpiredBanner: " + e);
			}
		}
	}

	private void ReportCurrentVisibleCells()
	{
		try
		{
			if (IsBestCategory(CurrentCategory) || !itemsGrid.gameObject.activeInHierarchy)
			{
				return;
			}
			List<string> itemsViewed = (from cell in itemsGrid.GetComponentsInChildren<ArmoryCell>(false)
				where cell.IsFullyVisible
				select cell.ItemId).ToList();
			if (PromoActionsManager.sharedManager != null)
			{
				int num = PromoActionsManager.sharedManager.ItemsViewed(itemsViewed);
				if (num > 0)
				{
					UpdateUnlockedItemsIndicators();
				}
			}
			else
			{
				Debug.LogErrorFormat("ShopNguiController.Update: PromoActionsManager.sharedManager == null");
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in ShopNguiController.Update: {0}", ex);
		}
	}

	private void Update()
	{
		if (!ActiveObject.activeInHierarchy)
		{
			return;
		}
		ExperienceController.sharedController.isShowRanks = rentScreenPoint.childCount == 0 && SkinEditorController.sharedController == null && (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled);
		if (Time.realtimeSinceStartup - m_timeOfPLastStuffUpdate >= 1f && CurrentCategory != CategoryNames.PetsCategory)
		{
			m_timeOfPLastStuffUpdate = Time.realtimeSinceStartup;
			if (GuiActive && CurrentItem != null && CurrentItem.Id != null && WeaponManager.sharedManager != null && WeaponManager.sharedManager.IsWeaponDiscountedAsTryGun(CurrentItem.Id))
			{
				UpdateTryGunDiscountTime(propertiesContainer, CurrentItem.Id);
			}
		}
		if (m_firstReportItemsViewedSkipped && Time.realtimeSinceStartup - m_timeOfLAstReportVisibleCells >= 0.2f)
		{
			m_timeOfLAstReportVisibleCells = Time.realtimeSinceStartup;
			ReportCurrentVisibleCells();
		}
		m_firstReportItemsViewedSkipped = true;
		bool state = superCategoriesButtonController.currentBtnName == Supercategory.Wear.ToString() || CurrentCategory == CategoryNames.BestWear || CurrentCategory == CategoryNames.LeagueHatsCategory || CurrentCategory == CategoryNames.LeagueSkinsCategory;
		showArmorButton.gameObject.SetActiveSafeSelf(state);
		showWearButton.gameObject.SetActiveSafeSelf(state);
		if (Time.realtimeSinceStartup - _timePurchaseSuccessfulShown >= 2f)
		{
			purchaseSuccessful.SetActive(false);
		}
		if (mainPanel.activeInHierarchy && !HOTween.IsTweening(MainMenu_Pers) && ArmoryInfoScreenController.sharedController == null && !EggWindowIsOpened)
		{
			RilisoftRotator.RotateCharacter(MainMenu_Pers, _rotationRateForCharacter, _touchZoneForRotatingCharacter, ref idleTimerLastTime, ref lastTime);
		}
		if (Time.realtimeSinceStartup - idleTimerLastTime > IdleTimeoutPers)
		{
			HOTween.Kill(MainMenu_Pers);
			Vector3 zero = Vector3.zero;
			idleTimerLastTime = Time.realtimeSinceStartup;
			HOTween.To(MainMenu_Pers, 0.5f, new TweenParms().Prop("localRotation", new PlugQuaternion(zero)).Ease(EaseType.Linear).OnComplete((TweenDelegate.TweenCallback)delegate
			{
				idleTimerLastTime = Time.realtimeSinceStartup;
			}));
		}
		if ((CurrentItem == null || CurrentItem.Category != CategoryNames.CapesCategory) && Time.realtimeSinceStartup - idleTimerLastTime > IdleTimeoutPers)
		{
			SetCamera();
		}
		ActivityIndicator.IsActiveIndicator = StoreKitEventListener.restoreInProcess;
		CheckCenterItemChanging();
	}

	private void LateUpdate()
	{
		if (needRefreshInLateUpdate > 0)
		{
			if (gridScrollView.panel != null)
			{
				gridScrollView.panel.ResetAndUpdateAnchors();
				gridScrollView.panel.SetDirty();
				gridScrollView.panel.Refresh();
			}
			needRefreshInLateUpdate--;
			gridScrollView.DisableSpring();
			gridScrollView.RestrictWithinBounds(true);
		}
		if (GuiActive)
		{
			if (updateScrollViewOnLateUpdateForTryPanel)
			{
				updateScrollViewOnLateUpdateForTryPanel = false;
				gridScrollView.DisableSpring();
				gridScrollView.RestrictWithinBounds(true);
				UIPanel component = gridScrollView.GetComponent<UIPanel>();
				component.SetDirty();
				component.Refresh();
			}
			if (!categoryGridsRepositioned)
			{
				AdjustCategoryGridCells();
				categoryGridsRepositioned = true;
				foreach (UIRect item in sharedShop.widgetsToUpdateAnchorsOnStart)
				{
					item.ResetAndUpdateAnchors();
				}
				armoryRootPanel.Refresh();
			}
		}
		if (CurrentCategory == CategoryNames.ArmorCategory)
		{
			float num = scrollViewPanel.GetViewSize().x / 2f;
			ShopCarouselElement[] componentsInChildren = wrapContent.GetComponentsInChildren<ShopCarouselElement>(false);
			ShopCarouselElement[] array = componentsInChildren;
			foreach (ShopCarouselElement shopCarouselElement in array)
			{
				Transform transform = shopCarouselElement.transform;
				float x = scrollViewPanel.clipOffset.x;
				float num2 = transform.localPosition.x - x - 20f;
				float num3 = Mathf.Abs(num2);
				float num4 = scaleCoefficent + (1f - scaleCoefficent) * (1f - num3 / num);
				float num5 = 0.65f;
				num4 = ((!(num3 <= num / 3f)) ? (scaleCoefficent + (num5 - scaleCoefficent) * (1f - (num3 - num / 3f) / (num * 3f / 3f))) : (num5 + (1f - num5) * (1f - num3 / (num / 3f))));
				if (num3 >= num * 0.75f)
				{
					num4 = 0f;
				}
				float num6 = 0f;
				float num7 = ((num2 <= 0f) ? 1 : (-1));
				if (num2 != 0f)
				{
					num6 = ((num3 <= wrapContent.cellWidth) ? (firstOFfset * (num3 / wrapContent.cellWidth)) : ((!(num3 <= 2f * wrapContent.cellWidth)) ? (secondOffset * (1f - (num3 - 2f * wrapContent.cellWidth) / wrapContent.cellWidth)) : (firstOFfset + (secondOffset - firstOFfset) * ((num3 - wrapContent.cellWidth) / wrapContent.cellWidth))));
				}
				num6 *= num7;
				if (!EnableConfigurePos || scrollViewPanel.GetComponent<UIScrollView>().isDragging || scrollViewPanel.GetComponent<UIScrollView>().currentMomentum.x > 0f)
				{
					shopCarouselElement.SetPos(num4, num6);
				}
				shopCarouselElement.topSeller.gameObject.SetActive(shopCarouselElement.showTS && num3 <= wrapContent.cellWidth / 10f);
				shopCarouselElement.newnew.gameObject.SetActive(shopCarouselElement.showNew && num3 <= wrapContent.cellWidth / 10f);
				shopCarouselElement.quantity.gameObject.SetActive(shopCarouselElement.showQuantity && num3 <= wrapContent.cellWidth / 10f);
			}
		}
		if (_escapeRequested)
		{
			StartCoroutine(BackAfterDelay());
			_escapeRequested = false;
		}
	}

	private void HandleEscape()
	{
		if ((BankController.Instance != null && BankController.Instance.InterfaceEnabled) || (ProfileController.Instance != null && ProfileController.Instance.InterfaceEnabled) || (FriendsWindowGUI.Instance != null && FriendsWindowGUI.Instance.InterfaceEnabled))
		{
			return;
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage < TrainingController.NewTrainingCompletedStage.ShopCompleted)
		{
			if (Application.isEditor)
			{
				Debug.Log("Ignoring [Escape] since Tutorial is not completed.");
			}
		}
		else if (InTrainingAfterNoviceArmorRemoved)
		{
			if (Application.isEditor)
			{
				Debug.Log("Ignoring [Escape] since Tutorial after removing Novice Armor is not completed.");
			}
		}
		else if (!GuiActive)
		{
			if (Application.isEditor)
			{
				Debug.Log(GetType().Name + ".LateUpdate():    Ignoring Escape because Shop GUI is not active.");
			}
		}
		else
		{
			_escapeRequested = true;
		}
	}

	public void SetInGame(bool e)
	{
		inGame = e;
	}

	private IEnumerator DisableStub()
	{
		for (int i = 0; i < 3; i++)
		{
			yield return null;
		}
		stub.SetActive(false);
	}

	private void UpdateAllWearAndSkinOnPers()
	{
		UpdatePersHat(WearForCat(CategoryNames.HatsCategory));
		UpdatePersCape(WearForCat(CategoryNames.CapesCategory));
		UpdatePersArmor(WearForCat(CategoryNames.ArmorCategory));
		UpdatePersBoots(WearForCat(CategoryNames.BootsCategory));
		UpdatePersMask(WearForCat(CategoryNames.MaskCategory));
		UpdatePersSkin(SkinsController.currentSkinNameForPers);
		try
		{
			UpdatePersPet(Singleton<PetsManager>.Instance.GetEqipedPetId());
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in UpdateAllWearAndSkinOnPers when updating pet: {0}", ex);
		}
	}

	private void MakeShopActive()
	{
		Light[] array = UnityEngine.Object.FindObjectsOfType<Light>() ?? new Light[0];
		Light[] array2 = array;
		foreach (Light light in array2)
		{
			if (!mylights.Contains(light))
			{
				light.cullingMask &= ~(1 << LayerMask.NameToLayer("NGUIShop"));
				light.cullingMask &= ~(1 << LayerMask.NameToLayer("NGUIShopWorld"));
			}
		}
		sharedShop.ActiveObject.SetActive(true);
		wrapContent.Reposition();
		if (ExperienceController.sharedController != null && ExpController.Instance != null)
		{
			ExperienceController.sharedController.isShowRanks = true;
			ExpController.Instance.InterfaceEnabled = true;
		}
		UpdateAllWearAndSkinOnPers();
		MyCenterOnChild myCenterOnChild = carouselCenter;
		myCenterOnChild.onFinished = (SpringPanel.OnFinished)Delegate.Combine(myCenterOnChild.onFinished, new SpringPanel.OnFinished(HandleCarouselCentering));
		PromoActionsManager.ActionsUUpdated += HandleOffersUpdated;
		PlayPersAnimations();
		idleTimerLastTime = Time.realtimeSinceStartup;
		sharedShop.carouselCenter.enabled = true;
		AdjustCategoryButtonsForFilterMap();
	}

	public void PlayPersAnimations()
	{
		PlayWeaponAnimation();
		PlayPetAnimation();
	}

	private void SetArmoryCellShaderParams()
	{
		AllArmoryCells.ForEach(delegate(ArmoryCell armoryCell)
		{
			try
			{
				SetSliceShaderParams(armoryCell);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in setting OnApplicationPause SliceToWorldPosShader: " + ex);
			}
		});
	}

	private IEnumerator OnApplicationPause(bool pauseStatus)
	{
		if (Application.platform != RuntimePlatform.Android || !GuiActive)
		{
			yield break;
		}
		yield return null;
		yield return null;
		yield return null;
		try
		{
			SetAppropeiateStateForPropertiesPanel();
			gridScrollView.MoveRelative(new Vector3(0f, 2f));
			if (CurrentCategory == CategoryNames.SkinsCategory || CurrentCategory == CategoryNames.CapesCategory || CurrentCategory == CategoryNames.BestWear)
			{
				SetArmoryCellShaderParams();
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Exception in ShopNGUIController OnApplicationPause: " + e);
		}
	}

	private void DisableButtonsInIndexes(List<CategoryNames> indexesPar)
	{
		foreach (Transform item in AllCategoryButtonTransforms())
		{
			item.GetComponent<UIButton>().isEnabled = !indexesPar.Contains((CategoryNames)(int)Enum.Parse(typeof(CategoryNames), item.name));
		}
	}

	private void AdjustCategoryButtonsForFilterMap()
	{
		List<CategoryNames> indexesPar = new List<CategoryNames>();
		if (SceneLoader.ActiveSceneName.Equals("Sniper"))
		{
			List<CategoryNames> list = new List<CategoryNames>();
			list.Add(CategoryNames.PrimaryCategory);
			list.Add(CategoryNames.SpecilCategory);
			list.Add(CategoryNames.PremiumCategory);
			indexesPar = list;
			DisableButtonsInIndexes(indexesPar);
		}
		else if (SceneLoader.ActiveSceneName.Equals("Knife"))
		{
			List<CategoryNames> list = new List<CategoryNames>();
			list.Add(CategoryNames.PrimaryCategory);
			list.Add(CategoryNames.BackupCategory);
			list.Add(CategoryNames.SpecilCategory);
			list.Add(CategoryNames.PremiumCategory);
			list.Add(CategoryNames.SniperCategory);
			indexesPar = list;
			DisableButtonsInIndexes(indexesPar);
		}
		else
		{
			DisableButtonsInIndexes(indexesPar);
		}
	}

	private void SetPetsCategoryEnable()
	{
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

	private static string TemppOrHighestDPSGunInCategory(int cInt)
	{
		string text = null;
		if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.FilteredShopListsNoUpgrades != null && WeaponManager.sharedManager.FilteredShopListsNoUpgrades.Count > cInt)
		{
			List<GameObject> list = WeaponManager.sharedManager.FilteredShopListsNoUpgrades[cInt];
			GameObject gameObject = list.Find((GameObject w) => ItemDb.IsTemporaryGun(ItemDb.GetByPrefabName(w.name.Replace("(Clone)", string.Empty)).Tag));
			if (gameObject != null)
			{
				text = ItemDb.GetByPrefabName(gameObject.name.Replace("(Clone)", string.Empty)).Tag;
			}
			if (text == null && list.Count > 0)
			{
				for (int num = list.Count - 1; num >= 0; num--)
				{
					string text2 = ItemDb.GetByPrefabName(list[num].name.Replace("(Clone)", string.Empty)).Tag;
					if (!ItemDb.IsTemporaryGun(text2) && ExpController.Instance != null && list[num].GetComponent<WeaponSounds>().tier <= ExpController.Instance.OurTier)
					{
						text = text2;
						break;
					}
				}
			}
		}
		return text;
	}

	private static string HighestDPSGun(CategoryNames desiredCategory, out CategoryNames inFactCategory)
	{
		inFactCategory = desiredCategory;
		string text = null;
		text = TemppOrHighestDPSGunInCategory((int)desiredCategory);
		if (text == null && WeaponManager.sharedManager.playerWeapons.Count > 0)
		{
			int num = (WeaponManager.sharedManager.playerWeapons[0] as Weapon).weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1;
			text = TemppOrHighestDPSGunInCategory(num);
			inFactCategory = (CategoryNames)num;
		}
		return text;
	}

	private void OnLevelWasLoaded(int level)
	{
		if (GuiActive)
		{
			SwitchAmbientLightAndFogToShop();
		}
	}

	public void SetOtherCamerasEnabled(bool e)
	{
		if (e)
		{
			foreach (Camera disablesCamera in disablesCameras)
			{
				if (!(disablesCamera == null))
				{
					disablesCamera.enabled = e;
					if (disablesCamera.GetComponent<UICamera>() != null)
					{
						disablesCamera.GetComponent<UICamera>().enabled = e;
					}
				}
			}
			disablesCameras.Clear();
			return;
		}
		List<Camera> list = (Camera.allCameras ?? new Camera[0]).ToList();
		List<Camera> collection = ProfileController.Instance.GetComponentsInChildren<Camera>(true).ToList();
		list.AddRange(collection);
		list.AddRange(BankRelatedCameras());
		foreach (Camera item in list)
		{
			if ((!(ExpController.Instance != null) || !ExpController.Instance.IsRenderedWithCamera(item)) && !item.gameObject.CompareTag("CamTemp") && !sharedShop.ourCameras.Contains(item) && !object.ReferenceEquals(item, InfoWindowController.Instance.infoWindowCamera))
			{
				disablesCameras.Add(item);
				item.enabled = e;
				if (item.GetComponent<UICamera>() != null)
				{
					item.GetComponent<UICamera>().enabled = e;
				}
			}
		}
	}

	public void IsInShopFromPromoPanel(bool isFromPromoACtions, string tg)
	{
		_isFromPromoActions = isFromPromoACtions;
		_promoActionsIdClicked = tg;
	}

	private static void SwitchAmbientLightAndFogToShop()
	{
		sharedShop._storedAmbientLight = RenderSettings.ambientLight;
		sharedShop._storedFogEnabled = RenderSettings.fog;
		RenderSettings.ambientLight = Defs.AmbientLightColorForShop();
		RenderSettings.fog = false;
	}

	[ContextMenu("AdjustCategoryGridCells")]
	public void AdjustCategoryGridCells()
	{
		float a = (float)weaponCategoriesGrid.GetComponentInChildren<UIToggle>().activeSprite.width * 2f;
		float b = (float)sccrollViewBackground.width / 6f - 3f;
		weaponCategoriesGrid.cellWidth = Mathf.Min(a, b);
		wearCategoriesGrid.cellWidth = Mathf.Min((float)wearCategoriesGrid.GetComponentInChildren<UIToggle>().activeSprite.width * 2f, (float)sccrollViewBackground.width / 6f);
		gridCategoriesLeague.cellWidth = Mathf.Min((float)gridCategoriesLeague.GetComponentInChildren<UIToggle>().activeSprite.width * 2f, (float)sccrollViewBackground.width / 3f);
		gadgetsGrid.cellWidth = Mathf.Min((float)gadgetsGrid.GetComponentInChildren<UIToggle>().activeSprite.width * 2f, (float)sccrollViewBackground.width / 3f);
		petsGrid.cellWidth = Mathf.Min((float)petsGrid.GetComponentInChildren<UIToggle>().activeSprite.width * 2f, (float)sccrollViewBackground.width / 3f);
		bestCategoriesGrid.cellWidth = Mathf.Min((float)bestCategoriesGrid.GetComponentInChildren<UIToggle>().activeSprite.width * 2f, (float)sccrollViewBackground.width / 3f);
		weaponCategoriesGrid.Reposition();
		wearCategoriesGrid.Reposition();
		gridCategoriesLeague.Reposition();
		gadgetsGrid.Reposition();
		petsGrid.Reposition();
		bestCategoriesGrid.Reposition();
	}

	public static void DisableLightProbesRecursively(GameObject w)
	{
		Player_move_c.PerformActionRecurs(w, delegate(Transform t)
		{
			MeshRenderer component = t.GetComponent<MeshRenderer>();
			SkinnedMeshRenderer component2 = t.GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				component.useLightProbes = false;
			}
			if (component2 != null)
			{
				component2.useLightProbes = false;
			}
		});
	}

	public void SetWeapon(string weaponTag, string weaponSkinId)
	{
		characterInterface.skinCharacter.SetActive(true);
		if (gadgetPreview != null)
		{
			UnityEngine.Object.Destroy(gadgetPreview);
			gadgetPreview = null;
		}
		animationCoroutineRunner.StopAllCoroutines();
		if (WeaponManager.sharedManager == null)
		{
			return;
		}
		if (armorPoint.childCount > 0)
		{
			ArmorRefs component = armorPoint.GetChild(0).GetChild(0).GetComponent<ArmorRefs>();
			if (component != null)
			{
				if (component.leftBone != null)
				{
					Vector3 position = component.leftBone.position;
					Quaternion rotation = component.leftBone.rotation;
					component.leftBone.parent = armorPoint.GetChild(0).GetChild(0);
					component.leftBone.position = position;
					component.leftBone.rotation = rotation;
				}
				if (component.rightBone != null)
				{
					Vector3 position2 = component.rightBone.position;
					Quaternion rotation2 = component.rightBone.rotation;
					component.rightBone.parent = armorPoint.GetChild(0).GetChild(0);
					component.rightBone.position = position2;
					component.rightBone.rotation = rotation2;
				}
			}
		}
		List<Transform> list = new List<Transform>();
		foreach (Transform item in body.transform)
		{
			list.Add(item);
		}
		foreach (Transform item2 in list)
		{
			item2.parent = null;
			item2.position = new Vector3(0f, -10000f, 0f);
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		if (weaponTag == null)
		{
			return;
		}
		if (profile != null)
		{
			Resources.UnloadAsset(profile);
			profile = null;
		}
		ItemRecord byTag = ItemDb.GetByTag(weaponTag);
		if (byTag == null || string.IsNullOrEmpty(byTag.PrefabName))
		{
			Debug.Log("rec == null || string.IsNullOrEmpty(rec.PrefabName)");
			return;
		}
		GameObject gameObject = Resources.Load<GameObject>("Weapons/" + byTag.PrefabName);
		if (gameObject == null)
		{
			Debug.Log("pref==null");
			return;
		}
		profile = Resources.Load<AnimationClip>("ProfileAnimClips/" + gameObject.name + "_Profile");
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		DisableLightProbesRecursively(gameObject2);
		try
		{
			GameObject to = gameObject2.GetComponent<WeaponSounds>()._innerPars.gameObject;
			if (weaponSkinId != null)
			{
				WeaponSkinsManager.GetSkin(weaponSkinId).SetTo(to);
			}
			else
			{
				WeaponSkin skinForWeapon = WeaponSkinsManager.GetSkinForWeapon(byTag.PrefabName);
				if (skinForWeapon != null)
				{
					skinForWeapon.SetTo(gameObject2);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in setting weapon skin in SetWeapon: " + ex);
		}
		Player_move_c.SetLayerRecursively(gameObject2, LayerMask.NameToLayer("NGUIShop"));
		gameObject2.transform.parent = body.transform;
		weapon = gameObject2;
		weapon.transform.localScale = new Vector3(1f, 1f, 1f);
		weapon.transform.position = body.transform.position;
		weapon.transform.localPosition = Vector3.zero;
		weapon.transform.localRotation = Quaternion.identity;
		WeaponSounds component2 = weapon.GetComponent<WeaponSounds>();
		if (armorPoint.childCount > 0 && component2 != null)
		{
			ArmorRefs component3 = armorPoint.GetChild(0).GetChild(0).GetComponent<ArmorRefs>();
			if (component3 != null)
			{
				if (component3.leftBone != null && component2.LeftArmorHand != null)
				{
					component3.leftBone.parent = component2.LeftArmorHand;
					component3.leftBone.localPosition = Vector3.zero;
					component3.leftBone.localRotation = Quaternion.identity;
					component3.leftBone.localScale = new Vector3(1f, 1f, 1f);
				}
				if (component3.rightBone != null && component2.RightArmorHand != null)
				{
					component3.rightBone.parent = component2.RightArmorHand;
					component3.rightBone.localPosition = Vector3.zero;
					component3.rightBone.localRotation = Quaternion.identity;
					component3.rightBone.localScale = new Vector3(1f, 1f, 1f);
				}
			}
		}
		PlayWeaponAnimation();
		DisableGunflashes(weapon);
		if (SkinsController.currentSkinForPers != null)
		{
			SetSkinOnPers(SkinsController.currentSkinForPers);
		}
		_assignedWeaponTag = weaponTag;
		DisableLightProbesRecursively(gameObject2.gameObject);
	}

	internal static void SynchronizeAndroidPurchases(string comment)
	{
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
		{
			return;
		}
		Debug.LogFormat("Trying to synchronize purchases to cloud ({0})", comment);
		Action ResetWeaponManager = delegate
		{
			PlayerPrefs.DeleteKey("PendingGooglePlayGamesSync");
			if (WeaponManager.sharedManager != null)
			{
				int currentWeaponIndex = WeaponManager.sharedManager.CurrentWeaponIndex;
				string key = SceneManager.GetActiveScene().name;
				int value;
				if (!Defs.filterMaps.TryGetValue(key, out value))
				{
					value = 0;
				}
				WeaponManager.sharedManager.Reset(value);
				WeaponManager.sharedManager.CurrentWeaponIndex = currentWeaponIndex;
			}
			if (GuiActive)
			{
				sharedShop.UpdateIcons(false);
			}
		};
		switch (Defs.AndroidEdition)
		{
		case Defs.RuntimeAndroidEdition.Amazon:
			PurchasesSynchronizer.Instance.SynchronizeAmazonPurchases();
			ResetWeaponManager();
			break;
		case Defs.RuntimeAndroidEdition.GoogleLite:
			PlayerPrefs.SetInt("PendingGooglePlayGamesSync", 1);
			PurchasesSynchronizer.Instance.AuthenticateAndSynchronize(delegate(bool success)
			{
				Debug.LogFormat("[Rilisoft] ShopNguiController.PurchasesSynchronizer.Callback({0}) >: {1:F3}", success, Time.realtimeSinceStartup);
				try
				{
					Debug.LogFormat("Google purchases syncronized ({0}): {1}", comment, success);
					if (success)
					{
						ResetWeaponManager();
					}
				}
				finally
				{
					Debug.LogFormat("[Rilisoft] ShopNguiController.PurchasesSynchronizer.Callback({0}) <: {1:F3}", success, Time.realtimeSinceStartup);
				}
			}, true);
			break;
		}
	}

	public static void FireWeaponOrArmorBought()
	{
		Action gunOrArmorBought = ShopNGUIController.GunOrArmorBought;
		if (gunOrArmorBought != null)
		{
			gunOrArmorBought();
		}
	}

	public void SetItemToShow(ShopItem itemToSet)
	{
		m_itemToSetAfterEnter = itemToSet;
	}

	private void CreateCharacterModel()
	{
		GameObject original = Resources.Load("Character_model") as GameObject;
		characterInterface = UnityEngine.Object.Instantiate(original).GetComponent<CharacterInterface>();
		characterPoint.localPosition = new Vector3(-2.64f - ((float)Screen.width / (float)Screen.height - 1.333f) * 0.927f, characterPoint.localPosition.y, characterPoint.localPosition.z);
		characterInterface.transform.SetParent(characterPoint.GetChild(0), false);
		characterInterface.SetCharacterType(false, false, false);
		DisableLightProbesRecursively(characterInterface.gameObject);
		Player_move_c.SetLayerRecursively(characterInterface.gameObject, characterPoint.gameObject.layer);
	}

	private void ReparentButtons()
	{
		if (CurrentItem == null)
		{
			return;
		}
		if (IsWeaponCategory(CurrentItem.Category))
		{
			string text = WeaponManager.LastBoughtTag(CurrentItem.Id);
			int index = (((text != null && WeaponManager.FirstUnboughtOrForOurTier(CurrentItem.Id) != text) || (WeaponManager.sharedManager != null && WeaponManager.sharedManager.IsAvailableTryGun(CurrentItem.Id))) ? 1 : 2);
			Transform parent = buttonContainers[index];
			equipped.transform.parent = parent;
			equipButton.transform.parent = parent;
		}
		else if (IsWearCategory(CurrentItem.Category))
		{
			if (CurrentItem.Id == "cape_Custom")
			{
				equipped.transform.parent = buttonContainers[1];
				equipButton.transform.parent = buttonContainers[0];
				unequipButton.transform.parent = buttonContainers[0];
			}
			else
			{
				string text2 = WeaponManager.LastBoughtTag(CurrentItem.Id);
				bool flag = text2 != null && WeaponManager.FirstUnboughtTag(CurrentItem.Id) != text2;
				equipped.transform.parent = buttonContainers[flag ? 1 : 0];
				unequipButton.transform.parent = buttonContainers[(!flag) ? 2 : 0];
				equipButton.transform.parent = buttonContainers[(!flag) ? 2 : 0];
			}
		}
		else if (CurrentItem.Category == CategoryNames.SkinsCategory)
		{
			if (CurrentItem.Id == "CustomSkinID" || SkinsController.CustomSkinIds().Contains(CurrentItem.Id))
			{
				equipped.transform.parent = buttonContainers[1];
				equipButton.transform.parent = buttonContainers[1];
			}
			else
			{
				equipped.transform.parent = buttonContainers[2];
				equipButton.transform.parent = buttonContainers[2];
			}
		}
		else if (CurrentItem.Category == CategoryNames.LeagueWeaponSkinsCategory)
		{
			equipped.transform.parent = buttonContainers[0];
			equipButton.transform.parent = buttonContainers[2];
			unequipButton.transform.parent = buttonContainers[2];
		}
		else if (CurrentItem.Category == CategoryNames.EggsCategory)
		{
			equipped.transform.parent = buttonContainers[0];
			equipButton.transform.parent = buttonContainers[2];
			unequipButton.transform.parent = buttonContainers[2];
		}
		else if (CurrentItem.Category == CategoryNames.PetsCategory)
		{
			bool flag2 = Singleton<PetsManager>.Instance.PlayerPets.Count > 0 && Singleton<PetsManager>.Instance.GetNextUp(CurrentItem.Id) != null;
			equipped.transform.parent = buttonContainers[1];
			equipButton.transform.parent = buttonContainers[flag2 ? 1 : 2];
			unequipButton.transform.parent = buttonContainers[flag2 ? 1 : 2];
		}
		else if (IsGadgetsCategory(CurrentItem.Category))
		{
			string text3 = GadgetsInfo.LastBoughtFor(CurrentItem.Id);
			int index2 = ((text3 != null && text3 != GadgetsInfo.Upgrades[text3].Last()) ? 1 : 2);
			Transform parent2 = buttonContainers[index2];
			equipped.transform.parent = parent2;
			equipButton.transform.parent = parent2;
		}
		equipped.transform.localPosition = Vector3.zero;
		equipButton.transform.localPosition = Vector3.zero;
		unequipButton.transform.localPosition = Vector3.zero;
	}
}

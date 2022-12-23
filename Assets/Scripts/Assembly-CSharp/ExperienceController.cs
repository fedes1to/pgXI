using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class ExperienceController : MonoBehaviour
{
	[CompilerGenerated]
	private sealed class _003CInitController_003Ec__Iterator2E : IDisposable, IEnumerator<float>, IEnumerator
	{
		internal int _003Ci_003E__0;

		internal int _003Cd_003E__1;

		internal int _003Ci_003E__2;

		internal int _003Ck_003E__3;

		internal int _003Cmin_003E__4;

		internal int _003Cmax_003E__5;

		internal int _003Cd_003E__6;

		internal Exception _003Cex_003E__7;

		internal int _0024PC;

		internal float _0024current;

		internal ExperienceController _003C_003Ef__this;

		float IEnumerator<float>.Current
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
			//Discarded unreachable code: IL_02b8, IL_02cf
			uint num = (uint)_0024PC;
			_0024PC = -1;
			switch (num)
			{
			case 0u:
				for (_003Ci_003E__0 = 0; _003Ci_003E__0 < 31; _003Ci_003E__0++)
				{
					for (_003Cd_003E__1 = 0; _003Cd_003E__1 < 31; _003Cd_003E__1++)
					{
						accessByLevels[_003Ci_003E__0, _003Cd_003E__1] = 0;
					}
				}
				for (_003Ci_003E__2 = 0; _003Ci_003E__2 < 31; _003Ci_003E__2++)
				{
					for (_003Ck_003E__3 = _003C_003Ef__this.limitsLeveling.GetLowerBound(0); _003Ck_003E__3 <= _003C_003Ef__this.limitsLeveling.GetUpperBound(0); _003Ck_003E__3++)
					{
						_003Cmin_003E__4 = _003C_003Ef__this.limitsLeveling[_003Ck_003E__3, 0] - 1;
						_003Cmax_003E__5 = _003C_003Ef__this.limitsLeveling[_003Ck_003E__3, 1] - 1;
						if (_003Ci_003E__2 >= _003Cmin_003E__4 && _003Ci_003E__2 <= _003Cmax_003E__5)
						{
							for (_003Cd_003E__6 = _003Cmin_003E__4; _003Cd_003E__6 <= _003Cmax_003E__5; _003Cd_003E__6++)
							{
								accessByLevels[_003Ci_003E__2, _003Cd_003E__6] = 1;
							}
							break;
						}
					}
				}
				_0024current = 0f;
				_0024PC = 1;
				return true;
			case 1u:
				try
				{
					InitializeStoragerKeysIfNeeded();
					UnityEngine.Object.DontDestroyOnLoad(_003C_003Ef__this.gameObject);
					_003C_003Ef__this.Refresh();
					if (_003C_003Ef__this.currentLevel < 31 && _003C_003Ef__this.currentExperience.Value >= MaxExpLevels[_003C_003Ef__this.currentLevel])
					{
						_003C_003Ef__this.currentExperience = 0;
						_003C_003Ef__this.currentLevel++;
						Storager.setInt(GetCurrentLevelKey(_003C_003Ef__this.currentLevel), 1, true);
						Storager.setInt("currentExperience", _003C_003Ef__this.currentExperience.Value, false);
						_003C_003Ef__this.AddCurrenciesForLevelUP();
						BankController.canShowIndication = true;
						AnalyticsFacade.LevelUp(_003C_003Ef__this.currentLevel);
					}
					_003C_003Ef__this.isShowRanks = false;
				}
				catch (Exception ex)
				{
					_003Cex_003E__7 = ex;
					UnityEngine.Debug.LogError("<<< ExperienceController.Start() failed.");
					UnityEngine.Debug.LogException(_003Cex_003E__7);
					break;
				}
				finally
				{
					_003C_003E__Finally0();
				}
				_0024PC = -1;
				break;
			}
			return false;
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
			_003C_003Ef__this._initialized = true;
		}
	}

	public const int maxLevel = 31;

	public static int[] MaxExpLevelsDefault = new int[32]
	{
		0, 15, 35, 50, 90, 100, 110, 115, 120, 125,
		130, 135, 140, 150, 160, 170, 180, 200, 220, 250,
		290, 340, 400, 470, 550, 640, 740, 850, 970, 1100,
		1240, 100000
	};

	public static int[] MaxExpLevels = InitMaxLevelMass(MaxExpLevelsDefault);

	public static readonly float[] HealthByLevel = new float[32]
	{
		0f, 9f, 10f, 10f, 11f, 11f, 12f, 13f, 13f, 14f,
		14f, 15f, 16f, 16f, 17f, 17f, 18f, 19f, 19f, 20f,
		20f, 21f, 22f, 22f, 23f, 23f, 24f, 25f, 25f, 26f,
		26f, 27f
	};

	public bool isMenu;

	public bool isConnectScene;

	public int currentLevelForEditor;

	public int[,] limitsLeveling = new int[6, 2]
	{
		{ 1, 6 },
		{ 7, 11 },
		{ 12, 16 },
		{ 17, 21 },
		{ 22, 26 },
		{ 27, 31 }
	};

	public static int[,] accessByLevels = new int[31, 31];

	public Texture2D[] marks;

	private SaltedInt currentExperience = new SaltedInt(12512238, 0);

	private static int[] _addCoinsFromLevelsDefault = new int[32]
	{
		0, 5, 10, 15, 20, 25, 25, 25, 30, 30,
		30, 35, 35, 40, 40, 40, 45, 45, 50, 50,
		50, 55, 55, 60, 60, 60, 65, 65, 70, 70,
		70, 0
	};

	private static int[] _addCoinsFromLevels = InitAddCoinsFromLevels(_addCoinsFromLevelsDefault);

	private static int[] _addGemsFromLevelsDefault = new int[32]
	{
		0, 4, 4, 5, 5, 6, 6, 7, 7, 8,
		8, 9, 9, 10, 10, 11, 11, 12, 12, 13,
		13, 14, 14, 15, 15, 16, 16, 17, 17, 18,
		18, 0
	};

	private static int[] _addGemsFromLevels = InitAddGemsFromLevels(_addGemsFromLevelsDefault);

	private static bool _storagerKeysInitialized = false;

	private bool _isShowRanks = true;

	public bool isShowNextPlashka;

	public Vector2 posRanks = Vector2.zero;

	private int oldCurrentExperience;

	public int oldCurrentLevel;

	public bool isShowAdd;

	public AudioClip exp_1;

	public AudioClip exp_2;

	public AudioClip exp_3;

	public AudioClip Tierup;

	public static ExperienceController sharedController;

	private bool _initialized;

	private static readonly StringBuilder _sharedStringBuilder = new StringBuilder();

	public int AddHealthOnCurLevel
	{
		get
		{
			int num = currentLevel;
			if (HealthByLevel.Length > num && num > 0)
			{
				return (int)(HealthByLevel[num] - HealthByLevel[num - 1]);
			}
			return 0;
		}
	}

	public int currentLevel
	{
		get
		{
			return currentLevelForEditor;
		}
		private set
		{
			bool flag = false;
			if (currentLevelForEditor != value)
			{
				flag = true;
			}
			currentLevelForEditor = value;
			if (value >= 4)
			{
				ReviewController.CheckActiveReview();
			}
			if (flag && ExperienceController.onLevelChange != null)
			{
				ExperienceController.onLevelChange();
			}
		}
	}

	public static int[] addCoinsFromLevels
	{
		get
		{
			return _addCoinsFromLevels;
		}
	}

	public static int[] addGemsFromLevels
	{
		get
		{
			return _addGemsFromLevels;
		}
	}

	public int CurrentExperience
	{
		get
		{
			return currentExperience.Value;
		}
	}

	public bool isShowRanks
	{
		get
		{
			return _isShowRanks;
		}
		set
		{
			_isShowRanks = value;
			if (ExpController.Instance != null)
			{
				ExpController.Instance.InterfaceEnabled = value;
			}
		}
	}

	public static event Action onLevelChange;

	public ExperienceController()
	{
		currentLevel = 1;
	}

	public static int[] InitMaxLevelMass(int[] _mass)
	{
		int[] array = new int[_mass.Length];
		Array.Copy(_mass, array, _mass.Length);
		return array;
	}

	public static int[] InitAddCoinsFromLevels(int[] _mass)
	{
		int[] array = new int[_mass.Length];
		Array.Copy(_mass, array, _mass.Length);
		return array;
	}

	public static int[] InitAddGemsFromLevels(int[] _mass)
	{
		int[] array = new int[_mass.Length];
		Array.Copy(_mass, array, _mass.Length);
		return array;
	}

	public static void ResetLevelingOnDefault()
	{
		MaxExpLevels = InitMaxLevelMass(MaxExpLevelsDefault);
		_addCoinsFromLevels = InitAddCoinsFromLevels(_addCoinsFromLevelsDefault);
		_addGemsFromLevels = InitAddGemsFromLevels(_addGemsFromLevelsDefault);
	}

	public static void RewriteLevelingParametersForLevel(int _level, int _exp, int _coins, int _gems)
	{
		MaxExpLevels[_level] = _exp;
		_addCoinsFromLevels[_level] = _coins;
		_addGemsFromLevels[_level] = _gems;
	}

	public static void RewriteLevelingParametersForLevel(int _level, int _coins, int _gems)
	{
		try
		{
			_addCoinsFromLevels[_level] = _coins;
			_addGemsFromLevels[_level] = _gems;
		}
		catch
		{
			UnityEngine.Debug.LogWarning("augh, _level was " + _level);
		}
	}

	public void SetCurrentExperience(int _exp)
	{
		currentExperience = _exp;
		Storager.setInt("currentExperience", _exp, false);
		UnityEngine.Debug.Log(currentExperience.Value);
	}

	private static void InitializeStoragerKeysIfNeeded()
	{
		if (!_storagerKeysInitialized)
		{
			if (!Storager.hasKey("currentLevel1"))
			{
				Storager.setInt("currentLevel1", 1, true);
			}
			_storagerKeysInitialized = true;
		}
	}

	public static int GetCurrentLevelWithUpdateCorrection()
	{
		InitializeStoragerKeysIfNeeded();
		int num = GetCurrentLevel();
		if (num < 31 && Storager.getInt("currentExperience", false) >= MaxExpLevels[num])
		{
			num++;
		}
		return num;
	}

	public static int GetCurrentLevel()
	{
		int result = 1;
		for (int i = 1; i <= 31; i++)
		{
			string currentLevelKey = GetCurrentLevelKey(i);
			if (Storager.getInt(currentLevelKey, true) == 1)
			{
				result = i;
				Storager.setInt(currentLevelKey, 1, true);
			}
		}
		return result;
	}

	public void Refresh()
	{
		currentExperience = Storager.getInt("currentExperience", false);
		currentLevel = GetCurrentLevel();
	}

	private void AddCurrenciesForLevelUP()
	{
		int count = addGemsFromLevels[currentLevel - 1];
		BankController.canShowIndication = false;
		BankController.AddGems(count, false);
		if (currentLevel == 2 && BalanceController.startCapitalEnabled)
		{
			int startCapitalGems = BalanceController.startCapitalGems;
			BankController.AddGems(startCapitalGems, false);
		}
		StartCoroutine(BankController.WaitForIndicationGems(true));
		int count2 = addCoinsFromLevels[currentLevel - 1];
		BankController.AddCoins(count2, false);
		if (currentLevel == 2 && BalanceController.startCapitalEnabled)
		{
			int startCapitalCoins = BalanceController.startCapitalCoins;
			BankController.AddCoins(startCapitalCoins, false);
		}
		StartCoroutine(BankController.WaitForIndicationGems(false));
	}

	private void Awake()
	{
		sharedController = this;
		CoroutineRunner.Instance.StartCoroutine(WaitInitializationThenLogProgressInExperience());
	}

	private IEnumerator WaitInitializationThenLogProgressInExperience()
	{
		while (!_initialized)
		{
			yield return null;
		}
		while (AnalyticsFacade.FlurryFacade == null)
		{
			yield return null;
		}
		int levelBase1 = currentLevel;
		int tierBase1 = ExpController.OurTierForAnyPlace() + 1;
		AnalyticsStuff.LogProgressInExperience(levelBase1, tierBase1);
	}

	public IEnumerable<float> InitController()
	{
		for (int i = 0; i < 31; i++)
		{
			for (int d = 0; d < 31; d++)
			{
				accessByLevels[i, d] = 0;
			}
		}
		for (int j = 0; j < 31; j++)
		{
			for (int k = limitsLeveling.GetLowerBound(0); k <= limitsLeveling.GetUpperBound(0); k++)
			{
				int min = limitsLeveling[k, 0] - 1;
				int max = limitsLeveling[k, 1] - 1;
				if (j >= min && j <= max)
				{
					for (int d2 = min; d2 <= max; d2++)
					{
						accessByLevels[j, d2] = 1;
					}
					break;
				}
			}
		}
		yield return 0f;
		try
		{
			InitializeStoragerKeysIfNeeded();
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			Refresh();
			if (currentLevel < 31 && currentExperience.Value >= MaxExpLevels[currentLevel])
			{
				currentExperience = 0;
				currentLevel++;
				Storager.setInt(GetCurrentLevelKey(currentLevel), 1, true);
				Storager.setInt("currentExperience", currentExperience.Value, false);
				AddCurrenciesForLevelUP();
				BankController.canShowIndication = true;
				AnalyticsFacade.LevelUp(currentLevel);
			}
			isShowRanks = false;
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			UnityEngine.Debug.LogError("<<< ExperienceController.Start() failed.");
			UnityEngine.Debug.LogException(ex);
		}
	}

	public static void SendAnalyticsForLevelsFromCloud(int levelBefore)
	{
		if (sharedController == null)
		{
			UnityEngine.Debug.LogError("SendAnalyticsForLevelsFromCloud ExperienceController.sharedController == null");
			return;
		}
		for (int i = levelBefore + 1; i <= sharedController.currentLevel; i++)
		{
			AnalyticsFacade.LevelUp(i);
		}
	}

	public void addExperience(int experience)
	{
		if (currentLevel == 31)
		{
			return;
		}
		oldCurrentLevel = currentLevel;
		oldCurrentExperience = currentExperience.Value;
		if (currentLevel < 31 && experience >= MaxExpLevels[currentLevel] - currentExperience.Value + MaxExpLevels[currentLevel + 1])
		{
			experience = MaxExpLevels[currentLevel + 1] + MaxExpLevels[currentLevel] - currentExperience.Value - 5;
		}
		string key = "Statistics.ExpInMode.Level" + sharedController.currentLevel;
		if (PlayerPrefs.HasKey(key) && Initializer.lastGameMode != -1)
		{
			string key2 = Initializer.lastGameMode.ToString();
			string @string = PlayerPrefs.GetString(key, "{}");
			try
			{
				Dictionary<string, object> dictionary = (Json.Deserialize(@string) as Dictionary<string, object>) ?? new Dictionary<string, object>();
				object value;
				if (dictionary.TryGetValue(key2, out value))
				{
					int num = Convert.ToInt32(value) + experience;
					dictionary[key2] = num;
				}
				else
				{
					dictionary.Add(key2, experience);
				}
				string value2 = Json.Serialize(dictionary);
				PlayerPrefs.SetString(key, value2);
			}
			catch (OverflowException exception)
			{
				UnityEngine.Debug.LogError("Cannot deserialize exp-in-mode: " + @string);
				UnityEngine.Debug.LogException(exception);
			}
			catch (Exception exception2)
			{
				UnityEngine.Debug.LogError("Unknown exception: " + @string);
				UnityEngine.Debug.LogException(exception2);
			}
		}
		currentExperience = currentExperience.Value + experience;
		Storager.setInt("currentExperience", currentExperience.Value, false);
		if (currentLevel < 31 && currentExperience.Value >= MaxExpLevels[currentLevel])
		{
			DateTime utcNow = DateTime.UtcNow;
			string key3 = "Statistics.TimeInRank.Level" + (currentLevel + 1);
			PlayerPrefs.SetString(key3, utcNow.ToString("s"));
			string key4 = "Statistics.MatchCount.Level" + sharedController.currentLevel;
			int @int = PlayerPrefs.GetInt(key4, 0);
			string key5 = "Statistics.WinCount.Level" + sharedController.currentLevel;
			int int2 = PlayerPrefs.GetInt(key5, 0);
			AnalyticsFacade.SendCustomEventToAppsFlyer("af_level_achieved", new Dictionary<string, string> { 
			{
				"af_level",
				currentLevel.ToString()
			} });
			AnalyticsFacade.SendCustomEventToFacebook("level_reached", new Dictionary<string, object> { { "level", currentLevel } });
			currentExperience = currentExperience.Value - MaxExpLevels[currentLevel];
			currentLevel++;
			PlayerPrefs.SetString(LeaderboardScript.LeaderboardsResponseCacheTimestamp, DateTime.UtcNow.Subtract(TimeSpan.FromHours(2.0)).ToString("s", CultureInfo.InvariantCulture));
			if (currentLevel == 3)
			{
				AnalyticsStuff.TrySendOnceToAppsFlyer("levelup_3");
			}
			if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted && currentLevel > 1)
			{
				if (Storager.getInt("Training.NoviceArmorUsedKey", false) == 1)
				{
					Storager.setInt("Training.ShouldRemoveNoviceArmorInShopKey", 1, false);
					if (HintController.instance != null)
					{
						HintController.instance.ShowHintByName("shop_remove_novice_armor", 2.5f);
					}
				}
				TrainingController.CompletedTrainingStage = TrainingController.NewTrainingCompletedStage.FirstMatchCompleted;
				AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Finished);
				if (!Storager.hasKey("Analytics:tutorial_levelup"))
				{
					Storager.setString("Analytics:tutorial_levelup", "{}", false);
					AnalyticsFacade.SendCustomEventToAppsFlyer("tutorial_levelup", new Dictionary<string, string>());
					Storager.setString("Analytics:af_tutorial_completion", "{}", false);
					AnalyticsFacade.SendCustomEventToAppsFlyer("af_tutorial_completion", new Dictionary<string, string>());
				}
				try
				{
					if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.playerWeapons != null)
					{
						IEnumerable<Weapon> source = WeaponManager.sharedManager.playerWeapons.OfType<Weapon>();
						if (source.FirstOrDefault((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == 3) == null)
						{
							WeaponManager.sharedManager.EquipWeapon(WeaponManager.sharedManager.allAvailablePlayerWeapons.OfType<Weapon>().First((Weapon w) => w.weaponPrefab.name.Replace("(Clone)", string.Empty) == WeaponManager.SimpleFlamethrower_WN));
						}
						if (source.FirstOrDefault((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == 5) == null)
						{
							WeaponManager.sharedManager.EquipWeapon(WeaponManager.sharedManager.allAvailablePlayerWeapons.OfType<Weapon>().First((Weapon w) => w.weaponPrefab.name.Replace("(Clone)", string.Empty) == WeaponManager.Rocketnitza_WN));
						}
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogError("Exception in gequipping flamethrower and rocketniza: " + ex);
				}
			}
			if (currentLevel == 2)
			{
				try
				{
					EggData eggData = Singleton<EggsManager>.Instance.GetEggData("egg_champion_steel");
					Singleton<EggsManager>.Instance.AddEgg(eggData);
				}
				catch (Exception ex2)
				{
					UnityEngine.Debug.LogErrorFormat("Exception in giving after-training steel league egg: {0}", ex2);
				}
			}
			Storager.setInt(GetCurrentLevelKey(currentLevel), 1, true);
			Storager.setInt("currentExperience", currentExperience.Value, false);
			ShopNGUIController.SynchronizeAndroidPurchases("Current level: " + currentLevel);
			AddCurrenciesForLevelUP();
			FriendsController.sharedController.rank = currentLevel;
			FriendsController.sharedController.SendOurData(false);
			FriendsController.sharedController.UpdatePopularityMaps();
			AnalyticsFacade.LevelUp(currentLevel);
		}
		if (Defs.isSoundFX)
		{
			NGUITools.PlaySound(exp_1);
		}
		if (ExpController.Instance != null)
		{
			ExpController.Instance.AddExperience(oldCurrentLevel, oldCurrentExperience, experience, exp_2, exp_3, Tierup);
		}
	}

	private void HideNextPlashka()
	{
		isShowNextPlashka = false;
		isShowAdd = false;
	}

	private void DoOnGUI()
	{
	}

	public static void SetEnable(bool enable)
	{
		if (!(sharedController == null))
		{
			sharedController.isShowRanks = enable;
		}
	}

	private static string GetCurrentLevelKey(int levelIndex)
	{
		_sharedStringBuilder.Length = 0;
		_sharedStringBuilder.Append("currentLevel").Append(levelIndex);
		string result = _sharedStringBuilder.ToString();
		_sharedStringBuilder.Length = 0;
		return result;
	}
}

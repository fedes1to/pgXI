using System;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameController
{
	public static bool HasSurvivalRecord;

	public static bool LeftHanded;

	public static bool switchingWeaponSwipe;

	public static List<int> survScoreThresh;

	public static int curThr;

	public static int thrStep;

	public static Font fontHolder;

	public static int EditingLogo;

	public static string TempClanName;

	public static Texture2D LogoToEdit;

	public static List<Texture2D> Logos;

	public static readonly int NumOfLevels;

	private static int _currentLevel;

	private static int _allLevelsCompleted;

	public static bool showTableMyPlayer;

	public static bool imDeadInHungerGame;

	public static bool isFullVersion;

	public static Vector3 posMyPlayer;

	public static Quaternion rotMyPlayer;

	public static float healthMyPlayer;

	public static bool is60FPSEnableInit;

	public static bool _is60FPSEnable;

	public static int numOfCompletedLevels;

	public static int totalNumOfCompletedLevels;

	public static int countKillsBlue;

	public static int countKillsRed;

	public static int EditingCape;

	public static bool EditedCapeSaved;

	private static int? _enemiesToKillOverride;

	private static SaltedInt _saltedScore;

	private static SaltedInt _saltedCountKills;

	private static int _countDaySessionInCurrentVersion;

	public static int coinsBase;

	public static int coinsBaseAdding;

	public static int levelsToGetCoins;

	public static readonly string AppVersion;

	public static float armorMyPlayer { get; set; }

	public static int currentLevel
	{
		get
		{
			return _currentLevel;
		}
		set
		{
			_currentLevel = value;
		}
	}

	public static int AllLevelsCompleted
	{
		get
		{
			return _allLevelsCompleted;
		}
		set
		{
			_allLevelsCompleted = value;
		}
	}

	public static bool is60FPSEnable
	{
		get
		{
			if (!is60FPSEnableInit)
			{
				_is60FPSEnable = PlayerPrefs.GetInt("fps60Enable", (!Device.isPixelGunLow) ? 1 : 0) == 1;
				is60FPSEnableInit = true;
			}
			return _is60FPSEnable;
		}
		set
		{
			_is60FPSEnable = value;
			PlayerPrefs.SetInt("fps60Enable", _is60FPSEnable ? 1 : 0);
			is60FPSEnableInit = true;
			Application.targetFrameRate = ((!_is60FPSEnable) ? 30 : 60);
		}
	}

	public static int ZombiesInWave
	{
		get
		{
			return 4;
		}
	}

	public static int EnemiesToKill
	{
		get
		{
			return GetEnemiesToKill(SceneManager.GetActiveScene().name);
		}
		set
		{
			_enemiesToKillOverride = value;
		}
	}

	internal static int Score
	{
		get
		{
			return _saltedScore.Value;
		}
		set
		{
			_saltedScore.Value = value;
		}
	}

	internal static int CountKills
	{
		get
		{
			return _saltedCountKills.Value;
		}
		set
		{
			_saltedCountKills.Value = value;
		}
	}

	public static int CountDaySessionInCurrentVersion
	{
		get
		{
			if (_countDaySessionInCurrentVersion == -1)
			{
				_countDaySessionInCurrentVersion = PlayerPrefs.GetInt(Defs.SessionDayNumberKey, 1) - PlayerPrefs.GetInt("countSessionDayOnStartCorrentVersion", 1);
			}
			return _countDaySessionInCurrentVersion;
		}
		set
		{
			_countDaySessionInCurrentVersion = value;
		}
	}

	public static int SimultaneousEnemiesOnLevelConstraint
	{
		get
		{
			return 20;
		}
	}

	internal static bool NewVersionAvailable { get; set; }

	public static string MultiplayerProtocolVersion
	{
		get
		{
			return "11.3.0";
		}
	}

	static GlobalGameController()
	{
		LeftHanded = true;
		switchingWeaponSwipe = false;
		survScoreThresh = new List<int>();
		thrStep = 10000;
		fontHolder = null;
		EditingLogo = 0;
		NumOfLevels = 11;
		_currentLevel = -1;
		_allLevelsCompleted = 0;
		showTableMyPlayer = false;
		imDeadInHungerGame = false;
		isFullVersion = true;
		is60FPSEnableInit = false;
		numOfCompletedLevels = 0;
		totalNumOfCompletedLevels = 0;
		countKillsBlue = 0;
		countKillsRed = 0;
		EditingCape = 0;
		EditedCapeSaved = false;
		_saltedScore = new SaltedInt(233495534);
		_saltedCountKills = new SaltedInt(233495534);
		_countDaySessionInCurrentVersion = -1;
		coinsBase = 1;
		coinsBaseAdding = 0;
		levelsToGetCoins = 1;
		AppVersion = "11.3.1";
	}

	public static void SetMultiMode()
	{
		Defs.isMulti = true;
		Defs.isCOOP = false;
		Defs.isHunger = false;
		Defs.isDuel = false;
		Defs.isCompany = false;
		Defs.isFlag = false;
		Defs.IsSurvival = false;
		Defs.isCapturePoints = false;
	}

	private static void Swap(IList<int> list, int indexA, int indexB)
	{
		int value = list[indexA];
		list[indexA] = list[indexB];
		list[indexB] = value;
	}

	internal static int GetEnemiesToKill(string sceneName)
	{
		if (!TrainingController.TrainingCompleted || StringComparer.OrdinalIgnoreCase.Equals(sceneName, Defs.TrainingSceneName))
		{
			return 3;
		}
		if (_enemiesToKillOverride.HasValue)
		{
			return _enemiesToKillOverride.Value;
		}
		if (!Defs.IsSurvival)
		{
			return ZombieCreator.GetCountMobsForLevel();
		}
		return 35;
	}

	public static void ResetParameters()
	{
		AllLevelsCompleted = 0;
		numOfCompletedLevels = -1;
		totalNumOfCompletedLevels = -1;
	}

	public static void GoInBattle()
	{
		Defs.isFlag = false;
		Defs.isCOOP = false;
		Defs.isMulti = true;
		Defs.isHunger = false;
		Defs.isDuel = false;
		Defs.isCompany = false;
		Defs.IsSurvival = false;
		Defs.isFlag = false;
		MenuBackgroundMusic.keepPlaying = true;
		LoadConnectScene.textureToShow = null;
		LoadConnectScene.sceneToLoad = "ConnectScene";
		LoadConnectScene.noteToShow = null;
		SceneManager.LoadScene(Defs.PromSceneName);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class SceneInfoController : MonoBehaviour
{
	private const float timerUpdateDataFromServer = 870f;

	public static SceneInfoController instance = null;

	public List<SceneInfo> allScenes = new List<SceneInfo>();

	public List<AllScenesForMode> modeInfo = new List<AllScenesForMode>();

	private bool _isLoadingDataActive;

	private List<SceneInfo> copyAllScenes;

	private List<AllScenesForMode> copyModeInfo;

	private static readonly Dictionary<TypeModeGame, int> _modeUnlockLevels = new Dictionary<TypeModeGame, int>(TypeModeGameComparer.Instance)
	{
		{
			TypeModeGame.Deathmatch,
			1
		},
		{
			TypeModeGame.TeamFight,
			2
		},
		{
			TypeModeGame.TimeBattle,
			3
		},
		{
			TypeModeGame.FlagCapture,
			4
		},
		{
			TypeModeGame.DeadlyGames,
			5
		},
		{
			TypeModeGame.CapturePoints,
			6
		}
	};

	private static readonly Dictionary<TypeModeGame, ConnectSceneNGUIController.RegimGame> _modesMap = new Dictionary<TypeModeGame, ConnectSceneNGUIController.RegimGame>(TypeModeGameComparer.Instance)
	{
		{
			TypeModeGame.Deathmatch,
			ConnectSceneNGUIController.RegimGame.Deathmatch
		},
		{
			TypeModeGame.TeamFight,
			ConnectSceneNGUIController.RegimGame.TeamFight
		},
		{
			TypeModeGame.TimeBattle,
			ConnectSceneNGUIController.RegimGame.TimeBattle
		},
		{
			TypeModeGame.FlagCapture,
			ConnectSceneNGUIController.RegimGame.FlagCapture
		},
		{
			TypeModeGame.DeadlyGames,
			ConnectSceneNGUIController.RegimGame.DeadlyGames
		},
		{
			TypeModeGame.CapturePoints,
			ConnectSceneNGUIController.RegimGame.CapturePoints
		},
		{
			TypeModeGame.Duel,
			ConnectSceneNGUIController.RegimGame.Duel
		}
	};

	private Version CurrentVersion
	{
		get
		{
			return GetType().Assembly.GetName().Version;
		}
	}

	public static string UrlForLoadData
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/infomap_pixelgun_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/infomap_pixelgun_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/infomap_pixelgun_android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/infomap_pixelgun_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/infomap_pixelgun_wp8.json";
			}
			return string.Empty;
		}
	}

	public static event Action onChangeInfoMap;

	private void Awake()
	{
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		ExperienceController.onLevelChange += UpdateListAvaliableMap;
		LocalizationStore.AddEventCallAfterLocalize(OnChangeLocalize);
		UpdateListAvaliableMap();
	}

	private void OnDestroy()
	{
		ExperienceController.onLevelChange -= UpdateListAvaliableMap;
		instance = null;
	}

	public void UpdateListAvaliableMap()
	{
		if (!_isLoadingDataActive)
		{
			_isLoadingDataActive = true;
			TextAsset textAsset = Resources.Load<TextAsset>("infomap_pixelgun_test");
			if (textAsset != null)
			{
				StartCoroutine(ParseLoadData(textAsset.text));
			}
			else
			{
				Debug.LogWarning("Bindata == null");
			}
		}
	}

	public SceneInfo GetInfoScene(string nameScene)
	{
		return allScenes.Find((SceneInfo curInf) => StringComparer.OrdinalIgnoreCase.Equals(curInf.gameObject.name, nameScene));
	}

	public SceneInfo GetInfoScene(string nameScene, List<SceneInfo> needList)
	{
		if (needList == null)
		{
			return null;
		}
		return needList.Find((SceneInfo curInf) => StringComparer.OrdinalIgnoreCase.Equals(curInf.gameObject.name, nameScene));
	}

	public SceneInfo GetInfoScene(TypeModeGame needMode, int indexMap)
	{
		SceneInfo infoScene = GetInfoScene(indexMap);
		if (infoScene != null && infoScene.IsAvaliableForMode(needMode))
		{
			return infoScene;
		}
		return null;
	}

	public SceneInfo GetInfoScene(int indexMap)
	{
		return allScenes.Find((SceneInfo curInf) => curInf.indexMap == indexMap);
	}

	public int GetMaxCountMapsInRegims()
	{
		int num = 0;
		foreach (AllScenesForMode item in modeInfo)
		{
			if (item.avaliableScenes.Count > num)
			{
				num = item.avaliableScenes.Count;
			}
		}
		return num;
	}

	public AllScenesForMode GetListScenesForMode(TypeModeGame needMode)
	{
		return modeInfo.Find((AllScenesForMode mG) => mG.mode == needMode);
	}

	public AllScenesForMode GetListScenesForMode(TypeModeGame needMode, List<AllScenesForMode> needList)
	{
		if (needList == null)
		{
			return null;
		}
		return needList.Find((AllScenesForMode mG) => mG.mode == needMode);
	}

	public int GetCountScenesForMode(TypeModeGame needMode)
	{
		AllScenesForMode allScenesForMode = modeInfo.Find((AllScenesForMode nM) => nM.mode == needMode);
		if (allScenesForMode != null)
		{
			return allScenesForMode.avaliableScenes.Count;
		}
		return 0;
	}

	private void AddSceneIfAvaliableVersion(string nameScene, string minVersion, string maxVersion)
	{
		SceneInfo infoScene = GetInfoScene(nameScene, copyAllScenes);
		if (infoScene == null)
		{
			Version currentVersion = CurrentVersion;
			Version version = new Version(maxVersion);
			Version version2 = new Version(minVersion);
			if (currentVersion >= version2 && currentVersion <= version)
			{
				GameObject gameObject = Resources.Load("SceneInfo/" + nameScene) as GameObject;
				SceneInfo component = gameObject.GetComponent<SceneInfo>();
				GameObject gameObject2 = UnityEngine.Object.Instantiate(component.gameObject);
				gameObject2.transform.SetParent(base.transform);
				gameObject2.gameObject.name = nameScene;
				component = gameObject2.GetComponent<SceneInfo>();
				component.minAvaliableVersion = minVersion;
				component.maxAvaliableVersion = maxVersion;
				component.UpdateKeyLoaded();
				copyAllScenes.Add(component);
			}
		}
	}

	public bool MapExistInProject(string nameScene)
	{
		return true;
	}

	private void AddSceneInModeGame(string nameScene, TypeModeGame needMode)
	{
		SceneInfo infoScene = GetInfoScene(nameScene, copyAllScenes);
		if (!(infoScene != null))
		{
			return;
		}
		infoScene.AddMode(needMode);
		if (infoScene.IsLoaded)
		{
			AllScenesForMode allScenesForMode = GetListScenesForMode(needMode, copyModeInfo);
			if (allScenesForMode == null)
			{
				allScenesForMode = new AllScenesForMode();
				allScenesForMode.mode = needMode;
				copyModeInfo.Add(allScenesForMode);
			}
			allScenesForMode.AddInfoScene(infoScene);
		}
	}

	private IEnumerator GetDataFromServerLoop()
	{
		while (true)
		{
			yield return StartCoroutine(DownloadDataFormServer());
			yield return new WaitForRealSeconds(870f);
		}
	}

	private IEnumerator DownloadDataFormServer()
	{
		if (_isLoadingDataActive)
		{
			yield break;
		}
		_isLoadingDataActive = true;
		string urlDataAddress = UrlForLoadData;
		WWW downloadData = null;
		int iter = 3;
		while (iter > 0)
		{
			downloadData = Tools.CreateWwwIfNotConnected(urlDataAddress);
			if (downloadData == null)
			{
				yield break;
			}
			while (!downloadData.isDone)
			{
				yield return null;
			}
			if (!string.IsNullOrEmpty(downloadData.error))
			{
				yield return new WaitForRealSeconds(5f);
				iter--;
				continue;
			}
			break;
		}
		if (downloadData == null || !string.IsNullOrEmpty(downloadData.error))
		{
			if (Defs.IsDeveloperBuild && downloadData != null)
			{
				Debug.LogWarningFormat("Request to {0} failed: {1}", urlDataAddress, downloadData.error);
			}
			_isLoadingDataActive = false;
		}
		else
		{
			string responseText = URLs.Sanitize(downloadData);
			yield return ParseLoadData(responseText);
			_isLoadingDataActive = false;
		}
	}

	private IEnumerator ParseLoadData(string lData)
	{
		Dictionary<string, object> allData = Json.Deserialize(lData) as Dictionary<string, object>;
		if (allData == null)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogError("Bad response: " + lData);
			}
			_isLoadingDataActive = false;
			yield break;
		}
		while (ExperienceController.sharedController == null)
		{
			yield return null;
		}
		copyAllScenes = new List<SceneInfo>();
		copyModeInfo = new List<AllScenesForMode>();
		copyModeInfo.Clear();
		if (allData.ContainsKey("allAvaliableMap"))
		{
			List<object> listMap = allData["allAvaliableMap"] as List<object>;
			for (int iM = 0; iM < listMap.Count; iM++)
			{
				Dictionary<string, object> infoMap = listMap[iM] as Dictionary<string, object>;
				if (infoMap == null)
				{
					continue;
				}
				string curNameScene2 = string.Empty;
				string minV = string.Empty;
				string maxV = string.Empty;
				if (infoMap.ContainsKey("nameScene"))
				{
					curNameScene2 = infoMap["nameScene"].ToString();
					if (infoMap.ContainsKey("minV"))
					{
						minV = infoMap["minV"].ToString();
					}
					if (infoMap.ContainsKey("maxV"))
					{
						maxV = infoMap["maxV"].ToString();
					}
					AddSceneIfAvaliableVersion(curNameScene2, minV, maxV);
				}
			}
		}
		if (allData.ContainsKey("modeMap"))
		{
			List<object> listAllMode = allData["modeMap"] as List<object>;
			for (int iMod = 0; iMod < listAllMode.Count; iMod++)
			{
				Dictionary<string, object> infoMode = listAllMode[iMod] as Dictionary<string, object>;
				if (infoMode == null || !infoMode.ContainsKey("modeId"))
				{
					continue;
				}
				TypeModeGame curModeMap = ConvertModeToEnum(infoMode["modeId"].ToString());
				if (!infoMode.ContainsKey("scenesForMode"))
				{
					continue;
				}
				List<object> listModeScenes = infoMode["scenesForMode"] as List<object>;
				for (int iSc = 0; iSc < listModeScenes.Count; iSc++)
				{
					Dictionary<string, object> curSceneInf = listModeScenes[iSc] as Dictionary<string, object>;
					if (curSceneInf == null)
					{
						continue;
					}
					bool avalForCurLev = true;
					if (curSceneInf.ContainsKey("minLevPlayerForAval"))
					{
						int minAvalLev = Convert.ToInt32(curSceneInf["minLevPlayerForAval"]);
						if (ExperienceController.sharedController.currentLevel < minAvalLev)
						{
							avalForCurLev = false;
						}
					}
					if (curSceneInf.ContainsKey("ratingCount"))
					{
						//int ratCount = Convert.ToInt32(curSceneInf["ratingCount"]);
						//if (RatingSystem.instance.currentRating < ratCount)
						//{
						//	avalForCurLev = false;
						//}
					}
					if (avalForCurLev && curSceneInf.ContainsKey("nameScene"))
					{
						AddSceneInModeGame(curSceneInf["nameScene"].ToString(), curModeMap);
					}
				}
			}
		}
		OnDataLoaded();
	}

	public static TypeModeGame ConvertModeToEnum(string modeStr)
	{
		switch (modeStr)
		{
		case "Deathmatch":
			return TypeModeGame.Deathmatch;
		case "TimeBattle":
			return TypeModeGame.TimeBattle;
		case "TeamFight":
			return TypeModeGame.TeamFight;
		case "DeadlyGames":
			return TypeModeGame.DeadlyGames;
		case "FlagCapture":
			return TypeModeGame.FlagCapture;
		case "CapturePoints":
			return TypeModeGame.CapturePoints;
		case "Dater":
			return TypeModeGame.Dater;
		case "Duel":
			return TypeModeGame.Duel;
		default:
			return TypeModeGame.Deathmatch;
		}
	}

	internal static HashSet<TypeModeGame> GetUnlockedModesByLevel(int level)
	{
		HashSet<TypeModeGame> hashSet = new HashSet<TypeModeGame>();
		foreach (KeyValuePair<TypeModeGame, int> modeUnlockLevel in _modeUnlockLevels)
		{
			if (modeUnlockLevel.Value <= level)
			{
				hashSet.Add(modeUnlockLevel.Key);
			}
		}
		return hashSet;
	}

	internal static HashSet<ConnectSceneNGUIController.RegimGame> SelectModes(IEnumerable<TypeModeGame> modes)
	{
		HashSet<ConnectSceneNGUIController.RegimGame> hashSet = new HashSet<ConnectSceneNGUIController.RegimGame>();
		foreach (TypeModeGame mode in modes)
		{
			ConnectSceneNGUIController.RegimGame value;
			if (_modesMap.TryGetValue(mode, out value))
			{
				hashSet.Add(value);
			}
		}
		return hashSet;
	}

	private void OnDataLoaded()
	{
		allScenes = copyAllScenes;
		modeInfo = copyModeInfo;
		OnChangeLocalize();
		if (SceneInfoController.onChangeInfoMap != null)
		{
			SceneInfoController.onChangeInfoMap();
		}
		_isLoadingDataActive = false;
	}

	private void OnChangeLocalize()
	{
		for (int i = 0; i < allScenes.Count; i++)
		{
			allScenes[i].UpdateLocalize();
		}
	}
}

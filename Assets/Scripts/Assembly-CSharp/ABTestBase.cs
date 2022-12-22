using System;
using System.Collections;
using System.Collections.Generic;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public abstract class ABTestBase
{
	private const string baseFolder = "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests";

	private bool _isConfigNameInit;

	private string _configName = "none";

	private ABTestController.ABTestCohortsType _cohort;

	private bool _isInitCohort;

	private bool isRunGetABTestConfig;

	public abstract string currentFolder { get; }

	private string platformFolder
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "test";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "android" : "amazon";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "wp";
			}
			return "ios";
		}
	}

	private string url
	{
		get
		{
			return string.Format("{0}/{1}/abtestconfig_{2}.json", "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests", currentFolder, platformFolder);
		}
	}

	private string configNameKey
	{
		get
		{
			return string.Format("CN_{0}", currentFolder);
		}
	}

	public string configName
	{
		get
		{
			if (!_isConfigNameInit)
			{
				_configName = PlayerPrefs.GetString(configNameKey, "none");
				_isConfigNameInit = true;
			}
			return _configName;
		}
		set
		{
			_isConfigNameInit = true;
			_configName = value;
			PlayerPrefs.SetString(configNameKey, _configName);
		}
	}

	private string cohortKey
	{
		get
		{
			return string.Format("cohort_{0}", currentFolder);
		}
	}

	public ABTestController.ABTestCohortsType cohort
	{
		get
		{
			if (!_isInitCohort)
			{
				_cohort = (ABTestController.ABTestCohortsType)PlayerPrefs.GetInt(cohortKey, 0);
				_isInitCohort = true;
			}
			return _cohort;
		}
		set
		{
			_cohort = value;
			_isInitCohort = true;
			PlayerPrefs.SetInt(cohortKey, (int)_cohort);
		}
	}

	public string cohortName
	{
		get
		{
			return configName + cohort;
		}
	}

	private string abTestConfigKey
	{
		get
		{
			return string.Format("abTest{0}ConfigKey", currentFolder);
		}
	}

	public void UpdateABTestConfig()
	{
		CoroutineRunner.Instance.StartCoroutine(GetABTestConfig());
	}

	private IEnumerator GetABTestConfig()
	{
		while (!isRunGetABTestConfig)
		{
			isRunGetABTestConfig = true;
			WWWForm form = new WWWForm();
			WWW download = Tools.CreateWwwIfNotConnected(URLs.ABTestQuestSystemURL);
			if (download == null)
			{
				yield return CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.WaitForSeconds(30f + (float)UnityEngine.Random.Range(0, 10)));
				continue;
			}
			yield return download;
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild || Application.isEditor)
				{
					Debug.LogWarning(string.Format("GetABTest {0} error: {1}", currentFolder, download.error));
				}
				yield return CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.WaitForSeconds(30f + (float)UnityEngine.Random.Range(0, 10)));
				continue;
			}
			string responseText = URLs.Sanitize(download);
			if (!string.IsNullOrEmpty(responseText))
			{
				Storager.setString(abTestConfigKey, responseText, false);
				ParseABTestConfig(false);
			}
			isRunGetABTestConfig = false;
			break;
		}
	}

	public void InitTest()
	{
		if (Storager.hasKey(abTestConfigKey))
		{
			ParseABTestConfig(false);
		}
		else
		{
			Storager.setString(abTestConfigKey, string.Empty, false);
		}
	}

	private void ParseABTestConfig(bool isFromReset = false)
	{
		if (string.IsNullOrEmpty(Storager.getString(abTestConfigKey, false)))
		{
			return;
		}
		string @string = Storager.getString(abTestConfigKey, false);
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary == null || !dictionary.ContainsKey("enableABTest"))
		{
			return;
		}
		int num = Convert.ToInt32(dictionary["enableABTest"]);
		object settingsB = null;
		if (dictionary.ContainsKey("SettingsB"))
		{
			settingsB = dictionary["SettingsB"];
		}
		if (num == 1 && cohort != ABTestController.ABTestCohortsType.SKIP)
		{
			if (cohort == ABTestController.ABTestCohortsType.NONE)
			{
				configName = Convert.ToString(dictionary["configName"]);
				int num2 = UnityEngine.Random.Range(1, 3);
				cohort = (ABTestController.ABTestCohortsType)num2;
				AnalyticsStuff.LogABTest(currentFolder, cohortName);
				if (FriendsController.sharedController != null)
				{
					FriendsController.sharedController.SendOurData(false);
				}
			}
			ApplyState(cohort, settingsB);
		}
		else
		{
			if (!isFromReset)
			{
				ResetABTest();
			}
			bool flag = false;
			if (dictionary.ContainsKey("currentStateIsB"))
			{
				flag = Convert.ToBoolean(dictionary["currentStateIsB"]);
			}
			ApplyState((!flag) ? ABTestController.ABTestCohortsType.A : ABTestController.ABTestCohortsType.B, settingsB);
		}
	}

	protected virtual void ApplyState(ABTestController.ABTestCohortsType _state, object settingsB)
	{
	}

	public void ResetABTest()
	{
		if (cohort == ABTestController.ABTestCohortsType.SKIP)
		{
			return;
		}
		if (cohort == ABTestController.ABTestCohortsType.A || cohort == ABTestController.ABTestCohortsType.B)
		{
			AnalyticsStuff.LogABTest(currentFolder, cohortName, false);
			if (FriendsController.sharedController != null)
			{
				FriendsController.sharedController.SendOurData(false);
			}
		}
		cohort = ABTestController.ABTestCohortsType.SKIP;
		ParseABTestConfig(true);
	}
}

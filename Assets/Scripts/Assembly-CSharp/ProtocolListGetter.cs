using System.Collections;
using Rilisoft;
using UnityEngine;

public sealed class ProtocolListGetter : MonoBehaviour
{
	public static bool currentVersionIsSupported = true;

	private string CurrentVersionSupportedKey = "CurrentVersionSupportedKey" + GlobalGameController.AppVersion;

	public static int CurrentPlatform
	{
		get
		{
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return 0;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
			{
				return 1;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return 2;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return 2;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				return 3;
			}
			return 101;
		}
	}

	private IEnumerator Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		if (!Storager.hasKey(CurrentVersionSupportedKey))
		{
			Storager.setInt(CurrentVersionSupportedKey, 1, false);
		}
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage <= TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
		{
			yield return null;
		}
		currentVersionIsSupported = Storager.getInt(CurrentVersionSupportedKey, false) == 1;
		WaitForSeconds waitForSeconds = new WaitForSeconds(10f);
		string appVersionField = CurrentPlatform + ":" + GlobalGameController.AppVersion;
		WWWForm form = new WWWForm();
		form.AddField("action", "check_version");
		form.AddField("app_version", appVersionField);
		string response;
		while (true)
		{
			WWW download = Tools.CreateWwwIfNotConnected(FriendsController.actionAddress, form, string.Empty);
			if (download == null)
			{
				yield return waitForSeconds;
				continue;
			}
			yield return download;
			response = URLs.Sanitize(download);
			if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
			{
				Debug.Log(response);
			}
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log("ProtocolListGetter error: " + download.error);
				}
				yield return waitForSeconds;
			}
			else
			{
				if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response))
				{
					break;
				}
				yield return waitForSeconds;
			}
		}
		if ("no".Equals(response))
		{
			currentVersionIsSupported = false;
			Storager.setInt(CurrentVersionSupportedKey, 0, false);
		}
		else
		{
			currentVersionIsSupported = true;
			Storager.setInt(CurrentVersionSupportedKey, 1, false);
		}
	}
}

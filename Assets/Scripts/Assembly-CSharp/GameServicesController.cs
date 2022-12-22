using System.Collections;
using Rilisoft;
using UnityEngine;

internal sealed class GameServicesController : MonoBehaviour
{
	private static GameServicesController _instance;

	public static GameServicesController Instance
	{
		get
		{
			return _instance;
		}
	}

	public void WaitAuthenticationAndIncrementBeginnerAchievement()
	{
		using (new StopwatchLogger("WaitAuthenticationAndIncrementBeginnerAchievement()"))
		{
			StartCoroutine(WaitAndIncrementBeginnerAchievementCoroutine());
		}
	}

	private static IEnumerator WaitAndIncrementBeginnerAchievementCoroutine()
	{
		int oldGamesStartedCount = PlayerPrefs.GetInt("GamesStartedCount", 0);
		int newGamesStartedCount = oldGamesStartedCount + 1;
		PlayerPrefs.SetInt("GamesStartedCount", newGamesStartedCount);
		float step = 20f;
		switch (BuildSettings.BuildTargetPlatform)
		{
		case RuntimePlatform.Android:
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
				Debug.Log("Social platform local user authenticated: " + Social.localUser.authenticated);
				while (!Social.localUser.authenticated)
				{
					yield return new WaitForSeconds(2f);
				}
				Debug.Log("Social platform local user authenticated: " + Social.localUser.userName + ",\t\tid: " + Social.localUser.id);
				Debug.Log("Trying to increment Beginner achievement...");
				GpgFacade.Instance.IncrementAchievement("CgkIr8rGkPIJEAIQBg", 1, delegate(bool success)
				{
					Debug.Log("Achievement Beginner incremented: " + success);
				});
			}
			else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				WaitForSeconds waitForSeconds = new WaitForSeconds(2f);
				while (!AGSClient.IsServiceReady())
				{
					yield return waitForSeconds;
				}
				Debug.Log("Social platform local user authenticated: " + Social.localUser.authenticated);
				while (!Social.localUser.authenticated)
				{
					yield return waitForSeconds;
				}
				Debug.Log("Social platform local user authenticated: " + GameCircleSocial.Instance.localUser.userName + ",\t\tid: " + GameCircleSocial.Instance.localUser.id);
				Debug.Log("Trying to increment Beginner achievement...");
				AGSAchievementsClient.UpdateAchievementProgress("beginner_id", (float)newGamesStartedCount * step);
			}
			break;
		case RuntimePlatform.IPhonePlayer:
			Debug.Log("Social platform local user authenticated: " + Social.localUser.authenticated);
			while (!Social.localUser.authenticated)
			{
				yield return new WaitForSeconds(2f);
			}
			Debug.Log("Social platform local user authenticated: " + Social.localUser.userName + ",\t\tid: " + Social.localUser.id);
			Debug.Log(string.Format("Trying to report {0} achievement...", "beginner_id"));
			Social.ReportProgress("beginner_id", (float)newGamesStartedCount * step, delegate(bool success)
			{
				Debug.Log(string.Format("Achievement {0} incremented: {1}", "beginner_id", success));
			});
			break;
		}
	}

	private void Awake()
	{
		if (_instance != null)
		{
			Debug.LogWarning(GetType().Name + " already exists.");
		}
		_instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}
}

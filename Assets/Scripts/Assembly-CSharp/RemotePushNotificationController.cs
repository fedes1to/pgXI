using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Prime31;
using Rilisoft;
using UnityEngine;

public sealed class RemotePushNotificationController : MonoBehaviour
{
	private const string RemotePushRegistrationKey = "RemotePushRegistration";

	public static RemotePushNotificationController Instance;

	private bool _isResponceRuning;

	private bool _isStartUpdateRecive;

	private string UrlPushNotificationServer
	{
		get
		{
			return "https://secure.pixelgunserver.com/push_service";
		}
	}

	private IEnumerator Start()
	{
		string thisMethod = string.Format("{0}.Start()", GetType().Name);
		ScopeLogger scopeLogger = new ScopeLogger(thisMethod, Defs.IsDeveloperBuild && !Application.isEditor);
		try
		{
			if (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.GoogleLite)
			{
				yield break;
			}
			string remotePushRegistrationJson = PlayerPrefs.GetString("RemotePushRegistration", "{}");
			RemotePushRegistrationMemento remotePushRegistrationMemento = ParseRemotePushRegistrationMemento(remotePushRegistrationJson);
			if (!string.IsNullOrEmpty(remotePushRegistrationMemento.RegistrationId) && !CheckIfExpired(remotePushRegistrationMemento))
			{
				Debug.LogFormat("Remote push notifications, already registered: '{0}'", remotePushRegistrationJson);
				yield break;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (Application.isEditor)
			{
				HandleRegistered(DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture));
				yield break;
			}
			yield return new WaitForSeconds(1f);
			ScopeLogger scopeLogger2 = new ScopeLogger(thisMethod, "GoogleCloudMessaging.register()", Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				GoogleCloudMessagingManager.registrationSucceededEvent += HandleRegistered;
				GoogleCloudMessagingManager.registrationFailedEvent += HandleError;
				GoogleCloudMessaging.register("339873998127");
			}
			finally
			{
				scopeLogger2.Dispose();
			}
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}

	private void HandleError(string error)
	{
		Debug.LogError(error);
	}

	private void HandleRegistered(string registrationId)
	{
		string callee = string.Format("{0}.HandleRegistered('{1}')", GetType().Name, registrationId);
		ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild && !Application.isEditor);
		try
		{
			if (!string.IsNullOrEmpty(registrationId))
			{
				StartCoroutine(ReciveUpdateDataToServer(registrationId));
			}
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}

	public void UpdateDataOnServer()
	{
		RemotePushRegistrationMemento remotePushRegistrationMemento = LoadRemotePushRegistrationMemento();
		if (!string.IsNullOrEmpty(remotePushRegistrationMemento.RegistrationId))
		{
			StartCoroutine(ReciveUpdateDataToServer(remotePushRegistrationMemento.RegistrationId));
		}
	}

	private static RemotePushRegistrationMemento ParseRemotePushRegistrationMemento(string remotePushRegistrationJson)
	{
		//Discarded unreachable code: IL_000c, IL_0032
		try
		{
			return JsonUtility.FromJson<RemotePushRegistrationMemento>(remotePushRegistrationJson);
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
			return new RemotePushRegistrationMemento(string.Empty, DateTime.MinValue, string.Empty);
		}
	}

	private static RemotePushRegistrationMemento LoadRemotePushRegistrationMemento()
	{
		string @string = PlayerPrefs.GetString("RemotePushRegistration", "{}");
		return ParseRemotePushRegistrationMemento(@string);
	}

	private static bool IsDeviceRegistred()
	{
		string remotePushNotificationToken = GetRemotePushNotificationToken();
		return !string.IsNullOrEmpty(remotePushNotificationToken);
	}

	private static bool CheckIfExpired(RemotePushRegistrationMemento remotePushRegistrationMemento)
	{
		DateTime result;
		if (DateTime.TryParse(remotePushRegistrationMemento.RegistrationTime, out result) && DateTime.UtcNow - result < TimeSpan.FromDays(2.0))
		{
			return false;
		}
		return true;
	}

	internal static string GetRemotePushNotificationToken()
	{
		RemotePushRegistrationMemento remotePushRegistrationMemento = LoadRemotePushRegistrationMemento();
		if (CheckIfExpired(remotePushRegistrationMemento))
		{
			return string.Empty;
		}
		return remotePushRegistrationMemento.RegistrationId;
	}

	private IEnumerator ReciveUpdateDataToServer(string deviceToken)
	{
		string thisMethod = string.Format("{0}.ReciveUpdateDataToServer('{1}')", GetType().Name, deviceToken);
		ScopeLogger scopeLogger = new ScopeLogger(thisMethod, Defs.IsDeveloperBuild && !Application.isEditor);
		try
		{
			if (_isResponceRuning)
			{
				yield break;
			}
			_isResponceRuning = true;
			bool friendsControllerIsNotInitialized = FriendsController.sharedController == null;
			if (Defs.IsDeveloperBuild && FriendsController.sharedController == null)
			{
				Debug.Log("Waiting FriendsController being initialized...");
			}
			while (FriendsController.sharedController == null)
			{
				yield return null;
			}
			if (friendsControllerIsNotInitialized)
			{
				yield return null;
			}
			if (Defs.IsDeveloperBuild && FriendsController.sharedController.id == null)
			{
				Debug.Log("Waiting FriendsController.id being initialized...");
			}
			while (string.IsNullOrEmpty(FriendsController.sharedController.id))
			{
				yield return null;
			}
			_isStartUpdateRecive = true;
			WWWForm form = new WWWForm();
			string appVersion = string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion);
			string playerId = FriendsController.sharedController.id;
			string languageCode = LocalizationStore.GetCurrentLanguageCode();
			string isPayingPlayer = Storager.getInt("PayingUser", true).ToString();
			string dateLastPaying = PlayerPrefs.GetString("Last Payment Time", string.Empty);
			if (string.IsNullOrEmpty(dateLastPaying))
			{
				dateLastPaying = "None";
			}
			string timeUtcOffsetString = DateTimeOffset.Now.Offset.Hours.ToString();
			string countMoney = Storager.getInt("Coins", false).ToString();
			string countGems = Storager.getInt("GemsCurrency", false).ToString();
			string playerLevel = ExperienceController.GetCurrentLevel().ToString();
			form.AddField("app_version", appVersion);
			form.AddField("device_token", deviceToken);
			form.AddField("uniq_id", playerId);
			form.AddField("is_paying", isPayingPlayer);
			form.AddField("last_payment_date", dateLastPaying);
			form.AddField("utc_shift", timeUtcOffsetString);
			form.AddField("coins", countMoney);
			form.AddField("gems", countGems);
			form.AddField("level", playerLevel);
			form.AddField("language", languageCode);
			form.AddField("allow_invites", Defs.isEnableRemoteInviteFromFriends ? 1 : 0);
			int androidSdkLevel = 0;
			if (Application.platform == RuntimePlatform.Android)
			{
				try
				{
					androidSdkLevel = AndroidSystem.GetSdkVersion();
				}
				catch (Exception ex2)
				{
					Exception ex = ex2;
					Debug.LogException(ex);
				}
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				form.AddField("os", androidSdkLevel);
			}
			else
			{
				form.AddField("os", SystemInfo.operatingSystem);
			}
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("RemotePushNotificationController(ReciveDeviceTokenToServer): form data");
				StringBuilder dataLog = new StringBuilder();
				dataLog.AppendLine("app_version: " + appVersion);
				dataLog.AppendLine("device_token: " + deviceToken);
				dataLog.AppendLine("uniq_id: " + playerId);
				dataLog.AppendLine("is_paying: " + isPayingPlayer);
				dataLog.AppendLine("last_payment_date: " + dateLastPaying);
				dataLog.AppendLine("utc_shift: " + timeUtcOffsetString);
				dataLog.AppendLine("coins: " + countMoney);
				dataLog.AppendLine("gems: " + countGems);
				dataLog.AppendLine("level: " + playerLevel);
				dataLog.AppendLine("language: " + languageCode);
				dataLog.AppendLine("androidSdkLevel: " + androidSdkLevel);
				Debug.Log(dataLog.ToString());
			}
			Dictionary<string, string> headers = new Dictionary<string, string> { 
			{
				"Authorization",
				FriendsController.HashForPush(form.data)
			} };
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Trying to send device token to server: " + deviceToken);
			}
			WWW request = Tools.CreateWwwIf(true, UrlPushNotificationServer, form, "RemotePushNotificationController.ReciveUpdateDataToServer()", headers);
			if (request == null)
			{
				yield break;
			}
			yield return request;
			try
			{
				if (!string.IsNullOrEmpty(request.error))
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.Log("RemotePushNotificationController(ReciveDeviceTokenToServer): error = " + request.error);
					}
				}
				else
				{
					if (string.IsNullOrEmpty(request.text))
					{
						yield break;
					}
					if (Defs.IsDeveloperBuild)
					{
						Debug.Log("RemotePushNotificationController(ReciveDeviceTokenToServer): request.text = " + request.text);
					}
					if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
					{
						RemotePushRegistrationMemento remotePushRegistrationMemento = new RemotePushRegistrationMemento(deviceToken, DateTime.UtcNow, GlobalGameController.AppVersion);
						string remotePushRegistrationJson = JsonUtility.ToJson(remotePushRegistrationMemento);
						if (Defs.IsDeveloperBuild)
						{
							Debug.LogFormat("Saving remote push registration: '{0}'", remotePushRegistrationJson);
						}
						PlayerPrefs.SetString("RemotePushRegistration", remotePushRegistrationJson);
					}
				}
			}
			finally
			{
				_isResponceRuning = false;
			}
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}
}

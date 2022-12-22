using System;
using System.Collections.Generic;
using System.Globalization;
using Prime31;
using UnityEngine;

namespace Rilisoft
{
	[DisallowMultipleComponent]
	internal sealed class LocalNotificationController : MonoBehaviour
	{
		private struct LocalNotification
		{
			private readonly string _title;

			private readonly string _subtitle;

			private readonly string _ticker;

			public string Title
			{
				get
				{
					return _title ?? string.Empty;
				}
			}

			public string Subtitle
			{
				get
				{
					return _subtitle ?? string.Empty;
				}
			}

			public string Ticker
			{
				get
				{
					return _ticker ?? string.Empty;
				}
			}

			public LocalNotification(string title, string subtitle, string ticker)
			{
				_title = title ?? string.Empty;
				_subtitle = subtitle ?? string.Empty;
				_ticker = ticker ?? string.Empty;
			}

			public static LocalNotification FromLocalizationKeys(string titleKey, string subtitleKey, string tickerKey)
			{
				string title = LocalizationStore.Get(titleKey ?? string.Empty);
				string subtitle = LocalizationStore.Get(subtitleKey ?? string.Empty);
				string ticker = LocalizationStore.Get(tickerKey ?? string.Empty);
				return new LocalNotification(title, subtitle, ticker);
			}

			public static LocalNotification FromLocalizationKeys(string titleKey, string subtitleKey)
			{
				return FromLocalizationKeys(titleKey, subtitleKey, titleKey);
			}
		}

		private const int GachaNotificationId = 1000;

		private const int ReturnNotificationId = 2000;

		private const int EggNotificationId = 3000;

		private const int PetNotificationId = 4000;

		private readonly List<LocalNotification> _returnNotifications = new List<LocalNotification>(4);

		private bool EggsNotificationEnabled
		{
			get
			{
				return true;
			}
		}

		private bool GachaNotificationEnabled
		{
			get
			{
				return true;
			}
		}

		private bool ReturnNotificationEnabled
		{
			get
			{
				return true;
			}
		}

		private bool PetsNotificationEnabled
		{
			get
			{
				return true;
			}
		}

		private List<LocalNotification> ReturnNotifications
		{
			get
			{
				return _returnNotifications;
			}
		}

		public LocalNotificationController()
		{
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				_returnNotifications.Add(LocalNotification.FromLocalizationKeys("Key_2225", "Key_2239", "Key_2247"));
				_returnNotifications.Add(LocalNotification.FromLocalizationKeys("Key_2225", "Key_2240", "Key_2248"));
				_returnNotifications.Add(LocalNotification.FromLocalizationKeys("Key_2225", "Key_2239", "Key_2248"));
				_returnNotifications.Add(LocalNotification.FromLocalizationKeys("Key_2225", "Key_2240", "Key_2247"));
			}
			else
			{
				_returnNotifications.Add(LocalNotification.FromLocalizationKeys("Key_2239", "Key_2284"));
				_returnNotifications.Add(LocalNotification.FromLocalizationKeys("Key_2240", "Key_2284"));
			}
		}

		private void Awake()
		{
			string callee = string.Format("{0}.Awake()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				CancelNotifications();
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void Destroy()
		{
			string callee = string.Format("{0}.Destroy()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				ScheduleNotifications();
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void OnApplicationQuit()
		{
			string callee = string.Format("{0}.OnApplicationQuit()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				ScheduleNotifications();
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			string callee = string.Format("{0}.OnApplicationPause({1})", GetType().Name, pauseStatus);
			ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				if (pauseStatus)
				{
					ScheduleNotifications();
				}
				else
				{
					CancelNotifications();
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void ScheduleNotifications()
		{
			string text = string.Format("{0}.ScheduleNotifications()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(text, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				ScheduleEggsNotifications();
				SchedulePetsNotifications(text);
				ScheduleGachaNotifications(text);
				ScheduleReturnNotifications();
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void ScheduleEggsNotifications()
		{
			if (EggsNotificationEnabled && !(Nest.Instance == null) && !Nest.Instance.EggIsReady && Nest.Instance.TimeLeft.HasValue)
			{
				long value = Nest.Instance.TimeLeft.Value;
				DateTime dateTime = DateTime.Now.AddSeconds(value);
				DateTime dateTime2 = dateTime;
				LocalNotification localNotification = LocalNotification.FromLocalizationKeys("Key_2225", "Key_2801", "Key_2801");
				AndroidNotificationConfiguration androidNotificationConfiguration = new AndroidNotificationConfiguration(value, localNotification.Title, localNotification.Subtitle, localNotification.Ticker);
				androidNotificationConfiguration.smallIcon = "small_icon";
				androidNotificationConfiguration.largeIcon = "large_icon";
				androidNotificationConfiguration.requestCode = 3000;
				androidNotificationConfiguration.cancelsNotificationId = 3000;
				AndroidNotificationConfiguration androidNotificationConfiguration2 = androidNotificationConfiguration;
				int num = EtceteraAndroid.scheduleNotification(androidNotificationConfiguration2);
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Scheduled notification {0}: `{1}`, `{2}`, `{3}`", num, androidNotificationConfiguration2.title, androidNotificationConfiguration2.subtitle, androidNotificationConfiguration2.tickerText);
				}
			}
		}

		private void SchedulePetsNotifications(string caller)
		{
			if (!PetsNotificationEnabled || Singleton<EggsManager>.Instance == null)
			{
				return;
			}
			List<Egg> playerEggs = Singleton<EggsManager>.Instance.GetPlayerEggs();
			if (playerEggs == null || playerEggs.Count == 0)
			{
				return;
			}
			Egg egg = null;
			for (int i = 0; i != playerEggs.Count; i++)
			{
				Egg egg2 = playerEggs[i];
				if (egg2.HatchedType == EggHatchedType.Time && egg2.IncubationTimeLeft.HasValue && !egg2.CheckReady())
				{
					if (egg == null)
					{
						egg = egg2;
					}
					else if (egg2.IncubationTimeLeft.Value < egg.IncubationTimeLeft.Value)
					{
						egg = egg2;
					}
				}
			}
			if (egg == null)
			{
				return;
			}
			long value = egg.IncubationTimeLeft.Value;
			string callee = string.Format(CultureInfo.InvariantCulture, "Scheduling pet notification in {0}", value);
			ScopeLogger scopeLogger = new ScopeLogger(caller, callee, true);
			try
			{
				LocalNotification localNotification = LocalNotification.FromLocalizationKeys("Key_2225", "Key_2802", "Key_2802");
				AndroidNotificationConfiguration androidNotificationConfiguration = new AndroidNotificationConfiguration(value, localNotification.Title, localNotification.Subtitle, localNotification.Ticker);
				androidNotificationConfiguration.smallIcon = "small_icon";
				androidNotificationConfiguration.largeIcon = "large_icon";
				androidNotificationConfiguration.requestCode = 4000;
				androidNotificationConfiguration.cancelsNotificationId = 4000;
				AndroidNotificationConfiguration androidNotificationConfiguration2 = androidNotificationConfiguration;
				int num = EtceteraAndroid.scheduleNotification(androidNotificationConfiguration2);
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Scheduled notification {0}: `{1}`, `{2}`, `{3}`", num, androidNotificationConfiguration2.title, androidNotificationConfiguration2.subtitle, androidNotificationConfiguration2.tickerText);
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void ScheduleGachaNotifications(string caller)
		{
			if (!GachaNotificationEnabled || ExperienceController.GetCurrentLevel() < 2 || GiftController.Instance == null)
			{
				return;
			}
			TimeSpan freeGachaAvailableIn = GiftController.Instance.FreeGachaAvailableIn;
			string callee = string.Format(CultureInfo.InvariantCulture, "Scheduling gacha notification in {0}", freeGachaAvailableIn);
			ScopeLogger scopeLogger = new ScopeLogger(caller, callee, true);
			try
			{
				int num = Convert.ToInt32(freeGachaAvailableIn.TotalSeconds);
				if (num > 0)
				{
					LocalNotification localNotification = LocalNotification.FromLocalizationKeys("Key_2225", "Key_2800", "Key_2800");
					AndroidNotificationConfiguration androidNotificationConfiguration = new AndroidNotificationConfiguration(num, localNotification.Title, localNotification.Subtitle, localNotification.Ticker);
					androidNotificationConfiguration.smallIcon = "small_icon";
					androidNotificationConfiguration.largeIcon = "large_icon";
					androidNotificationConfiguration.requestCode = 1000;
					androidNotificationConfiguration.cancelsNotificationId = 1000;
					AndroidNotificationConfiguration androidNotificationConfiguration2 = androidNotificationConfiguration;
					int num2 = EtceteraAndroid.scheduleNotification(androidNotificationConfiguration2);
					if (Defs.IsDeveloperBuild)
					{
						Debug.LogFormat("Scheduled notification {0}: `{1}`, `{2}`, `{3}`", num2, androidNotificationConfiguration2.title, androidNotificationConfiguration2.subtitle, androidNotificationConfiguration2.tickerText);
					}
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void ScheduleReturnNotifications()
		{
			if (!ReturnNotificationEnabled)
			{
				return;
			}
			DateTime now = DateTime.Now;
			List<DateTime> list = new List<DateTime>(3);
			if (Defs.IsDeveloperBuild)
			{
				list.Add(now.AddSeconds(40.0));
				list.Add(now.AddMinutes(1.5));
				list.Add(now.AddMinutes(5.0));
				list.Add(now.AddMinutes(10.0));
			}
			list.Add(ClampTimeOfTheDay(now.AddDays(3.0)));
			list.Add(ClampTimeOfTheDay(now.AddDays(7.0)));
			int num = UnityEngine.Random.Range(0, ReturnNotifications.Count);
			int count = list.Count;
			for (int i = 0; i != count; i++)
			{
				DateTime dateTime = list[i];
				int num2 = Convert.ToInt32((dateTime - now).TotalSeconds);
				int index = (num + i) % ReturnNotifications.Count;
				LocalNotification localNotification = ReturnNotifications[index];
				AndroidNotificationConfiguration androidNotificationConfiguration = new AndroidNotificationConfiguration(num2, localNotification.Title, localNotification.Subtitle, localNotification.Ticker);
				androidNotificationConfiguration.groupKey = "Return";
				androidNotificationConfiguration.isGroupSummary = i == 0;
				androidNotificationConfiguration.smallIcon = "small_icon";
				androidNotificationConfiguration.largeIcon = "large_icon";
				androidNotificationConfiguration.requestCode = 2000 + i;
				androidNotificationConfiguration.cancelsNotificationId = 2000 + i;
				AndroidNotificationConfiguration androidNotificationConfiguration2 = androidNotificationConfiguration;
				if (Defs.IsDeveloperBuild)
				{
					androidNotificationConfiguration2.title = string.Format("({0}) {1}", num2, androidNotificationConfiguration2.title);
					androidNotificationConfiguration2.tickerText = string.Format("({0}) {1}", num2, androidNotificationConfiguration2.tickerText);
				}
				int num3 = EtceteraAndroid.scheduleNotification(androidNotificationConfiguration2);
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Scheduled notification {0}: `{1}`, `{2}`, `{3}`", num3, androidNotificationConfiguration2.title, androidNotificationConfiguration2.subtitle, androidNotificationConfiguration2.tickerText);
				}
			}
		}

		private void CancelNotifications()
		{
			string callee = string.Format("{0}.CancelNotifications()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild && !Application.isEditor);
			try
			{
				EtceteraAndroid.cancelNotification(1000);
				for (int i = 0; i != 10; i++)
				{
					EtceteraAndroid.cancelNotification(2000 + i);
				}
				EtceteraAndroid.cancelNotification(3000);
				EtceteraAndroid.cancelNotification(4000);
				EtceteraAndroid.cancelAllNotifications();
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private DateTime ClampTimeOfTheDay(DateTime rawDateTime)
		{
			TimeSpan timeSpan = new TimeSpan(16, 0, 0);
			TimeSpan timeSpan2 = TimeSpan.FromMinutes(UnityEngine.Random.Range(-30f, 30f));
			return rawDateTime.Date + timeSpan + timeSpan2;
		}

		private int SafeGetSdkLevel()
		{
			//Discarded unreachable code: IL_000b, IL_001e
			try
			{
				return AndroidSystem.GetSdkVersion();
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
				return 0;
			}
		}
	}
}

using UnityEngine;

namespace FyberPlugin
{
	internal class PluginBridgeComponent : IPluginBridge
	{
		static PluginBridgeComponent()
		{
			FyberGameObject.Init();
		}

		public void StartSDK(string json)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.fyber.mediation.MediationAdapterStarter"))
			{
				FyberSettings instance = FyberSettings.Instance;
				androidJavaClass.CallStatic("setup", instance.BundlesInfoJson(), instance.BundlesCount());
			}
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.fyber.mediation.MediationConfigProvider"))
			{
				FyberSettings instance2 = FyberSettings.Instance;
				androidJavaClass2.CallStatic("setup", instance2.BundlesConfigJson());
			}
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.FyberPlugin"))
			{
				androidJavaObject.CallStatic("setPluginParameters", "8.6.0", Application.unityVersion);
				androidJavaObject.CallStatic("start", json);
			}
		}

		public bool Cache(string action)
		{
			//Discarded unreachable code: IL_001e
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.fyber.unity.cache.CacheWrapper"))
			{
				return androidJavaClass.CallStatic<bool>(action, new object[0]);
			}
		}

		public void Request(string json)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.requesters.RequesterWrapper"))
			{
				androidJavaObject.CallStatic("request", json);
			}
		}

		public void StartAd(string json)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.ads.AdWrapper"))
			{
				androidJavaObject.CallStatic("start", json);
			}
		}

		public bool Banner(string json)
		{
			//Discarded unreachable code: IL_002c
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.ads.AdWrapper"))
			{
				return androidJavaObject.CallStatic<bool>("performAdActions", new object[1] { json });
			}
		}

		public void Report(string json)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.reporters.ReporterWrapper"))
			{
				androidJavaObject.CallStatic("report", json);
			}
		}

		public string Settings(string json)
		{
			//Discarded unreachable code: IL_002c
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.unity.settings.SettingsWrapper"))
			{
				return androidJavaObject.CallStatic<string>("perform", new object[1] { json });
			}
		}

		public void EnableLogging(bool shouldLog)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.fyber.utils.FyberLogger"))
			{
				androidJavaObject.CallStatic<bool>("enableLogging", new object[1] { shouldLog });
			}
		}

		public void GameObjectStarted()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.fyber.unity.helpers.NativeMessage"))
			{
				androidJavaClass.CallStatic("resendFailedMessages");
			}
		}

		public void ApplicationQuit()
		{
			Cache("unregisterOnVideoCacheListener");
		}
	}
}

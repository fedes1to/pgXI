using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using com.amazon.mas.cpt.ads.json;
using UnityEngine;

namespace com.amazon.mas.cpt.ads
{
	public abstract class AmazonMobileAdsImpl : MonoBehaviour, IAmazonMobileAds
	{
		private abstract class AmazonMobileAdsBase : AmazonMobileAdsImpl
		{
			private static readonly object startLock = new object();

			private static volatile bool startCalled = false;

			public AmazonMobileAdsBase()
			{
				logger = new AmazonLogger(GetType().Name);
			}

			protected void Start()
			{
				if (startCalled)
				{
					return;
				}
				lock (startLock)
				{
					if (!startCalled)
					{
						Init();
						RegisterCallback();
						RegisterEventListener();
						RegisterCrossPlatformTool();
						startCalled = true;
					}
				}
			}

			protected abstract void Init();

			protected abstract void RegisterCallback();

			protected abstract void RegisterEventListener();

			protected abstract void RegisterCrossPlatformTool();

			public override void UnityFireEvent(string jsonMessage)
			{
				FireEvent(jsonMessage);
			}

			public override void SetApplicationKey(ApplicationKey applicationKey)
			{
				Start();
				Jsonable.CheckForErrors(Json.Deserialize(SetApplicationKeyJson(applicationKey.ToJson())) as Dictionary<string, object>);
			}

			private string SetApplicationKeyJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeSetApplicationKeyJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeSetApplicationKeyJson(string jsonMessage);

			public override void RegisterApplication()
			{
				Start();
				Jsonable.CheckForErrors(Json.Deserialize(RegisterApplicationJson("{}")) as Dictionary<string, object>);
			}

			private string RegisterApplicationJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeRegisterApplicationJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeRegisterApplicationJson(string jsonMessage);

			public override void EnableLogging(ShouldEnable shouldEnable)
			{
				Start();
				Jsonable.CheckForErrors(Json.Deserialize(EnableLoggingJson(shouldEnable.ToJson())) as Dictionary<string, object>);
			}

			private string EnableLoggingJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeEnableLoggingJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeEnableLoggingJson(string jsonMessage);

			public override void EnableTesting(ShouldEnable shouldEnable)
			{
				Start();
				Jsonable.CheckForErrors(Json.Deserialize(EnableTestingJson(shouldEnable.ToJson())) as Dictionary<string, object>);
			}

			private string EnableTestingJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeEnableTestingJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeEnableTestingJson(string jsonMessage);

			public override void EnableGeoLocation(ShouldEnable shouldEnable)
			{
				Start();
				Jsonable.CheckForErrors(Json.Deserialize(EnableGeoLocationJson(shouldEnable.ToJson())) as Dictionary<string, object>);
			}

			private string EnableGeoLocationJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeEnableGeoLocationJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeEnableGeoLocationJson(string jsonMessage);

			public override Ad CreateFloatingBannerAd(Placement placement)
			{
				Start();
				return Ad.CreateFromJson(CreateFloatingBannerAdJson(placement.ToJson()));
			}

			private string CreateFloatingBannerAdJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeCreateFloatingBannerAdJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeCreateFloatingBannerAdJson(string jsonMessage);

			public override Ad CreateInterstitialAd()
			{
				Start();
				return Ad.CreateFromJson(CreateInterstitialAdJson("{}"));
			}

			private string CreateInterstitialAdJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeCreateInterstitialAdJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeCreateInterstitialAdJson(string jsonMessage);

			public override LoadingStarted LoadAndShowFloatingBannerAd(Ad ad)
			{
				Start();
				return LoadingStarted.CreateFromJson(LoadAndShowFloatingBannerAdJson(ad.ToJson()));
			}

			private string LoadAndShowFloatingBannerAdJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeLoadAndShowFloatingBannerAdJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeLoadAndShowFloatingBannerAdJson(string jsonMessage);

			public override LoadingStarted LoadInterstitialAd()
			{
				Start();
				return LoadingStarted.CreateFromJson(LoadInterstitialAdJson("{}"));
			}

			private string LoadInterstitialAdJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeLoadInterstitialAdJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeLoadInterstitialAdJson(string jsonMessage);

			public override AdShown ShowInterstitialAd()
			{
				Start();
				return AdShown.CreateFromJson(ShowInterstitialAdJson("{}"));
			}

			private string ShowInterstitialAdJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeShowInterstitialAdJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeShowInterstitialAdJson(string jsonMessage);

			public override void CloseFloatingBannerAd(Ad ad)
			{
				Start();
				Jsonable.CheckForErrors(Json.Deserialize(CloseFloatingBannerAdJson(ad.ToJson())) as Dictionary<string, object>);
			}

			private string CloseFloatingBannerAdJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeCloseFloatingBannerAdJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeCloseFloatingBannerAdJson(string jsonMessage);

			public override IsReady IsInterstitialAdReady()
			{
				Start();
				return IsReady.CreateFromJson(IsInterstitialAdReadyJson("{}"));
			}

			private string IsInterstitialAdReadyJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeIsInterstitialAdReadyJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeIsInterstitialAdReadyJson(string jsonMessage);

			public override IsEqual AreAdsEqual(AdPair adPair)
			{
				Start();
				return IsEqual.CreateFromJson(AreAdsEqualJson(adPair.ToJson()));
			}

			private string AreAdsEqualJson(string jsonMessage)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string result = NativeAreAdsEqualJson(jsonMessage);
				stopwatch.Stop();
				logger.Debug(string.Format("Successfully called native code in {0} ms", stopwatch.ElapsedMilliseconds));
				return result;
			}

			protected abstract string NativeAreAdsEqualJson(string jsonMessage);

			public override void AddAdCollapsedListener(AdCollapsedDelegate responseDelegate)
			{
				Start();
				string key = "adCollapsed";
				lock (eventLock)
				{
					if (eventListeners.ContainsKey(key))
					{
						eventListeners[key].Add(new AdCollapsedDelegator(responseDelegate));
						return;
					}
					List<IDelegator> list = new List<IDelegator>();
					list.Add(new AdCollapsedDelegator(responseDelegate));
					eventListeners.Add(key, list);
				}
			}

			public override void RemoveAdCollapsedListener(AdCollapsedDelegate responseDelegate)
			{
				Start();
				string key = "adCollapsed";
				lock (eventLock)
				{
					if (!eventListeners.ContainsKey(key))
					{
						return;
					}
					foreach (AdCollapsedDelegator item in eventListeners[key])
					{
						if (item.responseDelegate == responseDelegate)
						{
							eventListeners[key].Remove(item);
							break;
						}
					}
				}
			}

			public override void AddAdDismissedListener(AdDismissedDelegate responseDelegate)
			{
				Start();
				string key = "adDismissed";
				lock (eventLock)
				{
					if (eventListeners.ContainsKey(key))
					{
						eventListeners[key].Add(new AdDismissedDelegator(responseDelegate));
						return;
					}
					List<IDelegator> list = new List<IDelegator>();
					list.Add(new AdDismissedDelegator(responseDelegate));
					eventListeners.Add(key, list);
				}
			}

			public override void RemoveAdDismissedListener(AdDismissedDelegate responseDelegate)
			{
				Start();
				string key = "adDismissed";
				lock (eventLock)
				{
					if (!eventListeners.ContainsKey(key))
					{
						return;
					}
					foreach (AdDismissedDelegator item in eventListeners[key])
					{
						if (item.responseDelegate == responseDelegate)
						{
							eventListeners[key].Remove(item);
							break;
						}
					}
				}
			}

			public override void AddAdExpandedListener(AdExpandedDelegate responseDelegate)
			{
				Start();
				string key = "adExpanded";
				lock (eventLock)
				{
					if (eventListeners.ContainsKey(key))
					{
						eventListeners[key].Add(new AdExpandedDelegator(responseDelegate));
						return;
					}
					List<IDelegator> list = new List<IDelegator>();
					list.Add(new AdExpandedDelegator(responseDelegate));
					eventListeners.Add(key, list);
				}
			}

			public override void RemoveAdExpandedListener(AdExpandedDelegate responseDelegate)
			{
				Start();
				string key = "adExpanded";
				lock (eventLock)
				{
					if (!eventListeners.ContainsKey(key))
					{
						return;
					}
					foreach (AdExpandedDelegator item in eventListeners[key])
					{
						if (item.responseDelegate == responseDelegate)
						{
							eventListeners[key].Remove(item);
							break;
						}
					}
				}
			}

			public override void AddAdFailedToLoadListener(AdFailedToLoadDelegate responseDelegate)
			{
				Start();
				string key = "adFailedToLoad";
				lock (eventLock)
				{
					if (eventListeners.ContainsKey(key))
					{
						eventListeners[key].Add(new AdFailedToLoadDelegator(responseDelegate));
						return;
					}
					List<IDelegator> list = new List<IDelegator>();
					list.Add(new AdFailedToLoadDelegator(responseDelegate));
					eventListeners.Add(key, list);
				}
			}

			public override void RemoveAdFailedToLoadListener(AdFailedToLoadDelegate responseDelegate)
			{
				Start();
				string key = "adFailedToLoad";
				lock (eventLock)
				{
					if (!eventListeners.ContainsKey(key))
					{
						return;
					}
					foreach (AdFailedToLoadDelegator item in eventListeners[key])
					{
						if (item.responseDelegate == responseDelegate)
						{
							eventListeners[key].Remove(item);
							break;
						}
					}
				}
			}

			public override void AddAdLoadedListener(AdLoadedDelegate responseDelegate)
			{
				Start();
				string key = "adLoaded";
				lock (eventLock)
				{
					if (eventListeners.ContainsKey(key))
					{
						eventListeners[key].Add(new AdLoadedDelegator(responseDelegate));
						return;
					}
					List<IDelegator> list = new List<IDelegator>();
					list.Add(new AdLoadedDelegator(responseDelegate));
					eventListeners.Add(key, list);
				}
			}

			public override void RemoveAdLoadedListener(AdLoadedDelegate responseDelegate)
			{
				Start();
				string key = "adLoaded";
				lock (eventLock)
				{
					if (!eventListeners.ContainsKey(key))
					{
						return;
					}
					foreach (AdLoadedDelegator item in eventListeners[key])
					{
						if (item.responseDelegate == responseDelegate)
						{
							eventListeners[key].Remove(item);
							break;
						}
					}
				}
			}

			public override void AddAdResizedListener(AdResizedDelegate responseDelegate)
			{
				Start();
				string key = "adResized";
				lock (eventLock)
				{
					if (eventListeners.ContainsKey(key))
					{
						eventListeners[key].Add(new AdResizedDelegator(responseDelegate));
						return;
					}
					List<IDelegator> list = new List<IDelegator>();
					list.Add(new AdResizedDelegator(responseDelegate));
					eventListeners.Add(key, list);
				}
			}

			public override void RemoveAdResizedListener(AdResizedDelegate responseDelegate)
			{
				Start();
				string key = "adResized";
				lock (eventLock)
				{
					if (!eventListeners.ContainsKey(key))
					{
						return;
					}
					foreach (AdResizedDelegator item in eventListeners[key])
					{
						if (item.responseDelegate == responseDelegate)
						{
							eventListeners[key].Remove(item);
							break;
						}
					}
				}
			}
		}

		private class AmazonMobileAdsDefault : AmazonMobileAdsBase
		{
			protected override void Init()
			{
			}

			protected override void RegisterCallback()
			{
			}

			protected override void RegisterEventListener()
			{
			}

			protected override void RegisterCrossPlatformTool()
			{
			}

			protected override string NativeSetApplicationKeyJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeRegisterApplicationJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeEnableLoggingJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeEnableTestingJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeEnableGeoLocationJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeCreateFloatingBannerAdJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeCreateInterstitialAdJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeLoadAndShowFloatingBannerAdJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeLoadInterstitialAdJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeShowInterstitialAdJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeCloseFloatingBannerAdJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeIsInterstitialAdReadyJson(string jsonMessage)
			{
				return "{}";
			}

			protected override string NativeAreAdsEqualJson(string jsonMessage)
			{
				return "{}";
			}
		}

		private abstract class AmazonMobileAdsDelegatesBase : AmazonMobileAdsBase
		{
			private const string CrossPlatformTool = "XAMARIN";

			protected CallbackDelegate callbackDelegate;

			protected CallbackDelegate eventDelegate;

			protected override void Init()
			{
				NativeInit();
			}

			protected override void RegisterCallback()
			{
				callbackDelegate = callback;
				NativeRegisterCallback(callbackDelegate);
			}

			protected override void RegisterEventListener()
			{
				eventDelegate = FireEvent;
				NativeRegisterEventListener(eventDelegate);
			}

			protected override void RegisterCrossPlatformTool()
			{
				NativeRegisterCrossPlatformTool("XAMARIN");
			}

			public override void UnityFireEvent(string jsonMessage)
			{
				throw new NotSupportedException("UnityFireEvent is not supported");
			}

			protected abstract void NativeInit();

			protected abstract void NativeRegisterCallback(CallbackDelegate callback);

			protected abstract void NativeRegisterEventListener(CallbackDelegate callback);

			protected abstract void NativeRegisterCrossPlatformTool(string crossPlatformTool);
		}

		private class Builder
		{
			internal static readonly IAmazonMobileAds instance;

			static Builder()
			{
				instance = AmazonMobileAdsUnityAndroid.Instance;
			}
		}

		private class AmazonMobileAdsUnityAndroid : AmazonMobileAdsUnityBase
		{
			public new static AmazonMobileAdsUnityAndroid Instance
			{
				get
				{
					return AmazonMobileAdsUnityBase.getInstance<AmazonMobileAdsUnityAndroid>();
				}
			}

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeRegisterCallbackGameObject(string name);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeInit();

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeSetApplicationKeyJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeRegisterApplicationJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeEnableLoggingJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeEnableTestingJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeEnableGeoLocationJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeCreateFloatingBannerAdJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeCreateInterstitialAdJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeLoadAndShowFloatingBannerAdJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeLoadInterstitialAdJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeShowInterstitialAdJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeCloseFloatingBannerAdJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeIsInterstitialAdReadyJson(string jsonMessage);

			[DllImport("AmazonMobileAdsBridge")]
			private static extern string nativeAreAdsEqualJson(string jsonMessage);

			protected override void NativeInit()
			{
				nativeInit();
			}

			protected override void RegisterCallback()
			{
				nativeRegisterCallbackGameObject(base.gameObject.name);
			}

			protected override void RegisterEventListener()
			{
				nativeRegisterCallbackGameObject(base.gameObject.name);
			}

			protected override void NativeRegisterCrossPlatformTool(string crossPlatformTool)
			{
			}

			protected override string NativeSetApplicationKeyJson(string jsonMessage)
			{
				return nativeSetApplicationKeyJson(jsonMessage);
			}

			protected override string NativeRegisterApplicationJson(string jsonMessage)
			{
				return nativeRegisterApplicationJson(jsonMessage);
			}

			protected override string NativeEnableLoggingJson(string jsonMessage)
			{
				return nativeEnableLoggingJson(jsonMessage);
			}

			protected override string NativeEnableTestingJson(string jsonMessage)
			{
				return nativeEnableTestingJson(jsonMessage);
			}

			protected override string NativeEnableGeoLocationJson(string jsonMessage)
			{
				return nativeEnableGeoLocationJson(jsonMessage);
			}

			protected override string NativeCreateFloatingBannerAdJson(string jsonMessage)
			{
				return nativeCreateFloatingBannerAdJson(jsonMessage);
			}

			protected override string NativeCreateInterstitialAdJson(string jsonMessage)
			{
				return nativeCreateInterstitialAdJson(jsonMessage);
			}

			protected override string NativeLoadAndShowFloatingBannerAdJson(string jsonMessage)
			{
				return nativeLoadAndShowFloatingBannerAdJson(jsonMessage);
			}

			protected override string NativeLoadInterstitialAdJson(string jsonMessage)
			{
				return nativeLoadInterstitialAdJson(jsonMessage);
			}

			protected override string NativeShowInterstitialAdJson(string jsonMessage)
			{
				return nativeShowInterstitialAdJson(jsonMessage);
			}

			protected override string NativeCloseFloatingBannerAdJson(string jsonMessage)
			{
				return nativeCloseFloatingBannerAdJson(jsonMessage);
			}

			protected override string NativeIsInterstitialAdReadyJson(string jsonMessage)
			{
				return nativeIsInterstitialAdReadyJson(jsonMessage);
			}

			protected override string NativeAreAdsEqualJson(string jsonMessage)
			{
				return nativeAreAdsEqualJson(jsonMessage);
			}
		}

		private abstract class AmazonMobileAdsUnityBase : AmazonMobileAdsBase
		{
			private const string CrossPlatformTool = "UNITY";

			private static AmazonMobileAdsUnityBase instance;

			private static Type instanceType;

			private static volatile bool quit;

			private static object initLock;

			static AmazonMobileAdsUnityBase()
			{
				quit = false;
				initLock = new object();
			}

			public static T getInstance<T>() where T : AmazonMobileAdsUnityBase
			{
				//Discarded unreachable code: IL_00d5
				if (quit)
				{
					return (T)null;
				}
				if (instance != null)
				{
					return (T)instance;
				}
				lock (initLock)
				{
					Type typeFromHandle = typeof(T);
					assertTrue(instance == null || (instance != null && instanceType == typeFromHandle), "Only 1 instance of 1 subtype of AmazonMobileAdsUnityBase can exist.");
					if (instance == null)
					{
						instanceType = typeFromHandle;
						GameObject gameObject = new GameObject();
						instance = gameObject.AddComponent<T>();
						gameObject.name = typeFromHandle.ToString() + "_Singleton";
						UnityEngine.Object.DontDestroyOnLoad(gameObject);
					}
					return (T)instance;
				}
			}

			public void OnDestroy()
			{
				quit = true;
			}

			private static void assertTrue(bool statement, string errorMessage)
			{
				if (!statement)
				{
					throw new AmazonException("FATAL: An internal error occurred", new InvalidOperationException(errorMessage));
				}
			}

			protected override void Init()
			{
				NativeInit();
			}

			protected override void RegisterCrossPlatformTool()
			{
				NativeRegisterCrossPlatformTool("UNITY");
			}

			protected abstract void NativeInit();

			protected abstract void NativeRegisterCrossPlatformTool(string crossPlatformTool);
		}

		protected delegate void CallbackDelegate(string jsonMessage);

		private static AmazonLogger logger;

		private static readonly Dictionary<string, IDelegator> callbackDictionary = new Dictionary<string, IDelegator>();

		private static readonly object callbackLock = new object();

		private static readonly Dictionary<string, List<IDelegator>> eventListeners = new Dictionary<string, List<IDelegator>>();

		private static readonly object eventLock = new object();

		public static IAmazonMobileAds Instance
		{
			get
			{
				return Builder.instance;
			}
		}

		private AmazonMobileAdsImpl()
		{
		}

		public static void callback(string jsonMessage)
		{
			//Discarded unreachable code: IL_0067
			Dictionary<string, object> dictionary = null;
			try
			{
				logger.Debug("Executing callback");
				dictionary = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				string callerId = dictionary["callerId"] as string;
				Dictionary<string, object> response = dictionary["response"] as Dictionary<string, object>;
				callbackCaller(response, callerId);
			}
			catch (KeyNotFoundException inner)
			{
				logger.Debug("callerId not found in callback");
				throw new AmazonException("Internal Error: Unknown callback id", inner);
			}
			catch (AmazonException ex)
			{
				logger.Debug("Async call threw exception: " + ex.ToString());
			}
		}

		private static void callbackCaller(Dictionary<string, object> response, string callerId)
		{
			IDelegator delegator = null;
			try
			{
				Jsonable.CheckForErrors(response);
				lock (callbackLock)
				{
					delegator = callbackDictionary[callerId];
					callbackDictionary.Remove(callerId);
					delegator.ExecuteSuccess(response);
				}
			}
			catch (AmazonException e)
			{
				lock (callbackLock)
				{
					if (delegator == null)
					{
						delegator = callbackDictionary[callerId];
					}
					callbackDictionary.Remove(callerId);
					delegator.ExecuteError(e);
				}
			}
		}

		public static void FireEvent(string jsonMessage)
		{
			try
			{
				logger.Debug("eventReceived");
				Dictionary<string, object> dictionary = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				string key = dictionary["eventId"] as string;
				Dictionary<string, object> dictionary2 = null;
				if (dictionary.ContainsKey("response"))
				{
					dictionary2 = dictionary["response"] as Dictionary<string, object>;
					Jsonable.CheckForErrors(dictionary2);
				}
				lock (eventLock)
				{
					foreach (IDelegator item in eventListeners[key])
					{
						if (dictionary2 != null)
						{
							item.ExecuteSuccess(dictionary2);
						}
						else
						{
							item.ExecuteSuccess();
						}
					}
				}
			}
			catch (AmazonException ex)
			{
				logger.Debug("Event call threw exception: " + ex.ToString());
			}
		}

		public abstract void SetApplicationKey(ApplicationKey applicationKey);

		public abstract void RegisterApplication();

		public abstract void EnableLogging(ShouldEnable shouldEnable);

		public abstract void EnableTesting(ShouldEnable shouldEnable);

		public abstract void EnableGeoLocation(ShouldEnable shouldEnable);

		public abstract Ad CreateFloatingBannerAd(Placement placement);

		public abstract Ad CreateInterstitialAd();

		public abstract LoadingStarted LoadAndShowFloatingBannerAd(Ad ad);

		public abstract LoadingStarted LoadInterstitialAd();

		public abstract AdShown ShowInterstitialAd();

		public abstract void CloseFloatingBannerAd(Ad ad);

		public abstract IsReady IsInterstitialAdReady();

		public abstract IsEqual AreAdsEqual(AdPair adPair);

		public abstract void UnityFireEvent(string jsonMessage);

		public abstract void AddAdCollapsedListener(AdCollapsedDelegate responseDelegate);

		public abstract void RemoveAdCollapsedListener(AdCollapsedDelegate responseDelegate);

		public abstract void AddAdDismissedListener(AdDismissedDelegate responseDelegate);

		public abstract void RemoveAdDismissedListener(AdDismissedDelegate responseDelegate);

		public abstract void AddAdExpandedListener(AdExpandedDelegate responseDelegate);

		public abstract void RemoveAdExpandedListener(AdExpandedDelegate responseDelegate);

		public abstract void AddAdFailedToLoadListener(AdFailedToLoadDelegate responseDelegate);

		public abstract void RemoveAdFailedToLoadListener(AdFailedToLoadDelegate responseDelegate);

		public abstract void AddAdLoadedListener(AdLoadedDelegate responseDelegate);

		public abstract void RemoveAdLoadedListener(AdLoadedDelegate responseDelegate);

		public abstract void AddAdResizedListener(AdResizedDelegate responseDelegate);

		public abstract void RemoveAdResizedListener(AdResizedDelegate responseDelegate);
	}
}

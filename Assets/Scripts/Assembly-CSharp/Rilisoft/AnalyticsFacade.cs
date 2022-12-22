using System;
using System.Collections.Generic;
using System.Globalization;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class AnalyticsFacade
	{
		private static bool _initialized = false;

		private static DevToDevFacade _devToDevFacade;

		private static AppsFlyerFacade _appsFlyerFacade;

		private static FacebookFacade _facebookFacade;

		private static FlurryFacade _flurryFacade;

		private static readonly Dictionary<string, string> _recyclingFlurryParameters = new Dictionary<string, string>();

		private static readonly Lazy<string> _simpleEventFormat = new Lazy<string>(InitializeSimpleEventFormat);

		private static readonly Lazy<string> _parametrizedEventFormat = new Lazy<string>(InitializeParametrizedEventFormat);

		private static Dictionary<string, string> RecyclingFlurryParameters
		{
			get
			{
				return _recyclingFlurryParameters;
			}
		}

		public static bool DuplicateToConsoleByDefault { get; set; }

		public static bool LoggingEnabled
		{
			set
			{
				DevToDevFacade.LoggingEnabled = value;
				AppsFlyerFacade.LoggingEnabled = value;
			}
		}

		internal static DevToDevFacade DevToDevFacade
		{
			get
			{
				return _devToDevFacade;
			}
		}

		internal static AppsFlyerFacade AppsFlyerFacade
		{
			get
			{
				return _appsFlyerFacade;
			}
		}

		internal static FacebookFacade FacebookFacade
		{
			get
			{
				return _facebookFacade;
			}
		}

		internal static FlurryFacade FlurryFacade
		{
			get
			{
				return _flurryFacade;
			}
		}

		public static void Initialize()
		{
			if (_initialized)
			{
				return;
			}
			if (MiscAppsMenu.Instance == null)
			{
				Debug.LogError("MiscAppsMenu.Instance == null");
				return;
			}
			if (MiscAppsMenu.Instance.misc == null)
			{
				Debug.LogError("MiscAppsMenu.Instance.misc == null");
				return;
			}
			try
			{
				HiddenSettings misc = MiscAppsMenu.Instance.misc;
				DuplicateToConsoleByDefault = Defs.IsDeveloperBuild;
				LoggingEnabled = Defs.IsDeveloperBuild;
				string text = string.Empty;
				string text2 = string.Empty;
				if (Defs.IsDeveloperBuild || Application.isEditor)
				{
					switch (BuildSettings.BuildTargetPlatform)
					{
					case RuntimePlatform.Android:
						text = "8517441f-d330-04c5-b621-5d88e92f50e3";
						text2 = "xkjaPTLIgGQKs5MftquXrEHDW0y8OBAS";
						break;
					case RuntimePlatform.IPhonePlayer:
						text = "92002d69-82d8-067e-997d-88d1c5e804f7";
						text2 = "tQ4zhKGBvyFVObPUofaiHj7pSAcWn3Mw";
						break;
					}
				}
				else
				{
					switch (BuildSettings.BuildTargetPlatform)
					{
					case RuntimePlatform.Android:
						if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
						{
							text = "8d1482db-5181-0647-a80e-decf21db619f";
							text2 = misc.devtodevSecretGoogle;
						}
						else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
						{
							text = "531e6d54-b959-06c1-8a38-6dfdfbf309eb";
							text2 = misc.devtodevSecretAmazon;
						}
						break;
					case RuntimePlatform.IPhonePlayer:
						text = "3c77b196-8042-0dab-a5dc-92eb4377aa8e";
						text2 = misc.devtodevSecretIos;
						break;
					case RuntimePlatform.MetroPlayerX64:
						text = "cd19ad66-971e-09b2-b449-ba84d3fb52d8";
						text2 = misc.devtodevSecretWsa;
						break;
					}
				}
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Initializing DevtoDev {0}; appId: '*{1}', appSecret: '*{2}'...", DevToDevFacade.Version, text.Substring(Math.Max(text.Length - 4, 0)), text2.Substring(Math.Max(text2.Length - 4, 0)));
				}
				InitializeDevToDev(text, text2);
				string text3 = string.Empty;
				string appsFlyerAppKey = misc.appsFlyerAppKey;
				if (!Defs.IsDeveloperBuild && !Application.isEditor)
				{
					switch (BuildSettings.BuildTargetPlatform)
					{
					case RuntimePlatform.Android:
						if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
						{
							text3 = "com.pixel.gun3d";
						}
						else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
						{
							text3 = "com.PixelGun.a3D";
						}
						break;
					case RuntimePlatform.IPhonePlayer:
						text3 = "ecd1e376-8e2f-45e4-a9dc-9e938f999d20";
						break;
					}
				}
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Initializing AppsFlyer; appsFlyerAppKey: '*{0}', appsFlyerAppId: '*{1}'...", appsFlyerAppKey.Substring(Math.Max(appsFlyerAppKey.Length - 4, 0)), text3.Substring(Math.Max(text3.Length - 4, 0)));
				}
				InitializeAppsFlyer(appsFlyerAppKey, text3);
				InitializeFacebook();
				InitializeFlurry(GetFlurryApiKey(misc));
				_initialized = true;
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in AnalyticsFacade.Initialize: " + ex);
			}
		}

		private static string GetFlurryApiKey(HiddenSettings settings)
		{
			bool flag = Defs.IsDeveloperBuild || Application.isEditor;
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return (!flag) ? settings.flurryIosApiKey : settings.flurryIosDevApiKey;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				switch (Defs.AndroidEdition)
				{
				case Defs.RuntimeAndroidEdition.Amazon:
					return (!flag) ? settings.flurryAmazonApiKey : settings.flurryAmazonDevApiKey;
				case Defs.RuntimeAndroidEdition.GoogleLite:
					return (!flag) ? settings.flurryAndroidApiKey : settings.flurryAndroidDevApiKey;
				}
			}
			return string.Empty;
		}

		public static void SendCustomEvent(string eventName)
		{
			Initialize();
			SendCustomEvent(eventName, DuplicateToConsoleByDefault);
		}

		public static void SendCustomEvent(string eventName, IDictionary<string, object> eventParams)
		{
			Initialize();
			SendCustomEvent(eventName, eventParams, DuplicateToConsoleByDefault);
		}

		public static void SendCustomEvent(string eventName, bool duplicateToConsole)
		{
			Initialize();
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (_devToDevFacade != null)
			{
				_devToDevFacade.SendCustomEvent(eventName);
			}
			if (_flurryFacade != null)
			{
				_flurryFacade.LogEvent(eventName);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_simpleEventFormat.Value, eventName);
			}
		}

		public static void SendCustomEvent(string eventName, IDictionary<string, object> eventParams, bool duplicateToConsole)
		{
			Initialize();
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (eventParams == null)
			{
				throw new ArgumentNullException("eventParams");
			}
			if (_devToDevFacade != null)
			{
				_devToDevFacade.SendCustomEvent(eventName, eventParams);
			}
			if (_flurryFacade != null)
			{
				RecyclingFlurryParameters.Clear();
				foreach (KeyValuePair<string, object> eventParam in eventParams)
				{
					RecyclingFlurryParameters[eventParam.Key] = eventParam.Value.ToString();
				}
				_flurryFacade.LogEventWithParameters(eventName, RecyclingFlurryParameters);
				RecyclingFlurryParameters.Clear();
			}
			if (duplicateToConsole)
			{
				string text = Json.Serialize(eventParams);
				Debug.LogFormat(_parametrizedEventFormat.Value, eventName, text);
			}
		}

		public static void SendCustomEventToAppsFlyer(string eventName, Dictionary<string, string> eventParams)
		{
			Initialize();
			SendCustomEventToAppsFlyer(eventName, eventParams, DuplicateToConsoleByDefault);
		}

		public static void SendCustomEventToAppsFlyer(string eventName, Dictionary<string, string> eventParams, bool duplicateToConsole)
		{
			Initialize();
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (eventParams == null)
			{
				throw new ArgumentNullException("eventParams");
			}
			if (_appsFlyerFacade != null)
			{
				_appsFlyerFacade.TrackRichEvent(eventName, eventParams);
			}
			if (duplicateToConsole)
			{
				string text = Json.Serialize(eventParams);
				Debug.LogFormat(_parametrizedEventFormat.Value, eventName, text);
			}
		}

		public static void SendCustomEventToFlurry(string eventName, Dictionary<string, string> parameters)
		{
			Initialize();
			SendCustomEventToFlurry(eventName, parameters, DuplicateToConsoleByDefault);
		}

		public static void SendCustomEventToFlurry(string eventName, Dictionary<string, string> parameters, bool duplicateToConsole)
		{
			Initialize();
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			if (_flurryFacade != null)
			{
				_flurryFacade.LogEventWithParameters(eventName, parameters);
			}
			if (duplicateToConsole)
			{
				string text = Json.Serialize(parameters);
				Debug.LogFormat(_parametrizedEventFormat.Value, eventName, text);
			}
		}

		public static void SendCustomEventToFacebook(string eventName, Dictionary<string, object> parameters)
		{
			Initialize();
			SendCustomEventToFacebook(eventName, null, parameters, DuplicateToConsoleByDefault);
		}

		internal static void SendCustomEventToFacebook(string eventName, float? valueToSum, Dictionary<string, object> parameters, bool duplicateToConsole)
		{
			Initialize();
			if (_facebookFacade != null)
			{
				_facebookFacade.LogAppEvent(eventName, valueToSum, parameters);
			}
			if (duplicateToConsole)
			{
				string text = ((parameters == null) ? "{}" : Json.Serialize(parameters));
				Debug.LogFormat(_parametrizedEventFormat.Value, eventName, text);
			}
		}

		public static void Flush()
		{
			Initialize();
			if (_devToDevFacade != null)
			{
				_devToDevFacade.SendBufferedEvents();
			}
		}

		public static void Tutorial(AnalyticsConstants.TutorialState step)
		{
			Initialize();
			Tutorial(step, DuplicateToConsoleByDefault);
		}

		public static void Tutorial(AnalyticsConstants.TutorialState step, bool duplicateToConsole)
		{
			Initialize();
			if (_devToDevFacade != null)
			{
				_devToDevFacade.Tutorial(step);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_parametrizedEventFormat.Value, "TUTORIAL_BUILTIN", Json.Serialize(new Dictionary<string, object> { 
				{
					"step",
					step.ToString()
				} }));
			}
		}

		public static void LevelUp(int level)
		{
			Initialize();
			LevelUp(level, DuplicateToConsoleByDefault);
		}

		public static void LevelUp(int level, bool duplicateToConsole)
		{
			Initialize();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add("Coins", Storager.getInt("Coins", false));
			dictionary.Add("GemsCurrency", Storager.getInt("GemsCurrency", false));
			if (_devToDevFacade != null)
			{
				_devToDevFacade.LevelUp(level, dictionary);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_parametrizedEventFormat.Value, "LEVELUP_BUILTIN", Json.Serialize(new Dictionary<string, object>
				{
					{
						"level",
						level.ToString()
					},
					{ "resources", dictionary }
				}));
			}
		}

		public static void CurrencyAccrual(int amount, string currencyName, AnalyticsConstants.AccrualType accrualType = AnalyticsConstants.AccrualType.Earned)
		{
			Initialize();
			CurrencyAccrual(amount, currencyName, accrualType, DuplicateToConsoleByDefault);
		}

		public static void CurrencyAccrual(int amount, string currencyName, AnalyticsConstants.AccrualType accrualType, bool duplicateToConsole)
		{
			Initialize();
			if (_devToDevFacade != null)
			{
				_devToDevFacade.CurrencyAccrual(amount, currencyName, accrualType);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_parametrizedEventFormat.Value, "CURRENCY_ACCRUAL_BUILTIN", Json.Serialize(new Dictionary<string, object>
				{
					{
						"amount",
						amount.ToString()
					},
					{ "currencyName", currencyName },
					{
						"accrualType",
						accrualType.ToString()
					}
				}));
			}
		}

		public static void RealPayment(string paymentId, float inAppPrice, string inAppName, string currencyIsoCode, string keyOfInappAction)
		{
			Initialize();
			RealPayment(paymentId, inAppPrice, inAppName, currencyIsoCode, DuplicateToConsoleByDefault, keyOfInappAction);
		}

		public static void RealPayment(string paymentId, float inAppPrice, string inAppName, string currencyIsoCode, bool duplicateToConsole, string keyOfInappAction)
		{
			Initialize();
			if (_devToDevFacade != null)
			{
				try
				{
					_devToDevFacade.RealPayment(paymentId, inAppPrice, inAppName, currencyIsoCode);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			if (_flurryFacade != null)
			{
				RecyclingFlurryParameters.Clear();
				RecyclingFlurryParameters.Add("Total", inAppName);
				if (keyOfInappAction != null)
				{
					RecyclingFlurryParameters.Add("Special Offer", string.Format("{0}   {1}", inAppName, keyOfInappAction));
				}
				_flurryFacade.LogEventWithParameters("RealPayment", RecyclingFlurryParameters);
				RecyclingFlurryParameters.Clear();
			}
			Lazy<Dictionary<string, string>> lazy = new Lazy<Dictionary<string, string>>(delegate
			{
				string value = inAppPrice.ToString("0.00", CultureInfo.InvariantCulture);
				return new Dictionary<string, string>(4)
				{
					{ "af_revenue", value },
					{ "af_content_id", inAppName },
					{ "af_currency", currencyIsoCode },
					{ "af_receipt_id", paymentId }
				};
			});
			if (_appsFlyerFacade != null)
			{
				try
				{
					_appsFlyerFacade.TrackRichEvent("af_purchase", lazy.Value);
				}
				catch (Exception exception2)
				{
					Debug.LogException(exception2);
				}
			}
			if (_facebookFacade != null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("payment_id", paymentId);
				dictionary.Add("inapp_name", inAppName);
				Dictionary<string, object> parameters = dictionary;
				try
				{
					_facebookFacade.LogPurchase(inAppPrice, currencyIsoCode, parameters);
				}
				catch (Exception exception3)
				{
					Debug.LogException(exception3);
				}
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_parametrizedEventFormat.Value, "REAL_PAYMENT_BUILTIN", Json.Serialize(lazy.Value));
			}
		}

		public static void SendFirstTimeRealPayment(string paymentId, float inAppPrice, string inAppName, string currencyIsoCode)
		{
			Initialize();
			SendFirstTimeRealPayment(paymentId, inAppPrice, inAppName, currencyIsoCode, DuplicateToConsoleByDefault);
		}

		public static void SendFirstTimeRealPayment(string paymentId, float inAppPrice, string inAppName, string currencyIsoCode, bool duplicateToConsole)
		{
			Initialize();
			string priceAsString = inAppPrice.ToString("0.00", CultureInfo.InvariantCulture);
			Lazy<Dictionary<string, string>> lazy = new Lazy<Dictionary<string, string>>(() => new Dictionary<string, string>(4)
			{
				{ "af_revenue", priceAsString },
				{ "af_content_id", inAppName },
				{ "af_currency", currencyIsoCode },
				{ "af_receipt_id", paymentId }
			});
			if (_appsFlyerFacade != null)
			{
				_appsFlyerFacade.TrackRichEvent("first_buy", lazy.Value);
			}
			if (_facebookFacade != null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("product", inAppName);
				dictionary.Add("revenue", priceAsString);
				dictionary.Add("currency", currencyIsoCode);
				Dictionary<string, object> parameters = dictionary;
				_facebookFacade.LogAppEvent("first_buy", inAppPrice, parameters);
			}
			if (duplicateToConsole)
			{
				Debug.LogFormat(_parametrizedEventFormat.Value, "First time real payment", Json.Serialize(lazy.Value));
			}
		}

		public static void InAppPurchase(string purchaseId, string purchaseType, int purchaseAmount, int purchasePrice, string purchaseCurrency)
		{
			Initialize();
			InAppPurchase(purchaseId, purchaseType, purchaseAmount, purchasePrice, purchaseCurrency, DuplicateToConsoleByDefault);
		}

		public static void InAppPurchase(string purchaseId, string purchaseType, int purchaseAmount, int purchasePrice, string purchaseCurrency, bool duplicateToConsole)
		{
			Initialize();
			if (_devToDevFacade != null)
			{
				_devToDevFacade.InAppPurchase(purchaseId, purchaseType, purchaseAmount, purchasePrice, purchaseCurrency);
			}
		}

		private static void InitializeDevToDev(string appKey, string secretKey)
		{
			_devToDevFacade = new DevToDevFacade(appKey, secretKey);
		}

		private static void InitializeAppsFlyer(string appKey, string appId)
		{
			_appsFlyerFacade = new AppsFlyerFacade(appKey, appId);
			_appsFlyerFacade.TrackAppLaunch();
		}

		private static void InitializeFacebook()
		{
			_facebookFacade = new FacebookFacade();
		}

		private static void InitializeFlurry(string apiKey)
		{
			_flurryFacade = new FlurryFacade(apiKey, Defs.IsDeveloperBuild);
		}

		private static string InitializeSimpleEventFormat()
		{
			return (!Application.isEditor) ? "\"{0}\"" : "<color=magenta>\"{0}\"</color>";
		}

		private static string InitializeParametrizedEventFormat()
		{
			return (!Application.isEditor) ? "\"{0}\": {1}" : "<color=magenta>\"{0}\": {1}</color>";
		}
	}
}

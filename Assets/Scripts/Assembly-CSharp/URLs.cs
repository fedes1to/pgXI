using System;
using System.Globalization;
using System.IO;
using System.Text;
using Rilisoft;
using UnityEngine;

internal sealed class URLs
{
	public const string UrlForTwitterPost = "http://goo.gl/dQMf4n";

	public static string BanURL = "https://secure.pixelgunserver.com/pixelgun3d-config/getBanList.php";

	private static readonly Lazy<string> _trafficForwardingConfigUrl = new Lazy<string>(InitializeTrafficForwardingConfigUrl);

	private static readonly Lazy<string> _newPerelivConfigUrl = new Lazy<string>(GetNewPerelivConfigUrl);

	public static string Friends = "https://pixelgunserver.com/~rilisoft/action.php";

	public static string TimeOnSecure = "https://secure.pixelgunserver.com/get_time.php";

	public static string ABTestViewBankURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/abTestViewBank/test2505/currentView.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/abTestViewBank/android2505/currentView.json" : "https://secure.pixelgunserver.com/pixelgun3d-config/abTestViewBank/amazon/currentView.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/abTestViewBank/wp/currentView.json";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/abTestViewBank/ios/currentView.json";
		}
	}

	public static string ABTestBalansURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/test/abTest.php?key=0LjZB3GmACx15N7HJPYm";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/android/abTest.php?key=0LjZB3GmACx15N7HJPYm" : "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/amazon/abTest.php?key=0LjZB3GmACx15N7HJPYm";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/wp/abTest.php?key=0LjZB3GmACx15N7HJPYm";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/ios/abTest.php?key=0LjZB3GmACx15N7HJPYm";
		}
	}

	public static string ABTestBalansFolderURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/test/";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/android/" : "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/amazon/";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/wp/";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/ios/";
		}
	}

	public static string ABTestSandBoxURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/SandBox/abtestconfig_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/SandBox/abtestconfig_android.json" : "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/SandBox/abtestconfig_amazon.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/SandBox/abtestconfig_wp.json";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/SandBox/abtestconfig_ios.json";
		}
	}

	public static string ABTestQuestSystemURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/QuestSystem/abtestconfig_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/QuestSystem/abtestconfig_android.json" : "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/QuestSystem/abtestconfig_amazon.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/QuestSystem/abtestconfig_wp.json";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/QuestSystem/abtestconfig_ios.json";
		}
	}

	public static string ABTestSpecialOffersURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/SpecialOffers/abtestconfig_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/SpecialOffers/abtestconfig_android.json" : "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/SpecialOffers/abtestconfig_amazon.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/SpecialOffers/abtestconfig_wp.json";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/SpecialOffers/abtestconfig_ios.json";
		}
	}

	public static string ABTestPolygonURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestPolygon/abtestconfig_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestPolygon/abtestconfig_android.json" : "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestPolygon/abtestconfig_amazon.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestPolygon/abtestconfig_wp.json";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestPolygon/abtestconfig_ios.json";
		}
	}

	public static string ABTestAdvertURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestAdvert/abtestconfig_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestAdvert/abtestconfig_android.json" : "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestAdvert/abtestconfig_amazon.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestAdvert/abtestconfig_wp.json";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestAdvert/abtestconfig_ios.json";
		}
	}

	public static string RatingSystemConfigURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ratingConfig/rating_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/ratingConfig/rating_android.json" : "https://secure.pixelgunserver.com/pixelgun3d-config/ratingConfig/rating_amazon.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ratingConfig/rating_wp.json";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/ratingConfig/rating_ios.json";
		}
	}

	public static string ABTestBalansActualCohortNameURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/test/bCohortNameActual.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/android/bCohortNameActual.json" : "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/amazom/bCohortNameActual.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/wp/bCohortNameActual.json";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/ABTests/abTestBalans/ios/bCohortNameActual.json";
		}
	}

	public static string PromoActions
	{
		get
		{
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/PromoActions/promo_actions_android.json" : "https://secure.pixelgunserver.com/pixelgun3d-config/PromoActions/promo_actions_amazon.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/PromoActions/promo_actions_wp8.json";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/PromoActions/promo_actions.json";
		}
	}

	public static string PromoActionsTest
	{
		get
		{
			return "https://secure.pixelgunserver.com/pixelgun3d-config/PromoActions/promo_actions_test.json";
		}
	}

	public static string AmazonEvent
	{
		get
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/amazonEvent/amazon-event-test.json";
			}
			if (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/amazonEvent/amazon-event-test.json";
			}
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/amazonEvent/amazon-event-test.json";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/amazonEvent/amazon-event.json";
		}
	}

	public static string QuestConfig
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/questConfig/quest-config-test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/questConfig/quest-config-ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/questConfig/quest-config-android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/questConfig/quest-config-amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/questConfig/quest-config-wp8.json";
			}
			return string.Empty;
		}
	}

	public static string TutorialQuestConfig
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return string.Format("https://secure.pixelgunserver.com/pixelgun3d-config/tutorial-quests/tutorial-quests-{0}.json", "test");
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return string.Format("https://secure.pixelgunserver.com/pixelgun3d-config/tutorial-quests/tutorial-quests-{0}.json", "ios");
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return string.Format("https://secure.pixelgunserver.com/pixelgun3d-config/tutorial-quests/tutorial-quests-{0}.json", "android");
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return string.Format("https://secure.pixelgunserver.com/pixelgun3d-config/tutorial-quests/tutorial-quests-{0}.json", "amazon");
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return string.Format("https://secure.pixelgunserver.com/pixelgun3d-config/tutorial-quests/tutorial-quests-{0}.json", "wp8");
			}
			return string.Empty;
		}
	}

	public static string EventX3
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/event_x3_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/event_x3_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/event_x3_android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/event_x3_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/event_x3_wp8.json";
			}
			return string.Empty;
		}
	}

	public static string FilterBadWord
	{
		get
		{
			return "https://secure.pixelgunserver.com/pixelgun3d-config/FilterBadWord.json";
		}
	}

	internal static string TrafficForwardingConfigUrl
	{
		get
		{
			return _trafficForwardingConfigUrl.Value;
		}
	}

	public static string PixelbookSettings
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/PixelBookSettings/PixelBookSettings_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/PixelBookSettings/PixelBookSettings_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/PixelBookSettings/PixelBookSettings_androd.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/PixelBookSettings/PixelBookSettings_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/PixelBookSettings/PixelBookSettings_wp.json";
			}
			return string.Empty;
		}
	}

	public static string BuffSettings
	{
		get
		{
			string text = ((!StoreKitEventListener.IsPayingUser()) ? ".json" : "_paying.json");
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings/BuffSettings_test" + text;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings/BuffSettings_ios" + text;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings/BuffSettings_android" + text;
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings/BuffSettings_amazon" + text;
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings/BuffSettings_WP" + text;
			}
			return string.Empty;
		}
	}

	public static string BuffSettings1031
	{
		get
		{
			string text = ((!StoreKitEventListener.IsPayingUser()) ? ".json" : "_paying.json");
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings1031/BuffSettings_test" + text;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings1031/BuffSettings_ios" + text;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings1031/BuffSettings_android" + text;
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings1031/BuffSettings_amazon" + text;
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings1031/BuffSettings_WP" + text;
			}
			return string.Empty;
		}
	}

	public static string BuffSettings1050
	{
		get
		{
			string text = ((!StoreKitEventListener.IsPayingUser()) ? ".json" : "_paying.json");
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings1050/BuffSettings_test" + text;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings1050/BuffSettings_ios" + text;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings1050/BuffSettings_android" + text;
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings1050/BuffSettings_amazon" + text;
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/BuffSettings1050/BuffSettings_WP" + text;
			}
			return string.Empty;
		}
	}

	public static string ABTestBuffSettingsURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/abTestBuffSettings/test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/abTestBuffSettings/ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/abTestBuffSettings/android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/abTestBuffSettings/amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/abTestBuffSettings/wp.json";
			}
			return string.Empty;
		}
	}

	public static string LobbyNews
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/lobbyNews/LobbyNews_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/lobbyNews/LobbyNews_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/lobbyNews/LobbyNews_androd.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/lobbyNews/LobbyNews_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/lobbyNews/LobbyNews_wp.json";
			}
			return string.Empty;
		}
	}

	public static string NewAdvertisingConfig
	{
		get
		{
			string empty = string.Empty;
			if (Defs.IsDeveloperBuild)
			{
				return FormatNewAdvertisingConfigUrl("advert-test", empty);
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return FormatNewAdvertisingConfigUrl("advert-ios", empty);
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return FormatNewAdvertisingConfigUrl("advert-amazon", empty);
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return FormatNewAdvertisingConfigUrl("advert-android", empty);
				}
			}
			return FormatNewAdvertisingConfigUrl("fallback", empty);
		}
	}

	public static string NewPerelivConfig
	{
		get
		{
			return _newPerelivConfigUrl.Value;
		}
	}

	public static string Advert
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/advert/advert_ios_TEST.json";
				}
				if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
				{
					if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
					{
						return "https://secure.pixelgunserver.com/pixelgun3d-config/advert/advert_amazon_TEST.json";
					}
					return "https://secure.pixelgunserver.com/pixelgun3d-config/advert/advert_android_TEST.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/advert/advert_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/advert/advert_amazon.json";
				}
				return "https://secure.pixelgunserver.com/pixelgun3d-config/advert/advert_android.json";
			}
			return string.Empty;
		}
	}

	public static string BestBuy
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/bestBuy/best_buy_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/bestBuy/best_buy_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/bestBuy/best_buy_android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/bestBuy/best_buy_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/bestBuy/best_buy_wp8.json";
			}
			return string.Empty;
		}
	}

	public static string DayOfValor
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/daysOfValor/days_of_valor_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/daysOfValor/days_of_valor_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/daysOfValor/days_of_valor_android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/daysOfValor/days_of_valor_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/daysOfValor/days_of_valor_wp8.json";
			}
			return string.Empty;
		}
	}

	public static string PremiumAccount
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/premiumAccount/premium_account_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/premiumAccount/premium_account_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/premiumAccount/premium_account_android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/premiumAccount/premium_account_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/premiumAccount/premium_account_wp8.json";
			}
			return string.Empty;
		}
	}

	public static string PopularityMapUrl
	{
		get
		{
			int num = ExpController.GetOurTier() + 1;
			return "https://secure.pixelgunserver.com/mapstats/" + GlobalGameController.MultiplayerProtocolVersion + "_" + (int)(ConnectSceneNGUIController.myPlatformConnect - 1) + "_" + num + "_mapstat.json";
		}
	}

	private URLs()
	{
	}

	private static string InitializeTrafficForwardingConfigUrl()
	{
		if (Defs.IsDeveloperBuild)
		{
			return string.Format("https://secure.pixelgunserver.com/pixelgun3d-config/trafficForwarding/traffic_forwarding_{0}.json", "test");
		}
		switch (BuildSettings.BuildTargetPlatform)
		{
		case RuntimePlatform.IPhonePlayer:
			return string.Format("https://secure.pixelgunserver.com/pixelgun3d-config/trafficForwarding/traffic_forwarding_{0}.json", "ios");
		case RuntimePlatform.Android:
			switch (Defs.AndroidEdition)
			{
			case Defs.RuntimeAndroidEdition.GoogleLite:
				return string.Format("https://secure.pixelgunserver.com/pixelgun3d-config/trafficForwarding/traffic_forwarding_{0}.json", "android");
			case Defs.RuntimeAndroidEdition.Amazon:
				return string.Format("https://secure.pixelgunserver.com/pixelgun3d-config/trafficForwarding/traffic_forwarding_{0}.json", "amazon");
			default:
				return string.Empty;
			}
		case RuntimePlatform.MetroPlayerX64:
			return string.Format("https://secure.pixelgunserver.com/pixelgun3d-config/trafficForwarding/traffic_forwarding_{0}.json", "wp8");
		default:
			return string.Empty;
		}
	}

	private static string GetNewPerelivConfigUrl()
	{
		if (Defs.IsDeveloperBuild)
		{
			return FormatNewPerelivConfigUrl("pereliv-test");
		}
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			return FormatNewPerelivConfigUrl("pereliv-ios");
		}
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
		{
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				return FormatNewPerelivConfigUrl("pereliv-amazon");
			}
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
				return FormatNewPerelivConfigUrl("pereliv-android");
			}
		}
		return FormatNewPerelivConfigUrl("pereliv-fallback");
	}

	private static string FormatNewAdvertisingConfigUrl(string name, string suffix)
	{
		return string.Format(CultureInfo.InvariantCulture, "https://secure.pixelgunserver.com/pixelgun3d-config/advert-v2/{0}{1}.json", name, suffix);
	}

	private static string FormatNewPerelivConfigUrl(string name)
	{
		return "https://secure.pixelgunserver.com/pixelgun3d-config/pereliv/" + name + ".json";
	}

	internal static string Sanitize(WWW request)
	{
		//Discarded unreachable code: IL_008e
		if (request == null)
		{
			return string.Empty;
		}
		if (!request.isDone)
		{
			throw new InvalidOperationException("Request should be done.");
		}
		if (!string.IsNullOrEmpty(request.error))
		{
			return string.Empty;
		}
		UTF8Encoding uTF8Encoding = new UTF8Encoding(false);
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64)
		{
			return uTF8Encoding.GetString(request.bytes, 0, request.bytes.Length).Trim();
		}
		using (StreamReader streamReader = new StreamReader(new MemoryStream(request.bytes), uTF8Encoding))
		{
			return streamReader.ReadToEnd().Trim();
		}
	}
}

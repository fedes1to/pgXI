using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	public class AnalyticsStuff
	{
		public enum LogTrafficForwardingMode
		{
			Show,
			Press
		}

		private const string dailyGiftEventNameBase = "Daily Gift";

		private const string WeaponsSpecialOffersEvent = "Weapons Special Offers";

		private static int trainingStep = -1;

		private static bool trainingStepLoaded;

		private static string trainingProgressKey = "TrainingStepKeyAnalytics";

		public static int TrainingStep
		{
			get
			{
				LoadTrainingStep();
				return trainingStep;
			}
		}

		internal static void TrySendOnceToFacebook(string eventName, Lazy<Dictionary<string, object>> eventParams, Version excludeVersion)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (excludeVersion != null)
			{
				try
				{
					Version version = new Version(Switcher.InitialAppVersion);
					if (version <= excludeVersion)
					{
						return;
					}
				}
				catch
				{
				}
			}
			string key = "Analytics:" + eventName;
			if (!Storager.hasKey(key) || string.IsNullOrEmpty(Storager.getString(key, false)))
			{
				Storager.setString(key, "True", false);
				Dictionary<string, object> parameters = ((eventParams == null) ? null : eventParams.Value);
				AnalyticsFacade.SendCustomEventToFacebook(eventName, null, parameters, false);
			}
		}

		internal static void TrySendOnceToAppsFlyer(string eventName, Lazy<Dictionary<string, string>> eventParams, Version excludeVersion)
		{
			//Discarded unreachable code: IL_0060
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (eventParams == null)
			{
				throw new ArgumentNullException("eventParams");
			}
			if (excludeVersion == null)
			{
				throw new ArgumentNullException("excludeVersion");
			}
			try
			{
				Version version = new Version(Switcher.InitialAppVersion);
				if (version <= excludeVersion)
				{
					return;
				}
			}
			catch
			{
				return;
			}
			string key = "Analytics:" + eventName;
			if (!Storager.hasKey(key) || string.IsNullOrEmpty(Storager.getString(key, false)))
			{
				Storager.setString(key, Json.Serialize(eventParams), false);
				AnalyticsFacade.SendCustomEventToAppsFlyer(eventName, eventParams.Value);
			}
		}

		public static void TrySendOnceToAppsFlyer(string eventName)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			string key = "Analytics:" + eventName;
			if (!Storager.hasKey(key) || string.IsNullOrEmpty(Storager.getString(key, false)))
			{
				Storager.setString(key, "{}", false);
				AnalyticsFacade.SendCustomEventToAppsFlyer(eventName, new Dictionary<string, string>());
			}
		}

		internal static void SendInGameDay(int newInGameDayIndex)
		{
			Version version = new Version(Switcher.InitialAppVersion);
			if (!(version <= new Version(11, 2, 3, 0)))
			{
				Dictionary<string, object> dictionary = ParametersCache.Acquire(1);
				dictionary.Add("Day", newInGameDayIndex.ToString(CultureInfo.InvariantCulture));
				AnalyticsFacade.SendCustomEvent("InGameDay", dictionary);
				ParametersCache.Release(dictionary);
			}
		}

		public static void LogCampaign(string map, string boxName)
		{
			try
			{
				if (string.IsNullOrEmpty(map))
				{
					Debug.LogError("LogCampaign string.IsNullOrEmpty(map)");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Maps", map);
				Dictionary<string, object> dictionary2 = dictionary;
				if (boxName != null)
				{
					dictionary2.Add("Boxes", boxName);
				}
				AnalyticsFacade.SendCustomEvent("Campaign", dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogCampaign: " + ex);
			}
		}

		public static void LogMultiplayer()
		{
			try
			{
				string text = ConnectSceneNGUIController.regim.ToString();
				if (Defs.isDaterRegim)
				{
					text = "Sandbox";
				}
				if (text == null)
				{
					Debug.LogError("LogMultiplayer modeName == null");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Game Modes", text);
				dictionary.Add(text + " By Tier", ExpController.OurTierForAnyPlace() + 1);
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Multiplayer", eventParams);
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				try
				{
					int indexMap = Convert.ToInt32(PhotonNetwork.room.customProperties[ConnectSceneNGUIController.mapProperty]);
					SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(indexMap);
					dictionary2["Total"] = infoScene.NameScene;
					dictionary2[text] = infoScene.NameScene;
				}
				catch (Exception ex)
				{
					dictionary2["Error"] = ex.GetType().Name;
					Debug.LogException(ex);
				}
				AnalyticsFacade.SendCustomEvent("Maps", dictionary2);
			}
			catch (Exception ex2)
			{
				Debug.LogError("Exception in LogMultiplayer: " + ex2);
			}
		}

		public static void LogSandboxTimeGamePopularity(int timeGame, bool isStart)
		{
			try
			{
				string key = ((timeGame != 5 && timeGame != 10 && timeGame != 15) ? "Other" : ("Time " + timeGame));
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(key, (!isStart) ? "End" : "Start");
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Sandbox", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Sandbox exception: " + ex);
			}
		}

		public static void LogFirstBattlesKillRate(int battleIndex, float killRate)
		{
			try
			{
				string empty = string.Empty;
				empty = ((killRate < 0.4f) ? "<0,4" : ((killRate < 0.6f) ? "0,4 - 0,6" : ((killRate < 0.8f) ? "0,6 - 0,8" : ((killRate < 1f) ? "0,8 - 1" : ((killRate < 1.2f) ? "1 - 1,2" : ((killRate < 1.5f) ? "1,2 - 1,5" : ((killRate < 2f) ? "1,5 - 2" : ((!(killRate < 3f)) ? ">3" : "2 - 3"))))))));
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Battle " + battleIndex, empty);
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("First Battles KillRate", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogFirstBattlesKillRate: " + ex);
			}
		}

		public static void LogFirstBattlesResult(int battleIndex, bool winner)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Battle " + battleIndex, (!winner) ? "Lose" : "Win");
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("First Battles Result", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogFirstBattlesResult: " + ex);
			}
		}

		public static void LogABTest(string nameTest, string nameCohort, bool isStart = true)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(nameTest, (!isStart) ? ("Excluded " + nameCohort) : nameCohort);
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("A/B Test", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("A/B Test exception: " + ex);
			}
		}

		public static void LogArenaWavesPassed(int countWaveComplite)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Waves Passed", (countWaveComplite >= 9) ? ">=9" : countWaveComplite.ToString());
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Arena", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("ArenaFirst  exception: " + ex);
			}
		}

		public static void LogArenaFirst(bool isPause, bool isMoreOneWave)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("First", isPause ? "Quit" : ((!isMoreOneWave) ? "Fail" : "Complete"));
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Arena", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("ArenaFirst  exception: " + ex);
			}
		}

		public static void Tutorial(AnalyticsConstants.TutorialState step, int count_battle = 0)
		{
			try
			{
				LoadTrainingStep();
				if ((int)step <= trainingStep && ((step != AnalyticsConstants.TutorialState.Battle_Start && step != AnalyticsConstants.TutorialState.Battle_End) || trainingStep >= 90))
				{
					return;
				}
				int num = trainingStep;
				trainingStep = (int)step;
				string text = trainingStep + "_" + step;
				if (step == AnalyticsConstants.TutorialState.Get_Progress)
				{
					text = trainingStep.ToString() + "_" + num + "_" + step.ToString();
				}
				if (step == AnalyticsConstants.TutorialState.Battle_Start || step == AnalyticsConstants.TutorialState.Battle_End)
				{
					if (step == AnalyticsConstants.TutorialState.Battle_Start)
					{
						int @int = PlayerPrefs.GetInt("SendingStartButtle", -1);
						if (count_battle <= @int)
						{
							return;
						}
						PlayerPrefs.SetInt("SendingStartButtle", count_battle);
					}
					text = trainingStep.ToString() + "_" + count_battle + "_" + step.ToString();
				}
				FriendsController.SendToturialEvent((int)step, text);
				AnalyticsFacade.Tutorial(step);
				AnalyticsFacade.SendCustomEvent("Tutorial", new Dictionary<string, object> { { "Progress", text } });
				switch (step)
				{
				case AnalyticsConstants.TutorialState.Portal:
					AnalyticsFacade.SendCustomEventToFacebook("training_controls", null);
					break;
				case AnalyticsConstants.TutorialState.Connect_Scene:
					AnalyticsFacade.SendCustomEventToFacebook("training_armory", null);
					break;
				case AnalyticsConstants.TutorialState.Finished:
					AnalyticsFacade.SendCustomEventToFacebook("training_completed", null);
					break;
				}
				if (step > AnalyticsConstants.TutorialState.Portal)
				{
					SaveTrainingStep();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in Tutorial: " + ex);
			}
		}

		public static void SaveTrainingStep()
		{
			if (trainingStepLoaded)
			{
				Storager.setInt(trainingProgressKey, trainingStep, false);
			}
		}

		public static void LogDailyGiftPurchases(string packId)
		{
			try
			{
				if (string.IsNullOrEmpty(packId))
				{
					Debug.LogError("LogDailyGiftPurchases: string.IsNullOrEmpty(packId)");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Purchases", ReadableNameForInApp(packId));
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Daily Gift Total", eventParams);
				AnalyticsFacade.SendCustomEvent("Daily Gift" + GetPayingSuffixNo10(), eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogDailyGiftPurchases: " + ex);
			}
		}

		public static void LogDailyGift(string giftId, GiftCategoryType giftCategoryType, int count, bool isForMoneyGift)
		{
			try
			{
				if (string.IsNullOrEmpty(giftId))
				{
					Debug.LogError("LogDailyGift: string.IsNullOrEmpty(giftId)");
					return;
				}
				if (SkinsController.shopKeyFromNameSkin.ContainsKey(giftId))
				{
					giftId = "Skin";
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Chance", giftId);
				dictionary.Add("Spins", (!isForMoneyGift) ? "Free" : "Paid");
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Daily Gift Total", eventParams);
				AnalyticsFacade.SendCustomEvent("Daily Gift" + GetPayingSuffixNo10(), eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogDailyGift: " + ex);
			}
		}

		public static void LogTrafficForwarding(LogTrafficForwardingMode mode)
		{
			try
			{
				string text = ((mode != 0) ? "Button Pressed" : "Button Show");
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Conversion", text);
				dictionary.Add(text + " Levels", (!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
				dictionary.Add(text + " Tiers", ExpController.OurTierForAnyPlace() + 1);
				dictionary.Add(text + " Paying", (!StoreKitEventListener.IsPayingUser()) ? "FALSE" : "TRUE");
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Pereliv Button", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogTrafficForwarding: " + ex);
			}
		}

		public static void LogWEaponsSpecialOffers_MoneySpended(string packId)
		{
			try
			{
				if (string.IsNullOrEmpty(packId))
				{
					Debug.LogError("LogWEaponsSpecialOffers_MoneySpended: string.IsNullOrEmpty(packId)");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Money Spended", ReadableNameForInApp(packId));
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Weapons Special Offers Total", eventParams);
				AnalyticsFacade.SendCustomEvent("Weapons Special Offers" + GetPayingSuffixNo10(), eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogWEaponsSpecialOffers_MoneySpended: " + ex);
			}
		}

		public static void LogWEaponsSpecialOffers_Conversion(bool show, string weaponId = null)
		{
			try
			{
				if (!show && string.IsNullOrEmpty(weaponId))
				{
					Debug.LogError("LogWEaponsSpecialOffers_Conversion: string.IsNullOrEmpty(weaponId)");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Conversion", (!show) ? "Buy" : "Show");
				Dictionary<string, object> dictionary2 = dictionary;
				try
				{
					float num = ((!ABTestController.useBuffSystem) ? KillRateCheck.instance.GetKillRate() : BuffSystem.instance.GetKillrateByInteractions());
					string arg = ((num <= 0.5f) ? "Weak" : ((!(num <= 1.2f)) ? "Strong" : "Normal"));
					string key = string.Format("Conversion {0} Players", arg);
					if (!show)
					{
						dictionary2.Add("Currency Spended", weaponId);
						dictionary2.Add("Buy (Tier)", ExpController.OurTierForAnyPlace() + 1);
						dictionary2.Add("Buy (Level)", (!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
						dictionary2.Add(key, "Buy");
					}
					else
					{
						dictionary2.Add("Show (Tier)", ExpController.OurTierForAnyPlace() + 1);
						dictionary2.Add("Show (Level)", (!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel);
						dictionary2.Add(key, "Show");
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in LogWEaponsSpecialOffers_Conversion adding paramters: " + ex);
				}
				AnalyticsFacade.SendCustomEvent("Weapons Special Offers Total", dictionary2);
				AnalyticsFacade.SendCustomEvent("Weapons Special Offers" + GetPayingSuffixNo10(), dictionary2);
			}
			catch (Exception ex2)
			{
				Debug.LogError("Exception in LogWEaponsSpecialOffers_Conversion: " + ex2);
			}
		}

		public static void LogSpecialOffersPanel(string efficiencyPArameter, string efficiencyValue, string additionalParameter = null, string additionalValue = null)
		{
			try
			{
				if (string.IsNullOrEmpty(efficiencyPArameter) || string.IsNullOrEmpty(efficiencyValue))
				{
					Debug.LogError("LogSpecialOffersPanel:  string.IsNullOrEmpty(efficiencyPArameter) || string.IsNullOrEmpty(efficiencyValue)");
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(efficiencyPArameter, efficiencyValue);
				Dictionary<string, object> dictionary2 = dictionary;
				if (additionalParameter != null && additionalValue != null)
				{
					dictionary2.Add(additionalParameter, additionalValue);
				}
				AnalyticsFacade.SendCustomEvent("Special Offers Banner Total", dictionary2);
				AnalyticsFacade.SendCustomEvent("Special Offers Banner" + GetPayingSuffixNo10(), dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogSpecialOffersPanel: " + ex);
			}
		}

		public static string AnalyticsReadableCategoryNameFromOldCategoryName(string categoryParameterName)
		{
			categoryParameterName = NewSkinCategoryToOldSkinCategory(categoryParameterName);
			if (categoryParameterName == AnalyticsConstants.GetSalesName(ShopNGUIController.CategoryNames.LeagueHatsCategory) || categoryParameterName == AnalyticsConstants.GetSalesName(ShopNGUIController.CategoryNames.LeagueWeaponSkinsCategory))
			{
				categoryParameterName = "League";
			}
			return categoryParameterName;
		}

		private static string NewSkinCategoryToOldSkinCategory(string categoryParameterName)
		{
			if (categoryParameterName == AnalyticsConstants.GetSalesName(ShopNGUIController.CategoryNames.SkinsCategoryMale) || categoryParameterName == AnalyticsConstants.GetSalesName(ShopNGUIController.CategoryNames.SkinsCategoryFemale) || categoryParameterName == AnalyticsConstants.GetSalesName(ShopNGUIController.CategoryNames.SkinsCategorySpecial) || categoryParameterName == AnalyticsConstants.GetSalesName(ShopNGUIController.CategoryNames.SkinsCategoryPremium) || categoryParameterName == AnalyticsConstants.GetSalesName(ShopNGUIController.CategoryNames.SkinsCategoryEditor) || categoryParameterName == AnalyticsConstants.GetSalesName(ShopNGUIController.CategoryNames.LeagueSkinsCategory))
			{
				categoryParameterName = "Skins";
			}
			return categoryParameterName;
		}

		public static void LogEggDelivery(string eggId)
		{
			if (string.IsNullOrEmpty(eggId))
			{
				Debug.LogWarning("LogEggDelivery: egg id is empty.");
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>(1);
			dictionary.Add("Eggs Delivery", eggId);
			Dictionary<string, object> eventParams = dictionary;
			AnalyticsFacade.SendCustomEvent("Eggs Drop Total", eventParams);
			AnalyticsFacade.SendCustomEvent("Eggs Drop" + GetPayingSuffixNo10(), eventParams);
		}

		public static void LogHatching(string petId, Egg egg)
		{
			if (egg == null)
			{
				Debug.LogWarning("LogHatching: egg is null.");
				return;
			}
			string key = DetermineHatchingParameterName(egg);
			string value = petId ?? string.Empty;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(key, value);
			Dictionary<string, object> eventParams = dictionary;
			AnalyticsFacade.SendCustomEvent("Eggs Drop Total", eventParams);
			AnalyticsFacade.SendCustomEvent("Eggs Drop" + GetPayingSuffixNo10(), eventParams);
		}

		internal static void LogDailyVideoRewarded(int countWithinCurrentDay)
		{
			string value = countWithinCurrentDay.ToString(CultureInfo.InvariantCulture);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Rewarded", value);
			Dictionary<string, object> eventParams = dictionary;
			AnalyticsFacade.SendCustomEvent("Ads Total", eventParams);
			AnalyticsFacade.SendCustomEvent("Ads" + GetPayingSuffixNo10(), eventParams);
		}

		internal static void LogBattleInviteSent()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Conversion", "Send Invite");
			Dictionary<string, object> eventParams = dictionary;
			AnalyticsFacade.SendCustomEvent("Push Notifications", eventParams);
		}

		internal static void LogBattleInviteAccepted()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Conversion", "Accept Invite");
			Dictionary<string, object> eventParams = dictionary;
			AnalyticsFacade.SendCustomEvent("Push Notifications", eventParams);
		}

		private static string DetermineHatchingParameterName(Egg egg)
		{
			if (egg.HatchedType == EggHatchedType.Champion)
			{
				return "Drop Super Incubator";
			}
			if (egg.HatchedType == EggHatchedType.League)
			{
				return "Drop Champion";
			}
			string arg = egg.Data.Rare.ToString();
			string arg2 = egg.HatchedType.ToString();
			return string.Format("Drop {0} {1}", arg, arg2);
		}

		public static void LogBestSales(string itemId, ShopNGUIController.CategoryNames category)
		{
			try
			{
				if (string.IsNullOrEmpty(itemId))
				{
					Debug.LogError("LogBestSales: string.IsNullOrEmpty(itemId)");
					return;
				}
				string text = null;
				switch (category)
				{
				case ShopNGUIController.CategoryNames.BestWeapons:
					text = "Weapons";
					break;
				case ShopNGUIController.CategoryNames.BestWear:
					text = "Wear";
					break;
				case ShopNGUIController.CategoryNames.BestGadgets:
					text = "Gadgets";
					break;
				default:
					Debug.LogErrorFormat("LogBestSales: incorrect category: {0}", category);
					return;
				}
				string text2 = "Best Sales";
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(text, itemId);
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent(text2 + " Total", eventParams);
				AnalyticsFacade.SendCustomEvent(text2 + GetPayingSuffixNo10(), eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogBestSales: " + ex);
			}
		}

		public static void LogSales(string itemId, string categoryParameterName, bool isDaterWeapon = false)
		{
			try
			{
				if (string.IsNullOrEmpty(itemId))
				{
					Debug.LogError("LogSales: string.IsNullOrEmpty(itemId)");
					return;
				}
				if (string.IsNullOrEmpty(categoryParameterName))
				{
					Debug.LogError("LogSales: string.IsNullOrEmpty(categoryParameterName)");
					return;
				}
				categoryParameterName = AnalyticsReadableCategoryNameFromOldCategoryName(categoryParameterName);
				string salesCategory = SalesConstants.Instance.GetSalesCategory(categoryParameterName);
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(categoryParameterName, itemId);
				Dictionary<string, object> dictionary2 = dictionary;
				if (isDaterWeapon)
				{
					dictionary2.Add("Dater Weapons", itemId);
				}
				AnalyticsFacade.SendCustomEvent(salesCategory + " Total", dictionary2);
				AnalyticsFacade.SendCustomEvent(salesCategory + GetPayingSuffixNo10(), dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogSales: " + ex);
			}
		}

		public static void RateUsFake(bool rate, int stars, bool sendNegativFeedback = false)
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Efficiency", (!rate) ? "Later" : "Rate");
				if (rate)
				{
					dictionary.Add("Rating (Stars)", stars);
				}
				if (stars > 0 && stars < 4)
				{
					dictionary.Add("Negative Feedback", (!sendNegativFeedback) ? "Not sended" : "Sended");
				}
				AnalyticsFacade.SendCustomEvent("Rate Us Fake", dictionary);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in RateUsFake: " + ex);
			}
		}

		public static string ReadableNameForInApp(string purchaseId)
		{
			return (!StoreKitEventListener.inAppsReadableNames.ContainsKey(purchaseId)) ? purchaseId : StoreKitEventListener.inAppsReadableNames[purchaseId];
		}

		private static void LogGameDayCount()
		{
			//Discarded unreachable code: IL_0153
			string text = DateTime.UtcNow.ToString("yyyy-MM-dd");
			try
			{
				string @string = PlayerPrefs.GetString("Analytics.GameDayCount", string.Empty);
				Dictionary<string, object> dictionary = (Json.Deserialize(@string) as Dictionary<string, object>) ?? new Dictionary<string, object>();
				if (dictionary.Count == 0)
				{
					int num = 1;
					dictionary.Add(text, num);
					string value = Json.Serialize(dictionary);
					PlayerPrefs.SetString("Analytics.GameDayCount", value);
					AnalyticsFacade.SendCustomEventToFacebook("game_days_count", new Dictionary<string, object> { { "count", num } });
					return;
				}
				KeyValuePair<string, object> keyValuePair = dictionary.First();
				object value2 = keyValuePair.Value;
				if (text == keyValuePair.Key)
				{
					object value3 = value2;
					AnalyticsFacade.SendCustomEventToFacebook("game_days_count", new Dictionary<string, object> { { "count", value3 } });
					return;
				}
				int num2 = Convert.ToInt32(value2) + 1;
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2.Add(text, num2);
				Dictionary<string, object> obj = dictionary2;
				string value4 = Json.Serialize(obj);
				PlayerPrefs.SetString("Analytics.GameDayCount", value4);
				AnalyticsFacade.SendCustomEventToFacebook("game_days_count", new Dictionary<string, object> { { "count", num2 } });
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void LogAchievementEarned(int achievementId, int newStage)
		{
			try
			{
				if (newStage < 1 || newStage > 3)
				{
					Debug.LogError(string.Format("invalid achievement newStage : '{0}'", newStage));
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Earned", string.Format("{0}_{1}", achievementId, newStage));
				Dictionary<string, object> eventParams = dictionary;
				AnalyticsFacade.SendCustomEvent("Achievements", eventParams);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogAchievementEarned: " + ex);
			}
		}

		internal static IEnumerator WaitInitializationThenLogGameDayCountCoroutine()
		{
			yield return new WaitUntil(() => AnalyticsFacade.FacebookFacade != null);
			LogGameDayCount();
		}

		internal static void LogProgressInExperience(int levelBase1, int tierBase1)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>(2);
			dictionary.Add("Tier", tierBase1);
			dictionary.Add("Level", levelBase1);
			Dictionary<string, object> eventParams = dictionary;
			AnalyticsFacade.SendCustomEvent("Active Users Progress Total", eventParams);
			AnalyticsFacade.SendCustomEvent("Active Users Progress" + GetPayingSuffixNo10(), eventParams);
		}

		private static void LoadTrainingStep()
		{
			if (!trainingStepLoaded)
			{
				if (!Storager.hasKey(trainingProgressKey))
				{
					trainingStep = -1;
					Storager.setInt(trainingProgressKey, trainingStep, false);
				}
				else
				{
					trainingStep = Storager.getInt(trainingProgressKey, false);
				}
				trainingStepLoaded = true;
			}
		}

		public static string GetPayingSuffixNo10()
		{
			if (!StoreKitEventListener.IsPayingUser())
			{
				return " (Non Paying)";
			}
			return " (Paying)";
		}
	}
}

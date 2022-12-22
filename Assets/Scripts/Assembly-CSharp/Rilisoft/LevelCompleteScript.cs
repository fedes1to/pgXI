using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rilisoft
{
	[DisallowMultipleComponent]
	internal sealed class LevelCompleteScript : MonoBehaviour
	{
		private const string AllStarsForBoxRewardWindowIsShownNameBase = "AllStarsForBoxRewardWindowIsShown_";

		private const string AllSecretsForBoxRewardWindowIsShownNameBase = "AllSecretsForBoxRewardWindowIsShown_";

		public static LevelCompleteScript sharedScript;

		public RewardWindowBase rewardWindow;

		public RewardWindowBase rewardWindowSurvival;

		public CampaignLevelCompleteRewardSettings rewardSettings;

		public ArenaRewardWindowSettings survivalRewardWindowSettings;

		public GameObject mainInterface;

		public GameObject premium;

		public Transform RentWindowPoint;

		public GameObject mainPanel;

		public GameObject loadingPanel;

		public GameObject quitButton;

		public GameObject menuButton;

		public GameObject retryButton;

		public GameObject nextButton;

		public GameObject shopButton;

		public GameObject brightStarPrototypeSprite;

		public GameObject darkStarPrototypeSprite;

		public GameObject award1coinSprite;

		public GameObject checkboxSpritePrototype;

		public AudioClip[] starClips;

		public AudioClip shopButtonSound;

		public AudioClip awardClip;

		public GameObject survivalResults;

		public GameObject facebookButton;

		public GameObject twitterButton;

		public GameObject backgroundTexture;

		public GameObject backgroundSurvivalTexture;

		public GameObject[] statisticLabels;

		public GameObject gameOverSprite;

		public UICamera uiCamera;

		private static LevelCompleteScript _instance = null;

		private int _numOfRewardWindowsShown;

		private bool _hasAwardForMission;

		private bool _shouldBlinkCoinsIndicatorAfterRewardWindow;

		private bool _shouldBlinkGemsIndicatorAfterRewardWindow;

		private bool _shouldShowAllStarsCollectedRewardWindow;

		private bool _shouldShowAllSecretsCollectedRewardWindow;

		private IDisposable _backSubscription;

		private static Dictionary<string, string> boxNamesTwitter = new Dictionary<string, string>
		{
			{ "Real", "PIXELATED WORLD" },
			{ "minecraft", "BLOCK WORLD" },
			{ "Crossed", "CROSSED WORLDS" }
		};

		private bool _awardConferred;

		private AudioSource _awardAudioSource;

		private ExperienceController _experienceController;

		private int _oldStarCount;

		private int _starCount;

		private ShopNGUIController _shopInstance;

		private string _nextSceneName = string.Empty;

		private bool _isLastLevel;

		private int? _boxCompletionExperienceAward;

		private bool completedFirstTime;

		private bool _gameOver;

		private bool _shouldShowFacebookButton;

		private bool _shouldShowTwitterButton;

		public static bool IsInterfaceBusy
		{
			get
			{
				if (sharedScript == null)
				{
					return false;
				}
				return IsShowRewardWindow() || sharedScript.DisplayLevelResultIsRunning || sharedScript.DisplaySurvivalResultIsRunning;
			}
		}

		public bool DisplayLevelResultIsRunning { get; set; }

		public bool DisplaySurvivalResultIsRunning { get; set; }

		internal static GameResult LastGameResult { private get; set; }

		private bool AllStarsForBoxRewardWindowIsShown(string boXName)
		{
			return PlayerPrefs.GetInt("AllStarsForBoxRewardWindowIsShown_" + boXName, 0) == 1;
		}

		private bool AllSecretsForBoxRewardWindowIsShown(string boXName)
		{
			return PlayerPrefs.GetInt("AllSecretsForBoxRewardWindowIsShown_" + boXName, 0) == 1;
		}

		private void Awake()
		{
			RewardWindowBase.Shown += HandleRewardWindowShown;
			sharedScript = this;
			EventDelegate.Add(rewardWindow.hideButton.onClick, new EventDelegate(this, "HideRewardWindow"));
			EventDelegate.Add(rewardWindow.continueButton.onClick, new EventDelegate(this, "HideRewardWindow"));
			EventDelegate.Add(rewardWindow.collect.onClick, new EventDelegate(this, "HideRewardWindow"));
			EventDelegate.Add(rewardWindow.collectAndShare.onClick, new EventDelegate(this, "HideRewardWindow"));
			EventDelegate.Add(rewardWindow.continueAndShare.onClick, new EventDelegate(this, "HideRewardWindow"));
			EventDelegate.Add(rewardWindowSurvival.hideButton.onClick, new EventDelegate(this, "HideRewardWindowSurvival"));
			EventDelegate.Add(rewardWindowSurvival.continueButton.onClick, new EventDelegate(this, "HideRewardWindowSurvival"));
			EventDelegate.Add(rewardWindowSurvival.collect.onClick, new EventDelegate(this, "HideRewardWindowSurvival"));
			EventDelegate.Add(rewardWindowSurvival.continueAndShare.onClick, new EventDelegate(this, "HideRewardWindowSurvival"));
			EventDelegate.Add(rewardWindowSurvival.collectAndShare.onClick, new EventDelegate(this, "HideRewardWindowSurvival"));
			FacebookController.StoryPriority priority = FacebookController.StoryPriority.Red;
			rewardWindowSurvival.priority = priority;
			rewardWindowSurvival.twitterPriority = FacebookController.StoryPriority.ArenaLimit;
			rewardWindowSurvival.shareAction = delegate
			{
				FacebookController.PostOpenGraphStory("complete", "fight", priority, new Dictionary<string, string> { 
				{
					"map",
					Defs.SurvivalMaps[Defs.CurrentSurvMapIndex]
				} });
			};
			rewardWindowSurvival.HasReward = false;
			rewardWindowSurvival.twitterStatus = () => "I've beaten ARENA score in @PixelGun3D! Can you beat my record? #pixelgun3d #pixelgun #pg3d #mobile #fps #shooter http://goo.gl/8fzL9u";
			rewardWindowSurvival.EventTitle = "Arena Survival";
		}

		private void HandleRewardWindowShown()
		{
			_numOfRewardWindowsShown++;
		}

		private static bool IsBox1Completed()
		{
			return CurrentCampaignGame.levelSceneName.Equals("School");
		}

		private static bool IsBox2Completed()
		{
			return CurrentCampaignGame.levelSceneName.StartsWith("Gluk");
		}

		private static void PostBoxCompletedAchievement()
		{
			string text = string.Empty;
			string achievementName = string.Empty;
			bool flag = IsBox1Completed();
			bool flag2 = IsBox2Completed();
			if (flag)
			{
				text = ((BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer) ? "CgkIr8rGkPIJEAIQCA" : "block_world_id");
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					text = "Block_Survivor_id";
				}
				achievementName = "Block World Survivor";
			}
			else if (flag2)
			{
				text = "CgkIr8rGkPIJEAIQCQ";
				achievementName = "Dragon Slayer";
			}
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogWarning("Achievement Box Completed: id is null. Scene: " + CurrentCampaignGame.levelSceneName);
				return;
			}
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				AGSAchievementsClient.UpdateAchievementProgress(text, 100f);
				Debug.LogFormat("Achievement {0} completed.", achievementName);
				return;
			}
			Social.ReportProgress(text, 100.0, delegate(bool success)
			{
				Debug.LogFormat("Achievement {0} completed: {1}", achievementName, success);
			});
		}

		private IEnumerator PlayGetCoinsClip()
		{
			float delay = ((!(ExperienceController.sharedController != null)) ? 0.5f : ExperienceController.sharedController.exp_1.length);
			yield return new WaitForSeconds(delay);
			if (awardClip != null && Defs.isSoundFX)
			{
				NGUITools.PlaySound(awardClip);
			}
		}

		private static string EnglishNameForCompletedLevel(out CampaignLevel campaignLevel)
		{
			campaignLevel = LevelBox.GetLevelBySceneName(CurrentCampaignGame.levelSceneName);
			if (IsBox3Completed())
			{
				return "???";
			}
			if (campaignLevel == null || campaignLevel.localizeKeyForLevelMap == null)
			{
				return "FARM";
			}
			return (LocalizationStore.GetByDefault(campaignLevel.localizeKeyForLevelMap) ?? "FARM").Replace("\n", " ");
		}

		private void GiveAwardForCampaign()
		{
			int num = 0;
			int num2 = 0;
			if (_awardConferred || _hasAwardForMission)
			{
				num = Mathf.Min(InitializeCoinIndexBound(), _starCount);
				num *= ((!(PremiumAccountController.Instance != null)) ? 1 : PremiumAccountController.Instance.RewardCoeff);
				num2 = 0;
				if (_awardConferred)
				{
					num2 = GemsToAddForBox();
					int num3 = CoinsToAddForBox();
					num += num3;
					PostBoxCompletedAchievement();
				}
				if (num > 0)
				{
					BankController.AddCoins(num);
				}
				if (num2 > 0)
				{
					BankController.AddGems(num2);
				}
			}
			int num4 = 0;
			if (_starCount == 3 && _oldStarCount < 3 && ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel < 31)
			{
				num4 += 5;
			}
			if (_boxCompletionExperienceAward.HasValue && ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel < 31)
			{
				num4 += _boxCompletionExperienceAward.Value;
			}
			num4 *= ((!(PremiumAccountController.Instance != null)) ? 1 : PremiumAccountController.Instance.RewardCoeff);
			if (num4 != 0)
			{
				_experienceController.addExperience(num4);
			}
			if (num > 0 || num2 > 0)
			{
				StartCoroutine(PlayGetCoinsClip());
				if (num2 > 0)
				{
					_shouldBlinkGemsIndicatorAfterRewardWindow = true;
				}
				if (num > 0)
				{
					_shouldBlinkCoinsIndicatorAfterRewardWindow = true;
				}
			}
			bool flag = _awardConferred && IsBox3Completed();
			CampaignLevel campaignLevel = null;
			string arg = EnglishNameForCompletedLevel(out campaignLevel);
			string twitterStatus = string.Format("All enemies {0} {1} are defeated in @PixelGun3D! Join my adventures now! #pixelgun3d #pixelgun #pg3d http://goo.gl/8fzL9u", campaignLevel.predlog, arg);
			string eventTitle = "Level Complete";
			if (_isLastLevel)
			{
				if (IsBox1Completed())
				{
					twitterStatus = "I’ve defeated the RIDER in @PixelGun3D! Join my adventures now! #pixelgun3d #pixelgun #3d #pg3d #mobile #fps http://goo.gl/8fzL9u";
					eventTitle = "Box 1 Complete";
				}
				else if (IsBox2Completed())
				{
					twitterStatus = "I’ve defeated the DRAGON in @PixelGun3D! Join my adventures now! #pixelgun3d #pixelgun #3d #pg3d #mobile #fps http://goo.gl/8fzL9u";
					eventTitle = "Box 2 Complete";
				}
				else if (IsBox3Completed())
				{
					twitterStatus = "I’ve defeated the EVIL BUG in @PixelGun3D! Join my adventures now! #pixelgun3d #pixelgun #3d #pg3d #mobile #fps http://goo.gl/8fzL9u";
					eventTitle = "Box 3 Complete";
				}
			}
			FacebookController.StoryPriority storyPriority = ((!_isLastLevel) ? FacebookController.StoryPriority.Red : FacebookController.StoryPriority.Green);
			rewardWindow.priority = storyPriority;
			rewardWindow.twitterStatus = () => twitterStatus;
			rewardWindow.EventTitle = eventTitle;
			rewardWindow.HasReward = true;
			rewardWindow.shareAction = delegate
			{
				FacebookController.PostOpenGraphStory((!_isLastLevel) ? "complete" : "finish", (!_isLastLevel) ? "mission" : "chapter", storyPriority, (!_isLastLevel) ? new Dictionary<string, string> { 
				{
					"mission",
					CurrentCampaignGame.levelSceneName
				} } : new Dictionary<string, string> { 
				{
					"chapter",
					CurrentCampaignGame.boXName
				} });
			};
			rewardSettings.normalBackground.SetActive(PremiumAccountController.Instance == null || !PremiumAccountController.Instance.isAccountActive);
			rewardSettings.premiumBackground.SetActive(PremiumAccountController.Instance != null && PremiumAccountController.Instance.isAccountActive);
			foreach (UILabel item in rewardSettings.bossDefeatedHeader)
			{
				item.gameObject.SetActive(_awardConferred);
				if (_awardConferred)
				{
					if (IsBox1Completed())
					{
						item.text = LocalizationStore.Get("Key_1546");
					}
					else if (IsBox2Completed())
					{
						item.text = LocalizationStore.Get("Key_1547");
					}
					else if (IsBox3Completed())
					{
						item.text = LocalizationStore.Get("Key_1548");
					}
				}
			}
			foreach (UILabel boxCompletedLabel in rewardSettings.boxCompletedLabels)
			{
				boxCompletedLabel.gameObject.SetActive(_awardConferred);
				if (_awardConferred)
				{
					if (IsBox1Completed())
					{
						boxCompletedLabel.text = LocalizationStore.Get("Key_1549");
					}
					else if (IsBox2Completed())
					{
						boxCompletedLabel.text = LocalizationStore.Get("Key_1550");
					}
					else if (IsBox3Completed())
					{
						boxCompletedLabel.text = LocalizationStore.Get("Key_1551");
					}
				}
			}
			foreach (UILabel item2 in rewardSettings.missionHeader)
			{
				item2.gameObject.SetActive(!_awardConferred);
			}
			float num5 = ((!flag) ? 1f : 0.8f);
			rewardSettings.coinsReward.gameObject.SetActive(num > 0);
			rewardSettings.coinsReward.localScale = new Vector3(num5, num5, num5);
			foreach (UILabel coinsRewardLabel in rewardSettings.coinsRewardLabels)
			{
				coinsRewardLabel.text = "+" + num + " " + LocalizationStore.Get("Key_0275");
			}
			rewardSettings.coinsMultiplierContainer.SetActive(((!(PremiumAccountController.Instance != null)) ? 1 : PremiumAccountController.Instance.RewardCoeff) > 1 && !_awardConferred);
			foreach (UILabel coinsMultiplierLabel in rewardSettings.coinsMultiplierLabels)
			{
				coinsMultiplierLabel.text = "x" + ((!(PremiumAccountController.Instance != null)) ? 1 : PremiumAccountController.Instance.RewardCoeff);
			}
			rewardSettings.gemsReward.gameObject.SetActive(num2 > 0);
			rewardSettings.gemsReward.localScale = new Vector3(num5, num5, num5);
			foreach (UILabel gemsRewrdLabel in rewardSettings.gemsRewrdLabels)
			{
				gemsRewrdLabel.text = "+" + num2 + " " + LocalizationStore.Get("Key_0951");
			}
			rewardSettings.gemsMultyplierContainer.SetActive(((!(PremiumAccountController.Instance != null)) ? 1 : PremiumAccountController.Instance.RewardCoeff) > 1 && !_awardConferred);
			foreach (UILabel gemsMultiplierLabel in rewardSettings.gemsMultiplierLabels)
			{
				gemsMultiplierLabel.text = "x" + ((!(PremiumAccountController.Instance != null)) ? 1 : PremiumAccountController.Instance.RewardCoeff);
			}
			rewardSettings.experienceReward.gameObject.SetActive(num4 > 0);
			rewardSettings.experienceReward.localScale = new Vector3(num5, num5, num5);
			foreach (UILabel experienceRewardLabel in rewardSettings.experienceRewardLabels)
			{
				experienceRewardLabel.text = "+" + num4 + " " + LocalizationStore.Get("Key_0204");
			}
			rewardSettings.expMultiplier.SetActive(((!(PremiumAccountController.Instance != null)) ? 1 : PremiumAccountController.Instance.RewardCoeff) > 1);
			foreach (UILabel expMultiplierLabel in rewardSettings.expMultiplierLabels)
			{
				expMultiplierLabel.text = "x" + ((!(PremiumAccountController.Instance != null)) ? 1 : PremiumAccountController.Instance.RewardCoeff);
			}
			rewardSettings.badcode.gameObject.SetActive(flag);
			rewardSettings.badcode.localScale = new Vector3(num5, num5, num5);
			rewardSettings.grid.Reposition();
		}

		private void Start()
		{
			if (Application.isEditor)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			QuestSystem.Instance.SaveQuestProgressIfDirty();
			if (Defs.IsSurvival)
			{
				backgroundSurvivalTexture.SetActive(true);
			}
			else
			{
				backgroundTexture.SetActive(true);
			}
			ActivityIndicator.IsActiveIndicator = false;
			if (LastGameResult == GameResult.Death)
			{
				_gameOver = true;
				LastGameResult = GameResult.None;
			}
			if (!_gameOver && !Defs.IsSurvival)
			{
				StoreKitEventListener.State.PurchaseKey = "Level Completed";
				StoreKitEventListener.State.Parameters["Level"] = CurrentCampaignGame.levelSceneName + " Level Completed";
			}
			else if (_gameOver && !Defs.IsSurvival)
			{
				StoreKitEventListener.State.PurchaseKey = "Level Failed";
				StoreKitEventListener.State.Parameters["Level"] = CurrentCampaignGame.levelSceneName + " Level Failed";
			}
			else if (!_gameOver && Defs.IsSurvival)
			{
				StoreKitEventListener.State.PurchaseKey = "Player quit";
				StoreKitEventListener.State.Parameters["Waves"] = StoreKitEventListener.State.Parameters["Waves"].Substring(0, StoreKitEventListener.State.Parameters["Waves"].IndexOf(" In game")) + " Player quit";
			}
			else if (_gameOver && Defs.IsSurvival)
			{
				StoreKitEventListener.State.PurchaseKey = "Game over";
				StoreKitEventListener.State.Parameters["Waves"] = StoreKitEventListener.State.Parameters["Waves"].Substring(0, StoreKitEventListener.State.Parameters["Waves"].IndexOf(" In game")) + " Game over";
			}
			_shouldShowFacebookButton = FacebookController.FacebookSupported;
			_shouldShowTwitterButton = TwitterController.TwitterSupported;
			_experienceController = InitializeExperienceController();
			BindButtonHandler(menuButton, HandleMenuButton);
			BindButtonHandler(retryButton, HandleRetryButton);
			BindButtonHandler(nextButton, HandleNextButton);
			BindButtonHandler(shopButton, HandleShopButton);
			BindButtonHandler(quitButton, HandleQuitButton);
			BindButtonHandler(facebookButton, HandleFacebookButton);
			BindButtonHandler(twitterButton, HandleTwitterButton);
			if (!Defs.IsSurvival)
			{
				int num = -1;
				LevelBox levelBox = null;
				foreach (LevelBox campaignBox in LevelBox.campaignBoxes)
				{
					if (!campaignBox.name.Equals(CurrentCampaignGame.boXName))
					{
						continue;
					}
					levelBox = campaignBox;
					for (int i = 0; i != campaignBox.levels.Count; i++)
					{
						CampaignLevel campaignLevel = campaignBox.levels[i];
						if (campaignLevel.sceneName.Equals(CurrentCampaignGame.levelSceneName))
						{
							num = i;
							break;
						}
					}
					break;
				}
				if (levelBox != null)
				{
					_isLastLevel = num >= levelBox.levels.Count - 1;
					_nextSceneName = levelBox.levels[(!_isLastLevel) ? (num + 1) : num].sceneName;
				}
				else
				{
					Debug.LogError("Current box not found in the list of boxes!");
					_isLastLevel = true;
					_nextSceneName = SceneManager.GetActiveScene().name;
				}
				_oldStarCount = 0;
				_starCount = InitializeStarCount();
				if (!_gameOver)
				{
					if (WeaponManager.sharedManager != null)
					{
						WeaponManager.sharedManager.DecreaseTryGunsMatchesCount();
					}
					Dictionary<string, int> dictionary = CampaignProgress.boxesLevelsAndStars[CurrentCampaignGame.boXName];
					if (!dictionary.ContainsKey(CurrentCampaignGame.levelSceneName))
					{
						completedFirstTime = true;
						if (_isLastLevel)
						{
							_boxCompletionExperienceAward = levelBox.CompletionExperienceAward;
						}
						dictionary.Add(CurrentCampaignGame.levelSceneName, _starCount);
						CampaignProgress.SaveCampaignProgress();
						try
						{
							string text = null;
							if (_isLastLevel)
							{
								text = (IsBox1Completed() ? "Box 1" : ((!IsBox2Completed()) ? "Box 3" : "Box 2"));
							}
							CampaignLevel campaignLevel2;
							string text2 = EnglishNameForCompletedLevel(out campaignLevel2);
							AnalyticsStuff.LogCampaign(text2, text);
							AnalyticsFacade.SendCustomEventToFacebook("campaign_level_reached", new Dictionary<string, object>
							{
								{ "level", text2 },
								{
									"box_level",
									string.Format("{0}: {1}", text, text2)
								}
							});
						}
						catch (Exception ex)
						{
							Debug.LogError("Exception in LogCampaign(LevelCompleteScript): " + ex);
						}
					}
					else
					{
						_oldStarCount = dictionary[CurrentCampaignGame.levelSceneName];
						dictionary[CurrentCampaignGame.levelSceneName] = Math.Max(_oldStarCount, _starCount);
						CampaignProgress.SaveCampaignProgress();
					}
					CampaignProgress.OpenNewBoxIfPossible();
					var rememberedAmmo = WeaponManager.sharedManager.allAvailablePlayerWeapons.OfType<Weapon>().ToDictionary((Weapon w) => w.weaponPrefab.nameNoClone(), (Weapon w) => new
					{
						AmmoInClip = w.currentAmmoInClip,
						AmmoInBackpack = w.currentAmmoInBackpack
					});
					Action returnRememberedAmmp = delegate
					{
						IEnumerable<Weapon> source2 = WeaponManager.sharedManager.allAvailablePlayerWeapons.OfType<Weapon>();
						foreach (var kvp in rememberedAmmo)
						{
							Weapon weapon = source2.FirstOrDefault((Weapon w) => w.weaponPrefab.nameNoClone() == kvp.Key);
							if (weapon != null)
							{
								weapon.currentAmmoInClip = kvp.Value.AmmoInClip;
								weapon.currentAmmoInBackpack = kvp.Value.AmmoInBackpack;
							}
						}
					};
					if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
					{
						if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
						{
							ProgressSynchronizer.Instance.AuthenticateAndSynchronize(delegate
							{
								WeaponManager.sharedManager.Reset(WeaponManager.sharedManager.CurrentFilterMap);
								returnRememberedAmmp();
							}, true);
							CampaignProgressSynchronizer.Instance.Sync();
						}
						else if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
						{
							ProgressSynchronizer.Instance.SynchronizeAmazonProgress();
							WeaponManager.sharedManager.Reset(0);
							returnRememberedAmmp();
						}
					}
					if (Application.platform == RuntimePlatform.IPhonePlayer)
					{
						ProgressSynchronizer.Instance.SynchronizeIosProgress();
						WeaponManager.sharedManager.Reset(0);
						CampaignProgressSynchronizer.Instance.Sync();
						returnRememberedAmmp();
						AchievementSynchronizer.Instance.Sync();
					}
					try
					{
						if (!AllStarsForBoxRewardWindowIsShown(CurrentCampaignGame.boXName))
						{
							int num2 = dictionary.Values.ToList().Sum();
							_shouldShowAllStarsCollectedRewardWindow = num2 == LevelBox.campaignBoxes.Find((LevelBox lb) => lb.name == CurrentCampaignGame.boXName).levels.Count * 3;
						}
						if (!AllSecretsForBoxRewardWindowIsShown(CurrentCampaignGame.boXName))
						{
							List<string> source = LevelBox.campaignBoxes.Find((LevelBox lb) => lb.name == CurrentCampaignGame.boXName).levels.Select((CampaignLevel level) => level.sceneName).ToList();
							HashSet<string> levelsWhereGotCoins = new HashSet<string>(CoinBonus.GetLevelsWhereGotBonus(VirtualCurrencyBonusType.Coin));
							HashSet<string> levelsWhereGotGems = new HashSet<string>(CoinBonus.GetLevelsWhereGotBonus(VirtualCurrencyBonusType.Gem));
							_shouldShowAllSecretsCollectedRewardWindow = source.All((string l) => levelsWhereGotCoins.Contains(l) && levelsWhereGotGems.Contains(l));
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
					_hasAwardForMission = _starCount > _oldStarCount && InitializeCoinIndexBound() > _oldStarCount;
				}
				_awardConferred = InitializeAwardConferred();
			}
			survivalResults.SetActive(false);
			quitButton.SetActive(false);
			if (!_gameOver)
			{
				bool active = PremiumAccountController.Instance != null && PremiumAccountController.Instance.isAccountActive;
				premium.SetActive(active);
				if (!Defs.IsSurvival)
				{
					award1coinSprite.SetActive(true);
				}
				GameObject[] array = statisticLabels;
				foreach (GameObject gameObject in array)
				{
					gameObject.SetActive(Defs.IsSurvival);
				}
				if (_starCount > _oldStarCount)
				{
					CoinsMessage.FireCoinsAddedEvent(false, 2);
				}
			}
			else
			{
				award1coinSprite.SetActive(false);
				nextButton.SetActive(false);
				checkboxSpritePrototype.SetActive(false);
				if (!Defs.IsSurvival && gameOverSprite != null)
				{
					gameOverSprite.SetActive(true);
				}
				GameObject[] array2 = statisticLabels;
				foreach (GameObject gameObject2 in array2)
				{
					gameObject2.SetActive(Defs.IsSurvival);
				}
				if (!Defs.IsSurvival)
				{
					float x = (retryButton.transform.position.x - menuButton.transform.position.x) / 2f;
					Vector3 vector = new Vector3(x, 0f, 0f);
					menuButton.transform.position = retryButton.transform.position - vector;
					retryButton.transform.position += vector;
				}
				menuButton.SetActive(!Defs.IsSurvival);
				if (!Defs.IsSurvival)
				{
					StartCoroutine(TryToShowExpiredBanner());
				}
			}
			if (Defs.IsSurvival)
			{
				if (WeaponManager.sharedManager != null && WavesSurvivedStat.SurvivedWaveCount > 0)
				{
					WeaponManager.sharedManager.DecreaseTryGunsMatchesCount();
				}
				WeaponManager sharedManager = WeaponManager.sharedManager;
				sharedManager.Reset(0);
			}
			_instance = this;
			if ((!Defs.IsSurvival && (_awardConferred || _hasAwardForMission)) || (_starCount == 3 && _oldStarCount < 3 && !_gameOver && ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel < 31))
			{
				mainInterface.SetActive(false);
				rewardWindow.gameObject.SetActive(true);
				GiveAwardForCampaign();
			}
			else if (!Defs.IsSurvival)
			{
				mainInterface.SetActive(true);
				rewardWindow.gameObject.SetActive(false);
				if (!_gameOver && brightStarPrototypeSprite != null && darkStarPrototypeSprite != null)
				{
					StartCoroutine(DisplayLevelResult());
				}
			}
			else
			{
				if (!Defs.IsSurvival)
				{
					return;
				}
				int num3 = CalculateExperienceAward(GlobalGameController.Score);
				if (num3 > 0)
				{
					_experienceController.addExperience(num3 * ((!(PremiumAccountController.Instance != null)) ? 1 : PremiumAccountController.Instance.RewardCoeff));
				}
				if (GlobalGameController.HasSurvivalRecord)
				{
					GlobalGameController.HasSurvivalRecord = false;
					if ((FacebookController.FacebookSupported || TwitterController.TwitterSupported) && !Device.isPixelGunLow)
					{
						mainInterface.SetActive(false);
						rewardWindowSurvival.gameObject.SetActive(true);
						{
							foreach (UILabel scoreLabel in survivalRewardWindowSettings.scoreLabels)
							{
								scoreLabel.text = string.Format(LocalizationStore.Get("Key_1553"), GlobalGameController.Score);
							}
							return;
						}
					}
					DisplaySurvivalResult();
				}
				else
				{
					DisplaySurvivalResult();
				}
			}
		}

		public void HideRewardWindow()
		{
			ButtonClickSound.TryPlayClick();
			mainInterface.SetActive(true);
			rewardWindow.gameObject.SetActive(false);
			if (!Defs.IsSurvival && brightStarPrototypeSprite != null && darkStarPrototypeSprite != null)
			{
				StartCoroutine(DisplayLevelResult());
			}
		}

		public void HideRewardWindowSurvival()
		{
			ButtonClickSound.TryPlayClick();
			mainInterface.SetActive(true);
			rewardWindowSurvival.gameObject.SetActive(false);
			DisplaySurvivalResult();
		}

		private void OnDestroy()
		{
			_instance = null;
			if (_experienceController != null)
			{
				_experienceController.isShowRanks = false;
			}
			PlayerPrefs.Save();
			RewardWindowBase.Shown -= HandleRewardWindowShown;
			sharedScript = null;
		}

		public static bool IsShowRewardWindow()
		{
			if (sharedScript == null)
			{
				return false;
			}
			bool flag = sharedScript.rewardWindowSurvival != null && sharedScript.rewardWindowSurvival.gameObject != null && sharedScript.rewardWindowSurvival.gameObject.activeInHierarchy;
			bool flag2 = sharedScript.rewardWindow != null && sharedScript.rewardWindow.gameObject != null && sharedScript.rewardWindow.gameObject.activeInHierarchy;
			return flag || flag2;
		}

		private void Update()
		{
			if (_experienceController != null && BankController.Instance != null && !BankController.Instance.InterfaceEnabled && !ShopNGUIController.GuiActive)
			{
				_experienceController.isShowRanks = RentWindowPoint.childCount == 0 && !loadingPanel.activeSelf && (!(sharedScript != null) || !IsShowRewardWindow());
			}
			bool active = FacebookController.FacebookSupported && _shouldShowFacebookButton && FacebookController.FacebookPost_Old_Supported && FB.IsLoggedIn;
			facebookButton.SetActive(active);
			twitterButton.SetActive(TwitterController.TwitterSupported && _shouldShowTwitterButton && TwitterController.TwitterSupported_OldPosts && TwitterController.IsLoggedIn);
		}

		private void OnEnable()
		{
			if (_backSubscription != null)
			{
				_backSubscription.Dispose();
			}
			_backSubscription = BackSystem.Instance.Register(delegate
			{
				HandleMenuButton(this, EventArgs.Empty);
			}, "Level Complete");
		}

		private void OnDisable()
		{
			if (_backSubscription != null)
			{
				_backSubscription.Dispose();
				_backSubscription = null;
			}
		}

		private static void BindButtonHandler(GameObject button, EventHandler handler)
		{
			if (button != null)
			{
				ButtonHandler component = button.GetComponent<ButtonHandler>();
				if (component != null)
				{
					component.Clicked += handler;
				}
			}
		}

		private static int CalculateExperienceAward(int score)
		{
			if (ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel == 31)
			{
				return 0;
			}
			int num = ((!Application.isEditor) ? 1 : 100);
			if (score < 15000 / num)
			{
				return 0;
			}
			if (score < 50000 / num)
			{
				return 10;
			}
			if (score < 100000 / num)
			{
				return 35;
			}
			if (score < 150000 / num)
			{
				return 50;
			}
			return 75;
		}

		private void DisplaySurvivalResult()
		{
			try
			{
				DisplaySurvivalResultIsRunning = true;
				menuButton.SetActive(false);
				retryButton.SetActive(false);
				nextButton.SetActive(false);
				shopButton.SetActive(false);
				quitButton.SetActive(false);
				survivalResults.SetActive(true);
				retryButton.SetActive(true);
				shopButton.SetActive(true);
				quitButton.SetActive(true);
				StartCoroutine(TryToShowExpiredBanner());
			}
			finally
			{
				DisplaySurvivalResultIsRunning = false;
			}
		}

		private static int InitializeCoinIndexBound()
		{
			int diffGame = Defs.diffGame;
			return diffGame + 1;
		}

		private static bool IsBox3Completed()
		{
			return CurrentCampaignGame.levelSceneName.Equals("Code_campaign3");
		}

		private int GemsToAddForBox()
		{
			int result = 0;
			if (IsBox1Completed())
			{
				result = LevelBox.campaignBoxes[0].gems;
			}
			else if (IsBox2Completed())
			{
				result = LevelBox.campaignBoxes[1].gems;
			}
			else if (IsBox3Completed())
			{
				result = LevelBox.campaignBoxes[2].gems;
			}
			return result;
		}

		private int CoinsToAddForBox()
		{
			int result = 0;
			if (IsBox1Completed())
			{
				result = LevelBox.campaignBoxes[0].coins;
			}
			else if (IsBox2Completed())
			{
				result = LevelBox.campaignBoxes[1].coins;
			}
			else if (IsBox3Completed())
			{
				result = LevelBox.campaignBoxes[2].coins;
			}
			return result;
		}

		private IEnumerator DisplayLevelResult()
		{
			try
			{
				DisplayLevelResultIsRunning = true;
				menuButton.SetActive(false);
				retryButton.SetActive(false);
				nextButton.SetActive(false);
				shopButton.SetActive(false);
				_shouldShowFacebookButton = false;
				_shouldShowTwitterButton = false;
				int coinIndexBound = InitializeCoinIndexBound();
				List<GameObject> stars = new List<GameObject>(3);
				for (int i = 0; i != 3; i++)
				{
					float x = -140f + (float)i * 140f;
					GameObject star = UnityEngine.Object.Instantiate(darkStarPrototypeSprite);
					star.transform.parent = darkStarPrototypeSprite.transform.parent;
					star.transform.localPosition = new Vector3(x, darkStarPrototypeSprite.transform.localPosition.y, 0f);
					star.transform.localScale = darkStarPrototypeSprite.transform.localScale;
					star.SetActive(true);
					stars.Add(star);
				}
				int currentStarIndex = 0;
				for (int checkboxIndex = 0; checkboxIndex < 3; checkboxIndex++)
				{
					if ((checkboxIndex != 1 || CurrentCampaignGame.completeInTime) && (checkboxIndex != 2 || CurrentCampaignGame.withoutHits))
					{
						yield return new WaitForSeconds(0.7f);
						GameObject star2 = UnityEngine.Object.Instantiate(brightStarPrototypeSprite);
						star2.transform.parent = brightStarPrototypeSprite.transform.parent;
						star2.transform.localPosition = stars[currentStarIndex].transform.localPosition;
						star2.transform.localScale = stars[currentStarIndex].transform.localScale;
						star2.SetActive(true);
						UnityEngine.Object.Destroy(stars[currentStarIndex]);
						GameObject checkbox = UnityEngine.Object.Instantiate(checkboxSpritePrototype);
						checkbox.transform.parent = checkboxSpritePrototype.transform.parent;
						checkbox.transform.localPosition = new Vector3(checkboxSpritePrototype.transform.localPosition.x, checkboxSpritePrototype.transform.localPosition.y - 45f * (float)checkboxIndex, checkboxSpritePrototype.transform.localPosition.z);
						checkbox.transform.localScale = checkboxSpritePrototype.transform.localScale;
						checkbox.SetActive(true);
						if (starClips != null && currentStarIndex < starClips.Length && starClips[currentStarIndex] != null && Defs.isSoundFX)
						{
							NGUITools.PlaySound(starClips[currentStarIndex]);
						}
						currentStarIndex++;
					}
				}
				UnityEngine.Object.Destroy(brightStarPrototypeSprite);
				UnityEngine.Object.Destroy(darkStarPrototypeSprite);
				menuButton.SetActive(true);
				retryButton.SetActive(true);
				nextButton.SetActive(true);
				shopButton.SetActive(true);
				_shouldShowFacebookButton = FacebookController.FacebookSupported;
				_shouldShowTwitterButton = TwitterController.TwitterSupported;
				if (_shouldBlinkCoinsIndicatorAfterRewardWindow)
				{
					CoinsMessage.FireCoinsAddedEvent(false, 2);
				}
				if (_shouldBlinkGemsIndicatorAfterRewardWindow)
				{
					CoinsMessage.FireCoinsAddedEvent(true);
				}
				yield return new WaitForSeconds(0.7f);
				StartCoroutine(TryToShowExpiredBanner());
			}
			finally
			{
				DisplayLevelResultIsRunning = false;
			}
		}

		private static string BoxNameForTwitter()
		{
			return boxNamesTwitter[CurrentCampaignGame.boXName];
		}

		private IEnumerator TryToShowExpiredBanner()
		{
			while (FriendsController.sharedController == null || TempItemsController.sharedController == null)
			{
				yield return null;
			}
			while (true)
			{
				yield return StartCoroutine(FriendsController.sharedController.MyWaitForSeconds(1f));
				try
				{
					if (ShopNGUIController.GuiActive || (BankController.Instance != null && BankController.Instance.InterfaceEnabled) || (ExpController.Instance != null && ExpController.Instance.WaitingForLevelUpView) || (ExpController.Instance != null && ExpController.Instance.IsLevelUpShown) || loadingPanel.activeInHierarchy || RentWindowPoint.childCount != 0)
					{
						continue;
					}
					if (_shouldShowAllStarsCollectedRewardWindow && _numOfRewardWindowsShown < 2)
					{
						_shouldShowAllStarsCollectedRewardWindow = false;
						PlayerPrefs.SetInt("AllStarsForBoxRewardWindowIsShown_" + CurrentCampaignGame.boXName, 1);
						if ((!FacebookController.FacebookSupported && !TwitterController.TwitterSupported) || Device.isPixelGunLow)
						{
							continue;
						}
						GameObject window = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("NguiWindows/AllStarsNGUI"));
						RewardWindowBase rwb = window.GetComponent<RewardWindowBase>();
						FacebookController.StoryPriority priority = FacebookController.StoryPriority.Green;
						rwb.priority = priority;
						rwb.shareAction = delegate
						{
							FacebookController.PostOpenGraphStory("get", "star", priority, new Dictionary<string, string> { 
							{
								"chapter",
								CurrentCampaignGame.boXName
							} });
						};
						rwb.HasReward = false;
						rwb.twitterStatus = () => string.Format("I've got all the stars in {0} in @PixelGun3D! Play now and try to get them! #pixelgun3d #pixelgun #pg3d http://goo.gl/8fzL9u", BoxNameForTwitter());
						rwb.EventTitle = "All Stars";
						AllStarsRewardSettings asrs = window.GetComponent<AllStarsRewardSettings>();
						foreach (UILabel lab2 in asrs.headerLabels)
						{
							if (CurrentCampaignGame.boXName == LevelBox.campaignBoxes[0].name)
							{
								lab2.text = LocalizationStore.Get("Key_1543");
							}
							else if (CurrentCampaignGame.boXName == LevelBox.campaignBoxes[1].name)
							{
								lab2.text = LocalizationStore.Get("Key_1544");
							}
							else if (CurrentCampaignGame.boXName == LevelBox.campaignBoxes[2].name)
							{
								lab2.text = LocalizationStore.Get("Key_1545");
							}
						}
						window.transform.parent = RentWindowPoint;
						Player_move_c.SetLayerRecursively(window, LayerMask.NameToLayer("Default"));
						window.transform.localPosition = new Vector3(0f, 0f, -130f);
						window.transform.localRotation = Quaternion.identity;
						window.transform.localScale = new Vector3(1f, 1f, 1f);
						continue;
					}
					if (_shouldShowAllSecretsCollectedRewardWindow && _numOfRewardWindowsShown < 2)
					{
						_shouldShowAllSecretsCollectedRewardWindow = false;
						PlayerPrefs.SetInt("AllSecretsForBoxRewardWindowIsShown_" + CurrentCampaignGame.boXName, 1);
						if ((!FacebookController.FacebookSupported && !TwitterController.TwitterSupported) || Device.isPixelGunLow)
						{
							continue;
						}
						GameObject window2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("NguiWindows/AllSecretsNGUI"));
						RewardWindowBase rwb2 = window2.GetComponent<RewardWindowBase>();
						FacebookController.StoryPriority priority2 = FacebookController.StoryPriority.Green;
						rwb2.priority = priority2;
						rwb2.shareAction = delegate
						{
							FacebookController.PostOpenGraphStory("find", "secret", priority2, new Dictionary<string, string> { 
							{
								"chapter",
								CurrentCampaignGame.boXName
							} });
						};
						rwb2.HasReward = false;
						rwb2.twitterStatus = () => string.Format("I've found all coins in {0} in @PixelGun3D! Play now and try to find them! #pixelgun3d #pixelgun #pg3d http://goo.gl/8fzL9u", BoxNameForTwitter());
						rwb2.EventTitle = "All Coins";
						AllSecretsRewardSettings asrs2 = window2.GetComponent<AllSecretsRewardSettings>();
						foreach (UILabel lab in asrs2.headerLabels)
						{
							if (CurrentCampaignGame.boXName == LevelBox.campaignBoxes[0].name)
							{
								lab.text = LocalizationStore.Get("Key_1540");
							}
							else if (CurrentCampaignGame.boXName == LevelBox.campaignBoxes[1].name)
							{
								lab.text = LocalizationStore.Get("Key_1541");
							}
							else if (CurrentCampaignGame.boXName == LevelBox.campaignBoxes[2].name)
							{
								lab.text = LocalizationStore.Get("Key_1542");
							}
						}
						window2.transform.parent = RentWindowPoint;
						Player_move_c.SetLayerRecursively(window2, LayerMask.NameToLayer("Default"));
						window2.transform.localPosition = new Vector3(0f, 0f, -130f);
						window2.transform.localRotation = Quaternion.identity;
						window2.transform.localScale = new Vector3(1f, 1f, 1f);
						continue;
					}
					bool premiumExpiredShown = Storager.getInt(Defs.PremiumEnabledFromServer, false) == 1 && ShopNGUIController.ShowPremimAccountExpiredIfPossible(RentWindowPoint, "Default", string.Empty);
				}
				catch (Exception e)
				{
					Debug.LogWarning("exception in LevelComplete  TryToShowExpiredBanner: " + e);
				}
			}
		}

		private void HandleMenuButton(object sender, EventArgs args)
		{
			if (_shopInstance != null)
			{
				return;
			}
			bool afterDeath = LastGameResult == GameResult.Death;
			string reasonToDismissInterstitialCampaign = LevelCompleteInterstitialRunner.GetReasonToDismissInterstitialCampaign(afterDeath);
			if (string.IsNullOrEmpty(reasonToDismissInterstitialCampaign))
			{
				if (Application.isEditor)
				{
					Debug.Log("<color=magenta>HandleMenuButton()</color>");
				}
				LevelCompleteInterstitialRunner levelCompleteInterstitialRunner = new LevelCompleteInterstitialRunner();
				levelCompleteInterstitialRunner.Run();
			}
			else
			{
				string format = ((!Application.isEditor) ? "Dismissing interstitial. {0}" : "<color=magenta>Dismissing interstitial. {0}</color>");
				Debug.LogFormat(format, reasonToDismissInterstitialCampaign);
			}
			string sceneName = ((!Defs.IsSurvival) ? "ChooseLevel" : Defs.MainMenuScene);
			Singleton<SceneLoader>.Instance.LoadScene(sceneName);
		}

		private void HandleQuitButton(object sender, EventArgs args)
		{
			bool afterDeath = LastGameResult == GameResult.Death;
			string reasonToDismissInterstitialSurvivalArena = LevelCompleteInterstitialRunner.GetReasonToDismissInterstitialSurvivalArena(afterDeath);
			if (string.IsNullOrEmpty(reasonToDismissInterstitialSurvivalArena))
			{
				if (Application.isEditor)
				{
					Debug.Log("<color=magenta>HandleQuitButton()</color>");
				}
				LevelCompleteInterstitialRunner levelCompleteInterstitialRunner = new LevelCompleteInterstitialRunner();
				levelCompleteInterstitialRunner.Run();
			}
			else
			{
				string format = ((!Application.isEditor) ? "Dismissing interstitial. {0}" : "<color=magenta>Dismissing interstitial. {0}</color>");
				Debug.LogFormat(format, reasonToDismissInterstitialSurvivalArena);
			}
			ActivityIndicator.IsActiveIndicator = true;
			loadingPanel.SetActive(true);
			mainPanel.SetActive(false);
			ExperienceController.sharedController.isShowRanks = false;
			Invoke("QuitLevel", 0.1f);
		}

		[Obfuscation(Exclude = true)]
		private void QuitLevel()
		{
			Singleton<SceneLoader>.Instance.LoadSceneAsync(Defs.MainMenuScene);
		}

		private static void SetInitialAmmoForAllGuns()
		{
			foreach (Weapon allAvailablePlayerWeapon in WeaponManager.sharedManager.allAvailablePlayerWeapons)
			{
				WeaponSounds component = allAvailablePlayerWeapon.weaponPrefab.GetComponent<WeaponSounds>();
				if (allAvailablePlayerWeapon.currentAmmoInClip + allAvailablePlayerWeapon.currentAmmoInBackpack < component.InitialAmmoWithEffectsApplied + component.ammoInClip)
				{
					allAvailablePlayerWeapon.currentAmmoInClip = component.ammoInClip;
					allAvailablePlayerWeapon.currentAmmoInBackpack = component.InitialAmmoWithEffectsApplied;
				}
				else if (allAvailablePlayerWeapon.currentAmmoInClip < component.ammoInClip)
				{
					int num = Mathf.Min(component.ammoInClip - allAvailablePlayerWeapon.currentAmmoInClip, allAvailablePlayerWeapon.currentAmmoInBackpack);
					allAvailablePlayerWeapon.currentAmmoInClip += num;
					allAvailablePlayerWeapon.currentAmmoInBackpack -= num;
				}
			}
		}

		private void HandleRetryButton(object sender, EventArgs args)
		{
			if (!(_shopInstance != null))
			{
				if (!Defs.IsSurvival)
				{
					SetInitialAmmoForAllGuns();
				}
				else
				{
					WeaponManager.sharedManager.Reset(0);
				}
				GlobalGameController.Score = 0;
				if (Defs.IsSurvival)
				{
					Defs.CurrentSurvMapIndex = UnityEngine.Random.Range(0, Defs.SurvivalMaps.Length);
				}
				Singleton<SceneLoader>.Instance.LoadScene("CampaignLoading");
			}
		}

		private void HandleFacebookButton(object sender, EventArgs args)
		{
			if (!(_shopInstance != null))
			{
				FacebookController.ShowPostDialog();
			}
		}

		private void HandleTwitterButton(object sender, EventArgs args)
		{
			if (Application.isEditor)
			{
				Debug.Log("Send Twitter: " + _SocialMessage());
			}
			else if (!(_shopInstance != null) && TwitterController.Instance != null)
			{
				TwitterController.Instance.PostStatusUpdate(_SocialMessage());
			}
		}

		private void HandleNextButton(object sender, EventArgs args)
		{
			if (!(_shopInstance != null))
			{
				if (!_isLastLevel)
				{
					CurrentCampaignGame.levelSceneName = _nextSceneName;
					SetInitialAmmoForAllGuns();
					LevelArt.endOfBox = false;
					Singleton<SceneLoader>.Instance.LoadScene((!LevelArt.ShouldShowArts) ? "CampaignLoading" : "LevelArt");
				}
				else
				{
					LevelArt.endOfBox = true;
					Singleton<SceneLoader>.Instance.LoadScene((!LevelArt.ShouldShowArts) ? "ChooseLevel" : "LevelArt");
				}
			}
		}

		[Obfuscation(Exclude = true)]
		private void GoToChooseLevel()
		{
			Singleton<SceneLoader>.Instance.LoadScene("ChooseLevel");
		}

		private void HandleShopButton(object sender, EventArgs args)
		{
			if (_shopInstance != null)
			{
				return;
			}
			_shopInstance = ShopNGUIController.sharedShop;
			if (_shopInstance != null)
			{
				_shopInstance.SetInGame(false);
				ShopNGUIController.GuiActive = true;
				if (shopButtonSound != null && Defs.isSoundFX)
				{
					NGUITools.PlaySound(shopButtonSound);
				}
				if (Defs.IsSurvival)
				{
					backgroundSurvivalTexture.SetActive(false);
				}
				else
				{
					backgroundTexture.SetActive(false);
				}
				_shopInstance.resumeAction = HandleResumeFromShop;
			}
			quitButton.SetActive(false);
		}

		private void HandleResumeFromShop()
		{
			if (!(_shopInstance == null))
			{
				ShopNGUIController.GuiActive = false;
				_shopInstance.resumeAction = delegate
				{
				};
				_shopInstance = null;
				if (coinsPlashka.thisScript != null)
				{
					coinsPlashka.thisScript.enabled = false;
				}
				quitButton.SetActive(Defs.IsSurvival);
				if (_experienceController != null)
				{
					_experienceController.isShowRanks = true;
				}
				if (Defs.IsSurvival)
				{
					backgroundSurvivalTexture.SetActive(true);
				}
				else
				{
					backgroundTexture.SetActive(true);
				}
			}
		}

		private static ExperienceController InitializeExperienceController()
		{
			ExperienceController experienceController = null;
			GameObject gameObject = GameObject.FindGameObjectWithTag("ExperienceController");
			if (gameObject != null)
			{
				experienceController = gameObject.GetComponent<ExperienceController>();
			}
			if (experienceController == null)
			{
				Debug.LogError("Cannot find experience controller.");
			}
			else
			{
				experienceController.posRanks = new Vector2(21f * Defs.Coef, 21f * Defs.Coef);
				experienceController.isShowRanks = true;
				if (ExpController.Instance != null)
				{
					ExpController.Instance.InterfaceEnabled = true;
				}
			}
			return experienceController;
		}

		private static int InitializeStarCount()
		{
			int num = 1;
			if (CurrentCampaignGame.completeInTime)
			{
				num++;
			}
			if (CurrentCampaignGame.withoutHits)
			{
				num++;
			}
			return num;
		}

		private bool InitializeAwardConferred()
		{
			return _isLastLevel && completedFirstTime;
		}

		private string _SocialMessage()
		{
			if (Defs.IsSurvival)
			{
				string text = string.Format(CultureInfo.InvariantCulture, LocalizationStore.GetByDefault("Key_1382"), WavesSurvivedStat.SurvivedWaveCount, PlayerPrefs.GetInt(Defs.KilledZombiesSett, 0), GlobalGameController.Score);
				Debug.Log(text);
				return text;
			}
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(CurrentCampaignGame.levelSceneName);
			if (infoScene != null)
			{
				if (!_gameOver)
				{
					string text2 = string.Format(LocalizationStore.GetByDefault("Key_1382"), infoScene.TranslateName, _starCount);
					Debug.Log(text2);
					return text2;
				}
				string text3 = string.Format(LocalizationStore.GetByDefault("Key_1380"), infoScene.TranslateName);
				Debug.Log(text3);
				return text3;
			}
			return "error map";
		}

		public static void SetInputEnabled(bool enabled)
		{
			if (_instance != null)
			{
				_instance.uiCamera.enabled = enabled;
			}
		}
	}
}

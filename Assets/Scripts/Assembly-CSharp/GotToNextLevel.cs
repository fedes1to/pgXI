using System;
using System.Collections.Generic;
using System.Globalization;
using Rilisoft;
using UnityEngine;

internal sealed class GotToNextLevel : MonoBehaviour
{
	private Action OnPlayerAddedAct;

	private GameObject _player;

	private Player_move_c _playerMoveC;

	private bool runLoading;

	private IDisposable _backSubscription;

	private void Awake()
	{
		OnPlayerAddedAct = delegate
		{
			_player = GameObject.FindGameObjectWithTag("Player");
			_playerMoveC = GameObject.FindGameObjectWithTag("PlayerGun").GetComponent<Player_move_c>();
		};
		Initializer.PlayerAddedEvent += OnPlayerAddedAct;
	}

	private void OnDestroy()
	{
		Initializer.PlayerAddedEvent -= OnPlayerAddedAct;
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.SetEnablePerfectLabel(false);
		}
	}

	private void Update()
	{
		if (!(_player == null) && !(_playerMoveC == null) && !runLoading && Vector3.SqrMagnitude(base.transform.position - _player.transform.position) < 2.25f)
		{
			runLoading = true;
			GoToNextLevelInstance();
			if (InGameGUI.sharedInGameGUI != null)
			{
				InGameGUI.sharedInGameGUI.SetEnablePerfectLabel(true);
			}
		}
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void HandleEscape()
	{
		if (Application.isEditor)
		{
			Debug.Log("Ignoring [Escape] after touching the portal.");
		}
	}

	private void GoToNextLevelInstance()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(HandleEscape, "Touching the Portal");
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Portal);
			Invoke("BannerTrainingCompleteInvoke", 2f);
			AutoFade.fadeKilled(2.05f, 0f, 0f, Color.white);
		}
		else
		{
			GoToNextLevel();
		}
	}

	private void GetRewardForTraining()
	{
		TrainingController.CompletedTrainingStage = TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted;
		AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Rewards);
		Storager.setInt("GrenadeID", 5, false);
		PlayerPrefs.Save();
		LevelCompleteLoader.sceneName = Defs.MainMenuScene;
		HashSet<RuntimePlatform> hashSet = new HashSet<RuntimePlatform>();
		hashSet.Add(RuntimePlatform.Android);
		hashSet.Add(RuntimePlatform.IPhonePlayer);
		hashSet.Add(RuntimePlatform.MetroPlayerX64);
		HashSet<RuntimePlatform> hashSet2 = hashSet;
		if (hashSet2.Contains(BuildSettings.BuildTargetPlatform) && !Storager.hasKey(Defs.GotCoinsForTraining))
		{
			if (!new Lazy<bool>(delegate
			{
				if (!Storager.UseSignedPreferences)
				{
					return false;
				}
				string value;
				if (!Defs2.SignedPreferences.TryGetValue("Manterry", out value))
				{
					return false;
				}
				if (!Defs2.SignedPreferences.Verify("Manterry"))
				{
					return true;
				}
				int result;
				return !int.TryParse(value, out result) || result % 2 == 1;
			}).Value)
			{
				int gemsForTraining = Defs.GemsForTraining;
				BankController.AddGems(gemsForTraining, false);
				int coinsForTraining = Defs.CoinsForTraining;
				BankController.AddCoins(coinsForTraining, false);
				AudioClip clip = Resources.Load<AudioClip>("coin_get");
				if (Defs.isSoundFX)
				{
					NGUITools.PlaySound(clip);
				}
				if (Storager.UseSignedPreferences)
				{
					int num = UnityEngine.Random.Range(255, 32767) << 2;
					Defs2.SignedPreferences.Add("Manterry", num.ToString(CultureInfo.InvariantCulture));
				}
			}
			else if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Skipping reward since it has been already claimed.");
			}
			if (ExperienceController.sharedController != null)
			{
				ExperienceController.sharedController.addExperience(Defs.ExpForTraining);
			}
		}
		MainMenuController.trainingCompleted = true;
		ShopNGUIController.GiveArmorArmy1OrNoviceArmor();
		try
		{
			Singleton<EggsManager>.Instance.AddEgg("egg_Training");
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in giving training egg: {0}", ex);
		}
		AnalyticsFacade.SendCustomEventToAppsFlyer("Training complete", new Dictionary<string, string>());
		TrainingController.TrainingCompletedFlagForLogging = true;
		PlayerPrefs.Save();
	}

	private void BannerTrainingCompleteInvoke()
	{
		GameObject.Find("Background_Training(Clone)").SetActive(false);
		coinsShop.hideCoinsShop();
		if (ABTestController.useBuffSystem)
		{
			Storager.setInt("Training.NoviceArmorUsedKey", 1, false);
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("NguiWindows/TrainigCompleteNGUI"));
		RewardWindowBase component = gameObject.GetComponent<RewardWindowBase>();
		FacebookController.StoryPriority priority = FacebookController.StoryPriority.Green;
		component.CollectOnlyNoShare = true;
		component.shareAction = delegate
		{
			FacebookController.PostOpenGraphStory("complete", "tutorial", priority);
		};
		component.customHide = delegate
		{
			GetRewardForTraining();
			ActivityIndicator.IsActiveIndicator = true;
			Invoke("LoadPromLevel", 0.4f);
		};
		component.priority = priority;
		component.twitterStatus = () => "Training completed in @PixelGun3D! Come to play with me! \n#pixelgun3d #pixelgun #3d #pg3d #mobile #fps #shooter http://goo.gl/8fzL9u";
		component.EventTitle = "Training Completed";
		component.HasReward = true;
		gameObject.transform.parent = InGameGUI.sharedInGameGUI.transform.GetChild(0);
		InGameGUI.sharedInGameGUI.joystikPanel.gameObject.SetActive(false);
		InGameGUI.sharedInGameGUI.interfacePanel.gameObject.SetActive(false);
		InGameGUI.sharedInGameGUI.shopPanel.gameObject.SetActive(false);
		InGameGUI.sharedInGameGUI.bloodPanel.gameObject.SetActive(false);
		Player_move_c.SetLayerRecursively(gameObject, LayerMask.NameToLayer("NGUI"));
		gameObject.transform.localPosition = new Vector3(0f, 0f, -130f);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		GameObject gameObject2 = new GameObject();
		UITexture uITexture = gameObject2.AddComponent<UITexture>();
		string path = ConnectSceneNGUIController.MainLoadingTexture();
		uITexture.mainTexture = Resources.Load<Texture>(path);
		uITexture.SetRect(0f, 0f, 1366f, 768f);
		uITexture.transform.SetParent(InGameGUI.sharedInGameGUI.transform.GetChild(0), false);
		uITexture.transform.localScale = Vector3.one;
		uITexture.transform.localPosition = Vector3.zero;
		if (WeaponManager.sharedManager.myPlayer != null)
		{
			WeaponManager.sharedManager.myPlayer.SetActive(false);
		}
	}

	private void LoadPromLevel()
	{
		Singleton<SceneLoader>.Instance.LoadScene("LevelToCompleteProm");
	}

	public static void GoToNextLevel()
	{
		LevelCompleteLoader.action = null;
		LevelCompleteLoader.sceneName = "LevelComplete";
		AutoFade.LoadLevel("LevelToCompleteProm", 2f, 0f, Color.white);
	}
}

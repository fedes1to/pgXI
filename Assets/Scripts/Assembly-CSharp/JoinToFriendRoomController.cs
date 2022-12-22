using System;
using System.Collections;
using System.Reflection;
using Rilisoft;
using UnityEngine;

public class JoinToFriendRoomController : MonoBehaviour
{
	public int gameMode;

	public string roomName;

	public GameObject connectPanel;

	public UITexture backgroundConnectTexture;

	public UILabel infoBoxLabel;

	public GameObject infoBoxContainer;

	public GameObject passwordPanel;

	public GameObject wrongPasswordLabel;

	public UIInput inputPasswordLabel;

	public static JoinToFriendRoomController Instance;

	private bool _isFaledConnectToRoom;

	private string _passwordRoom;

	private float _timerShowWrongPassword;

	private bool _isBackFromPassword;

	private IDisposable _backSubscription;

	private LoadingNGUIController _loadingNGUIController;

	private void Awake()
	{
		inputPasswordLabel.onSubmit.Add(new EventDelegate(delegate
		{
			EnterPassword(inputPasswordLabel.value);
		}));
	}

	private void Start()
	{
		Instance = this;
	}

	private void OnEnable()
	{
		PhotonObjectCacher.AddObject(base.gameObject);
	}

	private void OnEsc()
	{
		PhotonNetwork.isMessageQueueRunning = true;
		PhotonNetwork.Disconnect();
		closeConnectPanel();
	}

	private void OnDisable()
	{
		PhotonObjectCacher.RemoveObject(base.gameObject);
	}

	private void OnDestroy()
	{
		Instance = null;
		PhotonObjectCacher.RemoveObject(base.gameObject);
	}

	public void BackFromPasswordButton()
	{
		_isBackFromPassword = true;
		SetEnabledPasswordPanel(false);
		PhotonNetwork.isMessageQueueRunning = true;
		PhotonNetwork.Disconnect();
	}

	public void OnClickAcceptPassword()
	{
		EnterPassword(inputPasswordLabel.value);
	}

	public void EnterPassword(string pass)
	{
		if (pass == _passwordRoom)
		{
			PhotonNetwork.isMessageQueueRunning = false;
			StartCoroutine(MoveToGameScene());
			ActivityIndicator.IsActiveIndicator = true;
		}
		else
		{
			_timerShowWrongPassword = 3f;
			wrongPasswordLabel.SetActive(true);
		}
	}

	private void ShowLoadingGUI(string _mapName)
	{
		_loadingNGUIController = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("LoadingGUI")).GetComponent<LoadingNGUIController>();
		_loadingNGUIController.SceneToLoad = _mapName;
		_loadingNGUIController.loadingNGUITexture.mainTexture = Resources.Load<Texture2D>("LevelLoadings" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/Loading_" + _mapName);
		_loadingNGUIController.transform.parent = backgroundConnectTexture.transform.parent;
		_loadingNGUIController.transform.localPosition = Vector3.zero;
		_loadingNGUIController.Init();
	}

	private void RemoveLoadingGUI()
	{
		if (_loadingNGUIController != null)
		{
			UnityEngine.Object.Destroy(_loadingNGUIController.gameObject);
			_loadingNGUIController = null;
		}
	}

	private IEnumerator SetFonLoadingWaitForReset(string _mapName = "", bool isAddCountRun = false)
	{
		RemoveLoadingGUI();
		while (WeaponManager.sharedManager == null)
		{
			yield return null;
		}
		while (WeaponManager.sharedManager.LockGetWeaponPrefabs > 0)
		{
			yield return null;
		}
		ShowLoadingGUI(_mapName);
	}

	public void ConnectToRoom(int gameModeCode, string nameRoom, string map)
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(OnEsc, "Connect To Friend");
		InfoWindowController.HideCurrentWindow();
		SetEnabledPasswordPanel(false);
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(map));
		if (infoScene.isPremium && Storager.getInt(infoScene.NameScene + "Key", true) != 1 && !PremiumAccountController.MapAvailableDueToPremiumAccount(infoScene.NameScene))
		{
			Action successfulUnlockCallback = delegate
			{
			};
			ShowUnlockMapDialog(successfulUnlockCallback, infoScene.NameScene);
			return;
		}
		int gameTier = ((gameModeCode <= 99) ? (gameModeCode / 10) : (gameModeCode % 100 / 10));
		gameMode = gameModeCode % 10;
		roomName = nameRoom;
		Defs.isMulti = true;
		Defs.isInet = true;
		Defs.isFlag = false;
		Defs.isCOOP = false;
		Defs.isCompany = false;
		Defs.isHunger = false;
		Defs.isCapturePoints = false;
		Defs.isDuel = false;
		switch (gameMode)
		{
		default:
			return;
		case 0:
			StoreKitEventListener.State.Mode = "Deathmatch Wordwide";
			ConnectSceneNGUIController.regim = ConnectSceneNGUIController.RegimGame.Deathmatch;
			break;
		case 1:
			StoreKitEventListener.State.Mode = "Time Survival";
			Defs.isCOOP = true;
			ConnectSceneNGUIController.regim = ConnectSceneNGUIController.RegimGame.TimeBattle;
			break;
		case 2:
			StoreKitEventListener.State.Mode = "Team Battle";
			Defs.isCompany = true;
			ConnectSceneNGUIController.regim = ConnectSceneNGUIController.RegimGame.TeamFight;
			break;
		case 3:
			if (true)
			{
				Defs.isHunger = true;
				StoreKitEventListener.State.Mode = "Deadly Games";
				ConnectSceneNGUIController.regim = ConnectSceneNGUIController.RegimGame.DeadlyGames;
				break;
			}
			if (ShowNoJoinConnectFromRanks.sharedController != null)
			{
				ShowNoJoinConnectFromRanks.sharedController.resetShow(3);
			}
			return;
		case 4:
			if (true)
			{
				Defs.isFlag = true;
				ConnectSceneNGUIController.regim = ConnectSceneNGUIController.RegimGame.FlagCapture;
				break;
			}
			StoreKitEventListener.State.Mode = "Flag Capture";
			if (ShowNoJoinConnectFromRanks.sharedController != null)
			{
				ShowNoJoinConnectFromRanks.sharedController.resetShow(4);
			}
			return;
		case 5:
			Defs.isCapturePoints = true;
			ConnectSceneNGUIController.regim = ConnectSceneNGUIController.RegimGame.CapturePoints;
			break;
		case 8:
			Defs.isDuel = true;
			ConnectSceneNGUIController.regim = ConnectSceneNGUIController.RegimGame.Duel;
			break;
		case 6:
		case 7:
			return;
		}
		ActivityIndicator.IsActiveIndicator = true;
		connectPanel.SetActive(true);
		infoBoxLabel.gameObject.SetActive(false);
		infoBoxContainer.SetActive(false);
		WeaponManager.sharedManager.Reset((int)infoScene.AvaliableWeapon);
		StartCoroutine(SetFonLoadingWaitForReset(infoScene.NameScene));
		Defs.isDaterRegim = infoScene.AvaliableWeapon == ModeWeapon.dater;
		string gameVersion = Initializer.Separator + ConnectSceneNGUIController.regim.ToString() + (Defs.isDaterRegim ? "Dater" : ((!Defs.isHunger) ? gameTier.ToString() : "0")) + "v" + GlobalGameController.MultiplayerProtocolVersion;
		ConnectSceneNGUIController.gameTier = gameTier;
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.ConnectUsingSettings(gameVersion);
	}

	private void Update()
	{
		if (coinsPlashka.thisScript != null)
		{
			coinsPlashka.thisScript.enabled = coinsShop.thisScript != null && coinsShop.thisScript.enabled;
		}
		if (_timerShowWrongPassword > 0f && wrongPasswordLabel.activeSelf)
		{
			_timerShowWrongPassword -= Time.deltaTime;
		}
		if (_timerShowWrongPassword <= 0f && wrongPasswordLabel.activeSelf)
		{
			wrongPasswordLabel.SetActive(false);
		}
	}

	private void ShowUnlockMapDialog(Action successfulUnlockCallback, string levelName)
	{
		if (string.IsNullOrEmpty(levelName))
		{
			Debug.LogWarning("Level name shoul not be empty.");
			return;
		}
		UnityEngine.Object original = Resources.Load("UnlockPremiumMapView");
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		Tools.SetLayerRecursively(gameObject, base.gameObject.layer);
		ActivityIndicator.IsActiveIndicator = false;
		UnlockPremiumMapView unlockPremiumMapView = gameObject.GetComponent<UnlockPremiumMapView>();
		if (unlockPremiumMapView == null)
		{
			Debug.LogError("UnlockPremiumMapView should not be null.");
			return;
		}
		int value = 0;
		Defs.PremiumMaps.TryGetValue(levelName, out value);
		unlockPremiumMapView.Price = value;
		EventHandler value2 = delegate
		{
			HandleCloseUnlockDialog(unlockPremiumMapView);
		};
		EventHandler value3 = delegate
		{
			HandleUnlockPressed(unlockPremiumMapView, successfulUnlockCallback, levelName);
		};
		unlockPremiumMapView.ClosePressed += value2;
		unlockPremiumMapView.UnlockPressed += value3;
	}

	private void HandleCloseUnlockDialog(UnlockPremiumMapView unlockPremiumMapView)
	{
		closeConnectPanel();
		UnityEngine.Object.Destroy(unlockPremiumMapView.gameObject);
	}

	private void HandleUnlockPressed(UnlockPremiumMapView unlockPremiumMapView, Action successfulUnlockCallback, string levelName)
	{
		int priceAmount = unlockPremiumMapView.Price;
		ShopNGUIController.TryToBuy(base.transform.root.gameObject, new ItemPrice(unlockPremiumMapView.Price, "Coins"), delegate
		{
			Storager.setInt(levelName + "Key", 1, true);
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				PlayerPrefs.Save();
			}
			ShopNGUIController.SynchronizeAndroidPurchases("Friend's map unlocked: " + levelName);
			AnalyticsStuff.LogSales(levelName, "Premium Maps");
			AnalyticsFacade.InAppPurchase(levelName, "Premium Maps", 1, priceAmount, "Coins");
			if (coinsPlashka.thisScript != null)
			{
				coinsPlashka.thisScript.enabled = false;
			}
			successfulUnlockCallback();
			UnityEngine.Object.Destroy(unlockPremiumMapView.gameObject);
		}, delegate
		{
			StoreKitEventListener.State.PurchaseKey = "In map selection In Friends";
		});
	}

	[Obfuscation(Exclude = true)]
	public void closeConnectPanel()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
		backgroundConnectTexture.mainTexture = null;
		RemoveLoadingGUI();
		connectPanel.SetActive(false);
		infoBoxLabel.gameObject.SetActive(false);
		infoBoxContainer.SetActive(false);
		ActivityIndicator.IsActiveIndicator = false;
	}

	private void ShowLabel(string text)
	{
		infoBoxLabel.text = text;
		infoBoxLabel.gameObject.SetActive(true);
		infoBoxContainer.SetActive(true);
		ActivityIndicator.IsActiveIndicator = false;
		Invoke("closeConnectPanel", 3f);
	}

	private void OnDisconnectedFromPhoton()
	{
		if (_isFaledConnectToRoom)
		{
			ShowLabel(LocalizationStore.Get("Key_1410"));
		}
		else if (_isBackFromPassword)
		{
			closeConnectPanel();
		}
		else
		{
			ShowLabel(LocalizationStore.Get("Key_1410"));
		}
		_isFaledConnectToRoom = false;
		_isBackFromPassword = false;
	}

	private void OnFailedToConnectToPhoton(object parameters)
	{
		ShowLabel(LocalizationStore.Get("Key_1411"));
		Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters);
	}

	public void OnConnectedToMaster()
	{
		ConnectToRoom();
	}

	public void OnJoinedLobby()
	{
		ConnectToRoom();
	}

	[Obfuscation(Exclude = true)]
	private void ConnectToRoom()
	{
		PhotonNetwork.playerName = ProfileController.GetPlayerNameOrDefault();
		Debug.Log("OnJoinedLobby " + roomName);
		PhotonNetwork.JoinRoom(roomName);
		PlayerPrefs.SetString("RoomName", roomName);
	}

	private void OnPhotonJoinRoomFailed()
	{
		Debug.Log("OnPhotonJoinRoomFailed - init");
		_isFaledConnectToRoom = true;
		PhotonNetwork.isMessageQueueRunning = true;
		PhotonNetwork.Disconnect();
		InfoWindowController.ShowInfoBox(LocalizationStore.Get("Key_0137"));
		InfoWindowController.HideProcessing(3f);
		Defs.isFlag = false;
		Defs.isCOOP = false;
		Defs.isCompany = false;
		Defs.isHunger = false;
		Defs.isDuel = false;
		Defs.isCapturePoints = false;
		Defs.isDaterRegim = false;
		WeaponManager.sharedManager.Reset(0);
	}

	private void SetEnabledPasswordPanel(bool enabled)
	{
		passwordPanel.SetActive(enabled);
		if (_loadingNGUIController != null)
		{
			backgroundConnectTexture.mainTexture = ((!enabled) ? null : _loadingNGUIController.loadingNGUITexture.mainTexture);
			_loadingNGUIController.gameObject.SetActive(!enabled);
		}
	}

	private void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom - init");
		if (PhotonNetwork.room != null)
		{
			_passwordRoom = PhotonNetwork.room.customProperties[ConnectSceneNGUIController.passwordProperty].ToString();
			PhotonNetwork.isMessageQueueRunning = false;
			if (_passwordRoom.Equals(string.Empty))
			{
				PhotonNetwork.isMessageQueueRunning = false;
				StartCoroutine(MoveToGameScene());
				return;
			}
			Debug.Log("Show Password Panel " + _passwordRoom);
			ActivityIndicator.IsActiveIndicator = false;
			inputPasswordLabel.value = string.Empty;
			SetEnabledPasswordPanel(true);
		}
		else
		{
			PhotonNetwork.isMessageQueueRunning = true;
			PhotonNetwork.Disconnect();
			ShowLabel(LocalizationStore.Get("Key_1410"));
		}
	}

	private IEnumerator MoveToGameScene()
	{
		AnalyticsStuff.LogMultiplayer();
		if (SceneLoader.ActiveSceneName.Equals("Clans"))
		{
			Defs.isGameFromFriends = false;
			Defs.isGameFromClans = true;
		}
		else
		{
			Defs.isGameFromFriends = true;
			Defs.isGameFromClans = false;
		}
		SceneInfo scInfo = SceneInfoController.instance.GetInfoScene(int.Parse(PhotonNetwork.room.customProperties[ConnectSceneNGUIController.mapProperty].ToString()));
		string mapName = scInfo.NameScene;
		WeaponManager.sharedManager.Reset((int)scInfo.AvaliableWeapon);
		while (PhotonNetwork.room == null)
		{
			yield return 0;
		}
		Debug.Log("map = " + PhotonNetwork.room.customProperties[ConnectSceneNGUIController.mapProperty].ToString());
		Debug.Log(mapName);
		LoadConnectScene.textureToShow = Resources.Load("LevelLoadings" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/Loading_" + mapName) as Texture2D;
		LoadingInAfterGame.loadingTexture = LoadConnectScene.textureToShow;
		LoadConnectScene.sceneToLoad = mapName;
		LoadConnectScene.noteToShow = null;
		yield return Singleton<SceneLoader>.Instance.LoadSceneAsync("PromScene");
	}
}

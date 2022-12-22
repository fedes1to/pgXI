using ExitGames.Client.Photon;
using Rilisoft;
using UnityEngine;

public class DuelController : MonoBehaviour
{
	public enum GameStatus
	{
		None,
		WaitForOpponent,
		OpponentConnected,
		ReadyToStart,
		Playing,
		WaitForPlayerBack,
		RoomClosed,
		End,
		DisconnectInMatch,
		ChangeArea
	}

	public enum RoomStatus
	{
		None,
		Closed,
		MatchStarted
	}

	private const float duelTime = 120f;

	public static DuelController instance;

	[HideInInspector]
	public CharacterInterface myCharacter;

	[HideInInspector]
	public CharacterInterface enemyCharacter;

	public Transform myCharacterPoint;

	public Transform enemyCharacterPoint;

	private float _timeLeft;

	private float waitPlayerBackTime;

	private float nextSynchronizeTime;

	private int lastTimer = -1;

	private bool opponentLeftInEnd;

	private float goTimer;

	private PhotonView photonView;

	[HideInInspector]
	public GameStatus gameStatus;

	[HideInInspector]
	public NetworkStartTable opponentNetworkTable;

	[HideInInspector]
	public int myRespawnPoints = 1;

	private bool requestSended;

	private bool requestReceived;

	private bool roomHidden;

	private bool _wearIsInvisible;

	private bool equippedPetActionSet;

	public float timeLeft
	{
		get
		{
			return _timeLeft;
		}
	}

	public float playingTime
	{
		get
		{
			return 120f - _timeLeft;
		}
	}

	[HideInInspector]
	public RoomStatus roomStatus
	{
		get
		{
			return (RoomStatus)(int)PhotonNetwork.room.customProperties[ConnectSceneNGUIController.roomStatusProperty];
		}
		set
		{
			Hashtable hashtable = new Hashtable();
			hashtable["Closed"] = (int)value;
			PhotonNetwork.room.SetCustomProperties(hashtable);
		}
	}

	public bool showEnemyCharacter
	{
		get
		{
			return (opponentNetworkTable != null || opponentLeftInEnd) && duelUI != null && duelUI.showCharacters && gameStatus != GameStatus.WaitForOpponent && gameStatus != GameStatus.OpponentConnected && gameStatus != GameStatus.ChangeArea;
		}
	}

	public bool showMyCharacter
	{
		get
		{
			return duelUI != null && duelUI.showCharacters;
		}
	}

	public DuelUIController duelUI
	{
		get
		{
			return (!(NetworkStartTableNGUIController.sharedController != null)) ? null : NetworkStartTableNGUIController.sharedController.duelUI;
		}
	}

	public bool isMaster
	{
		get
		{
			return PhotonNetwork.isMasterClient;
		}
	}

	private void OnDestroy()
	{
		PhotonObjectCacher.RemoveObject(base.gameObject);
		ShopNGUIController.sharedShop.wearEquipAction = null;
		ShopNGUIController.sharedShop.wearUnequipAction = null;
		ShopNGUIController.sharedShop.equipAction = null;
		ShopNGUIController.sharedShop.onEquipSkinAction = null;
		ShopNGUIController.ShowWearChanged -= OnMyWearVisibleChanged;
		ShopNGUIController.ShowArmorChanged -= OnMyArmorVisibleChanged;
		if (equippedPetActionSet)
		{
			ShopNGUIController.EquippedPet -= OnPetEquipAction;
			ShopNGUIController.UnequippedPet -= OnPetUnequipAction;
		}
	}

	private void Awake()
	{
		base.enabled = Defs.isDuel && Defs.isMulti;
		instance = this;
		photonView = GetComponent<PhotonView>();
		PhotonObjectCacher.AddObject(base.gameObject);
	}

	private void Start()
	{
		SetShopEvents();
	}

	public void StartDuelMode()
	{
		if (myCharacter == null)
		{
			myCharacter = CreateCharacter(myCharacterPoint);
			myCharacter.isDuelInstance = true;
		}
		if (enemyCharacter == null)
		{
			enemyCharacter = CreateCharacter(enemyCharacterPoint);
			enemyCharacter.isDuelInstance = true;
			enemyCharacter.enemyInDuel = true;
		}
		SetMySkin();
		SetMyCape();
		SetMyMask();
		SetMyHat();
		SetMyBoots();
		SetMyWeapon();
		SetMyArmor();
		SetMyPet();
		if (gameStatus == GameStatus.None)
		{
			gameStatus = GameStatus.WaitForOpponent;
		}
	}

	private void StartMatch()
	{
		float num = 120f;
		PhotonNetwork.room.visible = false;
		roomHidden = true;
		ChangeRoomStatus(RoomStatus.MatchStarted);
		StartMatchRPC(num, myRespawnPoints);
		photonView.RPC("StartMatchRPC", PhotonTargets.Others, num, (myRespawnPoints == 2) ? 1 : 2);
	}

	public void StartRevengeMatch()
	{
		gameStatus = GameStatus.OpponentConnected;
		GoMatch();
	}

	private void GoMatch()
	{
		float num = 5f;
		GoMatchRPC(num);
		photonView.RPC("GoMatchRPC", PhotonTargets.Others, num);
	}

	[PunRPC]
	private void GoMatchRPC(float timer)
	{
		ChangeRoomStatus(RoomStatus.None);
		requestSended = false;
		requestReceived = false;
		duelUI.revengeButton.GetComponent<UIButton>().isEnabled = true;
		duelUI.ShowVersusUI();
		gameStatus = GameStatus.ReadyToStart;
		goTimer = timer;
	}

	private void ResumeMatch()
	{
		gameStatus = GameStatus.Playing;
		photonView.RPC("ResumeMatchRPC", PhotonTargets.Others, timeLeft, (myRespawnPoints == 2) ? 1 : 2);
	}

	[PunRPC]
	public void ResumeMatchRPC(float matchTime, int spawnPoint)
	{
		myRespawnPoints = spawnPoint;
		if (gameStatus == GameStatus.OpponentConnected || gameStatus == GameStatus.ReadyToStart)
		{
			StartMatchRPC(matchTime, spawnPoint);
		}
		else if (gameStatus == GameStatus.DisconnectInMatch)
		{
			gameStatus = GameStatus.Playing;
			_timeLeft = matchTime;
		}
	}

	[PunRPC]
	public void StartMatchRPC(float matchTime, int spawnPoint)
	{
		myRespawnPoints = spawnPoint;
		StartPlayer();
		_timeLeft = matchTime;
	}

	private void StartPlayer()
	{
		if (WeaponManager.sharedManager.myPlayerMoveC == null)
		{
			WeaponManager.sharedManager.myNetworkStartTable.StartPlayerButtonClick(0);
		}
		duelUI.revengeButton.GetComponent<UIButton>().isEnabled = true;
		gameStatus = GameStatus.Playing;
		duelUI.IngameUI();
	}

	[PunRPC]
	public void SynchronizeTimeRPC(float matchTime)
	{
		if (!isMaster)
		{
			_timeLeft = matchTime;
		}
	}

	private void EndMatch()
	{
		SetMyWeapon();
		SendMyWeapon();
		gameStatus = GameStatus.End;
		if (WeaponManager.sharedManager.myTable != null)
		{
			WeaponManager.sharedManager.myNetworkStartTable.win(string.Empty);
		}
		ChangeRoomStatus(RoomStatus.Closed);
	}

	public void OpponentConnected()
	{
		SendMyWearInvisible();
		SendMySkin();
		SendMyCape();
		SendMyMask();
		SendMyHat();
		SendMyBoots();
		SendMyWeapon();
		SendMyArmor();
		SendMyPet();
		if (gameStatus == GameStatus.WaitForOpponent)
		{
			gameStatus = GameStatus.OpponentConnected;
			goTimer = 2f;
		}
	}

	public void RevengeRequest()
	{
		if (!requestSended)
		{
			requestSended = true;
			photonView.RPC("RevengeRequestRPC", PhotonTargets.Others);
			duelUI.ShowRevengePanel(false, false);
			duelUI.revengeButton.GetComponent<UIButton>().isEnabled = false;
			if (requestReceived && isMaster)
			{
				StartRevengeMatch();
			}
		}
	}

	[PunRPC]
	public void RevengeRequestRPC()
	{
		requestReceived = true;
		if (!requestSended)
		{
			duelUI.ShowRevengePanel(true, false);
		}
		if (requestSended && isMaster)
		{
			StartRevengeMatch();
		}
	}

	[PunRPC]
	public void SetWearIsInvisibleRPC(bool isInvisible)
	{
		if (_wearIsInvisible != isInvisible)
		{
			_wearIsInvisible = isInvisible;
			enemyCharacter.UpdateCape(enemyCharacter.CurrentCapeId, enemyCharacter.CurrentCapeTexture, isInvisible);
			enemyCharacter.UpdateBoots(enemyCharacter.CurrentBootsId, isInvisible);
			enemyCharacter.UpdateHat(enemyCharacter.CurrentHatId, isInvisible);
			enemyCharacter.UpdateMask(enemyCharacter.CurrentMaskId, isInvisible);
		}
	}

	[PunRPC]
	public void SetEnemySkin(byte[] skin)
	{
		Texture2D texture2D = new Texture2D(64, 32);
		texture2D.LoadImage(skin);
		texture2D.filterMode = FilterMode.Point;
		texture2D.Apply();
		enemyCharacter.SetSkin(texture2D, (!(enemyCharacter.weapon != null)) ? null : enemyCharacter.weapon.GetComponent<WeaponSounds>());
	}

	[PunRPC]
	public void SetEnemyCape(string cape)
	{
		enemyCharacter.UpdateCape(cape, null, _wearIsInvisible);
	}

	[PunRPC]
	public void SetEnemyCape(string cape, byte[] skin)
	{
		Texture2D texture2D = new Texture2D(12, 16);
		texture2D.LoadImage(skin);
		texture2D.filterMode = FilterMode.Point;
		texture2D.Apply();
		enemyCharacter.UpdateCape(cape, texture2D, _wearIsInvisible);
	}

	[PunRPC]
	public void SetEnemyArmor(string armor, bool isInvisible)
	{
		enemyCharacter.UpdateArmor(armor, enemyCharacter.weapon, isInvisible || _wearIsInvisible);
	}

	[PunRPC]
	public void SetEnemyMask(string mask)
	{
		enemyCharacter.UpdateMask(mask, _wearIsInvisible);
	}

	[PunRPC]
	public void SetEnemyBoots(string boots)
	{
		enemyCharacter.UpdateBoots(boots, _wearIsInvisible);
	}

	[PunRPC]
	public void SetEnemyHat(string hat)
	{
		enemyCharacter.UpdateHat(hat, _wearIsInvisible);
	}

	[PunRPC]
	public void SetEnemyWeapon(string weapon, string altWeapon)
	{
		enemyCharacter.SetWeapon(weapon, altWeapon, string.Empty);
	}

	[PunRPC]
	public void SetEnemyWeapon(string weapon, string altWeapon, string skin)
	{
		enemyCharacter.SetWeapon(weapon, altWeapon, skin);
	}

	[PunRPC]
	public void SetEnemyPet(string petID)
	{
		enemyCharacter.UpdatePet(petID);
	}

	public void SendMyWearInvisible()
	{
		photonView.RPC("SetWearIsInvisibleRPC", PhotonTargets.Others, !ShopNGUIController.ShowWear);
	}

	public void SendMySkin()
	{
		photonView.RPC("SetEnemySkin", PhotonTargets.Others, SkinsController.currentSkinForPers.EncodeToPNG());
	}

	public void SendMyCape()
	{
		string @string = Storager.getString(Defs.CapeEquppedSN, false);
		if (!@string.Equals("cape_Custom"))
		{
			photonView.RPC("SetEnemyCape", PhotonTargets.Others, @string);
			return;
		}
		Texture2D capeUserTexture = SkinsController.capeUserTexture;
		byte[] array = capeUserTexture.EncodeToPNG();
		if (capeUserTexture.width == 12 && capeUserTexture.height == 16)
		{
			photonView.RPC("SetEnemyCape", PhotonTargets.Others, @string, array);
		}
	}

	public void SendMyArmor()
	{
		string @string = Storager.getString(Defs.ArmorNewEquppedSN, false);
		bool flag = !ShopNGUIController.ShowArmor;
		photonView.RPC("SetEnemyArmor", PhotonTargets.Others, @string, flag);
	}

	public void SendMyMask()
	{
		string @string = Storager.getString("MaskEquippedSN", false);
		photonView.RPC("SetEnemyMask", PhotonTargets.Others, @string);
	}

	public void SendMyBoots()
	{
		string @string = Storager.getString(Defs.BootsEquppedSN, false);
		photonView.RPC("SetEnemyBoots", PhotonTargets.Others, @string);
	}

	public void SendMyHat()
	{
		string @string = Storager.getString(Defs.HatEquppedSN, false);
		photonView.RPC("SetEnemyHat", PhotonTargets.Others, @string);
	}

	public void SendMyWeapon()
	{
		int index = WeaponManager.sharedManager.CurrentIndexOfLastUsedWeaponInPlayerWeapons();
		Weapon weapon = WeaponManager.sharedManager.playerWeapons[index] as Weapon;
		WeaponSkin skinForWeapon = WeaponSkinsManager.GetSkinForWeapon(weapon.weaponPrefab.name);
		if (skinForWeapon != null)
		{
			photonView.RPC("SetEnemyWeapon", PhotonTargets.Others, weapon.weaponPrefab.name, weapon.weaponPrefab.GetComponent<WeaponSounds>().alternativeName, skinForWeapon.Id);
		}
		else
		{
			photonView.RPC("SetEnemyWeapon", PhotonTargets.Others, weapon.weaponPrefab.name, weapon.weaponPrefab.GetComponent<WeaponSounds>().alternativeName);
		}
	}

	public void SendMyPet()
	{
		string eqipedPetId = Singleton<PetsManager>.Instance.GetEqipedPetId();
		photonView.RPC("SetEnemyPet", PhotonTargets.Others, eqipedPetId);
	}

	public void SetMySkin()
	{
		myCharacter.SetSkin(SkinsController.currentSkinForPers, (!(myCharacter.weapon != null)) ? null : myCharacter.weapon.GetComponent<WeaponSounds>());
	}

	public void SetMyCape()
	{
		string @string = Storager.getString(Defs.CapeEquppedSN, false);
		myCharacter.UpdateCape(@string, (!@string.Equals("cape_Custom")) ? null : SkinsController.capeUserTexture, !ShopNGUIController.ShowWear);
	}

	public void SetMyArmor()
	{
		string @string = Storager.getString(Defs.ArmorNewEquppedSN, false);
		bool isInvisible = !ShopNGUIController.ShowArmor;
		myCharacter.UpdateArmor(@string, myCharacter.weapon, isInvisible);
	}

	public void SetMyMask()
	{
		string @string = Storager.getString("MaskEquippedSN", false);
		myCharacter.UpdateMask(@string, !ShopNGUIController.ShowWear);
	}

	public void SetMyBoots()
	{
		string @string = Storager.getString(Defs.BootsEquppedSN, false);
		myCharacter.UpdateBoots(@string, !ShopNGUIController.ShowWear);
	}

	public void SetMyHat()
	{
		string @string = Storager.getString(Defs.HatEquppedSN, false);
		myCharacter.UpdateHat(@string, !ShopNGUIController.ShowWear);
	}

	public void SetMyPet()
	{
		string eqipedPetId = Singleton<PetsManager>.Instance.GetEqipedPetId();
		myCharacter.UpdatePet(eqipedPetId);
	}

	public void SetMyWearInvisible()
	{
		SetMyCape();
		SetMyMask();
		SetMyHat();
		SetMyBoots();
	}

	public void SetMyWeapon()
	{
		int index = WeaponManager.sharedManager.CurrentIndexOfLastUsedWeaponInPlayerWeapons();
		Weapon weapon = WeaponManager.sharedManager.playerWeapons[index] as Weapon;
		WeaponSkin skinForWeapon = WeaponSkinsManager.GetSkinForWeapon(weapon.weaponPrefab.name);
		myCharacter.SetWeapon(weapon.weaponPrefab.name, weapon.weaponPrefab.name, (skinForWeapon == null) ? string.Empty : skinForWeapon.Id);
	}

	public void SendGameLeft()
	{
		photonView.RPC("OpponentLeftGame", PhotonTargets.Others, PhotonNetwork.player);
	}

	[PunRPC]
	private void OpponentLeftGame(PhotonPlayer player)
	{
		OnPhotonPlayerDisconnected(player);
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		if (gameStatus == GameStatus.ReadyToStart || gameStatus == GameStatus.OpponentConnected)
		{
			if (!roomHidden)
			{
				gameStatus = GameStatus.WaitForOpponent;
				duelUI.ShowWaitForOpponentInterface();
			}
			else
			{
				gameStatus = GameStatus.End;
				ChangeRoomStatus(RoomStatus.Closed);
				duelUI.ShowChangeAreaInterface(true);
			}
		}
		if (gameStatus == GameStatus.Playing)
		{
			if (InGameGUI.sharedInGameGUI != null)
			{
				InGameGUI.sharedInGameGUI.waitDuelLabel.SetActive(true);
			}
			gameStatus = GameStatus.WaitForPlayerBack;
			waitPlayerBackTime = 30f;
		}
		if (gameStatus == GameStatus.End)
		{
			opponentLeftInEnd = true;
			duelUI.ShowRevengePanel(false, true);
		}
	}

	public void OnDisconnectedFromPhoton()
	{
		if (gameStatus == GameStatus.Playing)
		{
			gameStatus = GameStatus.DisconnectInMatch;
		}
		if (gameStatus == GameStatus.ReadyToStart)
		{
			gameStatus = GameStatus.WaitForOpponent;
		}
	}

	private void ChangeRoomStatus(RoomStatus newStatus)
	{
		roomStatus = newStatus;
		Debug.Log((newStatus != RoomStatus.Closed) ? "Room opened!" : "Room closed!");
	}

	private void FindEnemyTable()
	{
		if (WeaponManager.sharedManager.myNetworkStartTable == null)
		{
			return;
		}
		for (int i = 0; i < Initializer.networkTables.Count; i++)
		{
			if (!Initializer.networkTables[i].Equals(WeaponManager.sharedManager.myNetworkStartTable))
			{
				opponentNetworkTable = Initializer.networkTables[i];
				OpponentConnected();
				break;
			}
		}
	}

	private void Update()
	{
		if (opponentNetworkTable == null)
		{
			FindEnemyTable();
		}
		if (gameStatus == GameStatus.Playing)
		{
			_timeLeft -= Time.deltaTime;
			if (_timeLeft < 0f)
			{
				_timeLeft = 0f;
				EndMatch();
			}
			if (isMaster && nextSynchronizeTime < Time.time)
			{
				nextSynchronizeTime = Time.time + 0.5f;
				photonView.RPC("SynchronizeTimeRPC", PhotonTargets.Others, _timeLeft);
			}
		}
		if ((isMaster && gameStatus == GameStatus.WaitForPlayerBack) || gameStatus == GameStatus.RoomClosed)
		{
			waitPlayerBackTime -= Time.deltaTime;
			if (InGameGUI.sharedInGameGUI != null)
			{
				InGameGUI.sharedInGameGUI.waitDuelLabelTimer.text = LocalizationStore.Get("Key_1126") + " " + Mathf.RoundToInt(waitPlayerBackTime);
			}
			if (waitPlayerBackTime < 5f && gameStatus == GameStatus.WaitForPlayerBack)
			{
				gameStatus = GameStatus.RoomClosed;
				ChangeRoomStatus(RoomStatus.Closed);
			}
			if (waitPlayerBackTime < 0f)
			{
				waitPlayerBackTime = 0f;
				_timeLeft = 0f;
				EndMatch();
			}
		}
		if (gameStatus == GameStatus.ReadyToStart)
		{
			goTimer -= Time.deltaTime;
			int num = (int)Mathf.Floor(goTimer);
			if (duelUI != null)
			{
				if (lastTimer == -1 || lastTimer > num)
				{
					lastTimer = num;
					duelUI.versusTimer.GetComponent<TweenScale>().ResetToBeginning();
					duelUI.versusTimer.GetComponent<TweenScale>().PlayForward();
				}
				duelUI.versusTimer.text = ((!(Mathf.Floor(goTimer) <= 0f)) ? num.ToString() : "GO!");
			}
			if (goTimer < 0f)
			{
				goTimer = 0f;
				if (isMaster)
				{
					lastTimer = -1;
					StartMatch();
				}
			}
		}
		if (gameStatus == GameStatus.OpponentConnected)
		{
			goTimer -= Time.deltaTime;
			if (goTimer < 0f)
			{
				goTimer = 0f;
				if (isMaster)
				{
					GoMatch();
				}
			}
		}
		if (enemyCharacter != null)
		{
			enemyCharacter.gameObject.SetActive(showEnemyCharacter);
		}
		if (myCharacter != null)
		{
			myCharacter.gameObject.SetActive(showMyCharacter);
		}
		if (isMaster && (gameStatus == GameStatus.Playing || gameStatus == GameStatus.DisconnectInMatch) && opponentNetworkTable == null)
		{
			if (InGameGUI.sharedInGameGUI != null)
			{
				InGameGUI.sharedInGameGUI.waitDuelLabel.SetActive(true);
			}
			gameStatus = GameStatus.WaitForPlayerBack;
			waitPlayerBackTime = 30f;
		}
		if (waitPlayerBackTime >= 0f && (gameStatus == GameStatus.WaitForPlayerBack || gameStatus == GameStatus.RoomClosed) && opponentNetworkTable != null)
		{
			if (InGameGUI.sharedInGameGUI != null)
			{
				InGameGUI.sharedInGameGUI.waitDuelLabel.SetActive(false);
			}
			gameStatus = GameStatus.Playing;
			if (isMaster)
			{
				ResumeMatch();
			}
		}
	}

	private CharacterInterface CreateCharacter(Transform transform)
	{
		GameObject gameObject = Resources.Load<GameObject>("Character_model");
		gameObject.SetActive(false);
		GameObject gameObject2 = Object.Instantiate(gameObject, transform.position, transform.rotation) as GameObject;
		CharacterInterface component = gameObject2.GetComponent<CharacterInterface>();
		component.usePetFromStorager = false;
		gameObject2.transform.SetParent(transform);
		gameObject.SetActive(true);
		component.useLightprobes = true;
		component.SetCharacterType(false, false, false);
		return component;
	}

	public void SetShopEvents()
	{
		ShopNGUIController.sharedShop.wearEquipAction = delegate(ShopNGUIController.CategoryNames category, string unequippedItem, string equippedItem)
		{
			SendMyWearInvisible();
			if (category == ShopNGUIController.CategoryNames.CapesCategory)
			{
				SetMyCape();
				SendMyCape();
			}
			if (category == ShopNGUIController.CategoryNames.MaskCategory)
			{
				SetMyMask();
				SendMyMask();
			}
			if (category == ShopNGUIController.CategoryNames.HatsCategory)
			{
				SetMyHat();
				SendMyHat();
			}
			if (category == ShopNGUIController.CategoryNames.BootsCategory)
			{
				SetMyBoots();
				SendMyBoots();
			}
			if (category == ShopNGUIController.CategoryNames.ArmorCategory)
			{
				SetMyArmor();
				SendMyArmor();
			}
		};
		ShopNGUIController.sharedShop.wearUnequipAction = delegate(ShopNGUIController.CategoryNames category, string unequippedItem)
		{
			SendMyWearInvisible();
			if (category == ShopNGUIController.CategoryNames.CapesCategory)
			{
				SetMyCape();
				SendMyCape();
			}
			if (category == ShopNGUIController.CategoryNames.MaskCategory)
			{
				SetMyMask();
				SendMyMask();
			}
			if (category == ShopNGUIController.CategoryNames.HatsCategory)
			{
				SetMyHat();
				SendMyHat();
			}
			if (category == ShopNGUIController.CategoryNames.BootsCategory)
			{
				SetMyBoots();
				SendMyBoots();
			}
			if (category == ShopNGUIController.CategoryNames.ArmorCategory)
			{
				SetMyArmor();
				SendMyArmor();
			}
		};
		ShopNGUIController.sharedShop.equipAction = delegate
		{
			SetMyWeapon();
			SendMyWeapon();
		};
		if (!equippedPetActionSet)
		{
			equippedPetActionSet = true;
			ShopNGUIController.EquippedPet += OnPetEquipAction;
			ShopNGUIController.UnequippedPet += OnPetUnequipAction;
		}
		ShopNGUIController.sharedShop.onEquipSkinAction = delegate
		{
			SetMySkin();
			SendMySkin();
		};
		ShopNGUIController.ShowWearChanged -= OnMyWearVisibleChanged;
		ShopNGUIController.ShowWearChanged += OnMyWearVisibleChanged;
		ShopNGUIController.ShowArmorChanged -= OnMyArmorVisibleChanged;
		ShopNGUIController.ShowArmorChanged += OnMyArmorVisibleChanged;
	}

	public void OnPetUnequipAction(string nowId)
	{
		SetMyPet();
		SendMyPet();
	}

	public void OnPetEquipAction(string nowId, string beforeID)
	{
		SetMyPet();
		SendMyPet();
	}

	private void OnMyWearVisibleChanged()
	{
		SetMyWearInvisible();
		if (Defs.isMulti)
		{
			SendMyWearInvisible();
		}
	}

	private void OnMyArmorVisibleChanged()
	{
		SetMyArmor();
		if (Defs.isMulti)
		{
			SendMyArmor();
		}
	}

	[PunRPC]
	private void SyncGameStatusRPC(int state)
	{
	}
}

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Rilisoft;
using UnityEngine;

public class BonusController : MonoBehaviour
{
	public enum TypeBonus
	{
		Ammo,
		Health,
		Armor,
		Chest,
		Grenade,
		Mech,
		JetPack,
		Invisible,
		Turret,
		Gem
	}

	public static BonusController sharedController;

	public GameObject bonusPrefab;

	public BonusItem[] bonusStack;

	private float creationInterval = 7f;

	public float timerToAddBonus;

	private bool isMulti;

	private bool isInet;

	private bool isStopCreateBonus;

	public bool isBeginCreateBonuses;

	private WeaponManager _weaponManager;

	private GameObject[] bonusCreationZones;

	private ZombieCreator zombieCreator;

	private PhotonView photonView;

	public int maxCountBonus = 5;

	private int activeBonusesCount;

	private int sumProbabilitys;

	private Dictionary<int, int> probabilityBonusDict = new Dictionary<int, int>();

	private Dictionary<int, Dictionary<string, int>> probabilityBonus = new Dictionary<int, Dictionary<string, int>>();

	private NetworkView _networkView { get; set; }

	private void InitStack()
	{
		bonusStack = new BonusItem[maxCountBonus + 6];
		for (int i = 0; i < bonusStack.Length; i++)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(bonusPrefab, Vector3.down * 100f, Quaternion.identity);
			gameObject.transform.parent = base.transform;
			bonusStack[i] = gameObject.GetComponent<BonusItem>();
		}
	}

	private void Awake()
	{
		if (sharedController == null)
		{
			sharedController = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (Defs.IsSurvival)
		{
			creationInterval = 9f;
		}
		else if (Defs.isDuel)
		{
			creationInterval = 15f;
		}
		timerToAddBonus = creationInterval;
		isMulti = Defs.isMulti;
		isInet = Defs.isInet;
		maxCountBonus = (Defs.isDuel ? 2 : ((!Defs.IsSurvival) ? 5 : 3));
	}

	private void Start()
	{
		photonView = PhotonView.Get(this);
		_networkView = GetComponent<NetworkView>();
		if ((bool)photonView)
		{
			PhotonObjectCacher.AddObject(base.gameObject);
		}
		bonusCreationZones = GameObject.FindGameObjectsWithTag("BonusCreationZone");
		if (maxCountBonus > bonusCreationZones.Length)
		{
			maxCountBonus = bonusCreationZones.Length;
		}
		zombieCreator = GameObject.FindGameObjectWithTag("GameController").GetComponent<ZombieCreator>();
		_weaponManager = WeaponManager.sharedManager;
		SetProbability();
		InitStack();
	}

	private void SetProbability()
	{
		probabilityBonusDict.Clear();
		probabilityBonus.Clear();
		sumProbabilitys = 0;
		if (Defs.isMulti)
		{
			if (Defs.isHunger)
			{
				probabilityBonusDict.Add(3, 100);
			}
			else if (SceneLoader.ActiveSceneName.Equals("Knife"))
			{
				probabilityBonusDict.Add(1, 75);
				probabilityBonusDict.Add(2, 25);
			}
			else if (Defs.isDaterRegim)
			{
				probabilityBonusDict.Add(0, 100);
			}
			else if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
			{
				probabilityBonusDict.Add(0, 100);
			}
			else if (Defs.isCOOP)
			{
				probabilityBonusDict.Add(0, 55);
				probabilityBonusDict.Add(1, 14);
				probabilityBonusDict.Add(2, 12);
			}
			else if (Defs.isDuel)
			{
				probabilityBonusDict.Add(0, 50);
				probabilityBonusDict.Add(1, 20);
				probabilityBonusDict.Add(2, 20);
			}
			else if (SceneLoader.ActiveSceneName.Equals("WalkingFortress"))
			{
				probabilityBonusDict.Add(0, 50);
				probabilityBonusDict.Add(1, 10);
				probabilityBonusDict.Add(2, 5);
			}
			else
			{
				probabilityBonusDict.Add(0, 50);
				probabilityBonusDict.Add(1, 10);
				probabilityBonusDict.Add(2, 10);
			}
		}
		else if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			probabilityBonusDict.Add(0, 100);
		}
		else
		{
			probabilityBonusDict.Add(0, 55);
			probabilityBonusDict.Add(1, 14);
			probabilityBonusDict.Add(2, 12);
		}
		foreach (KeyValuePair<int, int> item in probabilityBonusDict)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add("min", sumProbabilitys);
			sumProbabilitys += item.Value;
			dictionary.Add("max", sumProbabilitys);
			probabilityBonus.Add(item.Key, dictionary);
		}
	}

	public void AddWeaponAfterKillPlayer(string _weaponName, Vector3 _pos)
	{
		photonView.RPC("AddWeaponAfterKillPlayerRPC", PhotonTargets.MasterClient, _weaponName, _pos);
	}

	[RPC]
	[PunRPC]
	private void AddWeaponAfterKillPlayerRPC(string _weaponName, Vector3 _pos)
	{
		PhotonNetwork.InstantiateSceneObject("Weapon_Bonuses/" + _weaponName + "_Bonus", new Vector3(_pos.x, _pos.y - 0.5f, _pos.z), Quaternion.identity, 0, null);
	}

	public void AddBonusAfterKillPlayer(Vector3 _pos)
	{
		if (Defs.isInet)
		{
			photonView.RPC("AddBonusAfterKillPlayerRPC", PhotonTargets.MasterClient, _pos);
		}
		else
		{
			_networkView.RPC("AddBonusAfterKillPlayerRPC", RPCMode.Server, _pos);
		}
	}

	[RPC]
	[PunRPC]
	private void AddBonusAfterKillPlayerRPC(Vector3 _pos)
	{
		AddBonusAfterKillPlayerRPC(IndexBonusOnKill(), _pos);
	}

	[PunRPC]
	[RPC]
	private void AddBonusAfterKillPlayerRPC(int _type, Vector3 _pos)
	{
		if (Defs.isMulti)
		{
			if (Defs.isInet && PhotonNetwork.isMasterClient && !Defs.isHunger)
			{
				AddBonus(_pos, _type);
			}
			if (!Defs.isInet && Network.isServer)
			{
				AddBonus(_pos, _type);
			}
		}
		else
		{
			AddBonus(_pos, _type);
		}
	}

	private void AddBonus(Vector3 pos, int _type)
	{
		if (_type == 5 || _type == 8 || _type == 7 || _type == 6 || _type == 4)
		{
			return;
		}
		if (!isMulti)
		{
			int num = GlobalGameController.EnemiesToKill - zombieCreator.NumOfDeadZombies;
			if ((!Defs.IsSurvival && num <= 0 && !zombieCreator.bossShowm) || (Defs.IsSurvival && zombieCreator.stopGeneratingBonuses))
			{
				if (!Defs.IsSurvival)
				{
					isStopCreateBonus = true;
				}
				return;
			}
		}
		if (_type == 9)
		{
			if (!CanSpawnGemBonus())
			{
				return;
			}
			Hashtable hashtable = new Hashtable();
			hashtable["SpecialBonus"] = PhotonNetwork.time + 480.0;
			PhotonNetwork.room.SetCustomProperties(hashtable);
		}
		int num2 = -1;
		if (pos.Equals(Vector3.zero))
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Chest");
			if (activeBonusesCount + array.Length >= maxCountBonus)
			{
				return;
			}
			num2 = UnityEngine.Random.Range(0, bonusCreationZones.Length);
			bool[] array2 = new bool[bonusCreationZones.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = false;
			}
			for (int j = 0; j < bonusStack.Length; j++)
			{
				if (bonusStack[j].isActive && bonusStack[j].mySpawnNumber != -1)
				{
					array2[bonusStack[j].mySpawnNumber] = true;
				}
			}
			for (int k = 0; k < array.Length; k++)
			{
				if (array[k].GetComponent<SettingBonus>().numberSpawnZone != -1)
				{
					array2[array[k].GetComponent<SettingBonus>().numberSpawnZone] = true;
				}
			}
			while (array2[num2])
			{
				num2++;
				if (num2 == array2.Length)
				{
					num2 = 0;
				}
			}
			GameObject gameObject = bonusCreationZones[num2];
			BoxCollider component = gameObject.GetComponent<BoxCollider>();
			Vector2 vector = new Vector2(component.size.x * gameObject.transform.localScale.x, component.size.z * gameObject.transform.localScale.z);
			Rect rect = new Rect(gameObject.transform.position.x - vector.x / 2f, gameObject.transform.position.z - vector.y / 2f, vector.x, vector.y);
			pos = new Vector3(rect.x + UnityEngine.Random.Range(0f, rect.width), gameObject.transform.position.y, rect.y + UnityEngine.Random.Range(0f, rect.height));
		}
		if (_type != 3)
		{
			for (int l = 0; l < bonusStack.Length; l++)
			{
				if (bonusStack[l].isActive)
				{
					continue;
				}
				MakeBonusRPC(l, _type, pos, (num2 != -1) ? (-1f) : ((float)GetTimeForBonus()), num2);
				if (isMulti)
				{
					if (isInet)
					{
						photonView.RPC("MakeBonusRPC", PhotonTargets.Others, l, _type, pos, (num2 != -1) ? (-1f) : ((float)GetTimeForBonus()), num2);
					}
					else
					{
						_networkView.RPC("MakeBonusRPC", RPCMode.Others, l, _type, pos, (num2 != -1) ? (-1f) : ((float)GetTimeForBonus()), num2);
					}
				}
				break;
			}
		}
		else if (!isMulti || !isInet)
		{
			GameObject original = Resources.Load("Bonuses/Bonus_" + _type) as GameObject;
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(original, pos, Quaternion.identity);
			gameObject2.GetComponent<SettingBonus>().numberSpawnZone = num2;
		}
		else
		{
			GameObject gameObject2 = PhotonNetwork.InstantiateSceneObject("Bonuses/Bonus_" + ((_type == -1) ? IndexBonus() : _type), pos, Quaternion.identity, 0, null);
			gameObject2.GetComponent<SettingBonus>().SetNumberSpawnZone(num2);
		}
	}

	public void AddBonusForHunger(Vector3 pos, int _type, int spawnZoneIndex)
	{
		if (!Defs.isHunger)
		{
			return;
		}
		for (int i = 0; i < bonusStack.Length; i++)
		{
			if (bonusStack[i].isActive)
			{
				continue;
			}
			MakeBonusRPC(i, _type, pos, -1f, spawnZoneIndex);
			if (isMulti)
			{
				if (isInet)
				{
					photonView.RPC("MakeBonusRPC", PhotonTargets.Others, i, _type, pos, -1f, spawnZoneIndex);
				}
				else
				{
					_networkView.RPC("MakeBonusRPC", RPCMode.Others, i, _type, pos, -1f, spawnZoneIndex);
				}
			}
			break;
		}
	}

	public void RemoveBonus(int index)
	{
		RemoveBonusRPC(index);
		if (isMulti)
		{
			if (isInet)
			{
				photonView.RPC("RemoveBonusRPC", PhotonTargets.Others, index);
			}
			else
			{
				_networkView.RPC("RemoveBonusRPC", RPCMode.Others, index);
			}
		}
	}

	public void GetAndRemoveBonus(int index)
	{
		if (isMulti && isInet && !NetworkStartTable.LocalOrPasswordRoom())
		{
			RemoveBonusWithRewardRPC(PhotonNetwork.player, index);
			photonView.RPC("RemoveBonusWithRewardRPC", PhotonTargets.Others, PhotonNetwork.player, index);
		}
	}

	public void ClearBonuses()
	{
		for (int i = 0; i < bonusStack.Length; i++)
		{
			if (bonusStack[i].isActive)
			{
				RemoveBonusRPC(i);
			}
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		for (int i = 0; i < bonusStack.Length; i++)
		{
			if (bonusStack[i].isActive)
			{
				photonView.RPC("MakeBonusRPC", player, i, (int)bonusStack[i].type, bonusStack[i].transform.position, (float)bonusStack[i].expireTime, bonusStack[i].mySpawnNumber);
			}
		}
	}

	private void OnPlayerConnected(NetworkPlayer player)
	{
		if (!Network.isServer)
		{
			return;
		}
		for (int i = 0; i < bonusStack.Length; i++)
		{
			if (bonusStack[i].isActive)
			{
				_networkView.RPC("MakeBonusRPC", player, i, (int)bonusStack[i].type, bonusStack[i].transform.position, (float)bonusStack[i].expireTime, bonusStack[i].mySpawnNumber);
			}
		}
	}

	[RPC]
	[PunRPC]
	private void MakeBonusRPC(int index, int type, Vector3 position, float expireTime, int zoneNumber)
	{
		if (index < bonusStack.Length && !bonusStack[index].isActive)
		{
			bonusStack[index].ActivateBonus((TypeBonus)type, position, expireTime, zoneNumber, index);
			if (!bonusStack[index].isTimeBonus)
			{
				activeBonusesCount++;
			}
		}
	}

	private void PickupBonus(int index, PhotonPlayer player)
	{
		if (index < bonusStack.Length && bonusStack[index].isActive && !bonusStack[index].isPickedUp)
		{
			bonusStack[index].PickupBonus(player);
		}
	}

	[PunRPC]
	[RPC]
	private void RemoveBonusRPC(int index)
	{
		if (index < bonusStack.Length && bonusStack[index].isActive)
		{
			if (!bonusStack[index].isTimeBonus)
			{
				activeBonusesCount--;
			}
			bonusStack[index].DeactivateBonus();
		}
	}

	[PunRPC]
	[RPC]
	private void RemoveBonusWithRewardRPC(PhotonPlayer sender, int index)
	{
		if (isMulti && isInet && !NetworkStartTable.LocalOrPasswordRoom() && index < bonusStack.Length && bonusStack[index].isActive)
		{
			PickupBonus(index, sender);
		}
	}

	[RPC]
	[PunRPC]
	private void GetBonusRewardRPC(int index)
	{
		if (index >= bonusStack.Length || !bonusStack[index].isActive || !bonusStack[index].isPickedUp)
		{
			return;
		}
		if (bonusStack[index].playerPicked.Equals(PhotonNetwork.player))
		{
			TypeBonus type = bonusStack[index].type;
			if (type == TypeBonus.Gem)
			{
				BankController.AddGems(1);
			}
		}
		RemoveBonusRPC(index);
	}

	private double GetTimeForBonus()
	{
		double num = -1.0;
		if (Defs.isInet)
		{
			return PhotonNetwork.time + 15.0;
		}
		return Network.time + 15.0;
	}

	private bool CanSpawnGemBonus()
	{
		if (Defs.isHunger || !Defs.isMulti || !Defs.isInet || NetworkStartTable.LocalOrPasswordRoom())
		{
			return false;
		}
		if (PhotonNetwork.room == null || PhotonNetwork.room.customProperties["SpecialBonus"] == null || Convert.ToDouble(PhotonNetwork.room.customProperties["SpecialBonus"]) > PhotonNetwork.time)
		{
			return false;
		}
		return true;
	}

	private int IndexBonus()
	{
		int num = UnityEngine.Random.Range(0, sumProbabilitys);
		foreach (KeyValuePair<int, Dictionary<string, int>> probabilityBonu in probabilityBonus)
		{
			if (num >= probabilityBonu.Value["min"] && num < probabilityBonu.Value["max"])
			{
				return probabilityBonu.Key;
			}
		}
		return 0;
	}

	private int IndexBonusOnKill()
	{
		if (CanSpawnGemBonus() && UnityEngine.Random.Range(0, 100) < 5)
		{
			return 9;
		}
		int num = UnityEngine.Random.Range(0, sumProbabilitys);
		foreach (KeyValuePair<int, Dictionary<string, int>> probabilityBonu in probabilityBonus)
		{
			if (num >= probabilityBonu.Value["min"] && num < probabilityBonu.Value["max"])
			{
				return probabilityBonu.Key;
			}
		}
		return 0;
	}

	private void Update()
	{
		bool flag = false;
		flag = !isMulti || ((!isInet) ? Network.isServer : PhotonNetwork.isMasterClient);
		if (flag)
		{
			for (int i = 0; i < bonusStack.Length; i++)
			{
				if (bonusStack[i].isActive && bonusStack[i].isPickedUp)
				{
					photonView.RPC("GetBonusRewardRPC", PhotonTargets.All, i);
				}
			}
		}
		if (!isStopCreateBonus && flag)
		{
			timerToAddBonus -= Time.deltaTime;
		}
		if (timerToAddBonus < 0f)
		{
			timerToAddBonus = creationInterval;
			AddBonus(Vector3.zero, IndexBonus());
		}
	}

	private void OnDestroy()
	{
		if ((bool)photonView)
		{
			PhotonObjectCacher.RemoveObject(base.gameObject);
		}
	}
}

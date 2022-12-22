using System.Collections.Generic;
using Photon;
using UnityEngine;

public class WeaponBonus : Photon.MonoBehaviour
{
	public GameObject weaponPrefab;

	private GameObject _player;

	private Player_move_c _playerMoveC;

	private bool runLoading;

	private bool oldIsMaster;

	public WeaponManager _weaponManager;

	private bool isHunger;

	public bool isKilled;

	private void Start()
	{
		string text = base.gameObject.name.Substring(0, base.gameObject.name.Length - 13);
		weaponPrefab = Resources.Load<GameObject>("Weapons/" + text);
		_weaponManager = WeaponManager.sharedManager;
		isHunger = Defs.isHunger;
		if (!isHunger)
		{
			_player = GameObject.FindGameObjectWithTag("Player");
			GameObject gameObject = GameObject.FindGameObjectWithTag("PlayerGun");
			if (gameObject != null)
			{
				_playerMoveC = gameObject.GetComponent<Player_move_c>();
			}
			else
			{
				Debug.LogWarning("WeaponBonus.Start(): playerGun == null");
			}
		}
		else
		{
			_player = _weaponManager.myPlayer;
			if (_player != null)
			{
				GameObject playerGameObject = _player.GetComponent<SkinName>().playerGameObject;
				if (playerGameObject != null)
				{
					_playerMoveC = playerGameObject.GetComponent<Player_move_c>();
				}
				else
				{
					Debug.LogWarning("WeaponBonus.Start(): playerGo == null");
				}
			}
		}
		if (!Defs.IsSurvival && !isHunger)
		{
			GameObject gameObject2 = Object.Instantiate(Resources.Load("BonusFX"), Vector3.zero, Quaternion.identity) as GameObject;
			gameObject2.transform.parent = base.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.layer = base.gameObject.layer;
			ZombieCreator.SetLayerRecursively(gameObject2, base.gameObject.layer);
		}
	}

	private void Update()
	{
		if (!oldIsMaster && PhotonNetwork.isMasterClient && isKilled)
		{
			PhotonNetwork.Destroy(base.gameObject);
			return;
		}
		oldIsMaster = PhotonNetwork.isMasterClient;
		float num = 120f;
		base.transform.Rotate(base.transform.InverseTransformDirection(Vector3.up), num * Time.deltaTime);
		if (runLoading)
		{
			return;
		}
		if (isHunger && (_player == null || _playerMoveC == null))
		{
			_player = _weaponManager.myPlayer;
			if (!(_player != null))
			{
				return;
			}
			_playerMoveC = _player.GetComponent<SkinName>().playerGameObject.GetComponent<Player_move_c>();
		}
		if (_playerMoveC == null || _playerMoveC.isGrenadePress || isKilled || _playerMoveC.isKilled || !(Vector3.SqrMagnitude(base.transform.position - _player.transform.position) < 2.25f))
		{
			return;
		}
		_playerMoveC.AddWeapon(weaponPrefab);
		isKilled = true;
		if (Defs.IsSurvival || (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None) || isHunger)
		{
			if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
			{
				TrainingController.isNextStep = TrainingState.GetTheGun;
			}
			if (!isHunger)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				base.photonView.RPC("DestroyRPC", PhotonTargets.AllBuffered);
			}
			return;
		}
		string[] array = Storager.getString(Defs.WeaponsGotInCampaign, false).Split('#');
		List<string> list = new List<string>();
		string[] array2 = array;
		foreach (string item in array2)
		{
			list.Add(item);
		}
		if (!list.Contains(LevelBox.weaponsFromBosses[Application.loadedLevelName]))
		{
			list.Add(LevelBox.weaponsFromBosses[Application.loadedLevelName]);
			Storager.setString(Defs.WeaponsGotInCampaign, string.Join("#", list.ToArray()), false);
		}
		Object.Destroy(base.gameObject);
		runLoading = true;
		LevelCompleteLoader.action = null;
		LevelCompleteLoader.sceneName = "LevelComplete";
		AutoFade.LoadLevel("LevelToCompleteProm", 2f, 0f, Color.white);
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.SetEnablePerfectLabel(true);
		}
		GameObject gameObject = Object.Instantiate(Resources.Load("PauseONGuiDrawer") as GameObject);
		gameObject.transform.parent = base.transform;
	}

	[RPC]
	[PunRPC]
	public void DestroyRPC()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		else
		{
			base.transform.position = new Vector3(0f, -10000f, 0f);
		}
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.SetEnablePerfectLabel(false);
		}
	}

	private void OnDestroy()
	{
		if (!Defs.IsSurvival && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != 0) && !isHunger)
		{
		}
	}
}

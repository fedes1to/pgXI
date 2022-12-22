using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public class BasePointController : MonoBehaviour
{
	public enum TypeCapture
	{
		none,
		blue,
		red
	}

	private const float timeCaptureCounter = 5f;

	private const float maxCaptureCounter = 100f;

	public string nameBase;

	private float _captureCounter;

	private Color redTeamColor = Color.red;

	private Color blueTeamColor = Color.blue;

	private Color redCaptureColor = new Color32(212, 0, 0, 130);

	private Color blueCaptureColor = new Color32(0, 0, 225, 130);

	public NickLabelController myLabelController;

	public PhotonView photonView;

	private bool isStartUpdateFromMasterClient;

	private float periodToSynch = 1f;

	private float timerToSynch;

	public List<Player_move_c> capturePlayers = new List<Player_move_c>();

	public bool isBaseActive = true;

	private bool myPlayerOnPoint;

	public GameObject baseRender;

	public LineRenderer rayPoint;

	private bool isSendMessageCaptureBlue;

	private bool isSendMessageCaptureRed;

	private bool isSendMessageRaiderBlue;

	private bool isSendMessageRaiderRed;

	private bool sendScoreEventBlue;

	private bool sendScoreEventRed;

	public TypeCapture captureConmmand;

	public float captureCounter
	{
		get
		{
			return _captureCounter;
		}
		set
		{
			_captureCounter = value;
		}
	}

	private void Awake()
	{
		photonView = GetComponent<PhotonView>();
		timerToSynch = periodToSynch;
		PhotonObjectCacher.AddObject(base.gameObject);
	}

	private void OnDestroy()
	{
		PhotonObjectCacher.RemoveObject(base.gameObject);
	}

	private void Start()
	{
		rayPoint.SetColors(Color.gray, Color.gray);
		myLabelController = NickLabelStack.sharedStack.GetNextCurrentLabel();
		myLabelController.StartShow(NickLabelController.TypeNickLabel.Point, base.transform);
	}

	private void Update()
	{
		if (isStartUpdateFromMasterClient && !PhotonNetwork.connected)
		{
			isStartUpdateFromMasterClient = false;
		}
		int num = 0;
		int num2 = 0;
		bool flag = false;
		if (isBaseActive)
		{
			for (int i = 0; i < capturePlayers.Count; i++)
			{
				if (capturePlayers[i].myCommand == 1)
				{
					num++;
				}
				else if (capturePlayers[i].myCommand == 2)
				{
					num2++;
				}
				if (capturePlayers[i].Equals(WeaponManager.sharedManager.myPlayerMoveC))
				{
					flag = true;
				}
			}
			if (num2 == 0 && num > 0)
			{
				float num3;
				switch (num)
				{
				case 1:
					num3 = 1f;
					break;
				case 2:
					num3 = 1.1f;
					break;
				case 3:
					num3 = 1.2f;
					break;
				default:
					num3 = 1.3f;
					break;
				}
				float num4 = num3;
				captureCounter += Time.deltaTime * num4 * 100f / 5f;
				if (captureCounter > 100f)
				{
					captureCounter = 100f;
				}
			}
			if (num == 0 && num2 > 0)
			{
				float num5;
				switch (num2)
				{
				case 1:
					num5 = 1f;
					break;
				case 2:
					num5 = 1.1f;
					break;
				case 3:
					num5 = 1.2f;
					break;
				default:
					num5 = 1.3f;
					break;
				}
				float num6 = num5;
				captureCounter -= Time.deltaTime * num6 * 100f / 5f;
				if (captureCounter < -100f)
				{
					captureCounter = -100f;
				}
			}
		}
		if (WeaponManager.sharedManager.myNetworkStartTable != null)
		{
			Color color = ((WeaponManager.sharedManager.myNetworkStartTable.myCommand == 0) ? Color.gray : (((!(captureCounter > 0f) || WeaponManager.sharedManager.myNetworkStartTable.myCommand != 1) && (!(captureCounter < 0f) || WeaponManager.sharedManager.myNetworkStartTable.myCommand != 2)) ? Color.red : Color.blue));
			myLabelController.pointFillSprite.color = color;
		}
		myLabelController.pointFillSprite.fillAmount = Mathf.Abs(captureCounter) / 100f;
		if (captureCounter > 0f && captureConmmand == TypeCapture.red && !isSendMessageRaiderBlue)
		{
			isSendMessageRaiderBlue = true;
			isSendMessageRaiderRed = false;
			if (WeaponManager.sharedManager.myPlayerMoveC != null)
			{
				if (WeaponManager.sharedManager.myPlayerMoveC.myCommand == 1)
				{
					WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(LocalizationStore.Get("Key_1783") + " " + nameBase);
				}
				else
				{
					WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(LocalizationStore.Get("Key_1784") + " " + nameBase);
				}
			}
		}
		if (captureCounter < 0f && captureConmmand == TypeCapture.blue && !isSendMessageRaiderRed)
		{
			if (WeaponManager.sharedManager.myPlayerMoveC != null)
			{
				if (WeaponManager.sharedManager.myPlayerMoveC.myCommand == 2)
				{
					WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(LocalizationStore.Get("Key_1783") + " " + nameBase);
				}
				else
				{
					WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(LocalizationStore.Get("Key_1784") + " " + nameBase);
				}
			}
			isSendMessageRaiderBlue = false;
			isSendMessageRaiderRed = true;
		}
		if (captureCounter > 99.9f)
		{
			if (captureConmmand != TypeCapture.blue)
			{
				photonView.RPC("SinchCapture", PhotonTargets.Others, 1);
			}
			if (WeaponManager.sharedManager.myPlayerMoveC != null && !isSendMessageCaptureBlue)
			{
				isSendMessageCaptureRed = false;
				isSendMessageCaptureBlue = true;
				if (WeaponManager.sharedManager.myPlayerMoveC.myCommand == 1)
				{
					WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(LocalizationStore.Get("Key_1781") + " " + nameBase);
				}
				else
				{
					WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(LocalizationStore.Get("Key_1785") + " " + nameBase);
				}
				if (WeaponManager.sharedManager.myPlayerMoveC.myCommand == 1 && flag)
				{
					QuestMediator.NotifyCapture(ConnectSceneNGUIController.RegimGame.CapturePoints);
					if (num == 1)
					{
						WeaponManager.sharedManager.myPlayerMoveC.SendMySpotEvent();
					}
					else
					{
						WeaponManager.sharedManager.myPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.teamCapturePoint);
					}
				}
			}
			if (WeaponManager.sharedManager.myNetworkStartTable != null)
			{
				if (WeaponManager.sharedManager.myNetworkStartTable.myCommand == 1)
				{
					rayPoint.SetColors(Color.blue, Color.blue);
				}
				else
				{
					rayPoint.SetColors(Color.red, Color.red);
				}
			}
			else
			{
				rayPoint.SetColors(Color.white, Color.white);
			}
			captureConmmand = TypeCapture.blue;
		}
		if (captureCounter < -99.9f)
		{
			if (captureConmmand != TypeCapture.red)
			{
				photonView.RPC("SinchCapture", PhotonTargets.Others, 2);
			}
			if (WeaponManager.sharedManager.myPlayerMoveC != null && !isSendMessageCaptureRed)
			{
				isSendMessageCaptureRed = true;
				isSendMessageCaptureBlue = false;
				if (WeaponManager.sharedManager.myPlayerMoveC.myCommand == 2)
				{
					WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(LocalizationStore.Get("Key_1781") + " " + nameBase);
				}
				else
				{
					WeaponManager.sharedManager.myPlayerMoveC.AddSystemMessage(LocalizationStore.Get("Key_1785") + " " + nameBase);
				}
				if (WeaponManager.sharedManager.myPlayerMoveC.myCommand == 2 && flag)
				{
					QuestMediator.NotifyCapture(ConnectSceneNGUIController.RegimGame.CapturePoints);
					if (num2 == 1)
					{
						WeaponManager.sharedManager.myPlayerMoveC.SendMySpotEvent();
					}
					else
					{
						WeaponManager.sharedManager.myPlayerMoveC.myScoreController.AddScoreOnEvent(PlayerEventScoreController.ScoreEvent.teamCapturePoint);
					}
				}
			}
			if (WeaponManager.sharedManager.myNetworkStartTable != null)
			{
				if (WeaponManager.sharedManager.myNetworkStartTable.myCommand == 2)
				{
					rayPoint.SetColors(Color.blue, Color.blue);
				}
				else
				{
					rayPoint.SetColors(Color.red, Color.red);
				}
			}
			else
			{
				rayPoint.SetColors(Color.white, Color.white);
			}
			captureConmmand = TypeCapture.red;
		}
		if (myPlayerOnPoint && InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.pointCaptureBar.SetActive(isBaseActive);
			InGameGUI.sharedInGameGUI.pointCaptureName.text = nameBase;
			InGameGUI.sharedInGameGUI.captureBarSprite.fillAmount = Mathf.Abs(captureCounter) / 100f;
			bool flag2 = (captureCounter > 0f && WeaponManager.sharedManager.myNetworkStartTable.myCommand == 1) || (captureCounter < 0f && WeaponManager.sharedManager.myNetworkStartTable.myCommand == 2);
			InGameGUI.sharedInGameGUI.captureBarSprite.color = ((!flag2) ? redTeamColor : blueTeamColor);
			InGameGUI.sharedInGameGUI.teamColorSprite.color = ((!flag2) ? redCaptureColor : blueCaptureColor);
		}
		if (PhotonNetwork.isMasterClient)
		{
			timerToSynch -= Time.deltaTime;
			if (timerToSynch <= 0f)
			{
				timerToSynch = periodToSynch;
				photonView.RPC("SynchCaptureCounter", PhotonTargets.Others, captureCounter);
			}
		}
	}

	private void MyPlayerEnterPoint()
	{
		myPlayerOnPoint = true;
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.pointCaptureBar.SetActive(isBaseActive);
		}
	}

	private void MyPlayerLeavePoint()
	{
		myPlayerOnPoint = false;
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.pointCaptureBar.SetActive(false);
		}
	}

	private void AddPlayerInList(GameObject _player)
	{
		Player_move_c playerMoveC = _player.GetComponent<SkinName>().playerMoveC;
		if (!capturePlayers.Contains(playerMoveC))
		{
			capturePlayers.Add(playerMoveC);
		}
	}

	private void RemoveFromList(GameObject _player)
	{
		Player_move_c playerMoveC = _player.GetComponent<SkinName>().playerMoveC;
		if (capturePlayers.Contains(playerMoveC))
		{
			capturePlayers.Remove(playerMoveC);
		}
	}

	[PunRPC]
	[RPC]
	public void SinchCapture(int command)
	{
		if (command == 1)
		{
			captureCounter = 200f;
			captureConmmand = TypeCapture.blue;
		}
		else
		{
			captureCounter = -200f;
			captureConmmand = TypeCapture.red;
		}
	}

	[PunRPC]
	[RPC]
	private void AddPlayerInCapturePoint(int _viewId, float _time)
	{
		for (int i = 0; i < Initializer.players.Count; i++)
		{
			if (Initializer.players[i].photonView.ownerId != _viewId)
			{
				continue;
			}
			if (capturePlayers.Contains(Initializer.players[i]))
			{
				break;
			}
			capturePlayers.Add(Initializer.players[i]);
			if (!isBaseActive || !(PhotonNetwork.time > (double)_time))
			{
				break;
			}
			int num = 0;
			int num2 = 0;
			for (int j = 0; j < capturePlayers.Count; j++)
			{
				if (capturePlayers[j].myCommand == 1)
				{
					num++;
				}
				else if (capturePlayers[j].myCommand == 2)
				{
					num2++;
				}
			}
			if (num2 == 0 && num == 1)
			{
				captureCounter += ((float)PhotonNetwork.time - _time) * 100f / 5f;
				if (captureCounter > 100f)
				{
					captureCounter = 100f;
				}
			}
			if (num == 0 && num2 == 1)
			{
				captureCounter -= ((float)PhotonNetwork.time - _time) * 100f / 5f;
				if (captureCounter < -100f)
				{
					captureCounter = -100f;
				}
			}
			break;
		}
	}

	[PunRPC]
	[RPC]
	private void RemovePlayerInCapturePoint(int _viewId, float _time)
	{
		for (int i = 0; i < Initializer.players.Count; i++)
		{
			if (Initializer.players[i].photonView.ownerId == _viewId)
			{
				if (capturePlayers.Contains(Initializer.players[i]))
				{
					capturePlayers.Remove(Initializer.players[i]);
				}
				break;
			}
		}
	}

	public void OnEndMatch()
	{
		captureCounter = 0f;
		capturePlayers.Clear();
		rayPoint.SetColors(Color.gray, Color.gray);
		captureConmmand = TypeCapture.none;
		isSendMessageCaptureRed = false;
		isSendMessageCaptureBlue = false;
		isSendMessageRaiderBlue = false;
		isSendMessageRaiderRed = false;
		MyPlayerLeavePoint();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (Defs.isMulti && Defs.isCapturePoints && other.transform.parent != null && other.transform.parent.gameObject.CompareTag("Player"))
		{
			AddPlayerInList(other.transform.parent.gameObject);
			if (other.transform.parent.gameObject.Equals(WeaponManager.sharedManager.myPlayer))
			{
				MyPlayerEnterPoint();
				photonView.RPC("AddPlayerInCapturePoint", PhotonTargets.Others, WeaponManager.sharedManager.myPlayerMoveC.photonView.ownerId, (float)PhotonNetwork.time);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (Defs.isMulti && Defs.isCapturePoints && other.transform.parent != null && other.transform.parent.gameObject.CompareTag("Player"))
		{
			RemoveFromList(other.transform.parent.gameObject);
			if (other.transform.parent.gameObject.Equals(WeaponManager.sharedManager.myPlayer))
			{
				MyPlayerLeavePoint();
				photonView.RPC("RemovePlayerInCapturePoint", PhotonTargets.Others, WeaponManager.sharedManager.myPlayerMoveC.photonView.ownerId, (float)PhotonNetwork.time);
			}
		}
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (Defs.isMulti && Defs.isCapturePoints)
		{
			photonView.RPC("SynchCaptureCounterNewPlayer", player, player.ID, PhotonNetwork.isMasterClient, captureCounter, (int)captureConmmand);
		}
	}

	[PunRPC]
	[RPC]
	public void SynchCaptureCounterNewPlayer(int _viewId, bool isMaster, float _captureCounter, int _captureCommand)
	{
		if (!isStartUpdateFromMasterClient && PhotonNetwork.player.ID == _viewId)
		{
			SynchCaptureCounter(captureCounter);
			captureConmmand = (TypeCapture)_captureCommand;
			isStartUpdateFromMasterClient = isMaster;
		}
	}

	[RPC]
	[PunRPC]
	private void SynchCaptureCounter(float _captureCounter)
	{
		captureCounter = _captureCounter;
	}

	public void OnDisconnectedFromPhoton()
	{
		isStartUpdateFromMasterClient = false;
	}

	private void OnFailedToConnectToPhoton(object parameters)
	{
		OnEndMatch();
	}
}

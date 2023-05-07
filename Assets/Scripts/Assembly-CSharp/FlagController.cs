using UnityEngine;

public class FlagController : MonoBehaviour
{
	public bool isBlue;

	public int masterServerID;

	private PhotonView photonView;

	public bool isCapture;

	private int idCapturePlayer;

	public bool isBaza;

	private GameObject myBaza;

	public GameObject rayBlue;

	public GameObject rayRed;

	public float timerToBaza = 10f;

	private float maxTimerToBaza = 10f;

	public GameObject flagModelRed;

	public GameObject flagModelBlue;

	public Transform targetTrasform;

	private InGameGUI inGameGui;

	public GameObject pointObjTexture;

	private GameObject _objBazaTexture;

	private GameObject _objFlagTexture;

	public GameObject flagModel;

	private FlagPedestalController pedistal;

	private int currentColor = -1;

	private void Awake()
	{
		GameObject original = Resources.Load("FlagPedestal") as GameObject;
		if (isBlue)
		{
			myBaza = GameObject.FindGameObjectWithTag("BazaZoneCommand1");
			Initializer.flag1 = this;
		}
		else
		{
			myBaza = GameObject.FindGameObjectWithTag("BazaZoneCommand2");
			Initializer.flag2 = this;
		}
		GameObject gameObject = Object.Instantiate(original, myBaza.transform.position, myBaza.transform.rotation) as GameObject;
		pedistal = gameObject.GetComponent<FlagPedestalController>();
		_objBazaTexture = Object.Instantiate(Resources.Load("ObjectPictFlag") as GameObject, myBaza.transform.position, myBaza.transform.rotation) as GameObject;
		_objFlagTexture = Object.Instantiate(Resources.Load("ObjectPictFlag") as GameObject, myBaza.transform.position, myBaza.transform.rotation) as GameObject;
		_objBazaTexture.GetComponent<ObjectPictFlag>().target = gameObject.transform.GetChild(0);
		_objBazaTexture.GetComponent<ObjectPictFlag>().isBaza = true;
		_objBazaTexture.GetComponent<ObjectPictFlag>().myFlagController = this;
		_objFlagTexture.GetComponent<ObjectPictFlag>().target = pointObjTexture.transform;
		SetColor(0);
		PhotonObjectCacher.AddObject(base.gameObject);
	}

	private void Start()
	{
		photonView = GetComponent<PhotonView>();
		photonView.RPC("SetMasterSeverIDRPC", PhotonTargets.AllBuffered, photonView.viewID);
	}

	public void SetColor(int _color)
	{
		if (_color != currentColor)
		{
			currentColor = _color;
			pedistal.SetColor(_color);
			flagModelRed.SetActive(_color == 2);
			flagModelBlue.SetActive(_color == 1);
			rayRed.SetActive(_color == 2);
			rayBlue.SetActive(_color == 1);
			if (_color > 0)
			{
				_objBazaTexture.GetComponent<ObjectPictFlag>().SetTexture(Resources.Load((_color != 1) ? "red_base" : "blue_base") as Texture2D);
				_objFlagTexture.GetComponent<ObjectPictFlag>().SetTexture(Resources.Load((_color != 1) ? "red_flag" : "blue_flag") as Texture2D);
			}
			else
			{
				_objBazaTexture.GetComponent<ObjectPictFlag>().SetTexture(null);
				_objFlagTexture.GetComponent<ObjectPictFlag>().SetTexture(null);
			}
		}
	}

	private void Update()
	{
		if (inGameGui == null)
		{
			inGameGui = InGameGUI.sharedInGameGUI;
		}
		SetColor((!(WeaponManager.sharedManager.myPlayerMoveC == null)) ? (((WeaponManager.sharedManager.myPlayerMoveC.myCommand == 1 && isBlue) || (WeaponManager.sharedManager.myPlayerMoveC.myCommand == 2 && !isBlue)) ? 1 : 2) : 0);
		if (rayBlue.activeInHierarchy == isCapture)
		{
			rayBlue.SetActive(!isCapture);
		}
		if (rayRed.activeInHierarchy == isCapture)
		{
			rayRed.SetActive(!isCapture);
		}
		if (targetTrasform != null)
		{
			base.transform.position = targetTrasform.position;
			base.transform.rotation = targetTrasform.rotation;
		}
		else
		{
			isCapture = false;
		}
		int num = 0;
		int num2 = 0;
		foreach (Player_move_c player in Initializer.players)
		{
			if (player != null)
			{
				int myCommand = player.myCommand;
				if (myCommand == 1)
				{
					num++;
				}
				if (myCommand == 2)
				{
					num2++;
				}
			}
		}
		if ((num == 0 || num2 == 0) && flagModel.activeSelf)
		{
			flagModel.SetActive(false);
		}
		if (inGameGui != null && (num == 0 || num2 == 0) && !inGameGui.message_wait.activeSelf)
		{
			inGameGui.message_wait.SetActive(true);
			inGameGui.timerShowNow = 0f;
		}
		if (inGameGui != null && num != 0 && num2 != 0 && inGameGui.message_wait.activeSelf)
		{
			inGameGui.message_wait.SetActive(false);
			inGameGui.timerShowNow = 3f;
		}
		if (num != 0 && num2 != 0 && !flagModel.activeSelf)
		{
			flagModel.SetActive(true);
		}
		if ((num == 0 || num2 == 0) && isCapture)
		{
			foreach (Player_move_c player2 in Initializer.players)
			{
				if (idCapturePlayer == player2.mySkinName.photonView.ownerId)
				{
					player2.isCaptureFlag = false;
				}
				GoBaza();
			}
		}
		if (!PhotonNetwork.isMasterClient || isCapture || isBaza)
		{
			return;
		}
		timerToBaza -= Time.deltaTime;
		if (timerToBaza < 0f)
		{
			GoBaza();
			if (WeaponManager.sharedManager.myPlayer != null)
			{
				WeaponManager.sharedManager.myPlayerMoveC.SendSystemMessegeFromFlagReturned(isBlue);
			}
		}
	}

	public void GoBaza()
	{
		timerToBaza = maxTimerToBaza;
		photonView.RPC("GoBazaRPC", PhotonTargets.All);
	}

	[PunRPC]
	[RPC]
	public void GoBazaRPC()
	{
		Debug.Log("GoBazaRPC");
		isBaza = true;
		isCapture = false;
		idCapturePlayer = -1;
		targetTrasform = null;
		base.transform.position = myBaza.transform.position;
		base.transform.rotation = myBaza.transform.rotation;
	}

	public void SetCapture(int _viewIdCapture)
	{
		photonView.RPC("SetCaptureRPC", PhotonTargets.All, _viewIdCapture);
	}

	[RPC]
	[PunRPC]
	public void SetCaptureRPC(int _viewIdCapture)
	{
		isBaza = false;
		idCapturePlayer = _viewIdCapture;
		isCapture = true;
		foreach (Player_move_c player in Initializer.players)
		{
			if (player.mySkinName.photonView.ownerId == _viewIdCapture)
			{
				targetTrasform = player.flagPoint.transform;
				player.isCaptureFlag = true;
			}
		}
	}

	public void SetNOCapture(Vector3 pos, Quaternion rot)
	{
		photonView.RPC("SetNOCaptureRPC", PhotonTargets.All, pos, rot);
		timerToBaza = maxTimerToBaza;
	}

	[RPC]
	[PunRPC]
	public void SetNOCaptureRPC(Vector3 pos, Quaternion rot)
	{
		isCapture = false;
		idCapturePlayer = -1;
		if (targetTrasform != null)
		{
			targetTrasform.parent.GetComponent<SkinName>().playerMoveC.isCaptureFlag = false;
		}
		targetTrasform = null;
	}

	[PunRPC]
	[RPC]
	public void SetNOCaptureRPCNewPlayer(int idNewPlayer, Vector3 pos, Quaternion rot, bool _isBaza)
	{
		if (photonView == null)
		{
			photonView = GetComponent<PhotonView>();
		}
		if (photonView != null && photonView.ownerId == idNewPlayer)
		{
			isBaza = _isBaza;
			SetNOCaptureRPC(pos, rot);
		}
	}

	[PunRPC]
	[RPC]
	public void SetCaptureRPCNewPlayer(int idNewPlayer, int _viewIdCapture)
	{
		if (photonView == null)
		{
			photonView = GetComponent<PhotonView>();
		}
		if (PhotonNetwork.player.ID == idNewPlayer)
		{
			SetCaptureRPC(_viewIdCapture);
		}
	}

	[RPC]
	[PunRPC]
	public void SetMasterSeverIDRPC(int _id)
	{
		masterServerID = _id;
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (photonView == null)
		{
			Debug.Log("FlagController.OnPhotonPlayerConnected():    photonView == null");
		}
		else if (isCapture)
		{
			photonView.RPC("SetCaptureRPCNewPlayer", player, player.ID, idCapturePlayer);
		}
		else
		{
			photonView.RPC("SetNOCaptureRPCNewPlayer", player, player.ID, base.transform.position, base.transform.rotation, isBaza);
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(_objBazaTexture);
		Object.Destroy(_objFlagTexture);
		PhotonObjectCacher.RemoveObject(base.gameObject);
	}
}

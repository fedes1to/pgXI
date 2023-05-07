using Photon;
using UnityEngine;

internal sealed class HungerGameController : Photon.MonoBehaviour
{
	public bool isStartGame;

	public bool isStartTimer;

	public float startTimer = 30f;

	public int countPlayers;

	public int maxCountPlayers = 10;

	public bool isRunPlayer;

	public float goTimer = 10.5f;

	public bool isGo;

	private float timeToSynchTimer = 2f;

	public int minCountPlayer = 2;

	public bool isShowGo;

	private float timerShowGo = 1f;

	public float gameTimer = 600f;

	public bool theEnd;

	private static HungerGameController _instance;

	public static HungerGameController Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Start()
	{
		maxCountPlayers = PhotonNetwork.room.maxPlayers;
		gameTimer = int.Parse(PhotonNetwork.room.customProperties[ConnectSceneNGUIController.maxKillProperty].ToString()) * 60;
		_instance = this;
	}

	private void OnDestroy()
	{
		_instance = null;
	}

	private void Update()
	{
		if (isStartTimer && startTimer > 0f)
		{
			startTimer -= Time.deltaTime;
		}
		if (isStartGame && goTimer > 0f)
		{
			goTimer -= Time.deltaTime;
		}
		if (goTimer < 0f)
		{
			goTimer = 0f;
		}
		if (isShowGo && timerShowGo >= 0f)
		{
			timerShowGo -= Time.deltaTime;
		}
		if (isShowGo && timerShowGo < 0f)
		{
			isShowGo = false;
		}
		if (isGo && gameTimer > 0f && Initializer.players.Count > 0)
		{
			gameTimer -= Time.deltaTime;
		}
		if (!base.photonView.isMine)
		{
			return;
		}
		if (gameTimer <= 0f && !theEnd)
		{
			theEnd = true;
			base.photonView.RPC("Draw", PhotonTargets.AllBuffered);
		}
		timeToSynchTimer -= Time.deltaTime;
		if (isGo && timeToSynchTimer < 0f)
		{
			timeToSynchTimer = 0.5f;
			base.photonView.RPC("SynchGameTimer", PhotonTargets.Others, gameTimer);
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("NetworkTable");
		if (!isStartGame)
		{
			if (!isStartTimer && array.Length >= minCountPlayer)
			{
				base.photonView.RPC("StartTimer", PhotonTargets.AllBuffered, true);
			}
			if (timeToSynchTimer < 0f)
			{
				timeToSynchTimer = 0.5f;
				base.photonView.RPC("SynchStartTimer", PhotonTargets.Others, startTimer);
			}
			if ((!isStartGame && isStartTimer && startTimer < 0.1f && array.Length >= minCountPlayer) || (!isStartGame && isStartTimer && array.Length == PhotonNetwork.room.maxPlayers))
			{
				base.photonView.RPC("StartGame", PhotonTargets.AllBuffered);
				PhotonNetwork.room.visible = false;
			}
		}
		else
		{
			if (timeToSynchTimer < 0f)
			{
				timeToSynchTimer = 0.5f;
				base.photonView.RPC("SynchTimerGo", PhotonTargets.Others, goTimer);
			}
			if (!isGo && goTimer < 0.1f)
			{
				base.photonView.RPC("Go", PhotonTargets.AllBuffered);
			}
		}
	}

	[PunRPC]
	[RPC]
	private void Draw()
	{
		Debug.Log("Draw!!!");
		NetworkStartTable myNetworkStartTable = WeaponManager.sharedManager.myNetworkStartTable;
		if (myNetworkStartTable != null)
		{
			StartCoroutine(myNetworkStartTable.DrawInHanger());
		}
	}

	[RPC]
	[PunRPC]
	private void StartTimer(bool _isStartTimer)
	{
		isStartTimer = _isStartTimer;
	}

	[PunRPC]
	[RPC]
	private void SynchStartTimer(float _startTimer)
	{
		startTimer = _startTimer;
	}

	[RPC]
	[PunRPC]
	private void SynchTimerGo(float _goTimer)
	{
		goTimer = _goTimer;
	}

	[PunRPC]
	[RPC]
	private void SynchGameTimer(float _gameTimer)
	{
		gameTimer = _gameTimer;
	}

	[RPC]
	[PunRPC]
	private void StartGame()
	{
		isStartGame = true;
	}

	[PunRPC]
	[RPC]
	private void Go()
	{
		isGo = true;
		isShowGo = true;
	}
}

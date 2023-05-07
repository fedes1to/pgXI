using System;
using System.Collections;
using System.Reflection;
using ExitGames.Client.Photon;
using Rilisoft;
using UnityEngine;

public class TimeGameController : MonoBehaviour
{
	public static TimeGameController sharedController;

	public double timeEndMatch;

	public double timerToEndMatch;

	public double networkTime;

	public PhotonView photonView;

	public double timeLocalServer;

	public string ipServera;

	private long pauseTime;

	private bool paused;

	private bool wasPaused;

	public bool isEndMatch = true;

	private bool matchEnding;

	private int matchEndingPos;

	private int writtedMatchEndingPos;

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!PhotonNetwork.connected)
		{
			return;
		}
		if (pauseStatus)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer && !Defs.isDuel)
			{
				paused = true;
				wasPaused = true;
				PhotonNetwork.isMessageQueueRunning = false;
			}
			else
			{
				PhotonNetwork.isMessageQueueRunning = true;
				PhotonNetwork.Disconnect();
			}
		}
		else
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer && !Defs.isDuel)
			{
				CheckPause();
			}
			PhotonNetwork.FetchServerTimestamp();
		}
	}

	private void Awake()
	{
		if (!Defs.isMulti || Defs.isHunger || Defs.isDuel)
		{
			base.enabled = false;
		}
		else
		{
			StartCoroutine(FetchServerTimestamp());
		}
	}

	private IEnumerator FetchServerTimestamp()
	{
		while (true)
		{
			PhotonNetwork.FetchServerTimestamp();
			yield return new WaitForRealSeconds(60f);
		}
	}

	private void Start()
	{
		sharedController = this;
		if (Defs.isMulti && !Defs.isInet && Network.isServer)
		{
			InvokeRepeating("SinchServerTimeInvoke", 0.1f, 2f);
			Debug.Log("TimeGameController: Start synch server time");
		}
	}

	[Obfuscation(Exclude = true)]
	public void SinchServerTimeInvoke()
	{
		GetComponent<NetworkView>().RPC("SynchTimeServer", RPCMode.Others, (float)Network.time);
	}

	public void StartMatch()
	{
		bool flag = false;
		matchEnding = false;
		if (CapturePointController.sharedController != null)
		{
			CapturePointController.sharedController.isEndMatch = false;
		}
		if (Defs.isCapturePoints || Defs.isFlag)
		{
			double num = Convert.ToDouble(PhotonNetwork.room.customProperties["TimeMatchEnd"]);
			if (num < -5000000.0)
			{
				flag = true;
			}
		}
		if (Defs.isInet && ((timeEndMatch < PhotonNetwork.time && !Defs.isFlag) || Initializer.players.Count == 0 || (Defs.isFlag && flag)))
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			double num2 = PhotonNetwork.time + (double)(((!Defs.isCOOP) ? ((int)PhotonNetwork.room.customProperties[ConnectSceneNGUIController.maxKillProperty]) : 4) * 60);
			if (num2 > 4294967.0 && PhotonNetwork.time < 4294967.0)
			{
				num2 = 4294967.0;
			}
			hashtable["TimeMatchEnd"] = num2;
			hashtable[ConnectSceneNGUIController.endingProperty] = 0;
			PhotonNetwork.room.SetCustomProperties(hashtable);
			matchEndingPos = 0;
			timerToEndMatch = num2 - PhotonNetwork.time;
		}
		if (!Defs.isInet && (timeEndMatch < networkTime || Initializer.players.Count == 0))
		{
			timeEndMatch = networkTime + (double)((PlayerPrefs.GetString("MaxKill", "9").Equals(string.Empty) ? 5 : int.Parse(PlayerPrefs.GetString("MaxKill", "5"))) * 60);
			GetComponent<NetworkView>().RPC("SynchTimeEnd", RPCMode.Others, (float)timeEndMatch);
		}
	}

	private void CheckPause()
	{
		paused = false;
		long currentUnixTime = Tools.CurrentUnixTime;
		if (pauseTime > currentUnixTime || pauseTime + 60 < currentUnixTime)
		{
			PhotonNetwork.isMessageQueueRunning = true;
			PhotonNetwork.Disconnect();
		}
	}

	private void Update()
	{
		if (paused && Defs.isInet && Application.platform == RuntimePlatform.IPhonePlayer)
		{
			CheckPause();
			if (!PhotonNetwork.connected)
			{
				return;
			}
		}
		ipServera = PhotonNetwork.ServerAddress;
		if (Defs.isInet && PhotonNetwork.room != null && PhotonNetwork.room.customProperties["TimeMatchEnd"] != null)
		{
			double num = networkTime - PhotonNetwork.time;
			if (WeaponManager.sharedManager.myPlayerMoveC != null && num > 6.0 && num < 600.0)
			{
				Debug.LogError("Speedhack detected! Delta: " + num + ", Photon time: " + PhotonNetwork.time + ", Last time: " + networkTime);
				PhotonNetwork.isMessageQueueRunning = true;
				PhotonNetwork.Disconnect();
			}
			networkTime = PhotonNetwork.time;
			if (networkTime < 0.1)
			{
				return;
			}
			timeEndMatch = Convert.ToDouble(PhotonNetwork.room.customProperties["TimeMatchEnd"]);
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null && timeEndMatch > PhotonNetwork.time + 1500.0)
			{
				Initializer.Instance.goToConnect();
			}
			if (PhotonNetwork.room.customProperties.ContainsKey(ConnectSceneNGUIController.endingProperty))
			{
				matchEndingPos = (int)PhotonNetwork.room.customProperties[ConnectSceneNGUIController.endingProperty];
			}
			writtedMatchEndingPos = matchEndingPos;
			switch (matchEndingPos)
			{
			case 0:
				if (timeEndMatch < PhotonNetwork.time + (double)((!PhotonNetwork.isMasterClient) ? 110 : 130))
				{
					matchEndingPos = 2;
					Debug.Log("two minutes remain");
				}
				break;
			case 2:
				if (timeEndMatch < PhotonNetwork.time + (double)((!PhotonNetwork.isMasterClient) ? 50 : 70))
				{
					Debug.Log("one minute remain");
					matchEndingPos = 1;
				}
				break;
			}
			if (writtedMatchEndingPos != matchEndingPos)
			{
				Debug.Log("Write ending: " + matchEndingPos);
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable[ConnectSceneNGUIController.endingProperty] = matchEndingPos;
				PhotonNetwork.room.SetCustomProperties(hashtable);
			}
			if (timeEndMatch > 4290000.0 && networkTime < 2000000.0)
			{
				ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
				double num2 = networkTime + 60.0;
				hashtable2["TimeMatchEnd"] = num2;
				PhotonNetwork.room.SetCustomProperties(hashtable2);
			}
			if (timeEndMatch > 0.0)
			{
				timerToEndMatch = timeEndMatch - networkTime;
			}
			else
			{
				timerToEndMatch = -1.0;
			}
		}
		if (!Defs.isInet)
		{
			if (Network.isServer)
			{
				networkTime = Network.time;
			}
			else
			{
				networkTime += Time.deltaTime;
			}
			timerToEndMatch = timeEndMatch - networkTime;
		}
		if (timerToEndMatch < 0.0 && !Defs.isFlag)
		{
			if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints)
			{
				if (CapturePointController.sharedController != null && !isEndMatch)
				{
					CapturePointController.sharedController.EndMatch();
					isEndMatch = true;
				}
			}
			else if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TimeBattle)
			{
				if (!isEndMatch)
				{
					ZombiManager.sharedManager.EndMatch();
				}
			}
			else if (WeaponManager.sharedManager.myPlayer != null)
			{
				WeaponManager.sharedManager.myPlayerMoveC.WinFromTimer();
			}
			else
			{
				GlobalGameController.countKillsRed = 0;
				GlobalGameController.countKillsBlue = 0;
			}
		}
		else
		{
			isEndMatch = false;
		}
		if (wasPaused)
		{
			wasPaused = false;
			StartCoroutine(OnUnpause());
		}
		pauseTime = Tools.CurrentUnixTime;
	}

	private IEnumerator OnUnpause()
	{
		yield return null;
		yield return null;
		PhotonNetwork.isMessageQueueRunning = true;
	}

	private void OnPlayerConnected(NetworkPlayer player)
	{
		if (Network.isServer)
		{
			GetComponent<NetworkView>().RPC("SynchTimeEnd", RPCMode.Others, (float)timeEndMatch);
			GetComponent<NetworkView>().RPC("SynchTimeServer", RPCMode.Others, (float)Network.time);
		}
	}

	[RPC]
	[PunRPC]
	private void SynchTimeEnd(float synchTime)
	{
		timeEndMatch = synchTime;
	}

	[PunRPC]
	[RPC]
	private void SynchTimeServer(float synchTime)
	{
		if (networkTime < (double)synchTime)
		{
			networkTime = synchTime;
		}
	}

	private void OnDestroy()
	{
		sharedController = null;
	}
}

using System;
using System.Collections;
using System.Diagnostics;
using ExitGames.Client.Photon;
using UnityEngine;

internal class PhotonHandler : MonoBehaviour
{
	private const string PlayerPrefsKey = "PUNCloudBestRegion";

	public static PhotonHandler SP;

	public int updateInterval;

	public int updateIntervalOnSerialize;

	private int nextSendTickCount;

	private int nextSendTickCountOnSerialize;

	private static bool sendThreadShouldRun;

	private static Stopwatch timerToStopConnectionInBackground;

	protected internal static bool AppQuits;

	protected internal static Type PingImplementation;

	internal static CloudRegionCode BestRegionCodeCurrently = CloudRegionCode.none;

	internal static CloudRegionCode BestRegionCodeInPreferences
	{
		get
		{
			string @string = PlayerPrefs.GetString("PUNCloudBestRegion", string.Empty);
			if (!string.IsNullOrEmpty(@string))
			{
				return Region.Parse(@string);
			}
			return CloudRegionCode.none;
		}
		set
		{
			if (value == CloudRegionCode.none)
			{
				PlayerPrefs.DeleteKey("PUNCloudBestRegion");
			}
			else
			{
				PlayerPrefs.SetString("PUNCloudBestRegion", value.ToString());
			}
		}
	}

	protected void Awake()
	{
		if (SP != null && SP != this && SP.gameObject != null)
		{
			UnityEngine.Object.DestroyImmediate(SP.gameObject);
		}
		SP = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		updateInterval = 1000 / PhotonNetwork.sendRate;
		updateIntervalOnSerialize = 1000 / PhotonNetwork.sendRateOnSerialize;
		StartFallbackSendAckThread();
	}

	protected void OnLevelWasLoaded(int level)
	{
		PhotonNetwork.networkingPeer.NewSceneLoaded();
		PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
	}

	protected void OnApplicationQuit()
	{
		AppQuits = true;
		StopFallbackSendAckThread();
		PhotonNetwork.Disconnect();
	}

	protected void OnApplicationPause(bool pause)
	{
		if (PhotonNetwork.BackgroundTimeout > 0.1f)
		{
			if (timerToStopConnectionInBackground == null)
			{
				timerToStopConnectionInBackground = new Stopwatch();
			}
			timerToStopConnectionInBackground.Reset();
			if (pause)
			{
				timerToStopConnectionInBackground.Start();
			}
			else
			{
				timerToStopConnectionInBackground.Stop();
			}
		}
	}

	protected void OnDestroy()
	{
		StopFallbackSendAckThread();
	}

	protected void Update()
	{
		if (PhotonNetwork.networkingPeer == null)
		{
			UnityEngine.Debug.LogError("NetworkPeer broke!");
		}
		else
		{
			if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated || PhotonNetwork.connectionStateDetailed == ClientState.Disconnected || PhotonNetwork.offlineMode || !PhotonNetwork.isMessageQueueRunning)
			{
				return;
			}
			Defs.inComingMessagesCounter = 0;
			bool flag = true;
			while (PhotonNetwork.isMessageQueueRunning && flag)
			{
				flag = PhotonNetwork.networkingPeer.DispatchIncomingCommands();
				Defs.inComingMessagesCounter++;
			}
			Defs.inComingMessagesCounter = 0;
			int num = (int)(Time.realtimeSinceStartup * 1000f);
			if (PhotonNetwork.isMessageQueueRunning && num > nextSendTickCountOnSerialize)
			{
				PhotonNetwork.networkingPeer.RunViewUpdate();
				nextSendTickCountOnSerialize = num + updateIntervalOnSerialize;
				nextSendTickCount = 0;
			}
			num = (int)(Time.realtimeSinceStartup * 1000f);
			if (num > nextSendTickCount)
			{
				bool flag2 = true;
				while (PhotonNetwork.isMessageQueueRunning && flag2)
				{
					flag2 = PhotonNetwork.networkingPeer.SendOutgoingCommands();
				}
				nextSendTickCount = num + updateInterval;
			}
		}
	}

	protected void OnJoinedRoom()
	{
		PhotonNetwork.networkingPeer.LoadLevelIfSynced();
	}

	protected void OnCreatedRoom()
	{
		PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
	}

	public static void StartFallbackSendAckThread()
	{
		if (!sendThreadShouldRun)
		{
			sendThreadShouldRun = true;
			SupportClass.CallInBackground(FallbackSendAckThread);
		}
	}

	public static void StopFallbackSendAckThread()
	{
		sendThreadShouldRun = false;
	}

	public static bool FallbackSendAckThread()
	{
		if (sendThreadShouldRun && PhotonNetwork.networkingPeer != null)
		{
			if (timerToStopConnectionInBackground != null && PhotonNetwork.BackgroundTimeout > 0.1f && (float)timerToStopConnectionInBackground.ElapsedMilliseconds > PhotonNetwork.BackgroundTimeout * 1000f)
			{
				if (PhotonNetwork.connected)
				{
					PhotonNetwork.Disconnect();
				}
				timerToStopConnectionInBackground.Stop();
				timerToStopConnectionInBackground.Reset();
				return sendThreadShouldRun;
			}
			if (PhotonNetwork.networkingPeer.ConnectionTime - PhotonNetwork.networkingPeer.LastSendOutgoingTime > 200)
			{
				PhotonNetwork.networkingPeer.SendAcksOnly();
			}
		}
		return sendThreadShouldRun;
	}

	protected internal static void PingAvailableRegionsAndConnectToBest()
	{
		SP.StartCoroutine(SP.PingAvailableRegionsCoroutine(true));
	}

	internal IEnumerator PingAvailableRegionsCoroutine(bool connectToBest)
	{
		BestRegionCodeCurrently = CloudRegionCode.none;
		while (PhotonNetwork.networkingPeer.AvailableRegions == null)
		{
			if (PhotonNetwork.connectionStateDetailed != ClientState.ConnectingToNameServer && PhotonNetwork.connectionStateDetailed != ClientState.ConnectedToNameServer)
			{
				UnityEngine.Debug.LogError("Call ConnectToNameServer to ping available regions.");
				yield break;
			}
			UnityEngine.Debug.Log(string.Concat("Waiting for AvailableRegions. State: ", PhotonNetwork.connectionStateDetailed, " Server: ", PhotonNetwork.Server, " PhotonNetwork.networkingPeer.AvailableRegions ", PhotonNetwork.networkingPeer.AvailableRegions != null));
			yield return new WaitForSeconds(0.25f);
		}
		if (PhotonNetwork.networkingPeer.AvailableRegions == null || PhotonNetwork.networkingPeer.AvailableRegions.Count == 0)
		{
			UnityEngine.Debug.LogError("No regions available. Are you sure your appid is valid and setup?");
			yield break;
		}
		PhotonPingManager pingManager = new PhotonPingManager();
		foreach (Region region in PhotonNetwork.networkingPeer.AvailableRegions)
		{
			SP.StartCoroutine(pingManager.PingSocket(region));
		}
		while (!pingManager.Done)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Region best = pingManager.BestRegion;
		BestRegionCodeCurrently = best.Code;
		BestRegionCodeInPreferences = best.Code;
		UnityEngine.Debug.Log(string.Concat("Found best region: ", best.Code, " ping: ", best.Ping, ". Calling ConnectToRegionMaster() is: ", connectToBest));
		if (connectToBest)
		{
			PhotonNetwork.networkingPeer.ConnectToRegionMaster(best.Code);
		}
	}
}

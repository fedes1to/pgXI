using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

[Serializable]
public class ServerSettings : ScriptableObject
{
	public enum HostingOption
	{
		NotSet,
		PhotonCloud,
		SelfHosted,
		OfflineMode,
		BestRegion
	}

	public HostingOption HostType;

	public ConnectionProtocol Protocol;

	public string ServerAddress = string.Empty;

	public int ServerPort = 5055;

	public string AppID = string.Empty;

	public string VoiceAppID = string.Empty;

	public CloudRegionCode PreferredRegion;

	public CloudRegionFlag EnabledRegions = (CloudRegionFlag)(-1);

	public bool JoinLobby;

	public bool EnableLobbyStatistics;

	public List<string> RpcList = new List<string>();

	[HideInInspector]
	public bool DisableAutoOpenWizard;

	public void UseCloudBestRegion(string cloudAppid)
	{
		HostType = HostingOption.BestRegion;
		AppID = cloudAppid;
	}

	public void UseCloud(string cloudAppid)
	{
		HostType = HostingOption.PhotonCloud;
		AppID = cloudAppid;
	}

	public void UseCloud(string cloudAppid, CloudRegionCode code)
	{
		HostType = HostingOption.PhotonCloud;
		AppID = "4d2df5e9-5172-44a0-bf8b-df5346a54393";
		PreferredRegion = code;
	}

	public void UseMyServer(string serverAddress, int serverPort, string application)
	{
		HostType = HostingOption.SelfHosted;
		AppID = ((application == null) ? "master" : application);
		ServerAddress = serverAddress;
		ServerPort = serverPort;
	}

	public override string ToString()
	{
		return string.Concat("ServerSettings: ", HostType, " ", ServerAddress);
	}
}

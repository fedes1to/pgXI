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
		AppID = "79c808e8-417d-4df4-8b77-e3029c00e42f";
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

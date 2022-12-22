using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

public class InRoomTime : MonoBehaviour
{
	private const string StartTimeKey = "#rt";

	private int roomStartTimestamp;

	public double RoomTime
	{
		get
		{
			uint roomTimestamp = (uint)RoomTimestamp;
			double num = roomTimestamp;
			return num / 1000.0;
		}
	}

	public int RoomTimestamp
	{
		get
		{
			return PhotonNetwork.inRoom ? (PhotonNetwork.ServerTimestamp - roomStartTimestamp) : 0;
		}
	}

	public bool IsRoomTimeSet
	{
		get
		{
			return PhotonNetwork.inRoom && PhotonNetwork.room.customProperties.ContainsKey("#rt");
		}
	}

	internal IEnumerator SetRoomStartTimestamp()
	{
		if (!IsRoomTimeSet && PhotonNetwork.isMasterClient)
		{
			if (PhotonNetwork.ServerTimestamp == 0)
			{
				yield return 0;
			}
			ExitGames.Client.Photon.Hashtable startTimeProp = new ExitGames.Client.Photon.Hashtable();
			startTimeProp["#rt"] = PhotonNetwork.ServerTimestamp;
			PhotonNetwork.room.SetCustomProperties(startTimeProp);
		}
	}

	public void OnJoinedRoom()
	{
		StartCoroutine("SetRoomStartTimestamp");
	}

	public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		StartCoroutine("SetRoomStartTimestamp");
	}

	public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey("#rt"))
		{
			roomStartTimestamp = (int)propertiesThatChanged["#rt"];
		}
	}
}

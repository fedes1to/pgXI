using System;
using UnityEngine;

public class ServerTime : MonoBehaviour
{
	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, 0f, 200f, 30f));
		GUILayout.Label(string.Format("Time Offset: {0}", PhotonNetwork.ServerTimestamp - Environment.TickCount));
		if (GUILayout.Button("fetch"))
		{
			PhotonNetwork.FetchServerTimestamp();
		}
		GUILayout.EndArea();
	}
}

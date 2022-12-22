using System.Collections.Generic;
using UnityEngine;

public class PhotonObjectCacher : MonoBehaviour
{
	public static void AddObject(GameObject obj)
	{
		if (PhotonNetwork.SendMonoMessageTargets == null)
		{
			PhotonNetwork.SendMonoMessageTargets = new HashSet<GameObject>();
		}
		if (obj != null && !PhotonNetwork.SendMonoMessageTargets.Contains(obj))
		{
			PhotonNetwork.SendMonoMessageTargets.Add(obj);
		}
	}

	public static void RemoveObject(GameObject obj)
	{
		if (PhotonNetwork.SendMonoMessageTargets != null && PhotonNetwork.SendMonoMessageTargets.Contains(obj))
		{
			PhotonNetwork.SendMonoMessageTargets.Remove(obj);
		}
	}
}

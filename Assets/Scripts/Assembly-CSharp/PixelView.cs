using UnityEngine;

public sealed class PixelView : MonoBehaviour
{
	private static int viewIdCount = 1000;

	private PhotonView photonView;

	private PhotonView _PhotonView;

	private int localViewID = -1;

	public int viewID
	{
		get
		{
			if (Defs.isMulti)
			{
				if (Defs.isInet)
				{
					return photonView.viewID;
				}
				return localViewID;
			}
			return 0;
		}
	}

	private void Awake()
	{
		if (!Defs.isMulti)
		{
			return;
		}
		if (Defs.isInet)
		{
			photonView = GetComponent<PhotonView>();
			if (photonView == null)
			{
				Debug.LogError("GetComponent<PhotonView>() == null");
			}
			return;
		}
		_PhotonView = GetComponent<PhotonView>();
		_PhotonView.RPC("SendViewID", PhotonTargets.AllBuffered, viewIdCount++);
	}

	private void SendViewID(int id)
	{
		if (localViewID != -1)
		{
			Debug.LogError("Local id is already set! " + localViewID + " (new: " + id + ")");
		}
		localViewID = id;
	}
}

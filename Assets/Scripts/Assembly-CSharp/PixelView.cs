using UnityEngine;

public sealed class PixelView : MonoBehaviour
{
	private static int viewIdCount = 1000;

	private PhotonView photonView;

	private NetworkView _networkView;

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
		_networkView = GetComponent<NetworkView>();
		if (Network.isServer)
		{
			_networkView.RPC("SendViewID", RPCMode.AllBuffered, viewIdCount++);
		}
	}

	[RPC]
	private void SendViewID(int id)
	{
		if (localViewID != -1)
		{
			Debug.LogError("Local id is already set! " + localViewID + " (new: " + id + ")");
		}
		localViewID = id;
	}
}

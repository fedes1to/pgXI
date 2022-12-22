using UnityEngine;

public class DaterHockeyController : MonoBehaviour
{
	public float coefForce = 200f;

	public int score1;

	public int score2;

	private bool isForceMyPlayer;

	private float timeSendForce = 0.3f;

	private float timerToSendForce = -1f;

	private PhotonView photonView;

	private Rigidbody thisRigidbody;

	private Transform thisTransform;

	private Vector3 resetPositionPoint;

	private bool isFirstSynhPos = true;

	private bool isResetPosition;

	private Vector3 synchPos;

	private Quaternion synchRot;

	private bool isMine;

	private void Awake()
	{
		photonView = GetComponent<PhotonView>();
		thisRigidbody = GetComponent<Rigidbody>();
		thisTransform = base.transform;
		resetPositionPoint = thisTransform.position;
	}

	private void Start()
	{
		isMine = !Defs.isMulti || (!Defs.isInet && GetComponent<NetworkView>().isMine) || (Defs.isInet && photonView.isMine);
	}

	private void Update()
	{
		if (isForceMyPlayer && WeaponManager.sharedManager.myPlayer == null)
		{
			isForceMyPlayer = false;
		}
		if (isForceMyPlayer)
		{
			timerToSendForce -= Time.deltaTime;
			if (timerToSendForce < 0f)
			{
				timerToSendForce = timeSendForce;
				AddForce(Vector3.Normalize(thisTransform.position - WeaponManager.sharedManager.myPlayerMoveC.myPlayerTransform.position) * coefForce);
			}
		}
		if (!isMine)
		{
			thisTransform.position = Vector3.Lerp(thisTransform.position, synchPos, Time.deltaTime * 5f);
			thisTransform.rotation = Quaternion.Lerp(thisTransform.rotation, synchRot, Time.deltaTime * 5f);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (WeaponManager.sharedManager.myPlayerMoveC != null && collider.gameObject.transform.parent != null && collider.gameObject.transform.parent.Equals(WeaponManager.sharedManager.myPlayerMoveC.myPlayerTransform))
		{
			isForceMyPlayer = true;
			return;
		}
		if (isMine && collider.gameObject.name.Equals("Gates1"))
		{
			ResetPosition();
		}
		if (isMine && collider.gameObject.name.Equals("Gates2"))
		{
			ResetPosition();
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (WeaponManager.sharedManager.myPlayerMoveC != null && collider.gameObject.transform.parent != null && collider.gameObject.transform.parent.Equals(WeaponManager.sharedManager.myPlayerMoveC.myPlayerTransform))
		{
			isForceMyPlayer = false;
		}
		if (isMine && collider.gameObject.name.Equals("Stadium"))
		{
			ResetPosition();
		}
	}

	[RPC]
	[PunRPC]
	private void AddForceRPC(Vector3 _force)
	{
		GetComponent<Rigidbody>().AddForce(_force);
	}

	private void AddForce(Vector3 _force)
	{
		if (Defs.isInet)
		{
			photonView.RPC("AddForceRPC", PhotonTargets.All, _force);
		}
		else
		{
			GetComponent<NetworkView>().RPC("AddForceRPC", RPCMode.Server, _force);
			AddForceRPC(_force);
		}
	}

	private void ResetPosition()
	{
		thisTransform.position = resetPositionPoint;
		thisRigidbody.velocity = Vector3.zero;
		thisRigidbody.angularVelocity = Vector3.zero;
		isResetPosition = true;
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(thisTransform.position);
			stream.SendNext(thisTransform.rotation);
			stream.SendNext(thisRigidbody.velocity);
			stream.SendNext(thisRigidbody.angularVelocity);
			stream.SendNext(isResetPosition);
			return;
		}
		synchPos = (Vector3)stream.ReceiveNext();
		synchRot = (Quaternion)stream.ReceiveNext();
		thisRigidbody.velocity = (Vector3)stream.ReceiveNext();
		thisRigidbody.angularVelocity = (Vector3)stream.ReceiveNext();
		isResetPosition = (bool)stream.ReceiveNext();
		if (isFirstSynhPos)
		{
			thisTransform.position = synchPos;
			thisTransform.rotation = synchRot;
			isFirstSynhPos = false;
			isResetPosition = false;
		}
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 value = thisTransform.position;
			Quaternion value2 = thisTransform.rotation;
			stream.Serialize(ref value);
			stream.Serialize(ref value2);
			bool value3 = isResetPosition;
			stream.Serialize(ref value3);
			if (isResetPosition)
			{
				isResetPosition = false;
			}
			return;
		}
		Vector3 value4 = Vector3.zero;
		Quaternion value5 = Quaternion.identity;
		bool value6 = false;
		stream.Serialize(ref value4);
		stream.Serialize(ref value5);
		stream.Serialize(ref value6);
		synchPos = value4;
		synchRot = value5;
		isResetPosition = value6;
		if (isFirstSynhPos || isResetPosition)
		{
			thisTransform.position = synchPos;
			thisTransform.rotation = synchRot;
			isFirstSynhPos = false;
			isResetPosition = false;
		}
	}
}

using UnityEngine;

public sealed class PortalForPlayerController : MonoBehaviour
{
	public PortalForPlayerController myDublicatePortal;

	private Transform myPointOut;

	private void Start()
	{
		myPointOut = base.transform.GetChild(0);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name.Equals("BodyCollider") && other.transform.parent != null && other.transform.parent.gameObject.Equals(WeaponManager.sharedManager.myPlayer))
		{
			WeaponManager.sharedManager.myPlayer.transform.position = myDublicatePortal.myPointOut.position;
			WeaponManager.sharedManager.myPlayerMoveC.myPersonNetwork.isTeleported = true;
			float y = myPointOut.transform.rotation.eulerAngles.y;
			float num = myDublicatePortal.myPointOut.transform.rotation.eulerAngles.y;
			if (num < y)
			{
				num += 360f;
			}
			float yAngle = num - y - 180f;
			WeaponManager.sharedManager.myPlayer.transform.Rotate(0f, yAngle, 0f);
			WeaponManager.sharedManager.myPlayerMoveC.PlayPortalSound();
		}
	}
}

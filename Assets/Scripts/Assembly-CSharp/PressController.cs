using UnityEngine;

public class PressController : MonoBehaviour
{
	public bool isPrimary;

	public PressController primaryPress;

	private GameObject firstCollision;

	[HideInInspector]
	public GameObject secondCollision;

	private void OnTriggerEnter(Collider col)
	{
		if (col.transform.gameObject == WeaponManager.sharedManager.myPlayer)
		{
			if (isPrimary)
			{
				firstCollision = col.transform.gameObject;
				CheckSmash();
			}
			else
			{
				primaryPress.secondCollision = col.transform.gameObject;
				primaryPress.CheckSmash();
			}
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.transform.gameObject == WeaponManager.sharedManager.myPlayer)
		{
			if (isPrimary)
			{
				firstCollision = null;
			}
			else
			{
				primaryPress.secondCollision = null;
			}
		}
	}

	public void CheckSmash()
	{
		if (firstCollision == secondCollision)
		{
			Player_move_c playerMoveC = firstCollision.GetComponent<SkinName>().playerMoveC;
			playerMoveC.KillSelf();
		}
	}

	private void Update()
	{
	}
}

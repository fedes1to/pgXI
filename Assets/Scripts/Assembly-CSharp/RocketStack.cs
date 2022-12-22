using System.Collections.Generic;
using UnityEngine;

public class RocketStack : MonoBehaviour
{
	public static RocketStack sharedController;

	private List<GameObject> gameObjects = new List<GameObject>();

	private float timeUseGameObjects;

	public Transform mytranform;

	private void Awake()
	{
		sharedController = this;
		mytranform = base.transform;
	}

	public GameObject GetRocket()
	{
		while (gameObjects.Count > 0 && gameObjects[0] == null)
		{
			gameObjects.RemoveAt(0);
		}
		GameObject gameObject = null;
		if (gameObjects.Count > 0)
		{
			gameObject = gameObjects[0];
			gameObjects.RemoveAt(0);
			gameObject.SetActive(true);
		}
		else
		{
			gameObject = ((!Defs.isMulti) ? (Object.Instantiate(Resources.Load("Rocket") as GameObject, Vector3.down * 10000f, Quaternion.identity) as GameObject) : (Defs.isInet ? PhotonNetwork.Instantiate("Rocket", Vector3.down * 10000f, Quaternion.identity, 0) : ((GameObject)Network.Instantiate(Resources.Load("Rocket") as GameObject, Vector3.down * 10000f, Quaternion.identity, 0))));
			gameObject.transform.parent = mytranform;
		}
		return gameObject;
	}

	public void ReturnRocket(GameObject returnObject)
	{
		Rigidbody component = returnObject.GetComponent<Rigidbody>();
		component.velocity = Vector3.zero;
		component.isKinematic = false;
		component.useGravity = false;
		component.angularVelocity = Vector3.zero;
		returnObject.transform.position = Vector3.down * 10000f;
		returnObject.SetActive(false);
		timeUseGameObjects = Time.realtimeSinceStartup;
		gameObjects.Add(returnObject);
	}

	private void OnDestroy()
	{
		sharedController = null;
	}
}

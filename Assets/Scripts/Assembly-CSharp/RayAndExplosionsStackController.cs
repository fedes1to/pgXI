using System.Collections.Generic;
using UnityEngine;

public class RayAndExplosionsStackController : MonoBehaviour
{
	public static RayAndExplosionsStackController sharedController;

	private Dictionary<string, List<GameObject>> gameObjects = new Dictionary<string, List<GameObject>>();

	private Dictionary<string, float> timeUseGameObjects = new Dictionary<string, float>();

	public Transform mytranform;

	private void Awake()
	{
		sharedController = this;
		mytranform = GetComponent<Transform>();
	}

	public GameObject GetObjectFromName(string _name)
	{
		GameObject gameObject = null;
		bool flag = gameObjects.ContainsKey(_name);
		if (flag)
		{
			while (gameObjects[_name].Count > 0 && gameObjects[_name][0] == null)
			{
				gameObjects[_name].RemoveAt(0);
			}
		}
		if (flag && gameObjects[_name].Count > 0)
		{
			gameObject = gameObjects[_name][0];
			gameObjects[_name].RemoveAt(0);
			gameObject.SetActive(true);
		}
		else
		{
			GameObject gameObject2 = Resources.Load(_name) as GameObject;
			if (gameObject2 != null)
			{
				gameObject = Object.Instantiate(gameObject2, Vector3.down * 10000f, Quaternion.identity) as GameObject;
				gameObject.GetComponent<Transform>().parent = mytranform;
				gameObject.GetComponent<RayAndExplosionsStackItem>().myName = _name;
			}
		}
		if (gameObject == null && Application.isEditor)
		{
			Debug.LogError("GameOblect " + _name + " in RayAndExplosionsStackController not create!!!");
		}
		return gameObject;
	}

	public void ReturnObjectFromName(GameObject returnObject, string _name)
	{
		returnObject.GetComponent<Transform>().position = Vector3.down * 10000f;
		returnObject.SetActive(false);
		returnObject.transform.parent = mytranform;
		if (!gameObjects.ContainsKey(_name))
		{
			gameObjects.Add(_name, new List<GameObject>());
		}
		if (!timeUseGameObjects.ContainsKey(_name))
		{
			timeUseGameObjects.Add(_name, Time.realtimeSinceStartup);
		}
		else
		{
			timeUseGameObjects[_name] = Time.realtimeSinceStartup;
		}
		gameObjects[_name].Add(returnObject);
	}

	private void OnDestroy()
	{
		sharedController = null;
	}
}

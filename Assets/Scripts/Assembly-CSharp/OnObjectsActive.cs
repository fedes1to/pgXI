using UnityEngine;

public class OnObjectsActive : MonoBehaviour
{
	public GameObject[] objects;

	private int activeObjectsCount;

	public MonoBehaviour objectToEnable;

	private void Update()
	{
		activeObjectsCount = objects.Length;
		for (int i = 0; i < objects.Length; i++)
		{
			activeObjectsCount += (objects[i].activeInHierarchy ? 1 : (-1));
		}
		objectToEnable.enabled = activeObjectsCount > 0;
	}
}

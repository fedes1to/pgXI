using System.Collections;
using UnityEngine;

public class RemoveRay : MonoBehaviour
{
	public float lifetime = 0.7f;

	public float length = 100f;

	private IEnumerator Start()
	{
		float startTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - startTime < lifetime)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}

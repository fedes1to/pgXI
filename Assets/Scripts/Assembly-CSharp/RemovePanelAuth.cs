using UnityEngine;

public class RemovePanelAuth : MonoBehaviour
{
	public float lifetime = 0.7f;

	private float startTime;

	private bool isDestroyed;

	private void Start()
	{
		startTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (!isDestroyed && Time.realtimeSinceStartup - startTime >= lifetime)
		{
			Object.Destroy(base.gameObject);
			isDestroyed = true;
		}
	}
}

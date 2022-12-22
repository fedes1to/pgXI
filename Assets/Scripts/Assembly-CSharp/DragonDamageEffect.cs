using UnityEngine;

public class DragonDamageEffect : MonoBehaviour
{
	public float lifeTime = 2f;

	private float lifeTimer;

	private void Awake()
	{
		lifeTimer = Time.time + lifeTime;
	}

	private void Update()
	{
		base.transform.position += base.transform.forward * 23f * Time.deltaTime;
		if (lifeTimer < Time.time)
		{
			Object.Destroy(base.gameObject);
		}
	}
}

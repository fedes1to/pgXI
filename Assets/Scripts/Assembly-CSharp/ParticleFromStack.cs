using UnityEngine;

public class ParticleFromStack : MonoBehaviour
{
	public ParticleStackController fromStack;

	public float lifeTime;

	private float lifeTimer;

	private void OnEnable()
	{
		lifeTimer = lifeTime + Time.time;
	}

	private void Update()
	{
		if (!(fromStack == null) && lifeTimer < Time.time)
		{
			fromStack.ReturnParticle(base.gameObject);
		}
	}
}

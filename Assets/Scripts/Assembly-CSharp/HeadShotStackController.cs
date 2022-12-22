using UnityEngine;

public class HeadShotStackController : MonoBehaviour
{
	public static HeadShotStackController sharedController;

	public HitParticle[] particles;

	public Material mobHeadShotMaterial;

	private int currentIndexHole;

	private void Start()
	{
		if (Device.isPixelGunLow)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		sharedController = this;
		Transform transform = base.transform;
		transform.position = Vector3.zero;
		int childCount = transform.childCount;
		particles = new HitParticle[childCount];
		for (int i = 0; i < childCount; i++)
		{
			particles[i] = transform.GetChild(i).GetComponent<HitParticle>();
			if (!Defs.isMulti || Defs.isCOOP)
			{
				ParticleSystem myParticleSystem = particles[i].myParticleSystem;
				myParticleSystem.GetComponent<Renderer>().material = mobHeadShotMaterial;
			}
		}
	}

	public HitParticle GetCurrentParticle(bool _isUseMine)
	{
		bool flag = true;
		do
		{
			currentIndexHole++;
			if (currentIndexHole >= particles.Length)
			{
				if (!flag)
				{
					return null;
				}
				currentIndexHole = 0;
				flag = false;
			}
		}
		while (particles[currentIndexHole].isUseMine && !_isUseMine);
		return particles[currentIndexHole];
	}

	private void OnDestroy()
	{
		sharedController = null;
	}
}

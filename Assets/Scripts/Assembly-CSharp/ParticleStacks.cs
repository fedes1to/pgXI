using UnityEngine;

public class ParticleStacks : MonoBehaviour
{
	public static ParticleStacks instance;

	public ParticleStackController fireStack;

	public ParticleStackController lightningStack;

	public HitStackController hitStack;

	public HitStackController poisonHitStack;

	public HitStackController criticalHitStack;

	public HitStackController bleedHitStack;

	public GameObject dragonPrefab;

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		instance = null;
	}
}

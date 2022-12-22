using UnityEngine;

public class ParticleStackController : MonoBehaviour
{
	public GameObject prefab;

	public int capacity = 20;

	private int count;

	private GameObject[] particleBuffer;

	private void Awake()
	{
		particleBuffer = new GameObject[capacity];
		for (int i = 0; i < particleBuffer.Length; i++)
		{
			particleBuffer[i] = Object.Instantiate(prefab);
			particleBuffer[i].transform.parent = base.transform;
		}
		count = particleBuffer.Length;
	}

	public void ReturnParticle(GameObject particle)
	{
		if (count < particleBuffer.Length)
		{
			particle.transform.parent = base.transform;
			particle.transform.localScale = Vector3.one;
			particle.SetActive(false);
			particleBuffer[count++] = particle;
		}
	}

	public GameObject GetParticle()
	{
		if (count <= 0)
		{
			return null;
		}
		GameObject gameObject = particleBuffer[--count];
		gameObject.SetActive(true);
		return gameObject;
	}
}

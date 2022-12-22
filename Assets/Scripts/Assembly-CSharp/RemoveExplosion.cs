using System.Reflection;
using UnityEngine;

internal sealed class RemoveExplosion : MonoBehaviour
{
	private void Start()
	{
		float num = ((!(GetComponent<ParticleSystem>() != null)) ? 0.1f : GetComponent<ParticleSystem>().duration);
		if ((bool)GetComponent<AudioSource>() && GetComponent<AudioSource>().enabled && Defs.isSoundFX)
		{
			GetComponent<AudioSource>().Play();
		}
		Invoke("Remove", 7f);
	}

	[Obfuscation(Exclude = true)]
	private void Remove()
	{
		Object.Destroy(base.gameObject);
	}
}

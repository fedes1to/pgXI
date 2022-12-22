using UnityEngine;

public class WeaponParticleDelay : MonoBehaviour
{
	private Animation weaponAnimation;

	public string animationName;

	public float delay;

	public ParticleSystem partSystem;

	private bool seqStarted;

	private void Awake()
	{
		weaponAnimation = GetComponent<Animation>();
	}

	private void Update()
	{
		if (weaponAnimation.IsPlaying(animationName))
		{
			if (!seqStarted)
			{
				seqStarted = true;
				partSystem.gameObject.SetActive(false);
				Invoke("TurnOnParticleSystem", delay);
			}
		}
		else
		{
			seqStarted = false;
		}
	}

	private void TurnOnParticleSystem()
	{
		partSystem.gameObject.SetActive(true);
	}
}

using UnityEngine;

public class PlayAnimationSounds : MonoBehaviour
{
	public AudioSource aSource;

	public AudioClip[] sounds;

	private void PlayAnimSound(int number)
	{
		aSource.pitch = 1f;
		if (Defs.isSoundFX)
		{
			aSource.loop = false;
			aSource.clip = sounds[number];
			aSource.Play();
		}
	}

	private void PlayAnimSoundPithced(int number)
	{
		aSource.pitch = Random.Range(0.6f, 1.2f);
		if (Defs.isSoundFX)
		{
			aSource.loop = false;
			aSource.clip = sounds[number];
			aSource.Play();
		}
	}
}

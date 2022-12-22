using UnityEngine;

public class AudioSourseOnOff : MonoBehaviour
{
	private AudioSource[] myAudioSources;

	private void Awake()
	{
		myAudioSources = GetComponents<AudioSource>();
		for (int i = 0; i < myAudioSources.Length; i++)
		{
			if (myAudioSources[i] != null)
			{
				myAudioSources[i].enabled = Defs.isSoundFX;
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < myAudioSources.Length; i++)
		{
			if (myAudioSources[i] != null && myAudioSources[i].enabled != Defs.isSoundFX)
			{
				myAudioSources[i].enabled = Defs.isSoundFX;
			}
		}
	}
}

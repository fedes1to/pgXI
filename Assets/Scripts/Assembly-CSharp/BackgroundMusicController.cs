using UnityEngine;

internal sealed class BackgroundMusicController : MonoBehaviour
{
	private void Start()
	{
		MenuBackgroundMusic.sharedMusic.PlayMusic(GetComponent<AudioSource>());
	}

	public void Play()
	{
		MenuBackgroundMusic.sharedMusic.PlayMusic(GetComponent<AudioSource>());
	}

	public void Stop()
	{
		MenuBackgroundMusic.sharedMusic.StopMusic(GetComponent<AudioSource>());
	}
}

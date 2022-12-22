using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Rilisoft;
using UnityEngine;
using UnityEngine.SceneManagement;

internal sealed class MenuBackgroundMusic : MonoBehaviour
{
	public enum LobbyBackgroundClip
	{
		None,
		Classic,
		Ambient,
		Modern
	}

	public const string KEY_LOBBY_SETTED_BG_MUSIC = "lobby_background_music";

	private List<AudioSource> _customMusicStack = new List<AudioSource>();

	private AudioSource currentAudioSource;

	public static bool keepPlaying = false;

	public static MenuBackgroundMusic sharedMusic;

	private static string[] scenetsToPlayMusicOn = new string[10]
	{
		Defs.MainMenuScene,
		"ConnectScene",
		"ConnectSceneSandbox",
		"SettingScene",
		"SkinEditor",
		"ChooseLevel",
		"CampaignChooseBox",
		"ProfileShop",
		"Friends",
		"Clans"
	};

	public static string SettedLobbyBackgrounClip
	{
		get
		{
			return Storager.getString("lobby_background_music", true);
		}
	}

	public void PlayCustomMusicFrom(GameObject audioSourceObj)
	{
		RemoveNullsFromCustomMusicStack();
		if (audioSourceObj != null && Defs.isSoundMusic)
		{
			AudioSource component = audioSourceObj.GetComponent<AudioSource>();
			PlayMusic(component);
			if (!_customMusicStack.Contains(component))
			{
				if (_customMusicStack.Count > 0)
				{
					StopMusic(_customMusicStack[_customMusicStack.Count - 1]);
				}
				_customMusicStack.Add(audioSourceObj.GetComponent<AudioSource>());
			}
		}
		string value = SceneManager.GetActiveScene().name;
		if (Array.IndexOf(scenetsToPlayMusicOn, value) >= 0)
		{
			Stop();
			return;
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("BackgroundMusic");
		if (gameObject != null)
		{
			AudioSource component2 = gameObject.GetComponent<AudioSource>();
			if (component2 != null)
			{
				StopMusic(component2);
			}
		}
	}

	public void StopCustomMusicFrom(GameObject audioSourceObj)
	{
		RemoveNullsFromCustomMusicStack();
		AudioSource component = audioSourceObj.GetComponent<AudioSource>();
		if (audioSourceObj != null && component != null)
		{
			StopMusic(component);
			_customMusicStack.Remove(component);
		}
		if (_customMusicStack.Count > 0)
		{
			PlayMusic(_customMusicStack[_customMusicStack.Count - 1]);
			return;
		}
		if (Array.IndexOf(scenetsToPlayMusicOn, Application.loadedLevelName) >= 0)
		{
			Play();
			return;
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("BackgroundMusic");
		if (gameObject != null)
		{
			AudioSource component2 = gameObject.GetComponent<AudioSource>();
			if (component2 != null)
			{
				PlayMusic(component2);
			}
		}
	}

	internal void Start()
	{
		sharedMusic = this;
		Defs.isSoundMusic = PlayerPrefsX.GetBool(PlayerPrefsX.SoundMusicSetting, true);
		Defs.isSoundFX = PlayerPrefsX.GetBool(PlayerPrefsX.SoundFXSetting, true);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		string text = Storager.getString("lobby_background_music", true);
		if (text.IsNullOrEmpty())
		{
			text = LobbyBackgroundClip.Ambient.ToString();
			Storager.setString("lobby_background_music", text, true);
		}
		LobbyBackgroundClip? lobbyBackgroundClip = text.ToEnum<LobbyBackgroundClip>(LobbyBackgroundClip.None);
		if (lobbyBackgroundClip.HasValue && (lobbyBackgroundClip.GetValueOrDefault() != 0 || !lobbyBackgroundClip.HasValue))
		{
			AudioSource component = sharedMusic.GetComponent<AudioSource>();
			if (component != null)
			{
				AudioClip clip = Resources.Load<AudioClip>("MenuMusic/menu_music_" + lobbyBackgroundClip.Value.ToString().ToLower());
				component.clip = clip;
			}
		}
	}

	internal void Play()
	{
		if (Defs.isSoundMusic)
		{
			PlayMusic(GetComponent<AudioSource>());
		}
	}

	public void Stop()
	{
		StopMusic(GetComponent<AudioSource>());
	}

	private void RemoveNullsFromCustomMusicStack()
	{
		List<AudioSource> customMusicStack = _customMusicStack;
		_customMusicStack = new List<AudioSource>();
		foreach (AudioSource item in customMusicStack)
		{
			if (item != null)
			{
				_customMusicStack.Add(item);
			}
		}
	}

	private IEnumerator WaitFreeAwardControllerAndSubscribeCoroutine()
	{
		ScopeLogger scopeLogger = new ScopeLogger("WaitFreeAwardControllerAndSubscribeCoroutine", false);
		try
		{
			while (FreeAwardController.Instance == null)
			{
				yield return null;
			}
			FreeAwardController.Instance.StateChanged -= HandleFreeAwardControllerStateChanged;
			FreeAwardController.Instance.StateChanged += HandleFreeAwardControllerStateChanged;
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}

	private void OnLevelWasLoaded(int idx)
	{
		StopAllCoroutines();
		CoroutineRunner.Instance.StartCoroutine(WaitFreeAwardControllerAndSubscribeCoroutine());
		foreach (AudioSource item in _customMusicStack)
		{
			if (item != null)
			{
				item.Stop();
			}
		}
		_customMusicStack.Clear();
		if (Array.IndexOf(scenetsToPlayMusicOn, Application.loadedLevelName) >= 0 || keepPlaying)
		{
			if (!GetComponent<AudioSource>().isPlaying && PlayerPrefsX.GetBool(PlayerPrefsX.SoundMusicSetting, true))
			{
				PlayMusic(GetComponent<AudioSource>());
			}
		}
		else
		{
			StopMusic(GetComponent<AudioSource>());
		}
		keepPlaying = false;
	}

	private void HandleFreeAwardControllerStateChanged(object sender, FreeAwardController.StateEventArgs e)
	{
		string callee = string.Format(CultureInfo.InvariantCulture, "HandleFreeAwardControllerStateChanged({0} -> {1})", e.OldState, e.State);
		ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
		try
		{
			if (e.State is FreeAwardController.WatchingState)
			{
				Stop();
			}
			else if (e.OldState is FreeAwardController.WatchingState)
			{
				Play();
			}
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}

	public void PlayMusic(AudioSource audioSource)
	{
		if (!(audioSource == null) && Defs.isSoundMusic)
		{
			if (Switcher.comicsSound != null && audioSource != Switcher.comicsSound.GetComponent<AudioSource>())
			{
				UnityEngine.Object.Destroy(Switcher.comicsSound);
				Switcher.comicsSound = null;
			}
			if (PhotonNetwork.connected)
			{
				float num = 0f;
				num = Convert.ToSingle(PhotonNetwork.time) - audioSource.clip.length * (float)Mathf.FloorToInt(Convert.ToSingle(PhotonNetwork.time) / audioSource.clip.length);
				audioSource.time = num;
			}
			audioSource.Play();
		}
	}

	public void StopMusic(AudioSource audioSource)
	{
		if (!(audioSource == null))
		{
			audioSource.Stop();
		}
	}

	private IEnumerator PlayMusicInternal(AudioSource audioSource)
	{
		float targetVolume = 1f;
		audioSource.volume = 1f;
		audioSource.Play();
		currentAudioSource = audioSource;
		float startTime = Time.realtimeSinceStartup;
		float fadeTime = 0.5f;
		while (Time.realtimeSinceStartup - startTime <= fadeTime)
		{
			if (audioSource == null)
			{
				audioSource.volume = 1f;
				yield break;
			}
			audioSource.volume = targetVolume * (Time.realtimeSinceStartup - startTime) / fadeTime;
			Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ PlayMusicInternal " + audioSource.volume);
			yield return null;
		}
		audioSource.volume = 1f;
		Debug.Log("----------------------------------------------------------------- PlayMusicInternal " + audioSource.volume);
	}

	private IEnumerator StopMusicInternal(AudioSource audioSource)
	{
		float currentVolume = 1f;
		float startTime = Time.realtimeSinceStartup;
		float fadeTime = 0.5f;
		while (Time.realtimeSinceStartup - startTime <= fadeTime)
		{
			if (audioSource == null)
			{
				yield break;
			}
			audioSource.volume = currentVolume * (1f - (Time.realtimeSinceStartup - startTime) / fadeTime);
			Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ StopMusicInternal " + audioSource.volume);
			yield return null;
		}
		audioSource.volume = 0f;
		audioSource.Stop();
		currentAudioSource = null;
		audioSource.volume = 1f;
		Debug.Log("----------------------------------------------------------------- StopMusicInternal " + audioSource.volume);
	}

	private void PlayCurrentMusic()
	{
		if (currentAudioSource != null)
		{
			PlayMusic(currentAudioSource);
		}
	}

	private void PauseCurrentMusic()
	{
		if (currentAudioSource != null)
		{
			currentAudioSource.Pause();
		}
	}

	private void OnApplicationPause(bool pausing)
	{
		if (!pausing)
		{
			PlayCurrentMusic();
		}
		else
		{
			PauseCurrentMusic();
		}
	}

	public static void SetBackgroundClip(LobbyBackgroundClip clipType)
	{
		if (!(sharedMusic != null))
		{
			return;
		}
		AudioSource component = sharedMusic.GetComponent<AudioSource>();
		if (component != null)
		{
			string text = clipType.ToString().ToLower();
			if (SettedLobbyBackgrounClip.ToLower() != text && clipType != 0)
			{
				Storager.setString("lobby_background_music", clipType.ToString(), true);
				AudioClip clip = Resources.Load<AudioClip>("MenuMusic/menu_music_" + text);
				sharedMusic.Stop();
				component.clip = clip;
				sharedMusic.Play();
			}
		}
	}
}

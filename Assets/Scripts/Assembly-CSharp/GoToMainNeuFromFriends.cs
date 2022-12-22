using System;
using Rilisoft;
using UnityEngine;

[Obsolete]
internal sealed class GoToMainNeuFromFriends : MonoBehaviour
{
	private bool firstFrame = true;

	private void HandleClick()
	{
		ButtonClickSound.Instance.PlayClick();
		MenuBackgroundMusic.keepPlaying = true;
		LoadConnectScene.textureToShow = null;
		LoadConnectScene.sceneToLoad = Defs.MainMenuScene;
		LoadConnectScene.noteToShow = null;
		Singleton<SceneLoader>.Instance.LoadScene(Defs.PromSceneName);
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			firstFrame = false;
		}
		else if (!firstFrame)
		{
			HandleClick();
		}
	}
}

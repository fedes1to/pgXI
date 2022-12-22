using Rilisoft;
using UnityEngine;

public class SceneAndURLLoader : MonoBehaviour
{
	private PauseMenu m_PauseMenu;

	private void Awake()
	{
		m_PauseMenu = GetComponentInChildren<PauseMenu>();
	}

	public void SceneLoad(string sceneName)
	{
		m_PauseMenu.MenuOff();
		Singleton<SceneLoader>.Instance.LoadScene(sceneName);
	}

	public void LoadURL(string url)
	{
		Application.OpenURL(url);
	}
}

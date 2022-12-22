using Rilisoft;
using UnityEngine;

public class ReturnToMainMenu : MonoBehaviour
{
	private bool m_Levelloaded;

	public void Start()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void OnLevelWasLoaded(int level)
	{
		m_Levelloaded = true;
	}

	private void Update()
	{
		if (m_Levelloaded)
		{
			Canvas component = base.gameObject.GetComponent<Canvas>();
			component.enabled = false;
			component.enabled = true;
			m_Levelloaded = false;
		}
	}

	public void GoBackToMainMenu()
	{
		Debug.Log("going back to main menu");
		Singleton<SceneLoader>.Instance.LoadScene("MainMenu");
	}
}

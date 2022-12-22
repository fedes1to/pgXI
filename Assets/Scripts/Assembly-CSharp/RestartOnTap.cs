using Rilisoft;
using UnityEngine;

public class RestartOnTap : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.touchCount > 0)
		{
			Singleton<SceneLoader>.Instance.LoadScene("Level2");
		}
	}
}

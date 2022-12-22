using Rilisoft;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelReset : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
{
	public void OnPointerClick(PointerEventData data)
	{
		Singleton<SceneLoader>.Instance.LoadSceneAsync(Application.loadedLevelName);
	}

	private void Update()
	{
	}
}

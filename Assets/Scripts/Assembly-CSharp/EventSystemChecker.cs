using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemChecker : MonoBehaviour
{
	public GameObject eventSystem;

	private IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		if (!Object.FindObjectOfType<EventSystem>())
		{
			Object.Instantiate(eventSystem);
		}
	}
}

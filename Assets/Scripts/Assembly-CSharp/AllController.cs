using UnityEngine;

public class AllController : MonoBehaviour
{
	public static AllController instance;

	private void Awake()
	{
		Screen.orientation = ScreenOrientation.AutoRotation;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		instance = null;
	}
}

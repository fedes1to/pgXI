using UnityEngine;

internal sealed class MiscAppsMenu : MonoBehaviour
{
	public static MiscAppsMenu Instance;

	public HiddenSettings misc;

	public void UnloadMisc()
	{
		misc = null;
	}

	private void Awake()
	{
		Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}
}

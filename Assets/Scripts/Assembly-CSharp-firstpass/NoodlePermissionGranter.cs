using System;
using UnityEngine;

public class NoodlePermissionGranter : MonoBehaviour
{
	public enum NoodleAndroidPermission
	{
		WRITE_EXTERNAL_STORAGE,
		ACCESS_COARSE_LOCATION
	}

	private const string WRITE_EXTERNAL_STORAGE = "WRITE_EXTERNAL_STORAGE";

	private const string PERMISSION_GRANTED = "PERMISSION_GRANTED";

	private const string PERMISSION_DENIED = "PERMISSION_DENIED";

	private const string NOODLE_PERMISSION_GRANTER = "NoodlePermissionGranter";

	public static Action<bool> PermissionRequestCallback;

	public static EventHandler<EventArgs> PermissionRequestFinished;

	private static NoodlePermissionGranter instance;

	private static bool initialized;

	private static AndroidJavaClass noodlePermissionGranterClass;

	private static AndroidJavaObject activity;

	public static void GrantPermission(NoodleAndroidPermission permission)
	{
		if (!initialized)
		{
			initialize();
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			noodlePermissionGranterClass.CallStatic("grantPermission", activity, (int)permission);
		}
	}

	public void Awake()
	{
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (base.name != "NoodlePermissionGranter")
		{
			base.name = "NoodlePermissionGranter";
		}
	}

	private static void initialize()
	{
	}

	private void permissionRequestCallbackInternal(string message)
	{
		bool obj = message == "PERMISSION_GRANTED";
		if (PermissionRequestCallback != null)
		{
			PermissionRequestCallback(obj);
		}
	}
}

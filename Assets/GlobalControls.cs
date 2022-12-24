using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControls : MonoBehaviour
{
	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Update()
	{
		if (Application.platform == RuntimePlatform.WindowsEditor && Input.GetKey(",") && Input.GetKey(".") && Input.GetKey(";"))
		{
			PlayerPrefs.DeleteAll();
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			Screen.lockCursor = !Screen.lockCursor;
		}
	}
}

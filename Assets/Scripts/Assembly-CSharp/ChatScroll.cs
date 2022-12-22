using System;
using UnityEngine;

public class ChatScroll : MonoBehaviour
{
	public UITextListEdit _Textlist;

	private bool isTouch;

	private void Start()
	{
		base.transform.position = new Vector3(posNGUI.getPosX((float)Screen.width * 0.1f), posNGUI.getPosY((float)Screen.height * 0.03f), 1f);
	}

	private void Update()
	{
		if (!Application.isEditor)
		{
			int touchCount = Input.touchCount;
			for (int i = 0; i < touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				if (touch.phase == TouchPhase.Began)
				{
					isTouch = true;
					_Textlist.OnSelect(true);
				}
				if (touch.phase == TouchPhase.Ended)
				{
					isTouch = false;
					_Textlist.OnSelect(false);
				}
			}
		}
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				isTouch = true;
				_Textlist.OnSelect(true);
			}
			if (Input.GetMouseButtonUp(0))
			{
				isTouch = false;
				_Textlist.OnSelect(false);
			}
		}
	}

	private void LateUpdate()
	{
		if (isTouch)
		{
			_Textlist.OnScroll((float)Math.Round((0f - Input.GetAxis("Mouse Y")) / 5f, 1));
		}
	}
}

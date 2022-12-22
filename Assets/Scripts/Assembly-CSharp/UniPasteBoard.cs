using System;
using UnityEngine;

public class UniPasteBoard
{
	private static AndroidJavaClass _javaClass;

	private static AndroidJavaClass JavaClass
	{
		get
		{
			if (_javaClass == null)
			{
				try
				{
					_javaClass = new AndroidJavaClass("com.onevcat.UniPasteBoard.PasteBoard");
				}
				catch (Exception ex)
				{
					Debug.Log(ex.ToString());
				}
			}
			return _javaClass;
		}
	}

	public static string GetClipBoardString()
	{
		return androidGetClipBoardString();
	}

	public static void SetClipBoardString(string text)
	{
		androidSetClipBoardString(text);
	}

	private static string androidGetClipBoardString()
	{
		string result = null;
		if (JavaClass != null)
		{
			result = JavaClass.CallStatic<string>("getClipBoardString", new object[0]);
		}
		return result;
	}

	private static void androidSetClipBoardString(string text)
	{
		if (JavaClass != null)
		{
			JavaClass.CallStatic("setClipBoardString", text);
		}
	}
}

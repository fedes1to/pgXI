using System;
using UnityEngine;

public class NumberVersionLabel : MonoBehaviour
{
	private Version CurrentVersion
	{
		get
		{
			return GetType().Assembly.GetName().Version;
		}
	}

	private void Start()
	{
		UILabel component = GetComponent<UILabel>();
		if (component != null)
		{
			component.text = CurrentVersion.ToString();
		}
	}
}

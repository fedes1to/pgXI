using System.Collections.Generic;
using UnityEngine;

public class EnableNotifier : MonoBehaviour
{
	public List<EventDelegate> onEnable = new List<EventDelegate>();

	public bool isSoundFX;

	private void OnEnable()
	{
		if (!isSoundFX)
		{
			EventDelegate.Execute(onEnable);
		}
		else if (Defs.isSoundFX)
		{
			EventDelegate.Execute(onEnable);
		}
	}
}

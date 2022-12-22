using System;
using UnityEngine;

public class HatchingEndedCallback : MonoBehaviour
{
	public static event Action HatchingEnded;

	public void HatchingCompleted()
	{
		Action hatchingEnded = HatchingEndedCallback.HatchingEnded;
		if (hatchingEnded != null)
		{
			hatchingEnded();
		}
	}
}

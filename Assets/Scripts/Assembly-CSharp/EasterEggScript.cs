using System;
using UnityEngine;

public class EasterEggScript : MonoBehaviour
{
	private void Start()
	{
		if (DateTime.Now.Hour < 23 && DateTime.Now.Minute < 55)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}

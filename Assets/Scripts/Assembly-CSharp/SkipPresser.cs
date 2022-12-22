using System;
using UnityEngine;

public sealed class SkipPresser : MonoBehaviour
{
	public GameObject windowAnchor;

	public static event Action SkipPressed;

	private void Start()
	{
		base.gameObject.SetActive(false);
	}
}

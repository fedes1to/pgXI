using System;
using UnityEngine;

public class SkinInfo : MonoBehaviour
{
	[NonSerialized]
	public Texture skin;

	public string skinStr = string.Empty;

	public int price = 20;

	public string currency = "Coins";

	public string localizeKey = string.Empty;
}

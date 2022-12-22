using System;
using UnityEngine;

[Serializable]
public class MaterialToChange
{
	public string description = "description";

	public Color[] cicleColors;

	public Material[] materials;

	public float[] cicleLerp;

	public bool changecolor;

	[HideInInspector]
	public Color currentColor;

	[HideInInspector]
	public float currentLerp;
}

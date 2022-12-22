using System;
using UnityEngine;

[Serializable]
public class Scale : MonoBehaviour
{
	public ParticleEmitter[] particleEmitters;

	public float scale;

	[SerializeField]
	[HideInInspector]
	private float[] minsize;

	[HideInInspector]
	[SerializeField]
	private float[] maxsize;

	[HideInInspector]
	[SerializeField]
	private Vector3[] worldvelocity;

	[SerializeField]
	[HideInInspector]
	private Vector3[] localvelocity;

	[SerializeField]
	[HideInInspector]
	private Vector3[] rndvelocity;

	[HideInInspector]
	[SerializeField]
	private Vector3[] scaleBackUp;

	[HideInInspector]
	[SerializeField]
	private bool firstUpdate;

	public Scale()
	{
		scale = 1f;
		firstUpdate = true;
	}

	public virtual void UpdateScale()
	{
		int num = 0;
		if (firstUpdate)
		{
			minsize = new float[num];
			maxsize = new float[num];
			worldvelocity = new Vector3[num];
			localvelocity = new Vector3[num];
			rndvelocity = new Vector3[num];
			scaleBackUp = new Vector3[num];
		}
		firstUpdate = false;
	}

	public virtual void Main()
	{
	}
}

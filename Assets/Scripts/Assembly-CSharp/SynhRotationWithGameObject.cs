using UnityEngine;

public class SynhRotationWithGameObject : MonoBehaviour
{
	public new Transform gameObject;

	public bool transformPos;

	private Transform myTransform;

	public Vector3 addpos = Vector3.zero;

	private void Start()
	{
		myTransform = base.transform;
	}

	private void Update()
	{
		myTransform.rotation = gameObject.rotation;
		if (transformPos)
		{
			myTransform.position = gameObject.TransformPoint(addpos);
		}
	}
}

using UnityEngine;

public class CameraFacingBilloard : MonoBehaviour
{
	private Transform thisTransform;

	public bool Invert;

	private void Awake()
	{
		thisTransform = base.transform;
	}

	private void Update()
	{
		if (NickLabelController.currentCamera != null)
		{
			thisTransform.rotation = NickLabelController.currentCamera.transform.rotation;
			if (Invert)
			{
				thisTransform.rotation *= new Quaternion(1f, 180f, 1f, 1f);
			}
		}
	}
}

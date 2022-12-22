using UnityEngine;

public class HeadRotationScript : MonoBehaviour
{
	public Transform gunPoint;

	private float prevX;

	private void Start()
	{
		prevX = 0f;
	}

	private void LateUpdate()
	{
		Quaternion localRotation = default(Quaternion);
		float num = (0f - gunPoint.localRotation.eulerAngles.x) / 2f;
		num = ((!(num < -45f)) ? num : (num + 180f));
		num = Mathf.Lerp(prevX, num, 0.2f);
		localRotation.eulerAngles = new Vector3(0f, 0f, num);
		base.transform.localRotation = localRotation;
		prevX = num;
	}
}

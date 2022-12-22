using UnityEngine;

public class ReversScale : MonoBehaviour
{
	public bool x;

	public bool y;

	public bool z;

	private void Update()
	{
		if (x && base.transform.lossyScale.x < 0f)
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
		}
		if (y && base.transform.lossyScale.y < 0f)
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y * -1f, base.transform.localScale.z);
		}
		if (z && base.transform.lossyScale.z < 0f)
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z * -1f);
		}
	}
}

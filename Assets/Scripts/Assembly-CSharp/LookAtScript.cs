using UnityEngine;

public class LookAtScript : MonoBehaviour
{
	public Transform t_target;

	private void Update()
	{
		base.transform.LookAt(t_target);
	}
}

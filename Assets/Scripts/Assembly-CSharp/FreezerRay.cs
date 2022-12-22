using UnityEngine;

public class FreezerRay : MonoBehaviour
{
	private Player_move_c mc;

	public float lifetime = 0.1f;

	public float timeLeft;

	private Transform target;

	public float Length
	{
		set
		{
			base.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(1, new Vector3(0f, 0f, value));
		}
	}

	private void OnEnable()
	{
		timeLeft += lifetime;
	}

	private void Update()
	{
		timeLeft -= Time.deltaTime;
		if (timeLeft <= 0f)
		{
			GetComponent<RayAndExplosionsStackItem>().Deactivate();
		}
		else if (mc != null && target != null && target.parent != null && target.parent.parent != null)
		{
			base.transform.position = target.position;
			base.transform.forward = target.parent.parent.forward;
		}
	}

	public void Activate(Player_move_c move_c, Transform gunFlash)
	{
		mc = move_c;
		target = gunFlash;
	}

	public void UpdatePosition(float length)
	{
		timeLeft += lifetime;
		Length = length;
	}

	private void OnDisable()
	{
		if (mc != null)
		{
			mc.RemoveRay(this);
		}
	}
}

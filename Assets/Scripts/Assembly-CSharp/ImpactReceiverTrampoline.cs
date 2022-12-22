using UnityEngine;

public class ImpactReceiverTrampoline : MonoBehaviour
{
	private float mass = 1f;

	private Vector3 impact = Vector3.zero;

	private CharacterController character;

	private void Start()
	{
		character = GetComponent<CharacterController>();
	}

	private void Update()
	{
		if (impact.magnitude > 0.2f)
		{
			character.Move(impact * Time.deltaTime);
		}
		else
		{
			Object.Destroy(this);
		}
		impact = Vector3.Lerp(impact, Vector3.zero, 1f * Time.deltaTime);
	}

	public void AddImpact(Vector3 dir, float force)
	{
		impact += dir.normalized * force / mass;
	}
}

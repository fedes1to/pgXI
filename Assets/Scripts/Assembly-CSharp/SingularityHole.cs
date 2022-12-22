using UnityEngine;

public class SingularityHole : MonoBehaviour
{
	public Player_move_c owner;

	public float outRadius = 15f;

	public float inRadius = 2f;

	public float maxForce = 10f;

	public float minForce = 1f;

	private void OnEnable()
	{
		Initializer.singularities.Add(this);
	}

	private void DestroyByNetwork()
	{
		PhotonNetwork.Destroy(base.gameObject);
	}

	private void OnDisable()
	{
		Initializer.singularities.Remove(this);
	}

	public float GetForce(float distance)
	{
		if (distance < outRadius * outRadius && distance > inRadius * inRadius)
		{
			return Mathf.Clamp(minForce + (maxForce - minForce) * (1f - distance / (outRadius * outRadius)), minForce, maxForce);
		}
		if (distance < inRadius * inRadius)
		{
			return -1f;
		}
		return 0f;
	}
}

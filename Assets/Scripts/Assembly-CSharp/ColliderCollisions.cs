using UnityEngine;

public class ColliderCollisions : MonoBehaviour
{
	public bool isCollision;

	public Transform thisTransform;

	private int countColision;

	private void Awake()
	{
		thisTransform = base.transform;
	}

	private void Update()
	{
		isCollision = countColision > 0;
		countColision = 0;
	}

	private void OnTriggerStay(Collider other)
	{
		countColision++;
	}
}

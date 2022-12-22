using UnityEngine;

public class WrapContent3D : MonoBehaviour
{
	public Transform[] wrappedObjects;

	public float maxDistance;

	public float deltaX;

	public Transform center;

	private void Update()
	{
		for (int i = 0; i < wrappedObjects.Length; i++)
		{
			float f = (wrappedObjects[i].position.x - center.position.x) / 0.002604167f;
			float num = Mathf.Clamp01((maxDistance - Mathf.Abs(f)) / maxDistance);
			wrappedObjects[i].localScale = Vector3.Lerp(Vector3.one, Vector3.zero, 0.7f - num);
			wrappedObjects[i].gameObject.SetActive(Mathf.Abs(f) < maxDistance);
		}
	}
}

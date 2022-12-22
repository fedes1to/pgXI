using UnityEngine;

[ExecuteInEditMode]
public class SortOrder : MonoBehaviour
{
	public int sortOrder;

	private void Start()
	{
	}

	private void Update()
	{
		GetComponent<Renderer>().sortingOrder = sortOrder;
	}
}

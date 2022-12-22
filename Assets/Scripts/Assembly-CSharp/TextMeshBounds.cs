using UnityEngine;

[ExecuteInEditMode]
public class TextMeshBounds : MonoBehaviour
{
	private GameObject[] outlines = new GameObject[4];

	private Vector3 boundSize;

	public Vector3 textBound = new Vector3(1f, 0.4f, 0f);

	private TextMesh text;

	private string oldText = string.Empty;

	private bool outlined;

	private void Start()
	{
		text = GetComponent<TextMesh>();
	}

	private void Update()
	{
		if (!text.text.Equals(oldText) && text.text.Length > 1)
		{
			Quaternion rotation = base.transform.rotation;
			base.transform.rotation = new Quaternion
			{
				eulerAngles = Vector3.zero
			};
			ResizeTextByWidth();
			base.transform.rotation = rotation;
		}
		oldText = text.text;
	}

	private void ResizeTextByWidth()
	{
		float x = base.transform.lossyScale.x;
		Vector3 size = text.GetComponent<Renderer>().bounds.size;
		while ((size.x > textBound.x * x || size.y > textBound.y * x) && (double)text.characterSize > 0.01)
		{
			text.characterSize -= 0.003f;
			size = text.GetComponent<Renderer>().bounds.size;
		}
		while (size.x < textBound.x * x && size.y < textBound.y * x)
		{
			text.characterSize += 0.003f;
			size = text.GetComponent<Renderer>().bounds.size;
		}
	}

	private void OnDrawGizmos()
	{
		Matrix4x4 matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, base.transform.lossyScale);
		Gizmos.matrix = matrix;
		Gizmos.DrawWireCube(Vector3.zero, boundSize);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(Vector3.zero, textBound);
	}
}

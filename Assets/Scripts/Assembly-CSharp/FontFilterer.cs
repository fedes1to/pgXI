using UnityEngine;

[ExecuteInEditMode]
public class FontFilterer : MonoBehaviour
{
	private void Start()
	{
		TextMesh component = GetComponent<TextMesh>();
		component.font.material.mainTexture.filterMode = FilterMode.Point;
	}
}

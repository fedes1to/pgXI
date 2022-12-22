using UnityEngine;

public class UvScroller : MonoBehaviour
{
	public float scrollSpeed = -0.5f;

	private float ScrollX;

	private Renderer rendererGlossnes;

	private void Start()
	{
		if (GetComponent<Renderer>() != null)
		{
			rendererGlossnes = GetComponent<Renderer>();
		}
	}

	private void Update()
	{
		ScrollX += Time.unscaledDeltaTime * scrollSpeed;
		if (Mathf.Abs(ScrollX) >= 1f)
		{
			ScrollX = 0f;
		}
		if (rendererGlossnes.material.HasProperty("_GlossTex"))
		{
			rendererGlossnes.material.SetTextureOffset("_GlossTex", new Vector2(ScrollX, 0f));
		}
		else if (rendererGlossnes.materials.Length > 1 && rendererGlossnes.materials[1].HasProperty("_GlossTex"))
		{
			rendererGlossnes.materials[1].SetTextureOffset("_GlossTex", new Vector2(ScrollX, 0f));
		}
	}
}

using UnityEngine;

public sealed class Button_light : MonoBehaviour
{
	public UITexture lightTexture;

	private void Start()
	{
		if (lightTexture != null)
		{
			lightTexture.enabled = false;
		}
	}

	private void OnPress(bool isDown)
	{
		if (lightTexture != null)
		{
			lightTexture.enabled = isDown;
		}
	}
}

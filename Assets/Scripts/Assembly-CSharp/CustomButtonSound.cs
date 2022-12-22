using UnityEngine;

public class CustomButtonSound : MonoBehaviour
{
	public AudioClip clickSound;

	private void OnClick()
	{
		if (Defs.isSoundFX)
		{
			NGUITools.PlaySound(clickSound, 1f, 1f);
		}
	}
}

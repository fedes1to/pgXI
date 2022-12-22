using System.Collections;
using UnityEngine;

public class PreloadTexture : MonoBehaviour
{
	public string pathTexture;

	public bool clearMemoryOnUnload = true;

	private UITexture nguiTexture;

	private void OnEnable()
	{
		if (Device.IsLoweMemoryDevice)
		{
			if (nguiTexture == null)
			{
				nguiTexture = GetComponent<UITexture>();
			}
			if (nguiTexture != null)
			{
				StartCoroutine(Crt_LoadTexture());
			}
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private IEnumerator Crt_LoadTexture()
	{
		while (string.IsNullOrEmpty(pathTexture))
		{
			yield return null;
		}
		Texture needTx = Resources.Load<Texture>(pathTexture);
		if (nguiTexture != null)
		{
			nguiTexture.mainTexture = needTx;
		}
		yield return null;
	}

	private void OnDisable()
	{
		if (Device.IsLoweMemoryDevice)
		{
			if (nguiTexture != null)
			{
				nguiTexture.mainTexture = null;
			}
			ActivityIndicator.ClearMemory();
		}
	}
}

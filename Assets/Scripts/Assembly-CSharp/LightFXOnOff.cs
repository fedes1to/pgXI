using Rilisoft;
using UnityEngine;

public class LightFXOnOff : MonoBehaviour
{
	private void Start()
	{
		if (Device.isWeakDevice || Application.platform == RuntimePlatform.Android || Tools.RuntimePlatform == RuntimePlatform.MetroPlayerX64)
		{
			base.gameObject.SetActive(false);
		}
	}
}

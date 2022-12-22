using System.Reflection;
using UnityEngine;

public class RayAndExplosionsStackItem : MonoBehaviour
{
	public string myName;

	public float timeDeactivate = 1f;

	private bool isNotAutoEnd;

	private void Start()
	{
		isNotAutoEnd = GetComponent<FreezerRay>() == null;
		if (isNotAutoEnd)
		{
			Invoke("Deactivate", timeDeactivate);
		}
	}

	private void OnEnable()
	{
		isNotAutoEnd = GetComponent<FreezerRay>() == null;
		if ((bool)GetComponent<AudioSource>() && Defs.isSoundFX)
		{
			GetComponent<AudioSource>().Play();
		}
		Invoke("Deactivate", timeDeactivate);
	}

	[Obfuscation(Exclude = true)]
	public void Deactivate()
	{
		CancelInvoke("Deactivate");
		if (RayAndExplosionsStackController.sharedController != null)
		{
			if ((bool)GetComponent<AudioSource>())
			{
				GetComponent<AudioSource>().Stop();
			}
			RayAndExplosionsStackController.sharedController.ReturnObjectFromName(base.gameObject, myName);
		}
	}
}

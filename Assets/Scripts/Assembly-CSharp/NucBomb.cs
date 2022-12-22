using System.Collections;
using UnityEngine;

public class NucBomb : MonoBehaviour
{
	public float BeforeActivate = 12f;

	public float BeforeDestroy = 90f;

	private IEnumerator Start()
	{
		GetComponent<AudioSource>().Play();
		GetComponent<AudioSource>().mute = !Defs.isSoundFX;
		yield return new WaitForSeconds(BeforeActivate);
		base.transform.GetChild(0).gameObject.SetActive(true);
		yield return new WaitForSeconds(Mathf.Max(0f, BeforeDestroy - BeforeActivate));
		Object.Destroy(base.gameObject);
	}

	private void FixedUpdate()
	{
		GetComponent<AudioSource>().mute = !Defs.isSoundFX;
	}
}

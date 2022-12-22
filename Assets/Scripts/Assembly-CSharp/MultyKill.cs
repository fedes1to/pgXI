using UnityEngine;

public class MultyKill : MonoBehaviour
{
	public AudioSource multikillSound;

	public UIPlayTween multikillTween;

	public UISprite scorePict;

	private string scorePictTempName;

	private void OnEnable()
	{
		if (Defs.isSoundFX)
		{
			multikillSound.Play();
		}
		multikillTween.Play(true);
	}

	public void PlayTween()
	{
		base.transform.GetChild(0).gameObject.SetActive(false);
		base.transform.GetChild(0).gameObject.SetActive(true);
		if (Defs.isSoundFX)
		{
			multikillSound.Stop();
			multikillSound.Play();
		}
		multikillTween.Play(true);
	}
}

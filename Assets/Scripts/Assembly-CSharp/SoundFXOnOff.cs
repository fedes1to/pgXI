using UnityEngine;

public class SoundFXOnOff : MonoBehaviour
{
	private GameObject soundFX;

	private bool _isWeakdevice;

	private void Start()
	{
		_isWeakdevice = Device.isWeakDevice;
		if (_isWeakdevice && !Application.isEditor)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		soundFX = base.transform.GetChild(0).gameObject;
		if (Defs.isSoundFX)
		{
			soundFX.SetActive(true);
		}
	}

	private void Update()
	{
		if (!_isWeakdevice && soundFX.activeSelf != Defs.isSoundFX)
		{
			soundFX.SetActive(Defs.isSoundFX);
		}
	}
}

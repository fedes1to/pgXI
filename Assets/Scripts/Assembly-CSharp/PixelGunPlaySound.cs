public class PixelGunPlaySound : UIPlaySound
{
	private void OnClick()
	{
		if (Defs.isSoundFX && /*base.canPlay &&*/ trigger == Trigger.OnClick)
		{
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}
}

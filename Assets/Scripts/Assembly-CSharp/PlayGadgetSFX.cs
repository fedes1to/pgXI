using UnityEngine;

public class PlayGadgetSFX : MonoBehaviour
{
	public enum IfDefsLow
	{
		noParticles,
		noSpriteSfx,
		both,
		doNothing
	}

	public IfDefsLow ifLowDevice;

	public float time = 1f;

	public AnimationCurve alphaChange = new AnimationCurve
	{
		keys = new Keyframe[3]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.5f, 1f),
			new Keyframe(1f, 0f)
		},
		postWrapMode = WrapMode.Loop,
		preWrapMode = WrapMode.Loop
	};

	public UIWidget sfxSpriteContainer;

	public GameObject[] particles;

	private float timer;

	private bool isPlaying;

	private bool isLow;

	private bool isZone;

	private bool isInZone;

	private float tempZoneTime;

	[Header("Включается для проигрывания анимации")]
	public GameObject sfx;

	private void Start()
	{
		isLow = Device.isPixelGunLow;
	}

	public void Play()
	{
		if (!isLow || (isLow && ifLowDevice != IfDefsLow.both))
		{
			timer = Time.time + time;
			isPlaying = true;
			sfx.SetActive(true);
		}
	}

	public void Play(float playTime)
	{
		time = playTime;
		Play();
	}

	public void Play(bool inZone)
	{
		timer = Time.time + time;
		isZone = true;
		isInZone = inZone;
		isPlaying = true;
		sfx.SetActive(true);
	}

	public void Stop()
	{
		isPlaying = false;
		sfx.SetActive(false);
	}

	private void Update()
	{
		if (!isPlaying)
		{
			return;
		}
		if (isLow)
		{
			sfxSpriteContainer.gameObject.SetActive(ifLowDevice != IfDefsLow.noSpriteSfx);
			if (particles.Length > 0)
			{
				for (int i = 0; i < particles.Length; i++)
				{
					particles[i].SetActive(ifLowDevice != IfDefsLow.noParticles);
				}
			}
		}
		if (!isZone)
		{
			float num = Mathf.Max(0f, timer - Time.time);
			sfxSpriteContainer.alpha = alphaChange.Evaluate(1f - num / time);
			if (timer < Time.time)
			{
				Stop();
			}
			return;
		}
		if (isInZone)
		{
			float num2 = Mathf.Max(0f, timer - tempZoneTime - Time.time);
			sfxSpriteContainer.alpha = alphaChange.Evaluate(1f - num2 / time);
			return;
		}
		float num3 = (tempZoneTime = Mathf.Max(0f, timer - Time.time));
		sfxSpriteContainer.alpha = alphaChange.Evaluate(num3 / time);
		if (num3 == 0f)
		{
			isZone = false;
			Stop();
		}
	}
}

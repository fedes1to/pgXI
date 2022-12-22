using Rilisoft;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class AdmobPerelivWindow : MonoBehaviour
{
	public enum WinState
	{
		none,
		on,
		show,
		off
	}

	public WinState state;

	private float timeOn = 0.2f;

	public static Texture admobTexture = null;

	public static string admobUrl = string.Empty;

	public UITexture adTexture;

	public GameObject closeAnchor;

	public UISprite closeSprite;

	public UITexture lightTexture;

	public UISprite closeSpriteAndr;

	private Transform _transform;

	public static string Context { get; set; }

	private Transform myTransform
	{
		get
		{
			if (_transform == null)
			{
				_transform = base.transform;
			}
			return _transform;
		}
	}

	private bool NeedSmoothShow
	{
		get
		{
			return false;
		}
	}

	private void Awake()
	{
		_transform = base.transform;
		closeSprite.gameObject.SetActive(BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer);
		closeSpriteAndr.gameObject.SetActive(BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer);
	}

	private void Start()
	{
		if (closeAnchor != null)
		{
			closeAnchor.transform.localPosition = new Vector3((float)(-Screen.width / 2) * 768f / (float)Screen.height, 384f, 0f);
		}
	}

	private void Update()
	{
		if (state == WinState.on && myTransform.localPosition.y < 0f)
		{
			float y = myTransform.localPosition.y;
			y += 770f / timeOn * Time.deltaTime;
			if (y > 0f)
			{
				y = 0f;
				state = WinState.show;
			}
			myTransform.localPosition = new Vector3(0f, y, 0f);
		}
		if (state != WinState.off || !(myTransform.localPosition.y > -770f))
		{
			return;
		}
		float y2 = myTransform.localPosition.y;
		y2 -= 770f / timeOn * Time.deltaTime;
		if (y2 < -770f)
		{
			y2 = -770f;
			state = WinState.none;
			base.gameObject.SetActive(false);
			adTexture.mainTexture = null;
			if (admobTexture != null)
			{
				Object.Destroy(admobTexture);
			}
			admobTexture = null;
			admobUrl = string.Empty;
		}
		myTransform.localPosition = new Vector3(0f, y2, 0f);
	}

	public void Show()
	{
		if (state != 0)
		{
			return;
		}
		if (admobTexture == null)
		{
			Debug.LogWarningFormat("AdmobTexture is null.");
			return;
		}
		float num = admobTexture.width;
		float num2 = admobTexture.height;
		if (num2 / num >= (float)Screen.height / (float)Screen.width)
		{
			num = num * 768f / num2;
			num2 = 768f;
		}
		else
		{
			num2 = num2 * (768f * (float)Screen.width) / ((float)Screen.height * num);
			num = 768f * (float)Screen.width / (float)Screen.height;
		}
		if (adTexture != null)
		{
			adTexture.keepAspectRatio = UIWidget.AspectRatioSource.Free;
			adTexture.mainTexture = admobTexture;
			adTexture.width = Mathf.RoundToInt(num);
			adTexture.height = Mathf.RoundToInt(num2);
		}
		else
		{
			Debug.LogWarning("AdTexture is null.");
		}
		if (NeedSmoothShow)
		{
			state = WinState.on;
			myTransform.localPosition = new Vector3(0f, -770f, 0f);
			float num3 = 44f;
			float f = 242f;
			float num4 = 30f;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
			}
			num3 = num3 * 768f / (float)Screen.height;
			num4 = num4 * 768f / (float)Screen.height;
			closeSprite.width = Mathf.RoundToInt(num3);
			closeSprite.height = Mathf.RoundToInt(num3);
			closeSprite.transform.localPosition = new Vector3(num4, 0f - num4, 0f);
			if (lightTexture != null)
			{
				lightTexture.width = Mathf.RoundToInt(f);
				lightTexture.height = Mathf.RoundToInt(f);
			}
		}
		else
		{
			myTransform.localPosition = new Vector3(0f, 0f, 0f);
			state = WinState.show;
		}
	}

	public void Hide()
	{
		if (state != WinState.show)
		{
			return;
		}
		if (NeedSmoothShow)
		{
			state = WinState.off;
			return;
		}
		adTexture.mainTexture = null;
		if (admobTexture != null)
		{
			Object.Destroy(admobTexture);
		}
		admobTexture = null;
		admobUrl = string.Empty;
		state = WinState.none;
		base.gameObject.SetActive(false);
	}
}

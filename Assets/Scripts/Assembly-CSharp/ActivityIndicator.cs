using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Rilisoft;
using UnityEngine;

public sealed class ActivityIndicator : MonoBehaviour
{
	internal const string DefaultLegendLabel = "PLEASE REBOOT YOUR DEVICE IF FROZEN.";

	public static ActivityIndicator instance;

	public float rotSpeed = 180f;

	private Vector3 vectRotateSpeed;

	public string text;

	public Camera needCam;

	public GameObject panelWindowLoading;

	public GameObject panelIndicator;

	public GameObject objIndicator;

	public GameObject panelProgress;

	public UILabel lbLoading;

	public UILabel lbPercentLoading;

	public UILabel legendLabel;

	public UITexture[] txFon;

	public UITexture txProgressBar;

	private static float curPers;

	private bool canClearMemory = true;

	public static float LoadingProgress
	{
		get
		{
			return curPers;
		}
		set
		{
			if (instance != null)
			{
				curPers = value;
				curPers = Mathf.Clamp01(curPers);
				if (curPers < 0f)
				{
					curPers = 0f;
				}
				if (curPers > 1f)
				{
					curPers = 1f;
				}
				if (instance.txProgressBar != null)
				{
					instance.txProgressBar.fillAmount = curPers;
				}
				if ((bool)instance.lbPercentLoading)
				{
					instance.lbPercentLoading.text = string.Format("{0}%", Mathf.RoundToInt(curPers * 100f));
				}
			}
		}
	}

	public static bool IsShowWindowLoading
	{
		set
		{
			if (instance != null)
			{
				if (!value && instance.txFon != null)
				{
					instance.txFon[0].mainTexture = null;
				}
				if (instance.panelWindowLoading != null)
				{
					instance.panelWindowLoading.SetActive(value);
				}
			}
		}
	}

	public static bool IsActiveIndicator
	{
		get
		{
			if (instance == null || instance.panelIndicator == null)
			{
				return false;
			}
			return instance.panelIndicator.activeSelf;
		}
		set
		{
			if (!(instance == null))
			{
				if (instance.panelIndicator != null)
				{
					instance.panelIndicator.SetActive(value);
				}
				if (instance.needCam != null)
				{
					instance.needCam.Render();
				}
				if (!value)
				{
					instance.HandleLocalizationChanged();
				}
			}
		}
	}

	public void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		vectRotateSpeed = new Vector3(0f, rotSpeed, 0f);
		LocalizationStore.AddEventCallAfterLocalize(HandleLocalizationChanged);
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
		{
			base.gameObject.AddComponent<PurchasesSynchronizerListener>();
		}
	}

	private void Start()
	{
		OnEnable();
		lbLoading.GetComponent<Localize>().enabled = true;
		if (Launcher.UsingNewLauncher)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (objIndicator != null)
		{
			objIndicator.transform.Rotate(vectRotateSpeed * Time.unscaledDeltaTime);
		}
	}

	private void OnDestroy()
	{
		instance = null;
		LocalizationStore.DelEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void HandleLocalizationChanged()
	{
		if (lbLoading != null)
		{
			text = LocalizationStore.Get("Key_0853");
			lbLoading.text = text;
		}
	}

	private void OnEnable()
	{
		HandleLocalizationChanged();
	}

	public static void SetLoadingFon(Texture needFon)
	{
		if (!(instance == null) && !(instance.txFon[0] == null))
		{
			instance.txFon[0].mainTexture = needFon;
		}
	}

	public IEnumerable<float> ReplaceLoadingFon(Texture needFon, float duration)
	{
		txFon[1].mainTexture = needFon;
		txFon[1].alpha = 0f;
		float _curDuration = 0f;
		yield return 0f;
		while (_curDuration < duration)
		{
			_curDuration += Time.deltaTime;
			float _alpha = _curDuration / duration;
			Mathf.Min(_alpha, 1f);
			txFon[1].alpha = _alpha;
			yield return _alpha;
		}
		txFon[1].mainTexture = null;
		txFon[0].mainTexture = needFon;
	}

	public static void SetActiveWithCaption(string caption)
	{
		if (instance != null && instance.lbLoading != null)
		{
			instance.lbLoading.text = caption ?? string.Empty;
		}
		IsActiveIndicator = true;
	}

	public static void ClearMemory()
	{
		if (instance != null && instance.canClearMemory)
		{
			instance.StartCoroutine(instance.Crt_ClearMemory());
		}
	}

	private IEnumerator Crt_ClearMemory()
	{
		if (canClearMemory)
		{
			canClearMemory = false;
			yield return null;
			meminfo.gc_Collect();
			yield return null;
			Resources.UnloadUnusedAssets();
			yield return null;
			canClearMemory = true;
		}
	}
}

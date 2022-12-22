using System;
using System.Collections;
using Rilisoft;
using UnityEngine;

public sealed class ReplaceAdmobPerelivController : MonoBehaviour
{
	private Texture2D _image;

	private string _adUrl;

	private static int _timesWantToShow = -1;

	private static int _timesShown;

	private long _timeSuspended;

	public static ReplaceAdmobPerelivController sharedController { get; private set; }

	public static bool ShouldShowAtThisTime
	{
		get
		{
			if (PromoActionsManager.ReplaceAdmobPereliv == null)
			{
				return false;
			}
			if (PromoActionsManager.ReplaceAdmobPereliv.ShowEveryTimes <= 0)
			{
				return false;
			}
			return _timesWantToShow % PromoActionsManager.ReplaceAdmobPereliv.ShowEveryTimes == 0;
		}
	}

	public Texture2D Image
	{
		get
		{
			return _image;
		}
	}

	public string AdUrl
	{
		get
		{
			return _adUrl;
		}
	}

	public bool DataLoaded
	{
		get
		{
			return _image != null && _adUrl != null;
		}
	}

	public bool DataLoading { get; private set; }

	public bool ShouldShowInLobby { get; set; }

	public static void IncreaseTimesCounter()
	{
		_timesWantToShow++;
	}

	public static void TryShowPereliv(string context)
	{
		if (sharedController != null && sharedController.Image != null && sharedController.AdUrl != null)
		{
			AdmobPerelivWindow.admobTexture = sharedController.Image;
			AdmobPerelivWindow.admobUrl = sharedController.AdUrl;
			AdmobPerelivWindow.Context = context;
			PlayerPrefs.SetString(Defs.LastTimeShowBanerKey, DateTime.UtcNow.ToString("s"));
			FyberFacade.Instance.IncrementCurrentDailyInterstitialCount();
			_timesShown++;
			InterstitialCounter.Instance.IncrementFakeInterstitialCount();
		}
	}

	public void DestroyImage()
	{
		if (Image != null)
		{
			_image = null;
		}
	}

	public void LoadPerelivData()
	{
		try
		{
			if (DataLoading)
			{
				Debug.LogWarning("ReplaceAdmobPerelivController: data is already loading. returning...");
				return;
			}
			if (_image != null)
			{
				UnityEngine.Object.Destroy(_image);
			}
			_image = null;
			_adUrl = null;
			if (AdsConfigManager.Instance.LastLoadedConfig == null)
			{
				Debug.LogWarning("LoadPerelivData(): AdsConfigManager.Instance.LastLoadedConfig == null");
				return;
			}
			FakeInterstitialConfigMemento fakeInterstitialConfig = AdsConfigManager.Instance.LastLoadedConfig.FakeInterstitialConfig;
			int count = fakeInterstitialConfig.ImageUrls.Count;
			if (count <= 0)
			{
				Debug.LogWarning("LoadPerelivData(): fakeInterstitialConfig.ImageUrls.Count == 0");
				return;
			}
			int index = UnityEngine.Random.Range(0, count);
			StartCoroutine(LoadDataCoroutine(fakeInterstitialConfig, index));
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private string GetImageURLForOurQuality(string urlString)
	{
		string value = string.Empty;
		if (Screen.height >= 500)
		{
			value = "-Medium";
		}
		if (Screen.height >= 900)
		{
			value = "-Hi";
		}
		urlString = urlString.Insert(urlString.LastIndexOf("."), value);
		return urlString;
	}

	private IEnumerator LoadDataCoroutine(FakeInterstitialConfigMemento fakeInterstitialConfig, int index)
	{
		DataLoading = true;
		if (fakeInterstitialConfig.ImageUrls.Count == 0)
		{
			Debug.LogWarning("LoadDataCoroutine(): fakeInterstitialConfig.ImageUrls.Count == 0");
			yield break;
		}
		string imageUrl = fakeInterstitialConfig.ImageUrls[index % fakeInterstitialConfig.ImageUrls.Count];
		string replaceAdmobUrl = GetImageURLForOurQuality(imageUrl);
		WWW imageRequest = Tools.CreateWwwIfNotConnected(replaceAdmobUrl);
		if (imageRequest == null)
		{
			DataLoading = false;
			yield break;
		}
		yield return imageRequest;
		if (!string.IsNullOrEmpty(imageRequest.error))
		{
			Debug.LogWarningFormat("ReplaceAdmobPerelivController: {0}", imageRequest.error);
			DataLoading = false;
			yield break;
		}
		if (!imageRequest.texture)
		{
			DataLoading = false;
			Debug.LogWarning("ReplaceAdmobPerelivController: imageRequest.texture = null. returning...");
			yield break;
		}
		if (imageRequest.texture.width < 20)
		{
			DataLoading = false;
			Debug.LogWarning("ReplaceAdmobPerelivController: imageRequest.texture is dummy. returning...");
			yield break;
		}
		_image = imageRequest.texture;
		if (fakeInterstitialConfig.RedirectUrls.Count == 0)
		{
			Debug.LogWarning("LoadDataCoroutine(): fakeInterstitialConfig.RedirectUrls.Count == 0");
			yield break;
		}
		_adUrl = fakeInterstitialConfig.RedirectUrls[index % fakeInterstitialConfig.RedirectUrls.Count];
		DataLoading = false;
	}

	private void Awake()
	{
		sharedController = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnApplicationPause(bool pausing)
	{
		if (!pausing)
		{
			if (PromoActionsManager.CurrentUnixTime - _timeSuspended > 3600)
			{
				_timesShown = 0;
				InterstitialCounter.Instance.Reset();
			}
		}
		else
		{
			_timeSuspended = PromoActionsManager.CurrentUnixTime;
		}
	}
}

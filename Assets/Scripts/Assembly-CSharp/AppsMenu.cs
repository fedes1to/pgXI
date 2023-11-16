using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Rilisoft;
using UnityEngine;
using UnityEngine.SceneManagement;

[Obfuscation(Exclude = true)]
internal sealed class AppsMenu : MonoBehaviour
{
	private const string _suffix = "Scene";

	public Texture androidFon;

	public Texture riliFon;

	public Texture commicsFon;

	public Material fadeMaterial;

	public GameObject activityIndikatorPrefab;

	public string intendedSignatureHash;

	private Texture currentFon;

	private static Material m_Material;

	private static int _startFrameIndex;

	internal volatile object _preventAggressiveOptimisation;

	private static volatile uint _preventInlining = 3565584061u;

	private IDisposable _backSubscription;

	private Lazy<string> _expansionFilePath = new Lazy<string>(GooglePlayDownloader.GetExpansionFilePath);

	private readonly TaskCompletionSource<bool> _storagePermissionGrantedPromise = new TaskCompletionSource<bool>();

	private bool _storagePermissionRequested;

	private TaskCompletionSource<string> _fetchObbPromise;

	internal static bool ApplicationBinarySplitted
	{
		get
		{
			return true;
		}
	}

	private Task<bool> StoragePermissionFuture
	{
		get
		{
			return _storagePermissionGrantedPromise.Task;
		}
	}

	private Task<string> FetchObbFuture
	{
		get
		{
			if (_fetchObbPromise == null)
			{
				return null;
			}
			return _fetchObbPromise.Task;
		}
	}

	internal IEnumerable<float> AppsMenuAwakeCoroutine()
	{
		yield return 0.1f;
		Device.isPixelGunLow = Device.isPixelGunLowDevice;
		Application.targetFrameRate = ((!GlobalGameController.is60FPSEnable) ? 30 : 60);
		_startFrameIndex = Time.frameCount;
		yield return 0.2f;
		if (!Launcher.UsingNewLauncher)
		{
			m_Material = fadeMaterial;
		}
		if ((float)Screen.width / (float)Screen.height > 1.7777778f)
		{
			Screen.SetResolution(Mathf.RoundToInt((float)Screen.height * 16f / 9f), Mathf.RoundToInt(Screen.height), false);
		}
	}

	private static IEnumerator MeetTheCoroutine(string sceneName, long abuseTicks, long nowTicks)
	{
		TimeSpan timespan = TimeSpan.FromTicks(Math.Abs(nowTicks - abuseTicks));
		if (Defs.IsDeveloperBuild)
		{
			if (timespan.TotalMinutes < 3.0)
			{
				yield break;
			}
		}
		else if (timespan.TotalDays < 1.0)
		{
			yield break;
		}
		System.Random prng = new System.Random(nowTicks.GetHashCode());
		float delaySeconds = prng.Next(15, 60);
		yield return new WaitForSeconds(delaySeconds);
		SceneManager.LoadScene(sceneName);
	}

	private static string GetAbuseKey_53232de5(uint pad)
	{
		uint num = 0x97C95CDCu ^ pad;
		_preventInlining++;
		return num.ToString("x");
	}

	private static string GetAbuseKey_21493d18(uint pad)
	{
		uint num = 0xE5A34C21u ^ pad;
		_preventInlining++;
		return num.ToString("x");
	}

	private static string GetTerminalSceneName_4de1(uint gamma)
	{
		return "Closing4de1Scene".Replace(gamma.ToString("x"), string.Empty);
	}

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(Application.Quit, "AppsMenu");
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void LoadModData()
	{
		if (Storager.getInt("loadedOneTimeModData", false) == 0)
		{
			Storager.setInt(Defs.IsFacebookLoginRewardaGained, 1, true);
			foreach (string str in WeaponManager.GotchaGuns)
			{
				Storager.setInt(str, 1, true);
			}
			Storager.setInt("currentLevel31", 1, false);
			Storager.setInt("Coins", 9999999, false);
			Storager.setInt("GemsCurrency", 9999999, false);
			Storager.setInt("loadedOneTimeModData", 1, false);
		}
	}

	private void Awake()
	{
		LoadModData();
		PhotonNetwork.PhotonServerSettings.UseCloud("4d2df5e9-5172-44a0-bf8b-df5346a54393", 0);
		LogsManager.Initialize();
		WeaponManager.FirstTagForOurTier(WeaponTags.PistolTag);
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			foreach (GadgetInfo.GadgetCategory item in Enum.GetValues(typeof(GadgetInfo.GadgetCategory)).OfType<GadgetInfo.GadgetCategory>())
			{
				if (item != GadgetInfo.GadgetCategory.Throwing && !Storager.hasKey(GadgetsInfo.SNForCategory(item)))
				{
					Storager.setString(GadgetsInfo.SNForCategory(item), string.Empty, false);
				}
			}
		}
		if (!Storager.hasKey(GadgetsInfo.SNForCategory(GadgetInfo.GadgetCategory.Throwing)))
		{
			Storager.setString(GadgetsInfo.SNForCategory(GadgetInfo.GadgetCategory.Throwing), "gadget_fraggrenade", false);
			GadgetsInfo.ProvideGadget("gadget_fraggrenade");
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted && PlayerPrefs.GetInt("shop_tutorial_state_passed_VER_12_1", 0) >= 9)
		{
			TrainingController.CompletedTrainingStage = TrainingController.NewTrainingCompletedStage.ShopCompleted;
			AnalyticsStuff.Tutorial(AnalyticsConstants.TutorialState.Back_Shop);
		}
		currentFon = riliFon;
		if (ActivityIndicator.instance == null && activityIndikatorPrefab != null)
		{
			UnityEngine.Object target = UnityEngine.Object.Instantiate(activityIndikatorPrefab);
			UnityEngine.Object.DontDestroyOnLoad(target);
		}
		ActivityIndicator.SetLoadingFon(currentFon);
		ActivityIndicator.IsShowWindowLoading = true;
	}

	private IEnumerator Start()
	{
		yield return null;
		Switcher.timer.Start();
		if (Defs.IsDeveloperBuild && Application.platform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			StringBuilder message = new StringBuilder("[Rilisoft] Trying to instantiate `android.os.AsyncTask`... ");
			try
			{
				using (new AndroidJavaClass("android.os.AsyncTask"))
				{
					message.Append("Done.");
				}
			}
			catch (Exception ex3)
			{
				Exception ex2 = ex3;
				message.Append("Failed.");
				Debug.LogException(ex2);
			}
			Debug.Log(message.ToString());
		}
		yield return null;
		if (!Storager.hasKey(Defs.PremiumEnabledFromServer))
		{
			Storager.setInt(Defs.PremiumEnabledFromServer, 0, false);
		}
		ActivityIndicator.IsActiveIndicator = false;
		foreach (float item in AppsMenuAwakeCoroutine())
		{
			float step = item;
			Switcher.timer.Reset();
			Switcher.timer.Start();
			yield return null;
			_preventAggressiveOptimisation = step;
		}
		Switcher.timer.Reset();
		Switcher.timer.Start();
		if (Launcher.UsingNewLauncher)
		{
			yield break;
		}
		if (Application.platform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
		{
			Action<string> handle = delegate(string sceneName)
			{
				if (Application.platform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					string abuseKey_21493d = GetAbuseKey_21493d18(558447896u);
					long num = DateTime.UtcNow.Ticks >> 1;
					long result = num;
					if (!Storager.hasKey(abuseKey_21493d))
					{
						Storager.setString(abuseKey_21493d, num.ToString(), false);
					}
					else if (long.TryParse(Storager.getString(abuseKey_21493d, false), out result))
					{
						Storager.setString(abuseKey_21493d, Math.Min(num, result).ToString(), false);
					}
					else
					{
						Storager.setString(abuseKey_21493d, num.ToString(), false);
					}
					CoroutineRunner.Instance.StartCoroutine(MeetTheCoroutine(sceneName, result << 1, num << 1));
				}
			};
			LicenseVerificationController.PackageInfo actualPackageInfo = default(LicenseVerificationController.PackageInfo);
			try
			{
//				actualPackageInfo = LicenseVerificationController.GetPackageInfo();
//				Launcher.PackageInfo = actualPackageInfo;
			}
			catch (Exception ex4)
			{
//				Exception ex = ex4;
//				Debug.Log("LicenseVerificationController.GetPackageInfo() failed:    " + ex);
//				handle(GetTerminalSceneName_4de1(19937u));
			}
			finally
			{
				if (actualPackageInfo.SignatureHash == null)
				{
//					Debug.Log("actualPackageInfo.SignatureHash == null");
//					handle(GetTerminalSceneName_4de1(19937u));
				}
			}
			string actualPackageName = actualPackageInfo.PackageName;
			if (string.Compare(actualPackageName, Defs.GetIntendedAndroidPackageName(), StringComparison.Ordinal) != 0)
			{
//				Debug.LogWarning("Verification FakeBundleDetected:    " + actualPackageName);
//				handle(GetTerminalSceneName_4de1(19937u));
			}
			else
			{
				Debug.Log("Package check passed.");
			}
			if (string.IsNullOrEmpty(intendedSignatureHash))
			{
//				Debug.LogWarning("String.IsNullOrEmpty(intendedSignatureHash)");
//				handle(GetTerminalSceneName_4de1(19937u));
			}
			string actualSignatureHash = actualPackageInfo.SignatureHash;
			if (string.Compare(actualSignatureHash, intendedSignatureHash, StringComparison.Ordinal) != 0)
			{
//				Debug.LogWarning("Verification FakeSignatureDetected:    " + actualSignatureHash);
//				Switcher.AppendAbuseMethod(AbuseMetod.AndroidPackageSignature);
//				handle(GetTerminalSceneName_4de1(19937u));
			}
			else
			{
				Debug.Log("Signature check passed.");
			}
		}
		//if (!Application.isEditor && ApplicationBinarySplitted)
		//{
		//	Debug.LogFormat("Expansion file path: '{0}'", _expansionFilePath.Value);
		//	string mainPath2 = GooglePlayDownloader.GetMainOBBPath(_expansionFilePath.Value);
		//	if (mainPath2 == null)
		//	{
		//		if (_fetchObbPromise != null)
		//		{
		//			_fetchObbPromise.TrySetCanceled();
		//		}
		//		_fetchObbPromise = new TaskCompletionSource<string>();
		//		Debug.LogWarning("Waiting mainPath...");
		//		if (!_storagePermissionRequested)
		//		{
		//			_storagePermissionRequested = true;
		//			NoodlePermissionGranter.PermissionRequestCallback = HandleStoragePermissionDialog;
		//			NoodlePermissionGranter.GrantPermission(NoodlePermissionGranter.NoodleAndroidPermission.WRITE_EXTERNAL_STORAGE);
		//		}
		//		while (!StoragePermissionFuture.IsCompleted)
		//		{
		//			yield return null;
		//		}
		//		if (!StoragePermissionFuture.Result)
		//		{
		//			Application.Quit();
		//			yield break;
		//		}
		//		GooglePlayDownloader.FetchOBB();
		//		while (true)
		//		{
		//			mainPath2 = GooglePlayDownloader.GetMainOBBPath(_expansionFilePath.Value);
		//			if (mainPath2 != null)
		//			{
		//				break;
		//			}
		//			yield return new WaitForRealSeconds(0.5f);
		//		}
		//		_fetchObbPromise.TrySetResult(mainPath2);
		//		Debug.LogFormat("Main path: '{0}'", mainPath2);
		//	}
		//	else
		//	{
		//		Debug.LogFormat("OBB already exists: '{0}'", mainPath2);
		//	}
		//}
		yield return null;
//		NoodlePermissionGranter.GrantPermission(NoodlePermissionGranter.NoodleAndroidPermission.ACCESS_COARSE_LOCATION);
		StartCoroutine(Fade(1f, 1f));
		SetCurrentLanguage();
	}

	private void HandleStoragePermissionDialog(bool permissionGranted)
	{
		_storagePermissionGrantedPromise.TrySetResult(permissionGranted);
		NoodlePermissionGranter.PermissionRequestCallback = null;
	}

	private IEnumerator OnApplicationPause(bool pause)
	{
		bool fetchingObb = FetchObbFuture != null && !FetchObbFuture.IsCompleted;
		Debug.LogFormat("AppsMenu pause: {0}; fetching OBB: {1}", pause, fetchingObb);
		if (pause || FetchObbFuture == null || (FetchObbFuture.IsCompleted && !FetchObbFuture.IsFaulted && !FetchObbFuture.IsCanceled && !string.IsNullOrEmpty(FetchObbFuture.Result)))
		{
			yield break;
		}
		if (_fetchObbPromise != null)
		{
			_fetchObbPromise.TrySetCanceled();
		}
		_fetchObbPromise = new TaskCompletionSource<string>();
		if (StoragePermissionFuture.IsCompleted)
		{
			if (!StoragePermissionFuture.Result)
			{
				Application.Quit();
			}
			yield break;
		}
		if (!_storagePermissionRequested)
		{
			_storagePermissionRequested = true;
			NoodlePermissionGranter.PermissionRequestCallback = HandleStoragePermissionDialog;
			NoodlePermissionGranter.GrantPermission(NoodlePermissionGranter.NoodleAndroidPermission.WRITE_EXTERNAL_STORAGE);
		}
		while (!StoragePermissionFuture.IsCompleted)
		{
			yield return null;
		}
		if (!StoragePermissionFuture.Result)
		{
			Application.Quit();
		}
		else
		{
			GooglePlayDownloader.FetchOBB();
		}
	}

	private static void CheckRenameOldLanguageName()
	{
		if (Storager.IsInitialized(Defs.ChangeOldLanguageName))
		{
			return;
		}
		Storager.SetInitialized(Defs.ChangeOldLanguageName);
		string @string = PlayerPrefs.GetString(Defs.CurrentLanguage, string.Empty);
		if (!string.IsNullOrEmpty(@string))
		{
			switch (@string)
			{
			case "Français":
				PlayerPrefs.SetString(Defs.CurrentLanguage, "French");
				PlayerPrefs.Save();
				break;
			case "Deutsch":
				PlayerPrefs.SetString(Defs.CurrentLanguage, "German");
				PlayerPrefs.Save();
				break;
			case "日本人":
				PlayerPrefs.SetString(Defs.CurrentLanguage, "Japanese");
				PlayerPrefs.Save();
				break;
			case "Español":
				PlayerPrefs.SetString(Defs.CurrentLanguage, "Spanish");
				PlayerPrefs.Save();
				break;
			}
		}
	}

	internal static void SetCurrentLanguage()
	{
		CheckRenameOldLanguageName();
		string @string = PlayerPrefs.GetString(Defs.CurrentLanguage);
		if (string.IsNullOrEmpty(@string))
		{
			@string = LocalizationStore.CurrentLanguage;
		}
		else
		{
			LocalizationStore.CurrentLanguage = @string;
		}
	}

	private static void HandleNotification(string message, Dictionary<string, object> additionalData, bool isActive)
	{
		Debug.LogFormat("GameThrive HandleNotification('{0}', ..., {1})", message, isActive);
	}

	private void LoadLoading()
	{
		Switcher.timer.Reset();
		Switcher.timer.Start();
		GlobalGameController.currentLevel = -1;
		SceneManager.LoadSceneAsync("Loading");
	}

	private void DrawQuad(Color aColor, float aAlpha)
	{
		aColor.a = aAlpha;
		if (m_Material != null && m_Material.SetPass(0))
		{
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Begin(7);
			GL.Color(aColor);
			GL.Vertex3(0f, 0f, -1f);
			GL.Vertex3(0f, 1f, -1f);
			GL.Vertex3(1f, 1f, -1f);
			GL.Vertex3(1f, 0f, -1f);
			GL.End();
			GL.PopMatrix();
		}
		else
		{
			Debug.LogWarning("Couldnot set pass for material.");
		}
	}

	private IEnumerator Fade(float aFadeOutTime, float aFadeInTime)
	{
		Color aColor = Color.black;
		for (float t2 = 0f; t2 < aFadeOutTime; t2 += Time.deltaTime)
		{
			float alpha = Mathf.InverseLerp(0f, aFadeOutTime, t2);
			DrawQuad(aColor, alpha);
			yield return null;
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			currentFon = commicsFon;
			if (ActivityIndicator.instance != null)
			{
				ActivityIndicator.instance.legendLabel.gameObject.SetActive(true);
				string legendLocalization = LocalizationStore.Get("Key_1925");
				bool localizationFailed = "Key_1925" == legendLocalization;
				string legendText = ((!localizationFailed) ? legendLocalization : "PLEASE REBOOT YOUR DEVICE IF FROZEN.");
				if (localizationFailed)
				{
					ActivityIndicator.instance.legendLabel.effectStyle = UILabel.Effect.None;
					ActivityIndicator.instance.legendLabel.color = Color.white;
				}
				ActivityIndicator.instance.legendLabel.text = legendText;
			}
			else
			{
				Debug.LogWarning("ActivityIndicator.instance is null.");
			}
			ActivityIndicator.IsActiveIndicator = false;
		}
		else
		{
			currentFon = androidFon;
			ActivityIndicator.IsActiveIndicator = true;
		}
		ActivityIndicator.SetLoadingFon(currentFon);
		for (float t = 0f; t < aFadeInTime; t += Time.deltaTime)
		{
			float alpha2 = Mathf.InverseLerp(0f, aFadeInTime, t);
			DrawQuad(aColor, 1f - alpha2);
			yield return null;
		}
		LoadLoading();
	}

	public static Rect RiliFonRect()
	{
		float num = (float)Screen.height * 1.7766234f;
		return new Rect((float)Screen.width / 2f - num / 2f, 0f, num, Screen.height);
	}

	private void OnGUI()
	{
		//if (!Launcher.UsingNewLauncher && !Application.isEditor && !GooglePlayDownloader.RunningOnAndroid())
		//{
		//	GUI.Label(new Rect(10f, 10f, Screen.width - 10, 20f), "Use GooglePlayDownloader only on Android device!");
		//}
	}

	private IEnumerator LoadLoadingScene()
	{
		yield return new WaitForRealSeconds(0.5f);
		Singleton<SceneLoader>.Instance.LoadScene("Loading");
	}
}

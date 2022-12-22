using System.Collections;
using Rilisoft;
using UnityEngine;

internal sealed class UpdatesChecker : MonoBehaviour
{
	private enum Store
	{
		Ios,
		Play,
		Wp8,
		Amazon,
		Unknown
	}

	private const string ActionAddress = "https://pixelgunserver.com/~rilisoft/action.php";

	private Store _currentStore;

	private IEnumerator CheckUpdatesCoroutine(Store store)
	{
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage <= TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
		{
			yield return null;
		}
		string version = string.Format("{0}:{1}", (int)store, GlobalGameController.AppVersion);
		if (Application.isEditor)
		{
			Debug.LogFormat("Sending version: {0}", version);
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "check_shop_version");
		form.AddField("app_version", version);
		WWW request = Tools.CreateWwwIfNotConnected("https://pixelgunserver.com/~rilisoft/action.php", form, string.Empty);
		if (request == null)
		{
			yield break;
		}
		yield return request;
		if (!string.IsNullOrEmpty(request.error))
		{
			Debug.LogWarningFormat("Error while receiving version: {0}", request.error);
			yield break;
		}
		string response = URLs.Sanitize(request);
		if (string.IsNullOrEmpty(response))
		{
			Debug.Log("response is empty");
			yield break;
		}
		if (Application.isEditor)
		{
			Debug.Log("UpdatesChecker: " + response);
		}
		if (response.Equals("no"))
		{
			GlobalGameController.NewVersionAvailable = true;
			Debug.Log("NewVersionAvailable: true");
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		_currentStore = Store.Unknown;
		switch (BuildSettings.BuildTargetPlatform)
		{
		case RuntimePlatform.IPhonePlayer:
			_currentStore = Store.Ios;
			break;
		case RuntimePlatform.Android:
			switch (Defs.AndroidEdition)
			{
			case Defs.RuntimeAndroidEdition.GoogleLite:
				_currentStore = Store.Play;
				break;
			case Defs.RuntimeAndroidEdition.Amazon:
				_currentStore = Store.Amazon;
				break;
			}
			break;
		case RuntimePlatform.MetroPlayerX64:
			_currentStore = Store.Wp8;
			break;
		}
	}

	private void Start()
	{
		StartCoroutine(CheckUpdatesCoroutine(_currentStore));
	}

	private void OnApplicationPause(bool pause)
	{
		if (Application.isEditor)
		{
			Debug.Log(">>> UpdatesChecker.OnApplicationPause()");
		}
		if (!pause)
		{
			StartCoroutine(CheckUpdatesCoroutine(_currentStore));
		}
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingInAfterGame : MonoBehaviour
{
	public static Texture loadingTexture;

	public static bool isShowLoading;

	private float timerShow = 2f;

	private LoadingNGUIController _loadingNGUIController;

	private bool ShouldShowLoading
	{
		get
		{
			return !(timerShow <= 0f) && !(loadingTexture == null) && Defs.isMulti && !Defs.isHunger;
		}
	}

	private void Awake()
	{
		if (ShouldShowLoading)
		{
			_loadingNGUIController = Object.Instantiate(Resources.Load<GameObject>("LoadingGUI")).GetComponent<LoadingNGUIController>();
			_loadingNGUIController.SceneToLoad = SceneManager.GetActiveScene().name;
			_loadingNGUIController.loadingNGUITexture.mainTexture = loadingTexture;
			_loadingNGUIController.transform.localPosition = Vector3.zero;
			_loadingNGUIController.Init();
			isShowLoading = true;
		}
	}

	private void Update()
	{
		if (timerShow > 0f)
		{
			timerShow -= Time.deltaTime;
		}
		if (!ActivityIndicator.IsActiveIndicator)
		{
			ActivityIndicator.IsActiveIndicator = true;
		}
		if (!ShouldShowLoading)
		{
			isShowLoading = false;
			base.enabled = false;
			loadingTexture = null;
			ActivityIndicator.IsActiveIndicator = false;
			if (_loadingNGUIController != null)
			{
				Object.Destroy(_loadingNGUIController.gameObject);
				_loadingNGUIController = null;
				Resources.UnloadUnusedAssets();
			}
		}
	}

	private void OnDestroy()
	{
		loadingTexture = null;
		isShowLoading = false;
	}
}

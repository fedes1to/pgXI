using System.Reflection;
using Rilisoft;
using UnityEngine;

public sealed class LoadConnectScene : MonoBehaviour
{
	public static string sceneToLoad = string.Empty;

	public static Texture textureToShow = null;

	public static Texture noteToShow = null;

	public static float interval = _defaultInterval;

	public Texture loadingNote;

	private static readonly float _defaultInterval = 1f;

	private Texture loading;

	private LoadingNGUIController _loadingNGUIController;

	public static LoadConnectScene Instance;

	private void Awake()
	{
		loading = textureToShow;
		if (loading == null)
		{
			string path = ConnectSceneNGUIController.MainLoadingTexture();
			loading = Resources.Load<Texture>(path);
		}
		_loadingNGUIController = Object.Instantiate(Resources.Load<GameObject>("LoadingGUI")).GetComponent<LoadingNGUIController>();
		_loadingNGUIController.SceneToLoad = sceneToLoad;
		_loadingNGUIController.loadingNGUITexture.mainTexture = loading;
		_loadingNGUIController.Init();
		NotificationController.instance.SaveTimeValues();
	}

	private void Start()
	{
		Instance = this;
		if (interval != -1f)
		{
			Invoke("_loadConnectScene", interval);
		}
		interval = _defaultInterval;
		ActivityIndicator.IsActiveIndicator = true;
	}

	private void OnGUI()
	{
		ActivityIndicator.IsActiveIndicator = true;
	}

	[Obfuscation(Exclude = true)]
	private void _loadConnectScene()
	{
		if (sceneToLoad.Equals("ConnectScene"))
		{
			Singleton<SceneLoader>.Instance.LoadScene(sceneToLoad);
		}
		else
		{
			Singleton<SceneLoader>.Instance.LoadSceneAsync(sceneToLoad);
		}
	}

	public static void LoadScene()
	{
		if (!(Instance == null))
		{
			Instance._loadConnectScene();
		}
	}

	private void OnDestroy()
	{
		Instance = null;
		if (!sceneToLoad.Equals("ConnectScene"))
		{
			ActivityIndicator.IsActiveIndicator = false;
		}
		loading = null;
		textureToShow = null;
	}
}

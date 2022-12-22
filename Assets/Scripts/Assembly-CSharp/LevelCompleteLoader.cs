using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

internal sealed class LevelCompleteLoader : MonoBehaviour
{
	public static Action action;

	public static string sceneName = string.Empty;

	private Texture fon;

	public UICamera myUICam;

	private Texture loadingNote;

	private void Start()
	{
		ActivityIndicator.IsActiveIndicator = true;
		if (!sceneName.Equals("LevelComplete"))
		{
			string path = ConnectSceneNGUIController.MainLoadingTexture();
			fon = Resources.Load<Texture>(path);
		}
		else
		{
			string path2 = "LevelLoadings" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/LevelComplete_back";
			if (Defs.IsSurvival)
			{
				path2 = "GameOver_Coliseum";
			}
			fon = Resources.Load<Texture>(path2);
		}
		GameObject gameObject = new GameObject();
		UITexture uITexture = gameObject.AddComponent<UITexture>();
		uITexture.mainTexture = fon;
		uITexture.SetRect(0f, 0f, 1366f, 768f);
		uITexture.transform.SetParent(myUICam.transform, false);
		uITexture.transform.localScale = Vector3.one;
		uITexture.transform.localPosition = Vector3.zero;
		StartCoroutine(loadNext());
		CampaignProgress.SaveCampaignProgress();
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
			PlayerPrefs.Save();
		}
	}

	private IEnumerator loadNext()
	{
		yield return new WaitForSeconds(0.25f);
		SceneManager.LoadScene(sceneName);
	}
}

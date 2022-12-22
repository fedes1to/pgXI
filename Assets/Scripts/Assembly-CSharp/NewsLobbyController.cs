using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using I2.Loc;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public class NewsLobbyController : MonoBehaviour
{
	public UIGrid newsGrid;

	public UIScrollView newsScroll;

	public UIScrollView fullNewsScroll;

	public UILabel headerLabel;

	public UILabel descLabel;

	public UILabel desc2Label;

	public UILabel dateLabel;

	public UITexture newsPic;

	public string currentURL;

	public string currentNewsName;

	public int selectedIndex;

	public GameObject newsItemPrefab;

	public GameObject urlButton;

	private List<NewsLobbyItem> newsList = new List<NewsLobbyItem>();

	private List<Dictionary<string, object>> newsListInfo = new List<Dictionary<string, object>>();

	private Texture2D[] newsFullPic;

	public static NewsLobbyController sharedController;

	private string cacheFullPictureNewsFileNamesKey = "cacheFullPictureFileNamesKey";

	private void Awake()
	{
		sharedController = this;
	}

	public void UpdateNewsList()
	{
		if (GetNews())
		{
			UpdateItemsCount();
			FillData();
			for (int i = 0; i < newsList.Count; i++)
			{
				newsList[i].GetComponent<UIToggle>().Set(false);
			}
			newsList[0].GetComponent<UIToggle>().Set(true);
			SetNewsIndex(0);
			newsScroll.enabled = newsListInfo.Count > 4;
			StartCoroutine(ClearCacheFullPictures());
			return;
		}
		while (newsList.Count > 0)
		{
			UnityEngine.Object.Destroy(newsList[newsList.Count - 1].gameObject);
			newsList.RemoveAt(newsList.Count - 1);
		}
		headerLabel.text = LocalizationStore.Get("Key_1807");
		dateLabel.text = string.Empty;
		descLabel.text = string.Empty;
		desc2Label.text = string.Empty;
		newsPic.aspectRatio = 200f;
		newsPic.enabled = false;
		urlButton.SetActive(false);
	}

	private void OnEnable()
	{
		UpdateNewsList();
	}

	private void OnDisable()
	{
		if (newsPic.mainTexture != null)
		{
			UnityEngine.Object.Destroy(newsPic.mainTexture);
			newsPic.mainTexture = null;
		}
	}

	private bool GetNews()
	{
		string @string = PlayerPrefs.GetString("LobbyNewsKey", "[]");
		newsListInfo = (Json.Deserialize(@string) as List<object>).OfType<Dictionary<string, object>>().ToList();
		if (newsListInfo == null || newsListInfo.Count == 0)
		{
			return false;
		}
		return true;
	}

	private void FillData()
	{
		for (int i = 0; i < newsList.Count; i++)
		{
			Dictionary<string, object> dictionary = newsListInfo[i];
			Dictionary<string, object> dictionary2 = dictionary["short_header"] as Dictionary<string, object>;
			Dictionary<string, object> dictionary3 = dictionary["short_description"] as Dictionary<string, object>;
			if (dictionary2 != null && dictionary3 != null)
			{
				object value;
				if (!dictionary2.TryGetValue(LocalizationManager.CurrentLanguage, out value))
				{
					dictionary2.TryGetValue("English", out value);
				}
				object value2;
				if (!dictionary3.TryGetValue(LocalizationManager.CurrentLanguage, out value2))
				{
					dictionary3.TryGetValue("English", out value2);
				}
				newsList[i].headerLabel.text = (string)value;
				if (Convert.ToInt32(dictionary["readed"]) == 0)
				{
					newsList[i].GetComponent<UISprite>().color = Color.white;
					newsList[i].indicatorNew.SetActive(true);
				}
				else
				{
					newsList[i].GetComponent<UISprite>().color = Color.gray;
					newsList[i].indicatorNew.SetActive(false);
				}
				newsList[i].shortDescLabel.text = (string)value2;
				DateTime currentTimeByUnixTime = Tools.GetCurrentTimeByUnixTime(Convert.ToInt32(dictionary["date"]) + DateTimeOffset.Now.Offset.Hours * 3600);
				newsList[i].dateLabel.text = currentTimeByUnixTime.Day.ToString("D2") + "." + currentTimeByUnixTime.Month.ToString("D2") + "." + currentTimeByUnixTime.Year + "\n" + currentTimeByUnixTime.Hour + ":" + currentTimeByUnixTime.Minute.ToString("D2");
				object value3;
				if (dictionary.TryGetValue("previewpicture", out value3))
				{
					newsList[i].LoadPreview((string)value3);
				}
			}
		}
	}

	public void OnNewsItemClick()
	{
		ButtonClickSound.TryPlayClick();
		for (int i = 0; i < newsList.Count; i++)
		{
			if (newsList[i].GetComponent<UIToggle>().value)
			{
				SetNewsIndex(i);
				break;
			}
		}
	}

	public void OnURLClick()
	{
		if (!string.IsNullOrEmpty(currentURL))
		{
			try
			{
				AnalyticsFacade.SendCustomEvent("News", new Dictionary<string, object>
				{
					{ "Conversion Total", "Source" },
					{ "Conversion By News", currentNewsName }
				});
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in log News: " + ex);
			}
			Application.OpenURL(currentURL);
		}
	}

	private IEnumerator LoadPictureForFullNews(int index, string picLink)
	{
		Texture2D picTexture2 = null;
		if (newsPic.mainTexture != null)
		{
			UnityEngine.Object.Destroy(newsPic.mainTexture);
			newsPic.mainTexture = null;
		}
		newsPic.aspectRatio = 200f;
		newsPic.mainTexture = null;
		string cachePath = PersistentCache.Instance.GetCachePathByUri(picLink);
		bool isLoadedFromCache = false;
		if (!string.IsNullOrEmpty(cachePath))
		{
			yield return null;
			try
			{
				bool cacheExists = File.Exists(cachePath);
				if (Defs.IsDeveloperBuild)
				{
					string formattedPath = ((!Application.isEditor) ? cachePath : ("<color=orange>" + cachePath + "</color>"));
					Debug.LogFormat("Trying to load news image from cache '{0}': {1}", formattedPath, cacheExists);
				}
				if (cacheExists)
				{
					byte[] cacheBytes = File.ReadAllBytes(cachePath);
					Texture2D cachedTexture = new Texture2D(2, 2);
					cachedTexture.LoadImage(cacheBytes);
					cachedTexture.filterMode = FilterMode.Point;
					picTexture2 = cachedTexture;
					isLoadedFromCache = true;
				}
			}
			catch (Exception ex2)
			{
				Debug.LogWarning("Caught exception while reading cached news image. See next message for details.");
				Debug.LogException(ex2);
			}
			if (!isLoadedFromCache)
			{
				WWW loadPic2 = Tools.CreateWwwIfNotConnected(picLink);
				if (loadPic2 == null)
				{
					yield break;
				}
				yield return loadPic2;
				if (!string.IsNullOrEmpty(loadPic2.error))
				{
					Debug.LogWarning("Download pic error: " + loadPic2.error);
					yield break;
				}
				picTexture2 = loadPic2.texture;
				picTexture2.filterMode = FilterMode.Point;
				if (!string.IsNullOrEmpty(cachePath))
				{
					try
					{
						if (Defs.IsDeveloperBuild)
						{
							string formattedPath2 = ((!Application.isEditor) ? cachePath : ("<color=orange>" + cachePath + "</color>"));
							Debug.LogFormat("Trying to save news image to cache '{0}'", formattedPath2);
						}
						string directoryPath = Path.GetDirectoryName(cachePath);
						if (!Directory.Exists(directoryPath))
						{
							Directory.CreateDirectory(directoryPath);
						}
						byte[] cacheBytes2 = loadPic2.texture.EncodeToPNG();
						File.WriteAllBytes(cachePath, cacheBytes2);
						SaveCacheFullPicturesNameFile(picLink);
					}
					catch (Exception ex3)
					{
						Exception ex = ex3;
						Debug.LogWarning("Caught exception while saving news image to cache. See next message for details.");
						Debug.LogException(ex);
					}
				}
				loadPic2 = null;
			}
		}
		if (selectedIndex == index)
		{
			BoxCollider collider2 = newsPic.GetComponent<BoxCollider>();
			if (collider2 == null)
			{
				collider2 = newsPic.gameObject.AddComponent<BoxCollider>();
				newsPic.gameObject.AddComponent<UIDragScrollView>();
				UIButton button = newsPic.gameObject.AddComponent<UIButton>();
				button.onClick.Add(new EventDelegate(OnURLClick));
			}
			newsPic.mainTexture = picTexture2;
			newsPic.aspectRatio = (float)picTexture2.width / (float)picTexture2.height;
			yield return null;
			picTexture2 = null;
			newsPic.ResizeCollider();
		}
	}

	private IEnumerator ClearCacheFullPictures()
	{
		string _cacheListStr = PlayerPrefs.GetString(cacheFullPictureNewsFileNamesKey, "[]");
		List<object> _cacheListObj = Json.Deserialize(_cacheListStr) as List<object>;
		List<string> _cacheList = new List<string>();
		List<string> _cacheListForRemove = new List<string>();
		for (int k = 0; k < _cacheListObj.Count; k++)
		{
			_cacheList.Add(_cacheListObj[k].ToString());
			_cacheListForRemove.Add(_cacheListObj[k].ToString());
		}
		for (int j = 0; j < newsListInfo.Count; j++)
		{
			if (newsListInfo[j].ContainsKey("fullpicture") && _cacheListForRemove.Contains(newsListInfo[j]["fullpicture"].ToString()))
			{
				_cacheListForRemove.Remove(newsListInfo[j]["fullpicture"].ToString());
			}
		}
		for (int i = 0; i < _cacheListForRemove.Count; i++)
		{
			string cachePath = PersistentCache.Instance.GetCachePathByUri(_cacheListForRemove[i]);
			File.Delete(cachePath);
			_cacheList.Remove(_cacheListForRemove[i]);
			yield return null;
		}
		PlayerPrefs.SetString(cacheFullPictureNewsFileNamesKey, Json.Serialize(_cacheList));
	}

	private void SaveCacheFullPicturesNameFile(string _nameFile)
	{
		string @string = PlayerPrefs.GetString(cacheFullPictureNewsFileNamesKey, "[]");
		List<object> list = Json.Deserialize(@string) as List<object>;
		List<string> list2 = new List<string>();
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(list[i].ToString());
		}
		if (!list2.Contains(_nameFile))
		{
			list2.Add(_nameFile);
		}
		PlayerPrefs.SetString(cacheFullPictureNewsFileNamesKey, Json.Serialize(list2));
	}

	private void SaveReaded()
	{
		PlayerPrefs.SetString("LobbyNewsKey", Json.Serialize(newsListInfo));
		bool flag = false;
		for (int i = 0; i < newsListInfo.Count; i++)
		{
			if (Convert.ToInt32(newsListInfo[i]["readed"]) == 0)
			{
				flag = true;
			}
		}
		PlayerPrefs.SetInt("LobbyIsAnyNewsKey", flag ? 1 : 0);
		MainMenuController.sharedController.newsIndicator.SetActive(flag);
		PlayerPrefs.Save();
	}

	private void SetNewsIndex(int index)
	{
		selectedIndex = index;
		fullNewsScroll.ResetPosition();
		Dictionary<string, object> dictionary = newsListInfo[index];
		Dictionary<string, object> dictionary2 = dictionary["header"] as Dictionary<string, object>;
		Dictionary<string, object> dictionary3 = dictionary["description"] as Dictionary<string, object>;
		Dictionary<string, object> dictionary4 = dictionary["category"] as Dictionary<string, object>;
		if (dictionary2 == null || dictionary3 == null || dictionary4 == null)
		{
			return;
		}
		object value;
		if (!dictionary2.TryGetValue(LocalizationManager.CurrentLanguage, out value))
		{
			dictionary2.TryGetValue("English", out value);
		}
		object value2;
		if (!dictionary3.TryGetValue(LocalizationManager.CurrentLanguage, out value2))
		{
			dictionary3.TryGetValue("English", out value2);
		}
		object value3;
		if (!dictionary4.TryGetValue(LocalizationManager.CurrentLanguage, out value3))
		{
			dictionary4.TryGetValue("English", out value3);
		}
		object value4;
		if (dictionary.TryGetValue("URL", out value4) && !value4.Equals(string.Empty))
		{
			currentURL = (string)value4;
			currentNewsName = ((!dictionary2.ContainsKey("English")) ? "NO ENGLISH TRANSLATION" : dictionary2["English"].ToString());
			urlButton.SetActive(true);
		}
		else
		{
			currentURL = string.Empty;
			urlButton.SetActive(false);
		}
		headerLabel.text = (string)value;
		string text = (string)value2;
		string[] array = text.Split(new string[1] { "[news-pic]" }, StringSplitOptions.None);
		object value5;
		dictionary.TryGetValue("fullpicture", out value5);
		if (array.Length > 1 && !string.IsNullOrEmpty((string)value5))
		{
			descLabel.text = array[0];
			desc2Label.text = array[1];
			newsPic.enabled = true;
			StartCoroutine(LoadPictureForFullNews(index, (string)value5));
		}
		else
		{
			descLabel.text = (string)value2;
			desc2Label.text = string.Empty;
			newsPic.aspectRatio = 200f;
			newsPic.enabled = false;
		}
		DateTime currentTimeByUnixTime = Tools.GetCurrentTimeByUnixTime(Convert.ToInt32(dictionary["date"]) + DateTimeOffset.Now.Offset.Hours * 3600);
		dateLabel.text = "[bababa]" + currentTimeByUnixTime.Day.ToString("D2") + "." + currentTimeByUnixTime.Month.ToString("D2") + "." + currentTimeByUnixTime.Year + " / [-]" + value3;
		try
		{
			if (Convert.ToInt32(dictionary["readed"]) == 0)
			{
				AnalyticsFacade.SendCustomEvent("News", new Dictionary<string, object>
				{
					{ "CTR", "Open" },
					{ "Conversion Total", "Open" },
					{
						"News",
						(!dictionary2.ContainsKey("English")) ? "NO ENGLISH TRANSLATION" : dictionary2["English"].ToString()
					}
				});
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in log News: " + ex);
		}
		dictionary["readed"] = 1;
		FillData();
		SaveReaded();
	}

	private void UpdateItemsCount()
	{
		while (newsList.Count < newsListInfo.Count)
		{
			GameObject gameObject = NGUITools.AddChild(newsGrid.gameObject, newsItemPrefab);
			gameObject.SetActive(true);
			newsList.Add(gameObject.GetComponent<NewsLobbyItem>());
		}
		while (newsList.Count > newsListInfo.Count)
		{
			UnityEngine.Object.Destroy(newsList[newsList.Count - 1].gameObject);
			newsList.RemoveAt(newsList.Count - 1);
		}
		newsGrid.Reposition();
		newsScroll.ResetPosition();
	}

	public void Close()
	{
		if (MainMenuController.sharedController != null)
		{
			MainMenuController.sharedController._isCancellationRequested = true;
		}
	}
}

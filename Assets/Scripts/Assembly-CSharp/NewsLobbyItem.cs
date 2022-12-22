using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Rilisoft;
using UnityEngine;

[DisallowMultipleComponent]
internal sealed class NewsLobbyItem : MonoBehaviour
{
	public GameObject indicatorNew;

	public UILabel headerLabel;

	public UILabel shortDescLabel;

	public UILabel dateLabel;

	public UITexture previewPic;

	public string previewPicUrl;

	private static readonly Dictionary<string, Task<bool>> s_currentlyRunningRequests = new Dictionary<string, Task<bool>>();

	public void LoadPreview(string url)
	{
		StartCoroutine(LoadPreviewPicture(url));
	}

	private IEnumerator LoadPreviewPicture(string picLink)
	{
		if (previewPic.mainTexture != null && previewPicUrl == picLink)
		{
			yield break;
		}
		previewPic.width = 100;
		if (previewPic.mainTexture != null)
		{
			UnityEngine.Object.Destroy(previewPic.mainTexture);
		}
		Task<bool> currentlyRunningRequest;
		if (s_currentlyRunningRequests.TryGetValue(picLink, out currentlyRunningRequest))
		{
			if (Defs.IsDeveloperBuild && currentlyRunningRequest.IsCompleted)
			{
				Debug.LogFormat("Request is completed: {0}", picLink);
			}
			float finishWaiting = Time.realtimeSinceStartup + 5f;
			while (!currentlyRunningRequest.IsCompleted)
			{
				if (finishWaiting < Time.realtimeSinceStartup && Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Stop waiting for completion: {0}", picLink);
					break;
				}
				yield return null;
			}
		}
		string cachePath = PersistentCache.Instance.GetCachePathByUri(picLink);
		if (!string.IsNullOrEmpty(cachePath))
		{
			try
			{
				bool cacheExists = File.Exists(cachePath);
				if (Defs.IsDeveloperBuild && !cacheExists)
				{
					string formattedPath = ((!Application.isEditor) ? cachePath : string.Format("<color=magenta>{0}</color>", cachePath));
					Debug.LogFormat("Cache miss: '{0}'", formattedPath);
				}
				if (cacheExists)
				{
					byte[] cacheBytes = File.ReadAllBytes(cachePath);
					Texture2D cachedTexture = new Texture2D(2, 2);
					cachedTexture.LoadImage(cacheBytes);
					cachedTexture.filterMode = FilterMode.Point;
					previewPicUrl = picLink;
					previewPic.mainTexture = cachedTexture;
					previewPic.mainTexture.filterMode = FilterMode.Point;
					previewPic.width = 100;
					yield break;
				}
			}
			catch (Exception ex4)
			{
				Exception ex3 = ex4;
				Debug.LogWarning("Caught exception while reading cached preview. See next message for details.");
				Debug.LogException(ex3);
			}
		}
		WWW loadPic = Tools.CreateWwwIfNotConnected(picLink);
		if (loadPic == null)
		{
			yield return new WaitForSeconds(60f);
			StartCoroutine(LoadPreviewPicture(picLink));
			yield break;
		}
		TaskCompletionSource<bool> promise = new TaskCompletionSource<bool>();
		s_currentlyRunningRequests[picLink] = promise.Task;
		yield return loadPic;
		if (!string.IsNullOrEmpty(loadPic.error))
		{
			promise.TrySetException(new InvalidOperationException(loadPic.error));
			s_currentlyRunningRequests.Remove(picLink);
			Debug.LogWarning("Download preview pic error: " + loadPic.error);
			if (loadPic.error.StartsWith("Resolving host timed out"))
			{
				yield return new WaitForSeconds(1f);
				if (Application.isEditor && FriendsController.isDebugLogWWW)
				{
					Debug.Log("Reloading timed out pic");
				}
				StartCoroutine(LoadPreviewPicture(picLink));
			}
			yield break;
		}
		previewPicUrl = picLink;
		previewPic.mainTexture = loadPic.texture;
		previewPic.mainTexture.filterMode = FilterMode.Point;
		previewPic.width = 100;
		if (!string.IsNullOrEmpty(cachePath))
		{
			try
			{
				if (Defs.IsDeveloperBuild)
				{
					string formattedPath2 = ((!Application.isEditor) ? cachePath : ("<color=magenta>" + cachePath + "</color>"));
					Debug.LogFormat("Trying to save preview to cache '{0}'", formattedPath2);
				}
				string directoryPath = Path.GetDirectoryName(cachePath);
				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
				}
				byte[] cacheBytes2 = loadPic.texture.EncodeToPNG();
				File.WriteAllBytes(cachePath, cacheBytes2);
				promise.TrySetResult(true);
			}
			catch (IOException ex5)
			{
				IOException ex2 = ex5;
				Debug.LogWarning("Caught IOException while saving preview to cache. See next message for details.");
				Debug.LogException(ex2);
				promise.TrySetException(ex2);
				s_currentlyRunningRequests.Remove(picLink);
			}
			catch (Exception ex6)
			{
				Exception ex = ex6;
				Debug.LogWarning("Caught exception while saving preview to cache. See next message for details.");
				Debug.LogException(ex);
				promise.TrySetException(ex);
				s_currentlyRunningRequests.Remove(picLink);
			}
		}
		else
		{
			promise.TrySetException(new InvalidOperationException("Cache path is null or empty."));
			s_currentlyRunningRequests.Remove(picLink);
		}
		ScopeLogger scopeLogger = new ScopeLogger("Dispose " + picLink, Defs.IsDeveloperBuild);
		try
		{
			loadPic.Dispose();
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}
}

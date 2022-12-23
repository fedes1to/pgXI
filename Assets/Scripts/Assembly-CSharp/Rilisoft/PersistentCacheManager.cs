using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class PersistentCacheManager : MonoBehaviour
	{
		[Serializable]
		private struct CacheEntry
		{
			public string signature;

			public string payload;
		}

		[CompilerGenerated]
		private sealed class _003CDownloadSignaturesCoroutine_003Ec__Iterator1A9 : IDisposable, IEnumerator, IEnumerator<object>
		{
			internal string[] urls;

			internal TaskCompletionSource<string> promise;

			internal List<string> _003Cnames_003E__0;

			internal string _003CnamesSerialized_003E__1;

			internal WWWForm _003Cform_003E__2;

			internal WWW _003Crequest_003E__3;

			internal CancellationToken cancellationToken;

			internal string _003Cresponse_003E__4;

			internal int _0024PC;

			internal object _0024current;

			internal string[] _003C_0024_003Eurls;

			internal TaskCompletionSource<string> _003C_0024_003Epromise;

			internal CancellationToken _003C_0024_003EcancellationToken;

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return _0024current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return _0024current;
				}
			}

			public bool MoveNext()
			{
				//Discarded unreachable code: IL_01b2
				uint num = (uint)_0024PC;
				_0024PC = -1;
				switch (num)
				{
				case 0u:
					if (urls.Length == 0)
					{
						promise.TrySetResult("{}");
						break;
					}
					_003Cnames_003E__0 = urls.Select(GetSegmentsAsString).ToList();
					_003CnamesSerialized_003E__1 = Json.Serialize(_003Cnames_003E__0);
					_003Cform_003E__2 = new WWWForm();
					_003Cform_003E__2.AddField("app_version", string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion));
					_003Cform_003E__2.AddField("names", _003CnamesSerialized_003E__1);
					_003Crequest_003E__3 = Tools.CreateWwwIfNotConnected("https://secure.pixelgunserver.com/get_files_info.php", _003Cform_003E__2, "DownloadSignaturesCoroutine()");
					if (_003Crequest_003E__3 == null)
					{
						promise.TrySetCanceled();
						break;
					}
					goto IL_0132;
				case 1u:
					{
						if (cancellationToken.IsCancellationRequested)
						{
							promise.TrySetCanceled();
							break;
						}
						goto IL_0132;
					}
					IL_0132:
					if (!_003Crequest_003E__3.isDone)
					{
						_0024current = null;
						_0024PC = 1;
						return true;
					}
					try
					{
						if (!string.IsNullOrEmpty(_003Crequest_003E__3.error))
						{
							promise.TrySetException(new InvalidOperationException(_003Crequest_003E__3.error));
							break;
						}
						_003Cresponse_003E__4 = URLs.Sanitize(_003Crequest_003E__3);
						promise.TrySetResult(_003Cresponse_003E__4);
						goto IL_01a7;
					}
					finally
					{
					}
					IL_01a7:
					_0024PC = -1;
					break;
				}
				return false;
			}

			[DebuggerHidden]
			public void Dispose()
			{
				_0024PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		private readonly TaskCompletionSource<Dictionary<string, string>> _firstResponsePromise = new TaskCompletionSource<Dictionary<string, string>>();

		private readonly Dictionary<string, string> _signatures = new Dictionary<string, string>();

		private readonly Lazy<EncryptedPlayerPrefs> _encryptedPlayerPrefs;

		private static readonly Lazy<PersistentCacheManager> _instance = new Lazy<PersistentCacheManager>(Create);

		internal bool IsEnabled
		{
			get
			{
				return BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64;
			}
		}

		internal Task FirstResponse
		{
			get
			{
				return _firstResponsePromise.Task;
			}
		}

		public static PersistentCacheManager Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		private EncryptedPlayerPrefs EncryptedPlayerPrefs
		{
			get
			{
				return _encryptedPlayerPrefs.Value;
			}
		}

		private PersistentCacheManager()
		{
			_encryptedPlayerPrefs = new Lazy<EncryptedPlayerPrefs>(CreateEncryptedPlayerPrefs);
		}

		public Task StartDownloadSignaturesLoop()
		{
			TaskCompletionSource<Dictionary<string, string>> firstResponsePromise = _firstResponsePromise;
			if (!IsEnabled)
			{
				firstResponsePromise.TrySetException(new NotSupportedException());
				return firstResponsePromise.Task;
			}
			_cancellationTokenSource.Cancel();
			_cancellationTokenSource = new CancellationTokenSource();
			float delaySecondsInCaseOfSuccess = (Application.isEditor ? 30f : ((!Defs.IsDeveloperBuild) ? 600f : 60f));
			float delaySecondsInCaseOfFailure = (Application.isEditor ? 3f : ((!Defs.IsDeveloperBuild) ? 60f : 6f));
			StartCoroutine(DownloadSignaturesLoopCoroutine(delaySecondsInCaseOfSuccess, delaySecondsInCaseOfFailure, _cancellationTokenSource.Token, firstResponsePromise));
			return firstResponsePromise.Task;
		}

		public string GetValue(string keyUrl)
		{
			//Discarded unreachable code: IL_00e6, IL_0122
			if (keyUrl == null)
			{
				throw new ArgumentNullException("keyUrl");
			}
			if (!IsEnabled)
			{
				return null;
			}
			if (string.IsNullOrEmpty(keyUrl))
			{
				return string.Empty;
			}
			string segmentsAsString = GetSegmentsAsString(keyUrl);
			if (string.IsNullOrEmpty(segmentsAsString))
			{
				return string.Empty;
			}
			string storageKey = GetStorageKey(segmentsAsString);
			string @string = EncryptedPlayerPrefs.GetString(storageKey);
			if (string.IsNullOrEmpty(@string))
			{
				return string.Empty;
			}
			try
			{
				CacheEntry cacheEntry = JsonUtility.FromJson<CacheEntry>(@string);
				string value;
				if (_signatures.TryGetValue(segmentsAsString, out value) && StringComparer.Ordinal.Equals(value, cacheEntry.signature))
				{
					return cacheEntry.payload ?? string.Empty;
				}
				if (EncryptedPlayerPrefs.HasKey(storageKey))
				{
					EncryptedPlayerPrefs.SetString(storageKey, string.Empty);
				}
				return string.Empty;
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogErrorFormat("Failed to deserialize {0}: \"{1}\"", typeof(CacheEntry).Name, @string);
				UnityEngine.Debug.LogException(exception);
				return null;
			}
		}

		public bool SetValue(string keyUrl, string value)
		{
			if (keyUrl == null)
			{
				throw new ArgumentNullException("keyUrl");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!IsEnabled)
			{
				return false;
			}
			string segmentsAsString = GetSegmentsAsString(keyUrl);
			if (string.IsNullOrEmpty(segmentsAsString))
			{
				return false;
			}
			string value2;
			if (!_signatures.TryGetValue(segmentsAsString, out value2))
			{
				return false;
			}
			string storageKey = GetStorageKey(segmentsAsString);
			CacheEntry cacheEntry = default(CacheEntry);
			cacheEntry.signature = value2;
			cacheEntry.payload = value;
			CacheEntry cacheEntry2 = cacheEntry;
			string value3 = JsonUtility.ToJson(cacheEntry2);
			EncryptedPlayerPrefs.SetString(storageKey, value3);
			return true;
		}

		internal static void DebugReportCacheHit(string url)
		{
			if (Defs.IsDeveloperBuild)
			{
				string text = string.Format("Cache hit: {0}", url ?? string.Empty);
				if (Application.isEditor)
				{
					UnityEngine.Debug.LogFormat("<color=green><b>{0}</b></color>", text);
				}
				else
				{
					UnityEngine.Debug.Log(text);
				}
			}
		}

		internal static void DebugReportCacheMiss(string url)
		{
			if (Defs.IsDeveloperBuild)
			{
				string text = string.Format("Cache miss: {0}", url ?? string.Empty);
				if (Application.isEditor)
				{
					UnityEngine.Debug.LogFormat("<color=orange><b>{0}</b></color>", text);
				}
				else
				{
					UnityEngine.Debug.Log(text);
				}
			}
		}

		private IEnumerator DownloadSignaturesLoopCoroutine(float delaySecondsInCaseOfSuccess, float delaySecondsInCaseOfFailure, CancellationToken cancellationToken, TaskCompletionSource<Dictionary<string, string>> promise)
		{
			int i = 0;
			TaskCompletionSource<Dictionary<string, string>> promise2 = default(TaskCompletionSource<Dictionary<string, string>>);
			while (!cancellationToken.IsCancellationRequested)
			{
				TaskCompletionSource<Dictionary<string, string>> promiseMayFail = new TaskCompletionSource<Dictionary<string, string>>();
				StartCoroutine(DownloadSignaturesOnceCoroutine(cancellationToken, promiseMayFail, i));
				Task<Dictionary<string, string>> taskMayFail = promiseMayFail.Task;
				taskMayFail.ContinueWith(delegate(Task<Dictionary<string, string>> t)
				{
					if (!t.IsFaulted && !t.IsCanceled)
					{
						promise2.TrySetResult(t.Result);
					}
				});
				float nextTimeToRequest = Time.realtimeSinceStartup + delaySecondsInCaseOfSuccess;
				while (Time.realtimeSinceStartup < nextTimeToRequest && !taskMayFail.IsFaulted)
				{
					yield return null;
				}
				if (taskMayFail.IsFaulted)
				{
					float restTimeToWait = nextTimeToRequest - Time.realtimeSinceStartup;
					float delaySeconds = Mathf.Clamp(restTimeToWait, 0f, delaySecondsInCaseOfFailure);
					yield return new WaitForSeconds(delaySeconds);
				}
				i++;
			}
		}

		private IEnumerator DownloadSignaturesOnceCoroutine(CancellationToken cancellationToken, TaskCompletionSource<Dictionary<string, string>> promise, int context = 0)
		{
			string[] urls = new string[11]
			{
				URLs.PromoActions,
				URLs.PromoActionsTest,
				URLs.LobbyNews,
				URLs.Advert,
				URLs.NewAdvertisingConfig,
				URLs.NewPerelivConfig,
				ChestBonusModel.GetUrlForDownloadBonusesData(),
				(!ABTestController.useBuffSystem) ? URLs.BuffSettings1031 : URLs.BuffSettings1050,
				URLs.BestBuy,
				URLs.PixelbookSettings,
				BalanceController.balanceURL
			};
			TaskCompletionSource<string> signaturesStringPromise = new TaskCompletionSource<string>();
			yield return StartCoroutine(DownloadSignaturesCoroutine(urls, cancellationToken, signaturesStringPromise));
			yield return StartCoroutine(WaitCompletionAndParseSignaturesCoroutine(signaturesStringPromise.Task, promise));
			if (promise.Task.IsCanceled)
			{
				UnityEngine.Debug.LogWarningFormat("DownloadSignaturesOnceCoroutine({0}) cancelled.", context);
				yield break;
			}
			if (promise.Task.IsFaulted)
			{
				UnityEngine.Debug.LogWarningFormat("DownloadSignaturesOnceCoroutine({0}) failed: {1}", context, promise.Task.Exception.InnerException);
				string[] array = urls;
				foreach (string url in array)
				{
					string segments = GetSegmentsAsString(url);
					string storageKey = GetStorageKey(segments);
					if (EncryptedPlayerPrefs.HasKey(storageKey))
					{
						EncryptedPlayerPrefs.SetString(storageKey, string.Empty);
					}
				}
				_signatures.Clear();
				yield break;
			}
			Dictionary<string, string> d = promise.Task.Result;
			if (Defs.IsDeveloperBuild && !Application.isEditor)
			{
				string format = ((!Application.isEditor) ? "DownloadSignaturesOnceCoroutine({0}) succeeded: {1}" : "DownloadSignaturesOnceCoroutine({0}) succeeded: <color=magenta>{1}</color>");
				UnityEngine.Debug.LogFormat(format, context, Json.Serialize(d));
			}
			foreach (KeyValuePair<string, string> kv in d)
			{
				_signatures[kv.Key] = kv.Value;
			}
		}

		private static IEnumerator WaitCompletionAndParseSignaturesCoroutine(Task<string> future, TaskCompletionSource<Dictionary<string, string>> promise)
		{
			try
			{
				yield return new WaitUntil(() => future.IsCompleted);
				if (future.IsCanceled)
				{
					promise.TrySetCanceled();
					yield break;
				}
				if (future.IsFaulted)
				{
					promise.TrySetException(future.Exception);
					yield break;
				}
				Dictionary<string, string> result = new Dictionary<string, string>();
				Dictionary<string, object> signaturesDictionaryObjects = Json.Deserialize(future.Result) as Dictionary<string, object>;
				if (signaturesDictionaryObjects == null)
				{
					promise.TrySetResult(result);
					yield break;
				}
				foreach (KeyValuePair<string, object> kv in signaturesDictionaryObjects)
				{
					string s = kv.Value as string;
					if (!string.IsNullOrEmpty(s))
					{
						result[kv.Key] = s;
					}
				}
				promise.TrySetResult(result);
			}
			finally
			{
			}
		}

		private static IEnumerator DownloadSignaturesCoroutine(string[] urls, CancellationToken cancellationToken, TaskCompletionSource<string> promise)
		{
			if (urls.Length == 0)
			{
				promise.TrySetResult("{}");
				yield break;
			}
			List<string> names = urls.Select(GetSegmentsAsString).ToList();
			string namesSerialized = Json.Serialize(names);
			WWWForm form = new WWWForm();
			form.AddField("app_version", string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion));
			form.AddField("names", namesSerialized);
			WWW request = Tools.CreateWwwIfNotConnected("https://secure.pixelgunserver.com/get_files_info.php", form, "DownloadSignaturesCoroutine()");
			if (request == null)
			{
				promise.TrySetCanceled();
				yield break;
			}
			while (!request.isDone)
			{
				yield return null;
				if (cancellationToken.IsCancellationRequested)
				{
					promise.TrySetCanceled();
					yield break;
				}
			}
			try
			{
				if (!string.IsNullOrEmpty(request.error))
				{
					promise.TrySetException(new InvalidOperationException(request.error));
					yield break;
				}
				string response = URLs.Sanitize(request);
				promise.TrySetResult(response);
			}
			finally
			{
	//			((_003CDownloadSignaturesCoroutine_003Ec__Iterator1A9)(object)this)._003C_003E__Finally0();
			}
		}

		private static string GetStorageKey(string segments)
		{
			return string.Format("Cache:{0}", segments);
		}

		private static string GetSegmentsAsString(string url)
		{
			//Discarded unreachable code: IL_0090, IL_00a2
			if (string.IsNullOrEmpty(url))
			{
				return string.Empty;
			}
			try
			{
				Uri uri = new Uri(url);
				string[] segments = uri.Segments;
				if (segments.Length == 0)
				{
					return string.Empty;
				}
				StringComparer c = StringComparer.OrdinalIgnoreCase;
				string[] array = segments.SkipWhile((string s, int i) => i == 0 && c.Equals(s, "/")).SkipWhile((string s, int i) => i == 0 && c.Equals(s, "pixelgun3d-config/")).ToArray();
				return string.Concat(array).TrimStart('/');
			}
			catch
			{
				return string.Empty;
			}
		}

		private static PersistentCacheManager Create()
		{
			PersistentCacheManager persistentCacheManager = UnityEngine.Object.FindObjectOfType<PersistentCacheManager>();
			if (persistentCacheManager != null)
			{
				UnityEngine.Object.DontDestroyOnLoad(persistentCacheManager.gameObject);
				return persistentCacheManager;
			}
			GameObject gameObject = new GameObject("Rilisoft.PersistentCacheManager");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			return gameObject.AddComponent<PersistentCacheManager>();
		}

		private EncryptedPlayerPrefs CreateEncryptedPlayerPrefs()
		{
			HiddenSettings hiddenSettings = ((!(MiscAppsMenu.Instance != null)) ? null : MiscAppsMenu.Instance.misc);
			byte[] array = null;
			if (hiddenSettings == null)
			{
				UnityEngine.Debug.LogError("CreateEncryptedPlayerPrefs(): settings are null.");
				array = new byte[40];
			}
			else
			{
				try
				{
					array = Convert.FromBase64String(hiddenSettings.PersistentCacheManagerKey);
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
					array = new byte[40];
				}
			}
			return new EncryptedPlayerPrefs(array);
		}
	}
}

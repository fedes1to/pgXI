using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class PerelivConfigManager
	{
		[CompilerGenerated]
		private sealed class _003CGetPerelivConfigOnce_003Ec__Iterator12A : IDisposable, IEnumerator, IEnumerator<object>
		{
			internal string _003Curl_003E__0;

			internal string _003CcachedResponse_003E__1;

			internal string _003CresponseText_003E__2;

			internal WWW _003Cresponse_003E__3;

			internal Dictionary<string, object> _003ClastLoadedConfig_003E__4;

			internal int _0024PC;

			internal object _0024current;

			internal PerelivConfigManager _003C_003Ef__this;

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
				//Discarded unreachable code: IL_0191
				uint num = (uint)_0024PC;
				_0024PC = -1;
				switch (num)
				{
				case 0u:
					_003Curl_003E__0 = URLs.NewPerelivConfig;
					_003CcachedResponse_003E__1 = PersistentCacheManager.Instance.GetValue(_003Curl_003E__0);
					if (!string.IsNullOrEmpty(_003CcachedResponse_003E__1))
					{
						PersistentCacheManager.DebugReportCacheHit(_003Curl_003E__0);
						_003CresponseText_003E__2 = _003CcachedResponse_003E__1;
						goto IL_0152;
					}
					PersistentCacheManager.DebugReportCacheMiss(_003Curl_003E__0);
					_003Cresponse_003E__3 = Tools.CreateWww(_003Curl_003E__0);
					_0024current = _003Cresponse_003E__3;
					_0024PC = 1;
					return true;
				case 1u:
					{
						try
						{
							if (_003Cresponse_003E__3 == null || !string.IsNullOrEmpty(_003Cresponse_003E__3.error))
							{
								UnityEngine.Debug.LogWarningFormat("Pereliv response error: {0}", (_003Cresponse_003E__3 == null) ? "null" : _003Cresponse_003E__3.error);
								break;
							}
							_003CresponseText_003E__2 = URLs.Sanitize(_003Cresponse_003E__3);
							if (string.IsNullOrEmpty(_003CresponseText_003E__2))
							{
								UnityEngine.Debug.LogWarning("Pereliv response is empty");
								break;
							}
							PersistentCacheManager.Instance.SetValue(_003Cresponse_003E__3.url, _003CresponseText_003E__2);
							goto IL_0152;
						}
						finally
						{
							_003C_003E__Finally0();
						}
					}
					IL_0152:
					_003ClastLoadedConfig_003E__4 = Json.Deserialize(_003CresponseText_003E__2) as Dictionary<string, object>;
					_003C_003Ef__this._lastLoadedConfig = _003ClastLoadedConfig_003E__4 ?? new Dictionary<string, object>(0);
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

			private void _003C_003E__Finally0()
			{
				_003Cresponse_003E__3.Dispose();
			}
		}

		private const float AdvertInfoTimeoutInSeconds = 960f;

		private static readonly Lazy<PerelivConfigManager> s_instance = new Lazy<PerelivConfigManager>(() => new PerelivConfigManager());

		private static readonly Dictionary<string, object> s_emptyDictionary = new Dictionary<string, object>(0);

		private Dictionary<string, object> _lastLoadedConfig = new Dictionary<string, object>(0);

		public static PerelivConfigManager Instance
		{
			get
			{
				return s_instance.Value;
			}
		}

		public Dictionary<string, object> LastLoadedConfig
		{
			get
			{
				return _lastLoadedConfig;
			}
		}

		private PerelivConfigManager()
		{
		}

		internal IEnumerator GetPerelivConfigLoop(Task futureToWait)
		{
			if (futureToWait != null)
			{
				while (!futureToWait.IsCompleted)
				{
					yield return null;
				}
			}
			while (true)
			{
				float advertGetInfoStartTime = Time.realtimeSinceStartup;
				yield return CoroutineRunner.Instance.StartCoroutine(GetPerelivConfigOnce());
				while (Time.realtimeSinceStartup - advertGetInfoStartTime < 960f)
				{
					yield return null;
				}
			}
		}

		internal PerelivSettings GetPerelivSettings(string category)
		{
			//Discarded unreachable code: IL_018d, IL_01a7
			if (LastLoadedConfig.Count == 0)
			{
				return new PerelivSettings("Last loaded config is empty");
			}
			if (string.IsNullOrEmpty(category))
			{
				return new PerelivSettings("Category is empty");
			}
			object value;
			if (!LastLoadedConfig.TryGetValue("mainMenu", out value))
			{
				return new PerelivSettings("Root object not found");
			}
			Dictionary<string, object> dictionary = value as Dictionary<string, object>;
			if (value == null)
			{
				return new PerelivSettings("Root object is not dictionary");
			}
			object value2;
			dictionary.TryGetValue("any", out value2);
			Dictionary<string, object> root = (value2 as Dictionary<string, object>) ?? s_emptyDictionary;
			object value3;
			dictionary.TryGetValue(category, out value3);
			Dictionary<string, object> overrides = (value3 as Dictionary<string, object>) ?? s_emptyDictionary;
			try
			{
				object value4 = TryGetValue(root, overrides, "enabled");
				object obj = TryGetValue(root, overrides, "imageUrl");
				object obj2 = TryGetValue(root, overrides, "redirectUrl");
				object obj3 = TryGetValue(root, overrides, "text");
				object value5 = TryGetValue(root, overrides, "showAlways");
				object obj4 = TryGetValue(root, overrides, "minLevel");
				object obj5 = TryGetValue(root, overrides, "maxLevel");
				object value6 = TryGetValue(root, overrides, "buttonClose");
				int minLevel = ((obj4 == null) ? (-1) : Convert.ToInt32(obj4));
				int maxLevel = ((obj5 == null) ? (-1) : Convert.ToInt32(obj5));
				return new PerelivSettings(Convert.ToBoolean(value4), obj as string, obj2 as string, obj3 as string, Convert.ToBoolean(value5), minLevel, maxLevel, Convert.ToBoolean(value6));
			}
			catch (Exception ex)
			{
				return new PerelivSettings(ex.Message);
			}
		}

		private static object TryGetValue(Dictionary<string, object> root, Dictionary<string, object> overrides, string key)
		{
			object value;
			if (overrides.TryGetValue(key, out value))
			{
				return value;
			}
			if (root.TryGetValue(key, out value))
			{
				return value;
			}
			return null;
		}

		private IEnumerator GetPerelivConfigOnce()
		{
			string url = URLs.NewPerelivConfig;
			string cachedResponse = PersistentCacheManager.Instance.GetValue(url);
			string responseText;
			if (!string.IsNullOrEmpty(cachedResponse))
			{
				PersistentCacheManager.DebugReportCacheHit(url);
				responseText = cachedResponse;
			}
			else
			{
				PersistentCacheManager.DebugReportCacheMiss(url);
				WWW response = Tools.CreateWww(url);
				yield return response;
				try
				{
					if (response == null || !string.IsNullOrEmpty(response.error))
					{
						UnityEngine.Debug.LogWarningFormat("Pereliv response error: {0}", (response == null) ? "null" : response.error);
						yield break;
					}
					responseText = URLs.Sanitize(response);
					if (string.IsNullOrEmpty(responseText))
					{
						UnityEngine.Debug.LogWarning("Pereliv response is empty");
						yield break;
					}
					PersistentCacheManager.Instance.SetValue(response.url, responseText);
				}
				finally
				{
				}
			}
			Dictionary<string, object> lastLoadedConfig = Json.Deserialize(responseText) as Dictionary<string, object>;
			_lastLoadedConfig = lastLoadedConfig ?? new Dictionary<string, object>(0);
		}
	}
}

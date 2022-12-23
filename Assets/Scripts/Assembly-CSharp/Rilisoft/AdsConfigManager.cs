using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class AdsConfigManager
	{
		[CompilerGenerated]
		private sealed class _003CGetAdvertInfoOnce_003Ec__Iterator124 : IDisposable, IEnumerator, IEnumerator<object>
		{
			internal string _003Curl_003E__0;

			internal string _003CresponseText_003E__1;

			internal string _003CcachedResponse_003E__2;

			internal WWW _003Cresponse_003E__3;

			internal EventHandler _003Chandler_003E__4;

			internal int _0024PC;

			internal object _0024current;

			internal AdsConfigManager _003C_003Ef__this;

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
				//Discarded unreachable code: IL_01ee
				uint num = (uint)_0024PC;
				_0024PC = -1;
				switch (num)
				{
				case 0u:
					_003Curl_003E__0 = URLs.NewAdvertisingConfig;
					if (!string.IsNullOrEmpty(configFromABTestAdvert))
					{
						_003CresponseText_003E__1 = configFromABTestAdvert;
					}
					else
					{
						_003CcachedResponse_003E__2 = PersistentCacheManager.Instance.GetValue(_003Curl_003E__0);
						if (string.IsNullOrEmpty(_003CcachedResponse_003E__2))
						{
							PersistentCacheManager.DebugReportCacheMiss(_003Curl_003E__0);
							_003Cresponse_003E__3 = Tools.CreateWww(_003Curl_003E__0);
							_0024current = _003Cresponse_003E__3;
							_0024PC = 1;
							return true;
						}
						PersistentCacheManager.DebugReportCacheHit(_003Curl_003E__0);
						_003CresponseText_003E__1 = _003CcachedResponse_003E__2;
					}
					goto IL_0171;
				case 1u:
					{
						try
						{
							if (_003Cresponse_003E__3 == null || !string.IsNullOrEmpty(_003Cresponse_003E__3.error))
							{
								UnityEngine.Debug.LogWarningFormat("Advert response error: {0}", (_003Cresponse_003E__3 == null) ? "null" : _003Cresponse_003E__3.error);
								break;
							}
							_003CresponseText_003E__1 = URLs.Sanitize(_003Cresponse_003E__3);
							if (string.IsNullOrEmpty(_003CresponseText_003E__1))
							{
								UnityEngine.Debug.LogWarning("Advert response is empty");
								break;
							}
							PersistentCacheManager.Instance.SetValue(_003Cresponse_003E__3.url, _003CresponseText_003E__1);
							goto IL_0171;
						}
						finally
						{
							_003C_003E__Finally0();
						}
					}
					IL_0171:
					_003C_003Ef__this._lastLoadedConfig = AdsConfigMemento.FromJson(_003CresponseText_003E__1);
					_003Chandler_003E__4 = _003C_003Ef__this.ConfigLoaded;
					if (_003Chandler_003E__4 != null)
					{
						_003Chandler_003E__4(_003C_003Ef__this, EventArgs.Empty);
					}
					if (_003C_003Ef__this._lastLoadedConfig.Exception != null)
					{
						UnityEngine.Debug.LogWarning(_003C_003Ef__this._lastLoadedConfig.Exception);
					}
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
				if (Application.isEditor)
				{
					UnityEngine.Debug.Log("<color=teal>AdsConfigManager.GetAdvertInfoOnce(): response.Dispose()</color>");
				}
				_003Cresponse_003E__3.Dispose();
			}
		}

		private const float AdvertInfoTimeout = 960f;

		public static string configFromABTestAdvert = string.Empty;

		private static readonly Lazy<AdsConfigManager> s_instance = new Lazy<AdsConfigManager>(() => new AdsConfigManager());

		private AdsConfigMemento _lastLoadedConfig;

		public static AdsConfigManager Instance
		{
			get
			{
				return s_instance.Value;
			}
		}

		public AdsConfigMemento LastLoadedConfig
		{
			get
			{
				return _lastLoadedConfig;
			}
		}

		public event EventHandler ConfigLoaded;

		private AdsConfigManager()
		{
		}

		internal IEnumerator GetAdvertInfoLoop(Task futureToWait)
		{
			if (futureToWait != null)
			{
				yield return new WaitUntil(() => futureToWait.IsCompleted);
			}
			while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage <= TrainingController.NewTrainingCompletedStage.None)
			{
				yield return null;
			}
			while (!FriendsController.isReadABTestAdvertConfig)
			{
				yield return null;
			}
			while (true)
			{
				float advertGetInfoStartTime = Time.realtimeSinceStartup;
				yield return CoroutineRunner.Instance.StartCoroutine(GetAdvertInfoOnce());
				yield return new WaitWhile(() => Time.realtimeSinceStartup - advertGetInfoStartTime < 960f);
			}
		}

		private IEnumerator GetAdvertInfoOnce()
		{
			string url = URLs.NewAdvertisingConfig;
			string responseText;
			if (!string.IsNullOrEmpty(configFromABTestAdvert))
			{
				responseText = configFromABTestAdvert;
			}
			else
			{
				string cachedResponse = PersistentCacheManager.Instance.GetValue(url);
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
							UnityEngine.Debug.LogWarningFormat("Advert response error: {0}", (response == null) ? "null" : response.error);
							yield break;
						}
						responseText = URLs.Sanitize(response);
						if (string.IsNullOrEmpty(responseText))
						{
							UnityEngine.Debug.LogWarning("Advert response is empty");
							yield break;
						}
						PersistentCacheManager.Instance.SetValue(response.url, responseText);
					}
					finally
					{
					}
				}
			}
			_lastLoadedConfig = AdsConfigMemento.FromJson(responseText);
			EventHandler handler = this.ConfigLoaded;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
			if (_lastLoadedConfig.Exception != null)
			{
				UnityEngine.Debug.LogWarning(_lastLoadedConfig.Exception);
			}
		}

		internal static CheatingMethods GetCheatingMethods(AdsConfigMemento config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (config.Exception != null)
			{
				return CheatingMethods.None;
			}
			if (config.CheaterConfig.CheckSignatureTampering && (Switcher.AbuseMethod & AbuseMetod.AndroidPackageSignature) != 0)
			{
				return CheatingMethods.SignatureTampering;
			}
			int @int = Storager.getInt("Coins", false);
			if (@int >= config.CheaterConfig.CoinThreshold)
			{
				return CheatingMethods.CoinThreshold;
			}
			int int2 = Storager.getInt("GemsCurrency", false);
			if (int2 >= config.CheaterConfig.GemThreshold)
			{
				return CheatingMethods.GemThreshold;
			}
			return CheatingMethods.None;
		}

		internal static string GetPlayerCategory(AdsConfigMemento config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (config.Exception != null)
			{
				return string.Empty;
			}
			if (GetCheatingMethods(config) != 0)
			{
				return "cheater";
			}
			bool flag = StoreKitEventListener.IsPayingUser();
			int @int = PlayerPrefs.GetInt(Defs.SessionDayNumberKey, 1);
			float num = ((!(NotificationController.instance != null)) ? Time.realtimeSinceStartup : NotificationController.instance.currentPlayTime);
			int num2 = Mathf.FloorToInt(num / 60f);
			foreach (KeyValuePair<string, PlayerStateMemento> playerState in config.PlayerStates)
			{
				PlayerStateMemento value = playerState.Value;
				if (value.IsPaying != flag)
				{
					continue;
				}
				if (value.MinInGameMinutes.HasValue && value.MaxInGameMinutes.HasValue)
				{
					if (num2 < value.MinInGameMinutes.Value || num2 > value.MaxInGameMinutes.Value)
					{
						continue;
					}
				}
				else if (@int < value.MinDay || @int > value.MaxDay)
				{
					continue;
				}
				return value.Id;
			}
			return string.Empty;
		}

		internal static int GetInterstitialDisabledReasonCode(AdsConfigMemento config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (config.Exception != null)
			{
				return 10;
			}
			if (config.InterstitialConfig == null)
			{
				return 20;
			}
			string playerCategory = GetPlayerCategory(config);
			string deviceModel = SystemInfo.deviceModel;
			double timeSpanSinceLastShowInMinutes = GetTimeSpanSinceLastShowInMinutes();
			int disabledReasonCode = config.InterstitialConfig.GetDisabledReasonCode(playerCategory, deviceModel, timeSpanSinceLastShowInMinutes);
			if (disabledReasonCode != 0)
			{
				return 30 + disabledReasonCode;
			}
			return 0;
		}

		internal static string GetInterstitialDisabledReason(AdsConfigMemento config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (config.Exception != null)
			{
				return config.Exception.Message;
			}
			if (config.InterstitialConfig == null)
			{
				return "`InterstitialConfig == null` (probably not received yet)";
			}
			string playerCategory = GetPlayerCategory(config);
			string deviceModel = SystemInfo.deviceModel;
			double timeSpanSinceLastShowInMinutes = GetTimeSpanSinceLastShowInMinutes();
			return config.InterstitialConfig.GetDisabledReason(playerCategory, deviceModel, timeSpanSinceLastShowInMinutes);
		}

		internal static string GetVideoDisabledReason(AdsConfigMemento config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (config.Exception != null)
			{
				return config.Exception.Message;
			}
			string playerCategory = GetPlayerCategory(config);
			string deviceModel = SystemInfo.deviceModel;
			return config.VideoConfig.GetDisabledReason(playerCategory, deviceModel);
		}

		internal static double GetTimeSpanSinceLastShowInMinutes()
		{
			string @string = PlayerPrefs.GetString(Defs.LastTimeShowBanerKey, string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				return 3.4028234663852886E+38;
			}
			DateTime result;
			if (!DateTime.TryParse(@string, out result))
			{
				return 3.4028234663852886E+38;
			}
			double totalMinutes = DateTime.UtcNow.Subtract(result).TotalMinutes;
			if (totalMinutes < 0.0)
			{
				PlayerPrefs.SetString(Defs.LastTimeShowBanerKey, DateTime.UtcNow.ToString("s"));
				PlayerPrefs.Save();
				return 0.0;
			}
			return totalMinutes;
		}
	}
}

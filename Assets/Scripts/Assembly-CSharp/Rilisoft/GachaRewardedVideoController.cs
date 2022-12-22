using System;
using System.Globalization;
using UnityEngine;

namespace Rilisoft
{
	[DisallowMultipleComponent]
	internal sealed class GachaRewardedVideoController : MonoBehaviour
	{
		[SerializeField]
		private GameObject gachaRewardedVideoParent;

		[SerializeField]
		private GameObject gachaRewardedVideoButton;

		[SerializeField]
		private GameObject skinCamera;

		private GachaRewardedVideo _gachaRewardedVideo;

		private static GachaRewardedVideoController s_instance;

		public static GachaRewardedVideoController Instance
		{
			get
			{
				return s_instance;
			}
		}

		private GachaRewardedVideo GachaRewardedVideo
		{
			get
			{
				if (_gachaRewardedVideo == null)
				{
					GachaRewardedVideo gachaRewardedVideo = Resources.Load<GachaRewardedVideo>("GachaRewardedVideo");
					if (gachaRewardedVideo == null)
					{
						Debug.LogWarning("gachaRewardedVideoPrefab is null.");
						return null;
					}
					_gachaRewardedVideo = UnityEngine.Object.Instantiate(gachaRewardedVideo);
					if (_gachaRewardedVideo == null)
					{
						Debug.LogWarning("gachaRewardedVideo is null.");
						return null;
					}
					_gachaRewardedVideo.transform.SetParent(gachaRewardedVideoParent.transform);
					_gachaRewardedVideo.transform.localPosition = Vector3.zero;
					_gachaRewardedVideo.transform.localScale = Vector3.one;
					_gachaRewardedVideo.EnterIdle += OnEnterIdle;
					_gachaRewardedVideo.ExitIdle += OnExitIdle;
					_gachaRewardedVideo.AdWatchedSuccessfully += OnAdWatchedSuccessfully;
				}
				return _gachaRewardedVideo;
			}
		}

		private GachaRewardedVideoController()
		{
		}

		public void OnGachaRewardedVideoButton()
		{
			if (GachaRewardedVideo != null)
			{
				GachaRewardedVideo.OnWatchButtonClicked();
			}
		}

		public void RefreshGui(bool forceButton)
		{
			gachaRewardedVideoButton.SetActive(forceButton);
		}

		public void RefreshGui()
		{
			bool forceButton = GachaRewardedVideoButtonIsEnabled();
			RefreshGui(forceButton);
		}

		private void Awake()
		{
			s_instance = this;
		}

		private void Start()
		{
			RefreshGui();
		}

		private void OnDestroy()
		{
			if (_gachaRewardedVideo != null)
			{
				_gachaRewardedVideo.EnterIdle -= OnEnterIdle;
				_gachaRewardedVideo.ExitIdle -= OnExitIdle;
				_gachaRewardedVideo.AdWatchedSuccessfully -= OnAdWatchedSuccessfully;
			}
		}

		private static string GetReasonToDismissVideoFreeSpin()
		{
			AdsConfigMemento lastLoadedConfig = AdsConfigManager.Instance.LastLoadedConfig;
			if (lastLoadedConfig == null)
			{
				return "Ads config is `null`.";
			}
			if (lastLoadedConfig.Exception != null)
			{
				return lastLoadedConfig.Exception.Message;
			}
			string videoDisabledReason = AdsConfigManager.GetVideoDisabledReason(lastLoadedConfig);
			if (!string.IsNullOrEmpty(videoDisabledReason))
			{
				return videoDisabledReason;
			}
			FreeSpinPointMemento freeSpin = lastLoadedConfig.AdPointsConfig.FreeSpin;
			if (freeSpin == null)
			{
				return string.Format("`{0}` config is `null`", freeSpin.Id);
			}
			string playerCategory = AdsConfigManager.GetPlayerCategory(lastLoadedConfig);
			string disabledReason = freeSpin.GetDisabledReason(playerCategory);
			if (!string.IsNullOrEmpty(disabledReason))
			{
				return disabledReason;
			}
			double finalTimeoutBetweenShowInMinutes = freeSpin.GetFinalTimeoutBetweenShowInMinutes(playerCategory);
			double timeSpanSinceLastShowInMinutes = GetTimeSpanSinceLastShowInMinutes();
			if (timeSpanSinceLastShowInMinutes < finalTimeoutBetweenShowInMinutes)
			{
				return string.Format(CultureInfo.InvariantCulture, "`{0}`: {1:f2} < `timeoutInMinutes: {2:f2}`", freeSpin.Id, timeSpanSinceLastShowInMinutes, finalTimeoutBetweenShowInMinutes);
			}
			return string.Empty;
		}

		private static double GetTimeSpanSinceLastShowInMinutes()
		{
			DateTime? timeSinceLastShow = GetTimeSinceLastShow();
			if (!timeSinceLastShow.HasValue)
			{
				return 3.4028234663852886E+38;
			}
			return DateTime.UtcNow.Subtract(timeSinceLastShow.Value).TotalMinutes;
		}

		private static DateTime? GetTimeSinceLastShow()
		{
			string @string = PlayerPrefs.GetString("Ads.LastTimeShown", string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				return null;
			}
			DateTime result;
			if (!DateTime.TryParse(@string, out result))
			{
				return null;
			}
			return result;
		}

		public static bool VideoViewedToday()
		{
			DateTime? timeSinceLastShow = GetTimeSinceLastShow();
			if (!timeSinceLastShow.HasValue)
			{
				return false;
			}
			DateTime dateTime = timeSinceLastShow.Value.AddDays(1.0);
			DateTime utcNow = DateTime.UtcNow;
			return dateTime >= utcNow;
		}

		private bool GachaRewardedVideoButtonIsEnabled()
		{
			if (GiftController.Instance.CanGetGift)
			{
				return false;
			}
			string reasonToDismissVideoFreeSpin = GetReasonToDismissVideoFreeSpin();
			if (string.IsNullOrEmpty(reasonToDismissVideoFreeSpin))
			{
				return true;
			}
			string format = ((!Application.isEditor) ? "GachaRewardedVideoButtonIsEnabled(): false. {0}" : "<color=magenta>GachaRewardedVideoButtonIsEnabled(): false. {0}</color>");
			Debug.LogFormat(format, reasonToDismissVideoFreeSpin);
			return false;
		}

		private void OnEnterIdle(object sender, FinishedEventArgs e)
		{
			bool succeeded = e.Succeeded;
			if (Application.isEditor)
			{
				Debug.LogFormat("<color=magenta>OnEnterIdle: {0}</color>", succeeded);
			}
			if (skinCamera != null)
			{
				skinCamera.SetActive(true);
			}
			gachaRewardedVideoButton.SetActive(GachaRewardedVideoButtonIsEnabled());
		}

		private void OnExitIdle(object sender, EventArgs e)
		{
			if (Application.isEditor)
			{
				Debug.Log("<color=magenta>OnExitIdle</color>");
			}
			if (skinCamera != null)
			{
				skinCamera.SetActive(false);
			}
			gachaRewardedVideoButton.SetActive(GachaRewardedVideoButtonIsEnabled());
		}

		private void OnAdWatchedSuccessfully(object sender, EventArgs e)
		{
			if (Application.isEditor)
			{
				Debug.Log("<color=magenta>OnAdWatchedSuccessfully</color>");
			}
			GiftController.Instance.IncrementFreeSpins();
			GiftController.Instance.ReCreateSlots();
		}
	}
}

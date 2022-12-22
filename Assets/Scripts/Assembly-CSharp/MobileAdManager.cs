using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

internal sealed class MobileAdManager
{
	public enum Type
	{
		Image,
		Video
	}

	public enum State
	{
		None,
		Idle,
		Loaded
	}

	internal enum SampleGroup
	{
		Unknown,
		Video,
		Image
	}

	internal const string TextInterstitialUnitId = "ca-app-pub-5590536419057381/7885668153";

	internal const string DefaultImageInterstitialUnitId = "ca-app-pub-5590536419057381/1950086558";

	internal const string DefaultVideoInterstitialUnitId = "ca-app-pub-5590536419057381/2096360557";

	private static byte[] _guid = new byte[0];

	private int _imageAdUnitIdIndex;

	private int _imageIdGroupIndex;

	private int _videoAdUnitIdIndex;

	private int _videoIdGroupIndex;

	private static readonly Lazy<MobileAdManager> _instance = new Lazy<MobileAdManager>(() => new MobileAdManager());

	public static MobileAdManager Instance
	{
		get
		{
			return _instance.Value;
		}
	}

	public State VideoInterstitialState
	{
		get
		{
			return State.None;
		}
	}

	public string ImageAdFailedToLoadMessage { get; private set; }

	public string VideoAdFailedToLoadMessage { get; private set; }

	internal bool SuppressShowOnReturnFromPause { get; set; }

	internal static byte[] GuidBytes
	{
		get
		{
			if (_guid != null && _guid.Length > 0)
			{
				return _guid;
			}
			if (PlayerPrefs.HasKey("Guid"))
			{
				try
				{
					_guid = new Guid(PlayerPrefs.GetString("Guid")).ToByteArray();
				}
				catch
				{
					Guid guid = Guid.NewGuid();
					_guid = guid.ToByteArray();
					PlayerPrefs.SetString("Guid", guid.ToString("D"));
					PlayerPrefs.Save();
				}
			}
			else
			{
				Guid guid2 = Guid.NewGuid();
				_guid = guid2.ToByteArray();
				PlayerPrefs.SetString("Guid", guid2.ToString("D"));
				PlayerPrefs.Save();
			}
			return _guid;
		}
	}

	private string ImageInterstitialUnitId
	{
		get
		{
			if (PromoActionsManager.MobileAdvert == null || PromoActionsManager.MobileAdvert.AdmobImageAdUnitIds.Count == 0)
			{
				return "ca-app-pub-5590536419057381/1950086558";
			}
			return AdmobImageAdUnitIds[_imageAdUnitIdIndex % AdmobImageAdUnitIds.Count];
		}
	}

	private string VideoInterstitialUnitId
	{
		get
		{
			if (PromoActionsManager.MobileAdvert == null)
			{
				return "ca-app-pub-5590536419057381/2096360557";
			}
			if (AdmobVideoAdUnitIds.Count == 0)
			{
				return (!string.IsNullOrEmpty(PromoActionsManager.MobileAdvert.AdmobVideoAdUnitId)) ? PromoActionsManager.MobileAdvert.AdmobVideoAdUnitId : "ca-app-pub-5590536419057381/2096360557";
			}
			return AdmobVideoAdUnitIds[_videoAdUnitIdIndex % AdmobVideoAdUnitIds.Count];
		}
	}

	private List<string> AdmobVideoAdUnitIds
	{
		get
		{
			if (PromoActionsManager.MobileAdvert.AdmobVideoIdGroups.Count == 0)
			{
				return PromoActionsManager.MobileAdvert.AdmobVideoAdUnitIds;
			}
			return PromoActionsManager.MobileAdvert.AdmobVideoIdGroups[_videoIdGroupIndex % PromoActionsManager.MobileAdvert.AdmobVideoIdGroups.Count];
		}
	}

	private List<string> AdmobImageAdUnitIds
	{
		get
		{
			if (PromoActionsManager.MobileAdvert.AdmobImageIdGroups.Count == 0)
			{
				return PromoActionsManager.MobileAdvert.AdmobImageAdUnitIds;
			}
			return PromoActionsManager.MobileAdvert.AdmobImageIdGroups[_imageIdGroupIndex % PromoActionsManager.MobileAdvert.AdmobImageIdGroups.Count];
		}
	}

	internal int ImageAdUnitIndexClamped
	{
		get
		{
			if (AdmobImageAdUnitIds.Count == 0)
			{
				return -1;
			}
			return _imageAdUnitIdIndex % AdmobImageAdUnitIds.Count;
		}
	}

	internal int VideoAdUnitIndexClamped
	{
		get
		{
			if (AdmobVideoAdUnitIds.Count == 0)
			{
				return -1;
			}
			return _videoAdUnitIdIndex % AdmobVideoAdUnitIds.Count;
		}
	}

	private MobileAdManager()
	{
		ImageAdFailedToLoadMessage = string.Empty;
		VideoAdFailedToLoadMessage = string.Empty;
	}

	public static string GetReasonToDismissVideoChestInLobby()
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
		ChestInLobbyPointMemento chestInLobby = lastLoadedConfig.AdPointsConfig.ChestInLobby;
		if (chestInLobby == null)
		{
			return string.Format("`{0}` config is `null`", chestInLobby.Id);
		}
		string playerCategory = AdsConfigManager.GetPlayerCategory(lastLoadedConfig);
		string disabledReason = chestInLobby.GetDisabledReason(playerCategory);
		if (!string.IsNullOrEmpty(disabledReason))
		{
			return disabledReason;
		}
		int lobbyLevel = ExpController.LobbyLevel;
		if (lobbyLevel < 3)
		{
			return string.Format(CultureInfo.InvariantCulture, "lobbyLevel: {0} < 3", lobbyLevel);
		}
		return string.Empty;
	}

	public void DestroyImageInterstitial()
	{
	}

	public void DestroyVideoInterstitial()
	{
	}

	public static bool UserPredicate(Type adType, bool verbose, bool showToPaying = false, bool showToNew = false)
	{
		bool flag = IsNewUser();
		bool flag2 = IsPayingUser();
		bool flag9;
		if (adType == Type.Video)
		{
			int lobbyLevel = ExpController.LobbyLevel;
			bool flag3 = lobbyLevel >= 3;
			bool flag4 = PromoActionsManager.MobileAdvert != null && PromoActionsManager.MobileAdvert.VideoEnabled;
			bool flag5 = PromoActionsManager.MobileAdvert != null && PromoActionsManager.MobileAdvert.VideoShowPaying;
			bool flag6 = PromoActionsManager.MobileAdvert != null && PromoActionsManager.MobileAdvert.VideoShowNonpaying;
			bool flag7 = (flag2 && flag5) || (!flag2 && flag6);
			bool flag8 = PlayerPrefs.GetInt("CountRunMenu", 0) >= 3;
			flag9 = flag4 && flag8 && flag7 && flag3;
			if (verbose)
			{
				Debug.LogFormat("AdIsApplicable ({0}): {1}    Paying: {2},  Need to show: {3},  Session count satisfied: {4},  Lobby level: {5}", adType, flag9, flag2, (!flag2) ? flag6 : flag5, flag8, lobbyLevel);
			}
		}
		else
		{
			bool flag10 = IsLongTimeShowBaner();
			flag9 = PromoActionsManager.MobileAdvert != null && PromoActionsManager.MobileAdvert.ImageEnabled && (!flag || showToNew) && (!flag2 || showToPaying) && flag10;
			if (verbose)
			{
				Dictionary<string, bool> dictionary = new Dictionary<string, bool>(6);
				dictionary.Add("ImageEnabled", PromoActionsManager.MobileAdvert != null && PromoActionsManager.MobileAdvert.ImageEnabled);
				dictionary.Add("isNewUser", flag);
				dictionary.Add("showToNew", showToNew);
				dictionary.Add("isPayingUser", flag2);
				dictionary.Add("showToPaying", showToPaying);
				dictionary.Add("longTimeShowBanner", flag10);
				Dictionary<string, bool> obj = dictionary;
				string message = string.Format("AdIsApplicable ({0}): {1}    Details: {2}", adType, flag9, Json.Serialize(obj));
				Debug.Log(message);
			}
		}
		return flag9;
	}

	internal static void RefreshBytes()
	{
		PlayerPrefs.SetString("Guid", new Guid(_guid).ToString("D"));
		PlayerPrefs.Save();
	}

	internal static SampleGroup GetSempleGroup()
	{
		byte b = GuidBytes[0];
		return ((int)b % 2 != 0) ? SampleGroup.Video : SampleGroup.Image;
	}

	public static bool IsNewUserOldMetod()
	{
		string @string = PlayerPrefs.GetString("First Launch (Advertisement)", string.Empty);
		DateTimeOffset result;
		if (!string.IsNullOrEmpty(@string) && DateTimeOffset.TryParse(@string, out result))
		{
			return (DateTimeOffset.Now - result).TotalDays < 7.0;
		}
		return true;
	}

	private static bool IsLongTimeShowBaner()
	{
		string @string = PlayerPrefs.GetString(Defs.LastTimeShowBanerKey, string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			return true;
		}
		DateTime result;
		if (!DateTime.TryParse(@string, out result))
		{
			return false;
		}
		DateTime utcNow = DateTime.UtcNow;
		double totalSeconds = (utcNow - result).TotalSeconds;
		return totalSeconds > (double)PromoActionsManager.MobileAdvert.TimeoutBetweenShowInterstitial;
	}

	private static bool IsNewUser()
	{
		int @int = PlayerPrefs.GetInt(Defs.SessionDayNumberKey, 1);
		if (@int > PromoActionsManager.MobileAdvert.CountSessionNewPlayer)
		{
			return false;
		}
		return true;
	}

	public static bool IsPayingUser()
	{
		return StoreKitEventListener.IsPayingUser();
	}

	internal bool SwitchImageAdUnitId()
	{
		int imageAdUnitIdIndex = _imageAdUnitIdIndex;
		string imageInterstitialUnitId = ImageInterstitialUnitId;
		_imageAdUnitIdIndex++;
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("Switching image ad unit id from {0} ({1}) to {2} ({3})", imageAdUnitIdIndex, RemovePrefix(imageInterstitialUnitId), _imageAdUnitIdIndex, RemovePrefix(ImageInterstitialUnitId));
			Debug.Log(message);
		}
		return PromoActionsManager.MobileAdvert.AdmobImageAdUnitIds.Count == 0 || _imageAdUnitIdIndex % PromoActionsManager.MobileAdvert.AdmobImageAdUnitIds.Count == 0;
	}

	internal bool SwitchVideoAdUnitId()
	{
		int videoAdUnitIdIndex = _videoAdUnitIdIndex;
		string videoInterstitialUnitId = VideoInterstitialUnitId;
		_videoAdUnitIdIndex++;
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("Switching video ad unit id from {0} ({1}) to {2} ({3}); group index {4}", videoAdUnitIdIndex, RemovePrefix(videoInterstitialUnitId), _videoAdUnitIdIndex, RemovePrefix(VideoInterstitialUnitId), _videoIdGroupIndex);
			Debug.Log(message);
		}
		return AdmobVideoAdUnitIds.Count == 0 || _videoAdUnitIdIndex % AdmobVideoAdUnitIds.Count == 0;
	}

	internal bool SwitchImageIdGroup()
	{
		int imageIdGroupIndex = _imageIdGroupIndex;
		List<string> obj = AdmobImageAdUnitIds.Select(RemovePrefix).ToList();
		string text = Json.Serialize(obj);
		_imageIdGroupIndex++;
		_imageAdUnitIdIndex = 0;
		List<string> obj2 = AdmobImageAdUnitIds.Select(RemovePrefix).ToList();
		string text2 = Json.Serialize(obj2);
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("Switching image id group from {0} ({1}) to {2} ({3})", imageIdGroupIndex, text, _imageIdGroupIndex, text2);
			Debug.Log(message);
		}
		return PromoActionsManager.MobileAdvert.AdmobImageIdGroups.Count == 0 || _imageIdGroupIndex % PromoActionsManager.MobileAdvert.AdmobImageIdGroups.Count == 0;
	}

	internal bool SwitchVideoIdGroup()
	{
		int videoIdGroupIndex = _videoIdGroupIndex;
		List<string> obj = AdmobVideoAdUnitIds.Select(RemovePrefix).ToList();
		string text = Json.Serialize(obj);
		_videoIdGroupIndex++;
		_videoAdUnitIdIndex = 0;
		List<string> obj2 = AdmobVideoAdUnitIds.Select(RemovePrefix).ToList();
		string text2 = Json.Serialize(obj2);
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("Switching video id group from {0} ({1}) to {2} ({3})", videoIdGroupIndex, text, _videoIdGroupIndex, text2);
			Debug.Log(message);
		}
		return PromoActionsManager.MobileAdvert.AdmobVideoIdGroups.Count == 0 || _videoIdGroupIndex % PromoActionsManager.MobileAdvert.AdmobVideoIdGroups.Count == 0;
	}

	internal static string RemovePrefix(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return string.Empty;
		}
		int num = s.IndexOf('/');
		return (num <= 0) ? s : s.Remove(0, num);
	}

	internal bool ResetVideoAdUnitId()
	{
		int videoAdUnitIdIndex = _videoAdUnitIdIndex;
		string videoInterstitialUnitId = VideoInterstitialUnitId;
		int videoIdGroupIndex = _videoIdGroupIndex;
		_videoAdUnitIdIndex = 0;
		_videoIdGroupIndex = 0;
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("Resetting video group from {0} to {1}", videoIdGroupIndex, _videoIdGroupIndex);
			Debug.Log(message);
		}
		return true;
	}

	internal bool ResetImageAdUnitId()
	{
		int imageAdUnitIdIndex = _imageAdUnitIdIndex;
		string imageInterstitialUnitId = ImageInterstitialUnitId;
		int imageIdGroupIndex = _imageIdGroupIndex;
		_imageAdUnitIdIndex = 0;
		_imageIdGroupIndex = 0;
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("Resetting image ad unit id from {0} to {1}; group index from {2} to 0", imageAdUnitIdIndex, _imageAdUnitIdIndex, imageIdGroupIndex);
			Debug.Log(message);
		}
		return true;
	}
}

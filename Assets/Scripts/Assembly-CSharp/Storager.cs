using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public static class Storager
{
	private const bool useCryptoPlayerPrefs = true;

	private const bool _useSignedPreferences = false;

	private static readonly Dictionary<string, Action> _onValueChanged;

	private static bool iCloudAvailable;

	private static readonly Dictionary<string, SaltedInt> _keychainCache;

	private static readonly Dictionary<string, string> _keychainStringCache;

	private static Dictionary<string, int> iosCloudSyncBuffer;

	private static bool _weaponDigestIsDirty;

	private static HashSet<string> m_keysInKeychainIos;

	private static readonly IDictionary<string, SaltedInt> _protectedIntCache;

	private static readonly System.Random _prng;

	private static readonly string[] _expendableKeys;

	public static bool ICloudAvailable
	{
		get
		{
			return iCloudAvailable;
		}
	}

	public static bool UseSignedPreferences
	{
		get
		{
			return false;
		}
	}

	public static event EventHandler RatingUpdated;

	static Storager()
	{
		_onValueChanged = new Dictionary<string, Action>();
		iCloudAvailable = false;
		_keychainCache = new Dictionary<string, SaltedInt>();
		_keychainStringCache = new Dictionary<string, string>();
		iosCloudSyncBuffer = new Dictionary<string, int>();
		m_keysInKeychainIos = new HashSet<string>();
		_protectedIntCache = new Dictionary<string, SaltedInt>();
		_prng = new System.Random();
		_expendableKeys = new string[4]
		{
			GearManager.InvisibilityPotion,
			GearManager.Jetpack,
			GearManager.Turret,
			GearManager.Mech
		};
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer)
		{
			return;
		}
		IEnumerable<string> enumerable = PurchasesSynchronizer.AllItemIds();
		foreach (string item in enumerable)
		{
			iosCloudSyncBuffer.Add(item, 0);
		}
	}

	public static void SubscribeToChanged(string key, Action act)
	{
		if (!key.IsNullOrEmpty())
		{
			if (_onValueChanged.ContainsKey(key))
			{
				Dictionary<string, Action> onValueChanged;
				Dictionary<string, Action> dictionary = (onValueChanged = _onValueChanged);
				string key2;
				string key3 = (key2 = key);
				Action a = onValueChanged[key2];
				dictionary[key3] = (Action)Delegate.Combine(a, act);
			}
			else
			{
				_onValueChanged.Add(key, act);
			}
		}
	}

	public static void UnSubscribeToChanged(string key, Action act)
	{
		if (key.IsNullOrEmpty() || !_onValueChanged.ContainsKey(key))
		{
			return;
		}
		Action action = _onValueChanged[key];
		Delegate[] array = action.GetInvocationList().ToArray();
		foreach (Delegate @delegate in array)
		{
			if (@delegate == act)
			{
				action = (Action)Delegate.Remove(action, act);
			}
		}
	}

	private static void InvokeSubscribers(string key)
	{
		if (Application.isPlaying && _onValueChanged.ContainsKey(key))
		{
			Action action = _onValueChanged[key];
			if (action != null)
			{
				action();
			}
		}
	}

	public static void SynchronizeIosWithCloud(ref List<string> weaponsForWhichSetRememberedTier, out bool armorArmy1Comes)
	{
		armorArmy1Comes = false;
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer && !iCloudAvailable)
		{
		}
	}

	private static int GetIntFromICloud(string key, int defaultValue)
	{
		return defaultValue;
	}

	private static void SetIntToICloud(string key, int value)
	{
	}

	private static void SynchronizeTrophiesWithIosCloud()
	{
		int intFromICloud = GetIntFromICloud("RatingNegative_CLOUD", 0);
		int intFromICloud2 = GetIntFromICloud("RatingPositive_CLOUD", 0);
		int num = intFromICloud2 - intFromICloud;
		int intFromICloud3 = GetIntFromICloud("Season_CLOUD", 0);
		int negativeRating = RatingSystem.instance.negativeRating;
		int positiveRating = RatingSystem.instance.positiveRating;
		int num2 = positiveRating - negativeRating;
		int currentCompetition = FriendsController.sharedController.currentCompetition;
		bool flag = false;
		bool flag2 = false;
		if (intFromICloud3 == 0)
		{
			if (RatingSystem.instance.currentLeague == RatingSystem.RatingLeague.Adamant)
			{
				flag2 = true;
			}
			else
			{
				int num3 = negativeRating;
				if (intFromICloud > negativeRating)
				{
					num3 = intFromICloud;
					RatingSystem.instance.negativeRating = num3;
					flag = true;
				}
				else if (intFromICloud < negativeRating)
				{
					flag2 = true;
				}
				int num4 = positiveRating;
				if (intFromICloud2 > positiveRating)
				{
					num4 = intFromICloud2;
					RatingSystem.instance.positiveRating = num4;
					flag = true;
				}
				else if (intFromICloud2 < positiveRating)
				{
					flag2 = true;
				}
				int num5 = num4 - num3;
				int trophiesSeasonThreshold = RatingSystem.instance.TrophiesSeasonThreshold;
				if (num5 > trophiesSeasonThreshold)
				{
					int num6 = num5 - trophiesSeasonThreshold;
					num3 += num6;
					RatingSystem.instance.negativeRating = num3;
					flag = true;
					flag2 = true;
					TournamentAvailableBannerWindow.CanShow = true;
				}
			}
		}
		else if (intFromICloud3 > currentCompetition)
		{
			FriendsController.sharedController.currentCompetition = intFromICloud3;
			RatingSystem.instance.negativeRating = intFromICloud;
			RatingSystem.instance.positiveRating = intFromICloud2;
			flag = true;
		}
		else if (intFromICloud3 == currentCompetition)
		{
			int num7 = negativeRating;
			if (intFromICloud > negativeRating)
			{
				num7 = intFromICloud;
				RatingSystem.instance.negativeRating = num7;
				flag = true;
			}
			else if (intFromICloud < negativeRating)
			{
				flag2 = true;
			}
			int num8 = positiveRating;
			if (intFromICloud2 > positiveRating)
			{
				num8 = intFromICloud2;
				RatingSystem.instance.positiveRating = num8;
				flag = true;
			}
			else if (intFromICloud2 < positiveRating)
			{
				flag2 = true;
			}
		}
		else
		{
			flag2 = true;
		}
		EventHandler ratingUpdated = Storager.RatingUpdated;
		if (flag && ratingUpdated != null)
		{
			ratingUpdated(null, EventArgs.Empty);
		}
		if (flag2)
		{
			SetIntToICloud("RatingNegative_CLOUD", RatingSystem.instance.negativeRating);
			SetIntToICloud("RatingPositive_CLOUD", RatingSystem.instance.positiveRating);
			SetIntToICloud("Season_CLOUD", FriendsController.sharedController.currentCompetition);
		}
	}

	public static void Initialize(bool cloudAvailable)
	{
	}

	public static bool hasKey(string key)
	{
		return CryptoPlayerPrefsFacade.HasKey(key);
	}

	public static void setInt(string key, int val, bool useICloud)
	{
		if (Application.isEditor)
		{
			PlayerPrefs.SetInt(key, val);
		}
		else
		{
			CryptoPlayerPrefsFacade.SetInt(key, val);
			_protectedIntCache[key] = new SaltedInt(_prng.Next(), val);
		}
		if (key.Equals("Coins") || key.Equals("GemsCurrency"))
		{
			DigestStorager.Instance.Set(key, val);
		}
		if (_expendableKeys.Contains(key))
		{
			RefreshExpendablesDigest();
		}
		if (WeaponManager.PurchasableWeaponSetContains(key))
		{
			_weaponDigestIsDirty = true;
		}
		InvokeSubscribers(key);
	}

	public static int getInt(string key, bool useICloud)
	{
		if (Application.isEditor)
		{
			return PlayerPrefs.GetInt(key);
		}
		SaltedInt value;
		if (_protectedIntCache.TryGetValue(key, out value))
		{
			return value.Value;
		}
		if (CryptoPlayerPrefsFacade.HasKey(key))
		{
			int @int = CryptoPlayerPrefsFacade.GetInt(key);
			_protectedIntCache.Add(key, new SaltedInt(_prng.Next(), @int));
			return @int;
		}
		return 0;
	}

	public static void setString(string key, string val, bool useICloud)
	{
		_keychainStringCache[key] = val;
		if (Application.isEditor)
		{
			PlayerPrefs.SetString(key, val);
			InvokeSubscribers(key);
		}
		else
		{
			CryptoPlayerPrefsFacade.SetString(key, val);
			InvokeSubscribers(key);
		}
	}

	public static string getString(string key, bool useICloud)
	{
		string value;
		if (_keychainStringCache.TryGetValue(key, out value))
		{
			return value;
		}
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			return PlayerPrefs.GetString(key);
		}
		if (CryptoPlayerPrefsFacade.HasKey(key))
		{
			string @string = CryptoPlayerPrefsFacade.GetString(key);
			_keychainStringCache.Add(key, @string);
			return @string;
		}
		return string.Empty;
	}

	public static bool IsInitialized(string flagName)
	{
		if (Application.isEditor)
		{
			return PlayerPrefs.HasKey(flagName);
		}
		return hasKey(flagName);
	}

	public static void SetInitialized(string flagName)
	{
		setInt(flagName, 0, false);
	}

	public static void SyncWithCloud(string storageId)
	{
		getInt(storageId, true);
	}

	private static void RefreshExpendablesDigest()
	{
		byte[] value = _expendableKeys.SelectMany((string key) => BitConverter.GetBytes(getInt(key, false))).ToArray();
		DigestStorager.Instance.Set("ExpendablesCount", value);
	}

	public static void RefreshWeaponDigestIfDirty()
	{
		if (_weaponDigestIsDirty)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("[Rilisoft] > RefreshWeaponsDigest: {0:F3}", Time.realtimeSinceStartup);
			}
			RefreshWeaponsDigest();
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("[Rilisoft] < RefreshWeaponsDigest: {0:F3}", Time.realtimeSinceStartup);
			}
		}
	}

	private static void RefreshWeaponsDigest()
	{
		IEnumerable<string> source = WeaponManager.storeIDtoDefsSNMapping.Values.Where((string w) => getInt(w, false) == 1);
		int value = source.Count();
		DigestStorager.Instance.Set("WeaponsCount", value);
		_weaponDigestIsDirty = false;
	}
}

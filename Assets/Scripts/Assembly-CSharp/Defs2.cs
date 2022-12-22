using System.Collections.Generic;
using System.Security.Cryptography;
using Rilisoft;
using UnityEngine;

public sealed class Defs2
{
	private static bool _ourGunPricesCogorteInitialized = false;

	private static bool TierAfter8_3_0Initialized = false;

	private static SignedPreferences _signedPreferences;

	private static readonly byte[] _rsaParameters = new byte[308]
	{
		7, 2, 0, 0, 0, 164, 0, 0, 82, 83,
		65, 50, 0, 2, 0, 0, 17, 0, 0, 0,
		1, 24, 67, 211, 214, 189, 210, 144, 254, 145,
		230, 212, 19, 254, 185, 112, 117, 120, 142, 89,
		80, 227, 74, 157, 136, 99, 204, 254, 117, 105,
		106, 52, 143, 219, 180, 55, 4, 174, 130, 222,
		59, 143, 80, 32, 56, 220, 204, 215, 254, 202,
		38, 42, 34, 141, 116, 38, 68, 147, 247, 71,
		65, 49, 18, 153, 205, 10, 30, 210, 118, 97,
		196, 36, 168, 88, 201, 246, 230, 160, 110, 13,
		124, 85, 105, 5, 43, 72, 1, 158, 28, 194,
		234, 109, 169, 124, 57, 167, 5, 106, 4, 145,
		166, 174, 181, 8, 222, 238, 193, 247, 67, 4,
		63, 158, 68, 238, 149, 46, 126, 245, 244, 34,
		194, 82, 16, 202, 202, 47, 85, 234, 177, 145,
		103, 107, 6, 167, 139, 19, 113, 83, 144, 51,
		172, 211, 28, 133, 56, 20, 84, 65, 236, 67,
		16, 239, 26, 32, 10, 254, 38, 72, 99, 157,
		197, 181, 106, 238, 33, 247, 188, 47, 35, 40,
		87, 193, 215, 151, 33, 197, 170, 220, 239, 73,
		82, 102, 162, 100, 132, 69, 125, 74, 225, 224,
		235, 68, 230, 233, 9, 162, 182, 97, 205, 7,
		35, 71, 107, 239, 213, 14, 6, 135, 7, 137,
		140, 150, 80, 39, 253, 197, 12, 101, 164, 157,
		109, 89, 10, 134, 225, 17, 130, 168, 84, 111,
		116, 89, 20, 67, 132, 7, 204, 191, 33, 103,
		113, 0, 12, 11, 19, 139, 190, 49, 110, 98,
		16, 209, 75, 236, 139, 213, 86, 4, 8, 182,
		121, 126, 53, 5, 123, 132, 234, 114, 1, 125,
		120, 63, 150, 29, 192, 102, 100, 11, 230, 161,
		170, 133, 253, 231, 199, 89, 5, 45
	};

	public static Dictionary<string, List<ItemPrice>> GadgetPricesFromServer { get; set; }

	public static Dictionary<string, ItemPrice> ArmorPricesFromServer { get; set; }

	public static bool CanShowPremiumAccountExpiredWindow
	{
		get
		{
			return TrainingController.TrainingCompleted;
		}
	}

	public static int MaxGrenadeCount
	{
		get
		{
			return 10;
		}
	}

	public static int GrenadeOnePrice
	{
		get
		{
			return VirtualCurrencyHelper.Price("GrenadeID" + GearManager.OneItemSuffix + GearManager.CurrentNumberOfUphradesForGear("GrenadeID")).Price;
		}
	}

	public static string ApplicationUrl
	{
		get
		{
			return (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon) ? "http://www.amazon.com/RiliSoft-Games-Pixel-Gun-3D/dp/B00I6IKSZ0" : ((BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64) ? "http://pixelgun3d.com/get.html" : "https://www.microsoft.com/ru-ru/store/games/pixel-gun-3d/9wzdncrdzvbf");
		}
	}

	internal static SignedPreferences SignedPreferences
	{
		get
		{
			if (_signedPreferences == null)
			{
				RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(512);
				rSACryptoServiceProvider.ImportCspBlob(_rsaParameters);
				_signedPreferences = new RsaSignedPreferences(new PersistentPreferences(), rSACryptoServiceProvider, SystemInfo.deviceUniqueIdentifier);
			}
			return _signedPreferences;
		}
	}

	public static void InitializeTier8_3_0Key()
	{
		if (!TierAfter8_3_0Initialized)
		{
			if (!Storager.hasKey(Defs.TierAfter8_3_0Key))
			{
				Storager.setInt(Defs.TierAfter8_3_0Key, ExpController.GetOurTier(), false);
			}
			TierAfter8_3_0Initialized = true;
		}
	}

	public static bool IsAvalibleAddFrends()
	{
		return FriendsController.sharedController.friends.Count + FriendsController.sharedController.invitesToUs.Count < Defs.maxCountFriend;
	}
}

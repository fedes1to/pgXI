using Rilisoft;
using UnityEngine;

internal sealed class ChestBonusModel
{
	public const string pathToCommonBonusItems = "Textures/Bank/StarterPack_Weapon";

	public static string GetUrlForDownloadBonusesData()
	{
		string empty = string.Empty;
		empty = (Defs.IsDeveloperBuild ? "chest_bonus_test.json" : ((BuildSettings.BuildTargetPlatform == RuntimePlatform.Android) ? ((Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "chest_bonus_android.json" : "chest_bonus_amazon.json") : ((BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64) ? "chest_bonus_ios.json" : "chest_bonus_wp8.json")));
		return string.Format("{0}{1}", "https://secure.pixelgunserver.com/pixelgun3d-config/chestBonus/", empty);
	}
}

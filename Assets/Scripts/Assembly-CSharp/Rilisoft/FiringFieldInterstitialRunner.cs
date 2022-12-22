using System.Globalization;
using UnityEngine.SceneManagement;

namespace Rilisoft
{
	internal sealed class FiringFieldInterstitialRunner : FyberInterstitialRunnerBase
	{
		public static string GetReasonToDismissInterstitialPolygon(int entryCount)
		{
			AdsConfigMemento lastLoadedConfig = AdsConfigManager.Instance.LastLoadedConfig;
			if (lastLoadedConfig == null)
			{
				return "Interstitials config is `null`.";
			}
			string interstitialDisabledReason = AdsConfigManager.GetInterstitialDisabledReason(lastLoadedConfig);
			if (!string.IsNullOrEmpty(interstitialDisabledReason))
			{
				return interstitialDisabledReason;
			}
			PolygonAdPointMemento polygon = lastLoadedConfig.AdPointsConfig.Polygon;
			if (polygon == null)
			{
				return "Polygon config is `null`";
			}
			string playerCategory = AdsConfigManager.GetPlayerCategory(lastLoadedConfig);
			string disabledReason = polygon.GetDisabledReason(playerCategory);
			if (!string.IsNullOrEmpty(disabledReason))
			{
				return disabledReason;
			}
			int? entryCountOverride = polygon.GetEntryCountOverride(playerCategory);
			if (entryCountOverride.HasValue)
			{
				if (entryCount < entryCountOverride.Value)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0} < `entryCount` = {1} in `{2}` for category `{3}`.", entryCount, entryCountOverride.Value, polygon.Id, playerCategory);
				}
			}
			else if (entryCount < polygon.EntryCount)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} < `waveMinCount` = {1} in `{2}`.", entryCount, polygon.EntryCount, polygon.Id);
			}
			return string.Empty;
		}

		protected override string GetReasonToSkip()
		{
			string name = SceneManager.GetActiveScene().name;
			if (name == Defs.MainMenuScene)
			{
				return string.Empty;
			}
			return "Not in main scene: " + name;
		}
	}
}

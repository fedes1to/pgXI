using System.Globalization;

namespace Rilisoft
{
	internal sealed class AfterMatchInterstitialRunner : FyberInterstitialRunnerBase
	{
		public static string GetReasonToDismissInterstitialAfterMatch(bool winner, double matchDurationInMinutes)
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
			AfterMatchAdPointMemento afterMatch = lastLoadedConfig.AdPointsConfig.AfterMatch;
			if (afterMatch == null)
			{
				return string.Format("`{0}` config is `null`", afterMatch.Id);
			}
			string playerCategory = AdsConfigManager.GetPlayerCategory(lastLoadedConfig);
			string disabledReason = afterMatch.GetDisabledReason(playerCategory);
			if (!string.IsNullOrEmpty(disabledReason))
			{
				return disabledReason;
			}
			double? matchMinDurationInMinutesOverride = afterMatch.GetMatchMinDurationInMinutesOverride(playerCategory);
			if (matchMinDurationInMinutesOverride.HasValue)
			{
				if (matchDurationInMinutes < matchMinDurationInMinutesOverride.Value)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:f2} < `matchMinDuration` = {1:f2} in `{2}` for category `{3}`.", matchDurationInMinutes, matchMinDurationInMinutesOverride.Value, afterMatch.Id, playerCategory);
				}
			}
			else if (matchDurationInMinutes < afterMatch.MatchMinDurationInMinutes)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0:f2} < `matchMinDuration` = {1:f2} in `{2}`.", matchDurationInMinutes, afterMatch.MatchMinDurationInMinutes, afterMatch.Id);
			}
			bool? flag = ((!winner) ? afterMatch.GetEnabledForLoserOverride(playerCategory) : afterMatch.GetEnabledForWinnerOverride(playerCategory));
			if (flag.HasValue)
			{
				if (!flag.Value)
				{
					return string.Format(CultureInfo.InvariantCulture, "Disabled for `winner: {0}` in `{1}` for category `{2}`.", winner, afterMatch.Id, playerCategory);
				}
			}
			else if (!((!winner) ? afterMatch.EnabledForLoser : afterMatch.EnabledForWinner))
			{
				return string.Format(CultureInfo.InvariantCulture, "Disabled for `winner: {0}` in `{1}`.", winner, afterMatch.Id);
			}
			return string.Empty;
		}

		protected override string GetReasonToSkip()
		{
			if (Initializer.Instance == null)
			{
				return "Initializer.Instance == null";
			}
			if (ShopNGUIController.GuiActive)
			{
				return "ShopNGUIController.GuiActive";
			}
			return string.Empty;
		}
	}
}

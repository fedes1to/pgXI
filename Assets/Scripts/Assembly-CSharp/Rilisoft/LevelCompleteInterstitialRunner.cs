using System.Collections.Generic;
using System.Globalization;
using UnityEngine.SceneManagement;

namespace Rilisoft
{
	internal sealed class LevelCompleteInterstitialRunner : FyberInterstitialRunnerBase
	{
		private readonly HashSet<string> _allowedScenes;

		public LevelCompleteInterstitialRunner()
		{
			_allowedScenes = new HashSet<string>
			{
				Defs.MainMenuScene,
				"LevelComplete",
				"ChooseLevel",
				"CampaignChooseBox",
				"PromScene",
				"LevelToCompleteProm",
				SceneManager.GetActiveScene().name
			};
		}

		public static string GetReasonToDismissInterstitialCampaign(bool afterDeath)
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
			CampaignAdPointMemento campaign = lastLoadedConfig.AdPointsConfig.Campaign;
			if (campaign == null)
			{
				return "Campaign config is `null`";
			}
			string playerCategory = AdsConfigManager.GetPlayerCategory(lastLoadedConfig);
			string disabledReason = campaign.GetDisabledReason(playerCategory);
			if (!string.IsNullOrEmpty(disabledReason))
			{
				return disabledReason;
			}
			string reasonToDismissInterstitialLevelComplete = GetReasonToDismissInterstitialLevelComplete(campaign, playerCategory, afterDeath);
			if (!string.IsNullOrEmpty(reasonToDismissInterstitialLevelComplete))
			{
				return reasonToDismissInterstitialLevelComplete;
			}
			return string.Empty;
		}

		public static string GetReasonToDismissInterstitialSurvivalArena(bool afterDeath)
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
			SurvivalArenaAdPointMemento survivalArena = lastLoadedConfig.AdPointsConfig.SurvivalArena;
			if (survivalArena == null)
			{
				return string.Format("`{0}` config is `null`", survivalArena.Id);
			}
			string playerCategory = AdsConfigManager.GetPlayerCategory(lastLoadedConfig);
			string disabledReason = survivalArena.GetDisabledReason(playerCategory);
			if (!string.IsNullOrEmpty(disabledReason))
			{
				return disabledReason;
			}
			string reasonToDismissInterstitialLevelComplete = GetReasonToDismissInterstitialLevelComplete(survivalArena, playerCategory, afterDeath);
			if (!string.IsNullOrEmpty(reasonToDismissInterstitialLevelComplete))
			{
				return reasonToDismissInterstitialLevelComplete;
			}
			int? waveMinCountOverride = survivalArena.GetWaveMinCountOverride(playerCategory);
			if (waveMinCountOverride.HasValue)
			{
				if (WavesSurvivedStat.SurvivedWaveCount < waveMinCountOverride.Value)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0} < `waveMinCount` = {1} in `{2}` for category `{3}`.", WavesSurvivedStat.SurvivedWaveCount, waveMinCountOverride.Value, survivalArena.Id, playerCategory);
				}
			}
			else if (WavesSurvivedStat.SurvivedWaveCount < survivalArena.WaveMinCount)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} < `waveMinCount` = {1} in `{2}`.", WavesSurvivedStat.SurvivedWaveCount, survivalArena.WaveMinCount, survivalArena.Id);
			}
			return string.Empty;
		}

		private static string GetReasonToDismissInterstitialLevelComplete(LevelCompleteAdPointMementoBase levelCompleteConfig, string category, bool afterDeath)
		{
			if (afterDeath)
			{
				bool? deathOverride = levelCompleteConfig.GetDeathOverride(category);
				if (deathOverride.HasValue)
				{
					if (!deathOverride.Value)
					{
						return string.Format(CultureInfo.InvariantCulture, "`death` in `{0}` explicitely disabled for category `{1}`.", levelCompleteConfig.Id, category);
					}
				}
				else if (!levelCompleteConfig.Death)
				{
					return string.Format(CultureInfo.InvariantCulture, "`death` in `{0}` disabled.", levelCompleteConfig.Id);
				}
			}
			else
			{
				bool? quitOverride = levelCompleteConfig.GetQuitOverride(category);
				if (quitOverride.HasValue)
				{
					if (!quitOverride.Value)
					{
						return string.Format(CultureInfo.InvariantCulture, "`quit` in `{0}` explicitely disabled for category `{1}`.", levelCompleteConfig.Id, category);
					}
				}
				else if (!levelCompleteConfig.Quit)
				{
					return string.Format(CultureInfo.InvariantCulture, "`quit` in `{0}` disabled.", levelCompleteConfig.Id);
				}
			}
			return string.Empty;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_allowedScenes.Clear();
		}

		protected override string GetReasonToSkip()
		{
			string name = SceneManager.GetActiveScene().name;
			if (_allowedScenes.Contains(name))
			{
				return string.Empty;
			}
			return "Scene is not allowed: " + name;
		}
	}
}

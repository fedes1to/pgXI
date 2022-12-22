using System.Reflection;
using Rilisoft;
using UnityEngine;

public sealed class CampaignLoading : MonoBehaviour
{
	public static readonly string DesignersTestMap = "Coliseum";

	public UITexture backgroundUiTexture;

	public GameObject survivalNotesOverlay;

	public GameObject campaignNotesOverlay;

	public GameObject trainingNotesOverlay;

	public GameObject ordinaryAwardLabel;

	public GameObject stackOfCoinsLabel;

	public Texture loadingNote;

	private Texture fonToDraw;

	private Texture plashkaCoins;

	private Rect plashkaCoinsRect;

	private void Start()
	{
		ActivityIndicator.IsActiveIndicator = true;
		string b;
		if (!Defs.IsSurvival)
		{
			if (TrainingController.TrainingCompleted)
			{
				int num = 0;
				LevelBox levelBox = null;
				foreach (LevelBox campaignBox in LevelBox.campaignBoxes)
				{
					if (!campaignBox.name.Equals(CurrentCampaignGame.boXName))
					{
						continue;
					}
					levelBox = campaignBox;
					foreach (CampaignLevel level in campaignBox.levels)
					{
						if (level.sceneName.Equals(CurrentCampaignGame.levelSceneName))
						{
							num = campaignBox.levels.IndexOf(level);
							break;
						}
					}
				}
				bool flag = false;
				flag = num >= levelBox.levels.Count - 1;
				bool flag2 = false;
				if (!CampaignProgress.boxesLevelsAndStars[CurrentCampaignGame.boXName].ContainsKey(CurrentCampaignGame.levelSceneName))
				{
					flag2 = true;
				}
				bool flag3 = flag2 && flag;
				b = ((!flag3) ? "gey_1" : "gey_15");
				if (ordinaryAwardLabel != null)
				{
					ordinaryAwardLabel.SetActive(!flag3);
				}
				if (stackOfCoinsLabel != null)
				{
					stackOfCoinsLabel.SetActive(flag3);
				}
				if (campaignNotesOverlay != null)
				{
					campaignNotesOverlay.SetActive(true);
				}
			}
			else
			{
				b = "Restore";
				if (trainingNotesOverlay != null)
				{
					trainingNotesOverlay.SetActive(true);
				}
			}
		}
		else
		{
			b = "gey_surv";
			if (survivalNotesOverlay != null)
			{
				survivalNotesOverlay.SetActive(true);
			}
		}
		plashkaCoins = Resources.Load<Texture>(ResPath.Combine("CoinsIndicationSystem", b));
		float num2 = (float)((!TrainingController.TrainingCompleted) ? 484 : 500) * Defs.Coef;
		float num3 = (float)((!TrainingController.TrainingCompleted) ? 279 : 244) * Defs.Coef;
		plashkaCoinsRect = new Rect(((float)Screen.width - num2) / 2f, (float)Screen.height * 0.8f - num3 / 2f, num2, num3);
		string text = ((!TrainingController.TrainingCompleted) ? "Training" : ((!Defs.IsSurvival) ? CurrentCampaignGame.levelSceneName : Defs.SurvivalMaps[Defs.CurrentSurvMapIndex % Defs.SurvivalMaps.Length]));
		string b2 = "Loading_" + text;
		fonToDraw = Resources.Load<Texture>(ResPath.Combine(Switcher.LoadingInResourcesPath + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi"), b2));
		if (backgroundUiTexture != null)
		{
			backgroundUiTexture.mainTexture = fonToDraw;
		}
		Invoke("Load", 2f);
	}

	[Obfuscation(Exclude = true)]
	private void Load()
	{
		if (Defs.IsSurvival)
		{
			Singleton<SceneLoader>.Instance.LoadScene(Defs.SurvivalMaps[Defs.CurrentSurvMapIndex % Defs.SurvivalMaps.Length]);
		}
		else
		{
			Singleton<SceneLoader>.Instance.LoadScene(TrainingController.TrainingCompleted ? CurrentCampaignGame.levelSceneName : "Training");
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

public class CampaignLevelCompleteRewardSettings : MonoBehaviour
{
	public UIGrid grid;

	public GameObject coinsMultiplierContainer;

	public GameObject gemsMultyplierContainer;

	public GameObject expMultiplier;

	public GameObject normalBackground;

	public GameObject premiumBackground;

	public List<UILabel> boxCompletedLabels;

	public List<UILabel> bossDefeatedHeader;

	public List<UILabel> missionHeader;

	public List<UILabel> coinsMultiplierLabels;

	public List<UILabel> gemsMultiplierLabels;

	public List<UILabel> expMultiplierLabels;

	public List<UILabel> coinsRewardLabels;

	public List<UILabel> gemsRewrdLabels;

	public List<UILabel> experienceRewardLabels;

	public Transform coinsReward;

	public Transform badcode;

	public Transform experienceReward;

	public Transform gemsReward;
}

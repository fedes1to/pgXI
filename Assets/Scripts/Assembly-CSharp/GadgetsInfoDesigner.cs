using System.Collections.Generic;
using UnityEngine;

public class GadgetsInfoDesigner : MonoBehaviour
{
	public PlayerEventScoreController.ScoreEvent killStreakType = PlayerEventScoreController.ScoreEvent.none;

	public List<GadgetInfo.Parameter> parameters;

	public List<WeaponSounds.Effects> effects;

	public string Lkey;

	public GadgetInfo.GadgetCategory category;

	public int tier_FROM_1;

	public GadgetsInfoDesigner previousUpgarde;

	public GadgetsInfoDesigner nextUpgrade;

	public string DescriptionLkey;

	public float Duration = 30f;

	public float Cooldown = 5f;
}

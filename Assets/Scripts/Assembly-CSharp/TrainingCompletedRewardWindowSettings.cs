using System.Collections.Generic;
using UnityEngine;

public class TrainingCompletedRewardWindowSettings : MonoBehaviour
{
	public List<UILabel> armorNameLabels;

	public UITexture armorTexture;

	public List<UILabel> exp;

	public List<UILabel> gems;

	public List<UILabel> coins;

	private void Awake()
	{
		foreach (UILabel item in exp)
		{
			item.text = string.Format(LocalizationStore.Get("Key_1532"), Defs.ExpForTraining);
		}
		foreach (UILabel gem in gems)
		{
			gem.text = string.Format(LocalizationStore.Get("Key_1531"), Defs.GemsForTraining);
		}
		foreach (UILabel coin in coins)
		{
			coin.text = string.Format(LocalizationStore.Get("Key_1530"), Defs.CoinsForTraining);
		}
		foreach (UILabel armorNameLabel in armorNameLabels)
		{
			armorNameLabel.text = LocalizationStore.Get((Storager.getInt("Training.NoviceArmorUsedKey", false) != 1) ? "Key_0724" : "Key_2045");
		}
		if (Storager.getInt("Training.NoviceArmorUsedKey", false) == 1)
		{
			armorTexture.mainTexture = Resources.Load<Texture2D>("OfferIcons/Armor_Novice_icon1_big");
		}
	}
}

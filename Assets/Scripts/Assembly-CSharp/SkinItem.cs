using UnityEngine;

public class SkinItem : ScriptableObject
{
	public enum CategoryNames
	{
		SkinsCategoryMale = 1100,
		SkinsCategoryFamele = 1200,
		SkinsCategorySpecial = 1300,
		SkinsCategoryPremium = 1400,
		LeagueSkinsCategory = 2200
	}

	public string skinStr = string.Empty;

	public int price = 20;

	public string currency = "Coins";

	public string localizeKey = string.Empty;

	public CategoryNames category = CategoryNames.SkinsCategoryMale;

	public RatingSystem.RatingLeague currentLeague = RatingSystem.RatingLeague.none;
}

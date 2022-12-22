using System;
using Rilisoft;
using UnityEngine;

public static class EggHatchingConditionFormatter
{
	public static string TextForConditionOfEgg(Egg egg)
	{
		if (egg == null || egg.Data == null)
		{
			Debug.LogError("TextForConditionOfEgg: egg == null || egg.Data == null || egg.Data.HatchedTypes.Count == 0");
			return string.Empty;
		}
		try
		{
			switch (egg.Data.HatchedType)
			{
			case EggHatchedType.League:
				return RatingSystem.instance.RatingNeededForLeague(egg.Data.League).ToString();
			case EggHatchedType.Time:
				if (egg.IncubationTimeLeft.HasValue)
				{
					long value = egg.IncubationTimeLeft.Value;
					return (value < 86400) ? RiliExtensions.GetTimeString(value) : string.Format("{0} {1}", LocalizationStore.Get("Key_1125"), RiliExtensions.GetTimeStringDays(value));
				}
				return LocalizationStore.Get("Key_2566");
			case EggHatchedType.Wins:
				return string.Format(LocalizationStore.Get("Key_2511"), egg.WinsLeft);
			case EggHatchedType.Rating:
				return string.Format(LocalizationStore.Get("Key_2732"), egg.RatingLeft);
			case EggHatchedType.Champion:
				break;
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in TextForConditionOfEgg: {0}", ex);
		}
		Debug.LogError("TextForConditionOfEgg: end of method reached");
		return string.Empty;
	}
}

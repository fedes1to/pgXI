using System.Collections.Generic;

namespace Rilisoft
{
	public class AchievementExistsGadgetsInAllCategories : Achievement
	{
		public AchievementExistsGadgetsInAllCategories(AchievementData data, AchievementProgressData progressData)
			: base(data, progressData)
		{
			if (!base.IsCompleted)
			{
				GadgetsInfo.OnGetGadget += GadgetsInfo_OnGetGadget;
				UpdateMe();
			}
		}

		private void GadgetsInfo_OnGetGadget(string obj)
		{
			if (!base.IsCompleted)
			{
				UpdateMe();
			}
		}

		private void UpdateMe()
		{
			List<GadgetInfo.GadgetCategory> existsCategories = new List<GadgetInfo.GadgetCategory>();
			RiliExtensions.ForEachEnum(delegate(GadgetInfo.GadgetCategory val)
			{
				Dictionary<string, GadgetInfo> dictionary = GadgetsInfo.GadgetsOfCategory(val);
				foreach (KeyValuePair<string, GadgetInfo> item in dictionary)
				{
					if (GadgetsInfo.IsBought(item.Key))
					{
						existsCategories.Add(val);
						break;
					}
				}
			});
			if (existsCategories.Count == RiliExtensions.EnumLen<GadgetInfo.GadgetCategory>())
			{
				Gain(1);
			}
		}

		public override void Dispose()
		{
			GadgetsInfo.OnGetGadget -= GadgetsInfo_OnGetGadget;
		}
	}
}

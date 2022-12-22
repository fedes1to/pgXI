using System.Collections.Generic;
using Rilisoft;

public static class InGameGadgetSet
{
	private static readonly Dictionary<GadgetInfo.GadgetCategory, Gadget> _inGameGadgets = new Dictionary<GadgetInfo.GadgetCategory, Gadget>(3, GadgetCategoryComparer.Instance);

	private static bool _inGameGadgetsInitialized = false;

	public static Dictionary<GadgetInfo.GadgetCategory, Gadget> CurrentSet
	{
		get
		{
			if (!_inGameGadgetsInitialized)
			{
				Renew();
			}
			return _inGameGadgets;
		}
	}

	public static void Renew()
	{
		string[] array = new string[3]
		{
			GadgetsInfo.EquippedForCategory(GadgetInfo.GadgetCategory.Throwing),
			GadgetsInfo.EquippedForCategory(GadgetInfo.GadgetCategory.Tools),
			GadgetsInfo.EquippedForCategory(GadgetInfo.GadgetCategory.Support)
		};
		_inGameGadgets.Clear();
		if (!array[0].IsNullOrEmpty())
		{
			_inGameGadgets.Add(GadgetInfo.GadgetCategory.Throwing, Gadget.Create(GadgetsInfo.info[array[0]]));
		}
		if (!array[1].IsNullOrEmpty())
		{
			_inGameGadgets.Add(GadgetInfo.GadgetCategory.Tools, Gadget.Create(GadgetsInfo.info[array[1]]));
		}
		if (!array[2].IsNullOrEmpty())
		{
			_inGameGadgets.Add(GadgetInfo.GadgetCategory.Support, Gadget.Create(GadgetsInfo.info[array[2]]));
		}
		_inGameGadgetsInitialized = true;
	}
}

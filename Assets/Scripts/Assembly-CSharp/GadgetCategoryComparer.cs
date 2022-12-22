using System.Collections.Generic;

internal sealed class GadgetCategoryComparer : IEqualityComparer<GadgetInfo.GadgetCategory>
{
	private static readonly GadgetCategoryComparer s_instance = new GadgetCategoryComparer();

	public static GadgetCategoryComparer Instance
	{
		get
		{
			return s_instance;
		}
	}

	public bool Equals(GadgetInfo.GadgetCategory x, GadgetInfo.GadgetCategory y)
	{
		return x == y;
	}

	public int GetHashCode(GadgetInfo.GadgetCategory obj)
	{
		return (int)obj;
	}
}

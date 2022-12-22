using System;
using Rilisoft;

[Serializable]
public class GiftNewPlayerInfo
{
	public GiftCategoryType TypeCategory;

	public SaltedInt Count = new SaltedInt(15645678, 0);

	public float Percent;
}

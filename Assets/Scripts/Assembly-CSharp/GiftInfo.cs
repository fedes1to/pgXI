using System;
using Rilisoft;
using UnityEngine;

[Serializable]
public class GiftInfo
{
	public string Id;

	public SaltedInt Count = new SaltedInt(12499947, 0);

	public float PercentAddInSlot;

	public string KeyTranslateInfo = string.Empty;

	[HideInInspector]
	public bool IsRandomSkin;

	[ReadOnly]
	public GiftInfo RootInfo;

	public ShopNGUIController.CategoryNames? TypeShopCat
	{
		get
		{
			ItemDb.GetByTag(Id);
			if (Id.IsNullOrEmpty())
			{
				return null;
			}
			int itemCategory = ItemDb.GetItemCategory(Id);
			if (itemCategory < 0)
			{
				return null;
			}
			return (ShopNGUIController.CategoryNames)itemCategory;
		}
	}

	public static GiftInfo CreateInfo(GiftInfo rootGift, string giftId, int count = 1)
	{
		GiftInfo giftInfo = new GiftInfo();
		giftInfo.Count = count;
		giftInfo.Id = giftId;
		giftInfo.KeyTranslateInfo = rootGift.KeyTranslateInfo;
		giftInfo.PercentAddInSlot = rootGift.PercentAddInSlot;
		giftInfo.RootInfo = rootGift;
		return giftInfo;
	}
}

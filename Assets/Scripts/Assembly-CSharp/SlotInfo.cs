using System;
using Rilisoft;
using UnityEngine;

[Serializable]
public class SlotInfo
{
	public GiftInfo gift;

	public int positionInScroll;

	public float percentGetSlot;

	public GiftCategory category;

	public bool NoDropped;

	[HideInInspector]
	public bool isActiveEvent;

	private SaltedInt _countGift = new SaltedInt(15645675, 0);

	[HideInInspector]
	public int numInScroll;

	public int CountGift
	{
		get
		{
			if (isActiveEvent)
			{
				return _countGift.Value;
			}
			return gift.Count.Value;
		}
		set
		{
			_countGift.Value = value;
		}
	}

	public bool CheckAvaliableGift()
	{
		if ((GiftController.Instance != null && gift == null) || category == null || !category.AvailableGift(gift.Id, category.Type))
		{
			GiftController.Instance.UpdateSlot(this);
			return true;
		}
		return false;
	}
}

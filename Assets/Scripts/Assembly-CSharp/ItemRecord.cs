using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemRecord
{
	public int Id { get; private set; }

	public string Tag { get; private set; }

	public string StorageId { get; private set; }

	public string PrefabName { get; private set; }

	public string ShopId { get; private set; }

	public string ShopDisplayName { get; private set; }

	public bool CanBuy { get; private set; }

	public bool Deactivated { get; private set; }

	public bool UseImagesFromFirstUpgrade { get; private set; }

	public ItemPrice Price
	{
		get
		{
			try
			{
				List<ItemPrice> value;
				if (BalanceController.GunPricesFromServer.TryGetValue(PrefabName, out value) && value != null)
				{
					string text = WeaponManager.FirstTagForOurTier(Tag);
					int num = ((!(text == Tag)) ? 1 : 0);
					if (value.Count > num && value[num] != null)
					{
						return value[num];
					}
					Debug.LogError("listServerPrices.Count > index && listServerPrices [index] != null: Tag = " + Tag);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in ItemRecord.Price: " + (PrefabName ?? "null") + " exception: " + ex);
			}
			return new ItemPrice(200, "Coins");
		}
	}

	public bool TemporaryGun
	{
		get
		{
			return ShopId != null && StorageId == null;
		}
	}

	public ItemRecord(int id, string tag, string storageId, string prefabName, string shopId, string shopDisplayName, bool canBuy, bool deactivated, List<ItemPrice> UNUSED_pricesForDiffTiers, bool useImageOfFirstUpgrade = false)
	{
		SetMainFields(id, tag, storageId, prefabName, shopId, shopDisplayName, canBuy, deactivated, useImageOfFirstUpgrade);
	}

	public ItemRecord(int id, string tag, string storageId, string prefabName, string shopId, string shopDisplayName, int UNUSED_price, bool canBuy, bool deactivated, string UNUSED_currency = "Coins", int UNUSED_secondCurrencyPrice = -1, bool useImageOfFirstUpgrade = false)
	{
		SetMainFields(id, tag, storageId, prefabName, shopId, shopDisplayName, canBuy, deactivated, useImageOfFirstUpgrade);
	}

	private void SetMainFields(int id, string tag, string storageId, string prefabName, string shopId, string shopDisplayName, bool canBuy, bool deactivated, bool useImageOfFirstUpgrade)
	{
		Id = id;
		Tag = tag;
		StorageId = storageId;
		PrefabName = prefabName;
		ShopId = shopId;
		ShopDisplayName = shopDisplayName;
		CanBuy = canBuy;
		Deactivated = deactivated;
		UseImagesFromFirstUpgrade = useImageOfFirstUpgrade;
	}
}

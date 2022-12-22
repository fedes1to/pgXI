using System;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public class SuperIncubatorWindowController : GeneralBannerWindow
{
	public GameObject offerPriceBackground;

	public GameObject saleContainer;

	public List<UILabel> saleLabels;

	public List<UILabel> priceLabels;

	private float m_lastTimeUpdated = float.MinValue;

	public Action BuyAction { get; set; }

	public virtual void HandleBuyButton()
	{
		Action buyAction = BuyAction;
		if (buyAction != null)
		{
			buyAction();
		}
	}

	private void UpdateGui()
	{
		ItemPrice price = ShopNGUIController.GetItemPrice("Eggs.SuperIncubatorId", ShopNGUIController.CategoryNames.EggsCategory);
		priceLabels.ForEach(delegate(UILabel label)
		{
			label.text = price.Price.ToString();
		});
		bool onlyServerDiscount;
		int discount = ShopNGUIController.DiscountFor("Eggs.SuperIncubatorId", out onlyServerDiscount);
		offerPriceBackground.SetActiveSafeSelf(discount > 0);
		saleContainer.SetActiveSafeSelf(discount > 0);
		saleLabels.ForEach(delegate(UILabel label)
		{
			label.text = string.Format(LocalizationStore.Get("Key_2555"), discount.ToString());
		});
		m_lastTimeUpdated = Time.realtimeSinceStartup;
	}

	private void Awake()
	{
		UpdateGui();
	}

	private void Start()
	{
		RegisterEscapeHandler();
	}

	private void Update()
	{
		if (Time.realtimeSinceStartup - 0.5f >= m_lastTimeUpdated)
		{
			UpdateGui();
		}
	}

	private void OnDestroy()
	{
		UnregisterEscapeHandler();
	}
}

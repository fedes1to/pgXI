using System;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public class BankViewItem : AbstractBankViewItem
{
	public GameObject normalAmountContainer;

	public GameObject x3AmountContainer;

	public List<UILabel> countLabelsList;

	public List<UILabel> countLabelsX3List;

	public List<UILabel> countLabel;

	public List<UILabel> countX3Label;

	public UISprite discountSprite;

	public UILabel discountPercentsLabel;

	public UISprite bestBuy;

	public ChestBonusButtonView bonusButtonView;

	private Animator _bestBuyAnimator;

	private Animator _discountAnimator;

	public override bool isX3Item
	{
		set
		{
			base.isX3Item = value;
			if (discountSprite != null)
			{
				discountSprite.gameObject.SetActive(!value && IsDiscounted());
			}
			if (!value && purchaseInfo != null && discountPercentsLabel != null && IsDiscounted())
			{
				discountPercentsLabel.text = string.Format("{0}%", purchaseInfo.Discount);
			}
			UpdateViewBestBuy();
			normalAmountContainer.SetActiveSafeSelf(!value);
			x3AmountContainer.SetActiveSafeSelf(value);
		}
	}

	private int Count
	{
		set
		{
			if (countLabel != null)
			{
				for (int i = 0; i < countLabel.Count; i++)
				{
					countLabel[i].text = value.ToString();
				}
			}
		}
	}

	private int CountX3
	{
		set
		{
			if (countX3Label != null)
			{
				for (int i = 0; i < countX3Label.Count; i++)
				{
					countX3Label[i].text = value.ToString();
				}
			}
		}
	}

	public override void Setup(IMarketProduct product, PurchaseEventArgs newPurchaseInfo, EventHandler clickHandler)
	{
		base.Setup(product, newPurchaseInfo, clickHandler);
		Count = purchaseInfo.Count;
		CountX3 = 3 * purchaseInfo.Count;
		if (bonusButtonView != null)
		{
			bonusButtonView.UpdateState(purchaseInfo);
		}
	}

	protected override void Awake()
	{
		_bestBuyAnimator = ((!(bestBuy == null)) ? bestBuy.GetComponent<Animator>() : null);
		_discountAnimator = ((!(discountSprite == null)) ? discountSprite.GetComponent<Animator>() : null);
		if (bonusButtonView != null)
		{
			bonusButtonView.Initialize();
			if (purchaseInfo != null)
			{
				bonusButtonView.UpdateState(purchaseInfo);
			}
		}
		PromoActionsManager.BestBuyStateUpdate += UpdateViewBestBuy;
		base.Awake();
	}

	protected override void OnEnable()
	{
		UpdateViewBestBuy();
	}

	protected override void OnDisable()
	{
		if (!Device.IsLoweMemoryDevice)
		{
		}
	}

	protected override void OnDestroy()
	{
		if (bonusButtonView != null)
		{
			bonusButtonView.Deinitialize();
		}
		PromoActionsManager.BestBuyStateUpdate -= UpdateViewBestBuy;
		base.OnDestroy();
	}

	protected virtual string PathToIcon()
	{
		return (!(purchaseInfo.Currency == "GemsCurrency")) ? ("Textures/Bank/Coins_Shop_" + (purchaseInfo.Index + 1)) : ("Textures/Bank/Coins_Shop_Gem_" + (purchaseInfo.Index + 1));
	}

	protected override void SetIcon()
	{
		if (purchaseInfo == null)
		{
			Debug.LogErrorFormat("BankViewItem.SetIcon: purchaseInfo == null");
			return;
		}
		string path = PathToIcon();
		icon.mainTexture = Resources.Load<Texture>(path);
	}

	private void UpdateAnimationEventSprite(bool isEventActive)
	{
		PromoActionsManager sharedManager = PromoActionsManager.sharedManager;
		if (sharedManager != null && sharedManager.IsEventX3Active)
		{
			return;
		}
		bool flag = IsDiscounted();
		if (flag && _discountAnimator != null)
		{
			if (isEventActive)
			{
				_discountAnimator.Play("DiscountAnimation");
			}
			else
			{
				_discountAnimator.Play("Idle");
			}
		}
		if (isEventActive && _bestBuyAnimator != null)
		{
			if (flag)
			{
				_bestBuyAnimator.Play("BestBuyAnimation");
			}
			else
			{
				_bestBuyAnimator.Play("Idle");
			}
		}
	}

	private void UpdateViewBestBuy()
	{
		if (purchaseInfo == null)
		{
			Debug.LogWarningFormat("UpdateViewBestBuy: purchaseInfo == null");
		}
		PromoActionsManager sharedManager = PromoActionsManager.sharedManager;
		bool flag = purchaseInfo != null && sharedManager != null && sharedManager.IsBankItemBestBuy(purchaseInfo);
		bestBuy.gameObject.SetActive(flag);
		UpdateAnimationEventSprite(flag);
	}
}

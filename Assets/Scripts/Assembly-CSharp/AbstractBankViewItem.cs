using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public abstract class AbstractBankViewItem : MonoBehaviour
{
	public List<UILabel> inappNameLabels;

	public UITexture icon;

	public UILabel priceLabel;

	public UIButton btnBuy;

	[NonSerialized]
	public PurchaseEventArgs purchaseInfo;

	public GameObject aDFree;

	public GameObject[] x3Elements;

	public GameObject[] usualElements;

	private IMarketProduct m_marketProduct;

	private Dictionary<string, object> m_inappBonusParameters;

	private bool m_isX3Item;

	private ButtonHandler m_purchaseButtonScript;

	public Dictionary<string, object> InappBonusParameters
	{
		get
		{
			return m_inappBonusParameters;
		}
		protected set
		{
			m_inappBonusParameters = value;
		}
	}

	public virtual string Price
	{
		set
		{
			if (priceLabel != null)
			{
				priceLabel.text = value ?? string.Empty;
			}
		}
	}

	public virtual bool isX3Item
	{
		get
		{
			return m_isX3Item;
		}
		set
		{
			m_isX3Item = value;
			for (int i = 0; i < x3Elements.Length; i++)
			{
				if (x3Elements[i] != null)
				{
					x3Elements[i].SetActive(value);
				}
			}
			for (int j = 0; j < usualElements.Length; j++)
			{
				if (usualElements[j] != null)
				{
					usualElements[j].SetActive(!value);
				}
			}
		}
	}

	protected IMarketProduct MarketProduct
	{
		get
		{
			return m_marketProduct;
		}
		set
		{
			m_marketProduct = value;
		}
	}

	private EventHandler PurchaseButtonHandler { get; set; }

	private ButtonHandler PurchaseButtonScript
	{
		get
		{
			if (m_purchaseButtonScript == null)
			{
				m_purchaseButtonScript = btnBuy.GetComponent<ButtonHandler>();
				if (m_purchaseButtonScript == null)
				{
					Debug.LogErrorFormat("BankViewItem.PurchaseButtonScript: m_purchaseButtonScript == null");
				}
			}
			return m_purchaseButtonScript;
		}
	}

	public virtual void Setup(IMarketProduct product, PurchaseEventArgs newPurchaseInfo, EventHandler clickHandler)
	{
		if (product == null)
		{
			Debug.LogErrorFormat("AbstractBankViewItem.Setup: product == null");
		}
		MarketProduct = product;
		List<Dictionary<string, object>> currentInnapBonus = BalanceController.GetCurrentInnapBonus();
		if (currentInnapBonus != null)
		{
			InappBonusParameters = currentInnapBonus.FirstOrDefault((Dictionary<string, object> bonus) => Convert.ToString(bonus["ID"]) == product.Id);
		}
		else
		{
			InappBonusParameters = null;
		}
		purchaseInfo = newPurchaseInfo;
		RemovePurchaseButtonHandler();
		PurchaseButtonHandler = clickHandler;
		AddPurchaseButtonHandler();
		SetIcon();
	}

	protected static bool PaymentOccursInLastTwoWeeks()
	{
		string @string = PlayerPrefs.GetString("Last Payment Time", string.Empty);
		DateTime result;
		if (!string.IsNullOrEmpty(@string) && DateTime.TryParse(@string, out result))
		{
			TimeSpan timeSpan = DateTime.UtcNow - result;
			return timeSpan <= TimeSpan.FromDays(14.0);
		}
		return false;
	}

	protected virtual void Awake()
	{
		UpdateAdFree();
	}

	protected abstract void OnEnable();

	protected abstract void OnDisable();

	protected virtual void Update()
	{
		UpdateAdFree();
	}

	protected virtual void OnDestroy()
	{
		RemovePurchaseButtonHandler();
	}

	protected bool IsDiscounted()
	{
		return purchaseInfo != null && purchaseInfo.Discount > 0;
	}

	protected abstract void SetIcon();

	private void AddPurchaseButtonHandler()
	{
		try
		{
			if (PurchaseButtonHandler != null)
			{
				PurchaseButtonScript.Clicked += PurchaseButtonHandler;
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in AddPurchaseButtonHandler: {0}", ex);
		}
	}

	private void RemovePurchaseButtonHandler()
	{
		try
		{
			if (PurchaseButtonHandler != null)
			{
				PurchaseButtonScript.Clicked -= PurchaseButtonHandler;
				PurchaseButtonHandler = null;
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in RemovePurchaseButtonHandler: {0}", ex);
		}
	}

	private void UpdateAdFree()
	{
		if (aDFree != null)
		{
			try
			{
				int reasonCodeToDismissInterstitialConnectScene = ConnectSceneNGUIController.GetReasonCodeToDismissInterstitialConnectScene();
				aDFree.SetActiveSafeSelf(reasonCodeToDismissInterstitialConnectScene == 0);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in UpdateAdFree: : {0}", ex);
			}
		}
	}
}

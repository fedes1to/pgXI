using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using Rilisoft;
using UnityEngine;

internal abstract class AbstractBankView : MonoBehaviour
{
	public static int[] discountsCoins = new int[7] { 0, 0, 7, 10, 12, 15, 33 };

	public static int[] discountsGems = new int[7] { 0, 0, 7, 10, 12, 15, 33 };

	public GameObject[] x3BankElements;

	public GameObject[] usualBankElements;

	public TweenColor colorBlinkForX3;

	public ButtonHandler backButton;

	public UILabel connectionProblemLabel;

	public UILabel crackersWarningLabel;

	public UILabel notEnoughCoinsLabel;

	public UILabel notEnoughGemsLabel;

	public UISprite purchaseSuccessfulLabel;

	public UILabel[] eventX3RemainTime;

	public UIButton freeAwardButton;

	public UIWidget eventX3AmazonBonusWidget;

	public UILabel amazonEventCaptionLabel;

	public UILabel amazonEventTitleLabel;

	public GameObject[] AdFreeLabels;

	private UILabel _freeAwardButtonLagelCont;

	private float _lastUpdateTime;

	private string _localizeSaleLabel;

	private StoreKitEventListener _storeKitEventListener;

	private bool m_isX3Bank;

	public static IList<PurchaseEventArgs> goldPurchasesInfo
	{
		get
		{
			List<PurchaseEventArgs> list = new List<PurchaseEventArgs>();
			list.Add(new PurchaseEventArgs(0, 0, 0m, "Coins", discountsCoins[0]));
			list.Add(new PurchaseEventArgs(1, 0, 0m, "Coins", discountsCoins[1]));
			list.Add(new PurchaseEventArgs(2, 0, 0m, "Coins", discountsCoins[2]));
			list.Add(new PurchaseEventArgs(3, 0, 0m, "Coins", discountsCoins[3]));
			list.Add(new PurchaseEventArgs(4, 0, 0m, "Coins", discountsCoins[4]));
			list.Add(new PurchaseEventArgs(5, 0, 0m, "Coins", discountsCoins[5]));
			list.Add(new PurchaseEventArgs(6, 0, 0m, "Coins", discountsCoins[6]));
			return list;
		}
	}

	public static IList<PurchaseEventArgs> gemsPurchasesInfo
	{
		get
		{
			List<PurchaseEventArgs> list = new List<PurchaseEventArgs>();
			list.Add(new PurchaseEventArgs(0, 0, 0m, "GemsCurrency", discountsGems[0]));
			list.Add(new PurchaseEventArgs(1, 0, 0m, "GemsCurrency", discountsGems[1]));
			list.Add(new PurchaseEventArgs(2, 0, 0m, "GemsCurrency", discountsGems[2]));
			list.Add(new PurchaseEventArgs(3, 0, 0m, "GemsCurrency", discountsGems[3]));
			list.Add(new PurchaseEventArgs(4, 0, 0m, "GemsCurrency", discountsGems[4]));
			list.Add(new PurchaseEventArgs(5, 0, 0m, "GemsCurrency", discountsGems[5]));
			list.Add(new PurchaseEventArgs(6, 0, 0m, "GemsCurrency", discountsGems[6]));
			return list;
		}
	}

	public string DesiredCurrency { get; set; }

	public bool ConnectionProblemLabelEnabled
	{
		get
		{
			return connectionProblemLabel != null && connectionProblemLabel.gameObject.GetActive();
		}
		set
		{
			if (connectionProblemLabel != null)
			{
				connectionProblemLabel.gameObject.SetActive(value);
			}
		}
	}

	public bool CrackersWarningLabelEnabled
	{
		get
		{
			return crackersWarningLabel != null && crackersWarningLabel.gameObject.GetActive();
		}
		set
		{
			if (crackersWarningLabel != null)
			{
				crackersWarningLabel.gameObject.SetActive(value);
			}
		}
	}

	public bool NotEnoughCoinsLabelEnabled
	{
		get
		{
			return notEnoughCoinsLabel != null && notEnoughCoinsLabel.gameObject.GetActive();
		}
		set
		{
			if (notEnoughCoinsLabel != null)
			{
				notEnoughCoinsLabel.gameObject.SetActive(value);
			}
		}
	}

	public bool NotEnoughGemsLabelEnabled
	{
		get
		{
			return notEnoughGemsLabel != null && notEnoughGemsLabel.gameObject.GetActive();
		}
		set
		{
			if (notEnoughGemsLabel != null)
			{
				notEnoughGemsLabel.gameObject.SetActive(value);
			}
		}
	}

	public bool PurchaseSuccessfulLabelEnabled
	{
		get
		{
			return purchaseSuccessfulLabel != null && purchaseSuccessfulLabel.gameObject.GetActive();
		}
		set
		{
			if (purchaseSuccessfulLabel != null)
			{
				purchaseSuccessfulLabel.gameObject.SetActive(value);
			}
		}
	}

	public virtual bool IsX3Bank
	{
		get
		{
			return m_isX3Bank;
		}
		set
		{
			m_isX3Bank = value;
			for (int i = 0; i < x3BankElements.Length; i++)
			{
				if (x3BankElements[i] != null)
				{
					x3BankElements[i].SetActiveSafeSelf(value);
				}
			}
			for (int j = 0; j < usualBankElements.Length; j++)
			{
				if (usualBankElements[j] != null)
				{
					usualBankElements[j].SetActiveSafeSelf(!value);
				}
			}
			IEnumerable<AbstractBankViewItem> enumerable = AllItems();
			foreach (AbstractBankViewItem item in enumerable)
			{
				item.isX3Item = value;
			}
		}
	}

	public abstract bool AreBankContentsEnabled { get; set; }

	private UILabel _freeAwardButtonLabel
	{
		get
		{
			if (_freeAwardButtonLagelCont != null)
			{
				return _freeAwardButtonLagelCont;
			}
			if (freeAwardButton == null)
			{
				return _freeAwardButtonLagelCont;
			}
			return _freeAwardButtonLagelCont = freeAwardButton.GetComponentInChildren<UILabel>();
		}
	}

	public event Action<AbstractBankViewItem> PurchaseButtonPressed;

	public event EventHandler BackButtonPressed
	{
		add
		{
			if (backButton != null)
			{
				backButton.Clicked += value;
			}
		}
		remove
		{
			if (backButton != null)
			{
				backButton.Clicked -= value;
			}
		}
	}

	public abstract void UpdateUi();

	protected void Awake()
	{
		BackButtonPressed += AbstractBankView_BackButtonPressed;
		_storeKitEventListener = UnityEngine.Object.FindObjectOfType<StoreKitEventListener>();
		if (_storeKitEventListener == null)
		{
			Debug.LogError("storeKitEventListener == null");
			HandleNoStoreKitEventListener();
		}
		else
		{
			OnEnable();
		}
	}

	private void AbstractBankView_BackButtonPressed(object sender, EventArgs e)
	{
		try
		{
			if (Device.IsLoweMemoryDevice)
			{
				ActivityIndicator.ClearMemory();
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in AbstractBankView_BackButtonPressed: {0}", ex);
		}
	}

	protected virtual void OnEnable()
	{
		_localizeSaleLabel = LocalizationStore.Get("Key_0419");
	}

	protected virtual void Start()
	{
		bool state = StoreKitEventListener.IsPayingUser();
		GameObject[] adFreeLabels = AdFreeLabels;
		foreach (GameObject go in adFreeLabels)
		{
			go.SetActiveSafeSelf(state);
		}
	}

	protected virtual void Update()
	{
		if (Time.realtimeSinceStartup - _lastUpdateTime >= 0.5f)
		{
			long eventX3RemainedTime = PromoActionsManager.sharedManager.EventX3RemainedTime;
			TimeSpan timeSpan = TimeSpan.FromSeconds(eventX3RemainedTime);
			string empty = string.Empty;
			empty = ((timeSpan.Days <= 0) ? string.Format("{0}: {1:00}:{2:00}:{3:00}", _localizeSaleLabel, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds) : string.Format("{0}: {1} {2} {3:00}:{4:00}:{5:00}", _localizeSaleLabel, timeSpan.Days, (timeSpan.Days != 1) ? "Days" : "Day", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
			if (eventX3RemainTime != null)
			{
				for (int i = 0; i < eventX3RemainTime.Length; i++)
				{
					eventX3RemainTime[i].text = empty;
				}
			}
			if (colorBlinkForX3 != null && timeSpan.TotalHours < (double)Defs.HoursToEndX3ForIndicate && !colorBlinkForX3.enabled)
			{
				colorBlinkForX3.enabled = true;
			}
			_lastUpdateTime = Time.realtimeSinceStartup;
		}
		if (_freeAwardButtonLabel != null && freeAwardButton.isActiveAndEnabled)
		{
			_freeAwardButtonLabel.text = ((!(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")) ? string.Format("[FFA300FF]{0}[-]", ScriptLocalization.Get("Key_1155")) : string.Format("[50CEFFFF]{0}[-]", ScriptLocalization.Get("Key_2046")));
		}
	}

	protected virtual void OnDestroy()
	{
		BackButtonPressed -= AbstractBankView_BackButtonPressed;
	}

	protected abstract void HandleNoStoreKitEventListener();

	protected abstract IEnumerable<AbstractBankViewItem> AllItems();

	protected virtual void UpdateItem(AbstractBankViewItem item, PurchaseEventArgs purchaseInfo)
	{
		bool flag = purchaseInfo.Currency == "GemsCurrency";
		string[] array = ((!flag) ? StoreKitEventListener.coinIds : StoreKitEventListener.gemsIds);
		if (purchaseInfo.Index >= array.Length)
		{
			Debug.LogErrorFormat("UpdateItem: purchaseInfo.Index < inAppIds.Length");
			return;
		}
		string inappId = array[purchaseInfo.Index];
		purchaseInfo.Count = ((!flag) ? VirtualCurrencyHelper.coinInappsQuantity[purchaseInfo.Index] : VirtualCurrencyHelper.gemsInappsQuantity[purchaseInfo.Index]);
		decimal num = ((!flag) ? VirtualCurrencyHelper.coinPriceIds[purchaseInfo.Index] : VirtualCurrencyHelper.gemsPriceIds[purchaseInfo.Index]);
		purchaseInfo.CurrencyAmount = num - 0.01m;
		string price = string.Format("${0}", purchaseInfo.CurrencyAmount);
		IMarketProduct marketProduct = null;
		marketProduct = _storeKitEventListener.Products.FirstOrDefault((IMarketProduct p) => p.Id == inappId);
		if (marketProduct != null)
		{
			price = marketProduct.Price;
		}
		else
		{
			Debug.LogWarning("marketProduct == null,    inappId: " + inappId);
		}
		item.Price = price;
		try
		{
			item.Setup(marketProduct, purchaseInfo, delegate
			{
				Action<AbstractBankViewItem> purchaseButtonPressed = this.PurchaseButtonPressed;
				if (purchaseButtonPressed != null)
				{
					purchaseButtonPressed(item);
				}
			});
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in setup of BankViewItem: {0}", ex);
		}
	}
}

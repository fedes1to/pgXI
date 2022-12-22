using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

internal class BankView : AbstractBankView
{
	public sealed class ItemsComparer : IComparer<AbstractBankViewItem>
	{
		private List<Dictionary<string, object>> m_inappBonusActions;

		public ItemsComparer(List<Dictionary<string, object>> inappBonusActions)
		{
			m_inappBonusActions = inappBonusActions;
		}

		public int Compare(AbstractBankViewItem x, AbstractBankViewItem y)
		{
			//Discarded unreachable code: IL_00b0, IL_00d2
			if (x is BonusBankViewItem && y is BonusBankViewItem)
			{
				if (m_inappBonusActions == null)
				{
					return 0;
				}
				try
				{
					BonusBankViewItem bonusBankViewItem = x as BonusBankViewItem;
					BonusBankViewItem bonusBankViewItem2 = y as BonusBankViewItem;
					string xUniqueId = bonusBankViewItem.InappBonusParameters["Key"] as string;
					string yUniqueId = bonusBankViewItem2.InappBonusParameters["Key"] as string;
					return m_inappBonusActions.FindIndex((Dictionary<string, object> bonus) => bonus["Key"] as string == xUniqueId).CompareTo(m_inappBonusActions.FindIndex((Dictionary<string, object> bonus) => bonus["Key"] as string == yUniqueId));
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Exception in ItemsComparer.Compare: {0}", ex);
					return 0;
				}
			}
			if (x is BonusBankViewItem)
			{
				return -1;
			}
			if (y is BonusBankViewItem)
			{
				return 1;
			}
			int value = ((y != null) ? y.purchaseInfo.Count : 0);
			return (!StoreKitEventListener.IsPayingUser()) ? x.purchaseInfo.Count.CompareTo(value) : value.CompareTo(x.purchaseInfo.Count);
		}
	}

	public GameObject btnTabContainer;

	public UIButton btnTabGold;

	public UIButton btnTabGems;

	public UIScrollView goldScrollView;

	public UIGrid goldItemGrid;

	public AbstractBankViewItem goldItemPrefab;

	public UIScrollView gemsScrollView;

	public UIGrid gemsItemGrid;

	public AbstractBankViewItem gemsItemPrefab;

	public AbstractBankViewItem inappBonusItemPrefab;

	private bool m_areBankContentsEnabled;

	public override bool AreBankContentsEnabled
	{
		get
		{
			return m_areBankContentsEnabled;
		}
		set
		{
			bool areBankContentsEnabled = m_areBankContentsEnabled;
			m_areBankContentsEnabled = value;
			btnTabContainer.SetActiveSafeSelf(value);
			if (value)
			{
				if (!areBankContentsEnabled)
				{
					bool isEnabled = btnTabGold.isEnabled;
					goldScrollView.gameObject.SetActiveSafeSelf(!isEnabled);
					gemsScrollView.gameObject.SetActiveSafeSelf(isEnabled);
					UpdateUi();
					ResetScrollView(isEnabled);
				}
			}
			else
			{
				goldScrollView.gameObject.SetActiveSafeSelf(value);
				gemsScrollView.gameObject.SetActiveSafeSelf(value);
			}
		}
	}

	protected override void HandleNoStoreKitEventListener()
	{
		if (goldItemPrefab != null)
		{
			goldItemPrefab.gameObject.SetActive(false);
		}
		if (gemsItemPrefab != null)
		{
			gemsItemPrefab.gameObject.SetActive(false);
		}
		if (inappBonusItemPrefab != null)
		{
			inappBonusItemPrefab.gameObject.SetActive(false);
		}
	}

	protected override void OnEnable()
	{
		UpdateUi();
		UIButton btnTab = btnTabGems;
		if (coinsShop.thisScript != null && coinsShop.thisScript.notEnoughCurrency == "Coins")
		{
			btnTab = btnTabGold;
		}
		else if (coinsShop.thisScript != null && coinsShop.thisScript.notEnoughCurrency == "GemsCurrency")
		{
			btnTab = btnTabGems;
		}
		else if (base.DesiredCurrency != null)
		{
			btnTab = ((!(base.DesiredCurrency == "GemsCurrency")) ? btnTabGold : btnTabGems);
		}
		else
		{
			try
			{
				List<Dictionary<string, object>> currentInnapBonus = BalanceController.GetCurrentInnapBonus();
				if (currentInnapBonus != null && currentInnapBonus.Count() > 0 && currentInnapBonus.All(delegate(Dictionary<string, object> inappBonus)
				{
					InappRememberedBonus actualBonusSizeForInappBonus = InappBonuessController.Instance.GetActualBonusSizeForInappBonus(inappBonus);
					return actualBonusSizeForInappBonus != null && !actualBonusSizeForInappBonus.InappId.IsNullOrEmpty() && StoreKitEventListener.coinIds.Contains(actualBonusSizeForInappBonus.InappId);
				}))
				{
					btnTab = btnTabGold;
				}
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in BankView.OnEnable GetCurrenceCurrentInnapBonus: {0}", ex);
			}
		}
		base.DesiredCurrency = null;
		OnBtnTabClick(btnTab);
		if (connectionProblemLabel != null)
		{
			connectionProblemLabel.text = LocalizationStore.Get("Key_0278");
		}
		base.OnEnable();
	}

	protected override void Start()
	{
		UIPanel component = goldScrollView.GetComponent<UIPanel>();
		component.UpdateAnchors();
		UIPanel component2 = gemsScrollView.GetComponent<UIPanel>();
		component2.UpdateAnchors();
		ResetScrollView(false);
		ResetScrollView(true);
		base.Start();
	}

	private static void ClearGrid(UIGrid itemGrid)
	{
		while (itemGrid.transform.childCount > 0)
		{
			Transform child = itemGrid.transform.GetChild(0);
			child.parent = null;
			UnityEngine.Object.Destroy(child.gameObject);
		}
	}

	private void PopulateItemGrid(bool isGems, List<Dictionary<string, object>> inappBonusActions)
	{
		IList<PurchaseEventArgs> list2;
		if (isGems)
		{
			IList<PurchaseEventArgs> list = AbstractBankView.gemsPurchasesInfo;
			list2 = list;
		}
		else
		{
			list2 = AbstractBankView.goldPurchasesInfo;
		}
		IList<PurchaseEventArgs> list3 = list2;
		UIGrid uIGrid = ((!isGems) ? goldItemGrid : gemsItemGrid);
		AbstractBankViewItem abstractBankViewItem = ((!isGems) ? goldItemPrefab : gemsItemPrefab);
		abstractBankViewItem.gameObject.SetActiveSafeSelf(true);
		for (int i = 0; i < list3.Count; i++)
		{
			AbstractBankViewItem abstractBankViewItem2 = UnityEngine.Object.Instantiate(abstractBankViewItem);
			abstractBankViewItem2.transform.SetParent(uIGrid.transform);
			abstractBankViewItem2.transform.localScale = Vector3.one;
			abstractBankViewItem2.transform.localPosition = Vector3.zero;
			abstractBankViewItem2.transform.localRotation = Quaternion.identity;
			UpdateItem(abstractBankViewItem2, list3[i]);
		}
		abstractBankViewItem.gameObject.SetActiveSafeSelf(false);
		if (inappBonusActions != null)
		{
			for (int j = 0; j < inappBonusActions.Count; j++)
			{
				Dictionary<string, object> dictionary = inappBonusActions[j];
				PurchaseEventArgs purchaseEventArgs = null;
				InappRememberedBonus actualBonusSizeForInappBonus = InappBonuessController.Instance.GetActualBonusSizeForInappBonus(dictionary);
				if (actualBonusSizeForInappBonus != null)
				{
					if (!actualBonusSizeForInappBonus.InappId.IsNullOrEmpty())
					{
						int indexOfInappWithBonus = Array.IndexOf((!isGems) ? StoreKitEventListener.coinIds : StoreKitEventListener.gemsIds, actualBonusSizeForInappBonus.InappId);
						if (indexOfInappWithBonus != -1)
						{
							purchaseEventArgs = list3.FirstOrDefault((PurchaseEventArgs purchaseInfo) => purchaseInfo.Index == indexOfInappWithBonus);
							if (purchaseEventArgs == null)
							{
								Debug.LogErrorFormat("PopulateItemGrid inappBonusPurchaseInfo == null isGems = {0} , inappBonusAction = {1}, bonus.InappId = {2}", isGems, Json.Serialize(dictionary), actualBonusSizeForInappBonus.InappId);
							}
						}
					}
					else
					{
						Debug.LogErrorFormat("PopulateItemGrid: bonus.InappId.IsNullOrEmpty() isGems = {0} , inappBonusAction = {1}", isGems, Json.Serialize(dictionary));
					}
				}
				else
				{
					Debug.LogErrorFormat("PopulateItemGrid: bonus == null isGems = {0} , inappBonusAction = {1}", isGems, Json.Serialize(dictionary));
				}
				if (purchaseEventArgs != null)
				{
					inappBonusItemPrefab.gameObject.SetActiveSafeSelf(true);
					AbstractBankViewItem abstractBankViewItem3 = UnityEngine.Object.Instantiate(inappBonusItemPrefab);
					abstractBankViewItem3.transform.SetParent(uIGrid.transform);
					abstractBankViewItem3.transform.localScale = Vector3.one;
					abstractBankViewItem3.transform.localPosition = Vector3.zero;
					abstractBankViewItem3.transform.localRotation = Quaternion.identity;
					UpdateItem(abstractBankViewItem3, purchaseEventArgs);
				}
			}
		}
		inappBonusItemPrefab.gameObject.SetActiveSafeSelf(false);
		ResetScrollView(isGems);
	}

	public void OnBtnTabClick(UIButton btnTab)
	{
		bool flag = btnTab == btnTabGems;
		btnTabGold.isEnabled = flag;
		btnTabGems.isEnabled = !flag;
		goldScrollView.gameObject.SetActiveSafeSelf(!flag);
		gemsScrollView.gameObject.SetActiveSafeSelf(flag);
		ResetScrollView(flag);
		if (btnTab != btnTabGold && btnTab != btnTabGems)
		{
			Debug.LogErrorFormat("Unknown btnTab");
		}
	}

	public override void UpdateUi()
	{
		ClearGrid(goldItemGrid);
		ClearGrid(gemsItemGrid);
		if (!AreBankContentsEnabled)
		{
			IsX3Bank = false;
			return;
		}
		List<Dictionary<string, object>> inappBonusActions = null;
		try
		{
			inappBonusActions = BalanceController.GetCurrentInnapBonus();
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in UpdateUi BalanceController.GetCurrentInnapBonus: {0}", ex);
		}
		PopulateItemGrid(false, inappBonusActions);
		PopulateItemGrid(true, inappBonusActions);
		SortItemGrid(false, inappBonusActions);
		SortItemGrid(true, inappBonusActions);
		try
		{
			IsX3Bank = PromoActionsManager.sharedManager != null && PromoActionsManager.sharedManager.IsEventX3Active;
		}
		catch (Exception ex2)
		{
			Debug.LogErrorFormat("Exception in UpdateUi: {0}", ex2);
		}
	}

	private void SortItemGrid(bool isGems, List<Dictionary<string, object>> inappBonusActions)
	{
		UIGrid uIGrid = ((!isGems) ? goldItemGrid : gemsItemGrid);
		Transform transform = uIGrid.transform;
		List<AbstractBankViewItem> list = new List<AbstractBankViewItem>();
		for (int i = 0; i < transform.childCount; i++)
		{
			AbstractBankViewItem component = transform.GetChild(i).GetComponent<AbstractBankViewItem>();
			list.Add(component);
		}
		list.Sort(new ItemsComparer(inappBonusActions));
		for (int j = 0; j < list.Count; j++)
		{
			list[j].gameObject.name = string.Format("{0:00}", j);
		}
		ResetScrollView(isGems);
	}

	private void ResetScrollView(bool isGems)
	{
		UIScrollView uIScrollView = ((!isGems) ? goldScrollView : gemsScrollView);
		UIGrid uIGrid = ((!isGems) ? goldItemGrid : gemsItemGrid);
		uIGrid.Reposition();
		uIScrollView.ResetPosition();
	}

	protected override IEnumerable<AbstractBankViewItem> AllItems()
	{
		return (gemsItemGrid.GetComponentsInChildren<AbstractBankViewItem>() ?? new AbstractBankViewItem[0]).Concat(goldItemGrid.GetComponentsInChildren<AbstractBankViewItem>() ?? new AbstractBankViewItem[0]);
	}
}

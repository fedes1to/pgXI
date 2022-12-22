using System;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

internal sealed class PremiumAccountScreenController : MonoBehaviour
{
	public GameObject tapToActivate;

	public GameObject window;

	public UIButton[] rentButtons;

	public List<UILabel> headerLabels;

	public static PremiumAccountScreenController Instance;

	private bool ranksBefore;

	public string Header { get; set; }

	private bool InitialFreeAvailable
	{
		get
		{
			return Storager.getInt("PremiumInitialFree1Day", false) == 0;
		}
	}

	private void Start()
	{
		if (SceneLoader.ActiveSceneName.Equals(Defs.MainMenuScene) && ExperienceController.sharedController != null)
		{
			ranksBefore = ExperienceController.sharedController.isShowRanks;
			ExperienceController.sharedController.isShowRanks = false;
		}
		UpdateFreeButtons();
		for (int i = 0; i < rentButtons.Length; i++)
		{
			foreach (Transform item in rentButtons[i].transform)
			{
				if (item.name.Equals("GemsIcon"))
				{
					PremiumAccountController.AccountType accountType = (PremiumAccountController.AccountType)i;
					string key = accountType.ToString();
					ItemPrice itemPrice = VirtualCurrencyHelper.Price(key);
					UILabel component = item.GetChild(0).GetComponent<UILabel>();
					component.text = itemPrice.Price.ToString();
					break;
				}
			}
		}
		Instance = this;
	}

	public void HandleRentButtonPressed(UIButton button)
	{
		PremiumAccountController.AccountType accType = (PremiumAccountController.AccountType)Array.IndexOf(rentButtons, button);
		ItemPrice itemPrice = VirtualCurrencyHelper.Price(accType.ToString());
		Action<PremiumAccountController.AccountType> provideAcc = delegate(PremiumAccountController.AccountType at)
		{
			if (PremiumAccountController.Instance != null)
			{
				PremiumAccountController.Instance.BuyAccount(at);
			}
			UpdateFreeButtons();
		};
		if (InitialFreeAvailable && accType == PremiumAccountController.AccountType.OneDay)
		{
			SetInitialFreeUsed();
			provideAcc(accType);
			Hide();
			return;
		}
		int priceAmount = itemPrice.Price;
		string priceCurrency = itemPrice.Currency;
		ShopNGUIController.TryToBuy(window, itemPrice, delegate
		{
			provideAcc(accType);
			AnalyticsStuff.LogSales(accType.ToString(), "Premium Account");
			AnalyticsFacade.InAppPurchase(accType.ToString(), "Premium Account", 1, priceAmount, priceCurrency);
			if (InitialFreeAvailable)
			{
				SetInitialFreeUsed();
				provideAcc(PremiumAccountController.AccountType.OneDay);
			}
			Hide();
		});
	}

	public void Hide()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void UpdateFreeButtons()
	{
		bool initialFreeAvailable = InitialFreeAvailable;
		foreach (Transform item in rentButtons[0].transform)
		{
			if (item.name.Equals("Free"))
			{
				item.gameObject.SetActive(initialFreeAvailable);
			}
			if (item.name.Equals("GemsIcon"))
			{
				item.gameObject.SetActive(!initialFreeAvailable);
			}
		}
		tapToActivate.SetActive(initialFreeAvailable);
	}

	private void SetInitialFreeUsed()
	{
		Storager.setInt("PremiumInitialFree1Day", 1, false);
	}

	private void OnDestroy()
	{
		Instance = null;
		if (SceneLoader.ActiveSceneName.Equals(Defs.MainMenuScene) && ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.isShowRanks = ranksBefore;
		}
	}
}

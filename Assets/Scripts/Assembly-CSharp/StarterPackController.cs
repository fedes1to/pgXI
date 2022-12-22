using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.amazon.device.iap.cpt;
using Prime31;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

internal sealed class StarterPackController : MonoBehaviour
{
	public delegate void OnStarterPackEnableDelegate(bool enable);

	private DateTime _timeStartEvent;

	private TimeSpan _timeLiveEvent;

	private TimeSpan _timeToEndEvent;

	private List<StarterPackData> _starterPacksData;

	private int _orderCurrentPack;

	private bool _isDownloadDataRun;

	private StoreKitEventListener _storeKitEventListener;

	private float _lastCheckEventTime;

	private float timeUpdateConfig = -3600f;

	public bool isEventActive { get; private set; }

	public static StarterPackController Get { get; private set; }

	private List<string> BuyAnroidStarterPack { get; set; }

	public static event OnStarterPackEnableDelegate OnStarterPackEnable;

	private void Start()
	{
		Get = this;
		_timeLiveEvent = default(TimeSpan);
		_starterPacksData = new List<StarterPackData>();
		_orderCurrentPack = -1;
		_storeKitEventListener = UnityEngine.Object.FindObjectOfType<StoreKitEventListener>();
		BuyAnroidStarterPack = new List<string>();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Destroy()
	{
		Get = null;
	}

	private void Update()
	{
		if (isEventActive && Time.realtimeSinceStartup - _lastCheckEventTime >= 1f)
		{
			_timeLiveEvent = DateTime.UtcNow - _timeStartEvent;
			isEventActive = _timeLiveEvent <= StarterPackModel.MaxLiveTimeEvent;
			if (!isEventActive)
			{
				FinishCurrentStarterPack();
			}
			_lastCheckEventTime = Time.realtimeSinceStartup;
		}
	}

	private void FinishCurrentStarterPack()
	{
		Storager.setString("StartTimeShowStarterPack", string.Empty, false);
		Storager.setString("TimeEndStarterPack", DateTime.UtcNow.ToString("s"), false);
		Storager.setInt("NextNumberStarterPack", _orderCurrentPack + 1, false);
		isEventActive = false;
		CheckSendEventChangeEnabled();
	}

	private void CancelCurrentEvent()
	{
		Storager.setString("StartTimeShowStarterPack", string.Empty, false);
		Storager.setString("TimeEndStarterPack", string.Empty, false);
		Storager.setInt("NextNumberStarterPack", 0, false);
		PlayerPrefs.SetString("LastTimeShowStarterPack", string.Empty);
		PlayerPrefs.SetInt("CountShownStarterPack", 1);
		PlayerPrefs.Save();
		isEventActive = false;
		CheckSendEventChangeEnabled();
	}

	private void ResetToDefaultStateIfNeed()
	{
		if (Storager.hasKey("StartTimeShowStarterPack"))
		{
			string @string = Storager.getString("StartTimeShowStarterPack", false);
			if (!string.IsNullOrEmpty(@string))
			{
				Storager.setString("StartTimeShowStarterPack", string.Empty, false);
			}
		}
		if (Storager.hasKey("TimeEndStarterPack"))
		{
			string string2 = Storager.getString("TimeEndStarterPack", false);
			if (!string.IsNullOrEmpty(string2))
			{
				Storager.setString("TimeEndStarterPack", string.Empty, false);
			}
		}
		int @int = Storager.getInt("NextNumberStarterPack", false);
		if (@int > 0)
		{
			Storager.setInt("NextNumberStarterPack", 0, false);
		}
		string string3 = PlayerPrefs.GetString("LastTimeShowStarterPack", string.Empty);
		if (!string.IsNullOrEmpty(string3))
		{
			PlayerPrefs.SetString("LastTimeShowStarterPack", string.Empty);
		}
		int int2 = PlayerPrefs.GetInt("CountShownStarterPack", 1);
		if (int2 != 1)
		{
			PlayerPrefs.SetInt("CountShownStarterPack", 1);
		}
	}

	private void CheckCancelCurrentStarterPack()
	{
		ResetToDefaultStateIfNeed();
		if (isEventActive)
		{
			isEventActive = false;
			CheckSendEventChangeEnabled();
		}
	}

	public void CheckFindStoreKitEventListner()
	{
		if (!(_storeKitEventListener != null))
		{
			_storeKitEventListener = UnityEngine.Object.FindObjectOfType<StoreKitEventListener>();
		}
	}

	private IEnumerator DownloadDataAboutEvent()
	{
		if (_isDownloadDataRun)
		{
			yield break;
		}
		_isDownloadDataRun = true;
		string eventDataAddress = StarterPackModel.GetUrlForDownloadEventData();
		WWW downloadData = Tools.CreateWwwIfNotConnected(eventDataAddress);
		if (downloadData == null)
		{
			_isDownloadDataRun = false;
			yield break;
		}
		yield return downloadData;
		if (!string.IsNullOrEmpty(downloadData.error))
		{
			Debug.LogFormat("DownloadDataAboutEvent error: {0}", downloadData.error);
			_starterPacksData.Clear();
			_isDownloadDataRun = false;
			yield break;
		}
		string responseText = URLs.Sanitize(downloadData);
		Dictionary<string, object> eventData = Rilisoft.MiniJson.Json.Deserialize(responseText) as Dictionary<string, object>;
		if (eventData == null)
		{
			Debug.Log("DownloadDataAboutEvent eventData = null");
			_isDownloadDataRun = false;
			yield break;
		}
		_starterPacksData.Clear();
		if (!eventData.ContainsKey("packs"))
		{
			_isDownloadDataRun = false;
			yield break;
		}
		List<object> packsList = eventData["packs"] as List<object>;
		if (packsList != null)
		{
			for (int i = 0; i < packsList.Count; i++)
			{
				Dictionary<string, object> element = packsList[i] as Dictionary<string, object>;
				if (element == null)
				{
					continue;
				}
				StarterPackData data = new StarterPackData();
				if (element.ContainsKey("blockLevel"))
				{
					data.blockLevel = Convert.ToInt32((long)element["blockLevel"]);
				}
				if (element.ContainsKey("coinsCost"))
				{
					data.coinsCost = Convert.ToInt32((long)element["coinsCost"]);
				}
				else if (element.ContainsKey("gemsCost"))
				{
					data.gemsCost = Convert.ToInt32((long)element["gemsCost"]);
				}
				if (element.ContainsKey("enable"))
				{
					int enableFlag = Convert.ToInt32((long)element["enable"]);
					data.enable = enableFlag == 1;
				}
				if (element.ContainsKey("items"))
				{
					List<object> itemsObjects = element["items"] as List<object>;
					if (itemsObjects != null)
					{
						data.items = new List<StarterPackItemData>();
						for (int j = 0; j < itemsObjects.Count; j++)
						{
							Dictionary<string, object> itemsElement = itemsObjects[j] as Dictionary<string, object>;
							StarterPackItemData newItem = new StarterPackItemData();
							if (itemsElement == null)
							{
								continue;
							}
							if (itemsElement.ContainsKey("tagsVariant"))
							{
								List<object> tagsVariant = itemsElement["tagsVariant"] as List<object>;
								if (tagsVariant != null)
								{
									for (int k = 0; k < tagsVariant.Count; k++)
									{
										newItem.variantTags.Add((string)tagsVariant[k]);
									}
								}
							}
							if (itemsElement.ContainsKey("count"))
							{
								newItem.count = Convert.ToInt32((long)itemsElement["count"]);
							}
							data.items.Add(newItem);
						}
					}
				}
				if (element.ContainsKey("sale"))
				{
					data.sale = Convert.ToInt32((long)element["sale"]);
				}
				if (element.ContainsKey("coinsCount"))
				{
					data.coinsCount = Convert.ToInt32((long)element["coinsCount"]);
				}
				if (element.ContainsKey("gemsCount"))
				{
					data.gemsCount = Convert.ToInt32((long)element["gemsCount"]);
				}
				_starterPacksData.Add(data);
			}
		}
		_isDownloadDataRun = false;
	}

	private bool IsPlayerNotPayBeforeStartEvent()
	{
		if (Storager.getInt("PayingUser", true) == 0)
		{
			return true;
		}
		if (isEventActive)
		{
			return true;
		}
		return false;
	}

	private int GetMaxValidOrderPack()
	{
		int result = -1;
		int currentLevel = ExperienceController.GetCurrentLevel();
		for (int i = 0; i < _starterPacksData.Count; i++)
		{
			if (_starterPacksData[i].blockLevel <= currentLevel)
			{
				result = i;
			}
		}
		return result;
	}

	private int GetOrderCurrentPack()
	{
		int @int = Storager.getInt("NextNumberStarterPack", false);
		if (@int >= _starterPacksData.Count)
		{
			return -1;
		}
		return @int;
	}

	private bool IsCurrentPackEnable()
	{
		StarterPackData currentPackData = GetCurrentPackData();
		if (currentPackData == null)
		{
			return true;
		}
		return currentPackData.enable;
	}

	private bool IsStarterPackBuyByPackOrder(int packOrder)
	{
		string storageIdByPackOrder = GetStorageIdByPackOrder(packOrder);
		if (string.IsNullOrEmpty(storageIdByPackOrder))
		{
			return false;
		}
		return IsStarterPackBuy(storageIdByPackOrder);
	}

	private bool IsInvalidCurrentPack()
	{
		return IsInvalidPack(_orderCurrentPack);
	}

	private bool IsInvalidPack(int packOrder)
	{
		if (GetPackType(packOrder) != 0)
		{
			return false;
		}
		List<StarterPackItemData> items = _starterPacksData[packOrder].items;
		for (int i = 0; i < items.Count; i++)
		{
			if (string.IsNullOrEmpty(items[i].validTag))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsEventInEndState()
	{
		string @string = Storager.getString("StartTimeShowStarterPack", false);
		string string2 = Storager.getString("TimeEndStarterPack", false);
		return @string == string.Empty && !string.IsNullOrEmpty(string2);
	}

	private bool IsCooldownEventEnd()
	{
		DateTime utcNow = DateTime.UtcNow;
		DateTime timeDataEvent = StarterPackModel.GetTimeDataEvent("TimeEndStarterPack");
		return utcNow - timeDataEvent >= StarterPackModel.CooldownTimeEvent;
	}

	private IEnumerator CheckStartStarterPackEvent()
	{
		if (!TrainingController.TrainingCompleted)
		{
			CheckCancelCurrentStarterPack();
			yield break;
		}
		if (!IsPlayerNotPayBeforeStartEvent() || ExpController.LobbyLevel < 3)
		{
			CheckCancelCurrentStarterPack();
			yield break;
		}
		if (Time.time - timeUpdateConfig > 3600f)
		{
			yield return StartCoroutine(DownloadDataAboutEvent());
			timeUpdateConfig = Time.time;
		}
		if (IsEventInEndState() && !IsCooldownEventEnd())
		{
			yield break;
		}
		_orderCurrentPack = GetOrderCurrentPack();
		if (_orderCurrentPack == -1)
		{
			CheckCancelCurrentStarterPack();
			yield break;
		}
		int maxValidOrder = GetMaxValidOrderPack();
		if (maxValidOrder == -1)
		{
			CheckCancelCurrentStarterPack();
			yield break;
		}
		if (maxValidOrder > _orderCurrentPack)
		{
			_orderCurrentPack = maxValidOrder;
			Storager.setInt("NextNumberStarterPack", _orderCurrentPack, false);
			Storager.setString("StartTimeShowStarterPack", string.Empty, false);
		}
		else if (maxValidOrder < _orderCurrentPack)
		{
			CheckCancelCurrentStarterPack();
			yield break;
		}
		if (!IsCurrentPackEnable())
		{
			CheckCancelCurrentStarterPack();
			yield break;
		}
		if (IsStarterPackBuyOnAndroid(_orderCurrentPack))
		{
			CheckCancelCurrentStarterPack();
			yield break;
		}
		if (IsStarterPackBuyByPackOrder(_orderCurrentPack))
		{
			CheckCancelCurrentStarterPack();
			yield break;
		}
		if (IsInvalidCurrentPack())
		{
			CheckCancelCurrentStarterPack();
			yield break;
		}
		string timeStartSting = Storager.getString("StartTimeShowStarterPack", false);
		if (string.IsNullOrEmpty(timeStartSting))
		{
			InitializeEvent();
		}
		else
		{
			_timeStartEvent = StarterPackModel.GetTimeDataEvent("StartTimeShowStarterPack");
		}
		bool previusActive = isEventActive;
		isEventActive = true;
		if (previusActive != isEventActive)
		{
			CheckSendEventChangeEnabled();
		}
	}

	public void CheckShowStarterPack()
	{
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
		{
			Debug.Log("Skipping CheckShowStarterPack() on WSA.");
		}
		else
		{
			StartCoroutine(CheckStartStarterPackEvent());
		}
	}

	private void InitializeEvent()
	{
		_timeStartEvent = DateTime.UtcNow;
		Storager.setString("StartTimeShowStarterPack", _timeStartEvent.ToString("s"), false);
		Storager.setString("TimeEndStarterPack", string.Empty, false);
	}

	public StarterPackData GetCurrentPackData()
	{
		if (_orderCurrentPack == -1)
		{
			return null;
		}
		return _starterPacksData[_orderCurrentPack];
	}

	public StarterPackModel.TypePack GetPackType(int packOrder)
	{
		if (packOrder == -1)
		{
			return StarterPackModel.TypePack.None;
		}
		if (_starterPacksData[packOrder].items != null)
		{
			return StarterPackModel.TypePack.Items;
		}
		if (_starterPacksData[packOrder].coinsCount > 0)
		{
			return StarterPackModel.TypePack.Coins;
		}
		if (_starterPacksData[packOrder].gemsCount > 0)
		{
			return StarterPackModel.TypePack.Gems;
		}
		return StarterPackModel.TypePack.None;
	}

	public StarterPackModel.TypePack GetCurrentPackType()
	{
		return GetPackType(_orderCurrentPack);
	}

	public bool TryTakePurchasesForCurrentPack(string productId, bool isRestore = false)
	{
		return TryTakePurchases(productId, _orderCurrentPack, isRestore);
	}

	private bool IsStarterPackBuy(string storageId)
	{
		return Storager.hasKey(storageId) && Storager.getInt(storageId, false) == 1;
	}

	private bool TryTakePurchases(string productId, int packOrder, bool isRestore = false)
	{
		if (_starterPacksData.Count == 0)
		{
			return false;
		}
		if (packOrder == -1)
		{
			return false;
		}
		StarterPackModel.TypePack packType = GetPackType(packOrder);
		if (packType == StarterPackModel.TypePack.None)
		{
			return false;
		}
		if (IsStarterPackBuy(productId))
		{
			return false;
		}
		StarterPackData starterPackData = _starterPacksData[packOrder];
		switch (packType)
		{
		case StarterPackModel.TypePack.Coins:
			BankController.AddCoins(starterPackData.coinsCount, true, AnalyticsConstants.AccrualType.Purchased);
			StoreKitEventListener.LogVirtualCurrencyPurchased(productId, starterPackData.coinsCount, false);
			break;
		case StarterPackModel.TypePack.Gems:
			BankController.AddGems(starterPackData.gemsCount, true, AnalyticsConstants.AccrualType.Purchased);
			StoreKitEventListener.LogVirtualCurrencyPurchased(productId, starterPackData.gemsCount, true);
			break;
		case StarterPackModel.TypePack.Items:
		{
			if (starterPackData.items.Count == 0)
			{
				break;
			}
			for (int i = 0; i < starterPackData.items.Count; i++)
			{
				string validTag = starterPackData.items[i].validTag;
				int itemCategory = ItemDb.GetItemCategory(validTag);
				int count = starterPackData.items[i].count;
				if (itemCategory == 7 || ShopNGUIController.IsWeaponCategory((ShopNGUIController.CategoryNames)itemCategory))
				{
					ShopNGUIController.FireWeaponOrArmorBought();
				}
				ShopNGUIController.ProvideItem((ShopNGUIController.CategoryNames)itemCategory, validTag, count, false, 0, null, null, true, true, false);
			}
			if (ShopNGUIController.sharedShop != null && ShopNGUIController.sharedShop.wearEquipAction != null)
			{
				ShopNGUIController.sharedShop.wearEquipAction(ShopNGUIController.CategoryNames.ArmorCategory, string.Empty, string.Empty);
			}
			if (ShopNGUIController.sharedShop != null && ShopNGUIController.GuiActive)
			{
				ShopNGUIController.sharedShop.ChooseCategory(ShopNGUIController.sharedShop.CurrentCategory);
			}
			break;
		}
		}
		Storager.setInt(productId, 1, false);
		FinishCurrentStarterPack();
		if (!isRestore)
		{
			AnalyticsStuff.LogSales((!StoreKitEventListener.inAppsReadableNames.ContainsKey(productId)) ? productId : StoreKitEventListener.inAppsReadableNames[productId], "Starter Pack");
		}
		return true;
	}

	public void CheckSendEventChangeEnabled()
	{
		if (StarterPackController.OnStarterPackEnable != null)
		{
			StarterPackController.OnStarterPackEnable(isEventActive);
		}
	}

	public ItemPrice GetPriceDataForItemsPack()
	{
		StarterPackData currentPackData = GetCurrentPackData();
		if (currentPackData == null)
		{
			return null;
		}
		if (currentPackData.coinsCost <= 0 && currentPackData.gemsCost <= 0)
		{
			return null;
		}
		string currency = string.Empty;
		int price = 0;
		if (currentPackData.coinsCost > 0)
		{
			currency = "Coins";
			price = currentPackData.coinsCost;
		}
		else if (currentPackData.gemsCost > 0)
		{
			currency = "GemsCurrency";
			price = currentPackData.gemsCost;
		}
		return new ItemPrice(price, currency);
	}

	public bool IsPackSellForGameMoney()
	{
		StarterPackModel.TypeCost typeCostPack = GetTypeCostPack();
		return typeCostPack == StarterPackModel.TypeCost.Gems || typeCostPack == StarterPackModel.TypeCost.Money;
	}

	public void CheckBuyPackForGameMoney(StarterPackView view)
	{
		ItemPrice priceDataForItemsPack = GetPriceDataForItemsPack();
		if (priceDataForItemsPack != null)
		{
			ShopNGUIController.TryToBuy(view.gameObject, priceDataForItemsPack, delegate
			{
				string storageIdByPackOrder = GetStorageIdByPackOrder(_orderCurrentPack);
				TryTakePurchasesForCurrentPack(storageIdByPackOrder);
				view.HideWindow();
			});
		}
	}

	private StarterPackModel.TypeCost GetTypeCostPack()
	{
		StarterPackData currentPackData = GetCurrentPackData();
		if (currentPackData == null)
		{
			return StarterPackModel.TypeCost.None;
		}
		if (currentPackData.coinsCost > 0)
		{
			return StarterPackModel.TypeCost.Money;
		}
		if (currentPackData.gemsCost > 0)
		{
			return StarterPackModel.TypeCost.Gems;
		}
		return StarterPackModel.TypeCost.InApp;
	}

	public void CheckBuyRealMoney()
	{
		if (_orderCurrentPack >= StoreKitEventListener.starterPackIds.Length)
		{
			Debug.Log("Not purchase data for starter pack number: " + _orderCurrentPack);
			return;
		}
		ButtonClickSound.Instance.PlayClick();
		StoreKitEventListener.purchaseInProcess = true;
		string text = StoreKitEventListener.starterPackIds[_orderCurrentPack];
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			SkuInput skuInput = new SkuInput();
			skuInput.Sku = text;
			SkuInput skuInput2 = skuInput;
			Debug.Log("Amazon Purchase (StarterPackController.CheckBuyMoney): " + skuInput2.ToJson());
			AmazonIapV2Impl.Instance.Purchase(skuInput2);
		}
		else
		{
			AnalyticsFacade.SendCustomEventToAppsFlyer("af_initiated_checkout", new Dictionary<string, string> { { "af_content_id", text } });
			GoogleIAB.purchaseProduct(text);
		}
		MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
	}

	private int GetOrderPackByProductId(string productId)
	{
		if (!StoreKitEventListener.starterPackIds.Contains(productId))
		{
			return -1;
		}
		return Array.IndexOf(StoreKitEventListener.starterPackIds, productId);
	}

	private string GetStorageIdByPackOrder(int packOrder)
	{
		if (packOrder < 0 || packOrder >= StoreKitEventListener.starterPackIds.Length)
		{
			return string.Empty;
		}
		return StoreKitEventListener.starterPackIds[packOrder];
	}

	private IEnumerator TryRestoreStarterPackByProductId(string productId)
	{
		if (!(Application.loadedLevelName == "Loading") && StoreKitEventListener.starterPackIds.Contains(productId) && !IsStarterPackBuy(productId))
		{
			if (_starterPacksData.Count == 0)
			{
				yield return StartCoroutine(DownloadDataAboutEvent());
			}
			int packOrder = GetOrderPackByProductId(productId);
			if (!IsInvalidPack(packOrder) && GetPackType(packOrder) == StarterPackModel.TypePack.Items && TryTakePurchases(productId, packOrder, true))
			{
				FyberFacade.Instance.SetUserPaying("1");
				StoreKitEventListener.SetLastPaymentTime();
			}
		}
	}

	public void TryRestoreStarterPack(string productId)
	{
		StartCoroutine(TryRestoreStarterPackByProductId(productId));
	}

	public string GetTimeToEndEvent()
	{
		if (!isEventActive)
		{
			return string.Empty;
		}
		_timeToEndEvent = StarterPackModel.MaxLiveTimeEvent - _timeLiveEvent;
		return string.Format("{0:00}:{1:00}:{2:00}", _timeToEndEvent.Hours, _timeToEndEvent.Minutes, _timeToEndEvent.Seconds);
	}

	public string GetPriceLabelForCurrentPack()
	{
		if (_orderCurrentPack >= StoreKitEventListener.starterPackIds.Length)
		{
			return string.Empty;
		}
		if (Application.isEditor)
		{
			return string.Format("{0}$", VirtualCurrencyHelper.starterPackFakePrice[_orderCurrentPack]);
		}
		string productId = StoreKitEventListener.starterPackIds[_orderCurrentPack];
		IMarketProduct marketProduct = _storeKitEventListener.Products.FirstOrDefault((IMarketProduct p) => p.Id == productId);
		if (marketProduct != null)
		{
			return marketProduct.Price;
		}
		Debug.LogWarning("marketProduct == null,    id: " + productId);
		return string.Empty;
	}

	public string GetCurrentPackName()
	{
		if (_orderCurrentPack == -1)
		{
			return string.Empty;
		}
		if (_orderCurrentPack >= StarterPackModel.packNameLocalizeKey.Length)
		{
			return string.Empty;
		}
		string term = StarterPackModel.packNameLocalizeKey[_orderCurrentPack];
		return LocalizationStore.Get(term);
	}

	public Texture2D GetCurrentPackImage()
	{
		StarterPackModel.TypePack currentPackType = GetCurrentPackType();
		string text = string.Empty;
		switch (currentPackType)
		{
		case StarterPackModel.TypePack.Coins:
			text = "Textures/Bank/Coins_Shop_5";
			break;
		case StarterPackModel.TypePack.Gems:
			text = "Textures/Bank/Coins_Shop_Gem_5";
			break;
		case StarterPackModel.TypePack.Items:
			text = "Textures/Bank/StarterPack_Weapon";
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return Resources.Load<Texture2D>(text);
	}

	public void UpdateCountShownWindowByShowCondition()
	{
		if (PlayerPrefs.GetInt("CountShownStarterPack", 1) != 0)
		{
			PlayerPrefs.SetString("LastTimeShowStarterPack", DateTime.UtcNow.ToString("s"));
			int @int = PlayerPrefs.GetInt("CountShownStarterPack", 1);
			PlayerPrefs.SetInt("CountShownStarterPack", @int - 1);
			PlayerPrefs.Save();
		}
	}

	public bool IsNeedShowEventWindow()
	{
		int @int = PlayerPrefs.GetInt("CountShownStarterPack", 1);
		return isEventActive && @int > 0 && SceneLoader.ActiveSceneName.Equals(Defs.MainMenuScene);
	}

	public void UpdateCountShownWindowByTimeCondition()
	{
		string @string = PlayerPrefs.GetString("LastTimeShowStarterPack", string.Empty);
		if (!string.IsNullOrEmpty(@string))
		{
			DateTime result = default(DateTime);
			if (DateTime.TryParse(@string, out result) && DateTime.UtcNow - result >= StarterPackModel.TimeOutShownWindow)
			{
				PlayerPrefs.SetInt("CountShownStarterPack", 1);
			}
		}
	}

	public string GetSavingMoneyByCarrentPack()
	{
		int orderCurrentPack = GetOrderCurrentPack();
		if (orderCurrentPack == -1)
		{
			return string.Empty;
		}
		if (orderCurrentPack >= StarterPackModel.savingMoneyForBuyPack.Length)
		{
			return string.Empty;
		}
		if (IsPackSellForGameMoney())
		{
			return string.Empty;
		}
		return string.Format("{0} {1}$", LocalizationStore.Get("Key_1047"), StarterPackModel.savingMoneyForBuyPack[orderCurrentPack]);
	}

	public void AddBuyingStarterPack(List<string> buyingList, string starterPackId)
	{
		if (!buyingList.Contains(starterPackId))
		{
			buyingList.Add(starterPackId);
		}
	}

	public void RestoreBuyingStarterPack(List<string> buyingList)
	{
		for (int i = 0; i < buyingList.Count; i++)
		{
			string productId = buyingList[i];
			TryRestoreStarterPack(productId);
		}
	}

	private bool IsStarterPackBuying(List<string> buyingList, int orderPack)
	{
		string storageIdByPackOrder = GetStorageIdByPackOrder(orderPack);
		if (string.IsNullOrEmpty(storageIdByPackOrder))
		{
			return false;
		}
		return buyingList.Contains(storageIdByPackOrder);
	}

	public void AddBuyAndroidStarterPack(string starterPackId)
	{
		AddBuyingStarterPack(BuyAnroidStarterPack, starterPackId);
	}

	public void RestoreStarterPackForAmazon()
	{
		RestoreBuyingStarterPack(BuyAnroidStarterPack);
	}

	private bool IsStarterPackBuyOnAndroid(int orderPack)
	{
		return IsStarterPackBuying(BuyAnroidStarterPack, orderPack);
	}

	public void ClearAllGooglePurchases()
	{
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite && BuyAnroidStarterPack.Count != 0)
		{
			GoogleIAB.consumeProducts(BuyAnroidStarterPack.ToArray());
		}
	}
}

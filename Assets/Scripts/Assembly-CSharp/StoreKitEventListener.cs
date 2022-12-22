using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using com.amazon.device.iap.cpt;
using Prime31;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class StoreKitEventListener : MonoBehaviour
{
	private enum ContentType
	{
		Unknown,
		Coins,
		Gems,
		StarterPack
	}

	internal sealed class StoreKitEventListenerState
	{
		public string Mode { get; set; }

		public string PurchaseKey { get; set; }

		public IDictionary<string, string> Parameters { get; private set; }

		public StoreKitEventListenerState()
		{
			Mode = string.Empty;
			PurchaseKey = string.Empty;
			Parameters = new Dictionary<string, string>();
		}
	}

	public const string coin1 = "coin1";

	public const string coin2 = "coin2";

	public const string coin3 = "coin3.";

	public const string coin4 = "coin4";

	public const string coin5 = "coin5";

	public const string coin7 = "coin7";

	public const string coin8 = "coin8";

	private const string AmazonFulfilledReceiptsKey = "Amazon.FulfilledReceipts";

	private const string GooglePlayConsumedOrderIdsKey = "Android.GooglePlayOrderIdsKey";

	public const string bigAmmoPackID = "bigammopack";

	public const string crystalswordID = "crystalsword";

	public const string fullHealthID = "Fullhealth";

	public const string minerWeaponID = "MinerWeapon";

	[NonSerialized]
	internal readonly ICollection<IMarketProduct> _products = new List<IMarketProduct>();

	[NonSerialized]
	public readonly ICollection<GoogleSkuInfo> _skinProducts = new GoogleSkuInfo[0];

	[NonSerialized]
	public static bool billingSupported;

	[NonSerialized]
	public static readonly string[] coinIds;

	private static string[] _productIds;

	private readonly HashSet<GooglePurchase> _purchasesToConsume = new HashSet<GooglePurchase>();

	private readonly HashSet<GooglePurchase> _cheatedPurchasesToConsume = new HashSet<GooglePurchase>();

	private readonly TaskCompletionSource<AmazonUserData> _amazonUserPromise = new TaskCompletionSource<AmazonUserData>();

	private IDisposable _purchaseFailedSubscription = new ActionDisposable(null);

	private static List<string> listOfIdsForWhichX3WaitingCoroutinesRun;

	private readonly Lazy<SHA1Managed> _sha1 = new Lazy<SHA1Managed>(() => new SHA1Managed());

	private readonly Lazy<RSACryptoServiceProvider> _rsa = new Lazy<RSACryptoServiceProvider>(InitializeRsa);

	public static StoreKitEventListener Instance;

	private static string gem1;

	private static string gem2;

	private static string gem3;

	private static string gem4;

	private static string gem5;

	private static string gem6;

	private static string gem7;

	private static string starterPack2;

	private static string starterPack4;

	private static string starterPack6;

	private static string starterPack3;

	private static string starterPack5;

	private static string starterPack7;

	private static string starterPack8;

	public static readonly int[] realValue;

	public static readonly string[] gemsIds;

	public static readonly string[] starterPackIds;

	public static Dictionary<string, string> inAppsReadableNames;

	public static string elixirSettName;

	public static bool purchaseInProcess;

	public static bool restoreInProcess;

	public static string elixirID;

	public static string fullVersion;

	public static string armor;

	public static string armor2;

	public static string armor3;

	public static readonly string[] idsForSingle;

	public static readonly string[] idsForMulti;

	public static readonly string[] idsForFull;

	public static readonly string[][] categoriesSingle;

	public static readonly string[][] categoriesMulti;

	public GameObject messagePrefab;

	public static string[] categoryNames;

	public AudioClip onEarnCoinsSound;

	public AudioClip onEarnGemsSound;

	[NonSerialized]
	public static List<string> buyStarterPack;

	private static readonly StoreKitEventListenerState _state;

	internal ICollection<IMarketProduct> Products
	{
		get
		{
			return _products;
		}
	}

	public Task<AmazonUserData> AmazonUser
	{
		get
		{
			return _amazonUserPromise.Task;
		}
	}

	private static string starterPack1
	{
		get
		{
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
				return "starterpack1andr";
			}
			return "starterpack1";
		}
	}

	internal static StoreKitEventListenerState State
	{
		get
		{
			return _state;
		}
	}

	static StoreKitEventListener()
	{
		Instance = null;
		gem1 = "gem1";
		gem2 = "gem2";
		gem3 = "gem3";
		gem4 = "gem4";
		gem5 = "gem5";
		gem6 = "gem6";
		gem7 = "gem7";
		starterPack2 = "starterpack2";
		starterPack4 = "starterpack4";
		starterPack6 = "starterpack6";
		starterPack3 = "starterpack3";
		starterPack5 = "starterpack5";
		starterPack7 = "starterpack7";
		starterPack8 = "starterpack8";
		realValue = new int[7] { 1, 3, 5, 10, 20, 50, 100 };
		gemsIds = new string[7] { gem1, gem2, gem3, gem4, gem5, gem6, gem7 };
		starterPackIds = new string[8] { starterPack1, starterPack2, starterPack3, starterPack4, starterPack5, starterPack6, starterPack7, starterPack8 };
		inAppsReadableNames = new Dictionary<string, string>
		{
			{ "coin1", "Small Stack of Coins" },
			{ "coin7", "Medium Stack of Coins" },
			{ "coin2", "Big Stack of Coins" },
			{ "coin3.", "Huge Stack of Coins" },
			{ "coin4", "Chest with Coins" },
			{ "coin5", "Golden Chest with Coins" },
			{ "coin8", "Holy Grail" },
			{ gem1, "Few Gems" },
			{ gem2, "Handful of Gems" },
			{ gem3, "Pile of Gems" },
			{ gem4, "Chest with Gems" },
			{ gem5, "Treasure with Gems" },
			{ gem6, "Expensive Relic" },
			{ gem7, "Safe with Gems" },
			{ starterPack1, "Newbie Set" },
			{ "starterpack2", "Golden Coins Extra Pack" },
			{ starterPack3, "Trooper Set" },
			{ "starterpack4", "Gems Extra Pack" },
			{ starterPack5, "Veteran Set" },
			{ "starterpack6", "Mega Gems Pack" },
			{ starterPack7, "Hero Set" },
			{ starterPack8, "Winner Set" }
		};
		elixirSettName = Defs.NumberOfElixirsSett;
		purchaseInProcess = false;
		restoreInProcess = false;
		elixirID = ((!GlobalGameController.isFullVersion) ? "elixirlite" : "elixir");
		fullVersion = "extendedversion";
		armor = "armor";
		armor2 = "armor2";
		armor3 = "armor3";
		categoryNames = new string[5] { "Armory", "Guns", "Melee", "Special", "Gear" };
		buyStarterPack = new List<string>();
		_state = new StoreKitEventListenerState();
		billingSupported = false;
		coinIds = new string[8] { "coin1", "coin7", "coin2", "coin3.", "coin4", "coin5", "coin8", "coin9" };
		_productIds = new string[5] { "bigammopack", "Fullhealth", "crystalsword", "MinerWeapon", elixirID };
		listOfIdsForWhichX3WaitingCoroutinesRun = new List<string>();
		idsForSingle = new string[11]
		{
			"bigammopack", "Fullhealth", "ironSword", "MinerWeapon", "steelAxe", "spas", elixirID, "glock", "chainsaw", "scythe",
			"shovel"
		};
		idsForMulti = new string[10]
		{
			idsForSingle[2],
			idsForSingle[3],
			"steelAxe",
			"woodenBow",
			"combatrifle",
			"spas",
			"goldeneagle",
			idsForSingle[7],
			idsForSingle[8],
			"famas"
		};
		idsForFull = new string[1] { fullVersion };
		categoriesMulti = new string[2][]
		{
			new string[5]
			{
				idsForSingle[0],
				idsForSingle[1],
				armor,
				armor2,
				armor3
			},
			PotionsController.potions
		};
		categoriesSingle = categoriesMulti;
	}

	public static decimal GetPriceFromPriceAmountMicros(long priceAmountMicros)
	{
		decimal d = priceAmountMicros;
		decimal d2 = 1000000m;
		return decimal.Divide(d, d2);
	}

	private void Start()
	{
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			if (Application.isEditor && !_products.Any())
			{
				InitializeTestProductsAmazon();
				return;
			}
			List<string> skus = coinIds.Concat(gemsIds).ToList();
			SkusInput skusInput = new SkusInput();
			skusInput.Skus = skus;
			SkusInput skusInput2 = skusInput;
			Debug.Log("Amazon GetProductData (StoreKitEventListener.Start): " + skusInput2.ToJson());
			AmazonIapV2Impl.Instance.GetProductData(skusInput2);
		}
		else if (Application.isEditor)
		{
			InitializeTestProductsGoogle();
		}
		else
		{
			string publicKey = ((Defs.AndroidEdition != Defs.RuntimeAndroidEdition.GoogleLite) ? string.Empty : "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoTzMTaqsFhaywvCFKawFwL5KM+djLJfOCT/rbGQRfHmHYmOY2sBMgDWsA/67Szx6EVTZPVlFzHMgkAq1TwdL/A5aYGpGzaCX7o96cyp8R6wSF+xCuj++LAkTaDnLW0veI2bke3EVHu3At9xgM46e+VDucRUqQLvf6SQRb15nuflY5i08xKnewgX7I4U2H0RvAZDyoip+qZPmI4ZvaufAfc0jwZbw7XGiV41zibY3LU0N57mYKk51Wx+tOaJ7Tkc9Rl1qVCTjb+bwXshTqhVXVP6r4kabLWw/8OJUh0Sm69lbps6amP7vPy571XjscCTMLfXQan1959rHbNgkb2mLLQIDAQAB");
			GoogleIAB.init(publicKey);
			GoogleIAB.setAutoVerifySignatures(false);
			if (Defs.IsDeveloperBuild)
			{
				GoogleIAB.enableLogging(true);
			}
		}
	}

	private void OnEnable()
	{
		_purchaseFailedSubscription.Dispose();
		Action<string, int> googlePurchaseFailedHandler = delegate(string error, int response)
		{
			purchaseInProcess = false;
			Debug.LogWarning(string.Format("googlePurchaseFailedHandler({0}): {1}", response, error));
		};
		_purchaseFailedSubscription = new ActionDisposable(delegate
		{
			GoogleIABManager.purchaseFailedEvent -= googlePurchaseFailedHandler;
		});
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AmazonIapV2Impl.Instance.AddGetUserDataResponseListener(HandleGetUserIdResponseEvent);
			AmazonIapV2Impl.Instance.AddGetProductDataResponseListener(HandleItemDataRequestFinishedEvent);
			AmazonIapV2Impl.Instance.AddPurchaseResponseListener(HandlePurchaseSuccessfulEventAmazon);
			AmazonIapV2Impl.Instance.AddGetPurchaseUpdatesResponseListener(HandlePurchaseUpdatesRequestSuccessfulEvent);
			HandleAmazonSdkAvailableEvent(false);
			Debug.Log("Amazon GetUserData (StoreKitEventListener.OnEnable)");
			AmazonIapV2Impl.Instance.GetUserData();
		}
		else
		{
			GoogleIABManager.billingSupportedEvent += billingSupportedEvent;
			GoogleIABManager.billingNotSupportedEvent += billingNotSupportedEvent;
			GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
			GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
			GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
			GoogleIABManager.purchaseSucceededEvent += HandleGooglePurchaseSucceeded;
			GoogleIABManager.purchaseFailedEvent += googlePurchaseFailedHandler;
			GoogleIABManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
			GoogleIABManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
		}
	}

	private void OnDisable()
	{
		_purchaseFailedSubscription.Dispose();
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AmazonIapV2Impl.Instance.RemoveGetUserDataResponseListener(HandleGetUserIdResponseEvent);
			AmazonIapV2Impl.Instance.RemoveGetProductDataResponseListener(HandleItemDataRequestFinishedEvent);
			AmazonIapV2Impl.Instance.RemovePurchaseResponseListener(HandlePurchaseSuccessfulEventAmazon);
			AmazonIapV2Impl.Instance.RemoveGetPurchaseUpdatesResponseListener(HandlePurchaseUpdatesRequestSuccessfulEvent);
			return;
		}
		GoogleIABManager.billingSupportedEvent -= billingSupportedEvent;
		GoogleIABManager.billingNotSupportedEvent -= billingNotSupportedEvent;
		GoogleIABManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent -= purchaseCompleteAwaitingVerificationEvent;
		GoogleIABManager.purchaseSucceededEvent -= HandleGooglePurchaseSucceeded;
		GoogleIABManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
		GoogleIABManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
	}

	private void billingSupportedEvent()
	{
		billingSupported = true;
		Debug.Log("billingSupportedEvent");
		RefreshProducts();
	}

	public static void RefreshProducts()
	{
		if (billingSupported || Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			IEnumerable<string> source = _productIds.Concat(coinIds).Concat(gemsIds).Concat(starterPackIds);
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				SkusInput skusInput = new SkusInput();
				skusInput.Skus = source.ToList();
				SkusInput skusInput2 = skusInput;
				Debug.Log("Amazon GetProductData (RefreshProducts): " + skusInput2.ToJson());
				AmazonIapV2Impl.Instance.GetProductData(skusInput2);
			}
			else
			{
				GoogleIAB.queryInventory(source.ToArray());
			}
		}
	}

	private void billingNotSupportedEvent(string error)
	{
		billingSupported = false;
		Debug.LogWarning("billingNotSupportedEvent: " + error);
	}

	private void HandleAmazonSdkAvailableEvent(bool isSandboxMode)
	{
		Debug.Log("Amazon SDK available in sandbox mode: " + isSandboxMode);
		billingSupported = true;
		RefreshProducts();
	}

	private void HandleGetUserIdResponseEvent(GetUserDataResponse response)
	{
		string message = "Amazon GetUserDataResponse: " + response.Status;
		if (!"SUCCESSFUL".Equals(response.Status, StringComparison.OrdinalIgnoreCase))
		{
			Debug.LogWarning(message);
			_amazonUserPromise.TrySetException(new InvalidOperationException(message));
		}
		else
		{
			Debug.Log(message);
			_amazonUserPromise.TrySetResult(response.AmazonUserData);
		}
	}

	private void AndroidAddCurrencyAndConsume(GooglePurchase purchase)
	{
		string text = null;
		try
		{
			text = InappBonuessController.Instance.GiveBonusForInapp(purchase.productId);
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in giving inapp bonus in HandleGooglePurchaseSucceeded: {0}", ex);
		}
		TryAddVirtualCrrency(purchase.productId, text == null);
		Debug.Log("StoreKitEventListener.AddCurrencyAndConsumeNextGooglePlayPurchase(): Consuming Goole purchase " + purchase.ToString());
		GooglePlayConsumeAndSave(purchase);
		if (IsSinglePurchase(purchase))
		{
			SendFirstTimePayment(purchase);
		}
		LogRealPayment(purchase, text);
	}

	private static bool IsSinglePurchase(GooglePurchase purchase)
	{
		if (!Storager.hasKey("Android.GooglePlayOrderIdsKey"))
		{
			return false;
		}
		string @string = Storager.getString("Android.GooglePlayOrderIdsKey", false);
		if (string.IsNullOrEmpty(@string))
		{
			return false;
		}
		List<object> list = Rilisoft.MiniJson.Json.Deserialize(@string) as List<object>;
		if (list == null)
		{
			return false;
		}
		if (list.Count != 1)
		{
			return false;
		}
		string text = list.OfType<string>().FirstOrDefault(purchase.productId.Equals);
		if (text == null)
		{
			return false;
		}
		return true;
	}

	private void AddCurrencyAndConsumeNextGooglePlayPurchase()
	{
		try
		{
			GooglePurchase googlePurchase = null;
			if (_cheatedPurchasesToConsume.Count > 0)
			{
				googlePurchase = _cheatedPurchasesToConsume.FirstOrDefault((GooglePurchase p) => IsVirtualCurrency(p.productId));
				if (googlePurchase != null)
				{
					Debug.Log("StoreKitEventListener.AddCurrencyAndConsumeNextGooglePlayPurchase(): Consuming Goole purchase " + googlePurchase.ToString());
					GooglePlayConsumeAndSave(googlePurchase);
				}
				return;
			}
			googlePurchase = _purchasesToConsume.FirstOrDefault((GooglePurchase p) => IsVirtualCurrency(p.productId));
			if (googlePurchase == null)
			{
				return;
			}
			string text = string.Empty;
			if (Storager.hasKey("Android.GooglePlayOrderIdsKey"))
			{
				text = Storager.getString("Android.GooglePlayOrderIdsKey", false);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "[]";
			}
			List<object> source = (Rilisoft.MiniJson.Json.Deserialize(text) as List<object>) ?? new List<object>();
			HashSet<string> hashSet = new HashSet<string>(source.OfType<string>());
			if (!hashSet.Contains(googlePurchase.orderId))
			{
				if (!listOfIdsForWhichX3WaitingCoroutinesRun.Contains(googlePurchase.orderId))
				{
					if (CoroutineRunner.Instance != null)
					{
						CoroutineRunner.Instance.StartCoroutine(WaitForX3AndGiveCurrency(googlePurchase, null));
						return;
					}
					Debug.LogError("AddCurrencyAndConsumeNextGooglePlayPurchase CoroutineRunner.Instance == null ");
					AndroidAddCurrencyAndConsume(googlePurchase);
				}
			}
			else
			{
				Debug.Log("StoreKitEventListener.AddCurrencyAndConsumeNextGooglePlayPurchase(): Consuming Goole purchase " + googlePurchase.ToString());
				GooglePlayConsumeAndSave(googlePurchase);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Concat("AddCurrencyAndConsumeNextGooglePlayPurchase exception: ", ex, "\nstacktrace:\n", Environment.StackTrace));
		}
	}

	private void queryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
	{
		if (purchases == null)
		{
			purchases = new List<GooglePurchase>();
		}
		if (skus == null)
		{
			skus = new List<GoogleSkuInfo>();
		}
		_products.Clear();
		_purchasesToConsume.Clear();
		_cheatedPurchasesToConsume.Clear();
		try
		{
			if (skus.Any((GoogleSkuInfo s) => s.productId == "skinsmaker"))
			{
				return;
			}
			string[] productIds = skus.Select((GoogleSkuInfo sku) => sku.productId).ToArray();
			string arg = string.Join(", ", productIds);
			string[] value = purchases.Select((GooglePurchase p) => string.Format("<{0}, {1}>", p.productId, p.purchaseState)).ToArray();
			string arg2 = string.Join(", ", value);
			string message = string.Format("Google billing. Query inventory succeeded, purchases: [{0}], skus: [{1}]", arg2, arg);
			Debug.Log(message);
			IEnumerable<GoogleMarketProduct> enumerable = skus.Where((GoogleSkuInfo s) => productIds.Contains(s.productId)).Select(MarketProductFactory.CreateGoogleMarketProduct);
			foreach (GoogleMarketProduct item in enumerable)
			{
				if (item.Price.Contains("$0.0"))
				{
					Debug.LogWarningFormat("Unexpected price '{0}': '{1}' ('{2}')", item.Price, item.Id, item.Title);
					coinsShop.HasTamperedProducts = true;
				}
				if (!_products.Contains(item))
				{
					_products.Add(item);
				}
			}
			foreach (GooglePurchase purchase in purchases)
			{
				if (purchase.productId == "MinerWeapon" || purchase.productId == "MinerWeapon".ToLower() || purchase.productId == "crystalsword")
				{
					continue;
				}
				if (starterPackIds.Contains(purchase.productId))
				{
					try
					{
						StarterPackController.Get.AddBuyAndroidStarterPack(purchase.productId);
						StarterPackController.Get.TryRestoreStarterPack(purchase.productId);
					}
					catch (Exception ex)
					{
						Debug.LogFormat("Exception in queryInventorySucceededEvent starter packs: {0}", ex);
					}
				}
				else if (VerifyPurchase(purchase.originalJson, purchase.signature))
				{
					_purchasesToConsume.Add(purchase);
				}
				else
				{
					_cheatedPurchasesToConsume.Add(purchase);
				}
			}
			AddCurrencyAndConsumeNextGooglePlayPurchase();
		}
		finally
		{
			purchaseInProcess = false;
			restoreInProcess = false;
		}
	}

	private void queryInventoryFailedEvent(string error)
	{
		Debug.LogWarning("Google: queryInventoryFailedEvent: " + error);
		StartCoroutine(WaitAndQueryInventory());
	}

	private IEnumerator WaitAndQueryInventory()
	{
		Debug.LogWarning(string.Format("Waiting {0}s before requering inventory...", 10f));
		yield return new WaitForSeconds(10f);
		Debug.LogWarning(string.Format("Trying to repeat query inventory..."));
		string[] products = _productIds.Concat(coinIds).Concat(gemsIds).Concat(starterPackIds)
			.ToArray();
		GoogleIAB.queryInventory(products);
	}

	private void HandleItemDataRequestFinishedEvent(GetProductDataResponse response)
	{
		string message = "Amazon: GetProductDataResponse: " + response.Status;
		if (!"SUCCESSFUL".Equals(response.Status, StringComparison.OrdinalIgnoreCase))
		{
			Debug.LogWarning(message);
			return;
		}
		Debug.Log(message);
		_products.Clear();
		try
		{
			List<string> obj = response.ProductDataMap.Keys.ToList();
			string arg = Rilisoft.MiniJson.Json.Serialize(obj);
			string arg2 = Rilisoft.MiniJson.Json.Serialize(response.UnavailableSkus);
			string message2 = string.Format("Item data request finished;    Unavailable skus: {0}, Available skus: {1}", arg2, arg);
			Debug.Log(message2);
			IEnumerable<ProductData> enumerable = response.ProductDataMap.Values.Where((ProductData item) => coinIds.Contains(item.Sku) || gemsIds.Contains(item.Sku));
			IEnumerable<AmazonMarketProduct> enumerable2 = response.ProductDataMap.Values.Select(MarketProductFactory.CreateAmazonMarketProduct);
			foreach (AmazonMarketProduct item in enumerable2)
			{
				if (!_products.Contains(item))
				{
					_products.Add(item);
				}
			}
		}
		finally
		{
			purchaseInProcess = false;
			restoreInProcess = false;
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("[Rilisoft] Amazon: calling GetPurchaseUpdates()");
			}
			AmazonIapV2Impl.Instance.GetPurchaseUpdates(new ResetInput
			{
				Reset = true
			});
		}
	}

	private void HandleItemDataRequestFailedEvent()
	{
		Debug.LogWarning("Amamzon: Item data request failed.");
	}

	private void purchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature)
	{
		Debug.Log("purchaseCompleteAwaitingVerificationEvent. purchaseData: " + purchaseData + ", signature: " + signature);
	}

	private static bool IsVirtualCurrency(string productId)
	{
		if (productId == null)
		{
			return false;
		}
		int num = Array.IndexOf(coinIds, productId);
		int num2 = Array.IndexOf(gemsIds, productId);
		return num >= coinIds.GetLowerBound(0) || num2 >= gemsIds.GetLowerBound(0);
	}

	private bool TryAddVirtualCrrency(string productId, bool shouldAddCurrency)
	{
		if (string.IsNullOrEmpty(productId))
		{
			Debug.LogError("TryAddVirtualCrrency string.IsNullOrEmpty(productId)");
			return false;
		}
		int? num = null;
		int num2 = Array.IndexOf(coinIds, productId);
		int num3 = Array.IndexOf(gemsIds, productId);
		if (num2 >= coinIds.GetLowerBound(0))
		{
			num = Mathf.RoundToInt((float)VirtualCurrencyHelper.GetCoinInappsQuantity(num2) * PremiumAccountController.VirtualCurrencyMultiplier);
			if (shouldAddCurrency)
			{
				int val = Storager.getInt("Coins", false) + num.Value;
				Storager.setInt("Coins", val, false);
				AnalyticsFacade.CurrencyAccrual(num.Value, "Coins", AnalyticsConstants.AccrualType.Purchased);
			}
			coinsShop.TryToFireCurrenciesAddEvent("Coins");
			try
			{
				ChestBonusController.TryTakeChestBonus(false, num2);
			}
			catch (Exception ex)
			{
				Debug.LogError("TryAddVirtualCrrency ChestBonusController.TryTakeChestBonus exception: " + ex);
			}
		}
		else if (num3 >= gemsIds.GetLowerBound(0))
		{
			num = Mathf.RoundToInt((float)VirtualCurrencyHelper.GetGemsInappsQuantity(num3) * PremiumAccountController.VirtualCurrencyMultiplier);
			if (shouldAddCurrency)
			{
				int val2 = Storager.getInt("GemsCurrency", false) + num.Value;
				Storager.setInt("GemsCurrency", val2, false);
				AnalyticsFacade.CurrencyAccrual(num.Value, "GemsCurrency", AnalyticsConstants.AccrualType.Purchased);
			}
			coinsShop.TryToFireCurrenciesAddEvent("GemsCurrency");
			try
			{
				ChestBonusController.TryTakeChestBonus(true, num3);
			}
			catch (Exception ex2)
			{
				Debug.LogError("TryAddVirtualCrrency ChestBonusController.TryTakeChestBonus exception: " + ex2);
			}
		}
		if (num.HasValue)
		{
			try
			{
				LogVirtualCurrencyPurchased(productId, num.Value, num3 >= gemsIds.GetLowerBound(0));
				CheckIfFirstTimePayment();
				SetLastPaymentTime();
			}
			catch (Exception ex3)
			{
				Debug.LogWarningFormat("TryAddVirtualCrrency ANALYTICS, LogVirtualCurrencyPurchased({0}, {1}) threw exception: {2}", productId, num.Value, ex3);
			}
		}
		try
		{
			if (FriendsController.sharedController != null)
			{
				FriendsController.sharedController.SendOurData(false);
			}
		}
		catch (Exception ex4)
		{
			Debug.LogWarning("FriendsController.sharedController.SendOurData " + ex4);
		}
		return num.HasValue;
	}

	private bool TryAddStarterPackItem(string productId)
	{
		if (starterPackIds.Contains(productId))
		{
			bool flag = false;
			try
			{
				flag = StarterPackController.Get.TryTakePurchasesForCurrentPack(productId);
			}
			catch (Exception ex)
			{
				Debug.LogFormat("Exception in TryAddStarterPackItem starter packs: {0}", ex);
			}
			if (flag)
			{
				CheckIfFirstTimePayment();
				SetLastPaymentTime();
			}
			FriendsController.sharedController.SendOurData(false);
			return flag;
		}
		return false;
	}

	private void ConsumeProductIfCheating(GooglePurchase purchase)
	{
		if (Defs.IsDeveloperBuild)
		{
			Debug.LogError("Consuming cheated purchase: " + purchase.ToString());
		}
		GooglePlayConsumeAndSave(purchase);
	}

	private void LogRealPayment(GooglePurchase purchase, string keyOfInappAction)
	{
		if (purchase.purchaseState != 0)
		{
			return;
		}
		try
		{
			GoogleSkuInfo googleSkuInfo = Products.FirstOrDefault((IMarketProduct p) => (p.PlatformProduct as GoogleSkuInfo).productId == purchase.productId).PlatformProduct as GoogleSkuInfo;
			long priceAmountMicros = googleSkuInfo.priceAmountMicros;
			decimal priceFromPriceAmountMicros = GetPriceFromPriceAmountMicros(priceAmountMicros);
			string text = purchase.orderId ?? string.Empty;
			AnalyticsFacade.RealPayment(text, (float)priceFromPriceAmountMicros, AnalyticsStuff.ReadableNameForInApp(googleSkuInfo.productId), googleSkuInfo.priceCurrencyCode, false, keyOfInappAction);
			if (FriendsController.sharedController != null)
			{
				FriendsController.sharedController.SendAddPurchaseEvent(googleSkuInfo.productId, text, (float)priceFromPriceAmountMicros, googleSkuInfo.priceCurrencyCode, string.Empty);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in RealPayment: " + ex);
		}
		decimal value;
		if (VirtualCurrencyHelper.ReferencePricesInUsd.TryGetValue(purchase.productId, out value))
		{
			decimal num = IncrementAccumulatedPayments(value);
			if (num >= 100m)
			{
				AnalyticsStuff.TrySendOnceToFacebook("paid_100", null, null);
			}
			else if (num >= 50m)
			{
				AnalyticsStuff.TrySendOnceToFacebook("paid_50", null, null);
			}
			else if (num >= 25m)
			{
				AnalyticsStuff.TrySendOnceToFacebook("paid_25", null, null);
			}
		}
		else
		{
			Debug.LogErrorFormat("Cannot find price for product {0}", purchase.productId);
		}
	}

	private void SendFirstTimePayment(GooglePurchase purchase)
	{
		//Discarded unreachable code: IL_0040
		if (purchase.purchaseState != 0)
		{
			return;
		}
		try
		{
			Version version = new Version(Switcher.InitialAppVersion);
			if (version <= new Version(10, 3, 2, 891))
			{
				return;
			}
		}
		catch
		{
			return;
		}
		GoogleSkuInfo googleSkuInfo = Products.Select((IMarketProduct p) => p.PlatformProduct).OfType<GoogleSkuInfo>().FirstOrDefault(purchase.productId.Equals);
		if (googleSkuInfo == null)
		{
			Debug.LogErrorFormat("SendFirstTimePayment: sku == null, productId = {0}", purchase.productId);
		}
		else
		{
			decimal num = decimal.Divide(googleSkuInfo.priceAmountMicros, 1000000m);
			AnalyticsFacade.SendFirstTimeRealPayment(purchase.orderId, (float)num, AnalyticsStuff.ReadableNameForInApp(googleSkuInfo.productId), googleSkuInfo.priceCurrencyCode);
		}
	}

	private void HandleGooglePurchaseSucceeded(GooglePurchase purchase)
	{
		Debug.Log("HandleGooglePurchaseSucceeded: " + purchase);
		if (coinsShop.IsWideLayoutAvailable)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogError("Cheating attempt.");
			}
			ConsumeProductIfCheating(purchase);
			return;
		}
		if (!coinsShop.CheckHostsTimestamp())
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogError("Hosts tampering attempt.");
			}
			ConsumeProductIfCheating(purchase);
			return;
		}
		if (!VerifyPurchase(purchase.originalJson, purchase.signature))
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogError("Purchase verification failed.");
			}
			ConsumeProductIfCheating(purchase);
			return;
		}
		ContentType contentType = ContentType.Unknown;
		try
		{
			string text = null;
			try
			{
				text = InappBonuessController.Instance.GiveBonusForInapp(purchase.productId);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in giving inapp bonus in HandleGooglePurchaseSucceeded: {0}", ex);
			}
			if (TryAddVirtualCrrency(purchase.productId, text == null))
			{
				if (Array.IndexOf(coinIds, purchase.productId) >= 0)
				{
					contentType = ContentType.Coins;
				}
				else if (Array.IndexOf(gemsIds, purchase.productId) >= 0)
				{
					contentType = ContentType.Gems;
				}
				Debug.Log("StoreKitEventListener.HandleGooglePurchaseSucceeded(): Consuming Goole product " + purchase.productId);
				GooglePlayConsumeAndSave(purchase);
			}
			else if (TryAddStarterPackItem(purchase.productId))
			{
				contentType = ContentType.StarterPack;
			}
			if (IsSinglePurchase(purchase))
			{
				SendFirstTimePayment(purchase);
			}
			LogRealPayment(purchase, text);
		}
		finally
		{
			purchaseInProcess = false;
			restoreInProcess = false;
			if (purchase.purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
			{
				decimal value;
				if (VirtualCurrencyHelper.ReferencePricesInUsd.TryGetValue(purchase.productId, out value))
				{
					decimal num = Math.Round(value, 0, MidpointRounding.AwayFromZero);
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("af_revenue", num.ToString("F2"));
					dictionary.Add("af_content_type", contentType.ToString());
					dictionary.Add("af_content_id", purchase.productId);
					dictionary.Add("af_currency", "USD");
					dictionary.Add("af_validated", "true");
					dictionary.Add("af_receipt_id", purchase.orderId);
					Dictionary<string, string> eventParams = dictionary;
					AnalyticsFacade.SendCustomEventToAppsFlyer("af_purchase_approximate", eventParams);
				}
				else
				{
					Debug.LogErrorFormat("Cannot find price for product {0}", purchase.productId);
				}
			}
		}
	}

	private bool VerifyPurchase(string purchaseJson, string base64Signature)
	{
		//Discarded unreachable code: IL_0036
		try
		{
			byte[] signature = Convert.FromBase64String(base64Signature);
			byte[] bytes = Encoding.UTF8.GetBytes(purchaseJson);
			return _rsa.Value.VerifyData(bytes, _sha1.Value, signature);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return false;
	}

	private void HandlePurchaseSuccessfulEventAmazon(PurchaseResponse response)
	{
		string message = "Amazon PurchaseResponse (StoreKitEventListener): " + response.Status;
		if (!"SUCCESSFUL".Equals(response.Status, StringComparison.OrdinalIgnoreCase))
		{
			Debug.LogWarning(message);
			purchaseInProcess = false;
			return;
		}
		Debug.Log(message);
		PurchaseReceipt purchaseReceipt = response.PurchaseReceipt;
		Debug.Log("Amazon PurchaseResponse.PurchaseReceipt: " + purchaseReceipt.ToJson());
		try
		{
			NotifyFulfillmentInput notifyFulfillmentInput = new NotifyFulfillmentInput();
			notifyFulfillmentInput.ReceiptId = purchaseReceipt.ReceiptId;
			notifyFulfillmentInput.FulfillmentResult = "FULFILLED";
			NotifyFulfillmentInput notifyFulfillmentInput2 = notifyFulfillmentInput;
			string text = null;
			try
			{
				text = InappBonuessController.Instance.GiveBonusForInapp(purchaseReceipt.Sku);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in giving inapp bonus in HandlePurchaseSuccessfulEventAmazon: {0}", ex);
			}
			int num = Array.IndexOf(coinIds, purchaseReceipt.Sku);
			if (num >= coinIds.GetLowerBound(0))
			{
				int num2 = Mathf.RoundToInt((float)VirtualCurrencyHelper.GetCoinInappsQuantity(num) * PremiumAccountController.VirtualCurrencyMultiplier);
				string message2 = string.Format("Process purchase {0}, VirtualCurrencyHelper.GetCoinInappsQuantity({1})", purchaseReceipt.Sku, num);
				Debug.Log(message2);
				if (text == null)
				{
					int val = Storager.getInt("Coins", false) + num2;
					Storager.setInt("Coins", val, false);
					AnalyticsFacade.CurrencyAccrual(num2, "Coins", AnalyticsConstants.AccrualType.Purchased);
				}
				Debug.Log("Amazon NotifyFulfillment (HandlePurchaseSuccessfulEvent): " + notifyFulfillmentInput2.ToJson());
				AmazonNotifyFulfillmentAndSave(notifyFulfillmentInput2);
				ChestBonusController.TryTakeChestBonus(false, num);
				coinsShop.TryToFireCurrenciesAddEvent("Coins");
				CheckIfFirstTimePayment();
				SetLastPaymentTime();
				LogVirtualCurrencyPurchased(purchaseReceipt.Sku, num2, false);
			}
			num = Array.IndexOf(gemsIds, purchaseReceipt.Sku);
			if (num >= gemsIds.GetLowerBound(0))
			{
				int num3 = Mathf.RoundToInt((float)VirtualCurrencyHelper.GetGemsInappsQuantity(num) * PremiumAccountController.VirtualCurrencyMultiplier);
				string message3 = string.Format("Process purchase {0}, VirtualCurrencyHelper.GetGemsInappsQuantity({1})", purchaseReceipt.Sku, num);
				Debug.Log(message3);
				if (text == null)
				{
					int val2 = Storager.getInt("GemsCurrency", false) + num3;
					Storager.setInt("GemsCurrency", val2, false);
					AnalyticsFacade.CurrencyAccrual(num3, "GemsCurrency", AnalyticsConstants.AccrualType.Purchased);
				}
				Debug.Log("Amazon NotifyFulfillment (HandlePurchaseSuccessfulEvent): " + notifyFulfillmentInput2.ToJson());
				AmazonNotifyFulfillmentAndSave(notifyFulfillmentInput2);
				ChestBonusController.TryTakeChestBonus(true, num);
				coinsShop.TryToFireCurrenciesAddEvent("GemsCurrency");
				CheckIfFirstTimePayment();
				SetLastPaymentTime();
				LogVirtualCurrencyPurchased(purchaseReceipt.Sku, num3, true);
			}
			if (TryAddStarterPackItem(purchaseReceipt.Sku))
			{
				string message4 = string.Format("Process purchase {0}. Starter pack.", purchaseReceipt.Sku, num);
				Debug.Log(message4);
			}
			FriendsController.sharedController.SendOurData(false);
		}
		finally
		{
			purchaseInProcess = false;
			restoreInProcess = false;
		}
	}

	private void consumePurchaseSucceededEvent(GooglePurchase purchase)
	{
		Debug.Log("consumePurchaseSucceededEvent: " + purchase);
		if (_cheatedPurchasesToConsume.RemoveWhere((GooglePurchase p) => p.productId == purchase.productId) == 0)
		{
			_purchasesToConsume.RemoveWhere((GooglePurchase p) => p.productId == purchase.productId);
		}
		AddCurrencyAndConsumeNextGooglePlayPurchase();
	}

	private void consumePurchaseFailedEvent(string error)
	{
		Debug.LogWarning("consumePurchaseFailedEvent: " + error);
	}

	private static NotifyFulfillmentInput FulfillmentInputForReceipt(PurchaseReceipt receipt)
	{
		NotifyFulfillmentInput notifyFulfillmentInput = new NotifyFulfillmentInput();
		notifyFulfillmentInput.ReceiptId = receipt.ReceiptId;
		notifyFulfillmentInput.FulfillmentResult = "FULFILLED";
		return notifyFulfillmentInput;
	}

	public IEnumerator MyWaitForSeconds(float tm)
	{
		float startTime = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (Time.realtimeSinceStartup - startTime < tm);
	}

	private IEnumerator WaitForX3AndGiveCurrency(GooglePurchase purchase, PurchaseReceipt receipt)
	{
		string idToList = ((purchase == null) ? receipt.ReceiptId : purchase.orderId);
		listOfIdsForWhichX3WaitingCoroutinesRun.Add(idToList);
		try
		{
			while (ShouldDelayCompletingTransactions())
			{
				if (CoroutineRunner.Instance != null)
				{
					yield return CoroutineRunner.Instance.StartCoroutine(MyWaitForSeconds(1f));
				}
				else
				{
					Debug.LogError("Amazon/Android WaitForX3AndGiveCurrency CoroutineRunner.Instance == null ");
				}
			}
		}
		finally
		{
			listOfIdsForWhichX3WaitingCoroutinesRun.Remove(idToList);
		}
		try
		{
			if (PromoActionsManager.sharedManager != null)
			{
				PromoActionsManager.sharedManager.ForceCheckEventX3Active();
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Amazon WaitForX3AndGiveCurrency PromoActionsManager.sharedManager.ForceCheckEventX3Active() exception: " + e);
		}
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			GiveCoinsOrGemsOnAmazon(receipt);
		}
		else
		{
			AndroidAddCurrencyAndConsume(purchase);
		}
	}

	private static void GiveCoinsOrGemsOnAmazon(PurchaseReceipt receipt)
	{
		try
		{
			Debug.Log("[Rilisoft] Amazon: restoring purchase: " + receipt.Sku);
			string text = null;
			try
			{
				text = InappBonuessController.Instance.GiveBonusForInapp(receipt.Sku);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in giving inapp bonus in HandlePurchaseSuccessfulEventAmazon: {0}", ex);
			}
			int num = Array.IndexOf(coinIds, receipt.Sku);
			if (num >= coinIds.GetLowerBound(0))
			{
				int num2 = Mathf.RoundToInt((float)VirtualCurrencyHelper.GetCoinInappsQuantity(num) * PremiumAccountController.VirtualCurrencyMultiplier);
				if (text == null)
				{
					int val = Storager.getInt("Coins", false) + num2;
					Storager.setInt("Coins", val, false);
					AnalyticsFacade.CurrencyAccrual(num2, "Coins", AnalyticsConstants.AccrualType.Purchased);
				}
				try
				{
					ChestBonusController.TryTakeChestBonus(false, num);
				}
				catch (Exception ex2)
				{
					Debug.LogError("[Rilisoft] Amazon: TryTakeChestBonus exception: " + ex2);
				}
				coinsShop.TryToFireCurrenciesAddEvent("Coins");
				CheckIfFirstTimePayment();
				SetLastPaymentTime();
				LogVirtualCurrencyPurchased(receipt.Sku, num2, false);
			}
			num = Array.IndexOf(gemsIds, receipt.Sku);
			if (num >= gemsIds.GetLowerBound(0))
			{
				int num3 = Mathf.RoundToInt((float)VirtualCurrencyHelper.GetGemsInappsQuantity(num) * PremiumAccountController.VirtualCurrencyMultiplier);
				string message = string.Format("Process purchase {0}, VirtualCurrencyHelper.GetGemsInappsQuantity({1})", receipt.Sku, num);
				Debug.Log(message);
				if (text == null)
				{
					int val2 = Storager.getInt("GemsCurrency", false) + num3;
					Storager.setInt("GemsCurrency", val2, false);
					AnalyticsFacade.CurrencyAccrual(num3, "GemsCurrency", AnalyticsConstants.AccrualType.Purchased);
				}
				try
				{
					ChestBonusController.TryTakeChestBonus(true, num);
				}
				catch (Exception ex3)
				{
					Debug.LogError("[Rilisoft] Amazon: TryTakeChestBonus exception: " + ex3);
				}
				coinsShop.TryToFireCurrenciesAddEvent("GemsCurrency");
				CheckIfFirstTimePayment();
				SetLastPaymentTime();
				LogVirtualCurrencyPurchased(receipt.Sku, num3, true);
			}
		}
		catch (Exception ex4)
		{
			Debug.LogError("Exception GiveCoinsOrGemsOnAmazon: " + ex4);
		}
		finally
		{
			NotifyFulfillmentInput notifyFulfillmentInput = FulfillmentInputForReceipt(receipt);
			Debug.Log("Amazon NotifyFulfillment (HandlePurchaseUpdatesRequestSuccessfulEvent): " + notifyFulfillmentInput.ToJson());
			AmazonNotifyFulfillmentAndSave(notifyFulfillmentInput);
		}
	}

	private void HandlePurchaseUpdatesRequestSuccessfulEvent(GetPurchaseUpdatesResponse response)
	{
		string message = "[Rilisoft] Amazon GetPurchaseUpdatesResponse: " + response.ToJson();
		if (!"SUCCESSFUL".Equals(response.Status, StringComparison.OrdinalIgnoreCase))
		{
			Debug.LogWarning(message);
			return;
		}
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log(message);
		}
		string text = string.Empty;
		if (Storager.hasKey("Amazon.FulfilledReceipts"))
		{
			text = Storager.getString("Amazon.FulfilledReceipts", false);
		}
		if (string.IsNullOrEmpty(text))
		{
			text = "[]";
		}
		List<object> source = (Rilisoft.MiniJson.Json.Deserialize(text) as List<object>) ?? new List<object>();
		HashSet<string> hashSet = new HashSet<string>(source.OfType<string>());
		List<PurchaseReceipt> receipts = response.Receipts;
		for (int i = 0; i != receipts.Count; i++)
		{
			PurchaseReceipt purchaseReceipt = receipts[i];
			string sku = purchaseReceipt.Sku;
			if (starterPackIds.Contains(sku))
			{
				try
				{
					StarterPackController.Get.AddBuyAndroidStarterPack(sku);
					StarterPackController.Get.TryRestoreStarterPack(sku);
				}
				catch (Exception ex)
				{
					Debug.LogFormat("Exception in HandlePurchaseUpdatesRequestSuccessfulEvent starter packs: {0}", ex);
				}
			}
			else
			{
				if (!coinIds.Contains(sku) && !gemsIds.Contains(sku))
				{
					continue;
				}
				try
				{
					if (!hashSet.Contains(purchaseReceipt.ReceiptId))
					{
						if (!listOfIdsForWhichX3WaitingCoroutinesRun.Contains(purchaseReceipt.ReceiptId))
						{
							if (CoroutineRunner.Instance != null)
							{
								CoroutineRunner.Instance.StartCoroutine(WaitForX3AndGiveCurrency(null, purchaseReceipt));
								continue;
							}
							Debug.LogError("Amazon NotifyFulfillment CoroutineRunner.Instance == null ");
							GiveCoinsOrGemsOnAmazon(purchaseReceipt);
						}
					}
					else
					{
						NotifyFulfillmentInput notifyFulfillmentInput = FulfillmentInputForReceipt(purchaseReceipt);
						Debug.Log("Amazon NotifyFulfillment (HandlePurchaseUpdatesRequestSuccessfulEvent): " + notifyFulfillmentInput.ToJson());
						AmazonNotifyFulfillmentAndSave(notifyFulfillmentInput);
					}
				}
				catch (Exception ex2)
				{
					Debug.LogError("Exception HandlePurchaseUpdatesRequestSuccessfulEvent: " + ex2);
				}
			}
		}
	}

	private static void AmazonNotifyFulfillmentAndSave(NotifyFulfillmentInput notifyFulfillmentInput)
	{
		string callee = ((!Defs.IsDeveloperBuild) ? string.Empty : string.Format("AmazonNotifyFulfillmentAndSave('{0}')", notifyFulfillmentInput.ToJson()));
		ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
		try
		{
			if (notifyFulfillmentInput == null)
			{
				throw new ArgumentNullException("notifyFulfillmentInput");
			}
			string text = string.Empty;
			if (Storager.hasKey("Amazon.FulfilledReceipts"))
			{
				text = Storager.getString("Amazon.FulfilledReceipts", false);
				Debug.LogFormat("Storager has {0}: {1}", "Amazon.FulfilledReceipts", text);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "[]";
				Debug.LogFormat("Storager doesn't have {0}: {1}", "Amazon.FulfilledReceipts", text);
			}
			List<object> source = (Rilisoft.MiniJson.Json.Deserialize(text) as List<object>) ?? new List<object>();
			AmazonIapV2Impl.Instance.NotifyFulfillment(notifyFulfillmentInput);
			HashSet<string> hashSet = new HashSet<string>(source.OfType<string>());
			hashSet.Add(notifyFulfillmentInput.ReceiptId);
			text = Rilisoft.MiniJson.Json.Serialize(hashSet.ToList());
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("[Rilisoft] Saving fulfillments: " + text);
			}
			Storager.setString("Amazon.FulfilledReceipts", text, false);
			PlayerPrefs.Save();
		}
		finally
		{
			scopeLogger.Dispose();
		}
	}

	private static void GooglePlayConsumeAndSave(GooglePurchase purchase)
	{
		try
		{
			if (purchase == null)
			{
				Debug.LogWarning("GooglePlayConsumeAndSave: purchase == null");
				return;
			}
			string text = string.Empty;
			if (Storager.hasKey("Android.GooglePlayOrderIdsKey"))
			{
				text = Storager.getString("Android.GooglePlayOrderIdsKey", false);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "[]";
			}
			List<object> source = (Rilisoft.MiniJson.Json.Deserialize(text) as List<object>) ?? new List<object>();
			GoogleIAB.consumeProduct(purchase.productId);
			HashSet<string> hashSet = new HashSet<string>(source.OfType<string>());
			hashSet.Add(purchase.orderId);
			text = Rilisoft.MiniJson.Json.Serialize(hashSet.ToList());
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("[Rilisoft] Saving consumed order ids: " + text);
			}
			Storager.setString("Android.GooglePlayOrderIdsKey", text, false);
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Concat("GooglePlayConsumeAndSave exception: ", ex, "\nstacktrace:\n", Environment.StackTrace));
		}
	}

	private void HandlePurchaseUpdatesRequestFailedEvent()
	{
		Debug.LogWarning("Amazon: Purchase updates request failed.");
	}

	private static RSACryptoServiceProvider InitializeRsa()
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString("<RSAKeyValue><Modulus>oTzMTaqsFhaywvCFKawFwL5KM+djLJfOCT/rbGQRfHmHYmOY2sBMgDWsA/67Szx6EVTZPVlFzHMgkAq1TwdL/A5aYGpGzaCX7o96cyp8R6wSF+xCuj++LAkTaDnLW0veI2bke3EVHu3At9xgM46e+VDucRUqQLvf6SQRb15nuflY5i08xKnewgX7I4U2H0RvAZDyoip+qZPmI4ZvaufAfc0jwZbw7XGiV41zibY3LU0N57mYKk51Wx+tOaJ7Tkc9Rl1qVCTjb+bwXshTqhVXVP6r4kabLWw/8OJUh0Sm69lbps6amP7vPy571XjscCTMLfXQan1959rHbNgkb2mLLQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
		return rSACryptoServiceProvider;
	}

	private void InitializeTestProductsAmazon()
	{
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "33 руб." },
			{ "sku", "coin1" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of coins" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "33 руб." },
			{ "sku", "coin2" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of coins" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "33 руб." },
			{ "sku", "coin3." },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of coins" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "33 руб." },
			{ "sku", "coin4" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of coins" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "33 руб." },
			{ "sku", "coin5" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of coins" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "33 руб." },
			{ "sku", "coin17" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of coins" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "33 руб." },
			{ "sku", "coin8" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of coins" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "33 руб." },
			{ "sku", "coin9" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of coins" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "99 руб." },
			{ "sku", "gem1" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of gems" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "99 руб." },
			{ "sku", "gem2" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of gems" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "99 руб." },
			{ "sku", "gem3" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of gems" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "99 руб." },
			{ "sku", "gem4" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of gems" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "99 руб." },
			{ "sku", "gem5" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of gems" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "99 руб." },
			{ "sku", "gem6" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of gems" }
		})));
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Amazon edition" },
			{ "productType", "Not defined" },
			{ "price", "99 руб." },
			{ "sku", "gem7" },
			{ "smallIconUrl", "http://example.com" },
			{ "title", "Small pack of gems" }
		})));
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("description", "Test starter pack product for editor in Amazon edition");
		dictionary.Add("productType", "Not defined");
		dictionary.Add("price", "33 руб.");
		dictionary.Add("sku", starterPack1);
		dictionary.Add("smallIconUrl", "http://example.com");
		dictionary.Add("title", "First starter pack(amazon)");
		Dictionary<string, object> jsonMap = dictionary;
		_products.Add(new AmazonMarketProduct(ProductData.CreateFromDictionary(jsonMap)));
	}

	private void InitializeTestProductsGoogle()
	{
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "99 руб." },
			{ "productId", "coin1" },
			{ "title", "Average pack of coins" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "99 руб." },
			{ "productId", "coin2" },
			{ "title", "Average pack of coins" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "99 руб." },
			{ "productId", "coin3." },
			{ "title", "Average pack of coins" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "99 руб." },
			{ "productId", "coin4" },
			{ "title", "Average pack of coins" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "99 руб." },
			{ "productId", "coin5" },
			{ "title", "Average pack of coins" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "99 руб." },
			{ "productId", "coin7" },
			{ "title", "Average pack of coins" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "99 руб." },
			{ "productId", "coin8" },
			{ "title", "Average pack of coins" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test coin product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "99 руб." },
			{ "productId", "coin9" },
			{ "title", "Average pack of coins" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "33 руб." },
			{ "productId", "gem1" },
			{ "title", "Average pack of gems" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "33 руб." },
			{ "productId", "gem2" },
			{ "title", "Average pack of gems" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "33 руб." },
			{ "productId", "gem3" },
			{ "title", "Average pack of gems" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "33 руб." },
			{ "productId", "gem4" },
			{ "title", "Average pack of gems" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "33 руб." },
			{ "productId", "gem5" },
			{ "title", "Average pack of gems" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "33 руб." },
			{ "productId", "gem6" },
			{ "title", "Average pack of gems" }
		})));
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(new Dictionary<string, object>
		{
			{ "description", "Test gem product for editor in Google edition" },
			{ "type", "Not defined" },
			{ "price", "33 руб." },
			{ "productId", "gem7" },
			{ "title", "Average pack of gems" }
		})));
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("description", "Test starter pack product for editor in Google edition");
		dictionary.Add("type", "Not defined");
		dictionary.Add("price", "33 руб.");
		dictionary.Add("productId", starterPack1);
		dictionary.Add("title", "First starter pack(android)");
		Dictionary<string, object> dict = dictionary;
		_products.Add(new GoogleMarketProduct(new GoogleSkuInfo(dict)));
	}

	public static bool ShouldDelayCompletingTransactions()
	{
		//Discarded unreachable code: IL_003d, IL_005a
		try
		{
			return Time.realtimeSinceStartup - PromoActionsManager.startupTime < 45f && (!PromoActionsManager.x3InfoDownloadaedOnceDuringCurrentRun || !ChestBonusController.chestBonusesObtainedOnceInCurrentRun || !coinsShop.IsStoreAvailable);
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in ShouldDelayCompletingTransactions: " + ex);
			return false;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	internal static bool IsPayingUser()
	{
		return Storager.getInt("PayingUser", true) > 0;
	}

	public void ProvideContent()
	{
	}

	private static IEnumerator WaitForFyberAndSetIsPaying()
	{
		while (FyberFacade.Instance == null)
		{
			yield return null;
		}
		FyberFacade.Instance.SetUserPaying("1");
	}

	internal static decimal IncrementAccumulatedPayments(decimal payment)
	{
		decimal result;
		if (!Storager.hasKey("Analytics.AccumulatedPayments") || !decimal.TryParse(Storager.getString("Analytics.AccumulatedPayments", false), out result))
		{
			result = 0m;
		}
		decimal result2 = result + payment;
		Storager.setString("Analytics.AccumulatedPayments", result2.ToString(CultureInfo.InvariantCulture), false);
		return result2;
	}

	internal static void CheckIfFirstTimePayment()
	{
		if (!Storager.hasKey("PayingUser") || Storager.getInt("PayingUser", true) != 1)
		{
			Storager.setInt("PayingUser", 1, true);
			if (CoroutineRunner.Instance != null)
			{
				CoroutineRunner.Instance.StartCoroutine(WaitForFyberAndSetIsPaying());
			}
			else
			{
				Debug.LogError("CheckIfFirstTimePayment CoroutineRunner.Instance == null");
			}
		}
	}

	public static int GetDollarsSpent()
	{
		return PlayerPrefs.GetInt("ALLCoins", 0) + PlayerPrefs.GetInt("ALLGems", 0);
	}

	internal static void SetLastPaymentTime()
	{
		string value = DateTime.UtcNow.ToString("s");
		PlayerPrefs.SetString("Last Payment Time", value);
		Storager.setInt("PayingUser", 1, true);
		PlayerPrefs.SetString("Last Payment Time (Advertisement)", value);
	}

	public static void LogVirtualCurrencyPurchased(string purchaseId, int virtualCurrencyCount, bool isGems)
	{
		try
		{
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.AnyDiscountForTryGuns)
			{
				AnalyticsStuff.LogWEaponsSpecialOffers_MoneySpended(purchaseId);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("LogVirtualCurrencyPurchased exception (Weapons Special Offers): " + ex);
		}
		try
		{
			if (GiftBannerWindow.instance != null && GiftBannerWindow.instance.IsShow)
			{
				AnalyticsStuff.LogDailyGiftPurchases(purchaseId);
			}
			if (BuySmileBannerController.openedFromPromoActions || (ShopNGUIController.sharedShop != null && ShopNGUIController.sharedShop.IsFromPromoActions))
			{
				AnalyticsStuff.LogSpecialOffersPanel((!isGems) ? "Buy Coins" : "Buy Gems", AnalyticsStuff.ReadableNameForInApp(purchaseId));
			}
			if (ShopNGUIController.sharedShop.GunThatWeUsedInPolygon != null)
			{
				AnalyticsFacade.SendCustomEvent("Polygon", new Dictionary<string, object> { 
				{
					"Money Spended",
					AnalyticsStuff.ReadableNameForInApp(purchaseId)
				} });
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError("LogVirtualCurrencyPurchased exception: " + ex2);
		}
		string deviceModel = SystemInfo.deviceModel;
		ShopNGUIController.AddBoughtCurrency((!isGems) ? "Coins" : "GemsCurrency", virtualCurrencyCount);
		string value = string.Format("{0} ({1})", purchaseId, virtualCurrencyCount);
		string value2 = PlayerPrefs.GetInt(Defs.SessionNumberKey, 1).ToString();
		string value3 = ((!(ExperienceController.sharedController != null)) ? "Unknown" : ExperienceController.sharedController.currentLevel.ToString());
		string eventName = (((!isGems) ? "Coins Purchased " : "Gems Purchased ") + State.Mode) ?? string.Empty;
		Dictionary<string, string> dictionary = new Dictionary<string, string>(State.Parameters);
		dictionary.Add(State.PurchaseKey, purchaseId);
		dictionary.Add("Rank", value3);
		dictionary.Add("Session number", value2);
		dictionary.Add("SKU", value);
		dictionary.Add("Device model", deviceModel);
		Dictionary<string, string> eventParams = dictionary;
		AnalyticsFacade.SendCustomEventToAppsFlyer(eventName, eventParams);
		int @int = PlayerPrefs.GetInt("CountPaying", 0);
		int num = Array.IndexOf(coinIds, purchaseId);
		bool flag = false;
		if (num == -1)
		{
			num = Array.IndexOf(gemsIds, purchaseId);
			if (num == -1)
			{
				num = Array.IndexOf(starterPackIds, purchaseId);
				flag = true;
			}
		}
		if (((BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon) || BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64) && FriendsController.sharedController != null)
		{
			FriendsController.sharedController.SendAddPurchaseEvent(purchaseId, string.Empty, 0f, string.Empty, string.Empty);
		}
		if (num == -1)
		{
			string message = string.Format("Could not find “{0}” value in coinIds array.", purchaseId);
			Debug.Log(message);
			return;
		}
		if (ABTestController.useBuffSystem)
		{
			BuffSystem.instance.OnCurrencyBuyed(isGems, num);
		}
		int num2 = 0;
		if (isGems)
		{
			num2 = PlayerPrefs.GetInt("ALLGems", 0);
			num2 += ((!flag) ? VirtualCurrencyHelper.gemsPriceIds[num] : VirtualCurrencyHelper.starterPackFakePrice[num]);
			PlayerPrefs.SetInt("ALLGems", num2);
		}
		else
		{
			num2 = PlayerPrefs.GetInt("ALLCoins", 0);
			num2 += ((!flag) ? VirtualCurrencyHelper.coinPriceIds[num] : VirtualCurrencyHelper.starterPackFakePrice[num]);
			PlayerPrefs.SetInt("ALLCoins", num2);
		}
		if (!flag)
		{
			Storager.setInt(Defs.AllCurrencyBought + ((!isGems) ? "Coins" : "GemsCurrency"), Storager.getInt(Defs.AllCurrencyBought + ((!isGems) ? "Coins" : "GemsCurrency"), false) + virtualCurrencyCount, false);
		}
		@int++;
		PlayerPrefs.SetInt("CountPaying", @int);
		if (@int >= 1 && PlayerPrefs.GetInt("Paying_User", 0) == 0)
		{
			PlayerPrefs.SetInt("Paying_User", 1);
			FacebookController.LogEvent("Paying_User");
			Debug.Log("Paying_User detected.");
		}
		if (@int > 1 && PlayerPrefs.GetInt("Paying_User_Dolphin", 0) == 0)
		{
			PlayerPrefs.SetInt("Paying_User_Dolphin", 1);
			FacebookController.LogEvent("Paying_User_Dolphin");
			Debug.Log("Paying_User_Dolphin detected.");
		}
		if (@int > 3 && PlayerPrefs.GetInt("Paying_User_Whale", 0) == 0)
		{
			PlayerPrefs.SetInt("Paying_User_Whale", 1);
			FacebookController.LogEvent("Paying_User_Whale");
			Debug.Log("Paying_User_Whale detected.");
		}
		if (num2 >= 100 && PlayerPrefs.GetInt("SendKit", 0) == 0)
		{
			PlayerPrefs.SetInt("SendKit", 1);
			FacebookController.LogEvent("Whale_detected");
			Debug.Log("Whale detected.");
		}
		if (PlayerPrefs.GetInt("confirmed_1st_time", 0) == 0)
		{
			PlayerPrefs.SetInt("confirmed_1st_time", 1);
			FacebookController.LogEvent("Purchase_confirmed_1st_time");
			Debug.Log("Purchase confirmed first time.");
		}
		if (PlayerPrefs.GetInt("Active_loyal_users_payed_send", 0) == 0 && PlayerPrefs.GetInt("PostFacebookCount", 0) > 2 && PlayerPrefs.GetInt("PostVideo", 0) > 0)
		{
			FacebookController.LogEvent("Active_loyal_users_payed");
			PlayerPrefs.SetInt("Active_loyal_users_payed_send", 1);
		}
	}
}

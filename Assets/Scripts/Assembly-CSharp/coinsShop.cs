using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.amazon.device.iap.cpt;
using Prime31;
using Rilisoft;
using UnityEngine;

internal sealed class coinsShop : MonoBehaviour
{
	public static coinsShop thisScript;

	public static bool showPlashkuPriExit = false;

	public Action onReturnAction;

	private float _timeWhenPurchShown = float.MinValue;

	private List<string> currenciesBought = new List<string>();

	private bool productsReceived;

	public Action onResumeFronNGUI;

	private static readonly HashSet<string> _loggedPackages = new HashSet<string>();

	private static DateTime? _etcFileTimestamp;

	private Action _drawInnerInterface;

	public string notEnoughCurrency { get; set; }

	public bool ProductPurchasedRecently
	{
		get
		{
			return Time.realtimeSinceStartup - _timeWhenPurchShown <= 1.25f;
		}
	}

	public static bool IsStoreAvailable
	{
		get
		{
			return !IsWideLayoutAvailable && !IsNoConnection;
		}
	}

	public static bool IsWideLayoutAvailable
	{
		get
		{
			return CheckAndroidHostsTampering() || CheckLuckyPatcherInstalled() || IsBangerrySupported() || HasTamperedProducts;
		}
	}

	internal static bool HasTamperedProducts { private get; set; }

	public static bool IsBillingSupported
	{
		get
		{
			if (!Application.isEditor)
			{
				return StoreKitEventListener.billingSupported;
			}
			return true;
		}
	}

	public static bool IsNoConnection
	{
		get
		{
			if (thisScript == null)
			{
				return true;
			}
			if (!thisScript.productsReceived)
			{
				return true;
			}
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				if (StoreKitEventListener.Instance == null)
				{
					return true;
				}
				return StoreKitEventListener.Instance.Products.Count() <= 0;
			}
			return !IsBillingSupported;
		}
	}

	private void HandleQueryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
	{
		if (!skus.Any((GoogleSkuInfo s) => s.productId == "skinsmaker"))
		{
			string[] value = skus.Select((GoogleSkuInfo sku) => sku.productId).ToArray();
			string arg = string.Join(", ", value);
			string message = string.Format("Google: Query inventory succeeded;\tPurchases count: {0}, Skus: [{1}]", purchases.Count, arg);
			Debug.Log(message);
			productsReceived = true;
		}
	}

	private void HandleItemDataRequestFinishedEvent(GetProductDataResponse response)
	{
		if (!"SUCCESSFUL".Equals(response.Status, StringComparison.OrdinalIgnoreCase))
		{
			Debug.LogWarning("Amazon GetProductDataResponse (CoinsShop): " + response.Status);
			return;
		}
		Debug.Log("Amazon GetProductDataResponse (CoinsShop): " + response.ToJson());
		productsReceived = true;
	}

	private void OnEnable()
	{
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AmazonIapV2Impl.Instance.AddPurchaseResponseListener(HandlePurchaseSuccessfulEvent);
		}
		else
		{
			GoogleIABManager.purchaseSucceededEvent += HandlePurchaseSucceededEvent;
		}
		if (Application.loadedLevelName != "Loading")
		{
			ActivityIndicator.IsActiveIndicator = false;
		}
		currenciesBought.Clear();
	}

	private void OnDisable()
	{
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AmazonIapV2Impl.Instance.RemovePurchaseResponseListener(HandlePurchaseSuccessfulEvent);
		}
		else
		{
			GoogleIABManager.purchaseSucceededEvent -= HandlePurchaseSucceededEvent;
		}
		ActivityIndicator.IsActiveIndicator = false;
		currenciesBought.Clear();
	}

	private void OnDestroy()
	{
		thisScript = null;
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AmazonIapV2Impl.Instance.RemoveGetProductDataResponseListener(HandleItemDataRequestFinishedEvent);
		}
		else
		{
			GoogleIABManager.queryInventorySucceededEvent -= HandleQueryInventorySucceededEvent;
		}
	}

	private void HandlePurchaseSuccessfullCore()
	{
		_timeWhenPurchShown = Time.realtimeSinceStartup;
	}

	private void HandlePurchaseSucceededEvent(GooglePurchase purchase)
	{
		HandlePurchaseSuccessfullCore();
	}

	private void HandlePurchaseSuccessfulEvent(PurchaseResponse response)
	{
		string message = "Amazon PurchaseResponse (CoinsShop): " + response.Status;
		if (!"SUCCESSFUL".Equals(response.Status, StringComparison.OrdinalIgnoreCase))
		{
			Debug.LogWarning(message);
			return;
		}
		Debug.Log(message);
		HandlePurchaseSuccessfullCore();
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		notEnoughCurrency = null;
		if (Application.isEditor)
		{
			productsReceived = true;
		}
		thisScript = this;
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AmazonIapV2Impl.Instance.AddGetProductDataResponseListener(HandleItemDataRequestFinishedEvent);
		}
		else
		{
			GoogleIABManager.queryInventorySucceededEvent += HandleQueryInventorySucceededEvent;
		}
		RefreshProductsIfNeed(false);
	}

	public static void TryToFireCurrenciesAddEvent(string currency)
	{
		try
		{
			CoinsMessage.FireCoinsAddedEvent(currency == "GemsCurrency");
		}
		catch (Exception ex)
		{
			Debug.LogError("coinsShop.TryToFireCurrenciesAddEvent: FireCoinsAddedEvent( currency == Defs.Gems ): " + ex);
		}
	}

	public void HandlePurchaseButton(int i, string currency, AbstractBankViewItem item)
	{
		ButtonClickSound.Instance.PlayClick();
		if ((currency.Equals("Coins") && (i >= StoreKitEventListener.coinIds.Length || i >= VirtualCurrencyHelper.coinInappsQuantity.Length)) || (currency.Equals("GemsCurrency") && (i >= StoreKitEventListener.gemsIds.Length || i >= VirtualCurrencyHelper.gemsInappsQuantity.Length)))
		{
			Debug.LogWarning("Index of purchase is out of range: " + i);
			return;
		}
		string empty = string.Empty;
		if ("Coins" == currency)
		{
			empty = StoreKitEventListener.coinIds[i];
		}
		else
		{
			if (!("GemsCurrency" == currency))
			{
				Debug.LogError("HandlePurchaseButton: Unknown currency: " + currency);
				return;
			}
			empty = StoreKitEventListener.gemsIds[i];
		}
		if (!InappBonuessController.Instance.InappBonusAlreadyBought(item.InappBonusParameters))
		{
			currenciesBought.Add(currency);
			StoreKitEventListener.purchaseInProcess = true;
			InappBonuessController.Instance.RememberCurrentBonusForInapp(empty, item.InappBonusParameters);
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				SkuInput skuInput = new SkuInput();
				skuInput.Sku = empty;
				SkuInput skuInput2 = skuInput;
				Debug.Log("Amazon Purchase (HandlePurchaseButton): " + skuInput2.ToJson());
				AmazonIapV2Impl.Instance.Purchase(skuInput2);
				MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			}
			else
			{
				_etcFileTimestamp = GetHostsTimestamp();
				AnalyticsFacade.SendCustomEventToAppsFlyer("af_initiated_checkout", new Dictionary<string, string> { { "af_content_id", empty } });
				GoogleIAB.purchaseProduct(empty);
				MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			}
		}
	}

	public static void showCoinsShop()
	{
		thisScript.enabled = true;
		coinsPlashka.hideButtonCoins = true;
		coinsPlashka.showPlashka();
	}

	public static void hideCoinsShop()
	{
		if (thisScript != null)
		{
			thisScript.enabled = false;
			thisScript.notEnoughCurrency = null;
			Resources.UnloadUnusedAssets();
		}
	}

	public static void ExitFromShop(bool performOnExitActs)
	{
		hideCoinsShop();
		if (showPlashkuPriExit)
		{
			coinsPlashka.hidePlashka();
		}
		coinsPlashka.hideButtonCoins = false;
		if (performOnExitActs)
		{
			if (thisScript.onReturnAction != null && thisScript.notEnoughCurrency != null && thisScript.currenciesBought.Contains(thisScript.notEnoughCurrency))
			{
				thisScript.currenciesBought.Clear();
				thisScript.onReturnAction();
			}
			else
			{
				thisScript.onReturnAction = null;
			}
			if (thisScript.onResumeFronNGUI != null)
			{
				thisScript.onResumeFronNGUI();
				thisScript.onResumeFronNGUI = null;
				coinsPlashka.hidePlashka();
			}
		}
	}

	internal static bool CheckAndroidHostsTampering()
	{
		//Discarded unreachable code: IL_0082, IL_0095
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
		{
			return false;
		}
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
		{
			if (!File.Exists("/etc/hosts"))
			{
				return false;
			}
			try
			{
				string[] source = File.ReadAllLines("/etc/hosts");
				IEnumerable<string> source2 = source.Where((string l) => l.TrimStart().StartsWith("127."));
				return source2.Any((string l) => l.Contains("android.clients.google.com") || l.Contains("mtalk.google.com "));
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				return false;
			}
		}
		return false;
	}

	internal static bool CheckLuckyPatcherInstalled()
	{
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
		{
			return false;
		}
		string[] source = new string[3] { "Y29tLmRpbW9udmlkZW8ubHVja3lwYXRjaGVy", "Y29tLmNoZWxwdXMubGFja3lwYXRjaA==", "Y29tLmZvcnBkYS5scA==" };
		IEnumerable<string> source2 = from bytes in source.Select(Convert.FromBase64String)
			where bytes != null
			select Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		return source2.Any(PackageExists);
	}

	private static bool PackageExists(string packageName)
	{
		//Discarded unreachable code: IL_00a9
		if (packageName == null)
		{
			throw new ArgumentNullException("packageName");
		}
		if (Application.isEditor)
		{
			return false;
		}
		try
		{
			AndroidJavaObject currentActivity = AndroidSystem.Instance.CurrentActivity;
			if (currentActivity == null)
			{
				Debug.LogWarning("activity == null");
				return false;
			}
			AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>("getPackageManager", new object[0]);
			if (androidJavaObject == null)
			{
				Debug.LogWarning("manager == null");
				return false;
			}
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageInfo", new object[2] { packageName, 0 });
			if (androidJavaObject2 == null)
			{
				Debug.LogWarning("packageInfo == null");
				return false;
			}
			return true;
		}
		catch (Exception arg)
		{
			if (_loggedPackages.Contains(packageName))
			{
				return false;
			}
			string message = string.Format("Error while retrieving Android package info:    {0}", arg);
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogWarning(message);
				_loggedPackages.Add(packageName);
			}
		}
		return false;
	}

	private static string ConvertFromBase64(string s)
	{
		byte[] array = Convert.FromBase64String(s);
		return Encoding.UTF8.GetString(array, 0, array.Length);
	}

	private static bool IsBangerrySupported()
	{
		//Discarded unreachable code: IL_0103, IL_012c
		try
		{
			if (BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer)
			{
				return false;
			}
			string path = ConvertFromBase64("L0xpYnJhcnkvTW9iaWxlU3Vic3RyYXRlL0R5bmFtaWNMaWJyYXJpZXM=");
			if (File.Exists(Path.Combine(path, ConvertFromBase64("TG9jYWxJQVBTdG9yZS5keWxpYg=="))) || File.Exists(Path.Combine(path, ConvertFromBase64("TG9jYWxsQVBTdG9yZS5keWxpYg=="))))
			{
				Debug.LogWarningFormat("{0}: `cetrer`", "IsBangerrySupported");
				return true;
			}
			if (File.Exists(Path.Combine(path, ConvertFromBase64("aWFwLmR5bGli"))))
			{
				Debug.LogWarningFormat("{0}: `panemer`", "IsBangerrySupported");
				return true;
			}
			if (File.Exists(Path.Combine(path, ConvertFromBase64("aWFwZnJlZS5jb3JlLmR5bGli"))) || File.Exists(Path.Combine(path, ConvertFromBase64("SUFQRnJlZVNlcnZpY2UuZHlsaWI="))))
			{
				Debug.LogWarningFormat("{0}: `rastat`", "IsBangerrySupported");
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Debug.LogWarningFormat("Exception in {0}: {1}", "IsBangerrySupported", ex);
			return false;
		}
	}

	private static DateTime? GetHostsTimestamp()
	{
		//Discarded unreachable code: IL_0043, IL_005f
		try
		{
			Debug.Log("Trying to get /ets/hosts timestamp...");
			FileInfo fileInfo = new FileInfo("/etc/hosts");
			DateTime lastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
			Debug.Log("/ets/hosts timestamp: " + lastWriteTimeUtc.ToString("s"));
			return lastWriteTimeUtc;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}

	internal static bool CheckHostsTimestamp()
	{
		if (_etcFileTimestamp.HasValue)
		{
			DateTime? hostsTimestamp = GetHostsTimestamp();
			if (hostsTimestamp.HasValue && _etcFileTimestamp.Value != hostsTimestamp.Value)
			{
				Debug.LogError(string.Format("Timestamp check failed: {0:s} expcted, but actual value is {1:s}.", _etcFileTimestamp.Value, hostsTimestamp.Value));
				return false;
			}
		}
		return true;
	}

	public void RefreshProductsIfNeed(bool force = false)
	{
		if (!productsReceived || force)
		{
			StoreKitEventListener.RefreshProducts();
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			RefreshProductsIfNeed(false);
		}
	}
}

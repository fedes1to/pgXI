using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public class BalanceController : MonoBehaviour
{
	public static string curencyActionName = "currence";

	public static string leprechaunActionName = "leprechaun";

	public static string petActionName = "pet";

	public static string weaponActionName = "weapon";

	public static string gadgetActionName = "gadget";

	public static List<string> supportedInappBonusIds = new List<string> { curencyActionName, petActionName, weaponActionName, gadgetActionName, leprechaunActionName };

	[HideInInspector]
	public string jsonConfig;

	public static BalanceController sharedController = null;

	[SerializeField]
	private string encryptedPlayerprefsKey;

	public static readonly string balanceKey = "balanceKey";

	public static Dictionary<string, float[]> dpsWeapons = new Dictionary<string, float[]>();

	public static Dictionary<string, float[]> damageWeapons = new Dictionary<string, float[]>();

	public static Dictionary<string, int> survivalDamageWeapons = new Dictionary<string, int>();

	private static Dictionary<string, List<ItemPrice>> _gunPricesFromServerNew = new Dictionary<string, List<ItemPrice>>();

	public static Dictionary<string, float> damageGadgetes = new Dictionary<string, float>();

	public static Dictionary<string, float> amplificationGadgetes = new Dictionary<string, float>();

	public static Dictionary<string, float> dpsGadgetes = new Dictionary<string, float>();

	public static Dictionary<string, int> survivalDamageGadgetes = new Dictionary<string, int>();

	public static Dictionary<string, float> cooldownGadgetes = new Dictionary<string, float>();

	public static Dictionary<string, float> durationGadgetes = new Dictionary<string, float>();

	public static Dictionary<string, float> durabilityGadgetes = new Dictionary<string, float>();

	public static Dictionary<string, float> healGadgetes = new Dictionary<string, float>();

	public static Dictionary<string, float> hpsGadgetes = new Dictionary<string, float>();

	public static Dictionary<string, float> damagePets = new Dictionary<string, float>();

	public static Dictionary<string, int> dpsPets = new Dictionary<string, int>();

	public static Dictionary<string, int> survivalDamagePets = new Dictionary<string, int>();

	public static Dictionary<string, float> respawnTimePets = new Dictionary<string, float>();

	public static Dictionary<string, float> speedPets = new Dictionary<string, float>();

	public static Dictionary<string, int> cashbackPets = new Dictionary<string, int>();

	public static Dictionary<string, float> hpPets = new Dictionary<string, float>();

	public static Dictionary<string, int> timeEggs = new Dictionary<string, int>();

	public static Dictionary<string, int> victoriasEggs = new Dictionary<string, int>();

	public static Dictionary<string, int> ratingEggs = new Dictionary<string, int>();

	public static Dictionary<string, List<EggPetInfo>> rarityPetsInEggs = new Dictionary<string, List<EggPetInfo>>();

	public static List<Dictionary<string, object>> inappsBonus = new List<Dictionary<string, object>>();

	private static Dictionary<string, List<ItemPrice>> _gadgetPricesFromServerNew = new Dictionary<string, List<ItemPrice>>();

	public static int startCapitalCoins = 0;

	public static int startCapitalGems = 0;

	public static bool startCapitalEnabled = false;

	public static ItemPrice competitionAward = new ItemPrice(0, "Coins");

	public static int countPlaceAwardInCompetion = 0;

	private static Dictionary<string, ItemPrice> _pricesFromServer = new Dictionary<string, ItemPrice>();

	private static string _inappObj = null;

	private static string _inappObjBonus = null;

	private static List<string> _curentGadgetesIDs = null;

	private static List<string> _keysInappBonusActionGiven = null;

	private static List<Dictionary<string, object>> cacheCurrentInnapBonus = null;

	private static int countFrameInCache = -1;

	private static Dictionary<string, int> curPackDict = new Dictionary<string, int>();

	private static Dictionary<string, DateTime> timeNextUpdateDict = new Dictionary<string, DateTime>();

	private EncryptedPlayerPrefs _encryptedPlayerPrefs;

	private string EncryptedPlayerprefsKey
	{
		get
		{
			return encryptedPlayerprefsKey ?? string.Empty;
		}
	}

	public static string balanceURL
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/balance/balance_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "https://secure.pixelgunserver.com/pixelgun3d-config/balance/balance_androd.json" : "https://secure.pixelgunserver.com/pixelgun3d-config/balance/balance_amazon.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/balance/balance_wp.json";
			}
			return "https://secure.pixelgunserver.com/pixelgun3d-config/balance/balance_ios.json";
		}
	}

	public static Dictionary<string, List<ItemPrice>> GunPricesFromServer
	{
		get
		{
			return _gunPricesFromServerNew;
		}
		set
		{
			_gunPricesFromServerNew = value;
		}
	}

	public static Dictionary<string, List<ItemPrice>> GadgetPricesFromServer
	{
		get
		{
			return _gadgetPricesFromServerNew;
		}
		set
		{
			_gadgetPricesFromServerNew = value;
		}
	}

	public static Dictionary<string, ItemPrice> pricesFromServer
	{
		get
		{
			return _pricesFromServer;
		}
		set
		{
			_pricesFromServer = value;
		}
	}

	private static List<string> curentGadgetesIDs
	{
		get
		{
			//Discarded unreachable code: IL_0094
			if (_curentGadgetesIDs == null)
			{
				try
				{
					string @string = Storager.getString(Defs.keyInappPresentIDGadgetkey, false);
					if (!string.IsNullOrEmpty(@string))
					{
						List<object> list = Json.Deserialize(@string) as List<object>;
						if (list != null && list.Count == 3)
						{
							_curentGadgetesIDs = new List<string>();
							for (int i = 0; i < list.Count; i++)
							{
								_curentGadgetesIDs.Add(list[i].ToString());
							}
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("Parse curentGadgetesIDs: " + ex);
					return null;
				}
			}
			return _curentGadgetesIDs;
		}
		set
		{
			_curentGadgetesIDs = value;
			string val = Json.Serialize(_curentGadgetesIDs);
			Storager.setString(Defs.keyInappPresentIDGadgetkey, val, false);
		}
	}

	public static List<string> keysInappBonusActionGiven
	{
		get
		{
			if (_keysInappBonusActionGiven == null)
			{
				_keysInappBonusActionGiven = new List<string>();
				string @string = Storager.getString(Defs.keysInappBonusGivenkey, false);
				List<object> list = null;
				if (!string.IsNullOrEmpty(@string))
				{
					list = Json.Deserialize(@string) as List<object>;
				}
				if (list != null)
				{
					foreach (object item in list)
					{
						_keysInappBonusActionGiven.Add(item.ToString());
					}
				}
			}
			return _keysInappBonusActionGiven;
		}
	}

	public static event Action UpdatedBankView;

	private void Awake()
	{
		byte[] masterKey = Convert.FromBase64String(EncryptedPlayerprefsKey);
		_encryptedPlayerPrefs = new EncryptedPlayerPrefs(masterKey);
	}

	private void Start()
	{
		sharedController = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		ParseConfig(false);
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64)
		{
			StartCoroutine(GetBalansFromServer());
		}
	}

	[Obfuscation(Exclude = true)]
	private void UpdateBalansFromServer()
	{
		StopCoroutine(GetBalansFromServer());
		StartCoroutine(GetBalansFromServer());
	}

	private IEnumerator GetBalansFromServer()
	{
		string responseText;
		while (true)
		{
			Task futureToWait = PersistentCacheManager.Instance.FirstResponse;
			if (_encryptedPlayerPrefs.HasKey(balanceKey) || !string.IsNullOrEmpty(_encryptedPlayerPrefs.GetString(balanceKey)))
			{
				yield return new WaitUntil(() => futureToWait.IsCompleted);
				string cachedResponse = PersistentCacheManager.Instance.GetValue(balanceURL);
				if (!string.IsNullOrEmpty(cachedResponse))
				{
					yield break;
				}
			}
			WWWForm form = new WWWForm();
			WWW download = Tools.CreateWwwIfNotConnected(balanceURL);
			if (download == null)
			{
				yield return new WaitForRealSeconds(30f);
				continue;
			}
			yield return download;
			if (!string.IsNullOrEmpty(download.error))
			{
				if (Debug.isDebugBuild || Application.isEditor)
				{
					Debug.LogWarning("GetBalans error: " + download.error);
				}
				yield return new WaitForRealSeconds(30f);
			}
			else
			{
				responseText = URLs.Sanitize(download);
				if (!string.IsNullOrEmpty(responseText))
				{
					break;
				}
			}
		}
		ScopeLogger scopeLogger = new ScopeLogger("GetBalansFromServer()", "Saving to storager", Defs.IsDeveloperBuild);
		try
		{
			_encryptedPlayerPrefs.SetString(balanceKey, responseText);
		}
		finally
		{
			scopeLogger.Dispose();
		}
		ScopeLogger scopeLogger2 = new ScopeLogger("GetBalansFromServer()", "Saving to cache", Defs.IsDeveloperBuild);
		try
		{
			PersistentCacheManager.Instance.SetValue(balanceURL, responseText);
		}
		finally
		{
			scopeLogger2.Dispose();
		}
		ParseConfig(false);
		if (Debug.isDebugBuild)
		{
			Debug.Log("GetConfigABtestBalans");
		}
	}

	public void ParseConfig(bool isFirstParse = false)
	{
		Dictionary<string, object> dictionary = null;
		string @string = Storager.getString("abTestBalansConfig2Key", false);
		if (!string.IsNullOrEmpty(@string))
		{
			dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
			if (dictionary == null)
			{
				Storager.setString("abTestBalansConfig2Key", string.Empty, false);
				Debug.LogError("AB TEST BALANCE CONFIG NOT CORRECT !!!");
				if (Defs.IsDeveloperBuild)
				{
					Debug.Log("jsonConfigABTest = ' " + @string + "'");
				}
				return;
			}
			if (dictionary.ContainsKey("NameConfig"))
			{
				ParseABTestBalansNameConfig(dictionary["NameConfig"], isFirstParse);
			}
			if (Defs.abTestBalansCohort == Defs.ABTestCohortsType.A)
			{
				if (_encryptedPlayerPrefs.HasKey(balanceKey) || !string.IsNullOrEmpty(_encryptedPlayerPrefs.GetString(balanceKey)))
				{
					jsonConfig = _encryptedPlayerPrefs.GetString(balanceKey);
				}
				dictionary = Json.Deserialize(jsonConfig) as Dictionary<string, object>;
			}
		}
		else
		{
			if (Defs.abTestBalansCohort != 0)
			{
				AnalyticsStuff.LogABTest("New Balance", Defs.abTestBalansCohortName, false);
				Defs.abTestBalansCohort = Defs.ABTestCohortsType.NONE;
				Defs.abTestBalansCohortName = string.Empty;
				if (FriendsController.sharedController != null)
				{
					FriendsController.sharedController.SendOurData(false);
				}
			}
			if (_encryptedPlayerPrefs.HasKey(balanceKey) || !string.IsNullOrEmpty(_encryptedPlayerPrefs.GetString(balanceKey)))
			{
				jsonConfig = _encryptedPlayerPrefs.GetString(balanceKey);
			}
			if (string.IsNullOrEmpty(jsonConfig))
			{
				Debug.LogError("BALANCE CONFIG EMPTY !!!");
				return;
			}
			try
			{
				dictionary = Json.Deserialize(jsonConfig) as Dictionary<string, object>;
			}
			catch (Exception ex)
			{
				Debug.LogError("Balans Controller Error parse config: " + ex.Message);
			}
		}
		if (dictionary == null)
		{
			Debug.LogError("BALANCE CONFIG NOT CORRECT !!!");
			return;
		}
		pricesFromServer.Clear();
		if (dictionary.ContainsKey("Weapons"))
		{
			ParseWeaponsConfig(dictionary["Weapons"] as Dictionary<string, object>);
		}
		if (dictionary.ContainsKey("Gadgets"))
		{
			ParseGadgetsConfig(dictionary["Gadgets"] as Dictionary<string, object>);
		}
		if (dictionary.ContainsKey("Pets"))
		{
			ParsePetsConfig(dictionary["Pets"] as Dictionary<string, object>);
		}
		if (dictionary.ContainsKey("Eggs"))
		{
			ParseEggsConfig(dictionary["Eggs"] as Dictionary<string, object>);
		}
		if (dictionary.ContainsKey("ItemPrices"))
		{
			ParseItemPricesConfig(dictionary["ItemPrices"] as Dictionary<string, object>);
		}
		if (dictionary.ContainsKey("Levelling"))
		{
			ParseLevelingConfig(dictionary["Levelling"]);
		}
		bool flag = false;
		if (dictionary.ContainsKey("Inapps"))
		{
			flag = ParseInappsConfig(dictionary["Inapps"]);
		}
		bool flag2 = false;
		if (dictionary.ContainsKey("InappsBonus"))
		{
			flag2 = ParseInappsBonusConfig(dictionary["InappsBonus"]);
		}
		else
		{
			inappsBonus.Clear();
			_inappObjBonus = string.Empty;
		}
		if ((flag || flag2) && BalanceController.UpdatedBankView != null)
		{
			BalanceController.UpdatedBankView();
		}
		if (dictionary.ContainsKey("Rewards"))
		{
			ParseAwardConfig(dictionary["Rewards"]);
		}
		if (dictionary.ContainsKey("Levelling"))
		{
			ParseLevelingConfig(dictionary["Levelling"]);
		}
		if (dictionary.ContainsKey("SpecialEvents"))
		{
			ParseSpecialEventsConfig(dictionary["SpecialEvents"]);
		}
	}

	private static void ParseABTestBalansNameConfig(object obj, bool isFirstParse)
	{
		List<object> list = obj as List<object>;
		Dictionary<string, object> dictionary = list[0] as Dictionary<string, object>;
		if (dictionary.ContainsKey("Group"))
		{
			Defs.abTestBalansCohort = (Defs.ABTestCohortsType)(int)Enum.Parse(typeof(Defs.ABTestCohortsType), dictionary["Group"] as string);
			if (Defs.abTestBalansCohort == Defs.ABTestCohortsType.B)
			{
				Defs.isABTestBalansCohortActual = true;
			}
			if (Application.isEditor)
			{
				Debug.Log("Defs.abTestBalansCohort = " + Defs.abTestBalansCohort);
			}
		}
		if (!dictionary.ContainsKey("NameGroup"))
		{
			return;
		}
		Defs.abTestBalansCohortName = dictionary["NameGroup"] as string;
		if (isFirstParse)
		{
			AnalyticsStuff.LogABTest("New Balance", Defs.abTestBalansCohortName);
			if (FriendsController.sharedController != null)
			{
				FriendsController.sharedController.SendOurData(false);
			}
		}
		if (Application.isEditor)
		{
			Debug.Log("abTestBalansCohortName = " + Defs.abTestBalansCohortName.ToString());
		}
	}

	private void ParseWeaponsConfig(Dictionary<string, object> _weaponsConfig)
	{
		dpsWeapons.Clear();
		damageWeapons.Clear();
		survivalDamageWeapons.Clear();
		GunPricesFromServer.Clear();
		foreach (KeyValuePair<string, object> item in _weaponsConfig)
		{
			if (string.IsNullOrEmpty(item.Key))
			{
				continue;
			}
			Dictionary<string, object> dictionary = item.Value as Dictionary<string, object>;
			int num = 1;
			if (dictionary.ContainsKey("D1"))
			{
				float[] array = new float[6];
				for (int i = 2; i <= 6; i++)
				{
					if (dictionary.ContainsKey("D" + i))
					{
						num = i;
					}
				}
				float num2 = 0.1f;
				float num3 = 0.1f;
				float result;
				if (float.TryParse(dictionary["D1"].ToString(), out result))
				{
					num2 = result;
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseWeaponsConfig: not parse " + item.Key + " parametr D1");
				}
				if (num > 1)
				{
					float result2;
					if (float.TryParse(dictionary["D" + num].ToString(), out result2))
					{
						num3 = result2;
					}
					else if (Application.isEditor)
					{
						Debug.LogError("ParseWeaponsConfig: not parse " + item.Key + " parametr D" + num);
					}
				}
				else
				{
					num3 = num2;
				}
				for (int j = 1; j <= 6; j++)
				{
					array[j - 1] = ((j >= num) ? num3 : (num2 + (num3 - num2) / (float)(num - 1) * (float)(j - 1)));
				}
				dpsWeapons.Add("Weapon" + item.Key, array);
			}
			if (dictionary.ContainsKey("U1"))
			{
				float[] array2 = new float[6];
				float num4 = 0.1f;
				float num5 = 0.1f;
				float result3;
				if (float.TryParse(dictionary["U1"].ToString(), out result3))
				{
					num4 = result3;
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseWeaponsConfig: not parse " + item.Key + " parametr U1");
				}
				if (num > 1)
				{
					float result4;
					if (float.TryParse(dictionary["U" + num].ToString(), out result4))
					{
						num5 = result4;
					}
					else if (Application.isEditor)
					{
						Debug.LogError("ParseWeaponsConfig: not parse " + item.Key + " parametr U" + num);
					}
				}
				else
				{
					num5 = num4;
				}
				for (int k = 1; k <= 6; k++)
				{
					array2[k - 1] = ((k >= num) ? num5 : (num4 + (num5 - num4) / (float)(num - 1) * (float)(k - 1)));
				}
				damageWeapons.Add("Weapon" + item.Key, array2);
			}
			if (dictionary.ContainsKey("S"))
			{
				int result5;
				if (int.TryParse(dictionary["S"].ToString(), out result5))
				{
					survivalDamageWeapons.Add(item.Key, result5);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseWeaponsConfig: not parse " + item.Key + " parametr Survival_damage ");
				}
			}
			if (dictionary.ContainsKey("P"))
			{
				string text = dictionary["P"].ToString();
				int num6 = Convert.ToInt32(text.Substring(1));
				int price = ((!dictionary.ContainsKey("oP")) ? num6 : Convert.ToInt32(dictionary["oP"]));
				string currency = ((!text.Substring(0, 1).Equals("C")) ? "GemsCurrency" : "Coins");
				GunPricesFromServer.Add("Weapon" + item.Key, new List<ItemPrice>
				{
					new ItemPrice(price, currency),
					new ItemPrice(num6, currency)
				});
			}
		}
	}

	private void ParseGadgetsConfig(Dictionary<string, object> _gadgetsConfig)
	{
		GadgetPricesFromServer.Clear();
		damageGadgetes.Clear();
		dpsGadgetes.Clear();
		survivalDamageGadgetes.Clear();
		cooldownGadgetes.Clear();
		durationGadgetes.Clear();
		durabilityGadgetes.Clear();
		healGadgetes.Clear();
		hpsGadgetes.Clear();
		foreach (KeyValuePair<string, object> item in _gadgetsConfig)
		{
			if (string.IsNullOrEmpty(item.Key))
			{
				continue;
			}
			Dictionary<string, object> dictionary = item.Value as Dictionary<string, object>;
			string key = "gadget_" + item.Key;
			string text = "CD";
			if (dictionary.ContainsKey(text))
			{
				float result;
				if (float.TryParse(dictionary[text].ToString(), out result))
				{
					cooldownGadgetes.Add(key, result);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseGadgetConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "T";
			if (dictionary.ContainsKey(text))
			{
				float result2;
				if (float.TryParse(dictionary[text].ToString(), out result2))
				{
					durationGadgetes.Add(key, result2);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseGadgetConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "D";
			if (dictionary.ContainsKey(text))
			{
				float result3;
				if (float.TryParse(dictionary[text].ToString(), out result3))
				{
					damageGadgetes.Add(key, result3);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseGadgetConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "DPS";
			if (dictionary.ContainsKey(text))
			{
				float result4;
				if (float.TryParse(dictionary[text].ToString(), out result4))
				{
					dpsGadgetes.Add(key, result4);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseGadgetConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "SD";
			if (dictionary.ContainsKey(text))
			{
				int result5;
				if (int.TryParse(dictionary[text].ToString(), out result5))
				{
					survivalDamageGadgetes.Add(key, result5);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseGadgetConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "Dur";
			if (dictionary.ContainsKey(text))
			{
				float result6;
				if (float.TryParse(dictionary[text].ToString(), out result6))
				{
					durabilityGadgetes.Add(key, result6);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseGadgetConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "H";
			if (dictionary.ContainsKey(text))
			{
				float result7;
				if (float.TryParse(dictionary[text].ToString(), out result7))
				{
					healGadgetes.Add(key, result7);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseGadgetConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "HPS";
			if (dictionary.ContainsKey(text))
			{
				float result8;
				if (float.TryParse(dictionary[text].ToString(), out result8))
				{
					hpsGadgetes.Add(key, result8);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseGadgetConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "P";
			if (!dictionary.ContainsKey(text))
			{
				continue;
			}
			string text2 = dictionary[text].ToString();
			int num = Convert.ToInt32(text2.Substring(1));
			string key2 = "oP";
			int result9;
			if (dictionary.ContainsKey(key2))
			{
				if (!int.TryParse(dictionary[key2].ToString(), out result9))
				{
					result9 = num;
					if (Application.isEditor)
					{
						Debug.LogError("ParseGadgetConfig: not parse " + item.Key + " parametr " + text);
					}
				}
			}
			else
			{
				result9 = num;
			}
			string currency = ((!text2.Substring(0, 1).Equals("C")) ? "GemsCurrency" : "Coins");
			GadgetPricesFromServer.Add(key, new List<ItemPrice>
			{
				new ItemPrice(result9, currency),
				new ItemPrice(num, currency)
			});
		}
	}

	private void ParsePetsConfig(Dictionary<string, object> _petsConfig)
	{
		damagePets.Clear();
		dpsPets.Clear();
		survivalDamagePets.Clear();
		respawnTimePets.Clear();
		hpPets.Clear();
		speedPets.Clear();
		cashbackPets.Clear();
		foreach (KeyValuePair<string, object> item in _petsConfig)
		{
			if (string.IsNullOrEmpty(item.Key))
			{
				continue;
			}
			Dictionary<string, object> dictionary = item.Value as Dictionary<string, object>;
			string key = "pet_" + item.Key;
			string text = "R";
			if (dictionary.ContainsKey(text))
			{
				float result;
				if (float.TryParse(dictionary[text].ToString(), out result))
				{
					respawnTimePets.Add(key, result);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParsePetsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "HP";
			if (dictionary.ContainsKey(text))
			{
				float result2;
				if (float.TryParse(dictionary[text].ToString(), out result2))
				{
					hpPets.Add(key, result2);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParsePetsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "DPS";
			if (dictionary.ContainsKey(text))
			{
				int result3;
				if (int.TryParse(dictionary[text].ToString(), out result3))
				{
					dpsPets.Add(key, result3);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParsePetsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "D";
			if (dictionary.ContainsKey(text))
			{
				float result4;
				if (float.TryParse(dictionary[text].ToString(), out result4))
				{
					damagePets.Add(key, result4);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParsePetsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "SD";
			if (dictionary.ContainsKey(text))
			{
				int result5;
				if (int.TryParse(dictionary[text].ToString(), out result5))
				{
					survivalDamagePets.Add(key, result5);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParsePetsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "S";
			if (dictionary.ContainsKey(text))
			{
				float result6;
				if (float.TryParse(dictionary[text].ToString(), out result6))
				{
					speedPets.Add(key, result6);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParsePetsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "C";
			if (dictionary.ContainsKey(text))
			{
				int result7;
				if (int.TryParse(dictionary[text].ToString(), out result7))
				{
					cashbackPets.Add(key, result7);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParsePetsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "P";
			if (dictionary.ContainsKey(text))
			{
				string text2 = dictionary[text].ToString();
				int price = Convert.ToInt32(text2.Substring(1));
				string currency = ((!text2.Substring(0, 1).Equals("C")) ? "GemsCurrency" : "Coins");
				pricesFromServer.Add(key, new ItemPrice(price, currency));
			}
		}
	}

	private void ParseEggsConfig(Dictionary<string, object> _eggsConfig)
	{
		timeEggs.Clear();
		victoriasEggs.Clear();
		ratingEggs.Clear();
		rarityPetsInEggs.Clear();
		foreach (KeyValuePair<string, object> item in _eggsConfig)
		{
			if (string.IsNullOrEmpty(item.Key))
			{
				continue;
			}
			Dictionary<string, object> dictionary = item.Value as Dictionary<string, object>;
			string key = item.Key;
			string text = "T";
			if (dictionary.ContainsKey(text))
			{
				int result;
				if (int.TryParse(dictionary[text].ToString(), out result))
				{
					timeEggs.Add(key, result);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseEggsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "V";
			if (dictionary.ContainsKey(text))
			{
				int result2;
				if (int.TryParse(dictionary[text].ToString(), out result2))
				{
					victoriasEggs.Add(key, result2);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseEggsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "Rat";
			if (dictionary.ContainsKey(text))
			{
				int result3;
				if (int.TryParse(dictionary[text].ToString(), out result3))
				{
					ratingEggs.Add(key, result3);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseEggsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "C";
			if (dictionary.ContainsKey(text))
			{
				float result4;
				if (float.TryParse(dictionary[text].ToString(), out result4))
				{
					if (!rarityPetsInEggs.ContainsKey(key))
					{
						rarityPetsInEggs.Add(key, new List<EggPetInfo>());
					}
					EggPetInfo eggPetInfo = new EggPetInfo();
					eggPetInfo.Rarity = ItemDb.ItemRarity.Common;
					eggPetInfo.Chance = result4;
					rarityPetsInEggs[key].Add(eggPetInfo);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseEggsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "U";
			if (dictionary.ContainsKey(text))
			{
				float result5;
				if (float.TryParse(dictionary[text].ToString(), out result5))
				{
					if (!rarityPetsInEggs.ContainsKey(key))
					{
						rarityPetsInEggs.Add(key, new List<EggPetInfo>());
					}
					EggPetInfo eggPetInfo2 = new EggPetInfo();
					eggPetInfo2.Rarity = ItemDb.ItemRarity.Uncommon;
					eggPetInfo2.Chance = result5;
					rarityPetsInEggs[key].Add(eggPetInfo2);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseEggsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "R";
			if (dictionary.ContainsKey(text))
			{
				float result6;
				if (float.TryParse(dictionary[text].ToString(), out result6))
				{
					if (!rarityPetsInEggs.ContainsKey(key))
					{
						rarityPetsInEggs.Add(key, new List<EggPetInfo>());
					}
					EggPetInfo eggPetInfo3 = new EggPetInfo();
					eggPetInfo3.Rarity = ItemDb.ItemRarity.Rare;
					eggPetInfo3.Chance = result6;
					rarityPetsInEggs[key].Add(eggPetInfo3);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseEggsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "E";
			if (dictionary.ContainsKey(text))
			{
				float result7;
				if (float.TryParse(dictionary[text].ToString(), out result7))
				{
					if (!rarityPetsInEggs.ContainsKey(key))
					{
						rarityPetsInEggs.Add(key, new List<EggPetInfo>());
					}
					EggPetInfo eggPetInfo4 = new EggPetInfo();
					eggPetInfo4.Rarity = ItemDb.ItemRarity.Epic;
					eggPetInfo4.Chance = result7;
					rarityPetsInEggs[key].Add(eggPetInfo4);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseEggsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "L";
			if (dictionary.ContainsKey(text))
			{
				float result8;
				if (float.TryParse(dictionary[text].ToString(), out result8))
				{
					if (!rarityPetsInEggs.ContainsKey(key))
					{
						rarityPetsInEggs.Add(key, new List<EggPetInfo>());
					}
					EggPetInfo eggPetInfo5 = new EggPetInfo();
					eggPetInfo5.Rarity = ItemDb.ItemRarity.Legendary;
					eggPetInfo5.Chance = result8;
					rarityPetsInEggs[key].Add(eggPetInfo5);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseEggsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "M";
			if (dictionary.ContainsKey(text))
			{
				float result9;
				if (float.TryParse(dictionary[text].ToString(), out result9))
				{
					if (!rarityPetsInEggs.ContainsKey(key))
					{
						rarityPetsInEggs.Add(key, new List<EggPetInfo>());
					}
					EggPetInfo eggPetInfo6 = new EggPetInfo();
					eggPetInfo6.Rarity = ItemDb.ItemRarity.Mythic;
					eggPetInfo6.Chance = result9;
					rarityPetsInEggs[key].Add(eggPetInfo6);
				}
				else if (Application.isEditor)
				{
					Debug.LogError("ParseEggsConfig: not parse " + item.Key + " parametr " + text);
				}
			}
			text = "P";
			if (dictionary.ContainsKey(text))
			{
				string text2 = dictionary[text].ToString();
				int price = Convert.ToInt32(text2.Substring(1));
				string currency = ((!text2.Substring(0, 1).Equals("C")) ? "GemsCurrency" : "Coins");
				pricesFromServer.Add((!(key == "SI")) ? key : "Eggs.SuperIncubatorId", new ItemPrice(price, currency));
			}
		}
	}

	private void ParseItemPricesConfig(Dictionary<string, object> _itemPricesConfig)
	{
		foreach (KeyValuePair<string, object> item in _itemPricesConfig)
		{
			string text = item.Value.ToString();
			int price = Convert.ToInt32(text.Substring(1));
			string currency = ((!text.Substring(0, 1).Equals("C")) ? "GemsCurrency" : "Coins");
			pricesFromServer.Add(item.Key, new ItemPrice(price, currency));
		}
	}

	private static void ParseLevelingConfig(object obj)
	{
		List<object> list = obj as List<object>;
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
			int num = Convert.ToInt32(dictionary["L"]);
			int coins = Convert.ToInt32(dictionary["C"]);
			int gems = Convert.ToInt32(dictionary["G"]);
			ExperienceController.RewriteLevelingParametersForLevel(num - 1, coins, gems);
		}
	}

	private static bool ParseInappsConfig(object obj)
	{
		string text = Json.Serialize(obj);
		if (text == _inappObj)
		{
			return false;
		}
		_inappObj = text;
		List<object> list = obj as List<object>;
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
			int priceId = Convert.ToInt32(dictionary["Real"]);
			int coinQuantity = Convert.ToInt32(dictionary["C"]);
			int gemsQuantity = Convert.ToInt32(dictionary["G"]);
			int bonusCoins = Convert.ToInt32(dictionary["BC"]);
			int bonusGems = Convert.ToInt32(dictionary["BG"]);
			VirtualCurrencyHelper.RewriteInappsQuantity(priceId, coinQuantity, gemsQuantity, bonusCoins, bonusGems);
		}
		return true;
	}

	private static bool ParseInappsBonusConfig(object obj)
	{
		string text = Json.Serialize(obj);
		if (text == _inappObjBonus)
		{
			return false;
		}
		_inappObjBonus = text;
		inappsBonus.Clear();
		List<object> list = obj as List<object>;
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
			if (!dictionary.ContainsKey("ID"))
			{
				continue;
			}
			string item = Convert.ToString(dictionary["ID"]);
			if (!supportedInappBonusIds.Contains(item))
			{
				continue;
			}
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			string value = Convert.ToString(dictionary["ID"]);
			int num = Convert.ToInt32(dictionary["Real"]);
			bool flag = Convert.ToString(dictionary["Cur"]) != "Coins";
			string value2 = string.Empty;
			string[] array = ((!flag) ? StoreKitEventListener.coinIds : StoreKitEventListener.gemsIds);
			int num2 = 0;
			for (int j = 0; j < VirtualCurrencyHelper.coinPriceIds.Length; j++)
			{
				if (VirtualCurrencyHelper.coinPriceIds[j] == num)
				{
					value2 = array[j];
					num2 = j;
					break;
				}
			}
			if (string.IsNullOrEmpty(value2))
			{
				continue;
			}
			dictionary2.Add("action", value);
			dictionary2.Add("isGems", flag);
			dictionary2.Add("Start", dictionary["Start"]);
			dictionary2.Add("End", dictionary["End"]);
			float num3 = (float)num / (float)(VirtualCurrencyHelper.gemsInappsQuantity[num2] * 3);
			float num4 = (float)num / (float)(VirtualCurrencyHelper.coinInappsQuantity[num2] * 3);
			dictionary2.Add("Real", dictionary["Real"]);
			dictionary2.Add("priceGems", num3);
			dictionary2.Add("priceCoins", num4);
			dictionary2.Add("id", value2);
			if (dictionary.ContainsKey("C"))
			{
				dictionary2.Add("Coins", dictionary["C"]);
			}
			if (dictionary.ContainsKey("G"))
			{
				dictionary2.Add("GemsCurrency", dictionary["G"]);
			}
			if (dictionary.ContainsKey("Type"))
			{
				dictionary2.Add("Type", dictionary["Type"]);
			}
			if (dictionary.ContainsKey("Packs"))
			{
				dictionary2.Add("Pack", dictionary["Packs"]);
			}
			if (dictionary.ContainsKey("Count"))
			{
				dictionary2.Add("Count", dictionary["Count"]);
			}
			if (dictionary.ContainsKey("Profit"))
			{
				dictionary2.Add("Profit", dictionary["Profit"]);
			}
			if (dictionary.ContainsKey("Ids"))
			{
				List<object> list2 = Json.Deserialize(Convert.ToString(dictionary["Ids"])) as List<object>;
				if (list2 != null)
				{
					dictionary2.Add("Ids", list2);
				}
			}
			if (dictionary.ContainsKey("AddBonus"))
			{
				string text2 = dictionary["AddBonus"].ToString();
				int num5 = Convert.ToInt32(text2.Substring(1));
				string value3 = ((!text2.Substring(0, 1).Equals("C")) ? "GemsCurrency" : "Coins");
				dictionary2.Add("AddBonusCount", num5);
				dictionary2.Add("AddBonusCurrency", value3);
			}
			inappsBonus.Add(dictionary2);
		}
		return true;
	}

	private static void ParseAwardConfig(object obj)
	{
		List<object> list = obj as List<object>;
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
			string text = Convert.ToString(dictionary["Mode"]);
			int[] array = new int[10];
			int num = 5;
			if (dictionary.ContainsKey("Sc"))
			{
				num = Convert.ToInt32(dictionary["Sc"]);
			}
			for (int j = 1; j <= 10; j++)
			{
				if (dictionary.ContainsKey(j.ToString()))
				{
					array[j - 1] = Convert.ToInt32(dictionary[j.ToString()]);
				}
			}
			if (text.Equals("XP Team"))
			{
				AdminSettingsController.expAvardTeamFight[0] = array;
				AdminSettingsController.minScoreTeamFight = num;
			}
			if (text.Equals("Coins Team"))
			{
				AdminSettingsController.coinAvardTeamFight[0] = array;
				AdminSettingsController.minScoreTeamFight = num;
			}
			if (text.Equals("XP DM"))
			{
				AdminSettingsController.expAvardDeathMath[0] = array;
				AdminSettingsController.minScoreDeathMath = num;
			}
			if (text.Equals("Coins DM"))
			{
				AdminSettingsController.coinAvardDeathMath[0] = array;
				AdminSettingsController.minScoreDeathMath = num;
			}
			if (text.Equals("XP Coop"))
			{
				AdminSettingsController.expAvardTimeBattle = array;
				AdminSettingsController.minScoreTimeBattle = num;
			}
			if (text.Equals("Coins Coop"))
			{
				AdminSettingsController.coinAvardTimeBattle = array;
				AdminSettingsController.minScoreTimeBattle = num;
			}
			if (text.Equals("XP Flag"))
			{
				AdminSettingsController.expAvardFlagCapture[0] = array;
				AdminSettingsController.minScoreFlagCapture = num;
			}
			if (text.Equals("Coins Flag"))
			{
				AdminSettingsController.coinAvardFlagCapture[0] = array;
				AdminSettingsController.minScoreFlagCapture = num;
			}
			if (text.Equals("XP Deadly"))
			{
				AdminSettingsController.expAvardDeadlyGames = array;
			}
			if (text.Equals("Coins Deadly"))
			{
				AdminSettingsController.coinAvardDeadlyGames = array;
			}
			if (text.Equals("XP Points"))
			{
				AdminSettingsController.expAvardCapturePoint[0] = array;
				AdminSettingsController.minScoreCapturePoint = num;
			}
			if (text.Equals("Coins Points"))
			{
				AdminSettingsController.coinAvardCapturePoint[0] = array;
				AdminSettingsController.minScoreCapturePoint = num;
			}
			if (text.Equals("XP Duels"))
			{
				AdminSettingsController.expAvardDuel = array;
				AdminSettingsController.minScoreDuel = num;
			}
			if (text.Equals("Coins Duels"))
			{
				AdminSettingsController.coinAvardDuel = array;
				AdminSettingsController.minScoreDuel = num;
			}
		}
	}

	private static void ParseSpecialEventsConfig(object obj)
	{
		List<object> list = obj as List<object>;
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
			string text = Convert.ToString(dictionary["Event"]);
			if (text.Equals("StartCapital"))
			{
				if (Convert.ToBoolean(dictionary["Enable"]))
				{
					int num = Convert.ToInt32(dictionary["Coins"]);
					int num2 = Convert.ToInt32(dictionary["Gems"]);
					startCapitalCoins = num;
					startCapitalGems = num2;
					startCapitalEnabled = true;
				}
				else
				{
					startCapitalEnabled = false;
				}
			}
			if (!text.Equals("СompetitionAward"))
			{
				continue;
			}
			if (Convert.ToBoolean(dictionary["Enable"]))
			{
				int num3 = Convert.ToInt32(dictionary["Coins"]);
				int num4 = Convert.ToInt32(dictionary["Gems"]);
				countPlaceAwardInCompetion = Convert.ToInt32(dictionary["XP"]);
				if (num3 > 0)
				{
					competitionAward = new ItemPrice(num3, "Coins");
				}
				else if (num4 > 0)
				{
					competitionAward = new ItemPrice(num4, "GemsCurrency");
				}
				else
				{
					competitionAward = new ItemPrice(0, "Coins");
				}
			}
			else
			{
				competitionAward = new ItemPrice(0, "Coins");
				countPlaceAwardInCompetion = 0;
			}
		}
	}

	public static string GetCurrenceCurrentInnapBonus()
	{
		string result = string.Empty;
		List<Dictionary<string, object>> currentInnapBonus = GetCurrentInnapBonus();
		bool flag = false;
		bool flag2 = false;
		if (currentInnapBonus != null)
		{
			foreach (Dictionary<string, object> item in currentInnapBonus)
			{
				if (item.ContainsKey("isGems"))
				{
					if (Convert.ToBoolean(item["isGems"]))
					{
						flag = true;
					}
					else
					{
						flag2 = true;
					}
				}
			}
		}
		if (flag)
		{
			result = "GemsCurrency";
		}
		else if (flag2)
		{
			result = "Coins";
		}
		return result;
	}

	public static bool isActiveInnapBonus()
	{
		if (!TrainingController.TrainingCompleted || ExperienceController.sharedController.currentLevel < 2 || FriendsController.ServerTime < 1)
		{
			return false;
		}
		foreach (Dictionary<string, object> inappsBonu in inappsBonus)
		{
			if (inappsBonu.ContainsKey("Start") && keysInappBonusActionGiven.Contains(Convert.ToString(inappsBonu["Start"])))
			{
				continue;
			}
			DateTime dateTime = DateTime.MinValue;
			DateTime dateTime2 = DateTime.MinValue;
			DateTime currentTimeByUnixTime = Tools.GetCurrentTimeByUnixTime(FriendsController.ServerTime);
			if (inappsBonu.ContainsKey("Start") && inappsBonu.ContainsKey("End"))
			{
				dateTime = Convert.ToDateTime(inappsBonu["Start"], CultureInfo.InvariantCulture);
				dateTime2 = Convert.ToDateTime(inappsBonu["End"], CultureInfo.InvariantCulture);
			}
			if (!(dateTime <= currentTimeByUnixTime) || !(currentTimeByUnixTime <= dateTime2))
			{
				continue;
			}
			if (inappsBonu.ContainsKey("action") && Convert.ToString(inappsBonu["action"]) == petActionName)
			{
				string value = string.Empty;
				if (inappsBonu.ContainsKey("Ids"))
				{
					value = GetCurentPetID(inappsBonu["Start"].ToString(), inappsBonu["Ids"] as List<object>);
				}
				if (string.IsNullOrEmpty(value))
				{
					continue;
				}
			}
			if (inappsBonu.ContainsKey("action") && Convert.ToString(inappsBonu["action"]) == weaponActionName)
			{
				string value2 = string.Empty;
				if (inappsBonu.ContainsKey("Ids"))
				{
					value2 = GetCurentWeaponID(inappsBonu["Start"].ToString(), inappsBonu["Ids"] as List<object>);
				}
				if (string.IsNullOrEmpty(value2))
				{
					continue;
				}
			}
			if (inappsBonu.ContainsKey("action") && Convert.ToString(inappsBonu["action"]) == gadgetActionName)
			{
				List<string> list = GetCurentGadgetesIDs(inappsBonu["Start"].ToString(), inappsBonu["Ids"] as List<object>);
				if (list == null)
				{
					continue;
				}
			}
			return true;
		}
		return false;
	}

	private static string GetCurentPetID(string _key, List<object> _ids)
	{
		string @string = Storager.getString(Defs.keyInappBonusStartActionForPresentIDPetkey, false);
		if (@string == _key)
		{
			return Storager.getString(Defs.keyInappPresentIDPetkey, false);
		}
		List<string> list = new List<string>();
		for (int i = 0; i < _ids.Count; i++)
		{
			list.Add(_ids[i].ToString());
		}
		string firstSmallestUpPet = Singleton<PetsManager>.Instance.GetFirstSmallestUpPet(list);
		if (!string.IsNullOrEmpty(firstSmallestUpPet))
		{
			Storager.setString(Defs.keyInappPresentIDPetkey, firstSmallestUpPet, false);
			Storager.setString(Defs.keyInappBonusStartActionForPresentIDPetkey, _key, false);
		}
		return firstSmallestUpPet;
	}

	private static bool isWeaponAvalibleForBonus(string weaponTag)
	{
		ItemRecord byTag = ItemDb.GetByTag(weaponTag);
		if (byTag == null || byTag.StorageId == null)
		{
			return false;
		}
		return Storager.getInt(byTag.StorageId, true) <= 0;
	}

	private static bool isGadgetAvalibleForBonus(string gadgetTag)
	{
		if (!GadgetsInfo.info.ContainsKey(gadgetTag))
		{
			return false;
		}
		return !GadgetsInfo.IsBought(gadgetTag);
	}

	private static string GetCurentWeaponID(string _key, List<object> _ids)
	{
		string @string = Storager.getString(Defs.keyInappBonusStartActionForPresentIDWeaponRedkey, false);
		if (@string == _key)
		{
			string string2 = Storager.getString(Defs.keyInappPresentIDWeaponRedkey, false);
			if (isWeaponAvalibleForBonus(string2))
			{
				return string2;
			}
		}
		string text = null;
		int num = ExpController.OurTierForAnyPlace();
		int num2 = num;
		while (num2 >= num - 1 && num2 >= 0)
		{
			List<object> list = _ids[num2] as List<object>;
			for (int i = 0; i < list.Count; i++)
			{
				if (isWeaponAvalibleForBonus(list[i].ToString()))
				{
					text = list[i].ToString();
					break;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				break;
			}
			num2--;
		}
		if (!string.IsNullOrEmpty(text))
		{
			Storager.setString(Defs.keyInappPresentIDWeaponRedkey, text, false);
			Storager.setString(Defs.keyInappBonusStartActionForPresentIDWeaponRedkey, _key, false);
		}
		return text;
	}

	private static bool isAvalibleListGadgetes(List<string> _ids)
	{
		if (_ids != null)
		{
			for (int i = 0; i < _ids.Count; i++)
			{
				if (GadgetsInfo.IsBought(_ids[i]))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private static List<string> GetCurentGadgetesIDs(string _key, List<object> _ids)
	{
		string @string = Storager.getString(Defs.keyInappBonusStartActionForPresentIDGadgetkey, false);
		if (@string == _key)
		{
			List<string> list = curentGadgetesIDs;
			if (isAvalibleListGadgetes(list))
			{
				return list;
			}
		}
		List<string> list2 = new List<string>();
		int num = ExpController.OurTierForAnyPlace();
		for (int i = 0; i < _ids.Count; i++)
		{
			List<object> list3 = _ids[i] as List<object>;
			string text = null;
			for (int num2 = num; num2 >= 0; num2--)
			{
				List<object> list4 = list3[num2] as List<object>;
				for (int j = 0; j < list4.Count; j++)
				{
					if (!GadgetsInfo.IsBought(list4[j].ToString()))
					{
						text = list4[j].ToString();
						break;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					list2.Add(text);
					break;
				}
			}
		}
		if (list2.Count == 3)
		{
			curentGadgetesIDs = list2;
			Storager.setString(Defs.keyInappBonusStartActionForPresentIDGadgetkey, _key, false);
			return list2;
		}
		return null;
	}

	public static void AddKeysInappBonusActionGiven(string _key)
	{
		if (!keysInappBonusActionGiven.Contains(_key))
		{
			keysInappBonusActionGiven.Add(_key);
			Storager.setString(Defs.keysInappBonusGivenkey, Json.Serialize(keysInappBonusActionGiven), false);
		}
	}

	public static List<Dictionary<string, object>> GetCurrentInnapBonus()
	{
		if (!TrainingController.TrainingCompleted || ExperienceController.sharedController.currentLevel < 2 || FriendsController.ServerTime < 1)
		{
			return null;
		}
		if (countFrameInCache == Time.frameCount)
		{
			return cacheCurrentInnapBonus;
		}
		List<Dictionary<string, object>> list = null;
		foreach (Dictionary<string, object> inappsBonu in inappsBonus)
		{
			string text = string.Empty;
			if (inappsBonu.ContainsKey("Start") && inappsBonu.ContainsKey("id"))
			{
				text = inappsBonu["id"].ToString() + inappsBonu["Start"].ToString();
			}
			if (!inappsBonu.ContainsKey("Type") || keysInappBonusActionGiven.Contains(text))
			{
				continue;
			}
			Dictionary<string, object> dictionary = null;
			DateTime currentTimeByUnixTime = Tools.GetCurrentTimeByUnixTime(FriendsController.ServerTime);
			object obj = inappsBonu["Start"];
			object value = inappsBonu["End"];
			DateTime dateTime = Convert.ToDateTime(obj, CultureInfo.InvariantCulture);
			DateTime dateTime2 = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
			if (!(dateTime <= currentTimeByUnixTime) || !(currentTimeByUnixTime <= dateTime2))
			{
				continue;
			}
			string text2 = inappsBonu["Type"].ToString();
			bool flag = text2 == "packs";
			float num = Convert.ToSingle(inappsBonu["priceGems"]);
			float num2 = Convert.ToSingle(inappsBonu["priceCoins"]);
			int num3 = Convert.ToInt32(inappsBonu["Real"]);
			float num4 = -1f * (float)num3;
			dictionary = new Dictionary<string, object>();
			string text3 = Convert.ToString(inappsBonu["action"]);
			dictionary.Add("action", text3);
			dictionary.Add("Key", text);
			dictionary.Add("End", Mathf.RoundToInt((float)dateTime2.Subtract(currentTimeByUnixTime).TotalSeconds));
			dictionary.Add("ID", inappsBonu["id"]);
			object value2;
			if (inappsBonu.TryGetValue("Coins", out value2))
			{
				int num5 = Convert.ToInt32(value2);
				dictionary.Add("Coins", value2);
				num4 += (float)num5 * num2;
			}
			object value3;
			if (inappsBonu.TryGetValue("GemsCurrency", out value3))
			{
				int num6 = Convert.ToInt32(value3);
				dictionary.Add("GemsCurrency", value3);
				num4 += (float)num6 * num2;
			}
			if (flag)
			{
				int fullPack = Convert.ToInt32(inappsBonu["Pack"]);
				dictionary.Add("Pack", GetCurrentPack(text, dateTime, currentTimeByUnixTime, dateTime2, fullPack));
			}
			dictionary.Add("Type", text2);
			object value4 = null;
			if (inappsBonu.TryGetValue("Count", out value4))
			{
				dictionary.Add("Count", value4);
			}
			dictionary.Add("isGems", inappsBonu["isGems"]);
			if (text3 == curencyActionName)
			{
				float num7 = num4 / (float)num3;
				dictionary.Add("Profit", num7);
			}
			else if (text3 == petActionName)
			{
				string value5 = string.Empty;
				if (inappsBonu.ContainsKey("Ids"))
				{
					value5 = GetCurentPetID(obj.ToString(), inappsBonu["Ids"] as List<object>);
				}
				int result = 0;
				if (!inappsBonu.ContainsKey("Count") || int.TryParse(value4.ToString(), out result))
				{
				}
				if (string.IsNullOrEmpty(value5) || result == 0)
				{
					continue;
				}
				dictionary.Add("Pet", value5);
				dictionary.Add("Quantity", result);
			}
			else if (text3 == weaponActionName)
			{
				string text4 = string.Empty;
				if (inappsBonu.ContainsKey("Ids"))
				{
					text4 = GetCurentWeaponID(obj.ToString(), inappsBonu["Ids"] as List<object>);
				}
				if (string.IsNullOrEmpty(text4))
				{
					continue;
				}
				ItemPrice itemPrice = ShopNGUIController.GetItemPrice(text4, ShopNGUIController.CategoryNames.SpecilCategory);
				num4 = ((!(itemPrice.Currency == "Coins")) ? (num4 + (float)itemPrice.Price * num) : (num4 + (float)itemPrice.Price * num2));
				float num8 = num4 / (float)num3;
				dictionary.Add("Profit", num8);
				dictionary.Add("Weapon", text4);
			}
			else if (text3 == leprechaunActionName)
			{
				int num9 = Convert.ToInt32(inappsBonu["AddBonusCount"]) * Convert.ToInt32(value4);
				num4 = ((!(Convert.ToString(inappsBonu["AddBonusCurrency"]) == "Coins")) ? (num4 + (float)num9 * num) : (num4 + (float)num9 * num2));
				float num10 = num4 / (float)num3;
				dictionary.Add("Profit", num10);
				dictionary.Add("CurrencyLeprechaun", inappsBonu["AddBonusCurrency"]);
				dictionary.Add("PerDayLeprechaun", inappsBonu["AddBonusCount"]);
				dictionary.Add("DaysLeprechaun", value4);
			}
			else if (text3 == gadgetActionName)
			{
				List<string> list2 = GetCurentGadgetesIDs(obj.ToString(), inappsBonu["Ids"] as List<object>);
				if (list2 == null)
				{
					continue;
				}
				for (int i = 0; i < list2.Count; i++)
				{
					ItemPrice itemPrice2 = ShopNGUIController.GetItemPrice(list2[i], ShopNGUIController.CategoryNames.SupportCategory);
					num4 = ((!(itemPrice2.Currency == "Coins")) ? (num4 + (float)itemPrice2.Price * num) : (num4 + (float)itemPrice2.Price * num2));
				}
				float num11 = num4 / (float)num3;
				dictionary.Add("Profit", num11);
				dictionary.Add("Gadgets", list2);
			}
			if (list == null)
			{
				list = new List<Dictionary<string, object>>();
			}
			list.Add(dictionary);
		}
		cacheCurrentInnapBonus = list;
		countFrameInCache = Time.frameCount;
		return list;
	}

	private static int GetCurrentPack(string keyAction, DateTime start, DateTime now, DateTime end, int fullPack)
	{
		if (end.Subtract(now).TotalSeconds == 0.0)
		{
			return 0;
		}
		if (!timeNextUpdateDict.ContainsKey(keyAction))
		{
			timeNextUpdateDict.Add(keyAction, DateTime.MinValue);
		}
		if (!curPackDict.ContainsKey(keyAction))
		{
			curPackDict.Add(keyAction, -1);
		}
		DateTime dateTime = timeNextUpdateDict[keyAction];
		int num = curPackDict[keyAction];
		if (num == -1 || now.Subtract(dateTime).TotalSeconds > 15.0)
		{
			num = CurPackForDate(start, now, end, fullPack);
			timeNextUpdateDict[keyAction] = now + TimeSpan.FromSeconds(UnityEngine.Random.Range(3, 7));
			curPackDict[keyAction] = num;
			return num;
		}
		if (now > dateTime)
		{
			int num2 = UnityEngine.Random.Range(3, 7);
			DateTime now2 = now + TimeSpan.FromSeconds(num2);
			int num3 = CurPackForDate(start, now2, end, fullPack);
			if (num > num3)
			{
				int num4 = UnityEngine.Random.Range(0, num - num3);
				num -= num4;
			}
			else
			{
				num = num3;
			}
			if (num < 1)
			{
				num = 1;
			}
			curPackDict[keyAction] = num;
			timeNextUpdateDict[keyAction] = now + TimeSpan.FromSeconds(num2);
		}
		return num;
	}

	private static int CurPackForDate(DateTime start, DateTime now, DateTime end, int fullPack)
	{
		double totalSeconds = end.Subtract(now).TotalSeconds;
		double totalSeconds2 = end.Subtract(start).TotalSeconds;
		double num = totalSeconds / totalSeconds2;
		return (int)(num * (double)fullPack);
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			Invoke("UpdateBalansFromServer", 1f);
		}
	}

	private void OnDestroy()
	{
		sharedController = null;
	}
}

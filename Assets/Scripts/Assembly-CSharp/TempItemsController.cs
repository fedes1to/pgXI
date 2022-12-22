using System;
using System.Collections;
using System.Collections.Generic;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class TempItemsController : MonoBehaviour
{
	private const long _salt = 1002855644958404316L;

	private const string DurationKey = "Duration";

	private const string StartKey = "Start";

	private const string ExpiredItemsKey = "ExpiredITemptemsControllerKey";

	public static TempItemsController sharedController;

	public List<string> ExpiredItems = new List<string>();

	public static Dictionary<string, List<float>> PriceCoefs;

	public static Dictionary<string, string> GunsMappingFromTempToConst;

	private Dictionary<string, Dictionary<string, SaltedLong>> Items = new Dictionary<string, Dictionary<string, SaltedLong>>();

	private static List<int> rentTms;

	static TempItemsController()
	{
		PriceCoefs = new Dictionary<string, List<float>>
		{
			{
				WeaponTags.Assault_Machine_Gun_Tag,
				new List<float> { 1f, 2f, 4f }
			},
			{
				WeaponTags.Impulse_Sniper_Rifle_Tag,
				new List<float> { 1f, 2.3333333f, 3.6666667f }
			},
			{
				"Armor_Adamant_3",
				new List<float> { 1f, 2.6666667f, 5.3333335f }
			},
			{
				"hat_Adamant_3",
				new List<float> { 1f, 2.6666667f, 5.3333335f }
			},
			{
				WeaponTags.RailRevolver_1_Tag,
				new List<float> { 1f, 2f, 4f }
			},
			{
				WeaponTags.Autoaim_Rocketlauncher_Tag,
				new List<float> { 1f, 2f, 3.125f }
			},
			{
				WeaponTags.TwoBoltersRent_Tag,
				new List<float> { 1f, 2f, 3.125f }
			},
			{
				WeaponTags.Red_StoneRent_Tag,
				new List<float> { 1f, 2f, 3.125f }
			},
			{
				WeaponTags.DragonGunRent_Tag,
				new List<float> { 1f, 2f, 3.125f }
			},
			{
				WeaponTags.PumpkinGunRent_Tag,
				new List<float> { 1f, 2f, 3.125f }
			},
			{
				WeaponTags.RayMinigunRent_Tag,
				new List<float> { 1f, 2f, 3.125f }
			}
		};
		GunsMappingFromTempToConst = new Dictionary<string, string>();
		rentTms = null;
		GunsMappingFromTempToConst.Add(WeaponTags.Assault_Machine_Gun_Tag, WeaponTags.Assault_Machine_GunBuy_Tag);
		GunsMappingFromTempToConst.Add(WeaponTags.Impulse_Sniper_Rifle_Tag, WeaponTags.Impulse_Sniper_RifleBuy_Tag);
		GunsMappingFromTempToConst.Add(WeaponTags.RailRevolver_1_Tag, WeaponTags.RailRevolverBuy_Tag);
		GunsMappingFromTempToConst.Add(WeaponTags.Autoaim_Rocketlauncher_Tag, WeaponTags.Autoaim_RocketlauncherBuy_Tag);
	}

	public static int RentIndexFromDays(int days)
	{
		int result = 0;
		switch (days)
		{
		case 1:
			result = 0;
			break;
		case 2:
			result = 3;
			break;
		case 3:
			result = 1;
			break;
		case 5:
			result = 4;
			break;
		case 7:
			result = 2;
			break;
		}
		return result;
	}

	public static bool IsCategoryContainsTempItems(ShopNGUIController.CategoryNames cat)
	{
		return ShopNGUIController.IsWeaponCategory(cat) || cat == ShopNGUIController.CategoryNames.ArmorCategory || cat == ShopNGUIController.CategoryNames.HatsCategory;
	}

	public void AddTemporaryItem(string tg, int tm)
	{
		AddTimeForItem(tg, (tm >= 0) ? tm : 0);
	}

	public static int RentTimeForIndex(int timeForRentIndex)
	{
		if (rentTms == null)
		{
			List<int> list = new List<int>();
			list.Add(86400);
			list.Add(259200);
			list.Add(604800);
			list.Add(172800);
			list.Add(432000);
			rentTms = list;
		}
		int result = 86400;
		if (timeForRentIndex < rentTms.Count && timeForRentIndex >= 0)
		{
			result = rentTms[timeForRentIndex];
		}
		return result;
	}

	public bool CanShowExpiredBannerForTag(string tg)
	{
		return false;
	}

	public long TimeRemainingForItems(string tg)
	{
		return 0L;
	}

	public string TimeRemainingForItemString(string tg)
	{
		return RiliExtensions.GetTimeStringDays(TimeRemainingForItems(tg));
	}

	public void AddTimeForItem(string item, int time)
	{
	}

	public bool ContainsItem(string item)
	{
		return false;
	}

	private static void PrepareKeyForItemsJson()
	{
		if (!Storager.hasKey(Defs.TempItemsDictionaryKey))
		{
			Storager.setString(Defs.TempItemsDictionaryKey, "{}", false);
		}
	}

	private static bool ItemIsArmorOrHat(string tg)
	{
		int itemCategory = ItemDb.GetItemCategory(tg);
		int result;
		switch (itemCategory)
		{
		default:
			result = ((itemCategory == 6) ? 1 : 0);
			break;
		case 7:
			result = 1;
			break;
		case -1:
			result = 0;
			break;
		}
		return (byte)result != 0;
	}

	private void Awake()
	{
		sharedController = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		DeserializeItems();
		DeserializeExpiredObjects();
		CheckForTimeHack();
		RemoveExpiredItems();
	}

	private void Start()
	{
		StartCoroutine(Step());
	}

	private void RemoveExpiredItems()
	{
	}//Discarded unreachable code: IL_0001


	private void RemoveTemporaryItem(string key)
	{
		if (ItemIsArmorOrHat(key))
		{
			Wear.RemoveTemporaryWear(key);
		}
		else
		{
			WeaponManager.sharedManager.RemoveTemporaryItem(key);
		}
	}

	private IEnumerator Step()
	{
		yield break;
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

	private void CheckForTimeHack()
	{
	}//Discarded unreachable code: IL_0001


	private static Dictionary<string, Dictionary<string, SaltedLong>> ToSaltedDictionary(Dictionary<string, Dictionary<string, long>> normalDict)
	{
		if (normalDict == null)
		{
			return null;
		}
		Dictionary<string, Dictionary<string, SaltedLong>> dictionary = new Dictionary<string, Dictionary<string, SaltedLong>>();
		foreach (KeyValuePair<string, Dictionary<string, long>> item in normalDict)
		{
			Dictionary<string, SaltedLong> dictionary2 = new Dictionary<string, SaltedLong>();
			if (item.Value != null)
			{
				foreach (KeyValuePair<string, long> item2 in item.Value)
				{
					if (item2.Key != null)
					{
						dictionary2.Add(item2.Key, new SaltedLong(1002855644958404316L, item2.Value));
					}
				}
			}
			dictionary.Add(item.Key, dictionary2);
		}
		return dictionary;
	}

	private static Dictionary<string, Dictionary<string, long>> ToNormalDictionary(Dictionary<string, Dictionary<string, SaltedLong>> saltedDict_)
	{
		if (saltedDict_ == null)
		{
			return null;
		}
		Dictionary<string, Dictionary<string, long>> dictionary = new Dictionary<string, Dictionary<string, long>>();
		foreach (KeyValuePair<string, Dictionary<string, SaltedLong>> item in saltedDict_)
		{
			Dictionary<string, long> dictionary2 = new Dictionary<string, long>();
			if (item.Value != null)
			{
				foreach (KeyValuePair<string, SaltedLong> item2 in item.Value)
				{
					if (item2.Key != null)
					{
						dictionary2.Add(item2.Key, item2.Value.Value);
					}
				}
			}
			dictionary.Add(item.Key, dictionary2);
		}
		return dictionary;
	}

	private void DeserializeItems()
	{
		//Discarded unreachable code: IL_0100, IL_015e
		PrepareKeyForItemsJson();
		object obj = Json.Deserialize(Storager.getString(Defs.TempItemsDictionaryKey, false));
		if (obj == null)
		{
			Debug.LogWarning("Error Deserializing temp items JSON");
			return;
		}
		Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
		if (dictionary == null)
		{
			Debug.LogWarning("Error casting to dict in deserializing temp items JSON");
			return;
		}
		Dictionary<string, Dictionary<string, long>> dictionary2 = new Dictionary<string, Dictionary<string, long>>();
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			if (item.Value == null)
			{
				Debug.LogWarning("Error kvp.Value == null kvp.Key = " + item.Key + " in deserializing temp items JSON");
				continue;
			}
			Dictionary<string, object> dictionary3 = item.Value as Dictionary<string, object>;
			object value;
			if (dictionary3 == null)
			{
				Debug.LogWarning("Error innerDict == null kvp.Key = " + item.Key + " in deserializing temp items JSON");
			}
			else if (dictionary3.TryGetValue("Duration", out value) && value != null)
			{
				long value2;
				try
				{
					value2 = (long)value;
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Error unboxing DurationValue in deserializing temp items JSON: " + ex.Message);
					continue;
				}
				object value3;
				if (dictionary3.TryGetValue("Start", out value3) && value3 != null)
				{
					long value4;
					try
					{
						value4 = (long)value3;
					}
					catch (Exception ex2)
					{
						Debug.LogWarning("Error unboxing StartValue in deserializing temp items JSON: " + ex2.Message);
						continue;
					}
					dictionary2.Add(item.Key, new Dictionary<string, long>
					{
						{ "Start", value4 },
						{ "Duration", value2 }
					});
				}
				else
				{
					Debug.LogWarning(" ! (innerDict.TryGetValue(StartKey,out StartValueObj) && StartValueObj != null) in deserializing temp items JSON");
				}
			}
			else
			{
				Debug.LogWarning(" ! (innerDict.TryGetValue(DurationKey,out DurationValueObj) && DurationValueObj != null) in deserializing temp items JSON");
			}
		}
		Items = ToSaltedDictionary(dictionary2);
	}

	private void SerializeItems()
	{
		Dictionary<string, Dictionary<string, long>> obj = ToNormalDictionary(Items ?? new Dictionary<string, Dictionary<string, SaltedLong>>());
		Storager.setString(Defs.TempItemsDictionaryKey, Json.Serialize(obj), false);
	}

	private void DeserializeExpiredObjects()
	{
		//Discarded unreachable code: IL_00c7
		if (!Storager.hasKey("ExpiredITemptemsControllerKey"))
		{
			Storager.setString("ExpiredITemptemsControllerKey", "[]", false);
		}
		string @string = Storager.getString("ExpiredITemptemsControllerKey", false);
		object obj = Json.Deserialize(@string);
		if (obj == null)
		{
			Debug.LogWarning("Error Deserializing expired items JSON");
			return;
		}
		List<object> list = obj as List<object>;
		if (list == null)
		{
			Debug.LogWarning("Error casting expired items obj to list");
			return;
		}
		try
		{
			ExpiredItems.Clear();
			foreach (string item in list)
			{
				ExpiredItems.Add(item);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Exception when iterating expired items list: " + ex);
		}
	}

	private void SerializeExpiredItems()
	{
		Storager.setString("ExpiredITemptemsControllerKey", Json.Serialize(ExpiredItems), false);
	}

	private static long GetLastSuspendTime()
	{
		return PromoActionsManager.GetUnixTimeFromStorage(Defs.LastTimeTempItemsSuspended);
	}

	private static void SaveSuspendTime()
	{
		Storager.setString(Defs.LastTimeTempItemsSuspended, PromoActionsManager.CurrentUnixTime.ToString(), false);
	}

	private void OnDestroy()
	{
	}

	public void TakeTemporaryItemToPlayer(ShopNGUIController.CategoryNames categoryName, string tag, int indexTimeLife)
	{
		ExpiredItems.Remove(tag);
	}
}

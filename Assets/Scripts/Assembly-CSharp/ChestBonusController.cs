using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class ChestBonusController : MonoBehaviour
{
	public delegate void OnChestBonusEnabledDelegate();

	private const float BonusUpdateTimeout = 870f;

	public static bool chestBonusesObtainedOnceInCurrentRun;

	private ChestBonusesData _bonusesData;

	private float _lastCheckEventTime;

	private bool _lastBonusActive;

	private DateTime _timeStartBonus;

	private DateTime _timeEndBonus;

	private bool _isGetBonusInfoRunning;

	private float _eventGetBonusInfoStartTime;

	public static ChestBonusController Get { get; private set; }

	public bool IsBonusActive { get; private set; }

	public static event OnChestBonusEnabledDelegate OnChestBonusChange;

	private void Start()
	{
		Get = this;
		_bonusesData = new ChestBonusesData();
		_timeStartBonus = default(DateTime);
		_timeEndBonus = default(DateTime);
		Task firstResponse = PersistentCacheManager.Instance.FirstResponse;
		StartCoroutine(GetEventBonusInfoLoop(firstResponse));
	}

	private void OnDestroy()
	{
		Get = null;
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			StartCoroutine(DownloadDataAboutBonuses());
		}
	}

	private IEnumerator GetEventBonusInfoLoop(Task futureToWait)
	{
		yield return new WaitUntil(() => futureToWait.IsCompleted);
		while (true)
		{
			yield return StartCoroutine(DownloadDataAboutBonuses());
			while (Time.realtimeSinceStartup - _eventGetBonusInfoStartTime < 870f)
			{
				yield return null;
			}
		}
	}

	private void Update()
	{
		if (IsBonusActive && Time.realtimeSinceStartup - _lastCheckEventTime >= 1f)
		{
			IsBonusActive = IsBonusActivate();
			if (_lastBonusActive != IsBonusActive && ChestBonusController.OnChestBonusChange != null)
			{
				ChestBonusController.OnChestBonusChange();
				_lastBonusActive = IsBonusActive;
			}
			_lastCheckEventTime = Time.realtimeSinceStartup;
		}
	}

	private IEnumerator DownloadDataAboutBonuses()
	{
		if (_isGetBonusInfoRunning)
		{
			yield break;
		}
		_eventGetBonusInfoStartTime = Time.realtimeSinceStartup;
		_isGetBonusInfoRunning = true;
		string bonusesDataAddress = ChestBonusModel.GetUrlForDownloadBonusesData();
		string cachedResponse = PersistentCacheManager.Instance.GetValue(bonusesDataAddress);
		string responseText;
		if (!string.IsNullOrEmpty(cachedResponse))
		{
			responseText = cachedResponse;
		}
		else
		{
			WWW downloadData = Tools.CreateWwwIfNotConnected(bonusesDataAddress);
			if (downloadData == null)
			{
				_isGetBonusInfoRunning = false;
				yield break;
			}
			yield return downloadData;
			if (!string.IsNullOrEmpty(downloadData.error))
			{
				Debug.LogError("DownloadDataAboutBonuses error: " + downloadData.error);
				_bonusesData.Clear();
				_isGetBonusInfoRunning = false;
				yield break;
			}
			responseText = URLs.Sanitize(downloadData);
			PersistentCacheManager.Instance.SetValue(bonusesDataAddress, responseText);
		}
		Dictionary<string, object> bonusesData = Json.Deserialize(responseText) as Dictionary<string, object>;
		if (bonusesData == null)
		{
			Debug.LogWarning("DownloadDataAboutBonuses bonusesData = null");
			_isGetBonusInfoRunning = false;
			yield break;
		}
		chestBonusesObtainedOnceInCurrentRun = true;
		_bonusesData.Clear();
		if (bonusesData.ContainsKey("start"))
		{
			_bonusesData.timeStart = Convert.ToInt32((long)bonusesData["start"]);
		}
		if (bonusesData.ContainsKey("duration"))
		{
			long duration2 = Math.Min(Convert.ToInt64(bonusesData["duration"]), 2147483647L);
			duration2 = Math.Max(duration2, -2147483648L);
			_bonusesData.duration = Convert.ToInt32(duration2);
		}
		if (_bonusesData.timeStart == 0 || _bonusesData.duration == 0)
		{
			_isGetBonusInfoRunning = false;
			yield break;
		}
		if (!bonusesData.ContainsKey("bonuses"))
		{
			_isGetBonusInfoRunning = false;
			yield break;
		}
		List<object> bonusesList = bonusesData["bonuses"] as List<object>;
		if (bonusesList != null)
		{
			_bonusesData.bonuses = new List<ChestBonusData>();
			for (int i = 0; i < bonusesList.Count; i++)
			{
				Dictionary<string, object> bonusElement = bonusesList[i] as Dictionary<string, object>;
				if (bonusElement == null)
				{
					continue;
				}
				ChestBonusData newBonus = new ChestBonusData();
				if (bonusElement.ContainsKey("linkKey"))
				{
					newBonus.linkKey = (string)bonusElement["linkKey"];
				}
				if (bonusElement.ContainsKey("isVisible"))
				{
					int value = Convert.ToInt32((long)bonusElement["isVisible"]);
					newBonus.isVisible = value == 1;
				}
				if (bonusElement.ContainsKey("items"))
				{
					List<object> bonusItemsList = bonusElement["items"] as List<object>;
					if (bonusItemsList != null)
					{
						newBonus.items = new List<ChestBonusItemData>();
						for (int j = 0; j < bonusItemsList.Count; j++)
						{
							Dictionary<string, object> bonusItemData = bonusItemsList[j] as Dictionary<string, object>;
							if (bonusItemData != null)
							{
								ChestBonusItemData newItem = new ChestBonusItemData();
								if (bonusItemData.ContainsKey("tag"))
								{
									newItem.tag = (string)bonusItemData["tag"];
								}
								if (bonusItemData.ContainsKey("count"))
								{
									newItem.count = Convert.ToInt32((long)bonusItemData["count"]);
								}
								if (bonusItemData.ContainsKey("timeLife"))
								{
									newItem.timeLife = Convert.ToInt32((long)bonusItemData["timeLife"]);
								}
								newBonus.items.Add(newItem);
							}
						}
					}
				}
				_bonusesData.bonuses.Add(newBonus);
			}
		}
		_timeStartBonus = StarterPackModel.GetCurrentTimeByUnixTime(_bonusesData.timeStart);
		int timeEnd = _bonusesData.timeStart + _bonusesData.duration;
		_timeEndBonus = StarterPackModel.GetCurrentTimeByUnixTime(timeEnd);
		IsBonusActive = IsBonusActivate();
		if (ChestBonusController.OnChestBonusChange != null)
		{
			ChestBonusController.OnChestBonusChange();
		}
		_isGetBonusInfoRunning = false;
	}

	private bool IsBonusActivate()
	{
		if (_bonusesData.timeStart == 0 || _bonusesData.duration == 0)
		{
			return false;
		}
		DateTime utcNow = DateTime.UtcNow;
		return utcNow >= _timeStartBonus && utcNow <= _timeEndBonus;
	}

	public void ShowBonusWindowForItem(PurchaseEventArgs purchaseInfo)
	{
		ChestBonusData bonusData = GetBonusData(purchaseInfo);
		BankController instance = BankController.Instance;
		if (bonusData != null && instance != null)
		{
			instance.bonusDetailView.Show(bonusData);
		}
	}

	public bool IsBonusActiveForItem(PurchaseEventArgs purchaseInfo)
	{
		if (!IsBonusActive)
		{
			return false;
		}
		ChestBonusData bonusData = GetBonusData(purchaseInfo);
		return bonusData != null && bonusData.isVisible;
	}

	public ChestBonusData GetBonusData(PurchaseEventArgs purchaseInfo)
	{
		bool isGemsPack = purchaseInfo.Currency == "GemsCurrency";
		return GetBonusData(isGemsPack, purchaseInfo.Index);
	}

	private ChestBonusData GetBonusData(bool isGemsPack, int packOrder)
	{
		if (_bonusesData == null || _bonusesData.bonuses == null)
		{
			return null;
		}
		string arg = ((!isGemsPack) ? "coins" : "gems");
		string text = string.Format("{0}_{1}", arg, packOrder + 1);
		for (int i = 0; i < _bonusesData.bonuses.Count; i++)
		{
			ChestBonusData chestBonusData = _bonusesData.bonuses[i];
			if (chestBonusData.linkKey == text)
			{
				return chestBonusData;
			}
		}
		return null;
	}

	public static bool TryTakeChestBonus(bool isGemsPack, int packOrder)
	{
		ChestBonusController get = Get;
		if (get == null)
		{
			return false;
		}
		if (!get.IsBonusActive)
		{
			return false;
		}
		ChestBonusData bonusData = get.GetBonusData(isGemsPack, packOrder);
		if (bonusData == null)
		{
			return false;
		}
		if (bonusData.items == null || bonusData.items.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < bonusData.items.Count; i++)
		{
			ChestBonusItemData chestBonusItemData = bonusData.items[i];
			ShopNGUIController.CategoryNames itemCategory = (ShopNGUIController.CategoryNames)ItemDb.GetItemCategory(chestBonusItemData.tag);
			ShopNGUIController.CategoryNames category = itemCategory;
			string text = chestBonusItemData.tag;
			int count = chestBonusItemData.count;
			int timeLife = chestBonusItemData.timeLife;
			int timeForRentIndexForOldTempWeapons = 0;
			if (timeLife != -1)
			{
				int days = timeLife / 24;
				timeForRentIndexForOldTempWeapons = TempItemsController.RentIndexFromDays(days);
			}
			ShopNGUIController.ProvideItem(category, text, count, false, timeForRentIndexForOldTempWeapons, null, delegate(string wearId)
			{
				if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.weaponsInGame != null)
				{
					if (ShopNGUIController.GuiActive && ShopNGUIController.sharedShop != null)
					{
						int itemCategory2 = ItemDb.GetItemCategory(wearId);
						if (itemCategory2 != -1)
						{
							ShopNGUIController.EquipWearInCategoryIfNotEquiped(wearId, (ShopNGUIController.CategoryNames)itemCategory2, WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null);
						}
					}
					else
					{
						int itemCategory3 = ItemDb.GetItemCategory(wearId);
						if (itemCategory3 != -1)
						{
							ShopNGUIController.SetAsEquippedAndSendToServer(wearId, (ShopNGUIController.CategoryNames)itemCategory3);
							if (ShopNGUIController.IsWearCategory((ShopNGUIController.CategoryNames)itemCategory3))
							{
								ShopNGUIController.SendEquippedWearInCategory(wearId, (ShopNGUIController.CategoryNames)itemCategory3, string.Empty);
							}
						}
					}
				}
			}, true, true, false);
			TempItemsController.sharedController.ExpiredItems.Remove(text);
		}
		return true;
	}
}

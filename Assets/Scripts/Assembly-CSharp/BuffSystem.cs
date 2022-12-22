using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

internal sealed class BuffSystem : MonoBehaviour
{
	private sealed class SituationBuffComparer : IEqualityComparer<SituationBuffType>
	{
		public bool Equals(SituationBuffType x, SituationBuffType y)
		{
			return x == y;
		}

		public int GetHashCode(SituationBuffType obj)
		{
			return (int)obj;
		}
	}

	public class ParamByTier
	{
		public float timeToGetGunLow;

		public float timeToGetGunMiddle;

		public float timeToGetGunHigh;

		public float lowKillRate;

		public float highKillRate;

		public float a;

		public float b;

		public float midbottom;

		public float midtop;

		public float top;

		public float bottom;

		public ParamByTier()
		{
			timeToGetGunLow = 2400f;
			timeToGetGunMiddle = 3600f;
			timeToGetGunHigh = 4800f;
			lowKillRate = 0.5f;
			highKillRate = 1.2f;
			a = 50f;
			b = 50f;
			midbottom = 0.8f;
			midtop = 1.2f;
			top = 2f;
			bottom = 0f;
		}

		public ParamByTier(Dictionary<string, object> dictionary)
		{
			timeToGetGunLow = Convert.ToSingle(dictionary["timeGunLow"]);
			timeToGetGunMiddle = Convert.ToSingle(dictionary["timeGunMiddle"]);
			timeToGetGunHigh = Convert.ToSingle(dictionary["timeGunHigh"]);
			lowKillRate = Convert.ToSingle(dictionary["lowKillRate"]);
			highKillRate = Convert.ToSingle(dictionary["highKillRate"]);
			a = Convert.ToSingle(dictionary["form_a"]);
			b = Convert.ToSingle(dictionary["form_b"]);
			midbottom = Convert.ToSingle(dictionary["form_midbottom"]);
			midtop = Convert.ToSingle(dictionary["form_midtop"]);
			top = Convert.ToSingle(dictionary["form_top"]);
			if (dictionary.ContainsKey("form_bottom"))
			{
				bottom = Convert.ToSingle(dictionary["form_bottom"]);
			}
		}
	}

	private class BuffParameter
	{
		public int priority;

		public SituationBuffType type;

		public float healthBuff;

		public float damageBuff;

		public float time;

		public float timeForPurchase;

		public BuffParameter(SituationBuffType type, float healthBuff, float damageBuff, float time, int priority)
		{
			this.type = type;
			this.healthBuff = healthBuff;
			this.damageBuff = damageBuff;
			this.priority = priority;
			this.time = time;
			timeForPurchase = 1800f;
		}

		public BuffParameter(Dictionary<string, object> dictionary)
		{
			type = (SituationBuffType)(int)Enum.Parse(typeof(SituationBuffType), Convert.ToString(dictionary["type"]));
			if (dictionary.ContainsKey("health"))
			{
				healthBuff = Convert.ToSingle(dictionary["health"]);
			}
			else
			{
				healthBuff = 1f;
			}
			if (dictionary.ContainsKey("damage"))
			{
				damageBuff = Convert.ToSingle(dictionary["damage"]);
			}
			else
			{
				damageBuff = 1f;
			}
			if (dictionary.ContainsKey("timeToBuy"))
			{
				timeForPurchase = Convert.ToSingle(dictionary["timeToBuy"]);
			}
			else
			{
				timeForPurchase = 0f;
			}
			priority = Convert.ToInt32(dictionary["prior"]);
			time = Convert.ToSingle(dictionary["time"]);
		}
	}

	private class SituationBuff
	{
		public BuffParameter param;

		private float expireTime;

		public string weapon;

		public bool isDebuff
		{
			get
			{
				return param.healthBuff < 1f || param.damageBuff < 1f;
			}
		}

		public bool expired
		{
			get
			{
				return expireTime < NotificationController.instance.currentPlayTimeMatch;
			}
		}

		public SituationBuff(BuffParameter param, string weaponBuff)
		{
			this.param = param;
			weapon = weaponBuff;
			expireTime = NotificationController.instance.currentPlayTimeMatch + param.time;
		}

		public SituationBuff(BuffParameter param, string weaponBuff, float savedTime)
		{
			this.param = param;
			weapon = weaponBuff;
			expireTime = savedTime;
		}

		public Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["type"] = (int)param.type;
			dictionary["expire"] = expireTime;
			if (!string.IsNullOrEmpty(weapon))
			{
				dictionary["weapon"] = weapon;
			}
			return dictionary;
		}
	}

	private enum CheckStatus
	{
		None,
		NewPlayer,
		OldPlayer,
		Regular
	}

	private enum InteractionType
	{
		None,
		Kill,
		Death
	}

	private enum SituationBuffType
	{
		DebuffBeforeGun,
		DebuffAfterGun,
		TierLvlUp,
		TryGunBuff,
		BuyedTryGun,
		Coin1,
		Coin7,
		Coin2,
		Coin3,
		Coin4,
		Coin5,
		Coin8,
		Gem1,
		Gem2,
		Gem3,
		Gem4,
		Gem5,
		Gem6,
		Gem7,
		Count
	}

	public const int DiscountTryGun = 50;

	public const int TryGunPromoDuration = 3600;

	private static BuffSystem _instance;

	private bool[] interactionBuffs = new bool[17]
	{
		true, true, false, true, false, true, false, false, true, false,
		false, true, false, false, true, false, false
	};

	private ParamByTier[] paramsByTier;

	private Dictionary<SituationBuffType, BuffParameter> buffParamByType;

	private List<SituationBuff> situationBuffs = new List<SituationBuff>();

	private SituationBuff currentBuff;

	private SituationBuff weaponBuff;

	private bool configLoaded;

	private bool loadValuesCalled;

	private CheckStatus status;

	private InteractionType[] interactions = new InteractionType[30];

	private bool interactionsChanged;

	private bool buffsActive;

	private int interactionCounter;

	private BuffParameter waitingForPurchaseBuff;

	private float waitingForPurchaseTime;

	private float lastGiveGunTime;

	private bool readyToGiveGun;

	public bool giveTryGun;

	public float timeForDiscount = 3600f;

	public int discountValue = 50;

	private int roundsForGunLow = 3;

	private int roundsForGunMiddle = 2;

	private int roundsForGunHigh = 2;

	private float debuffKillrateForGun = 0.8f;

	private float firstBuffArmor = 8f;

	private float firstBuffNoArmor = 2f;

	private int interactionCountForOldPlayer = 10;

	private bool isFirstRounds;

	private int allRoundsCount;

	private float damageBuff = 1f;

	private float healthBuff = 1f;

	private float killRateCached = -1f;

	private readonly SituationBuffType[] gemsBuffByIndex = new SituationBuffType[7]
	{
		SituationBuffType.Gem1,
		SituationBuffType.Gem2,
		SituationBuffType.Gem3,
		SituationBuffType.Gem4,
		SituationBuffType.Gem5,
		SituationBuffType.Gem6,
		SituationBuffType.Gem7
	};

	private readonly SituationBuffType[] coinsBuffByIndex = new SituationBuffType[7]
	{
		SituationBuffType.Coin1,
		SituationBuffType.Coin7,
		SituationBuffType.Coin2,
		SituationBuffType.Coin3,
		SituationBuffType.Coin4,
		SituationBuffType.Coin5,
		SituationBuffType.Coin8
	};

	public static BuffSystem instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("BuffSystem");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				_instance = gameObject.AddComponent<BuffSystem>();
			}
			return _instance;
		}
	}

	private ParamByTier tierParam
	{
		get
		{
			return paramsByTier[ExpController.Instance.OurTier];
		}
	}

	public bool haveFirstInteractons
	{
		get
		{
			return interactionCounter >= 4;
		}
	}

	public bool haveAllInteractons
	{
		get
		{
			return status != CheckStatus.NewPlayer;
		}
	}

	public float weaponBuffValue
	{
		get
		{
			return (weaponBuff == null) ? 1f : Mathf.Clamp(weaponBuff.param.damageBuff, (GetKillrateByInteractions() < 0.8f) ? 1 : 0, (GetKillrateByInteractions() > 2f) ? 1 : 2);
		}
	}

	public void BuffsActive(bool value)
	{
		buffsActive = value;
		CheckExpiredBuffs();
	}

	public void CheckForPlayerBuff()
	{
		damageBuff = 1f;
		healthBuff = 1f;
		if (buffsActive)
		{
			float killrateByInteractions = GetKillrateByInteractions();
			currentBuff = null;
			weaponBuff = null;
			for (int i = 0; i < situationBuffs.Count; i++)
			{
				if (string.IsNullOrEmpty(situationBuffs[i].weapon))
				{
					if (currentBuff == null || currentBuff.param.priority < situationBuffs[i].param.priority)
					{
						currentBuff = situationBuffs[i];
					}
					if (currentBuff != null)
					{
						Debug.Log(string.Format("<color=green>Buff active: {0}</color>", currentBuff.param.type.ToString()));
					}
				}
				else
				{
					if (weaponBuff == null || weaponBuff.param.priority < situationBuffs[i].param.priority)
					{
						weaponBuff = situationBuffs[i];
					}
					if (weaponBuff != null)
					{
						Debug.Log(string.Format("<color=green>Weapon buff active: {0}</color>", weaponBuffValue));
					}
				}
			}
			switch (status)
			{
			case CheckStatus.NewPlayer:
				if (interactionCounter < interactionBuffs.Length && interactionBuffs[interactionCounter])
				{
					healthBuff = ((ExperienceController.sharedController.currentLevel != 1 && !ShopNGUIController.NoviceArmorAvailable) ? firstBuffNoArmor : firstBuffArmor);
				}
				break;
			case CheckStatus.Regular:
				if (currentBuff != null)
				{
					damageBuff = currentBuff.param.damageBuff;
					healthBuff = currentBuff.param.healthBuff;
				}
				else
				{
					healthBuff = (damageBuff = 1f + 0.01f * GetBuffPercentByKillRate(killrateByInteractions));
				}
				break;
			}
			if (status != CheckStatus.NewPlayer)
			{
				damageBuff = Mathf.Clamp(damageBuff, (killrateByInteractions < 0.8f) ? 1 : 0, (killrateByInteractions > 2f) ? 1 : 2);
				healthBuff = Mathf.Clamp(healthBuff, (killrateByInteractions < 0.8f) ? 1 : 0, (killrateByInteractions > 2f) ? 1 : 2);
			}
		}
		if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			WeaponManager.sharedManager.myPlayerMoveC.SetupBuffParameters(damageBuff, healthBuff);
		}
	}

	private float GetBuffPercentByKillRate(float value)
	{
		float num = Mathf.Round(10f * value) / 10f;
		Debug.Log(string.Format("<color=green>Killrate: {0}</color>", num));
		if (tierParam.midbottom < num && num < tierParam.midtop)
		{
			return 0f;
		}
		return tierParam.b - Mathf.Clamp(num, tierParam.bottom, tierParam.top) * tierParam.a;
	}

	public void KillInteraction()
	{
		CheckAndWriteInteraction(InteractionType.Kill);
	}

	public void DeathInteraction()
	{
		CheckAndWriteInteraction(InteractionType.Death);
		SaveInteractions();
	}

	private void CheckAndWriteInteraction(InteractionType value)
	{
		if (!buffsActive)
		{
			return;
		}
		switch (status)
		{
		case CheckStatus.NewPlayer:
			if (interactionCounter < interactionBuffs.Length && !interactionBuffs[interactionCounter])
			{
				WriteInteraction(value);
			}
			interactionCounter++;
			if (interactionCounter >= interactionBuffs.Length)
			{
				lastGiveGunTime = NotificationController.instance.currentPlayTimeMatch;
				status = CheckStatus.Regular;
			}
			break;
		case CheckStatus.OldPlayer:
			WriteInteraction(value);
			interactionCounter++;
			if (interactionCounter >= interactionCountForOldPlayer)
			{
				lastGiveGunTime = NotificationController.instance.currentPlayTimeMatch;
				status = CheckStatus.Regular;
			}
			break;
		case CheckStatus.Regular:
			WriteInteraction(value);
			interactionCounter++;
			break;
		}
		CheckForPlayerBuff();
	}

	private void WriteInteraction(InteractionType value)
	{
		interactionsChanged = true;
		killRateCached = -1f;
		for (int num = interactions.Length - 2; num >= 0; num--)
		{
			interactions[num + 1] = interactions[num];
		}
		interactions[0] = value;
	}

	public float GetKillrateByInteractions()
	{
		if (killRateCached != -1f)
		{
			return killRateCached;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < interactions.Length; i++)
		{
			switch (interactions[i])
			{
			case InteractionType.Kill:
				num++;
				break;
			case InteractionType.Death:
				num2++;
				break;
			}
		}
		if (num2 != 0)
		{
			killRateCached = (float)num / (float)num2;
		}
		else
		{
			killRateCached = num;
		}
		return killRateCached;
	}

	public void OnGetProgress()
	{
		if (status == CheckStatus.None || status == CheckStatus.NewPlayer)
		{
			status = CheckStatus.OldPlayer;
			damageBuff = 1f;
			healthBuff = 1f;
			isFirstRounds = false;
		}
		SaveValues();
	}

	private void WriteDefaultParameters()
	{
		configLoaded = false;
		if (paramsByTier == null)
		{
			paramsByTier = new ParamByTier[6];
			for (int i = 0; i < paramsByTier.Length; i++)
			{
				paramsByTier[i] = new ParamByTier();
			}
		}
		buffParamByType = new Dictionary<SituationBuffType, BuffParameter>(new SituationBuffComparer());
		buffParamByType.Add(SituationBuffType.DebuffBeforeGun, new BuffParameter(SituationBuffType.DebuffBeforeGun, 1f, 0.5f, 540f, -1));
		buffParamByType.Add(SituationBuffType.DebuffAfterGun, new BuffParameter(SituationBuffType.DebuffAfterGun, 1f, 0.5f, 180f, -1));
		buffParamByType.Add(SituationBuffType.TierLvlUp, new BuffParameter(SituationBuffType.TierLvlUp, 1f, 0.7f, 240f, -1));
		buffParamByType.Add(SituationBuffType.TryGunBuff, new BuffParameter(SituationBuffType.TryGunBuff, 1f, 1.25f, 0f, 1));
		buffParamByType.Add(SituationBuffType.BuyedTryGun, new BuffParameter(SituationBuffType.BuyedTryGun, 1f, 1.25f, 600f, 2));
		buffParamByType.Add(SituationBuffType.Coin1, new BuffParameter(SituationBuffType.Coin1, 1f, 1.25f, 300f, 3));
		buffParamByType.Add(SituationBuffType.Coin7, new BuffParameter(SituationBuffType.Coin7, 1f, 1.25f, 400f, 3));
		buffParamByType.Add(SituationBuffType.Coin2, new BuffParameter(SituationBuffType.Coin2, 1f, 1.25f, 500f, 3));
		buffParamByType.Add(SituationBuffType.Coin3, new BuffParameter(SituationBuffType.Coin3, 1f, 1.25f, 600f, 3));
		buffParamByType.Add(SituationBuffType.Coin4, new BuffParameter(SituationBuffType.Coin4, 1f, 1.25f, 700f, 3));
		buffParamByType.Add(SituationBuffType.Coin5, new BuffParameter(SituationBuffType.Coin5, 1f, 1.25f, 800f, 3));
		buffParamByType.Add(SituationBuffType.Coin8, new BuffParameter(SituationBuffType.Coin8, 1f, 1.25f, 900f, 3));
		buffParamByType.Add(SituationBuffType.Gem1, new BuffParameter(SituationBuffType.Gem1, 1f, 1.25f, 300f, 4));
		buffParamByType.Add(SituationBuffType.Gem2, new BuffParameter(SituationBuffType.Gem2, 1f, 1.25f, 400f, 4));
		buffParamByType.Add(SituationBuffType.Gem3, new BuffParameter(SituationBuffType.Gem3, 1f, 1.25f, 500f, 4));
		buffParamByType.Add(SituationBuffType.Gem4, new BuffParameter(SituationBuffType.Gem4, 1f, 1.25f, 600f, 4));
		buffParamByType.Add(SituationBuffType.Gem5, new BuffParameter(SituationBuffType.Gem5, 1f, 1.25f, 700f, 4));
		buffParamByType.Add(SituationBuffType.Gem6, new BuffParameter(SituationBuffType.Gem6, 1f, 1.25f, 800f, 4));
		buffParamByType.Add(SituationBuffType.Gem7, new BuffParameter(SituationBuffType.Gem7, 1f, 1.25f, 900f, 4));
	}

	public void TryLoadConfig()
	{
		if (configLoaded)
		{
			return;
		}
		if (!Storager.hasKey("BuffsParam"))
		{
			WriteDefaultParameters();
			return;
		}
		string @string = Storager.getString("BuffsParam", false);
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary == null || !dictionary.ContainsKey("buffSettings"))
		{
			WriteDefaultParameters();
			return;
		}
		Dictionary<string, object> dictionary2 = dictionary["buffSettings"] as Dictionary<string, object>;
		string text = string.Empty;
		if (dictionary2.ContainsKey("roundsForGunLow"))
		{
			roundsForGunLow = Convert.ToInt32(dictionary2["roundsForGunLow"]);
		}
		else
		{
			text = "get roundsForGunLow";
		}
		if (dictionary2.ContainsKey("roundsForGunMiddle"))
		{
			roundsForGunMiddle = Convert.ToInt32(dictionary2["roundsForGunMiddle"]);
		}
		else
		{
			text = "get roundsForGunMiddle";
		}
		if (dictionary2.ContainsKey("roundsForGunHigh"))
		{
			roundsForGunHigh = Convert.ToInt32(dictionary2["roundsForGunHigh"]);
		}
		else
		{
			text = "get roundsForGunHigh";
		}
		if (dictionary2.ContainsKey("timeForDiscount"))
		{
			timeForDiscount = Convert.ToSingle(dictionary2["timeForDiscount"]);
		}
		else
		{
			text = "get timeForDiscount";
		}
		if (dictionary2.ContainsKey("discountValue"))
		{
			discountValue = Convert.ToInt32(dictionary2["discountValue"]);
		}
		else
		{
			text = "get discountValue";
		}
		if (dictionary2.ContainsKey("debuffKillrateForGun"))
		{
			debuffKillrateForGun = Convert.ToSingle(dictionary2["debuffKillrateForGun"]);
		}
		else
		{
			text = "get debuffKillrateForGun";
		}
		if (dictionary2.ContainsKey("firstBuffArmor"))
		{
			firstBuffArmor = Convert.ToSingle(dictionary2["firstBuffArmor"]);
		}
		else
		{
			text = "get firstBuffArmor";
		}
		if (dictionary2.ContainsKey("firstBuffNoArmor"))
		{
			firstBuffNoArmor = Convert.ToSingle(dictionary2["firstBuffNoArmor"]);
		}
		else
		{
			text = "get firstBuffNoArmor";
		}
		if (dictionary2.ContainsKey("tierParams"))
		{
			List<object> list = dictionary2["tierParams"] as List<object>;
			if (list != null)
			{
				paramsByTier = list.Select((object e) => new ParamByTier(e as Dictionary<string, object>)).ToArray();
			}
			else
			{
				text = "tierParams == null";
			}
		}
		else
		{
			text = "get tierParams";
		}
		if (dictionary2.ContainsKey("buffsParams"))
		{
			List<object> list2 = dictionary2["buffsParams"] as List<object>;
			buffParamByType = new Dictionary<SituationBuffType, BuffParameter>();
			for (int i = 0; i < list2.Count; i++)
			{
				Dictionary<string, object> dictionary3 = list2[i] as Dictionary<string, object>;
				SituationBuffType key = (SituationBuffType)(int)Enum.Parse(typeof(SituationBuffType), Convert.ToString(dictionary3["type"]));
				buffParamByType.Add(key, new BuffParameter(dictionary3));
			}
		}
		else
		{
			text = "get buffsParams";
		}
		if (!string.IsNullOrEmpty(text))
		{
			Debug.LogError("Error Deserialize JSON: buffSettings - " + text);
			WriteDefaultParameters();
		}
		else
		{
			configLoaded = true;
		}
	}

	private void LoadValues()
	{
		loadValuesCalled = true;
		string @string = Storager.getString("buffsValues", false);
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary != null)
		{
			if (dictionary.ContainsKey("interactionCount"))
			{
				interactionCounter = Convert.ToInt32(dictionary["interactionCount"]);
			}
			if (dictionary.ContainsKey("allRoundsCount"))
			{
				allRoundsCount = Convert.ToInt32(dictionary["allRoundsCount"]);
			}
			if (dictionary.ContainsKey("isFirstRounds"))
			{
				isFirstRounds = Convert.ToInt32(dictionary["isFirstRounds"]) == 1;
			}
			if (dictionary.ContainsKey("status"))
			{
				status = (CheckStatus)Convert.ToInt32(dictionary["status"]);
			}
			if (dictionary.ContainsKey("lastGiveGunTime"))
			{
				lastGiveGunTime = Convert.ToSingle(dictionary["lastGiveGunTime"]);
			}
			if (dictionary.ContainsKey("giveGun"))
			{
				readyToGiveGun = Convert.ToInt32(dictionary["giveGun"]) == 1;
			}
			if (dictionary.ContainsKey("waitTime"))
			{
				waitingForPurchaseTime = Convert.ToSingle(dictionary["waitTime"]);
			}
			if (dictionary.ContainsKey("waitBuff"))
			{
				SituationBuffType key = (SituationBuffType)Convert.ToInt32(dictionary["waitBuff"]);
				if (buffParamByType.ContainsKey(key))
				{
					waitingForPurchaseBuff = buffParamByType[key];
				}
			}
			if (dictionary.ContainsKey("buffs"))
			{
				List<object> list = dictionary["buffs"] as List<object>;
				for (int i = 0; i < list.Count; i++)
				{
					Dictionary<string, object> dictionary2 = list[i] as Dictionary<string, object>;
					SituationBuffType key2 = (SituationBuffType)Convert.ToInt32(dictionary2["type"]);
					string text = ((!dictionary2.ContainsKey("weapon")) ? string.Empty : Convert.ToString(dictionary2["weapon"]));
					float savedTime = Convert.ToSingle(dictionary2["expire"]);
					SituationBuff item = new SituationBuff(buffParamByType[key2], text, savedTime);
					situationBuffs.Add(item);
				}
			}
		}
		if (status == CheckStatus.None)
		{
			if (Storager.getInt(Defs.TrainingCompleted_4_4_Sett, false) != 1)
			{
				status = CheckStatus.NewPlayer;
				isFirstRounds = true;
			}
			else
			{
				status = CheckStatus.OldPlayer;
			}
			SaveValues();
		}
		string string2 = Storager.getString("buffsPlayerInteractions", false);
		List<object> list2 = Json.Deserialize(string2) as List<object>;
		if (list2 != null)
		{
			interactions = list2.Select((object o) => (InteractionType)Convert.ToInt32(o)).ToArray();
		}
	}

	private void SaveValues()
	{
		if (!loadValuesCalled)
		{
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (interactionCounter > 0)
		{
			dictionary["interactionCount"] = interactionCounter;
		}
		if (isFirstRounds)
		{
			dictionary["isFirstRounds"] = 1;
		}
		if (readyToGiveGun)
		{
			dictionary["giveGun"] = 1;
		}
		if (allRoundsCount > 0)
		{
			dictionary["allRoundsCount"] = allRoundsCount;
		}
		if (situationBuffs != null && situationBuffs.Count > 0)
		{
			dictionary["buffs"] = situationBuffs.Select((SituationBuff o) => o.Serialize()).ToArray();
		}
		if (lastGiveGunTime > 0f)
		{
			dictionary["lastGiveGunTime"] = lastGiveGunTime;
		}
		if (waitingForPurchaseTime > 0f)
		{
			dictionary["waitTime"] = waitingForPurchaseTime;
		}
		if (waitingForPurchaseBuff != null)
		{
			dictionary["waitBuff"] = (int)waitingForPurchaseBuff.type;
		}
		dictionary["status"] = (int)status;
		Storager.setString("buffsValues", Json.Serialize(dictionary), false);
		SaveInteractions();
	}

	private void SaveInteractions()
	{
		if (interactionsChanged)
		{
			interactionsChanged = false;
			Storager.setString("buffsPlayerInteractions", Json.Serialize(interactions.Select((InteractionType o) => (int)o).ToArray()), false);
		}
	}

	private void GiveTryGunToPlayer()
	{
		readyToGiveGun = true;
	}

	private float GetTimeForGun()
	{
		float killrateByInteractions = GetKillrateByInteractions();
		if (killrateByInteractions < tierParam.lowKillRate)
		{
			return tierParam.timeToGetGunLow;
		}
		if (killrateByInteractions < tierParam.highKillRate)
		{
			return tierParam.timeToGetGunMiddle;
		}
		return tierParam.timeToGetGunHigh;
	}

	private void CheckExpiredBuffs()
	{
		if (!buffsActive)
		{
			return;
		}
		for (int i = 0; i < situationBuffs.Count; i++)
		{
			if (situationBuffs[i].expired && situationBuffs[i].param.type != SituationBuffType.TryGunBuff)
			{
				if (situationBuffs[i].param.type == SituationBuffType.DebuffBeforeGun)
				{
					GiveTryGunToPlayer();
				}
				situationBuffs.RemoveAt(i);
				i--;
			}
		}
	}

	public void PlayerLeaved()
	{
		CheckExpiredBuffs();
		SaveValues();
	}

	public void EndRound()
	{
		float killrateByInteractions = GetKillrateByInteractions();
		Debug.Log(killrateByInteractions);
		if (isFirstRounds && allRoundsCount < 10)
		{
			AnalyticsStuff.LogFirstBattlesKillRate(allRoundsCount, killrateByInteractions);
		}
		allRoundsCount++;
		if (allRoundsCount == 3)
		{
			AnalyticsStuff.TrySendOnceToAppsFlyer("third_round_complete");
		}
		if (allRoundsCount > 9)
		{
			isFirstRounds = false;
		}
		if (buffsActive && configLoaded)
		{
			CheckExpiredBuffs();
			if (status != CheckStatus.NewPlayer && status != CheckStatus.OldPlayer)
			{
				if (lastGiveGunTime + GetTimeForGun() < NotificationController.instance.currentPlayTimeMatch)
				{
					lastGiveGunTime = NotificationController.instance.currentPlayTimeMatch;
					if (GetKillrateByInteractions() > debuffKillrateForGun)
					{
						AddSituationBuff(SituationBuffType.DebuffBeforeGun, string.Empty);
					}
					else
					{
						GiveTryGunToPlayer();
					}
				}
				if (readyToGiveGun && WeaponManager.sharedManager._currentFilterMap == 0)
				{
					giveTryGun = true;
				}
			}
		}
		SaveValues();
		CheckForPlayerBuff();
	}

	private void AddSituationBuff(SituationBuffType type, string buffForWeapon = "")
	{
		situationBuffs.Add(new SituationBuff(buffParamByType[type], buffForWeapon));
		SaveValues();
		CheckForPlayerBuff();
	}

	private void ClearDebuffs()
	{
		for (int i = 0; i < situationBuffs.Count; i++)
		{
			if (situationBuffs[i].isDebuff)
			{
				situationBuffs.RemoveAt(i);
				i--;
			}
		}
	}

	private void ClearBuffOfType(SituationBuffType type)
	{
		for (int i = 0; i < situationBuffs.Count; i++)
		{
			if (situationBuffs[i].param.type == type)
			{
				situationBuffs.RemoveAt(i);
				i--;
			}
		}
	}

	public void SetGetTryGun(string weaponName)
	{
		giveTryGun = false;
		readyToGiveGun = false;
		ClearDebuffs();
		AddSituationBuff(SituationBuffType.TryGunBuff, weaponName);
	}

	public void OnTryGunBuyed(string weaponName)
	{
		ClearDebuffs();
		AddSituationBuff(SituationBuffType.BuyedTryGun, weaponName);
	}

	public void OnGunBuyed()
	{
		ClearDebuffs();
		lastGiveGunTime = NotificationController.instance.currentPlayTimeMatch;
		SaveValues();
		CheckForPlayerBuff();
	}

	public void OnCurrencyBuyed(bool isGems, int index)
	{
		SituationBuffType key;
		if (isGems)
		{
			if (index >= gemsBuffByIndex.Length)
			{
				return;
			}
			key = gemsBuffByIndex[index];
		}
		else
		{
			if (index >= coinsBuffByIndex.Length)
			{
				return;
			}
			key = coinsBuffByIndex[index];
		}
		if (buffParamByType.ContainsKey(key))
		{
			BuffParameter buffParameter = buffParamByType[key];
			if (waitingForPurchaseBuff == null || waitingForPurchaseTime < buffParameter.timeForPurchase + NotificationController.instance.currentPlayTime)
			{
				waitingForPurchaseBuff = buffParameter;
				waitingForPurchaseTime = waitingForPurchaseBuff.timeForPurchase + NotificationController.instance.currentPlayTime;
			}
			SaveValues();
		}
	}

	public void OnSomethingPurchased()
	{
		if (waitingForPurchaseBuff != null)
		{
			if (waitingForPurchaseTime > NotificationController.instance.currentPlayTime)
			{
				SituationBuffType type = waitingForPurchaseBuff.type;
				waitingForPurchaseBuff = null;
				waitingForPurchaseTime = 0f;
				ClearDebuffs();
				AddSituationBuff(type, string.Empty);
			}
			else
			{
				waitingForPurchaseBuff = null;
				waitingForPurchaseTime = 0f;
				SaveValues();
			}
		}
	}

	public void OnGunTakeOff()
	{
		ClearBuffOfType(SituationBuffType.TryGunBuff);
		AddSituationBuff(SituationBuffType.DebuffAfterGun, string.Empty);
	}

	public void RemoveGunBuff()
	{
		ClearBuffOfType(SituationBuffType.TryGunBuff);
	}

	public void OnTierLvlUp()
	{
		AddSituationBuff(SituationBuffType.TierLvlUp, string.Empty);
	}

	public int GetRoundsForGun()
	{
		float killrateByInteractions = GetKillrateByInteractions();
		if (killrateByInteractions < tierParam.lowKillRate)
		{
			return roundsForGunLow;
		}
		if (killrateByInteractions < tierParam.highKillRate)
		{
			return roundsForGunMiddle;
		}
		return roundsForGunHigh;
	}

	public bool haveBuffForWeapon(string weapon)
	{
		return weaponBuff != null && !string.IsNullOrEmpty(weapon) && weaponBuff.weapon == weapon;
	}

	public void LogFirstBattlesResult(bool isWinner)
	{
		if (isFirstRounds && allRoundsCount < 10)
		{
			AnalyticsStuff.LogFirstBattlesResult(allRoundsCount, isWinner);
		}
	}

	private void Awake()
	{
		TryLoadConfig();
		LoadValues();
		CheckForPlayerBuff();
		ShopNGUIController.GunOrArmorBought += OnGunBuyed;
	}
}

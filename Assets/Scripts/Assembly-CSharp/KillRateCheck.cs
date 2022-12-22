using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public class KillRateCheck
{
	private enum CheckStatus
	{
		None,
		Starter,
		StarterBuff,
		WaitGetGun,
		GetGun,
		BuyedGun,
		CoolDown,
		StarterBuff2,
		HardPlayer,
		HardDebuff,
		HardDebuff2,
		DebuffAfterGun
	}

	public class ParamByTier
	{
		public int cooldownRoundsLow;

		public int cooldownRoundsMiddle;

		public int cooldownRoundsHigh;

		public float lowKillRate;

		public float highKillRate;

		public float buffL1 = 1f;

		public float buffL2 = 1f;

		public int buffRoundsL1 = 2;

		public int buffRoundsL2 = 1;

		public ParamByTier()
		{
			cooldownRoundsLow = 10;
			cooldownRoundsMiddle = 15;
			cooldownRoundsHigh = 20;
			lowKillRate = 0.5f;
			highKillRate = 1.2f;
			buffL1 = 1.4f;
			buffL2 = 1.2f;
			buffRoundsL1 = 2;
			buffRoundsL2 = 2;
		}

		public ParamByTier(Dictionary<string, object> dictionary)
		{
			cooldownRoundsLow = Convert.ToInt32(dictionary["cooldownRoundsLow"]);
			cooldownRoundsMiddle = Convert.ToInt32(dictionary["cooldownRoundsMiddle"]);
			cooldownRoundsHigh = Convert.ToInt32(dictionary["cooldownRoundsHigh"]);
			lowKillRate = Convert.ToSingle(dictionary["lowKillRate"]);
			highKillRate = Convert.ToSingle(dictionary["highKillRate"]);
			buffL1 = Convert.ToSingle(dictionary["buffL1"]);
			buffL2 = Convert.ToSingle(dictionary["buffL2"]);
			buffRoundsL1 = Convert.ToInt32(dictionary["buffRoundsL1"]);
			buffRoundsL2 = Convert.ToInt32(dictionary["buffRoundsL2"]);
		}
	}

	public const int DiscountTryGun = 50;

	public const int TryGunPromoDuration = 3600;

	private static KillRateCheck _instance;

	private static float lastConfigCheck;

	private bool activeFromServer;

	private ParamByTier[] paramsByTier;

	private int startRounds = 5;

	private int roundsForCheckL1 = 3;

	private int roundsForCheckL2 = 2;

	private int roundsForL2Buff = 3;

	private float starterHighKillrate = 1.2f;

	private int roundsForGunLow = 3;

	private int roundsForGunMiddle = 2;

	private int roundsForGunHigh = 2;

	private float debuffL1 = 0.85f;

	private float debuffL2 = 0.7f;

	private int debuffRoundsL1 = 2;

	private int debuffRoundsL2 = 2;

	private float debuffVal = 1.2f;

	private int debuffRoundsAfterGun = 1;

	private float debuffAfterGun = 0.75f;

	public float timeForDiscount = 3600f;

	public int discountValue = 50;

	private CheckStatus status;

	private bool writeKill;

	private int[] kills = new int[30];

	private int[] deaths = new int[30];

	private int roundCount;

	private int allRoundsCount;

	private float killRateVal;

	private int starterBuff;

	private float nextBuffCheck;

	private int killRateLength;

	private int killCount;

	private int deathCount;

	public bool buffEnabled;

	public float damageBuff = 1f;

	public float healthBuff = 1f;

	public bool giveWeapon;

	public bool active;

	private bool calcbuff;

	private bool isFirstRounds;

	private bool configLoaded;

	public static KillRateCheck instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KillRateCheck();
			}
			if (!_instance.configLoaded && Time.time > lastConfigCheck)
			{
				lastConfigCheck = Time.time + 20f;
				_instance.LoadParameters();
				Debug.LogWarning("KillRateCheck config not loaded: try loading");
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

	public KillRateCheck()
	{
		LoadParameters();
		string @string = Storager.getString("killRateValues", false);
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary != null)
		{
			if (dictionary.ContainsKey("roundCount"))
			{
				roundCount = Convert.ToInt32(dictionary["roundCount"]);
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
			if (dictionary.ContainsKey("killRateVal"))
			{
				killRateVal = Convert.ToSingle(dictionary["killRateVal"]);
			}
			if (dictionary.ContainsKey("nextBuffCheck"))
			{
				nextBuffCheck = Convert.ToSingle(dictionary["nextBuffCheck"]);
			}
			if (dictionary.ContainsKey("StarterBuff"))
			{
				starterBuff = Convert.ToInt32(dictionary["StarterBuff"]);
			}
			CheckForBuff();
		}
		if (status == CheckStatus.None)
		{
			if (Storager.getInt(Defs.TrainingCompleted_4_4_Sett, false) != 1)
			{
				status = CheckStatus.StarterBuff;
				starterBuff = 2;
				isFirstRounds = true;
			}
			else
			{
				status = CheckStatus.Starter;
			}
			SaveValues();
			CheckForBuff();
		}
		string string2 = Storager.getString("LastKillRates", false);
		List<object> list = Json.Deserialize(string2) as List<object>;
		if (list != null && list.Count == 2)
		{
			kills = (list[0] as List<object>).Select((object o) => Convert.ToInt32(o)).ToArray();
			deaths = (list[1] as List<object>).Select((object o) => Convert.ToInt32(o)).ToArray();
		}
	}

	public void SetActive(bool isAcitve, bool roundMore30Sec)
	{
		active = isAcitve && activeFromServer && roundMore30Sec && configLoaded;
		writeKill = isAcitve && roundMore30Sec;
		calcbuff = roundMore30Sec && configLoaded;
	}

	private void WriteDefaultParameters()
	{
		active = false;
		if (paramsByTier == null)
		{
			paramsByTier = new ParamByTier[6];
			for (int i = 0; i < paramsByTier.Length; i++)
			{
				paramsByTier[i] = new ParamByTier();
			}
		}
	}

	public void OnGetProgress()
	{
		if (status == CheckStatus.StarterBuff)
		{
			status = CheckStatus.Starter;
			buffEnabled = false;
			damageBuff = 1f;
			healthBuff = 1f;
		}
		isFirstRounds = false;
		SaveValues();
	}

	private void LoadParameters()
	{
		//Discarded unreachable code: IL_027d
		if (!Storager.hasKey("BuffsParam"))
		{
			WriteDefaultParameters();
			return;
		}
		string @string = Storager.getString("BuffsParam", false);
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary == null || !dictionary.ContainsKey("killRate"))
		{
			WriteDefaultParameters();
			return;
		}
		try
		{
			Dictionary<string, object> dictionary2 = dictionary["killRate"] as Dictionary<string, object>;
			activeFromServer = Convert.ToInt32(dictionary2["active"]) == 1;
			startRounds = Convert.ToInt32(dictionary2["startRounds"]);
			roundsForCheckL1 = Convert.ToInt32(dictionary2["roundsForCheckL1"]);
			roundsForCheckL2 = Convert.ToInt32(dictionary2["roundsForCheckL2"]);
			roundsForL2Buff = Convert.ToInt32(dictionary2["roundsForL2Buff"]);
			starterHighKillrate = Convert.ToSingle(dictionary2["starterHighKillrate"]);
			roundsForGunLow = Convert.ToInt32(dictionary2["roundsForGunLow"]);
			roundsForGunMiddle = Convert.ToInt32(dictionary2["roundsForGunMiddle"]);
			roundsForGunHigh = Convert.ToInt32(dictionary2["roundsForGunHigh"]);
			killRateLength = Convert.ToInt32(dictionary2["killRateLength"]);
			timeForDiscount = Convert.ToSingle(dictionary2["timeForDiscount"]);
			discountValue = Convert.ToInt32(dictionary2["discountValue"]);
			debuffL1 = Convert.ToSingle(dictionary2["debuffL1"]);
			debuffL2 = Convert.ToSingle(dictionary2["debuffL2"]);
			debuffRoundsL1 = Convert.ToInt32(dictionary2["debuffRoundsL1"]);
			debuffRoundsL2 = Convert.ToInt32(dictionary2["debuffRoundsL2"]);
			debuffVal = Convert.ToSingle(dictionary2["killrateForDebuff"]);
			debuffRoundsAfterGun = Convert.ToInt32(dictionary2["debuffRoundsAfterGun"]);
			debuffAfterGun = Convert.ToSingle(dictionary2["debuffAfterGun"]);
			List<object> list = dictionary2["tierParams"] as List<object>;
			if (list == null)
			{
				Debug.LogWarning("Error Deserialize JSON: tierParams");
				return;
			}
			paramsByTier = list.Select((object e) => new ParamByTier(e as Dictionary<string, object>)).ToArray();
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Error Deserialize JSON: BuffsParam: " + ex);
			WriteDefaultParameters();
			return;
		}
		configLoaded = true;
	}

	public void IncrementKills()
	{
		if (active)
		{
			killCount++;
		}
	}

	public void IncrementDeath()
	{
		if (active)
		{
			deathCount++;
		}
	}

	private void WriteKillRate()
	{
		for (int num = kills.Length - 2; num >= 0; num--)
		{
			kills[num + 1] = kills[num];
		}
		kills[0] = killCount;
		for (int num2 = deaths.Length - 2; num2 >= 0; num2--)
		{
			deaths[num2 + 1] = deaths[num2];
		}
		deaths[0] = deathCount;
		killCount = 0;
		deathCount = 0;
		SaveKillRates();
	}

	public float GetKillRate()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < Mathf.Min(killRateLength, kills.Length); i++)
		{
			num += kills[i];
		}
		for (int j = 0; j < Mathf.Min(killRateLength, deaths.Length); j++)
		{
			num2 += deaths[j];
		}
		if (num2 != 0)
		{
			return (float)num / (float)num2;
		}
		return num;
	}

	private void SaveKillRates()
	{
		Storager.setString("LastKillRates", Json.Serialize(new int[2][] { kills, deaths }), false);
	}

	private void CheckForBuff()
	{
		damageBuff = 1f;
		healthBuff = 1f;
		buffEnabled = false;
		if (status == CheckStatus.StarterBuff || status == CheckStatus.StarterBuff2 || status == CheckStatus.HardPlayer)
		{
			switch (starterBuff)
			{
			case 2:
				buffEnabled = true;
				damageBuff = tierParam.buffL1;
				healthBuff = tierParam.buffL1;
				break;
			case 1:
				buffEnabled = true;
				damageBuff = tierParam.buffL2;
				healthBuff = tierParam.buffL2;
				break;
			}
		}
		if (active)
		{
			if (status == CheckStatus.HardDebuff)
			{
				buffEnabled = true;
				damageBuff = debuffL1;
				healthBuff = debuffL1;
			}
			else if (status == CheckStatus.HardDebuff2)
			{
				buffEnabled = true;
				damageBuff = debuffL2;
				healthBuff = debuffL2;
			}
			else if (status == CheckStatus.DebuffAfterGun)
			{
				buffEnabled = true;
				damageBuff = debuffAfterGun;
				healthBuff = debuffAfterGun;
			}
			else if (killRateVal < tierParam.lowKillRate)
			{
				if (status == CheckStatus.GetGun || (status == CheckStatus.BuyedGun && roundCount < tierParam.buffRoundsL1))
				{
					buffEnabled = true;
					damageBuff = tierParam.buffL1;
					healthBuff = tierParam.buffL1;
				}
				else if (status == CheckStatus.BuyedGun && roundCount < tierParam.buffRoundsL1 + tierParam.buffRoundsL2)
				{
					buffEnabled = true;
					damageBuff = tierParam.buffL2;
					healthBuff = tierParam.buffL2;
				}
			}
			else if (killRateVal < tierParam.highKillRate && (status == CheckStatus.GetGun || (status == CheckStatus.BuyedGun && roundCount < tierParam.buffRoundsL2)))
			{
				buffEnabled = true;
				damageBuff = tierParam.buffL2;
				healthBuff = tierParam.buffL2;
			}
		}
		if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			WeaponManager.sharedManager.myPlayerMoveC.SetupBuffParameters(damageBuff, healthBuff);
		}
	}

	private void GiveWeaponByKillRate(float rateValue)
	{
		if (active)
		{
			giveWeapon = true;
			roundCount = 0;
			status = CheckStatus.WaitGetGun;
			killRateVal = rateValue;
			SaveValues();
		}
	}

	public int GetRoundsForGun()
	{
		if (killRateVal < tierParam.lowKillRate)
		{
			return roundsForGunLow;
		}
		if (killRateVal < tierParam.highKillRate)
		{
			return roundsForGunMiddle;
		}
		return roundsForGunHigh;
	}

	private int GetCooldownForGun(float value)
	{
		if (value < tierParam.lowKillRate)
		{
			return tierParam.cooldownRoundsLow;
		}
		if (value < tierParam.highKillRate)
		{
			return tierParam.cooldownRoundsMiddle;
		}
		return tierParam.cooldownRoundsHigh;
	}

	public void LogFirstBattlesResult(bool isWinner)
	{
		if (isFirstRounds && allRoundsCount < 10)
		{
			AnalyticsStuff.LogFirstBattlesResult(allRoundsCount, isWinner);
		}
	}

	public void CheckKillRate()
	{
		if (writeKill)
		{
			WriteKillRate();
		}
		float killRate = GetKillRate();
		Debug.Log(killRate);
		if (isFirstRounds && allRoundsCount < 10)
		{
			AnalyticsStuff.LogFirstBattlesKillRate(allRoundsCount, killRate);
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
		switch (status)
		{
		case CheckStatus.Starter:
			if (!calcbuff)
			{
				return;
			}
			roundCount++;
			if (roundCount >= startRounds)
			{
				if (killRate < tierParam.lowKillRate)
				{
					GiveWeaponByKillRate(killRate);
					break;
				}
				roundCount = 0;
				status = CheckStatus.CoolDown;
			}
			break;
		case CheckStatus.StarterBuff:
			if (!calcbuff)
			{
				return;
			}
			roundCount++;
			if (roundCount >= roundsForCheckL1 && killRate >= tierParam.highKillRate)
			{
				roundCount = 0;
				starterBuff = 1;
				status = CheckStatus.HardPlayer;
			}
			else if (roundCount >= startRounds)
			{
				if (killRate < tierParam.lowKillRate)
				{
					GiveWeaponByKillRate(killRate);
				}
				else if (killRate < tierParam.highKillRate && roundCount == startRounds)
				{
					roundCount = 0;
					starterBuff = 1;
					status = CheckStatus.StarterBuff2;
				}
				else
				{
					roundCount = 0;
					status = CheckStatus.CoolDown;
				}
			}
			break;
		case CheckStatus.StarterBuff2:
			if (!calcbuff)
			{
				return;
			}
			roundCount++;
			if (killRate < tierParam.lowKillRate)
			{
				GiveWeaponByKillRate(killRate);
			}
			else if (roundCount >= roundsForL2Buff)
			{
				roundCount = 0;
				status = CheckStatus.CoolDown;
			}
			break;
		case CheckStatus.HardPlayer:
			if (!calcbuff)
			{
				return;
			}
			roundCount++;
			if (roundCount == roundsForCheckL2)
			{
				if (killRate >= tierParam.highKillRate)
				{
					roundCount = 0;
					status = CheckStatus.CoolDown;
				}
				else
				{
					roundCount = 0;
					status = CheckStatus.StarterBuff2;
				}
			}
			break;
		case CheckStatus.CoolDown:
			if (!active)
			{
				return;
			}
			roundCount++;
			if (roundCount >= GetCooldownForGun(killRate))
			{
				if (killRate > debuffVal)
				{
					roundCount = 0;
					status = CheckStatus.HardDebuff;
				}
				else
				{
					GiveWeaponByKillRate(killRate);
				}
			}
			break;
		case CheckStatus.HardDebuff:
			if (!active)
			{
				return;
			}
			roundCount++;
			if (roundCount >= debuffRoundsL1)
			{
				roundCount = 0;
				status = CheckStatus.HardDebuff2;
			}
			break;
		case CheckStatus.HardDebuff2:
			if (!active)
			{
				return;
			}
			roundCount++;
			if (roundCount >= debuffRoundsL2)
			{
				roundCount = 0;
				GiveWeaponByKillRate(killRate);
			}
			break;
		case CheckStatus.DebuffAfterGun:
			if (!active)
			{
				return;
			}
			roundCount++;
			if (roundCount >= debuffRoundsAfterGun)
			{
				roundCount = 0;
				status = CheckStatus.CoolDown;
			}
			break;
		case CheckStatus.WaitGetGun:
			if (!active)
			{
				return;
			}
			giveWeapon = true;
			break;
		case CheckStatus.BuyedGun:
			if (!calcbuff)
			{
				return;
			}
			roundCount++;
			if (roundCount >= tierParam.buffRoundsL1 + tierParam.buffRoundsL2)
			{
				roundCount = 0;
				status = CheckStatus.CoolDown;
			}
			break;
		}
		SaveValues();
		CheckForBuff();
	}

	public void SetGetWeapon()
	{
		giveWeapon = false;
		if (active)
		{
			roundCount = 0;
			status = CheckStatus.GetGun;
			SaveValues();
			CheckForBuff();
		}
	}

	private void SaveValues()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (roundCount > 0)
		{
			dictionary["roundCount"] = roundCount;
		}
		if (allRoundsCount > 0)
		{
			dictionary["allRoundsCount"] = allRoundsCount;
		}
		if (isFirstRounds)
		{
			dictionary["isFirstRounds"] = 1;
		}
		if (killRateVal > 0f)
		{
			dictionary["killRateVal"] = killRateVal;
		}
		if (nextBuffCheck > 0f)
		{
			dictionary["nextBuffCheck"] = nextBuffCheck;
		}
		if (status == CheckStatus.StarterBuff && starterBuff > 0)
		{
			dictionary["StarterBuff"] = starterBuff;
		}
		dictionary["status"] = (int)status;
		Storager.setString("killRateValues", Json.Serialize(dictionary), false);
	}

	public static void OnTryGunBuyed()
	{
		instance.WriteStatusAndResetCounter(CheckStatus.BuyedGun, 0);
	}

	public static void OnGunTakeOff()
	{
		instance.WriteStatusAndResetCounter(CheckStatus.DebuffAfterGun, -1);
	}

	public static void RemoveGunBuff()
	{
		instance.MakeRemoveGunBuff();
	}

	public void MakeRemoveGunBuff()
	{
		if (status == CheckStatus.GetGun)
		{
			instance.WriteStatusAndResetCounter(CheckStatus.CoolDown, 0);
		}
	}

	private void WriteStatusAndResetCounter(CheckStatus stat, int round)
	{
		roundCount = round;
		status = stat;
		SaveValues();
		CheckForBuff();
	}
}

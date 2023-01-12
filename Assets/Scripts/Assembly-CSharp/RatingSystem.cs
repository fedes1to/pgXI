using System;
using System.Collections.Generic;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class RatingSystem : MonoBehaviour
{
	public struct MatchStat
	{
		public int place;

		public int playerCount;

		public bool winner;

		public bool deadHeat;

		public static MatchStat LooseStat
		{
			get
			{
				return new MatchStat(1, 2, false, false);
			}
		}

		public MatchStat(int place, int playerCount, bool winner, bool deadHeat)
		{
			this.place = place;
			this.playerCount = playerCount;
			this.winner = winner;
			this.deadHeat = deadHeat;
		}
	}

	public struct RatingChange
	{
		public RatingLeague oldLeague;

		public RatingLeague newLeague;

		public int oldDivision;

		public int newDivision;

		public int oldRating;

		public int newRating;

		public float oldRatingAmount
		{
			get
			{
				float num = instance.leagueRatings[Mathf.Clamp(GetOldLeagueIndex(), 0, instance.leagueRatings.Length - 1)];
				float num2 = instance.leagueRatings[Mathf.Clamp(GetOldLeagueIndex() + 1, 0, instance.leagueRatings.Length - 1)];
				float a = ((oldRating <= newRating) ? 0.015f : 0.03f);
				if (oldRating == 0)
				{
					a = 0f;
				}
				if (oldLeague == RatingLeague.Adamant && newLeague == oldLeague)
				{
					return 1f;
				}
				return Mathf.Max(a, Mathf.Clamp01(((float)oldRating - num) / (num2 - num)));
			}
		}

		public float newRatingAmount
		{
			get
			{
				float num = instance.leagueRatings[Mathf.Clamp(GetOldLeagueIndex(), 0, instance.leagueRatings.Length - 1)];
				float num2 = instance.leagueRatings[Mathf.Clamp(GetOldLeagueIndex() + 1, 0, instance.leagueRatings.Length - 1)];
				float a = ((newRating <= oldRating) ? 0.015f : 0.03f);
				if ((float)newRating - (num - 100f) < 0f)
				{
					a = 0f;
				}
				return Mathf.Max(a, Mathf.Clamp01(((float)newRating - num) / (num2 - num)));
			}
		}

		public int addRating
		{
			get
			{
				return newRating - oldRating;
			}
		}

		public int maxRating
		{
			get
			{
				return instance.MaxRatingInDivision(oldLeague, oldDivision);
			}
		}

		public bool leagueChanged
		{
			get
			{
				return oldLeague != newLeague;
			}
		}

		public bool divisionChanged
		{
			get
			{
				return oldDivision != newDivision;
			}
		}

		public bool isUp
		{
			get
			{
				return GetNewLeagueIndex() > GetOldLeagueIndex();
			}
		}

		public bool isDown
		{
			get
			{
				return GetNewLeagueIndex() < GetOldLeagueIndex();
			}
		}

		public RatingChange(RatingLeague currentLeague, int currentDivision, int currentRating)
		{
			oldLeague = currentLeague;
			oldDivision = currentDivision;
			oldRating = currentRating;
			newLeague = currentLeague;
			newDivision = currentDivision;
			newRating = currentRating;
		}

		public RatingChange(RatingLeague oldLeague, RatingLeague newLeague, int oldDivision, int newDivision, int oldRating, int newRating)
		{
			this.oldLeague = oldLeague;
			this.oldDivision = oldDivision;
			this.oldRating = oldRating;
			this.newLeague = newLeague;
			this.newDivision = newDivision;
			this.newRating = newRating;
		}

		public RatingChange AddChange(RatingLeague league, int division, int rating)
		{
			return new RatingChange(oldLeague, league, oldDivision, division, oldRating, rating);
		}

		private int GetNewLeagueIndex()
		{
			return (int)newLeague * 3 + newDivision;
		}

		private int GetOldLeagueIndex()
		{
			return (int)oldLeague * 3 + oldDivision;
		}
	}

	public enum RatingLeague
	{
		none = -1,
		Wood,
		Steel,
		Gold,
		Crystal,
		Ruby,
		Adamant
	}

	internal sealed class RatingLeagueComparer : IEqualityComparer<RatingLeague>
	{
		private static readonly RatingLeagueComparer s_instance = new RatingLeagueComparer();

		public static RatingLeagueComparer Instance
		{
			get
			{
				return s_instance;
			}
		}

		private RatingLeagueComparer()
		{
		}

		public bool Equals(RatingLeague x, RatingLeague y)
		{
			return x == y;
		}

		public int GetHashCode(RatingLeague obj)
		{
			return (int)obj;
		}
	}

	private const string POSITIVE_RATING_LOCAL_KEY = "RatingPositiveLocal";

	private static RatingSystem _instance;

	public static readonly string[] divisionByIndex = new string[3] { "III", "II", "I" };

	public static readonly string[] leagueChangeLocalizations = new string[6] { "Key_2139", "Key_2140", "Key_2141", "Key_2142", "Key_2143", "Key_2144" };

	public static readonly string[] leagueLocalizations = new string[6] { "Key_1953", "Key_1954", "Key_1955", "Key_1956", "Key_1957", "Key_1958" };

	private float[] winRaitingFactorByPlace = new float[5] { 1.2f, 1.1f, 1f, 0.9f, 0.8f };

	private float[] looseRaitingFactorByPlace = new float[5] { 0.8f, 0.9f, 1f, 1.1f, 1.2f };

	private float form_kd_a = 3f;

	private float form_kd_b = 3f;

	private float form_kd_top = 2f;

	private float form_place_coeff = 3f;

	private float form_hunger_a = 0.5f;

	private float form_hunger_b = 5f;

	private int[] hungerLeagueCoefs = new int[16]
	{
		40, 40, 40, 35, 35, 35, 30, 30, 30, 25,
		25, 25, 20, 20, 20, 20
	};

	private int form_min = 1;

	private int form_max = 1;

	private int[] leagueCoefs = new int[16]
	{
		50, 40, 40, 40, 30, 30, 30, 20, 20, 20,
		10, 10, 10, 5, 5, 5
	};

	private int[] leagueMCoefs = new int[16]
	{
		8, 8, 8, 6, 6, 6, 4, 4, 4, 2,
		2, 2, 1, 1, 1, 1
	};

	private int[] leagueRatings = new int[16]
	{
		0, 200, 400, 600, 800, 1000, 1200, 1400, 1600, 1800,
		2000, 2200, 2400, 2600, 2800, 3000
	};

	public RatingLeague currentLeague;

	public int currentDivision;

	public SaltedInt lastRatingChange = new SaltedInt(210674148);

	public bool ratingMatch = true;

	public static RatingSystem instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("RatingSystem");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				_instance = gameObject.AddComponent<RatingSystem>();
			}
			return _instance;
		}
	}

	private int form_hungerCoef
	{
		get
		{
			return hungerLeagueCoefs[Mathf.Min((int)currentLeague * 3 + currentDivision, hungerLeagueCoefs.Length - 1)];
		}
	}

	private int form_coef
	{
		get
		{
			return leagueCoefs[Mathf.Min((int)currentLeague * 3 + currentDivision, leagueCoefs.Length - 1)];
		}
	}

	private int form_m_coef
	{
		get
		{
			return leagueMCoefs[Mathf.Min((int)currentLeague * 3 + currentDivision, leagueMCoefs.Length - 1)];
		}
	}

	internal int TrophiesSeasonThreshold
	{
		get
		{
			return 3100;
		}
	}

	public int currentRating
	{
		get
		{
			return positiveRating - negativeRating;
		}
	}

	public RatingChange currentRatingChange
	{
		get
		{
			return new RatingChange(currentLeague, currentDivision, currentRating);
		}
	}

	public int positiveRating
	{
		get
		{
			if (Storager.hasKey("RatingPositive"))
			{
				return Storager.getInt("RatingPositive", false);
			}
			return 0;
		}
		set
		{
			Storager.setInt("RatingPositive", value, false);
		}
	}

	public int positiveRatingLocal
	{
		get
		{
			return Storager.hasKey("RatingPositiveLocal") ? Storager.getInt("RatingPositiveLocal", false) : 0;
		}
		set
		{
			Storager.setInt("RatingPositiveLocal", value, false);
		}
	}

	public int negativeRating
	{
		get
		{
			if (Storager.hasKey("RatingNegative"))
			{
				return Storager.getInt("RatingNegative", false);
			}
			return 0;
		}
		set
		{
			Storager.setInt("RatingNegative", value, false);
		}
	}

	public static event Action OnRatingUpdate;

	public int RatingNeededForLeague(RatingLeague league)
	{
		int num = 3 * (int)league;
		if (num >= leagueRatings.Length)
		{
			num = leagueRatings.Length - 1;
		}
		return leagueRatings[num];
	}

	public float GetRatingAmountForLeague(RatingLeague league)
	{
		float num = leagueRatings[Mathf.Clamp((int)league * 3, 0, leagueRatings.Length - 1)];
		float num2 = leagueRatings[Mathf.Clamp((int)(league + 1) * 3, 0, leagueRatings.Length - 1)];
		float a = 0.03f;
		if (currentRating == 0)
		{
			a = 0f;
		}
		return Mathf.Max(a, Mathf.Clamp01(((float)currentRating - num) / (num2 - num)));
	}

	public int MaxRatingInDivision(RatingLeague league, int division)
	{
		if ((int)league * 3 + division + 1 >= leagueRatings.Length)
		{
			return int.MaxValue;
		}
		return leagueRatings[(int)league * 3 + division + 1];
	}

	public int MaxRatingInLeague(RatingLeague league)
	{
		return MaxRatingInDivision(league, 2);
	}

	public int DivisionInLeague(RatingLeague league)
	{
		if (league < currentLeague)
		{
			return 2;
		}
		if (league > currentLeague)
		{
			return 0;
		}
		return currentDivision;
	}

	private void AddToRating(int rating)
	{
		if (rating > 0)
		{
			positiveRating += rating;
			positiveRatingLocal += rating;
		}
		else
		{
			negativeRating -= rating;
			negativeRating = Mathf.Min(negativeRating, positiveRating);
		}
		UpdateLeague(rating > 0);
		SaveValues();
		Debug.Log(string.Format("<color=yellow>Add {0} rating.</color>", rating));
		Debug.Log(string.Format("<color=yellow>I'm in {0} league, division: {1}. Rating: {2}</color>", currentLeague.ToString(), 3 - currentDivision, currentRating));
		TrophiesSynchronizer.Instance.Sync();
		StartCoroutine(FriendsController.sharedController.SynchRating(currentRating));
		if (RatingSystem.OnRatingUpdate != null)
		{
			RatingSystem.OnRatingUpdate();
		}
	}

	public int GetRatingValueForParams(int playersCount, int place, float matchKillrate, bool deadheat = false)
	{
		int num = ((!(matchKillrate <= form_kd_top)) ? Mathf.RoundToInt(form_kd_b) : Mathf.RoundToInt(matchKillrate * form_kd_b - form_kd_a));
		int num2 = Mathf.Max(playersCount - 1, 1);
		float num3 = ((float)num2 / 2f - (float)place) / ((float)num2 / form_place_coeff);
		int num4 = Mathf.RoundToInt((float)form_coef * num3);
		if (num4 >= 0 && num4 + num >= 0)
		{
			num4 += num;
		}
		if (num4 < 0)
		{
			num4 = Mathf.RoundToInt((float)(form_coef / form_m_coef) * num3);
		}
		if (deadheat)
		{
			num4 = num;
		}
		return num4;
	}

	public RatingChange CalculateRating(int playersCount, int place, float matchKillrate, bool deadheat = false)
	{
		RatingChange ratingChange = currentRatingChange;
		int ratingValueForParams = GetRatingValueForParams(playersCount, place, matchKillrate, deadheat);
		AddToRating(ratingValueForParams);
		ratingChange = ratingChange.AddChange(currentLeague, currentDivision, currentRating);
		lastRatingChange.Value = ratingChange.addRating;
		if (ratingChange.oldLeague != ratingChange.newLeague && ratingChange.newLeague == RatingLeague.Adamant)
		{
			TournamentAvailableBannerWindow.CanShow = true;
		}
		if (ratingChange.newLeague != RatingLeague.Adamant)
		{
			TournamentAvailableBannerWindow.CanShow = false;
		}
		return ratingChange;
	}

	public void BackupLastRatingTake()
	{
		if (lastRatingChange.Value < 0)
		{
			negativeRating += lastRatingChange.Value;
			UpdateLeague(true);
			SaveValues();
			Debug.Log(string.Format("<color=yellow>Rating backup: {0} rating.</color>", lastRatingChange.Value));
			Debug.Log(string.Format("<color=yellow>I'm in {0} league, division: {1}. Rating: {2}</color>", currentLeague.ToString(), 3 - currentDivision, currentRating));
			lastRatingChange.Value = 0;
			PlayerPrefs.SetInt("leave_from_duel_penalty", 0);
		}
	}

	private void UpdateLeague(bool up)
	{
		int num = currentRating;
		int num2 = (int)currentLeague * 3 + currentDivision;
		for (int i = (up ? num2 : 0); i < ((!up) ? (num2 + 1) : leagueRatings.Length); i++)
		{
			if (num >= leagueRatings[i] + ((!up) ? (-100) : 0))
			{
				currentLeague = (RatingLeague)Mathf.FloorToInt((float)i / 3f);
				currentDivision = i - (int)currentLeague * 3;
			}
		}
	}

	private void Update()
	{
		if (currentLeague != RatingLeague.Adamant)
		{
			currentLeague = RatingLeague.Adamant;
		}
	}

	public void UpdateLeagueEvent(object o, EventArgs arg)
	{
		UpdateLeagueByRating();
		SaveValues();
		if (RatingSystem.OnRatingUpdate != null)
		{
			RatingSystem.OnRatingUpdate();
		}
	}

	private void UpdateLeagueByRating()
	{
		int num = currentRating;
		for (int i = 0; i < leagueRatings.Length; i++)
		{
			if (num >= leagueRatings[i])
			{
				currentLeague = (RatingLeague)Mathf.FloorToInt((float)i / 3f);
				currentDivision = i - (int)currentLeague * 3;
			}
		}
	}

	public void OnGetCloudValues(int ratingPositive, int ratingNegative)
	{
		positiveRating = ratingPositive;
		negativeRating = ratingNegative;
		UpdateLeagueByRating();
		SaveValues();
	}

	private void LoadValues()
	{
		string @string = Storager.getString("RatingSystem", false);
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary != null)
		{
			if (dictionary.ContainsKey("League"))
			{
				currentLeague = (RatingLeague)Convert.ToInt32(dictionary["League"]);
			}
			if (dictionary.ContainsKey("Division"))
			{
				currentDivision = Convert.ToInt32(dictionary["Division"]);
			}
		}
	}

	private void SaveValues()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["League"] = (int)currentLeague;
		dictionary["Division"] = currentDivision;
		Storager.setString("RatingSystem", Json.Serialize(dictionary), false);
	}

	private void Awake()
	{
		LoadValues();
		ParseConfig();
		TrophiesSynchronizer.Instance.Updated += UpdateLeagueEvent;
		Storager.RatingUpdated += UpdateLeagueEvent;
	}

	public void ParseConfig()
	{
		string @string = Storager.getString("rSCKeyV2", false);
		Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
		if (dictionary == null)
		{
			return;
		}
		if (dictionary.ContainsKey("min"))
		{
			form_min = Convert.ToInt32(dictionary["min"]);
		}
		if (dictionary.ContainsKey("max"))
		{
			form_max = Convert.ToInt32(dictionary["max"]);
		}
		if (dictionary.ContainsKey("leveling"))
		{
			List<object> list = dictionary["leagueRatings"] as List<object>;
			for (int i = 0; i < leagueRatings.Length; i++)
			{
				if (list.Count > i)
				{
					leagueRatings[i] = Convert.ToInt32(list[i]);
				}
			}
		}
		if (dictionary.ContainsKey("leagueCoefs"))
		{
			List<object> list2 = dictionary["leagueCoefs"] as List<object>;
			for (int j = 0; j < leagueCoefs.Length; j++)
			{
				if (list2.Count > j)
				{
					leagueCoefs[j] = Convert.ToInt32(list2[j]);
				}
			}
		}
		if (dictionary.ContainsKey("leagueMCoefs"))
		{
			List<object> list3 = dictionary["leagueMCoefs"] as List<object>;
			for (int k = 0; k < leagueMCoefs.Length; k++)
			{
				if (list3.Count > k)
				{
					leagueMCoefs[k] = Convert.ToInt32(list3[k]);
				}
			}
		}
		if (dictionary.ContainsKey("form_kd_a"))
		{
			form_kd_a = (float)Convert.ToDouble(dictionary["form_kd_a"]);
		}
		if (dictionary.ContainsKey("form_kd_b"))
		{
			form_kd_b = Convert.ToInt32(dictionary["form_kd_b"]);
		}
		if (dictionary.ContainsKey("form_kd_top"))
		{
			form_kd_top = Convert.ToInt32(dictionary["form_kd_top"]);
		}
		if (dictionary.ContainsKey("form_place_coeff"))
		{
			form_min = Convert.ToInt32(dictionary["form_place_coeff"]);
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class EggsManager : Singleton<EggsManager>
	{
		public const string EGG_AFTER_TRAINING = "egg_Training";

		private const float UPDATE_DELAY_SECS = 1f;

		public const string EGGS_PLAYER_DATA_KEY = "player_eggs";

		private EggsData _eggsData;

		private readonly List<Egg> _eggs = new List<Egg>();

		private int _prevPositiveRating = -1;

		public static long CurrentTime
		{
			get
			{
				return FriendsController.ServerTime;
			}
		}

		public static event Action<Egg> OnReadyToUse;

		public static event Action<Egg, PetInfo> OnEggHatched;

		private void OnInstanceCreated()
		{
			_eggsData = EggsData.Load();
			if (_eggsData == null || _eggsData.Eggs.IsNullOrEmpty())
			{
				Debug.LogError("[EGGS] load static data fail");
				return;
			}
			if (Application.platform == RuntimePlatform.IPhonePlayer && !Storager.hasKey("player_eggs"))
			{
				Storager.setString("player_eggs", string.Empty, false);
			}
			string @string = Storager.getString("player_eggs", false);
			PlayerEggs playerEggs = ((!@string.IsNullOrEmpty()) ? PlayerEggs.Create(@string) : new PlayerEggs());
			PlayerEgg pData;
			foreach (PlayerEgg egg in playerEggs.Eggs)
			{
				pData = egg;
				EggData eggData = _eggsData.Eggs.FirstOrDefault((EggData d) => d.Id == pData.DataId);
				if (eggData != null)
				{
					Egg item = new Egg(eggData, pData);
					_eggs.Add(item);
				}
				else
				{
					Debug.LogErrorFormat("[EGGS] not found egg data: '{0}'", pData.DataId);
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			RatingSystem.OnRatingUpdate += RatingSystemOnRatingUpdate;
		}

		private void Start()
		{
			CoroutineRunner.Instance.StartCoroutine(WaitRatingSystem());
		}

		private void OnEnable()
		{
			StartCoroutine(UpdateEggsReadyCoroutine());
		}

		public void OnMathEnded(bool isWinner)
		{
			if (!isWinner)
			{
				return;
			}
			IEnumerable<Egg> enumerable = _eggs.Where((Egg e) => e.OnIncubator && !e.PlayerEggData.IsReady && e.HatchedType == EggHatchedType.Wins);
			foreach (Egg item in enumerable)
			{
				item.PlayerEggData.Wins++;
				if (item.CheckReady())
				{
					EggReady(item);
				}
				else
				{
					Save();
				}
			}
		}

		private IEnumerator WaitRatingSystem()
		{
			while (RatingSystem.instance == null)
			{
				yield return null;
			}
			_prevPositiveRating = RatingSystem.instance.positiveRatingLocal;
		}

		private void RatingSystemOnRatingUpdate()
		{
			if (_prevPositiveRating < 0)
			{
				return;
			}
			int num = RatingSystem.instance.positiveRatingLocal - _prevPositiveRating;
			if (num < 1)
			{
				return;
			}
			_prevPositiveRating = RatingSystem.instance.positiveRatingLocal;
			IEnumerable<Egg> enumerable = _eggs.Where((Egg e) => e.OnIncubator && !e.PlayerEggData.IsReady && e.HatchedType == EggHatchedType.Rating);
			foreach (Egg item in enumerable)
			{
				item.PlayerEggData.Rating += num;
				if (item.CheckReady())
				{
					EggReady(item);
				}
				else
				{
					Save();
				}
			}
		}

		public void AddEggsForSuperIncubator()
		{
			EggData eggData = GetEggData("SI_simple");
			AddEgg(eggData);
			EggData eggData2 = GetEggData("SI_ancient");
			AddEgg(eggData2);
			EggData eggData3 = GetEggData("SI_magical");
			AddEgg(eggData3);
		}

		public List<EggData> GetAllEggs()
		{
			return _eggsData.Eggs.ToList();
		}

		public List<Egg> GetPlayerEggs()
		{
			return _eggs.ToList();
		}

		public List<Egg> GetPlayerEggsInIncubator()
		{
			return (from e in GetPlayerEggs()
				where !e.CheckReady()
				select e).ToList();
		}

		public bool ExistsEgg(string eggId)
		{
			return _eggs.Exists((Egg e) => e.Data.Id == eggId);
		}

		public List<Egg> ReadyEggs()
		{
			return _eggs.Where((Egg e) => e.CheckReady()).ToList();
		}

		public EggData GetEggData(string eggId)
		{
			EggData eggData = _eggsData.Eggs.FirstOrDefault((EggData e) => e.Id == eggId);
			if (eggData == null)
			{
				Egg.LogFormat("data not found id: '{0}'", eggId);
			}
			return eggData;
		}

		public Egg AddEgg(string eggId)
		{
			return AddEgg(GetEggData(eggId));
		}

		public Egg AddEgg(EggData data)
		{
			if (data == null)
			{
				Egg.LogErrorFormat("egg data is null");
				return null;
			}
			if (CurrentTime < 1 && data.Id != "egg_Training" && data.HatchedType != EggHatchedType.Champion)
			{
				Egg.LogErrorFormat("server time not setted");
				return null;
			}
			int thisId = ((!_eggs.Any()) ? 1 : (_eggs.Max((Egg e) => e.PlayerEggData.Id) + 1));
			Egg egg = new Egg(data, new PlayerEgg(data.Id, thisId));
			_eggs.Add(egg);
			Egg.LogFormat("egg added '{0}'", data.Id);
			if (data.Id == "egg_Training" || data.HatchedType == EggHatchedType.Champion)
			{
				egg.PlayerEggData.IncubationStart = RiliExtensions.SystemTime;
			}
			else if (CurrentTime > 0)
			{
				egg.PlayerEggData.IncubationStart = CurrentTime;
			}
			Save();
			return egg;
		}

		public Egg AddRandomEgg()
		{
			if (!_eggsData.Eggs.Any())
			{
				return null;
			}
			EggData[] array = (from e in _eggsData.Eggs
				where e.HatchedType == EggHatchedType.Time || e.HatchedType == EggHatchedType.Rating
				where e.Rare == EggRarity.Simple || e.Rare == EggRarity.Ancient || e.Rare == EggRarity.Magical
				where e.Id != "egg_Training"
				select e).ToArray();
			int num = UnityEngine.Random.Range(0, array.Count());
			return AddEgg(array[num]);
		}

		public string Use(Egg egg)
		{
			if (egg == null)
			{
				Egg.LogErrorFormat("egg is null");
				return string.Empty;
			}
			int id = egg.Id;
			egg = _eggs.FirstOrDefault((Egg e) => e.Id == egg.Id);
			if (egg == null)
			{
				Egg.LogErrorFormat("unknown egg '{0}'", id);
				return string.Empty;
			}
			if (!egg.CheckReady())
			{
				Egg.LogErrorFormat("use fail, egg not ready", egg.Id);
				return string.Empty;
			}
			ItemDb.ItemRarity itemRarity = egg.DropPet();
			Egg.LogFormat("pet with rarity '{0}' dropped from egg type '{1}'", itemRarity, egg.Data.Id);
			PetInfo randomInfo = Singleton<PetsManager>.Instance.GetRandomInfo(itemRarity);
			_eggs.Remove(egg);
			Save();
			if (randomInfo != null)
			{
				Egg.LogFormat("[EGGS] pet dropped: '{0}'", randomInfo.Id);
				if (EggsManager.OnEggHatched != null)
				{
					EggsManager.OnEggHatched(egg, randomInfo);
				}
				return randomInfo.Id;
			}
			Egg.LogErrorFormat("[EGGS] dropped null pet. Pet rarity: {0}", itemRarity.ToString());
			return null;
		}

		private IEnumerator UpdateEggsReadyCoroutine()
		{
			while (true)
			{
				foreach (Egg egg in _eggs)
				{
					if (!egg.PlayerEggData.IsReady && egg.CheckReady())
					{
						EggReady(egg);
					}
				}
				yield return new WaitForRealSeconds(1f);
			}
		}

		private void EggReady(Egg egg)
		{
			egg.PlayerEggData.IsReady = true;
			Save();
			if (EggsManager.OnReadyToUse != null)
			{
				EggsManager.OnReadyToUse(egg);
			}
		}

		private void Save()
		{
			PlayerEggs playerEggs = new PlayerEggs();
			playerEggs.Eggs = _eggs.Select((Egg e) => e.PlayerEggData).ToList();
			string val = playerEggs.ToString();
			Storager.setString("player_eggs", val, false);
		}
	}
}

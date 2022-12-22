using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class EggData
	{
		[Tooltip("уникальный идентификатор")]
		[Header("Common settings")]
		public string Id;

		[Tooltip("Рарность яйца")]
		public EggRarity Rare;

		[Tooltip("Ключ локализации")]
		public string LKey;

		private long _secs;

		[Tooltip("яйцо вылупляется при достижении этой лиги")]
		public RatingSystem.RatingLeague League = RatingSystem.RatingLeague.none;

		private EggHatchedType? _hatchedType;

		public long Secs
		{
			get
			{
				if (BalanceController.timeEggs.ContainsKey(Id))
				{
					return (long)BalanceController.timeEggs[Id] * 60L;
				}
				return _secs;
			}
			private set
			{
				_secs = value;
			}
		}

		public int Wins
		{
			get
			{
				if (BalanceController.victoriasEggs.ContainsKey(Id))
				{
					return BalanceController.victoriasEggs[Id];
				}
				return -1;
			}
		}

		public int Rating
		{
			get
			{
				if (BalanceController.ratingEggs.ContainsKey(Id))
				{
					return BalanceController.ratingEggs[Id];
				}
				return -1;
			}
		}

		public List<EggPetInfo> Pets
		{
			get
			{
				if (BalanceController.rarityPetsInEggs.ContainsKey(Id))
				{
					return BalanceController.rarityPetsInEggs[Id];
				}
				return new List<EggPetInfo>();
			}
		}

		public EggHatchedType HatchedType
		{
			get
			{
				if (!_hatchedType.HasValue)
				{
					if (Secs > 0 || Id == "egg_Training")
					{
						_hatchedType = EggHatchedType.Time;
					}
					else if (League != RatingSystem.RatingLeague.none)
					{
						_hatchedType = EggHatchedType.League;
					}
					else if (Wins > 0)
					{
						_hatchedType = EggHatchedType.Wins;
					}
					else if (Rating > 0)
					{
						_hatchedType = EggHatchedType.Rating;
					}
					else if (Id.IndexOf("SI_") == 0)
					{
						_hatchedType = EggHatchedType.Champion;
					}
				}
				if (!_hatchedType.HasValue)
				{
					Debug.LogErrorFormat("[EGGS] : unrecognized hatched type for egg '{0}'", Id);
					_hatchedType = EggHatchedType.Time;
					Secs = 3600L;
				}
				return _hatchedType.Value;
			}
		}

		public static string LkeyForRarity(EggRarity rarity)
		{
			switch (rarity)
			{
			case EggRarity.Simple:
				return "Key_2685";
			case EggRarity.Ancient:
				return "Key_2686";
			case EggRarity.Magical:
				return "Key_2687";
			case EggRarity.Champion:
				return "Key_2688";
			default:
				Debug.LogErrorFormat("LkeyForRarity: incorrect rarity: " + rarity);
				return "Key_2685";
			}
		}
	}
}

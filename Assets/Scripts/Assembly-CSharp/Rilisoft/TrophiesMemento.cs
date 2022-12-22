using System;
using System.Globalization;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public struct TrophiesMemento : IEquatable<TrophiesMemento>
	{
		private readonly bool _conflicted;

		[SerializeField]
		private int trophiesNegative;

		[SerializeField]
		private int trophiesPositive;

		[SerializeField]
		private int season;

		public bool Conflicted
		{
			get
			{
				return _conflicted;
			}
		}

		public int TrophiesNegative
		{
			get
			{
				return trophiesNegative;
			}
		}

		public int TrophiesPositive
		{
			get
			{
				return trophiesPositive;
			}
		}

		public int Trophies
		{
			get
			{
				return trophiesPositive - trophiesNegative;
			}
		}

		public int Season
		{
			get
			{
				return season;
			}
		}

		public TrophiesMemento(int trophiesNegative, int trophiesPositive, int season)
			: this(trophiesNegative, trophiesPositive, season, false)
		{
		}

		public TrophiesMemento(int trophiesNegative, int trophiesPositive, int season, bool conflicted)
		{
			_conflicted = conflicted;
			this.trophiesNegative = trophiesNegative;
			this.trophiesPositive = trophiesPositive;
			this.season = season;
		}

		internal static TrophiesMemento Merge(TrophiesMemento left, TrophiesMemento right)
		{
			bool conflicted = left.Conflicted || right.Conflicted;
			if (left.Season == right.Season)
			{
				int num = Math.Max(left.TrophiesNegative, right.TrophiesNegative);
				int num2 = Math.Max(left.TrophiesPositive, right.TrophiesPositive);
				int num3 = left.Season;
				return new TrophiesMemento(num, num2, num3, conflicted);
			}
			TrophiesMemento trophiesMemento = ((left.Season >= right.Season) ? left : right);
			return new TrophiesMemento(trophiesMemento.TrophiesNegative, trophiesMemento.TrophiesPositive, trophiesMemento.Season, conflicted);
		}

		public bool Equals(TrophiesMemento other)
		{
			if (TrophiesNegative != other.TrophiesNegative)
			{
				return false;
			}
			if (TrophiesPositive != other.TrophiesPositive)
			{
				return false;
			}
			if (Season != other.Season)
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is TrophiesMemento))
			{
				return false;
			}
			TrophiesMemento other = (TrophiesMemento)obj;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			return TrophiesNegative.GetHashCode() ^ TrophiesPositive.GetHashCode() ^ Season.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{{ \"negative\":{0},\"positive\":{1},\"season\":{2} }}", trophiesNegative, trophiesPositive, season);
		}
	}
}

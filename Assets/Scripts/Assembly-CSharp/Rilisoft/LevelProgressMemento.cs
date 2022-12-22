using System;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	internal sealed class LevelProgressMemento : IEquatable<LevelProgressMemento>
	{
		[SerializeField]
		private string levelId;

		[SerializeField]
		private int coinCount;

		[SerializeField]
		private int gemCount;

		[SerializeField]
		private int starCount;

		internal string LevelId
		{
			get
			{
				if (levelId == null)
				{
					levelId = string.Empty;
				}
				return levelId;
			}
			set
			{
				levelId = value ?? string.Empty;
			}
		}

		internal int CoinCount
		{
			get
			{
				return coinCount;
			}
			set
			{
				coinCount = value;
			}
		}

		internal int GemCount
		{
			get
			{
				return gemCount;
			}
			set
			{
				gemCount = value;
			}
		}

		internal int StarCount
		{
			get
			{
				return starCount;
			}
			set
			{
				starCount = value;
			}
		}

		public LevelProgressMemento(string levelId)
		{
			if (levelId == null)
			{
				throw new ArgumentNullException("levelId");
			}
			this.levelId = levelId;
		}

		public bool Equals(LevelProgressMemento other)
		{
			if (other == null)
			{
				return false;
			}
			if (CoinCount != other.CoinCount)
			{
				return false;
			}
			if (GemCount != other.GemCount)
			{
				return false;
			}
			if (StarCount != other.StarCount)
			{
				return false;
			}
			if (LevelId != other.LevelId)
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			LevelProgressMemento levelProgressMemento = obj as LevelProgressMemento;
			if (object.ReferenceEquals(levelProgressMemento, null))
			{
				return false;
			}
			return Equals(levelProgressMemento);
		}

		public override int GetHashCode()
		{
			return LevelId.GetHashCode() ^ CoinCount.GetHashCode() ^ GemCount.GetHashCode() ^ StarCount.GetHashCode();
		}

		public override string ToString()
		{
			return JsonUtility.ToJson(this);
		}

		internal static LevelProgressMemento Merge(LevelProgressMemento left, LevelProgressMemento right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			if (left.LevelId != right.LevelId)
			{
				throw new ArgumentException("Level ids shoud be equal.");
			}
			LevelProgressMemento levelProgressMemento = new LevelProgressMemento(left.LevelId);
			levelProgressMemento.CoinCount = Math.Max(left.CoinCount, right.CoinCount);
			levelProgressMemento.GemCount = Math.Max(left.GemCount, right.GemCount);
			levelProgressMemento.StarCount = Math.Max(left.StarCount, right.StarCount);
			return levelProgressMemento;
		}
	}
}

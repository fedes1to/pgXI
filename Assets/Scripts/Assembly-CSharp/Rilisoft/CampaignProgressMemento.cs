using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	internal struct CampaignProgressMemento : IEquatable<CampaignProgressMemento>
	{
		[SerializeField]
		private List<LevelProgressMemento> levels;

		private bool _conflicted;

		internal bool Conflicted
		{
			get
			{
				return _conflicted;
			}
		}

		internal List<LevelProgressMemento> Levels
		{
			get
			{
				if (levels == null)
				{
					levels = new List<LevelProgressMemento>();
				}
				return levels;
			}
		}

		internal CampaignProgressMemento(bool conflicted)
		{
			levels = new List<LevelProgressMemento>();
			_conflicted = conflicted;
		}

		internal Dictionary<string, LevelProgressMemento> GetLevelsAsDictionary()
		{
			Dictionary<string, LevelProgressMemento> dictionary = new Dictionary<string, LevelProgressMemento>(Levels.Count);
			foreach (LevelProgressMemento level in Levels)
			{
				LevelProgressMemento value;
				if (dictionary.TryGetValue(level.LevelId, out value))
				{
					dictionary[value.LevelId] = LevelProgressMemento.Merge(level, value);
				}
				else
				{
					dictionary.Add(level.LevelId, level);
				}
			}
			return dictionary;
		}

		internal void SetConflicted()
		{
			_conflicted = true;
		}

		public bool Equals(CampaignProgressMemento other)
		{
			EqualityComparer<List<LevelProgressMemento>> @default = EqualityComparer<List<LevelProgressMemento>>.Default;
			if (!@default.Equals(Levels, other.Levels))
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CampaignProgressMemento))
			{
				return false;
			}
			CampaignProgressMemento other = (CampaignProgressMemento)obj;
			return Equals(other);
		}

		public override int GetHashCode()
		{
			return Levels.GetHashCode();
		}

		public override string ToString()
		{
			string[] value = Levels.Select((LevelProgressMemento l) => '"' + l.LevelId + '"').ToArray();
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", string.Join(",", value));
		}

		internal static CampaignProgressMemento Merge(CampaignProgressMemento left, CampaignProgressMemento right)
		{
			Dictionary<string, LevelProgressMemento> dictionary = new Dictionary<string, LevelProgressMemento>();
			IEnumerable<LevelProgressMemento> enumerable = from l in left.Levels.Concat(right.Levels)
				where l != null
				select l;
			foreach (LevelProgressMemento item in enumerable)
			{
				LevelProgressMemento value;
				if (dictionary.TryGetValue(item.LevelId, out value))
				{
					dictionary[item.LevelId] = LevelProgressMemento.Merge(value, item);
				}
				else
				{
					dictionary.Add(item.LevelId, item);
				}
			}
			bool conflicted = left.Conflicted || right.Conflicted;
			CampaignProgressMemento result = new CampaignProgressMemento(conflicted);
			result.Levels.AddRange(dictionary.Values);
			return result;
		}
	}
}

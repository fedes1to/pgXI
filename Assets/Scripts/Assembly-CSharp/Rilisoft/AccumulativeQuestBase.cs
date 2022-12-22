using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	public abstract class AccumulativeQuestBase : QuestBase
	{
		private readonly int _requiredCount;

		private int _currentCount;

		public int CurrentCount
		{
			get
			{
				return _currentCount;
			}
		}

		public int RequiredCount
		{
			get
			{
				return _requiredCount;
			}
		}

		public AccumulativeQuestBase(string id, long day, int slot, Difficulty difficulty, Reward reward, bool active, bool rewarded, int requiredCount, int initialCound)
			: base(id, day, slot, difficulty, reward, active, rewarded)
		{
			if (requiredCount < 1)
			{
				throw new ArgumentOutOfRangeException("requiredCount", requiredCount, "Requires at least 1.");
			}
			_requiredCount = requiredCount;
			_currentCount = Mathf.Clamp(initialCound, 0, requiredCount);
		}

		public void IncrementIf(bool condition, int count = 1)
		{
			if (condition)
			{
				decimal num = CalculateProgress();
				_currentCount = Mathf.Clamp(_currentCount + count, 0, _requiredCount);
				if (num < 1m)
				{
					SetDirty();
				}
			}
		}

		public void Increment(int count = 1)
		{
			IncrementIf(true, count);
		}

		public override decimal CalculateProgress()
		{
			return (decimal)_currentCount / (decimal)RequiredCount;
		}

		protected override void ApppendDifficultyProperties(Dictionary<string, object> difficultyProperties)
		{
			difficultyProperties["parameter"] = _requiredCount;
		}

		protected override void AppendProperties(Dictionary<string, object> properties)
		{
			properties["currentCount"] = _currentCount;
		}
	}
}

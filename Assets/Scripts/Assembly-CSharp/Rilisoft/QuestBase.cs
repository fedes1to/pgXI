using System;
using System.Collections.Generic;
using Rilisoft.MiniJson;
using Rilisoft.NullExtensions;

namespace Rilisoft
{
	public abstract class QuestBase
	{
		private readonly string _id;

		private long _day;

		private readonly int _slot;

		private readonly Difficulty _difficulty;

		private readonly Reward _reward;

		private bool _dirty;

		private bool _active;

		private bool _rewarded;

		public string Id
		{
			get
			{
				return _id;
			}
		}

		public long Day
		{
			get
			{
				return _day;
			}
		}

		public int Slot
		{
			get
			{
				return _slot;
			}
		}

		public Difficulty Difficulty
		{
			get
			{
				return _difficulty;
			}
		}

		public Reward Reward
		{
			get
			{
				return _reward;
			}
		}

		public bool Dirty
		{
			get
			{
				return _dirty;
			}
		}

		public bool Rewarded
		{
			get
			{
				return _rewarded;
			}
		}

		public event EventHandler Changed;

		public QuestBase(string id, long day, int slot, Difficulty difficulty, Reward reward, bool active, bool rewarded)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Id should not be empty.");
			}
			_id = id;
			_day = day;
			_slot = slot;
			_difficulty = difficulty;
			_reward = reward;
			_active = active;
			_rewarded = rewarded;
		}

		public void SetClean()
		{
			_dirty = false;
		}

		public void SetRewarded()
		{
			_rewarded = true;
			_dirty = true;
		}

		public bool SetActive()
		{
			if (_active)
			{
				return false;
			}
			_active = true;
			_dirty = true;
			return true;
		}

		public Dictionary<string, object> ToJson()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>(2);
			dictionary.Add("reward", Reward.ToJson());
			Dictionary<string, object> dictionary2 = dictionary;
			ApppendDifficultyProperties(dictionary2);
			dictionary = new Dictionary<string, object>(3);
			dictionary.Add("id", Id);
			dictionary.Add("day", Day);
			dictionary.Add("slot", Slot);
			dictionary.Add(QuestConstants.GetDifficultyKey(Difficulty), dictionary2);
			dictionary.Add("active", Convert.ToInt32(_active));
			dictionary.Add("rewarded", Convert.ToInt32(Rewarded));
			Dictionary<string, object> dictionary3 = dictionary;
			AppendProperties(dictionary3);
			return dictionary3;
		}

		public override string ToString()
		{
			return Json.Serialize(ToJson());
		}

		protected void SetDirty()
		{
			_dirty = true;
			this.Changed.Do(delegate(EventHandler h)
			{
				h(this, EventArgs.Empty);
			});
		}

		public abstract decimal CalculateProgress();

		protected virtual void ApppendDifficultyProperties(Dictionary<string, object> difficultyProperties)
		{
		}

		protected virtual void AppendProperties(Dictionary<string, object> properties)
		{
		}

		internal void DebugSetDay(long day)
		{
			_day = day;
			_dirty = true;
		}
	}
}

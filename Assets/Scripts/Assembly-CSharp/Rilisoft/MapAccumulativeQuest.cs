using System;
using System.Collections.Generic;

namespace Rilisoft
{
	public sealed class MapAccumulativeQuest : AccumulativeQuestBase
	{
		private readonly string _map;

		public string Map
		{
			get
			{
				return _map;
			}
		}

		public MapAccumulativeQuest(string id, long day, int slot, Difficulty difficulty, Reward reward, bool active, bool rewarded, int requiredCount, string map, int initialCount = 0)
			: base(id, day, slot, difficulty, reward, active, rewarded, requiredCount, initialCount)
		{
			if (map == null)
			{
				throw new ArgumentNullException("map");
			}
			_map = map;
		}

		protected override void AppendProperties(Dictionary<string, object> properties)
		{
			base.AppendProperties(properties);
			properties["map"] = _map;
		}
	}
}

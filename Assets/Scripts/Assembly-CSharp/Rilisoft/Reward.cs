using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	public struct Reward
	{
		public int Coins { get; set; }

		public int Gems { get; set; }

		public int Experience { get; set; }

		public static Reward Create(Dictionary<string, object> reward)
		{
			Reward result = default(Reward);
			if (reward == null)
			{
				return result;
			}
			try
			{
				object value;
				if (reward.TryGetValue("coins", out value))
				{
					result.Coins = Convert.ToInt32(value);
				}
				object value2;
				if (reward.TryGetValue("gems", out value2))
				{
					result.Gems = Convert.ToInt32(value2);
				}
				object value3;
				if (reward.TryGetValue("xp", out value3))
				{
					result.Experience = Convert.ToInt32(value3);
					return result;
				}
				return result;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return result;
			}
		}

		public static Reward Create(List<object> reward)
		{
			Reward result = default(Reward);
			if (reward == null)
			{
				return result;
			}
			try
			{
				for (int i = 0; i != Math.Max(reward.Count, 3); i++)
				{
					int num = Convert.ToInt32(reward[i]);
					switch (i)
					{
					case 0:
						result.Coins = num;
						break;
					case 1:
						result.Gems = num;
						break;
					case 2:
						result.Experience = num;
						break;
					}
				}
				return result;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return result;
			}
		}

		public List<int> ToJson()
		{
			List<int> list = new List<int>(3);
			list.Add(Coins);
			list.Add(Gems);
			list.Add(Experience);
			return list;
		}
	}
}

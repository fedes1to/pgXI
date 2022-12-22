using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class AchievementProgressData
	{
		public int AchievementId;

		public int Points;

		public int Stage;

		public Dictionary<string, object> CustomData;

		public AchievementProgressData()
		{
			CustomData = new Dictionary<string, object>();
		}

		public override bool Equals(object obj)
		{
			AchievementProgressData o = obj as AchievementProgressData;
			if (o == null)
			{
				return false;
			}
			if (AchievementId != o.AchievementId || Points != o.Points || Stage != o.Stage)
			{
				return false;
			}
			if (CustomData == null && o.CustomData == null)
			{
				return true;
			}
			if ((CustomData == null && o.CustomData != null) || (CustomData != null && o.CustomData == null))
			{
				return false;
			}
			if (CustomData.Keys.Count != o.CustomData.Keys.Count)
			{
				return false;
			}
			if (!CustomData.Keys.All((string k) => o.CustomData.Keys.Contains(k)) || !o.CustomData.Keys.All((string k) => CustomData.Keys.Contains(k)))
			{
				return false;
			}
			int num = 0;
			List<string> list = o.CustomData.Keys.ToList();
			foreach (KeyValuePair<string, object> customDatum in CustomData)
			{
				object value = customDatum.Value;
				object obj2 = o.CustomData[list[num]];
				if (value != null || obj2 != null)
				{
					if ((value == null && obj2 != null) || (value != null && obj2 == null))
					{
						return false;
					}
					if (customDatum.Value.ToString() != obj2.ToString())
					{
						return false;
					}
					num++;
				}
			}
			return true;
		}

		public static AchievementProgressData Create(string raw)
		{
			if (!raw.IsNullOrEmpty())
			{
				Dictionary<string, object> obj = Json.Deserialize(raw) as Dictionary<string, object>;
				return Create(obj);
			}
			return null;
		}

		public static AchievementProgressData Create(Dictionary<string, object> obj)
		{
			if (obj == null)
			{
				return null;
			}
			AchievementProgressData achievementProgressData = new AchievementProgressData();
			achievementProgressData.AchievementId = obj.ParseJSONField("id", ConvertToInt32Invariant);
			achievementProgressData.Points = obj.ParseJSONField("p", ConvertToInt32Invariant);
			achievementProgressData.Stage = obj.ParseJSONField("s", ConvertToInt32Invariant);
			achievementProgressData.CustomData = obj.ParseJSONField("cd", (object o) => o as Dictionary<string, object>, true) ?? new Dictionary<string, object>();
			return achievementProgressData;
		}

		private static int ConvertToInt32Invariant(object o)
		{
			return Convert.ToInt32(o, CultureInfo.InvariantCulture);
		}

		public static List<AchievementProgressData> ParseAll(string raw)
		{
			if (raw.IsNullOrEmpty())
			{
				return new List<AchievementProgressData>(0);
			}
			List<object> list = Json.Deserialize(raw) as List<object>;
			if (list == null)
			{
				Debug.LogError("[Achievements] parse progresses error");
				return new List<AchievementProgressData>(0);
			}
			return (from d in list.OfType<Dictionary<string, object>>()
				select Create(d)).ToList();
		}

		public Dictionary<string, object> ObjectForSave()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("id", AchievementId);
			dictionary.Add("p", Points);
			dictionary.Add("s", Stage);
			Dictionary<string, object> dictionary2 = dictionary;
			if (CustomData != null && CustomData.Any())
			{
				dictionary2.Add("cd", CustomData);
			}
			return dictionary2;
		}

		public static string Merge(string rawListOne, string rawListTwo)
		{
			List<AchievementProgressData> progressData = ParseAll(rawListOne);
			List<AchievementProgressData> progressData2 = ParseAll(rawListTwo);
			AchievementProgressSyncObject achievementProgressSyncObject = Merge(new AchievementProgressSyncObject(progressData), new AchievementProgressSyncObject(progressData2));
			List<Dictionary<string, object>> obj = achievementProgressSyncObject.ProgressData.Select((AchievementProgressData o) => o.ObjectForSave()).ToList();
			return Json.Serialize(obj);
		}

		public static AchievementProgressSyncObject Merge(AchievementProgressSyncObject one, AchievementProgressSyncObject two)
		{
			AchievementProgressSyncObject res = new AchievementProgressSyncObject();
			AchievementProgressData item;
			foreach (AchievementProgressData progressDatum in one.ProgressData)
			{
				item = progressDatum;
				AchievementProgressData achievementProgressData = two.ProgressData.FirstOrDefault((AchievementProgressData i) => i.AchievementId == item.AchievementId);
				if (achievementProgressData != null)
				{
					res.ProgressData.Add(Merge(item, achievementProgressData));
				}
				else
				{
					res.ProgressData.Add(item);
				}
			}
			IEnumerable<AchievementProgressData> collection = two.ProgressData.Where((AchievementProgressData i) => res.ProgressData.All((AchievementProgressData ei) => ei.AchievementId != i.AchievementId));
			res.ProgressData.AddRange(collection);
			Achievement.LogMsg("[MERGE] [ONE] " + AchievementProgressSyncObject.ToJson(one));
			Achievement.LogMsg("[MERGE] [TWO] " + AchievementProgressSyncObject.ToJson(two));
			Achievement.LogMsg("[MERGE] [RES] " + AchievementProgressSyncObject.ToJson(res));
			return res;
		}

		public static AchievementProgressData Merge(AchievementProgressData one, AchievementProgressData two)
		{
			if (one == null)
			{
				return two;
			}
			if (two == null)
			{
				return one;
			}
			AchievementProgressData achievementProgressData = null;
			achievementProgressData = ((one.Stage != two.Stage) ? ((one.Stage <= two.Stage) ? two : one) : ((one.Points == two.Points) ? one : ((one.Points <= two.Points) ? two : one)));
			if (one.CustomData == null || two.CustomData == null)
			{
				achievementProgressData.CustomData = ((one.CustomData == null) ? two.CustomData : one.CustomData);
			}
			else
			{
				List<KeyValuePair<string, object>> all = new List<KeyValuePair<string, object>>();
				one.CustomData.ForEach(delegate(KeyValuePair<string, object> kvp)
				{
					all.Add(kvp);
				});
				two.CustomData.ForEach(delegate(KeyValuePair<string, object> kvp)
				{
					all.Add(kvp);
				});
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (all.Any())
				{
					foreach (KeyValuePair<string, object> item in all)
					{
						string key = item.Key;
						if (dictionary.Keys.Contains(key))
						{
							continue;
						}
						KeyValuePair<string, object>[] array = all.Where((KeyValuePair<string, object> k) => k.Key == key).ToArray();
						if (array.Count() < 2)
						{
							dictionary.Add(array[0].Key, array[0].Value);
							continue;
						}
						object value = array[0].Value;
						object value2 = array[1].Value;
						if (value == null || value2 == null)
						{
							dictionary.Add(key, value ?? value2);
							continue;
						}
						Type type = value.GetType();
						Type type2 = value2.GetType();
						if (type != type2)
						{
							dictionary.Add(key, value);
						}
						else if (value is double)
						{
							dictionary.Add(key, (!((double)value > (double)value2)) ? value2 : value);
						}
						else if (value is long)
						{
							dictionary.Add(key, ((long)value <= (long)value2) ? value2 : value);
						}
						else if (value is int)
						{
							dictionary.Add(key, ((int)value <= (int)value2) ? value2 : value);
						}
						else if (value is string)
						{
							dictionary.Add(key, (value.ToString().Length <= value2.ToString().Length) ? value2 : value);
						}
						else if (value is bool)
						{
							dictionary.Add(key, (bool)value || (bool)value2);
						}
						else
						{
							dictionary.Add(key, value);
						}
					}
					achievementProgressData.CustomData = dictionary;
				}
			}
			return achievementProgressData;
		}
	}
}

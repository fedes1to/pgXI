using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	public class AchievementProgressSyncObject
	{
		[SerializeField]
		private readonly List<AchievementProgressData> _progressData;

		private bool _conflicted;

		public List<AchievementProgressData> ProgressData
		{
			get
			{
				return _progressData;
			}
		}

		internal bool Conflicted
		{
			get
			{
				return _conflicted;
			}
		}

		public AchievementProgressSyncObject()
			: this(new List<AchievementProgressData>())
		{
		}

		public AchievementProgressSyncObject(List<AchievementProgressData> progressData)
		{
			_progressData = progressData ?? new List<AchievementProgressData>();
		}

		public static AchievementProgressSyncObject FromJson(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				return new AchievementProgressSyncObject();
			}
			Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			if (dictionary == null)
			{
				return new AchievementProgressSyncObject();
			}
			object value;
			if (!dictionary.TryGetValue("progressData", out value))
			{
				return new AchievementProgressSyncObject();
			}
			List<object> list = value as List<object>;
			if (list == null)
			{
				return new AchievementProgressSyncObject();
			}
			List<AchievementProgressData> list2 = new List<AchievementProgressData>();
			foreach (object item2 in list)
			{
				Dictionary<string, object> dictionary2 = item2 as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					try
					{
						AchievementProgressData item = AchievementProgressData.Create(dictionary2);
						list2.Add(item);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
			}
			return new AchievementProgressSyncObject(list2);
		}

		public static string ToJson(AchievementProgressSyncObject memento)
		{
			if (memento == null)
			{
				return string.Empty;
			}
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			foreach (AchievementProgressData progressDatum in memento.ProgressData)
			{
				Dictionary<string, object> item = progressDatum.ObjectForSave();
				list.Add(item);
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("progressData", list);
			Dictionary<string, object> obj = dictionary;
			return Json.Serialize(obj);
		}

		internal void SetConflicted()
		{
			_conflicted = true;
		}

		public override bool Equals(object obj)
		{
			AchievementProgressSyncObject o = obj as AchievementProgressSyncObject;
			if (o == null)
			{
				return false;
			}
			if (ProgressData == null && o.ProgressData == null)
			{
				return true;
			}
			if ((ProgressData == null && o.ProgressData != null) || (ProgressData != null && o.ProgressData == null))
			{
				return false;
			}
			if (!ProgressData.All((AchievementProgressData d) => o.ProgressData.Contains(d)) || !o.ProgressData.All((AchievementProgressData k) => ProgressData.Contains(k)))
			{
				return false;
			}
			int num = 0;
			foreach (AchievementProgressData progressDatum in ProgressData)
			{
				AchievementProgressData obj2 = o.ProgressData[num];
				if (!progressDatum.Equals(obj2))
				{
					return false;
				}
				num++;
			}
			return true;
		}
	}
}

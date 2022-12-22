using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft.DictionaryExtensions;
using Rilisoft.MiniJson;
using Rilisoft.NullExtensions;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class TutorialQuestManager
	{
		[Serializable]
		internal struct Memento
		{
			public List<string> fulfilledQuests;

			public bool received;
		}

		private const string Key = "TutorialQuestManager";

		private static readonly Lazy<TutorialQuestManager> _instance = new Lazy<TutorialQuestManager>(Create);

		private bool _dirty;

		private readonly HashSet<string> _fulfilledQuests;

		private bool _received;

		public static TutorialQuestManager Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		public bool Received
		{
			get
			{
				return _received;
			}
		}

		private TutorialQuestManager()
		{
			_fulfilledQuests = new HashSet<string>();
		}

		private TutorialQuestManager(Memento dto)
		{
			Debug.Log("> TutorialQuestManager.TutorialQuestManager()");
			try
			{
				_fulfilledQuests = ((dto.fulfilledQuests == null) ? new HashSet<string>() : new HashSet<string>(dto.fulfilledQuests));
				_received = dto.received;
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("TutorialQuestManager.TutorialQuestManager(): Exception caught: {0}", ex.GetType().Name);
				Debug.LogException(ex);
			}
			finally
			{
				Debug.Log("< TutorialQuestManager.TutorialQuestManager()");
			}
		}

		public override string ToString()
		{
			Memento memento = default(Memento);
			memento.fulfilledQuests = _fulfilledQuests.ToList();
			memento.received = _received;
			Memento memento2 = memento;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("fulfilledQuests", _fulfilledQuests.ToList());
			dictionary.Add("received", Convert.ToBoolean(_received));
			Dictionary<string, object> obj = dictionary;
			return Json.Serialize(obj);
		}

		public void AddFulfilledQuest(string questId)
		{
			if (questId != null)
			{
				_dirty = _fulfilledQuests.Add(questId);
			}
		}

		public void SetReceived()
		{
			_received = true;
			_dirty = true;
		}

		public bool CheckQuestIfFulfilled(string questId)
		{
			if (questId == null)
			{
				return false;
			}
			if (_fulfilledQuests.Contains(questId))
			{
				return true;
			}
			if (Storager.getInt(Defs.TrainingCompleted_4_4_Sett, false) == 1)
			{
				return true;
			}
			switch (questId)
			{
			case "loginFacebook":
				if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
				{
					return true;
				}
				return Storager.hasKey(Defs.IsFacebookLoginRewardaGained) && Storager.getInt(Defs.IsFacebookLoginRewardaGained, true) == 1;
			case "loginTwitter":
				if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
				{
					return true;
				}
				return Application.isEditor;
			case "likeFacebook":
				if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
				{
					return true;
				}
				return Application.isEditor;
			default:
				return false;
			}
		}

		public void SaveIfDirty()
		{
			if (_dirty)
			{
				string val = ToString();
				Storager.setString("TutorialQuestManager", val, false);
				_dirty = false;
			}
		}

		public void FillTutorialQuests(IList<object> inputJsons, long day, IList<QuestBase> outputQuests)
		{
			if (inputJsons == null || outputQuests == null)
			{
				return;
			}
			foreach (object inputJson in inputJsons)
			{
				if (inputJson == null)
				{
					continue;
				}
				Dictionary<string, object> dictionary = inputJson as Dictionary<string, object>;
				if (dictionary == null)
				{
					Debug.LogWarningFormat("Skipping bad quest: {0}", Json.Serialize(inputJson));
					continue;
				}
				QuestBase questBase = CreateQuestFromJson(dictionary, day);
				if (questBase != null && !questBase.Rewarded && (!CheckQuestIfFulfilled(questBase.Id) || !(questBase.CalculateProgress() < 1m)))
				{
					outputQuests.Add(questBase);
				}
			}
		}

		private static TutorialQuestManager Create()
		{
			//Discarded unreachable code: IL_00f1, IL_012a
			try
			{
				if (!Storager.hasKey("TutorialQuestManager"))
				{
					Storager.setString("TutorialQuestManager", "{}", false);
				}
				string text = Storager.getString("TutorialQuestManager", false);
				if (string.IsNullOrEmpty(text))
				{
					text = "{}";
				}
				Debug.LogFormat("TutorialQuestManager.Create(): parsing data transfer object: {0}", text);
				Memento memento = default(Memento);
				memento.fulfilledQuests = new List<string>();
				Memento dto = memento;
				Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
				if (dictionary != null)
				{
					object value;
					if (dictionary.TryGetValue("fulfilledQuests", out value))
					{
						List<object> list = value as List<object>;
						dto.fulfilledQuests = ((list == null) ? new List<string>() : list.OfType<string>().ToList());
					}
					object value2;
					if (dictionary.TryGetValue("received", out value2))
					{
						dto.received = Convert.ToBoolean(value2);
					}
				}
				Debug.Log("TutorialQuestManager.Create(): data transfer object parsed.");
				return new TutorialQuestManager(dto);
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("TutorialQuestManager.Create(): Exception caught: {0}", ex.GetType().Name);
				Debug.LogException(ex);
				return new TutorialQuestManager();
			}
		}

		private static QuestBase CreateQuestFromJson(Dictionary<string, object> questJson, long day)
		{
			//Discarded unreachable code: IL_01c2, IL_01eb
			if (questJson == null)
			{
				throw new ArgumentNullException("questJson");
			}
			try
			{
				string text = questJson.TryGet("id") as string;
				if (text == null)
				{
					Debug.LogWarningFormat("Failed to create quest, id = null: {0}", Json.Serialize(questJson));
					return null;
				}
				int slot = Convert.ToInt32(questJson.TryGet("slot") ?? ((object)0));
				Difficulty[] array = new Difficulty[3]
				{
					Difficulty.Easy,
					Difficulty.Normal,
					Difficulty.Hard
				};
				Difficulty difficulty = Difficulty.None;
				Dictionary<string, object> dictionary = null;
				Difficulty[] array2 = array;
				foreach (Difficulty difficulty2 in array2)
				{
					string difficultyKey = QuestConstants.GetDifficultyKey(difficulty2);
					object value;
					if (questJson.TryGetValue(difficultyKey, out value))
					{
						difficulty = difficulty2;
						dictionary = value as Dictionary<string, object>;
						break;
					}
				}
				if (dictionary == null)
				{
					Debug.LogWarningFormat("Failed to create quest, difficulty = null: {0}", Json.Serialize(questJson));
					return null;
				}
				Reward reward = Reward.Create(dictionary.TryGet("reward") as List<object>);
				int requiredCount = Convert.ToInt32(dictionary.TryGet("parameter") ?? ((object)1));
				int initialCount = questJson.TryGet("currentCount").Map(Convert.ToInt32);
				day = questJson.TryGet("day").Map(Convert.ToInt64, day);
				bool rewarded = questJson.TryGet("rewarded").Map(Convert.ToBoolean);
				bool active = questJson.TryGet("active").Map(Convert.ToBoolean);
				return new SimpleAccumulativeQuest(text, day, slot, difficulty, reward, active, rewarded, requiredCount, initialCount);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Caught exception while creating quest object: {0}", ex.Message);
				return null;
			}
		}
	}
}

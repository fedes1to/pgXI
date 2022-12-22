using System;
using System.Collections.Generic;

namespace Rilisoft
{
	[Serializable]
	internal sealed class ChestInLobbyPointMemento : AdPointMementoBase
	{
		private readonly List<double> _rewardedVideoDelayMinutes = new List<double>();

		public static int DefaultAward
		{
			get
			{
				return 1;
			}
		}

		public static string DefaultCurrency
		{
			get
			{
				return "Coins";
			}
		}

		public static bool DefaultSimplifiedInterface
		{
			get
			{
				return false;
			}
		}

		public int Award { get; private set; }

		public string AwardCurrency { get; private set; }

		public bool SimplifiedInterface { get; private set; }

		public List<double> RewardedVideoDelayMinutes
		{
			get
			{
				return _rewardedVideoDelayMinutes;
			}
		}

		public ChestInLobbyPointMemento(string id)
			: base(id)
		{
			Award = DefaultAward;
			AwardCurrency = DefaultCurrency;
			SimplifiedInterface = DefaultSimplifiedInterface;
		}

		public int GetFinalAward(string category)
		{
			int? int32Override = GetInt32Override("award", category);
			if (int32Override.HasValue)
			{
				return int32Override.Value;
			}
			return Award;
		}

		public string GetFinalAwardCurrency(string category)
		{
			string stringOverride = GetStringOverride("awardCurrency", category);
			if (stringOverride != null)
			{
				return stringOverride;
			}
			return AwardCurrency;
		}

		public bool GetFinalSimplifiedInterface(string category)
		{
			bool? booleanOverride = GetBooleanOverride("simplifiedInterface", category);
			if (booleanOverride.HasValue)
			{
				return booleanOverride.Value;
			}
			return SimplifiedInterface;
		}

		public List<double> GetFinalRewardedVideoDelayMinutes(string category)
		{
			//Discarded unreachable code: IL_0050
			List<object> list = GetNodeObjectOverride("rewardedVideoDelayMinutes", category) as List<object>;
			if (list == null)
			{
				return RewardedVideoDelayMinutes;
			}
			List<double> list2 = new List<double>();
			foreach (object item in list)
			{
				try
				{
					list2.Add(Convert.ToDouble(item));
				}
				catch
				{
				}
			}
			return list2;
		}

		internal static ChestInLobbyPointMemento FromObject(string id, object obj)
		{
			//Discarded unreachable code: IL_00eb
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (obj == null)
			{
				return null;
			}
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return null;
			}
			ChestInLobbyPointMemento chestInLobbyPointMemento = new ChestInLobbyPointMemento(id);
			chestInLobbyPointMemento.Reset(dictionary);
			int? @int = ParsingHelper.GetInt32(dictionary, "award");
			if (@int.HasValue)
			{
				chestInLobbyPointMemento.Award = @int.Value;
			}
			string @string = ParsingHelper.GetString(dictionary, "awardCurrency");
			if (@string != null)
			{
				chestInLobbyPointMemento.AwardCurrency = @string;
			}
			bool? boolean = ParsingHelper.GetBoolean(dictionary, "simplifiedInterface");
			if (boolean.HasValue)
			{
				chestInLobbyPointMemento.SimplifiedInterface = boolean.Value;
			}
			List<object> list = ParsingHelper.GetObject(dictionary, "rewardedVideoDelayMinutes") as List<object>;
			if (list != null)
			{
				foreach (object item2 in list)
				{
					try
					{
						double item = Convert.ToDouble(item2);
						chestInLobbyPointMemento.RewardedVideoDelayMinutes.Add(item);
					}
					catch
					{
					}
				}
				return chestInLobbyPointMemento;
			}
			return chestInLobbyPointMemento;
		}
	}
}

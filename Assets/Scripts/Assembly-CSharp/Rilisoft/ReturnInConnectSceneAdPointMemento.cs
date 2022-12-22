using System;
using System.Collections.Generic;

namespace Rilisoft
{
	[Serializable]
	internal sealed class ReturnInConnectSceneAdPointMemento : AdPointMementoBase
	{
		public double DelayInSeconds { get; private set; }

		public double MinInGameTimePerDayInMinutes { get; private set; }

		public int ImpressionMaxCountPerDay { get; private set; }

		public ReturnInConnectSceneAdPointMemento(string id)
			: base(id)
		{
		}

		public double GetFinalDelayInSeconds(string category)
		{
			double? delayInSecondsOverride = GetDelayInSecondsOverride(category);
			if (delayInSecondsOverride.HasValue)
			{
				return delayInSecondsOverride.Value;
			}
			return DelayInSeconds;
		}

		public double GetFinalMinInGameTimePerDayInMinutes(string category)
		{
			double? minInGameTimePerDayInMinutesOverride = GetMinInGameTimePerDayInMinutesOverride(category);
			if (minInGameTimePerDayInMinutesOverride.HasValue)
			{
				return minInGameTimePerDayInMinutesOverride.Value;
			}
			return MinInGameTimePerDayInMinutes;
		}

		public int GetFinalImpressionMaxCountPerDay(string category)
		{
			int? impressionMaxCountPerDayOverride = GetImpressionMaxCountPerDayOverride(category);
			if (impressionMaxCountPerDayOverride.HasValue)
			{
				return impressionMaxCountPerDayOverride.Value;
			}
			return ImpressionMaxCountPerDay;
		}

		private double? GetDelayInSecondsOverride(string category)
		{
			return GetDoubleOverride("delaySeconds", category);
		}

		private double? GetMinInGameTimePerDayInMinutesOverride(string category)
		{
			return GetDoubleOverride("minInGameTimePerDayMinutes", category);
		}

		private int? GetImpressionMaxCountPerDayOverride(string category)
		{
			return GetInt32Override("impressionMaxCountPerDay", category);
		}

		internal static ReturnInConnectSceneAdPointMemento FromObject(string id, object obj)
		{
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
			ReturnInConnectSceneAdPointMemento returnInConnectSceneAdPointMemento = new ReturnInConnectSceneAdPointMemento(id);
			returnInConnectSceneAdPointMemento.Reset(dictionary);
			double? @double = ParsingHelper.GetDouble(dictionary, "delaySeconds");
			if (@double.HasValue)
			{
				returnInConnectSceneAdPointMemento.DelayInSeconds = @double.Value;
			}
			double? double2 = ParsingHelper.GetDouble(dictionary, "minInGameTimePerDayMinutes");
			if (double2.HasValue)
			{
				returnInConnectSceneAdPointMemento.MinInGameTimePerDayInMinutes = double2.Value;
			}
			int? @int = ParsingHelper.GetInt32(dictionary, "impressionMaxCountPerDay");
			if (@int.HasValue)
			{
				returnInConnectSceneAdPointMemento.ImpressionMaxCountPerDay = @int.Value;
			}
			return returnInConnectSceneAdPointMemento;
		}
	}
}

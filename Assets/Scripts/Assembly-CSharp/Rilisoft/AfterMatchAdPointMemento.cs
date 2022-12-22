using System;
using System.Collections.Generic;

namespace Rilisoft
{
	[Serializable]
	internal sealed class AfterMatchAdPointMemento : AdPointMementoBase
	{
		public bool EnabledForWinner { get; private set; }

		public bool EnabledForLoser { get; private set; }

		public double MatchMinDurationInMinutes { get; private set; }

		public AfterMatchAdPointMemento(string id)
			: base(id)
		{
		}

		internal bool? GetEnabledForWinnerOverride(string category)
		{
			return GetBooleanOverride("enabledForWinner", category);
		}

		internal bool? GetEnabledForLoserOverride(string category)
		{
			return GetBooleanOverride("enabledForLoser", category);
		}

		internal double? GetMatchMinDurationInMinutesOverride(string category)
		{
			return GetDoubleOverride("matchMinDurationMinutes", category);
		}

		internal static AfterMatchAdPointMemento FromObject(string id, object obj)
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
			AfterMatchAdPointMemento afterMatchAdPointMemento = new AfterMatchAdPointMemento(id);
			afterMatchAdPointMemento.Reset(dictionary);
			bool? boolean = ParsingHelper.GetBoolean(dictionary, "enabledForWinner");
			if (boolean.HasValue)
			{
				afterMatchAdPointMemento.EnabledForWinner = boolean.Value;
			}
			bool? boolean2 = ParsingHelper.GetBoolean(dictionary, "enabledForLoser");
			if (boolean2.HasValue)
			{
				afterMatchAdPointMemento.EnabledForLoser = boolean2.Value;
			}
			double? @double = ParsingHelper.GetDouble(dictionary, "matchMinDurationMinutes");
			if (@double.HasValue)
			{
				afterMatchAdPointMemento.MatchMinDurationInMinutes = @double.Value;
			}
			return afterMatchAdPointMemento;
		}
	}
}

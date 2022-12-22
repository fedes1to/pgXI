using System;
using System.Collections.Generic;

namespace Rilisoft
{
	[Serializable]
	internal sealed class FreeSpinPointMemento : AdPointMementoBase
	{
		public static double DefaultTimeoutBetweenShowInMinutes
		{
			get
			{
				return 1440.0;
			}
		}

		public double TimeoutBetweenShowInMinutes { get; private set; }

		public FreeSpinPointMemento(string id)
			: base(id)
		{
			TimeoutBetweenShowInMinutes = DefaultTimeoutBetweenShowInMinutes;
		}

		public double GetFinalTimeoutBetweenShowInMinutes(string category)
		{
			double? doubleOverride = GetDoubleOverride("timeoutBetweenShowMinutes", category);
			if (doubleOverride.HasValue)
			{
				return doubleOverride.Value;
			}
			return TimeoutBetweenShowInMinutes;
		}

		internal static FreeSpinPointMemento FromObject(string id, object obj)
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
			FreeSpinPointMemento freeSpinPointMemento = new FreeSpinPointMemento(id);
			freeSpinPointMemento.Reset(dictionary);
			double? @double = ParsingHelper.GetDouble(dictionary, "timeoutBetweenShowMinutes");
			if (@double.HasValue)
			{
				freeSpinPointMemento.TimeoutBetweenShowInMinutes = @double.Value;
			}
			return freeSpinPointMemento;
		}
	}
}

using System;
using System.Collections.Generic;

namespace Rilisoft
{
	[Serializable]
	internal abstract class LevelCompleteAdPointMementoBase : AdPointMementoBase
	{
		public bool Quit { get; private set; }

		public bool Death { get; private set; }

		public LevelCompleteAdPointMementoBase(string id)
			: base(id)
		{
		}

		public bool? GetQuitOverride(string category)
		{
			return GetBooleanOverride("quit", category);
		}

		public bool? GetDeathOverride(string category)
		{
			return GetBooleanOverride("death", category);
		}

		protected new void Reset(Dictionary<string, object> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			base.Reset(dictionary);
			bool? boolean = ParsingHelper.GetBoolean(dictionary, "quit");
			if (boolean.HasValue)
			{
				Quit = boolean.Value;
			}
			bool? boolean2 = ParsingHelper.GetBoolean(dictionary, "death");
			if (boolean2.HasValue)
			{
				Death = boolean2.Value;
			}
		}
	}
}

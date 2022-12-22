using System;
using System.Collections.Generic;

namespace Rilisoft
{
	[Serializable]
	internal sealed class SurvivalArenaAdPointMemento : LevelCompleteAdPointMementoBase
	{
		public int WaveMinCount { get; private set; }

		public SurvivalArenaAdPointMemento(string id)
			: base(id)
		{
		}

		public int? GetWaveMinCountOverride(string category)
		{
			return GetInt32Override("waveMinCount", category);
		}

		internal static SurvivalArenaAdPointMemento FromObject(string id, object obj)
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
			SurvivalArenaAdPointMemento survivalArenaAdPointMemento = new SurvivalArenaAdPointMemento(id);
			survivalArenaAdPointMemento.Reset(dictionary);
			int? @int = ParsingHelper.GetInt32(dictionary, "waveMinCount");
			if (@int.HasValue)
			{
				survivalArenaAdPointMemento.WaveMinCount = @int.Value;
			}
			return survivalArenaAdPointMemento;
		}
	}
}

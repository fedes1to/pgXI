using System;
using System.Collections.Generic;

namespace Rilisoft
{
	[Serializable]
	internal sealed class PolygonAdPointMemento : AdPointMementoBase
	{
		public int EntryCount { get; private set; }

		public PolygonAdPointMemento(string id)
			: base(id)
		{
		}

		public int? GetEntryCountOverride(string category)
		{
			return GetInt32Override("entryCount", category);
		}

		internal static PolygonAdPointMemento FromObject(string id, object obj)
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
			PolygonAdPointMemento polygonAdPointMemento = new PolygonAdPointMemento(id);
			polygonAdPointMemento.Reset(dictionary);
			int? @int = ParsingHelper.GetInt32(dictionary, "entryCount");
			if (@int.HasValue)
			{
				polygonAdPointMemento.EntryCount = @int.Value;
			}
			return polygonAdPointMemento;
		}
	}
}

using System.Collections.Generic;

namespace Rilisoft.DictionaryExtensions
{
	internal static class DictionaryEx
	{
		internal static object TryGet(this Dictionary<string, object> dictionary, string key)
		{
			if (dictionary == null || key == null)
			{
				return null;
			}
			object value = null;
			dictionary.TryGetValue(key, out value);
			return value;
		}
	}
}

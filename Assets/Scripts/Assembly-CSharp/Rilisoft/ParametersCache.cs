using System;
using System.Collections.Generic;

namespace Rilisoft
{
	internal static class ParametersCache
	{
		private const int MaxCapacity = 10;

		[ThreadStatic]
		private static Dictionary<string, object> s_cachedObjectDictionary;

		public static Dictionary<string, object> Acquire(int capacity = 1)
		{
			if (capacity > 10)
			{
				return new Dictionary<string, object>(capacity);
			}
			Dictionary<string, object> dictionary = s_cachedObjectDictionary;
			if (dictionary == null)
			{
				return new Dictionary<string, object>(capacity);
			}
			s_cachedObjectDictionary = null;
			dictionary.Clear();
			return dictionary;
		}

		public static void Release(Dictionary<string, object> d)
		{
			if (d != null)
			{
				if (d.Count > 10)
				{
					d.Clear();
					return;
				}
				d.Clear();
				s_cachedObjectDictionary = d;
			}
		}
	}
}

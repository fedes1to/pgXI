using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class ParsingHelper
	{
		internal static object GetObject(Dictionary<string, object> dictionary, string key)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			object value;
			if (!dictionary.TryGetValue(key, out value))
			{
				return null;
			}
			return value;
		}

		internal static bool? GetBoolean(Dictionary<string, object> dictionary, string key)
		{
			//Discarded unreachable code: IL_0029, IL_0060
			object @object = GetObject(dictionary, key);
			if (@object == null)
			{
				return null;
			}
			try
			{
				return Convert.ToBoolean(@object);
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as bool. {2}", key, @object, ex.Message);
				return null;
			}
		}

		internal static int? GetInt32(Dictionary<string, object> dictionary, string key)
		{
			//Discarded unreachable code: IL_0029, IL_0060
			object @object = GetObject(dictionary, key);
			if (@object == null)
			{
				return null;
			}
			try
			{
				return Convert.ToInt32(@object);
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as int. {2}", key, @object, ex.Message);
				return null;
			}
		}

		internal static double? GetDouble(Dictionary<string, object> dictionary, string key)
		{
			//Discarded unreachable code: IL_0029, IL_0060
			object @object = GetObject(dictionary, key);
			if (@object == null)
			{
				return null;
			}
			try
			{
				return Convert.ToDouble(@object);
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("Failed to interpret `\"{0}\": {1}` as double. {2}", key, @object, ex.Message);
				return null;
			}
		}

		internal static string GetString(Dictionary<string, object> dictionary, string key)
		{
			object @object = GetObject(dictionary, key);
			if (@object == null)
			{
				return null;
			}
			return @object as string;
		}
	}
}

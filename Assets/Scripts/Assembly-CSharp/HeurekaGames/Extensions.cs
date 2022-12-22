using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HeurekaGames
{
	public static class Extensions
	{
		public static Vector2 YZ(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static Vector2[] YZ(this Vector3[] v)
		{
			Vector2[] array = new Vector2[v.Length];
			for (int i = 0; i < v.Length; i++)
			{
				array[i] = new Vector2(v[i].x, v[i].z);
			}
			return array;
		}

		public static float Remap(this float value, float from1, float to1, float from2, float to2)
		{
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}

		public static string ToCamelCase(this string camelCaseString)
		{
			return Regex.Replace(camelCaseString, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ").Trim();
		}

		public static void SetComponentRecursively<T>(this GameObject gameObject, bool tf) where T : Component
		{
			T[] componentsInChildren = gameObject.GetComponentsInChildren<T>();
			T[] array = componentsInChildren;
			foreach (T obj in array)
			{
				try
				{
					PropertyInfo property = typeof(T).GetProperty("enabled");
					if (property != null && property.CanWrite)
					{
						property.SetValue(obj, tf, null);
						continue;
					}
					Console.WriteLine("BLABLA");
					Debug.Log("Property does not exist, or cannot write");
				}
				catch (NullReferenceException ex)
				{
					Debug.Log("The property does not exist in MyClass." + ex.Message);
				}
			}
		}

		public static void CastList<T>(this List<T> targetList)
		{
			targetList = targetList.Cast<T>().ToList();
		}

		public static bool Has<T>(this Enum type, T value)
		{
			//Discarded unreachable code: IL_0025, IL_0032
			try
			{
				return ((int)(object)type & (int)(object)value) == (int)(object)value;
			}
			catch
			{
				return false;
			}
		}

		public static bool Is<T>(this Enum type, T value)
		{
			//Discarded unreachable code: IL_0019, IL_0026
			try
			{
				return (int)(object)type == (int)(object)value;
			}
			catch
			{
				return false;
			}
		}

		public static T Add<T>(this Enum type, T value)
		{
			//Discarded unreachable code: IL_0022, IL_0048
			try
			{
				return (T)(object)((int)(object)type | (int)(object)value);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException(string.Format("Could not append value from enumerated type '{0}'.", typeof(T).Name), innerException);
			}
		}

		public static T Remove<T>(this Enum type, T value)
		{
			//Discarded unreachable code: IL_0023, IL_0049
			try
			{
				return (T)(object)((int)(object)type & ~(int)(object)value);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException(string.Format("Could not remove value from enumerated type '{0}'.", typeof(T).Name), innerException);
			}
		}

		public static Color ModifiedAlpha(this Color color, float alpha)
		{
			Color result = color;
			result.a = alpha;
			return result;
		}
	}
}

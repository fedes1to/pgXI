using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Unity.Linq;
using UnityEngine;

namespace Rilisoft
{
	public static class RiliExtensions
	{
		private static System.Random _random = new System.Random();

		public static long SystemTime
		{
			get
			{
				return (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			}
		}

		public static bool TryGetValue<T>(this Dictionary<string, object> dict, string key, out T value)
		{
			//Discarded unreachable code: IL_0071, IL_008e
			if (dict == null)
			{
				value = default(T);
				return false;
			}
			object value2;
			if (!dict.TryGetValue(key, out value2))
			{
				value = default(T);
				return false;
			}
			if (value2 is T)
			{
				value = (T)value2;
				return true;
			}
			try
			{
				value = (T)Convert.ChangeType(value2, typeof(T));
				return true;
			}
			catch
			{
				value = default(T);
				return false;
			}
		}

		public static IEnumerable<T> WithoutLast<T>(this IEnumerable<T> source)
		{
			using (IEnumerator<T> e = source.GetEnumerator())
			{
				if (e.MoveNext())
				{
					T value = e.Current;
					while (e.MoveNext())
					{
						yield return value;
						value = e.Current;
					}
				}
			}
		}

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
		{
			return source == null || !source.Any();
		}

		public static bool IsEmpty<T>(this IEnumerable<T> source)
		{
			return !source.Any();
		}

		public static void Swap<T>(ref T lhs, ref T rhs)
		{
			T val = lhs;
			lhs = rhs;
			rhs = val;
		}

		public static void SetInstantlyNoHandlers(this UIToggle toggle, bool state)
		{
			if (!(toggle == null))
			{
				List<EventDelegate> onChange = toggle.onChange;
				toggle.onChange = new List<EventDelegate>();
				bool instantTween = toggle.instantTween;
				toggle.instantTween = true;
				toggle.Set(state);
				toggle.onChange = onChange;
				toggle.instantTween = instantTween;
			}
		}

		public static string nameNoClone(this UnityEngine.Object obj)
		{
			if (obj == null)
			{
				return null;
			}
			return obj.name.Replace("(Clone)", string.Empty);
		}

		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static T? ToEnum<T>(this string str, T? defaultVal = null) where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			if (str.IsNullOrEmpty())
			{
				Debug.LogError("String is null or empty");
				return defaultVal;
			}
			str = str.ToLower();
			foreach (T value in Enum.GetValues(typeof(T)))
			{
				if (value.ToString().ToLower() == str)
				{
					return value;
				}
			}
			Debug.LogErrorFormat("'{0}' does not contain '{1}'", typeof(T).Name, str);
			return defaultVal;
		}

		public static string[] EnumValues<T>() where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			return Enum.GetValues(typeof(T)).Cast<string>().ToArray();
		}

		public static int[] EnumNumbers<T>() where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			return Enum.GetValues(typeof(T)).Cast<int>().ToArray();
		}

		public static int EnumLen<T>()
		{
			return Enum.GetNames(typeof(T)).Length;
		}

		public static void ForEachEnum<T>(Action<T> action)
		{
			if (action == null)
			{
				return;
			}
			Array values = Enum.GetValues(typeof(T));
			foreach (object item in values)
			{
				action((T)item);
			}
		}

		public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
		{
			foreach (T item in enumeration)
			{
				action(item);
			}
		}

		public static int ToInt(this bool boolValue)
		{
			return boolValue ? 1 : 0;
		}

		public static bool ToBool(this int intValue)
		{
			return intValue > 0;
		}

		public static List<Transform> Neighbors(this Transform tr, bool withSelf = false)
		{
			if (tr == null || tr.parent == null)
			{
				return new List<Transform>();
			}
			return (from Transform neighbor in tr.parent
				where withSelf || neighbor != tr
				select neighbor).ToList();
		}

		public static GameObject GetChildGameObject(this GameObject go, string name, bool includeInactive = false)
		{
			Transform transform = go.transform.GetComponentsInChildren<Transform>(includeInactive).FirstOrDefault((Transform t) => t.gameObject.name == name);
			return (!(transform != null)) ? null : transform.gameObject;
		}

		public static T GetComponentInChildren<T>(this GameObject go, string name, bool includeInactive = false)
		{
			Transform[] componentsInChildren = go.transform.GetComponentsInChildren<Transform>(includeInactive);
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				if (transform.gameObject.name == name)
				{
					return transform.gameObject.GetComponent<T>();
				}
			}
			return default(T);
		}

		public static GameObject GetGameObjectInParent(this GameObject go, string name, bool includeInactive = false)
		{
			Transform[] componentsInParent = go.transform.GetComponentsInParent<Transform>(includeInactive);
			Transform[] array = componentsInParent;
			foreach (Transform transform in array)
			{
				if (transform.gameObject.name == name)
				{
					return transform.gameObject;
				}
			}
			return null;
		}

		public static T GetComponentInParents<T>(this GameObject go)
		{
			T component = go.GetComponent<T>();
			if (component != null && !component.Equals(default(T)))
			{
				return component;
			}
			Transform parent = go.transform.parent;
			return parent.gameObject.GetComponentInParents<T>();
		}

		public static bool IsSubobjectOf(this GameObject go, GameObject assumedRoot)
		{
			if (go.Equals(null) || assumedRoot.Equals(null))
			{
				return false;
			}
			foreach (GameObject item in assumedRoot.Descendants())
			{
				if (item.Equals(go))
				{
					return true;
				}
			}
			return false;
		}

		public static void SetActiveSafe(this GameObject go, bool state)
		{
			if (!(go == null) && go.activeInHierarchy != state)
			{
				go.SetActive(state);
			}
		}

		public static void SetActiveSafeSelf(this GameObject go, bool state)
		{
			if (go != null && go.activeSelf != state)
			{
				go.SetActive(state);
			}
		}

		public static T GetOrAddComponent<T>(this Component child) where T : Component
		{
			T val = child.GetComponent<T>();
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = child.gameObject.AddComponent<T>();
			}
			return val;
		}

		public static T GetOrAddComponent<T>(this GameObject child) where T : Component
		{
			T val = child.GetComponent<T>();
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = child.gameObject.AddComponent<T>();
			}
			return val;
		}

		public static IEnumerable<T> Random<T>(this IEnumerable<T> source, int count)
		{
			if (source == null)
			{
				return source;
			}
			List<T> list = source.ToList();
			if (!list.Any())
			{
				return list;
			}
			List<T> list2 = new List<T>();
			if (count < 1)
			{
				return list2;
			}
			bool flag = true;
			while (flag)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				list2.Add(list[index]);
				list.RemoveAt(index);
				if (list.Count == 0 || list2.Count == count)
				{
					flag = false;
				}
			}
			return list2;
		}

		public static Color ColorFromRGB(int r, int g, int b, int a = 255)
		{
			return new Color(r / 255, g / 255, b / 255, a / 255);
		}

		public static Color ColorFromHex(string hex)
		{
			hex = hex.Replace("0x", string.Empty);
			hex = hex.Replace("#", string.Empty);
			byte a = byte.MaxValue;
			byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			if (hex.Length == 8)
			{
				a = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			}
			return new Color32(r, g, b, a);
		}

		public static string ToHex(this Color32 color)
		{
			return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		}

		public static T ParseJSONField<T>(this Dictionary<string, object> dict, string name, Func<object, T> converter, bool ignoreNotFoundError = false)
		{
			//Discarded unreachable code: IL_0039, IL_006a
			if (dict == null)
			{
				Debug.LogError("dict is null");
				return default(T);
			}
			if (dict.ContainsKey(name))
			{
				try
				{
					return converter(dict[name]);
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("[PARSE] error parse '{0}' : {1}, {2}", name, ex.Message, ex.StackTrace));
					return default(T);
				}
			}
			if (ignoreNotFoundError)
			{
				return default(T);
			}
			Debug.LogError(string.Format("[PARSE] unknown key '{0}'", name));
			return default(T);
		}

		public static void ShuffleThis<T>(this IList<T> list)
		{
			int num = list.Count;
			while (num > 1)
			{
				num--;
				int index = _random.Next(num + 1);
				T value = list[index];
				list[index] = list[num];
				list[num] = value;
			}
		}

		public static IList<T> Shuffle<T>(this IList<T> list)
		{
			List<T> list2 = new List<T>(list);
			int num = list2.Count;
			while (num > 1)
			{
				num--;
				int index = _random.Next(num + 1);
				T value = list2[index];
				list2[index] = list2[num];
				list2[num] = value;
			}
			return list2;
		}

		public static IEnumerator MoveOverTime(this GameObject go, Vector3 from, Vector3 to, float seconds, Action onFinished = null)
		{
			go.transform.position = from;
			float elapsedTime = 0f;
			while (elapsedTime < seconds)
			{
				go.transform.position = Vector3.Lerp(from, to, elapsedTime / seconds);
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			go.transform.position = to;
			if (onFinished != null)
			{
				onFinished();
			}
		}

		public static void DrawGUIRect(Rect position, Color color)
		{
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.SetPixel(0, 0, color);
			texture2D.Apply();
			GUI.skin.box.normal.background = texture2D;
			GUI.Box(position, GUIContent.none);
		}

		public static string CombinePaths(params string[] paths)
		{
			if (paths == null)
			{
				throw new ArgumentNullException("paths");
			}
			Func<string, string, string> func = (string path1, string path2) => Path.Combine(path1, path2);
			return paths.Aggregate(func);
		}

		public static Vector2 PointOnCircle(float angleDegrees, float radius)
		{
			float num = 0f;
			num = angleDegrees * (float)Math.PI / 180f;
			float x = radius * Mathf.Cos(num);
			float y = radius * Mathf.Sin(num);
			return new Vector2(x, y);
		}

		public static Vector2 RandomOnCircle(float radius)
		{
			Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
			insideUnitCircle.Normalize();
			return insideUnitCircle * radius;
		}

		public static Vector3 RandomOnSphere(float radius)
		{
			Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
			insideUnitSphere.Normalize();
			return insideUnitSphere * radius;
		}

		public static Vector3 Add(this Vector3 ts, Vector3 v)
		{
			return ts + v;
		}

		public static Vector3 Add(this Vector3 ts, Vector2 v)
		{
			return new Vector3(ts.x + v.x, ts.y + v.y, ts.z);
		}

		public static string GetTimeStringDays(long seconds)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
			string empty = string.Empty;
			if (seconds >= 86400)
			{
				long num = seconds / 86400;
				long num2 = seconds % 86400;
				if (num2 > 43200)
				{
					num++;
				}
				return num.ToString();
			}
			return string.Format("{0:D2}h:{1:D2}m:{2:D2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}

		public static string GetTimeStringLocalizedShort(long seconds)
		{
			if (seconds >= 86400)
			{
				long num = seconds / 86400;
				long num2 = seconds % 86400;
				if (num2 > 43200)
				{
					num++;
				}
				return string.Format(LocalizationStore.Get("Key_2913"), num);
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
			if (timeSpan.Days > 0)
			{
				return string.Format(LocalizationStore.Get("Key_2913"), timeSpan.Days);
			}
			if (timeSpan.Hours > 0)
			{
				return string.Format(LocalizationStore.Get("Key_2917"), timeSpan.Hours);
			}
			if (timeSpan.Minutes > 0)
			{
				return string.Format(LocalizationStore.Get("Key_2918"), timeSpan.Minutes);
			}
			return string.Format(LocalizationStore.Get("Key_2918"), 1);
		}

		public static string GetTimeString(long secs, string delimer = ":")
		{
			if (secs < 1)
			{
				return string.Empty;
			}
			int num = (int)(secs / 3600);
			int num2 = (int)(secs / 60) - num * 60;
			int num3 = (int)secs - num * 3600 - num2 * 60;
			string text = ((num >= 10) ? num.ToString() : ("0" + num));
			string text2 = ((num2 >= 10) ? num2.ToString() : ("0" + num2));
			string text3 = ((num3 >= 10) ? num3.ToString() : ("0" + num3));
			return text + delimer + text2 + delimer + text3;
		}
	}
}

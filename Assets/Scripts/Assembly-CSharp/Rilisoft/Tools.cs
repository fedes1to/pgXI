using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Rilisoft
{
	public sealed class Tools
	{
		public enum PreviewType
		{
			Head,
			HeadAndBody
		}

		public static RuntimePlatform RuntimePlatform
		{
			get
			{
				return Application.platform;
			}
		}

		public static int AllWithoutDamageCollidersMask
		{
			get
			{
				return -5 & ~(1 << LayerMask.NameToLayer("DamageCollider"));
			}
		}

		public static int AllWithoutMyPlayerMask
		{
			get
			{
				return -5 & ~(1 << LayerMask.NameToLayer("MyPlayer"));
			}
		}

		public static int AllWithoutDamageCollidersMaskAndWithoutRocket
		{
			get
			{
				return -5 & ~(1 << LayerMask.NameToLayer("DamageCollider")) & ~(1 << LayerMask.NameToLayer("Rocket")) & ~(1 << LayerMask.NameToLayer("TransparentFX"));
			}
		}

		public static int AllAvailabelBotRaycastMask
		{
			get
			{
				return -5 & ~(1 << LayerMask.NameToLayer("DamageCollider")) & ~(1 << LayerMask.NameToLayer("NotDetectMobRaycast"));
			}
		}

		public static long CurrentUnixTime
		{
			get
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
				return (long)(DateTime.UtcNow - dateTime).TotalSeconds;
			}
		}

		public static bool EscapePressed()
		{
			if (BackSystem.Active)
			{
				return false;
			}
			return Input.GetKeyUp(KeyCode.Escape);
		}

		private static bool ConnectedToPhoton()
		{
			return PhotonNetwork.room != null;
		}

		internal static WWW CreateWww(string url)
		{
			WWW wWW = new WWW(url);
			if (Application.isEditor && FriendsController.isDebugLogWWW)
			{
				string[] source = url.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				string text = source.LastOrDefault() ?? url;
				if (wWW != null)
				{
					Debug.LogFormat("<color=yellow>{0}</color>", text);
				}
				else
				{
					Debug.LogFormat("<color=orange>Skipping {0}</color>", text);
				}
			}
			return wWW;
		}

		internal static WWW CreateWww(string url, WWWForm form, string comment = "")
		{
			return CreateWwwIf(true, url, form, comment);
		}

		internal static WWW CreateWwwIfNotConnected(string url, WWWForm form, string comment = "", Dictionary<string, string> headers = null)
		{
			return CreateWwwIf(!ConnectedToPhoton(), url, form, comment, headers);
		}

		internal static WWW CreateWwwIf(bool condition, string url, WWWForm form, string comment = "", Dictionary<string, string> headers = null)
		{
			WWW result = ((!condition) ? null : ((headers == null) ? new WWW(url, form) : new WWW(url, form.data, headers)));
			if (Application.isEditor && FriendsController.isDebugLogWWW)
			{
				byte[] array = form.data ?? new byte[0];
				string @string = Encoding.UTF8.GetString(array, 0, array.Length);
				string[] source = @string.Split(new char[1] { '&' }, StringSplitOptions.RemoveEmptyEntries);
				string text = source.FirstOrDefault((string p) => p.StartsWith("action=")) ?? url;
				string text2 = ((!string.IsNullOrEmpty(comment)) ? string.Format("{0}; {1}", text, comment) : text);
				if (condition)
				{
					Debug.LogFormat("<b><color=yellow>{0}</color></b>", text2);
				}
				else
				{
					Debug.LogFormat("<b><color=orange>Skipping {0}</color></b>", text2);
				}
			}
			return result;
		}

		internal static WWW CreateWwwIfNotConnected(string url)
		{
			WWW wWW = null;
			try
			{
				wWW = ((!ConnectedToPhoton()) ? new WWW(url) : null);
				if (Application.isEditor && FriendsController.isDebugLogWWW)
				{
					string[] source = url.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
					string text = source.LastOrDefault() ?? url;
					if (wWW != null)
					{
						Debug.LogFormat("<color=yellow>{0}</color>", text);
					}
					else
					{
						Debug.LogFormat("<color=orange>Skipping {0}</color>", text);
					}
				}
				} catch {
					Debug.LogWarning("WWW broke lmao");
				}
			return wWW;
		}

		public static bool ParseDateTimeFromPlayerPrefs(string dateKey, out DateTime parsedDate)
		{
			string @string = Storager.getString(dateKey, false);
			return DateTime.TryParse(@string, out parsedDate);
		}

		public static DateTime GetCurrentTimeByUnixTime(long unixTime)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);
		}

		public static void AddSessionNumber()
		{
			int @int = PlayerPrefs.GetInt(Defs.SessionNumberKey, 1);
			PlayerPrefs.SetInt(Defs.SessionNumberKey, @int + 1);
			string @string = PlayerPrefs.GetString(Defs.LastTimeSessionDayKey, string.Empty);
			DateTimeOffset result;
			DateTimeOffset.TryParse(DateTimeOffset.UtcNow.ToString("s"), out result);
			DateTimeOffset result2;
			if (string.IsNullOrEmpty(@string) || (DateTimeOffset.TryParse(@string, out result2) && ((!Defs.IsDeveloperBuild && (result - result2).TotalHours > 23.0) || (Defs.IsDeveloperBuild && (result - result2).TotalMinutes > 3.0))))
			{
				int int2 = PlayerPrefs.GetInt(Defs.SessionDayNumberKey, 0);
				int num = int2 + 1;
				PlayerPrefs.SetInt(Defs.SessionDayNumberKey, num);
				PlayerPrefs.SetString(Defs.LastTimeSessionDayKey, DateTimeOffset.UtcNow.ToString("s"));
				GlobalGameController.CountDaySessionInCurrentVersion = GlobalGameController.CountDaySessionInCurrentVersion;
				AnalyticsStuff.SendInGameDay(num);
			}
		}

		public static Rect SuccessMessageRect()
		{
			return new Rect((float)(Screen.width / 2) - (float)Screen.height * 0.5f, (float)Screen.height * 0.5f - (float)Screen.height * 0.0525f, Screen.height, (float)Screen.height * 0.105f);
		}

		public static void SetVibibleNguiObjectByAlpha(GameObject nguiObject, bool isVisible)
		{
			UIWidget component = nguiObject.GetComponent<UIWidget>();
			if (!(component == null))
			{
				component.alpha = ((!isVisible) ? 0.001f : 1f);
			}
		}

		public static void SetLayerRecursively(GameObject obj, int newLayer)
		{
			if (null == obj)
			{
				return;
			}
			obj.layer = newLayer;
			int childCount = obj.transform.childCount;
			Transform transform = obj.transform;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (!(null == child))
				{
					SetLayerRecursively(child.gameObject, newLayer);
				}
			}
		}

		public static void SetTextureRecursivelyFrom(GameObject obj, Texture txt, GameObject[] stopObjs)
		{
			Transform transform = obj.transform;
			int childCount = obj.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				bool flag = false;
				foreach (GameObject o in stopObjs)
				{
					if (child.gameObject.Equals(o))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				if ((bool)child.gameObject.GetComponent<Renderer>() && (bool)child.gameObject.GetComponent<Renderer>().material)
				{
					child.gameObject.GetComponent<Renderer>().material.mainTexture = txt;
				}
				flag = false;
				foreach (GameObject o2 in stopObjs)
				{
					if (child.gameObject.Equals(o2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					SetTextureRecursivelyFrom(child.gameObject, txt, stopObjs);
				}
			}
		}

		public static Color[] FlipColorsHorizontally(Color[] colors, int width, int height)
		{
			Color[] array = new Color[colors.Length];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					array[i + width * j] = colors[width - i - 1 + width * j];
				}
			}
			return array;
		}

		public static Texture2D GetPreviewFromSkin(string skinStr, PreviewType type)
		{
			Texture2D texture2D = null;
			if (!string.IsNullOrEmpty(skinStr) && !skinStr.Equals("empty"))
			{
				texture2D = new Texture2D(64, 32);
				texture2D.LoadImage(Convert.FromBase64String(skinStr));
			}
			else
			{
				texture2D = Resources.Load<Texture2D>(ResPath.Combine(Defs.MultSkinsDirectoryName, "multi_skin_1"));
			}
			Texture2D texture2D2 = null;
			switch (type)
			{
			case PreviewType.Head:
			{
				texture2D2 = new Texture2D(8, 8, TextureFormat.ARGB32, false);
				for (int k = 0; k < texture2D2.width; k++)
				{
					for (int l = 0; l < texture2D2.height; l++)
					{
						texture2D2.SetPixel(k, l, Color.clear);
					}
				}
				texture2D2.SetPixels(0, 0, 8, 8, texture2D.GetPixels(8, 16, 8, 8));
				break;
			}
			case PreviewType.HeadAndBody:
			{
				texture2D2 = new Texture2D(16, 14, TextureFormat.ARGB32, false);
				for (int i = 0; i < texture2D2.width; i++)
				{
					for (int j = 0; j < texture2D2.height; j++)
					{
						texture2D2.SetPixel(i, j, Color.clear);
					}
				}
				texture2D2.SetPixels(4, 6, 8, 8, texture2D.GetPixels(8, 16, 8, 8));
				texture2D2.SetPixels(4, 0, 8, 6, texture2D.GetPixels(20, 6, 8, 6));
				texture2D2.SetPixels(0, 0, 4, 6, texture2D.GetPixels(44, 6, 4, 6));
				texture2D2.SetPixels(12, 0, 4, 6, FlipColorsHorizontally(texture2D.GetPixels(44, 6, 4, 6), 4, 6));
				break;
			}
			}
			texture2D2.anisoLevel = 1;
			texture2D2.mipMapBias = -0.5f;
			texture2D2.Apply();
			texture2D2.filterMode = FilterMode.Point;
			return texture2D2;
		}

		internal static T DeserializeJson<T>(string json)
		{
			//Discarded unreachable code: IL_0021, IL_003c
			if (string.IsNullOrEmpty(json))
			{
				return default(T);
			}
			try
			{
				return JsonUtility.FromJson<T>(json);
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
				return default(T);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	public class AchievementData
	{
		private static readonly AchievementData[] s_empty = new AchievementData[0];

		public int Id { get; private set; }

		public int GroupId { get; private set; }

		public AchievementClassType ClassType { get; private set; }

		public AchievementType Type { get; private set; }

		public string LKeyName { get; private set; }

		public string LKeyDesc { get; private set; }

		public int[] Thresholds { get; private set; }

		public string Icon { get; set; }

		public ShopNGUIController.CategoryNames? WeaponCategory { get; private set; }

		public ConnectSceneNGUIController.RegimGame? RegimGame { get; private set; }

		public string Currency { get; private set; }

		public RatingSystem.RatingLeague? League { get; private set; }

		public string ItemId { get; private set; }

		public List<ShopNGUIController.CategoryNames?> WeaponCategories { get; private set; }

		public Dictionary<string, object> RawData { get; private set; }

		public AchievementData(Dictionary<string, object> raw)
		{
			if (raw == null)
			{
				Debug.LogError("Achievement parse error");
				return;
			}
			RawData = raw;
			Id = raw.ParseJSONField("id", ConvertToInt32Invariant);
			GroupId = raw.ParseJSONField("group", ConvertToInt32Invariant, true);
			Type = raw.ParseJSONField("type", (object o) => o.ToString().ToEnum<AchievementType>().Value);
			ClassType = raw.ParseJSONField("classType", (object o) => o.ToString().ToEnum<AchievementClassType>(AchievementClassType.Unknown).Value);
			Icon = raw.ParseJSONField("icon", ConvertToString);
			LKeyName = raw.ParseJSONField("keyName", ConvertToString);
			LKeyDesc = raw.ParseJSONField("keyDesc", ConvertToString);
			Thresholds = raw.ParseJSONField("thresholds", (object o) => (o as List<object>).Select(ConvertToInt32Invariant).ToArray());
			WeaponCategory = raw.ParseJSONField("weaponCategory", (object o) => o.ToString().ToEnum<ShopNGUIController.CategoryNames>(), true);
			RegimGame = raw.ParseJSONField("regimGame", (object o) => o.ToString().ToEnum<ConnectSceneNGUIController.RegimGame>(), true);
			Currency = raw.ParseJSONField("currency", delegate(object o)
			{
				string x = o.ToString();
				StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
				if (ordinalIgnoreCase.Equals(x, "gems"))
				{
					return "GemsCurrency";
				}
				return ordinalIgnoreCase.Equals(x, "coins") ? "Coins" : null;
			}, true);
			League = raw.ParseJSONField("league", (object o) => o.ToString().ToEnum<RatingSystem.RatingLeague>(), true);
			ItemId = raw.ParseJSONField("itemId", ConvertToString, true);
			WeaponCategories = raw.ParseJSONField("weaponCategories", delegate(object o)
			{
				List<object> list = o as List<object>;
				return (list == null) ? null : list.Select((object itm) => itm.ToString().ToEnum<ShopNGUIController.CategoryNames>()).ToList();
			}, true);
		}

		public override string ToString()
		{
			return Json.Serialize(this);
		}

		private static int ConvertToInt32Invariant(object o)
		{
			return Convert.ToInt32(o, CultureInfo.InvariantCulture);
		}

		private static string ConvertToString(object o)
		{
			return o.ToString();
		}

		internal static IEnumerable<AchievementData> ParseAllAsEnumerable(string rawData)
		{
			List<object> list = Json.Deserialize(rawData) as List<object>;
			if (list == null)
			{
				Debug.LogError("[Achievements] parse error");
				return s_empty;
			}
			return from d in list.OfType<Dictionary<string, object>>()
				select new AchievementData(d);
		}
	}
}

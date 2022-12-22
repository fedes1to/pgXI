using System;
using System.Collections.Generic;
using com.amazon.mas.cpt.ads.json;

namespace com.amazon.mas.cpt.ads
{
	public sealed class IsReady : Jsonable
	{
		private static AmazonLogger logger = new AmazonLogger("Pi");

		public bool BooleanValue { get; set; }

		public string ToJson()
		{
			//Discarded unreachable code: IL_0013, IL_0025
			try
			{
				Dictionary<string, object> objectDictionary = GetObjectDictionary();
				return Json.Serialize(objectDictionary);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while Jsoning", inner);
			}
		}

		public override Dictionary<string, object> GetObjectDictionary()
		{
			//Discarded unreachable code: IL_0023, IL_0035
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("booleanValue", BooleanValue);
				return dictionary;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
		}

		public static IsReady CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			//Discarded unreachable code: IL_0040, IL_0052
			try
			{
				if (jsonMap == null)
				{
					return null;
				}
				IsReady isReady = new IsReady();
				if (jsonMap.ContainsKey("booleanValue"))
				{
					isReady.BooleanValue = (bool)jsonMap["booleanValue"];
				}
				return isReady;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
		}

		public static IsReady CreateFromJson(string jsonMessage)
		{
			//Discarded unreachable code: IL_001e, IL_0030
			try
			{
				Dictionary<string, object> jsonMap = Json.Deserialize(jsonMessage) as Dictionary<string, object>;
				Jsonable.CheckForErrors(jsonMap);
				return CreateFromDictionary(jsonMap);
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while UnJsoning", inner);
			}
		}

		public static Dictionary<string, IsReady> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, IsReady> dictionary = new Dictionary<string, IsReady>();
			foreach (KeyValuePair<string, object> item in jsonMap)
			{
				IsReady value = CreateFromDictionary(item.Value as Dictionary<string, object>);
				dictionary.Add(item.Key, value);
			}
			return dictionary;
		}

		public static List<IsReady> ListFromJson(List<object> array)
		{
			List<IsReady> list = new List<IsReady>();
			foreach (object item in array)
			{
				list.Add(CreateFromDictionary(item as Dictionary<string, object>));
			}
			return list;
		}
	}
}

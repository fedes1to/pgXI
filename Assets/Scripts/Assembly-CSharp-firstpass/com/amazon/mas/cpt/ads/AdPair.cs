using System;
using System.Collections.Generic;
using com.amazon.mas.cpt.ads.json;

namespace com.amazon.mas.cpt.ads
{
	public sealed class AdPair : Jsonable
	{
		private static AmazonLogger logger = new AmazonLogger("Pi");

		public Ad AdOne { get; set; }

		public Ad AdTwo { get; set; }

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
			//Discarded unreachable code: IL_005b, IL_006d
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("adOne", (AdOne == null) ? null : AdOne.GetObjectDictionary());
				dictionary.Add("adTwo", (AdTwo == null) ? null : AdTwo.GetObjectDictionary());
				return dictionary;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while getting object dictionary", inner);
			}
		}

		public static AdPair CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			//Discarded unreachable code: IL_0070, IL_0082
			try
			{
				if (jsonMap == null)
				{
					return null;
				}
				AdPair adPair = new AdPair();
				if (jsonMap.ContainsKey("adOne"))
				{
					adPair.AdOne = Ad.CreateFromDictionary(jsonMap["adOne"] as Dictionary<string, object>);
				}
				if (jsonMap.ContainsKey("adTwo"))
				{
					adPair.AdTwo = Ad.CreateFromDictionary(jsonMap["adTwo"] as Dictionary<string, object>);
				}
				return adPair;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
		}

		public static AdPair CreateFromJson(string jsonMessage)
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

		public static Dictionary<string, AdPair> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, AdPair> dictionary = new Dictionary<string, AdPair>();
			foreach (KeyValuePair<string, object> item in jsonMap)
			{
				AdPair value = CreateFromDictionary(item.Value as Dictionary<string, object>);
				dictionary.Add(item.Key, value);
			}
			return dictionary;
		}

		public static List<AdPair> ListFromJson(List<object> array)
		{
			List<AdPair> list = new List<AdPair>();
			foreach (object item in array)
			{
				list.Add(CreateFromDictionary(item as Dictionary<string, object>));
			}
			return list;
		}
	}
}

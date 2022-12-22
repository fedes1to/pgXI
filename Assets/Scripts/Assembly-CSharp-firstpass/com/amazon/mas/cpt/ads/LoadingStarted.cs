using System;
using System.Collections.Generic;
using com.amazon.mas.cpt.ads.json;

namespace com.amazon.mas.cpt.ads
{
	public sealed class LoadingStarted : Jsonable
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

		public static LoadingStarted CreateFromDictionary(Dictionary<string, object> jsonMap)
		{
			//Discarded unreachable code: IL_0040, IL_0052
			try
			{
				if (jsonMap == null)
				{
					return null;
				}
				LoadingStarted loadingStarted = new LoadingStarted();
				if (jsonMap.ContainsKey("booleanValue"))
				{
					loadingStarted.BooleanValue = (bool)jsonMap["booleanValue"];
				}
				return loadingStarted;
			}
			catch (ApplicationException inner)
			{
				throw new AmazonException("Error encountered while creating Object from dicionary", inner);
			}
		}

		public static LoadingStarted CreateFromJson(string jsonMessage)
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

		public static Dictionary<string, LoadingStarted> MapFromJson(Dictionary<string, object> jsonMap)
		{
			Dictionary<string, LoadingStarted> dictionary = new Dictionary<string, LoadingStarted>();
			foreach (KeyValuePair<string, object> item in jsonMap)
			{
				LoadingStarted value = CreateFromDictionary(item.Value as Dictionary<string, object>);
				dictionary.Add(item.Key, value);
			}
			return dictionary;
		}

		public static List<LoadingStarted> ListFromJson(List<object> array)
		{
			List<LoadingStarted> list = new List<LoadingStarted>();
			foreach (object item in array)
			{
				list.Add(CreateFromDictionary(item as Dictionary<string, object>));
			}
			return list;
		}
	}
}

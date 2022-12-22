using System;
using System.Collections.Generic;
using Rilisoft.MiniJson;

namespace Rilisoft
{
	public sealed class CurrencyChangedEventArgs : EventArgs
	{
		public string Currency { get; set; }

		public int NewValue { get; set; }

		public int AddedValue { get; set; }

		public Dictionary<string, object> ToJson()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("currency", Currency);
			dictionary.Add("newValue", NewValue);
			dictionary.Add("addedValue", AddedValue);
			return dictionary;
		}

		public override string ToString()
		{
			return Json.Serialize(ToJson());
		}
	}
}

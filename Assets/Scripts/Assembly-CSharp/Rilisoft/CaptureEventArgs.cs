using System;
using System.Collections.Generic;
using Rilisoft.MiniJson;

namespace Rilisoft
{
	public sealed class CaptureEventArgs : EventArgs
	{
		public ConnectSceneNGUIController.RegimGame Mode { get; set; }

		public Dictionary<string, object> ToJson()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("mode", Mode);
			return dictionary;
		}

		public override string ToString()
		{
			return Json.Serialize(ToJson());
		}
	}
}

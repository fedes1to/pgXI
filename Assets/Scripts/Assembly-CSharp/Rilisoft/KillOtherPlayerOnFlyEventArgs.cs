using System;
using System.Collections.Generic;
using Rilisoft.MiniJson;

namespace Rilisoft
{
	public sealed class KillOtherPlayerOnFlyEventArgs : EventArgs
	{
		public bool IamFly { get; set; }

		public bool KilledPlayerFly { get; set; }

		public Dictionary<string, object> ToJson()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("iamFly", IamFly);
			dictionary.Add("killedPlayerFly", KilledPlayerFly);
			return dictionary;
		}

		public override string ToString()
		{
			return Json.Serialize(ToJson());
		}
	}
}

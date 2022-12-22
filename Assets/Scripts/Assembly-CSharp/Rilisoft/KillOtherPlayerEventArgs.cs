using System;
using System.Collections.Generic;
using Rilisoft.MiniJson;

namespace Rilisoft
{
	public sealed class KillOtherPlayerEventArgs : EventArgs
	{
		public ConnectSceneNGUIController.RegimGame Mode { get; set; }

		public ShopNGUIController.CategoryNames WeaponSlot { get; set; }

		public bool Headshot { get; set; }

		public bool Grenade { get; set; }

		public bool Revenge { get; set; }

		public bool VictimIsFlagCarrier { get; set; }

		public bool IsInvisible { get; set; }

		public Dictionary<string, object> ToJson()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("mode", Mode);
			dictionary.Add("weaponSlot", WeaponSlot);
			dictionary.Add("headshot", Headshot);
			dictionary.Add("grenade", Grenade);
			dictionary.Add("revenge", Revenge);
			dictionary.Add("victimIsFlagCarrier", VictimIsFlagCarrier);
			dictionary.Add("isInvisible", IsInvisible);
			return dictionary;
		}

		public override string ToString()
		{
			return Json.Serialize(ToJson());
		}
	}
}

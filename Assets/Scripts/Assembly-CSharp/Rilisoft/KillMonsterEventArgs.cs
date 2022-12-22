using System;
using System.Collections.Generic;
using Rilisoft.MiniJson;

namespace Rilisoft
{
	public sealed class KillMonsterEventArgs : EventArgs
	{
		public ShopNGUIController.CategoryNames WeaponSlot { get; set; }

		public bool Campaign { get; set; }

		public Dictionary<string, object> ToJson()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("weaponSlot", WeaponSlot);
			dictionary.Add("campaign", Campaign);
			return dictionary;
		}

		public override string ToString()
		{
			return Json.Serialize(ToJson());
		}
	}
}

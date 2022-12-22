using System;
using System.Collections.Generic;

namespace Rilisoft
{
	[Serializable]
	public class BonusData
	{
		public enum BonusType
		{
			Unknown,
			Currency,
			Weapons,
			Pets,
			Leprechaunt,
			Gadgets
		}

		public bool IsGems;

		public BonusType Type;

		public string Action;

		public string BonusId;

		public int End;

		public bool IsTypePack;

		public int Pack;

		public int Coins;

		public int Gems;

		public string WeaponId;

		public string PetId;

		public int Quantity;

		public string LeprechauntCurrency;

		public int LeprechauntPerDay;

		public int LeprechauntDays;

		public List<string> Gadgets;

		public void Set(Dictionary<string, object> bonusData)
		{
			if (bonusData == null)
			{
				return;
			}
			if (bonusData.ContainsKey("isGems"))
			{
				IsGems = Convert.ToBoolean(bonusData["isGems"]);
			}
			if (bonusData.ContainsKey("action"))
			{
				Action = Convert.ToString(bonusData["action"]);
			}
			if (bonusData.ContainsKey("ID"))
			{
				BonusId = Convert.ToString(bonusData["ID"]);
			}
			if (bonusData.ContainsKey("End"))
			{
				End = Convert.ToInt32(bonusData["End"]);
			}
			if (bonusData.ContainsKey("Type"))
			{
				IsTypePack = bonusData["Type"].ToString() == "packs";
			}
			if (bonusData.ContainsKey("Coins"))
			{
				Coins = Convert.ToInt32(bonusData["Coins"]);
			}
			if (bonusData.ContainsKey("GemsCurrency"))
			{
				Gems = Convert.ToInt32(bonusData["GemsCurrency"]);
			}
			if (bonusData.ContainsKey("Pack"))
			{
				Pack = Convert.ToInt32(bonusData["Pack"]);
			}
			if (Action == BalanceController.weaponActionName)
			{
				Type = BonusType.Weapons;
				if (bonusData.ContainsKey("Weapon"))
				{
					WeaponId = Convert.ToString(bonusData["Weapon"]);
				}
			}
			else if (Action == BalanceController.petActionName)
			{
				Type = BonusType.Pets;
				if (bonusData.ContainsKey("Pet"))
				{
					PetId = Convert.ToString(bonusData["Pet"]);
				}
				if (bonusData.ContainsKey("Quantity"))
				{
					Quantity = Convert.ToInt32(bonusData["Quantity"]);
				}
			}
			else if (Action == BalanceController.leprechaunActionName)
			{
				Type = BonusType.Leprechaunt;
				if (bonusData.ContainsKey("CurrencyLeprechaun"))
				{
					LeprechauntCurrency = Convert.ToString(bonusData["CurrencyLeprechaun"]);
				}
				if (bonusData.ContainsKey("PerDayLeprechaun"))
				{
					LeprechauntPerDay = Convert.ToInt32(bonusData["PerDayLeprechaun"]);
				}
				if (bonusData.ContainsKey("DaysLeprechaun"))
				{
					LeprechauntDays = Convert.ToInt32(bonusData["DaysLeprechaun"]);
				}
			}
			else if (Action == BalanceController.gadgetActionName)
			{
				Type = BonusType.Gadgets;
				if (bonusData.ContainsKey("Gadgets"))
				{
					Gadgets = (List<string>)bonusData["Gadgets"];
				}
			}
			if (Type == BonusType.Unknown)
			{
				Type = BonusType.Currency;
			}
		}
	}
}

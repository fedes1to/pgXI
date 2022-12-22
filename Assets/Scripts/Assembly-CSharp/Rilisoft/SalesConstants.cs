using System.Collections.Generic;
using System;

namespace Rilisoft
{
	internal sealed class SalesConstants
	{
		private readonly List<string> _weaponSales = new List<string> { "Primary", "Back Up", "Melee", "Special", "Sniper", "Premium" };

		private readonly List<string> _equipmentSales = new List<string> { "Skins", "Armor", "Boots", "Capes", "Hats", "Gear", "Masks", "League" };

		private readonly List<string> _gadgetsSales = new List<string> { "Throwing Gadgets", "Tool Gadgets", "Support Gadgets" };

		private readonly List<string> _petsEggsSales = new List<string> { "Pets", "Eggs" };

		private static readonly Lazy<SalesConstants> s_instance = new Lazy<SalesConstants>(() => new SalesConstants());

		public static SalesConstants Instance
		{
			get
			{
				return s_instance.Value;
			}
		}

		private SalesConstants()
		{
		}

		public string GetSalesCategory(string category)
		{
			if (category == null)
			{
				return string.Empty;
			}
			if (_weaponSales.Contains(category))
			{
				return "Weapons Sales";
			}
			if (_equipmentSales.Contains(category))
			{
				return "Equipment Sales";
			}
			if (_gadgetsSales.Contains(category))
			{
				return "Gadgets Sales";
			}
			if (_petsEggsSales.Contains(category))
			{
				return "Pets and Eggs Sales";
			}
			return "Other";
		}
	}
}

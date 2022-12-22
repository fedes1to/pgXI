using System.Collections.Generic;

public static class PixelGunLowWeaponsTable
{
	public static readonly Dictionary<WeaponManager.WeaponTypeForLow, List<string>> table = new Dictionary<WeaponManager.WeaponTypeForLow, List<string>>(27, new WeaponManager.WeaponTypeForLowComparer())
	{
		{
			WeaponManager.WeaponTypeForLow.AssaultRifle_1,
			new List<string> { "Weapon3", "Weapon309", "Weapon309", "Weapon62", "Weapon62", "Weapon220" }
		},
		{
			WeaponManager.WeaponTypeForLow.AssaultRifle_2,
			new List<string> { "Weapon142", "Weapon142", "Weapon263", "Weapon263", "Weapon330", "Weapon330" }
		},
		{
			WeaponManager.WeaponTypeForLow.Shotgun_1,
			new List<string> { "Weapon2", "Weapon163", "Weapon163", "Weapon177", "Weapon177", "Weapon231" }
		},
		{
			WeaponManager.WeaponTypeForLow.Shotgun_2,
			new List<string> { "Weapon167", "Weapon167", "Weapon172", "Weapon172", "Weapon173", "Weapon173" }
		},
		{
			WeaponManager.WeaponTypeForLow.Machinegun,
			new List<string> { "Weapon127", "Weapon141", "Weapon207", "Weapon148", "Weapon159", "Weapon232" }
		},
		{
			WeaponManager.WeaponTypeForLow.Pistol_1,
			new List<string> { "Weapon1", "Weapon164", "Weapon164", "Weapon71", "Weapon71", "Weapon223" }
		},
		{
			WeaponManager.WeaponTypeForLow.Pistol_2,
			new List<string> { "Weapon364", "Weapon364", "Weapon150", "Weapon150", "Weapon152", "Weapon152" }
		},
		{
			WeaponManager.WeaponTypeForLow.Submachinegun,
			new List<string> { "Weapon160", "Weapon160", "Weapon54", "Weapon54", "Weapon175", "Weapon175" }
		},
		{
			WeaponManager.WeaponTypeForLow.Knife,
			new List<string> { "Weapon9", "Weapon9", "Weapon9", "Weapon9", "Weapon9", "Weapon9" }
		},
		{
			WeaponManager.WeaponTypeForLow.Sword,
			new List<string> { "Weapon131", "Weapon32", "Weapon30", "Weapon90", "Weapon155", "Weapon238" }
		},
		{
			WeaponManager.WeaponTypeForLow.Flamethrower_1,
			new List<string> { "Weapon333", "Weapon336", "Weapon336", "Weapon239", "Weapon283", "Weapon338" }
		},
		{
			WeaponManager.WeaponTypeForLow.Flamethrower_2,
			new List<string> { "Weapon281", "Weapon281", "Weapon81", "Weapon81", "Weapon124", "Weapon124" }
		},
		{
			WeaponManager.WeaponTypeForLow.SniperRifle_1,
			new List<string> { "Weapon67", "Weapon339", "Weapon339", "Weapon340", "Weapon340", "Weapon221" }
		},
		{
			WeaponManager.WeaponTypeForLow.SniperRifle_2,
			new List<string> { "Weapon61", "Weapon188", "Weapon346", "Weapon192", "Weapon211", "Weapon242" }
		},
		{
			WeaponManager.WeaponTypeForLow.Bow,
			new List<string> { "Weapon256", "Weapon27", "Weapon268", "Weapon268", "Weapon269", "Weapon269" }
		},
		{
			WeaponManager.WeaponTypeForLow.RocketLauncher_1,
			new List<string> { "Weapon162", "Weapon162", "Weapon162", "Weapon254", "Weapon254", "Weapon254" }
		},
		{
			WeaponManager.WeaponTypeForLow.RocketLauncher_2,
			new List<string> { "Weapon75", "Weapon76", "Weapon76", "Weapon76", "Weapon76", "Weapon248" }
		},
		{
			WeaponManager.WeaponTypeForLow.RocketLauncher_3,
			new List<string> { "Weapon82", "Weapon82", "Weapon157", "Weapon157", "Weapon158", "Weapon158" }
		},
		{
			WeaponManager.WeaponTypeForLow.GrenadeLauncher,
			new List<string> { "Weapon80", "Weapon143", "Weapon253", "Weapon140", "Weapon274", "Weapon222" }
		},
		{
			WeaponManager.WeaponTypeForLow.Snaryad,
			new List<string> { "Weapon166", "Weapon261", "Weapon168", "Weapon272", "Weapon169", "Weapon273" }
		},
		{
			WeaponManager.WeaponTypeForLow.Snaryad_Otskok,
			new List<string> { "Weapon303", "Weapon303", "Weapon304", "Weapon304", "Weapon305", "Weapon305" }
		},
		{
			WeaponManager.WeaponTypeForLow.Snaryad_Disk,
			new List<string> { "Weapon331", "Weapon331", "Weapon260", "Weapon260", "Weapon349", "Weapon349" }
		},
		{
			WeaponManager.WeaponTypeForLow.Railgun,
			new List<string> { "Weapon77", "Weapon77", "Weapon121", "Weapon121", "Weapon122", "Weapon122" }
		},
		{
			WeaponManager.WeaponTypeForLow.Ray,
			new List<string> { "Weapon178", "Weapon105", "Weapon133", "Weapon156", "Weapon306", "Weapon243" }
		},
		{
			WeaponManager.WeaponTypeForLow.AOE,
			new List<string> { "Weapon297", "Weapon278", "Weapon324", "Weapon279", "Weapon325", "Weapon224" }
		},
		{
			WeaponManager.WeaponTypeForLow.Instant_Area_Damage,
			new List<string> { "Weapon427", "Weapon427", "Weapon427", "Weapon251", "Weapon429", "Weapon271" }
		},
		{
			WeaponManager.WeaponTypeForLow.X3_Snaryad,
			new List<string> { "Weapon255", "Weapon255", "Weapon255", "Weapon255", "Weapon275", "Weapon275" }
		}
	};
}

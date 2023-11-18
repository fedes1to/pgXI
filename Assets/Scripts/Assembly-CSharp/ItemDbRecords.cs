using System.Collections.Generic;

public static class ItemDbRecords
{
	public static List<ItemRecord> GetRecords()
	{
		List<ItemRecord> list = new List<ItemRecord>(145);
		list.Add(new ItemRecord(1, "FirstPistol", null, "Weapon1", null, null, 0, false, false));
		list.Add(new ItemRecord(2, "FirstShotgun", null, "Weapon2", null, null, 0, false, false));
		list.Add(new ItemRecord(3, "UziWeapon", null, "Weapon3", null, null, 0, false, false));
		list.Add(new ItemRecord(4, "Revolver", "Revolver", "Weapon4", "Revolver", "Revolver", 0, true, false));
		list.Add(new ItemRecord(5, "Machingun", "Machingun", "Weapon5", "Machingun", "Machingun", 0, true, false));
		list.Add(new ItemRecord(6, "MinersWeapon", "MinerWeaponSett", "Weapon6", "MinerWeapon", "Miner Weapon", 35, true, false));
		list.Add(new ItemRecord(7, "CrystalSword", "SwordSett", "Weapon7", "crystalsword", "Crystal Sword", 185, true, false, "Coins", 93));
		list.Add(new ItemRecord(8, "AK47", "AK47", "Weapon8", "AK47", "AK47", 0, true, false));
		list.Add(new ItemRecord(9, "Knife", null, "Weapon9", null, null, 0, false, false));
		list.Add(new ItemRecord(10, "m16", "CombatRifleSett", "Weapon10", "combatrifle", "Combat Rifle", 85, true, false));
		list.Add(new ItemRecord(11, "Eagle 1", "GoldenEagleSett", "Weapon11", "goldeneagle", "Golden Eagle", 75, true, false));
		list.Add(new ItemRecord(12, "Bow", "MagicBowSett", "Weapon12", "magicbow", "Magic Bow", 40, false, true));
		list.Add(new ItemRecord(13, "SPAS", "SPASSett", "Weapon13", "spas", "Mega Destroyer", 60, true, false));
		list.Add(new ItemRecord(14, "GoldenAxe", "GoldenAxeSett", "Weapon14", "axe", "Golden Axe", 85, true, false));
		list.Add(new ItemRecord(15, "Chainsaw", "ChainsawS", "Weapon15", "chainsaw", "Tiny Chainsaw", 100, true, true));
		list.Add(new ItemRecord(16, "FAMAS", "FAMASS", "Weapon16", "famas", "Elite Rifle", 120, true, false));
		list.Add(new ItemRecord(17, "Glock", "GlockSett", "Weapon17", "glock", "Fast Death", 50, true, false));
		list.Add(new ItemRecord(18, "Scythe", "ScytheSN", "Weapon18", "scythe", "Creeper's Scythe", 150, true, true));
		list.Add(new ItemRecord(19, "Shovel", "ShovelSN", "Weapon19", "shovel", "Battle Shovel", 30, true, false));
		list.Add(new ItemRecord(20, "Hammer", "HammerSN", "Weapon20", "hammer", "Big Pig Hammer", 85, true, false));
		list.Add(new ItemRecord(21, "Sword_2", "Sword_2_SN", "Weapon21", "sword_2", "Skeleton Sword", 255, true, true));
		list.Add(new ItemRecord(22, "Staff", "StaffSN", "Weapon22", "staff", "Wizard's Arsenal", 200, true, true));
		list.Add(new ItemRecord(23, "Red_Stone", "LaserRifleSN", "Weapon23", "laser", "Redstone Cannon", 340, true, true));
		list.Add(new ItemRecord(24, "LightSword", "LightSwordSN", "Weapon24", "lightsword", "Space Saber", 340, true, true));
		list.Add(new ItemRecord(25, "Beretta", "BerettaSN", "Weapon25", "beretta", "Killer Mushroom", 95, true, false));
		list.Add(new ItemRecord(26, "Mace", "MaceSN", "Weapon26", "mace", "Cactus Flail", 120, true, true));
		list.Add(new ItemRecord(27, "Crossbow", "CrossbowSN", "Weapon27", "crossbow", "Royal Crossbow", 120, true, false));
		list.Add(new ItemRecord(28, "Minigun", "MinigunSN", "Weapon28", "minigun", "Automatic Peacemaker", 300, true, true));
		list.Add(new ItemRecord(29, "GoldenPick", "GoldenPickSN", "Weapon29", "goldenPick", "Miner Weapon 2", 70, true, false));
		list.Add(new ItemRecord(30, "CrystalPick", "CrystakPickSN", "Weapon30", "crystalPick", "Miner Weapon 3", 85, true, false, "Coins", 50));
		list.Add(new ItemRecord(31, "IronSword", "IronSwordSN", "Weapon31", "ironSword", "Iron Sword 1", 70, true, false));
		list.Add(new ItemRecord(32, "GoldenSword", "GoldenSwordSN", "Weapon32", "goldenSword", "Iron Sword 2", 120, true, false));
		list.Add(new ItemRecord(33, "GoldenRed_Stone", "GoldenRed_StoneSN", "Weapon33", "goldenRedStone", "Redstone Cannon 2", 340, true, true));
		list.Add(new ItemRecord(34, "GoldenSPAS", "GoldenSPASSN", "Weapon34", "goldenSPAS", "Mega Destroyer 2", 30, true, false));
		list.Add(new ItemRecord(35, "GoldenGlock", "GoldenGlockSN", "Weapon35", "goldenGlock", "Fast Death 2", 70, true, false));
		list.Add(new ItemRecord(36, "RedMinigun", "RedMinigunSN", "Weapon36", "redMinigun", "Automatic Peacemaker 2", 300, true, true));
		list.Add(new ItemRecord(37, "CrystalCrossbow", "CrystalCrossbowSN", "Weapon37", "crystalCrossbow", "Royal Crossbow 2", 150, true, false, "Coins", 79));
		list.Add(new ItemRecord(38, "RedLightSaber", "RedLightSaberSN", "Weapon38", "redLightSaber", "Dark Force Saber", 140, true, true));
		list.Add(new ItemRecord(39, "SandFamas", "SandFamasSN", "Weapon39", "sandFamas", "Spec OPS Rifle", 150, true, false));
		list.Add(new ItemRecord(40, "WhiteBeretta", "WhiteBerettaSN", "Weapon40", "whiteBeretta", "Assassin Mushroom", 120, true, false));
		list.Add(new ItemRecord(41, "BlackEagle", "BlackEagleSN", "Weapon41", "blackEagle", "Black Python", 65, true, false));
		list.Add(new ItemRecord(42, "CrystalAxe", "CrystalAxeSN", "Weapon42", "crystalAxe", "Crystal Double Axe", 170, true, false, "Coins", 86));
		list.Add(new ItemRecord(43, "SteelAxe", "SteelAxeSN", "Weapon43", "steelAxe", "Steel Axe", 50, true, false));
		list.Add(new ItemRecord(44, "WoodenBow", "WoodenBowSN", "Weapon44", "woodenBow", "Wooden Bow", 100, true, false));
		list.Add(new ItemRecord(45, "Chainsaw 2", "Chainsaw2SN", "Weapon45", "chainsaw2", "Laser Chainsaw", 100, true, false));
		list.Add(new ItemRecord(46, "SteelCrossbow", "SteelCrossbowSN", "Weapon46", "steelCrossbow", "Steel Crossbow", 120, true, false));
		list.Add(new ItemRecord(47, "Hammer 2", "Hammer2SN", "Weapon47", "hammer2", "Pigman Hammer", 150, true, false));
		list.Add(new ItemRecord(48, "Mace 2", "Mace2SN", "Weapon48", "mace2", "Lava Flail", 120, true, false));
		list.Add(new ItemRecord(49, "Sword_2 2", "Sword_22SN", "Weapon49", "sword_22", "Fire Demon", 255, true, true));
		list.Add(new ItemRecord(50, "Staff 2", "Staff2SN", "Weapon50", "staff2", "Archimage Dragon Wand", 200, true, false));
		list.Add(new ItemRecord(51, "DoubleShotgun", "DoubleShotgun", "Weapon51", "DoubleShotgun", "DoubleShotgun", 0, true, false));
		list.Add(new ItemRecord(52, "AlienGun", "AlienGun", "Weapon52", "AlienGun", "AlienGun", 0, true, false));
		list.Add(new ItemRecord(53, "m16_2", "m16_2", "Weapon53", "m16_2", "m16_2", 0, true, false));
		list.Add(new ItemRecord(54, "CrystalGlock", "CrystalGlockSN", "Weapon54", "crystalGlock", "Fast Death 3", 135, true, false, "Coins", 65));
		list.Add(new ItemRecord(55, "CrystalSPAS", "CrystalSPASSN", "Weapon55", "crystalSPAS", "Mega Destroyer 3", 45, true, false));
		list.Add(new ItemRecord(56, "Tree", "TreeSN", "Weapon56", "tree", "Christmas Sword", 75, true, false));
		list.Add(new ItemRecord(57, "Fire_Axe", "FireAxeSN", "Weapon57", "fireAxe", "Happy Tree Slayer", 135, true, false));
		list.Add(new ItemRecord(58, "3pl_Shotgun", "_3PLShotgunSN", "Weapon58", "_3plShotgun", "Deadly ??andy", 135, true, true));
		list.Add(new ItemRecord(59, "Revolver2", "Revolver2SN", "Weapon59", "revolver2", "Powerful Gift", 255, true, true));
		list.Add(new ItemRecord(60, "Barrett50Cal", "BarrettSN", "Weapon60", "barrett", "Brutal Headhunter", 150, true, true));
		list.Add(new ItemRecord(61, "SVD", "SVDSN", "Weapon61", "svd", "Guerilla Rifle", 135, true, false));
		list.Add(new ItemRecord(62, "NavyFamas", "NavyFamasSN", "Weapon62", "navyFamas", "SWAT Rifle", 220, true, false, "Coins", 109));
		list.Add(new ItemRecord(63, "SVD_2", "SVD_2SN", "Weapon63", "svd_2", "Guerilla Rifle M2", 135, true, false));
		list.Add(new ItemRecord(64, "Eagle_3", "Eagle_3SN", "Weapon64", "eagle3", "Deadly Viper", 150, true, false, "Coins", 72));
		list.Add(new ItemRecord(65, "Barrett_2", "Barrett2SN", "Weapon65", "barrett_2", "Ultimate Headhunter", 150, true, false));
		list.Add(new ItemRecord(66, "Uzi2", "Uzi2", "Weapon66", "Uzi2", "Uzi2", 0, true, false));
		list.Add(new ItemRecord(67, "Hunter_Rifle", null, "Weapon67", null, null, 0, false, false));
		list.Add(new ItemRecord(68, "Scythe_2", "Scythe_2_SN", "Weapon68", "scythe2", "Laser Scythe", 150, true, true));
		list.Add(new ItemRecord(69, "m16_3", "m_16_3_Sett", "Weapon69", "m16_3", "Combat Rifle M2", 100, true, false));
		list.Add(new ItemRecord(70, "m16_4", "m_16_4_Sett", "Weapon70", "m16_4", "Combat Rifle M3", 150, true, false, "Coins", 80));
		list.Add(new ItemRecord(71, "BlackBeretta", "Beretta_2_SN", "Weapon71", "beretta2", "Amanita", 200, true, false, "Coins", 97));
		list.Add(new ItemRecord(72, "Tree_2", "Tree_2_SN", "Weapon72", "tree2", "Cactus Sword", 35, true, false));
		list.Add(new ItemRecord(73, "Flamethrower", "FlameThrowerSN", "Weapon73", "flamethrower", "Total Exterminator", 170, true, true));
		list.Add(new ItemRecord(74, "Flamethrower_2", "FlameThrower_2SN", "Weapon74", "flamethrower_2", "Doomsda Flamethrower", 170, true, true));
		list.Add(new ItemRecord(75, "Bazooka", "BazookaSN", "Weapon75", "bazooka", "Apocalypse 3000", 150, true, false));
		list.Add(new ItemRecord(76, "Bazooka_2", "Bazooka_2SN", "Weapon76", "bazooka_2", "Nuclear Launcher", 150, true, false));
		list.Add(new ItemRecord(77, "railgun", "RailgunSN", "Weapon77", "railgun", "Prototype PSR-1", 320, true, false));
		list.Add(new ItemRecord(78, "tesla", "TeslaSN", "Weapon78", "tesla", "Tesla Generator", 270, true, false));
		list.Add(new ItemRecord(79, "grenade_launcher", "GrenadeLnchSN", "Weapon79", "greandeLauncher", "Grenade Launcher", 135, true, false));
		list.Add(new ItemRecord(80, "grenade_launcher2", "GrenadeLnch_2SN", "Weapon80", "greandeLauncher_2", "Firestorm G2", 185, true, false));
		list.Add(new ItemRecord(81, "tesla_2", "Tesla_2SN", "Weapon81", "tesla_2", "Chain Thunderbolt", 170, true, false));
		list.Add(new ItemRecord(82, "Bazooka_3", "Bazooka_3SN", "Weapon82", "bazooka_3", "Armageddon", 340, true, false));
		list.Add(new ItemRecord(83, "GravityGun", "GravigunSN", "Weapon83", "gravigun", "Gravity Gun", 170, true, false));
		list.Add(new ItemRecord(84, "AUG", "AUGSett", "Weapon84", "aug", "Marksman M1", 185, true, false));
		list.Add(new ItemRecord(85, "AUG_2", "AUG_2SN", "Weapon85", "aug_2", "Marksman M2", 170, true, false));
		list.Add(new ItemRecord(86, "Razer", "RazerSN", "Weapon86", "razer", "Razor1", 200, true, true));
		list.Add(new ItemRecord(87, "Razer_2", "Razer_2SN", "Weapon87", "razer_2", "Razor2", 200, true, true));
		list.Add(new ItemRecord(88, "katana", "katana_SN", "Weapon88", "katana", "katana", 185, true, true));
		list.Add(new ItemRecord(89, "katana_2", "katana_2_SN", "Weapon89", "katana_2", "katana 2", 185, true, true));
		list.Add(new ItemRecord(90, "katana_3", "katana_3_SN", "Weapon90", "katana_3", "katana 3", 185, true, false));
		list.Add(new ItemRecord(91, "plazma", "plazmaSN", "Weapon91", "plazma", " Plasma Rifle PZ-1", 155, true, true));
		list.Add(new ItemRecord(92, "plazma_pistol", "plazma_pistol_SN", "Weapon92", "plazma_pistol", " Plasma Pistol PZ-1", 100, true, true));
		list.Add(new ItemRecord(93, "flowerpower", "FlowePowerSN", "Weapon93", "flower_power", "FlowerPower", 135, true, true));
		list.Add(new ItemRecord(94, "bigbuddy", "BuddySN", "Weapon94", "buddy", "BigBuddy", 185, true, false));
		list.Add(new ItemRecord(95, "Mauser", "MauserSN", "Weapon95", "mauser", "Old Comrade", 120, true, false));
		list.Add(new ItemRecord(96, "Shmaiser", "ShmaiserSN", "Weapon96", "shmaiser", "Eindringling", 200, true, false));
		list.Add(new ItemRecord(97, "Tompson", "ThompsonSN", "Weapon97", "thompson", "Brave Patriot", 170, true, false));
		list.Add(new ItemRecord(98, "Tompson_2", "Thompson_2SN", "Weapon98", "thompson_2", "State Defender", 235, true, false));
		list.Add(new ItemRecord(99, "basscannon", "BassCannonSN", "Weapon99", "basscannon", "Basscannon", 255, true, false));
		list.Add(new ItemRecord(100, "SparklyBlaster", "SparklyBlasterSN", "Weapon100", "sparklyBlaster", "Sparkly Blaster", 120, true, false));
		list.Add(new ItemRecord(101, "CherryGun", "CherryGunSN", "Weapon101", "cherry", "Cherru BOMB", 270, true, false));
		list.Add(new ItemRecord(102, "AK74", "AK74_SN", "Weapon102", "ak74", "AK74 1", 100, true, false));
		list.Add(new ItemRecord(103, "AK74_2", "AK74_2_SN", "Weapon103", "ak74_2", "AK74 2", 120, true, false));
		list.Add(new ItemRecord(104, "AK74_3", "AK74_3_SN", "Weapon104", "ak74_3", "AK74 3", 170, true, false, "Coins", 59));
		list.Add(new ItemRecord(105, "FreezeGun", "FreezeGun_SN", "Weapon105", "freeze", "FreezeGun", 340, true, false, "Coins", 175));
		list.Add(new ItemRecord(106, "3pl_Shotgun_2", "_3_shotgun_2", "Weapon107", "_3_shotgun_2", null, 135, true, true));
		list.Add(new ItemRecord(107, "3pl_Shotgun_3", "_3_shotgun_3", "Weapon108", "_3_shotgun_3", null, 135, true, false));
		list.Add(new ItemRecord(108, "flowerpower_2", "flower_2", "Weapon109", "flower_2", null, 135, true, true));
		list.Add(new ItemRecord(109, "flowerpower_3", "flower_3", "Weapon110", "flower_3", null, 135, true, false));
		list.Add(new ItemRecord(110, "GravityGun_2", "gravity_2", "Weapon111", "gravity_2", null, 195, true, false));
		list.Add(new ItemRecord(111, "GravityGun_3", "gravity_3", "Weapon112", "gravity_3", null, 250, true, false, "Coins", 135));
		list.Add(new ItemRecord(112, "grenade_launcher3", "grenade_launcher_3", "Weapon113", "grenade_launcher_3", null, 210, true, false));
		list.Add(new ItemRecord(113, "Revolver3", "revolver_2_2", "Weapon114", "revolver_2_2", null, 255, true, true));
		list.Add(new ItemRecord(114, "Revolver4", "revolver_2_3", "Weapon115", "revolver_2_3", null, 215, true, false));
		list.Add(new ItemRecord(115, "Scythe_3", "scythe_3", "Weapon116", "scythe_3", null, 150, true, false));
		list.Add(new ItemRecord(116, "plazma_2", "plazma_2", "Weapon117", "plazma_2", null, 155, true, true));
		list.Add(new ItemRecord(117, "plazma_3", "plazma_3", "Weapon118", "plazma_3", null, 155, true, false));
		list.Add(new ItemRecord(118, "plazma_pistol_2", "plazma_pistol_2", "Weapon119", "plazma_pistol_2", null, 105, true, false));
		list.Add(new ItemRecord(119, "plazma_pistol_3", "plazma_pistol_3", "Weapon120", "plazma_pistol_3", null, 150, true, false));
		list.Add(new ItemRecord(120, "railgun_2", "railgun_2", "Weapon121", "railgun_2", null, 220, true, false));
		list.Add(new ItemRecord(121, "railgun_3", "railgun_3", "Weapon122", "railgun_3", null, 145, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(122, "Razer_3", "Razer_3", "Weapon123", "Razer_3", null, 200, true, false));
		list.Add(new ItemRecord(123, "tesla_3", "tesla_3", "Weapon124", "tesla_3", null, 139, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(124, "Flamethrower_3", "Flamethrower_3", "Weapon125", "Flamethrower_3", null, 170, true, false));
		list.Add(new ItemRecord(125, "FreezeGun_0", "FreezeGun_0", "Weapon126", "FreezeGun_0", null, 340, true, false, "Coins", 155));
		list.Add(new ItemRecord(126, "Minigun_3", "minigun_3", "Weapon127", "minigun_3", null, 300, true, false));
		list.Add(new ItemRecord(127, "SVD_3", "svd_3", "Weapon128", "svd_3", null, 170, true, false, "Coins", 85));
		list.Add(new ItemRecord(128, "Barret_3", "barret_3", "Weapon129", "barret_3", "barret_3", 220, true, false));
		list.Add(new ItemRecord(129, "LightSword_3", "LightSword_3", "Weapon130", "LightSword_3", null, 340, true, false, "Coins", 145));
		list.Add(new ItemRecord(130, "Sword_2_3", "Sword_2_3", "Weapon131", "Sword_2_3", null, 255, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(131, "Staff 3", "Staff 3", "Weapon132", "Staff 3", null, 235, true, false));
		list.Add(new ItemRecord(132, "DragonGun", "DragonGun", "Weapon133", "DragonGun", null, 425, true, false, "Coins", 211));
		list.Add(new ItemRecord(133, "Bow_3", "Bow_3", "Weapon134", "Bow_3", null, 185, true, false));
		list.Add(new ItemRecord(134, "Bazooka_1_3", "Bazooka_1_3", "Weapon135", "Bazooka_1_3", null, 185, true, false, "Coins", 93));
		list.Add(new ItemRecord(135, "Bazooka_2_1", "Bazooka_2_1", "Weapon136", "Bazooka_2_1", null, 285, true, true));
		list.Add(new ItemRecord(136, "Bazooka_2_3", "Bazooka_2_3", "Weapon137", "Bazooka_2_3", null, 285, true, false));
		list.Add(new ItemRecord(137, "m79_2", "m79_2", "Weapon138", "m79_2", null, 220, true, true));
		list.Add(new ItemRecord(138, "m79_3", "m79_3", "Weapon139", "m79_3", null, 220, true, false));
		list.Add(new ItemRecord(139, "m32_1_2", "m32_1_2", "Weapon140", "m32_1_2", null, 255, true, false, "Coins", 127));
		list.Add(new ItemRecord(140, "Red_Stone_3", "Red_Stone_3", "Weapon141", "Red_Stone_3", "RedStone3", 340, true, false, "Coins", 155, true));
		list.Add(new ItemRecord(141, "XM8_1", "XM8_1", "Weapon142", "XM8_1", "XM8_1", 295, true, false));
		list.Add(new ItemRecord(143, "XM8_2", "XM8_2", "Weapon144", "XM8_2", "XM8_2", 170, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(144, "XM8_3", "XM8_3", "Weapon145", "XM8_3", "XM8_3", 131, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(146, "Red_Stone_4", "Red_Stone_4", "Weapon148", "Red_Stone_4", "Red_Stone_4", 205, true, false, "Coins", 88, true));
		list.Add(new ItemRecord(147, "Minigun_4", "Minigun_4", "Weapon149", "Minigun_4", "Minigun_4", 160, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(148, "Revolver5", "Revolver5", "Weapon150", "Revolver5", "Revolver5", 135, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(149, "FreezeGun_0_2", "FreezeGun_0_2", "Weapon151", "FreezeGun_0_2", "FreezeGun_0_2", 78, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(150, "Revolver6", "Revolver6", "Weapon152", "Revolver6", "Revolver6", 100, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(151, "Sword_2_4", "Sword_2_4", "Weapon153", "Sword_2_4", "Sword_2_4", 120, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(152, "LightSword_4", "LightSword_4", "Weapon154", "LightSword_4", "LightSword_4", 205, true, false, "Coins", 102, true));
		list.Add(new ItemRecord(153, "Sword_2_5", "Sword_2_5", "Weapon155", "Sword_2_5", "Sword_2_5", 127, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(154, "FreezeGun_2", "FreezeGun_2", "Weapon156", "FreezeGun_2", "FreezeGun_2", 190, true, false, "Coins", 112, true));
		list.Add(new ItemRecord(155, "Bazooka_3_2", "Bazooka_3_2", "Weapon157", "Bazooka_3_2", "Bazooka_3_2", 190, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(156, "Bazooka_3_3", "Bazooka_3_3", "Weapon158", "Bazooka_3_3", "Bazooka_3_3", 144, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(157, "Minigun_5", "Minigun_5", "Weapon159", "Minigun_5", "Minigun_5", 112, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(158, "TwoBolters", "TwoBolters", "Weapon160", "TwoBolters", "TwoBolters", 150, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(159, "RayMinigun", "RayMinigun", "Weapon161", "RayMinigun", "RayMinigun", 297, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(160, "SignalPistol", null, "Weapon162", null, null, 0, false, false));
		list.Add(new ItemRecord(161, "AutoShotgun", "AutoShotgun", "Weapon163", "AutoShotgun", "AutoShotgun", 320, true, false, "Coins", 158));
		list.Add(new ItemRecord(162, "TwoRevolvers", "TwoRevolvers", "Weapon164", "TwoRevolvers", "TwoRevolvers", 220, true, false, "Coins", 109));
		list.Add(new ItemRecord(163, "SnowballGun", "SnowballGun", "Weapon165", "SnowballGun", "SnowballGun", 170, true, false));
		list.Add(new ItemRecord(164, "SnowballMachingun", "SnowballMachingun", "Weapon166", "SnowballMachingun", "SnowballMachingun", 150, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(165, "HeavyShotgun", "HeavyShotgun", "Weapon167", "HeavyShotgun", "HeavyShotgun", 153, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(166, "SnowballMachingun_2", "SnowballMachingun_2", "Weapon168", "SnowballMachingun_2", "SnowballMachingun_2", 97, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(167, "SnowballMachingun_3", "SnowballMachingun_3", "Weapon169", "SnowballMachingun_3", "SnowballMachingun_3", 159, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(168, "SnowballGun_2", "SnowballGun_2", "Weapon170", "SnowballGun_2", "SnowballGun_2", 100, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(169, "SnowballGun_3", "SnowballGun_3", "Weapon171", "SnowballGun_3", "SnowballGun_3", 102, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(170, "HeavyShotgun_2", "HeavyShotgun_2", "Weapon172", "HeavyShotgun_2", "HeavyShotgun_2", 85, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(171, "HeavyShotgun_3", "HeavyShotgun_3", "Weapon173", "HeavyShotgun_3", "HeavyShotgun_3", 102, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(172, "TwoBolters_2", "TwoBolters_2", "Weapon174", "TwoBolters_2", "TwoBolters_2", 78, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(173, "TwoBolters_3", "TwoBolters_3", "Weapon175", "TwoBolters_3", "TwoBolters_3", 112, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(174, "TwoRevolvers_2", "TwoRevolvers_2", "Weapon176", "TwoRevolvers_2", "TwoRevolvers_2", 76, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(175, "AutoShotgun_2", "AutoShotgun_2", "Weapon177", "AutoShotgun_2", "AutoShotgun_2", 92, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(176, "Solar_Ray", "Solar_Ray", "Weapon178", "Solar_Ray", "Solar_Ray", 169, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(177, "Water_Pistol", "Water_Pistol", "Weapon179", "Water_Pistol", "Water_Pistol", 120, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(178, "Solar_Power_Cannon", "Solar_Power_Cannon", "Weapon180", "Solar_Power_Cannon", "Solar_Power_Cannon", 169, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(179, "Water_Rifle", "Water_Rifle", "Weapon181", "Water_Rifle", "Water_Rifle", 272, true, false));
		list.Add(new ItemRecord(180, "Water_Rifle_2", "Water_Rifle_2", "Weapon182", "Water_Rifle_2", "Water_Rifle_2", 153, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(181, "Water_Rifle_3", "Water_Rifle_3", "Weapon183", "Water_Rifle_3", "Water_Rifle_3", 110, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(182, "Water_Pistol_2", "Water_Pistol_2", "Weapon184", "Water_Pistol_2", "Water_Pistol_2", 85, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(183, "Solar_Power_Cannon_2", "Solar_Power_Cannon_2", "Weapon185", "Solar_Power_Cannon_2", "Solar_Power_Cannon_2", 115, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(184, "Solar_Ray_2", "Solar_Ray_2", "Weapon186", "Solar_Ray_2", "Solar_Ray_2", 89, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(185, "Solar_Ray_3", "Solar_Ray_3", "Weapon187", "Solar_Ray_3", "Solar_Ray_3", 153, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(186, "Needle_Throw", "Needle_Throw", "Weapon188", "Needle_Throw", "Needle_Throw", 160, true, false));
		list.Add(new ItemRecord(187, "Valentine_Shotgun", "Valentine_Shotgun", "Weapon189", "Valentine_Shotgun", "Valentine_Shotgun", 255, true, false));
		list.Add(new ItemRecord(188, "Valentine_Shotgun_2", "Valentine_Shotgun_2", "Weapon190", "Valentine_Shotgun_2", "Valentine_Shotgun_2", 153, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(189, "Valentine_Shotgun_3", "Valentine_Shotgun_3", "Weapon191", "Valentine_Shotgun_3", "Valentine_Shotgun_3", 99, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(190, "Needle_Throw_2", "Needle_Throw_2", "Weapon192", "Needle_Throw_2", "Needle_Throw_2", 150, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(242, "Needle_Throw_3", "Needle_Throw_3", "Weapon242", "Needle_Throw_3", "Needle_Throw_3", 125, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(191, "RailRevolver_1", null, "Weapon193", "RailRevolver_1", "RailRevolver_1", 5, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(192, "Assault_Machine_Gun", null, "Weapon194", "Assault_Machine_Gun", "Assault_Machine_Gun", 5, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(193, "Impulse_Sniper_Rifle", null, "Weapon195", "Impulse_Sniper_Rifle", "Impulse_Sniper_Rifle", 6, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(194, "Autoaim_Rocketlauncher", null, "Weapon196", "Autoaim_Rocketlauncher", "Autoaim_Rocketlauncher", 8, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(195, "Carrot_Sword", "Carrot_Sword", "Weapon197", "Carrot_Sword", "Carrot_Sword", 221, true, false));
		list.Add(new ItemRecord(199, "Carrot_Sword_2", "Carrot_Sword_2", "Weapon201", "Carrot_Sword_2", "Carrot_Sword_2", 102, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(200, "Carrot_Sword_3", "Carrot_Sword_3", "Weapon202", "Carrot_Sword_3", "Carrot_Sword_3", 119, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(201, "RailRevolverBuy", "RailRevolverBuy", "Weapon203", "RailRevolverBuy", "RailRevolverBuy", true, false, new List<ItemPrice>
		{
			new ItemPrice(165, "GemsCurrency"),
			new ItemPrice(165, "GemsCurrency"),
			new ItemPrice(165, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(202, "RailRevolverBuy_2", "RailRevolverBuy_2", "Weapon204", "RailRevolverBuy_2", "RailRevolverBuy_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(75, "GemsCurrency"),
			new ItemPrice(197, "GemsCurrency"),
			new ItemPrice(177, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(203, "RailRevolverBuy_3", "RailRevolverBuy_3", "Weapon205", "RailRevolverBuy_3", "RailRevolverBuy_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(128, "GemsCurrency"),
			new ItemPrice(128, "GemsCurrency"),
			new ItemPrice(234, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(196, "Easter_Bazooka", "Easter_Bazooka", "Weapon198", "Easter_Bazooka", "Easter_Bazooka", true, false, new List<ItemPrice>
		{
			new ItemPrice(272, "Coins"),
			new ItemPrice(272, "Coins"),
			new ItemPrice(272, "Coins")
		}, true));
		list.Add(new ItemRecord(197, "Easter_Bazooka_2", "Easter_Bazooka_2", "Weapon199", "Easter_Bazooka_2", "Easter_Bazooka_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(153, "Coins"),
			new ItemPrice(315, "Coins"),
			new ItemPrice(315, "Coins")
		}, true));
		list.Add(new ItemRecord(198, "Easter_Bazooka_3", "Easter_Bazooka_3", "Weapon200", "Easter_Bazooka_3", "Easter_Bazooka_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(130, "GemsCurrency"),
			new ItemPrice(130, "GemsCurrency"),
			new ItemPrice(235, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(204, "Assault_Machine_GunBuy", "Assault_Machine_GunBuy", "Weapon206", "Assault_Machine_GunBuy", "Assault_Machine_GunBuy", true, false, new List<ItemPrice>
		{
			new ItemPrice(189, "GemsCurrency"),
			new ItemPrice(189, "GemsCurrency"),
			new ItemPrice(189, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(205, "Assault_Machine_GunBuy_2", "Assault_Machine_GunBuy_2", "Weapon207", "Assault_Machine_GunBuy_2", "Assault_Machine_GunBuy_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(95, "GemsCurrency"),
			new ItemPrice(213, "GemsCurrency"),
			new ItemPrice(189, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(206, "Assault_Machine_GunBuy_3", "Assault_Machine_GunBuy_3", "Weapon208", "Assault_Machine_GunBuy_3", "Assault_Machine_GunBuy_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(139, "GemsCurrency"),
			new ItemPrice(139, "GemsCurrency"),
			new ItemPrice(254, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(207, "Impulse_Sniper_RifleBuy", "Impulse_Sniper_RifleBuy", "Weapon209", "Impulse_Sniper_RifleBuy", "Impulse_Sniper_RifleBuy", true, false, new List<ItemPrice>
		{
			new ItemPrice(183, "GemsCurrency"),
			new ItemPrice(183, "GemsCurrency"),
			new ItemPrice(183, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(208, "Impulse_Sniper_RifleBuy_2", "Impulse_Sniper_RifleBuy_2", "Weapon210", "Impulse_Sniper_RifleBuy_2", "Impulse_Sniper_RifleBuy_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(125, "GemsCurrency"),
			new ItemPrice(231, "GemsCurrency"),
			new ItemPrice(183, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(209, "Impulse_Sniper_RifleBuy_3", "Impulse_Sniper_RifleBuy_3", "Weapon211", "Impulse_Sniper_RifleBuy_3", "Impulse_Sniper_RifleBuy_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(149, "GemsCurrency"),
			new ItemPrice(149, "GemsCurrency"),
			new ItemPrice(274, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(210, "Autoaim_RocketlauncherBuy", "Autoaim_RocketlauncherBuy", "Weapon212", "Autoaim_RocketlauncherBuy", "Autoaim_RocketlauncherBuy", true, false, new List<ItemPrice>
		{
			new ItemPrice(195, "GemsCurrency"),
			new ItemPrice(215, "GemsCurrency"),
			new ItemPrice(215, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(211, "Autoaim_RocketlauncherBuy_2", "Autoaim_RocketlauncherBuy_2", "Weapon213", "Autoaim_RocketlauncherBuy_2", "Autoaim_RocketlauncherBuy_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(112, "GemsCurrency"),
			new ItemPrice(245, "GemsCurrency"),
			new ItemPrice(215, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(212, "Autoaim_RocketlauncherBuy_3", "Autoaim_RocketlauncherBuy_3", "Weapon214", "Autoaim_RocketlauncherBuy_3", "Autoaim_RocketlauncherBuy_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(222, "GemsCurrency"),
			new ItemPrice(222, "GemsCurrency"),
			new ItemPrice(329, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(213, "TwoBoltersRent", null, "Weapon215", "TwoBoltersRent", "TwoBoltersRent", 6, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(214, "Red_StoneRent", null, "Weapon216", "Red_StoneRent", "Red_StoneRent", 6, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(215, "DragonGunRent", null, "Weapon217", "DragonGunRent", "DragonGunRent", 6, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(216, "PumpkinGunRent", null, "Weapon218", "PumpkinGunRent", "PumpkinGunRent", 6, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(217, "RayMinigunRent", null, "Weapon219", "RayMinigunRent", "RayMinigunRent", 6, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(218, "PX-3000", "PX-3000", "Weapon220", "PX-3000", "PX-3000", 289, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(219, "Sunrise", "Sunrise", "Weapon221", "Sunrise", "Sunrise", 265, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(220, "Bastion", "Bastion", "Weapon222", "Bastion", "Bastion", 275, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(221, "SteamPower_2", "SteamPower_2", "Weapon225", "SteamPower_2", "SteamPower_2", 85, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(222, "SteamPower_3", "SteamPower_3", "Weapon226", "SteamPower_3", "SteamPower_3", 85, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(223, "PlasmaRifle_2", "PlasmaRifle_2", "Weapon227", "PlasmaRifle_2", "PlasmaRifle_2", 88, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(224, "PlasmaRifle_3", "PlasmaRifle_3", "Weapon228", "PlasmaRifle_3", "PlasmaRifle_3", 99, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(225, "StateDefender_2", "StateDefender_2", "Weapon229", "StateDefender_2", "StateDefender_2", 170, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(226, "AUG_3", "AUG_3", "Weapon230", "AUG_3", "AUG_3", 99, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(227, "AutoShotgun_3", "AutoShotgun_3", "Weapon231", "AutoShotgun_3", "AutoShotgun_3", 142, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(228, "Red_Stone_5", "Red_Stone_5", "Weapon232", "Red_Stone_5", "Red_Stone_5", 139, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(229, "SparklyBlaster_2", "SparklyBlaster_2", "Weapon233", "SparklyBlaster_2", "SparklyBlaster_2", 85, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(230, "SparklyBlaster_3", "SparklyBlaster_3", "Weapon234", "SparklyBlaster_3", "SparklyBlaster_3", 75, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(231, "TwoRevolvers_3", "TwoRevolvers_3", "Weapon235", "TwoRevolvers_3", "TwoRevolvers_3", 125, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(232, "Water_Pistol_3", "Water_Pistol_3", "Weapon236", "Water_Pistol_3", "Water_Pistol_3", 105, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(233, "katana_4", "katana_4", "Weapon237", "katana_4", "katana_4", 102, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(234, "LightSword_5", "LightSword_5", "Weapon238", "LightSword_5", "LightSword_5", 120, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(235, "Flamethrower_4", "Flamethrower_4", "Weapon239", "Flamethrower_4", "Flamethrower_4", 102, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(236, "Flamethrower_5", "Flamethrower_5", "Weapon240", "Flamethrower_5", "Flamethrower_5", 100, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(237, "Barret_4", "Barret_4", "Weapon241", "Barret_4", "Barret_4", 153, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(239, "FreezeGun_3", "FreezeGun_3", "Weapon243", "FreezeGun_3", "FreezeGun_3", 135, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(240, "bigbuddy_2", "bigbuddy_2", "Weapon244", "bigbuddy_2", "bigbuddy_2", 85, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(241, "bigbuddy_3", "bigbuddy_3", "Weapon245", "bigbuddy_3", "bigbuddy_3", 96, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(243, "Bazooka_2_4", "Bazooka_2_4", "Weapon247", "Bazooka_2_4", "Bazooka_2_4", 204, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(244, "Solar_Power_Cannon_3", "Solar_Power_Cannon_3", "Weapon248", "Solar_Power_Cannon_3", "Solar_Power_Cannon_3", 171, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(246, "StormHammer", "StormHammer", "Weapon224", "StormHammer", "StormHammer", 999, true, false, "Coins", -1, true));
		list.Add(new ItemRecord(247, "DualHawks", "DualHawks", "Weapon223", "DualHawks", "DualHawks", 265, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(248, "DragonGun_2", "DragonGun_2", "Weapon249", "DragonGun_2", "DragonGun_2", 150, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(249, "Badcode_gun", null, "Weapon250", null, null, 0, false, false));
		list.Add(new ItemRecord(189, "LuckyStrike", "LuckyStrike", "Weapon521", "LuckyStrike", "LuckyStrike", 99, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(258, "DualUzi", "DualUzi", "Weapon259", "DualUzi", "DualUzi", true, false, new List<ItemPrice>
		{
			new ItemPrice(255, "Coins"),
			new ItemPrice(149, "Coins"),
			new ItemPrice(274, "Coins")
		}, true));
		list.Add(new ItemRecord(262, "DualUzi_2", "DualUzi_2", "Weapon263", "DualUzi_2", "DualUzi_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(153, "Coins"),
			new ItemPrice(306, "Coins"),
			new ItemPrice(274, "Coins")
		}, true));
		list.Add(new ItemRecord(263, "DualUzi_3", "DualUzi_3", "Weapon264", "DualUzi_3", "DualUzi_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(99, "GemsCurrency"),
			new ItemPrice(99, "GemsCurrency"),
			new ItemPrice(255, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(251, "PlasmaShotgun", "PlasmaShotgun", "Weapon252", "PlasmaShotgun", "PlasmaShotgun", true, false, new List<ItemPrice>
		{
			new ItemPrice(170, "Coins"),
			new ItemPrice(99, "Coins"),
			new ItemPrice(255, "Coins")
		}, true));
		list.Add(new ItemRecord(264, "PlasmaShotgun_2", "PlasmaShotgun_2", "Weapon265", "PlasmaShotgun_2", "PlasmaShotgun_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(110, "Coins"),
			new ItemPrice(210, "Coins"),
			new ItemPrice(255, "Coins")
		}, true));
		list.Add(new ItemRecord(257, "RapidFireRifle", "RapidFireRifle", "Weapon258", "RapidFireRifle", "RapidFireRifle", true, false, new List<ItemPrice>
		{
			new ItemPrice(204, "Coins"),
			new ItemPrice(99, "Coins"),
			new ItemPrice(255, "Coins")
		}, true));
		list.Add(new ItemRecord(265, "RapidFireRifle_2", "RapidFireRifle_2", "Weapon266", "RapidFireRifle_2", "RapidFireRifle_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(108, "Coins"),
			new ItemPrice(240, "Coins"),
			new ItemPrice(255, "Coins")
		}, true));
		list.Add(new ItemRecord(256, "FutureRifle", "FutureRifle", "Weapon257", "FutureRifle", "FutureRifle", true, false, new List<ItemPrice>
		{
			new ItemPrice(235, "Coins"),
			new ItemPrice(99, "Coins"),
			new ItemPrice(255, "Coins")
		}, true));
		list.Add(new ItemRecord(276, "FutureRifle_2", "FutureRifle_2", "Weapon277", "FutureRifle_2", "FutureRifle_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(170, "Coins"),
			new ItemPrice(305, "Coins"),
			new ItemPrice(255, "Coins")
		}, true));
		list.Add(new ItemRecord(261, "Photon_Pistol", "Photon_Pistol", "Weapon262", "Photon_Pistol", "Photon_Pistol", true, false, new List<ItemPrice>
		{
			new ItemPrice(205, "Coins"),
			new ItemPrice(99, "Coins"),
			new ItemPrice(255, "Coins")
		}, true));
		list.Add(new ItemRecord(266, "Photon_Pistol_2", "Photon_Pistol_2", "Weapon267", "Photon_Pistol_2", "Photon_Pistol_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "Coins"),
			new ItemPrice(270, "Coins"),
			new ItemPrice(255, "Coins")
		}, true));
		list.Add(new ItemRecord(255, "TacticalBow", "TacticalBow", "Weapon256", "TacticalBow", "TacticalBow", true, false, new List<ItemPrice>
		{
			new ItemPrice(195, "Coins"),
			new ItemPrice(99, "Coins"),
			new ItemPrice(255, "Coins")
		}, true));
		list.Add(new ItemRecord(267, "TacticalBow_2", "TacticalBow_2", "Weapon268", "TacticalBow_2", "TacticalBow_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "Coins"),
			new ItemPrice(235, "Coins"),
			new ItemPrice(255, "Coins")
		}, true));
		list.Add(new ItemRecord(268, "TacticalBow_3", "TacticalBow_3", "Weapon269", "TacticalBow_3", "TacticalBow_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(150, "GemsCurrency"),
			new ItemPrice(150, "GemsCurrency"),
			new ItemPrice(265, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(259, "LaserDiscThower", "LaserDiscThower", "Weapon260", "LaserDiscThower", "LaserDiscThower", true, false, new List<ItemPrice>
		{
			new ItemPrice(200, "Coins"),
			new ItemPrice(150, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(269, "LaserDiscThower_2", "LaserDiscThower_2", "Weapon270", "LaserDiscThower_2", "LaserDiscThower_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "Coins"),
			new ItemPrice(255, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(250, "ElectroBlastRifle", "ElectroBlastRifle", "Weapon251", "ElectroBlastRifle", "ElectroBlastRifle", true, false, new List<ItemPrice>
		{
			new ItemPrice(220, "Coins"),
			new ItemPrice(150, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(270, "ElectroBlastRifle_2", "ElectroBlastRifle_2", "Weapon271", "ElectroBlastRifle_2", "ElectroBlastRifle_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(153, "Coins"),
			new ItemPrice(280, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(260, "Tesla_Cannon", "Tesla_Cannon", "Weapon261", "Tesla_Cannon", "Tesla_Cannon", true, false, new List<ItemPrice>
		{
			new ItemPrice(200, "Coins"),
			new ItemPrice(150, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(271, "Tesla_Cannon_2", "Tesla_Cannon_2", "Weapon272", "Tesla_Cannon_2", "Tesla_Cannon_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "Coins"),
			new ItemPrice(255, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(272, "Tesla_Cannon_3", "Tesla_Cannon_3", "Weapon273", "Tesla_Cannon_3", "Tesla_Cannon_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(240, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(252, "Devostator", "Devostator", "Weapon253", "Devostator", "Devostator", true, false, new List<ItemPrice>
		{
			new ItemPrice(235, "Coins"),
			new ItemPrice(255, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(273, "Devostator_2", "Devostator_2", "Weapon274", "Devostator_2", "Devostator_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "Coins"),
			new ItemPrice(270, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(254, "Hydra", "Hydra", "Weapon255", "Hydra", "Hydra", true, false, new List<ItemPrice>
		{
			new ItemPrice(215, "Coins"),
			new ItemPrice(255, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(274, "Hydra_2", "Hydra_2", "Weapon275", "Hydra_2", "Hydra_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(155, "Coins"),
			new ItemPrice(295, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(253, "Dark_Matter_Generator", "Dark_Matter_Generator", "Weapon254", "Dark_Matter_Generator", "Dark_Matter_Generator", true, false, new List<ItemPrice>
		{
			new ItemPrice(265, "Coins"),
			new ItemPrice(255, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(275, "Dark_Matter_Generator_2", "Dark_Matter_Generator_2", "Weapon276", "Dark_Matter_Generator_2", "Dark_Matter_Generator_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(179, "Coins"),
			new ItemPrice(370, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(278, "Fire_orb", "Fire_orb", "Weapon278", "Fire_orb", "Fire_orb", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(153, "Coins"),
			new ItemPrice(120, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(279, "Fire_orb_2", "Fire_orb_2", "Weapon279", "Fire_orb_2", "Fire_orb_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(165, "Coins"),
			new ItemPrice(180, "Coins"),
			new ItemPrice(120, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(280, "Fire_orb_3", "Fire_orb_3", "Weapon280", "Fire_orb_3", "Fire_orb_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(135, "GemsCurrency"),
			new ItemPrice(135, "GemsCurrency"),
			new ItemPrice(195, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(281, "Hand_dragon", "Hand_dragon", "Weapon281", "Hand_dragon", "Hand_dragon", true, false, new List<ItemPrice>
		{
			new ItemPrice(185, "Coins"),
			new ItemPrice(120, "Coins"),
			new ItemPrice(100, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(282, "Hand_dragon_2", "Hand_dragon_2", "Weapon282", "Hand_dragon_2", "Hand_dragon_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "Coins"),
			new ItemPrice(205, "Coins"),
			new ItemPrice(100, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(283, "Hand_dragon_3", "Hand_dragon_3", "Weapon283", "Hand_dragon_3", "Hand_dragon_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(284, "Alligator", "Alligator", "Weapon284", "Alligator", "Alligator", true, false, new List<ItemPrice>
		{
			new ItemPrice(150, "GemsCurrency"),
			new ItemPrice(255, "Coins"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(287, "Alligator_2", "Alligator_2", "Weapon287", "Alligator_2", "Alligator_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(80, "GemsCurrency"),
			new ItemPrice(180, "GemsCurrency"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(288, "Alligator_3", "Alligator_3", "Weapon288", "Alligator_3", "Alligator_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(270, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(285, "Hippo", "Hippo", "Weapon285", "Hippo", "Hippo", true, false, new List<ItemPrice>
		{
			new ItemPrice(150, "GemsCurrency"),
			new ItemPrice(255, "GemsCurrency"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(289, "Hippo_2", "Hippo_2", "Weapon289", "Hippo_2", "Hippo_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(80, "GemsCurrency"),
			new ItemPrice(180, "GemsCurrency"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(290, "Hippo_3", "Hippo_3", "Weapon290", "Hippo_3", "Hippo_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(130, "GemsCurrency"),
			new ItemPrice(130, "GemsCurrency"),
			new ItemPrice(270, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(286, "Alien_Cannon", "Alien_Cannon", "Weapon286", "Alien_Cannon", "Alien_Cannon", true, false, new List<ItemPrice>
		{
			new ItemPrice(185, "Coins"),
			new ItemPrice(255, "Coins"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(291, "Alien_Cannon_2", "Alien_Cannon_2", "Weapon291", "Alien_Cannon_2", "Alien_Cannon_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(85, "Coins"),
			new ItemPrice(203, "Coins"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(292, "Alien_Cannon_3", "Alien_Cannon_3", "Weapon292", "Alien_Cannon_3", "Alien_Cannon_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(96, "GemsCurrency"),
			new ItemPrice(96, "GemsCurrency"),
			new ItemPrice(200, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(293, "Alien_Laser_Pistol", "Alien_Laser_Pistol", "Weapon293", "Alien_Laser_Pistol", "Alien_Laser_Pistol", true, false, new List<ItemPrice>
		{
			new ItemPrice(135, "GemsCurrency"),
			new ItemPrice(96, "GemsCurrency"),
			new ItemPrice(200, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(294, "Alien_Laser_Pistol_2", "Alien_Laser_Pistol_2", "Weapon294", "Alien_Laser_Pistol_2", "Alien_Laser_Pistol_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(95, "GemsCurrency"),
			new ItemPrice(240, "GemsCurrency"),
			new ItemPrice(200, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(295, "Alien_Laser_Pistol_3", "Alien_Laser_Pistol_3", "Weapon295", "Alien_Laser_Pistol_3", "Alien_Laser_Pistol_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(130, "GemsCurrency"),
			new ItemPrice(130, "GemsCurrency"),
			new ItemPrice(270, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(296, "Dater_Flowers", "Dater_Flowers", "Weapon296", "Dater_Flowers", "Dater_Flowers", true, false, new List<ItemPrice>
		{
			new ItemPrice(102, "Coins"),
			new ItemPrice(204, "Coins"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(326, "Dater_Flowers_2", "Dater_Flowers_2", "Weapon326", "Dater_Flowers_2", "Dater_Flowers_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(135, "Coins"),
			new ItemPrice(192, "Coins"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(327, "Dater_Flowers_3", "Dater_Flowers_3", "Weapon327", "Dater_Flowers_3", "Dater_Flowers_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(115, "GemsCurrency"),
			new ItemPrice(115, "GemsCurrency"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(297, "Dater_DJ", "Dater_DJ", "Weapon297", "Dater_DJ", "Dater_DJ", true, false, new List<ItemPrice>
		{
			new ItemPrice(115, "GemsCurrency"),
			new ItemPrice(110, "GemsCurrency"),
			new ItemPrice(168, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(324, "Dater_DJ_2", "Dater_DJ_2", "Weapon324", "Dater_DJ_2", "Dater_DJ_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(75, "GemsCurrency"),
			new ItemPrice(109, "GemsCurrency"),
			new ItemPrice(168, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(325, "Dater_DJ_3", "Dater_DJ_3", "Weapon325", "Dater_DJ_3", "Dater_DJ_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(115, "GemsCurrency"),
			new ItemPrice(115, "GemsCurrency"),
			new ItemPrice(240, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(298, "Dater_Arms", null, "Weapon298", null, null, 0, false, false));
		list.Add(new ItemRecord(299, "Dater_Bow", "Dater_Bow", "Weapon299", "Dater_Bow", "Dater_Bow", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "Coins"),
			new ItemPrice(255, "Coins"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(322, "Dater_Bow_2", "Dater_Bow_2", "Weapon322", "Dater_Bow_2", "Dater_Bow_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(119, "Coins"),
			new ItemPrice(204, "Coins"),
			new ItemPrice(199, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(323, "Dater_Bow_3", "Dater_Bow_3", "Weapon323", "Dater_Bow_3", "Dater_Bow_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(99, "GemsCurrency"),
			new ItemPrice(99, "GemsCurrency"),
			new ItemPrice(195, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(302, "FriendsUzi", null, "Weapon302", null, null, 0, false, false));
		list.Add(new ItemRecord(303, "Alien_rifle", "Alien_rifle", "Weapon303", "Alien_rifle", "Alien_rifle", true, false, new List<ItemPrice>
		{
			new ItemPrice(272, "Coins"),
			new ItemPrice(120, "Coins"),
			new ItemPrice(100, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(304, "Alien_rifle_2", "Alien_rifle_2", "Weapon304", "Alien_rifle_2", "Alien_rifle_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(153, "Coins"),
			new ItemPrice(319, "Coins"),
			new ItemPrice(100, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(305, "Alien_rifle_3", "Alien_rifle_3", "Weapon305", "Alien_rifle_3", "Alien_rifle_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(110, "GemsCurrency"),
			new ItemPrice(110, "GemsCurrency"),
			new ItemPrice(270, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(306, "VACUUMIZER", "VACUUMIZER", "Weapon306", "VACUUMIZER", "VACUUMIZER", 275, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(307, "Fireworks_Launcher", "Fireworks_Launcher", "Weapon307", "Fireworks_Launcher", "Fireworks_Launcher", true, false, new List<ItemPrice>
		{
			new ItemPrice(115, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(100, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(328, "Fireworks_Launcher_2", "Fireworks_Launcher_2", "Weapon328", "Fireworks_Launcher_2", "Fireworks_Launcher_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(80, "GemsCurrency"),
			new ItemPrice(180, "GemsCurrency"),
			new ItemPrice(100, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(329, "Fireworks_Launcher_3", "Fireworks_Launcher_3", "Weapon329", "Fireworks_Launcher_3", "Fireworks_Launcher_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(95, "GemsCurrency"),
			new ItemPrice(95, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(310, "Pit_Bull", "Pit_Bull", "Weapon310", "Pit_Bull", "Pit_Bull", true, false, new List<ItemPrice>
		{
			new ItemPrice(221, "Coins"),
			new ItemPrice(110, "GemsCurrency"),
			new ItemPrice(168, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(308, "Shotgun_Pistol", "Shotgun_Pistol", "Weapon308", "Shotgun_Pistol", "Shotgun_Pistol", 235, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(312, "Tiger_gun", "Tiger_gun", "Weapon312", "Tiger_gun", "Tiger_gun", true, false, new List<ItemPrice>
		{
			new ItemPrice(205, "Coins"),
			new ItemPrice(250, "GemsCurrency"),
			new ItemPrice(258, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(313, "Tiger_gun_2", "Tiger_gun_2", "Weapon313", "Tiger_gun_2", "Tiger_gun_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(170, "Coins"),
			new ItemPrice(280, "Coins"),
			new ItemPrice(288, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(314, "Tiger_gun_3", "Tiger_gun_3", "Weapon314", "Tiger_gun_3", "Tiger_gun_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(260, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(309, "Range_Rifle", "Range_Rifle", "Weapon309", "Range_Rifle", "Range_Rifle", true, false, new List<ItemPrice>
		{
			new ItemPrice(204, "Coins"),
			new ItemPrice(260, "GemsCurrency"),
			new ItemPrice(268, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(320, "Range_Rifle_2", "Range_Rifle_2", "Weapon320", "Range_Rifle_2", "Range_Rifle_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(155, "Coins"),
			new ItemPrice(281, "Coins"),
			new ItemPrice(268, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(321, "Range_Rifle_3", "Range_Rifle_3", "Weapon321", "Range_Rifle_3", "Range_Rifle_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(109, "GemsCurrency"),
			new ItemPrice(109, "GemsCurrency"),
			new ItemPrice(255, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(315, "Pit_Bull_2", "Pit_Bull_2", "Weapon315", "Pit_Bull_2", "Pit_Bull_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(136, "Coins"),
			new ItemPrice(268, "Coins"),
			new ItemPrice(268, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(316, "Pit_Bull_3", "Pit_Bull_3", "Weapon316", "Pit_Bull_3", "Pit_Bull_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(105, "GemsCurrency"),
			new ItemPrice(105, "GemsCurrency"),
			new ItemPrice(240, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(317, "Balloon_Cannon", "Balloon_Cannon", "Weapon317", "Balloon_Cannon", "Balloon_Cannon", true, false, new List<ItemPrice>
		{
			new ItemPrice(155, "Coins"),
			new ItemPrice(105, "Coins"),
			new ItemPrice(240, "Coins")
		}, true));
		list.Add(new ItemRecord(318, "Balloon_Cannon_2", "Balloon_Cannon_2", "Weapon318", "Balloon_Cannon_2", "Balloon_Cannon_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(125, "Coins"),
			new ItemPrice(235, "Coins"),
			new ItemPrice(235, "Coins")
		}, true));
		list.Add(new ItemRecord(319, "Balloon_Cannon_3", "Balloon_Cannon_3", "Weapon319", "Balloon_Cannon_3", "Balloon_Cannon_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(225, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(330, "Mech_heavy_rifle", "Mech_heavy_rifle", "Weapon330", "Mech_heavy_rifle", "Mech_heavy_rifle", 240, true, false, "GemsCurrency"));
		list.Add(new ItemRecord(142, "PumpkinGun_1", "PumpkinGun_1", "Weapon143", "PumpkinGun_1", "PumpkinGun_1", 340, true, false, "Coins", 150, true));
		list.Add(new ItemRecord(145, "PumpkinGun_2", "PumpkinGun_2", "Weapon147", "PumpkinGun_2", "PumpkinGun_2", 315, true, false, "Coins", 115, true));
		list.Add(new ItemRecord(332, "PumpkinGun_5", "PumpkinGun_5", "Weapon332", "PumpkinGun_5", "PumpkinGun_5", 159, true, false, "GemsCurrency", -1, true));
		list.Add(new ItemRecord(331, "Shuriken_Thrower", "Shuriken_Thrower", "Weapon331", "Shuriken_Thrower", "Shuriken_Thrower", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "Coins"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(225, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(348, "Shuriken_Thrower2", "Shuriken_Thrower2", "Weapon348", "Shuriken_Thrower2", "Shuriken_Thrower2", true, false, new List<ItemPrice>
		{
			new ItemPrice(90, "Coins"),
			new ItemPrice(160, "Coins"),
			new ItemPrice(225, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(349, "Shuriken_Thrower3", "Shuriken_Thrower3", "Weapon349", "Shuriken_Thrower3", "Shuriken_Thrower3", true, false, new List<ItemPrice>
		{
			new ItemPrice(80, "GemsCurrency"),
			new ItemPrice(80, "GemsCurrency"),
			new ItemPrice(170, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(333, "BASIC_FLAMETHROWER", null, "Weapon333", null, null, 0, false, false));
		list.Add(new ItemRecord(335, "snowball", "snowball", "Weapon335", "snowball", "snowball", true, false, new List<ItemPrice>
		{
			new ItemPrice(110, "Coins"),
			new ItemPrice(160, "Coins"),
			new ItemPrice(225, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(353, "snowball2", "snowball2", "Weapon353", "snowball2", "snowball2", true, false, new List<ItemPrice>
		{
			new ItemPrice(85, "Coins"),
			new ItemPrice(145, "Coins"),
			new ItemPrice(225, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(354, "snowball3", "snowball3", "Weapon354", "snowball3", "snowball3", true, false, new List<ItemPrice>
		{
			new ItemPrice(70, "GemsCurrency"),
			new ItemPrice(80, "GemsCurrency"),
			new ItemPrice(170, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(339, "Laser_Crossbow", "Laser_Crossbow", "Weapon339", "Laser_Crossbow", "Laser_Crossbow", true, false, new List<ItemPrice>
		{
			new ItemPrice(163, "GemsCurrency"),
			new ItemPrice(235, "GemsCurrency"),
			new ItemPrice(235, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(340, "Laser_Crossbow2", "Laser_Crossbow2", "Weapon340", "Laser_Crossbow2", "Laser_Crossbow2", true, false, new List<ItemPrice>
		{
			new ItemPrice(90, "GemsCurrency"),
			new ItemPrice(180, "GemsCurrency"),
			new ItemPrice(235, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(341, "Laser_Crossbow3", "Laser_Crossbow3", "Weapon341", "Laser_Crossbow3", "Laser_Crossbow3", true, false, new List<ItemPrice>
		{
			new ItemPrice(139, "GemsCurrency"),
			new ItemPrice(139, "GemsCurrency"),
			new ItemPrice(300, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(342, "Nutcracker", "Nutcracker", "Weapon342", "Nutcracker", "Nutcracker", true, false, new List<ItemPrice>
		{
			new ItemPrice(250, "Coins"),
			new ItemPrice(235, "Coins"),
			new ItemPrice(235, "Coins")
		}, true));
		list.Add(new ItemRecord(343, "Nutcracker2", "Nutcracker2", "Weapon343", "Nutcracker2", "Nutcracker2", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "Coins"),
			new ItemPrice(310, "Coins"),
			new ItemPrice(235, "Coins")
		}, true));
		list.Add(new ItemRecord(344, "Nutcracker3", "Nutcracker3", "Weapon344", "Nutcracker3", "Nutcracker3", true, false, new List<ItemPrice>
		{
			new ItemPrice(135, "GemsCurrency"),
			new ItemPrice(135, "GemsCurrency"),
			new ItemPrice(300, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(345, "SPACE_RIFLE", "SPACE_RIFLE", "Weapon345", "SPACE_RIFLE", "SPACE_RIFLE", true, false, new List<ItemPrice>
		{
			new ItemPrice(150, "GemsCurrency"),
			new ItemPrice(235, "GemsCurrency"),
			new ItemPrice(235, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(346, "SPACE_RIFLE_UP1", "SPACE_RIFLE_UP1", "Weapon346", "SPACE_RIFLE_UP1", "SPACE_RIFLE_UP1", true, false, new List<ItemPrice>
		{
			new ItemPrice(110, "GemsCurrency"),
			new ItemPrice(200, "GemsCurrency"),
			new ItemPrice(240, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(347, "SPACE_RIFLE_UP2", "SPACE_RIFLE_UP2", "Weapon347", "SPACE_RIFLE_UP2", "SPACE_RIFLE_UP2", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "GemsCurrency"),
			new ItemPrice(140, "GemsCurrency"),
			new ItemPrice(305, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(350, "Icicle_Generator", "Icicle_Generator", "Weapon350", "Icicle_Generator", "Icicle_Generator", true, false, new List<ItemPrice>
		{
			new ItemPrice(220, "Coins"),
			new ItemPrice(240, "Coins"),
			new ItemPrice(245, "Coins")
		}, true));
		list.Add(new ItemRecord(351, "Icicle_Generator2", "Icicle_Generator2", "Weapon351", "Icicle_Generator2", "Icicle_Generator2", true, false, new List<ItemPrice>
		{
			new ItemPrice(190, "Coins"),
			new ItemPrice(305, "Coins"),
			new ItemPrice(245, "Coins")
		}, true));
		list.Add(new ItemRecord(352, "Icicle_Generator3", "Icicle_Generator3", "Weapon352", "Icicle_Generator3", "Icicle_Generator3", true, false, new List<ItemPrice>
		{
			new ItemPrice(130, "GemsCurrency"),
			new ItemPrice(130, "GemsCurrency"),
			new ItemPrice(260, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(355, "PORTABLE_DEATH_MOON", "PORTABLE_DEATH_MOON", "Weapon355", "PORTABLE_DEATH_MOON", "PORTABLE_DEATH_MOON", true, false, new List<ItemPrice>
		{
			new ItemPrice(180, "GemsCurrency"),
			new ItemPrice(210, "Coins"),
			new ItemPrice(245, "Coins")
		}, true));
		list.Add(new ItemRecord(356, "PORTABLE_DEATH_MOON_UP1", "PORTABLE_DEATH_MOON_UP1", "Weapon356", "PORTABLE_DEATH_MOON_UP1", "PORTABLE_DEATH_MOON_UP1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(200, "GemsCurrency"),
			new ItemPrice(245, "Coins")
		}, true));
		list.Add(new ItemRecord(357, "PORTABLE_DEATH_MOON_UP2", "PORTABLE_DEATH_MOON_UP2", "Weapon357", "PORTABLE_DEATH_MOON_UP2", "PORTABLE_DEATH_MOON_UP2", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "GemsCurrency"),
			new ItemPrice(140, "GemsCurrency"),
			new ItemPrice(310, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(336, "MysticOreEmitter", "MysticOreEmitter", "Weapon336", "MysticOreEmitter", "MysticOreEmitter", true, false, new List<ItemPrice>
		{
			new ItemPrice(150, "Coins"),
			new ItemPrice(200, "Coins"),
			new ItemPrice(250, "Coins")
		}, true));
		list.Add(new ItemRecord(337, "MysticOreEmitter_UP1", "MysticOreEmitter_UP1", "Weapon337", "MysticOreEmitter_UP1", "MysticOreEmitter_UP1", true, false, new List<ItemPrice>
		{
			new ItemPrice(95, "Coins"),
			new ItemPrice(185, "Coins"),
			new ItemPrice(300, "Coins")
		}, true));
		list.Add(new ItemRecord(338, "MysticOreEmitter_UP2", "MysticOreEmitter_UP2", "Weapon338", "MysticOreEmitter_UP2", "MysticOreEmitter_UP2", true, false, new List<ItemPrice>
		{
			new ItemPrice(90, "GemsCurrency"),
			new ItemPrice(90, "GemsCurrency"),
			new ItemPrice(170, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(358, "Candy_Baton", null, "Weapon358", null, null, 0, false, false));
		list.Add(new ItemRecord(361, "Hockey_stick", "Hockey_stick", "Weapon361", "Hockey_stick", "Hockey_stick", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "Coins"),
			new ItemPrice(90, "Coins"),
			new ItemPrice(170, "Coins")
		}, true));
		list.Add(new ItemRecord(362, "Hockey_stick_UP1", "Hockey_stick_UP1", "Weapon362", "Hockey_stick_UP1", "Hockey_stick_UP1", true, false, new List<ItemPrice>
		{
			new ItemPrice(160, "Coins"),
			new ItemPrice(230, "Coins"),
			new ItemPrice(170, "Coins")
		}, true));
		list.Add(new ItemRecord(363, "Hockey_stick_UP2", "Hockey_stick_UP2", "Weapon363", "Hockey_stick_UP2", "Hockey_stick_UP2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(240, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(364, "Space_blaster", "Space_blaster", "Weapon364", "Space_blaster", "Space_blaster", true, false, new List<ItemPrice>
		{
			new ItemPrice(200, "Coins"),
			new ItemPrice(116, "GemsCurrency"),
			new ItemPrice(240, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(365, "Space_blaster_UP1", "Space_blaster_UP1", "Weapon365", "Space_blaster_UP1", "Space_blaster_UP1", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "Coins"),
			new ItemPrice(240, "Coins"),
			new ItemPrice(240, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(366, "Space_blaster_UP2", "Space_blaster_UP2", "Weapon366", "Space_blaster_UP2", "Space_blaster_UP2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(367, "mp5_gold_gift", null, "Weapon367", null, null, 0, false, false));
		list.Add(new ItemRecord(368, "Dynamite_Gun_1", "Dynamite_Gun_1", "Weapon368", "Dynamite_Gun_1", "Dynamite_Gun_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(150, "GemsCurrency"),
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(369, "Dynamite_Gun_2", "Dynamite_Gun_2", "Weapon369", "Dynamite_Gun_2", "Dynamite_Gun_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(85, "GemsCurrency"),
			new ItemPrice(185, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(370, "Dynamite_Gun_3", "Dynamite_Gun_3", "Weapon370", "Dynamite_Gun_3", "Dynamite_Gun_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(135, "GemsCurrency"),
			new ItemPrice(135, "GemsCurrency"),
			new ItemPrice(275, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(371, "Dual_shotguns_1", "Dual_shotguns_1", "Weapon371", "Dual_shotguns_1", "Dual_shotguns_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(235, "Coins"),
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(372, "Dual_shotguns_2", "Dual_shotguns_2", "Weapon372", "Dual_shotguns_2", "Dual_shotguns_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(141, "Coins"),
			new ItemPrice(285, "Coins"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(373, "Dual_shotguns_3", "Dual_shotguns_3", "Weapon373", "Dual_shotguns_3", "Dual_shotguns_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(95, "GemsCurrency"),
			new ItemPrice(95, "GemsCurrency"),
			new ItemPrice(245, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(374, "Antihero_Rifle_1", "Antihero_Rifle_1", "Weapon374", "Antihero_Rifle_1", "Antihero_Rifle_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(175, "GemsCurrency"),
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(375, "Antihero_Rifle_2", "Antihero_Rifle_2", "Weapon375", "Antihero_Rifle_2", "Antihero_Rifle_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(95, "GemsCurrency"),
			new ItemPrice(190, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(376, "Antihero_Rifle_3", "Antihero_Rifle_3", "Weapon376", "Antihero_Rifle_3", "Antihero_Rifle_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(145, "GemsCurrency"),
			new ItemPrice(145, "GemsCurrency"),
			new ItemPrice(310, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(377, "HarpoonGun_1", "HarpoonGun_1", "Weapon377", "HarpoonGun_1", "HarpoonGun_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(165, "GemsCurrency"),
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(378, "HarpoonGun_2", "HarpoonGun_2", "Weapon378", "HarpoonGun_2", "HarpoonGun_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(85, "GemsCurrency"),
			new ItemPrice(225, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(379, "HarpoonGun_3", "HarpoonGun_3", "Weapon379", "HarpoonGun_3", "HarpoonGun_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(380, "Red_twins_pistols_1", "Red_twins_pistols_1", "Weapon380", "Red_twins_pistols_1", "Red_twins_pistols_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(175, "Coins"),
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(381, "Red_twins_pistols_2", "Red_twins_pistols_2", "Weapon381", "Red_twins_pistols_2", "Red_twins_pistols_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(135, "Coins"),
			new ItemPrice(245, "Coins"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(382, "Red_twins_pistols_3", "Red_twins_pistols_3", "Weapon382", "Red_twins_pistols_3", "Red_twins_pistols_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(102, "GemsCurrency"),
			new ItemPrice(102, "GemsCurrency"),
			new ItemPrice(235, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(383, "Toxic_sniper_rifle_1", "Toxic_sniper_rifle_1", "Weapon383", "Toxic_sniper_rifle_1", "Toxic_sniper_rifle_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(217, "Coins"),
			new ItemPrice(101, "GemsCurrency"),
			new ItemPrice(235, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(384, "Toxic_sniper_rifle_2", "Toxic_sniper_rifle_2", "Weapon384", "Toxic_sniper_rifle_2", "Toxic_sniper_rifle_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(178, "Coins"),
			new ItemPrice(279, "Coins"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(386, "NuclearRevolver_1", "NuclearRevolver_1", "Weapon386", "NuclearRevolver_1", "NuclearRevolver_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(195, "Coins"),
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(387, "NuclearRevolver_2", "NuclearRevolver_2", "Weapon387", "NuclearRevolver_2", "NuclearRevolver_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(105, "Coins"),
			new ItemPrice(220, "Coins"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(388, "NuclearRevolver_3", "NuclearRevolver_3", "Weapon388", "NuclearRevolver_3", "NuclearRevolver_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(90, "GemsCurrency"),
			new ItemPrice(90, "GemsCurrency"),
			new ItemPrice(210, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(389, "NAIL_MINIGUN_1", "NAIL_MINIGUN_1", "Weapon389", "NAIL_MINIGUN_1", "NAIL_MINIGUN_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(145, "GemsCurrency"),
			new ItemPrice(90, "GemsCurrency"),
			new ItemPrice(210, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(390, "NAIL_MINIGUN_2", "NAIL_MINIGUN_2", "Weapon390", "NAIL_MINIGUN_2", "NAIL_MINIGUN_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(70, "GemsCurrency"),
			new ItemPrice(195, "GemsCurrency"),
			new ItemPrice(210, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(391, "NAIL_MINIGUN_3", "NAIL_MINIGUN_3", "Weapon391", "NAIL_MINIGUN_3", "NAIL_MINIGUN_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(108, "GemsCurrency"),
			new ItemPrice(108, "GemsCurrency"),
			new ItemPrice(275, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(392, "DUAL_MACHETE_1", "DUAL_MACHETE_1", "Weapon392", "DUAL_MACHETE_1", "DUAL_MACHETE_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(189, "Coins"),
			new ItemPrice(189, "Coins"),
			new ItemPrice(189, "Coins")
		}, true));
		list.Add(new ItemRecord(393, "DUAL_MACHETE_2", "DUAL_MACHETE_2", "Weapon393", "DUAL_MACHETE_2", "DUAL_MACHETE_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(142, "Coins"),
			new ItemPrice(245, "Coins"),
			new ItemPrice(245, "Coins")
		}, true));
		list.Add(new ItemRecord(394, "DUAL_MACHETE_3", "DUAL_MACHETE_3", "Weapon394", "DUAL_MACHETE_3", "DUAL_MACHETE_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(105, "GemsCurrency"),
			new ItemPrice(105, "GemsCurrency"),
			new ItemPrice(229, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(395, "Fighter_1", "Fighter_1", "Weapon395", "Fighter_1", "Fighter_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(225, "Coins"),
			new ItemPrice(225, "Coins"),
			new ItemPrice(225, "Coins")
		}, true));
		list.Add(new ItemRecord(396, "Fighter_2", "Fighter_2", "Weapon396", "Fighter_2", "Fighter_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(175, "Coins"),
			new ItemPrice(175, "Coins"),
			new ItemPrice(290, "Coins")
		}, true));
		list.Add(new ItemRecord(397, "Gas_spreader", "Gas_spreader", "Weapon397", "Gas_spreader", "Gas_spreader", true, false, new List<ItemPrice>
		{
			new ItemPrice(265, "Coins"),
			new ItemPrice(265, "Coins"),
			new ItemPrice(265, "Coins")
		}, true));
		list.Add(new ItemRecord(398, "Gas_spreader_up1", "Gas_spreader_up1", "Weapon398", "Gas_spreader_up1", "Gas_spreader_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(195, "Coins"),
			new ItemPrice(295, "Coins"),
			new ItemPrice(295, "Coins")
		}, true));
		list.Add(new ItemRecord(399, "LaserBouncer_1", "LaserBouncer_1", "Weapon399", "LaserBouncer_1", "LaserBouncer_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(345, "Coins"),
			new ItemPrice(345, "Coins"),
			new ItemPrice(345, "Coins")
		}, true));
		list.Add(new ItemRecord(400, "LaserBouncer_2", "LaserBouncer_2", "Weapon400", "LaserBouncer_2", "LaserBouncer_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(235, "Coins"),
			new ItemPrice(235, "Coins"),
			new ItemPrice(345, "Coins")
		}, true));
		list.Add(new ItemRecord(401, "magicbook_fireball", "magicbook_fireball", "Weapon401", "magicbook_fireball", "magicbook_fireball", true, false, new List<ItemPrice>
		{
			new ItemPrice(260, "Coins"),
			new ItemPrice(260, "Coins"),
			new ItemPrice(260, "Coins")
		}, true));
		list.Add(new ItemRecord(402, "magicbook_fireball_2", "magicbook_fireball_2", "Weapon402", "magicbook_fireball_2", "magicbook_fireball_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(145, "Coins"),
			new ItemPrice(320, "Coins"),
			new ItemPrice(320, "Coins")
		}, true));
		list.Add(new ItemRecord(403, "magicbook_fireball_3", "magicbook_fireball_3", "Weapon403", "magicbook_fireball_3", "magicbook_fireball_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "GemsCurrency"),
			new ItemPrice(140, "GemsCurrency"),
			new ItemPrice(310, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(404, "magicbook_frostbeam", "magicbook_frostbeam", "Weapon404", "magicbook_frostbeam", "magicbook_frostbeam", true, false, new List<ItemPrice>
		{
			new ItemPrice(190, "Coins"),
			new ItemPrice(190, "Coins"),
			new ItemPrice(190, "Coins")
		}, true));
		list.Add(new ItemRecord(405, "magicbook_frostbeam_2", "magicbook_frostbeam_2", "Weapon405", "magicbook_frostbeam_2", "magicbook_frostbeam_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(130, "Coins"),
			new ItemPrice(240, "Coins"),
			new ItemPrice(240, "Coins")
		}, true));
		list.Add(new ItemRecord(406, "magicbook_thunder", "magicbook_thunder", "Weapon406", "magicbook_thunder", "magicbook_thunder", true, false, new List<ItemPrice>
		{
			new ItemPrice(220, "Coins"),
			new ItemPrice(220, "Coins"),
			new ItemPrice(220, "Coins")
		}, true));
		list.Add(new ItemRecord(407, "magicbook_thunder_2", "magicbook_thunder_2", "Weapon407", "magicbook_thunder_2", "magicbook_thunder_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(170, "Coins"),
			new ItemPrice(285, "Coins"),
			new ItemPrice(285, "Coins")
		}, true));
		list.Add(new ItemRecord(408, "TurboPistols_1", "TurboPistols_1", "Weapon408", "TurboPistols_1", "TurboPistols_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(409, "TurboPistols_2", "TurboPistols_2", "Weapon409", "TurboPistols_2", "TurboPistols_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(85, "GemsCurrency"),
			new ItemPrice(165, "GemsCurrency"),
			new ItemPrice(165, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(410, "TurboPistols_3", "TurboPistols_3", "Weapon410", "TurboPistols_3", "TurboPistols_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(105, "GemsCurrency"),
			new ItemPrice(105, "GemsCurrency"),
			new ItemPrice(260, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(411, "Laser_Bow_1", "Laser_Bow_1", "Weapon411", "Laser_Bow_1", "Laser_Bow_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(205, "GemsCurrency"),
			new ItemPrice(205, "GemsCurrency"),
			new ItemPrice(205, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(412, "Laser_Bow_2", "Laser_Bow_2", "Weapon412", "Laser_Bow_2", "Laser_Bow_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(145, "GemsCurrency"),
			new ItemPrice(300, "GemsCurrency"),
			new ItemPrice(300, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(413, "loud_piggy", "loud_piggy", "Weapon413", "loud_piggy", "loud_piggy", true, false, new List<ItemPrice>
		{
			new ItemPrice(215, "GemsCurrency"),
			new ItemPrice(215, "GemsCurrency"),
			new ItemPrice(215, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(414, "loud_piggy_up1", "loud_piggy_up1", "Weapon414", "loud_piggy_up1", "loud_piggy_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(165, "GemsCurrency"),
			new ItemPrice(320, "GemsCurrency"),
			new ItemPrice(320, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(415, "Trapper_1", "Trapper_1", "Weapon415", "Trapper_1", "Trapper_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(270, "Coins"),
			new ItemPrice(200, "GemsCurrency"),
			new ItemPrice(200, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(416, "Trapper_2", "Trapper_2", "Weapon416", "Trapper_2", "Trapper_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(190, "Coins"),
			new ItemPrice(320, "Coins"),
			new ItemPrice(290, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(417, "chainsaw_sword_1", "chainsaw_sword_1", "Weapon417", "chainsaw_sword_1", "chainsaw_sword_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(200, "GemsCurrency"),
			new ItemPrice(200, "GemsCurrency"),
			new ItemPrice(200, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(418, "chainsaw_sword_2", "chainsaw_sword_2", "Weapon418", "chainsaw_sword_2", "chainsaw_sword_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "GemsCurrency"),
			new ItemPrice(290, "GemsCurrency"),
			new ItemPrice(290, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(419, "dark_star", "dark_star", "Weapon419", "dark_star", "dark_star", true, false, new List<ItemPrice>
		{
			new ItemPrice(270, "Coins"),
			new ItemPrice(270, "Coins"),
			new ItemPrice(270, "Coins")
		}, true));
		list.Add(new ItemRecord(420, "dark_star_up1", "dark_star_up1", "Weapon420", "dark_star_up1", "dark_star_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(240, "Coins"),
			new ItemPrice(380, "Coins"),
			new ItemPrice(380, "Coins")
		}, true));
		list.Add(new ItemRecord(421, "toy_bomber", "toy_bomber", "Weapon421", "toy_bomber", "toy_bomber", true, false, new List<ItemPrice>
		{
			new ItemPrice(300, "Coins"),
			new ItemPrice(300, "Coins"),
			new ItemPrice(300, "Coins")
		}, true));
		list.Add(new ItemRecord(422, "toy_bomber_up1", "toy_bomber_up1", "Weapon422", "toy_bomber_up1", "toy_bomber_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(170, "Coins"),
			new ItemPrice(330, "Coins"),
			new ItemPrice(330, "Coins")
		}, true));
		list.Add(new ItemRecord(423, "toy_bomber_up2", "toy_bomber_up2", "Weapon423", "toy_bomber_up2", "toy_bomber_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(150, "GemsCurrency"),
			new ItemPrice(290, "GemsCurrency"),
			new ItemPrice(290, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(424, "zombie_head", "zombie_head", "Weapon424", "zombie_head", "zombie_head", true, false, new List<ItemPrice>
		{
			new ItemPrice(185, "Coins"),
			new ItemPrice(200, "Coins"),
			new ItemPrice(290, "Coins")
		}, true));
		list.Add(new ItemRecord(425, "zombie_head_up1", "zombie_head_up1", "Weapon425", "zombie_head_up1", "zombie_head_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(166, "Coins"),
			new ItemPrice(272, "Coins"),
			new ItemPrice(272, "Coins")
		}, true));
		list.Add(new ItemRecord(426, "zombie_head_up2", "zombie_head_up2", "Weapon426", "zombie_head_up2", "zombie_head_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(115, "GemsCurrency"),
			new ItemPrice(115, "GemsCurrency"),
			new ItemPrice(262, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(427, "mr_squido", "mr_squido", "Weapon427", "mr_squido", "mr_squido", true, false, new List<ItemPrice>
		{
			new ItemPrice(165, "GemsCurrency"),
			new ItemPrice(99, "Coins"),
			new ItemPrice(255, "Coins")
		}, true));
		list.Add(new ItemRecord(428, "mr_squido_up1", "mr_squido_up1", "Weapon428", "mr_squido_up1", "mr_squido_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(235, "GemsCurrency"),
			new ItemPrice(235, "Coins")
		}, true));
		list.Add(new ItemRecord(429, "mr_squido_up2", "mr_squido_up2", "Weapon429", "mr_squido_up2", "mr_squido_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "GemsCurrency"),
			new ItemPrice(140, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(433, "spark_shark", null, "Weapon433", null, null, 0, false, false));
		list.Add(new ItemRecord(430, "RocketCrossbow", "RocketCrossbow", "Weapon430", "RocketCrossbow", "RocketCrossbow", true, false, new List<ItemPrice>
		{
			new ItemPrice(205, "Coins"),
			new ItemPrice(210, "Coins"),
			new ItemPrice(210, "Coins")
		}, true));
		list.Add(new ItemRecord(431, "RocketCrossbow_up1", "RocketCrossbow_up1", "Weapon431", "RocketCrossbow_up1", "RocketCrossbow_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(174, "Coins"),
			new ItemPrice(284, "Coins"),
			new ItemPrice(284, "Coins")
		}, true));
		list.Add(new ItemRecord(432, "RocketCrossbow_up2", "RocketCrossbow_up2", "Weapon432", "RocketCrossbow_up2", "RocketCrossbow_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(129, "GemsCurrency"),
			new ItemPrice(129, "GemsCurrency"),
			new ItemPrice(295, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(434, "power_claw", null, "Weapon434", null, null, 0, false, false));
		list.Add(new ItemRecord(435, "zombie_slayer", "zombie_slayer", "Weapon435", "zombie_slayer", "zombie_slayer", true, false, new List<ItemPrice>
		{
			new ItemPrice(150, "Coins"),
			new ItemPrice(185, "Coins"),
			new ItemPrice(185, "Coins")
		}, true));
		list.Add(new ItemRecord(436, "zombie_slayer_up1", "zombie_slayer_up1", "Weapon436", "zombie_slayer_up1", "zombie_slayer_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "Coins"),
			new ItemPrice(219, "Coins"),
			new ItemPrice(219, "Coins")
		}, true));
		list.Add(new ItemRecord(437, "zombie_slayer_up2", "zombie_slayer_up2", "Weapon437", "zombie_slayer_up2", "zombie_slayer_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(240, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(438, "AcidCannon", "AcidCannon", "Weapon438", "AcidCannon", "AcidCannon", true, false, new List<ItemPrice>
		{
			new ItemPrice(185, "Coins"),
			new ItemPrice(185, "Coins"),
			new ItemPrice(185, "Coins")
		}, true));
		list.Add(new ItemRecord(439, "AcidCannon_up1", "AcidCannon_up1", "Weapon439", "AcidCannon_up1", "AcidCannon_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(166, "Coins"),
			new ItemPrice(266, "Coins"),
			new ItemPrice(320, "Coins")
		}, true));
		list.Add(new ItemRecord(440, "AcidCannon_up2", "AcidCannon_up2", "Weapon440", "AcidCannon_up2", "AcidCannon_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(126, "GemsCurrency"),
			new ItemPrice(126, "GemsCurrency"),
			new ItemPrice(276, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(441, "frank_sheepone", "frank_sheepone", "Weapon441", "frank_sheepone", "frank_sheepone", true, false, new List<ItemPrice>
		{
			new ItemPrice(210, "Coins"),
			new ItemPrice(210, "Coins"),
			new ItemPrice(210, "Coins")
		}, true));
		list.Add(new ItemRecord(442, "frank_sheepone_up1", "frank_sheepone_up1", "Weapon442", "frank_sheepone_up1", "frank_sheepone_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(200, "Coins"),
			new ItemPrice(310, "Coins"),
			new ItemPrice(310, "Coins")
		}, true));
		list.Add(new ItemRecord(443, "frank_sheepone_up2", "frank_sheepone_up2", "Weapon443", "frank_sheepone_up2", "frank_sheepone_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(140, "GemsCurrency"),
			new ItemPrice(140, "GemsCurrency"),
			new ItemPrice(295, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(444, "Ghost_Lantern", "Ghost_Lantern", "Weapon444", "Ghost_Lantern", "Ghost_Lantern", true, false, new List<ItemPrice>
		{
			new ItemPrice(195, "GemsCurrency"),
			new ItemPrice(195, "GemsCurrency"),
			new ItemPrice(195, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(446, "Semiauto_sniper", "Semiauto_sniper", "Weapon446", "Semiauto_sniper", "Semiauto_sniper", true, false, new List<ItemPrice>
		{
			new ItemPrice(215, "GemsCurrency"),
			new ItemPrice(215, "GemsCurrency"),
			new ItemPrice(215, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(448, "Semiauto_sniper_up1", "Semiauto_sniper_up1", "Weapon448", "Semiauto_sniper_up1", "Semiauto_sniper_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(155, "GemsCurrency"),
			new ItemPrice(315, "GemsCurrency"),
			new ItemPrice(315, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(445, "Ghost_Lantern_up1", "Ghost_Lantern_up1", "Weapon445", "Ghost_Lantern_up1", "Ghost_Lantern_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(135, "GemsCurrency"),
			new ItemPrice(255, "GemsCurrency"),
			new ItemPrice(255, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(450, "Barier_Generator", "Barier_Generator", "Weapon450", "Barier_Generator", "Barier_Generator", true, false, new List<ItemPrice>
		{
			new ItemPrice(250, "Coins"),
			new ItemPrice(250, "Coins"),
			new ItemPrice(250, "Coins")
		}, true));
		list.Add(new ItemRecord(454, "Barier_Generator_up1", "Barier_Generator_up1", "Weapon454", "Barier_Generator_up1", "Barier_Generator_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(195, "Coins"),
			new ItemPrice(330, "Coins"),
			new ItemPrice(330, "Coins")
		}, true));
		list.Add(new ItemRecord(455, "Barier_Generator_up2", "Barier_Generator_up2", "Weapon455", "Barier_Generator_up2", "Barier_Generator_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(135, "GemsCurrency"),
			new ItemPrice(135, "GemsCurrency"),
			new ItemPrice(290, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(447, "Chain_electro_cannon", "Chain_electro_cannon", "Weapon447", "Chain_electro_cannon", "Chain_electro_cannon", true, false, new List<ItemPrice>
		{
			new ItemPrice(270, "Coins"),
			new ItemPrice(270, "Coins"),
			new ItemPrice(270, "Coins")
		}, true));
		list.Add(new ItemRecord(449, "Chain_electro_cannon_up1", "Chain_electro_cannon_up1", "Weapon449", "Chain_electro_cannon_up1", "Chain_electro_cannon_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(205, "Coins"),
			new ItemPrice(350, "Coins"),
			new ItemPrice(350, "Coins")
		}, true));
		list.Add(new ItemRecord(451, "autoaim_bazooka", "autoaim_bazooka", "Weapon451", "autoaim_bazooka", "autoaim_bazooka", true, false, new List<ItemPrice>
		{
			new ItemPrice(340, "Coins"),
			new ItemPrice(340, "Coins"),
			new ItemPrice(340, "Coins")
		}, true));
		list.Add(new ItemRecord(452, "autoaim_bazooka_up1", "autoaim_bazooka_up1", "Weapon452", "autoaim_bazooka_up1", "autoaim_bazooka_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(285, "Coins"),
			new ItemPrice(390, "Coins"),
			new ItemPrice(390, "Coins")
		}, true));
		list.Add(new ItemRecord(456, "Demoman", "Demoman", "Weapon456", "Demoman", "Demoman", true, false, new List<ItemPrice>
		{
			new ItemPrice(245, "Coins"),
			new ItemPrice(245, "Coins"),
			new ItemPrice(245, "Coins")
		}, true));
		list.Add(new ItemRecord(457, "Demoman_up1", "Demoman_up1", "Weapon457", "Demoman_up1", "Demoman_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(180, "Coins"),
			new ItemPrice(320, "Coins"),
			new ItemPrice(320, "Coins")
		}, true));
		list.Add(new ItemRecord(458, "charge_rifle", "charge_rifle", "Weapon458", "charge_rifle", "charge_rifle", true, false, new List<ItemPrice>
		{
			new ItemPrice(275, "Coins"),
			new ItemPrice(275, "Coins"),
			new ItemPrice(275, "Coins")
		}, true));
		list.Add(new ItemRecord(459, "charge_rifle_UP1", "charge_rifle_UP1", "Weapon459", "charge_rifle_UP1", "charge_rifle_UP1", true, false, new List<ItemPrice>
		{
			new ItemPrice(210, "Coins"),
			new ItemPrice(365, "Coins"),
			new ItemPrice(365, "Coins")
		}, true));
		list.Add(new ItemRecord(460, "Bee_Swarm_Spell", "Bee_Swarm_Spell", "Weapon460", "Bee_Swarm_Spell", "Bee_Swarm_Spell", true, false, new List<ItemPrice>
		{
			new ItemPrice(210, "Coins"),
			new ItemPrice(365, "Coins"),
			new ItemPrice(365, "Coins")
		}, true));
		list.Add(new ItemRecord(461, "Bee_Swarm_Spell_up1", "Bee_Swarm_Spell_up1", "Weapon461", "Bee_Swarm_Spell_up1", "Bee_Swarm_Spell_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(210, "Coins"),
			new ItemPrice(365, "Coins"),
			new ItemPrice(365, "Coins")
		}, true));
		list.Add(new ItemRecord(462, "Bee_Swarm_Spell_up2", "Bee_Swarm_Spell_up2", "Weapon462", "Bee_Swarm_Spell_up2", "Bee_Swarm_Spell_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(210, "Coins"),
			new ItemPrice(365, "Coins"),
			new ItemPrice(365, "Coins")
		}, true));
		list.Add(new ItemRecord(463, "minigun_pistol", "minigun_pistol", "Weapon463", "minigun_pistol", "minigun_pistol", true, false, new List<ItemPrice>
		{
			new ItemPrice(210, "Coins"),
			new ItemPrice(365, "Coins"),
			new ItemPrice(365, "Coins")
		}, true));
		list.Add(new ItemRecord(464, "minigun_pistol_up1", "minigun_pistol_up1", "Weapon464", "minigun_pistol_up1", "minigun_pistol_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(210, "Coins"),
			new ItemPrice(365, "Coins"),
			new ItemPrice(365, "Coins")
		}, true));
		list.Add(new ItemRecord(465, "minigun_pistol_up2", "minigun_pistol_up2", "Weapon465", "minigun_pistol_up2", "minigun_pistol_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(210, "Coins"),
			new ItemPrice(365, "Coins"),
			new ItemPrice(365, "Coins")
		}, true));
		list.Add(new ItemRecord(467, "bad_doctor_1", "bad_doctor_1", "Weapon467", "bad_doctor_1", "bad_doctor_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(165, "GemsCurrency"),
			new ItemPrice(100, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(468, "bad_doctor_2", "bad_doctor_2", "Weapon468", "bad_doctor_2", "bad_doctor_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(85, "GemsCurrency"),
			new ItemPrice(225, "GemsCurrency"),
			new ItemPrice(230, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(469, "bad_doctor_3", "bad_doctor_3", "Weapon469", "bad_doctor_3", "bad_doctor_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(470, "dual_laser_blasters", "dual_laser_blasters", "Weapon470", "dual_laser_blasters", "dual_laser_blasters", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(471, "dual_laser_blasters_up1", "dual_laser_blasters_up1", "Weapon471", "dual_laser_blasters_up1", "dual_laser_blasters_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(472, "toxic_bane", "toxic_bane", "Weapon472", "toxic_bane", "toxic_bane", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(473, "toxic_bane_up1", "toxic_bane_up1", "Weapon473", "toxic_bane_up1", "toxic_bane_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(474, "toxic_bane_up2", "toxic_bane_up2", "Weapon474", "toxic_bane_up2", "toxic_bane_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(475, "Charge_Cannon", "Charge_Cannon", "Weapon475", "Charge_Cannon", "Charge_Cannon", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(476, "Charge_Cannon_up1", "Charge_Cannon_up1", "Weapon476", "Charge_Cannon_up1", "Charge_Cannon_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(477, "Heavy_Shocker", "Heavy_Shocker", "Weapon477", "Heavy_Shocker", "Heavy_Shocker", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(478, "Heavy_Shocker_up1", "Heavy_Shocker_up1", "Weapon478", "Heavy_Shocker_up1", "Heavy_Shocker_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(479, "Heavy_Shocker_up2", "Heavy_Shocker_up2", "Weapon479", "Heavy_Shocker_up2", "Heavy_Shocker_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(120, "GemsCurrency"),
			new ItemPrice(285, "GemsCurrency")
		}, true));
		list.Add(new ItemRecord(480, "ruler_sword_1", "ruler_sword_1", "Weapon480", "ruler_sword_1", "ruler_sword_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(481, "ruler_sword_2", "ruler_sword_2", "Weapon481", "ruler_sword_2", "ruler_sword_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(482, "ruler_sword_3", "ruler_sword_3", "Weapon482", "ruler_sword_3", "ruler_sword_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(483, "pencil_thrower_1", "pencil_thrower_1", "Weapon483", "pencil_thrower_1", "pencil_thrower_1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(484, "pencil_thrower_2", "pencil_thrower_2", "Weapon484", "pencil_thrower_2", "pencil_thrower_2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(485, "pencil_thrower_3", "pencil_thrower_3", "Weapon485", "pencil_thrower_3", "pencil_thrower_3", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(489, "sword_of_shadows", "sword_of_shadows", "Weapon489", "sword_of_shadows", "sword_of_shadows", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(490, "sword_of_shadows_up1", "sword_of_shadows_up1", "Weapon490", "sword_of_shadows_up1", "sword_of_shadows_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(491, "sword_of_shadows_up2", "sword_of_shadows_up2", "Weapon491", "sword_of_shadows_up2", "sword_of_shadows_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(486, "napalm_cannon", "napalm_cannon", "Weapon486", "napalm_cannon", "napalm_cannon", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(487, "napalm_cannon_up1", "napalm_cannon_up1", "Weapon487", "napalm_cannon_up1", "napalm_cannon_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(488, "napalm_cannon_up2", "napalm_cannon_up2", "Weapon488", "napalm_cannon_up2", "napalm_cannon_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(492, "dracula", "dracula", "Weapon492", "dracula", "dracula", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(493, "dracula_up1", "dracula_up1", "Weapon493", "dracula_up1", "dracula_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(494, "dracula_up2", "dracula_up2", "Weapon494", "dracula_up2", "dracula_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(495, "xmas_destroyer", "xmas_destroyer", "Weapon495", "xmas_destroyer", "xmas_destroyer", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(496, "xmas_destroyer_up1", "xmas_destroyer_up1", "Weapon496", "xmas_destroyer_up1", "xmas_destroyer_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(497, "xmas_destroyer_up2", "xmas_destroyer_up2", "Weapon497", "xmas_destroyer_up2", "xmas_destroyer_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(498, "santa_sword", "santa_sword", "Weapon498", "santa_sword", "santa_sword", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(499, "santa_sword_up1", "santa_sword_up1", "Weapon499", "santa_sword_up1", "santa_sword_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(500, "santa_sword_up2", "santa_sword_up2", "Weapon500", "santa_sword_up2", "santa_sword_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(501, "snow_storm", "snow_storm", "Weapon501", "snow_storm", "snow_storm", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(502, "snow_storm_up1", "snow_storm_up1", "Weapon502", "snow_storm_up1", "snow_storm_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(503, "snow_storm_up2", "snow_storm_up2", "Weapon503", "snow_storm_up2", "snow_storm_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(504, "heavy_gifter", "heavy_gifter", "Weapon504", "heavy_gifter", "heavy_gifter", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(505, "heavy_gifter_up1", "heavy_gifter_up1", "Weapon505", "heavy_gifter_up1", "heavy_gifter_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(506, "heavy_gifter_up2", "heavy_gifter_up2", "Weapon506", "heavy_gifter_up2", "heavy_gifter_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(507, "bell_revolver", "bell_revolver", "Weapon507", "bell_revolver", "bell_revolver", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(508, "bell_revolver_up1", "bell_revolver_up1", "Weapon508", "bell_revolver_up1", "bell_revolver_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(509, "elfs_revenge", "elfs_revenge", "Weapon509", "elfs_revenge", "elfs_revenge", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(510, "elfs_revenge_up1", "elfs_revenge_up1", "Weapon510", "elfs_revenge_up1", "elfs_revenge_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(511, "elfs_revenge_up2", "elfs_revenge_up2", "Weapon511", "elfs_revenge_up2", "elfs_revenge_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(515, "subzero", "subzero", "Weapon515", "subzero", "subzero", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(516, "subzero_up1", "subzero_up1", "Weapon516", "subzero_up1", "subzero_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(517, "subzero_up2", "subzero_up2", "Weapon517", "subzero_up2", "subzero_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(512, "photon_sniper_rifle", "photon_sniper_rifle", "Weapon512", "photon_sniper_rifle", "photon_sniper_rifle", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(513, "photon_sniper_rifle_up1", "photon_sniper_rifle_up1", "Weapon513", "photon_sniper_rifle_up1", "photon_sniper_rifle_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(514, "photon_sniper_rifle_up2", "photon_sniper_rifle_up2", "Weapon514", "photon_sniper_rifle_up2", "photon_sniper_rifle_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(518, "mercenary", "mercenary", "Weapon518", "mercenary", "mercenary", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(519, "mercenary_up1", "mercenary_up1", "Weapon519", "mercenary_up1", "mercenary_up1", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(520, "mercenary_up2", "mercenary_up2", "Weapon520", "mercenary_up2", "mercenary_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins"),
			new ItemPrice(100, "Coins")
		}, true));
		list.Add(new ItemRecord(522, "autoaim_bazooka_up2", "autoaim_bazooka_up2", "Weapon522", "autoaim_bazooka_up2", "autoaim_bazooka_up2", true, false, new List<ItemPrice>
		{
			new ItemPrice(285, "Coins"),
			new ItemPrice(390, "Coins"),
			new ItemPrice(390, "Coins")
		}, true));
		return list;
	}
}

using System.Collections.Generic;
using UnityEngine;

public class PetsInfo
{
	private static Dictionary<string, PetInfo> _info;

	public static Dictionary<string, PetInfo> info
	{
		get
		{
			return _info;
		}
	}

	static PetsInfo()
	{
		_info = new Dictionary<string, PetInfo>();
		_info = new Dictionary<string, PetInfo>
		{
			{
				"pet_arnold_3000_up0",
				new PetInfo
				{
					Id = "pet_arnold_3000_up0",
					Up = 0,
					IdWithoutUp = "pet_arnold_3000",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 0,
					Lkey = "Key_2727",
					ToUpPoints = 2,
					AttackDistance = 6f,
					AttackStopDistance = 8f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Laser }
				}
			},
			{
				"pet_arnold_3000_up1",
				new PetInfo
				{
					Id = "pet_arnold_3000_up1",
					Up = 1,
					IdWithoutUp = "pet_arnold_3000",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 0,
					Lkey = "Key_2727",
					ToUpPoints = 4,
					AttackDistance = 6f,
					AttackStopDistance = 8f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Laser }
				}
			},
			{
				"pet_arnold_3000_up2",
				new PetInfo
				{
					Id = "pet_arnold_3000_up2",
					Up = 2,
					IdWithoutUp = "pet_arnold_3000",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 1,
					Lkey = "Key_2727",
					ToUpPoints = 7,
					AttackDistance = 6f,
					AttackStopDistance = 8f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Laser }
				}
			},
			{
				"pet_arnold_3000_up3",
				new PetInfo
				{
					Id = "pet_arnold_3000_up3",
					Up = 3,
					IdWithoutUp = "pet_arnold_3000",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 2,
					Lkey = "Key_2727",
					ToUpPoints = 12,
					AttackDistance = 6f,
					AttackStopDistance = 8f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Laser }
				}
			},
			{
				"pet_arnold_3000_up4",
				new PetInfo
				{
					Id = "pet_arnold_3000_up4",
					Up = 4,
					IdWithoutUp = "pet_arnold_3000",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 3,
					Lkey = "Key_2727",
					ToUpPoints = 17,
					AttackDistance = 6f,
					AttackStopDistance = 8f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Laser }
				}
			},
			{
				"pet_arnold_3000_up5",
				new PetInfo
				{
					Id = "pet_arnold_3000_up5",
					Up = 5,
					IdWithoutUp = "pet_arnold_3000",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 4,
					Lkey = "Key_2727",
					ToUpPoints = 22,
					AttackDistance = 6f,
					AttackStopDistance = 8f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Laser }
				}
			},
			{
				"pet_bat_up0",
				new PetInfo
				{
					Id = "pet_bat_up0",
					Up = 0,
					IdWithoutUp = "pet_bat",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 0,
					Lkey = "Key_2505",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, -0.034f, 4.711f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_bat_up1",
				new PetInfo
				{
					Id = "pet_bat_up1",
					Up = 1,
					IdWithoutUp = "pet_bat",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 0,
					Lkey = "Key_2505",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, -0.034f, 4.711f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_bat_up2",
				new PetInfo
				{
					Id = "pet_bat_up2",
					Up = 2,
					IdWithoutUp = "pet_bat",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 1,
					Lkey = "Key_2505",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, -0.034f, 4.711f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_bat_up3",
				new PetInfo
				{
					Id = "pet_bat_up3",
					Up = 3,
					IdWithoutUp = "pet_bat",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 2,
					Lkey = "Key_2505",
					ToUpPoints = 16,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, -0.034f, 4.711f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_bat_up4",
				new PetInfo
				{
					Id = "pet_bat_up4",
					Up = 4,
					IdWithoutUp = "pet_bat",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 3,
					Lkey = "Key_2505",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, -0.034f, 4.711f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_bat_up5",
				new PetInfo
				{
					Id = "pet_bat_up5",
					Up = 5,
					IdWithoutUp = "pet_bat",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 4,
					Lkey = "Key_2505",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, -0.034f, 4.711f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_bear_up0",
				new PetInfo
				{
					Id = "pet_bear_up0",
					Up = 0,
					IdWithoutUp = "pet_bear",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 0,
					Lkey = "Key_2699",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_bear_up1",
				new PetInfo
				{
					Id = "pet_bear_up1",
					Up = 1,
					IdWithoutUp = "pet_bear",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 0,
					Lkey = "Key_2699",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_bear_up2",
				new PetInfo
				{
					Id = "pet_bear_up2",
					Up = 2,
					IdWithoutUp = "pet_bear",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 1,
					Lkey = "Key_2699",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 6,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_bear_up3",
				new PetInfo
				{
					Id = "pet_bear_up3",
					Up = 3,
					IdWithoutUp = "pet_bear",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 2,
					Lkey = "Key_2699",
					ToUpPoints = 12,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 6,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_bear_up4",
				new PetInfo
				{
					Id = "pet_bear_up4",
					Up = 4,
					IdWithoutUp = "pet_bear",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 3,
					Lkey = "Key_2699",
					ToUpPoints = 20,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_bear_up5",
				new PetInfo
				{
					Id = "pet_bear_up5",
					Up = 5,
					IdWithoutUp = "pet_bear",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 4,
					Lkey = "Key_2699",
					ToUpPoints = 64,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_cat_up0",
				new PetInfo
				{
					Id = "pet_cat_up0",
					Up = 0,
					IdWithoutUp = "pet_cat",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 0,
					Lkey = "Key_2506",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_cat_up1",
				new PetInfo
				{
					Id = "pet_cat_up1",
					Up = 1,
					IdWithoutUp = "pet_cat",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 0,
					Lkey = "Key_2506",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_cat_up2",
				new PetInfo
				{
					Id = "pet_cat_up2",
					Up = 2,
					IdWithoutUp = "pet_cat",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 1,
					Lkey = "Key_2506",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_cat_up3",
				new PetInfo
				{
					Id = "pet_cat_up3",
					Up = 3,
					IdWithoutUp = "pet_cat",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 2,
					Lkey = "Key_2506",
					ToUpPoints = 12,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_cat_up4",
				new PetInfo
				{
					Id = "pet_cat_up4",
					Up = 4,
					IdWithoutUp = "pet_cat",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 3,
					Lkey = "Key_2506",
					ToUpPoints = 20,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_cat_up5",
				new PetInfo
				{
					Id = "pet_cat_up5",
					Up = 5,
					IdWithoutUp = "pet_cat",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 4,
					Lkey = "Key_2506",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_deer_up0",
				new PetInfo
				{
					Id = "pet_deer_up0",
					Up = 0,
					IdWithoutUp = "pet_deer",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 0,
					Lkey = "Key_2703",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_deer_up1",
				new PetInfo
				{
					Id = "pet_deer_up1",
					Up = 1,
					IdWithoutUp = "pet_deer",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 0,
					Lkey = "Key_2703",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_deer_up2",
				new PetInfo
				{
					Id = "pet_deer_up2",
					Up = 2,
					IdWithoutUp = "pet_deer",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 1,
					Lkey = "Key_2703",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_deer_up3",
				new PetInfo
				{
					Id = "pet_deer_up3",
					Up = 3,
					IdWithoutUp = "pet_deer",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 2,
					Lkey = "Key_2703",
					ToUpPoints = 12,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_deer_up4",
				new PetInfo
				{
					Id = "pet_deer_up4",
					Up = 4,
					IdWithoutUp = "pet_deer",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 3,
					Lkey = "Key_2703",
					ToUpPoints = 20,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_deer_up5",
				new PetInfo
				{
					Id = "pet_deer_up5",
					Up = 5,
					IdWithoutUp = "pet_deer",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 4,
					Lkey = "Key_2703",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_dinosaur_up0",
				new PetInfo
				{
					Id = "pet_dinosaur_up0",
					Up = 0,
					IdWithoutUp = "pet_dinosaur",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 0,
					Lkey = "Key_2507",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 3f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 1.5f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.019f, -0.436f, 3.544f),
					RotationInBanners = new Vector3(0f, 145.758f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_dinosaur_up1",
				new PetInfo
				{
					Id = "pet_dinosaur_up1",
					Up = 1,
					IdWithoutUp = "pet_dinosaur",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 0,
					Lkey = "Key_2507",
					ToUpPoints = 3,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 3f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 1.5f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.019f, -0.436f, 3.544f),
					RotationInBanners = new Vector3(0f, 145.758f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_dinosaur_up2",
				new PetInfo
				{
					Id = "pet_dinosaur_up2",
					Up = 2,
					IdWithoutUp = "pet_dinosaur",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 1,
					Lkey = "Key_2507",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 3f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 1.5f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.019f, -0.436f, 3.544f),
					RotationInBanners = new Vector3(0f, 145.758f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_dinosaur_up3",
				new PetInfo
				{
					Id = "pet_dinosaur_up3",
					Up = 3,
					IdWithoutUp = "pet_dinosaur",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 2,
					Lkey = "Key_2507",
					ToUpPoints = 5,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 3f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 1.5f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.019f, -0.436f, 3.544f),
					RotationInBanners = new Vector3(0f, 145.758f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_dinosaur_up4",
				new PetInfo
				{
					Id = "pet_dinosaur_up4",
					Up = 4,
					IdWithoutUp = "pet_dinosaur",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 3,
					Lkey = "Key_2507",
					ToUpPoints = 6,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 3f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 1.5f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.019f, -0.436f, 3.544f),
					RotationInBanners = new Vector3(0f, 145.758f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_dinosaur_up5",
				new PetInfo
				{
					Id = "pet_dinosaur_up5",
					Up = 5,
					IdWithoutUp = "pet_dinosaur",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 4,
					Lkey = "Key_2507",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 3f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 1.5f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.019f, -0.436f, 3.544f),
					RotationInBanners = new Vector3(0f, 145.758f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_dog_up0",
				new PetInfo
				{
					Id = "pet_dog_up0",
					Up = 0,
					IdWithoutUp = "pet_dog",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 0,
					Lkey = "Key_2504",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_dog_up1",
				new PetInfo
				{
					Id = "pet_dog_up1",
					Up = 1,
					IdWithoutUp = "pet_dog",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 0,
					Lkey = "Key_2504",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_dog_up2",
				new PetInfo
				{
					Id = "pet_dog_up2",
					Up = 2,
					IdWithoutUp = "pet_dog",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 1,
					Lkey = "Key_2504",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_dog_up3",
				new PetInfo
				{
					Id = "pet_dog_up3",
					Up = 3,
					IdWithoutUp = "pet_dog",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 2,
					Lkey = "Key_2504",
					ToUpPoints = 16,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_dog_up4",
				new PetInfo
				{
					Id = "pet_dog_up4",
					Up = 4,
					IdWithoutUp = "pet_dog",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 3,
					Lkey = "Key_2504",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_dog_up5",
				new PetInfo
				{
					Id = "pet_dog_up5",
					Up = 5,
					IdWithoutUp = "pet_dog",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 4,
					Lkey = "Key_2504",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_eagle_up0",
				new PetInfo
				{
					Id = "pet_eagle_up0",
					Up = 0,
					IdWithoutUp = "pet_eagle",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 0,
					Lkey = "Key_2613",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0.046f, 0.088f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_eagle_up1",
				new PetInfo
				{
					Id = "pet_eagle_up1",
					Up = 1,
					IdWithoutUp = "pet_eagle",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 0,
					Lkey = "Key_2613",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0.046f, 0.088f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_eagle_up2",
				new PetInfo
				{
					Id = "pet_eagle_up2",
					Up = 2,
					IdWithoutUp = "pet_eagle",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 1,
					Lkey = "Key_2613",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.4f,
					PositionInBanners = new Vector3(0.046f, 0.088f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_eagle_up3",
				new PetInfo
				{
					Id = "pet_eagle_up3",
					Up = 3,
					IdWithoutUp = "pet_eagle",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 2,
					Lkey = "Key_2613",
					ToUpPoints = 12,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.4f,
					PositionInBanners = new Vector3(0.046f, 0.088f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_eagle_up4",
				new PetInfo
				{
					Id = "pet_eagle_up4",
					Up = 4,
					IdWithoutUp = "pet_eagle",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 3,
					Lkey = "Key_2613",
					ToUpPoints = 20,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.5f,
					PositionInBanners = new Vector3(0.046f, 0.088f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_eagle_up5",
				new PetInfo
				{
					Id = "pet_eagle_up5",
					Up = 5,
					IdWithoutUp = "pet_eagle",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Epic,
					Tier = 4,
					Lkey = "Key_2613",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.5f,
					PositionInBanners = new Vector3(0.046f, 0.088f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_fox_up0",
				new PetInfo
				{
					Id = "pet_fox_up0",
					Up = 0,
					IdWithoutUp = "pet_fox",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 0,
					Lkey = "Key_2702",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_fox_up1",
				new PetInfo
				{
					Id = "pet_fox_up1",
					Up = 1,
					IdWithoutUp = "pet_fox",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 0,
					Lkey = "Key_2702",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_fox_up2",
				new PetInfo
				{
					Id = "pet_fox_up2",
					Up = 2,
					IdWithoutUp = "pet_fox",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 1,
					Lkey = "Key_2702",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_fox_up3",
				new PetInfo
				{
					Id = "pet_fox_up3",
					Up = 3,
					IdWithoutUp = "pet_fox",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 2,
					Lkey = "Key_2702",
					ToUpPoints = 16,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_fox_up4",
				new PetInfo
				{
					Id = "pet_fox_up4",
					Up = 4,
					IdWithoutUp = "pet_fox",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 3,
					Lkey = "Key_2702",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_fox_up5",
				new PetInfo
				{
					Id = "pet_fox_up5",
					Up = 5,
					IdWithoutUp = "pet_fox",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 4,
					Lkey = "Key_2702",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_lion_up0",
				new PetInfo
				{
					Id = "pet_lion_up0",
					Up = 0,
					IdWithoutUp = "pet_lion",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 0,
					Lkey = "Key_2701",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_lion_up1",
				new PetInfo
				{
					Id = "pet_lion_up1",
					Up = 1,
					IdWithoutUp = "pet_lion",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 0,
					Lkey = "Key_2701",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_lion_up2",
				new PetInfo
				{
					Id = "pet_lion_up2",
					Up = 2,
					IdWithoutUp = "pet_lion",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 1,
					Lkey = "Key_2701",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_lion_up3",
				new PetInfo
				{
					Id = "pet_lion_up3",
					Up = 3,
					IdWithoutUp = "pet_lion",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 2,
					Lkey = "Key_2701",
					ToUpPoints = 12,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_lion_up4",
				new PetInfo
				{
					Id = "pet_lion_up4",
					Up = 4,
					IdWithoutUp = "pet_lion",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 3,
					Lkey = "Key_2701",
					ToUpPoints = 20,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_lion_up5",
				new PetInfo
				{
					Id = "pet_lion_up5",
					Up = 5,
					IdWithoutUp = "pet_lion",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 4,
					Lkey = "Key_2701",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Bleeding,
					poisonCount = 4,
					poisonTime = 3f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Bleeding }
				}
			},
			{
				"pet_magical_dragon_up0",
				new PetInfo
				{
					Id = "pet_magical_dragon_up0",
					Up = 0,
					IdWithoutUp = "pet_magical_dragon",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 0,
					Lkey = "Key_2877",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, 0f, 3.5f),
					RotationInBanners = new Vector3(0f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_magical_dragon_up1",
				new PetInfo
				{
					Id = "pet_magical_dragon_up1",
					Up = 1,
					IdWithoutUp = "pet_magical_dragon",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 0,
					Lkey = "Key_2877",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, 0f, 3.5f),
					RotationInBanners = new Vector3(0f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_magical_dragon_up2",
				new PetInfo
				{
					Id = "pet_magical_dragon_up2",
					Up = 2,
					IdWithoutUp = "pet_magical_dragon",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 0,
					Lkey = "Key_2877",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, 0f, 3.5f),
					RotationInBanners = new Vector3(0f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_magical_dragon_up3",
				new PetInfo
				{
					Id = "pet_magical_dragon_up3",
					Up = 3,
					IdWithoutUp = "pet_magical_dragon",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 0,
					Lkey = "Key_2877",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, 0f, 3.5f),
					RotationInBanners = new Vector3(0f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_magical_dragon_up4",
				new PetInfo
				{
					Id = "pet_magical_dragon_up4",
					Up = 4,
					IdWithoutUp = "pet_magical_dragon",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 0,
					Lkey = "Key_2877",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, 0f, 3.5f),
					RotationInBanners = new Vector3(0f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_magical_dragon_up5",
				new PetInfo
				{
					Id = "pet_magical_dragon_up5",
					Up = 5,
					IdWithoutUp = "pet_magical_dragon",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 0,
					Lkey = "Key_2877",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, 0f, 3.5f),
					RotationInBanners = new Vector3(0f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_owl_up0",
				new PetInfo
				{
					Id = "pet_owl_up0",
					Up = 0,
					IdWithoutUp = "pet_owl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 0,
					Lkey = "Key_2708",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0.1f, 3.72f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_owl_up1",
				new PetInfo
				{
					Id = "pet_owl_up1",
					Up = 1,
					IdWithoutUp = "pet_owl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 0,
					Lkey = "Key_2708",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0.1f, 3.72f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_owl_up2",
				new PetInfo
				{
					Id = "pet_owl_up2",
					Up = 2,
					IdWithoutUp = "pet_owl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 1,
					Lkey = "Key_2708",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0.1f, 3.72f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_owl_up3",
				new PetInfo
				{
					Id = "pet_owl_up3",
					Up = 3,
					IdWithoutUp = "pet_owl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 2,
					Lkey = "Key_2708",
					ToUpPoints = 12,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0.1f, 3.72f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_owl_up4",
				new PetInfo
				{
					Id = "pet_owl_up4",
					Up = 4,
					IdWithoutUp = "pet_owl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 3,
					Lkey = "Key_2708",
					ToUpPoints = 20,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0.1f, 3.72f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_owl_up5",
				new PetInfo
				{
					Id = "pet_owl_up5",
					Up = 5,
					IdWithoutUp = "pet_owl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 4,
					Lkey = "Key_2708",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0.1f, 3.72f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_panda_up0",
				new PetInfo
				{
					Id = "pet_panda_up0",
					Up = 0,
					IdWithoutUp = "pet_panda",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 0,
					Lkey = "Key_2876",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_panda_up1",
				new PetInfo
				{
					Id = "pet_panda_up1",
					Up = 1,
					IdWithoutUp = "pet_panda",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 0,
					Lkey = "Key_2876",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_panda_up2",
				new PetInfo
				{
					Id = "pet_panda_up2",
					Up = 2,
					IdWithoutUp = "pet_panda",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 1,
					Lkey = "Key_2876",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_panda_up3",
				new PetInfo
				{
					Id = "pet_panda_up3",
					Up = 3,
					IdWithoutUp = "pet_panda",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 2,
					Lkey = "Key_2876",
					ToUpPoints = 12,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_panda_up4",
				new PetInfo
				{
					Id = "pet_panda_up4",
					Up = 4,
					IdWithoutUp = "pet_panda",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 3,
					Lkey = "Key_2876",
					ToUpPoints = 20,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_panda_up5",
				new PetInfo
				{
					Id = "pet_panda_up5",
					Up = 5,
					IdWithoutUp = "pet_panda",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 4,
					Lkey = "Key_2876",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 5,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, -0.45f, 3.6f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_parrot_up0",
				new PetInfo
				{
					Id = "pet_parrot_up0",
					Up = 0,
					IdWithoutUp = "pet_parrot",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 0,
					Lkey = "Key_2509",
					ToUpPoints = 2,
					AttackDistance = 1f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0f, 3.4f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_parrot_up1",
				new PetInfo
				{
					Id = "pet_parrot_up1",
					Up = 1,
					IdWithoutUp = "pet_parrot",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 0,
					Lkey = "Key_2509",
					ToUpPoints = 4,
					AttackDistance = 1f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0f, 3.4f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_parrot_up2",
				new PetInfo
				{
					Id = "pet_parrot_up2",
					Up = 2,
					IdWithoutUp = "pet_parrot",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 1,
					Lkey = "Key_2509",
					ToUpPoints = 8,
					AttackDistance = 1f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0f, 3.4f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_parrot_up3",
				new PetInfo
				{
					Id = "pet_parrot_up3",
					Up = 3,
					IdWithoutUp = "pet_parrot",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 2,
					Lkey = "Key_2509",
					ToUpPoints = 16,
					AttackDistance = 1f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0f, 3.4f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_parrot_up4",
				new PetInfo
				{
					Id = "pet_parrot_up4",
					Up = 4,
					IdWithoutUp = "pet_parrot",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 3,
					Lkey = "Key_2509",
					ToUpPoints = 32,
					AttackDistance = 1f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0f, 3.4f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_parrot_up5",
				new PetInfo
				{
					Id = "pet_parrot_up5",
					Up = 5,
					IdWithoutUp = "pet_parrot",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 4,
					Lkey = "Key_2509",
					ToUpPoints = 32,
					AttackDistance = 1f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, 0f, 3.4f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Flying }
				}
			},
			{
				"pet_penguin_up0",
				new PetInfo
				{
					Id = "pet_penguin_up0",
					Up = 0,
					IdWithoutUp = "pet_penguin",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 0,
					Lkey = "Key_2705",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.29f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_penguin_up1",
				new PetInfo
				{
					Id = "pet_penguin_up1",
					Up = 1,
					IdWithoutUp = "pet_penguin",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 0,
					Lkey = "Key_2705",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.29f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_penguin_up2",
				new PetInfo
				{
					Id = "pet_penguin_up2",
					Up = 2,
					IdWithoutUp = "pet_penguin",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 1,
					Lkey = "Key_2705",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.29f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_penguin_up3",
				new PetInfo
				{
					Id = "pet_penguin_up3",
					Up = 3,
					IdWithoutUp = "pet_penguin",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 2,
					Lkey = "Key_2705",
					ToUpPoints = 16,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.29f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_penguin_up4",
				new PetInfo
				{
					Id = "pet_penguin_up4",
					Up = 4,
					IdWithoutUp = "pet_penguin",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 3,
					Lkey = "Key_2705",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.29f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_penguin_up5",
				new PetInfo
				{
					Id = "pet_penguin_up5",
					Up = 5,
					IdWithoutUp = "pet_penguin",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 4,
					Lkey = "Key_2705",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.4f, 3.29f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_phoenix_up0",
				new PetInfo
				{
					Id = "pet_phoenix_up0",
					Up = 0,
					IdWithoutUp = "pet_phoenix",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 0,
					Lkey = "Key_2700",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Burn,
					poisonCount = 6,
					poisonTime = 0.5f,
					poisonDamagePercent = 0.1f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, 0.15f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Burning }
				}
			},
			{
				"pet_phoenix_up1",
				new PetInfo
				{
					Id = "pet_phoenix_up1",
					Up = 1,
					IdWithoutUp = "pet_phoenix",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 0,
					Lkey = "Key_2700",
					ToUpPoints = 3,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Burn,
					poisonCount = 6,
					poisonTime = 0.5f,
					poisonDamagePercent = 0.1f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, 0.15f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Burning }
				}
			},
			{
				"pet_phoenix_up2",
				new PetInfo
				{
					Id = "pet_phoenix_up2",
					Up = 2,
					IdWithoutUp = "pet_phoenix",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 1,
					Lkey = "Key_2700",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Burn,
					poisonCount = 6,
					poisonTime = 0.5f,
					poisonDamagePercent = 0.1f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, 0.15f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Burning }
				}
			},
			{
				"pet_phoenix_up3",
				new PetInfo
				{
					Id = "pet_phoenix_up3",
					Up = 3,
					IdWithoutUp = "pet_phoenix",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 2,
					Lkey = "Key_2700",
					ToUpPoints = 5,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Burn,
					poisonCount = 6,
					poisonTime = 0.5f,
					poisonDamagePercent = 0.1f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, 0.15f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Burning }
				}
			},
			{
				"pet_phoenix_up4",
				new PetInfo
				{
					Id = "pet_phoenix_up4",
					Up = 4,
					IdWithoutUp = "pet_phoenix",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 3,
					Lkey = "Key_2700",
					ToUpPoints = 6,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Burn,
					poisonCount = 6,
					poisonTime = 0.5f,
					poisonDamagePercent = 0.1f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, 0.15f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Burning }
				}
			},
			{
				"pet_phoenix_up5",
				new PetInfo
				{
					Id = "pet_phoenix_up5",
					Up = 5,
					IdWithoutUp = "pet_phoenix",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 4,
					Lkey = "Key_2700",
					ToUpPoints = 7,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Burn,
					poisonCount = 6,
					poisonTime = 0.5f,
					poisonDamagePercent = 0.1f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0.046f, 0.15f, 3.823f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Burning }
				}
			},
			{
				"pet_pterodactyl_up0",
				new PetInfo
				{
					Id = "pet_pterodactyl_up0",
					Up = 0,
					IdWithoutUp = "pet_pterodactyl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 0,
					Lkey = "Key_2707",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, -0.12f, 3.5f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_pterodactyl_up1",
				new PetInfo
				{
					Id = "pet_pterodactyl_up1",
					Up = 1,
					IdWithoutUp = "pet_pterodactyl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 0,
					Lkey = "Key_2707",
					ToUpPoints = 3,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0f, -0.12f, 3.5f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_pterodactyl_up2",
				new PetInfo
				{
					Id = "pet_pterodactyl_up2",
					Up = 2,
					IdWithoutUp = "pet_pterodactyl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 1,
					Lkey = "Key_2707",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.4f,
					PositionInBanners = new Vector3(0f, -0.12f, 3.5f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_pterodactyl_up3",
				new PetInfo
				{
					Id = "pet_pterodactyl_up3",
					Up = 3,
					IdWithoutUp = "pet_pterodactyl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 2,
					Lkey = "Key_2707",
					ToUpPoints = 5,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.4f,
					PositionInBanners = new Vector3(0f, -0.12f, 3.5f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_pterodactyl_up4",
				new PetInfo
				{
					Id = "pet_pterodactyl_up4",
					Up = 4,
					IdWithoutUp = "pet_pterodactyl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 3,
					Lkey = "Key_2707",
					ToUpPoints = 6,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.5f,
					PositionInBanners = new Vector3(0f, -0.12f, 3.5f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_pterodactyl_up5",
				new PetInfo
				{
					Id = "pet_pterodactyl_up5",
					Up = 5,
					IdWithoutUp = "pet_pterodactyl",
					Behaviour = PetInfo.BehaviourType.Flying,
					Rarity = ItemDb.ItemRarity.Legendary,
					Tier = 4,
					Lkey = "Key_2707",
					ToUpPoints = 7,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 4f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.5f,
					PositionInBanners = new Vector3(0f, -0.12f, 3.5f),
					RotationInBanners = new Vector3(-14.102f, 162.642f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_rabbit_up0",
				new PetInfo
				{
					Id = "pet_rabbit_up0",
					Up = 0,
					IdWithoutUp = "pet_rabbit",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 0,
					Lkey = "Key_2704",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_rabbit_up1",
				new PetInfo
				{
					Id = "pet_rabbit_up1",
					Up = 1,
					IdWithoutUp = "pet_rabbit",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 0,
					Lkey = "Key_2704",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_rabbit_up2",
				new PetInfo
				{
					Id = "pet_rabbit_up2",
					Up = 2,
					IdWithoutUp = "pet_rabbit",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 1,
					Lkey = "Key_2704",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_rabbit_up3",
				new PetInfo
				{
					Id = "pet_rabbit_up3",
					Up = 3,
					IdWithoutUp = "pet_rabbit",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 2,
					Lkey = "Key_2704",
					ToUpPoints = 16,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_rabbit_up4",
				new PetInfo
				{
					Id = "pet_rabbit_up4",
					Up = 4,
					IdWithoutUp = "pet_rabbit",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 3,
					Lkey = "Key_2704",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_rabbit_up5",
				new PetInfo
				{
					Id = "pet_rabbit_up5",
					Up = 5,
					IdWithoutUp = "pet_rabbit",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Common,
					Tier = 4,
					Lkey = "Key_2704",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.Melee }
				}
			},
			{
				"pet_snake_up0",
				new PetInfo
				{
					Id = "pet_snake_up0",
					Up = 0,
					IdWithoutUp = "pet_snake",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 0,
					Lkey = "Key_2612",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Toxic,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.PoisonShots }
				}
			},
			{
				"pet_snake_up1",
				new PetInfo
				{
					Id = "pet_snake_up1",
					Up = 1,
					IdWithoutUp = "pet_snake",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 0,
					Lkey = "Key_2612",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Toxic,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.PoisonShots }
				}
			},
			{
				"pet_snake_up2",
				new PetInfo
				{
					Id = "pet_snake_up2",
					Up = 2,
					IdWithoutUp = "pet_snake",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 1,
					Lkey = "Key_2612",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Toxic,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.PoisonShots }
				}
			},
			{
				"pet_snake_up3",
				new PetInfo
				{
					Id = "pet_snake_up3",
					Up = 3,
					IdWithoutUp = "pet_snake",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 2,
					Lkey = "Key_2612",
					ToUpPoints = 12,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Toxic,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.PoisonShots }
				}
			},
			{
				"pet_snake_up4",
				new PetInfo
				{
					Id = "pet_snake_up4",
					Up = 4,
					IdWithoutUp = "pet_snake",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 3,
					Lkey = "Key_2612",
					ToUpPoints = 20,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Toxic,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.PoisonShots }
				}
			},
			{
				"pet_snake_up5",
				new PetInfo
				{
					Id = "pet_snake_up5",
					Up = 5,
					IdWithoutUp = "pet_snake",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Rare,
					Tier = 4,
					Lkey = "Key_2612",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = true,
					poisonType = Player_move_c.PoisonType.Toxic,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.3f,
					criticalHitChance = 0,
					criticalHitCoef = 2f,
					PositionInBanners = new Vector3(0f, -0.316f, 2.614f),
					RotationInBanners = new Vector3(0f, 161.0368f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.PoisonShots }
				}
			},
			{
				"pet_unicorn_up0",
				new PetInfo
				{
					Id = "pet_unicorn_up0",
					Up = 0,
					IdWithoutUp = "pet_unicorn",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 0,
					Lkey = "Key_2508",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.5f,
					PositionInBanners = new Vector3(0.03137f, -0.523f, 4.367f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_unicorn_up1",
				new PetInfo
				{
					Id = "pet_unicorn_up1",
					Up = 1,
					IdWithoutUp = "pet_unicorn",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 0,
					Lkey = "Key_2508",
					ToUpPoints = 3,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.5f,
					PositionInBanners = new Vector3(0.03137f, -0.523f, 4.367f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_unicorn_up2",
				new PetInfo
				{
					Id = "pet_unicorn_up2",
					Up = 2,
					IdWithoutUp = "pet_unicorn",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 1,
					Lkey = "Key_2508",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.6f,
					PositionInBanners = new Vector3(0.03137f, -0.523f, 4.367f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_unicorn_up3",
				new PetInfo
				{
					Id = "pet_unicorn_up3",
					Up = 3,
					IdWithoutUp = "pet_unicorn",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 2,
					Lkey = "Key_2508",
					ToUpPoints = 5,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.6f,
					PositionInBanners = new Vector3(0.03137f, -0.523f, 4.367f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_unicorn_up4",
				new PetInfo
				{
					Id = "pet_unicorn_up4",
					Up = 4,
					IdWithoutUp = "pet_unicorn",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 3,
					Lkey = "Key_2508",
					ToUpPoints = 6,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.7f,
					PositionInBanners = new Vector3(0.03137f, -0.523f, 4.367f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_unicorn_up5",
				new PetInfo
				{
					Id = "pet_unicorn_up5",
					Up = 5,
					IdWithoutUp = "pet_unicorn",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Mythic,
					Tier = 4,
					Lkey = "Key_2508",
					ToUpPoints = 32,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 7,
					criticalHitCoef = 1.7f,
					PositionInBanners = new Vector3(0.03137f, -0.523f, 4.367f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_wolf_up0",
				new PetInfo
				{
					Id = "pet_wolf_up0",
					Up = 0,
					IdWithoutUp = "pet_wolf",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 0,
					Lkey = "Key_2706",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 4,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_wolf_up1",
				new PetInfo
				{
					Id = "pet_wolf_up1",
					Up = 1,
					IdWithoutUp = "pet_wolf",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 0,
					Lkey = "Key_2706",
					ToUpPoints = 4,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 4,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_wolf_up2",
				new PetInfo
				{
					Id = "pet_wolf_up2",
					Up = 2,
					IdWithoutUp = "pet_wolf",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 1,
					Lkey = "Key_2706",
					ToUpPoints = 8,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 4,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_wolf_up3",
				new PetInfo
				{
					Id = "pet_wolf_up3",
					Up = 3,
					IdWithoutUp = "pet_wolf",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 2,
					Lkey = "Key_2706",
					ToUpPoints = 12,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 4,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_wolf_up4",
				new PetInfo
				{
					Id = "pet_wolf_up4",
					Up = 4,
					IdWithoutUp = "pet_wolf",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 3,
					Lkey = "Key_2706",
					ToUpPoints = 17,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 4,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			},
			{
				"pet_wolf_up5",
				new PetInfo
				{
					Id = "pet_wolf_up5",
					Up = 5,
					IdWithoutUp = "pet_wolf",
					Behaviour = PetInfo.BehaviourType.Ground,
					Rarity = ItemDb.ItemRarity.Uncommon,
					Tier = 4,
					Lkey = "Key_2706",
					ToUpPoints = 2,
					AttackDistance = 2f,
					AttackStopDistance = 3f,
					MinToOwnerDistance = 2f,
					MaxToOwnerDistance = 20f,
					TargetDetectRange = 15f,
					OffenderDetectRange = 10f,
					ToTargetTeleportDistance = 7f,
					poisonEnabled = false,
					poisonType = Player_move_c.PoisonType.None,
					poisonCount = 3,
					poisonTime = 2f,
					poisonDamagePercent = 0.033f,
					criticalHitChance = 4,
					criticalHitCoef = 1.3f,
					PositionInBanners = new Vector3(0.03137f, -0.363f, 3.0128f),
					RotationInBanners = new Vector3(0f, 154.398f, 0f),
					Parameters = new List<GadgetInfo.Parameter>
					{
						GadgetInfo.Parameter.Attack,
						GadgetInfo.Parameter.HP,
						GadgetInfo.Parameter.Speed,
						GadgetInfo.Parameter.Respawn
					},
					Effects = new List<WeaponSounds.Effects> { WeaponSounds.Effects.CriticalDamage }
				}
			}
		};
	}
}

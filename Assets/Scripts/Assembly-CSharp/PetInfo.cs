using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PetInfo
{
	public enum BehaviourType
	{
		Ground,
		Flying
	}

	public enum Parameter
	{
		Durability,
		Attack,
		Cooldown
	}

	public enum Effect
	{
		Healing,
		Flamethrower,
		Poison
	}

	public List<GadgetInfo.Parameter> Parameters;

	public List<WeaponSounds.Effects> Effects;

	public string Id;

	public int Up;

	public string IdWithoutUp;

	public BehaviourType Behaviour;

	public ItemDb.ItemRarity Rarity;

	public int Tier;

	public string Lkey;

	public int ToUpPoints;

	public float AttackDistance;

	public float AttackStopDistance;

	public float MinToOwnerDistance;

	public float MaxToOwnerDistance;

	public float TargetDetectRange;

	public float OffenderDetectRange = 10f;

	public float ToTargetTeleportDistance;

	public bool poisonEnabled;

	public Player_move_c.PoisonType poisonType;

	public int poisonCount;

	public float poisonTime;

	public float poisonDamagePercent;

	public int criticalHitChance;

	public float criticalHitCoef;

	private Vector3 m_positionInBanners;

	private Vector3 m_rotationInBanners;

	public float HP
	{
		get
		{
			if (BalanceController.hpPets.ContainsKey(Id))
			{
				return BalanceController.hpPets[Id];
			}
			return 9f;
		}
	}

	public float Attack
	{
		get
		{
			if (BalanceController.damagePets.ContainsKey(Id))
			{
				return BalanceController.damagePets[Id];
			}
			return 1f;
		}
	}

	public float SurvivalAttack
	{
		get
		{
			if (BalanceController.survivalDamagePets.ContainsKey(Id))
			{
				return BalanceController.survivalDamagePets[Id];
			}
			return 1f;
		}
	}

	public int DPS
	{
		get
		{
			if (BalanceController.dpsPets.ContainsKey(Id))
			{
				return BalanceController.dpsPets[Id];
			}
			return 1;
		}
	}

	public float SpeedModif
	{
		get
		{
			if (BalanceController.speedPets.ContainsKey(Id))
			{
				return BalanceController.speedPets[Id];
			}
			return 4f;
		}
	}

	public float RespawnTime
	{
		get
		{
			if (BalanceController.respawnTimePets.ContainsKey(Id))
			{
				return BalanceController.respawnTimePets[Id];
			}
			return 4f;
		}
	}

	public float Cashback
	{
		get
		{
			if (BalanceController.cashbackPets.ContainsKey(Id))
			{
				return BalanceController.cashbackPets[Id];
			}
			return 4f;
		}
	}

	public Vector3 PositionInBanners
	{
		get
		{
			return m_positionInBanners;
		}
		set
		{
			m_positionInBanners = value;
		}
	}

	public Vector3 RotationInBanners
	{
		get
		{
			return m_rotationInBanners;
		}
		set
		{
			m_rotationInBanners = value;
		}
	}

	public string GetRelativePrefabPath()
	{
		return string.Format("Pets/Content/{0}_up0", IdWithoutUp);
	}
}

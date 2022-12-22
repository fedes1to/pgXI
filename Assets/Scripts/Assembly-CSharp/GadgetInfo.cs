using System.Collections.Generic;

public class GadgetInfo
{
	public enum Parameter
	{
		Damage,
		Durability,
		Healing,
		Lifetime,
		Cooldown,
		Attack,
		HP,
		Speed,
		Respawn
	}

	public enum GadgetCategory
	{
		Throwing = 12500,
		Support = 13500,
		Tools = 13000
	}

	private int _tier_FROM_ZERO;

	public List<Parameter> Parameters { get; protected set; }

	public List<WeaponSounds.Effects> Effects { get; protected set; }

	public GadgetCategory Category { get; protected set; }

	public string Id { get; protected set; }

	public string PreviousUpgradeId { get; protected set; }

	public string NextUpgradeId { get; protected set; }

	public string Lkey { get; protected set; }

	public string DescriptionLkey { get; protected set; }

	public PlayerEventScoreController.ScoreEvent KillStreakType { get; protected set; }

	public float Heal
	{
		get
		{
			if (BalanceController.healGadgetes.ContainsKey(Id))
			{
				return BalanceController.healGadgetes[Id];
			}
			return 5f;
		}
	}

	public float HPS
	{
		get
		{
			if (BalanceController.hpsGadgetes.ContainsKey(Id))
			{
				return BalanceController.hpsGadgetes[Id];
			}
			return 1f;
		}
	}

	public float Durability
	{
		get
		{
			if (BalanceController.durabilityGadgetes.ContainsKey(Id))
			{
				return BalanceController.durabilityGadgetes[Id];
			}
			return 5f;
		}
	}

	public float DPS
	{
		get
		{
			if (BalanceController.dpsGadgetes.ContainsKey(Id))
			{
				return BalanceController.dpsGadgetes[Id];
			}
			return 1f;
		}
	}

	public float Damage
	{
		get
		{
			if (BalanceController.damageGadgetes.ContainsKey(Id))
			{
				return BalanceController.damageGadgetes[Id];
			}
			return 3f;
		}
	}

	public float Amplification
	{
		get
		{
			if (BalanceController.amplificationGadgetes.ContainsKey(Id))
			{
				return BalanceController.amplificationGadgetes[Id];
			}
			return 25f;
		}
	}

	public int SurvivalDamage
	{
		get
		{
			if (BalanceController.survivalDamageGadgetes.ContainsKey(Id))
			{
				return BalanceController.survivalDamageGadgetes[Id];
			}
			return 100;
		}
	}

	public float Duration
	{
		get
		{
			if (BalanceController.durationGadgetes.ContainsKey(Id))
			{
				return BalanceController.durationGadgetes[Id];
			}
			return 15f;
		}
	}

	public float Cooldown
	{
		get
		{
			if (BalanceController.cooldownGadgetes.ContainsKey(Id))
			{
				return BalanceController.cooldownGadgetes[Id];
			}
			return 10f;
		}
	}

	public int Tier
	{
		get
		{
			return _tier_FROM_ZERO;
		}
		protected set
		{
			_tier_FROM_ZERO = value;
		}
	}

	public GadgetInfo(GadgetCategory cat, string id, string locKey, int tier_FROM_ONE, string previousUpgradeId, string nextUpgradeId, string descriptionLkey, List<Parameter> parameters, List<WeaponSounds.Effects> effects, PlayerEventScoreController.ScoreEvent killStreakType)
	{
		Category = cat;
		Id = id;
		Lkey = locKey;
		_tier_FROM_ZERO = tier_FROM_ONE - 1;
		PreviousUpgradeId = previousUpgradeId;
		NextUpgradeId = nextUpgradeId;
		DescriptionLkey = descriptionLkey;
		Parameters = parameters;
		Effects = effects;
		KillStreakType = killStreakType;
	}
}

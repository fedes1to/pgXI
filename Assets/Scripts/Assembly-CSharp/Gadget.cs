using System;
using Rilisoft;
using UnityEngine;

public abstract class Gadget
{
	protected SaltedFloat _cooldownTime = new SaltedFloat(0f);

	protected SaltedFloat _durationTime = new SaltedFloat(0f);

	public virtual bool CanUse
	{
		get
		{
			return _cooldownTime.value == 0f && _durationTime.value == 0f;
		}
	}

	public float CooldownProgress
	{
		get
		{
			return _cooldownTime.value / Info.Cooldown;
		}
	}

	public float ExpirationProgress
	{
		get
		{
			return _durationTime.value / Info.Duration;
		}
	}

	public virtual GadgetInfo Info { get; protected set; }

	protected Gadget(GadgetInfo _info)
	{
		Info = _info;
	}

	public static Gadget Create(GadgetInfo gadgetInfo)
	{
		if (gadgetInfo == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(gadgetInfo.Id))
		{
			return null;
		}
		Gadget result = null;
		try
		{
			if (GadgetsInfo.Upgrades["gadget_fraggrenade"].Contains(gadgetInfo.Id))
			{
				result = new BaseGrenadeGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_molotov"].Contains(gadgetInfo.Id))
			{
				result = new BaseGrenadeGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_mine"].Contains(gadgetInfo.Id))
			{
				result = new BaseGrenadeGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_firework"].Contains(gadgetInfo.Id))
			{
				result = new BaseGrenadeGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_fakebonus"].Contains(gadgetInfo.Id))
			{
				result = new BaseGrenadeGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_singularity"].Contains(gadgetInfo.Id))
			{
				result = new BaseGrenadeGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_nucleargrenade"].Contains(gadgetInfo.Id))
			{
				result = new BaseGrenadeGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_nutcracker"].Contains(gadgetInfo.Id))
			{
				result = new BaseGrenadeGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_Blizzard_generator"].Contains(gadgetInfo.Id))
			{
				result = new BaseGrenadeGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_black_label"].Contains(gadgetInfo.Id))
			{
				result = new BaseGrenadeGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_stickycandy"].Contains(gadgetInfo.Id))
			{
				result = new BaseGrenadeGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_ninjashurickens"].Contains(gadgetInfo.Id))
			{
				result = new ShurikenGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_turret"].Contains(gadgetInfo.Id))
			{
				result = new BaseTurretGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_shield"].Contains(gadgetInfo.Id))
			{
				result = new BaseTurretGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_medicalstation"].Contains(gadgetInfo.Id))
			{
				result = new BaseTurretGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_snowman"].Contains(gadgetInfo.Id))
			{
				result = new BaseTurretGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_christmastreeturret"].Contains(gadgetInfo.Id))
			{
				result = new BaseTurretGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_stealth"].Contains(gadgetInfo.Id))
			{
				result = new BaseEffectGadget(gadgetInfo, Player_move_c.GadgetEffect.invisible);
			}
			if (GadgetsInfo.Upgrades["gadget_jetpack"].Contains(gadgetInfo.Id))
			{
				result = new BaseEffectGadget(gadgetInfo, Player_move_c.GadgetEffect.jetpack);
			}
			if (GadgetsInfo.Upgrades["gadget_reflector"].Contains(gadgetInfo.Id))
			{
				result = new BaseEffectGadget(gadgetInfo, Player_move_c.GadgetEffect.reflector, true);
			}
			if (GadgetsInfo.Upgrades["gadget_leaderdrum"].Contains(gadgetInfo.Id))
			{
				result = new BaseEffectGadget(gadgetInfo, Player_move_c.GadgetEffect.drumSupport, true);
			}
			if (GadgetsInfo.Upgrades["gadget_petbooster"].Contains(gadgetInfo.Id))
			{
				result = new PetAdrenalineGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_mech"].Contains(gadgetInfo.Id))
			{
				result = new MechGadget(gadgetInfo, Player_move_c.GadgetEffect.mech);
			}
			if (GadgetsInfo.Upgrades["gadget_demon_stone"].Contains(gadgetInfo.Id))
			{
				result = new MechGadget(gadgetInfo, Player_move_c.GadgetEffect.demon);
			}
			if (GadgetsInfo.Upgrades["gadget_medkit"].Contains(gadgetInfo.Id))
			{
				result = new HealthGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_timewatch"].Contains(gadgetInfo.Id))
			{
				result = new TimeWatchGadget(gadgetInfo);
			}
			//if (GadgetsInfo.Upgrades["gadget_resurrection"].Contains(gadgetInfo.Id))
			//{
			//	result = new RessurectionGadget(gadgetInfo);
			//}
			if (GadgetsInfo.Upgrades["gadget_disabler"].Contains(gadgetInfo.Id))
			{
				result = new DisablerGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_firemushroom"].Contains(gadgetInfo.Id))
			{
				result = new FireMushroomGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_dragonwhistle"].Contains(gadgetInfo.Id))
			{
				result = new DragonWhistleGadget(gadgetInfo);
			}
			if (GadgetsInfo.Upgrades["gadget_pandorabox"].Contains(gadgetInfo.Id))
			{
				result = new PandoraBoxGadget(gadgetInfo);
				return result;
			}
			return result;
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in Gadget.Create: {0}", ex);
			return result;
		}
	}

	public void ResetUseCounter()
	{
		_durationTime.value = 0f;
		OnTimeExpire();
	}

	public void ResetCooldownCounter()
	{
		_cooldownTime.value = 0f;
	}

	public abstract void PreUse();

	public abstract void Use();

	public virtual void PostUse()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void StartCooldown()
	{
		_cooldownTime.value = Info.Cooldown;
	}

	public virtual void StartUseTimer()
	{
		_durationTime.value = Info.Duration;
	}

	public virtual void Step(float time)
	{
		if (_cooldownTime.value > 0f)
		{
			_cooldownTime.value -= time;
			if (_cooldownTime.value < 0f)
			{
				_cooldownTime.value = 0f;
			}
		}
		if (_durationTime.value > 0f)
		{
			_durationTime.value -= time;
			if (_durationTime.value < 0f)
			{
				_durationTime.value = 0f;
				OnTimeExpire();
			}
		}
		Update();
	}

	public virtual void OnKill(bool inDeathCollider)
	{
	}

	public virtual void OnTimeExpire()
	{
	}

	public virtual void OnMatchEnd()
	{
	}
}

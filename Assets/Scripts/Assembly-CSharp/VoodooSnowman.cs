using System.Collections.Generic;
using UnityEngine;

public class VoodooSnowman : TurretController
{
	private IDamageable voodooTarget;

	private bool targetSelected;

	public override void ApplyDamage(float damage, IDamageable damageFrom, Player_move_c.TypeKills typeKill, WeaponSounds.TypeDead typeDead, string weaponName, int killerId = 0)
	{
		if (targetSelected && !voodooTarget.Equals(null) && !voodooTarget.IsDead())
		{
			StopCoroutine(FlashRed());
			StartCoroutine(FlashRed());
			if (Defs.isSoundFX)
			{
				GetComponent<AudioSource>().PlayOneShot(hitSound);
			}
			voodooTarget.ApplyDamage(damage * 0.5f, damageFrom, typeKill, typeDead, gadgetName, killerId);
		}
	}

	public override bool IsEnemyTo(Player_move_c player)
	{
		if (!myPlayerMoveC.Equals(player))
		{
			return false;
		}
		return true;
	}

	protected override void UpdateTurret()
	{
		base.UpdateTurret();
		if (isReady && (!Defs.isMulti || isMine) && !targetSelected)
		{
			SelectVoodooTarget();
		}
		if (targetSelected && (voodooTarget.Equals(null) || voodooTarget.IsDead()))
		{
			if (GadgetOnKill != null)
			{
				GadgetOnKill();
			}
			SendImKilledRPC();
		}
	}

	private void SelectVoodooTarget()
	{
		if (Defs.isMulti && !Defs.isCOOP)
		{
			List<Player_move_c> list = new List<Player_move_c>(9);
			for (int i = 0; i < Initializer.players.Count; i++)
			{
				if (!Initializer.players[i].Equals(myPlayerMoveC) && !Initializer.players[i].isKilled && !Initializer.players[i].isImmortality && Initializer.players[i].myDamageable.IsEnemyTo(myPlayerMoveC))
				{
					list.Add(Initializer.players[i]);
				}
			}
			if (list.Count > 0)
			{
				voodooTarget = list[Random.Range(0, list.Count)].myDamageable;
			}
		}
		else if (Initializer.enemiesObj.Count > 0)
		{
			voodooTarget = Initializer.enemiesObj[Random.Range(0, Initializer.enemiesObj.Count)].GetComponent<IDamageable>();
		}
		if (voodooTarget != null)
		{
			targetSelected = true;
		}
	}

	public override void StartTurret()
	{
		base.StartTurret();
	}
}

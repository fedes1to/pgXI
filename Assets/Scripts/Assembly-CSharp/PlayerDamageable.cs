using System;
using UnityEngine;

public class PlayerDamageable : MonoBehaviour, IDamageable
{
	[NonSerialized]
	public Player_move_c myPlayer;

	public bool isLivingTarget
	{
		get
		{
			return true;
		}
	}

	private void Awake()
	{
		myPlayer = GetComponent<SkinName>().playerMoveC;
	}

	public void ApplyDamage(float damage, IDamageable damageFrom, Player_move_c.TypeKills typeKill)
	{
		ApplyDamage(damage, damageFrom, typeKill, WeaponSounds.TypeDead.angel, string.Empty);
	}

	public void ApplyDamage(float damage, IDamageable damageFrom, Player_move_c.TypeKills typeKill, WeaponSounds.TypeDead typeDead, string weaponName, int killerViewId = 0)
	{
		if (!Defs.isDaterRegim)
		{
			Vector3 posKiller = Vector3.zero;
			if (damageFrom != null)
			{
				posKiller = (damageFrom as MonoBehaviour).transform.position;
			}
			if (damageFrom != null && typeKill != Player_move_c.TypeKills.reflector && myPlayer.IsGadgetEffectActive(Player_move_c.GadgetEffect.reflector))
			{
				damage /= 2f;
				damageFrom.ApplyDamage(damage, this, Player_move_c.TypeKills.reflector, typeDead, weaponName, myPlayer.skinNamePixelView.viewID);
			}
			myPlayer.GetDamage(damage, typeKill, typeDead, posKiller, weaponName, killerViewId);
		}
	}

	public bool IsEnemyTo(Player_move_c player)
	{
		if (!Defs.isMulti || (!player.Equals(myPlayer) && (Defs.isCOOP || (ConnectSceneNGUIController.isTeamRegim && myPlayer.myCommand == player.myCommand))))
		{
			return false;
		}
		return true;
	}

	public bool IsDead()
	{
		return myPlayer.isKilled || myPlayer.isImmortality;
	}
}

using Photon;
using UnityEngine;

public class ChestController : Photon.MonoBehaviour, IDamageable
{
	public bool isStartChest = true;

	public int currentSpawnZone;

	public float live = 5f;

	public bool isKilled;

	public bool isChestBonus;

	public static readonly int[] weaponForHungerGames = new int[12]
	{
		1, 2, 3, 8, 53, 5, 52, 51, 66, 67,
		162, 333
	};

	public AudioClip brokenAudio;

	private bool oldIsMaster;

	public bool isLivingTarget
	{
		get
		{
			return false;
		}
	}

	private void Start()
	{
		Initializer.damageableObjects.Add(base.gameObject);
	}

	private void OnDestroy()
	{
		Initializer.damageableObjects.Remove(base.gameObject);
	}

	private void Update()
	{
		if (!oldIsMaster && PhotonNetwork.isMasterClient && isKilled)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		oldIsMaster = PhotonNetwork.isMasterClient;
	}

	public void ApplyDamage(float damage, IDamageable damageFrom, Player_move_c.TypeKills typeKill)
	{
		ApplyDamage(damage, damageFrom, typeKill, WeaponSounds.TypeDead.angel, string.Empty);
	}

	public void ApplyDamage(float damage, IDamageable damageFrom, Player_move_c.TypeKills typeKill, WeaponSounds.TypeDead typeDead, string weaponName, int killerViewId = 0)
	{
		MinusLive(damage);
	}

	public bool IsEnemyTo(Player_move_c player)
	{
		return true;
	}

	public bool IsDead()
	{
		return isKilled;
	}

	public void MinusLive(float _minus)
	{
		base.photonView.RPC("KilledChest", PhotonTargets.All);
	}

	[PunRPC]
	[RPC]
	public void MinusLiveRPC(float _minus)
	{
		if (!isKilled)
		{
			live -= _minus;
			base.photonView.RPC("SynchLiveRPC", PhotonTargets.AllBuffered, live);
			if (live <= 0f)
			{
				base.photonView.RPC("KilledChest", PhotonTargets.AllBuffered);
			}
		}
	}

	[PunRPC]
	[RPC]
	public void SynchLiveRPC(float _live)
	{
		live = _live;
	}

	[RPC]
	[PunRPC]
	public void KilledChest()
	{
		if (isKilled)
		{
			return;
		}
		isKilled = true;
		if (PhotonNetwork.isMasterClient)
		{
			int num = Random.Range(0, weaponForHungerGames.Length);
			if (isChestBonus)
			{
				if (Random.Range(0, 11) < 7)
				{
					BonusController.sharedController.AddBonusForHunger(base.transform.position, TypeBonus(), base.transform.GetComponent<SettingBonus>().numberSpawnZone);
				}
				else
				{
					PhotonNetwork.InstantiateSceneObject("Weapon_Bonuses/Weapon" + weaponForHungerGames[num] + "_Bonus", base.transform.position, base.transform.rotation, 0, null);
				}
			}
			else
			{
				PhotonNetwork.InstantiateSceneObject("Weapon_Bonuses/Weapon" + weaponForHungerGames[num] + "_Bonus", base.transform.position, base.transform.rotation, 0, null);
			}
		}
		if (Defs.isSoundFX)
		{
			base.gameObject.GetComponent<AudioSource>().PlayOneShot(brokenAudio);
		}
		GetComponent<Animation>().Stop();
		GetComponent<Animation>().Play("Broken");
		Invoke("DestroyChestRPC", 0.5f);
	}

	private int TypeBonus()
	{
		int num = Random.Range(0, 100);
		if (num < 70)
		{
			return 0;
		}
		return 1;
	}

	private void DestroyChest()
	{
		base.photonView.RPC("DestroyChestRPC", PhotonTargets.AllBuffered);
	}

	[PunRPC]
	[RPC]
	private void DestroyChestRPC()
	{
		Debug.Log("DestroyChestRPC");
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		else
		{
			base.transform.position = new Vector3(0f, -10000f, 0f);
		}
	}
}

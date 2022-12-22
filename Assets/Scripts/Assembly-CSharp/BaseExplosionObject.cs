using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public class BaseExplosionObject : MonoBehaviour
{
	private const string ExplosionAnimationName = "Broken";

	[Header("Common Settings")]
	public GameObject explosionObject;

	[Header("Common Damage settings")]
	public float radiusExplosion;

	[Header("Common Damage settings")]
	public float radiusMaxExplosion;

	public float damageZombie = 2f;

	public float[] damageByTier = new float[ExpController.LevelsForTiers.Length];

	[Header("Common Effect settings")]
	public GameObject explosionEffect;

	protected bool isMultiplayerMode;

	protected PhotonView photonView;

	private ExplosionObjectRespawnController _respawnController;

	private void Start()
	{
		InitializeData();
		Initializer.damageableObjects.Add(base.gameObject);
	}

	private void OnDestroy()
	{
		Initializer.damageableObjects.Remove(base.gameObject);
	}

	protected virtual void InitializeData()
	{
		isMultiplayerMode = Defs.isMulti;
		photonView = PhotonView.Get(this);
		InitializeRespawnPoint();
	}

	private void InitializeRespawnPoint()
	{
		if (isMultiplayerMode && !PhotonNetwork.isMasterClient)
		{
			GameObject gameObject = null;
			float num = float.MaxValue;
			for (int i = 0; i < ExplosionObjectRespawnController.respawnList.Count; i++)
			{
				if (ExplosionObjectRespawnController.respawnList[i] != null)
				{
					float sqrMagnitude = (ExplosionObjectRespawnController.respawnList[i].transform.position - base.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						gameObject = ExplosionObjectRespawnController.respawnList[i];
					}
				}
			}
			if (gameObject != null)
			{
				base.transform.parent = gameObject.transform;
				_respawnController = gameObject.GetComponent<ExplosionObjectRespawnController>();
			}
			else
			{
				_respawnController = null;
			}
		}
		else
		{
			_respawnController = base.transform.parent.GetComponent<ExplosionObjectRespawnController>();
		}
	}

	private void PlayDestroyEffect()
	{
		Object.Instantiate(explosionEffect, base.transform.position, Quaternion.identity);
		GetComponent<Animation>().Play("Broken");
	}

	protected bool IsTargetAvailable(Transform targetTransform)
	{
		if (targetTransform.Equals(base.transform))
		{
			return false;
		}
		return targetTransform.CompareTag("Player") || targetTransform.CompareTag("Enemy") || targetTransform.CompareTag("Turret") || (targetTransform.childCount > 0 && targetTransform.GetChild(0).CompareTag("DamagedExplosion"));
	}

	private void CheckTakeDamage()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, radiusExplosion, Tools.AllWithoutDamageCollidersMask);
		if (array.Length == 0)
		{
			return;
		}
		List<Transform> list = new List<Transform>();
		float num = radiusExplosion * radiusExplosion;
		float diameterMaxExplosion = radiusMaxExplosion * radiusMaxExplosion;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject == null)
			{
				continue;
			}
			Transform root = array[i].transform.root;
			if (!(root.gameObject == null) && !(base.transform.gameObject == null) && !list.Contains(root) && IsTargetAvailable(root))
			{
				float sqrMagnitude = (root.position - base.transform.position).sqrMagnitude;
				if (!(sqrMagnitude > num))
				{
					ApplyDamage(root, sqrMagnitude, num, diameterMaxExplosion);
					list.Add(root);
				}
			}
		}
	}

	private void ApplyDamage(Transform target, float distanceToTarget, float diameterExplosion, float diameterMaxExplosion)
	{
		IDamageable component = target.GetComponent<IDamageable>();
		if (component != null)
		{
			float num = 0f;
			int num2 = ExpController.OurTierForAnyPlace();
			if (component is PlayerDamageable)
			{
				Player_move_c myPlayer = (component as PlayerDamageable).myPlayer;
				num2 = ((myPlayer.myTable != null) ? ExpController.TierForLevel(myPlayer.myTable.GetComponent<NetworkStartTable>().myRanks) : 0);
			}
			num = ((!(distanceToTarget > diameterMaxExplosion)) ? damageByTier[num2] : (damageByTier[num2] * ((diameterExplosion - (distanceToTarget - diameterMaxExplosion)) / diameterExplosion)));
			IDamageable damageFrom = null;
			if (this is IDamageable)
			{
				damageFrom = this as IDamageable;
			}
			component.ApplyDamage(num, damageFrom, Player_move_c.TypeKills.none);
		}
	}

	private void RecreateObject()
	{
		if (isMultiplayerMode)
		{
			DestroyObjectByNetworkRpc();
			photonView.RPC("DestroyObjectByNetworkRpc", PhotonTargets.Others);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		if (isMultiplayerMode)
		{
			StartNewRespanObjectRpc();
			photonView.RPC("StartNewRespanObjectRpc", PhotonTargets.Others);
		}
		else if (_respawnController != null)
		{
			_respawnController.StartProcessNewRespawn();
		}
	}

	public void RunExplosion()
	{
		if (isMultiplayerMode)
		{
			PlayDestroyEffect();
			photonView.RPC("PlayDestroyEffectRpc", PhotonTargets.Others);
		}
		else
		{
			PlayDestroyEffect();
		}
		CheckTakeDamage();
		RecreateObject();
	}

	[RPC]
	[PunRPC]
	public void DestroyObjectByNetworkRpc()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		else
		{
			explosionObject.SetActive(false);
		}
	}

	[RPC]
	[PunRPC]
	public void StartNewRespanObjectRpc()
	{
		if (_respawnController != null)
		{
			_respawnController.StartProcessNewRespawn();
		}
	}

	[PunRPC]
	[RPC]
	public void PlayDestroyEffectRpc()
	{
		PlayDestroyEffect();
	}
}

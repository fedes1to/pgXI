using System.Collections;
using Rilisoft;
using UnityEngine;

public sealed class TurretController_MusicBox : TurretController
{
	[Header("MusicBox settings")]
	public AudioClip musicDater;

	public Transform tower;

	public Transform gun;

	private bool isPlayMusicDater;

	protected override void SearchTarget()
	{
		base.SearchTarget();
		if (isPlayMusicDater)
		{
			PlayMusic(false);
			if (!Defs.isInet)
			{
				_networkView.RPC("PlayMusic", RPCMode.Others, false);
			}
			else
			{
				photonView.RPC("PlayMusic", PhotonTargets.Others, false);
			}
		}
	}

	protected override IEnumerator ScanTarget()
	{
		inScaning = true;
		GameObject closestTargetObj = null;
		float closestTarget = float.MaxValue;
		Initializer.TargetsList targets = new Initializer.TargetsList(WeaponManager.sharedManager.myPlayerMoveC, false, false);
		foreach (Transform enemy in targets)
		{
			Vector3 enemyDelta = enemy.position - base.transform.position;
			Vector3 enemyForward = new Vector3(enemyDelta.x, 0f, enemyDelta.z);
			float targetDistance = enemyDelta.sqrMagnitude;
			if (targetDistance < closestTarget && targetDistance < maxRadiusScanTargetSQR)
			{
				Vector3 popravochka = Vector3.zero;
				BoxCollider _collider = enemy.GetComponent<BoxCollider>();
				if (_collider != null)
				{
					popravochka = _collider.center;
				}
				Ray ray = new Ray(tower.position, enemy.position + popravochka - tower.position);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, maxRadiusScanTarget, Tools.AllWithoutDamageCollidersMask) && (hit.collider.gameObject == enemy.gameObject || (hit.collider.gameObject.transform.parent != null && (hit.collider.gameObject.transform.parent.Equals(enemy) || hit.collider.gameObject.transform.parent.Equals(enemy.parent)))))
				{
					closestTarget = targetDistance;
					closestTargetObj = enemy.gameObject;
				}
			}
			yield return null;
		}
		if (closestTargetObj != null)
		{
			target = closestTargetObj.transform;
		}
		else
		{
			target = null;
		}
		inScaning = false;
	}

	protected override void TargetUpdate()
	{
		base.TargetUpdate();
		if (!isPlayMusicDater)
		{
			PlayMusic(true);
			if (!Defs.isInet)
			{
				_networkView.RPC("PlayMusic", RPCMode.Others, true);
			}
			else
			{
				photonView.RPC("PlayMusic", PhotonTargets.Others, true);
			}
		}
	}

	protected override void UpdateTurret()
	{
		base.UpdateTurret();
		if (isPlayMusicDater)
		{
			tower.Rotate(new Vector3(0f, 0f, 180f * Time.deltaTime));
			gun.Rotate(new Vector3(180f * Time.deltaTime, 0f, 0f));
		}
	}

	[RPC]
	[PunRPC]
	private void PlayMusic(bool isPlay)
	{
		if (isPlayMusicDater == isPlay)
		{
			return;
		}
		isPlayMusicDater = isPlay;
		if (isPlay)
		{
			if (Defs.isSoundFX)
			{
				GetComponent<AudioSource>().loop = true;
				GetComponent<AudioSource>().clip = musicDater;
				GetComponent<AudioSource>().Play();
			}
		}
		else
		{
			GetComponent<AudioSource>().Stop();
		}
	}

	protected override void PlayerConnectedLocal(NetworkPlayer player)
	{
		base.PlayerConnectedLocal(player);
		_networkView.RPC("PlayMusic", player, isPlayMusicDater);
	}

	protected override void PlayerConnectedPhoton(PhotonPlayer player)
	{
		base.PlayerConnectedPhoton(player);
		photonView.RPC("PlayMusic", player, isPlayMusicDater);
	}
}

using System.Collections.Generic;
using UnityEngine;

public sealed class AdvancedEffects : MonoBehaviour
{
	public enum AdvancedEffect
	{
		none,
		burning
	}

	public struct ActiveAdvancedEffect
	{
		public AdvancedEffect effect;

		public float lifeTime;

		public ActiveAdvancedEffect(AdvancedEffect effect, float time)
		{
			this.effect = effect;
			lifeTime = Time.time + time;
		}

		public ActiveAdvancedEffect UpdateTime(float time)
		{
			return new ActiveAdvancedEffect(effect, time);
		}
	}

	public bool syncInLocal;

	private bool isMine;

	private PhotonView _photonView;

	private NetworkView _networkView;

	private List<ActiveAdvancedEffect> playerEffects = new List<ActiveAdvancedEffect>(3);

	private GameObject burningEffect;

	private GameObject bleedingEffect;

	private void Awake()
	{
		_photonView = GetComponent<PhotonView>();
		if (syncInLocal)
		{
			_networkView = GetComponent<NetworkView>();
		}
		isMine = !Defs.isMulti || _photonView == null || (Defs.isInet ? _photonView.isMine : (syncInLocal && _networkView.isMine));
	}

	public void SendAdvancedEffect(int effectIndex, float effectTime)
	{
		if (Defs.isMulti)
		{
			if (Defs.isInet)
			{
				_photonView.RPC("AdvancedEffectRPC", PhotonTargets.Others, effectIndex, effectTime);
			}
			else if (syncInLocal)
			{
				_networkView.RPC("AdvancedEffectRPC", RPCMode.Others, effectIndex, effectTime);
			}
		}
		AdvancedEffectRPC(effectIndex, effectTime);
	}

	[PunRPC]
	[RPC]
	public void AdvancedEffectRPC(int effectIndex, float effectTime)
	{
		for (int i = 0; i < playerEffects.Count; i++)
		{
			if (playerEffects[i].effect == (AdvancedEffect)effectIndex)
			{
				playerEffects[i] = playerEffects[i].UpdateTime(effectTime);
				return;
			}
		}
		playerEffects.Add(new ActiveAdvancedEffect((AdvancedEffect)effectIndex, effectTime));
		ActivateAdvancedEffect((AdvancedEffect)effectIndex);
	}

	private float GetCenterPosition()
	{
		if (base.transform.childCount > 0)
		{
			BoxCollider component = base.transform.GetChild(0).GetComponent<BoxCollider>();
			if (component != null)
			{
				return component.center.y;
			}
		}
		return 0f;
	}

	private void ActivateAdvancedEffect(AdvancedEffect effect)
	{
		if (effect == AdvancedEffect.burning)
		{
			burningEffect = ParticleStacks.instance.fireStack.GetParticle();
			if (burningEffect != null)
			{
				burningEffect.transform.SetParent(base.transform, false);
				burningEffect.transform.localPosition = Vector3.up * GetCenterPosition();
			}
		}
	}

	private void DeactivateAdvancedEffect(AdvancedEffect effect)
	{
		if (effect == AdvancedEffect.burning && burningEffect != null && ParticleStacks.instance != null)
		{
			burningEffect.transform.parent = null;
			ParticleStacks.instance.fireStack.ReturnParticle(burningEffect);
			burningEffect = null;
		}
	}

	private void Update()
	{
		for (int i = 0; i < playerEffects.Count; i++)
		{
			if (playerEffects[i].lifeTime < Time.time)
			{
				DeactivateAdvancedEffect(playerEffects[i].effect);
				playerEffects.RemoveAt(i);
				i--;
			}
		}
	}

	private void OnDestroy()
	{
		int num;
		for (num = 0; num < playerEffects.Count; num++)
		{
			DeactivateAdvancedEffect(playerEffects[num].effect);
			playerEffects.RemoveAt(num);
			num--;
		}
	}
}

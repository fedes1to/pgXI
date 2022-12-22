using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponAnimParticleEffects : MonoBehaviour
{
	public WeaponAnimEffectData[] effects;

	private List<GameObject> _eo;

	[ReadOnly]
	[SerializeField]
	private WeaponAnimEffectData _currentEffect;

	[ReadOnly]
	[SerializeField]
	private string _lastStartedAnimationName;

	[SerializeField]
	[ReadOnly]
	private string _lastFinishedAnimationName;

	private bool _isInit;

	private bool _isCanStopNotLoopEffect;

	private List<GameObject> _effectObjects
	{
		get
		{
			if (_eo == null)
			{
				_eo = new List<GameObject>();
				WeaponAnimEffectData[] array = effects;
				foreach (WeaponAnimEffectData weaponAnimEffectData in array)
				{
					ParticleSystem[] particleSystems = weaponAnimEffectData.particleSystems;
					foreach (ParticleSystem particleSystem in particleSystems)
					{
						_eo.Add(particleSystem.gameObject);
					}
				}
			}
			return _eo;
		}
	}

	private void Start()
	{
		if (!_isInit)
		{
			WeaponAnimEffectData[] array = effects;
			foreach (WeaponAnimEffectData effectData in array)
			{
				InitiAnimatonEventForEffect(effectData);
			}
			_isInit = true;
		}
		ActivateDefaultEffect();
	}

	private void OnEnable()
	{
		ActivateDefaultEffect();
	}

	public List<GameObject> GetListAnimEffects()
	{
		return _effectObjects;
	}

	private void InitiAnimatonEventForEffect(WeaponAnimEffectData effectData)
	{
		AnimationClip animationClip = GetComponent<Animation>().GetClip(effectData.animationName);
		if (!(animationClip == null))
		{
			if (!animationClip.events.Any((AnimationEvent e) => e.stringParameter == effectData.animationName && e.functionName == "OnStartAnimEffects" && Math.Abs(e.time) < 0.001f))
			{
				AnimationEvent animationEvent = new AnimationEvent();
				animationEvent.stringParameter = effectData.animationName;
				animationEvent.functionName = "OnStartAnimEffects";
				animationEvent.time = 0f;
				animationClip.AddEvent(animationEvent);
			}
			if (!animationClip.events.Any((AnimationEvent e) => e.stringParameter == effectData.animationName && e.functionName == "OnAnimationFinished" && Math.Abs(e.time - animationClip.length) < 0.001f))
			{
				AnimationEvent animationEvent2 = new AnimationEvent();
				animationEvent2.stringParameter = effectData.animationName;
				animationEvent2.functionName = "OnAnimationFinished";
				animationEvent2.time = animationClip.length;
				animationClip.AddEvent(animationEvent2);
			}
			effectData.animationLength = ((!effectData.isLoop) ? animationClip.length : 0f);
		}
	}

	public void ActivateDefaultEffect()
	{
		WeaponAnimEffectData weaponAnimEffectData = effects.FirstOrDefault((WeaponAnimEffectData e) => e.animationName == "Default");
		if (weaponAnimEffectData != null)
		{
			SetActiveEffect(weaponAnimEffectData, true);
		}
	}

	public void DisableEffectForAnimation(string animName)
	{
		WeaponAnimEffectData effectData = GetEffectData(animName);
		if (effectData != null)
		{
			effectData.isPlaying = false;
			SetActiveEffect(effectData, false);
			_currentEffect = null;
		}
	}

	private void SetActiveEffect(WeaponAnimEffectData effectData, bool active)
	{
		if (effectData == null || effectData.particleSystems == null || (active && effectData.blockAtPlay && effectData.isPlaying))
		{
			return;
		}
		ParticleSystem[] particleSystems = effectData.particleSystems;
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			if (active)
			{
				particleSystem.gameObject.SetActive(true);
				if (effectData.EmitCount < 0)
				{
					particleSystem.Play();
				}
				else
				{
					particleSystem.Emit(effectData.EmitCount);
				}
			}
			else if (effectData.EmitCount < 0)
			{
				particleSystem.gameObject.SetActive(false);
			}
		}
	}

	private WeaponAnimEffectData GetEffectData(string animationName)
	{
		if (effects == null)
		{
			return null;
		}
		int num = effects.Length;
		for (int i = 0; i < num; i++)
		{
			WeaponAnimEffectData weaponAnimEffectData = effects[i];
			if (weaponAnimEffectData != null && weaponAnimEffectData.animationName == animationName)
			{
				return weaponAnimEffectData;
			}
		}
		return null;
	}

	private bool CheckSkipStartEffectForAnimation(string animationName)
	{
		if (_currentEffect == null)
		{
			return false;
		}
		if (_currentEffect.isLoop)
		{
			return _lastStartedAnimationName == animationName;
		}
		WeaponAnimEffectData effectData = GetEffectData(animationName);
		if (effectData == null)
		{
			return false;
		}
		if (effectData != null && !effectData.isLoop)
		{
			CancelInvoke("ChangeEffectAfterStopAnimation");
			return false;
		}
		return !_isCanStopNotLoopEffect;
	}

	private void OnStartAnimEffects(string animationName)
	{
		if (CheckSkipStartEffectForAnimation(animationName))
		{
			return;
		}
		_lastStartedAnimationName = animationName;
		WeaponAnimEffectData effectData = GetEffectData(animationName);
		if (effectData == null)
		{
			return;
		}
		bool flag = false;
		if (_currentEffect != null)
		{
			flag = _currentEffect.particleSystems.SequenceEqual(effectData.particleSystems) && _currentEffect.isLoop && effectData.isLoop;
			if (!flag)
			{
				SetActiveEffect(_currentEffect, false);
			}
		}
		_currentEffect = effectData;
		if (effectData != null)
		{
			if (!flag)
			{
				SetActiveEffect(effectData, true);
				effectData.isPlaying = true;
			}
			if (!effectData.isLoop)
			{
				_isCanStopNotLoopEffect = false;
				Invoke("ChangeEffectAfterStopAnimation", effectData.animationLength);
			}
		}
	}

	private void OnAnimationFinished(string animationName)
	{
		WeaponAnimEffectData effectData = GetEffectData(animationName);
		_lastFinishedAnimationName = animationName;
		if (effectData != null)
		{
			effectData.isPlaying = false;
		}
	}

	private string GetNamePlayingAnimation()
	{
		if (GetComponent<Animation>() == null)
		{
			return string.Empty;
		}
		foreach (AnimationState item in GetComponent<Animation>())
		{
			if (GetComponent<Animation>().IsPlaying(item.name))
			{
				return item.name;
			}
		}
		return string.Empty;
	}

	public void ChangeEffectAfterStopAnimation()
	{
		_isCanStopNotLoopEffect = true;
		string namePlayingAnimation = GetNamePlayingAnimation();
		if (!string.IsNullOrEmpty(namePlayingAnimation))
		{
			OnStartAnimEffects(namePlayingAnimation);
		}
	}
}

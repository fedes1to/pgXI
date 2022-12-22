using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using UnityEngine;

public class CharacterViewRotator : MonoBehaviour
{
	public Transform characterView;

	private Quaternion _defaultLocalRotation;

	private float _toDefaultOrientationTime;

	private float _lastRotateTime;

	private List<Tweener> m_currentReturnTweeners = new List<Tweener>();

	public void SetDefaultRotationFromCharacterView()
	{
		_defaultLocalRotation = characterView.localRotation;
	}

	private void Awake()
	{
		if (characterView != null)
		{
			SetDefaultRotationFromCharacterView();
		}
	}

	private void Start()
	{
		if (!(characterView == null))
		{
			ReturnCharacterToDefaultOrientation();
		}
	}

	private void OnEnable()
	{
		if (!(characterView == null))
		{
			ReturnCharacterToDefaultOrientation();
		}
	}

	private void Update()
	{
		if (!(characterView == null) && Time.realtimeSinceStartup > _toDefaultOrientationTime)
		{
			ReturnCharacterToDefaultOrientation();
		}
	}

	private void OnDragStart()
	{
		if (!(characterView == null))
		{
			_lastRotateTime = Time.realtimeSinceStartup;
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (!(characterView == null) && !HOTween.IsTweening(characterView))
		{
			RefreshToDefaultOrientationTime();
			float num = -30f;
			characterView.Rotate(Vector3.up, delta.x * num * (Time.realtimeSinceStartup - _lastRotateTime));
			_lastRotateTime = Time.realtimeSinceStartup;
		}
	}

	private void OnScroll(float delta)
	{
		OnDrag(new Vector2((0f - delta) * 20f, 0f));
	}

	private void RefreshToDefaultOrientationTime()
	{
		_toDefaultOrientationTime = Time.realtimeSinceStartup + ShopNGUIController.IdleTimeoutPers;
	}

	private void ReturnCharacterToDefaultOrientation()
	{
		if (characterView == null)
		{
			return;
		}
		foreach (Tweener currentReturnTweener in m_currentReturnTweeners)
		{
			HOTween.Kill(currentReturnTweener);
		}
		m_currentReturnTweeners.Clear();
		RefreshToDefaultOrientationTime();
		TweenParms p_parms = new TweenParms().Prop("localRotation", new PlugQuaternion(_defaultLocalRotation)).UpdateType(UpdateType.TimeScaleIndependentUpdate).Ease(EaseType.Linear)
			.OnComplete((TweenDelegate.TweenCallback)delegate
			{
				RefreshToDefaultOrientationTime();
			});
		Tweener item = HOTween.To(characterView, 0.5f, p_parms);
		m_currentReturnTweeners.Add(item);
	}
}

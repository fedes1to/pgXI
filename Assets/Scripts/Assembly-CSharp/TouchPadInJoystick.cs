using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft.MiniJson;
using UnityEngine;

public class TouchPadInJoystick : MonoBehaviour
{
	public Transform fireSprite;

	public bool isShooting;

	public bool isJumpPressed;

	public InGameGUI inGameGUI;

	public bool isActiveFireButton;

	private Rect _fireRect = default(Rect);

	private bool _shouldRecalcRects;

	private bool _isFirstFrame = true;

	private HungerGameController _hungerGameController;

	private bool _joyActive = true;

	private Player_move_c _playerMoveC;

	private bool pressured;

	private IEnumerator ReCalcRects()
	{
		yield return null;
		yield return null;
		CalcRects();
	}

	public void SetJoystickActive(bool active)
	{
		_joyActive = active;
		if (!active)
		{
			isShooting = false;
			isJumpPressed = false;
		}
	}

	private void OnEnable()
	{
		isShooting = false;
		if (_shouldRecalcRects)
		{
			StartCoroutine(ReCalcRects());
		}
		_shouldRecalcRects = false;
		StartCoroutine(_SetIsFirstFrame());
	}

	private IEnumerator _SetIsFirstFrame()
	{
		float tm = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (Time.realtimeSinceStartup - tm < 0.1f);
		_isFirstFrame = false;
	}

	private IEnumerator Start()
	{
		if (Defs.isHunger)
		{
			_hungerGameController = GameObject.FindGameObjectWithTag("HungerGameController").GetComponent<HungerGameController>();
		}
		PauseNGUIController.PlayerHandUpdated += SetSideAndCalcRects;
		ControlsSettingsBase.ControlsChanged += SetShouldRecalcRects;
		yield return null;
		yield return null;
		CalcRects();
	}

	private void SetSideAndCalcRects()
	{
		SetShouldRecalcRects();
	}

	private void SetShouldRecalcRects()
	{
		_shouldRecalcRects = true;
	}

	private bool IsActiveFireButton()
	{
		if ((!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None) || Defs.isTurretWeapon)
		{
			return false;
		}
		if (Defs.gameSecondFireButtonMode == Defs.GameSecondFireButtonMode.On)
		{
			return true;
		}
		if (Defs.gameSecondFireButtonMode == Defs.GameSecondFireButtonMode.Sniper && _playerMoveC != null && _playerMoveC.isZooming)
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (_playerMoveC == null)
		{
			if (Defs.isMulti && WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayer != null)
			{
				_playerMoveC = WeaponManager.sharedManager.myPlayerMoveC;
			}
			else
			{
				GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
				if (gameObject != null)
				{
					_playerMoveC = gameObject.GetComponent<SkinName>().playerMoveC;
				}
			}
		}
		if (!_joyActive)
		{
			isShooting = false;
			isJumpPressed = false;
			return;
		}
		isActiveFireButton = IsActiveFireButton();
		bool flag = isActiveFireButton && (!Defs.isHunger || _hungerGameController.isGo);
		if (flag != fireSprite.gameObject.activeSelf)
		{
			fireSprite.gameObject.SetActive(flag);
		}
	}

	private void OnPressure(float pressure)
	{
		if (Defs.touchPressureSupported && Defs.isUseJump3DTouch && pressure > Defs.touchPressurePower)
		{
			if (!pressured)
			{
				pressured = true;
				isJumpPressed = true;
				if (TrainingController.sharedController != null)
				{
					TrainingController.sharedController.Hide3dTouchJump();
				}
			}
		}
		else
		{
			pressured = false;
			isJumpPressed = false;
		}
	}

	private void OnPress(bool isDown)
	{
		if (!_joyActive || inGameGUI.playerMoveC == null)
		{
			return;
		}
		if (_fireRect.width.Equals(0f))
		{
			CalcRects();
		}
		if (!_isFirstFrame)
		{
			if (isDown && _fireRect.Contains(UICamera.lastTouchPosition) && fireSprite.gameObject.activeSelf)
			{
				isShooting = true;
			}
			if (!isDown)
			{
				isShooting = false;
				pressured = false;
				isJumpPressed = false;
			}
		}
	}

	private void CalcRects()
	{
		Transform transform = NGUITools.GetRoot(base.gameObject).transform;
		Camera component = transform.GetChild(0).GetChild(0).GetComponent<Camera>();
		Transform relativeTo = component.transform;
		float num = 768f;
		float num2 = num * ((float)Screen.width / (float)Screen.height);
		List<object> list = Json.Deserialize(PlayerPrefs.GetString("Controls.Size", "[]")) as List<object>;
		if (list == null)
		{
			list = new List<object>();
			Debug.LogWarning(list.GetType().FullName);
		}
		int[] array = list.Select(Convert.ToInt32).ToArray();
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, fireSprite, true);
		float num3 = 60f;
		if (array.Length > 6)
		{
			num3 = (float)array[6] * 0.5f;
		}
		bounds.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
		_fireRect = new Rect((bounds.center.x - num3) * Defs.Coef, (bounds.center.y - num3) * Defs.Coef, 2f * num3 * Defs.Coef, 2f * num3 * Defs.Coef);
	}

	private void OnDestroy()
	{
		PauseNGUIController.PlayerHandUpdated -= SetSideAndCalcRects;
		ControlsSettingsBase.ControlsChanged -= SetShouldRecalcRects;
	}
}

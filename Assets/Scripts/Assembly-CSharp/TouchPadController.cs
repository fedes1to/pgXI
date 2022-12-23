using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public class TouchPadController : MonoBehaviour
{
	public const string GRENADE_BUY_NORMAL_SPRITE_NAME = "grenade_btn";

	private const string GRENADE_BUY_PRESSED_SPRITE_NAME = "grenade_btn_n";

	public const string LIKE_BUY_NORMAL_SPRITE_NAME = "grenade_like_btn";

	private const string LIKE_BUY_PRESSED_SPRITE_NAME = "grenade_like_btn_n";

	public static int thresholdGadgetPanel = 70;

	public static float timeGadgetPanel = 0.5f;

	public ChooseGadgetPanel chooseGadgetPanel;

	public GrenadeButton grenadeButton;

	public bool grenadePressed;

	public bool jumpPressed;

	public Transform fireSprite;

	public Transform jumpSprite;

	public Transform reloadSpirte;

	public Transform zoomSprite;

	public bool hasAmmo = true;

	public bool _isFirstFrame = true;

	public GameObject jetPackIcon;

	public GameObject jumpIcon;

	public GameObject cantUseGadget;

	private Rect grenadeRect = default(Rect);

	private Rect availableGadget1Rect = default(Rect);

	private Rect availableGadget2Rect = default(Rect);

	private bool isInvokeGrenadePress;

	private bool chooseGadgetPanelShown;

	private bool gadgetsPanelPress;

	private Vector2 _initialGrenadePressPosition;

	private UISprite reloadUISprite;

	public bool isShooting;

	public bool isShootingPressure;

	private bool isHunger;

	private Player_move_c move;

	private HungerGameController hungerGameController;

	private Rect fireRect = default(Rect);

	private Rect jumpRect = default(Rect);

	private Rect reloadRect;

	private Rect zoomRect;

	private Rect moveRect;

	private bool _joyActive = true;

	private bool _isBuyGrenadePressed;

	private CameraTouchControlScheme _touchControlScheme;

	private bool _shouldRecalcRects;

	public bool gadgetPanelVisible;

	private float zoomAlpha = 1f;

	private float reloadAlpha = 1f;

	private float jumpAlpha = 1f;

	private float fireAlpha = 1f;

	private bool rectsCalculated;

	private int framesCount;

	private int selectedGadget;

	private int MoveTouchID = -1;

	private int gadgetsTouchID = -1;

	private bool firstDeltaSkip;

	private Vector2 pastPos = Vector2.zero;

	private Vector2 pastDelta = Vector2.zero;

	private Vector2 compDeltas = Vector2.zero;

	private bool m_shouldHideGadgetPanel;

	private float m_hideGadgetsPanelSettedTime;

	public CameraTouchControlScheme touchControlScheme
	{
		get
		{
			return _touchControlScheme;
		}
		set
		{
			_touchControlScheme = value;
			_touchControlScheme.Reset();
		}
	}

	public void MakeInactive()
	{
		jumpPressed = false;
		isShooting = false;
		isShootingPressure = false;
		Reset();
		HasAmmo();
		_joyActive = false;
	}

	public void MakeActive()
	{
		_joyActive = true;
	}

	private void Awake()
	{
		ChooseGadgetPanel.OnDisablePanel += ChooseGadgetPanel_OnDisablePanel;
		AdjustGadgetPanelVisibility();
		reloadUISprite = reloadSpirte.GetComponent<UISprite>();
		isHunger = Defs.isHunger;
		if (!Device.isPixelGunLow)
		{
			_touchControlScheme = new CameraTouchControlScheme_CleanNGUI();
		}
		else if (Defs.isTouchControlSmoothDump)
		{
			_touchControlScheme = new CameraTouchControlScheme_SmoothDump();
		}
		else
		{
			_touchControlScheme = new CameraTouchControlScheme_CleanNGUI();
		}
	}

	private void ChooseGadgetPanel_OnDisablePanel()
	{
		if (chooseGadgetPanelShown)
		{
			HideGadgetsPanel();
			m_shouldHideGadgetPanel = false;
		}
		grenadePressed = false;
		isInvokeGrenadePress = false;
	}

	private void OnEnable()
	{
		isShooting = false;
		isShootingPressure = false;
		if (_shouldRecalcRects)
		{
			Invoke("ReCalcRects", 0.1f);
		}
		_shouldRecalcRects = false;
		StartCoroutine(_SetIsFirstFrame());
	}

	public static int GetGrenadeCount()
	{
		if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			return WeaponManager.sharedManager.myPlayerMoveC.GrenadeCount;
		}
		return 0;
	}

	private static bool IsButtonGrenadeVisible()
	{
		return ((InGameGUI.sharedInGameGUI.playerMoveC != null && !InGameGUI.sharedInGameGUI.playerMoveC.isMechActive && !Defs.isTurretWeapon) || InGameGUI.sharedInGameGUI.playerMoveC == null) && !Defs.isZooming;
	}

	private bool IsUseGrenadeActive()
	{
		return IsButtonGrenadeVisible() && Defs.isGrenateFireEnable && (!isHunger || hungerGameController.isGo) && (!Defs.isDaterRegim || GetGrenadeCount() > 0) && WeaponManager.sharedManager._currentFilterMap != 1 && WeaponManager.sharedManager._currentFilterMap != 2 && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None);
	}

	public static bool IsBuyGrenadeActive()
	{
		if (IsButtonGrenadeVisible() && Defs.isDaterRegim && WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null && WeaponManager.sharedManager.myPlayerMoveC.GrenadeCount <= 0)
		{
			return true;
		}
		return false;
	}

	private void SetSpritesState()
	{
		SetGrenadeUISpriteState();
		if (WeaponManager.sharedManager != null)
		{
			string text = WeaponManager.sharedManager.currentWeaponSounds.gameObject.name;
			if (!Defs.isTurretWeapon && !text.Contains("Weapon"))
			{
				return;
			}
		}
		jumpSprite.gameObject.SetActive((Defs.isJumpAndShootButtonOn || !Defs.isUseJump3DTouch) && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != 0 || TrainingController.stepTraining >= TrainingState.GetTheGun));
		bool flag = TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None || TrainingController.FireButtonEnabled;
		fireSprite.gameObject.SetActive((Defs.isJumpAndShootButtonOn || !Defs.isUseShoot3DTouch) && !Defs.isTurretWeapon && flag && (!isHunger || hungerGameController.isGo) && !WeaponManager.sharedManager.currentWeaponSounds.isGrenadeWeapon);
		reloadSpirte.gameObject.SetActive(((InGameGUI.sharedInGameGUI.playerMoveC != null && !InGameGUI.sharedInGameGUI.playerMoveC.isMechActive) || InGameGUI.sharedInGameGUI.playerMoveC == null) && !Defs.isTurretWeapon && flag && WeaponManager.sharedManager != null && WeaponManager.sharedManager.currentWeaponSounds != null && !WeaponManager.sharedManager.currentWeaponSounds.isMelee && !WeaponManager.sharedManager.currentWeaponSounds.isShotMelee);
		zoomSprite.gameObject.SetActive(((InGameGUI.sharedInGameGUI.playerMoveC != null && !InGameGUI.sharedInGameGUI.playerMoveC.isMechActive) || InGameGUI.sharedInGameGUI.playerMoveC == null) && !Defs.isTurretWeapon && flag && WeaponManager.sharedManager != null && WeaponManager.sharedManager.currentWeaponSounds != null && WeaponManager.sharedManager.currentWeaponSounds.isZooming);
		if (jumpIcon.activeSelf == Defs.isJetpackEnabled)
		{
			jumpIcon.SetActive(!Defs.isJetpackEnabled);
		}
		if (jetPackIcon.activeSelf != Defs.isJetpackEnabled)
		{
			jetPackIcon.SetActive(Defs.isJetpackEnabled);
		}
	}

	private void SetGrenadeUISpriteState()
	{
		if (!Defs.isDaterRegim)
		{
			return;
		}
		grenadeButton.gameObject.SetActiveSafeSelf(true);
		chooseGadgetPanel.gameObject.SetActiveSafeSelf(false);
		bool flag = IsBuyGrenadeActive();
		bool flag2 = IsUseGrenadeActive();
		grenadeButton.gameObject.SetActiveSafeSelf(flag || flag2);
		if (!grenadeButton.gameObject.activeSelf)
		{
			return;
		}
		grenadeButton.grenadeSprite.spriteName = (((grenadePressed || _isBuyGrenadePressed) && grenadeRect.Contains(UICamera.lastTouchPosition)) ? ((!Defs.isDaterRegim) ? "grenade_btn_n" : "grenade_like_btn_n") : ((!Defs.isDaterRegim) ? "grenade_btn" : "grenade_like_btn"));
		if (flag)
		{
			if (Defs.isDaterRegim)
			{
				grenadeButton.priceLabel.gameObject.SetActiveSafeSelf(true);
				grenadeButton.countLabel.gameObject.SetActiveSafeSelf(false);
				grenadeButton.fullLabel.gameObject.SetActiveSafeSelf(false);
			}
			else
			{
				grenadeButton.gameObject.SetActiveSafeSelf(false);
			}
		}
		else
		{
			grenadeButton.gameObject.SetActiveSafeSelf(true);
			grenadeButton.priceLabel.gameObject.SetActiveSafeSelf(false);
			int grenadeCount = GetGrenadeCount();
			grenadeButton.countLabel.gameObject.SetActiveSafeSelf(true);
			grenadeButton.countLabel.text = grenadeCount.ToString();
			grenadeButton.fullLabel.gameObject.SetActiveSafeSelf(false);
		}
	}

	private void SetSide()
	{
		bool flag = (GetComponent<UIAnchor>().side == UIAnchor.Side.BottomRight && GlobalGameController.LeftHanded) || (GetComponent<UIAnchor>().side == UIAnchor.Side.BottomLeft && !GlobalGameController.LeftHanded);
		GetComponent<UIAnchor>().side = (GlobalGameController.LeftHanded ? UIAnchor.Side.BottomRight : UIAnchor.Side.BottomLeft);
		Vector3 center = GetComponent<BoxCollider>().center;
		center.x *= ((!flag) ? (-1f) : 1f);
		GetComponent<BoxCollider>().center = center;
		chooseGadgetPanel.transform.localScale = new Vector3(GlobalGameController.LeftHanded ? 1 : (-1), 1f, 1f);
		for (int i = 0; i < chooseGadgetPanel.objectsNoFilpXScale.Count; i++)
		{
			chooseGadgetPanel.objectsNoFilpXScale[i].localScale = new Vector3(GlobalGameController.LeftHanded ? 1 : (-1), chooseGadgetPanel.objectsNoFilpXScale[i].localScale.y, chooseGadgetPanel.objectsNoFilpXScale[i].localScale.z);
		}
	}

	private void SetSideAndCalcRects()
	{
		SetSide();
		SetShouldRecalcRects();
	}

	private void SetShouldRecalcRects()
	{
		_shouldRecalcRects = true;
	}

	[Obfuscation(Exclude = true)]
	private void ReCalcRects()
	{
		CalcRects(true);
	}

	private IEnumerator Start()
	{
		SetSide();
		PauseNGUIController.PlayerHandUpdated += SetSideAndCalcRects;
		ControlsSettingsBase.ControlsChanged += SetShouldRecalcRects;
		if (isHunger)
		{
			hungerGameController = GameObject.FindGameObjectWithTag("HungerGameController").GetComponent<HungerGameController>();
		}
		SetSpritesState();
		yield return null;
		CalcRects(false);
		Reset();
	}

	public void Reset()
	{
		_touchControlScheme.Reset();
	}

	public void HideButtonsOnGadgetPanel()
	{
		AlphaUpdate();
		if (!gadgetPanelVisible)
		{
			fireAlpha = 1f;
			jumpAlpha = 1f;
			reloadAlpha = 1f;
			zoomAlpha = 1f;
			return;
		}
		Transform transform = NGUITools.GetRoot(base.gameObject).transform;
		Camera component = transform.GetChild(0).GetChild(0).GetComponent<Camera>();
		Transform relativeTo = component.transform;
		float num = 768f;
		float num2 = num * ((float)Screen.width / (float)Screen.height);
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, chooseGadgetPanel.gadgetButtonScript.ContainerForScale, true);
		bounds.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
		Rect rect = new Rect((bounds.center.x - bounds.size.x / 2f) * Defs.Coef, (bounds.center.y - bounds.size.y / 2f) * Defs.Coef, bounds.size.x * Defs.Coef, bounds.size.y * Defs.Coef);
		fireAlpha = ((!rect.Overlaps(fireRect)) ? 1 : 0);
		jumpAlpha = ((!rect.Overlaps(jumpRect)) ? 1 : 0);
		reloadAlpha = ((!rect.Overlaps(reloadRect)) ? 1 : 0);
		zoomAlpha = ((!rect.Overlaps(zoomRect)) ? 1 : 0);
	}

	private void AlphaUpdate()
	{
		if (fireSprite.GetComponent<UISprite>().alpha != fireAlpha)
		{
			fireSprite.GetComponent<UISprite>().alpha = Mathf.MoveTowards(fireSprite.GetComponent<UISprite>().alpha, fireAlpha, Time.deltaTime * 7f);
		}
		if (jumpSprite.GetComponent<UISprite>().alpha != jumpAlpha)
		{
			jumpSprite.GetComponent<UISprite>().alpha = Mathf.MoveTowards(jumpSprite.GetComponent<UISprite>().alpha, jumpAlpha, Time.deltaTime * 7f);
		}
		if (reloadSpirte.GetComponent<UISprite>().alpha != reloadAlpha)
		{
			reloadSpirte.GetComponent<UISprite>().alpha = Mathf.MoveTowards(reloadSpirte.GetComponent<UISprite>().alpha, reloadAlpha, Time.deltaTime * 7f);
		}
		if (zoomSprite.GetComponent<UISprite>().alpha != zoomAlpha)
		{
			zoomSprite.GetComponent<UISprite>().alpha = Mathf.MoveTowards(zoomSprite.GetComponent<UISprite>().alpha, zoomAlpha, Time.deltaTime * 7f);
		}
	}

	private void CalcRects(bool forceRecalculation = false)
	{
		if (forceRecalculation)
		{
			rectsCalculated = false;
		}
		if (!rectsCalculated)
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
			float num3 = 62f;
			if (array.Length > 3)
			{
				num3 = (float)array[3] * 0.5f;
			}
			bounds.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
			fireRect = new Rect((bounds.center.x - num3) * Defs.Coef, (bounds.center.y - num3) * Defs.Coef, 2f * num3 * Defs.Coef, 2f * num3 * Defs.Coef);
			Bounds bounds2 = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, jumpSprite, true);
			bounds2.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
			float num4 = 62f;
			if (array.Length > 2)
			{
				num4 = (float)array[2] * 0.5f;
			}
			jumpRect = new Rect((bounds2.center.x - num4 * 0.7f) * Defs.Coef, (bounds2.center.y - num4) * Defs.Coef, 2f * num4 * Defs.Coef, 2f * num4 * Defs.Coef);
			Bounds bounds3 = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, reloadSpirte, true);
			float num5 = 55f;
			if (array.Length > 1)
			{
				num5 = (float)array[1] * 0.5f;
			}
			bounds3.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
			reloadRect = new Rect((bounds3.center.x - num5) * Defs.Coef, (bounds3.center.y - num5) * Defs.Coef, 2f * num5 * Defs.Coef, 2f * num5 * Defs.Coef);
			float num6 = 55f;
			if (array.Length > 0)
			{
				num6 = (float)array[0] * 0.5f;
			}
			Bounds bounds4 = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, zoomSprite, true);
			bounds4.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
			zoomRect = new Rect((bounds4.center.x - num6) * Defs.Coef, (bounds4.center.y - num6) * Defs.Coef, 2f * num6 * Defs.Coef, 2f * num6 * Defs.Coef);
			float num7 = 45f;
			if (array.Length > 5)
			{
				num7 = (float)array[5] * 0.5f;
			}
			float num8 = num7 * 2f / 90f;
			Bounds bounds5 = NGUIMath.CalculateRelativeWidgetBounds(relativeTo, chooseGadgetPanel.gadgetButtonScript.gadgetIcon.transform, true);
			bounds5.center += new Vector3(num2 * 0.5f, num * 0.5f, 0f);
			grenadeRect = new Rect((bounds5.center.x - num7) * Defs.Coef, (bounds5.center.y - num7) * Defs.Coef, 2f * num7 * Defs.Coef, 2f * num7 * Defs.Coef);
			availableGadget1Rect.Set(grenadeRect.x - chooseGadgetPanel.transform.localScale.x * (grenadeRect.width + 20f * num8 * Defs.Coef), grenadeRect.y, grenadeRect.width, grenadeRect.height);
			availableGadget2Rect.Set(grenadeRect.x - chooseGadgetPanel.transform.localScale.x * (grenadeRect.width + 20f * num8 * Defs.Coef) * 2f, grenadeRect.y, grenadeRect.width, grenadeRect.height);
			float num9 = (float)Screen.height * 0.81f;
			if (!GlobalGameController.LeftHanded)
			{
				moveRect = new Rect(0f, 0f, num9, (float)Screen.height * 0.65f);
			}
			else
			{
				moveRect = new Rect((float)Screen.width - num9, 0f, num9, (float)Screen.height * 0.65f);
			}
			rectsCalculated = !fireRect.width.Equals(0f) && Screen.width > Screen.height;
			Debug.Log("Control rects calculated. Success == " + rectsCalculated);
		}
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

	private void AdjustGadgetPanelVisibility()
	{
		bool flag = WeaponManager.sharedManager.myPlayerMoveC != null && WeaponManager.sharedManager.myPlayerMoveC.showGadgetsPanel;
		if (flag != chooseGadgetPanel.gameObject.activeSelf)
		{
			chooseGadgetPanel.gameObject.SetActiveSafeSelf(flag);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown("g"))
		{
			OnGadgetPanelClick(true);
			move.gadgeted = !move.gadgeted;
		}
		HideButtonsOnGadgetPanel();
		if (m_shouldHideGadgetPanel && Time.realtimeSinceStartup - 5f >= m_hideGadgetsPanelSettedTime)
		{
			HideGadgetsPanel();
			m_shouldHideGadgetPanel = false;
		}
		AdjustGadgetPanelVisibility();
		cantUseGadget.SetActive(WeaponManager.sharedManager.myPlayerMoveC != null && WeaponManager.sharedManager.myPlayerMoveC.gadgetsDisabled);
		if (chooseGadgetPanelShown)
		{
			chooseGadgetPanel.hoverBackground1.SetActiveSafeSelf(grenadeRect.Contains(UICamera.lastTouchPosition));
			chooseGadgetPanel.hoverBackground2.SetActiveSafeSelf(availableGadget1Rect.Contains(UICamera.lastTouchPosition));
			chooseGadgetPanel.hoverBackground3.SetActiveSafeSelf(availableGadget2Rect.Contains(UICamera.lastTouchPosition));
		}
		framesCount++;
		CalcRects(false);
		SetSpritesState();
		_isFirstFrame = false;
		if (!_joyActive)
		{
			jumpPressed = false;
			isShooting = false;
			isShootingPressure = false;
			_touchControlScheme.Reset();
			return;
		}
		int touchCount = Input.touchCount;
		for (int i = 0; i < touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (gadgetsTouchID == -1)
			{
				if (touch.phase == TouchPhase.Began && grenadeRect.Contains(touch.position))
				{
					gadgetsTouchID = touch.fingerId;
					OnGadgetPanelClick(true, touch.position);
				}
			}
			else if (touch.fingerId == gadgetsTouchID)
			{
				if (!Defs.isDaterRegim && isInvokeGrenadePress && chooseGadgetPanel.CanExtend() && (touch.position - _initialGrenadePressPosition).sqrMagnitude > (float)(thresholdGadgetPanel * thresholdGadgetPanel) * Defs.Coef)
				{
					isInvokeGrenadePress = false;
					CancelInvoke("GrenadePressInvoke");
					chooseGadgetPanelShown = true;
					chooseGadgetPanel.Show();
				}
				if (touch.phase == TouchPhase.Ended)
				{
					gadgetsTouchID = -1;
					OnGadgetPanelClick(false);
				}
			}
			if (touch.phase == TouchPhase.Began && chooseGadgetPanelShown && ProcessPressOnGadgetsPanel(touch.position))
			{
				m_shouldHideGadgetPanel = false;
			}
			if (touch.phase == TouchPhase.Ended && chooseGadgetPanelShown)
			{
				bool flag = ProcessPressOnGadgetsPanel(touch.position);
				if (chooseGadgetPanelShown && !m_shouldHideGadgetPanel)
				{
					m_shouldHideGadgetPanel = true;
					m_hideGadgetsPanelSettedTime = Time.realtimeSinceStartup;
				}
			}
		}
		_touchControlScheme.OnUpdate();
		touchCount = Input.touchCount;
		if (touchCount > 0)
		{
			if (MoveTouchID == -1)
			{
				for (int j = 0; j < touchCount; j++)
				{
					Touch touch2 = Input.GetTouch(j);
					if (touch2.phase == TouchPhase.Began && moveRect.Contains(touch2.position))
					{
						MoveTouchID = touch2.fingerId;
					}
				}
			}
			if (!gadgetsPanelPress)
			{
				UpdateMoveTouch();
			}
		}
		else
		{
			MoveTouchID = -1;
			pastPos = Vector2.zero;
			pastDelta = Vector2.zero;
		}
	}

	private void UpdateMoveTouch()
	{
		int touchCount = Input.touchCount;
		for (int i = 0; i < touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.fingerId == MoveTouchID)
			{
				if (touch.phase == TouchPhase.Ended)
				{
					MoveTouchID = -1;
					pastPos = Vector2.zero;
					pastDelta = Vector2.zero;
					firstDeltaSkip = false;
					break;
				}
				if (pastPos == Vector2.zero)
				{
					pastPos = touch.position;
				}
				Vector2 vector = touch.position - pastPos;
				if (vector == Vector2.zero && pastDelta != Vector2.zero && IsDifferendDirections(pastDelta, JoystickController.leftJoystick.value) && pastDelta.sqrMagnitude > 15f)
				{
					vector = pastDelta / 2f;
					compDeltas += vector;
				}
				else
				{
					vector -= compDeltas;
					compDeltas = Vector2.zero;
				}
				if (firstDeltaSkip)
				{
					OnDragTouch(vector);
				}
				else if (vector.sqrMagnitude > float.Epsilon)
				{
					firstDeltaSkip = true;
				}
				pastPos = touch.position;
				pastDelta = vector;
			}
		}
	}

	private void OnGadgetPanelClick(bool pressed, [Optional] Vector3 clickPos)
	{
		if (pressed)
		{
			if (chooseGadgetPanelShown)
			{
				return;
			}
			if (IsBuyGrenadeActive())
			{
				BuyGrenadePressInvoke();
			}
			else
			{
				if (InGameGUI.sharedInGameGUI != null && InGameGUI.sharedInGameGUI.changeWeaponScroll != null && InGameGUI.sharedInGameGUI.changeWeaponScroll.isDragging)
				{
					return;
				}
				if (Defs.isDaterRegim)
				{
					if (IsUseGrenadeActive())
					{
						GrenadePressInvoke();
					}
				}
				else if (!grenadePressed && WeaponManager.sharedManager.myPlayerMoveC.canUseGadgets)
				{
					_initialGrenadePressPosition = clickPos;
					isInvokeGrenadePress = true;
					gadgetsPanelPress = true;
					Invoke("GrenadePressInvoke", timeGadgetPanel);
				}
			}
		}
		else
		{
			if (!grenadePressed)
			{
				return;
			}
			grenadePressed = false;
			if (Defs.isDaterRegim)
			{
				grenadeButton.grenadeSprite.spriteName = ((!Defs.isDaterRegim) ? "grenade_btn" : "grenade_like_btn");
				move.GrenadeFire();
				return;
			}
			Gadget value = null;
			if (InGameGadgetSet.CurrentSet.TryGetValue(GadgetsInfo.DefaultGadget, out value) && value.CanUse)
			{
				value.Use();
			}
		}
	}

	private bool IsDifferendDirections(Vector2 delta1, Vector2 delta2)
	{
		return JoystickController.leftJoystick.value != Vector2.zero && Mathf.Sign(delta1.x) != Mathf.Sign(delta2.x);
	}

	public void HasAmmo()
	{
		BlinkReloadButton.isBlink = false;
	}

	public void NoAmmo()
	{
		BlinkReloadButton.isBlink = true;
	}

	[Obfuscation(Exclude = true)]
	private IEnumerator BlinkReload()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			reloadUISprite.spriteName = "Reload_0";
			yield return new WaitForSeconds(0.5f);
			reloadUISprite.spriteName = "Reload_1";
		}
	}

	private bool ProcessPressOnGadgetsPanel(Vector2 pos)
	{
		if (grenadeRect.Contains(pos))
		{
			chooseGadgetPanel.ChooseDefault();
			HideGadgetsPanel();
			return true;
		}
		if (availableGadget1Rect.Contains(pos))
		{
			chooseGadgetPanel.ChooseFirst();
			HideGadgetsPanel();
			return true;
		}
		if (availableGadget2Rect.Contains(pos))
		{
			chooseGadgetPanel.ChooseSecond();
			HideGadgetsPanel();
			return true;
		}
		return false;
	}

	private void HideGadgetsPanel()
	{
		chooseGadgetPanel.Hide();
		chooseGadgetPanelShown = false;
		chooseGadgetPanel.hoverBackground1.SetActiveSafeSelf(false);
		chooseGadgetPanel.hoverBackground2.SetActiveSafeSelf(false);
		chooseGadgetPanel.hoverBackground3.SetActiveSafeSelf(false);
	}

	private void OnPress(bool isDown)
	{
		_touchControlScheme.OnPress(isDown);
		if (!move)
		{
			if (!Defs.isMulti)
			{
				move = GameObject.FindGameObjectWithTag("Player").GetComponent<SkinName>().playerMoveC;
			}
			else
			{
				move = WeaponManager.sharedManager.myPlayerMoveC;
			}
		}
		CalcRects(false);
		if (!_joyActive || _isFirstFrame)
		{
			return;
		}
		if (isDown)
		{
		}
		if (isDown && fireRect.Contains(UICamera.lastTouchPosition) && (Defs.isJumpAndShootButtonOn || !Defs.isUseShoot3DTouch) && !gadgetsPanelPress && fireAlpha == 1f)
		{
			isShooting = true;
		}
		if (isDown && jumpRect.Contains(UICamera.lastTouchPosition) && (Defs.isJumpAndShootButtonOn || !Defs.isUseJump3DTouch) && !gadgetsPanelPress && jumpAlpha == 1f && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != 0 || TrainingController.stepTraining >= TrainingState.GetTheGun))
		{
			jumpPressed = true;
		}
		if (isDown && ((InGameGUI.sharedInGameGUI.playerMoveC != null && !InGameGUI.sharedInGameGUI.playerMoveC.isMechActive) || InGameGUI.sharedInGameGUI.playerMoveC == null) && reloadRect.Contains(UICamera.lastTouchPosition) && !gadgetsPanelPress && reloadAlpha == 1f && (bool)move && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None || TrainingController.FireButtonEnabled))
		{
			move.ReloadPressed();
		}
		bool flag = zoomSprite != null && zoomSprite.gameObject.activeInHierarchy;
		if (isDown && ((InGameGUI.sharedInGameGUI.playerMoveC != null && !InGameGUI.sharedInGameGUI.playerMoveC.isMechActive) || InGameGUI.sharedInGameGUI.playerMoveC == null) && flag && zoomRect.Contains(UICamera.lastTouchPosition) && !gadgetsPanelPress && zoomAlpha == 1f && (bool)move && WeaponManager.sharedManager != null && WeaponManager.sharedManager.currentWeaponSounds != null && WeaponManager.sharedManager.currentWeaponSounds.isZooming)
		{
			move.ZoomPress();
		}
		if (isDown)
		{
			return;
		}
		gadgetsPanelPress = false;
		if (isInvokeGrenadePress)
		{
			isInvokeGrenadePress = false;
			CancelInvoke("GrenadePressInvoke");
			Gadget value = null;
			if (InGameGadgetSet.CurrentSet.TryGetValue(GadgetsInfo.DefaultGadget, out value) && value.CanUse)
			{
				value.PreUse();
				value.Use();
			}
		}
		if (_isBuyGrenadePressed)
		{
			_isBuyGrenadePressed = false;
			grenadeButton.grenadeSprite.spriteName = ((!Defs.isDaterRegim) ? "grenade_btn" : "grenade_like_btn");
			if (grenadeRect.Contains(UICamera.lastTouchPosition))
			{
				InGameGUI.sharedInGameGUI.HandleBuyGrenadeClicked(null, EventArgs.Empty);
			}
		}
		isShooting = false;
		jumpPressed = false;
		isShootingPressure = false;
	}

	[Obfuscation(Exclude = true)]
	private void GrenadePressInvoke()
	{
		gadgetsPanelPress = false;
		Gadget value = null;
		bool flag = false;
		if (Defs.isDaterRegim)
		{
			grenadeButton.grenadeSprite.spriteName = "grenade_like_btn_n";
			value = WeaponManager.sharedManager.myPlayerMoveC.daterLikeGadget;
			flag = true;
		}
		else
		{
			InGameGadgetSet.CurrentSet.TryGetValue(GadgetsInfo.DefaultGadget, out value);
			flag = WeaponManager.sharedManager.myPlayerMoveC.canUseGadgets && value.CanUse;
		}
		if (value != null && flag)
		{
			isInvokeGrenadePress = false;
			grenadePressed = true;
			value.PreUse();
		}
	}

	private void BuyGrenadePressInvoke()
	{
		_isBuyGrenadePressed = true;
		grenadeButton.grenadeSprite.spriteName = "grenade_like_btn_n";
	}

	private void OnPressure(float pressure)
	{
		if (Defs.touchPressureSupported && Defs.isUseShoot3DTouch && WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			if ((TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None || TrainingController.FireButtonEnabled) && (!isHunger || hungerGameController.isGo) && hasAmmo && !WeaponManager.sharedManager.currentWeaponSounds.isGrenadeWeapon && pressure > Defs.touchPressurePower)
			{
				isShootingPressure = true;
			}
			else
			{
				isShootingPressure = false;
			}
		}
	}

	private void OnDragTouch(Vector2 delta)
	{
		if (!_joyActive)
		{
			jumpPressed = false;
			isShooting = false;
			_touchControlScheme.ResetDelta();
		}
		else
		{
			framesCount = 0;
			_touchControlScheme.OnDrag(delta);
		}
	}

	private void OnDestroy()
	{
		PauseNGUIController.PlayerHandUpdated -= SetSideAndCalcRects;
		ControlsSettingsBase.ControlsChanged -= SetShouldRecalcRects;
		ChooseGadgetPanel.OnDisablePanel -= ChooseGadgetPanel_OnDisablePanel;
	}

	public Vector2 GrabDeltaPosition()
	{
		Vector2 result = Vector2.zero;
		if (_touchControlScheme != null)
		{
			result = _touchControlScheme.DeltaPosition;
			_touchControlScheme.ResetDelta();
		}
		return result;
	}

	public void ApplyDeltaTo(Vector2 deltaPosition, Transform yawTransform, Transform pitchTransform, float sensitivity, bool invert)
	{
		if (_touchControlScheme != null)
		{
			_touchControlScheme.ApplyDeltaTo(deltaPosition, yawTransform, pitchTransform, sensitivity, invert);
		}
	}

	public void OnDisable()
	{
		jumpPressed = false;
		isShooting = false;
		isShootingPressure = false;
		MoveTouchID = -1;
		gadgetsTouchID = -1;
		ChooseGadgetPanel_OnDisablePanel();
	}
}

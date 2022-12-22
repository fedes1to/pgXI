using System;
using Rilisoft;
using UnityEngine;

public sealed class PauseNGUIController : ControlsSettingsBase
{
	public static PauseNGUIController sharedController;

	public SettingsToggleButtons switchingWeaponsToggleButtons;

	public SettingsToggleButtons chatToggleButtons;

	public SettingsToggleButtons musicToggleButtons;

	public SettingsToggleButtons soundToggleButtons;

	public SettingsToggleButtons invertCameraToggleButtons;

	public SettingsToggleButtons shoot3dTouchToggleButtons;

	public SettingsToggleButtons jump3dTouchToggleButtons;

	public SettingsToggleButtons hideJumpAndShootButtons;

	public SettingsToggleButtons leftHandedToggleButtons;

	public SettingsToggleButtons fps60ToggleButtons;

	public UIButton controlsButton;

	public GameObject tapPanelInSettings;

	public GameObject swipePanelInSettings;

	public UISlider sensitivitySlider;

	public UIButton resumeButton;

	private IDisposable _backSubscription;

	private float _cachedSensitivity;

	private bool _shopOpened;

	private float _lastBackFromShopTime;

	public static event Action InvertCamUpdated;

	public static event Action ChatSettUpdated;

	public static event Action PlayerHandUpdated;

	public static event Action SwitchingWeaponsUpdated;

	public static void Set60FPSEnable(bool isChecked, Action handler = null)
	{
		GlobalGameController.is60FPSEnable = !isChecked;
	}

	private new void Start()
	{
		base.Start();
		sharedController = this;
		resumeButton.GetComponent<ButtonHandler>().Clicked += HandleResumeButton;
		musicToggleButtons.IsChecked = Defs.isSoundMusic;
		musicToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			bool isSoundMusic = Defs.isSoundMusic;
			Defs.isSoundMusic = e.IsChecked;
			PlayerPrefsX.SetBool(PlayerPrefsX.SoundMusicSetting, Defs.isSoundMusic);
			PlayerPrefs.Save();
			if (isSoundMusic != Defs.isSoundMusic && isSoundMusic != Defs.isSoundMusic)
			{
				if (Defs.isSoundMusic)
				{
					GameObject gameObject = GameObject.FindGameObjectWithTag("BackgroundMusic");
					if (MenuBackgroundMusic.sharedMusic != null && gameObject != null)
					{
						AudioSource component = gameObject.GetComponent<AudioSource>();
						if (component != null)
						{
							MenuBackgroundMusic.sharedMusic.PlayMusic(component);
						}
					}
				}
				else
				{
					GameObject gameObject2 = GameObject.FindGameObjectWithTag("BackgroundMusic");
					if (MenuBackgroundMusic.sharedMusic != null && gameObject2 != null)
					{
						AudioSource component2 = gameObject2.GetComponent<AudioSource>();
						if (component2 != null)
						{
							MenuBackgroundMusic.sharedMusic.StopMusic(component2);
						}
					}
				}
			}
		};
		soundToggleButtons.IsChecked = Defs.isSoundFX;
		soundToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			Defs.isSoundFX = e.IsChecked;
			PlayerPrefsX.SetBool(PlayerPrefsX.SoundFXSetting, Defs.isSoundFX);
			PlayerPrefs.Save();
		};
		chatToggleButtons.IsChecked = Defs.IsChatOn;
		chatToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			SettingsController.SwitchChatSetting(e.IsChecked, delegate
			{
				Action chatSettUpdated = PauseNGUIController.ChatSettUpdated;
				if (chatSettUpdated != null)
				{
					chatSettUpdated();
				}
			});
		};
		invertCameraToggleButtons.IsChecked = PlayerPrefs.GetInt(Defs.InvertCamSN, 0) == 1;
		invertCameraToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			if (Application.isEditor)
			{
				Debug.Log("[Invert Camera] button clicked: " + e.IsChecked);
			}
			bool flag = PlayerPrefs.GetInt(Defs.InvertCamSN, 0) == 1;
			if (flag != e.IsChecked)
			{
				PlayerPrefs.SetInt(Defs.InvertCamSN, Convert.ToInt32(e.IsChecked));
				PlayerPrefs.Save();
				Action invertCamUpdated = PauseNGUIController.InvertCamUpdated;
				if (invertCamUpdated != null)
				{
					invertCamUpdated();
				}
			}
		};
		if (fps60ToggleButtons != null)
		{
			fps60ToggleButtons.IsChecked = !GlobalGameController.is60FPSEnable;
			fps60ToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				Set60FPSEnable(e.IsChecked);
			};
		}
		if (Defs.touchPressureSupported || Application.isEditor)
		{
			shoot3dTouchToggleButtons.gameObject.SetActive(true);
			shoot3dTouchToggleButtons.IsChecked = Defs.isUseShoot3DTouch;
			shoot3dTouchToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				if (Application.isEditor)
				{
					Debug.Log("3D touche button clicked: " + e.IsChecked);
				}
				Defs.isUseShoot3DTouch = e.IsChecked;
				hideJumpAndShootButtons.gameObject.SetActive(Defs.isUseJump3DTouch || Defs.isUseShoot3DTouch);
			};
		}
		else
		{
			shoot3dTouchToggleButtons.gameObject.SetActive(false);
		}
		if (Defs.touchPressureSupported || Application.isEditor)
		{
			jump3dTouchToggleButtons.gameObject.SetActive(true);
			jump3dTouchToggleButtons.IsChecked = Defs.isUseJump3DTouch;
			jump3dTouchToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				if (Application.isEditor)
				{
					Debug.Log("3D touche button clicked: " + e.IsChecked);
				}
				Defs.isUseJump3DTouch = e.IsChecked;
				hideJumpAndShootButtons.gameObject.SetActive(Defs.isUseJump3DTouch || Defs.isUseShoot3DTouch);
			};
		}
		else
		{
			jump3dTouchToggleButtons.gameObject.SetActive(false);
		}
		if (Defs.touchPressureSupported || Application.isEditor)
		{
			hideJumpAndShootButtons.gameObject.SetActive(Defs.isUseJump3DTouch || Defs.isUseShoot3DTouch);
			hideJumpAndShootButtons.IsChecked = Defs.isJumpAndShootButtonOn;
			hideJumpAndShootButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				if (Application.isEditor)
				{
					Debug.Log("3D touche button clicked: " + e.IsChecked);
				}
				Defs.isJumpAndShootButtonOn = e.IsChecked;
			};
		}
		else
		{
			hideJumpAndShootButtons.gameObject.SetActive(false);
		}
		if (sensitivitySlider != null)
		{
			float sensitivity = Defs.Sensitivity;
			float num = Mathf.Clamp(sensitivity, 6f, 19f);
			float num2 = num - 6f;
			sensitivitySlider.value = num2 / 13f;
			_cachedSensitivity = num;
		}
		else
		{
			Debug.LogWarning("sensitivitySlider == null");
		}
		if (leftHandedToggleButtons != null)
		{
			leftHandedToggleButtons.IsChecked = GlobalGameController.LeftHanded;
			leftHandedToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				SettingsController.ChangeLeftHandedRightHanded(e.IsChecked, delegate
				{
					Action playerHandUpdated = PauseNGUIController.PlayerHandUpdated;
					if (playerHandUpdated != null)
					{
						playerHandUpdated();
					}
				});
			};
		}
		switchingWeaponsToggleButtons.IsChecked = !GlobalGameController.switchingWeaponSwipe;
		switchingWeaponsToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			SettingsController.ChangeSwitchingWeaponHanded(e.IsChecked, delegate
			{
				Action switchingWeaponsUpdated = PauseNGUIController.SwitchingWeaponsUpdated;
				if (switchingWeaponsUpdated != null)
				{
					switchingWeaponsUpdated();
				}
			});
		};
		if (!(controlsButton != null))
		{
			return;
		}
		controlsButton.GetComponent<ButtonHandler>().Clicked += delegate
		{
			if (!InPauseShop())
			{
				ButtonClickSound.Instance.PlayClick();
				settingsPanel.SetActive(false);
				SettingsJoysticksPanel.SetActive(true);
				swipePanelInSettings.transform.parent.gameObject.SetActive(!Defs.isDaterRegim || !GlobalGameController.switchingWeaponSwipe);
				swipePanelInSettings.SetActive(Defs.isDaterRegim && GlobalGameController.switchingWeaponSwipe);
				tapPanelInSettings.SetActive(!GlobalGameController.switchingWeaponSwipe);
				ExperienceController.sharedController.isShowRanks = false;
				ExpController.Instance.InterfaceEnabled = false;
				HandleControlsClicked();
			}
		};
	}

	private void HandleResumeButton(object sender, EventArgs e)
	{
		if (!InPauseShop())
		{
			base.gameObject.SetActive(false);
		}
	}

	private bool InPauseShop()
	{
		return InGameGUI.sharedInGameGUI != null && InGameGUI.sharedInGameGUI.playerMoveC != null && InGameGUI.sharedInGameGUI.playerMoveC.isInappWinOpen;
	}

	private void Update()
	{
		if (_isCancellationRequested)
		{
			if (SettingsJoysticksPanel.activeInHierarchy)
			{
				HandleCancelPosJoystikClicked(this, null);
			}
			else if (!InPauseShop())
			{
				HandleResumeButton(this, null);
				_isCancellationRequested = false;
				return;
			}
			_isCancellationRequested = false;
		}
		float num = sensitivitySlider.value * 13f;
		float num2 = Mathf.Clamp(num + 6f, 6f, 19f);
		if (_cachedSensitivity != num2)
		{
			if (Application.isEditor)
			{
				Debug.Log("New sensitivity: " + num2);
			}
			Defs.Sensitivity = num2;
			_cachedSensitivity = num2;
		}
		if (!InPauseShop())
		{
			if (_shopOpened)
			{
				_lastBackFromShopTime = Time.realtimeSinceStartup;
			}
			_shopOpened = false;
			if (!Defs.isMulti)
			{
				ExperienceController.sharedController.isShowRanks = true;
			}
		}
		else
		{
			_shopOpened = true;
			_lastBackFromShopTime = float.PositiveInfinity;
		}
	}

	private new void OnEnable()
	{
		base.OnEnable();
		if (!InPauseShop())
		{
			ExperienceController.sharedController.isShowRanks = true;
			ExperienceController.sharedController.posRanks = NetworkStartTable.ExperiencePosRanks;
		}
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(HandleEscape, "Pause");
	}

	private void OnDestroy()
	{
		sharedController = null;
	}

	private void OnDisable()
	{
		if (!InPauseShop())
		{
			ExperienceController.sharedController.isShowRanks = false;
		}
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void HandleEscape()
	{
		if (!InPauseShop())
		{
			_isCancellationRequested = true;
		}
	}

	protected override void HandleSavePosJoystikClicked(object sender, EventArgs e)
	{
		base.HandleSavePosJoystikClicked(sender, e);
		ExperienceController.sharedController.isShowRanks = true;
		ExpController.Instance.InterfaceEnabled = true;
	}

	protected override void HandleCancelPosJoystikClicked(object sender, EventArgs e)
	{
		SettingsJoysticksPanel.SetActive(false);
		settingsPanel.SetActive(true);
		ExperienceController.sharedController.isShowRanks = true;
		ExpController.Instance.InterfaceEnabled = true;
	}
}

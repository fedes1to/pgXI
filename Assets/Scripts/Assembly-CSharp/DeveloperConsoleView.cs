using System.Globalization;
using UnityEngine;

internal sealed class DeveloperConsoleView : MonoBehaviour
{
	public static DeveloperConsoleView instance;

	public UIToggle strongDeivceCheckbox;

	public UIInput gemsInput;

	public UIToggle set60FpsCheckbox;

	public UIToggle mouseCOntrollCheckbox;

	public UIToggle spectatorModeCheckbox;

	public UIToggle fbRewardCheckbox;

	public UIToggle tempGunCheckbox;

	public UILabel levelLabel;

	public UILabel experienceLabel;

	public UISlider experienceSlider;

	public UISlider levelSlider;

	public UILabel ratingLabel;

	public UISlider ratingSlider;

	public UIInput coinsInput;

	public UIInput enemyCountInSurvivalWave;

	public UIInput enemiesInCampaignInput;

	public UIInput daysInput;

	public UIToggle trainingCheckbox;

	public UIToggle downgradeResolutionCheckbox;

	public UIToggle isPayingCheckbox;

	public UIToggle serverTimeCheckbox;

	public UIToggle areLogsEnabledCheckbox;

	public UIToggle isDebugGuiVisibleCheckbox;

	public UIToggle isEventX3ForcedCheckbox;

	public UIToggle adIdCheckbox;

	public UIInput marathonCurrentDay;

	public UIToggle marathonTestMode;

	public UIToggle gameGUIOffMode;

	public UILabel deviceInfo;

	public UILabel diagonalInfo;

	public UIInput devicePushTokenInput;

	public UIInput playerIdInput;

	public UILabel starterPackLive;

	public UILabel starterPackCooldown;

	public UILabel socialUsername;

	public UIInput oneDayPreminAccount;

	public UIInput threeDayPreminAccount;

	public UIInput sevenDayPreminAccount;

	public UIInput monthDayPreminAccount;

	public UIToggle memoryCheckbox;

	public UIToggle isPixelGunLowCheckbox;

	public UIToggle reviewCheckbox;

	public string LevelLabel
	{
		set
		{
			if (!(levelLabel == null))
			{
				levelLabel.text = value;
			}
		}
	}

	public string ExperienceLabel
	{
		set
		{
			if (!(experienceLabel == null))
			{
				experienceLabel.text = value;
			}
		}
	}

	public float ExperiencePercentage
	{
		get
		{
			return (!(experienceSlider != null)) ? 0f : experienceSlider.value;
		}
		set
		{
			if (!(experienceSlider == null))
			{
				experienceSlider.value = Mathf.Clamp01(value);
			}
		}
	}

	public float LevelPercentage
	{
		get
		{
			return (!(levelSlider != null)) ? 0f : levelSlider.value;
		}
		set
		{
			if (!(levelSlider == null))
			{
				levelSlider.value = Mathf.Clamp01(value);
			}
		}
	}

	public string RatingLabel
	{
		set
		{
			if (!(ratingLabel == null))
			{
				ratingLabel.text = value;
			}
		}
	}

	public float RatingPercentage
	{
		get
		{
			return (!(ratingSlider != null)) ? 0f : ratingSlider.value;
		}
		set
		{
			if (!(ratingSlider == null))
			{
				ratingSlider.value = Mathf.Clamp01(value);
			}
		}
	}

	public int CoinsInput
	{
		set
		{
			if (coinsInput != null)
			{
				coinsInput.value = value.ToString();
			}
		}
	}

	public int GemsInput
	{
		set
		{
			if (gemsInput != null)
			{
				gemsInput.value = value.ToString();
			}
		}
	}

	public int EnemiesInSurvivalWaveInput
	{
		set
		{
			if (enemyCountInSurvivalWave != null)
			{
				enemyCountInSurvivalWave.value = value.ToString(CultureInfo.InvariantCulture);
			}
		}
	}

	public int EnemiesInCampaignInput
	{
		set
		{
			if (enemiesInCampaignInput != null)
			{
				enemiesInCampaignInput.value = value.ToString(CultureInfo.InvariantCulture);
			}
		}
	}

	public int Days
	{
		set
		{
			if (daysInput != null)
			{
				daysInput.value = value.ToString(CultureInfo.InvariantCulture);
			}
		}
	}

	public bool StrongDevice
	{
		set
		{
			if (strongDeivceCheckbox != null)
			{
				strongDeivceCheckbox.value = value;
			}
		}
	}

	public bool TrainingCompleted
	{
		set
		{
			if (trainingCheckbox != null)
			{
				trainingCheckbox.value = value;
			}
		}
	}

	public bool TempGunActive
	{
		set
		{
			if (tempGunCheckbox != null)
			{
				tempGunCheckbox.value = value;
			}
		}
	}

	public bool Set60FPSActive
	{
		set
		{
			if (set60FpsCheckbox != null)
			{
				set60FpsCheckbox.value = value;
			}
		}
	}

	public bool SetMouseControll
	{
		set
		{
			if (mouseCOntrollCheckbox != null)
			{
				mouseCOntrollCheckbox.value = value;
			}
		}
	}

	public bool SetSpectatorMode
	{
		set
		{
			if (spectatorModeCheckbox != null)
			{
				spectatorModeCheckbox.value = value;
			}
		}
	}

	public bool SetFBReward
	{
		set
		{
			if (fbRewardCheckbox != null)
			{
				fbRewardCheckbox.value = value;
			}
		}
	}

	public bool IsPayingUser
	{
		set
		{
			if (isPayingCheckbox != null)
			{
				isPayingCheckbox.value = value;
			}
		}
	}

	public int MarathonDayInput
	{
		set
		{
			if (marathonCurrentDay != null)
			{
				marathonCurrentDay.value = value.ToString();
			}
		}
	}

	public bool MarathonTestMode
	{
		set
		{
			if (marathonTestMode != null)
			{
				marathonTestMode.value = value;
			}
		}
	}

	public bool GameGUIOffMode
	{
		set
		{
			if (gameGUIOffMode != null)
			{
				gameGUIOffMode.value = value;
			}
		}
	}

	public string DevicePushTokenInput
	{
		set
		{
			if (devicePushTokenInput != null)
			{
				devicePushTokenInput.value = value;
			}
		}
	}

	public string PlayerIdInput
	{
		set
		{
			if (playerIdInput != null)
			{
				playerIdInput.value = value;
			}
		}
	}

	public string SocialUserName
	{
		set
		{
			if (socialUsername != null)
			{
				socialUsername.text = value;
			}
		}
	}

	public bool MemoryInfoActive
	{
		get
		{
			if ((bool)memoryCheckbox)
			{
				return memoryCheckbox.value;
			}
			return false;
		}
		set
		{
			if ((bool)memoryCheckbox)
			{
				memoryCheckbox.value = value;
			}
		}
	}

	public bool ReviewActive
	{
		get
		{
			if ((bool)reviewCheckbox)
			{
				return reviewCheckbox.value;
			}
			return false;
		}
		set
		{
			if ((bool)reviewCheckbox)
			{
				reviewCheckbox.value = value;
			}
		}
	}

	private void Awake()
	{
		instance = this;
		diagonalInfo.text = "Диагональ: " + Defs.ScreenDiagonal;
	}

	private void OnDestroy()
	{
		instance = null;
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using I2.Loc;
using Rilisoft;
using UnityEngine;

internal sealed class FreeAwardView : MonoBehaviour
{
	public GameObject backgroundPanel;

	public GameObject waitingPanel;

	public CurrencySpecificWatchPanel watchForCoinsPanel;

	public CurrencySpecificWatchPanel watchForGemsPanel;

	public GameObject connectionPanel;

	public GameObject awardPanelCoins;

	public GameObject awardPanelGems;

	public GameObject closePanel;

	public UIButton devSkipButton;

	public UITexture loadingSpinner;

	public UILabel awardOuterLabelCoins;

	public UILabel awardOuterLabelGems;

	private FreeAwardController.State _currentState;

	internal FreeAwardController.State CurrentState
	{
		private get
		{
			return _currentState;
		}
		set
		{
			if (value != _currentState)
			{
				FreeAwardController.WatchState watchState = value as FreeAwardController.WatchState;
				if (watchState != null)
				{
					TimeSpan estimatedTimeSpan = watchState.GetEstimatedTimeSpan();
					bool flag = estimatedTimeSpan <= TimeSpan.FromMinutes(0.0);
					SetWatchButtonEnabled(flag, estimatedTimeSpan);
				}
				else
				{
					SetWatchButtonEnabled(false);
				}
				RefreshAwardLabel(watchState != null);
			}
			if (backgroundPanel != null)
			{
				backgroundPanel.SetActive(!(value is FreeAwardController.IdleState));
			}
			bool flag2 = value is FreeAwardController.WaitingState;
			if (FreeAwardController.Instance.SimplifiedInterface)
			{
				if (waitingPanel != null)
				{
					waitingPanel.SetActive(false);
				}
				if ((_currentState is FreeAwardController.WaitingState || value is FreeAwardController.WaitingState) && ActivityIndicator.IsActiveIndicator != flag2)
				{
					ActivityIndicator.IsActiveIndicator = flag2;
				}
			}
			else
			{
				if (waitingPanel != null)
				{
					waitingPanel.SetActive(flag2);
				}
				if (_currentState is FreeAwardController.WaitingState && !(value is FreeAwardController.WaitingState) && ActivityIndicator.IsActiveIndicator)
				{
					ActivityIndicator.IsActiveIndicator = false;
				}
			}
			if (connectionPanel != null)
			{
				connectionPanel.SetActive(value is FreeAwardController.ConnectionState);
			}
			if (closePanel != null)
			{
				closePanel.SetActive(value is FreeAwardController.CloseState);
			}
			if (value is FreeAwardController.WatchState)
			{
				if (FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")
				{
					watchForGemsPanel.gameObject.SetActive(true);
					watchForCoinsPanel.gameObject.SetActive(false);
				}
				else
				{
					watchForGemsPanel.gameObject.SetActive(false);
					watchForCoinsPanel.gameObject.SetActive(true);
				}
			}
			else
			{
				watchForGemsPanel.gameObject.SetActive(false);
				watchForCoinsPanel.gameObject.SetActive(false);
			}
			if (value is FreeAwardController.AwardState)
			{
				if (FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")
				{
					awardPanelGems.SetActive(true);
				}
				else
				{
					awardPanelCoins.SetActive(true);
				}
			}
			else
			{
				awardPanelCoins.SetActive(false);
				awardPanelGems.SetActive(false);
			}
			_currentState = value;
		}
	}

	private void RefreshAwardLabel(bool visible)
	{
		if (!visible)
		{
			return;
		}
		string text = LocalizationStore.Get(ScriptLocalization.Key_0291);
		int countMoneyForAward = FreeAwardController.CountMoneyForAward;
		text += ((!(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")) ? countMoneyForAward.ToString(CultureInfo.InvariantCulture) : string.Format(CultureInfo.InvariantCulture, " [c][50CEFFFF]{0}[-][/c]", countMoneyForAward));
		List<UILabel> list = new List<UILabel>();
		list.AddRange(awardOuterLabelCoins.gameObject.GetComponentsInChildren<UILabel>(true));
		list.AddRange(awardOuterLabelGems.gameObject.GetComponentsInChildren<UILabel>(true));
		foreach (UILabel item in list)
		{
			item.text = text;
		}
	}

	private void Start()
	{
		if (devSkipButton != null)
		{
			devSkipButton.gameObject.SetActive(Application.isEditor || (Defs.IsDeveloperBuild && BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64));
		}
	}

	private void Update()
	{
		FreeAwardController.WaitingState waitingState = CurrentState as FreeAwardController.WaitingState;
		if (waitingState != null)
		{
			if (Application.isEditor && Input.GetKeyUp(KeyCode.S))
			{
				FreeAwardController.Instance.HandleDeveloperSkip();
			}
			if (loadingSpinner != null)
			{
				float num = Time.realtimeSinceStartup - waitingState.StartTime;
				int num2 = Mathf.FloorToInt(num);
				loadingSpinner.invert = num2 % 2 == 0;
				loadingSpinner.fillAmount = ((!loadingSpinner.invert) ? (1f - num + (float)num2) : (num - (float)num2));
			}
		}
		FreeAwardController.WatchState watchState = CurrentState as FreeAwardController.WatchState;
		if (watchState != null && Time.frameCount % 10 == 0)
		{
			TimeSpan estimatedTimeSpan = watchState.GetEstimatedTimeSpan();
			bool flag = estimatedTimeSpan <= TimeSpan.FromMinutes(0.0);
			SetWatchButtonEnabled(flag, estimatedTimeSpan);
		}
	}

	private void SetWatchButtonEnabled(bool enabled, TimeSpan nextTimeAwailable)
	{
		CurrencySpecificWatchPanel currencySpecificWatchPanel = ((!(FreeAwardController.Instance.CurrencyForAward == "GemsCurrency")) ? watchForCoinsPanel : watchForGemsPanel);
		if (currencySpecificWatchPanel == null)
		{
			Debug.LogWarning("panel == null");
			return;
		}
		if (currencySpecificWatchPanel.WatchButton != null)
		{
			currencySpecificWatchPanel.WatchButton.isEnabled = enabled;
		}
		if (currencySpecificWatchPanel.WatchHeader != null)
		{
			currencySpecificWatchPanel.WatchHeader.gameObject.SetActive(enabled);
		}
		if (!(currencySpecificWatchPanel.WatchTimer != null))
		{
			return;
		}
		currencySpecificWatchPanel.WatchTimer.transform.parent.gameObject.SetActive(!enabled);
		if (enabled)
		{
			return;
		}
		string text = ((nextTimeAwailable.Hours <= 0) ? string.Format("{0}:{1:D2}", nextTimeAwailable.Minutes, nextTimeAwailable.Seconds) : string.Format("{0}:{1:D2}:{2:D2}", nextTimeAwailable.Hours, nextTimeAwailable.Minutes, nextTimeAwailable.Seconds));
		foreach (UILabel watchTimerLabel in GetWatchTimerLabels(currencySpecificWatchPanel.WatchTimer))
		{
			watchTimerLabel.text = text;
		}
	}

	private void SetWatchButtonEnabled(bool enabled)
	{
		SetWatchButtonEnabled(enabled, default(TimeSpan));
	}

	private List<UILabel> GetWatchTimerLabels(UILabel rootLabel)
	{
		List<UILabel> list = new List<UILabel>(3);
		list.Add(rootLabel);
		List<UILabel> result = list;
		rootLabel.GetComponentsInChildren(true, result);
		return result;
	}
}

using System;
using System.Collections.Generic;
using Rilisoft;

public sealed class SocialGunBannerView : BannerWindow
{
	public bool freePanelBanner;

	public List<UILabel> rewardLabels;

	private IDisposable _backSubscription;

	public static event Action<bool> SocialGunBannerViewLoginCompletedWithResult;

	private void SetRewardLabelsText()
	{
		foreach (UILabel rewardLabel in rewardLabels)
		{
			rewardLabel.text = string.Format(LocalizationStore.Get("Key_1531"), 10);
		}
	}

	private void Awake()
	{
		SetRewardLabelsText();
		LocalizationStore.AddEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(HandleEscape, "Social Gun");
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void HandleEscape()
	{
		HideWindow();
	}

	public override void Show()
	{
		base.Show();
		if (FacebookController.sharedController != null)
		{
			FacebookController.sharedController.UpdateCountShownWindowByShowCondition();
		}
	}

	public void HideWindow()
	{
		ButtonClickSound.TryPlayClick();
		if (!freePanelBanner)
		{
			BannerWindowController sharedController = BannerWindowController.SharedController;
			if (sharedController != null)
			{
				sharedController.HideBannerWindow();
				return;
			}
		}
		Hide();
	}

	public void Continue()
	{
		MainMenuController.DoMemoryConsumingTaskInEmptyScene(delegate
		{
			FacebookController.Login(delegate
			{
				FireSocialGunBannerViewLoginCompletedEvent(true);
			}, delegate
			{
				FireSocialGunBannerViewLoginCompletedEvent(false);
			}, "Social Gun Banner");
		}, delegate
		{
			FacebookController.Login(null, null, "Social Gun Banner");
		});
	}

	private void FireSocialGunBannerViewLoginCompletedEvent(bool val)
	{
		Action<bool> socialGunBannerViewLoginCompletedWithResult = SocialGunBannerView.SocialGunBannerViewLoginCompletedWithResult;
		if (socialGunBannerViewLoginCompletedWithResult != null)
		{
			socialGunBannerViewLoginCompletedWithResult(val);
		}
	}

	private void OnDestroy()
	{
		LocalizationStore.DelEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void HandleLocalizationChanged()
	{
		SetRewardLabelsText();
	}
}

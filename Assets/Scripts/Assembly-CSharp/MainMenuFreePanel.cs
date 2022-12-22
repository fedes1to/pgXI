using System;
using Facebook.Unity;
using Rilisoft;
using UnityEngine;

internal sealed class MainMenuFreePanel : MonoBehaviour
{
	[SerializeField]
	private GameObject _postNewsLabel;

	[SerializeField]
	private GameObject _starParticleSocialGunButton;

	[SerializeField]
	private GameObject _socialGunPanel;

	[SerializeField]
	private ButtonHandler _youtubeButton;

	[SerializeField]
	private ButtonHandler _enderManButton;

	[SerializeField]
	private ButtonHandler _postFacebookButton;

	[SerializeField]
	private ButtonHandler _postTwitterButton;

	[SerializeField]
	private ButtonHandler _rateUsButton;

	[SerializeField]
	private ButtonHandler _backButton;

	[SerializeField]
	private ButtonHandler _twitterSubcribeButton;

	[SerializeField]
	private ButtonHandler _facebookSubcribeButton;

	[SerializeField]
	private ButtonHandler _instagramSubcribeButton;

	[SerializeField]
	private UILabel _socialGunEventTimerLabel;

	private void Start()
	{
		if (_socialGunPanel != null)
		{
			_socialGunPanel.SetActive(FacebookController.FacebookSupported);
		}
		_postNewsLabel.SetActive(false);
		if (_youtubeButton != null)
		{
			_youtubeButton.Clicked += HandleYoutubeClicked;
		}
		if (_enderManButton != null)
		{
			_enderManButton.Clicked += HandleEnderClicked;
		}
		if (_postFacebookButton != null)
		{
			_postFacebookButton.Clicked += HandlePostFacebookClicked;
		}
		if (_postTwitterButton != null)
		{
			_postTwitterButton.Clicked += HandlePostTwittwerClicked;
		}
		if (_rateUsButton != null)
		{
			_rateUsButton.Clicked += HandleRateAsClicked;
		}
		if (_twitterSubcribeButton != null)
		{
			_twitterSubcribeButton.Clicked += HandleTwitterSubscribeClicked;
		}
		if (_facebookSubcribeButton != null)
		{
			_facebookSubcribeButton.Clicked += HandleFacebookSubscribeClicked;
		}
		if (_instagramSubcribeButton != null)
		{
			_instagramSubcribeButton.Clicked += HandleInstagramSubscribeClicked;
		}
		if (_backButton != null)
		{
			_backButton.Clicked += delegate
			{
				MainMenuController.sharedController._isCancellationRequested = true;
			};
		}
		FacebookController.SocialGunEventStateChanged += HandleSocialGunEventStateChanged;
		if (FacebookController.sharedController != null)
		{
			HandleSocialGunEventStateChanged(FacebookController.sharedController.SocialGunEventActive);
		}
	}

	private void Update()
	{
		bool flag = (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled) && !ShopNGUIController.GuiActive;
		if (_starParticleSocialGunButton != null && _starParticleSocialGunButton.activeInHierarchy != flag)
		{
			_starParticleSocialGunButton.SetActive(flag);
		}
		if (_postFacebookButton.gameObject.activeSelf != (FacebookController.FacebookSupported && FacebookController.FacebookPost_Old_Supported && FB.IsLoggedIn))
		{
			_postFacebookButton.gameObject.SetActive(FacebookController.FacebookSupported && FacebookController.FacebookPost_Old_Supported && FB.IsLoggedIn);
		}
		if (_postTwitterButton.gameObject.activeSelf != (TwitterController.TwitterSupported && TwitterController.TwitterSupported_OldPosts && TwitterController.IsLoggedIn))
		{
			_postTwitterButton.gameObject.SetActive(TwitterController.TwitterSupported && TwitterController.TwitterSupported_OldPosts && TwitterController.IsLoggedIn);
		}
		if (FacebookController.sharedController != null && FacebookController.sharedController.SocialGunEventActive)
		{
			_socialGunEventTimerLabel.text = string.Empty;
		}
	}

	private void OnDestroy()
	{
		FacebookController.SocialGunEventStateChanged -= HandleSocialGunEventStateChanged;
	}

	public void SetVisible(bool visible)
	{
		if (base.gameObject.activeSelf != visible)
		{
			base.gameObject.SetActive(visible);
		}
	}

	public void OnSocialGunButtonClicked()
	{
		MainMenuController.sharedController.OnSocialGunEventButtonClick();
	}

	private void HandleSocialGunEventStateChanged(bool enable)
	{
		_socialGunPanel.gameObject.SetActive(enable);
		GetComponentsInChildren<RewardedLikeButton>(true).ForEach(delegate(RewardedLikeButton b)
		{
			b.gameObject.SetActive(!enable);
		});
		if (FacebookController.sharedController != null)
		{
			_socialGunEventTimerLabel.text = string.Empty;
		}
	}

	private void HandleYoutubeClicked(object sender, EventArgs e)
	{
		if (!MainMenuController.ShopOpened)
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			Application.OpenURL("http://www.youtube.com/channel/UCsClw1gnMrmF6ssIB_166_Q");
		}
	}

	private void HandleEnderClicked(object sender, EventArgs e)
	{
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		ButtonClickSound.Instance.PlayClick();
		if (Application.isEditor)
		{
			Debug.Log(MainMenu.GetEndermanUrl());
		}
		else
		{
			Application.OpenURL(MainMenu.GetEndermanUrl());
		}
	}

	private void HandlePostFacebookClicked(object sender, EventArgs e)
	{
		if (!MainMenuController.ShopOpened && !MainMenuController.ShowBannerOrLevelup())
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			FacebookController.ShowPostDialog();
		}
	}

	private void HandlePostTwittwerClicked(object sender, EventArgs e)
	{
		if (MainMenuController.ShopOpened || MainMenuController.ShowBannerOrLevelup())
		{
			return;
		}
		if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
		{
			TrainingController.TrainingCompletedFlagForLogging = null;
		}
		ButtonClickSound.Instance.PlayClick();
		if (!Application.isEditor)
		{
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			if (TwitterController.Instance != null)
			{
				TwitterController.Instance.PostStatusUpdate("Come and play with me in epic multiplayer shooter - Pixel Gun 3D! http://goo.gl/dQMf4n");
			}
		}
	}

	private void HandleRateAsClicked(object sender, EventArgs e)
	{
		if (MainMenuController.sharedController != null)
		{
			MainMenuController.sharedController.RateUs();
		}
	}

	private void HandleTwitterSubscribeClicked(object sender, EventArgs e)
	{
		if (!MainMenuController.ShopOpened)
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			Application.OpenURL("https://twitter.com/PixelGun3D");
		}
	}

	private void HandleFacebookSubscribeClicked(object sender, EventArgs e)
	{
		if (!MainMenuController.ShopOpened)
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			Application.OpenURL("http://pixelgun3d.com/facebook.html");
		}
	}

	private void HandleInstagramSubscribeClicked(object sender, EventArgs e)
	{
		if (!MainMenuController.ShopOpened)
		{
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
			Application.OpenURL("http://www.instagram.com/pixelgun3d_official");
		}
	}
}

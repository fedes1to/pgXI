using System;
using UnityEngine;

namespace Rilisoft
{
	[Serializable]
	[DisallowMultipleComponent]
	internal sealed class MainMenuGooglePlayGamesButtonController : MonoBehaviour
	{
		private bool _runningAuthentication;

		[SerializeField]
		private GameObject _buttonsHolder;

		[SerializeField]
		private GameObject _signOutButton;

		private MainMenuGooglePlayGamesButtonController()
		{
		}

		private void Start()
		{
			GpgFacade.Instance.SignedOut += HandleSignOut;
		}

		private void OnDestroy()
		{
			GpgFacade.Instance.SignedOut -= HandleSignOut;
		}

		private void OnEnable()
		{
			RefreshButtonsVisibility();
			RefreshSignOutButton();
		}

		private void RefreshButtonsVisibility()
		{
			if (!(_buttonsHolder == null))
			{
				if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
				{
					_buttonsHolder.SetActive(false);
				}
				else if (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.GoogleLite)
				{
					_buttonsHolder.SetActive(false);
				}
				else
				{
					_buttonsHolder.SetActive(true);
				}
			}
		}

		private void RefreshSignOutButton()
		{
			if (!(_signOutButton == null))
			{
				if (Application.isEditor)
				{
					_signOutButton.SetActive(true);
					return;
				}
				bool active = GpgFacade.Instance.IsAuthenticated();
				_signOutButton.SetActive(active);
			}
		}

		private void HandleSignOut(object sender, EventArgs e)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("{0}.HandleSignOut(); isAuthenticated: {1}", GetType().Name, GpgFacade.Instance.IsAuthenticated());
			}
			RefreshSignOutButton();
		}

		public void HandlePlayGamesButton()
		{
			if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android || Defs.AndroidEdition != Defs.RuntimeAndroidEdition.GoogleLite)
			{
				return;
			}
			if (TrainingController.TrainingCompletedFlagForLogging.HasValue)
			{
				TrainingController.TrainingCompletedFlagForLogging = null;
			}
			ButtonClickSound.Instance.PlayClick();
			if (_runningAuthentication)
			{
				Debug.Log("Ignoring Play Games button since authentication is currently running.");
				return;
			}
			RefreshSignOutButton();
			if (GpgFacade.Instance.IsAuthenticated())
			{
				Social.ShowAchievementsUI();
				return;
			}
			_runningAuthentication = true;
			GpgFacade.Instance.Authenticate(delegate(bool succeeded)
			{
				RefreshSignOutButton();
				if (succeeded)
				{
					Social.ShowAchievementsUI();
				}
				else
				{
					PlayerPrefs.SetInt("GoogleSignInDenied", 1);
				}
				_runningAuthentication = false;
			}, false);
		}

		public void HandleSignOutGooglePlayGamesButton()
		{
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
			{
				ButtonClickSound.TryPlayClick();
				PlayerPrefs.SetInt("GoogleSignInDenied", 1);
				GpgFacade.Instance.SignOut();
				string text = LocalizationStore.Get("Key_2103") ?? "Signed out.";
				InfoWindowController.ShowInfoBox(text);
			}
		}
	}
}

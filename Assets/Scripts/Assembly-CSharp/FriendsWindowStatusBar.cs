using System;
using Facebook.Unity;
using UnityEngine;

public class FriendsWindowStatusBar : MonoBehaviour
{
	public UIWidget findContainer;

	public UIWidget inboxContainer;

	public UILabel warningMessageAboutLimit;

	public MyUIInput findFriendInput;

	public UIWidget facebookContainer;

	public UILabel messageFacebookLabel;

	public UILabel[] facebookButtonLabels;

	public UILabel[] facebookRewardLabels;

	public UILabel[] facebookInviteLabels;

	public UIWidget facebookRewardLabelContainer;

	public UIButton refreshButton;

	public UIButton sendMyIdButton;

	public UIButton clearAllInviteButton;

	public bool IsFindFriendByIdStateActivate { get; set; }

	private void Start()
	{
		if (findFriendInput != null)
		{
			MyUIInput myUIInput = findFriendInput;
			myUIInput.onKeyboardInter = (Action)Delegate.Combine(myUIInput.onKeyboardInter, new Action(OnClickFindFriendButton));
			MyUIInput myUIInput2 = findFriendInput;
			myUIInput2.onKeyboardCancel = (Action)Delegate.Combine(myUIInput2.onKeyboardCancel, new Action(OnClickCancelFindFriendButton));
		}
	}

	private void OnEnable()
	{
		string text = string.Format(LocalizationStore.Get("Key_1416"), Defs.maxCountFriend, Defs.maxCountFriend);
		warningMessageAboutLimit.text = text;
		findFriendInput.defaultText = LocalizationStore.Get("Key_1422");
		InitFacebookRewardButtonText();
	}

	private void OnDestroy()
	{
		if (findFriendInput != null)
		{
			MyUIInput myUIInput = findFriendInput;
			myUIInput.onKeyboardInter = (Action)Delegate.Remove(myUIInput.onKeyboardInter, new Action(OnClickFindFriendButton));
			MyUIInput myUIInput2 = findFriendInput;
			myUIInput2.onKeyboardCancel = (Action)Delegate.Remove(myUIInput2.onKeyboardCancel, new Action(OnClickCancelFindFriendButton));
		}
	}

	private void InitFacebookRewardButtonText()
	{
		if (facebookButtonLabels != null && facebookButtonLabels.Length != 0)
		{
			for (int i = 0; i < facebookButtonLabels.Length; i++)
			{
				facebookButtonLabels[i].text = LocalizationStore.Get("Key_1582");
			}
		}
	}

	private void SetFacebookNotRewardButtonText(string text)
	{
		if (facebookInviteLabels != null && facebookInviteLabels.Length != 0)
		{
			for (int i = 0; i < facebookInviteLabels.Length; i++)
			{
				facebookInviteLabels[i].text = text;
			}
		}
	}

	private void SetVisibleFacebookButtonTextByState(bool needFullLabel)
	{
		if (facebookButtonLabels.Length != 0 && facebookInviteLabels.Length != 0)
		{
			facebookButtonLabels[0].gameObject.SetActive(!needFullLabel);
			facebookInviteLabels[0].gameObject.SetActive(needFullLabel);
		}
	}

	private void SetTextFacebookButton(bool isLogin, bool isRewardTake)
	{
		SetVisibleFacebookButtonTextByState(isLogin || isRewardTake);
		if (isRewardTake)
		{
			facebookRewardLabelContainer.gameObject.SetActive(false);
			string facebookNotRewardButtonText = (isLogin ? LocalizationStore.Get("Key_1437") : LocalizationStore.Get("Key_1582"));
			SetFacebookNotRewardButtonText(facebookNotRewardButtonText);
			return;
		}
		facebookRewardLabelContainer.gameObject.SetActive(true);
		for (int i = 0; i < facebookRewardLabels.Length; i++)
		{
			facebookRewardLabels[i].text = string.Format("+{0}", 10);
		}
	}

	private void SetTextFacebookDescription()
	{
		if (FB.IsLoggedIn)
		{
			messageFacebookLabel.text = LocalizationStore.Get("Key_1413");
		}
		else
		{
			messageFacebookLabel.text = LocalizationStore.Get("Key_1415");
		}
	}

	private void SetupStateFacebookContainer(bool needHide)
	{
		if (!FacebookController.FacebookSupported)
		{
			needHide = true;
		}
		if (needHide)
		{
			facebookContainer.gameObject.SetActive(false);
			return;
		}
		bool isRewardTake = Storager.getInt(Defs.IsFacebookLoginRewardaGained, true) == 1;
		facebookContainer.gameObject.SetActive(true);
		SetTextFacebookButton(FB.IsLoggedIn, isRewardTake);
	}

	public void OnClickBackButton()
	{
		ButtonClickSound.TryPlayClick();
		FriendsWindowGUI.Instance.HideInterface();
	}

	private void OnSuccsessGetFacebookFriendList()
	{
		FriendsController sharedController = FriendsController.sharedController;
		if (!(sharedController == null))
		{
			sharedController.DownloadDataAboutPossibleFriends();
			FriendsWindowController.Instance.NeedUpdateCurrentFriendsList = true;
		}
	}

	private void OnFacebookLoginComplete()
	{
		BlockClickInWindow(false);
		SetupStateFacebookContainer(false);
		FacebookController sharedController = FacebookController.sharedController;
		if (sharedController != null)
		{
			sharedController.InputFacebookFriends(OnSuccsessGetFacebookFriendList);
		}
	}

	private void OnFacebookLoginCancel()
	{
		BlockClickInWindow(false);
	}

	private void BlockClickInWindow(bool enable)
	{
	}

	public void OnClickFacebookButton()
	{
		ButtonClickSound.TryPlayClick();
		if (!FacebookController.FacebookSupported)
		{
			return;
		}
		FacebookController sharedController = FacebookController.sharedController;
		if (sharedController == null)
		{
			return;
		}
		BlockClickInWindow(true);
		if (FB.IsLoggedIn)
		{
			sharedController.InvitePlayer(null);
			return;
		}
		MainMenuController.DoMemoryConsumingTaskInEmptyScene(delegate
		{
			FacebookController.Login(OnFacebookLoginComplete, OnFacebookLoginCancel, "Friends");
		}, delegate
		{
			FacebookController.Login(null, null, "Friends");
		});
	}

	public void OnClickFindFriendButton()
	{
		ButtonClickSound.TryPlayClick();
		FriendsWindowController instance = FriendsWindowController.Instance;
		if (!(instance == null) && !string.IsNullOrEmpty(findFriendInput.value))
		{
			IsFindFriendByIdStateActivate = true;
			StartCoroutine(instance.ShowResultFindPlayer(findFriendInput.value));
			findFriendInput.value = string.Empty;
		}
	}

	public void OnClickCancelFindFriendButton()
	{
		ButtonClickSound.TryPlayClick();
		findFriendInput.value = string.Empty;
	}

	public void OnClickClearAllButton()
	{
		ButtonClickSound.TryPlayClick();
		FriendsWindowController instance = FriendsWindowController.Instance;
		if (!(instance == null))
		{
			instance.OnClickClearAllInboxButton();
		}
	}

	public void OnClickSendMyIdButton()
	{
		ButtonClickSound.TryPlayClick();
		FriendsController.SendMyIdByEmail();
	}

	public void OnClickRefreshButton()
	{
		ButtonClickSound.TryPlayClick();
		FriendsController sharedController = FriendsController.sharedController;
		if (!(sharedController == null))
		{
			FriendsWindowController instance = FriendsWindowController.Instance;
			if (!(instance == null))
			{
				instance.ShowMessageBoxProcessingData();
				refreshButton.isEnabled = false;
				Invoke("SetEnableRefreshButton", Defs.timeBlockRefreshFriendDate);
				sharedController.GetFriendsData(true);
			}
		}
	}

	private void SetEnableRefreshButton()
	{
		refreshButton.isEnabled = true;
	}

	public void UpdateFriendListState(bool isFriendsMax)
	{
		SetupStateFacebookContainer(isFriendsMax);
		warningMessageAboutLimit.gameObject.SetActive(isFriendsMax);
		findContainer.gameObject.SetActive(false);
		inboxContainer.gameObject.SetActive(false);
		refreshButton.gameObject.SetActive(true);
	}

	public void UpdateFindFriendsState(bool isFriendsMax)
	{
		IsFindFriendByIdStateActivate = false;
		SetupStateFacebookContainer(true);
		warningMessageAboutLimit.gameObject.SetActive(isFriendsMax);
		findContainer.gameObject.SetActive(!isFriendsMax);
		inboxContainer.gameObject.SetActive(false);
		refreshButton.gameObject.SetActive(false);
	}

	public void OnClickInboxFriendsTab(bool isInboxListFound, bool isFriendsMax)
	{
		SetupStateFacebookContainer(isFriendsMax || isInboxListFound);
		findContainer.gameObject.SetActive(false);
		bool flag = !isFriendsMax && isInboxListFound;
		inboxContainer.gameObject.SetActive(flag);
		if (flag && Application.platform != RuntimePlatform.IPhonePlayer)
		{
			sendMyIdButton.gameObject.SetActive(false);
			clearAllInviteButton.transform.position = sendMyIdButton.transform.position;
		}
		refreshButton.gameObject.SetActive(false);
	}

	public void OnClickChatTab()
	{
		SetupStateFacebookContainer(true);
		findContainer.gameObject.SetActive(false);
		inboxContainer.gameObject.SetActive(false);
		warningMessageAboutLimit.gameObject.SetActive(false);
		refreshButton.gameObject.SetActive(false);
	}
}

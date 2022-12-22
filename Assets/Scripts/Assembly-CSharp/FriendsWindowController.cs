using System;
using System.Collections;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public class FriendsWindowController : MonoBehaviour
{
	private enum WindowState
	{
		friendList,
		findFriends,
		chat,
		inbox
	}

	public static Action UpdateFriendsOnlineEvent;

	public UIButton goInBattleButton;

	public UIWrapContent friendsListWrap;

	public UIWrapContent inviteFriendsWrap;

	public UIWrapContent findFriendsWrap;

	private string[] inviteFriendsList;

	private string[] findFriendsList;

	private string[] myFriendsList;

	private string[] localFriendsList;

	public GameObject friendPreviewPrefab;

	public UIToggle chatTab;

	public UIToggle friendsTab;

	public FriendsWindowStatusBar statusBar;

	public UILabel emptyStateTabLabel;

	public GameObject emptyStateLocalPlayersTabLabel;

	public GameObject chatContainer;

	public GameObject joinToFriendRoomPhoton;

	public static FriendsWindowController Instance;

	private WindowState currentWindowState;

	private bool _isAnyFriendsDataExists;

	private bool _isFriendsMax;

	private bool wrapsInit;

	[NonSerialized]
	public bool isNeedRebuildFindFriendsList = true;

	private FriendPreviewItem _selectProfileItem;

	private bool _isWindowInStartState;

	public bool NeedUpdateCurrentFriendsList { get; set; }

	public void OnClickGoInBattleButton()
	{
		ButtonClickSound.TryPlayClick();
		GlobalGameController.GoInBattle();
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		currentWindowState = WindowState.friendList;
	}

	private void OnEnable()
	{
		FriendsController.FriendsUpdated += UpdateFriendsListInterface;
		FriendsController.OnShowBoxProcessFriendsData = (Action)Delegate.Combine(FriendsController.OnShowBoxProcessFriendsData, new Action(ShowMessageBoxProcessingData));
		FriendsController.OnHideBoxProcessFriendsData = (Action)Delegate.Combine(FriendsController.OnHideBoxProcessFriendsData, new Action(HideInfoBox));
		FriendsController.UpdateFriendsInfoAction = (Action)Delegate.Combine(FriendsController.UpdateFriendsInfoAction, new Action(EventUpdateFriendsOnlineAndSorting));
	}

	public void ShowMessageBoxProcessingData()
	{
		InfoWindowController.ShowProcessingDataBox();
	}

	public void HideInfoBox()
	{
		InfoWindowController.HideProcessing();
	}

	private void ShowMessageByEmptyStateTab(string text)
	{
		emptyStateTabLabel.gameObject.SetActive(true);
		emptyStateTabLabel.text = text;
	}

	private void HideMessageByEmptyStateTab()
	{
		emptyStateTabLabel.gameObject.SetActive(false);
	}

	private void CheckShowEmptyStateTabLabel(bool isListNotEmpty, bool isFriendsMax)
	{
		if (isListNotEmpty)
		{
			if (currentWindowState == WindowState.chat)
			{
				chatContainer.SetActive(true);
			}
			HideMessageByEmptyStateTab();
		}
		else if (currentWindowState == WindowState.chat)
		{
			ShowMessageByEmptyStateTab(LocalizationStore.Get("Key_1557"));
			chatContainer.SetActive(false);
		}
		else if (isFriendsMax)
		{
			ShowMessageByEmptyStateTab(LocalizationStore.Get("Key_1424"));
		}
		else if (currentWindowState == WindowState.inbox)
		{
			ShowMessageByEmptyStateTab(LocalizationStore.Get("Key_1425"));
		}
		else if (currentWindowState == WindowState.friendList)
		{
			ShowMessageByEmptyStateTab(LocalizationStore.Get("Key_0223"));
		}
		else
		{
			ShowMessageByEmptyStateTab(LocalizationStore.Get("Key_1423"));
		}
	}

	private void UpdateFriendsListState()
	{
		if (myFriendsList != null)
		{
			UpdateList(friendsListWrap, FriendItemPreviewType.view);
		}
		_isAnyFriendsDataExists = FriendsController.IsFriendsOrLocalDataExist();
		_isFriendsMax = FriendsController.IsFriendsMax();
		if (currentWindowState == WindowState.chat)
		{
			CheckShowEmptyStateTabLabel(FriendsController.IsFriendsDataExist(), _isFriendsMax);
		}
		else
		{
			CheckShowEmptyStateTabLabel(_isAnyFriendsDataExists, _isFriendsMax);
		}
		bool active = !_isAnyFriendsDataExists && ProtocolListGetter.currentVersionIsSupported;
		goInBattleButton.gameObject.SetActive(active);
		if (currentWindowState == WindowState.friendList)
		{
			statusBar.UpdateFriendListState(_isFriendsMax);
		}
	}

	private void SetActiveFriendsListContainer()
	{
		currentWindowState = WindowState.friendList;
		UpdateFriendsListState();
		if (!_isAnyFriendsDataExists)
		{
			_isWindowInStartState = false;
			return;
		}
		if (NeedUpdateCurrentFriendsList)
		{
			StartCoroutine(UpdateCurrentFriendsList());
		}
		_isWindowInStartState = false;
	}

	private string GetItemFromCurrentState(int index)
	{
		switch (currentWindowState)
		{
		case WindowState.friendList:
			if (index < localFriendsList.Length)
			{
				return localFriendsList[index];
			}
			if (index - localFriendsList.Length < myFriendsList.Length)
			{
				return myFriendsList[index - localFriendsList.Length];
			}
			return string.Empty;
		case WindowState.chat:
			if (index < myFriendsList.Length)
			{
				return myFriendsList[index];
			}
			return string.Empty;
		case WindowState.inbox:
			if (index < inviteFriendsList.Length)
			{
				return inviteFriendsList[index];
			}
			return string.Empty;
		case WindowState.findFriends:
			if (index < findFriendsList.Length && !_isFriendsMax)
			{
				return findFriendsList[index];
			}
			return string.Empty;
		default:
			return null;
		}
	}

	private string GetLocalPlayerItem(int index)
	{
		if (index < localFriendsList.Length)
		{
			return localFriendsList[index];
		}
		return string.Empty;
	}

	private int GetLenghtFromCurrentList()
	{
		switch (currentWindowState)
		{
		case WindowState.friendList:
			return ((localFriendsList != null) ? localFriendsList.Length : 0) + myFriendsList.Length;
		case WindowState.inbox:
			return (inviteFriendsList != null) ? inviteFriendsList.Length : 0;
		case WindowState.findFriends:
			return (findFriendsList != null) ? findFriendsList.Length : 0;
		default:
			return 0;
		}
	}

	private FriendItemPreviewType GetPreviewTypeForCurrentWindowState()
	{
		switch (currentWindowState)
		{
		case WindowState.friendList:
			return FriendItemPreviewType.view;
		case WindowState.inbox:
			return FriendItemPreviewType.inbox;
		case WindowState.findFriends:
			return FriendItemPreviewType.find;
		default:
			return FriendItemPreviewType.none;
		}
	}

	private UIWrapContent GetCurrentWrap()
	{
		switch (currentWindowState)
		{
		case WindowState.chat:
			return null;
		case WindowState.friendList:
			return friendsListWrap;
		case WindowState.inbox:
			return inviteFriendsWrap;
		case WindowState.findFriends:
			return findFriendsWrap;
		default:
			return null;
		}
	}

	public void UpdateCurrentFriendsArrayAndItems()
	{
		UpdateFriendsArray(currentWindowState);
		UIWrapContent currentWrap = GetCurrentWrap();
		if (!(currentWrap == null))
		{
			UpdateList(currentWrap, GetPreviewTypeForCurrentWindowState());
			UIScrollView component = currentWrap.transform.parent.GetComponent<UIScrollView>();
			int minForCurrentState = GetMinForCurrentState();
			int lenghtFromCurrentList = GetLenghtFromCurrentList();
			if (lenghtFromCurrentList > minForCurrentState && component.transform.localPosition.y + (float)(currentWrap.itemSize * minForCurrentState) > (float)(lenghtFromCurrentList * currentWrap.itemSize))
			{
				component.MoveRelative(Vector3.down * currentWrap.itemSize);
			}
		}
	}

	private void ResetWrapPosition(UIWrapContent wrap)
	{
		wrap.SortAlphabetically();
		wrap.transform.parent.GetComponent<UIScrollView>().ResetPosition();
	}

	private void UpdateFriendsArray(WindowState state, bool resetScroll = false)
	{
		switch (state)
		{
		case WindowState.friendList:
		{
			List<string> list2 = new List<string>();
			for (int j = 0; j < FriendsController.sharedController.friends.Count; j++)
			{
				string text2 = FriendsController.sharedController.friends[j];
				if (FriendsController.sharedController.friendsInfo.ContainsKey(text2))
				{
					list2.Add(text2);
				}
			}
			list2.Sort(SortFriendsByOnlineStatusAndClickJoin);
			myFriendsList = list2.ToArray();
			if (localFriendsList == null)
			{
				friendsListWrap.minIndex = myFriendsList.Length * -1;
			}
			else
			{
				friendsListWrap.minIndex = (myFriendsList.Length + localFriendsList.Length) * -1;
			}
			if (resetScroll)
			{
				friendsListWrap.transform.parent.GetComponent<UIScrollView>().ResetPosition();
			}
			break;
		}
		case WindowState.findFriends:
		{
			List<string> list3 = new List<string>();
			foreach (KeyValuePair<string, FriendsController.PossiblleOrigin> item in FriendsController.sharedController.getPossibleFriendsResult)
			{
				string key = item.Key;
				if (FriendsController.sharedController.profileInfo.ContainsKey(key))
				{
					list3.Add(key);
				}
			}
			findFriendsList = list3.ToArray();
			Array.Sort(findFriendsList, SortFriendsByFindOrigin);
			findFriendsWrap.minIndex = findFriendsList.Length * -1;
			if (resetScroll)
			{
				findFriendsWrap.transform.parent.GetComponent<UIScrollView>().ResetPosition();
			}
			break;
		}
		case WindowState.inbox:
		{
			List<string> list = new List<string>();
			for (int i = 0; i < FriendsController.sharedController.invitesToUs.Count; i++)
			{
				string text = FriendsController.sharedController.invitesToUs[i];
				if (FriendsController.sharedController.profileInfo.ContainsKey(text))
				{
					list.Add(text);
				}
			}
			inviteFriendsList = list.ToArray();
			inviteFriendsWrap.minIndex = inviteFriendsList.Length * -1;
			if (resetScroll)
			{
				inviteFriendsWrap.transform.parent.GetComponent<UIScrollView>().ResetPosition();
			}
			break;
		}
		case WindowState.chat:
			break;
		}
	}

	private void UpdateLocalFriendsArray(bool resetScroll)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, FriendsController.PossiblleOrigin> item in FriendsController.sharedController.getPossibleFriendsResult)
		{
			if (FriendsController.sharedController.profileInfo.ContainsKey(item.Key) && item.Value == FriendsController.PossiblleOrigin.Local)
			{
				list.Add(item.Key);
			}
		}
		localFriendsList = list.ToArray();
		friendsListWrap.minIndex = (myFriendsList.Length + localFriendsList.Length) * -1;
	}

	private IEnumerator UpdateCurrentFriendsList()
	{
		FriendsController friendsController = FriendsController.sharedController;
		if (friendsController == null)
		{
			yield break;
		}
		UpdateFriendsArray(WindowState.friendList);
		UpdateFriendsArray(WindowState.findFriends);
		UpdateFriendsArray(WindowState.inbox);
		UpdateLocalFriendsArray(false);
		if (!wrapsInit)
		{
			inviteFriendsWrap.onInitializeItem = OnIniviteItemWrap;
			friendsListWrap.onInitializeItem = OnFriendsItemWrap;
			findFriendsWrap.onInitializeItem = OnFindFriendsItemWrap;
			friendPreviewPrefab.transform.GetComponent<UIDragScrollView>().scrollView = inviteFriendsWrap.GetComponent<UIScrollView>();
			for (int f3 = 0; f3 < 6; f3++)
			{
				GameObject friendPreviewItem = NGUITools.AddChild(inviteFriendsWrap.gameObject, friendPreviewPrefab);
				friendPreviewItem.name = "FriendPreviewItem_" + f3;
			}
			inviteFriendsWrap.SortAlphabetically();
			friendPreviewPrefab.transform.GetComponent<UIDragScrollView>().scrollView = friendsListWrap.GetComponent<UIScrollView>();
			for (int f2 = 0; f2 < 6; f2++)
			{
				GameObject friendPreviewItem2 = NGUITools.AddChild(friendsListWrap.gameObject, friendPreviewPrefab);
				friendPreviewItem2.name = "FriendPreviewItem_" + f2;
			}
			friendsListWrap.SortAlphabetically();
			friendPreviewPrefab.transform.GetComponent<UIDragScrollView>().scrollView = findFriendsWrap.GetComponent<UIScrollView>();
			for (int f = 0; f < 6; f++)
			{
				GameObject friendPreviewItem3 = NGUITools.AddChild(findFriendsWrap.gameObject, friendPreviewPrefab);
				friendPreviewItem3.name = "FriendPreviewItem_" + f;
			}
			findFriendsWrap.SortAlphabetically();
			wrapsInit = true;
		}
		if (currentWindowState != WindowState.chat)
		{
			UpdateList(GetCurrentWrap(), GetPreviewTypeForCurrentWindowState());
		}
		NeedUpdateCurrentFriendsList = false;
	}

	private void OnIniviteItemWrap(GameObject go, int wrapInd, int realInd)
	{
		go.GetComponent<FriendPreviewItem>().myWrapIndex = Mathf.Abs(realInd);
		if (currentWindowState == WindowState.inbox)
		{
			updateItemInfo(go.GetComponent<FriendPreviewItem>(), FriendItemPreviewType.inbox);
		}
	}

	private void OnFriendsItemWrap(GameObject go, int wrapInd, int realInd)
	{
		go.GetComponent<FriendPreviewItem>().myWrapIndex = Mathf.Abs(realInd);
		if (currentWindowState == WindowState.friendList)
		{
			updateItemInfo(go.GetComponent<FriendPreviewItem>(), FriendItemPreviewType.view);
		}
	}

	private void OnFindFriendsItemWrap(GameObject go, int wrapInd, int realInd)
	{
		go.GetComponent<FriendPreviewItem>().myWrapIndex = Mathf.Abs(realInd);
		if (currentWindowState == WindowState.findFriends)
		{
			updateItemInfo(go.GetComponent<FriendPreviewItem>(), FriendItemPreviewType.find);
		}
	}

	private void OnLocalFriendsItemWrap(GameObject go, int wrapInd, int realInd)
	{
		go.GetComponent<FriendPreviewItem>().myWrapIndex = Mathf.Abs(realInd);
		if (currentWindowState == WindowState.friendList)
		{
			updateItemInfo(go.GetComponent<FriendPreviewItem>(), FriendItemPreviewType.find);
		}
	}

	private void updateItemInfo(FriendPreviewItem previewItem, FriendItemPreviewType _typeItems)
	{
		string itemFromCurrentState = GetItemFromCurrentState(previewItem.myWrapIndex);
		if (!string.IsNullOrEmpty(itemFromCurrentState))
		{
			if (_typeItems == FriendItemPreviewType.view && previewItem.myWrapIndex < localFriendsList.Length)
			{
				previewItem.FillData(itemFromCurrentState, FriendItemPreviewType.find);
			}
			else
			{
				previewItem.FillData(itemFromCurrentState, _typeItems);
			}
			previewItem.gameObject.SetActive(false);
			previewItem.gameObject.SetActive(true);
		}
		else if (previewItem.gameObject.activeSelf)
		{
			previewItem.gameObject.SetActive(false);
		}
	}

	private int GetMinForCurrentState()
	{
		return 4;
	}

	private void UpdateList(UIWrapContent _wrap, FriendItemPreviewType _typeItems)
	{
		if (_wrap == null)
		{
			return;
		}
		FriendPreviewItem[] componentsInChildren = _wrap.GetComponentsInChildren<FriendPreviewItem>(true);
		bool flag = false;
		int minForCurrentState = GetMinForCurrentState();
		int lenghtFromCurrentList = GetLenghtFromCurrentList();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (lenghtFromCurrentList != 0)
			{
				if (lenghtFromCurrentList > minForCurrentState)
				{
					componentsInChildren[i].GetComponent<UIDragScrollView>().enabled = true;
				}
				else
				{
					componentsInChildren[i].GetComponent<UIDragScrollView>().enabled = false;
					flag = true;
				}
			}
			updateItemInfo(componentsInChildren[i], _typeItems);
		}
		if (flag)
		{
			ResetWrapPosition(_wrap);
		}
	}

	private void UpdateFriendsListInterface()
	{
		UpdateFriendsListState();
		NeedUpdateCurrentFriendsList = true;
		if (currentWindowState == WindowState.inbox)
		{
			NeedUpdateCurrentFriendsList = true;
			UpdateFriendsInboxState();
		}
		else if (currentWindowState == WindowState.findFriends)
		{
			NeedUpdateCurrentFriendsList = true;
			if (!statusBar.IsFindFriendByIdStateActivate)
			{
				UpdateFindFriendsState();
			}
		}
		if (NeedUpdateCurrentFriendsList && (currentWindowState != WindowState.findFriends || !statusBar.IsFindFriendByIdStateActivate))
		{
			StartCoroutine(UpdateCurrentFriendsList());
		}
	}

	public void OnClickFriendListTab(UIToggle toggle)
	{
		if (toggle.value)
		{
			ButtonClickSound.TryPlayClick();
		}
		if (toggle.value)
		{
			HideInfoBox();
			NeedUpdateCurrentFriendsList = true;
			SetActiveFriendsListContainer();
		}
	}

	private void UpdateFindFriendsState()
	{
		FriendsController sharedController = FriendsController.sharedController;
		_isFriendsMax = FriendsController.IsFriendsMax();
		if (statusBar.IsFindFriendByIdStateActivate)
		{
			_isAnyFriendsDataExists = !_isFriendsMax;
		}
		else
		{
			_isAnyFriendsDataExists = FriendsController.IsPossibleFriendsDataExist() && !_isFriendsMax;
		}
		statusBar.UpdateFindFriendsState(_isFriendsMax);
		CheckShowEmptyStateTabLabel(_isAnyFriendsDataExists, _isFriendsMax);
		UpdateList(findFriendsWrap, FriendItemPreviewType.find);
	}

	private void SetActiveFindFriendsContainer()
	{
		currentWindowState = WindowState.findFriends;
		UpdateFindFriendsState();
	}

	public void OnClickFindFriendsTab(UIToggle toggle)
	{
		if (toggle.value)
		{
			ButtonClickSound.TryPlayClick();
		}
		if (toggle.value)
		{
			HideInfoBox();
			NeedUpdateCurrentFriendsList = true;
			if (statusBar.IsFindFriendByIdStateActivate)
			{
				UpdateFriendsArray(WindowState.findFriends);
				ResetWrapPosition(findFriendsWrap);
			}
			SetActiveFindFriendsContainer();
			statusBar.findFriendInput.value = string.Empty;
		}
	}

	private void SetActiveChatFriendsContainer()
	{
		currentWindowState = WindowState.chat;
		bool flag = FriendsController.IsFriendsDataExist();
		CheckShowEmptyStateTabLabel(flag, false);
		chatContainer.SetActive(flag);
		statusBar.OnClickChatTab();
	}

	public void OnClickFriendsChatTab(UIToggle toggle)
	{
		if (toggle.value)
		{
			ButtonClickSound.TryPlayClick();
		}
		if (toggle.value)
		{
			HideInfoBox();
			SetActiveChatFriendsContainer();
		}
	}

	private void UpdateFriendsInboxState()
	{
		UpdateFriendsArray(WindowState.inbox);
		UpdateList(inviteFriendsWrap, FriendItemPreviewType.inbox);
		_isAnyFriendsDataExists = FriendsController.IsFriendInvitesDataExist();
		_isFriendsMax = FriendsController.IsFriendsMax();
		statusBar.OnClickInboxFriendsTab(_isAnyFriendsDataExists, _isFriendsMax);
		CheckShowEmptyStateTabLabel(_isAnyFriendsDataExists, false);
	}

	private void SetActiveInboxFriendsContainer()
	{
		currentWindowState = WindowState.inbox;
		UpdateFriendsInboxState();
	}

	public void OnClickInboxFriendsTab(UIToggle toggle)
	{
		if (toggle.value)
		{
			ButtonClickSound.TryPlayClick();
		}
		if (toggle.value)
		{
			HideInfoBox();
			SetActiveInboxFriendsContainer();
		}
	}

	public void UpdateCurrentTabState()
	{
		if (currentWindowState == WindowState.friendList)
		{
			SetActiveFriendsListContainer();
		}
		else if (currentWindowState == WindowState.inbox)
		{
			SetActiveInboxFriendsContainer();
		}
		else if (currentWindowState == WindowState.findFriends)
		{
			SetActiveFindFriendsContainer();
		}
	}

	private int GetModeByInfo(Dictionary<string, string> onlineData)
	{
		switch (Convert.ToInt32(onlineData["game_mode"]))
		{
		case 6:
			return 1;
		case 7:
			return 2;
		default:
			return 0;
		}
	}

	private int SortFriendsByOnlineStatusAndClickJoin(string friend1, string friend2)
	{
		int num;
		if (!FriendsController.sharedController.onlineInfo.ContainsKey(friend1))
		{
			num = 3;
		}
		else
		{
			Dictionary<string, string> dictionary = FriendsController.sharedController.onlineInfo[friend1];
			num = ((dictionary != null) ? GetModeByInfo(dictionary) : 3);
		}
		int num2;
		if (!FriendsController.sharedController.onlineInfo.ContainsKey(friend2))
		{
			num2 = 3;
		}
		else
		{
			Dictionary<string, string> dictionary2 = FriendsController.sharedController.onlineInfo[friend2];
			num2 = ((dictionary2 != null) ? GetModeByInfo(dictionary2) : 3);
		}
		HashSet<string> hashSet = BattleInviteListener.Instance.GetFriendIds() as HashSet<string>;
		if (hashSet != null)
		{
			bool flag = hashSet.Contains(friend1);
			bool flag2 = hashSet.Contains(friend2);
			if (flag && !flag2)
			{
				return -1;
			}
			if (!flag && flag2)
			{
				return 1;
			}
		}
		if (num < num2)
		{
			return -1;
		}
		if (num > num2)
		{
			return 1;
		}
		FriendsController sharedController = FriendsController.sharedController;
		if (sharedController == null)
		{
			return 0;
		}
		DateTime dateLastClickJoinFriend = sharedController.GetDateLastClickJoinFriend(friend1);
		DateTime dateLastClickJoinFriend2 = sharedController.GetDateLastClickJoinFriend(friend2);
		if (dateLastClickJoinFriend < dateLastClickJoinFriend2)
		{
			return -1;
		}
		if (dateLastClickJoinFriend > dateLastClickJoinFriend2)
		{
			return 1;
		}
		return 0;
	}

	private int SortFriendsByFindOrigin(string player1, string player2)
	{
		int possibleFriendFindOrigin = (int)FriendsController.GetPossibleFriendFindOrigin(player1);
		int possibleFriendFindOrigin2 = (int)FriendsController.GetPossibleFriendFindOrigin(player2);
		if (possibleFriendFindOrigin < possibleFriendFindOrigin2)
		{
			return -1;
		}
		if (possibleFriendFindOrigin > possibleFriendFindOrigin2)
		{
			return 1;
		}
		return 0;
	}

	private void EventUpdateFriendsOnlineAndSorting()
	{
		if (currentWindowState == WindowState.friendList)
		{
			UpdateFriendsOnlineAndSorting(false);
		}
	}

	private void UpdateFriendsOnlineAndSorting(bool isNeedReposition)
	{
		if (UpdateFriendsOnlineEvent != null)
		{
			UpdateFriendsOnlineEvent();
		}
		if (currentWindowState == WindowState.friendList)
		{
			UpdateFriendsArray(WindowState.friendList);
			UpdateList(GetCurrentWrap(), GetPreviewTypeForCurrentWindowState());
		}
	}

	private void OnDisable()
	{
		FriendsController.FriendsUpdated -= UpdateFriendsListInterface;
		FriendsController.OnShowBoxProcessFriendsData = (Action)Delegate.Remove(FriendsController.OnShowBoxProcessFriendsData, new Action(ShowMessageBoxProcessingData));
		FriendsController.OnHideBoxProcessFriendsData = (Action)Delegate.Remove(FriendsController.OnHideBoxProcessFriendsData, new Action(HideInfoBox));
		FriendsController.UpdateFriendsInfoAction = (Action)Delegate.Remove(FriendsController.UpdateFriendsInfoAction, new Action(EventUpdateFriendsOnlineAndSorting));
		InfoWindowController.HideCurrentWindow();
		BattleInviteListener.Instance.ClearIncomingInvites();
	}

	private void OnDestroy()
	{
		FriendsController.sharedController.StopRefreshingOnline();
		FriendsController.DisposeProfile();
		Instance = null;
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	private void OnCloseProfileWindow(bool needUpdateFriendList)
	{
		if (needUpdateFriendList)
		{
			NeedUpdateCurrentFriendsList = true;
		}
		base.gameObject.SetActive(true);
		joinToFriendRoomPhoton.SetActive(true);
		if (_selectProfileItem != null)
		{
			_selectProfileItem.UpdateData();
		}
		UpdateCurrentTabState();
	}

	public void ShowProfileWindow(string friendId, FriendPreviewItem selectedItem)
	{
		_selectProfileItem = selectedItem;
		base.gameObject.SetActive(false);
		joinToFriendRoomPhoton.SetActive(false);
		FriendsController.ShowProfile(friendId, ProfileWindowType.friend, OnCloseProfileWindow);
	}

	public void SetActiveChatTab(string id)
	{
		if (PrivateChatController.sharedController != null)
		{
			PrivateChatController.sharedController.selectedPlayerID = id;
		}
		chatTab.Set(true);
		SetActiveChatFriendsContainer();
	}

	public IEnumerator ShowResultFindPlayer(string param)
	{
		HideMessageByEmptyStateTab();
		isNeedRebuildFindFriendsList = true;
		findFriendsList = new string[1] { string.Empty };
		UpdateList(findFriendsWrap, FriendItemPreviewType.find);
		FriendsController friendsController = FriendsController.sharedController;
		if (friendsController != null)
		{
			AnalyticsFacade.SendCustomEvent("Social", new Dictionary<string, object> { { "Search Friends", "Search" } });
			yield return StartCoroutine(friendsController.GetInfoByParamCoroutine(param));
		}
		if (statusBar.IsFindFriendByIdStateActivate)
		{
			List<string> findResultIds = friendsController.findPlayersByParamResult;
			if (findResultIds == null)
			{
				InfoWindowController.ShowInfoBox(LocalizationStore.Get("Key_1426"));
				CheckShowEmptyStateTabLabel(false, false);
				yield break;
			}
			findFriendsList = findResultIds.ToArray();
			findFriendsWrap.minIndex = findFriendsList.Length * -1;
			findFriendsWrap.SortAlphabetically();
			findFriendsWrap.transform.parent.GetComponent<UIScrollView>().ResetPosition();
			UpdateList(findFriendsWrap, FriendItemPreviewType.find);
		}
	}

	public void OnClickClearAllInboxButton()
	{
		ButtonClickSound.TryPlayClick();
		statusBar.clearAllInviteButton.isEnabled = false;
		FriendsController sharedController = FriendsController.sharedController;
		if (sharedController != null)
		{
			sharedController.ClearAllFriendsInvites();
		}
	}

	public static bool IsActiveFriendListTab()
	{
		if (Instance == null)
		{
			return false;
		}
		return Instance.currentWindowState == WindowState.friendList;
	}

	public static bool IsActiveFindFriendTab()
	{
		if (Instance == null)
		{
			return false;
		}
		return Instance.currentWindowState == WindowState.findFriends;
	}

	public void SetStartState()
	{
		StartCoroutine(SetStartStateCoroutine());
	}

	public void SetCancelState()
	{
		friendsTab.Set(false);
	}

	private IEnumerator SetStartStateCoroutine()
	{
		_isWindowInStartState = true;
		UIPanel windowPanel = base.gameObject.GetComponent<UIPanel>();
		windowPanel.alpha = 0.001f;
		yield return null;
		chatContainer.SetActive(false);
		friendsTab.Set(true);
		windowPanel.alpha = 1f;
	}
}

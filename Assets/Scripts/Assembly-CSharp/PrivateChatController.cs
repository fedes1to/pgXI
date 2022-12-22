using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Rilisoft;
using UnityEngine;

public class PrivateChatController : MonoBehaviour
{
	public static PrivateChatController sharedController;

	public string selectedPlayerID = string.Empty;

	public FriendPrevInChatItem selectedPlayerItem;

	public GameObject friendPreviewPrefab;

	public GameObject messagePrefab;

	public MyUIInput sendMessageInput;

	public UIButton sendMessageButton;

	public UIWrapContent friendsWrap;

	public UITable messageTable;

	public UIScrollView scrollMessages;

	public UIScrollView scrollFriends;

	public Transform smilesBtnContainer;

	public Transform leftInputAnchor;

	private UIPanel scrollFriensPanel;

	private UIPanel scrollPanel;

	private Transform scrollFriendsTransform;

	private Transform scrollTransform;

	private float heightMessage = 134f;

	private float heightFriends = 100f;

	private float stickerPosShow = -150f;

	private float stickerPosHide = -314f;

	private float speedHideOrShowStiker = 500f;

	public bool isShowSmilePanel;

	public Transform smilePanelTransform;

	public GameObject showSmileButton;

	public GameObject hideSmileButton;

	public GameObject buySmileButton;

	public bool isBuySmile;

	public GameObject buySmileBannerPrefab;

	private bool wrapsInit;

	private List<string> _friends = new List<string>();

	private List<string> friendsWithInfo = new List<string>();

	private List<ChatController.PrivateMessage> curListMessages = new List<ChatController.PrivateMessage>();

	private float keyboardSize;

	public Transform bottomAnchor;

	public UIPanel panelSmiles;

	private List<PrivateMessageItem> privateMessageItems = new List<PrivateMessageItem>();

	private bool isKeyboardVisible;

	private void Awake()
	{
		heightFriends = friendsWrap.itemSize;
		scrollTransform = scrollMessages.transform;
		scrollFriendsTransform = scrollFriends.transform;
		scrollPanel = scrollMessages.GetComponent<UIPanel>();
		scrollFriensPanel = scrollFriends.GetComponent<UIPanel>();
		sharedController = this;
		if (sendMessageInput != null)
		{
			MyUIInput myUIInput = sendMessageInput;
			myUIInput.onKeyboardInter = (Action)Delegate.Combine(myUIInput.onKeyboardInter, new Action(SendMessageFromInput));
			MyUIInput myUIInput2 = sendMessageInput;
			myUIInput2.onKeyboardCancel = (Action)Delegate.Combine(myUIInput2.onKeyboardCancel, new Action(CancelSendPrivateMessage));
			MyUIInput myUIInput3 = sendMessageInput;
			myUIInput3.onKeyboardVisible = (Action)Delegate.Combine(myUIInput3.onKeyboardVisible, new Action(OnKeyboardVisible));
			MyUIInput myUIInput4 = sendMessageInput;
			myUIInput4.onKeyboardHide = (Action)Delegate.Combine(myUIInput4.onKeyboardHide, new Action(OnKeyboardHide));
		}
		smilePanelTransform.localPosition = new Vector3(smilePanelTransform.localPosition.x, stickerPosHide, smilePanelTransform.localPosition.z);
		isShowSmilePanel = false;
		isBuySmile = StickersController.IsBuyAnyPack();
		if (isBuySmile)
		{
			showSmileButton.SetActive(true);
			buySmileButton.SetActive(false);
		}
		else
		{
			showSmileButton.SetActive(false);
			buySmileButton.SetActive(true);
		}
		hideSmileButton.SetActive(false);
	}

	private void OnEnable()
	{
		FriendsController.FriendsUpdated += Start_UpdateFriendList;
		Start_UpdateFriendListCore(true);
		sendMessageInput.value = string.Empty;
		if (string.IsNullOrEmpty(selectedPlayerID) && _friends.Count > 0)
		{
			selectedPlayerID = _friends[0];
		}
		StartCoroutine(SetSelectedPlayerWithPause(selectedPlayerID));
	}

	private IEnumerator SetSelectedPlayerWithPause(string _playerId, bool updateToogleState = true)
	{
		yield return null;
		SetSelectedPlayer(selectedPlayerID, updateToogleState);
	}

	private void OnDisable()
	{
		OnKeyboardHide();
		HideSmilePannelOnClick();
		sendMessageInput.DeselectInput();
		FriendsController.FriendsUpdated -= Start_UpdateFriendList;
	}

	private void Start_UpdateFriendListCore(bool isUpdatePos)
	{
		StartCoroutine(UpdateFriendList(isUpdatePos));
	}

	private void Start_UpdateFriendList()
	{
		Start_UpdateFriendListCore(false);
	}

	private void OnDestroy()
	{
		FriendsController.FriendsUpdated -= Start_UpdateFriendList;
		sharedController = null;
		if (sendMessageInput != null)
		{
			MyUIInput myUIInput = sendMessageInput;
			myUIInput.onKeyboardInter = (Action)Delegate.Remove(myUIInput.onKeyboardInter, new Action(SendPrivateMessage));
			MyUIInput myUIInput2 = sendMessageInput;
			myUIInput2.onKeyboardCancel = (Action)Delegate.Remove(myUIInput2.onKeyboardCancel, new Action(CancelSendPrivateMessage));
			MyUIInput myUIInput3 = sendMessageInput;
			myUIInput3.onKeyboardVisible = (Action)Delegate.Remove(myUIInput3.onKeyboardVisible, new Action(OnKeyboardVisible));
			MyUIInput myUIInput4 = sendMessageInput;
			myUIInput4.onKeyboardHide = (Action)Delegate.Remove(myUIInput4.onKeyboardHide, new Action(OnKeyboardHide));
		}
	}

	private void onFriendItemWrap(GameObject go, int wrapInd, int realInd)
	{
		go.GetComponent<FriendPrevInChatItem>().myWrapIndex = Mathf.Abs(realInd);
		UpdateItemInfo(go.GetComponent<FriendPrevInChatItem>());
	}

	private void UpdateItemInfo(FriendPrevInChatItem previewItem)
	{
		if (_friends.Count > previewItem.myWrapIndex)
		{
			if (!previewItem.gameObject.activeSelf)
			{
				previewItem.gameObject.SetActive(true);
			}
			string text = _friends[previewItem.myWrapIndex];
			string text2 = text;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, object> value;
			if (!FriendsController.sharedController.friendsInfo.ContainsKey(text2) || !FriendsController.sharedController.friendsInfo[text2].TryGetValue<Dictionary<string, object>>("player", out value))
			{
				return;
			}
			foreach (KeyValuePair<string, object> item in value)
			{
				dictionary.Add(item.Key, Convert.ToString(item.Value));
			}
			previewItem.playerID = text2;
			previewItem.UpdateCountNewMessage();
			previewItem.nickLabel.text = dictionary["nick"];
			previewItem.rank.spriteName = "Rank_" + dictionary["rank"];
			previewItem.previewTexture.mainTexture = Tools.GetPreviewFromSkin(dictionary["skin"], Tools.PreviewType.Head);
			previewItem.GetComponent<UIToggle>().Set(selectedPlayerID == text2);
		}
		else if (previewItem.gameObject.activeSelf)
		{
			previewItem.gameObject.SetActive(false);
		}
	}

	private int SortByMessagesCount(string x, string y)
	{
		double num = 0.0;
		double num2 = 0.0;
		int num3 = 0;
		int num4 = 0;
		if (ChatController.privateMessages.ContainsKey(x))
		{
			for (int i = 0; i < ChatController.privateMessages[x].Count; i++)
			{
				double timeStamp = ChatController.privateMessages[x][i].timeStamp;
				if (timeStamp > num)
				{
					num = timeStamp;
				}
				if (!ChatController.privateMessages[x][i].isRead)
				{
					num3++;
				}
			}
		}
		if (ChatController.privateMessages.ContainsKey(y))
		{
			for (int j = 0; j < ChatController.privateMessages[y].Count; j++)
			{
				double timeStamp2 = ChatController.privateMessages[y][j].timeStamp;
				if (timeStamp2 > num2)
				{
					num2 = timeStamp2;
				}
				if (!ChatController.privateMessages[y][j].isRead)
				{
					num4++;
				}
			}
		}
		if (ChatController.privateMessagesForSend.ContainsKey(x))
		{
			for (int k = 0; k < ChatController.privateMessagesForSend[x].Count; k++)
			{
				double timeStamp3 = ChatController.privateMessagesForSend[x][k].timeStamp;
				if (timeStamp3 > num)
				{
					num = timeStamp3;
				}
			}
		}
		if (ChatController.privateMessagesForSend.ContainsKey(y))
		{
			for (int l = 0; l < ChatController.privateMessagesForSend[y].Count; l++)
			{
				double timeStamp4 = ChatController.privateMessagesForSend[y][l].timeStamp;
				if (timeStamp4 > num2)
				{
					num2 = timeStamp4;
				}
			}
		}
		return (num3 == num4) ? ((num <= num2) ? 1 : (-1)) : ((num3 < num4) ? 1 : (-1));
	}

	public void UpdateFriendItemsInfoAndSort()
	{
		_friends = new List<string>(friendsWithInfo);
		_friends.Sort(SortByMessagesCount);
		FriendPrevInChatItem[] componentsInChildren = friendsWrap.GetComponentsInChildren<FriendPrevInChatItem>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UpdateItemInfo(componentsInChildren[i]);
		}
	}

	public IEnumerator UpdateFriendList(bool isUpdatePos = false)
	{
		friendsWithInfo.Clear();
		for (int j = 0; j < FriendsController.sharedController.friends.Count; j++)
		{
			string _friend = FriendsController.sharedController.friends[j];
			if (FriendsController.sharedController.friendsInfo.ContainsKey(_friend))
			{
				friendsWithInfo.Add(_friend);
			}
		}
		if (!wrapsInit)
		{
			friendsWrap.onInitializeItem = onFriendItemWrap;
			friendPreviewPrefab.transform.GetComponent<UIDragScrollView>().scrollView = friendsWrap.GetComponent<UIScrollView>();
			for (int f = 0; f < 16; f++)
			{
				GameObject friendPreviewItem = NGUITools.AddChild(friendsWrap.gameObject, friendPreviewPrefab);
				friendPreviewItem.name = "FriendPreviewItem_" + f;
				friendPreviewItem.GetComponent<UIToggle>().group = 3;
			}
			scrollFriends.ResetPosition();
			friendsWrap.SortAlphabetically();
			wrapsInit = true;
		}
		_friends = new List<string>(friendsWithInfo);
		_friends.Sort(SortByMessagesCount);
		friendsWrap.minIndex = _friends.Count * -1;
		FriendPrevInChatItem[] previewItems = friendsWrap.GetComponentsInChildren<FriendPrevInChatItem>(true);
		for (int i = 0; i < previewItems.Length; i++)
		{
			UpdateItemInfo(previewItems[i]);
		}
		if (!string.IsNullOrEmpty(selectedPlayerID) && !FriendsController.sharedController.friends.Contains(selectedPlayerID))
		{
			selectedPlayerID = string.Empty;
			SetSelectedPlayer(selectedPlayerID);
			OnKeyboardHide();
			sendMessageInput.DeselectInput();
		}
		if (wrapsInit)
		{
			friendsWrap.SortAlphabetically();
			scrollFriends.ResetPosition();
		}
		yield return null;
	}

	public void SetSelectedPlayer(string _playerId, bool updateToogleState = true)
	{
		selectedPlayerItem = null;
		if (string.IsNullOrEmpty(_playerId))
		{
			sendMessageInput.gameObject.SetActive(false);
			showSmileButton.SetActive(false);
			buySmileButton.SetActive(false);
			hideSmileButton.SetActive(false);
		}
		else
		{
			if (!sendMessageInput.gameObject.activeSelf)
			{
				sendMessageInput.gameObject.SetActive(true);
			}
			showSmileButton.SetActive(isBuySmile && !isShowSmilePanel);
			buySmileButton.SetActive(!isBuySmile);
			hideSmileButton.SetActive(isBuySmile && isShowSmilePanel);
		}
		FriendPrevInChatItem[] componentsInChildren = friendsWrap.GetComponentsInChildren<FriendPrevInChatItem>(true);
		for (int i = 0; i < _friends.Count; i++)
		{
			if (_friends[i].Equals(_playerId))
			{
				float num = 42 * i;
				if (Mathf.Abs(scrollFriensPanel.clipOffset.y + 291f) > num)
				{
					float yPosition = num - (scrollFriends.transform.localPosition.y - 291f);
					MoveFriendWrapToPosition(yPosition);
				}
				if (Mathf.Abs(scrollFriensPanel.clipOffset.y + 291f) + scrollFriensPanel.baseClipRegion.w - 42f < num)
				{
					float yPosition2 = num - (scrollFriends.transform.localPosition.y - 291f + scrollFriensPanel.baseClipRegion.w - 50f);
					MoveFriendWrapToPosition(yPosition2);
				}
			}
		}
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			if (componentsInChildren[j].playerID.Equals(_playerId))
			{
				selectedPlayerItem = componentsInChildren[j];
			}
		}
		selectedPlayerID = _playerId;
		UpdateMessageForSelectedUsers(true);
	}

	private void MoveFriendWrapToPosition(float yPosition)
	{
		bool flag = yPosition < 0f;
		float num = Mathf.Abs(yPosition);
		int num2 = (int)Mathf.Floor(num / 42f);
		float num3 = num - (float)(42 * num2);
		for (int i = 0; i < num2; i++)
		{
			scrollFriends.MoveRelative(new Vector3(0f, (!flag) ? 42 : (-42), 0f));
		}
		scrollFriends.MoveRelative(new Vector3(0f, (!flag) ? num3 : (0f - num3), 0f));
		SpringPanel.Begin(scrollFriensPanel.gameObject, scrollFriensPanel.transform.localPosition, 100000f);
	}

	public void UpdateMessageForSelectedUsers(bool resetPosition = false)
	{
		UpdateMessageForSelectedUsersCoroutine(resetPosition);
	}

	private void UpdateMessageItemInfo(PrivateMessageItem messageItem)
	{
		if (curListMessages.Count > messageItem.myWrapIndex)
		{
			if (!messageItem.gameObject.activeSelf)
			{
				messageItem.gameObject.SetActive(true);
			}
			messageItem.transform.localPosition = new Vector3(0f, messageItem.transform.localPosition.y, messageItem.transform.localPosition.z);
			messageItem.SetWidth(Mathf.RoundToInt(scrollPanel.baseClipRegion.z));
			ChatController.PrivateMessage privateMessage = curListMessages[messageItem.myWrapIndex];
			messageItem.isRead = privateMessage.isRead;
			messageItem.timeStamp = privateMessage.timeStamp.ToString("F8", CultureInfo.InvariantCulture);
			if (privateMessage.playerIDFrom.Equals(FriendsController.sharedController.id))
			{
				messageItem.SetFon(true);
				messageItem.otherMessageLabel.text = string.Empty;
				if (privateMessage.message.Contains(Defs.SmileMessageSuffix))
				{
					messageItem.yourSmileSprite.spriteName = privateMessage.message.Substring(Defs.SmileMessageSuffix.Length);
					messageItem.yourMessageLabel.text = string.Empty;
					messageItem.yourMessageLabel.overflowMethod = UILabel.Overflow.ShrinkContent;
					messageItem.yourMessageLabel.height = 80;
					messageItem.yourMessageLabel.width = 80;
					messageItem.yourSmileSprite.gameObject.SetActive(true);
				}
				else
				{
					messageItem.yourMessageLabel.text = privateMessage.message;
					messageItem.yourMessageLabel.overflowMethod = UILabel.Overflow.ResizeHeight;
					messageItem.yourMessageLabel.width = Mathf.CeilToInt((float)messageItem.yourWidget.width * 0.8f);
					messageItem.yourSmileSprite.gameObject.SetActive(false);
				}
				if (privateMessage.isSending)
				{
					DateTime currentTimeByUnixTime = Tools.GetCurrentTimeByUnixTime((int)privateMessage.timeStamp + DateTimeOffset.Now.Offset.Hours * 3600);
					messageItem.yourTimeLabel.text = currentTimeByUnixTime.Day.ToString("D2") + "." + currentTimeByUnixTime.Month.ToString("D2") + "." + currentTimeByUnixTime.Year + " " + currentTimeByUnixTime.Hour + ":" + currentTimeByUnixTime.Minute.ToString("D2");
				}
				else
				{
					messageItem.yourTimeLabel.text = LocalizationStore.Get("Key_1556");
				}
			}
			else
			{
				messageItem.SetFon(false);
				messageItem.yourMessageLabel.text = string.Empty;
				if (privateMessage.message.Contains(Defs.SmileMessageSuffix))
				{
					messageItem.otherSmileSprite.spriteName = privateMessage.message.Substring(Defs.SmileMessageSuffix.Length);
					messageItem.otherMessageLabel.text = string.Empty;
					messageItem.otherMessageLabel.overflowMethod = UILabel.Overflow.ShrinkContent;
					messageItem.otherMessageLabel.height = 80;
					messageItem.otherMessageLabel.width = 80;
					messageItem.otherSmileSprite.gameObject.SetActive(true);
				}
				else
				{
					messageItem.otherMessageLabel.text = privateMessage.message;
					messageItem.otherMessageLabel.overflowMethod = UILabel.Overflow.ResizeHeight;
					messageItem.otherMessageLabel.width = Mathf.CeilToInt((float)messageItem.otherWidget.width * 0.8f);
					messageItem.otherSmileSprite.gameObject.SetActive(false);
				}
				DateTime currentTimeByUnixTime2 = Tools.GetCurrentTimeByUnixTime((int)privateMessage.timeStamp + DateTimeOffset.Now.Offset.Hours * 3600);
				messageItem.otherTimeLabel.text = currentTimeByUnixTime2.Day.ToString("D2") + "." + currentTimeByUnixTime2.Month.ToString("D2") + "." + currentTimeByUnixTime2.Year + " " + currentTimeByUnixTime2.Hour + ":" + currentTimeByUnixTime2.Minute.ToString("D2");
			}
		}
		else if (messageItem.gameObject.activeSelf)
		{
			messageItem.gameObject.SetActive(false);
		}
	}

	private void UpdateMessageForSelectedUsersCoroutine(bool resetPosition)
	{
		curListMessages.Clear();
		if (!string.IsNullOrEmpty(selectedPlayerID))
		{
			if (ChatController.privateMessages.ContainsKey(selectedPlayerID))
			{
				curListMessages.AddRange(ChatController.privateMessages[selectedPlayerID]);
			}
			if (ChatController.privateMessagesForSend.ContainsKey(selectedPlayerID))
			{
				curListMessages.AddRange(ChatController.privateMessagesForSend[selectedPlayerID]);
			}
		}
		curListMessages.Sort((ChatController.PrivateMessage x, ChatController.PrivateMessage y) => (x.timeStamp <= y.timeStamp) ? 1 : (-1));
		if (selectedPlayerItem != null)
		{
			UpdateItemInfo(selectedPlayerItem);
		}
		while (privateMessageItems.Count < curListMessages.Count)
		{
			GameObject gameObject = NGUITools.AddChild(messageTable.gameObject, messagePrefab);
			if (privateMessageItems.Count > 0)
			{
				gameObject.transform.position = privateMessageItems[0].transform.position;
			}
			gameObject.name = privateMessageItems.Count.ToString();
			gameObject.GetComponent<PrivateMessageItem>().myWrapIndex = privateMessageItems.Count;
			privateMessageItems.Add(gameObject.GetComponent<PrivateMessageItem>());
		}
		while (privateMessageItems.Count > curListMessages.Count)
		{
			UnityEngine.Object.Destroy(privateMessageItems[privateMessageItems.Count - 1].gameObject);
			privateMessageItems.RemoveAt(privateMessageItems.Count - 1);
			for (int i = 0; i < privateMessageItems.Count; i++)
			{
				privateMessageItems[i].gameObject.name = i.ToString();
				privateMessageItems[i].myWrapIndex = i;
			}
		}
		messageTable.onCustomSort = (Transform x, Transform y) => (int.Parse(x.name) > int.Parse(y.name)) ? 1 : (-1);
		for (int j = 0; j < privateMessageItems.Count; j++)
		{
			UpdateMessageItemInfo(privateMessageItems[j]);
		}
		messageTable.transform.localPosition = new Vector3(scrollPanel.baseClipRegion.x, messageTable.transform.localPosition.y, messageTable.transform.localPosition.z);
		messageTable.repositionNow = true;
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(RepositionNextFrame(resetPosition));
		}
	}

	private IEnumerator RepositionNextFrame(bool resetPosition)
	{
		yield return new WaitForEndOfFrame();
		messageTable.repositionNow = true;
		scrollMessages.ResetPosition();
	}

	private void Update()
	{
		if (!isKeyboardVisible)
		{
			panelSmiles.UpdateAnchors();
			smilePanelTransform.GetComponent<UISprite>().UpdateAnchors();
			leftInputAnchor.GetComponent<UIWidget>().UpdateAnchors();
		}
		if (isShowSmilePanel && smilePanelTransform.localPosition.y < stickerPosShow)
		{
			smilePanelTransform.localPosition = new Vector3(smilePanelTransform.localPosition.x, smilePanelTransform.localPosition.y + Time.deltaTime * speedHideOrShowStiker, smilePanelTransform.localPosition.z);
			scrollMessages.MoveRelative(Vector3.up * Time.deltaTime * speedHideOrShowStiker);
			if (smilePanelTransform.localPosition.y > stickerPosShow)
			{
				smilePanelTransform.localPosition = new Vector3(smilePanelTransform.localPosition.x, stickerPosShow, smilePanelTransform.localPosition.z);
				scrollMessages.ResetPosition();
				smilePanelTransform.gameObject.SetActive(false);
				smilePanelTransform.gameObject.SetActive(true);
			}
		}
		if (!isShowSmilePanel && smilePanelTransform.localPosition.y > stickerPosHide)
		{
			smilePanelTransform.localPosition = new Vector3(smilePanelTransform.localPosition.x, smilePanelTransform.localPosition.y - Time.deltaTime * speedHideOrShowStiker, smilePanelTransform.localPosition.z);
			scrollMessages.MoveRelative(Vector3.down * Time.deltaTime * speedHideOrShowStiker);
			if (smilePanelTransform.localPosition.y < stickerPosHide)
			{
				smilePanelTransform.localPosition = new Vector3(smilePanelTransform.localPosition.x, stickerPosHide, smilePanelTransform.localPosition.z);
				scrollMessages.ResetPosition();
				smilePanelTransform.gameObject.SetActive(false);
				smilePanelTransform.gameObject.SetActive(true);
			}
		}
		bool flag = string.IsNullOrEmpty(sendMessageInput.value);
		if (sendMessageButton.isEnabled == flag)
		{
			sendMessageButton.isEnabled = !flag;
		}
	}

	public void CancelSendPrivateMessage()
	{
		sendMessageInput.value = string.Empty;
	}

	public void OnKeyboardVisible()
	{
		if (!isKeyboardVisible)
		{
			isKeyboardVisible = true;
			keyboardSize = sendMessageInput.heightKeyboard;
			if (Application.isEditor)
			{
				keyboardSize = 200f;
			}
			bottomAnchor.localPosition = new Vector3(bottomAnchor.localPosition.x, bottomAnchor.localPosition.y + keyboardSize / Defs.Coef, bottomAnchor.localPosition.z);
			sendMessageButton.gameObject.SetActive(true);
			smilesBtnContainer.localPosition = new Vector3(-1f * smilesBtnContainer.localPosition.x, smilesBtnContainer.localPosition.y, smilesBtnContainer.localPosition.z);
			leftInputAnchor.localPosition = new Vector3(leftInputAnchor.localPosition.x - 162f, leftInputAnchor.localPosition.y, leftInputAnchor.localPosition.z);
			StartCoroutine(ResetpositionCoroutine());
		}
	}

	private IEnumerator ResetpositionCoroutine()
	{
		yield return null;
		scrollMessages.ResetPosition();
		smilePanelTransform.gameObject.SetActive(false);
		smilePanelTransform.gameObject.SetActive(true);
	}

	public void OnKeyboardHide()
	{
		if (isKeyboardVisible)
		{
			isKeyboardVisible = false;
			bottomAnchor.localPosition = new Vector3(bottomAnchor.localPosition.x, bottomAnchor.localPosition.y - keyboardSize / Defs.Coef, bottomAnchor.localPosition.z);
			sendMessageButton.gameObject.SetActive(false);
			smilesBtnContainer.localPosition = new Vector3(-1f * smilesBtnContainer.localPosition.x, smilesBtnContainer.localPosition.y, smilesBtnContainer.localPosition.z);
			leftInputAnchor.localPosition = new Vector3(leftInputAnchor.localPosition.x + 162f, leftInputAnchor.localPosition.y, leftInputAnchor.localPosition.z);
			StartCoroutine(ResetpositionCoroutine());
			smilePanelTransform.gameObject.SetActive(false);
			smilePanelTransform.gameObject.SetActive(true);
		}
	}

	public void BuySmileOnClick()
	{
		ButtonClickSound.TryPlayClick();
		buySmileBannerPrefab.SetActive(true);
		sendMessageInput.DeselectInput();
	}

	public void ShowSmilePannelOnClick()
	{
		if (ButtonClickSound.Instance != null)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		isShowSmilePanel = true;
		showSmileButton.SetActive(false);
		hideSmileButton.SetActive(true);
		scrollMessages.ResetPosition();
	}

	public void HideSmilePannelOnClick()
	{
		if (ButtonClickSound.Instance != null)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		HideSmilePannel();
	}

	public void HideSmilePannel()
	{
		isShowSmilePanel = false;
		if (isBuySmile)
		{
			showSmileButton.SetActive(true);
			buySmileButton.SetActive(false);
		}
		else
		{
			showSmileButton.SetActive(false);
			buySmileButton.SetActive(true);
		}
		hideSmileButton.SetActive(false);
	}

	public void SendSmile(string smile)
	{
		SendPrivateMessageCore(Defs.SmileMessageSuffix + smile);
		HideSmilePannel();
	}

	public void SendMessageFromInput()
	{
		SendPrivateMessage();
		if (isShowSmilePanel)
		{
			HideSmilePannel();
		}
	}

	public void SendPrivateMessage()
	{
		SendPrivateMessageCore(string.Empty);
	}

	public void SendPrivateMessageCore(string customMessage)
	{
		if (!string.IsNullOrEmpty(customMessage) || (!string.IsNullOrEmpty(sendMessageInput.value) && !sendMessageInput.value.Contains(Defs.SmileMessageSuffix)))
		{
			bool flag = !string.IsNullOrEmpty(customMessage);
			ChatController.PrivateMessage item = new ChatController.PrivateMessage(FriendsController.sharedController.id, (!flag) ? FilterBadWorld.FilterString(sendMessageInput.value) : customMessage, Tools.CurrentUnixTime + 10000000, false, true);
			if (!ChatController.privateMessagesForSend.ContainsKey(selectedPlayerID))
			{
				ChatController.privateMessagesForSend.Add(selectedPlayerID, new List<ChatController.PrivateMessage>());
			}
			ChatController.privateMessagesForSend[selectedPlayerID].Add(item);
			ChatController.SavePrivatMessageInPrefs();
			if (!flag)
			{
				sendMessageInput.value = string.Empty;
			}
			UpdateMessageForSelectedUsers(true);
			FriendsController.sharedController.GetFriendsData(false);
			if (selectedPlayerItem.myWrapIndex != 0)
			{
				_friends = new List<string>(friendsWithInfo);
				_friends.Sort(SortByMessagesCount);
				friendsWrap.SortAlphabetically();
				scrollFriends.ResetPosition();
			}
		}
	}
}

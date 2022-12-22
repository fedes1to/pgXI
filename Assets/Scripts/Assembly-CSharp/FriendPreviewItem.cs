using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class FriendPreviewItem : MonoBehaviour
{
	public string id;

	public UIWidget friendListButtonContainer;

	public UIWidget findFrinedsButtonContainer;

	public UIWidget inboxFriendsButtonContainer;

	public UIButton connectToRoomButton;

	public UIButton goToChat;

	public UILabel levelAndName;

	public UISprite level;

	public UIWidget detailInfoConatiner;

	public UILabel detailInfo;

	public UITexture avatarIcon;

	public UIWidget clanContainer;

	public UITexture clanIcon;

	public UILabel clanName;

	public UISprite playingIcon;

	public UISprite inFriendsIcon;

	public UISprite offlineIcon;

	public UISprite invitedBackground;

	public UIGrid playerDetailInfoGrid;

	[Header("Not connect button")]
	public UISprite notConnectIcon;

	public UILabel notConnectLabel;

	[Header("add friend button")]
	public UIButton addFriendButton;

	public UIWidget invitationAddSentContainer;

	public UIWidget friendAddContainer;

	public UIWidget selfAddContainer;

	[Header("inbox button")]
	public UIButton acceptInviteButton;

	public UIButton cancelInviteButton;

	[NonSerialized]
	public int FindOrigin;

	public int myWrapIndex;

	private FriendItemPreviewType _type;

	public GameObject inYourNetworkIcon;

	[SerializeField]
	private GameObject _callFriendButtonHolder;

	[SerializeField]
	private GameObject friendCallLabel;

	[SerializeField]
	private GameObject friendCalledLabel;

	public string blinkButtonSprite = "blue_btn";

	private string defaultButtonSprite;

	private bool invitedToBattle;

	private float nextBlinkTime;

	public int OnlineCodeStatus { get; private set; }

	private void Update()
	{
		if (invitedToBattle)
		{
			if (nextBlinkTime < Time.time)
			{
				connectToRoomButton.normalSprite = defaultButtonSprite;
				nextBlinkTime = Time.time + 1f;
			}
			else if (nextBlinkTime < Time.time + 0.5f)
			{
				connectToRoomButton.normalSprite = blinkButtonSprite;
			}
		}
		else
		{
			connectToRoomButton.normalSprite = defaultButtonSprite;
		}
	}

	private void Start()
	{
		defaultButtonSprite = connectToRoomButton.normalSprite;
		FriendsWindowController.UpdateFriendsOnlineEvent = (Action)Delegate.Combine(FriendsWindowController.UpdateFriendsOnlineEvent, new Action(UpdateOnline));
	}

	private Color[] FlipColorsHorizontally(Color[] colors, int width, int height)
	{
		Color[] array = new Color[colors.Length];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				array[i + width * j] = colors[width - i - 1 + width * j];
			}
		}
		return array;
	}

	public void SetSkin(string skinStr)
	{
		Texture mainTexture = avatarIcon.mainTexture;
		if (mainTexture != null && !mainTexture.name.Equals("dude") && !mainTexture.name.Equals("multi_skin_1"))
		{
			UnityEngine.Object.DestroyImmediate(mainTexture, true);
		}
		avatarIcon.mainTexture = Tools.GetPreviewFromSkin(skinStr, Tools.PreviewType.HeadAndBody);
	}

	public void HandleCallFriend()
	{
		string playerNameOrDefault = ProfileController.GetPlayerNameOrDefault();
		playerNameOrDefault = FilterBadWorld.FilterString(playerNameOrDefault);
		WWWForm wWWForm = new WWWForm();
		string value = string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion);
		wWWForm.AddField("app_version", value);
		wWWForm.AddField("player", FriendsController.sharedController.id ?? string.Empty);
		wWWForm.AddField("nickname", playerNameOrDefault ?? string.Empty);
		wWWForm.AddField("friend", id ?? string.Empty);
		if (Application.isEditor)
		{
			Debug.LogFormat("`HandleCallFriend()`: `{0}`", Encoding.UTF8.GetString(wWWForm.data, 0, wWWForm.data.Length));
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Authorization", FriendsController.HashForPush(wWWForm.data));
		WWW request = Tools.CreateWwwIfNotConnected("https://secure.pixelgunserver.com/invite_service", wWWForm, "FriendPreviewItem.HandleCallFriend()", dictionary);
		CoroutineRunner.Instance.StartCoroutine(WaitRequestResponse(request));
		AnalyticsStuff.LogBattleInviteSent();
		if (FriendsController.sharedController != null)
		{
			FriendsController.sharedController.SendInviteFightToPlayer(id);
		}
		BattleInviteListener.Instance.SetOutgoingInviteTimestamp(id, Time.realtimeSinceStartup);
		bool flag = BattleInviteListener.Instance.CallToFriendEnabled(id);
		RefreshCallFriendButton(true, flag);
	}

	private IEnumerator WaitRequestResponse(WWW request)
	{
		if (Application.isEditor)
		{
			while (!request.isDone)
			{
				yield return null;
			}
			if (!string.IsNullOrEmpty(request.error))
			{
				Debug.LogWarningFormat("Request failed: {0}", request.error);
			}
			else
			{
				Debug.LogFormat("Request succeeded: {0}", request.text);
			}
		}
	}

	private string GetLevelAndNameLabel(string level, string name)
	{
		return string.Format("[b]{0} {1}[/b]", level, name);
	}

	private void FillCommonAttrsByPlayerInfo()
	{
		Dictionary<string, object> fullPlayerDataById = FriendsController.GetFullPlayerDataById(id);
		Dictionary<string, object> value;
		if (fullPlayerDataById != null && fullPlayerDataById.TryGetValue<Dictionary<string, object>>("player", out value))
		{
			FillCommonAttrsByPlayerData(value);
		}
	}

	private void ResetPositionElementsDetailInfo(bool isPlayerInClan)
	{
		clanContainer.gameObject.SetActive(isPlayerInClan);
		playerDetailInfoGrid.Reposition();
	}

	private void FillCommonAttrsByPlayerData(Dictionary<string, object> playerData)
	{
		string text = Convert.ToString(playerData["nick"]);
		string text2 = Convert.ToString(playerData["rank"]);
		levelAndName.text = text;
		level.spriteName = "Rank_" + text2;
		if (playerData.ContainsKey("skin"))
		{
			string skin = playerData["skin"] as string;
			SetSkin(skin);
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, object> playerDatum in playerData)
		{
			dictionary.Add(playerDatum.Key, Convert.ToString(playerDatum.Value));
		}
		bool flag = dictionary.ContainsKey("clan_name") && !string.IsNullOrEmpty(dictionary["clan_name"]);
		ResetPositionElementsDetailInfo(flag);
		if (flag)
		{
			FillClanAttrs(dictionary);
		}
	}

	private void SetupFindStateButtons()
	{
		bool flag = FriendsController.IsAlreadySendInvitePlayer(id);
		bool flag2 = FriendsController.IsMyPlayerId(id);
		bool flag3 = FriendsController.IsPlayerOurFriend(id);
		bool active = !flag3 && !flag2 && !flag;
		addFriendButton.gameObject.SetActive(active);
		invitationAddSentContainer.gameObject.SetActive(flag);
		friendAddContainer.gameObject.SetActive(flag3);
		selfAddContainer.gameObject.SetActive(flag2);
	}

	private void ShowButtonsByTypePreview(FriendItemPreviewType typePreview)
	{
		friendListButtonContainer.gameObject.SetActive(typePreview == FriendItemPreviewType.view);
		bool flag = typePreview == FriendItemPreviewType.find;
		findFrinedsButtonContainer.gameObject.SetActive(flag);
		if (flag)
		{
			SetupFindStateButtons();
		}
		inboxFriendsButtonContainer.gameObject.SetActive(typePreview == FriendItemPreviewType.inbox);
	}

	private void HideDetailInfo()
	{
		detailInfoConatiner.gameObject.SetActive(false);
		playerDetailInfoGrid.Reposition();
	}

	public void FillData(string playerId, FriendItemPreviewType typeItem)
	{
		id = playerId;
		_type = typeItem;
		ShowButtonsByTypePreview(typeItem);
		FillCommonAttrsByPlayerInfo();
		inYourNetworkIcon.SetActive(false);
		switch (typeItem)
		{
		case FriendItemPreviewType.find:
			FindOrigin = (int)FriendsController.GetPossibleFriendFindOrigin(playerId);
			if (FindOrigin == 0)
			{
				HideDetailInfo();
			}
			else
			{
				SetStatusLabelFindOrigin((FriendsController.PossiblleOrigin)FindOrigin);
			}
			break;
		default:
			HideDetailInfo();
			break;
		case FriendItemPreviewType.view:
			break;
		}
		UpdateOnline();
	}

	private void FillClanAttrs(Dictionary<string, string> plDict)
	{
		if (plDict.ContainsKey("clan_logo") && !string.IsNullOrEmpty(plDict["clan_logo"]) && !plDict["clan_logo"].Equals("null"))
		{
			clanIcon.gameObject.SetActive(true);
			try
			{
				byte[] data = Convert.FromBase64String(plDict["clan_logo"]);
				Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoHeight, TextureFormat.ARGB32, false);
				texture2D.LoadImage(data);
				texture2D.filterMode = FilterMode.Point;
				texture2D.Apply();
				Texture mainTexture = clanIcon.mainTexture;
				clanIcon.mainTexture = texture2D;
				if (mainTexture != null)
				{
					UnityEngine.Object.DestroyImmediate(mainTexture, true);
				}
			}
			catch (Exception)
			{
				Texture mainTexture2 = clanIcon.mainTexture;
				clanIcon.mainTexture = null;
				if (mainTexture2 != null)
				{
					UnityEngine.Object.DestroyImmediate(mainTexture2, true);
				}
			}
		}
		else
		{
			clanIcon.gameObject.SetActive(false);
		}
		if (plDict.ContainsKey("clan_name") && !string.IsNullOrEmpty(plDict["clan_name"]) && !plDict["clan_name"].Equals("null"))
		{
			clanName.gameObject.SetActive(true);
			string text = plDict["clan_name"];
			if (text != null)
			{
				clanName.text = text;
			}
		}
		else
		{
			clanName.gameObject.SetActive(false);
		}
	}

	private void SetStatusLabelPlayerBusy()
	{
		if (!detailInfoConatiner.gameObject.activeSelf)
		{
			detailInfoConatiner.gameObject.SetActive(true);
			playerDetailInfoGrid.Reposition();
		}
		detailInfo.text = string.Format("[ff0000]{0}[-]", LocalizationStore.Get("Key_0576"));
		RefreshCallFriendButton(false);
	}

	private void SetStatusLabelPlayerPlaying(string gameModeName, string mapName)
	{
		if (!detailInfoConatiner.gameObject.activeSelf)
		{
			detailInfoConatiner.gameObject.SetActive(true);
			playerDetailInfoGrid.Reposition();
		}
		if (string.IsNullOrEmpty(mapName))
		{
			detailInfo.text = string.Format("[00aeff]{0}[-]", gameModeName);
		}
		else
		{
			detailInfo.text = string.Format("[77ef00]{0}: {1}[-]", gameModeName, mapName);
		}
		RefreshCallFriendButton(false);
	}

	private void SetStatusLabelFindOrigin(FriendsController.PossiblleOrigin findOrigin)
	{
		invitedBackground.gameObject.SetActive(false);
		invitedToBattle = false;
		if (!detailInfoConatiner.gameObject.activeSelf)
		{
			detailInfoConatiner.gameObject.SetActive(true);
			playerDetailInfoGrid.Reposition();
		}
		switch (findOrigin)
		{
		case FriendsController.PossiblleOrigin.None:
			return;
		case FriendsController.PossiblleOrigin.Local:
			detailInfo.text = string.Format("[ffe400]{0}[-]", LocalizationStore.Get("Key_1569"));
			inYourNetworkIcon.SetActive(true);
			break;
		case FriendsController.PossiblleOrigin.Facebook:
			detailInfo.text = string.Format("[00aeff]{0}[-]", LocalizationStore.Get("Key_1570"));
			break;
		case FriendsController.PossiblleOrigin.RandomPlayer:
			detailInfo.text = string.Format("[77ef00]{0}[-]", LocalizationStore.Get("Key_1571"));
			break;
		}
		RefreshCallFriendButton(false);
	}

	private void SetStateButtonConnectContainer(bool isCanConnect, string conditionNotConnect)
	{
		connectToRoomButton.gameObject.SetActive(isCanConnect);
		notConnectLabel.gameObject.SetActive(!isCanConnect);
		notConnectLabel.text = conditionNotConnect;
		bool flag = BattleInviteListener.Instance.CallToFriendEnabled(id);
		RefreshCallFriendButton(!isCanConnect && FriendsController.sharedController.friends.Contains(id), flag);
		HashSet<string> hashSet = BattleInviteListener.Instance.GetFriendIds() as HashSet<string>;
		invitedToBattle = hashSet.Contains(id);
		invitedBackground.gameObject.SetActive(invitedToBattle);
	}

	private void SetOfflineStatePreview()
	{
		SetStatusLabelPlayerBusy();
		detailInfo.text = string.Empty;
		bool flag = BattleInviteListener.Instance.CallToFriendEnabled(id);
		OnlineCodeStatus = 3;
		SetStateButtonConnectContainer(false, LocalizationStore.Get("Key_1577"));
	}

	private void RefreshCallFriendButton(bool active)
	{
		if (!(_callFriendButtonHolder == null))
		{
			_callFriendButtonHolder.SetActive(active);
		}
	}

	private void RefreshCallFriendButton(bool active, bool enabled)
	{
		if (!(_callFriendButtonHolder == null))
		{
			_callFriendButtonHolder.SetActive(active);
			UIButton componentInChildren = _callFriendButtonHolder.GetComponentInChildren<UIButton>();
			if (!(componentInChildren == null))
			{
				friendCallLabel.SetActive(enabled);
				friendCalledLabel.SetActive(!enabled);
				componentInChildren.isEnabled = enabled;
			}
		}
	}

	private void UpdateOnline()
	{
		if (_type == FriendItemPreviewType.find)
		{
			return;
		}
		if (!FriendsController.sharedController.onlineInfo.ContainsKey(id))
		{
			SetOfflineStatePreview();
			return;
		}
		Dictionary<string, string> onlineData = FriendsController.sharedController.onlineInfo[id];
		FriendsController.ResultParseOnlineData resultParseOnlineData = FriendsController.ParseOnlineData(onlineData);
		if (resultParseOnlineData == null)
		{
			SetOfflineStatePreview();
			return;
		}
		SetStatusLabelPlayerPlaying(resultParseOnlineData.GetGameModeName(), resultParseOnlineData.GetMapName());
		SetStateButtonConnectContainer(resultParseOnlineData.IsCanConnect, resultParseOnlineData.GetNotConnectConditionShortString());
		OnlineCodeStatus = (int)resultParseOnlineData.GetOnlineStatus();
	}

	private void OnDestroy()
	{
		FriendsWindowController.UpdateFriendsOnlineEvent = (Action)Delegate.Remove(FriendsWindowController.UpdateFriendsOnlineEvent, new Action(UpdateOnline));
	}

	private void CallbackFriendAddRequest(bool isComplete, bool isRequestExist)
	{
		addFriendButton.enabled = true;
		InfoWindowController.CheckShowRequestServerInfoBox(isComplete, isRequestExist);
		if (isComplete)
		{
			SetupFindStateButtons();
		}
	}

	public void OnClickAddFriend()
	{
		if (!string.IsNullOrEmpty(id))
		{
			ButtonClickSound.TryPlayClick();
			addFriendButton.enabled = false;
			bool flag = FriendsWindowController.Instance.Map((FriendsWindowController fwc) => fwc.statusBar).Map((FriendsWindowStatusBar s) => s.IsFindFriendByIdStateActivate);
			string value = ((!flag) ? string.Format("Find Friends: {0}", (FriendsController.PossiblleOrigin)FindOrigin) : "Search");
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Added Friends", value);
			dictionary.Add("Deleted Friends", "Add");
			Dictionary<string, object> dictionary2 = dictionary;
			if (flag)
			{
				dictionary2.Add("Search Friends", "Add");
			}
			FriendsController.SendFriendshipRequest(id, dictionary2, CallbackFriendAddRequest);
		}
	}

	public void OnClickConnectToFriendRoom()
	{
		ButtonClickSound.TryPlayClick();
		FriendsController.JoinToFriendRoom(id);
		if (invitedToBattle)
		{
			AnalyticsStuff.LogBattleInviteAccepted();
		}
	}

	public void OnClickGoTohatButton()
	{
		FriendsWindowController.Instance.SetActiveChatTab(id);
	}

	private void OnCompleteAcceptInviteAction(bool isComplete)
	{
		InfoWindowController.CheckShowRequestServerInfoBox(isComplete, false);
		acceptInviteButton.isEnabled = true;
		cancelInviteButton.isEnabled = true;
		if (isComplete)
		{
			AnalyticsFacade.SendCustomEvent("Social", new Dictionary<string, object> { { "Friend Requests", "Accepted" } });
			FriendsWindowController.Instance.UpdateCurrentTabState();
		}
	}

	private void OnCompletetRejectInviteAction(bool isComplete)
	{
		InfoWindowController.CheckShowRequestServerInfoBox(isComplete, false);
		acceptInviteButton.isEnabled = true;
		cancelInviteButton.isEnabled = true;
		if (isComplete)
		{
			AnalyticsFacade.SendCustomEvent("Social", new Dictionary<string, object> { { "Friend Requests", "Rejected" } });
			FriendsWindowController.Instance.UpdateCurrentTabState();
		}
	}

	public void OnClickAcceptButton()
	{
		ButtonClickSound.TryPlayClick();
		acceptInviteButton.isEnabled = false;
		cancelInviteButton.isEnabled = false;
		if (FriendsController.IsFriendsMax())
		{
			InfoWindowController.ShowInfoBox(LocalizationStore.Get("Key_1424"));
		}
		else
		{
			FriendsController.sharedController.AcceptInvite(id, OnCompleteAcceptInviteAction);
		}
	}

	public void OnClickDeclineButton()
	{
		acceptInviteButton.isEnabled = false;
		cancelInviteButton.isEnabled = false;
		ButtonClickSound.TryPlayClick();
		FriendsController.sharedController.RejectInvite(id, OnCompletetRejectInviteAction);
	}

	public void OnClick()
	{
		ButtonClickSound.TryPlayClick();
		FriendsWindowController.Instance.ShowProfileWindow(id, this);
	}

	public void UpdateData()
	{
		if (!string.IsNullOrEmpty(id))
		{
			FillData(id, _type);
		}
	}
}

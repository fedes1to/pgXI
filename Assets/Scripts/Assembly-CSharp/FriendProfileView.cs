using System;
using System.Collections;
using System.Reflection;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class FriendProfileView : MonoBehaviour
{
	private const string DefaultStatisticString = "-";

	public Transform pers;

	public GameObject[] bootsPoint;

	private CharacterInterface characterInterface;

	public UISprite rankSprite;

	public UILabel friendCountLabel;

	public UILabel friendLocationLabel;

	public UILabel friendGameModeLabel;

	public UILabel friendNameLabel;

	public UILabel survivalScoreLabel;

	public UILabel winCountLabel;

	public UILabel totalWinCountLabel;

	public UILabel clanName;

	public UILabel friendIdLabel;

	public UILabel[] titlesLabel;

	public UITexture clanLogo;

	[Header("Online state settings")]
	public UILabel inFriendStateLabel;

	[Header("Online state settings")]
	public UILabel offlineStateLabel;

	[Header("Online state settings")]
	public UILabel playingStateLabel;

	public UISprite inFriendState;

	public UISprite offlineState;

	public UISprite playingState;

	public GameObject playingStateInfoContainer;

	[Header("Buttons settings")]
	public UIButton backButton;

	public UIButton joinButton;

	public UIButton sendMyIdButton;

	public UIButton chatButton;

	public UIButton inviteToClanButton;

	public UIButton addFriendButton;

	public UIButton removeFriendButton;

	public UITable buttonAlignContainer;

	public UILabel addOrRemoveButtonLabel;

	public UISprite notConnectJoinButtonSprite;

	public UISprite addFrienButtonSentState;

	public UISprite addClanButtonSentState;

	private IDisposable _backSubscription;

	private bool _escapePressed;

	private float lastTime;

	private float idleTimerLastTime;

	private readonly Lazy<Rect> _touchZone = new Lazy<Rect>(() => new Rect(0f, 0.1f * (float)Screen.height, 0.5f * (float)Screen.width, 0.8f * (float)Screen.height));

	public GameObject characterModel
	{
		get
		{
			return characterInterface.gameObject;
		}
	}

	public bool IsCanConnectToFriend { get; set; }

	public string FriendLocation { get; set; }

	public int FriendCount { get; set; }

	public string FriendName { get; set; }

	public OnlineState Online { get; set; }

	public int Rank { get; set; }

	public int SurvivalScore { get; set; }

	public string Username { get; set; }

	public int WinCount { get; set; }

	public int TotalWinCount { get; set; }

	public string FriendGameMode { get; set; }

	public string FriendId { get; set; }

	public string NotConnectCondition { get; set; }

	public event Action BackButtonClickEvent;

	public event Action JoinButtonClickEvent;

	public event Action CopyMyIdButtonClickEvent;

	public event Action ChatButtonClickEvent;

	public event Action AddButtonClickEvent;

	public event Action RemoveButtonClickEvent;

	public event Action InviteToClanButtonClickEvent;

	public event Action UpdateRequested;

	public void Reset()
	{
		IsCanConnectToFriend = false;
		FriendLocation = string.Empty;
		FriendCount = 0;
		FriendName = string.Empty;
		Online = ((!FriendsController.IsPlayerOurFriend(FriendId)) ? OnlineState.none : OnlineState.offline);
		Rank = 0;
		SurvivalScore = 0;
		Username = string.Empty;
		WinCount = 0;
		if (characterModel != null)
		{
			Texture texture = Resources.Load<Texture>(ResPath.Combine(Defs.MultSkinsDirectoryName, "multi_skin_1"));
			if (texture != null)
			{
				characterInterface.SetSkin(texture);
			}
		}
		SetOnlineState(Online);
		characterInterface.RemoveBoots();
		characterInterface.RemoveHat();
		characterInterface.RemoveMask();
		characterInterface.RemoveCape();
		characterInterface.RemoveArmor();
		SetEnableAddButton(true);
		SetEnableInviteClanButton(true);
	}

	public void SetBoots(string name)
	{
		characterInterface.UpdateBoots(name);
	}

	private void SetOnlineState(OnlineState onlineState)
	{
		bool isStateOffline = onlineState == OnlineState.offline;
		bool isStateInFriends = onlineState == OnlineState.inFriends;
		bool isStatePlaying = onlineState == OnlineState.playing;
		offlineStateLabel.Do(delegate(UILabel l)
		{
			l.gameObject.SetActive(isStateOffline);
		});
		inFriendStateLabel.Do(delegate(UILabel l)
		{
			l.gameObject.SetActive(isStateInFriends);
		});
		playingStateLabel.Do(delegate(UILabel l)
		{
			l.gameObject.SetActive(isStatePlaying);
		});
		offlineState.Do(delegate(UISprite l)
		{
			l.gameObject.SetActive(isStateOffline);
		});
		inFriendState.Do(delegate(UISprite l)
		{
			l.gameObject.SetActive(isStateInFriends);
		});
		playingState.Do(delegate(UISprite l)
		{
			l.gameObject.SetActive(isStatePlaying);
		});
		if (playingStateInfoContainer != null)
		{
			playingStateInfoContainer.SetActive(isStatePlaying);
		}
	}

	public void SetStockCape(string capeName)
	{
		if (string.IsNullOrEmpty(capeName))
		{
			Debug.LogWarning("Name of cape should not be empty.");
		}
		else
		{
			characterInterface.UpdateCape(capeName);
		}
	}

	public void SetCustomCape(byte[] capeBytes)
	{
		capeBytes = capeBytes ?? new byte[0];
		Texture2D texture2D = new Texture2D(12, 16, TextureFormat.ARGB32, false);
		texture2D.LoadImage(capeBytes);
		texture2D.filterMode = FilterMode.Point;
		texture2D.Apply();
		characterInterface.UpdateCape("cape_Custom", texture2D);
	}

	public void SetArmor(string armorName)
	{
		if (string.IsNullOrEmpty(armorName))
		{
			Debug.LogWarning("Name of armor should not be empty.");
		}
		else
		{
			characterInterface.UpdateArmor(armorName);
		}
	}

	public void SetHat(string hatName)
	{
		if (string.IsNullOrEmpty(hatName))
		{
			Debug.LogWarning("Name of hat should not be empty.");
		}
		else
		{
			characterInterface.UpdateHat(hatName);
		}
	}

	public void SetMask(string maskName)
	{
		if (string.IsNullOrEmpty(maskName))
		{
			Debug.LogWarning("Name of mask should not be empty.");
		}
		else
		{
			characterInterface.UpdateMask(maskName);
		}
	}

	public void SetSkin(byte[] skinBytes)
	{
		skinBytes = skinBytes ?? new byte[0];
		if (characterModel != null)
		{
			Func<byte[], Texture2D> func = delegate(byte[] bytes)
			{
				Texture2D texture2D = new Texture2D(64, 32)
				{
					filterMode = FilterMode.Point
				};
				texture2D.LoadImage(bytes);
				texture2D.Apply();
				return texture2D;
			};
			Texture2D skin = ((skinBytes.Length <= 0) ? Resources.Load<Texture2D>(ResPath.Combine(Defs.MultSkinsDirectoryName, "multi_skin_1")) : func(skinBytes));
			characterInterface.SetSkin(skin);
		}
	}

	private void Awake()
	{
		GameObject original = Resources.Load("Character_model") as GameObject;
		characterInterface = UnityEngine.Object.Instantiate(original).GetComponent<CharacterInterface>();
		characterInterface.GetComponent<CharacterInterface>().usePetFromStorager = false;
		characterInterface.transform.SetParent(pers, false);
		characterInterface.SetCharacterType(true, true, false);
		Player_move_c.SetLayerRecursively(characterInterface.gameObject, pers.gameObject.layer);
		HOTween.Init(true, true, true);
		HOTween.EnableOverwriteManager(true);
		Reset();
		if (backButton != null)
		{
			EventDelegate.Add(backButton.onClick, OnBackButtonClick);
		}
		if (joinButton != null)
		{
			EventDelegate.Add(joinButton.onClick, OnJoinButtonClick);
		}
		if (sendMyIdButton != null)
		{
			EventDelegate.Add(sendMyIdButton.onClick, OnSendMyIdButtonClick);
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				sendMyIdButton.gameObject.SetActive(false);
			}
		}
		if (chatButton != null)
		{
			EventDelegate.Add(chatButton.onClick, OnChatButtonClick);
		}
		if (addFriendButton != null)
		{
			EventDelegate.Add(addFriendButton.onClick, OnAddButtonClick);
		}
		if (removeFriendButton != null)
		{
			EventDelegate.Add(removeFriendButton.onClick, OnRemoveButtonClick);
		}
		if (inviteToClanButton != null)
		{
			EventDelegate.Add(inviteToClanButton.onClick, OnInviteToClanButtonClick);
		}
	}

	private void OnDisable()
	{
		StopCoroutine("RequestUpdate");
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void OnEnable()
	{
		StartCoroutine("RequestUpdate");
		idleTimerLastTime = Time.realtimeSinceStartup;
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(HandleEscape, "Friend Profile");
	}

	private void HandleEscape()
	{
		if (!InfoWindowController.IsActive)
		{
			_escapePressed = true;
		}
	}

	private void OnBackButtonClick()
	{
		if (this.BackButtonClickEvent != null)
		{
			this.BackButtonClickEvent();
		}
	}

	private void OnJoinButtonClick()
	{
		if (this.JoinButtonClickEvent != null)
		{
			this.JoinButtonClickEvent();
		}
	}

	private void OnSendMyIdButtonClick()
	{
		if (this.CopyMyIdButtonClickEvent != null)
		{
			this.CopyMyIdButtonClickEvent();
		}
	}

	private void OnChatButtonClick()
	{
		if (this.ChatButtonClickEvent != null)
		{
			this.ChatButtonClickEvent();
		}
	}

	private void OnAddButtonClick()
	{
		if (this.AddButtonClickEvent != null)
		{
			this.AddButtonClickEvent();
		}
	}

	private void OnRemoveButtonClick()
	{
		if (this.RemoveButtonClickEvent != null)
		{
			this.RemoveButtonClickEvent();
		}
	}

	private void OnInviteToClanButtonClick()
	{
		if (this.InviteToClanButtonClickEvent != null)
		{
			this.InviteToClanButtonClickEvent();
		}
	}

	[Obfuscation(Exclude = true)]
	private IEnumerator RequestUpdate()
	{
		while (true)
		{
			if (this.UpdateRequested != null)
			{
				this.UpdateRequested();
			}
			yield return new WaitForSeconds(5f);
		}
	}

	private void Update()
	{
		if (_escapePressed)
		{
			_escapePressed = false;
			OnBackButtonClick();
			return;
		}
		UpdateLightweight();
		float rotationRateForCharacterInMenues = RilisoftRotator.RotationRateForCharacterInMenues;
		Rect value = _touchZone.Value;
		RilisoftRotator.RotateCharacter(pers, rotationRateForCharacterInMenues, value, ref idleTimerLastTime, ref lastTime);
		if (Time.realtimeSinceStartup - idleTimerLastTime > ShopNGUIController.IdleTimeoutPers)
		{
			ReturnPersTonNormState();
		}
	}

	private void ReturnPersTonNormState()
	{
		HOTween.Kill(pers);
		Vector3 p_endVal = new Vector3(0f, -180f, 0f);
		idleTimerLastTime = Time.realtimeSinceStartup;
		HOTween.To(pers, 0.5f, new TweenParms().Prop("localRotation", new PlugQuaternion(p_endVal)).Ease(EaseType.Linear).OnComplete((TweenDelegate.TweenCallback)delegate
		{
			idleTimerLastTime = Time.realtimeSinceStartup;
		}));
	}

	private void UpdateLightweight()
	{
		if (friendLocationLabel != null)
		{
			friendLocationLabel.text = FriendLocation ?? string.Empty;
		}
		if (friendCountLabel != null)
		{
			friendCountLabel.text = ((FriendCount >= 0) ? FriendCount.ToString() : "-");
		}
		if (friendNameLabel != null)
		{
			friendNameLabel.text = FriendName ?? string.Empty;
		}
		SetOnlineState(Online);
		notConnectJoinButtonSprite.alpha = ((!IsCanConnectToFriend) ? 1f : 0f);
		if (rankSprite != null)
		{
			string text = "Rank_" + Rank;
			if (!rankSprite.spriteName.Equals(text))
			{
				rankSprite.spriteName = text;
			}
		}
		if (survivalScoreLabel != null)
		{
			survivalScoreLabel.text = ((SurvivalScore >= 0) ? SurvivalScore.ToString() : "-");
		}
		if (winCountLabel != null)
		{
			winCountLabel.text = ((WinCount >= 0) ? WinCount.ToString() : "-");
		}
		if (totalWinCountLabel != null)
		{
			totalWinCountLabel.text = ((TotalWinCount >= 0) ? TotalWinCount.ToString() : "-");
		}
		if (friendGameModeLabel != null)
		{
			friendGameModeLabel.text = FriendGameMode;
		}
		if (friendIdLabel != null)
		{
			friendIdLabel.text = FriendId;
		}
	}

	public void SetTitle(string titleText)
	{
		for (int i = 0; i < titlesLabel.Length; i++)
		{
			titlesLabel[i].text = titleText;
		}
	}

	private void SetActiveAndRepositionButtons(GameObject button, bool isActive)
	{
		bool activeSelf = button.activeSelf;
		button.SetActive(isActive);
		if (activeSelf != isActive)
		{
			buttonAlignContainer.Reposition();
			buttonAlignContainer.repositionNow = true;
		}
	}

	public void SetActiveChatButton(bool isActive)
	{
		SetActiveAndRepositionButtons(chatButton.gameObject, isActive);
	}

	public void SetActiveInviteButton(bool isActive)
	{
		SetActiveAndRepositionButtons(inviteToClanButton.gameObject, isActive);
	}

	public void SetActiveAddButton(bool isActive)
	{
		SetActiveAndRepositionButtons(addFriendButton.gameObject, isActive);
	}

	public void SetActiveAddButtonSent(bool isActive)
	{
		SetActiveAndRepositionButtons(addFrienButtonSentState.gameObject, isActive);
	}

	public void SetActiveAddClanButtonSent(bool isActive)
	{
		SetActiveAndRepositionButtons(addClanButtonSentState.gameObject, isActive);
	}

	public void SetActiveRemoveButton(bool isActive)
	{
		SetActiveAndRepositionButtons(removeFriendButton.gameObject, isActive);
	}

	public void SetEnableAddButton(bool enable)
	{
		if (addFriendButton != null)
		{
			addFriendButton.isEnabled = enable;
		}
	}

	public void SetEnableRemoveButton(bool enable)
	{
		if (removeFriendButton != null)
		{
			removeFriendButton.isEnabled = enable;
		}
	}

	public void SetEnableInviteClanButton(bool enable)
	{
		if (inviteToClanButton != null)
		{
			inviteToClanButton.isEnabled = enable;
		}
	}
}

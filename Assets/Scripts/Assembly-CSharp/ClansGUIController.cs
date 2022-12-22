using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prime31;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

public sealed class ClansGUIController : MonoBehaviour, IFriendsGUIController
{
	internal enum State
	{
		Default,
		Inbox,
		ProfileDetails
	}

	private const string ShownCreateClanRewardWindow = "ShownCreateClanRewardWindowKey";

	public static ClansGUIController sharedController;

	public static bool AtAddPanel;

	public static bool AtStatsPanel;

	public GameObject rewardCreateClanWindow;

	public UIWrapContent friendsGrid;

	public UIGrid addFriendsGrid;

	public JoinRoomFromFrends joinRoomFromFrends;

	public bool InClan;

	public GameObject NoClanPanel;

	public GameObject CreateClanPanel;

	public GameObject CreateClanButton;

	public GameObject EditLogoBut;

	public GameObject BackBut;

	public GameObject Left;

	public GameObject Right;

	public GameObject ClanName;

	public GameObject clanPanel;

	public GameObject editLogoInPreviewButton;

	public GameObject leaveButton;

	public GameObject addMembersButton;

	public GameObject noMembersLabel;

	public GameObject statisticsButton;

	public GameObject statisiticPanel;

	public GameObject deleteClanButton;

	public GameObject startPanel;

	public GameObject addInClanPanel;

	public GameObject NameIsUsedPanel;

	public GameObject CheckConnectionPanel;

	public GameObject topLevelObject;

	public UITexture logo;

	public UITexture previewLogo;

	public UILabel nameClanLabel;

	public UILabel countMembersLabel;

	public UILabel inputNameClanLabel;

	public UILabel tapToEdit;

	public UILabel clanIsFull;

	public UILabel changeClanResult;

	public GameObject receivingPlashka;

	public GameObject deleteClanDialog;

	public UIButton yesDelteClan;

	public UIButton noDeleteClan;

	public UIInput changeClanNameInput;

	private bool BlockGUI;

	private List<Texture2D> _logos = new List<Texture2D>();

	private int _currentLogoInd;

	private bool _inCoinsShop;

	private float timeOfLastSort;

	private readonly Lazy<UISprite[]> _newMessagesOverlays;

	private readonly Lazy<ClanIncomingInvitesController> _clanIncomingInvitesController;

	private FriendProfileController _friendProfileController;

	private IDisposable _backSubscription;

	public static bool ShowProfile;

	private bool _isCancellationRequested;

	private float _defendTime;

	internal State CurrentState { get; set; }

	public ClansGUIController()
	{
		_clanIncomingInvitesController = new Lazy<ClanIncomingInvitesController>(() => base.gameObject.GetComponent<ClanIncomingInvitesController>());
		_newMessagesOverlays = new Lazy<UISprite[]>(delegate
		{
			UISprite[] first = clanPanel.Map((GameObject c) => c.GetComponentsInChildren<UISprite>(true), new UISprite[0]);
			UISprite[] second = NoClanPanel.Map((GameObject c) => c.GetComponentsInChildren<UISprite>(true), new UISprite[0]);
			IEnumerable<UISprite> source = from s in first.Concat(second)
				where "NewMessages".Equals(s.name)
				select s;
			return source.ToArray();
		});
	}

	void IFriendsGUIController.Hide(bool h)
	{
		topLevelObject.SetActive(!h);
		ShowProfile = h;
	}

	public void HideRewardWindow()
	{
		rewardCreateClanWindow.SetActive(false);
	}

	private IEnumerator SetName(string nm)
	{
		yield return null;
		inputNameClanLabel.text = nm;
		inputNameClanLabel.parent.GetComponent<UIInput>().value = nm;
	}

	private void OnChangeClanName(string newName)
	{
		if (!nameClanLabel.text.Equals(newName) && !changeClanNameInput.isSelected)
		{
			nameClanLabel.text = newName;
		}
	}

	private void Start()
	{
		sharedController = this;
		RewardWindowBase component = rewardCreateClanWindow.GetComponent<RewardWindowBase>();
		FacebookController.StoryPriority priority = FacebookController.StoryPriority.Green;
		component.shareAction = delegate
		{
			FacebookController.PostOpenGraphStory("create", "clan", priority, new Dictionary<string, string> { { "mode", "create" } });
		};
		component.priority = priority;
		component.twitterStatus = () => "I’ve created a CLAN in @PixelGun3D! Join my team and get ready to fight! #pixelgun3d #pixelgun #pg3d #mobile #fps http://goo.gl/8fzL9u";
		component.EventTitle = "Created Clan";
		component.HasReward = false;
		UIInputRilisoft uIInputRilisoft = ((!(ClanName != null)) ? null : ClanName.GetComponent<UIInputRilisoft>());
		if (uIInputRilisoft != null)
		{
			uIInputRilisoft.value = LocalizationStore.Key_0589;
			uIInputRilisoft.onFocus = (UIInputRilisoft.OnFocus)Delegate.Combine(uIInputRilisoft.onFocus, new UIInputRilisoft.OnFocus(OnFocusCreateClanName));
			uIInputRilisoft.onFocusLost = (UIInputRilisoft.OnFocusLost)Delegate.Combine(uIInputRilisoft.onFocusLost, new UIInputRilisoft.OnFocusLost(onFocusLostCreateClanName));
		}
		_friendProfileController = new FriendProfileController(this);
		InClan = !string.IsNullOrEmpty(FriendsController.sharedController.ClanID);
		FriendsController friendsController = FriendsController.sharedController;
		friendsController.onChangeClanName = (FriendsController.OnChangeClanName)Delegate.Combine(friendsController.onChangeClanName, new FriendsController.OnChangeClanName(OnChangeClanName));
		if (InClan && !string.IsNullOrEmpty(FriendsController.sharedController.clanName))
		{
			nameClanLabel.text = FriendsController.sharedController.clanName;
			changeClanNameInput.value = nameClanLabel.text;
		}
		AtAddPanel = false;
		AtStatsPanel = false;
		timeOfLastSort = Time.realtimeSinceStartup;
		FriendsController.sharedController.StartRefreshingClanOnline();
		startPanel.SetActive(!FriendsController.readyToOperate);
		NoClanPanel.SetActive(FriendsController.readyToOperate && !InClan);
		clanPanel.SetActive(FriendsController.readyToOperate && InClan);
		if (GlobalGameController.Logos == null)
		{
			Texture2D[] array = Resources.LoadAll<Texture2D>("Clan_Previews/");
			if (array == null)
			{
				array = new Texture2D[0];
			}
			_logos.AddRange(array);
			StringComparer nameComparer = StringComparer.OrdinalIgnoreCase;
			_logos.Sort((Texture2D a, Texture2D b) => nameComparer.Compare(a.name, b.name));
			_currentLogoInd = 0;
		}
		else if (InClan)
		{
			if (GlobalGameController.LogoToEdit != null)
			{
				byte[] inArray = GlobalGameController.LogoToEdit.EncodeToPNG();
				FriendsController.sharedController.clanLogo = Convert.ToBase64String(inArray);
				FriendsController.sharedController.ChangeClanLogo();
			}
		}
		else
		{
			CreateClanPanel.SetActive(FriendsController.readyToOperate);
			_logos = GlobalGameController.Logos;
			StartCoroutine(SetName(GlobalGameController.TempClanName));
			if (GlobalGameController.LogoToEdit != null)
			{
				_logos.Add(GlobalGameController.LogoToEdit);
				_currentLogoInd = _logos.Count - 1;
			}
			else
			{
				_currentLogoInd = 0;
			}
		}
		if (InClan && !string.IsNullOrEmpty(FriendsController.sharedController.clanLogo))
		{
			try
			{
				byte[] data = Convert.FromBase64String(FriendsController.sharedController.clanLogo);
				Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoHeight, TextureFormat.ARGB32, false);
				texture2D.LoadImage(data);
				texture2D.filterMode = FilterMode.Point;
				texture2D.Apply();
				Texture mainTexture = previewLogo.mainTexture;
				previewLogo.mainTexture = texture2D;
				SkinsController.logoClanUserTexture = EditorTextures.CreateCopyTexture(previewLogo.mainTexture);
			}
			catch
			{
			}
		}
		GlobalGameController.Logos = null;
		GlobalGameController.LogoToEdit = null;
		GlobalGameController.TempClanName = null;
		if (_logos.Count > _currentLogoInd)
		{
			logo.mainTexture = _logos[_currentLogoInd];
			SkinsController.logoClanUserTexture = EditorTextures.CreateCopyTexture(logo.mainTexture);
		}
		if (CreateClanButton != null)
		{
			ButtonHandler component2 = CreateClanButton.GetComponent<ButtonHandler>();
			if (component2 != null)
			{
				component2.Clicked += HandleCreateClanClicked;
			}
		}
		if (EditLogoBut != null)
		{
			ButtonHandler component3 = EditLogoBut.GetComponent<ButtonHandler>();
			if (component3 != null)
			{
				component3.Clicked += HandleEditClicked;
			}
		}
		if (BackBut != null)
		{
			ButtonHandler component4 = BackBut.GetComponent<ButtonHandler>();
			if (component4 != null)
			{
				component4.Clicked += HandleBackClicked;
			}
		}
		if (Left != null)
		{
			ButtonHandler component5 = Left.GetComponent<ButtonHandler>();
			if (component5 != null)
			{
				component5.Clicked += HandleArrowClicked;
			}
		}
		if (Right != null)
		{
			ButtonHandler component6 = Right.GetComponent<ButtonHandler>();
			if (component6 != null)
			{
				component6.Clicked += HandleArrowClicked;
			}
		}
		if (addMembersButton != null)
		{
			ButtonHandler component7 = addMembersButton.GetComponent<ButtonHandler>();
			if (component7 != null)
			{
				component7.Clicked += HandleAddMembersClicked;
			}
		}
		if (deleteClanButton != null)
		{
			ButtonHandler component8 = deleteClanButton.GetComponent<ButtonHandler>();
			if (component8 != null)
			{
				component8.Clicked += HandleDeleteClanClicked;
			}
		}
		if (leaveButton != null)
		{
			ButtonHandler component9 = leaveButton.GetComponent<ButtonHandler>();
			if (component9 != null)
			{
				component9.Clicked += HandleLeaveClicked;
			}
		}
		if (editLogoInPreviewButton != null)
		{
			ButtonHandler component10 = editLogoInPreviewButton.GetComponent<ButtonHandler>();
			if (component10 != null)
			{
				component10.Clicked += HandleEditLogoInPreviewClicked;
			}
		}
		if (statisticsButton != null)
		{
			ButtonHandler component11 = statisticsButton.GetComponent<ButtonHandler>();
			if (component11 != null)
			{
				component11.Clicked += HandleStatisticsButtonClicked;
			}
		}
		if (yesDelteClan != null)
		{
			ButtonHandler component12 = yesDelteClan.GetComponent<ButtonHandler>();
			if (component12 != null)
			{
				component12.Clicked += HandleYesDelClanClicked;
			}
		}
		if (noDeleteClan != null)
		{
			ButtonHandler component13 = noDeleteClan.GetComponent<ButtonHandler>();
			if (component13 != null)
			{
				component13.Clicked += HandleNoDelClanClicked;
			}
		}
	}

	private void OnEnable()
	{
		FriendsController.ClanUpdated += UpdateGUI;
		UpdateGUI();
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(delegate
		{
			_isCancellationRequested = true;
		}, "Clans");
	}

	private void OnDisable()
	{
		FriendsController.ClanUpdated -= UpdateGUI;
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	public void UpdateGUI()
	{
		StartCoroutine(__UpdateGUI());
	}

	public void ChangeClanName()
	{
		string oldText = FriendsController.sharedController.clanName ?? string.Empty;
		if (string.IsNullOrEmpty(nameClanLabel.text))
		{
			nameClanLabel.text = oldText;
			StartCoroutine(ShowThisNameInUse());
		}
		else
		{
			FriendsController.sharedController.ChangeClanName(nameClanLabel.text, delegate
			{
				FriendsController.sharedController.clanName = nameClanLabel.text;
				BlockGUI = false;
			}, delegate(string error)
			{
				nameClanLabel.text = oldText;
				Debug.Log("error " + error);
				if (!string.IsNullOrEmpty(error))
				{
					if (error.Equals("fail"))
					{
						StartCoroutine(ShowThisNameInUse());
					}
					else
					{
						StartCoroutine(ShowCheckConnection());
					}
				}
				else
				{
					BlockGUI = false;
				}
			});
		}
		BlockGUI = true;
	}

	private void _SortFriendPreviews()
	{
		FriendPreview[] componentsInChildren = friendsGrid.GetComponentsInChildren<FriendPreview>(true);
		FriendPreview[] array = friendsGrid.GetComponentsInChildren<FriendPreview>(false);
		if (array == null)
		{
			array = new FriendPreview[0];
		}
		StringComparer nameComparer = StringComparer.Ordinal;
		Array.Sort(array, (FriendPreview fp1, FriendPreview fp2) => nameComparer.Compare(fp1.name, fp2.name));
		string text = null;
		float num = 0f;
		if (array.Length > 0)
		{
			text = array[0].gameObject.name;
			Transform parent = friendsGrid.transform.parent;
			if (parent != null)
			{
				UIPanel component = parent.GetComponent<UIPanel>();
				if (component != null)
				{
					num = array[0].transform.localPosition.x - component.clipOffset.x;
				}
			}
		}
		Array.Sort(componentsInChildren, delegate(FriendPreview fp1, FriendPreview fp2)
		{
			if (fp1.id == null || !FriendsController.sharedController.onlineInfo.ContainsKey(fp1.id))
			{
				return 1;
			}
			if (fp2.id == null || !FriendsController.sharedController.onlineInfo.ContainsKey(fp2.id))
			{
				return -1;
			}
			string s = FriendsController.sharedController.onlineInfo[fp1.id]["delta"];
			string s2 = FriendsController.sharedController.onlineInfo[fp1.id]["game_mode"];
			int num3 = int.Parse(s);
			int num4 = int.Parse(s2);
			int num5 = (((float)num3 > FriendsController.onlineDelta || (num4 > 99 && num4 / 100 != (int)ConnectSceneNGUIController.myPlatformConnect && num4 / 100 != 3)) ? 2 : ((num4 == -1) ? 1 : 0));
			if (FriendsController.sharedController.clanLeaderID != null && fp1.id.Equals(FriendsController.sharedController.clanLeaderID))
			{
				num5 = -1;
			}
			string s3 = FriendsController.sharedController.onlineInfo[fp2.id]["delta"];
			string s4 = FriendsController.sharedController.onlineInfo[fp2.id]["game_mode"];
			int num6 = int.Parse(s3);
			int num7 = int.Parse(s4);
			int num8 = (((float)num6 > FriendsController.onlineDelta || (num7 > 99 && num7 / 100 != (int)ConnectSceneNGUIController.myPlatformConnect && num7 / 100 != 3)) ? 2 : ((num7 <= -1) ? 1 : 0));
			if (FriendsController.sharedController.clanLeaderID != null && fp2.id.Equals(FriendsController.sharedController.clanLeaderID))
			{
				num8 = -1;
			}
			int result;
			int result2;
			return (num5 == num8 && int.TryParse(fp1.id, out result) && int.TryParse(fp2.id, out result2)) ? (result - result2) : (num5 - num8);
		});
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.name = i.ToString("D7");
		}
		friendsGrid.SortAlphabetically();
		friendsGrid.WrapContent();
		Transform transform = null;
		if (text != null)
		{
			FriendPreview[] array2 = componentsInChildren;
			foreach (FriendPreview friendPreview in array2)
			{
				if (friendPreview.name.Equals(text))
				{
					transform = friendPreview.transform;
					break;
				}
			}
		}
		if (transform == null && componentsInChildren.Length > 0 && friendsGrid.gameObject.activeInHierarchy)
		{
			transform = componentsInChildren[0].transform;
		}
		if (transform != null)
		{
			float num2 = transform.localPosition.x - num;
			Transform parent2 = friendsGrid.transform.parent;
			if (parent2 != null)
			{
				UIPanel component2 = parent2.GetComponent<UIPanel>();
				if (component2 != null)
				{
					component2.clipOffset = new Vector2(num2, component2.clipOffset.y);
					parent2.localPosition = new Vector3(0f - num2, parent2.localPosition.y, parent2.localPosition.z);
				}
			}
		}
		friendsGrid.WrapContent();
	}

	private IEnumerator __UpdateGUI()
	{
		try
		{
			byte[] _skinByte = Convert.FromBase64String(FriendsController.sharedController.clanLogo);
			Texture2D _skinNew = new Texture2D(Defs.LogoWidth, Defs.LogoHeight, TextureFormat.ARGB32, false);
			_skinNew.LoadImage(_skinByte);
			_skinNew.filterMode = FilterMode.Point;
			_skinNew.Apply();
			Texture oldTexture = previewLogo.mainTexture;
			previewLogo.mainTexture = _skinNew;
			SkinsController.logoClanUserTexture = EditorTextures.CreateCopyTexture(previewLogo.mainTexture);
		}
		catch (Exception ex)
		{
			Exception e = ex;
			Debug.LogWarning(e);
		}
		FriendPreview[] fps = friendsGrid.GetComponentsInChildren<FriendPreview>(true);
		List<FriendPreview> toRemove = new List<FriendPreview>();
		List<string> existingPreviews = new List<string>();
		FriendPreview[] array = fps;
		foreach (FriendPreview fp in array)
		{
			bool found = false;
			foreach (Dictionary<string, string> member in FriendsController.sharedController.clanMembers)
			{
				string _id;
				if (!member.TryGetValue("id", out _id) || !_id.Equals(fp.id))
				{
					continue;
				}
				found = true;
				fp.nm.text = member["nick"];
				break;
			}
			if (!found)
			{
				toRemove.Add(fp);
			}
			else if (fp.id != null)
			{
				existingPreviews.Add(fp.id);
			}
		}
		foreach (FriendPreview fp2 in toRemove)
		{
			fp2.transform.parent = null;
			UnityEngine.Object.Destroy(fp2.gameObject);
		}
		foreach (Dictionary<string, string> member2 in FriendsController.sharedController.clanMembers)
		{
			if (member2.ContainsKey("id") && !existingPreviews.Contains(member2["id"]) && !member2["id"].Equals(FriendsController.sharedController.id))
			{
				GameObject f = UnityEngine.Object.Instantiate(Resources.Load("Friend")) as GameObject;
				f.transform.parent = friendsGrid.transform;
				f.transform.localScale = new Vector3(1f, 1f, 1f);
				f.GetComponent<FriendPreview>().id = member2["id"];
				f.GetComponent<FriendPreview>().ClanMember = true;
				f.GetComponent<FriendPreview>().join.GetComponent<JoinRoomFromFrendsButton>().joinRoomFromFrends = joinRoomFromFrends;
				if (member2.ContainsKey("nick"))
				{
					f.GetComponent<FriendPreview>().nm.text = member2["nick"];
				}
			}
		}
		yield return null;
		timeOfLastSort = Time.realtimeSinceStartup;
		_SortFriendPreviews();
	}

	private void HandleArrowClicked(object sender, EventArgs e)
	{
		if ((sender as ButtonHandler).gameObject == Left)
		{
			_currentLogoInd--;
			if (_currentLogoInd < 0)
			{
				_currentLogoInd = _logos.Count - 1;
				if (_currentLogoInd < 0)
				{
					_currentLogoInd = 0;
				}
			}
		}
		else
		{
			_currentLogoInd++;
			if (_currentLogoInd >= _logos.Count)
			{
				_currentLogoInd = 0;
			}
		}
		logo.mainTexture = _logos[_currentLogoInd];
		SkinsController.logoClanUserTexture = EditorTextures.CreateCopyTexture(logo.mainTexture);
	}

	private void HandleBackClicked(object sender, EventArgs e)
	{
		_isCancellationRequested = true;
	}

	private void HandleCancellation()
	{
		if (_clanIncomingInvitesController.Value.Map((ClanIncomingInvitesController c) => c.inboxPanel).Map((GameObject p) => p.activeInHierarchy))
		{
			_clanIncomingInvitesController.Value.Do(delegate(ClanIncomingInvitesController c)
			{
				c.HandleBackFromInboxPressed();
			});
		}
		else if (deleteClanDialog.activeSelf)
		{
			deleteClanDialog.SetActive(false);
			DisableStatisticsPanel(false);
		}
		else if (!(_defendTime > 0f))
		{
			if (CreateClanPanel.activeInHierarchy)
			{
				CreateClanPanel.SetActive(false);
				NoClanPanel.SetActive(true);
			}
			else if (statisiticPanel.activeInHierarchy)
			{
				statisiticPanel.SetActive(false);
				AtStatsPanel = false;
				clanPanel.SetActive(true);
			}
			else if (addInClanPanel.activeInHierarchy)
			{
				HideAddPanel();
				clanPanel.SetActive(true);
			}
			else
			{
				MenuBackgroundMusic.keepPlaying = true;
				LoadConnectScene.textureToShow = null;
				LoadConnectScene.sceneToLoad = Defs.MainMenuScene;
				LoadConnectScene.noteToShow = null;
				Singleton<SceneLoader>.Instance.LoadScene(Defs.PromSceneName);
			}
		}
	}

	private void HideAddPanel()
	{
		addInClanPanel.SetActive(false);
		FriendsController.sharedController.StopRefreshingOnlineWithClanInfo();
		FriendsController.sharedController.StartRefreshingClanOnline();
		AtAddPanel = false;
		foreach (Transform item in addFriendsGrid.transform)
		{
			item.parent = null;
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	private void HandleEditClicked(object sender, EventArgs e)
	{
		GoToSM();
	}

	public void GoToSM()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("SkinEditorController"));
		SkinEditorController component = gameObject.GetComponent<SkinEditorController>();
		if (!(component != null))
		{
			return;
		}
		Action<string> backHandler = null;
		backHandler = delegate(string name)
		{
			MenuBackgroundMusic.sharedMusic.StopCustomMusicFrom(SkinEditorController.sharedController.gameObject);
			SkinEditorController.ExitFromSkinEditor -= backHandler;
			logo.mainTexture = EditorTextures.CreateCopyTexture(SkinsController.logoClanUserTexture);
			if (InClan)
			{
				Debug.Log("InClan");
				byte[] inArray = SkinsController.logoClanUserTexture.EncodeToPNG();
				FriendsController.sharedController.clanLogo = Convert.ToBase64String(inArray);
				FriendsController.sharedController.ChangeClanLogo();
				previewLogo.mainTexture = EditorTextures.CreateCopyTexture(SkinsController.logoClanUserTexture);
			}
			else if (!string.IsNullOrEmpty(name))
			{
				_logos.Add(logo.mainTexture as Texture2D);
				_currentLogoInd = _logos.Count - 1;
			}
			base.gameObject.SetActive(true);
		};
		SkinEditorController.ExitFromSkinEditor += backHandler;
		if (InClan)
		{
			SkinsController.logoClanUserTexture = EditorTextures.CreateCopyTexture((Texture2D)previewLogo.mainTexture);
		}
		else
		{
			SkinsController.logoClanUserTexture = EditorTextures.CreateCopyTexture((Texture2D)logo.mainTexture);
		}
		SkinEditorController.modeEditor = SkinEditorController.ModeEditor.LogoClan;
		SkinEditorController.currentSkin = EditorTextures.CreateCopyTexture((Texture2D)logo.mainTexture);
		gameObject.transform.parent = null;
		base.gameObject.SetActive(false);
	}

	public void FailedSendBuyClan()
	{
		FriendsController.sharedController.FailedSendNewClan -= FailedSendBuyClan;
		FriendsController.sharedController.ReturnNewIDClan -= ReturnIDNewClan;
	}

	public void ReturnIDNewClan(int _idNewClan)
	{
		FriendsController.sharedController.FailedSendNewClan -= FailedSendBuyClan;
		FriendsController.sharedController.ReturnNewIDClan -= ReturnIDNewClan;
		if (_idNewClan > 0)
		{
			BlockGUI = false;
			FriendsController.sharedController.ClanID = _idNewClan.ToString();
			FriendsController.sharedController.clanLeaderID = FriendsController.sharedController.id;
			Texture2D texture2D = logo.mainTexture as Texture2D;
			byte[] inArray = texture2D.EncodeToPNG();
			string clanLogo = Convert.ToBase64String(inArray);
			FriendsController.sharedController.clanLogo = clanLogo;
			Texture mainTexture = previewLogo.mainTexture;
			previewLogo.mainTexture = logo.mainTexture;
			SkinsController.logoClanUserTexture = EditorTextures.CreateCopyTexture(previewLogo.mainTexture);
			if (mainTexture != null)
			{
			}
			FriendsController.sharedController.clanName = inputNameClanLabel.text;
			nameClanLabel.text = FriendsController.sharedController.clanName;
			BuyNewClan();
			if ((FacebookController.FacebookSupported || TwitterController.TwitterSupported) && Storager.getInt("ShownCreateClanRewardWindowKey", false) == 0 && !Device.isPixelGunLow)
			{
				rewardCreateClanWindow.SetActive(true);
				Storager.setInt("ShownCreateClanRewardWindowKey", 1, false);
			}
		}
		else
		{
			StartCoroutine(ShowThisNameInUse());
		}
	}

	public IEnumerator ShowThisNameInUse()
	{
		NameIsUsedPanel.SetActive(true);
		yield return new WaitForSeconds(3f);
		NameIsUsedPanel.SetActive(false);
		BlockGUI = false;
	}

	public IEnumerator ShowCheckConnection()
	{
		CheckConnectionPanel.SetActive(true);
		yield return new WaitForSeconds(3f);
		CheckConnectionPanel.SetActive(false);
		BlockGUI = false;
	}

	public void BuyNewClan()
	{
		int clansPrice = Defs.ClansPrice;
		int @int = Storager.getInt("Coins", false);
		int val = @int - clansPrice;
		Storager.setInt("Coins", val, false);
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
			PlayerPrefs.Save();
		}
		CreateClanPanel.SetActive(false);
		InClan = true;
		ShowClanPanel();
	}

	private void HandleCreateClanClicked(object sender, EventArgs e)
	{
		Action act = null;
		act = delegate
		{
			CreateClanPanel.SetActive(true);
			coinsShop.thisScript.notEnoughCurrency = null;
			coinsShop.thisScript.onReturnAction = null;
			int clansPrice = Defs.ClansPrice;
			int @int = Storager.getInt("Coins", false);
			int num = @int - clansPrice;
			Action<string> showShop = null;
			showShop = delegate(string pressedbutton)
			{
				EtceteraAndroidManager.alertButtonClickedEvent -= showShop;
				if (!pressedbutton.Equals(Defs.CancelButtonTitle))
				{
					coinsShop.thisScript.notEnoughCurrency = "Coins";
					coinsShop.thisScript.onReturnAction = act;
					coinsShop.showCoinsShop();
				}
			};
			Texture2D texture2D = logo.mainTexture as Texture2D;
			byte[] inArray = texture2D.EncodeToPNG();
			string skinClan = Convert.ToBase64String(inArray);
			if (num >= 0)
			{
				if (inputNameClanLabel.text.Equals(string.Empty))
				{
					StartCoroutine(ShowThisNameInUse());
				}
				else
				{
					FriendsController.sharedController.SendCreateClan(FriendsController.sharedController.id, inputNameClanLabel.text, skinClan, ErrorHandler);
					FriendsController.sharedController.FailedSendNewClan += FailedSendBuyClan;
					FriendsController.sharedController.ReturnNewIDClan += ReturnIDNewClan;
				}
				BlockGUI = true;
			}
			else
			{
				showShop("Yes!");
			}
		};
		act();
	}

	private void ErrorHandler(string error)
	{
		FriendsController.sharedController.FailedSendNewClan -= FailedSendBuyClan;
		FriendsController.sharedController.ReturnNewIDClan -= ReturnIDNewClan;
		BlockGUI = false;
	}

	private void HandleEditLogoInPreviewClicked(object sender, EventArgs e)
	{
		GoToSM();
	}

	private void HandleLeaveClicked(object sender, EventArgs e)
	{
		InClan = false;
		NoClanPanel.SetActive(true);
		FriendsController.sharedController.ExitClan(null);
		FriendsController.sharedController.ClearClanData();
	}

	private void HandleAddMembersClicked(object sender, EventArgs e)
	{
		ShowAddMembersScreen();
	}

	internal void ShowAddMembersScreen()
	{
		clanPanel.SetActive(false);
		addInClanPanel.SetActive(true);
		FriendsController.sharedController.StartRefreshingOnlineWithClanInfo();
		FriendsController.sharedController.StopRefreshingClanOnline();
		AtAddPanel = true;
		StartCoroutine(FillAddMembers());
	}

	private void HandleDeleteClanClicked(object sender, EventArgs e)
	{
		deleteClanDialog.SetActive(true);
		DisableStatisticsPanel(true);
	}

	private void HandleYesDelClanClicked(object sender, EventArgs e)
	{
		deleteClanDialog.SetActive(false);
		DisableStatisticsPanel(false);
		InClan = false;
		statisiticPanel.SetActive(false);
		FriendsController.sharedController.DeleteClan();
		FriendsController.sharedController.ClearClanData();
	}

	private void HandleNoDelClanClicked(object sender, EventArgs e)
	{
		_isCancellationRequested = true;
	}

	private void DisableStatisticsPanel(bool disable)
	{
		BackBut.GetComponent<UIButton>().isEnabled = !disable;
		deleteClanButton.GetComponent<UIButton>().isEnabled = !disable;
	}

	private void HandleStatisticsButtonClicked(object sender, EventArgs e)
	{
		clanPanel.SetActive(false);
		statisiticPanel.SetActive(true);
		AtStatsPanel = true;
	}

	public void ShowClanPanel()
	{
		clanPanel.SetActive(true);
	}

	private IEnumerator FillAddMembers()
	{
		foreach (Transform child in addFriendsGrid.transform)
		{
			child.parent = null;
			UnityEngine.Object.Destroy(child.gameObject);
		}
		foreach (string friend in FriendsController.sharedController.friends)
		{
			Dictionary<string, object> playerInfo;
			if (!FriendsController.sharedController.playersInfo.TryGetValue(friend, out playerInfo))
			{
				continue;
			}
			object playerNode;
			if (playerInfo.TryGetValue("player", out playerNode))
			{
				Dictionary<string, string> playerDictionary = (playerNode as Dictionary<string, object>).Map((Dictionary<string, object> d) => d.ToDictionary((KeyValuePair<string, object> kv) => kv.Key, (KeyValuePair<string, object> kv) => Convert.ToString(kv.Value)));
				string clanCreatorId;
				if (playerDictionary.TryGetValue("clan_creator_id", out clanCreatorId) && clanCreatorId == FriendsController.sharedController.id)
				{
					continue;
				}
			}
			GameObject f = UnityEngine.Object.Instantiate(Resources.Load("Friend")) as GameObject;
			FriendPreview fp = f.GetComponent<FriendPreview>();
			f.transform.parent = addFriendsGrid.transform;
			f.transform.localScale = new Vector3(1f, 1f, 1f);
			fp.ClanInvite = true;
			fp.id = friend;
			fp.join.GetComponent<JoinRoomFromFrendsButton>().joinRoomFromFrends = joinRoomFromFrends;
			Dictionary<string, string> plDict = playerInfo.ToDictionary((KeyValuePair<string, object> kv) => kv.Key, (KeyValuePair<string, object> kv) => Convert.ToString(kv.Value));
			if (playerInfo.ContainsKey("nick"))
			{
				fp.nm.text = plDict["nick"];
			}
			if (playerInfo.ContainsKey("rank"))
			{
				string r = plDict["rank"];
				if (r.Equals("0"))
				{
					r = "1";
				}
				fp.rank.spriteName = "Rank_" + r;
			}
			if (playerInfo.ContainsKey("skin"))
			{
				fp.SetSkin(plDict["skin"]);
			}
			fp.FillClanAttrs(plDict);
		}
		yield return null;
		addFriendsGrid.Reposition();
	}

	private void Update()
	{
		if (startPanel.activeSelf != !FriendsController.readyToOperate)
		{
			startPanel.SetActive(!FriendsController.readyToOperate);
		}
		if (_isCancellationRequested)
		{
			HandleCancellation();
			_isCancellationRequested = false;
		}
		addMembersButton.SetActive(!string.IsNullOrEmpty(FriendsController.sharedController.id) && FriendsController.sharedController.clanLeaderID != null && FriendsController.sharedController.id.Equals(FriendsController.sharedController.clanLeaderID) && !BlockGUI && FriendsController.ClanDataSettted);
		previewLogo.transform.parent.GetComponent<UIButton>().isEnabled = addMembersButton.activeInHierarchy;
		tapToEdit.gameObject.SetActive(addMembersButton.activeInHierarchy);
		leaveButton.SetActive(!string.IsNullOrEmpty(FriendsController.sharedController.id) && FriendsController.sharedController.clanLeaderID != null && !FriendsController.sharedController.id.Equals(FriendsController.sharedController.clanLeaderID));
		deleteClanButton.SetActive(!string.IsNullOrEmpty(FriendsController.sharedController.id) && FriendsController.sharedController.clanLeaderID != null && FriendsController.sharedController.id.Equals(FriendsController.sharedController.clanLeaderID));
		changeClanNameInput.gameObject.SetActive(!string.IsNullOrEmpty(FriendsController.sharedController.id) && FriendsController.sharedController.clanLeaderID != null && FriendsController.sharedController.id.Equals(FriendsController.sharedController.clanLeaderID) && !BlockGUI);
		InClan = !string.IsNullOrEmpty(FriendsController.sharedController.ClanID);
		NoClanPanel.SetActive(FriendsController.readyToOperate && !InClan && !CreateClanPanel.activeInHierarchy && SkinEditorController.sharedController == null && CurrentState != State.Inbox && CurrentState != State.ProfileDetails);
		clanPanel.SetActive(FriendsController.readyToOperate && InClan && !AtAddPanel && !AtStatsPanel && !ShowProfile && CurrentState != State.Inbox && CurrentState != State.ProfileDetails);
		statisiticPanel.SetActive(FriendsController.readyToOperate && InClan && !AtAddPanel && AtStatsPanel);
		bool activeInHierarchy = addInClanPanel.activeInHierarchy;
		addInClanPanel.SetActive(FriendsController.readyToOperate && InClan && AtAddPanel && !AtStatsPanel && CurrentState != State.ProfileDetails);
		if (!InClan)
		{
			deleteClanDialog.SetActive(false);
			DisableStatisticsPanel(false);
		}
		if (clanPanel.activeInHierarchy)
		{
			statisticsButton.SetActive(!BlockGUI);
			friendsGrid.gameObject.SetActive(!BlockGUI);
		}
		if (!addInClanPanel.activeInHierarchy && activeInHierarchy)
		{
			HideAddPanel();
		}
		if (!statisiticPanel.activeInHierarchy)
		{
			AtStatsPanel = false;
		}
		if (ShowProfile && (!InClan || !FriendsController.readyToOperate))
		{
			_friendProfileController.HandleBackClicked();
		}
		if (AtAddPanel)
		{
			clanIsFull.gameObject.SetActive(FriendsController.sharedController.ClanLimitReached);
		}
		SetScreenMessage();
		if (InClan)
		{
			countMembersLabel.text = string.Format("{0}\n{1}", LocalizationStore.Get("Key_0983"), FriendsController.sharedController.clanMembers.Count);
		}
		noMembersLabel.SetActive(FriendsController.sharedController.clanMembers != null && FriendsController.sharedController.clanMembers.Count < 2);
		ClanName.SetActive(!BlockGUI);
		if (!statisiticPanel.activeInHierarchy)
		{
			BackBut.GetComponent<UIButton>().isEnabled = !BlockGUI;
		}
		CreateClanButton.GetComponent<UIButton>().isEnabled = !BlockGUI;
		Left.GetComponent<UIButton>().isEnabled = !BlockGUI;
		Right.GetComponent<UIButton>().isEnabled = !BlockGUI;
		EditLogoBut.GetComponent<UIButton>().isEnabled = !BlockGUI;
		if (_defendTime > 0f)
		{
			_defendTime -= Time.deltaTime;
		}
		friendsGrid.transform.parent.GetComponent<UIScrollView>().enabled = friendsGrid.transform.childCount > 4;
		if (friendsGrid.transform.childCount > 0 && friendsGrid.transform.childCount <= 4)
		{
			float num = 0f;
			foreach (Transform item in friendsGrid.transform)
			{
				num += item.localPosition.x;
			}
			num /= (float)friendsGrid.transform.childCount;
			Transform parent = friendsGrid.transform.parent;
			if (parent != null)
			{
				UIPanel component = parent.GetComponent<UIPanel>();
				if (component != null)
				{
					component.clipOffset = new Vector2(num, component.clipOffset.y);
					parent.localPosition = new Vector3(0f - num, parent.localPosition.y, parent.localPosition.z);
				}
			}
		}
		if (Time.realtimeSinceStartup - timeOfLastSort > 10f)
		{
			FriendsGUIController.RaiseUpdaeOnlineEvent();
			timeOfLastSort = Time.realtimeSinceStartup;
			_SortFriendPreviews();
		}
		UISprite[] value = _newMessagesOverlays.Value;
		UISprite[] array = value;
		foreach (UISprite uISprite in array)
		{
			if (ClanIncomingInvitesController.CurrentRequest == null || !ClanIncomingInvitesController.CurrentRequest.IsCompleted)
			{
				uISprite.gameObject.SetActive(false);
			}
			else if (ClanIncomingInvitesController.CurrentRequest.IsCanceled || ClanIncomingInvitesController.CurrentRequest.IsFaulted)
			{
				uISprite.gameObject.SetActive(false);
			}
			else
			{
				uISprite.gameObject.SetActive(ClanIncomingInvitesController.CurrentRequest.Result.Count > 0);
			}
		}
	}

	private void SetScreenMessage()
	{
		if (receivingPlashka == null)
		{
			return;
		}
		string text = string.Empty;
		if (!FriendsController.ClanDataSettted && InClan)
		{
			text = LocalizationStore.Key_0348;
		}
		else if (FriendsController.sharedController != null && InClan)
		{
			if (_friendProfileController != null && _friendProfileController.FriendProfileGo != null && _friendProfileController.FriendProfileGo.activeInHierarchy)
			{
				if (FriendsController.sharedController.NumberOffFullInfoRequests > 0)
				{
					text = LocalizationStore.Key_0348;
				}
			}
			else if (CreateClanPanel.activeInHierarchy && FriendsController.sharedController.NumberOfCreateClanRequests > 0)
			{
				text = LocalizationStore.Key_0348;
			}
		}
		else if ((!InClan || !FriendsController.readyToOperate) && !NoClanPanel.activeInHierarchy && !CreateClanPanel.activeInHierarchy && !clanPanel.activeInHierarchy && !statisiticPanel.activeInHierarchy && !addInClanPanel.activeInHierarchy && (CurrentState != State.Inbox || (CurrentState == State.Inbox && ClanIncomingInvitesController.CurrentRequest.Filter((Task<List<object>> t) => t.IsCompleted) == null)))
		{
			text = LocalizationStore.Key_0348;
		}
		if (!string.IsNullOrEmpty(text))
		{
			receivingPlashka.GetComponent<UILabel>().text = text;
			receivingPlashka.SetActive(true);
		}
		else
		{
			receivingPlashka.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		sharedController = null;
		_friendProfileController.Dispose();
		FriendsController.sharedController.StopRefreshingClanOnline();
		FriendsController friendsController = FriendsController.sharedController;
		friendsController.onChangeClanName = (FriendsController.OnChangeClanName)Delegate.Remove(friendsController.onChangeClanName, new FriendsController.OnChangeClanName(OnChangeClanName));
		AtAddPanel = false;
		AtStatsPanel = false;
		ShowProfile = false;
		UIInputRilisoft uIInputRilisoft = ((!(ClanName != null)) ? null : ClanName.GetComponent<UIInputRilisoft>());
		if (uIInputRilisoft != null)
		{
			uIInputRilisoft.onFocus = (UIInputRilisoft.OnFocus)Delegate.Remove(uIInputRilisoft.onFocus, new UIInputRilisoft.OnFocus(OnFocusCreateClanName));
			uIInputRilisoft.onFocusLost = (UIInputRilisoft.OnFocusLost)Delegate.Remove(uIInputRilisoft.onFocusLost, new UIInputRilisoft.OnFocusLost(onFocusLostCreateClanName));
		}
		FriendsController.DisposeProfile();
	}

	private void OnFocusCreateClanName()
	{
		if (ClanName != null)
		{
			ClanName.GetComponent<UIInputRilisoft>().value = string.Empty;
		}
	}

	private void onFocusLostCreateClanName()
	{
		if (ClanName != null)
		{
			UIInputRilisoft component = ClanName.GetComponent<UIInputRilisoft>();
			if (string.IsNullOrEmpty(component.value))
			{
				component.value = LocalizationStore.Key_0589;
			}
		}
	}
}

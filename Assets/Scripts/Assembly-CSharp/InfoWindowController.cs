using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InfoWindowController : MonoBehaviour
{
	private enum WindowType
	{
		None,
		infoBox,
		processDataBox,
		blockClick,
		DialogBox,
		QuestMessage,
		RestoreInventory,
		DeveloperConsoleMini,
		AchievementMessage,
		BattleInvite
	}

	private abstract class TopWindowData
	{
		public bool Opened { get; set; }

		public AchieveBox Box { get; protected set; }

		public TopWindowData(AchieveBox box)
		{
			Box = box;
		}

		public virtual void ShowThis()
		{
			Opened = true;
		}
	}

	private sealed class ShowBattleInviteData : TopWindowData
	{
		private readonly string _friendNickname;

		public ShowBattleInviteData(AchieveBox box, string friendNickname)
			: base(box)
		{
			_friendNickname = friendNickname ?? string.Empty;
		}

		public override void ShowThis()
		{
			if (!(Instance._friendNickname == null))
			{
				Instance._friendNickname.text = _friendNickname;
				base.Box.ShowBox();
			}
		}
	}

	private sealed class ShowQuestData : TopWindowData
	{
		public string Header;

		public string Text;

		public ShowQuestData(AchieveBox box)
			: base(box)
		{
		}

		public override void ShowThis()
		{
			Instance.questText.text = Text;
			base.Box.ShowBox();
		}
	}

	private sealed class ShowAchievementData : TopWindowData
	{
		public string Text;

		public Texture BgTexture;

		public string SpriteIcon;

		public ShowAchievementData(AchieveBox box)
			: base(box)
		{
		}

		public override void ShowThis()
		{
			Instance.achievementText.text = Text;
			Instance.achievementTextureBg.mainTexture = BgTexture;
			Instance.achievementTextureBg.fixedAspect = true;
			Instance.achievementSpriteIcon.spriteName = SpriteIcon;
			base.Box.ShowBox();
		}
	}

	public Camera infoWindowCamera;

	public UIWidget background;

	[Header("Processing data box")]
	public UIWidget processindDataBoxContainer;

	public UILabel processingDataBoxLabel;

	[Header("Info box")]
	public UIWidget infoBoxContainer;

	public UILabel infoBoxLabel;

	[Header("Dialog box Warning")]
	public UIWidget dialogBoxContainer;

	public UILabel dialogBoxText;

	[Header("Restore Window")]
	public GameObject restoreWindowPanel;

	[Header("quest box")]
	public AchieveBox questBox;

	public UILabel questText;

	public AudioClip questCompleteSound;

	[Header("achievement box")]
	public AchieveBox achievementBox;

	public UILabel achievementText;

	public UITexture achievementTextureBg;

	public UISprite achievementSpriteIcon;

	[SerializeField]
	[Header("Battle Invite Box")]
	private AchieveBox _battleInviteBox;

	[SerializeField]
	private UILabel _friendNickname;

	[Header("")]
	public Transform InfoWindowsRoot;

	private GameObject developerConsole;

	private Action DialogBoxOkClick;

	private Action DialogBoxCancelClick;

	private WindowType _typeCurrentWindow;

	private static InfoWindowController _instance;

	private static readonly Queue<TopWindowData> _showTopWindowQueue = new Queue<TopWindowData>();

	private IDisposable _backSubscription;

	private Action _unsubscribe;

	public static InfoWindowController Instance
	{
		get
		{
			if (_instance == null)
			{
				UnityEngine.Object original = Resources.Load("InfoWindows");
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original, Vector3.down * 567f, Quaternion.identity);
				_instance = gameObject.GetComponent<InfoWindowController>();
				return _instance;
			}
			return _instance;
		}
	}

	public static bool IsActive
	{
		get
		{
			return _instance != null && _instance.infoBoxContainer != null && _instance.infoBoxContainer.gameObject != null && _instance.infoBoxContainer.gameObject.activeInHierarchy;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		processingDataBoxLabel.text = LocalizationStore.Key_0348;
		LocalizationStore.AddEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void OnDisable()
	{
		if (_showTopWindowQueue.Count > 0)
		{
			TopWindowData topWindowData = _showTopWindowQueue.Dequeue();
			topWindowData.Opened = false;
			topWindowData.Box.isOpened = false;
		}
	}

	private void OnDestroy()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
		LocalizationStore.DelEventCallAfterLocalize(HandleLocalizationChanged);
		if (_unsubscribe != null)
		{
			_unsubscribe();
		}
	}

	private void HandleLocalizationChanged()
	{
		processingDataBoxLabel.text = LocalizationStore.Key_0348;
	}

	private void ActivateDevConsole()
	{
	}

	private void ActivateInfoBox(string text)
	{
		if (Instance._backSubscription != null)
		{
			Instance._backSubscription.Dispose();
		}
		Instance._backSubscription = BackSystem.Instance.Register(Instance.HandleEscape, "Info Window");
		infoBoxLabel.text = text;
		infoBoxContainer.gameObject.SetActive(true);
		background.gameObject.SetActive(true);
	}

	public void OnClickOkDialog()
	{
		if (DialogBoxOkClick != null)
		{
			DialogBoxOkClick();
		}
		Hide();
	}

	public void OnClickCancelDialog()
	{
		if (DialogBoxCancelClick != null)
		{
			DialogBoxCancelClick();
		}
		Hide();
	}

	private void ActivateDialogBox(string text, Action onOkClick, Action onCancelClick)
	{
		dialogBoxText.text = text;
		dialogBoxContainer.gameObject.SetActive(true);
		SetActiveBackground(true);
		DialogBoxOkClick = onOkClick;
		DialogBoxCancelClick = onCancelClick;
		if (Instance._backSubscription != null)
		{
			Instance._backSubscription.Dispose();
		}
		Instance._backSubscription = BackSystem.Instance.Register(Instance.HandleEscape, "Dialog Box");
	}

	public void ActivateRestorePanel(Action okCallback)
	{
		if (!(restoreWindowPanel == null))
		{
			restoreWindowPanel.SetActive(true);
			SetActiveBackground(false);
			DialogBoxOkClick = okCallback;
			if (Instance._backSubscription != null)
			{
				Instance._backSubscription.Dispose();
			}
			Instance._backSubscription = BackSystem.Instance.Register(Instance.BackButtonFromRestoreClick, "Restore Panel");
		}
	}

	private void BackButtonFromRestoreClick()
	{
	}

	private void ShowNextTopWindow()
	{
		if (!_showTopWindowQueue.Any((TopWindowData i) => i.Opened) && _showTopWindowQueue.Count >= 1)
		{
			TopWindowData topWindowData = _showTopWindowQueue.Peek();
			topWindowData.ShowThis();
			CoroutineRunner.Instance.StartCoroutine(HideBoxAfter(topWindowData, 3f));
			if (Defs.isSoundFX)
			{
				NGUITools.PlaySound(questCompleteSound);
			}
		}
	}

	private IEnumerator HideBoxAfter(TopWindowData tw, float secs)
	{
		yield return new WaitForRealSeconds(secs);
		tw.Box.HideBox();
		CoroutineRunner.Instance.StartCoroutine(WaitAchieveBoxHided(tw));
	}

	private IEnumerator WaitAchieveBoxHided(TopWindowData tw)
	{
		while (tw.Box.isOpened)
		{
			yield return null;
		}
		tw.Opened = false;
		if (_showTopWindowQueue.Count > 0)
		{
			_showTopWindowQueue.Dequeue();
		}
		ShowNextTopWindow();
	}

	private void DeactivateQuestBox()
	{
		questBox.HideBox();
	}

	private void DeactivateRestorePanel()
	{
		DialogBoxOkClick = null;
		DialogBoxCancelClick = null;
		if (restoreWindowPanel != null)
		{
			restoreWindowPanel.SetActive(false);
		}
	}

	private void DeactivateDialogBox()
	{
		DialogBoxOkClick = null;
		DialogBoxCancelClick = null;
		dialogBoxContainer.gameObject.SetActive(false);
	}

	private void DeactivateInfoBox()
	{
		background.gameObject.SetActive(false);
		infoBoxContainer.gameObject.SetActive(false);
	}

	private void SetActiveProcessDataBox(bool enable)
	{
		processindDataBoxContainer.gameObject.SetActive(enable);
	}

	private void SetActiveBackground(bool enable)
	{
		background.gameObject.SetActive(enable);
	}

	private void Initialize(WindowType typeWindow)
	{
		_typeCurrentWindow = typeWindow;
		base.gameObject.SetActive(true);
	}

	private void HideInfoAndProcessingBox()
	{
		if (_unsubscribe != null)
		{
			_unsubscribe();
		}
		if (_typeCurrentWindow != 0 && (_typeCurrentWindow == WindowType.infoBox || _typeCurrentWindow == WindowType.processDataBox))
		{
			if (_typeCurrentWindow == WindowType.infoBox)
			{
				DeactivateInfoBox();
			}
			else if (_typeCurrentWindow == WindowType.processDataBox)
			{
				SetActiveProcessDataBox(false);
			}
			SetActiveBackground(false);
			_typeCurrentWindow = WindowType.None;
			base.gameObject.SetActive(false);
		}
	}

	private void Hide()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
		if (_unsubscribe != null)
		{
			_unsubscribe();
		}
		if (_typeCurrentWindow != 0)
		{
			if (_typeCurrentWindow == WindowType.infoBox)
			{
				DeactivateInfoBox();
			}
			else if (_typeCurrentWindow == WindowType.processDataBox)
			{
				SetActiveProcessDataBox(false);
			}
			else if (_typeCurrentWindow == WindowType.DialogBox)
			{
				DeactivateDialogBox();
			}
			else if (_typeCurrentWindow == WindowType.RestoreInventory)
			{
				DeactivateRestorePanel();
			}
			SetActiveBackground(false);
			_typeCurrentWindow = WindowType.None;
			base.gameObject.SetActive(false);
		}
	}

	public static void ShowInfoBox(string text)
	{
		Instance.Initialize(WindowType.infoBox);
		Instance.ActivateInfoBox(text);
	}

	public static void ShowDevConsole()
	{
		Instance.Initialize(WindowType.DeveloperConsoleMini);
		Instance.ActivateDevConsole();
	}

	private void HandleEscape()
	{
		OnClickCancelDialog();
	}

	public static void ShowProcessingDataBox()
	{
		Instance.Initialize(WindowType.processDataBox);
		Instance.SetActiveProcessDataBox(true);
		Instance.SetActiveBackground(false);
	}

	public static void BlockAllClick()
	{
		Instance.Initialize(WindowType.blockClick);
		Instance.SetActiveBackground(true);
	}

	public static void ShowDialogBox(string text, Action callbackOkButton, Action callbackCancelButton = null)
	{
		Instance.Initialize(WindowType.DialogBox);
		Instance.ActivateDialogBox(text, callbackOkButton, callbackCancelButton);
	}

	public static void ShowRestorePanel(Action okCallback)
	{
		Instance.Initialize(WindowType.RestoreInventory);
		Instance.ActivateRestorePanel(okCallback);
	}

	public static void ShowQuestBox(string header, string text)
	{
		Instance.Initialize(WindowType.QuestMessage);
		_showTopWindowQueue.Enqueue(new ShowQuestData(Instance.questBox)
		{
			Header = header,
			Text = text
		});
		Instance.ShowNextTopWindow();
	}

	public static void ShowAchievementsBox(string text, Texture bgTexture, string spriteIcon)
	{
		Instance.Initialize(WindowType.AchievementMessage);
		_showTopWindowQueue.Enqueue(new ShowAchievementData(Instance.achievementBox)
		{
			Text = text,
			BgTexture = bgTexture,
			SpriteIcon = spriteIcon
		});
		Instance.ShowNextTopWindow();
	}

	public void ShowBattleInvite(string friendNickname)
	{
		if (!(_battleInviteBox == null))
		{
			Initialize(WindowType.BattleInvite);
			TopWindowData item = new ShowBattleInviteData(_battleInviteBox, friendNickname);
			_showTopWindowQueue.Enqueue(item);
			Instance.ShowNextTopWindow();
		}
	}

	public static void HideCurrentWindow()
	{
		Instance.Hide();
	}

	public static void HideProcessing(float time)
	{
		Instance.Invoke("HideInfoAndProcessingBox", time);
	}

	public static void HideProcessing()
	{
		Instance.HideInfoAndProcessingBox();
	}

	public void OnClickExitButton()
	{
		if (_typeCurrentWindow != WindowType.blockClick)
		{
			Hide();
		}
	}

	public static void CheckShowRequestServerInfoBox(bool isComplete, bool isRequestExist)
	{
		if (!isComplete)
		{
			ShowInfoBox(LocalizationStore.Get("Key_1528"));
		}
		else if (isRequestExist)
		{
			ShowInfoBox(LocalizationStore.Get("Key_1563"));
		}
	}
}

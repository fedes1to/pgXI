using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

public sealed class LeaderboardsView : MonoBehaviour
{
	public enum State
	{
		None = 0,
		Clans = 1,
		Friends = 2,
		BestPlayers = 3,
		Tournament = 4,
		Default = 3
	}

	public const float FarAwayX = 9000f;

	internal const string LeaderboardsTabCache = "Leaderboards.TabCache";

	[SerializeField]
	public UIWrapContent clansGrid;

	[SerializeField]
	public UIWrapContent friendsGrid;

	[SerializeField]
	public UIWrapContent bestPlayersGrid;

	[SerializeField]
	public UIWrapContent tournamentGrid;

	[SerializeField]
	public ButtonHandler backButton;

	[SerializeField]
	private UIButton clansButton;

	[SerializeField]
	private UIButton friendsButton;

	[SerializeField]
	private UIButton bestPlayersButton;

	[SerializeField]
	private UIButton tournamentButton;

	[SerializeField]
	private UIScrollView clansScroll;

	[SerializeField]
	private UIScrollView friendsScroll;

	[SerializeField]
	private UIScrollView bestPlayersScroll;

	[SerializeField]
	private UIScrollView tournamentScroll;

	[SerializeField]
	private UIScrollView LeagueScroll;

	[SerializeField]
	private GameObject friendsTableHeader;

	[SerializeField]
	private GameObject bestPlayersTableHeader;

	[SerializeField]
	private GameObject clansTableHeader;

	[SerializeField]
	private GameObject tournamentTableHeader;

	[SerializeField]
	public GameObject leaderboardHeader;

	[SerializeField]
	public GameObject leaderboardFooter;

	[SerializeField]
	public GameObject tournamentHeader;

	[SerializeField]
	public GameObject tournamentFooter;

	[SerializeField]
	public GameObject tournamentTableFooter;

	[SerializeField]
	public GameObject clansTableFooter;

	[SerializeField]
	public GameObject clansFooter;

	[SerializeField]
	public GameObject tournamentJoinInfo;

	[SerializeField]
	public UILabel expirationLabel;

	[SerializeField]
	public GameObject expirationIconObj;

	[SerializeField]
	public GameObject touchBlocker;

	private IDisposable _backSubscription;

	private bool _overallTopFooterActive;

	private bool _leagueTopFooterActive;

	private readonly Lazy<UIPanel> _leaderboardsPanel;

	private bool _prepared;

	private Dictionary<GameObject, int> _scrollsDefHeights = new Dictionary<GameObject, int>();

	private State _currentState;

	public bool InTournamentTop { get; set; }

	public bool CanShowClanTableFooter { get; set; }

	private bool IsTournamentMember
	{
		get
		{
			return RatingSystem.instance != null && RatingSystem.instance.currentLeague >= RatingSystem.RatingLeague.Adamant;
		}
	}

	public State CurrentState
	{
		get
		{
			return _currentState;
		}
		set
		{
			PlayerPrefs.SetInt("Leaderboards.TabCache", (int)value);
			if (clansButton != null)
			{
				clansButton.isEnabled = value != State.Clans;
				Transform transform = clansButton.gameObject.transform.FindChild("SpriteLabel");
				if (transform != null)
				{
					transform.gameObject.SetActive(value != State.Clans);
				}
				Transform transform2 = clansButton.gameObject.transform.FindChild("ChekmarkLabel");
				if (transform2 != null)
				{
					transform2.gameObject.SetActive(value == State.Clans);
				}
				clansFooter.Do(delegate(GameObject g)
				{
					g.SetActiveSafe(value == State.Clans);
				});
				clansTableFooter.Do(delegate(GameObject g)
				{
					g.SetActiveSafe(value == State.Clans && CanShowClanTableFooter);
				});
			}
			if (friendsButton != null)
			{
				friendsButton.isEnabled = value != State.Friends;
				Transform transform3 = friendsButton.gameObject.transform.FindChild("SpriteLabel");
				if (transform3 != null)
				{
					transform3.gameObject.SetActive(value != State.Friends);
				}
				Transform transform4 = friendsButton.gameObject.transform.FindChild("ChekmarkLabel");
				if (transform4 != null)
				{
					transform4.gameObject.SetActive(value == State.Friends);
				}
			}
			if (bestPlayersButton != null)
			{
				bestPlayersButton.isEnabled = value != State.BestPlayers;
				Transform transform5 = bestPlayersButton.gameObject.transform.FindChild("SpriteLabel");
				if (transform5 != null)
				{
					transform5.gameObject.SetActive(value != State.BestPlayers);
				}
				Transform transform6 = bestPlayersButton.gameObject.transform.FindChild("ChekmarkLabel");
				if (transform6 != null)
				{
					transform6.gameObject.SetActive(value == State.BestPlayers);
				}
				leaderboardFooter.Do(delegate(GameObject g)
				{
					g.SetActiveSafe(value == State.BestPlayers);
				});
				leaderboardHeader.Do(delegate(GameObject g)
				{
					g.SetActiveSafe(value == State.BestPlayers);
				});
			}
			if (tournamentButton != null)
			{
				tournamentButton.isEnabled = value != State.Tournament;
				Transform transform7 = tournamentButton.gameObject.transform.FindChild("SpriteLabel");
				if (transform7 != null)
				{
					transform7.gameObject.SetActive(value != State.Tournament);
				}
				Transform transform8 = tournamentButton.gameObject.transform.FindChild("ChekmarkLabel");
				if (transform8 != null)
				{
					transform8.gameObject.SetActive(value == State.Tournament);
				}
			}
			if (clansTableHeader != null)
			{
				bool active = value == State.Clans;
				clansTableHeader.SetActive(active);
			}
			bestPlayersGrid.Do(delegate(UIWrapContent g)
			{
				Vector3 localPosition4 = g.transform.localPosition;
				localPosition4.x = ((value != State.BestPlayers) ? 9000f : 0f);
				g.gameObject.transform.localPosition = localPosition4;
			});
			friendsGrid.Do(delegate(UIWrapContent g)
			{
				Vector3 localPosition3 = g.transform.localPosition;
				localPosition3.x = ((value != State.Friends) ? 9000f : 0f);
				g.gameObject.transform.localPosition = localPosition3;
			});
			clansGrid.Do(delegate(UIWrapContent g)
			{
				Vector3 localPosition2 = g.transform.localPosition;
				localPosition2.x = ((value != State.Clans) ? 9000f : 0f);
				g.gameObject.transform.localPosition = localPosition2;
			});
			tournamentJoinInfo.Do(delegate(GameObject o)
			{
				o.SetActiveSafe(value == State.Tournament && !IsTournamentMember);
			});
			bool canShowTournamentGrid = value == State.Tournament && IsTournamentMember;
			tournamentFooter.Do(delegate(GameObject g)
			{
				g.SetActiveSafe(canShowTournamentGrid);
			});
			tournamentHeader.Do(delegate(GameObject g)
			{
				g.SetActiveSafe(canShowTournamentGrid);
			});
			tournamentTableFooter.Do(delegate(GameObject g)
			{
				g.SetActiveSafe(canShowTournamentGrid && InTournamentTop);
			});
			tournamentGrid.Do(delegate(UIWrapContent g)
			{
				Vector3 localPosition = g.transform.localPosition;
				localPosition.x = ((!canShowTournamentGrid) ? 9000f : 0f);
				g.gameObject.transform.localPosition = localPosition;
			});
			UpdateScrollSize(tournamentGrid.gameObject, tournamentTableFooter);
			UpdateScrollSize(clansGrid.gameObject, clansTableFooter);
			bestPlayersTableHeader.Do(delegate(GameObject o)
			{
				o.SetActive(value == State.BestPlayers);
			});
			clansTableHeader.Do(delegate(GameObject o)
			{
				o.SetActive(value == State.Clans);
			});
			friendsTableHeader.Do(delegate(GameObject o)
			{
				o.SetActive(value == State.Friends);
			});
			tournamentTableHeader.Do(delegate(GameObject o)
			{
				o.SetActive(value == State.Tournament && !tournamentJoinInfo.activeInHierarchy);
			});
			State currentState = _currentState;
			_currentState = value;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(currentState, _currentState);
			}
		}
	}

	internal bool Prepared
	{
		get
		{
			return _prepared;
		}
	}

	public event Action<State, State> OnStateChanged;

	public LeaderboardsView()
	{
		_leaderboardsPanel = new Lazy<UIPanel>(base.GetComponent<UIPanel>);
	}

	public void SetOverallTopFooterActive()
	{
		_overallTopFooterActive = true;
	}

	public void SetLeagueTopFooterActive()
	{
		_leagueTopFooterActive = true;
	}

	private void RefreshGrid(UIGrid grid)
	{
		grid.Reposition();
	}

	private IEnumerator SkipFrameAndExecuteCoroutine(Action a)
	{
		if (a != null)
		{
			yield return new WaitForEndOfFrame();
			a();
		}
	}

	private void HandleTabPressed(object sender, EventArgs e)
	{
		GameObject gameObject = ((ButtonHandler)sender).gameObject;
		if (clansButton != null && gameObject == clansButton.gameObject)
		{
			CurrentState = State.Clans;
		}
		else if (friendsButton != null && gameObject == friendsButton.gameObject)
		{
			CurrentState = State.Friends;
		}
		else if (bestPlayersButton != null && gameObject == bestPlayersButton.gameObject)
		{
			CurrentState = State.BestPlayers;
		}
		else if (tournamentButton != null && gameObject == tournamentButton.gameObject)
		{
			CurrentState = State.Tournament;
		}
	}

	private IEnumerator UpdateGridsAndScrollers()
	{
		_prepared = false;
		yield return new WaitForEndOfFrame();
		IEnumerable<UIWrapContent> wraps = new UIWrapContent[3] { friendsGrid, clansGrid, tournamentGrid }.Where((UIWrapContent g) => g != null);
		foreach (UIWrapContent w in wraps)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		}
		yield return null;
		IEnumerable<UIScrollView> scrolls = new UIScrollView[3] { clansScroll, friendsScroll, LeagueScroll }.Where((UIScrollView s) => s != null);
		foreach (UIScrollView s2 in scrolls)
		{
			s2.ResetPosition();
			s2.UpdatePosition();
		}
		_prepared = true;
	}

	private void OnEnable()
	{
		_backSubscription = BackSystem.Instance.Register(delegate
		{
			LeaderboardScript.Instance.ReturnBack(this, null);
		}, "Leaderboards");
		StartCoroutine(UpdateGridsAndScrollers());
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_prepared = false;
	}

	private void UpdateScrollSize(GameObject scrollChildObj, GameObject widgetObject)
	{
		UIPanel componentInParent = scrollChildObj.GetComponentInParent<UIPanel>();
		if (widgetObject == null)
		{
			return;
		}
		UIWidget component = widgetObject.GetComponent<UIWidget>();
		if (!(componentInParent == null) && !(component == null))
		{
			int num = ((!_scrollsDefHeights.Keys.Contains(scrollChildObj)) ? (-1) : _scrollsDefHeights[scrollChildObj]);
			if (num >= 0)
			{
				componentInParent.bottomAnchor.absolute = ((!widgetObject.activeInHierarchy) ? num : (num + component.height));
			}
		}
	}

	private void Awake()
	{
		List<UIWrapContent> list = new UIWrapContent[4] { friendsGrid, bestPlayersGrid, clansGrid, tournamentGrid }.Where((UIWrapContent g) => g != null).ToList();
		foreach (UIWrapContent item in list)
		{
			item.gameObject.SetActive(true);
			Vector3 localPosition = item.transform.localPosition;
			localPosition.x = 9000f;
			item.gameObject.transform.localPosition = localPosition;
		}
		UIPanel componentInParent = clansGrid.gameObject.GetComponentInParent<UIPanel>();
		_scrollsDefHeights.Add(clansGrid.gameObject, componentInParent.bottomAnchor.absolute);
		componentInParent = friendsGrid.gameObject.GetComponentInParent<UIPanel>();
		_scrollsDefHeights.Add(friendsGrid.gameObject, componentInParent.bottomAnchor.absolute);
		componentInParent = bestPlayersGrid.gameObject.GetComponentInParent<UIPanel>();
		_scrollsDefHeights.Add(bestPlayersGrid.gameObject, componentInParent.bottomAnchor.absolute);
		componentInParent = tournamentGrid.gameObject.GetComponentInParent<UIPanel>();
		_scrollsDefHeights.Add(tournamentGrid.gameObject, componentInParent.bottomAnchor.absolute);
	}

	private IEnumerator Start()
	{
		IEnumerable<UIButton> buttons = new UIButton[4] { clansButton, friendsButton, bestPlayersButton, tournamentButton }.Where((UIButton b) => b != null);
		foreach (UIButton b2 in buttons)
		{
			ButtonHandler bh = b2.GetComponent<ButtonHandler>();
			if (bh != null)
			{
				bh.Clicked += HandleTabPressed;
			}
		}
		IEnumerable<UIScrollView> scrollViews = new UIScrollView[4] { clansScroll, friendsScroll, bestPlayersScroll, LeagueScroll }.Where((UIScrollView s) => s != null);
		foreach (UIScrollView scrollView in scrollViews)
		{
			scrollView.ResetPosition();
		}
		yield return null;
		friendsGrid.Do(delegate(UIWrapContent w)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		});
		bestPlayersGrid.Do(delegate(UIWrapContent w)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		});
		clansGrid.Do(delegate(UIWrapContent w)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		});
		tournamentGrid.Do(delegate(UIWrapContent w)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		});
		yield return null;
		int stateInt = PlayerPrefs.GetInt("Leaderboards.TabCache", 3);
		State state = ((!Enum.IsDefined(typeof(State), stateInt)) ? State.BestPlayers : ((State)stateInt));
		CurrentState = ((state == State.None) ? State.BestPlayers : state);
	}
}

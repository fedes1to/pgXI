using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Rilisoft;
using Rilisoft.MiniJson;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class LeaderboardScript : MonoBehaviour
{
	private enum GridState
	{
		Empty,
		FillingWithCache,
		Cache,
		FillingWithResponse,
		Response
	}

	private const int VisibleItemMaxCount = 15;

	private float _expirationTimeSeconds;

	private float _expirationNextUpateTimeSeconds;

	private bool _fillLock;

	private readonly List<LeaderboardItemViewModel> _clansList = new List<LeaderboardItemViewModel>(101);

	private readonly List<LeaderboardItemViewModel> _friendsList = new List<LeaderboardItemViewModel>(101);

	private readonly List<LeaderboardItemViewModel> _playersList = new List<LeaderboardItemViewModel>(101);

	private readonly List<LeaderboardItemViewModel> _tournamentList = new List<LeaderboardItemViewModel>(201);

	private Vector3? _startScrollPos;

	private bool _scrollToPlayerRunningVal;

	[SerializeField]
	private GameObject _viewHandler;

	[SerializeField]
	private PrefabHandler _viewPrefab;

	private LazyObject<LeaderboardsView> _view;

	private UIPanel _panelVal;

	private bool _isInit;

	private Lazy<MainMenuController> _mainMenuController;

	private TaskCompletionSource<bool> _returnPromise = new TaskCompletionSource<bool>();

	private bool _profileIsOpened;

	private static TaskCompletionSource<string> _currentRequestPromise;

	private GridState _state;

	private LeaderboardsView LeaderboardView
	{
		get
		{
			return _view.Value;
		}
	}

	public UILabel ExpirationLabel
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.expirationLabel : null;
		}
	}

	public GameObject ExpirationIconObject
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.expirationIconObj : null;
		}
	}

	private GameObject TopFriendsGrid
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.friendsGrid.gameObject : null;
		}
	}

	private GameObject TopPlayersGrid
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.bestPlayersGrid.gameObject : null;
		}
	}

	private GameObject TopClansGrid
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.clansGrid.gameObject : null;
		}
	}

	private GameObject TournamentGrid
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.tournamentGrid.gameObject : null;
		}
	}

	private GameObject ClansTableFooter
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.clansTableFooter : null;
		}
	}

	private GameObject Headerleaderboard
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.leaderboardHeader : null;
		}
	}

	private GameObject FooterLeaderboard
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.leaderboardFooter : null;
		}
	}

	private GameObject FooterTournament
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.tournamentFooter : null;
		}
	}

	private GameObject FooterTableTournament
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.tournamentTableFooter : null;
		}
	}

	private GameObject HeaderTournament
	{
		get
		{
			return (!(LeaderboardView == null)) ? LeaderboardView.tournamentHeader : null;
		}
	}

	private bool ScrollToPlayerRunning
	{
		get
		{
			return _scrollToPlayerRunningVal;
		}
		set
		{
			_scrollToPlayerRunningVal = value;
		}
	}

	public static LeaderboardScript Instance { get; private set; }

	public UIPanel Panel
	{
		get
		{
			if (_panelVal == null)
			{
				_panelVal = ((_view == null || !_view.ObjectIsLoaded) ? null : _view.Value.gameObject.GetComponent<UIPanel>());
			}
			return _panelVal;
		}
	}

	public bool UIEnabled
	{
		get
		{
			return _view != null && _view.ObjectIsActive;
		}
	}

	private Task<string> CurrentRequest
	{
		get
		{
			return _currentRequestPromise.Map((TaskCompletionSource<string> p) => p.Task);
		}
	}

	private static string LeaderboardsResponseCache
	{
		get
		{
			return "Leaderboards.Tier.ResponseCache";
		}
	}

	public static string LeaderboardsResponseCacheTimestamp
	{
		get
		{
			return "Leaderboards.New.ResponseCacheTimestamp";
		}
	}

	public static event EventHandler<ClickedEventArgs> PlayerClicked;

	private void OnLeaderboardViewStateChanged(LeaderboardsView.State fromState, LeaderboardsView.State toState)
	{
		if (fromState != toState && toState == LeaderboardsView.State.BestPlayers)
		{
			StartCoroutine(ScrollTopPlayersGridTo(FriendsController.sharedController.id));
		}
	}

	private void UpdateLocs()
	{
		if (ClansTableFooter != null)
		{
			ClansTableFooter.transform.FindChild("LabelPlace").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
			{
				n.text = LocalizationStore.Get("Key_0053");
			});
		}
	}

	private IEnumerator FillGrids(string response, string playerId, GridState state)
	{
		while (_fillLock)
		{
			yield return null;
		}
		_fillLock = true;
		try
		{
			if (string.IsNullOrEmpty(playerId))
			{
				throw new ArgumentException("Player id should not be empty", "playerId");
			}
			Dictionary<string, object> d = Json.Deserialize(response ?? string.Empty) as Dictionary<string, object>;
			if (d == null)
			{
				Debug.LogWarning("Leaderboards response is ill-formed.");
				d = new Dictionary<string, object>();
			}
			else if (d.Count == 0)
			{
				Debug.LogWarning("Leaderboards response contains no elements.");
			}
			LeaderboardItemViewModel me = new LeaderboardItemViewModel
			{
				Id = playerId,
				Nickname = ProfileController.GetPlayerNameOrDefault(),
				Rank = ExperienceController.sharedController.currentLevel,
				WinCount = RatingSystem.instance.currentRating,
				Highlight = true,
				ClanName = FriendsController.sharedController.Map((FriendsController s) => s.clanName, string.Empty),
				ClanLogo = FriendsController.sharedController.Map((FriendsController s) => s.clanLogo, string.Empty)
			};
			object meObject;
			if (d.TryGetValue("me", out meObject))
			{
				Dictionary<string, object> meDictionary = meObject as Dictionary<string, object>;
				object myWinCount;
				if (meDictionary.TryGetValue("wins", out myWinCount))
				{
					int wins = Convert.ToInt32(myWinCount);
					PlayerPrefs.SetInt("TotalWinsForLeaderboards", wins);
				}
			}
			int _competitionId;
			if (d.ContainsKey("competition_id") && int.TryParse(d["competition_id"].ToString(), out _competitionId) && FriendsController.sharedController != null && _competitionId > FriendsController.sharedController.currentCompetition)
			{
				FriendsController.sharedController.SendRequestGetCurrentcompetition();
			}
			List<LeaderboardItemViewModel> rawFriendsList = LeaderboardsController.ParseLeaderboardEntries(playerId, "friends", d);
			HashSet<string> friendIds = new HashSet<string>(FriendsController.sharedController.friends);
			if (FriendsController.sharedController != null)
			{
				for (int j = rawFriendsList.Count - 1; j >= 0; j--)
				{
					LeaderboardItemViewModel item = rawFriendsList[j];
					Dictionary<string, object> info;
					if (friendIds.Contains(item.Id) && FriendsController.sharedController.friendsInfo.TryGetValue(item.Id, out info))
					{
						try
						{
							Dictionary<string, object> playerDict = info["player"] as Dictionary<string, object>;
							item.Nickname = Convert.ToString(playerDict["nick"]);
							item.Rank = Convert.ToInt32(playerDict["rank"]);
							object clanName;
							if (playerDict.TryGetValue("clan_name", out clanName))
							{
								item.ClanName = Convert.ToString(clanName);
							}
							object clanLogo;
							if (playerDict.TryGetValue("clan_logo", out clanLogo))
							{
								item.ClanLogo = Convert.ToString(clanLogo);
							}
						}
						catch (KeyNotFoundException)
						{
							Debug.LogError("Failed to process cached friend: " + item.Id);
						}
					}
					else
					{
						rawFriendsList.RemoveAt(j);
					}
				}
			}
			rawFriendsList.Add(me);
			yield return StartCoroutine(FillFriendsGrid(list: GroupAndOrder(rawFriendsList), gridGo: TopFriendsGrid, state: state));
			List<LeaderboardItemViewModel> rawTopPlayersList = LeaderboardsController.ParseLeaderboardEntries(playerId, "best_players", d);
			if (rawTopPlayersList.Any())
			{
				if (rawTopPlayersList.All((LeaderboardItemViewModel i) => i.Id != me.Id))
				{
					LeaderboardItemViewModel min = rawTopPlayersList.OrderByDescending((LeaderboardItemViewModel i) => i.WinCount).Last();
					rawTopPlayersList.Remove(min);
					rawTopPlayersList.Add(me);
				}
				else
				{
					LeaderboardItemViewModel myitem = rawTopPlayersList.FirstOrDefault((LeaderboardItemViewModel i) => i.Id == me.Id);
					if (myitem != null)
					{
						myitem.Nickname = me.Nickname;
						myitem.Rank = me.Rank;
						myitem.WinCount = me.WinCount;
						myitem.Highlight = me.Highlight;
						myitem.ClanName = me.ClanName;
						myitem.ClanLogo = me.ClanLogo;
					}
				}
			}
			List<LeaderboardItemViewModel> orderedTopPlayersList = GroupAndOrder(rawTopPlayersList);
			AddCacheInProfileInfo(rawTopPlayersList);
			Coroutine fillPlayersCoroutine = StartCoroutine(FillPlayersGrid(TopPlayersGrid, orderedTopPlayersList, state));
			rawTopPlayersList.Clear();
			List<LeaderboardItemViewModel> rawTournamentList = LeaderboardsController.ParseLeaderboardEntries(playerId, "competition", d);
			if (rawTournamentList.Any())
			{
				if (rawTournamentList.All((LeaderboardItemViewModel i) => i.Id != me.Id))
				{
					if (rawTournamentList.Any((LeaderboardItemViewModel i) => i.WinCount <= me.WinCount))
					{
						LeaderboardItemViewModel min2 = rawTournamentList.OrderByDescending((LeaderboardItemViewModel i) => i.WinCount).Last();
						rawTournamentList.Remove(min2);
						rawTournamentList.Add(me);
					}
				}
				else
				{
					LeaderboardItemViewModel myitem2 = rawTournamentList.FirstOrDefault((LeaderboardItemViewModel i) => i.Id == me.Id);
					if (myitem2 != null)
					{
						myitem2.Nickname = me.Nickname;
						myitem2.Rank = me.Rank;
						myitem2.WinCount = me.WinCount;
						myitem2.Highlight = me.Highlight;
						myitem2.ClanName = me.ClanName;
						myitem2.ClanLogo = me.ClanLogo;
					}
				}
			}
			LeaderboardView.InTournamentTop = rawTournamentList.Any() && rawTournamentList.All((LeaderboardItemViewModel i) => i.Id != me.Id);
			if (FooterTableTournament != null)
			{
				if (LeaderboardView.InTournamentTop)
				{
					FooterTableTournament.transform.FindChild("LabelPlace").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
					{
						n.text = LocalizationStore.Get("Key_0053");
					});
					FooterTableTournament.transform.FindChild("LabelNick").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
					{
						n.text = me.Nickname;
					});
					FooterTableTournament.transform.FindChild("LabelWins").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
					{
						n.text = RatingSystem.instance.currentRating.ToString();
					});
					FooterTableTournament.SetActiveSafe(true);
				}
				else
				{
					FooterTableTournament.SetActiveSafe(false);
				}
			}
			Coroutine fillTournamentCoroutine = StartCoroutine(FillTournamentGrid(list: GroupAndOrder(rawTournamentList), gridGo: TournamentGrid, state: state));
			rawTournamentList.Clear();
			List<LeaderboardItemViewModel> rawClansList = LeaderboardsController.ParseLeaderboardEntries(playerId, "top_clans", d);
			Coroutine fillClansCoroutine = StartCoroutine(FillClansGrid(list: GroupAndOrder(rawClansList), gridGo: TopClansGrid, state: state));
			if (ClansTableFooter != null)
			{
				string clanId = FriendsController.sharedController.Map((FriendsController s) => s.ClanID);
				if (string.IsNullOrEmpty(clanId))
				{
					LeaderboardView.CanShowClanTableFooter = false;
					ClansTableFooter.SetActive(false);
				}
				else
				{
					LeaderboardItemViewModel myClanInTop = rawClansList.FirstOrDefault((LeaderboardItemViewModel c) => c.Id == clanId);
					LeaderboardView.CanShowClanTableFooter = myClanInTop == null;
					ClansTableFooter.transform.FindChild("LabelPlace").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
					{
						n.text = LocalizationStore.Get("Key_0053");
					});
					ClansTableFooter.transform.FindChild("LabelMembers").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
					{
						n.text = string.Empty;
					});
					ClansTableFooter.transform.FindChild("LabelWins").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel w)
					{
						w.text = string.Empty;
					});
					UILabel clanLabel = ClansTableFooter.transform.FindChild("LabelNick").Map((Transform t) => t.gameObject.GetComponent<UILabel>());
					clanLabel.Do(delegate(UILabel cl)
					{
						cl.text = FriendsController.sharedController.Map((FriendsController s) => s.clanName, string.Empty);
					});
					clanLabel.Map((UILabel cl) => cl.GetComponentsInChildren<UITexture>(true).FirstOrDefault()).Do(delegate(UITexture t)
					{
						SetClanLogo(t, FriendsController.sharedController.Map((FriendsController s) => s.clanLogo, string.Empty));
					});
				}
			}
			yield return fillPlayersCoroutine;
			yield return fillClansCoroutine;
			yield return fillTournamentCoroutine;
		}
		finally
		{
			_fillLock = false;
		}
	}

	private void AddCacheInProfileInfo(List<LeaderboardItemViewModel> _list)
	{
		foreach (LeaderboardItemViewModel item in _list)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("nick", item.Nickname);
			dictionary.Add("rank", item.Rank);
			dictionary.Add("clan_name", item.ClanName);
			dictionary.Add("clan_logo", item.ClanLogo);
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("player", dictionary);
			if (!FriendsController.sharedController.profileInfo.ContainsKey(item.Id))
			{
				FriendsController.sharedController.profileInfo.Add(item.Id, dictionary2);
			}
			else
			{
				FriendsController.sharedController.profileInfo[item.Id] = dictionary2;
			}
		}
	}

	private IEnumerator FillClansGrid(GameObject gridGo, List<LeaderboardItemViewModel> list, GridState state)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		UIWrapContent wrap = gridGo.GetComponent<UIWrapContent>();
		if (wrap == null)
		{
			throw new InvalidOperationException("Game object does not contain UIWrapContent component.");
		}
		wrap.minIndex = Math.Min(-list.Count + 1, wrap.maxIndex);
		wrap.onInitializeItem = null;
		GameObject gridGo2 = default(GameObject);
		GridState state2 = default(GridState);
		wrap.onInitializeItem = (UIWrapContent.OnInitializeItem)Delegate.Combine(wrap.onInitializeItem, (UIWrapContent.OnInitializeItem)delegate(GameObject go, int wrapIndex, int realIndex)
		{
			int index = -realIndex;
			FillClanItem(gridGo2, _clansList, state2, index, go);
		});
		int childCount = gridGo.transform.childCount;
		if (childCount == 0)
		{
			Debug.LogError("No children in grid.");
			yield break;
		}
		Transform itemPrototype = gridGo.transform.GetChild(childCount - 1);
		if (itemPrototype == null)
		{
			Debug.LogError("Cannot find prototype for item.");
			yield break;
		}
		_clansList.Clear();
		_clansList.AddRange(list);
		GameObject itemPrototypeGo = itemPrototype.gameObject;
		itemPrototypeGo.SetActive(_clansList.Count > 0);
		int bound = Math.Min(15, _clansList.Count);
		for (int i = 0; i != bound; i++)
		{
			LeaderboardItemViewModel item = _clansList[i];
			GameObject newItem;
			if (i < childCount)
			{
				newItem = gridGo.transform.GetChild(i).gameObject;
			}
			else
			{
				newItem = NGUITools.AddChild(gridGo, itemPrototypeGo);
				newItem.name = i.ToString(CultureInfo.InvariantCulture);
			}
			FillClanItem(gridGo, _clansList, state, i, newItem);
		}
		yield return new WaitForEndOfFrame();
		wrap.SortBasedOnScrollMovement();
		wrap.WrapContent();
		UIScrollView scrollView = gridGo.transform.parent.gameObject.GetComponent<UIScrollView>();
		if (scrollView != null)
		{
			scrollView.enabled = true;
			scrollView.ResetPosition();
			scrollView.UpdatePosition();
		}
	}

	private IEnumerator FillPlayersGrid(GameObject gridGo, List<LeaderboardItemViewModel> list, GridState state)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		UIWrapContent wrap = gridGo.GetComponent<UIWrapContent>();
		if (wrap == null)
		{
			throw new InvalidOperationException("Game object does not contain UIWrapContent component.");
		}
		wrap.minIndex = Math.Min(-list.Count + 1, wrap.maxIndex);
		wrap.onInitializeItem = null;
		GameObject gridGo2 = default(GameObject);
		GridState state2 = default(GridState);
		wrap.onInitializeItem = (UIWrapContent.OnInitializeItem)Delegate.Combine(wrap.onInitializeItem, (UIWrapContent.OnInitializeItem)delegate(GameObject go, int wrapIndex, int realIndex)
		{
			int index = -realIndex;
			FillIndividualItem(gridGo2, _playersList, state2, index, go);
		});
		int childCount = gridGo.transform.childCount;
		if (childCount == 0)
		{
			Debug.LogError("No children in grid.");
			yield break;
		}
		Transform itemPrototype = gridGo.transform.GetChild(childCount - 1);
		if (itemPrototype == null)
		{
			Debug.LogError("Cannot find prototype for item.");
			yield break;
		}
		_playersList.Clear();
		_playersList.AddRange(list);
		GameObject itemPrototypeGo = itemPrototype.gameObject;
		itemPrototypeGo.SetActive(_playersList.Count > 0);
		int bound = Math.Min(15, _playersList.Count);
		for (int i = 0; i != bound; i++)
		{
			LeaderboardItemViewModel item = _playersList[i];
			GameObject newItem;
			if (i < childCount)
			{
				newItem = gridGo.transform.GetChild(i).gameObject;
			}
			else
			{
				newItem = NGUITools.AddChild(gridGo, itemPrototypeGo);
				newItem.name = i.ToString(CultureInfo.InvariantCulture);
			}
			FillIndividualItem(gridGo, _playersList, state, i, newItem);
		}
		yield return new WaitForEndOfFrame();
		wrap.SortBasedOnScrollMovement();
		wrap.WrapContent();
		UIScrollView scrollView = gridGo.transform.parent.gameObject.GetComponent<UIScrollView>();
		if (scrollView != null)
		{
			scrollView.enabled = true;
			scrollView.ResetPosition();
			scrollView.UpdatePosition();
			while (!base.gameObject.activeInHierarchy)
			{
				yield return null;
			}
			StartCoroutine(ScrollTopPlayersGridTo(FriendsController.sharedController.id));
		}
	}

	private IEnumerator ScrollTopPlayersGridTo(string viewId = null)
	{
		if (ScrollToPlayerRunning)
		{
			yield break;
		}
		ScrollToPlayerRunning = true;
		UIScrollView scroll = TopPlayersGrid.transform.parent.gameObject.GetComponent<UIScrollView>();
		if (scroll == null)
		{
			ScrollToPlayerRunning = false;
			yield break;
		}
		UIWrapContent wrapContent = TopPlayersGrid.GetComponent<UIWrapContent>();
		if (wrapContent == null)
		{
			ScrollToPlayerRunning = false;
			yield break;
		}
		int itemHeight = wrapContent.itemSize;
		float panelHeight = scroll.gameObject.GetComponent<UIPanel>().height;
		int visibleItemsCount = (int)(panelHeight / (float)itemHeight);
		Debug.Log("=>>> visible items count: " + visibleItemsCount);
		if (_playersList.Count <= visibleItemsCount)
		{
			ScrollToPlayerRunning = false;
			yield break;
		}
		yield return null;
		if (viewId.IsNullOrEmpty())
		{
			scroll.MoveRelative(scroll.panel.clipOffset);
			ScrollToPlayerRunning = false;
			yield break;
		}
		string viewId2 = default(string);
		LeaderboardItemViewModel me = _playersList.FirstOrDefault((LeaderboardItemViewModel i) => i.Id == viewId2);
		if (me == null)
		{
			ScrollToPlayerRunning = false;
			yield break;
		}
		int idx2 = _playersList.IndexOf(me);
		int lastposibleIdxToScroll = _playersList.Count - visibleItemsCount;
		idx2 = Mathf.Clamp(idx2, 0, lastposibleIdxToScroll);
		if (idx2 > lastposibleIdxToScroll)
		{
			idx2 = lastposibleIdxToScroll;
		}
		float scrollFactor = itemHeight * idx2;
		if (!_startScrollPos.HasValue)
		{
			_startScrollPos = scroll.panel.gameObject.transform.localPosition;
		}
		scroll.panel.clipOffset = Vector2.zero;
		scroll.panel.gameObject.transform.localPosition = _startScrollPos.Value;
		float scrollLeft = Mathf.Abs(scrollFactor);
		while (scrollLeft > 0f)
		{
			float s = itemHeight * visibleItemsCount - 1;
			if (!(scrollLeft > 0f))
			{
				continue;
			}
			if (scrollLeft - s < 0f)
			{
				scroll.MoveRelative(new Vector3(0f, scrollLeft));
				scrollLeft = 0f;
				continue;
			}
			scrollLeft -= s;
			if (scrollFactor < 0f)
			{
				s *= -1f;
			}
			scroll.MoveRelative(new Vector3(0f, s));
		}
		if (idx2 + visibleItemsCount >= _playersList.Count)
		{
			scroll.MoveRelative(new Vector3(0f, -60f));
		}
		ScrollToPlayerRunning = false;
	}

	private IEnumerator FillFriendsGrid(GameObject gridGo, List<LeaderboardItemViewModel> list, GridState state)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		UIWrapContent wrap = gridGo.GetComponent<UIWrapContent>();
		if (wrap == null)
		{
			throw new InvalidOperationException("Game object does not contain UIWrapContent component.");
		}
		wrap.minIndex = Math.Min(-list.Count + 1, wrap.maxIndex);
		wrap.onInitializeItem = null;
		GameObject gridGo2 = default(GameObject);
		GridState state2 = default(GridState);
		wrap.onInitializeItem = (UIWrapContent.OnInitializeItem)Delegate.Combine(wrap.onInitializeItem, (UIWrapContent.OnInitializeItem)delegate(GameObject go, int wrapIndex, int realIndex)
		{
			int index = -realIndex;
			FillIndividualItem(gridGo2, _friendsList, state2, index, go);
		});
		int childCount = gridGo.transform.childCount;
		if (childCount == 0)
		{
			Debug.LogError("No children in grid.");
			yield break;
		}
		Transform itemPrototype = gridGo.transform.GetChild(childCount - 1);
		if (itemPrototype == null)
		{
			Debug.LogError("Cannot find prototype for item.");
			yield break;
		}
		_friendsList.Clear();
		_friendsList.AddRange(list);
		GameObject itemPrototypeGo = itemPrototype.gameObject;
		itemPrototypeGo.SetActive(_friendsList.Count > 0);
		int bound = Math.Min(15, _friendsList.Count);
		for (int j = 0; j != bound; j++)
		{
			LeaderboardItemViewModel item = _friendsList[j];
			GameObject newItem;
			if (j < childCount)
			{
				newItem = gridGo.transform.GetChild(j).gameObject;
			}
			else
			{
				newItem = NGUITools.AddChild(gridGo, itemPrototypeGo);
				newItem.name = j.ToString(CultureInfo.InvariantCulture);
			}
			FillIndividualItem(gridGo, _friendsList, state, j, newItem);
		}
		int newChildCount = gridGo.transform.childCount;
		List<Transform> oddItemsToRemove = new List<Transform>(Math.Max(0, newChildCount - bound));
		for (int i = list.Count; i < newChildCount; i++)
		{
			oddItemsToRemove.Add(gridGo.transform.GetChild(i));
		}
		foreach (Transform item2 in oddItemsToRemove)
		{
			NGUITools.Destroy(item2);
		}
		yield return new WaitForEndOfFrame();
		wrap.SortBasedOnScrollMovement();
		wrap.WrapContent();
		UIScrollView scrollView = gridGo.transform.parent.gameObject.GetComponent<UIScrollView>();
		if (scrollView != null)
		{
			scrollView.enabled = true;
			scrollView.ResetPosition();
			scrollView.UpdatePosition();
		}
	}

	private IEnumerator FillTournamentGrid(GameObject gridGo, List<LeaderboardItemViewModel> list, GridState state)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		UIWrapContent wrap = gridGo.GetComponent<UIWrapContent>();
		if (wrap == null)
		{
			throw new InvalidOperationException("Game object does not contain UIWrapContent component.");
		}
		wrap.minIndex = Math.Min(-list.Count + 1, wrap.maxIndex);
		wrap.onInitializeItem = null;
		GameObject gridGo2 = default(GameObject);
		GridState state2 = default(GridState);
		wrap.onInitializeItem = (UIWrapContent.OnInitializeItem)Delegate.Combine(wrap.onInitializeItem, (UIWrapContent.OnInitializeItem)delegate(GameObject go, int wrapIndex, int realIndex)
		{
			int index = -realIndex;
			FillIndividualItem(gridGo2, _tournamentList, state2, index, go);
		});
		int childCount = gridGo.transform.childCount;
		if (childCount == 0)
		{
			Debug.LogError("No children in grid.");
			yield break;
		}
		Transform itemPrototype = gridGo.transform.GetChild(childCount - 1);
		if (itemPrototype == null)
		{
			Debug.LogError("Cannot find prototype for item.");
			yield break;
		}
		_tournamentList.Clear();
		_tournamentList.AddRange(list);
		GameObject itemPrototypeGo = itemPrototype.gameObject;
		itemPrototypeGo.SetActive(_tournamentList.Count > 0);
		int bound = Math.Min(15, _tournamentList.Count);
		for (int i = 0; i != bound; i++)
		{
			LeaderboardItemViewModel item = _tournamentList[i];
			GameObject newItem;
			if (i < childCount)
			{
				newItem = gridGo.transform.GetChild(i).gameObject;
			}
			else
			{
				newItem = NGUITools.AddChild(gridGo, itemPrototypeGo);
				newItem.name = i.ToString(CultureInfo.InvariantCulture);
			}
			FillIndividualItem(gridGo, _tournamentList, state, i, newItem);
		}
		yield return new WaitForEndOfFrame();
		wrap.SortBasedOnScrollMovement();
		wrap.WrapContent();
		UIScrollView scrollView = gridGo.transform.parent.gameObject.GetComponent<UIScrollView>();
		if (scrollView != null)
		{
			scrollView.enabled = true;
			scrollView.ResetPosition();
			scrollView.UpdatePosition();
		}
	}

	internal void RefreshMyLeaderboardEntries()
	{
		foreach (LeaderboardItemViewModel friends in _friendsList)
		{
			if (friends != null && friends.Id == FriendsController.sharedController.id)
			{
				friends.Nickname = ProfileController.GetPlayerNameOrDefault();
				friends.ClanName = FriendsController.sharedController.clanName ?? string.Empty;
				break;
			}
		}
	}

	private void FillIndividualItem(GameObject grid, List<LeaderboardItemViewModel> list, GridState state, int index, GameObject newItem)
	{
		if (index >= list.Count || index < 0)
		{
			return;
		}
		LeaderboardItemViewModel item = list[index];
		int num = index + 1;
		item.Place = num;
		LeaderboardItemView component = newItem.GetComponent<LeaderboardItemView>();
		if (component != null)
		{
			component.NewReset(item);
			if (component.background != null)
			{
				if ((float)num % 2f > 0f)
				{
					Color color = new Color(0.8f, 0.9f, 1f);
					component.GetComponent<UIButton>().defaultColor = color;
					component.background.color = color;
				}
				else
				{
					Color color2 = new Color(1f, 1f, 1f);
					component.GetComponent<UIButton>().defaultColor = color2;
					component.background.color = color2;
				}
			}
		}
		component.Clicked += delegate(object sender, ClickedEventArgs e)
		{
			LeaderboardScript.PlayerClicked.Do(delegate(EventHandler<ClickedEventArgs> handler)
			{
				handler(this, e);
			});
			if (Application.isEditor && Defs.IsDeveloperBuild)
			{
				Debug.Log(string.Format("Clicked: {0}", e.Id));
			}
		};
		UILabel[] componentsInChildren = newItem.GetComponentsInChildren<UILabel>(true);
		Transform[] array = new Transform[3]
		{
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsFirstPlace")).Map((UILabel l) => l.transform),
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsSecondPlace")).Map((UILabel l) => l.transform),
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsThirdPlace")).Map((UILabel l) => l.transform)
		};
		for (int p2 = 0; p2 != array.Length; p2++)
		{
			array[p2].Do(delegate(Transform l)
			{
				l.gameObject.SetActive(p2 + 1 == item.Place && item.WinCount > 0);
			});
		}
		newItem.transform.FindChild("LabelsPlace").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel p)
		{
			p.text = ((item.Place <= 3) ? string.Empty : item.Place.ToString(CultureInfo.InvariantCulture));
		});
	}

	private void FillClanItem(GameObject gridGo, List<LeaderboardItemViewModel> list, GridState state, int index, GameObject newItem)
	{
		if (index >= list.Count)
		{
			return;
		}
		LeaderboardItemViewModel item = list[index];
		item.Place = index + 1;
		LeaderboardItemView component = newItem.GetComponent<LeaderboardItemView>();
		if (component != null)
		{
			component.NewReset(item);
		}
		UILabel[] componentsInChildren = newItem.GetComponentsInChildren<UILabel>(true);
		Transform[] array = new Transform[3]
		{
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsFirstPlace")).Map((UILabel l) => l.transform),
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsSecondPlace")).Map((UILabel l) => l.transform),
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsThirdPlace")).Map((UILabel l) => l.transform)
		};
		for (int p2 = 0; p2 != array.Length; p2++)
		{
			array[p2].Do(delegate(Transform l)
			{
				l.gameObject.SetActive(p2 + 1 == item.Place);
			});
		}
		newItem.transform.FindChild("LabelsPlace").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel p)
		{
			p.text = ((item.Place <= 3) ? string.Empty : item.Place.ToString(CultureInfo.InvariantCulture));
		});
	}

	internal static void SetClanLogo(UITexture s, Texture2D clanLogoTexture)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		Texture mainTexture = s.mainTexture;
		s.mainTexture = ((!(clanLogoTexture != null)) ? null : UnityEngine.Object.Instantiate(clanLogoTexture));
		mainTexture.Do(UnityEngine.Object.Destroy);
	}

	internal static void SetClanLogo(UITexture s, string clanLogo)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		Texture mainTexture = s.mainTexture;
		if (string.IsNullOrEmpty(clanLogo))
		{
			s.mainTexture = null;
		}
		else
		{
			s.mainTexture = LeaderboardItemViewModel.CreateLogoFromBase64String(clanLogo);
		}
		mainTexture.Do(UnityEngine.Object.Destroy);
	}

	private static List<LeaderboardItemViewModel> GroupAndOrder(List<LeaderboardItemViewModel> items)
	{
		List<LeaderboardItemViewModel> list = new List<LeaderboardItemViewModel>();
		IOrderedEnumerable<IGrouping<int, LeaderboardItemViewModel>> orderedEnumerable = from vm in items
			group vm by vm.WinCount into g
			orderby g.Key descending
			select g;
		int num = 1;
		foreach (IGrouping<int, LeaderboardItemViewModel> item in orderedEnumerable)
		{
			IOrderedEnumerable<LeaderboardItemViewModel> orderedEnumerable2 = item.OrderByDescending((LeaderboardItemViewModel vm) => vm.Rank);
			foreach (LeaderboardItemViewModel item2 in orderedEnumerable2)
			{
				item2.Place = num;
				list.Add(item2);
			}
			num += item.Count();
		}
		return list;
	}

	public static int GetLeagueId()
	{
		return (int)RatingSystem.instance.currentLeague;
	}

	internal static void RequestLeaderboards(string playerId)
	{
		if (string.IsNullOrEmpty(playerId))
		{
			throw new ArgumentException("Player id should not be empty", "playerId");
		}
		if (FriendsController.sharedController == null)
		{
			Debug.LogError("Friends controller is null.");
			return;
		}
		if (_currentRequestPromise != null)
		{
			_currentRequestPromise.TrySetCanceled();
		}
		PlayerPrefs.SetString(LeaderboardsResponseCacheTimestamp, DateTime.UtcNow.Subtract(TimeSpan.FromHours(2.0)).ToString("s", CultureInfo.InvariantCulture));
		_currentRequestPromise = new TaskCompletionSource<string>();
		FriendsController.sharedController.StartCoroutine(LoadLeaderboardsCoroutine(playerId, _currentRequestPromise));
	}

	private void HandlePlayerClicked(object sender, ClickedEventArgs e)
	{
		if (FriendsController.sharedController != null && e.Id == FriendsController.sharedController.id)
		{
			return;
		}
		if (Panel == null)
		{
			Debug.LogError("Leaderboards panel not found.");
			return;
		}
		Panel.alpha = float.Epsilon;
		Panel.gameObject.SetActive(false);
		Action<bool> onCloseEvent = delegate
		{
			Panel.gameObject.SetActive(true);
			TopFriendsGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			TopClansGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			TournamentGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			TopFriendsGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			TopClansGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			TournamentGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			Panel.alpha = 1f;
			_profileIsOpened = false;
		};
		_profileIsOpened = true;
		FriendsController.ShowProfile(e.Id, ProfileWindowType.other, onCloseEvent);
	}

	private void Awake()
	{
		Instance = this;
		_view = new LazyObject<LeaderboardsView>(_viewPrefab.ResourcePath, _viewHandler);
		_mainMenuController = new Lazy<MainMenuController>(() => MainMenuController.sharedController);
	}

	private void OnDestroy()
	{
		if (_currentRequestPromise != null)
		{
			_currentRequestPromise.TrySetCanceled();
		}
		_currentRequestPromise = null;
		LeaderboardScript.PlayerClicked = null;
		FriendsController.DisposeProfile();
		_mainMenuController.Value.Do(delegate(MainMenuController m)
		{
			m.BackPressed -= ReturnBack;
		});
		LocalizationStore.DelEventCallAfterLocalize(UpdateLocs);
	}

	public void Show()
	{
		StartCoroutine(ShowCoroutine());
	}

	private IEnumerator ShowCoroutine()
	{
		if (!_isInit)
		{
			if (LeaderboardView != null)
			{
				if (LeaderboardView.backButton != null)
				{
					LeaderboardView.backButton.Clicked += ReturnBack;
				}
				LeaderboardView.OnStateChanged += OnLeaderboardViewStateChanged;
			}
			LeaderboardScript.PlayerClicked = (EventHandler<ClickedEventArgs>)Delegate.Combine(LeaderboardScript.PlayerClicked, new EventHandler<ClickedEventArgs>(HandlePlayerClicked));
			LocalizationStore.AddEventCallAfterLocalize(UpdateLocs);
			_isInit = true;
		}
		if (FriendsController.sharedController == null)
		{
			Debug.LogError("Friends controller is null.");
			yield break;
		}
		string playerId = FriendsController.sharedController.id;
		if (string.IsNullOrEmpty(playerId))
		{
			Debug.LogWarning("Player id should not be null.");
			yield break;
		}
		if (_currentRequestPromise == null)
		{
			_currentRequestPromise = new TaskCompletionSource<string>();
			FriendsController.sharedController.StartCoroutine(LoadLeaderboardsCoroutine(playerId, _currentRequestPromise));
		}
		if (!CurrentRequest.IsCompleted)
		{
			string response2 = PlayerPrefs.GetString(LeaderboardsResponseCache, string.Empty);
			if (string.IsNullOrEmpty(response2))
			{
				yield return StartCoroutine(FillGrids(string.Empty, playerId, _state));
			}
			else
			{
				_state = GridState.FillingWithCache;
				yield return StartCoroutine(FillGrids(response2, playerId, _state));
				_state = GridState.Cache;
			}
		}
		while (!CurrentRequest.IsCompleted)
		{
			yield return null;
		}
		if (CurrentRequest.IsCanceled)
		{
			Debug.LogWarning("Request is cancelled.");
			yield break;
		}
		if (CurrentRequest.IsFaulted)
		{
			Debug.LogException(CurrentRequest.Exception);
			yield break;
		}
		string response = CurrentRequest.Result;
		_state = GridState.FillingWithResponse;
		yield return StartCoroutine(FillGrids(response, playerId, _state));
		_state = GridState.Response;
	}

	public void FillGrids(string rawDara)
	{
		StartCoroutine(FillGrids(rawDara, FriendsController.sharedController.id, GridState.Response));
	}

	private static string FormatExpirationLabel(float expirationTimespanSeconds)
	{
		//Discarded unreachable code: IL_0064, IL_0157
		if (expirationTimespanSeconds < 0f)
		{
			throw new ArgumentOutOfRangeException("expirationTimespanSeconds");
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(expirationTimespanSeconds);
		try
		{
			return string.Format(LocalizationStore.Get("Key_2857"), Convert.ToInt32(Math.Floor(timeSpan.TotalDays)), timeSpan.Hours, timeSpan.Minutes);
		}
		catch
		{
			if (timeSpan.TotalHours < 1.0)
			{
				return string.Format("{0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
			}
			if (timeSpan.TotalDays < 1.0)
			{
				return string.Format("{0}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			return string.Format("{0}d {1}:{2:00}:{3:00}", Convert.ToInt32(Math.Floor(timeSpan.TotalDays)), timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
	}

	private void Update()
	{
		if (!_isInit || !(Time.realtimeSinceStartup > _expirationNextUpateTimeSeconds))
		{
			return;
		}
		_expirationNextUpateTimeSeconds = Time.realtimeSinceStartup + 5f;
		if (!(ExpirationLabel != null))
		{
			return;
		}
		if (_state == GridState.Empty)
		{
			ExpirationLabel.text = LocalizationStore.Key_0348;
			return;
		}
		float num = FriendsController.sharedController.expirationTimeCompetition - Time.realtimeSinceStartup;
		if (num <= 0f)
		{
			ExpirationLabel.text = string.Empty;
			ExpirationIconObject.SetActiveSafe(false);
		}
		else
		{
			ExpirationLabel.text = FormatExpirationLabel(num);
			ExpirationIconObject.SetActiveSafe(true);
		}
	}

	private void OnDisable()
	{
		ScrollToPlayerRunning = false;
	}

	private static IEnumerator LoadLeaderboardsCoroutine(string playerId, TaskCompletionSource<string> requestPromise)
	{
		if (requestPromise == null)
		{
			throw new ArgumentNullException("requestPromise");
		}
		if (requestPromise.Task.IsCanceled)
		{
			yield break;
		}
		if (string.IsNullOrEmpty(playerId))
		{
			throw new ArgumentException("Player id should not be null.", "playerId");
		}
		if (FriendsController.sharedController == null)
		{
			throw new InvalidOperationException("Friends controller should not be null.");
		}
		if (string.IsNullOrEmpty(FriendsController.sharedController.id))
		{
			Debug.LogWarning("Current player id is empty.");
			requestPromise.TrySetException(new InvalidOperationException("Current player id is empty."));
			yield break;
		}
		string leaderboardsResponseCacheTimestampString = PlayerPrefs.GetString(LeaderboardsResponseCacheTimestamp, string.Empty);
		DateTime leaderboardsResponseCacheTimestamp;
		if (DateTime.TryParse(leaderboardsResponseCacheTimestampString, out leaderboardsResponseCacheTimestamp))
		{
			DateTime timeOfNnextRequest = leaderboardsResponseCacheTimestamp + TimeSpan.FromMinutes(Defs.pauseUpdateLeaderboard);
			float secondsTillNextRequest = (float)(timeOfNnextRequest - DateTime.UtcNow).TotalSeconds;
			if (secondsTillNextRequest > 3600f)
			{
				secondsTillNextRequest = 0f;
			}
			yield return new WaitForSeconds(secondsTillNextRequest);
		}
		if (requestPromise.Task.IsCanceled)
		{
			yield break;
		}
		int leagueId = GetLeagueId();
		WWWForm form = new WWWForm();
		form.AddField("action", "get_leaderboards_wins_tiers");
		form.AddField("app_version", string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion));
		form.AddField("uniq_id", FriendsController.sharedController.id);
		int _tier = ExpController.OurTierForAnyPlace();
		form.AddField("tier", _tier);
		form.AddField("auth", FriendsController.Hash("get_leaderboards_wins_tiers"));
		form.AddField("league_id", leagueId);
		form.AddField("competition_id", FriendsController.sharedController.currentCompetition);
		WWW request = Tools.CreateWwwIfNotConnected(FriendsController.actionAddress, form, string.Empty);
		if (request == null)
		{
			requestPromise.TrySetException(new InvalidOperationException("Request forbidden while connected."));
			TaskCompletionSource<string> newRequestPromise3 = (_currentRequestPromise = new TaskCompletionSource<string>());
			yield return new WaitForSeconds(Defs.timeUpdateLeaderboardIfNullResponce);
			FriendsController.sharedController.StartCoroutine(LoadLeaderboardsCoroutine(playerId, newRequestPromise3));
			yield break;
		}
		while (!request.isDone)
		{
			if (requestPromise.Task.IsCanceled)
			{
				yield break;
			}
			yield return null;
		}
		if (!string.IsNullOrEmpty(request.error))
		{
			requestPromise.TrySetException(new InvalidOperationException(request.error));
			Debug.LogError(request.error);
			TaskCompletionSource<string> newRequestPromise2 = (_currentRequestPromise = new TaskCompletionSource<string>());
			yield return new WaitForSeconds(Defs.timeUpdateLeaderboardIfNullResponce);
			FriendsController.sharedController.StartCoroutine(LoadLeaderboardsCoroutine(playerId, newRequestPromise2));
			yield break;
		}
		string responseText = URLs.Sanitize(request);
		if (string.IsNullOrEmpty(responseText) || responseText == "fail")
		{
			string message = string.Format("Leaderboars response: '{0}'", responseText);
			requestPromise.TrySetException(new InvalidOperationException(message));
			Debug.LogWarning(message);
			TaskCompletionSource<string> newRequestPromise = (_currentRequestPromise = new TaskCompletionSource<string>());
			yield return new WaitForSeconds(Defs.timeUpdateLeaderboardIfNullResponce);
			FriendsController.sharedController.StartCoroutine(LoadLeaderboardsCoroutine(playerId, newRequestPromise));
		}
		else
		{
			requestPromise.TrySetResult(responseText);
			PlayerPrefs.SetString(LeaderboardsResponseCache, responseText);
			PlayerPrefs.SetString(LeaderboardsResponseCacheTimestamp, DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture));
		}
	}

	public Task GetReturnFuture()
	{
		if (_returnPromise.Task.IsCompleted)
		{
			_returnPromise = new TaskCompletionSource<bool>();
		}
		_mainMenuController.Value.Do(delegate(MainMenuController m)
		{
			m.BackPressed -= ReturnBack;
		});
		_mainMenuController.Value.Do(delegate(MainMenuController m)
		{
			m.BackPressed += ReturnBack;
		});
		return _returnPromise.Task;
	}

	public void ReturnBack(object sender, EventArgs e)
	{
		if (!_profileIsOpened)
		{
			TopFriendsGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			TopClansGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			TournamentGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			TopFriendsGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			TopClansGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			TournamentGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			_returnPromise.TrySetResult(true);
			_mainMenuController.Value.Do(delegate(MainMenuController m)
			{
				m.BackPressed -= ReturnBack;
			});
		}
	}
}

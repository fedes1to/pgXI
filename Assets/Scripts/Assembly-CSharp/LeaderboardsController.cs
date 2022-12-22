using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

internal sealed class LeaderboardsController : MonoBehaviour
{
	private LeaderboardsView _leaderboardsView;

	private FriendsGUIController _friendsGuiController;

	private string _playerId = string.Empty;

	public LeaderboardsView LeaderboardsView
	{
		private get
		{
			return _leaderboardsView;
		}
		set
		{
			_leaderboardsView = value;
		}
	}

	public FriendsGUIController FriendsGuiController
	{
		private get
		{
			return _friendsGuiController;
		}
		set
		{
			_friendsGuiController = value;
		}
	}

	public string PlayerId
	{
		private get
		{
			return _playerId;
		}
		set
		{
			_playerId = value ?? string.Empty;
		}
	}

	public void RequestLeaderboards()
	{
		if (!string.IsNullOrEmpty(_playerId))
		{
			StartCoroutine(GetLeaderboardsCoroutine(_playerId));
		}
		else
		{
			Debug.Log("Player id should not be empty.");
		}
	}

	internal static List<LeaderboardItemViewModel> ParseLeaderboardEntries(string entryId, string leaderboardName, Dictionary<string, object> response)
	{
		if (string.IsNullOrEmpty(leaderboardName))
		{
			throw new ArgumentException("Leaderbord should not be empty.", "leaderboardName");
		}
		if (response == null)
		{
			throw new ArgumentNullException("response");
		}
		List<LeaderboardItemViewModel> list = new List<LeaderboardItemViewModel>();
		object value;
		if (response.TryGetValue(leaderboardName, out value))
		{
			List<object> list2 = value as List<object>;
			if (list2 != null)
			{
				IEnumerable<Dictionary<string, object>> enumerable = list2.OfType<Dictionary<string, object>>();
				{
					foreach (Dictionary<string, object> item in enumerable)
					{
						LeaderboardItemViewModel leaderboardItemViewModel = new LeaderboardItemViewModel();
						object value2;
						if (item.TryGetValue("id", out value2))
						{
							leaderboardItemViewModel.Id = Convert.ToString(value2);
							leaderboardItemViewModel.Highlight = !string.IsNullOrEmpty(leaderboardItemViewModel.Id) && leaderboardItemViewModel.Id.Equals(entryId);
						}
						object value3;
						int result;
						if (item.TryGetValue("rank", out value3) && int.TryParse(value3 as string, out result))
						{
							leaderboardItemViewModel.Rank = result;
						}
						else if (item.TryGetValue("member_cnt", out value3))
						{
							try
							{
								leaderboardItemViewModel.Rank = Convert.ToInt32(value3);
							}
							catch (Exception exception)
							{
								Debug.LogException(exception);
							}
						}
						object value4;
						if (item.TryGetValue("nick", out value4))
						{
							leaderboardItemViewModel.Nickname = (value4 as string) ?? string.Empty;
						}
						else if (item.TryGetValue("name", out value4))
						{
							leaderboardItemViewModel.Nickname = (value4 as string) ?? string.Empty;
						}
						try
						{
							object value5;
							if (item.TryGetValue("trophies", out value5))
							{
								leaderboardItemViewModel.WinCount = Convert.ToInt32(value5);
							}
							else if (item.TryGetValue("wins", out value5))
							{
								leaderboardItemViewModel.WinCount = Convert.ToInt32(value5);
							}
							else if (item.TryGetValue("win", out value5))
							{
								leaderboardItemViewModel.WinCount = Convert.ToInt32(value5);
							}
						}
						catch (Exception exception2)
						{
							Debug.LogException(exception2);
						}
						object value6;
						if (item.TryGetValue("logo", out value6))
						{
							leaderboardItemViewModel.ClanLogo = (value6 as string) ?? string.Empty;
						}
						else
						{
							leaderboardItemViewModel.ClanLogo = string.Empty;
						}
						object value7;
						if (item.TryGetValue("name", out value7))
						{
							leaderboardItemViewModel.ClanName = (value7 as string) ?? string.Empty;
						}
						else if (item.TryGetValue("clan_name", out value7))
						{
							leaderboardItemViewModel.ClanName = (value7 as string) ?? string.Empty;
						}
						else
						{
							leaderboardItemViewModel.ClanName = string.Empty;
						}
						list.Add(leaderboardItemViewModel);
					}
					return list;
				}
			}
		}
		return list;
	}

	private void Start()
	{
		RequestLeaderboards();
	}

	private IEnumerator GetLeaderboardsCoroutine(string playerId)
	{
		if (string.IsNullOrEmpty(playerId))
		{
			Debug.LogWarning("Player id should not be empty.");
			yield break;
		}
		Debug.Log("LeaderboardsController.GetLeaderboardsCoroutine(" + playerId + ")");
		WWWForm form = new WWWForm();
		form.AddField("action", "get_leaderboards_league");
		form.AddField("app_version", string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion));
		form.AddField("id", playerId);
		form.AddField("league_id", LeaderboardScript.GetLeagueId());
		form.AddField("uniq_id", FriendsController.sharedController.id);
		form.AddField("auth", FriendsController.Hash("get_leaderboards_league"));
		if (FriendsController.sharedController.NumberOfBestPlayersRequests > 0)
		{
			Debug.Log("Waiting previous leaderboards request...");
			while (FriendsController.sharedController.NumberOfBestPlayersRequests > 0)
			{
				yield return null;
			}
		}
		FriendsController.sharedController.NumberOfBestPlayersRequests++;
		WWW request = Tools.CreateWwwIfNotConnected(FriendsController.actionAddress, form, string.Empty);
		yield return request;
		FriendsController.sharedController.NumberOfBestPlayersRequests--;
		HandleRequestCompleted(request);
	}

	private void HandleRequestCompleted(WWW request)
	{
		if (Application.isEditor)
		{
			Debug.Log("HandleRequestCompleted()");
		}
		if (request == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(request.error))
		{
			Debug.LogWarning(request.error);
			return;
		}
		string text = URLs.Sanitize(request);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogWarning("Leaderboars response is empty.");
		}
		else
		{
			LeaderboardScript.Instance.FillGrids(text);
		}
	}
}

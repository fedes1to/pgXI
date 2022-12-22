using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rilisoft;
using Rilisoft.MiniJson;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class ClanIncomingInvitesController : MonoBehaviour
{
	public ClansGUIController clansGui;

	public UIGrid clanIncomingInvitesGrid;

	public ClanIncomingInviteView clanIncomingInviteViewPrototype;

	public GameObject clanPanel;

	public GameObject noClanPanel;

	public GameObject inboxPanel;

	public GameObject noClanIncomingInvitesLabel;

	public GameObject cannotAcceptClanIncomingInvitesLabel;

	private Action _back;

	private static Task<List<object>> _currentRequest;

	private static CancellationTokenSource _cts;

	internal static Task<List<object>> CurrentRequest
	{
		get
		{
			return _currentRequest;
		}
	}

	public void HandleInboxPressed()
	{
		ClansGUIController.State previousState = clansGui.Map((ClansGUIController c) => c.CurrentState);
		inboxPanel.Do(delegate(GameObject i)
		{
			i.SetActive(true);
		});
		clanPanel.Do(delegate(GameObject i)
		{
			i.SetActive(false);
		});
		noClanPanel.Do(delegate(GameObject i)
		{
			i.SetActive(false);
		});
		clansGui.Do(delegate(ClansGUIController c)
		{
			c.CurrentState = ClansGUIController.State.Inbox;
		});
		StartCoroutine(RepositionAfterPause());
		_back = delegate
		{
			bool inClan = !string.IsNullOrEmpty(FriendsController.sharedController.Map((FriendsController f) => f.ClanID));
			inboxPanel.Do(delegate(GameObject i)
			{
				i.SetActive(false);
			});
			clanPanel.Do(delegate(GameObject i)
			{
				i.SetActive(inClan);
			});
			noClanPanel.Do(delegate(GameObject i)
			{
				i.SetActive(!inClan);
			});
			clansGui.Do(delegate(ClansGUIController c)
			{
				c.CurrentState = previousState;
			});
		};
	}

	public void HandleBackFromInboxPressed()
	{
		if (_back != null)
		{
			_back();
		}
	}

	internal static void FetchClanIncomingInvites(string playerId)
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
		_cts = new CancellationTokenSource();
		_currentRequest = RequestClanIncomingInvitesAsync(playerId, _cts.Token);
	}

	internal static Task<List<object>> RequestClanIncomingInvitesAsync(string playerId, float delay, CancellationToken ct)
	{
		if (string.IsNullOrEmpty(playerId))
		{
			throw new ArgumentException("Player id should not be empty", "playerId");
		}
		if (FriendsController.sharedController == null)
		{
			throw new InvalidOperationException("FriendsController instance should not be null.");
		}
		TaskCompletionSource<List<object>> taskCompletionSource = new TaskCompletionSource<List<object>>();
		FriendsController.sharedController.StartCoroutine(RequestClanIncomingInvitesCoroutine(playerId, delay, taskCompletionSource, ct));
		return taskCompletionSource.Task;
	}

	internal static Task<List<object>> RequestClanIncomingInvitesAsync(string playerId, CancellationToken ct)
	{
		return RequestClanIncomingInvitesAsync(playerId, 0f, ct);
	}

	internal static Task<List<object>> RequestClanIncomingInvitesAsync(string playerId)
	{
		return RequestClanIncomingInvitesAsync(playerId, 0f, CancellationToken.None);
	}

	internal void Refresh()
	{
		if (clanIncomingInvitesGrid != null && noClanIncomingInvitesLabel != null)
		{
			bool flag = clanIncomingInvitesGrid.transform.childCount == 0;
			GameObject gameObject = clansGui.Map((ClansGUIController c) => c.receivingPlashka);
			bool active = ((gameObject == null) ? flag : (flag && !gameObject.activeInHierarchy));
			noClanIncomingInvitesLabel.gameObject.SetActive(active);
		}
		if (cannotAcceptClanIncomingInvitesLabel != null)
		{
			bool flag2 = string.IsNullOrEmpty(FriendsController.sharedController.ClanID);
			bool active2 = ((noClanIncomingInvitesLabel == null) ? (!flag2) : (!flag2 && !noClanIncomingInvitesLabel.activeInHierarchy));
			cannotAcceptClanIncomingInvitesLabel.SetActive(active2);
		}
	}

	private static IEnumerator RequestClanIncomingInvitesCoroutine(string playerId, float delay, TaskCompletionSource<List<object>> promise, CancellationToken ct)
	{
		while (!TrainingController.TrainingCompleted)
		{
			yield return null;
		}
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		if (ct.IsCancellationRequested)
		{
			promise.TrySetCanceled();
			yield break;
		}
		if (string.IsNullOrEmpty(FriendsController.sharedController.id))
		{
			Debug.LogWarning("Current player id is empty.");
			promise.TrySetException(new InvalidOperationException("Current player id is empty."));
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "get_clan_incoming_invites");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id_player", playerId);
		form.AddField("uniq_id", FriendsController.sharedController.id);
		form.AddField("auth", FriendsController.Hash("get_clan_incoming_invites"));
		WWW request = Tools.CreateWwwIfNotConnected(FriendsController.actionAddress, form, string.Empty);
		if (request == null)
		{
			promise.TrySetException(new InvalidOperationException("Request was not performed because player is connected to Photon."));
			_currentRequest = RequestClanIncomingInvitesAsync(playerId, 10f, ct);
			yield break;
		}
		while (!request.isDone)
		{
			yield return null;
		}
		if (!string.IsNullOrEmpty(request.error))
		{
			Debug.LogError(request.error);
			promise.TrySetException(new InvalidOperationException(request.error));
			_currentRequest = RequestClanIncomingInvitesAsync(playerId, 10f, ct);
			yield break;
		}
		string responseText = URLs.Sanitize(request);
		if (string.IsNullOrEmpty(responseText))
		{
			Debug.LogWarning("Clan incoming invites response is empty.");
			promise.TrySetException(new InvalidOperationException("Clan incoming invites response is empty."));
			_currentRequest = RequestClanIncomingInvitesAsync(playerId, 10f, ct);
			yield break;
		}
		Dictionary<string, object> d = Json.Deserialize(responseText) as Dictionary<string, object>;
		object invites;
		if (d != null && d.TryGetValue("clans_invites", out invites))
		{
			List<object> result = invites as List<object>;
			if (invites == null)
			{
				promise.TrySetException(new InvalidOperationException("“clans_invites” could not be parsed."));
			}
			else
			{
				promise.TrySetResult(result);
			}
		}
		else
		{
			promise.TrySetException(new InvalidOperationException("“clans_invites” not found."));
		}
	}

	internal static void SetRequestDirty()
	{
		if (_currentRequest == null)
		{
			return;
		}
		if (!_currentRequest.IsCompleted)
		{
			_cts.Do(delegate(CancellationTokenSource c)
			{
				c.Cancel();
			});
		}
		_cts = new CancellationTokenSource();
		_currentRequest = null;
	}

	private IEnumerator Start()
	{
		Refresh();
		if (FriendsController.sharedController == null)
		{
			Debug.LogError("Friends controller is null.");
			yield break;
		}
		string playerId = FriendsController.sharedController.id;
		if (string.IsNullOrEmpty(playerId))
		{
			Debug.LogError("Player id should not be null.");
			yield break;
		}
		if (CurrentRequest == null)
		{
			_cts = new CancellationTokenSource();
			_currentRequest = RequestClanIncomingInvitesAsync(playerId, _cts.Token);
		}
		else if (CurrentRequest.IsCompleted && (CurrentRequest.IsCanceled || CurrentRequest.IsFaulted))
		{
			_cts = new CancellationTokenSource();
			_currentRequest = RequestClanIncomingInvitesAsync(playerId, _cts.Token);
		}
		if (!CurrentRequest.IsCompleted)
		{
		}
		while (!CurrentRequest.IsCompleted)
		{
			yield return null;
		}
		if (CurrentRequest.IsCanceled)
		{
			Debug.LogWarning("Request is cancelled.");
		}
		else if (CurrentRequest.IsFaulted)
		{
			Debug.LogException(CurrentRequest.Exception);
		}
		else if (clanIncomingInviteViewPrototype != null && clanIncomingInvitesGrid != null)
		{
			List<object> inviteList = CurrentRequest.Result;
			if (inviteList != null)
			{
				List<Dictionary<string, object>> invites = inviteList.OfType<Dictionary<string, object>>().ToList();
				clanIncomingInviteViewPrototype.gameObject.SetActive(invites.Count > 0);
				foreach (Dictionary<string, object> invite in invites)
				{
					GameObject newItem = NGUITools.AddChild(clanIncomingInvitesGrid.gameObject, clanIncomingInviteViewPrototype.gameObject);
					ClanIncomingInviteView c = newItem.GetComponent<ClanIncomingInviteView>();
					if (c != null)
					{
						object clanId;
						if (invite.TryGetValue("id", out clanId))
						{
							c.ClanId = Convert.ToString(clanId);
						}
						else
						{
							c.ClanId = string.Empty;
						}
						object clanName;
						if (invite.TryGetValue("name", out clanName))
						{
							c.ClanName = Convert.ToString(clanName);
						}
						else
						{
							c.ClanName = string.Empty;
						}
						object clanLogo;
						if (invite.TryGetValue("logo", out clanLogo))
						{
							c.ClanLogo = Convert.ToString(clanLogo);
						}
						else
						{
							c.ClanLogo = string.Empty;
						}
						object clanCreatorId;
						if (invite.TryGetValue("creator_id", out clanCreatorId))
						{
							c.ClanCreatorId = Convert.ToString(clanLogo);
						}
						else
						{
							c.ClanCreatorId = string.Empty;
						}
					}
				}
				clanIncomingInvitesGrid.transform.parent.GetComponent<UIScrollView>().Do(delegate(UIScrollView s)
				{
					s.disableDragIfFits = true;
				});
			}
			clanIncomingInviteViewPrototype.gameObject.SetActive(false);
			yield return new WaitForEndOfFrame();
			clanIncomingInvitesGrid.Reposition();
		}
		Refresh();
	}

	private IEnumerator RepositionAfterPause()
	{
		yield return new WaitForEndOfFrame();
		clanIncomingInvitesGrid.Do(delegate(UIGrid g)
		{
			g.Reposition();
		});
		Refresh();
	}
}

using System;
using System.Collections;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class ClanIncomingInviteView : MonoBehaviour
{
	public UIButton acceptButton;

	public UIButton rejectButton;

	public UITexture clanLogo;

	public UILabel clanName;

	private string _clanName = string.Empty;

	private string _clanLogo = string.Empty;

	public string ClanId { get; set; }

	public string ClanCreatorId { get; set; }

	public string ClanName
	{
		get
		{
			return _clanName ?? string.Empty;
		}
		set
		{
			_clanName = value ?? string.Empty;
			clanName.Do(delegate(UILabel l)
			{
				l.text = _clanName;
			});
		}
	}

	public string ClanLogo
	{
		get
		{
			return _clanLogo ?? string.Empty;
		}
		set
		{
			_clanLogo = value ?? string.Empty;
			clanLogo.Do(delegate(UITexture t)
			{
				LeaderboardScript.SetClanLogo(t, _clanLogo);
			});
		}
	}

	public void HandleAcceptButton()
	{
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("Accept invite to clan {0} ({1})", ClanName, ClanId);
			Debug.Log(message);
		}
		FriendsController.sharedController.Do(delegate(FriendsController f)
		{
			f.StartCoroutine(AcceptClanInviteCoroutine());
		});
		ClanIncomingInvitesController.SetRequestDirty();
	}

	public void HandleRejectButton()
	{
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("Reject invite to clan {0} ({1})", ClanName, ClanId);
			Debug.Log(message);
		}
		FriendsController.sharedController.Do(delegate(FriendsController f)
		{
			f.StartCoroutine(RejectClanInviteCoroutine());
		});
		ClanIncomingInvitesController.SetRequestDirty();
	}

	private void Start()
	{
		Refresh();
	}

	private void OnEnable()
	{
		Refresh();
	}

	internal void Refresh()
	{
		if (acceptButton != null && rejectButton != null && FriendsController.sharedController != null)
		{
			bool flag = string.IsNullOrEmpty(FriendsController.sharedController.ClanID);
			Vector3[] array = (flag ? new Vector3[2]
			{
				rejectButton.transform.localPosition,
				acceptButton.transform.localPosition
			} : new Vector3[2]
			{
				acceptButton.transform.localPosition,
				rejectButton.transform.localPosition
			});
			rejectButton.transform.localPosition = array[0];
			acceptButton.transform.localPosition = array[1];
			acceptButton.gameObject.SetActive(flag);
		}
	}

	private IEnumerator AcceptClanInviteCoroutine()
	{
		if (FriendsController.sharedController == null)
		{
			yield break;
		}
		string playerId = FriendsController.sharedController.id;
		if (string.IsNullOrEmpty(playerId))
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "accept_invite");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id_player", playerId);
		form.AddField("id_clan", ClanId ?? string.Empty);
		form.AddField("uniq_id", FriendsController.sharedController.id);
		form.AddField("auth", FriendsController.Hash("accept_invite"));
		acceptButton.Do(delegate(UIButton b)
		{
			b.isEnabled = false;
		});
		rejectButton.Do(delegate(UIButton b)
		{
			b.isEnabled = false;
		});
		try
		{
			WWW request = Tools.CreateWwwIfNotConnected(FriendsController.actionAddress, form, string.Empty);
			if (request == null)
			{
				yield break;
			}
			while (!request.isDone)
			{
				yield return null;
			}
			if (!string.IsNullOrEmpty(request.error))
			{
				Debug.LogError(request.error);
				yield break;
			}
			string responseText = URLs.Sanitize(request);
			if ("fail".Equals(responseText, StringComparison.OrdinalIgnoreCase))
			{
				Debug.LogError("accept_invite failed.");
				yield break;
			}
			FriendsController.sharedController.clanLogo = ClanLogo;
			FriendsController.sharedController.ClanID = ClanId ?? string.Empty;
			FriendsController.sharedController.clanName = ClanName;
			FriendsController.sharedController.clanLeaderID = ClanCreatorId ?? string.Empty;
			if (ClansGUIController.sharedController != null)
			{
				ClansGUIController.sharedController.nameClanLabel.text = FriendsController.sharedController.clanName;
			}
			UIGrid g = base.transform.parent.GetComponent<UIGrid>();
			if (g != null)
			{
				NGUITools.Destroy(base.transform);
				yield return new WaitForEndOfFrame();
				g.Reposition();
				acceptButton = null;
				rejectButton = null;
				g.GetComponentInParent<ClanIncomingInvitesController>().Do(delegate(ClanIncomingInvitesController c)
				{
					c.Refresh();
				});
			}
			ClanIncomingInviteView[] views = g.GetComponentsInChildren<ClanIncomingInviteView>();
			ClanIncomingInviteView[] array = views;
			foreach (ClanIncomingInviteView view in array)
			{
				view.Refresh();
			}
		}
		finally
		{
			acceptButton.Do(delegate(UIButton b)
			{
				b.isEnabled = string.IsNullOrEmpty(FriendsController.sharedController.ClanID);
			});
			rejectButton.Do(delegate(UIButton b)
			{
				b.isEnabled = true;
			});
		}
	}

	private IEnumerator RejectClanInviteCoroutine()
	{
		string playerId = FriendsController.sharedController.Map((FriendsController sc) => sc.id) ?? string.Empty;
		if (string.IsNullOrEmpty(playerId))
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "reject_invite");
		form.AddField("app_version", ProtocolListGetter.CurrentPlatform + ":" + GlobalGameController.AppVersion);
		form.AddField("id_player", playerId);
		form.AddField("id_clan", ClanId ?? string.Empty);
		form.AddField("id", playerId);
		form.AddField("uniq_id", FriendsController.sharedController.id);
		form.AddField("auth", FriendsController.Hash("reject_invite"));
		acceptButton.Do(delegate(UIButton b)
		{
			b.isEnabled = false;
		});
		rejectButton.Do(delegate(UIButton b)
		{
			b.isEnabled = false;
		});
		try
		{
			WWW request = Tools.CreateWwwIfNotConnected(FriendsController.actionAddress, form, string.Empty);
			if (request == null)
			{
				yield break;
			}
			while (!request.isDone)
			{
				yield return null;
			}
			if (!string.IsNullOrEmpty(request.error))
			{
				Debug.LogError(request.error);
				yield break;
			}
			string responseText = URLs.Sanitize(request);
			if ("fail".Equals(responseText, StringComparison.OrdinalIgnoreCase))
			{
				Debug.LogError("reject_invite failed.");
				yield break;
			}
			UIGrid g = base.transform.parent.GetComponent<UIGrid>();
			if (g != null)
			{
				NGUITools.Destroy(base.transform);
				yield return new WaitForEndOfFrame();
				g.Reposition();
				acceptButton = null;
				rejectButton = null;
				g.GetComponentInParent<ClanIncomingInvitesController>().Do(delegate(ClanIncomingInvitesController c)
				{
					c.Refresh();
				});
			}
		}
		finally
		{
			acceptButton.Do(delegate(UIButton b)
			{
				b.isEnabled = string.IsNullOrEmpty(FriendsController.sharedController.ClanID);
			});
			rejectButton.Do(delegate(UIButton b)
			{
				b.isEnabled = true;
			});
		}
	}
}

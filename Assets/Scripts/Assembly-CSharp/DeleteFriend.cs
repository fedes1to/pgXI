using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class DeleteFriend : MonoBehaviour
{
	private void OnClick()
	{
		ButtonClickSound.Instance.PlayClick();
		if (!base.transform.parent.GetComponent<FriendPreview>().ClanMember)
		{
			string clanID = FriendsController.sharedController.ClanID;
			string clanLeaderID = FriendsController.sharedController.clanLeaderID;
			FriendPreview fp = base.transform.parent.GetComponent<FriendPreview>();
			if (!string.IsNullOrEmpty(clanID) && !string.IsNullOrEmpty(clanLeaderID) && fp.id != null && FriendsController.sharedController.clanLeaderID.Equals(fp.id))
			{
				FriendsController.sharedController.RejectInvite(fp.id);
				fp.DisableButtons();
				FriendsController.sharedController.ExitClan(null);
			}
			else if (IsDeletePlayerFromClan(clanID, clanLeaderID, fp.id))
			{
				FriendsController.sharedController.RejectInvite(fp.id, delegate(bool ok)
				{
					if (ok)
					{
						FriendsController.sharedController.friendsDeletedLocal.Add(fp.id);
					}
					else
					{
						FriendsController.sharedController.friendsDeletedLocal.Remove(fp.id);
					}
				});
				fp.DisableButtons();
				FriendsController.sharedController.ExitClan(fp.id);
			}
			else
			{
				FriendsController.sharedController.RejectInvite(fp.id);
				fp.DisableButtons();
			}
		}
		else
		{
			FriendsController.sharedController.clanDeletedLocal.Add(base.transform.parent.GetComponent<FriendPreview>().id);
			FriendsController.sharedController.DeleteClanMember(base.transform.parent.GetComponent<FriendPreview>().id);
		}
	}

	private bool IsDeletePlayerFromClan(string clanId, string leaderId, string friendId)
	{
		FriendsController sharedController = FriendsController.sharedController;
		if (string.IsNullOrEmpty(sharedController.id))
		{
			return false;
		}
		bool flag = !string.IsNullOrEmpty(clanId) && !string.IsNullOrEmpty(leaderId);
		bool flag2 = sharedController.id.Equals(sharedController.clanLeaderID);
		bool flag3 = friendId != null && sharedController.playersInfo.ContainsKey(friendId);
		if (!flag || !flag2 || !flag3)
		{
			return false;
		}
		Dictionary<string, object> dictionary = sharedController.playersInfo[friendId];
		if (dictionary == null)
		{
			return false;
		}
		Dictionary<string, object> dictionary2 = dictionary["player"] as Dictionary<string, object>;
		bool result = false;
		if (dictionary2 != null && dictionary2["clan_creator_id"] != null)
		{
			string text = Convert.ToString(dictionary2["clan_creator_id"]);
			if (text != null)
			{
				result = text.Equals(sharedController.clanLeaderID);
			}
		}
		return result;
	}
}

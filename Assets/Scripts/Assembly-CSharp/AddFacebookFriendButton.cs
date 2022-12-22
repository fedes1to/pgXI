using System.Collections.Generic;
using UnityEngine;

internal sealed class AddFacebookFriendButton : MonoBehaviour
{
	private void OnClick()
	{
		FriendPreview component = base.transform.parent.GetComponent<FriendPreview>();
		ButtonClickSound.Instance.PlayClick();
		string id = component.id;
		if (id != null)
		{
			if (component.ClanInvite)
			{
				FriendsController.SendPlayerInviteToClan(id);
				FriendsController.sharedController.clanSentInvitesLocal.Add(id);
			}
			else
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("Added Friends", "Find Friends: Facebook");
				dictionary.Add("Deleted Friends", "Add");
				dictionary.Add("Search Friends", "Add");
				Dictionary<string, object> socialEventParameters = dictionary;
				FriendsController.sharedController.SendInvitation(id, socialEventParameters);
			}
		}
		if (!component.ClanInvite)
		{
			component.DisableButtons();
		}
	}
}

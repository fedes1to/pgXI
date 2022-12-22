using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public class NewMessageIconController : MonoBehaviour
{
	public bool privateMessageFriends;

	public bool inviteToFriends;

	public bool inviteToClan;

	public bool clanMessages;

	public bool privateMessageClan;

	public GameObject newMessageSprite;

	public UILabel countLabel;

	private void Start()
	{
		UpdateStateNewMessage();
	}

	private void UpdateStateNewMessage()
	{
		bool flag = false;
		int num = 0;
		if (privateMessageFriends && ChatController.countNewPrivateMessage > 0)
		{
			flag = true;
			num += ChatController.countNewPrivateMessage;
		}
		if (inviteToFriends)
		{
			HashSet<string> hashSet = BattleInviteListener.Instance.GetFriendIds() as HashSet<string>;
			if (hashSet != null && hashSet.Count > 0)
			{
				num++;
				flag = true;
			}
			else if (FriendsController.sharedController.invitesToUs.Count > 0)
			{
				for (int i = 0; i < FriendsController.sharedController.invitesToUs.Count; i++)
				{
					string key = FriendsController.sharedController.invitesToUs[i];
					if (FriendsController.sharedController.friendsInfo.ContainsKey(key))
					{
						flag = true;
						num++;
					}
					else if (FriendsController.sharedController.clanFriendsInfo.ContainsKey(key))
					{
						flag = true;
						num++;
					}
					else if (FriendsController.sharedController.profileInfo.ContainsKey(key))
					{
						flag = true;
						num++;
					}
				}
			}
		}
		if (newMessageSprite != null && flag != newMessageSprite.activeSelf)
		{
			newMessageSprite.SetActive(flag);
		}
		if (countLabel != null)
		{
			countLabel.text = num.ToString();
		}
	}

	private void Update()
	{
		UpdateStateNewMessage();
	}
}

using System.Collections.Generic;
using UnityEngine;

public class FriendPrevInChatItem : MonoBehaviour
{
	public UILabel nickLabel;

	public UITexture previewTexture;

	public UISprite rank;

	public string playerID;

	public GameObject newMessageObj;

	public UILabel countNewMessageLabel;

	private int contNewMessage;

	public int myWrapIndex;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void UpdateCountNewMessage()
	{
		int num = 0;
		if (ChatController.privateMessages.ContainsKey(playerID))
		{
			List<ChatController.PrivateMessage> list = ChatController.privateMessages[playerID];
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].isRead)
				{
					num++;
				}
			}
		}
		contNewMessage = num;
		if (contNewMessage == 0)
		{
			newMessageObj.SetActive(false);
			return;
		}
		newMessageObj.SetActive(true);
		countNewMessageLabel.text = contNewMessage.ToString();
	}

	public void SetActivePlayer()
	{
		if (!(PrivateChatController.sharedController.selectedPlayerID == playerID))
		{
			if (ButtonClickSound.Instance != null)
			{
				ButtonClickSound.Instance.PlayClick();
			}
			PrivateChatController.sharedController.SetSelectedPlayer(playerID, false);
		}
	}
}

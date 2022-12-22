using System;
using System.Collections.Generic;
using System.Globalization;
using Rilisoft.MiniJson;
using UnityEngine;

public class PrivateMessageItem : MonoBehaviour
{
	[Header("Your message obj")]
	public GameObject yourFonSprite;

	public UILabel yourMessageLabel;

	public UILabel yourTimeLabel;

	public UISprite yourSmileSprite;

	public UIWidget yourWidget;

	[Header("Other message obj")]
	public GameObject otherFonSprite;

	public UILabel otherMessageLabel;

	public UILabel otherTimeLabel;

	public UISprite otherSmileSprite;

	public UIWidget otherWidget;

	[NonSerialized]
	public bool isRead;

	public string timeStamp;

	private UIPanel myPanel;

	private Transform myTransform;

	public int myWrapIndex;

	private void Awake()
	{
		myTransform = base.transform;
	}

	private void Start()
	{
		if (myTransform.parent != null)
		{
			myPanel = base.transform.parent.parent.GetComponent<UIPanel>();
		}
	}

	public void SetFon(bool isMine)
	{
		yourWidget.gameObject.SetActive(isMine);
		otherWidget.gameObject.SetActive(!isMine);
	}

	public void SetWidth(int width)
	{
		otherWidget.width = width;
		yourWidget.width = width;
	}

	private void Update()
	{
		if (isRead || !(myTransform.localPosition.y >= myPanel.clipOffset.y - myPanel.baseClipRegion.w * 0.5f + myPanel.baseClipRegion.y) || !(myTransform.localPosition.y <= myPanel.clipOffset.y + myPanel.baseClipRegion.w * 0.5f + myPanel.baseClipRegion.y))
		{
			return;
		}
		Dictionary<string, List<Dictionary<string, string>>> dictionary = new Dictionary<string, List<Dictionary<string, string>>>();
		foreach (KeyValuePair<string, List<ChatController.PrivateMessage>> privateMessage in ChatController.privateMessages)
		{
			List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
			foreach (ChatController.PrivateMessage item in privateMessage.Value)
			{
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				dictionary2.Add("playerIDFrom", item.playerIDFrom);
				dictionary2.Add("message", item.message);
				dictionary2.Add("timeStamp", string.Empty);
				dictionary2.Add("isRead", item.isRead.ToString());
				dictionary2.Add("isSending", item.isSending.ToString());
				list.Add(dictionary2);
			}
			dictionary.Add(privateMessage.Key, list);
		}
		string text = Json.Serialize(dictionary);
		for (int i = 0; i < ChatController.privateMessages[PrivateChatController.sharedController.selectedPlayerID].Count; i++)
		{
			if (ChatController.privateMessages[PrivateChatController.sharedController.selectedPlayerID][i].timeStamp.ToString("F8", CultureInfo.InvariantCulture).Equals(timeStamp) && !ChatController.privateMessages[PrivateChatController.sharedController.selectedPlayerID][i].isRead)
			{
				isRead = true;
				ChatController.PrivateMessage value = ChatController.privateMessages[PrivateChatController.sharedController.selectedPlayerID][i];
				value.isRead = true;
				ChatController.countNewPrivateMessage--;
				ChatController.privateMessages[PrivateChatController.sharedController.selectedPlayerID][i] = value;
				PrivateChatController.sharedController.selectedPlayerItem.UpdateCountNewMessage();
				ChatController.SavePrivatMessageInPrefs();
				break;
			}
		}
		Dictionary<string, List<Dictionary<string, string>>> dictionary3 = new Dictionary<string, List<Dictionary<string, string>>>();
		foreach (KeyValuePair<string, List<ChatController.PrivateMessage>> privateMessage2 in ChatController.privateMessages)
		{
			List<Dictionary<string, string>> list2 = new List<Dictionary<string, string>>();
			foreach (ChatController.PrivateMessage item2 in privateMessage2.Value)
			{
				Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
				dictionary4.Add("playerIDFrom", item2.playerIDFrom);
				dictionary4.Add("message", item2.message);
				dictionary4.Add("timeStamp", string.Empty);
				dictionary4.Add("isRead", item2.isRead.ToString());
				dictionary4.Add("isSending", item2.isSending.ToString());
				list2.Add(dictionary4);
			}
			dictionary3.Add(privateMessage2.Key, list2);
		}
	}

	private void OnEnable()
	{
	}

	private void DetectNew()
	{
	}
}

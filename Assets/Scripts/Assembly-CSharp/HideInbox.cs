using System;
using UnityEngine;

[Obsolete]
public sealed class HideInbox : MonoBehaviour
{
	private void OnClick()
	{
		FriendsGUIController component = NGUITools.GetRoot(base.gameObject).GetComponent<FriendsGUIController>();
		if (component != null)
		{
			component.inboxPanel.gameObject.SetActive(false);
			component.friendsPanel.gameObject.SetActive(true);
		}
		ButtonClickSound.Instance.PlayClick();
	}
}

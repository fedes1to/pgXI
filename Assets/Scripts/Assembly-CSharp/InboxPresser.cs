using UnityEngine;

public class InboxPresser : MonoBehaviour
{
	private void OnClick()
	{
		ButtonClickSound.Instance.PlayClick();
		FriendsGUIController component = NGUITools.GetRoot(base.gameObject).GetComponent<FriendsGUIController>();
		if (component != null)
		{
			component.friendsPanel.gameObject.SetActive(false);
			component.inboxPanel.gameObject.SetActive(true);
			component.invitationsGrid.Reposition();
			component.sentInvitationsGrid.Reposition();
		}
	}
}

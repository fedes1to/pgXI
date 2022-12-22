using Rilisoft;
using UnityEngine;

public sealed class JoinClanPressed : MonoBehaviour
{
	private void OnClick()
	{
		ButtonClickSound.Instance.PlayClick();
		Invitation component = base.transform.parent.GetComponent<Invitation>();
		if (!(component == null))
		{
			FriendsController.sharedController.AcceptClanInvite(component.recordId);
			component.DisableButtons();
			component.KeepClanData();
		}
	}
}

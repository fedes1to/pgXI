using UnityEngine;

public class SendInvitationsButton : MonoBehaviour
{
	private void OnClick()
	{
		if (FacebookController.FacebookSupported)
		{
			FacebookController.sharedController.InvitePlayer(null);
		}
		ButtonClickSound.Instance.PlayClick();
	}
}

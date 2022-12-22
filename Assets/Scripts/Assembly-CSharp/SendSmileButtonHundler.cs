using UnityEngine;

public class SendSmileButtonHundler : MonoBehaviour
{
	private string smileName = string.Empty;

	private void Awake()
	{
		smileName = GetComponent<UISprite>().spriteName;
	}

	private void OnClick()
	{
		if (ButtonClickSound.Instance != null)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		if (PrivateChatController.sharedController != null)
		{
			PrivateChatController.sharedController.SendSmile(smileName);
		}
		if (ChatViewrController.sharedController != null && InGameGUI.sharedInGameGUI.playerMoveC != null)
		{
			InGameGUI.sharedInGameGUI.playerMoveC.SendChat(string.Empty, false, smileName);
			ChatViewrController.sharedController.HideSmilePannelOnClick();
		}
	}
}

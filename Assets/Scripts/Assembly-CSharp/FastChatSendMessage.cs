using UnityEngine;

public class FastChatSendMessage : MonoBehaviour
{
	public string message = "-=GO!=-";

	public UISprite iconSprite;

	private void Awake()
	{
	}

	private void OnClick()
	{
		if (InGameGUI.sharedInGameGUI.playerMoveC != null)
		{
			InGameGUI.sharedInGameGUI.playerMoveC.SendChat(message, false, string.Empty);
			if ((bool)ChatViewrController.sharedController)
			{
				ChatViewrController.sharedController.CloseChat(false);
			}
		}
	}
}

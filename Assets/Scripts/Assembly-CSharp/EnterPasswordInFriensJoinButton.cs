using UnityEngine;

public class EnterPasswordInFriensJoinButton : MonoBehaviour
{
	public UILabel passwordLabel;

	public JoinRoomFromFrends joinRoomFromFrends;

	private void Start()
	{
	}

	private void OnClick()
	{
		if (joinRoomFromFrends == null)
		{
			joinRoomFromFrends = JoinRoomFromFrends.sharedJoinRoomFromFrends;
		}
		if (joinRoomFromFrends != null)
		{
			joinRoomFromFrends.EnterPassword(passwordLabel.text);
		}
	}
}

using UnityEngine;

public sealed class JoinRoomFromFrendsButton : MonoBehaviour
{
	public JoinRoomFromFrends joinRoomFromFrends;

	private void OnClick()
	{
		ButtonClickSound.Instance.PlayClick();
		string id = base.transform.parent.GetComponent<FriendPreview>().id;
		if (FriendsController.sharedController.onlineInfo.ContainsKey(id))
		{
			int game_mode = int.Parse(FriendsController.sharedController.onlineInfo[id]["game_mode"]);
			string room_name = FriendsController.sharedController.onlineInfo[id]["room_name"];
			string text = FriendsController.sharedController.onlineInfo[id]["map"];
			if (joinRoomFromFrends == null)
			{
				joinRoomFromFrends = JoinRoomFromFrends.sharedJoinRoomFromFrends;
			}
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(text));
			if (infoScene != null)
			{
				joinRoomFromFrends.ConnectToRoom(game_mode, room_name, text);
			}
		}
	}
}

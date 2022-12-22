using Rilisoft;
using UnityEngine;

internal sealed class FriendProfileExample : MonoBehaviour
{
	public FriendProfileView friendProfileView;

	private void Start()
	{
		if (friendProfileView != null)
		{
			friendProfileView.Reset();
			friendProfileView.IsCanConnectToFriend = true;
			friendProfileView.FriendLocation = "Deathmatch/Bridge";
			friendProfileView.FriendCount = 42;
			friendProfileView.FriendName = "Дуэйн «Rock» Джонсон";
			friendProfileView.Online = OnlineState.playing;
			friendProfileView.Rank = 4;
			friendProfileView.SurvivalScore = 4376;
			friendProfileView.Username = "John Doe";
			friendProfileView.WinCount = 13;
			friendProfileView.SetBoots("boots_blue");
			friendProfileView.SetHat("hat_KingsCrown");
			friendProfileView.SetStockCape("cape_BloodyDemon");
		}
	}
}

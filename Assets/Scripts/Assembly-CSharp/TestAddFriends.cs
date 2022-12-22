using System.Collections.Generic;
using UnityEngine;

internal sealed class TestAddFriends : MonoBehaviour
{
	private void OnClick()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("Added Friends", "Test");
		dictionary.Add("Deleted Friends", "Add");
		Dictionary<string, object> socialEventParameters = dictionary;
		FriendsController.sharedController.SendInvitation("123", socialEventParameters);
	}
}

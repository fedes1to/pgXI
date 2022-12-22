using Rilisoft;
using UnityEngine;

public class FacebookFriendButton : MonoBehaviour
{
	private void Start()
	{
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnClick()
	{
		ButtonClickSound.Instance.PlayClick();
		if (FacebookController.FacebookSupported)
		{
			NGUITools.GetRoot(base.gameObject).GetComponent<FriendsGUIController>().friendsPanel.gameObject.SetActive(false);
			NGUITools.GetRoot(base.gameObject).GetComponent<FriendsGUIController>().facebookFriensPanel.gameObject.SetActive(true);
			FacebookController.sharedController.InputFacebookFriends(null, false);
		}
	}
}

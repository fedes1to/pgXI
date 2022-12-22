using System;
using Rilisoft;
using UnityEngine;

internal sealed class BackFacebookFriens : MonoBehaviour
{
	private IDisposable _backSubscription;

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(OnClick, "Facebook Friends");
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void OnClick()
	{
		NGUITools.GetRoot(base.gameObject).GetComponent<FriendsGUIController>().friendsPanel.gameObject.SetActive(true);
		NGUITools.GetRoot(base.gameObject).GetComponent<FriendsGUIController>().facebookFriensPanel.gameObject.SetActive(false);
		FriendsController.sharedController.facebookFriendsInfo.Clear();
		FacebookFriendsGUIController.sharedController._infoRequested = false;
		ButtonClickSound.Instance.PlayClick();
	}
}

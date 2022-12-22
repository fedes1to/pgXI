using System;
using Rilisoft;
using UnityEngine;

public sealed class BackRanksTapReceiver : MonoBehaviour
{
	public NetworkStartTableNGUIController networkStartTableNGUIController;

	private IDisposable _backSubscription;

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(OnClick, "Back Ranks");
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
		ButtonClickSound.TryPlayClick();
		networkStartTableNGUIController.BackPressFromRanksTable(true);
	}
}

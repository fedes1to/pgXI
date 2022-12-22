using System;
using Rilisoft;
using UnityEngine;

internal sealed class AvardPanelOkBtn : MonoBehaviour
{
	private IDisposable _backSubscription;

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(OnClick, "Award Panel");
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
		ButtonClickSound.Instance.PlayClick();
		if (NetworkStartTableNGUIController.sharedController != null)
		{
			NetworkStartTableNGUIController.sharedController.HideAvardPanel();
		}
	}
}

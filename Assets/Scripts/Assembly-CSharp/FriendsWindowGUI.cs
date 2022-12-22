using System;
using Rilisoft;
using UnityEngine;

public sealed class FriendsWindowGUI : MonoBehaviour
{
	public static FriendsWindowGUI Instance;

	public GameObject cameraObject;

	public Action OnExitCallback;

	public FriendsWindowController friendsWindow;

	private IDisposable _escapeSubscription;

	public bool InterfaceEnabled
	{
		get
		{
			return cameraObject.activeSelf;
		}
		private set
		{
			cameraObject.SetActive(value);
			if (_escapeSubscription != null)
			{
				_escapeSubscription.Dispose();
			}
			object escapeSubscription;
			if (value)
			{
				IDisposable disposable = BackSystem.Instance.Register(HandleEscape, "Friends");
				escapeSubscription = disposable;
			}
			else
			{
				escapeSubscription = null;
			}
			_escapeSubscription = (IDisposable)escapeSubscription;
		}
	}

	private void Awake()
	{
		Instance = this;
		InterfaceEnabled = false;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public void ShowInterface(Action _exitCallback = null)
	{
		InterfaceEnabled = true;
		OnExitCallback = _exitCallback;
		friendsWindow.SetStartState();
	}

	public void HideInterface()
	{
		if (OnExitCallback != null)
		{
			OnExitCallback();
		}
		ActivityIndicator.IsActiveIndicator = false;
		friendsWindow.SetCancelState();
		InterfaceEnabled = false;
	}

	private void HandleEscape()
	{
		HideInterface();
	}
}

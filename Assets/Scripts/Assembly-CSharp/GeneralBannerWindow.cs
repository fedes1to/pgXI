using System;
using Rilisoft;
using UnityEngine;

public class GeneralBannerWindow : MonoBehaviour
{
	private IDisposable _escapeSubscription;

	public Action OnCloseCustomAction { get; set; }

	public virtual void HandleClose()
	{
		DestroyScreen();
		Action onCloseCustomAction = OnCloseCustomAction;
		if (onCloseCustomAction != null)
		{
			onCloseCustomAction();
		}
	}

	private void DestroyScreen()
	{
		base.transform.parent = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected virtual void RegisterEscapeHandler()
	{
		_escapeSubscription = BackSystem.Instance.Register(delegate
		{
			OnHardwareBackPressed();
		}, string.Format("{0} Controller", base.gameObject.name));
	}

	protected virtual void UnregisterEscapeHandler()
	{
		_escapeSubscription.Dispose();
	}

	protected virtual void OnHardwareBackPressed()
	{
		HandleClose();
	}

	private void Start()
	{
		RegisterEscapeHandler();
	}

	private void OnDestroy()
	{
		UnregisterEscapeHandler();
	}
}

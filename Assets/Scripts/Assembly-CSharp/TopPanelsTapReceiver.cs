using System;
using UnityEngine;

public class TopPanelsTapReceiver : MonoBehaviour
{
	public static event Action OnClicked;

	static TopPanelsTapReceiver()
	{
		TopPanelsTapReceiver.OnClicked = delegate
		{
		};
	}

	private void Start()
	{
		base.gameObject.SetActive(Defs.isMulti);
	}

	private void OnClick()
	{
		ButtonClickSound.TryPlayClick();
		TopPanelsTapReceiver.OnClicked();
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public class CategoryButtonsController : MonoBehaviour
{
	public BtnCategory[] buttons;

	public float scaleMultypler = 1.1f;

	public float animTime = 0.7f;

	public UITable buttonsTable;

	public string currentBtnName;

	private List<Action<BtnCategory>> _actions;

	public List<Action<BtnCategory>> actions
	{
		get
		{
			if (_actions == null)
			{
				_actions = new List<Action<BtnCategory>>();
			}
			return _actions;
		}
	}

	private void Start()
	{
		buttonsTable.Reposition();
		BtnCategory[] array = buttons;
		foreach (BtnCategory btnCategory in array)
		{
			btnCategory.scaleMultypler = scaleMultypler;
			btnCategory.animTime = animTime;
		}
	}

	public void BtnClicked(string btnName, bool instant = false)
	{
		buttons.ForEach(delegate(BtnCategory b)
		{
			b.isDefault = b.btnName == btnName;
			b.isPressed = b.isDefault;
		});
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		for (int i = 0; i < buttons.Length; i++)
		{
			if (buttons[i].btnName == btnName)
			{
				buttons[i].isPressed = true;
				buttons[i].wasPressed = true;
				StartCoroutine(buttons[i].SetButtonPressed(true, instant));
				currentBtnName = btnName;
			}
			else
			{
				buttons[i].isPressed = false;
				StartCoroutine(buttons[i].SetButtonPressed(false));
			}
		}
		StartCoroutine(AnimateButtons());
	}

	private IEnumerator AnimateButtons()
	{
		float animationTimer = animTime;
		while (animationTimer > 0f)
		{
			animationTimer -= Time.unscaledDeltaTime;
			buttonsTable.Reposition();
			yield return null;
		}
		if (currentBtnName != null)
		{
			BtnCategory currentButton = buttons.FirstOrDefault((BtnCategory btn) => btn.btnName == currentBtnName);
			while (currentButton != null && currentButton.IsAnimationPlayed)
			{
				yield return null;
				buttonsTable.Reposition();
			}
		}
	}
}

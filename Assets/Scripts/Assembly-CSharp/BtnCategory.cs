using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BtnCategory : MonoBehaviour
{
	private bool _isEnable = true;

	public GameObject lockSprite;

	public UISprite normalState;

	public UISprite pressedState;

	public UITweener btnAnimation;

	public CategoryButtonsController btnController;

	public bool isPressed;

	public bool isDefault;

	private Color alphaColor;

	private Color normalColor;

	private Color pressedColor;

	[HideInInspector]
	public float scaleMultypler = 1.1f;

	[HideInInspector]
	public float animTime = 0.7f;

	public string btnName;

	[HideInInspector]
	public bool wasPressed;

	private bool isAnimationPlayed;

	[SerializeField]
	public List<BtnCategoryDependent> _dependents;

	public bool isEnable
	{
		get
		{
			return _isEnable;
		}
		set
		{
			_isEnable = value;
			if (lockSprite != null)
			{
				lockSprite.SetActive(!_isEnable);
			}
		}
	}

	public bool IsAnimationPlayed
	{
		get
		{
			return isAnimationPlayed;
		}
	}

	public event EventHandler Clicked;

	private void OnEnable()
	{
		ResetButton();
	}

	private void OnDisable()
	{
		ResetButton();
	}

	private void ResetButton()
	{
		isAnimationPlayed = false;
		if (isDefault)
		{
			btnController.currentBtnName = btnName;
			isPressed = true;
			wasPressed = true;
		}
		else
		{
			isPressed = false;
		}
		alphaColor = new Color(1f, 1f, 1f, 0.04f);
		normalColor = new Color(1f, 1f, 1f, 1f);
		pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
		if (isPressed)
		{
			base.transform.localScale = Vector3.one * scaleMultypler;
			normalState.color = alphaColor;
			pressedState.color = normalColor;
		}
		else
		{
			base.transform.localScale = Vector3.one;
			normalState.color = normalColor;
			pressedState.color = alphaColor;
		}
		btnController.buttonsTable.Reposition();
		SetDependentsState(isPressed);
	}

	public IEnumerator SetButtonPressed(bool isButtonPressed, bool instant = false)
	{
		while (isAnimationPlayed)
		{
			yield return null;
		}
		if (isButtonPressed != isPressed)
		{
			yield break;
		}
		SetDependentsState(isButtonPressed);
		if (isButtonPressed)
		{
			if (instant)
			{
				normalState.color = Color.Lerp(alphaColor, normalColor, 0f);
				pressedState.color = Color.Lerp(normalColor, alphaColor, 0f);
				base.transform.localScale = Vector3.Lerp(Vector3.one * scaleMultypler, Vector3.one, 0f);
				isAnimationPlayed = false;
				yield break;
			}
			isAnimationPlayed = true;
			float animationTimer2 = animTime;
			while (animationTimer2 > 0f && isPressed)
			{
				animationTimer2 -= Time.unscaledDeltaTime;
				float lerpFactor2 = animationTimer2 / animTime;
				normalState.color = Color.Lerp(alphaColor, normalColor, lerpFactor2);
				pressedState.color = Color.Lerp(normalColor, alphaColor, lerpFactor2);
				base.transform.localScale = Vector3.Lerp(Vector3.one * scaleMultypler, Vector3.one, lerpFactor2);
				yield return null;
			}
			isAnimationPlayed = false;
		}
		else if (wasPressed)
		{
			isAnimationPlayed = true;
			float animationTimer = 0f;
			while (animationTimer < animTime)
			{
				animationTimer += Time.unscaledDeltaTime;
				float lerpFactor = animationTimer / animTime;
				normalState.color = Color.Lerp(alphaColor, normalColor, lerpFactor);
				pressedState.color = Color.Lerp(normalColor, alphaColor, lerpFactor);
				base.transform.localScale = Vector3.Lerp(Vector3.one * scaleMultypler, Vector3.one, lerpFactor);
				yield return null;
			}
			wasPressed = false;
			isAnimationPlayed = false;
		}
		else
		{
			normalState.color = normalColor;
			pressedState.color = alphaColor;
			base.transform.localScale = Vector3.one;
			yield return null;
		}
	}

	private void OnClick()
	{
		if (!isEnable)
		{
			return;
		}
		EventHandler clicked = this.Clicked;
		if (clicked != null)
		{
			clicked(this, EventArgs.Empty);
		}
		if (isPressed || isAnimationPlayed)
		{
			return;
		}
		if (ButtonClickSound.Instance != null)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		btnController.BtnClicked(btnName);
		if (btnAnimation != null)
		{
			btnAnimation.ResetToBeginning();
			btnAnimation.PlayForward();
		}
		try
		{
			if (btnController.actions != null && btnController.buttons != null)
			{
				int num = btnController.buttons.ToList().IndexOf(this);
				if (num != -1 && btnController.actions.Count > num && btnController.actions[num] != null)
				{
					btnController.actions[num](this);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Exception in invoking action in BtnCategory: " + ex);
		}
	}

	private void OnPress(bool pressed)
	{
		if (isEnable && !isPressed)
		{
			if (pressed)
			{
				normalState.color = pressedColor;
			}
			else
			{
				normalState.color = normalColor;
			}
		}
	}

	private void SetDependentsState(bool buttonSelected)
	{
		foreach (BtnCategoryDependent dependent in _dependents)
		{
			if (dependent != null && !(dependent.Dependent == null))
			{
				bool flag = ((!dependent.InvertVisible) ? buttonSelected : (!buttonSelected));
				if (dependent.Dependent.activeSelf != flag)
				{
					dependent.Dependent.SetActive(flag);
				}
			}
		}
	}
}

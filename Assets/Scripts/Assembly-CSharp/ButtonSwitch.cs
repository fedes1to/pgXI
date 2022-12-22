using System;
using UnityEngine;

public class ButtonSwitch : MonoBehaviour
{
	[SerializeField]
	[Header("GO to show when enabled")]
	private GameObject enabledGo;

	[SerializeField]
	[Header("GO to show when disabled")]
	private GameObject disbledGo;

	[Header("Does button should play click sound?")]
	[SerializeField]
	private bool noSound;

	[Header("Должна ли кнопка нажиматься сама или активироваться из кода")]
	[SerializeField]
	private bool isAutomatic = true;

	[SerializeField]
	private UITweener tweenOn;

	[SerializeField]
	private UITweener tweenOff;

	private Collider collider
	{
		get
		{
			return base.gameObject.GetComponent<Collider>();
		}
	}

	public bool HasClickedHandlers
	{
		get
		{
			return this.Clicked != null;
		}
	}

	private bool isEnable
	{
		get
		{
			if (collider == null)
			{
				return false;
			}
			return collider.enabled;
		}
		set
		{
			if (collider != null)
			{
				collider.enabled = value;
			}
		}
	}

	public event EventHandler Clicked;

	private void OnClick()
	{
		if (isAutomatic && isEnable)
		{
			if (ButtonClickSound.Instance != null && !noSound)
			{
				ButtonClickSound.Instance.PlayClick();
			}
			Switch(false);
			EventHandler clicked = this.Clicked;
			if (clicked != null)
			{
				clicked(this, EventArgs.Empty);
			}
		}
	}

	public void DoClick()
	{
		if (isEnable)
		{
			OnClick();
		}
	}

	public void Switch(bool isEnabled)
	{
		enabledGo.SetActive(isEnabled);
		disbledGo.SetActive(!isEnabled);
		isEnable = isEnabled;
		if (isEnabled)
		{
			if ((bool)tweenOn)
			{
				tweenOn.ResetToBeginning();
				tweenOn.PlayForward();
			}
		}
		else if ((bool)tweenOff)
		{
			tweenOff.ResetToBeginning();
			tweenOff.PlayForward();
		}
	}
}

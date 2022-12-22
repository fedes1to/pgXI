using System;
using Rilisoft;
using UnityEngine;

public sealed class SettingsToggleButtons : MonoBehaviour
{
	public UIButton offButton;

	public UIButton onButton;

	private bool _isChecked;

	private UIToggle _toggleVal;

	public bool IsChecked
	{
		get
		{
			if (_toggle != null)
			{
				return _toggle.value;
			}
			return _isChecked;
		}
		set
		{
			if (_toggle != null)
			{
				_toggle.value = value;
				return;
			}
			_isChecked = value;
			if (offButton == null || onButton == null)
			{
				Debug.LogError(string.Format("toggle not setted, GO: '{0}'", base.gameObject.name));
				return;
			}
			offButton.isEnabled = _isChecked;
			onButton.isEnabled = !_isChecked;
			EventHandler<ToggleButtonEventArgs> clicked = this.Clicked;
			if (clicked != null)
			{
				clicked(this, new ToggleButtonEventArgs
				{
					IsChecked = _isChecked
				});
			}
		}
	}

	private UIToggle _toggle
	{
		get
		{
			if (_toggleVal == null)
			{
				_toggleVal = base.gameObject.GetComponentInChildren<UIToggle>(true);
				if (_toggleVal != null)
				{
					_toggleVal.onChange.Add(new EventDelegate(OnValueChanged));
				}
			}
			return _toggleVal;
		}
	}

	public event EventHandler<ToggleButtonEventArgs> Clicked;

	private void OnValueChanged()
	{
		EventHandler<ToggleButtonEventArgs> clicked = this.Clicked;
		if (clicked != null)
		{
			clicked(this, new ToggleButtonEventArgs
			{
				IsChecked = _toggle.value
			});
		}
	}

	private void Start()
	{
		if (_toggle == null)
		{
			onButton.GetComponent<ButtonHandler>().Clicked += delegate
			{
				IsChecked = true;
			};
			offButton.GetComponent<ButtonHandler>().Clicked += delegate
			{
				IsChecked = false;
			};
		}
	}
}

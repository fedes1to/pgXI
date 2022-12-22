using UnityEngine;

[RequireComponent(typeof(UISprite))]
[RequireComponent(typeof(UIToggle))]
public class RilisoftAlphaToggle : MonoBehaviour
{
	[Range(0f, 1f)]
	public float alphaOnState;

	[Range(0f, 1f)]
	public float alphaOffState;

	private UIToggle _toggle;

	private UISprite _toggledSprite;

	private void Start()
	{
		_toggle = GetComponent<UIToggle>();
		_toggledSprite = _toggle.GetComponent<UISprite>();
		if (_toggle != null && _toggledSprite != null)
		{
			OnAlphaChange();
			EventDelegate.Add(_toggle.onChange, OnAlphaChange);
		}
	}

	public void OnAlphaChange()
	{
		if (_toggle.value)
		{
			_toggledSprite.alpha = alphaOnState;
		}
		else
		{
			_toggledSprite.alpha = alphaOffState;
		}
	}

	private void OnDestroy()
	{
		if (_toggle != null)
		{
			EventDelegate.Remove(_toggle.onChange, OnAlphaChange);
		}
	}
}

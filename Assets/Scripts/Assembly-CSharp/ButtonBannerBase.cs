using UnityEngine;

public class ButtonBannerBase : MonoBehaviour
{
	[HideInInspector]
	public int indexBut;

	public int priorityShow;

	public virtual void OnShow()
	{
	}

	public virtual void OnHide()
	{
	}

	public virtual bool BannerIsActive()
	{
		return false;
	}

	public virtual void OnClickButton()
	{
	}

	public virtual void OnChangeLocalize()
	{
	}

	public virtual void OnUpdateParameter()
	{
	}

	private void OnClick()
	{
		OnClickButton();
	}

	private void OnPress(bool IsDown)
	{
		if (IsDown)
		{
			ButtonBannerHUD.instance.StopTimerNextBanner();
		}
		else
		{
			ButtonBannerHUD.instance.ResetTimerNextBanner();
		}
	}
}

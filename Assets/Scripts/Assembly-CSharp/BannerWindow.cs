using UnityEngine;

public class BannerWindow : MonoBehaviour
{
	public UITexture Background;

	public UIButton ExitButton;

	private bool _isShow;

	public BannerWindowType type { get; set; }

	public bool IsShow
	{
		get
		{
			return _isShow;
		}
		set
		{
			_isShow = value;
		}
	}

	public void SetBackgroundImage(Texture2D image)
	{
		if (!(Background == null))
		{
			Background.mainTexture = image;
		}
	}

	public void SetEnableExitButton(bool enable)
	{
		if (!(ExitButton == null))
		{
			ExitButton.gameObject.SetActive(enable);
		}
	}

	protected virtual void SetActiveAndShow()
	{
		base.gameObject.SetActive(true);
		IsShow = true;
	}

	public virtual void Show()
	{
		SetActiveAndShow();
		AdmobPerelivWindow component = GetComponent<AdmobPerelivWindow>();
		if (component != null)
		{
			component.Show();
		}
	}

	public void Hide()
	{
		AdmobPerelivWindow component = GetComponent<AdmobPerelivWindow>();
		if (component != null)
		{
			component.Hide();
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		IsShow = false;
	}

	internal virtual void Submit()
	{
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public sealed class FeedbackMenuController : MonoBehaviour
{
	public enum State
	{
		FAQ,
		TermsFuse
	}

	private State _currentState;

	public UIButton faqButton;

	public UIButton termsFuseButton;

	public UIButton sendFeedbackButton;

	public UIButton backButton;

	public GameObject textFAQScroll;

	public GameObject textTermsOfUse;

	[SerializeField]
	private UILabel _versionLabel;

	public static FeedbackMenuController Instance { get; private set; }

	public State CurrentState
	{
		get
		{
			return _currentState;
		}
		set
		{
			if (faqButton != null)
			{
				faqButton.isEnabled = value != State.FAQ;
				Transform transform = faqButton.gameObject.transform.FindChild("SpriteLabel");
				if (transform != null)
				{
					transform.gameObject.SetActive(value != State.FAQ);
				}
				Transform transform2 = faqButton.gameObject.transform.FindChild("ChekmarkLabel");
				if (transform2 != null)
				{
					transform2.gameObject.SetActive(value == State.FAQ);
				}
				textFAQScroll.SetActive(value == State.FAQ);
			}
			if (termsFuseButton != null)
			{
				termsFuseButton.isEnabled = value != State.TermsFuse;
				Transform transform3 = termsFuseButton.gameObject.transform.FindChild("SpriteLabel");
				if (transform3 != null)
				{
					transform3.gameObject.SetActive(value != State.TermsFuse);
				}
				else
				{
					Debug.Log("_spriteLabel=null");
				}
				Transform transform4 = termsFuseButton.gameObject.transform.FindChild("ChekmarkLabel");
				if (transform4 != null)
				{
					transform4.gameObject.SetActive(value == State.TermsFuse);
				}
				else
				{
					Debug.Log("_spriteLabel=null");
				}
				textTermsOfUse.SetActive(value == State.TermsFuse);
			}
			_currentState = value;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		CurrentState = State.FAQ;
		IEnumerable<UIButton> enumerable = new UIButton[2] { faqButton, termsFuseButton }.Where((UIButton b) => b != null);
		foreach (UIButton item in enumerable)
		{
			ButtonHandler component = item.GetComponent<ButtonHandler>();
			if (component != null)
			{
				component.Clicked += HandleTabPressed;
			}
		}
		if (sendFeedbackButton != null)
		{
			ButtonHandler component2 = sendFeedbackButton.GetComponent<ButtonHandler>();
			if (component2 != null)
			{
				component2.Clicked += HandleSendFeedback;
			}
		}
		if (backButton != null)
		{
			ButtonHandler component3 = backButton.GetComponent<ButtonHandler>();
			if (component3 != null)
			{
				component3.Clicked += HandleBackButton;
			}
		}
		string text = typeof(SettingsController).Assembly.GetName().Version.ToString();
		if (_versionLabel != null)
		{
			_versionLabel.text = text;
		}
	}

	private void BackButton()
	{
		base.gameObject.SetActive(false);
		if (MainMenuController.sharedController != null && MainMenuController.sharedController.settingsPanel != null)
		{
			MainMenuController.sharedController.settingsPanel.SetActive(true);
		}
	}

	private void HandleBackButton(object sender, EventArgs e)
	{
		BackButton();
	}

	public static void ShowDialogWithCompletion(Action handler)
	{
		if (handler != null)
		{
			handler();
		}
	}

	private void HandleSendFeedback(object sender, EventArgs e)
	{
		Action handler = delegate
		{
			string text = typeof(FeedbackMenuController).Assembly.GetName().Version.ToString();
			string text2 = string.Concat("mailto:pixelgun3D.supp0rt@gmail.com?subject=Feedback&body=%0D%0A%0D%0A%0D%0A%0D%0A%0D%0A------------%20DO NOT DELETE%20------------%0D%0AUTC%20Time:%20", DateTime.Now.ToString(), "%0D%0AGame:%20PixelGun3D%0D%0AVersion:%20", text, "%0D%0APlayerID:%20", FriendsController.sharedController.id, "%0D%0ACategory:%20Feedback%0D%0ADevice%20Type:%20", SystemInfo.deviceType, "%20", SystemInfo.deviceModel, "%0D%0AOS%20Version:%20", SystemInfo.operatingSystem, "%0D%0A------------------------");
			text2 = text2.Replace(" ", "%20");
			Debug.Log(text2);
			Application.OpenURL(text2);
		};
		ShowDialogWithCompletion(handler);
	}

	private void HandleTabPressed(object sender, EventArgs e)
	{
		GameObject gameObject = ((ButtonHandler)sender).gameObject;
		if (faqButton != null && gameObject == faqButton.gameObject)
		{
			CurrentState = State.FAQ;
		}
		else if (termsFuseButton != null && gameObject == termsFuseButton.gameObject)
		{
			CurrentState = State.TermsFuse;
		}
	}
}

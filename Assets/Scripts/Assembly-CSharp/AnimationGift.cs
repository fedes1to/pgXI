using System;
using System.Collections;
using Rilisoft;
using UnityEngine;

public class AnimationGift : MonoBehaviour
{
	public static AnimationGift instance;

	public GameObject objGift;

	public AudioClip soundOpen;

	public AudioClip soundClose;

	public AudioClip soundGetGift;

	private Animator _animator;

	private float timeoutShowGift = 2f;

	public static event Action onEndAnimOpen;

	private void Awake()
	{
		instance = this;
		_animator = objGift.GetComponent<Animator>();
		SetVisibleObjGift(false);
	}

	private void OnEnable()
	{
		GiftBannerWindow.onGetGift += OpenGift;
		GiftBannerWindow.onHideInfoGift += CloseGift;
		GiftBannerWindow.onHideInfoGift += CheckStateGift;
		GiftBannerWindow.onOpenInfoGift += OnOpenInfoGift;
		GiftController.OnTimerEnded += CheckStateGift;
		GiftController.OnChangeSlots += CheckVisibleGift;
		MainMenuController.onLoadMenu += OnLoadMenu;
		TrainingController.onChangeTraining += CheckVisibleGift;
		FriendsController.ServerTimeUpdated += CheckVisibleGift;
		MainMenuHeroCamera.onEndOpenGift += EventOpenEndCam;
		MainMenuHeroCamera.onEndCloseGift += EventCloseEndCam;
		MainMenuController.onActiveMainMenu += ChangeActiveMainMenu;
	}

	private void OnDisable()
	{
		GiftBannerWindow.onHideInfoGift -= CloseGift;
		GiftBannerWindow.onGetGift -= OpenGift;
		GiftBannerWindow.onHideInfoGift -= CheckStateGift;
		GiftBannerWindow.onOpenInfoGift -= OnOpenInfoGift;
		GiftController.OnTimerEnded -= CheckStateGift;
		FriendsController.ServerTimeUpdated -= CheckVisibleGift;
		GiftController.OnChangeSlots -= CheckVisibleGift;
		MainMenuController.onLoadMenu -= OnLoadMenu;
		TrainingController.onChangeTraining -= CheckVisibleGift;
		MainMenuHeroCamera.onEndOpenGift -= EventOpenEndCam;
		MainMenuHeroCamera.onEndCloseGift -= EventCloseEndCam;
		MainMenuController.onActiveMainMenu -= ChangeActiveMainMenu;
	}

	private void OnDetstroy()
	{
		instance = null;
	}

	private void OnLoadMenu()
	{
		CheckVisibleGift();
	}

	public void OpenGift()
	{
		StartCoroutine(WaitOpenGift());
		if (Defs.isSoundFX && (bool)GetComponent<AudioSource>() && (bool)soundOpen)
		{
			GetComponent<AudioSource>().PlayOneShot(soundOpen);
		}
		TutorialQuestManager.Instance.AddFulfilledQuest("getGotcha");
		QuestMediator.NotifyGetGotcha();
	}

	private IEnumerator WaitOpenGift()
	{
		yield return new WaitForSeconds(timeoutShowGift);
		if (AnimationGift.onEndAnimOpen != null)
		{
			AnimationGift.onEndAnimOpen();
		}
	}

	public void OnOpenInfoGift()
	{
		if (_animator != null)
		{
			_animator.SetBool("IsOpen", true);
		}
		if (Defs.isSoundFX && (bool)GetComponent<AudioSource>() && (bool)soundGetGift)
		{
			GetComponent<AudioSource>().PlayOneShot(soundGetGift);
		}
	}

	public void CloseGift()
	{
		if (_animator != null)
		{
			_animator.SetBool("IsOpen", false);
		}
		if (Defs.isSoundFX && (bool)GetComponent<AudioSource>() && (bool)soundClose)
		{
			GetComponent<AudioSource>().PlayOneShot(soundClose);
		}
	}

	public void CheckStateGift()
	{
		bool flag = false;
		if (ButOpenGift.instance != null)
		{
			ButOpenGift.instance.IsCanGetGift = GiftController.Instance.CanGetTimerGift || flag;
		}
		if (_animator != null)
		{
			_animator.SetBool("IsEnabled", GiftController.Instance.CanGetTimerGift || flag);
		}
	}

	public void CheckVisibleGift()
	{
		if (TrainingController.TrainingCompleted && GiftController.Instance.ActiveGift && !MainMenuController.sharedController.FreePanelIsActive && !MainMenuController.sharedController.settingsPanel.activeSelf)
		{
			SetVisibleObjGift(true);
			CheckStateGift();
		}
		else
		{
			SetVisibleObjGift(false);
		}
	}

	public void ResetAnimation()
	{
		if (_animator != null)
		{
			_animator.SetBool("IsOpen", false);
		}
		StopCoroutine("WaitOpenGift");
		if (ButOpenGift.instance != null)
		{
			ButOpenGift.instance.CanShowLabel = false;
		}
	}

	public void StartAnimForGetGift()
	{
		if (_animator != null)
		{
			_animator.SetBool("IsEnabled", true);
		}
	}

	public void StopAnimForGetGift()
	{
		CheckStateGift();
	}

	private void SetVisibleObjGift(bool val)
	{
		if (objGift.activeSelf == val || (GiftBannerWindow.instance != null && GiftBannerWindow.instance.IsShow && GiftBannerWindow.instance.curStateAnimAward != 0))
		{
			return;
		}
		if (!TrainingController.TrainingCompleted || GiftController.Instance == null || !GiftController.Instance.ActiveGift)
		{
			val = false;
		}
		if (ButOpenGift.instance != null)
		{
			ButOpenGift.instance.ActiveHighLight = val;
		}
		objGift.SetActive(val);
		if (val)
		{
			Invoke("WaitEndAnimShow", 1f);
			return;
		}
		if (ButOpenGift.instance != null)
		{
			ButOpenGift.instance.CanShowLabel = false;
		}
		if (GiftBannerWindow.instance != null && GiftBannerWindow.instance.bannerObj.activeInHierarchy)
		{
			GiftBannerWindow.instance.ForceCloseAll();
		}
	}

	private void WaitEndAnimShow()
	{
		CancelInvoke("WaitEndAnimShow");
		if (ButOpenGift.instance != null)
		{
			ButOpenGift.instance.CanShowLabel = true;
		}
	}

	private void OnApplicationPause(bool pausing)
	{
		if (!pausing)
		{
			Invoke("CheckVisibleGift", 0.1f);
		}
	}

	private void ChangeLayer(string nameLayer)
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer(nameLayer);
		}
	}

	private void EventOpenEndCam()
	{
		if (ButOpenGift.instance != null)
		{
			ButOpenGift.instance.OpenGift();
		}
	}

	private void EventCloseEndCam()
	{
		if (ButOpenGift.instance != null)
		{
			ButOpenGift.instance.CloseGift();
		}
	}

	private void ChangeActiveMainMenu(bool val)
	{
		if (ButOpenGift.instance != null)
		{
			if (val)
			{
				ButOpenGift.instance.UpdateHUDStateGift();
			}
			else
			{
				ButOpenGift.instance.HideLabelTap();
			}
		}
	}
}

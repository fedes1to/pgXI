using System;
using System.Collections;
using UnityEngine;

public class MainMenuHeroCamera : MonoBehaviour
{
	public Animator moveMenuAnimator;

	private string animGotcha = "MainMenuOpenGotcha";

	private string animNormal = "MainMenuCloseOptions";

	private float YawForMainMenu
	{
		get
		{
			return (!MenuLeaderboardsView.IsNeedShow) ? 6f : 0f;
		}
	}

	public bool IsAnimPlaying { get; private set; }

	public static event Action onEndOpenGift;

	public static event Action onEndCloseGift;

	public void Start()
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		base.transform.rotation = Quaternion.Euler(new Vector3(eulerAngles.x, YawForMainMenu, eulerAngles.z));
	}

	private void OnEnable()
	{
		ButOpenGift.onOpen += OnShowGift;
		GiftBannerWindow.onClose += OnCloseGift;
	}

	private void OnDisable()
	{
		ButOpenGift.onOpen -= OnShowGift;
		GiftBannerWindow.onClose -= OnCloseGift;
	}

	public void OnCloseGift()
	{
		GetComponent<Animation>()[animGotcha].speed = -1f;
		GetComponent<Animation>()[animGotcha].time = GetComponent<Animation>()[animGotcha].length;
		GetComponent<Animation>().Play(animGotcha);
		StartCoroutine(WaitAnimEnd(animGotcha));
	}

	public void OnShowGift()
	{
		GetComponent<Animation>()[animGotcha].speed = 1f;
		GetComponent<Animation>()[animGotcha].time = 0f;
		GetComponent<Animation>().Play(animGotcha);
		StartCoroutine(WaitAnimEnd(animGotcha));
	}

	private void EndAnimation(string nameAnim)
	{
		switch (nameAnim)
		{
		case "MainMenuOpenGotcha":
			if (GetComponent<Animation>()[animGotcha].speed > 0f)
			{
				if (MainMenuHeroCamera.onEndOpenGift != null)
				{
					MainMenuHeroCamera.onEndOpenGift();
				}
			}
			else if (MainMenuHeroCamera.onEndCloseGift != null)
			{
				MainMenuHeroCamera.onEndCloseGift();
			}
			break;
		}
	}

	private IEnumerator WaitAnimEnd(string nameAnim)
	{
		while (GetComponent<Animation>().IsPlaying(nameAnim))
		{
			yield return null;
		}
		EndAnimation(nameAnim);
	}

	public void OnMainMenuOpenSocial()
	{
		PlayAnim(14.5f);
		if (MenuLeaderboardsController.sharedController != null && MenuLeaderboardsController.sharedController.menuLeaderboardsView != null)
		{
			MenuLeaderboardsController.sharedController.menuLeaderboardsView.Show(false, false);
		}
	}

	public void OnMainMenuOpenOptions()
	{
		PlayAnim(23.5f);
		if (MenuLeaderboardsController.sharedController != null && MenuLeaderboardsController.sharedController.menuLeaderboardsView != null)
		{
			MenuLeaderboardsController.sharedController.menuLeaderboardsView.Show(false, false);
		}
	}

	public void OnMainMenuCloseOptions()
	{
		PlayAnim(YawForMainMenu);
		if (MenuLeaderboardsController.sharedController != null && MenuLeaderboardsController.sharedController.menuLeaderboardsView != null && MenuLeaderboardsView.IsNeedShow)
		{
			MenuLeaderboardsController.sharedController.menuLeaderboardsView.Show(true, true);
		}
	}

	public void OnMainMenuOpenLeaderboards()
	{
		PlayAnim(0f);
	}

	public void OnMainMenuCloseLeaderboards()
	{
		PlayAnim(6f);
	}

	private void PlayAnim(float endYaw)
	{
		StopAllCoroutines();
		StartCoroutine(PlayAnimCoroutine(endYaw));
	}

	private IEnumerator PlayAnimCoroutine(float endYaw)
	{
		IsAnimPlaying = true;
		Transform heroCameraTransform = base.transform;
		Quaternion heroCameraRotation = heroCameraTransform.rotation;
		float startTime = Time.realtimeSinceStartup;
		Quaternion startRotation = heroCameraRotation;
		Quaternion endRotation = Quaternion.Euler(heroCameraRotation.eulerAngles.x, endYaw, heroCameraRotation.eulerAngles.z);
		while (Time.realtimeSinceStartup - startTime <= 1f)
		{
			float deltaFromStart = Time.realtimeSinceStartup - startTime;
			if (deltaFromStart <= 0.1f)
			{
				yield return null;
			}
			heroCameraTransform.rotation = Quaternion.Lerp(startRotation, endRotation, (deltaFromStart - 0.1f) / 0.9f);
			yield return null;
		}
		heroCameraTransform.rotation = endRotation;
		IsAnimPlaying = false;
	}

	public void OnOpenSingleModePanel()
	{
		StopAllCoroutines();
		moveMenuAnimator.enabled = false;
	}

	public void OnCloseSingleModePanel()
	{
		moveMenuAnimator.enabled = true;
		PlayAnim(YawForMainMenu);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using Rilisoft;
using UnityEngine;

internal sealed class RewardedLikeButton : MonoBehaviour
{
	private const int RewardGemsCount = 10;

	internal const string RewardKey = "RewardForLikeGained";

	public UIButton rewardedLikeButton;

	public UILabel rewardedLikeCaption;

	private IEnumerator Start()
	{
		LocalizationStore.AddEventCallAfterLocalize(RefreshRewardedLikeButton);
		RefreshRewardedLikeButton();
		while (MainMenuController.sharedController == null)
		{
			yield return null;
		}
		Refresh();
		if (!FB.IsLoggedIn)
		{
			while (!FB.IsLoggedIn)
			{
				yield return new WaitForSeconds(1f);
			}
			Refresh();
		}
	}

	private void OnEnable()
	{
		Refresh();
	}

	private void OnDestroy()
	{
		LocalizationStore.DelEventCallAfterLocalize(RefreshRewardedLikeButton);
	}

	public void OnClick()
	{
		Application.OpenURL("https://www.facebook.com/PixelGun3DOfficial");
		try
		{
			TutorialQuestManager.Instance.AddFulfilledQuest("likeFacebook");
			QuestMediator.NotifySocialInteraction("likeFacebook");
			if (Storager.getInt("RewardForLikeGained", true) <= 0)
			{
				Storager.setInt("RewardForLikeGained", 1, true);
				int @int = Storager.getInt("GemsCurrency", false);
				Storager.setInt("GemsCurrency", @int + 10, false);
				AnalyticsFacade.CurrencyAccrual(10, "GemsCurrency");
				AnalyticsFacade.SendCustomEvent("Virality", new Dictionary<string, object> { { "Like Facebook Page", "Likes" } });
				CoinsMessage.FireCoinsAddedEvent(true);
			}
		}
		finally
		{
			Refresh();
		}
	}

	private void RefreshRewardedLikeButton()
	{
		if (rewardedLikeCaption == null)
		{
			Debug.LogError("rewardedLikeCaption == null");
			return;
		}
		try
		{
			string format = LocalizationStore.Get("Key_1653");
			rewardedLikeCaption.text = string.Format(format, 10);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	internal void Refresh()
	{
		if (!FacebookController.FacebookSupported)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if (rewardedLikeButton == null)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if (Storager.hasKey("RewardForLikeGained") && Storager.getInt("RewardForLikeGained", true) > 0)
		{
			UnityEngine.Object.Destroy(rewardedLikeButton.gameObject);
			UnityEngine.Object.Destroy(this);
		}
		else if (!FB.IsLoggedIn)
		{
			rewardedLikeButton.gameObject.SetActive(false);
		}
		else if (!Storager.hasKey(Defs.IsFacebookLoginRewardaGained) || Storager.getInt(Defs.IsFacebookLoginRewardaGained, true) == 0)
		{
			rewardedLikeButton.gameObject.SetActive(false);
		}
		else if (ExpController.LobbyLevel <= 1)
		{
			rewardedLikeButton.gameObject.SetActive(false);
		}
		else if (MainMenuController.SavedShwonLobbyLevelIsLessThanActual())
		{
			rewardedLikeButton.gameObject.SetActive(false);
		}
		else
		{
			RefreshRewardedLikeButton();
			rewardedLikeButton.gameObject.SetActive(true);
		}
	}
}

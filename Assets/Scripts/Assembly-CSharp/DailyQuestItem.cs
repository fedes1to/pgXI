using System.Collections.Generic;
using System.Globalization;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DailyQuestItem : MonoBehaviour
{
	public UISprite progressBar;

	public GameObject coinsObject;

	public GameObject gemObject;

	public GameObject expObject;

	public UILabel coinsCount;

	public UILabel gemsCount;

	public UILabel expCount;

	public UILabel progressLabel;

	public UILabel questDescription;

	public UITable awardTable;

	public GameObject getRewardButton;

	public GameObject toBattleButton;

	public GameObject completedObject;

	public GameObject questInProgress;

	public UILabel viewAllLabel;

	public bool itemInLobby;

	public UIButton skipButton;

	public UILabel questsButtonLabel;

	public UILabel rewardButtonLabel;

	[Header("Animations for quest frame")]
	public GameObject questSkipFrame;

	public GameObject oldQuest;

	public TweenPosition skipAnimPosition;

	public TweenScale rewardAnim;

	public UISprite modeColor;

	public UISprite modeIcon;

	private QuestInfo _questInfo;

	private AccumulativeQuestBase Quest
	{
		get
		{
			return _questInfo.Map((QuestInfo qi) => qi.Quest as AccumulativeQuestBase);
		}
	}

	public bool CanSkip
	{
		get
		{
			if (_questInfo == null)
			{
				return false;
			}
			return _questInfo.CanSkip;
		}
	}

	public void Refresh()
	{
		if (itemInLobby)
		{
			FillData(-1);
		}
		else if (_questInfo != null)
		{
			QuestBase quest = _questInfo.Quest;
			if (quest != null)
			{
				FillData(Quest.Slot);
			}
		}
	}

	private void OnEnable()
	{
		if (questSkipFrame != null)
		{
			questSkipFrame.SetActive(false);
		}
		if (rewardAnim != null)
		{
			rewardAnim.enabled = false;
		}
		if (skipAnimPosition != null)
		{
			skipAnimPosition.enabled = false;
			base.transform.localScale = Vector3.one;
			base.transform.localPosition = skipAnimPosition.from;
		}
		if (itemInLobby)
		{
			FillData(-1);
		}
	}

	private int GetQuestMode(AccumulativeQuestBase quest)
	{
		ModeAccumulativeQuest modeAccumulativeQuest = quest as ModeAccumulativeQuest;
		if (modeAccumulativeQuest != null)
		{
			return (int)modeAccumulativeQuest.Mode;
		}
		return 0;
	}

	private string GetQuestMap(AccumulativeQuestBase quest)
	{
		MapAccumulativeQuest mapAccumulativeQuest = quest as MapAccumulativeQuest;
		if (mapAccumulativeQuest != null)
		{
			return mapAccumulativeQuest.Map;
		}
		return string.Empty;
	}

	public void FillData(int slot)
	{
		if (!TrainingController.TrainingCompleted)
		{
			base.gameObject.SetActive(false);
			return;
		}
		QuestProgress questProgress = QuestSystem.Instance.QuestProgress;
		if (questProgress == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		_questInfo = ((slot != -1) ? questProgress.GetActiveQuestInfoBySlot(slot + 1) : questProgress.GetRandomQuestInfo());
		if (Quest == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (Quest.SetActive())
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Total", "Get");
			Dictionary<string, object> eventParams = dictionary;
			AnalyticsFacade.SendCustomEvent("Daily Quests", eventParams);
			string value = string.Format(CultureInfo.InvariantCulture, "{0} / {1}", Quest.Id, QuestConstants.GetDifficultyKey(Quest.Difficulty));
			dictionary = new Dictionary<string, object>();
			dictionary.Add("Get", value);
			Dictionary<string, object> eventParams2 = dictionary;
			AnalyticsFacade.SendCustomEvent("Daily Quests", eventParams2);
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		questDescription.text = QuestConstants.GetAccumulativeQuestDescriptionByType(Quest);
		progressLabel.text = string.Format("{0}/{1}", Quest.CurrentCount, Quest.RequiredCount);
		progressBar.fillAmount = (float)Quest.CalculateProgress();
		if (Defs.IsDeveloperBuild)
		{
			UISprite component = GetComponent<UISprite>();
			if (component != null)
			{
				oldQuest.SetActive(Quest.Day < questProgress.Day);
			}
		}
		if (itemInLobby)
		{
			if (_questInfo.Quest != null)
			{
				bool flag = _questInfo.Quest.CalculateProgress() >= 1m && !_questInfo.Quest.Rewarded;
				if (questsButtonLabel != null && rewardButtonLabel != null)
				{
					questsButtonLabel.gameObject.SetActive(!flag);
					rewardButtonLabel.gameObject.SetActive(flag);
					if (flag)
					{
						DailyQuestsButton component2 = questsButtonLabel.parent.GetComponent<DailyQuestsButton>();
						if (component2 != null)
						{
							component2.SetUI();
						}
					}
				}
				if (modeColor != null)
				{
					modeColor.color = QuestImage.Instance.GetColor(_questInfo.Quest);
				}
				if (modeIcon != null)
				{
					modeIcon.spriteName = QuestImage.Instance.GetSpriteName(_questInfo.Quest);
				}
			}
		}
		else
		{
			if (Quest.CalculateProgress() >= 1m)
			{
				getRewardButton.SetActive(!Quest.Rewarded);
				completedObject.SetActive(Quest.Rewarded);
				toBattleButton.SetActive(false);
				questInProgress.SetActive(false);
				questSkipFrame.SetActive(false);
			}
			else
			{
				getRewardButton.SetActive(false);
				completedObject.SetActive(false);
				toBattleButton.SetActive(false);
				questInProgress.SetActive(true);
				questSkipFrame.SetActive(false);
			}
			if (SceneManager.GetActiveScene().name != Defs.MainMenuScene)
			{
				toBattleButton.SetActive(false);
			}
			if (modeColor != null)
			{
				modeColor.color = QuestImage.Instance.GetColor(_questInfo.Quest);
			}
			if (modeIcon != null)
			{
				modeIcon.spriteName = QuestImage.Instance.GetSpriteName(_questInfo.Quest);
			}
		}
		coinsCount.text = Quest.Reward.Coins.ToString();
		gemsCount.text = Quest.Reward.Gems.ToString();
		expCount.text = Quest.Reward.Experience.ToString();
		coinsObject.SetActive(Quest.Reward.Coins > 0);
		gemObject.SetActive(Quest.Reward.Gems > 0);
		expObject.SetActive(Quest.Reward.Experience > 0 && (ExperienceController.sharedController.currentLevel != 31 || (Quest.Reward.Coins == 0 && Quest.Reward.Gems == 0)));
		awardTable.repositionNow = true;
		if (skipButton != null)
		{
			skipButton.gameObject.SetActive(_questInfo.CanSkip);
		}
	}

	public void OnGetRewardButtonClick()
	{
		if (QuestSystem.Instance.QuestProgress == null)
		{
			return;
		}
		QuestSystem.Instance.QuestProgress.FilterFulfilledTutorialQuests();
		if (Quest.CalculateProgress() >= 1m && !Quest.Rewarded)
		{
			Quest.SetRewarded();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Total", "Rewarded");
			Dictionary<string, object> eventParams = dictionary;
			AnalyticsFacade.SendCustomEvent("Daily Quests", eventParams);
			string value = string.Format(CultureInfo.InvariantCulture, "{0} / {1}", Quest.Id, QuestConstants.GetDifficultyKey(Quest.Difficulty));
			dictionary = new Dictionary<string, object>();
			dictionary.Add("Quests", value);
			Dictionary<string, object> eventParams2 = dictionary;
			AnalyticsFacade.SendCustomEvent("Daily Quests", eventParams2);
			QuestSystem.Instance.QuestProgress.TryRemoveTutorialQuest(Quest.Id);
			QuestSystem.Instance.SaveQuestProgressIfDirty();
			getRewardButton.SetActive(false);
			completedObject.SetActive(true);
			rewardAnim.enabled = true;
			Reward reward = Quest.Reward;
			if (reward.Coins > 0)
			{
				BankController.AddCoins(reward.Coins);
			}
			if (reward.Gems > 0)
			{
				BankController.AddGems(reward.Gems);
			}
			if (reward.Experience > 0)
			{
				ExperienceController.sharedController.addExperience(reward.Experience);
			}
		}
		DailyQuestsBannerController.Instance.UpdateItems();
		MainMenuQuestSystemListener.Refresh();
	}

	public void OnSkipInMainMenuClick()
	{
		HandleSkip();
	}

	public void OnSkipInGameClick()
	{
		HandleSkip();
	}

	private void HandleSkip()
	{
		if (_questInfo == null)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogError("QuestInfo is null.");
			}
			return;
		}
		if (_questInfo.CanSkip)
		{
			_questInfo.Skip();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Total", "Skipped");
			Dictionary<string, object> eventParams = dictionary;
			AnalyticsFacade.SendCustomEvent("Daily Quests", eventParams);
			QuestBase quest = _questInfo.Quest;
			string value = string.Format(CultureInfo.InvariantCulture, "{0} / {1}", quest.Id, QuestConstants.GetDifficultyKey(quest.Difficulty));
			dictionary = new Dictionary<string, object>();
			dictionary.Add("Skip", value);
			Dictionary<string, object> eventParams2 = dictionary;
			AnalyticsFacade.SendCustomEvent("Daily Quests", eventParams2);
			DailyQuestsBannerController.Instance.Do(delegate(DailyQuestsBannerController c)
			{
				c.UpdateItems();
			});
			if (questSkipFrame != null)
			{
				questSkipFrame.SetActive(true);
			}
			if (skipAnimPosition != null)
			{
				skipAnimPosition.enabled = true;
			}
		}
		else if (Defs.IsDeveloperBuild)
		{
			Debug.LogError("Cannot skip!");
		}
		MainMenuQuestSystemListener.Refresh();
	}

	public void OnViewAllButtonClick()
	{
		BannerWindowController sharedController = BannerWindowController.SharedController;
		if (!(sharedController == null))
		{
			sharedController.ForceShowBanner(BannerWindowType.DailyQuests);
			base.gameObject.SetActive(false);
		}
	}

	private void OpenConnectScene(int mode)
	{
		PlayerPrefs.SetInt("RegimMulty", mode);
		ConnectSceneNGUIController.directedFromQuests = true;
		MainMenuController.sharedController.OnClickMultiplyerButton();
	}

	public void OnToBattleButtonClick()
	{
		if (Quest == null)
		{
			Debug.LogError("Quest is null.");
			return;
		}
		switch (Quest.Id)
		{
		case "winInMode":
		case "killInMode":
			OpenConnectScene(GetQuestMode(Quest));
			break;
		case "killFlagCarriers":
		case "captureFlags":
			OpenConnectScene(4);
			break;
		case "capturePoints":
			OpenConnectScene(5);
			break;
		case "killNpcWithWeapon":
			OpenConnectScene(1);
			break;
		case "winInMap":
			ConnectSceneNGUIController.selectedMap = GetQuestMap(Quest);
			MainMenuController.sharedController.OnClickMultiplyerButton();
			break;
		case "killWithWeapon":
		case "killViaHeadshot":
		case "killWithGrenade":
		case "revenge":
		case "breakSeries":
		case "makeSeries":
			MainMenuController.sharedController.OnClickMultiplyerButton();
			break;
		case "surviveWavesInArena":
			MainMenuController.sharedController.StartSurvivalButton();
			break;
		case "killInCampaign":
			MainMenuController.sharedController.StartCampaingButton();
			break;
		}
	}
}

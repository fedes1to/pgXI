using System;
using System.Collections;
using Unity.Linq;
using UnityEngine;

internal sealed class DailyQuestsButton : MonoBehaviour
{
	public bool inBannerSystem = true;

	[SerializeField]
	private DailyQuestsBannerController controller;

	public GameObject rewardIndicator;

	[SerializeField]
	private GameObject dailyQuestsParent;

	public event Action OnClickAction;

	private void Awake()
	{
		if (inBannerSystem)
		{
			QuestSystem.Instance.Updated += HandleQuestSystemUpdate;
		}
		else if (Defs.isDaterRegim)
		{
			base.gameObject.SetActive(false);
		}
		if (QuestSystem.Instance.QuestProgress != null)
		{
			SetUI();
		}
	}

	private void OnEnable()
	{
		SetUI();
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(UpdateUI());
		}
	}

	private void OnDisable()
	{
		controller = null;
		if (dailyQuestsParent != null)
		{
			dailyQuestsParent.transform.DestroyChildren();
		}
	}

	private void OnDestroy()
	{
		if (inBannerSystem)
		{
			QuestSystem.Instance.Updated -= HandleQuestSystemUpdate;
		}
	}

	private void OnClick()
	{
		ButtonClickSound.TryPlayClick();
		if ((BankController.Instance != null && BankController.Instance.InterfaceEnabled) || (ExpController.Instance != null && ExpController.Instance.IsLevelUpShown) || ShopNGUIController.GuiActive || ExperienceController.sharedController.isShowNextPlashka)
		{
			return;
		}
		if (this.OnClickAction != null)
		{
			this.OnClickAction();
		}
		else if (inBannerSystem)
		{
			BannerWindowController sharedController = BannerWindowController.SharedController;
			if (!(sharedController == null))
			{
				sharedController.ForceShowBanner(BannerWindowType.DailyQuests);
			}
		}
		else
		{
			if (LoadingInAfterGame.isShowLoading)
			{
				return;
			}
			if ((controller == null || controller.gameObject == null) && dailyQuestsParent != null)
			{
				dailyQuestsParent.transform.DestroyChildren();
				DailyQuestsBannerController original = Resources.Load<DailyQuestsBannerController>("Windows/DailyQuests-Window");
				DailyQuestsBannerController dailyQuestsBannerController = UnityEngine.Object.Instantiate(original);
				if (dailyQuestsBannerController != null)
				{
					dailyQuestsBannerController.transform.parent = dailyQuestsParent.transform;
					dailyQuestsBannerController.transform.localPosition = Vector3.zero;
					dailyQuestsBannerController.transform.localRotation = Quaternion.identity;
					dailyQuestsBannerController.transform.localScale = Vector3.one;
					int layer = base.gameObject.layer;
					dailyQuestsBannerController.gameObject.layer = layer;
					foreach (GameObject item in dailyQuestsBannerController.gameObject.Descendants())
					{
						item.layer = layer;
					}
					dailyQuestsBannerController.inBannerSystem = inBannerSystem;
				}
				controller = dailyQuestsBannerController;
			}
			if (controller != null)
			{
				controller.Show();
			}
		}
	}

	private void CheckUnrewardedEvent(object sender, EventArgs e)
	{
		SetUI();
	}

	public void SetUI()
	{
		bool flag = QuestSystem.Instance.QuestProgress != null && QuestSystem.Instance.AnyActiveQuest;
		if (rewardIndicator != null && QuestSystem.Instance.QuestProgress != null)
		{
			bool active = QuestSystem.Instance.QuestProgress.HasUnrewaredAccumQuests();
			rewardIndicator.SetActive(active);
		}
	}

	private IEnumerator UpdateUI()
	{
		WaitForSeconds delay = new WaitForSeconds(0.5f);
		while (true)
		{
			yield return delay;
			SetUI();
		}
	}

	private void HandleQuestSystemUpdate(object sender, EventArgs e)
	{
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("Refreshing after quest system update.");
		}
		SetUI();
	}
}

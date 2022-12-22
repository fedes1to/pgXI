using System;
using UnityEngine;

internal sealed class MainMenuQuestSystemListener : MonoBehaviour
{
	public DailyQuestItem dailyQuestItem;

	private static MainMenuQuestSystemListener _instance;

	public static void Refresh()
	{
		if (!(_instance == null) && !(_instance.dailyQuestItem == null))
		{
			_instance.dailyQuestItem.Refresh();
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		QuestSystem.Instance.Updated += HandleQuestSystemUpdate;
	}

	private void OnDestroy()
	{
		QuestSystem.Instance.Updated -= HandleQuestSystemUpdate;
	}

	private void HandleQuestSystemUpdate(object sender, EventArgs e)
	{
		if (dailyQuestItem != null)
		{
			dailyQuestItem.Refresh();
		}
	}
}

using Rilisoft;
using Unity.Linq;
using UnityEngine;

public class DailyQuestBannerHandler : MonoBehaviour
{
	public bool inBannerSystem = true;

	[SerializeField]
	private DailyQuestsButton questsButton;

	[SerializeField]
	private GameObject _windowRoot;

	[SerializeField]
	private PrefabHandler _prefab;

	[SerializeField]
	private LazyObject<DailyQuestsBannerController> _controller;

	public static DailyQuestBannerHandler Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		_controller = new LazyObject<DailyQuestsBannerController>(_prefab.ResourcePath, _windowRoot);
		ExpController.LevelUpShown += HandleLevelUpShown;
		if (questsButton != null)
		{
			questsButton.OnClickAction += ShowUI;
		}
	}

	private void OnDetroy()
	{
		ExpController.LevelUpShown -= HandleLevelUpShown;
	}

	private void HandleLevelUpShown()
	{
		if (_controller.ObjectIsLoaded)
		{
			_controller.Value.Hide();
		}
	}

	public void ShowUI()
	{
		if (!_controller.ObjectIsLoaded)
		{
			DailyQuestsBannerController value = _controller.Value;
			int layer = _windowRoot.layer;
			value.gameObject.layer = layer;
			foreach (GameObject item in value.gameObject.Descendants())
			{
				item.layer = layer;
			}
			_controller.Value.inBannerSystem = inBannerSystem;
		}
		_controller.Value.Show();
		if (questsButton != null)
		{
			questsButton.SetUI();
		}
	}

	public void HideUI()
	{
		_controller.Value.Hide();
	}
}

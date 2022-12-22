using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Rilisoft;
using UnityEngine;

public class LevelUpWithOffers : MonoBehaviour
{
	public struct ItemDesc
	{
		public string tag;

		public ShopNGUIController.CategoryNames category;
	}

	public RewardWindowBase shareScript;

	public UILabel[] rewardGemsPriceLabel;

	public UILabel[] currentRankLabel;

	public UILabel[] rewardPriceLabel;

	public UILabel[] healthLabel;

	public UILabel[] gemsStarterBank;

	public UILabel[] coinsStarterBank;

	public List<UILabel> youReachedLabels;

	public NewAvailableItemInShop[] items;

	public bool isTierLevelUp;

	private float gemsStarterBankValue;

	private float coinsStarterBankValue;

	private IEnumerator UpdatePanelsAndAnchors()
	{
		yield return new WaitForEndOfFrame();
		Player_move_c.PerformActionRecurs(base.transform.parent.parent.parent.gameObject, delegate(Transform t)
		{
			UIPanel component2 = t.GetComponent<UIPanel>();
			if (component2 != null)
			{
				component2.Refresh();
			}
		});
		Player_move_c.PerformActionRecurs(base.transform.parent.parent.parent.gameObject, delegate(Transform t)
		{
			UIRect component = t.GetComponent<UIRect>();
			if (component != null)
			{
				component.UpdateAnchors();
			}
		});
	}

	private void Awake()
	{
		if (!isTierLevelUp)
		{
			FacebookController.StoryPriority levelupPriority = FacebookController.StoryPriority.Red;
			shareScript.priority = levelupPriority;
			shareScript.shareAction = delegate
			{
				FacebookController.PostOpenGraphStory("reach", "level", levelupPriority, new Dictionary<string, string> { 
				{
					"level",
					ExperienceController.sharedController.currentLevel.ToString()
				} });
			};
			shareScript.HasReward = true;
			shareScript.twitterStatus = () => string.Format("I've reached level {0} in @PixelGun3D! Come to the battle and try to defeat me! #pixelgun3d #pixelgun #3d #pg3d #fps http://goo.gl/8fzL9u", ExperienceController.sharedController.currentLevel);
			shareScript.EventTitle = "Level-up";
		}
		else
		{
			FacebookController.StoryPriority tierupPriority = FacebookController.StoryPriority.Green;
			shareScript.priority = tierupPriority;
			shareScript.shareAction = delegate
			{
				FacebookController.PostOpenGraphStory("unlock", "new weapon", tierupPriority, new Dictionary<string, string> { 
				{
					"new weapon",
					(ExpController.Instance.OurTier + 1).ToString()
				} });
			};
			shareScript.HasReward = true;
			shareScript.twitterStatus = () => "I've unlocked cool new weapons in @PixelGun3D! Let’s try them out! #pixelgun3d #pixelgun #3d #pg3d #mobile #fps http://goo.gl/8fzL9u";
			shareScript.EventTitle = "Tier-up";
		}
	}

	[ContextMenu("Update")]
	public void OnEnable()
	{
		StartCoroutine(UpdatePanelsAndAnchors());
	}

	private void OnDisable()
	{
		ShowIndicationMoney();
	}

	private void OnDestroy()
	{
		ShowIndicationMoney();
	}

	private void ShowIndicationMoney()
	{
		BankController.canShowIndication = true;
		BankController.UpdateAllIndicatorsMoney();
	}

	public void SetCurrentRank(string currentRank)
	{
		for (int i = 0; i < currentRankLabel.Length; i++)
		{
			currentRankLabel[i].text = LocalizationStore.Get("Key_0226").ToUpper() + " " + currentRank + "!";
		}
		string text = string.Empty;
		switch (ProfileController.CurOrderCup)
		{
		case 0:
			text = ScriptLocalization.Get("Key_1938");
			break;
		case 1:
			text = ScriptLocalization.Get("Key_1939");
			break;
		case 2:
			text = ScriptLocalization.Get("Key_1940");
			break;
		case 3:
			text = ScriptLocalization.Get("Key_1941");
			break;
		case 4:
			text = ScriptLocalization.Get("Key_1942");
			break;
		case 5:
			text = ScriptLocalization.Get("Key_1943");
			break;
		}
		foreach (UILabel youReachedLabel in youReachedLabels)
		{
			youReachedLabel.text = text;
		}
	}

	public void SetRewardPrice(string rewardPrice)
	{
		for (int i = 0; i < rewardPriceLabel.Length; i++)
		{
			rewardPriceLabel[i].text = rewardPrice;
		}
	}

	public void SetGemsRewardPrice(string gemsReward)
	{
		for (int i = 0; i < rewardGemsPriceLabel.Length; i++)
		{
			rewardGemsPriceLabel[i].text = gemsReward;
		}
	}

	public void SetAddHealthCount(string count)
	{
		if (healthLabel != null)
		{
			for (int i = 0; i < healthLabel.Length; i++)
			{
				healthLabel[i].text = count;
			}
		}
	}

	private void SetGemsLabel(int value)
	{
		for (int i = 0; i < gemsStarterBank.Length; i++)
		{
			gemsStarterBank[i].text = string.Format(LocalizationStore.Get("Key_1531"), value);
		}
	}

	private void SetCoinsLabel(int value)
	{
		for (int i = 0; i < coinsStarterBank.Length; i++)
		{
			coinsStarterBank[i].text = string.Format(LocalizationStore.Get("Key_1530"), value);
		}
	}

	public IEnumerator GemsStarterAnimation()
	{
		float seconds = 0f;
		SetGemsLabel(0);
		while (seconds < 1f)
		{
			for (int i = 0; i < gemsStarterBank.Length; i++)
			{
				SetGemsLabel(Mathf.RoundToInt(Mathf.Lerp(0f, gemsStarterBankValue, seconds)));
			}
			seconds += Time.deltaTime;
			yield return null;
		}
		SetGemsLabel(Mathf.RoundToInt(gemsStarterBankValue));
	}

	public IEnumerator CoinsStarterAnimation()
	{
		float seconds = 0f;
		SetCoinsLabel(0);
		while (seconds < 1f)
		{
			for (int i = 0; i < coinsStarterBank.Length; i++)
			{
				SetCoinsLabel(Mathf.RoundToInt(Mathf.Lerp(0f, coinsStarterBankValue, seconds)));
			}
			seconds += Time.deltaTime;
			yield return null;
		}
		SetCoinsLabel(Mathf.RoundToInt(coinsStarterBankValue));
	}

	public void SetStarterBankValues(int gemsReward, int coinsReward)
	{
		gemsStarterBankValue = gemsReward;
		coinsStarterBankValue = coinsReward;
		SetGemsLabel(0);
		SetCoinsLabel(0);
	}

	public void SetItems(List<string> itemTags)
	{
		if (items == null || items.Length == 0)
		{
			return;
		}
		for (int i = 0; i < items.Length; i++)
		{
			items[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < itemTags.Count; j++)
		{
			items[j].gameObject.SetActive(true);
			string text = itemTags[j];
			int itemCategory = ItemDb.GetItemCategory(text);
			items[j]._tag = text;
			items[j].category = (ShopNGUIController.CategoryNames)itemCategory;
			items[j].itemImage.mainTexture = ItemDb.GetItemIcon(text, (ShopNGUIController.CategoryNames)itemCategory, 1);
			foreach (UILabel item in items[j].itemName)
			{
				item.text = ItemDb.GetItemName(text, (ShopNGUIController.CategoryNames)itemCategory);
			}
			items[j].GetComponent<UIButton>().isEnabled = !Defs.isHunger || text == null || ItemDb.GetByTag(text) == null;
		}
	}

	public void Close()
	{
		ExpController.Instance.HandleContinueButton(base.gameObject);
	}
}

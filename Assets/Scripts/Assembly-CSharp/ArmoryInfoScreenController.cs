using System;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public class ArmoryInfoScreenController : MonoBehaviour
{
	public static ArmoryInfoScreenController sharedController;

	public PropertiesArmoryItemContainer propertiesContainer;

	public SetHeadLabelText itemNameLabel;

	public UILabel rarityLabel;

	public UIScrollView itemsScroll;

	public UIGrid itemsGrid;

	public UpgradeWindow[] upgradesWindows;

	private UpgradeWindow currentUpgradeWindows;

	public GameObject labelFirstBuy;

	public PriceContainer priceContainer;

	public GameObject discountContainer;

	public UILabel discountLabel;

	public GameObject previewPrefab;

	[SerializeField]
	private GameObject hintsContainer;

	private List<ItemPreviewInArmoryInfoScreen> listItems = new List<ItemPreviewInArmoryInfoScreen>();

	private ItemPreviewInArmoryInfoScreen selectedItemController;

	private int curCountUpgrades;

	private IDisposable _infoScreenBackSubscription;

	private List<string> chainForTag;

	private List<string> actualUpgrades;

	public void Close()
	{
		if (ButtonClickSound.Instance != null)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		DestroyWindow();
	}

	public void DestroyWindow()
	{
		base.transform.parent = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Awake()
	{
		DisposeInfoScreenBackSubscription();
		_infoScreenBackSubscription = BackSystem.Instance.Register(Close, "Info screen in Armory");
	}

	private void Start()
	{
		sharedController = this;
	}

	private void OnDestroy()
	{
		sharedController = null;
		DisposeInfoScreenBackSubscription();
	}

	private void DisposeInfoScreenBackSubscription()
	{
		if (_infoScreenBackSubscription != null)
		{
			_infoScreenBackSubscription.Dispose();
			_infoScreenBackSubscription = null;
		}
	}

	public void OnSelectItem(ItemPreviewInArmoryInfoScreen selectedItem, ShopNGUIController.CategoryNames category)
	{
		foreach (ItemPreviewInArmoryInfoScreen listItem in listItems)
		{
			listItem.SetSelected(listItem.Equals(selectedItem));
		}
		itemNameLabel.SetText(selectedItem.headName);
		selectedItemController = selectedItem;
		string text = null;
		string viewedId = selectedItem.id;
		if (ShopNGUIController.IsWeaponCategory(category))
		{
			text = WeaponManager.LastBoughtTag(selectedItem.id);
			viewedId = text ?? WeaponManager.FirstUnboughtOrForOurTier(selectedItem.id);
		}
		else if (ShopNGUIController.IsGadgetsCategory(category))
		{
			text = GadgetsInfo.LastBoughtFor(selectedItem.id);
			viewedId = text ?? GadgetsInfo.FirstUnboughtOrForOurTier(selectedItem.id);
		}
		if (currentUpgradeWindows != null)
		{
			currentUpgradeWindows.SetUpgrade(selectedItem.numUpgrade, (text == null) ? (-1) : actualUpgrades.IndexOf(text));
		}
		ShopNGUIController.sharedShop.GetStateButtons(viewedId, selectedItem.id, propertiesContainer, false);
	}

	public void SetItem(ShopNGUIController.ShopItem item)
	{
		List<string> list = new List<string>();
		int num = 0;
		if (ShopNGUIController.IsWeaponCategory(item.Category))
		{
			chainForTag = WeaponUpgrades.ChainForTag(item.Id);
			string item2 = WeaponManager.FirstTagForOurTier(item.Id);
			if (chainForTag != null)
			{
				int num2 = chainForTag.IndexOf(item2);
				actualUpgrades = chainForTag.GetRange(num2, chainForTag.Count - num2);
			}
			else
			{
				actualUpgrades = new List<string> { item.Id };
			}
			curCountUpgrades = actualUpgrades.Count;
			for (int i = 0; i < upgradesWindows.Length; i++)
			{
				upgradesWindows[i].gameObject.SetActive(i == curCountUpgrades - 1);
			}
			currentUpgradeWindows = upgradesWindows[curCountUpgrades - 1];
			string text = WeaponManager.FirstUnboughtOrForOurTier(item.Id);
			num = actualUpgrades.IndexOf(text);
			if (num > 0 && !text.Equals(item.Id))
			{
				num--;
			}
			for (int j = num; j < actualUpgrades.Count; j++)
			{
				list.Add(actualUpgrades[j]);
			}
		}
		else if (ShopNGUIController.IsGadgetsCategory(item.Category))
		{
			List<string> list2 = GadgetsInfo.Upgrades[item.Id];
			chainForTag = GadgetsInfo.Upgrades[item.Id];
			string item3 = GadgetsInfo.FirstForOurTier(item.Id);
			if (chainForTag != null)
			{
				int num3 = chainForTag.IndexOf(item3);
				actualUpgrades = chainForTag.GetRange(num3, chainForTag.Count - num3);
			}
			else
			{
				actualUpgrades = new List<string> { item.Id };
			}
			curCountUpgrades = actualUpgrades.Count;
			for (int k = 0; k < upgradesWindows.Length; k++)
			{
				upgradesWindows[k].gameObject.SetActive(k == curCountUpgrades - 1);
			}
			currentUpgradeWindows = upgradesWindows[curCountUpgrades - 1];
			string text2 = GadgetsInfo.FirstUnboughtOrForOurTier(item.Id);
			num = actualUpgrades.IndexOf(text2);
			if (num > 0 && !text2.Equals(item.Id))
			{
				num--;
			}
			for (int l = num; l < actualUpgrades.Count; l++)
			{
				list.Add(actualUpgrades[l]);
			}
		}
		LoadPreviewCarousel(item.Category, item.Id, list, num, (!(selectedItemController != null)) ? string.Empty : selectedItemController.id);
		if (selectedItemController != null)
		{
			selectedItemController.SetSelected(true, true);
			OnSelectItem(selectedItemController, item.Category);
		}
	}

	public void LoadPreviewCarousel(ShopNGUIController.CategoryNames category, string itemId, List<string> _availableTags, int startIndexUpgrade, string selectedItemID)
	{
		while (itemsGrid.transform.childCount > 0)
		{
			Transform child = itemsGrid.transform.GetChild(0);
			child.parent = null;
			UnityEngine.Object.Destroy(child.gameObject);
		}
		listItems.Clear();
		bool flag = ShopNGUIController.IsWeaponCategory(category);
		bool flag2 = ShopNGUIController.IsWearCategory(category);
		bool flag3 = ShopNGUIController.IsGadgetsCategory(category);
		for (int i = 0; i < _availableTags.Count; i++)
		{
			string currentId = _availableTags[i];
			WeaponSounds wsForPos = null;
			GameObject pref = null;
			string headName = string.Empty;
			if (flag)
			{
				GameObject prefabByTag = WeaponManager.sharedManager.GetPrefabByTag(currentId);
				wsForPos = prefabByTag.GetComponent<WeaponSounds>();
				pref = WeaponManager.InnerPrefabForWeaponSync(prefabByTag.nameNoClone());
				headName = ItemDb.GetItemName(itemId, category);
			}
			if (flag2)
			{
				pref = ItemDb.GetWearFromResources(currentId, category);
			}
			if (flag3)
			{
				pref = GadgetsInfo.GetArmoryInfoPrefabFromName(currentId);
				headName = ItemDb.GetItemName(itemId, category);
			}
			GameObject _preview = UnityEngine.Object.Instantiate(previewPrefab);
			_preview.SetActive(true);
			_preview.name = "item_" + i;
			_preview.transform.SetParent(itemsGrid.transform);
			float num = ((!(currentId == selectedItemID)) ? ItemPreviewInArmoryInfoScreen.minScale : ItemPreviewInArmoryInfoScreen.maxScale);
			_preview.transform.localScale = new Vector3(num, num, num);
			if (currentId.Equals("cape_Custom"))
			{
				Tools.SetTextureRecursivelyFrom(_preview, SkinsController.capeUserTexture, new GameObject[0]);
			}
			ItemPreviewInArmoryInfoScreen itemController = _preview.GetComponent<ItemPreviewInArmoryInfoScreen>();
			itemController.id = currentId;
			itemController.category = category;
			itemController.headName = headName;
			itemController.id = currentId;
			itemController.numUpgrade = startIndexUpgrade + i;
			if (currentId.Equals(itemId))
			{
				selectedItemController = itemController;
			}
			itemController.OnSelect += OnSelectItem;
			listItems.Add(itemController);
			ShopNGUIController.AddModel(pref, delegate(GameObject manipulateObject, Vector3 positionShop, Vector3 rotationShop, string readableName, float scaleCoefShop, int tier, int league)
			{
				if (_preview == null)
				{
					UnityEngine.Object.Destroy(manipulateObject);
				}
				else
				{
					manipulateObject.transform.SetParent(_preview.transform);
					manipulateObject.transform.localScale = new Vector3(scaleCoefShop, scaleCoefShop, scaleCoefShop);
					if (currentId == "Eagle_3")
					{
						manipulateObject.transform.GetChild(0).localPosition = Vector3.zero;
						manipulateObject.transform.localPosition = new Vector3(0f, -10f, -1000f);
					}
					else if (currentId == "Fighter_1" || currentId == "Fighter_2")
					{
						manipulateObject.transform.GetChild(0).localPosition = Vector3.zero;
						manipulateObject.transform.GetChild(0).localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
						manipulateObject.transform.localPosition = new Vector3(-21.5f, 12.5f, -1000f);
					}
					else
					{
						manipulateObject.transform.localPosition = new Vector3(positionShop.x, positionShop.y, -1000f);
					}
					manipulateObject.transform.localRotation = Quaternion.Euler(rotationShop);
					itemController.model = manipulateObject.transform;
					itemController.baseRotation = rotationShop;
				}
			}, category, false, wsForPos);
		}
		itemsGrid.Reposition();
	}

	public void UpgradeButtonOnClick()
	{
		if (ButtonClickSound.Instance != null)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		ShopNGUIController.sharedShop.HandleUpgradeButton();
		if (base.gameObject.activeInHierarchy)
		{
			SetItem(ShopNGUIController.sharedShop.CurrentItem);
		}
	}

	public void BuyButtonButtonOnClick()
	{
		if (ButtonClickSound.Instance != null)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		ShopNGUIController.sharedShop.HandleBuyButton();
		if (base.gameObject.activeInHierarchy)
		{
			SetItem(ShopNGUIController.sharedShop.CurrentItem);
		}
	}

	public void EquipButtonOnClick()
	{
		if (ButtonClickSound.Instance != null)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		ShopNGUIController.sharedShop.HandleEquipButton();
		string viewedId = WeaponManager.LastBoughtTag(selectedItemController.id) ?? WeaponManager.FirstUnboughtOrForOurTier(selectedItemController.id);
		ShopNGUIController.sharedShop.GetStateButtons(viewedId, selectedItemController.id, propertiesContainer, false);
	}

	public void TryGunOnClick()
	{
	}
}

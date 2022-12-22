using System.Collections.Generic;
using UnityEngine;

public sealed class LoadingNGUIController : MonoBehaviour
{
	private string sceneToLoad = string.Empty;

	public UITexture loadingNGUITexture;

	public UILabel[] levelNameLabels;

	public UILabel recommendedForThisMap;

	public Transform gunsPoint;

	public string SceneToLoad
	{
		set
		{
			sceneToLoad = value;
		}
	}

	public void Init()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("PromoForLoadings");
		if (textAsset == null)
		{
			return;
		}
		string text = textAsset.text;
		if (text == null)
		{
			return;
		}
		string[] array = text.Split('\r', '\n');
		Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			string[] array3 = text2.Split('\t');
			if (array3.Length < 2 || array3[0] == null || sceneToLoad == null || !array3[0].Equals(sceneToLoad))
			{
				continue;
			}
			List<string> list = new List<string>();
			for (int j = 1; j < array3.Length; j++)
			{
				if (array3[j] != null && array3[j].Equals("armor"))
				{
					list.AddRange(Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0]);
				}
				else
				{
					if (array3[j] != null && array3[j].Equals("hat"))
					{
						continue;
					}
					for (int k = 0; k < WeaponManager.sharedManager.weaponsInGame.Length; k++)
					{
						if (WeaponManager.sharedManager.weaponsInGame[k].name.Equals(array3[j]))
						{
							array3[j] = ItemDb.GetByPrefabName(WeaponManager.sharedManager.weaponsInGame[k].name).Tag;
							array3[j] = PromoActionsGUIController.FilterForLoadings(array3[j], list) ?? string.Empty;
							break;
						}
					}
					if (!string.IsNullOrEmpty(array3[j]))
					{
						list.Add(array3[j]);
					}
				}
			}
			List<string> list2 = PromoActionsGUIController.FilterPurchases(list, true);
			foreach (string item in list2)
			{
				list.Remove(item);
			}
			if (dictionary.ContainsKey(array3[0]))
			{
				dictionary[array3[0]] = list;
			}
			else
			{
				dictionary.Add(array3[0], list);
			}
		}
		if (sceneToLoad != null && dictionary.ContainsKey(sceneToLoad ?? string.Empty))
		{
			List<string> list3 = dictionary[sceneToLoad ?? string.Empty];
			if (list3 != null)
			{
				for (int l = 0; l < list3.Count; l++)
				{
					GameObject gameObject = Object.Instantiate(Resources.Load("PromoItemForLoading") as GameObject);
					gameObject.transform.parent = gunsPoint;
					gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
					gameObject.transform.localPosition = new Vector3(-256f * (float)list3.Count / 2f + 128f + (float)l * 256f, 0f, 0f);
					PromoItemForLoading component = gameObject.GetComponent<PromoItemForLoading>();
					int itemCategory = ItemDb.GetItemCategory(list3[l]);
					Texture itemIcon = ItemDb.GetItemIcon(list3[l], (ShopNGUIController.CategoryNames)itemCategory, null, false);
					UITexture[] texture = component.texture;
					foreach (UITexture uITexture in texture)
					{
						if (itemIcon != null)
						{
							uITexture.mainTexture = itemIcon;
						}
					}
					UILabel[] label = component.label;
					foreach (UILabel uILabel in label)
					{
						uILabel.text = ItemDb.GetItemName(list3[l], (ShopNGUIController.CategoryNames)itemCategory).Trim().Replace(" -", "-");
					}
				}
			}
		}
		recommendedForThisMap.gameObject.SetActive(sceneToLoad != null && dictionary.ContainsKey(sceneToLoad) && dictionary[sceneToLoad] != null && dictionary[sceneToLoad].Count > 0);
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(sceneToLoad);
		UILabel[] array4 = levelNameLabels;
		foreach (UILabel uILabel2 in array4)
		{
			if (infoScene != null && !string.IsNullOrEmpty(sceneToLoad))
			{
				uILabel2.gameObject.SetActive(true);
				string translatePreviewName = infoScene.TranslatePreviewName;
				translatePreviewName = translatePreviewName.Replace("\n", " ");
				translatePreviewName = translatePreviewName.Replace("\r", " ");
				translatePreviewName = translatePreviewName.ToUpper();
				uILabel2.text = translatePreviewName;
			}
			else
			{
				uILabel2.gameObject.SetActive(false);
			}
		}
	}

	public void SetEnabledMapName(bool enabled)
	{
		for (int i = 0; i < levelNameLabels.Length; i++)
		{
			levelNameLabels[i].gameObject.SetActive(enabled);
		}
	}

	public void SetEnabledGunsScroll(bool enabled)
	{
		if (recommendedForThisMap != null)
		{
			recommendedForThisMap.gameObject.SetActive(enabled);
		}
		if (gunsPoint != null)
		{
			gunsPoint.gameObject.SetActive(enabled);
		}
	}

	private void OnDestroy()
	{
		loadingNGUITexture = null;
	}
}

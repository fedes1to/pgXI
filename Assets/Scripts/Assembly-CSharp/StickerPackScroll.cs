using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerPackScroll : MonoBehaviour
{
	public List<TypePackSticker> listItemData = new List<TypePackSticker>();

	public List<BtnPackItem> listButton = new List<BtnPackItem>();

	public GameObject parentButton;

	public TypePackSticker curShowPack;

	private UIGrid sortScript;

	private void Awake()
	{
		listButton.Clear();
		listButton.AddRange(GetComponentsInChildren<BtnPackItem>(true));
	}

	private void OnEnable()
	{
		UpdateListButton();
		StickersController.onBuyPack += UpdateListButton;
	}

	private void OnDisable()
	{
		StickersController.onBuyPack -= UpdateListButton;
	}

	public void UpdateListButton()
	{
		StartCoroutine(crtUpdateListButton());
	}

	private IEnumerator crtUpdateListButton()
	{
		if (sortScript == null)
		{
			sortScript = parentButton.GetComponent<UIGrid>();
		}
		listItemData = StickersController.GetAvaliablePack();
		BtnPackItem fistAvaliableBtn = null;
		for (int i = 0; i < listButton.Count; i++)
		{
			BtnPackItem curButtonItem = listButton[i];
			if (listItemData.Contains(curButtonItem.typePack))
			{
				curButtonItem.transform.parent = parentButton.transform;
				curButtonItem.gameObject.SetActive(true);
				if (fistAvaliableBtn == null)
				{
					fistAvaliableBtn = curButtonItem;
				}
			}
			else
			{
				curButtonItem.transform.parent = base.transform;
				curButtonItem.gameObject.SetActive(false);
			}
		}
		if (fistAvaliableBtn != null)
		{
			ShowPack(fistAvaliableBtn.typePack);
		}
		yield return null;
		Sort();
	}

	public void Sort()
	{
		if (sortScript != null)
		{
			parentButton.SetActive(false);
			parentButton.SetActive(true);
			sortScript.Reposition();
		}
	}

	public void ShowPack(TypePackSticker val)
	{
		for (int i = 0; i < listButton.Count; i++)
		{
			BtnPackItem btnPackItem = listButton[i];
			if (btnPackItem.typePack == val)
			{
				btnPackItem.ShowPack();
				curShowPack = btnPackItem.typePack;
			}
			else
			{
				btnPackItem.HidePack();
			}
		}
	}
}

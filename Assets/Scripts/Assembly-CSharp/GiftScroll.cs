using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftScroll : MonoBehaviour
{
	private List<SlotInfo> listItemData = new List<SlotInfo>();

	public List<GiftHUDItem> listButton = new List<GiftHUDItem>();

	public GiftHUDItem exampleBut;

	public GameObject parentButton;

	public UIWrapContent wrapScript;

	public UIScrollView scView;

	public BoxCollider scrollAreaCollider;

	public static bool canReCreateSlots = true;

	private void Awake()
	{
		if ((bool)exampleBut)
		{
			exampleBut.gameObject.SetActive(false);
		}
		scView = GetComponentInParent<UIScrollView>();
	}

	private void OnEnable()
	{
		GiftController.OnChangeSlots += UpdateListButton;
		UpdateListButton();
	}

	private void OnDisable()
	{
		GiftController.OnChangeSlots -= UpdateListButton;
	}

	public void UpdateListButton()
	{
		if (canReCreateSlots && base.gameObject.activeInHierarchy)
		{
			StartCoroutine(crtUpdateListButton());
		}
	}

	private IEnumerator crtUpdateListButton()
	{
		while (GiftBannerWindow.instance == null)
		{
			yield return null;
		}
		if (wrapScript == null)
		{
			wrapScript = parentButton.GetComponent<UIWrapContent>();
		}
		listItemData = GiftController.Instance.Slots;
		SetButtonCount(listItemData.Count);
		for (int i = 0; i < listButton.Count; i++)
		{
			GiftHUDItem curButtonRoom = listButton[i];
			curButtonRoom.gameObject.name = i + "_" + listItemData[i].gift.Id;
			curButtonRoom.SetInfoButton(listItemData[i]);
		}
		Sort();
	}

	public void Sort()
	{
		if (canReCreateSlots)
		{
			StartCoroutine(CrtSort());
		}
	}

	private IEnumerator CrtSort()
	{
		yield return null;
		GiftBannerWindow.instance.UpdateSizeScroll();
		scView.ResetPosition();
		if (wrapScript != null)
		{
			wrapScript.SortAlphabetically();
		}
		if (wrapScript != null)
		{
			wrapScript.WrapContent();
		}
		scView.restrictWithinPanel = true;
		yield return null;
		scView.disableDragIfFits = false;
		listButton[0].InCenter(false, 1);
	}

	private void SetButtonCount(int needCount)
	{
		if (listButton.Count < needCount)
		{
			for (int i = listButton.Count; i < needCount; i++)
			{
				GiftHUDItem item = CreateButton();
				listButton.Add(item);
			}
		}
		else if (listButton.Count > needCount)
		{
			int num = listButton.Count - needCount;
			for (int j = 0; j < num; j++)
			{
				GameObject obj = listButton[listButton.Count - 1].gameObject;
				listButton[listButton.Count - 1] = null;
				listButton.RemoveAt(listButton.Count - 1);
				Object.Destroy(obj);
			}
		}
	}

	private GiftHUDItem CreateButton()
	{
		GameObject gameObject = Object.Instantiate(exampleBut.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.SetActive(true);
		GiftHUDItem component = gameObject.GetComponent<GiftHUDItem>();
		gameObject.transform.parent = parentButton.transform;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		return component;
	}

	public void AnimScrollGift(int num)
	{
		if (listButton.Count > num)
		{
			listButton[num].InCenter(true, listButton.Count);
		}
	}

	public void SetCanDraggable(bool val)
	{
		if ((bool)scrollAreaCollider)
		{
			scrollAreaCollider.enabled = val;
		}
		for (int i = 0; i < listButton.Count; i++)
		{
			listButton[i].colliderForDrag.enabled = val;
		}
	}

	[ContextMenu("Sort gift")]
	private void TestSortGift()
	{
		Sort();
	}

	[ContextMenu("Center main gift")]
	private void TestCenterGift()
	{
		if (listButton.Count > 6)
		{
			listButton[0].InCenter(false, 1);
		}
	}
}

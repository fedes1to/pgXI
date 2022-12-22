using UnityEngine;

public class BtnPackItem : MonoBehaviour
{
	public TypePackSticker typePack;

	public GameObject objListSticker;

	public GameObject activeState;

	public GameObject noActiveState;

	private StickerPackScroll scrollPack;

	private void Awake()
	{
		scrollPack = GetComponentInParent<StickerPackScroll>();
	}

	private void OnClick()
	{
		if ((bool)scrollPack)
		{
			ButtonClickSound.Instance.PlayClick();
			scrollPack.ShowPack(typePack);
		}
	}

	public void ShowPack()
	{
		if ((bool)objListSticker)
		{
			objListSticker.SetActive(true);
			UIGrid component = objListSticker.GetComponent<UIGrid>();
			if (component != null)
			{
				component.Reposition();
			}
		}
		if ((bool)activeState)
		{
			activeState.SetActive(true);
		}
		if ((bool)noActiveState)
		{
			noActiveState.SetActive(false);
		}
	}

	public void HidePack()
	{
		if ((bool)objListSticker)
		{
			objListSticker.SetActive(false);
		}
		if ((bool)activeState)
		{
			activeState.SetActive(false);
		}
		if ((bool)noActiveState)
		{
			noActiveState.SetActive(true);
		}
	}
}

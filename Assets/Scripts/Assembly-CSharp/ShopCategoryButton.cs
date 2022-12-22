using System;
using UnityEngine;

public class ShopCategoryButton : MonoBehaviour
{
	public UITexture icon;

	public GameObject emptyIcon;

	public Transform modelPoint;

	public static event Action<ShopCategoryButton> CategoryButtonClicked;

	private void OnClick()
	{
		Action<ShopCategoryButton> categoryButtonClicked = ShopCategoryButton.CategoryButtonClicked;
		if (categoryButtonClicked != null)
		{
			categoryButtonClicked(this);
		}
	}
}

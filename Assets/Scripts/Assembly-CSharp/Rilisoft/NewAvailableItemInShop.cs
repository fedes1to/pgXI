using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	public class NewAvailableItemInShop : MonoBehaviour
	{
		public string _tag = string.Empty;

		public ShopNGUIController.CategoryNames category;

		public UITexture itemImage;

		public List<UILabel> itemName;

		private void OnClick()
		{
			LevelUpWithOffers componentInParent = GetComponentInParent<LevelUpWithOffers>();
			if (componentInParent != null)
			{
				ExpController.Instance.HandleNewAvailableItem(componentInParent.gameObject, this);
			}
		}

		[ContextMenu("Set ref")]
		private void SetRef()
		{
			itemImage = GetComponentsInChildren<UITexture>()[1];
			itemName.Clear();
			itemName.AddRange(GetComponentsInChildren<UILabel>(true));
		}
	}
}

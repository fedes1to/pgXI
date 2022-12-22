using System;
using Rilisoft;
using UnityEngine;

public class RemoveNoviceArmorController : MonoBehaviour
{
	private IDisposable _escapeSubscription;

	private void Awake()
	{
		_escapeSubscription = BackSystem.Instance.Register(delegate
		{
			Hide();
		}, "RemoveNoviceArmorController");
		Storager.setInt("Training.ShouldRemoveNoviceArmorInShopKey", 0, false);
		ShopNGUIController.UnequipCurrentWearInCategory(ShopNGUIController.CategoryNames.ArmorCategory, false);
		ShopNGUIController.ProvideItem(ShopNGUIController.CategoryNames.ArmorCategory, "Armor_Army_1", 1, false, 0, null, null, true, false, false);
	}

	public void Hide()
	{
		base.transform.parent = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		_escapeSubscription.Dispose();
	}
}

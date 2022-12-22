using System;
using Rilisoft;
using UnityEngine;

public class UnlockedItemsArmoryIndicatorController : MonoBehaviour
{
	public UILabel label;

	private float m_lastUpdateTime = float.MinValue;

	private void Start()
	{
		UpdateIndicator();
	}

	private void Update()
	{
		try
		{
			if (!(Time.realtimeSinceStartup - 0.5f < m_lastUpdateTime))
			{
				UpdateIndicator();
			}
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Exception in UnlockedItemsArmoryIndicatorController.Update: {0}", ex);
		}
	}

	private void UpdateIndicator()
	{
		int num = ShopNGUIController.CurrentNumberOfUnlockedItems();
		bool flag = num > 0;
		label.gameObject.SetActiveSafeSelf(flag);
		if (flag)
		{
			label.text = num.ToString();
		}
		m_lastUpdateTime = Time.realtimeSinceStartup;
	}
}

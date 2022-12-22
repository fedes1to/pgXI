using System;
using UnityEngine;

public class ItemPreviewInArmoryInfoScreen : MonoBehaviour
{
	private Transform thisTranform;

	private bool isSelected;

	public string id;

	public ShopNGUIController.CategoryNames category;

	public string headName;

	public int numUpgrade;

	public Transform model;

	public Vector3 baseRotation;

	public static readonly float maxScale = 1.34f;

	public static readonly float minScale = 0.8f;

	private float timeFromRotate = 1000f;

	private float oldTime;

	public event Action<ItemPreviewInArmoryInfoScreen, ShopNGUIController.CategoryNames> OnSelect;

	public void SetSelected(bool _isSelected, bool isMomentumScale = false)
	{
		isSelected = _isSelected;
		if (isMomentumScale)
		{
			if (isSelected)
			{
				thisTranform.localScale = new Vector3(maxScale, maxScale, maxScale);
			}
			else
			{
				thisTranform.localScale = new Vector3(minScale, minScale, minScale);
			}
		}
		if (!isSelected)
		{
			model.localRotation = Quaternion.Euler(baseRotation);
		}
	}

	private void OnClick()
	{
		if (ButtonClickSound.Instance != null)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		if (!isSelected && this.OnSelect != null)
		{
			this.OnSelect(this, category);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (isSelected)
		{
			Vector3 eulerAngles = model.localRotation.eulerAngles;
			model.localRotation = Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y - delta.x, eulerAngles.z));
			timeFromRotate = 0f;
		}
	}

	private void Awake()
	{
		thisTranform = base.transform;
	}

	private void Update()
	{
		float num = Time.realtimeSinceStartup - oldTime;
		if (num > 0.5f)
		{
			num = 0f;
		}
		oldTime = Time.realtimeSinceStartup;
		if (timeFromRotate < 3f)
		{
			timeFromRotate += Time.unscaledDeltaTime;
			if (timeFromRotate >= 3f)
			{
				model.localRotation = Quaternion.Euler(baseRotation);
			}
		}
		float x = thisTranform.localScale.x;
		if (isSelected && x < maxScale)
		{
			float num2 = x + num * 2f;
			thisTranform.localScale = new Vector3(num2, num2, num2);
		}
		if (!isSelected && x > minScale)
		{
			float num3 = x - num * 2f;
			thisTranform.localScale = new Vector3(num3, num3, num3);
		}
	}
}

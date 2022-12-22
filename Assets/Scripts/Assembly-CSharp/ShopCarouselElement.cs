using System;
using System.Collections;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

public class ShopCarouselElement : MonoBehaviour
{
	public GameObject priceAndDiscountContainer;

	public GameObject gem;

	public GameObject coin;

	public List<UILabel> priceLabels;

	public List<UILabel> discountLabels;

	public UISprite frameDiscountAndPrice;

	public GameObject locked;

	public Transform arrow;

	public UILabel topSeller;

	public UILabel quantity;

	public UILabel newnew;

	public bool showTS;

	public bool showNew;

	public bool showQuantity;

	public string prefabPath;

	public Vector3 baseScale;

	public Vector3 ourPosition;

	public string itemID;

	public string readableName;

	public Transform model;

	private float lastTimeUpdated;

	public Vector3 arrnoInitialPos;

	private string IdToGetPriceFor { get; set; }

	private ShopNGUIController.CategoryNames Category { get; set; }

	public void SetupPriceAndDiscount(string idToGetPriceFor, ShopNGUIController.CategoryNames category)
	{
		IdToGetPriceFor = idToGetPriceFor;
		Category = category;
		StartUpdatePriceAndDiscount();
	}

	private void OnEnable()
	{
		StartUpdatePriceAndDiscount();
		Invoke("RecalculatePriceAndDiscount", Time.unscaledDeltaTime);
	}

	private void RecalculatePriceAndDiscountPositions(bool discount)
	{
		if (discount)
		{
			priceLabels[0].transform.localPosition = new Vector3(42f, 0f, 0f);
			frameDiscountAndPrice.width = 135;
		}
		else
		{
			frameDiscountAndPrice.width = 80;
			priceLabels[0].transform.localPosition = new Vector3(12f, 0f, 0f);
		}
	}

	private void StartUpdatePriceAndDiscount()
	{
		if (IdToGetPriceFor != null)
		{
			StartCoroutine(UpdatePriceAndDiscountCoroutine(IdToGetPriceFor, Category));
		}
	}

	private IEnumerator UpdatePriceAndDiscountCoroutine(string idToGetPriceFor, ShopNGUIController.CategoryNames category)
	{
		do
		{
			UpdatePriceAndDiscount(idToGetPriceFor, category);
			yield return CoroutineRunner.Instance.StartCoroutine(CoroutineRunner.WaitForSeconds(0.5f));
		}
		while (!(this == null) && !(base.gameObject == null));
	}

	private void UpdatePriceAndDiscount(string idToGetPriceFor, ShopNGUIController.CategoryNames category)
	{
		string text = WeaponManager.LastBoughtTag(idToGetPriceFor);
		string text2 = WeaponManager.FirstUnboughtTag(idToGetPriceFor);
		bool flag = idToGetPriceFor == text2 && text != text2;
		if (priceAndDiscountContainer.activeSelf != flag)
		{
			priceAndDiscountContainer.SetActive(flag);
		}
		if (!flag)
		{
			return;
		}
		ItemPrice itemPrice = ShopNGUIController.GetItemPrice(text2, category);
		bool onlyServerDiscount;
		int num = ShopNGUIController.DiscountFor(text2, out onlyServerDiscount);
		foreach (UILabel priceLabel in priceLabels)
		{
			priceLabel.text = itemPrice.Price.ToString();
		}
		bool flag2 = num > 0;
		foreach (UILabel discountLabel in discountLabels)
		{
			discountLabel.gameObject.SetActiveSafeSelf(flag2);
			if (flag2)
			{
				discountLabel.text = "-" + num + "%";
			}
		}
		GameObject gameObject = ((!(itemPrice.Currency == "GemsCurrency")) ? coin : gem);
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
		GameObject gameObject2 = ((!(itemPrice.Currency == "Coins")) ? coin : gem);
		if (gameObject2.activeSelf)
		{
			gameObject2.SetActive(false);
		}
		RecalculatePriceAndDiscountPositions(flag2);
	}

	public void SetQuantity()
	{
		quantity.text = Storager.getInt(GearManager.HolderQuantityForID(itemID), false) + ((itemID == null || !GearManager.HolderQuantityForID(itemID).Equals(GearManager.Grenade)) ? string.Empty : ("/" + GearManager.MaxCountForGear(GearManager.HolderQuantityForID(itemID))));
	}

	private void Awake()
	{
		arrnoInitialPos = new Vector3(70.05f, -0.00016f, -120f);
	}

	private void Start()
	{
		if (Array.IndexOf(PotionsController.potions, itemID) >= 0)
		{
			quantity.gameObject.SetActive(true);
			HandlePotionActivated(itemID);
		}
		PotionsController.PotionActivated += HandlePotionActivated;
	}

	private void HandlePotionActivated(string obj)
	{
		if (itemID != null && obj != null && itemID.Equals(obj))
		{
			quantity.text = Storager.getInt(GearManager.HolderQuantityForID(itemID), false) + ((itemID == null || !GearManager.HolderQuantityForID(itemID).Equals(GearManager.Grenade)) ? string.Empty : ("/" + GearManager.MaxCountForGear(GearManager.HolderQuantityForID(itemID))));
		}
	}

	public void SetPos(float scaleCoef, float offset)
	{
		if (model != null)
		{
			model.localScale = baseScale * scaleCoef;
			model.localPosition = new Vector3(0f, 0f, -55f);
		}
		if (arrow != null)
		{
			if (scaleCoef <= 0f)
			{
				arrow.localScale = Vector3.zero;
			}
			else
			{
				arrow.localScale = Vector3.one;
			}
			arrow.localPosition = new Vector3(arrnoInitialPos.x * scaleCoef * 0.7f, arrnoInitialPos.y * scaleCoef, -300f);
		}
		if (locked != null)
		{
			locked.transform.localScale = new Vector3(1f, 1f, 1f) * scaleCoef;
			locked.transform.localPosition = new Vector3(0f, 0f, -300f);
		}
		if (frameDiscountAndPrice != null)
		{
			if (scaleCoef <= 0f)
			{
				frameDiscountAndPrice.transform.localScale = Vector3.zero;
			}
			else
			{
				frameDiscountAndPrice.transform.localScale = Vector3.one * 0.75f;
			}
			frameDiscountAndPrice.transform.localPosition = new Vector3(0f, -70f * (scaleCoef * 1.4f), -300f);
		}
	}

	private void OnDestroy()
	{
		PotionsController.PotionActivated -= HandlePotionActivated;
	}
}

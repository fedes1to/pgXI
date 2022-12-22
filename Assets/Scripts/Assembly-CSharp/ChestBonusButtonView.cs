using UnityEngine;

public class ChestBonusButtonView : MonoBehaviour
{
	public UILabel timeOrCountLabel;

	public UITexture itemTexture;

	private PurchaseEventArgs _purchaseInfo;

	private void Awake()
	{
		LocalizationStore.AddEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void OnDestroy()
	{
		LocalizationStore.DelEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void HandleLocalizationChanged()
	{
		CheckBonusButtonUpdate();
	}

	public void Initialize()
	{
		ChestBonusController.OnChestBonusChange += CheckBonusButtonUpdate;
	}

	public void UpdateState(PurchaseEventArgs purchaseInfo)
	{
		_purchaseInfo = purchaseInfo;
		CheckBonusButtonUpdate();
	}

	public void Deinitialize()
	{
		ChestBonusController.OnChestBonusChange -= CheckBonusButtonUpdate;
		_purchaseInfo = null;
	}

	private void CheckBonusButtonUpdate()
	{
		bool flag = _purchaseInfo != null && ChestBonusController.Get.IsBonusActiveForItem(_purchaseInfo);
		base.gameObject.SetActive(flag);
		if (flag)
		{
			SetViewData(_purchaseInfo);
		}
	}

	private void SetViewData(PurchaseEventArgs purchaseInfo)
	{
		ChestBonusData bonusData = ChestBonusController.Get.GetBonusData(purchaseInfo);
		timeOrCountLabel.text = bonusData.GetItemCountOrTime();
		itemTexture.mainTexture = bonusData.GetImage();
	}

	public void OnButtonClick()
	{
		ChestBonusController.Get.ShowBonusWindowForItem(_purchaseInfo);
	}
}

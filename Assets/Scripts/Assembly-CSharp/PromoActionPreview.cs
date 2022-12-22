using UnityEngine;

public class PromoActionPreview : MonoBehaviour
{
	public UIButton button;

	public UITexture stickerTexture;

	public GameObject stickersLabel;

	public UISprite currencyImage;

	public string tg;

	public UITexture icon;

	public UILabel topSeller;

	public UILabel newItem;

	public UILabel sale;

	public UILabel coins;

	public Texture unpressed;

	public Texture pressed;

	public int Discount { get; set; }

	private void Start()
	{
		LocalizationStore.AddEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void SetSaleText()
	{
		if (Discount > 0 && sale != null)
		{
			sale.text = string.Format("{0}\n{1}%", LocalizationStore.Key_0419, Discount);
		}
	}

	private void OnEnable()
	{
		UIButton[] componentsInChildren = GetComponentsInChildren<UIButton>(true);
		foreach (UIButton uIButton in componentsInChildren)
		{
			uIButton.isEnabled = TrainingController.TrainingCompleted;
		}
		SetSaleText();
	}

	private void OnDestroy()
	{
		LocalizationStore.DelEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void HandleLocalizationChanged()
	{
		SetSaleText();
	}
}

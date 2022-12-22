using UnityEngine;

public class DaterDayLivedLabel : MonoBehaviour
{
	private UILabel myLabel;

	private void Awake()
	{
		myLabel = GetComponent<UILabel>();
	}

	private void SetText()
	{
		myLabel.text = LocalizationStore.Get("Key_1615") + ": " + Storager.getInt("DaterDayLived", false);
	}

	private void OnEnable()
	{
		SetText();
	}

	private void Start()
	{
		LocalizationStore.AddEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void OnDestroy()
	{
		LocalizationStore.DelEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void HandleLocalizationChanged()
	{
		SetText();
	}
}

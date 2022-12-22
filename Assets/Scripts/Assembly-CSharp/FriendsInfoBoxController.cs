using UnityEngine;

public class FriendsInfoBoxController : MonoBehaviour
{
	private enum BoxType
	{
		infoWindow,
		processDataWindow,
		blockClick,
		None
	}

	public UIWidget background;

	[Header("Processing data box")]
	public UIWidget processindDataBoxContainer;

	public UILabel processingDataBoxLabel;

	[Header("Info box")]
	public UIWidget infoBoxContainer;

	public UILabel infoBoxLabel;

	private BoxType _currentTypeBox = BoxType.None;

	private void Start()
	{
		processingDataBoxLabel.text = LocalizationStore.Key_0348;
		LocalizationStore.AddEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void OnDestroy()
	{
		LocalizationStore.DelEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void HandleLocalizationChanged()
	{
		processingDataBoxLabel.text = LocalizationStore.Key_0348;
	}

	public void ShowInfoBox(string text)
	{
		_currentTypeBox = BoxType.infoWindow;
		base.gameObject.SetActive(true);
		processindDataBoxContainer.gameObject.SetActive(false);
		infoBoxLabel.text = text;
		infoBoxContainer.gameObject.SetActive(true);
		background.gameObject.SetActive(true);
	}

	public void ShowProcessingDataBox()
	{
		_currentTypeBox = BoxType.processDataWindow;
		base.gameObject.SetActive(true);
		processindDataBoxContainer.gameObject.SetActive(true);
		infoBoxContainer.gameObject.SetActive(false);
		background.gameObject.SetActive(false);
	}

	public void Hide()
	{
		_currentTypeBox = BoxType.None;
		base.gameObject.SetActive(false);
	}

	public void OnClickExitButton()
	{
		if (_currentTypeBox != BoxType.processDataWindow && _currentTypeBox != BoxType.blockClick)
		{
			Hide();
		}
	}

	public void SetBlockClickState()
	{
		_currentTypeBox = BoxType.blockClick;
		base.gameObject.SetActive(true);
		processindDataBoxContainer.gameObject.SetActive(false);
		infoBoxContainer.gameObject.SetActive(false);
		background.gameObject.SetActive(false);
	}
}

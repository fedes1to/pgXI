using UnityEngine;

public class setupGadgetPanel : MonoBehaviour
{
	public UIInputRilisoft timeLabel;

	public UIInputRilisoft thresholdLabel;

	private void Awake()
	{
		timeLabel.value = TouchPadController.timeGadgetPanel.ToString();
		thresholdLabel.value = TouchPadController.thresholdGadgetPanel.ToString();
	}

	public void timeInputChanged(UIInputRilisoft input)
	{
		try
		{
			TouchPadController.timeGadgetPanel = float.Parse(timeLabel.value);
		}
		catch
		{
		}
	}

	public void thresholdInputChanged(UIInputRilisoft input)
	{
		try
		{
			TouchPadController.thresholdGadgetPanel = int.Parse(thresholdLabel.value);
		}
		catch
		{
		}
	}
}

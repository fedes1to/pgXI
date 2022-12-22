using UnityEngine;

public class SetHeadTextFromOtherLabel : MonoBehaviour
{
	public UILabel otherLabel;

	public UILabel[] headLabels;

	private void Update()
	{
		for (int i = 0; i < headLabels.Length; i++)
		{
			headLabels[i].text = otherLabel.text;
		}
	}
}

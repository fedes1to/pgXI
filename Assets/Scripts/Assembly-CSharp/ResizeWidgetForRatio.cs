using UnityEngine;

public class ResizeWidgetForRatio : MonoBehaviour
{
	[SerializeField]
	private UIWidget[] widgets;

	[SerializeField]
	private Vector2 sizeFor4x3;

	[SerializeField]
	private Vector2 sizeFor16x9;

	public bool isLabels;

	[SerializeField]
	private int fontSizeFor4x3;

	[SerializeField]
	private int fontSizeFor16x9;

	private void Start()
	{
		if (widgets == null)
		{
			return;
		}
		if ((double)((float)Screen.width / (float)Screen.height) > 1.5)
		{
			for (int i = 0; i < widgets.Length; i++)
			{
				widgets[i].width = Mathf.RoundToInt(sizeFor16x9.x);
				widgets[i].height = Mathf.RoundToInt(sizeFor16x9.y);
			}
			if (!isLabels)
			{
				return;
			}
			for (int j = 0; j < widgets.Length; j++)
			{
				if (!(widgets[j].GetComponent<UILabel>() == null))
				{
					widgets[j].GetComponent<UILabel>().fontSize = fontSizeFor16x9;
				}
			}
			return;
		}
		for (int k = 0; k < widgets.Length; k++)
		{
			widgets[k].width = Mathf.RoundToInt(sizeFor4x3.x);
			widgets[k].height = Mathf.RoundToInt(sizeFor4x3.y);
		}
		if (!isLabels)
		{
			return;
		}
		for (int l = 0; l < widgets.Length; l++)
		{
			if (!(widgets[l].GetComponent<UILabel>() == null))
			{
				widgets[l].GetComponent<UILabel>().fontSize = fontSizeFor4x3;
			}
		}
	}
}

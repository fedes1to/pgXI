using UnityEngine;

public class FrameResizer : MonoBehaviour
{
	public GameObject[] objects;

	public UISprite frame;

	public UITable table;

	public Vector2[] frameSize;

	private int activeObjectsCounter;

	public void ResizeFrame()
	{
		activeObjectsCounter = 0;
		for (int i = 0; i < objects.Length; i++)
		{
			if (objects[i].activeSelf)
			{
				activeObjectsCounter++;
			}
		}
		if (activeObjectsCounter > 0)
		{
			frame.width = Mathf.RoundToInt(frameSize[activeObjectsCounter - 1].x);
			frame.height = Mathf.RoundToInt(frameSize[activeObjectsCounter - 1].y);
		}
		if (table != null)
		{
			table.sorting = UITable.Sorting.Alphabetic;
			table.Reposition();
			table.repositionNow = true;
		}
	}
}

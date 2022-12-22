using UnityEngine;

public class ScaleButtonsPanel : MonoBehaviour
{
	private void Start()
	{
		if ((double)((float)Screen.width / (float)Screen.height) > 1.5)
		{
			base.transform.localScale = new Vector3(0.89f, 0.89f, 1f);
		}
	}
}

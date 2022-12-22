using UnityEngine;

public class RefreshAndUpdateAnchors : MonoBehaviour
{
	public UIPanel panel;

	private void Start()
	{
		panel.ResetAndUpdateAnchors();
		panel.Refresh();
	}
}

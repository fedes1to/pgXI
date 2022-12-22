using UnityEngine;

public class CreateClanButton : MonoBehaviour
{
	private void OnClick()
	{
		ButtonClickSound.TryPlayClick();
		NGUITools.GetRoot(base.gameObject).GetComponent<ClansGUIController>().CreateClanPanel.SetActive(true);
	}
}

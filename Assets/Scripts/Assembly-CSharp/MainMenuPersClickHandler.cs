using UnityEngine;

public class MainMenuPersClickHandler : MonoBehaviour
{
	public float DragDistance = 20f;

	private Vector3 _startPos;

	private void OnMouseDown()
	{
		_startPos = Input.mousePosition;
	}

	private void OnMouseUp()
	{
		Vector3 mousePosition = Input.mousePosition;
		if (!(Mathf.Abs(_startPos.magnitude - mousePosition.magnitude) > DragDistance) && (!(MainMenuController.sharedController != null) || !(MainMenuController.sharedController.mainPanel != null) || MainMenuController.sharedController.mainPanel.activeInHierarchy) && !(UICamera.lastHit.collider != null) && TrainingController.TrainingCompleted)
		{
			if (ProfileController.Instance != null)
			{
				ProfileController.Instance.SetStaticticTab(ProfileStatTabType.Multiplayer);
			}
			MainMenuController.sharedController.GoToProfile();
		}
	}
}

using UnityEngine;

public class NewPrivateChatMessage : MonoBehaviour
{
	private GameObject newMessageSprite;

	private void Start()
	{
		newMessageSprite = base.gameObject.transform.GetChild(0).gameObject;
		UpdateStateNewMessage();
	}

	private void UpdateStateNewMessage()
	{
		if (newMessageSprite.activeSelf != ChatController.countNewPrivateMessage > 0)
		{
			newMessageSprite.SetActive(ChatController.countNewPrivateMessage > 0);
		}
	}

	private void Update()
	{
		UpdateStateNewMessage();
	}
}

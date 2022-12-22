using UnityEngine;

public class SetPasswordSprite : MonoBehaviour
{
	public UIInput input;

	public GameObject openSprite;

	public GameObject closeSprite;

	private void Update()
	{
		if (string.IsNullOrEmpty(input.value))
		{
			if (!openSprite.activeSelf)
			{
				openSprite.SetActive(true);
				closeSprite.SetActive(false);
			}
		}
		else if (openSprite.activeSelf)
		{
			openSprite.SetActive(false);
			closeSprite.SetActive(true);
		}
	}
}

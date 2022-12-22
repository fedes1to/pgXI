using UnityEngine;

public class StarReview : MonoBehaviour
{
	[HideInInspector]
	public int numOrderStar;

	public UILabel lbNumStar;

	public GameObject objFonStar;

	public GameObject objActiveStar;

	public void SetActiveStar(bool val)
	{
		if ((bool)objActiveStar)
		{
			objActiveStar.SetActive(val);
		}
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			ReviewHUDWindow.Instance.SelectStar(this);
		}
		else
		{
			ReviewHUDWindow.Instance.SelectStar(null);
		}
	}

	private void OnClick()
	{
		ReviewHUDWindow.Instance.OnClickStarRating();
	}
}

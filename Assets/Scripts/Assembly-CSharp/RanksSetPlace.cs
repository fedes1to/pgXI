using UnityEngine;

public class RanksSetPlace : MonoBehaviour
{
	public UILabel placeLabel;

	public GameObject cupGold;

	public GameObject cupSilver;

	public GameObject cupBronze;

	public bool isShowCups;

	public void SetPlace(int place)
	{
		if (place <= 3 && isShowCups)
		{
			placeLabel.text = string.Empty;
			cupGold.SetActive(place == 1);
			cupSilver.SetActive(place == 2);
			cupBronze.SetActive(place == 3);
		}
		else
		{
			cupGold.SetActive(false);
			cupSilver.SetActive(false);
			cupBronze.SetActive(false);
			placeLabel.text = place.ToString();
		}
	}
}

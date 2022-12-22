using UnityEngine;

public class FlagPedestalController : MonoBehaviour
{
	public GameObject BluePedestal;

	public GameObject RedPedestal;

	public void SetColor(int _color)
	{
		switch (_color)
		{
		case 1:
			BluePedestal.SetActive(true);
			RedPedestal.SetActive(false);
			break;
		case 2:
			BluePedestal.SetActive(false);
			RedPedestal.SetActive(true);
			break;
		default:
			BluePedestal.SetActive(false);
			RedPedestal.SetActive(false);
			break;
		}
	}
}

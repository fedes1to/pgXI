using UnityEngine;

public class PriceContainer : MonoBehaviour
{
	public UITexture background;

	public UILabel priceLabel;

	public GameObject gem;

	public GameObject coin;

	public void SetPrice(ItemPrice price)
	{
		priceLabel.text = price.Price.ToString();
		bool flag = price.Currency.Equals("Coins");
		gem.SetActive(!flag);
		coin.SetActive(flag);
	}
}

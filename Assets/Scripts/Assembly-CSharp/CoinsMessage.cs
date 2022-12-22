using Rilisoft;
using UnityEngine;

public sealed class CoinsMessage : MonoBehaviour
{
	public delegate void CoinsLabelDisappearedDelegate(bool isGems, int count);

	public GUIStyle labelStyle;

	public Rect rect = Tools.SuccessMessageRect();

	public string message = "Purchases restored";

	public Texture texture;

	public int depth = -2;

	public bool singleMessage;

	public Texture youveGotCoin;

	public Texture passNextLevels;

	private int coinsToShow;

	private int coinsForNextLevels;

	private double startTime;

	private float _time = 2f;

	public Texture plashka;

	public static event CoinsLabelDisappearedDelegate CoinsLabelDisappeared;

	public static void FireCoinsAddedEvent(bool isGems = false, int count = 2)
	{
		if (CoinsMessage.CoinsLabelDisappeared != null)
		{
			CoinsMessage.CoinsLabelDisappeared(isGems, count);
		}
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		coinsToShow = Storager.getInt(Defs.EarnedCoins, false);
		Storager.setInt(Defs.EarnedCoins, 0, false);
		if (coinsToShow > 1)
		{
			plashka = Resources.Load<Texture>(ResPath.Combine("CoinsIndicationSystem", "got_prize"));
		}
		else
		{
			plashka = Resources.Load<Texture>(ResPath.Combine("CoinsIndicationSystem", "got_coin"));
		}
		startTime = Time.realtimeSinceStartup;
	}

	private void Remove()
	{
		Object.Destroy(base.gameObject);
	}
}

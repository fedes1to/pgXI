using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

public class TournamentWinnerBannerWindow : BannerWindow
{
	private static readonly PrefsBoolCachedProperty _canShow = new PrefsBoolCachedProperty("TournamentWinnerBannerWindow_needShow");

	[SerializeField]
	private GameObject _coinsIconObj;

	[SerializeField]
	private GameObject _gemsIconObj;

	[SerializeField]
	private TextGroup _RewardCoinsTextGroup;

	[SerializeField]
	private TextGroup _RewardGemsTextGroup;

	public static bool CanShow
	{
		get
		{
			return _canShow.Value;
		}
		set
		{
			_canShow.Value = value;
		}
	}

	public override void Show()
	{
		base.Show();
		if (BalanceController.competitionAward == null)
		{
			return;
		}
		if (BalanceController.competitionAward.Currency == "Coins")
		{
			_RewardCoinsTextGroup.Do(delegate(TextGroup t)
			{
				t.Text = BalanceController.competitionAward.Price.ToString();
			});
			_coinsIconObj.Do(delegate(GameObject o)
			{
				o.SetActiveSafe(true);
			});
			_gemsIconObj.Do(delegate(GameObject o)
			{
				o.SetActiveSafe(false);
			});
		}
		else
		{
			_RewardGemsTextGroup.Do(delegate(TextGroup t)
			{
				t.Text = BalanceController.competitionAward.Price.ToString();
			});
			_coinsIconObj.Do(delegate(GameObject o)
			{
				o.SetActiveSafe(false);
			});
			_gemsIconObj.Do(delegate(GameObject o)
			{
				o.SetActiveSafe(true);
			});
		}
	}

	public void HideButtonAction()
	{
		if (BalanceController.competitionAward != null)
		{
			if (BalanceController.competitionAward.Currency == "Coins")
			{
				BankController.AddCoins(BalanceController.competitionAward.Price);
			}
			else
			{
				BankController.AddGems(BalanceController.competitionAward.Price);
			}
		}
		StartCoroutine(BankController.WaitForIndicationGems(true));
		CanShow = false;
		if (BannerWindowController.SharedController != null)
		{
			BannerWindowController.SharedController.ResetStateBannerShowed(BannerWindowType.TournamentWunner);
			BannerWindowController.SharedController.HideBannerWindow();
		}
	}
}

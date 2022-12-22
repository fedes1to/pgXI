using UnityEngine;

namespace Rilisoft
{
	public class LeaderboardTournamentFooter : MonoBehaviour
	{
		[SerializeField]
		private UILabel _textLabel;

		[SerializeField]
		private UILabel _countLabel;

		[SerializeField]
		private GameObject _coinObject;

		[SerializeField]
		private GameObject _gemObject;

		private void OnEnable()
		{
			_textLabel.text = string.Format(LocalizationStore.Get("Key_2813"), BalanceController.countPlaceAwardInCompetion);
			if (_textLabel.text.Length > 47)
			{
				_textLabel.overflowMethod = UILabel.Overflow.ShrinkContent;
				_textLabel.width = 690;
			}
			if (BalanceController.competitionAward != null)
			{
				_countLabel.text = BalanceController.competitionAward.Price + "!";
				if (BalanceController.competitionAward.Currency == "Coins")
				{
					_coinObject.SetActiveSafe(true);
					_gemObject.SetActiveSafe(false);
				}
				else
				{
					_coinObject.SetActiveSafe(false);
					_gemObject.SetActiveSafe(true);
				}
			}
		}
	}
}

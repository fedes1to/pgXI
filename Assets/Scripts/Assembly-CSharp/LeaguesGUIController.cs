using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public class LeaguesGUIController : MonoBehaviour
{
	[SerializeField]
	private UICenterOnChild _centerOnChild;

	[SerializeField]
	private UILabel _lblLeagueName;

	[SerializeField]
	private UILabel _lblLeagueNameOutline;

	[SerializeField]
	private UILabel _lblScore;

	[SerializeField]
	private UISprite _sprScoreBar;

	[SerializeField]
	private GameObject _progressGO;

	[SerializeField]
	private UILabel _progressTextLabel;

	[SerializeField]
	[ReadOnly]
	private List<ProfileCup> _cups;

	[ReadOnly]
	[SerializeField]
	private LeagueItemsView _itemsView;

	private ProfileCup _selectedCup;

	private readonly Dictionary<RatingSystem.RatingLeague, string> _leaguesLKeys = new Dictionary<RatingSystem.RatingLeague, string>(6, RatingSystem.RatingLeagueComparer.Instance)
	{
		{
			RatingSystem.RatingLeague.Wood,
			"Key_1953"
		},
		{
			RatingSystem.RatingLeague.Steel,
			"Key_1954"
		},
		{
			RatingSystem.RatingLeague.Gold,
			"Key_1955"
		},
		{
			RatingSystem.RatingLeague.Crystal,
			"Key_1956"
		},
		{
			RatingSystem.RatingLeague.Ruby,
			"Key_1957"
		},
		{
			RatingSystem.RatingLeague.Adamant,
			"Key_1958"
		}
	};

	private void OnEnable()
	{
		_cups = GetComponentsInChildren<ProfileCup>(true).ToList();
		_itemsView = GetComponentInChildren<LeagueItemsView>(true);
		Reposition();
	}

	private void Reposition()
	{
		_selectedCup = _cups.FirstOrDefault((ProfileCup c) => c.League == RatingSystem.instance.currentLeague);
		StartCoroutine(PositionToCurrentLeagueCoroutine());
	}

	private IEnumerator PositionToCurrentLeagueCoroutine()
	{
		yield return null;
		ProfileCup to = _cups.FirstOrDefault((ProfileCup c) => c.League == RatingSystem.instance.currentLeague);
		_centerOnChild.CenterOn(to.gameObject.transform);
		yield return null;
		SetInfoFromLeague(to.League);
	}

	public void CupCentered(ProfileCup cup)
	{
		_selectedCup = cup;
		SetInfoFromLeague(cup.League);
		_itemsView.Repaint(cup.League);
	}

	private void SetInfoFromLeague(RatingSystem.RatingLeague league)
	{
		string text = LocalizationStore.Get(_leaguesLKeys[league]);
		_lblLeagueName.text = text;
		_lblLeagueNameOutline.text = text;
		if (league < RatingSystem.instance.currentLeague)
		{
			_progressGO.SetActive(false);
			_progressTextLabel.gameObject.SetActive(true);
			_progressTextLabel.text = LocalizationStore.Get("Key_2173");
		}
		else if (league > RatingSystem.instance.currentLeague)
		{
			_progressGO.SetActive(false);
			_progressTextLabel.gameObject.SetActive(true);
			int num = RatingSystem.instance.MaxRatingInLeague(league - 1) - RatingSystem.instance.currentRating;
			_progressTextLabel.text = string.Format(LocalizationStore.Get("Key_2172"), num);
		}
		else if (league == (RatingSystem.RatingLeague)RiliExtensions.EnumNumbers<RatingSystem.RatingLeague>().Max())
		{
			_progressGO.SetActive(false);
			_progressTextLabel.gameObject.SetActive(true);
			_progressTextLabel.text = LocalizationStore.Get("Key_2249");
		}
		else
		{
			_progressGO.SetActive(true);
			_progressTextLabel.gameObject.SetActive(false);
			int num2 = RatingSystem.instance.MaxRatingInLeague(league);
			_lblScore.text = string.Format("{0}/{1}", RatingSystem.instance.currentRating, num2);
			_sprScoreBar.fillAmount = (float)RatingSystem.instance.currentRating / (float)num2;
		}
	}
}

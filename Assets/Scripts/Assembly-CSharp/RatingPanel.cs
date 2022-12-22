using System;
using Rilisoft;
using UnityEngine;

public class RatingPanel : MonoBehaviour
{
	public GameObject leaguePanel;

	public UISprite cup;

	public UILabel leagueLabel;

	public UILabel ratingLabel;

	[SerializeField]
	private ButtonHandler _btnOpenProfile;

	private void OnEnable()
	{
		UpdateInfo();
		RatingSystem.OnRatingUpdate += UpdateInfo;
	}

	private void UpdateInfo()
	{
		if (!TrainingController.TrainingCompleted)
		{
			leaguePanel.SetActive(false);
			return;
		}
		leaguePanel.SetActive(true);
		cup.spriteName = RatingSystem.instance.currentLeague.ToString() + " " + (3 - RatingSystem.instance.currentDivision);
		if (RatingSystem.instance.currentLeague != RatingSystem.RatingLeague.Adamant)
		{
			leagueLabel.text = LocalizationStore.Get(RatingSystem.leagueLocalizations[(int)RatingSystem.instance.currentLeague]) + " " + RatingSystem.divisionByIndex[RatingSystem.instance.currentDivision];
		}
		else
		{
			leagueLabel.text = LocalizationStore.Get(RatingSystem.leagueLocalizations[(int)RatingSystem.instance.currentLeague]);
		}
		ratingLabel.text = RatingSystem.instance.currentRating.ToString();
		if (_btnOpenProfile != null)
		{
			_btnOpenProfile.Clicked += OnBtnOpenProfileClicked;
		}
	}

	private void OnDisable()
	{
		if (_btnOpenProfile != null)
		{
			_btnOpenProfile.Clicked -= OnBtnOpenProfileClicked;
		}
		RatingSystem.OnRatingUpdate -= UpdateInfo;
	}

	private void OnBtnOpenProfileClicked(object sender, EventArgs e)
	{
		if (MainMenuController.sharedController != null)
		{
			MainMenuController.sharedController.GoToProfile();
		}
		if (ProfileController.Instance != null)
		{
			ProfileController.Instance.SetStaticticTab(ProfileStatTabType.Leagues);
		}
	}
}

using System.Collections;
using UnityEngine;

public class RaitingPanelController : MonoBehaviour
{
	[SerializeField]
	private Transform oldRaiting;

	[SerializeField]
	private Transform newRating;

	[SerializeField]
	private UILabel raitingLabel;

	private string raitingText;

	private int maxRating
	{
		get
		{
			return RatingSystem.instance.MaxRatingInLeague(RatingSystem.instance.currentLeague);
		}
	}

	private int league
	{
		get
		{
			return (int)RatingSystem.instance.currentLeague;
		}
	}

	private void Start()
	{
		RatingSystem.OnRatingUpdate += OnRatingUpdated;
		if (league != 5)
		{
			raitingText = string.Format("{0}/{1}", RatingSystem.instance.currentRating, maxRating);
			oldRaiting.localScale = new Vector3((float)RatingSystem.instance.currentRating / (float)maxRating, 1f, 1f);
			newRating.localScale = new Vector3((float)RatingSystem.instance.currentRating / (float)maxRating, 1f, 1f);
		}
		else
		{
			raitingText = RatingSystem.instance.currentRating.ToString();
			oldRaiting.localScale = Vector3.one;
		}
		raitingLabel.text = raitingText;
	}

	private void OnEnable()
	{
		if (league != 5)
		{
			raitingText = string.Format("{0}/{1}", RatingSystem.instance.currentRating, maxRating);
			oldRaiting.localScale = new Vector3((float)RatingSystem.instance.currentRating / (float)maxRating, 1f, 1f);
			newRating.localScale = new Vector3((float)RatingSystem.instance.currentRating / (float)maxRating, 1f, 1f);
		}
		else
		{
			raitingText = RatingSystem.instance.currentRating.ToString();
			oldRaiting.localScale = Vector3.one;
		}
		raitingLabel.text = raitingText;
	}

	private void OnRatingUpdated()
	{
		if (league != 5)
		{
			raitingText = string.Format("{0}/{1}", RatingSystem.instance.currentRating, maxRating);
			newRating.localScale = new Vector3((float)RatingSystem.instance.currentRating / (float)maxRating, 1f, 1f);
			CoroutineRunner.Instance.StartCoroutine(AnimateRaitingPanel());
		}
		else
		{
			raitingText = RatingSystem.instance.currentRating.ToString();
			oldRaiting.localScale = Vector3.one;
		}
		raitingLabel.text = raitingText;
	}

	private void OnDestroy()
	{
		RatingSystem.OnRatingUpdate -= OnRatingUpdated;
	}

	private IEnumerator AnimateRaitingPanel()
	{
		for (int i = 0; i != 4; i++)
		{
			newRating.gameObject.SetActive(false);
			yield return new WaitForSeconds(0.15f);
			newRating.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.15f);
		}
		yield return new WaitForSeconds(0.15f);
		oldRaiting.localScale = new Vector3((float)RatingSystem.instance.currentRating / (float)maxRating, 1f, 1f);
	}
}

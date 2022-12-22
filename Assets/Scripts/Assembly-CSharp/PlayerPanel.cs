using System;
using System.Collections;
using UnityEngine;

public sealed class PlayerPanel : MonoBehaviour
{
	public static PlayerPanel instance;

	[SerializeField]
	private UILabel raitingLabel;

	private string raitingText;

	[SerializeField]
	public UILabel experienceLabel;

	[SerializeField]
	public UISprite currentExp;

	[SerializeField]
	public UISprite oldExp;

	[SerializeField]
	public UISprite rankSprite;

	private int curentExp;

	private int currentLevel = 1;

	[SerializeField]
	private UILabel playerName;

	[SerializeField]
	private UITexture clanIcon;

	[SerializeField]
	private UILabel clanName;

	private Vector3 playerNameStartPos;

	private GameObject panelContainer;

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

	public string ExperienceLabel
	{
		get
		{
			return (!(experienceLabel != null)) ? string.Empty : experienceLabel.text;
		}
		set
		{
			if (experienceLabel != null)
			{
				experienceLabel.text = value ?? string.Empty;
			}
		}
	}

	public float CurrentProgress
	{
		get
		{
			return (!(currentExp != null)) ? 0f : currentExp.transform.localScale.x;
		}
		set
		{
			if (currentExp != null)
			{
				currentExp.transform.localScale = new Vector3(Mathf.Clamp(value, 0f, 1f), 1f, 1f);
			}
		}
	}

	public float OldProgress
	{
		get
		{
			return (!(oldExp != null)) ? 0f : oldExp.transform.localScale.x;
		}
		set
		{
			if (oldExp != null)
			{
				oldExp.transform.localScale = new Vector3(Mathf.Clamp(value, 0f, 1f), base.transform.localScale.y, base.transform.localScale.z);
			}
		}
	}

	public int RankSprite
	{
		get
		{
			return currentLevel;
		}
		set
		{
			if (rankSprite != null)
			{
				string spriteName = string.Format("Rank_{0}", value);
				rankSprite.spriteName = spriteName;
				currentLevel = value;
			}
		}
	}

	private void Awake()
	{
		instance = this;
		RatingSystem.OnRatingUpdate += OnRatingUpdated;
		playerNameStartPos = playerName.transform.localPosition;
		panelContainer = base.transform.GetChild(0).gameObject;
		panelContainer.SetActive(AskNameManager.isComplete);
	}

	private void OnEnable()
	{
		if (FriendsController.sharedController.clanName != string.Empty)
		{
			string text = FriendsController.sharedController.clanName;
			clanName.text = text;
			int num = 0;
			while (clanName.width > 168)
			{
				num++;
				clanName.text = text.Remove(text.Length - num);
				clanName.ProcessText();
			}
			byte[] data = Convert.FromBase64String(FriendsController.sharedController.clanLogo);
			Texture2D texture2D = new Texture2D(Defs.LogoWidth, Defs.LogoWidth);
			texture2D.LoadImage(data);
			texture2D.filterMode = FilterMode.Point;
			texture2D.Apply();
			clanIcon.mainTexture = texture2D;
			playerName.transform.localPosition = playerNameStartPos;
		}
		else
		{
			clanName.enabled = false;
			clanIcon.enabled = false;
			playerName.transform.localPosition = playerNameStartPos - Vector3.down * -16f;
		}
		UpdateNickPlayer();
		UpdateRating();
		UpdateExp();
	}

	public void UpdateRating()
	{
		raitingText = RatingSystem.instance.currentRating.ToString();
		raitingLabel.text = raitingText;
	}

	public void UpdateNickPlayer()
	{
		string text = FilterBadWorld.FilterString(ProfileController.GetPlayerNameOrDefault());
		playerName.text = text;
	}

	public void UpdateExp()
	{
		int num = ExperienceController.sharedController.currentLevel;
		curentExp = ExperienceController.sharedController.CurrentExperience;
		int num2 = ExperienceController.MaxExpLevelsDefault[num];
		RankSprite = num;
		if (num != 31)
		{
			OldProgress = (float)curentExp / (float)num2;
			CurrentProgress = (float)curentExp / (float)num2;
			ExperienceLabel = curentExp + "/" + num2;
		}
		else
		{
			ExperienceLabel = LocalizationStore.Get("Key_0928");
			CurrentProgress = 1f;
		}
	}

	private void OnDisable()
	{
		oldExp.enabled = true;
		oldExp.transform.localScale = currentExp.transform.localScale;
		StopAllCoroutines();
	}

	private void Update()
	{
		if (curentExp != ExperienceController.sharedController.CurrentExperience || currentLevel != ExperienceController.sharedController.currentLevel)
		{
			OnExpUpdate();
		}
		if (panelContainer.activeSelf != AskNameManager.isComplete)
		{
			panelContainer.SetActive(AskNameManager.isComplete);
		}
	}

	private void OnExpUpdate()
	{
		int num = ExperienceController.sharedController.currentLevel;
		curentExp = ExperienceController.sharedController.CurrentExperience;
		int num2 = ExperienceController.MaxExpLevelsDefault[num];
		RankSprite = num;
		if (num != 31)
		{
			CurrentProgress = (float)curentExp / (float)num2;
			ExperienceLabel = curentExp + "/" + num2;
			if (oldExp.transform.localScale.x > currentExp.transform.localScale.x)
			{
				oldExp.transform.localScale = Vector3.zero;
			}
			StartCoroutine(StartExpAnim());
		}
		else
		{
			ExperienceLabel = LocalizationStore.Get("Key_0928");
			CurrentProgress = 1f;
		}
	}

	private IEnumerator StartExpAnim()
	{
		for (int i = 0; i != 4; i++)
		{
			currentExp.enabled = false;
			yield return new WaitForSeconds(0.15f);
			currentExp.enabled = true;
			yield return new WaitForSeconds(0.15f);
		}
		yield return null;
		oldExp.transform.localScale = currentExp.transform.localScale;
	}

	public void HandleOpenProfile()
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

	private void OnRatingUpdated()
	{
		raitingText = RatingSystem.instance.currentRating.ToString();
		raitingLabel.text = raitingText;
	}

	private void OnDestroy()
	{
		instance = null;
		RatingSystem.OnRatingUpdate -= OnRatingUpdated;
	}
}

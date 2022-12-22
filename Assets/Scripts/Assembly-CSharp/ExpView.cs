using System;
using System.Collections;
using Rilisoft;
using UnityEngine;

public sealed class ExpView : MonoBehaviour
{
	public GameObject rankIndicatorContainer;

	public UIRoot interfaceHolder;

	public Camera experienceCamera;

	public UISprite experienceFrame;

	public UILabel experienceLabel;

	public UISprite currentProgress;

	public UISprite oldProgress;

	public UISprite rankSprite;

	[SerializeField]
	private PrefabHandler _levelUpPanelPrefab;

	[SerializeField]
	private PrefabHandler _levelUpPanelTierPrefab;

	public GameObject objHUD;

	[SerializeField]
	private GameObject _levelUpPanelsContainer;

	private LazyObject<LevelUpWithOffers> _levelUpPanelValue;

	private LazyObject<LevelUpWithOffers> _levelUpPanelTierValue;

	private LevelUpWithOffers _currentLevelUpPanel;

	private LeveUpPanelShowOptions _levelUpPanelOptions;

	public bool LevelUpPanelOpened
	{
		get
		{
			return _levelUpPanel.ObjectIsActive || _levelUpPanelTier.ObjectIsActive;
		}
	}

	public LevelUpWithOffers CurrentVisiblePanel
	{
		get
		{
			if (_levelUpPanel.ObjectIsActive)
			{
				return _levelUpPanel.Value;
			}
			if (_levelUpPanelTier.ObjectIsActive)
			{
				return _levelUpPanelTier.Value;
			}
			return null;
		}
	}

	public LazyObject<LevelUpWithOffers> _levelUpPanel
	{
		get
		{
			if (_levelUpPanelValue == null)
			{
				_levelUpPanelValue = new LazyObject<LevelUpWithOffers>(_levelUpPanelPrefab.ResourcePath, _levelUpPanelsContainer);
			}
			return _levelUpPanelValue;
		}
	}

	public LazyObject<LevelUpWithOffers> _levelUpPanelTier
	{
		get
		{
			if (_levelUpPanelTierValue == null)
			{
				_levelUpPanelTierValue = new LazyObject<LevelUpWithOffers>(_levelUpPanelTierPrefab.ResourcePath, _levelUpPanelsContainer);
			}
			return _levelUpPanelTierValue;
		}
	}

	public bool VisibleHUD
	{
		get
		{
			return objHUD.activeSelf;
		}
		set
		{
			objHUD.SetActive(value);
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
			return (!(currentProgress != null)) ? 0f : currentProgress.transform.localScale.x;
		}
		set
		{
			if (currentProgress != null)
			{
				currentProgress.transform.localScale = new Vector3(Mathf.Clamp(value, 0f, 1f), base.transform.localScale.y, base.transform.localScale.z);
			}
		}
	}

	public float OldProgress
	{
		get
		{
			return (!(oldProgress != null)) ? 0f : oldProgress.transform.localScale.x;
		}
		set
		{
			if (oldProgress != null)
			{
				oldProgress.transform.localScale = new Vector3(Mathf.Clamp(value, 0f, 1f), base.transform.localScale.y, base.transform.localScale.z);
			}
		}
	}

	public int RankSprite
	{
		get
		{
			if (rankSprite == null)
			{
				return 1;
			}
			string s = rankSprite.spriteName.Replace("Rank_", string.Empty);
			int result = 0;
			return (!int.TryParse(s, out result)) ? 1 : result;
		}
		set
		{
			if (rankSprite != null)
			{
				string spriteName = string.Format("Rank_{0}", value);
				rankSprite.spriteName = spriteName;
			}
		}
	}

	public LeveUpPanelShowOptions LevelUpPanelOptions
	{
		get
		{
			if (_levelUpPanelOptions == null)
			{
				_levelUpPanelOptions = new LeveUpPanelShowOptions();
			}
			return _levelUpPanelOptions;
		}
	}

	private void Awake()
	{
		Singleton<SceneLoader>.Instance.OnSceneLoading += delegate
		{
			_levelUpPanel.DestroyValue();
			_levelUpPanelTier.DestroyValue();
		};
	}

	public void ShowLevelUpPanel()
	{
		_currentLevelUpPanel = ((!LevelUpPanelOptions.ShowTierView) ? _levelUpPanel.Value : _levelUpPanelTier.Value);
		_currentLevelUpPanel.SetCurrentRank(LevelUpPanelOptions.CurrentRank.ToString());
		_currentLevelUpPanel.SetRewardPrice("+" + LevelUpPanelOptions.CoinsReward + "\n" + LocalizationStore.Get("Key_0275"));
		_currentLevelUpPanel.SetGemsRewardPrice("+" + LevelUpPanelOptions.GemsReward + "\n" + LocalizationStore.Get("Key_0951"));
		_currentLevelUpPanel.SetAddHealthCount(string.Format(LocalizationStore.Get("Key_1856"), ExperienceController.sharedController.AddHealthOnCurLevel.ToString()));
		_currentLevelUpPanel.SetItems(LevelUpPanelOptions.NewItems);
		_currentLevelUpPanel.shareScript.share.IsChecked = LevelUpPanelOptions.ShareButtonEnabled;
		ExpController.ShowTierPanel(_currentLevelUpPanel.gameObject);
	}

	public void ToBonus(int starterGemsReward, int starterCoinsReward)
	{
		if (_currentLevelUpPanel != null)
		{
			_currentLevelUpPanel.SetStarterBankValues(starterGemsReward, starterCoinsReward);
			_currentLevelUpPanel.shareScript.animatorLevel.SetTrigger("Bonus");
		}
	}

	public void StopAnimation()
	{
		if (currentProgress.gameObject.activeInHierarchy)
		{
			currentProgress.StopAllCoroutines();
		}
		if (oldProgress != null && oldProgress.gameObject.activeInHierarchy)
		{
			oldProgress.StopAllCoroutines();
			oldProgress.enabled = true;
			oldProgress.transform.localScale = currentProgress.transform.localScale;
		}
	}

	public IDisposable StartBlinkingWithNewProgress()
	{
		if (currentProgress == null || !currentProgress.gameObject.activeInHierarchy)
		{
			Debug.LogWarning("(currentProgress == null || !currentProgress.gameObject.activeInHierarchy)");
			return new ActionDisposable(delegate
			{
			});
		}
		currentProgress.StopAllCoroutines();
		IEnumerator c = StartBlinkingCoroutine();
		currentProgress.StartCoroutine(c);
		return new ActionDisposable(delegate
		{
			currentProgress.StopCoroutine(c);
			if (currentProgress != null)
			{
				currentProgress.enabled = true;
			}
		});
	}

	public void WaitAndUpdateOldProgress(AudioClip sound)
	{
		if (!(oldProgress == null) && oldProgress.gameObject.activeInHierarchy)
		{
			oldProgress.StopAllCoroutines();
			oldProgress.StartCoroutine(WaitAndUpdateCoroutine(sound));
		}
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		oldProgress.enabled = true;
		oldProgress.transform.localScale = currentProgress.transform.localScale;
		if (currentProgress != null && currentProgress.gameObject.activeInHierarchy)
		{
			currentProgress.StopAllCoroutines();
		}
	}

	private IEnumerator StartBlinkingCoroutine()
	{
		for (int i = 0; i != 4; i++)
		{
			currentProgress.enabled = false;
			yield return new WaitForSeconds(0.15f);
			currentProgress.enabled = true;
			yield return new WaitForSeconds(0.15f);
		}
	}

	private IEnumerator WaitAndUpdateCoroutine(AudioClip sound)
	{
		yield return new WaitForSeconds(1.2f);
		if (currentProgress != null)
		{
			oldProgress.transform.localScale = currentProgress.transform.localScale;
		}
		if (Defs.isSoundFX)
		{
			NGUITools.PlaySound(sound);
		}
	}
}

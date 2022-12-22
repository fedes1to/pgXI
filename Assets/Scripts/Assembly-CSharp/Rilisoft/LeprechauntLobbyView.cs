using System.Collections;
using UnityEngine;

namespace Rilisoft
{
	public class LeprechauntLobbyView : MonoBehaviour
	{
		private const string AnimBoolRewardAvailable = "RewardAvailable";

		private const string AnimTriggerGetReward = "GetReward";

		private const string AnimTriggerTap = "Tap";

		public static LeprechauntLobbyView Instance;

		[SerializeField]
		private GameObject _model;

		[SerializeField]
		private Animator _animator;

		private NickLabelController _nickLabelValue;

		private bool _waitEndRewardAnimation;

		private NickLabelController NickLabel
		{
			get
			{
				if (_nickLabelValue == null)
				{
					if (NickLabelStack.sharedStack != null)
					{
						_nickLabelValue = NickLabelStack.sharedStack.GetNextCurrentLabel();
					}
					if (_nickLabelValue != null)
					{
						_nickLabelValue.StartShow(NickLabelController.TypeNickLabel.Leprechaunt, _model.transform);
					}
				}
				return _nickLabelValue;
			}
		}

		public bool CanShow()
		{
			return (!(Singleton<LeprechauntManager>.Instance != null) || Singleton<LeprechauntManager>.Instance.CurrentTime.HasValue) && TrainingController.TrainingCompleted && ExperienceController.sharedController.currentLevel >= 2 && (!(MainMenuController.sharedController != null) || (!MainMenuController.sharedController.SettingsJoysticksPanel.activeInHierarchy && !MainMenuController.sharedController.settingsPanel.activeInHierarchy && !MainMenuController.sharedController.FreePanelIsActive && !MainMenuController.sharedController.singleModePanel.activeInHierarchy)) && (!(FeedbackMenuController.Instance != null) || !FeedbackMenuController.Instance.gameObject.activeInHierarchy);
		}

		private void Awake()
		{
			Instance = this;
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			_waitEndRewardAnimation = false;
		}

		private void OnDestroy()
		{
			_waitEndRewardAnimation = false;
			Instance = null;
		}

		private void OnEnable()
		{
			_waitEndRewardAnimation = false;
			_model.SetActive(Singleton<LeprechauntManager>.Instance.LeprechauntExists);
			StartCoroutine(WaitMainMenuAndBindBillboard());
			StartCoroutine(MainLoopCoroutine());
		}

		private void Update()
		{
			if (_animator.gameObject.activeInHierarchy && _animator.GetBool("RewardAvailable") != Singleton<LeprechauntManager>.Instance.RewardIsReadyToDrop)
			{
				_animator.SetBool("RewardAvailable", Singleton<LeprechauntManager>.Instance.RewardIsReadyToDrop);
			}
		}

		private IEnumerator WaitMainMenuAndBindBillboard()
		{
			while (MainMenuController.sharedController == null)
			{
				yield return null;
			}
			MainMenuController.sharedController.LeprechauntBindedBillboard.BindTo(() => _model.transform);
		}

		private IEnumerator MainLoopCoroutine()
		{
			while (true)
			{
				if (!CanShow())
				{
					_model.SetActiveSafe(false);
					NickLabel.LeprechauntGO.SetActiveSafe(false);
				}
				else
				{
					_model.SetActiveSafe(Singleton<LeprechauntManager>.Instance.LeprechauntExists);
					if (Singleton<LeprechauntManager>.Instance.LeprechauntExists && !_waitEndRewardAnimation)
					{
						NickLabel.LeprechauntGO.SetActiveSafe(true);
						if (Singleton<LeprechauntManager>.Instance.LeprechauntLifeTimeLeft.HasValue)
						{
							NickLabel.LeprechauntDaysLeft.gameObject.SetActiveSafe(true);
							int daysLeft = Singleton<LeprechauntManager>.Instance.LeprechauntLifeTimeLeft.Value / 3600 / 24 + 1;
							NickLabel.LeprechauntDaysLeft.text = string.Format(LocalizationStore.Get("Key_2913"), daysLeft);
						}
						if (Singleton<LeprechauntManager>.Instance.RewardIsReadyToDrop)
						{
							NickLabel.LeprechauntGemsRewardAvailableGO.SetActiveSafe(true);
							NickLabel.LeprechauntRewardTimeLeft.gameObject.SetActiveSafe(false);
							NickLabel.LeprechauntGemsRewardAvailable.text = string.Format(LocalizationStore.Get("Key_2914"), Singleton<LeprechauntManager>.Instance.RewardReadyToDrop);
							if (Singleton<LeprechauntManager>.Instance.RewardCurrency == "GemsCurrency")
							{
								NickLabel.LeprechauntGemsIcon.SetActiveSafe(true);
								NickLabel.LeprechauntCoinsIcon.SetActiveSafe(false);
							}
							else
							{
								NickLabel.LeprechauntGemsIcon.SetActiveSafe(false);
								NickLabel.LeprechauntCoinsIcon.SetActiveSafe(true);
							}
						}
						else
						{
							NickLabel.LeprechauntGemsRewardAvailableGO.SetActiveSafe(false);
							NickLabel.LeprechauntRewardTimeLeft.gameObject.SetActiveSafe(true);
							NickLabel.LeprechauntRewardTimeLeft.text = RiliExtensions.GetTimeString((long)Singleton<LeprechauntManager>.Instance.RewardDropSecsLeft.Value);
						}
					}
					else
					{
						NickLabel.LeprechauntDaysLeft.gameObject.SetActiveSafe(false);
						NickLabel.LeprechauntRewardTimeLeft.gameObject.SetActiveSafe(false);
						NickLabel.LeprechauntGemsRewardAvailableGO.SetActiveSafe(false);
					}
				}
				yield return new WaitForRealSeconds(0.2f);
			}
		}

		public void Tap()
		{
			if (!_waitEndRewardAnimation)
			{
				if (Singleton<LeprechauntManager>.Instance.RewardIsReadyToDrop)
				{
					_waitEndRewardAnimation = true;
					StopCoroutine("MainLoopCoroutine");
					NickLabel.LeprechauntGO.SetActiveSafe(false);
					_animator.SetTrigger("GetReward");
				}
				else
				{
					_animator.SetTrigger("Tap");
				}
			}
		}

		public void OnAnimatorStateExit(string stateName)
		{
			if (stateName == "Close")
			{
				_waitEndRewardAnimation = false;
				_animator.SetBool("RewardAvailable", false);
				Singleton<LeprechauntManager>.Instance.DropReward();
				StartCoroutine(MainLoopCoroutine());
			}
		}
	}
}

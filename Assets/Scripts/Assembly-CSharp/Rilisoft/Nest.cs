using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft.NullExtensions;
using UnityEngine;

namespace Rilisoft
{
	public class Nest : MonoBehaviour
	{
		private const string KEY_DROPPED_EGGS_COUNTER = "nest_dropped_eggs_counter";

		private const string KEY_START_WAIT_AT = "nest_start_wait_at";

		private const string AP_IS_ENABLED = "IsEnabled";

		private const string AP_IS_OPEN = "IsOpen";

		public static Nest Instance;

		public static List<long> timerIntervalDelays = new List<long>
		{
			0L, 900L, 900L, 1800L, 1800L, 3600L, 3600L, 7200L, 7200L, 14400L,
			14400L, 21600L, 21600L, 43200L
		};

		private SaltedLong _startWaitTime = new SaltedLong(187649984473770L);

		private SaltedInt _dropCounter = new SaltedInt(178956970, -1);

		[SerializeField]
		private GameObject NestGO;

		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private AnimationHandler _animationHandler;

		[SerializeField]
		private GameObject _bannerPrefab;

		private LazyObject<NestBanner> _banner;

		private bool _getEggProcessed;

		private NickLabelController _nickLabelControllerValue;

		private Vector3 _prevPos = Vector3.zero;

		private bool _nickLabelControllerVisible = true;

		public static long CurrentTime
		{
			get
			{
				return FriendsController.ServerTime;
			}
		}

		private long TimerInterval
		{
			get
			{
				return (timerIntervalDelays.Count <= DropCounter) ? timerIntervalDelays.Last() : timerIntervalDelays[DropCounter];
			}
		}

		private int DropCounter
		{
			get
			{
				if (_dropCounter.Value < timerIntervalDelays.Count - 1 && PlayerPrefs.GetInt("nest_first_egg_getted", 0) > 0)
				{
					_dropCounter = timerIntervalDelays.Count - 1;
					PlayerPrefs.SetInt("nest_dropped_eggs_counter", _dropCounter.Value);
				}
				if (_dropCounter.Value < 0)
				{
					_dropCounter = PlayerPrefs.GetInt("nest_dropped_eggs_counter", 0);
				}
				return _dropCounter.Value;
			}
			set
			{
				_dropCounter = value;
				PlayerPrefs.SetInt("nest_dropped_eggs_counter", _dropCounter.Value);
			}
		}

		public bool BannerIsVisible
		{
			get
			{
				return _banner != null && _banner.HasValue && _banner.Value.IsVisible;
			}
		}

		internal bool EggIsReady
		{
			get
			{
				return DropCounter == 0 || (CurrentTime > 0 && _startWaitTime.Value > 0 && _startWaitTime.Value + TimerInterval <= CurrentTime);
			}
		}

		public long? TimeLeft
		{
			get
			{
				if (CurrentTime < 1 || _startWaitTime.Value < 1)
				{
					return null;
				}
				return _startWaitTime.Value + TimerInterval - CurrentTime;
			}
		}

		private NickLabelController _nickLabelController
		{
			get
			{
				if (_nickLabelControllerValue == null)
				{
					if (NickLabelStack.sharedStack != null)
					{
						_nickLabelControllerValue = NickLabelStack.sharedStack.GetNextCurrentLabel();
					}
					if (_nickLabelControllerValue != null)
					{
						_nickLabelControllerValue.StartShow(NickLabelController.TypeNickLabel.Nest, NestGO.transform);
					}
				}
				return _nickLabelControllerValue;
			}
		}

		public bool NestCanShow()
		{
			return CurrentTime >= 1 && TrainingController.TrainingCompleted && ExperienceController.sharedController.currentLevel >= 2 && (!(MainMenuController.sharedController != null) || (!MainMenuController.sharedController.SettingsJoysticksPanel.activeInHierarchy && !MainMenuController.sharedController.settingsPanel.activeInHierarchy && !MainMenuController.sharedController.FreePanelIsActive && !MainMenuController.sharedController.singleModePanel.activeInHierarchy)) && (!(FeedbackMenuController.Instance != null) || !FeedbackMenuController.Instance.gameObject.activeInHierarchy);
		}

		private void Awake()
		{
			Instance = this;
			StartCoroutine(WaitMainMenu());
			_startWaitTime.Value = (long)PlayerPrefs.GetFloat("nest_start_wait_at");
			_animationHandler.OnAnimationEvent += _animationHandler_OnAnimationEvent;
		}

		private IEnumerator WaitMainMenu()
		{
			while (MainMenuController.sharedController == null)
			{
				yield return null;
			}
			_banner = new LazyObject<NestBanner>(_bannerPrefab, MainMenuController.sharedController.gameObject);
			MainMenuController.sharedController.EggChestBindedBillboard.BindTo(() => NestGO.transform);
		}

		private void Update()
		{
			if (!NestCanShow())
			{
				NestGO.SetActiveSafe(false);
				if (_nickLabelController != null)
				{
					_nickLabelController.gameObject.SetActiveSafe(false);
				}
				if (_banner != null && _banner.HasValue)
				{
					_banner.Value.EnableTouchBlocker(false);
				}
				return;
			}
			NestGO.SetActiveSafe(true);
			if (_nickLabelController != null)
			{
				_nickLabelController.gameObject.SetActiveSafe(_nickLabelControllerVisible);
			}
			if (DropCounter == 0)
			{
				_animator.SetBool("IsEnabled", EggIsReady);
				ShowLobbyHeader(false, 0L);
				return;
			}
			if (_startWaitTime.Value < 1)
			{
				ResetTimer();
			}
			_animator.SetBool("IsEnabled", EggIsReady);
			ShowLobbyHeader(true, TimeLeft.Value);
		}

		public void Click()
		{
			if (!(MainMenuController.sharedController != null) || !MainMenuController.sharedController.LeaderboardsIsOpening)
			{
				GetEgg();
			}
		}

		private void GetEgg()
		{
			if (EggIsReady && NestGO.activeInHierarchy && !_getEggProcessed)
			{
				_getEggProcessed = true;
				SetMenuInteractionEnabled(false);
				_animator.SetBool("IsOpen", true);
			}
		}

		private void _animationHandler_OnAnimationEvent(string animName, AnimationHandler.AnimState animState)
		{
			if (animName == "Open" && animState == AnimationHandler.AnimState.Finished && _getEggProcessed)
			{
				DropEgg();
			}
		}

		private void DropEgg()
		{
			if (!EggIsReady)
			{
				OnBannerClose();
				return;
			}
			Egg egg = null;
			if (DropCounter == 0)
			{
				EggData data = Singleton<EggsManager>.Instance.GetAllEggs().FirstOrDefault((EggData e) => e.Id == "egg_simple_rating");
				egg = Singleton<EggsManager>.Instance.AddEgg(data);
			}
			else
			{
				egg = Singleton<EggsManager>.Instance.AddRandomEgg();
			}
			if (egg != null && egg.Data != null)
			{
				AnalyticsStuff.LogEggDelivery(egg.Data.Id);
			}
			ResetTimer();
			_banner.Value.OnClose += OnBannerClose;
			_banner.Value.Show(egg);
		}

		private void OnBannerClose()
		{
			_banner.Value.OnClose -= OnBannerClose;
			_animator.SetBool("IsOpen", false);
			_animator.SetBool("IsEnabled", false);
			SetMenuInteractionEnabled(true);
			_getEggProcessed = false;
		}

		private void ResetTimer()
		{
			if (CurrentTime >= 1)
			{
				_startWaitTime.Value = CurrentTime;
				PlayerPrefs.SetFloat("nest_start_wait_at", _startWaitTime.Value);
				DropCounter++;
			}
		}

		private void SetMenuInteractionEnabled(bool enabled)
		{
			if (!enabled)
			{
				_banner.Value.EnableTouchBlocker(true);
				if (FreeAwardShowHandler.Instance != null)
				{
					FreeAwardShowHandler.Instance.IsInteractable = false;
				}
			}
			else
			{
				_banner.Value.EnableTouchBlocker(false);
				if (FreeAwardShowHandler.Instance != null)
				{
					FreeAwardShowHandler.Instance.IsInteractable = true;
				}
			}
		}

		private void ShowLobbyHeader(bool visible, long timeLeft)
		{
			if (_nickLabelController == null)
			{
				return;
			}
			if (_getEggProcessed)
			{
				visible = false;
			}
			_nickLabelController.NestTimerLabel.gameObject.SetActiveSafe(visible && timeLeft > 0);
			_nickLabelController.NestGO.transform.localPosition = ((timeLeft > 0) ? _nickLabelController.NestLabelPos : _nickLabelController.NestLabelPosWithoutTimer);
			if (_nickLabelController.NestGO.transform.localPosition != _prevPos)
			{
				_nickLabelController.GetComponent<UIPanel>().Do(delegate(UIPanel p)
				{
					p.SetDirty();
					p.Refresh();
				});
			}
			_prevPos = _nickLabelController.NestGO.transform.localPosition;
			if (visible)
			{
				_nickLabelController.NestTimerLabel.text = RiliExtensions.GetTimeString(timeLeft);
			}
		}

		public void SetNickLabelVisible(bool isVisible)
		{
			if (!(_nickLabelController == null))
			{
				_nickLabelControllerVisible = isVisible;
				_nickLabelController.gameObject.SetActive(isVisible);
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			SetMenuInteractionEnabled(true);
			_getEggProcessed = false;
		}
	}
}

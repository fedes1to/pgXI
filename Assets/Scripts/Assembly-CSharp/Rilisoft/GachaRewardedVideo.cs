using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FyberPlugin;
using UnityEngine;

namespace Rilisoft
{
	[DisallowMultipleComponent]
	internal sealed class GachaRewardedVideo : MonoBehaviour
	{
		private enum Input
		{
			None,
			Start,
			Update,
			Proceed,
			Close
		}

		private sealed class Initial : StateBase<Input>
		{
			private readonly GachaRewardedVideo _gachaRewardedVideo;

			public Initial(GachaRewardedVideo gachaRewardedVideo)
			{
				if (gachaRewardedVideo == null)
				{
					throw new ArgumentNullException("gachaRewardedVideo");
				}
				_gachaRewardedVideo = gachaRewardedVideo;
			}

			public override ReactionBase<Input> React(Input input)
			{
				return new TransitReaction<Idle, Input>(new Idle(_gachaRewardedVideo));
			}
		}

		private sealed class Idle : StateBase<Input>
		{
			private readonly GachaRewardedVideo _gachaRewardedVideo;

			public Idle(GachaRewardedVideo gachaRewardedVideo)
			{
				if (gachaRewardedVideo == null)
				{
					throw new ArgumentNullException("gachaRewardedVideo");
				}
				_gachaRewardedVideo = gachaRewardedVideo;
			}

			public override void Enter(StateBase<Input> oldState, Input input)
			{
				DebugLog("Idle.Enter");
				_gachaRewardedVideo.RaiseEnterIdle(GetEnterEventArgs(oldState));
			}

			public override ReactionBase<Input> React(Input input)
			{
				if (input == Input.Proceed)
				{
					return new TransitReaction<Waiting, Input>(new Waiting(_gachaRewardedVideo));
				}
				return DiscardReaction<Input>.Default;
			}

			public override void Exit(StateBase<Input> newState, Input input)
			{
				DebugLog("Idle.Exit");
				_gachaRewardedVideo.RaiseExitIdle(EventArgs.Empty);
			}

			private FinishedEventArgs GetEnterEventArgs(StateBase<Input> oldState)
			{
				Watching watching = oldState as Watching;
				if (watching == null)
				{
					return FinishedEventArgs.Failure;
				}
				if (!watching.AdClosedFuture.IsCompleted)
				{
					return FinishedEventArgs.Failure;
				}
				if (watching.AdClosedFuture.IsFaulted)
				{
					return FinishedEventArgs.Failure;
				}
				return FinishedEventArgs.Success;
			}
		}

		private sealed class Waiting : StateBase<Input>
		{
			private const float TimeoutInSeconds = 5f;

			private readonly GachaRewardedVideo _gachaRewardedVideo;

			private readonly TaskCompletionSource<Ad> _adPromise = new TaskCompletionSource<Ad>();

			private float _startTime = Time.realtimeSinceStartup;

			private Task<Ad> AdFuture
			{
				get
				{
					return _adPromise.Task;
				}
			}

			public Waiting(GachaRewardedVideo gachaRewardedVideo)
			{
				if (gachaRewardedVideo == null)
				{
					throw new ArgumentNullException("gachaRewardedVideo");
				}
				_gachaRewardedVideo = gachaRewardedVideo;
			}

			public override void Enter(StateBase<Input> oldState, Input input)
			{
				DebugLog("Waiting.Enter");
				_gachaRewardedVideo.waitingPanel.SetActive(true);
				if (_gachaRewardedVideo.simulateButton != null)
				{
					_gachaRewardedVideo.simulateButton.SetActive(Application.isEditor);
				}
				_startTime = Time.realtimeSinceStartup;
				FyberCallback.AdAvailable += OnAdAvailable;
				FyberCallback.AdNotAvailable += OnAdNotAvailable;
				FyberCallback.RequestFail += OnRequestFail;
				if (!Application.isEditor)
				{
					RewardedVideoRequester.Create().NotifyUserOnCompletion(false).Request();
				}
			}

			public override ReactionBase<Input> React(Input input)
			{
				if (_gachaRewardedVideo.loadingSpinner != null)
				{
					float num = Time.realtimeSinceStartup - _startTime;
					int num2 = Mathf.FloorToInt(num);
					bool flag = num2 % 2 == 0;
					_gachaRewardedVideo.loadingSpinner.invert = flag;
					_gachaRewardedVideo.loadingSpinner.fillAmount = ((!flag) ? (1f - num + (float)num2) : (num - (float)num2));
				}
				if (input == Input.Proceed && Application.isEditor)
				{
					_adPromise.TrySetResult(null);
					return DiscardReaction<Input>.Default;
				}
				if (input != Input.Update)
				{
					return DiscardReaction<Input>.Default;
				}
				if (AdFuture.IsCompleted)
				{
					if (AdFuture.IsFaulted)
					{
						Exception ex = AdFuture.Exception.InnerExceptions.FirstOrDefault();
						string reason = ((ex == null) ? AdFuture.Exception.Message : ex.Message);
						return new TransitReaction<Failure, Input>(new Failure(_gachaRewardedVideo, reason));
					}
					if (AdFuture.Result == null)
					{
						if (Application.isEditor)
						{
							return new TransitReaction<Watching, Input>(new Watching(_gachaRewardedVideo, AdFuture.Result));
						}
						return new TransitReaction<Failure, Input>(new Failure(_gachaRewardedVideo, "Ad is not available."));
					}
					return new TransitReaction<Watching, Input>(new Watching(_gachaRewardedVideo, AdFuture.Result));
				}
				if (5f <= Time.realtimeSinceStartup - _startTime)
				{
					string reason2 = string.Format(CultureInfo.InvariantCulture, "Timeout {0:f1} seconds.", 5f);
					return new TransitReaction<Failure, Input>(new Failure(_gachaRewardedVideo, reason2));
				}
				return DiscardReaction<Input>.Default;
			}

			public override void Exit(StateBase<Input> newState, Input input)
			{
				DebugLog("Waiting.Exit");
				FyberCallback.AdAvailable -= OnAdAvailable;
				FyberCallback.AdNotAvailable -= OnAdNotAvailable;
				FyberCallback.RequestFail -= OnRequestFail;
				if (_gachaRewardedVideo.simulateButton != null)
				{
					_gachaRewardedVideo.simulateButton.SetActive(false);
				}
				_gachaRewardedVideo.waitingPanel.SetActive(false);
			}

			private void OnAdAvailable(Ad ad)
			{
				_adPromise.TrySetResult(ad);
			}

			private void OnAdNotAvailable(AdFormat adFormat)
			{
				if (adFormat != AdFormat.REWARDED_VIDEO)
				{
					DebugLog("Unexpected ad format: " + adFormat);
				}
				else
				{
					_adPromise.TrySetResult(null);
				}
			}

			private void OnRequestFail(RequestError requestError)
			{
				_adPromise.TrySetException(new InvalidOperationException(requestError.Description));
			}
		}

		private sealed class Failure : StateBase<Input>
		{
			private readonly GachaRewardedVideo _gachaRewardedVideo;

			private readonly string _reason;

			public Failure(GachaRewardedVideo gachaRewardedVideo, string reason)
			{
				if (gachaRewardedVideo == null)
				{
					throw new ArgumentNullException("gachaRewardedVideo");
				}
				_gachaRewardedVideo = gachaRewardedVideo;
				_reason = reason ?? string.Empty;
			}

			public override void Enter(StateBase<Input> oldState, Input input)
			{
				DebugLog("Failure.Enter: " + _reason);
				_gachaRewardedVideo.failurePanel.SetActive(true);
			}

			public override ReactionBase<Input> React(Input input)
			{
				if (input == Input.Close)
				{
					return new TransitReaction<Idle, Input>(new Idle(_gachaRewardedVideo));
				}
				return DiscardReaction<Input>.Default;
			}

			public override void Exit(StateBase<Input> newState, Input input)
			{
				DebugLog("Failure.Exit: " + _reason);
				_gachaRewardedVideo.failurePanel.SetActive(false);
			}
		}

		private sealed class Watching : StateBase<Input>
		{
			private readonly Ad _ad;

			private readonly TaskCompletionSource<string> _adClosedPromise = new TaskCompletionSource<string>();

			private readonly GachaRewardedVideo _gachaRewardedVideo;

			internal Task<string> AdClosedFuture
			{
				get
				{
					return _adClosedPromise.Task;
				}
			}

			public Watching(GachaRewardedVideo gachaRewardedVideo, Ad ad)
			{
				if (gachaRewardedVideo == null)
				{
					throw new ArgumentNullException("gachaRewardedVideo");
				}
				if (!Application.isEditor && ad == null)
				{
					throw new ArgumentNullException("ad");
				}
				_ad = ad;
				_gachaRewardedVideo = gachaRewardedVideo;
			}

			public override void Enter(StateBase<Input> oldState, Input input)
			{
				DebugLog("Watching.Enter");
				_gachaRewardedVideo.watchingPanel.SetActive(true);
				FyberCallback.AdFinished += OnAdFinished;
				CoroutineRunner.Instance.StartCoroutine(WaitFutureThenContinue());
				if (Application.isEditor)
				{
					CoroutineRunner.Instance.StartCoroutine(SimulateWatchingCoroutine());
				}
				else
				{
					_ad.Start();
				}
			}

			public override ReactionBase<Input> React(Input input)
			{
				switch (input)
				{
				case Input.Close:
				{
					string message = "Watching panel was closed manually.";
					_adClosedPromise.TrySetException(new InvalidOperationException(message));
					return new TransitReaction<Idle, Input>(new Idle(_gachaRewardedVideo));
				}
				default:
					return DiscardReaction<Input>.Default;
				case Input.Update:
					if (!AdClosedFuture.IsCompleted)
					{
						return DiscardReaction<Input>.Default;
					}
					return new TransitReaction<Idle, Input>(new Idle(_gachaRewardedVideo));
				}
			}

			public override void Exit(StateBase<Input> newState, Input input)
			{
				DebugLog("Watching.Exit");
				FyberCallback.AdFinished -= OnAdFinished;
				if (AdClosedFuture.IsFaulted)
				{
					Exception ex = AdClosedFuture.Exception.InnerExceptions.FirstOrDefault();
					string message = ((ex == null) ? AdClosedFuture.Exception.Message : ex.Message);
					Debug.LogWarning(message);
				}
				_gachaRewardedVideo.watchingPanel.SetActive(false);
			}

			private void OnAdFinished(AdResult adResult)
			{
				FyberCallback.AdFinished -= OnAdFinished;
				if (adResult.AdFormat != AdFormat.REWARDED_VIDEO)
				{
					DebugLog("Unexpected ad format: " + adResult.AdFormat);
				}
				else if (adResult.Status != 0)
				{
					string message = "Bad status: " + adResult.Status;
					_adClosedPromise.TrySetException(new InvalidOperationException(message));
				}
				else
				{
					PlayerPrefs.SetString("Ads.LastTimeShown", DateTime.UtcNow.ToString("s"));
					_adClosedPromise.TrySetResult(adResult.Message);
				}
			}

			private IEnumerator WaitFutureThenContinue()
			{
				Task<string> f = AdClosedFuture;
				yield return new WaitUntil(() => f.IsCompleted);
				if (!f.IsFaulted && !f.IsCanceled && f.Result == "CLOSE_FINISHED")
				{
					_gachaRewardedVideo.RaiseAdWatchedSuccessfully(EventArgs.Empty);
				}
			}

			private IEnumerator SimulateWatchingCoroutine()
			{
				yield return new WaitForSeconds(5f);
				if (!AdClosedFuture.IsCompleted)
				{
					PlayerPrefs.SetString("Ads.LastTimeShown", DateTime.UtcNow.ToString("s"));
					_adClosedPromise.TrySetResult("CLOSE_FINISHED");
				}
			}
		}

		internal const string LastTimeShownKey = "Ads.LastTimeShown";

		[SerializeField]
		[Header("Internal")]
		private GameObject waitingPanel;

		[SerializeField]
		private GameObject watchingPanel;

		[SerializeField]
		private GameObject failurePanel;

		[SerializeField]
		private GameObject simulateButton;

		[SerializeField]
		private UITexture loadingSpinner;

		public GameObject windowBlocker;

		private IDisposable _backSubscription;

		private StateBase<Input> _currentState;

		public event EventHandler<FinishedEventArgs> EnterIdle;

		public event EventHandler ExitIdle;

		public event EventHandler AdWatchedSuccessfully;

		private GachaRewardedVideo()
		{
			_currentState = new Initial(this);
		}

		public void OnCloseFailurePanel()
		{
			Process(Input.Close);
		}

		public void OnCloseWatchingPanel()
		{
			Process(Input.Close);
		}

		public void OnSimulateButtonClicked()
		{
			Process(Input.Proceed);
		}

		public void OnWatchButtonClicked()
		{
			Process(Input.Proceed);
		}

		private void Awake()
		{
			Process(Input.Start);
		}

		private void OnEnable()
		{
			if (_backSubscription != null)
			{
				_backSubscription.Dispose();
			}
			_backSubscription = BackSystem.Instance.Register(OnBackRequested, GetType().Name);
		}

		private void OnDisable()
		{
			if (_backSubscription != null)
			{
				_backSubscription.Dispose();
				_backSubscription = null;
			}
		}

		private void Update()
		{
			if (windowBlocker != null)
			{
				windowBlocker.SetActive(waitingPanel.activeInHierarchy || watchingPanel.activeInHierarchy || failurePanel.activeInHierarchy);
			}
			Process(Input.Update);
		}

		private void OnBackRequested()
		{
			ReactionBase<Input> reactionBase = _currentState.React(Input.Close);
			StateBase<Input> newState = reactionBase.GetNewState();
			if (newState != null)
			{
				OnCloseFailurePanel();
			}
			else
			{
				_backSubscription = BackSystem.Instance.Register(OnCloseFailurePanel, GetType().Name);
			}
		}

		private void RaiseEnterIdle(FinishedEventArgs e)
		{
			EventHandler<FinishedEventArgs> enterIdle = this.EnterIdle;
			if (enterIdle != null)
			{
				enterIdle(this, e);
			}
		}

		private void RaiseExitIdle(EventArgs e)
		{
			EventHandler exitIdle = this.ExitIdle;
			if (exitIdle != null)
			{
				exitIdle(this, e);
			}
		}

		private void RaiseAdWatchedSuccessfully(EventArgs e)
		{
			EventHandler adWatchedSuccessfully = this.AdWatchedSuccessfully;
			if (adWatchedSuccessfully != null)
			{
				adWatchedSuccessfully(this, e);
			}
		}

		private void Process(Input input)
		{
			ReactionBase<Input> reactionBase = _currentState.React(input);
			StateBase<Input> newState = reactionBase.GetNewState();
			if (newState != null)
			{
				StateBase<Input> currentState = _currentState;
				currentState.Exit(newState, input);
				newState.Enter(currentState, input);
				_currentState = newState;
				IDisposable disposable = currentState as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		private static void DebugLog(string message)
		{
			if (Defs.IsDeveloperBuild)
			{
				string format = ((!Application.isEditor) ? "[{0}] {1}" : "<color=cyan>[{0}] {1}</color>");
				Debug.LogFormat(format, typeof(GachaRewardedVideo).Name, message);
			}
		}
	}
}

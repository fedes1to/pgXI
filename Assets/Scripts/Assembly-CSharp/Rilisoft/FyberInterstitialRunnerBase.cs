using System;
using System.Collections;
using System.Globalization;
using System.Threading.Tasks;
using FyberPlugin;
using UnityEngine;

namespace Rilisoft
{
	internal abstract class FyberInterstitialRunnerBase : IDisposable, AdCallback
	{
		private readonly TaskCompletionSource<AdResult> _adFinishedPromise = new TaskCompletionSource<AdResult>();

		private bool _disposed;

		public Task<AdResult> AdFinishedFuture
		{
			get
			{
				return _adFinishedPromise.Task;
			}
		}

		public void Run()
		{
			if (Defs.IsDeveloperBuild)
			{
				string format = ((!Application.isEditor) ? "{0}.Run" : "<color=magenta>{0}.Run</color>");
				Debug.LogFormat(format, GetType().Name);
			}
			FyberCallback.AdAvailable += OnAdAvailable;
			FyberCallback.AdNotAvailable += OnAdNotAvailable;
			FyberCallback.RequestFail += OnRequestFail;
			if (Application.isEditor || BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				CoroutineRunner.Instance.StartCoroutine(SimulateRequestCoroutine());
			}
			else
			{
				InterstitialRequester.Create().Request();
			}
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				Unsubscribe();
				Dispose(true);
				_disposed = true;
			}
		}

		public void OnAdFinished(AdResult result)
		{
			if (!_disposed)
			{
				if (Defs.IsDeveloperBuild)
				{
					string format = ((!Application.isEditor) ? "{0}.OnAdFinished: {1}" : "<color=magenta>{0}.OnAdFinished: {1}</color>");
					string text = string.Format(CultureInfo.InvariantCulture, "{{ status: {0}, message: '{1}' }}", result.Status, result.Message);
					Debug.LogFormat(format, GetType().Name, text);
				}
				_adFinishedPromise.TrySetResult(result);
			}
		}

		public void OnAdStarted(Ad ad)
		{
			if (!_disposed && Defs.IsDeveloperBuild)
			{
				string format = ((!Application.isEditor) ? "{0}.OnAdStarted: {1}" : "<color=magenta>{0}.OnAdStarted: {1}</color>");
				Debug.LogFormat(format, GetType().Name, ad);
			}
		}

		protected abstract string GetReasonToSkip();

		protected virtual void Dispose(bool disposing)
		{
		}

		private void OnAdAvailable(Ad ad)
		{
			if (_disposed || ad.AdFormat != AdFormat.INTERSTITIAL)
			{
				return;
			}
			Unsubscribe();
			if (Defs.IsDeveloperBuild)
			{
				string format = ((!Application.isEditor) ? "{0}.OnAdAvailable: {1}" : "<color=magenta>{0}.OnAdAvailable: {1}</color>");
				Debug.LogFormat(format, GetType().Name, ad);
			}
			string reasonToSkip = GetReasonToSkip();
			if (!string.IsNullOrEmpty(reasonToSkip))
			{
				Debug.LogFormat("Skipping showing interstitial: {0}", reasonToSkip);
				return;
			}
			Debug.Log("Trying to show interstitial.");
			PlayerPrefs.SetString(Defs.LastTimeShowBanerKey, DateTime.UtcNow.ToString("s"));
			FyberFacade.Instance.IncrementCurrentDailyInterstitialCount();
			if (Application.isEditor || BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				CoroutineRunner.Instance.StartCoroutine(SimulateStartCoroutine());
			}
			else
			{
				ad.WithCallback(this).Start();
			}
		}

		private void OnAdNotAvailable(AdFormat adFormat)
		{
			if (!_disposed && adFormat == AdFormat.INTERSTITIAL)
			{
				Unsubscribe();
				if (Defs.IsDeveloperBuild)
				{
					string format = ((!Application.isEditor) ? "{0}.OnAdNotAvailable: {1}" : "<color=magenta>{0}.OnAdNotAvailable: {1}</color>");
					Debug.LogFormat(format, GetType().Name, adFormat);
				}
				string message = "Interstitial is not available.";
				_adFinishedPromise.TrySetException(new InvalidOperationException(message));
			}
		}

		private void OnRequestFail(RequestError requestError)
		{
			if (!_disposed)
			{
				Unsubscribe();
				if (Defs.IsDeveloperBuild)
				{
					string format = ((!Application.isEditor) ? "{0}.OnRequestFail: {1}" : "<color=magenta>{0}.OnRequestFail: {1}</color>");
					Debug.LogFormat(format, GetType().Name, requestError.Description);
				}
				_adFinishedPromise.TrySetException(new InvalidOperationException(requestError.Description));
			}
		}

		private void Unsubscribe()
		{
			FyberCallback.AdAvailable -= OnAdAvailable;
			FyberCallback.AdNotAvailable -= OnAdNotAvailable;
			FyberCallback.RequestFail -= OnRequestFail;
		}

		private IEnumerator SimulateRequestCoroutine()
		{
			yield return new WaitForSeconds(1f);
			OnAdNotAvailable(AdFormat.INTERSTITIAL);
		}

		private IEnumerator SimulateStartCoroutine()
		{
			yield return new WaitForSeconds(1f);
			_adFinishedPromise.TrySetException(new NotSupportedException());
		}
	}
}

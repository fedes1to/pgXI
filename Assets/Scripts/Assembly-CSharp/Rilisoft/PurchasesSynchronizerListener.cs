using System;
using System.Collections;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class PurchasesSynchronizerListener : MonoBehaviour
	{
		private IDisposable _escapeSubscription;

		private void Start()
		{
			PurchasesSynchronizer.Instance.PurchasesSavingStarted += HandlePurchasesSavingStarted;
		}

		private void OnDestroy()
		{
			PurchasesSynchronizer.Instance.PurchasesSavingStarted -= HandlePurchasesSavingStarted;
			if (_escapeSubscription != null)
			{
				_escapeSubscription.Dispose();
			}
		}

		private void HandlePurchasesSavingStarted(object sender, PurchasesSavingEventArgs e)
		{
			string callee = string.Format(CultureInfo.InvariantCulture, "{0}.HandlePurchasesSavingStarted()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(callee, Defs.IsDeveloperBuild);
			try
			{
				_escapeSubscription = BackSystem.Instance.Register(HandleEscape, "PurchasesSynchronizerListener");
				string activeWithCaption = LocalizationStore.Get("Key_1974");
				ActivityIndicator.SetActiveWithCaption(activeWithCaption);
				InfoWindowController.BlockAllClick();
				StartCoroutine(WaitCompletionCoroutine(e.Future));
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private IEnumerator WaitCompletionCoroutine(Task<bool> future)
		{
			string thisMethod = string.Format(CultureInfo.InvariantCulture, "{0}.WaitCompletionCoroutine()", GetType().Name);
			ScopeLogger scopeLogger = new ScopeLogger(thisMethod, Defs.IsDeveloperBuild);
			try
			{
				while (!future.IsCompleted)
				{
					yield return null;
				}
				InfoWindowController.HideCurrentWindow();
				ActivityIndicator.IsActiveIndicator = false;
				if (_escapeSubscription != null)
				{
					_escapeSubscription.Dispose();
				}
			}
			finally
			{
				scopeLogger.Dispose();
			}
		}

		private void HandleEscape()
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Ignoring [Escape] while syncing.");
			}
		}
	}
}

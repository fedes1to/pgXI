using UnityEngine;
using System;

namespace Rilisoft
{
	internal sealed class InterstitialManager
	{
		private static readonly Lazy<InterstitialManager> _instance = new Lazy<InterstitialManager>(() => new InterstitialManager());

		private int _interstitialProviderIndex;

		public static InterstitialManager Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		public AdProvider Provider
		{
			get
			{
				return GetProviderByIndex(_interstitialProviderIndex);
			}
		}

		private InterstitialManager()
		{
		}

		public AdProvider GetProviderByIndex(int index)
		{
			if (PromoActionsManager.MobileAdvert == null)
			{
				return AdProvider.None;
			}
			if (PromoActionsManager.MobileAdvert.InterstitialProviders.Count == 0)
			{
				return AdProvider.None;
			}
			return (AdProvider)PromoActionsManager.MobileAdvert.InterstitialProviders[index % PromoActionsManager.MobileAdvert.InterstitialProviders.Count];
		}

		internal int SwitchAdProvider()
		{
			int interstitialProviderIndex = _interstitialProviderIndex;
			AdProvider provider = Provider;
			_interstitialProviderIndex++;
			if (provider == AdProvider.GoogleMobileAds)
			{
				MobileAdManager.Instance.DestroyImageInterstitial();
			}
			if (Provider == AdProvider.GoogleMobileAds)
			{
				MobileAdManager.Instance.SwitchImageIdGroup();
			}
			if (Defs.IsDeveloperBuild)
			{
				string message = string.Format("Switching interstitial provider from {0} ({1}) to {2} ({3})", interstitialProviderIndex, provider, _interstitialProviderIndex, Provider);
				Debug.Log(message);
			}
			return _interstitialProviderIndex;
		}

		internal void ResetAdProvider()
		{
			int interstitialProviderIndex = _interstitialProviderIndex;
			AdProvider provider = Provider;
			_interstitialProviderIndex = 0;
			if (provider == AdProvider.GoogleMobileAds)
			{
				MobileAdManager.Instance.DestroyImageInterstitial();
			}
			if (Provider == AdProvider.GoogleMobileAds)
			{
				MobileAdManager.Instance.ResetImageAdUnitId();
			}
			if (Defs.IsDeveloperBuild)
			{
				string message = string.Format("Resetting image interstitial provider from {0} ({1}) to {2} ({3})", interstitialProviderIndex, provider, _interstitialProviderIndex, Provider);
				Debug.Log(message);
			}
		}
	}
}

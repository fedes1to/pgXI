namespace com.amazon.mas.cpt.ads
{
	public interface IAmazonMobileAds
	{
		void SetApplicationKey(ApplicationKey applicationKey);

		void RegisterApplication();

		void EnableLogging(ShouldEnable shouldEnable);

		void EnableTesting(ShouldEnable shouldEnable);

		void EnableGeoLocation(ShouldEnable shouldEnable);

		Ad CreateFloatingBannerAd(Placement placement);

		Ad CreateInterstitialAd();

		LoadingStarted LoadAndShowFloatingBannerAd(Ad ad);

		LoadingStarted LoadInterstitialAd();

		AdShown ShowInterstitialAd();

		void CloseFloatingBannerAd(Ad ad);

		IsReady IsInterstitialAdReady();

		IsEqual AreAdsEqual(AdPair adPair);

		void UnityFireEvent(string jsonMessage);

		void AddAdCollapsedListener(AdCollapsedDelegate responseDelegate);

		void RemoveAdCollapsedListener(AdCollapsedDelegate responseDelegate);

		void AddAdDismissedListener(AdDismissedDelegate responseDelegate);

		void RemoveAdDismissedListener(AdDismissedDelegate responseDelegate);

		void AddAdExpandedListener(AdExpandedDelegate responseDelegate);

		void RemoveAdExpandedListener(AdExpandedDelegate responseDelegate);

		void AddAdFailedToLoadListener(AdFailedToLoadDelegate responseDelegate);

		void RemoveAdFailedToLoadListener(AdFailedToLoadDelegate responseDelegate);

		void AddAdLoadedListener(AdLoadedDelegate responseDelegate);

		void RemoveAdLoadedListener(AdLoadedDelegate responseDelegate);

		void AddAdResizedListener(AdResizedDelegate responseDelegate);

		void RemoveAdResizedListener(AdResizedDelegate responseDelegate);
	}
}

namespace FyberPlugin
{
	public class AdMobNetworkBannerSizes
	{
		private const string NETWORK_NAME = "AdMob";

		public static readonly NetworkBannerSize BANNER = new NetworkBannerSize("AdMob", new BannerSize(320, 50));

		public static readonly NetworkBannerSize LARGE_BANNER = new NetworkBannerSize("AdMob", new BannerSize(320, 100));

		public static readonly NetworkBannerSize MEDIUM_RECTANGLE = new NetworkBannerSize("AdMob", new BannerSize(300, 250));

		public static readonly NetworkBannerSize FULL_BANNER = new NetworkBannerSize("AdMob", new BannerSize(468, 60));

		public static readonly NetworkBannerSize LEADERBOARD = new NetworkBannerSize("AdMob", new BannerSize(728, 90));

		public static readonly NetworkBannerSize SMART_BANNER = new NetworkBannerSize("AdMob", new BannerSize(-1, -2));
	}
}

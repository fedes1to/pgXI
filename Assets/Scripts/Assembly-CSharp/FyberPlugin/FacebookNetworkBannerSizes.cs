namespace FyberPlugin
{
	public class FacebookNetworkBannerSizes
	{
		private const string NETWORK_NAME = "FacebookAudienceNetwork";

		public static readonly NetworkBannerSize BANNER_50 = new NetworkBannerSize("FacebookAudienceNetwork", new BannerSize(320, 50));

		public static readonly NetworkBannerSize BANNER_90 = new NetworkBannerSize("FacebookAudienceNetwork", new BannerSize(320, 90));

		public static readonly NetworkBannerSize RECTANGLE_HEIGHT_250 = new NetworkBannerSize("FacebookAudienceNetwork", new BannerSize(300, 250));
	}
}

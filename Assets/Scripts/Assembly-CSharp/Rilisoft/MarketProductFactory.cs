using com.amazon.device.iap.cpt;
using Prime31;

namespace Rilisoft
{
	internal static class MarketProductFactory
	{
		internal static GoogleMarketProduct CreateGoogleMarketProduct(GoogleSkuInfo googleSkuInfo)
		{
			return new GoogleMarketProduct(googleSkuInfo);
		}

		internal static AmazonMarketProduct CreateAmazonMarketProduct(ProductData amazonItem)
		{
			return new AmazonMarketProduct(amazonItem);
		}
	}
}

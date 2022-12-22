using System;
using Prime31;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class GoogleMarketProduct : IMarketProduct
	{
		private readonly GoogleSkuInfo _marketProduct;

		public string Id
		{
			get
			{
				return _marketProduct.productId;
			}
		}

		public string Title
		{
			get
			{
				return _marketProduct.title;
			}
		}

		public string Description
		{
			get
			{
				return _marketProduct.description;
			}
		}

		public string Price
		{
			get
			{
				return _marketProduct.price;
			}
		}

		public object PlatformProduct
		{
			get
			{
				return _marketProduct;
			}
		}

		public decimal PriceValue
		{
			get
			{
				if (_marketProduct == null)
				{
					Debug.LogErrorFormat("GoogleMarketProduct.PriceValue: _marketProduct == null");
					return 0m;
				}
				decimal result = 0m;
				try
				{
					result = StoreKitEventListener.GetPriceFromPriceAmountMicros(_marketProduct.priceAmountMicros);
					return result;
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Exception in PriceValue: {0}", ex);
					return result;
				}
			}
		}

		public string Currency
		{
			get
			{
				if (_marketProduct == null)
				{
					Debug.LogErrorFormat("GoogleMarketProduct.Currency: _marketProduct == null");
					return string.Empty;
				}
				string result = string.Empty;
				try
				{
					result = _marketProduct.priceCurrencyCode ?? string.Empty;
					return result;
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Exception in Currency: {0}", ex);
					return result;
				}
			}
		}

		public GoogleMarketProduct(GoogleSkuInfo googleSkuInfo)
		{
			_marketProduct = googleSkuInfo;
		}

		public override bool Equals(object obj)
		{
			GoogleMarketProduct googleMarketProduct = obj as GoogleMarketProduct;
			if (googleMarketProduct == null)
			{
				return false;
			}
			GoogleSkuInfo marketProduct = googleMarketProduct._marketProduct;
			return _marketProduct.Equals(marketProduct);
		}

		public override int GetHashCode()
		{
			return _marketProduct.GetHashCode();
		}

		public override string ToString()
		{
			return _marketProduct.ToString();
		}
	}
}

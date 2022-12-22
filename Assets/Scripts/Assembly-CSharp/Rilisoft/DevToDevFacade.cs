using System;
using System.Collections.Generic;

namespace Rilisoft
{
	internal sealed class DevToDevFacade
	{
		private readonly string _appKey;

		public static string Version
		{
			get
			{
				return string.Empty;
			}
		}

		public bool UserIsCheater
		{
			set
			{
				if (!string.IsNullOrEmpty(_appKey))
				{
				}
			}
		}

		public static bool LoggingEnabled
		{
			set
			{
			}
		}

		public DevToDevFacade(string appKey, string secretKey)
		{
			if (appKey == null)
			{
				throw new ArgumentNullException("appKey");
			}
			if (secretKey == null)
			{
				throw new ArgumentNullException("secretKey");
			}
			_appKey = appKey;
		}

		public void SendCustomEvent(string eventName)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (string.IsNullOrEmpty(eventName))
			{
				throw new ArgumentException("Event name must not be empty.", "eventName");
			}
			if (!string.IsNullOrEmpty(_appKey))
			{
			}
		}

		public void InAppPurchase(string purchaseId, string purchaseType, int purchaseAmount, int purchasePrice, string purchaseCurrency)
		{
			purchaseId = purchaseId.Substring(0, Math.Min(purchaseId.Length, 32));
			purchaseType = purchaseType.Substring(0, Math.Min(purchaseType.Length, 96));
		}

		public void RealPayment(string paymentId, float inAppPrice, string inAppName, string inAppCurrencyISOCode)
		{
		}

		public void CurrencyAccrual(int amount, string currencyName, AnalyticsConstants.AccrualType accrualType)
		{
		}

		public void LevelUp(int level, Dictionary<string, int> resources)
		{
		}

		public void Tutorial(AnalyticsConstants.TutorialState step)
		{
		}

		public void SendCustomEvent(string eventName, IDictionary<string, object> eventParams)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (eventParams == null)
			{
				throw new ArgumentNullException("eventParams");
			}
			if (string.IsNullOrEmpty(eventName))
			{
				throw new ArgumentException("Event name must not be empty.", "eventName");
			}
			if (!string.IsNullOrEmpty(_appKey))
			{
			}
		}

		public void SendBufferedEvents()
		{
			if (!string.IsNullOrEmpty(_appKey))
			{
			}
		}
	}
}

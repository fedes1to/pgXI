using System.Collections.Generic;
using Facebook.Unity;
using Rilisoft.MiniJson;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class FacebookFacade
	{
		internal void LogPurchase(float currencyAmount, string currencyIsoCode, Dictionary<string, object> parameters = null)
		{
			if (!FB.IsInitialized)
			{
				Debug.LogWarning("Facebook is not initialized.");
			}
			else
			{
				FB.LogPurchase(currencyAmount, currencyIsoCode, parameters);
			}
		}

		internal void LogAppEvent(string logEvent, float? valueToSum, Dictionary<string, object> parameters)
		{
			if (!FB.IsInitialized)
			{
				Debug.LogWarning("Facebook is not initialized.");
				return;
			}
			if (Application.isEditor)
			{
				string text = ((parameters == null) ? "{}" : Json.Serialize(parameters));
				Debug.LogFormat("`{0}`: {1}", logEvent, text);
			}
			FB.LogAppEvent(logEvent, valueToSum, parameters);
		}
	}
}

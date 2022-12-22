using System;
using System.Collections.Generic;
using Prime31;

namespace Rilisoft
{
	internal sealed class FlurryFacade
	{
		public FlurryFacade(string apiKey, bool enableLogging)
		{
			if (apiKey == null)
			{
				throw new ArgumentNullException("apiKey");
			}
			FlurryAnalytics.startSession(apiKey, enableLogging);
			FlurryAnalytics.setLogEnabled(enableLogging);
		}

		public void LogEvent(string eventName)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			FlurryAnalytics.logEvent(eventName);
		}

		public void LogEventWithParameters(string eventName, Dictionary<string, string> parameters)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			FlurryAnalytics.logEvent(eventName, parameters);
		}

		public void SetUserId(string userId)
		{
			if (userId == null)
			{
				throw new ArgumentNullException("userId");
			}
			FlurryAnalytics.setUserID(userId);
		}
	}
}

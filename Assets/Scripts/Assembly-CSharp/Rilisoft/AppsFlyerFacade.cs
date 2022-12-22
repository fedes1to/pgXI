using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class AppsFlyerFacade
	{
		private readonly string _appId;

		public static bool LoggingEnabled
		{
			set
			{
				if (!Application.isEditor)
				{
					AppsFlyer.setIsDebug(value);
				}
			}
		}

		public AppsFlyerFacade(string appKey, string appId)
		{
			if (appKey == null)
			{
				throw new ArgumentNullException("appKey");
			}
			if (appId == null)
			{
				throw new ArgumentNullException(appId);
			}
			_appId = appId;
			if (!Application.isEditor)
			{
				AppsFlyer.setAppsFlyerKey(appKey);
				AppsFlyer.setAppID(appId);
			}
		}

		public void TrackEvent(string eventName, string eventValue)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (eventValue == null)
			{
				throw new ArgumentNullException("eventValue");
			}
			if (string.IsNullOrEmpty(eventName))
			{
				throw new ArgumentException("Event name must not be empty.", "eventName");
			}
			if (!string.IsNullOrEmpty(_appId) && !Application.isEditor)
			{
				AppsFlyer.trackEvent(eventName, eventValue);
			}
		}

		public void TrackRichEvent(string eventName, Dictionary<string, string> eventValues)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (eventValues == null)
			{
				throw new ArgumentNullException("eventValues");
			}
			if (string.IsNullOrEmpty(eventName))
			{
				throw new ArgumentException("Event name must not be empty.", "eventName");
			}
			if (!string.IsNullOrEmpty(_appId) && !Application.isEditor)
			{
				AppsFlyer.trackRichEvent(eventName, eventValues);
			}
		}

		public void TrackAppLaunch()
		{
			if (!string.IsNullOrEmpty(_appId) && !Application.isEditor)
			{
				AppsFlyer.trackAppLaunch();
			}
		}
	}
}

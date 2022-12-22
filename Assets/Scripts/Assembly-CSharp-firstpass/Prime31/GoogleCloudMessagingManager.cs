using System;
using System.Collections.Generic;

namespace Prime31
{
	public class GoogleCloudMessagingManager : AbstractManager
	{
		public static event Action<Dictionary<string, object>> notificationReceivedEvent;

		public static event Action<string> registrationSucceededEvent;

		public static event Action<string> registrationFailedEvent;

		public static event Action unregistrationSucceededEvent;

		public static event Action<string> unregistrationFailedEvent;

		static GoogleCloudMessagingManager()
		{
			AbstractManager.initialize(typeof(GoogleCloudMessagingManager));
		}

		public void notificationReceived(string json)
		{
			GoogleCloudMessagingManager.notificationReceivedEvent.fire(json.dictionaryFromJson());
		}

		public void registrationSucceeded(string registrationId)
		{
			GoogleCloudMessagingManager.registrationSucceededEvent.fire(registrationId);
		}

		public void unregistrationFailed(string param)
		{
			GoogleCloudMessagingManager.unregistrationFailedEvent.fire(param);
		}

		public void registrationFailed(string error)
		{
			GoogleCloudMessagingManager.registrationFailedEvent.fire(error);
		}

		public void unregistrationSucceeded(string empty)
		{
			GoogleCloudMessagingManager.unregistrationSucceededEvent.fire();
		}
	}
}

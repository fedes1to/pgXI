using System;
using UnityEngine;

namespace Prime31
{
	public class TwitterManager : MonoBehaviour
	{
		public static event Action<string> loginSucceededEvent;

		public static event Action<string> loginFailedEvent;

		public static event Action<object> requestDidFinishEvent;

		public static event Action<string> requestDidFailEvent;

		public static event Action<bool> tweetSheetCompletedEvent;

		public static event Action twitterInitializedEvent;

		static TwitterManager()
		{
			AbstractManager.initialize(typeof(TwitterManager));
		}

		public static void noop()
		{
		}

		public void loginSucceeded(string screenname)
		{
			if (TwitterManager.loginSucceededEvent != null)
			{
				TwitterManager.loginSucceededEvent(screenname);
			}
		}

		public void loginFailed(string error)
		{
			if (TwitterManager.loginFailedEvent != null)
			{
				TwitterManager.loginFailedEvent(error);
			}
		}

		public void requestSucceeded(string results)
		{
			if (TwitterManager.requestDidFinishEvent != null)
			{
				TwitterManager.requestDidFinishEvent(Json.decode(results));
			}
		}

		public void requestFailed(string error)
		{
			if (TwitterManager.requestDidFailEvent != null)
			{
				TwitterManager.requestDidFailEvent(error);
			}
		}

		public void tweetSheetCompleted(string oneOrZero)
		{
			if (TwitterManager.tweetSheetCompletedEvent != null)
			{
				TwitterManager.tweetSheetCompletedEvent(oneOrZero == "1");
			}
		}

		public void twitterInitialized()
		{
			if (TwitterManager.twitterInitializedEvent != null)
			{
				TwitterManager.twitterInitializedEvent();
			}
		}
	}
}

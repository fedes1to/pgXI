using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Prime31
{
	public class EtceteraAndroid
	{
		public enum ScalingMode
		{
			None,
			AspectFit,
			Fill
		}

		public class Contact
		{
			public string name;

			public List<string> emails;

			public List<string> phoneNumbers;
		}

		private static AndroidJavaObject _plugin;

		static EtceteraAndroid()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.EtceteraPlugin"))
			{
				_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
		}

		public static Texture2D textureFromFileAtPath(string filePath)
		{
			byte[] data = File.ReadAllBytes(filePath);
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(data);
			texture2D.Apply();
			Debug.Log("texture size: " + texture2D.width + "x" + texture2D.height);
			return texture2D;
		}

		public static void setSystemUiVisibilityToLowProfile(bool useLowProfile)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("setSystemUiVisibilityToLowProfile", useLowProfile);
			}
		}

		public static void playMovie(string pathOrUrl, uint bgColor, bool showControls, ScalingMode scalingMode, bool closeOnTouch)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("playMovie", pathOrUrl, (int)bgColor, showControls, (int)scalingMode, closeOnTouch);
			}
		}

		public static void setAlertDialogTheme(int theme)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("setAlertDialogTheme", theme);
			}
		}

		public static void showToast(string text, bool useShortDuration)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("showToast", text, useShortDuration);
			}
		}

		public static void showAlert(string title, string message, string positiveButton)
		{
			showAlert(title, message, positiveButton, string.Empty);
		}

		public static void showAlert(string title, string message, string positiveButton, string negativeButton)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("showAlert", title, message, positiveButton, negativeButton);
			}
		}

		public static void showAlertPrompt(string title, string message, string promptHint, string promptText, string positiveButton, string negativeButton)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("showAlertPrompt", title, message, promptHint, promptText, positiveButton, negativeButton);
			}
		}

		public static void showAlertPromptWithTwoFields(string title, string message, string promptHintOne, string promptTextOne, string promptHintTwo, string promptTextTwo, string positiveButton, string negativeButton)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("showAlertPromptWithTwoFields", title, message, promptHintOne, promptTextOne, promptHintTwo, promptTextTwo, positiveButton, negativeButton);
			}
		}

		public static void showProgressDialog(string title, string message)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("showProgressDialog", title, message);
			}
		}

		public static void hideProgressDialog()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("hideProgressDialog");
			}
		}

		public static void showWebView(string url)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("showWebView", url);
			}
		}

		public static void showCustomWebView(string url, bool disableTitle, bool disableBackButton)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("showCustomWebView", url, disableTitle, disableBackButton);
			}
		}

		public static void showEmailComposer(string toAddress, string subject, string text, bool isHTML)
		{
			showEmailComposer(toAddress, subject, text, isHTML, string.Empty);
		}

		public static void showEmailComposer(string toAddress, string subject, string text, bool isHTML, string attachmentFilePath)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("showEmailComposer", toAddress, subject, text, isHTML, attachmentFilePath);
			}
		}

		public static bool isSMSComposerAvailable()
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return false;
			}
			return _plugin.Call<bool>("isSMSComposerAvailable", new object[0]);
		}

		public static void showSMSComposer(string body)
		{
			showSMSComposer(body, null);
		}

		public static void showSMSComposer(string body, string[] recipients)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return;
			}
			string text = string.Empty;
			if (recipients != null && recipients.Length > 0)
			{
				text = "smsto:";
				foreach (string text2 in recipients)
				{
					text = text + text2 + ";";
				}
			}
			_plugin.Call("showSMSComposer", text, body);
		}

		public static void shareImageWithNativeShareIntent(string pathToImage, string chooserText)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("shareImageWithNativeShareIntent", pathToImage, chooserText);
			}
		}

		public static void shareWithNativeShareIntent(string text, string subject, string chooserText, string pathToImage = null)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("shareWithNativeShareIntent", text, subject, chooserText, pathToImage);
			}
		}

		public static void promptToTakePhoto(string name)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("promptToTakePhoto", name);
			}
		}

		public static void promptForPictureFromAlbum(string name)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("promptForPictureFromAlbum", name);
			}
		}

		public static void promptToTakeVideo(string name)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("promptToTakeVideo", name);
			}
		}

		public static bool saveImageToGallery(string pathToPhoto, string title)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return false;
			}
			return _plugin.Call<bool>("saveImageToGallery", new object[2] { pathToPhoto, title });
		}

		public static void scaleImageAtPath(string pathToImage, float scale)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("scaleImageAtPath", pathToImage, scale);
			}
		}

		public static Vector2 getImageSizeAtPath(string pathToImage)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return Vector2.zero;
			}
			string text = _plugin.Call<string>("getImageSizeAtPath", new object[1] { pathToImage });
			string[] array = text.Split(',');
			return new Vector2(int.Parse(array[0]), int.Parse(array[1]));
		}

		public static void enableImmersiveMode(bool shouldEnable)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("enableImmersiveMode", shouldEnable ? 1 : 0);
			}
		}

		public static void loadContacts(int startingIndex, int totalToRetrieve)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("loadContacts", startingIndex, totalToRetrieve);
			}
		}

		public static void initTTS()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("initTTS");
			}
		}

		public static void teardownTTS()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("teardownTTS");
			}
		}

		public static void speak(string text)
		{
			speak(text, TTSQueueMode.Add);
		}

		public static void speak(string text, TTSQueueMode queueMode)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("speak", text, (int)queueMode);
			}
		}

		public static void stop()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("stop");
			}
		}

		public static void playSilence(long durationInMs, TTSQueueMode queueMode)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("playSilence", durationInMs, (int)queueMode);
			}
		}

		public static void setPitch(float pitch)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("setPitch", pitch);
			}
		}

		public static void setSpeechRate(float rate)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("setSpeechRate", rate);
			}
		}

		public static void askForReviewSetButtonTitles(string remindMeLaterTitle, string dontAskAgainTitle, string rateItTitle)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("askForReviewSetButtonTitles", remindMeLaterTitle, dontAskAgainTitle, rateItTitle);
			}
		}

		public static void askForReview(int launchesUntilPrompt, int hoursUntilFirstPrompt, int hoursBetweenPrompts, string title, string message, bool isAmazonAppStore = false)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				if (isAmazonAppStore)
				{
					_plugin.Set("isAmazonAppStore", true);
				}
				_plugin.Call("askForReview", launchesUntilPrompt, hoursUntilFirstPrompt, hoursBetweenPrompts, title, message);
			}
		}

		public static void askForReviewNow(string title, string message, bool isAmazonAppStore = false)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				if (isAmazonAppStore)
				{
					_plugin.Set("isAmazonAppStore", true);
				}
				_plugin.Call("askForReviewNow", title, message);
			}
		}

		public static void resetAskForReview()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("resetAskForReview");
			}
		}

		public static void openReviewPageInPlayStore(bool isAmazonAppStore = false)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				if (isAmazonAppStore)
				{
					_plugin.Set("isAmazonAppStore", true);
				}
				_plugin.Call("openReviewPageInPlayStore");
			}
		}

		public static void inlineWebViewShow(string url, int x, int y, int width, int height)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("inlineWebViewShow", url, x, y, width, height);
			}
		}

		public static void inlineWebViewClose()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("inlineWebViewClose");
			}
		}

		public static void inlineWebViewSetUrl(string url)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("inlineWebViewSetUrl", url);
			}
		}

		public static void inlineWebViewSetFrame(int x, int y, int width, int height)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("inlineWebViewSetFrame", x, y, width, height);
			}
		}

		[Obsolete("Use the scheduleNotification variant that accepts a AndroidNotificationConfiguration parameter")]
		public static int scheduleNotification(long secondsFromNow, string title, string subtitle, string tickerText, string extraData, int requestCode = -1)
		{
			AndroidNotificationConfiguration androidNotificationConfiguration = new AndroidNotificationConfiguration(secondsFromNow, title, subtitle, tickerText);
			androidNotificationConfiguration.extraData = extraData;
			androidNotificationConfiguration.requestCode = requestCode;
			return scheduleNotification(androidNotificationConfiguration);
		}

		[Obsolete("Use the scheduleNotification variant that accepts a AndroidNotificationConfiguration parameter")]
		public static int scheduleNotification(long secondsFromNow, string title, string subtitle, string tickerText, string extraData, string smallIcon, string largeIcon, int requestCode = -1)
		{
			AndroidNotificationConfiguration androidNotificationConfiguration = new AndroidNotificationConfiguration(secondsFromNow, title, subtitle, tickerText);
			androidNotificationConfiguration.extraData = extraData;
			androidNotificationConfiguration.smallIcon = smallIcon;
			androidNotificationConfiguration.largeIcon = largeIcon;
			androidNotificationConfiguration.requestCode = requestCode;
			return scheduleNotification(androidNotificationConfiguration);
		}

		public static int scheduleNotification(AndroidNotificationConfiguration config)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return -1;
			}
			config.build();
			return _plugin.Call<int>("scheduleNotification", new object[1] { Json.encode(config) });
		}

		public static void cancelNotification(int notificationId)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("cancelNotification", notificationId);
			}
		}

		public static void cancelAllNotifications()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("cancelAllNotifications");
			}
		}

		public static void checkForNotifications()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("checkForNotifications");
			}
		}

		public static void requestPermissions(string[] permissions, int requestCode = 575757)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				_plugin.Call("requestPermissions", permissions, requestCode);
			}
		}

		public static bool shouldShowRequestPermissionRationale(string permission)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return false;
			}
			return _plugin.Call<bool>("shouldShowRequestPermissionRationale", new object[1] { permission });
		}

		public static bool checkSelfPermission(string permission)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return false;
			}
			return _plugin.Call<bool>("checkSelfPermission", new object[1] { permission });
		}
	}
}

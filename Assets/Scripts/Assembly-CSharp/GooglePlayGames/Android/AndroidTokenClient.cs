using System;
using Com.Google.Android.Gms.Common.Api;
using GooglePlayGames.BasicApi;
using GooglePlayGames.OurUtils;
using UnityEngine;

namespace GooglePlayGames.Android
{
	internal class AndroidTokenClient : TokenClient
	{
		private const string TokenFragmentClass = "com.google.games.bridge.TokenFragment";

		private const string FetchTokenSignature = "(Landroid/app/Activity;Ljava/lang/String;Ljava/lang/String;ZZZLjava/lang/String;)Lcom/google/android/gms/common/api/PendingResult;";

		private const string FetchTokenMethod = "fetchToken";

		private string playerId;

		private bool fetchingEmail;

		private bool fetchingAccessToken;

		private bool fetchingIdToken;

		private string accountName;

		private string accessToken;

		private string idToken;

		private string idTokenScope;

		private Action<string> idTokenCb;

		private string rationale;

		private bool apiAccessDenied;

		private int apiWarningFreq = 100000;

		private int apiWarningCount;

		private int webClientWarningFreq = 100000;

		private int webClientWarningCount;

		public AndroidTokenClient(string playerId)
		{
			this.playerId = playerId;
		}

		public static AndroidJavaObject GetActivity()
		{
			//Discarded unreachable code: IL_001c
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				return androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
		}

		public void SetRationale(string rationale)
		{
			this.rationale = rationale;
		}

		internal void Fetch(string scope, bool fetchEmail, bool fetchAccessToken, bool fetchIdToken, Action<CommonStatusCodes> doneCallback)
		{
			if (apiAccessDenied)
			{
				if (apiWarningCount++ % apiWarningFreq == 0)
				{
					GooglePlayGames.OurUtils.Logger.w("Access to API denied");
					apiWarningCount = apiWarningCount / apiWarningFreq + 1;
				}
				doneCallback(CommonStatusCodes.AuthApiAccessForbidden);
				return;
			}
			PlayGamesHelperObject.RunOnGameThread(delegate
			{
				FetchToken(scope, playerId, rationale, fetchEmail, fetchAccessToken, fetchIdToken, delegate(int rc, string access, string id, string email)
				{
					if (rc != 0)
					{
						apiAccessDenied = rc == 3001 || rc == 16;
						GooglePlayGames.OurUtils.Logger.w("Non-success returned from fetch: " + rc);
						doneCallback(CommonStatusCodes.AuthApiAccessForbidden);
					}
					else
					{
						if (fetchAccessToken)
						{
							GooglePlayGames.OurUtils.Logger.d("a = " + access);
						}
						if (fetchEmail)
						{
							GooglePlayGames.OurUtils.Logger.d("email = " + email);
						}
						if (fetchIdToken)
						{
							GooglePlayGames.OurUtils.Logger.d("idt = " + id);
						}
						if (fetchAccessToken && !string.IsNullOrEmpty(access))
						{
							accessToken = access;
						}
						if (fetchIdToken && !string.IsNullOrEmpty(id))
						{
							idToken = id;
							idTokenCb(idToken);
						}
						if (fetchEmail && !string.IsNullOrEmpty(email))
						{
							accountName = email;
						}
						doneCallback(CommonStatusCodes.Success);
					}
				});
			});
		}

		internal static void FetchToken(string scope, string playerId, string rationale, bool fetchEmail, bool fetchAccessToken, bool fetchIdToken, Action<int, string, string, string> callback)
		{
			object[] args = new object[7];
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.games.bridge.TokenFragment"))
				{
					using (AndroidJavaObject androidJavaObject = GetActivity())
					{
						IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(androidJavaClass.GetRawClass(), "fetchToken", "(Landroid/app/Activity;Ljava/lang/String;Ljava/lang/String;ZZZLjava/lang/String;)Lcom/google/android/gms/common/api/PendingResult;");
						array[0].l = androidJavaObject.GetRawObject();
						array[1].l = AndroidJNI.NewStringUTF(playerId);
						array[2].l = AndroidJNI.NewStringUTF(rationale);
						array[3].z = fetchEmail;
						array[4].z = fetchAccessToken;
						array[5].z = fetchIdToken;
						array[6].l = AndroidJNI.NewStringUTF(scope);
						IntPtr ptr = AndroidJNI.CallStaticObjectMethod(androidJavaClass.GetRawClass(), staticMethodID, array);
						PendingResult<TokenResult> pendingResult = new PendingResult<TokenResult>(ptr);
						pendingResult.setResultCallback(new TokenResultCallback(callback));
					}
				}
			}
			catch (Exception ex)
			{
				GooglePlayGames.OurUtils.Logger.e("Exception launching token request: " + ex.Message);
				GooglePlayGames.OurUtils.Logger.e(ex.ToString());
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		private string GetAccountName(Action<CommonStatusCodes, string> callback)
		{
			if (string.IsNullOrEmpty(accountName))
			{
				if (!fetchingEmail)
				{
					fetchingEmail = true;
					Fetch(idTokenScope, true, false, false, delegate(CommonStatusCodes status)
					{
						fetchingEmail = false;
						if (callback != null)
						{
							callback(status, accountName);
						}
					});
				}
			}
			else if (callback != null)
			{
				callback(CommonStatusCodes.Success, accountName);
			}
			return accountName;
		}

		public string GetEmail()
		{
			return GetAccountName(null);
		}

		public void GetEmail(Action<CommonStatusCodes, string> callback)
		{
			GetAccountName(callback);
		}

		[Obsolete("Use PlayGamesPlatform.GetServerAuthCode()")]
		public string GetAccessToken()
		{
			if (string.IsNullOrEmpty(accessToken) && !fetchingAccessToken)
			{
				fetchingAccessToken = true;
				Fetch(idTokenScope, false, true, false, delegate
				{
					fetchingAccessToken = false;
				});
			}
			return accessToken;
		}

		[Obsolete("Use PlayGamesPlatform.GetServerAuthCode()")]
		public void GetIdToken(string serverClientId, Action<string> idTokenCallback)
		{
			if (string.IsNullOrEmpty(serverClientId))
			{
				if (webClientWarningCount++ % webClientWarningFreq == 0)
				{
					GooglePlayGames.OurUtils.Logger.w("serverClientId is empty, cannot get Id Token");
					webClientWarningCount = webClientWarningCount / webClientWarningFreq + 1;
				}
				idTokenCallback(null);
				return;
			}
			string text = "audience:server:client_id:" + serverClientId;
			if (string.IsNullOrEmpty(idToken) || text != idTokenScope)
			{
				if (fetchingIdToken)
				{
					return;
				}
				fetchingIdToken = true;
				idTokenScope = text;
				idTokenCb = idTokenCallback;
				Fetch(idTokenScope, false, false, true, delegate(CommonStatusCodes status)
				{
					fetchingIdToken = false;
					if (status == CommonStatusCodes.Success)
					{
						idTokenCb(null);
					}
					else
					{
						idTokenCb(idToken);
					}
				});
			}
			else
			{
				idTokenCallback(idToken);
			}
		}
	}
}

using System;
using Com.Google.Android.Gms.Common.Api;

namespace GooglePlayGames.Android
{
	internal class TokenResultCallback : ResultCallbackProxy<TokenResult>
	{
		private Action<int, string, string, string> callback;

		public TokenResultCallback(Action<int, string, string, string> callback)
		{
			this.callback = callback;
		}

		public override void OnResult(TokenResult arg_Result_1)
		{
			if (callback != null)
			{
				callback(arg_Result_1.getStatusCode(), arg_Result_1.getAccessToken(), arg_Result_1.getIdToken(), arg_Result_1.getEmail());
			}
		}

		public string toString()
		{
			return ToString();
		}
	}
}

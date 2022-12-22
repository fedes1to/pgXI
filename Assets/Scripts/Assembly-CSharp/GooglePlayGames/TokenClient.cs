using System;
using GooglePlayGames.BasicApi;

namespace GooglePlayGames
{
	internal interface TokenClient
	{
		string GetEmail();

		void GetEmail(Action<CommonStatusCodes, string> callback);

		string GetAccessToken();

		void GetIdToken(string serverClientId, Action<string> idTokenCallback);

		void SetRationale(string rationale);
	}
}

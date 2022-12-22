using System;
using System.Runtime.InteropServices;
using System.Text;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal class GameServices : BaseReferenceHolder
	{
		internal class FetchServerAuthCodeResponse : BaseReferenceHolder
		{
			internal FetchServerAuthCodeResponse(IntPtr selfPointer)
				: base(selfPointer)
			{
			}

			internal CommonErrorStatus.ResponseStatus Status()
			{
				return GooglePlayGames.Native.Cwrapper.GameServices.GameServices_FetchServerAuthCodeResponse_GetStatus(SelfPtr());
			}

			internal string Code()
			{
				return PInvokeUtilities.OutParamsToString((StringBuilder out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.GameServices.GameServices_FetchServerAuthCodeResponse_GetCode(SelfPtr(), out_string, out_size));
			}

			protected override void CallDispose(HandleRef selfPointer)
			{
				GooglePlayGames.Native.Cwrapper.GameServices.GameServices_FetchServerAuthCodeResponse_Dispose(selfPointer);
			}

			internal static FetchServerAuthCodeResponse FromPointer(IntPtr pointer)
			{
				if (pointer.Equals(IntPtr.Zero))
				{
					return null;
				}
				return new FetchServerAuthCodeResponse(pointer);
			}
		}

		internal GameServices(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal bool IsAuthenticated()
		{
			return GooglePlayGames.Native.Cwrapper.GameServices.GameServices_IsAuthorized(SelfPtr());
		}

		internal void SignOut()
		{
			GooglePlayGames.Native.Cwrapper.GameServices.GameServices_SignOut(SelfPtr());
		}

		internal void StartAuthorizationUI()
		{
			GooglePlayGames.Native.Cwrapper.GameServices.GameServices_StartAuthorizationUI(SelfPtr());
		}

		public AchievementManager AchievementManager()
		{
			return new AchievementManager(this);
		}

		public LeaderboardManager LeaderboardManager()
		{
			return new LeaderboardManager(this);
		}

		public PlayerManager PlayerManager()
		{
			return new PlayerManager(this);
		}

		public StatsManager StatsManager()
		{
			return new StatsManager(this);
		}

		internal HandleRef AsHandle()
		{
			return SelfPtr();
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.GameServices.GameServices_Dispose(selfPointer);
		}

		internal void FetchServerAuthCode(string server_client_id, Action<FetchServerAuthCodeResponse> callback)
		{
			Misc.CheckNotNull(callback);
			Misc.CheckNotNull(server_client_id);
			GooglePlayGames.Native.Cwrapper.GameServices.GameServices_FetchServerAuthCode(AsHandle(), server_client_id, InternalFetchServerAuthCodeCallback, Callbacks.ToIntPtr(callback, FetchServerAuthCodeResponse.FromPointer));
		}

		[MonoPInvokeCallback(typeof(GooglePlayGames.Native.Cwrapper.GameServices.FetchServerAuthCodeCallback))]
		private static void InternalFetchServerAuthCodeCallback(IntPtr response, IntPtr data)
		{
			Callbacks.PerformInternalCallback("GameServices#InternalFetchServerAuthCodeCallback", Callbacks.Type.Temporary, response, data);
		}
	}
}

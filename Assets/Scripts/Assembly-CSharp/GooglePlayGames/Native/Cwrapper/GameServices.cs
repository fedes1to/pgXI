using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class GameServices
	{
		internal delegate void FlushCallback(CommonErrorStatus.FlushStatus arg0, IntPtr arg1);

		internal delegate void FetchServerAuthCodeCallback(IntPtr arg0, IntPtr arg1);

		[DllImport("gpg")]
		internal static extern void GameServices_Flush(HandleRef self, FlushCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		internal static extern void GameServices_FetchServerAuthCode(HandleRef self, string server_client_id, FetchServerAuthCodeCallback callback, IntPtr callback_arg);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool GameServices_IsAuthorized(HandleRef self);

		[DllImport("gpg")]
		internal static extern void GameServices_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern void GameServices_SignOut(HandleRef self);

		[DllImport("gpg")]
		internal static extern void GameServices_StartAuthorizationUI(HandleRef self);

		[DllImport("gpg")]
		internal static extern void GameServices_FetchServerAuthCodeResponse_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern CommonErrorStatus.ResponseStatus GameServices_FetchServerAuthCodeResponse_GetStatus(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr GameServices_FetchServerAuthCodeResponse_GetCode(HandleRef self, StringBuilder out_arg, UIntPtr out_size);
	}
}

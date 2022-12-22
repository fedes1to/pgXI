using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GooglePlayGames.Native.Cwrapper
{
	internal static class Player
	{
		[DllImport("gpg")]
		internal static extern IntPtr Player_CurrentLevel(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr Player_Name(HandleRef self, StringBuilder out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern void Player_Dispose(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr Player_AvatarUrl(HandleRef self, Types.ImageResolution resolution, StringBuilder out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern ulong Player_LastLevelUpTime(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr Player_Title(HandleRef self, StringBuilder out_arg, UIntPtr out_size);

		[DllImport("gpg")]
		internal static extern ulong Player_CurrentXP(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool Player_Valid(HandleRef self);

		[DllImport("gpg")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool Player_HasLevelInfo(HandleRef self);

		[DllImport("gpg")]
		internal static extern IntPtr Player_NextLevel(HandleRef self);

		[DllImport("gpg")]
		internal static extern UIntPtr Player_Id(HandleRef self, StringBuilder out_arg, UIntPtr out_size);
	}
}

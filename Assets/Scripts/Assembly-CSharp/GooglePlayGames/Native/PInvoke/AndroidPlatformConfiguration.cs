using System;
using System.Runtime.InteropServices;
using AOT;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.Native.PInvoke
{
	internal sealed class AndroidPlatformConfiguration : PlatformConfiguration
	{
		private delegate void IntentHandlerInternal(IntPtr intent, IntPtr userData);

		private AndroidPlatformConfiguration(IntPtr selfPointer)
			: base(selfPointer)
		{
		}

		internal void SetActivity(IntPtr activity)
		{
			GooglePlayGames.Native.Cwrapper.AndroidPlatformConfiguration.AndroidPlatformConfiguration_SetActivity(SelfPtr(), activity);
		}

		internal void SetOptionalIntentHandlerForUI(Action<IntPtr> intentHandler)
		{
			Misc.CheckNotNull(intentHandler);
			GooglePlayGames.Native.Cwrapper.AndroidPlatformConfiguration.AndroidPlatformConfiguration_SetOptionalIntentHandlerForUI(SelfPtr(), InternalIntentHandler, Callbacks.ToIntPtr(intentHandler));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.AndroidPlatformConfiguration.AndroidPlatformConfiguration_Dispose(selfPointer);
		}

		[MonoPInvokeCallback(typeof(IntentHandlerInternal))]
		private static void InternalIntentHandler(IntPtr intent, IntPtr userData)
		{
			Callbacks.PerformInternalCallback("AndroidPlatformConfiguration#InternalIntentHandler", Callbacks.Type.Permanent, intent, userData);
		}

		internal static AndroidPlatformConfiguration Create()
		{
			return new AndroidPlatformConfiguration(GooglePlayGames.Native.Cwrapper.AndroidPlatformConfiguration.AndroidPlatformConfiguration_Construct());
		}
	}
}

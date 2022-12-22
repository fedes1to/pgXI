using System;
using System.Runtime.InteropServices;
using System.Text;
using GooglePlayGames.BasicApi.Nearby;
using GooglePlayGames.Native.Cwrapper;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeEndpointDetails : BaseReferenceHolder
	{
		internal NativeEndpointDetails(IntPtr pointer)
			: base(pointer)
		{
		}

		internal string EndpointId()
		{
			return PInvokeUtilities.OutParamsToString((StringBuilder out_arg, UIntPtr out_size) => NearbyConnectionTypes.EndpointDetails_GetEndpointId(SelfPtr(), out_arg, out_size));
		}

		internal string DeviceId()
		{
			return PInvokeUtilities.OutParamsToString((StringBuilder out_arg, UIntPtr out_size) => NearbyConnectionTypes.EndpointDetails_GetDeviceId(SelfPtr(), out_arg, out_size));
		}

		internal string Name()
		{
			return PInvokeUtilities.OutParamsToString((StringBuilder out_arg, UIntPtr out_size) => NearbyConnectionTypes.EndpointDetails_GetName(SelfPtr(), out_arg, out_size));
		}

		internal string ServiceId()
		{
			return PInvokeUtilities.OutParamsToString((StringBuilder out_arg, UIntPtr out_size) => NearbyConnectionTypes.EndpointDetails_GetServiceId(SelfPtr(), out_arg, out_size));
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			NearbyConnectionTypes.EndpointDetails_Dispose(selfPointer);
		}

		internal EndpointDetails ToDetails()
		{
			return new EndpointDetails(EndpointId(), DeviceId(), Name(), ServiceId());
		}

		internal static NativeEndpointDetails FromPointer(IntPtr pointer)
		{
			if (pointer.Equals(IntPtr.Zero))
			{
				return null;
			}
			return new NativeEndpointDetails(pointer);
		}
	}
}

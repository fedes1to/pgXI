using System;
using GooglePlayGames.BasicApi;
using GooglePlayGames.Native.Cwrapper;
using UnityEngine;

namespace GooglePlayGames.Native
{
	internal static class ConversionUtils
	{
		internal static ResponseStatus ConvertResponseStatus(CommonErrorStatus.ResponseStatus status)
		{
			switch (status)
			{
			case CommonErrorStatus.ResponseStatus.VALID:
				return ResponseStatus.Success;
			case CommonErrorStatus.ResponseStatus.VALID_BUT_STALE:
				return ResponseStatus.SuccessWithStale;
			case CommonErrorStatus.ResponseStatus.ERROR_INTERNAL:
				return ResponseStatus.InternalError;
			case CommonErrorStatus.ResponseStatus.ERROR_LICENSE_CHECK_FAILED:
				return ResponseStatus.LicenseCheckFailed;
			case CommonErrorStatus.ResponseStatus.ERROR_NOT_AUTHORIZED:
				return ResponseStatus.NotAuthorized;
			case CommonErrorStatus.ResponseStatus.ERROR_TIMEOUT:
				return ResponseStatus.Timeout;
			case CommonErrorStatus.ResponseStatus.ERROR_VERSION_UPDATE_REQUIRED:
				return ResponseStatus.VersionUpdateRequired;
			default:
				throw new InvalidOperationException("Unknown status: " + status);
			}
		}

		internal static CommonStatusCodes ConvertResponseStatusToCommonStatus(CommonErrorStatus.ResponseStatus status)
		{
			switch (status)
			{
			case CommonErrorStatus.ResponseStatus.VALID:
				return CommonStatusCodes.Success;
			case CommonErrorStatus.ResponseStatus.VALID_BUT_STALE:
				return CommonStatusCodes.SuccessCached;
			case CommonErrorStatus.ResponseStatus.ERROR_INTERNAL:
				return CommonStatusCodes.InternalError;
			case CommonErrorStatus.ResponseStatus.ERROR_LICENSE_CHECK_FAILED:
				return CommonStatusCodes.LicenseCheckFailed;
			case CommonErrorStatus.ResponseStatus.ERROR_NOT_AUTHORIZED:
				return CommonStatusCodes.AuthApiAccessForbidden;
			case CommonErrorStatus.ResponseStatus.ERROR_TIMEOUT:
				return CommonStatusCodes.Timeout;
			case CommonErrorStatus.ResponseStatus.ERROR_VERSION_UPDATE_REQUIRED:
				return CommonStatusCodes.ServiceVersionUpdateRequired;
			default:
				Debug.LogWarning(string.Concat("Unknown ResponseStatus: ", status, ", defaulting to CommonStatusCodes.Error"));
				return CommonStatusCodes.Error;
			}
		}

		internal static UIStatus ConvertUIStatus(CommonErrorStatus.UIStatus status)
		{
			switch (status)
			{
			case CommonErrorStatus.UIStatus.VALID:
				return UIStatus.Valid;
			case CommonErrorStatus.UIStatus.ERROR_INTERNAL:
				return UIStatus.InternalError;
			case CommonErrorStatus.UIStatus.ERROR_NOT_AUTHORIZED:
				return UIStatus.NotAuthorized;
			case CommonErrorStatus.UIStatus.ERROR_TIMEOUT:
				return UIStatus.Timeout;
			case CommonErrorStatus.UIStatus.ERROR_VERSION_UPDATE_REQUIRED:
				return UIStatus.VersionUpdateRequired;
			case CommonErrorStatus.UIStatus.ERROR_CANCELED:
				return UIStatus.UserClosedUI;
			case CommonErrorStatus.UIStatus.ERROR_UI_BUSY:
				return UIStatus.UiBusy;
			default:
				throw new InvalidOperationException("Unknown status: " + status);
			}
		}

		internal static GooglePlayGames.Native.Cwrapper.Types.DataSource AsDataSource(DataSource source)
		{
			switch (source)
			{
			case DataSource.ReadCacheOrNetwork:
				return GooglePlayGames.Native.Cwrapper.Types.DataSource.CACHE_OR_NETWORK;
			case DataSource.ReadNetworkOnly:
				return GooglePlayGames.Native.Cwrapper.Types.DataSource.NETWORK_ONLY;
			default:
				throw new InvalidOperationException("Found unhandled DataSource: " + source);
			}
		}
	}
}

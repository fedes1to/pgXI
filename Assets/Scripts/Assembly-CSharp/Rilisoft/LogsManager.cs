using System;
using UnityEngine;

namespace Rilisoft
{
	public static class LogsManager
	{
		public enum LoggingState
		{
			Default,
			EnabledByUserOrServer
		}

		public const string LoggingEnabledKey = "LogsManager.LoggingEnabledKey";

		private static LoggingState m_loggingState;

		public static LoggingState State
		{
			get
			{
				return m_loggingState;
			}
		}

		public static void Initialize()
		{
			m_loggingState = ((Storager.getInt("LogsManager.LoggingEnabledKey", false) == 1) ? LoggingState.EnabledByUserOrServer : LoggingState.Default);
			SetLoggingEnabledCore(true);
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				try
				{
					Debug.logger.logHandler = new IosLogsHandler();
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Exception in setting IosLogsHandler: {0}", ex);
				}
			}
		}

		public static void DisableLogsIfAllowed()
		{
			if (m_loggingState != LoggingState.EnabledByUserOrServer)
			{
				try
				{
					SetLoggingEnabledCore(false);
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Exception in DisableLogsIfAllowed: {0}", ex);
				}
			}
		}

		public static void EnableLogsFromServer()
		{
			m_loggingState = LoggingState.EnabledByUserOrServer;
			try
			{
				SetLoggingEnabledCore(true);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in EnableLogsFromServer: {0}", ex);
			}
		}

		public static void SetLoggingEnabled(bool enabled)
		{
			m_loggingState = (enabled ? LoggingState.EnabledByUserOrServer : LoggingState.Default);
			try
			{
				SetLoggingEnabledCore(enabled);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception in SetLoggingEnabled: {0}", ex);
			}
			Storager.setInt("LogsManager.LoggingEnabledKey", enabled ? 1 : 0, false);
		}

		private static void Application_logMessageReceived(string condition, string stackTrace, LogType type)
		{
			if (type == LogType.Exception)
			{
				string text = string.Format("EXCEPTION: {0}\n{1}", condition ?? string.Empty, stackTrace ?? string.Empty);
				if (text != null)
				{
					IosLogsHandler.LogIos(text);
				}
			}
		}

		private static void SetLoggingEnabledCore(bool enabled)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				Application.logMessageReceived -= Application_logMessageReceived;
				if (enabled)
				{
					Application.logMessageReceived += Application_logMessageReceived;
				}
			}
			Debug.logger.logEnabled = enabled || Application.isEditor;
		}
	}
}

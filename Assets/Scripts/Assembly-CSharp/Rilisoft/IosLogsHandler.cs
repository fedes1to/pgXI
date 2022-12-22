using System;
using UnityEngine;

namespace Rilisoft
{
	public sealed class IosLogsHandler : ILogHandler
	{
		public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
		{
			if (format == null || args == null)
			{
				return;
			}
			string text = string.Format(format, args);
			if (text != null)
			{
				string arg = Environment.StackTrace ?? "empty stack trace";
				string text2 = string.Format("{0}: {1}\n{2}", logType.ToString(), text, arg);
				if (text2 != null)
				{
					LogIos(text2);
				}
			}
		}

		public void LogException(Exception exception, UnityEngine.Object context)
		{
			if (exception == null)
			{
				return;
			}
			try
			{
				LogFormat(LogType.Exception, context, exception.ToString());
			}
			catch (Exception)
			{
				int num = 10;
			}
		}

		public static void LogIos(string message)
		{
		}
	}
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;

namespace FlyingWormConsole3
{
	public class ConsoleProRemoteServer : MonoBehaviour
	{
		public class HTTPContext
		{
			public HttpListenerContext context;

			public string path;

			public string Command
			{
				get
				{
					return WWW.UnEscapeURL(context.Request.Url.AbsolutePath);
				}
			}

			public HttpListenerRequest Request
			{
				get
				{
					return context.Request;
				}
			}

			public HttpListenerResponse Response
			{
				get
				{
					return context.Response;
				}
			}

			public HTTPContext(HttpListenerContext inContext)
			{
				context = inContext;
			}

			public void RespondWithString(string inString)
			{
				Response.StatusDescription = "OK";
				Response.StatusCode = 200;
				if (!string.IsNullOrEmpty(inString))
				{
					Response.ContentType = "text/plain";
					byte[] bytes = Encoding.UTF8.GetBytes(inString);
					Response.ContentLength64 = bytes.Length;
					Response.OutputStream.Write(bytes, 0, bytes.Length);
				}
			}
		}

		[Serializable]
		public class QueuedLog
		{
			public string message;

			public string stackTrace;

			public LogType type;
		}

		public int port = 51000;

		private static HttpListener listener = new HttpListener();

		[NonSerialized]
		public List<QueuedLog> logs = new List<QueuedLog>();

		private static ConsoleProRemoteServer instance = null;

		private void Awake()
		{
			if (instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			Debug.Log("Starting Console Pro Server on port : " + port);
			listener.Prefixes.Add("http://*:" + port + "/");
			listener.Start();
			listener.BeginGetContext(ListenerCallback, null);
		}

		private void OnEnable()
		{
			Application.logMessageReceived += LogCallback;
		}

		private void OnDisable()
		{
			Application.logMessageReceived -= LogCallback;
		}

		public void LogCallback(string logString, string stackTrace, LogType type)
		{
			if (!logString.StartsWith("CPIGNORE"))
			{
				QueueLog(logString, stackTrace, type);
			}
		}

		private void QueueLog(string logString, string stackTrace, LogType type)
		{
			logs.Add(new QueuedLog
			{
				message = logString,
				stackTrace = stackTrace,
				type = type
			});
		}

		private void ListenerCallback(IAsyncResult result)
		{
			HTTPContext context = new HTTPContext(listener.EndGetContext(result));
			HandleRequest(context);
			listener.BeginGetContext(ListenerCallback, null);
		}

		private void HandleRequest(HTTPContext context)
		{
			bool flag = false;
			switch (context.Command)
			{
			case "/NewLogs":
				flag = true;
				if (logs.Count > 0)
				{
					string text = string.Empty;
					for (int i = 0; i < logs.Count; i++)
					{
						QueuedLog queuedLog = logs[i];
						text = text + "::::" + queuedLog.type;
						text = text + "||||" + queuedLog.message;
						text = text + ">>>>" + queuedLog.stackTrace + ">>>>";
					}
					context.RespondWithString(text);
					logs.Clear();
				}
				else
				{
					context.RespondWithString(string.Empty);
				}
				break;
			}
			if (!flag)
			{
				context.Response.StatusCode = 404;
				context.Response.StatusDescription = "Not Found";
			}
			context.Response.OutputStream.Close();
		}
	}
}

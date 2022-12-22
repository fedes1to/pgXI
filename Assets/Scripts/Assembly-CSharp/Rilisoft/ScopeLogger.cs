using System;
using UnityEngine;

namespace Rilisoft
{
	internal struct ScopeLogger : IDisposable
	{
		private string _callee;

		private string _caller;

		private bool _enabled;

		private bool _initialized;

		private readonly int _startFrame;

		private readonly float _startTime;

		public ScopeLogger(string caller, string callee, bool enabled)
		{
			_caller = caller ?? string.Empty;
			_callee = callee ?? string.Empty;
			_enabled = enabled;
			if (_enabled)
			{
				_startTime = Time.realtimeSinceStartup;
				_startFrame = Time.frameCount;
				string text = ((!string.IsNullOrEmpty(_caller)) ? "{0} > {1}: {2:f3}, {3}" : "> {1}: {2:f3}, {3}");
				string format = ((!Application.isEditor) ? text : ("<color=orange>" + text + "</color>"));
				Debug.LogFormat(format, _caller, _callee, _startTime, _startFrame);
			}
			else
			{
				_startTime = 0f;
				_startFrame = 0;
			}
			_initialized = true;
		}

		public ScopeLogger(string callee, bool enabled)
			: this(string.Empty, callee, enabled)
		{
		}

		public void Dispose()
		{
			if (_initialized)
			{
				if (_enabled)
				{
					string text = ((!string.IsNullOrEmpty(_caller)) ? "{0} < {1}: +{2:f3}, +{3}" : "< {1}: +{2:f3}, +{3}");
					string format = ((!Application.isEditor) ? text : ("<color=orange>" + text + "</color>"));
					Debug.LogFormat(format, _caller, _callee, Time.realtimeSinceStartup - _startTime, Time.frameCount - _startFrame);
				}
				_callee = string.Empty;
				_caller = string.Empty;
				_initialized = false;
			}
		}
	}
}

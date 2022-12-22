using UnityEngine;

namespace Rilisoft
{
	public class WaitForRealSeconds : CustomYieldInstruction
	{
		private float _waitSeconds;

		private float _elapsed;

		private float _prevRealTime;

		public override bool keepWaiting
		{
			get
			{
				_elapsed += Time.realtimeSinceStartup - _prevRealTime;
				_prevRealTime = Time.realtimeSinceStartup;
				return _elapsed < _waitSeconds;
			}
		}

		public WaitForRealSeconds(float seconds)
		{
			_prevRealTime = Time.realtimeSinceStartup;
			_waitSeconds = Mathf.Abs(seconds);
		}
	}
}

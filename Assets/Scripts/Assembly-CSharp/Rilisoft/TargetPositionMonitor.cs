using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rilisoft
{
	public class TargetPositionMonitor : MonoBehaviour
	{
		private readonly List<Vector3> _path = new List<Vector3>();

		private float _checkTimeElapsed;

		private int _lastIdx;

		public float MinDistance { get; private set; }

		public float CheckInterval { get; private set; }

		public bool Enabled { get; private set; }

		public Func<Vector3> TargetPositionGetter { get; private set; }

		public void StartMonitoring(Func<Vector3> targetPositionGetter, float minDistance = 0.1f, float checkInterval = 0.1f)
		{
			TargetPositionGetter = null;
			_path.Clear();
			_checkTimeElapsed = 0f;
			_lastIdx = 0;
			if (targetPositionGetter == null)
			{
				Enabled = false;
				return;
			}
			TargetPositionGetter = targetPositionGetter;
			MinDistance = minDistance;
			CheckInterval = checkInterval;
			_path.Add(TargetPositionGetter());
			Enabled = true;
		}

		public void Reset()
		{
			if (Enabled)
			{
				_path.Clear();
				_checkTimeElapsed = 0f;
				_lastIdx = 0;
				_path.Add(TargetPositionGetter());
			}
		}

		public void StopMonitoring()
		{
			_path.Clear();
			Enabled = false;
		}

		public bool HasNextPoint()
		{
			return _path.Count - 1 > _lastIdx;
		}

		public Vector3 GetCurrentPoint()
		{
			return _path[_lastIdx];
		}

		public Vector3 GetNextPoint()
		{
			_lastIdx++;
			return _path[_lastIdx];
		}

		private void Update()
		{
			if (!Enabled || TargetPositionGetter == null)
			{
				return;
			}
			_checkTimeElapsed += Time.deltaTime;
			if (!(_checkTimeElapsed < CheckInterval))
			{
				_checkTimeElapsed = 0f;
				Vector3 vector = _path[_path.Count - 1];
				if (Mathf.Abs((vector - TargetPositionGetter()).sqrMagnitude) >= MinDistance * 2f)
				{
					_path.Add(TargetPositionGetter());
				}
			}
		}
	}
}

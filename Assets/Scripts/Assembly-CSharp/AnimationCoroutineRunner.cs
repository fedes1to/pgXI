using System;
using System.Collections;
using UnityEngine;

public class AnimationCoroutineRunner : MonoBehaviour
{
	public void StartPlay(Animation animation, string clipName, bool useTimeScale, Action onComplete)
	{
		StartCoroutine(Play(animation, clipName, useTimeScale, onComplete));
	}

	public IEnumerator Play(Animation animation, string clipName, bool useTimeScale, Action onComplete)
	{
		if (!useTimeScale)
		{
			AnimationState _currState = animation[clipName];
			bool isPlaying = true;
			float _progressTime = 0f;
			float _timeAtLastFrame2 = 0f;
			float _timeAtCurrentFrame2 = 0f;
			float deltaTime2 = 0f;
			animation.Play(clipName);
			_timeAtLastFrame2 = Time.realtimeSinceStartup;
			while (isPlaying)
			{
				try
				{
					_timeAtCurrentFrame2 = Time.realtimeSinceStartup;
					deltaTime2 = _timeAtCurrentFrame2 - _timeAtLastFrame2;
					_timeAtLastFrame2 = _timeAtCurrentFrame2;
					_progressTime += deltaTime2;
					_currState.normalizedTime = _progressTime / _currState.length;
					animation.Sample();
					if (_progressTime >= _currState.length)
					{
						if (_currState.wrapMode != WrapMode.Loop)
						{
							isPlaying = false;
						}
						else
						{
							_progressTime = 0f;
						}
					}
				}
				catch (Exception e)
				{
					Debug.LogErrorFormat("Exception in AnimationCoroutineRunner Play: {0}", e);
				}
				yield return new WaitForEndOfFrame();
				if (this == null || base.gameObject == null)
				{
					yield break;
				}
			}
			yield return null;
			if (onComplete != null)
			{
				onComplete();
			}
		}
		else
		{
			animation.Play(clipName);
		}
	}
}

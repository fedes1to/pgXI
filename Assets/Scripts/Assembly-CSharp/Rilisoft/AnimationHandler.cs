using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class AnimationHandler : MonoBehaviour
	{
		public enum AnimState
		{
			Started,
			Finished,
			Custom1
		}

		private class Subscriber
		{
			public string AnimationName;

			public AnimState ToState;

			public bool CallOnce;

			public Action Callback;
		}

		private List<AnimationClip> clips = new List<AnimationClip>();

		private List<Subscriber> _subscribers = new List<Subscriber>();

		private List<Subscriber> _toRemove = new List<Subscriber>();

		public event Action<string, AnimState> OnAnimationEvent;

		public void SubscribeTo(string animationName, AnimState toState, bool callOnce, Action callback)
		{
			_subscribers.Add(new Subscriber
			{
				AnimationName = animationName,
				ToState = toState,
				CallOnce = callOnce,
				Callback = callback
			});
		}

		public void Unsubscribe(string animationName, AnimState toState, Action callback)
		{
			Subscriber[] array = _subscribers.Where((Subscriber s) => s.AnimationName == animationName && s.ToState == toState && s.Callback == callback).ToArray();
			Subscriber[] array2 = array;
			foreach (Subscriber item in array2)
			{
				_subscribers.Remove(item);
			}
		}

		public bool AnimationIsExists(string animName)
		{
			if (animName.IsNullOrEmpty())
			{
				return false;
			}
			return clips.Any((AnimationClip c) => c.name == animName);
		}

		private void Awake()
		{
			clips.Clear();
			Animation component = GetComponent<Animation>();
			if (component != null)
			{
				foreach (AnimationState item in component)
				{
					clips.Add(item.clip);
				}
			}
			else
			{
				Animator component2 = GetComponent<Animator>();
				if (component2 != null)
				{
					clips = component2.runtimeAnimatorController.animationClips.ToList();
				}
			}
			if (!clips.Any())
			{
				Debug.LogError("animations not found");
				return;
			}
			foreach (AnimationClip clip in clips)
			{
				bool flag = false;
				bool flag2 = false;
				AnimationEvent[] events = clip.events;
				foreach (AnimationEvent animationEvent in events)
				{
					if (animationEvent.time == 0f && animationEvent.functionName == AnimState.Started.ToString())
					{
						flag = true;
					}
					if (animationEvent.time == 0f && animationEvent.functionName == AnimState.Finished.ToString())
					{
						flag2 = true;
					}
				}
				if (!flag)
				{
					AnimationEvent animationEvent2 = new AnimationEvent();
					animationEvent2.time = 0f;
					animationEvent2.functionName = AnimState.Started.ToString();
					animationEvent2.stringParameter = clip.name;
					AnimationEvent evt = animationEvent2;
					clip.AddEvent(evt);
				}
				if (!flag2)
				{
					AnimationEvent animationEvent2 = new AnimationEvent();
					animationEvent2.time = clip.length;
					animationEvent2.functionName = AnimState.Finished.ToString();
					animationEvent2.stringParameter = clip.name;
					AnimationEvent evt2 = animationEvent2;
					clip.AddEvent(evt2);
				}
				flag = false;
				flag2 = false;
			}
		}

		private void Started(string animationName)
		{
			InvokeSubscribers(animationName, AnimState.Started);
			if (this.OnAnimationEvent != null)
			{
				this.OnAnimationEvent(animationName, AnimState.Started);
			}
		}

		private void Finished(string animationName)
		{
			InvokeSubscribers(animationName, AnimState.Finished);
			if (this.OnAnimationEvent != null)
			{
				this.OnAnimationEvent(animationName, AnimState.Finished);
			}
		}

		private void CustomCall(string animName_param)
		{
			string[] array = animName_param.Split('_');
			if (array.Length != 2)
			{
				return;
			}
			AnimState? animState = array[1].ToEnum<AnimState>();
			if (animState.HasValue)
			{
				string text = array[0];
				InvokeSubscribers(text, animState.Value);
				if (this.OnAnimationEvent != null)
				{
					this.OnAnimationEvent(text, animState.Value);
				}
			}
		}

		private void InvokeSubscribers(string animationName, AnimState state)
		{
			for (int num = _subscribers.Count - 1; num >= 0; num--)
			{
				if (_subscribers.Count - 1 >= num)
				{
					Subscriber subscriber = _subscribers[num];
					if (subscriber.AnimationName == animationName && subscriber.ToState == state)
					{
						if (subscriber.Callback != null)
						{
							subscriber.Callback();
						}
						if (subscriber.CallOnce || subscriber.Callback == null)
						{
							_toRemove.Add(subscriber);
						}
					}
					foreach (Subscriber item in _toRemove)
					{
						_subscribers.Remove(item);
					}
					_toRemove.Clear();
				}
			}
		}
	}
}

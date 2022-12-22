using UnityEngine;
using UnityEngine.Events;

namespace Rilisoft
{
	public class PetEffectHandler : MonoBehaviour
	{
		public float WaitTime = 1f;

		public UnityEvent OnEffectCompleted;

		private float _timeElapsed;

		private Transform _thisTransform;

		public bool IsPlaying
		{
			get
			{
				return base.gameObject.activeInHierarchy;
			}
		}

		private void OnEnable()
		{
			_thisTransform = base.transform;
			_timeElapsed = 0f;
		}

		private void OnDisable()
		{
			OnEffectCompleted.Invoke();
		}

		private void Update()
		{
			_thisTransform.rotation = Quaternion.Euler(new Vector3(0f, _thisTransform.rotation.y, 0f));
			_timeElapsed += Time.deltaTime;
			if (_timeElapsed >= WaitTime)
			{
				base.gameObject.SetActiveSafe(false);
			}
		}

		public void Play()
		{
			base.gameObject.SetActiveSafe(true);
		}
	}
}

using System.Collections;
using UnityEngine;

namespace Rilisoft
{
	public class DirectionViewerTarget : MonoBehaviour
	{
		[SerializeField]
		private DirectionViewTargetType _Type;

		[ReadOnly]
		[SerializeField]
		private Rocket _rocketComponent;

		public DirectionViewTargetType Type
		{
			get
			{
				return _Type;
			}
		}

		public Transform Transform
		{
			get
			{
				return base.gameObject.transform;
			}
		}

		private void OnEnable()
		{
			switch (Type)
			{
			case DirectionViewTargetType.Grenade:
				StartCoroutine(RocketMonitorCoroutine());
				break;
			case DirectionViewTargetType.Pet:
				StartCoroutine(PetMonitorCoroutine());
				break;
			}
		}

		private void OnDisable()
		{
			HidePointer();
		}

		private IEnumerator RocketMonitorCoroutine()
		{
			while (_rocketComponent == null)
			{
				_rocketComponent = base.transform.root.GetComponentInParent<Rocket>();
				yield return null;
			}
			while (!_rocketComponent.isRun)
			{
				yield return null;
			}
			ShowPointer();
			while (_rocketComponent.isRun)
			{
				yield return null;
			}
			HidePointer();
		}

		private IEnumerator PetMonitorCoroutine()
		{
			ShowPointer();
			yield return null;
		}

		private void ShowPointer()
		{
			CoroutineRunner.WaitUntil(() => DirectionViewer.Instance != null, delegate
			{
				DirectionViewer.Instance.LookToMe(this);
			});
		}

		private void HidePointer()
		{
			if (DirectionViewer.Instance != null)
			{
				DirectionViewer.Instance.ForgetMe(this);
			}
		}
	}
}

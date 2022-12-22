using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	public class DirectionViewer : MonoBehaviour
	{
		[SerializeField]
		private UIPanel _panel;

		[SerializeField]
		private List<DirectionViewerSettings> _settings = new List<DirectionViewerSettings>
		{
			new DirectionViewerSettings
			{
				ForType = DirectionViewTargetType.Grenade,
				LookRadius = 10f,
				CircleRadius = 200f
			},
			new DirectionViewerSettings
			{
				ForType = DirectionViewTargetType.Pet,
				LookRadius = 1000f,
				CircleRadius = 400f
			}
		};

		private readonly Dictionary<DirectionViewTargetType, Queue<DirectionPointer>> _freePointers = new Dictionary<DirectionViewTargetType, Queue<DirectionPointer>>();

		private readonly List<DirectionPointer> _activePointers = new List<DirectionPointer>();

		public static DirectionViewer Instance { get; private set; }

		private void Awake()
		{
			if (Instance != null)
			{
				return;
			}
			Instance = this;
			_activePointers.Clear();
			_freePointers.Clear();
			List<DirectionPointer> list = GetComponentsInChildren<DirectionPointer>(true).ToList();
			foreach (DirectionPointer item in list)
			{
				if (_freePointers.ContainsKey(item.ForPointerType))
				{
					_freePointers[item.ForPointerType].Enqueue(item);
					continue;
				}
				Queue<DirectionPointer> queue = new Queue<DirectionPointer>();
				queue.Enqueue(item);
				_freePointers.Add(item.ForPointerType, queue);
			}
		}

		private void OnDisable()
		{
			_activePointers.ForEach(delegate(DirectionPointer p)
			{
				ForgetPointer(p);
			});
		}

		private void LateUpdate()
		{
			if (!(WeaponManager.sharedManager == null))
			{
				int count = _activePointers.Count;
				for (int i = 0; i < count; i++)
				{
					DirectionPointer pointerState = _activePointers[i];
					SetPointerState(pointerState);
				}
			}
		}

		public void LookToMe(DirectionViewerTarget target)
		{
			if (!(target == null) && !_activePointers.Any((DirectionPointer p) => p.Target == target) && _freePointers.ContainsKey(target.Type) && _freePointers[target.Type].Any())
			{
				DirectionPointer directionPointer = _freePointers[target.Type].Dequeue();
				_activePointers.Add(directionPointer);
				directionPointer.TurnOn(target);
			}
		}

		public void ForgetMe(DirectionViewerTarget target)
		{
			DirectionPointer directionPointer = _activePointers.FirstOrDefault((DirectionPointer p) => p.Target == target);
			if (!(directionPointer == null))
			{
				directionPointer.TurnOff();
				_activePointers.Remove(directionPointer);
				_freePointers[directionPointer.ForPointerType].Enqueue(directionPointer);
			}
		}

		private void ForgetPointer(DirectionPointer pointer)
		{
			pointer.TurnOff();
			if (_activePointers.Contains(pointer))
			{
				_activePointers.Remove(pointer);
				_freePointers[pointer.ForPointerType].Enqueue(pointer);
			}
		}

		private bool CheckDistance(DirectionViewerTarget poiner)
		{
			if (WeaponManager.sharedManager == null || poiner == null)
			{
				return false;
			}
			return (WeaponManager.sharedManager.myPlayer.transform.position - poiner.Transform.position).sqrMagnitude < Mathf.Pow(GetSettings(poiner.Type).LookRadius, 2f);
		}

		private void SetPointerState(DirectionPointer pointer)
		{
			if (!CheckDistance(pointer.Target))
			{
				pointer.OutOfRange = true;
				pointer.Hide();
				return;
			}
			if (pointer.OutOfRange)
			{
				pointer.OutOfRange = false;
				pointer.TurnOn(pointer.Target);
			}
			float angle = GetAngle(NickLabelController.currentCamera.transform, pointer.Target.transform.position, Vector3.up);
			float circleRadius = GetSettings(pointer.ForPointerType).CircleRadius;
			Vector3 localPosition = new Vector3(circleRadius * Mathf.Sin(angle * ((float)Math.PI / 180f)), circleRadius * Mathf.Cos(angle * ((float)Math.PI / 180f)), pointer.transform.position.z);
			pointer.transform.localPosition = localPosition;
		}

		private float GetAngle(Transform from, Vector3 target, Vector3 n)
		{
			Vector3 forward = from.forward;
			Vector3 rhs = target - from.position;
			return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(forward, rhs)), Vector3.Dot(forward, rhs)) * 57.29578f;
		}

		private DirectionViewerSettings GetSettings(DirectionViewTargetType type)
		{
			return _settings.First((DirectionViewerSettings s) => s.ForType == type);
		}
	}
}

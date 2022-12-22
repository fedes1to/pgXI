using System.Collections;
using UnityEngine;

namespace Rilisoft
{
	public class FlyingPetEngine : PetEngine
	{
		private class PetMoveToOwnerState : State<PetState>
		{
			private const float _resetMonitorInterval = 0.3f;

			private FlyingPetEngine ctx;

			private float _resetMonitorTimeElapsed;

			public PetMoveToOwnerState(FlyingPetEngine context)
				: base(PetState.moveToOwner, (StateMachine<PetState>)context)
			{
				ctx = context;
			}

			public override void In(PetState fromState)
			{
				base.In(fromState);
				if (!ctx.IsVisible(ctx.Owner.gameObject))
				{
					To(PetState.teleport);
					return;
				}
				ctx.TargetPosMon.StartMonitoring(() => ctx.Owner.transform.position);
				ctx.CheckIsMovingStart();
			}

			public override void Update()
			{
				base.Update();
				if (ctx.Owner == null || !ctx.IsAlive)
				{
					To(PetState.idle);
					return;
				}
				if (!ctx.InRange(ctx.MovePosition, ctx.ThisTransform.position, ctx.Info.MaxToOwnerDistance) || ctx.Owner.isKilled)
				{
					To(PetState.teleport);
					return;
				}
				if (ctx.InRange(ctx.ThisTransform.position, ctx.MovePosition, ctx.Info.MinToOwnerDistance))
				{
					To(PetState.idle);
					return;
				}
				if (ctx.Target != null)
				{
					To(PetState.moveToTarget);
					return;
				}
				_resetMonitorTimeElapsed += Time.deltaTime;
				if (_resetMonitorTimeElapsed >= 0.3f)
				{
					_resetMonitorTimeElapsed = 0f;
					if (ctx.IsVisible(ctx.Owner.gameObject))
					{
						ctx.TargetPosMon.Reset();
					}
				}
				ctx.Move();
			}

			public override void Out(PetState toState)
			{
				base.Out(toState);
				ctx.TargetPosMon.StopMonitoring();
				_resetMonitorTimeElapsed = 0f;
				ctx.CheckIsMovingStop();
			}
		}

		private class PetMoveToTargetState : State<PetState>
		{
			private const float _resetMonitorInterval = 0.3f;

			private FlyingPetEngine ctx;

			private float _resetMonitorTimeElapsed;

			public PetMoveToTargetState(FlyingPetEngine context)
				: base(PetState.moveToTarget, (StateMachine<PetState>)context)
			{
				ctx = context;
			}

			public override void In(PetState fromState)
			{
				base.In(fromState);
				if (ctx.Target == null)
				{
					To(PetState.idle);
					return;
				}
				ctx.TargetPosMon.StartMonitoring(() => ctx.MoveToTargetPosition.Value);
				ctx.CheckIsMovingStart();
			}

			public override void Update()
			{
				base.Update();
				if (ctx.Target == null || !ctx.IsAlive || !ctx.MoveToTargetPosition.HasValue)
				{
					To(PetState.idle);
					return;
				}
				if (!ctx.InRange(ctx.ThisTransform.position, ctx.Owner.transform.position, ctx.Info.MaxToOwnerDistance))
				{
					To(PetState.teleport);
					return;
				}
				if (ctx.Owner.isKilled)
				{
					To(PetState.teleport);
					return;
				}
				if (ctx.Target != null && ctx.InRange(ctx.ThisTransform.position, ctx.MoveToTargetPosition.Value, ctx.Info.AttackDistance))
				{
					if (!ctx.InAttackState)
					{
						To(PetState.attack);
					}
					return;
				}
				_resetMonitorTimeElapsed += Time.deltaTime;
				if (_resetMonitorTimeElapsed >= 0.3f)
				{
					_resetMonitorTimeElapsed = 0f;
					if (ctx.IsVisible(ctx.Target.gameObject))
					{
						ctx.TargetPosMon.Reset();
					}
				}
				ctx.Move();
			}

			public override void Out(PetState toState)
			{
				base.Out(toState);
				ctx.TargetPosMon.StopMonitoring();
				_resetMonitorTimeElapsed = 0f;
				ctx.CheckIsMovingStop();
			}
		}

		private const float _unwaikableTeleportTime = 1f;

		public CharacterController Character;

		private TargetPositionMonitor _posMonVal;

		private Collider _characterControllerCollider;

		private float _unwaikableElapsedTime;

		public override Vector3 MovePosition
		{
			get
			{
				return (!(base.Owner.GetPointForFlyingPet() != null)) ? base.Owner.transform.position : base.Owner.GetPointForFlyingPet().position;
			}
		}

		protected override Vector3? MoveToTargetPosition
		{
			get
			{
				if (Target != null)
				{
					return new Vector3(Target.position.x, Target.position.y + 1.5f, Target.position.z);
				}
				return null;
			}
		}

		protected override State<PetState> MoveToOwnerState
		{
			get
			{
				return new PetMoveToOwnerState(this);
			}
		}

		protected override State<PetState> MoveToTargetState
		{
			get
			{
				return new PetMoveToTargetState(this);
			}
		}

		private TargetPositionMonitor TargetPosMon
		{
			get
			{
				if (_posMonVal == null)
				{
					_posMonVal = GetComponent<TargetPositionMonitor>() ?? base.gameObject.AddComponent<TargetPositionMonitor>();
				}
				return _posMonVal;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			Character = GetComponent<CharacterController>();
			Collider[] components = base.gameObject.GetComponents<Collider>();
			foreach (Collider collider in components)
			{
				if (collider != BodyCollider)
				{
					_characterControllerCollider = collider;
				}
			}
		}

		protected override void StopEngine()
		{
			base.StopEngine();
			Character.enabled = false;
		}

		protected override void InitSM()
		{
			base.InitSM();
			if (Character != null)
			{
				Character.enabled = base.IsMine;
			}
		}

		public void Move()
		{
			Vector3 vector = TargetPosMon.GetCurrentPoint();
			if (Vector3.Distance(ThisTransform.position, vector) < 0.2f)
			{
				vector = ((!TargetPosMon.HasNextPoint()) ? Vector3.zero : TargetPosMon.GetNextPoint());
			}
			if (!(vector == Vector3.zero))
			{
				Quaternion b = Quaternion.LookRotation(vector - base.transform.position, Vector3.up);
				ThisTransform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 10f);
				Vector3 normalized = (vector - ThisTransform.position).normalized;
				if (base.CurrentState.StateId == PetState.moveToTarget)
				{
					normalized *= Mathf.Clamp(GetCalculatedSpeedMultyplier() * base.Info.SpeedModif, 0f, base.Info.SpeedModif) * 0.015f;
				}
				else
				{
					normalized *= GetCalculatedSpeedMultyplier() * base.Info.SpeedModif * 0.015f;
				}
				Character.Move(normalized);
				PlaySound(ClipWalk);
			}
		}

		private IEnumerator CheckIsMoving()
		{
			_unwaikableElapsedTime = 0f;
			while (true)
			{
				if (base.IsMoving)
				{
					_unwaikableElapsedTime = 0f;
				}
				else
				{
					_unwaikableElapsedTime += Time.deltaTime;
					if (_unwaikableElapsedTime >= 1f)
					{
						_unwaikableElapsedTime = 0f;
						To(PetState.teleport);
						StopCoroutine("CheckIsMoving");
					}
				}
				yield return null;
			}
		}

		public void CheckIsMovingStart()
		{
			StopCoroutine("CheckIsMoving");
			StartCoroutine("CheckIsMoving");
		}

		public void CheckIsMovingStop()
		{
			StopCoroutine("CheckIsMoving");
		}

		public override void SetCollidersEnabled(bool enabled)
		{
			base.SetCollidersEnabled(enabled);
			if (_characterControllerCollider.enabled != enabled)
			{
				_characterControllerCollider.enabled = enabled;
			}
		}
	}
}

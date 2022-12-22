using UnityEngine;
using UnityEngine.AI;

namespace Rilisoft
{
	public class GroundPetEngine : PetEngine
	{
		private class PetMoveToOwnerState : State<PetState>
		{
			private GroundPetEngine ctx;

			public PetMoveToOwnerState(PetEngine context)
				: base(PetState.moveToOwner, (StateMachine<PetState>)context)
			{
				ctx = context as GroundPetEngine;
			}

			public override void Update()
			{
				base.Update();
				if (ctx.Owner == null || !ctx.IsAlive)
				{
					To(PetState.idle);
				}
				else if (ctx.Owner.isKilled)
				{
					To(PetState.teleport);
				}
				else if (ctx.InRange(ctx.ThisTransform.position, ctx.MovePosition, ctx.Info.MinToOwnerDistance))
				{
					To(PetState.idle);
				}
				else if (ctx.Target != null)
				{
					To(PetState.moveToTarget);
				}
				else if (!ctx.InRange(ctx.MovePosition, ctx.ThisTransform.position, ctx.Info.MaxToOwnerDistance))
				{
					To(PetState.teleport);
				}
				else if (ctx.Nma.enabled && !ctx.InRange(ctx.MovePosition, ctx.ThisTransform.position, ctx.Info.MinToOwnerDistance))
				{
					if (ctx.CanMoveToPlayer || !ctx.OwnerIsGrounded)
					{
						if (!ctx.Nma.isOnOffMeshLink)
						{
							NavMesh.CalculatePath(ctx.gameObject.transform.position, ctx.Owner.myPlayerTransform.position, -1, ctx.Path);
							ctx.Nma.SetPath(ctx.Path);
							ctx.Nma.speed = ctx.GetCalculatedSpeedMultyplier() * ctx.Info.SpeedModif;
							ctx.Nma.Resume();
						}
					}
					else
					{
						To(PetState.teleport);
					}
				}
				else
				{
					To(PetState.teleport);
				}
			}

			public override void Out(PetState toState)
			{
				base.Out(toState);
				ctx.Nma.ResetPath();
				ctx.Nma.Stop();
			}
		}

		private class PetMoveToTargetState : State<PetState>
		{
			private GroundPetEngine ctx;

			public PetMoveToTargetState(PetEngine context)
				: base(PetState.moveToTarget, (StateMachine<PetState>)context)
			{
				ctx = context as GroundPetEngine;
			}

			public override void In(PetState fromState)
			{
				base.In(fromState);
				if (ctx.Target == null)
				{
					ctx.Nma.Stop();
					To(PetState.idle);
				}
				else if (ctx.Owner.isKilled)
				{
					ctx.Nma.Stop();
					To(PetState.teleport);
				}
			}

			public override void Update()
			{
				base.Update();
				if (ctx.Target == null || !ctx.IsAlive || !ctx.IsVisible(ctx.Target.gameObject))
				{
					To(PetState.idle);
				}
				else if (!ctx.InRange(ctx.ThisTransform.position, ctx.Owner.transform.position, ctx.Info.MaxToOwnerDistance))
				{
					To(PetState.teleport);
				}
				else if (ctx.InRange(ctx.ThisTransform.position, ctx.Target.position, ctx.Info.AttackDistance))
				{
					if (!ctx.InAttackState)
					{
						To(PetState.attack);
					}
				}
				else if (ctx.CanTeleportToTarget)
				{
					ctx.Nma.Stop();
					ctx.Nma.ResetPath();
					ctx.Nma.Warp(ctx.Target.position);
					ctx.EffectShow.Play();
					if (Defs.isMulti && ctx.IsMine)
					{
						ctx.synchScript.isTeleported = true;
					}
				}
				else if (!ctx.Nma.isOnOffMeshLink)
				{
					ctx.Nma.SetDestination(ctx.Target.position);
					ctx.Nma.speed = Mathf.Clamp(ctx.GetCalculatedSpeedMultyplier() * ctx.Info.SpeedModif, 0f, ctx.Info.SpeedModif);
					ctx.Nma.Resume();
				}
			}

			public override void Out(PetState toState)
			{
				base.Out(toState);
				ctx.Nma.Stop();
			}
		}

		[ReadOnly]
		public NavMeshAgent Nma;

		public NavMeshPath Path;

		public override Vector3 MovePosition
		{
			get
			{
				return (!(base.Owner.GetPointForGroundPet() != null)) ? base.Owner.transform.position : base.Owner.GetPointForGroundPet().position;
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

		public override bool CanMoveToPlayer
		{
			get
			{
				if (Path == null)
				{
					Path = new NavMeshPath();
				}
				if (ThisTransform.position == base.Owner.myPlayerTransform.position)
				{
					return false;
				}
				NavMesh.CalculatePath(ThisTransform.position, base.Owner.myPlayerTransform.position, -1, Path);
				if (Path.corners.Length < 1)
				{
					return false;
				}
				float num = 0f;
				for (int i = 1; i < Path.corners.Length; i++)
				{
					num += Vector3.Magnitude(Path.corners[i - 1] - Path.corners[i]);
				}
				return num <= base.Info.MaxToOwnerDistance * 2f;
			}
		}

		public bool CanTeleportToTarget
		{
			get
			{
				if (Target == null || Defs.isMulti)
				{
					return false;
				}
				if (ThisTransform.position == Target.position)
				{
					return false;
				}
				float magnitude = (Target.position - ThisTransform.position).magnitude;
				if (magnitude > base.Info.ToTargetTeleportDistance)
				{
					return false;
				}
				if (Target.root != null && Initializer.enemiesObj.Contains(Target.root.gameObject))
				{
					return false;
				}
				if (Path == null)
				{
					Path = new NavMeshPath();
				}
				NavMesh.CalculatePath(ThisTransform.position, Target.position, -1, Path);
				if (Path.corners.Length < 1 || (Path.corners[Path.corners.Length - 1] - Target.transform.position).magnitude > base.Info.AttackDistance)
				{
					return true;
				}
				return false;
			}
		}

		public override void WarpToOwner()
		{
			Nma.Warp(MovePosition);
		}

		protected override void Awake()
		{
			base.Awake();
			Nma = GetComponent<NavMeshAgent>();
			if (base.gameObject.GetComponent<AgentLinkMover>() == null)
			{
				base.gameObject.AddComponent<AgentLinkMover>();
			}
		}

		protected override void StopEngine()
		{
			base.StopEngine();
			Nma.enabled = false;
		}
	}
}

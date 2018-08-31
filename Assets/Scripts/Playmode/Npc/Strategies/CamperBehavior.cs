using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Npc.Strategies.Routines.MovementRoutines;
using Playmode.Npc.Strategies.Routines.SightRoutines;
using Playmode.Pickable.TypePickable;
using UnityEngine;

namespace Playmode.Npc.Strategies
{
	public class CamperBehavior : BaseNpcBehavior
	{
		private readonly MovementRoutine retreatingMovementRoutine;
		private readonly SightRoutine noEnemySightRoutine;
		
		public CamperBehavior(Mover mover, HandController handController, HitSensor hitSensor, Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound) : base(mover, handController, hitSensor,
			health, npcSensorSight, npcSensorSound)
		{
			retreatingMovementRoutine = new RetreatWhileDodgingMovementRoutine(Mover);
			noEnemySightRoutine = new LookAroundSightRoutine(Mover);
		}

		protected override void DoIdle()
		{
			Mover.RotateTowardsAngle(RotationOrientation);
		}

		protected override void DoRoaming()
		{
			Mover.MoveTowardsDirection(MovementDirection);
			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);
		}
		
		protected override void DoInvestigating()
		{
			MovementDirection = NpcSensorSound.GetNewestSoundPosition() - Mover.transform.root.position;
			Mover.MoveTowardsDirection(MovementDirection);
			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);
		}

		protected override void DoEngaging()
		{
			if (Health.HealthPoints % HealthRetreatTolerance <= 0 && CurrentMedicalKitTarget != null)
			{
				Mover.RotateTowardsPosition(CurrentMedicalKitTarget.transform.root.position);				
				Mover.MoveTowardsPosition(CurrentMedicalKitTarget.transform.root.position);
				
				if (CurrentEnemyTarget != null)
				{
					Mover.MoveAwayFromPosition(CurrentEnemyTarget.transform.root.position);
					HandController.Use();
				}
			}
			else
			{
				if (CurrentMedicalKitTarget != null)
				{
					if (CurrentMedicalKitTarget.GetPickableType() == TypePickable.Medicalkit)
					{
						// TODO: What
						Vector3 direction = (CurrentMedicalKitTarget.transform.position - Mover.transform.position);
						direction += new Vector3(0, 0);
						
						Mover.RotateTowardsDirection(direction);
						Mover.MoveTowardsDirection(direction);

						if (CurrentEnemyTarget != null)
						{
							Mover.RotateTowardsPosition(CurrentEnemyTarget.transform.root.position);
							HandController.Use();
						}
					}
					else
					{
						Mover.RotateTowardsPosition(CurrentMedicalKitTarget.transform.root.position);
						Mover.MoveTowardsPosition(CurrentMedicalKitTarget.transform.root.position);
					}
				}
				else if (CurrentEnemyTarget != null)
				{
					Mover.RotateTowardsPosition(CurrentEnemyTarget.transform.root.position);
					Mover.MoveTowardsPosition(CurrentEnemyTarget.transform.root.position);
					HandController.Use();
				}
			}
		}

		protected override void DoAttacking()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = NpcSensorSight.GetClosestNpc();

			if (CurrentMedicalKitTarget != null)
			{
				Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			}
			else
			{
				Mover.MoveTowardsDirection(CurrentEnemyTarget.transform.position);
				Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			}

			HandController.Use();
		}

		protected override void DoRetreating()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = NpcSensorSight.GetClosestNpc();
			
			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			retreatingMovementRoutine.UpdateMovementRoutine(NpcSensorSight.GetClosestNpc().transform.root.position);
			
			HandController.Use();
		}

		protected override State EvaluateIdle()
		{
			if (NpcSensorSight.PickablesInSight.Any() || NpcSensorSight.NpcsInSight.Any())
			{
				return State.Engaging;
			}
			
			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateIdle();
		}

		protected override State EvaluateRoaming()
		{
			if (NpcSensorSight.PickablesInSight.Any() || NpcSensorSight.NpcsInSight.Any())
			{
				return State.Engaging;
			}
			
			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateRoaming();
		}

		protected override State EvaluateInvestigating()
		{
			if (NpcSensorSight.NpcsInSight.Any())
			{
				return State.Engaging;
			}

			if (!NpcSensorSound.SoundsInformations.Any())
			{
				return State.Idle;
			}

			return State.Investigating;
		}

		protected override State EvaluateEngaging()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}
			
			return DistanceToCurrentEnemy > DistanceSwitchFromEngagingToAttacking ? State.Engaging : State.Attacking;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}
			
			return DistanceToCurrentEnemy < DistanceSwitchFromEngagingToAttacking ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateRetreating()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return State.Retreating;
		}
	}
}
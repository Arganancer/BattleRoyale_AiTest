using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Npc.Strategies.Routines.MovementRoutines;
using Playmode.Npc.Strategies.Routines.SightRoutines;
using Playmode.Pickable;
using Playmode.Pickable.TypePickable;
using UnityEngine;

namespace Playmode.Npc.Strategies
{
	public class CamperBehavior : BaseNpcBehavior
	{
		private readonly MovementRoutine retreatingMovementRoutine;
		private readonly SightRoutine noEnemySightRoutine;
		private bool isCamping;
		private PickableController campedMedKit;

		public CamperBehavior(Mover mover, HandController handController, Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound) : base(mover, handController,
			health, npcSensorSight, npcSensorSound)
		{
			retreatingMovementRoutine = new RetreatWhileDodgingMovementRoutine(Mover);
			noEnemySightRoutine = new LookAroundSightRoutine(Mover);
			isCamping = false;
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
			if (!isCamping)
			{
				Mover.MoveTowardsDirection(MovementDirection);
			}

			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);
		}

		protected override void DoEngaging()
		{
			var movementTarget = new Vector3();
			var rotationTarget = new Vector3();

			if (NpcSensorSight.PickablesInSight.Any())
			{
				if (campedMedKit != null)
				{
					movementTarget = campedMedKit.transform.root.position;
					rotationTarget = movementTarget;
				}
				else if (CurrentUziTarget != null)
				{
					movementTarget = CurrentShotgunTarget.transform.root.position;
					rotationTarget = movementTarget;
				}
			}

			if (CurrentEnemyTarget != null)
			{
				if (campedMedKit == null)
					movementTarget = CurrentEnemyTarget.transform.root.position;
				rotationTarget = CurrentEnemyTarget.transform.root.position;
			}

			Mover.MoveTowardsPosition(movementTarget);
			Mover.RotateTowardsPosition(rotationTarget);
			if (CurrentEnemyTarget != null)
			{
				HandController.Use();
			}
		}

		protected override void DoAttacking()
		{
			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			HandController.Use();
		}

		protected override void DoRetreating()
		{
			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			retreatingMovementRoutine.UpdateMovementRoutine(NpcSensorSight.GetClosestNpc().transform.root.position);
			HandController.Use();
		}

		protected override State EvaluateIdle()
		{
			UpdateCampedMedicalKit();
			if (isCamping)
			{
				if (Health.HealthPoints < HealthRetreatTolerance)
					return State.Engaging;
				if (NpcSensorSight.NpcsInSight.Any())
					return State.Attacking;
				return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : State.Idle;
			}

			if (NpcSensorSight.PickablesInSight.Any() || NpcSensorSight.NpcsInSight.Any())
				return State.Engaging;
			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateIdle();
		}

		protected override State EvaluateRoaming()
		{
			UpdateCampedMedicalKit();
			if (NpcSensorSight.PickablesInSight.Any() || NpcSensorSight.NpcsInSight.Any())
				return State.Engaging;

			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateRoaming();
		}

		protected override State EvaluateInvestigating()
		{
			UpdateCampedMedicalKit();
			if (NpcSensorSight.NpcsInSight.Any())
				return isCamping ? State.Attacking : State.Engaging;

			return !NpcSensorSound.SoundsInformations.Any() ? State.Idle : State.Investigating;
		}

		protected override State EvaluateEngaging()
		{
			UpdateCampedMedicalKit();
			if (isCamping)
				return Health.HealthPoints < HealthRetreatTolerance ? State.Engaging : State.Idle;
			
			if (!NpcSensorSight.NpcsInSight.Any() && !NpcSensorSight.PickablesInSight.Any())
				return State.Idle;

			if (campedMedKit != null)
				return State.Engaging;

			if (NpcSensorSight.NpcsInSight.Any() && Health.HealthPoints < HealthRetreatTolerance)
				return State.Retreating;

			return DistanceToCurrentEnemy > DistanceSwitchFromEngagingToAttacking ? State.Engaging : State.Attacking;
		}

		protected override State EvaluateAttacking()
		{
			UpdateCampedMedicalKit();
			if (!NpcSensorSight.NpcsInSight.Any())
				return State.Idle;

			if (Health.HealthPoints < HealthRetreatTolerance)
				return State.Retreating;

			return isCamping || DistanceToCurrentEnemy < DistanceSwitchFromEngagingToAttacking
				? State.Attacking
				: State.Engaging;
		}

		protected override State EvaluateRetreating()
		{
			UpdateCampedMedicalKit();
			if (campedMedKit != null)
				return State.Engaging;
			return !NpcSensorSight.NpcsInSight.Any() ? State.Idle : State.Retreating;
		}

		private void UpdateCampedMedicalKit()
		{
			if (campedMedKit == null && CurrentMedicalKitTarget != null)
				campedMedKit = CurrentMedicalKitTarget;
			if (campedMedKit != null)
			{
				if (!isCamping &&
				    Vector3.Distance(Mover.transform.root.position, campedMedKit.transform.root.position) <= 3)
				{
					isCamping = true;
				}
			}
			else
				isCamping = false;
		}
	}
}
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
			HealthRetreatTolerance = 600;
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
			if (IsOutsideOfZone)
				MovementDirection = -Mover.transform.parent.root.position;
			Mover.MoveTowardsDirection(MovementDirection);
			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);
		}

		protected override void DoInvestigating()
		{
			MovementDirection = NpcSensorSound.GetNewestSoundPosition() - Mover.transform.root.position;
			if (!isCamping)
				Mover.MoveTowardsDirection(MovementDirection);

			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);
		}

		protected override void DoEngaging()
		{
			if (campedMedKit != null)
			{
				Mover.MoveTowardsPosition(campedMedKit.transform.root.position);
				if (CurrentEnemyTarget != null)
				{
					Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
					HandController.Use();
				}
				else
					Mover.RotateTowardsPosition(campedMedKit.transform.root.position);
			}
			else if (CurrentUziTarget != null)
			{
				Mover.MoveTowardsPosition(CurrentUziTarget.transform.root.position);
				Mover.RotateTowardsPosition(CurrentUziTarget.transform.root.position);
			}
			else if (CurrentEnemyTarget != null)
			{
				Mover.MoveRightAroundPosition(CurrentEnemyTarget.transform.root.position);
				Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
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

			if (CurrentUziTarget != null || campedMedKit != null || CurrentEnemyTarget != null)
				return State.Engaging;
			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateIdle();
		}

		protected override State EvaluateRoaming()
		{
			UpdateCampedMedicalKit();
			if (isCamping)
				return State.Idle;

			if (CurrentUziTarget != null || campedMedKit != null || NpcSensorSight.NpcsInSight.Any())
				return State.Engaging;
			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateRoaming();
		}

		protected override State EvaluateInvestigating()
		{
			UpdateCampedMedicalKit();
			if (isCamping)
			{
				if (NpcSensorSight.NpcsInSight.Any())
					return State.Attacking;
				return !NpcSensorSound.SoundsInformations.Any() ? State.Idle : State.Investigating;
			}

			if (campedMedKit != null || CurrentUziTarget != null || CurrentEnemyTarget != null)
				return State.Engaging;
			return !NpcSensorSound.SoundsInformations.Any() ? State.Idle : State.Investigating;
		}

		protected override State EvaluateEngaging()
		{
			UpdateCampedMedicalKit();
			if (isCamping)
			{
				if (Health.HealthPoints < HealthRetreatTolerance)
					if(CurrentEnemyTarget != null &&
				    Vector3.Distance(
					    CurrentEnemyTarget.transform.root.position,
					    campedMedKit.transform.root.position) + 5 <
				    Vector3.Distance(Mover.transform.root.position,
					    campedMedKit.transform.root.position))
						return State.Engaging;
				return State.Idle;
			}

			if (!NpcSensorSight.NpcsInSight.Any() && !NpcSensorSight.PickablesInSight.Any())
				return State.Idle;

			if (campedMedKit != null)
				return State.Engaging;

			if (NpcSensorSight.NpcsInSight.Any() && Health.HealthPoints < HealthRetreatTolerance)
				return State.Retreating;

			if (CurrentUziTarget != null)
				return State.Engaging;

			return DistanceToCurrentEnemy > DistanceSwitchFromEngagingToAttacking ? State.Engaging : State.Attacking;
		}

		protected override State EvaluateAttacking()
		{
			UpdateCampedMedicalKit();
			if (isCamping)
			{
				if (CurrentEnemyTarget == null)
					return State.Idle;
				if (Vector3.Distance(NpcSensorSight.GetClosestNpc().transform.root.position, campedMedKit.transform.root.position) - 3
				     <
				    Vector3.Distance(Mover.transform.root.position, campedMedKit.transform.root.position) ||
				    Health.HealthPoints < HealthRetreatTolerance)
					return State.Engaging;
				return State.Attacking;
			}

			if (!NpcSensorSight.NpcsInSight.Any())
				return State.Idle;

			if (Health.HealthPoints < HealthRetreatTolerance)
				return State.Retreating;

			return DistanceToCurrentEnemy < DistanceSwitchFromEngagingToAttacking ? State.Attacking : State.Engaging;
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
			if (campedMedKit != null &&
			    Vector3.Distance(Mover.transform.root.position, campedMedKit.transform.root.position) <= 3)
				isCamping = true;
			else
				isCamping = false;
		}
	}
}
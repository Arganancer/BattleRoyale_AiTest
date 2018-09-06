using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Npc.Strategies.Routines.SightRoutines;

namespace Playmode.Npc.Strategies
{
	public class NormalBehavior : BaseNpcBehavior
	{
		private readonly SightRoutine noEnemySightRoutine;

		public NormalBehavior(Mover mover, HandController handController,Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound) : base(mover, handController,
			health, npcSensorSight, npcSensorSound)
		{
			HealthRetreatTolerance = 0;
			noEnemySightRoutine = new LookAroundSightRoutine(Mover);
			DistanceSwitchFromAttackingToEngaging = 6f;
			DistanceSwitchFromEngagingToAttacking = 5f;
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

		protected override void DoEngaging()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = NpcSensorSight.GetClosestNpc();

			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));

			Mover.MoveTowardsPosition(CurrentEnemyTarget.transform.root.position);

			HandController.Use();
		}

		protected override void DoInvestigating()
		{
			MovementDirection = NpcSensorSound.GetNewestSoundPosition() - Mover.transform.root.position;
			
			Mover.MoveTowardsDirection(MovementDirection);
			noEnemySightRoutine.UpdateSightRoutine(MovementDirection);
		}

		protected override void DoAttacking()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = NpcSensorSight.GetClosestNpc();
			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			HandController.Use();
		}

		protected override void DoRetreating()
		{
			// Too dumb to retreat
		}

		protected override State EvaluateIdle()
		{
			if (NpcSensorSight.NpcsInSight.Any())
				return State.Engaging;

			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateIdle();
		}

		protected override State EvaluateRoaming()
		{
			if (NpcSensorSight.NpcsInSight.Any())
				return State.Engaging;

			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateRoaming();
		}

		protected override State EvaluateInvestigating()
		{
			if (NpcSensorSight.NpcsInSight.Any())
				return State.Engaging;
			return !NpcSensorSound.SoundsInformations.Any() ? State.Idle : State.Investigating;
		}

		protected override State EvaluateEngaging()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
				return State.Idle;

			return DistanceToCurrentEnemy < DistanceSwitchFromEngagingToAttacking ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
				return State.Idle;

			return DistanceToCurrentEnemy > DistanceSwitchFromAttackingToEngaging ? State.Engaging : State.Attacking;
		}

		protected override State EvaluateRetreating()
		{
			return State.Idle;
		}
	}
}
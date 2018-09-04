using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Npc.Strategies.Routines.SightRoutines;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playmode.Npc.Strategies
{
	public class CowboyBehavior : BaseNpcBehavior
	{
		private readonly SightRoutine noEnemySightRoutine;

		public CowboyBehavior(Mover mover, HandController handController,Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound)
			: base(mover, handController, health, npcSensorSight, npcSensorSound)
		{
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
			if (CurrentShotgunTarget != null)
			{
				Mover.RotateTowardsPosition(CurrentShotgunTarget.transform.root.position);
				Mover.MoveTowardsPosition(CurrentShotgunTarget.transform.root.position);
			}
			else if (CurrentUziTarget != null)
			{
				Mover.RotateTowardsPosition(CurrentUziTarget.transform.root.position);
				Mover.MoveTowardsPosition(CurrentUziTarget.transform.root.position);
			}
			else if (CurrentEnemyTarget != null)
			{
				Mover.RotateTowardsPosition(CurrentEnemyTarget.transform.root.position);
				Mover.MoveTowardsPosition(CurrentEnemyTarget.transform.root.position);
				HandController.Use();
			}
		}

		protected override void DoAttacking()
		{
			if (CurrentEnemyTarget != null)
			{
				Mover.RotateTowardsPosition(CurrentEnemyTarget.transform.root.position);
				HandController.Use();
			}
		}

		protected override void DoRetreating()
		{
			// A cowboy never retreats ಠ_ಠ
		}

		protected override State EvaluateIdle()
		{
			if (NpcSensorSight.NpcsInSight.Any() || CurrentShotgunTarget != null || CurrentUziTarget != null)
			{
				return State.Engaging;
			}

			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateIdle();
		}

		protected override State EvaluateRoaming()
		{
			if (NpcSensorSight.NpcsInSight.Any() || CurrentShotgunTarget != null || CurrentUziTarget != null)
			{
				return State.Engaging;
			}

			return NpcSensorSound.SoundsInformations.Any() ? State.Investigating : base.EvaluateRoaming();
		}

		protected override State EvaluateInvestigating()
		{
			if (NpcSensorSight.NpcsInSight.Any() || CurrentShotgunTarget != null || CurrentUziTarget != null)
			{
				return State.Engaging;
			}

			return !NpcSensorSound.SoundsInformations.Any() ? State.Idle : State.Investigating;
		}

		protected override State EvaluateEngaging()
		{
			if (CurrentShotgunTarget != null || CurrentUziTarget != null)
			{
				return State.Engaging;
			}

			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return DistanceToCurrentEnemy > DistanceSwitchFromEngagingToAttacking ? State.Engaging : State.Attacking;
		}

		protected override State EvaluateAttacking()
		{
			if (CurrentShotgunTarget != null || CurrentUziTarget != null)
			{
				return State.Engaging;
			}

			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return DistanceToCurrentEnemy < DistanceSwitchFromEngagingToAttacking ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateRetreating()
		{
			return State.Idle;
		}
	}
}
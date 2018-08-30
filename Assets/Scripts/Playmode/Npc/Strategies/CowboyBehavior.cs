using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Pickable.TypePickable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playmode.Npc.Strategies
{
	public class CowboyBehavior : BaseNpcBehavior
	{
		public CowboyBehavior(Mover mover, HandController handController, HitSensor hitSensor, Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound)
			: base(mover, handController, hitSensor, health, npcSensorSight, npcSensorSound)
		{
		}

		protected override void DoIdle()
		{
			Mover.RotateTowardsAngle(RotationOrientation);
		}

		protected override void DoRoaming()
		{
			UpdateSightRoutine();
			Mover.MoveTowardsDirection(MovementDirection);
		}

		protected override void DoInvestigating()
		{
			MovementDirection = GetNewestSoundPosition() - Mover.transform.root.position;
			UpdateSightRoutine();
			Mover.MoveTowardsDirection(MovementDirection);
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
			if (TimeUntilStateSwitch > MaxIdleTime)
			{
				TimeUntilStateSwitch = Random.Range(MinIdleTime, MaxIdleTime);
			}

			if (NpcSensorSight.NpcsInSight.Any() || CurrentShotgunTarget != null || CurrentUziTarget != null)
			{
				return State.Engaging;
			}

			if (NpcSensorSound.SoundsInformations.Any())
			{
				return State.Investigating;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				MovementDirection = Mover.GetRandomDirection();
				TimeUntilStateSwitch = Random.Range(MinRoamingTime, MaxRoamingTime);
				return State.Roaming;
			}

			return State.Idle;
		}

		protected override State EvaluateRoaming()
		{
			if (TimeUntilStateSwitch > MaxIdleTime)
			{
				TimeUntilStateSwitch = Random.Range(MinIdleTime, MaxIdleTime);
			}

			if (NpcSensorSight.NpcsInSight.Any() || CurrentShotgunTarget != null || CurrentUziTarget != null)
			{
				return State.Engaging;
			}

			if (NpcSensorSound.SoundsInformations.Any())
			{
				return State.Investigating;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				TimeUntilStateSwitch = Random.Range(MinIdleTime, MaxIdleTime);
				return State.Idle;
			}

			return State.Roaming;
		}

		protected override State EvaluateInvestigating()
		{
			if (NpcSensorSight.NpcsInSight.Any() || CurrentShotgunTarget != null || CurrentUziTarget != null)
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
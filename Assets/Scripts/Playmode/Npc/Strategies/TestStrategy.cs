using System;
using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

namespace Playmode.Npc.Strategies
{
	public class TestStrategy : BaseNpcBehavior
	{
		public TestStrategy(Mover mover, HandController handController, HitSensor hitSensor, Health health,
			NpcSensor npcSensor) : base(mover, handController, hitSensor, health, npcSensor)
		{
		}

		#region DoTheStuff

		protected override void DoIdle()
		{
			Mover.Rotate(RotationOrientation);
		}

		protected override void DoRoaming()
		{
			UpdateSightRoutine();
			Mover.MoveRelativeToWorld(MovementDirection);
		}

		protected override void DoEngaging()
		{
			Mover.Rotate(
				HandController.AimTowardsPoint(GetClosestNpc(NpcSensor.NpcsInSight).transform.parent.position));

			HandController.Use();
			Mover.MoveRelativeToWorld(GetClosestNpc(NpcSensor.NpcsInSight).transform.parent.position -
			                          Mover.transform.parent.position);
		}

		protected override void DoAttacking()
		{
			Mover.Rotate(
				HandController.AimTowardsPoint(GetClosestNpc(NpcSensor.NpcsInSight).transform.parent.position));

			HandController.Use();
		}

		protected override void DoRetreating()
		{
			Vector3 DirectionAwayFromEnemy = Mover.transform.parent.position -
			                                 GetClosestNpc(NpcSensor.NpcsInSight).transform.parent.position;
			Mover.Rotate(
				HandController.AimTowardsPoint(GetClosestNpc(NpcSensor.NpcsInSight).transform.parent.position));
			Mover.MoveRelativeToWorld(DirectionAwayFromEnemy.normalized + Vector3.right);
			HandController.Use();
		}

		#endregion

		#region Evaluate

		protected override State EvaluateIdle()
		{
			if (NpcSensor.NpcsInSight.Any())
			{
				return State.Engaging;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				MovementDirection = GetRandomDirection();
				TimeUntilStateSwitch = Random.Range(6f, 8f);
				return State.Roaming;
			}

			return State.Idle;
		}

		protected override State EvaluateRoaming()
		{
			if (NpcSensor.NpcsInSight.Any())
			{
				return State.Engaging;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				while (RotationOrientation != 0)
				{
					RotationOrientation = Random.Range(-1, 1);
				}

				TimeUntilStateSwitch = Random.Range(0.2f, 0.5f);
				return State.Idle;
			}

			return State.Roaming;
		}

		protected override State EvaluateEngaging()
		{
			if (!NpcSensor.NpcsInSight.Any())
			{
				TimeUntilStateSwitch = Random.Range(0.2f, 0.5f);
				return State.Idle;
			}

			if (Health.HealthPoints < 50)
			{
				return State.Retreating;
			}

			return DistanceToCurrentTarget < 5 ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensor.NpcsInSight.Any())
			{
				TimeUntilStateSwitch = Random.Range(0.2f, 0.5f);
				return State.Idle;
			}
			
			if (Health.HealthPoints < 50)
			{
				return State.Retreating;
			}

			return DistanceToCurrentTarget < 5 ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateRetreating()
		{
			if (!NpcSensor.NpcsInSight.Any())
			{
				TimeUntilStateSwitch = Random.Range(0.2f, 0.5f);
				return State.Idle;
			}

			return State.Retreating;
		}

		#endregion
	}
}
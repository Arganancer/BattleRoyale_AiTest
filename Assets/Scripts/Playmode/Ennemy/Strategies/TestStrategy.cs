using System;
using System.Linq;
using Playmode.Ennemy.BodyParts;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playmode.Ennemy.Strategies
{
	public class TestStrategy : BaseEnemyBehavior
	{
		public TestStrategy(
			Mover mover,
			HandController handController,
			HitSensor hitSensor,
			Health health,
			EnnemySensor enemySensor) : base(mover, handController, hitSensor, health, enemySensor)
		{
		}

		protected override void UpdateNPCLogic()
		{
			switch (CurrentState)
			{
				case State.Idle:
					break;
				case State.Roaming:
					Mover.Rotate(HandController.AimTowardsDirection(Mover, MovementDirection));
					Mover.MoveRelativeToWorld(MovementDirection);
					break;
				case State.Engaging:
					Mover.Rotate(HandController.AimTowardsPoint(GetClosestEnnemy(EnnemySensor.EnnemiesInSight).transform.parent.position));
					HandController.Use();
					Mover.MoveRelativeToWorld(GetClosestEnnemy(EnnemySensor.EnnemiesInSight).transform.parent.position -
					           Mover.transform.parent.position
					);
					break;
				case State.Attacking:
					break;
				case State.Retreating:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override State EvaluateIdle()
		{
			if (EnnemySensor.EnnemiesInSight.Any())
			{
				return State.Engaging;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				MovementDirection = GetRandomDirection();
				TimeUntilStateSwitch = Random.Range(4f, 6f);
				return State.Roaming;
			}

			return State.Idle;
		}

		protected override State EvaluateRoaming()
		{
			if (EnnemySensor.EnnemiesInSight.Any())
			{
				return State.Engaging;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				TimeUntilStateSwitch = Random.Range(0.5f, 1.5f);
				return State.Idle;
			}

			return State.Roaming;
		}

		protected override State EvaluateEngaging()
		{
			if (!EnnemySensor.EnnemiesInSight.Any())
			{
				return State.Idle;
			}

			return State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			throw new System.NotImplementedException();
		}

		protected override State EvaluateRetreating()
		{
			throw new System.NotImplementedException();
		}
	}
}
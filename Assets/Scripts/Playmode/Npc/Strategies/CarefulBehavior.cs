using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using UnityEngine;

namespace Playmode.Npc.Strategies
{
	public class CarefulBehavior : BaseNpcBehavior
	{
		[SerializeField] private int healthPointsToLose = 50;
		
		public CarefulBehavior(Mover mover, HandController handController, HitSensor hitSensor, Health health, NpcSensor npcSensor) : base(mover, handController, hitSensor, health, npcSensor)
		{
		}

		protected override void DoIdle()
		{
			throw new System.NotImplementedException();
		}

		protected override void DoRoaming()
		{
			throw new System.NotImplementedException();
		}

		protected override void DoEngaging()
		{
			throw new System.NotImplementedException();
		}

		protected override void DoAttacking()
		{
			throw new System.NotImplementedException();
		}

		protected override void DoRetreating()
		{
			throw new System.NotImplementedException();
		}

		protected override State EvaluateIdle()
		{
			if (NpcSensor.NpcsInSight.Any())
			{
				return State.Attacking;
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
			if (NpcSensor.NpcsInSight.Any())
			{
				return State.Attacking;
			}
			
			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				MovementDirection = GetRandomDirection();
				TimeUntilStateSwitch = Random.Range(4f, 6f);
				return State.Idle;
			}

			return State.Roaming;
		}

		protected override State EvaluateEngaging()
		{
			if (!NpcSensor.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensor.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return State.Attacking;
		}

		protected override State EvaluateRetreating()
		{
			if (Health.HealthPoints >= healthPointsToLose)
			{
				return State.Attacking;
			}

			return State.Retreating;
		}
	}
}
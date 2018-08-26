using System;
using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategies;
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
			throw new NotImplementedException();
		}

		protected override void DoRoaming()
		{
			Mover.Rotate(HandController.AimTowardsDirection(Mover, MovementDirection));
			Mover.MoveRelativeToWorld(MovementDirection);
		}

		protected override void DoEngaging()
		{
			Mover.Rotate(
				HandController.AimTowardsPoint(GetClosestNpc(NpcSensorSight.NpcsInSight).transform.parent.position));
			HandController.Use();
			Mover.MoveRelativeToWorld(GetClosestNpc(NpcSensorSight.NpcsInSight).transform.parent.position -
			                          Mover.transform.parent.position);
		}

		protected override void DoInvestigating()
		{
			throw new NotImplementedException();
		}

		protected override void DoAttacking()
		{
			throw new NotImplementedException();
		}

		protected override void DoRetreating()
		{
			throw new NotImplementedException();
		}

		protected override State EvaluateIdle()
		{
			if (NpcSensorSight.NpcsInSight.Any())
			{
				return State.Attacking;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				TimeUntilStateSwitch = Random.Range(4f, 6f);
				return State.Roaming;
			}

			return State.Idle;
		}

		protected override State EvaluateRoaming()
		{
			if (NpcSensorSight.NpcsInSight.Any())
			{
				return State.Attacking;
			}
			
			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				TimeUntilStateSwitch = Random.Range(0.5f, 1.5f);
				return State.Idle;
			}

			return State.Roaming;
		}

		protected override State EvaluateInvestigating()
		{
			throw new NotImplementedException();
		}

		protected override State EvaluateEngaging()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}
			
			return State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}
			
			return State.Attacking;
		}

		protected override State EvaluateRetreating()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}
			
			return State.Attacking;
		}
	}
}
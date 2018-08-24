using System;
using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playmode.Npc.Strategies
{
	public class CowboyBehaviour : BaseNpcBehavior
	{
		public CowboyBehaviour(Mover mover, HandController handController, HitSensor hitSensor, Health health,
			NpcSensor npcSensor)
			: base(mover, handController, hitSensor, health, npcSensor)
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
				HandController.AimTowardsPoint(GetClosestNpc(NpcSensor.NpcsInSight).transform.parent.position));
			HandController.Use();
			Mover.MoveRelativeToWorld(GetClosestNpc(NpcSensor.NpcsInSight).transform.parent.position -
			                          Mover.transform.parent.position);
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
			if (NpcSensor.NpcsInSight.Any())
			{
				return State.Roaming;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				TimeUntilStateSwitch = Random.Range(0.2f, 1.0f);
				return State.Idle;
			}

			return State.Roaming;
		}

		protected override State EvaluateRoaming()
		{
			throw new System.NotImplementedException();
		}

		protected override State EvaluateEngaging()
		{
			throw new System.NotImplementedException();
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
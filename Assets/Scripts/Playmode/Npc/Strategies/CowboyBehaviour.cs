using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using UnityEngine;

namespace Playmode.Npc.Strategies
{
	public class CowboyBehaviour : BaseNpcBehavior
	{
		public CowboyBehaviour(Mover mover, HandController handController, HitSensor hitSensor, Health health, NpcSensor npcSensor) 
			: base(mover, handController, hitSensor, health, npcSensor)
		{
		}

		protected override void UpdateNpcLogic()
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
					Mover.Rotate(HandController.AimTowardsPoint(GetClosestNpc(NpcSensor.NpcsInSight).transform.parent.position));
					HandController.Use();
					Mover.MoveRelativeToWorld(GetClosestNpc(NpcSensor.NpcsInSight).transform.parent.position -
					                          Mover.transform.parent.position
					);
					break;
				case State.Attacking:
					break;
				//if object en vue ->
				case State.Retreating:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override State EvaluateIdle()
		{
			throw new System.NotImplementedException();
		}

		protected override State EvaluateRoaming()
		{
			throw new System.NotImplementedException();
		}

		protected override State EvaluateEngaging()
		{
			throw new System.NotImplementedException();
			/*if (npcsInSight != null)
			{
				MoveTowardsObject(GetClosestNpc());
			}*/
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
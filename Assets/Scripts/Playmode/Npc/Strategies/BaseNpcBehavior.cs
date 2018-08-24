using System;
using System.Collections.Generic;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using UnityEngine;

namespace Playmode.Npc.Strategies
{
	/// <summary>
	/// An NPC will change their current state depending on their behavior as well as current situation:
	/// </summary>
	public enum State
	{
		Idle,
		Roaming,
		Engaging,
		Attacking,
		Retreating
	}

	/// <summary>
	/// 
	/// An NPC has a list of priorities depending on their current state for the following targets:
	///     Enemy
	///     MedicalKit
	///     Weapon
	///
	/// An NPC will change their current state depending on their behavior as well as current situation:
	///     Idle/Roaming
	///     Engaging
	///     Attacking
	///     Retreating
	///
	/// Sensors considered by NPCs to determine action:
	/// 	Vision
	/// 		Nearest Target
	/// 			Angle to target
	/// 		Current Target Lock
	/// 	CurrentHealth
	/// 
	/// </summary>
	public abstract class BaseNpcBehavior : INpcStrategy
	{
		protected readonly Mover Mover;
		protected readonly HandController HandController;
		protected readonly NpcSensor NpcSensor;
		protected readonly HitSensor HitSensor;
		protected readonly Health Health;
		protected State CurrentState;
		protected float TimeUntilStateSwitch;
		protected Vector3 MovementDirection;

		protected BaseNpcBehavior(Mover mover, HandController handController,
			HitSensor hitSensor, Health health, NpcSensor npcSensor)
		{
			CurrentState = State.Idle;
			Mover = mover;
			HandController = handController;
			HitSensor = hitSensor;
			Health = health;
			NpcSensor = npcSensor;
			TimeUntilStateSwitch = 0;
			MovementDirection = new Vector3();
		}

		protected void MoveTowardsObject(GameObject target)
		{
			var direction = target.transform.position - Mover.transform.parent.position;
			Mover.MoveRelativeToSelf(direction);
		}

		protected void FleeFromObject(GameObject npc)
		{
			var direction = Mover.transform.parent.position - npc.transform.position;
			Mover.MoveRelativeToSelf(direction);
		}

		protected Vector3 GetRandomDirection()
		{
			return UnityEngine.Random.insideUnitCircle.normalized;
		}

		protected NpcController GetClosestNpc(IEnumerable<NpcController> npcsInSight)
		{
			NpcController closestNpc = null;
			var distance = float.MaxValue;
			foreach (var npc in npcsInSight)
			{
				if (closestNpc == null)
				{
					closestNpc = npc;
					distance = Vector3.Distance(closestNpc.transform.position,
						Mover.transform.parent.position);
				}
				else
				{
					var currentNpcDistance =
						Vector3.Distance(closestNpc.transform.position, npc.transform.position);
					if (distance > currentNpcDistance)
					{
						distance = currentNpcDistance;
						closestNpc = npc;
					}
				}
			}

			return closestNpc;
		}

		public void Act()
		{
			switch (CurrentState)
			{
				case State.Idle:
					CurrentState = EvaluateIdle();
					break;
				case State.Roaming:
					CurrentState = EvaluateRoaming();
					break;
				case State.Engaging:
					CurrentState = EvaluateEngaging();
					break;
				case State.Attacking:
					CurrentState = EvaluateAttacking();
					break;
				case State.Retreating:
					CurrentState = EvaluateRetreating();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			UpdateNpcLogic();
		}

		protected abstract void UpdateNpcLogic();

		protected abstract State EvaluateIdle();

		protected abstract State EvaluateRoaming();

		protected abstract State EvaluateEngaging();

		protected abstract State EvaluateAttacking();

		protected abstract State EvaluateRetreating();
	}
}
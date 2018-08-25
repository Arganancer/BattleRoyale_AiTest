using System;
using System.Collections.Generic;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using UnityEngine;
using Random = UnityEngine.Random;

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
		protected float DistanceToCurrentTarget;
		protected int RotationOrientation;

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

		protected void MoveTowardsPosition(Vector3 position)
		{
			var direction = position - Mover.transform.root.position;
			Mover.MoveRelativeToSelf(direction);
		}

		protected void MoveAwayFromPosition(Vector3 position)
		{
			var direction = Mover.transform.root.position - position;
			Mover.MoveRelativeToSelf(direction);
		}

		protected Vector3 GetRandomDirection()
		{
			return UnityEngine.Random.insideUnitCircle.normalized;
		}

		protected enum SightRoutine
		{
			None,
			LookingLeft,
			LookingRight,
			LookingSideToSide
		}

		protected SightRoutine CurrentSightRoutine;
		private float currentSightRoutineDelay;
		private const float SightRoutineDelay = 1.5f;

		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		protected void UpdateSightRoutine()
		{
			if (currentSightRoutineDelay > 0f)
			{
				currentSightRoutineDelay -= Time.deltaTime;
			}
			else if (CurrentSightRoutine == SightRoutine.None)
			{
				var chanceOfSightRoutine = Random.Range(1, 100);
				if (chanceOfSightRoutine <= 3)
				{
					currentSightRoutineDelay = SightRoutineDelay;
					if (chanceOfSightRoutine <= 2)
					{
						CurrentSightRoutine = SightRoutine.LookingRight;
					}

					CurrentSightRoutine = SightRoutine.LookingLeft;
				}
			}

			switch (CurrentSightRoutine)
			{
				case SightRoutine.LookingLeft:
					LookToTheLeft();
					break;
				case SightRoutine.LookingRight:
					LookToTheRight();
					break;
				case SightRoutine.LookingSideToSide:
					LookSideToSide();
					break;
				case SightRoutine.None:
					LookForward();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void LookForward()
		{
			Mover.Rotate(HandController.AimTowardsDirection(Mover, MovementDirection));
		}

		private void LookToTheRight()
		{
			if (Vector3.Angle(Mover.transform.up, MovementDirection) < 70f)
			{
				Mover.Rotate(1);
			}
			else
			{
				CurrentSightRoutine = SightRoutine.None;
			}
		}

		private void LookToTheLeft()
		{
			if (Vector3.Angle(Mover.transform.up, MovementDirection) < 70f)
			{
				Mover.Rotate(-1);
			}
			else
			{
				CurrentSightRoutine = SightRoutine.None;
			}
		}

		private void LookSideToSide()
		{
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

			DistanceToCurrentTarget = distance;
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

		private void UpdateNpcLogic()
		{
			switch (CurrentState)
			{
				case State.Idle:
					DoIdle();
					break;
				case State.Roaming:
					DoRoaming();
					break;
				case State.Engaging:
					DoEngaging();
					break;
				case State.Attacking:
					DoAttacking();
					break;
				case State.Retreating:
					DoRetreating();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected abstract void DoIdle();

		protected abstract void DoRoaming();

		protected abstract void DoEngaging();

		protected abstract void DoAttacking();

		protected abstract void DoRetreating();

		protected abstract State EvaluateIdle();

		protected abstract State EvaluateRoaming();

		protected abstract State EvaluateEngaging();

		protected abstract State EvaluateAttacking();

		protected abstract State EvaluateRetreating();
	}
}
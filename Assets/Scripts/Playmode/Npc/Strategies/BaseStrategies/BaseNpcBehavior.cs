using System;
using System.Collections.Generic;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Pickable;
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
		protected enum SightRoutine
		{
			None,
			LookingLeft,
			LookingRight,
			LookingSideToSide
		}
		private float currentSightRoutineDelay;
		private const float SightRoutineDelay = 1.2f;
		
		protected SightRoutine CurrentSightRoutine;
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
		protected NpcController CurrentEnemyTarget;
		protected PickableController CurrentPickableTarget;
		protected float AttackingDistance = 5f;

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

		/// <summary>
		/// Moves the object in the direction of the given position.
		///
		/// This function works by obtaining a directional vector via vector substraction.
		/// </summary>
		protected void MoveTowardsNpc(NpcController npcController)
		{
			Mover.MoveRelativeToWorld(npcController.transform.parent.position - Mover.transform.parent.position);
		}

		protected void MoveTowardsDirection(Vector3 direction)
		{
			Mover.MoveRelativeToWorld(direction);
		}

		protected void MoveAwayFromNpc(NpcController npcController)
		{
			Mover.MoveRelativeToWorld(Mover.transform.parent.position - npcController.transform.parent.position);
		}

		protected void MoveAwayFromDirection(Vector3 direction)
		{
			Mover.MoveRelativeToWorld(-direction);
		}

		protected void RotateTowardsAngle(int angle)
		{
			Mover.Rotate(angle);
		}

		protected void RotateTowardsDirection(Vector3 direction)
		{
			Mover.RotateTowards(direction);
		}

		protected void RotateTowardsNpc(NpcController npcController)
		{
			Mover.RotateTowards(npcController.transform.root.position - Mover.transform.root.position);
		}

		protected static Vector3 GetRandomDirection()
		{
			return Random.insideUnitCircle;
		}

		protected void UpdateSightRoutine()
		{
			if (currentSightRoutineDelay > 0f)
			{
				currentSightRoutineDelay -= Time.deltaTime;
			}
			else if (CurrentSightRoutine == SightRoutine.None)
			{
				var chanceOfSightRoutine = Random.Range(1, 100);
				if (chanceOfSightRoutine <= 2)
				{
					CurrentSightRoutine = chanceOfSightRoutine <= 1 ? SightRoutine.LookingRight : SightRoutine.LookingLeft;
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
			RotateTowardsDirection(MovementDirection);
		}

		private void LookToTheRight()
		{
			if (Vector3.Angle(Mover.transform.up, MovementDirection) < 70f)
			{
				Mover.Rotate(1);
			}
			else
			{
				StartSightRoutineDelay();
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
				StartSightRoutineDelay();
			}
		}

		private void LookSideToSide()
		{
			
		}

		private void StartSightRoutineDelay()
		{
			currentSightRoutineDelay = SightRoutineDelay;
			CurrentSightRoutine = SightRoutine.None;
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

		protected void UpdateCurrentEnemyTarget()
		{
			if (CurrentEnemyTarget != null)
			{
				DistanceToCurrentTarget = Vector3.Distance(CurrentEnemyTarget.transform.position,
					Mover.transform.parent.position);
			}
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
			UpdateCurrentEnemyTarget();
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
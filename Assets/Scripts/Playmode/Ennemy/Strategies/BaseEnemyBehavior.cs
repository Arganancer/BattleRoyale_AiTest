using System;
using System.Collections.Generic;
using Playmode.Ennemy.BodyParts;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using UnityEngine;

namespace Playmode.Ennemy.Strategies
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
	public abstract class BaseEnemyBehavior : IEnnemyStrategy
	{
		protected readonly Mover Mover;
		protected readonly HandController HandController;
		protected readonly EnnemySensor EnnemySensor;
		protected readonly HitSensor HitSensor;
		protected readonly Health Health;
		protected State CurrentState;
		protected float TimeUntilStateSwitch;
		protected Vector3 MovementDirection;

		protected BaseEnemyBehavior(Mover mover, HandController handController,
			HitSensor hitSensor, Health health, EnnemySensor ennemySensor)
		{
			CurrentState = State.Idle;
			Mover = mover;
			HandController = handController;
			HitSensor = hitSensor;
			Health = health;
			EnnemySensor = ennemySensor;
			TimeUntilStateSwitch = 0;
			MovementDirection = new Vector3();
		}

		protected void MoveTowardsObject(GameObject target)
		{
			var direction = target.transform.position - Mover.transform.parent.position;
			Mover.MoveRelativeToSelf(direction);
		}

		protected void FleeFromObject(GameObject enemy)
		{
			var direction = Mover.transform.parent.position - enemy.transform.position;
			Mover.MoveRelativeToSelf(direction);
		}

		protected Vector3 GetRandomDirection()
		{
			return UnityEngine.Random.insideUnitCircle.normalized;
		}

		protected EnnemyController GetClosestEnnemy(IEnumerable<EnnemyController> ennemiesInSight)
		{
			EnnemyController closestEnnemy = null;
			var distance = float.MaxValue;
			foreach (var ennemy in ennemiesInSight)
			{
				if (closestEnnemy == null)
				{
					closestEnnemy = ennemy;
					distance = Vector3.Distance(closestEnnemy.transform.position,
						Mover.transform.parent.position);
				}
				else
				{
					var currentEnnemyDistance =
						Vector3.Distance(closestEnnemy.transform.position, ennemy.transform.position);
					if (distance > currentEnnemyDistance)
					{
						distance = currentEnnemyDistance;
						closestEnnemy = ennemy;
					}
				}
			}

			return closestEnnemy;
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

			UpdateNPCLogic();
		}

		protected abstract void UpdateNPCLogic();

		protected abstract State EvaluateIdle();

		protected abstract State EvaluateRoaming();

		protected abstract State EvaluateEngaging();

		protected abstract State EvaluateAttacking();

		protected abstract State EvaluateRetreating();
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Pickable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playmode.Npc.Strategies.BaseStrategies
{
	/// <summary>
	/// An NPC will change their current state depending on their behavior as well as current situation:
	/// </summary>
	public enum State
	{
		Idle,
		Roaming,
		Investigating,
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

		protected enum RetreatingRoutine
		{
			RunningBackwards,
			RotatingRight,
			RotatingLeft
		}

		private float currentSightRoutineDelay;
		private const float DefaultSightRoutineDelay = 0.5f;
		private float currentRetreatingRoutineDelay;

		protected SightRoutine CurrentSightRoutine;
		protected RetreatingRoutine CurrentRetreatingRoutine;

		protected readonly NpcSensorSound NpcSensorSound;
		protected readonly Mover Mover;
		protected readonly HandController HandController;
		protected readonly NpcSensorSight NpcSensorSight;
		protected readonly HitSensor HitSensor;
		protected readonly Health Health;
		protected readonly float DistanceSwitchFromEngagingToAttacking = 8f;
		protected readonly float DistanceSwitchFromAttackingToEngaging = 15f;
		protected State CurrentState;
		protected float TimeUntilStateSwitch;
		protected Vector3 MovementDirection;
		protected float DistanceToCurrentTarget;
		protected int RotationOrientation;
		protected NpcController CurrentEnemyTarget;
		protected PickableController CurrentPickableTarget;

		protected const float MinIdleTime = 0.2f;
		protected const float MaxIdleTime = 0.5f;
		protected const float MinRoamingTime = 1.2f;
		protected const float MaxRoamingTime = 2.8f;

		protected BaseNpcBehavior(Mover mover, HandController handController,
			HitSensor hitSensor, Health health, NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound)
		{
			CurrentState = State.Idle;
			Mover = mover;
			HandController = handController;
			HitSensor = hitSensor;
			Health = health;
			NpcSensorSight = npcSensorSight;
			TimeUntilStateSwitch = 0;
			MovementDirection = new Vector3();
			NpcSensorSound = npcSensorSound;
		}

		public void Act()
		{
			NpcSensorSound.UpdateSoundSensor(Mover.transform.root.position, Mover.transform.up);
			
			switch (CurrentState)
			{
				case State.Idle:
					CurrentState = EvaluateIdle();
					break;
				case State.Roaming:
					CurrentState = EvaluateRoaming();
					break;
				case State.Investigating:
					CurrentState = EvaluateInvestigating();
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
			Debug.Log(CurrentState.ToString());

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
				case State.Investigating:
					DoInvestigating();
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

		private void UpdateCurrentEnemyTarget()
		{
			if (CurrentEnemyTarget != null)
			{
				DistanceToCurrentTarget = Vector3.Distance(CurrentEnemyTarget.transform.position,
					Mover.transform.parent.position);
			}
		}

		protected void UpdateRetreatingRoutine(NpcController npcController)
		{
			if (currentRetreatingRoutineDelay > 0f)
			{
				currentRetreatingRoutineDelay -= Time.deltaTime;
			}
			else
			{
				CurrentRetreatingRoutine = RetreatingRoutine.RunningBackwards;
				var chanceOfRetreatingRoutine = Random.Range(1, 150);
				if (chanceOfRetreatingRoutine <= 1)
				{
					currentRetreatingRoutineDelay = Random.Range(0.8f, 1.2f);
					CurrentRetreatingRoutine = RetreatingRoutine.RotatingLeft;
				}
				else if (chanceOfRetreatingRoutine <= 2)
				{
					currentRetreatingRoutineDelay = Random.Range(0.8f, 1.2f);
					CurrentRetreatingRoutine = RetreatingRoutine.RotatingRight;
				}
			}

			switch (CurrentRetreatingRoutine)
			{
				case RetreatingRoutine.RunningBackwards:
					MoveAwayFromNpc(npcController);
					break;
				case RetreatingRoutine.RotatingRight:
					MoveRightAroundEnemy(npcController);
					break;
				case RetreatingRoutine.RotatingLeft:
					MoveLeftAroundEnemy(npcController);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
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
					CurrentSightRoutine =
						chanceOfSightRoutine <= 1 ? SightRoutine.LookingRight : SightRoutine.LookingLeft;
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

		private void StartSightRoutineDelay()
		{
			currentSightRoutineDelay = DefaultSightRoutineDelay;
			CurrentSightRoutine = SightRoutine.None;
		}

		protected void MoveTowardsNpc(NpcController npcController)
		{
			Mover.MoveRelativeToWorld(npcController.transform.parent.position - Mover.transform.parent.position);
		}

		protected void MoveTowardsPickable(PickableController pickableController)
		{
			Mover.MoveRelativeToWorld(pickableController.transform.parent.position - Mover.transform.parent.position);
		}

		protected void MoveTowardsDirection(Vector3 direction)
		{
			Mover.MoveRelativeToWorld(direction);
		}

		protected void MoveAwayFromNpc(NpcController npcController)
		{
			Mover.MoveRelativeToWorld(Mover.transform.parent.position - npcController.transform.parent.position);
		}

		protected void MoveRightAroundEnemy(NpcController npcController)
		{
			var directionTowardsEnemy = npcController.transform.parent.position - Mover.transform.parent.position;
			var perpendicularDirection =
				new Vector3(directionTowardsEnemy.y, -directionTowardsEnemy.x, directionTowardsEnemy.z);
			Mover.MoveRelativeToWorld(perpendicularDirection);
		}

		protected void MoveLeftAroundEnemy(NpcController npcController)
		{
			var directionTowardsEnemy = npcController.transform.parent.position - Mover.transform.parent.position;
			var perpendicularDirection =
				new Vector3(-directionTowardsEnemy.y, directionTowardsEnemy.x, directionTowardsEnemy.z);
			Mover.MoveRelativeToWorld(perpendicularDirection);
		}

		protected static Vector3 GetRandomDirection()
		{
			return Random.insideUnitCircle;
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

		protected void RotateTowardsPickable(PickableController pickableController)
		{
			Mover.RotateTowards(pickableController.transform.root.position - Mover.transform.root.position);
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

		protected PickableController GetClosestPickable(IEnumerable<PickableController> pickablesInSight)
		{
			PickableController closestPickable = null;
			var distance = float.MaxValue;
			foreach (var pickable in pickablesInSight)
			{
				if (closestPickable == null)
				{
					closestPickable = pickable;
					distance = Vector3.Distance(closestPickable.transform.position,
						Mover.transform.parent.position);
				}
				else
				{
					var currentPickableDistance =
						Vector3.Distance(closestPickable.transform.position, pickable.transform.position);
					if (distance > currentPickableDistance)
					{
						distance = currentPickableDistance;
						closestPickable = pickable;
					}
				}
			}

			DistanceToCurrentTarget = distance;
			return closestPickable;
		}

		public Vector3 GetClosestSoundPosition(Vector3 npcCurrentPosition)
		{
			var closestSoundDistance = float.MaxValue;
			var closestSoundPosition = new Vector3();
			foreach (var soundValue in NpcSensorSound.SoundsInformations)
			{
				if (Vector3.Magnitude(soundValue.Value - npcCurrentPosition) < closestSoundDistance)
				{
					closestSoundDistance = Vector3.Magnitude(soundValue.Value - npcCurrentPosition);
					closestSoundPosition = soundValue.Value;
				}
			}

			return closestSoundPosition;
		}
		
		protected Vector3 GetNewestSoundPosition()
		{
			return NpcSensorSound.SoundsInformations.Values.Last();
		}

		/// <summary>
		/// Reference for Predictive Aiming:
		/// 	https://www.gamasutra.com/blogs/KainShin/20090515/83954/Predictive_Aim_Mathematics_for_AI_Targeting.php
		/// </summary>
		protected Vector3 GetPredictiveAimDirection(NpcController npc)
		{
			var bulletSpeed = HandController.GetProjectileSpeed();
			var bulletSpeedSq = bulletSpeed * bulletSpeed;
			var bulletOrigin = HandController.GetWeaponPosition();
			var enemyInitialPosition = npc.transform.root.position;
			var enemyVelocity = npc.GetVelocity();
			var enemySpeed = enemyVelocity.magnitude;
			var enemySpeedSq = enemySpeed * enemySpeed;
			var enemyToBullet = bulletOrigin - enemyInitialPosition;
			var enemyToBulletDistance = enemyToBullet.magnitude;
			var enemyToBulletDistanceSq = enemyToBulletDistance * enemyToBulletDistance;
			var enemyToBulletDirection = enemyToBullet.normalized;
			var enemyVelocityDirection = enemyVelocity.normalized;

			//Law of Cosines: A*A + B*B - 2*A*B*cos(theta) = C*C
			//A is distance from bullet to enemy (known value: enemyToBulletDistance)
			//B is distance traveled by enemy until impact (enemySpeed * t (time))
			//C is distance traveled by bullet until impact (bulletSpeed * t (time))
			var cosTheta = Vector3.Dot(enemyToBulletDirection, enemyVelocityDirection);

			float t;

			// START VALIDATE PREDICTIVE AIM POSSIBLE
			var a = bulletSpeedSq - enemySpeedSq;
			var b = 2.0f * enemyToBulletDistance * enemySpeed * cosTheta;
			var c = -enemyToBulletDistanceSq;
			var discriminant = b * b - 4.0f * a * c;

			var uglyNumber = Mathf.Sqrt(discriminant);
			var t0 = 0.5f * (-b + uglyNumber) / a;
			var t1 = 0.5f * (-b - uglyNumber) / a;

			t = Mathf.Min(t0, t1);
			if (t < Mathf.Epsilon)
			{
				t = Mathf.Max(t0, t1);
			}

			if (t < Mathf.Epsilon)
			{
				// TODO: Validsolution not found;
				// t = PredictiveAimWildGuessAtImpactTime();
			}
			// END PREDICTIVE AIM VALIDATION

			var bulletVelocity = enemyVelocity + -enemyToBullet / t;


			return bulletVelocity;
		}

		protected abstract void DoIdle();

		protected abstract void DoRoaming();

		protected abstract void DoEngaging();

		protected abstract void DoInvestigating();

		protected abstract void DoAttacking();

		protected abstract void DoRetreating();

		protected abstract State EvaluateIdle();

		protected abstract State EvaluateRoaming();

		protected abstract State EvaluateInvestigating();

		protected abstract State EvaluateEngaging();

		protected abstract State EvaluateAttacking();

		protected abstract State EvaluateRetreating();
	}
}
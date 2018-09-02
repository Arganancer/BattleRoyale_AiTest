using System;
using System.Collections.Generic;
using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Pickable;
using Playmode.Pickable.TypePickable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playmode.Npc.Strategies.BaseStrategyClasses
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

	public abstract class BaseNpcBehavior : INpcStrategy
	{
		private State currentState;

		protected readonly NpcSensorSound NpcSensorSound;
		protected readonly Mover Mover;
		protected readonly HandController HandController;
		protected readonly NpcSensorSight NpcSensorSight;
		protected readonly HitSensor HitSensor;
		protected readonly Health Health;

		protected float DistanceSwitchFromEngagingToAttacking = 8f;
		protected float DistanceSwitchFromAttackingToEngaging = 15f;
		protected float DistanceToCurrentEnemy;
		protected Vector3 MovementDirection;
		protected float RotationOrientation;

		protected float HealthRetreatTolerance;
		protected float TimeUntilStateSwitch = 0f;
		protected bool IsOutsideOfZone = false;

		protected NpcController CurrentEnemyTarget;
		protected PickableController CurrentMedicalKitTarget;
		protected PickableController CurrentShotgunTarget;
		protected PickableController CurrentUziTarget;

		protected const float MinIdleTime = 0.2f;
		protected const float MaxIdleTime = 0.5f;
		protected const float MinRoamingTime = 1.2f;
		protected const float MaxRoamingTime = 2.8f;

		protected BaseNpcBehavior(Mover mover, HandController handController,
			HitSensor hitSensor, Health health, NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound)
		{
			currentState = State.Idle;
			Mover = mover;
			HandController = handController;
			HitSensor = hitSensor;
			Health = health;
			NpcSensorSight = npcSensorSight;
			MovementDirection = new Vector3();
			NpcSensorSound = npcSensorSound;
		}

		public void Act()
		{
			NpcSensorSound.UpdateSoundSensor(Mover.transform.root.position, Mover.transform.up);
			UpdateTargetInformation();
			UpdatNpcState();
			UpdateNpcAction();
		}

		private void UpdatNpcState()
		{
			switch (currentState)
			{
				case State.Idle:
					currentState = EvaluateIdle();
					break;
				case State.Roaming:
					currentState = EvaluateRoaming();
					break;
				case State.Investigating:
					currentState = EvaluateInvestigating();
					break;
				case State.Engaging:
					currentState = EvaluateEngaging();
					break;
				case State.Attacking:
					currentState = EvaluateAttacking();
					break;
				case State.Retreating:
					currentState = EvaluateRetreating();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void UpdateNpcAction()
		{
			switch (currentState)
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
		
		private void UpdateTargetInformation()
		{
			if (NpcSensorSight.NpcsInSight.Any() && CurrentEnemyTarget == null)
			{
				CurrentEnemyTarget = NpcSensorSight.GetClosestNpc();
			}

			if (NpcSensorSight.PickablesInSight.Any() && CurrentMedicalKitTarget == null)
			{
				CurrentMedicalKitTarget = NpcSensorSight.GetClosestPickable(TypePickable.Medicalkit);
			}

			if (NpcSensorSight.PickablesInSight.Any() && CurrentShotgunTarget == null)
			{
				CurrentShotgunTarget = NpcSensorSight.GetClosestPickable(TypePickable.Shotgun);
			}

			if (NpcSensorSight.PickablesInSight.Any() && CurrentUziTarget == null)
			{
				CurrentUziTarget = NpcSensorSight.GetClosestPickable(TypePickable.Uzi);
			}

			UpdateCurrentEnemyDistance();
		}

		private void UpdateCurrentEnemyDistance()
		{
			if (CurrentEnemyTarget != null)
			{
				DistanceToCurrentEnemy = Vector3.Distance(CurrentEnemyTarget.transform.position,
					Mover.transform.parent.position);
			}
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

			float time;

			var a = bulletSpeedSq - enemySpeedSq;
			var b = 2.0f * enemyToBulletDistance * enemySpeed * cosTheta;
			var c = -enemyToBulletDistanceSq;
			var discriminant = b * b - 4.0f * a * c;

			var uglyNumber = Mathf.Sqrt(discriminant);
			var t0 = 0.5f * (-b + uglyNumber) / a;
			var t1 = 0.5f * (-b - uglyNumber) / a;

			time = Mathf.Min(t0, t1);
			if (time < Mathf.Epsilon)
			{
				time = Mathf.Max(t0, t1);
			}

			var bulletVelocity = enemyVelocity + -enemyToBullet / time;

			return bulletVelocity;
		}

		protected abstract void DoIdle();

		protected abstract void DoRoaming();

		protected abstract void DoEngaging();

		protected abstract void DoInvestigating();

		protected abstract void DoAttacking();

		protected abstract void DoRetreating();

		protected virtual State EvaluateIdle()
		{
			if (TimeUntilStateSwitch > MaxIdleTime)
			{
				TimeUntilStateSwitch = Random.Range(MinIdleTime, MaxIdleTime);
			}
			
			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				MovementDirection = Mover.GetRandomDirection();
				TimeUntilStateSwitch = Random.Range(MinRoamingTime, MaxRoamingTime);
				return State.Roaming;
			}

			return State.Idle;
		}

		protected virtual State EvaluateRoaming()
		{
			if (TimeUntilStateSwitch > MaxRoamingTime)
			{
				TimeUntilStateSwitch = Random.Range(MinRoamingTime, MaxRoamingTime);
			}
			
			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				RotationOrientation = Random.Range(-1, 1);

				TimeUntilStateSwitch = Random.Range(MinIdleTime, MaxRoamingTime);
				return State.Idle;
			}

			return State.Roaming;
		}

		protected abstract State EvaluateInvestigating();

		protected abstract State EvaluateEngaging();

		protected abstract State EvaluateAttacking();

		protected abstract State EvaluateRetreating();
	}
}
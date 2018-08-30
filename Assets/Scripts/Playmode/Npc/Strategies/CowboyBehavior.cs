﻿using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Pickable.TypePickable;
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
			RotateTowardsAngle(RotationOrientation);
		}

		protected override void DoRoaming()
		{
			UpdateSightRoutine();
			MoveTowardsDirection(MovementDirection);
		}

		protected override void DoInvestigating()
		{
			MovementDirection = GetNewestSoundPosition() - Mover.transform.root.position;
			UpdateSightRoutine();
			MoveTowardsDirection(MovementDirection);
		}

		protected override void DoEngaging()
		{
			// Evaluate Surroundings
			if (NpcSensorSight.NpcsInSight.Any() && CurrentEnemyTarget == null)
			{
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);
			}

			if (NpcSensorSight.PickablesInSight.Any() && CurrentPickableTarget == null)
			{
				var pickableToEvaluate = GetClosestPickable(NpcSensorSight.PickablesInSight);
				if (pickableToEvaluate.GetPickableType() != TypePickable.Medicalkit)
					CurrentPickableTarget = pickableToEvaluate;
			}

			// Make Decision
			if (CurrentPickableTarget != null)
			{
				if (CurrentEnemyTarget != null)
				{
					RotateTowardsNpc(CurrentEnemyTarget);
					HandController.Use();
				}
				else
				{
					RotateTowardsPickable(CurrentPickableTarget);
				}

				MoveTowardsPickable(CurrentPickableTarget);
			}
			else
			{
				RotateTowardsNpc(CurrentEnemyTarget);
				MoveTowardsNpc(CurrentEnemyTarget);
				HandController.Use();
			}
		}

		protected override void DoAttacking()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);

			if (CurrentPickableTarget != null && CurrentPickableTarget.GetPickableType() != TypePickable.Medicalkit)
			{
				RotateTowardsDirection(CurrentPickableTarget.transform.position);
				MoveTowardsDirection(CurrentPickableTarget.transform.position);
			}
			else
			{
				MoveTowardsDirection(CurrentEnemyTarget.transform.position);
				RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
				
				HandController.Use();
			}
		}

		protected override void DoRetreating()
		{
			// A cowboy never retreats ಠ_ಠ
		}

		protected override State EvaluateIdle()
		{
			if (TimeUntilStateSwitch > MaxIdleTime)
			{
				TimeUntilStateSwitch = Random.Range(MinIdleTime, MaxIdleTime);
			}

			if (NpcSensorSight.PickablesInSight.Any())
			{
				if (GetClosestPickable(NpcSensorSight.PickablesInSight).GetPickableType() != TypePickable.Medicalkit)
					return State.Engaging;
			}

			if (NpcSensorSight.NpcsInSight.Any())
			{
				return State.Engaging;
			}

			if (NpcSensorSound.SoundsInformations.Any())
			{
				return State.Investigating;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				MovementDirection = GetRandomDirection();
				TimeUntilStateSwitch = Random.Range(MinRoamingTime, MaxRoamingTime);
				return State.Roaming;
			}

			return State.Idle;
		}

		protected override State EvaluateRoaming()
		{
			if (TimeUntilStateSwitch > MaxRoamingTime)
			{
				TimeUntilStateSwitch = Random.Range(MinRoamingTime, MaxRoamingTime);
			}

			if (NpcSensorSight.PickablesInSight.Any())
			{
				if (GetClosestPickable(NpcSensorSight.PickablesInSight).GetPickableType() != TypePickable.Medicalkit)
				{
					return State.Engaging;
				}
			}

			if (NpcSensorSight.NpcsInSight.Any())
			{
				return State.Engaging;
			}

			if (NpcSensorSound.SoundsInformations.Any())
			{
				return State.Investigating;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				TimeUntilStateSwitch = Random.Range(MinIdleTime, MaxIdleTime);
				return State.Idle;
			}

			return State.Roaming;
		}

		protected override State EvaluateInvestigating()
		{
			if (NpcSensorSight.NpcsInSight.Any())
			{
				return State.Engaging;
			}

			if (!NpcSensorSound.SoundsInformations.Any())
			{
				return State.Idle;
			}

			return State.Investigating;
		}

		protected override State EvaluateEngaging()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return DistanceToCurrentTarget > DistanceSwitchFromEngagingToAttacking ? State.Engaging : State.Attacking;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return DistanceToCurrentTarget < DistanceSwitchFromEngagingToAttacking ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateRetreating()
		{
			return State.Idle;
		}
	}
}
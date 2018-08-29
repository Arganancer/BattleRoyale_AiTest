using System;
using System.Linq;
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
	public class CarefulBehavior : BaseNpcBehavior
	{
		[SerializeField] private float healthPercentageToLose = 50f;
		[SerializeField] private float safeDistance = 100f;

		public CarefulBehavior(Mover mover, HandController handController, HitSensor hitSensor, Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound) : base(mover, handController, hitSensor,
			health, npcSensorSight, npcSensorSound)
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

		// TODO: Remove this section
//		private RetreatingRoutine movementDirection = RetreatingRoutine.RotatingRight;
//		private int nbOfFramesMoved = 0;
//		private void VibrateRoutine()
//		{
//			switch (movementDirection)
//			{
//				case RetreatingRoutine.RunningBackwards:
//					break;
//				case RetreatingRoutine.RotatingRight:
//					MoveRightAroundEnemy(CurrentEnemyTarget);
//					if (--nbOfFramesMoved <= 0)
//					{
//						nbOfFramesMoved = 5;
//						movementDirection = RetreatingRoutine.RotatingLeft;
//					}
//
//					break;
//				case RetreatingRoutine.RotatingLeft:
//					MoveLeftAroundEnemy(CurrentEnemyTarget);
//					if (--nbOfFramesMoved <= 0)
//					{
//						nbOfFramesMoved = 5;
//						movementDirection = RetreatingRoutine.RotatingRight;
//					}
//
//					break;
//				default:
//					throw new ArgumentOutOfRangeException();
//			}
//		}

		protected override void DoEngaging()
		{
			// Evaluate
			if (NpcSensorSight.NpcsInSight.Any() && CurrentEnemyTarget == null)
			{
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);
			}

			if (NpcSensorSight.PickablesInSight.Any() && CurrentPickableTarget == null)
			{
				CurrentPickableTarget = GetClosestPickable(NpcSensorSight.PickablesInSight);
			}
			
			// Decision
			if (Health.HealthPoints % healthPercentageToLose <= 0)
			{
				if (CurrentPickableTarget != null && CurrentPickableTarget.GetPickableType() == TypePickable.Medicalkit)
				{
					RotateTowardsPickable(CurrentPickableTarget);
					MoveTowardsPickable(CurrentPickableTarget);
				}

				if (CurrentEnemyTarget != null)
				{
					//TODO: change for UpdateRetreatingRoutine?
					CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);
					RotateTowardsNpc(CurrentEnemyTarget);
					MoveAwayFromNpc(CurrentEnemyTarget);
					
					HandController.Use();
				}
			}
			else
			{
				if (CurrentEnemyTarget != null)
				{
					RotateTowardsNpc(CurrentEnemyTarget);
					MoveTowardsNpc(CurrentEnemyTarget);
					
					HandController.Use();
				}

				if (CurrentPickableTarget != null && CurrentPickableTarget.GetPickableType() != TypePickable.Medicalkit)
				{
					RotateTowardsPickable(CurrentPickableTarget);
					MoveTowardsPickable(CurrentPickableTarget);
				}
			}
		}

		protected override void DoAttacking()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);

			MoveRightAroundEnemy(CurrentEnemyTarget);
			float distance = Vector3.Distance(CurrentEnemyTarget.transform.position, Mover.transform.position);
			if (distance >= safeDistance)
			{
				MoveTowardsDirection(CurrentEnemyTarget.transform.position);
			}
			else
			{
				MoveAwayFromNpc(CurrentEnemyTarget);
			}
			
			MoveTowardsNpc(CurrentEnemyTarget);
			
			RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));

			HandController.Use();
		}

		protected override void DoRetreating()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);

			RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			UpdateRetreatingRoutine(GetClosestNpc(NpcSensorSight.NpcsInSight));
			MoveTowardsNpc(CurrentEnemyTarget);

			HandController.Use();
		}

		protected override State EvaluateIdle()
		{
			if (TimeUntilStateSwitch > MaxIdleTime)
			{
				TimeUntilStateSwitch = Random.Range(MinIdleTime, MaxIdleTime);
			}

			if (NpcSensorSight.PickablesInSight.Any())
			{
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
				while (RotationOrientation != 0)
				{
					RotationOrientation = Random.Range(-1, 1);
				}

				TimeUntilStateSwitch = Random.Range(MinIdleTime, MaxRoamingTime);
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
			if (Health.HealthPoints % healthPercentageToLose >= healthPercentageToLose && !NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return State.Retreating;
		}
	}
}
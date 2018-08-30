using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Pickable.TypePickable;
using UnityEngine;

namespace Playmode.Npc.Strategies
{
	public class CamperBehavior : BaseNpcBehavior
	{
		[SerializeField] private float healthPointsToLose = 70f;
		[SerializeField] private float distanceToStay = 2f;
		
		public CamperBehavior(Mover mover, HandController handController, HitSensor hitSensor, Health health,
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
			if (Health.HealthPoints % healthPointsToLose <= 0 && CurrentPickableTarget != null)
			{
				RotateTowardsPickable(CurrentPickableTarget);
				MoveTowardsPickable(CurrentPickableTarget);
				
				if (CurrentEnemyTarget != null)
				{
					MoveAwayFromNpc(CurrentEnemyTarget);
					HandController.Use();
				}
			}
			else
			{
				if (CurrentPickableTarget != null)
				{
					if (CurrentPickableTarget.GetPickableType() == TypePickable.Medicalkit)
					{
						Vector3 direction = (CurrentPickableTarget.transform.position - Mover.transform.position);
						direction += new Vector3(distanceToStay, distanceToStay);
						
						RotateTowardsDirection(direction);
						MoveTowardsDirection(direction);

						if (CurrentEnemyTarget != null)
						{
							RotateTowardsNpc(CurrentEnemyTarget);
							HandController.Use();
						}
					}
					else
					{
						RotateTowardsPickable(CurrentPickableTarget);
						MoveTowardsPickable(CurrentPickableTarget);
					}
				}
				else if (CurrentEnemyTarget != null)
				{
					RotateTowardsNpc(CurrentEnemyTarget);
					MoveTowardsNpc(CurrentEnemyTarget);
					HandController.Use();
				}
			}
		}

		protected override void DoAttacking()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);

			if (CurrentPickableTarget != null)
			{
				RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			}
			else
			{
				MoveTowardsDirection(CurrentEnemyTarget.transform.position);
				RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			}

			HandController.Use();
		}

		protected override void DoRetreating()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);
			
			RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			UpdateRetreatingRoutine(GetClosestNpc(NpcSensorSight.NpcsInSight));
			
			HandController.Use();
		}

		protected override State EvaluateIdle()
		{
			if (TimeUntilStateSwitch > MaxIdleTime)
			{
				TimeUntilStateSwitch = Random.Range(MinIdleTime, MaxIdleTime);
			}

			if (NpcSensorSight.PickablesInSight.Any() || NpcSensorSight.NpcsInSight.Any())
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

			if (NpcSensorSight.PickablesInSight.Any() || NpcSensorSight.NpcsInSight.Any())
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
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return State.Retreating;
		}
	}
}
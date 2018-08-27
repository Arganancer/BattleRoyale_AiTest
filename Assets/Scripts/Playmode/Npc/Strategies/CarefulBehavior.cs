using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategies;
using Playmode.Pickable.TypePickable;
using UnityEngine;

namespace Playmode.Npc.Strategies
{
	public class CarefulBehavior : BaseNpcBehavior
	{
		[SerializeField] private int healthPointsToLose = 50;

		public CarefulBehavior(Mover mover, HandController handController, HitSensor hitSensor, Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound) : base(mover, handController, hitSensor,
			health, npcSensorSight, npcSensorSound)
		{
			AttackingDistance = 10f;
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
			//TODO: changer pour un % de vie
			if (Health.HealthPoints <= healthPointsToLose)
			{
				if (NpcSensorSight.PickablesInSight.Any() && CurrentPickableTarget == null)
				{
					var pickableToEvaluate = GetClosestPickable(NpcSensorSight.PickablesInSight);
					if (pickableToEvaluate.GetPickableType() == TypePickable.Medicalkit)
					{
						CurrentPickableTarget = pickableToEvaluate;
						RotateTowardsPickable(CurrentPickableTarget);
						MoveTowardsPickable(CurrentPickableTarget);
					}
				}

				if (NpcSensorSight.NpcsInSight.Any() && CurrentEnemyTarget == null)
				{
					CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);
					MoveAwayFromNpc(CurrentEnemyTarget);
				}
			}

			else
			{
				if (NpcSensorSight.NpcsInSight.Any() && CurrentEnemyTarget == null)
				{
					CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);
					
					RotateTowardsNpc(CurrentEnemyTarget);
					MoveTowardsNpc(CurrentEnemyTarget);
					
					HandController.Use();
				}

				if (NpcSensorSight.PickablesInSight.Any() && CurrentPickableTarget == null)
				{
					CurrentPickableTarget = GetClosestPickable(NpcSensorSight.PickablesInSight);
					
					RotateTowardsDirection(CurrentPickableTarget.transform.position);
					MoveTowardsPickable(CurrentPickableTarget);
				}
			}
		}

		protected override void DoAttacking()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);

			RotateTowardsNpc(CurrentEnemyTarget);

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
			
			if (NpcSensorSight.NpcsInSight.Any() || NpcSensorSight.PickablesInSight.Any())
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
				TimeUntilStateSwitch = Random.Range(4f, 6f);
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
			
			if (NpcSensorSight.NpcsInSight.Any() || NpcSensorSight.PickablesInSight.Any())
			{
				return State.Attacking;
			}
			
			if (NpcSensorSound.SoundsInformations.Any())
			{
				return State.Investigating;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				MovementDirection = GetRandomDirection();
				TimeUntilStateSwitch = Random.Range(4f, 6f);
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
			
			return DistanceToCurrentTarget > AttackingDistance ? State.Engaging : State.Attacking;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return DistanceToCurrentTarget < AttackingDistance ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateRetreating()
		{
			if (Health.HealthPoints >= healthPointsToLose && !NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return State.Retreating;
		}
	}
}
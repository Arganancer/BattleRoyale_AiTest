using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playmode.Npc.Strategies
{
	public class TestStrategy : BaseNpcBehavior
	{
		public TestStrategy(Mover mover, HandController handController, HitSensor hitSensor, Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound) : base(mover, handController, hitSensor,
			health, npcSensorSight, npcSensorSound)
		{
		}

		#region DoTheStuff

		protected override void DoIdle()
		{
			Mover.RotateTowardsAngle(RotationOrientation);
		}

		protected override void DoRoaming()
		{
			UpdateSightRoutine();
			Mover.MoveTowardsDirection(MovementDirection);
		}
		
		protected override void DoInvestigating()
		{
			MovementDirection = GetNewestSoundPosition() - Mover.transform.root.position;
			
			UpdateSightRoutine();
			Mover.MoveTowardsDirection(MovementDirection);
		}

		protected override void DoEngaging()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);

			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));

			Mover.MoveTowardsPosition(CurrentEnemyTarget.transform.root.position);

			HandController.Use();
		}

		protected override void DoAttacking()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);

			Mover.MoveRightAroundPosition(CurrentEnemyTarget.transform.root.position);
			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));

			HandController.Use();
		}

		protected override void DoRetreating()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);

			Mover.RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			UpdateRetreatingRoutine(GetClosestNpc(NpcSensorSight.NpcsInSight));

			HandController.Use();
		}

		#endregion

		#region Evaluate

		protected override State EvaluateIdle()
		{
			if (TimeUntilStateSwitch > MaxIdleTime)
			{
				TimeUntilStateSwitch = Random.Range(MinIdleTime, MaxIdleTime);
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
				MovementDirection = Mover.GetRandomDirection();
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

			if (Health.HealthPoints < 50)
			{
				return State.Retreating;
			}

			return DistanceToCurrentEnemy < DistanceSwitchFromEngagingToAttacking ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}

			if (Health.HealthPoints < 80)
			{
				return State.Retreating;
			}

			return DistanceToCurrentEnemy < DistanceSwitchFromEngagingToAttacking ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateRetreating()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				TimeUntilStateSwitch = MaxRoamingTime;
				return State.Roaming;
			}

			return State.Retreating;
		}

		#endregion
	}
}
using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategies;
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
			AttackingDistance = 5f;
		}

		#region DoTheStuff

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
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);

			RotateTowardsNpc(CurrentEnemyTarget);
			MoveTowardsNpc(CurrentEnemyTarget);

			HandController.Use();
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

			RotateTowardsNpc(CurrentEnemyTarget);
			UpdateRetreatingRoutine(CurrentEnemyTarget);

			HandController.Use();
		}

		#endregion

		#region Evaluate

		protected override State EvaluateIdle()
		{
			if (NpcSensorSight.NpcsInSight.Any())
			{
				return State.Engaging;
			}
			
			if (NpcSensorSound.SoundsInformations.Any())
			{
				// TODO: Remove
				Debug.Log("Investigating");
				return State.Investigating;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				MovementDirection = GetRandomDirection();
				TimeUntilStateSwitch = Random.Range(1.2f, 3.5f);
				return State.Roaming;
			}

			return State.Idle;
		}

		protected override State EvaluateRoaming()
		{
			if (NpcSensorSight.NpcsInSight.Any())
			{
				return State.Engaging;
			}

			if (NpcSensorSound.SoundsInformations.Any())
			{
				// TODO: Remove
				Debug.Log("Investigating");
				return State.Investigating;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				while (RotationOrientation != 0)
				{
					RotationOrientation = Random.Range(-1, 1);
				}

				TimeUntilStateSwitch = Random.Range(0.2f, 0.6f);
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
				// TODO: Remove
				Debug.Log("Stopping Investigation");
				return State.Idle;
			}

			return State.Investigating;
		}

		protected override State EvaluateEngaging()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				TimeUntilStateSwitch = Random.Range(0.2f, 0.5f);
				return State.Idle;
			}

			if (Health.HealthPoints < 50)
			{
				return State.Retreating;
			}

			return DistanceToCurrentTarget < AttackingDistance ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				TimeUntilStateSwitch = Random.Range(0.2f, 0.5f);
				return State.Idle;
			}

			if (Health.HealthPoints < 80)
			{
				return State.Retreating;
			}

			return DistanceToCurrentTarget < AttackingDistance ? State.Attacking : State.Engaging;
		}

		protected override State EvaluateRetreating()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				TimeUntilStateSwitch = Random.Range(0.2f, 0.5f);
				return State.Idle;
			}

			return State.Retreating;
		}

		#endregion
	}
}
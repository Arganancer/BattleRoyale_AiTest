using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategies;
using Playmode.Pickable;
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

		protected override void DoEngaging()
		{
			//TODO: suspicious comparison
			
			if (CurrentPickableTarget == null)
			{
				PickableController pickableToEvaluate = GetClosestPickable(NpcSensorSight.PickablesInSight);
				if (!pickableToEvaluate.Equals(TypePickable.Medicalkit))
					CurrentPickableTarget = pickableToEvaluate;
				
				RotateTowardsPickable(CurrentPickableTarget);
				MoveTowardsPickable(CurrentPickableTarget);
			}

			if (CurrentPickableTarget == null && CurrentEnemyTarget == null)
			{
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);
				
				RotateTowardsNpc(CurrentEnemyTarget);
				MoveTowardsNpc(CurrentEnemyTarget);
				
				HandController.Use();
			}
		}

		protected override void DoInvestigating()
		{
			MovementDirection = GetNewestSoundPosition() - Mover.transform.root.position;
			UpdateSightRoutine();
			MoveTowardsDirection(MovementDirection);
		}

		protected override void DoAttacking()
		{
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);

			RotateTowardsNpc(CurrentEnemyTarget);
			MoveTowardsNpc(CurrentEnemyTarget);

			HandController.Use();
		}

		protected override void DoRetreating()
		{
			//TODO: never retreat
		}

		protected override State EvaluateIdle()
		{
			//TODO: ne doit pas evaluer les medical kits
			if (NpcSensorSight.PickablesInSight.Any())
			{
				if(!NpcSensorSight.PickablesInSight.First().Equals(TypePickable.Medicalkit))
					return State.Engaging;
			}
			
			if (NpcSensorSight.NpcsInSight.Any())
			{
				return State.Attacking;
			}

			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				TimeUntilStateSwitch = Random.Range(4f, 6f);
				return State.Roaming;
			}

			return State.Idle;
		}

		protected override State EvaluateRoaming()
		{
			//TODO: ne doit pas evaluer les medical kits
			if (NpcSensorSight.PickablesInSight.Any())
			{
				return State.Engaging;
			}
			
			if (NpcSensorSight.NpcsInSight.Any())
			{
				return State.Attacking;
			}
			
			TimeUntilStateSwitch -= Time.deltaTime;
			if (TimeUntilStateSwitch <= 0)
			{
				TimeUntilStateSwitch = Random.Range(0.5f, 1.5f);
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
			
			return State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}
			
			return State.Attacking;
		}

		protected override State EvaluateRetreating()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}
			
			return State.Attacking;
		}
	}
}
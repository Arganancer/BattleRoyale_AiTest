using System.Linq;
using System.Runtime.InteropServices;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategies;
using Playmode.Pickable.TypePickable;
using UnityEngine;

namespace Playmode.Npc.Strategies
{
	public class CamperBehavior : BaseNpcBehavior
	{
		[SerializeField] private float healthPercentageToLose = 70f;
		
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
			throw new System.NotImplementedException();
		}

		protected override void DoEngaging()
		{
			var pickableToEvaluate = GetClosestPickable(NpcSensorSight.PickablesInSight);

			if (Health.HealthPoints % healthPercentageToLose <= 0 && CurrentPickableTarget != null)
			{
				if (CurrentEnemyTarget != null)
				{
					//TODO: pick the pickable and look for another one
					MoveAwayFromNpc(CurrentEnemyTarget);
				}
			}
			
			if (NpcSensorSight.PickablesInSight.Any() && CurrentPickableTarget == null)
			{
				if (pickableToEvaluate.GetPickableType() == TypePickable.Medicalkit)
				{
					//TODO: doit rester a coté
				}
				else
				{
					CurrentPickableTarget = pickableToEvaluate;
					
					RotateTowardsPickable(CurrentPickableTarget);
					MoveTowardsPickable(CurrentPickableTarget);
				}
			}
		}

		protected override void DoAttacking()
		{
			throw new System.NotImplementedException();
		}

		protected override void DoRetreating()
		{
			throw new System.NotImplementedException();
		}

		protected override State EvaluateIdle()
		{
			throw new System.NotImplementedException();
		}

		protected override State EvaluateRoaming()
		{
			throw new System.NotImplementedException();
		}

		protected override State EvaluateInvestigating()
		{
			throw new System.NotImplementedException();
		}

		protected override State EvaluateEngaging()
		{
			throw new System.NotImplementedException();
		}

		protected override State EvaluateAttacking()
		{
			throw new System.NotImplementedException();
		}

		protected override State EvaluateRetreating()
		{
			throw new System.NotImplementedException();
		}
	}
}
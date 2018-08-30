using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using UnityEngine;

namespace Playmode.Npc.Strategies
{
	public class NormalBehavior : BaseNpcBehavior
	{
		/*private bool hasTarget;
		private readonly Mover mover;
		private readonly HandController handController;
		private NpcController npcController;
		private NpcSensor npcSensor;

		public NormalBehaviour(Mover mover, HandController handController)
		{
			this.mover = mover;
			this.handController = handController;
			npcController = null;
		}

		public void Act()
		{
		}

		public void MoveAndShootTowardTheNpc(Vector2 direction)
		{
			mover.MoveRelativeToSelf(direction * Time.deltaTime);
			handController.Use();
		}

		public void ReactToLooseOfNpcSight(NpcController npc)
		{
			if (npc == npcController)
			{
				npcController = null;
			}
		}*/

		[SerializeField] private int healthPointsToLose = 20;

		public NormalBehavior(Mover mover, HandController handController, HitSensor hitSensor, Health health,
			NpcSensorSight npcSensorSight, NpcSensorSound npcSensorSound) : base(mover, handController, hitSensor,
			health, npcSensorSight, npcSensorSound)
		{
			DistanceSwitchFromAttackingToEngaging = 3f;
			DistanceSwitchFromEngagingToAttacking = 4f;
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
			if (CurrentEnemyTarget == null)
				CurrentEnemyTarget = GetClosestNpc(NpcSensorSight.NpcsInSight);

			RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));

			MoveTowardsNpc(CurrentEnemyTarget);

			HandController.Use();
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

			//MoveRightAroundEnemy(CurrentEnemyTarget);
			RotateTowardsDirection(GetPredictiveAimDirection(CurrentEnemyTarget));
			HandController.Use();
		}

		protected override void DoRetreating()
		{
			// only goes forward
		}

		protected override State EvaluateIdle()
		{
			if (NpcSensorSight.NpcsInSight.Any())
			{
				return State.Attacking;
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

			if (DistanceToCurrentTarget < DistanceSwitchFromEngagingToAttacking)
			{
				return State.Attacking;
			}
			return State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensorSight.NpcsInSight.Any())
			{
				return State.Idle;
			}

			if (DistanceToCurrentTarget > DistanceSwitchFromAttackingToEngaging)
			{
				return State.Engaging;
			}
			return State.Attacking;
		}

		protected override State EvaluateRetreating()
		{
			return State.Idle;
		}
	}
}
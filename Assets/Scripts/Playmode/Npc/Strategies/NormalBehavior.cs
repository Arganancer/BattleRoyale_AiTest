using System.Linq;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
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
		
		public NormalBehavior(Mover mover, HandController handController, HitSensor hitSensor, Health health, NpcSensor npcSensor) : base(mover, handController, hitSensor, health, npcSensor)
		{
		}

		protected override void DoIdle()
		{
			throw new System.NotImplementedException();
		}

		protected override void DoRoaming()
		{
			throw new System.NotImplementedException();
		}

		protected override void DoEngaging()
		{
			throw new System.NotImplementedException();
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
			if (NpcSensor.NpcsInSight.Any())
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
			if (NpcSensor.NpcsInSight.Any())
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

		protected override State EvaluateEngaging()
		{
			if (!NpcSensor.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return State.Engaging;
		}

		protected override State EvaluateAttacking()
		{
			if (!NpcSensor.NpcsInSight.Any())
			{
				return State.Idle;
			}

			return State.Attacking;
		}

		protected override State EvaluateRetreating()
		{
			//if (Health.HealthPoints >= healthPointsToLose)
			//{
				return State.Attacking;
			//}
			
			//return State.Retreating;
		}
	}
}
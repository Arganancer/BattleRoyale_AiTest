using Playmode.Ennemy.BodyParts;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using UnityEngine;

namespace Playmode.Ennemy.Strategies
{
	public class NormalBehaviour : IEnnemyStrategy
	{
		private bool hasTarget;
		private readonly Mover mover;
		private readonly HandController handController;
		private EnnemyController ennemyController;
		private EnnemySensor ennemySensor;

		public NormalBehaviour(Mover mover, HandController handController)
		{
			this.mover = mover;
			this.handController = handController;
			ennemyController = null;
		}

		public void Act()
		{
			if (ennemyController != null)
			{
				Vector2 direction = GetDirectionTowardTheEnemy();
				MoveAndShootTowardTheEnemy(direction);
			}
			else
			{
				mover.MoveRelativeToSelf(Vector2.down);
			}
		}

		public void ReactToEnemyInSight(EnnemyController ennemy)
		{
			if (ennemyController == null)
			{
				ennemyController = ennemy;
			}
		}

		public void MoveAndShootTowardTheEnemy(Vector2 direction)
		{
			mover.MoveRelativeToSelf(direction * Time.deltaTime);
			handController.Use();
		}

		private Vector2 GetDirectionTowardTheEnemy()
		{
			return ennemyController.transform.position - mover.transform.position;
		}

		public void ReactToLooseOfEnemySight(EnnemyController enemy)
		{
			if (enemy == ennemyController)
			{
				ennemyController = null;
			}
		}
	}
}
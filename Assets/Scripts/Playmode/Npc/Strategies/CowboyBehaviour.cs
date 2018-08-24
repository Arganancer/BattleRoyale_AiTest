using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Npc.BodyParts;
using UnityEngine;

namespace Playmode.Npc.Strategies
{
	public class CowboyBehaviour : INpcStrategy
	{
		private readonly Mover mover;
		private readonly HandController handController;
		private NpcController npcController;
		private GameObject gameObject;
		private NpcSensor npcSensor;

		private bool hasNpcTarget;
		private bool hasMunitionTarget;

		public CowboyBehaviour(Mover mover, HandController handController)
		{
			this.mover = mover;
			this.handController = handController;
		}

		public void Act()
		{
			var position = mover.transform.position;

			if (hasNpcTarget)
			{
				npcSensor.See(npcController);
				mover.MoveRelativeToSelf(npcController.transform.position - position);
			}
			else if (hasMunitionTarget)
			{
				mover.MoveRelativeToSelf(gameObject.transform.position - position);
			}
		}
	}
}
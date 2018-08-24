using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Npc.BodyParts;
using UnityEngine;

namespace Playmode.Npc.Strategies
{
	public class NormalBehaviour : INpcStrategy
	{
		private bool hasTarget;
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
		}
	}
}
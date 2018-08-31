using System.Collections;
using System.Collections.Generic;
using Playmode.Entity.Senses;
using Playmode.Npc;
using UnityEngine;

namespace Playmode.Pickable.TypePickable
{
	public abstract class PickableBonusEffect
	{
		public PickableBonusEffect()
		{
		}

		public abstract void ApplyBonusEffect(NpcController npcController);
	}
}


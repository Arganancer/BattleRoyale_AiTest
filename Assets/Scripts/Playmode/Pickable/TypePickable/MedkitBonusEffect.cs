using System.Collections;
using System.Collections.Generic;
using Playmode.Entity.Senses;
using Playmode.Npc;
using UnityEngine;

namespace Playmode.Pickable.TypePickable
{
	public class MedkitBonusEffect : PickableBonusEffect {

		public MedkitBonusEffect()
		{
			
		}

		public override void ApplyBonusEffect(NpcController npcController)
		{
			npcController.Heal(10);
		}
	}
}


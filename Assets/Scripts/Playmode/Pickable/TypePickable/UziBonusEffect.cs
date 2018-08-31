using System.Collections;
using System.Collections.Generic;
using Playmode.Entity.Senses;
using Playmode.Npc;
using UnityEngine;

namespace Playmode.Pickable.TypePickable
{
	public class UziBonusEffect : PickableBonusEffect {


		public UziBonusEffect()
		{
		}

		public override void ApplyBonusEffect(NpcController npcController)
		{
			npcController.PickUzi();
		}
	}
}


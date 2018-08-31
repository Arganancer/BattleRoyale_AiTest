using System.Collections;
using System.Collections.Generic;
using Playmode.Entity.Senses;
using Playmode.Npc;
using UnityEngine;

namespace Playmode.Pickable.TypePickable
{
	public class ShotgunBonusEffect : PickableBonusEffect {

		// Use this for initialization
		public ShotgunBonusEffect()
		{
		}

		public override void ApplyBonusEffect(NpcController npcController)
		{
			npcController.PickShotgun();
		}
	}
}


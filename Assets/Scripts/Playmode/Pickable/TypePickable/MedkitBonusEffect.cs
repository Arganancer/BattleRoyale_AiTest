using System.Collections;
using System.Collections.Generic;
using Playmode.Entity.Senses;
using UnityEngine;

namespace Playmode.Pickable.TypePickable
{
	public class MedkitBonusEffect : PickableBonusEffect {

		public MedkitBonusEffect()
		{
			
		}

		public override void ApplyBonusEffect(PickableSensor pickableSensor)
		{
			pickableSensor.Heal(10);
		}
	}
}


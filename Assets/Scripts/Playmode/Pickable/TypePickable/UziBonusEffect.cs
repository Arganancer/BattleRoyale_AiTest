using System.Collections;
using System.Collections.Generic;
using Playmode.Entity.Senses;
using UnityEngine;

namespace Playmode.Pickable.TypePickable
{
	public class UziBonusEffect : PickableBonusEffect {


		public UziBonusEffect()
		{
		}

		public override void ApplyBonusEffect(PickableSensor pickableSensor)
		{
			pickableSensor.PickUzi();
		}
	}
}


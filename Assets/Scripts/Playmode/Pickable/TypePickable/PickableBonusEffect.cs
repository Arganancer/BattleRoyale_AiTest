using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playmode.Pickable.TypePickable
{
	public abstract class PickableBonusEffect
	{
		private GameObject objectToApplyTheEffect;
		public PickableBonusEffect(GameObject objectToApplyTheEffect)
		{
			this.objectToApplyTheEffect = objectToApplyTheEffect;
		}

		public abstract void ApplyBonusEffect();
	}
}


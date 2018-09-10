using System;
using Playmode.Bullet;
using UnityEngine;

namespace Playmode.Entity.Senses
{
	public class HitStimulus : MonoBehaviour
	{
		//BEN_CORRECTION : Pourquoi avez vous enlevé le "SerializeField" ?
		//
		//				   Ne jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais jamais 
		//				   jamais jamais jamais jamais jamais utiliser d'attribut public. C'est du niveau de 2e
		//				   session.
		public int HitPoints = 5;

		private void Awake()
		{
			ValidateSerializeFields();
		}

		private void ValidateSerializeFields()
		{
			if (HitPoints < 0)
				throw new ArgumentException("Hit points can't be less than 0.");
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.GetComponent<HitSensor>() != null)
			{
				other.GetComponent<HitSensor>().Hit(HitPoints);
				transform.root.GetComponentInChildren<BulletController>().Hit();
			}
		}
	}
}
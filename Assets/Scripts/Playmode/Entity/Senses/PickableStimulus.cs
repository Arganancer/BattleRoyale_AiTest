using Playmode.Npc;
using Playmode.Pickable;
using UnityEngine;

namespace Playmode.Entity.Senses
{
	public class PickableStimulus : MonoBehaviour
	{
		private const string BodyObjectName = "Body";
		private const string SightObjectName = "Sight";
		
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.transform.root?.GetComponentInChildren<NpcController>() 
			    && other.name == BodyObjectName)
			{
				other.gameObject.transform.root.GetComponentInChildren<HitSensor>()?
					.PickPickable(other.gameObject.transform.root.GetComponentInChildren<NpcController>(),
						transform.root.GetComponentInChildren<PickableController>());
				Destroy(transform.root.gameObject);
			}
			else if (other.gameObject.transform.root?.GetComponentInChildren<NpcController>() 
			         && other.name == SightObjectName)
			{
				other.gameObject.transform.root.
					GetComponentInChildren<NpcSensorSight>()?.
					SeePickable(transform.root.GetComponentInChildren<PickableController>());
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.gameObject.transform.root?.GetComponentInChildren<NpcController>() 
			    && other.name == SightObjectName)
			{
				other.gameObject.transform.root.
					GetComponentInChildren<NpcSensorSight>()?.
					LoseSightOfPickable(transform.root.GetComponentInChildren<PickableController>());
			}
		}
	}
}
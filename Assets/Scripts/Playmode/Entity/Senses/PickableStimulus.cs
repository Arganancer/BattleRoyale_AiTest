using Playmode.Npc;
using Playmode.Pickable;
using UnityEngine;

namespace Playmode.Entity.Senses
{
	public class PickableStimulus : MonoBehaviour
	{
		private void OnCollisionEnter2D(Collision2D other)
		{
			other.gameObject.GetComponent<NpcSensorSight>()?.
				PickPickable(other.gameObject.transform.root.GetComponentInChildren<NpcController>(),
				transform.root.GetComponentInChildren<PickableSensor>(),
				transform.root.GetComponentInChildren<PickableController>());
		}
		
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.name == "Body")
			{
				other.gameObject.transform.root.GetComponentInChildren<NpcSensorSight>()?
					.PickPickable(other.gameObject.transform.root.GetComponentInChildren<NpcController>(),
						transform.root.GetComponentInChildren<PickableSensor>(),
						transform.root.GetComponentInChildren<PickableController>());
				Destroy(transform.root.gameObject);
			}
		}
	}
}
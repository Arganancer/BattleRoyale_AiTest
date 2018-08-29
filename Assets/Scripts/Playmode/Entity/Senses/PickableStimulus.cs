using Playmode.Pickable;
using UnityEngine;

namespace Playmode.Entity.Senses
{
	public class PickableStimulus : MonoBehaviour
	{
		private void OnCollisionEnter2D(Collision2D other)
		{
			other.gameObject.GetComponent<NpcSensorSight>()?.PickPickable();
		}
	}
}
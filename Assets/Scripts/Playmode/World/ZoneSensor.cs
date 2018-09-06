using Playmode.Npc;
using Playmode.Pickable;
using UnityEngine;

namespace Playmode.World
{
	public class ZoneSensor : MonoBehaviour
	{
		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.gameObject.transform.root?.GetComponentInChildren<NpcController>())
			{
				NotifyObjectLeavingZone(other.gameObject.transform.root
					.GetComponentInChildren<NpcController>());
			}
			else if (other.gameObject.transform.root?.GetComponentInChildren<PickableController>())
			{
				DestroyOutOfZonePickable(other.gameObject.transform.root
					.GetComponentInChildren<PickableController>());
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.transform.root?.GetComponentInChildren<NpcController>())
			{
				NotifyObjectEnteringZone(other.gameObject.transform.root
					.GetComponentInChildren<NpcController>());
			}
		}

		private static void NotifyObjectLeavingZone(NpcController npcController)
		{
			npcController.UpdateNpcStateExitZone();
		}

		private static void NotifyObjectEnteringZone(NpcController npcController)
		{
			npcController.UpdateNpcStateEnterZone();
		}

		private static void DestroyOutOfZonePickable(PickableController pickableController)
		{
			pickableController.DestroyPickable();
		}
	}
}

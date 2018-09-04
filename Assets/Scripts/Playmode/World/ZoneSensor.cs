using System.Collections;
using System.Collections.Generic;
using Playmode.Npc;
using Playmode.Pickable;
using UnityEngine;

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

	private void NotifyObjectLeavingZone(NpcController npcController)
	{
		npcController.UpdateNpcStateExitZone();
	}

	private void NotifyObjectEnteringZone(NpcController npcController)
	{
		npcController.UpdateNpcStateEnterZone();
	}

	private void DestroyOutOfZonePickable(PickableController pickableController)
	{
		pickableController.DestroyPickable();
	}
}

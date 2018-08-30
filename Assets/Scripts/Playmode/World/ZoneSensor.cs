using System.Collections;
using System.Collections.Generic;
using Playmode.Npc;
using UnityEngine;

public delegate void ZoneSensorExitEventHandler(GameObject gameObject);

public delegate void ZoneSensorEnterEventHandler(GameObject gameObject);
public class ZoneSensor : MonoBehaviour
{
	public event ZoneSensorExitEventHandler onZoneExitSensor;
	public event ZoneSensorEnterEventHandler onZoneEnterSensor;
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject?.GetComponentInChildren<NpcController>())
		{
			NotifyObjectLeavingZone(other.gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject?.GetComponentInChildren<NpcController>())
		{
			NotifyObjectEnteringZone(other.gameObject);
		}
	}

	private void NotifyObjectLeavingZone(GameObject gameObject)
	{
		if (onZoneExitSensor != null)
		{
			onZoneExitSensor(gameObject);
		}
	}

	private void NotifyObjectEnteringZone(GameObject gameObject)
	{
		if (onZoneEnterSensor != null)
		{
			onZoneEnterSensor(gameObject);
		}
	}
}

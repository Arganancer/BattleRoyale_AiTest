using System.Collections.Generic;
using Playmode.Npc;
using Playmode.Pickable;
using UnityEngine;

namespace Playmode.Entity.Senses
{
	public delegate void NpcSensorEventHandler(NpcController npc);

	public delegate void PickableNcpSensorEventHandler(PickableController pickableController);
	public class NpcSensor : MonoBehaviour
	{
		private ICollection<NpcController> npcsInSight;

		public event NpcSensorEventHandler OnNpcSeen;
		public event NpcSensorEventHandler OnNpcSightLost;
		public event PickableNcpSensorEventHandler pickablePickedEventHandler;

		public IEnumerable<NpcController> NpcsInSight => npcsInSight;

		private void Awake()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			npcsInSight = new HashSet<NpcController>();
		}

		public void See(NpcController npc)
		{
			npcsInSight.Add(npc);

			NotifyNpcSeen(npc);
		}

		public void LooseSightOf(NpcController npc)
		{
			npcsInSight.Remove(npc);

			NotifyNpcSightLost(npc);
		}

		private void NotifyNpcSeen(NpcController npc)
		{
			if (OnNpcSeen != null) OnNpcSeen(npc);
		}

		private void NotifyNpcSightLost(NpcController npc)
		{
			if (OnNpcSightLost != null) OnNpcSightLost(npc);
		}

		public void RemoveNullNpc()
		{
			ICollection<NpcController> npcsInSightToRemove = new List<NpcController>();
			foreach (var npc in npcsInSight)
			{
				if (npc == null)
				{
					npcsInSightToRemove.Add(npc);
				}
			}

			foreach (var npc in npcsInSightToRemove)
			{
				npcsInSight.Remove(npc);
			}
		}

		public void pickPickable(PickableController pickableController)
		{
			if (pickablePickedEventHandler != null)
			{
				pickablePickedEventHandler(pickableController);
			}
		}
	}
}
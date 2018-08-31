using System.Collections.Generic;
using Playmode.Npc;
using Playmode.Pickable;
using UnityEngine;

namespace Playmode.Entity.Senses
{
	public delegate void NpcSensorEventHandler(NpcController npc);
	public delegate void PickableViewSensorEventHandler(PickableController pickable);

	public class NpcSensorSight : MonoBehaviour
	{
		private ICollection<NpcController> npcsInSight;
		private ICollection<PickableController> pickablesInSight;

		public event NpcSensorEventHandler OnNpcSeen;
		public event NpcSensorEventHandler OnNpcSightLost;
		public event PickableViewSensorEventHandler OnPickableSeen;
		public event PickableViewSensorEventHandler OnPickableSightLost;

		public IEnumerable<NpcController> NpcsInSight => npcsInSight;
		public IEnumerable<PickableController> PickablesInSight => pickablesInSight;

		private void Awake()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			npcsInSight = new HashSet<NpcController>();
			pickablesInSight = new HashSet<PickableController>();
		}

		public void SeeNpc(NpcController npc)
		{
			npcsInSight.Add(npc);

			NotifyNpcSeen(npc);
		}

		public void LoseSightOfNpc(NpcController npc)
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

		public void SeePickable(PickableController pickable)
		{
			pickablesInSight.Add(pickable);
			NotifyPickableSeen(pickable);
		}

		public void LoseSightOfPickable(PickableController pickable)
		{
			pickablesInSight.Remove(pickable);
			NotifyPickableSightLost(pickable);
		}

		private void NotifyPickableSeen(PickableController pickable)
		{
			if (OnPickableSeen != null) OnPickableSeen(pickable);
		}

		private void NotifyPickableSightLost(PickableController pickable)
		{
			if (OnNpcSightLost != null) OnPickableSightLost(pickable);
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
	}
}
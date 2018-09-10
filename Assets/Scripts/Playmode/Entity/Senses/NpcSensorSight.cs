using System.Collections.Generic;
using Playmode.Npc;
using Playmode.Pickable;
using Playmode.Pickable.TypePickable;
using UnityEngine;

namespace Playmode.Entity.Senses
{
	public delegate void NpcSensorEventHandler(NpcController npc);
	public delegate void PickableViewSensorEventHandler(PickableController pickable);

	public class NpcSensorSight : MonoBehaviour
	{
		public event NpcSensorEventHandler OnNpcSeen;
		public event NpcSensorEventHandler OnNpcSightLost;
		public event PickableViewSensorEventHandler OnPickableSeen;
		public event PickableViewSensorEventHandler OnPickableSightLost;
		
		public IEnumerable<NpcController> NpcsInSight => npcsInSight;
		public IEnumerable<PickableController> PickablesInSight => pickablesInSight;
		
		private HashSet<NpcController> npcsInSight;
		private HashSet<PickableController> pickablesInSight;

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
			if (OnPickableSightLost != null) OnPickableSightLost(pickable);
		}
		
		public void RemoveNullNpc()
		{
			npcsInSight.RemoveWhere(it => it == null);
		}

		public void RemoveNullPickable()
		{
			pickablesInSight.RemoveWhere(it => it == null);
		}
		
		public NpcController GetClosestNpc()
		{
			//BEN_CORRECTION : Il y a du code qui aurait pu être réutilisé ici (exemple, dans GetClosestPickable).

			NpcController closestNpc = null;
			var distance = float.MaxValue;
						
			foreach (var npc in npcsInSight)
			{
				if (closestNpc == null)
				{
					closestNpc = npc;
					distance = Vector3.Distance(closestNpc.transform.position,
						transform.root.position);
				}
				else
				{
					var currentNpcDistance =
						Vector3.Distance(closestNpc.transform.position, npc.transform.position);
					if (distance > currentNpcDistance)
					{
						distance = currentNpcDistance;
						closestNpc = npc;
					}
				}
			}
			
			return closestNpc;
		}
		
		public PickableController GetClosestPickable(TypePickable typePickable)
		{
			PickableController closestPickable = null;
			var distance = float.MaxValue;
			
			foreach (var pickable in pickablesInSight)
			{
				if (pickable.GetPickableType() != typePickable) continue;
				
				if (closestPickable == null)
				{
					closestPickable = pickable;
					distance = Vector3.Distance(closestPickable.transform.position,
						transform.root.position);
				}
				else
				{
					var currentPickableDistance =
						Vector3.Distance(closestPickable.transform.position, pickable.transform.position);
					
					if (distance > currentPickableDistance)
					{
						distance = currentPickableDistance;
						closestPickable = pickable;
					}
				}
			}

			return closestPickable;
		}
	}
}
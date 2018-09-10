using Playmode.Npc;
using Playmode.Pickable;
using UnityEngine;

namespace Playmode.Entity.Senses
{
	public delegate void MedkitPickedEventHandler(int healthPoint);
	public delegate void ShotgunPickedEventHandler();
	public delegate void UziPickedEventHandler();
	public delegate void HitSensorEventHandler(int hitPoints);
	public delegate void PickableHitSensorEventHandler(NpcController npcController);

	public class HitSensor : MonoBehaviour
	{
		public event PickableHitSensorEventHandler PickablePickedEventHandler; //BEN_CORRECTION : Mal nommé. Que représente cet événement ?
		public event MedkitPickedEventHandler onMedkitPick; //BEN_CORRECTION : Devraient débuter par une majuscule.
		public event ShotgunPickedEventHandler onShotgunPick;
		public event UziPickedEventHandler onUziPick; //BEN_CORRECTION : Code mort. C'est jamais utilisé ça...
		public event HitSensorEventHandler OnHit;

		public void Hit(int hitPoints)
		{
			NotifyHit(hitPoints);
		}

		private void NotifyHit(int hitPoints)
		{
			if (OnHit != null) OnHit(hitPoints);
		}

		public void PickPickable(NpcController npcController,PickableController pickableController)
		{
			if (PickablePickedEventHandler != null)
			{
				PickablePickedEventHandler(npcController);
			}
			else
			{
				SetPickEventAction(pickableController);
				PickablePickedEventHandler?.Invoke(npcController);
				RemovePickEventAction(pickableController);
			}
		}
		
		private void SetPickEventAction(PickableController pickableController)
		{
			PickablePickedEventHandler += pickableController.OnPickablePicked;
		}

		private void RemovePickEventAction(PickableController pickableController)
		{
			PickablePickedEventHandler -= pickableController.OnPickablePicked;
		}
	}
}
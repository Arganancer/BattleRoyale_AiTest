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
		public event PickableHitSensorEventHandler PickablePickedEventHandler;
		public event MedkitPickedEventHandler onMedkitPick;
		public event ShotgunPickedEventHandler onShotgunPick;
		public event UziPickedEventHandler onUziPick;
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
				SetPickEventAction(npcController,pickableController);
				PickablePickedEventHandler(npcController);
			}
		}
		
		public void SetPickEventAction(NpcController npcController,
			PickableController pickableController)
		{
			PickablePickedEventHandler += pickableController.OnPickablePicked;
		}
	}
}
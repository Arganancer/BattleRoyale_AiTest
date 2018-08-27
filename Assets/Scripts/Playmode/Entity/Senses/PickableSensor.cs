using Playmode.Pickable;
using UnityEngine;

namespace Playmode.Entity.Senses
{
	public delegate void MidkitPickedEventHandler(int healthPoint);

	public delegate void ShotgunPickedEventHandler();

	public delegate void UziPickedEventHandler();
	
	public class PickableSensor : MonoBehaviour
	{

		public event MidkitPickedEventHandler onMedkitPick;
		public event ShotgunPickedEventHandler onShotgunPick;
		public event UziPickedEventHandler onUziPick;

		public void Heal(int healPoint)
		{
			NotifyHealing(healPoint);
		}

		private void NotifyHealing(int healPoint)
		{
			
			if (onMedkitPick != null)
			{
				onMedkitPick(healPoint);
			}
		}

		public void PickShotgun()
		{
			NotifyPickingShotgun();
		}

		private void NotifyPickingShotgun()
		{
			
			if (onShotgunPick != null)
			{
				onShotgunPick();
			}
		}

		public void PickUzi()
		{
			NotifyPickingUzi();
		}

		private void NotifyPickingUzi()
		{
			
			if (onUziPick != null)
			{
				onUziPick();
			}
		}
	}
}
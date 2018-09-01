using Playmode.Pickable.TypePickable;

namespace Playmode.Event
{
	public delegate void OnWeaponShotEventHandler(TypePickable weaponType);
	
	public class ShootEventChannel : EventChannel
	{
		public event OnWeaponShotEventHandler OnWeaponShot;
		
		public void PublishWeaponShot(TypePickable weaponType)
		{
			OnWeaponShot?.Invoke(weaponType);
		}
	}
}
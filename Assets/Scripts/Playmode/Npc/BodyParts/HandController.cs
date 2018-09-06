using Playmode.Pickable.TypePickable;
using Playmode.Weapon;
using UnityEngine;

namespace Playmode.Npc.BodyParts
{
	public delegate void WeaponFiredEventHandler(Vector3 positionWhenFired);
	
	public class HandController : MonoBehaviour
	{
		public event WeaponFiredEventHandler OnWeaponFired;
		
		private WeaponController weapon;

		private void Awake()
		{
			InitializeComponent();
		}

		private static void InitializeComponent()
		{
		}

		public void Hold(GameObject heldGameObject)
		{
			if (weapon != null)
			{
				DropWeapon();
			}
			
			if (heldGameObject != null)
			{
				heldGameObject.transform.parent = transform;
				heldGameObject.transform.localPosition = Vector3.zero;
				heldGameObject.transform.rotation = new Quaternion();
				weapon = heldGameObject.GetComponentInChildren<WeaponController>();
			}
			else
			{
				weapon = null;
			}
		}

		public void Use()
		{
			if (OnWeaponFired != null) OnWeaponFired(GetWeaponPosition());
			if (weapon != null)
			{
				weapon.Shoot();
			}
		}

		public float GetProjectileSpeed()
		{
			return weapon.GetComponentInChildren<WeaponController>().GetBulletSpeed();
		}

		public Vector3 GetWeaponPosition()
		{
			return weapon.GetComponentInChildren<WeaponController>().transform.position;
		}

		private void DropWeapon()
		{
			Destroy(weapon.transform.parent.gameObject);
		}

		public void AdjustWeaponSpeed()
		{
			weapon.GetComponentInChildren<WeaponController>().FireDelayInSeconds = 0.04f;
			weapon.WeaponType = TypePickable.Uzi;
		}

		public void AdjustWeaponNbOfBullet()
		{
			weapon.GetComponentInChildren<WeaponController>().NbOfShotgunBullets = 5;
			weapon.WeaponType = TypePickable.Shotgun;
		}
	}
}
using Playmode.Entity.Movement;
using Playmode.Weapon;
using UnityEngine;

namespace Playmode.Npc.BodyParts
{
	public delegate void WeaponFiredEventHandler(Vector3 positionWhenFired);
	
	public class HandController : MonoBehaviour
	{
		public event WeaponFiredEventHandler OnWeaponFired;
		private AnchoredMover mover;
		private WeaponController weapon;

		private void Awake()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			mover = GetComponent<AnchoredMover>();
		}

		public void Hold(GameObject heldGameObject)
		{
			if (heldGameObject != null)
			{
				heldGameObject.transform.parent = transform;
				heldGameObject.transform.localPosition = Vector3.zero;

				weapon = heldGameObject.GetComponentInChildren<WeaponController>();
			}
			else
			{
				weapon = null;
			}
		}

		public float AimTowardsPoint(Vector3 point)
		{
			return Vector3.Dot(point - mover.transform.root.position, transform.right);
		}

		public static float AimTowardsDirection(Mover mainMover, Vector3 movementDirection)
		{
			return Vector3.Dot(movementDirection, mainMover.transform.right);
		}

		public void Use()
		{
			// TODO: Remove this line
			// Debug.Log("Time Fired: " + Time.time + "\nHandController position: " + mover.transform.root.position);
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
	}
}
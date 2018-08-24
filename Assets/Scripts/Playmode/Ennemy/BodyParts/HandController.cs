using System;
using Playmode.Entity.Movement;
using Playmode.Weapon;
using UnityEngine;

namespace Playmode.Ennemy.BodyParts
{
	public class HandController : MonoBehaviour
	{
		private Mover mover;
		private WeaponController weapon;

		private void Awake()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			mover = GetComponent<AnchoredMover>();
		}

		public void Hold(GameObject gameObject)
		{
			if (gameObject != null)
			{
				gameObject.transform.parent = transform;
				gameObject.transform.localPosition = Vector3.zero;

				weapon = gameObject.GetComponentInChildren<WeaponController>();
			}
			else
			{
				weapon = null;
			}
		}

		public float AimTowardsPoint(Vector3 point)
		{
			return Vector3.Dot(point - mover.transform.parent.position, transform.right);
		}

		public float AimTowardsDirection(Mover mainMover, Vector3 movementDirection)
		{
			return Vector3.Dot(movementDirection, mainMover.transform.right);
		}

		public void Use()
		{
			if (weapon != null) weapon.Shoot();
		}
	}
}
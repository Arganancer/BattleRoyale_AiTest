using Playmode.Entity.Movement;
using Playmode.Weapon;
using UnityEngine;

namespace Playmode.Npc.BodyParts
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
			if (weapon != null) weapon.Shoot();
		}
	}
}
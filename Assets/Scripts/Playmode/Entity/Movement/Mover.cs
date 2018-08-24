using System;
using UnityEngine;

namespace Playmode.Entity.Movement
{
	public abstract class Mover : MonoBehaviour
	{
		public static readonly Vector3 Foward = Vector3.up;
		public const float Clockwise = 1f;

		[SerializeField] protected float Speed = 2f;
		[SerializeField] protected float RotateSpeed = 90f;

		protected void Awake()
		{
			ValidateSerialisedFields();
		}

		private void ValidateSerialisedFields()
		{
			if (Speed < 0)
				throw new ArgumentException("Speed can't be lower than 0.");
			if (RotateSpeed < 0)
				throw new ArgumentException("RotateSpeed can't be lower than 0.");
		}

		public abstract void MoveRelativeToSelf(Vector3 direction);

		public abstract void MoveRelativeToWorld(Vector3 direction);

		public abstract void Rotate(float direction);
	}
}
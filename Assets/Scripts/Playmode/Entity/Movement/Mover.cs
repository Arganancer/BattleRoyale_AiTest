using System;
using UnityEngine;

namespace Playmode.Entity.Movement
{
	public class Mover : MonoBehaviour
	{
		private Transform rootTransform;
		public static readonly Vector3 Forward = Vector3.up;
		public const float Clockwise = 1f;

		[SerializeField] protected float Speed = 2f;
		[SerializeField] protected float RotateSpeed = 90f;
		protected Vector3 PositionLastFrame;
		protected Vector3 PositionThisFrame;

		protected void Awake()
		{
			ValidateSerialisedFields();
			InitializeComponent();
		}

		private void ValidateSerialisedFields()
		{
			if (Speed < 0)
				throw new ArgumentException("Speed can't be lower than 0.");
			if (RotateSpeed < 0)
				throw new ArgumentException("RotateSpeed can't be lower than 0.");
		}

		private void InitializeComponent()
		{
			rootTransform = transform.root;
		}

		public void MoveRelativeToSelf(Vector3 direction)
		{
			rootTransform.Translate(direction.normalized * Speed * Time.deltaTime, Space.Self);
		}

		public void MoveRelativeToWorld(Vector3 direction)
		{
			rootTransform.Translate(direction.normalized * Speed * Time.deltaTime, Space.World);
		}

		public void Rotate(float direction)
		{
			rootTransform.Rotate(
				Vector3.forward,
				(direction < 0 ? RotateSpeed : -RotateSpeed) * Time.deltaTime
			);
		}

		public void RotateTowards(Vector3 target)
		{
			var desiredOrientation = Quaternion.LookRotation(Vector3.forward, target);
			rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, desiredOrientation, RotateSpeed * Time.deltaTime);
		}

		public float GetSpeed()
		{
			return Speed;
		}

		public void UpdatePosition()
		{
			PositionLastFrame = PositionThisFrame;
			PositionThisFrame = rootTransform.position;
		}

		public Vector3 GetVelocity()
		{
			if(PositionLastFrame == PositionThisFrame)
				return new Vector3(0, 0, 0);
			var directionalVector = (PositionThisFrame - PositionLastFrame).normalized;
			var velocityVector = directionalVector * Speed;
			return velocityVector;
		}
	}
}
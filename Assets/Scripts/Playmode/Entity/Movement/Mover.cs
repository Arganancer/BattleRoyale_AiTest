using System;
using Playmode.Npc;
using Playmode.Pickable;
using UnityEngine;
using Random = UnityEngine.Random;

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

		private void RotateTowards(Vector3 target)
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
		
		public void MoveTowardsPosition(Vector3 position)
		{
			MoveRelativeToWorld(position - transform.root.position);
		}

		public void MoveTowardsDirection(Vector3 direction)
		{
			MoveRelativeToWorld(direction);
		}

		public void MoveAwayFromPosition(Vector3 position)
		{
			MoveRelativeToWorld(transform.parent.position - position);
		}

		public void MoveRightAroundPosition(Vector3 position)
		{
			var directionTowardsPosition = position - transform.parent.position;
			var perpendicularDirection =
				new Vector3(directionTowardsPosition.y, -directionTowardsPosition.x, directionTowardsPosition.z);
			MoveRelativeToWorld(perpendicularDirection);
		}

		public void MoveLeftAroundPosition(Vector3 position)
		{
			var directionTowardsPosition = position - transform.parent.position;
			var perpendicularDirection =
				new Vector3(-directionTowardsPosition.y, directionTowardsPosition.x, directionTowardsPosition.z);
			MoveRelativeToWorld(perpendicularDirection);
		}

		public static Vector3 GetRandomDirection()
		{
			return Random.insideUnitCircle;
		}

		public void RotateTowardsAngle(float angle)
		{
			Rotate(angle);
		}

		public void RotateTowardsDirection(Vector3 direction)
		{
			RotateTowards(direction);
		}

		public void RotateTowardsPosition(Vector3 position)
		{
			RotateTowards(position - transform.root.position);
		}
	}
}
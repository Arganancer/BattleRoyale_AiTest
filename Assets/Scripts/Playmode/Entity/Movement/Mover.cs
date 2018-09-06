using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playmode.Entity.Movement
{
	public class Mover : MonoBehaviour
	{
		[SerializeField] protected float Speed = 2f;
		[SerializeField] protected float RotateSpeed = 90f;
		
		private Vector3 positionLastFrame;
		private Vector3 positionThisFrame;
		private Transform rootTransform;

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

		private void MoveRelativeToWorld(Vector3 direction)
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

		public void UpdatePosition()
		{
			positionLastFrame = positionThisFrame;
			positionThisFrame = rootTransform.position;
		}

		public Vector3 GetVelocity()
		{
			if(positionLastFrame == positionThisFrame)
				return new Vector3(0, 0, 0);
			
			var directionalVector = (positionThisFrame - positionLastFrame).normalized;
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
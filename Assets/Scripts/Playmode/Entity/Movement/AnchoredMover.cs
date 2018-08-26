using UnityEngine;

namespace Playmode.Entity.Movement
{
	public class AnchoredMover : Mover
	{
		private Transform rootTransform;

		private new void Awake()
		{
			base.Awake();

			InitializeComponent();
		}

		private void InitializeComponent()
		{
			rootTransform = transform.root;
		}

		public override void MoveRelativeToSelf(Vector3 direction)
		{
			transform.Translate(direction.normalized * Speed * Time.deltaTime);
		}

		public override void MoveRelativeToWorld(Vector3 direction)
		{
			rootTransform.Translate(direction.normalized * Speed * Time.deltaTime, Space.World);
		}

		public override void Rotate(float direction)
		{
			transform.RotateAround(
				rootTransform.position,
				Vector3.forward,
				(direction < 0 ? RotateSpeed : -RotateSpeed) * Time.deltaTime
			);
		}

		public override void RotateTowards(Vector3 target)
		{
			var desiredOrientation = Quaternion.LookRotation(target);
			Quaternion.RotateTowards(rootTransform.rotation, desiredOrientation, RotateSpeed * Time.deltaTime);
		}

		public override void UpdatePosition()
		{
			PositionLastFrame = rootTransform.position;
		}

		public override Vector3 GetVelocity()
		{
			if(PositionLastFrame == rootTransform.position)
				return new Vector3(0, 0, 0);
			var directionalVector = (rootTransform.position - PositionLastFrame).normalized;
			var velocityVector = directionalVector * Speed;
			return velocityVector;
		}
	}
}
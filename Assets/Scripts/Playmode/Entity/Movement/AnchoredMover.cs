using System;
using UnityEngine;

namespace Playmode.Entity.Movement
{
	public class AnchoredMover : MonoBehaviour
	{
		private Transform rootTransform;

		[SerializeField] protected float Speed = 2f;
		[SerializeField] protected float RotateSpeed = 90f;
		
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

		public void Rotate(float direction)
		{
			transform.RotateAround(
				rootTransform.position,
				Vector3.forward,
				(direction < 0 ? RotateSpeed : -RotateSpeed) * Time.deltaTime
			);
		}
	}
}
using System;
using UnityEngine;

namespace Playmode.Pickable
{
	public class PickableController : MonoBehaviour
	{
		private SpriteRenderer visualComponent;

		[SerializeField] private Sprite medicalKit;
		[SerializeField] private Sprite shotgun;
		[SerializeField] private Sprite uzi;

		private void Awake()
		{
			visualComponent = GameObject.Find("Visual").GetComponent<SpriteRenderer>();
			ValidateSerialisedFields();
		}


		private void ValidateSerialisedFields()
		{
			if (medicalKit == null)
			{
				throw new ArgumentException("Type sprites must be provided. MedicalKit is missing.");
			}

			if (shotgun == null)
			{
				throw new ArgumentException("Type sprites must be provided. Shotgun is missing.");
			}

			if (uzi == null)
			{
				throw new ArgumentException("Type sprites must be provided. Uzi is missing.");
			}
		}

		public void Configure(PickableBehaviour.PickableBehaviour pickableBehaviour)
		{
			switch (pickableBehaviour)
			{
				case PickableBehaviour.PickableBehaviour.Medicalkit:
					visualComponent.sprite = medicalKit;
					break;
				case PickableBehaviour.PickableBehaviour.Shotgun:
					visualComponent.sprite = shotgun;
					break;
				case PickableBehaviour.PickableBehaviour.Uzi:
					visualComponent.sprite = uzi;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(pickableBehaviour), pickableBehaviour, null);
			}
		}

		public void OnPickablePicked()
		{
			//call the behaviour corresponding to the type of pickable
		}
	}
}
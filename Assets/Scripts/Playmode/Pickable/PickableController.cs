using System;
using Playmode.Entity.Senses;
using Playmode.Pickable.TypePickable;
using UnityEngine;

namespace Playmode.Pickable
{
	public class PickableController : MonoBehaviour
	{
		private SpriteRenderer visualComponent;

		[SerializeField] private Sprite medicalKit;
		[SerializeField] private Sprite shotgun;
		[SerializeField] private Sprite uzi;
		private PickableSensorEventHandler pickableSensorEventHandler;
		private TypePickable.TypePickable typePickable;

		private PickableBonusEffect ActivateBonusEffect;
		private void Awake()
		{
			visualComponent = GameObject.Find("Visual").GetComponent<SpriteRenderer>();
			pickableSensorEventHandler += OnPickablePicked;
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

		public void Configure(TypePickable.TypePickable typePickable)
		{
			switch (typePickable)
			{
				case TypePickable.TypePickable.Medicalkit:
					visualComponent.sprite = medicalKit;
					break;
				case TypePickable.TypePickable.Shotgun:
					visualComponent.sprite = shotgun;
					break;
				case TypePickable.TypePickable.Uzi:
					visualComponent.sprite = uzi;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(typePickable), typePickable, null);
			}

			this.typePickable = typePickable;
		}

		public TypePickable.TypePickable GetPickableType()
		{
			return typePickable;
		}

		public void OnPickablePicked()
		{
//		}
//
//		private void AppliedRelatedPickableEffect()
//		{
//			switch (typePickable)
//			{
//			}
		}
	}
}
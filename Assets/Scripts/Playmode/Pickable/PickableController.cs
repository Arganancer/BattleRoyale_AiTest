using System;
using System.Diagnostics;
using Playmode.Entity.Senses;
using Playmode.Pickable.TypePickable;
using UnityEngine;

namespace Playmode.Pickable
{
	public class PickableController : MonoBehaviour
	{
		[SerializeField] private Sprite medicalKit;
		[SerializeField] private Sprite shotgun;
		[SerializeField] private Sprite uzi;

		private TypePickable.TypePickable typePickable;
		private NpcSensorSight pickableSensorEventHandler;
		private SpriteRenderer visualComponent;
		private PickableBonusEffect bonusEffect;
		
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

		public void Configure(TypePickable.TypePickable typePickable)
		{
			switch (typePickable)
			{
				case TypePickable.TypePickable.Medicalkit:
					bonusEffect = new MedkitBonusEffect();
					visualComponent.sprite = medicalKit;
					break;
				case TypePickable.TypePickable.Shotgun:
					bonusEffect = new ShotgunBonusEffect();
					visualComponent.sprite = shotgun;
					break;
				case TypePickable.TypePickable.Uzi:
					bonusEffect = new UziBonusEffect();
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
			if (bonusEffect != null)
			{
				bonusEffect.ApplyBonusEffect(gameObject.transform.root.GetComponentInChildren<PickableSensor>());
			}
		}
	}
}
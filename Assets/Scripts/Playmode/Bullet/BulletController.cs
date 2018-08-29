using System;
using Playmode.Entity.Destruction;
using Playmode.Entity.Movement;
using UnityEngine;

namespace Playmode.Bullet
{
	public class BulletController : MonoBehaviour
	{
		[Header("Behaviour")] [SerializeField] private float lifeSpanInSeconds = 5f;

		public float LifeSpanInSeconds
		{
			get { return lifeSpanInSeconds; }
			set { lifeSpanInSeconds = value; }
		}

		private AnchoredMover anchoredMover;
		private Destroyer destroyer;
		private float timeSinceSpawnedInSeconds;
		private float currentPercentageDuration;
		private float startDyingPercentageDuration = 0.7f;
		private float dyingPercentageRemaining = 0.3f;

		private bool IsAlive => timeSinceSpawnedInSeconds < lifeSpanInSeconds;

		private void Awake()
		{
			ValidateSerialisedFields();
			InitialzeComponent();
		}

		private void ValidateSerialisedFields()
		{
			if (lifeSpanInSeconds < 0)
				throw new ArgumentException("LifeSpan can't be lower than 0.");
		}

		private void InitialzeComponent()
		{
			anchoredMover = GetComponent<AnchoredMover>();
			destroyer = GetComponent<RootDestroyer>();

			timeSinceSpawnedInSeconds = 0;
		}

		private void Update()
		{
			UpdateLifeSpan();

			Act();
		}

		private void UpdateLifeSpan()
		{
			timeSinceSpawnedInSeconds += Time.deltaTime;
			currentPercentageDuration = timeSinceSpawnedInSeconds / lifeSpanInSeconds;
			if (currentPercentageDuration > startDyingPercentageDuration)
			{
				UpdateBulletDying();
			}
		}

		private void UpdateBulletDying()
		{
			var percentageModifier = 1f - (currentPercentageDuration - startDyingPercentageDuration) / dyingPercentageRemaining;
			transform.root.localScale = new Vector3(0.5f, 0.5f, 1) * percentageModifier;
			anchoredMover.SetCurrentSpeed(anchoredMover.GetMaxSpeed() * percentageModifier);
			
		}

		public void Hit()
		{
			timeSinceSpawnedInSeconds = lifeSpanInSeconds;
		}

		private void Act()
		{
			if (IsAlive)
			{
				anchoredMover.MoveRelativeToSelf(AnchoredMover.Forward);
			}
			else
				destroyer.Destroy();
		}
	}
}
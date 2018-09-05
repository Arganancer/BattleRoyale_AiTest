using System;
using Playmode.Entity.Destruction;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
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
			anchoredMover.SetCurrentSpeed(anchoredMover.MaxSpeed * percentageModifier);
			
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

		public void ConfigureLineShoot(GameObject bullet,int bulletDamage)
		{
			bullet.GetComponentInChildren<HitStimulus>().HitPoints = bulletDamage;
		}

		public void ConfigureConeShoot(GameObject bullet,int bulletDamage)
		{
			
			bullet.GetComponentInChildren<HitStimulus>().HitPoints = bulletDamage;
			bullet.transform.Rotate(Vector3.forward * UnityEngine.Random.Range(-4, 4), Space.Self);
			bullet.transform.GetComponentInChildren<AnchoredMover>().MaxSpeed *= UnityEngine.Random.Range(1.1f, 1.2f);
			bullet.transform.GetComponentInChildren<BulletController>().LifeSpanInSeconds = 0.5f;
		}
	}
}
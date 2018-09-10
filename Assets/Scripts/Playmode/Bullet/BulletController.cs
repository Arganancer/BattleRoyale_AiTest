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

		private const float StartDyingPercentageDuration = 0.7f;
		private const float DyingPercentageRemaining = 0.3f;
		
		private AnchoredMover anchoredMover;
		private Destroyer destroyer;
		
		private float timeSinceSpawnedInSeconds;
		private float currentPercentageDuration;

		//BEN_CORRECTION : Propriété me semble inutile. Un attribut suffirait (vous utilisez l'attribut dans 90% des
		//				   cas de toute façon).
		private float LifeSpanInSeconds
		{
			set { lifeSpanInSeconds = value; }
		}

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
			
			if (currentPercentageDuration > StartDyingPercentageDuration)
			{
				UpdateBulletDying();
			}
		}

		//BEN_REVIEW : Avec tout ça, je pense qu'une coroutine aurait été plus claire.
		//
		//			   Je comprends ce que cela fait, mais était-ce vraiment nécessaire ?
		private void UpdateBulletDying()
		{
			var percentageModifier = 1f - (currentPercentageDuration - StartDyingPercentageDuration) / DyingPercentageRemaining;
			transform.root.localScale = new Vector3(0.5f, 0.5f, 1) * percentageModifier;
			anchoredMover.SetCurrentSpeed(anchoredMover.MaxSpeed * percentageModifier);
			
		}

		//BEN_CORRECTION : Intrusion de responsabilité. BulletController aurait du s'abonner à un événement de
		//				   "HitStimulus". HitStimulus ne devrait pas connaitre "BulletController". Qui dit qu'un
		//				   "HitStimulus" ne pourrait pas être placé sur autre chose.
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

		public static void ConfigureLineShoot(GameObject bullet,int bulletDamage)
		{
			bullet.GetComponentInChildren<HitStimulus>().HitPoints = bulletDamage;
		}

		public static void ConfigureConeShoot(GameObject bullet,int bulletDamage)
		{
			
			bullet.GetComponentInChildren<HitStimulus>().HitPoints = bulletDamage;
			bullet.transform.Rotate(Vector3.forward * UnityEngine.Random.Range(-4, 4), Space.Self);
			bullet.transform.GetComponentInChildren<AnchoredMover>().MaxSpeed *= UnityEngine.Random.Range(1.1f, 1.2f);
			bullet.transform.GetComponentInChildren<BulletController>().LifeSpanInSeconds = 0.5f;
		}
	}
}
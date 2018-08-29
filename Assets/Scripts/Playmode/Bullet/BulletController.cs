using System;
using Playmode.Entity.Destruction;
using Playmode.Entity.Movement;
using UnityEngine;

namespace Playmode.Bullet
{
	public class BulletController : MonoBehaviour
	{
		[Header("Behaviour")] [SerializeField] private float lifeSpanInSeconds = 5f;

		private AnchoredMover anchoredMover;
		private Destroyer destroyer;
		private float timeSinceSpawnedInSeconds;

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
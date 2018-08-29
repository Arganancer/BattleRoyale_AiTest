using System;
using Playmode.Entity.Destruction;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies;
using UnityEngine;

namespace Playmode.Npc
{
	public class NpcController : MonoBehaviour
	{
		[Header("Body Parts")] [SerializeField]
		private GameObject body;

		[SerializeField] private GameObject hand;
		[SerializeField] private GameObject sight;
		[SerializeField] private GameObject typeSign;

		[Header("Type Images")] [SerializeField]
		private Sprite normalSprite;

		[SerializeField] private Sprite carefulSprite;
		[SerializeField] private Sprite cowboySprite;
		[SerializeField] private Sprite camperSprite;

		[Header("Behaviour")] [SerializeField] private GameObject startingWeaponPrefab;

		private Health health;
		private Mover mover;
		private Destroyer destroyer;
		private NpcSensorSight npcSensorSight;
		private NpcSensorSound npcSensorSound;
		private HitSensor hitSensor;
		private HandController handController;

		private INpcStrategy strategy;

		private void Awake()
		{
			ValidateSerialisedFields();
			InitializeComponent();
			CreateStartingWeapon();
		}

		private void ValidateSerialisedFields()
		{
			if (body == null)
				throw new ArgumentException("Body parts must be provided. Body is missing.");
			if (hand == null)
				throw new ArgumentException("Body parts must be provided. Hand is missing.");
			if (sight == null)
				throw new ArgumentException("Body parts must be provided. Sight is missing.");
			if (typeSign == null)
				throw new ArgumentException("Body parts must be provided. TypeSign is missing.");
			if (normalSprite == null)
				throw new ArgumentException("Type sprites must be provided. Normal is missing.");
			if (carefulSprite == null)
				throw new ArgumentException("Type sprites must be provided. Careful is missing.");
			if (cowboySprite == null)
				throw new ArgumentException("Type sprites must be provided. Cowboy is missing.");
			if (camperSprite == null)
				throw new ArgumentException("Type sprites must be provided. Camper is missing.");
			if (startingWeaponPrefab == null)
				throw new ArgumentException("StartingWeapon prefab must be provided.");
		}

		private void InitializeComponent()
		{
			health = GetComponent<Health>();
			mover = GetComponent<Mover>();
			destroyer = GetComponent<RootDestroyer>();

			var rootTransform = transform.root;
			npcSensorSight = rootTransform.GetComponentInChildren<NpcSensorSight>();
			npcSensorSound = rootTransform.GetComponentInChildren<NpcSensorSound>();
			hitSensor = rootTransform.GetComponentInChildren<HitSensor>();
			handController = hand.GetComponent<HandController>();
		}

		private void CreateStartingWeapon()
		{
			handController.Hold(Instantiate(
				startingWeaponPrefab,
				Vector3.zero,
				Quaternion.identity
			));
		}

		private void OnEnable()
		{
			npcSensorSight.OnNpcSeen += OnNpcSeen;
			npcSensorSight.OnNpcSightLost += OnNpcSightLost;
			hitSensor.OnHit += OnHit;
			health.OnDeath += OnDeath;
		}

		private void Update()
		{
			npcSensorSight.RemoveNullNpc();
			strategy.Act();
		}

		private void LateUpdate()
		{
			mover.UpdatePosition();
		}

		public Vector3 GetVelocity()
		{
			return mover.GetVelocity();
		}

		private void OnDisable()
		{
			npcSensorSight.OnNpcSeen -= OnNpcSeen;
			npcSensorSight.OnNpcSightLost -= OnNpcSightLost;
			hitSensor.OnHit -= OnHit;
			health.OnDeath -= OnDeath;
		}

		public void Configure(NpcStrategy strategy, Color color)
		{
//			body.GetComponent<SpriteRenderer>().color = color;
//			sight.GetComponent<SpriteRenderer>().color = color;
			switch (strategy)
			{
				case NpcStrategy.Cowboy:
//					body.GetComponent<SpriteRenderer>().color = Color.cyan;
//					sight.GetComponent<SpriteRenderer>().color = Color.cyan;
//					typeSign.GetComponent<SpriteRenderer>().sprite = cowboySprite;
//					this.strategy = new CowboyBehavior(mover, handController, hitSensor, health, npcSensorSight, npcSensorSound);
//					break;
				case NpcStrategy.Careful:
//					body.GetComponent<SpriteRenderer>().color = Color.white;
//					sight.GetComponent<SpriteRenderer>().color = Color.white;
//					typeSign.GetComponent<SpriteRenderer>().sprite = carefulSprite;
//					this.strategy = new CarefulBehavior(mover, handController, hitSensor, health, npcSensorSight, npcSensorSound);
//					break;
				case NpcStrategy.Camper:
//					typeSign.GetComponent<SpriteRenderer>().sprite = camperSprite;
//					break;
				case NpcStrategy.Normal:
					body.GetComponent<SpriteRenderer>().color = Color.red;
					sight.GetComponent<SpriteRenderer>().color = Color.red;
					typeSign.GetComponent<SpriteRenderer>().sprite = normalSprite;
					this.strategy = new TestStrategy(mover, handController, hitSensor, health, npcSensorSight, npcSensorSound);
					break;
				default:
					typeSign.GetComponent<SpriteRenderer>().sprite = normalSprite;
					break;
			}
		}

		private void OnHit(int hitPoints)
		{
			health.Hit(hitPoints);
		}

		private void OnDeath()
		{
			destroyer.Destroy();
		}

		public void OnHeal(int healPoint)
		{
			health.Heal(healPoint);
		}

		public void OnPickShotgun()
		{
			
		}

		public void OnPickUzi()
		{
			
		}
		private void OnNpcSeen(NpcController npc)
		{
		}

		private void OnNpcSightLost(NpcController npc)
		{
		}
	}
}
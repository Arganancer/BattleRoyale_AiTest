using System;
using Playmode.Entity.Destruction;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Event;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies;
using Playmode.Npc.Strategies.BaseStrategyClasses;
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
		[SerializeField] private GameObject uziWeapon;
		[SerializeField] private GameObject shotgunWeapon;

		private Health health;
		private Mover mover;
		private Destroyer destroyer;
		private NpcSensorSight npcSensorSight;
		private NpcSensorSound npcSensorSound;
		private HitSensor hitSensor;
		private HandController handController;

		private INpcStrategy strategy;

		private NpcDeathEventChannel npcDeathEventChannel;
		private HitEventChannel hitEventChannel;

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
			if (uziWeapon == null)
				throw new ArgumentException("UziWeapon prefab must be provided.");
			if (shotgunWeapon == null)
				throw new ArgumentException("ShotgunWeapon prefab must be provided.");
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

			npcDeathEventChannel = GameObject.FindWithTag("GameController").GetComponent<NpcDeathEventChannel>();
			hitEventChannel = GameObject.FindWithTag("GameController").GetComponent<HitEventChannel>();
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
			hitSensor.onUziPick += OnPickUzi;
			hitSensor.onMedkitPick += OnPickMedKit;
			hitSensor.onShotgunPick += OnPickShotgun;
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
			switch (strategy)
			{
				case NpcStrategy.Cowboy:
					body.GetComponent<SpriteRenderer>().color = Color.cyan;
					sight.GetComponent<SpriteRenderer>().color = Color.cyan;
					typeSign.GetComponent<SpriteRenderer>().sprite = cowboySprite;
					this.strategy = new CowboyBehavior(mover, handController, hitSensor, health, npcSensorSight, npcSensorSound);
					break;
				case NpcStrategy.Careful:
					body.GetComponent<SpriteRenderer>().color = Color.white;
					sight.GetComponent<SpriteRenderer>().color = Color.white;
					typeSign.GetComponent<SpriteRenderer>().sprite = carefulSprite;
					this.strategy = new CarefulBehavior(mover, handController, hitSensor, health, npcSensorSight, npcSensorSound);
					break;
				case NpcStrategy.Camper:
					body.GetComponent<SpriteRenderer>().color = Color.yellow;
					sight.GetComponent<SpriteRenderer>().color = Color.yellow;
					typeSign.GetComponent<SpriteRenderer>().sprite = carefulSprite;
					this.strategy = new CamperBehavior(mover, handController, hitSensor, health, npcSensorSight, npcSensorSound);
					break;
				case NpcStrategy.Normal:
					body.GetComponent<SpriteRenderer>().color = Color.red;
					sight.GetComponent<SpriteRenderer>().color = Color.red;
					typeSign.GetComponent<SpriteRenderer>().sprite = normalSprite;
					this.strategy = new NormalBehavior(mover, handController, hitSensor, health, npcSensorSight, npcSensorSound);
					break;
				default:
					body.GetComponent<SpriteRenderer>().color = Color.blue;
					sight.GetComponent<SpriteRenderer>().color = Color.blue;
					typeSign.GetComponent<SpriteRenderer>().sprite = normalSprite;
					this.strategy = new NormalBehavior(mover, handController, hitSensor, health, npcSensorSight, npcSensorSound);
					break;
			}
		}

		private void OnHit(int hitPoints)
		{
			NotifyHit();
			
			health.Hit(hitPoints);
		}

		private void OnDeath()
		{
			NotifyDeath();
			
			destroyer.Destroy();
		}

		public void OnPickMedKit(int healPoint)
		{
			health.Heal(healPoint);
		}

		public void OnPickShotgun()
		{
			SwitchWeapon(shotgunWeapon);
			handController.AdjustWeaponNbOfBullet();
		}

		public void OnPickUzi()
		{
			SwitchWeapon(uziWeapon);
			handController.AdjustWeaponSpeed();
		}
		private void OnNpcSeen(NpcController npc)
		{
		}

		private void OnNpcSightLost(NpcController npc)
		{
		}

		private void SwitchWeapon(GameObject weaponObject)
		{
			handController.DropWeapon();
			handController.Hold(Instantiate(
				weaponObject,
				Vector3.zero,
				Quaternion.identity
			));
		}

		private void NotifyDeath()
		{
			npcDeathEventChannel.Publish();
		}

		private void NotifyHit()
		{
			hitEventChannel.Publish();
		}

		public int GetHealth()
		{
			return health.HealthPoints;
		}
		
		public void Heal(int healPoint)
		{
			OnPickMedKit(healPoint);
		}

		public void PickShotgun()
		{
			OnPickShotgun();
		}

		public void PickUzi()
		{
			OnPickUzi();
		}
	}
}
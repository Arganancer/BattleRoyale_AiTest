using System;
using Playmode.Entity.Destruction;
using Playmode.Entity.Movement;
using Playmode.Entity.Senses;
using Playmode.Entity.Status;
using Playmode.Event;
using Playmode.Npc.BodyParts;
using Playmode.Npc.Strategies;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Npc
{
	public class NpcController : MonoBehaviour
	{
		[Header("Body Parts")] 
		[SerializeField]private GameObject body;
		[SerializeField] private GameObject hand;
		[SerializeField] private GameObject sight;
		[SerializeField] private GameObject typeSign;

		[Header("Type Images")] 
		[SerializeField]private Sprite normalSprite;
		[SerializeField] private Sprite carefulSprite;
		[SerializeField] private Sprite cowboySprite;
		[SerializeField] private Sprite camperSprite;

		[Header("Behaviour")] 
		[SerializeField] private GameObject startingWeaponPrefab;
		[SerializeField] private GameObject uziWeapon;
		[SerializeField] private GameObject shotgunWeapon;

		private readonly Color[] colors = {
			new Color32(255, 142, 24, 255),
			new Color32(37, 255, 35, 255), 
			Color.white,
			new Color32(16, 193, 232, 255),
			new Color32(224, 42, 11, 255)
		};

		private Health health;
		private Mover mover;
		private Destroyer destroyer;
		private NpcSensorSight npcSensorSight;
		private NpcSensorSound npcSensorSound;
		private HitSensor hitSensor;
		private HandController handController;

		private BaseNpcBehavior strategy;
		private string strategyName;

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

			npcDeathEventChannel = GameObject.FindWithTag(Tags.GameController).GetComponent<NpcDeathEventChannel>();
			hitEventChannel = GameObject.FindWithTag(Tags.GameController).GetComponent<HitEventChannel>();
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
			hitSensor.OnHit += OnHit;
			health.OnDeath += OnDeath;
			hitSensor.onUziPick += OnPickUzi;
			hitSensor.onMedkitPick += OnPickMedKit;
			hitSensor.onShotgunPick += OnPickShotgun;
		}

		private void Update()
		{
			npcSensorSight.RemoveNullNpc();
			npcSensorSight.RemoveNullPickable();
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
			hitSensor.OnHit -= OnHit;
			health.OnDeath -= OnDeath;
		}

		public void Configure(NpcStrategy strategy)
		{
			strategyName = Enum.GetName(typeof(NpcStrategy), strategy);
			switch (strategy)
			{
				case NpcStrategy.Cowboy:
					body.GetComponent<SpriteRenderer>().color = colors[2];
					sight.GetComponent<SpriteRenderer>().color = colors[2];
					typeSign.GetComponent<SpriteRenderer>().sprite = cowboySprite;
					this.strategy = new CowboyBehavior(mover, handController, health, npcSensorSight,
						npcSensorSound);
					break;
				case NpcStrategy.Careful:
					body.GetComponent<SpriteRenderer>().color = colors[0];
					sight.GetComponent<SpriteRenderer>().color = colors[0];
					typeSign.GetComponent<SpriteRenderer>().sprite = carefulSprite;
					this.strategy = new CarefulBehavior(mover, handController, health, npcSensorSight,
						npcSensorSound);
					break;
				case NpcStrategy.Camper:
					body.GetComponent<SpriteRenderer>().color = colors[1];
					sight.GetComponent<SpriteRenderer>().color = colors[1];
					typeSign.GetComponent<SpriteRenderer>().sprite = camperSprite;
					this.strategy = new CamperBehavior(mover, handController, health, npcSensorSight,
						npcSensorSound);
					break;
				case NpcStrategy.Normal:
					body.GetComponent<SpriteRenderer>().color = colors[3];
					sight.GetComponent<SpriteRenderer>().color = colors[3];
					typeSign.GetComponent<SpriteRenderer>().sprite = normalSprite;
					this.strategy = new NormalBehavior(mover, handController, health, npcSensorSight, npcSensorSound);
					break;
				case NpcStrategy.Op:
					body.GetComponent<SpriteRenderer>().color = colors[4];
					sight.GetComponent<SpriteRenderer>().color = colors[4];
					typeSign.GetComponent<SpriteRenderer>().sprite = normalSprite;
					this.strategy = new OpStrategy(mover, handController, health, npcSensorSight,
						npcSensorSound);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
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

		private void OnPickMedKit(int healPoint)
		{
			health.Heal(healPoint);
		}

		private void OnPickShotgun()
		{
			SwitchWeapon(shotgunWeapon);
			handController.AdjustWeaponNbOfBullet();
		}

		private void OnPickUzi()
		{
			SwitchWeapon(uziWeapon);
			handController.AdjustWeaponSpeed();
		}

		private void SwitchWeapon(GameObject weaponObject)
		{
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

		public string GetStateString()
		{
			return Enum.GetName(typeof(State), strategy.GetState());
		}

		public string GetStrategyString()
		{
			return strategyName;
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

		public void UpdateNpcStateEnterZone()
		{
			strategy.SetIsOutsideOfZone = false;
		}

		public void UpdateNpcStateExitZone()
		{
			strategy.SetIsOutsideOfZone = true;
		}
	}
}
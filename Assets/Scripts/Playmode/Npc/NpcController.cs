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
		private NpcSensor npcSensor;
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
			mover = GetComponent<RootMover>();
			destroyer = GetComponent<RootDestroyer>();

			var rootTransform = transform.root;
			npcSensor = rootTransform.GetComponentInChildren<NpcSensor>();
			hitSensor = rootTransform.GetComponentInChildren<HitSensor>();
			handController = hand.GetComponent<HandController>();

			strategy = new TestStrategy(mover, handController, hitSensor, health, npcSensor);
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
			npcSensor.OnNpcSeen += OnNpcSeen;
			npcSensor.OnNpcSightLost += OnNpcSightLost;
			hitSensor.OnHit += OnHit;
			health.OnDeath += OnDeath;
		}

		private void Update()
		{
			npcSensor.RemoveNullNpc();
			strategy.Act();
		}

		private void OnDisable()
		{
			npcSensor.OnNpcSeen -= OnNpcSeen;
			npcSensor.OnNpcSightLost -= OnNpcSightLost;
			hitSensor.OnHit -= OnHit;
			health.OnDeath -= OnDeath;
		}

		public void Configure(NpcStrategy strategy, Color color)
		{
			body.GetComponent<SpriteRenderer>().color = color;
			sight.GetComponent<SpriteRenderer>().color = color;

			switch (strategy)
			{
				case NpcStrategy.Careful:
					typeSign.GetComponent<SpriteRenderer>().sprite = carefulSprite;
					break;
				case NpcStrategy.Cowboy:
					typeSign.GetComponent<SpriteRenderer>().sprite = cowboySprite;
					break;
				case NpcStrategy.Camper:
					typeSign.GetComponent<SpriteRenderer>().sprite = camperSprite;
					break;
				default:
					typeSign.GetComponent<SpriteRenderer>().sprite = normalSprite;
					break;
			}
		}

		private void OnHit(int hitPoints)
		{
			Debug.Log("OW, I'm hurt! I am super the hurted!!!");

			health.Hit(hitPoints);
		}

		private void OnDeath()
		{
			Debug.Log("Yaaaaarggg....!! I died....GG.");

			destroyer.Destroy();
		}

		private void OnNpcSeen(NpcController npc)
		{
			Debug.Log("I've seen an ennemy!! Ya so dead noob!!!");
		}

		private void OnNpcSightLost(NpcController npc)
		{
			Debug.Log("I've lost sight of an ennemy...Yikes!!!");
		}
	}
}
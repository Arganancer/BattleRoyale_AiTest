using System;
using Playmode.Util.Collections;
using UnityEngine;
using Random = System.Random;

namespace Playmode.Pickable
{
	public class PickableSpawner : MonoBehaviour
	{
		private const string ZoneObjectName = "Zone";
		
		private ZoneController zoneController;

		private float worldSize;
		private Vector2 lastPickableCoordonate;
		private float minDistanceBetween2Pickable = 10;
		private int nbOfPickableToSpawn;
		[SerializeField] private int numberOfPickable =1;

		private static readonly TypePickable.TypePickable[] DefaultTypePickable =
		{
			TypePickable.TypePickable.Medicalkit,
			TypePickable.TypePickable.Shotgun,
			TypePickable.TypePickable.Uzi
		};

		[SerializeField] private GameObject pickablePrefab;

		private void Awake()
		{
			ValidateSerialisedFields();
			zoneController = GameObject.Find(ZoneObjectName).GetComponentInChildren<ZoneController>();
			worldSize = zoneController.CurrentRadius;
		}

		private void ValidateSerialisedFields()
		{
			if (pickablePrefab == null)
				throw new ArgumentException("Can't spawn null ennemy prefab.");
		}

		private void Start()
		{
			SelectNbOfPickableToSpawn();
			SpawnPickables();
		}

//		private void Update()
//		{
//			throw new System.NotImplementedException();
//		}

		private void SpawnPickables()
		{
			var pickableTypeProvider = new LoopingEnumerator<TypePickable.TypePickable>(DefaultTypePickable);
//			for (int i = 0; i < nbOfPickableToSpawn; ++i)
//			{
//				SpawnPickable(
//					CreateRandomCoordonate(),
//					pickableStragegyProvider.Next()
//				);
//			}
			// TEST LOOP
			for (int i = 0; i < numberOfPickable; ++i)
			{
				SpawnPickable(
					CreateRandomCoordonate(),
					pickableTypeProvider.Next()
				);
			}
		}

		private Vector2 CreateRandomCoordonate()
		{
			 Vector2 currentPickableCoordonate = new Vector2(UnityEngine.Random.Range(0, worldSize),
				UnityEngine.Random.Range(0, worldSize));
			while (Math.Abs(currentPickableCoordonate.x - lastPickableCoordonate.x) < minDistanceBetween2Pickable &&
			       Math.Abs(currentPickableCoordonate.y - lastPickableCoordonate.y) < minDistanceBetween2Pickable)
			{
				currentPickableCoordonate = new Vector2(UnityEngine.Random.Range(0, worldSize),
					UnityEngine.Random.Range(0, worldSize));
			}

			lastPickableCoordonate = currentPickableCoordonate;
			return currentPickableCoordonate;
		}

		private void SpawnPickable(Vector3 position, TypePickable.TypePickable strategy)
		{
			// TEST VARIABLE
			position = transform.position;
			strategy = TypePickable.TypePickable.Medicalkit;
			
			Instantiate(pickablePrefab, position, Quaternion.identity)
				.GetComponentInChildren<PickableController>()
				.Configure(strategy);
		}

		private void SelectNbOfPickableToSpawn()
		{
			nbOfPickableToSpawn = UnityEngine.Random.Range(1, 3);
		}
	}
}
using System;
using Playmode.Util.Collections;
using Playmode.World;
using UnityEngine;

namespace Playmode.Pickable
{
	public class PickableSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject pickablePrefab;
		
		private const string ZoneObjectName = "Zone";
		private const float TimeToSpawn = 7;
		private const float MinDistanceBetween2Pickable = 10;
		
		private ZoneController zoneController;

		private Vector2 worldSize;
		private Vector2 lastPickableCoordonate;
		private int nbOfPickableToSpawn = 5;
		private float timeLastSpawn;

		private static readonly TypePickable.TypePickable[] DefaultTypePickable =
		{
			TypePickable.TypePickable.Medicalkit,
			TypePickable.TypePickable.Shotgun,
			TypePickable.TypePickable.Uzi
		};

		private void Awake()
		{
			ValidateSerialisedFields();
			zoneController = GameObject.Find(ZoneObjectName).GetComponentInChildren<ZoneController>();
			
			if (zoneController.DistanceOffSet.x > 0)
			{
				worldSize = zoneController.DistanceOffSet*zoneController.CurrentRadius*7;
			}
			else
			{
				worldSize = new Vector2(zoneController.CurrentRadius*7,zoneController.CurrentRadius*7);
			}
			
			timeLastSpawn = 0;
		}

		private void ValidateSerialisedFields()
		{
			if (pickablePrefab == null)
				throw new ArgumentException("Can't spawn null pickable prefab.");
		}

		private void Start()
		{
			SpawnPickables();
			nbOfPickableToSpawn = 1;
		}

		private void Update()
		{
			if (Time.time - timeLastSpawn > TimeToSpawn && zoneController.ZoneIsNotShrinking)
			{
				if (zoneController.DistanceOffSet.x > 0)
				{
					worldSize = zoneController.DistanceOffSet*zoneController.CurrentRadius*7;
				}
				else
				{
					worldSize = new Vector2(zoneController.CurrentRadius*7,zoneController.CurrentRadius*7);
				}
				
				SpawnPickables();
				timeLastSpawn = Time.time;
			}
		}

		private void SpawnPickables()
		{
			var pickableTypeProvider = new LoopingEnumerator<TypePickable.TypePickable>(DefaultTypePickable);
			
			for (int i = 0; i < nbOfPickableToSpawn; ++i)
			{
				CreatePickable(
					CreateRandomCoordonate(),
					pickableTypeProvider.Next()
				);
			}
		}

		private Vector2 CreateRandomCoordonate()
		{
			int nbOfTry = 0;
			
			Vector2 currentPickableCoordonate = new Vector2(UnityEngine.Random.Range(0, worldSize.x),
				UnityEngine.Random.Range(0, worldSize.x));
			
			while (Math.Abs(currentPickableCoordonate.x - lastPickableCoordonate.x) < MinDistanceBetween2Pickable &&
			       Math.Abs(currentPickableCoordonate.y - lastPickableCoordonate.y) < MinDistanceBetween2Pickable || 
			       nbOfTry <5)
			{
				currentPickableCoordonate = new Vector2(UnityEngine.Random.Range(-worldSize.x, worldSize.x),
					UnityEngine.Random.Range(-worldSize.x, worldSize.x));
				
				nbOfTry++;
			}

			lastPickableCoordonate = currentPickableCoordonate;
			
			return currentPickableCoordonate;
		}

		private void CreatePickable(Vector3 position, TypePickable.TypePickable strategy)
		{
			GameObject test = Instantiate(pickablePrefab, position, Quaternion.identity);
				test.GetComponentInChildren<PickableController>()
				.Configure(strategy);
		}
	}
}
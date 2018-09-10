using System;
using System.Collections.Generic;
using Playmode.Npc.Strategies.BaseStrategyClasses;
using Playmode.Util.Collections;
using Playmode.Util.Values;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playmode.Npc
{
	public class NpcSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject npcPrefab;
		
		private static readonly NpcStrategy[] DefaultStrategies =
		{
			NpcStrategy.Normal,
			NpcStrategy.Careful,
			NpcStrategy.Cowboy,
			NpcStrategy.Camper,
			NpcStrategy.Op
		};

		private void Awake()
		{
			ValidateSerialisedFields();
		}

		private void Start()
		{
			SpawnNpcs();
		}

		private void ValidateSerialisedFields()
		{
			if (npcPrefab == null)
				throw new ArgumentException("Can't spawn null ennemy prefab.");
		}

		private void SpawnNpcs()
		{
			var stragegyProvider = new LoopingEnumerator<NpcStrategy>(DefaultStrategies);
			var spawnsPoints = GenerateSpawnPoints();

			for (var i = 0; i < GameValues.NbOfEnemies; i++)
				SpawnNpc(
					spawnsPoints[i],
					stragegyProvider.Next()
				);
		}

		private static List<Vector3> GenerateSpawnPoints()
		{
			var spawnPoints = new List<Vector3>();
			
			//BEN_CORRECTION : Valeures magiques. Devraient être en SerializedField.
			//				   Aussi, en général, constantes sont en haut de fichier.
			const float maxDistanceFromMapCenter = 100f;
			const float minDistanceBetweenNpcs = 30f;

			//BEN_REVIEW : C'est pas élégant comme algo. Vous auriez pu utiliser un algorithme de "Noise" pour faire à peu
			//près la même chose, mais beaucoup plus rapidement et ... plus élégant.
			
			//BEN_CORRECTION : J'aurais sorti cet algorithme dans une fonction statique.
			//				   Ex : public static List<Vector3> GenerateEvenlyDistributedPositions(Vector3 center, float radius);
			
			var currentSpawnPoint = GeneratePointWithinPlayableArea(maxDistanceFromMapCenter);
			spawnPoints.Add(currentSpawnPoint);

			for (var i = 0; i < GameValues.NbOfEnemies - 1; i++)
			{
				var j = 0;
				var positionTooClose = false;
				
				do
				{
					j += 1;
					positionTooClose = false;
					currentSpawnPoint = GeneratePointWithinPlayableArea(maxDistanceFromMapCenter);
					
					foreach (var spawnPoint in spawnPoints)
					{
						if (!(Vector3.Distance(spawnPoint, currentSpawnPoint) < minDistanceBetweenNpcs)) continue;
						positionTooClose = true;
					}
					
				} while (positionTooClose && j < 100);

				spawnPoints.Add(currentSpawnPoint);
			}

			return spawnPoints;
		}

		private static Vector3 GeneratePointWithinPlayableArea(float maxDistanceFromMapCenter)
		{
			Vector3 position = Random.insideUnitCircle;
			
			return position * CRandom.Nextf(0f, maxDistanceFromMapCenter);
		}

		private void SpawnNpc(Vector3 position, NpcStrategy strategy)
		{
			Instantiate(npcPrefab, position, Quaternion.identity)
				.GetComponentInChildren<NpcController>()
				.Configure(strategy);
		}
	}
}
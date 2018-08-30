using System;
using Playmode.Npc.Strategies;
using Playmode.Util.Collections;
using UnityEngine;

namespace Playmode.Npc
{
	public class NpcSpawner : MonoBehaviour
	{
		// TODO: Remove (Debug Field)
		[SerializeField] private int nbOfEnemies;

		private static readonly Color[] DefaultColors =
		{
			Color.white, Color.black, Color.blue, Color.cyan, Color.green,
			Color.magenta, Color.red, Color.yellow, new Color(255, 125, 0, 255)
		};

		private static readonly NpcStrategy[] DefaultStrategies =
		{
			NpcStrategy.Normal,
			NpcStrategy.Careful,
			NpcStrategy.Cowboy,
			NpcStrategy.Camper
		};

		[SerializeField] private GameObject npcPrefab;
		[SerializeField] private Color[] colors = DefaultColors;

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
			if (colors == null || colors.Length == 0)
				throw new ArgumentException("Ennemies needs colors to be spawned.");
			if (transform.childCount <= 0)
				throw new ArgumentException("Can't spawn ennemis whitout spawn points. " +
				                            "Create chilldrens for this GameObject as spawn points.");
		}

		private void SpawnNpcs()
		{
			var stragegyProvider = new LoopingEnumerator<NpcStrategy>(DefaultStrategies);
			var colorProvider = new LoopingEnumerator<Color>(colors);

			// TODO: Remove (Debug variable)
			if (nbOfEnemies > transform.childCount)
				nbOfEnemies = transform.childCount;

			for (var i = 0; i < nbOfEnemies /* <-- TODO: Change for transform.childCount */; i++)
				SpawnNpc(
					transform.GetChild(i).position,
					//stragegyProvider.Next(),
					NpcStrategy.Normal,
					colorProvider.Next()
				);
		}

		private void SpawnNpc(Vector3 position, NpcStrategy strategy, Color color)
		{
			Instantiate(npcPrefab, position, Quaternion.identity)
				.GetComponentInChildren<NpcController>()
				.Configure(strategy, color);
		}
	}
}
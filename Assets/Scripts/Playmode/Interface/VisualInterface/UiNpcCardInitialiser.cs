using System;
using System.Collections;
using System.Collections.Generic;
using Playmode.Npc;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Interface.VisualInterface
{
	public class UiNpcCardInitialiser : MonoBehaviour
	{
		[SerializeField] private GameObject detailedNpcCard;
		private List<GameObject> npcs;

		private void Awake()
		{
			ValidateSerializedFields();
			InitialiseComponents();
		}

		private void InitialiseComponents()
		{
			npcs = new List<GameObject>();
		}

		private void ValidateSerializedFields()
		{
			if (detailedNpcCard == null)
				throw new Exception("Can't instantiate null detailedNpcCards");
		}

		private void Start()
		{
			StartCoroutine(LateStart(0.01f));
		}

		private IEnumerator LateStart(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			npcs.AddRange(GameObject.FindGameObjectsWithTag(Tags.Npc));
			CreateDetailedNpcCards();
		}

		private void CreateDetailedNpcCards()
		{
			foreach (var npc in npcs)
			{
				CreateDetailedNpcCard(npc.GetComponentInChildren<NpcController>());
			}
		}

		private void CreateDetailedNpcCard(NpcController npcController)
		{
			var newDetailedNpcCard = Instantiate(detailedNpcCard);
			newDetailedNpcCard.GetComponentInChildren<UiNpcCardController>()
				.Configure(npcController);
			newDetailedNpcCard.transform.parent = transform;
		}
	}
}
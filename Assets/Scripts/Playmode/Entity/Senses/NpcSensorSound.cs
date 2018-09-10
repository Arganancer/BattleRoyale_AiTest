using System.Collections.Generic;
using System.Linq;
using Playmode.Npc;
using Playmode.Npc.BodyParts;
using UnityEngine;

namespace Playmode.Entity.Senses
{
	public class NpcSensorSound : MonoBehaviour
	{
		[SerializeField] private float timeUntilSoundInfoOutdated = 7.5f;
		[SerializeField] private float maxDistanceToSoundPosition = 40f;
		
		//BEN_REVIEW : Une liste aurait pas fait l'affaire ? Une liste de struct ?
		public IReadOnlyDictionary<float, Vector3> SoundsInformations => soundsInformations;
		
		private SortedDictionary<float, Vector3> soundsInformations;
		private List<NpcController> npcControllers;

		private void Awake()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			soundsInformations = new SortedDictionary<float, Vector3>();
			npcControllers = new List<NpcController>();
		}

		//BEN_CORRECTION : Pourquoi est-ce qu'il ne s'update pas tout seul ?
		public void UpdateSoundSensor(Vector3 npcCurrentPosition, Vector3 npcCurrentRotation)
		{
			UpdateSoundInformation(npcCurrentPosition, npcCurrentRotation);
		}

		private void UpdateSoundInformation(Vector3 npcCurrentPosition, Vector3 npcCurrentRotation)
		{
			//BEN_REVIEW : Aussi faisable avec "RemoveWhere" ou une reverse for loop.
			var outdatedSoundInformation = new Dictionary<float, Vector3>();

			foreach (var soundInformation in soundsInformations)
			{
				// Remove sound information as it is too old.
				if (soundInformation.Key < Time.time - timeUntilSoundInfoOutdated)
				{
					outdatedSoundInformation.Add(soundInformation.Key, soundInformation.Value);
				}
				// Remove sound information as it is now too far away.
				else if (Vector3.Magnitude(soundInformation.Value - npcCurrentPosition) > maxDistanceToSoundPosition)
				{
					outdatedSoundInformation.Add(soundInformation.Key, soundInformation.Value);
				}
				// Remove sound information as it has now been investigated
				else if (Vector3.Angle(npcCurrentRotation, soundInformation.Value - npcCurrentPosition) < 10f &&
				         Vector3.Magnitude(soundInformation.Value - npcCurrentPosition) < 10f)
				{
					outdatedSoundInformation.Add(soundInformation.Key, soundInformation.Value);
				}
				else if (Vector3.Distance(soundInformation.Value, npcCurrentPosition) < 2f)
				{
					outdatedSoundInformation.Add(soundInformation.Key, soundInformation.Value);
				}
			}

			foreach (var outdatedSound in outdatedSoundInformation)
			{
				soundsInformations.Remove(outdatedSound.Key);
			}
		}

		public Vector3 GetNewestSoundPosition()
		{
			return SoundsInformations.Values.Last();
		}

		public void EnterSoundRange(NpcController npc)
		{
			//BEN_CORRECTION : Wrong data flow. Tel que mentionné dans le code review.
			npc.transform.root.GetComponentInChildren<HandController>().OnWeaponFired += OnWeaponFired;
			npcControllers.Add(npc);
		}

		public void LeaveSoundRange(NpcController npc)
		{
			npc.transform.root.GetComponentInChildren<HandController>().OnWeaponFired -= OnWeaponFired;
			npcControllers.Remove(npc);
		}

		private void OnDisable()
		{
			npcControllers.RemoveAll(npcController => npcController == null);
			
			foreach (var npcController in npcControllers)
			{
				npcController.transform.root.GetComponentInChildren<HandController>().OnWeaponFired -= OnWeaponFired;
			}
		}

		private void OnWeaponFired(Vector3 positionWhenFired)
		{
			var adjustedTime = Time.time;
			
			while (soundsInformations.ContainsKey(adjustedTime))
			{
				adjustedTime += 0.0001f;
			}

			soundsInformations.Add(adjustedTime, positionWhenFired);
		}
	}
}
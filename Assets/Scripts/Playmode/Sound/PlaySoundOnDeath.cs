using Playmode.Event;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Sound
{
	public class PlaySoundOnDeath : MonoBehaviour
	{
		private NpcDeathEventChannel npcDeathEventChannel;
		private AudioSource audioSource;

		private void Awake()
		{
			npcDeathEventChannel = GameObject.FindWithTag(Tags.GameController).GetComponent<NpcDeathEventChannel>();
			var aSources = GetComponents(typeof(AudioSource));
			audioSource = (AudioSource) aSources[1];
		}

		private void OnEnable()
		{
			npcDeathEventChannel.OnEventPublished += PlaySound;
		}

		private void OnDisable()
		{
			npcDeathEventChannel.OnEventPublished -= PlaySound;
		}

		private void PlaySound()
		{
			audioSource.Play();
		}
	}
}
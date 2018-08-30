using Playmode.Event;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Sound
{
	public class SoundController : MonoBehaviour
	{
		private ShootEventChannel shootEventChannel;
		private NpcDeathEventChannel npcDeathEventChannel;
		private HitEventChannel hitEventChannel;

		private AudioSource shootSound;
		private AudioSource deathSound;
		private AudioSource hitSound;
		
		private void Awake()
		{
			shootEventChannel = GameObject.FindWithTag(Tags.SoundController).GetComponent<ShootEventChannel>();
			npcDeathEventChannel = GameObject.FindWithTag(Tags.SoundController).GetComponent<NpcDeathEventChannel>();
			hitEventChannel = GameObject.FindWithTag(Tags.SoundController).GetComponent<HitEventChannel>();

			var sources = GetComponents(typeof(AudioSource));
			shootSound = (AudioSource) sources[0];
			deathSound = (AudioSource) sources[1];
			hitSound = (AudioSource) sources[2];
		}

		private void OnEnable()
		{
			shootEventChannel.OnEventPublished += PlayShootSound;
			npcDeathEventChannel.OnEventPublished += PlayDeathSound;
			hitEventChannel.OnEventPublished += PlayHitSound;
		}

		private void OnDisable()
		{
			shootEventChannel.OnEventPublished -= PlayShootSound;
			npcDeathEventChannel.OnEventPublished -= PlayDeathSound;
			hitEventChannel.OnEventPublished -= PlayHitSound;
		}

		private void PlayShootSound()
		{
			shootSound.Play();
		}

		private void PlayDeathSound()
		{
			deathSound.Play();
		}

		private void PlayHitSound()
		{
			hitSound.Play();
		}
	}
}
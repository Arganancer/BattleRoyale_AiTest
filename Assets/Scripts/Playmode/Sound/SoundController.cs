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

		private AudioSource[] starterWeaponShootSounds;
		private AudioSource[] uziShootSounds;
		private AudioSource shotgunShootSound;
		private AudioSource deathSound;
		private AudioSource hitSound;

		private void Awake()
		{
			shootEventChannel = GameObject.FindWithTag(Tags.SoundController).GetComponent<ShootEventChannel>();
			npcDeathEventChannel = GameObject.FindWithTag(Tags.SoundController).GetComponent<NpcDeathEventChannel>();
			hitEventChannel = GameObject.FindWithTag(Tags.SoundController).GetComponent<HitEventChannel>();

			var sources = GetComponents(typeof(AudioSource));
			starterWeaponShootSounds = new[]
			{
				(AudioSource) sources[0],
				(AudioSource) sources[1],
				(AudioSource) sources[2]
			};
			uziShootSounds = new[]
			{
				(AudioSource) sources[3],
				(AudioSource) sources[4]
			};
			shotgunShootSound = (AudioSource) sources[5];
			deathSound = (AudioSource) sources[6];
			hitSound = (AudioSource) sources[7];
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
			starterWeaponShootSounds[CRandom.Next(0, 2)].Play();
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
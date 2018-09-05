using Playmode.Event;
using Playmode.Util.Values;
using UnityEngine;

namespace Playmode.Sound
{
	public class PlaySoundOnHit : MonoBehaviour
	{
		private HitEventChannel hitEventChannel;
		private AudioSource audioSource;

		private void Awake()
		{
			hitEventChannel = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<HitEventChannel>();
			
			var aSources = GetComponents(typeof(AudioSource));
			audioSource = (AudioSource) aSources[7];
		}

		private void OnEnable()
		{
			hitEventChannel.OnEventPublished += PlaySound;
		}

		private void OnDisable()
		{
			hitEventChannel.OnEventPublished -= PlaySound;
		}

		private void PlaySound()
		{
			audioSource.Play();
		}
	}
}